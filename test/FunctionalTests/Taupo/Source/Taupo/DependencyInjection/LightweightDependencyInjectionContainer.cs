//---------------------------------------------------------------------
// <copyright file="LightweightDependencyInjectionContainer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DependencyInjection
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.ServiceModel;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Lightweight implementation of basic dependency injection container with constructor
    /// injection.
    /// </summary>
    public class LightweightDependencyInjectionContainer : DependencyInjectionContainer, IDependencyInjector
    {
        private static object constructingMarker = new object();

        private int containerdepth = 0;
        private Dictionary<Type, List<WeakReference>> objectIDs = new Dictionary<Type, List<WeakReference>>();
        private Dictionary<Type, IResolutionStrategy> resolutionStrategies = new Dictionary<Type, IResolutionStrategy>();
        private Dictionary<Type, DependencyResolutionOptions> resolutionOptions = new Dictionary<Type, DependencyResolutionOptions>();
        private Dictionary<Type, object> injectedDependencies = new Dictionary<Type, object>();
        private Dictionary<Type, object> locallyCreatedDependencies = new Dictionary<Type, object>();
        private Dictionary<object, Type> dependencyToContract = new Dictionary<object, Type>();
        private Dictionary<object, List<WeakReference>> dependencies = new Dictionary<object, List<WeakReference>>();

        /// <summary>
        /// Initializes a new instance of the LightweightDependencyInjectionContainer class.
        /// </summary>
        public LightweightDependencyInjectionContainer()
        {
        }

        /// <summary>
        /// Initializes a new instance of the LightweightDependencyInjectionContainer class.
        /// </summary>
        /// <param name="parentContainer">The parent container.</param>
        public LightweightDependencyInjectionContainer(LightweightDependencyInjectionContainer parentContainer)
            : this()
        {
            this.containerdepth = parentContainer.containerdepth + 1;
            this.resolutionStrategies = new Dictionary<Type, IResolutionStrategy>(parentContainer.resolutionStrategies);
            this.injectedDependencies = new Dictionary<Type, object>(parentContainer.injectedDependencies);
            foreach (var kvp in parentContainer.locallyCreatedDependencies.ToList())
            {
                this.injectedDependencies.Add(kvp.Key, kvp.Value);
            }            
            
            this.dependencyToContract = new Dictionary<object, Type>(parentContainer.dependencyToContract);
            this.dependencies = new Dictionary<object, List<WeakReference>>(parentContainer.dependencies);
            this.resolutionOptions = new Dictionary<Type, DependencyResolutionOptions>(parentContainer.resolutionOptions);
            this.objectIDs = parentContainer.objectIDs;
            this.Logger = parentContainer.Logger;

            foreach (var param in parentContainer.TestParameters)
            {
                this.TestParameters.Add(param);
            }
        }

        /// <summary>
        /// Registers a singleton object as an implementation for all contracts implemented by this instance.
        /// </summary>
        /// <param name="contractType">Contract type.</param>
        /// <param name="instance">The instance.</param>
        public override void RegisterInstance(Type contractType, object instance)
        {
            ExceptionUtilities.CheckArgumentNotNull(contractType, "contractType");
            ExceptionUtilities.CheckArgumentNotNull(instance, "instance");

            foreach (Type interfaceContractType in instance.GetType().GetInterfaces().Where(iType => !iType.Namespace.Contains("System")))
            {
                this.SetResolutionStrategy(interfaceContractType, new SingletonResolutionStrategy(instance));
                this.Resolve(interfaceContractType);
            }

            for (Type classContractType = instance.GetType(); classContractType != typeof(object); classContractType = classContractType.GetBaseType())
            {
                this.SetResolutionStrategy(classContractType, new SingletonResolutionStrategy(instance));
                this.Resolve(classContractType);
            }
        }

        /// <summary>
        /// Register <paramref name="implementationType"/> as an implementation for <paramref name="contractType"/>.
        /// </summary>
        /// <param name="contractType">Contract type.</param>
        /// <param name="implementationType">Implementation type.</param>
        /// <returns>Instance of <see cref="DependencyResolutionOptions"/> which can be used to further configure the resolution process.</returns>
        public override DependencyResolutionOptions Register(Type contractType, Type implementationType)
        {
            return this.SetResolutionStrategy(contractType, new RecursiveResolutionStrategy(implementationType));
        }

        /// <summary>
        /// Registers the custom resolver.
        /// </summary>
        /// <param name="contractType">Contract type.</param>
        /// <param name="constructorFunction">The custom resolver.</param>
        /// <returns>Instance of <see cref="DependencyResolutionOptions"/> which can be used to further configure the resolution process.</returns>
        public override DependencyResolutionOptions RegisterCustomResolver(Type contractType, Func<Type, object> constructorFunction)
        {
            return this.SetResolutionStrategy(contractType, new CustomResolutionStrategy(constructorFunction));
        }

        /// <summary>
        /// Attempts to resolve concrete type for a given contract type, and if possible,
        /// constructs and returns the instance.
        /// </summary>
        /// <param name="targetType">Type of the object that requires this dependency (can be null).</param>
        /// <param name="contractType">The contract type.</param>
        /// <param name="result">Variable to be assigned with the constructed object reference.</param>
        /// <returns>
        /// true if object construction was successful, false otherwise
        /// </returns>
        public override bool TryResolve(Type targetType, Type contractType, out object result)
        {
            return this.TryCreateObject(targetType, contractType, false, out result);
        }

        /// <summary>
        /// Determines whether this instance can resolve the specified contract type.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>
        /// <c>true</c> if this instance can resolve the specified contract type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanResolve(Type contractType)
        {
            return this.resolutionStrategies.ContainsKey(contractType);
        }

        /// <summary>
        /// Resolves dependencies for all properties of a given object marked with [InjectDependency]
        /// </summary>
        /// <typeparam name="TObject">Type of the target object.</typeparam>
        /// <param name="targetObject">The target object.</param>
        /// <returns>Target object.</returns>
        public override TObject InjectDependenciesInto<TObject>(TObject targetObject)
        {
            if (targetObject != null)
            {
                this.InjectPropertyDependencies(targetObject);
                this.InjectTestArguments(targetObject);
            }

            return targetObject;
        }

        /// <summary>
        /// Creates the sub container.
        /// </summary>
        /// <returns>Subcontainer which inherits dependency definitions</returns>
        public override DependencyInjectionContainer CreateInheritedContainer()
        {
            return new LightweightDependencyInjectionContainer(this);
        }

        internal void RegisterDependency(object principalObject, object dependentObject)
        {
            List<WeakReference> principals;

            if (!this.dependencies.TryGetValue(dependentObject, out principals))
            {
                principals = new List<WeakReference>();
                this.dependencies.Add(dependentObject, principals);
            }

            if (!principals.Any(wr => wr.Target == principalObject))
            {
                this.Logger.WriteLine(LogLevel.Trace, "  {0} is using {1}", this.GetObjectName(principalObject), this.GetObjectName(dependentObject));
                principals.Add(new WeakReference(principalObject));
            }
        }

        internal bool TryCreateObject(Type targetType, Type contractType, bool throwOnError, out object value)
        {
            this.Logger.WriteLine(LogLevel.Trace, "Lightweight Dependency Injector[depth:{0}].Try Create Object(target type:'{1}', contract type:'{2}', throw On Error:'{3}' out value)", this.containerdepth, targetType == null ? "null" : targetType.FullName, contractType.FullName, throwOnError.ToString());

            if (this.injectedDependencies.TryGetValue(contractType, out value) || this.locallyCreatedDependencies.TryGetValue(contractType, out value))
            {
                if (value != constructingMarker)
                {
                    return true;
                }
                else
                {
                    if (throwOnError)
                    {
                        throw new TaupoInvalidOperationException("Attempt to recursively construct " + contractType);
                    }

                    return false;
                }
            }

            IResolutionStrategy strategy;

            if (!this.resolutionStrategies.TryGetValue(contractType, out strategy))
            {
                if (contractType.IsAbstract() || contractType.IsInterface())
                {
                    if (throwOnError)
                    {
                        string message = "No implementation defined for " + contractType;
                        if (targetType != null)
                        {
                            message += " (dependency of " + targetType.FullName + ")";
                        }

                        throw new TaupoInvalidOperationException(message);
                    }

                    return false;
                }
                else
                {
                    // otherwise try to create the contract type itself
                    strategy = new RecursiveResolutionStrategy(contractType);
                }
            }

            this.locallyCreatedDependencies.Add(contractType, constructingMarker);
            try
            {
                ResolutionContext context = new ResolutionContext
                {
                    Container = this,
                    TargetType = targetType,
                    ThrowOnError = false,
                };

                if (strategy.TryResolve(context, out value))
                {
                    this.AddConstructedObject(contractType, value);
                    return true;
                }
                else if (throwOnError)
                {
                    // will throw
                    context.ThrowOnError = true;
                    strategy.TryResolve(context, out value);
                }
            }
            finally
            {
                if (this.locallyCreatedDependencies[contractType] == constructingMarker)
                {
                    this.locallyCreatedDependencies.Remove(contractType);
                }
            }

            return false;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                var disposableInstances = this.locallyCreatedDependencies.Select(kvp => kvp.Value).Distinct().OfType<IDisposable>().ToList();
                foreach (IDisposable disposable in disposableInstances)
                {
                    if (disposable != this)
                    {
                        this.Logger.WriteLine(LogLevel.Trace, "Disposing of {0}", this.GetObjectName(disposable));
                        disposable.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Resolves concrete type for a given contract type, constructs and returns the instance.
        /// </summary>
        /// <param name="targetType">Type of the object that requires this dependency (can be null).</param>
        /// <param name="contractType">Contract type.</param>
        /// <returns>
        /// Fully resolved object that implements the contract
        /// </returns>
        protected override object Resolve(Type targetType, Type contractType)
        {
            object value;

            this.TryCreateObject(targetType, contractType, true, out value);
            return value;
        }

        private void InjectTestArguments(object targetObject)
        {
            foreach (PropertyInfo pi in targetObject.GetType().GetProperties().Where(c => c.IsDefined(typeof(InjectTestParameterAttribute), false)))
            {
                var testParameterAttributes = pi.GetCustomAttributes(false).OfType<InjectTestParameterAttribute>().ToList();
                var nonObsoleteAttributes = testParameterAttributes.Where(a => !a.IsObsolete).ToList();
                
                ExceptionUtilities.Assert(nonObsoleteAttributes.Count <= 1, "At most 1 non-obsolete InjectTestParameterAttribute expected");
                var nonObsoleteAttribute = nonObsoleteAttributes.SingleOrDefault();

                foreach (InjectTestParameterAttribute tpa in testParameterAttributes)
                {
                    string attributeName = tpa.ParameterName;

                    string ltmPropertyName = attributeName;
                    string value;

                    if (this.TestParameters.TryGetValue(ltmPropertyName, out value))
                    {
                        if (tpa.IsObsolete)
                        { 
                            if (nonObsoleteAttribute != null)
                            {
                                this.Logger.WriteLine(LogLevel.Warning, "Using obsolete test-parameter '{0}'. Consider using '{1}' instead.", tpa.ParameterName, nonObsoleteAttribute.ParameterName);
                            }
                            else 
                            {
                                this.Logger.WriteLine(LogLevel.Warning, "Using obsolete test-parameter '{0}' which has no obvious alternative.", tpa.ParameterName);
                            }
                        }

                        try
                        {
                            object convertedValue = this.ConvertValue(pi.PropertyType, value);
                            pi.SetValue(targetObject, convertedValue, null);
                        }
                        catch (Exception ex)
                        {
                            throw new TaupoInfrastructureException("Cannot convert '" + value + "' to type " + pi.PropertyType + " (alias parameter " + ltmPropertyName + ", target=" + pi.DeclaringType + "." + pi.Name + ")", ex);
                        }
                    }
                }
            }
        }

        private object ConvertValue(Type propertyType, string value)
        {
            if (value == null)
            {
                return null;
            }

            propertyType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

            if (propertyType.IsEnum())
            {
                return Enum.Parse(propertyType, value, true);
            }

            // primarily for System.Uri, but may be useful for other types
            if (propertyType.GetInstanceConstructor(true, new[] { typeof(string) }) != null)
            {
                return Activator.CreateInstance(propertyType, new object[] { value });
            }

            return Convert.ChangeType(value, propertyType, CultureInfo.InvariantCulture);
        }

        private void InjectPropertyDependencies(object targetObject)
        {
            foreach (PropertyInfo pi in targetObject.GetType().GetProperties().Where(p => p.IsDefined(typeof(InjectDependencyAttribute), false)))
            {
                InjectDependencyAttribute ida = PlatformHelper.GetCustomAttribute<InjectDependencyAttribute>(pi, false);

                object resolvedValue;

                if (ida.IsRequired)
                {
                    resolvedValue = this.Resolve(targetObject.GetType(), pi.PropertyType);
                }
                else
                {
                    if (!this.TryResolve(targetObject.GetType(), pi.PropertyType, out resolvedValue))
                    {
                        continue;
                    }
                }

                pi.SetValue(targetObject, resolvedValue, null);
                this.RegisterDependency(targetObject, resolvedValue);
            }
        }

        private void AddConstructedObject(Type contractType, object value)
        {
            this.Logger.WriteLine(LogLevel.Trace, "Resolved contract {0} as {1}", contractType.Name, this.GetObjectName(value));
            DependencyResolutionOptions options;

            if (this.resolutionOptions.TryGetValue(contractType, out options))
            {
                if (!options.IsTransient)
                {
                    this.locallyCreatedDependencies[contractType] = value;
                    if (value != null)
                    {
                        this.dependencyToContract[value] = contractType;
                    }
                }
            }

            this.InjectDependenciesInto(value);
        }

        private DependencyResolutionOptions SetResolutionStrategy(Type contractType, IResolutionStrategy resolutionStrategy)
        {
            object constructedObject;

            if (this.resolutionStrategies.ContainsKey(contractType))
            {
                if (this.resolutionStrategies[contractType].Equals(resolutionStrategy))
                {
                    return this.resolutionOptions[contractType];
                }
            }

            if (this.locallyCreatedDependencies.TryGetValue(contractType, out constructedObject) || this.injectedDependencies.TryGetValue(contractType, out constructedObject))
            {
                this.InvalidateConstructedObjects(constructedObject);
            }

            this.resolutionStrategies[contractType] = resolutionStrategy;
            var options = new DependencyResolutionOptions();
            this.resolutionOptions[contractType] = options;
            return options;
        }

        private void InvalidateConstructedObjects(object constructedObject)
        {
            if (constructedObject != null)
            {
                this.InvalidateObjects(constructedObject, this.locallyCreatedDependencies);
                this.InvalidateObjects(constructedObject, this.injectedDependencies);
            }
        }

        private void InvalidateObjects(object constructedObject, Dictionary<Type, object> dependencyDictionary)
        {
            var allContractsImplementedByInstance = dependencyDictionary.Where(kvp => kvp.Value == constructedObject).Select(kvp => kvp.Key).ToList();
            foreach (Type contractType in allContractsImplementedByInstance)
            {
                if (dependencyDictionary.Remove(contractType))
                {
                    this.Logger.WriteLine(LogLevel.Trace, "Invalidated {0} because {1} has been invalidated", contractType.Name, this.GetObjectName(constructedObject));
                }

                this.dependencyToContract.Remove(constructedObject);

                List<WeakReference> deps;

                if (this.dependencies.TryGetValue(constructedObject, out deps))
                {
                    this.dependencies.Remove(deps);
                    foreach (WeakReference principal in deps)
                    {
                        this.InvalidateConstructedObjects(principal.Target);
                    }
                }
            }
        }

        private string GetObjectName(object obj)
        {
            List<WeakReference> idForType;

            if (!this.objectIDs.TryGetValue(obj.GetType(), out idForType))
            {
                idForType = new List<WeakReference>();
                this.objectIDs.Add(obj.GetType(), idForType);
            }

            for (int i = 0; i < idForType.Count; ++i)
            {
                if (idForType[i].Target == obj)
                {
                    return obj.GetType().Name + "#" + i;
                }
            }

            int cnt = idForType.Count;
            idForType.Add(new WeakReference(obj));
            return obj.GetType().Name + "#" + cnt;
        }
    }
}

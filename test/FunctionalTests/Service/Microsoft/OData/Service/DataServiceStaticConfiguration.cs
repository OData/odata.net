//---------------------------------------------------------------------
// <copyright file="DataServiceStaticConfiguration.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.OData.Service.Configuration;
    using Microsoft.OData.Service.Providers;

    #endregion

    /// <summary>
    /// Static configuration that is initialized one time per service type and then cached.
    /// </summary>
    internal class DataServiceStaticConfiguration
    {
        /// <summary>
        /// A lookup of resource sets to the corresponding QueryInterceptors.
        /// For IDSP there is no guarantee that the provider will always return the same metadata instance.  We should
        /// use the name instead of the instance as key since the configuration is cached across requests.
        /// </summary>
        private readonly Dictionary<string, List<MethodInfo>> readAuthorizationMethods;

        /// <summary>
        /// A lookup of resource sets to the corresponding ChangeInterceptors.
        /// For IDSP there is no guarantee that the provider will always return the same metadata instance.  We should
        /// use the name instead of the instance as key since the configuration is cached across requests.
        /// </summary>
        private readonly Dictionary<string, List<MethodInfo>> writeAuthorizationMethods;

        /// <summary>The provider for the web service.</summary>
        private IDataServiceMetadataProvider provider;

        /// <summary>
        /// Constructors the static configuration object which can be cached for the whole AppDomain lifecycle.
        /// </summary>
        /// <param name="dataServiceType">Service type.</param>
        /// <param name="provider">Metadata provider instance.</param>
        internal DataServiceStaticConfiguration(Type dataServiceType, IDataServiceMetadataProvider provider)
        {
            this.provider = provider;
            this.readAuthorizationMethods = new Dictionary<string, List<MethodInfo>>(EqualityComparer<string>.Default);
            this.writeAuthorizationMethods = new Dictionary<string, List<MethodInfo>>(EqualityComparer<string>.Default);
            this.RegisterCallbacks(dataServiceType);
            this.LoadConfigurationSettings();
        }

        /// <summary>
        /// Cached copy of DataServicesFeaturesSection.
        /// </summary>
        internal DataServicesFeaturesSection DataServicesFeaturesSection
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns the list of QueryInterceptors for the given resource set
        /// </summary>
        /// <param name="resourceSet">resource set instance</param>
        /// <returns>List of QueryInterceptors for the resource set, null if there is none defined for the resource set.</returns>
        internal MethodInfo[] GetReadAuthorizationMethods(ResourceSet resourceSet)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");

            List<MethodInfo> methods;
            if (this.readAuthorizationMethods.TryGetValue(resourceSet.Name, out methods))
            {
                return methods.ToArray();
            }

            return null;
        }

        /// <summary>
        /// Returns the list of ChangeInterceptors for the given resource set
        /// </summary>
        /// <param name="resourceSet">resource set instance</param>
        /// <returns>List of ChangeInterceptors for the resource set, null if there is none defined for the resource set.</returns>
        internal MethodInfo[] GetWriteAuthorizationMethods(ResourceSet resourceSet)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");

            List<MethodInfo> methods;
            if (this.writeAuthorizationMethods.TryGetValue(resourceSet.Name, out methods))
            {
                return methods.ToArray();
            }

            return null;
        }

        /// <summary>Checks that the specified <paramref name="method"/> has a correct signature.</summary>
        /// <param name="dataServiceType">Service type.</param>
        /// <param name="method">Method to check.</param>
        /// <param name="container">Container associated with the interceptor.</param>
        private static void CheckQueryInterceptorSignature(Type dataServiceType, MethodInfo method, ResourceSet container)
        {
            Debug.Assert(dataServiceType != null, "dataServiceType != null");
            Debug.Assert(method != null, "method != null");
            Debug.Assert(container != null, "container != null");

            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length != 0)
            {
                throw new InvalidOperationException(Strings.DataService_QueryInterceptorIncorrectParameterCount(method.Name, dataServiceType.FullName, parameters.Length));
            }

            Type lambdaType = typeof(Func<,>).MakeGenericType(container.ResourceType.InstanceType, typeof(bool));
            Type expectedReturnType = typeof(Expression<>).MakeGenericType(lambdaType);
            Type returnType = method.ReturnType;
            if (returnType == typeof(void))
            {
                throw new InvalidOperationException(Strings.DataService_AuthorizationMethodVoid(method.Name, dataServiceType.FullName, expectedReturnType));
            }

            if (!expectedReturnType.IsAssignableFrom(returnType))
            {
                Type nullableLambdaType = typeof(Func<,>).MakeGenericType(container.ResourceType.InstanceType, typeof(bool?));
                if (!(typeof(Expression<>).MakeGenericType(nullableLambdaType).IsAssignableFrom(returnType)))
                {
                    throw new InvalidOperationException(
                        Strings.DataService_AuthorizationReturnTypeNotAssignable(method.Name, dataServiceType.FullName, returnType.FullName, expectedReturnType.FullName));
                }
            }
        }

        /// <summary>Verifies that the specified <paramref name="parameter"/> is not an [out] parameter.</summary>
        /// <param name="method">Method with parameter to check.</param>
        /// <param name="parameter">Parameter to check.</param>
        private static void CheckParameterIsNotOut(MethodInfo method, ParameterInfo parameter)
        {
            Debug.Assert(method != null, "method != null");
            Debug.Assert(parameter != null, "parameter != null");

            if (parameter.IsOut)
            {
                throw new InvalidOperationException(Strings.DataService_ParameterIsOut(method.DeclaringType.FullName, method.Name, parameter.Name));
            }
        }

        /// <summary>
        /// Register authorization callbacks specified on the given
        /// <paramref name="dataServiceType"/>.
        /// </summary>
        /// <param name="dataServiceType">Type of web data service to check.</param>
        private void RegisterCallbacks(Type dataServiceType)
        {
            Debug.Assert(dataServiceType != null, "dataServiceType != null");
            Debug.Assert(this.provider != null, "this.provider != null");

            const BindingFlags Flags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public;
            while (dataServiceType != null)
            {
                MethodInfo[] methods = dataServiceType.GetMethods(Flags);
                for (int i = 0; i < methods.Length; i++)
                {
                    MethodInfo method = methods[i];
                    QueryInterceptorAttribute[] queryAttributes = (QueryInterceptorAttribute[])
                        method.GetCustomAttributes(typeof(QueryInterceptorAttribute), true /* inherit */);
                    foreach (QueryInterceptorAttribute attribute in queryAttributes)
                    {
                        ResourceSet container;
                        if (!this.provider.TryResolveResourceSet(attribute.EntitySetName, out container) || container == null)
                        {
                            string message = Strings.DataService_AttributeEntitySetNotFound(
                                attribute.EntitySetName,
                                method.Name,
                                dataServiceType.FullName);
                            throw new InvalidOperationException(message);
                        }

                        CheckQueryInterceptorSignature(dataServiceType, method, container);
                        if (!method.IsAbstract)
                        {
                            if (!this.readAuthorizationMethods.ContainsKey(container.Name))
                            {
                                this.readAuthorizationMethods[container.Name] = new List<MethodInfo>();
                            }

                            this.readAuthorizationMethods[container.Name].Add(method);
                        }
                    }

                    ChangeInterceptorAttribute[] changeAttributes = (ChangeInterceptorAttribute[])
                        method.GetCustomAttributes(typeof(ChangeInterceptorAttribute), true /* inherit */);
                    foreach (ChangeInterceptorAttribute attribute in changeAttributes)
                    {
                        ResourceSet container;
                        if (!this.provider.TryResolveResourceSet(attribute.EntitySetName, out container) || container == null)
                        {
                            string message = Strings.DataService_AttributeEntitySetNotFound(
                                attribute.EntitySetName,
                                method.Name,
                                dataServiceType.FullName);
                            throw new InvalidOperationException(message);
                        }

                        // Check the signature.
                        ParameterInfo[] parameters = method.GetParameters();
                        if (parameters.Length != 2)
                        {
                            string message = Strings.DataService_ChangeInterceptorIncorrectParameterCount(
                                method.Name,
                                dataServiceType.FullName,
                                parameters.Length);
                            throw new InvalidOperationException(message);
                        }

                        CheckParameterIsNotOut(method, parameters[0]);
                        CheckParameterIsNotOut(method, parameters[1]);
                        Type elementParameterType = parameters[0].ParameterType;
                        if (!elementParameterType.IsAssignableFrom(container.ResourceType.InstanceType))
                        {
                            string message = Strings.DataService_AuthorizationParameterNotAssignable(
                                parameters[0].Name,
                                method.Name,
                                dataServiceType.FullName,
                                elementParameterType.FullName,
                                container.ResourceType.InstanceType);
                            throw new InvalidOperationException(message);
                        }

                        Type actionParameterType = parameters[1].ParameterType;
                        if (actionParameterType != typeof(UpdateOperations))
                        {
                            string message = Strings.DataService_AuthorizationParameterNotResourceAction(
                                parameters[1].Name,
                                method.Name,
                                dataServiceType.FullName,
                                typeof(UpdateOperations).FullName);
                            throw new InvalidOperationException(message);
                        }

                        Type returnType = method.ReturnType;
                        if (returnType != typeof(void))
                        {
                            string message = Strings.DataService_AuthorizationMethodNotVoid(
                                method.Name,
                                dataServiceType.FullName,
                                returnType.FullName);
                            throw new InvalidOperationException(message);
                        }

                        if (!method.IsAbstract)
                        {
                            if (!this.writeAuthorizationMethods.ContainsKey(container.Name))
                            {
                                this.writeAuthorizationMethods[container.Name] = new List<MethodInfo>();
                            }

                            this.writeAuthorizationMethods[container.Name].Add(method);
                        }
                    }
                }

                dataServiceType = dataServiceType.BaseType;
            }
        }

        /// <summary>
        /// Loads settings defined in the configuration file.
        /// </summary>
        private void LoadConfigurationSettings()
        {
            this.DataServicesFeaturesSection = (DataServicesFeaturesSection)ConfigurationManager.GetSection("wcfDataServices/features");
        }
    }
}

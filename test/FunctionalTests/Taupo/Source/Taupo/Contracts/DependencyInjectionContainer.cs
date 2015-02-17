//---------------------------------------------------------------------
// <copyright file="DependencyInjectionContainer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Provides dependency injection services.
    /// </summary>
    public abstract class DependencyInjectionContainer : IDependencyInjector, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the DependencyInjectionContainer class.
        /// </summary>
        protected DependencyInjectionContainer()
        {
            this.Logger = Logger.Null;
            this.TestParameters = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets or sets the logger to use.
        /// </summary>
        /// <value>The logger.</value>
        [InjectDependency]
        public Logger Logger { get; set; }

        /// <summary>
        /// Gets the test parameters.
        /// </summary>
        /// <value>The test parameters.</value>
        public IDictionary<string, string> TestParameters { get; private set; }

        /// <summary>
        /// Resolves concrete type for a given contract type, constructs and returns the instance.
        /// </summary>
        /// <typeparam name="TContract">Contract type.</typeparam>
        /// <returns>
        /// Fully resolved object that implements the contract
        /// </returns>
        public TContract Resolve<TContract>() where TContract : class
        {
            return (TContract)this.Resolve(typeof(TContract));
        }

        /// <summary>
        /// Resolves concrete type for a given contract type, constructs and returns the instance.
        /// </summary>
        /// <param name="contractType">Contract type.</param>
        /// <returns>
        /// Fully resolved object that implements the contract
        /// </returns>
        public object Resolve(Type contractType)
        {
            return this.Resolve(null, contractType);
        }

        /// <summary>
        /// Registers a singleton object as an implementation for a given contract.
        /// </summary>
        /// <typeparam name="TContract">Contract implemented by the singleton.</typeparam>
        /// <param name="instance">Instance to register.</param>
        public void RegisterInstance<TContract>(TContract instance)
        {
            this.RegisterInstance(typeof(TContract), instance);
        }

        /// <summary>
        /// Registers the custom resolver for a given type
        /// </summary>
        /// <param name="contractType">Contract type.</param>
        /// <param name="constructorFunction">The constructor function.</param>
        /// <returns>Instance of <see cref="DependencyResolutionOptions"/> which can be used to further configure the resolution process.</returns>
        public abstract DependencyResolutionOptions RegisterCustomResolver(Type contractType, Func<Type, object> constructorFunction);

        /// <summary>
        /// Registers a singleton object as an implementation for a given contract.
        /// </summary>
        /// <param name="contractType">The contract.</param>
        /// <param name="instance">The instance.</param>
        public abstract void RegisterInstance(Type contractType, object instance);

        /// <summary>
        /// Register <typeparamref name="TImplementation"/> as an implementation for <typeparamref name="TContract"/>.
        /// </summary>
        /// <typeparam name="TContract">Contract type.</typeparam>
        /// <typeparam name="TImplementation">Implementation type.</typeparam>
        /// <returns>Instance of <see cref="DependencyResolutionOptions"/> which can be used to further configure the resolution process.</returns>
        public DependencyResolutionOptions Register<TContract, TImplementation>() where TImplementation : TContract
        {
            return this.Register(typeof(TContract), typeof(TImplementation));
        }

        /// <summary>
        /// Register <paramref name="implementationType"/> as an implementation for <paramref name="contractType"/>.
        /// </summary>
        /// <param name="contractType">Contract type.</param>
        /// <param name="implementationType">Implementation type.</param>
        /// <returns>Instance of <see cref="DependencyResolutionOptions"/> which can be used to further configure the resolution process.</returns>
        public abstract DependencyResolutionOptions Register(Type contractType, Type implementationType);

        /// <summary>
        /// Attempts to resolve concrete type for a given contract type, and if possible,
        /// constructs and returns the instance.
        /// </summary>
        /// <typeparam name="TContract">Contract type.</typeparam>
        /// <param name="result">Variable to be assigned with the constructed object reference.</param>
        /// <returns>
        /// true if object construction was successful, false otherwise
        /// </returns>
        public bool TryResolve<TContract>(out TContract result) where TContract : class
        {
            object untypedResult;

            if (this.TryResolve(typeof(TContract), out untypedResult))
            {
                result = (TContract)untypedResult;
                return true;
            }
            else
            {
                result = default(TContract);
                return false;
            }
        }

        /// <summary>
        /// Attempts to resolve concrete type for a given contract type, and if possible,
        /// constructs and returns the instance.
        /// </summary>
        /// <param name="contractType">The contract type.</param>
        /// <param name="result">Variable to be assigned with the constructed object reference.</param>
        /// <returns>
        /// true if object construction was successful, false otherwise
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", Justification = "Generic version is also available.")]
        public bool TryResolve(Type contractType, out object result)
        {
            return this.TryResolve(contractType, contractType, out result);
        }

        /// <summary>
        /// Determines whether this instance can resolve the specified contract type.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>
        /// <c>true</c> if this instance can resolve the specified contract type; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool CanResolve(Type contractType);

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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", Justification = "Generic version is also available.")]
        public abstract bool TryResolve(Type targetType, Type contractType, out object result);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Resolves dependencies for all properties of a given object marked with [InjectDependency]
        /// </summary>
        /// <typeparam name="TObject">Type of the target object.</typeparam>
        /// <param name="targetObject">The target object.</param>
        /// <returns>Target object.</returns>
        public abstract TObject InjectDependenciesInto<TObject>(TObject targetObject) where TObject : class;

        /// <summary>
        /// Creates the dependency injection container which inherits all resolution rules of the base container
        /// at the time of creation.
        /// </summary>
        /// <returns>Inherited dependency injection container.</returns>
        public abstract DependencyInjectionContainer CreateInheritedContainer();

        /// <summary>
        /// Resolves concrete type for a given contract type, constructs and returns the instance.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="contractType">Contract type.</param>
        /// <returns>
        /// Fully resolved object that implements the contract
        /// </returns>
        protected abstract object Resolve(Type targetType, Type contractType);

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected abstract void Dispose(bool disposing);
    }
}

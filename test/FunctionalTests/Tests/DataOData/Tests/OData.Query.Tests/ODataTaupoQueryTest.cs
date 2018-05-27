//---------------------------------------------------------------------
// <copyright file="ODataTaupoQueryTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Query.Tests
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.Taupo.Astoria;
    using Microsoft.Test.Taupo.Astoria.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.DependencyInjection;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Query.Tests.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.Query.Common;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// A Taupo Query Test Base with a test which will ensure that the infrastructure for running shared query tests from Taupo and the verifier we have implemented is not broken
    /// </summary>
    [TestClass, TestCase]
    public class ODataTaupoQueryTest : ODataQueryTestCase
    {
        /// <summary>
        /// Gets or sets the model generator.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ODataTestWorkspace Workspace { get; set; }

        /// <summary>
        /// This will kick in the dependency injection and provide default values for Properties that have 
        /// InjectDependency attribute specified when running in MSTest. 
        /// </summary>
        [TestInitialize]
        public virtual void Initialize()
        {
            // TODO: This initialize method and code in InjectDependencies is duplicated because of trying to inherit from two class hierarchies.
            // One(ODataBaseTestCase) is needed for query tests and the code in ODataTestCaseBase which is copied here is needed for dependency injection
            // to work inside MSTest. The other two options are to make all ODataTestCaseBase inherit from ODataBaseTestCase, but that brings in a lot of 
            // unnecessary dependencies for all the tests. The other option was to move the duplicated code to a helper, and the helper would work with polymorphism
            // but the methods are protected and changing them in the base classes which are in Taupo.Core will require changes to too many files.
            // Matt has done some work to split Taupo.Astoria, once we get that work after next FI see if this can be avoided
            this.InjectDependencies();
        }

        /// <summary>
        /// Configure dependencies for query tests
        /// </summary>
        /// <param name="container">container to configure dependencies on</param>
        protected override void ConfigureDependencies(Test.Taupo.Contracts.DependencyInjectionContainer container)
        {
            base.ConfigureDependencies(container);
            ODataTaupoTestUtils.ConfigureDependenciesHelper(container);
            container.Register<IQueryVerifier, ODataUriParserVerifier>();
            container.Register<IDataServiceProviderFactory, StronglyTypedDataServiceProviderFactory>();
            container.Register<ClientResultComparerBase, ODataObjectResultComparer>();
            container.Register<IClientExpectedErrorComparer, EmptyErrorResultComparer>();
            container.Register<IClientCodeLayerGenerator, PocoClientCodeLayerGenerator>();
            container.Register<DataProviderSettings, ReflectionDataProviderSettings>();
        }

        /// <summary>
        /// This helps the dependency injection mechanism find assemblies to look for implementations
        /// </summary>
        protected virtual void ConfigureDependencyImplentationSelector(ImplementationSelector defaultImplementationSelector)
        {
            foreach (Assembly assembly in DependencyImplementationAssemblies.GetAssemblies())
            {
                defaultImplementationSelector.AddAssembly(assembly);
            }
        }

        /// <summary>
        /// Injects default dependencies when Taupo runner is not being used
        /// </summary>
        protected void InjectDependencies()
        {
            var container = new LightweightDependencyInjectionContainer();

            ImplementationSelector defaultImplSelector = new ImplementationSelector();
            this.ConfigureDependencyImplentationSelector(defaultImplSelector);

            DependencyInjectionConfigurator dependInjecConfigurator = new DependencyInjectionConfigurator(defaultImplSelector, new Dictionary<string, string>());
            dependInjecConfigurator.ConfigureDefaultDependencies(container);

            this.ConfigureDependencies(container);

            // Configure MSTest only dependencies
            container.Register<Logger, TraceLogger>();

            container.RegisterInstance(container);
            container.InjectDependenciesInto(this);
        }
    }
}

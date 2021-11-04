//---------------------------------------------------------------------
// <copyright file="ODataTestCaseBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData
{
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.DependencyInjection;
    using Microsoft.Test.Taupo.EntityModel;
    using Microsoft.Test.Taupo.EntityModel.Edm;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Base class for ODataLib Tests
    /// </summary>
    [TestClass]
    public class ODataTestCaseBase : TestCase
    {
        /// <summary>
        /// Gets or sets the primitive type resolver
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IEntityModelPrimitiveTypeResolver PrimitiveTypeResolver { get; set; }

        /// <summary>
        /// Gets or sets the edm data type resolver
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public EdmDataTypeResolver EdmDataTypeResolver { get; set; }

        /// <summary>
        /// Gets or sets MSTest Context
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// This will kick in the dependency injection and provide default values for Properties that have 
        /// InjectDependency attribute specified
        /// </summary>
        [TestInitialize]
        public virtual void Initialize()
        {
            this.InjectDependencies();
        }

        /// <summary>
        /// Specify Astoria Model for ModelGenerator
        /// </summary>
        /// <param name="container">container on which to register impelemntation dependencies</param>
        protected override void ConfigureDependencies(DependencyInjectionContainer container)
        {
            base.ConfigureDependencies(container);
            ODataTaupoTestUtils.ConfigureDependenciesHelper(container);
        }

        /// <summary>
        /// Injects default dependencies when Taupo runner is not being used
        /// </summary>
        protected void InjectDependencies()
        {
            var container = new LightweightDependencyInjectionContainer();
            this.ConfigureMSTestDependencies(container);
            container.InjectDependenciesInto(this);
        }

        protected virtual void ConfigureMSTestDependencies(DependencyInjectionContainer container)
        {
            ImplementationSelector defaultImplSelector = new ImplementationSelector();
            this.ConfigureDependencyImplentationSelector(defaultImplSelector);

            DependencyInjectionConfigurator dependInjecConfigurator = new DependencyInjectionConfigurator(defaultImplSelector, new Dictionary<string, string>());
            dependInjecConfigurator.ConfigureDefaultDependencies(container);

            this.ConfigureDependencies(container);

            // Configure MSTest only dependencies
            container.Register<Logger, TraceLogger>();
            
            // Set test parameter ApplyTransform to false for ODL tests.
            container.TestParameters.Add("ApplyTransform", "false");

            container.RegisterInstance(container);
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

        protected EntityContainer AddContainer(EntityModelSchema schema)
        {
            return this.AddContainer(schema, "TestNS");
        }

        protected EntityContainer AddContainer(EntityModelSchema schema, string namespaceName)
        {
            EntityContainer container = new EntityContainer("TestContainer");
            schema.Add(container);
            new ApplyDefaultNamespaceFixup(namespaceName).Fixup(schema);
            new ResolveReferencesFixup().Fixup(schema);
            this.PrimitiveTypeResolver.ResolveProviderTypes(schema, this.EdmDataTypeResolver);

            return container;
        }
    }
}

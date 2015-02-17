//---------------------------------------------------------------------
// <copyright file="ODataReaderTestCase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces

    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.Test.Taupo.Astoria;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.DependencyInjection;
    using Microsoft.Test.Taupo.Edmlib;
    using Microsoft.Test.Taupo.Edmlib.Contracts;
    using Microsoft.Test.Taupo.OData;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Json;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.Test.Taupo.Spatial.EntityModel;
    #endregion Namespaces

    /// <summary>
    /// Base class for ODataLib Tests with extra assembly added for unit test
    /// </summary>
    public class ODataReaderTestCase : ODataTestCaseBase
    {
        /// <summary>
        /// The reader test configuration provider for this test module.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ReaderTestConfigurationProvider ReaderTestConfigurationProvider { get; set; }

        /// <summary>
        /// The combinatorial engine to use in matrix based tests.
        /// </summary>
        [InjectDependency]
        public ICombinatorialEngineProvider CombinatorialEngineProvider { get; set; }

        [InjectDependency(IsRequired = true)]
        public IExceptionVerifier ExceptionVerifier { get; set; }

        /// <summary>
        /// This injects the current assembly into the DependencyImplementationSelector
        /// </summary>
        protected override void ConfigureDependencyImplentationSelector(ImplementationSelector defaultImplementationSelector)
        {
            base.ConfigureDependencyImplentationSelector(defaultImplementationSelector);
            defaultImplementationSelector.AddAssembly(typeof(ODataUnitTestModule).Assembly);
            defaultImplementationSelector.AddAssembly(typeof(ODataReaderTestCase).Assembly);
            // TODO: Consider adding this to the base if we find adding spatial to other test projects.
            defaultImplementationSelector.AddAssembly(typeof(SpatialClrTypeResolver).Assembly);
        }

        /// <summary>
        /// Configure Dependencies specific to Reader Test Cases
        /// </summary>
        /// <param name="container">container to set dependencies on</param>
        protected override void ConfigureDependencies(Taupo.Contracts.DependencyInjectionContainer container)
        {
            base.ConfigureDependencies(container);
            container.Register<IEntityModelSchemaComparer, TaupoModelComparer>();
            container.Register<IODataRequestManager, ODataRequestManager>();
            container.Register<IBatchPayloadSerializer, BatchPayloadSerializer>();
            container.Register<StackBasedAssertionHandler, DefaultStackBasedAssertionHandler>();
            container.Register<IPayloadElementToJsonConverter, AnnotatedPayloadElementToJsonConverter>();
            container.Register<IPayloadElementToJsonLightConverter, AnnotatedPayloadElementToJsonLightConverter>();
            container.Register<IPayloadGenerator, PayloadGenerator>();
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="ODataWriterTestCase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests
{
    using System;
    using ApprovalTests.Reporters;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.Test.OData.Utils.Common;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.ResponseVerification;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Json;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Fixups;
    using Microsoft.Test.Taupo.OData.Writer.Tests.WriterCombinatorialEngine;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ExceptionUtilities = Microsoft.Test.Taupo.Common.ExceptionUtilities;

    /// <summary>
    /// Base class for ODataLib Tests with extra assembly added for unit test
    /// </summary>
    [UseReporter(typeof(LoggingReporter))]
    [DeploymentItem("CollectionWriter")]
    [DeploymentItem("JsonLight")]
    [DeploymentItem("ParameterWriter")]
    [DeploymentItem("Writer")]
    public class ODataWriterTestCase : ODataTestCaseBase
    {
        /// <summary>
        /// The writer test configuration provider for this test module.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public WriterTestConfigurationProvider WriterTestConfigurationProvider { get; set; }

        /// <summary>
        /// The combinatorial engine to use for running matrix based tests.
        /// </summary>
        public WriterCombinatorialEngineProvider CombinatorialEngineProvider { get; set; }

        public BaselineLogger Logger { get; set; }

        /// <summary>
        /// The converter from entity model schema to Edm models.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public EntityModelSchemaToEdmModelConverter EntityModelSchemaToEdmModelConverter { get; set; }

        /// <summary>
        /// Gets or sets the exception verifier.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IExceptionVerifier ExceptionVerifier { get; set; }

        [InjectDependency(IsRequired = true)]
        public IPayloadElementODataWriter PayloadElementWriter { get; set; }

        [InjectDependency(IsRequired = true)]
        public IODataPayloadElementPropertyWriter PropertyPayloadElementWriter { get; set; }

        [InjectDependency(IsRequired = true)]
        public IODataPayloadElementComparer PayloadElementComparer { get; set; }

        [InjectDependency(IsRequired = true)]
        public IProtocolFormatStrategySelector FormatSelector { get; set; }

        [InjectDependency(IsRequired = true)]
        public IODataJsonDateTimeFormattingValidator JsonDateTimeFormatValidator { get; set; }

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
            this.Logger = new BaselineLogger();

            this.CombinatorialEngineProvider = new WriterCombinatorialEngineProvider()
                .SetBaselineCallback(this.Logger.GetBaseline)
                .SetLogCombinationCallback(this.Logger.LogCombination);
            this.CombinatorialEngineProvider.SetApprovalFileSourcePath(TestContext.DeploymentDirectory);
        }

        /// <summary>
        /// This injects the current assembly into the DependencyImplementationSelector
        /// </summary>
        protected override void ConfigureDependencyImplentationSelector(DependencyInjection.ImplementationSelector defaultImplementationSelector)
        {
            base.ConfigureDependencyImplentationSelector(defaultImplementationSelector);
            defaultImplementationSelector.AddAssembly(typeof(ODataUnitTestModule).Assembly);
        }

        protected override void ConfigureDependencies(Taupo.Contracts.DependencyInjectionContainer container)
        {
            base.ConfigureDependencies(container);
            container.Register<IProtocolFormatNormalizerSelector, DefaultProtocolFormatNormalizerSelector>();
            container.Register<IPayloadElementToJsonConverter, AnnotatedPayloadElementToJsonConverter>();
            container.Register<IPayloadElementToJsonLightConverter, AnnotatedPayloadElementToJsonLightConverter>();
            container.Register<Taupo.Contracts.IEntityModelSchemaComparer, ODataEntityModelSchemaComparer>();
            container.Register<IPayloadGenerator, PayloadGenerator>();
        }

        /// <summary>
        /// Creates an ODataWriter for the specified format and the specified version and
        /// writes the payload in the descriptor to an in-memory stream. It then parses
        /// the written Xml and compares it to the expected result as specified in the descriptor.
        /// </summary>
        /// <param name="originalPayload">expectedPayload to write and</param>
        /// <param name="testConfiguration">Configuration for the test</param>
        protected void WriteAndVerifyODataPayloadElement(ODataPayloadElement originalPayload, WriterTestConfiguration testConfiguration)
        {
            this.Logger.LogConfiguration(testConfiguration);

            bool feedWriter = originalPayload.ElementType == ODataPayloadElementType.EntitySetInstance;
            using (var memoryStream = new TestStream())
            {
                using (var messageWriter = TestWriterUtils.CreateMessageWriter(memoryStream, testConfiguration, this.Assert))
                {
                    ODataWriter writer = messageWriter.CreateODataWriter(feedWriter);
                    Action<ODataPayloadElement> writeToStream = payload => this.PayloadElementWriter.WritePayload(writer, payload);
                    this.WriteAndLogODataPayload(originalPayload, messageWriter.Message, testConfiguration.Version, testConfiguration.Format, writeToStream);
                }
            }
        }

        /// <summary>
        /// Creates an ODataWriter for the specified format and the specified version and
        /// writes the property in the descriptor to an in-memory stream. It then parses
        /// the written Xml and compares it to the expected result as specified in the descriptor.
        /// </summary>
        /// <param name="originalPayload">The payload to first clone and then write.</param>
        /// <param name="testConfiguration">Configuration for the test</param>
        /// <param name="model">The model to use.</param>
        protected void WriteAndVerifyODataProperty(ODataPayloadElement originalPayload, WriterTestConfiguration testConfiguration, IEdmModel model)
        {
            this.Logger.LogConfiguration(testConfiguration);
            this.Logger.LogModelPresence(model);

            using (var memoryStream = new TestStream())
            {
                TestMessage testMessage;
                using (ODataMessageWriterTestWrapper writer = TestWriterUtils.CreateMessageWriter(memoryStream, testConfiguration, this.Assert, out testMessage, null, model))
                {
                    Action<ODataPayloadElement> writeElementToStream = payload => this.PropertyPayloadElementWriter.WriteProperty(writer.MessageWriter, payload);
                    this.WriteAndLogODataPayload(originalPayload, writer.Message, testConfiguration.Version, testConfiguration.Format, writeElementToStream);
                }
            }
        }

        /// <summary>
        /// Writes the payload to the stream using the given callback, then verifies the payload using the test deserializer
        /// </summary>
        /// <param name="originalPayload">The payload being tested, of which a copy will be made</param>
        /// <param name="message">The stream to write to</param>
        /// <param name="odataVersion">The OData protocol version to be used for writing payloads.</param>
        /// <param name="format">The current format</param>
        /// <param name="writeToStream">The callback to write to the stream</param>
        private void WriteAndLogODataPayload(ODataPayloadElement originalPayload, TestMessage message, ODataVersion odataVersion, ODataFormat format, Action<ODataPayloadElement> writeToStream)
        {
            ExceptionUtilities.CheckArgumentNotNull(originalPayload, "originalPayload");
            ExceptionUtilities.CheckArgumentNotNull(writeToStream, "writeToStream");

            // This is needed because we may modify the payload in use but the same is used in another iteration of the combinatorial engine
            var payload = originalPayload.DeepCopy();
            WriteToStream(format, writeToStream, payload);
            var newPayload = TestWriterUtils.ReadToString(message);
            this.Logger.LogPayload(newPayload);
        }

        private static string WriteToStream(ODataFormat format, Action<ODataPayloadElement> writeToStream, ODataPayloadElement payload)
        {
            string contentType = null;
            if (format == ODataFormat.Json)
            {
                payload.Accept(new ODataPayloadJsonNormalizer());
                contentType = MimeTypes.ApplicationJsonLight;
            }
            else
            {
                ExceptionUtilities.Assert(false, "Format not supported: {0}", format);
                contentType = MimeTypes.ApplicationAtomXml;
            }

            writeToStream(payload);
            return contentType;
        }
    }
}

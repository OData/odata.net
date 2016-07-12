//---------------------------------------------------------------------
// <copyright file="MessageReaderJsonLightTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.JsonLight
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// JSON Lite specific tests for the ODataMessageReader class.
    /// </summary>
    [TestClass, TestCase]
    public class MessageReaderJsonLightTests : ODataReaderTestCase
    {
        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Validates that reading certain inputs without metadata fails in JSON Light.")]
        public void NoMetadataTest()
        {
            EdmModel model = new EdmModel();
            var container = new EdmEntityContainer("TestModel", "DefaultContainer");
            model.AddElement(container);

            var testDescriptors = new[]
                {
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.EntitySet(),
                        SkipTestConfiguration = tc => !tc.IsRequest,
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightInputContext_ModelRequiredForReading"),
                    },
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.EntitySet(),
                        SkipTestConfiguration = tc => tc.IsRequest,
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightDeserializer_ContextLinkNotFoundAsFirstProperty"),
                    },
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.EntitySet(),
                        PayloadEdmModel = model,
                        SkipTestConfiguration = tc => !tc.IsRequest,
                        //ExpectedException = ODataExpectedExceptions.ODataException("ReaderValidationUtils_ResourceWithoutType"),
                    },
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity(),
                        SkipTestConfiguration = tc => !tc.IsRequest,
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightInputContext_ModelRequiredForReading"),
                    },
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity(),
                        SkipTestConfiguration = tc => tc.IsRequest,
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightDeserializer_ContextLinkNotFoundAsFirstProperty"),
                    },
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity(),
                        PayloadEdmModel = model,
                        SkipTestConfiguration = tc => !tc.IsRequest,
                        ExpectedException = ODataExpectedExceptions.ODataException("ReaderValidationUtils_ResourceWithoutType"),
                    },
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.PrimitiveProperty("propertyName", 42),
                        SkipTestConfiguration = tc => !tc.IsRequest,
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightInputContext_ModelRequiredForReading"),
                    },
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Property("propertyName", PayloadBuilder.ComplexValue()),
                        SkipTestConfiguration = tc => !tc.IsRequest,
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightInputContext_ModelRequiredForReading"),
                    },
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Property("propertyName", PayloadBuilder.PrimitiveMultiValue()),
                        SkipTestConfiguration = tc => !tc.IsRequest,
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightInputContext_ModelRequiredForReading"),
                    },
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.PrimitiveCollection(),
                        SkipTestConfiguration = tc => !tc.IsRequest,
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightInputContext_ModelRequiredForReading"),
                    },
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.PrimitiveCollection(),
                        SkipTestConfiguration = tc => tc.IsRequest,
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightDeserializer_ContextLinkNotFoundAsFirstProperty"),
                    },
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.PrimitiveCollection(),
                        //PayloadModel = model,
                        PayloadEdmModel = model,
                        SkipTestConfiguration = tc => !tc.IsRequest,
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightInputContext_ItemTypeRequiredForCollectionReaderInRequests"),
                    },
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.ComplexCollection(),
                        SkipTestConfiguration = tc => !tc.IsRequest,
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightInputContext_ModelRequiredForReading"),
                    },
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.ComplexCollection(),
                        SkipTestConfiguration = tc => tc.IsRequest,
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightDeserializer_ContextLinkNotFoundAsFirstProperty"),
                    },
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.ComplexCollection(),
                        PayloadEdmModel = model,
                        SkipTestConfiguration = tc => !tc.IsRequest,
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightInputContext_ItemTypeRequiredForCollectionReaderInRequests"),
                    },
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.ServiceDocument().Workspace(PayloadBuilder.Workspace()),
                        SkipTestConfiguration = tc => tc.IsRequest,
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightDeserializer_ContextLinkNotFoundAsFirstProperty"),
                    },
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.DeferredLink("http://odata.org/deferred"),
                        SkipTestConfiguration = tc => !tc.IsRequest,
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightInputContext_ModelRequiredForReading"),
                    },
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.DeferredLink("http://odata.org/deferred"),
                        SkipTestConfiguration = tc => tc.IsRequest,
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightDeserializer_ContextLinkNotFoundAsFirstProperty"),
                    },
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.LinkCollection(),
                        SkipTestConfiguration = tc => tc.IsRequest,
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightDeserializer_ContextLinkNotFoundAsFirstProperty"),
                    },
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.ComplexValue(),
                        SkipTestConfiguration = tc => !tc.IsRequest,
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightInputContext_ModelRequiredForReading"),
                    },
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.ComplexValue(),
                        PayloadEdmModel = model,
                        SkipTestConfiguration = tc => !tc.IsRequest,
                        ExpectedException = ODataExpectedExceptions.ArgumentNullException("ODataJsonLightInputContext_OperationCannotBeNullForCreateParameterReader", "operation")
                    },
                };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    // These descriptors are already tailored specifically for Json Light and 
                    // do not require normalization.
                    testDescriptor.TestDescriptorNormalizers.Clear();
                    testDescriptor.RunTest(testConfiguration);
                });
        }
    }
}

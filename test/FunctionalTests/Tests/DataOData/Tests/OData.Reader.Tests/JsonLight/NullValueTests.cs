//---------------------------------------------------------------------
// <copyright file="NullValueTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.JsonLight
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm.Library;
    using Microsoft.Test.OData.Utils.ODataLibTest;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts.Json;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests reading of collection values in JSON.
    /// </summary>
    [TestClass, TestCase]
    public class NullValueJsonLightTests : ODataReaderTestCase
    {
        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Verifies that non-property readers cannot read null values.")]
        public void NullValueNonPropertyErrorTests()
        {
            EdmModel model = new EdmModel();
            var entityType = model.EntityType("OwningType", "TestModel").KeyProperty("ID", EdmCoreModel.Instance.GetInt32(false) as EdmTypeReference);
            model.EntityContainer("DefaultContainer");
            var entitySet = model.EntitySet("EntitySet", entityType);

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new[]
            {
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "null collection - should fail.",
                    PayloadEdmModel = model,
                    PayloadElement = PayloadBuilder.PrimitiveCollection().ExpectedCollectionItemType(EdmDataTypes.Int32)
                        .JsonRepresentation(
                            "{" + 
                            "\"@odata.context\":\"http://odata.org/test/$metadata#Edm.Null\"" +
                            "}"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightContextUriParser_ContextUriDoesNotMatchExpectedPayloadKind", "http://odata.org/test/$metadata#Edm.Null", "Collection")
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "null entry - should fail.",
                    PayloadEdmModel = model,
                    PayloadElement = PayloadBuilder.NullEntity().ExpectedEntityType(entityType, entitySet)
                        .JsonRepresentation(
                            "{" + 
                            "\"@odata.context\":\"http://odata.org/test/$metadata#Edm.Null\"" +
                            "}"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightContextUriParser_ContextUriDoesNotMatchExpectedPayloadKind", "http://odata.org/test/$metadata#Edm.Null", "Entry")
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "null feed - should fail.",
                    PayloadEdmModel = model,
                    PayloadElement = PayloadBuilder.EntitySet().ExpectedEntityType(entityType, entitySet)
                        .JsonRepresentation(
                            "{" + 
                            "\"@odata.context\":\"http://odata.org/test/$metadata#Edm.Null\"" +
                            "}"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightContextUriParser_ContextUriDoesNotMatchExpectedPayloadKind", "http://odata.org/test/$metadata#Edm.Null", "Feed")
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    if (testConfiguration.IsRequest)
                    {
                        return;
                    }

                    // These descriptors are already tailored specifically for Json Light and 
                    // do not require normalization.
                    testDescriptor.TestDescriptorNormalizers.Clear();
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Verifies correct reading of null property values")]
        public void NullValuePropertyTests()
        {
            EdmModel model = new EdmModel();

            var testCases = new []
            {
                new
                {
                    Payload = "{{\"@odata.context\":\"http://odata.org/test/$metadata#Edm.Null\",\"@odata.null\":true{0}}}",
                    SkipTestConfiguration = (Func<ReaderTestConfiguration, bool>)(tc => false)
                },
                new
                {
                    Payload = "{{\"@odata.context\":\"http://odata.org/test/$metadata#Edm.Null\"{0}}}",
                    SkipTestConfiguration = (Func<ReaderTestConfiguration, bool>)(tc => tc.IsRequest)
                },
                new
                {
                    Payload = "{{\"@odata.context\":\"http://odata.org/test/$metadata#Edm.String\",\"@odata.null\":true{0}}}",
                    SkipTestConfiguration = (Func<ReaderTestConfiguration, bool>)(tc => false)
                },
                new
                {
                    Payload = "{{\"@odata.null\":true{0}}}",
                    SkipTestConfiguration = (Func<ReaderTestConfiguration, bool>)(tc => tc.IsRequest == false)
                },
            };

            var testDescriptors = testCases.SelectMany(testCase =>
                new[]
                {
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        DebugDescription = "null property value - should pass.",
                        PayloadEdmModel = model,
                        PayloadElement = PayloadBuilder.PrimitiveProperty(null, null).JsonRepresentation(string.Format(testCase.Payload, string.Empty)),
                        SkipTestConfiguration = testCase.SkipTestConfiguration
                    },
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        DebugDescription = "null property value with custom annotation - should pass.",
                        PayloadEdmModel = model,
                        PayloadElement = PayloadBuilder.PrimitiveProperty(null, null).JsonRepresentation(string.Format(testCase.Payload, ",\"@Custom.Annotation\":\"foo\"")),
                        SkipTestConfiguration = testCase.SkipTestConfiguration
                    },
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        DebugDescription = "null property value with sub-property - should fail.",
                        PayloadEdmModel = model,
                        PayloadElement = PayloadBuilder.PrimitiveProperty(null, null).JsonRepresentation(string.Format(testCase.Payload, ",\"p1\":1")),
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightPropertyAndValueDeserializer_NoPropertyAndAnnotationAllowedInNullPayload", "p1"),
                        SkipTestConfiguration = testCase.SkipTestConfiguration
                    },
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        DebugDescription = "null property value with custom annotation and sub-property - should fail.",
                        PayloadEdmModel = model,
                        PayloadElement = PayloadBuilder.PrimitiveProperty(null, null).JsonRepresentation(string.Format(testCase.Payload, ",\"@Custom.Annotation\":\"foo\",\"p1\":1")),
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightPropertyAndValueDeserializer_NoPropertyAndAnnotationAllowedInNullPayload", "p1"),
                        SkipTestConfiguration = testCase.SkipTestConfiguration
                    },
                });

            testDescriptors = testDescriptors.Concat(new[]
            {
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "null property context URI with custom annotation followed by odata.null - should fail.",
                    PayloadEdmModel = model,
                    PayloadElement = PayloadBuilder.PrimitiveProperty(null, null)
                        .JsonRepresentation(
                            "{" + 
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Edm.Null\"," +
                            "\"@Custom.Annotation\":\"foo\"," +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataNullAnnotationName + "\":true" +
                            "}"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties", "odata.null")
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "null property context URI with property followed by odata.null - should fail.",
                    PayloadEdmModel = model,
                    PayloadElement = PayloadBuilder.PrimitiveProperty(null, null)
                        .JsonRepresentation(
                            "{" + 
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Edm.Null\"," +
                            "\"foo\":\"bar\"," +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataNullAnnotationName + "\":true" +
                            "}"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightPropertyAndValueDeserializer_NoPropertyAndAnnotationAllowedInNullPayload", "foo"),
                    SkipTestConfiguration = (Func<ReaderTestConfiguration, bool>)(tc => tc.IsRequest)
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "null property context URI on non-nullable type - should fail.",
                    PayloadEdmModel = model,
                    PayloadElement = PayloadBuilder.PrimitiveProperty(null, null).AddExpectedTypeAnnotation(new ExpectedTypeODataPayloadElementAnnotation { ExpectedType = EdmDataTypes.Int32})
                        .JsonRepresentation(
                            "{" + 
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Edm.Null\"" +
                            "}"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ReaderValidationUtils_NullValueForNonNullableType", "Edm.Int32"),
                    SkipTestConfiguration = (Func<ReaderTestConfiguration, bool>)(tc => tc.IsRequest)
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "null property context URI on non-nullable type - should fail.",
                    PayloadEdmModel = model,
                    PayloadElement = PayloadBuilder.PrimitiveProperty(null, null).AddExpectedTypeAnnotation(new ExpectedTypeODataPayloadElementAnnotation { ExpectedType = EdmDataTypes.Int32})
                        .JsonRepresentation(
                            "{" + 
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Edm.Int32\"," +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataNullAnnotationName + "\":true" +
                            "}"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ReaderValidationUtils_NullValueForNonNullableType", "Edm.Int32")
                },
            });

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

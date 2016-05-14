//---------------------------------------------------------------------
// <copyright file="EntryReaderJsonLightTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.JsonLight
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.Spatial;
    using Microsoft.Test.OData.Utils.ODataLibTest;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Common.Annotations;
    using Microsoft.Test.Taupo.OData.Contracts.Json;
    using Microsoft.Test.Taupo.OData.Contracts.JsonLight;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for correct handling of entry payloads.
    /// </summary>
    [TestClass, TestCase]
    public class EntryReaderJsonLightTests : ODataReaderTestCase
    {
        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Verifies correct reading of entries.")]
        public void EntryReaderTest()
        {
            IEdmModel model = Test.OData.Utils.Metadata.TestModels.BuildTestModel();
            IEdmEntityContainer container = model.FindEntityContainer("DefaultContainer");
            IEdmEntitySet citiesEntitySet = container.FindEntitySet("Cities");
            IEdmType cityType = model.FindType("TestModel.CityType");

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new[]
            {
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Entry without any type",
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1)
                        .JsonRepresentation("{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#TestModel.DefaultContainer.Cities()/$entity\"," +
                            "\"Id\":1" +
                            "}")
                        .AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null })
                        .ExpectedEntityType(cityType, citiesEntitySet),
                    PayloadEdmModel = model
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Entry without any type and no context URI in the payload and no expected type - should fail",
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType")
                        .JsonRepresentation("{" +
                            "}"),
                    PayloadEdmModel = model,
                    ExpectedResultCallback = tc => new PayloadReaderTestExpectedResult(this.Settings.ExpectedResultSettings)
                        {
                            ExpectedException = tc.IsRequest
                                ? ODataExpectedExceptions.ODataException("ODataJsonLightInputContext_NoEntitySetForRequest")
                                : ODataExpectedExceptions.ODataException("ODataJsonLightDeserializer_ContextLinkNotFoundAsFirstProperty")
                        }
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Entry without any type and no context URI but with expected type in responses - should fail due to missing context URI",
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType")
                        .JsonRepresentation("{" +
                            "}")
                        .ExpectedEntityType(cityType, citiesEntitySet),
                    PayloadEdmModel = model,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightDeserializer_ContextLinkNotFoundAsFirstProperty"),
                    SkipTestConfiguration = tc => tc.IsRequest
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Entry without any type and no context URI but with expected type in requests - should work",
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1)
                        .JsonRepresentation("{" +
                            "\"Id\":1" +
                            "}")
                        .AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null })
                        .ExpectedEntityType(cityType, citiesEntitySet),
                    PayloadEdmModel = model,
                    SkipTestConfiguration = tc => !tc.IsRequest
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Entry without the odata.type but with context URI and no expected type or entity set in responses - should work.",
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1)
                        .JsonRepresentation("{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#TestModel.DefaultContainer.Cities()/$entity\"," +
                            "\"Id\":1" +
                            "}")
                        .AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null }),
                    PayloadEdmModel = model,
                    // The context URI is only read in responses
                    SkipTestConfiguration = tc => tc.IsRequest
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Entry without the odata.type but with context URI and no expected type or entity set in requests - should fail.",
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType")
                        .JsonRepresentation("{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#TestModel.DefaultContainer.Cities()/$entity\"" +
                            "}"),
                    PayloadEdmModel = model,
                    // For requests the context URI is ignored and thus there's no type information in this case the reading should fail.
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightInputContext_NoEntitySetForRequest"),
                    SkipTestConfiguration = tc => !tc.IsRequest
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Entry with same type name in the payload",
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1)
                        .JsonRepresentation("{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#TestModel.DefaultContainer.Cities()/$entity\"," +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.CityType\"," +
                            "\"Id\":1" +
                            "}")
                        .ExpectedEntityType(cityType, citiesEntitySet),
                    PayloadEdmModel = model
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Entry with type name in the payload but no expected type (and no context URI) - request only - should fail",
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType")
                        .JsonRepresentation("{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.CityType\"" +
                            "}"),
                    PayloadEdmModel = model,
                    SkipTestConfiguration = tc => !tc.IsRequest,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightInputContext_NoEntitySetForRequest")
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Entry with null type name - should fail",
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType")
                        .JsonRepresentation("{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#TestModel.DefaultContainer.Cities()/$entity\"," +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":null" +
                            "}")
                        .ExpectedEntityType(cityType, citiesEntitySet),
                    PayloadEdmModel = model,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightPropertyAndValueDeserializer_InvalidTypeName", string.Empty)
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Entry with derived type name in the payload",
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityOpenType").PrimitiveProperty("Id", 1)
                        .JsonRepresentation("{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#TestModel.DefaultContainer.Cities()/$entity\"," +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.CityOpenType\"," +
                            "\"Id\":1" +
                            "}")
                        .ExpectedEntityType(cityType, citiesEntitySet),
                    PayloadEdmModel = model
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Entry with id and etag annotations",
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1)
                        .Id("urn:id")
                        .ETag("etag")
                        .JsonRepresentation("{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#TestModel.DefaultContainer.Cities()/$entity\"," +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataIdAnnotationName + "\":\"urn:id\"," +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataETagAnnotationName + "\":\"etag\"," +
                            "\"Id\":1" +
                            "}")
                        .AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null })
                        .ExpectedEntityType(cityType, citiesEntitySet),
                    PayloadEdmModel = model
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Type annotation is not the first one",
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType")
                        .Id("urn:id")
                        .ETag("etag")
                        .JsonRepresentation("{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#TestModel.DefaultContainer.Cities()/$entity\"," +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataIdAnnotationName + "\":\"urn:id\"," +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.CityType\"," +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataETagAnnotationName + "\":\"etag\"" +
                            "}")
                        .ExpectedEntityType(cityType, citiesEntitySet),
                    PayloadEdmModel = model,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntryAndFeedDeserializer_EntryTypeAnnotationNotFirst")
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Id annotation is after a property",
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType")
                        .Id("urn:id")
                        .JsonRepresentation("{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#TestModel.DefaultContainer.Cities()/$entity\"," +
                            "\"Id\":1," +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataIdAnnotationName + "\":\"urn:id\"" +
                            "}")
                        .AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null })
                        .ExpectedEntityType(cityType, citiesEntitySet),
                    PayloadEdmModel = model,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntryAndFeedDeserializer_EntryInstanceAnnotationPrecededByProperty", JsonLightConstants.ODataIdAnnotationName)
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "ETag annotation is after a property",
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1)
                        .Id("urn:id")
                        .ETag("etag")
                        .JsonRepresentation("{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#TestModel.DefaultContainer.Cities()/$entity\"," +
                            "\"Id\":1," +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataETagAnnotationName + "\":\"etag\"" +
                            "}")
                        .ExpectedEntityType(cityType, citiesEntitySet),
                    PayloadEdmModel = model,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntryAndFeedDeserializer_EntryInstanceAnnotationPrecededByProperty", JsonLightConstants.ODataETagAnnotationName)
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Duplicate data properties (sanity test only; more in format-agnostic tests)",
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1)
                        .Id("1")
                        .JsonRepresentation("{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#TestModel.DefaultContainer.Cities()/$entity\"," +
                            "\"Id\":1," +
                            "\"Id\":2" +
                            "}")
                        .ExpectedEntityType(cityType, citiesEntitySet),
                    PayloadEdmModel = model,
                    ExpectedException = ODataExpectedExceptions.ODataException("DuplicatePropertyNamesChecker_DuplicatePropertyNamesNotAllowed", "Id")
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

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Verifies correct validation of entry instance annotation values.")]
        public void InvalidEntryInstanceAnnotationValues()
        {
            IEdmModel model = Test.OData.Utils.Metadata.TestModels.BuildTestModel();
            IEdmEntityContainer container = model.FindEntityContainer("DefaultContainer");
            IEdmEntitySet citiesEntitySet = container.FindEntitySet("Cities");
            IEdmType cityType = model.FindType("TestModel.CityType");

            var invalidValues = new[]
            {
                new InvalidEntryInstanceAnnotationValue 
                { 
                    Json = "null", 
                    // ID is allowed to be null (since that indicates a transient entity), but other annotations must not be null.
                    ExpectedException = annotationName => 
                        annotationName == JsonLightConstants.ODataIdAnnotationName 
                        ? null 
                        : ODataExpectedExceptions.ODataException("ODataJsonLightReaderUtils_AnnotationWithNullValue", annotationName)
                },
                new InvalidEntryInstanceAnnotationValue 
                { 
                    Json = "42", 
                    ExpectedException = annotationName => ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_CannotReadPropertyValueAsString", "42", annotationName)
                },
                new InvalidEntryInstanceAnnotationValue 
                { 
                    Json = "{}", 
                    ExpectedException = annotationName => ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "PrimitiveValue", "StartObject")
                },
                new InvalidEntryInstanceAnnotationValue 
                { 
                    Json = "[]", 
                    ExpectedException = annotationName => ODataExpectedExceptions.ODataExceptionContains("JsonReaderExtensions_UnexpectedNodeDetected", "PrimitiveValue", "StartArray")
                },
            };

            var annotationNames = new string[]
            {
                JsonLightConstants.ODataIdAnnotationName,
                JsonLightConstants.ODataETagAnnotationName,
                JsonLightConstants.ODataEditLinkAnnotationName,
                JsonLightConstants.ODataReadLinkAnnotationName,
                JsonLightConstants.ODataMediaEditLinkAnnotationName,
                JsonLightConstants.ODataMediaReadLinkAnnotationName,
                JsonLightConstants.ODataMediaContentTypeAnnotationName,
                JsonLightConstants.ODataMediaETagAnnotationName
            };

            var testDescriptors = invalidValues.SelectMany(invalidValue => annotationNames.Select(annotationName =>
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType")
                        .JsonRepresentation("{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#TestModel.DefaultContainer.Cities/$entity\"," +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + annotationName + "\":" + invalidValue.Json +
                            "}")
                        .ExpectedEntityType(cityType, citiesEntitySet),
                    PayloadEdmModel = model,
                    ExpectedException = invalidValue.ExpectedException(annotationName)
                }));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    // This test only tests error conditions
                    if (testDescriptor.ExpectedException == null)
                    {
                        return;
                    }

                    // These descriptors are already tailored specifically for Json Light and 
                    // do not require normalization.
                    testDescriptor.TestDescriptorNormalizers.Clear();
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Verifies correct reading of entry level annotations")]
        public void EntryAnnotationTest()
        {
            var injectedProperties = new[]
                {
                    new
                    {
                        InjectedJSON = string.Empty,
                        ExpectedException = (ExpectedException)null
                    },
                    new
                    {
                        InjectedJSON = "\"@custom.annotation\": null",
                        ExpectedException = (ExpectedException)null
                    },
                    new
                    {
                        InjectedJSON = "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\": { }",
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties", JsonLightConstants.ODataContextAnnotationName)
                    },
                    new
                    {
                        InjectedJSON = "\"@custom.annotation\": null, \"@custom.annotation\": 42",
                        ExpectedException = ODataExpectedExceptions.ODataException("DuplicatePropertyNamesChecker_DuplicateAnnotationNotAllowed", "custom.annotation")
                    },
                };

            var payloads = new[]
                {
                    new
                    {
                        Json = "{{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#TestModel.DefaultContainer.Cities()/$entity\"{1}" +
                            "{0}" +
                            ",\"Id\":1" +
                            "}}",
                        ExpectedValue = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1)
                    },
                    new
                    {
                        Json = "{{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#TestModel.DefaultContainer.Cities()/$entity\"," +
                            "{0}{1}" +
                            " \"Name\": \"Value\"," +
                            "\"Id\":1" +
                            "}}",
                        ExpectedValue = PayloadBuilder.Entity("TestModel.CityType")
                            .PrimitiveProperty("Name", "Value")
                            .PrimitiveProperty("Id", 1)
                    },
                    new
                    {
                        Json = "{{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#TestModel.DefaultContainer.Cities()/$entity\"," +
                            "\"Name\": \"Value\"," +
                            "\"Id\":1" +
                            "{1}{0}" +
                            "}}",
                        ExpectedValue = PayloadBuilder.Entity("TestModel.CityType")
                            .PrimitiveProperty("Name", "Value")
                            .PrimitiveProperty("Id", 1)
                    },
                    new
                    {
                        Json = "{{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#TestModel.DefaultContainer.Cities()/$entity\"," +
                            "\"Name\":\"Value\"," +
                            "{0}{1}" +
                            "\"Id\":42" +
                            "}}",
                        ExpectedValue = PayloadBuilder.Entity("TestModel.CityType")
                            .PrimitiveProperty("Name", "Value")
                            .PrimitiveProperty("Id", 42)
                    },
                };

            IEdmModel model = Test.OData.Utils.Metadata.TestModels.BuildTestModel();
            IEdmEntityContainer container = model.FindEntityContainer("DefaultContainer");
            IEdmEntitySet citiesEntitySet = container.FindEntitySet("Cities");
            IEdmType cityType = model.FindType("TestModel.CityType");

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = payloads.SelectMany(payload => injectedProperties.Select(injectedProperty =>
            {
                return new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = payload.ExpectedValue.DeepCopy()
                        .JsonRepresentation(string.Format(payload.Json, injectedProperty.InjectedJSON, string.IsNullOrEmpty(injectedProperty.InjectedJSON) ? string.Empty : ","))
                        // JSON Light payloads don't store entity type names since the type can be inferred from the context URI and or API.
                        .SetAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null })
                        .ExpectedEntityType(cityType, citiesEntitySet),
                    PayloadEdmModel = model,
                    ExpectedException = injectedProperty.ExpectedException,
                };
            }));

            var explicitPayloads = new[]
                {
                    new
                    {
                        Description = "Type annotation preceded by custom annotation - should fail",
                        Json = "{" +
                                "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#TestModel.DefaultContainer.Cities()/$entity\"," +
                                "\"@custom.annotation\": null," +
                                "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\": \"TestModel.CityType\"," +
                                "\"Name\": \"Value\"" +
                            "}",
                        ExpectedPayload = PayloadBuilder.Entity("TestModel.CityType"),
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntryAndFeedDeserializer_EntryTypeAnnotationNotFirst")
                    },
                    new
                    {
                        Description = "Custom property annotation - should be ignored.",
                        Json = "{" +
                                "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#TestModel.DefaultContainer.Cities()/$entity\"," +
                                "\"" + JsonLightUtils.GetPropertyAnnotationName("Name", "custom.annotation") + "\": null," +
                                "\"Name\": \"Value\"," +
                                "\"Id\":1" +
                            "}",
                        ExpectedPayload = PayloadBuilder.Entity("TestModel.CityType")
                            .PrimitiveProperty("Name", "Value")
                            .PrimitiveProperty("Id", 1),
                        ExpectedException = (ExpectedException)null
                    },
                    new
                    {
                        Description = "Duplicate custom property annotation - should be fail.",
                        Json = "{" +
                                "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#TestModel.DefaultContainer.Cities()/$entity\"," +
                                "\"" + JsonLightUtils.GetPropertyAnnotationName("Name", "custom.annotation") + "\": null," +
                                "\"" + JsonLightUtils.GetPropertyAnnotationName("Name", "custom.annotation") + "\": 42," +
                                "\"Name\": \"Value\"" +
                            "}",
                        ExpectedPayload = PayloadBuilder.Entity("TestModel.CityType"),
                        ExpectedException = ODataExpectedExceptions.ODataException("DuplicatePropertyNamesChecker_DuplicateAnnotationForPropertyNotAllowed", "custom.annotation", "Name")
                    },
                    new
                    {
                        Description = "Unrecognized odata property annotation - should be ignored.",
                        Json = "{" +
                                "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#TestModel.DefaultContainer.Cities()/$entity\"," +
                                "\"" + JsonLightUtils.GetPropertyAnnotationName("Name", JsonLightConstants.ODataAnnotationNamespacePrefix + "unknown") + "\": null," +
                                "\"Name\": \"Value\"," +
                                "\"Id\": 1" +
                            "}",
                        ExpectedPayload = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Name", "Value").PrimitiveProperty("Id", 1),
                        ExpectedException = (ExpectedException)null
                    },
                    new
                    {
                        Description = "Custom property annotation after the property - should fail.",
                        Json = "{" +
                                "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#TestModel.DefaultContainer.Cities()/$entity\"," +
                                "\"Name\": \"Value\"," +
                                "\"" + JsonLightUtils.GetPropertyAnnotationName("Name", "custom.annotation") + "\": null" +
                            "}",
                        ExpectedPayload = PayloadBuilder.Entity("TestModel.CityType"),
                        ExpectedException = ODataExpectedExceptions.ODataException("DuplicatePropertyNamesChecker_PropertyAnnotationAfterTheProperty", "custom.annotation", "Name")
                    },
                    new
                    {
                        Description = "OData property annotation.",
                        Json = "{" +
                                "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#TestModel.DefaultContainer.Cities()/$entity\"," +
                                "\"" + JsonLightUtils.GetPropertyAnnotationName("Name", JsonLightConstants.ODataTypeAnnotationName) + "\": \"Edm.String\"," +
                                "\"Name\": \"Value\"," +
                                "\"Id\":1" +
                            "}",
                        ExpectedPayload = PayloadBuilder.Entity("TestModel.CityType")
                            .PrimitiveProperty("Name", "Value")
                            .PrimitiveProperty("Id", 1),
                        ExpectedException = (ExpectedException)null
                    },
                    new
                    {
                        Description = "Duplicate odata property annotation.",
                        Json = "{" +
                                "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#TestModel.DefaultContainer.Cities()/$entity\"," +
                                "\"" + JsonLightUtils.GetPropertyAnnotationName("Name", JsonLightConstants.ODataTypeAnnotationName) + "\": \"Edm.Int32\"," +
                                "\"" + JsonLightUtils.GetPropertyAnnotationName("Name", JsonLightConstants.ODataTypeAnnotationName) + "\": \"Edm.String\"," +
                                "\"Name\": \"Value\"" +
                            "}",
                        ExpectedPayload = PayloadBuilder.Entity("TestModel.CityType"),
                        ExpectedException = ODataExpectedExceptions.ODataException("DuplicatePropertyNamesChecker_DuplicateAnnotationForPropertyNotAllowed", JsonLightConstants.ODataTypeAnnotationName, "Name")
                    },
                };

            testDescriptors = testDescriptors.Concat(explicitPayloads.Select(payload =>
            {
                return new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = payload.ExpectedPayload.DeepCopy()
                        .JsonRepresentation(payload.Json)
                        // JSON Light payloads don't store complex type names since the type can be inferred from the context URI and or API.
                        .SetAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null })
                        .ExpectedEntityType(cityType, citiesEntitySet),
                    PayloadEdmModel = model,
                    ExpectedException = payload.ExpectedException,
                    DebugDescription = payload.Description,
                };
            }));

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

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Verifies correct reading of data properties.")]
        public void DataPropertyTest()
        {
            var addressType = new EdmComplexType("TestModel", "Address");
            addressType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(isNullable: false));

            var testCases = new DataPropertyTestCase[]
            {
                new DataPropertyTestCase
                {
                    DebugDescription = "Property with object value and type name annotation - should fail.",
                    ExpectedProperty = PayloadBuilder.Property("TestProperty", PayloadBuilder.ComplexValue("TestModel.Address")),
                    EdmPropertyType = new EdmComplexTypeReference(addressType, false),
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("TestProperty", JsonLightConstants.ODataTypeAnnotationName) + "\":\"TestModel.Address\"," +
                        "\"TestProperty\":{}",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightPropertyAndValueDeserializer_ComplexValueWithPropertyTypeAnnotation", JsonLightConstants.ODataTypeAnnotationName)
                },
                new DataPropertyTestCase
                {
                    DebugDescription = "String property with type annotation after the property - should fail.",
                    ExpectedProperty = PayloadBuilder.PrimitiveProperty("TestProperty", "value"),
                    EdmPropertyType = new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), false),
                    Json = 
                        "\"TestProperty\":\"value\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("TestProperty", JsonLightConstants.ODataTypeAnnotationName) + "\":\"Edm.String\"",
                    ExpectedException = ODataExpectedExceptions.ODataException("DuplicatePropertyNamesChecker_PropertyAnnotationAfterTheProperty", JsonLightConstants.ODataTypeAnnotationName, "TestProperty"),
                },
                new DataPropertyTestCase
                {
                    DebugDescription = "String property with null type annotation - should fail.",
                    ExpectedProperty = PayloadBuilder.PrimitiveProperty("TestProperty", "value"),
                    EdmPropertyType = new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), false),
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("TestProperty", JsonLightConstants.ODataTypeAnnotationName) + "\":null," +
                        "\"TestProperty\":\"value\"",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightPropertyAndValueDeserializer_InvalidTypeName", string.Empty)
                },
                new DataPropertyTestCase
                {
                    DebugDescription = "null property with unknown type annotation (declared) - should fail.",
                    ExpectedProperty = PayloadBuilder.PrimitiveProperty("TestProperty", "value"),
                    EdmPropertyType = new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), false),
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("TestProperty", JsonLightConstants.ODataTypeAnnotationName) + "\":\"Unknown\"," +
                        "\"TestProperty\":null",
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_IncorrectTypeKind", "Unknown", "Primitive", "Complex"),
                    OnlyOnDeclaredProperty = true
                },
                new DataPropertyTestCase
                {
                    DebugDescription = "null property with unknown type annotation (open) - should fail.",
                    ExpectedProperty = PayloadBuilder.PrimitiveProperty("TestProperty", "value"),
                    EdmPropertyType = new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), false),
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("TestProperty", JsonLightConstants.ODataTypeAnnotationName) + "\":\"Unknown\"," +
                        "\"TestProperty\":null",
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_UnrecognizedTypeName", "Unknown"),
                    OnlyOnOpenProperty = true
                },
                new DataPropertyTestCase
                {
                    DebugDescription = "Open property with navigation link annotation should fail.",
                    ExpectedProperty = PayloadBuilder.Property("TestProperty", PayloadBuilder.PrimitiveMultiValue()),
                    EdmPropertyType = new EdmPrimitiveTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32), false),
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("TestProperty", JsonLightConstants.ODataTypeAnnotationName) + "\":\"Edm.Int32\"," +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("TestProperty", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/link\"," +
                        "\"TestProperty\":42",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightPropertyAndValueDeserializer_UnexpectedDataPropertyAnnotation", "TestProperty", JsonLightConstants.ODataNavigationLinkUrlAnnotationName)
                },
            };

            // Declared property
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = testCases.Where(testCase => !testCase.OnlyOnOpenProperty).Select(testCase =>
            {
                EdmModel model = new EdmModel();
                var testContainerClone = new EdmEntityContainer("TestModel", "DefaultContainer");
                model.AddElement(testContainerClone);

                // must use addressType declared before, becuase VerifyComplexType method use ReferenceEquals to do validation 
                model.AddElement(addressType);

                var entityType = new EdmEntityType("TestModel", "EntityType");
                entityType.AddKeys(entityType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(isNullable: false)));
                entityType.AddStructuralProperty("TestProperty", testCase.EdmPropertyType);
                model.AddElement(entityType);

                var entitiesSet = testContainerClone.AddEntitySet("Entities", entityType);

                return new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = testCase.DebugDescription + " (declared)",
                    PayloadElement = PayloadBuilder.Entity("TestModel.Entity")
                        .Property(testCase.ExpectedProperty)
                        .JsonRepresentation("{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#TestModel.DefaultContainer.Entities/TestModel.EntityType/$entity\"," +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.EntityType\"," +
                            testCase.Json +
                            "}")
                        .ExpectedEntityType(entityType, entitiesSet),
                    PayloadEdmModel = model,
                    ExpectedException = testCase.ExpectedException
                };
            });

            // Open property
            testDescriptors = testDescriptors.Concat(testCases.Where(testCase => !testCase.OnlyOnDeclaredProperty).Select(testCase =>
            {
                EdmModel model = new EdmModel();
                var testContainerClone = new EdmEntityContainer("TestModel", "DefaultContainer");
                model.AddElement(testContainerClone);

                // must use addressType declared before, becuase VerifyComplexType method use ReferenceEquals to do validation 
                model.AddElement(addressType);

                var entityType = new EdmEntityType("TestModel", "EntityType", baseType: null, isAbstract: false, isOpen: true);
                entityType.AddKeys(entityType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(isNullable: false)));
                model.AddElement(entityType);

                var entitiesSet = testContainerClone.AddEntitySet("Entities", entityType);

                return new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = testCase.DebugDescription + " (open property)",
                    PayloadElement = PayloadBuilder.Entity("TestModel.Entity")
                        .Property(testCase.ExpectedProperty)
                        .JsonRepresentation("{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#TestModel.DefaultContainer.Entities/TestModel.EntityType/$entity\"," +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.EntityType\"," +
                            testCase.Json +
                            "}")
                        .ExpectedEntityType(entityType, entitiesSet),
                    PayloadEdmModel = model,
                    ExpectedException = testCase.ExpectedOpenPropertyException ?? testCase.ExpectedException
                };
            }));

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

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Verifies correct reading of open properties on entry.")]
        public void OpenPropertiesTest()
        {
            IEdmModel model = Test.OData.Utils.Metadata.TestModels.BuildTestModel();
            IEdmEntityContainer container = model.FindEntityContainer("DefaultContainer");
            IEdmEntitySet citiesEntitySet = container.FindEntitySet("Cities");
            IEdmType cityType = model.FindType("TestModel.CityType");

            var testCases = new OpenPropertyTestCase[]
            {
                new OpenPropertyTestCase
                {
                    DebugDescription = "Simple integer open property without type information.",
                    ExpectedProperty = PayloadBuilder.PrimitiveProperty("OpenProperty", 42),
                    Json = "\"OpenProperty\":42",
                },
                new OpenPropertyTestCase
                {
                    DebugDescription = "null open property without type information.",
                    ExpectedProperty = PayloadBuilder.PrimitiveProperty("OpenProperty", null),
                    Json = "\"OpenProperty\":null",
                },
                new OpenPropertyTestCase
                {
                    DebugDescription = "String open property with string type information.",
                    ExpectedProperty = PayloadBuilder.PrimitiveProperty("OpenProperty", "value"),
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("OpenProperty", JsonLightConstants.ODataTypeAnnotationName) + "\":\"Edm.String\"," +
                        "\"OpenProperty\":\"value\"",
                },
                new OpenPropertyTestCase
                {
                    DebugDescription = "String open property with date time value without type information.",
                    ExpectedProperty = PayloadBuilder.PrimitiveProperty("OpenProperty", "2012-04-13T02:43:10.215Z"),
                    Json = "\"OpenProperty\":\"2012-04-13T02:43:10.215Z\"",
                },
                new OpenPropertyTestCase
                {
                    DebugDescription = "DateTimeOffset open property with type information.",
                    ExpectedProperty = PayloadBuilder.PrimitiveProperty("OpenProperty", new DateTimeOffset(2012, 4, 13, 2, 43, 10, 215, TimeSpan.Zero)),
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("OpenProperty", JsonLightConstants.ODataTypeAnnotationName) + "\":\"Edm.DateTimeOffset\"," +
                        "\"OpenProperty\":\"2012-04-13T02:43:10.215Z\"",
                },
                new OpenPropertyTestCase
                {
                    DebugDescription = "Spatial open property with type information - should work.",
                    ExpectedProperty = PayloadBuilder.PrimitiveProperty("OpenProperty", GeographyFactory.Point(25.0, 23.2).Build()),
                    Json = "\"" + JsonLightUtils.GetPropertyAnnotationName("OpenProperty", JsonLightConstants.ODataTypeAnnotationName) + "\":\"Edm.GeographyPoint\"," + 
                           "\"OpenProperty\":" + SpatialUtils.GetSpatialStringValue(ODataFormat.Json, GeographyFactory.Point(25.0, 23.2).Build()),
                },
                new OpenPropertyTestCase
                {
                    DebugDescription = "Empty complex open property with type information.",
                    ExpectedProperty = PayloadBuilder.Property("OpenProperty", PayloadBuilder.ComplexValue("TestModel.Address")),
                    Json = "\"OpenProperty\":{ \"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.Address\"}",
                },
                new OpenPropertyTestCase
                {
                    DebugDescription = "Complex property with data with type information.",
                    ExpectedProperty = PayloadBuilder.Property("OpenProperty", PayloadBuilder.ComplexValue("TestModel.Address").PrimitiveProperty("Street", "First")),
                    Json = "\"OpenProperty\":{ \"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.Address\", \"Street\":\"First\"}",
                },
                new OpenPropertyTestCase
                {
                    DebugDescription = "String open property with complex type information - should fail.",
                    ExpectedProperty = PayloadBuilder.PrimitiveProperty("OpenProperty", "value"),
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("OpenProperty", JsonLightConstants.ODataTypeAnnotationName) + "\":\"TestModel.Address\"," +
                        "\"OpenProperty\":\"value\"",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightPropertyAndValueDeserializer_ComplexValueWithPropertyTypeAnnotation")
                },
                new OpenPropertyTestCase
                {
                    DebugDescription = "Open collection property - should pass.",
                    ExpectedProperty = PayloadBuilder.Property("OpenProperty", PayloadBuilder.PrimitiveMultiValue(EntityModelUtils.GetCollectionTypeName("Edm.Int32")).Item(1)),
                    Json = 
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("OpenProperty", JsonLightConstants.ODataTypeAnnotationName) + "\":\"Collection(Edm.Int32)\"," +
                        "\"OpenProperty\":[1]",
                },
            };

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = testCases.Select(testCase =>
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = testCase.DebugDescription,
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityOpenType").PrimitiveProperty("Id", 1)
                        .Property(testCase.ExpectedProperty)
                        .JsonRepresentation("{" +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#TestModel.DefaultContainer.Cities/TestModel.CityOpenType()/$entity\"," +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.CityOpenType\"," +
                            "\"Id\":1," +
                            testCase.Json +
                            "}")
                        .ExpectedEntityType(cityType, citiesEntitySet),
                    PayloadEdmModel = model,
                    ExpectedException = testCase.ExpectedException
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

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Verifies correct reading of projected stream properties (in the payload and templatized).")]
        public void StreamPropertiesProjectionTest()
        {
            EdmModel model = new EdmModel();
            var container = new EdmEntityContainer("TestModel", "DefaultContainer");
            model.AddElement(container);
            EdmEntityType townType = model.EntityType("TownType");
            townType.KeyProperty("Id", EdmCoreModel.Instance.GetInt32(false));
            townType.StreamProperty("MapSmall");
            townType.StreamProperty("MapMedium");
            townType.NavigationProperty("NavProp", townType);

            EdmEntityType cityType = model.EntityType("CityType", null, townType);
            cityType.StreamProperty("CityLogo");

            EdmEntitySet townsSet = model.EntitySet("Towns", townType);
            model.Fixup();

            var testCases = new ProjectionTestCase[]
            {
                #region No $select
                new ProjectionTestCase
                {
                    DebugDescription = "No $select => three templatized properties.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).StreamProperty("MapSmall").StreamProperty("MapMedium").StreamProperty("CityLogo").NavigationProperty("NavProp", /*url*/null),
                    ProjectionString = null,
                },
                new ProjectionTestCase
                {
                    DebugDescription = "No $select + one property in the payload => two templatized properties.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).StreamProperty("MapSmall", "http://odata.org/stream/read"),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).StreamProperty("MapSmall", "http://odata.org/stream/read").StreamProperty("MapMedium").StreamProperty("CityLogo").NavigationProperty("NavProp", /*url*/null),
                    ProjectionString = null,
                },
                new ProjectionTestCase
                {
                    DebugDescription = "No $select + three properties in the payload => no templatized properties.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).StreamProperty("MapSmall", "http://odata.org/stream/read").StreamProperty("MapMedium", "http://odata.org/stream/read").StreamProperty("CityLogo", "http://odata.org/stream/read"),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).StreamProperty("MapSmall", "http://odata.org/stream/read").StreamProperty("MapMedium", "http://odata.org/stream/read").StreamProperty("CityLogo", "http://odata.org/stream/read").NavigationProperty("NavProp", /*url*/null),
                    ProjectionString = null,
                },
                #endregion No $select
                #region Empty $select
                new ProjectionTestCase
                {
                    DebugDescription = "Empty $select => no templatized properties.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1),
                    ProjectionString = string.Empty,
                },
                new ProjectionTestCase
                {
                    DebugDescription = "Empty $select + one property in the payload => no templatized properties.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).StreamProperty("MapSmall", "http://odata.org/stream/read"),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).StreamProperty("MapSmall", "http://odata.org/stream/read"),
                    ProjectionString = string.Empty,
                },
                new ProjectionTestCase
                {
                    DebugDescription = "Empty $select + three properties in the payload => no templatized properties.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).StreamProperty("MapSmall", "http://odata.org/stream/read").StreamProperty("MapMedium", "http://odata.org/stream/read").StreamProperty("CityLogo", "http://odata.org/stream/read"),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).StreamProperty("MapSmall", "http://odata.org/stream/read").StreamProperty("MapMedium", "http://odata.org/stream/read").StreamProperty("CityLogo", "http://odata.org/stream/read"),
                    ProjectionString = string.Empty,
                },
                #endregion Empty $select
                #region $select=*
                new ProjectionTestCase
                {
                    DebugDescription = "$select=* => three templatized stream properties and and one templatized navigation.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).StreamProperty("MapSmall").StreamProperty("MapMedium").StreamProperty("CityLogo").NavigationProperty("NavProp", /*url*/null),
                    ProjectionString = "*",
                },
                new ProjectionTestCase
                {
                    DebugDescription = "$select=* + one property in the payload => two templatized stream properties and one templatized navigation.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).StreamProperty("MapSmall", "http://odata.org/stream/read"),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).StreamProperty("MapSmall", "http://odata.org/stream/read").StreamProperty("MapMedium").StreamProperty("CityLogo").NavigationProperty("NavProp", /*url*/null),
                    ProjectionString = "*",
                },
                new ProjectionTestCase
                {
                    DebugDescription = "$select=* + three properties in the payload => no templatized stream properties, one templatized navigation.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).StreamProperty("MapSmall", "http://odata.org/stream/read").StreamProperty("MapMedium", "http://odata.org/stream/read").StreamProperty("CityLogo", "http://odata.org/stream/read"),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).StreamProperty("MapSmall", "http://odata.org/stream/read").StreamProperty("MapMedium", "http://odata.org/stream/read").StreamProperty("CityLogo", "http://odata.org/stream/read").NavigationProperty("NavProp", /*url*/null),
                    ProjectionString = "*",
                },
                #endregion $select=*
                #region $select=MapMedium
                new ProjectionTestCase
                {
                    DebugDescription = "$select=MapMedium => one templatized properties.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).StreamProperty("MapMedium"),
                    ProjectionString = "MapMedium",
                },
                new ProjectionTestCase
                {
                    DebugDescription = "$select=MapMedium + MapMedium property in the payload => no templatized properties.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).StreamProperty("MapMedium", "http://odata.org/stream/read"),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).StreamProperty("MapMedium", "http://odata.org/stream/read"),
                    ProjectionString = "MapMedium",
                },
                new ProjectionTestCase
                {
                    DebugDescription = "$select=MapMedium + MapSmall property in the payload => one templatized properties.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).StreamProperty("MapSmall", "http://odata.org/stream/read"),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).StreamProperty("MapSmall", "http://odata.org/stream/read").StreamProperty("MapMedium"),
                    ProjectionString = "MapMedium",
                },
                new ProjectionTestCase
                {
                    DebugDescription = "$select=MapMedium + three properties in the payload => no templatized properties.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).StreamProperty("MapSmall", "http://odata.org/stream/read").StreamProperty("MapMedium", "http://odata.org/stream/read").StreamProperty("CityLogo", "http://odata.org/stream/read"),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).StreamProperty("MapSmall", "http://odata.org/stream/read").StreamProperty("MapMedium", "http://odata.org/stream/read").StreamProperty("CityLogo", "http://odata.org/stream/read"),
                    ProjectionString = "MapMedium",
                },
                #endregion $select=MapMedium
                #region $select=NavProp/*
                new ProjectionTestCase
                {
                    DebugDescription = "$select=NavProp/* => three templatized stream properties and and one templatized navigation.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).ExpandedNavigationProperty("NavProp", PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 2)),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).ExpandedNavigationProperty("NavProp", PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 2).StreamProperty("MapSmall").StreamProperty("MapMedium").StreamProperty("CityLogo").NavigationProperty("NavProp", /*url*/null)),
                    ProjectionString = "NavProp/*",
                },
                new ProjectionTestCase
                {
                    DebugDescription = "$select=NavProp/* + one property in the payload => two templatized stream properties and one templatized navigation.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).ExpandedNavigationProperty("NavProp", PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 2).StreamProperty("MapSmall", "http://odata.org/stream/read")),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).ExpandedNavigationProperty("NavProp", PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 2).StreamProperty("MapSmall", "http://odata.org/stream/read").StreamProperty("MapMedium").StreamProperty("CityLogo").NavigationProperty("NavProp", /*url*/null)),
                    ProjectionString = "NavProp/*",
                },
                new ProjectionTestCase
                {
                    DebugDescription = "$select=NavProp/* + three properties in the payload => no templatized stream properties, one templatized navigation.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).ExpandedNavigationProperty("NavProp", PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 2).StreamProperty("MapSmall", "http://odata.org/stream/read").StreamProperty("MapMedium", "http://odata.org/stream/read").StreamProperty("CityLogo", "http://odata.org/stream/read")),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).ExpandedNavigationProperty("NavProp", PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 2).StreamProperty("MapSmall", "http://odata.org/stream/read").StreamProperty("MapMedium", "http://odata.org/stream/read").StreamProperty("CityLogo", "http://odata.org/stream/read").NavigationProperty("NavProp", /*url*/null)),
                    ProjectionString = "NavProp/*",
                },
                #endregion $select=NavProp/*
                #region $select=NavProp/NavProp/MapMedium
                new ProjectionTestCase
                {
                    DebugDescription = "$select=NavProp/NavProp/MapMedium => one templatized properties.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).ExpandedNavigationProperty("NavProp", PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 2).ExpandedNavigationProperty("NavProp", PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 3))),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).ExpandedNavigationProperty("NavProp", PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 2).ExpandedNavigationProperty("NavProp", PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 3).StreamProperty("MapMedium"))),
                    ProjectionString = "NavProp/NavProp/MapMedium",
                },
                #endregion $select=NavProp/NavProp/MapMedium
                #region $select=NavProp/NavProp
                new ProjectionTestCase
                {
                    DebugDescription = "$select=NavProp/NavProp => three templatized stream properties and one navigation property.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).ExpandedNavigationProperty("NavProp", PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 2).ExpandedNavigationProperty("NavProp", PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 3))),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).ExpandedNavigationProperty("NavProp", PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 2).ExpandedNavigationProperty("NavProp", PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 3).StreamProperty("MapSmall").StreamProperty("MapMedium").StreamProperty("CityLogo").NavigationProperty("NavProp", /*url*/null))),
                    ProjectionString = "NavProp/NavProp",
                },
                #endregion $select=NavProp/NavProp
                #region $select=TestModel.CityType/CityLogo
                new ProjectionTestCase
                {
                    DebugDescription = "$select=TestModel.CityType/CityLogo on base type => no templatized properties.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.TownType").PrimitiveProperty("Id", 1),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.TownType").PrimitiveProperty("Id", 1),
                    ProjectionString = "TestModel.CityType/CityLogo",
                },
                new ProjectionTestCase
                {
                    DebugDescription = "$select=TestModel.CityType/CityLogo on derived type => specific property templatized.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).StreamProperty("CityLogo"),
                    ProjectionString = "TestModel.CityType/CityLogo",
                },
                #endregion $select=TestModel.CityType/CityLogo
                #region $select=TestModel.CityType/MapSmall
                new ProjectionTestCase
                {
                    DebugDescription = "$select=TestModel.CityType/MapSmall on base type => no templatized properties.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.TownType").PrimitiveProperty("Id", 1),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.TownType").PrimitiveProperty("Id", 1),
                    ProjectionString = "TestModel.CityType/MapSmall",
                },
                new ProjectionTestCase
                {
                    DebugDescription = "$select=TestModel.CityType/MapSmall on derived type => specific property templatized.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).StreamProperty("MapSmall"),
                    ProjectionString = "TestModel.CityType/MapSmall",
                },
                #endregion $select=TestModel.CityType/MapSmall
                #region $select=TestModel.TownType/MapSmall
                new ProjectionTestCase
                {
                    DebugDescription = "$select=TestModel.TownType/MapSmall on base type => specific property templatized..",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.TownType").PrimitiveProperty("Id", 1),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.TownType").PrimitiveProperty("Id", 1).StreamProperty("MapSmall"),
                    ProjectionString = "TestModel.TownType/MapSmall",
                },
                new ProjectionTestCase
                {
                    DebugDescription = "$select=TestModel.TownType/MapSmall on derived type => specific property templatized.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).StreamProperty("MapSmall"),
                    ProjectionString = "TestModel.TownType/MapSmall",
                },
                #endregion $select=TestModel.TownType/MapSmall
            };

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = testCases.Select(testCase =>
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = testCase.DebugDescription,
                    PayloadEdmModel = model,
                    PayloadElement = testCase.PayloadEntity
                        .WithContextUriProjection(testCase.ProjectionString)
                        .ExpectedEntityType(townType, townsSet),
                    ExpectedResultPayloadElement = tc => testCase.ExpectedEntity,
                    ExpectedException = testCase.ExpectedException,
                });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations.Where(tc => !tc.IsRequest),
                (testDescriptor, testConfiguration) =>
                {
                    // These descriptors are already tailored specifically for Json Light and 
                    // do not require normalization.
                    testDescriptor.TestDescriptorNormalizers.Clear();
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Verifies correct reading of projected navigation properties (in the payload and templatized).")]
        public void NavigationPropertiesProjectionTest()
        {
            EdmModel model = new EdmModel();

            EdmEntityType townType = model.EntityType("TownType");
            townType.KeyProperty("Id", EdmCoreModel.Instance.GetInt32(false) as EdmTypeReference);
            townType.NavigationProperty("NavProp1", townType);
            EdmEntitySet townsSet = model.EntitySet("Towns", townType);

            EdmEntityType cityType = new EdmEntityType("TestModel", "CityType", townType);
            model.AddElement(cityType);
            cityType.NavigationProperty("NavProp2", townType);
            model.EntitySet("Cities", cityType);

            EdmEntityType cityType2 = new EdmEntityType("TestModel", "DuplicateCityType", townType);
            model.AddElement(cityType2);
            cityType2.NavigationProperty("NavProp2", townType);
            model.EntitySet("DuplicateCities", cityType2);

            model.Fixup();

            var testCases = new ProjectionTestCase[]
            {
                #region No $select
                new ProjectionTestCase
                {
                    DebugDescription = "No $select => two templatized properties.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1)
                        .NavigationProperty("NavProp1", /*url*/null).NavigationProperty("NavProp2", /*url*/null),
                    ProjectionString = null,
                },
                new ProjectionTestCase
                {
                    DebugDescription = "No $select + one property in the payload => one templatized properties.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).NavigationProperty("NavProp1", "http://odata.org/nav1"),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1)
                        .NavigationProperty("NavProp1", "http://odata.org/nav1").NavigationProperty("NavProp2", /*url*/null),
                    ProjectionString = null,
                },
                new ProjectionTestCase
                {
                    DebugDescription = "No $select + two properties in the payload => no templatized properties.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).NavigationProperty("NavProp1", "http://odata.org/nav1").NavigationProperty("NavProp2", "http://odata.org/nav2"),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1)
                        .NavigationProperty("NavProp1", "http://odata.org/nav1").NavigationProperty("NavProp2", "http://odata.org/nav2"),
                    ProjectionString = null,
                },
                #endregion No $select
                #region Empty $select
                new ProjectionTestCase
                {
                    DebugDescription = "Empty $select => no templatized properties.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1),
                    ProjectionString = string.Empty,
                },
                new ProjectionTestCase
                {
                    DebugDescription = "Empty $select + one property in the payload => no templatized properties.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).NavigationProperty("NavProp1", "http://odata.org/nav1"),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).NavigationProperty("NavProp1", "http://odata.org/nav1"),
                    ProjectionString = string.Empty,
                },
                new ProjectionTestCase
                {
                    DebugDescription = "Empty $select + two properties in the payload => no templatized properties.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).NavigationProperty("NavProp1", "http://odata.org/nav1").NavigationProperty("NavProp2", "http://odata.org/nav2"),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).NavigationProperty("NavProp1", "http://odata.org/nav1").NavigationProperty("NavProp2", "http://odata.org/nav2"),
                    ProjectionString = string.Empty,
                },
                #endregion Empty $select
                #region $select=*
                new ProjectionTestCase
                {
                    DebugDescription = "$select=* => two templatized properties.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).NavigationProperty("NavProp1", /*url*/null).NavigationProperty("NavProp2", /*url*/null),
                    ProjectionString = "*",
                },
                new ProjectionTestCase
                {
                    DebugDescription = "$select=* + one property in the payload => one templatized property.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).NavigationProperty("NavProp2", "http://odata.org/nav2"),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).NavigationProperty("NavProp2", "http://odata.org/nav2").NavigationProperty("NavProp1", /*url*/null),
                    ProjectionString = "*",
                },
                new ProjectionTestCase
                {
                    DebugDescription = "$select=* + two properties in the payload => no templatized properties.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).NavigationProperty("NavProp1", "http://odata.org/nav1").NavigationProperty("NavProp2", "http://odata.org/nav2"),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).NavigationProperty("NavProp1", "http://odata.org/nav1").NavigationProperty("NavProp2", "http://odata.org/nav2"),
                    ProjectionString = "*",
                },
                #endregion $select=*
                #region $select=NavProp2,*
                new ProjectionTestCase
                {
                    DebugDescription = "$select=NavProp2,* => two templatized properties.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).NavigationProperty("NavProp1", /*url*/null).NavigationProperty("NavProp2", /*url*/null),
                    ProjectionString = "NavProp2,*",
                },
                new ProjectionTestCase
                {
                    DebugDescription = "$select=NavProp2,* + NavProp2 property in the payload => one templatized property.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).NavigationProperty("NavProp2", "http://odata.org/nav2"),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).NavigationProperty("NavProp2", "http://odata.org/nav2").NavigationProperty("NavProp1", /*url*/null),
                    ProjectionString = "NavProp2,*",
                },
                new ProjectionTestCase
                {
                    DebugDescription = "$select=NavProp2,* + NavProp1 property in the payload => one templatized property.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).NavigationProperty("NavProp1", "http://odata.org/nav1"),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).NavigationProperty("NavProp1", "http://odata.org/nav1").NavigationProperty("NavProp2", /*url*/null),
                    ProjectionString = "NavProp2,*",
                },
                new ProjectionTestCase
                {
                    DebugDescription = "$select=NavProp2,* + two properties in the payload => no templatized properties.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).NavigationProperty("NavProp1", "http://odata.org/nav1").NavigationProperty("NavProp2", "http://odata.org/nav2"),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).NavigationProperty("NavProp1", "http://odata.org/nav1").NavigationProperty("NavProp2", "http://odata.org/nav2"),
                    ProjectionString = "NavProp2,*",
                },
                #endregion $select=NavProp2,*
                #region $select=NavProp1,NavProp2
                new ProjectionTestCase
                {
                    DebugDescription = "$select=NavProp1,NavProp2 => two templatized properties.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).NavigationProperty("NavProp1", /*url*/null).NavigationProperty("NavProp2", /*url*/null),
                    ProjectionString = "NavProp1,NavProp2",
                },
                new ProjectionTestCase
                {
                    DebugDescription = "$select=NavProp1,NavProp2 + NavProp2 property in the payload => one templatized properties.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).NavigationProperty("NavProp2", "http://odata.org/nav2"),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).NavigationProperty("NavProp2", "http://odata.org/nav2").NavigationProperty("NavProp1", /*url*/null),
                    ProjectionString = "NavProp1,NavProp2",
                },
                new ProjectionTestCase
                {
                    DebugDescription = "$select=NavProp1,NavProp2 + two properties in the payload => no templatized properties.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).NavigationProperty("NavProp1", "http://odata.org/nav1").NavigationProperty("NavProp2", "http://odata.org/nav2"),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).NavigationProperty("NavProp1", "http://odata.org/nav1").NavigationProperty("NavProp2", "http://odata.org/nav2"),
                    ProjectionString = "NavProp1,NavProp2",
                },
                #endregion $select=NavProp1,NavProp2
                #region $select=NavProp2
                new ProjectionTestCase
                {
                    DebugDescription = "$select=NavProp2 => one templatized property.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).NavigationProperty("NavProp2", /*url*/null),
                    ProjectionString = "NavProp2",
                },
                new ProjectionTestCase
                {
                    DebugDescription = "$select=NavProp2 + NavProp2 property in the payload => no templatized properties.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).NavigationProperty("NavProp2", "http://odata.org/nav2"),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).NavigationProperty("NavProp2", "http://odata.org/nav2"),
                    ProjectionString = "NavProp2",
                },
                new ProjectionTestCase
                {
                    DebugDescription = "$select=NavProp2 + NavProp1 property in the payload => one templatized property.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).NavigationProperty("NavProp1", "http://odata.org/nav1"),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).NavigationProperty("NavProp1", "http://odata.org/nav1").NavigationProperty("NavProp2", /*url*/null),
                    ProjectionString = "NavProp2",
                },
                new ProjectionTestCase
                {
                    DebugDescription = "$select=NavProp2 + two properties in the payload => no templatized properties.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).NavigationProperty("NavProp1", "http://odata.org/nav1").NavigationProperty("NavProp2", "http://odata.org/nav2"),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).NavigationProperty("NavProp1", "http://odata.org/nav1").NavigationProperty("NavProp2", "http://odata.org/nav2"),
                    ProjectionString = "NavProp2",
                },
                #endregion $select=NavProp2
                #region $select=NavProp1/*
                new ProjectionTestCase
                {
                    DebugDescription = "$select=NavProp1/* => three templatized stream properties and and one templatized navigation.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).ExpandedNavigationProperty("NavProp1", PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 2)),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).ExpandedNavigationProperty("NavProp1", PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 2).NavigationProperty("NavProp1", /*url*/null).NavigationProperty("NavProp2", /*url*/null)),
                    ProjectionString = "NavProp1/*",
                },
                #endregion $select=NavProp/*
                #region $select=NavProp1/NavProp1
                new ProjectionTestCase
                {
                    DebugDescription = "$select=NavProp1/NavProp1 => one templatized navigation property.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).ExpandedNavigationProperty("NavProp1", PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 2)),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).ExpandedNavigationProperty("NavProp1", PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 2).NavigationProperty("NavProp1", /*url*/null)),
                    ProjectionString = "NavProp1/NavProp1",
                },
                new ProjectionTestCase
                {
                    DebugDescription = "$select=NavProp1/NavProp1 with expand => two templatized navigation properties.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).ExpandedNavigationProperty("NavProp1", PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 2).ExpandedNavigationProperty("NavProp1", PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 3))),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).ExpandedNavigationProperty("NavProp1", PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 2).ExpandedNavigationProperty("NavProp1", PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 3).NavigationProperty("NavProp1", /*url*/null).NavigationProperty("NavProp2", /*url*/null))),
                    ProjectionString = "NavProp1/NavProp1",
                },
                #endregion $select=NavProp1/NavProp1
                #region $select=TestModel.CityType/NavProp2
                new ProjectionTestCase
                {
                    DebugDescription = "$select=TestModel.CityType/NavProp2 on base type => no templatized properties.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.TownType").PrimitiveProperty("Id", 1),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.TownType").PrimitiveProperty("Id", 1),
                    ProjectionString = "TestModel.CityType/NavProp2",
                },
                new ProjectionTestCase
                {
                    DebugDescription = "$select=TestModel.CityType/NavProp2 on derived type => specific property templatized.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).NavigationProperty("NavProp2", /*url*/null),
                    ProjectionString = "TestModel.CityType/NavProp2",
                },
                new ProjectionTestCase
                {
                    DebugDescription = "$select=TestModel.CityType/NavProp2 on different derived type => no templatized properties.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.DuplicateCityType").PrimitiveProperty("Id", 1),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.DuplicateCityType").PrimitiveProperty("Id", 1),
                    ProjectionString = "TestModel.CityType/NavProp2",
                },
                #endregion $select=TestModel.CityType/NavProp2
                #region $select=TestModel.CityType/NavProp1
                new ProjectionTestCase
                {
                    DebugDescription = "$select=TestModel.CityType/NavProp1 on base type => no templatized properties.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.TownType").PrimitiveProperty("Id", 1),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.TownType").PrimitiveProperty("Id", 1),
                    ProjectionString = "TestModel.CityType/NavProp1",
                },
                new ProjectionTestCase
                {
                    DebugDescription = "$select=TestModel.CityType/NavProp1 on derived type => specific property templatized.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).NavigationProperty("NavProp1", /*url*/null),
                    ProjectionString = "TestModel.CityType/NavProp1",
                },
                #endregion $select=TestModel.CityType/NavProp1
                #region $select=TestModel.TownType/NavProp1
                new ProjectionTestCase
                {
                    DebugDescription = "$select=TestModel.TownType/NavProp1 on base type => specific property templatized..",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.TownType").PrimitiveProperty("Id", 1),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.TownType").PrimitiveProperty("Id", 1).NavigationProperty("NavProp1", /*url*/null),
                    ProjectionString = "TestModel.TownType/NavProp1",
                },
                new ProjectionTestCase
                {
                    DebugDescription = "$select=TestModel.TownType/NavProp1 on derived type => specific property templatized.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).NavigationProperty("NavProp1", /*url*/null),
                    ProjectionString = "TestModel.TownType/NavProp1",
                },
                #endregion $select=TestModel.TownType/NavProp1
                #region $select=NavProp1/TestModel.City/NavProp2
                new ProjectionTestCase
                {
                    DebugDescription = "$select=NavProp1/TestModel.TownType/NavProp2 on expanded base type => no templatized property.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.TownType").PrimitiveProperty("Id", 1).ExpandedNavigationProperty("NavProp1", PayloadBuilder.Entity("TestModel.TownType").PrimitiveProperty("Id", 2)),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.TownType").PrimitiveProperty("Id", 1).ExpandedNavigationProperty("NavProp1", PayloadBuilder.Entity("TestModel.TownType").PrimitiveProperty("Id", 2)),
                    ProjectionString = "NavProp1/TestModel.TownType/NavProp2",
                },
                new ProjectionTestCase
                {
                    DebugDescription = "$select=NavProp1/TestModel.TownType/NavProp2 on expanded derived type type => specific templatized property.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.TownType").PrimitiveProperty("Id", 1).ExpandedNavigationProperty("NavProp1", PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 2)),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.TownType").PrimitiveProperty("Id", 1).ExpandedNavigationProperty("NavProp1", PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 2).NavigationProperty("NavProp2", /*url*/null)),
                    ProjectionString = "NavProp1/TestModel.TownType/NavProp2",
                },
                new ProjectionTestCase
                {
                    DebugDescription = "$select=TestModel.CityType/* on different derived type => no templatized properties.",
                    PayloadEntity = PayloadBuilder.Entity("TestModel.TownType").PrimitiveProperty("Id", 1).ExpandedNavigationProperty("NavProp1", PayloadBuilder.Entity("TestModel.DuplicateCityType").PrimitiveProperty("Id", 2)),
                    ExpectedEntity = PayloadBuilder.Entity("TestModel.TownType").PrimitiveProperty("Id", 1).ExpandedNavigationProperty("NavProp1", PayloadBuilder.Entity("TestModel.DuplicateCityType").PrimitiveProperty("Id", 2)),
                    ProjectionString = "TestModel.CityType/*",
                },
                #endregion $select=NavProp1/TestModel.City/NavProp2
            };

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = testCases.Select(testCase =>
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = testCase.DebugDescription,
                    PayloadEdmModel = model,
                    PayloadElement = testCase.PayloadEntity
                        .WithContextUriProjection(testCase.ProjectionString)
                        .ExpectedEntityType(townType, townsSet),
                    ExpectedResultPayloadElement = tc => testCase.ExpectedEntity,
                    ExpectedException = testCase.ExpectedException,
                });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations.Where(tc => !tc.IsRequest),
                (testDescriptor, testConfiguration) =>
                {
                    // These descriptors are already tailored specifically for Json Light and 
                    // do not require normalization.
                    testDescriptor.TestDescriptorNormalizers.Clear();
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Verifies correct reading of entries with sub context uri in json.")]
        public void EntryReaderWithSubContextUriTest()
        {
            IEdmModel model = Test.OData.Utils.Metadata.TestModels.BuildTestModel();
            IEdmEntityContainer container = model.FindEntityContainer("DefaultContainer");
            IEdmEntitySet citiesEntitySet = container.FindEntitySet("Cities");
            IEdmType cityType = model.FindType("TestModel.CityType");

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new[]
            {
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = "Entry with expanded navagation properties with sub context uri",
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType")
                        .PrimitiveProperty("Id", 1)
                        .ExpandedNavigationProperty(
                            "CityHall", 
                            PayloadBuilder.EntitySet(new EntityInstance[]
                            {
                                PayloadBuilder.Entity("TestModel.OfficeType").PrimitiveProperty("Id", 1).AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null }),
                            }))
                        .JsonRepresentation("{" +
                                "\"@odata.context\":\"http://odata.org/test/$metadata#Cities(Id,CityHall(Id))/$entity\"," +
                                "\"Id\":1," +
                                "\"CityHall\":[" +
                                    "{" +
                                        "\"@odata.context\":\"http://odata.org/test/$metadata#Offices/$entity\"," +
                                        "\"Id\":1" +
                                    "}" +
                                "]" +
                            "}")
                        .AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null })
                        .ExpectedEntityType(cityType, citiesEntitySet),
                    PayloadEdmModel = model
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.TestDescriptorNormalizers.Clear();
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        private sealed class InvalidEntryInstanceAnnotationValue
        {
            public string Json { get; set; }
            public Func<string, ExpectedException> ExpectedException { get; set; }
        }

        private sealed class DataPropertyTestCase
        {
            public string DebugDescription { get; set; }
            public PropertyInstance ExpectedProperty { get; set; }
            public IEdmTypeReference EdmPropertyType { get; set; }
            public string Json { get; set; }
            public ExpectedException ExpectedException { get; set; }
            public ExpectedException ExpectedOpenPropertyException { get; set; }
            public bool OnlyOnDeclaredProperty { get; set; }
            public bool OnlyOnOpenProperty { get; set; }
        }

        private sealed class OpenPropertyTestCase
        {
            public string DebugDescription { get; set; }
            public PropertyInstance ExpectedProperty { get; set; }
            public string Json { get; set; }
            public ExpectedException ExpectedException { get; set; }
        }

        private sealed class ProjectionTestCase
        {
            public string DebugDescription { get; set; }
            public EntityInstance PayloadEntity { get; set; }
            public EntityInstance ExpectedEntity { get; set; }
            public string ProjectionString { get; set; }
            public ExpectedException ExpectedException { get; set; }
        }
    }
}

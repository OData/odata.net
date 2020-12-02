//---------------------------------------------------------------------
// <copyright file="PropertyReaderJsonLightTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.JsonLight
{
    #region Namespaces

    using System;
    using System.Globalization;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Spatial;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Common.Annotations;
    using Microsoft.Test.Taupo.OData.Contracts.Json;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TestModels = Microsoft.Test.OData.Utils.Metadata.TestModels;

    #endregion Namespaces

    /// <summary>
    /// Tests reading of primitive values in JSON.
    /// </summary>
    [TestClass, TestCase]
    public class PropertyReaderJsonLightTests : ODataReaderTestCase
    {
        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [InjectDependency]
        public EntityModelSchemaToEdmModelConverter EntityModelSchemaToEdmModelConverter { get; set; }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Verifies correct reading of top-level property payloads")]
        public void TopLevelPropertyTest()
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
                        InjectedJSON = "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataAnnotationNamespacePrefix + "unknown\": { }",
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
                        ExpectedException = (ExpectedException)null
                    },
                };

            var payloads = new[]
                {
                    new
                    {
                        Json = "{{ " +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Edm.Int32\", " +
                            "\"value\": null" +
                            "{1}{0}" +
                        "}}",
                    },
                    new
                    {
                        Json = "{{ " +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Edm.Int32\", " +
                            "{0}{1}" +
                            "\"" + JsonLightConstants.ODataValuePropertyName + "\": 42" +
                        "}}",
                    },
                    new
                    {
                        Json = "{{ " +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Edm.Int32\", " +
                            "\"" + JsonLightConstants.ODataValuePropertyName + "\": 42" +
                            "{1}{0}" +
                        "}}",
                    },
                };

            EdmModel model = new EdmModel();
            var container = new EdmEntityContainer("TestModel", "DefaultContainer");
            model.AddElement(container);

            var owningType = new EdmEntityType("TestModel", "OwningType");
            owningType.AddKeys(owningType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false)));
            owningType.AddStructuralProperty("TopLevelProperty", EdmCoreModel.Instance.GetInt32(true));
            owningType.AddStructuralProperty("TopLevelSpatialProperty", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, false));
            owningType.AddStructuralProperty("TopLevelCollectionProperty", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(false)));
            model.AddElement(owningType);

            IEdmEntityType edmOwningType = (IEdmEntityType)model.FindDeclaredType("TestModel.OwningType");
            IEdmStructuralProperty edmTopLevelProperty = (IEdmStructuralProperty)edmOwningType.FindProperty("TopLevelProperty");
            IEdmStructuralProperty edmTopLevelSpatialProperty = (IEdmStructuralProperty)edmOwningType.FindProperty("TopLevelSpatialProperty");
            IEdmStructuralProperty edmTopLevelCollectionProperty = (IEdmStructuralProperty)edmOwningType.FindProperty("TopLevelCollectionProperty");

            PayloadReaderTestDescriptor.ReaderMetadata readerMetadata = new PayloadReaderTestDescriptor.ReaderMetadata(edmTopLevelProperty);
            PayloadReaderTestDescriptor.ReaderMetadata spatialReaderMetadata = new PayloadReaderTestDescriptor.ReaderMetadata(edmTopLevelSpatialProperty);
            PayloadReaderTestDescriptor.ReaderMetadata collectionReaderMetadata = new PayloadReaderTestDescriptor.ReaderMetadata(edmTopLevelCollectionProperty);

            var testDescriptors = payloads.SelectMany(payload => injectedProperties.Select(injectedProperty =>
            {
                return new NativeInputReaderTestDescriptor()
                {
                    PayloadKind = ODataPayloadKind.Property,
                    InputCreator = (tc) =>
                    {
                        string input = string.Format(payload.Json, injectedProperty.InjectedJSON, string.IsNullOrEmpty(injectedProperty.InjectedJSON) ? string.Empty : ",");
                        return input;
                    },
                    PayloadEdmModel = model,

                    // We use payload expected result just as a convenient way to run the reader for the Property payload kind.
                    // We validate whether the reading succeeds or fails, but not the actual read values (at least not here).
                    ExpectedResultCallback = (tc) =>
                        new PayloadReaderTestExpectedResult(this.Settings.ExpectedResultSettings)
                        {
                            ReaderMetadata = readerMetadata,
                            ExpectedException = injectedProperty.ExpectedException,
                        }
                };
            }));

            var explicitPayloads = new[]
                {
                    new
                    {
                        Description = "Custom property annotation - should be ignored.",
                        Json = "{ " +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Edm.Int32\", " +
                            "\"" + JsonLightUtils.GetPropertyAnnotationName("value", "custom.annotation") + "\": null," +
                            "\"" + JsonLightConstants.ODataValuePropertyName + "\": 42" +
                        "}",
                        ReaderMetadata = readerMetadata,
                        ExpectedException = (ExpectedException)null
                    },
                    new
                    {
                        Description = "Duplicate custom property annotation - should not fail.",
                        Json = "{ " +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Edm.Int32\", " +
                            "\"" + JsonLightUtils.GetPropertyAnnotationName("value", "custom.annotation") + "\": null," +
                            "\"" + JsonLightUtils.GetPropertyAnnotationName("value", "custom.annotation") + "\": 42," +
                            "\"" + JsonLightConstants.ODataValuePropertyName + "\": 42" +
                        "}",
                        ReaderMetadata = readerMetadata,
                        ExpectedException = (ExpectedException)null
                    },
                    new
                    {
                        Description = "Unrecognized odata property annotation - should be ignored.",
                        Json = "{ " +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Edm.Int32\", " +
                            "\"" + JsonLightUtils.GetPropertyAnnotationName("value", JsonLightConstants.ODataAnnotationNamespacePrefix + "unknown") + "\": null," +
                            "\"" + JsonLightConstants.ODataValuePropertyName + "\": 42" +
                        "}",
                        ReaderMetadata = readerMetadata,
                        ExpectedException = (ExpectedException)null
                    },
                    new
                    {
                        Description = "Custom property annotation after the property - should fail.",
                        Json = "{ " +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Edm.Int32\", " +
                            "\"" + JsonLightConstants.ODataValuePropertyName + "\": 42," +
                            "\"" + JsonLightUtils.GetPropertyAnnotationName("value", "custom.annotation") + "\": null" +
                        "}",
                        ReaderMetadata = readerMetadata,
                        ExpectedException = ODataExpectedExceptions.ODataException("PropertyAnnotationAfterTheProperty", "custom.annotation", "value")
                    },
                    new
                    {
                        Description = "OData property annotation.",
                        Json = "{ " +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Edm.Int32\", " +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\": \"Edm.Int32\"," +
                            "\"" + JsonLightConstants.ODataValuePropertyName + "\": 42" +
                        "}",
                        ReaderMetadata = readerMetadata,
                        ExpectedException = (ExpectedException)null
                    },
                    new
                    {
                        Description = "Duplicate odata.type property should fail.",
                        Json = "{ " +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Edm.Int32\", " +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\": \"Edm.Int32\"," +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\": \"Edm.String\"," +
                            "\"" + JsonLightConstants.ODataValuePropertyName + "\": 42" +
                        "}",
                        ReaderMetadata = readerMetadata,
                        ExpectedException = ODataExpectedExceptions.ODataException("DuplicateAnnotationNotAllowed", JsonLightConstants.ODataTypeAnnotationName)
                    },
                    new
                    {
                        Description = "Type information for top-level property - correct.",
                        Json = "{ " +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Edm.Int32\", " +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\": \"Edm.Int32\"," +
                            "\"" + JsonLightConstants.ODataValuePropertyName + "\": 42" +
                        "}",
                        ReaderMetadata = readerMetadata,
                        ExpectedException = (ExpectedException)null
                    },
                    new
                    {
                        Description = "Type information for top-level property - different kind - should fail.",
                        Json = "{ " +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Edm.Int32\", " +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\": \"Collection(Edm.Int32)\"," +
                            "\"" + JsonLightConstants.ODataValuePropertyName + "\": 42" +
                        "}",
                        ReaderMetadata = readerMetadata,
                        ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_IncorrectTypeKind", "Collection(Edm.Int32)", "Primitive", "Collection")
                    },
                    new
                    {
                        Description = "Unknown type information for top-level null - should fail.",
                        Json = "{ " +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Edm.Int32\", " +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\": \"Unknown\"," +
                            "\"" + JsonLightConstants.ODataValuePropertyName + "\": null" +
                        "}",
                        ReaderMetadata = readerMetadata,
                        ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_IncorrectTypeKind", "Unknown", "Primitive", "Complex")
                    },
                    new
                    {
                        Description = "Type information for top-level spatial - should work.",
                        Json = "{ " +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Edm.GeographyPoint\", " +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\": \"Edm.GeographyPoint\"," +
                            "\"" + JsonLightConstants.ODataValuePropertyName + "\":" + SpatialUtils.GetSpatialStringValue(ODataFormat.Json, GeographyFactory.Point(33.1, -110.0).Build()) +
                        "}",
                        ReaderMetadata = spatialReaderMetadata,
                        ExpectedException = (ExpectedException)null
                    },
                };

            testDescriptors = testDescriptors.Concat(explicitPayloads.Select(payload =>
            {
                return new NativeInputReaderTestDescriptor()
                {
                    PayloadKind = ODataPayloadKind.Property,
                    InputCreator = (tc) =>
                    {
                        return payload.Json;
                    },
                    PayloadEdmModel = model,

                    // We use payload expected result just as a convenient way to run the reader for the Property payload kind
                    // since the reading should always fail, we don't need anything to compare the results to.
                    ExpectedResultCallback = (tc) =>
                        new PayloadReaderTestExpectedResult(this.Settings.ExpectedResultSettings)
                        {
                            ReaderMetadata = payload.ReaderMetadata,
                            ExpectedException = payload.ExpectedException,
                        },
                    DebugDescription = payload.Description
                };
            }));

            // Add a test descriptor to verify that we ignore odata.context in requests
            testDescriptors = testDescriptors.Concat(
                new NativeInputReaderTestDescriptor[]
                {
                    new NativeInputReaderTestDescriptor()
                    {
                        DebugDescription = "Top-level property with invalid name.",
                        PayloadKind = ODataPayloadKind.Property,
                        InputCreator = tc =>
                        {
                            return "{ " +
                                "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Edm.Int32\", " +
                                "\"TopLevelProperty\": 42" +
                            "}";
                        },
                        PayloadEdmModel = model,
                        ExpectedResultCallback = tc =>
                            new PayloadReaderTestExpectedResult(this.Settings.ExpectedResultSettings)
                            {
                                ReaderMetadata = readerMetadata,
                                ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightPropertyAndValueDeserializer_InvalidTopLevelPropertyName", "TopLevelProperty", "value")
                            },
                        SkipTestConfiguration = tc => !tc.IsRequest
                    }
                });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Verifies correct reading of top-level open properties.")]
        public void OpenTopLevelPropertiesTest()
        {
            IEdmModel model = TestModels.BuildTestModel();

            var testCases = new OpenPropertyTestCase[]
            {   
                new OpenPropertyTestCase
                {
                    DebugDescription = "Integer open property.",
                    ExpectedProperty = PayloadBuilder.PrimitiveProperty("OpenProperty", 42),
                    ExpectedPropertyWhenTypeUnavailable = PayloadBuilder.PrimitiveProperty("OpenProperty", 42),
                    ExpectedPropertyType = EdmCoreModel.Instance.GetInt32(false),
                    JsonTypeInformation = "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"Edm.Int32\",",
                    Json = "{0}{1}\"" + JsonLightConstants.ODataValuePropertyName + "\":42",
                },
                new OpenPropertyTestCase
                {
                    DebugDescription = "Null open property.",
                    ExpectedProperty = PayloadBuilder.PrimitiveProperty("OpenProperty", null),
                    ExpectedPropertyWhenTypeUnavailable = PayloadBuilder.PrimitiveProperty("OpenProperty", null),
                    ExpectedPropertyType = EdmCoreModel.Instance.GetString(true),
                    JsonTypeInformation = string.Empty,
                    Json = "{0}{1}\"value\":null",
                },
                new OpenPropertyTestCase
                {
                    DebugDescription = "String open property.",
                    ExpectedProperty = PayloadBuilder.PrimitiveProperty("OpenProperty", "value"),
                    ExpectedPropertyWhenTypeUnavailable = PayloadBuilder.PrimitiveProperty("OpenProperty", "value"),
                    ExpectedPropertyType = EdmCoreModel.Instance.GetString(true),
                    JsonTypeInformation = "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"Edm.String\",",
                    Json = "{0}{1}\"" + JsonLightConstants.ODataValuePropertyName + "\":\"" + JsonLightConstants.ODataValuePropertyName + "\"",
                },
                new OpenPropertyTestCase
                {
                    DebugDescription = "DateTimeOffset open property with type information.",
                    ExpectedProperty = PayloadBuilder.PrimitiveProperty("OpenProperty", new DateTimeOffset(2012, 4, 13, 2, 43, 10, 215, TimeSpan.Zero)),
                    ExpectedPropertyWhenTypeUnavailable = PayloadBuilder.PrimitiveProperty("OpenProperty", "2012-04-13T02:43:10.215Z"),
                    ExpectedPropertyType = EdmCoreModel.Instance.GetDateTimeOffset(false),
                    JsonTypeInformation = "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"Edm.DateTimeOffset\",",
                    Json = "{0}{1}\"" + JsonLightConstants.ODataValuePropertyName + "\":\"2012-04-13T02:43:10.215Z\"",
                },
            };

            bool[] withExpectedTypes = new bool[] { true, false };
            bool[] withPayloadTypes = new bool[] { true, false };
            bool[] includeContextUri = new bool[] { true, false };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                withExpectedTypes,
                withPayloadTypes,
                includeContextUri,
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations,
                (testCase, withExpectedType, withPayloadType, withContextUri, testConfiguration) =>
                {
                    if (withContextUri && testConfiguration.IsRequest)
                    {
                        return;
                    }

                    string expectedTypeName = testCase.ExpectedPropertyType is IEdmCollectionTypeReference ? "Collection(" + ((IEdmCollectionType)testCase.ExpectedPropertyType.Definition).ElementType.FullName() + ")" : testCase.ExpectedPropertyType.TestFullName();
                    string contextUri = withContextUri ? "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + string.Format("\":\"http://odata.org/test/$metadata#{0}\",", expectedTypeName) : string.Empty;
                    string json = string.Format(
                        CultureInfo.InvariantCulture,
                        testCase.Json,
                        contextUri,
                        withPayloadType ? testCase.JsonTypeInformation : string.Empty);

                    bool typeGiven = withExpectedType || withPayloadType || withContextUri;

                    if (!typeGiven && testCase.ExpectedPropertyWhenTypeUnavailable == null)
                    {
                        testCase.ExpectedException = ODataExpectedExceptions.ODataException("ReaderValidationUtils_ValueWithoutType");
                    }

                    PropertyInstance property = typeGiven || testCase.ExpectedPropertyWhenTypeUnavailable == null ? testCase.ExpectedProperty : testCase.ExpectedPropertyWhenTypeUnavailable;
                    property = property.DeepCopy();

                    if (withExpectedType)
                    {
                        property = property.ExpectedPropertyType(testCase.ExpectedPropertyType);
                    }

                    if (!withPayloadType)
                    {
                        ComplexProperty complexProperty = property as ComplexProperty;
                        if (complexProperty != null)
                        {
                            complexProperty.Value.AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null });
                        }
                        else
                        {
                            PrimitiveMultiValueProperty primitiveCollectionProperty = property as PrimitiveMultiValueProperty;
                            if (primitiveCollectionProperty != null)
                            {
                                primitiveCollectionProperty.Value.AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null });
                            }
                            else
                            {
                                ComplexMultiValueProperty complexCollectionProperty = property as ComplexMultiValueProperty;
                                if (complexCollectionProperty != null)
                                {
                                    complexCollectionProperty.Value.AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null });
                                }
                            }
                        }
                    }

                    ExpectedException expectedException = testCase.ExpectedException;
                    if (!withContextUri && !testConfiguration.IsRequest)
                    {
                        expectedException = ODataExpectedExceptions.ODataException("ODataJsonLightDeserializer_ContextLinkNotFoundAsFirstProperty");
                    }

                    PayloadReaderTestDescriptor testDescriptor = new PayloadReaderTestDescriptor(this.Settings)
                    {
                        DebugDescription = testCase.DebugDescription + "[Expected type: " + withExpectedType + ", payload type: " + withPayloadType + "]",
                        PayloadElement = property
                            .JsonRepresentation("{" + json + "}"),
                        PayloadEdmModel = model,
                        ExpectedException = expectedException,
                    };

                    // These descriptors are already tailored specifically for Json Light and 
                    // do not require normalization.
                    testDescriptor.TestDescriptorNormalizers.Clear();
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Verifies correct reading of complex top-level open properties.")]
        public void OpenComplexTopLevelPropertiesTest()
        {
            IEdmModel model = TestModels.BuildTestModel();

            var addressType = model.FindDeclaredType("TestModel.Address");

            var testCases = new OpenPropertyTestCase[]
            {
                new OpenPropertyTestCase
                {
                    DebugDescription = "Empty complex open property with type information.",
                    ExpectedProperty = PayloadBuilder.Property("OpenProperty", PayloadBuilder.ComplexValue("TestModel.Address")),
                    ExpectedPropertyType = addressType.ToTypeReference(),
                    JsonTypeInformation = "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.Address\"",
                    Json = "{0}{1}",
                },
                new OpenPropertyTestCase
                {
                    DebugDescription = "Complex property with data.",
                    ExpectedProperty = PayloadBuilder.Property("OpenProperty", PayloadBuilder.ComplexValue("TestModel.Address").PrimitiveProperty("Street", "First")),
                    ExpectedPropertyType = addressType.ToTypeReference(),
                    JsonTypeInformation = "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.Address\",",
                    Json = "{0}{1}\"Street\":\"First\"",
                },

                new OpenPropertyTestCase
                {
                    DebugDescription = "Complex property with data.",
                    ExpectedProperty = PayloadBuilder.Property("OpenProperty", PayloadBuilder.ComplexValue("TestModel.Address").PrimitiveProperty("Street", "First")),
                    ExpectedPropertyType = addressType.ToTypeReference(),
                    JsonTypeInformation = "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.Address\",",
                    Json = "{0}{1}\"Street\":\"First\"",
                },
            };

            bool[] withExpectedTypes = new bool[] { true, false};
            bool[] withPayloadTypes = new bool[] { true, false };
            bool[] includeContextUri = new bool[] { true, false };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                withExpectedTypes,
                withPayloadTypes,
                includeContextUri,
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations,
                (testCase, withExpectedType, withPayloadType, withContextUri, testConfiguration) =>
                {
                    if (withContextUri && testConfiguration.IsRequest)
                    {
                        return;
                    }

                    PropertyInstance property = testCase.ExpectedProperty.DeepCopy();
                    ComplexProperty complexProperty = (ComplexProperty)property;
                    ComplexInstance complexValue = complexProperty.Value;
                    bool isEmpty = !complexValue.Properties.Any();

                    string contextUri = withContextUri ? "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#TestModel.Address\"," : string.Empty;
                    string json = string.Format(
                        CultureInfo.InvariantCulture,
                        testCase.Json,
                        contextUri,
                        withPayloadType ? testCase.JsonTypeInformation : string.Empty);

                    if (isEmpty && json.EndsWith(","))
                    {
                        json = json.Substring(0, json.Length - 1);
                    }

                    if (withExpectedType)
                    {
                        property = property.ExpectedPropertyType(testCase.ExpectedPropertyType);
                    }

                    if (!withPayloadType)
                    {
                        complexProperty.Value.AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null });
                    }

                    ExpectedException expectedException = testCase.ExpectedException;
                    if (!withContextUri && !testConfiguration.IsRequest)
                    {
                        expectedException = ODataExpectedExceptions.ODataException("ODataJsonLightDeserializer_ContextLinkNotFoundAsFirstProperty");
                    }
                    else if (!withExpectedType && !withPayloadType && !withContextUri)
                    {
                        string firstPropertyName = isEmpty ? null : complexValue.Properties.First().Name;

                        // An open property without expected and payload type cannot be read; expect an exception.
                        expectedException = ODataExpectedExceptions.ODataException("ReaderValidationUtils_ResourceWithoutType");
                    }

                    PayloadReaderTestDescriptor testDescriptor = new PayloadReaderTestDescriptor(this.Settings)
                    {
                        DebugDescription = testCase.DebugDescription + "[Expected type: " + withExpectedType + ", payload type: " + withPayloadType + "]",
                        PayloadElement = property
                            .JsonRepresentation("{" + json + "}"),
                        PayloadEdmModel = model,
                        ExpectedException = expectedException,
                    };

                    // These descriptors are already tailored specifically for Json Light and 
                    // do not require normalization.
                    testDescriptor.TestDescriptorNormalizers.Clear();

                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Verifies correct error behavior for top-level open properties.")]
        public void OpenTopLevelPropertiesErrorTest()
        {
            IEdmModel model = TestModels.BuildTestModel();

            var testCases = new OpenPropertyTestCase[]
            {
                new OpenPropertyTestCase
                {
                    DebugDescription = "String open property with property annotation type information - should fail.",
                    ExpectedProperty = PayloadBuilder.PrimitiveProperty("OpenProperty", "value").ExpectedPropertyType(EdmCoreModel.Instance.GetString(true)),
                    Json = 
                        "{0}" +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName(JsonLightConstants.ODataValuePropertyName, JsonLightConstants.ODataTypeAnnotationName) + "\":\"Edm.String\"," +
                        "\"" + JsonLightConstants.ODataValuePropertyName + "\":\"" + JsonLightConstants.ODataValuePropertyName + "\"",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightPropertyAndValueDeserializer_UnexpectedODataPropertyAnnotation", JsonLightConstants.ODataTypeAnnotationName)
                },
                new OpenPropertyTestCase
                {
                    DebugDescription = "String open property with complex type information - should fail.",
                    ExpectedProperty = PayloadBuilder.PrimitiveProperty("OpenProperty", "value").ExpectedPropertyType(EdmCoreModel.Instance.GetString(true)),
                    Json = 
                        "{0}" +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.Address\"," +
                        "\"" + JsonLightConstants.ODataValuePropertyName + "\":\"" + JsonLightConstants.ODataValuePropertyName + "\"",
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_IncorrectTypeKind", "TestModel.Address", "Primitive", "Complex")
                },
                new OpenPropertyTestCase
                {
                    DebugDescription = "Type property after the data property - should fail.",
                    ExpectedProperty = PayloadBuilder.PrimitiveProperty("OpenProperty", "value").ExpectedPropertyType(EdmCoreModel.Instance.GetString(true)),
                    Json = 
                        "{0}" +
                        "\"" + JsonLightConstants.ODataValuePropertyName + "\":\"" + JsonLightConstants.ODataValuePropertyName + "\"," +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"Edm.String\"" ,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightPropertyAndValueDeserializer_TypePropertyAfterValueProperty", JsonLightConstants.ODataTypeAnnotationName, JsonLightConstants.ODataValuePropertyName)
                },
                new OpenPropertyTestCase
                {
                    DebugDescription = "Duplicate data property - should fail.",
                    ExpectedProperty = PayloadBuilder.PrimitiveProperty("OpenProperty", "value").ExpectedPropertyType(EdmCoreModel.Instance.GetString(true)),
                    Json = 
                        "{0}" +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"Edm.String\"," +
                        "\"" + JsonLightConstants.ODataValuePropertyName + "\":\"" + JsonLightConstants.ODataValuePropertyName + "\"," +
                        "\"" + JsonLightConstants.ODataValuePropertyName + "\":\"" + JsonLightConstants.ODataValuePropertyName + "\"",
                    ExpectedException = ODataExpectedExceptions.ODataException("DuplicatePropertyNamesNotAllowed", JsonLightConstants.ODataValuePropertyName)
                },
                new OpenPropertyTestCase
                {
                    DebugDescription = "Duplicate odata.type property - should fail.",
                    ExpectedProperty = PayloadBuilder.PrimitiveProperty("OpenProperty", "value").ExpectedPropertyType(EdmCoreModel.Instance.GetString(true)),
                    Json = 
                        "{0}" +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"Edm.String\"," +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"Edm.String\"," +
                        "\"" + JsonLightConstants.ODataValuePropertyName + "\":\"" + JsonLightConstants.ODataValuePropertyName + "\"",
                    ExpectedException = ODataExpectedExceptions.ODataException("DuplicateAnnotationNotAllowed", JsonLightConstants.ODataTypeAnnotationName)
                },
                new OpenPropertyTestCase
                {
                    DebugDescription = "No data property - should fail.",
                    ExpectedProperty = PayloadBuilder.PrimitiveProperty("OpenProperty", "value").ExpectedPropertyType(EdmCoreModel.Instance.GetString(true)),
                    Json = 
                        "{0}" +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"Edm.String\"",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightPropertyAndValueDeserializer_InvalidTopLevelPropertyPayload")
                },
                new OpenPropertyTestCase
                {
                    DebugDescription = "No data property (only type property) - should fail.",
                    ExpectedProperty = PayloadBuilder.PrimitiveProperty("OpenProperty", "value").ExpectedPropertyType(EdmCoreModel.Instance.GetString(true)),
                    Json = 
                        "{0}" +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"Edm.String\"",
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightPropertyAndValueDeserializer_InvalidTopLevelPropertyPayload")
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations,
                (testCase, testConfiguration) =>
                {
                    string json = string.Format(
                        CultureInfo.InvariantCulture,
                        testCase.Json,
                        testConfiguration.IsRequest
                            ? string.Empty
                            : "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Edm.String\",");

                    PayloadReaderTestDescriptor testDescriptor = new PayloadReaderTestDescriptor(this.Settings)
                    {
                        DebugDescription = testCase.DebugDescription,
                        PayloadElement = testCase.ExpectedProperty
                            .JsonRepresentation("{" + json + "}"),
                        PayloadEdmModel = model,
                        ExpectedException = testCase.ExpectedException,
                    };

                    // These descriptors are already tailored specifically for Json Light and 
                    // do not require normalization.
                    testDescriptor.TestDescriptorNormalizers.Clear();
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Verifies correct error behavior for top-level spatial property payloads")]
        public void SpatialPropertyErrorTest()
        {
            EdmModel model = new EdmModel();
            var container = new EdmEntityContainer("TestModel", "DefaultContainer");
            model.AddElement(container);

            var owningType = new EdmEntityType("TestModel", "OwningType");
            owningType.AddKeys(owningType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false)));
            owningType.AddStructuralProperty("TopLevelProperty", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, true));
            model.AddElement(owningType);

            IEdmEntityType edmOwningType = (IEdmEntityType)model.FindDeclaredType("TestModel.OwningType");
            IEdmStructuralProperty edmTopLevelProperty = (IEdmStructuralProperty)edmOwningType.FindProperty("TopLevelProperty");

            PayloadReaderTestDescriptor.ReaderMetadata readerMetadata = new PayloadReaderTestDescriptor.ReaderMetadata(edmTopLevelProperty);
            string pointValue = SpatialUtils.GetSpatialStringValue(ODataFormat.Json, GeographyFactory.Point(33.1, -110.0).Build(), "Edm.GeographyPoint");

            var explicitPayloads = new[]
                {
                    new
                    {
                        Description = "Spatial value with type information and odata.type annotation - should fail.",
                        Json = "{ " +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Edm.GeographyPoint\", " +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"Edm.GeographyPoint\"," +
                            "\"" + JsonLightConstants.ODataValuePropertyName + "\":" + pointValue +
                        "}",
                        ReaderMetadata = readerMetadata,
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightPropertyAndValueDeserializer_ODataTypeAnnotationInPrimitiveValue", JsonLightConstants.ODataTypeAnnotationName)
                    },
                    new
                    {
                        Description = "Spatial value with odata.type annotation - should fail.",
                        Json = "{ " +
                            "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"http://odata.org/test/$metadata#Edm.GeographyPoint\", " +
                            "\"" + JsonLightConstants.ODataValuePropertyName + "\":" + pointValue +
                        "}",
                        ReaderMetadata = readerMetadata,
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightPropertyAndValueDeserializer_ODataTypeAnnotationInPrimitiveValue", JsonLightConstants.ODataTypeAnnotationName)
                    },
                };

            var testDescriptors = explicitPayloads.Select(payload =>
            {
                return new NativeInputReaderTestDescriptor()
                {
                    DebugDescription = payload.Description,
                    PayloadKind = ODataPayloadKind.Property,
                    InputCreator = (tc) =>
                    {
                        return payload.Json;
                    },
                    PayloadEdmModel = model,

                    // We use payload expected result just as a convenient way to run the reader for the Property payload kind
                    // since the reading should always fail, we don't need anything to compare the results to.
                    ExpectedResultCallback = (tc) =>
                        new PayloadReaderTestExpectedResult(this.Settings.ExpectedResultSettings)
                        {
                            ReaderMetadata = payload.ReaderMetadata,
                            ExpectedException = payload.ExpectedException,
                        },
                };
            });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        private sealed class OpenPropertyTestCase
        {
            public string DebugDescription { get; set; }
            public PropertyInstance ExpectedProperty { get; set; }
            public IEdmTypeReference ExpectedPropertyType { get; set; }
            public string JsonTypeInformation { get; set; }
            public string Json { get; set; }
            public ExpectedException ExpectedException { get; set; }
            public PropertyInstance ExpectedPropertyWhenTypeUnavailable { get; set; }
        }
    }
}

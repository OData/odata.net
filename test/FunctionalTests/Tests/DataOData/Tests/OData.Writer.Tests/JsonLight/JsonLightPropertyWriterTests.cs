//---------------------------------------------------------------------
// <copyright file="JsonLightPropertyWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.JsonLight
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Spatial;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Json.TextAnnotations;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for writing property payloads in JSON Lite format.
    /// </summary>
    [TestClass, TestCase]
    public class JsonLightPropertyWriterTests : ODataWriterTestCase
    {
        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestDescriptor.Settings Settings { get; set; }

        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestExpectedResults.Settings ExpectedResultSettings { get; set; }

        [TestMethod, Variation(Description = "Test correct serialization format when writing top-level open properties in JSON Lite.")]
        public void TopLevelOpenPropertiesTest()
        {
            EdmModel edmModel = new EdmModel();
            var container = new EdmEntityContainer("TestModel", "DefaultContainer");
            edmModel.AddElement(container);

            var openCustomerType = new EdmEntityType("TestModel", "OpenCustomerType", null, isAbstract: false, isOpen: true);
            openCustomerType.AddKeys(openCustomerType.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32, isNullable: false));
            edmModel.AddElement(openCustomerType);

            var addressType = new EdmComplexType("TestModel", "AddressType");
            addressType.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String, isNullable: true);
            edmModel.AddElement(addressType);

            container.AddEntitySet("CustomerSet", openCustomerType);

            ISpatial pointValue = GeographyFactory.Point(32.0, -100.0).Build();

            IEnumerable<PropertyPayloadTestCase> testCases = new[]
            {
                new PropertyPayloadTestCase
                {
                    DebugDescription = "Top-level open primitive property.",
                    Property = new ODataProperty { Name = "Age", Value = (long)42 },
                    Model = edmModel,
                    PropertyType = "Edm.Int64",
                    Json = string.Join("$(NL)",
                        "{{",
                        "{0},\"value\":\"42\"",
                        "}}")
                },
                new PropertyPayloadTestCase
                {
                    DebugDescription = "Top-level open spatial property.",
                    Property = new ODataProperty { Name = "Location", Value = pointValue },
                    Model = edmModel,
                    PropertyType = "Edm.GeographyPoint",
                    Json = string.Join("$(NL)",
                        "{{",
                        "{0}," +
                        "\"" + JsonLightConstants.ODataValuePropertyName + "\":{{",
                        "\"type\":\"Point\",\"coordinates\":[",
                        "-100.0,32.0",
                        "],\"crs\":{{",
                        "\"type\":\"name\",\"properties\":{{",
                        "\"name\":\"EPSG:4326\"",
                        "}}",
                        "}}",
                        "}}",
                        "}}")
                }
            };

            IEnumerable<PayloadWriterTestDescriptor<ODataProperty>> testDescriptors = testCases.Select(testCase =>
                new PayloadWriterTestDescriptor<ODataProperty>(
                    this.Settings,
                    testCase.Property,
                    tc => new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        Json = string.Format(
                            CultureInfo.InvariantCulture,
                            testCase.Json,
                            JsonLightWriterUtils.GetMetadataUrlPropertyForProperty(testCase.PropertyType)),
                        FragmentExtractor = (result) => result.RemoveAllAnnotations(true)
                    })
                {
                    DebugDescription = testCase.DebugDescription,
                    Model = testCase.Model,
                });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.JsonLightFormatConfigurationsWithIndent,
                (testDescriptor, testConfiguration) =>
                {
                    TestWriterUtils.WriteAndVerifyTopLevelContent(
                        testDescriptor,
                        testConfiguration,
                        (messageWriter) => messageWriter.WriteProperty(testDescriptor.PayloadItems.Single()),
                        this.Assert,
                        baselineLogger: this.Logger);
                });
        }

        [TestMethod, Variation(Description = "Test correct serialization format when writing top-level spatial properties in JSON Lite.")]
        public void TopLevelSpatialPropertiesTest()
        {
            EdmModel edmModel = new EdmModel();
            var container = new EdmEntityContainer("TestModel", "DefaultContainer");
            edmModel.AddElement(container);

            var customerType = new EdmEntityType("TestModel", "CustomerType");
            customerType.AddKeys(customerType.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32, isNullable: false));
            customerType.AddStructuralProperty("Location1", EdmPrimitiveTypeKind.Geography, isNullable: true);
            customerType.AddStructuralProperty("Location2", EdmPrimitiveTypeKind.GeographyPoint, isNullable: true);
            edmModel.AddElement(customerType);
            container.AddEntitySet("CustomerSet", customerType);

            ISpatial pointValue = GeographyFactory.Point(32.0, -100.0).Build();

            IEnumerable<PropertyPayloadTestCase> testCases = new[]
            {
                new PropertyPayloadTestCase
                {
                    DebugDescription = "Top-level open property with non-matching expected and payload type.",
                    Property = new ODataProperty { Name = "Location1", Value = pointValue },
                    Model = edmModel,
                    PropertyType = "Edm.GeographyPoint",
                    Json = string.Join("$(NL)",
                        "{{",
                        "{0}" +
                        "\"" + JsonLightConstants.ODataValuePropertyName + "\":{{",
                        "\"type\":\"Point\",\"coordinates\":[",
                        "-100.0,32.0",
                        "],\"crs\":{{",
                        "\"type\":\"name\",\"properties\":{{",
                        "\"name\":\"EPSG:4326\"",
                        "}}",
                        "}}",
                        "}}",
                        "}}")
                },
                new PropertyPayloadTestCase
                {
                    DebugDescription = "Top-level spatial property with matching expected and payload type.",
                    Property = new ODataProperty { Name = "Location2", Value = pointValue },
                    Model = edmModel,
                    PropertyType = "Edm.GeographyPoint",
                    Json = string.Join("$(NL)",
                        "{{",
                        "{0}" +
                        "\"" + JsonLightConstants.ODataValuePropertyName + "\":{{",
                        "\"type\":\"Point\",\"coordinates\":[",
                        "-100.0,32.0",
                        "],\"crs\":{{",
                        "\"type\":\"name\",\"properties\":{{",
                        "\"name\":\"EPSG:4326\"",
                        "}}",
                        "}}",
                        "}}",
                        "}}")
                },
            };

            IEnumerable<PayloadWriterTestDescriptor<ODataProperty>> testDescriptors = testCases.Select(testCase =>
                new PayloadWriterTestDescriptor<ODataProperty>(
                    this.Settings,
                    testCase.Property,
                    tc => new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        Json = string.Format(
                            CultureInfo.InvariantCulture,
                            testCase.Json,
                            JsonLightWriterUtils.GetMetadataUrlPropertyForProperty(testCase.PropertyType) + ","),
                        FragmentExtractor = (result) => result.RemoveAllAnnotations(true)
                    })
                {
                    DebugDescription = testCase.DebugDescription,
                    Model = testCase.Model,
                });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.JsonLightFormatConfigurationsWithIndent,
                (testDescriptor, testConfiguration) =>
                {
                    TestWriterUtils.WriteAndVerifyTopLevelContent(
                        testDescriptor,
                        testConfiguration,
                        (messageWriter) => messageWriter.WriteProperty(testDescriptor.PayloadItems.Single()),
                        this.Assert,
                        baselineLogger: this.Logger);
                });
        }

        [TestMethod, Variation(Description = "Cases for writing properties in JSON Lite with top level float value.")]
        public void WriteFloatingPointValue()
        {
            var edmModel = new EdmModel();

            IEnumerable<PropertyPayloadTestCase> testCases = new[]
            {
                new PropertyPayloadTestCase
                {
                    DebugDescription = "Top-level property float point type.",
                    Property = new ODataProperty { Name = "Speed", Value = float.PositiveInfinity },
                    Model = edmModel,
                    PropertyType = "Edm.Double",
                },
                new PropertyPayloadTestCase
                {
                    DebugDescription = "Top-level property float point type.",
                    Property = new ODataProperty { Name = "Speed", Value = float.NegativeInfinity },
                    Model = edmModel,
                    PropertyType = "Edm.Double",
                },
                new PropertyPayloadTestCase
                {
                    DebugDescription = "Top-level property float point type.",
                    Property = new ODataProperty { Name = "Speed", Value = double.PositiveInfinity },
                    Model = edmModel,
                    PropertyType = "Edm.Double",
                },
                new PropertyPayloadTestCase
                {
                    DebugDescription = "Top-level property float point type.",
                    Property = new ODataProperty { Name = "Speed", Value = double.NegativeInfinity },
                    Model = edmModel,
                    PropertyType = "Edm.Double",
                },
            };

            IEnumerable<PayloadWriterTestDescriptor<ODataProperty>> testDescriptors = testCases.Select(testCase =>
                new PayloadWriterTestDescriptor<ODataProperty>(
                    this.Settings,
                    testCase.Property,
                    tc => new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        FragmentExtractor = (result) => result.RemoveAllAnnotations(true)
                    })
                {
                    DebugDescription = testCase.DebugDescription,
                    Model = testCase.Model,
                });


            this.CombinatorialEngineProvider.RunCombinations(
               testDescriptors,
               this.WriterTestConfigurationProvider.JsonLightFormatConfigurationsWithIndent,
               (testDescriptor, testConfiguration) =>
               {
                   TestWriterUtils.WriteAndVerifyTopLevelContent(
                       testDescriptor,
                       testConfiguration,
                       (messageWriter) => messageWriter.WriteProperty(testDescriptor.PayloadItems.Single()),
                       this.Assert,
                       baselineLogger: this.Logger);
               });
        }

        [TestMethod, Variation(Description = "Cases for writing properties in JSON Lite with untyped value.")]
        public void WriteUntypedValueTest()
        {
            EdmModel edmModel = new EdmModel();

            var jsonType = new EdmComplexType("TestModel", "JsonType");
            var jsonTypeRef = new EdmComplexTypeReference(jsonType, isNullable: true);
            edmModel.AddElement(jsonType);

            var collectionType = new EdmCollectionType(jsonTypeRef);
            var collectionTypeRef = new EdmCollectionTypeReference(collectionType);

            var entityType = new EdmEntityType("TestModel", "EntityType");
            entityType.AddStructuralProperty("Value", jsonTypeRef);
            entityType.AddStructuralProperty("CollectionValue", collectionTypeRef);
            edmModel.AddElement(entityType);

            var container = new EdmEntityContainer("TestModel", "DefaultContainer");
            container.AddEntitySet("EntitySet", entityType);
            edmModel.AddElement(container);

            const string JsonFormat = "$(NL){{{0},\"Value\":{1}}}";

            IEnumerable<PropertyPayloadTestCase> testCases = new[]
            {
                new PropertyPayloadTestCase
                {
                    DebugDescription = "Null.",
                    Property = new ODataProperty
                                {
                                    Name = "Value",
                                    Value = new ODataUntypedValue() { RawValue = "null" }
                                },
                },
                new PropertyPayloadTestCase
                {
                    DebugDescription = "Integer.",
                    Property = new ODataProperty
                                {
                                    Name = "Value",
                                    Value = new ODataUntypedValue() { RawValue = "42" }
                                },
                },
                new PropertyPayloadTestCase
                {
                    DebugDescription = "Float.",
                    Property = new ODataProperty
                                {
                                    Name = "Value",
                                    Value = new ODataUntypedValue() { RawValue = "3.1415" }
                                },
                },
                new PropertyPayloadTestCase
                {
                    DebugDescription = "String.",
                    Property = new ODataProperty
                                {
                                    Name = "Value",
                                    Value = new ODataUntypedValue() { RawValue = "\"string\"" }
                                },
                },
                new PropertyPayloadTestCase
                {
                    DebugDescription = "Array of elements of mixed types.",
                    Property = new ODataProperty
                                {
                                    Name = "Value",
                                    Value = new ODataUntypedValue() { RawValue = "[1, 2, \"abc\"]" }
                                },
                },
                new PropertyPayloadTestCase
                {
                    DebugDescription = "Array of arrays.",
                    Property = new ODataProperty
                                {
                                    Name = "Value",
                                    Value = new ODataUntypedValue() { RawValue = "[ [1, \"abc\"], [2, \"def\"], [[3],[4, 5]] ]" }
                                },
                },
                new PropertyPayloadTestCase
                {
                    DebugDescription = "Negative - empty RawValue",
                    Property = new ODataProperty
                                {
                                    Name = "Value",
                                    Value = new ODataUntypedValue() { RawValue = string.Empty },
                                },
                    ExpectedException = ODataExpectedExceptions.ODataException(
                                            TextRes.ODataJsonLightValueSerializer_MissingRawValueOnUntyped),
                },
                new PropertyPayloadTestCase
                {
                    DebugDescription = "Collection of Edm.Untyped elements.",
                    Property = new ODataProperty
                                {
                                    Name = "CollectionValue",
                                    Value = new ODataCollectionValue()
                                            {
                                                TypeName = "Collection(TestModel.JsonType)",
                                                Items = new object[]
                                                        {
                                                            new ODataUntypedValue()
                                                            {
                                                                RawValue = "\"string\""
                                                            },
                                                            new ODataUntypedValue()
                                                            {
                                                                RawValue = "[1, 2, \"abc\"]"
                                                            },
                                                            new ODataUntypedValue()
                                                            {
                                                                RawValue = "3.1415"
                                                            }
                                                        }
                                            }
                                },
                },
                new PropertyPayloadTestCase
                {
                    DebugDescription = "Integer.",
                    Property = new ODataProperty
                                {
                                    Name = "Value",
                                    Value = new ODataPrimitiveValue(42)
                                },
                },
                new PropertyPayloadTestCase
                {
                    DebugDescription = "Float.",
                    Property = new ODataProperty
                                {
                                    Name = "Value",
                                    Value = new ODataPrimitiveValue(3.1415)
                                },
                },
                new PropertyPayloadTestCase
                {
                    DebugDescription = "String.",
                    Property = new ODataProperty
                                {
                                    Name = "Value",
                                    Value = new ODataPrimitiveValue("string")
                                },
                },
            };

            IEnumerable<PayloadWriterTestDescriptor<ODataProperty>> testDescriptors = testCases.Select(testCase =>
                new PayloadWriterTestDescriptor<ODataProperty>(
                    this.Settings,
                    testCase.Property,
                    tc => new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        Json = string.Format(
                            CultureInfo.InvariantCulture,
                            JsonFormat,
                            JsonLightWriterUtils.GetMetadataUrlPropertyForEntry("EntitySet"),
                            GetExpectedJson(testCase.Property.Value)),
                        FragmentExtractor = (result) => result.RemoveAllAnnotations(false),
                        ExpectedException2 = testCase.ExpectedException,
                    })
                {
                    DebugDescription = testCase.DebugDescription,
                    Model = edmModel,
                });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.JsonLightFormatConfigurationsWithIndent,
                (testDescriptor, testConfiguration) =>
                {
                    TestWriterUtils.WriteAndVerifyTopLevelContent(
                        testDescriptor,
                        testConfiguration,
                        (messageWriter) =>
                        {
                            messageWriter.PrivateSettings.Validations = ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType;
                            messageWriter.WriteProperty(testDescriptor.PayloadItems.Single());
                        },
                        this.Assert,
                        baselineLogger: this.Logger);
                });
        }

        /// <summary>
        /// Gets the expected JSON string from a given <see cref="T:ODataUntypedValue"/> or a
        /// <see cref="T:ODataCollectionValue"/> whose elements are <see cref="T:ODataUntypedValue"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The expected JSON string.</returns>
        private static string GetExpectedJson(object value)
        {
            ODataUntypedValue untypedValue = value as ODataUntypedValue;
            if (untypedValue != null)
            {
                return untypedValue.RawValue;
            }

            ODataCollectionValue collectionValue = value as ODataCollectionValue;
            if (collectionValue != null)
            {
                return
                    "[" +
                        string.Join<string>(
                            ",",
                            collectionValue.Items.OfType<ODataUntypedValue>().Select(uv => uv.RawValue)) +
                     "]";
            }

            return string.Empty;
        }

        private sealed class PropertyPayloadTestCase
        {
            public string DebugDescription { get; set; }
            public ODataProperty Property { get; set; }
            public string Json { get; set; }
            public IEdmModel Model { get; set; }
            public string PropertyType { get; set; }
            public ExpectedException ExpectedException { get; set; }
        }
    }
}

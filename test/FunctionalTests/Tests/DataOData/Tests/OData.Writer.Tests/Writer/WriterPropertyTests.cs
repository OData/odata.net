//---------------------------------------------------------------------
// <copyright file="WriterPropertyTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Writer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Xml.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Spatial;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.OData.Utils.ODataLibTest;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Json;
    using Microsoft.Test.Taupo.OData.Json.TextAnnotations;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Json;
    using Microsoft.Test.Taupo.OData.Writer.Tests.JsonLight;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/883
    /// <summary>
    /// Tests for writing properties with the OData writer.
    /// </summary>
    [TestClass]
    public class WriterPropertyTests : ODataWriterTestCase
    {
        private static readonly Uri ServiceDocumentUri = new Uri("http://odata.org/");
        private static readonly ODataResourceSerializationInfo MySerializationInfo = new ODataResourceSerializationInfo()
        {
            NavigationSourceEntityTypeName = "TestModel.EntityType",
            NavigationSourceName = "MySet",
            ExpectedTypeName = "TestModel.EntityType"
        };

        private const string MissingTypeNameSentinelTextAtom = "<missingTypeName/>";
        private const string MissingTypeNameSentinelTextJson = "\"missingTypeName\":null";

        private static readonly XElement MissingTypeNameSentinelXElement = new XElement("missingTypeName");
        private static readonly JsonValue MissingTypeNameSentinelJsonProperty = new JsonProperty("missingTypeName", new JsonPrimitiveValue(null));

        /// <summary>The set of characters that are invalid in property names.</summary>
        private static readonly char[] InvalidCharactersInPropertyNames = new char[] { ':', '.', '@' };

        /// <summary>Set of invalid property names used in tests.</summary>
        private static readonly string[] InvalidPropertyNames = new string[] { "@", "Me@Work", "Me:Work", "Me.Work", "MeWork@", "MeWork:", "MeWork.", };

        [InjectDependency(IsRequired = true)]
        public IPayloadGenerator PayloadGenerator { get; set; }

        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestExpectedResults.Settings ExpectedResultSettings { get; set; }

        [InjectDependency(IsRequired = true)]
        public IModelGenerator ModelGenerator { get; set; }

        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestDescriptor.Settings Settings { get; set; }

        // This test only runs on async which is not supported on Phone and Silverlight
#if !SILVERLIGHT && !WINDOWS_PHONE
        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Test single property JSON payloads from payload Generator.")]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void TaupoSinglePropertyTests_Json()
        {
            // TODO: Fix places where we've lost JsonVerbose coverage to add JsonLight
            IEnumerable<EntityInstance> generator = PayloadGenerator.GenerateJsonPayloads();
            this.TestSinglePropertyGeneration(ODataFormat.Json, generator);
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Test complex property payloads with metadata")]
        public void TaupoComplexPropertyMetadataErrorTests()
        {
            var metadata = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildDefaultAstoriaTestModel();

            IEnumerable<EntityInstance> payloads = PayloadGenerator.GenerateAtomPayloads();

            this.CombinatorialEngineProvider.RunCombinations(
                this.GenerateComplexPropertyErrorTestCases(payloads),
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent.Where(c => !c.Synchronous),
                (testCase, testConfiguration) =>
                {
                    testCase.Model = metadata;

                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyTopLevelContent(testCase, testConfiguration,
                        (messageWriter) =>
                        {
                            var visitor = new ODataPayloadElementPropertyWriter();
                            visitor.WriteProperty(messageWriter.MessageWriter, testCase.PayloadItems.Single());
                        },
                        this.Assert,
                        baselineLogger: this.Logger);
                });
        }
#endif

        #region Primitive properties
        private IEnumerable<PayloadWriterTestDescriptor<ODataProperty>> CreatePrimitiveTopLevelPropertyDescriptors()
        {
            var model = new EdmModel();
            ODataProperty[] properties = ObjectModelUtils.CreateDefaultPrimitiveProperties(model).Where(p => p.Value != null).ToArray();
            model.Fixup();
            var owningType = MetadataUtils.EntityTypes(model).Single(et => et.Name == "EntryWithPrimitiveProperties");

            string[] propertiesJsonLightResults = new string[]
            {
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":1.0",
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":\"AAEAAQ==\"",
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":1",
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":true",
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":1",
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":\"2010-10-10T10:10:10Z\"",
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":\"2010-10-10T10:10:10+01:00\"",
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":\"2010-10-10T10:10:10-08:00\"",
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":\"1\"",
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":\"11111111-2222-3333-4444-555555555555\"",
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":1",
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":1",
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":1",
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":\"1\"",
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":\"1\"",
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":\"PT12M20.4S\"",
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":" + SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeographyValue),
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":" + SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeographyPointValue),
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":" + SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeographyLineStringValue),
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":" + SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeographyPolygonValue),
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":" + SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeographyCollectionValue),
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":" + SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeographyMultiPointValue),
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":" + SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeographyMultiLineStringValue),
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":" + SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeographyMultiPolygonValue),
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":" + SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeometryValue),
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":" + SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeometryPointValue),
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":" + SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeometryLineStringValue),
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":" + SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeometryPolygonValue),
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":" + SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeometryCollectionValue),
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":" + SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeometryMultiPointValue),
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":" + SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeometryMultiLineStringValue),
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":" + SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeometryMultiPolygonValue),

                "\"" + JsonLightConstants.ODataValuePropertyName + "\":1.0",
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":1",
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":true",
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":1",
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":\"2010-10-10T10:10:10Z\"",
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":\"2010-10-10T10:10:10+01:00\"",
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":\"2010-10-10T10:10:10-08:00\"",
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":\"1\"",
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":\"11111111-2222-3333-4444-555555555555\"",
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":1",
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":1",
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":1",
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":\"1\"",
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":\"1\"",
                "\"" + JsonLightConstants.ODataValuePropertyName + "\":\"PT12M20.4S\"",
            };

            string[][] propertiesJsonLightRequestResultLines = new string[propertiesJsonLightResults.Length][];
            string[][] propertiesJsonLightResponseResultLines = new string[propertiesJsonLightResults.Length][];
            for (int i = 0; i < propertiesJsonLightResults.Length; ++i)
            {
                string propertyJsonLightResult = propertiesJsonLightResults[i];

                var property = owningType.FindProperty(properties[i].Name);
                var typeName = property.Type.TestFullName();
                if (properties[i].Value is ISpatial)
                {
                    typeName = "Edm." + properties[i].Value.GetType().BaseType.Name;
                }

                string contextUri = JsonLightConstants.DefaultMetadataDocumentUri.AbsoluteUri + "#" + typeName;
                string propertyJsonLightWithMetadataAnnotationResult = "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"" + contextUri + "\"," + propertyJsonLightResult;

                propertiesJsonLightRequestResultLines[i] = JsonUtils.WrapTopLevelObject(JsonUtils.GetJsonLines(propertyJsonLightWithMetadataAnnotationResult));
                propertiesJsonLightResponseResultLines[i] = JsonUtils.WrapTopLevelObject(JsonUtils.GetJsonLines(propertyJsonLightWithMetadataAnnotationResult));
            }

            for (int i = 0; i < properties.Length; ++i)
            {
                ODataProperty property = properties[i];
                string[] jsonLightRequestResult = propertiesJsonLightRequestResultLines[i];
                string[] jsonLightResponseResult = propertiesJsonLightResponseResultLines[i];

                yield return new PayloadWriterTestDescriptor<ODataProperty>(
                    this.Settings,
                    property,
                    (testConfiguration) =>
                    {
                        if (testConfiguration.Format == ODataFormat.Json)
                        {
                            return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                            {
                                Json = JsonLightWriterUtils.CombineLines(testConfiguration.IsRequest ? jsonLightRequestResult : jsonLightResponseResult),
                                FragmentExtractor = null,
                            };
                        }
                        else
                        {
                            string formatName = testConfiguration.Format == null ? "null" : testConfiguration.Format.GetType().Name;
                            throw new NotSupportedException("Format " + formatName + " + is not supported.");
                        }
                    })
                {
                    Model = model,
                    PayloadEdmElementContainer = owningType
                };
            }
        }
        #endregion Primitive properties

        #region Complex properties
        private IEnumerable<PayloadWriterTestDescriptor<ODataItem[]>> CreateComplexPropertyDescriptors()
        {
            EdmModel model = new EdmModel();
            ODataItem[][] properties = ObjectModelUtils.CreateDefaultComplexProperties(model);
            model.Fixup();

            var owningType = MetadataUtils.EntityTypes(model).Single(et => et.Name == "EntryWithComplexProperties");

            Func<bool, string[][]> propertiesJsonLightResultsFunc = isRequest => new string[][]
            {
                StringUtils.Flatten(
                    "{",
                    "$(Indent)" +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"" + JsonLightConstants.DefaultMetadataDocumentUri + "#My.AddressType\"," +
                        "\"Street\":\"One Redmond Way\",\"City\":\" Redmond\"",
                    "}"
                ),
                StringUtils.Flatten(
                    "{",
                    "$(Indent)" +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"" + JsonLightConstants.DefaultMetadataDocumentUri + "#My.NestedAddressType\"," +
                        "\"Street\":{",
                    "$(Indent)$(Indent)\"StreetName\":\"One Redmond Way\",\"Number\":1234",
                    "$(Indent)},\"City\":\"Redmond \"",
                    "}"
                )
            };
            string[][] propertiesJsonLightRequestResults = propertiesJsonLightResultsFunc(true);
            string[][] propertiesJsonLightResponseResults = propertiesJsonLightResultsFunc(false);

            for (int i = 0; i < properties.Length; ++i)
            {
                ODataItem[] property = properties[i];
                string[] jsonLightRequestResult = propertiesJsonLightRequestResults[i];
                string[] jsonLightResponseResult = propertiesJsonLightResponseResults[i];

                yield return new PayloadWriterTestDescriptor<ODataItem[]>(
                    this.Settings,
                    property,
                    (testConfiguration) =>
                    {
                        if (testConfiguration.Format == ODataFormat.Json)
                        {
                            return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                            {
                                Json = JsonLightWriterUtils.CombineLines(testConfiguration.IsRequest ? jsonLightRequestResult : jsonLightResponseResult),
                                FragmentExtractor = null
                            };
                        }
                        else
                        {
                            string formatName = testConfiguration.Format == null ? "null" : testConfiguration.Format.GetType().Name;
                            throw new NotSupportedException("Format " + formatName + " + is not supported.");
                        }
                    })
                {
                    Model = model,
                    PayloadEdmElementContainer = owningType
                };
            }
        }
        #endregion Complex properties

        #region Collection properties
        private IEnumerable<PayloadWriterTestDescriptor<ODataProperty>> CreateCollectionPropertyDescriptors()
        {
            EdmModel model = new EdmModel();
            var properties = ObjectModelUtils.CreateDefaultCollectionProperties(model);
            model.Fixup();

            var owningType = MetadataUtils.EntityTypes(model).Single(et => et.Name == "EntryWithCollectionProperties");

            Func<bool, string[][]> propertiesJsonLightResultsFunc = isRequest => new string[][]
            {
                StringUtils.Flatten(
                    "{",
                    "$(Indent)" +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"" + JsonLightConstants.DefaultMetadataDocumentUri + "#Collection(Edm.String)\"," +
                        "\"" + JsonLightConstants.ODataValuePropertyName + "\":[",
                    "$(Indent)$(Indent)",
                    "$(Indent)]",
                    "}"
                ),
                StringUtils.Flatten(
                    "{",
                    "$(Indent)" +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"" + JsonLightConstants.DefaultMetadataDocumentUri + "#Collection(Edm.Int32)\"," +
                        "\"" + JsonLightConstants.ODataValuePropertyName + "\":[",
                    "$(Indent)$(Indent)0,1,2,3,4,5,6,7,8,9",
                    "$(Indent)]",
                    "}"
                ),
                StringUtils.Flatten(
                    "{",
                    "$(Indent)" +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"" + JsonLightConstants.DefaultMetadataDocumentUri + "#Collection(Edm.Int32)\"," +
                        "\"" + JsonLightConstants.ODataValuePropertyName + "\":[",
                    "$(Indent)$(Indent)0,1,2",
                    "$(Indent)]",
                    "}"
                ),
                StringUtils.Flatten(
                    "{",
                    "$(Indent)" +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"" + JsonLightConstants.DefaultMetadataDocumentUri + "#Collection(Edm.String)\"," +
                        "\"" + JsonLightConstants.ODataValuePropertyName + "\":[",
                    "$(Indent)$(Indent)\"One\",\"Two\",\"Three\"",
                    "$(Indent)]",
                    "}"
                ),
                StringUtils.Flatten(
                    "{",
                    "$(Indent)" +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"" + JsonLightConstants.DefaultMetadataDocumentUri + "#Collection(Edm.Geography)\"," +
                        "\"" + JsonLightConstants.ODataValuePropertyName + "\":[",
                    JsonUtils.GetJsonLines(
                        "$(Indent)$(Indent)" +
                        SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeographyCollectionValue) + "," +
                        SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeographyLineStringValue) + "," +
                        SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeographyMultiLineStringValue) + "," +
                        SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeographyMultiPointValue) + "," +
                        SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeographyMultiPolygonValue) + "," +
                        SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeographyPointValue) + "," +
                        SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeographyPolygonValue) + "," +
                        SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeographyValue), indentDepth: 2),
                    "$(Indent)]",
                    "}"
                ),
                new string[0],
                new string[0],
            };

            string[][] propertiesJsonLightRequestResults = propertiesJsonLightResultsFunc(true);
            string[][] propertiesJsonLightResponseResults = propertiesJsonLightResultsFunc(false);

            for (int i = 0; i < properties.Length; ++i)
            {
                ODataProperty property = properties[i];
                string[] jsonLightRequestResultLines = propertiesJsonLightRequestResults[i];
                string[] jsonLightResponseResultLines = propertiesJsonLightResponseResults[i];

                yield return new PayloadWriterTestDescriptor<ODataProperty>(
                    this.Settings,
                    property,
                    (testConfiguration) =>
                    {
                        if (testConfiguration.Format == ODataFormat.Json)
                        {
                            ODataCollectionValue collectionValue = ((ODataCollectionValue)property.Value);
                            if (string.IsNullOrEmpty(collectionValue.TypeName) && (collectionValue.TypeAnnotation == null || string.IsNullOrEmpty(collectionValue.TypeAnnotation.TypeName)))
                            {
                                return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                                {
                                    ExpectedException2 = ODataExpectedExceptions.ODataException("WriterValidationUtils_MissingTypeNameWithMetadata")
                                };
                            }

                            return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                            {
                                Json = JsonLightWriterUtils.CombineLines(testConfiguration.IsRequest ? jsonLightRequestResultLines : jsonLightResponseResultLines),
                                FragmentExtractor = testConfiguration.IsRequest
                                    ? (Func<JsonValue, JsonValue>)null
                                    : (result) => result
                            };
                        }
                        else
                        {
                            string formatName = testConfiguration.Format == null ? "null" : testConfiguration.Format.GetType().Name;
                            throw new NotSupportedException("Format " + formatName + " + is not supported.");
                        }
                    })
                {
                    Model = model,
                    PayloadEdmElementContainer = owningType
                };
            }
        }

        private PayloadWriterTestDescriptor<ODataItem[]> CreateComplexCollectionPropertyDescriptors()
        {
            EdmModel model = new EdmModel();
            var property = CreateDefaultComplexCollectionProperties(model);
            model.Fixup();

            var owningType = MetadataUtils.EntityTypes(model).Single(et => et.Name == "EntryWithCollectionProperties");

            Func<bool, string[]> propertiesJsonLightResultsFunc = isRequest => new string[]
            {
                "{",
                "$(Indent)" +
                    "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataContextAnnotationName + "\":\"" + JsonLightConstants.DefaultMetadataDocumentUri + "#Collection(My.AddressType)\"," +
                    "\"" + JsonLightConstants.ODataValuePropertyName + "\":[",
                "$(Indent)$(Indent){",
                "$(Indent)$(Indent)$(Indent)\"Street\":\"One Redmond Way\",\"City\":\" Redmond\"",
                "$(Indent)$(Indent)},{",
                "$(Indent)$(Indent)$(Indent)\"Street\":\"Am Euro Platz 3\",\"City\":\"Vienna \"",
                "$(Indent)$(Indent)}",
                "$(Indent)]",
                "}"
            };

            string[] jsonLightRequestResultLines = propertiesJsonLightResultsFunc(true);
            string[] jsonLightResponseResultLines = propertiesJsonLightResultsFunc(false);

            return new PayloadWriterTestDescriptor<ODataItem[]>(
                this.Settings,
                property,
                (testConfiguration) =>
                {
                    if (testConfiguration.Format == ODataFormat.Json)
                    {
                        return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                        {
                            Json = JsonLightWriterUtils.CombineLines(testConfiguration.IsRequest ? jsonLightRequestResultLines : jsonLightResponseResultLines),
                            FragmentExtractor = testConfiguration.IsRequest
                                ? (Func<JsonValue, JsonValue>)null
                                : (result) => result
                        };
                    }
                    else
                    {
                        string formatName = testConfiguration.Format == null ? "null" : testConfiguration.Format.GetType().Name;
                        throw new NotSupportedException("Format " + formatName + " + is not supported.");
                    }
                })
            {
                Model = model,
                PayloadEdmElementContainer = owningType
            };
        }

        public static ODataItem[] CreateDefaultComplexCollectionProperties(EdmModel model = null)
        {
            if (model != null)
            {
                var addressType = model.ComplexType("AddressType", "My")
                    .Property("Street", EdmPrimitiveTypeKind.String)
                    .Property("City", EdmPrimitiveTypeKind.String);

                model.EntityType("EntryWithCollectionProperties", "TestModel")
                .Property("ComplexCollection", new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(addressType, true))));
            }

            return new ODataItem[]
            {
                new ODataResourceSet(){TypeName = EntityModelUtils.GetCollectionTypeName("My.AddressType")},
                new ODataResource()
                {
                    TypeName = "My.AddressType",
                    Properties = new []
                    {
                        new ODataProperty() { Name = "Street", Value = "One Redmond Way" },
                        new ODataProperty() { Name = "City", Value = " Redmond" },
                    }
                },
                new ODataResource()
                {
                    TypeName = null,
                    Properties = new []
                    {
                        new ODataProperty() { Name = "Street", Value = "Am Euro Platz 3" },
                        new ODataProperty() { Name = "City", Value = "Vienna " },
                    }
                }
            };
        }
        #endregion Collection properties

        [TestMethod, Variation(Description = "Test property writing.")]
        public void PropertyTests()
        {
            var testDescriptors = this.CreatePrimitiveTopLevelPropertyDescriptors()
                .Concat(this.CreateCollectionPropertyDescriptors());

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent,
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);
                    testDescriptor.RunTopLevelPropertyPayload(testConfiguration, baselineLogger: this.Logger);
                });
        }

        [TestMethod, Variation(Description = "Test property writing.")]
        public void ComplexPropertyTests()
        {
            var testDescriptors = this.CreateComplexPropertyDescriptors();

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent,
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);
                    testDescriptor.RunTopLevelPropertyPayload(testConfiguration, baselineLogger: this.Logger,
                        writeAction:

                        (messageWriter) =>
                        {
                            var resourceWriter = messageWriter.CreateODataResourceWriter();
                            foreach (var item in testDescriptor.PayloadItems.Single())
                            {
                                var resource = item as ODataResource;
                                if (resource != null)
                                {
                                    resourceWriter.WriteStart(resource);
                                }
                                else
                                {
                                    var nestedInfo = item as ODataNestedResourceInfo;
                                    if (nestedInfo != null)
                                    {
                                        resourceWriter.WriteStart(nestedInfo);
                                    }
                                }
                            }

                            foreach (var item in testDescriptor.PayloadItems.Single())
                            {
                                resourceWriter.WriteEnd();
                            }
                        });
                });
        }

        [TestMethod, Variation(Description = "Test property writing.")]
        public void ComplexCollectionPropertyTests()
        {
            var testDescriptors = new[] { this.CreateComplexCollectionPropertyDescriptors() };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent,
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);
                    testDescriptor.RunTopLevelPropertyPayload(testConfiguration, baselineLogger: this.Logger,
                        writeAction:

                        (messageWriter) =>
                        {
                            var resourceWriter = messageWriter.CreateODataResourceSetWriter();
                            foreach (var item in testDescriptor.PayloadItems.Single())
                            {
                                var resource = item as ODataResource;
                                if (resource != null)
                                {
                                    resourceWriter.WriteStart(resource);
                                    resourceWriter.WriteEnd();
                                }
                                else
                                {
                                    var set = item as ODataResourceSet;
                                    if (set != null)
                                    {
                                        resourceWriter.WriteStart(set);
                                    }
                                }
                            }

                            foreach (var item in testDescriptor.PayloadItems.Single().OfType<ODataResourceSet>())
                            {
                                resourceWriter.WriteEnd();
                            }
                        });
                });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Test in-stream error cases when writing a property.")]
        public void PropertyInStreamErrorTests()
        {
            EdmModel model = new EdmModel();
            ODataProperty property = ObjectModelUtils.CreateDefaultPrimitiveProperties(model)[0];
            var container = new EdmEntityContainer("TestModel", "DefaultContainer");
            model.AddElement(container);

            var entityType = model.FindType("TestModel.EntryWithPrimitiveProperties") as EdmEntityType;

            this.Assert.AreEqual("Null", property.Name, "Expected null property to be the first primitive property.");

            PayloadWriterTestDescriptor<ODataProperty>[] testDescriptors = new PayloadWriterTestDescriptor<ODataProperty>[]
                {
                    new PayloadWriterTestDescriptor<ODataProperty>(this.Settings, property, (string)null, (string)null)
                    {
                        Model = model,
                        PayloadEdmElementContainer = entityType
                    },
                };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                TestWriterUtils.InvalidSettingSelectors,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent,
                (testDescriptor, selector, testConfiguration) =>
                {
                    TestWriterUtils.WriteWithStreamErrors(
                        testDescriptor,
                        selector,
                        testConfiguration,
                        (messageWriter) => messageWriter.WriteProperty(testDescriptor.PayloadItems.Single()),
                        this.Assert);
                });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Test error case when writing a property with an invalid Xml character in its value.")]
        public void InvalidXmlCharactersTests()
        {
            ODataProperty property = ObjectModelUtils.CreateDefaultPrimitiveProperties().First(p => p.Name == "String");
            this.Assert.AreEqual("String", property.Name, "Expected string property to be the primitive property at position 16.");

            string invalidXmlString = "Invalid character: '" + (char)1 + "'";

            string atomResult = @"<{5} xmlns=""{1}"" xmlns:{0}=""{1}"">Invalid character: '&#x1;'</{5}>";
            atomResult = string.Format(atomResult,
                TestAtomConstants.ODataMetadataNamespacePrefix,
                TestAtomConstants.ODataMetadataNamespace,
                TestAtomConstants.ODataNamespacePrefix,
                TestAtomConstants.ODataNamespace,
                TestAtomConstants.AtomTypeAttributeName,
                TestAtomConstants.ODataValueElementName);

            property.Value = invalidXmlString;
            PayloadWriterTestDescriptor.WriterTestExpectedResultCallback expectedResultCallback =
                (testConfig) =>
                {
                    if (testConfig.MessageWriterSettings.EnableCharactersCheck)
                    {
                        return new WriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                        {
                            // TODO: Support raw error message verification for non-product exceptions
                            ExpectedException2 = new ExpectedException(typeof(ArgumentException)),
                        };
                    }
                    else
                    {
                        return new AtomWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                        {
                            FragmentExtractor = null,
                            Xml = atomResult
                        };
                    }
                };

            PayloadWriterTestDescriptor<ODataProperty>[] testDescriptors = new PayloadWriterTestDescriptor<ODataProperty>[]
                {
                    new PayloadWriterTestDescriptor<ODataProperty>(this.Settings, property, expectedResultCallback),
                };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.AtomFormatConfigurationsWithIndent,
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);
                    testDescriptor.RunTopLevelPropertyPayload(testConfiguration, baselineLogger: this.Logger);

                    WriterTestConfiguration otherTestConfiguration = testConfiguration.Clone();
                    otherTestConfiguration.MessageWriterSettings.EnableCharactersCheck = !testConfiguration.MessageWriterSettings.EnableCharactersCheck;
                    testDescriptor.RunTopLevelPropertyPayload(otherTestConfiguration, baselineLogger: this.Logger);
                });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Makes sure xml:space=\"preserve\" is written for string values with leading and/or trailing spaces.")]
        public void PreserveSpaceTests()
        {
            ODataProperty property = ObjectModelUtils.CreateDefaultPrimitiveProperties().First(p => p.Name == "String");
            this.Assert.AreEqual("String", property.Name, "Expected string property to be the primitive property at position 16.");

            string[] whiteSpaces = new string[]
            {
                " ",
                "\t",
                "\n",
                "\r",
                "\r\n",
            };
            string[] stringValues = whiteSpaces.SelectMany(s => new[] { string.Format("{0}foo", s), string.Format("foo{0}", s), string.Format("{0}foo{1}", s, s) }).ToArray();

            this.CombinatorialEngineProvider.SetBaselineCallback(() =>
            {
                // In this case the newline is the check point, but the diff reporter don't support this
                string payload = this.Logger.GetBaseline();
                payload = payload.Replace(Environment.NewLine, "@@Environment.NewLine@@");
                payload = payload.Replace("\r\n", "\\r\\n");
                payload = payload.Replace("\n", "\\n");
                payload = payload.Replace("\r", "\\r");
                payload = payload.Replace("@@Environment.NewLine@@", Environment.NewLine);

                return payload;
            });

            this.CombinatorialEngineProvider.RunCombinations(
                stringValues,
                this.WriterTestConfigurationProvider.AtomFormatConfigurationsWithIndent,
                (stringValue, testConfiguration) =>
                {
                    string atomResult = @"<{5} xmlns:{0}=""{1}"" xml:space=""preserve"" xmlns=""{1}"">{4}</{5}>";
                    atomResult = string.Format(atomResult,
                        TestAtomConstants.ODataMetadataNamespacePrefix,
                        TestAtomConstants.ODataMetadataNamespace,
                        TestAtomConstants.ODataNamespacePrefix,
                        TestAtomConstants.ODataNamespace,
                        stringValue.Replace("\r", "&#xD;"),
                        TestAtomConstants.ODataValueElementName);

                    property.Value = stringValue;
                    PayloadWriterTestDescriptor.WriterTestExpectedResultCallback expectedResultCallback =
                        (testConfig) =>
                        {
                            return new AtomWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                            {
                                FragmentExtractor = null,
                                Xml = atomResult
                            };
                        };

                    var testDescriptor = new PayloadWriterTestDescriptor<ODataProperty>(this.Settings, property, expectedResultCallback);
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);
                    testDescriptor.RunTopLevelPropertyPayload(testConfiguration, baselineLogger: this.Logger);
                });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Verifies correct failures if invalid property names are specified.")]
        public void InvalidPropertyNameTest()
        {
            EdmModel model = new EdmModel();
            EdmEntityType emptyEntityType = new EdmEntityType("TestModel", "EmptyEntityType");
            model.AddElement(emptyEntityType);

            var container = new EdmEntityContainer("TestModel", "DefaultContainer");
            model.AddElement(container);

            container.AddEntitySet("EmptyEntityType", emptyEntityType);

            var testDescriptors = InvalidPropertyNames.Select(propertyName =>
                new PayloadWriterTestDescriptor<ODataProperty>(
                    this.Settings,
                    new ODataProperty { Name = propertyName, Value = null },
                    (tc) => new WriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        ExpectedException2 = ODataExpectedExceptions.ODataException("ValidationUtils_PropertiesMustNotContainReservedChars", propertyName, "':', '.', '@'")
                    }
                )
                {
                    Model = model,
                    PayloadEdmElementType = emptyEntityType
                });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    testDescriptor.RunTopLevelPropertyPayload(testConfiguration, baselineLogger: this.Logger);
                });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Verifies correct failures if invalid property values are specified.")]
        public void InvalidPropertyValueTest()
        {
            EdmModel model = new EdmModel();
            EdmEntityType emptyEntityType = new EdmEntityType("TestModel", "EntityTypeWithProperty");
            emptyEntityType.AddStructuralProperty("PrimitiveProperty", EdmCoreModel.Instance.GetString(isNullable: false));
            emptyEntityType.AddStructuralProperty("CollectionProperty", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(isNullable: false)));
            model.AddElement(emptyEntityType);

            var container = new EdmEntityContainer("TestModel", "DefaultContainer");
            model.AddElement(container);

            container.AddEntitySet("EmptyEntityType", emptyEntityType);

            var testDescriptors = new[]
            {
                // Note: Bogus value for a property value covered by ODataPropertyTests in the TDD test suite.
                // Bogus value for a collection item
                new PayloadWriterTestDescriptor<ODataProperty>(
                    this.Settings,
                    new ODataProperty { Name = "CollectionProperty", Value = new ODataCollectionValue { TypeName = EntityModelUtils.GetCollectionTypeName("Edm.String"), Items = new object[] { new ODataMessageWriterSettings() } } },
                    (tc) => new WriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        ExpectedException2 =  ODataExpectedExceptions.ODataException("ValidationUtils_UnsupportedPrimitiveType", "Microsoft.OData.ODataMessageWriterSettings"),
                    }
                ) { Model = model, PayloadEdmElementContainer = container }
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    testDescriptor.RunTopLevelPropertyPayload(testConfiguration, baselineLogger: this.Logger);
                });
        }

        private sealed class NonNullableEdmPrimitiveTypeWithValue
        {
            public NonNullableEdmPrimitiveTypeWithValue(EdmPrimitiveTypeKind type, object value, string atom, string jsonLight)
                : this(type, value, atom, jsonLight, null)
            {
            }

            public NonNullableEdmPrimitiveTypeWithValue(EdmPrimitiveTypeKind type, object value, string atom, string jsonLight, ODataVersion? version)
            {
                this.Version = version;
                this.Type = type;
                this.Value = value;
                this.AtomRepresentation = atom;
                this.JsonLightRepresentation = jsonLight;
            }

            public EdmPrimitiveTypeKind Type { get; private set; }
            public object Value { get; private set; }
            public ODataVersion? Version { get; set; }
            public string AtomRepresentation { get; private set; }
            public string JsonLightRepresentation { get; private set; }
        }

        private static NonNullableEdmPrimitiveTypeWithValue[] NonNullableEdmPrimitiveTypesWithValues = new NonNullableEdmPrimitiveTypeWithValue[]
            {
                new NonNullableEdmPrimitiveTypeWithValue(EdmPrimitiveTypeKind.Binary, new byte[] { 0, 1, 0, 1}, "AAEAAQ==", "\"AAEAAQ==\""),
                new NonNullableEdmPrimitiveTypeWithValue(EdmPrimitiveTypeKind.Boolean, true, "true", "true"),
                new NonNullableEdmPrimitiveTypeWithValue(EdmPrimitiveTypeKind.Byte, (byte)1, "1", "1"),
                new NonNullableEdmPrimitiveTypeWithValue(EdmPrimitiveTypeKind.DateTimeOffset, DateTimeOffset.Parse("2010-10-10T10:10:10Z"), "2010-10-10T10:10:10Z", "\"2010-10-10T10:10:10Z\""),
                new NonNullableEdmPrimitiveTypeWithValue(EdmPrimitiveTypeKind.Decimal, (decimal)1, "1", "\"1\""),
                new NonNullableEdmPrimitiveTypeWithValue(EdmPrimitiveTypeKind.Double, (double)1, "1", "1.0"),
                new NonNullableEdmPrimitiveTypeWithValue(EdmPrimitiveTypeKind.Guid, new Guid("11111111-2222-3333-4444-555555555555"), "11111111-2222-3333-4444-555555555555", "\"11111111-2222-3333-4444-555555555555\""),
                new NonNullableEdmPrimitiveTypeWithValue(EdmPrimitiveTypeKind.Int16, (Int16)1, "1", "1"),
                new NonNullableEdmPrimitiveTypeWithValue(EdmPrimitiveTypeKind.Int32, (Int32)1, "1", "1"),
                new NonNullableEdmPrimitiveTypeWithValue(EdmPrimitiveTypeKind.Int64, (Int64)1, "1", "\"1\""),
                new NonNullableEdmPrimitiveTypeWithValue(EdmPrimitiveTypeKind.SByte, (sbyte)1, "1", "1"),
                new NonNullableEdmPrimitiveTypeWithValue(EdmPrimitiveTypeKind.Single, (Single)1, "1", "1"),
                new NonNullableEdmPrimitiveTypeWithValue(EdmPrimitiveTypeKind.String, "1", "1", "\"1\""),
                new NonNullableEdmPrimitiveTypeWithValue(EdmPrimitiveTypeKind.Duration, TimeSpan.FromMinutes(12.34), "PT12M20.4S", "\"PT12M20.4S\""),
            };

        // NOTE that we're only testing null property on a complex top-level property or on an entry, but the same behavior should apply to
        // all of these cases:
        //   primitive on a complex value
        //   primitive on an entry
        //   primitive on a complex value in collection
        //   complex on an entry
        //   complex on a complex value
        //   complex on a complex value in collection
        //   both ATOM and JSON
        //   primitive on complex or entry value in EPM
        // We're not testing these here, since all of it goes through the same code-path, the only difference is passing around the settings.
        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Verifies that null values for nullable/non-nullable types are correctly validated against metadata and correctly serialized.")]
        public void NullPropertyOnEntryTest()
        {
            var complexType = new EdmComplexType("TestModel", "NullValueComplexType");

            // Primitive properties
            var testDescriptors = NonNullableEdmPrimitiveTypesWithValues.SelectMany(primitiveTypeWithValue =>
                new bool[] { false, true }.Select(nullable => new Func<TestODataBehaviorKind, ODataVersion, PayloadWriterTestDescriptor<ODataItem>>(
                    (behaviorKind, version) =>
                    {
                        IEdmTypeReference dataType = EdmCoreModel.Instance.GetPrimitive(primitiveTypeWithValue.Type, nullable);
                        return this.CreateNullPropertyOnEntryTestDescriptor(
                            dataType,
                            version,
                            nullable);
                    })));

            // Complex property
            testDescriptors = testDescriptors.Concat(new bool[] { false, true }.Select(nullable =>
                new Func<TestODataBehaviorKind, ODataVersion, PayloadWriterTestDescriptor<ODataItem>>(
                    (behaviorKind, version) =>
                    {
                        IEdmTypeReference dataType = new EdmComplexTypeReference(complexType, nullable);
                        return this.CreateNullPropertyOnEntryTestDescriptor(
                            dataType,
                            version,
                            nullable);
                    })));

            this.CombinatorialEngineProvider.RunCombinations(
                TestWriterUtils.ODataBehaviorKinds,
                testDescriptors,
                this.WriterTestConfigurationProvider.AtomFormatConfigurationsWithIndent,
                (behaviorKind, testDescriptorCreator, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.CloneAndApplyBehavior(behaviorKind);
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);
                    TestWriterUtils.WriteAndVerifyODataPayload(
                        testDescriptorCreator(behaviorKind, testConfiguration.Version),
                        testConfiguration,
                        this.Assert,
                        this.Logger);
                });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Verifies that null values for nullable/non-nullable types are correctly validated against metadata and correctly serialized.")]
        public void NullPropertyOnComplexValueTest()
        {
            var complexType = new EdmComplexType("TestModel", "NullValueComplexType");

            // Primitive properties
            var testDescriptors = NonNullableEdmPrimitiveTypesWithValues.SelectMany(primitiveTypeWithValue =>
                new bool[] { false, true }.Select(nullable => new Func<TestODataBehaviorKind, ODataVersion, PayloadWriterTestDescriptor<ODataProperty>>(
                    (behaviorKind, version) =>
                    {
                        IEdmTypeReference dataType = EdmCoreModel.Instance.GetPrimitive(primitiveTypeWithValue.Type, nullable);
                        return this.CreateNullPropertyOnComplexValueTestDescriptor(
                            dataType,
                            version,
                            nullable);
                    })));

            // Complex property
            testDescriptors = testDescriptors.Concat(new bool[] { false, true }.Select(nullable =>
                new Func<TestODataBehaviorKind, ODataVersion, PayloadWriterTestDescriptor<ODataProperty>>(
                    (behaviorKind, version) =>
                    {
                        IEdmTypeReference dataType = new EdmComplexTypeReference(complexType, nullable);
                        return this.CreateNullPropertyOnComplexValueTestDescriptor(
                            dataType,
                            version,
                            nullable);
                    })));

            this.CombinatorialEngineProvider.RunCombinations(
                TestWriterUtils.ODataBehaviorKinds,
                testDescriptors,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent,
                (behaviorKind, testDescriptorCreator, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.CloneAndApplyBehavior(behaviorKind);
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);
                    PayloadWriterTestDescriptor<ODataProperty> testDescriptor = testDescriptorCreator(behaviorKind, testConfiguration.Version);
                    testDescriptor.RunTopLevelPropertyPayload(testConfiguration, baselineLogger: this.Logger);
                });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Verifies that non-null values for non-nullable properties on complex values are correctly validated against metadata and correctly serialized.")]
        public void NonNullablePropertyOnComplexValueTest()
        {
            // Primitive properties
            var testDescriptors = NonNullableEdmPrimitiveTypesWithValues.Select(nonNullablePrimitiveTypeWithValue =>
                new Func<TestODataBehaviorKind, ODataVersion, PayloadWriterTestDescriptor<ODataProperty>>(
                    (behaviorKind, version) =>
                    {
                        if (!nonNullablePrimitiveTypeWithValue.Version.HasValue || nonNullablePrimitiveTypeWithValue.Version == version)
                        {
                            return this.CreateNonNullPropertyOnComplexValueTestDescriptor(
                                nonNullablePrimitiveTypeWithValue,
                                nonNullablePrimitiveTypeWithValue.Type != EdmPrimitiveTypeKind.String,
                                version,
                                "nonNullProperty");
                        }

                        return null;
                    }));

            this.CombinatorialEngineProvider.RunCombinations(
                TestWriterUtils.ODataBehaviorKinds,
                testDescriptors,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent,
                (behaviorKind, testDescriptorCreator, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.CloneAndApplyBehavior(behaviorKind);
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);
                    PayloadWriterTestDescriptor<ODataProperty> testDescriptor = testDescriptorCreator(behaviorKind, testConfiguration.Version);
                    if (testDescriptor != null)
                    {
                        testDescriptor.RunTopLevelPropertyPayload(testConfiguration, baselineLogger: this.Logger);
                    }
                });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Verifies that non-null values for non-nullable properties on entries are correctly validated against metadata and correctly serialized.")]
        public void NonNullablePropertyOnEntryTest()
        {
            // Primitive properties
            var testDescriptors = NonNullableEdmPrimitiveTypesWithValues.Select(nonNullablePrimitiveTypeWithValue =>
                new Func<TestODataBehaviorKind, ODataVersion, PayloadWriterTestDescriptor<ODataItem>>(
                    (behaviorKind, version) =>
                    {
                        if (!nonNullablePrimitiveTypeWithValue.Version.HasValue || nonNullablePrimitiveTypeWithValue.Version == version)
                        {
                            return this.CreateNonNullPropertyOnEntryTestDescriptor(
                                nonNullablePrimitiveTypeWithValue,
                                nonNullablePrimitiveTypeWithValue.Type != EdmPrimitiveTypeKind.String,
                                version,
                                "nonNullProperty");
                        }

                        return null;
                    }));

            this.CombinatorialEngineProvider.RunCombinations(
                TestWriterUtils.ODataBehaviorKinds,
                testDescriptors,
                this.WriterTestConfigurationProvider.AtomFormatConfigurationsWithIndent,
                (behaviorKind, testDescriptorCreator, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.CloneAndApplyBehavior(behaviorKind);
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);
                    var testDescriptor = testDescriptorCreator(behaviorKind, testConfiguration.Version);

                    if (testDescriptor != null)
                    {
                        TestWriterUtils.WriteAndVerifyODataPayload(
                            testDescriptor,
                            testConfiguration,
                            this.Assert,
                            this.Logger);
                    }
                });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Verifies that we fail on properties with invalid names on complex values.")]
        public void PropertyWithInvalidNameOnComplexValueTest()
        {
            var nonNullablePrimitiveTypeWithValue = NonNullableEdmPrimitiveTypesWithValues.First();
            var testDescriptorCreators = new Func<string, ODataVersion, PayloadWriterTestDescriptor<ODataProperty>>[]
            {
                new Func<string, ODataVersion, PayloadWriterTestDescriptor<ODataProperty>>(
                    (propertyName, version) =>
                    {
                        return this.CreateNonNullPropertyOnComplexValueTestDescriptor(
                            nonNullablePrimitiveTypeWithValue,
                            nonNullablePrimitiveTypeWithValue.Type != EdmPrimitiveTypeKind.String,
                            version,
                            propertyName);
                    })
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptorCreators,
                InvalidPropertyNames,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent,
                (testDescriptorCreator, propertyName, testConfiguration) =>
                {
                    PayloadWriterTestDescriptor<ODataProperty> testDescriptor = testDescriptorCreator(propertyName, testConfiguration.Version);
                    testDescriptor.RunTopLevelPropertyPayload(testConfiguration, baselineLogger: this.Logger);
                });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Verifies that we fail on properties with invalid names on entries.")]
        public void PropertyWithInvalidNameOnEntryTest()
        {
            var nonNullablePrimitiveTypeWithValue = NonNullableEdmPrimitiveTypesWithValues.First();
            var testDescriptorCreators = new Func<string, ODataVersion, PayloadWriterTestDescriptor<ODataItem>>[]
            {
                new Func<string, ODataVersion, PayloadWriterTestDescriptor<ODataItem>>(
                    (propertyName, version) =>
                    {
                        return this.CreateNonNullPropertyOnEntryTestDescriptor(
                            nonNullablePrimitiveTypeWithValue,
                            nonNullablePrimitiveTypeWithValue.Type != EdmPrimitiveTypeKind.String,
                            version,
                            propertyName);
                    })
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptorCreators,
                InvalidPropertyNames,
                this.WriterTestConfigurationProvider.AtomFormatConfigurationsWithIndent,
                (testDescriptorCreator, propertyName, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyODataPayload(
                        testDescriptorCreator(propertyName, testConfiguration.Version),
                        testConfiguration,
                        this.Assert,
                        this.Logger);
                });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Verify that TypeName property and ODataTypeAnnotation behave as expected on a complex value.")]
        public void ComplexValueODataTypeAnnotationTest()
        {
            // TODO: Move this string to resources and localize it
            const string ODataJsonLightValueSerializer_MissingTypeNameOnComplex = "A type name was not provided for an instance of ODataComplexValue.";

            var testCases = new[]
            {
                new
                {
                    TypeName = (string)null,
                    ODataTypeAnnotation = (ODataTypeAnnotation)null,
                    XmlTypeName = MissingTypeNameSentinelTextAtom,
                    JsonLightTypeName = (string)null,
                    ExpectedExceptionInJsonLight = (object)ODataExpectedExceptions.ODataException("ODataJsonLightValueSerializer_MissingTypeNameOnComplex"),
                    ExpectedExceptionInAtom = (object) null,
                    ExpectedExceptionInJsonLightForResponse = (object)ODataExpectedExceptions.ODataException("ODataContextUriBuilder_TypeNameMissingForProperty"),
                    ExpectedExceptionInAtomForResponse = (object)ODataExpectedExceptions.ODataException("ODataContextUriBuilder_TypeNameMissingForProperty")
                },
                new
                {
                    TypeName = "TestNS.MyType",
                    ODataTypeAnnotation = (ODataTypeAnnotation)null,
                    XmlTypeName = "<typeName>TestNS.MyType</typeName>",
                    JsonLightTypeName = MissingTypeNameSentinelTextJson,
                    ExpectedExceptionInJsonLight = (object) null,
                    ExpectedExceptionInAtom = (object) null,
                    ExpectedExceptionInJsonLightForResponse = (object) null,
                    ExpectedExceptionInAtomForResponse = (object) null,
                },
                new
                {
                    TypeName = "TestNS.MyType",
                    ODataTypeAnnotation = new ODataTypeAnnotation(),
                    XmlTypeName = MissingTypeNameSentinelTextAtom,
                    JsonLightTypeName = MissingTypeNameSentinelTextJson,
                    ExpectedExceptionInJsonLight = (object) null,
                    ExpectedExceptionInAtom = (object) null,
                    ExpectedExceptionInJsonLightForResponse = (object) null,
                    ExpectedExceptionInAtomForResponse = (object) null,
                },
                new
                {
                    TypeName = (string)null,
                    ODataTypeAnnotation = new ODataTypeAnnotation("DifferentType"),
                    XmlTypeName = "<typeName>DifferentType</typeName>",
                    JsonLightTypeName = (string) null,
                    ExpectedExceptionInJsonLight = (object) new ODataException(ODataJsonLightValueSerializer_MissingTypeNameOnComplex),
                    ExpectedExceptionInAtom = (object) null,
                    ExpectedExceptionInJsonLightForResponse = (object) null,
                    ExpectedExceptionInAtomForResponse = (object) null,
                },
                new
                {
                    TypeName = (string)null,
                    ODataTypeAnnotation = new ODataTypeAnnotation(string.Empty),
                    XmlTypeName = "<typeName></typeName>",
                    JsonLightTypeName = (string) null,
                    ExpectedExceptionInJsonLight = (object)ODataExpectedExceptions.ODataException("ODataJsonLightValueSerializer_MissingTypeNameOnComplex"),
                    ExpectedExceptionInAtom = (object) null,
                    ExpectedExceptionInJsonLightForResponse = (object)ODataExpectedExceptions.ODataException("ODataContextUriBuilder_TypeNameMissingForProperty"),
                    ExpectedExceptionInAtomForResponse = (object)ODataExpectedExceptions.ODataException("ODataContextUriBuilder_TypeNameMissingForProperty"),
                },
                new
                {
                    TypeName = "TestNS.MyType",
                    ODataTypeAnnotation = new ODataTypeAnnotation("DifferentType"),
                    XmlTypeName = "<typeName>DifferentType</typeName>",
                    JsonLightTypeName = "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"DifferentType\"",
                    ExpectedExceptionInJsonLight = (object) null,
                    ExpectedExceptionInAtom = (object) null,
                    ExpectedExceptionInJsonLightForResponse = (object) null,
                    ExpectedExceptionInAtomForResponse = (object) null,
                },
            };

            var testDescriptors = testCases.Select(tc =>
            {
                EdmModel model = new EdmModel();

                var complexType = new EdmComplexType("TestNS", "MyType");
                complexType.AddStructuralProperty("TestProperty", EdmCoreModel.Instance.GetString(isNullable: true));
                model.AddElement(complexType);

                var owningEntityType = new EdmEntityType("TestNS", "OwningEntityType");
                owningEntityType.AddStructuralProperty("PropertyName", new EdmComplexTypeReference(complexType, isNullable: true));
                model.AddElement(owningEntityType);

                var container = new EdmEntityContainer("TestNS", "TestContainer");
                model.AddElement(container);

                ODataComplexValue complexValue = new ODataComplexValue();
                complexValue.TypeName = tc.TypeName;
                complexValue.Properties = new[] { new ODataProperty() { Name = "TestProperty", Value = "TestValue" } };
                if (tc.ODataTypeAnnotation != null)
                {
                    complexValue.TypeAnnotation = tc.ODataTypeAnnotation;
                }

                return new PayloadWriterTestDescriptor<ODataProperty>(
                    this.Settings,
                    new ODataProperty { Name = "PropertyName", Value = complexValue },
                    (testConfiguration) =>
                    {
                        if (testConfiguration.Format == ODataFormat.Json)
                        {
                            if (tc.ExpectedExceptionInJsonLight is Exception)
                            {
                                return new WriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                                {
                                    ExpectedException = (Exception)tc.ExpectedExceptionInJsonLight
                                };
                            }

                            var exception = testConfiguration.IsRequest ? tc.ExpectedExceptionInJsonLight : tc.ExpectedExceptionInJsonLightForResponse;
                            if (exception is ExpectedException)
                            {
                                return new WriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                                {
                                    ExpectedException2 = (ExpectedException)exception
                                };
                            }

                            return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                            {
                                FragmentExtractor = (result) =>
                                {
                                    var complexObject = JsonLightWriterUtils.TrimWhitespace(result).Object();
                                    JsonProperty typeProperty = null;
                                    if (complexObject != null)
                                    {
                                        typeProperty = complexObject.Property(JsonLightConstants.ODataTypeAnnotationName);
                                    }

                                    return typeProperty == null ? MissingTypeNameSentinelJsonProperty : typeProperty.RemoveAllAnnotations(true);
                                },
                                Json = tc.JsonLightTypeName
                            };
                        }
                        else
                        {
                            throw new NotSupportedException("Format " + testConfiguration.Format.GetType().Name + " is not supported.");
                        }
                    })
                {
                    Model = model,
                    PayloadEdmElementContainer = owningEntityType
                };
            });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent,
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);
                    testDescriptor.RunTopLevelPropertyPayload(testConfiguration, baselineLogger: this.Logger);
                });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Verify that TypeName property and ODataTypeAnnotation behave as expected on a collection.")]
        public void CollectionValueODataTypeAnnotationTest()
        {
            #region test cases

            var testCases = new[]
            {
                new
                {
                    TypeName = (string)null,
                    ODataTypeAnnotation = (ODataTypeAnnotation)null,
                    XmlTypeName = MissingTypeNameSentinelTextAtom,
                    JsonTypeName = MissingTypeNameSentinelTextJson,
                    JsonLightTypeName = MissingTypeNameSentinelTextJson,
                    ExpectedExceptionInJsonLight = (object)ODataExpectedExceptions.ODataException("WriterValidationUtils_MissingTypeNameWithMetadata"),
                    ExpectedExceptionInAtom = new object(),
                    ExpectedExceptionInJsonLightForResponse = (object)ODataExpectedExceptions.ODataException("ODataContextUriBuilder_TypeNameMissingForProperty"),
                    ExpectedExceptionInAtomForResponse = (object)ODataExpectedExceptions.ODataException("ODataContextUriBuilder_TypeNameMissingForProperty"),
                },
                new
                {
                    TypeName = EntityModelUtils.GetCollectionTypeName("Edm.Int32"),
                    ODataTypeAnnotation = (ODataTypeAnnotation)null,
                    XmlTypeName = "<typeName>" + EntityModelUtils.GetCollectionTypeName("Edm.Int32") + "</typeName>",
                    JsonTypeName = "\"type\":\"" + EntityModelUtils.GetCollectionTypeName("Edm.Int32") + "\"",
                    JsonLightTypeName = MissingTypeNameSentinelTextJson,
                    ExpectedExceptionInJsonLight = new object(),
                    ExpectedExceptionInAtom = new object(),
                    ExpectedExceptionInJsonLightForResponse = new object(),
                    ExpectedExceptionInAtomForResponse = new object(),
                },
                new
                {
                    TypeName = EntityModelUtils.GetCollectionTypeName("Edm.Int32"),
                    ODataTypeAnnotation = new ODataTypeAnnotation(),
                    XmlTypeName = MissingTypeNameSentinelTextAtom,
                    JsonTypeName = MissingTypeNameSentinelTextJson,
                    JsonLightTypeName = MissingTypeNameSentinelTextJson,
                    ExpectedExceptionInJsonLight = new object(),
                    ExpectedExceptionInAtom = new object(),
                    ExpectedExceptionInJsonLightForResponse = new object(),
                    ExpectedExceptionInAtomForResponse = new object(),
                },
                new
                {
                    TypeName = (string)null,
                    ODataTypeAnnotation = new ODataTypeAnnotation(EntityModelUtils.GetCollectionTypeName("Edm.String")),
                    XmlTypeName = "<typeName>" + EntityModelUtils.GetCollectionTypeName("Edm.String") + "</typeName>",
                    JsonTypeName = "\"type\":\"" + EntityModelUtils.GetCollectionTypeName("Edm.String") + "\"",
                    JsonLightTypeName = "\"@odata.type\":\"#" + EntityModelUtils.GetCollectionTypeName("Edm.String") + "\"",
                    ExpectedExceptionInJsonLight = (object)ODataExpectedExceptions.ODataException("WriterValidationUtils_MissingTypeNameWithMetadata"),
                    ExpectedExceptionInAtom = new object(),
                    ExpectedExceptionInJsonLightForResponse = (object)ODataExpectedExceptions.ODataException("ODataJsonLightValueSerializer_MissingTypeNameOnCollection"),
                    ExpectedExceptionInAtomForResponse = new object(),
                },
                new
                {
                    TypeName = (string)null,
                    ODataTypeAnnotation = new ODataTypeAnnotation(string.Empty),
                    XmlTypeName = "<typeName></typeName>",
                    JsonTypeName = "\"type\":\"\"",
                    JsonLightTypeName = "\"@odata.type\":\"\"",
                    ExpectedExceptionInJsonLight = (object)ODataExpectedExceptions.ODataException("WriterValidationUtils_MissingTypeNameWithMetadata"),
                    ExpectedExceptionInAtom = new object(),
                    ExpectedExceptionInJsonLightForResponse = (object)ODataExpectedExceptions.ODataException("ODataContextUriBuilder_TypeNameMissingForProperty"),
                    ExpectedExceptionInAtomForResponse = (object)ODataExpectedExceptions.ODataException("ODataContextUriBuilder_TypeNameMissingForProperty"),
                },
                new
                {
                    TypeName = (string)null,
                    ODataTypeAnnotation = new ODataTypeAnnotation("NonCollectionTypeName"),
                    XmlTypeName = "<typeName>NonCollectionTypeName</typeName>",
                    JsonTypeName = "\"type\":\"NonCollectionTypeName\"",
                    JsonLightTypeName = "\"@odata.type\":\"#NonCollectionTypeName\"",
                    ExpectedExceptionInJsonLight = (object)ODataExpectedExceptions.ODataException("WriterValidationUtils_MissingTypeNameWithMetadata"),
                    ExpectedExceptionInAtom = new object(),
                    ExpectedExceptionInJsonLightForResponse = (object)ODataExpectedExceptions.ODataException("ODataJsonLightValueSerializer_MissingTypeNameOnCollection"),
                    ExpectedExceptionInAtomForResponse = new object(),
                },
                new
                {
                    TypeName = EntityModelUtils.GetCollectionTypeName("Edm.Int32"),
                    ODataTypeAnnotation = new ODataTypeAnnotation(EntityModelUtils.GetCollectionTypeName("Edm.String")),
                    XmlTypeName = "<typeName>" + EntityModelUtils.GetCollectionTypeName("Edm.String") + "</typeName>",
                    JsonTypeName = "\"type\":\"" + EntityModelUtils.GetCollectionTypeName("Edm.String") + "\"",
                    JsonLightTypeName = "\"@odata.type\":\"#" + EntityModelUtils.GetCollectionTypeName("Edm.String") + "\"",
                    ExpectedExceptionInJsonLight = new object(),
                    ExpectedExceptionInAtom = new object(),
                    ExpectedExceptionInJsonLightForResponse = new object(),
                    ExpectedExceptionInAtomForResponse = new object(),
                },
            };
            #endregion test cases

            var testDescriptors = testCases.Select(tc =>
            {
                EdmModel model = new EdmModel();

                var owningEntityType = new EdmEntityType("TestNS", "OwningEntityType");
                owningEntityType.AddStructuralProperty("PropertyName", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(isNullable: false)));
                model.AddElement(owningEntityType);

                var container = new EdmEntityContainer("TestNS", "TestContainer");
                model.AddElement(container);

                ODataComplexValue complexValue = new ODataComplexValue();
                complexValue.TypeName = tc.TypeName;
                complexValue.Properties = new[] { new ODataProperty() { Name = "TestProperty", Value = "TestValue" } };
                if (tc.ODataTypeAnnotation != null)
                {
                    complexValue.TypeAnnotation = tc.ODataTypeAnnotation;
                }

                ODataCollectionValue collection = new ODataCollectionValue();
                collection.TypeName = tc.TypeName;
                if (tc.ODataTypeAnnotation != null)
                {
                    collection.TypeAnnotation = tc.ODataTypeAnnotation;
                }

                return new PayloadWriterTestDescriptor<ODataProperty>(
                    this.Settings,
                    new ODataProperty { Name = "PropertyName", Value = collection },
                    (testConfiguration) =>
                    {
                        if (testConfiguration.Format == ODataFormat.Json)
                        {
                            if (testConfiguration.Format == ODataFormat.Json)
                            {
                                if (tc.ExpectedExceptionInJsonLight is Exception)
                                {
                                    return new WriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                                    {
                                        ExpectedException = (Exception)tc.ExpectedExceptionInJsonLight
                                    };
                                }

                                var exception = testConfiguration.IsRequest ? tc.ExpectedExceptionInJsonLight : tc.ExpectedExceptionInJsonLightForResponse;

                                if (exception is ExpectedException)
                                {
                                    return new WriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                                    {
                                        ExpectedException2 = (ExpectedException)exception
                                    };
                                }
                            }

                            return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                            {
                                FragmentExtractor = (result) =>
                                {
                                    var topLevelJsonObject = JsonLightWriterUtils.TrimWhitespace(result).Object();
                                    JsonProperty typeProperty = null;
                                    if (topLevelJsonObject != null)
                                    {
                                        typeProperty = topLevelJsonObject.Property(JsonLightConstants.ODataTypeAnnotationName);
                                    }

                                    return typeProperty == null ? MissingTypeNameSentinelJsonProperty : typeProperty.RemoveAllAnnotations(true);
                                },
                                Json = tc.JsonLightTypeName
                            };
                        }

                        throw new NotSupportedException("Format " + testConfiguration.Format.GetType().Name + " is not supported.");
                    })
                {
                    Model = model,
                    PayloadEdmElementContainer = owningEntityType
                };
            });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent,
                (testDescriptor, testConfig) =>
                {
                    testConfig = testConfig.Clone();
                    testConfig.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);
                    testDescriptor.RunTopLevelPropertyPayload(testConfig, baselineLogger: this.Logger);
                });
        }

        [Ignore] // Remove Atom
        // [TestMethod, Variation(Description = "Verify that ODataTypeAnnotation behave as expected on a collection.")]
        public void PrimitiveValueODataTypeAnnotationTest()
        {
            #region test case
            var testCases = new[]
            {
                new
                {
                    DebugDescription = "A null ODataTypeAnnotation should invoke default primitive type name writing behavior.",
                    ODataTypeAnnotation = (ODataTypeAnnotation)null,
                    XmlTypeName = "<typeName>Edm.Int64</typeName>",
                    JsonLightTypeName = MissingTypeNameSentinelTextJson,   // Since this is a declared property, its type name isn't written by default in JSON light.
                },
                new
                {
                    DebugDescription = "A ODataTypeAnnotation with a null TypeName should force primitive type names to not be written, regardless of format.",
                    ODataTypeAnnotation = new ODataTypeAnnotation(),
                    XmlTypeName = MissingTypeNameSentinelTextAtom,
                    JsonLightTypeName = MissingTypeNameSentinelTextJson,
                },
                new
                {
                    DebugDescription = "A ODataTypeAnnotation with a TypeName of \"DifferentType\" should cause that type name to be written for primitive values, except in JSON verbose, where the format doesn't allow explicit type names for primitives.",
                    ODataTypeAnnotation = new ODataTypeAnnotation("DifferentType"),
                    XmlTypeName = "<typeName>DifferentType</typeName>",
                    JsonLightTypeName = "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"DifferentType\"",
                },
                new
                {
                    DebugDescription = "A ODataTypeAnnotation with an empty string TypeName should cause an empty type name to be written for primitive values, except in JSON verbose, where the format doesn't allow explicit type names for primitives.",
                    ODataTypeAnnotation = new ODataTypeAnnotation(string.Empty),
                    XmlTypeName = "<typeName></typeName>",
                    JsonLightTypeName = "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"\"",
                }
            };
            #endregion test cases

            var testDescriptors = testCases.Select(tc =>
            {
                EdmModel model = new EdmModel();

                var owningEntityType = new EdmEntityType("TestNS", "OwningEntityType");
                owningEntityType.AddStructuralProperty("PropertyName", EdmCoreModel.Instance.GetInt64(isNullable: false));
                model.AddElement(owningEntityType);

                var container = new EdmEntityContainer("TestNS", "TestContainer");
                model.AddElement(container);

                ODataPrimitiveValue primitiveValue = new ODataPrimitiveValue((Int64)42);
                if (tc.ODataTypeAnnotation != null)
                {
                    primitiveValue.TypeAnnotation = tc.ODataTypeAnnotation;
                }

                return new PayloadWriterTestDescriptor<ODataProperty>(
                    this.Settings,
                    new ODataProperty { Name = "PropertyName", Value = primitiveValue },
                    (testConfiguration) =>
                    {
                        if (testConfiguration.Format == ODataFormat.Json)
                        {
                            return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                            {
                                FragmentExtractor = (result) =>
                                {
                                    var jsonObject = JsonLightWriterUtils.TrimWhitespace(result).Object();
                                    JsonProperty typeProperty = null;
                                    if (jsonObject != null)
                                    {
                                        typeProperty = jsonObject.Property(JsonLightConstants.ODataTypeAnnotationName);
                                    }

                                    return typeProperty == null ? MissingTypeNameSentinelJsonProperty : typeProperty.RemoveAllAnnotations(true);
                                },
                                Json = tc.JsonLightTypeName
                            };
                        }

                        throw new NotSupportedException("Format " + testConfiguration.Format.GetType().Name + " is not supported.");
                    })
                {
                    Model = model,
                    PayloadEdmElementContainer = owningEntityType
                };
            });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                // We exclude Verbose JSON from this test because it has no way of writing type names on primitive values.
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent,
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);
                    testDescriptor.RunTopLevelPropertyPayload(testConfiguration, baselineLogger: this.Logger);
                });
        }

        private PayloadWriterTestDescriptor<ODataProperty> CreateNullPropertyOnComplexValueTestDescriptor(
            IEdmTypeReference dataType,
            ODataVersion version,
            bool allowNulls)
        {
            EdmPrimitiveTypeReference primitiveDataType = dataType as EdmPrimitiveTypeReference;
            EdmComplexTypeReference complexDataType = dataType as EdmComplexTypeReference;
            string typeName = primitiveDataType == null ? complexDataType.Definition.TestFullName() : primitiveDataType.Definition.TestFullName();

            var td = new PayloadWriterTestDescriptor<ODataProperty>(
                this.Settings,
                new ODataProperty
                {
                    Name = "complex",
                    Value = new ODataComplexValue
                    {
                        TypeName = "TestModel.ComplexType",
                        Properties = new[] { new ODataProperty {
                                    Name = "nullProperty",
                                    Value = null } }
                    }
                },
                (tc) =>
                {
                    if (allowNulls)
                    {
                        if (tc.Format == ODataFormat.Json)
                        {
                            return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                            {
                                Json = "\"nullProperty\":null",
                                FragmentExtractor = (result) => JsonLightWriterUtils.TrimWhitespace(result).Object().Property("nullProperty")
                            };
                        }
                        else
                        {
                            string formatName = tc.Format == null ? "null" : tc.Format.GetType().Name;
                            throw new NotSupportedException("Format " + formatName + " + is not supported.");
                        }
                    }
                    else
                    {
                        return new WriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                        {
                            ExpectedException2 = ODataExpectedExceptions.ODataException("WriterValidationUtils_NonNullablePropertiesMustNotHaveNullValue", "nullProperty", typeName),
                        };
                    }
                });

            EdmModel edmModel = new EdmModel();

            var complexType = new EdmComplexType("TestModel", "ComplexType");
            complexType.AddStructuralProperty("nullProperty", dataType);
            edmModel.AddElement(complexType);

            var entityType = new EdmEntityType("TestModel", "EntityType");
            entityType.AddStructuralProperty("complex", new EdmComplexTypeReference(complexType, isNullable: true));
            edmModel.AddElement(entityType);

            var container = new EdmEntityContainer("TestNS", "TestContainer");
            edmModel.AddElement(container);

            td.Model = edmModel as IEdmModel;

            td.PayloadEdmElementContainer = entityType;

            // NOTE: important to set Edm version to control null value validation!
            td.Model.SetEdmVersion(version.ToSystemVersion());

            return td;
        }


        private PayloadWriterTestDescriptor<ODataProperty> CreateNonNullPropertyOnComplexValueTestDescriptor(
            NonNullableEdmPrimitiveTypeWithValue nonNullablePrimitiveTypeWithValue,
            bool typeAttributeExpected,
            ODataVersion version,
            string propertyName)
        {
            string typeName = EdmCoreModel.Instance.GetPrimitive(nonNullablePrimitiveTypeWithValue.Type, true).Definition.TestFullName();

            bool isInvalidPropertyName = propertyName.IndexOfAny(InvalidCharactersInPropertyNames) >= 0;

            var td = new PayloadWriterTestDescriptor<ODataProperty>(
                this.Settings,
                new ODataProperty
                {
                    Name = "complex",
                    Value = new ODataComplexValue
                    {
                        TypeName = "TestModel.ComplexType",
                        Properties = new[] { new ODataProperty {
                                    Name = propertyName,
                                    Value = nonNullablePrimitiveTypeWithValue.Value } }
                    }
                },
                (tc) =>
                {
                    if (tc.Format == ODataFormat.Json)
                    {
                        return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                        {
                            Json = "\"" + propertyName + "\":" + nonNullablePrimitiveTypeWithValue.JsonLightRepresentation,
                            FragmentExtractor = (result) => JsonLightWriterUtils.TrimWhitespace(result).Object().Property(propertyName),
                            ExpectedException2 = isInvalidPropertyName
                                    ? ODataExpectedExceptions.ODataException("ValidationUtils_PropertiesMustNotContainReservedChars", propertyName, "':', '.', '@'")
                                    : null,
                        };
                    }
                    else
                    {
                        string formatName = tc.Format == null ? "null" : tc.Format.GetType().Name;
                        throw new NotSupportedException("Format " + formatName + " is not supported.");
                    }
                });

            EdmModel edmModel = new EdmModel();

            var complexType = new EdmComplexType("TestModel", "ComplexType");
            complexType.AddStructuralProperty(propertyName, EdmCoreModel.Instance.GetPrimitive(nonNullablePrimitiveTypeWithValue.Type, true));
            edmModel.AddElement(complexType);

            var entityType = new EdmEntityType("TestModel", "EntityType");
            entityType.AddStructuralProperty("complex", new EdmComplexTypeReference(complexType, isNullable: true));
            edmModel.AddElement(entityType);

            var container = new EdmEntityContainer("TestNS", "TestContainer");
            edmModel.AddElement(container);

            td.Model = edmModel as IEdmModel;

            // NOTE: important to set Edm version and to control null value validation!
            td.Model.SetEdmVersion(version.ToSystemVersion());
            td.PayloadEdmElementContainer = entityType;

            return td;
        }

        private PayloadWriterTestDescriptor<ODataItem> CreateNullPropertyOnEntryTestDescriptor(
            IEdmTypeReference dataType,
            ODataVersion version,
            bool allowNulls)
        {
            EdmPrimitiveTypeReference primitiveDataType = dataType as EdmPrimitiveTypeReference;
            EdmComplexTypeReference complexDataType = dataType as EdmComplexTypeReference;
            string typeName = primitiveDataType == null ? complexDataType.Definition.TestFullName() : primitiveDataType.Definition.TestFullName();

            var td = new PayloadWriterTestDescriptor<ODataItem>(
                this.Settings,
                new ODataResource
                {
                    TypeName = "TestModel.EntityType",
                    Properties = new[] { new ODataProperty {
                                Name = "nullProperty",
                                Value = null } },
                    SerializationInfo = MySerializationInfo
                },
                (tc) =>
                {
                    if (allowNulls)
                    {
                        return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                        {
                            Json = "\"nullProperty\":null",
                            FragmentExtractor = (result) => JsonUtils.UnwrapTopLevelValue(tc, result).Object().Property("nullProperty")
                        };
                    }
                    else
                    {
                        return new WriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                        {
                            ExpectedException2 = ODataExpectedExceptions.ODataException("WriterValidationUtils_NonNullablePropertiesMustNotHaveNullValue", "nullProperty", typeName),
                        };
                    }
                });

            // doesn't have a clone method so has to new an EdmModel
            EdmModel edmModel = new EdmModel();

            var entityType = new EdmEntityType("TestModel", "EntityType");
            edmModel.AddElement(entityType);

            var container = new EdmEntityContainer("TestNS", "TestContainer");
            entityType.AddStructuralProperty("nullProperty", dataType);
            edmModel.AddElement(container);

            td.Model = edmModel as IEdmModel;

            // NOTE: important to set Edm version and to control null value validation!
            td.Model.SetEdmVersion(version.ToSystemVersion());

            return td;
        }

        private PayloadWriterTestDescriptor<ODataItem> CreateNonNullPropertyOnEntryTestDescriptor(
            NonNullableEdmPrimitiveTypeWithValue nonNullablePrimitiveTypeWithValue,
            bool typeAttributeExpected,
            ODataVersion version,
            string propertyName)
        {
            string typeName = EdmCoreModel.Instance.GetPrimitive(nonNullablePrimitiveTypeWithValue.Type, true).Definition.TestFullName();

            bool isInvalidPropertyName = propertyName.IndexOfAny(InvalidCharactersInPropertyNames) >= 0;

            var td = new PayloadWriterTestDescriptor<ODataItem>(
                this.Settings,
                new ODataResource
                {
                    TypeName = "TestModel.EntityType",
                    Properties = new[] { new ODataProperty {
                                Name = propertyName,
                                Value = nonNullablePrimitiveTypeWithValue.Value } },
                    SerializationInfo = MySerializationInfo
                },
                (tc) =>
                {
                    if (tc.Format == ODataFormat.Json)
                    {
                        return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                        {
                            Json = "\"" + JsonLightConstants.ODataValuePropertyName + "\":" + nonNullablePrimitiveTypeWithValue.JsonLightRepresentation,
                            FragmentExtractor = (result) => JsonLightWriterUtils.TrimWhitespace(result).Object().Property(propertyName),
                            ExpectedException2 = isInvalidPropertyName
                                    ? ODataExpectedExceptions.ODataException("ValidationUtils_PropertiesMustNotContainReservedChars", propertyName, "':', '.', '@'")
                                    : null,
                        };
                    }
                    else
                    {
                        string formatName = tc.Format == null ? "null" : tc.Format.GetType().Name;
                        throw new NotSupportedException("Format " + formatName + " + is not supported.");
                    }
                });

            EdmModel edmModel = new EdmModel();

            var entityType = new EdmEntityType("TestModel", "EntityType");
            entityType.AddStructuralProperty(propertyName, EdmCoreModel.Instance.GetPrimitive(nonNullablePrimitiveTypeWithValue.Type, false));
            edmModel.AddElement(entityType);

            var container = new EdmEntityContainer("TestNS", "TestContainer");
            edmModel.AddElement(container);

            td.Model = edmModel as IEdmModel;


            // NOTE: important to set Edm version and to control null value validation!
            td.Model.SetEdmVersion(version.ToSystemVersion());

            return td;
        }

        private void TestSinglePropertyGeneration(ODataFormat testFormat, IEnumerable<EntityInstance> generator)
        {
            List<PropertyInstance> payload = new List<PropertyInstance>();
            foreach (EntityInstance entity in generator)
            {
                foreach (PropertyInstance property in entity.Properties)
                {
                    // For top level property, it should be named with property name.
                    payload.Add(property);
                }
            }

            this.CombinatorialEngineProvider.RunCombinations(
                payload,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent.Where(c => !c.Synchronous && c.Format == testFormat),
                (testCase, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    this.WriteAndVerifyODataProperty(testCase, testConfiguration, null);
                });
        }

        /// <summary>
        /// Creates error test cases from the given set of entity instance payloads.
        /// </summary>
        /// <param name="payloads">The payloads get the complex properties from</param>
        /// <returns>A set of WriterTestDescriptors with mutated complexproperty elements where either name of a member of the complex property or its type are incorrect</returns>
        /// <remarks>Mutates the properties within the entities provided in payloads.</remarks>
        private IEnumerable<PayloadWriterTestDescriptor<ODataPayloadElement>> GenerateComplexPropertyErrorTestCases(IEnumerable<EntityInstance> payloads)
        {
            List<PayloadWriterTestDescriptor<ODataPayloadElement>> testCases = new List<PayloadWriterTestDescriptor<ODataPayloadElement>>();
            foreach (var payload in payloads)
            {
                var entityType = (payload.GetAnnotation<EntityModelTypeAnnotation>().EdmModelType).Definition;

                foreach (ComplexProperty property in payload.Properties.OfType<ComplexProperty>())
                {
                    var complexType = (IEdmComplexType)(((IEdmEntityType)entityType).Properties().Single(p => p.Name == property.Name).Type).Definition;
                    var duplicateProperty = property.DeepCopy();
                    var propertyInComplexValue = property.Value.Properties.First();
                    var primitiveType = complexType.Properties().Single(p => p.Name == propertyInComplexValue.Name).Type;

                    if (propertyInComplexValue.GetType() == typeof(PrimitiveProperty))
                    {
                        var primitive = propertyInComplexValue as PrimitiveProperty;
                        var oldType = primitive.Value.ClrValue.GetType();
                        if (oldType != typeof(string))
                        {
                            primitive.Value.ClrValue = primitive.Name;
                        }
                        else
                        {
                            primitive.Value.ClrValue = 3;
                        }

                        var newType = primitive.Value.ClrValue.GetType();
                        var isNullable = !newType.IsValueType || newType.IsGenericType && newType.GetGenericTypeDefinition() == typeof(Nullable<>);

                        testCases.Add(new PayloadWriterTestDescriptor<ODataPayloadElement>(
                            this.Settings,
                            property,
                            new PayloadWriterTestDescriptor.WriterTestExpectedResultCallback(
                                (config) =>
                                {
                                    return new WriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                                    {
                                        ExpectedException2 = ODataExpectedExceptions.ODataException(
                                            "ValidationUtils_IncompatiblePrimitiveItemType",
                                            "Edm." + newType.Name,
                                            isNullable.ToString(),
                                            "Edm." + oldType.Name,
                                            primitiveType.IsNullable.ToString())
                                    };
                                }
                            )
                        ));
                    }
                }
            }
            return testCases;
        }
    }
}

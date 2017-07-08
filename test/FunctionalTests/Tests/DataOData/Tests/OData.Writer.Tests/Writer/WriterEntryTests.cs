//---------------------------------------------------------------------
// <copyright file="WriterEntryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Writer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Spatial;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Json;
    using Microsoft.Test.Taupo.OData.Json.TextAnnotations;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Atom;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Fixups;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Json;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MetadataUtils = Microsoft.Test.OData.Utils.Metadata.MetadataUtils;
    using TestModels = Microsoft.Test.OData.Utils.Metadata.TestModels;

    /// <summary>
    /// Tests for writing entries with the OData writer.
    /// </summary>
    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/883
    // [TestClass, TestCase]
    public class WriterEntryTests : ODataWriterTestCase
    {
        private static readonly Uri ServiceDocumentUri = new Uri("http://www.odata.org");

        [InjectDependency(IsRequired = true)]
        public IPayloadGenerator PayloadGenerator { get; set; }

        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestExpectedResults.Settings ExpectedResultSettings { get; set; }

        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestDescriptor.Settings Settings { get; set; }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Test feed payloads from payload Generator.")]
        public void TaupoSingleEntryTests()
        {
            this.CombinatorialEngineProvider.RunCombinations(
                PayloadGenerator.GenerateAtomPayloads(),
                this.WriterTestConfigurationProvider.AtomFormatConfigurationsWithIndent,
                (testCase, testConfiguration) =>
                {
                    WriterTestConfiguration newConfiguration = testConfiguration.Clone();
                    newConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    this.WriteAndVerifyODataPayloadElement(testCase, newConfiguration);
                });

            // TODO: Fix places where we've lost JsonVerbose coverage to add JsonLight
            ////this.CombinatorialEngineProvider.RunCombinations(
            ////    PayloadGenerator.GenerateJsonPayloads(),
            ////    this.WriterTestConfigurationProvider.JsonLightFormatConfigurationsWithIndent,
            ////    (testCase, testConfiguration) =>
            ////    {
            ////        this.WriteAndVerifyODataPayloadElement(testCase, testConfiguration);
            ////    });
        }
        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Test default entry payload.")]
        public void DefaultEntryTests()
        {
            ODataResource entry = ObjectModelUtils.CreateDefaultEntryWithAtomMetadata();

            PayloadWriterTestDescriptor<ODataItem>.WriterTestExpectedResultCallback expectedCallback = (testConfiguration) =>
                {
                    var jsonResult = string.Join("$(NL)",
                        "{",
                        "\"@odata.id\":\"http://www.odata.org/entryid\",\"@odata.readLink\":\"http://www.odata.org/entry/readlink\"",
                        "}"
                    );

                    return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        Json = jsonResult,
                        FragmentExtractor = (result) => result.RemoveAllAnnotations(true)
                    };
                };

            var testCases = new[]
            {
                new PayloadWriterTestDescriptor<ODataItem>(this.Settings, entry, expectedCallback)
            };

            // TODO: Fix places where we've lost JsonVerbose coverage to add JsonLight
            this.CombinatorialEngineProvider.RunCombinations(
                testCases.PayloadCases(WriterPayloads.EntryPayloads),
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent.Where(tc => false),
                (testCase, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyODataPayload(testCase, testConfiguration, this.Assert, this.Logger);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Test entry with entry Id not null.")]
        public void EntryIdTests()
        {
            ODataResource idEntry = ObjectModelUtils.CreateDefaultEntry();
            idEntry.Id = new Uri("Some:Id");
            idEntry.ReadLink = null;

            PayloadWriterTestDescriptor<ODataItem>.WriterTestExpectedResultCallback expectedCallback = (testConfiguration) =>
               {
                   var jsonResult = string.Join("$(NL)",
                       "{",
                       "\"@odata.id\":\"SomeId\"",
                       "}"
                   );

                   return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                   {
                       Json = jsonResult,
                       FragmentExtractor = (result) => result.RemoveAllAnnotations(true)
                   };
               };

            var testDescriptors = new[]
            {
                new PayloadWriterTestDescriptor<ODataItem>(this.Settings, idEntry, expectedCallback)
            };

            // TODO: Fix places where we've lost JsonVerbose coverage to add JsonLight
            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors.PayloadCases(WriterPayloads.EntryPayloads),
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent.Where(tc => false),
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyODataPayload(testDescriptor.DeferredLinksToEntityReferenceLinksInRequest(testConfiguration), testConfiguration, this.Assert, this.Logger);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Test entry with self and/or edit link.")]
        public void NoEntryIdTests()
        {
            ODataResource noIdEntry = ObjectModelUtils.CreateDefaultEntry();
            noIdEntry.Id = null;

            PayloadWriterTestDescriptor<ODataItem>.WriterTestExpectedResultCallback noIdExpectedCallback = (testConfiguration) =>
            {
                var jsonResult = string.Join("$(NL)",
                           "{",
                           "\"@odata.readLink\":\"http://www.odata.org/entry/readlink\"",
                           "}"
                       );

                return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                {
                    Json = jsonResult,
                    FragmentExtractor = (result) => result.RemoveAllAnnotations(true)
                };
            };

            var testCases = new[]
            {
                new PayloadWriterTestDescriptor<ODataItem>(this.Settings, noIdEntry, noIdExpectedCallback),
            };

            // TODO: Fix places where we've lost JsonVerbose coverage to add JsonLight
            this.CombinatorialEngineProvider.RunCombinations(
                testCases.PayloadCases(WriterPayloads.EntryPayloads),
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent.Where(tc => false),
                (testCase, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyODataPayload(testCase, testConfiguration, this.Assert, this.Logger);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Test entry with self and/or edit link.")]
        public void SelfAndEditLinkTests()
        {
            const string editLink = "http://www.odata.org/entry/editlink";
            ODataResource selfLinkEntry = ObjectModelUtils.CreateDefaultEntry();
            this.Assert.IsNotNull(selfLinkEntry.ReadLink, "ReadLink property of default entry must not be null.");

            ODataResource editLinkEntry = ObjectModelUtils.CreateDefaultEntry();
            editLinkEntry.ReadLink = null;
            editLinkEntry.EditLink = new Uri(editLink);

            ODataResource selfAndEditLinkEntry = ObjectModelUtils.CreateDefaultEntry();
            this.Assert.IsNotNull(selfLinkEntry.ReadLink, "ReadLink property of default entry must not be null.");
            selfAndEditLinkEntry.EditLink = new Uri(editLink);

            ODataResource noSelfOrEditLinkEntry = ObjectModelUtils.CreateDefaultEntryWithAtomMetadata();
            noSelfOrEditLinkEntry.ReadLink = null;
            this.Assert.IsNull(noSelfOrEditLinkEntry.ReadLink, "noSelfOrEditLinkEntry.ReadLink");
            this.Assert.IsNull(noSelfOrEditLinkEntry.EditLink, "noSelfOrEditLinkEntry.EditLink");

            Func<WriterTestConfiguration, bool, WriterTestExpectedResults> selfLinkExpectedCallback = (testConfiguration, editLinkExpected) =>
                {
                    return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        Json = "\"uri\":\"" + ObjectModelUtils.DefaultEntryReadLink + "\"",
                        FragmentExtractor = (result) => result.Object().PropertyObject("__metadata").Property("uri").RemoveAllAnnotations(true)
                    };
                };

            Func<WriterTestConfiguration, bool, WriterTestExpectedResults> editLinkExpectedCallback = (testConfiguration, selfLinkExpected) =>
                {
                    return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        Json = "\"uri\":\"" + editLink + "\"",
                        FragmentExtractor = (result) => result.Object().PropertyObject("__metadata").Property("uri").RemoveAllAnnotations(true)
                    };
                };

            PayloadWriterTestDescriptor<ODataItem>.WriterTestExpectedResultCallback noLinkExpectedCallback = (testConfiguration) =>
            {
                return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                {
                    Json = string.Join("$(NL)",
                            "{",
                            "\"__metadata\":{",
                            "\"id\":\"http://www.odata.org/entryid\"",
                            "}",
                            "}"
                        ),
                    FragmentExtractor = (result) => result.RemoveAllAnnotations(true)
                };
            };

            var testCases = new[]
            {
                new PayloadWriterTestDescriptor<ODataItem>(this.Settings, selfLinkEntry, (testConfiguration) => selfLinkExpectedCallback(testConfiguration, false)),
                new PayloadWriterTestDescriptor<ODataItem>(this.Settings, editLinkEntry, (testConfiguration) => editLinkExpectedCallback(testConfiguration, false)),
                new PayloadWriterTestDescriptor<ODataItem>(this.Settings, selfAndEditLinkEntry, (testConfiguration) => editLinkExpectedCallback(testConfiguration, true)),
                new PayloadWriterTestDescriptor<ODataItem>(this.Settings, noSelfOrEditLinkEntry, noLinkExpectedCallback),
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases.PayloadCases(WriterPayloads.EntryPayloads),
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent.Where(tc => false),
                (testCase, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyODataPayload(testCase, testConfiguration, this.Assert, this.Logger);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Verifies writer behavior around TypeName property.")]
        public void EntryTypeNameTest()
        {
            EdmModel model = new EdmModel();
            var container = new EdmEntityContainer("TestModel", "Default");
            model.AddElement(container);

            var entityType = new EdmEntityType("TestModel", "EntityType");
            model.AddElement(entityType);
            var entitySet = container.AddEntitySet("EntitySet", entityType);

            var derivedType = new EdmEntityType("TestModel", "DerivedType", entityType, isAbstract: false, isOpen: false);
            model.AddElement(derivedType);

            var testCases = new[]
            {
                new
                {
                    TypeName = (string)null,
                    ExpectedXml = "<categoryMissing/>",
                    ExpectedJsonLight = (string)null,
                    Model = (EdmModel)null
                },
                // empty string as TypeName is invalid and is covered in input validation tests.
                new
                {
                    TypeName = "MyType",
                    ExpectedXml = "<category term='MyType' scheme='" + TestAtomConstants.ODataSchemeNamespace +"' xmlns='" + TestAtomConstants.AtomNamespace + "' />",
                    ExpectedJsonLight = (string)null,
                    Model = (EdmModel)null
                },
                new
                {
                    TypeName = "TestModel.EntityType",
                    ExpectedXml = "<category term='TestModel.EntityType' scheme='" + TestAtomConstants.ODataSchemeNamespace +"' xmlns='" + TestAtomConstants.AtomNamespace + "' />",
                    ExpectedJsonLight = "\"typeMissing\":null",
                    Model = model
                },
                new
                {
                    TypeName = "TestModel.DerivedType",
                    ExpectedXml = "<category term='TestModel.DerivedType' scheme='" + TestAtomConstants.ODataSchemeNamespace +"' xmlns='" + TestAtomConstants.AtomNamespace + "' />",
                    ExpectedJsonLight = "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.DerivedType\"",
                    Model = model
                },
            };

            var testDescriptors = testCases.Select(tc =>
            {
                ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                entry.TypeName = tc.TypeName;
                return new PayloadWriterTestDescriptor<ODataItem>(
                    this.Settings,
                    entry,
                    (testConfiguration) =>
                    {
                        if (testConfiguration.Format == ODataFormat.Json)
                        {
                            return tc.ExpectedJsonLight == null ? null :
                                new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                                {
                                    FragmentExtractor = (result) =>
                                    {
                                        var typeProperty = result.Object().Property(JsonLightConstants.ODataTypeAnnotationName);
                                        return typeProperty == null ? (JsonValue)(new JsonProperty("typeMissing", new JsonPrimitiveValue(null))) : typeProperty.RemoveAllAnnotations(true);
                                    },
                                    Json = tc.ExpectedJsonLight
                                };
                        }
                        else
                        {
                            this.Assert.Fail("Unsupported format '{0}'.", testConfiguration.Format);
                            return null;
                        }
                    })
                {
                    Model = tc.Model,
                    PayloadEdmElementContainer = tc.Model == null ? null : entitySet,
                    PayloadEdmElementType = tc.Model == null ? null : entityType
                };
            });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyODataEdmPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        private sealed class EntrySerializationTypeNameAnnotationTestCase
        {
            public string TypeName { get; set; }
            public ODataTypeAnnotation TypeAnnotation { get; set; }
            public string ExpectedXml { get; set; }
            public string ExpectedJsonLight { get; set; }
            public IEdmModel Model { get; set; }
            public EntityType ExpectedEntityType { get; set; }
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Verifies writer behavior around TypeName property and SerializationTypeNameAnnotation.")]
        public void EntrySerializationTypeNameAnnotationTest()
        {
            EdmModel model = new EdmModel();
            var myEntityType = new EdmEntityType("TestModel", "MyType");
            model.AddElement(myEntityType);

            var derivedType = new EdmEntityType("TestModel", "DerivedType", myEntityType, isAbstract: false, isOpen: false);
            model.AddElement(derivedType);

            var container = new EdmEntityContainer("TestModel", "Default");
            model.AddElement(container);

            var entitySet = container.AddEntitySet("EntitySet", myEntityType);

            var testCases = new[]
            {
                new EntrySerializationTypeNameAnnotationTestCase
                {
                    TypeName = null,
                    TypeAnnotation = null,
                    ExpectedXml = "<categoryMissing/>",
                    Model = null
                },
                // empty string as TypeName is invalid and is covered in input validation tests.
                new EntrySerializationTypeNameAnnotationTestCase
                {
                    TypeName = "TestModel.MyType",
                    TypeAnnotation = null,
                    ExpectedXml = "<category term='TestModel.MyType' scheme='" + TestAtomConstants.ODataSchemeNamespace +"' xmlns='" + TestAtomConstants.AtomNamespace + "' />",
                    Model = null
                },
                new EntrySerializationTypeNameAnnotationTestCase
                {
                    TypeName = "TestModel.MyType",
                    TypeAnnotation = null,
                    ExpectedXml = "<category term='TestModel.MyType' scheme='" + TestAtomConstants.ODataSchemeNamespace +"' xmlns='" + TestAtomConstants.AtomNamespace + "' />",
                    ExpectedJsonLight = "\"typeMissing\":null",
                    Model = model
                },
                new EntrySerializationTypeNameAnnotationTestCase
                {
                    TypeName = "TestModel.DerivedType",
                    TypeAnnotation = null,
                    ExpectedXml = "<category term='TestModel.DerivedType' scheme='" + TestAtomConstants.ODataSchemeNamespace +"' xmlns='" + TestAtomConstants.AtomNamespace + "' />",
                    ExpectedJsonLight = "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.DerivedType\"",
                    Model = model
                },
                new EntrySerializationTypeNameAnnotationTestCase
                {
                    TypeName = null,
                    TypeAnnotation = new ODataTypeAnnotation(),
                    ExpectedXml = "<categoryMissing/>",
                    Model = null
                },
                new EntrySerializationTypeNameAnnotationTestCase
                {
                    TypeName = null,
                    TypeAnnotation = new ODataTypeAnnotation("DifferentType"),
                    ExpectedXml = "<category term='DifferentType' scheme='" + TestAtomConstants.ODataSchemeNamespace +"' xmlns='" + TestAtomConstants.AtomNamespace + "' />",
                    Model = null
                },
                new EntrySerializationTypeNameAnnotationTestCase
                {
                    TypeName = null,
                    TypeAnnotation = new ODataTypeAnnotation(string.Empty),
                    ExpectedXml = "<category term='' scheme='" + TestAtomConstants.ODataSchemeNamespace +"' xmlns='" + TestAtomConstants.AtomNamespace + "' />",
                    ExpectedJsonLight = "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"\"",
                    Model = null
                },
                new EntrySerializationTypeNameAnnotationTestCase
                {
                    TypeName = "TestNS.MyType",
                    TypeAnnotation = new ODataTypeAnnotation("DifferentType"),
                    ExpectedXml = "<category term='DifferentType' scheme='" + TestAtomConstants.ODataSchemeNamespace +"' xmlns='" + TestAtomConstants.AtomNamespace + "' />",
                    Model = null
                },
                new EntrySerializationTypeNameAnnotationTestCase
                {
                    TypeName = "TestModel.MyType",
                    TypeAnnotation = new ODataTypeAnnotation("DifferentType"),
                    ExpectedXml = "<category term='DifferentType' scheme='" + TestAtomConstants.ODataSchemeNamespace +"' xmlns='" + TestAtomConstants.AtomNamespace + "' />",
                    ExpectedJsonLight = "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"DifferentType\"",
                    Model = model
                },
            };

            var testDescriptors = testCases.Select(tc =>
            {
                ODataResource entry = ObjectModelUtils.CreateDefaultEntry();
                entry.TypeName = tc.TypeName;
                if (tc.TypeAnnotation != null)
                {
                    entry.TypeAnnotation = tc.TypeAnnotation;
                }

                return new PayloadWriterTestDescriptor<ODataItem>(
                    this.Settings,
                    entry,
                    (testConfiguration) =>
                    {
                        if (testConfiguration.Format == ODataFormat.Json)
                        {
                            return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                            {
                                FragmentExtractor = (result) =>
                                {
                                    var typeProperty = result.Object().Property(JsonLightConstants.ODataTypeAnnotationName);
                                    return typeProperty == null ? (JsonValue)(new JsonProperty("typeMissing", new JsonPrimitiveValue(null))) : typeProperty.RemoveAllAnnotations(true);
                                },
                                Json = tc.ExpectedJsonLight
                            };
                        }
                        else
                        {
                            this.Settings.Assert.Fail("Unsupported format '{0}'.", testConfiguration.Format);
                            return null;
                        }
                    })
                {
                    Model = tc.Model,
                    PayloadEdmElementContainer = tc.Model == null ? null : entitySet,
                    SkipTestConfiguration = testConfig => tc.Model == null && testConfig.Format == ODataFormat.Json,
                };
            });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyODataEdmPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Verifies the recursion depth limit restriction on entries.")]
        public void PropertyValueDepthLimitTest()
        {
            int depthLimit = 15;

            // A depth value which is greater tthan half of the depth limit, but still under it (for testing that depth counts aren't shared between sibling properties).
            int depthOverHalfButStillBelowLimit = 10;

            // Create entries with properties that nest complex values inside complex values.
            ODataResource overLimitEntry = ObjectModelUtils.CreateDefaultEntry();
            overLimitEntry.Properties = new ODataProperty[] { CreateDeeplyNestedComplexValues(depthLimit + 1, "TestModel.ComplexType", "PropertyName") };

            ODataResource atLimitEntry = ObjectModelUtils.CreateDefaultEntry();
            atLimitEntry.Properties = new ODataProperty[] { CreateDeeplyNestedComplexValues(depthLimit, "TestModel.ComplexType", "PropertyName") };

            // Create an entry where the total number of complex properties is greater than the depth limit, but no individual property exceeds the limit.
            ODataResource belowLimitForSinglePropertyEntry = ObjectModelUtils.CreateDefaultEntry();
            belowLimitForSinglePropertyEntry.Properties = new ODataProperty[]
            {
                CreateDeeplyNestedComplexValues(depthOverHalfButStillBelowLimit, "TestModel.ComplexType", "FirstPropertyName"),
                CreateDeeplyNestedComplexValues(depthOverHalfButStillBelowLimit, "TestModel.ComplexType", "SecondPropertyName")
            };

            // Create entries that nest complex values inside collections inside complex values (V3 only).
            ODataResource overLimitInCollectionEntry = ObjectModelUtils.CreateDefaultEntry();
            overLimitInCollectionEntry.Properties = new ODataProperty[] { CreateDeeplyNestedComplexValuesInCollections(depthLimit + 1, "TestModel.ComplexType", "PropertyName") };

            ODataResource atLimitInCollectionEntry = ObjectModelUtils.CreateDefaultEntry();
            atLimitInCollectionEntry.Properties = new ODataProperty[] { CreateDeeplyNestedComplexValuesInCollections(depthLimit, "TestModel.ComplexType", "PropertyName") };

            // An entry where the total number of complex properties is greater than the depth limit, but no individual property exceeds the limit.
            ODataResource belowLimitForSinglePropertyInCollectionEntry = ObjectModelUtils.CreateDefaultEntry();
            belowLimitForSinglePropertyInCollectionEntry.Properties = new ODataProperty[]
            {
                CreateDeeplyNestedComplexValuesInCollections(depthOverHalfButStillBelowLimit, "TestModel.ComplexType", "FirstPropertyName"),
                CreateDeeplyNestedComplexValuesInCollections(depthOverHalfButStillBelowLimit, "TestModel.ComplexType", "SecondPropertyName")
            };

            var testCases = new[]
                {
                    new
                    {
                        Item = overLimitEntry,
                        ShouldFail = true
                    },
                    new
                    {
                        Item = overLimitInCollectionEntry,
                        ShouldFail = true
                    },
                    new
                    {
                        Item = atLimitEntry,
                        ShouldFail = false
                    },
                    new
                    {
                        Item = belowLimitForSinglePropertyEntry,
                        ShouldFail = false
                    },
                    new
                    {
                        Item = atLimitInCollectionEntry,
                        ShouldFail = false
                    },
                    new
                    {
                        Item = belowLimitForSinglePropertyInCollectionEntry,
                        ShouldFail = false
                    },
                };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                this.WriterTestConfigurationProvider.AtomFormatConfigurations,
                (testCase, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);
                    testConfiguration.MessageWriterSettings.MessageQuotas.MaxNestingDepth = depthLimit;

                    using (var memoryStream = new TestStream())
                    using (var messageWriter = TestWriterUtils.CreateMessageWriter(memoryStream, testConfiguration, this.Assert))
                    {
                        ODataWriter writer = messageWriter.CreateODataWriter(isFeed: false);
                        if (testCase.ShouldFail)
                        {
                            TestExceptionUtils.ExpectedException(
                                this.Assert,
                                () => TestWriterUtils.WritePayload(messageWriter, writer, true, testCase.Item),
                                ODataExpectedExceptions.ODataException("ValidationUtils_RecursionDepthLimitReached", Convert.ToString(depthLimit)),
                                this.Settings.ExpectedResultSettings.ExceptionVerifier);
                        }
                        else
                        {
                            TestWriterUtils.WritePayload(messageWriter, writer, true, testCase.Item);
                        }
                    }
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Invalid entry payload.")]
        public void EntryErrorTests()
        {
            ODataResource emptyIdEntry = ObjectModelUtils.CreateDefaultEntry();
            emptyIdEntry.Id = null;

            ODataResource nullPropertyNameEntry = ObjectModelUtils.CreateDefaultEntry();
            nullPropertyNameEntry.Properties = new[]
                {
                    new ODataProperty()
                };

            ODataResource emptyPropertyNameEntry = ObjectModelUtils.CreateDefaultEntry();
            emptyPropertyNameEntry.Properties = new[]
                {
                    new ODataProperty() { Name = string.Empty }
                };

            // TODO, ckerer: follow up to see whether title has to be non-empty, is required at all on self/edit link, etc.

            ODataResource nestedCollectionEntry = ObjectModelUtils.CreateDefaultEntry();
            nestedCollectionEntry.Properties = new[]
                {
                    new ODataProperty()
                    {
                        Name = "FirstCollection",
                        Value = new ODataCollectionValue()
                        {
                            TypeName = EntityModelUtils.GetCollectionTypeName("My.AddressType"),
                            Items = new []
                            {
                                new ODataCollectionValue()
                                {
                                    TypeName = EntityModelUtils.GetCollectionTypeName("My.StreetType"),
                                    Items = null
                                }
                            }
                        }
                    }
                };

            var testCases = new[]
            {
                new
                {
                    Item = nullPropertyNameEntry,
                    ExpectedException = ODataExpectedExceptions.ODataException("WriterValidationUtils_PropertiesMustHaveNonEmptyName"),
                },
                new
                {
                    Item = emptyPropertyNameEntry,
                    ExpectedException = ODataExpectedExceptions.ODataException("WriterValidationUtils_PropertiesMustHaveNonEmptyName"),
                },
                new
                {
                    Item = nestedCollectionEntry,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_NestedCollectionsAreNotSupported"),
                },
            };

            // TODO: Fix places where we've lost JsonVerbose coverage to add JsonLight
            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurations.Where(tc => false),
                (testCase, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    using (var memoryStream = new TestStream())
                    using (var messageWriter = TestWriterUtils.CreateMessageWriter(memoryStream, testConfiguration, this.Assert))
                    {
                        ODataWriter writer = messageWriter.CreateODataWriter(isFeed: false);
                        TestExceptionUtils.ExpectedException(
                            this.Assert,
                            () => TestWriterUtils.WritePayload(messageWriter, writer, true, testCase.Item),
                            testCase.ExpectedException,
                            this.Settings.ExpectedResultSettings.ExceptionVerifier);
                    }
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Test writing entry payloads with user exceptions being thrown at various points.")]
        public void EntryUserExceptionTests()
        {
            ODataResourceSet defaultFeed = ObjectModelUtils.CreateDefaultFeed();
            ODataResource defaultEntry = ObjectModelUtils.CreateDefaultEntry();
            ODataNestedResourceInfo defaultCollectionLink = ObjectModelUtils.CreateDefaultCollectionLink();
            ODataNestedResourceInfo defaultSingletonLink = ObjectModelUtils.CreateDefaultSingletonLink();

            ODataItem[][] writerPayloads = new ODataItem[][]
            {
                new ODataItem[]
                {
                    defaultEntry,
                        defaultCollectionLink,
                            defaultFeed,
                                defaultEntry,
                                null,
                                defaultEntry,
                                null,
                                defaultEntry,
                                    defaultSingletonLink,
                                        defaultEntry,
                                            defaultCollectionLink,
                                                defaultFeed,
                                                    defaultEntry,
                                                    null,
                                                null,
                                            null,
                                        null,
                                    null,
                                null,
                            null,
                        null,
                },
                new ODataItem[]
                {
                    defaultEntry,
                        defaultSingletonLink,
                            defaultEntry,
                            null,
                        null,
                },
            };

            IEnumerable<PayloadWriterTestDescriptor<ODataItem>> testDescriptors = writerPayloads.Select(payload =>
                new PayloadWriterTestDescriptor<ODataItem>(
                    this.Settings,
                    payload,
                    tc => new WriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        ExpectedException2 = ODataExpectedExceptions.TestException(),
                    }
                ));

            // TODO: Fix places where we've lost JsonVerbose coverage to add JsonLight
            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors.PayloadCases(WriterPayloads.EntryPayloads),
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent.Where(tc => false),
                (testDescriptor, testConfiguration) =>
                {
                    foreach (int throwUserExceptionAt in Enumerable.Range(0, testDescriptor.PayloadItems.Count + 1))
                    {
                        var configuredTestDescriptor = new PayloadWriterTestDescriptor<ODataItem>(this.Settings, testDescriptor.PayloadItems, testDescriptor.ExpectedResultCallback)
                        {
                            ThrowUserExceptionAt = throwUserExceptionAt,
                        };

                        TestWriterUtils.WriteAndVerifyODataPayload(configuredTestDescriptor, testConfiguration, this.Assert, this.Logger);
                    }
                });
        }

        #region Entry with no properties

        private PayloadWriterTestDescriptor<ODataItem> CreateEntryWithNullPropertyListDescriptor()
        {
            ODataResource entryWithoutProperties = ObjectModelUtils.CreateDefaultEntry();
            entryWithoutProperties.TypeName = "My.EntryWithoutProperties";

            return new PayloadWriterTestDescriptor<ODataItem>(
                this.Settings,
                entryWithoutProperties,
                (testConfiguration) =>
                {
                    return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        Json = JsonUtils.WrapTopLevelValue(
                            testConfiguration,
                            JsonUtils.GetJsonLines(
                            "{" +
                                "\"__metadata\":{\"id\":\"http://www.odata.org/entryid\",\"uri\":\"http://www.odata.org/entry/readlink\",\"type\":\"My.EntryWithoutProperties\"}" +
                            "}")),
                        FragmentExtractor = (result) => result
                    };
                });
        }

        private PayloadWriterTestDescriptor<ODataItem> CreateEntryWithEmptyPropertyListDescriptor()
        {
            ODataResource noPropertiesEntry = ObjectModelUtils.CreateDefaultEntry();
            noPropertiesEntry.TypeName = "My.EntryWithoutProperties";
            noPropertiesEntry.Properties = Enumerable.Empty<ODataProperty>();

            return new PayloadWriterTestDescriptor<ODataItem>(
                this.Settings,
                noPropertiesEntry,
                (testConfiguration) =>
                {
                    return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        Json = JsonUtils.WrapTopLevelValue(
                            testConfiguration,
                            JsonUtils.GetJsonLines(
                            "{" +
                                "\"__metadata\":{\"id\":\"http://www.odata.org/entryid\",\"uri\":\"http://www.odata.org/entry/readlink\",\"type\":\"My.EntryWithoutProperties\"}" +
                            "}")),
                        FragmentExtractor = (result) => result
                    };
                });
        }

        #endregion

        #region Entry with primitive properties
        private PayloadWriterTestDescriptor<ODataItem> CreatePrimitivePropertiesEntryDescriptor(string odataNamespace)
        {
            ODataResource primitivePropertiesEntry = ObjectModelUtils.CreateDefaultEntry();
            primitivePropertiesEntry.TypeName = "My.EntryWithOpenProperties";
            primitivePropertiesEntry.Properties = ObjectModelUtils.CreateDefaultPrimitiveProperties();

            string primitivePropertiesJsonResult = string.Join(
                ",",
                "\"Null\":null",
                "\"Double\":1",
                "\"Binary\":\"AAEAAQ==\"",
                "\"Single\":1",
                "\"Boolean\":true",
                "\"Byte\":1",
                "\"DateTimeOffset1\":\"2010-10-10T10:10:10Z\"",
                "\"DateTimeOffset2\":\"2010-10-10T10:10:10+01:00\"",
                "\"DateTimeOffset3\":\"2010-10-10T10:10:10-08:00\"",
                "\"Decimal\":\"1\"",
                "\"Guid\":\"11111111-2222-3333-4444-555555555555\"",
                "\"SByte\":1",
                "\"Int16\":1",
                "\"Int32\":1",
                "\"Int64\":\"1\"",
                "\"String\":\"1\"",
                "\"Time\":\"PT12M20.4S\"",
                "\"Geography\":" + SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeographyValue, "Edm.GeographyPoint"),
                "\"GeographyPoint\":" + SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeographyPointValue, "Edm.GeographyPoint"),
                "\"GeographyLineString\":" + SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeographyLineStringValue, "Edm.GeographyLineString"),
                "\"GeographyPolygon\":" + SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeographyPolygonValue, "Edm.GeographyPolygon"),
                "\"GeographyCollection\":" + SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeographyCollectionValue, "Edm.GeographyCollection"),
                "\"GeographyMultiPoint\":" + SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeographyMultiPointValue, "Edm.GeographyMultiPoint"),
                "\"GeographyMultiLineString\":" + SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeographyMultiLineStringValue, "Edm.GeographyMultiLineString"),
                "\"GeographyMultiPolygon\":" + SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeographyMultiPolygonValue, "Edm.GeographyMultiPolygon"),
                "\"Geometry\":" + SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeometryValue, "Edm.GeometryPoint"),
                "\"GeometryPoint\":" + SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeometryPointValue, "Edm.GeometryPoint"),
                "\"GeometryLineString\":" + SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeometryLineStringValue, "Edm.GeometryLineString"),
                "\"GeometryPolygon\":" + SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeometryPolygonValue, "Edm.GeometryPolygon"),
                "\"GeometryCollection\":" + SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeometryCollectionValue, "Edm.GeometryCollection"),
                "\"GeometryMultiPoint\":" + SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeometryMultiPointValue, "Edm.GeometryMultiPoint"),
                "\"GeometryMultiLineString\":" + SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeometryMultiLineStringValue, "Edm.GeometryMultiLineString"),
                "\"GeometryMultiPolygon\":" + SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeometryMultiPolygonValue, "Edm.GeometryMultiPolygon"),

                "\"NullableDouble\":1",
                "\"NullableSingle\":1",
                "\"NullableBoolean\":true",
                "\"NullableByte\":1",
                "\"NullableDateTimeOffset1\":\"2010-10-10T10:10:10Z\"",
                "\"NullableDateTimeOffset2\":\"2010-10-10T10:10:10+01:00\"",
                "\"NullableDateTimeOffset3\":\"2010-10-10T10:10:10-08:00\"",
                "\"NullableDecimal\":\"1\"",
                "\"NullableGuid\":\"11111111-2222-3333-4444-555555555555\"",
                "\"NullableSByte\":1",
                "\"NullableInt16\":1",
                "\"NullableInt32\":1",
                "\"NullableInt64\":\"1\"",
                "\"NullableString\":\"1\"",
                "\"NullableDuration\":\"PT12M20.4S\"",

                "\"NullDouble\":null",
                "\"NullBinary\":null",
                "\"NullSingle\":null",
                "\"NullBoolean\":null",
                "\"NullByte\":null",
                "\"NullDateTimeOffset1\":null",
                "\"NullDateTimeOffset2\":null",
                "\"NullDateTimeOffset3\":null",
                "\"NullDecimal\":null",
                "\"NullGuid\":null",
                "\"NullSByte\":null",
                "\"NullInt16\":null",
                "\"NullInt32\":null",
                "\"NullInt64\":null",
                "\"NullString\":null",
                "\"NullDuration\":null",
                "\"NullGeography\":null",
                "\"NullGeographyPoint\":null",
                "\"NullGeographyLineString\":null",
                "\"NullGeographyPolygon\":null",
                "\"NullGeographyMultiPoint\":null",
                "\"NullGeographyMultiLineString\":null",
                "\"NullGeographyMultiPolygon\":null",
                "\"NullGeographyCollection\":null",
                "\"NullGeometry\":null",
                "\"NullGeometryPoint\":null",
                "\"NullGeometryLineString\":null",
                "\"NullGeometryPolygon\":null",
                "\"NullGeometryMultiPoint\":null",
                "\"NullGeometryMultiLineString\":null",
                "\"NullGeometryMultiPolygon\":null",
                "\"NullGeometryCollection\":null");

            var primitivePropertiesJsonResultLines = JsonUtils.GetJsonLines(
                "{" +
                    "\"__metadata\":{\"id\":\"http://www.odata.org/entryid\",\"uri\":\"http://www.odata.org/entry/readlink\",\"type\":\"My.EntryWithOpenProperties\"}," +
                    primitivePropertiesJsonResult +
                "}");

            return new PayloadWriterTestDescriptor<ODataItem>(
                this.Settings,
                primitivePropertiesEntry,
                (testConfiguration) =>
                {
                    return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        Json = JsonUtils.WrapTopLevelValue(testConfiguration, primitivePropertiesJsonResultLines),
                        FragmentExtractor = (result) => result
                    };
                });
        }
        #endregion Entry with primitive properties

        #region Entry with complex properties
        private PayloadWriterTestDescriptor<ODataItem> CreateComplexPropertiesEntryDescriptor(string odataNamespace)
        {
            ODataResource complexPropertiesEntry = ObjectModelUtils.CreateDefaultEntry();
            complexPropertiesEntry.TypeName = "My.EntryWithOpenProperties";

            List<ODataProperty> properties = new List<ODataProperty>();
            ODataProperty idProperty = new ODataProperty() { Name = "Id", Value = 1 };
            properties.Add(idProperty);

            //// modify the default complex properties to use a different namespace for the StreetType
            //ODataProperty[] complexProperties = ObjectModelUtils.CreateDefaultComplexProperties();
            //this.Assert.AreEqual(complexProperties[1].Name, "NestedComplex", "Expected a 'NestedComplex' property as third property of the type.");
            //ODataComplexValue complexValue = (ODataComplexValue)complexProperties[1].Value;
            //this.Assert.AreEqual(complexValue.Properties.First().Name, "Street", "Expected a 'Street' property as first property.");
            //ODataComplexValue streetValue = complexValue.Properties.First().Value as ODataComplexValue;
            //streetValue.TypeName = "OtherTestNamespace.StreetType";

            //properties.AddRange(complexProperties);
            complexPropertiesEntry.Properties = properties.ToArray();

            string complexPropertiesAtomResult = string.Join(
                "$(NL)",
                @"<{0}:{4} xmlns:{0}=""{1}"">",
                @"  <{2}:Id {0}:{5}=""Edm.Int32"" xmlns:{2}=""{3}"">1</{2}:Id>",
                @"  <{2}:ComplexAddress  {0}:{5}=""My.AddressType"" xmlns:{2}=""{3}"">",
                @"    <{2}:Street>One Redmond Way</{2}:Street>",
                @"    <{2}:City xml:space=""preserve""> Redmond</{2}:City>",
                @"  </{2}:ComplexAddress>",
                @"  <{2}:NestedComplex {0}:{5}=""My.NestedAddressType"" xmlns:{2}=""{3}"">",
                @"    <{2}:Street {0}:{5}=""OtherTestNamespace.StreetType"">",
                @"      <{2}:StreetName>One Redmond Way</{2}:StreetName>",
                @"      <{2}:Number {0}:{5}=""Edm.Int32"">1234</{2}:Number>",
                @"    </{2}:Street>",
                @"    <{2}:City xml:space=""preserve"">Redmond </{2}:City>",
                @"  </{2}:NestedComplex>",
                @"</{0}:{4}>");
            complexPropertiesAtomResult =
                string.Format(complexPropertiesAtomResult,
                    TestAtomConstants.ODataMetadataNamespacePrefix,
                    TestAtomConstants.ODataMetadataNamespace,
                    TestAtomConstants.ODataNamespacePrefix,
                    odataNamespace,
                    TestAtomConstants.AtomPropertiesElementName,
                    TestAtomConstants.AtomTypeAttributeName);

            string[] complexPropertiesJsonResult = StringUtils.Flatten(
                "{",
                "$(Indent)\"__metadata\":{",
                "$(Indent)$(Indent)\"id\":\"http://www.odata.org/entryid\",\"uri\":\"http://www.odata.org/entry/readlink\",\"type\":\"My.EntryWithOpenProperties\"",
                "$(Indent)},\"Id\":1,\"ComplexAddress\":{",
                JsonUtils.GetMetadataPropertyForComplexType("My.AddressType", 2, ",\"Street\":\"One Redmond Way\",\"City\":\" Redmond\""),
                "$(Indent)},\"NestedComplex\":{",
                JsonUtils.GetMetadataPropertyForComplexType("My.NestedAddressType", 2, ",\"Street\":{"),
                JsonUtils.GetMetadataPropertyForComplexType("OtherTestNamespace.StreetType", 3, ",\"StreetName\":\"One Redmond Way\",\"Number\":1234"),
                "$(Indent)$(Indent)},\"City\":\"Redmond \"",
                "$(Indent)}",
                "}");

            return new PayloadWriterTestDescriptor<ODataItem>(
                this.Settings,
                complexPropertiesEntry,
                (testConfiguration) =>
                {
                    return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        Json = JsonUtils.WrapTopLevelValue(testConfiguration, complexPropertiesJsonResult),
                        FragmentExtractor = (result) => result
                    };
                });
        }
        #endregion Entry with complex properties

        #region Entry with collection properties
        private PayloadWriterTestDescriptor<ODataItem> CreateCollectionPropertiesEntryDescriptor(bool withModel, string odataNamespace, bool addCollectionWithoutTypeName = false)
        {
            ODataResource collectionPropertiesEntry = ObjectModelUtils.CreateDefaultEntry();
            collectionPropertiesEntry.TypeName = "My.EntryWithOpenProperties";

            List<ODataProperty> properties = new List<ODataProperty>();
            ODataProperty idProperty = new ODataProperty() { Name = "Id", Value = 1 };
            properties.Add(idProperty);
            properties.AddRange(ObjectModelUtils.CreateDefaultCollectionProperties());

            string collectionWithoutTypeNameAtomPayload = String.Empty;
            string[] collectionWithoutTypeNameJsonPayload = new string[0];
            if (addCollectionWithoutTypeName)
            {
                ODataProperty complexCollectionWithoutTypeName = new ODataProperty()
                {
                    Name = "ComplexCollectionWithoutTypeName",
                    Value = new ODataCollectionValue()
                    {
                        Items = new[]
                        {
                            new ODataComplexValue()
                            {
                                TypeName = "My.AddressType",
                                Properties = new []
                                {
                                    new ODataProperty() { Name = "Street", Value = "One Redmond Way" },
                                    new ODataProperty() { Name = "City", Value = " Redmond" },
                                }
                            },
                            new ODataComplexValue()
                            {
                                TypeName = "My.AddressType",
                                Properties = new []
                                {
                                    new ODataProperty() { Name = "Street", Value = "Am Euro Platz 3" },
                                    new ODataProperty() { Name = "City", Value = "Vienna " },
                                }
                            },
                            new ODataComplexValue()
                            {
                                TypeName = "My.AddressType",
                                Properties = new []
                                {
                                    new ODataProperty() { Name = "City", Value = "Sammamish" },
                                }
                            },
                        }
                    }
                };

                properties.Add(complexCollectionWithoutTypeName);

                collectionWithoutTypeNameAtomPayload = string.Join(
                    "$(NL)",
                    @"  <{2}:ComplexCollectionWithoutTypeName xmlns:{2}=""{3}"">",
                    @"    <{0}:{5} {0}:{6}=""My.AddressType"">",
                    @"      <{2}:Street>One Redmond Way</{2}:Street>",
                    @"      <{2}:City xml:space=""preserve""> Redmond</{2}:City>",
                    @"    </{0}:{5}>",
                    @"    <{0}:{5} {0}:{6}=""My.AddressType"">",
                    @"      <{2}:Street>Am Euro Platz 3</{2}:Street>",
                    @"      <{2}:City xml:space=""preserve"">Vienna </{2}:City>",
                    @"    </{0}:{5}>",
                    @"    <{0}:{5} {0}:{6}=""My.AddressType"">",
                    @"      <{2}:City>Sammamish</{2}:City>",
                    @"    </{0}:{5}>",
                    @"  </{2}:ComplexCollectionWithoutTypeName>");

                collectionWithoutTypeNameJsonPayload = StringUtils.Flatten(
                    "$(Indent)},\"ComplexCollectionWithoutTypeName\":{",
                    "$(Indent)$(Indent)\"results\":[",
                    "$(Indent)$(Indent)$(Indent){",
                    JsonUtils.GetMetadataPropertyForComplexType("My.AddressType", 4, ",\"Street\":\"One Redmond Way\",\"City\":\" Redmond\""),
                    "$(Indent)$(Indent)$(Indent)},{",
                    JsonUtils.GetMetadataPropertyForComplexType("My.AddressType", 4, ",\"Street\":\"Am Euro Platz 3\",\"City\":\"Vienna \""),
                    "$(Indent)$(Indent)$(Indent)},{",
                    JsonUtils.GetMetadataPropertyForComplexType("My.AddressType", 4, ",\"City\":\"Sammamish\""),
                    "$(Indent)$(Indent)$(Indent)}",
                    "$(Indent)$(Indent)]");
            }

            string[] collectionPropertiesJsonResult = StringUtils.Flatten(
                "{",
                "$(Indent)\"__metadata\":{",
                "$(Indent)$(Indent)\"id\":\"http://www.odata.org/entryid\",\"uri\":\"http://www.odata.org/entry/readlink\",\"type\":\"My.EntryWithOpenProperties\"",
                "$(Indent)},\"Id\":1,\"EmptyCollection\":{",
                JsonUtils.GetMetadataPropertyForCollectionType(EntityModelUtils.GetCollectionTypeName("Edm.String"), 2, ",\"results\":["),
                "$(Indent)$(Indent)$(Indent)",
                "$(Indent)$(Indent)]",
                "$(Indent)},\"PrimitiveCollection\":{",
                JsonUtils.GetMetadataPropertyForCollectionType(EntityModelUtils.GetCollectionTypeName("Edm.Int32"), 2, ",\"results\":["),
                "$(Indent)$(Indent)$(Indent)0,1,2,3,4,5,6,7,8,9",
                "$(Indent)$(Indent)]",
                "$(Indent)},\"IntCollectionNoTypeName\":{",
                withModel ? JsonUtils.GetMetadataPropertyForCollectionType(EntityModelUtils.GetCollectionTypeName("Edm.Int32"), 2, ",\"results\":[") : new string[] { "$(Indent)$(Indent)\"results\":[" },
                "$(Indent)$(Indent)$(Indent)0,1,2",
                "$(Indent)$(Indent)]",
                "$(Indent)},\"StringCollectionNoTypeName\":{",
                withModel ? JsonUtils.GetMetadataPropertyForCollectionType(EntityModelUtils.GetCollectionTypeName("Edm.String"), 2, ",\"results\":[") : new string[] { "$(Indent)$(Indent)\"results\":[" },
                "$(Indent)$(Indent)$(Indent)\"One\",\"Two\",\"Three\"",
                "$(Indent)$(Indent)]",
                "$(Indent)},\"GeographyCollectionNoTypeName\":{",
                withModel ? JsonUtils.GetMetadataPropertyForCollectionType(EntityModelUtils.GetCollectionTypeName("Edm.Geography"), 2, ",\"results\":[") : new string[] { "$(Indent)$(Indent)\"results\":[" },
                JsonUtils.GetJsonLines(
                    "$(Indent)$(Indent)$(Indent)" +
                    SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeographyCollectionValue, "Edm.GeographyCollection") + "," +
                    SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeographyLineStringValue, "Edm.GeographyLineString") + "," +
                    SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeographyMultiLineStringValue, "Edm.GeographyMultiLineString") + "," +
                    SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeographyMultiPointValue, "Edm.GeographyMultiPoint") + "," +
                    SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeographyMultiPolygonValue, "Edm.GeographyMultiPolygon") + "," +
                    SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeographyPointValue, "Edm.GeographyPoint") + "," +
                    SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeographyPolygonValue, "Edm.GeographyPolygon") + "," +
                    SpatialUtils.GetSpatialStringValue(ODataFormat.Json, ObjectModelUtils.GeographyValue, "Edm.GeographyPoint"), indentDepth: 3),
                "$(Indent)$(Indent)]",
                "$(Indent)},\"ComplexCollection\":{",
                JsonUtils.GetMetadataPropertyForCollectionType(EntityModelUtils.GetCollectionTypeName("My.AddressType"), 2, ",\"results\":["),
                "$(Indent)$(Indent)$(Indent){",
                "$(Indent)$(Indent)$(Indent)$(Indent)\"Street\":\"One Redmond Way\",\"City\":\" Redmond\"",
                "$(Indent)$(Indent)$(Indent)},{",
                "$(Indent)$(Indent)$(Indent)$(Indent)\"Street\":\"Am Euro Platz 3\",\"City\":\"Vienna \"",
                "$(Indent)$(Indent)$(Indent)}",
                "$(Indent)$(Indent)]",
                collectionWithoutTypeNameJsonPayload,
                "$(Indent)}",
                    "}"
            );

            collectionPropertiesEntry.Properties = properties.ToArray();
            return new PayloadWriterTestDescriptor<ODataItem>(
                this.Settings,
                collectionPropertiesEntry,
                (testConfiguration) =>
                {
                    return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        Json = JsonUtils.WrapTopLevelValue(testConfiguration, collectionPropertiesJsonResult),
                        FragmentExtractor = (result) => result
                    };
                });
        }
        #endregion Entry with collection properties

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Test entry with self and/or edit link.")]
        public void EntryPropertyTests()
        {
            // add entity/complex types to the container namespace of the metadata provider as well as another namespace. Adding types with the same name
            // to multiple namespaces ensures we do not rely on the local (non-namespace qualified) name of the type.
            EdmModel model = new EdmModel();
            string[] testNamespaces = new string[] { "My", "OtherTestNamespace" };

            var addressType = new EdmComplexType("My", "AddressType");
            addressType.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String, true);
            addressType.AddStructuralProperty("City", EdmPrimitiveTypeKind.String, true);

            // Entity type with primitive properties of all supported types
            foreach (string namespaceName in testNamespaces)
            {
                // Here's the full list of primitive properties defined in ObjectModelUtils.CreateDefaultPrimitiveProperties():
                // Null value for supported primitive types are already covered below, let the property 'Null' be an untyped open property here.
                var entryWithOpenProperties = new EdmEntityType(namespaceName, "EntryWithOpenProperties", null, isAbstract: false, isOpen: true);
                entryWithOpenProperties.AddKeys(entryWithOpenProperties.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32, isNullable: false));
                entryWithOpenProperties.AddStructuralProperty("Double", EdmPrimitiveTypeKind.Double, isNullable: false);
                entryWithOpenProperties.AddStructuralProperty("Binary", EdmPrimitiveTypeKind.Binary, isNullable: true);
                entryWithOpenProperties.AddStructuralProperty("Single", EdmPrimitiveTypeKind.Single, isNullable: false);
                entryWithOpenProperties.AddStructuralProperty("Boolean", EdmPrimitiveTypeKind.Boolean, isNullable: false);
                entryWithOpenProperties.AddStructuralProperty("Byte", EdmPrimitiveTypeKind.Byte, isNullable: false);
                entryWithOpenProperties.AddStructuralProperty("DateTimeOffset", EdmPrimitiveTypeKind.DateTimeOffset, isNullable: false);
                entryWithOpenProperties.AddStructuralProperty("Decimal", EdmPrimitiveTypeKind.Decimal, isNullable: false);
                entryWithOpenProperties.AddStructuralProperty("Guid", EdmPrimitiveTypeKind.Guid, isNullable: false);
                entryWithOpenProperties.AddStructuralProperty("SByte", EdmPrimitiveTypeKind.SByte, isNullable: false);
                entryWithOpenProperties.AddStructuralProperty("Int16", EdmPrimitiveTypeKind.Int16, isNullable: false);
                entryWithOpenProperties.AddStructuralProperty("Int64", EdmPrimitiveTypeKind.Int64, isNullable: false);
                entryWithOpenProperties.AddStructuralProperty("String", EdmPrimitiveTypeKind.String, isNullable: false);
                entryWithOpenProperties.AddStructuralProperty("Stream", EdmPrimitiveTypeKind.Stream, isNullable: false);
                entryWithOpenProperties.AddStructuralProperty("Duration", EdmPrimitiveTypeKind.Duration, isNullable: false);
                entryWithOpenProperties.AddStructuralProperty("Geography", EdmPrimitiveTypeKind.Geography, isNullable: true);
                entryWithOpenProperties.AddStructuralProperty("GeographyPoint", EdmPrimitiveTypeKind.GeographyPoint, isNullable: true);
                entryWithOpenProperties.AddStructuralProperty("GeographyLineString", EdmPrimitiveTypeKind.GeographyLineString, isNullable: true);
                entryWithOpenProperties.AddStructuralProperty("GeographyPolygon", EdmPrimitiveTypeKind.GeographyPolygon, isNullable: true);
                entryWithOpenProperties.AddStructuralProperty("GeographyCollection", EdmPrimitiveTypeKind.GeographyCollection, isNullable: true);
                entryWithOpenProperties.AddStructuralProperty("GeographyMultiPoint", EdmPrimitiveTypeKind.GeographyMultiPoint, isNullable: true);
                entryWithOpenProperties.AddStructuralProperty("GeographyMultiLineString", EdmPrimitiveTypeKind.GeographyMultiLineString, isNullable: true);
                entryWithOpenProperties.AddStructuralProperty("GeographyMultiPolygon", EdmPrimitiveTypeKind.GeographyMultiPolygon, isNullable: true);
                entryWithOpenProperties.AddStructuralProperty("Geometry", EdmPrimitiveTypeKind.Geometry, isNullable: true);
                entryWithOpenProperties.AddStructuralProperty("GeometryPoint", EdmPrimitiveTypeKind.GeometryPoint, isNullable: true);
                entryWithOpenProperties.AddStructuralProperty("GeometryLineString", EdmPrimitiveTypeKind.GeometryLineString, isNullable: true);
                entryWithOpenProperties.AddStructuralProperty("GeometryPolygon", EdmPrimitiveTypeKind.GeometryPolygon, isNullable: true);
                entryWithOpenProperties.AddStructuralProperty("GeometryCollection", EdmPrimitiveTypeKind.GeometryCollection, isNullable: true);
                entryWithOpenProperties.AddStructuralProperty("GeometryMultiPoint", EdmPrimitiveTypeKind.GeometryMultiPoint, isNullable: true);
                entryWithOpenProperties.AddStructuralProperty("GeometryMultiLineString", EdmPrimitiveTypeKind.GeometryMultiLineString, isNullable: true);
                entryWithOpenProperties.AddStructuralProperty("GeometryMultiPolygon", EdmPrimitiveTypeKind.GeometryMultiPolygon, isNullable: true);

                entryWithOpenProperties.AddStructuralProperty("NullableBoolean", EdmPrimitiveTypeKind.Boolean, isNullable: true);
                entryWithOpenProperties.AddStructuralProperty("NullableDateTimeOffset", EdmPrimitiveTypeKind.DateTimeOffset, isNullable: true);
                entryWithOpenProperties.AddStructuralProperty("NullableDecimal", EdmPrimitiveTypeKind.Decimal, isNullable: true);
                entryWithOpenProperties.AddStructuralProperty("NullableGuid", EdmPrimitiveTypeKind.Guid, isNullable: true);
                entryWithOpenProperties.AddStructuralProperty("NullableSByte", EdmPrimitiveTypeKind.SByte, isNullable: true);
                entryWithOpenProperties.AddStructuralProperty("NullableInt16", EdmPrimitiveTypeKind.Int16, isNullable: true);
                entryWithOpenProperties.AddStructuralProperty("NullableInt32", EdmPrimitiveTypeKind.Int32, isNullable: true);
                entryWithOpenProperties.AddStructuralProperty("NullableInt64", EdmPrimitiveTypeKind.Int64, isNullable: true);
                entryWithOpenProperties.AddStructuralProperty("NullableString", EdmPrimitiveTypeKind.String, isNullable: true);
                entryWithOpenProperties.AddStructuralProperty("NullableDuration", EdmPrimitiveTypeKind.Duration, isNullable: true);
                entryWithOpenProperties.AddStructuralProperty("EmptyCollection", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(isNullable: true)));
                entryWithOpenProperties.AddStructuralProperty("PrimitiveCollection", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(isNullable: false)));
                entryWithOpenProperties.AddStructuralProperty("IntCollectionNoTypeName", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(isNullable: false)));
                entryWithOpenProperties.AddStructuralProperty("StringCollectionNoTypeName", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(isNullable: true)));
                entryWithOpenProperties.AddStructuralProperty("GeographyCollectionNoTypeName", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.Geography, isNullable: true)));
                entryWithOpenProperties.AddStructuralProperty("ComplexCollection", EdmCoreModel.GetCollection(new EdmComplexTypeReference(addressType, isNullable: false)));

                model.AddElement(entryWithOpenProperties);
            }

            var entryWithoutProperties = new EdmEntityType("My", "EntryWithoutProperties");
            model.AddElement(entryWithoutProperties);

            model.AddElement(addressType);

            addressType = new EdmComplexType("OtherTestNamespace", "AddressType");
            model.AddElement(addressType);

            var emptyType = new EdmComplexType("My", "EmptyType");
            model.AddElement(emptyType);

            emptyType = new EdmComplexType("OtherTestNamespace", "EmptyType");
            model.AddElement(emptyType);

            var streetType = new EdmComplexType("My", "StreetType");
            model.AddElement(streetType);

            streetType = new EdmComplexType("OtherTestNamespace", "StreetType");
            streetType.AddStructuralProperty("StreetName", EdmPrimitiveTypeKind.String);
            streetType.AddStructuralProperty("Number", EdmPrimitiveTypeKind.Int32, isNullable: false);
            model.AddElement(streetType);

            var nestedAddressTypeType = new EdmComplexType("My", "NestedAddressType");
            nestedAddressTypeType.AddStructuralProperty("Street", new EdmComplexTypeReference(streetType, isNullable: false));
            nestedAddressTypeType.AddStructuralProperty("City", EdmPrimitiveTypeKind.String);
            model.AddElement(nestedAddressTypeType);

            var container = new EdmEntityContainer("My", "TestContainer");
            model.AddElement(container);

            var testCases = new[]
            {
                this.CreateEntryWithNullPropertyListDescriptor(),
                this.CreateEntryWithEmptyPropertyListDescriptor(),
                this.CreatePrimitivePropertiesEntryDescriptor(TestAtomConstants.ODataNamespace),
                this.CreateComplexPropertiesEntryDescriptor(TestAtomConstants.ODataNamespace),
                this.CreateCollectionPropertiesEntryDescriptor(/*withModel*/ true, TestAtomConstants.ODataNamespace)
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                this.WriterTestConfigurationProvider.AtomFormatConfigurationsWithIndent,
                (testCase, testConfig) =>
                {
                    testCase.Model = model;

                    testConfig = testConfig.Clone();
                    testConfig.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyODataPayload(testCase, testConfig, this.Assert, this.Logger);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Test entry with self and/or edit link without metadata.")]
        public void EntryPropertyTestsWithoutMetadata()
        {
            this.CombinatorialEngineProvider.RunCombinations(
                this.WriterTestConfigurationProvider.AtomFormatConfigurationsWithIndent,
                (testConfig) =>
                {
                    testConfig = testConfig.Clone();
                    testConfig.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    var testCases = new[]
                    {
                        this.CreateEntryWithNullPropertyListDescriptor(),
                        this.CreateEntryWithEmptyPropertyListDescriptor(),
                        this.CreatePrimitivePropertiesEntryDescriptor(TestAtomConstants.ODataNamespace),
                        this.CreateComplexPropertiesEntryDescriptor(TestAtomConstants.ODataNamespace),
                        this.CreateCollectionPropertiesEntryDescriptor(/*withModel*/ false, TestAtomConstants.ODataNamespace, /*addCollectionWithoutTypeName*/ true)
                    };

                    foreach (var testCase in testCases)
                    {
                        TestWriterUtils.WriteAndVerifyODataPayload(testCase, testConfig, this.Assert, this.Logger);
                    }
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Writing null to proprties which cannot be null should fail.")]
        public void EntryNullPropertyErrorTests()
        {
            EdmModel model = new EdmModel();

            var complex = new EdmComplexType("My", "Complex");
            complex.AddStructuralProperty("Number", EdmPrimitiveTypeKind.Int32, isNullable: true);
            complex.AddStructuralProperty("String", EdmPrimitiveTypeKind.String, isNullable: true);
            complex.AddStructuralProperty("InnerComplex", new EdmComplexTypeReference(complex, isNullable: false));
            model.AddElement(complex);

            var entryWithNullProperties = new EdmEntityType("My", "EntryWithNullProperties");
            entryWithNullProperties.AddKeys(entryWithNullProperties.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32, isNullable: false));
            entryWithNullProperties.AddStructuralProperty("Double", EdmPrimitiveTypeKind.Double, isNullable: false);
            entryWithNullProperties.AddStructuralProperty("Binary", EdmPrimitiveTypeKind.Binary, isNullable: false);
            entryWithNullProperties.AddStructuralProperty("Single", EdmPrimitiveTypeKind.Single, isNullable: false);
            entryWithNullProperties.AddStructuralProperty("Boolean", EdmPrimitiveTypeKind.Boolean, isNullable: false);
            entryWithNullProperties.AddStructuralProperty("Byte", EdmPrimitiveTypeKind.Byte, isNullable: false);
            entryWithNullProperties.AddStructuralProperty("Decimal", EdmPrimitiveTypeKind.Decimal, isNullable: false);
            entryWithNullProperties.AddStructuralProperty("Guid", EdmPrimitiveTypeKind.Guid, isNullable: false);
            entryWithNullProperties.AddStructuralProperty("SByte", EdmPrimitiveTypeKind.SByte, isNullable: false);
            entryWithNullProperties.AddStructuralProperty("Int16", EdmPrimitiveTypeKind.Int16, isNullable: false);
            entryWithNullProperties.AddStructuralProperty("Int32", EdmPrimitiveTypeKind.Int32, isNullable: false);
            entryWithNullProperties.AddStructuralProperty("Int64", EdmPrimitiveTypeKind.Int64, isNullable: false);
            entryWithNullProperties.AddStructuralProperty("Collection", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(isNullable: false)));
            entryWithNullProperties.AddStructuralProperty("NamedStream", EdmPrimitiveTypeKind.Stream, isNullable: false);
            entryWithNullProperties.AddStructuralProperty("Complex", new EdmComplexTypeReference(complex, isNullable: false));
            model.AddElement(entryWithNullProperties);

            model.AddElement(new EdmEntityType("OtherTestNamespace", "EntryWithNullProperties"));

            var container = new EdmEntityContainer("My", "TestContainer");
            model.AddElement(container);

            string[] propertyNames = new string[]
            {
                "Double",
                "Single",
                "Boolean",
                "Byte",
                "Decimal",
                "Guid",
                "SByte",
                "Int16",
                "Int32",
                "Int64",
                "Collection",
                "NamedStream",
                "Complex",
            };

            var versions = new Version[] {
                    null,
                    new Version(4, 0),
                };

            this.CombinatorialEngineProvider.RunCombinations(
                propertyNames,
                versions,
                versions,
                TestWriterUtils.ODataBehaviorKinds,
                this.WriterTestConfigurationProvider.AtomFormatConfigurationsWithIndent,
                (propertyName, dataServiceVersion, edmVersion, behaviorKind, testConfig) =>
                {
                    ODataResource primitivePropertiesEntry = ObjectModelUtils.CreateDefaultEntry();
                    primitivePropertiesEntry.TypeName = "My.EntryWithNullProperties";
                    primitivePropertiesEntry.Properties = new ODataProperty[] { new ODataProperty { Name = propertyName, Value = null } };

                    string errorResourceKey;
                    var messageParameters = new List<string> { propertyName };

                    if (propertyName == "Collection")
                    {
                        errorResourceKey = "WriterValidationUtils_CollectionPropertiesMustNotHaveNullValue";
                    }
                    else if (propertyName == "NamedStream")
                    {
                        errorResourceKey = "WriterValidationUtils_StreamPropertiesMustNotHaveNullValue";
                    }
                    else if (propertyName == "Complex")
                    {
                        errorResourceKey = "WriterValidationUtils_NonNullablePropertiesMustNotHaveNullValue";
                        messageParameters.Add("My." + propertyName);
                    }
                    else
                    {
                        errorResourceKey = "WriterValidationUtils_NonNullablePropertiesMustNotHaveNullValue";
                        messageParameters.Add("Edm." + propertyName);
                    }

                    PayloadWriterTestDescriptor<ODataItem> descriptor = new PayloadWriterTestDescriptor<ODataItem>(this.Settings, primitivePropertiesEntry)
                    {
                        ExpectedResultCallback = (PayloadWriterTestDescriptor<ODataItem>.WriterTestExpectedResultCallback)(
                           (testConfiguration) =>
                           {
                               // When AllowNullValuesForNonNullablePrimitiveTypes flag is set (happens only for the WCF DS server), primitive null value validation is disabled.
                               if (behaviorKind == TestODataBehaviorKind.WcfDataServicesServer
                                   && new[] { "Collection", "Complex", "NamedStream" }.Where(v => v != propertyName).Count() > 0)
                               {
                                   return null;
                               }

                               return new WriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                               {
                                   ExpectedException2 = ODataExpectedExceptions.ODataException(errorResourceKey, messageParameters.ToArray()),
                               };
                           }),
                        Model = model,
                    };

                    TestWriterUtils.WriteAndVerifyODataPayload(descriptor, testConfig.CloneAndApplyBehavior(behaviorKind), this.Assert, this.Logger);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Test type inference from metadata.")]
        public void InferredTypeNamesTests()
        {
            EdmModel model = new EdmModel();
            var addressTypeNS = new EdmComplexType("TestNS", "AddressComplexType");
            addressTypeNS.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            model.AddElement(addressTypeNS);

            var addressTypeOther = new EdmComplexType("OtherTestNamespace", "AddressComplexType");
            model.AddElement(addressTypeOther);

            var orderTypeNS = new EdmComplexType("TestNS", "OrderComplexType");
            model.AddElement(orderTypeNS);

            var orderTypeOther = new EdmComplexType("OtherTestNamespace", "OrderComplexType");
            model.AddElement(orderTypeOther);

            var entityTypeNS = new EdmEntityType("TestNS", "EntityType");
            entityTypeNS.AddKeys(entityTypeNS.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(isNullable: false)));
            entityTypeNS.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            entityTypeNS.AddStructuralProperty("Scores", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(isNullable: true)));
            entityTypeNS.AddStructuralProperty("Address", new EdmComplexTypeReference(addressTypeNS, isNullable: false));
            model.AddElement(entityTypeNS);

            var entityTypeOther = new EdmEntityType("OtherTestNamespace", "EntityType");
            entityTypeOther.AddKeys(entityTypeOther.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(isNullable: false)));
            entityTypeOther.AddStructuralProperty("Age", EdmCoreModel.Instance.GetInt32(isNullable: false));
            model.AddElement(entityTypeOther);

            var container = new EdmEntityContainer("TestNS", "TestContainer");
            model.AddElement(container);

            ODataResource entry = ObjectModelUtils.CreateDefaultEntryWithAtomMetadata();
            entry.TypeName = "TestNS.EntityType";
            entry.Properties = new ODataProperty[]
            {
                new ODataProperty() { Name = "Id", Value = 1 },
                new ODataProperty() { Name = "Name", Value = "Bill" },
                new ODataProperty()
                {
                    Name = "Address",
                    Value = new ODataComplexValue()
                    {
                        Properties = new []
                        {
                            new ODataProperty() { Name = "Name", Value = "" }
                        }
                    }
                },
                new ODataProperty()
                {
                    Name = "Scores",
                    Value = new ODataCollectionValue()
                    {
                        Items = null
                    }
                },
            };

            var testCases = new[]
                {
                    new
                    {
                        ExpectedResults = (PayloadWriterTestDescriptor<ODataItem>.WriterTestExpectedResultCallback)(
                            (testConfiguration) =>
                            {

                                    return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                                    {
                                        Json = "$(JsonDataWrapperIndent)$(Indent)\"Id\":1",
                                        FragmentExtractor = (result) => JsonUtils.UnwrapTopLevelValue(testConfiguration, result).Object().Property("Id")
                                    };

                            })
                    },
                    new
                    {
                        ExpectedResults = (PayloadWriterTestDescriptor<ODataItem>.WriterTestExpectedResultCallback)(
                            (testConfiguration) =>
                            {

                                    return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                                    {
                                        Json = "$(JsonDataWrapperIndent)$(Indent)\"Name\":\"Bill\"",
                                        FragmentExtractor = (result) => JsonUtils.UnwrapTopLevelValue(testConfiguration, result).Object().Property("Name")
                                    };

                            })
                    },
                    new
                    {
                        ExpectedResults = (PayloadWriterTestDescriptor<ODataItem>.WriterTestExpectedResultCallback)(
                            (testConfiguration) =>
                            {

                                    string json = string.Join(
                                        "$(NL)",
                                        "$(JsonDataWrapperIndent)$(Indent)\"Address\":{",
                                        "$(JsonDataWrapperIndent)$(Indent)$(Indent)\"__metadata\":{",
                                        "$(JsonDataWrapperIndent)$(Indent)$(Indent)$(Indent)\"type\":\"TestNS.AddressComplexType\"",
                                        "$(JsonDataWrapperIndent)$(Indent)$(Indent)},\"Name\":\"\"",
                                        "$(JsonDataWrapperIndent)$(Indent)}");
                                    return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                                    {
                                        Json = json,
                                        FragmentExtractor = (result) => JsonUtils.UnwrapTopLevelValue(testConfiguration, result).Object().Property("Address")
                                    };

                            })
                    },
                    new
                    {
                        ExpectedResults = (PayloadWriterTestDescriptor<ODataItem>.WriterTestExpectedResultCallback)(
                            (testConfiguration) =>
                            {

                                    string json = string.Join(
                                        "$(NL)",
                                        "$(JsonDataWrapperIndent)$(Indent)\"Scores\":{",
                                        "$(JsonDataWrapperIndent)$(Indent)$(Indent)\"__metadata\":{",
                                        "$(JsonDataWrapperIndent)$(Indent)$(Indent)$(Indent)\"type\":\"" + EntityModelUtils.GetCollectionTypeName("Edm.String") + "\"",
                                        "$(JsonDataWrapperIndent)$(Indent)$(Indent)},\"results\":[",
                                        "$(JsonDataWrapperIndent)$(Indent)$(Indent)$(Indent)",
                                        "$(JsonDataWrapperIndent)$(Indent)$(Indent)]",
                                        "$(JsonDataWrapperIndent)$(Indent)}",
                                        string.Empty);  // TODO: unclear why I need the extra $(NL) in this case.
                                    return new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                                    {
                                        Json = json,
                                        FragmentExtractor = (result) => JsonUtils.UnwrapTopLevelValue(testConfiguration, result).Object().Property("Scores")
                                    };

                            })
                    },
                };


            var testDescriptors = testCases.Select(tc =>
            {
                var descriptor = new PayloadWriterTestDescriptor<ODataItem>(this.Settings, entry, tc.ExpectedResults);
                descriptor.Model = model;
                return descriptor;
            });

            // TODO: Fix places where we've lost JsonVerbose coverage to add JsonLight
            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent.Where(tc => false),
                (testDescriptor, testConfig) =>
                {
                    WriterTestConfiguration newConfiguration = testConfig.Clone();
                    newConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);
                    TestWriterUtils.WriteAndVerifyODataPayload(testDescriptor, newConfiguration, this.Assert, this.Logger);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Verifies correct serialization of empty complex values in ATOM.")]
        public void EmptyComplexValueTest()
        {
            ODataResourceSerializationInfo info = new ODataResourceSerializationInfo()
            {
                NavigationSourceEntityTypeName = "Null",
                NavigationSourceName = "MySet",
                ExpectedTypeName = "Null"
            };

            var testDescriptors = new[]
            {
                // Empty entry level complex value -> no property written and no m:properties included either
                new PayloadWriterTestDescriptor<ODataItem>(
                    this.Settings,
                    new ODataResource() {
                        Properties = new [] { new ODataProperty { Name = "EmptyComplex", Value =
                            new ODataComplexValue { Properties = null }
                        } },
                        SerializationInfo = info
                    },
                    (tc) => new AtomWriterTestExpectedResults(this.Settings.ExpectedResultSettings) {
                        Xml = "<root/>",
                        FragmentExtractor = (result) => new XElement("root", result
                            .Element(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomContentElementName)
                            .Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.AtomPropertiesElementName))
                    }),
                // Empty second level complex value -> no property written and no m:properties included either
                new PayloadWriterTestDescriptor<ODataItem>(
                    this.Settings,
                    new ODataResource() { Properties = new [] { new ODataProperty { Name = "OuterComplex", Value =
                        new ODataComplexValue { Properties = new [] { new ODataProperty { Name = "InnerComplex", Value =
                            new ODataComplexValue { Properties = null }
                        } } }
                    } },
                    SerializationInfo = info},
                    (tc) => new AtomWriterTestExpectedResults(this.Settings.ExpectedResultSettings) {
                        Xml = "<root/>",
                        FragmentExtractor = (result) => new XElement("root", result
                            .Element(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomContentElementName)
                            .Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.AtomPropertiesElementName))
                    }),
                // Empty second level complex value in collection
                new PayloadWriterTestDescriptor<ODataItem>(
                    this.Settings,
                    new ODataResource() { Properties = new [] { new ODataProperty { Name = "ComplexCollection", Value = new ODataCollectionValue { Items = new [] {
                        new ODataComplexValue { Properties = new [] { new ODataProperty { Name = "InnerComplex", Value =
                            new ODataComplexValue { Properties = null }
                        } } }
                    } } } },
                    SerializationInfo = info},
                    (tc) => new AtomWriterTestExpectedResults(this.Settings.ExpectedResultSettings) {
                        Xml = string.Format("<{0}:ComplexCollection xmlns:{0}='{1}' xmlns:{2}='{3}'><{2}:element/></{0}:ComplexCollection>",
                            TestAtomConstants.ODataNamespacePrefix,
                            TestAtomConstants.ODataNamespace,
                            TestAtomConstants.ODataMetadataNamespacePrefix,
                            TestAtomConstants.ODataMetadataNamespace),
                        FragmentExtractor = (result) => TestAtomUtils.ExtractPropertiesFromEntry(result)
                            .Element(TestAtomConstants.ODataXNamespace + "ComplexCollection")
                    })
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.AtomFormatConfigurationsWithIndent,
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyODataPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Validates that if metadata is specified, null properties have the m:type attribute in ATOM in WCF DS compat scenarios.")]
        public void NullPropertyTypeNameTest()
        {
            Version[] versions = new Version[] {
                null,
                new Version(4, 0),
            };

            var innerComplexType = new EdmComplexType("TestNS", "ComplexType");
            innerComplexType.AddStructuralProperty("NumberProperty", EdmCoreModel.Instance.GetInt32(true));
            innerComplexType.AddStructuralProperty("StringProperty", EdmCoreModel.Instance.GetString(true));

            var outerComplexType = new EdmComplexType("TestNS", "ComplexType");
            outerComplexType.AddStructuralProperty("NumberProperty", EdmCoreModel.Instance.GetInt32(true));
            outerComplexType.AddStructuralProperty("StringProperty", EdmCoreModel.Instance.GetString(true));
            outerComplexType.AddStructuralProperty("InnerComplex", new EdmComplexTypeReference(innerComplexType, true));

            var model = new EdmModel();
            var complexType = new EdmComplexType("TestNS", "ComplexType");
            complexType.AddStructuralProperty("NumberProperty", EdmCoreModel.Instance.GetInt32(true));
            complexType.AddStructuralProperty("StringProperty", EdmCoreModel.Instance.GetString(true));
            complexType.AddStructuralProperty("InnerComplex", new EdmComplexTypeReference(innerComplexType, true));
            model.AddElement(complexType);

            var entityType = new EdmEntityType("TestNS", "EntityType", null, false, true);
            entityType.AddStructuralProperty("NumberProperty", EdmCoreModel.Instance.GetInt32(true));
            entityType.AddStructuralProperty("StringProperty", EdmCoreModel.Instance.GetString(true));
            entityType.AddStructuralProperty("ComplexProperty", new EdmComplexTypeReference(outerComplexType, true));

            model.AddElement(entityType);

            var container = new EdmEntityContainer("TestNS", "DefaultContainer");
            model.AddElement(container);

            var testDescriptorSet = versions.SelectMany(dataServiceVersion =>
                versions.SelectMany(edmVersion =>
                {
                    model.SetEdmVersion(edmVersion);

                    // Client only writes type for primitive properties with null value, Server writes it for both primitive and complex.
                    // Edm.String is never written as the default is Edm.String.
                    var testCases = new[]
                        {
                            new { PropertyName = "NumberProperty", ExpectedServerTypeName = "Edm.Int32", ExpectedClientTypeName = "Edm.Int32" },
                            new { PropertyName = "StringProperty", ExpectedServerTypeName = (string)null, ExpectedClientTypeName = (string)null },
                            new { PropertyName = "ComplexProperty", ExpectedServerTypeName = "TestNS.ComplexType", ExpectedClientTypeName = (string)null },
                            new { PropertyName = "OpenProperty", ExpectedServerTypeName = (string)null, ExpectedClientTypeName = (string)null },
                            new { PropertyName = "OpenProperty", ExpectedServerTypeName = (string)null, ExpectedClientTypeName = (string)null },
                            new { PropertyName = "ComplexProperty/NumberProperty", ExpectedServerTypeName = "Edm.Int32", ExpectedClientTypeName = "Edm.Int32" },
                            new { PropertyName = "ComplexProperty/StringProperty", ExpectedServerTypeName = (string)null, ExpectedClientTypeName = (string)null },
                            new { PropertyName = "ComplexProperty/InnerComplex", ExpectedServerTypeName = "TestNS.ComplexType", ExpectedClientTypeName = (string)null },
                            new { PropertyName = "ComplexProperty/InnerComplex/NumberProperty", ExpectedServerTypeName = "Edm.Int32", ExpectedClientTypeName = "Edm.Int32" },
                            new { PropertyName = "ComplexProperty/InnerComplex/StringProperty", ExpectedServerTypeName = (string)null, ExpectedClientTypeName = (string)null },
                        };

                    return testCases.Select(testCase =>
                    {
                        string[] propertyPath = testCase.PropertyName.Split('/');
                        ODataProperty property = new ODataProperty { Name = propertyPath[propertyPath.Length - 1], Value = null };
                        for (int i = propertyPath.Length - 2; i >= 0; i--)
                        {
                            property = new ODataProperty
                            {
                                Name = propertyPath[i],
                                Value = new ODataComplexValue
                                {
                                    Properties = new[] { property }
                                }
                            };
                        }

                        Func<XElement, XElement> extractor = (result) =>
                        {
                            result = TestAtomUtils.ExtractPropertiesFromEntry(result);
                            foreach (string name in propertyPath)
                            {
                                result = result.Element(TestAtomConstants.ODataXNamespace + name);
                            }

                            return result;
                        };

                        return new Func<TestODataBehaviorKind, ODataVersion, PayloadWriterTestDescriptor<ODataItem>>(
                            (behaviorKind, version) =>
                            {
                                string expectedTypeName = null;
                                switch (behaviorKind)
                                {
                                    case TestODataBehaviorKind.Default:
                                        break;
                                    case TestODataBehaviorKind.WcfDataServicesClient:
                                        expectedTypeName = testCase.ExpectedClientTypeName;
                                        break;
                                    case TestODataBehaviorKind.WcfDataServicesServer:
                                        expectedTypeName = testCase.ExpectedServerTypeName;
                                        break;
                                }

                                // Starting with V3, we only support the standard behavior
                                expectedTypeName = null;

                                return new PayloadWriterTestDescriptor<ODataItem>(
                                    this.Settings,
                                    new ODataResource()
                                    {
                                        TypeName = "TestNS.EntityType",
                                        Properties = new[] { property },
                                        SerializationInfo = new ODataResourceSerializationInfo()
                                        {
                                            NavigationSourceEntityTypeName = "TestNS.EntityType",
                                            NavigationSourceName = "MySet",
                                            ExpectedTypeName = "TestNS.EntityType"
                                        }
                                    },
                                    (tc) => new AtomWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                                    {
                                        Xml = expectedTypeName == null ? "<type/>" : "<type>" + expectedTypeName + "</type>",
                                        FragmentExtractor = (result) => new XElement("type",
                                            (string)extractor(result).Attribute(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.AtomTypeAttributeName))
                                    }
                                    )
                                { Model = model };
                            });
                    });
                }));

            this.CombinatorialEngineProvider.RunCombinations(
                TestWriterUtils.ODataBehaviorKinds,
                testDescriptorSet,
                this.WriterTestConfigurationProvider.AtomFormatConfigurationsWithIndent,
                (behaviorKind, testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.CloneAndApplyBehavior(behaviorKind);
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);
                    TestWriterUtils.WriteAndVerifyODataPayload(testDescriptor(behaviorKind, testConfiguration.Version), testConfiguration, this.Assert, this.Logger);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Verifies that arbitrarily nested complex and collection properties within an entry are written correctly.")]
        public void NestedComplexCollectionExpandedLinksTest()
        {
            IEdmModel model = TestModels.BuildTestModel();

            var complexvalue = PayloadBuilder.ComplexValue("TestModel.Address");
            complexvalue.PrimitiveProperty("Street", "1234 Redmond Way");
            complexvalue.PrimitiveProperty("Zip", 12345);
            complexvalue.WithTypeAnnotation(
                model.SchemaElements.OfType<IEdmComplexType>().Single(type => type.FullName() == "TestModel.Address"), true);

            var feed = PayloadBuilder.EntitySet().WithTypeAnnotation(MetadataUtils.EntityTypes(model).First());

            var payloadDescriptors = new PayloadTestDescriptor[]
            {
                // Multiple nesting of Complex Values and Collection Values.
                new PayloadTestDescriptor()
                {
                    PayloadElement = PayloadBuilder.Property("RootProperyName", complexvalue),
                    PayloadEdmModel = model.Clone(),
                }.InComplexValue().InCollection().InProperty("PropertyName").InComplexValue().InCollection().InProperty("PropertyName").InComplexValue()
                .InCollection().InProperty("PropertyName").InComplexValue().InCollection().InProperty("PropertyName").InEntity(),

                // Multiple nesting of Complex Values.
                new PayloadTestDescriptor()
                {
                    PayloadElement = PayloadBuilder.Property("RootProperyName", complexvalue),
                    PayloadEdmModel = model.Clone(),
                }.InComplexValue().InProperty("PropertyName").InComplexValue().InProperty("PropertyName").InComplexValue().InProperty("PropertyName")
                .InComplexValue().InProperty("PropertyName").InComplexValue().InProperty("PropertyName").InComplexValue().InProperty("PropertyName").InEntity(1,0),

                // Entry With an Expanded Link which is an entry containing a Complex collection.
                new PayloadTestDescriptor()
                {
                    PayloadElement = complexvalue,
                    PayloadEdmModel = model.Clone(),
                }.InCollection(1,1).InProperty("PropertyName").InComplexValue().InCollection().InProperty("PropertyName").InEntity().InEntryWithExpandedLink(/*singletonRelationship*/ true),

                // Entry With an Expanded Link which is a Feed containing an Entry with Complex collection properties.
                new PayloadTestDescriptor()
                {
                    PayloadElement = complexvalue,
                    PayloadEdmModel = model.Clone(),
                }.InCollection(1,2).InProperty("PropertyName").InComplexValue(1, 1).InCollection(1, 0).InProperty("PropertyName").InEntity(1,1).InFeed(2).InEntryWithExpandedLink(),

                // Entry With Nested Expanded Links which contain Entries.
                new PayloadTestDescriptor()
                {
                    PayloadElement = PayloadBuilder.Property("RootProperyName", complexvalue),
                    PayloadEdmModel = model.Clone(),
                }.InEntity(1, 1, ODataVersion.V4).InEntryWithExpandedLink(/*singletonRelationship*/ true).InEntryWithExpandedLink(/*singletonRelationship*/ true)
                .InEntryWithExpandedLink(/*singletonRelationship*/ true).InEntryWithExpandedLink(/*singletonRelationship*/ true)
                .InEntryWithExpandedLink(/*singletonRelationship*/ true).InEntryWithExpandedLink(/*singletonRelationship*/ true),

                // Entry with inline expanded feed association to an arbitrary depth (7) where the expanded feed has no entries
                new PayloadTestDescriptor()
                {
                    PayloadElement = feed,
                    PayloadEdmModel = model.Clone(),
                }.InEntryWithExpandedLink().InFeed(2).InEntryWithExpandedLink().InFeed(2).InEntryWithExpandedLink().InFeed(2).InEntryWithExpandedLink()
                .InFeed(2).InEntryWithExpandedLink(),
            };
            var testDescriptors = new List<PayloadWriterTestDescriptor>();

            foreach (var payloadDescriptor in payloadDescriptors)
            {
                var payload = payloadDescriptor.PayloadElement.DeepCopy();
                testDescriptors.Add(new PayloadWriterTestDescriptor<ODataPayloadElement>(this.Settings, payload)
                {
                    PayloadDescriptor = payloadDescriptor,
                    ExpectedResultCallback = (tc) =>
                    {
                        return new PayloadWriterTestExpectedResults(this.ExpectedResultSettings)
                        {
                            ExpectedPayload = payload,
                        };
                    },
                    PayloadElement = ((EntityInstance)payload),
                    SkipTestConfiguration = payloadDescriptor.SkipTestConfiguration
                });
            }

            // TODO: Fix places where we've lost JsonVerbose coverage to add JsonLight
            ////this.CombinatorialEngineProvider.RunCombinations(
            ////   testDescriptors,
            ////   this.WriterTestConfigurationProvider.JsonLightFormatConfigurationsWithIndent,
            ////   (testDescriptor, testConfiguration) =>
            ////   {
            ////       testDescriptor.RunTest(testConfiguration, this.Logger);
            ////   });

            testDescriptors.Clear();
            foreach (var payloadDescriptor in payloadDescriptors)
            {
                var payload = payloadDescriptor.PayloadElement.DeepCopy();
                payload.Accept(new AddFeedIDFixup());
                testDescriptors.Add(new PayloadWriterTestDescriptor<ODataPayloadElement>(this.Settings, payload)
                {
                    PayloadDescriptor = payloadDescriptor,
                    ExpectedResultCallback = (tc) =>
                    {
                        return new PayloadWriterTestExpectedResults(this.ExpectedResultSettings)
                        {
                            ExpectedPayload = payload,
                        };
                    },
                    PayloadElement = ((EntityInstance)payload),
                    SkipTestConfiguration = payloadDescriptor.SkipTestConfiguration
                });
            }

            this.CombinatorialEngineProvider.RunCombinations(
               testDescriptors,
               this.WriterTestConfigurationProvider.AtomFormatConfigurationsWithIndent,
               (testDescriptor, testConfiguration) =>
               {
                   testConfiguration = testConfiguration.Clone();
                   testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                   testDescriptor.RunTest(testConfiguration, this.Logger);
               });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Verifies versioning with null value properties on open types.")]
        public void NullPropertiesOnOpenTypes()
        {
            EdmModel model = new EdmModel();

            var entityTypeWithSpatial = new EdmEntityType("TestModel", "OpenTypeWithNullSpatial", null, isAbstract: false, isOpen: true);
            entityTypeWithSpatial.AddStructuralProperty("SpatialProperty", EdmPrimitiveTypeKind.GeometryPoint, isNullable: true);
            model.AddElement(entityTypeWithSpatial);

            var entityType = new EdmEntityType("TestModel", "OpenTypeWithNullUndeclaredProperty", null, isAbstract: false, isOpen: true);
            model.AddElement(entityType);

            var container = new EdmEntityContainer("TestModel", "DefaultContainer");
            container.AddEntitySet("OpenTypeWithNullSpatial", entityTypeWithSpatial);
            container.AddEntitySet("OpenTypeWithNullUndeclaredProperty", entityType);
            model.AddElement(container);

            // We use NullPropertyInstance to represent the expected value in the two tests below due
            // to limitations in the test deserialiser.

            // Spatial property set to null.
            EntityInstance entityWithNullSpatialProperty = new EntityInstance("TestModel.OpenTypeWithNullSpatial", false);
            entityWithNullSpatialProperty.PrimitiveProperty("SpatialProperty", null);
            entityWithNullSpatialProperty.WithTypeAnnotation(entityTypeWithSpatial);
            entityWithNullSpatialProperty.Id = "http://test/Id";

            EntityInstance spatialExpected = new EntityInstance("TestModel.OpenTypeWithNullSpatial", false);
            spatialExpected.Property(new NullPropertyInstance("SpatialProperty", null));
            spatialExpected.Id = "http://test/Id";
            spatialExpected.WithTypeAnnotation(entityTypeWithSpatial);

            // Undeclared property set to null.
            EntityInstance entityWithNullUndeclaredProperty = new EntityInstance("TestModel.OpenTypeWithNullUndeclaredProperty", false);
            entityWithNullUndeclaredProperty.PrimitiveProperty("UndeclaredProperty", null);
            entityWithNullUndeclaredProperty.Id = "http://test/Id";
            entityWithNullUndeclaredProperty.WithTypeAnnotation(entityType);

            EntityInstance undeclaredExpected = new EntityInstance("TestModel.OpenTypeWithNullUndeclaredProperty", false);
            undeclaredExpected.Property(new NullPropertyInstance("UndeclaredProperty", null));
            undeclaredExpected.Id = "http://test/Id";
            undeclaredExpected.WithTypeAnnotation(entityType);

            PayloadWriterTestDescriptor[] testDescriptors = new PayloadWriterTestDescriptor[]
            {
                // Spatial property with model on v3
                new PayloadWriterTestDescriptor<ODataPayloadElement>(this.Settings, (ODataPayloadElement)null)
                {
                    PayloadElement = entityWithNullSpatialProperty,
                    Model = model,

                    ExpectedResultCallback = (tc) =>
                    {
                        return new PayloadWriterTestExpectedResults(this.ExpectedResultSettings)
                        {
                            ExpectedPayload = spatialExpected
                        };
                    },
                },
                // Spatial property with model on version < v3
                new PayloadWriterTestDescriptor<ODataPayloadElement>(this.Settings, (ODataPayloadElement)null)
                {
                    PayloadElement = entityWithNullSpatialProperty,
                    Model = model,
                    SkipTestConfiguration = tc => tc.Version >= ODataVersion.V4,
                    ExpectedResultCallback = (tc) =>
                    {
                        return new PayloadWriterTestExpectedResults(this.ExpectedResultSettings)
                        {
                            ExpectedException2 = ODataExpectedExceptions.ODataException("ODataVersionChecker_GeographyAndGeometryNotSupported", tc.Version.ToText())
                        };
                    },
                },
                // Undeclared null property
                new PayloadWriterTestDescriptor<ODataPayloadElement>(this.Settings, entityWithNullUndeclaredProperty)
                {
                    PayloadElement = entityWithNullUndeclaredProperty,
                    Model = model,
                    ExpectedResultCallback = (tc) =>
                    {
                        return new PayloadWriterTestExpectedResults(this.ExpectedResultSettings)
                        {
                            ExpectedPayload = undeclaredExpected
                        };
                    },
                }
            };

            this.CombinatorialEngineProvider.RunCombinations(
               testDescriptors,
               this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent.Where(tc => false),
               (testDescriptor, testConfiguration) =>
               {
                   testConfiguration = testConfiguration.Clone();
                   testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                   testDescriptor.RunTest(testConfiguration, this.Logger);
               });
        }

        // [TestMethod, Variation(Description = "Verify correct parsing of entries with geolocated URIs")]
        public void GeolocatedUriTest()
        {
            // Geolocated URIs are cases where the read/edit/etc URIs for the same resource use different services
            string baseUri = "http://www.test.com/foo.svc/Target";

            EdmModel model = new EdmModel();

            var mleType = new EdmEntityType("TestModel", "MediaLinkType", null, false, false, true);
            model.AddElement(mleType);

            var entityType = new EdmEntityType("TestModel", "GeolocatedUriType");
            entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.String, isNullable: true));
            var navProp = entityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "NavProp", Target = mleType, TargetMultiplicity = EdmMultiplicity.Many });
            entityType.AddStructuralProperty("StreamProperty", EdmPrimitiveTypeKind.Stream, isNullable: false);
            model.AddElement(entityType);

            var container = new EdmEntityContainer("TestModel", "DefaultContainer");
            var mediaLinkSet = container.AddEntitySet("MediaLinkType", mleType);
            var geolocatedUriSet = container.AddEntitySet("GeolocatedUriType", entityType);
            geolocatedUriSet.AddNavigationTarget(navProp, mediaLinkSet);
            model.AddElement(container);

            EntityInstance entityInstance = PayloadBuilder.Entity(entityType.FullName())
                .Property("Id", PayloadBuilder.PrimitiveValue(System.Guid.NewGuid().ToString()))
                .Property(PayloadBuilder.DeferredNavigationProperty("NavProp",
                    PayloadBuilder.DeferredLink(baseUri.Replace("foo", "geo1")).IsCollection(true),
                    PayloadBuilder.DeferredLink(baseUri.Replace("foo", "geo2"))))
                .StreamProperty("StreamProperty", baseUri.Replace("foo", "read"), baseUri.Replace("foo", "edit"));

            entityInstance.Id = "1";

            EntityInstance mleInstance = PayloadBuilder.Entity(mleType.FullName()).EnsureMediaLinkEntry();
            mleInstance.Id = "1";
            mleInstance.StreamContentType = "image/jpeg";
            mleInstance.StreamEditLink = baseUri.Replace("foo", "edit-stream");
            mleInstance.StreamSourceLink = baseUri.Replace("foo", "source-stream");

            var testDescriptors = new[]
            {
                new PayloadWriterTestDescriptor<ODataPayloadElement>(this.Settings,entityInstance)
                {
                    Model = model,
                    PayloadDescriptor = new PayloadTestDescriptor()
                    {
                        PayloadElement = entityInstance
                    },
                    ExpectedResultCallback = (tc) =>
                    {
                        return new PayloadWriterTestExpectedResults(this.ExpectedResultSettings)
                        {
                            ExpectedPayload = entityInstance,
                        };
                    },
                },
                new PayloadWriterTestDescriptor<ODataPayloadElement>(this.Settings, mleInstance)
                {
                    Model = model,
                    PayloadDescriptor = new PayloadTestDescriptor()
                    {
                        PayloadElement = mleInstance
                    },
                    ExpectedResultCallback = (tc) =>
                    {
                        return new PayloadWriterTestExpectedResults(this.ExpectedResultSettings)
                        {
                            ExpectedPayload = mleInstance
                        };
                    },
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.JsonLightFormatConfigurationsWithIndent,
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration, this.Logger);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Ensures correct validation where typenames are null in the presence of a model.")]
        public void NullPropertyNameTest()
        {
            EdmModel model = new EdmModel();

            var complexType1 = new EdmComplexType("TestNS", "ComplexType1");
            complexType1.AddStructuralProperty("StringProperty", EdmPrimitiveTypeKind.String, isNullable: false);
            model.AddElement(complexType1);

            var entityType1 = new EdmEntityType("TestNS", "EntityType1");
            entityType1.AddStructuralProperty("ComplexProperty", new EdmComplexTypeReference(complexType1, isNullable: false));
            model.AddElement(entityType1);

            var entityType2 = new EdmEntityType("TestNS", "EntityType2");
            entityType2.AddStructuralProperty("ComplexCollection", EdmCoreModel.GetCollection(new EdmComplexTypeReference(complexType1, isNullable: false)));
            model.AddElement(entityType2);

            // For these payloads we expect that the product will infer and write the type on the complex and collection properties on the entries.
            // As such we expect a different payload to what we write.
            // Complex Property + expected
            ComplexInstance instance = PayloadBuilder.ComplexValue();
            instance.PrimitiveProperty("StringProperty", "Hello");
            ComplexProperty complexProperty = new ComplexProperty("ComplexProperty", instance);

            ComplexInstance instanceWithType = PayloadBuilder.ComplexValue("TestNS.ComplexType1");
            instanceWithType.PrimitiveProperty("StringProperty", "Hello");
            ComplexProperty complexPropertyWithType = new ComplexProperty("ComplexProperty", instanceWithType);

            // Entity Instance with complex property + expected
            EntityInstance entity1 = new EntityInstance("TestNS.EntityType1", false /*isNull*/);
            entity1.Property(complexProperty);
            entity1.Id = "urn:Id";
            entity1.WithTypeAnnotation(entityType1);
            EntityInstance expectedEntity1 = new EntityInstance("TestNS.EntityType1", false /*isNull*/);
            expectedEntity1.Property(complexPropertyWithType);
            expectedEntity1.Id = "urn:Id";
            expectedEntity1.WithTypeAnnotation(entityType1);

            // Complex Collection Property
            ComplexMultiValueProperty collection = new ComplexMultiValueProperty("ComplexCollection", new ComplexMultiValue(null, false, instance));
            ComplexMultiValueProperty collectionWithType = new ComplexMultiValueProperty("ComplexCollection", new ComplexMultiValue("Collection(TestNS.ComplexType1)", false, instanceWithType));

            // Entity Instance with collection property
            EntityInstance entity2 = new EntityInstance("TestNS.EntityType2", false);
            entity2.Property(collection);
            entity2.Id = "urn:Id";
            entity2.WithTypeAnnotation(entityType2);
            EntityInstance expectedEntity2 = new EntityInstance("TestNS.EntityType2", false);
            expectedEntity2.Property(collectionWithType);
            expectedEntity2.Id = "urn:Id";
            expectedEntity2.WithTypeAnnotation(entityType2);

            PayloadWriterTestDescriptor<ODataPayloadElement>[] testDescriptors = new PayloadWriterTestDescriptor<ODataPayloadElement>[]
            {
                new PayloadWriterTestDescriptor<ODataPayloadElement>(this.Settings, (ODataPayloadElement)null)
                {
                    PayloadElement = entity1,
                    Model = model,
                    ExpectedResultCallback = (tc) =>
                    {
                        return new PayloadWriterTestExpectedResults(this.ExpectedResultSettings)
                        {
                            ExpectedPayload = expectedEntity1
                        };
                    },
                },
                new PayloadWriterTestDescriptor<ODataPayloadElement>(this.Settings, (ODataPayloadElement)null)
                {
                    PayloadElement = entity2,
                    Model = model,
                    ExpectedResultCallback = (tc) =>
                    {
                        return new PayloadWriterTestExpectedResults(this.ExpectedResultSettings)
                        {
                            ExpectedPayload = expectedEntity2
                        };
                    },

                }
            };

            // TODO: Fix places where we've lost JsonVerbose coverage to add JsonLight
            this.CombinatorialEngineProvider.RunCombinations(
               testDescriptors,
               this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent.Where(tc => false),
               (testDescriptor, testConfiguration) =>
               {
                   testConfiguration = testConfiguration.Clone();
                   testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                   testDescriptor.RunTest(testConfiguration, this.Logger);
               });
        }

        // These tests and helpers are disabled on Silverlight and Phone because they
        // use private reflection not available on Silverlight and Phone
#if !SILVERLIGHT && !WINDOWS_PHONE
        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Test writing entry payloads with duplicate property names.")]
        public void DuplicatePropertyNamesTest()
        {
            ODataResourceSet defaultFeed = ObjectModelUtils.CreateDefaultFeed();
            ODataResource defaultEntry = ObjectModelUtils.CreateDefaultEntry();

            ODataProperty primitiveProperty = new ODataProperty { Name = "Foo", Value = 1 };
            ODataProperty complexProperty = new ODataProperty { Name = "Foo", Value = new ODataComplexValue { Properties = new[] { new ODataProperty() { Name = "StringProperty", Value = "xyz" } } } };
            ODataProperty collectionProperty = new ODataProperty { Name = "Foo", Value = new ODataCollectionValue { Items = new object[] { 1, 2 } } };
            ODataProperty streamProperty = new ODataProperty { Name = "Foo", Value = new ODataStreamReferenceValue { ReadLink = new Uri("http://odata.org/readlink") } };

            ODataNestedResourceInfo singletonLink = new ODataNestedResourceInfo { Name = "Foo", IsCollection = false, Url = new Uri("http://odata.org/link") };
            ODataNestedResourceInfo collectionLink = new ODataNestedResourceInfo { Name = "Foo", IsCollection = true, Url = new Uri("http://odata.org/links") };

            ExpectedException error = ODataExpectedExceptions.ODataException("DuplicatePropertyNamesNotAllowed", "Foo");
            ExpectedException duplicateExpandedLinkError = ODataExpectedExceptions.ODataException("MultipleLinksForSingleton", "Foo");
            ExpectedException streamPropertyInRequest = ODataExpectedExceptions.ODataException("WriterValidationUtils_StreamPropertyInRequest", "Foo");

            DuplicatePropertyNamesTestCase[] testCases = new DuplicatePropertyNamesTestCase[]
            {
                #region Duplicate primitive properties
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[] { new ODataResource { Properties = new [] { primitiveProperty, primitiveProperty } } },
                    ExpectedException = (duplicatesAllowed, tc) => duplicatesAllowed ? null : error,
                },
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[] { new ODataResource { Properties = new [] { complexProperty, complexProperty } } },
                    ExpectedException = (duplicatesAllowed, tc) => duplicatesAllowed ? null : error,
                },
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[] { new ODataResource { Properties = new [] { primitiveProperty, complexProperty } } },
                    ExpectedException = (duplicatesAllowed, tc) => duplicatesAllowed ? null : error,
                },
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[] { new ODataResource { Properties = new [] { collectionProperty, collectionProperty } } },
                    ExpectedException = (duplicatesAllowed, tc) => error,
                },
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[] { new ODataResource { Properties = new [] { primitiveProperty, collectionProperty } } },
                    ExpectedException = (duplicatesAllowed, tc) => error,
                },
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[] { new ODataResource { Properties = new [] { complexProperty, collectionProperty } } },
                    ExpectedException = (duplicatesAllowed, tc) => error,
                },
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[] { new ODataResource { Properties = new [] { primitiveProperty, streamProperty } } },
                    ExpectedException = (duplicatesAllowed, tc) => error,
                },
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[] { new ODataResource { Properties = new [] { complexProperty, streamProperty } } },
                    ExpectedException = (duplicatesAllowed, tc) =>  error,
                },
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[] { new ODataResource { Properties = new [] { collectionProperty, streamProperty } } },
                    ExpectedException = (duplicatesAllowed, tc) =>  error,
                },
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[] { new ODataResource { Properties = new [] { streamProperty, streamProperty } } },
                    ExpectedException = (duplicatesAllowed, tc) => tc.IsRequest ? streamPropertyInRequest : error,
                },
                #endregion Duplicate primitive properties

                #region Deferred navigation links
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[]
                    {
                        defaultEntry,
                            singletonLink,
                            null,
                            singletonLink,
                            null,
                    },
                    // Duplicate singleton entity reference links are allowed in requests, they're used for binding
                    ExpectedException = (duplicatesAllowed, tc) => (duplicatesAllowed || tc.IsRequest) ? null : error,
                },
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[]
                    {
                        defaultEntry,
                            singletonLink,
                            null,
                            collectionLink,
                            null,
                    },
                    // Singleton and collection entity reference links are allowed in requests, possible binding case.
                    ExpectedException = (duplicatesAllowed, tc) => (duplicatesAllowed || tc.IsRequest) ? null : error,
                },
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[]
                    {
                        defaultEntry,
                            collectionLink,
                            null,
                            collectionLink,
                            null,
                    },
                    // Duplicate collection entity reference links are allowed in requests, possible binding case.
                    ExpectedException = (duplicatesAllowed, tc) => (duplicatesAllowed || tc.IsRequest) ? null : error,
                },
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[]
                    {
                        new ODataResource { Properties = new [] { primitiveProperty } },
                            singletonLink,
                            null,
                    },
                    ExpectedException = (duplicatesAllowed, tc) => duplicatesAllowed ? null : error,
                },
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[]
                    {
                        new ODataResource { Properties = new [] { primitiveProperty } },

                            collectionLink,
                            null,
                    },
                    ExpectedException = (duplicatesAllowed, tc) => duplicatesAllowed ? null : error,
                },
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[]
                    {
                        new ODataResource { Properties = new [] { complexProperty } },

                            singletonLink,
                            null,
                    },
                    ExpectedException = (duplicatesAllowed, tc) => duplicatesAllowed ? null : error,
                },
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[]
                    {
                        new ODataResource { Properties = new [] { complexProperty } },
                            collectionLink,
                            null,
                    },
                    ExpectedException = (duplicatesAllowed, tc) => duplicatesAllowed ? null : error,
                },
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[]
                    {
                        new ODataResource { Properties = new [] { collectionProperty } },
                            singletonLink,
                            null,
                    },
                    ExpectedException = (duplicatesAllowed, tc) => error,
                },
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[]
                    {
                        new ODataResource { Properties = new [] { collectionProperty } },
                            collectionLink,
                            null,
                    },
                    ExpectedException = (duplicatesAllowed, tc) => error,
                },
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[]
                    {
                        new ODataResource { Properties = new [] { streamProperty } },
                            singletonLink,
                            null,
                    },
                    ExpectedException = (duplicatesAllowed, tc) => error,
                },
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[]
                    {
                        new ODataResource { Properties = new [] { streamProperty } },
                            collectionLink,
                            null,
                    },
                    ExpectedException = (duplicatesAllowed, tc) => error,
                },
                #endregion Deferred navigation links

                #region Expanded navigation links
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[]
                    {
                        defaultEntry,
                            singletonLink,
                                defaultEntry,
                                null,
                            null,
                            singletonLink,
                                defaultEntry,
                                null,
                            null,
                    },
                    ExpectedException = (duplicatesAllowed, tc) => duplicatesAllowed ? null : (tc.IsRequest ? duplicateExpandedLinkError : error),
                },
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[]
                    {
                        defaultEntry,
                            singletonLink,
                                defaultEntry,
                                null,
                            null,
                            collectionLink,
                                defaultFeed,
                                null,
                            null,
                    },
                    ExpectedException = (duplicatesAllowed, tc) => duplicatesAllowed ? null : (tc.IsRequest ? duplicateExpandedLinkError : error),
                },
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[]
                    {
                        defaultEntry,
                            collectionLink,
                                defaultFeed,
                                null,
                            null,
                            collectionLink,
                                defaultFeed,
                                null,
                            null,
                    },
                    ExpectedException = (duplicatesAllowed, tc) => duplicatesAllowed ? null : (tc.IsRequest ? null : error),
                },
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[]
                    {
                        new ODataResource { Properties = new [] { primitiveProperty } },

                            singletonLink,
                                defaultEntry,
                                null,
                            null,
                    },
                    ExpectedException = (duplicatesAllowed, tc) => duplicatesAllowed ? null : error,
                },
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[]
                    {
                        new ODataResource { Properties = new [] { primitiveProperty } },
                            collectionLink,
                                defaultFeed,
                                null,
                            null,
                    },
                    ExpectedException = (duplicatesAllowed, tc) => duplicatesAllowed ? null : error,
                },
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[]
                    {
                        new ODataResource { Properties = new [] { complexProperty } },
                            singletonLink,
                                defaultEntry,
                                null,
                            null,
                    },
                    ExpectedException = (duplicatesAllowed, tc) => duplicatesAllowed ? null : error,
                },
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[]
                    {
                        new ODataResource { Properties = new [] { complexProperty } },
                            collectionLink,
                                defaultFeed,
                                null,
                            null,
                    },
                    ExpectedException = (duplicatesAllowed, tc) => duplicatesAllowed ? null : error,
                },
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[]
                    {
                        new ODataResource { Properties = new [] { collectionProperty } },
                            singletonLink,
                                defaultEntry,
                                null,
                            null,
                    },
                    ExpectedException = (duplicatesAllowed, tc) => error,
                },
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[]
                    {
                        new ODataResource { Properties = new [] { collectionProperty } },
                            collectionLink,
                                defaultFeed,
                                null,
                            null,
                    },
                    ExpectedException = (duplicatesAllowed, tc) => error,
                },
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[]
                    {
                        new ODataResource { Properties = new [] { streamProperty } },
                            singletonLink,
                                defaultEntry,
                                null,
                            null,
                    },
                    ExpectedException = (duplicatesAllowed, tc) => error,
                },
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[]
                    {
                        new ODataResource { Properties = new [] { streamProperty } },
                            collectionLink,
                                defaultFeed,
                                null,
                            null,
                    },
                    ExpectedException = (duplicatesAllowed, tc) => error,
                },
                #endregion Expanded navigation links

                #region Duplicate properties on complex values
                // We should also put these complex values at the top-level and inside of collections.
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[] { new ODataResource { Properties = new [] { new ODataProperty { Name = "ComplexProp", Value = new ODataComplexValue { Properties = new [] { primitiveProperty, primitiveProperty } } } } } },
                    ExpectedException = (duplicatesAllowed, tc) => duplicatesAllowed ? null : error,
                },
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[] { new ODataResource { Properties = new [] { new ODataProperty { Name = "ComplexProp", Value = new ODataComplexValue { Properties = new [] { complexProperty, complexProperty } } } } } },
                    ExpectedException = (duplicatesAllowed, tc) => duplicatesAllowed ? null : error,
                },
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[] { new ODataResource { Properties = new [] { new ODataProperty { Name = "ComplexProp", Value = new ODataComplexValue { Properties = new [] { collectionProperty, collectionProperty } } } } } },
                    ExpectedException = (duplicatesAllowed, tc) => error,
                },
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[] { new ODataResource { Properties = new [] { new ODataProperty { Name = "ComplexProp", Value = new ODataComplexValue { Properties = new [] { primitiveProperty, complexProperty } } } } } },
                    ExpectedException = (duplicatesAllowed, tc) => duplicatesAllowed ? null : error,
                },
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[] { new ODataResource { Properties = new [] { new ODataProperty { Name = "ComplexProp", Value = new ODataComplexValue { Properties = new [] { primitiveProperty, collectionProperty } } } } } },
                    ExpectedException = (duplicatesAllowed, tc) => error,
                },
                new DuplicatePropertyNamesTestCase
                {
                    PayloadItems = new ODataItem[] { new ODataResource { Properties = new [] { new ODataProperty { Name = "ComplexProp", Value = new ODataComplexValue { Properties = new [] { complexProperty, collectionProperty } } } } } },
                    ExpectedException = (duplicatesAllowed, tc) => error,
                },
                #endregion Duplicate properties on complex values
            };

            IEnumerable<PayloadWriterTestDescriptor<ODataItem>> testDescriptors = testCases.Select(testCase =>
                new PayloadWriterTestDescriptor<ODataItem>(
                    this.Settings,
                    testCase.PayloadItems,
                    tc =>
                    {
                        bool allowDuplicateNames = tc.MessageWriterSettings.GetAllowDuplicatePropertyNames();
                        ExpectedException expectedError = testCase.ExpectedException(allowDuplicateNames, tc);
                        if (expectedError == null)
                        {
                            throw new NotSupportedException("Only ATOM and JSON formats are supported.");
                        }
                        else
                        {
                            return new WriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                            {
                                ExpectedException2 = expectedError,
                            };
                        }
                    }
                ));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors.PayloadCases(WriterPayloads.EntryPayloads),
                new bool[] { true, false },
                this.WriterTestConfigurationProvider.AtomFormatConfigurationsWithIndent,
                (testDescriptor, allowDuplicates, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();

                    if (allowDuplicates)
                    {
                        testConfiguration.MessageWriterSettings.Validations &= ~ValidationKinds.ThrowOnDuplicatePropertyNames;
                    }

                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    TestWriterUtils.WriteAndVerifyODataPayload(testDescriptor.DeferredLinksToEntityReferenceLinksInRequest(testConfiguration), testConfiguration, this.Assert, this.Logger);
                });
        }

        // Solve flush problem when disposing a writer in async mode.
        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Test writing payload asynchronously. Disposes before flush.")]
        public void EnsureSynchronousFlushWhenDisposingOnAsync()
        {
            //ToDo: Fix places where we've lost JsonVerbose coverage to add JsonLight
            this.CombinatorialEngineProvider.RunCombinations(
            this.WriterTestConfigurationProvider.ExplicitFormatConfigurations.Where(tc => false),
            (testConfiguration) =>
            {
                testConfiguration = testConfiguration.Clone();
                testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                TestStream messageStream = new TestStream(new MemoryStream());
                messageStream.FailSynchronousCalls = true;
                TestMessage message = TestWriterUtils.CreateOutputMessageFromStream(messageStream, testConfiguration);

                string expectedOutput = null;

                if (testConfiguration.IsRequest)
                {
                    expectedOutput = "{\"@odata.id\":\"urn:id\"";
                }
                else
                {
                    expectedOutput = "{\"@odata.id\":\"urn:id\"";
                }

                using (ODataMessageWriterTestWrapper messageWriterWrapper = TestWriterUtils.CreateMessageWriter(message, null, testConfiguration, this.Assert))
                {
                    var entryWriter = messageWriterWrapper.CreateODataResourceWriter();
                    entryWriter.WriteStart(new ODataResource()
                    {
                        Id = new Uri("urn:id"),
                        SerializationInfo = new ODataResourceSerializationInfo()
                        {
                            NavigationSourceEntityTypeName = "Null",
                            NavigationSourceName = "MySet",
                            ExpectedTypeName = "Null"
                        }
                    });
                }

                byte[] result = new byte[messageStream.InnerStream.Length];
                messageStream.InnerStream.Seek(0, SeekOrigin.Begin);
                var bytesRead = messageStream.InnerStream.Read(result, 0, (int)messageStream.InnerStream.Length);
                this.Assert.AreEqual(expectedOutput, Encoding.UTF8.GetString(result), "The result is not the same");
            });
        }

        private sealed class DuplicatePropertyNamesTestCase
        {
            public ODataItem[] PayloadItems { get; set; }
            public Func<bool, WriterTestConfiguration, ExpectedException> ExpectedException { get; set; }
        }
#endif

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Test writing entry payloads with duplicate navigation links. This test is targetted at the binding scenarios.")]
        public void DuplicateNavigationLinkTest()
        {
            IEdmModel model = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();

            Uri navLinkUri = new Uri("http://odata.org/navlink");
            ODataResource officeEntryInstance = ObjectModelUtils.CreateDefaultEntry();
            officeEntryInstance.TypeName = "TestModel.OfficeType";

            // Note that these tests specify behavior for requests. For responses we compute the behavior from these.
            IEnumerable<DuplicateNavigationLinkTestCase> testCases = new[]
            {
                new DuplicateNavigationLinkTestCase
                {
                    DebugDescription = "Two expanded links, both collection",
                    NavigationLinks = new []
                    {
                        new ODataItem[]
                        {
                            new ODataNestedResourceInfo { Name = "CityHall", Url = navLinkUri, IsCollection = true },
                                ObjectModelUtils.CreateDefaultFeed(),
                                    officeEntryInstance,
                                    null,
                                null,
                            null
                        },
                        new ODataItem[]
                        {
                            new ODataNestedResourceInfo { Name = "CityHall", Url = navLinkUri, IsCollection = true },
                                ObjectModelUtils.CreateDefaultFeed(),
                                null,
                            null
                        }
                    },
                },
                new DuplicateNavigationLinkTestCase
                {
                    DebugDescription = "Two expanded links, both singletons",
                    NavigationLinks = new []
                    {
                        new ODataItem[]
                        {
                            new ODataNestedResourceInfo { Name = "PoliceStation", Url = navLinkUri, IsCollection = false },
                                officeEntryInstance,
                                null,
                            null
                        },
                        new ODataItem[]
                        {
                            new ODataNestedResourceInfo { Name = "PoliceStation", Url = navLinkUri, IsCollection = false },
                                ObjectModelUtils.ODataNullEntry,
                                null,
                            null
                        }
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("MultipleLinksForSingleton", "PoliceStation"),
                },

                new DuplicateNavigationLinkTestCase
                {
                    DebugDescription = "Expanded singleton, deferred without type",
                    NavigationLinks = new []
                    {
                        new ODataItem[]
                        {
                            new ODataNestedResourceInfo { Name = "PoliceStation", Url = navLinkUri, IsCollection = false },
                                officeEntryInstance,
                                null,
                            null
                        },
                        new ODataItem[]
                        {
                            new ODataNestedResourceInfo { Name = "PoliceStation", Url = navLinkUri, IsCollection = null },
                                new ODataEntityReferenceLink { Url = navLinkUri },
                            null
                        }
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("MultipleLinksForSingleton", "PoliceStation"),
                    // ATOM requires IsCollection to be non-null, JSON in request request IsCollection to be non-null
                    SkipTestConfiguration = tc => false
                },
                new DuplicateNavigationLinkTestCase
                {
                    DebugDescription = "Expanded singleton, deferred singleton",
                    NavigationLinks = new []
                    {
                        new ODataItem[]
                        {
                            new ODataNestedResourceInfo { Name = "PoliceStation", Url = navLinkUri, IsCollection = false },
                                officeEntryInstance,
                                null,
                            null
                        },
                        new ODataItem[]
                        {
                            new ODataNestedResourceInfo { Name = "PoliceStation", Url = navLinkUri, IsCollection = false },
                                new ODataEntityReferenceLink { Url = navLinkUri },
                            null
                        }
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("MultipleLinksForSingleton", "PoliceStation"),
                },
                new DuplicateNavigationLinkTestCase
                {
                    DebugDescription = "Expanded singleton, deferred collection (no metadata), we will fail on this since we don't allow multiple links for singletons",
                    NavigationLinks = new []
                    {
                        new ODataItem[]
                        {
                            new ODataNestedResourceInfo { Name = "PoliceStation", Url = navLinkUri, IsCollection = false },
                                officeEntryInstance,
                                null,
                            null
                        },
                        new ODataItem[]
                        {
                            new ODataNestedResourceInfo { Name = "PoliceStation", Url = navLinkUri, IsCollection = false },
                                new ODataEntityReferenceLink { Url = navLinkUri },
                            null
                        }
                    },
                    ExpectedException = ODataExpectedExceptions.ODataException("MultipleLinksForSingleton", "PoliceStation"),
                },

                new DuplicateNavigationLinkTestCase
                {
                    DebugDescription = "Expanded collection, deferred without type - no failures",
                    NavigationLinks = new []
                    {
                        new ODataItem[]
                        {
                            new ODataNestedResourceInfo { Name = "CityHall", Url = navLinkUri, IsCollection = true },
                                ObjectModelUtils.CreateDefaultFeed(),
                                null,
                            null
                        },
                        new ODataItem[]
                        {
                            new ODataNestedResourceInfo { Name = "CityHall", Url = navLinkUri, IsCollection = null },
                                new ODataEntityReferenceLink { Url = navLinkUri },
                            null
                        }
                    },
                    // ATOM requires IsCollection to be non-null, JSON in request requires IsCollection to be non-null
                    SkipTestConfiguration = tc => false
                },
                new DuplicateNavigationLinkTestCase
                {
                    DebugDescription = "Expanded collection, deferred singleton - no failures (binding scenario) (the metadata mismatch is specifically allowed)",
                    NavigationLinks = new []
                    {
                        new ODataItem[]
                        {
                            new ODataNestedResourceInfo { Name = "CityHall", Url = navLinkUri, IsCollection = true },
                                ObjectModelUtils.CreateDefaultFeed(),
                                null,
                            null
                        },
                        new ODataItem[]
                        {
                            new ODataNestedResourceInfo { Name = "CityHall", Url = navLinkUri, IsCollection = false },
                                new ODataEntityReferenceLink { Url = navLinkUri },
                            null
                        }
                    },
                    // Skip this test in responses since we omit the entity reference link to turn the payload into
                    // a response and then the IsCollection value is invalid (since the mismatch is only allowed in requests)
                    SkipTestConfiguration = tc => !tc.IsRequest
                },
                new DuplicateNavigationLinkTestCase
                {
                    DebugDescription = "Expanded collection, deferred collection - no failures (binding scenario)",
                    NavigationLinks = new []
                    {
                        new ODataItem[]
                        {
                            new ODataNestedResourceInfo { Name = "CityHall", Url = navLinkUri, IsCollection = true },
                                ObjectModelUtils.CreateDefaultFeed(),
                                null,
                            null
                        },
                        new ODataItem[]
                        {
                            new ODataNestedResourceInfo { Name = "CityHall", Url = navLinkUri, IsCollection = true },
                                new ODataEntityReferenceLink { Url = navLinkUri },
                            null
                        }
                    },
                },

                new DuplicateNavigationLinkTestCase
                {
                    DebugDescription = "Two deferred singletons on a collection property - no failures (binding scenario) (the metadata mismatch is specifically allowed)",
                    NavigationLinks = new []
                    {
                        new ODataItem[]
                        {
                            new ODataNestedResourceInfo { Name = "CityHall", Url = navLinkUri, IsCollection = false },
                                new ODataEntityReferenceLink { Url = navLinkUri },
                            null
                        },
                        new ODataItem[]
                        {
                            new ODataNestedResourceInfo { Name = "CityHall", Url = navLinkUri, IsCollection = false },
                                new ODataEntityReferenceLink { Url = navLinkUri },
                            null
                        }
                    },
                    // Skip this test in responses since we omit the entity reference link to turn the payload into
                    // a response and then the IsCollection value is invalid (since the mismatch is only allowed in requests)
                    SkipTestConfiguration = tc => !tc.IsRequest
                },
                new DuplicateNavigationLinkTestCase
                {
                    DebugDescription = "Two deferred collections on a collection property - no failures (binding scenario)",
                    NavigationLinks = new []
                    {
                        new ODataItem[]
                        {
                            new ODataNestedResourceInfo { Name = "CityHall", Url = navLinkUri, IsCollection = true },
                                new ODataEntityReferenceLink { Url = navLinkUri },
                            null
                        },
                        new ODataItem[]
                        {
                            new ODataNestedResourceInfo { Name = "CityHall", Url = navLinkUri, IsCollection = true },
                                new ODataEntityReferenceLink { Url = navLinkUri },
                            null
                        }
                    },
                },
               new DuplicateNavigationLinkTestCase
                {
                    DebugDescription = "Deferred collection and deferred singleton - no failures (binding scenario) (the metadata mismatch is specifically allowed)",
                    NavigationLinks = new []
                    {
                        new ODataItem[]
                        {
                            new ODataNestedResourceInfo { Name = "CityHall", Url = navLinkUri, IsCollection = true },
                                new ODataEntityReferenceLink { Url = navLinkUri },
                            null
                        },
                        new ODataItem[]
                        {
                            new ODataNestedResourceInfo { Name = "CityHall", Url = navLinkUri, IsCollection = false },
                                new ODataEntityReferenceLink { Url = navLinkUri },
                            null
                        }
                    },
                    // Skip this test in responses since we omit the entity reference link to turn the payload into
                    // a response and then the IsCollection value is invalid (since the mismatch is only allowed in requests)
                    SkipTestConfiguration = tc => !tc.IsRequest
                },

                new DuplicateNavigationLinkTestCase
                {
                    DebugDescription = "Expanded collection, deferred collection and deferred singleton - no failures (binding scenario) (the metadata mismatch is specifically allowed)",
                    NavigationLinks = new []
                    {
                        new ODataItem[]
                        {
                            new ODataNestedResourceInfo { Name = "CityHall", Url = navLinkUri, IsCollection = true },
                                ObjectModelUtils.CreateDefaultFeed(),
                                null,
                            null
                        },
                        new ODataItem[]
                        {
                            new ODataNestedResourceInfo { Name = "CityHall", Url = navLinkUri, IsCollection = true },
                                new ODataEntityReferenceLink { Url = navLinkUri },
                            null
                        },
                        new ODataItem[]
                        {
                            new ODataNestedResourceInfo { Name = "CityHall", Url = navLinkUri, IsCollection = false },
                                new ODataEntityReferenceLink { Url = navLinkUri },
                            null
                        }
                    },
                    // Skip this test in responses since we omit the entity reference link to turn the payload into
                    // a response and then the IsCollection value is invalid (since the mismatch is only allowed in requests)
                    SkipTestConfiguration = tc => !tc.IsRequest
                },
            };

            testCases = testCases.SelectMany(testCase => testCase.NavigationLinks.Permutations().Select(navigationLinks =>
                new DuplicateNavigationLinkTestCase(testCase) { NavigationLinks = navigationLinks }));

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                new bool[] { true, false },
                this.WriterTestConfigurationProvider.AtomFormatConfigurationsWithIndent,
                (testCase, withMetadata, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    if (testCase.SkipTestConfiguration != null && testCase.SkipTestConfiguration(testConfiguration))
                    {
                        return;
                    }

                    if (testConfiguration.IsRequest)
                    {
                        return;
                    }

                    ODataResource entryInstance = ObjectModelUtils.CreateDefaultEntry();
                    entryInstance.TypeName = "TestModel.CityType";

                    string duplicationPropertyName = null;
                    ODataItem firstItem = testCase.NavigationLinks[0][0];
                    ODataNestedResourceInfo firstNavigationLink = firstItem as ODataNestedResourceInfo;
                    if (firstNavigationLink != null)
                    {
                        duplicationPropertyName = firstNavigationLink.Name;
                    }

                    Func<IEnumerable<ODataItem>, IEnumerable<ODataItem>> filterNavigationLinks = input => input;

                    ExpectedException expectedException = testCase.ExpectedException;
                    if (!testConfiguration.IsRequest)
                    {
                        // In responses, remove all entity reference links that will turn the payloads into true deferred links
                        filterNavigationLinks = input => input.Where(i => !(i is ODataEntityReferenceLink));

                        // In responses all duplicates will fail with the same error message since we don't allow any duplicates there.
                        expectedException =
                            ODataExpectedExceptions.ODataException(
                                "DuplicatePropertyNamesNotAllowed",
                                duplicationPropertyName);
                    }

                    PayloadWriterTestDescriptor<ODataItem> testDescriptor = new PayloadWriterTestDescriptor<ODataItem>(
                        this.Settings,
                        new[] { entryInstance }.Concat(
                            testCase.NavigationLinks.SelectMany(
                                navigationLinkItems => filterNavigationLinks(navigationLinkItems)))
                            .ConcatSingle(null)
                            .ToArray(),
                        tc => (WriterTestExpectedResults)
                                new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                                {
                                    // Skip verification of payload in success cases
                                    Json = null,
                                    ExpectedException2 = expectedException
                                })
                    {
                        Model = withMetadata ? model : null,
                    };

                    TestWriterUtils.WriteAndVerifyODataPayload(testDescriptor, testConfiguration, this.Assert,
                        this.Logger);
                });
        }

        [Ignore] // Remove Atom
        [TestMethod, Variation(Description = "Test writing entry payloads with assignable types that do not match the model exactly. This should fail.")]
        public void PrimitiveTypesMustMatchExactlyOnWriteTest()
        {
            EdmModel model = new EdmModel();

            var entityType = new EdmEntityType("TestNS", "WithIntType");
            entityType.AddStructuralProperty("Int64Property", EdmPrimitiveTypeKind.Int64, isNullable: false);
            model.AddElement(entityType);

            var entityType2 = new EdmEntityType("TestNS", "WithStringType");
            entityType2.AddStructuralProperty("StringProperty", EdmPrimitiveTypeKind.String, isNullable: false);
            model.AddElement(entityType2);

            EdmModel model2 = new EdmModel();

            var entityType3 = new EdmEntityType("TestNS", "WithSpatialType");
            entityType3.AddStructuralProperty("GeographyProperty", EdmPrimitiveTypeKind.GeographyPoint, isNullable: true);
            model2.AddElement(entityType3);

            var int64EntityWithInt32 = new EntityInstance("TestNS.WithIntType", false);
            int64EntityWithInt32.Property(new PrimitiveProperty("Int64Property", "Edm.Int32", 5));

            var stringEntityWithInt = new EntityInstance("TestNS.WithStringType", false);
            stringEntityWithInt.Property(new PrimitiveProperty("StringProperty", "Edm.Int32", 5));

            var geographyEntityWithString = new EntityInstance("TestNS.WithSpatialType", false);
            geographyEntityWithString.Property(new PrimitiveProperty("GeographyProperty", "Edm.String", "Hello"));

            var intEntityWithGeography = new EntityInstance("TestNS.WithIntType", false);
            intEntityWithGeography.Property(new PrimitiveProperty("Int64Property", "Edm.GeographyPoint", GeographyFactory.Point(32.0, -100.0).Build()).WithTypeAnnotation(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, false)));

            PayloadWriterTestDescriptor[] descriptors = new PayloadWriterTestDescriptor<ODataPayloadElement>[]
            {
                new PayloadWriterTestDescriptor<ODataPayloadElement>(this.Settings, (ODataPayloadElement)null)
                {
                    Model = model,
                    PayloadElement = int64EntityWithInt32,
                    ExpectedResultCallback = (tc) => new PayloadWriterTestExpectedResults(this.ExpectedResultSettings)
                    {
                        ExpectedException2 = ODataExpectedExceptions.ODataException("ValidationUtils_IncompatiblePrimitiveItemType", "Edm.Int32","False", "Edm.Int64", "False")
                    }
                },
                new PayloadWriterTestDescriptor<ODataPayloadElement>(this.Settings, (ODataPayloadElement)null)
                {
                    Model = model,
                    PayloadElement = stringEntityWithInt,
                    ExpectedResultCallback = (tc) => new PayloadWriterTestExpectedResults(this.ExpectedResultSettings)
                    {
                        ExpectedException2 = ODataExpectedExceptions.ODataException("ValidationUtils_IncompatiblePrimitiveItemType", "Edm.Int32","False", "Edm.String", "False")
                    }
                },
                new PayloadWriterTestDescriptor<ODataPayloadElement>(this.Settings, (ODataPayloadElement)null)
                {
                    Model = model2,
                    PayloadElement = geographyEntityWithString,
                    ExpectedResultCallback = (tc) => new PayloadWriterTestExpectedResults(this.ExpectedResultSettings)
                    {
                        ExpectedException2 = ODataExpectedExceptions.ODataException("ValidationUtils_IncompatiblePrimitiveItemType", "Edm.String","True", "Edm.GeographyPoint", "True")
                    },

                },
                new PayloadWriterTestDescriptor<ODataPayloadElement>(this.Settings, (ODataPayloadElement)null)
                {
                    Model = model,
                    PayloadElement = intEntityWithGeography,
                    ExpectedResultCallback = (tc) => new PayloadWriterTestExpectedResults(this.ExpectedResultSettings)
                    {
                        ExpectedException2 = ODataExpectedExceptions.ODataException("ValidationUtils_IncompatiblePrimitiveItemType", "Edm.GeographyPoint","True", "Edm.Int64", "False")
                    }
                }
            };

            this.CombinatorialEngineProvider.RunCombinations(descriptors,
                WriterTestConfigurationProvider.AtomFormatConfigurations,
                (descriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);

                    descriptor.RunTest(testConfiguration, this.Logger);
                });
        }

        private sealed class DuplicateNavigationLinkTestCase
        {
            public ODataItem[][] NavigationLinks { get; set; }
            public ExpectedException ExpectedException { get; set; }
            public bool NoMetadataOnly { get; set; }
            public Func<WriterTestConfiguration, bool> SkipTestConfiguration { get; set; }
            public string DebugDescription { get; set; }

            public DuplicateNavigationLinkTestCase()
            {
            }

            public DuplicateNavigationLinkTestCase(DuplicateNavigationLinkTestCase other)
            {
                this.NavigationLinks = other.NavigationLinks;
                this.ExpectedException = other.ExpectedException;
                this.NoMetadataOnly = other.NoMetadataOnly;
                this.SkipTestConfiguration = other.SkipTestConfiguration;
                this.DebugDescription = other.DebugDescription;
            }

            public override string ToString()
            {
                return this.DebugDescription;
            }
        }

        /// <summary>
        /// Constructs a nested chain of complex properties in order to test recursive depth limits.
        /// </summary>
        /// <param name="depth">The number of complex values deep to make the chain.</param>
        /// <param name="complexTypeName">The name of the type of the complex value.</param>
        /// <param name="propertyName">The name of the property whose value is a complex type.</param>
        /// <returns>An ODataProperty with the requested levels of depth.</returns>
        private static ODataProperty CreateDeeplyNestedComplexValues(int depth, string complexTypeName, string propertyName)
        {
            if (depth <= 0)
            {
                return new ODataProperty
                {
                    Name = propertyName,
                    Value = null
                };
            }

            return new ODataProperty
            {
                Name = propertyName,
                Value = new ODataComplexValue
                {
                    TypeName = complexTypeName,
                    Properties = new ODataProperty[]
                    {
                        CreateDeeplyNestedComplexValues(depth - 1, complexTypeName, propertyName)
                    }
                }
            };
        }

        /// <summary>
        /// Constructs a nested chain of complex collections in order to test recursive depth limits.
        /// </summary>
        /// <param name="depth">The number of complex and collection values deep to make the chain.</param>
        /// <param name="complexTypeName">The name of the type of the complex value.</param>
        /// <param name="propertyName">The name of the property whose value is a collection of complex values.</param>
        /// <returns>An ODataProperty with the requested levels of depth.</returns>
        private static ODataProperty CreateDeeplyNestedComplexValuesInCollections(int depth, string complexTypeName, string propertyName)
        {
            if (depth == 1)
            {
                return CreateDeeplyNestedComplexValues(depth, complexTypeName, propertyName);
            }

            if (depth <= 0)
            {
                return new ODataProperty
                {
                    Name = propertyName,
                    Value = null
                };
            }

            return new ODataProperty
            {
                Name = propertyName,
                Value = new ODataCollectionValue
                {
                    Items = new ODataComplexValue[]
                    {
                        new ODataComplexValue
                        {
                            TypeName = complexTypeName,
                            Properties = new ODataProperty[]
                            {
                                CreateDeeplyNestedComplexValuesInCollections(depth - 2, complexTypeName, propertyName)
                            }
                        }
                    }
                }
            };
        }
    }
}

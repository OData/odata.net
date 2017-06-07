//---------------------------------------------------------------------
// <copyright file="JsonLightEntryWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.JsonLight
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.Spatial;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Json.TextAnnotations;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for writing entry payloads in JSON Lite format.
    /// </summary>
    [TestClass, TestCase]
    public class JsonLightEntryWriterTests : ODataWriterTestCase
    {
        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestDescriptor.Settings Settings { get; set; }

        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestExpectedResults.Settings ExpectedResultSettings { get; set; }

        [TestMethod, Variation(Description = "Test payload order when writing JSON Lite entries.")]
        public void PayloadOrderTest()
        {
            EdmModel model = new EdmModel();
            var container = new EdmEntityContainer("TestModel", "DefaultContainer");
            model.AddElement(container);

            var otherType = new EdmEntityType("TestModel", "OtherType");
            otherType.AddKeys(otherType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false)));
            model.AddElement(otherType);
            var otherset = container.AddEntitySet("OtherType", otherType);

            var nonMLEBaseType = new EdmEntityType("TestModel", "NonMLEBaseType");
            nonMLEBaseType.AddKeys(nonMLEBaseType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false)));
            model.AddElement(nonMLEBaseType);
            var nonMLESet = container.AddEntitySet("NonMLESet", nonMLEBaseType);

            var nonMLEType = new EdmEntityType("TestModel", "NonMLEType", nonMLEBaseType);
            nonMLEType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(true));
            nonMLEType.AddStructuralProperty("Description", EdmCoreModel.Instance.GetString(true));
            nonMLEType.AddStructuralProperty("StreamProperty", EdmPrimitiveTypeKind.Stream, isNullable: false);
            var nonMLENav = nonMLEType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "NavProp", Target = otherType, TargetMultiplicity = EdmMultiplicity.Many });
            nonMLESet.AddNavigationTarget(nonMLENav, otherset);
            model.AddElement(nonMLEType);

            var mleBaseType = new EdmEntityType("TestModel", "MLEBaseType");
            mleBaseType.AddKeys(mleBaseType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false)));
            model.AddElement(mleBaseType);
            var mleSet = container.AddEntitySet("MLESet", mleBaseType);

            var mleType = new EdmEntityType("TestModel", "MLEType", mleBaseType, false, false, true);
            mleType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(true));
            mleType.AddStructuralProperty("Description", EdmCoreModel.Instance.GetString(true));
            mleType.AddStructuralProperty("StreamProperty", EdmPrimitiveTypeKind.Stream, isNullable: false);
            var mleNav = mleType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "NavProp", Target = otherType, TargetMultiplicity = EdmMultiplicity.Many });
            mleSet.AddNavigationTarget(mleNav, otherset);
            model.AddElement(mleType);

            IEnumerable<EntryPayloadTestCase> testCases = new[]
            {
                new EntryPayloadTestCase
                {
                    DebugDescription = "TypeName at the beginning, nothing else",
                    Entry = new ODataEntry() { TypeName = "TestModel.NonMLEType" },
                    Model = model,
                    EntitySet = nonMLESet,
                    Json = string.Join("$(NL)",
                        "{{",
                        "{1}" +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.NonMLEType\"{0}",
                        "}}")
                },
                new EntryPayloadTestCase
                {
                    DebugDescription = "TypeName at the beginning, changes at the end - the one from the beginning is used (also for validation).",
                    Entry = new ODataEntry() {
                            MediaResource = new ODataStreamReferenceValue(),
                            Properties = new []
                            {
                                new ODataProperty { Name = "ID", Value = (int)42 },
                                new ODataProperty { Name = "Name", Value = "test" },
                            }
                        }
                        .WithAnnotation(new WriteEntryCallbacksAnnotation
                            {
                                BeforeWriteStartCallback = (entry) => { entry.TypeName = "TestModel.MLEType"; },
                                BeforeWriteEndCallback = (entry) => { entry.TypeName = "NonExistingType"; }
                            }),
                    Model = model,
                    EntitySet = mleSet,
                    Json = string.Join("$(NL)",
                        "{{",
                        "{1}" +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.MLEType\"{0},\"ID\":\"42\",\"Name\":\"test\"",
                        "}}")
                },
                new EntryPayloadTestCase
                {
                    DebugDescription = "TypeName, ID and ETag at the beginning, nothing else",
                    Entry = new ODataEntry() { TypeName = "TestModel.NonMLEType", Id = new Uri("urn:id"), ETag="etag" },
                    Model = model,
                    EntitySet = nonMLESet,
                    Json = string.Join("$(NL)",
                        "{{",
                        "{1}" +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.NonMLEType\"," +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataIdAnnotationName + "\":\"urn:id\"," +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataETagAnnotationName + "\":\"etag\"{0}",
                        "}}")
                },
                new EntryPayloadTestCase
                {
                    DebugDescription = "TypeName at the beginning, ID and ETag at the end, ID and ETag are not written and are ignored at the end",
                    Entry = new ODataEntry() { TypeName = "TestModel.NonMLEType" }
                        .WithAnnotation(new WriteEntryCallbacksAnnotation
                            {
                                BeforeWriteStartCallback = (entry) => { entry.Id = null; entry.ETag = null; },
                                BeforeWriteEndCallback = (entry) => { entry.Id = new Uri("urn:id"); entry.ETag = "etag"; }
                            }),
                    Model = model,
                    EntitySet = nonMLESet,
                    Json = string.Join("$(NL)",
                        "{{",
                        "{1}" +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.NonMLEType\"{0}",
                        "}}")
                },
                new EntryPayloadTestCase
                {
                    DebugDescription = "Everything at the beginning",
                    Entry = new ODataEntry() {
                        TypeName = "TestModel.MLEType",
                        Id = new Uri("urn:id"),
                        ETag = "etag",
                        EditLink = new Uri("http://odata.org/editlink"),
                        ReadLink = new Uri("http://odata.org/readlink"),
                        MediaResource = new ODataStreamReferenceValue()
                        {
                            EditLink = new Uri("http://odata.org/mediaeditlink"),
                            ReadLink = new Uri("http://odata.org/mediareadlink"),
                            ETag = "mediaetag",
                            ContentType = "media/contenttype"
                        },
                        Properties = new []
                        {
                            new ODataProperty { Name = "ID", Value = (int)42 },
                            new ODataProperty { Name = "Name", Value = "test" },
                        }
                    },
                    Model = model,
                    EntitySet = mleSet,
                    Json = string.Join("$(NL)",
                        "{{",
                        "{1}" +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.MLEType\"," +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataIdAnnotationName + "\":\"urn:id\"," +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataETagAnnotationName + "\":\"etag\"," +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataEditLinkAnnotationName + "\":\"http://odata.org/editlink\"," +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataReadLinkAnnotationName + "\":\"http://odata.org/readlink\"," +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataMediaEditLinkAnnotationName + "\":\"http://odata.org/mediaeditlink\"," +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataMediaReadLinkAnnotationName + "\":\"http://odata.org/mediareadlink\"," +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataMediaContentTypeAnnotationName + "\":\"media/contenttype\"," +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataMediaETagAnnotationName + "\":\"mediaetag\"{0}," +
                        "\"ID\":\"42\"," +
                        "\"Name\":\"test\"",
                        "}}")
                },
                new EntryPayloadTestCase
                {
                    DebugDescription = "TypeName, Id, ETag and ReadLinks at the beginning, the rest at the end",
                    Entry = new ODataEntry() {
                        TypeName = "TestModel.MLEType",
                        Id = new Uri("urn:id"),
                        ETag = "etag",
                        ReadLink = new Uri("http://odata.org/readlink"),
                        MediaResource = new ODataStreamReferenceValue()
                        {
                            ReadLink = new Uri("http://odata.org/mediareadlink")
                        },
                        Properties = new []
                        {
                            new ODataProperty { Name = "ID", Value = (int)42 },
                            new ODataProperty { Name = "Name", Value = "test" },
                        }
                    }.WithAnnotation(new WriteEntryCallbacksAnnotation
                        {
                            BeforeWriteStartCallback = (entry) =>
                            {
                                entry.EditLink = null;
                                entry.MediaResource.EditLink = null;
                                entry.MediaResource.ETag = null;
                                entry.MediaResource.ContentType = null;
                            },
                            BeforeWriteEndCallback = (entry) =>
                            {
                                entry.EditLink = new Uri("http://odata.org/editlink");
                                entry.MediaResource.EditLink = new Uri("http://odata.org/mediaeditlink");
                                entry.MediaResource.ETag = "mediaetag";
                                entry.MediaResource.ContentType = "media/contenttype";
                            }
                        }),
                    Model = model,
                    EntitySet = mleSet,
                    Json = string.Join("$(NL)",
                        "{{",
                        "{1}" +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.MLEType\"," +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataIdAnnotationName + "\":\"urn:id\"," +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataETagAnnotationName + "\":\"etag\"," +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataReadLinkAnnotationName + "\":\"http://odata.org/readlink\"," +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataMediaReadLinkAnnotationName + "\":\"http://odata.org/mediareadlink\"{0}," +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataEditLinkAnnotationName + "\":\"http://odata.org/editlink\"," +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataMediaEditLinkAnnotationName + "\":\"http://odata.org/mediaeditlink\"," +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataMediaContentTypeAnnotationName + "\":\"media/contenttype\"," +
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataMediaETagAnnotationName + "\":\"mediaetag\"," +
                        "\"ID\":\"42\"," +
                        "\"Name\":\"test\"",
                        "}}")
                },
            };

            IEnumerable<PayloadWriterTestDescriptor<ODataItem>> testDescriptors = testCases.Select(testCase =>
                new PayloadWriterTestDescriptor<ODataItem>(
                    this.Settings,
                    new ODataItem[] { testCase.Entry },
                    tc => new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        Json = string.Format(CultureInfo.InvariantCulture, testCase.Json,
                            string.Empty,
                            JsonLightWriterUtils.GetMetadataUrlPropertyForEntry(testCase.EntitySet.Name) + ","),
                        FragmentExtractor = (result) => result.RemoveAllAnnotations(true)
                    })
                {
                    DebugDescription = testCase.DebugDescription,
                    Model = testCase.Model,
                    PayloadEdmElementContainer = testCase.EntitySet,
                    PayloadEdmElementType = testCase.EntityType,
                    SkipTestConfiguration = testCase.SkipTestConfiguration
                });

            testDescriptors = testDescriptors.Concat(testCases.Select(testCase =>
                new PayloadWriterTestDescriptor<ODataItem>(
                    this.Settings,
                    new ODataItem[] { testCase.Entry, new ODataNavigationLink
                    {
                        Name = "NavProp",
                        IsCollection = true,
                        Url = new Uri("http://odata.org/navprop/uri")
                    }},
                    tc => new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        Json = string.Format(CultureInfo.InvariantCulture, testCase.Json,
                            tc.IsRequest ?
                                ",\"" + JsonLightUtils.GetPropertyAnnotationName("NavProp", JsonLightConstants.ODataBindAnnotationName) + "\":[$(NL)\"http://odata.org/navprop/uri\"$(NL)]" :
                                ",\"" + JsonLightUtils.GetPropertyAnnotationName("NavProp", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navprop/uri\"",
                            JsonLightWriterUtils.GetMetadataUrlPropertyForEntry(testCase.EntitySet.Name) + ","),
                        FragmentExtractor = (result) => result.RemoveAllAnnotations(true)
                    })
                {
                    DebugDescription = testCase.DebugDescription + "- with navigation property",
                    Model = testCase.Model,
                    PayloadEdmElementContainer = testCase.EntitySet,
                    PayloadEdmElementType = testCase.EntityType,
                    SkipTestConfiguration = testCase.SkipTestConfiguration
                }))
                .Concat(testCases.Select(testCase =>
                new PayloadWriterTestDescriptor<ODataItem>(
                    this.Settings,
                    new ODataItem[] { testCase.Entry, new ODataNavigationLink
                    {
                        Name = "NavProp",
                        IsCollection = true,
                        Url = new Uri("http://odata.org/navprop/uri"),
                        AssociationLinkUrl = new Uri("http://odata.org/assoclink")
                    }},
                    tc => new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        Json = string.Format(CultureInfo.InvariantCulture, testCase.Json,
                            tc.IsRequest ?
                                ",\"" + JsonLightUtils.GetPropertyAnnotationName("NavProp", JsonLightConstants.ODataBindAnnotationName) + "\":[$(NL)\"http://odata.org/navprop/uri\"$(NL)]" :
                                ",\"" + JsonLightUtils.GetPropertyAnnotationName("NavProp", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navprop/uri\"" +
                                ",\"" + JsonLightUtils.GetPropertyAnnotationName("NavProp", JsonLightConstants.ODataAssociationLinkUrlAnnotationName) + "\":\"http://odata.org/assoclink\"",
                            JsonLightWriterUtils.GetMetadataUrlPropertyForEntry(testCase.EntitySet.Name) + ","),
                        FragmentExtractor = (result) => result.RemoveAllAnnotations(true)
                    })
                {
                    DebugDescription = testCase.DebugDescription + "- with navigation property",
                    Model = testCase.Model,
                    PayloadEdmElementContainer = testCase.EntitySet,
                    PayloadEdmElementType = testCase.EntityType,
                    SkipTestConfiguration = testCase.SkipTestConfiguration
                }));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.JsonLightFormatConfigurationsWithIndent,
                (testDescriptor, testConfiguration) =>
                {
                    TestWriterUtils.WriteAndVerifyODataEdmPayload(testDescriptor.DeferredLinksToEntityReferenceLinksInRequest(testConfiguration), testConfiguration, this.Assert, this.Logger);
                });
        }

        [TestMethod, Variation(Description = "Test payload order when writing actions and functions in JSON Lite entries.")]
        public void ActionAndFunctionPayloadOrderTest()
        {
            EdmModel model = new EdmModel();
            var container = new EdmEntityContainer("TestModel", "TestContainer");
            model.AddElement(container);

            var otherType = new EdmEntityType("TestModel", "OtherType");
            otherType.AddKeys(otherType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false)));
            model.AddElement(otherType);
            container.AddEntitySet("OtherType", otherType);

            var nonMLEBaseType = new EdmEntityType("TestModel", "NonMLEBaseType");
            nonMLEBaseType.AddKeys(nonMLEBaseType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false)));
            model.AddElement(nonMLEBaseType);
            var nonMLESet = container.AddEntitySet("NonMLESet", nonMLEBaseType);

            var nonMLEType = new EdmEntityType("TestModel", "NonMLEType", nonMLEBaseType);
            nonMLEType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(true));
            nonMLEType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "NavProp", Target = otherType, TargetMultiplicity = EdmMultiplicity.Many });
            model.AddElement(nonMLEType);
            container.AddEntitySet("NonMLEType", nonMLEType);

            ODataAction action = new ODataAction
            {
                Metadata = new Uri("http://odata.org/test/$metadata#defaultAction"),
                Title = "Default Action",
                Target = new Uri("http://www.odata.org/defaultAction"),
            };

            ODataFunction function = new ODataFunction
            {
                Metadata = new Uri("http://odata.org/test/$metadata#defaultFunction()"),
                Title = "Default Function",
                Target = new Uri("defaultFunctionTarget", UriKind.Relative)
            };

            string defaultJson = string.Join("$(NL)",
                "{{",
                "{1}" +
                "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"#TestModel.NonMLEType\"{0},\"#defaultAction\":{{",
                "\"title\":\"Default Action\",\"target\":\"http://www.odata.org/defaultAction\"",
                "}},\"#defaultFunction()\":{{",
                "\"title\":\"Default Function\",\"target\":\"defaultFunctionTarget\"",
                "}}",
                "}}");

            var entryWithActionAndFunction = new ODataEntry() { TypeName = "TestModel.NonMLEType", };
            entryWithActionAndFunction.AddAction(action);
            entryWithActionAndFunction.AddFunction(function);
            IEnumerable<EntryPayloadTestCase> testCases = new[]
            {
                new EntryPayloadTestCase
                {
                    DebugDescription = "Functions and actions available at the beginning.",
                    Entry = entryWithActionAndFunction,
                    Model = model,
                    EntitySet = nonMLESet,
                    Json = defaultJson
                },
            };

            IEnumerable<PayloadWriterTestDescriptor<ODataItem>> testDescriptors = testCases.Select(testCase =>
                new PayloadWriterTestDescriptor<ODataItem>(
                    this.Settings,
                    new ODataItem[] { testCase.Entry },
                    tc => new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        Json = string.Format(CultureInfo.InvariantCulture, testCase.Json,
                            string.Empty,
                            tc.IsRequest ? string.Empty : (JsonLightWriterUtils.GetMetadataUrlPropertyForEntry(testCase.EntitySet.Name) + ",")),
                        FragmentExtractor = (result) => result.RemoveAllAnnotations(true)
                    })
                {
                    DebugDescription = testCase.DebugDescription,
                    Model = testCase.Model,
                    PayloadEdmElementContainer = testCase.EntitySet,
                    PayloadEdmElementType = testCase.EntityType,
                    SkipTestConfiguration = testCase.SkipTestConfiguration
                });

            testDescriptors = testDescriptors.Concat(testCases.Select(testCase =>
                new PayloadWriterTestDescriptor<ODataItem>(
                    this.Settings,
                    new ODataItem[] { testCase.Entry, new ODataNavigationLink
                    {
                        Name = "NavProp",
                        IsCollection = true,
                        Url = new Uri("http://odata.org/navprop/uri")
                    }},
                    tc => new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        Json = string.Format(CultureInfo.InvariantCulture, testCase.Json,
                            tc.IsRequest ?
                                ",\"NavProp\":[$(NL){$(NL)\"__metadata\":{$(NL)\"uri\":\"http://odata.org/navprop/uri\"$(NL)}$(NL)}$(NL)]" :
                                ",\"" + JsonLightUtils.GetPropertyAnnotationName("NavProp", JsonLightConstants.ODataNavigationLinkUrlAnnotationName) + "\":\"http://odata.org/navprop/uri\"",
                                tc.IsRequest ? string.Empty : (JsonLightWriterUtils.GetMetadataUrlPropertyForEntry(testCase.EntitySet.Name) + ",")),
                        FragmentExtractor = (result) => result.RemoveAllAnnotations(true)
                    })
                {
                    DebugDescription = testCase.DebugDescription + "- with navigation property",
                    Model = testCase.Model,
                    PayloadEdmElementContainer = testCase.EntitySet,
                    PayloadEdmElementType = testCase.EntityType,
                    SkipTestConfiguration = testCase.SkipTestConfiguration
                }));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                // Actions/functions are only supported in responses.
                this.WriterTestConfigurationProvider.JsonLightFormatConfigurationsWithIndent.Where(tc => !tc.IsRequest),
                (testDescriptor, testConfiguration) =>
                {
                    TestWriterUtils.WriteAndVerifyODataEdmPayload(testDescriptor.DeferredLinksToEntityReferenceLinksInRequest(testConfiguration), testConfiguration, this.Assert, this.Logger);
                });
        }

        [TestMethod, Variation(Description = "Test skipping null values when writing JSON Lite entries with IgnoreNullValues = true.")]
        public void IgnoreNullPropertiesInEntryTest()
        {
            EdmModel model = new EdmModel();
            var container = new EdmEntityContainer("TestModel", "TestContainer");
            model.AddElement(container);

            var customerType = new EdmEntityType("TestModel", "CustomerType", null, false, isOpen: false);
            customerType.AddKeys(customerType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false)));
            customerType.AddKeys(customerType.AddStructuralProperty("Hobby", EdmCoreModel.Instance.GetString(true)));
            model.AddElement(customerType);
            var customerSet = container.AddEntitySet("CustomerSet", customerType);

            var addressType = new EdmComplexType("TestModel", "AddressType");
            addressType.AddStructuralProperty("Street", EdmCoreModel.Instance.GetStream(true));
            model.AddElement(addressType);

            IEnumerable<EntryPayloadTestCase> testCases = new[]
            {
                new EntryPayloadTestCase
                {
                    DebugDescription = "Customer instance with all properties set.",
                    Entry = new ODataEntry()
                    {
                        TypeName = "TestModel.CustomerType",
                        Properties = new ODataProperty[]
                        {
                            new ODataProperty { Name = "ID", Value = (int)42 },
                            new ODataProperty { Name = "Hobby", Value = "Hiking" },
                        }
                    },
                    Model = model,
                    EntitySet = customerSet,
                    Json = string.Join("$(NL)",
                        "{{",
                        "{0}\"ID\":\"42\", \"Hobby\":\"Hiking\"",
                        "}}")
                },
                new EntryPayloadTestCase
                {
                    DebugDescription = "Customer instance without Hobby.",
                    Entry = new ODataEntry()
                    {
                        TypeName = "TestModel.CustomerType",
                        Properties = new ODataProperty[]
                        {
                            new ODataProperty { Name = "ID", Value = (int)44 },
                            new ODataProperty { Name = "Hobby", Value = null },
                        }
                    },
                    Model = model,
                    EntitySet = customerSet,
                    Json = string.Join("$(NL)",
                        "{{",
                        "{0}\"ID\":\"44\"",
                        "}}")
                },

            };

            IEnumerable<PayloadWriterTestDescriptor<ODataItem>> testDescriptors = testCases.Select(testCase =>
                new PayloadWriterTestDescriptor<ODataItem>(
                    this.Settings,
                    new ODataItem[] { testCase.Entry },
                    tc => new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        Json = string.Format(
                            CultureInfo.InvariantCulture,
                            testCase.Json,
                            JsonLightWriterUtils.GetMetadataUrlPropertyForEntry(testCase.EntitySet.Name) + ","),
                        FragmentExtractor = (result) => result.RemoveAllAnnotations(true)
                    })
                {
                    DebugDescription = testCase.DebugDescription,
                    Model = testCase.Model,
                    PayloadEdmElementContainer = testCase.EntitySet,
                    PayloadEdmElementType = testCase.EntityType,
                    SkipTestConfiguration = testCase.SkipTestConfiguration
                });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.JsonLightFormatConfigurationsWithIndent,
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration.MessageWriterSettings.IgnoreNullValues = true;
                    TestWriterUtils.WriteAndVerifyODataEdmPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        [TestMethod, Variation(Description = "Test correct serialization format when writing JSON Lite entries with open properties.")]
        public void OpenPropertiesInEntryTest()
        {
            EdmModel model = new EdmModel();
            var container = new EdmEntityContainer("TestModel", "TestContainer");
            model.AddElement(container);

            var openCustomerType = new EdmEntityType("TestModel", "OpenCustomerType", null, false, isOpen: true);
            openCustomerType.AddKeys(openCustomerType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false)));
            model.AddElement(openCustomerType);
            var customerSet = container.AddEntitySet("CustomerSet", openCustomerType);

            var addressType = new EdmComplexType("TestModel", "AddressType");
            addressType.AddStructuralProperty("Street", EdmCoreModel.Instance.GetStream(true));
            model.AddElement(addressType);

            ISpatial pointValue = GeographyFactory.Point(32.0, -100.0).Build();

            IEnumerable<EntryPayloadTestCase> testCases = new[]
            {
                new EntryPayloadTestCase
                {
                    DebugDescription = "Customer instance with open primitive property.",
                    Entry = new ODataEntry()
                    {
                        TypeName = "TestModel.OpenCustomerType",
                        Properties = new ODataProperty[]
                        {
                            new ODataProperty { Name = "Age", Value = (long)42 }
                        }
                    },
                    Model = model,
                    EntitySet = customerSet,
                    Json = string.Join("$(NL)",
                        "{{",
                        "{0}\"" + JsonLightUtils.GetPropertyAnnotationName("Age", JsonLightConstants.ODataTypeAnnotationName) + "\":\"Edm.Int64\",\"Age\":\"42\"",
                        "}}")
                },
                new EntryPayloadTestCase
                {
                    DebugDescription = "Customer instance with open spatial property.",
                    Entry = new ODataEntry()
                    {
                        TypeName = "TestModel.OpenCustomerType",
                        Properties = new ODataProperty[]
                        {
                            new ODataProperty { Name = "Location", Value = pointValue }
                        }
                    },
                    Model = model,
                    EntitySet = customerSet,
                    Json = string.Join("$(NL)",
                        "{{",
                        "{0}" +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("Location", JsonLightConstants.ODataTypeAnnotationName) + "\":\"Edm.GeographyPoint\"," +
                        "\"Location\":{{",
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
                new EntryPayloadTestCase
                {
                    DebugDescription = "Customer instance with open complex property.",
                    Entry = new ODataEntry()
                    {
                        TypeName = "TestModel.OpenCustomerType",
                        Properties = new ODataProperty[]
                        {
                            new ODataProperty { Name = "Address", Value = new ODataComplexValue { TypeName = "TestModel.AddressType" } }
                        }
                    },
                    Model = model,
                    EntitySet = customerSet,
                    Json = string.Join("$(NL)",
                        "{{",
                        "{0}" +
                        "\"Address\":{{",
                        "\"" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataTypeAnnotationName + "\":\"TestModel.AddressType\"",
                        "}}",
                        "}}")
                },
            };

            IEnumerable<PayloadWriterTestDescriptor<ODataItem>> testDescriptors = testCases.Select(testCase =>
                new PayloadWriterTestDescriptor<ODataItem>(
                    this.Settings,
                    new ODataItem[] { testCase.Entry },
                    tc => new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        Json = string.Format(
                            CultureInfo.InvariantCulture,
                            testCase.Json,
                            JsonLightWriterUtils.GetMetadataUrlPropertyForEntry(testCase.EntitySet.Name) + ","),
                        FragmentExtractor = (result) => result.RemoveAllAnnotations(true)
                    })
                {
                    DebugDescription = testCase.DebugDescription,
                    Model = testCase.Model,
                    PayloadEdmElementContainer = testCase.EntitySet,
                    PayloadEdmElementType = testCase.EntityType,
                    SkipTestConfiguration = testCase.SkipTestConfiguration
                });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.JsonLightFormatConfigurationsWithIndent,
                (testDescriptor, testConfiguration) =>
                {
                    TestWriterUtils.WriteAndVerifyODataEdmPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        [TestMethod, Variation(Description = "Test correct serialization format when writing JSON Lite entries with spatial properties.")]
        public void SpatialPropertiesInEntryTest()
        {
            EdmModel model = new EdmModel();
            var container = new EdmEntityContainer("TestModel", "TestContainer");
            model.AddElement(container);

            var customerType = new EdmEntityType("TestModel", "CustomerType");
            customerType.AddKeys(customerType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false)));
            customerType.AddStructuralProperty("Location1", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.Geography, false));
            customerType.AddStructuralProperty("Location2", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, false));
            model.AddElement(customerType);
            var customerSet = container.AddEntitySet("CustomerSet", customerType);

            ISpatial pointValue = GeographyFactory.Point(32.0, -100.0).Build();

            IEnumerable<EntryPayloadTestCase> testCases = new[]
            {
                new EntryPayloadTestCase
                {
                    DebugDescription = "Customer instance with spatial property (expected and payload type don't match).",
                    Entry = new ODataEntry()
                    {
                        TypeName = "TestModel.CustomerType",
                        Properties = new ODataProperty[]
                        {
                            new ODataProperty { Name = "Location1", Value = pointValue }
                        }
                    },
                    Model = model,
                    EntitySet = customerSet,
                    Json = string.Join("$(NL)",
                        "{{",
                        "{0}" +
                        "\"" + JsonLightUtils.GetPropertyAnnotationName("Location1", JsonLightConstants.ODataTypeAnnotationName) + "\":\"GeographyPoint\"," +
                        "\"Location1\":{{",
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
                new EntryPayloadTestCase
                {
                    DebugDescription = "Customer instance with spatial property (expected and payload type match).",
                    Entry = new ODataEntry()
                    {
                        TypeName = "TestModel.CustomerType",
                        Properties = new ODataProperty[]
                        {
                            new ODataProperty { Name = "Location2", Value = pointValue }
                        }
                    },
                    Model = model,
                    EntitySet = customerSet,
                    Json = string.Join("$(NL)",
                        "{{",
                        "{0}" +
                        "\"Location2\":{{",
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

            IEnumerable<PayloadWriterTestDescriptor<ODataItem>> testDescriptors = testCases.Select(testCase =>
                new PayloadWriterTestDescriptor<ODataItem>(
                    this.Settings,
                    new ODataItem[] { testCase.Entry },
                    tc => new JsonWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        Json = string.Format(
                            CultureInfo.InvariantCulture,
                            testCase.Json,
                            JsonLightWriterUtils.GetMetadataUrlPropertyForEntry(testCase.EntitySet.Name) + ","),
                        FragmentExtractor = (result) => result.RemoveAllAnnotations(true)
                    })
                {
                    DebugDescription = testCase.DebugDescription,
                    Model = testCase.Model,
                    PayloadEdmElementContainer = testCase.EntitySet,
                    PayloadEdmElementType = testCase.EntityType,
                    SkipTestConfiguration = testCase.SkipTestConfiguration
                });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.JsonLightFormatConfigurationsWithIndent,
                (testDescriptor, testConfiguration) =>
                {
                    TestWriterUtils.WriteAndVerifyODataEdmPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }

        private sealed class EntryPayloadTestCase
        {
            public string DebugDescription { get; set; }
            public ODataEntry Entry { get; set; }
            public string Json { get; set; }
            public EdmModel Model { get; set; }
            public EdmEntitySet EntitySet { get; set; }
            public EdmEntityType EntityType { get; set; }
            public Func<Microsoft.Test.Taupo.OData.Common.TestConfiguration, bool> SkipTestConfiguration { get; set; }
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="WriterEntryAtomTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Atom
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Atom;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for writing entries with the OData writer in ATOM format.
    /// </summary>
    [TestClass, TestCase]
    public class WriterEntryAtomTests : ODataWriterTestCase
    {
        private static readonly Uri ServiceDocumentUri = new Uri("http://odata.org/");
        private const string DefaultNamespaceName = "TestModel";
        private static readonly IEdmStringTypeReference StringTypeRef = EdmCoreModel.Instance.GetString(isNullable: true);
        private static readonly IEdmPrimitiveTypeReference Int32TypeRef = EdmCoreModel.Instance.GetInt32(isNullable: false);
        private static readonly IEdmPrimitiveTypeReference Int32NullableTypeRef = EdmCoreModel.Instance.GetInt32(isNullable: true);

        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestDescriptor.Settings Settings { get; set; }

        private sealed class PayloadOrderTestCase
        {
            public string DebugDescription { get; set; }
            public ODataEntry Entry { get; set; }
            public string Xml { get; set; }
            public IEdmModel Model { get; set; }
            public Func<Microsoft.Test.Taupo.OData.Common.TestConfiguration, bool> SkipTestConfiguration { get; set; }
        }

        private IEdmModel BuildPayloadOrderTestModel()
        {
            var model = new EdmModel();
            var otherType = new EdmEntityType(DefaultNamespaceName, "OtherType");
            otherType.AddKeys(otherType.AddStructuralProperty("ID", Int32TypeRef));
            model.AddElement(otherType);

            var allInType = new EdmEntityType(DefaultNamespaceName, "AllInType", null, false, false, true);
            allInType.AddKeys(allInType.AddStructuralProperty("ID", Int32TypeRef));
            allInType.AddStructuralProperty("Name", StringTypeRef);
            allInType.AddStructuralProperty("Description", StringTypeRef);
            allInType.AddStructuralProperty("StreamProperty", EdmPrimitiveTypeKind.Stream, isNullable: false);
            allInType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "NavProp", Target = otherType, TargetMultiplicity = EdmMultiplicity.Many });
            model.AddElement(allInType);

            var container = new EdmEntityContainer(DefaultNamespaceName, "DefaultContainer");
            container.AddEntitySet("OtherType", otherType);
            container.AddEntitySet("AllInType", allInType);
            model.AddElement(container);

            return model;
        }

        [TestMethod, Variation(Description = "Test payload order when writing ATOM entries.")]
        public void PayloadOrderTest()
        {
            ODataFeedAndEntrySerializationInfo infoMyType = new ODataFeedAndEntrySerializationInfo()
            {
                NavigationSourceEntityTypeName = "MyType",
                NavigationSourceName = "MySet",
                ExpectedTypeName = "MyType"
            };

            ODataFeedAndEntrySerializationInfo infoAllInType = new ODataFeedAndEntrySerializationInfo()
            {
                NavigationSourceEntityTypeName = "TestModel.AllInType",
                NavigationSourceName = "MySet",
                ExpectedTypeName = "TestModel.AllInType"
            };

            IEdmModel model = BuildPayloadOrderTestModel();

            IEnumerable<PayloadOrderTestCase> testCases = new []
            {
                new PayloadOrderTestCase
                {
                    DebugDescription = "TypeName at the beginning, nothing else",
                    Entry = new ODataEntry() { TypeName = "MyType", SerializationInfo = infoMyType }
                        .WithAnnotation(new AtomEntryMetadata { Updated = ObjectModelUtils.DefaultEntryUpdatedDateTime }),
                    Xml = string.Join(
                        "$(NL)",
                        "<entry xmlns=\"" + TestAtomConstants.AtomNamespace + "\">",
                        "  <category term=\"#MyType\" scheme=\"http://docs.oasis-open.org/odata/ns/scheme\"/>",
                        "{0}  <id />",
                        "  <title />",
                        "  <updated>2010-10-12T17:13:00Z</updated>",
                        "  <author>",
                        "    <name />",
                        "  </author>",
                        "  <content type=\"application/xml\" />",
                        "</entry>"),
                },
                new PayloadOrderTestCase
                {
                    DebugDescription = "TypeName at the beginning, changes at the end - the one from the beginning is used (also for validation).",
                    Entry = new ODataEntry() {
                            MediaResource = new ODataStreamReferenceValue(),
                            Properties = new []
                            {
                                new ODataProperty { Name = "ID", Value = 42 },
                                new ODataProperty { Name = "Name", Value = "foo" },
                            },
                            SerializationInfo = infoMyType
                        }
                        .WithAnnotation(new WriteEntryCallbacksAnnotation 
                            {
                                BeforeWriteStartCallback = (entry) => { entry.TypeName = "TestModel.AllInType"; },
                                BeforeWriteEndCallback = (entry) => { entry.TypeName = "NonExistingType"; }
                            })
                        .WithAnnotation(new AtomEntryMetadata { Updated = ObjectModelUtils.DefaultEntryUpdatedDateTime }),
                    Model = model,
                    Xml = string.Join(
                        "$(NL)",
                        "<entry xmlns=\"" + TestAtomConstants.AtomNamespace + "\">",
                        "  <category term=\"#TestModel.AllInType\" scheme=\"http://docs.oasis-open.org/odata/ns/scheme\"/>",
                        "{0}  <id />",
                        "  <title />",
                        "  <updated>2010-10-12T17:13:00Z</updated>",
                        "  <author>",
                        "    <name />",
                        "  </author>",
                        "   <properties xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\">",
                        "       <ID p3:type=\"Edm.Int32\" xmlns:p3=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns=\"http://docs.oasis-open.org/odata/ns/data\">42</ID>",
                        "       <Name xmlns=\"http://docs.oasis-open.org/odata/ns/data\">foo</Name>",
                        "   </properties>",   
                        "</entry>"),
                },
                new PayloadOrderTestCase
                {
                    DebugDescription = "Just ETag at the beginning, changed at the end - the one from the beginning should be used.",
                    Entry = new ODataEntry() { SerializationInfo = infoMyType }
                        .WithAnnotation(new WriteEntryCallbacksAnnotation 
                            {
                                BeforeWriteStartCallback = (entry) => { entry.ETag = "\"etag1\""; },
                                BeforeWriteEndCallback = (entry) => { entry.ETag = "\"etag2\""; }
                            })
                        .WithAnnotation(new AtomEntryMetadata { Updated = ObjectModelUtils.DefaultEntryUpdatedDateTime }),
                    Xml = string.Join(
                        "$(NL)",
                        "<entry p1:etag=\"&quot;etag1&quot;\" xmlns:p1=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns=\"http://www.w3.org/2005/Atom\">",
                        "{0}  <id />",
                        "  <title />",
                        "  <updated>2010-10-12T17:13:00Z</updated>",
                        "  <author>",
                        "    <name />",
                        "  </author>",
                        "  <content type=\"application/xml\" />",
                        "</entry>"),
                },
                new PayloadOrderTestCase
                {
                    DebugDescription = "No TypeName at the beginning, nothing else - should not write the category",
                    Entry = new ODataEntry() { TypeName = null, SerializationInfo = infoMyType }
                        .WithAnnotation(new AtomEntryMetadata { Updated = ObjectModelUtils.DefaultEntryUpdatedDateTime }),
                    Xml = string.Join(
                        "$(NL)",
                        "<entry xmlns=\"" + TestAtomConstants.AtomNamespace + "\">",
                        "{0}  <id />",
                        "  <title />",
                        "  <updated>2010-10-12T17:13:00Z</updated>",
                        "  <author>",
                        "    <name />",
                        "  </author>",
                        "  <content type=\"application/xml\" />",
                        "</entry>"),
                },
                new PayloadOrderTestCase
                {
                    DebugDescription = "TypeName and ID at the beginning",
                    Entry = new ODataEntry() { TypeName = "MyType", Id = new Uri("http://odata.org/MyId"), SerializationInfo = infoMyType }
                        .WithAnnotation(new AtomEntryMetadata { Updated = ObjectModelUtils.DefaultEntryUpdatedDateTime }),
                    Xml = string.Join(
                        "$(NL)",
                        "<entry xmlns=\"" + TestAtomConstants.AtomNamespace + "\">",
                        "  <id>urn:MyId</id>",
                        "  <category term=\"#MyType\" scheme=\"http://docs.oasis-open.org/odata/ns/scheme\"/>",
                        "{0}  <title />",
                        "  <updated>2010-10-12T17:13:00Z</updated>",
                        "  <author>",
                        "    <name />",
                        "  </author>",
                        "  <content type=\"application/xml\" />",
                        "</entry>"),
                },
                new PayloadOrderTestCase
                {
                    DebugDescription = "TypeName and ID at the end",
                    Entry = new ODataEntry() { TypeName = "MyType", SerializationInfo = infoMyType }
                        .WithAnnotation(new WriteEntryCallbacksAnnotation { 
                            BeforeWriteStartCallback = (entry) => entry.Id = new Uri("http://odata.org/MyId"),
                            BeforeWriteEndCallback = (entry) => entry.Id = new Uri("http://odata.org/MyId") })
                        .WithAnnotation(new AtomEntryMetadata { Updated = ObjectModelUtils.DefaultEntryUpdatedDateTime }),
                    Xml = string.Join(
                        "$(NL)",
                        "<entry xmlns=\"" + TestAtomConstants.AtomNamespace + "\">",
                        "  <category term=\"#MyType\" scheme=\"http://docs.oasis-open.org/odata/ns/scheme\"/>",
                        "{0}  <id>urn:MyId</id>",
                        "  <title />",
                        "  <updated>2010-10-12T17:13:00Z</updated>",
                        "  <author>",
                        "    <name />",
                        "  </author>",
                        "  <content type=\"application/xml\" />",
                        "</entry>"),
                },
                new PayloadOrderTestCase
                {
                    DebugDescription = "TypeName, Self and Edit link at the beginning, Id at the end",
                    Entry = new ODataEntry() { TypeName = "MyType", ReadLink = ObjectModelUtils.DefaultEntryReadLink, EditLink = new Uri("http://odata.org/editlink"), SerializationInfo = infoMyType }
                        .WithAnnotation(new WriteEntryCallbacksAnnotation { 
                            BeforeWriteStartCallback = (entry) => entry.Id = null,
                            BeforeWriteEndCallback = (entry) => entry.Id = new Uri("http://odata.org/MyId") })
                        .WithAnnotation(new AtomEntryMetadata { Updated = ObjectModelUtils.DefaultEntryUpdatedDateTime }),
                    Xml = string.Join(
                        "$(NL)",
                        "<entry xmlns=\"" + TestAtomConstants.AtomNamespace + "\">",
                        "  <category term=\"#MyType\" scheme=\"http://docs.oasis-open.org/odata/ns/scheme\"/>",
                        "  <link rel=\"edit\" href=\"http://odata.org/editlink\" />",
                        "  <link rel=\"self\" href=\"" + ObjectModelUtils.DefaultEntryReadLink.OriginalString + "\" />",
                        "{0}  <id>urn:MyId</id>",
                        "  <title />",
                        "  <updated>2010-10-12T17:13:00Z</updated>",
                        "  <author>",
                        "    <name />",
                        "  </author>",
                        "  <content type=\"application/xml\" />",
                        "</entry>"),
                },
                new PayloadOrderTestCase
                {
                    DebugDescription = "Everything at the beginning",
                    Entry = new ODataEntry() { TypeName = "MyType", Id = new Uri("http://odata.org/MyId"), ReadLink = ObjectModelUtils.DefaultEntryReadLink, EditLink = new Uri("http://odata.org/editlink"), SerializationInfo = infoMyType }
                        .WithAnnotation(new AtomEntryMetadata { Updated = ObjectModelUtils.DefaultEntryUpdatedDateTime }),
                    Xml = string.Join(
                        "$(NL)",
                        "<entry xmlns=\"" + TestAtomConstants.AtomNamespace + "\">",
                        "  <id>urn:MyId</id>",
                        "  <category term=\"#MyType\" scheme=\"http://docs.oasis-open.org/odata/ns/scheme\"/>",
                        "  <link rel=\"edit\" href=\"http://odata.org/editlink\" />",
                        "  <link rel=\"self\" href=\"" + ObjectModelUtils.DefaultEntryReadLink.OriginalString + "\" />",
                        "{0}  <title />",
                        "  <updated>2010-10-12T17:13:00Z</updated>",
                        "  <author>",
                        "    <name />",
                        "  </author>",
                        "  <content type=\"application/xml\" />",
                        "</entry>"),
                },
                new PayloadOrderTestCase
                {
                    DebugDescription = "Everything at the end",
                    Entry = new ODataEntry() { TypeName = "MyType", Id = new Uri("http://odata.org/MyId"), SerializationInfo = infoMyType  }
                        .WithAnnotation(new WriteEntryCallbacksAnnotation {
                            BeforeWriteStartCallback = (entry) => { entry.Id = null; entry.ReadLink = null; entry.EditLink = null; },
                            BeforeWriteEndCallback = (entry) => { entry.Id = new Uri("http://odata.org/MyId"); entry.ReadLink = ObjectModelUtils.DefaultEntryReadLink; entry.EditLink = new Uri("http://odata.org/editlink"); }})
                        .WithAnnotation(new AtomEntryMetadata { Updated = ObjectModelUtils.DefaultEntryUpdatedDateTime }),
                    Xml = string.Join(
                        "$(NL)",
                        "<entry xmlns=\"" + TestAtomConstants.AtomNamespace + "\">",
                        "  <category term=\"#MyType\" scheme=\"http://docs.oasis-open.org/odata/ns/scheme\"/>",
                        "{0}  <id>urn:MyId</id>",
                        "  <link rel=\"edit\" href=\"http://odata.org/editlink\" />",
                        "  <link rel=\"self\" href=\"" + ObjectModelUtils.DefaultEntryReadLink.OriginalString + "\" />",
                        "  <title />",
                        "  <updated>2010-10-12T17:13:00Z</updated>",
                        "  <author>",
                        "    <name />",
                        "  </author>",
                        "  <content type=\"application/xml\" />",
                        "</entry>"),
                },
                new PayloadOrderTestCase
                {
                    DebugDescription = "Everything at the beginning, category with type name, edit and self link with ATOM metadata",
                    Entry = new ODataEntry() { TypeName = "MyType", Id = new Uri("http://odata.org/MyId"), ReadLink = ObjectModelUtils.DefaultEntryReadLink, EditLink = new Uri("http://odata.org/editlink"), SerializationInfo = infoMyType }
                        .WithAnnotation(new AtomEntryMetadata { 
                            EditLink = new AtomLinkMetadata { Title = "EditLinkTitle" },
                            SelfLink = new AtomLinkMetadata { Title = "SelfLinkTitle" },
                            CategoryWithTypeName = new AtomCategoryMetadata { Label = "TypeNameLabel" },
                            Updated = ObjectModelUtils.DefaultEntryUpdatedDateTime }),
                    Xml = string.Join(
                        "$(NL)",
                        "<entry xmlns=\"" + TestAtomConstants.AtomNamespace + "\">",
                        "  <id>urn:MyId</id>",
                        "  <category term=\"#MyType\" scheme=\"http://docs.oasis-open.org/odata/ns/scheme\" label=\"TypeNameLabel\" />",
                        "  <link rel=\"edit\" title=\"EditLinkTitle\" href=\"http://odata.org/editlink\" />",
                        "  <link rel=\"self\" title=\"SelfLinkTitle\" href=\"" + ObjectModelUtils.DefaultEntryReadLink.OriginalString + "\" />",
                        "{0}  <title />",
                        "  <updated>2010-10-12T17:13:00Z</updated>",
                        "  <author>",
                        "    <name />",
                        "  </author>",
                        "  <content type=\"application/xml\" />",
                        "</entry>"),
                },
                new PayloadOrderTestCase
                {
                    DebugDescription = "Everything at the end, category with type name, edit and self link with ATOM metadata (from the beginning)",
                    Entry = new ODataEntry() { TypeName = "MyType", SerializationInfo = infoMyType }
                        .WithAnnotation(new WriteEntryCallbacksAnnotation {
                            BeforeWriteStartCallback = (entry) => { entry.Id = null; entry.ReadLink = null; entry.EditLink = null; },
                            BeforeWriteEndCallback = (entry) => { entry.Id = new Uri("http://odata.org/MyId"); entry.ReadLink = ObjectModelUtils.DefaultEntryReadLink; entry.EditLink = new Uri("http://odata.org/editlink"); }})
                        .WithAnnotation(new AtomEntryMetadata { 
                            EditLink = new AtomLinkMetadata { Title = "EditLinkTitle" },
                            SelfLink = new AtomLinkMetadata { Title = "SelfLinkTitle" },
                            CategoryWithTypeName = new AtomCategoryMetadata { Label = "TypeNameLabel" },
                            Updated = ObjectModelUtils.DefaultEntryUpdatedDateTime }),
                    Xml = string.Join(
                        "$(NL)",
                        "<entry xmlns=\"" + TestAtomConstants.AtomNamespace + "\">",
                        "  <category term=\"#MyType\" scheme=\"http://docs.oasis-open.org/odata/ns/scheme\" label=\"TypeNameLabel\" />",
                        "{0}  <id>urn:MyId</id>",
                        "  <link rel=\"edit\" title=\"EditLinkTitle\" href=\"http://odata.org/editlink\" />",
                        "  <link rel=\"self\" title=\"SelfLinkTitle\" href=\"" + ObjectModelUtils.DefaultEntryReadLink.OriginalString + "\" />",
                        "  <title />",
                        "  <updated>2010-10-12T17:13:00Z</updated>",
                        "  <author>",
                        "    <name />",
                        "  </author>",
                        "  <content type=\"application/xml\" />",
                        "</entry>"),
                },
                new PayloadOrderTestCase
                {
                    DebugDescription = "With default stream and stream property, everything at the end",
                    Entry = new ODataEntry() { 
                            TypeName = "TestModel.AllInType",
                            MediaResource = new ODataStreamReferenceValue
                            {
                                ReadLink = new Uri("http://odata.org/mrreadlink"),
                                ContentType = "mr/type",
                                EditLink = new Uri("http://odata.org/mreditlink")
                            },
                            Properties = new []
                            {
                                new ODataProperty { Name = "ID", Value = 42 },
                                new ODataProperty { Name = "Name", Value = "foo" },
                                new ODataProperty { Name = "Description", Value = "bar" },
                                new ODataProperty { Name = "StreamProperty", Value = new ODataStreamReferenceValue
                                {
                                    ReadLink = new Uri("http://odata.org/streamproperty/readlink"),
                                    ContentType = "streamproperty/type",
                                    EditLink = new Uri("http://odata.org/streamproperty/editlink"),
                                }}
                            },
                            SerializationInfo = infoAllInType
                        }
                        .WithAnnotation(new WriteEntryCallbacksAnnotation {
                            BeforeWriteStartCallback = (entry) => { entry.Id = null; entry.ReadLink = null; entry.EditLink = null; },
                            BeforeWriteEndCallback = (entry) => { entry.Id = new Uri("http://odata.org/MyId"); entry.ReadLink = ObjectModelUtils.DefaultEntryReadLink; entry.EditLink = new Uri("http://odata.org/editlink"); }})
                        .WithAnnotation(new AtomEntryMetadata { 
                            EditLink = new AtomLinkMetadata { Title = "EditLinkTitle" },
                            SelfLink = new AtomLinkMetadata { Title = "SelfLinkTitle" },
                            CategoryWithTypeName = new AtomCategoryMetadata { Label = "TypeNameLabel" },
                            Updated = ObjectModelUtils.DefaultEntryUpdatedDateTime }),
                    Model = model,
                    Xml = string.Join(
                        "$(NL)",
                        "<entry xmlns=\"" + TestAtomConstants.AtomNamespace + "\">",
                        "  <category term=\"#TestModel.AllInType\" scheme=\"http://docs.oasis-open.org/odata/ns/scheme\" label=\"TypeNameLabel\" />",
                        "{0}  <id>urn:MyId</id>",
                        "  <link rel=\"edit\" title=\"EditLinkTitle\" href=\"http://odata.org/editlink\" />",
                        "  <link rel=\"self\" title=\"SelfLinkTitle\" href=\"" + ObjectModelUtils.DefaultEntryReadLink.OriginalString + "\" />",
                        "  <title />",
                        "  <updated>2010-10-12T17:13:00Z</updated>",
                        "  <author>",
                        "    <name />",
                        "  </author>",
                        "  <link rel=\"http://docs.oasis-open.org/odata/ns/mediaresource/StreamProperty\" type=\"streamproperty/type\" title=\"StreamProperty\" href=\"http://odata.org/streamproperty/readlink\" />",
                        "  <link rel=\"http://docs.oasis-open.org/odata/ns/edit-media/StreamProperty\" type=\"streamproperty/type\" title=\"StreamProperty\" href=\"http://odata.org/streamproperty/editlink\" />",
                        "  <link rel=\"edit-media\" href=\"http://odata.org/mreditlink\" />",
                        "  <content type=\"mr/type\" src=\"http://odata.org/mrreadlink\" />",
                        "  <properties xmlns=\"http://docs.oasis-open.org/odata/ns/metadata\">",
                        "    <ID p3:type=\"Edm.Int32\" xmlns:p3=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns=\"http://docs.oasis-open.org/odata/ns/data\">42</ID>",
                        "    <Name xmlns=\"http://docs.oasis-open.org/odata/ns/data\">foo</Name>",
                        "    <Description xmlns=\"http://docs.oasis-open.org/odata/ns/data\">bar</Description>",
                        "  </properties>",
                        "</entry>"),
                    // Stream properties are not allowed in requests.
                    SkipTestConfiguration = tc => tc.IsRequest
                },
            };

            IEnumerable<PayloadWriterTestDescriptor<ODataItem>> testDescriptors = testCases.Select(testCase =>
                new PayloadWriterTestDescriptor<ODataItem>(
                    this.Settings,
                    new ODataItem[] { testCase.Entry },
                    tc => new AtomWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        Xml = string.Format(CultureInfo.InvariantCulture, testCase.Xml, string.Empty),
                        FragmentExtractor = (element) => element
                    })
                {
                    DebugDescription = testCase.DebugDescription,
                    Model = testCase.Model,
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
                    tc => new AtomWriterTestExpectedResults(this.Settings.ExpectedResultSettings)
                    {
                        Xml = string.Format(CultureInfo.InvariantCulture, testCase.Xml, 
                            "  <link rel=\"" + TestAtomConstants.ODataNavigationPropertiesRelatedLinkRelationPrefix + "NavProp\" " +
                            "type=\"application/atom+xml;type=feed\" title=\"NavProp\" " +
                            "href=\"http://odata.org/navprop/uri\" />$(NL)"),
                        FragmentExtractor = (element) => element
                    })
                    {
                        DebugDescription = testCase.DebugDescription + "- with navigation property",
                        Model = testCase.Model,
                        SkipTestConfiguration = testCase.SkipTestConfiguration
                    }));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.AtomFormatConfigurationsWithIndent,
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = testConfiguration.Clone();
                    testConfiguration.MessageWriterSettings.SetServiceDocumentUri(ServiceDocumentUri);
                    TestWriterUtils.WriteAndVerifyODataPayload(testDescriptor.DeferredLinksToEntityReferenceLinksInRequest(testConfiguration), testConfiguration, this.Assert, this.Logger);
                });
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="ODataJsonLightSingletonWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Core.JsonLight;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.ScenarioTests.Writer.JsonLight
{
    public class ODataJsonLightSingletonWriterTests
    {
        private readonly Uri serviceDocumentUri = new Uri("http://odata.org/test/");
        private EdmEntityContainer defaultContainer;
        private IEdmModel userModel;
        private EdmModel referencedModel;
        private EdmSingleton singleton;
        private EdmEntityType webType;
        private EdmEntityType pageType;

        public ODataJsonLightSingletonWriterTests()
        {
            referencedModel = new EdmModel();

            this.webType = new EdmEntityType("NS", "Web");
            this.webType.AddStructuralProperty("WebId", EdmPrimitiveTypeKind.Int32);
            this.webType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            referencedModel.AddElement(this.webType);

            this.defaultContainer = new EdmEntityContainer("NS", "DefaultContainer_sub");
            referencedModel.AddElement(defaultContainer);

            this.userModel = TestUtils.WrapReferencedModelsToMainModel("NS", "DefaultContainer", referencedModel);
            this.singleton = new EdmSingleton(defaultContainer, "MySingleton", this.webType);
            this.defaultContainer.AddElement(this.singleton);
        }

        [Fact]
        public void WriteSimpleSingletonTest()
        {
            var entry = new ODataEntry { TypeName = "NS.Web" };
            entry.Properties = new[]
            {
                new ODataProperty {Name = "WebId", Value = 10},
                new ODataProperty {Name = "Name", Value = "SingletonWeb" }, 
            };
            const string expectedPayload = "{" +
                "\"@odata.context\":\"http://odata.org/test/$metadata#MySingleton\"," +
                "\"@odata.type\":\"#NS.Web\"," +
                "\"@odata.id\":\"MySingleton\"," +
                "\"@odata.editLink\":\"MySingleton\"," +
                "\"WebId\":10," +
                "\"Name\":\"SingletonWeb\"}";
            this.WriteEntryAndValidatePayloadWithoutModel(entry, expectedPayload);
            this.WriteEntryAndValidatePayloadWithModel(entry, expectedPayload);
        }

        [Fact]
        public void WriteSingletonInstanceAnnotationTest()
        {
            var entry = new ODataEntry { TypeName = "NS.Web" };
            entry.Properties = new[]
            {
                new ODataProperty {Name = "WebId", Value = 10}
            };
            entry.InstanceAnnotations.Add(new ODataInstanceAnnotation("Is.Singleton", new ODataPrimitiveValue(true)));
            const string expectedPayload = "{" +
                "\"@odata.context\":\"http://odata.org/test/$metadata#MySingleton\"," +
                "\"@odata.type\":\"#NS.Web\"," +
                "\"@odata.id\":\"MySingleton\"," +
                "\"@odata.editLink\":\"MySingleton\"," +
                "\"@Is.Singleton\":true," +
                "\"WebId\":10}";
            this.WriteEntryAndValidatePayloadWithoutModel(entry, expectedPayload);
            this.WriteEntryAndValidatePayloadWithModel(entry, expectedPayload);
        }

        [Fact]
        public void WriteSingletonWithNoPropertiesTest()
        {
            var entry = new ODataEntry { TypeName = "NS.Web" };
            const string expectedPayload = "{" +
                "\"@odata.context\":\"http://odata.org/test/$metadata#MySingleton\"," +
                "\"@odata.type\":\"#NS.Web\"," +
                "\"@odata.id\":\"MySingleton\"," +
                "\"@odata.editLink\":\"MySingleton\"}";
            this.WriteEntryAndValidatePayloadWithoutModel(entry, expectedPayload);
            this.WriteEntryAndValidatePayloadWithModel(entry, expectedPayload);
        }

        [Fact]
        public void WriteSingletonWithOnlyNavigationLinksTest()
        {
            this.NavigationLinkTestSetting();
            var entry = new ODataEntry { TypeName = "NS.Web" };
            string expectedPayload = "{" +
                "\"@odata.context\":\"http://odata.org/test/$metadata#MySingleton\"," +
                "\"@odata.type\":\"#NS.Web\"," +
                "\"@odata.id\":\"MySingleton\"," +
                "\"@odata.editLink\":\"MySingleton\"}";
            this.WriteEntryAndValidatePayloadWithoutModel(entry, expectedPayload);
            expectedPayload = "{" +
                "\"@odata.context\":\"http://odata.org/test/$metadata#MySingleton\"," +
                "\"@odata.type\":\"#NS.Web\"," +
                "\"@odata.id\":\"MySingleton\"," +
                "\"@odata.editLink\":\"MySingleton\"," +
                "\"Pages@odata.associationLink\":\"http://odata.org/test/MySingleton/Pages/$ref\"," +
                "\"Pages@odata.navigationLink\":\"http://odata.org/test/MySingleton/Pages\"}";
            this.WriteEntryAndValidatePayloadWithModel(entry, expectedPayload);
        }

        private void NavigationLinkTestSetting()
        {
            this.pageType = new EdmEntityType("NS", "Page");
            this.pageType.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32);
            this.pageType.AddStructuralProperty("PageLink", EdmPrimitiveTypeKind.String);
            this.referencedModel.AddElement(this.pageType);

            this.webType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Name = "Pages",
                Target = this.pageType,
                TargetMultiplicity = EdmMultiplicity.Many
            });
        }

        [Fact]
        public void WriteSingletonWhichHasBoundAction()
        {
            this.BoundActionTestSetting();
            var entry = new ODataEntry { TypeName = "NS.Web" };
            const string expectedPayload = "{" +
                "\"@odata.context\":\"http://odata.org/test/$metadata#MySingleton\"," +
                "\"@odata.type\":\"#NS.Web\"," +
                "\"@odata.id\":\"MySingleton\"," +
                "\"@odata.editLink\":\"MySingleton\"," +
                "\"#NS.SingletonAction\":{" +
                    "\"title\":\"NS.SingletonAction\"," +
                    "\"target\":\"http://odata.org/test/MySingleton/NS.SingletonAction\"" +
                    "}" +
                "}";
            this.WriteEntryAndValidatePayloadWithModel(entry, expectedPayload);
        }

        private void BoundActionTestSetting()
        {
            EdmAction action = new EdmAction("NS", "SingletonAction", null, true, null);
            action.AddParameter(new EdmOperationParameter(action, "p", new EdmEntityTypeReference(this.webType, false)));
            this.referencedModel.AddElement(action);
        }

        [Fact]
        public void WriteSingletonSteamPropertyWithDefaultValueTest()
        {
            this.StreamTestSetting();
            var entry = new ODataEntry { TypeName = "NS.Web" };
            entry.Properties = new[]
            {
                new ODataProperty { Name = "Logo", Value = new ODataStreamReferenceValue() }
            };
            const string expectedPayload = "{" +
                "\"@odata.context\":\"http://odata.org/test/$metadata#MySingleton\"," +
                "\"@odata.type\":\"#NS.Web\"," +
                "\"@odata.id\":\"MySingleton\"," +
                "\"@odata.editLink\":\"MySingleton\"," +
                "\"Logo@odata.mediaEditLink\":\"http://odata.org/test/MySingleton/Logo\"," +
                "\"Logo@odata.mediaReadLink\":\"http://odata.org/test/MySingleton/Logo\"}";
            this.WriteEntryAndValidatePayloadWithoutModel(entry, expectedPayload);
            this.WriteEntryAndValidatePayloadWithModel(entry, expectedPayload);
        }

        [Fact]
        public void WriteSingletonStreamPropertyWithValueSettingTest()
        {
            this.StreamTestSetting();
            var entry = new ODataEntry { TypeName = "NS.Web" };
            entry.Properties = new[]
            {
                new ODataProperty { 
                    Name = "Logo", 
                    Value = new ODataStreamReferenceValue()
                    {
                        ContentType = "image/jpeg", 
                        EditLink = new Uri("http://example.com/stream/edit"),
                        ReadLink = new Uri("http://example.com/stream/read"),
                        ETag = "stream etag"
                    } 
                }
            };
            const string expectedPayload = "{" +
                "\"@odata.context\":\"http://odata.org/test/$metadata#MySingleton\"," +
                "\"@odata.type\":\"#NS.Web\"," +
                "\"@odata.id\":\"MySingleton\"," +
                "\"@odata.editLink\":\"MySingleton\"," +
                "\"Logo@odata.mediaEditLink\":\"http://example.com/stream/edit\"," +
                "\"Logo@odata.mediaReadLink\":\"http://example.com/stream/read\"," +
                "\"Logo@odata.mediaContentType\":\"image/jpeg\"," +
                "\"Logo@odata.mediaEtag\":\"stream etag\"}";
            this.WriteEntryAndValidatePayloadWithoutModel(entry, expectedPayload);
            this.WriteEntryAndValidatePayloadWithModel(entry, expectedPayload);
        }

        private void StreamTestSetting()
        {
            referencedModel = new EdmModel();

            this.webType = new EdmEntityType("NS", "Web");
            this.webType.AddStructuralProperty("Logo", EdmPrimitiveTypeKind.Stream);
            referencedModel.AddElement(this.webType);

            this.defaultContainer = new EdmEntityContainer("NS", "DefaultContainer_sub");
            referencedModel.AddElement(defaultContainer);
            this.userModel = TestUtils.WrapReferencedModelsToMainModel("NS", "DefaultContainer", referencedModel);
            this.singleton = new EdmSingleton(defaultContainer, "MySingleton", this.webType);
            this.defaultContainer.AddElement(this.singleton);
        }

        [Fact]
        public void WriteSingletonAsMediaEntryTest()
        {
            this.MediaEntrySetSetting();
            var entry = new ODataEntry()
            {
                TypeName = "NS.Web",
                MediaResource = new ODataStreamReferenceValue()
                {
                    ContentType = "image/jpeg",
                    ReadLink = new Uri("http://Bla"),
                    ETag = "BlaBla"
                },
            };
            const string expectedPayload = "{" +
                "\"@odata.context\":\"http://odata.org/test/$metadata#MySingleton\"," +
                "\"@odata.type\":\"#NS.Web\"," +
                "\"@odata.id\":\"MySingleton\"," +
                "\"@odata.editLink\":\"MySingleton\"," +
                "\"@odata.mediaEditLink\":\"MySingleton/$value\"," +
                "\"@odata.mediaReadLink\":\"http://bla/\"," +
                "\"@odata.mediaContentType\":\"image/jpeg\"," +
                "\"@odata.mediaEtag\":\"BlaBla\"}";
            this.WriteEntryAndValidatePayloadWithoutModel(entry, expectedPayload);
            this.WriteEntryAndValidatePayloadWithModel(entry, expectedPayload);
        }

        private void MediaEntrySetSetting()
        {
            referencedModel = new EdmModel();

            this.webType = new EdmEntityType("NS", "Web", null, /*isAbstract*/false, /*isOpen*/false, /*hasStream*/true);
            referencedModel.AddElement(this.webType);

            this.defaultContainer = new EdmEntityContainer("NS", "DefaultContainer_sub");
            referencedModel.AddElement(defaultContainer);

            this.userModel = TestUtils.WrapReferencedModelsToMainModel("NS", "DefaultContainer", referencedModel);
            this.singleton = new EdmSingleton(defaultContainer, "MySingleton", this.webType);
            this.defaultContainer.AddElement(this.singleton);
        }

        [Fact]
        public void WriteSingletonWithEtagTest()
        {
            var entry = new ODataEntry()
            {
                TypeName = "NS.Web",
                ETag = "Bla"
            };

            const string expectedPayload = "{" +
                "\"@odata.context\":\"http://odata.org/test/$metadata#MySingleton\"," +
                "\"@odata.type\":\"#NS.Web\"," +
                "\"@odata.id\":\"MySingleton\"," +
                "\"@odata.etag\":\"Bla\"," +
                "\"@odata.editLink\":\"MySingleton\"}";

            this.WriteEntryAndValidatePayloadWithoutModel(entry, expectedPayload);
            this.WriteEntryAndValidatePayloadWithModel(entry, expectedPayload);
        }

        private void WriteEntryAndValidatePayloadWithoutModel(ODataEntry entry, string expectedPayload, bool writingResponse = true, bool setMetadataDocumentUri = true)
        {
            MemoryStream stream = new MemoryStream();
            ODataJsonLightOutputContext outputContext = CreateJsonLightOutputContext(stream, writingResponse, this.userModel, setMetadataDocumentUri ? this.serviceDocumentUri : null);

            ODataFeedAndEntrySerializationInfo serializationInfo = new ODataFeedAndEntrySerializationInfo
            {
                NavigationSourceName = "MySingleton",
                NavigationSourceEntityTypeName = "NS.Web",
                NavigationSourceKind = EdmNavigationSourceKind.Singleton,
            };

            entry.SerializationInfo = serializationInfo;
            ODataJsonLightWriter writer = new ODataJsonLightWriter(outputContext, /*navigationSource*/ null, /*entityType*/ null, /*writingFeed*/ false);
            WriteEntryAndValidatePayload(entry, stream, writer, expectedPayload);
        }

        private void WriteEntryAndValidatePayloadWithModel(ODataEntry entry, string expectedPayload, bool writingResponse = true, bool setMetadataDocumentUri = true)
        {
            MemoryStream stream = new MemoryStream();
            ODataJsonLightOutputContext outputContext = CreateJsonLightOutputContext(stream, writingResponse, this.userModel, setMetadataDocumentUri ? this.serviceDocumentUri : null);
            entry.SerializationInfo = null;
            ODataJsonLightWriter writer = new ODataJsonLightWriter(outputContext, /*navigationSource*/ singleton, /*entityType*/ webType, /*writingFeed*/ false);
            WriteEntryAndValidatePayload(entry, stream, writer, expectedPayload);
        }

        private static void WriteEntryAndValidatePayload(ODataEntry entry, MemoryStream stream, ODataJsonLightWriter writer, string expectedPayload)
        {
            writer.WriteStart(entry);
            writer.WriteEnd();
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            string payload = (new StreamReader(stream)).ReadToEnd();
            payload.Should().Be(expectedPayload);
        }

        private static ODataJsonLightOutputContext CreateJsonLightOutputContext(MemoryStream stream, bool writingResponse = true, IEdmModel userModel = null, Uri serviceDocumentUri = null)
        {
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings { Version = ODataVersion.V4, AutoComputePayloadMetadataInJson = true, ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*") };
            if (serviceDocumentUri != null)
            {
                settings.SetServiceDocumentUri(serviceDocumentUri);
            }

            ODataMediaType mediaType = new ODataMediaType("application", "json", new KeyValuePair<string, string>("odata.metadata", "full"));
            return new ODataJsonLightOutputContext(
                ODataFormat.Json,
                new NonDisposingStream(stream),
                mediaType,
                Encoding.UTF8,
                settings,
                writingResponse,
                /*synchronous*/ true,
                userModel ?? EdmCoreModel.Instance,
                /*urlResolver*/ null);
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="JsonLightReaderIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Reader.JsonLight
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.Test.OData.TDD.Tests.Common;
    using Microsoft.Test.OData.Utils.ODataLibTest;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class JsonLightReaderIntegrationTests
    {
        private static readonly IEdmModel Model;

        private static readonly EdmEntityType EntityType;

        private static readonly EdmEntitySet EntitySet;

        private const string ContextUrl = "\"@odata.context\":\"http://www.example.com/defaultService.svc/$metadata#Namespace.Container.EntitySet/$entity\"";

        private const string FullMetadata = "application/json;odata.metadata=full";

        private const string MinimalMetadata = "application/json;odata.metadata=minimal";

        static JsonLightReaderIntegrationTests()
        {
            EdmModel tmp = new EdmModel();
            EntityType = new EdmEntityType("Namespace", "EntityType");
            EdmEntityContainer edmEntityContainer = new EdmEntityContainer("Namespace", "Container_sub");
            EntitySet = edmEntityContainer.AddEntitySet("EntitySet", EntityType);
            tmp.AddElement(edmEntityContainer);
            tmp.Fixup();
            Model = TestUtils.WrapReferencedModelsToMainModel("Namespace", "Container", tmp);
        }

        [TestMethod]
        public void ShouldBeAbleToReadEntryWithIdOnly()
        {
            const string payload = "{" + ContextUrl + ",\"@odata.id\":\"entryId\"}";
            var actual = ReadJsonLightEntry(payload, FullMetadata, readingResponse: true);
            actual.Id.Should().Be("http://www.example.com/defaultService.svc/entryId");
        }

        [TestMethod]
        public void ShouldBeAbleToReadTransientEntryInFullMetadataLevel()
        {
            const string payload = "{" + ContextUrl + ",\"@odata.id\":null}";
            var actual = ReadJsonLightEntry(payload, FullMetadata, readingResponse: true);
            actual.IsTransient.Should().BeTrue();
        }

        [TestMethod]
        public void ShouldBeAbleToReadTransientEntryInMinimalMetadataLevel()
        {
            const string payload = "{" + ContextUrl + ",\"@odata.id\":null}";
            var actual = ReadJsonLightEntry(payload, MinimalMetadata, readingResponse: true);
            actual.IsTransient.Should().BeTrue();
        }

        [TestMethod]
        public void LinkReadingRequestShouldBeTheSameForJsonEntry()
        {
            const string payload = "{" + ContextUrl + ", " +
                "\"@odata.editLink\": \"http://www.example.com/defaultService.svc/edit\", " +
                "\"@odata.id\":\"entryId\"}";
            var actual = this.ReadJsonLightEntry(payload, FullMetadata, readingResponse: false);
            actual.Id.Should().Be(CreateUri("entryId"));
            actual.EditLink.Should().Be(CreateUri("http://www.example.com/defaultService.svc/edit"));
        }

        [TestMethod]
        public void LinkReadingResponseShouldBeReadAsAbsoluteForJsonEntry()
        {
            const string payload = "{" + ContextUrl + ", " +
                "\"@odata.editLink\": \"http://www.example.com/defaultService.svc/edit\", " +
                "\"@odata.id\":\"entryId\"}";
            var actual = this.ReadJsonLightEntry(payload, FullMetadata, readingResponse: true);
            actual.Id.Should().Be(CreateUri("http://www.example.com/defaultService.svc/entryId"));
            actual.EditLink.Should().Be(CreateUri("http://www.example.com/defaultService.svc/edit"));
        }

        [TestMethod]
        public void EditLinkShouldBeNullIfItsReadonlyEntry()
        {
            const string payload = "{" + ContextUrl + ", " +
                "\"@odata.readLink\": \"http://www.example.com/defaultService.svc/readonlyEntity\", " +
                "\"@odata.id\":\"entryId\"}";
            var actual = this.ReadJsonLightEntry(payload, FullMetadata, readingResponse: true);
            actual.Id.Should().Be(CreateUri("http://www.example.com/defaultService.svc/entryId"));
            actual.EditLink.Should().BeNull();
            actual.ReadLink.Should().Be(CreateUri("http://www.example.com/defaultService.svc/readonlyEntity"));
        }

        [TestMethod]
        public void LinkReadingRequestShouldBeTheSameForJsonFeed()
        {
            const string payload =
                "{" +
                    "\"@odata.context\":\"http://www.example.com/defaultService.svc/$metadata#Namespace.Container.EntitySet\", " +
                    "\"value\":[" +
                        "{" +
                          "\"@odata.context\":\"http://www.example.com/defaultService.svc/$metadata#Namespace.Container.EntitySet/$entity\", " +
                          "\"@odata.editLink\":\"http://www.example.com/defaultService.svc/edit1\", " +
                          "\"@odata.id\":\"entryId1\"" +
                     "}," +
                      "{" +
                          "\"@odata.editLink\":\"edit2\", " +
                          "\"@odata.id\":\"http://www.example.com/defaultService.svc/entryId2\"" +
                       "}" +
                    "]" +
                "}";
            var entries = this.ReadJsonLightFeed(payload, MinimalMetadata, readingResponse: false);
            entries[0].Id.Should().Be(CreateUri("entryId1"));
            entries[0].EditLink.Should().Be(CreateUri("http://www.example.com/defaultService.svc/edit1"));
            entries[1].Id.Should().Be(CreateUri("http://www.example.com/defaultService.svc/entryId2"));
            entries[1].EditLink.Should().Be(CreateUri("edit2"));
        }

        [TestMethod]
        public void LinkReadingResponseShouldBeReadAsAbsoluteForJsonFeed()
        {
            const string payload =
                "{" +
                    "\"@odata.context\":\"http://www.example.com/defaultService.svc/$metadata#Namespace.Container.EntitySet\", " +
                    "\"value\":[" +
                        "{" +
                          "\"@odata.context\":\"http://www.example.com/defaultService.svc/$metadata#Namespace.Container.EntitySet/$entity\", " +
                          "\"@odata.editLink\":\"edit1\", " +
                          "\"@odata.id\":\"http://www.example.com/defaultService.svc/entryId1\"" +
                     "}," +
                      "{" +
                          "\"@odata.editLink\":\"http://www.example.com/defaultService.svc/edit2\", " +
                          "\"@odata.id\":\"entryId2\"" +
                       "}" +
                    "]" +
                "}";
            var entries = this.ReadJsonLightFeed(payload, MinimalMetadata, readingResponse: true);
            entries[0].Id.Should().Be(CreateUri("http://www.example.com/defaultService.svc/entryId1"));
            entries[0].EditLink.Should().Be(CreateUri("http://www.example.com/defaultService.svc/edit1"));
            entries[1].Id.Should().Be(CreateUri("http://www.example.com/defaultService.svc/entryId2"));
            entries[1].EditLink.Should().Be(CreateUri("http://www.example.com/defaultService.svc/edit2"));
        }

        [TestMethod]
        public void UnknownAnnotationRequestShouldBeIgnored()
        {
            const string payload = "{" + ContextUrl + ", " +
                "\"@odata.unknown\": \"unknown\", " +
                "\"@odata.metadataEtag\": \"W\\\"A1FF3E230954908F\", " +
                "\"@odata.id\":\"entryId\"}";
            var actual = this.ReadJsonLightEntry(payload, FullMetadata, readingResponse: false);
            actual.Id.Should().Be(CreateUri("entryId"));
        }

        [TestMethod]
        public void UnknownAnnotationResponseShouldBeIgnored()
        {
            const string payload = "{" + ContextUrl + ", " +
                "\"@odata.unknown\": \"unknown\", " +
                "\"@odata.metadataEtag\": \"W/A1FF3E230954908F\", " +
                "\"@odata.id\":\"entryId\"}";
            var actual = this.ReadJsonLightEntry(payload, FullMetadata, readingResponse: true);
            actual.Id.Should().Be(CreateUri("http://www.example.com/defaultService.svc/entryId"));
        }

        private ODataEntry ReadJsonLightEntry(string payload, string contentType, bool readingResponse)
        {
            InMemoryMessage message = new InMemoryMessage();
            message.SetHeader("Content-Type", contentType);
            message.Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));

            ODataEntry topLevelEntry = null;

            using (var messageReader = readingResponse
                ? new ODataMessageReader((IODataResponseMessage)message, null, Model)
                : new ODataMessageReader((IODataRequestMessage)message, null, Model))
            {
                var reader = messageReader.CreateODataEntryReader(EntitySet, EntityType);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.EntryEnd:
                            topLevelEntry = (ODataEntry)reader.Item;
                            break;
                    }
                }
            }

            return topLevelEntry;
        }

        private IList<ODataEntry> ReadJsonLightFeed(string payload, string contentType, bool readingResponse)
        {
            InMemoryMessage message = new InMemoryMessage();
            message.SetHeader("Content-Type", contentType);
            message.Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));

            IList<ODataEntry> entries = new List<ODataEntry>();

            using (var messageReader = readingResponse
                ? new ODataMessageReader((IODataResponseMessage)message, null, Model)
                : new ODataMessageReader((IODataRequestMessage)message, null, Model))
            {
                var reader = messageReader.CreateODataFeedReader(EntitySet, EntityType);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.EntryEnd:
                            entries.Add((ODataEntry)reader.Item);
                            break;
                    }
                }
            }

            return entries;
        }

        private static Uri CreateUri(string uriValue)
        {
            return new Uri(uriValue, UriKind.RelativeOrAbsolute);
        }
    }
}

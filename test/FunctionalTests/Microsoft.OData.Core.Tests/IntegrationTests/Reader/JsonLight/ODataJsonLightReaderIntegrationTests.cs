//---------------------------------------------------------------------
// <copyright file="ODataJsonLightReaderIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Microsoft.Test.OData.Utils.ODataLibTest;
using Xunit;

namespace Microsoft.OData.Core.Tests.IntegrationTests.Reader.JsonLight
{
    public class ODataJsonLightReaderIntegrationTests
    {
        private static readonly IEdmModel Model;

        private static readonly EdmEntityType EntityType;

        private static readonly EdmEntitySet EntitySet;

        private const string ContextUrl = "\"@odata.context\":\"http://www.example.com/defaultService.svc/$metadata#Namespace.Container.EntitySet/$entity\"";

        private const string FullMetadata = "application/json;odata.metadata=full";

        private const string MinimalMetadata = "application/json;odata.metadata=minimal";

        static ODataJsonLightReaderIntegrationTests()
        {
            EdmModel tmp = new EdmModel();
            EntityType = new EdmEntityType("Namespace", "EntityType");
            EdmEntityContainer edmEntityContainer = new EdmEntityContainer("Namespace", "Container_sub");
            EntitySet = edmEntityContainer.AddEntitySet("EntitySet", EntityType);
            tmp.AddElement(edmEntityContainer);
            tmp.Fixup();
            Model = TestUtils.WrapReferencedModelsToMainModel("Namespace", "Container", tmp);
        }

        #region ShouldBeAbleToReadEntryWithIdOnly

        private void ShouldBeAbleToReadEntryWithIdOnlyImplementation(string payload, bool odataSimplified)
        {
            var actual = ReadJsonLightEntry(payload, FullMetadata, readingResponse: true, odataSimplified: odataSimplified);
            actual.Id.Should().Be("http://www.example.com/defaultService.svc/entryId");
        }

        [Fact]
        public void ShouldBeAbleToReadEntryWithIdOnly()
        {
            const string payload = "{" + ContextUrl + ",\"@odata.id\":\"entryId\"}";
            ShouldBeAbleToReadEntryWithIdOnlyImplementation(payload, false);
        }

        [Fact]
        public void ShouldBeAbleToReadEntryWithSimplifiedIdOnlyODataSimplified()
        {
            // cover "@id"
            const string payload = "{" + ContextUrl + ",\"@id\":\"entryId\"}";
            ShouldBeAbleToReadEntryWithIdOnlyImplementation(payload, true);
        }

        [Fact]
        public void ShouldBeAbleToReadEntryWithFullIdOnlyODataSimplified()
        {
            // cover "@odata.id"
            const string payload = "{" + ContextUrl + ",\"@odata.id\":\"entryId\"}";
            ShouldBeAbleToReadEntryWithIdOnlyImplementation(payload, true);
        }

        #endregion

        [Fact]
        public void ShouldBeAbleToReadTransientEntryInFullMetadataLevel()
        {
            const string payload = "{" + ContextUrl + ",\"@odata.id\":null}";
            var actual = ReadJsonLightEntry(payload, FullMetadata, readingResponse: true);
            actual.IsTransient.Should().BeTrue();
        }

        [Fact]
        public void ShouldBeAbleToReadTransientEntryInMinimalMetadataLevel()
        {
            const string payload = "{" + ContextUrl + ",\"@odata.id\":null}";
            var actual = ReadJsonLightEntry(payload, MinimalMetadata, readingResponse: true);
            actual.IsTransient.Should().BeTrue();
        }

        #region LinkReadingRequestShouldBeTheSameForJsonEntry

        private void LinkReadingRequestShouldBeTheSameForJsonEntryImplementation(string payload, bool odataSimplified)
        {
            var actual = this.ReadJsonLightEntry(payload, FullMetadata, readingResponse: false, odataSimplified: odataSimplified);
            actual.Id.Should().Be(CreateUri("entryId"));
            actual.EditLink.Should().Be(CreateUri("http://www.example.com/defaultService.svc/edit"));
        }

        [Fact]
        public void LinkReadingRequestShouldBeTheSameForJsonEntry()
        {
            const string payload = "{" + ContextUrl + ", " +
                "\"@odata.editLink\": \"http://www.example.com/defaultService.svc/edit\", " +
                "\"@odata.id\":\"entryId\"}";
            LinkReadingRequestShouldBeTheSameForJsonEntryImplementation(payload, false);
        }

        [Fact]
        public void LinkReadingRequestShouldBeTheSameForJsonEntrySimplifiedEditLinkODataSimplified()
        {
            // cover "@editLink"
            const string payload = "{" + ContextUrl + ", " +
                "\"@editLink\": \"http://www.example.com/defaultService.svc/edit\", " +
                "\"@id\":\"entryId\"}";
            LinkReadingRequestShouldBeTheSameForJsonEntryImplementation(payload, true);
        }

        [Fact]
        public void LinkReadingRequestShouldBeTheSameForJsonEntryFullEditLinkODataSimplified()
        {
            // cover "@odata.editLink"
            const string payload = "{" + ContextUrl + ", " +
                "\"@odata.editLink\": \"http://www.example.com/defaultService.svc/edit\", " +
                "\"@odata.id\":\"entryId\"}";
            LinkReadingRequestShouldBeTheSameForJsonEntryImplementation(payload, true);
        }

        #endregion

        [Fact]
        public void LinkReadingResponseShouldBeReadAsAbsoluteForJsonEntry()
        {
            const string payload = "{" + ContextUrl + ", " +
                "\"@odata.editLink\": \"http://www.example.com/defaultService.svc/edit\", " +
                "\"@odata.id\":\"entryId\"}";
            var actual = this.ReadJsonLightEntry(payload, FullMetadata, readingResponse: true);
            actual.Id.Should().Be(CreateUri("http://www.example.com/defaultService.svc/entryId"));
            actual.EditLink.Should().Be(CreateUri("http://www.example.com/defaultService.svc/edit"));
        }

        #region EditLinkShouldBeNullIfItsReadonlyEntry

        private void EditLinkShouldBeNullIfItsReadonlyEntryImplmentation(string payload, bool odataSimplified)
        {
            var actual = this.ReadJsonLightEntry(payload, FullMetadata, readingResponse: true, odataSimplified: odataSimplified);
            actual.Id.Should().Be(CreateUri("http://www.example.com/defaultService.svc/entryId"));
            actual.EditLink.Should().BeNull();
            actual.ReadLink.Should().Be(CreateUri("http://www.example.com/defaultService.svc/readonlyEntity"));
        }

        [Fact]
        public void EditLinkShouldBeNullIfItsReadonlyEntry()
        {
            const string payload = "{" + ContextUrl + ", " +
                "\"@odata.readLink\": \"http://www.example.com/defaultService.svc/readonlyEntity\", " +
                "\"@odata.id\":\"entryId\"}";
            EditLinkShouldBeNullIfItsReadonlyEntryImplmentation(payload, odataSimplified: false);
        }

        [Fact]
        public void EditLinkShouldBeNullIfItsReadonlyEntrySimplifiedReadLinkODataSimplified()
        {
            // cover "@readLink"
            const string payload = "{" + ContextUrl + ", " +
                "\"@readLink\": \"http://www.example.com/defaultService.svc/readonlyEntity\", " +
                "\"@id\":\"entryId\"}";
            EditLinkShouldBeNullIfItsReadonlyEntryImplmentation(payload, odataSimplified: true);
        }

        [Fact]
        public void EditLinkShouldBeNullIfItsReadonlyEntryFullReadLinkODataSimplified()
        {
            // cover "@odata.readLink"
            const string payload = "{" + ContextUrl + ", " +
                "\"@odata.readLink\": \"http://www.example.com/defaultService.svc/readonlyEntity\", " +
                "\"@odata.id\":\"entryId\"}";
            EditLinkShouldBeNullIfItsReadonlyEntryImplmentation(payload, odataSimplified: true);
        }

        #endregion

        [Fact]
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

        [Fact]
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

        [Fact]
        public void UnknownAnnotationRequestShouldBeIgnored()
        {
            const string payload = "{" + ContextUrl + ", " +
                "\"@odata.unknown\": \"unknown\", " +
                "\"@odata.metadataEtag\": \"W\\\"A1FF3E230954908F\", " +
                "\"@odata.id\":\"entryId\"}";
            var actual = this.ReadJsonLightEntry(payload, FullMetadata, readingResponse: false);
            actual.Id.Should().Be(CreateUri("entryId"));
        }

        [Fact]
        public void UnknownAnnotationResponseShouldBeIgnored()
        {
            const string payload = "{" + ContextUrl + ", " +
                "\"@odata.unknown\": \"unknown\", " +
                "\"@odata.metadataEtag\": \"W/A1FF3E230954908F\", " +
                "\"@odata.id\":\"entryId\"}";
            var actual = this.ReadJsonLightEntry(payload, FullMetadata, readingResponse: true);
            actual.Id.Should().Be(CreateUri("http://www.example.com/defaultService.svc/entryId"));
        }

        private ODataEntry ReadJsonLightEntry(string payload, string contentType, bool readingResponse, bool odataSimplified = false)
        {
            InMemoryMessage message = new InMemoryMessage();
            message.SetHeader("Content-Type", contentType);
            message.Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));

            ODataEntry topLevelEntry = null;

            ODataMessageReaderSettings settings = new ODataMessageReaderSettings { ODataSimplified = odataSimplified };

            using (var messageReader = readingResponse
                ? new ODataMessageReader((IODataResponseMessage)message, settings, Model)
                : new ODataMessageReader((IODataRequestMessage)message, settings, Model))
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

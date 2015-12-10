//---------------------------------------------------------------------
// <copyright file="ODataAtomWriterIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using Microsoft.OData.Core.Atom;
using Microsoft.OData.Edm.Library;
using Microsoft.Test.OData.Utils.ODataLibTest;
using Xunit;

namespace Microsoft.OData.Core.Tests.IntegrationTests.Writer.Atom
{
    public class ODataAtomWriterIntegrationTests
    {
        private static readonly EdmModel Model;
        private static readonly EdmEntitySet EntitySet;
        private static readonly EdmEntityType EntityType;

        static ODataAtomWriterIntegrationTests()
        {
            Model = new EdmModel();
            EntityType = new EdmEntityType("Namespace", "EntityType");

            Model.AddElement(EntityType);
            var entityContainer = new EdmEntityContainer("Namespace", "Container");
            EntitySet = entityContainer.AddEntitySet("EntitySet", EntityType);
            Model.AddElement(entityContainer);
            Model.Fixup();
        }


        [Fact]
        public void ReadLinkShouldNotBeOmittedWhenNotIdenticalToEditLink()
        {
            DateTimeOffset updatedTime = DateTimeOffset.UtcNow;
            var entry = new ODataEntry
            {
                Id = new Uri("http://test.org/EntitySet('1')"),
                EditLink = new Uri("http://test.org/EntitySet('1')/edit"),
                ReadLink = new Uri("http://test.org/EntitySet('1')/read")
            };
            entry.SetAnnotation(new AtomEntryMetadata() { Updated = updatedTime });
            string actual = this.WriteAtomEntry(entry);
            string expected = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns:georss=\"http://www.georss.org/georss\" xmlns:gml=\"http://www.opengis.net/gml\" m:context=\"http://temp.org/$metadata#EntitySet/$entity\">" +
                    "<id>http://test.org/EntitySet('1')</id>" +
                    "<link rel=\"edit\" href=\"http://test.org/EntitySet('1')/edit\" />" +
                    "<link rel=\"self\" href=\"http://test.org/EntitySet('1')/read\" />" +
                    "<title />" +
                    "<updated>" + ODataAtomConvert.ToAtomString(updatedTime) + "</updated>" +
                    "<author>" +
                        "<name />" +
                    "</author>" +
                    "<content type=\"application/xml\" />" +
                "</entry>";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ReadLinkShouldBeOmittedWhenIdenticalToEditLink()
        {
            DateTimeOffset updatedTime = DateTimeOffset.UtcNow;
            var entry = new ODataEntry
            {
                Id = new Uri("http://test.org/EntitySet('1')"),
                EditLink = new Uri("http://test.org/EntitySet('1')"),
                ReadLink = new Uri("http://test.org/EntitySet('1')")
            };
            entry.SetAnnotation(new AtomEntryMetadata() { Updated = updatedTime });
            string actual = this.WriteAtomEntry(entry);
            string expected = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns:georss=\"http://www.georss.org/georss\" xmlns:gml=\"http://www.opengis.net/gml\" m:context=\"http://temp.org/$metadata#EntitySet/$entity\">" +
                    "<id>http://test.org/EntitySet('1')</id>" +
                    "<link rel=\"edit\" href=\"http://test.org/EntitySet('1')\" />" +
                    "<title />" +
                    "<updated>" + ODataAtomConvert.ToAtomString(updatedTime) + "</updated>" +
                    "<author>" +
                        "<name />" +
                    "</author>" +
                    "<content type=\"application/xml\" />" +
                "</entry>";
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ReadEditLinkShouldBeOmittedWhenNeitherSetInAtom()
        {
            DateTimeOffset updatedTime = DateTimeOffset.UtcNow;
            var entry = new ODataEntry
            {
                Id = new Uri("http://test.org/EntitySet('1')")
            };
            entry.SetAnnotation(new AtomEntryMetadata() { Updated = updatedTime });
            string actual = this.WriteAtomEntry(entry);
            string expected = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns:georss=\"http://www.georss.org/georss\" xmlns:gml=\"http://www.opengis.net/gml\" m:context=\"http://temp.org/$metadata#EntitySet/$entity\">" +
                    "<id>http://test.org/EntitySet('1')</id>" +
                    "<title />" +
                    "<updated>" + ODataAtomConvert.ToAtomString(updatedTime) + "</updated>" +
                    "<author>" +
                        "<name />" +
                    "</author>" +
                    "<content type=\"application/xml\" />" +
                "</entry>";
            Assert.Equal(expected, actual);
        }

        private string WriteAtomEntry(ODataEntry entry)
        {
            MemoryStream stream = new MemoryStream();
            InMemoryMessage message = new InMemoryMessage { Stream = stream };

            var settings = new ODataMessageWriterSettings { Version = ODataVersion.V4, AutoComputePayloadMetadataInJson = true, EnableAtom = true};
            settings.SetServiceDocumentUri(new Uri("http://temp.org/"));

            settings.SetContentType(ODataFormat.Atom);
            

            ODataMessageWriter messageWriter = new ODataMessageWriter((IODataResponseMessage)message, settings, Model);

            var entryWriter = messageWriter.CreateODataEntryWriter(EntitySet, EntityType);

            entryWriter.WriteStart(entry);
            entryWriter.WriteEnd();
            entryWriter.Flush();

            var actual = Encoding.UTF8.GetString(stream.ToArray());
            return actual;
        }
    }
}

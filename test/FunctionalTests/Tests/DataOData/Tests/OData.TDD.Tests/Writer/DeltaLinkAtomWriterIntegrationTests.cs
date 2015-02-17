//---------------------------------------------------------------------
// <copyright file="DeltaLinkAtomWriterIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Writer
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.Test.OData.TDD.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DeltaLinkJonLightWriterIntegrationTests : DeltaLinkWriterIntegrationTests
    {
        #region Writing delta link on top-level feeds

        [TestMethod]
        public void WriteStartShouldIgnoreDeltaLinkWhenWritingRequestTopLevelFeed()
        {
            string expectedPayload = 
            @"<?xml version=""1.0"" encoding=""utf-8""?>"
                +@"<feed xmlns=""http://www.w3.org/2005/Atom"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns:georss=""http://www.georss.org/georss"" xmlns:gml=""http://www.opengis.net/gml"" m:context=""http://www.example.com/$metadata#TestEntitySet"">"
                    + "<id>http://host/TestEntitySet</id>"
                    + "<title />";

            Action<ODataWriter> deltaLinkAtWriteStart = (odataWriter) =>
            {
                var feedToWrite = new ODataFeed { Id = new Uri("http://host/TestEntitySet", UriKind.Absolute), DeltaLink = new Uri("http://host/deltaLink", UriKind.Absolute) };
                odataWriter.WriteStart(feedToWrite);
            };

            WriteAnnotationsAndValidatePayload(deltaLinkAtWriteStart, ODataFormat.Atom, expectedPayload, request: true, createFeedWriter: true);
        }

        [TestMethod]
        public void WriteEndShouldIgnoreDeltaLinkWhenWritingRequestTopLevelFeed()
        {
            string expectedPayload =
                @"<?xml version=""1.0"" encoding=""utf-8""?>"
                + @"<feed xmlns=""http://www.w3.org/2005/Atom"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns:georss=""http://www.georss.org/georss"" xmlns:gml=""http://www.opengis.net/gml"" m:context=""http://www.example.com/$metadata#TestEntitySet"">"
                    + "<id>http://host/TestEntitySet</id>"
                    + "<title /><author><name /></author>"
                + "</feed>";

            Action<ODataWriter> deltaLinkAtWriteStart = (odataWriter) =>
            {
                var feedToWrite = new ODataFeed() { Id = new Uri("http://host/TestEntitySet", UriKind.Absolute) };
                odataWriter.WriteStart(feedToWrite);
                feedToWrite.DeltaLink = new Uri("http://host/deltaLink", UriKind.Absolute);
                odataWriter.WriteEnd();
            };

            WriteAnnotationsAndValidatePayload(deltaLinkAtWriteStart, ODataFormat.Atom, expectedPayload, request: true, createFeedWriter: true);
        }

        [TestMethod]
        public void WriteStartShouldIgnoreDeltaLinkWhenWritingResponseTopLevelFeed()
        {
            string expectedPayload =
                 @"<?xml version=""1.0"" encoding=""utf-8""?>"
                + @"<feed xmlns=""http://www.w3.org/2005/Atom"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns:georss=""http://www.georss.org/georss"" xmlns:gml=""http://www.opengis.net/gml"" m:context=""http://www.example.com/$metadata#TestEntitySet"">"
                    + "<id>http://host/TestEntitySet</id>"
                    + "<title />";

            Action<ODataWriter> deltaLinkAtWriteStart = (odataWriter) =>
            {
                var feedToWrite = new ODataFeed { Id = new Uri("http://host/TestEntitySet", UriKind.Absolute), DeltaLink = new Uri("http://host/deltaLink", UriKind.Absolute) };
                odataWriter.WriteStart(feedToWrite);
            };

            WriteAnnotationsAndValidatePayload(deltaLinkAtWriteStart, ODataFormat.Atom, expectedPayload, request: false, createFeedWriter: true);
        }

        [TestMethod]
        public void WriteEndShouldWriteDeltaLinkWhenWritingResponseTopLevelFeed()
        {
            string expectedPayload =
                  @"<?xml version=""1.0"" encoding=""utf-8""?>"
                + @"<feed xmlns=""http://www.w3.org/2005/Atom"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns:georss=""http://www.georss.org/georss"" xmlns:gml=""http://www.opengis.net/gml"" m:context=""http://www.example.com/$metadata#TestEntitySet"">"
                    + "<id>http://host/TestEntitySet</id>"
                    + "<title /><author><name /></author>"
                    + @"<link rel=""http://docs.oasis-open.org/odata/ns/delta"" href=""http://host/deltaLink"" />"
                + "</feed>";

            Action<ODataWriter> deltaLinkAtWriteStart = (odataWriter) =>
            {
                var feedToWrite = new ODataFeed() { Id = new Uri("http://host/TestEntitySet", UriKind.Absolute) };
                odataWriter.WriteStart(feedToWrite);
                feedToWrite.DeltaLink = new Uri("http://host/deltaLink", UriKind.Absolute);
                odataWriter.WriteEnd();
            };

            WriteAnnotationsAndValidatePayload(deltaLinkAtWriteStart, ODataFormat.Atom, expectedPayload, request: false, createFeedWriter: true);
        }

        #endregion Writing delta link on top-level feeds

        #region Writing delta link on expanded feeds

        [TestMethod]
        public void WriteStartOnExpandedFeedWithDeltaLinkShouldIgnoreDeltaLink()
        {
            string expectedPayload = 
                @"<?xml version=""1.0"" encoding=""utf-8""?>"
                + @"<entry xmlns=""http://www.w3.org/2005/Atom"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"" xmlns:georss=""http://www.georss.org/georss"" xmlns:gml=""http://www.opengis.net/gml"" m:context=""http://www.example.com/$metadata#TestEntitySet/$entity"">"
                + @"<link rel=""http://docs.oasis-open.org/odata/ns/related/ResourceSetNavigationProperty"" type=""application/atom+xml;type=feed"" title=""ResourceSetNavigationProperty"" href=""http://host/navProp"">"
                    +"<m:inline><feed><id>http://host/TestEntitySet</id><title />";

            Action<ODataWriter> deltaLinkAtWriteStart = (odataWriter) =>
            {
                var entryToWrite = new ODataEntry { Properties = new[] { new ODataProperty { Name = "ID", Value = 1 } } };
                odataWriter.WriteStart(entryToWrite);

                ODataNavigationLink navLink = new ODataNavigationLink { Name = "ResourceSetNavigationProperty", IsCollection = true, Url = new Uri("http://host/navProp")  };
                odataWriter.WriteStart(navLink);

                var feedToWrite = new ODataFeed() { Id = new Uri("http://host/TestEntitySet", UriKind.Absolute) };
                feedToWrite.DeltaLink = new Uri("http://host/relative", UriKind.Absolute);
                odataWriter.WriteStart(feedToWrite);
            };

            WriteAnnotationsAndValidatePayload(deltaLinkAtWriteStart, ODataFormat.Atom, expectedPayload, request: true, createFeedWriter: false);
            WriteAnnotationsAndValidatePayload(deltaLinkAtWriteStart, ODataFormat.Atom, expectedPayload, request: false, createFeedWriter: false);
        }

        [TestMethod]
        public void WriteEndOnExpandedFeedWithDeltaLinkShouldThrow()
        {
            Action<ODataWriter> deltaLinkAtWriteStart = (odataWriter) =>
            {
                var entryToWrite = new ODataEntry { Properties = new[] { new ODataProperty { Name = "ID", Value = 1 } } };
                odataWriter.WriteStart(entryToWrite);

                ODataNavigationLink navLink = new ODataNavigationLink { Name = "ResourceSetNavigationProperty", IsCollection = true, Url = new Uri("http://host/navProp") };
                odataWriter.WriteStart(navLink);

                var feedToWrite = new ODataFeed() { Id = new Uri("http://host/TestEntitySet", UriKind.Absolute) };
                feedToWrite.DeltaLink = new Uri("http://host/relative", UriKind.Absolute);
                odataWriter.WriteStart(feedToWrite);
                odataWriter.WriteEnd();
            };

            Action requestTest = () => WriteAnnotationsAndValidatePayload(deltaLinkAtWriteStart, ODataFormat.Atom, string.Empty, request: true, createFeedWriter: false);
            requestTest.ShouldThrow<ODataException>().WithMessage(Strings.ODataWriterCore_DeltaLinkNotSupportedOnExpandedFeed);

            Action responseTest = () => WriteAnnotationsAndValidatePayload(deltaLinkAtWriteStart, ODataFormat.Atom, string.Empty, request: false, createFeedWriter: false);
            responseTest.ShouldThrow<ODataException>().WithMessage(Strings.ODataWriterCore_DeltaLinkNotSupportedOnExpandedFeed);
        }

        #endregion Writing delta link on expanded feeds

    }
}

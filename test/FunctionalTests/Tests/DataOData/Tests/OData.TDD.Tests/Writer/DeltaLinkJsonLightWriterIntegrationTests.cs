//---------------------------------------------------------------------
// <copyright file="DeltaLinkJsonLightWriterIntegrationTests.cs" company="Microsoft">
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
    public class DeltaLinkJsonLightWriterIntegrationTests : DeltaLinkWriterIntegrationTests
    {
        #region Writing delta link on top-level feeds

        [TestMethod]
        public void WriteStartShouldIgnoreDeltaLinkWhenWritingRequestTopLevelFeed()
        {
            string expectedPayload =
            "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#TestEntitySet\"," +
                "\"value\":[]" +
            "}";

            Action<ODataWriter> deltaLinkAtWriteStart = (odataWriter) =>
            {
                var feedToWrite = new ODataFeed { DeltaLink = new Uri("http://host/deltaLink", UriKind.Absolute) };
                odataWriter.WriteStart(feedToWrite);
                odataWriter.WriteEnd();
            };

            WriteAnnotationsAndValidatePayload(deltaLinkAtWriteStart, ODataFormat.Json, expectedPayload, request: true, createFeedWriter: true);
        }

        [TestMethod]
        public void WriteEndShouldIgnoreDeltaLinkWhenWritingRequestTopLevelFeed()
        {
            string expectedPayload =
            "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#TestEntitySet\"," +
                "\"value\":[]" +
            "}";

            Action<ODataWriter> deltaLinkAtWriteStart = (odataWriter) =>
            {
                var feedToWrite = new ODataFeed();
                odataWriter.WriteStart(feedToWrite);
                feedToWrite.DeltaLink = new Uri("http://host/deltaLink", UriKind.Absolute);
                odataWriter.WriteEnd();
            };

            WriteAnnotationsAndValidatePayload(deltaLinkAtWriteStart, ODataFormat.Json, expectedPayload, request: true, createFeedWriter: true);
        }

        [TestMethod]
        public void WriteStartShouldWriteDeltaLinkWhenWritingResponseTopLevelFeed()
        {
            string expectedPayload =
            "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#TestEntitySet\"," +
                "\"@odata.deltaLink\":\"http://host/deltaLink\"," +
                "\"value\":[]" +
            "}";

            Action<ODataWriter> deltaLinkAtWriteStart = (odataWriter) =>
            {
                var feedToWrite = new ODataFeed { DeltaLink = new Uri("http://host/deltaLink", UriKind.Absolute) };
                odataWriter.WriteStart(feedToWrite);
                odataWriter.WriteEnd();
            };

            WriteAnnotationsAndValidatePayload(deltaLinkAtWriteStart, ODataFormat.Json, expectedPayload, request: false, createFeedWriter: true);
        }

        [TestMethod]
        public void WriteEndShouldWriteDeltaLinkWhenWritingResponseTopLevelFeed()
        {
            string expectedPayload =
            "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#TestEntitySet\"," +
                "\"value\":[]," +
                "\"@odata.deltaLink\":\"deltaLink\"" +
            "}";

            Action<ODataWriter> deltaLinkAtWriteStart = (odataWriter) =>
            {
                var feedToWrite = new ODataFeed();
                odataWriter.WriteStart(feedToWrite);
                feedToWrite.DeltaLink = new Uri("deltaLink", UriKind.Relative);
                odataWriter.WriteEnd();
            };

            WriteAnnotationsAndValidatePayload(deltaLinkAtWriteStart, ODataFormat.Json, expectedPayload, request: false, createFeedWriter: true);
        }

        #endregion Writing delta link on top-level feeds

        #region Writing delta link on expanded feeds

        [TestMethod]
        public void WriteStartOnExpandedFeedWithDeltaLinkShouldThrow()
        {
            Action<ODataWriter> deltaLinkAtWriteStart = (odataWriter) =>
            {
                var entryToWrite = new ODataEntry { Properties = new[] { new ODataProperty { Name = "ID", Value = 1 } }};
                odataWriter.WriteStart(entryToWrite);

                ODataNavigationLink navLink = new ODataNavigationLink { Name = "ResourceSetNavigationProperty", IsCollection = true };
                odataWriter.WriteStart(navLink);

                    var feedToWrite = new ODataFeed();
                    feedToWrite.DeltaLink = new Uri("relative", UriKind.Relative);
                    odataWriter.WriteStart(feedToWrite);
            };

            Action requestTest = () => WriteAnnotationsAndValidatePayload(deltaLinkAtWriteStart, ODataFormat.Json, string.Empty, request: true, createFeedWriter: false);
            requestTest.ShouldThrow<ODataException>().WithMessage(Strings.ODataWriterCore_DeltaLinkNotSupportedOnExpandedFeed);

            Action responseTest = () => WriteAnnotationsAndValidatePayload(deltaLinkAtWriteStart, ODataFormat.Json, string.Empty, request: false, createFeedWriter: false);
            responseTest.ShouldThrow<ODataException>().WithMessage(Strings.ODataWriterCore_DeltaLinkNotSupportedOnExpandedFeed);
        }

        [TestMethod]
        public void WriteEndOnExpandedFeedWithDeltaLinkShouldThrow()
        {
            Action<ODataWriter> deltaLinkAtWriteEnd = (odataWriter) =>
            {
                var entryToWrite = new ODataEntry { Properties = new[] { new ODataProperty { Name = "ID", Value = 1 } }};
                odataWriter.WriteStart(entryToWrite);

                ODataNavigationLink navLink = new ODataNavigationLink { Name = "ResourceSetNavigationProperty", IsCollection = true };
                odataWriter.WriteStart(navLink);

                    var feedToWrite = new ODataFeed();
                    odataWriter.WriteStart(feedToWrite);
                    feedToWrite.DeltaLink = new Uri("relative", UriKind.Relative);
                    odataWriter.WriteEnd();
            };

            Action requestTest = () => WriteAnnotationsAndValidatePayload(deltaLinkAtWriteEnd, ODataFormat.Json, string.Empty, request: true, createFeedWriter: false);
            requestTest.ShouldThrow<ODataException>().WithMessage(Strings.ODataWriterCore_DeltaLinkNotSupportedOnExpandedFeed);

            Action responseTest = () => WriteAnnotationsAndValidatePayload(deltaLinkAtWriteEnd, ODataFormat.Json, string.Empty, request: false, createFeedWriter: false);
            responseTest.ShouldThrow<ODataException>().WithMessage(Strings.ODataWriterCore_DeltaLinkNotSupportedOnExpandedFeed);
        }

        #endregion Writing delta link on expanded feeds

    }
}

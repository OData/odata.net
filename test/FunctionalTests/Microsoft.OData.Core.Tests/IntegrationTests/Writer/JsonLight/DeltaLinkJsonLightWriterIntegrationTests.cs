//---------------------------------------------------------------------
// <copyright file="DeltaLinkJsonLightWriterIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Xunit;

namespace Microsoft.OData.Tests.IntegrationTests.Writer.JsonLight
{
    public class DeltaLinkJsonLightWriterIntegrationTests : DeltaLinkWriterIntegrationTests
    {
        #region Writing delta link on top-level feeds

        [Fact]
        public void WriteStartShouldIgnoreDeltaLinkWhenWritingRequestTopLevelFeed()
        {
            string expectedPayload =
            "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#TestEntitySet\"," +
                "\"value\":[]" +
            "}";

            Action<ODataWriter> deltaLinkAtWriteStart = (odataWriter) =>
            {
                var feedToWrite = new ODataResourceSet { DeltaLink = new Uri("http://host/deltaLink", UriKind.Absolute) };
                odataWriter.WriteStart(feedToWrite);
                odataWriter.WriteEnd();
            };

            WriteAnnotationsAndValidatePayload(deltaLinkAtWriteStart, ODataFormat.Json, expectedPayload, request: true, createFeedWriter: true);
        }

        [Fact]
        public void WriteEndShouldIgnoreDeltaLinkWhenWritingRequestTopLevelFeed()
        {
            string expectedPayload =
            "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#TestEntitySet\"," +
                "\"value\":[]" +
            "}";

            Action<ODataWriter> deltaLinkAtWriteStart = (odataWriter) =>
            {
                var feedToWrite = new ODataResourceSet();
                odataWriter.WriteStart(feedToWrite);
                feedToWrite.DeltaLink = new Uri("http://host/deltaLink", UriKind.Absolute);
                odataWriter.WriteEnd();
            };

            WriteAnnotationsAndValidatePayload(deltaLinkAtWriteStart, ODataFormat.Json, expectedPayload, request: true, createFeedWriter: true);
        }

        [Fact]
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
                var feedToWrite = new ODataResourceSet { DeltaLink = new Uri("http://host/deltaLink", UriKind.Absolute) };
                odataWriter.WriteStart(feedToWrite);
                odataWriter.WriteEnd();
            };

            WriteAnnotationsAndValidatePayload(deltaLinkAtWriteStart, ODataFormat.Json, expectedPayload, request: false, createFeedWriter: true);
        }

        [Fact]
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
                var feedToWrite = new ODataResourceSet();
                odataWriter.WriteStart(feedToWrite);
                feedToWrite.DeltaLink = new Uri("deltaLink", UriKind.Relative);
                odataWriter.WriteEnd();
            };

            WriteAnnotationsAndValidatePayload(deltaLinkAtWriteStart, ODataFormat.Json, expectedPayload, request: false, createFeedWriter: true);
        }

        #endregion Writing delta link on top-level feeds

        #region Writing delta link on expanded feeds

        [Fact]
        public void WriteStartOnExpandedFeedWithDeltaLinkShouldThrow()
        {
            Action<ODataWriter> deltaLinkAtWriteStart = (odataWriter) =>
            {
                var entryToWrite = new ODataResource { Properties = new[] { new ODataProperty { Name = "ID", Value = 1 } }};
                odataWriter.WriteStart(entryToWrite);

                ODataNestedResourceInfo navLink = new ODataNestedResourceInfo { Name = "ResourceSetNavigationProperty", IsCollection = true };
                odataWriter.WriteStart(navLink);

                    var feedToWrite = new ODataResourceSet();
                    feedToWrite.DeltaLink = new Uri("relative", UriKind.Relative);
                    odataWriter.WriteStart(feedToWrite);
            };

            Action requestTest = () => WriteAnnotationsAndValidatePayload(deltaLinkAtWriteStart, ODataFormat.Json, string.Empty, request: true, createFeedWriter: false);
            requestTest.ShouldThrow<ODataException>().WithMessage(Strings.ODataWriterCore_DeltaLinkNotSupportedOnExpandedResourceSet);

            Action responseTest = () => WriteAnnotationsAndValidatePayload(deltaLinkAtWriteStart, ODataFormat.Json, string.Empty, request: false, createFeedWriter: false);
            responseTest.ShouldThrow<ODataException>().WithMessage(Strings.ODataWriterCore_DeltaLinkNotSupportedOnExpandedResourceSet);
        }

        [Fact]
        public void WriteEndOnExpandedFeedWithDeltaLinkShouldThrow()
        {
            Action<ODataWriter> deltaLinkAtWriteEnd = (odataWriter) =>
            {
                var entryToWrite = new ODataResource { Properties = new[] { new ODataProperty { Name = "ID", Value = 1 } }};
                odataWriter.WriteStart(entryToWrite);

                ODataNestedResourceInfo navLink = new ODataNestedResourceInfo { Name = "ResourceSetNavigationProperty", IsCollection = true };
                odataWriter.WriteStart(navLink);

                    var feedToWrite = new ODataResourceSet();
                    odataWriter.WriteStart(feedToWrite);
                    feedToWrite.DeltaLink = new Uri("relative", UriKind.Relative);
                    odataWriter.WriteEnd();
            };

            Action requestTest = () => WriteAnnotationsAndValidatePayload(deltaLinkAtWriteEnd, ODataFormat.Json, string.Empty, request: true, createFeedWriter: false);
            requestTest.ShouldThrow<ODataException>().WithMessage(Strings.ODataWriterCore_DeltaLinkNotSupportedOnExpandedResourceSet);

            Action responseTest = () => WriteAnnotationsAndValidatePayload(deltaLinkAtWriteEnd, ODataFormat.Json, string.Empty, request: false, createFeedWriter: false);
            responseTest.ShouldThrow<ODataException>().WithMessage(Strings.ODataWriterCore_DeltaLinkNotSupportedOnExpandedResourceSet);
        }

        #endregion Writing delta link on expanded feeds

    }
}

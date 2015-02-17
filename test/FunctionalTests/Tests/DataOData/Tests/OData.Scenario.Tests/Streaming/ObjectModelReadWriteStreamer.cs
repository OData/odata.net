//---------------------------------------------------------------------
// <copyright file="ObjectModelReadWriteStreamer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Scenario.Tests.Streaming
{
    using System;
    using Microsoft.OData.Core;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;

    class ObjectModelReadWriteStreamer
    {
        private WriterTestConfiguration testConfiguration;

        public void StreamMessage(ODataMessageReaderTestWrapper reader, ODataMessageWriterTestWrapper writer, ODataPayloadKind payloadKind, WriterTestConfiguration config)
        {
            ExceptionUtilities.CheckArgumentNotNull(reader, "reader is required");
            ExceptionUtilities.CheckArgumentNotNull(writer, "writer is required");
            ExceptionUtilities.CheckArgumentNotNull(payloadKind, "payloadKind is required");
            ExceptionUtilities.CheckArgumentNotNull(config, "config is required");

            this.testConfiguration = config;

            switch(payloadKind)
            {
                case ODataPayloadKind.Entry:
                    this.StartRead(reader.CreateODataEntryReader(), writer.CreateODataEntryWriter());
                    break;

                case ODataPayloadKind.Feed:
                    this.StartRead(reader.CreateODataFeedReader(), writer.CreateODataFeedWriter());
                    break;
                default:
                    throw new NotSupportedException("ObjectModelReadWriteStreamer currently supports only feed and entry");
            };
        }

        private void StartRead(ODataReader reader, ODataWriter writer)
        {
            ExceptionUtilities.Assert(reader.State == ODataReaderState.Start, "Reader is expected to be in state Start at StartRead.");
            while (reader.Read())
            {
                switch(reader.State)
                {
                    case ODataReaderState.EntryStart:
                        writer.WriteStart((ODataEntry)reader.Item);
                        break;
                    case ODataReaderState.FeedStart:
                        writer.WriteStart((ODataFeed)reader.Item);
                        break;
                    case ODataReaderState.NavigationLinkStart:
                        writer.WriteStart((ODataNavigationLink)reader.Item);
                        break;
                    case ODataReaderState.EntryEnd:
                    case ODataReaderState.FeedEnd:
                    case ODataReaderState.NavigationLinkEnd:
                        writer.WriteEnd();
                        break;
                    default:
                        throw new NotSupportedException();
                };
            }
        }
    }
}

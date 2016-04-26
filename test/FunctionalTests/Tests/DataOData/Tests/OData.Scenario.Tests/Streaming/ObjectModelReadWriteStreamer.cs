//---------------------------------------------------------------------
// <copyright file="ObjectModelReadWriteStreamer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Scenario.Tests.Streaming
{
    using System;
    using Microsoft.OData;
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
                case ODataPayloadKind.Resource:
                    this.StartRead(reader.CreateODataResourceReader(), writer.CreateODataResourceWriter());
                    break;

                case ODataPayloadKind.ResourceSet:
                    this.StartRead(reader.CreateODataResourceSetReader(), writer.CreateODataResourceSetWriter());
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
                    case ODataReaderState.ResourceStart:
                        writer.WriteStart((ODataResource)reader.Item);
                        break;
                    case ODataReaderState.ResourceSetStart:
                        writer.WriteStart((ODataResourceSet)reader.Item);
                        break;
                    case ODataReaderState.NestedResourceInfoStart:
                        writer.WriteStart((ODataNestedResourceInfo)reader.Item);
                        break;
                    case ODataReaderState.ResourceEnd:
                    case ODataReaderState.ResourceSetEnd:
                    case ODataReaderState.NestedResourceInfoEnd:
                        writer.WriteEnd();
                        break;
                    default:
                        throw new NotSupportedException();
                };
            }
        }
    }
}

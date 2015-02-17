//---------------------------------------------------------------------
// <copyright file="BatchPayloadSerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Default implementation of the batch payload deserializer
    /// </summary>
    [ImplementationName(typeof(IBatchPayloadSerializer), "Default")]
    public class BatchPayloadSerializer : IBatchPayloadSerializer
    {
        private StreamWriter writer;

        /// <summary>
        /// Serializes a batch request payload
        /// </summary>
        /// <param name="payload">The batch payload</param>
        /// <param name="boundary">The batch payload boundary</param>
        /// <param name="encodingName">Optional name of an encoding to use if it is relevant to the current format. May be null if no character-set information was known.</param>
        /// <returns>The serialized batch payload</returns>
        public byte[] SerializeBatchPayload(BatchRequestPayload payload, string boundary, string encodingName)
        {
            ExceptionUtilities.CheckArgumentNotNull(payload, "payload");

            var encoding = HttpUtilities.GetEncodingOrDefault(encodingName);
            return WriteBatchParts<IHttpRequest, BatchRequestChangeset>(payload, boundary, encoding, this.WriteRequest);
        }

        /// <summary>
        /// Serializes a batch response payload
        /// </summary>
        /// <param name="payload">The batch payload</param>
        /// <param name="boundary">The batch payload boundary</param>
        /// <param name="encodingName">Optional name of an encoding to use if it is relevant to the current format. May be null if no character-set information was known.</param>
        /// <returns>The serialized batch payload</returns>
        public byte[] SerializeBatchPayload(BatchResponsePayload payload, string boundary, string encodingName)
        {
            ExceptionUtilities.CheckArgumentNotNull(payload, "payload");

            var encoding = HttpUtilities.GetEncodingOrDefault(encodingName);
            return WriteBatchParts<HttpResponseData, BatchResponseChangeset>(payload, boundary, encoding, this.WriteResponse);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Stream is disposed when the writer is disposed")]
        private byte[] WriteBatchParts<TOperation, TChangeSet>(BatchPayload<TOperation, TChangeSet> batchPayload, string boundary, Encoding encoding, Action<TOperation> writeOperation)
            where TChangeSet : BatchChangeset<TOperation>
            where TOperation : class, IMimePart
        {
            using (this.writer = new StreamWriter(new MemoryStream(), encoding))
            {
                foreach (var batchPart in batchPayload.Parts)
                {
                    this.WriteStartBoundary(boundary);
                    this.WriteHeaders(batchPart);

                    var changeset = batchPart as TChangeSet;
                    if (changeset != null)
                    {
                        foreach (var changesetPart in changeset)
                        {
                            this.WriteStartBoundary(changeset.Boundary);
                            this.WriteHeaders(changesetPart);
                            var requestChangesetPart = changesetPart as MimePartData<TOperation>;
                            ExceptionUtilities.CheckArgumentNotNull(requestChangesetPart, "requestChangeSetPart cannot be null");
                            writeOperation(requestChangesetPart.Body);
                        }

                        // BatchPayloadSerializer does not write end boundaries if no operations/parts exist
                        //             Test triage decided that changesets without parts are enough of an edge case to not fix this.
                        if (changeset.Any())
                        {
                            this.WriteEndBoundary(changeset.Boundary);
                        }
                    }
                    else
                    {
                        // The batch part not a changeSet so write the request or response.
                        var batchOperation = batchPart as MimePartData<TOperation>;
                        ExceptionUtilities.CheckArgumentNotNull(batchOperation, "batchOperation cannot be null");
                        writeOperation(batchOperation.Body);
                    }
                }

                // BatchPayloadSerializer does not write end boundaries if no operations/parts exist
                // Test triage decided that batch payloads without parts are enough of an edge case to not fix this.
                if (batchPayload.Parts.Any())
                {
                    this.WriteEndBoundary(boundary);
                }

                return ((MemoryStream)this.writer.BaseStream).ToArray();
            }
        }

        private void WriteStartBoundary(string boundary)
        {
            // BatchPayloadSerializer does not write leading line feeds for boundaries
            // Test triage decided that we will rely on targeted ODataLib tests to ensure that
            // boundaries with leading CRLF work as expected.
            this.writer.Write("--");
            this.writer.WriteLine(boundary);
            this.writer.Flush();
        }

        private void WriteEndBoundary(string boundary)
        {
            // BatchPayloadSerializer does not write leading line feeds for boundaries
            // Test triage decided that we will rely on targeted ODataLib tests to ensure that
            // boundaries with leading CRLF work as expected.
            this.writer.Write("--");
            this.writer.Write(boundary);
            this.writer.WriteLine("--");
            this.writer.Flush();
        }

        private void WriteHeaders(IMimePart batchPart)
        {
            foreach (var header in batchPart.Headers)
            {
                this.writer.Write(header.Key);
                this.writer.Write(": ");
                this.writer.WriteLine(header.Value);
            }

            this.writer.WriteLine();
            this.writer.Flush();
        }

        private void WriteRequest(IHttpRequest request)
        {
            this.writer.WriteLine(request.GetFirstLine());
            
            this.WriteHeaders(request);

            var requestBody = request.GetRequestBody();
            this.WriteRequestOrResponseBody(requestBody);

            this.writer.Flush();
        }

        private void WriteResponse(HttpResponseData response)
        {
            this.writer.WriteLine(response.GetFirstLine());

            this.WriteHeaders(response);

            var responseBody = response.Body;
            this.WriteRequestOrResponseBody(responseBody);

            this.writer.Flush();
        }

        private void WriteRequestOrResponseBody(byte[] requestBody)
        {
            if (requestBody != null)
            {
                this.writer.Flush();
                this.writer.BaseStream.Write(requestBody, 0, requestBody.Length);
            }
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="BatchPayloadDeserializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Text;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Default implementation of the batch payload deserializer
    /// </summary>
    [ImplementationName(typeof(IBatchPayloadDeserializer), "Default")]
    public class BatchPayloadDeserializer : IBatchPayloadDeserializer
    {
        /// <summary>
        /// Gets or sets the request manager to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataRequestManager RequestManager { get; set; }

        /// <summary>
        /// Gets or sets the format selector to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IProtocolFormatStrategySelector FormatSelector { get; set; }

        /// <summary>
        /// Deserializes the given request's binary payload into a batch payload
        /// </summary>
        /// <param name="request">The request to deserialize</param>
        /// <returns>The deserialized batch request payload</returns>
        public BatchRequestPayload DeserializeBatchRequest(HttpRequestData request)
        {
            var encoding = request.GetEncodingFromHeadersOrDefault();

            MultipartMimeData<MimePartData<byte[]>> split;
            ExceptionUtilities.Assert(TrySplitMimePart(request, encoding, out split), "Failed to split batch response body");

            var batchRequest = new BatchRequestPayload();

            foreach (var subPart in split.ToList())
            {
                MultipartMimeData<MimePartData<byte[]>> splitChangeset;
                if (TrySplitMimePart(subPart, encoding, out splitChangeset))
                {
                    var changeset = new BatchRequestChangeset();
                    changeset.Headers.AddRange(splitChangeset.Headers);

                    foreach (var changesetPart in splitChangeset)
                    {
                        if (changesetPart.Body != null)
                        {
                            if (changesetPart.Body.Length > 0)
                            {
                                changeset.Add(this.BuildRequestFromPart(changesetPart, encoding));
                            }
                        }
                    }

                    batchRequest.Add(changeset);
                }
                else
                {
                    batchRequest.Add(this.BuildRequestFromPart(subPart, encoding));    
                }
            }

            return batchRequest;
        }

        /// <summary>
        /// Deserializes the given response's binary payload into a batch payload
        /// </summary>
        /// <param name="requestPayload">The batch request payload that corresponds to the request</param>
        /// <param name="response">The response to deserialize</param>
        /// <returns>The deserialized batch response payload</returns>
        public BatchResponsePayload DeserializeBatchResponse(BatchRequestPayload requestPayload, HttpResponseData response)
        {
            var encoding = HttpUtilities.DefaultEncoding;
            string charset;
            if (response.TryGetMimeCharset(out charset))
            {
                encoding = Encoding.GetEncoding(charset);
            }

            MultipartMimeData<MimePartData<byte[]>> split;
            ExceptionUtilities.Assert(TrySplitMimePart(response, encoding, out split), "Failed to split batch response body");

            var batchResponse = new BatchResponsePayload();
            
            var requestPartQueue = new Queue<IMimePart>(requestPayload.Parts);

            foreach (var subPart in split.ToList())
            {
                MultipartMimeData<MimePartData<byte[]>> splitChangeset;
                if (TrySplitMimePart(subPart, encoding, out splitChangeset))
                {
                    ExceptionUtilities.Assert(requestPartQueue.Count > 0, "Response changeset did not line up anything in request");
                    var requestChangeset = requestPartQueue.Dequeue() as BatchRequestChangeset;
                    ExceptionUtilities.CheckObjectNotNull(requestChangeset, "Response changeset did not line up with a request changeset");

                    var requestsQueue = new Queue<IHttpRequest>(requestChangeset.Operations);

                    var changeset = new BatchResponseChangeset();
                    changeset.Headers.AddRange(splitChangeset.Headers);

                    foreach (var changesetPart in splitChangeset)
                    {
                        ExceptionUtilities.Assert(requestsQueue.Count > 0, "Response did not line up anything in request changeset");
                        changeset.Add(this.BuildResponseFromPart(changesetPart, requestsQueue.Dequeue(), encoding));
                    }

                    batchResponse.Add(changeset);
                }
                else
                {
                    ExceptionUtilities.Assert(requestPartQueue.Count > 0, "Response did not line up anything in request");
                    var requestOperation = requestPartQueue.Dequeue() as MimePartData<IHttpRequest>;
                    ExceptionUtilities.CheckObjectNotNull(requestOperation, "Response operation did not line up with request operation");

                    batchResponse.Add(this.BuildResponseFromPart(subPart, requestOperation.Body, encoding));
                }
            }

            return batchResponse;
        }

        internal static bool TrySplitMimePart(MimePartData<byte[]> part, Encoding encoding, out MultipartMimeData<MimePartData<byte[]>> multipart)
        {
            string boundary;
            if (!part.TryGetMimeBoundary(out boundary))
            {
                multipart = null;
                return false;
            }

            multipart = new MultipartMimeData<MimePartData<byte[]>>();
            multipart.Headers.AddRange(part.Headers);

            var body = part.Body;
            ExceptionUtilities.CheckObjectNotNull(body, "Body cannot be null");

            byte[] boundaryBytes = encoding.GetBytes(boundary);
            byte[] newlineBytes = encoding.GetBytes("\r\n");
            byte[] controlBytes = encoding.GetBytes("--");

            while (EndsWithPattern(body, newlineBytes))
            {
                body = body.Take(body.Length - newlineBytes.Length).ToArray();
            }

            byte[] firstBoundaryBytes = controlBytes.Concat(boundaryBytes).Concat(newlineBytes).ToArray();
            byte[] innerBoundaryBytes = newlineBytes.Concat(controlBytes).Concat(boundaryBytes).Concat(newlineBytes).ToArray();
            byte[] finalBoundaryBytesWithoutFirstNewLine = controlBytes.Concat(boundaryBytes).Concat(controlBytes).ToArray();
            byte[] finalBoundaryBytes = newlineBytes.Concat(finalBoundaryBytesWithoutFirstNewLine).ToArray();

            int firstBoundaryStart = IndexOfPattern(body, firstBoundaryBytes, 0);
            if (firstBoundaryStart == -1)
            {
                // Possibly an empty changeset - empty multipart can be represented as just the end boundary
                int finalBoundaryStart = IndexOfPattern(body, finalBoundaryBytesWithoutFirstNewLine, 0);
                ExceptionUtilities.Assert(finalBoundaryStart == 0, "Malformed");
                ExceptionUtilities.Assert(EndsWithPattern(body, finalBoundaryBytesWithoutFirstNewLine), "Malformed");

                return true;
            }

            ExceptionUtilities.Assert(firstBoundaryStart == 0, "Malformed");
            ExceptionUtilities.Assert(EndsWithPattern(body, finalBoundaryBytes), "Malformed");

            int currentBoundaryEnd = firstBoundaryStart + firstBoundaryBytes.Length;
            while (currentBoundaryEnd < body.Length)
            {
                int nextBoundaryStart = IndexOfPattern(body, innerBoundaryBytes, currentBoundaryEnd);
                int nextBoundaryEnd = nextBoundaryStart + innerBoundaryBytes.Length;
                if (nextBoundaryStart < 0)
                {
                    nextBoundaryStart = IndexOfPattern(body, finalBoundaryBytes, currentBoundaryEnd);
                    nextBoundaryEnd = nextBoundaryStart + finalBoundaryBytes.Length;
                    ExceptionUtilities.Assert(nextBoundaryStart != 0, "Malformed");
                    ExceptionUtilities.Assert(nextBoundaryStart + finalBoundaryBytes.Length == body.Length, "Malformed");
                }

                var fragment = body.Skip(currentBoundaryEnd).Take(nextBoundaryStart - currentBoundaryEnd).ToArray();
                var subPart = new MimePartData<byte[]>();
                if (fragment.Length > 0)
                {
                    PopulateHeadersAndBody(subPart, fragment, encoding);
                    multipart.Add(subPart);
                }

                currentBoundaryEnd = nextBoundaryEnd;
            }

            return true;
        }

        internal static void PopulateHeadersAndBody(MimePartData<byte[]> part, byte[] bufferToReadFrom, Encoding encoding)
        {
            ReadHeaders(ref bufferToReadFrom, part, encoding);
            part.Body = bufferToReadFrom;
        }

        internal static HttpResponseData CreateResponse(byte[] body, Encoding encoding)
        {
            var response = new HttpResponseData();
            
            var statusLine = ReadLine(ref body, encoding);
            var statusSections = statusLine.Split(' ');
            ExceptionUtilities.Assert(statusSections.Length >= 3, "Response status line was malformed: '{0}'", statusLine);

            response.StatusCode = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), statusSections[1], false);

            PopulateHeadersAndBody(response, body, encoding);

            return response;
        }

        internal static HttpRequestData CreateRequest(byte[] body, Encoding encoding)
        {
            var request = new HttpRequestData();

            var statusLine = ReadLine(ref body, encoding);
            var statusSections = statusLine.Split(' ');
            ExceptionUtilities.Assert(statusSections.Length == 3, "Request status line was malformed: '{0}'", statusLine);

            request.Verb = (HttpVerb)Enum.Parse(typeof(HttpVerb), statusSections[0], true);
            request.Uri = new Uri(statusSections[1], UriKind.RelativeOrAbsolute);

            PopulateHeadersAndBody(request, body, encoding);

            return request;
        }

        internal static void ReadHeaders(ref byte[] buffer, IMimePart mimePart, Encoding encoding)
        {
            while (buffer.Length > 0)
            {
                var line = ReadLine(ref buffer, encoding);

                if (string.IsNullOrEmpty(line))
                {
                    // indicates two newlines in a row, means we should stop
                    break;
                }

                int index = line.IndexOf(": ", StringComparison.Ordinal);
                if (index < 0)
                {
                    // throw?
                    mimePart.Headers[line] = null;
                }
                else
                {
                    mimePart.Headers[line.Substring(0, index)] = line.Substring(index + 2);
                }
            }
        }

        internal static string ReadLine(ref byte[] buffer, Encoding encoding)
        {
            var linePattern = encoding.GetBytes("\r\n");
            var index = IndexOfPattern(buffer, linePattern, 0);
            if (index < 0)
            {
                var line = encoding.GetString(buffer);
                buffer = new byte[0];
                return line;
            }
            else
            {
                var lineBuffer = new byte[index];
                Array.Copy(buffer, lineBuffer, index);
                var line = encoding.GetString(lineBuffer);

                var startIndex = index + linePattern.Length;
                var newBuffer = new byte[buffer.Length - index - linePattern.Length];
                Array.Copy(buffer, startIndex, newBuffer, 0, newBuffer.Length);

                ExceptionUtilities.Assert(newBuffer.Length + lineBuffer.Length + linePattern.Length == buffer.Length, "Lost bytes!");
                buffer = newBuffer;
                return line;
            }
        }

        internal static int IndexOfPattern(byte[] buffer, byte[] pattern, int start)
        {
            if (pattern.Length > buffer.Length)
            {
                return -1;
            }

            for (int bufferIndex = start; bufferIndex < buffer.Length; bufferIndex++)
            {
                if (bufferIndex + pattern.Length > buffer.Length)
                {
                    return -1;
                }

                bool isMatch = true;
                for (int patternIndex = 0; patternIndex < pattern.Length; patternIndex++)
                {
                    // earlier check ensures this indexing is safe
                    var indexToCheck = bufferIndex + patternIndex;
                    if (buffer[indexToCheck] != pattern[patternIndex])
                    {
                        isMatch = false;
                        break;
                    }
                }

                if (isMatch)
                {
                    return bufferIndex;
                }
            }

            return -1;
        }

        internal static bool EndsWithPattern(byte[] buffer, byte[] pattern)
        {
            return buffer.Skip(buffer.Length - pattern.Length).SequenceEqual(pattern);
        }

        internal MimePartData<IHttpRequest> BuildRequestFromPart(MimePartData<byte[]> mimePart, Encoding encoding)
        {
            var requestData = CreateRequest(mimePart.Body, encoding);

            var odataUri = new ODataUri(new UnrecognizedSegment(requestData.Uri.OriginalString));
            var odataRequest = this.RequestManager.BuildRequest(odataUri, requestData.Verb, requestData.Headers);

            string contentType;
            if (requestData.TryGetHeaderValueIgnoreHeaderCase(HttpHeaders.ContentType, out contentType))
            {
                var formatStrategy = this.FormatSelector.GetStrategy(contentType, odataUri);
                var deserializer = formatStrategy.GetDeserializer();
                var rootElement = deserializer.DeserializeFromBinary(requestData.Body, ODataPayloadContext.BuildPayloadContextFromRequest(odataRequest));
                odataRequest.Body = new ODataPayloadBody(requestData.Body, rootElement);
            }
            else if (requestData.Body != null)
            {
                odataRequest.Body = new ODataPayloadBody(requestData.Body, new PrimitiveValue(null, requestData.Body));
            }

            var rebuiltPart = new MimePartData<IHttpRequest>();
            rebuiltPart.Headers.AddRange(mimePart.Headers);
            rebuiltPart.Body = odataRequest;

            return rebuiltPart;
        }

        internal MimePartData<HttpResponseData> BuildResponseFromPart(MimePartData<byte[]> mimePart, IMimePart currentRequestPart, Encoding encoding)
        {
            var odataRequest = currentRequestPart as ODataRequest;
            HttpResponseData responsePart = CreateResponse(mimePart.Body, encoding);
            if (odataRequest != null)
            {
                responsePart = this.RequestManager.BuildResponse(odataRequest, responsePart);
            }

            var rebuiltPart = new MimePartData<HttpResponseData>();
            rebuiltPart.Headers.AddRange(mimePart.Headers);
            rebuiltPart.Body = responsePart;

            return rebuiltPart;
        }
    }
}

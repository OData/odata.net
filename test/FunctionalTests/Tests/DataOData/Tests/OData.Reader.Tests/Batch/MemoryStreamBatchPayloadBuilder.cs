//---------------------------------------------------------------------
// <copyright file="MemoryStreamBatchPayloadBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Batch
{
    #region Namespaces
    using System.IO;
    using System.Linq;
    using System.Text;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    /// <summary>
    /// Batch payload builder that works by adding bytes to a memory stream.
    /// </summary>
    public sealed class MemoryStreamBatchPayloadBuilder
    {
        /// <summary>The characters forming the boundary delimiter.</summary>
        internal static readonly char[] BoundaryDelimiter = new char[] { '-', '-' };

        /// <summary>The characters to use as whitespace.</summary>
        internal static char[] whiteSpaceChars = new[] 
            { 
                (char)0x09, /*tab*/
                (char)0x0B, /*line tabulation*/ 
                (char)0x0C, /*form feed*/ 
                (char)0x20, /*space*/
            };

        /// <summary>The line feed characters to be used by this builder instance.</summary>
        private readonly char[] lineFeedChars;

        /// <summary>The memory stream backing the builder.</summary>
        private readonly MemoryStream memoryStream;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="encoding">The encoding to use for converting bytes to and from strings.</param>
        /// <param name="lineFeedChars">The line feed characters to be used by this builder instance.</param>
        internal MemoryStreamBatchPayloadBuilder(Encoding encoding, char[] lineFeedChars)
        {
            this.Encoding = encoding;
            this.lineFeedChars = lineFeedChars;
            this.memoryStream = new MemoryStream(BatchReaderStreamBufferWrapper.BufferLength);
        }

        /// <summary>The encoding to use for converting bytes to and from strings.</summary>
        internal Encoding Encoding
        {
            get;
            set;
        }

        /// <summary>The memory stream backing the builder.</summary>
        internal MemoryStream MemoryStream
        {
            get
            {
                return this.memoryStream;
            }
        }

        /// <summary>
        /// Resets the current position in the memory stream to the beginning of the stream.
        /// </summary>
        internal MemoryStream ResetMemoryStream()
        {
            // reset the stream position to '0'
            this.memoryStream.Position = 0;
            return this.memoryStream;
        }

        /// <summary>
        /// Writes a start boundary line with the specified boundary string.
        /// </summary>
        /// <param name="boundary">The boundary string to use.</param>
        /// <param name="leadingBoundaryLineFeed">true if a leading line feed should be included; otherwise false.</param>
        /// <param name="whitespaceCount">Optional number of whitespace characters to include in the boundary line.</param>
        /// <returns>The payload builder instance for composability reasons.</returns>
        internal MemoryStreamBatchPayloadBuilder StartBoundary(string boundary, bool leadingBoundaryLineFeed = true, int whitespaceCount = 0)
        {
            if (leadingBoundaryLineFeed)
            {
                // a boundary always starts with a CRLF
                this.LineFeed();
            }

            // add the start boundary delimiter
            this.Chars(BoundaryDelimiter);

            // add the boundary string and the terminating CRLF
            this.String(boundary);

            if (whitespaceCount > 0)
            {
                this.FillWhitespace(whitespaceCount);
            }

            // boundary is also terminated by CRLF
            this.LineFeed();

            return this;
        }

        /// <summary>
        /// Writes an end boundary line with the specified boundary string.
        /// </summary>
        /// <param name="boundary">The boundary string to use.</param>
        /// <param name="leadingBoundaryLineFeed">true if a leading line feed should be included; otherwise false.</param>
        /// <param name="whitespaceCount">Optional number of whitespace characters to include in the boundary line.</param>
        /// <returns>The payload builder instance for composability reasons.</returns>
        internal MemoryStreamBatchPayloadBuilder EndBoundary(string boundary, bool leadingBoundaryLineFeed = true, int whitespaceCount = 0)
        {
            if (leadingBoundaryLineFeed)
            {
                // a boundary always starts with a CRLF
                this.LineFeed();
            }

            // add the start boundary delimiter
            this.Chars(BoundaryDelimiter);

            // add the boundary string
            this.String(boundary);

            // add the end boundary delimiter
            this.Chars(BoundaryDelimiter);

            if (whitespaceCount > 0)
            {
                this.FillWhitespace(whitespaceCount);
            }

            // boundary is also terminated by CRLF
            this.LineFeed();

            return this;
        }

        /// <summary>
        /// Writes a header line.
        /// </summary>
        /// <param name="name">The name of the header.</param>
        /// <param name="value">The value of the header.</param>
        /// <returns>The payload builder instance for composability reasons.</returns>
        internal MemoryStreamBatchPayloadBuilder Header(string name, string value)
        {
            this.Append(this.Encoding.GetBytes(name));
            this.String(": ");
            this.Append(this.Encoding.GetBytes(value));
            this.LineFeed();
            return this;
        }

        /// <summary>
        /// Writes a request line.
        /// </summary>
        /// <param name="method">The HTTP method to use for the request.</param>
        /// <param name="uri">The URI of the request.</param>
        /// <returns>The payload builder instance for composability reasons.</returns>
        internal MemoryStreamBatchPayloadBuilder RequestLine(string method, string uri)
        {
            this.String(method);
            this.Chars(' ');
            this.String(uri);
            this.String(" HTTP/1.1");
            this.LineFeed();
            return this;
        }

        /// <summary>
        /// Writes a response line.
        /// </summary>
        /// <param name="statusCode">The status code of the response.</param>
        /// <returns>The payload builder instance for composability reasons.</returns>
        internal MemoryStreamBatchPayloadBuilder ResponseLine(int statusCode)
        {
            this.String("HTTP/1.1 ");
            this.String(statusCode.ToString());
            this.Chars(' ');
            this.String(GetStatusMessage(statusCode));
            this.LineFeed();
            return this;
        }

        /// <summary>
        /// Writes the beginning of a batch request (request line, HTTP headers, Content-Type header, line feed, preamble).
        /// </summary>
        /// <param name="uri">The URI of the batch request.</param>
        /// <param name="boundary">The boundary string to be used in the request.</param>
        /// <param name="preamble">An optional preamble.</param>
        /// <returns>The payload builder instance for composability reasons.</returns>
        internal MemoryStreamBatchPayloadBuilder BatchRequest(string uri, string boundary, string preamble = null)
        {
            // request line
            this.RequestLine("POST", uri);

            // add an HTTP header for good measure
            this.Header("Host", "localhost");

            // Content-Type header
            this.Header("Content-Type", "multipart/mixed;boundary=" + boundary);

            // empty line after HTTP headers
            this.LineFeed();

            if (preamble != null)
            {
                this.String(preamble);
            }

            return this;
        }

        /// <summary>
        /// Writes the beginning of a batch response.
        /// </summary>
        /// <param name="boundary">The boundary string to use in the response.</param>
        /// <returns>The payload builder instance for composability reasons.</returns>
        internal MemoryStreamBatchPayloadBuilder BatchResponse(string boundary)
        {
            // response line
            this.ResponseLine(202);

            // add DSV header
            this.Header("DataServiceVersion", "1.0");

            // Content-Type header
            this.Header("Content-Type", "multipart/mixed;boundary=" + boundary);

            // empty line after HTTP headers
            this.LineFeed();

            return this;
        }

        /// <summary>
        /// Writes a request part for a query operation (part headers, request line, request headers, line feed).
        /// </summary>
        /// <param name="method">The HTTP method to use in the request line.</param>
        /// <param name="uri">The URI to use in the request line.</param>
        /// <returns>The payload builder instance for composability reasons.</returns>
        internal MemoryStreamBatchPayloadBuilder QueryOperationRequest(string method, string uri)
        {
            // part headers
            this.Header("Content-Type", "application/http");
            this.Header("Content-Transfer-Encoding", "binary");

            // request line
            this.RequestLine(method, uri);

            // add an HTTP header for good measure
            this.Header("Host", "localhost");

            // empty line after HTTP headers
            this.LineFeed();

            return this;
        }

        /// <summary>
        /// Writes a response part for a query operation (part headers, response line).
        /// </summary>
        /// <param name="statusCode">The status code to use in the response line.</param>
        /// <returns>The payload builder instance for composability reasons.</returns>
        internal MemoryStreamBatchPayloadBuilder QueryOperationResponse(int statusCode)
        {
            // part headers
            this.Header("Content-Type", "application/http");
            this.Header("Content-Transfer-Encoding", "binary");

            // response line
            this.ResponseLine(statusCode);

            return this;
        }

        /// <summary>
        /// Write the start of a changeset part.
        /// </summary>
        /// <param name="boundary">The changeset boundary.</param>
        /// <param name="preamble">The optional preamble to write.</param>
        /// <returns>The payload builder instance for composability reasons.</returns>
        internal MemoryStreamBatchPayloadBuilder Changeset(string boundary, string preamble = null)
        {
            // part headers
            this.Header("Content-Type", "multipart/mixed;boundary=" + boundary);

            if (preamble != null)
            {
                this.String(preamble);
            }

            return this;
        }

        /// <summary>
        /// Writes a string.
        /// </summary>
        /// <param name="s">The string to write.</param>
        /// <returns>The payload builder instance for composability reasons.</returns>
        internal MemoryStreamBatchPayloadBuilder String(string s)
        {
            this.Append(this.Encoding.GetBytes(s));
            return this;
        }

        /// <summary>
        /// Writes a line (a string terminated by a line feed).
        /// </summary>
        /// <param name="line">The line to write (without the line feed characters).</param>
        /// <returns>The payload builder instance for composability reasons.</returns>
        internal MemoryStreamBatchPayloadBuilder Line(string line)
        {
            this.String(line);
            this.LineFeed();
            return this;
        }

        /// <summary>
        /// Writes a sequence of characters.
        /// </summary>
        /// <param name="chars">The characters to write.</param>
        /// <returns>The payload builder instance for composability reasons.</returns>
        internal MemoryStreamBatchPayloadBuilder Chars(params char[] chars)
        {
            this.Append(this.Encoding.GetBytes(chars));
            return this;
        }

        /// <summary>
        /// Writes a line termination sequence (line feed characters).
        /// </summary>
        /// <returns>The payload builder instance for composability reasons.</returns>
        internal MemoryStreamBatchPayloadBuilder LineFeed()
        {
            this.Chars(lineFeedChars);
            return this;
        }

        /// <summary>
        /// Writes a sequence of bytes.
        /// </summary>
        /// <param name="bytes">The bytes to write.</param>
        /// <returns>The payload builder instance for composability reasons.</returns>
        internal MemoryStreamBatchPayloadBuilder Bytes(byte[] bytes)
        {
            this.Append(bytes);
            return this;
        }

        /// <summary>
        /// Adds a sequence of <paramref name="count"/> bytes.
        /// </summary>
        /// <param name="count">The number of bytes to add.</param>
        /// <param name="charValue">The character to use as byte value; if none is specified a random one is generated.</param>
        /// <returns>The payload builder instance for composability reasons.</returns>
        internal MemoryStreamBatchPayloadBuilder FillBytes(int count, char? charValue = null)
        {
            if (count > 0)
            {
                byte[] bytes = new byte[count];
                for (int i = 0; i < count; ++i)
                {
                    bytes[i] = charValue.HasValue ? (byte)charValue.Value : (byte)(i % byte.MaxValue);
                }

                this.Append(bytes);
            }

            return this;
        }

        /// <summary>
        /// Gets the string status message for a given Http response status code.
        /// </summary>
        /// <param name="statusCode">The status code to get the status message for.</param>
        /// <returns>The string status message for the <paramref name="statusCode"/>.</returns>
        private static string GetStatusMessage(int statusCode)
        {
            // Non-localized messages for status codes.
            // These are the recommended reason phrases as per HTTP RFC 2616, Section 6.1.1
            switch (statusCode)
            {
                case 100:
                    return "Continue";
                case 101:
                    return "Switching Protocols";
                case 200:
                    return "OK";
                case 201:
                    return "Created";
                case 202:
                    return "Accepted";
                case 203:
                    return "Non-Authoritative Information";
                case 204:
                    return "No Content";
                case 205:
                    return "Reset Content";
                case 206:
                    return "Partial Content";
                case 300:
                    return "Multiple Choices";
                case 301:
                    return "Moved Permanently";
                case 302:
                    return "Found";
                case 303:
                    return "See Other";
                case 304:
                    return "Not Modified";
                case 305:
                    return "Use Proxy";
                case 307:
                    return "Temporary Redirect";
                case 400:
                    return "Bad Request";
                case 401:
                    return "Unauthorized";
                case 402:
                    return "Payment Required";
                case 403:
                    return "Forbidden";
                case 404:
                    return "Not Found";
                case 405:
                    return "Method Not Allowed";
                case 406:
                    return "Not Acceptable";
                case 407:
                    return "Proxy Authentication Required";
                case 408:
                    return "Request Time-out";
                case 409:
                    return "Conflict";
                case 410:
                    return "Gone";
                case 411:
                    return "Length Required";
                case 412:
                    return "Precondition Failed";
                case 413:
                    return "Request Entity Too Large";
                case 414:
                    return "Request-URI Too Large";
                case 415:
                    return "Unsupported Media Type";
                case 416:
                    return "Requested range not satisfiable";
                case 417:
                    return "Expectation Failed";
                case 500:
                    return "Internal Server Error";
                case 501:
                    return "Not Implemented";
                case 502:
                    return "Bad Gateway";
                case 503:
                    return "Service Unavailable";
                case 504:
                    return "Gateway Time-out";
                case 505:
                    return "HTTP Version not supported";
                default:
                    return "Unknown Status Code";
            }
        }

        /// <summary>
        /// Appends a sequence of bytes to the memory stream backing the builder.
        /// </summary>
        /// <param name="bytes">The bytes to add.</param>
        private void Append(params byte[] bytes)
        {
            this.memoryStream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Appends various whitespace characters.
        /// </summary>
        /// <param name="whitespaceLength">The number of whitespace characters to append.</param>
        private void FillWhitespace(int whitespaceLength)
        {
            StringBuilder builder = new StringBuilder();

            int allWhiteSpaceCount = whitespaceLength / whiteSpaceChars.Length;
            if (allWhiteSpaceCount > 0)
            {
                builder.Insert(0, new string(whiteSpaceChars), allWhiteSpaceCount);
            }

            int partialWhiteSpaceCount = whitespaceLength % whiteSpaceChars.Length;
            if (partialWhiteSpaceCount > 0)
            {
                builder.Append(new string(whiteSpaceChars.Take(partialWhiteSpaceCount).ToArray()));
            }

            this.String(builder.ToString());
        }
    }
}

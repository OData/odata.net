//---------------------------------------------------------------------
// <copyright file="ODataAsynchronousReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
#if PORTABLELIB
    using System.Threading.Tasks;
#endif
    #endregion Namespaces

    /// <summary>
    /// Class for reading OData async messages.
    /// </summary>
    public sealed class ODataAsynchronousReader
    {
        /// <summary>
        /// The input context to read the content from.
        /// </summary>
        private readonly ODataRawInputContext rawInputContext;

        /// <summary>
        /// The optional dependency injection container to get related services for message writing.
        /// </summary>
        private readonly IServiceProvider container;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="rawInputContext">The input context to read the content from.</param>
        /// <param name="encoding">The encoding for the underlying stream.</param>
        internal ODataAsynchronousReader(ODataRawInputContext rawInputContext, Encoding encoding)
        {
            Debug.Assert(rawInputContext != null, "rawInputContext != null");

            // Currently we only support single-byte UTF8 in async reader.
            if (encoding != null)
            {
                ReaderValidationUtils.ValidateEncodingSupportedInAsync(encoding);
            }

            this.rawInputContext = rawInputContext;
            this.container = rawInputContext.Container;
        }

        /// <summary>
        /// Returns a message for reading the content of an async response.
        /// </summary>
        /// <returns>A response message for reading the content of the async response.</returns>
        public ODataAsynchronousResponseMessage CreateResponseMessage()
        {
            this.VerifyCanCreateResponseMessage(true);

            return this.CreateResponseMessageImplementation();
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously returns a message for reading the content of an async response.
        /// </summary>
        /// <returns>A response message for reading the content of the async response.</returns>
        public Task<ODataAsynchronousResponseMessage> CreateResponseMessageAsync()
        {
            this.VerifyCanCreateResponseMessage(false);

            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateResponseMessageImplementation());
        }
#endif

        /// <summary>
        /// Validates that the async reader is not disposed.
        /// </summary>
        private void ValidateReaderNotDisposed()
        {
            this.rawInputContext.VerifyNotDisposed();
        }

        /// <summary>
        /// Verifies that a call is allowed to the reader.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        private void VerifyCallAllowed(bool synchronousCall)
        {
            if (synchronousCall)
            {
                if (!this.rawInputContext.Synchronous)
                {
                    throw new ODataException(Strings.ODataAsyncReader_SyncCallOnAsyncReader);
                }
            }
            else
            {
#if PORTABLELIB
                if (this.rawInputContext.Synchronous)
                {
                    throw new ODataException(Strings.ODataAsyncReader_AsyncCallOnSyncReader);
                }
#else
                Debug.Assert(false, "Async calls are not allowed in this build.");
#endif
            }
        }

        /// <summary>
        /// Verifies that calling CreateResponseMessage is valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        private void VerifyCanCreateResponseMessage(bool synchronousCall)
        {
            this.ValidateReaderNotDisposed();
            this.VerifyCallAllowed(synchronousCall);

            if (!this.rawInputContext.ReadingResponse)
            {
                throw new ODataException(Strings.ODataAsyncReader_CannotCreateResponseWhenNotReadingResponse);
            }
        }

        /// <summary>
        /// Returns an <see cref="ODataAsynchronousResponseMessage"/> for reading the content of an async response - implementation of the actual functionality.
        /// </summary>
        /// <returns>The message that can be used to read the response.</returns>
        private ODataAsynchronousResponseMessage CreateResponseMessageImplementation()
        {
            int statusCode;
            IDictionary<string, string> headers;

            this.ReadInnerEnvelope(out statusCode, out headers);

            return ODataAsynchronousResponseMessage.CreateMessageForReading(this.rawInputContext.Stream, statusCode, headers, this.container);
        }

        /// <summary>
        /// Reads the envelope from the inner HTTP message.
        /// </summary>
        /// <param name="statusCode">The status code to use for the async response message.</param>
        /// <param name="headers">The headers to use for the async response message.</param>
        private void ReadInnerEnvelope(out int statusCode, out IDictionary<string, string> headers)
        {
            string responseLine = this.ReadFirstNonEmptyLine();

            Debug.Assert(this.rawInputContext.ReadingResponse, "Must only be called for responses.");
            statusCode = ParseResponseLine(responseLine);
            headers = this.ReadHeaders();
        }

        /// <summary>
        /// Read and return the next line from the stream, skipping all empty lines.
        /// </summary>
        /// <returns>The text of the first non-empty line (not including any terminating newline characters).</returns>
        private string ReadFirstNonEmptyLine()
        {
            string line = this.ReadLine();

            while (line.Length == 0)
            {
                line = this.ReadLine();
            }

            return line;
        }

        /// <summary>
        /// Parses the response line of an async response.
        /// </summary>
        /// <param name="responseLine">The response line as a string.</param>
        /// <returns>The parsed status code from the response line.</returns>
        private static int ParseResponseLine(string responseLine)
        {
            // Async Response: HTTP/1.1 200 Ok
            // Since the http status code strings have spaces in them, we cannot use the same
            // logic. We need to check for the second space and anything after that is the error
            // message.
            if (responseLine.Length == 0)
            {
                // empty line
                throw new ODataException(Strings.ODataAsyncReader_InvalidResponseLine(responseLine));
            }

            int firstSpaceIndex = responseLine.IndexOf(' ');
            if (firstSpaceIndex <= 0 || responseLine.Length - 3 <= firstSpaceIndex)
            {
                // only 1 segment or empty first segment or not enough left for 2nd and 3rd segments
                throw new ODataException(Strings.ODataAsyncReader_InvalidResponseLine(responseLine));
            }

            int secondSpaceIndex = responseLine.IndexOf(' ', firstSpaceIndex + 1);
            if (secondSpaceIndex < 0 || secondSpaceIndex - firstSpaceIndex - 1 <= 0 || responseLine.Length - 1 <= secondSpaceIndex)
            {
                // only 2 segments or empty 2nd or 3rd segments
                // only 1 segment or empty first segment or not enough left for 2nd and 3rd segments
                throw new ODataException(Strings.ODataAsyncReader_InvalidResponseLine(responseLine));
            }

            string httpVersionSegment = responseLine.Substring(0, firstSpaceIndex);
            string statusCodeSegment = responseLine.Substring(firstSpaceIndex + 1, secondSpaceIndex - firstSpaceIndex - 1);

            // Validate HttpVersion
            if (string.CompareOrdinal(ODataConstants.HttpVersionInAsync, httpVersionSegment) != 0)
            {
                throw new ODataException(Strings.ODataAsyncReader_InvalidHttpVersionSpecified(httpVersionSegment, ODataConstants.HttpVersionInAsync));
            }

            int intResult;
            if (!Int32.TryParse(statusCodeSegment, out intResult))
            {
                throw new ODataException(Strings.ODataAsyncReader_NonIntegerHttpStatusCode(statusCodeSegment));
            }

            return intResult;
        }

        /// <summary>
        /// Reads the headers of an async response.
        /// </summary>
        /// <returns>A dictionary of header names to header values; never null.</returns>
        private IDictionary<string, string> ReadHeaders()
        {
            var headers = new Dictionary<string, string>(StringComparer.Ordinal);

            // Read all the headers
            string headerLine = this.ReadLine();
            while (!string.IsNullOrEmpty(headerLine))
            {
                string headerName, headerValue;
                ValidateHeaderLine(headerLine, out headerName, out headerValue);

                if (headers.ContainsKey(headerName))
                {
                    throw new ODataException(Strings.ODataAsyncReader_DuplicateHeaderFound(headerName));
                }

                headers.Add(headerName, headerValue);
                headerLine = this.ReadLine();
            }

            return headers;
        }

        /// <summary>
        /// Parses a header line and validates that it has the correct format.
        /// </summary>
        /// <param name="headerLine">The header line to validate.</param>
        /// <param name="headerName">The name of the header.</param>
        /// <param name="headerValue">The value of the header.</param>
        private static void ValidateHeaderLine(string headerLine, out string headerName, out string headerValue)
        {
            Debug.Assert(!string.IsNullOrEmpty(headerLine), "Expected non-empty header line.");

            int colon = headerLine.IndexOf(':');
            if (colon <= 0)
            {
                throw new ODataException(Strings.ODataAsyncReader_InvalidHeaderSpecified(headerLine));
            }

            headerName = headerLine.Substring(0, colon).Trim();
            headerValue = headerLine.Substring(colon + 1).Trim();
        }

        /// <summary>
        /// Reads a line from the underlying stream.
        /// </summary>
        /// <returns>The line read from the stream.</returns>
        private string ReadLine()
        {
            StringBuilder lineBuilder = new StringBuilder();

            int ch = this.ReadByte();

            while (ch != -1)
            {
                if (ch == '\n')
                {
                    throw new ODataException(Strings.ODataAsyncReader_InvalidNewLineEncountered('\n'));
                }

                if (ch == '\r')
                {
                    ch = this.ReadByte();

                    if (ch != '\n')
                    {
                        throw new ODataException(Strings.ODataAsyncReader_InvalidNewLineEncountered('\r'));
                    }

                    return lineBuilder.ToString();
                }

                lineBuilder.Append((char)ch);
                ch = this.ReadByte();
            }

            throw new ODataException(Strings.ODataAsyncReader_UnexpectedEndOfInput);
        }

        /// <summary>
        /// Reads a byte from the underlying stream.
        /// </summary>
        /// <returns>The byte read from the stream.</returns>
        private int ReadByte()
        {
            return this.rawInputContext.Stream.ReadByte();
        }
    }
}

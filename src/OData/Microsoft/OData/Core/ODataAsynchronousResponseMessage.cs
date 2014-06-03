//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    #endregion Namespaces

    /// <summary>
    /// Representing the message of a non-batch async response.
    /// </summary>
    public sealed class ODataAsynchronousResponseMessage :
#if ODATALIB_ASYNC
        IODataResponseMessageAsync
#else
        IODataResponseMessage
#endif
    {
        /// <summary>True if we are wrting the response; false if we are reading it.</summary>
        private readonly bool writing;

        /// <summary>The stream of the response message.</summary>
        private readonly Stream stream;

        /// <summary>The action to write envelope for the inner message before returning the stream.</summary>
        private readonly Action<ODataAsynchronousResponseMessage> writeEnvelope;

        /// <summary>Prevent the envelope for the inner message from being written more than one time.</summary>
        private bool envelopeWritten;

        /// <summary>The result status code of the response message.</summary>
        private int statusCode;

        /// <summary>The set of headers of the response message.</summary>
        private IDictionary<string, string> headers;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="stream">The stream underlying the response message.</param>
        /// <param name="statusCode">The status code to use for the async response message.</param>
        /// <param name="headers">The headers to use for the async response message.</param>
        /// <param name="writeEnvelope">The action to write envelope for the inner message before returning the stream.</param>
        /// <param name="writing">true if the response message is being written; false when it is read.</param>
        private ODataAsynchronousResponseMessage(
            Stream stream,
            int statusCode,
            IDictionary<string, string> headers,
            Action<ODataAsynchronousResponseMessage> writeEnvelope,
            bool writing)
        {
            Debug.Assert(stream != null, "stream != null");

            this.stream = stream;
            this.statusCode = statusCode;
            this.headers = headers;
            this.writeEnvelope = writeEnvelope;
            this.writing = writing;
        }

        /// <summary>Gets or sets the result status code of the response message.</summary>
        /// <returns>The result status code of the response message.</returns>
        public int StatusCode
        {
            get
            {
                return this.statusCode;
            }

            set
            {
                this.VerifyCanSetHeaderAndStatusCode();

                this.statusCode = value;
            }
        }

        /// <summary>Gets an enumerable over all the headers for this message.</summary>
        /// <returns>An enumerable over all the headers for this message.</returns>
        public IEnumerable<KeyValuePair<string, string>> Headers
        {
            get { return this.headers; }
        }

        /// <summary>Returns a value of an HTTP header of this operation.</summary>
        /// <returns>The value of the HTTP header, or null if no such header was present on the message.</returns>
        /// <param name="headerName">The name of the header to get.</param>
        public string GetHeader(string headerName)
        {
            if (this.headers != null)
            {
                string headerValue;
                if (this.headers.TryGetValue(headerName, out headerValue))
                {
                    return headerValue;
                }
            }

            return null;
        }

        /// <summary>Sets the value of an HTTP header of this operation.</summary>
        /// <param name="headerName">The name of the header to set.</param>
        /// <param name="headerValue">The value of the HTTP header or null if the header should be removed.</param>
        public void SetHeader(string headerName, string headerValue)
        {
            this.VerifyCanSetHeaderAndStatusCode();

            if (headerValue == null)
            {
                if (this.headers != null)
                {
                    this.headers.Remove(headerName);
                }
            }
            else
            {
                if (this.headers == null)
                {
                    this.headers = new Dictionary<string, string>(StringComparer.Ordinal);
                }

                this.headers[headerName] = headerValue;
            }
        }

        /// <summary>Gets the stream backing for this message.</summary>
        /// <returns>The stream backing for this message.</returns>
        public Stream GetStream()
        {
            // If writing response, the envelope for the inner message should be written once and only once before returning the stream.
            if (this.writing && !this.envelopeWritten)
            {
                if (this.writeEnvelope != null)
                {
                    this.writeEnvelope(this);
                }

                this.envelopeWritten = true;
            }
            
            return this.stream;
        }

#if ODATALIB_ASYNC
        /// <summary>Asynchronously get the stream backing for this message.</summary>
        /// <returns>The stream backing for this message.</returns>
        public Task<Stream> GetStreamAsync()
        {
            return Task<Stream>.Factory.StartNew(this.GetStream);
        }
#endif

        /// <summary>
        /// Creates an async response message that can be used to write the response content to.
        /// </summary>
        /// <param name="outputStream">The output stream underlying the response message.</param>
        /// <param name="writeEnvelope">The action to write envelope for the inner message before returning the stream.</param>
        /// <returns>An <see cref="ODataAsynchronousResponseMessage"/> that can be used to write the response content.</returns>
        internal static ODataAsynchronousResponseMessage CreateMessageForWriting(Stream outputStream, Action<ODataAsynchronousResponseMessage> writeEnvelope)
        {
            return new ODataAsynchronousResponseMessage(outputStream, /*statusCode*/0, /*headers*/null, writeEnvelope, /*writing*/true);
        }

        /// <summary>
        /// Creates an async response message that can be used to read the response content from.
        /// </summary>
        /// <param name="stream">The input stream underlying the response message.</param>
        /// <param name="statusCode">The status code to use for the async response message.</param>
        /// <param name="headers">The headers to use for the async response message.</param>
        /// <returns>An <see cref="ODataAsynchronousResponseMessage"/> that can be used to read the response content.</returns>
        internal static ODataAsynchronousResponseMessage CreateMessageForReading(Stream stream, int statusCode, IDictionary<string, string> headers)
        {
            return new ODataAsynchronousResponseMessage(stream, statusCode, headers, /*writeEnvelope*/null, /*writing*/false);
        }

        /// <summary>
        /// Verifies that setting a header or the status code is allowed
        /// </summary>
        /// <remarks>
        /// We allow modifying the headers and the status code only if we are writing the message.
        /// </remarks>
        private void VerifyCanSetHeaderAndStatusCode()
        {
            if (!this.writing)
            {
                throw new ODataException(Strings.ODataAsyncResponseMessage_MustNotModifyMessage);
            }
        }
    }
}

//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData
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
    /// Wrapper class around an IODataResponseMessageAsync to isolate our code from the interface implementation.
    /// </summary>
    /// <remarks>
    /// This class also implements the message interface since it is passed to the payload kind
    /// detection logic on the format implementations and manages the buffering read stream.
    /// </remarks>
    internal sealed class ODataResponseMessage : ODataMessage, 
#if ODATALIB_ASYNC
        IODataResponseMessageAsync
#else
        IODataResponseMessage
#endif
    {
        /// <summary>The response message this class is wrapping.</summary>
        private readonly IODataResponseMessage responseMessage;

        /// <summary>
        /// Constructs an internal wrapper around the <paramref name="responseMessage"/>
        /// that isolates the internal implementation of the ODataLib from the interface.
        /// </summary>
        /// <param name="responseMessage">The response message to wrap.</param>
        /// <param name="writing">true if the message is being written; false when it is read.</param>
        /// <param name="disableMessageStreamDisposal">true if the stream returned should ignore dispose calls.</param>
        /// <param name="maxMessageSize">The maximum size of the message in bytes (or a negative number if no maximum applies).</param>
        internal ODataResponseMessage(IODataResponseMessage responseMessage, bool writing, bool disableMessageStreamDisposal, long maxMessageSize)
            : base(writing, disableMessageStreamDisposal, maxMessageSize)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(responseMessage != null, "responseMessage != null");

            this.responseMessage = responseMessage;
        }

        /// <summary>
        /// The result status code of the response message.
        /// </summary>
        public int StatusCode
        {
            get
            {
                return this.responseMessage.StatusCode;
            }

            set
            {
                throw new ODataException(Strings.ODataMessage_MustNotModifyMessage);
            }
        }

        /// <summary>
        /// Returns an enumerable over all the headers for this message.
        /// </summary>
        public override IEnumerable<KeyValuePair<string, string>> Headers
        {
            get
            {
                return this.responseMessage.Headers;
            }
        }

        /// <summary>
        /// Returns a value of an HTTP header.
        /// </summary>
        /// <param name="headerName">The name of the header to get.</param>
        /// <returns>The value of the HTTP header, or null if no such header was present on the message.</returns>
        public override string GetHeader(string headerName)
        {
            return this.responseMessage.GetHeader(headerName);
        }

        /// <summary>
        /// Sets the value of an HTTP header.
        /// </summary>
        /// <param name="headerName">The name of the header to set.</param>
        /// <param name="headerValue">The value of the HTTP header or 'null' if the header should be removed.</param>
        public override void SetHeader(string headerName, string headerValue)
        {
            this.VerifyCanSetHeader();
            this.responseMessage.SetHeader(headerName, headerValue);
        }

        /// <summary>
        /// Get the stream backing this message.
        /// </summary>
        /// <returns>The stream for this message.</returns>
        public override Stream GetStream()
        {
            return this.GetStream(this.responseMessage.GetStream, /*isRequest*/ false);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously get the stream backing this message.
        /// </summary>
        /// <returns>The stream for this message.</returns>
        public override Task<Stream> GetStreamAsync()
        {
            IODataResponseMessageAsync asyncResponseMessage = this.responseMessage as IODataResponseMessageAsync;
            if (asyncResponseMessage == null)
            {
                throw new ODataException(Strings.ODataResponseMessage_AsyncNotAvailable);
            }

            return this.GetStreamAsync(asyncResponseMessage.GetStreamAsync, /*isRequest*/ false);
        }
#endif
    }
}

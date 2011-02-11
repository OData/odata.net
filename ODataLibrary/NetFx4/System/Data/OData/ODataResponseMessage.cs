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

namespace System.Data.OData
{
    #region Namespaces.
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    #endregion Namespaces.

    /// <summary>
    /// Wrapper class around an IODataResponseMessageAsync to isolate our code from the interface implementation.
    /// </summary>
    internal sealed class ODataResponseMessage : ODataMessage
    {
        /// <summary>The response message this class is wrapping.</summary>
        private readonly IODataResponseMessage responseMessage;

        /// <summary>
        /// Constructs an internal wrapper around the <paramref name="responseMessage"/>
        /// that isolates the internal implementation of the ODataLib from the interface.
        /// </summary>
        /// <param name="responseMessage">The response message to wrap.</param>
        internal ODataResponseMessage(IODataResponseMessage responseMessage)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(responseMessage != null, "responseMessage != null");

            this.responseMessage = responseMessage;
        }

        /// <summary>
        /// Returns an enumerable over all the headers for this message.
        /// </summary>
        internal override IEnumerable<KeyValuePair<string, string>> Headers
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.responseMessage.Headers;
            }
        }

        /// <summary>
        /// Returns a value of an HTTP header.
        /// </summary>
        /// <param name="headerName">The name of the header to get.</param>
        /// <returns>The value of the HTTP header, or null if no such header was present on the message.</returns>
        internal override string GetHeader(string headerName)
        {
            DebugUtils.CheckNoExternalCallers();
            return this.responseMessage.GetHeader(headerName);
        }

        /// <summary>
        /// Sets the value of an HTTP header.
        /// </summary>
        /// <param name="headerName">The name of the header to set.</param>
        /// <param name="headerValue">The value of the HTTP header or 'null' if the header should be removed.</param>
        internal override void SetHeader(string headerName, string headerValue)
        {
            DebugUtils.CheckNoExternalCallers();
            this.responseMessage.SetHeader(headerName, headerValue);
        }

        /// <summary>
        /// Get the stream backing this message.
        /// </summary>
        /// <returns>The stream for this message.</returns>
        internal override Stream GetStream()
        {
            DebugUtils.CheckNoExternalCallers();
            Stream messageStream = this.responseMessage.GetStream();
            ValidateMessageStream(messageStream);
            return messageStream;
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously get the stream backing this message.
        /// </summary>
        /// <returns>The stream for this message.</returns>
        internal override Task<Stream> GetStreamAsync()
        {
            DebugUtils.CheckNoExternalCallers();
            IODataResponseMessageAsync asyncResponseMessage = this.responseMessage as IODataResponseMessageAsync;
            if (asyncResponseMessage == null)
            {
                throw new ODataException(Strings.ODataResponseMessage_AsyncNotAvailable);
            }

            Task<Stream> task = asyncResponseMessage.GetStreamAsync();
            if (task == null)
            {
                throw new ODataException(Strings.ODataResponseMessage_StreamTaskIsNull);
            }

            return task.ContinueWith((streamTask) =>
            {
                Stream messageStream = streamTask.Result;
                ValidateMessageStream(messageStream);
                return messageStream;
            });
        }
#endif

        /// <summary>
        /// Validates that a given message stream can be used.
        /// </summary>
        /// <param name="stream">The stream to validate.</param>
        private static void ValidateMessageStream(Stream stream)
        {
            if (stream == null)
            {
                throw new ODataException(Strings.ODataResponseMessage_MessageStreamIsNull);
            }
        }
    }
}

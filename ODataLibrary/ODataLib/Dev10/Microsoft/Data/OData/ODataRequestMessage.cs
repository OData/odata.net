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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    #endregion Namespaces

    /// <summary>
    /// Wrapper class around an IODataRequestMessageAsync to isolate our code from the interface implementation.
    /// </summary>
    internal sealed class ODataRequestMessage : ODataMessage
    {
        /// <summary>The request message this class is wrapping.</summary>
        private readonly IODataRequestMessage requestMessage;

        /// <summary>
        /// Constructs an internal wrapper around the <paramref name="requestMessage"/>
        /// that isolates the internal implementation of the ODataLib from the interface.
        /// </summary>
        /// <param name="requestMessage">The request message to wrap.</param>
        internal ODataRequestMessage(IODataRequestMessage requestMessage)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(requestMessage != null, "requestMessage != null");

            this.requestMessage = requestMessage;
        }

        /// <summary>
        /// Returns an enumerable over all the headers for this message.
        /// </summary>
        internal override IEnumerable<KeyValuePair<string, string>> Headers
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.requestMessage.Headers;
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
            return this.requestMessage.GetHeader(headerName);
        }

        /// <summary>
        /// Sets the value of an HTTP header.
        /// </summary>
        /// <param name="headerName">The name of the header to set.</param>
        /// <param name="headerValue">The value of the HTTP header or 'null' if the header should be removed.</param>
        internal override void SetHeader(string headerName, string headerValue)
        {
            DebugUtils.CheckNoExternalCallers();
            this.requestMessage.SetHeader(headerName, headerValue);
        }

        /// <summary>
        /// Synchronously get the stream backing this message.
        /// </summary>
        /// <returns>The stream for this message.</returns>
        internal override Stream GetStream()
        {
            DebugUtils.CheckNoExternalCallers();
            Stream messageStream = this.requestMessage.GetStream();
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
            IODataRequestMessageAsync asyncRequestMessage = this.requestMessage as IODataRequestMessageAsync;
            if (asyncRequestMessage == null)
            {
                throw new ODataException(Strings.ODataRequestMessage_AsyncNotAvailable);
            }

            Task<Stream> task = asyncRequestMessage.GetStreamAsync();
            if (task == null)
            {
                throw new ODataException(Strings.ODataRequestMessage_StreamTaskIsNull);
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
                throw new ODataException(Strings.ODataRequestMessage_MessageStreamIsNull);
            }
        }
    }
}

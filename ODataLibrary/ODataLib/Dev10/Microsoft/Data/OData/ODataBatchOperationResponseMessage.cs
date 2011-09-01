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
    using System.IO;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    #endregion Namespaces

    /// <summary>
    /// Message representing an operation in a batch response.
    /// </summary>
#if ODATALIB_ASYNC
    public sealed class ODataBatchOperationResponseMessage : IODataResponseMessageAsync
#else
    public sealed class ODataBatchOperationResponseMessage : IODataResponseMessage
#endif
    {
        /// <summary>
        /// The actual implementation of the message.
        /// We don't derive from this class since we want the actual implementation to remain internal
        /// while this class is public.
        /// </summary>
        private readonly ODataBatchOperationMessage message;

        /// <summary>The result status code of the response message.</summary>
        private int statusCode;

        /// <summary>
        /// Constructor. Creates a request message for an operation of a batch response.
        /// </summary>
        /// <param name="outputStream">The underlying stream to write the message to.</param>
        /// <param name="operationListener">Listener interface to be notified of operation changes.</param>
        internal ODataBatchOperationResponseMessage(Stream outputStream, IODataBatchOperationListener operationListener)
        {
            DebugUtils.CheckNoExternalCallers();

            this.message = new ODataBatchOperationMessage(outputStream, operationListener);
        }

        /// <summary>
        /// The result status code of the response message.
        /// </summary>
        public int StatusCode
        {
            get
            {
                return this.statusCode;
            }

            set
            {
                this.message.VerifyNotCompleted();
                this.statusCode = value;
            }
        }

        /// <summary>
        /// Returns an enumerable over all the headers for this message.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Headers
        {
            get { return this.message.Headers; }
        }

        /// <summary>
        /// Returns the actual operation message which is being wrapped.
        /// </summary>
        internal ODataBatchOperationMessage OperationMessage
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.message;
            }
        }

        /// <summary>
        /// Returns a value of an HTTP header of this operation.
        /// </summary>
        /// <param name="headerName">The name of the header to get.</param>
        /// <returns>The value of the HTTP header, or null if no such header was present on the message.</returns>
        public string GetHeader(string headerName)
        {
            return this.message.GetHeader(headerName);
        }

        /// <summary>
        /// Sets the value of an HTTP header of this operation.
        /// </summary>
        /// <param name="headerName">The name of the header to set.</param>
        /// <param name="headerValue">The value of the HTTP header or 'null' if the header should be removed.</param>
        public void SetHeader(string headerName, string headerValue)
        {
            this.message.SetHeader(headerName, headerValue);
        }

        /// <summary>
        /// Get the stream backing this message.
        /// </summary>
        /// <returns>The stream for this message.</returns>
        public Stream GetStream()
        {
            return this.message.GetStream();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously get the stream backing this message.
        /// </summary>
        /// <returns>The stream for this message.</returns>
        public Task<Stream> GetStreamAsync()
        {
            return this.message.GetStreamAsync();
        }
#endif
    }
}

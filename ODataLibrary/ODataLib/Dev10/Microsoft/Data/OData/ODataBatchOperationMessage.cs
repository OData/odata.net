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
    using System.Linq;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    #endregion Namespaces

    /// <summary>
    /// Implementation class wrapped by the <see cref="ODataBatchOperationRequestMessage"/> and 
    /// <see cref="ODataBatchOperationResponseMessage"/> implementations.
    /// </summary>
    internal sealed class ODataBatchOperationMessage : ODataMessage
    {
        /// <summary>Listener interface to be notified of operation changes.</summary>
        private readonly IODataBatchOperationListener operationListener;

        /// <summary>The stream to write to.</summary>
        private Stream outputStream;

        /// <summary>The set of headers for this operation.</summary>
        private Dictionary<string, string> headers;

        /// <summary>
        /// Constructor. Base class constructor to create a message for an operation of a batch request/response.
        /// </summary>
        /// <param name="outputStream">The stream to write the message to.</param>
        /// <param name="operationListener">Listener interface to be notified of part changes.</param>
        internal ODataBatchOperationMessage(Stream outputStream, IODataBatchOperationListener operationListener)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(outputStream != null, "outputStream != null");
            Debug.Assert(operationListener != null, "operationListener != null");

            this.outputStream = outputStream;
            this.operationListener = operationListener;
        }

        /// <summary>
        /// Returns an enumerable over all the headers for this message.
        /// </summary>
        internal override IEnumerable<KeyValuePair<string, string>> Headers
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.headers ?? Enumerable.Empty<KeyValuePair<string, string>>();
            }
        }

        /// <summary>
        /// Returns a value of an HTTP header of this operation.
        /// </summary>
        /// <param name="headerName">The name of the header to get.</param>
        /// <returns>The value of the HTTP header, or null if no such header was present on the message.</returns>
        internal override string GetHeader(string headerName)
        {
            DebugUtils.CheckNoExternalCallers();
            if (this.headers != null)
            {
                string headerValue = null;
                if (this.headers.TryGetValue(headerName, out headerValue))
                {
                    return headerValue;
                }
            }

            return null;
        }

        /// <summary>
        /// Sets the value of an HTTP header of this operation.
        /// </summary>
        /// <param name="headerName">The name of the header to set.</param>
        /// <param name="headerValue">The value of the HTTP header or 'null' if the header should be removed.</param>
        internal override void SetHeader(string headerName, string headerValue)
        {
            DebugUtils.CheckNoExternalCallers();
            this.VerifyNotCompleted();

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
                    this.headers = new Dictionary<string, string>(EqualityComparer<string>.Default);
                }

                this.headers[headerName] = headerValue;
            }
        }

        /// <summary>
        /// Get the stream backing this message.
        /// </summary>
        /// <returns>The stream for this message.</returns>
        internal override Stream GetStream()
        {
            DebugUtils.CheckNoExternalCallers();
            this.VerifyNotCompleted();

            // notify the listener that the stream has been requested
            this.operationListener.BatchOperationContentStreamRequested();

            // now remember that we are done writing the message data (and only the payload is missing)
            Stream underlyingOutputStream = this.outputStream;
            this.WriteMessageDataCompleted();

            Stream operationStream = new ODataBatchOperationStream(underlyingOutputStream, this.operationListener);
            return operationStream;
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously get the stream backing this message.
        /// </summary>
        /// <returns>The stream for this message.</returns>
        internal override Task<Stream> GetStreamAsync()
        {
            DebugUtils.CheckNoExternalCallers();
            this.VerifyNotCompleted();

            // notify the listener that the stream has been requested
            Task listenerTask = this.operationListener.BatchOperationContentStreamRequestedAsync();

            // now remember that we are done writing the message data (and only the payload is missing)
            Stream underlyingOutputStream = this.outputStream;
            this.WriteMessageDataCompleted();

            Stream operationStream = new ODataBatchOperationStream(underlyingOutputStream, this.operationListener);
            return listenerTask.ContinueWith(
                task => { return operationStream; },
                TaskContinuationOptions.ExecuteSynchronously);
        }
#endif

        /// <summary>
        /// Indicates that the headers and request/response line have been written.
        /// Can be called only once per batch message and headers cannot be modified 
        /// anymore after this method was called.
        /// </summary>
        internal void WriteMessageDataCompleted()
        {
            DebugUtils.CheckNoExternalCallers();
            this.outputStream = null;
        }

        /// <summary>
        /// Verifies that writing of the message has not been completed; this is called from all methods
        /// that are only valid to be called before the message content is written or the message
        /// </summary>
        internal void VerifyNotCompleted()
        {
            DebugUtils.CheckNoExternalCallers();
            if (this.outputStream == null)
            {
                throw new ODataException(Strings.ODataBatchOperationMessage_VerifyNotCompleted);
            }
        }
    }
}

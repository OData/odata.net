//---------------------------------------------------------------------
// <copyright file="ODataBatchOperationMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
#if PORTABLELIB
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

        /// <summary>The URL converter to perform custom URL conversion for URLs read or written from/to the payload.</summary>
        private readonly IODataPayloadUriConverter payloadUriConverter;

        /// <summary>A function to retrieve the content stream for this batch operation message.</summary>
        private Func<Stream> contentStreamCreatorFunc;

        /// <summary>The set of headers for this operation.</summary>
        private ODataBatchOperationHeaders headers;

        /// <summary>
        /// Constructor. Base class constructor to create a message for an operation of a batch request/response.
        /// </summary>
        /// <param name="contentStreamCreatorFunc">A function to retrieve the content stream for this batch operation message.</param>
        /// <param name="headers">The headers of the batch operation message.</param>
        /// <param name="operationListener">Listener interface to be notified of part changes.</param>
        /// <param name="payloadUriConverter">The URL resolver to perform custom URL resolution for URLs read or written from/to the payload.</param>
        /// <param name="writing">true if the request message is being written; false when it is read.</param>
        internal ODataBatchOperationMessage(
            Func<Stream> contentStreamCreatorFunc,
            ODataBatchOperationHeaders headers,
            IODataBatchOperationListener operationListener,
            IODataPayloadUriConverter payloadUriConverter,
            bool writing)
            : base(writing, /*disableMessageStreamDisposal*/ false, /*maxMessageSize*/ -1)
        {
            Debug.Assert(contentStreamCreatorFunc != null, "contentStreamCreatorFunc != null");
            Debug.Assert(operationListener != null, "operationListener != null");

            this.contentStreamCreatorFunc = contentStreamCreatorFunc;
            this.operationListener = operationListener;
            this.headers = headers;
            this.payloadUriConverter = payloadUriConverter;
        }

        /// <summary>
        /// Returns an enumerable over all the headers for this message.
        /// </summary>
        public override IEnumerable<KeyValuePair<string, string>> Headers
        {
            get
            {
                return this.headers ?? Enumerable.Empty<KeyValuePair<string, string>>();
            }
        }

        /// <summary>
        /// Returns a value of an HTTP header of this operation.
        /// </summary>
        /// <param name="headerName">The name of the header to get.</param>
        /// <returns>The value of the HTTP header, or null if no such header was present on the message.</returns>
        public override string GetHeader(string headerName)
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

        /// <summary>
        /// Sets the value of an HTTP header of this operation.
        /// </summary>
        /// <param name="headerName">The name of the header to set.</param>
        /// <param name="headerValue">The value of the HTTP header or 'null' if the header should be removed.</param>
        public override void SetHeader(string headerName, string headerValue)
        {
            this.VerifyNotCompleted();
            this.VerifyCanSetHeader();

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
                    this.headers = new ODataBatchOperationHeaders();
                }

                this.headers[headerName] = headerValue;
            }
        }

        /// <summary>
        /// Get the stream backing this message.
        /// </summary>
        /// <returns>The stream for this message.</returns>
        public override Stream GetStream()
        {
            this.VerifyNotCompleted();

            // notify the listener that the stream has been requested
            this.operationListener.BatchOperationContentStreamRequested();

            // now remember that we are done processing the part header data (and only the payload is missing)
            Stream contentStream = this.contentStreamCreatorFunc();
            this.PartHeaderProcessingCompleted();
            return contentStream;
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously get the stream backing this message.
        /// </summary>
        /// <returns>The stream for this message.</returns>
        public override Task<Stream> GetStreamAsync()
        {
            this.VerifyNotCompleted();

            // notify the listener that the stream has been requested
            Task listenerTask = this.operationListener.BatchOperationContentStreamRequestedAsync();

            // now remember that we are done processing the part header data (and only the payload is missing)
            Stream contentStream = this.contentStreamCreatorFunc();
            this.PartHeaderProcessingCompleted();
            return listenerTask.FollowOnSuccessWith(task => { return (Stream)contentStream; });
        }
#endif

        /// <summary>
        /// Queries the message for the specified interface type.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface to query for.</typeparam>
        /// <returns>The instance of the interface asked for or null if it was not implemented by the message.</returns>
        internal override TInterface QueryInterface<TInterface>()
        {
            return null;
        }

        /// <summary>
        /// Method to implement a custom URL resolution scheme.
        /// This method returns null if not custom resolution is desired.
        /// If the method returns a non-null URL that value will be used without further validation.
        /// </summary>
        /// <param name="baseUri">The (optional) base URI to use for the resolution.</param>
        /// <param name="payloadUri">The URI read from the payload.</param>
        /// <returns>
        /// A <see cref="Uri"/> instance that reflects the custom resolution of the method arguments
        /// into a URL or null if no custom resolution is desired; in that case the default resolution is used.
        /// </returns>
        internal Uri ResolveUrl(Uri baseUri, Uri payloadUri)
        {
            ExceptionUtils.CheckArgumentNotNull(payloadUri, "payloadUri");

            if (this.payloadUriConverter != null)
            {
                return this.payloadUriConverter.ConvertPayloadUri(baseUri, payloadUri);
            }

            // Return null to indicate that no custom URL resolution is desired.
            return null;
        }

        /// <summary>
        /// Indicates that the headers and request/response line have been read or written.
        /// Can be called only once per batch part and headers cannot be modified
        /// anymore after this method was called.
        /// </summary>
        internal void PartHeaderProcessingCompleted()
        {
            this.contentStreamCreatorFunc = null;
        }

        /// <summary>
        /// Verifies that writing of the message has not been completed; this is called from all methods
        /// that are only valid to be called before the message content is written or the message
        /// </summary>
        internal void VerifyNotCompleted()
        {
            if (this.contentStreamCreatorFunc == null)
            {
                throw new ODataException(Strings.ODataBatchOperationMessage_VerifyNotCompleted);
            }
        }
    }
}

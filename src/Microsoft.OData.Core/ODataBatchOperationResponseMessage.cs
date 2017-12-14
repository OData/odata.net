//---------------------------------------------------------------------
// <copyright file="ODataBatchOperationResponseMessage.cs" company="Microsoft">
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
#if PORTABLELIB
    using System.Threading.Tasks;
#endif
    #endregion Namespaces

    /// <summary>
    /// Message representing an operation in a batch response.
    /// </summary>
#if PORTABLELIB
    public sealed class ODataBatchOperationResponseMessage : IODataResponseMessageAsync, IODataPayloadUriConverter, IContainerProvider
#else
    public sealed class ODataBatchOperationResponseMessage : IODataResponseMessage, IODataPayloadUriConverter, IContainerProvider
#endif
    {
        /// <summary>The Content-ID for this response message.</summary>
        public readonly string ContentId;

        /// <summary>
        /// The actual implementation of the message.
        /// We don't derive from this class since we want the actual implementation to remain internal
        /// while this class is public.
        /// </summary>
        private readonly ODataBatchOperationMessage message;

        /// <summary>The result status code of the response message.</summary>
        private int statusCode;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="contentStreamCreatorFunc">A function to retrieve the content stream for this batch operation message.</param>
        /// <param name="headers">The headers of the batch operation message.</param>
        /// <param name="operationListener">Listener interface to be notified of part changes.</param>
        /// <param name="contentId">The content-ID for the operation response message.</param>
        /// <param name="payloadUriConverter">The optional URL converter to perform custom URL conversion for URLs written to the payload.</param>
        /// <param name="writing">true if the request message is being written; false when it is read.</param>
        /// <param name="container">The dependency injection container to get related services.</param>
        /// <param name="groupId">Value for the group id that corresponding request belongs to. Can be null.</param>
        internal ODataBatchOperationResponseMessage(
            Func<Stream> contentStreamCreatorFunc,
            ODataBatchOperationHeaders headers,
            IODataBatchOperationListener operationListener,
            string contentId,
            IODataPayloadUriConverter payloadUriConverter,
            bool writing,
            IServiceProvider container,
            string groupId)
        {
            Debug.Assert(contentStreamCreatorFunc != null, "contentStreamCreatorFunc != null");
            Debug.Assert(operationListener != null, "operationListener != null");

            this.message = new ODataBatchOperationMessage(contentStreamCreatorFunc, headers, operationListener, payloadUriConverter, writing);
            this.ContentId = contentId;
            this.Container = container;
            this.GroupId = groupId;
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
                this.message.VerifyNotCompleted();
                this.statusCode = value;
            }
        }

        /// <summary>Gets an enumerable over all the headers for this message.</summary>
        /// <returns>An enumerable over all the headers for this message.</returns>
        public IEnumerable<KeyValuePair<string, string>> Headers
        {
            get { return this.message.Headers; }
        }

        /// <summary>
        /// The dependency injection container to get related services.
        /// </summary>
        public IServiceProvider Container { get; private set; }

        /// <summary>
        /// The id of the group or change set that this response message is part of.
        /// </summary>
        public string GroupId { get; }

        /// <summary>
        /// Returns the actual operation message which is being wrapped.
        /// </summary>
        internal ODataBatchOperationMessage OperationMessage
        {
            get
            {
                return this.message;
            }
        }

        /// <summary>Returns a value of an HTTP header of this operation.</summary>
        /// <returns>The value of the HTTP header, or null if no such header was present on the message.</returns>
        /// <param name="headerName">The name of the header to get.</param>
        public string GetHeader(string headerName)
        {
            return this.message.GetHeader(headerName);
        }

        /// <summary>Sets the value of an HTTP header of this operation.</summary>
        /// <param name="headerName">The name of the header to set.</param>
        /// <param name="headerValue">The value of the HTTP header or null if the header should be removed.</param>
        public void SetHeader(string headerName, string headerValue)
        {
            this.message.SetHeader(headerName, headerValue);
        }

        /// <summary>Gets the stream backing for this message.</summary>
        /// <returns>The stream backing for this message.</returns>
        public Stream GetStream()
        {
            return this.message.GetStream();
        }

#if PORTABLELIB
        /// <summary>Asynchronously get the stream backing for this message.</summary>
        /// <returns>The stream backing for this message.</returns>
        public Task<Stream> GetStreamAsync()
        {
            return this.message.GetStreamAsync();
        }
#endif

        /// <summary> Method to implement a custom URL resolution scheme. This method returns null if not custom resolution is desired. If the method returns a non-null URL that value will be used without further validation. </summary>
        /// <returns> A <see cref="T:System.Uri" /> instance that reflects the custom resolution of the method arguments into a URL or null if no custom resolution is desired; in that case the default resolution is used. </returns>
        /// <param name="baseUri">The (optional) base URI to use for the resolution.</param>
        /// <param name="payloadUri">The URI read from the payload.</param>
        Uri IODataPayloadUriConverter.ConvertPayloadUri(Uri baseUri, Uri payloadUri)
        {
            return this.message.ResolveUrl(baseUri, payloadUri);
        }
    }
}

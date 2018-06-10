//---------------------------------------------------------------------
// <copyright file="ODataBatchOperationRequestMessage.cs" company="Microsoft">
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
    /// Message representing an operation in a batch request.
    /// </summary>
#if PORTABLELIB
    public sealed class ODataBatchOperationRequestMessage : IODataRequestMessageAsync, IODataPayloadUriConverter, IContainerProvider
#else
    public sealed class ODataBatchOperationRequestMessage : IODataRequestMessage, IODataPayloadUriConverter, IContainerProvider
#endif
    {
        /// <summary>
        /// The Content-ID for this request message.</summary>
        /// <returns>The Content-ID for this request message.</returns>
        public readonly string ContentId;

        /// <summary>
        /// The Group Id for this request message. Can be null.
        /// </summary>
        /// <returns>The Group Id for this request message.</returns>
        private readonly string groupId;

        /// <summary>
        /// The actual implementation of the message.
        /// We don't derive from this class since we want the actual implementation to remain internal
        /// while this class is public.
        /// </summary>
        private readonly ODataBatchOperationMessage message;

        /// <summary>
        /// The list of request prerequisites for execution of  current batch operation.
        /// ODL-caller needs to ensure that all the prerequisites have returned successfully
        /// before current operation can start.
        /// </summary>
        private readonly List<string> dependsOnIds;

        /// <summary>
        /// Constructor. Creates a request message for an operation of a batch request.
        /// </summary>
        /// <param name="contentStreamCreatorFunc">A function to create the content stream.</param>
        /// <param name="method">The HTTP method used for this request message.</param>
        /// <param name="requestUrl">The request Url for this request message.</param>
        /// <param name="headers">The headers for this request message.</param>
        /// <param name="operationListener">Listener interface to be notified of operation changes.</param>
        /// <param name="contentId">The content-ID for the operation request message.</param>
        /// <param name="payloadUriConverter">The optional URL converter to perform custom URL conversion for URLs written to the payload.</param>
        /// <param name="writing">true if the request message is being written; false when it is read.</param>
        /// <param name="container">The dependency injection container to get related services.</param>
        /// <param name="dependsOnIds">
        /// Request or group Ids that current request has dependency on. Values are added to a new list.
        /// Empty list will be created if value is null.
        /// </param>
        /// <param name="groupId">Value for the group id that current request belongs to. Can be null.</param>
        internal ODataBatchOperationRequestMessage(
            Func<Stream> contentStreamCreatorFunc,
            string method,
            Uri requestUrl,
            ODataBatchOperationHeaders headers,
            IODataBatchOperationListener operationListener,
            string contentId,
            IODataPayloadUriConverter payloadUriConverter,
            bool writing,
            IServiceProvider container,
            IEnumerable<string> dependsOnIds,
            string groupId)
        {
            Debug.Assert(contentStreamCreatorFunc != null, "contentStreamCreatorFunc != null");
            Debug.Assert(operationListener != null, "operationListener != null");
            Debug.Assert(payloadUriConverter != null, "payloadUriConverter != null");

            this.Method = method;
            this.Url = requestUrl;
            this.ContentId = contentId;
            this.groupId = groupId;

            this.message = new ODataBatchOperationMessage(contentStreamCreatorFunc, headers, operationListener, payloadUriConverter, writing);
            this.Container = container;

            this.dependsOnIds = dependsOnIds == null
                ? new List<string>()
                : new List<string>(dependsOnIds);
        }

        /// <summary>Gets an enumerable over all the headers for this message.</summary>
        /// <returns>An enumerable over all the headers for this message.</returns>
        public IEnumerable<KeyValuePair<string, string>> Headers
        {
            get { return this.message.Headers; }
        }

        /// <summary>Gets or sets the request URL for this request message.</summary>
        /// <returns>The request URL for this request message.</returns>
        public Uri Url
        {
            get;
            set;
        }

        /// <summary>Gets or Sets the HTTP method used for this request message.</summary>
        /// <returns>The HTTP method used for this request message.</returns>
        public string Method
        {
            get;
            set;
        }

        /// <summary>
        /// The dependency injection container to get related services.
        /// </summary>
        public IServiceProvider Container { get; private set; }

        /// <summary>
        /// The Group Id for this request message. Can be null.
        /// </summary>
        /// <returns>The Group Id for this request message.</returns>
        public string GroupId
        {
            get { return this.groupId; }
        }

        /// <summary>
        /// Gets the prerequisite request or group ids.
        /// </summary>
        public IEnumerable<string> DependsOnIds
        {
            get
            {
                return this.dependsOnIds;
            }
        }

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
        /// <param name="headerValue">The value of the HTTP header or 'null' if the header should be removed.</param>
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

        /// <summary>Implements a custom URL resolution scheme.</summary>
        /// <returns>A <see cref="Uri"/> instance that reflects the custom resolution of the method arguments into a URL or null if no custom resolution is desired; in that case the default resolution is used.</returns>
        /// <param name="baseUri">The (optional) base URI to use for the resolution.</param>
        /// <param name="payloadUri">The URI read from the payload.</param>
        Uri IODataPayloadUriConverter.ConvertPayloadUri(Uri baseUri, Uri payloadUri)
        {
            return this.message.ResolveUrl(baseUri, payloadUri);
        }
    }
}

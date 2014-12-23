//   OData .NET Libraries ver. 6.9
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
    /// Message representing an operation in a batch request.
    /// </summary>
#if ODATALIB_ASYNC
    public sealed class ODataBatchOperationRequestMessage : IODataRequestMessageAsync, IODataUrlResolver
#else
    public sealed class ODataBatchOperationRequestMessage : IODataRequestMessage, IODataUrlResolver
#endif
    {
        /// <summary>Gets or Sets the Content-ID for this request message.</summary>
        /// <returns>The Content-ID for this request message.</returns>
        public readonly string ContentId;

        /// <summary>
        /// The actual implementation of the message.
        /// We don't derive from this class since we want the actual implementation to remain internal
        /// while this class is public.
        /// </summary>
        private readonly ODataBatchOperationMessage message;

        /// <summary>
        /// Constructor. Creates a request message for an operation of a batch request.
        /// </summary>
        /// <param name="contentStreamCreatorFunc">A function to create the content stream.</param>
        /// <param name="method">The HTTP method used for this request message.</param>
        /// <param name="requestUrl">The request Url for this request message.</param>
        /// <param name="headers">The headers for the this request message.</param>
        /// <param name="operationListener">Listener interface to be notified of operation changes.</param>
        /// <param name="contentId">The content-ID for the operation request message.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <param name="writing">true if the request message is being written; false when it is read.</param>
        private ODataBatchOperationRequestMessage(
            Func<Stream> contentStreamCreatorFunc,
            string method,
            Uri requestUrl,
            ODataBatchOperationHeaders headers,
            IODataBatchOperationListener operationListener,
            string contentId,
            IODataUrlResolver urlResolver,
            bool writing)
        {
            Debug.Assert(contentStreamCreatorFunc != null, "contentStreamCreatorFunc != null");
            Debug.Assert(operationListener != null, "operationListener != null");
            Debug.Assert(urlResolver != null, "urlResolver != null");

            this.Method = method;
            this.Url = requestUrl;
            this.ContentId = contentId;

            this.message = new ODataBatchOperationMessage(contentStreamCreatorFunc, headers, operationListener, urlResolver, writing);
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

#if ODATALIB_ASYNC
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
        Uri IODataUrlResolver.ResolveUrl(Uri baseUri, Uri payloadUri)
        {
            return this.message.ResolveUrl(baseUri, payloadUri);
        }

        /// <summary>
        /// Creates an operation request message that can be used to write the operation content to.
        /// </summary>
        /// <param name="outputStream">The output stream underlying the operation message.</param>
        /// <param name="method">The HTTP method to use for the message to create.</param>
        /// <param name="requestUrl">The request URL for the message to create.</param>
        /// <param name="operationListener">The operation listener.</param>
        /// <param name="urlResolver">The (optional) URL resolver for the message to create.</param>
        /// <returns>An <see cref="ODataBatchOperationRequestMessage"/> to write the request content to.</returns>
        internal static ODataBatchOperationRequestMessage CreateWriteMessage(
            Stream outputStream,
            string method,
            Uri requestUrl,
            IODataBatchOperationListener operationListener,
            IODataUrlResolver urlResolver)
        {
            Debug.Assert(outputStream != null, "outputStream != null");
            Debug.Assert(operationListener != null, "operationListener != null");

            Func<Stream> streamCreatorFunc = () => ODataBatchUtils.CreateBatchOperationWriteStream(outputStream, operationListener);
            return new ODataBatchOperationRequestMessage(streamCreatorFunc, method, requestUrl, /*headers*/ null, operationListener, /*contentId*/ null, urlResolver, /*writing*/ true);
        }

        /// <summary>
        /// Creates an operation request message that can be used to read the operation content from.
        /// </summary>
        /// <param name="batchReaderStream">The batch stream underyling the operation response message.</param>
        /// <param name="method">The HTTP method to use for the message to create.</param>
        /// <param name="requestUrl">The request URL for the message to create.</param>
        /// <param name="headers">The headers to use for the operation request message.</param>
        /// <param name="operationListener">The operation listener.</param>
        /// <param name="contentId">The content-ID for the operation request message.</param>
        /// <param name="urlResolver">The (optional) URL resolver for the message to create.</param>
        /// <returns>An <see cref="ODataBatchOperationRequestMessage"/> to read the request content from.</returns>
        internal static ODataBatchOperationRequestMessage CreateReadMessage(
            ODataBatchReaderStream batchReaderStream,
            string method,
            Uri requestUrl,
            ODataBatchOperationHeaders headers,
            IODataBatchOperationListener operationListener,
            string contentId,
            IODataUrlResolver urlResolver)
        {
            Debug.Assert(batchReaderStream != null, "batchReaderStream != null");
            Debug.Assert(operationListener != null, "operationListener != null");

            Func<Stream> streamCreatorFunc = () => ODataBatchUtils.CreateBatchOperationReadStream(batchReaderStream, headers, operationListener);
            return new ODataBatchOperationRequestMessage(streamCreatorFunc, method, requestUrl, headers, operationListener, contentId, urlResolver, /*writing*/ false);
        }
    }
}

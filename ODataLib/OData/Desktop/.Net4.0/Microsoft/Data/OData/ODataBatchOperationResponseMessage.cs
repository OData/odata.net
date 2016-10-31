//   OData .NET Libraries ver. 5.6.3
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
    /// Message representing an operation in a batch response.
    /// </summary>
#if ODATALIB_ASYNC
    public sealed class ODataBatchOperationResponseMessage : IODataResponseMessageAsync, IODataUrlResolver
#else
    public sealed class ODataBatchOperationResponseMessage : IODataResponseMessage, IODataUrlResolver
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
        /// Constructor.
        /// </summary>
        /// <param name="contentStreamCreatorFunc">A function to retrieve the content stream for this batch operation message.</param>
        /// <param name="headers">The headers of the batch operation message.</param>
        /// <param name="operationListener">Listener interface to be notified of part changes.</param>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <param name="writing">true if the request message is being written; false when it is read.</param>
        private ODataBatchOperationResponseMessage(
            Func<Stream> contentStreamCreatorFunc,
            ODataBatchOperationHeaders headers, 
            IODataBatchOperationListener operationListener,
            IODataUrlResolver urlResolver,
            bool writing)
        {
            Debug.Assert(contentStreamCreatorFunc != null, "contentStreamCreatorFunc != null");
            Debug.Assert(operationListener != null, "operationListener != null");

            this.message = new ODataBatchOperationMessage(contentStreamCreatorFunc, headers, operationListener, urlResolver, writing);
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

#if ODATALIB_ASYNC
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
        Uri IODataUrlResolver.ResolveUrl(Uri baseUri, Uri payloadUri)
        {
            return this.message.ResolveUrl(baseUri, payloadUri);
        }

        /// <summary>
        /// Creates an operation response message that can be used to write the operation content to.
        /// </summary>
        /// <param name="outputStream">The output stream underlying the operation message.</param>
        /// <param name="operationListener">The operation listener.</param>
        /// <param name="urlResolver">The (optional) URL resolver for the message to create.</param>
        /// <returns>An <see cref="ODataBatchOperationResponseMessage"/> that can be used to write the operation content.</returns>
        internal static ODataBatchOperationResponseMessage CreateWriteMessage(
            Stream outputStream,
            IODataBatchOperationListener operationListener,
            IODataUrlResolver urlResolver)
        {
            DebugUtils.CheckNoExternalCallers();

            Func<Stream> streamCreatorFunc = () => ODataBatchUtils.CreateBatchOperationWriteStream(outputStream, operationListener);
            return new ODataBatchOperationResponseMessage(streamCreatorFunc, /*headers*/ null, operationListener, urlResolver, /*writing*/ true);
        }

        /// <summary>
        /// Creates an operation response message that can be used to read the operation content from.
        /// </summary>
        /// <param name="batchReaderStream">The batch stream underyling the operation response message.</param>
        /// <param name="statusCode">The status code to use for the operation response message.</param>
        /// <param name="headers">The headers to use for the operation response message.</param>
        /// <param name="operationListener">The operation listener.</param>
        /// <param name="urlResolver">The (optional) URL resolver for the message to create.</param>
        /// <returns>An <see cref="ODataBatchOperationResponseMessage"/> that can be used to read the operation content.</returns>
        internal static ODataBatchOperationResponseMessage CreateReadMessage(
            ODataBatchReaderStream batchReaderStream,
            int statusCode,
            ODataBatchOperationHeaders headers,
            IODataBatchOperationListener operationListener,
            IODataUrlResolver urlResolver)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(batchReaderStream != null, "batchReaderStream != null");
            Debug.Assert(operationListener != null, "operationListener != null");

            Func<Stream> streamCreatorFunc = () => ODataBatchUtils.CreateBatchOperationReadStream(batchReaderStream, headers, operationListener);
            ODataBatchOperationResponseMessage responseMessage = 
                new ODataBatchOperationResponseMessage(streamCreatorFunc, headers, operationListener, urlResolver, /*writing*/ false);
            responseMessage.statusCode = statusCode;
            return responseMessage;
        }
    }
}

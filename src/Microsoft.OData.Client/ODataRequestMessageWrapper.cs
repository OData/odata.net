//---------------------------------------------------------------------
// <copyright file="ODataRequestMessageWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Net;
    using Microsoft.OData;

    /// <summary>
    /// IODataRequestMessage interface implementation.
    /// </summary>
    internal abstract class ODataRequestMessageWrapper
    {
        #region Private Fields

        /// <summary>Request Url.</summary>
        private readonly DataServiceClientRequestMessage requestMessage;

        /// <summary>RequestInfo instance.</summary>
        private readonly RequestInfo requestInfo;

#if DEBUG
        /// <summary>
        /// Keeps track whether FireSendingRequest2 method has been called or not. In debug bits,
        /// we need to make sure that no header is set after this method is called.
        /// </summary>
        private bool fireSendingRequest2MethodCalled;

        /// <summary>
        /// List of headers that were set on the request message when SendingRequest event was fired.
        /// This variable is used to track the headers before SendingRequest event is called,
        /// and to verify no headers is added/modified/remove after sending request event is called.
        /// </summary>
        private HeaderCollection cachedRequestHeaders = null;
#endif
        #endregion // Private Fields

        /// <summary>
        /// Creates a new instance of ODataRequestMessage. This constructor is used for top level requests.
        /// </summary>
        /// <param name="requestMessage">RequestMessage that needs to be wrapped.</param>
        /// <param name="requestInfo">Request Info.</param>
        /// <param name="descriptor">Descriptor for this request.</param>
        protected ODataRequestMessageWrapper(DataServiceClientRequestMessage requestMessage, RequestInfo requestInfo, Descriptor descriptor)
        {
            Debug.Assert(requestMessage != null, "requestMessage != null");
            Debug.Assert(requestInfo != null, "requestInfo != null");
            this.requestMessage = requestMessage;
            this.requestInfo = requestInfo;
            this.Descriptor = descriptor;
        }

        #region Properties

        /// <summary>
        /// Descriptor for this request; or null if there isn't one.
        /// </summary>
        internal Descriptor Descriptor { get; private set; }

        /// <summary>
        /// Return the stream containing the request payload.
        /// </summary>
        internal abstract ContentStream CachedRequestStream
        {
            get;
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to send request data in segments.
        /// </summary>
        internal bool SendChunked
        {
            set { this.requestMessage.SendChunked = value; }
        }

        /// <summary>
        /// Returns true if the message is part of the batch request, otherwise return false;
        /// </summary>
        internal abstract bool IsBatchPartRequest
        {
            get;
        }

#if DEBUG
        /// <summary>
        /// Returns the collection of request headers.
        /// </summary>
        private IEnumerable<KeyValuePair<string, string>> Headers
        {
            get { return this.requestMessage.Headers; }
        }
#endif

        #endregion

        /// <summary>
        /// Create a request message for a batch part from the batch writer. This method copies request headers
        /// from <paramref name="requestMessageArgs"/> in addition to the method and Uri.
        /// </summary>
        /// <param name="batchWriter">ODataBatchWriter instance to build operation message from.</param>
        /// <param name="requestMessageArgs">RequestMessageArgs for the request.</param>
        /// <param name="requestInfo">RequestInfo instance.</param>
        /// <param name="contentId">The Content-ID value to write in ChangeSet head.</param>
        /// <returns>an instance of ODataRequestMessageWrapper.</returns>
        internal static ODataRequestMessageWrapper CreateBatchPartRequestMessage(
            ODataBatchWriter batchWriter,
            BuildingRequestEventArgs requestMessageArgs,
            RequestInfo requestInfo,
            string contentId)
        {
            IODataRequestMessage requestMessage = batchWriter.CreateOperationRequestMessage(requestMessageArgs.Method, requestMessageArgs.RequestUri, contentId);

            foreach (var h in requestMessageArgs.Headers)
            {
                requestMessage.SetHeader(h.Key, h.Value);
            }

            var clientRequestMessage = new InternalODataRequestMessage(requestMessage, false /*allowGetStream*/);
            ODataRequestMessageWrapper messageWrapper = new InnerBatchRequestMessageWrapper(clientRequestMessage, requestMessage, requestInfo, requestMessageArgs.Descriptor);
            return messageWrapper;
        }

        /// <summary>
        /// Create a request message for a non-batch requests and outer $batch request. This method copies request headers
        /// from <paramref name="requestMessageArgs"/> in addition to the method and Uri.
        /// </summary>
        /// <param name="requestMessageArgs">RequestMessageArgs for the request.</param>
        /// <param name="requestInfo">RequestInfo instance.</param>
        /// <returns>an instance of ODataRequestMessageWrapper.</returns>
        internal static ODataRequestMessageWrapper CreateRequestMessageWrapper(BuildingRequestEventArgs requestMessageArgs, RequestInfo requestInfo)
        {
            Debug.Assert(requestMessageArgs != null, "requestMessageArgs != null");

            var requestMessage = requestInfo.CreateRequestMessage(requestMessageArgs);

            if (null != requestInfo.Credentials)
            {
                requestMessage.Credentials = requestInfo.Credentials;
            }

#if !PORTABLELIB // Timeout not available
            if (0 != requestInfo.Timeout)
            {
                requestMessage.Timeout = requestInfo.Timeout;
            }
#endif

            return new TopLevelRequestMessageWrapper(requestMessage, requestInfo, requestMessageArgs.Descriptor);
        }

#if DEBUG
        /// <summary>
        /// Returns the value of the header with the given name.
        /// </summary>
        /// <param name="headerName">Name of the header.</param>
        /// <returns>Returns the value of the header with the given name.</returns>
        internal string GetHeader(string headerName)
        {
            Debug.Assert(!string.IsNullOrEmpty(headerName), "!String.IsNullOrEmpty(headerName)");
            return this.requestMessage.GetHeader(headerName);
        }
#endif

        /// <summary>
        /// Create ODataMessageWriter given the writer settings.
        /// </summary>
        /// <param name="writerSettings">Writer settings.</param>
        /// <param name="isParameterPayload">true if the writer is intended to for a parameter payload, false otherwise.</param>
        /// <returns>An instance of ODataMessageWriter.</returns>
        internal abstract ODataMessageWriter CreateWriter(ODataMessageWriterSettings writerSettings, bool isParameterPayload);

        /// <summary>
        /// Abort the current request.
        /// </summary>
        internal void Abort()
        {
            this.requestMessage.Abort();
        }

        /// <summary>
        /// Sets the value of an HTTP header.
        /// </summary>
        /// <param name="headerName">The name of the header to set.</param>
        /// <param name="headerValue">The value of the HTTP header or 'null' if the header should be removed.</param>
        internal void SetHeader(string headerName, string headerValue)
        {
            this.requestMessage.SetHeader(headerName, headerValue);
        }

        /// <summary>
        /// Begins an asynchronous request for a System.IO.Stream object to use to write data.
        /// </summary>
        /// <param name="callback">The System.AsyncCallback delegate.</param>
        /// <param name="state">The state object for this request.</param>
        /// <returns>An System.IAsyncResult that references the asynchronous request.</returns>
        internal IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
        {
#if DEBUG
            this.ValidateHeaders();
#endif
            return this.requestMessage.BeginGetRequestStream(callback, state);
        }

        /// <summary>
        /// Ends an asynchronous request for a System.IO.Stream object to use to write data.
        /// </summary>
        /// <param name="asyncResult">The pending request for a stream.</param>
        /// <returns>A System.IO.Stream to use to write request data.</returns>
        internal Stream EndGetRequestStream(IAsyncResult asyncResult)
        {
            return this.requestMessage.EndGetRequestStream(asyncResult);
        }

#if !PORTABLELIB

        /// <summary>
        /// Sets the request stream.
        /// </summary>
        /// <param name="requestStreamContent">The content stream to copy into the request stream.</param>
        internal void SetRequestStream(ContentStream requestStreamContent)
        {
            if (requestStreamContent.IsKnownMemoryStream)
            {
                this.SetContentLengthHeader();
            }

#if DEBUG
            this.ValidateHeaders();
#endif
            using (Stream requestStream = this.requestMessage.GetStream())
            {
                if (requestStreamContent.IsKnownMemoryStream)
                {
                    MemoryStream bufferableStream = (MemoryStream)requestStreamContent.Stream;
                    Debug.Assert(bufferableStream.Position == 0, "Cached/buffered stream position should be 0");

                    byte[] buffer = bufferableStream.GetBuffer();
                    int bufferOffset = checked((int)bufferableStream.Position);
                    int bufferLength = checked((int)bufferableStream.Length) - bufferOffset;

                    // the following is useful in the debugging Immediate Window
                    // string x = System.Text.Encoding.UTF8.GetString(buffer, bufferOffset, bufferLength);
                    requestStream.Write(buffer, bufferOffset, bufferLength);
                }
                else
                {
                    byte[] buffer = new byte[WebUtil.DefaultBufferSizeForStreamCopy];
                    WebUtil.CopyStream(requestStreamContent.Stream, requestStream, ref buffer);
                }
            }
        }

#endif

        /// <summary>
        ///  Begins an asynchronous request to an Internet resource.
        /// </summary>
        /// <param name="callback">The System.AsyncCallback delegate.</param>
        /// <param name="state">The state object for this request.</param>
        /// <returns>An System.IAsyncResult that references the asynchronous request for a response.</returns>
        internal IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
        {
#if DEBUG
            this.ValidateHeaders();
#endif
            return this.requestMessage.BeginGetResponse(callback, state);
        }

        /// <summary>
        /// Ends an asynchronous request to an Internet resource.
        /// </summary>
        /// <param name="asyncResult">The pending request for a response.</param>
        /// <returns>A System.Net.WebResponse that contains the response from the Internet resource.</returns>
        internal IODataResponseMessage EndGetResponse(IAsyncResult asyncResult)
        {
            return this.requestMessage.EndGetResponse(asyncResult);
        }

#if !PORTABLELIB
        /// <summary>
        /// Returns a response from an Internet resource.
        /// </summary>
        /// <returns>A System.Net.WebResponse that contains the response from the Internet resource.</returns>
        internal IODataResponseMessage GetResponse()
        {
#if DEBUG
            this.ValidateHeaders();
#endif
            return this.requestMessage.GetResponse();
        }
#endif

        /// <summary>
        /// Sets the content length header for the given request message.
        /// </summary>
        internal void SetContentLengthHeader()
        {
            // Ideally we should set the content length header in data service client. We need to remove this long term.
            // For now, we only set the content length when SendingRequest event is fired.
            // Also make sure that content length header is only set after SendingRequest2 is fired, so that users
            // cannot depend on this functionality. Then when we remove SendingRequest event from the code, we need to
            // go longer set the content length.
#if DEBUG
            Debug.Assert(this.fireSendingRequest2MethodCalled, "FireSendingRequest2 method must be called before setting content length");
#endif
            // Setting the Contentlength when SendingRequest or SendRequest2 is subscribed to
            // because there are chances for the contentlength to be wrong and for a ProtocolViolation to occur
            // in the HttpWebRequest. Previously this was only done for SendingRequest but because we are
            // deprecating SendingRequest it makes sense to do this for SendingRequest2 as people will move their
            // code to use the SendingRequest2. So each event is consistent and will work properly as expected.
            if (this.requestInfo.HasSendingRequest2EventHandlers)
            {
                // Since in V2, we always use to set the content length header after firing the SendingRequest event.
                // Now since we are always setting the content length before firing the SendingRequest event, we need
                // to add the Content-Length header to the list of headers to reset.
                long contentLength = this.CachedRequestStream.Stream.Length;
                this.SetHeader(XmlConstants.HttpContentLength, contentLength.ToString(CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// Fires the following events, in order
        /// 1. WritingRequest
        /// 2. SendingRequest2
        /// </summary>
        /// <param name="descriptor">Descriptor for which this request is getting generated.</param>
        internal void FireSendingEventHandlers(Descriptor descriptor)
        {
            this.FireSendingRequest2(descriptor);
        }

        /// <summary>
        /// FireSendingRequest2 event.
        /// </summary>
        /// <param name="descriptor">Descriptor for which this request is getting generated.</param>
        internal void FireSendingRequest2(Descriptor descriptor)
        {
#if DEBUG
            Debug.Assert(!this.fireSendingRequest2MethodCalled, "!this.fireSendingRequest2MethodCalled");
            Debug.Assert(this.cachedRequestHeaders == null, "this.cachedRequestHeaders == null");

            // Currently for actions there are no descriptors and hence this assert needs to be disabled.
            // Once the actions have descriptors, we can enable this assert.
            // Debug.Assert(
            //    descriptor != null || this.requestMessage.Method == XmlConstants.HttpMethodGet || this.requestMessage.Url.AbsoluteUri.Contains("$batch"),
            //    "For CUD operations, decriptor must be specified in every SendingRequest2 event except top level batch request");
#endif

            // Do we need to fire these events if someone has replaced the transport layer? Maybe no.
            if (this.requestInfo.HasSendingRequest2EventHandlers)
            {
                // For now, we don't think this adds a lot of value exposing on the public DataServiceClientRequestMessage class
                // In future, we can always add it if customers ask for this. Erring on the side of keeping the public
                // class simple.
                var httpWebRequestMessage = this.requestMessage as HttpWebRequestMessage;
                if (httpWebRequestMessage != null)
                {
                    // For now we are saying that anyone who implements the transport layer do not get a chance to fire
                    // SendingRequest yet at all. That does not seem that bad.
                    httpWebRequestMessage.BeforeSendingRequest2Event();
                }

                try
                {
                    this.requestInfo.FireSendingRequest2(new SendingRequest2EventArgs(this.requestMessage, descriptor, this.IsBatchPartRequest));
                }
                finally
                {
                    if (httpWebRequestMessage != null)
                    {
                        httpWebRequestMessage.AfterSendingRequest2Event();
                    }
                }
            }
#if DEBUG
            else
            {
                // Cache the headers if there is no sending request 2 event subscribers. At the time of GetRequestStream
                // or GetSyncronousResponse, we will validate that the headers are the same.
                this.cachedRequestHeaders = new HeaderCollection();
                foreach (var header in this.requestMessage.Headers)
                {
                    this.cachedRequestHeaders.SetHeader(header.Key, header.Value);
                }
            }

            this.fireSendingRequest2MethodCalled = true;
#endif
        }


#if DEBUG
        /// <summary>
        /// The reason for adding this method is that we have seen a couple of asserts that we were not able to figure out why they are getting fired.
        /// So added this method which returns the current headers as well as cached headers as string and we display that in the assert message.
        /// </summary>
        /// <param name="currentHeaders">current header values.</param>
        /// <param name="cachedHeaders">cached header values.</param>
        /// <returns>returns a string which contains both current and cached header names.</returns>
        private static string GetHeaderValues(IEnumerable<KeyValuePair<string, string>> currentHeaders, HeaderCollection cachedHeaders)
        {
            StringBuilder sb = new StringBuilder();
            string separator = String.Empty;
            sb.Append("Current Headers: ");
            foreach (var header in currentHeaders)
            {
                sb.Append(separator);
                sb.Append(header.Key);
                separator = ", ";
            }

            sb.Append(". Headers fired in SendingRequest: ");
            separator = String.Empty;
            foreach (string name in cachedHeaders.HeaderNames)
            {
                sb.Append(separator);
                sb.Append(name);
                separator = ", ";
            }

            return sb.ToString();
        }

        /// <summary>
        /// This method validates that headers values are identical to what they originally were when the request was configured.
        /// DataServiceContext.CachedRequestHeaders is populated in the DataServiceContext.CreateGetRequest method.
        /// </summary>
        private void ValidateHeaders()
        {
            Debug.Assert(this.fireSendingRequest2MethodCalled, "In ValidateHeaders - FireSendingRequest2 method must have been called");

            if (this.cachedRequestHeaders != null)
            {
                Debug.Assert(this.Headers.Count() == this.cachedRequestHeaders.Count, "The request headers count must match" + GetHeaderValues(this.Headers, this.cachedRequestHeaders));

                foreach (KeyValuePair<string, string> header in this.Headers)
                {
                    if (!this.cachedRequestHeaders.HasHeader(header.Key))
                    {
                        Debug.Assert(false, "Missing header: " + GetHeaderValues(this.Headers, this.cachedRequestHeaders));
                    }

                    Debug.Assert(
                        header.Value == this.cachedRequestHeaders.GetHeader(header.Key),
                        String.Format(CultureInfo.InvariantCulture, "The header '{0}' has a different value. Old Value: '{1}', Current Value: '{2}' Please make sure to set the header before SendingRequest event is fired", header.Key, header.Value, this.cachedRequestHeaders.GetHeader(header.Key)));
                }

                this.cachedRequestHeaders = null;
            }
        }
#endif

        /// <summary>
        /// This is a just a pass through implementation of IODataRequestMessage.
        /// In order to keep the sync and non-async code the same, we write all requests into an cached stream and then copy
        /// it to the underlying network stream in sync or async manner.
        /// </summary>
        private class RequestMessageWithCachedStream : IODataRequestMessage
        {
            /// <summary>
            /// IODataRequestMessage implementation that this class wraps.
            /// </summary>
            private readonly DataServiceClientRequestMessage requestMessage;

            /// <summary>
            /// The cached request stream.
            /// </summary>
            private ContentStream cachedRequestStream;

            /// <summary>
            /// Creates a new instance of InternalODataRequestMessage.
            /// </summary>
            /// <param name="requestMessage">IODataRequestMessage that needs to be wrapped.</param>
            internal RequestMessageWithCachedStream(DataServiceClientRequestMessage requestMessage)
            {
                Debug.Assert(requestMessage != null, "requestMessage != null");
                this.requestMessage = requestMessage;
            }

            /// <summary>
            /// Returns the collection of request headers.
            /// </summary>
            public IEnumerable<KeyValuePair<string, string>> Headers
            {
                get { return this.requestMessage.Headers; }
            }

            /// <summary>
            /// Gets or Sets the request url.
            /// </summary>
            public Uri Url
            {
                get { return this.requestMessage.Url; }

                set { throw new NotImplementedException(); }
            }

            /// <summary>
            /// Gets or Sets the http method for this request.
            /// </summary>
            public string Method
            {
                get { return this.requestMessage.Method; }

                set { throw new NotImplementedException(); }
            }

            /// <summary>
            /// Return the stream containing the request payload.
            /// </summary>
            internal ContentStream CachedRequestStream
            {
                get
                {
                    this.cachedRequestStream.Stream.Position = 0;
                    return this.cachedRequestStream;
                }
            }

            /// <summary>
            /// Returns the value of the header with the given name.
            /// </summary>
            /// <param name="headerName">Name of the header.</param>
            /// <returns>Returns the value of the header with the given name.</returns>
            public string GetHeader(string headerName)
            {
                return this.requestMessage.GetHeader(headerName);
            }

            /// <summary>
            /// Sets the value of the header with the given name.
            /// </summary>
            /// <param name="headerName">Name of the header.</param>
            /// <param name="headerValue">Value of the header.</param>
            public void SetHeader(string headerName, string headerValue)
            {
                this.requestMessage.SetHeader(headerName, headerValue);
            }

            /// <summary>
            /// Gets the stream to be used to write the request payload.
            /// </summary>
            /// <returns>Stream to which the request payload needs to be written.</returns>
            public Stream GetStream()
            {
                if (this.cachedRequestStream == null)
                {
                    this.cachedRequestStream = new ContentStream(new MemoryStream(), true /*isKnownMemoryStream*/);
                }

                return this.cachedRequestStream.Stream;
            }
        }

        /// <summary>
        /// This class wraps the request message for non-batch requests and $batch requests.
        /// </summary>
        private class TopLevelRequestMessageWrapper : ODataRequestMessageWrapper
        {
            /// <summary>
            /// Wrapper for the top-level request messages which caches the request stream as it is written. In order to keep the sync and non-async
            /// code the same, we write all requests into an cached stream and then copy it to the underlying network stream in sync or async manner.
            /// </summary>
            private readonly RequestMessageWithCachedStream messageWithCachedStream;

            /// <summary>
            /// Creates a new instance of ODataOuterRequestMessage.
            /// </summary>
            /// <param name="requestMessage">DataServiceClientRequestMessage instance.</param>
            /// <param name="requestInfo">RequestInfo instance.</param>
            /// <param name="descriptor">Descriptor for this request.</param>
            internal TopLevelRequestMessageWrapper(DataServiceClientRequestMessage requestMessage, RequestInfo requestInfo, Descriptor descriptor)
                : base(requestMessage, requestInfo, descriptor)
            {
                // Wrapper for the top-level request messages which caches the request stream as it is written. In order to keep the sync and non-async
                // code the same, we write all requests into an cached stream and then copy it to the underlying network stream in sync or async manner.
                this.messageWithCachedStream = new RequestMessageWithCachedStream(this.requestMessage);
            }

            /// <summary>
            /// Returns true if the message is part of the batch request, otherwise return false;
            /// </summary>
            internal override bool IsBatchPartRequest
            {
                get { return false; }
            }

            /// <summary>
            /// Return the stream containing the request payload.
            /// </summary>
            internal override ContentStream CachedRequestStream
            {
                get
                {
                    Debug.Assert(this.messageWithCachedStream != null, "Cannot access the cached request stream for non-top-level messages.");
                    return this.messageWithCachedStream.CachedRequestStream;
                }
            }

            /// <summary>
            /// Create ODataMessageWriter given the writer settings.
            /// </summary>
            /// <param name="writerSettings">Writer settings.</param>
            /// <param name="isParameterPayload">true if the writer is intended to for a parameter payload, false otherwise.</param>
            /// <returns>An instance of ODataMessageWriter.</returns>
            internal override ODataMessageWriter CreateWriter(ODataMessageWriterSettings writerSettings, bool isParameterPayload)
            {
                // Memory stream to write the request payloads. In order to keep the sync and non-async code same,
                // we write all requests into an cached stream and then copy it to the underlying network stream in sync or async
                // manner.
                return this.requestInfo.WriteHelper.CreateWriter(this.messageWithCachedStream, writerSettings, isParameterPayload);
            }
        }

        /// <summary>
        /// This class wraps the request message for inner batch operations.
        /// </summary>
        private class InnerBatchRequestMessageWrapper : ODataRequestMessageWrapper
        {
            /// <summary>
            /// Inner batch request that ODataLib creates.
            /// </summary>
            private readonly IODataRequestMessage innerBatchRequestMessage;

            /// <summary>
            /// Creates a new instance of InnerBatchRequestMessageWrapper;
            /// </summary>
            /// <param name="clientRequestMessage">Instance of DataServiceClientRequestMessage that represents this request.</param>
            /// <param name="odataRequestMessage">Instance of IODataRequestMessage created by ODataLib.</param>
            /// <param name="requestInfo">RequestInfo instance.</param>
            /// <param name="descriptor">Descriptor for this request.</param>
            internal InnerBatchRequestMessageWrapper(DataServiceClientRequestMessage clientRequestMessage, IODataRequestMessage odataRequestMessage, RequestInfo requestInfo, Descriptor descriptor)
                : base(clientRequestMessage, requestInfo, descriptor)
            {
                this.innerBatchRequestMessage = odataRequestMessage;
            }

            /// <summary>
            /// Returns true if the message is part of the batch request, otherwise return false;
            /// </summary>
            internal override bool IsBatchPartRequest
            {
                get { return true; }
            }

            /// <summary>
            /// Return the stream containing the request payload.
            /// </summary>
            internal override ContentStream CachedRequestStream
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            /// <summary>
            /// Create ODataMessageWriter given the writer settings.
            /// </summary>
            /// <param name="writerSettings">Writer settings.</param>
            /// <param name="isParameterPayload">true if the writer is intended to for a parameter payload, false otherwise.</param>
            /// <returns>An instance of ODataMessageWriter.</returns>
            internal override ODataMessageWriter CreateWriter(ODataMessageWriterSettings writerSettings, bool isParameterPayload)
            {
                // Memory stream to write the request payloads. In order to keep the sync and non-async code same,
                // we write all requests into an cached stream and then copy it to the underlying network stream in sync or async
                // manner.
                return this.requestInfo.WriteHelper.CreateWriter(this.innerBatchRequestMessage, writerSettings, isParameterPayload);
            }
        }
    }
}

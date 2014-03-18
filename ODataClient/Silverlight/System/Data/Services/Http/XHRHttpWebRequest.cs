//---------------------------------------------------------------------
// <copyright file="XHRHttpWebRequest.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      Provides an HTTP-specific implementation of the WebRequest class.
// </summary>
//
// @owner  markash
//---------------------------------------------------------------------

namespace System.Data.Services.Http
{
    #region Namespaces.

    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Windows.Browser;
    using System.Diagnostics;

    #endregion Namespaces.

    /// <summary>
    /// Provides an HTTP-specific implementation of the WebRequest class.
    /// </summary>
    /// <remarks>
    /// See http://www.w3.org/TR/XMLHttpRequest/ for the W3C's specification.
    /// </remarks>
    internal sealed class XHRHttpWebRequest : System.Data.Services.Http.HttpWebRequest
    {
        #region Private fields.

        /// <summary>Whether the request has been aborted.</summary>
        private bool aborted;

        /// <summary>Asynchronous result for the request.</summary>
        private HttpWebRequestAsyncResult asyncRequestResult;

        /// <summary>Asynchronous result for the response.</summary>
        private HttpWebRequestAsyncResult asyncResponseResult;

        /// <summary>Content stream.</summary>
        private NonClosingMemoryStream contentStream;

        /// <summary>Request headers.</summary>
        private System.Data.Services.Http.XHRWebHeaderCollection headers;

        /// <summary>Whether submission has taken place.</summary>
        private bool invoked;

        /// <summary>Method for the request.</summary>
        private string method;

        /// <summary>Response for this request.</summary>
        private System.Data.Services.Http.HttpWebResponse response;

        /// <summary>Browser-based request.</summary>
        private ScriptXmlHttpRequest underlyingRequest;

        /// <summary>Target URI for the request.</summary>
        private Uri uri;

        #endregion Private fields.

        /// <summary>Initializes a new <see cref="HttpWebRequest"/> instance.</summary>
        /// <param name="uri">URI for the request.</param>
        internal XHRHttpWebRequest(Uri uri)
        {
            Debug.Assert(uri != null, "uri != null");
            this.uri = uri;
        }

        /// <summary>Gets or sets the 'Accept' header.</summary>
        public override string Accept
        {
            get
            {
                return this.Headers[System.Data.Services.Http.HttpRequestHeader.Accept];
            }

            set
            {
                ((System.Data.Services.Http.XHRWebHeaderCollection)this.Headers).SetSpecialHeader("accept", value);
            }
        }

        /// <summary>Gets or sets the 'Content-Length' header.</summary>
        public override long ContentLength
        {
            set
            {
                this.Headers[System.Data.Services.Http.HttpRequestHeader.ContentLength] = value.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>Gets or sets a value that indicates whether to buffer the data read from the Internet resource.</summary>
        public override bool AllowReadStreamBuffering
        {
            get { return true; }
            set { /* do nothing, we can't modify this setting and everything is still going to work without it */ }
        }

        /// <summary>
        /// Gets or sets the content type of the request data being sent.
        /// </summary>
        public override string ContentType
        {
            get
            {
                return this.Headers[System.Data.Services.Http.HttpRequestHeader.ContentType];
            }

            set
            {
                ((System.Data.Services.Http.XHRWebHeaderCollection)this.Headers).SetSpecialHeader("content-type", value);
            }
        }

        /// <summary>
        /// Gets the collection of header name/value pairs associated with the request.
        /// </summary>
        public override System.Data.Services.Http.WebHeaderCollection Headers
        {
            get
            {
                if (this.headers == null)
                {
                    this.headers = new System.Data.Services.Http.XHRWebHeaderCollection(System.Data.Services.Http.WebHeaderCollectionType.HttpWebRequest);
                }

                return this.headers;
            }
        }

        /// <summary>
        /// Gets or sets the protocol method to use in this request.
        /// </summary>
        public override string Method
        {
            get { return this.method; }
            set { this.method = value; }
        }

        /// <summary>
        /// Gets the URI of the Internet resource associated with the request.
        /// </summary>
        public override Uri RequestUri
        {
            get { return this.uri; }
        }

        /// <summary>Gets and sets the authentication information used by each query created using the context object.</summary>
        public override System.Net.ICredentials Credentials
        {
            get
            {
                return null;
            }

            set
            {
                throw new NotSupportedException(System.Data.Services.Client.Strings.HttpWebRequest_XmlHttpCredentialsNotSupported);
            }
        }

        /// <summary>Gets or sets a System.Boolean value that controls whether default credentials are sent with requests.</summary>
        public override bool UseDefaultCredentials
        {
            get
            {
                return true;
            }

            set
            {
                if (true != value)
                {
                    throw new NotSupportedException(System.Data.Services.Client.Strings.HttpWebRequest_XmlHttpNonDefaultCredentialsNotSupported);
                }
            }
        }

        /// <summary>
        /// Determines if the XHR can be used from the caller's context
        /// </summary>
        /// <returns>Returns true if the XHR request can be made from the caller's context.</returns>
        public static bool IsAvailable()
        {
            // Simply try to create the script XHR object. If it works, sending request through it will work as well.
            try
            {
                ScriptXmlHttpRequest request = new ScriptXmlHttpRequest();
                return (null != request);
            }
            catch (WebException)
            {
                return false;
            }
        }

        /// <summary>Cancels a request to an Internet resource.</summary>
        public override void Abort()
        {
            this.aborted = true;
            if (this.underlyingRequest != null)
            {
                this.underlyingRequest.Abort();
                this.underlyingRequest.Dispose();
                this.underlyingRequest = null;
            }

            if (this.response != null)
            {
                ((XHRHttpWebResponse)this.response).InternalRequest = null;
                this.response = null;
            }

            this.Close();
        }

        /// <summary>
        /// Provides an asynchronous version of the GetRequestStream method.
        /// </summary>
        /// <param name="callback">The AsyncCallback delegate.</param>
        /// <param name="state">The state object for this request.</param>
        /// <returns>An IAsyncResult that references the asynchronous request.</returns>
        public override IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
        {
            if (this.aborted)
            {
                throw CreateAbortException();
            }

            if (this.contentStream == null)
            {
                this.contentStream = new NonClosingMemoryStream();
            }
            else
            {
                this.contentStream.Seek(0L, SeekOrigin.Begin);
            }
            
            HttpWebRequestAsyncResult asyncResult = new HttpWebRequestAsyncResult(callback, state);
            this.asyncRequestResult = asyncResult;
            this.asyncRequestResult.CompletedSynchronously = true;  // really does complete synchronously
            if (asyncResult != null)
            {
                asyncResult.SetCompleted();
                asyncResult.Callback(asyncResult);
            }
            
            return asyncResult;
        }

        /// <summary>
        /// Begins an asynchronous request for an Internet resource.
        /// </summary>
        /// <param name="callback">The AsyncCallback delegate.</param>
        /// <param name="state">The state object for this request.</param>
        /// <returns>An IAsyncResult that references the asynchronous request for a response.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope",
            Justification = "The only resource disposed by HttpWebRequestAsyncResult is a WaitHandle that is not created until the caller accesses the public AsyncWaitHandle property, therefore it is always the caller's responsibility to dispose this object.")]
        public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
        {
            HttpWebRequestAsyncResult asyncResult = new HttpWebRequestAsyncResult(callback, state);
            try
            {
                asyncResult.InsideBegin = true;
                this.asyncResponseResult = asyncResult;
                this.InvokeRequest();
            }
            finally
            {
                asyncResult.InsideBegin = false;
            }

            return asyncResult;
        }

        /// <summary>
        /// Ends an asynchronous request for a Stream object to use to write data.
        /// </summary>
        /// <param name="asyncResult">The pending request for a stream.</param>
        /// <returns>A Stream to use to write request data.</returns>
        public override Stream EndGetRequestStream(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
            {
                throw new ArgumentNullException("asyncResult");
            }
         
            if (this.asyncRequestResult != asyncResult)
            {
                throw new InvalidOperationException(
                    System.Data.Services.Client.Strings.HttpWeb_Internal("HttpWebRequest.EndGetRequestStream"));
            }
            
            if (this.asyncRequestResult.EndCalled)
            {
                throw new InvalidOperationException(
                    System.Data.Services.Client.Strings.HttpWeb_Internal("HttpWebRequest.EndGetRequestStream.2"));
            }
            
            if (this.aborted)
            {
                throw CreateAbortException();
            }

            this.asyncRequestResult.EndCalled = true;
            this.asyncRequestResult.Dispose();
            this.asyncRequestResult = null;
            return this.contentStream;
        }

        /// <summary>
        /// Ends an asynchronous request to an Internet resource.
        /// </summary>
        /// <param name="asyncResult">The pending request for a response.</param>
        /// <returns>A WebResponse that contains the response from the Internet resource.</returns>
        public override System.Data.Services.Http.WebResponse EndGetResponse(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
            {
                throw new ArgumentNullException("asyncResult");
            }

            if (this.asyncResponseResult != asyncResult)
            {
                throw new InvalidOperationException(
                    System.Data.Services.Client.Strings.HttpWeb_Internal("HttpWebRequest.EndGetResponse"));
            }
            
            if (this.asyncResponseResult.EndCalled)
            {
                throw new InvalidOperationException(
                    System.Data.Services.Client.Strings.HttpWeb_Internal("HttpWebRequest.EndGetResponse.2"));
            }
            
            if (this.aborted)
            {
                throw CreateAbortException();
            }

            this.asyncResponseResult.EndCalled = true;
            this.CreateResponse();
            this.asyncResponseResult.Dispose();
            this.asyncResponseResult = null;
            return this.response;
        }

        /// <summary>
        /// Creates an empty instance of the System.Net.WebHeaderCollection with the right settings for this
        /// HTTP request. The two SL stacks need a bit different way to create this object for backward compatibility.
        /// </summary>
        /// <returns>A new empty instance of the System.Net.WebHeaderCollection.</returns>
        public override System.Net.WebHeaderCollection CreateEmptyWebHeaderCollection()
        {
            // For backward compat we need to return an instance of WebHeaderCollection which is already marked
            //   as HttpWebRequest collection (so it throws on Response headers for example).
            return System.Net.WebRequest.Create(this.RequestUri).Headers;
        }

        /// <summary>Closes this request.</summary>
        internal void Close()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Returns the underlying httpwebrequest instance if the httpStack is clientHttp. Otherwise returns null.
        /// </summary>
        /// <returns>The underlying httpwebrequest instance if the httpStack is clientHttp. Otherwise returns null.</returns>
        internal override System.Net.HttpWebRequest GetUnderlyingHttpRequest()
        {
            return null;
        }

        /// <summary>Gets a stream that can read the response.</summary>
        /// <param name="connection">Connection to close when the stream is response stream is closed.</param>
        /// <returns>A stream with the response.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller is responsible for disposing")]
        internal Stream ReadResponse(IDisposable connection)
        {
            Debug.Assert(connection != null, "connection != null");

            // check for null content type as this may occur with 204 (No Content) responses
            if ((this.response.ContentType == null) ||
                this.response.ContentType.Contains("json") ||
                this.response.ContentType.Contains("xml") ||
                this.response.ContentType.Contains("text") ||
                this.response.ContentType.Contains("multipart"))
            {
                // SQLBU 641147: this is where the browser converts the content from whatever to string
                // then to a UTF8 stream for the client.  Coordinate any change to this with 
                // HttpProcessUtility.EncodingFromName
                // which assumes that for silverlight, all encodings are UTF8 regardless of the charset 
                // in ContentType.
                string buffer = this.underlyingRequest.ReadResponseAsString();

                // fine to only use UTF8 as we already have a .NET string representing the response content 
                return (!string.IsNullOrEmpty(buffer) ? new DisposingMemoryStream(connection, Encoding.UTF8.GetBytes(buffer)) : null);
            }

            // unsupported media type returned
            throw WebException.CreateInternal("HttpWebRequest.ReadResponse");
        }

        /// <summary>Releases resources.</summary>
        /// <param name="disposing">Whether the dispose is being called explicitly rather than by the GC.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", Justification = "(1) contentStream is disposed in the InternalDispose method. (2) response.Dispose() only disposes its reference to this request, so it adds no value. See XHRHttpWebResponse.Close() method for details.")]
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.contentStream != null)
                {
                    this.contentStream.InternalDispose();
                    this.contentStream = null;
                }
            }
        }

        /// <summary>Creates a new WebException for calls into an aborted request.</summary>
        /// <returns>A new WebException instance.</returns>
        private static System.Data.Services.Http.WebException CreateAbortException()
        {
            return new System.Data.Services.Http.WebException(System.Data.Services.Client.Strings.HttpWebRequest_Aborted);
        }

        /// <summary>Gets the status code and headers and initializes the response.</summary>
        private void CreateResponse()
        {
            int statusCode;
            this.underlyingRequest.GetResponseStatus(out statusCode);
            if (statusCode != -1)
            {
                string responseHeaders = this.underlyingRequest.GetResponseHeaders();
                this.response = new System.Data.Services.Http.XHRHttpWebResponse(this, statusCode, responseHeaders);
            }
        }

        /// <summary>
        /// This method is invoked when the readystate property of the XML 
        /// HTTP request changes.
        /// </summary>
        private void ReadyStateChanged()
        {
            if (this.underlyingRequest.IsCompleted && (this.asyncResponseResult != null))
            {
                try
                {
                    if (this.asyncResponseResult.InsideBegin)
                    {
                        this.asyncResponseResult.CompletedSynchronously = true;
                    }

                    this.asyncResponseResult.SetCompleted();
                    this.asyncResponseResult.Callback(this.asyncResponseResult);
                }
                finally
                {
                    this.underlyingRequest.Dispose();
                }
            }
        }

        /// <summary>Calls the 'open' method on the XML HTTP request.</summary>
        private void InvokeRequest()
        {
            if (this.aborted)
            {
                throw CreateAbortException();
            }

            if (this.invoked)
            {
                // Already invoked.
                throw new InvalidOperationException(
                    System.Data.Services.Client.Strings.HttpWeb_Internal("HttpWebRequest.InvokeRequest"));
            }

            this.invoked = true;
            this.underlyingRequest = new ScriptXmlHttpRequest();
            this.underlyingRequest.Open(this.uri.AbsoluteUri, this.Method, (Action)this.ReadyStateChanged);

            if ((this.headers != null) && (this.headers.Count != 0))
            {
                foreach (string header in this.headers.AllKeys)
                {
                    string value = this.headers[header];
                    this.underlyingRequest.SetRequestHeader(header, value);
                }
            }

            string content = null;
            if (this.contentStream != null)
            {
                byte[] buf = this.contentStream.GetBuffer();
                if (buf != null)
                {
                    int bufferSize = checked((int)this.contentStream.Position);
                    content = Encoding.UTF8.GetString(buf, 0, bufferSize);
                    this.underlyingRequest.SetRequestHeader("content-length", bufferSize.ToString(CultureInfo.InvariantCulture));
                }
            }

            this.underlyingRequest.Send(content);
        }

        //// We wont support cross domain requests in Beta 2 as XMLHttpRequest does not support such things
        ////private static bool IsCrossDomainRequest(Uri uri)
        ////{
        ////    if (uri.Port == HtmlPage.Document.DocumentUri.Port)
        ////    {
        ////        string schemeAndServer = uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped);
        ////        string documentSchemeAndServer = HtmlPage.Document.DocumentUri.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped);
        ////        if (schemeAndServer.Equals(documentSchemeAndServer, StringComparison.OrdinalIgnoreCase))
        ////        {
        ////            return false;
        ////        }
        ////    }
        ////    return true;
        ////}

        /// <summary>Asynchronousl result object for the request.</summary>
        private sealed class HttpWebRequestAsyncResult : IAsyncResult, IDisposable
        {
            /// <summary>Delegate to call back on completion.</summary>
            private AsyncCallback callback;

            /// <summary>Whether the request has completed.</summary>
            private bool completed;

            /// <summary>Whether the request has completed synchronously.</summary>
            private bool completedSynchronously;

            /// <summary>Whether the End method of the request has been called.</summary>
            private bool endCalled;

            /// <summary>State object for callback.</summary>
            private object state;

            /// <summary>Signaling object for completion.</summary>
            private ManualResetEvent waitHandle;

            /// <summary>Initializes a new HttpWebRequestAsyncResult instance.</summary>
            /// <param name="callback">Delegate to call back on completion.</param>
            /// <param name="state">State object for callback.</param>
            public HttpWebRequestAsyncResult(AsyncCallback callback, object state)
            {
                this.callback = callback;
                this.state = state;
            }

            /// <summary>State object for callback.</summary>
            public object AsyncState
            {
                get { return this.state; }
            }

            /// <summary>Signaling object for completion.</summary>
            public WaitHandle AsyncWaitHandle
            {
                get
                {
                    if (this.waitHandle == null)
                    {
                        this.waitHandle = new ManualResetEvent(false);
                    }

                    return this.waitHandle;
                }
            }

            /// <summary>Delegate to call back on completion.</summary>
            public AsyncCallback Callback
            {
                get { return this.callback; }
            }

            /// <summary>Whether the request has completed synchronously.</summary>
            public bool CompletedSynchronously
            {
                get
                {
                    return this.completedSynchronously;
                }

                internal set
                {
                    this.completedSynchronously = value;
                }
            }

            /// <summary>Whether the End method of the request has been called.</summary>
            public bool EndCalled
            {
                get { return this.endCalled; }
                set { this.endCalled = value; }
            }

            /// <summary>Whether the request has completed.</summary>
            public bool IsCompleted
            {
                get { return this.completed; }
            }

            /// <summary>Whether processing is in the 'Begin' phase.</summary>
            public bool InsideBegin
            { 
                get; 
                set;
            }

            /// <summary>Releases all resources held onto by this object.</summary>
            public void Dispose()
            {
                if (this.waitHandle != null)
                {
                    ((IDisposable)this.waitHandle).Dispose();
                }
            }

            /// <summary>Sets the result to completed, signaling as necessary.</summary>
            public void SetCompleted()
            {
                this.completed = true;
                if (this.waitHandle != null)
                {
                    this.waitHandle.Set();
                }
            }
        }

        /// <summary>
        /// Use this class to implement a memory stream that can dispose of an object
        /// when closed.
        /// </summary>
        private sealed class DisposingMemoryStream : MemoryStream
        {
            /// <summary>The object to dispose of when this stream is closed.</summary>
            private readonly IDisposable disposable;

            /// <summary>
            /// Initializes a new non-resizable instance of the DisposingMemoryStream
            /// class based on the specified byte array.
            /// </summary>
            /// <param name="disposable">
            /// The object to dispose of when this stream is closed.
            /// </param>
            /// <param name="buffer">
            /// The array of unsigned bytes from which to create the current stream.
            /// </param>
            internal DisposingMemoryStream(IDisposable disposable, byte[] buffer) : base(buffer)
            {
                Debug.Assert(disposable != null, "disposable != null");
                this.disposable = disposable;
            }

            /// <summary>
            /// Releases the unmanaged resources used by the MemoryStream class and 
            /// optionally releases the managed resources.
            /// </summary>
            /// <param name="disposing">
            /// true to release both managed and unmanaged resources; false to release 
            /// only unmanaged resources.
            /// </param>
            protected override void Dispose(bool disposing)
            {
                this.disposable.Dispose();
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Use this class to implement a memory stream which will not close itself
        /// even when Close or Dispose is called on it.
        /// </summary>
        /// <remarks>This is usefull when returning the stream to the user
        /// and then using its methods to access the content. MemoryStream
        /// normally doesn't allow access to its properties/methods when it's been closed.</remarks>
        private sealed class NonClosingMemoryStream : MemoryStream
        {
            /// <summary>
            /// Closes the stream.
            /// </summary>
            public override void Close()
            {
                // Do nothing on close
            }

            /// <summary>
            /// Internal method to truly dispose the stream.
            /// This should be used once no more access to the stream is required.
            /// </summary>
            internal void InternalDispose()
            {
                base.Dispose();
            }

            /// <summary>
            /// Releases the unmanaged resources used by the MemoryStream class and 
            /// optionally releases the managed resources.
            /// </summary>
            /// <param name="disposing">
            /// true to release both managed and unmanaged resources; false to release 
            /// only unmanaged resources.
            /// </param>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2215:DisposeMethodsShouldCallBaseClassDispose", Justification = "The purpose of this class is to override the base class Dispose behavior and do nothing, so we explicitly do not want to call the base here.")]
            protected override void Dispose(bool disposing)
            {
                // Do nothing on dispose
            }
        }
    }
}

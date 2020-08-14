//---------------------------------------------------------------------
// <copyright file="HttpWebRequestMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Globalization;
    using System.Net;
    using Microsoft.OData;

    /// <summary> IODataRequestMessage interface implementation. </summary>
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Returning MemoryStream which doesn't require disposal")]
    [Obsolete("Migrate from the use of HttpWebRequestMessage to HttpClientRequestMessage")]
    public class HttpWebRequestMessage : DataServiceClientRequestMessage, ISendingRequest2
    {
        #region Private Fields
        /// <summary>Request Url.</summary>
        private readonly Uri requestUrl;

        /// <summary> The effective HTTP method. </summary>
        private readonly string effectiveHttpMethod;

        /// <summary>HttpWebRequest instance.</summary>
        private readonly HttpWebRequest httpRequest;

        /// <summary>True if SendingRequest2Event is currently invoked, otherwise false.</summary>
        private bool inSendingRequest2Event;
#if DEBUG
        /// <summary>True if the sendingRequest2 event is already fired.</summary>
        private bool sendingRequest2Fired;
#endif

        #endregion // Private Fields
        /// <summary>
        /// Creates a new instance of HttpWebRequestMessage.
        /// </summary>
        /// <param name="args">Arguments for creating the request message.</param>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "SetHeader is a safe virtual method to be called from the constructor")]
        public HttpWebRequestMessage(DataServiceClientRequestMessageArgs args)
            : base(args.ActualMethod)
        {
            Util.CheckArgumentNull(args, "args");
            Debug.Assert(args.RequestUri.IsAbsoluteUri, "request uri is not absolute uri");
            Debug.Assert(
                args.RequestUri.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase) ||
                args.RequestUri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase),
                "request uri is not for HTTP");
            this.effectiveHttpMethod = args.Method;
            this.requestUrl = args.RequestUri;

            this.httpRequest = HttpWebRequestMessage.CreateRequest(this.ActualMethod, this.Url, args);

            // Now set the headers.
            foreach (var keyValue in args.Headers)
            {
                this.SetHeader(keyValue.Key, keyValue.Value);
            }
        }

        #region Properties

        /// <summary> Returns the request url. </summary>
        public override Uri Url
        {
            get
            {
                return this.requestUrl;
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary> Returns the method for this request. </summary>
        public override string Method
        {
            get
            {
                return this.effectiveHttpMethod;
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary> Returns the collection of request headers. </summary>
        public override IEnumerable<KeyValuePair<string, string>> Headers
        {
            get
            {
                List<KeyValuePair<string, string>> headerValues = new List<KeyValuePair<string, string>>(this.httpRequest.Headers.Count);
                foreach (string headerName in this.httpRequest.Headers.AllKeys)
                {
                    string headerValue = this.httpRequest.Headers[headerName];
                    headerValues.Add(new KeyValuePair<string, string>(headerName, headerValue));
                }

                return headerValues;
            }
        }

        /// <summary> Returns the underlying HttpWebRequest </summary>
        public System.Net.HttpWebRequest HttpWebRequest
        {
            get
            {
                return this.httpRequest;
            }
        }

        /// <summary>
        /// Gets or set the credentials for this request.
        /// </summary>
        public override System.Net.ICredentials Credentials
        {
            get { return this.httpRequest.Credentials; }
            set { this.httpRequest.Credentials = value; }
        }

        /// <summary>
        /// Gets or sets the timeout (in seconds) for this request.
        /// </summary>
        public override int Timeout
        {
            get { return this.httpRequest.Timeout; }
            set { this.httpRequest.Timeout = (int)Math.Min(Int32.MaxValue, new TimeSpan(0, 0, value).TotalMilliseconds); }
        }

        /// <summary>
        /// Gets or sets the read and write timeout (in seconds) for this request.
        /// </summary>
        public override int ReadWriteTimeout
        {
            get { return this.httpRequest.ReadWriteTimeout; }
            set { this.httpRequest.ReadWriteTimeout = (int)Math.Min(Int32.MaxValue, new TimeSpan(0, 0, value).TotalMilliseconds); }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to send data in segments to the
        ///  Internet resource.
        /// </summary>
        public override bool SendChunked
        {
            get { return this.httpRequest.SendChunked; }
            set { this.httpRequest.SendChunked = value; }
        }

        #endregion

        /// <summary>
        /// Returns the value of the header with the given name.
        /// </summary>
        /// <param name="headerName">Name of the header.</param>
        /// <returns>Returns the value of the header with the given name.</returns>
        public override string GetHeader(string headerName)
        {
            Util.CheckArgumentNullAndEmpty(headerName, "headerName");
            return HttpWebRequestMessage.GetHeaderValue(this.httpRequest, headerName);
        }

        /// <summary>
        /// Sets the value of the header with the given name.
        /// </summary>
        /// <param name="headerName">Name of the header.</param>
        /// <param name="headerValue">Value of the header.</param>
        public override void SetHeader(string headerName, string headerValue)
        {
#if DEBUG
            // Only content length header is set after firing SendingRequest2 event
            Debug.Assert(!this.sendingRequest2Fired || headerName == XmlConstants.HttpContentLength, "!this.sendingRequest2Fired || headerName == XmlConstants.HttpContentLength");
#endif
            Util.CheckArgumentNullAndEmpty(headerName, "headerName");
            HttpWebRequestMessage.SetHeaderValue(this.httpRequest, headerName, headerValue);
        }

        /// <summary>
        /// Gets the stream to be used to write the request payload.
        /// </summary>
        /// <returns>Stream to which the request payload needs to be written.</returns>
        public override Stream GetStream()
        {
            if (this.inSendingRequest2Event)
            {
                throw new NotSupportedException(Strings.ODataRequestMessage_GetStreamMethodNotSupported);
            }

            return this.httpRequest.GetRequestStream();
        }

        /// <summary>
        /// Abort the current request.
        /// </summary>
        public override void Abort()
        {
            Debug.Assert(this.httpRequest != null, "this.httpRequest != null");
            this.httpRequest.Abort();
        }

        /// <summary>
        /// Begins an asynchronous request for a System.IO.Stream object to use to write data.
        /// </summary>
        /// <param name="callback">The System.AsyncCallback delegate.</param>
        /// <param name="state">The state object for this request.</param>
        /// <returns>An System.IAsyncResult that references the asynchronous request.</returns>
        public override IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
        {
            // Currently this method cannot get called from SendingRequest2 event - But if we expose these
            // method in the future, this is a good check to have.
            if (this.inSendingRequest2Event)
            {
                throw new NotSupportedException(Strings.ODataRequestMessage_GetStreamMethodNotSupported);
            }

            return this.httpRequest.BeginGetRequestStream(callback, state);
        }

        /// <summary>
        /// Ends an asynchronous request for a System.IO.Stream object to use to write data.
        /// </summary>
        /// <param name="asyncResult">The pending request for a stream.</param>
        /// <returns>A System.IO.Stream to use to write request data.</returns>
        public override Stream EndGetRequestStream(IAsyncResult asyncResult)
        {
            return this.httpRequest.EndGetRequestStream(asyncResult);
        }

        /// <summary>
        ///  Begins an asynchronous request to an Internet resource.
        /// </summary>
        /// <param name="callback">The System.AsyncCallback delegate.</param>
        /// <param name="state">The state object for this request.</param>
        /// <returns>An System.IAsyncResult that references the asynchronous request for a response.</returns>
        public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
        {
            return this.httpRequest.BeginGetResponse(callback, state);
        }

        /// <summary>
        /// Ends an asynchronous request to an Internet resource.
        /// </summary>
        /// <param name="asyncResult">The pending request for a response.</param>
        /// <returns>A System.Net.WebResponse that contains the response from the Internet resource.</returns>
        public override IODataResponseMessage EndGetResponse(IAsyncResult asyncResult)
        {
            Debug.Assert(this.httpRequest != null, "this.httpRequest != null");
            try
            {
                HttpWebResponse httpResponse = (HttpWebResponse)this.httpRequest.EndGetResponse(asyncResult);
                return new HttpWebResponseMessage(httpResponse);
            }
            catch (WebException webException)
            {
                throw ConvertToDataServiceWebException(webException);
            }
        }

        /// <summary>
        /// Returns a response from an Internet resource.
        /// </summary>
        /// <returns>A System.Net.WebResponse that contains the response from the Internet resource.</returns>
        public override IODataResponseMessage GetResponse()
        {

            try
            {
                HttpWebResponse httpResponse = (HttpWebResponse)this.httpRequest.GetResponse();
                //throw new Exception("oooo test");
                return new HttpWebResponseMessage(httpResponse);
            }
            catch (WebException webException)
            {
                //throw new Exception("pppp test" +this.httpRequest.Address+" "+webException.Message);
                throw ConvertToDataServiceWebException(webException);
            }
        }

        /// <summary>
        /// Sets the Content length of the Http web request
        /// </summary>
        /// <param name="httpWebRequest">Http web request to set the content length on</param>
        /// <param name="contentLength">Length to set</param>
        internal static void SetHttpWebRequestContentLength(HttpWebRequest httpWebRequest, long contentLength)
        {
            httpWebRequest.ContentLength = contentLength;
        }

        /// <summary>
        /// Sets the accept charset.
        /// </summary>
        /// <param name="httpWebRequest">The HTTP web request.</param>
        /// <param name="headerValue">The header value.</param>
        internal static void SetAcceptCharset(HttpWebRequest httpWebRequest, string headerValue)
        {
            httpWebRequest.Headers[XmlConstants.HttpAcceptCharset] = headerValue;
        }

        /// <summary>
        /// Attempts to set the UserAgent property if it exists other wise returns false
        /// </summary>
        /// <param name="httpWebRequest">The http web request.</param>
        /// <param name="headerValue">The value of the user agent.</param>
        internal static void SetUserAgentHeader(HttpWebRequest httpWebRequest, string headerValue)
        {
            httpWebRequest.UserAgent = headerValue;
        }

        /// <summary>
        /// Create an instance of HttpWebRequest from the given IODataRequestMessage instance. This method
        /// is also responsible for firing SendingRequest event.
        /// </summary>
        /// <param name="method">Http Method.</param>
        /// <param name="requestUrl">Request Url.</param>
        /// <param name="args">DataServiceClientRequestMessageArgs instance.</param>
        /// <returns>an instance of HttpWebRequest.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "args", Justification = "the parameter is used in the SL version.")]
        private static HttpWebRequest CreateRequest(string method, Uri requestUrl, DataServiceClientRequestMessageArgs args)
        {
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(requestUrl);
            // KeepAlive not available
            httpRequest.KeepAlive = true;
            httpRequest.Method = method;
            return httpRequest;
        }

        /// <summary>
        /// Sets the value of the given header in the httpwebrequest instance.
        /// This has a special case for some of the headers to set the properties on the request instead of using the Headers collection.
        /// </summary>
        /// <param name="request">The request to apply the header to.</param>
        /// <param name="headerName">Name of the header.</param>
        /// <param name="headerValue">Value of the header.</param>
        private static void SetHeaderValue(HttpWebRequest request, string headerName, string headerValue)
        {
            if (string.Equals(headerName, XmlConstants.HttpRequestAccept, StringComparison.OrdinalIgnoreCase))
            {
                request.Accept = headerValue;
            }
            else if (string.Equals(headerName, XmlConstants.HttpContentType, StringComparison.OrdinalIgnoreCase))
            {
                request.ContentType = headerValue;
            }
            else if (string.Equals(headerName, XmlConstants.HttpContentLength, StringComparison.OrdinalIgnoreCase))
            {
                // Keeping long.parse so that this will fail if the string is not a valid long value
                SetHttpWebRequestContentLength(request, long.Parse(headerValue, CultureInfo.InvariantCulture));
            }
            else if (string.Equals(headerName, XmlConstants.HttpUserAgent, StringComparison.OrdinalIgnoreCase))
            {
                SetUserAgentHeader(request, headerValue);
            }
            else if (string.Equals(headerName, XmlConstants.HttpAcceptCharset, StringComparison.OrdinalIgnoreCase))
            {
                SetAcceptCharset(request, headerValue);
            }
            else
            {
                request.Headers[headerName] = headerValue;
            }
        }

        /// <summary>
        /// Get the value of the given header in the httpwebrequest instance.
        /// This has a special case for some of the headers to set the properties on the request instead of using the Headers collection.
        /// </summary>
        /// <param name="request">The request to get the header value from.</param>
        /// <param name="headerName">Name of the header.</param>
        /// <returns>the value of the header with the given name.</returns>
        private static string GetHeaderValue(HttpWebRequest request, string headerName)
        {
            if (string.Equals(headerName, XmlConstants.HttpRequestAccept, StringComparison.OrdinalIgnoreCase))
            {
                return request.Accept;
            }
            else if (string.Equals(headerName, XmlConstants.HttpContentType, StringComparison.OrdinalIgnoreCase))
            {
                return request.ContentType;
            }
            else if (string.Equals(headerName, XmlConstants.HttpContentLength, StringComparison.OrdinalIgnoreCase))
            {
                return request.ContentLength.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                return request.Headers[headerName];
            }
        }

        /// <summary>
        /// Convert the WebException into DataServiceWebException.
        /// </summary>
        /// <param name="webException">WebException instance.</param>
        /// <returns>an instance of DataServiceWebException that abstracts the WebException.</returns>
        private static DataServiceTransportException ConvertToDataServiceWebException(WebException webException)
        {
            HttpWebResponseMessage errorResponseMessage = null;
            if (webException.Response != null)
            {
                var httpResponse = (HttpWebResponse)webException.Response;
                errorResponseMessage = new HttpWebResponseMessage(httpResponse);
            }

            return new DataServiceTransportException(errorResponseMessage, webException);
        }

        /// <summary>
        /// This method is called just before firing SendingRequest2 event.
        /// </summary>
        void ISendingRequest2.BeforeSendingRequest2Event()
        {
            this.inSendingRequest2Event = true;
        }

        /// <summary>
        /// This method is called immd. after SendingRequest2 event has been fired.
        /// </summary>
        void ISendingRequest2.AfterSendingRequest2Event()
        {
            this.inSendingRequest2Event = false;
#if DEBUG
            this.sendingRequest2Fired = true;
#endif
        }

        /*
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override IODataResponseMessage GetResponse()
        {
            throw new NotImplementedException();
        }*/
    }
}

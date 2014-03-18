//---------------------------------------------------------------------
// <copyright file="HttpWebRequestMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
// <summary>
//      IODataRequestMessage interface implementation
// </summary>
//
// @owner  pratikp
//---------------------------------------------------------------------

namespace System.Data.Services.Client
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Globalization;
#if !ASTORIA_LIGHT
    using System.Net;
#else // Data.Services http stack
    using System.Data.Services.Http;
#endif
#if PORTABLELIB
    using System.Reflection;
    using System.Xml;
#endif
    using Microsoft.Data.OData;

    /// <summary> IODataRequestMessage interface implementation. </summary>
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Returning MemoryStream which doesn't require disposal")]
    public class HttpWebRequestMessage : DataServiceClientRequestMessage
    {
        #region Private Fields

#if PORTABLELIB
        /// <summary> Running on silverlight </summary>
        internal static bool IsRunningOnSilverlight = false;

        /// <summary>Cached method info that will be used to call to set the userAgent property if its not null. </summary>
        private static MethodInfo UserAgentMethodSetter;

        /// <summary>Cached method info that will be used to call to set the ContentLength property if its not null. </summary>
        private static MethodInfo ContentLengthMethodSetter;

        /// <summary> Indicates the current platform the portablelib is running on can't set the user agent property </summary>
        private static bool SettingUserAgentPropertyValidOnCurrentPlatform = true;

        /// <summary> Indicates that the current platform doesn't support setting the header useragent property </summary>
        private static bool SettingUserAgentOnHeadersCollectionValidOnCurrentPlatform = true;

        /// <summary> Indicates the current platform the portablelib is running on can't set the user agent property </summary>
        private static bool SettingContentLengthPropertyValidOnCurrentPlatform = true;

        /// <summary> Indicates that the current platform doesn't support setting the header useragent property </summary>
        private static bool SettingContentLengthOnHeadersCollectionValidOnCurrentPlatform = true;
#endif
        /// <summary>Request Url.</summary>
        private readonly Uri requestUrl;

        /// <summary> The effective HTTP method. </summary>
        private readonly string effectiveHttpMethod;

        /// <summary>RequestInfo instance.</summary>
        private readonly RequestInfo requestInfo;

        /// <summary>HttpWebRequest instance.</summary>
        private HttpWebRequest httpRequest;

        /// <summary>List of header value to reset after SendingRequest event is fired.</summary>
        private List<string> headersToReset;

        /// <summary>True if the FireSendingRequest method is already called, otherwise false.</summary>
        private bool fireSendingRequestMethodCalled;

        /// <summary>True if SendingRequest2Event is currently invoked, otherwise false.</summary>
        private bool inSendingRequest2Event;
#if DEBUG
        /// <summary>True if the sendingRequest2 event is already fired.</summary>
        private bool sendingRequest2Fired;
#endif

        #endregion // Private Fields

#if PORTABLELIB
        /// <summary>
        /// Initializes the <see cref="HttpWebRequestMessage"/> class.
        /// </summary>
        static HttpWebRequestMessage()
        {
            IsRunningOnSilverlight = typeof(System.Windows.Input.ICommand).Assembly.GetType("System.Net.Browser.WebRequestCreator", false) != null;

            PropertyInfo pi = typeof(HttpWebRequest).GetProperty("ContentLength");
            if (pi != null)
            {
                ContentLengthMethodSetter = pi.GetSetMethod();
            }

            pi = typeof(HttpWebRequest).GetProperty("UserAgent");
            if (pi != null)
            {
                UserAgentMethodSetter = pi.GetSetMethod();
            }
        }
#endif

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

        /// <summary>
        /// Creates a new instance of HttpWebRequestMessage.
        /// </summary>
        /// <param name="args">Arguments for creating the request message.</param>
        /// <param name="requestInfo">RequestInfo instance.</param>
        internal HttpWebRequestMessage(DataServiceClientRequestMessageArgs args, RequestInfo requestInfo)
            : this(args)
        {
            Debug.Assert(requestInfo != null, "requestInfo != null");
            this.requestInfo = requestInfo;
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
#if !ASTORIA_LIGHT
                return this.httpRequest;
#else
                return this.httpRequest.GetUnderlyingHttpRequest();
#endif
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

#if !ASTORIA_LIGHT && !PORTABLELIB
        /// <summary>
        /// Gets or sets the timeout (in seconds) for this request.
        /// </summary>
        public override int Timeout
        {
            get { return this.httpRequest.Timeout; }
            set { this.httpRequest.Timeout = (int)Math.Min(Int32.MaxValue, new TimeSpan(0, 0, value).TotalMilliseconds); }
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
#endif
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
#if ASTORIA_LIGHT || PORTABLELIB
            // not supported in async environments. Another IODataRequestMessage which buffers the request should be used.
            throw new NotSupportedException(Strings.ODataRequestMessage_GetStreamMethodNotSupported);
#else
            Debug.Assert(!this.fireSendingRequestMethodCalled, "!this.fireSendingRequestMethodCalled");

            if (this.inSendingRequest2Event)
            {
                throw new NotSupportedException(Strings.ODataRequestMessage_GetStreamMethodNotSupported);
            }

            this.FireSendingRequest();
            return this.httpRequest.GetRequestStream();
#endif
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
            Debug.Assert(!this.fireSendingRequestMethodCalled, "!this.fireSendingRequestMethodCalled");

            // Currently this method cannot get called from SendingRequest2 event - But if we expose these
            // method in the future, this is a good check to have. Also this checks prevents us from writing
            // a lot of #if ASTORIA_LIGHT code.
            if (this.inSendingRequest2Event)
            {
                throw new NotSupportedException(Strings.ODataRequestMessage_GetStreamMethodNotSupported);
            }

            this.FireSendingRequest();
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
            this.FireSendingRequest();
            return this.httpRequest.BeginGetResponse(callback, state);
        }

        /// <summary>
        /// Ends an asynchronous request to an Internet resource.
        /// </summary>
        /// <param name="asyncResult">The pending request for a response.</param>
        /// <returns>A System.Net.WebResponse that contains the response from the Internet resource.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0013:HttpWebResponseMustBeEncapsulated", Justification = "Particular Silverlight stacks use HttpWebResponse others use XmlHttp.")]
        public override IODataResponseMessage EndGetResponse(IAsyncResult asyncResult)
        {
            Debug.Assert(this.httpRequest != null, "this.httpRequest != null");
            try
            {
                HttpWebResponse httpResponse = (HttpWebResponse)this.httpRequest.EndGetResponse(asyncResult);
#if PORTABLELIB
                if (!httpResponse.SupportsHeaders)
                {
                    throw new NotSupportedException(Strings.Silverlight_BrowserHttp_NotSupported);
                }
#endif
#if !ASTORIA_LIGHT
                return new HttpWebResponseMessage(httpResponse);
#else
                // Bug 1447942: Required to do this as the Request could be made via xmlHttp and not have an HttpWebResponse.
                System.Net.HttpWebResponse underlyingHttpResponse = httpResponse.GetUnderlyingHttpResponse();
                if (underlyingHttpResponse != null)
                {
                    return new HttpWebResponseMessage(underlyingHttpResponse); 
                }
                else
                {
                    HeaderCollection headerCollection = new HeaderCollection(httpResponse.Headers);
                    return new HttpWebResponseMessage(headerCollection, (int)httpResponse.StatusCode, httpResponse.GetResponseStream);
                }
#endif
            }
            catch (WebException webException)
            {
                throw ConvertToDataServiceWebException(webException);
            }
        }

#if !ASTORIA_LIGHT && !PORTABLELIB
        /// <summary>
        /// Returns a response from an Internet resource.
        /// </summary>
        /// <returns>A System.Net.WebResponse that contains the response from the Internet resource.</returns>
        public override IODataResponseMessage GetResponse()
        {
            this.FireSendingRequest();

            try
            {
                HttpWebResponse httpResponse = (HttpWebResponse)this.httpRequest.GetResponse();
                return new HttpWebResponseMessage(httpResponse);
            }
            catch (WebException webException)
            {
                throw ConvertToDataServiceWebException(webException);
            }
        }
#endif

        /// <summary>
        /// Sets the Content length of the Http web request
        /// </summary>
        /// <param name="httpWebRequest">Http web request to set the content length on</param>
        /// <param name="contentLength">Length to set</param>
        internal static void SetHttpWebRequestContentLength(HttpWebRequest httpWebRequest, long contentLength)
        {
#if PORTABLELIB
            // TODO: May need to add a special case for when the content length is zero
            bool headerSet = false;
            if (ContentLengthMethodSetter != null && SettingContentLengthPropertyValidOnCurrentPlatform)
            {
                try
                {
                    ContentLengthMethodSetter.Invoke(httpWebRequest, new object[] { contentLength });
                    headerSet = true;
                }
                catch (InvalidOperationException)
                {
                    SettingContentLengthPropertyValidOnCurrentPlatform = false;
                }
            }

            if (!headerSet && SettingContentLengthOnHeadersCollectionValidOnCurrentPlatform)
            {
                // Attempting to set the header but may just not work
                try
                {
                    // Need this code to set the user agent on silverlight when the property doesn't exist
                    httpWebRequest.Headers[XmlConstants.HttpContentLength] = XmlConvert.ToString(contentLength);
                }
                catch (ArgumentException)
                {
                    SettingContentLengthOnHeadersCollectionValidOnCurrentPlatform = false;
                }
            }
#else
            httpWebRequest.ContentLength = contentLength;
#endif
        }

        /// <summary>
        /// Sets the accept charset.
        /// </summary>
        /// <param name="httpWebRequest">The HTTP web request.</param>
        /// <param name="headerValue">The header value.</param>
        internal static void SetAcceptCharset(HttpWebRequest httpWebRequest, string headerValue)
        {
#if !ASTORIA_LIGHT && !PORTABLELIB
            httpWebRequest.Headers[XmlConstants.HttpAcceptCharset] = headerValue;
#endif
#if PORTABLELIB
            if (!IsRunningOnSilverlight)
            {
                httpWebRequest.Headers[XmlConstants.HttpAcceptCharset] = headerValue;
            }
#endif
        }

        /// <summary>
        /// Attempts to set the UserAgent property if it exists other wise returns false
        /// </summary>
        /// <param name="httpWebRequest">The http web request.</param>
        /// <param name="headerValue">The value of the user agent.</param>
        internal static void SetUserAgentHeader(HttpWebRequest httpWebRequest, string headerValue)
        {
#if !ASTORIA_LIGHT && !PORTABLELIB
            httpWebRequest.UserAgent = headerValue;
#endif
#if PORTABLELIB
            if (IsRunningOnSilverlight || !SettingUserAgentPropertyValidOnCurrentPlatform)
            {
                return;
            }

            bool headerSet = false;

            // Weird behavior, looks like this won't work in silverlight on the .net portable libaries. 
            // http://social.msdn.microsoft.com/Forums/en-US/netfxbcl/thread/7440fc9d-3490-4823-ac93-73706b535c6a/
            // http://connect.microsoft.com/VisualStudio/feedback/details/770104/cannot-set-useragent-of-a-httpwebrequest-in-portable-class-and-winrt-libraries
            // On Windows 8 store throws InvalidOperationException when setting the UserAgent
            if (UserAgentMethodSetter != null)
            {
                try
                {
                    UserAgentMethodSetter.Invoke(httpWebRequest, new object[] { headerValue });
                    headerSet = true;
                }
                catch (InvalidOperationException)
                {
                    SettingUserAgentPropertyValidOnCurrentPlatform = false;
                }
                catch (TargetInvocationException)
                {
                    SettingUserAgentPropertyValidOnCurrentPlatform = false;
                }
            }

            if (!headerSet || SettingUserAgentOnHeadersCollectionValidOnCurrentPlatform)
            {
                // Attempting to set the header but may just not work
                try
                {
                    // Need this code to set the user agent on silverlight when the property doesn't exist
                    httpWebRequest.Headers[XmlConstants.HttpUserAgent] = headerValue;
                }
                catch (ArgumentException)
                {
                    SettingUserAgentOnHeadersCollectionValidOnCurrentPlatform = false;
                }
            }
#endif
#if ASTORIA_LIGHT
            // Silverlight user agent cannot be set via a property, doesn't exist
            httpWebRequest.Headers[XmlConstants.HttpUserAgent] = headerValue;
#endif
        }

        /// <summary>
        /// Add the list of header names to the list of headers whose values need to be reset after firing sendingrequest event.
        /// </summary>
        /// <param name="headerNames">List of header names.</param>
        internal void AddHeadersToReset(IEnumerable<string> headerNames)
        {
            Debug.Assert(!this.fireSendingRequestMethodCalled, "!this.fireSendingRequestMethodCalled");
            if (this.headersToReset == null)
            {
                this.headersToReset = new List<string>();
            }

            this.headersToReset.AddRange(headerNames);
        }

        /// <summary>
        /// This method is called just before firing SendingRequest2 event.
        /// </summary>
        internal void BeforeSendingRequest2Event()
        {
            this.inSendingRequest2Event = true;
        }

        /// <summary>
        /// This method is called immd. after SendingRequest2 event has been fired.
        /// </summary>
        internal void AfterSendingRequest2Event()
        {
            this.inSendingRequest2Event = false;
#if DEBUG
            this.sendingRequest2Fired = true;
#endif
        }

        /// <summary>
        /// Set the header values on the request.
        /// </summary>
        /// <param name="requestMessage">IODataRequestMessage instance containing all the headers.</param>
        /// <param name="cachedHeaders">Dictionary of cached headers.</param>
        /// <param name="effectiveHttpMethod">The Effective http method.</param>
        private static void SetHeaderValues(HttpWebRequestMessage requestMessage, HeaderCollection cachedHeaders, string effectiveHttpMethod)
        {
            // DevNote(shank): We used to set request.AllowWriteStreamBuffering to true here. This is now removed
            //                 as it prevents customers from enabling HTTP streaming by setting it to false in the
            //                 SendingRequest event. This is because this method is called twice by CreateGetRequest(),
            //                 before *and* after the SendingRequest event (see DevNote in CreateGetRequest()).
            //                 We also do *not* set it to false by default for media resource requests. Enabling
            //                 streaming requires additional actions to be performed by the customer, e.g., sending
            //                 chunks if content-length is not specified, and pre-authenticating with a HEAD request.
#if !ASTORIA_LIGHT && !PORTABLELIB
            bool removeXMethod = true;
#endif
            Debug.Assert(requestMessage.requestInfo != null, "This method is called from FireSendingRequest. Hence requestInfo should never be null.");
            HttpWebRequest request = requestMessage.httpRequest;
            string method = requestMessage.Method;
            string contentType = null;
            cachedHeaders.TryGetHeader(XmlConstants.HttpContentType, out contentType);

            if (string.CompareOrdinal(effectiveHttpMethod, XmlConstants.HttpMethodGet) != 0)
            {
                if (string.CompareOrdinal(effectiveHttpMethod, XmlConstants.HttpMethodDelete) == 0)
                {
                    // In V2, we always use to over-write content type after sendingRequest event has been called.
                    // So doing the same for delete requests.
                    // TODO no need to override this if it was set in building request; just override it was changed elsewhere?
                    Debug.Assert(String.IsNullOrEmpty(contentType), "Content-Type must not be specified for delete operation");
                    request.ContentType = null;
                    SetHttpWebRequestContentLength(request, 0);
                }
                else
                {
                    Debug.Assert(!String.IsNullOrEmpty(contentType), "Content-Type must be specified for non get and non delete operations");
                    request.ContentType = contentType;
                }

                if (requestMessage.requestInfo.UsePostTunneling && (string.CompareOrdinal(effectiveHttpMethod, XmlConstants.HttpMethodPost) != 0))
                {
                    Debug.Assert(effectiveHttpMethod != null, "expected an effectiveHttpMethod");
                    request.Headers[XmlConstants.HttpXMethod] = effectiveHttpMethod;
                    method = XmlConstants.HttpMethodPost;
#if !ASTORIA_LIGHT && !PORTABLELIB
                    removeXMethod = false;
#endif
                }
            }

            // We cannot assert that the content type for GET request is null, since in some scenarios (e.g. GetReadStream),
            // the user can pass the content type in the API and they can set the content type to something. In those
            // scenarios, we set the content type as specified by the user and not check for anything.
#if !ASTORIA_LIGHT && !PORTABLELIB
            // Removing the If-Match header so that if it was accidently written when it should not
            // have, it will get removed after the event. The correct value will be written again
            // when reseting the headers.
            request.Headers.Remove(HttpRequestHeader.IfMatch);

            // alternate HttpXMethod header doesn't work
            if (removeXMethod)
            {
                request.Headers.Remove(XmlConstants.HttpXMethod);
            }
#endif
            request.Method = method;

            // Reset the list of headers, if any
            if (requestMessage.headersToReset != null)
            {
                foreach (string headerName in requestMessage.headersToReset)
                {
#if ASTORIA_LIGHT || PORTABLELIB
                    // Ignoring content length header since its not supported in silverlight
                    if (string.Equals(headerName, XmlConstants.HttpContentLength, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
#endif
                    HttpWebRequestMessage.SetHeaderValue(request, headerName, cachedHeaders.GetHeader(headerName));
                }
            }
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
#if !ASTORIA_LIGHT
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(requestUrl);
#if PORTABLELIB
            // Set same default as Silverlight when running silverlight in .Net Portable
            if (IsRunningOnSilverlight)
            {
                httpRequest.UseDefaultCredentials = args.UseDefaultCredentials;
            }
#endif
#else
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(requestUrl, args.ClientHttpStack);
            httpRequest.UseDefaultCredentials = args.UseDefaultCredentials;
#endif

#if !ASTORIA_LIGHT && !PORTABLELIB
            // KeepAlive not available
            httpRequest.KeepAlive = true;
#endif
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
#if !ASTORIA_LIGHT && !PORTABLELIB
            else if (string.Equals(headerName, XmlConstants.HttpContentLength, StringComparison.OrdinalIgnoreCase))
            {
                return request.ContentLength.ToString(CultureInfo.InvariantCulture);
            }
#endif
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
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller will dispose the message later.")]
        private static DataServiceTransportException ConvertToDataServiceWebException(WebException webException)
        {
            HttpWebResponseMessage errorResponseMessage = null;
            if (webException.Response != null)
            {
                var httpResponse = (HttpWebResponse)webException.Response;
#if ASTORIA_LIGHT
                errorResponseMessage = new HttpWebResponseMessage(httpResponse.GetUnderlyingHttpResponse());
#else
                errorResponseMessage = new HttpWebResponseMessage(httpResponse);
#endif
            }

            return new DataServiceTransportException(errorResponseMessage, webException);
        }

        /// <summary>
        /// Fire SendingRequest event if its conditions are met. 
        /// If the user has a handler for BuildingRequest, we will throw.
        /// If the user has no BuildingRequest handlers but does have a SendingRequest2 handler, we silently do not fire this event (this is shipped 5.0 behavior).
        /// </summary>
        private void FireSendingRequest()
        {
            // Do not fire SendingRequest event when user tries to wrap the HttpWebRequestMessage
            if (this.fireSendingRequestMethodCalled || this.requestInfo == null)
            {
                return;
            }

            // We need to set this before SendingRequest event is fired so that
            // GetStream method can throw if it is called from SendingRequest event.
            this.fireSendingRequestMethodCalled = true;

            HeaderCollection cachedHeaders = null;
            if (this.requestInfo.HasSendingRequestEventHandlers)
            {
                // Before firing SendingRequest event, we need to cache all the header values so that
                // we can reset them to the original values after SendingRequest event has been fired.
                cachedHeaders = new HeaderCollection();
                foreach (var header in this.Headers)
                {
                    cachedHeaders.SetHeader(header.Key, header.Value);
                }
#if PORTABLELIB
                cachedHeaders.SetHeader(XmlConstants.HttpContentLength, this.httpRequest.Headers[XmlConstants.HttpContentLength]);
#endif
#if !ASTORIA_LIGHT && !PORTABLELIB
                // Content-Length and accept header does not show up in the header collection at all.
                // Hence adding it explicitly, since we reset the content length header
                // after firing SendingRequest event
                cachedHeaders.SetHeader(XmlConstants.HttpContentLength, this.httpRequest.ContentLength.ToString(CultureInfo.InvariantCulture));
#endif
            }

            // Fires whenever a new HttpWebRequest has been created
            // The event fires early - before the client library sets many of its required property values.
            // This ensures the client library has the last say on the value of mandated properties
            // such as the HTTP verb  being used for the request.
            if (this.requestInfo.HasSendingRequestEventHandlers)
            {
                System.Net.WebHeaderCollection requestHeaders;
#if !ASTORIA_LIGHT
                requestHeaders = this.httpRequest.Headers;
                SendingRequestEventArgs args = new SendingRequestEventArgs(this.httpRequest, requestHeaders);
#else
                requestHeaders = this.httpRequest.CreateEmptyWebHeaderCollection();
                SendingRequestEventArgs args = new SendingRequestEventArgs(null, requestHeaders);
#endif
                this.requestInfo.FireSendingRequest(args);

#if !ASTORIA_LIGHT
                if (!Object.ReferenceEquals(args.Request, this.httpRequest))
                {
                    this.httpRequest = (System.Net.HttpWebRequest)args.Request;
                }
#else
                // apply all headers to the request
                foreach (string key in requestHeaders.AllKeys)
                {
                    this.httpRequest.Headers[key] = requestHeaders[key];
                }
#endif
                // Bug 14719: why do we need to reset the values back? if the user changes the header value, then
                // we should not try to fix it for him. Fixing this is a breaking change and hence not doing that.
                // Invoking the callback again to avoid being breaking change, since in V2, we used to set these values
                // after raising the OnSendingRequest event.
                HttpWebRequestMessage.SetHeaderValues(this, cachedHeaders, this.Method);
            }
        }
    }
}

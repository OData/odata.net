//---------------------------------------------------------------------
// <copyright file="HttpClientRequestMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{ 
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Microsoft.OData;

    /// <summary>
    /// HttpClient based implementation of DataServiceClientRequestMessage.
    /// </summary>
    public class HttpClientRequestMessage : DataServiceClientRequestMessage, ISendingRequest2, IDisposable
    {
        /// <summary>
        /// HttpClient distinguishes "Content" headers from "Request" headers, so we
        /// need to know which category a header belongs to.
        /// </summary>
        private static readonly HashSet<string> _contentHeaderNames = new HashSet<string> (new[] {
            "Allow",
            "Content-Disposition",
            "Content-Encoding",
            "Content-Language",
            "Content-Length",
            "Content-Location",
            "Content-MD5",
            "Content-Range",
            "Content-Type",
            "Expires",
            "Last-Modified",
        }, StringComparer.OrdinalIgnoreCase);

        /// <summary>Request Url.</summary>
        private readonly Uri _requestUrl;

        /// <summary> The effective HTTP method. </summary>
        private readonly string _effectiveHttpMethod;

        /// <summary>HttpRequestMessage instance.</summary>
        private readonly HttpRequestMessage _requestMessage;
        private readonly HttpClient _client;
        private readonly HttpClientHandler _handler;
        private readonly MemoryStream _messageStream;

        /// <summary>
        /// This will be used to cache content headers to be retrieved later. 
        /// When the message content is being set. 
        /// </summary>
        private readonly Dictionary<string, string> _contentHeaderValueCache;

        /// <summary>HttpResponseMessage instance.</summary>
        private HttpResponseMessage _httpResponseMessage;

        /// <summary>True if SendingRequest2Event is currently invoked, otherwise false.</summary>
        private bool inSendingRequest2Event;
#if DEBUG
        /// <summary>True if the sendingRequest2 event is already fired.</summary>
        private bool sendingRequest2Fired;
#endif

        /// <summary>
        /// Constructor for HttpClientRequestMessage.
        /// Initializes the <see cref="HttpClientRequestMessage"/> class.
        /// The args.ActualMethod is the actual method. In post tunneling situations method will be POST instead of the specified verb method.
        /// The args.method is the specified verb method
        /// </summary>
        /// </summary>
        public HttpClientRequestMessage(DataServiceClientRequestMessageArgs args) 
            : base(args.ActualMethod)
        {
            _messageStream = new MemoryStream();
            _handler = new HttpClientHandler();
            _client = new HttpClient(_handler, disposeHandler: true);
            _contentHeaderValueCache = new Dictionary<string, string>();
            _effectiveHttpMethod = args.Method;
            _requestUrl = args.RequestUri;
            _requestMessage = new HttpRequestMessage(new HttpMethod(this.ActualMethod), _requestUrl);

            // Now set the headers.
            foreach (KeyValuePair<string, string> keyValue in args.Headers)
            {
                this.SetHeader(keyValue.Key, keyValue.Value);
            }
        }

        /// <summary>
        /// Returns the collection of request headers.
        /// </summary>
        public override IEnumerable<KeyValuePair<string, string>> Headers
        {
            get
            {
                if (_requestMessage.Content != null)
                {
                    return HttpHeadersToStringDictionary(_requestMessage.Headers).Concat(HttpHeadersToStringDictionary(_requestMessage.Content.Headers));
                }

                return HttpHeadersToStringDictionary(_requestMessage.Headers).Concat(_contentHeaderValueCache);
            }
        }

        /// <summary>
        /// Gets or sets the request url.
        /// </summary>
        public override Uri Url
        {
            get
            {
                return _requestUrl;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets or sets the method for this request.
        /// </summary>
        public override string Method
        {
            get
            {
                return _effectiveHttpMethod;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        ///  Gets or set the credentials for this request.
        /// </summary>
        public override ICredentials Credentials
        {
            get
            {
                return _handler.Credentials;
            }
            set
            {
                _handler.Credentials = value;
            }
        }


#if !(NETCOREAPP1_0 || NETCOREAPP2_0)

        /// <summary>
        /// Gets or sets the timeout (in seconds) for this request.
        /// </summary>
        public override int Timeout
        {
            get
            {
                return (int)_client.Timeout.TotalSeconds;
            }
            set
            {
               _client.Timeout = new TimeSpan(0, 0, value);
            }
        }

        /// <summary>
        /// <see cref="HttpClientRequestMessage"/> internally uses <see cref="HttpClient"/> class to 
        /// Send HTTP requests and receive responses from a resource identified by the specified URI.
        /// <see cref="HttpClient"/> class does not support read and write timeout.
        /// Currently, this property just sets the<see cref= "Timeout" /> value.
        /// It is retained for backward compatibility and will be dropped in a future major release.
        /// </summary>
        [Obsolete("Use Timeout property instead. Read and write timeout not supported by HttpClient used internally.")]
        public override int ReadWriteTimeout
        {
            get
            {
                return Timeout;
            }
            set
            {
                Timeout = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to send data in segments. 
        /// </summary>
        public override bool SendChunked
        {
            get
            {
                return _requestMessage.Headers.TransferEncodingChunked ?? false;
            }
            set
            {
                _requestMessage.Headers.TransferEncodingChunked = value;
            }
        }
#endif

        /// <summary>
        /// Returns the value of the header with the given name.
        /// </summary>
        /// <param name="headerName">Name of the header.</param>
        /// <returns>Returns the value of the header with the given name.</returns>
        public override string GetHeader(string headerName)
        {
            if (_contentHeaderNames.Contains(headerName))
            {
                // If this is a "Content" header then we retrieve the value either
                // from the message content (if available) or the cache (otherwise)
                if (_requestMessage.Content != null)
                {
                    return string.Join(",", _requestMessage.Content.Headers.GetValues(headerName));
                }

                string headerValue;
                if (string.Equals(headerName, XmlConstants.HttpContentType, StringComparison.OrdinalIgnoreCase))
                {
                    return _contentHeaderValueCache.TryGetValue(XmlConstants.HttpContentType, out headerValue) ? headerValue : string.Empty;
                }
                else if (string.Equals(headerName, XmlConstants.HttpContentLength, StringComparison.OrdinalIgnoreCase))
                {
                    return _contentHeaderValueCache.TryGetValue(XmlConstants.HttpContentLength, out headerValue) ? headerValue : string.Empty;
                }
                else if(string.Equals(headerName, XmlConstants.HttpContentDisposition, StringComparison.OrdinalIgnoreCase))
                {
                    return _contentHeaderValueCache.TryGetValue(XmlConstants.HttpContentDisposition, out headerValue) ? headerValue : string.Empty;
                }
                else
                {
                    return _contentHeaderValueCache.TryGetValue(headerName, out headerValue) ? headerValue : string.Empty;
                }
            }

            return _requestMessage.Headers.Contains(headerName) ? string.Join(",", _requestMessage.Headers.GetValues(headerName)) : string.Empty;
        }

        /// <summary>
        /// Sets the value of the header with the given name.
        /// </summary>
        /// <param name="headerName">Name of the header.</param>
        /// <param name="headerValue">Value of the header.</param>
        public override void SetHeader(string headerName, string headerValue)
        {
            if (_contentHeaderNames.Contains(headerName))
            {
                // If this is a "Content" header then we cache the value (due
                // to the message content not being set yet) and set it
                // upon sending the message (when the content will be available)
                SetContentHeader(headerName, headerValue);
                return;
            }

            SetRequestHeader(headerName, headerValue);
        }

        /// <summary>
        /// Sets content headers.
        /// </summary>
        /// <param name="headerName"></param>
        /// <param name="headerValue"></param>
        private void SetContentHeader(string headerName, string headerValue)
        {
            if (string.Equals(headerName, XmlConstants.HttpContentType, StringComparison.OrdinalIgnoreCase))
            {
                _contentHeaderValueCache[XmlConstants.HttpContentType] = headerValue;
            }
            else if (string.Equals(headerName, XmlConstants.HttpContentLength, StringComparison.OrdinalIgnoreCase))
            {
                _contentHeaderValueCache[XmlConstants.HttpContentLength] = headerValue.ToString(CultureInfo.InvariantCulture.NumberFormat);
            }
            else if (string.Equals(headerName, XmlConstants.HttpContentDisposition, StringComparison.OrdinalIgnoreCase))
            {
                _contentHeaderValueCache[XmlConstants.HttpContentDisposition] = headerValue.ToString(CultureInfo.InvariantCulture.NumberFormat);
            }
            else
            {
                _contentHeaderValueCache[headerName] = headerValue;
            }            
        }

        /// <summary>
        /// Sets request headers
        /// </summary>
        /// <param name="headerName"></param>
        /// <param name="headerValue"></param>
        private void SetRequestHeader(string headerName, string headerValue)
        {
            if (string.Equals(headerName, XmlConstants.HttpRequestAccept, StringComparison.OrdinalIgnoreCase))
            {
                _requestMessage.Headers.Remove(headerName);
                _requestMessage.Headers.Add(XmlConstants.HttpAccept, headerValue);
            }
            else if (string.Equals(headerName, XmlConstants.HttpUserAgent, StringComparison.OrdinalIgnoreCase))
            {
                _requestMessage.Headers.Remove(headerName);
                _requestMessage.Headers.Add(XmlConstants.HttpUserAgent, headerValue);
            }
            else if (string.Equals(headerName, XmlConstants.HttpAcceptCharset, StringComparison.OrdinalIgnoreCase))
            {
                _requestMessage.Headers.Remove(headerName);
                _requestMessage.Headers.Add(XmlConstants.HttpAcceptCharset, headerValue);
            }
            else
            {
                _requestMessage.Headers.Remove(headerName);
                _requestMessage.Headers.Add(headerName, headerValue);
            }
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
            return _messageStream;
        }

        /// <summary>
        /// Abort the current request.
        /// </summary>
        public override void Abort()
        {
            _client.CancelPendingRequests();
        }

        /// <summary>
        /// Begins an asynchronous request for a System.IO.Stream object to use to write data.
        /// </summary>
        /// <param name="callback">The System.AsyncCallback delegate.</param>
        /// <param name="state">The state object for this request.</param>
        /// <returns>An System.IAsyncResult that references the asynchronous request.</returns>
        public override IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
        {
            if (this.inSendingRequest2Event)
            {
                throw new NotSupportedException(Strings.ODataRequestMessage_GetStreamMethodNotSupported);
            }
            TaskCompletionSource<Stream> taskCompletionSource = new TaskCompletionSource<Stream>();
            taskCompletionSource.TrySetResult(_messageStream);
            return taskCompletionSource.Task.ToApm(callback, state);
        }

        /// <summary>
        /// Ends an asynchronous request for a System.IO.Stream object to use to write data.
        /// </summary>
        /// <param name="asyncResult">The pending request for a stream.</param>
        /// <returns>A System.IO.Stream to use to write request data.</returns>
        public override Stream EndGetRequestStream(IAsyncResult asyncResult)
        {
            return ((Task<Stream>)asyncResult).Result;
        }

        /// <summary>
        ///  Begins an asynchronous request to an Internet resource.
        /// </summary>
        /// <param name="callback">The System.AsyncCallback delegate.</param>
        /// <param name="state">The state object for this request.</param>
        /// <returns>An System.IAsyncResult that references the asynchronous request for a response.</returns>
        public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
        {
            Task<HttpResponseMessage> send = CreateSendTask();
            return send.ToApm(callback, state);
        }

        /// <summary>
        /// Ends an asynchronous request to an Internet resource.
        /// </summary>
        /// <param name="asyncResult">The pending request for a response.</param>
        /// <returns>A System.Net.HttpResponseMessage that contains the response from the Internet resource.</returns>
        public override IODataResponseMessage EndGetResponse(IAsyncResult asyncResult)
        {
            return UnwrapAggregateException(() =>
            {
                HttpResponseMessage result = ((Task<HttpResponseMessage>)asyncResult).Result;
                return ConvertHttpWebResponse(result);
            });
        }

        /// <summary>
        /// Returns a response from an Internet resource.
        /// </summary>
        /// <returns>A System.Net.HttpResponseMessage that contains the response from the Internet resource.</returns>
        public override IODataResponseMessage GetResponse()
        {
            return UnwrapAggregateException(() =>
            {
                Task<HttpResponseMessage> send = CreateSendTask();
                send.Wait();
                _httpResponseMessage = send.Result;

                return ConvertHttpWebResponse(send.Result);
            });
        }

        private Task<HttpResponseMessage> CreateSendTask()
        {
            // Only set the message content if the stream has been written to, otherwise
            // HttpClient will complain if it's a GET request.
            Byte[] messageContent = _messageStream.ToArray();
            if (messageContent.Length > 0)
            {
                _requestMessage.Content = new ByteArrayContent(messageContent);

                _messageStream.Dispose();

                // Apply cached "Content" header values
                foreach (KeyValuePair<string, string> contentHeader in _contentHeaderValueCache)
                {
                    _requestMessage.Content.Headers.Add(contentHeader.Key, contentHeader.Value);
                }
            }

            _requestMessage.Method = new HttpMethod(_effectiveHttpMethod);

            return _client.SendAsync(_requestMessage);
        }

        private static IDictionary<string, string> HttpHeadersToStringDictionary(HttpHeaders headers)
        {
            return headers.ToDictionary((h1) => h1.Key, (h2) => string.Join(",", h2.Value));
        }

        /// <summary> Returns the underlying HttpRequestMessage </summary>
        internal HttpRequestMessage HttpRequestMessage
        {
            get
            {
                return _requestMessage;
            }
        }

        private static HttpWebResponseMessage ConvertHttpWebResponse(HttpResponseMessage response)
        {
            IEnumerable<KeyValuePair<string, string>> allHeaders = HttpHeadersToStringDictionary(response.Headers).Concat(HttpHeadersToStringDictionary(response.Content.Headers));
            return new HttpWebResponseMessage(
                allHeaders.ToDictionary((h1) => h1.Key, (h2) => h2.Value),
                (int)response.StatusCode,
                () =>
                {
                    Task<Stream> task = response.Content.ReadAsStreamAsync();
                    task.Wait();
                    return task.Result;
                });
        }

        private static T UnwrapAggregateException<T>(Func<T> action)
        {
            try
            {
                return action();
            }
            catch (AggregateException aggregateException)
            {
                if (aggregateException.InnerExceptions.Count == 1)
                {
                    throw ConvertToDataServiceTransportException(new WebException(aggregateException.InnerException.ToString()));
                }
                throw;
            }
        }

        private static DataServiceTransportException ConvertToDataServiceTransportException(WebException webException)
        {
            HttpWebResponseMessage errorResponseMessage = null;
            if (webException.Response != null)
            {
                HttpWebResponse httpResponse = (HttpWebResponse)webException.Response;
                errorResponseMessage = new HttpWebResponseMessage(httpResponse);
            }

            return new DataServiceTransportException(errorResponseMessage, webException);
        }

        /// <summary>
        /// This method is called just before firing SendingRequest2 event.
        /// </summary>
        internal void BeforeSendingRequest2Event()
        {
            this.inSendingRequest2Event = true;
        }

        /// <summary>
        /// This method is called immediately after SendingRequest2 event has been fired.
        /// </summary>
        internal void AfterSendingRequest2Event()
        {
            this.inSendingRequest2Event = false;
#if DEBUG
            this.sendingRequest2Fired = true;
#endif
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Perform the actual cleanup work.
        /// </summary>
        /// <param name="disposing">If 'true' this method is called from user code; if 'false' it is called by the runtime.</param>
        protected virtual void Dispose(bool disposing)
        {
            HttpResponseMessage response = _httpResponseMessage;
            _httpResponseMessage = null;
            if (response != null)
            {
                ((IDisposable)response).Dispose();
            }
        }

        void ISendingRequest2.BeforeSendingRequest2Event()
        {
            this.inSendingRequest2Event = true;
        }

        void ISendingRequest2.AfterSendingRequest2Event()
        {
            this.inSendingRequest2Event = false;
#if DEBUG
            this.sendingRequest2Fired = true;
#endif
        }
    }
}

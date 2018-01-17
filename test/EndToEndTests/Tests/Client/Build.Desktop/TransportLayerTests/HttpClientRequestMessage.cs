//---------------------------------------------------------------------
// <copyright file="HttpClientRequestMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.TransportLayerTests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.OData;

    /// <summary>
    /// HttpClient based implementation of DataServiceClientRequestMessage.
    /// </summary>
    public class HttpClientRequestMessage : DataServiceClientRequestMessage
    {
        /// <summary>
        /// HttpClient distinguishes "Content" headers from "Request" headers, so we
        /// need to know which category a header belongs to.
        /// </summary>
        private static readonly IEnumerable<string> contentHeaderNames = new[] {
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
        };
 
        private readonly HttpRequestMessage requestMessage;
        private readonly HttpClient client;
        private readonly HttpClientHandler handler;
        private readonly MemoryStream messageStream;
        private readonly Dictionary<string, string> contentHeaderValueCache;

        /// <summary>
        /// Constructor for HttpClientRequestMessage.
        /// </summary>
        public HttpClientRequestMessage(string actualMethod) : base(actualMethod)
        {
            this.requestMessage = new HttpRequestMessage();
            this.messageStream = new MemoryStream();
            this.handler = new HttpClientHandler();
            this.client = new HttpClient(this.handler, disposeHandler: true);
            this.contentHeaderValueCache = new Dictionary<string, string>(); 
        }

        /// <summary>
        /// Returns the collection of request headers.
        /// </summary>
        public override IEnumerable<KeyValuePair<string, string>> Headers
        {
            get
            {
                if (this.requestMessage.Content != null)
                {
                    return HttpHeadersToStringDictionary(this.requestMessage.Headers).Concat(HttpHeadersToStringDictionary(this.requestMessage.Content.Headers));
                }

                return HttpHeadersToStringDictionary(this.requestMessage.Headers).Concat(this.contentHeaderValueCache);
            }
        }

        /// <summary>
        /// Gets or sets the request url.
        /// </summary>
        public override Uri Url
        {
            get
            {
                return requestMessage.RequestUri;
            }
            set
            {
                requestMessage.RequestUri = value;
            }
        }

        /// <summary>
        /// Gets or sets the method for this request.
        /// </summary>
        public override string Method
        {
            get
            {
                return this.requestMessage.Method.ToString();
            }
            set
            {
                this.requestMessage.Method = new HttpMethod(value);
            }
        }

        /// <summary>
        ///  Gets or set the credentials for this request.
        /// </summary>
        public override ICredentials Credentials
        {
            get
            {
                return this.handler.Credentials;
            }
            set
            {
                this.handler.Credentials = value;
            }
        }

        /// <summary>
        /// Gets or sets the timeout (in seconds) for this request.
        /// </summary>
#if !PORTABLELIB && !(NETCOREAPP1_0 || NETCOREAPP2_0)
        public override int Timeout 
        { 
            get
            {
                return (int)this.client.Timeout.TotalSeconds;
            }
            set
            {
                this.client.Timeout = new TimeSpan(0, 0, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to send data in segments to the Internet resource. 
        /// </summary>
        public override bool SendChunked
        {
            get
            {
                bool? transferEncodingChunked = this.requestMessage.Headers.TransferEncodingChunked;
                return transferEncodingChunked.HasValue && transferEncodingChunked.Value;
            }
            set
            {
                this.requestMessage.Headers.TransferEncodingChunked = value;
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
            if (contentHeaderNames.Contains(headerName))
            {
                // If this is a "Content" header then we retrieve the value either
                // from the message content (if available) or the cache (otherwise)
                if (this.requestMessage.Content != null)
                {
                    return string.Join(",", this.requestMessage.Content.Headers.GetValues(headerName));
                }

                string headerValue;
                return this.contentHeaderValueCache.TryGetValue(headerName, out headerValue) ? headerValue : string.Empty;
            }
            return this.requestMessage.Headers.Contains(headerName) ? string.Join(",", this.requestMessage.Headers.GetValues(headerName)) : string.Empty;
        }

        /// <summary>
        /// Sets the value of the header with the given name.
        /// </summary>
        /// <param name="headerName">Name of the header.</param>
        /// <param name="headerValue">Value of the header.</param>
        public override void SetHeader(string headerName, string headerValue)
        {
            if (contentHeaderNames.Contains(headerName))
            {
                // If this is a "Content" header then we cache the value (due
                // to the message content not being set yet) and set it
                // upon sending the message (when the content will be available)
                this.contentHeaderValueCache[headerName] = headerValue;
                return;
            }

            this.requestMessage.Headers.Remove(headerName);
            this.requestMessage.Headers.Add(headerName, headerValue);
        }

        /// <summary>
        /// Gets the stream to be used to write the request payload.
        /// </summary>
        /// <returns>Stream to which the request payload needs to be written.</returns>
        public override Stream GetStream()
        {
            return this.messageStream;
        }

        /// <summary>
        /// Abort the current request.
        /// </summary>
        public override void Abort()
        {
            this.client.CancelPendingRequests();
        }

        /// <summary>
        /// Begins an asynchronous request for a System.IO.Stream object to use to write data.
        /// </summary>
        /// <param name="callback">The System.AsyncCallback delegate.</param>
        /// <param name="state">The state object for this request.</param>
        /// <returns>An System.IAsyncResult that references the asynchronous request.</returns>
        public override IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
        {
            var taskCompletionSource = new TaskCompletionSource<Stream>();
            taskCompletionSource.TrySetResult(this.messageStream);
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

        public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
        {
            var send = CreateSendTask();
            return send.ToApm(callback, state);
        }

        public override IODataResponseMessage EndGetResponse(IAsyncResult asyncResult)
        {
            return UnwrapAggregateException(() =>
            {
                var result = ((Task<HttpResponseMessage>)asyncResult).Result;
                return ConvertHttpClientResponse(result);
            });
        }

#if !PORTABLELIB && !(NETCOREAPP1_0 || NETCOREAPP2_0)
        public override IODataResponseMessage GetResponse()
        {
            return UnwrapAggregateException(() =>
                {
                    var send = CreateSendTask();
                    send.Wait();
                    return ConvertHttpClientResponse(send.Result);
                });
        }
#endif

        private Task<HttpResponseMessage> CreateSendTask()
        {
            // Only set the message content if the stream has been written to, otherwise
            // HttpClient will complain if it's a GET request.
            var messageContent = this.messageStream.ToArray();
            if (messageContent.Length > 0)
            {
                this.requestMessage.Content = new ByteArrayContent(messageContent);

                // Apply cached "Content" header values
                foreach (var contentHeader in this.contentHeaderValueCache)
                {
                    this.requestMessage.Content.Headers.Add(contentHeader.Key, contentHeader.Value);
                }
            }

            this.requestMessage.Method = new HttpMethod(this.ActualMethod);

            var send = this.client.SendAsync(this.requestMessage);
            return send;
        }

        private static IDictionary<string, string> HttpHeadersToStringDictionary(HttpHeaders headers)
        {
            return headers.ToDictionary((h1) => h1.Key, (h2) => string.Join(",", h2.Value));
        }

        private static HttpWebResponseMessage ConvertHttpClientResponse(HttpResponseMessage response)
        {
            var allHeaders = HttpHeadersToStringDictionary(response.Headers).Concat(HttpHeadersToStringDictionary(response.Content.Headers));
            return new HttpWebResponseMessage(
                allHeaders.ToDictionary((h1) => h1.Key, (h2) => h2.Value),
                (int)response.StatusCode,
                () =>
                {
                    var task = response.Content.ReadAsStreamAsync();
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
                    throw ConvertToDataServiceWebException(aggregateException.InnerExceptions.Single() as WebException);
                }

                throw;
            }
        }

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
    }
}

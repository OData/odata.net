---
layout: post
title: "Use HttpClient in OData Client"
description: ""
category: "4. Client"
---

In this session, we will dive into how to use HttpClient in OData client request. We will use the hook mechanism in OData client which has been introduced in [Client Hooks in OData Client](http://odata.github.io/odata.net/#04-06-use-client-hooks-in-odata-client).   

OData client enables developers to customize request message, and use it in `DataServiceContext.Configurations.RequestPipeline.OnMessageCreating`. This function will be triggered when creating request message. It will return an `IODataRequestMessage`.

Following is the code how to use `OnMessageCreating`.
 
{% highlight csharp %}

	public void UseHttpClientTest()
    {
        DefaultContainer dsc = new DefaultContainer(new Uri("http://services.odata.org/V4/(S(uvf1y321yx031rnxmcbqmlxw))/TripPinServiceRW/"));

        dsc.Configurations.RequestPipeline.OnMessageCreating = (args) =>
            {
                var message = new HttpClientRequestMessage(args.ActualMethod) { Url = args.RequestUri, Method = args.Method, };
                foreach (var header in args.Headers)
                {
                    message.SetHeader(header.Key, header.Value);
                }

                return message;
            };
        var people = dsc.People.ToList();
        foreach (var p in people)
        {
            Console.WriteLine(p.FirstName);
        }
    }

{% endhighlight %}

In this sample, we create a `HttpClientRequestMessage` instance in `OnMessageCreating` method. `HttpClientRequestMessage` is a class derived from `DataServiceClientRequestMessage`. In this class, we use MemoryStream to write data, and use HttpClient to get response. Once we get the HttpResponseMessage, we will convert it to `IODataResponseMessage`. So we also write a `HttpClientResponseMessage` class which implements `IODataResponseMessage`.
 
{% highlight csharp %}
	
	public class HttpClientRequestMessage : DataServiceClientRequestMessage
	{
		private readonly HttpRequestMessage requestMessage;
        private readonly HttpClient client;
        private readonly HttpClientHandler handler;
        private readonly MemoryStream messageStream;
        private readonly Dictionary<string, string> contentHeaderValueCache;

        public HttpClientRequestMessage(string actualMethod)
            : base(actualMethod)
        {
            this.requestMessage = new HttpRequestMessage();
            this.messageStream = new MemoryStream();
            this.handler = new HttpClientHandler();
            this.client = new HttpClient(this.handler, disposeHandler: true);
            this.contentHeaderValueCache = new Dictionary<string, string>();
        }

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

        public override Uri Url
        {
            get { return requestMessage.RequestUri;　}
            set { requestMessage.RequestUri = value; }
        }

        public override string Method
        {
            get { return this.requestMessage.Method.ToString();　}
            set { this.requestMessage.Method = new HttpMethod(value); }
        }

        public override ICredentials Credentials
        {
            get　{　return this.handler.Credentials;　}
            set { this.handler.Credentials = value; }
        }

        public override int Timeout
        {
            get { return (int)this.client.Timeout.TotalSeconds;　}
            set {　this.client.Timeout = new TimeSpan(0, 0, value);　}
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
            set　{　this.requestMessage.Headers.TransferEncodingChunked = value;　}
        }

        public override string GetHeader(string headerName)
        {
            //Returns the value of the header with the given name.
        }

        public override void SetHeader(string headerName, string headerValue)
        {
            // Sets the value of the header with the given name
        }

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

        public override IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
        {
            var taskCompletionSource = new TaskCompletionSource<Stream>();
            taskCompletionSource.TrySetResult(this.messageStream);
            return taskCompletionSource.Task.ToApm(callback, state);
        }

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
            var result = ((Task<HttpResponseMessage>)asyncResult).Result;
            return ConvertHttpClientResponse(result);
        }

        public override IODataResponseMessage GetResponse()
        {
            var send = CreateSendTask();
            send.Wait();
            return ConvertHttpClientResponse(send.Result);
        }

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

        private static HttpClientResponseMessage ConvertHttpClientResponse(HttpResponseMessage response)
        {
            return new HttpClientResponseMessage(response);
        }
    }

    public static class TaskExtensionMethods
    {
        public static Task<TResult> ToApm<TResult>(this Task<TResult> task, AsyncCallback callback, object state)
        {
            var tcs = new TaskCompletionSource<TResult>(state);

            task.ContinueWith(
                delegate
                {
                    if (task.IsFaulted)
                    {
                        tcs.TrySetException(task.Exception.InnerExceptions);
                    }
                    else if (task.IsCanceled)
                    {
                        tcs.TrySetCanceled();
                    }
                    else
                    {
                        tcs.TrySetResult(task.Result);
                    }

                    if (callback != null)
                    {
                        callback(tcs.Task);
                    }
                },
                CancellationToken.None,
                TaskContinuationOptions.None,
                TaskScheduler.Default);

            return tcs.Task;
        }
    }

{% endhighlight %}

{% highlight csharp %}

    public class HttpClientResponseMessage : IODataResponseMessage, IDisposable
    {
        private readonly IDictionary<string, string> headers;
        private readonly Func<Stream> getResponseStream;
        private readonly int statusCode;

    #if DEBUG
        /// <summary>set to true once the GetStream was called.</summary>
        private bool streamReturned;
    #endif

        public HttpClientResponseMessage(HttpResponseMessage httpResponse)
        {
            this.headers = HttpHeadersToStringDictionary(httpResponse.Headers);
            this.statusCode = (int)httpResponse.StatusCode;
            this.getResponseStream = () => { var task = httpResponse.Content.ReadAsStreamAsync(); task.Wait();  return task.Result; };
        }

        private static IDictionary<string, string> HttpHeadersToStringDictionary(HttpHeaders headers)
        {
            return headers.ToDictionary((h1) => h1.Key, (h2) => string.Join(",", h2.Value));
        }

        /// <summary>
        /// Returns the collection of response headers.
        /// </summary>
        public virtual IEnumerable<KeyValuePair<string, string>> Headers
        {
            get { return this.headers; }
        }

        /// <summary>
        /// The response status code.
        /// </summary>
        public virtual int StatusCode
        {
            get { return this.statusCode; }

            set {　throw new NotSupportedException();　}
        }

        public virtual string GetHeader(string headerName)
        {
            string result;
            if (this.headers.TryGetValue(headerName, out result))
            {
                return result;
            }

            // Since the unintialized value of ContentLength header is -1, we need to return
            // -1 if the content length header is not present
            if (string.Equals(headerName, "Content-Length", StringComparison.Ordinal))
            {
                return "-1";
            }

            return null;
        }

        public virtual void SetHeader(string headerName, string headerValue)
        {
            if(String.IsNullOrEmpty(headerValue))
            {
                return;
            }
            if (this.headers.ContainsKey(headerName))
            {
                this.headers[headerName] = headerValue;
            }
            else
            {
                this.headers.Add(headerName, headerValue);
            }
        }

        public virtual Stream GetStream()
        {
    #if DEBUG
            Debug.Assert(!this.streamReturned, "The GetStream can only be called once.");
            this.streamReturned = true;
    #endif

            return this.getResponseStream();
        }
    }

{% endhighlight %}
//---------------------------------------------------------------------
// <copyright file="IDataServiceHostRunner.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

//using System;
//using System.Web;
//using System.Linq;
//using System.Net;
//using Microsoft.OData.Service;
//using System.Reflection;
//using System.Data.Test.Astoria.Providers;
//using System.Collections.Generic;
//using System.Collections.Specialized;
namespace TestServiceHost
{
    class Program
    {
        #region HttpListenerHost
        private class HttpListenerHost : IDataServiceHost2
        {
            private HttpListenerContext httpListenerContext;

            // name of HTTP headers
            internal const string HttpAccept = "Accept";
            internal const string HttpAcceptEncoding = "Accept-Encoding";
            internal const string HttpAcceptCharset = "Accept-Charset";
            internal const string HttpContentLength = "Content-Length";
            internal const string HttpContentType = "Content-Type";
            internal const string HttpIfMatch = "If-Match";
            internal const string HttpIfNoneMatch = "If-None-Match";
            internal const string HttpCacheControl = "Cache-Control";
            internal const string HttpLocation = "Location";
            internal const string HttpETag = "ETag";
            internal const string HttpDataServiceVersion = "OData-Version";
            internal const string HttpMaxDataServiceVersion = "OData-MaxVersion";
            internal const string HttpXMethod = "X-HTTP-Method";

            public HttpListenerHost(HttpListenerContext context, string serviceUri)
            {
                this.httpListenerContext = context;

                // retrieve request information from HttpListenerContext
                HttpListenerRequest contextRequest = context.Request;
                this.AbsoluteRequestUri = new Uri(contextRequest.Url.AbsoluteUri);
                this.AbsoluteServiceUri = new Uri(serviceUri);
                this.RequestPathInfo = this.AbsoluteRequestUri.MakeRelativeUri(this.AbsoluteServiceUri).ToString();

                // retrieve request headers
                this.RequestAccept = contextRequest.Headers[HttpAccept];
                this.RequestAcceptCharSet = contextRequest.Headers[HttpAcceptCharset];
                this.RequestIfMatch = contextRequest.Headers[HttpIfMatch];
                this.RequestIfNoneMatch = contextRequest.Headers[HttpIfNoneMatch];
                this.RequestMaxVersion = contextRequest.Headers[HttpMaxDataServiceVersion];
                this.RequestVersion = contextRequest.Headers[HttpDataServiceVersion];
                this.RequestContentType = contextRequest.ContentType;

                this.RequestHeaders = new WebHeaderCollection();
                foreach (string header in contextRequest.Headers.AllKeys)
                    this.RequestHeaders.Add(header, contextRequest.Headers.Get(header));

                this.QueryStringValues = new Dictionary<string, string>();
                string queryString = this.AbsoluteRequestUri.Query;
                var parsedValues = HttpUtility.ParseQueryString(queryString);
                foreach (string option in parsedValues.AllKeys)
                    this.QueryStringValues.Add(option, parsedValues.Get(option));

                processExceptionCalled = false;
            }

            #region IDataServiceHost Members

            /// <summary>Gets the absolute resource upon which to apply the request.</summary>
            public Uri AbsoluteRequestUri
            {
                get;
                set;
            }

            /// <summary>Gets the absolute URI to the service.</summary>
            public Uri AbsoluteServiceUri
            {
                get;
                set;
            }

            public Dictionary<string, string> QueryStringValues
            {
                get;
                private set;
            }

            public string RequestPathInfo
            {
                get;
                private set;
            }

            /// <summary>Gets the value for the specified item in the request query string.</summary>
            /// <param name="item">Item to return.</param>
            /// <returns>
            /// The value for the specified item in the request query string;
            /// null if <paramref name="item"/> is not found.
            /// </returns>
            public string GetQueryStringItem(string item)
            {
                string result;
                QueryStringValues.TryGetValue(item, out result);
                return result;
            }

            /// <summary>Method to handle a data service exception during processing.</summary>
            /// <param name="args">Exception handling description.</param>
            public void ProcessException(HandleExceptionArgs args)
            {
                processExceptionCalled = true;

                if (!args.ResponseWritten)
                {
                    this.ResponseStatusCode = args.ResponseStatusCode;
                    this.ResponseContentType = args.ResponseContentType;
                }
            }

            /// <summary>
            /// Gets a comma-separated list of client-supported MIME Accept types.
            /// </summary>
            public string RequestAccept
            {
                get { return this.httpListenerContext.Request.Headers[HttpListenerHost.HttpAccept]; }
                set { this.httpListenerContext.Request.Headers[HttpListenerHost.HttpAccept] = value; }
            }

            /// <summary>
            /// Gets the string with the specification for the character set 
            /// encoding that the client requested, possibly null.
            /// </summary>
            public string RequestAcceptCharSet
            {
                get { return this.httpListenerContext.Request.Headers[HttpListenerHost.HttpAcceptCharset]; }
                set { this.httpListenerContext.Request.Headers[HttpListenerHost.HttpAcceptCharset] = value; }
            }

            /// <summary>Gets the HTTP MIME type of the request stream.</summary>
            public string RequestContentType
            {
                get { return this.httpListenerContext.Request.Headers[HttpListenerHost.HttpContentType]; }
                set { this.httpListenerContext.Request.Headers[HttpListenerHost.HttpContentType] = value; }
            }

            /// <summary>
            /// Gets the HTTP data transfer method (such as GET, POST, or HEAD) used by the client.
            /// </summary>
            public string RequestHttpMethod
            {
                get
                {
                    string method = this.RequestHeaders.Get(HttpListenerHost.HttpXMethod);
                    if (string.IsNullOrEmpty(method))
                        method = this.httpListenerContext.Request.HttpMethod;
                    return method;
                }
            }

            /// <summary>Gets the value of the If-Match header from the request made</summary>
            public string RequestIfMatch
            {
                get { return this.httpListenerContext.Request.Headers[HttpListenerHost.HttpIfMatch]; }
                set { this.httpListenerContext.Request.Headers[HttpListenerHost.HttpIfMatch] = value; }
            }

            /// <summary>Gets the value of the If-None-Match header from the request made</summary>
            public string RequestIfNoneMatch
            {
                get { return this.httpListenerContext.Request.Headers[HttpListenerHost.HttpIfNoneMatch]; }
                set { this.httpListenerContext.Request.Headers[HttpListenerHost.HttpIfNoneMatch] = value; }
            }

            /// <summary>Gets the value for the OData-MaxVersion request header.</summary>
            public string RequestMaxVersion
            {
                get { return this.httpListenerContext.Request.Headers[HttpListenerHost.HttpMaxDataServiceVersion]; }
                set { this.httpListenerContext.Request.Headers[HttpListenerHost.HttpMaxDataServiceVersion] = value; }
            }

            /// <summary>
            /// Gets the <see cref="Stream"/> from which the input must be read
            /// to the client.
            /// </summary>
            public Stream RequestStream
            {
                get { return this.httpListenerContext.Request.InputStream; }
            }

            /// <summary>Gets the value for the OData-Version request header.</summary>
            public string RequestVersion
            {
                get { return this.httpListenerContext.Request.Headers[HttpListenerHost.HttpDataServiceVersion]; }
                set { this.httpListenerContext.Request.Headers[HttpListenerHost.HttpDataServiceVersion] = value; }
            }

            /// <summary>Gets or sets the Cache-Control header on the response.</summary>
            public string ResponseCacheControl
            {
                get { return this.httpListenerContext.Response.Headers[HttpListenerHost.HttpCacheControl]; }
                set { this.httpListenerContext.Response.Headers[HttpListenerHost.HttpCacheControl] = value; }
            }

            /// <summary>Gets or sets the HTTP MIME type of the output stream.</summary>
            public string ResponseContentType
            {
                get { return this.httpListenerContext.Response.ContentType; }
                set { this.httpListenerContext.Response.ContentType = value; }
            }

            /// <summary>Gets/Sets the value of the ETag header on the response</summary>
            public string ResponseETag
            {
                get { return this.httpListenerContext.Response.Headers[HttpListenerHost.HttpETag]; }
                set { this.httpListenerContext.Response.Headers[HttpListenerHost.HttpETag] = value; }
            }

            /// <summary>Gets or sets the Location header on the response.</summary>
            public string ResponseLocation
            {
                get { return this.httpListenerContext.Response.Headers[HttpListenerHost.HttpLocation]; }
                set { this.httpListenerContext.Response.Headers[HttpListenerHost.HttpLocation] = value; }
            }

            /// <summary>
            /// Returns the status code for the request made
            /// </summary>
            public int ResponseStatusCode
            {
                get { return this.httpListenerContext.Response.StatusCode; }
                set
                {
                    if (MustNotReturnMessageBody((HttpStatusCode)value))
                    {
                        // in order to suppress the message body, we turn of chunked and specify an empty content
                        // note that at this layer, there is no direct equivalent of WCF's suppression flag used
                        // in HttpContextServiceHost
                        this.httpListenerContext.Response.ContentLength64 = 0;
                        this.httpListenerContext.Response.SendChunked = false;
                    }
                    else
                    {
                        // conceptually, this is simply an 'undo' of the previous block
                        // note that we can't set the content length back to 'unknown' but
                        // setting this back to the default works fine
                        this.httpListenerContext.Response.SendChunked = true;
                    }
                    this.httpListenerContext.Response.StatusCode = value;
                }
            }

            // taken from HttpContextServiceHost
            private static bool MustNotReturnMessageBody(HttpStatusCode statusCode)
            {
                // Both 204 and 304 must not include a message-body in the response.
                switch (statusCode)
                {
                    case HttpStatusCode.NoContent:    // 204
                    case HttpStatusCode.ResetContent: // 205
                    case HttpStatusCode.NotModified:  // 304
                        return true;

                    default:
                        return false;
                }
            }

            /// <summary>
            /// Gets the <see cref="Stream"/> to be written to send a response
            /// to the client.
            /// </summary>
            public Stream ResponseStream
            {
                get { return this.httpListenerContext.Response.OutputStream; }
            }

            /// <summary>Gets or sets the value for the OData-Version response header.</summary>
            public string ResponseVersion
            {
                get { return this.httpListenerContext.Response.Headers[HttpListenerHost.HttpDataServiceVersion]; }
                set { this.httpListenerContext.Response.Headers[HttpListenerHost.HttpDataServiceVersion] = value; }
            }

            #endregion

            #region IDataServiceHost2 Members

            /// <summary>Dictionary of all request headers from the host.</summary>
            public WebHeaderCollection RequestHeaders
            {
                get;
                private set;
            }

            /// <summary>Enumerates all response headers that has been set.</summary>
            public WebHeaderCollection ResponseHeaders
            {
                get { return this.httpListenerContext.Response.Headers; }
            }

            #endregion

            /// <summary>Flag to indicate if method ProcessException is called.</summary>
            public bool processExceptionCalled
            {
                get;
                private set;
            }
        }
        #endregion


        private static void ListenerCallback(IAsyncResult result)
        {
            HttpListener listener = (HttpListener)result.AsyncState;
            
            //process another request
            listener.BeginGetContext(new AsyncCallback(ListenerCallback), listener);

            try
            {
                HttpListenerContext context = listener.EndGetContext(result);
                HttpListenerHost host = new HttpListenerHost(context, uriBaseAddress);

                object hostWrapper;
                if (useV2Host)
                    hostWrapper = new IDSH2Wrapper(host);
                else
                    hostWrapper = new IDSHWrapper(host);

                object service = Activator.CreateInstance(serviceType);

                MethodInfo attachMethod = serviceType.GetMethod("AttachHost", new Type[] { typeof(IDataServiceHost) });
                attachMethod.Invoke(service, new object[] { hostWrapper });
                MethodInfo processRequestMethod = serviceType.GetMethod("ProcessRequest", BindingFlags.Public | BindingFlags.Instance);
                try
                {
                    processRequestMethod.Invoke(service, new object[0]);
                }
                catch (Exception e)
                {
                    if (!host.processExceptionCalled)
                        throw e;
                }

                if (host.processExceptionCalled)
                {
                    if (context.Request.HasEntityBody)
                    {
                        context.Request.InputStream.Close();
                        context.Request.InputStream.Dispose();
                    }
                }

                context.Response.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("An exception occurred:");
                Console.WriteLine(e.ToString());
                listener.Abort();
                Console.ReadLine();
            }
        }

        private static string uriBaseAddress;
        private static bool useV2Host;
        private static Type serviceType;

        static void Main(string[] args)
        {
            uriBaseAddress = args[0];
            if (!uriBaseAddress.EndsWith("/"))
                uriBaseAddress += "/";

            useV2Host = false;
            if (args.Length > 1)
                useV2Host = bool.Parse(args[1]);

            string waitHandleName = null;
            if (args.Length > 2)
                waitHandleName = args[2];

            serviceType = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Single(t => !t.IsAbstract 
                    && typeof(Microsoft.OData.Service.IRequestHandler).IsAssignableFrom(t));

            HttpListener listener = new HttpListener();
            try
            {
                listener.Prefixes.Add(uriBaseAddress);
                listener.Start();
                listener.BeginGetContext(new AsyncCallback(ListenerCallback), listener);
            }
            catch (Exception e)
            {
                Console.WriteLine("An exception occurred:");
                Console.WriteLine(e.ToString());
                listener.Abort();
                Console.ReadLine();
            }

            // both of the following cases will block, so there is no need for a loop
            if (waitHandleName == "Debug" || string.IsNullOrEmpty(waitHandleName))
            {
                Console.WriteLine("Running in Debug mode, please press any key to exit");
                // blocks until key pressed
                Console.Read();
            }
            else
            {
                // if the wait handle name was specified on the command line, then the test infrastructure
                // must have created one for us to wait on
                // we do this instead of blocking on the command line because it has proven to be more robust
                // and much easier to shut down the service when disposing the workspace
                EventWaitHandle waitHandle = EventWaitHandle.OpenExisting(waitHandleName);

                // blocks until the wait handle is triggered by the test infrastructure
                waitHandle.WaitOne();
            }
        }
    }

    //[[ServiceCode]]
}

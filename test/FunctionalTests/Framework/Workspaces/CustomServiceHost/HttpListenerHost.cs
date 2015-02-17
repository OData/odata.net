//---------------------------------------------------------------------
// <copyright file="HttpListenerHost.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using Microsoft.OData.Service;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Web;

    #endregion Namespaces
    [DebuggerDisplay("HttpListenerHost {serviceUri} {requestPathInfo}")]
    public class HttpListenerHost : IDataServiceHost2
    {
        // Input to cached parsed values.
        private string parsedValuesRequestPathInfo;
        // request headers
        private WebHeaderCollection standardRequestHeaders = new WebHeaderCollection();
        // response headers
        private WebHeaderCollection standardResponseHeaders = new WebHeaderCollection();
        //Cache for parsed values.
        private NameValueCollection parsedValues;
        private Dictionary<string, string> queryStringValues;
        private string requestPathInfo;
        // HttpListenerContext containing request and response information
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

        public HttpListenerHost(HttpListenerContext context)
        {
            // retrieve request information from HttpListenerContext
            HttpListenerRequest contextRequest = context.Request;
            this.AbsoluteRequestUri = new Uri(contextRequest.Url.AbsoluteUri);
            int index = contextRequest.Url.AbsoluteUri.IndexOf("host/");
            this.AbsoluteServiceUri = new Uri(contextRequest.Url.AbsoluteUri.Substring(0, index + 5));
            this.requestPathInfo = contextRequest.Url.AbsoluteUri.Substring(index + 5);

            this.httpListenerContext = context;

            // retrieve request headers
            this.RequestAccept = contextRequest.Headers[HttpAccept];
            this.RequestAcceptCharSet = contextRequest.Headers[HttpAcceptCharset];
            this.RequestIfMatch = contextRequest.Headers[HttpIfMatch];
            this.RequestIfNoneMatch = contextRequest.Headers[HttpIfNoneMatch];
            this.RequestMaxVersion = contextRequest.Headers[HttpMaxDataServiceVersion];
            this.RequestVersion = contextRequest.Headers[HttpDataServiceVersion];
            this.RequestContentType = contextRequest.ContentType;
            retrieveHeaderValues(standardRequestHeaders, contextRequest.Headers);
            this.queryStringValues = new Dictionary<string, string>();
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
            [DebuggerStepThrough]
            get { return this.queryStringValues; }
        }
        public string RequestPathInfo
        {
            get { return this.requestPathInfo; }
            set { this.requestPathInfo = value; }
        }

        /// <summary>Gets the value for the specified item in the request query string.</summary>
        /// <param name="item">Item to return.</param>
        /// <returns>
        /// The value for the specified item in the request query string;
        /// null if <paramref name="item"/> is not found.
        /// </returns>
        public string GetQueryStringItem(string item)
        {
            if (queryStringValues.Count == 0)
            {
                // Populate from query string if not cached.
                if (this.requestPathInfo != this.parsedValuesRequestPathInfo)
                {
                    string queryString = this.AbsoluteServiceUri.Query;
                    this.parsedValues = HttpUtility.ParseQueryString(queryString);
                    this.parsedValuesRequestPathInfo = this.requestPathInfo;
                }

                string[] result = this.parsedValues.GetValues(item);
                if (result == null || result.Length == 0)
                {
                    return null;
                }
                else if (result.Length == 1)
                {
                    return result[0];
                }
                else
                {
                    throw new DataServiceException(400, "HttpListenerHost GetQueryString('" + item + "')");
                }
            }
            else
            {
                string result;
                queryStringValues.TryGetValue(item, out result);
                return result;
            }
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
                throw new Exception(args.Exception.Message);
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
            get { return this.httpListenerContext.Request.HttpMethod; }
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
            set { this.httpListenerContext.Response.StatusCode = value; }
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
            get { return this.standardRequestHeaders; }
        }

        /// <summary>Enumerates all response headers that has been set.</summary>
        public WebHeaderCollection ResponseHeaders
        {
            get { return this.httpListenerContext.Response.Headers; }
        }

        #endregion

        /// <summary>
        /// Copy a NameValueCollection to a Dictionary.
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        private void retrieveHeaderValues(NameValueCollection destination, NameValueCollection source)
        {

            String[] keys = source.AllKeys;

            for (int i = 0; i < source.Count; i++)
            {
                destination.Add(keys[i], source.Get(keys[i]));
            }
        }

        /// <summary>Flag to indicate if method ProcessException is called.</summary>
        public bool processExceptionCalled
        {
            get;
            set;
        }
    }
}

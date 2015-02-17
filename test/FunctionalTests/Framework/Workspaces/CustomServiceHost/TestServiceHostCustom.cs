//---------------------------------------------------------------------
// <copyright file="TestServiceHostCustom.cs" company="Microsoft">
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

    [DebuggerDisplay("TestServiceHost {serviceUri} {requestPathInfo}")]
    public class TestServiceHost : IDataServiceHost
    {
        public static readonly Restorable<bool> AllowServerToSerializeException = new Restorable<bool>(false);

        protected WebHeaderCollection standardRequestHeaders = new WebHeaderCollection();
        protected WebHeaderCollection standardResponseHeaders = new WebHeaderCollection();

        private string httpMethod;
        private Exception plainException;
        private Dictionary<string, string> queryStringValues;
        private string requestPathInfo;
        private Stream requestStream;
        private int responseStatusCode;
        private Stream responseStream;
        private Uri serviceUri;
        private Uri requestUri;

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


        /// <summary>Cache for parsed values.</summary>
        private NameValueCollection parsedValues;

        /// <summary>Input to cached parsed values.</summary>
        private Uri parsedValuesRequestUri;

        /// <summary>The root URI of the service.</summary>
        private readonly Uri serviceRootUri;

        /// <summary>The prefix for the absolute request URI.</summary>
        private readonly string requestUriPrefix;

        public TestServiceHost(Uri serviceRootUri = null)
        {
            this.httpMethod = "GET";
            this.responseStream = new MemoryStream();
            this.queryStringValues = new Dictionary<string, string>();
            this.serviceRootUri = serviceRootUri ?? new Uri("http://host/");
            this.requestUriPrefix = serviceRootUri == null ? "http://host" : serviceRootUri.OriginalString.TrimEnd('/');
        }

        public Uri AbsoluteRequestUri
        {
            get
            {
                if (this.requestUri != null)
                {
                    return this.requestUri;
                }

                return new Uri(this.requestUriPrefix + this.RequestPathInfo);
            }

            set { this.requestUri = value; }
        }

        public Uri AbsoluteServiceUri
        {
            get { return this.serviceRootUri; }
        }

        public string RequestAcceptCharSet
        {
            get { return this.standardRequestHeaders[TestServiceHost.HttpAcceptCharset]; }
            set { this.standardRequestHeaders[TestServiceHost.HttpAcceptCharset] = value; }
        }

        public string RequestAccept
        {
            get { return this.standardRequestHeaders[TestServiceHost.HttpAccept]; }
            set { this.standardRequestHeaders[TestServiceHost.HttpAccept] = value; }
        }

        public long RequestContentLength
        {
            get { return long.Parse(this.standardRequestHeaders[TestServiceHost.HttpContentLength]); }
            set { this.standardRequestHeaders[TestServiceHost.HttpContentLength] = value.ToString(); }
        }

        public string RequestContentType
        {
            get { return this.standardRequestHeaders[TestServiceHost.HttpContentType]; }
            set { this.standardRequestHeaders[TestServiceHost.HttpContentType] = value; }
        }

        public string RequestHttpMethod
        {
            get { return this.httpMethod; }
            set { this.httpMethod = value; }
        }

        public string RequestIfMatch
        {
            get { return this.standardRequestHeaders[TestServiceHost.HttpIfMatch]; }
            set { this.standardRequestHeaders[TestServiceHost.HttpIfMatch] = value; }
        }

        public string RequestIfNoneMatch
        {
            get { return this.standardRequestHeaders[TestServiceHost.HttpIfNoneMatch]; }
            set { this.standardRequestHeaders[TestServiceHost.HttpIfNoneMatch] = value; }
        }

        public string RequestMaxVersion
        {
            get { return this.standardRequestHeaders[TestServiceHost.HttpMaxDataServiceVersion]; }
            set { this.standardRequestHeaders[TestServiceHost.HttpMaxDataServiceVersion] = value; }
        }

        public string RequestPathInfo
        {
            [DebuggerStepThrough]
            get { return this.requestPathInfo; }

            [DebuggerStepThrough]
            set { this.requestPathInfo = value; }
        }

        public Stream RequestStream
        {
            get
            {
                if (requestStream != null)
                {
                    this.requestStream.Position = 0;
                }
                return this.requestStream;
            }
            set { this.requestStream = value; }
        }

        public string RequestVersion
        {
            get { return this.standardRequestHeaders[TestServiceHost.HttpDataServiceVersion]; }
            set { this.standardRequestHeaders[TestServiceHost.HttpDataServiceVersion] = value; }
        }

        public string ResponseCacheControl
        {
            get { return this.standardResponseHeaders[TestServiceHost.HttpCacheControl]; }
            set { this.standardResponseHeaders[TestServiceHost.HttpCacheControl] = value; }
        }

        public string ResponseContentType
        {
            get { return this.standardResponseHeaders[TestServiceHost.HttpContentType]; }
            set
            {
                AstoriaTestLog.IsTrue(this.ResponseStatusCode != 204, "Response content type should be set only when status code is something other than 204");
                this.standardResponseHeaders[TestServiceHost.HttpContentType] = value;
            }
        }

        public int ResponseStatusCode
        {
            [DebuggerStepThrough]
            get { return this.responseStatusCode; }

            [DebuggerStepThrough]
            set
            {
                AstoriaTestLog.IsTrue(value != 204 || this.ResponseContentType == null, "Response content type should be null when status code is 204");
                this.responseStatusCode = value;
            }
        }

        public Dictionary<string, string> QueryStringValues
        {
            [DebuggerStepThrough]
            get { return this.queryStringValues; }
        }

        public string ResponseETag
        {
            get { return this.standardResponseHeaders[TestServiceHost.HttpETag]; }
            set { this.standardResponseHeaders[TestServiceHost.HttpETag] = value; }
        }

        public string ResponseLocation
        {
            get { return this.standardResponseHeaders[TestServiceHost.HttpLocation]; }
            set { this.standardResponseHeaders[TestServiceHost.HttpLocation] = value; }
        }

        public Stream ResponseStream
        {
            [DebuggerStepThrough]
            get { return this.responseStream; }
        }

        public string ResponseVersion
        {
            get { return this.standardResponseHeaders[TestServiceHost.HttpDataServiceVersion]; }
            set { this.standardResponseHeaders[TestServiceHost.HttpDataServiceVersion] = value; }
        }

        public Uri ServiceUri
        {
            [DebuggerStepThrough]
            get { return this.serviceUri; }

            [DebuggerStepThrough]
            set { this.serviceUri = value; }
        }

        public void ClearResponse()
        {
            this.plainException = null;
            this.standardResponseHeaders.Clear();
            this.responseStatusCode = 0;
            this.responseStream = new MemoryStream();
        }

        public string GetQueryStringItem(string item)
        {
            if (queryStringValues.Count == 0)
            {
                // Populate from query string if not cached.
                if (this.AbsoluteRequestUri != this.parsedValuesRequestUri)
                {
                    string queryString = this.AbsoluteRequestUri.Query;
                    this.parsedValues = HttpUtility.ParseQueryString(queryString);
                    this.parsedValuesRequestUri = this.AbsoluteRequestUri;
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
                    throw new DataServiceException(400, "GetQueryString('" + item + "')");
                }
            }
            else
            {
                string result;
                queryStringValues.TryGetValue(item, out result);
                return result;
            }
        }

        public void ProcessException(HandleExceptionArgs args)
        {
            this.plainException = args.Exception;

            DataServiceException dse = args.Exception as DataServiceException;
            if (dse != null)
            {
                if (dse.StatusCode == (int)HttpStatusCode.NotModified)
                {
                    // 304 is not a failure, we let the server handle it.
                    return;
                }
            }

            if (AllowServerToSerializeException.Value)
            {
                this.ResponseStatusCode = args.ResponseStatusCode;
                this.ResponseContentType = args.ResponseContentType;
            }
            else
            {
                // set the right status code and content type
                this.ResponseStatusCode = args.ResponseStatusCode;
                this.ResponseContentType = "text/plain";

                // write the error message in the payload
                StreamWriter writer = new StreamWriter(this.responseStream);
                writer.WriteLine("TestServiceHost.ProcessException special pre-writing handling:");
                writer.Write(args.Exception.Message);
                writer.Flush();

                // Re-throw the exception. This makes things consistent for tests,
                // which except an exception from HttpWebRequest.StatusCode <> 200 as well.
                throw new WebException("WebException from TestServiceHost.ProcessException", args.Exception);
            }
        }

        internal Exception PlainException
        {
            get { return this.plainException; }
        }

        internal DataServiceException ServiceException
        {
            get { return this.plainException as DataServiceException; }
        }
    }

    public class TestServiceHost2 : TestServiceHost, IDataServiceHost2
    {
        public TestServiceHost2(Uri serviceRootUri = null)
            : base(serviceRootUri)
        {
        }

        #region IDataServiceHost2 Members

        /// <summary>Gets custom headers for this request.</summary>
        public WebHeaderCollection RequestHeaders
        {
            [DebuggerStepThrough]
            get { return this.standardRequestHeaders; }
        }

        public WebHeaderCollection ResponseHeaders
        {
            get { return this.standardResponseHeaders; }
        }

        #endregion

    }
}

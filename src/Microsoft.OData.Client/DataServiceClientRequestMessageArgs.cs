//---------------------------------------------------------------------
// <copyright file="DataServiceClientRequestMessageArgs.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net.Http;

    /// <summary>
    /// Arguments for creating an instance of DataServiceClientRequestMessage.
    /// </summary>
    public class DataServiceClientRequestMessageArgs
    {
        /// <summary> The actual method. </summary>
        private readonly string actualMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceClientRequestMessageArgs"/> class.
        /// </summary>
        /// <param name="method">Method of the request.</param>
        /// <param name="requestUri">The Request Uri.</param>
        /// <param name="usePostTunneling">True if the request message must use POST verb for the request and pass the actual verb in X-HTTP-Method header, otherwise false.</param>
        /// <param name="headers">The set of headers for the request.</param>
        public DataServiceClientRequestMessageArgs(string method, Uri requestUri, bool usePostTunneling, IDictionary<string, string> headers)
            : this(method, requestUri, usePostTunneling, headers, httpClientFactory: null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceClientRequestMessageArgs"/> class.
        /// </summary>
        /// <param name="method">Method of the request.</param>
        /// <param name="requestUri">The Request Uri.</param>
        /// <param name="usePostTunneling">True if the request message must use POST verb for the request and pass the actual verb in X-HTTP-Method header, otherwise false.</param>
        /// <param name="headers">The set of headers for the request.</param>
        /// <param name="httpClientFactory">The <see cref="IHttpClientFactory"/> that provides the <see cref="HttpClient"/> that should be used to send the request message.</param>
        public DataServiceClientRequestMessageArgs(string method, Uri requestUri, bool usePostTunneling, IDictionary<string, string> headers, IHttpClientFactory httpClientFactory)
        {
            Debug.Assert(method != null, "method cannot be null");
            Debug.Assert(requestUri != null, "requestUri cannot be null");
            Debug.Assert(headers != null, "headers cannot be null");

            this.Headers = headers;
            this.Method = method;
            this.RequestUri = requestUri;
            this.UsePostTunneling = usePostTunneling;
            this.HttpClientFactory = httpClientFactory;

            this.actualMethod = this.Method;
            if (this.UsePostTunneling && this.Headers.ContainsKey(XmlConstants.HttpXMethod))
            {
                this.actualMethod = XmlConstants.HttpMethodPost;
            }
        }

        /// <summary>
        /// Gets the method.
        /// </summary>
        public string Method { get; private set; }

        /// <summary>
        /// Gets the request URI.
        /// </summary>
        public Uri RequestUri { get; private set; }

        /// <summary>
        /// Returns whether the request message should use Post-Tunneling.
        /// </summary>
        public bool UsePostTunneling { get; private set; }

        /// <summary>
        /// Gets the headers.
        /// </summary>
        public IDictionary<string, string> Headers { get; private set; }

        /// <summary>
        /// Gets the actual method. Indicates correct method to use in the post tunneling case.
        /// </summary>
        public string ActualMethod
        {
            get
            {
                return this.actualMethod;
            }
        }

        /// <summary>
        /// Gets the <see cref="IHttpClientFactory"/> that provides the <see cref="HttpClient"/>
        /// that should be used to send the request message.
        /// </summary>
        public IHttpClientFactory HttpClientFactory { get; private set; }
    }
}

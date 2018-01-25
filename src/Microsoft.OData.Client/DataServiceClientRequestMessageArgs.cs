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
    using System.Diagnostics.CodeAnalysis;

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
        /// <param name="useDefaultCredentials">True if the default credentials need to be sent with the request. Otherwise false.</param>
        /// <param name="usePostTunneling">True if the request message must use POST verb for the request and pass the actual verb in X-HTTP-Method header, otherwise false.</param>
        /// <param name="headers">The set of headers for the request.</param>
        public DataServiceClientRequestMessageArgs(string method, Uri requestUri, bool useDefaultCredentials, bool usePostTunneling, IDictionary<string, string> headers)
        {
            Debug.Assert(method != null, "method cannot be null");
            Debug.Assert(requestUri != null, "requestUri cannot be null");
            Debug.Assert(headers != null, "headers cannot be null");

            this.Headers = headers;
            this.Method = method;
            this.RequestUri = requestUri;
            this.UsePostTunneling = usePostTunneling;
            this.UseDefaultCredentials = useDefaultCredentials;

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
        /// Gets a System.Boolean value that controls whether default credentials are sent with requests.
        /// </summary>
        public bool UseDefaultCredentials { get; private set; }
    }
}

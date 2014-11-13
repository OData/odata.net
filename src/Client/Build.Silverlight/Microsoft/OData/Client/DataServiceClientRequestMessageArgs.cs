//   OData .NET Libraries ver. 6.8.1
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "useDefaultCredentials", Justification = "the parameter is used in the SL version.")]
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

#if ASTORIA_LIGHT
        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceClientRequestMessageArgs"/> class.
        /// </summary>
        /// <param name="method">Method of the request.</param>
        /// <param name="requestUri">The Request Uri.</param>
        /// <param name="usePostTunneling">True if the request message must use POST verb for the request and pass the actual verb in X-HTTP-Method header, otherwise false.</param>
        /// <param name="useDefaultCredentials">True if the default credentials need to be sent with the request. Otherwise false.</param>
        /// <param name="headers">The set of headers for the request.</param>
        /// <param name="httpStack">The http client stack to use.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "useDefaultCredentials", Justification = "the parameter is used in the SL version.")]
        public DataServiceClientRequestMessageArgs(string method, Uri requestUri, bool usePostTunneling, bool useDefaultCredentials, IDictionary<string, string> headers, HttpStack httpStack)
            : this(method, requestUri, usePostTunneling, useDefaultCredentials, headers)
        {
            this.ClientHttpStack = httpStack;
        }
#endif

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

#if ASTORIA_LIGHT
        /// <summary>
        /// Gets the http stack.
        /// </summary>
        public HttpStack ClientHttpStack { get; private set; }
#endif

        /// <summary>
        /// Gets a System.Boolean value that controls whether default credentials are sent with requests.
        /// </summary>
        public bool UseDefaultCredentials { get; private set; }
    }
}

//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service.Http
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.OData.Client;

    /// <summary>Contains protocol headers associated with a request or response.</summary>
    internal sealed class ClientWebHeaderCollection : WebHeaderCollection
    {
        /// <summary>
        /// The System.Net header collection this object is wrapping
        /// </summary>
        private System.Net.WebHeaderCollection innerCollection;

        /// <summary>
        /// The System.Net WebRequest this collection is associated with.
        /// This can be null - in which case no request is associated with the headers.
        /// If set some additional headers are allowed - for example the Cookie header
        /// </summary>
        private System.Net.HttpWebRequest request;

        /// <summary>
        /// Constructor for the header collection
        /// </summary>
        /// <param name="collection">The System.Net header collection to wrap</param>
        internal ClientWebHeaderCollection(System.Net.WebHeaderCollection collection)
        {
            Debug.Assert(collection != null, "collection can't be null.");
            this.innerCollection = collection;
        }

        /// <summary>
        /// Constructor for the header collection
        /// </summary>
        /// <param name="collection">The System.Net header collection to wrap.</param>
        /// <param name="request">The System.Net request this header collection is associated with.</param>
        internal ClientWebHeaderCollection(System.Net.WebHeaderCollection collection, System.Net.HttpWebRequest request)
        {
            Debug.Assert(collection != null, "collection can't be null.");
            this.innerCollection = collection;
            this.request = request;
        }

        #region Properties.

        /// <summary>Gets the number of headers in the collection.</summary>
        public override int Count
        {
            get
            {
                return this.innerCollection.Count;
            }
        }

        /// <summary>Collection of header names.</summary>
        public override ICollection<string> AllKeys
        {
            get
            {
                return this.innerCollection.AllKeys;
            }
        }

        /// <summary>Gets or sets a named header.</summary>
        /// <param name="name">Header name.</param>
        /// <returns>The header value.</returns>
        public override string this[string name]
        {
            get
            {
                return this.innerCollection[name];
            }

            set
            {
                if (name == XmlConstants.HttpContentLength)
                {
                    // ignore the Content-Length header since there's no need to set it when using
                    // the Silverlight Client HTTP stack. The stack will set the length correctly depending
                    // on the actual size of the request body.
                    return;
                }
                else if (name == XmlConstants.HttpAcceptCharset)
                {
                    // ignore the Accept-Charset header since the Client HTTP stack doesn't support it.
                    // We don't need it anyway, as the default is UTF-8 which is what we want.
                    Debug.Assert(value == XmlConstants.Utf8Encoding, "Asking for AcceptCharset different thatn UTF-8.");
                    return;
                }
                else if (name == XmlConstants.HttpCookie)
                {
                    if (this.request != null)
                    {
                        // The Client HTTP stack doesn't support setting Cookie header directly
                        //   but it does support CookieContainer, so we marshall the Cookie header
                        //   into the container here.
                        System.Net.CookieContainer cookieContainer = new System.Net.CookieContainer();
                        cookieContainer.SetCookies(this.request.RequestUri, value);
                        this.request.CookieContainer = cookieContainer;
                    }
                    else
                    {
                        // This will very likely fail - which is what we want anyway as we can't marshal the cookies
                        //   to the CookieContainer since we have no associated request 
                        //   (this is probably a response HTTP headers collection)
                        this.innerCollection[name] = value;
                    }
                }
                else
                {
                    this.innerCollection[name] = value;
                }
            }
        }

        /// <summary>Gets or sets a known request header.</summary>
        /// <param name="header">Header to get or set.</param>
        /// <returns>The header value.</returns>
        /// <remarks>Request headers are always allowed, the checks should be removed.</remarks>
        public override string this[Microsoft.OData.Service.Http.HttpRequestHeader header]
        {
            get
            {
                return this[HttpHeaderToName.GetRequestHeaderName(header)];
            }

            set
            {
                // Need to call us again to perform the checks for disallowed headers
                this[HttpHeaderToName.GetRequestHeaderName(header)] = value;
            }
        }
        #endregion Properties.
    }
}

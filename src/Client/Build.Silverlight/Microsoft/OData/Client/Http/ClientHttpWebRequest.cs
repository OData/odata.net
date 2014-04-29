//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service.Http
{
    #region Namespaces.

    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Diagnostics;

    #endregion Namespaces.

    /// <summary>
    /// Provides an HTTP-specific implementation of the WebRequest class
    /// based on the Silverlight's Client HTTP stack
    /// </summary>
    internal sealed class ClientHttpWebRequest : Microsoft.OData.Service.Http.HttpWebRequest
    {
        /// <summary>
        /// The System.Net request that this class is wrapping
        /// </summary>
        private readonly System.Net.HttpWebRequest innerRequest;

        /// <summary>
        /// Our wrapper for the header collection
        /// </summary>
        private ClientWebHeaderCollection headerCollection;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="requestUri">The URI to create the request for</param>
        public ClientHttpWebRequest(Uri requestUri)
        {
            Debug.Assert(requestUri != null, "requestUri can't be null.");
#if PORTABLELIB
            this.innerRequest = System.Net.HttpWebRequest.CreateHttp(requestUri);
#else
            this.innerRequest = (System.Net.HttpWebRequest)System.Net.Browser.WebRequestCreator.ClientHttp.Create(requestUri);
#endif
            Debug.Assert(this.innerRequest != null, "ClientHttp.Create failed to create a new request without throwing exception.");
        }

        /// <summary>Gets or sets the 'Accept' header.</summary>
        public override string Accept
        {
            get
            {
                return this.innerRequest.Accept;
            }

            set
            {
                this.innerRequest.Accept = value;
            }
        }

        /// <summary>Gets or sets the 'Content-Length' header.</summary>
        public override long ContentLength
        {
            set
            {
                // Content-Length is not supported by the Client stack
                // because it will have no effect, the stack will determine the actual length on its own
                // and set the header accordingly
                return;
            }
        }

        /// <summary>Gets or sets a value that indicates whether to buffer the data read from the Internet resource.</summary>
        public override bool AllowReadStreamBuffering
        {
            get
            {
#if PORTABLELIB
                return false;
#else
                return this.innerRequest.AllowReadStreamBuffering;
#endif
            }

            set
            {
#if !PORTABLELIB
                // TODO: 1200403 Determine what the proper thing to do for AllowReadStreamBuffering on PORTABLELIB
                this.innerRequest.AllowReadStreamBuffering = value;
#endif
            }
        }

        /// <summary>
        /// Gets or sets the content type of the request data being sent.
        /// </summary>
        public override string ContentType
        {
            get
            {
                return this.innerRequest.ContentType;
            }

            set
            {
                this.innerRequest.ContentType = value;
            }
        }

        /// <summary>
        /// Gets the collection of header name/value pairs associated with the request.
        /// </summary>
        public override Microsoft.OData.Service.Http.WebHeaderCollection Headers
        {
            get
            {
                if (this.headerCollection == null)
                {
                    this.headerCollection = new ClientWebHeaderCollection(this.innerRequest.Headers, this.innerRequest);
                }

                return this.headerCollection;
            }
        }

        /// <summary>
        /// Gets or sets the protocol method to use in this request.
        /// </summary>
        public override string Method
        {
            get
            {
                return this.innerRequest.Method;
            }

            set
            {
                this.innerRequest.Method = value;
            }
        }

        /// <summary>
        /// Gets the URI of the Internet resource associated with the request.
        /// </summary>
        public override Uri RequestUri
        {
            get
            {
                return this.innerRequest.RequestUri;
            }
        }

        /// <summary>Gets and sets the authentication information used by each query created using the context object.</summary>
        public override System.Net.ICredentials Credentials
        {
            get 
            {
                return this.innerRequest.Credentials;
            }

            set
            {
                this.innerRequest.Credentials = value;
            }
        }

        /// <summary>Gets or sets a System.Boolean value that controls whether default credentials are sent with requests.</summary>
        public override bool UseDefaultCredentials
        {
            get
            {
                return this.innerRequest.UseDefaultCredentials;
            }

            set
            {
                this.innerRequest.UseDefaultCredentials = value;
            }
        }
        
        /// <summary>Cancels a request to an Internet resource.</summary>
        public override void Abort()
        {
            this.innerRequest.Abort();
        }

        /// <summary>
        /// Provides an asynchronous version of the GetRequestStream method.
        /// </summary>
        /// <param name="callback">The AsyncCallback delegate.</param>
        /// <param name="state">The state object for this request.</param>
        /// <returns>An IAsyncResult that references the asynchronous request.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("DataWeb.Usage", "AC0013", Justification = "The WebUtil wrapper is the calling method in this case, so we actually need to call the underlying HttpWebRequest method here.")]
        public override IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
        {
            return this.innerRequest.BeginGetRequestStream(callback, state);
        }

        /// <summary>
        /// Begins an asynchronous request for an Internet resource.
        /// </summary>
        /// <param name="callback">The AsyncCallback delegate.</param>
        /// <param name="state">The state object for this request.</param>
        /// <returns>An IAsyncResult that references the asynchronous request for a response.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("DataWeb.Usage", "AC0013", Justification = "The WebUtil wrapper is the calling method in this case, so we actually need to call the underlying HttpWebRequest method here.")]
        public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
        {

            return this.innerRequest.BeginGetResponse(callback, state);
        }

        /// <summary>
        /// Ends an asynchronous request for a Stream object to use to write data.
        /// </summary>
        /// <param name="asyncResult">The pending request for a stream.</param>
        /// <returns>A Stream to use to write request data.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("DataWeb.Usage", "AC0013", Justification = "The WebUtil wrapper is the calling method in this case, so we actually need to call the underlying HttpWebRequest method here.")]
        public override Stream EndGetRequestStream(IAsyncResult asyncResult)
        {
            return this.innerRequest.EndGetRequestStream(asyncResult);
        }

        /// <summary>
        /// Ends an asynchronous request to an Internet resource.
        /// </summary>
        /// <param name="asyncResult">The pending request for a response.</param>
        /// <returns>A WebResponse that contains the response from the Internet resource.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller will dispose the returned value")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("DataWeb.Usage", "AC0013", Justification = "The WebUtil wrapper is the calling method in this case, so we actually need to call the underlying HttpWebRequest method here.")]
        public override Microsoft.OData.Service.Http.WebResponse EndGetResponse(IAsyncResult asyncResult)
        {
            try
            {
                System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)this.innerRequest.EndGetResponse(asyncResult);
                return new ClientHttpWebResponse(response, this);
            }
            catch (System.Net.WebException exception)
            {
                ClientHttpWebResponse response = exception.Response == null ? null : new ClientHttpWebResponse((System.Net.HttpWebResponse)exception.Response, this);
                throw new Microsoft.OData.Service.Http.WebException(exception.Message, exception, response);
            }
        }

        /// <summary>
        /// Creates an empty instance of the System.Net.WebHeaderCollection with the right settings for this
        /// HTTP request. The two SL stacks need a bit different way to create this object for backward compatibility.
        /// </summary>
        /// <returns>A new empty instance of the System.Net.WebHeaderCollection.</returns>
        public override System.Net.WebHeaderCollection CreateEmptyWebHeaderCollection()
        {
            // Create a new default instance of the WebHeaderCollection.
            // This instance will not block any headers to bet set by the user. This allows us to custom-marshal
            //   some headers. For example Cookie header which is normally disallowed (we will marshall it to the CookieContainer property)
            return new System.Net.WebHeaderCollection();
        }

        /// <summary>
        /// Returns the underlying httpwebrequest instance if the httpStack is clientHttp. Otherwise returns null.
        /// </summary>
        /// <returns>The underlying httpwebrequest instance if the httpStack is clientHttp. Otherwise returns null.</returns>
        internal override System.Net.HttpWebRequest GetUnderlyingHttpRequest()
        {
            return this.innerRequest;
        }

        /// <summary>Releases resources.</summary>
        /// <param name="disposing">Whether the dispose is being called explicitly rather than by the GC.</param>
        protected override void Dispose(bool disposing)
        {
            // Nothing to release in this case
        }
    }
}

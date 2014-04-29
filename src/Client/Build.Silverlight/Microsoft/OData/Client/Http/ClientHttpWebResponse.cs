//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service.Http
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;

    /// <summary>
    /// Provides an HTTP-specific implementation of the WebResponse class.
    /// Based on the Silverlight's Client HTTP stack.
    /// </summary>
    internal sealed class ClientHttpWebResponse : Microsoft.OData.Service.Http.HttpWebResponse
    {
        /// <summary>
        /// The System.Net web response this object is wrapping
        /// </summary>
        private System.Net.HttpWebResponse innerResponse;

        /// <summary>
        /// Wrapped header collection
        /// </summary>
        private ClientWebHeaderCollection headerCollection;

        /// <summary>
        /// The request which originated this response
        /// </summary>
        private ClientHttpWebRequest request;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="innerResponse">The System.Net response object to wrap</param>
        /// <param name="request">The originating request</param>
        internal ClientHttpWebResponse(System.Net.HttpWebResponse innerResponse, ClientHttpWebRequest request)
        {
            Debug.Assert(innerResponse != null, "innerResponse can't be null.");
            this.innerResponse = innerResponse;
            this.request = request;
            int statusCode = (int)this.innerResponse.StatusCode;
            if (statusCode > (int)HttpStatusCodeRange.MaxValue || statusCode < (int)HttpStatusCodeRange.MinValue)
            {
                // throw new WebException("Invalid response line returned " + statusCode);
                throw WebException.CreateInternal("HttpWebResponse.NormalizeResponseStatus");
            }
        }

        #region Properties.

        /// <summary>Gets the content length of data being received.</summary>
        public override long ContentLength
        {
            get
            {
                return this.innerResponse.ContentLength;
            }
        }

        /// <summary>Gets the content type of the data being received.</summary>
        public override string ContentType
        {
            get
            {
                return this.innerResponse.ContentType;
            }
        }

        /// <summary>Gets the headers of the data being received.</summary>
        public override Microsoft.OData.Service.Http.WebHeaderCollection Headers
        {
            get
            {
                if (this.headerCollection == null)
                {
                    this.headerCollection = new ClientWebHeaderCollection(this.innerResponse.Headers);
                }

                return this.headerCollection;
            }
        }

        /// <summary>Gets the request that originated this response.</summary>
        public override Microsoft.OData.Service.Http.HttpWebRequest Request
        {
            get
            {
                return this.request;
            }
        }

        /// <summary>Gets the status code for the data being received.</summary>
        public override System.Net.HttpStatusCode StatusCode
        {
            get
            {
                {
                    return (System.Net.HttpStatusCode)(int)this.innerResponse.StatusCode;
                }
            }
        }

        #endregion Properties.

        /// <summary>Closes the response stream.</summary>
        public override void Close()
        {
#if PORTABLELIB
            this.innerResponse.Dispose();
#endif
#if !PORTABLELIB
            this.innerResponse.Close();
#endif
        }

        /// <summary>Gets a specific header by name.</summary>
        /// <param name="headerName">Name of header.</param>
        /// <returns>The value for the header.</returns>
        public override string GetResponseHeader(string headerName)
        {
            return this.innerResponse.Headers[headerName];
        }

        /// <summary>
        /// Gets the underlying <see cref="System.Net.HttpWebResponse"/> if there is one, or null otherwise.
        /// </summary>
        /// <returns>
        /// The underlying response.
        /// </returns>
        public override System.Net.HttpWebResponse GetUnderlyingHttpResponse()
        {
            return this.innerResponse;
        }

        /// <summary>Gets the stream with the response contents.</summary>
        /// <returns>The stream with the response contents.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("DataWeb.Usage", "AC0013", Justification = "The WebUtil wrapper is the calling method in this case, so we actually need to call the underlying HttpWebResponse method here.")]
        public override Stream GetResponseStream()
        {
            return this.innerResponse.GetResponseStream();
        }

        /// <summary>Releases resources.</summary>
        /// <param name="disposing">Whether the dispose is being called explicitly rather than by the GC.</param>
        protected override void Dispose(bool disposing)
        {
            ((IDisposable)this.innerResponse).Dispose();
        }
    }
}


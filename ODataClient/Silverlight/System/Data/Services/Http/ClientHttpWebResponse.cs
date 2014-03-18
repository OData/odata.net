//---------------------------------------------------------------------
// <copyright file="ClientHttpWebResponse.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
// <summary>
//      Provides an HTTP-specific implementation of the WebResponse class.
//      Based on the Silverlight's Client HTTP stack.
// </summary>
//
// @owner  markash
//---------------------------------------------------------------------

namespace System.Data.Services.Http
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;

    /// <summary>
    /// Provides an HTTP-specific implementation of the WebResponse class.
    /// Based on the Silverlight's Client HTTP stack.
    /// </summary>
    internal sealed class ClientHttpWebResponse : System.Data.Services.Http.HttpWebResponse
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

#if WIN8
        /// <summary>
        /// Wrapped status code. Used in cases where the status is being accessed after the response is closed.
        /// This is specific to Win8 because on other platforms, we can call Close and still access some of the members.
        /// On Win8, the Close method has been removed and Dispose is used instead. However, no members are accessible
        /// after Dispose, so we need to cache the ones we need.
        /// </summary>
        private System.Data.Services.Http.HttpStatusCode statusCode;
#endif

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
        public override System.Data.Services.Http.WebHeaderCollection Headers
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
        public override System.Data.Services.Http.HttpWebRequest Request
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
#if WIN8
                if (this.innerResponse == null)
                {
                    return this.statusCode;
                }
                else
#endif
                {
                    return (System.Net.HttpStatusCode)(int)this.innerResponse.StatusCode;
                }
            }
        }

        #endregion Properties.

        /// <summary>Closes the response stream.</summary>
        public override void Close()
        {
#if WIN8 
            if (this.innerResponse != null)
            {
                // Save the headers first if we don't have them already
                if (this.headerCollection == null)
                {
                    this.headerCollection = new ClientWebHeaderCollection(this.innerResponse.Headers);
                }

                this.statusCode = (System.Data.Services.Http.HttpStatusCode)(int)this.innerResponse.StatusCode;

                this.innerResponse.Dispose();
                this.innerResponse = null;
            }
#endif
#if PORTABLELIB
            this.innerResponse.Dispose();
#endif
#if !WIN8 && !PORTABLELIB
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
        public override Net.HttpWebResponse GetUnderlyingHttpResponse()
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


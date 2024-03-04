//---------------------------------------------------------------------
// <copyright file="HttpWebResponseMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using Microsoft.OData;

    /// <summary>
    /// IODataResponseMessage interface implementation
    /// </summary>
    public class HttpWebResponseMessage : IODataResponseMessage, IDisposable
    {
        /// <summary>Cached headers.</summary>
        private readonly HeaderCollection headers;

        /// <summary>A func which returns the response stream.</summary>
        private readonly Func<Stream> getResponseStream;

        /// <summary>The response status code.</summary>
        private readonly int statusCode;

        /// <summary>HttpWebResponse instance.</summary>
        private HttpWebResponse httpWebResponse;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="headers">The headers.</param>
        /// <param name="statusCode">The status code.</param>
        /// <param name="getResponseStream">A function returning the response stream.</param>
        public HttpWebResponseMessage(IDictionary<string, string> headers, int statusCode, Func<Stream> getResponseStream)
        {
            Debug.Assert(headers != null, "headers != null");
            Debug.Assert(getResponseStream != null, "getResponseStream != null");

            this.headers = new HeaderCollection(headers);
            this.statusCode = statusCode;
            this.getResponseStream = getResponseStream;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="headers">The headers.</param>
        /// <param name="statusCode">The status code.</param>
        /// <param name="getResponseStream">A function returning the response stream.</param>
        internal HttpWebResponseMessage(HeaderCollection headers, int statusCode, Func<Stream> getResponseStream)
        {
            Debug.Assert(headers != null, "headers != null");
            Debug.Assert(getResponseStream != null, "getResponseStream != null");

            this.headers = headers;
            this.statusCode = statusCode;
            this.getResponseStream = getResponseStream;
        }

        /// <summary>
        /// Returns the collection of response headers.
        /// </summary>
        public virtual IEnumerable<KeyValuePair<string, string>> Headers
        {
            get
            {
                return this.headers.AsEnumerable();
            }
        }

        /// <summary>
        /// The response status code.
        /// </summary>
        public virtual int StatusCode
        {
            get
            {
                return this.statusCode;
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Returns the value of the header with the given name.
        /// </summary>
        /// <param name="headerName">Name of the header.</param>
        /// <returns>Returns the value of the header with the given name.</returns>
        public virtual string GetHeader(string headerName)
        {
            Util.CheckArgumentNullAndEmpty(headerName, "headerName");
            string result;
            if (this.headers.TryGetHeader(headerName, out result))
            {
                return result;
            }

            // Since the uninitialized value of ContentLength header is -1, we need to return
            // -1 if the content length header is not present
            if (string.Equals(headerName, XmlConstants.HttpContentLength, StringComparison.Ordinal))
            {
                return "-1";
            }

            return null;
        }

        /// <summary>
        /// Sets the value of the header with the given name.
        /// </summary>
        /// <param name="headerName">Name of the header.</param>
        /// <param name="headerValue">Value of the header.</param>
        public virtual void SetHeader(string headerName, string headerValue)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the stream to be used to read the response payload.
        /// </summary>
        /// <returns>Stream from which the response payload can be read.</returns>
        public virtual Stream GetStream()
        {
            return this.getResponseStream();
        }

        /// <summary>
        /// Close the underlying HttpWebResponse.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Perform the actual cleanup work.
        /// </summary>
        /// <param name="disposing">If 'true' this method is called from user code; if 'false' it is called by the runtime.</param>
        protected virtual void Dispose(bool disposing)
        {
            HttpWebResponse response = this.httpWebResponse;
            this.httpWebResponse = null;
            if (response != null)
            {
                ((IDisposable)response).Dispose();
            }
        }
    }
}

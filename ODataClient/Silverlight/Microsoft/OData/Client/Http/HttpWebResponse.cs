//---------------------------------------------------------------------
// <copyright file="HttpWebResponse.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      Provides an HTTP-specific implementation of the WebResponse class.
// </summary>
//
// @owner  markash
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Http
{
    using System;
    using System.Globalization;
    using System.IO;

    /// <summary>
    /// Provides an HTTP-specific implementation of the WebResponse class.
    /// </summary>
    internal abstract class HttpWebResponse : Microsoft.OData.Service.Http.WebResponse, IDisposable
    {
        #region Properties.

        /// <summary>Gets the headers of the data being received.</summary>
        public abstract Microsoft.OData.Service.Http.WebHeaderCollection Headers
        {
            get;
        }

        /// <summary>Gets the request that originated this response.</summary>
        public abstract Microsoft.OData.Service.Http.HttpWebRequest Request
        {
            get;
        }

        /// <summary>Gets the status code for the data being received.</summary>
        public abstract System.Net.HttpStatusCode StatusCode
        {
            get;
        }

        #endregion Properties.

        /// <summary>Gets a specific header by name.</summary>
        /// <param name="headerName">Name of header.</param>
        /// <returns>The value for the header.</returns>
        public abstract string GetResponseHeader(string headerName);

        /// <summary>
        /// Gets the underlying <see cref="System.Net.HttpWebResponse"/> if there is one, or null otherwise.
        /// </summary>
        /// <returns>The underlying response.</returns>
        public abstract System.Net.HttpWebResponse GetUnderlyingHttpResponse();
    }
}


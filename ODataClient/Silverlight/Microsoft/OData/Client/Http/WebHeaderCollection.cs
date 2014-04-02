//---------------------------------------------------------------------
// <copyright file="WebHeaderCollection.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      WebHeaderCollection type.
// </summary>
//
// @owner  markash
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Http
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>Contains protocol headers associated with a request or response.</summary>
    internal abstract class WebHeaderCollection
    {
        #region Properties.

        /// <summary>Gets the number of headers in the collection.</summary>
        public abstract int Count
        {
            get;
        }

        /// <summary>Collection of header names.</summary>
        public abstract ICollection<string> AllKeys
        {
            get;
        }

        /// <summary>Gets or sets a named header.</summary>
        /// <param name="name">Header name.</param>
        /// <returns>The header value.</returns>
        public abstract string this[string name]
        {
            get;
            set;
        }

        /// <summary>Gets or sets a known request header.</summary>
        /// <param name="header">Header to get or set.</param>
        /// <returns>The header value.</returns>
        /// <remarks>Request headers are always allowed, the checks should be removed.</remarks>
        public abstract string this[Microsoft.OData.Service.Http.HttpRequestHeader header]
        {
            get;
            set;
        }
        #endregion Properties.

        /// <summary>
        /// Sets a specified header
        /// </summary>
        /// <param name="header">The request header to set</param>
        /// <param name="value">The value for the header</param>
        public virtual void Set(HttpRequestHeader header, string value)
        {
            this[header] = value;
        }
    }
}

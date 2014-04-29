//---------------------------------------------------------------------
// <copyright file="HeaderInfo.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      Information about HTTP headers.
// </summary>
//
// @owner  markash
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Http
{
    using System;

    /// <summary>Use this class to record read-only information about headers.</summary>
    internal class HeaderInfo
    {
        /// <summary>Header name.</summary>
        internal readonly string HeaderName;

        /// <summary>Whether direct access to header is allowed on header collections.</summary>
        internal readonly bool IsRequestRestricted;

        /// <summary>
        /// Initializes a new <see cref="HeaderInfo"/> instance with AllowMultiValues set to false.
        /// </summary>
        /// <param name="name">Header name.</param>
        /// <param name="requestRestricted">Whether direct access to header is allowed on header collections.</param>
        internal HeaderInfo(string name, bool requestRestricted)
        {
            this.HeaderName = name;
            this.IsRequestRestricted = requestRestricted;
        }
    }
}

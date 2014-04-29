//---------------------------------------------------------------------
// <copyright file="WebHeaderCollectionType.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      WebHeaderCollectionType type.
// </summary>
//
// @owner  markash
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Http
{
    using System;

    /// <summary>Type of header collection.</summary>
    internal enum WebHeaderCollectionType : ushort
    {
        /// <summary>HTTP request.</summary>
        HttpWebRequest = 2,

        /// <summary>Unknown collection type.</summary>
        Unknown = 0,

        /// <summary>General web request.</summary>
        WebRequest = 1
    }
}


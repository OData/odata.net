//---------------------------------------------------------------------
// <copyright file="WebHeaderCollectionType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
// <summary>
//      WebHeaderCollectionType type.
// </summary>
//
// @owner  markash
//---------------------------------------------------------------------

namespace System.Data.Services.Http
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


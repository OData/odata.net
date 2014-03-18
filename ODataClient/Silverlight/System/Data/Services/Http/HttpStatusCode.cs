//---------------------------------------------------------------------
// <copyright file="HttpStatusCode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
// <summary>
//      Enumeration for HTTP status codes.
// </summary>
//
// @owner  markash
//---------------------------------------------------------------------

namespace System.Data.Services.Http
{
    /// <summary>This enumeration provides maximum and minimum acceptable values for status codes.</summary>
    internal enum HttpStatusCodeRange : int
    {
        /// <summary>Maximum valid value.</summary>
        MaxValue = 599,

        /// <summary>Minimum valid value.</summary>
        MinValue = 100
    }
}

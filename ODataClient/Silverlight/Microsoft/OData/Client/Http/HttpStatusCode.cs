//---------------------------------------------------------------------
// <copyright file="HttpStatusCode.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      Enumeration for HTTP status codes.
// </summary>
//
// @owner  markash
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Http
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

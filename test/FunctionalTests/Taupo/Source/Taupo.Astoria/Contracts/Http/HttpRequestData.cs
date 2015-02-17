//---------------------------------------------------------------------
// <copyright file="HttpRequestData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Http
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Data structure for representing a single HTTP request
    /// </summary>
    public class HttpRequestData : HttpRequestData<Uri, byte[]>
    {
        /// <summary>
        /// Converts the generic URI representation into a standard System.Uri
        /// </summary>
        /// <returns>The Uri for the request</returns>
        public override Uri GetRequestUri()
        {
            return this.Uri;
        }

        /// <summary>
        /// Returns the binary body of the request
        /// </summary>
        /// <returns>The binary body of the request</returns>
        public override byte[] GetRequestBody()
        {
            return this.Body;
        }
    }
}

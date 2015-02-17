//---------------------------------------------------------------------
// <copyright file="IHttpRequest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Http
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Read-only interface for HTTP-based requests
    /// </summary>
    public interface IHttpRequest : IHttpMessage
    {
        /// <summary>
        /// Gets the reqeust verb
        /// </summary>
        HttpVerb Verb { get; }

        /// <summary>
        /// Returns the request's URI
        /// </summary>
        /// <returns>The URI of the request</returns>
        Uri GetRequestUri();

        /// <summary>
        /// Returns the binary body of the request
        /// </summary>
        /// <returns>The binary body of the request</returns>
        byte[] GetRequestBody();
    }
}

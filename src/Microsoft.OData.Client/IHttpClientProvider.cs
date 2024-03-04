//---------------------------------------------------------------------
// <copyright file="IHttpClientHandlerProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Net.Http;

namespace Microsoft.OData.Client
{
    /// <summary>
    /// Provides the <see cref="HttpClient"/> instance(s) used by OData Client
    /// to execute requests.
    /// </summary>
    public interface IHttpClientProvider
    {
        /// <summary>
        /// Returns an <see cref="HttpClient"/> instance that OData Client should
        /// used to execute a request. This method is called per request.
        /// </summary>
        /// <returns>The <see cref="HttpClient"/> instance that will be used to execute the request.</returns>
        /// <remarks>
        /// OData Client does not dispose the <see cref="HttpClient"/> instance.
        /// </remarks>
        HttpClient GetHttpClient();
    }
}

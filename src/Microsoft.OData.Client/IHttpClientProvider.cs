//---------------------------------------------------------------------
// <copyright file="HttpClientFactory.cs" company="Microsoft">
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
        /// Returns the <see cref="HttpClient"/> instance that
        /// OData Client should used to execute the specified request.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="disposeClient">Whether the caller should dispose the returned <see cref="HttpClient"/>.
        /// Set this to true only if you want the HttpClient to be used for a single request then disposed automatically.
        /// If the HttpClient will be re-used for different requests or needs to outlive the request, then set it to false.</param>
        /// <returns>The <see cref="HttpClient"/> instanced that will be used to execute the request.</returns>
        HttpClient GetHttpClient(HttpRequestMessage request, out bool disposeClient);
    }
}

//---------------------------------------------------------------------
// <copyright file="IHttpClientHandlerProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Net.Http;

namespace Microsoft.OData.Client
{
    /// <summary>
    /// Provides the <see cref="HttpClientHandler"/> instance(s) used by OData Client
    /// to execute requests.
    /// </summary>
    public interface IHttpClientHandlerProvider
    {
        /// <summary>
        /// Returns an <see cref="HttpClientHandler"/> instance that OData Client should
        /// used to execute a request. This method is called per request.
        /// </summary>
        /// <returns>The <see cref="HttpClientHandler"/> instance that will be used to execute the request.</returns>
        /// <remarks>
        /// It's possible that the returned <see cref="HttpClientHandler"/> may be mutated inside a request hook
        /// e.g. via <see cref="HttpClientRequestMessage.Credentials"/>.
        /// OData Client does not dispose the <see cref="HttpClientHandler"/> instance.
        /// </remarks>
        HttpClientHandler GetHttpClientHandler();
    }
}

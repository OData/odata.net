//---------------------------------------------------------------------
// <copyright file="IHttpImplementation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Http
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Abstracts the networking stack used to make HTTP requests
    /// </summary>
    [ImplementationSelector("HttpImplementation", DefaultImplementation = "System.Net", HelpText = "The underlying HTTP implementation to use when making requests")]
    public interface IHttpImplementation
    {
        /// <summary>
        /// Sends the given request and returns the response. Should handle any exceptions thrown by the underlying stack and simply return the HTTP response as it was.
        /// </summary>
        /// <param name="request">The request to send</param>
        /// <returns>The response to the given request</returns>
        HttpResponseData GetResponse(IHttpRequest request);
    }
}

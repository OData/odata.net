//---------------------------------------------------------------------
// <copyright file="IODataRequestManager.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Contract for a component for creating and using OData requests and responses
    /// </summary>
    [ImplementationSelector("ODataRequestManager", DefaultImplementation = "Default")]
    public interface IODataRequestManager
    {
        /// <summary>
        /// Constructs a new OData request using default settings
        /// </summary>
        /// <returns>An OData request</returns>
        ODataRequest BuildRequest();

        /// <summary>
        /// Constructs an OData request with the given uri, verb, and headers
        /// </summary>
        /// <param name="uri">The uri for the request</param>
        /// <param name="verb">The verb for the request</param>
        /// <param name="headers">The headers for the request</param>
        /// <returns>A populated OData request</returns>
        ODataRequest BuildRequest(ODataUri uri, HttpVerb verb, IEnumerable<KeyValuePair<string, string>> headers);

        /// <summary>
        /// Constructs an OData response to the given request using the given response data
        /// </summary>
        /// <param name="request">The odata request that was made</param>
        /// <param name="underlyingResponse">The response data</param>
        /// <returns>An odata response</returns>
        ODataResponse BuildResponse(ODataRequest request, HttpResponseData underlyingResponse);

        /// <summary>
        /// Constructs an OData request body using the given settings
        /// </summary>
        /// <param name="contentType">The content type of the body</param>
        /// <param name="uri">The request uri</param>
        /// <param name="rootElement">The root payload element</param>
        /// <returns>An OData request body</returns>
        ODataPayloadBody BuildBody(string contentType, ODataUri uri, ODataPayloadElement rootElement);

        /// <summary>
        /// Gets a response to the given request using an injected Http stack implementation
        /// </summary>
        /// <param name="request">The request to send</param>
        /// <returns>A response for the given request</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "Method is only intended to be used for OData-level requests")]
        ODataResponse GetResponse(ODataRequest request);
    }
}
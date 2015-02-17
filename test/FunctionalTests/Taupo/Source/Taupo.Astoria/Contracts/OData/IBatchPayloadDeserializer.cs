//---------------------------------------------------------------------
// <copyright file="IBatchPayloadDeserializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// A contract for deserializing OData batch payloads
    /// </summary>
    [ImplementationSelector("BatchPayloadDeserializer", DefaultImplementation = "Default")]
    public interface IBatchPayloadDeserializer
    {
        /// <summary>
        /// Deserializes the given request's binary payload into a batch payload
        /// </summary>
        /// <param name="request">The request to deserialize</param>
        /// <returns>The deserialized batch request payload</returns>
        BatchRequestPayload DeserializeBatchRequest(HttpRequestData request);

        /// <summary>
        /// Deserializes the given response's binary payload into a batch payload
        /// </summary>
        /// <param name="requestPayload">The batch request payload that corresponds to the request</param>
        /// <param name="response">The response to deserialize</param>
        /// <returns>The deserialized batch response payload</returns>
        BatchResponsePayload DeserializeBatchResponse(BatchRequestPayload requestPayload, HttpResponseData response);
    }
}
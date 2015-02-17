//---------------------------------------------------------------------
// <copyright file="IBatchPayloadSerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// A contract for serializing OData batch payloads
    /// </summary>
    [ImplementationSelector("BatchPayloadSerializer", DefaultImplementation = "Default")]
    public interface IBatchPayloadSerializer
    {
        /// <summary>
        /// Serializes the batch request payload
        /// </summary>
        /// <param name="payload">The batch payload</param>
        /// <param name="boundary">The batch payload boundary</param>
        /// <param name="encodingName">Optional name of an encoding to use if it is relevant to the current format. May be null if no character-set information was known.</param>
        /// <returns>The serialized batch payload</returns>
        byte[] SerializeBatchPayload(BatchRequestPayload payload, string boundary, string encodingName);

        /// <summary>
        /// Serializes the batch response payload
        /// </summary>
        /// <param name="payload">The batch payload</param>
        /// <param name="boundary">The batch payload boundary</param>
        /// <param name="encodingName">Optional name of an encoding to use if it is relevant to the current format. May be null if no character-set information was known.</param>
        /// <returns>The serialized batch payload</returns>
        byte[] SerializeBatchPayload(BatchResponsePayload payload, string boundary, string encodingName);
    }
}
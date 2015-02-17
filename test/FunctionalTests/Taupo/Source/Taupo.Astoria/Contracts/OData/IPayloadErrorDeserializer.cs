//---------------------------------------------------------------------
// <copyright file="IPayloadErrorDeserializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    /// <summary>
    /// A contract for deserializing OData payloads
    /// </summary>
    public interface IPayloadErrorDeserializer
    {
        /// <summary>
        /// Deserializes the given HTTP payload a error payload or returns null
        /// </summary>
        /// <param name="serialized">Bytes of the Payload</param>
        /// <param name="encodingName">Optional name of an encoding to use if it is relevant to the current format. May be null if no character-set information was known.</param>
        /// <param name="errorPayload">Error payload that is found</param>
        /// <returns>True if it finds and error, false if not</returns>
        bool TryDeserializeErrorPayload(byte[] serialized, string encodingName, out ODataPayloadElement errorPayload);
    }
}
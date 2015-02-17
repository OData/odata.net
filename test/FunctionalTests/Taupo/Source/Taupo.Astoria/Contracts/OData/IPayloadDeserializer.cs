//---------------------------------------------------------------------
// <copyright file="IPayloadDeserializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    /// <summary>
    /// A contract for deserializing OData payloads
    /// </summary>
    public interface IPayloadDeserializer
    {
        /// <summary>
        /// Deserializes the given HTTP payload into a rich representation according to the rules of this format
        /// </summary>
        /// <param name="serialized">The binary that was sent over HTTP</param>
        /// <param name="payloadContext">Additional payload information to aid deserialization</param>
        /// <returns>The payload element</returns>
        ODataPayloadElement DeserializeFromBinary(byte[] serialized, ODataPayloadContext payloadContext);
    }
}
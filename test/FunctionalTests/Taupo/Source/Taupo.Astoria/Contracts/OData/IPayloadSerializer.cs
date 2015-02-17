//---------------------------------------------------------------------
// <copyright file="IPayloadSerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    /// <summary>
    /// A contract for serializing OData payloads
    /// </summary>
    public interface IPayloadSerializer
    {
        /// <summary>
        /// Serializes the given payload element into a form ready to be sent over HTTP
        /// </summary>
        /// <param name="rootElement">The root payload element to serialize</param>
        /// <param name="encodingName">Optional name of an encoding to use if it is relevant to the current format. May be null if no character-set information was known.</param>
        /// <returns>The binary representation of the payload for this format, ready to be sent over HTTP</returns>
        byte[] SerializeToBinary(ODataPayloadElement rootElement, string encodingName);
    }
}
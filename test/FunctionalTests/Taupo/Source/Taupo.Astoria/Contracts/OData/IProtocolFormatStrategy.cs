//---------------------------------------------------------------------
// <copyright file="IProtocolFormatStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Contract for working with OData wire formats.
    /// </summary>
    public interface IProtocolFormatStrategy
    {
        /// <summary>
        /// Gets the serializer for this strategy
        /// </summary>
        /// <returns>A serializer</returns>
        IPayloadSerializer GetSerializer();

        /// <summary>
        /// Gets the deserializer for this strategy
        /// </summary>
        /// <returns>A deserializer</returns>
        IPayloadDeserializer GetDeserializer();

        /// <summary>
        /// Gets the normalizer for this strategy
        /// </summary>
        /// <returns>A payload normalizer</returns>
        IODataPayloadElementNormalizer GetPayloadNormalizer();

        /// <summary>
        /// Gets the primitive value comparer for this strategy
        /// </summary>
        /// <returns>A primitive value comparer</returns>
        IQueryScalarValueToClrValueComparer GetPrimitiveComparer();
    }
}
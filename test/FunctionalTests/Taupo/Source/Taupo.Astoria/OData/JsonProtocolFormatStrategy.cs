//---------------------------------------------------------------------
// <copyright file="JsonProtocolFormatStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// The serialization strategy for 'application/json;odata.metadata=verbose'
    /// </summary>
    public class JsonProtocolFormatStrategy : IProtocolFormatStrategy
    {
        /// <summary>
        /// Gets or sets the payload element to json converter for this strategy
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IPayloadElementToJsonConverter PayloadElementConverter { get; set; }

        /// <summary>
        /// Gets or sets the json to payload element converter for this strategy
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IJsonToPayloadElementConverter JsonConverter { get; set; }

        /// <summary>
        /// Gets or sets the normalizer to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public JsonPayloadNormalizer Normalizer { get; set; }

        /// <summary>
        /// Gets or sets the primitive value comparer to use for this format
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public JsonPrimitiveValueComparer PrimitiveValueComparer { get; set; }

        /// <summary>
        /// Gets or sets PayloadErrorDeserializer
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public JsonPayloadErrorDeserializer JsonPayloadErrorDeserializer { get; set; }

        /// <summary>
        /// Gets the serializer for this strategy
        /// </summary>
        /// <returns>The serializer</returns>
        public IPayloadSerializer GetSerializer()
        {
            return new JsonPayloadSerializer(this.PayloadElementConverter.ConvertToJson);
        }

        /// <summary>
        /// Gets the deserializer for this strategy
        /// </summary>
        /// <returns>The deserializer</returns>
        public IPayloadDeserializer GetDeserializer()
        {
            return new JsonPayloadDeserializer(this.JsonConverter, this.JsonPayloadErrorDeserializer);
        }

        /// <summary>
        /// Gets the normalizer for this strategy
        /// </summary>
        /// <returns>A payload normalizer</returns>
        public IODataPayloadElementNormalizer GetPayloadNormalizer()
        {
            return this.Normalizer;
        }

        /// <summary>
        /// Gets the primitive value comparer for this strategy
        /// </summary>
        /// <returns>A primitive value comparer</returns>
        public IQueryScalarValueToClrValueComparer GetPrimitiveComparer()
        {
            return this.PrimitiveValueComparer;
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="SimpleProtocolFormatStrategyBase.cs" company="Microsoft">
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
    /// A base implementation of a payload serialization strategy for things like $value, binary, etc
    /// </summary>
    public abstract class SimpleProtocolFormatStrategyBase : IProtocolFormatStrategy, IPayloadSerializer, IPayloadDeserializer, IODataPayloadElementNormalizer, IPayloadErrorDeserializer
    {
        /// <summary>
        /// Gets or sets the primitive value comparer to use for this format
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IQueryScalarValueToClrValueComparer PrimitiveValueComparer { get; set; }

        /// <summary>
        /// Gets the serializer for this strategy
        /// </summary>
        /// <returns>The serializer</returns>
        public virtual IPayloadSerializer GetSerializer()
        {
            return this;
        }

        /// <summary>
        /// Gets the deserializer for this strategy
        /// </summary>
        /// <returns>The deserializer</returns>
        public virtual IPayloadDeserializer GetDeserializer()
        {
            return this;
        }

        /// <summary>
        /// Gets the normalizer for this strategy
        /// </summary>
        /// <returns>The normalizer</returns>
        public virtual IODataPayloadElementNormalizer GetPayloadNormalizer()
        {
            return this;
        }

        /// <summary>
        /// Serializes the given root element to a binary representation
        /// </summary>
        /// <param name="rootElement">The root element to serialize</param>
        /// <param name="encodingName">Optional name of an encoding to use if it is relevant to the current format. May be null if no character-set information was known.</param>
        /// <returns>The serialized value</returns>
        public abstract byte[] SerializeToBinary(ODataPayloadElement rootElement, string encodingName);

        /// <summary>
        /// Deserializes the given binary payload
        /// </summary>
        /// <param name="serialized">The serialized value</param>
        /// <param name="payloadContext">Additional payload information to aid deserialization</param>
        /// <returns>A primitive value containing the binary payload</returns>
        public abstract ODataPayloadElement DeserializeFromBinary(byte[] serialized, ODataPayloadContext payloadContext);

        /// <summary>
        /// Deserializes given HTTP payload a error payload or returns null
        /// </summary>
        /// <param name="serialized">The payload that was sent over HTTP</param>
        /// <param name="encodingName">Optional name of an encoding to use if it is relevant to the current format. May be null if no character-set information was known.</param>
        /// <param name="errorPayload">Error payload that is found</param>
        /// <returns>True if it finds and error, false if not</returns>
        public virtual bool TryDeserializeErrorPayload(byte[] serialized, string encodingName, out ODataPayloadElement errorPayload)
        {
            ExceptionUtilities.CheckArgumentNotNull(serialized, "serialized");
            errorPayload = null;
            return false;
        }

        /// <summary>
        /// Normalizes the given payload root
        /// </summary>
        /// <param name="rootElement">The payload to normalize</param>
        /// <returns>The normalized payload</returns>
        public abstract ODataPayloadElement Normalize(ODataPayloadElement rootElement);

        /// <summary>
        /// Gets the primitive value comparer for this strategy
        /// </summary>
        /// <returns>A primitive value comparer</returns>
        public virtual IQueryScalarValueToClrValueComparer GetPrimitiveComparer()
        {
            return this.PrimitiveValueComparer;
        }

        /// <summary>
        /// Apples any changes to the expected payload options based on the specifics of the current format
        /// </summary>
        /// <param name="expected">The expected options</param>
        /// <param name="protocolVersion">The protocol version to consider when updating the expectations</param>
        /// <returns>The options with any updates for the specific format</returns>
        public virtual ODataPayloadOptions UpdateExpectedPayloadOptions(ODataPayloadOptions expected, DataServiceProtocolVersion protocolVersion)
        {
            return expected.Without<ODataPayloadOptions>(ODataPayloadOptions.IncludeTypeNames);
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="CountProtocolFormatStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// The serialization strategy for $count requests
    /// </summary>
    public class CountProtocolFormatStrategy : TextValueProtocolFormatStrategy
    {
        /// <summary>
        /// Initializes a new instance of the CountProtocolFormatStrategy class
        /// </summary>
        public CountProtocolFormatStrategy()
            : base()
        {
        }

        /// <summary>
        /// Serializes the given root element to a binary representation. Not supported for $count.
        /// </summary>
        /// <param name="rootElement">The root element, must be a primitive value</param>
        /// <param name="encodingName">Optional name of an encoding to use if it is relevant to the current format. May be null if no character-set information was known.</param>
        /// <returns>The serialized value</returns>
        public override byte[] SerializeToBinary(ODataPayloadElement rootElement, string encodingName)
        {
            throw new TaupoNotSupportedException("Serialization is not supported for $count");
        }

        /// <summary>
        /// Deserializes the given binary payload as a long integer value
        /// </summary>
        /// <param name="serialized">The serialized value</param>
        /// <param name="payloadContext">Additional payload information to aid deserialization</param>
        /// <returns>A primitive value containing the binary payload</returns>
        public override ODataPayloadElement DeserializeFromBinary(byte[] serialized, ODataPayloadContext payloadContext)
        {
            var value = base.DeserializeFromBinary(serialized, payloadContext) as PrimitiveValue;
            ExceptionUtilities.CheckObjectNotNull(value, "Value unexpectedly null or not a primitive value");
            
            var stringValue = value.ClrValue as string;
            ExceptionUtilities.CheckObjectNotNull(stringValue, "Primitive value unexpectedly null or not a string");

            value.ClrValue = this.XmlPrimitiveConverter.DeserializePrimitive(stringValue, typeof(long));
            return value;
        }

        /// <summary>
        /// Gets the primitive value comparer for this strategy
        /// </summary>
        /// <returns>
        /// A primitive value comparer
        /// </returns>
        public override IQueryScalarValueToClrValueComparer GetPrimitiveComparer()
        {
            return this.PrimitiveValueComparer;
        }
    }
}

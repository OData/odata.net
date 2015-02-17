//---------------------------------------------------------------------
// <copyright file="BinaryValueProtocolFormatStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System.Text;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// A payload serialization strategy for raw $value requests with a binary content type
    /// </summary>
    public class BinaryValueProtocolFormatStrategy : SimpleProtocolFormatStrategyBase
    {
        /// <summary>
        /// Serializes the given root element to a binary representation
        /// </summary>
        /// <param name="rootElement">The root element, must be a primitive value with either a null or binary value</param>
        /// <param name="encodingName">Optional name of an encoding to use if it is relevant to the current format. May be null if no character-set information was known.</param>
        /// <returns>The serialized value</returns>
        public override byte[] SerializeToBinary(ODataPayloadElement rootElement, string encodingName)
        {
            ExceptionUtilities.CheckArgumentNotNull(rootElement, "rootElement");
            ExceptionUtilities.Assert(rootElement.ElementType == ODataPayloadElementType.PrimitiveValue, "Only primitive values can be serialized as raw values. Type was: {0}", rootElement.ElementType);

            var value = (rootElement as PrimitiveValue).ClrValue;

            if (value == null)
            {
                return new byte[0];
            }

            var binary = value as byte[];
            ExceptionUtilities.CheckObjectNotNull(binary, "Payload element was not a binary value");

            return binary;
        }

        /// <summary>
        /// Deserializes the given binary payload as a primitive binary value
        /// </summary>
        /// <param name="serialized">The serialized value</param>
        /// <param name="payloadContext">Additional payload information to aid deserialization</param>
        /// <returns>A primitive value containing the binary payload</returns>
        public override ODataPayloadElement DeserializeFromBinary(byte[] serialized, ODataPayloadContext payloadContext)
        {
            ExceptionUtilities.CheckArgumentNotNull(serialized, "serialized");
            return new PrimitiveValue(null, serialized);
        }

        /// <summary>
        /// Normalizes the given payload root
        /// </summary>
        /// <param name="rootElement">The payload to normalize</param>
        /// <returns>The normalized payload</returns>
        public override ODataPayloadElement Normalize(ODataPayloadElement rootElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(rootElement, "rootElement");

            // TODO: convert to correct CLR type based on metadata annotations
            return rootElement;
        }
    }
}

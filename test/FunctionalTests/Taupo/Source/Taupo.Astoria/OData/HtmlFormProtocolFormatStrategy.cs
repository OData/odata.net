//---------------------------------------------------------------------
// <copyright file="HtmlFormProtocolFormatStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// The serialization strategy for HTML form request body format
    /// </summary>
    public class HtmlFormProtocolFormatStrategy : SimpleProtocolFormatStrategyBase
    {
        /// <summary>
        /// Gets or sets the json converter to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IPayloadElementToJsonConverter JsonConverter { get; set; }

        /// <summary>
        /// Gets or sets the literal converter to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataLiteralConverter LiteralConverter { get; set; }

        /// <summary>
        /// Serializes the given root element to a binary representation
        /// </summary>
        /// <param name="rootElement">The root element</param>
        /// <param name="encodingName">Optional name of an encoding to use if it is relevant to the current format</param>
        /// <returns>The serialized value</returns>
        public override byte[] SerializeToBinary(ODataPayloadElement rootElement, string encodingName)
        {
            ExceptionUtilities.CheckArgumentNotNull(rootElement, "rootElement");
            ExceptionUtilities.Assert(rootElement.ElementType == ODataPayloadElementType.ComplexInstance, "Only complex instances form values can be serialized as html forms. Type was: {0}", rootElement.ElementType);

            var complexInstance = (ComplexInstance)rootElement;

            var queryOpions = new List<KeyValuePair<string, string>>();

            foreach (var property in complexInstance.Properties)
            {
                string value;
                if (property.ElementType == ODataPayloadElementType.PrimitiveProperty)
                {
                    value = this.LiteralConverter.SerializePrimitive(((PrimitiveProperty)property).Value.ClrValue);
                }
                else if (property.ElementType == ODataPayloadElementType.ComplexProperty)
                {
                    value = this.JsonConverter.ConvertToJson(((ComplexProperty)property).Value);
                }
                else if (property.ElementType == ODataPayloadElementType.PrimitiveMultiValueProperty)
                {
                    value = this.JsonConverter.ConvertToJson(((PrimitiveMultiValueProperty)property).Value);
                }
                else
                {
                    ExceptionUtilities.Assert(property.ElementType == ODataPayloadElementType.ComplexMultiValueProperty, "Only primitive, complex, and multi-value properties are supported");
                    value = this.JsonConverter.ConvertToJson(((ComplexMultiValueProperty)property).Value);
                }

                var option = new KeyValuePair<string, string>(property.Name, value);
                queryOpions.Add(option);
            }

            StringBuilder builder = new StringBuilder();
            UriHelpers.ConcatenateQueryOptions(builder, queryOpions);

            var encoding = HttpUtilities.GetEncodingOrDefault(encodingName);
            return encoding.GetBytes(builder.ToString());
        }

        /// <summary>
        /// Deserializes the given binary payload as an html form payload
        /// </summary>
        /// <param name="serialized">The serialized value</param>
        /// <param name="payloadContext">Additional payload information to aid deserialization</param>
        /// <returns>A ODataPayloadElement corresponding to the binary payload</returns>
        public override ODataPayloadElement DeserializeFromBinary(byte[] serialized, ODataPayloadContext payloadContext)
        {
            throw new TaupoNotSupportedException("Deserialization not supported");
        }

        /// <summary>
        /// Normalizes the given payload root
        /// </summary>
        /// <param name="rootElement">The payload to normalize</param>
        /// <returns>The normalized payload</returns>
        public override ODataPayloadElement Normalize(ODataPayloadElement rootElement)
        {
            throw new TaupoNotSupportedException("Normalization not supported");
        }
    }
}

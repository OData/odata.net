//---------------------------------------------------------------------
// <copyright file="TextValueProtocolFormatStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// The serialization strategy for $value requests formatted as text
    /// </summary>
    public class TextValueProtocolFormatStrategy : SimpleProtocolFormatStrategyBase
    {
        /// <summary>
        /// Gets or sets the converter to use when converting property values into text. Text formatting is equivalent to xml formatting.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IXmlPrimitiveConverter XmlPrimitiveConverter { get; set; }

        /// <summary>
        /// Gets or sets PayloadErrorDeserializer
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public HtmlPayloadErrorDeserializer HtmlPayloadErrorDeserializer { get; set; }

        /// <summary>
        /// Serializes the given root element to a binary representation
        /// </summary>
        /// <param name="rootElement">The root element, must be a primitive value</param>
        /// <param name="encodingName">Optional name of an encoding to use if it is relevant to the current format. May be null if no character-set information was known.</param>
        /// <returns>The serialized value</returns>
        public override byte[] SerializeToBinary(ODataPayloadElement rootElement, string encodingName)
        {
            ExceptionUtilities.CheckArgumentNotNull(rootElement, "rootElement");
            ExceptionUtilities.Assert(rootElement.ElementType == ODataPayloadElementType.PrimitiveValue, "Only primitive values can be serialized as raw values. Type was: {0}", rootElement.ElementType);

            var value = (rootElement as PrimitiveValue).ClrValue;
            var encoding = HttpUtilities.GetEncodingOrDefault(encodingName);
            return encoding.GetBytes(this.XmlPrimitiveConverter.SerializePrimitive(value));
        }

        /// <summary>
        /// Deserializes the given binary payload as a text value
        /// </summary>
        /// <param name="serialized">The serialized value</param>
        /// <param name="payloadContext">Additional payload information to aid deserialization</param>
        /// <returns>A primitive value containing the binary payload</returns>
        public override ODataPayloadElement DeserializeFromBinary(byte[] serialized, ODataPayloadContext payloadContext)
        {
            ExceptionUtilities.CheckArgumentNotNull(serialized, "serialized");
            ExceptionUtilities.CheckArgumentNotNull(payloadContext, "payloadContext");

            string encodingName = payloadContext.EncodingName;

            ODataPayloadElement errorPayloadElement = null;
            if (this.HtmlPayloadErrorDeserializer.TryDeserializeErrorPayload(serialized, encodingName, out errorPayloadElement))
            {
                return errorPayloadElement;
            }

            Encoding encoding = HttpUtilities.GetEncodingOrDefault(encodingName);
            
            // cannot guess type, must deserialize as a string
            return new PrimitiveValue(null, encoding.GetString(serialized, 0, serialized.Length));
        }
        
        /// <summary>
        /// Normalizes the given payload root
        /// </summary>
        /// <param name="rootElement">The payload to normalize</param>
        /// <returns>The normalized payload</returns>
        public override ODataPayloadElement Normalize(ODataPayloadElement rootElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(rootElement, "rootElement");

            ExceptionUtilities.Assert(rootElement.ElementType == ODataPayloadElementType.PrimitiveValue, "Only primitive values are supported by this format. Type was: '{0}'", rootElement.ElementType);
            var primitiveValue = (PrimitiveValue)rootElement;

            // replace non-null string with deserialized instance of correct type
            var serialized = primitiveValue.ClrValue as string;
            if (serialized != null)
            {
                var dataType = primitiveValue.Annotations.OfType<DataTypeAnnotation>().Select(t => t.DataType).OfType<PrimitiveDataType>().SingleOrDefault();
                if (dataType != null)
                {
                    var clrType = dataType.GetFacetValue<PrimitiveClrTypeFacet, Type>(null);
                    if (clrType != null && clrType != typeof(string))
                    {
                        var converted = this.XmlPrimitiveConverter.DeserializePrimitive(serialized, clrType);
                        primitiveValue = primitiveValue.ReplaceWith(new PrimitiveValue(primitiveValue.FullTypeName, converted));
                    }
                }
            }

            return primitiveValue;
        }

        /// <summary>
        /// Gets the primitive value comparer for this strategy
        /// </summary>
        /// <returns>
        /// A primitive value comparer
        /// </returns>
        public override IQueryScalarValueToClrValueComparer GetPrimitiveComparer()
        {
            return new TextValuePrimitiveComparer(base.GetPrimitiveComparer(), this.XmlPrimitiveConverter);
        }

        /// <summary>
        /// Primitive comparer for the text format
        /// </summary>
        internal class TextValuePrimitiveComparer : IQueryScalarValueToClrValueComparer
        {
            private readonly IXmlPrimitiveConverter converter;

            /// <summary>
            /// Initializes a new instance of the <see cref="TextValuePrimitiveComparer"/> class.
            /// </summary>
            /// <param name="realComparer">The real comparer.</param>
            /// <param name="converter">The converter.</param>
            public TextValuePrimitiveComparer(IQueryScalarValueToClrValueComparer realComparer, IXmlPrimitiveConverter converter)
            {
                this.UnderlyingComparer = realComparer;
                this.converter = converter;
            }

            /// <summary>
            /// Gets the underlying comparer.
            /// </summary>
            internal IQueryScalarValueToClrValueComparer UnderlyingComparer { get; private set; }

            /// <summary>
            /// Compares the given query scalar value to the given clr value, and throws a DataComparisonException if they dont match
            /// </summary>
            /// <param name="expected">expected query primitive value to compare</param>
            /// <param name="actual">actual CLR value</param>
            /// <param name="assert">The assertion handler to use</param>
            public void Compare(QueryScalarValue expected, object actual, AssertionHandler assert)
            {
                if (expected.IsDynamicPropertyValue())
                {
                    expected = new QueryClrPrimitiveType(typeof(string), expected.Type.EvaluationStrategy).CreateValue(this.converter.SerializePrimitive(expected.Value));
                    actual = this.converter.SerializePrimitive(actual);
                }

                this.UnderlyingComparer.Compare(expected, actual, assert);
            }

            /// <summary>
            /// Compares the given clr value value to the given query scalar value, and throws a DataComparisonException if they dont match
            /// </summary>
            /// <param name="expected">expected CLR value</param>
            /// <param name="actual">actual query primitive value to compare</param>
            /// <param name="assert">The assertion handler to use</param>
            public void Compare(object expected, QueryScalarValue actual, AssertionHandler assert)
            {
                if (actual.IsDynamicPropertyValue())
                {
                    expected = this.converter.SerializePrimitive(expected);
                    actual = new QueryClrPrimitiveType(typeof(string), actual.Type.EvaluationStrategy).CreateValue(this.converter.SerializePrimitive(actual.Value));
                }

                this.UnderlyingComparer.Compare(expected, actual, assert);
            }
        }
    }
}

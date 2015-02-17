//---------------------------------------------------------------------
// <copyright file="XmlProtocolFormatStrategy.cs" company="Microsoft">
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
    /// The serialization strategy for 'application/xml' and 'application/atom+xml'
    /// </summary>
    public class XmlProtocolFormatStrategy : IProtocolFormatStrategy
    {
        /// <summary>
        /// Gets or sets the payload element to xml converter for this strategy
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IPayloadElementToXmlConverter PayloadElementConverter { get; set; }

        /// <summary>
        /// Gets or sets the xml to payload element converter for this strategy
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IXmlToPayloadElementConverter XmlConverter { get; set; }

        /// <summary>
        /// Gets or sets the normalizer to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public XmlPayloadNormalizer Normalizer { get; set; }

        /// <summary>
        /// Gets or sets the primitive value comparer to use for this format
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IQueryScalarValueToClrValueComparer PrimitiveValueComparer { get; set; }

        /// <summary>
        /// Gets or sets PayloadErrorDeserializer
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public XmlPayloadErrorDeserializer XmlPayloadErrorDeserializer { get; set; }

        /// <summary>
        /// Gets the serializer for this strategy
        /// </summary>
        /// <returns>The serializer</returns>
        public IPayloadSerializer GetSerializer()
        {
            return new XmlPayloadSerializer(this.PayloadElementConverter);
        }

        /// <summary>
        /// Gets the deserializer for this strategy
        /// </summary>
        /// <returns>The deserializer</returns>
        public IPayloadDeserializer GetDeserializer()
        {
            return new XmlPayloadDeserializer(this.XmlPayloadErrorDeserializer, this.XmlConverter);
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

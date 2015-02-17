//---------------------------------------------------------------------
// <copyright file="XmlPayloadDeserializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System.IO;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Deserializer for OData payloads for the atom/xml wire format
    /// </summary>
    public sealed class XmlPayloadDeserializer : IPayloadDeserializer
    {
        private readonly IXmlToPayloadElementConverter xmlConverter;
        private readonly IPayloadErrorDeserializer payloadErrorDeserializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlPayloadDeserializer"/> class.
        /// </summary>
        /// <param name="payloadErrorDeserializer">Payload error deserializer</param>
        /// <param name="xmlConverter">The converter from an XML to a payload element representation.</param>
        public XmlPayloadDeserializer(IPayloadErrorDeserializer payloadErrorDeserializer, IXmlToPayloadElementConverter xmlConverter)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadErrorDeserializer, "payloadErrorDeserializer");
            ExceptionUtilities.CheckArgumentNotNull(xmlConverter, "xmlConverter");
            this.payloadErrorDeserializer = payloadErrorDeserializer;
            this.xmlConverter = xmlConverter;
        }

        /// <summary>
        /// Deserializes the given HTTP payload into a rich representation according to the rules of this format
        /// </summary>
        /// <param name="serialized">The binary that was sent over HTTP</param>
        /// <param name="payloadContext">Additional payload information to aid deserialization</param>
        /// <returns>The payload element</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Stream is disposed when the reader is disposed")]
        public ODataPayloadElement DeserializeFromBinary(byte[] serialized, ODataPayloadContext payloadContext)
        {
            ExceptionUtilities.CheckArgumentNotNull(serialized, "serialized");
            ExceptionUtilities.CheckArgumentNotNull(payloadContext, "payloadContext");

            string encodingName = payloadContext.EncodingName;

            ODataPayloadElement errorPayload = null;
            if (this.payloadErrorDeserializer.TryDeserializeErrorPayload(serialized, encodingName, out errorPayload))
            {
                return errorPayload;
            }

            var encoding = HttpUtilities.GetEncodingOrDefault(encodingName);

            XDocument document = null;
            using (StreamReader reader = new StreamReader(new MemoryStream(serialized), encoding))
            {
                document = XDocument.Load(reader);
            }

            ExceptionUtilities.Assert(document != null, "Deserialized document cannot be null.");

            return this.xmlConverter.ConvertToPayloadElement(document.Root);
        }
    }
}

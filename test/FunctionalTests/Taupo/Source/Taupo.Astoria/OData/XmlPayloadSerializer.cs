//---------------------------------------------------------------------
// <copyright file="XmlPayloadSerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System.IO;
    using System.Xml;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Serializer for OData payloads for the atom/xml wire format
    /// </summary>
    public sealed class XmlPayloadSerializer : IPayloadSerializer
    {
        private readonly IPayloadElementToXmlConverter payloadElementConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlPayloadSerializer"/> class.
        /// </summary>
        /// <param name="payloadElementConverter">The converter from a payload element to XML.</param>
        public XmlPayloadSerializer(IPayloadElementToXmlConverter payloadElementConverter)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElementConverter, "payloadElementConverter");
            this.payloadElementConverter = payloadElementConverter;
        }

        /// <summary>
        /// Serializes the given payload element into a form ready to be sent over HTTP
        /// </summary>
        /// <param name="rootElement">The root payload element to serialize</param>
        /// <param name="encodingName">Optional name of an encoding to use if it is relevant to the current format. May be null if no character-set information was known.</param>
        /// <returns>The binary representation of the payload for this format, ready to be sent over HTTP</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "MemoryStream can be safely disposed multiple times")]
        public byte[] SerializeToBinary(ODataPayloadElement rootElement, string encodingName)
        {
            bool omitDeclaration = false;

            SerializationEncodingNameAnnotation encodingNameAnnotation = (SerializationEncodingNameAnnotation)rootElement.GetAnnotation(typeof(SerializationEncodingNameAnnotation));
            if (encodingNameAnnotation != null)
            {
                encodingName = encodingNameAnnotation.EncodingName;
                omitDeclaration = encodingNameAnnotation.OmitDeclaration;
            }

            var encoding = HttpUtilities.GetEncodingOrDefault(encodingName);

            XDocument document = new XDocument(this.payloadElementConverter.ConvertToXml(rootElement));

            if (!omitDeclaration)
            {
                document.Declaration = new XDeclaration("1.0", encoding.WebName, "yes");
            }

            // settings taken from server
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = encoding;
            settings.OmitXmlDeclaration = omitDeclaration;
            settings.Indent = true;
            settings.NewLineHandling = NewLineHandling.Entitize;
            settings.NamespaceHandling = NamespaceHandling.OmitDuplicates;

            using (MemoryStream stream = new MemoryStream())
            {
                using (XmlWriter writer = XmlWriter.Create(stream, settings))
                {
                    document.Save(writer);
                    writer.Flush();

                    return stream.ToArray();
                }
            }
        }
    }
}

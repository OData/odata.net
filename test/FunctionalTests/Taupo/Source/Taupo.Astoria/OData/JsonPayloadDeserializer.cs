//---------------------------------------------------------------------
// <copyright file="JsonPayloadDeserializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System.IO;
    using System.Text;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Json;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Deserializer for OData payloads for the atom/xml wire format
    /// </summary>
    public class JsonPayloadDeserializer : IPayloadDeserializer
    {
        private readonly IJsonToPayloadElementConverter payloadConverter;
        private readonly IPayloadErrorDeserializer payloadErrorDeserializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonPayloadDeserializer"/> class.
        /// </summary>
        /// <param name="payloadConverter">Payload converter</param>
        /// <param name="payloadErrorDeserializer">Payload error deserializer</param>
        public JsonPayloadDeserializer(IJsonToPayloadElementConverter payloadConverter, IPayloadErrorDeserializer payloadErrorDeserializer)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadConverter, "payloadConverter");
            ExceptionUtilities.CheckArgumentNotNull(payloadErrorDeserializer, "payloadErrorDeserializer");
            this.payloadConverter = payloadConverter;
            this.payloadErrorDeserializer = payloadErrorDeserializer;
        }

        /// <summary>
        /// Deserializes the payload using the JsonParser, then converts the resulting clr objects into payload element form
        /// Throws an TaupoInvalidOperationException if the json does not evaluate successfully.
        /// </summary>
        /// <param name="serialized">A raw json payload</param>
        /// <param name="payloadContext">Additional payload information to aid deserialization</param>
        /// <returns>A PayloadElement representation of the payload</returns>
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

            // Evaluate the given JSON text
            Encoding encoding = HttpUtilities.GetEncodingOrDefault(encodingName);
            string payload = encoding.GetString(serialized, 0, serialized.Length);

            JsonValue jsonData = null;
            using (StringReader reader = new StringReader(payload))
            {
                JsonTokenizer tokenizer = new JsonTokenizer(reader);
                JsonParser parser = new JsonParser(tokenizer);
                jsonData = parser.ParseValue();
            }

            // convert the deserialized JsonValue objects into payload elements
            return this.payloadConverter.ConvertToPayloadElement(jsonData);
        }
    }
}

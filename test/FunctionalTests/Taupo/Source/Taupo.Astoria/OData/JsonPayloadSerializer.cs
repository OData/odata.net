//---------------------------------------------------------------------
// <copyright file="JsonPayloadSerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System;
    using System.IO;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Serializer for OData payloads for the json wire format
    /// </summary>
    public sealed class JsonPayloadSerializer : IPayloadSerializer
    {
        private readonly Func<ODataPayloadElement, string> payloadElementToStringConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonPayloadSerializer"/> class.
        /// </summary>
        /// <param name="payloadElementToStringConverter">The converter from a payload element to a Json string.</param>
        public JsonPayloadSerializer(Func<ODataPayloadElement, string> payloadElementToStringConverter)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElementToStringConverter, "payloadElementToStringConverter");
            this.payloadElementToStringConverter = payloadElementToStringConverter;
        }

        /// <summary>
        /// Serializes the given payload element into a form ready to be sent over HTTP
        /// </summary>
        /// <param name="rootElement">The root payload element to serialize</param>
        /// <param name="encodingName">Optional name of an encoding to use if it is relevant to the current format. May be null if no character-set information was known.</param>
        /// <returns>The binary representation of the payload for this format, ready to be sent over HTTP</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Stream is disposed when the writer is disposed")]
        public byte[] SerializeToBinary(ODataPayloadElement rootElement, string encodingName)
        {
            string json = this.payloadElementToStringConverter(rootElement);
            
            var encoding = HttpUtilities.GetEncodingOrDefault(encodingName);
            using (var writer = new StreamWriter(new MemoryStream(), encoding))
            {
                writer.Write(json);
                writer.Flush();
                return ((MemoryStream)writer.BaseStream).ToArray();
            }
        }
    }
}

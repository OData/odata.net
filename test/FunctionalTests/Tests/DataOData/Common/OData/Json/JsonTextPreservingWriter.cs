//---------------------------------------------------------------------
// <copyright file="JsonTextPreservingWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Json
{
    #region Namespaces
    using System.IO;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Json.TextAnnotations;
    #endregion Namespaces

    /// <summary>
    /// JSON writer - writes the JSON object model to a text representation
    /// </summary>
    public class JsonTextPreservingWriter
    {
        /// <summary>
        /// The writer to write to.
        /// </summary>
        private readonly TextWriter writer;

        /// <summary>
        /// True if we're writing JSON Light, false if we're writing Verbose JSON.
        /// </summary>
        private readonly bool writingJsonLight;

        /// <summary>
        /// If it is IEEE754Compatible, read quoted string for INT64 and decimal;
        /// otherwise read number directly.
        /// </summary>
        private readonly bool isIeee754Compatible;

        /// <summary>
        /// Creates a JSON writer.
        /// </summary>
        /// <param name="writer">The writer to write the result into.</param>
        /// <param name="writingJsonLight">true if we are writing JSON Light, false if we're writing Verbose JSON.</param>
        /// <param name="isIeee754Compatible">If it is IEEE754Compatible, default is false</param>
        public JsonTextPreservingWriter(TextWriter writer, bool writingJsonLight, bool isIeee754Compatible = false)
        {
            this.writer = writer;
            this.writingJsonLight = writingJsonLight;
            this.isIeee754Compatible = isIeee754Compatible;
        }

        /// <summary>
        /// Writes the specified JsonValue into the writer.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteValue(JsonValue value)
        {
            switch (value.JsonType)
            {
                case JsonValueType.JsonPrimitiveValue:
                    this.WritePrimitiveValue(value as JsonPrimitiveValue);
                    break;
                case JsonValueType.JsonProperty:
                    this.WriteProperty(value as JsonProperty);
                    break;
                case JsonValueType.JsonArray:
                    this.WriteArray(value as JsonArray);
                    break;
                case JsonValueType.JsonObject:
                    this.WriteObject(value as JsonObject);
                    break;
                default:
                    ExceptionUtilities.Assert(false, "Unknown JSON value type.");
                    break;
            }
        }

        /// <summary>
        /// Writes a text representation of the specified <paramref name="primitiveValue"/> into a text writer.
        /// </summary>
        /// <param name="primitiveValue">The JSON value to write.</param>
        private void WritePrimitiveValue(JsonPrimitiveValue primitiveValue)
        {
            var textAnnotation = primitiveValue.GetAnnotation<JsonPrimitiveValueTextAnnotation>()
                ?? JsonPrimitiveValueTextAnnotation.GetDefault(primitiveValue, this.writingJsonLight, isIeee754Compatible);
            this.writer.Write(textAnnotation.Text);
        }

        /// <summary>
        /// Writes a text representation of the specified <paramref name="propertyValue"/> into a text writer.
        /// </summary>
        /// <param name="propertyValue">The JSON value to write.</param>
        private void WriteProperty(JsonProperty propertyValue)
        {
            var nameTextAnnotation = propertyValue.GetAnnotation<JsonPropertyNameTextAnnotation>() 
                ?? JsonPropertyNameTextAnnotation.GetDefault(propertyValue.Name);
            var nameValueSeparatorTextAnnotation = propertyValue.GetAnnotation<JsonPropertyNameValueSeparatorTextAnnotation>()
                ?? JsonPropertyNameValueSeparatorTextAnnotation.GetDefault();

            this.writer.Write(nameTextAnnotation.Text);
            this.writer.Write(nameValueSeparatorTextAnnotation.Text);
            this.WriteValue(propertyValue.Value);
        }

        /// <summary>
        /// Writes a text representation of the specified <paramref name="arrayValue"/> into a text writer.
        /// </summary>
        /// <param name="arrayValue">The JSON value to write.</param>
        private void WriteArray(JsonArray arrayValue)
        {
            var startArrayTextAnnotation = arrayValue.GetAnnotation<JsonStartArrayTextAnnotation>()
                ?? JsonStartArrayTextAnnotation.GetDefault(this.writer);
            var endArrayTextAnnotation = arrayValue.GetAnnotation<JsonEndArrayTextAnnotation>()
                ?? JsonEndArrayTextAnnotation.GetDefault(this.writer);

            this.writer.Write(startArrayTextAnnotation.Text);

            bool first = true;
            foreach (JsonValue element in arrayValue.Elements)
            {
                var arrayElementSeparatorTextAnnotation = element.GetAnnotation<JsonArrayElementSeparatorTextAnnotation>()
                    ?? JsonArrayElementSeparatorTextAnnotation.GetDefault(first);
                first = false;

                this.writer.Write(arrayElementSeparatorTextAnnotation.Text);
                this.WriteValue(element);
            }

            this.writer.Write(endArrayTextAnnotation.Text);
        }

        /// <summary>
        /// Writes a text representation of the specified <paramref name="objectValue"/> into a text writer.
        /// </summary>
        /// <param name="objectValue">The JSON value to write.</param>
        private void WriteObject(JsonObject objectValue)
        {
            var startObjectTextAnnotation = objectValue.GetAnnotation<JsonStartObjectTextAnnotation>()
                ?? JsonStartObjectTextAnnotation.GetDefault(this.writer);
            var endObjectTextAnnotation = objectValue.GetAnnotation<JsonEndObjectTextAnnotation>()
                ?? JsonEndObjectTextAnnotation.GetDefault(this.writer);

            this.writer.Write(startObjectTextAnnotation.Text);

            bool first = true;
            foreach (JsonProperty propertyValue in objectValue.Properties)
            {
                var propertySeparatorTextAnnotation = propertyValue.GetAnnotation<JsonPropertySeparatorTextAnnotation>()
                    ?? JsonPropertySeparatorTextAnnotation.GetDefault(first);
                first = false;

                this.writer.Write(propertySeparatorTextAnnotation.Text);
                this.WriteValue(propertyValue);
            }

            this.writer.Write(endObjectTextAnnotation.Text);
        }
    }
}

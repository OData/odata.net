//---------------------------------------------------------------------
// <copyright file="IJsonReaderExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Microsoft.OData.Edm.Csdl.Json
{
    /// <summary>
    /// Extensions methods for <see cref="IJsonReader"/>
    /// </summary>
    internal static class IJsonReaderExtensions
    {
        /// <summary>
        /// Read JSON Primitive value.
        /// </summary>
        /// <param name="jsonReader">The <see cref="IJsonReader"/> to read from.</param>
        /// <returns>The <see cref="JsonPrimitiveValue"/> generated.</returns>
        internal static JsonPrimitiveValue ReadAsPrimitive(this IJsonReader jsonReader)
        {
            if (jsonReader == null)
            {
                throw new ArgumentNullException("jsonReader");
            }

            // Make sure the input is a primitive value
            jsonReader.ValidateNodeType(JsonNodeType.PrimitiveValue);

            object value = jsonReader.Value;

            // Move to next token.
            jsonReader.Read();

            return new JsonPrimitiveValue(value);
        }

        /// <summary>
        /// Read JSON Object value.
        /// </summary>
        /// <param name="jsonReader">The <see cref="IJsonReader"/> to read from.</param>
        /// <returns>The <see cref="JsonObjectValue"/> generated.</returns>
        internal static JsonObjectValue ReadAsObject(this IJsonReader jsonReader)
        {
            if (jsonReader == null)
            {
                throw new ArgumentNullException("jsonReader");
            }

            // Supports to read from Begin
            if (jsonReader.NodeType == JsonNodeType.None)
            {
                jsonReader.Read();
            }

            // Make sure the input is an object
            jsonReader.ValidateNodeType(JsonNodeType.StartObject);

            JsonObjectValue objectValue = new JsonObjectValue();

            // Consume the "{" tag.
            jsonReader.Read();

            while (jsonReader.NodeType != JsonNodeType.EndObject)
            {
                // Get the property name and move json reader to next token
                string propertyName = jsonReader.ReadPropertyName();

                // save as propertyName/PropertyValue pair
                objectValue[propertyName] = jsonReader.ReadJsonValue();
            }

            // Consume the "}" tag.
            jsonReader.Read();

            return objectValue;
        }

        /// <summary>
        /// Read JSON Array value.
        /// </summary>
        /// <param name="jsonReader">The <see cref="IJsonReader"/> to read from.</param>
        /// <returns>The <see cref="JsonArrayValue"/> generated.</returns>
        internal static JsonArrayValue ReadAsArray(this IJsonReader jsonReader)
        {
            if (jsonReader == null)
            {
                throw new ArgumentNullException("jsonReader");
            }

            // Supports to read from Begin
            if (jsonReader.NodeType == JsonNodeType.None)
            {
                jsonReader.Read();
            }

            // Make sure the input is an Array
            jsonReader.ValidateNodeType(JsonNodeType.StartArray);

            JsonArrayValue arrayValue = new JsonArrayValue();

            // Consume the "[" tag.
            jsonReader.Read();

            while (jsonReader.NodeType != JsonNodeType.EndArray)
            {
                arrayValue.Add(jsonReader.ReadJsonValue());
            }

            // Consume the "]" tag.
            jsonReader.Read();

            return arrayValue;
        }

        /// <summary>
        /// Reads the value from the <paramref name="jsonReader"/>
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <returns>The <see cref="IJsonValue"/> read.</returns>
        private static IJsonValue ReadJsonValue(this IJsonReader jsonReader)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");

            // Be noted: Json reader already verifies that value should be 'start array, start object or primitive value'
            // For any others, Json reader throws. So we don't care about other Node types.
            Debug.Assert(jsonReader.NodeType == JsonNodeType.StartArray ||
                jsonReader.NodeType == JsonNodeType.StartObject ||
                jsonReader.NodeType == JsonNodeType.PrimitiveValue,
                "json reader node type should be either start array, start object, or primitive value");

            if (jsonReader.NodeType == JsonNodeType.StartArray)
            {
                return jsonReader.ReadAsArray();
            }
            else if (jsonReader.NodeType == JsonNodeType.StartObject)
            {
                return jsonReader.ReadAsObject();
            }

            return jsonReader.ReadAsPrimitive();
        }

        /// <summary>
        /// Reads the next node from the <paramref name="jsonReader"/>,
        /// verifies that it is a Property node and returns the property name.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <returns>The property name of the property node read.</returns>
        private static string ReadPropertyName(this IJsonReader jsonReader)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");

            // Validates it's a property node.
            jsonReader.ValidateNodeType(JsonNodeType.Property);

            // Be noted: the JSON reader already verifies that property names are strings and not null/empty
            string propertyName = (string)jsonReader.Value;

            // Move the JSON reader to point to next token.
            jsonReader.Read();

            return propertyName;
        }

        /// <summary>
        /// Validates that the reader is positioned on the specified node type.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="expectedNodeType">The expected node type.</param>
        private static void ValidateNodeType(this IJsonReader jsonReader, JsonNodeType expectedNodeType)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");
            Debug.Assert(expectedNodeType != JsonNodeType.None, "expectedNodeType != JsonNodeType.None");

            if (jsonReader.NodeType != expectedNodeType)
            {
                throw new InvalidOperationException(Strings.CsdlJsonReader_UnexpectedJsonNodeType(jsonReader.NodeType, expectedNodeType));
            }
        }
    }
}

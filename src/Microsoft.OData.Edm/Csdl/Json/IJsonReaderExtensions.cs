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
        /// Reads the value from the <paramref name="jsonReader"/>
        /// </summary>
        /// <param name="jsonReader">The <see cref="IJsonReader"/> to read from.</param>
        /// <returns>The <see cref="IJsonValue"/> read.</returns>
        public static IJsonValue ReadAsJsonValue(this IJsonReader jsonReader)
        {
            EdmUtil.CheckArgumentNull(jsonReader, "jsonReader");

            // Supports to read from Begin
            if (jsonReader.NodeKind == JsonNodeKind.None)
            {
                jsonReader.Read();
            }

            // Be noted: Json reader already verifies that value should be 'start array, start object or primitive value'
            // For any others, Json reader throws. So we don't care about other Node types.
            Debug.Assert(
                jsonReader.NodeKind == JsonNodeKind.StartArray ||
                jsonReader.NodeKind == JsonNodeKind.StartObject ||
                jsonReader.NodeKind == JsonNodeKind.PrimitiveValue,
                "json reader node type should be either start array, start object, or primitive value");

            if (jsonReader.NodeKind == JsonNodeKind.StartArray)
            {
                return jsonReader.ReadAsArray();
            }
            else if (jsonReader.NodeKind == JsonNodeKind.StartObject)
            {
                return jsonReader.ReadAsObject();
            }

            return jsonReader.ReadAsPrimitive();
        }

        /// <summary>
        /// Read JSON Primitive value.
        /// </summary>
        /// <param name="jsonReader">The <see cref="IJsonReader"/> to read from.</param>
        /// <returns>The <see cref="JsonPrimitiveValue"/> generated.</returns>
        public static JsonPrimitiveValue ReadAsPrimitive(this IJsonReader jsonReader)
        {
            EdmUtil.CheckArgumentNull(jsonReader, "jsonReader");

            // Make sure the input is a primitive value
            jsonReader.ValidateNodeKind(JsonNodeKind.PrimitiveValue);

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
        public static JsonObjectValue ReadAsObject(this IJsonReader jsonReader)
        {
            EdmUtil.CheckArgumentNull(jsonReader, "jsonReader");

            // Supports to read from Begin
            if (jsonReader.NodeKind == JsonNodeKind.None)
            {
                jsonReader.Read();
            }

            // Make sure the input is an object
            jsonReader.ValidateNodeKind(JsonNodeKind.StartObject);

            JsonObjectValue objectValue = new JsonObjectValue();

            // Consume the "{" tag.
            jsonReader.Read();

            while (jsonReader.NodeKind != JsonNodeKind.EndObject)
            {
                // Get the property name and move json reader to next token
                string propertyName = jsonReader.ReadPropertyName();

                // Shall we throw the exception or just ignore it and make the last win?
                if (objectValue.ContainsKey(propertyName))
                {
                    throw new Exception(Strings.JsonReader_DuplicatedProperty(propertyName));
                }

                // save as propertyName/PropertyValue pair
                objectValue[propertyName] = jsonReader.ReadAsJsonValue();
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
        public static JsonArrayValue ReadAsArray(this IJsonReader jsonReader)
        {
            EdmUtil.CheckArgumentNull(jsonReader, "jsonReader");

            // Supports to read from Begin
            if (jsonReader.NodeKind == JsonNodeKind.None)
            {
                jsonReader.Read();
            }

            // Make sure the input is an Array
            jsonReader.ValidateNodeKind(JsonNodeKind.StartArray);

            JsonArrayValue arrayValue = new JsonArrayValue();

            // Consume the "[" tag.
            jsonReader.Read();

            while (jsonReader.NodeKind != JsonNodeKind.EndArray)
            {
                arrayValue.Add(jsonReader.ReadAsJsonValue());
            }

            // Consume the "]" tag.
            jsonReader.Read();

            return arrayValue;
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
            jsonReader.ValidateNodeKind(JsonNodeKind.Property);

            // Be noted: the JSON reader already verifies that property names are strings and not null/empty
            string propertyName = (string)jsonReader.Value;

            // Move the JSON reader to point to next token.
            jsonReader.Read();

            return propertyName;
        }

        /// <summary>
        /// Validates that the reader is positioned on the specified node kind.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="expectedNodeType">The expected node type.</param>
        private static void ValidateNodeKind(this IJsonReader jsonReader, JsonNodeKind expectedNodeType)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");
            Debug.Assert(expectedNodeType != JsonNodeKind.None, "expectedNodeType != JsonNodeType.None");

            if (jsonReader.NodeKind != expectedNodeType)
            {
                throw new Exception(Strings.JsonReader_UnexpectedJsonNodeKind(jsonReader.NodeKind, expectedNodeType));
            }
        }
    }
}

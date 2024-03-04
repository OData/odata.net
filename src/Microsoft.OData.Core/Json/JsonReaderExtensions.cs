//---------------------------------------------------------------------
// <copyright file="JsonReaderExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for the JSON reader.
    /// </summary>
    internal static class JsonReaderExtensions
    {
        /// <summary>
        /// Reads the next node from the <paramref name="jsonReader"/> and verifies that it is a StartObject node.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        internal static void ReadStartObject(this IJsonReader jsonReader)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");

            ReadNext(jsonReader, JsonNodeType.StartObject);
        }

        /// <summary>
        /// Reads the next node from the <paramref name="jsonReader"/> and verifies that it is an EndObject node.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        internal static void ReadEndObject(this IJsonReader jsonReader)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");

            ReadNext(jsonReader, JsonNodeType.EndObject);
        }

        /// <summary>
        /// Reads the next node from the <paramref name="jsonReader"/> and verifies that it is an StartArray node.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <exception cref="ODataException">
        /// Thrown if the <see cref="IJsonReader.NodeType"/> of <paramref name="jsonReader"/> is not <see cref="JsonNodeType.StartArray"/>
        /// </exception>
        internal static void ReadStartArray(this IJsonReader jsonReader)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");

            ReadNext(jsonReader, JsonNodeType.StartArray);
        }

        /// <summary>
        /// Reads the next node from the <paramref name="jsonReader"/> and verifies that it is an EndArray node.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <exception cref="ODataException">
        /// Thrown if the <see cref="IJsonReader.NodeType"/> of <paramref name="jsonReader"/> is not <see cref="JsonNodeType.EndArray"/>
        /// </exception>
        internal static void ReadEndArray(this IJsonReader jsonReader)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");

            ReadNext(jsonReader, JsonNodeType.EndArray);
        }

        /// <summary>
        /// Verifies that the current node is a property node and returns the property name.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <returns>The property name of the current property node.</returns>
        internal static string GetPropertyName(this IJsonReader jsonReader)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");
            Debug.Assert(jsonReader.NodeType == JsonNodeType.Property, "jsonReader.NodeType == JsonNodeType.Property");

            // NOTE: the JSON reader already verifies that property names are strings and not null/empty
            string propertyName = (string)jsonReader.GetValue();

            return propertyName;
        }

        /// <summary>
        /// Reads the next node from the <paramref name="jsonReader"/>, verifies that it is a Property node and returns the property name.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <returns>The property name of the property node read.</returns>
        internal static string ReadPropertyName(this IJsonReader jsonReader)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");

            jsonReader.ValidateNodeType(JsonNodeType.Property);
            string propertyName = jsonReader.GetPropertyName();
            jsonReader.ReadNext();
            return propertyName;
        }

        /// <summary>
        /// Reads the next node from the <paramref name="jsonReader"/> and verifies that it is a PrimitiveValue node.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <returns>The primitive value read from the reader.</returns>
        internal static object ReadPrimitiveValue(this IJsonReader jsonReader)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");

            object value = jsonReader.GetValue();
            ReadNext(jsonReader, JsonNodeType.PrimitiveValue);
            return value;
        }

        /// <summary>
        /// Reads the next node from the <paramref name="jsonReader"/> and verifies that it is a PrimitiveValue node of type string.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <returns>The string value read from the reader; throws an exception if no string value could be read.</returns>
        internal static string ReadStringValue(this IJsonReader jsonReader)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");

            object value = jsonReader.ReadPrimitiveValue();
            string stringValue = value as string;
            if (value == null || stringValue != null)
            {
                return stringValue;
            }

            throw CreateException(Strings.JsonReaderExtensions_CannotReadValueAsString(value));
        }

        /// <summary>
        /// Reads the next node from the <paramref name="jsonReader"/> as a URI and verifies that it is a PrimitiveValue node of type string.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <returns>The string value read from the reader as a URI; throws an exception if no string value could be read.</returns>
        internal static Uri ReadUriValue(this IJsonReader jsonReader)
        {
            return UriUtils.StringToUri(ReadStringValue(jsonReader));
        }

        /// <summary>
        /// Reads the next node from the <paramref name="jsonReader"/> and verifies that it is a PrimitiveValue node of type string.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="propertyName">The name of the property for which to read the string; used in error messages only.</param>
        /// <returns>The string value read from the reader; throws an exception if no string value could be read.</returns>
        internal static string ReadStringValue(this IJsonReader jsonReader, string propertyName)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");

            object value = jsonReader.ReadPrimitiveValue();
            string stringValue = value as string;
            if (value == null || stringValue != null)
            {
                return stringValue;
            }

            throw CreateException(Strings.JsonReaderExtensions_CannotReadPropertyValueAsString(value, propertyName));
        }

        /// <summary>
        /// Reads the next node from the <paramref name="jsonReader"/> and verifies that it is a PrimitiveValue node of type double.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <returns>The double value read from the reader; throws an exception if no double value could be read.</returns>
        internal static double? ReadDoubleValue(this IJsonReader jsonReader)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");

            object value = jsonReader.ReadPrimitiveValue();
            double? doubleValue = value as double?;
            if (value == null || doubleValue != null)
            {
                return doubleValue;
            }

            int? intValue = value as int?;
            if (intValue != null)
            {
                return (double)intValue;
            }

            decimal? decimalValue = value as decimal?;
            if (decimalValue != null)
            {
                return (double)decimalValue;
            }

            throw CreateException(Strings.JsonReaderExtensions_CannotReadValueAsDouble(value));
        }

        /// <summary>
        /// Skips over a JSON value (primitive, object or array).
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <remarks>
        /// Pre-Condition: JsonNodeType.PrimitiveValue, JsonNodeType.StartArray or JsonNodeType.StartObject
        /// Post-Condition: JsonNodeType.PrimitiveValue, JsonNodeType.EndArray or JsonNodeType.EndObject
        /// </remarks>
        internal static void SkipValue(this IJsonReader jsonReader)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");
            int depth = 0;
            do
            {
                switch (jsonReader.NodeType)
                {
                    case JsonNodeType.StartArray:
                    case JsonNodeType.StartObject:
                        depth++;
                        break;

                    case JsonNodeType.EndArray:
                    case JsonNodeType.EndObject:
                        Debug.Assert(depth > 0, "Seen too many scope ends.");
                        depth--;
                        break;

                    default:
                        Debug.Assert(
                            jsonReader.NodeType != JsonNodeType.EndOfInput,
                            "We should not have reached end of input, since the scopes should be well formed. Otherwise JsonReader should have failed by now.");
                        break;
                }
            }
            while (jsonReader.Read() && depth > 0);

            if (depth > 0)
            {
                // Not all open scopes were closed:
                // "Invalid JSON. Unexpected end of input was found in JSON content. Not all object and array scopes were closed."
                throw JsonReaderExtensions.CreateException(Strings.JsonReader_EndOfInputWithOpenScope);
            }
        }

        /// <summary>
        /// Skips over a JSON value (primitive, object or array), and append raw string to StringBuilder.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="jsonRawValueStringBuilder">The StringBuilder to receive JSON raw string.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="jsonRawValueStringBuilder"/> is <see langword="null"/></exception>
        /// <exception cref="ODataException">Thrown if the JSON that <paramref name="jsonReader"/> is reading is not well-formed</exception>
        internal static void SkipValue(this IJsonReader jsonReader, StringBuilder jsonRawValueStringBuilder)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");
            using (StringWriter stringWriter = new StringWriter(jsonRawValueStringBuilder, CultureInfo.InvariantCulture))
            {
                using (JsonWriter jsonWriter = new JsonWriter(stringWriter, isIeee754Compatible: false))
                {
                    int depth = 0;
                    do
                    {
                        switch (jsonReader.NodeType)
                        {
                            case JsonNodeType.PrimitiveValue:
                                if (jsonReader.GetValue() == null)
                                {
                                    jsonWriter.WriteValue((string)null);
                                }
                                else
                                {
                                    jsonWriter.WritePrimitiveValue(jsonReader.GetValue());
                                }

                                break;

                            case JsonNodeType.StartArray:
                                jsonWriter.StartArrayScope();
                                depth++;
                                break;

                            case JsonNodeType.StartObject:
                                jsonWriter.StartObjectScope();
                                depth++;
                                break;

                            case JsonNodeType.EndArray:
                                jsonWriter.EndArrayScope();
                                Debug.Assert(depth > 0, "Seen too many scope ends.");
                                depth--;
                                break;

                            case JsonNodeType.EndObject:
                                jsonWriter.EndObjectScope();
                                Debug.Assert(depth > 0, "Seen too many scope ends.");
                                depth--;
                                break;

                            case JsonNodeType.Property:
                                jsonWriter.WriteName(jsonReader.GetPropertyName());
                                break;

                            default:
                                Debug.Assert(
                                    jsonReader.NodeType != JsonNodeType.EndOfInput,
                                    "We should not have reached end of input, since the scopes should be well formed. Otherwise JsonReader should have failed by now.");
                                break;
                        }
                    }
                    while (jsonReader.Read() && depth > 0);

                    if (depth > 0)
                    {
                        // Not all open scopes were closed:
                        // "Invalid JSON. Unexpected end of input was found in JSON content. Not all object and array scopes were closed."
                        throw JsonReaderExtensions.CreateException(Strings.JsonReader_EndOfInputWithOpenScope);
                    }

                    jsonWriter.Flush();
                }
            }
        }

        /// <summary>
        /// Reads the elements in a JSON array and parses them as <see cref="ODataUntypedValue"/>s
        /// </summary>
        /// <param name="jsonReader">The <see cref="IJsonReader"/> that the collection of untyped values should be read from</param>
        /// <returns>The untyped values in the JSON array in <paramref name="jsonReader"/></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="jsonReader"/> is <see langword="null"/></exception>
        /// <exception cref="ODataException">
        /// Thrown if the <see cref="IJsonReader.NodeType"/> of <paramref name="jsonReader"/> is not <see cref="JsonNodeType.StartArray"/> or the JSON that
        /// <paramref name="jsonReader"/> is reading is not well-formed
        /// </exception>
        internal static IEnumerable<ODataUntypedValue> ReadUntypedCollectionValues(this IJsonReader jsonReader)
        {
            if (jsonReader == null)
            {
                throw new ArgumentNullException(nameof(jsonReader));
            }

            return jsonReader.ReadUntypedCollectionValuesIterator();
        }

        /// <summary>
        /// Reads the elements in a JSON array and parses them as <see cref="ODataUntypedValue"/>s
        /// </summary>
        /// <param name="jsonReader">
        /// The <see cref="IJsonReader"/> that the collection of untyped values should be read from; assumed to not be <see langword="null"/>
        /// </param>
        /// <returns>The untyped values in the JSON array in <paramref name="jsonReader"/></returns>
        /// <exception cref="ODataException">
        /// Thrown if the <see cref="IJsonReader.NodeType"/> of <paramref name="jsonReader"/> is not <see cref="JsonNodeType.StartArray"/> or the JSON that
        /// <paramref name="jsonReader"/> is reading is not well-formed
        /// </exception>
        private static IEnumerable<ODataUntypedValue> ReadUntypedCollectionValuesIterator(this IJsonReader jsonReader)
        {
            jsonReader.ReadStartArray();

            while (jsonReader.NodeType != JsonNodeType.EndArray)
            {
                yield return jsonReader.ReadAsUntypedOrNullValueImplementation();
            }

            jsonReader.ReadEndArray();
        }

        internal static ODataValue ReadAsUntypedOrNullValue(this IJsonReader jsonReader)
        {
            return jsonReader.ReadAsUntypedOrNullValueImplementation();
        }

        /// <summary>
        /// Reads the entirety of the next JSON value (primitive, object or array) in <paramref name="jsonReader"/> as an <see cref="ODataUntypedValue"/>
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <returns>The <see cref="ODataUntypedValue"/> that represents the value read from <paramref name="jsonReader"/></returns>
        /// <exception cref="ODataException">Thrown if the JSON that <paramref name="jsonReader"/> is reading is not well-formed</exception>
        private static ODataUntypedValue ReadAsUntypedOrNullValueImplementation(this IJsonReader jsonReader)
        {
            StringBuilder builder = new StringBuilder();
            jsonReader.SkipValue(builder);
            Debug.Assert(builder.Length > 0, "builder.Length > 0");
            return new ODataUntypedValue()
            {
                RawValue = builder.ToString(),
            };
        }

        internal static ODataValue ReadODataValue(this IJsonReader jsonReader)
        {
            if (jsonReader.NodeType == JsonNodeType.PrimitiveValue)
            {
                object primitiveValue = jsonReader.ReadPrimitiveValue();

                return primitiveValue.ToODataValue();
            }
            else if (jsonReader.NodeType == JsonNodeType.StartObject)
            {
                jsonReader.ReadStartObject();
                ODataResourceValue resourceValue = new ODataResourceValue();
                IList<ODataProperty> propertyList = new List<ODataProperty>();

                while (jsonReader.NodeType != JsonNodeType.EndObject)
                {
                    ODataProperty property = new ODataProperty();
                    property.Name = jsonReader.ReadPropertyName();
                    property.Value = jsonReader.ReadODataValue();
                    propertyList.Add(property);
                }

                resourceValue.Properties = propertyList;

                jsonReader.ReadEndObject();

                return resourceValue;
            }
            else if (jsonReader.NodeType == JsonNodeType.StartArray)
            {
                jsonReader.ReadStartArray();
                ODataCollectionValue collectionValue = new ODataCollectionValue();
                IList<object> propertyList = new List<object>();

                while (jsonReader.NodeType != JsonNodeType.EndArray)
                {
                    propertyList.Add(jsonReader.ReadODataValue());
                }

                collectionValue.Items = propertyList;
                jsonReader.ReadEndArray();

                return collectionValue;
            }
            else
            {
                return jsonReader.ReadAsUntypedOrNullValue();
            }
        }

        /// <summary>
        /// Reads the next node. Use this instead of the direct call to Read since this asserts that there actually is a next node.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <returns>The node type of the node that reader is positioned on after reading.</returns>
        internal static JsonNodeType ReadNext(this IJsonReader jsonReader)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");

#if DEBUG
            bool result = jsonReader.Read();
            Debug.Assert(result, "JsonReader.Read returned false in an unexpected place.");
#else
            jsonReader.Read();
#endif
            return jsonReader.NodeType;
        }

        /// <summary>
        /// Determines if the reader is on a value node.
        /// </summary>
        /// <param name="jsonReader">The reader to inspect.</param>
        /// <returns>true if the reader is on PrimitiveValue, StartObject or StartArray node, false otherwise.</returns>
        internal static bool IsOnValueNode(this IJsonReader jsonReader)
        {
            return IsValueNodeType(jsonReader.NodeType);
        }

        /// <summary>
        /// Asynchronously reads the next node from the <paramref name="jsonReader"/> and verifies that it is a StartObject node.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        internal static Task ReadStartObjectAsync(this IJsonReader jsonReader)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");

            return ReadNextAsync(jsonReader, JsonNodeType.StartObject);
        }

        /// <summary>
        /// Asynchronously reads the next node from the <paramref name="jsonReader"/> and verifies that it is an EndObject node.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        internal static Task ReadEndObjectAsync(this IJsonReader jsonReader)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");

            return ReadNextAsync(jsonReader, JsonNodeType.EndObject);
        }

        /// <summary>
        /// Asynchronously reads the next node from the <paramref name="jsonReader"/> and verifies that it is a StartArray node.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        internal static Task ReadStartArrayAsync(this IJsonReader jsonReader)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");

            return ReadNextAsync(jsonReader, JsonNodeType.StartArray);
        }

        /// <summary>
        /// Asynchronously reads the next node from the <paramref name="jsonReader"/> and verifies that it is an EndArray node.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        internal static Task ReadEndArrayAsync(this IJsonReader jsonReader)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");

            return ReadNextAsync(jsonReader, JsonNodeType.EndArray);
        }

        /// <summary>
        /// Verifies that the current node is a property node and returns the property name.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <returns>A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the property name of the current property node.</returns>
        internal static async Task<string> GetPropertyNameAsync(this IJsonReader jsonReader)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");
            Debug.Assert(jsonReader.NodeType == JsonNodeType.Property, "jsonReader.NodeType == JsonNodeType.Property");

            // NOTE: the JSON reader already verifies that property names are strings and not null/empty
            object value = await jsonReader.GetValueAsync()
                .ConfigureAwait(false);
            return (string)value;
        }

        /// <summary>
        /// Asynchronously reads the next node from the <paramref name="jsonReader"/>, verifies that it is a Property node and returns the property name.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <returns>A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the property name of the property node read.</returns>
        internal static async Task<string> ReadPropertyNameAsync(this IJsonReader jsonReader)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");

            jsonReader.ValidateNodeType(JsonNodeType.Property);
            string propertyName = await jsonReader.GetPropertyNameAsync()
                .ConfigureAwait(false);
            await jsonReader.ReadNextAsync()
                .ConfigureAwait(false);

            return propertyName;
        }

        /// <summary>
        /// Asynchronously reads the next node from the <paramref name="jsonReader"/> and verifies that it is a PrimitiveValue node.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <returns>A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the primitive value read from the reader.</returns>
        internal static async Task<object> ReadPrimitiveValueAsync(this IJsonReader jsonReader)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");

            object value = await jsonReader.GetValueAsync()
                .ConfigureAwait(false);
            await ReadNextAsync(jsonReader, JsonNodeType.PrimitiveValue)
                .ConfigureAwait(false);

            return value;
        }

        /// <summary>
        /// Asynchronously reads the next node from the <paramref name="jsonReader"/> and verifies that it is a PrimitiveValue node of type string.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="propertyName">The name of the property for which to read the string; used in error messages only.</param>
        /// <returns>A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the string value read from the reader;
        /// throws an exception if no string value could be read.</returns>
        internal static async Task<string> ReadStringValueAsync(this IJsonReader jsonReader, string propertyName = null)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");

            object value = await jsonReader.ReadPrimitiveValueAsync()
                .ConfigureAwait(false);

            string stringValue = value as string;
            if (value == null || stringValue != null)
            {
                return stringValue;
            }

            if (!string.IsNullOrEmpty(propertyName))
            {
                throw CreateException(Strings.JsonReaderExtensions_CannotReadPropertyValueAsString(value, propertyName));
            }
            else
            {
                throw CreateException(Strings.JsonReaderExtensions_CannotReadValueAsString(value));
            }
        }

        /// <summary>
        /// Asynchronously reads the next node from the <paramref name="jsonReader"/> as a URI and verifies that it is a PrimitiveValue node of type string.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <returns>A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the string value read from the reader as a URI;
        /// throws an exception if no string value could be read.</returns>
        internal static async Task<Uri> ReadUriValueAsync(this IJsonReader jsonReader)
        {
            return UriUtils.StringToUri(await ReadStringValueAsync(jsonReader)
                .ConfigureAwait(false));
        }

        /// <summary>
        /// Asynchronously reads the next node from the <paramref name="jsonReader"/> and verifies that it is a PrimitiveValue node of type double.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <returns>A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the double value read from the reader;
        /// throws an exception if no double value could be read.</returns>
        internal static async Task<double?> ReadDoubleValueAsync(this IJsonReader jsonReader)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");

            object value = await jsonReader.ReadPrimitiveValueAsync()
                .ConfigureAwait(false);

            double? doubleValue = value as double?;
            if (value == null || doubleValue != null)
            {
                return doubleValue;
            }

            int? intValue = value as int?;
            if (intValue != null)
            {
                return (double)intValue;
            }

            decimal? decimalValue = value as decimal?;
            if (decimalValue != null)
            {
                return (double)decimalValue;
            }

            throw CreateException(Strings.JsonReaderExtensions_CannotReadValueAsDouble(value));
        }

        /// <summary>
        /// Asynchronously skips over a JSON value (primitive, object or array).
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <remarks>
        /// Pre-Condition: JsonNodeType.PrimitiveValue, JsonNodeType.StartArray or JsonNodeType.StartObject
        /// Post-Condition: JsonNodeType.PrimitiveValue, JsonNodeType.EndArray or JsonNodeType.EndObject
        /// </remarks>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        internal static async Task SkipValueAsync(this IJsonReader jsonReader)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");
            int depth = 0;

            do
            {
                switch (jsonReader.NodeType)
                {
                    case JsonNodeType.StartArray:
                    case JsonNodeType.StartObject:
                        depth++;
                        break;

                    case JsonNodeType.EndArray:
                    case JsonNodeType.EndObject:
                        Debug.Assert(depth > 0, "Seen too many scope ends.");
                        depth--;
                        break;

                    default:
                        Debug.Assert(
                            jsonReader.NodeType != JsonNodeType.EndOfInput,
                            "We should not have reached end of input, since the scopes should be well formed. Otherwise JsonReader should have failed by now.");
                        break;
                }
            }
            while (await jsonReader.ReadAsync().ConfigureAwait(false) && depth > 0);

            if (depth > 0)
            {
                // Not all open scopes were closed:
                // "Invalid JSON. Unexpected end of input was found in JSON content. Not all object and array scopes were closed."
                throw CreateException(Strings.JsonReader_EndOfInputWithOpenScope);
            }
        }

        /// <summary>
        /// Asynchronously skips over a JSON value (primitive, object or array), and append raw string to StringBuilder.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="jsonRawValueStringBuilder">The StringBuilder to receive JSON raw string.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        internal static async Task SkipValueAsync(this IJsonReader jsonReader, StringBuilder jsonRawValueStringBuilder)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");
            using (StringWriter stringWriter = new StringWriter(jsonRawValueStringBuilder, CultureInfo.InvariantCulture))
            {
                JsonWriter jsonWriter = new JsonWriter(stringWriter, jsonReader.IsIeee754Compatible);
                int depth = 0;

                do
                {
                    switch (jsonReader.NodeType)
                    {
                        case JsonNodeType.PrimitiveValue:
                            if (await jsonReader.GetValueAsync().ConfigureAwait(false) == null)
                            {
                                await jsonWriter.WriteValueAsync((string)null)
                                    .ConfigureAwait(false);
                            }
                            else
                            {
                                object primitiveValue = await jsonReader.GetValueAsync()
                                    .ConfigureAwait(false);
                                await jsonWriter.WritePrimitiveValueAsync(primitiveValue)
                                    .ConfigureAwait(false);
                            }

                            break;

                        case JsonNodeType.StartArray:
                            await jsonWriter.StartArrayScopeAsync()
                                .ConfigureAwait(false);
                            depth++;
                            break;

                        case JsonNodeType.StartObject:
                            await jsonWriter.StartObjectScopeAsync()
                                .ConfigureAwait(false);
                            depth++;
                            break;

                        case JsonNodeType.EndArray:
                            await jsonWriter.EndArrayScopeAsync()
                                .ConfigureAwait(false);
                            Debug.Assert(depth > 0, "Seen too many scope ends.");
                            depth--;
                            break;

                        case JsonNodeType.EndObject:
                            await jsonWriter.EndObjectScopeAsync()
                                .ConfigureAwait(false);
                            Debug.Assert(depth > 0, "Seen too many scope ends.");
                            depth--;
                            break;

                        case JsonNodeType.Property:
                            string propertyName = await jsonReader.GetPropertyNameAsync()
                                .ConfigureAwait(false);
                            await jsonWriter.WriteNameAsync(propertyName)
                                .ConfigureAwait(false);
                            break;

                        default:
                            Debug.Assert(
                                jsonReader.NodeType != JsonNodeType.EndOfInput,
                                "We should not have reached end of input, since the scopes should be well formed. Otherwise JsonReader should have failed by now.");
                            break;
                    }
                }
                while (await jsonReader.ReadAsync().ConfigureAwait(false) && depth > 0);

                if (depth > 0)
                {
                    // Not all open scopes were closed:
                    // "Invalid JSON. Unexpected end of input was found in JSON content. Not all object and array scopes were closed."
                    throw CreateException(Strings.JsonReader_EndOfInputWithOpenScope);
                }

                await jsonWriter.FlushAsync()
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously reads the next node from the <paramref name="jsonReader"/> as an <see cref="ODataUntypedValue"/>.
        /// </summary>
        /// <param name="jsonReader">The reader to inspect.</param>
        /// <returns>A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the <see cref="ODataUntypedValue"/> value read from the reader.</returns>
        internal static async Task<ODataValue> ReadAsUntypedOrNullValueAsync(this IJsonReader jsonReader)
        {
            StringBuilder builder = new StringBuilder();
            await jsonReader.SkipValueAsync(builder)
                .ConfigureAwait(false);
            Debug.Assert(builder.Length > 0, "builder.Length > 0");

            return new ODataUntypedValue()
            {
                RawValue = builder.ToString(),
            };
        }

        /// <summary>
        /// Asynchronously reads the next node from the <paramref name="jsonReader"/> as an <see cref="ODataValue"/>.
        /// </summary>
        /// <param name="jsonReader">The reader to inspect.</param>
        /// <returns>A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the <see cref="ODataValue"/> value read from the reader.</returns>
        internal static async Task<ODataValue> ReadODataValueAsync(this IJsonReader jsonReader)
        {
            if (jsonReader.NodeType == JsonNodeType.PrimitiveValue)
            {
                object primitiveValue = await jsonReader.ReadPrimitiveValueAsync()
                    .ConfigureAwait(false);

                return primitiveValue.ToODataValue();
            }
            else if (jsonReader.NodeType == JsonNodeType.StartObject)
            {
                await jsonReader.ReadStartObjectAsync()
                    .ConfigureAwait(false);
                ODataResourceValue resourceValue = new ODataResourceValue();
                List<ODataProperty> properties = new List<ODataProperty>();

                while (jsonReader.NodeType != JsonNodeType.EndObject)
                {
                    ODataProperty property = new ODataProperty();
                    property.Name = await jsonReader.ReadPropertyNameAsync()
                        .ConfigureAwait(false);
                    property.Value = await jsonReader.ReadODataValueAsync()
                        .ConfigureAwait(false);
                    properties.Add(property);
                }

                resourceValue.Properties = properties;

                await jsonReader.ReadEndObjectAsync()
                    .ConfigureAwait(false);

                return resourceValue;
            }
            else if (jsonReader.NodeType == JsonNodeType.StartArray)
            {
                await jsonReader.ReadStartArrayAsync()
                    .ConfigureAwait(false);
                ODataCollectionValue collectionValue = new ODataCollectionValue();
                List<object> properties = new List<object>();

                while (jsonReader.NodeType != JsonNodeType.EndArray)
                {
                    ODataValue odataValue = await jsonReader.ReadODataValueAsync()
                        .ConfigureAwait(false);
                    properties.Add(odataValue);
                }

                collectionValue.Items = properties;
                await jsonReader.ReadEndArrayAsync()
                    .ConfigureAwait(false);

                return collectionValue;
            }
            else
            {
                return await jsonReader.ReadAsUntypedOrNullValueAsync()
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Asynchronously reads the next node. Use this instead of the direct call to Read since this asserts that there actually is a next node.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <returns>A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the node type of the node that reader is positioned on after reading.</returns>
        internal static async Task<JsonNodeType> ReadNextAsync(this IJsonReader jsonReader)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");

#if DEBUG
            bool result = await jsonReader.ReadAsync()
                .ConfigureAwait(false);
            Debug.Assert(result, "JsonReader.ReadAsync returned false in an unexpected place.");
#else
            await jsonReader.ReadAsync()
                .ConfigureAwait(false);
#endif
            return jsonReader.NodeType;
        }

        /// <summary>
        /// Asserts that the reader is not buffer.
        /// </summary>
        /// <param name="bufferedJsonReader">The <see cref="BufferingJsonReader"/> to read from.</param>
        [Conditional("DEBUG")]
        internal static void AssertNotBuffering(this BufferingJsonReader bufferedJsonReader)
        {
#if DEBUG
            Debug.Assert(!bufferedJsonReader.IsBuffering, "!bufferedJsonReader.IsBuffering");
#endif
        }

        /// <summary>
        /// Asserts that the reader is buffer.
        /// </summary>
        /// <param name="bufferedJsonReader">The <see cref="BufferingJsonReader"/> to read from.</param>
        [Conditional("DEBUG")]
        internal static void AssertBuffering(this BufferingJsonReader bufferedJsonReader)
        {
#if DEBUG
            Debug.Assert(bufferedJsonReader.IsBuffering, "bufferedJsonReader.IsBuffering");
#endif
        }

        /// <summary>
        /// Creates an exception instance that is appropriate for the current library being built.
        /// Allows the code in this class to be shared between ODataLib and the common spatial library.
        /// </summary>
        /// <param name="exceptionMessage">String to use for the exception message.</param>
        /// <returns>Exception to be thrown.</returns>
        internal static ODataException CreateException(string exceptionMessage)
        {
            return new ODataException(exceptionMessage);
        }

        /// <summary>
        /// Reads the next node from the <paramref name="jsonReader"/> and verifies that it is of the expected node type.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="expectedNodeType">The expected <see cref="JsonNodeType"/> of the read node.</param>
        /// <exception cref="ODataException">
        /// Thrown if the <see cref="IJsonReader.NodeType"/> of <paramref name="jsonReader"/> is not <paramref name="expectedNodeType"/>
        /// </exception>
        private static void ReadNext(this IJsonReader jsonReader, JsonNodeType expectedNodeType)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");
            Debug.Assert(expectedNodeType != JsonNodeType.None, "expectedNodeType != JsonNodeType.None");

            jsonReader.ValidateNodeType(expectedNodeType);
            jsonReader.Read();
        }

        /// <summary>
        /// Validates that the reader is positioned on the specified node type.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to use.</param>
        /// <param name="expectedNodeType">The expected node type.</param>
        /// <exception cref="ODataException">
        /// Thrown if the <see cref="IJsonReader.NodeType"/> of <paramref name="jsonReader"/> is not <paramref name="expectedNodeType"/>
        /// </exception>
        private static void ValidateNodeType(this IJsonReader jsonReader, JsonNodeType expectedNodeType)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");
            Debug.Assert(expectedNodeType != JsonNodeType.None, "expectedNodeType != JsonNodeType.None");

            if (jsonReader.NodeType != expectedNodeType)
            {
                throw CreateException(Strings.JsonReaderExtensions_UnexpectedNodeDetected(expectedNodeType, jsonReader.NodeType));
            }
        }

        /// <summary>
        /// Asynchronously reads the next node from the <paramref name="jsonReader"/> and verifies that it is of the expected node type.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="expectedNodeType">The expected <see cref="JsonNodeType"/> of the read node.</param>
        private static async Task ReadNextAsync(this IJsonReader jsonReader, JsonNodeType expectedNodeType)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");
            Debug.Assert(expectedNodeType != JsonNodeType.None, "expectedNodeType != JsonNodeType.None");

            jsonReader.ValidateNodeType(expectedNodeType);
            await jsonReader.ReadAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Determines if the node type is a value node type.
        /// </summary>
        /// <param name="nodeType">The node type.</param>
        /// <returns>true if the node type is PrimitiveValue, StartObject or StartArray node; false otherwise.</returns>
        private static bool IsValueNodeType(JsonNodeType nodeType)
        {
            return nodeType == JsonNodeType.PrimitiveValue || nodeType == JsonNodeType.StartObject || nodeType == JsonNodeType.StartArray;
        }
    }
}

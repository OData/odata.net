//---------------------------------------------------------------------
// <copyright file="JsonReaderExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if SPATIAL
namespace Microsoft.Data.Spatial
#else
namespace Microsoft.OData.Core.Json
#endif
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using Microsoft.OData.Core.JsonLight;

#if SPATIAL
    using Microsoft.Spatial;
#endif
    #endregion Namespaces

    /// <summary>
    /// Extension methods for the JSON reader.
    /// </summary>
    internal static class JsonReaderExtensions
    {
        /// <summary>
        /// Reads the next node from the <paramref name="jsonReader"/> and verifies that it is a StartObject node.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        internal static void ReadStartObject(this JsonReader jsonReader)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");

            ReadNext(jsonReader, JsonNodeType.StartObject);
        }

        /// <summary>
        /// Reads the next node from the <paramref name="jsonReader"/> and verifies that it is an EndObject node.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        internal static void ReadEndObject(this JsonReader jsonReader)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");

            ReadNext(jsonReader, JsonNodeType.EndObject);
        }

        /// <summary>
        /// Reads the next node from the <paramref name="jsonReader"/> and verifies that it is an StartArray node.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        internal static void ReadStartArray(this JsonReader jsonReader)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");

            ReadNext(jsonReader, JsonNodeType.StartArray);
        }

        /// <summary>
        /// Reads the next node from the <paramref name="jsonReader"/> and verifies that it is an EndArray node.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        internal static void ReadEndArray(this JsonReader jsonReader)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");

            ReadNext(jsonReader, JsonNodeType.EndArray);
        }

        /// <summary>
        /// Verifies that the current node is a property node and returns the property name.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <returns>The property name of the current property node.</returns>
        internal static string GetPropertyName(this JsonReader jsonReader)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");
            Debug.Assert(jsonReader.NodeType == JsonNodeType.Property, "jsonReader.NodeType == JsonNodeType.Property");

            // NOTE: the JSON reader already verifies that property names are strings and not null/empty
            string propertyName = (string)jsonReader.Value;

            return propertyName;
        }

        /// <summary>
        /// Reads the next node from the <paramref name="jsonReader"/>, verifies that it is a Property node and returns the property name.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <returns>The property name of the property node read.</returns>
        internal static string ReadPropertyName(this JsonReader jsonReader)
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
        internal static object ReadPrimitiveValue(this JsonReader jsonReader)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");

            object value = jsonReader.Value;
            ReadNext(jsonReader, JsonNodeType.PrimitiveValue);
            return value;
        }

        /// <summary>
        /// Reads the next node from the <paramref name="jsonReader"/> and verifies that it is a PrimitiveValue node of type string.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <returns>The string value read from the reader; throws an exception if no string value could be read.</returns>
        internal static string ReadStringValue(this JsonReader jsonReader)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");

            object value = jsonReader.ReadPrimitiveValue();
            string stringValue = value as string;
            if (value == null || stringValue != null)
            {
                return stringValue;
            }

            throw CreateException(
                Strings.JsonReaderExtensions_CannotReadValueAsString(value), 
                jsonReader.CaptureParsingContext());
        }

        /// <summary>
        /// Reads the next node from the <paramref name="jsonReader"/> and verifies that it is a PrimitiveValue node of type string.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="propertyName">The name of the property for which to read the string; used in error messages only.</param>
        /// <returns>The string value read from the reader; throws an exception if no string value could be read.</returns>
        internal static string ReadStringValue(this JsonReader jsonReader, string propertyName)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");

            object value = jsonReader.ReadPrimitiveValue();
            string stringValue = value as string;
            if (value == null || stringValue != null)
            {
                return stringValue;
            }

            throw CreateException(
                Strings.JsonReaderExtensions_CannotReadPropertyValueAsString(value, propertyName),
                jsonReader.CaptureParsingContext());
        }

        /// <summary>
        /// Reads the next node from the <paramref name="jsonReader"/> and verifies that it is a PrimitiveValue node of type double.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <returns>The double value read from the reader; throws an exception if no double value could be read.</returns>
        internal static double? ReadDoubleValue(this JsonReader jsonReader)
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

            throw CreateException(
                Strings.JsonReaderExtensions_CannotReadValueAsDouble(value), 
                jsonReader.CaptureParsingContext());
        }

        /// <summary>
        /// Skips over a JSON value (primitive, object or array).
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <remarks>
        /// Pre-Condition: JsonNodeType.PrimitiveValue, JsonNodeType.StartArray or JsonNodeType.StartObject
        /// Post-Condition: JsonNodeType.PrimitiveValue, JsonNodeType.EndArray or JsonNodeType.EndObject
        /// </remarks>
        internal static void SkipValue(this JsonReader jsonReader)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");

            int depth = 0;
            do
            {
                switch (jsonReader.NodeType)
                {
                    case JsonNodeType.PrimitiveValue:
                        break;
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

                jsonReader.ReadNext();
            }
            while (depth > 0);
        }

        /// <summary>
        /// Reads the next node. Use this instead of the direct call to Read since this asserts that there actually is a next node.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <returns>The node type of the node that reader is positioned on after reading.</returns>
        internal static JsonNodeType ReadNext(this JsonReader jsonReader)
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
        internal static bool IsOnValueNode(this JsonReader jsonReader)
        {
            JsonNodeType nodeType = jsonReader.NodeType;
            return nodeType == JsonNodeType.PrimitiveValue || nodeType == JsonNodeType.StartObject || nodeType == JsonNodeType.StartArray;
        }

#if !SPATIAL
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
#endif

        /// <summary>
        /// Creates an exception instance that is appropriate for the current library being built.
        /// Allows the code in this class to be shared between ODataLib and the common spatial library.
        /// </summary>
        /// <param name="exceptionMessage">String to use for the exception message.</param>
        /// <param name="parsingContext">String containing an excerpt of the JSON that caused the error.</param>
        /// <returns>Exception to be thrown.</returns>
#if SPATIAL
        internal static FormatException CreateException(string exceptionMessage)
        {
            return new FormatException(exceptionMessage);
        }
#else
        internal static ODataException CreateException(string exceptionMessage, string parsingContext)
        {
            return new ODataException(Strings.JsonReaderExtensions_ErrorContext(exceptionMessage, parsingContext));
        }
#endif

        /// <summary>
        /// Reads the next node from the <paramref name="jsonReader"/> and verifies that it is of the expected node type.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="expectedNodeType">The expected <see cref="JsonNodeType"/> of the read node.</param>
        private static void ReadNext(this JsonReader jsonReader, JsonNodeType expectedNodeType)
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
        private static void ValidateNodeType(this JsonReader jsonReader, JsonNodeType expectedNodeType)
        {
            Debug.Assert(jsonReader != null, "jsonReader != null");
            Debug.Assert(expectedNodeType != JsonNodeType.None, "expectedNodeType != JsonNodeType.None");

            if (jsonReader.NodeType != expectedNodeType)
            {
                throw CreateException(
                    Strings.JsonReaderExtensions_UnexpectedNodeDetected(expectedNodeType, jsonReader.NodeType), 
                    jsonReader.CaptureParsingContext());
            }
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="IJsonValueExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Microsoft.OData.Edm.Csdl.Json
{
    /// <summary>
    /// Extension methods for <see cref="IJsonValue"/>.
    /// </summary>
    internal static class IJsonValueExtensions
    {
        /// <summary>
        /// Read <see cref="IJsonValue"/> to a string, the input JsonValue should be a string,
        /// otherwise, throw unexpected value kind exception.
        /// </summary>
        /// <param name="jsonValue">The <see cref="IJsonValue"/> to parse from.</param>
        /// <param name="jsonPath">The JSON path for current JSON node which owns this value.</param>
        /// <returns>The string.</returns>
        public static string ReadPrimitiveString(this IJsonValue jsonValue, IJsonPath jsonPath)
        {
            EdmUtil.CheckArgumentNull(jsonValue, "jsonValue");
            EdmUtil.CheckArgumentNull(jsonPath, "jsonPath");

            jsonValue.ValidateValueKind(JsonValueKind.Primitive, jsonPath);

            JsonPrimitiveValue primitiveValue = (JsonPrimitiveValue)jsonValue;
            if (primitiveValue.Value == null)
            {
                return null;
            }

            if (primitiveValue.Value.GetType() == typeof(string))
            {
                return (string)primitiveValue.Value;
            }

            throw new Exception(Strings.JsonReader_CannotReadValueAsType(primitiveValue.Value, jsonPath.Path, "String"));
        }

        /// <summary>
        /// Read <see cref="IJsonValue"/> to a boolean value.the input JsonValue should be a boolean,
        /// otherwise, throw unexpected value kind exception.
        /// </summary>
        /// <param name="jsonValue">The <see cref="IJsonValue"/> to parse from.</param>
        /// <param name="jsonPath">The JSON path for current JSON node which owns this value.</param>
        /// <returns>The boolean value.</returns>
        public static bool? ReadPrimitiveBoolean(this IJsonValue jsonValue, IJsonPath jsonPath)
        {
            EdmUtil.CheckArgumentNull(jsonValue, "jsonValue");
            EdmUtil.CheckArgumentNull(jsonPath, "jsonPath");

            jsonValue.ValidateValueKind(JsonValueKind.Primitive, jsonPath);

            JsonPrimitiveValue primitiveValue = (JsonPrimitiveValue)jsonValue;
            if (primitiveValue.Value == null)
            {
                return null;
            }

            if (primitiveValue.Value.GetType() == typeof(bool))
            {
                // unboxing
                return (bool?)primitiveValue.Value;
            }

            throw new Exception(Strings.JsonReader_CannotReadValueAsType(primitiveValue.Value, jsonPath.Path, "Boolean"));
        }

        /// <summary>
        /// Read <see cref="IJsonValue"/> to an integer value.the input JsonValue should be an integer,
        /// otherwise, throw unexpected value kind exception.
        /// </summary>
        /// <param name="jsonValue">The <see cref="IJsonValue"/> to parse from.</param>
        /// <param name="jsonPath">The JSON path for current JSON node which owns this value.</param>
        /// <returns>The integer value.</returns>
        public static int? ConvertToInteger(this IJsonValue jsonValue, IJsonPath jsonPath)
        {
            EdmUtil.CheckArgumentNull(jsonValue, "jsonValue");
            EdmUtil.CheckArgumentNull(jsonPath, "jsonPath");

            jsonValue.ValidateValueKind(JsonValueKind.Primitive, jsonPath);

            JsonPrimitiveValue primitiveValue = (JsonPrimitiveValue)jsonValue;
            if (primitiveValue.Value == null)
            {
                return null;
            }

            if (primitiveValue.Value.GetType() == typeof(int))
            {
                // unboxing?
                return (int?)primitiveValue.Value;
            }

            throw new Exception(Strings.JsonReader_CannotReadValueAsType(primitiveValue.Value, jsonPath.Path, "Integer"));
        }

        /// <summary>
        /// Read <see cref="IJsonValue"/> to a string.
        /// </summary>
        /// <param name="jsonValue">The <see cref="IJsonValue"/> to parse from.</param>
        /// <returns>The string.</returns>
        public static string ReadAsJsonString(this IJsonValue jsonValue)
        {
            if (jsonValue == null)
            {
                throw new ArgumentNullException("jsonValue");
            }

            if (jsonValue.ValueKind == JsonValueKind.Array)
            {
                bool first = true;
                JsonArrayValue arrayValue = (JsonArrayValue)jsonValue;
                StringBuilder sb = new StringBuilder("[");
                foreach (var item in arrayValue)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        sb.Append(",");
                    }

                    sb.Append(item.ReadAsJsonString());
                }

                sb.Append("]");

                return sb.ToString();
            }
            else if (jsonValue.ValueKind == JsonValueKind.Object)
            {
                JsonObjectValue objectValue = (JsonObjectValue)jsonValue;
                StringBuilder sb = new StringBuilder("{");
                bool first = true;
                foreach (var property in objectValue)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        sb.Append(",");
                    }

                    sb.Append("\"" + property.Key + "\":");
                    sb.Append(property.Value.ReadAsJsonString());
                }

                sb.Append("}");

                return sb.ToString();
            }
            else
            {
                JsonPrimitiveValue primitiveValue = (JsonPrimitiveValue)jsonValue;
                if (primitiveValue.Value == null)
                {
                    return "null";
                }

                Type type = primitiveValue.Value.GetType();
                if (type == typeof(bool))
                {
                    bool boolValue = (bool)primitiveValue.Value;
                    if (boolValue)
                    {
                        return "true";
                    }

                    return "false";
                }

                if (type == typeof(int))
                {
                    int intValue = (int)primitiveValue.Value;
                    return intValue.ToString(CultureInfo.InvariantCulture);
                }

                if (type == typeof(decimal))
                {
                    decimal decimalValue = (decimal)primitiveValue.Value;
                    return decimalValue.ToString(CultureInfo.InvariantCulture);
                }

                if (type == typeof(double))
                {
                    double doubleValue = (double)primitiveValue.Value;
                    return doubleValue.ToString(CultureInfo.InvariantCulture);
                }

                return "\"" + primitiveValue.Value.ToString() + "\"";
            }
        }

        /// <summary>
        /// Validates that the reader is positioned on the specified node type.
        /// </summary>
        /// <param name="jsonValue">The <see cref="IJsonValue"/> to read from.</param>
        /// <param name="expectedKind">The expected JSON value kind.</param>
        /// <param name="jsonPath">The JSON path for current JSON value.</param>
        private static void ValidateValueKind(this IJsonValue jsonValue, JsonValueKind expectedKind, IJsonPath jsonPath)
        {
            Debug.Assert(jsonValue != null, "jsonValue != null");
            Debug.Assert(jsonPath != null, "jsonPath != null");

            if (jsonValue.ValueKind != expectedKind)
            {
                throw new Exception(Strings.JsonReader_UnexpectedJsonValueKind(jsonValue.ValueKind, jsonPath, expectedKind));
            }
        }
    }
}

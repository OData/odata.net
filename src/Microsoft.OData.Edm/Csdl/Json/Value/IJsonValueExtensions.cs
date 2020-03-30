//---------------------------------------------------------------------
// <copyright file="IJsonValueExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Microsoft.OData.Edm.Csdl.Json.Value
{
    /// <summary>
    /// Extensions methods for <see cref="IJsonValue"/>
    /// </summary>
    internal static class IJsonValueExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (enumerable != null)
            {
                foreach (T item in enumerable)
                {
                    action(item);
                }
            }
        }

        public static T ParseOptionalProperty<T>(this JsonObjectValue objValue, string propertyName, IJsonPath jsonPath,
            Func<IJsonValue, IJsonPath, T> propertyParser)
        {
            IJsonValue value;
            if (!objValue.TryGetValue(propertyName, out value))
            {
                return default;
            }

            jsonPath.Push(propertyName);
            T obj = propertyParser(value, jsonPath);
            jsonPath.Pop();
            return obj;
        }

        public static T ParseRequiredProperty<T>(this JsonObjectValue objValue, string propertyName,JsonPath jsonPath,
            Func<IJsonValue, IJsonPath, T> propertyParser)
        {
            IJsonValue value;
           if (!objValue.TryGetValue(propertyName, out value))
            {
                throw new Exception();
            }

            jsonPath.Push(propertyName);
            T obj = propertyParser(value, jsonPath);
            jsonPath.Pop();
            return obj;
        }

        public static void ProcessProperty(this JsonObjectValue objValue, IJsonPath jsonPath,
            Action<string, IJsonValue> propertyParser)
        {
            foreach (var propertyItem in objValue)
            {
                // The property value is either primitive, array or another object,
                // It could not be a null.
                // for null, it's wrappered in JsonPrimitiveValue.
                Debug.Assert(propertyItem.Value != null);

                jsonPath.Push(propertyItem.Key);

                propertyParser(propertyItem.Key, propertyItem.Value);

                jsonPath.Pop();
            }
        }

        public static void ProcessProperty(this JsonObjectValue objValue, IJsonPath jsonPath,
            ISet<string> skipSets,
            Action<string, IJsonValue> propertyParser)
        {
            foreach (var propertyItem in objValue)
            {
                // The property value is either primitive, array or another object,
                // It could not be a null.
                // for null, it's wrappered in JsonPrimitiveValue.
                Debug.Assert(propertyItem.Value != null);

                // skip
                if (skipSets.Contains(propertyItem.Key))
                {
                    continue;
                }

                jsonPath.Push(propertyItem.Key);

                propertyParser(propertyItem.Key, propertyItem.Value);

                jsonPath.Pop();
            }
        }

        public static void ProcessProperty(this JsonObjectValue objValue, IJsonPath jsonPath,
            Action<string, IJsonValue, IJsonPath> propertyParser)
        {
            foreach (var propertyItem in objValue)
            {
                // The property value is either primitive, array or another object,
                // It could not be a null.
                // for null, it's wrappered in JsonPrimitiveValue.
                Debug.Assert(propertyItem.Value != null);

                jsonPath.Push(propertyItem.Key);

                propertyParser(propertyItem.Key, propertyItem.Value, jsonPath);

                jsonPath.Pop();
            }
        }

        public static void ProcessItem(this JsonArrayValue arrayValue, IJsonPath jsonPath, Action<IJsonValue> itemParser)
        {
            int index = 0;
            foreach (var item in arrayValue)
            {
                // The property value is either primitive, array or another object,
                // It could not be a null.
                // for null, it's wrappered in JsonPrimitiveValue.
                Debug.Assert(item != null);

                jsonPath.Push(index++);

                itemParser(item);

                jsonPath.Pop();
            }
        }

        /// <summary>
        /// Parse <see cref="IJsonValue"/> to a string.
        /// </summary>
        /// <param name="jsonValue">The <see cref="IJsonValue"/> to parse from.</param>
        /// <returns>The string.</returns>
        public static string ReadAsJsonString(this IJsonValue jsonValue)
        {
            if (jsonValue == null)
            {
                throw new ArgumentNullException("jsonValue");
            }

            if (jsonValue.ValueKind == JsonValueKind.JArray)
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
            else if (jsonValue.ValueKind == JsonValueKind.JObject)
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

        public static bool TryParseAsString(this IJsonValue jsonValue, out string stringValue)
        {
            stringValue = null;
            if (jsonValue == null || jsonValue.ValueKind != JsonValueKind.JPrimitive)
            {
                return false;
            }

            JsonPrimitiveValue primitiveValue = (JsonPrimitiveValue)jsonValue;
            if (primitiveValue.Value == null)
            {
                return false;
            }

            if (primitiveValue.Value.GetType() != typeof(string))
            {
                return false;
            }


            stringValue = (string)primitiveValue.Value;
            return true;
        }

        /// <summary>
        /// Parse <see cref="IJsonValue"/> to a string.
        /// </summary>
        /// <param name="jsonValue">The <see cref="IJsonValue"/> to parse from.</param>
        /// <param name="jsonPath">The JSON path for current JSON node which owns this value.</param>
        /// <returns>The string.</returns>
        public static string ParseAsString(this IJsonValue jsonValue, IJsonPath jsonPath)
        {
            if (jsonValue == null)
            {
                throw new ArgumentNullException("jsonValue");
            }

            if (jsonPath == null)
            {
                throw new ArgumentNullException("jsonPath");
            }

            jsonValue.ValidateValueKind(JsonValueKind.JPrimitive, jsonPath);

            JsonPrimitiveValue primitiveValue = (JsonPrimitiveValue)jsonValue;
            if (primitiveValue.Value == null)
            {
                return null;
            }

            if (primitiveValue.Value.GetType() == typeof(string))
            {
                // unboxing
                return (string)primitiveValue.Value;
            }

            throw new CsdlParseException(
                    Strings.CsdlJsonParser_CannotReadValueAsType(primitiveValue.Value, jsonPath.Path, "Boolean"));
        }

        /// <summary>
        /// Parse <see cref="IJsonValue"/> to a boolean value.
        /// </summary>
        /// <param name="jsonValue">The <see cref="IJsonValue"/> to parse from.</param>
        /// <param name="jsonPath">The JSON path for current JSON node which owns this value.</param>
        /// <returns>The boolean value.</returns>
        public static bool? ParseAsBoolean(this IJsonValue jsonValue, IJsonPath jsonPath)
        {
            if (jsonValue == null)
            {
                throw new ArgumentNullException("jsonValue");
            }

            if (jsonPath == null)
            {
                throw new ArgumentNullException("jsonPath");
            }

            jsonValue.ValidateValueKind(JsonValueKind.JPrimitive, jsonPath);

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

            throw new CsdlParseException(
                    Strings.CsdlJsonParser_CannotReadValueAsType(primitiveValue.Value, jsonPath.Path, "Boolean"));
        }

        /// <summary>
        /// Parse <see cref="IJsonValue"/> to an integer value.
        /// </summary>
        /// <param name="jsonValue">The <see cref="IJsonValue"/> to parse from.</param>
        /// <param name="jsonPath">The JSON path for current JSON node which owns this value.</param>
        /// <returns>The integer value.</returns>
        public static int? ParseAsInteger(this IJsonValue jsonValue, IJsonPath jsonPath)
        {
            if (jsonValue == null)
            {
                throw new ArgumentNullException("jsonValue");
            }

            if (jsonPath == null)
            {
                throw new ArgumentNullException("jsonPath");
            }

            jsonValue.ValidateValueKind(JsonValueKind.JPrimitive, jsonPath);

            JsonPrimitiveValue primitiveValue = (JsonPrimitiveValue)jsonValue;
            if (primitiveValue.Value == null)
            {
                return null;
            }

            if (primitiveValue.Value.GetType() == typeof(int))
            {
                // unboxing
                return (int?)primitiveValue.Value;
            }

            throw new CsdlParseException(
                    Strings.CsdlJsonParser_CannotReadValueAsType(primitiveValue.Value, jsonPath.Path, "Integer"));
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
                throw new CsdlParseException(
                    Strings.CsdlJsonParser_UnexpectedJsonValueKind(jsonValue.ValueKind, jsonPath, expectedKind));
            }
        }
    }
}

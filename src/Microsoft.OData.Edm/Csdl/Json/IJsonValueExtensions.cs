//---------------------------------------------------------------------
// <copyright file="IJsonValueExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.OData.Edm.Csdl.Json
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

        public static T ParseRequiredProperty<T>(this JsonObjectValue objValue, JsonPath jsonPath,
            string propertyName,
            Func<IJsonValue, T> propertyParser)
        {
            IJsonValue value;
           if (!objValue.TryGetValue(propertyName, out value))
            {
                throw new Exception();
            }

            return propertyParser(value);
        }

        public static void ProcessProperty(this JsonObjectValue objValue, JsonPath jsonPath,
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



        public static void ProcessItem(this JsonArrayValue arrayValue, JsonPath jsonPath, Action<IJsonValue> itemParser)
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

        public static T OfType<T>(this IJsonValue jsonValue, JsonPath jsonPath)
            where T: class, IJsonValue
        {
            T expectedValue = jsonValue as T;
            if (expectedValue != null)
            {
                return expectedValue;
            }

            // create a default instance to get the expected value type.
            T instance = (T)Activator.CreateInstance(typeof(T));
            throw new CsdlParseException(
                    Strings.CsdlJsonParser_UnexpectedJsonValueKind(jsonValue.ValueKind, jsonPath, instance.ValueKind));
        }

        public static string ParseAsStringPrimitive(this IJsonValue jsonValue)
        {
            return ParseAsStringPrimitive(jsonValue, new JsonPath());
        }

        /// <summary>
        /// Parse <see cref="IJsonValue"/> to a string.
        /// </summary>
        /// <param name="jsonValue">The <see cref="IJsonValue"/> to parse from.</param>
        /// <param name="jsonPath">The JSON path for current JSON value.</param>
        /// <returns>The string.</returns>
        public static string ParseAsStringPrimitive(this IJsonValue jsonValue, JsonPath jsonPath)
        {
            if (jsonValue == null)
            {
                throw new ArgumentNullException("jsonValue");
            }

            jsonValue.ValidateValueKind(JsonValueKind.JPrimitive, jsonPath);

            JsonPrimitiveValue primitiveValue = (JsonPrimitiveValue)jsonValue;
            return primitiveValue.Value as string;
        }

        /// <summary>
        /// Parse <see cref="IJsonValue"/> to a boolean.
        /// </summary>
        /// <param name="jsonValue">The <see cref="IJsonValue"/> to parse from.</param>
        /// <param name="jsonPath">The JSON path for current JSON value.</param>
        /// <returns>The boolean value.</returns>
        public static bool? ParseAsBooleanPrimitive(this IJsonValue jsonValue, JsonPath jsonPath = null)
        {
            if (jsonValue == null)
            {
                throw new ArgumentNullException("jsonValue");
            }

            if (jsonPath == null)
            {
                jsonPath = new JsonPath();
            }
            jsonValue.ValidateValueKind(JsonValueKind.JPrimitive, jsonPath);

            JsonPrimitiveValue primitiveValue = (JsonPrimitiveValue)jsonValue;
            return (bool)primitiveValue.Value;
        }

        //private static int? ParseAsIntegerPrimitive(this IJsonValue jsonValue)
        //{
        //    if (jsonValue.ValueKind != JsonValueKind.JPrimitive)
        //    {
        //        throw new Exception();
        //    }

        //    JsonPrimitiveValue primitiveValue = (JsonPrimitiveValue)jsonValue;
        //    if (primitiveValue.Value == null)
        //    {
        //        return null;
        //    }

        //    if (primitiveValue.Value.GetType() == typeof(int))
        //    {
        //        return (int)primitiveValue.Value;
        //    }

        //    throw new Exception();
        //}

        /// <summary>
        /// Validates that the reader is positioned on the specified node type.
        /// </summary>
        /// <param name="jsonReader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="expectedKind">The expected JSON value kind.</param>
        /// <param name="jsonPath">The JSON path for current JSON value.</param>
        private static void ValidateValueKind(this IJsonValue jsonValue, JsonValueKind expectedKind, JsonPath jsonPath)
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

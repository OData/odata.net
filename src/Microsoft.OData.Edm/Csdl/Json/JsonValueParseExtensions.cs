//---------------------------------------------------------------------
// <copyright file="JsonValueParseExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.OData.Edm.Csdl.Json.Value;

namespace Microsoft.OData.Edm.Csdl.Json
{
    /// <summary>
    /// Provides for the loading and conversion of one or more CSDL XML readers into Entity Data Model.
    /// </summary>
    internal static class JsonValueParseExtensions
    {

        public static IJsonValue GetRequiredProperty(this JsonObjectValue objValue, string propertyName)
        {
            if (objValue != null && objValue.ContainsKey(propertyName))
            {
                return objValue[propertyName];
            }

            // "Miss required property!"
            throw new CsdlParseException();
        }

        public static IJsonValue GetOptionalProperty(this JsonObjectValue objValue, string propertyName)
        {
            if (objValue != null && objValue.ContainsKey(propertyName))
            {
                return objValue[propertyName];
            }

            return null;
        }

        //public static T ParseRequiredProperty<T>(this JsonObjectValue objValue,
        //    string propertyName, JsonPath jsonPath, CsdlSerializerOptions options,
        //    Func<IJsonValue, JsonPath, CsdlSerializerOptions, T> parser)
        //{
        //    IJsonValue jsonValue;
        //    if (objValue.TryGetValue(propertyName, out jsonValue))
        //    {
        //        jsonPath.Push(propertyName);
        //        T ret = parser(jsonValue, jsonPath, options);
        //        jsonPath.Pop();
        //        return ret;
        //    }

        //    throw new Exception();
        //}

        //public static T ParseOptionalProperty<T>(this JsonObjectValue objValue, string propertyName, JsonPath jsonPath, CsdlSerializerOptions options,
        //    Func<IJsonValue, JsonPath, CsdlSerializerOptions, T> parser)
        //{
        //    IJsonValue jsonValue;
        //    if (objValue.TryGetValue(propertyName, out jsonValue))
        //    {
        //        jsonPath.Push(propertyName);
        //        T ret = parser(jsonValue, jsonPath, options);
        //        jsonPath.Pop();
        //        return ret;
        //    }

        //    return default(T);
        //}

        ///// <summary>
        ///// </summary>
        ///// <param name="jsonValue"></param>
        ///// <param name="jsonPath"></param>
        ///// <returns></returns>
        //public static IList<T> ParseObject<T>(this IJsonValue jsonValue,
        //    string name,
        //    JsonPath jsonPath,
        //    CsdlSerializerOptions options,
        //    Func<string, IJsonValue, JsonPath, CsdlSerializerOptions, T> buildItemFunc)
        //{
        //    // The value of $Reference is an object that contains one member per referenced CSDL document.
        //    JsonArrayValue array = jsonValue.ValidateRequiredJsonValue<JsonArrayValue>(jsonPath);

        //    IList<T> includes = new List<T>();

        //    array.ProcessItem(jsonPath, (v) =>
        //    {
        //     //   T item = buildItemFunc(v, jsonPath, options);
        //       // includes.Add(item);
        //    });

        //    return includes;
        //}

        /// <summary>
        /// </summary>
        /// <param name="jsonValue"></param>
        /// <param name="jsonPath"></param>
        /// <returns></returns>
        public static IList<T> ParseArray<T>(this IJsonValue jsonValue, IJsonPath jsonPath,
            Func<IJsonValue, IJsonPath, T> buildItemFunc)
        {
            if (jsonValue == null)
            {
                throw new ArgumentNullException("jsonValue");
            }

            if (jsonPath == null)
            {
                throw new ArgumentNullException("jsonPath");
            }

            if (jsonValue.ValueKind != JsonValueKind.JArray)
            {
                return null;
            }

            JsonArrayValue array = (JsonArrayValue)jsonValue;
            IList<T> includes = new List<T>();

            int index = 0;
            foreach (var item in array)
            {
                // The property value is either primitive, array or another object,
                // It could not be a null.
                // for null, it's wrappered in JsonPrimitiveValue.
                Debug.Assert(item != null);

                jsonPath.Push(index++);

                T builtItem = buildItemFunc(item, jsonPath);

                includes.Add(builtItem);

                jsonPath.Pop();
            }

            return includes;
        }

        /// <summary>
        /// </summary>
        /// <param name="jsonValue"></param>
        /// <param name="jsonPath"></param>
        /// <returns></returns>
        public static IList<T> ParseArray<T>(this IJsonValue jsonValue,
            JsonPath jsonPath,
            CsdlSerializerOptions options,
            Func<IJsonValue, JsonPath, CsdlSerializerOptions, T> buildItemFunc)
        {
            // The value of $Reference is an object that contains one member per referenced CSDL document.
            JsonArrayValue array = jsonValue.ValidateRequiredJsonValue<JsonArrayValue>(jsonPath);

            IList<T> includes = new List<T>();

            array.ProcessItem(jsonPath, (v) =>
            {
                T item = buildItemFunc(v, jsonPath, options);
                includes.Add(item);
            });

            return includes;
        }

        internal static void ReportUnknownMember(this IJsonValue propertyValue, IJsonPath jsonPath, CsdlSerializerOptions options)
        {
            Debug.Assert(propertyValue != null);
            Debug.Assert(jsonPath != null);
            Debug.Assert(options != null);

            if (options.IgnoreUnexpectedMembers)
            {
                return;
            }

            throw new CsdlParseException(Strings.CsdlJsonParser_UnexpectedJsonMember(jsonPath, propertyValue.ValueKind));
        }

        internal static T ValidateRequiredJsonValue<T>(this IJsonValue jsonValue, IJsonPath jsonPath)
            where T : class, IJsonValue
        {
            if (jsonValue == null)
            {
                throw new ArgumentNullException("jsonValue");
            }

            if (jsonPath == null)
            {
                throw new ArgumentNullException("jsonPath");
            }

            T expectedValue = jsonValue as T;
            if (expectedValue != null)
            {
                return expectedValue;
            }

            // Create a default instance to get the expected value type.
            T instance = (T)Activator.CreateInstance(typeof(T));
            throw new CsdlParseException(
                    Strings.CsdlJsonParser_UnexpectedJsonValueKind(jsonValue.ValueKind, jsonPath, instance.ValueKind));
        }
    }
}

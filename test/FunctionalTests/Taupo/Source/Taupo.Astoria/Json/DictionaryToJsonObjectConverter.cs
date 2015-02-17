//---------------------------------------------------------------------
// <copyright file="DictionaryToJsonObjectConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Json
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Default implementation of the contract to convert from a dictionary into a json object
    /// </summary>
    [ImplementationName(typeof(IDictionaryToJsonObjectConverter), "Default")]
    public class DictionaryToJsonObjectConverter : IDictionaryToJsonObjectConverter
    {
        /// <summary>
        /// Converts the specified dictionary into a json object.
        /// </summary>
        /// <param name="dictionary">The dictionary to convert.</param>
        /// <returns>
        /// The converted object
        /// </returns>
        public JsonObject Convert(IDictionary<string, object> dictionary)
        {
            ExceptionUtilities.CheckArgumentNotNull(dictionary, "dictionary");

            var jsonObject = new JsonObject();
            foreach (var property in dictionary)
            {
                jsonObject.Add(new JsonProperty(property.Key, this.Convert(property.Value)));
            }

            return jsonObject;
        }

        /// <summary>
        /// Converts the specified json object into a dictionary.
        /// </summary>
        /// <param name="jsonObject">The json object to convert.</param>
        /// <returns>The converted dictionary</returns>
        public IDictionary<string, object> Convert(JsonObject jsonObject)
        {
            ExceptionUtilities.CheckArgumentNotNull(jsonObject, "jsonObject");
            return (IDictionary<string, object>)new JsonToDictionaryConvertingVisitor().Visit(jsonObject);
        }

        internal JsonValue Convert(object value)
        {
            if (value == null)
            {
                return new JsonPrimitiveValue(null);
            }

            var type = value.GetType();

            if (typeof(IDictionary<string, object>).IsAssignableFrom(type))
            {
                return this.Convert((IDictionary<string, object>)value);
            }

            if (typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string) && type != typeof(byte[]))
            {
                return this.Convert((IEnumerable)value);
            }

            return new JsonPrimitiveValue(value);
        }

        internal JsonArray Convert(IEnumerable enumerable)
        {
            ExceptionUtilities.CheckArgumentNotNull(enumerable, "enumerable");

            var jsonArray = new JsonArray();
            foreach (var element in enumerable)
            {
                jsonArray.Add(this.Convert(element));
            }

            return jsonArray;
        }
        
        internal class JsonToDictionaryConvertingVisitor : IJsonValueVisitor<object>
        {
            /// <summary>
            /// Visits the specified json object.
            /// </summary>
            /// <param name="jsonObject">The json object.</param>
            /// <returns>The converted object</returns>
            public object Visit(JsonObject jsonObject)
            {
                ExceptionUtilities.CheckArgumentNotNull(jsonObject, "jsonObject");
                return jsonObject.Properties.ToDictionary(p => p.Name, p => p.Value.Accept(this));
            }

            /// <summary>
            /// Visits the specified json array.
            /// </summary>
            /// <param name="jsonArray">The json array.</param>
            /// <returns>The converted object</returns>
            public object Visit(JsonArray jsonArray)
            {
                ExceptionUtilities.CheckArgumentNotNull(jsonArray, "jsonArray");
                return jsonArray.Elements.Select(e => e.Accept(this));
            }

            /// <summary>
            /// Visits the specified primitive.
            /// </summary>
            /// <param name="primitive">The primitive.</param>
            /// <returns>The converted object</returns>
            public object Visit(JsonPrimitiveValue primitive)
            {
                ExceptionUtilities.CheckArgumentNotNull(primitive, "primitive");
                return primitive.Value;
            }

            /// <summary>
            /// Visits the specified property.
            /// </summary>
            /// <param name="jsonProperty">The property.</param>
            /// <returns>The converted object</returns>
            public object Visit(JsonProperty jsonProperty)
            {
                throw new TaupoNotSupportedException("Visiting properties is not supported, should go through the visit method for json objects instead");
            }
        }
    }
}
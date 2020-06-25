//---------------------------------------------------------------------
// <copyright file="JsonElementExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if NETSTANDARD2_0
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using Microsoft.OData.Edm.Csdl.Parsing;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm.Csdl
{
    internal static class JsonElementExtensions
    {
        public static T ProcessRequiredProperty<T>(this JsonElement element,
            string propertyName,
            JsonParserContext context,
            Func<JsonElement, JsonParserContext, T> propertyParser)
        {
            Debug.Assert(element.ValueKind == JsonValueKind.Object);

            T value = default(T);
            if (element.TryGetProperty(propertyName, out JsonElement propertyValue))
            {
                context.EnterScope(propertyName);

                value = propertyParser(propertyValue, context);

                context.LeaveScope(propertyName);
            }
            else
            {
                context.ReportError(EdmErrorCode.MissingRequiredProperty,
                    Strings.CsdlJsonParser_MissingRequiredPropertyInObject(propertyName, context.Path));
            }

            return value;
        }

        public static T ProcessItem<T>(this JsonElement element, int index, JsonParserContext context,
            Func<JsonElement, JsonParserContext, T> parser)
        {
            Debug.Assert(element.ValueKind == JsonValueKind.Array);

            context.EnterScope(index);

            T ret = parser(element[index], context);

            context.LeaveScope();

            return ret;
        }

        public static IList<T> ProcessArrayProperty<T>(this JsonElement element, string propertyName, JsonParserContext context,
            Func<JsonElement, JsonParserContext, T> itemParser)
        {
            Debug.Assert(context != null);
            Debug.Assert(element.ValueKind == JsonValueKind.Array);

            context.EnterScope(propertyName);

            IList<T> includes = new List<T>();
            int index = 0;
            foreach (var item in element.EnumerateArray())
            {
                context.EnterScope(index);

                T builtItem = itemParser(item, context);

                includes.Add(builtItem);

                context.LeaveScope(index);

                index++;
            }

            context.LeaveScope(propertyName);
            return includes;
        }

        public static T ProcessProperty<T>(this JsonElement propertyValue, string propertyName, JsonParserContext context,
            Func<JsonElement, JsonParserContext, T> valueParser)
        {
            Debug.Assert(context != null);

            context.EnterScope(propertyName);

            T value = valueParser(propertyValue, context);

            context.LeaveScope(propertyName);
            return value;
        }

        public static void ParseAsObject(this JsonElement element, JsonParserContext context,
           Action<string, JsonElement> propertyParser)
        {
            Debug.Assert(context != null);
            Debug.Assert(element.ValueKind == JsonValueKind.Object);

            foreach (var propertyItem in element.EnumerateObject())
            {
                context.EnterScope(propertyItem.Name);

                propertyParser(propertyItem.Name, propertyItem.Value);

                context.LeaveScope(propertyItem.Name);
            }
        }

        public static IList<T> ParseAsArray<T>(this JsonElement element, JsonParserContext context,
            Func<JsonElement, JsonParserContext, T> itemParser)
        {
            Debug.Assert(context != null);
            Debug.Assert(element.ValueKind == JsonValueKind.Array);

            IList<T> includes = new List<T>();
            int index = 0;
            foreach (var item in element.EnumerateArray())
            {
                context.EnterScope(index);

                T builtItem = itemParser(item, context);

                includes.Add(builtItem);

                context.LeaveScope(index);

                index++;
            }

            return includes;
        }

        public static string ParseAsString(this JsonElement element, JsonParserContext context)
        {
            if (element.ValueKind == JsonValueKind.String || element.ValueKind == JsonValueKind.Null)
            {
                return element.GetString();
            }

            context.ReportError(EdmErrorCode.UnexpectedValueKind,
                Strings.CsdlJsonParser_UnexpectedJsonValueKind(element.ValueKind, context.Path, "String"));
            return null;
        }

        public static bool? ParseAsBoolean(this JsonElement element, JsonParserContext context)
        {
            if (element.ValueKind == JsonValueKind.True || element.ValueKind == JsonValueKind.False)
            {
                return element.GetBoolean();
            }

            context.ReportError(EdmErrorCode.UnexpectedValueKind,
                Strings.CsdlJsonParser_UnexpectedJsonValueKind(element.ValueKind, context.Path, "Boolean"));

            return null;
        }

        public static int? ParseAsInteger(this JsonElement element)
        {
            if (element.TryGetInt32(out int value))
            {
                return value;
            }

            return null;
        }

        public static int? ParseAsInteger(this JsonElement element, JsonParserContext context)
        {
            if (element.ValueKind == JsonValueKind.Number)
            {
                return element.GetInt32();
            }

            context.ReportError(EdmErrorCode.UnexpectedValueKind,
                Strings.CsdlJsonParser_UnexpectedJsonValueKind(element.ValueKind, context.Path, "Integer"));
            return default(int?);
        }

        public static string ParseAsJson(this JsonElement element)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream))
                {
                    element.WriteTo(writer);
                    writer.Flush();
                }

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        public static T ParseOptionalProperty<T>(this JsonElement element, string propertyName, JsonParserContext context,
            Func<JsonElement, JsonParserContext, T> parser)
        {
            if (element.ValueKind == JsonValueKind.Object &&
                element.TryGetProperty(propertyName, out JsonElement propertyValue))
            {
                context.EnterScope(propertyName);

                T ret = parser(propertyValue, context);

                context.LeaveScope(propertyName);
                return ret;
            }

            return default(T);
        }

        internal static bool ValidateValueKind(this JsonElement element, JsonValueKind expectedKind, JsonParserContext context)
        {
            Debug.Assert(context != null);

            if (element.ValueKind != expectedKind)
            {
                context.ReportError(EdmErrorCode.UnexpectedValueKind,
                    Strings.CsdlJsonParser_UnexpectedJsonValueKind(element.ValueKind, context.Path, expectedKind));
                return false;
            }

            return true;
        }

        internal static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (enumerable != null)
            {
                foreach (T item in enumerable)
                {
                    action(item);
                }
            }
        }
    }
}
#endif
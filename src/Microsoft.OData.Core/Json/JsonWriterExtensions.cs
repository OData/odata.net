//---------------------------------------------------------------------
// <copyright file="JsonWriterExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    #region Namespaces
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;
    using ODataErrorStrings = Microsoft.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// Extension methods for the JSON writer.
    /// </summary>
    internal static class JsonWriterExtensions
    {
        /// <summary>
        /// Writes the json object value to the <paramref name="jsonWriter"/>.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="jsonObjectValue">Writes the given json object value to the underlying json writer.</param>
        /// <param name="injectPropertyAction">Called when the top-level object is started to possibly inject first property into the object.</param>
        internal static void WriteJsonObjectValue(this IJsonWriter jsonWriter, IDictionary<string, object> jsonObjectValue, Action<IJsonWriter> injectPropertyAction)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(jsonObjectValue != null, "jsonObjectValue != null");

            jsonWriter.StartObjectScope();

            if (injectPropertyAction != null)
            {
                injectPropertyAction(jsonWriter);
            }

            foreach (KeyValuePair<string, object> property in jsonObjectValue)
            {
                jsonWriter.WriteName(property.Key);
                jsonWriter.WriteJsonValue(property.Value);
            }

            jsonWriter.EndObjectScope();
        }

        /// <summary>
        /// Writes a primitive value.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value to write.</param>
        internal static void WritePrimitiveValue(this IJsonWriter jsonWriter, object value)
        {
            if (value is bool)
            {
                jsonWriter.WriteValue((bool)value);
                return;
            }

            if (value is byte)
            {
                jsonWriter.WriteValue((byte)value);
                return;
            }

            if (value is decimal)
            {
                jsonWriter.WriteValue((decimal)value);
                return;
            }

            if (value is double)
            {
                jsonWriter.WriteValue((double)value);
                return;
            }

            if (value is Int16)
            {
                jsonWriter.WriteValue((Int16)value);
                return;
            }

            if (value is Int32)
            {
                jsonWriter.WriteValue((Int32)value);
                return;
            }

            if (value is Int64)
            {
                jsonWriter.WriteValue((Int64)value);
                return;
            }

            if (value is sbyte)
            {
                jsonWriter.WriteValue((sbyte)value);
                return;
            }

            if (value is Single)
            {
                jsonWriter.WriteValue((Single)value);
                return;
            }

            var str = value as string;
            if (str != null)
            {
                jsonWriter.WriteValue(str);
                return;
            }

            byte[] valueAsByteArray = value as byte[];
            if (valueAsByteArray != null)
            {
                jsonWriter.WriteValue(valueAsByteArray);
                return;
            }

            if (value is DateTimeOffset)
            {
                jsonWriter.WriteValue((DateTimeOffset)value);
                return;
            }

            if (value is Guid)
            {
                jsonWriter.WriteValue((Guid)value);
                return;
            }

            if (value is TimeSpan)
            {
                jsonWriter.WriteValue((TimeSpan)value);
                return;
            }

            if (value is Date)
            {
                jsonWriter.WriteValue((Date)value);
                return;
            }

            if (value is TimeOfDay)
            {
                jsonWriter.WriteValue((TimeOfDay)value);
                return;
            }

            if (value is Dictionary<string, object>)
            {
                jsonWriter.WriteJsonObjectValue((Dictionary<string, object>)value, null);
                return;
            }

            throw new ODataException(ODataErrorStrings.ODataJsonWriter_UnsupportedValueType(value.GetType().FullName));
        }

        /// <summary>
        /// Writes the ODataValue (primitive, collection or resource value) to the underlying json writer.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="odataValue">value to write.</param>
        internal static void WriteODataValue(this IJsonWriter jsonWriter, ODataValue odataValue)
        {
            if (odataValue == null || odataValue is ODataNullValue)
            {
                jsonWriter.WriteValue((string)null);
                return;
            }

            object objectValue = odataValue.FromODataValue();
            if (EdmLibraryExtensions.IsPrimitiveType(objectValue.GetType()))
            {
                jsonWriter.WritePrimitiveValue(objectValue);
                return;
            }

            ODataResourceValue resourceValue = odataValue as ODataResourceValue;
            if (resourceValue != null)
            {
                jsonWriter.StartObjectScope();

                foreach (ODataProperty property in resourceValue.Properties)
                {
                    jsonWriter.WriteName(property.Name);
                    jsonWriter.WriteODataValue(property.ODataValue);
                }

                jsonWriter.EndObjectScope();
                return;
            }

            ODataCollectionValue collectionValue = odataValue as ODataCollectionValue;
            if (collectionValue != null)
            {
                jsonWriter.StartArrayScope();

                foreach (object item in collectionValue.Items)
                {
                    // Will not be able to accurately serialize complex objects unless they are ODataValues.
                    ODataValue collectionItem = item as ODataValue;
                    if (item != null)
                    {
                        jsonWriter.WriteODataValue(collectionItem);
                    }
                    else
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonWriter_UnsupportedValueInCollection);
                    }
                }

                jsonWriter.EndArrayScope();

                return;
            }

            throw new ODataException(
                ODataErrorStrings.ODataJsonWriter_UnsupportedValueType(odataValue.GetType().FullName));
        }

        /// <summary>
        /// Writes the json array value.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="arrayValue">Writes the json array value to the underlying json writer.</param>
        private static void WriteJsonArrayValue(this IJsonWriter jsonWriter, IEnumerable arrayValue)
        {
            Debug.Assert(arrayValue != null, "arrayValue != null");

            jsonWriter.StartArrayScope();

            foreach (object element in arrayValue)
            {
                jsonWriter.WriteJsonValue(element);
            }

            jsonWriter.EndArrayScope();
        }

        /// <summary>
        /// Writes the json value (primitive, IDictionary or IEnumerable) to the underlying json writer.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="propertyValue">value to write.</param>
        private static void WriteJsonValue(this IJsonWriter jsonWriter, object propertyValue)
        {
            if (propertyValue == null)
            {
                jsonWriter.WriteValue((string)null);
            }
            else if (EdmLibraryExtensions.IsPrimitiveType(propertyValue.GetType()))
            {
                jsonWriter.WritePrimitiveValue(propertyValue);
            }
            else
            {
                IDictionary<string, object> objectValue = propertyValue as IDictionary<string, object>;
                if (objectValue != null)
                {
                    jsonWriter.WriteJsonObjectValue(objectValue, null /*typeName */);
                }
                else
                {
                    IEnumerable arrayValue = propertyValue as IEnumerable;
                    Debug.Assert(arrayValue != null, "arrayValue != null");
                    jsonWriter.WriteJsonArrayValue(arrayValue);
                }
            }
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="JsonWriterExtensions.Async.cs" company="Microsoft">
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
    using System.Threading.Tasks;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;
    using ODataErrorStrings = Microsoft.OData.Strings;
    #endregion Namespaces

    internal static partial class JsonWriterExtensions
    {
        /// <summary>
        /// Asynchronously writes the json object value to the <paramref name="jsonWriter"/>.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="jsonObjectValue">Writes the given json object value to the underlying json writer.</param>
        /// <param name="injectPropertyDelegate">Called when the top-level object is started to possibly inject first property into the object.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        internal static async Task WriteJsonObjectValueAsync(
            this IJsonWriter jsonWriter,
            IDictionary<string, object> jsonObjectValue,
            Func<IJsonWriter, Task> injectPropertyDelegate)
        {
            Debug.Assert(jsonWriter != null, "jsonWriter != null");
            Debug.Assert(jsonObjectValue != null, "jsonObjectValue != null");

            await jsonWriter.StartObjectScopeAsync().ConfigureAwait(false);

            if (injectPropertyDelegate != null)
            {
                await injectPropertyDelegate(jsonWriter).ConfigureAwait(false);
            }

            foreach (KeyValuePair<string, object> property in jsonObjectValue)
            {
                await jsonWriter.WriteNameAsync(property.Key).ConfigureAwait(false);
                await jsonWriter.WriteJsonValueAsync(property.Value).ConfigureAwait(false);
            }

            await jsonWriter.EndObjectScopeAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes a primitive value.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        internal static Task WritePrimitiveValueAsync(this IJsonWriter jsonWriter, object value)
        {
            if (value is bool)
            {
                return jsonWriter.WriteValueAsync((bool)value);
            }

            if (value is byte)
            {
                return jsonWriter.WriteValueAsync((byte)value);
            }

            if (value is decimal)
            {
                return jsonWriter.WriteValueAsync((decimal)value);
            }

            if (value is double)
            {
                return jsonWriter.WriteValueAsync((double)value);
            }

            if (value is short)
            {
                return jsonWriter.WriteValueAsync((short)value);
            }

            if (value is int)
            {
                return jsonWriter.WriteValueAsync((int)value);
            }

            if (value is long)
            {
                return jsonWriter.WriteValueAsync((long)value);
            }

            if (value is sbyte)
            {
                return jsonWriter.WriteValueAsync((sbyte)value);
            }

            if (value is float)
            {
                return jsonWriter.WriteValueAsync((float)value);
            }

            var str = value as string;
            if (str != null)
            {
                return jsonWriter.WriteValueAsync(str);
            }

            byte[] valueAsByteArray = value as byte[];
            if (valueAsByteArray != null)
            {
                return jsonWriter.WriteValueAsync(valueAsByteArray);
            }

            if (value is DateTimeOffset)
            {
                return jsonWriter.WriteValueAsync((DateTimeOffset)value);
            }

            if (value is Guid)
            {
                return jsonWriter.WriteValueAsync((Guid)value);
            }

            if (value is TimeSpan)
            {
                return jsonWriter.WriteValueAsync((TimeSpan)value);
            }

            if (value is Date)
            {
                return jsonWriter.WriteValueAsync((Date)value);
            }

            if (value is DateOnly dateOnly)
            {
                // will call 'WriteValueAsync(Date)' version implicitly
                return jsonWriter.WriteValueAsync(dateOnly);
            }

            if (value is TimeOfDay)
            {
                return jsonWriter.WriteValueAsync((TimeOfDay)value);
            }

            if (value is TimeOnly timeOnly)
            {
                // will call 'WriteValueAsync(TimeOfDay)' version implicitly
                return jsonWriter.WriteValueAsync(timeOnly);
            }

            return TaskUtils.GetFaultedTask(
                new ODataException(ODataErrorStrings.ODataJsonWriter_UnsupportedValueType(value.GetType().FullName)));
        }

        /// <summary>
        /// Asynchronously writes the ODataValue (primitive, collection or resource value) to the underlying json writer.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="odataValue">value to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        internal static Task WriteODataValueAsync(this IJsonWriter jsonWriter, ODataValue odataValue)
        {
            if (odataValue == null || odataValue is ODataNullValue)
            {
                return jsonWriter.WriteValueAsync((string)null);
            }

            object objectValue = odataValue.FromODataValue();
            if (EdmLibraryExtensions.IsPrimitiveType(objectValue.GetType()))
            {
                return jsonWriter.WritePrimitiveValueAsync(objectValue);
            }

            // The types of OData values handled by this method are mutually exclusive
            // We're using local functions to circumvent the async state machine overhead

            if (odataValue is ODataResourceValue resourceValue)
            {
                return WriteODataResourceValueAsync(jsonWriter, resourceValue);

                async Task WriteODataResourceValueAsync(
                    IJsonWriter innerJsonWriter,
                    ODataResourceValue innerResourceValue)
                {
                    await innerJsonWriter.StartObjectScopeAsync().ConfigureAwait(false);

                    foreach (ODataProperty property in innerResourceValue.Properties)
                    {
                        await innerJsonWriter.WriteNameAsync(property.Name).ConfigureAwait(false);
                        await innerJsonWriter.WriteODataValueAsync(property.ODataValue).ConfigureAwait(false);
                    }

                    await innerJsonWriter.EndObjectScopeAsync().ConfigureAwait(false);
                }
            }

            if (odataValue is ODataCollectionValue collectionValue)
            {
                return WriteODataCollectionValueAsync(jsonWriter, collectionValue);

                async Task WriteODataCollectionValueAsync(
                    IJsonWriter innerJsonWriter,
                    ODataCollectionValue innerCollectionValue)
                {
                    await innerJsonWriter.StartArrayScopeAsync().ConfigureAwait(false);

                    foreach (object item in innerCollectionValue.Items)
                    {
                        // Will not be able to accurately serialize complex objects unless they are ODataValues.
                        ODataValue collectionItem = item as ODataValue;
                        if (item != null)
                        {
                            await innerJsonWriter.WriteODataValueAsync(collectionItem).ConfigureAwait(false);
                        }
                        else
                        {
                            throw new ODataException(ODataErrorStrings.ODataJsonWriter_UnsupportedValueInCollection);
                        }
                    }

                    await innerJsonWriter.EndArrayScopeAsync().ConfigureAwait(false);
                }
            }

            return TaskUtils.GetFaultedTask(
                new ODataException(ODataErrorStrings.ODataJsonWriter_UnsupportedValueType(odataValue.GetType().FullName)));
        }

        /// <summary>
        /// Asynchronously writes the json array value.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="arrayValue">Writes the json array value to the underlying json writer.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private static async Task WriteJsonArrayValueAsync(this IJsonWriter jsonWriter, IEnumerable arrayValue)
        {
            Debug.Assert(arrayValue != null, "arrayValue != null");

            await jsonWriter.StartArrayScopeAsync().ConfigureAwait(false);

            foreach (object element in arrayValue)
            {
                await jsonWriter.WriteJsonValueAsync(element).ConfigureAwait(false);
            }

            await jsonWriter.EndArrayScopeAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously writes the json value (primitive, IDictionary or IEnumerable) to the underlying json writer.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="propertyValue">value to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private static Task WriteJsonValueAsync(this IJsonWriter jsonWriter, object propertyValue)
        {
            if (propertyValue == null)
            {
                return jsonWriter.WriteValueAsync((string)null);
            }
            else if (EdmLibraryExtensions.IsPrimitiveType(propertyValue.GetType()))
            {
                return jsonWriter.WritePrimitiveValueAsync(propertyValue);
            }
            else
            {
                if (propertyValue is IDictionary<string, object> objectValue)
                {
                    return jsonWriter.WriteJsonObjectValueAsync(objectValue, injectPropertyDelegate: null);
                }
                else
                {
                    IEnumerable arrayValue = propertyValue as IEnumerable;
                    Debug.Assert(arrayValue != null, "arrayValue != null");
                    return jsonWriter.WriteJsonArrayValueAsync(arrayValue);
                }
            }
        }
    }
}

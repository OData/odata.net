//---------------------------------------------------------------------
// <copyright file="JsonWriterAsyncExtensions.cs" company="Microsoft">
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

    /// <summary>
    /// Extension methods for the asynchronous JSON writer.
    /// </summary>
    internal static class JsonWriterAsyncExtensions
    {
        /// <summary>
        /// Asynchronously writes the json object value to the <paramref name="jsonWriter"/>.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="jsonObjectValue">Writes the given json object value to the underlying json writer.</param>
        /// <param name="injectPropertyDelegate">Called when the top-level object is started to possibly inject first property into the object.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        internal static async Task WriteJsonObjectValueAsync(
            this IJsonWriterAsync jsonWriter,
            IDictionary<string, object> jsonObjectValue,
            Func<IJsonWriterAsync, Task> injectPropertyDelegate)
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
        internal static async Task WritePrimitiveValueAsync(this IJsonWriterAsync jsonWriter, object value)
        {
            if (value is bool)
            {
                await jsonWriter.WriteValueAsync((bool)value).ConfigureAwait(false);
                return;
            }

            if (value is byte)
            {
                await jsonWriter.WriteValueAsync((byte)value).ConfigureAwait(false);
                return;
            }

            if (value is decimal)
            {
                await jsonWriter.WriteValueAsync((decimal)value).ConfigureAwait(false);
                return;
            }

            if (value is double)
            {
                await jsonWriter.WriteValueAsync((double)value).ConfigureAwait(false);
                return;
            }

            if (value is Int16)
            {
                await jsonWriter.WriteValueAsync((Int16)value).ConfigureAwait(false);
                return;
            }

            if (value is Int32)
            {
                await jsonWriter.WriteValueAsync((Int32)value).ConfigureAwait(false);
                return;
            }

            if (value is Int64)
            {
                await jsonWriter.WriteValueAsync((Int64)value).ConfigureAwait(false);
                return;
            }

            if (value is sbyte)
            {
                await jsonWriter.WriteValueAsync((sbyte)value).ConfigureAwait(false);
                return;
            }

            if (value is Single)
            {
                await jsonWriter.WriteValueAsync((Single)value).ConfigureAwait(false);
                return;
            }

            var str = value as string;
            if (str != null)
            {
                await jsonWriter.WriteValueAsync(str).ConfigureAwait(false);
                return;
            }

            byte[] valueAsByteArray = value as byte[];
            if (valueAsByteArray != null)
            {
                await jsonWriter.WriteValueAsync(valueAsByteArray).ConfigureAwait(false);
                return;
            }

            if (value is DateTimeOffset)
            {
                await jsonWriter.WriteValueAsync((DateTimeOffset)value).ConfigureAwait(false);
                return;
            }

            if (value is Guid)
            {
                await jsonWriter.WriteValueAsync((Guid)value).ConfigureAwait(false);
                return;
            }

            if (value is TimeSpan)
            {
                await jsonWriter.WriteValueAsync((TimeSpan)value).ConfigureAwait(false);
                return;
            }

            if (value is Date)
            {
                await jsonWriter.WriteValueAsync((Date)value).ConfigureAwait(false);
                return;
            }

            if (value is TimeOfDay)
            {
                await jsonWriter.WriteValueAsync((TimeOfDay)value).ConfigureAwait(false);
                return;
            }

            throw new ODataException(ODataErrorStrings.ODataJsonWriter_UnsupportedValueType(value.GetType().FullName));
        }

        /// <summary>
        /// Asynchronously writes the ODataValue (primitive, collection or resource value) to the underlying json writer.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="odataValue">value to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        internal static async Task WriteODataValueAsync(this IJsonWriterAsync jsonWriter, ODataValue odataValue)
        {
            if (odataValue == null || odataValue is ODataNullValue)
            {
                await jsonWriter.WriteValueAsync((string)null).ConfigureAwait(false);
                return;
            }

            object objectValue = odataValue.FromODataValue();
            if (EdmLibraryExtensions.IsPrimitiveType(objectValue.GetType()))
            {
                await jsonWriter.WritePrimitiveValueAsync(objectValue).ConfigureAwait(false);
                return;
            }

            ODataResourceValue resourceValue = odataValue as ODataResourceValue;
            if (resourceValue != null)
            {
                await jsonWriter.StartObjectScopeAsync().ConfigureAwait(false);

                foreach (ODataProperty property in resourceValue.Properties)
                {
                    await jsonWriter.WriteNameAsync(property.Name).ConfigureAwait(false);
                    await jsonWriter.WriteODataValueAsync(property.ODataValue).ConfigureAwait(false);
                }

                await jsonWriter.EndObjectScopeAsync().ConfigureAwait(false);
                return;
            }

            ODataCollectionValue collectionValue = odataValue as ODataCollectionValue;
            if (collectionValue != null)
            {
                await jsonWriter.StartArrayScopeAsync().ConfigureAwait(false);

                foreach (object item in collectionValue.Items)
                {
                    // Will not be able to accurately serialize complex objects unless they are ODataValues.
                    ODataValue collectionItem = item as ODataValue;
                    if (item != null)
                    {
                        await jsonWriter.WriteODataValueAsync(collectionItem).ConfigureAwait(false);
                    }
                    else
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonWriter_UnsupportedValueInCollection);
                    }
                }

                await jsonWriter.EndArrayScopeAsync().ConfigureAwait(false);

                return;
            }

            throw new ODataException(
                ODataErrorStrings.ODataJsonWriter_UnsupportedValueType(odataValue.GetType().FullName));
        }

        /// <summary>
        /// Asynchronously writes the json array value.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="arrayValue">Writes the json array value to the underlying json writer.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private static async Task WriteJsonArrayValueAsync(this IJsonWriterAsync jsonWriter, IEnumerable arrayValue)
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
        private static async Task WriteJsonValueAsync(this IJsonWriterAsync jsonWriter, object propertyValue)
        {
            if (propertyValue == null)
            {
                await jsonWriter.WriteValueAsync((string)null).ConfigureAwait(false);
            }
            else if (EdmLibraryExtensions.IsPrimitiveType(propertyValue.GetType()))
            {
                await jsonWriter.WritePrimitiveValueAsync(propertyValue).ConfigureAwait(false);
            }
            else
            {
                IDictionary<string, object> objectValue = propertyValue as IDictionary<string, object>;
                if (objectValue != null)
                {
                    await jsonWriter.WriteJsonObjectValueAsync(objectValue, null /*typeName */).ConfigureAwait(false);
                }
                else
                {
                    IEnumerable arrayValue = propertyValue as IEnumerable;
                    Debug.Assert(arrayValue != null, "arrayValue != null");
                    await jsonWriter.WriteJsonArrayValueAsync(arrayValue).ConfigureAwait(false);
                }
            }
        }
    }
}

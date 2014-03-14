//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData.Json
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Collections.Generic;
    using System.Collections;
    using Microsoft.Data.OData.Metadata;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
    using ODataPlatformHelper = Microsoft.Data.OData.PlatformHelper;
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
        /// <param name="odataVersion">The OData protocol version to be used for writing payloads.</param>
        internal static void WriteJsonObjectValue(this IJsonWriter jsonWriter, IDictionary<string, object> jsonObjectValue, Action<IJsonWriter> injectPropertyAction, ODataVersion odataVersion)
        {
            DebugUtils.CheckNoExternalCallers();
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
                jsonWriter.WriteJsonValue(property.Value, odataVersion);
            }

            jsonWriter.EndObjectScope();
        }

        /// <summary>
        /// Writes a primitive value.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="odataVersion">The OData protocol version to be used for writing payloads.</param>
        internal static void WritePrimitiveValue(this IJsonWriter jsonWriter, object value, ODataVersion odataVersion)
        {
            DebugUtils.CheckNoExternalCallers();

            TypeCode typeCode = ODataPlatformHelper.GetTypeCode(value.GetType());
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    jsonWriter.WriteValue((bool)value);
                    break;

                case TypeCode.Byte:
                    jsonWriter.WriteValue((byte)value);
                    break;

                case TypeCode.DateTime:
                    jsonWriter.WriteValue((DateTime)value, odataVersion);
                    break;

                case TypeCode.Decimal:
                    jsonWriter.WriteValue((decimal)value);
                    break;

                case TypeCode.Double:
                    jsonWriter.WriteValue((double)value);
                    break;

                case TypeCode.Int16:
                    jsonWriter.WriteValue((Int16)value);
                    break;

                case TypeCode.Int32:
                    jsonWriter.WriteValue((Int32)value);
                    break;

                case TypeCode.Int64:
                    jsonWriter.WriteValue((Int64)value);
                    break;

                case TypeCode.SByte:
                    jsonWriter.WriteValue((sbyte)value);
                    break;

                case TypeCode.Single:
                    jsonWriter.WriteValue((Single)value);
                    break;

                case TypeCode.String:
                    jsonWriter.WriteValue((string)value);
                    break;

                default:
                    {
                        byte[] valueAsByteArray = value as byte[];
                        if (valueAsByteArray != null)
                        {
                            jsonWriter.WriteValue(Convert.ToBase64String(valueAsByteArray));
                            break;
                        }

                        if (value is DateTimeOffset)
                        {
                            jsonWriter.WriteValue((DateTimeOffset)value, odataVersion);
                            break;
                        }

                        if (value is Guid)
                        {
                            jsonWriter.WriteValue((Guid)value);
                            break;
                        }

                        if (value is TimeSpan)
                        {
                            jsonWriter.WriteValue((TimeSpan)value);
                            break;
                        }
                    }

                    throw new ODataException(ODataErrorStrings.ODataJsonWriter_UnsupportedValueType(value.GetType().FullName));
            }
        }

        /// <summary>
        /// Writes the json array value.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="arrayValue">Writes the json array value to the underlying json writer.</param>
        /// <param name="odataVersion">The OData protocol version to be used for writing payloads.</param>
        private static void WriteJsonArrayValue(this IJsonWriter jsonWriter, IEnumerable arrayValue, ODataVersion odataVersion)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(arrayValue != null, "arrayValue != null");

            jsonWriter.StartArrayScope();

            foreach (object element in arrayValue)
            {
                jsonWriter.WriteJsonValue(element, odataVersion);
            }

            jsonWriter.EndArrayScope();
        }

        /// <summary>
        /// Writes the json value (primitive, IDictionary or IEnumerable) to the underlying json writer.
        /// </summary>
        /// <param name="jsonWriter">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="propertyValue">value to write.</param>
        /// <param name="odataVersion">The OData protocol version to be used for writing payloads.</param>
        private static void WriteJsonValue(this IJsonWriter jsonWriter, object propertyValue, ODataVersion odataVersion)
        {
            if (propertyValue == null)
            {
                jsonWriter.WriteValue((string)null);
            }
            else if (EdmLibraryExtensions.IsPrimitiveType(propertyValue.GetType()))
            {
                jsonWriter.WritePrimitiveValue(propertyValue, odataVersion);
            }
            else
            {
                IDictionary<string, object> objectValue = propertyValue as IDictionary<string, object>;
                if (objectValue != null)
                {
                    jsonWriter.WriteJsonObjectValue(objectValue, null /*typeName */, odataVersion);
                }
                else
                {
                    IEnumerable arrayValue = propertyValue as IEnumerable;
                    Debug.Assert(arrayValue != null, "arrayValue != null");
                    jsonWriter.WriteJsonArrayValue(arrayValue, odataVersion);
                }
            }
        }
    }
}

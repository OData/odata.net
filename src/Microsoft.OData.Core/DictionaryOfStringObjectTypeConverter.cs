//---------------------------------------------------------------------
// <copyright file="DictionaryOfStringObjectTypeConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;

namespace Microsoft.OData
{
    /// <summary>
    /// Handles serialization and deserialization for types derived from Geography.
    /// </summary>
    internal sealed class DictionaryOfStringObjectTypeConverter : IPrimitiveTypeConverter
    {
        /// <summary>
        /// Write the Atom representation of an instance of a primitive type to an XmlWriter.
        /// </summary>
        /// <param name="instance">The instance to write.</param>
        /// <param name="writer">The Xml writer to use to write the instance.</param>
        public void WriteAtom(object instance, XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Write the Atom representation of an instance of a primitive type to an TextWriter.
        /// </summary>
        /// <param name="instance">The instance to write.</param>
        /// <param name="writer">The text writer to use to write the instance.</param>
        public void WriteAtom(object instance, TextWriter writer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Write the Json Lite representation of an instance of a primitive type to a json writer.
        /// </summary>
        /// <param name="instance">The instance to write.</param>
        /// <param name="jsonWriter">Instance of JsonWriter.</param>
        public void WriteJsonLight(object instance, IJsonWriter jsonWriter)
        {
            var dictionary = (IDictionary<string, object>)instance;

            jsonWriter.StartObjectScope();

            foreach (var item in dictionary)
            {
                jsonWriter.WriteName(item.Key);

                switch (item.Value)
                {
                    case bool boolValue:
                        jsonWriter.WriteValue(boolValue);
                        break;
                    case float floatValue:
                        jsonWriter.WriteValue(floatValue);
                        break;
                    case short shortValue:
                        jsonWriter.WriteValue(shortValue);
                        break;
                    case long longValue:
                        jsonWriter.WriteValue(longValue);
                        break;
                    case double doubleValue:
                        jsonWriter.WriteValue(doubleValue);
                        break;
                    case Guid guidValue:
                        jsonWriter.WriteValue(guidValue);
                        break;
                    case decimal decimalValue:
                        jsonWriter.WriteValue(decimalValue);
                        break;
                    case DateTimeOffset dateTimeOffsetValue:
                        jsonWriter.WriteValue(dateTimeOffsetValue);
                        break;
                    case TimeSpan timeSpanValue:
                        jsonWriter.WriteValue(timeSpanValue);
                        break;
                    case byte byteValue:
                        jsonWriter.WriteValue(byteValue);
                        break;
                    case sbyte sbyteValue:
                        jsonWriter.WriteValue(sbyteValue);
                        break;
                    case byte[] byteArrayValue:
                        jsonWriter.WriteValue(byteArrayValue);
                        break;
                    case int intValue:
                        jsonWriter.WriteValue(intValue);
                        break;
                    case string stringValue:
                        jsonWriter.WriteValue(stringValue);
                        break;
                    case IDictionary<string, object> dictionaryOfStringObjectValue:
                        WriteJsonLight(dictionaryOfStringObjectValue, jsonWriter);
                        break;
                }
            }

            jsonWriter.EndObjectScope();
        }
    }
}

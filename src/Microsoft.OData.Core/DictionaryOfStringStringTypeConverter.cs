//---------------------------------------------------------------------
// <copyright file="DictionaryOfStringStringTypeConverter.cs" company="Microsoft">
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
    internal sealed class DictionaryOfStringStringTypeConverter : IPrimitiveTypeConverter
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
            var dictionary = (IDictionary<string, string>)instance;

            jsonWriter.StartObjectScope();

            foreach (var item in dictionary)
            {
                jsonWriter.WriteName(item.Key);
                jsonWriter.WriteValue(item.Value);
            }

            jsonWriter.EndObjectScope();
        }
    }
}

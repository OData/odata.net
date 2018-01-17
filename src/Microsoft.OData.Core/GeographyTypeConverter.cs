//---------------------------------------------------------------------
// <copyright file="GeographyTypeConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Diagnostics;
using System.IO;
using System.Xml;
using Microsoft.OData.Json;
using Microsoft.OData.Metadata;
using Microsoft.Spatial;

namespace Microsoft.OData
{
    /// <summary>
    /// Handles serialization and deserialization for types derived from Geography.
    /// </summary>
    internal sealed class GeographyTypeConverter : IPrimitiveTypeConverter
    {
        /// <summary>
        /// Write the Atom representation of an instance of a primitive type to an XmlWriter.
        /// </summary>
        /// <param name="instance">The instance to write.</param>
        /// <param name="writer">The Xml writer to use to write the instance.</param>
        public void WriteAtom(object instance, XmlWriter writer)
        {
            ((Geography)instance).SendTo(GmlFormatter.Create().CreateWriter(writer));
        }

        /// <summary>
        /// Write the Atom representation of an instance of a primitive type to an TextWriter.
        /// </summary>
        /// <param name="instance">The instance to write.</param>
        /// <param name="writer">The text writer to use to write the instance.</param>
        public void WriteAtom(object instance, TextWriter writer)
        {
            ((Geography)instance).SendTo(WellKnownTextSqlFormatter.Create().CreateWriter(writer));
        }

        /// <summary>
        /// Write the Json Lite representation of an instance of a primitive type to a json writer.
        /// </summary>
        /// <param name="instance">The instance to write.</param>
        /// <param name="jsonWriter">Instance of JsonWriter.</param>
        public void WriteJsonLight(object instance, IJsonWriter jsonWriter)
        {
            IGeoJsonWriter adapter = new GeoJsonWriterAdapter(jsonWriter);
            ((Geography)instance).SendTo(GeoJsonObjectFormatter.Create().CreateWriter(adapter));
        }
    }
}

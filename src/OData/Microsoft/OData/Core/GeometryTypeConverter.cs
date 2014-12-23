//   OData .NET Libraries ver. 6.9
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Xml;
    using Microsoft.Data.Spatial;
    using Microsoft.OData.Core.Atom;
    using Microsoft.OData.Core.Json;
    using Microsoft.Spatial;
    #endregion

    /// <summary>
    /// Handles serialization and deserialization for types derived from Geometry.
    /// This file is currently compiled by ODataLib and Astoria server, because it contains
    ///      functionality related to both serialization and deserialization, but deserialization
    ///      is not yet integrated into Astoria. Once that integration happens this functionality 
    ///      should be fully contained within ODataLib only.
    /// </summary>
    internal sealed class GeometryTypeConverter : IPrimitiveTypeConverter
    {
        /// <summary>
        /// Create a Geometry instance from the value in an Xml reader.
        /// </summary>
        /// <param name="reader">The Xml reader to use to read the value.</param>
        /// <remarks>In order to be consistent with how we are reading other types of property values elsewhere in the product, the reader
        /// is expected to be placed at the beginning of the element when entering this method. After this method call, the reader will be placed
        /// at the EndElement, such that the next Element will be read in the next Read call. The deserializer that uses this value expects 
        /// the reader to be in these states when entering and leaving the method.
        /// </remarks>
        /// <returns>Geometry instance that was read.</returns>
        public object TokenizeFromXml(XmlReader reader)
        {
            Debug.Assert(reader.NodeType == XmlNodeType.Element, "reader at element");
            reader.ReadStartElement(); // <d:Property>

            Geometry geometry = GmlFormatter.Create().Read<Geometry>(reader);
            reader.SkipInsignificantNodes();
            Debug.Assert(reader.NodeType == XmlNodeType.EndElement, "reader at end of current element");
            return geometry;
        }

        /// <summary>
        /// Write the Atom representation of an instance of a primitive type to an XmlWriter.
        /// </summary>
        /// <param name="instance">The instance to write.</param>
        /// <param name="writer">The Xml writer to use to write the instance.</param>
        public void WriteAtom(object instance, XmlWriter writer)
        {
            ((Geometry)instance).SendTo(GmlFormatter.Create().CreateWriter(writer));
        }

        /// <summary>
        /// Write the Atom representation of an instance of a primitive type to an TextWriter.
        /// </summary>
        /// <param name="instance">The instance to write.</param>
        /// <param name="writer">The text writer to use to write the instance.</param>
        public void WriteAtom(object instance, TextWriter writer)
        {
            ((Geometry)instance).SendTo(WellKnownTextSqlFormatter.Create().CreateWriter(writer));
        }

        /// <summary>
        /// Write the Json Lite representation of an instance of a primitive type to a json object.
        /// </summary>
        /// <param name="instance">The instance to write.</param>
        /// <param name="jsonWriter">Instance of JsonWriter.</param>
        public void WriteJsonLight(object instance, IJsonWriter jsonWriter)
        {
            IGeoJsonWriter adapter = new GeoJsonWriterAdapter(jsonWriter);
            ((Geometry)instance).SendTo(GeoJsonObjectFormatter.Create().CreateWriter(adapter));
        }
    }
}

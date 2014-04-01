//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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

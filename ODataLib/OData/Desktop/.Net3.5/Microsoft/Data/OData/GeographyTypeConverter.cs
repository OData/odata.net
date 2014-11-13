//   OData .NET Libraries ver. 5.6.3
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

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Spatial;
    using System.Xml;
    using Microsoft.Data.OData.Atom;
    using Microsoft.Data.OData.Json;
    using Microsoft.Data.OData.JsonLight;
    #endregion

    /// <summary>
    /// Handles serialization and deserialization for types derived from Geography.
    /// </summary>
    internal sealed class GeographyTypeConverter : IPrimitiveTypeConverter
    {
        /// <summary>
        /// Create a geography instance from the value in an Xml reader.
        /// </summary>
        /// <param name="reader">The Xml reader to use to read the value.</param>
        /// <remarks>In order to be consistent with how we are reading other types of property values elsewhere in the product, the reader
        /// is expected to be placed at the beginning of the element when entering this method. After this method call, the reader will be placed
        /// at the EndElement, such that the next Element will be read in the next Read call. The deserializer that uses this value expects 
        /// the reader to be in these states when entering and leaving the method.
        /// </remarks>
        /// <returns>Geography instance that was read.</returns>
        public object TokenizeFromXml(XmlReader reader)
        {
            Debug.Assert(reader.NodeType == XmlNodeType.Element, "reader at element");
            reader.ReadStartElement(); // <d:Property>

            Geography geography = GmlFormatter.Create().Read<Geography>(reader);
            reader.SkipInsignificantNodes();
            Debug.Assert(reader.NodeType == XmlNodeType.EndElement, "reader at end of current element");
            return geography;
        }

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
        /// Write the Verbose Json representation of an instance of a primitive type to a json writer.
        /// </summary>
        /// <param name="instance">The instance to write.</param>
        /// <param name="jsonWriter">Instance of JsonWriter.</param>
        /// <param name="typeName">Type name of the instance to write. If the type name is null, the type name will not be written in the payload.</param>
        /// <param name="odataVersion">The OData protocol version to be used for writing payloads.</param>
        public void WriteVerboseJson(object instance, IJsonWriter jsonWriter, string typeName, ODataVersion odataVersion)
        {
            IDictionary<string, object> jsonObject = GeoJsonObjectFormatter.Create().Write((ISpatial)instance);
            Debug.Assert(!jsonObject.ContainsKey(JsonConstants.ODataMetadataName), "__metadata should not be present in jsonObject");
            jsonWriter.WriteJsonObjectValue(jsonObject, (jw) => ODataJsonWriterUtils.WriteMetadataWithTypeName(jw, typeName), odataVersion);
        }

        /// <summary>
        /// Write the Json Lite representation of an instance of a primitive type to a json writer.
        /// </summary>
        /// <param name="instance">The instance to write.</param>
        /// <param name="jsonWriter">Instance of JsonWriter.</param>
        /// <param name="odataVersion">The OData protocol version to be used for writing payloads.</param>
        public void WriteJsonLight(object instance, IJsonWriter jsonWriter, ODataVersion odataVersion)
        {
            IDictionary<string, object> jsonObject = GeoJsonObjectFormatter.Create().Write((ISpatial)instance);
            jsonWriter.WriteJsonObjectValue(jsonObject, /*injectPropertyAction*/ null, odataVersion);
        }
    }
}

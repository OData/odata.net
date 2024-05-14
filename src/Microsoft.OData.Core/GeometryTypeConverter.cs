//---------------------------------------------------------------------
// <copyright file="GeometryTypeConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Json;
using Microsoft.Spatial;

namespace Microsoft.OData
{
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
        /// Write the Json representation of an instance of a primitive type to a json object.
        /// </summary>
        /// <param name="instance">The instance to write.</param>
        /// <param name="jsonWriter">Instance of JsonWriter.</param>
        public void WriteJson(object instance, IJsonWriter jsonWriter)
        {
            IGeoJsonWriter adapter = new GeoJsonWriterAdapter(jsonWriter);
            ((Geometry)instance).SendTo(GeoJsonObjectFormatter.Create().CreateWriter(adapter));
        }
    }
}

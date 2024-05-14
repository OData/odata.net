//---------------------------------------------------------------------
// <copyright file="GeographyTypeConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Json;
using Microsoft.Spatial;

namespace Microsoft.OData
{
    /// <summary>
    /// Handles serialization and deserialization for types derived from Geography.
    /// </summary>
    internal sealed class GeographyTypeConverter : IPrimitiveTypeConverter
    {
        /// <summary>
        /// Write the Json representation of an instance of a primitive type to a json writer.
        /// </summary>
        /// <param name="instance">The instance to write.</param>
        /// <param name="jsonWriter">Instance of JsonWriter.</param>
        public void WriteJson(object instance, IJsonWriter jsonWriter)
        {
            IGeoJsonWriter adapter = new GeoJsonWriterAdapter(jsonWriter);
            ((Geography)instance).SendTo(GeoJsonObjectFormatter.Create().CreateWriter(adapter));
        }
    }
}

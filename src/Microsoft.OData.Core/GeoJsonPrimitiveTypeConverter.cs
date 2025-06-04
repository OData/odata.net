//---------------------------------------------------------------------
// <copyright file="GeoJsonPrimitiveTypeConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System.Threading.Tasks;
    using Microsoft.OData.Json;

    #endregion

    internal class GeoJsonPrimitiveTypeConverter : SpatialPrimitiveTypeConverter, ISpatialPrimitiveTypeConverter
    {
        protected override void WriteCrs(IJsonWriter jsonWriter, int srid)
        {
            // No-op - to conform with GeoJSON spec, we don't write CRS in this implementation.
        }

        protected override Task WriteCrsAsync(IJsonWriter jsonWriter, int srid)
        {
            // No-op - to conform with GeoJSON spec, we don't write CRS in this implementation.
            return Task.CompletedTask;
        }
    }
}

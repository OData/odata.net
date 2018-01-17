//---------------------------------------------------------------------
// <copyright file="GeoJsonMember.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    /// <summary>
    /// Defines the members that may be found in a GeoJSON object.
    /// </summary>
    internal enum GeoJsonMember
    {
        /// <summary>
        /// "type" member in a GeoJSON object.
        /// </summary>
        Type,

        /// <summary>
        /// "coordinates" member in GeoJSON object.
        /// </summary>
        Coordinates,

        /// <summary>
        /// "geometries" member in GeoJSON object.
        /// </summary>
        Geometries,

        /// <summary>
        /// "crs" member in GeoJSON object.
        /// </summary>
        Crs,

        /// <summary>
        /// 'properties' member in GeoJSON object
        /// </summary>
        Properties,

        /// <summary>
        /// 'name' member in GeoJSON object
        /// </summary>
        Name,
    }
}

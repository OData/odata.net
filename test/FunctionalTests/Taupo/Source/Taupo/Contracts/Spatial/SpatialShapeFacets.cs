//---------------------------------------------------------------------
// <copyright file="SpatialShapeFacets.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Spatial
{
    using System;

    /// <summary>
    /// Enum representing different facets a spatial shape kind could have. Note that not all combinations are valid.
    /// </summary>
    [Flags]
    public enum SpatialShapeFacets
    {
        /// <summary>
        /// Indicates the shape is geographic
        /// </summary>
        Geography = 1 << 0,

        /// <summary>
        /// Indicates the shape is geometric
        /// </summary>
        Geometry = 1 << 1,

        /// <summary>
        /// Indicates the shape is a collection, ie: GeographyCollection, GeometryCollection, or any Multi*
        /// </summary>
        Collection = 1 << 2,

        /// <summary>
        /// Indicates the shape is either Point or MultiPoint
        /// </summary>
        Point = 1 << 3,

        /// <summary>
        /// Indicates the shape is either LineString or MultiLineString
        /// </summary>
        LineString = 1 << 4,

        /// <summary>
        /// Indicates the shape is either Polygon or MultiPolygon
        /// </summary>
        Polygon = 1 << 5,
    }
}
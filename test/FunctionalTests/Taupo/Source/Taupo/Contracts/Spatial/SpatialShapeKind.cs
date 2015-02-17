//---------------------------------------------------------------------
// <copyright file="SpatialShapeKind.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Spatial
{
    /// <summary>
    /// The different kind of spatial values that might be represented by a provider type (weak types)
    /// </summary>
    public enum SpatialShapeKind
    {
        /// <summary>
        /// No shape specified
        /// </summary>
        Unspecified = 0,

        /// <summary>
        /// Geography base type
        /// </summary>
        Geography = SpatialShapeFacets.Geography,

        /// <summary>
        /// Geometry base type
        /// </summary>
        Geometry = SpatialShapeFacets.Geometry,

        /// <summary>
        /// Geography LineString
        /// </summary>
        GeographyLineString = SpatialShapeFacets.Geography | SpatialShapeFacets.LineString,

        /// <summary>
        /// Geography MultiLineString
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "Spelling is correct and intended.")]
        GeographyMultiLineString = SpatialShapeFacets.Geography | SpatialShapeFacets.LineString | SpatialShapeFacets.Collection,

        /// <summary>
        /// Geography MultiPoint
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "Spelling is correct and intended.")]
        GeographyMultiPoint = SpatialShapeFacets.Geography | SpatialShapeFacets.Point | SpatialShapeFacets.Collection,

        /// <summary>
        /// Geography MultiPolygon
        /// </summary>
        GeographyMultiPolygon = SpatialShapeFacets.Geography | SpatialShapeFacets.Polygon | SpatialShapeFacets.Collection,

        /// <summary>
        /// Geography Point
        /// </summary>
        GeographyPoint = SpatialShapeFacets.Geography | SpatialShapeFacets.Point,

        /// <summary>
        /// Geography Polygon
        /// </summary>
        GeographyPolygon = SpatialShapeFacets.Geography | SpatialShapeFacets.Polygon,

        /// <summary>
        /// Geography Collection
        /// </summary>
        GeographyCollection = SpatialShapeFacets.Geography | SpatialShapeFacets.Collection,

        /// <summary>
        /// Geometry LineString
        /// </summary>
        GeometryLineString = SpatialShapeFacets.Geometry | SpatialShapeFacets.LineString,

        /// <summary>
        /// Geometry LineString
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "Spelling is correct and intended.")]
        GeometryMultiLineString = SpatialShapeFacets.Geometry | SpatialShapeFacets.LineString | SpatialShapeFacets.Collection,

        /// <summary>
        /// Geometry MultiPoint
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "Spelling is correct and intended.")]
        GeometryMultiPoint = SpatialShapeFacets.Geometry | SpatialShapeFacets.Point | SpatialShapeFacets.Collection,

        /// <summary>
        /// Geometry MultiPolygon
        /// </summary>
        GeometryMultiPolygon = SpatialShapeFacets.Geometry | SpatialShapeFacets.Polygon | SpatialShapeFacets.Collection,

        /// <summary>
        /// Geometry Point
        /// </summary>
        GeometryPoint = SpatialShapeFacets.Geometry | SpatialShapeFacets.Point,

        /// <summary>
        /// Geometry Polygon
        /// </summary>
        GeometryPolygon = SpatialShapeFacets.Geometry | SpatialShapeFacets.Polygon,

        /// <summary>
        /// Geometry Collection
        /// </summary>
        GeometryCollection = SpatialShapeFacets.Geometry | SpatialShapeFacets.Collection,
    }
}

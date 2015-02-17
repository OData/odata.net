//---------------------------------------------------------------------
// <copyright file="SpatialType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    /// <summary> Defines a list of allowed OpenGisTypes types.  </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1028", Justification = "byte required for packing")]
    public enum SpatialType : byte
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Point
        /// </summary>
        Point = 1,

        /// <summary>
        /// Line String
        /// </summary>
        LineString = 2,

        /// <summary>
        /// Polygon
        /// </summary>
        Polygon = 3,

        /// <summary>
        /// Multi-Point
        /// </summary>
        MultiPoint = 4,

        /// <summary>
        /// Multi-Line-String
        /// </summary>
        MultiLineString = 5,

        /// <summary>
        /// Multi-Polygon
        /// </summary>
        MultiPolygon = 6,

        /// <summary>
        /// Collection
        /// </summary>
        Collection = 7,

#if CURVE_SUPPORT
    /// <summary>
    /// Circular-String
    /// </summary>
        CircularString = 8,

        /// <summary>
        /// Compound Curve
        /// </summary>
        CompoundCurve = 9,

        /// <summary>
        /// Curve Polygon
        /// </summary>
        CurvePolygon = 10,
#endif

        /// <summary>
        /// Full Globe
        /// </summary>
        FullGlobe = 11
    }
}
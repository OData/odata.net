//---------------------------------------------------------------------
// <copyright file="GeometryFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Geometry Factory
    /// </summary>
    public static class GeometryFactory
    {
        #region Point and MultiPoint

        /// <summary>
        /// Create a Geometry Point
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <param name="x">The X value</param>
        /// <param name="y">The Y value</param>
        /// <param name="z">The Z value</param>
        /// <param name="m">The M value</param>
        /// <returns>A Geometry Point Factory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
        public static GeometryFactory<GeometryPoint> Point(CoordinateSystem coordinateSystem, double x, double y, double? z, double? m)
        {
            return new GeometryFactory<GeometryPoint>(coordinateSystem).Point(x, y, z, m);
        }

        /// <summary>
        /// Create a Geometry Point
        /// </summary>
        /// <param name="x">The X value</param>
        /// <param name="y">The Y value</param>
        /// <param name="z">The Z value</param>
        /// <param name="m">The M value</param>
        /// <returns>A Geometry Point Factory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
        public static GeometryFactory<GeometryPoint> Point(double x, double y, double? z, double? m)
        {
            return Point(CoordinateSystem.DefaultGeometry, x, y, z, m);
        }

        /// <summary>
        /// Create a Geometry Point
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <param name="x">The X value</param>
        /// <param name="y">The Y value</param>
        /// <returns>A Geometry Point Factory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
        public static GeometryFactory<GeometryPoint> Point(CoordinateSystem coordinateSystem, double x, double y)
        {
            return Point(coordinateSystem, x, y, null, null);
        }

        /// <summary>
        /// Create a Geometry Point
        /// </summary>
        /// <param name="x">The X value</param>
        /// <param name="y">The Y value</param>
        /// <returns>A Geometry Point Factory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
        public static GeometryFactory<GeometryPoint> Point(double x, double y)
        {
            return Point(CoordinateSystem.DefaultGeometry, x, y, null, null);
        }

        /// <summary>
        /// Create a factory with an empty Geometry Point
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <returns>A Geometry Point Factory</returns>
        public static GeometryFactory<GeometryPoint> Point(CoordinateSystem coordinateSystem)
        {
            return new GeometryFactory<GeometryPoint>(coordinateSystem).Point();
        }

        /// <summary>
        /// Create a factory with an empty Geometry Point
        /// </summary>
        /// <returns>A Geometry Point Factory</returns>
        public static GeometryFactory<GeometryPoint> Point()
        {
            return Point(CoordinateSystem.DefaultGeometry);
        }

        /// <summary>
        /// Create a Geometry MultiPoint
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <returns>A Geometry MultiPoint Factory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Multi is meaningful")]
        public static GeometryFactory<GeometryMultiPoint> MultiPoint(CoordinateSystem coordinateSystem)
        {
            return new GeometryFactory<GeometryMultiPoint>(coordinateSystem).MultiPoint();
        }

        /// <summary>
        /// Create a Geometry MultiPoint
        /// </summary>
        /// <returns>A Geometry MultiPoint Factory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Multi is meaningful")]
        public static GeometryFactory<GeometryMultiPoint> MultiPoint()
        {
            return MultiPoint(CoordinateSystem.DefaultGeometry);
        }

        #endregion

        #region LineString and MultiLineString

        /// <summary>
        /// Create a Geometry LineString with a starting position
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <param name="x">The X value</param>
        /// <param name="y">The Y value</param>
        /// <param name="z">The Z value</param>
        /// <param name="m">The M value</param>
        /// <returns>A Geometry LineString Factory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
        public static GeometryFactory<GeometryLineString> LineString(CoordinateSystem coordinateSystem, double x, double y, double? z, double? m)
        {
            return new GeometryFactory<GeometryLineString>(coordinateSystem).LineString(x, y, z, m);
        }

        /// <summary>
        /// Create a Geometry LineString with a starting position
        /// </summary>
        /// <param name="x">The X value</param>
        /// <param name="y">The Y value</param>
        /// <param name="z">The Z value</param>
        /// <param name="m">The M value</param>
        /// <returns>A Geometry LineString Factory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
        public static GeometryFactory<GeometryLineString> LineString(double x, double y, double? z, double? m)
        {
            return LineString(CoordinateSystem.DefaultGeometry, x, y, z, m);
        }

        /// <summary>
        /// Create a Geometry LineString with a starting position
        /// </summary>
        /// <param name="coordinateSystem">The coordinate system</param>
        /// <param name="x">The X value</param>
        /// <param name="y">The Y value</param>
        /// <returns>A Geometry LineString Factory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
        public static GeometryFactory<GeometryLineString> LineString(CoordinateSystem coordinateSystem, double x, double y)
        {
            return LineString(coordinateSystem, x, y, null, null);
        }

        /// <summary>
        /// Create a Geometry LineString with a starting position
        /// </summary>
        /// <param name="x">The X value</param>
        /// <param name="y">The Y value</param>
        /// <returns>A Geometry LineString Factory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
        public static GeometryFactory<GeometryLineString> LineString(double x, double y)
        {
            return LineString(CoordinateSystem.DefaultGeometry, x, y, null, null);
        }

        /// <summary>
        /// Create an empty Geometry LineString
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <returns>A Geometry LineString Factory</returns>
        public static GeometryFactory<GeometryLineString> LineString(CoordinateSystem coordinateSystem)
        {
            return new GeometryFactory<GeometryLineString>(coordinateSystem).LineString();
        }

        /// <summary>
        /// Create an empty Geometry LineString
        /// </summary>
        /// <returns>A Geometry LineString Factory</returns>
        public static GeometryFactory<GeometryLineString> LineString()
        {
            return LineString(CoordinateSystem.DefaultGeometry);
        }

        /// <summary>
        /// Create a Geometry MultiLineString
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <returns>A Geometry MultiLineString Factory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Multi is meaningful")]
        public static GeometryFactory<GeometryMultiLineString> MultiLineString(CoordinateSystem coordinateSystem)
        {
            return new GeometryFactory<GeometryMultiLineString>(coordinateSystem).MultiLineString();
        }

        /// <summary>
        /// Create a Geometry MultiLineString
        /// </summary>
        /// <returns>A Geometry MultiLineString Factory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Multi is meaningful")]
        public static GeometryFactory<GeometryMultiLineString> MultiLineString()
        {
            return MultiLineString(CoordinateSystem.DefaultGeometry);
        }

        #endregion

        #region Polygon

        /// <summary>
        /// Create a Geometry Polygon
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <returns>A Geometry Polygon Factory</returns>
        public static GeometryFactory<GeometryPolygon> Polygon(CoordinateSystem coordinateSystem)
        {
            return new GeometryFactory<GeometryPolygon>(coordinateSystem).Polygon();
        }

        /// <summary>
        /// Create a Geometry Polygon
        /// </summary>
        /// <returns>A Geometry Polygon Factory</returns>
        public static GeometryFactory<GeometryPolygon> Polygon()
        {
            return Polygon(CoordinateSystem.DefaultGeometry);
        }

        /// <summary>
        /// Create a Geometry MultiPolygon
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <returns>A Geometry MultiPolygon Factory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Multi is meaningful")]
        public static GeometryFactory<GeometryMultiPolygon> MultiPolygon(CoordinateSystem coordinateSystem)
        {
            return new GeometryFactory<GeometryMultiPolygon>(coordinateSystem).MultiPolygon();
        }

        /// <summary>
        /// Create a Geometry MultiPolygon
        /// </summary>
        /// <returns>A Geometry MultiPolygon Factory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Multi is meaningful")]
        public static GeometryFactory<GeometryMultiPolygon> MultiPolygon()
        {
            return MultiPolygon(CoordinateSystem.DefaultGeometry);
        }

        #endregion

        #region Collection

        /// <summary>
        /// Create a Geometry Collection
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <returns>A Geometry Collection Factory</returns>
        public static GeometryFactory<GeometryCollection> Collection(CoordinateSystem coordinateSystem)
        {
            return new GeometryFactory<GeometryCollection>(coordinateSystem).Collection();
        }

        /// <summary>
        /// Create a Geometry Collection
        /// </summary>
        /// <returns>A Geometry Collection Factory</returns>
        public static GeometryFactory<GeometryCollection> Collection()
        {
            return Collection(CoordinateSystem.DefaultGeometry);
        }

        #endregion
    }
}
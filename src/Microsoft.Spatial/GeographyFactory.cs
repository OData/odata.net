//---------------------------------------------------------------------
// <copyright file="GeographyFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Geography Factory
    /// </summary>
    public static class GeographyFactory
    {
        #region Point and MultiPoint

        /// <summary>
        /// Create a Geography Point
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <param name="latitude">The latitude value</param>
        /// <param name="longitude">The longitude value</param>
        /// <param name="z">The Z value</param>
        /// <param name="m">The M value</param>
        /// <returns>A Geography Point Factory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
        public static GeographyFactory<GeographyPoint> Point(CoordinateSystem coordinateSystem, double latitude, double longitude, double? z, double? m)
        {
            return new GeographyFactory<GeographyPoint>(coordinateSystem).Point(latitude, longitude, z, m);
        }

        /// <summary>
        /// Create a Geography Point
        /// </summary>
        /// <param name="latitude">The latitude value</param>
        /// <param name="longitude">The longitude value</param>
        /// <param name="z">The Z value</param>
        /// <param name="m">The M value</param>
        /// <returns>A Geography Point Factory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
        public static GeographyFactory<GeographyPoint> Point(double latitude, double longitude, double? z, double? m)
        {
            return Point(CoordinateSystem.DefaultGeography, latitude, longitude, z, m);
        }

        /// <summary>
        /// Create a Geography Point
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <param name="latitude">The latitude value</param>
        /// <param name="longitude">The longitude value</param>
        /// <returns>A Geography Point Factory</returns>
        public static GeographyFactory<GeographyPoint> Point(CoordinateSystem coordinateSystem, double latitude, double longitude)
        {
            return Point(coordinateSystem, latitude, longitude, null, null);
        }

        /// <summary>
        /// Create a Geography Point
        /// </summary>
        /// <param name="latitude">The latitude value</param>
        /// <param name="longitude">The longitude value</param>
        /// <returns>A Geography Point Factory</returns>
        public static GeographyFactory<GeographyPoint> Point(double latitude, double longitude)
        {
            return Point(CoordinateSystem.DefaultGeography, latitude, longitude, null, null);
        }

        /// <summary>
        /// Create a factory with an empty Geography Point
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <returns>A Geography Point Factory</returns>
        public static GeographyFactory<GeographyPoint> Point(CoordinateSystem coordinateSystem)
        {
            return new GeographyFactory<GeographyPoint>(coordinateSystem).Point();
        }

        /// <summary>
        /// Create a factory with an empty Geography Point
        /// </summary>
        /// <returns>A Geography Point Factory</returns>
        public static GeographyFactory<GeographyPoint> Point()
        {
            return Point(CoordinateSystem.DefaultGeography);
        }

        /// <summary>
        /// Create a Geography MultiPoint
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <returns>A Geography MultiPoint Factory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Multi is meaningful")]
        public static GeographyFactory<GeographyMultiPoint> MultiPoint(CoordinateSystem coordinateSystem)
        {
            return new GeographyFactory<GeographyMultiPoint>(coordinateSystem).MultiPoint();
        }

        /// <summary>
        /// Create a Geography MultiPoint
        /// </summary>
        /// <returns>A Geography MultiPoint Factory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Multi is meaningful")]
        public static GeographyFactory<GeographyMultiPoint> MultiPoint()
        {
            return MultiPoint(CoordinateSystem.DefaultGeography);
        }

        #endregion

        #region LineString and MultiLineString

        /// <summary>
        /// Create a Geography LineString with a starting position
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <param name="latitude">The latitude value</param>
        /// <param name="longitude">The longitude value</param>
        /// <param name="z">The Z value</param>
        /// <param name="m">The M value</param>
        /// <returns>A Geography LineString Factory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
        public static GeographyFactory<GeographyLineString> LineString(CoordinateSystem coordinateSystem, double latitude, double longitude, double? z, double? m)
        {
            return new GeographyFactory<GeographyLineString>(coordinateSystem).LineString(latitude, longitude, z, m);
        }

        /// <summary>
        /// Create a Geography LineString with a starting position
        /// </summary>
        /// <param name="latitude">The latitude value</param>
        /// <param name="longitude">The longitude value</param>
        /// <param name="z">The Z value</param>
        /// <param name="m">The M value</param>
        /// <returns>A Geography LineString Factory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "x, y, z and m are meaningful")]
        public static GeographyFactory<GeographyLineString> LineString(double latitude, double longitude, double? z, double? m)
        {
            return LineString(CoordinateSystem.DefaultGeography, latitude, longitude, z, m);
        }

        /// <summary>
        /// Create a Geography LineString with a starting position
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <param name="latitude">The latitude value</param>
        /// <param name="longitude">The longitude value</param>
        /// <returns>A Geography LineString Factory</returns>
        public static GeographyFactory<GeographyLineString> LineString(CoordinateSystem coordinateSystem, double latitude, double longitude)
        {
            return LineString(coordinateSystem, latitude, longitude, null, null);
        }

        /// <summary>
        /// Create a Geography LineString with a starting position
        /// </summary>
        /// <param name="latitude">The latitude value</param>
        /// <param name="longitude">The longitude value</param>
        /// <returns>A Geography LineString Factory</returns>
        public static GeographyFactory<GeographyLineString> LineString(double latitude, double longitude)
        {
            return LineString(CoordinateSystem.DefaultGeography, latitude, longitude, null, null);
        }

        /// <summary>
        /// Create an empty Geography LineString
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <returns>A Geography LineString Factory</returns>
        public static GeographyFactory<GeographyLineString> LineString(CoordinateSystem coordinateSystem)
        {
            return new GeographyFactory<GeographyLineString>(coordinateSystem).LineString();
        }

        /// <summary>
        /// Create an empty Geography LineString
        /// </summary>
        /// <returns>A Geography LineString Factory</returns>
        public static GeographyFactory<GeographyLineString> LineString()
        {
            return LineString(CoordinateSystem.DefaultGeography);
        }

        /// <summary>
        /// Create a Geography MultiLineString
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <returns>A Geography MultiLineString Factory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Multi is meaningful")]
        public static GeographyFactory<GeographyMultiLineString> MultiLineString(CoordinateSystem coordinateSystem)
        {
            return new GeographyFactory<GeographyMultiLineString>(coordinateSystem).MultiLineString();
        }

        /// <summary>
        /// Create a Geography MultiLineString
        /// </summary>
        /// <returns>A Geography MultiLineString Factory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Multi is meaningful")]
        public static GeographyFactory<GeographyMultiLineString> MultiLineString()
        {
            return MultiLineString(CoordinateSystem.DefaultGeography);
        }

        #endregion

        #region Polygon

        /// <summary>
        /// Create a Geography Polygon
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <returns>A Geography Polygon Factory</returns>
        public static GeographyFactory<GeographyPolygon> Polygon(CoordinateSystem coordinateSystem)
        {
            return new GeographyFactory<GeographyPolygon>(coordinateSystem).Polygon();
        }

        /// <summary>
        /// Create a Geography Polygon
        /// </summary>
        /// <returns>A Geography Polygon Factory</returns>
        public static GeographyFactory<GeographyPolygon> Polygon()
        {
            return Polygon(CoordinateSystem.DefaultGeography);
        }

        /// <summary>
        /// Create a Geography MultiPolygon
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <returns>A Geography MultiPolygon Factory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Multi is meaningful")]
        public static GeographyFactory<GeographyMultiPolygon> MultiPolygon(CoordinateSystem coordinateSystem)
        {
            return new GeographyFactory<GeographyMultiPolygon>(coordinateSystem).MultiPolygon();
        }

        /// <summary>
        /// Create a Geography MultiPolygon
        /// </summary>
        /// <returns>A Geography MultiPolygon Factory</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Multi is meaningful")]
        public static GeographyFactory<GeographyMultiPolygon> MultiPolygon()
        {
            return MultiPolygon(CoordinateSystem.DefaultGeography);
        }

        #endregion

        #region Collection

        /// <summary>
        /// Create a Geography Collection
        /// </summary>
        /// <param name="coordinateSystem">The CoordinateSystem</param>
        /// <returns>A Geography Collection Factory</returns>
        public static GeographyFactory<GeographyCollection> Collection(CoordinateSystem coordinateSystem)
        {
            return new GeographyFactory<GeographyCollection>(coordinateSystem).Collection();
        }

        /// <summary>
        /// Create a Geography Collection
        /// </summary>
        /// <returns>A Geography Collection Factory</returns>
        public static GeographyFactory<GeographyCollection> Collection()
        {
            return Collection(CoordinateSystem.DefaultGeography);
        }

        #endregion
    }
}
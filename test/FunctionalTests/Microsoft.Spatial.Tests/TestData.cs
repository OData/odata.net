//---------------------------------------------------------------------
// <copyright file="TestData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.Data.Spatial;

namespace Microsoft.Spatial.Tests
{
    /// <summary>
    ///   Generates data values for use in tests.
    /// </summary>
    internal static class TestData
    {
        /// <summary>
        ///   Returns a new instance each time it is called. Defaults any values you don't override.
        /// </summary>
        /// <returns></returns>
        internal static GeometryPointImplementation PointM(double x = 1.0, double y = 2.0, double? z = 3.0, double? m = 4.0, CoordinateSystem coords = null)
        {
            coords = coords ?? CoordinateSystem.Geometry(1234);
            return new GeometryPointImplementation(coords, SpatialImplementation.CurrentImplementation, x, y, z, m);
        }

        /// <summary>
        ///   Returns a new empty instance each time it is called.
        /// </summary>
        /// <returns></returns>
        internal static GeometryPointImplementation PointMEmpty(CoordinateSystem crs = null)
        {
            crs = crs ?? CoordinateSystem.DefaultGeometry;
            return new GeometryPointImplementation(crs, SpatialImplementation.CurrentImplementation);
        }

        /// <summary>
        ///   Returns a new instance each time it is called. Defaults any values you don't override.
        /// </summary>
        /// <returns></returns>
        internal static GeometryLineStringImplementation LineStringM()
        {
            return new GeometryLineStringImplementation(CoordinateSystem.Geometry(1234), SpatialImplementation.CurrentImplementation, PointM(), PointM());
        }

        /// <summary>
        ///   Returns a new empty instance each time it is called.
        /// </summary>
        /// <param name = "crs">The CRS.</param>
        /// <returns></returns>
        internal static GeometryLineStringImplementation LineStringMEmpty(CoordinateSystem crs = null)
        {
            crs = crs ?? CoordinateSystem.DefaultGeometry;
            return new GeometryLineStringImplementation(crs, SpatialImplementation.CurrentImplementation);
        }

        /// <summary>
        ///   Returns a new instance each time it is called. Defaults any values you don't override.
        /// </summary>
        /// <returns></returns>
        internal static GeometryPolygonImplementation PolygonM()
        {
            return new GeometryPolygonImplementation(CoordinateSystem.Geometry(1234), SpatialImplementation.CurrentImplementation, LineStringM());
        }

        /// <summary>
        ///   Returns a new empty instance each time it is called.
        /// </summary>
        /// <param name = "crs">The CRS.</param>
        /// <returns></returns>
        internal static GeometryPolygonImplementation PolygonMEmpty(CoordinateSystem crs = null)
        {
            crs = crs ?? CoordinateSystem.DefaultGeometry;
            return new GeometryPolygonImplementation(crs, SpatialImplementation.CurrentImplementation);
        }

        /// <summary>
        ///   Returns a new instance each time it is called. Defaults any values you don't override.
        /// </summary>
        /// <returns></returns>
        internal static GeometryCollectionImplementation GeometryCollection(params Geometry[] items)
        {
            items = (items.Length == 0) ? new[] {PointM()} : items;
            return new GeometryCollectionImplementation(SpatialImplementation.CurrentImplementation, items);
        }

        /// <summary>
        ///   Returns a new empty instance each time it is called.
        /// </summary>
        /// <returns></returns>
        internal static GeometryCollectionImplementation GeometryCollectionEmpty()
        {
            return new GeometryCollectionImplementation(SpatialImplementation.CurrentImplementation);
        }

        /// <summary>
        ///   Returns a new instance each time it is called. Defaults any values you don't override.
        /// </summary>
        /// <returns></returns>
        internal static GeometryMultiPointImplementation MultiPointM(params GeometryPoint[] items)
        {
            items = (items.Length == 0) ? new[] {PointM()} : items;
            return new GeometryMultiPointImplementation(CoordinateSystem.Geometry(1234), SpatialImplementation.CurrentImplementation, items);
        }

        /// <summary>
        ///   Returns a new empty instance each time it is called.
        /// </summary>
        /// <returns></returns>
        internal static GeometryMultiPointImplementation MultiPointMEmpty()
        {
            return new GeometryMultiPointImplementation(CoordinateSystem.Geometry(1234), SpatialImplementation.CurrentImplementation);
        }

        /// <summary>
        ///   Returns a new instance each time it is called. Defaults any values you don't override.
        /// </summary>
        /// <returns></returns>
        internal static GeometryMultiLineStringImplementation MultiLineStringM(params GeometryLineString[] items)
        {
            items = (items.Length == 0) ? new[] {LineStringM()} : items;
            return new GeometryMultiLineStringImplementation(CoordinateSystem.Geometry(1234), SpatialImplementation.CurrentImplementation, items);
        }

        /// <summary>
        ///   Returns a new empty instance each time it is called.
        /// </summary>
        /// <returns></returns>
        internal static GeometryMultiLineStringImplementation MultiLineStringMEmpty()
        {
            return new GeometryMultiLineStringImplementation(CoordinateSystem.Geometry(1234), SpatialImplementation.CurrentImplementation);
        }

        /// <summary>
        ///   Returns a new instance each time it is called. Defaults any values you don't override.
        /// </summary>
        /// <returns></returns>
        internal static GeometryMultiPolygonImplementation MultiPolygonM(params GeometryPolygon[] items)
        {
            items = (items.Length == 0) ? new[] {PolygonM()} : items;
            return new GeometryMultiPolygonImplementation(CoordinateSystem.Geometry(1234), SpatialImplementation.CurrentImplementation, items);
        }

        /// <summary>
        ///   Returns a new empty instance each time it is called.
        /// </summary>
        /// <returns></returns>
        internal static GeometryMultiPolygonImplementation MultiPolygonMEmpty()
        {
            return new GeometryMultiPolygonImplementation(CoordinateSystem.Geometry(1234), SpatialImplementation.CurrentImplementation);
        }

        /// <summary>
        ///   Returns a new instance each time it is called. Defaults any values you don't override.
        /// </summary>
        /// <returns></returns>
        internal static GeographyPointImplementation PointG(double x = 1.0, double y = 2.0, double? z = 3.0, double? m = 4.0, CoordinateSystem coords = null)
        {
            coords = coords ?? CoordinateSystem.Geography(1234);
            return new GeographyPointImplementation(coords, SpatialImplementation.CurrentImplementation, x, y, z, m);
        }

        /// <summary>
        ///   Returns a new empty instance each time it is called.
        /// </summary>
        /// <returns></returns>
        internal static GeographyPointImplementation PointGEmpty(CoordinateSystem crs = null)
        {
            crs = crs ?? CoordinateSystem.DefaultGeography;
            return new GeographyPointImplementation(crs, SpatialImplementation.CurrentImplementation);
        }

        /// <summary>
        ///   Returns a new instance each time it is called. Defaults any values you don't override.
        /// </summary>
        /// <returns></returns>
        internal static GeographyLineStringImplementation LineStringG()
        {
            return new GeographyLineStringImplementation(CoordinateSystem.Geography(1234), SpatialImplementation.CurrentImplementation, PointG(), PointG());
        }

        /// <summary>
        ///   Returns a new empty instance each time it is called.
        /// </summary>
        /// <param name = "crs">The CRS.</param>
        /// <returns></returns>
        internal static GeographyLineStringImplementation LineStringGEmpty(CoordinateSystem crs = null)
        {
            crs = crs ?? CoordinateSystem.DefaultGeography;
            return new GeographyLineStringImplementation(crs, SpatialImplementation.CurrentImplementation);
        }

        /// <summary>
        ///   Returns a new instance each time it is called. Defaults any values you don't override.
        /// </summary>
        /// <returns></returns>
        internal static GeographyPolygonImplementation PolygonG()
        {
            return new GeographyPolygonImplementation(CoordinateSystem.Geography(1234), SpatialImplementation.CurrentImplementation, LineStringG());
        }

        /// <summary>
        ///   Returns a new empty instance each time it is called.
        /// </summary>
        /// <param name = "crs">The CRS.</param>
        /// <returns></returns>
        internal static GeographyPolygonImplementation PolygonGEmpty(CoordinateSystem crs = null)
        {
            crs = crs ?? CoordinateSystem.DefaultGeography;
            return new GeographyPolygonImplementation(crs, SpatialImplementation.CurrentImplementation);
        }

        /// <summary>
        ///   Returns a new instance each time it is called. Defaults any values you don't override.
        /// </summary>
        /// <returns></returns>
        internal static GeographyFullGlobeImplementation FullGlobe(CoordinateSystem crs = null)
        {
            crs = crs ?? CoordinateSystem.Geography(1234);
            return new GeographyFullGlobeImplementation(crs, SpatialImplementation.CurrentImplementation);
        }

        /// <summary>
        ///   Returns a new instance each time it is called. Defaults any values you don't override.
        /// </summary>
        /// <returns></returns>
        internal static GeographyCollectionImplementation GeographyCollection(params Geography[] items)
        {
            items = (items.Length == 0) ? new[] { PointG() } : items;
            return new GeographyCollectionImplementation(SpatialImplementation.CurrentImplementation, items);
        }

        /// <summary>
        ///   Returns a new empty instance each time it is called.
        /// </summary>
        /// <returns></returns>
        internal static GeographyCollectionImplementation GeographyCollectionEmpty()
        {
            return new GeographyCollectionImplementation(SpatialImplementation.CurrentImplementation);
        }

        /// <summary>
        ///   Returns a new instance each time it is called. Defaults any values you don't override.
        /// </summary>
        /// <returns></returns>
        internal static GeographyMultiPointImplementation MultiPointG(params GeographyPoint[] items)
        {
            items = (items.Length == 0) ? new[] { PointG() } : items;
            return new GeographyMultiPointImplementation(CoordinateSystem.Geography(1234), SpatialImplementation.CurrentImplementation, items);
        }

        /// <summary>
        ///   Returns a new empty instance each time it is called.
        /// </summary>
        /// <returns></returns>
        internal static GeographyMultiPointImplementation MultiPointGEmpty()
        {
            return new GeographyMultiPointImplementation(CoordinateSystem.Geography(1234), SpatialImplementation.CurrentImplementation);
        }

        /// <summary>
        ///   Returns a new instance each time it is called. Defaults any values you don't override.
        /// </summary>
        /// <returns></returns>
        internal static GeographyMultiLineStringImplementation MultiLineStringG(params GeographyLineString[] items)
        {
            items = (items.Length == 0) ? new[] { LineStringG() } : items;
            return new GeographyMultiLineStringImplementation(CoordinateSystem.Geography(1234), SpatialImplementation.CurrentImplementation, items);
        }

        /// <summary>
        ///   Returns a new empty instance each time it is called.
        /// </summary>
        /// <returns></returns>
        internal static GeographyMultiLineStringImplementation MultiLineStringGEmpty()
        {
            return new GeographyMultiLineStringImplementation(CoordinateSystem.Geography(1234), SpatialImplementation.CurrentImplementation);
        }

        /// <summary>
        ///   Returns a new instance each time it is called. Defaults any values you don't override.
        /// </summary>
        /// <returns></returns>
        internal static GeographyMultiPolygonImplementation MultiPolygonG(params GeographyPolygon[] items)
        {
            items = (items.Length == 0) ? new[] { PolygonG() } : items;
            return new GeographyMultiPolygonImplementation(CoordinateSystem.Geography(1234), SpatialImplementation.CurrentImplementation, items);
        }

        /// <summary>
        ///   Returns a new empty instance each time it is called.
        /// </summary>
        /// <returns></returns>
        internal static GeographyMultiPolygonImplementation MultiPolygonGEmpty()
        {
            return new GeographyMultiPolygonImplementation(CoordinateSystem.Geography(1234), SpatialImplementation.CurrentImplementation);
        }
    }
}
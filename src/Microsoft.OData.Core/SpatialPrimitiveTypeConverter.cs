//---------------------------------------------------------------------
// <copyright file="SpatialPrimitiveTypeConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.OData.Core;
    using Microsoft.OData.Json;
    using NetTopologySuite.Geometries;

    #endregion

    internal class SpatialPrimitiveTypeConverter : IPrimitiveTypeConverter, ISpatialPrimitiveTypeConverter
    {
        /// <summary>
        /// The default instance for <see cref="ISpatialPrimitiveTypeConverter"/>.
        /// </summary>
        private static readonly ISpatialPrimitiveTypeConverter Default = new SpatialPrimitiveTypeConverter();

        public virtual void WriteJson(object instance, IJsonWriter jsonWriter)
        {
            Debug.Assert(jsonWriter != null);
            Debug.Assert(instance != null);
            Geometry ntsGeometry = ConvertToSpatial(instance);

            WriteGeometry(jsonWriter, ntsGeometry, writeCrs: true);
        }

        public virtual async Task WriteJsonAsync(object instance, IJsonWriter jsonWriter)
        {
            Debug.Assert(jsonWriter != null);
            Debug.Assert(instance != null);
            Geometry ntsGeometry = ConvertToSpatial(instance);

            await WriteGeometryAsync(jsonWriter, ntsGeometry, writeCrs: true).ConfigureAwait(false);
        }

        internal static ISpatialPrimitiveTypeConverter GetSpatialPrimitiveTypeConverter(IServiceProvider container)
        {
            if (container == null)
            {
                return Default;
            }

            return container.GetRequiredService<ISpatialPrimitiveTypeConverter>();
        }

        private static Geometry ConvertToSpatial(object instance)
        {
            Geometry ntsGeometry;
            if (instance is Spatial.Geography geography)
            {
                // If the instance is Geography, we convert it to Geometry for writing.
                ntsGeometry = geography;
            }
            else if (instance is Spatial.Geometry geometry)
            {
                ntsGeometry = geometry;
            }
            else
            {
                throw new ArgumentException($"Expected instance of type {nameof(Spatial.Geometry)} or {nameof(Spatial.Geography)} but got {instance.GetType()}.");
            }

            return ntsGeometry;
        }

        private void WriteGeometry(IJsonWriter jsonWriter, Geometry geometry, bool writeCrs)
        {
            jsonWriter.StartObjectScope();
            jsonWriter.WriteName("type");
            jsonWriter.WriteValue(geometry.GeometryType);
            switch (geometry)
            {
                case Point:
                case LineString:
                case Polygon:
                case MultiPoint:
                case MultiLineString:
                case MultiPolygon:
                    jsonWriter.WriteName("coordinates");
                    break;

                case GeometryCollection:
                    jsonWriter.WriteName("geometries");
                    break;

                default:
                    throw new ODataException(
                        Error.Format(SRResources.SpatialPrimitiveTypeConverter_UnsupportedGeometryType, typeof(SpatialPrimitiveTypeConverter).FullName, geometry.GeometryType));
            }

            switch (geometry)
            {
                case Point point:
                    WritePoint(jsonWriter, point);
                    break;

                case LineString line:
                    WriteLineString(jsonWriter, line);
                    break;

                case Polygon polygon:
                    WritePolygon(jsonWriter, polygon);
                    break;

                case MultiPoint multiPoint:
                    WriteMultiPoint(jsonWriter, multiPoint);
                    break;

                case MultiLineString multiLine:
                    WriteMultiLineString(jsonWriter, multiLine);
                    break;

                case MultiPolygon multiPolygon:
                    WriteMultiPolygon(jsonWriter, multiPolygon);
                    break;

                case GeometryCollection geometryCollection:
                    WriteGeometryCollection(jsonWriter, geometryCollection);
                    break;

                default:
                    throw new ODataException(
                        Error.Format(SRResources.SpatialPrimitiveTypeConverter_UnsupportedGeometryType, typeof(SpatialPrimitiveTypeConverter).FullName, geometry.GeometryType));
            }

            if (writeCrs)
            {
                WriteCrs(jsonWriter, geometry.SRID);
            }

            jsonWriter.EndObjectScope();
        }

        protected virtual void WriteCrs(IJsonWriter jsonWriter, int srid)
        {
            jsonWriter.WriteName("crs");
            jsonWriter.StartObjectScope();
            jsonWriter.WriteName("type");
            jsonWriter.WriteValue("name");
            jsonWriter.WriteName("properties");
            jsonWriter.StartObjectScope();
            jsonWriter.WriteName("name");
            jsonWriter.WriteValue($"EPSG:{srid}");
            jsonWriter.EndObjectScope();
            jsonWriter.EndObjectScope();
        }

        protected virtual async Task WriteCrsAsync(IJsonWriter jsonWriter, int srid)
        {
            await jsonWriter.WriteNameAsync("crs").ConfigureAwait(false);
            await jsonWriter.StartObjectScopeAsync().ConfigureAwait(false);
            await jsonWriter.WriteNameAsync("type").ConfigureAwait(false);
            await jsonWriter.WriteValueAsync("name").ConfigureAwait(false);
            await jsonWriter.WriteNameAsync("properties").ConfigureAwait(false);
            await jsonWriter.StartObjectScopeAsync().ConfigureAwait(false);
            await jsonWriter.WriteNameAsync("name").ConfigureAwait(false);
            await jsonWriter.WriteValueAsync($"EPSG:{srid}").ConfigureAwait(false);
            await jsonWriter.EndObjectScopeAsync().ConfigureAwait(false);
            await jsonWriter.EndObjectScopeAsync().ConfigureAwait(false);
        }

        private static void WritePoint(IJsonWriter jsonWriter, Point point)
        {
            WriteCoordinates(jsonWriter, point.X, point.Y, point.Z, point.M);
        }

        private static void WriteLineString(IJsonWriter jsonWriter, LineString lineString)
        {
            jsonWriter.StartArrayScope();
            foreach (Coordinate coordinate in lineString.Coordinates)
            {
                WriteCoordinates(jsonWriter, coordinate.X, coordinate.Y, coordinate.Z, coordinate.M);
            }

            jsonWriter.EndArrayScope();
        }

        private static void WritePolygon(IJsonWriter jsonWriter, Polygon polygon)
        {
            jsonWriter.StartArrayScope();

            // Exterior ring
            WriteLinearRing(jsonWriter, polygon.ExteriorRing.Coordinates);

            // Interior rings (holes)
            for (int i = 0; i < polygon.NumInteriorRings; i++)
            {
                WriteLinearRing(jsonWriter, polygon.GetInteriorRingN(i).Coordinates);
            }

            jsonWriter.EndArrayScope();
        }

        private static void WriteLinearRing(IJsonWriter jsonWriter, Coordinate[] coordinates)
        {
            jsonWriter.StartArrayScope();
            foreach (Coordinate coordinate in coordinates)
            {
                WriteCoordinates(jsonWriter, coordinate.X, coordinate.Y, coordinate.Z, coordinate.M);
            }
            jsonWriter.EndArrayScope();
        }

        private static void WriteMultiPoint(IJsonWriter jsonWriter, MultiPoint multiPoint)
        {
            jsonWriter.StartArrayScope();
            foreach (Point point in multiPoint.Geometries)
            {
                WriteCoordinates(jsonWriter, point.X, point.Y, point.Z, point.M);
            }
            jsonWriter.EndArrayScope();
        }

        private static void WriteMultiLineString(IJsonWriter jsonWriter, MultiLineString multiLine)
        {
            jsonWriter.StartArrayScope();
            foreach (LineString lineString in multiLine.Geometries)
            {
                jsonWriter.StartArrayScope();
                foreach (Coordinate coordinate in lineString.Coordinates)
                {
                    WriteCoordinates(jsonWriter, coordinate.X, coordinate.Y, coordinate.Z, coordinate.M);
                }

                jsonWriter.EndArrayScope();
            }

            jsonWriter.EndArrayScope();
        }

        private static void WriteMultiPolygon(IJsonWriter jsonWriter, MultiPolygon multiPolygon)
        {
            jsonWriter.StartArrayScope();
            foreach (Polygon polygon in multiPolygon.Geometries)
            {
                jsonWriter.StartArrayScope();
                WriteLinearRing(jsonWriter, polygon.ExteriorRing.Coordinates);

                for (int i = 0; i < polygon.NumInteriorRings; i++)
                {
                    WriteLinearRing(jsonWriter, polygon.GetInteriorRingN(i).Coordinates);
                }

                jsonWriter.EndArrayScope();
            }

            jsonWriter.EndArrayScope();
        }

        private void WriteGeometryCollection(IJsonWriter jsonWriter, GeometryCollection collection)
        {
            jsonWriter.StartArrayScope();

            foreach (Geometry geometry in collection.Geometries)
            {
                WriteGeometry(jsonWriter, geometry, writeCrs: false); // We don't write CRS for geometry in the collection
            }

            jsonWriter.EndArrayScope();
        }

        private static void WriteCoordinates(IJsonWriter jsonWriter, double x, double y, double z, double m)
        {
            jsonWriter.StartArrayScope();
            jsonWriter.WriteValue(x);
            jsonWriter.WriteValue(y);
            if (!double.IsNaN(z))
            {
                jsonWriter.WriteValue(z);
            }
            if (!double.IsNaN(m))
            {
                jsonWriter.WriteValue(m);
            }
            jsonWriter.EndArrayScope();
        }

        private async Task WriteGeometryAsync(IJsonWriter jsonWriter, Geometry geometry, bool writeCrs)
        {
            await jsonWriter.StartObjectScopeAsync().ConfigureAwait(false);
            await jsonWriter.WriteNameAsync("type").ConfigureAwait(false);
            await jsonWriter.WriteValueAsync(geometry.GeometryType).ConfigureAwait(false);
            switch (geometry)
            {
                case Point:
                case LineString:
                case Polygon:
                case MultiPoint:
                case MultiLineString:
                case MultiPolygon:
                    await jsonWriter.WriteNameAsync("coordinates").ConfigureAwait(false);
                    break;

                case GeometryCollection:
                    await jsonWriter.WriteNameAsync("geometries").ConfigureAwait(false);
                    break;

                default:
                    throw new ODataException(
                        Error.Format(SRResources.SpatialPrimitiveTypeConverter_UnsupportedGeometryType, typeof(SpatialPrimitiveTypeConverter).FullName, geometry.GeometryType));
            }

            switch (geometry)
            {
                case Point point:
                    await WritePointAsync(jsonWriter, point).ConfigureAwait(false);
                    break;

                case LineString line:
                    await WriteLineStringAsync(jsonWriter, line).ConfigureAwait(false);
                    break;

                case Polygon polygon:
                    await WritePolygonAsync(jsonWriter, polygon).ConfigureAwait(false);
                    break;

                case MultiPoint multiPoint:
                    await WriteMultiPointAsync(jsonWriter, multiPoint).ConfigureAwait(false);
                    break;

                case MultiLineString multiLine:
                    await WriteMultiLineStringAsync(jsonWriter, multiLine).ConfigureAwait(false);
                    break;

                case MultiPolygon multiPolygon:
                    await WriteMultiPolygonAsync(jsonWriter, multiPolygon).ConfigureAwait(false);
                    break;

                case GeometryCollection geometryCollection:
                    await WriteGeometryCollectionAsync(jsonWriter, geometryCollection).ConfigureAwait(false);
                    break;

                default:
                    throw new ODataException(
                        Error.Format(SRResources.SpatialPrimitiveTypeConverter_UnsupportedGeometryType, typeof(SpatialPrimitiveTypeConverter).FullName, geometry.GeometryType));
            }

            if (writeCrs)
            {
                await WriteCrsAsync(jsonWriter, geometry.SRID).ConfigureAwait(false);
            }

            await jsonWriter.EndObjectScopeAsync().ConfigureAwait(false);
        }

        private static async Task WritePointAsync(IJsonWriter jsonWriter, Point point)
        {
            await WriteCoordinatesAsync(jsonWriter, point.X, point.Y, point.Z, point.M).ConfigureAwait(false);
        }

        private static async Task WriteLineStringAsync(IJsonWriter jsonWriter, LineString lineString)
        {
            await jsonWriter.StartArrayScopeAsync().ConfigureAwait(false);
            foreach (Coordinate coordinate in lineString.Coordinates)
            {
                await WriteCoordinatesAsync(jsonWriter, coordinate.X, coordinate.Y, coordinate.Z, coordinate.M).ConfigureAwait(false);
            }
            await jsonWriter.EndArrayScopeAsync().ConfigureAwait(false);
        }

        private static async Task WritePolygonAsync(IJsonWriter jsonWriter, Polygon polygon)
        {
            await jsonWriter.StartArrayScopeAsync().ConfigureAwait(false);

            // Exterior ring
            await WriteLinearRingAsync(jsonWriter, polygon.ExteriorRing.Coordinates).ConfigureAwait(false);

            // Interior rings (holes)
            for (int i = 0; i < polygon.NumInteriorRings; i++)
            {
                await WriteLinearRingAsync(jsonWriter, polygon.GetInteriorRingN(i).Coordinates).ConfigureAwait(false);
            }

            await jsonWriter.EndArrayScopeAsync().ConfigureAwait(false);
        }

        private static async Task WriteLinearRingAsync(IJsonWriter jsonWriter, Coordinate[] coordinates)
        {
            await jsonWriter.StartArrayScopeAsync().ConfigureAwait(false);
            foreach (Coordinate coordinate in coordinates)
            {
                await WriteCoordinatesAsync(jsonWriter, coordinate.X, coordinate.Y, coordinate.Z, coordinate.M).ConfigureAwait(false);
            }
            await jsonWriter.EndArrayScopeAsync().ConfigureAwait(false);
        }

        private static async Task WriteMultiPointAsync(IJsonWriter jsonWriter, MultiPoint multiPoint)
        {
            await jsonWriter.StartArrayScopeAsync().ConfigureAwait(false);
            foreach (Point point in multiPoint.Geometries)
            {
                await WriteCoordinatesAsync(jsonWriter, point.X, point.Y, point.Z, point.M).ConfigureAwait(false);
            }
            await jsonWriter.EndArrayScopeAsync().ConfigureAwait(false);
        }

        private static async Task WriteMultiLineStringAsync(IJsonWriter jsonWriter, MultiLineString multiLine)
        {
            await jsonWriter.StartArrayScopeAsync().ConfigureAwait(false);
            foreach (LineString lineString in multiLine.Geometries)
            {
                await jsonWriter.StartArrayScopeAsync().ConfigureAwait(false);
                foreach (Coordinate coordinate in lineString.Coordinates)
                {
                    await WriteCoordinatesAsync(jsonWriter, coordinate.X, coordinate.Y, coordinate.Z, coordinate.M).ConfigureAwait(false);
                }
                await jsonWriter.EndArrayScopeAsync().ConfigureAwait(false);
            }
            await jsonWriter.EndArrayScopeAsync().ConfigureAwait(false);
        }

        private static async Task WriteMultiPolygonAsync(IJsonWriter jsonWriter, MultiPolygon multiPolygon)
        {
            await jsonWriter.StartArrayScopeAsync().ConfigureAwait(false);
            foreach (Polygon polygon in multiPolygon.Geometries)
            {
                await jsonWriter.StartArrayScopeAsync().ConfigureAwait(false);
                await WriteLinearRingAsync(jsonWriter, polygon.ExteriorRing.Coordinates).ConfigureAwait(false);

                for (int i = 0; i < polygon.NumInteriorRings; i++)
                {
                    await WriteLinearRingAsync(jsonWriter, polygon.GetInteriorRingN(i).Coordinates).ConfigureAwait(false);
                }

                await jsonWriter.EndArrayScopeAsync().ConfigureAwait(false);
            }
            await jsonWriter.EndArrayScopeAsync().ConfigureAwait(false);
        }

        private async Task WriteGeometryCollectionAsync(IJsonWriter jsonWriter, GeometryCollection collection)
        {
            await jsonWriter.StartArrayScopeAsync().ConfigureAwait(false);

            foreach (Geometry geometry in collection.Geometries)
            {
                await WriteGeometryAsync(jsonWriter, geometry, writeCrs: false).ConfigureAwait(false); // We don't write CRS for geometry in the collection
            }

            await jsonWriter.EndArrayScopeAsync().ConfigureAwait(false);
        }

        private static async Task WriteCoordinatesAsync(IJsonWriter jsonWriter, double x, double y, double z, double m)
        {
            await jsonWriter.StartArrayScopeAsync().ConfigureAwait(false);
            await jsonWriter.WriteValueAsync(x).ConfigureAwait(false);
            await jsonWriter.WriteValueAsync(y).ConfigureAwait(false);
            if (!double.IsNaN(z))
            {
                await jsonWriter.WriteValueAsync(z).ConfigureAwait(false);
            }
            if (!double.IsNaN(m))
            {
                await jsonWriter.WriteValueAsync(m).ConfigureAwait(false);
            }
            await jsonWriter.EndArrayScopeAsync().ConfigureAwait(false);
        }
    }
}

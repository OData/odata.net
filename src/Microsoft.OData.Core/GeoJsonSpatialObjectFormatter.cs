//---------------------------------------------------------------------
// <copyright file="GeoJsonSpatialObjectFormatter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Core;
    using NetTopologySuite;
    using NetTopologySuite.Geometries;

    #endregion

    internal sealed class GeoJsonSpatialObjectFormatter
    {
        private const string GeometryPoint = "Point";
        private const string GeometryLineString = "LineString";
        private const string GeometryPolygon = "Polygon";
        private const string GeometryMultiPoint = "MultiPoint";
        private const string GeometryMultiLineString = "MultiLineString";
        private const string GeometryMultiPolygon = "MultiPolygon";
        private const string GeometryCollection = "GeometryCollection";
        private static GeometryFactory geometryFactory;

        private static readonly Dictionary<string, string> errorFormatStrings = new Dictionary<string, string>()
        {
            { GeometryPoint, SRResources.GeoJsonSpatialObjectFormatter_InvalidGeometryPoint },
            { GeometryLineString, SRResources.GeoJsonSpatialObjectFormatter_InvalidGeometryLineString },
            { GeometryPolygon, SRResources.GeoJsonSpatialObjectFormatter_InvalidGeometryPolygon },
            { GeometryMultiPoint, SRResources.GeoJsonSpatialObjectFormatter_InvalidGeometryMultiPoint },
            { GeometryMultiLineString, SRResources.GeoJsonSpatialObjectFormatter_InvalidGeometryMultiLineString },
            { GeometryMultiPolygon, SRResources.GeoJsonSpatialObjectFormatter_InvalidGeometryMultiPolygon },
            { GeometryCollection, SRResources.GeoJsonSpatialObjectFormatter_InvalidGeometryCollection },
        };

        public static Geometry ParseGeometry(string propertyName, IDictionary<string, object> spatialDict)
        {
            Geometry geometry = null;

            if (spatialDict.TryGetValue("type", out object geomType) && geomType is string geometryType)
            {
                IEnumerable<object> coordinates = null;
                IEnumerable<object> geometries = null;
                if (spatialDict.TryGetValue("coordinates", out object coordinatesObject))
                {
                    coordinates = CastToEnumerable(propertyName, geometryType, coordinatesObject);
                }
                else if (spatialDict.TryGetValue("geometries", out object geometriesObject))
                {
                    // Geometry collection
                    geometries = CastToEnumerable(propertyName, geometryType, geometriesObject);
                }
                else
                {
                    throw new ODataException(SRResources.ODataJsonReaderCoreUtils_CannotReadSpatialPropertyValue);
                }

                if (spatialDict.TryGetValue("crs", out object crsObject) && TryParseEpsgCode(crsObject, out int epsgCode))
                {
                    geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: epsgCode);
                }
                else
                {
                    geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                }

                switch (geometryType)
                {
                    case GeometryPoint:
                        geometry = ParsePoint(propertyName, geometryType, coordinates);

                        break;
                    case GeometryLineString:
                        geometry = ParseLineString(propertyName, geometryType, coordinates);

                        break;
                    case GeometryPolygon:
                        geometry = ParsePolygon(propertyName, geometryType, coordinates);

                        break;
                    case GeometryMultiPoint:
                        geometry = ParseMultiPoint(propertyName, geometryType, coordinates);

                        break;
                    case GeometryMultiLineString:
                        geometry = ParseMultiLineString(propertyName, geometryType, coordinates);

                        break;
                    case GeometryMultiPolygon:
                        geometry = ParseMultiPolygon(propertyName, geometryType, coordinates);

                        break;
                    case GeometryCollection:
                        geometry = ParseGeometryCollection(propertyName, geometryType, geometries);

                        break;
                    default:
                        throw new ODataException(
                            Error.Format(SRResources.GeoJsonSpatialObjectFormatter_UnsupportedGeometryType, geometryType));

                }
            }
            else
            {
                throw new ODataException(SRResources.ODataJsonReaderCoreUtils_CannotReadSpatialPropertyValue);
            }

            return geometry;
        }

        private static bool TryParseEpsgCode(object crsObject, out int epsgCode)
        {
            epsgCode = 0;

            if (crsObject is not Dictionary<string, object> crsDict ||
                !crsDict.TryGetValue("properties", out var propertiesObject) ||
                propertiesObject is not Dictionary<string, object> properties ||
                !properties.TryGetValue("name", out var nameObject) ||
                nameObject is not string name)
            {
                return false;
            }

            // No-op if the string has no leading or trailing whitespace
            name = name.Trim();

            // Matches the following CRS formats:
            // - "EPSG:4326"
            // - "urn:ogc:def:crs:EPSG::4326"
            // - "http://www.opengis.net/def/crs/EPSG/0/3857"
            
            int indexOfLastColon = name.LastIndexOf(':');
            int indexOfLastSlash = name.LastIndexOf('/');
            int indexOfSeparator = Math.Max(indexOfLastColon, indexOfLastSlash);
            if (indexOfSeparator == -1 || indexOfSeparator + 1 >= name.Length)
            {
                return false;
            }

            // Approach avoids use of Regex
            return int.TryParse(name.AsSpan(indexOfSeparator + 1), out epsgCode);
        }

        private static Coordinate ParseCoordinate(string propertyName, string geometryType, IEnumerable<object> coordinates)
        {
            ValidateNotNullOrEmpty(propertyName, geometryType, coordinates);

            const int MinDimensions = 2;
            const int MaxDimensions = 4;
            // Use of array should be more efficient
            double[] coords = new double[MaxDimensions]; // X (Longitude), Y (Latitude), Z (Elevation), M (Measure)
            int i = 0;
            foreach (object coordinate in coordinates)
            {
                // If we already have the expected number of elements in a coordinate, throw an exception - no need to continue parsing
                if (i >= MaxDimensions)
                {
                    throw new ODataException(Error.Format(errorFormatStrings[geometryType], propertyName));
                }

                if (coordinate == null)
                {
                    throw new ODataException(
                        Error.Format(SRResources.GeoJsonSpatialObjectFormatter_CoordinateParsingNullValueError, propertyName));
                }

                try
                {
                    // Use CultureInfo.InvariantCulture since GeoJSON always uses period (.) as decimal separator
                    coords[i++] = Convert.ToDouble(coordinate, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception ex) when (ex is FormatException || ex is InvalidCastException || ex is OverflowException)
                {
                    throw new ODataException(
                        Error.Format(
                            SRResources.GeoJsonSpatialObjectFormatter_CoordinateParsingError,
                            propertyName,
                            string.Concat(ex.GetType().Name, " - ", ex.Message)),
                        ex);
                }
            }

            // Check for too few coordinates
            if (i < MinDimensions)
            {
                throw new ODataException(Error.Format(errorFormatStrings[geometryType], propertyName));
            }

            if (i == MinDimensions)
            {
                return new Coordinate(coords[0], coords[1]);
            }
            else if (i == MinDimensions + 1)
            {
                return new CoordinateZ(coords[0], coords[1], coords[2]);
            }
            else
            {
                return new CoordinateZM(coords[0], coords[1], coords[2], coords[3]);
            }
        }

        private static LinearRing ParseLinearRing(string propertyName, string geometryType, IEnumerable<object> collection)
        {
            ValidateNotNullOrEmpty(propertyName, geometryType, collection);

            const int MinCoordinates = 4;
            const int MinDistinctCoordinates = 3;
            List<Coordinate> coordinates = new List<Coordinate>();
            foreach (object item in collection)
            {
                IEnumerable<object> linearStringCoordinates = CastToEnumerable(propertyName, geometryType, item);
                coordinates.Add(ParseCoordinate(propertyName, geometryType, linearStringCoordinates));
            }

            CoordinateEqualityComparer comparer = new CoordinateEqualityComparer();
            if (!comparer.Equals(coordinates[0], coordinates[coordinates.Count - 1]))
            {
                throw new ODataException(
                    Error.Format(SRResources.GeoJsonSpatialObjectFormatter_PolygonParsingRingNotClosedError, propertyName));
            }

            // LinearRing allows 3 coordinate pairs for technical flexibility, but this is not a valid polygon
            if (coordinates.Count < MinCoordinates)
            {
                throw new ODataException(
                    Error.Format(SRResources.GeoJsonSpatialObjectFormatter_PolygonParsingRingTooFewCoordinatesError, propertyName));
            }

            // Ensure the linear ring contains at least 3 distinct coordinate pairs
            // excluding the closing point which must be equal to the first.
            // Required for the polygon to enclose a valid area.

            // HashSet provides simple and correct distinct-checking,
            // with early exit for efficiency on large coordinate sets - memory overhead is bounded.
            HashSet<Coordinate> distinctCoordinates = new HashSet<Coordinate>(new CoordinateEqualityComparer());
            for (int i = 0; i < coordinates.Count - 1; i++)
            {
                distinctCoordinates.Add(coordinates[i]);

                // Early exit once 3 distinct points found
                if (distinctCoordinates.Count >= 3)
                {
                    break;
                }
            }

            if (distinctCoordinates.Count < MinDistinctCoordinates)
            {
                throw new ODataException(
                    Error.Format(SRResources.GeoJsonSpatialObjectFormatter_PolygonParsingRingTooFewDistinctCoordinatesError, propertyName));
            }

            return new LinearRing(coordinates.ToArray());
        }

        private static Point ParsePoint(string propertyName, string geometryType, IEnumerable<object> coordinates)
        {
            // ParseCoordinate will perform the check for null or empty coordinates collection
            return geometryFactory.CreatePoint(
                ParseCoordinate(propertyName, geometryType, coordinates));
        }

        private static LineString ParseLineString(string propertyName, string geometryType, IEnumerable<object> coordinates)
        {
            ValidateNotNullOrEmpty(propertyName, geometryType, coordinates);

            const int MinCoordinates = 2;
            List<Coordinate> coords = new List<Coordinate>();
            foreach (object coordinate in coordinates)
            {
                IEnumerable<object> lineStringCoordinates = CastToEnumerable(propertyName, geometryType, coordinate);
                coords.Add(ParseCoordinate(propertyName, geometryType, lineStringCoordinates));
            }

            if (coords.Count < MinCoordinates)
            {
                throw new ODataException(
                    Error.Format(SRResources.GeoJsonSpatialObjectFormatter_LineStringParsingTooFewCoordinatesError, propertyName));
            }

            return geometryFactory.CreateLineString(coords.ToArray());
        }

        private static Polygon ParsePolygon(string propertyName, string geometryType, IEnumerable<object> coordinates)
        {
            ValidateNotNullOrEmpty(propertyName, geometryType, coordinates);

            List<LinearRing> rings = new List<LinearRing>();
            foreach (object coordinate in coordinates)
            {
                IEnumerable<object> polygonCoordinates = CastToEnumerable(propertyName, geometryType, coordinate);

                LinearRing ring = ParseLinearRing(propertyName, geometryType, polygonCoordinates);

                rings.Add(ring);
            }

            // TODO: Microsoft.Spatial supports empty polygons
            if (rings.Count == 0)
            {
                throw new ODataException(
                    Error.Format(SRResources.GeoJsonSpatialObjectFormatter_PolygonParsingNoRingError, propertyName));
            }

            LinearRing shell = rings[0];
            LinearRing[] holes = rings.Count > 1 ? rings.GetRange(1, rings.Count - 1).ToArray() : null;

            return geometryFactory.CreatePolygon(shell, holes);
        }

        private static MultiPoint ParseMultiPoint(string propertyName, string geometryType, IEnumerable<object> coordinates)
        {
            // Empty MultiPoint is supported
            ValidateNotNull(propertyName, geometryType, coordinates);

            List<Point> points = new List<Point>();
            foreach (object coordinate in coordinates)
            {
                IEnumerable<object> multiPointCoordinates = CastToEnumerable(propertyName, geometryType, coordinate);
                points.Add(ParsePoint(propertyName, geometryType, multiPointCoordinates));
            }

            return geometryFactory.CreateMultiPoint(points.ToArray());
        }

        private static MultiLineString ParseMultiLineString(string propertyName, string geometryType, IEnumerable<object> coordinates)
        {
            ValidateNotNull(propertyName, geometryType, coordinates);

            List<LineString> lineStrings = new List<LineString>();
            foreach (object coordinate in coordinates)
            {
                IEnumerable<object> multiLineStringCoordinates = CastToEnumerable(propertyName, geometryType, coordinate);
                lineStrings.Add(ParseLineString(propertyName, geometryType, multiLineStringCoordinates));
            }

            return geometryFactory.CreateMultiLineString(lineStrings.ToArray());
        }

        private static MultiPolygon ParseMultiPolygon(string propertyName, string geometryType, IEnumerable<object> coordinates)
        {
            ValidateNotNull(propertyName, geometryType, coordinates);

            List<Polygon> polygons = new List<Polygon>();
            foreach (object coordinate in coordinates)
            {
                IEnumerable<object> multiPolygonCoordinates = CastToEnumerable(propertyName, geometryType, coordinate);
                polygons.Add(ParsePolygon(propertyName, geometryType, multiPolygonCoordinates));
            }

            return geometryFactory.CreateMultiPolygon(polygons.ToArray());
        }

        private static GeometryCollection ParseGeometryCollection(string propertyName, string geometryType, IEnumerable<object> geometries)
        {
            // Empty GeometryCollection supported
            ValidateNotNull(propertyName, geometryType, geometries);

            List<Geometry> geometryList = new List<Geometry>();
            foreach (object geometry in geometries)
            {
                if (geometry is not Dictionary<string, object> geometryDict)
                {
                    throw new ODataException(Error.Format(errorFormatStrings[geometryType], propertyName));
                }

                geometryList.Add(ParseGeometry(propertyName, geometryDict));
            }

            return geometryFactory.CreateGeometryCollection(geometryList.ToArray());
        }

        private static void ValidateNotNull<T>(string propertyName, string geometryType, IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ODataException(Error.Format(errorFormatStrings[geometryType], propertyName));
            }
        }

        private static void ValidateNotNullOrEmpty<T>(string propertyName, string geometryType, IEnumerable<T> collection)
        {
            if (collection == null || !collection.Any())
            {
                throw new ODataException(Error.Format(errorFormatStrings[geometryType], propertyName));
            }
        }

        private static IEnumerable<object> CastToEnumerable(string propertyName, string geometryType, object value)
        {
            if (value is not IEnumerable<object> enumerable)
            {
                throw new ODataException(Error.Format(errorFormatStrings[geometryType], propertyName));
            }

            return enumerable;
        }

        // TODO: Consider making this class public if necessary
        private class CoordinateEqualityComparer : IEqualityComparer<Coordinate>
        {
            public bool Equals(Coordinate a, Coordinate b)
            {
                if (ReferenceEquals(a, b)) return true;
                if (a is null || b is null) return false;

                return AreDoublesEqual(a.X, b.X) &&
                       AreDoublesEqual(a.Y, b.Y) &&
                       AreDoublesEqual(a.Z, b.Z) &&
                       AreDoublesEqual(a.M, b.M);
            }

            public int GetHashCode(Coordinate c)
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 23 + GetNaNCompatibleHash(c.X);
                    hash = hash * 23 + GetNaNCompatibleHash(c.Y);
                    hash = hash * 23 + GetNaNCompatibleHash(c.Z);
                    hash = hash * 23 + GetNaNCompatibleHash(c.M);
                    return hash;
                }
            }

            private static bool AreDoublesEqual(double a, double b)
            {
                return a.Equals(b) || (double.IsNaN(a) && double.IsNaN(b));
            }

            private static int GetNaNCompatibleHash(double value)
            {
                return double.IsNaN(value) ? 0 : value.GetHashCode();
            }
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="GeoJsonSpatialObjectFormatterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using NetTopologySuite.Geometries;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class GeoJsonSpatialObjectFormatterTests
    {
        [Fact]
        public void ParseGeometry_Point_ReturnsExpectedPoint()
        {
            // Arrange
            var pointDict = CreatePointDictionary();

            // Act
            var geometry = GeoJsonSpatialObjectFormatter.ParseGeometry("PointProp", pointDict);

            // Assert
            Assert.NotNull(geometry);
            var point = Assert.IsType<Point>(geometry);
            Assert.Equal(2.2945, point.X, 4); // longitude
            Assert.Equal(48.8584, point.Y, 4); // latitude
            Assert.Equal(4326, point.SRID);
        }

        [Fact]
        public void ParseGeometry_Point3D_ReturnsExpectedPoint()
        {
            // Arrange
            var pointDict = new Dictionary<string, object>
            {
                ["type"] = "Point",
                ["coordinates"] = new List<object> { 2.2945, 48.8584, 100.5 }, // [lon, lat, elevation]
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = "EPSG:4326"
                    }
                }
            };

            // Act
            var geometry = GeoJsonSpatialObjectFormatter.ParseGeometry("PointWithElevationProp", pointDict);

            // Assert
            Assert.NotNull(geometry);
            var point = Assert.IsType<Point>(geometry);
            Assert.Equal(2.2945, point.X, 4);
            Assert.Equal(48.8584, point.Y, 4);
            Assert.Equal(100.5, point.Coordinate.Z, 4);
            Assert.Equal(4326, point.SRID);
        }

        [Fact]
        public void ParseGeometry_Point4D_ReturnsExpectedPoint()
        {
            // Arrange
            var pointDict = new Dictionary<string, object>
            {
                ["type"] = "Point",
                ["coordinates"] = new List<object> { 2.2945, 48.8584, 100.5, 7.7 }, // [lon, lat, elevation, measure]
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = "EPSG:4326"
                    }
                }
            };

            // Act
            var geometry = GeoJsonSpatialObjectFormatter.ParseGeometry("PointWithElevationAndMeasureProp", pointDict);

            // Assert
            Assert.NotNull(geometry);
            var point = Assert.IsType<Point>(geometry);
            Assert.Equal(2.2945, point.X, 4);
            Assert.Equal(48.8584, point.Y, 4);
            Assert.Equal(100.5, point.Coordinate.Z, 4);
            Assert.Equal(7.7, point.Coordinate.M, 4);
            Assert.Equal(4326, point.SRID);
        }

        [Fact]
        public void ParseGeometry_LineString_ReturnsExpectedLineString()
        {
            // Arrange
            var lineStringDict = CreateLineStringDictionary();

            // Act
            var geometry = GeoJsonSpatialObjectFormatter.ParseGeometry("LineStringProp", lineStringDict);

            // Assert
            Assert.NotNull(geometry);
            var lineString = Assert.IsType<LineString>(geometry);
            Assert.Equal(3, lineString.NumPoints);

            Assert.Equal(2.2945, lineString.GetCoordinateN(0).X, 4);
            Assert.Equal(48.8584, lineString.GetCoordinateN(0).Y, 4);

            Assert.Equal(2.3365, lineString.GetCoordinateN(1).X, 4);
            Assert.Equal(48.8610, lineString.GetCoordinateN(1).Y, 4);

            Assert.Equal(2.3376, lineString.GetCoordinateN(2).X, 4);
            Assert.Equal(48.8606, lineString.GetCoordinateN(2).Y, 4);

            Assert.Equal(4326, lineString.SRID);
        }

        [Fact]
        public void ParseGeometry_Polygon_ReturnsExpectedPolygon()
        {
            // Arrange
            var polygonDict = CreatePolygonDictionary();

            // Act
            var geometry = GeoJsonSpatialObjectFormatter.ParseGeometry("PolygonProp", polygonDict);

            // Assert
            Assert.NotNull(geometry);
            var polygon = Assert.IsType<Polygon>(geometry);
            Assert.Equal(4326, polygon.SRID);
            Assert.Equal(6, polygon.ExteriorRing.NumPoints);

            // Check the coordinates of the exterior ring
            var coords = polygon.ExteriorRing.Coordinates;
            Assert.Equal(2.2945, coords[0].X, 4);
            Assert.Equal(48.8584, coords[0].Y, 4);

            Assert.Equal(2.2955, coords[1].X, 4);
            Assert.Equal(48.8589, coords[1].Y, 4);

            Assert.Equal(2.2960, coords[2].X, 4);
            Assert.Equal(48.8594, coords[2].Y, 4);

            Assert.Equal(2.2950, coords[3].X, 4);
            Assert.Equal(48.8600, coords[3].Y, 4);

            Assert.Equal(2.2935, coords[4].X, 4);
            Assert.Equal(48.8590, coords[4].Y, 4);

            Assert.Equal(2.2945, coords[5].X, 4);
            Assert.Equal(48.8584, coords[5].Y, 4);
        }

        [Fact]
        public void ParseGeometry_MultiPoint_ReturnsExpectedMultiPoint()
        {
            // Arrange
            var multiPointDict = CreateMultiPointDictionary();

            // Act
            var geometry = GeoJsonSpatialObjectFormatter.ParseGeometry("MultiPointProp", multiPointDict);

            // Assert
            Assert.NotNull(geometry);
            var multiPoint = Assert.IsType<MultiPoint>(geometry);
            Assert.Equal(3, multiPoint.NumGeometries);
            Assert.Equal(4326, multiPoint.SRID);

            Assert.Equal(2.2945, multiPoint.Geometries[0].Coordinate.X, 4);
            Assert.Equal(48.8584, multiPoint.Geometries[0].Coordinate.Y, 4);

            Assert.Equal(2.3365, multiPoint.Geometries[1].Coordinate.X, 4);
            Assert.Equal(48.8610, multiPoint.Geometries[1].Coordinate.Y, 4);

            Assert.Equal(2.3376, multiPoint.Geometries[2].Coordinate.X, 4);
            Assert.Equal(48.8606, multiPoint.Geometries[2].Coordinate.Y, 4);
        }

        [Fact]
        public void ParseGeometry_WithEmptyCoordinates_ReturnsEmptyMultiPoint()
        {
            // Arrange
            var multiPointDict = new Dictionary<string, object>
            {
                ["type"] = "MultiPoint",
                ["coordinates"] = new List<object>(),
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = "EPSG:4326"
                    }
                }
            };

            // Act
            var geometry = GeoJsonSpatialObjectFormatter.ParseGeometry("MultiPointProp", multiPointDict);

            // Assert
            Assert.NotNull(geometry);
            var multiPoint = Assert.IsType<MultiPoint>(geometry);
            Assert.Equal(0, multiPoint.NumGeometries);
            Assert.Equal(4326, multiPoint.SRID);
            Assert.Empty(multiPoint.Geometries);
        }

        [Fact]
        public void ParseGeometry_MultiLineString_ReturnsExpectedMultiLineString()
        {
            // Arrange
            var multiLineStringDict = CreateMultiLineStringDictionary();

            // Act
            var geometry = GeoJsonSpatialObjectFormatter.ParseGeometry("MultiLineStringProp", multiLineStringDict);

            // Assert
            Assert.NotNull(geometry);
            var multiLineString = Assert.IsType<MultiLineString>(geometry);
            Assert.Equal(2, multiLineString.NumGeometries);
            Assert.Equal(4326, multiLineString.SRID);

            // First LineString
            var line1 = Assert.IsType<LineString>(multiLineString.Geometries[0]);
            Assert.Equal(3, line1.NumPoints);
            Assert.Equal(2.2945, line1.GetCoordinateN(0).X, 4);
            Assert.Equal(48.8584, line1.GetCoordinateN(0).Y, 4);
            Assert.Equal(2.3365, line1.GetCoordinateN(1).X, 4);
            Assert.Equal(48.8610, line1.GetCoordinateN(1).Y, 4);
            Assert.Equal(2.3376, line1.GetCoordinateN(2).X, 4);
            Assert.Equal(48.8606, line1.GetCoordinateN(2).Y, 4);

            // Second LineString
            var line2 = Assert.IsType<LineString>(multiLineString.Geometries[1]);
            Assert.Equal(3, line2.NumPoints);
            Assert.Equal(2.2950, line2.GetCoordinateN(0).X, 4);
            Assert.Equal(48.8738, line2.GetCoordinateN(0).Y, 4);
            Assert.Equal(2.3070, line2.GetCoordinateN(1).X, 4);
            Assert.Equal(48.8698, line2.GetCoordinateN(1).Y, 4);
            Assert.Equal(2.3212, line2.GetCoordinateN(2).X, 4);
            Assert.Equal(48.8656, line2.GetCoordinateN(2).Y, 4);
        }

        [Fact]
        public void ParseGeometry_WithEmptyCoordinates_ReturnsEmptyMultiLineString()
        {
            // Arrange
            var multiLineStringDict = new Dictionary<string, object>
            {
                ["type"] = "MultiLineString",
                ["coordinates"] = new List<object>(),
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = "EPSG:4326"
                    }
                }
            };

            // Act
            var geometry = GeoJsonSpatialObjectFormatter.ParseGeometry("MultiLineStringProp", multiLineStringDict);

            // Assert
            Assert.NotNull(geometry);
            var multiLineString = Assert.IsType<MultiLineString>(geometry);
            Assert.Equal(0, multiLineString.NumGeometries);
            Assert.Equal(4326, multiLineString.SRID);
            Assert.Empty(multiLineString.Geometries);
        }

        [Fact]
        public void ParseGeometry_MultiPolygon_ReturnsExpectedMultiPolygon()
        {
            // Arrange
            var multiPolygonDict = CreateMultiPolygonDictionary();

            // Act
            var geometry = GeoJsonSpatialObjectFormatter.ParseGeometry("MultiPolygonProp", multiPolygonDict);

            // Assert
            Assert.NotNull(geometry);
            var multiPolygon = Assert.IsType<MultiPolygon>(geometry);
            Assert.Equal(2, multiPolygon.NumGeometries);
            Assert.Equal(4326, multiPolygon.SRID);

            // First polygon (pentagon)
            var polygon1 = Assert.IsType<Polygon>(multiPolygon.Geometries[0]);
            Assert.Equal(6, polygon1.ExteriorRing.NumPoints);
            var coords1 = polygon1.ExteriorRing.Coordinates;
            Assert.Equal(2.2945, coords1[0].X, 4);
            Assert.Equal(48.8584, coords1[0].Y, 4);
            Assert.Equal(2.2955, coords1[1].X, 4);
            Assert.Equal(48.8589, coords1[1].Y, 4);
            Assert.Equal(2.2960, coords1[2].X, 4);
            Assert.Equal(48.8594, coords1[2].Y, 4);
            Assert.Equal(2.2950, coords1[3].X, 4);
            Assert.Equal(48.8600, coords1[3].Y, 4);
            Assert.Equal(2.2935, coords1[4].X, 4);
            Assert.Equal(48.8590, coords1[4].Y, 4);
            Assert.Equal(2.2945, coords1[5].X, 4);
            Assert.Equal(48.8584, coords1[5].Y, 4);

            // Second polygon (triangle)
            var polygon2 = Assert.IsType<Polygon>(multiPolygon.Geometries[1]);
            Assert.Equal(4, polygon2.ExteriorRing.NumPoints);
            var coords2 = polygon2.ExteriorRing.Coordinates;
            Assert.Equal(2.3376, coords2[0].X, 4);
            Assert.Equal(48.8606, coords2[0].Y, 4);
            Assert.Equal(2.3365, coords2[1].X, 4);
            Assert.Equal(48.8610, coords2[1].Y, 4);
            Assert.Equal(2.3350, coords2[2].X, 4);
            Assert.Equal(48.8600, coords2[2].Y, 4);
            Assert.Equal(2.3376, coords2[3].X, 4);
            Assert.Equal(48.8606, coords2[3].Y, 4);
        }

        [Fact]
        public void ParseGeometry_WithEmptyCoordinates_ReturnsEmptyMultiPolygon()
        {
            // Arrange
            var multiPolygonDict = new Dictionary<string, object>
            {
                ["type"] = "MultiPolygon",
                ["coordinates"] = new List<object>(),
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = "EPSG:4326"
                    }
                }
            };

            // Act
            var geometry = GeoJsonSpatialObjectFormatter.ParseGeometry("MultiPolygonProp", multiPolygonDict);

            // Assert
            Assert.NotNull(geometry);
            var multiPolygon = Assert.IsType<MultiPolygon>(geometry);
            Assert.Equal(0, multiPolygon.NumGeometries);
            Assert.Equal(4326, multiPolygon.SRID);
            Assert.Empty(multiPolygon.Geometries);
        }

        [Fact]
        public void ParseGeometry_GeometryCollection_ReturnsExpectedGeometryCollection()
        {
            // Arrange
            var geometryCollectionDict = new Dictionary<string, object>
            {
                ["type"] = "GeometryCollection",
                ["geometries"] = new List<object>
                {
                    CreatePointDictionary(),
                    CreateLineStringDictionary(),
                    CreatePolygonDictionary(),
                    CreateMultiPointDictionary(),
                    CreateMultiLineStringDictionary(),
                    CreateMultiPolygonDictionary()
                },
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = "EPSG:4326"
                    }
                }
            };

            // Act
            var geometry = GeoJsonSpatialObjectFormatter.ParseGeometry("GeometryCollectionProp", geometryCollectionDict);

            // Assert
            Assert.NotNull(geometry);
            var collection = Assert.IsType<GeometryCollection>(geometry);
            Assert.Equal(6, collection.NumGeometries);
            Assert.Equal(4326, collection.SRID);

            Assert.IsType<Point>(collection.Geometries[0]);
            Assert.IsType<LineString>(collection.Geometries[1]);
            Assert.IsType<Polygon>(collection.Geometries[2]);
            Assert.IsType<MultiPoint>(collection.Geometries[3]);
            Assert.IsType<MultiLineString>(collection.Geometries[4]);
            Assert.IsType<MultiPolygon>(collection.Geometries[5]);

            Assert.Equal(1, ((Point)collection.Geometries[0]).NumPoints);
            Assert.Equal(3, ((LineString)collection.Geometries[1]).NumPoints);
            Assert.Equal(6, ((Polygon)collection.Geometries[2]).ExteriorRing.NumPoints);
            Assert.Equal(3, ((MultiPoint)collection.Geometries[3]).NumGeometries);
            Assert.Equal(2, ((MultiLineString)collection.Geometries[4]).NumGeometries);
            Assert.Equal(2, ((MultiPolygon)collection.Geometries[5]).NumGeometries);
        }

        [Fact]
        public void ParseGeometry_WithEmptyCoordinates_ReturnsEmptyGeometryCollection()
        {
            // Arrange
            var multiPolygonDict = new Dictionary<string, object>
            {
                ["type"] = "GeometryCollection",
                ["geometries"] = new List<object>(),
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = "EPSG:4326"
                    }
                }
            };

            // Act
            var geometry = GeoJsonSpatialObjectFormatter.ParseGeometry("GeometryCollectionProp", multiPolygonDict);

            // Assert
            Assert.NotNull(geometry);
            var geometryCollection = Assert.IsType<GeometryCollection>(geometry);
            Assert.Equal(0, geometryCollection.NumGeometries);
            Assert.Equal(4326, geometryCollection.SRID);
            Assert.Empty(geometryCollection.Geometries);
        }

        [Fact]
        public void ParseGeometry_PolygonWithHoles_ReturnsExpectedPolygonWithInteriorRings()
        {
            // Arrange
            var polygonWithHolesDict = CreatePolygonWithHolesDictionary();

            // Act
            var geometry = GeoJsonSpatialObjectFormatter.ParseGeometry("PolygonWithHolesProp", polygonWithHolesDict);

            // Assert
            Assert.NotNull(geometry);
            var polygon = Assert.IsType<Polygon>(geometry);
            Assert.Equal(4326, polygon.SRID);

            // Exterior ring
            Assert.Equal(6, polygon.ExteriorRing.NumPoints);

            // Interior rings (holes)
            Assert.Equal(2, polygon.NumInteriorRings);

            // First hole (triangle)
            var hole1 = polygon.GetInteriorRingN(0);
            Assert.Equal(4, hole1.NumPoints);
            var hole1Coords = hole1.Coordinates;
            Assert.Equal(2.2950, hole1Coords[0].X, 4);
            Assert.Equal(48.8587, hole1Coords[0].Y, 4);
            Assert.Equal(2.2952, hole1Coords[1].X, 4);
            Assert.Equal(48.8588, hole1Coords[1].Y, 4);
            Assert.Equal(2.2948, hole1Coords[2].X, 4);
            Assert.Equal(48.8588, hole1Coords[2].Y, 4);
            Assert.Equal(2.2950, hole1Coords[3].X, 4);
            Assert.Equal(48.8587, hole1Coords[3].Y, 4);

            // Second hole (quadrilateral)
            var hole2 = polygon.GetInteriorRingN(1);
            Assert.Equal(5, hole2.NumPoints);
            var hole2Coords = hole2.Coordinates;
            Assert.Equal(2.2947, hole2Coords[0].X, 4);
            Assert.Equal(48.8586, hole2Coords[0].Y, 4);
            Assert.Equal(2.2949, hole2Coords[1].X, 4);
            Assert.Equal(48.8587, hole2Coords[1].Y, 4);
            Assert.Equal(2.2948, hole2Coords[2].X, 4);
            Assert.Equal(48.8589, hole2Coords[2].Y, 4);
            Assert.Equal(2.2946, hole2Coords[3].X, 4);
            Assert.Equal(48.8588, hole2Coords[3].Y, 4);
            Assert.Equal(2.2947, hole2Coords[4].X, 4);
            Assert.Equal(48.8586, hole2Coords[4].Y, 4);
        }

        [Fact]
        public void ParseGeometry_WithNoCrsProperty()
        {
            var pointDict = new Dictionary<string, object>
            {
                ["type"] = "Point",
                ["coordinates"] = new List<object> { 2.2945, 48.8584 }
            };

            var geometry = GeoJsonSpatialObjectFormatter.ParseGeometry("PointProp", pointDict);

            Assert.NotNull(geometry);
            Assert.IsType<Point>(geometry);
            Assert.Equal(4326, geometry.SRID);
        }

        [Theory]
        [InlineData("EPSG:4326", 4326)]
        [InlineData("urn:ogc:def:crs:EPSG::4326", 4326)]
        [InlineData("http://www.opengis.net/def/crs/EPSG/0/3857", 3857)]
        public void ParseGeometry_WithVariousEpsgFormats_SetsExpectedSrid(string crsName, int expectedSrid)
        {
            var pointDict = new Dictionary<string, object>
            {
                ["type"] = "Point",
                ["coordinates"] = new List<object> { 2.2945, 48.8584 },
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = crsName
                    }
                }
            };

            var geometry = GeoJsonSpatialObjectFormatter.ParseGeometry("PointProp", pointDict);

            Assert.NotNull(geometry);
            Assert.IsType<Point>(geometry);
            Assert.Equal(expectedSrid, geometry.SRID);
        }

        [Theory]
        [InlineData("urn:ogc:def:crs:OGC:1.3:CRS84")]
        [InlineData("http://www.opengis.net/def/crs/OGC/1.3/CRS84")]
        public void ParseGeometry_WithNonEpsgCrs_UsesDefaultSrid(string crsName)
        {
            var pointDict = new Dictionary<string, object>
            {
                ["type"] = "Point",
                ["coordinates"] = new List<object> { 2.2945, 48.8584 },
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = crsName
                    }
                }
            };

            var geometry = GeoJsonSpatialObjectFormatter.ParseGeometry("PointProp", pointDict);

            Assert.NotNull(geometry);
            Assert.IsType<Point>(geometry);
            Assert.Equal(4326, geometry.SRID); // Default SRID should be used
        }

        [Fact]
        public void ParseGeometry_WithMalformedCrs_UsesDefaultSrid()
        {
            // Arrange: CRS property is present but does not match the expected structure
            var pointDict = new Dictionary<string, object>
            {
                ["type"] = "Point",
                ["coordinates"] = new List<object> { 2.2945, 48.8584 },
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    // Missing "properties" or "name" inside "properties"
                }
            };

            // Act
            var geometry = GeoJsonSpatialObjectFormatter.ParseGeometry("PointProp", pointDict);

            // Assert
            Assert.NotNull(geometry);
            Assert.IsType<Point>(geometry);
            Assert.Equal(4326, geometry.SRID); // Default SRID should be used
        }

        #region Unhappy Path Tests

        [Fact]
        public void ParseGeometry_Point_EmptyCoordinates_ThrowsODataException()
        {
            var pointDict = new Dictionary<string, object>
            {
                ["type"] = "Point",
                ["coordinates"] = new List<object>(),
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = "EPSG:4326"
                    }
                }
            };

            var exception = Assert.Throws<ODataException>(() =>
                GeoJsonSpatialObjectFormatter.ParseGeometry("PointProp", pointDict));
            Assert.Equal(
                "Invalid 'PointProp' property: expected 'coordinates' to be [x, y] for geometry type 'Point', optionally with elevation [x, y, z], or with elevation and measure [x, y, z, m].",
                exception.Message);
        }

        [Fact]
        public void ParseGeometry_Point_TooFewCoordinates_ThrowsODataException()
        {
            var pointDict = new Dictionary<string, object>
            {
                ["type"] = "Point",
                ["coordinates"] = new List<object> { 2.2945 },
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = "EPSG:4326"
                    }
                }
            };

            var exception = Assert.Throws<ODataException>(() =>
                GeoJsonSpatialObjectFormatter.ParseGeometry("PointProp", pointDict));
            Assert.Equal(
                "Invalid 'PointProp' property: expected 'coordinates' to be [x, y] for geometry type 'Point', optionally with elevation [x, y, z], or with elevation and measure [x, y, z, m].",
                exception.Message);
        }

        [Fact]
        public void ParseGeometry_Point_TooManyCoordinates_ThrowsODataException()
        {
            var pointDict = new Dictionary<string, object>
            {
                ["type"] = "Point",
                // 6 values: [lon, lat, elevation, measure, extra]
                ["coordinates"] = new List<object> { 2.2945, 48.8584, 100.5, 7.7, 123.45 },
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = "EPSG:4326"
                    }
                }
            };

            var ex = Assert.Throws<ODataException>(() =>
                GeoJsonSpatialObjectFormatter.ParseGeometry("PointProp", pointDict));
            Assert.Equal(
                "Invalid 'PointProp' property: expected 'coordinates' to be [x, y] for geometry type 'Point', optionally with elevation [x, y, z], or with elevation and measure [x, y, z, m].",
                ex.Message);
        }

        [Fact]
        public void ParseGeometry_Point_CoordinateWithFormatException_ThrowsODataException()
        {
            var pointDict = new Dictionary<string, object>
            {
                ["type"] = "Point",
                ["coordinates"] = new List<object> { "not-a-number", 48.8584 },
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = "EPSG:4326"
                    }
                }
            };

            var ex = Assert.Throws<ODataException>(() =>
                GeoJsonSpatialObjectFormatter.ParseGeometry("PointProp", pointDict));
            Assert.Equal("Invalid 'PointProp' property: could not convert the coordinate value to double. " +
                "Error: FormatException - The input string 'not-a-number' was not in a correct format.", ex.Message);
        }

        [Fact]
        public void ParseGeometry_Point_CoordinateWithInvalidCastException_ThrowsODataException()
        {
            var pointDict = new Dictionary<string, object>
            {
                ["type"] = "Point",
                ["coordinates"] = new List<object> { new List<object>(), 48.8584 },
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = "EPSG:4326"
                    }
                }
            };

            var ex = Assert.Throws<ODataException>(() =>
                GeoJsonSpatialObjectFormatter.ParseGeometry("PointProp", pointDict));
            Assert.Equal("Invalid 'PointProp' property: could not convert the coordinate value to double. " +
                "Error: InvalidCastException - Unable to cast object of type 'System.Collections.Generic.List`1[System.Object]' to type 'System.IConvertible'.", ex.Message);
        }

        [Fact]
        public void ParseGeometry_Point_CoordinateWithNullValue_ThrowsODataException()
        {
            var pointDict = new Dictionary<string, object>
            {
                ["type"] = "Point",
                ["coordinates"] = new List<object> { null, 48.8584 },
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = "EPSG:4326"
                    }
                }
            };

            var ex = Assert.Throws<ODataException>(() =>
                GeoJsonSpatialObjectFormatter.ParseGeometry("PointProp", pointDict));
            Assert.Equal("Invalid 'PointProp' property: a null value was found in a coordinate pair.", ex.Message);
        }

        [Fact]
        public void ParseGeometry_Point_CoordinatesNotEnumerable_ThrowsODataException()
        {
            var pointDict = new Dictionary<string, object>
            {
                ["type"] = "Point",
                ["coordinates"] = 2.2945, // Not an IEnumerable<object>
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = "EPSG:4326"
                    }
                }
            };

            var ex = Assert.Throws<ODataException>(() =>
                GeoJsonSpatialObjectFormatter.ParseGeometry("PointProp", pointDict));
            Assert.Equal(
                "Invalid 'PointProp' property: expected 'coordinates' to be [x, y] for geometry type 'Point', optionally with elevation [x, y, z], or with elevation and measure [x, y, z, m].",
                ex.Message);
        }

        [Fact]
        public void ParseGeometry_LineString_TooFewCoordinates_ThrowsODataException()
        {
            var lineStringDict = new Dictionary<string, object>
            {
                ["type"] = "LineString",
                ["coordinates"] = new List<object>
                {
                    new List<object> { 2.2945, 48.8584 } // Only one coordinate pair
                },
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = "EPSG:4326"
                    }
                }
            };

            var ex = Assert.Throws<ODataException>(() =>
                GeoJsonSpatialObjectFormatter.ParseGeometry("LineStringProp", lineStringDict));
            Assert.Equal("Invalid 'LineStringProp' property: LineString must contain two or more coordinate pairs.", ex.Message);
        }

        [Fact]
        public void ParseGeometry_LineString_CoordinatesNotEnumerable_ThrowsODataException()
        {
            var lineStringDict = new Dictionary<string, object>
            {
                ["type"] = "LineString",
                ["coordinates"] = 2.2945, // Not an IEnumerable<object>
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = "EPSG:4326"
                    }
                }
            };

            var ex = Assert.Throws<ODataException>(() =>
                GeoJsonSpatialObjectFormatter.ParseGeometry("LineStringProp", lineStringDict));
            Assert.Equal(
                "Invalid 'LineStringProp' property: expected 'coordinates' to be an array of positions for geometry type 'LineString' (e.g., [[x1, y1], [x2, y2]]).",
                ex.Message);
        }

        [Fact]
        public void ParseGeometry_Polygon_ExteriorRingTooFewCoordinates_ThrowsODataException()
        {
            var polygonDict = new Dictionary<string, object>
            {
                ["type"] = "Polygon",
                ["coordinates"] = new List<object>
                {
                    // Exterior ring with only 3 coordinate pairs (should be at least 4)
                    new List<object>
                    {
                        new List<object> { 2.2945, 48.8584 },
                        new List<object> { 2.2955, 48.8589 },
                        new List<object> { 2.2945, 48.8584 }
                    }
                },
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = "EPSG:4326"
                    }
                }
            };

            var ex = Assert.Throws<ODataException>(() =>
                GeoJsonSpatialObjectFormatter.ParseGeometry("PolygonProp", polygonDict));
            Assert.Equal(
                "Invalid 'PolygonProp' property: ring of Polygon must contain at least 4 coordinate pairs.",
                ex.Message);
        }

        [Fact]
        public void ParseGeometry_Polygon_ExteriorRingNotClosed_ThrowsODataException()
        {
            var polygonDict = new Dictionary<string, object>
            {
                ["type"] = "Polygon",
                ["coordinates"] = new List<object>
                {
                    // Exterior ring with 5 coordinate pairs, but not closed (first != last)
                    new List<object>
                    {
                        new List<object> { 2.2945, 48.8584 },
                        new List<object> { 2.2955, 48.8589 },
                        new List<object> { 2.2960, 48.8594 },
                        new List<object> { 2.2950, 48.8600 },
                        new List<object> { 2.2935, 48.8590 },
                    }
                },
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = "EPSG:4326"
                    }
                }
            };

            var ex = Assert.Throws<ODataException>(() =>
                GeoJsonSpatialObjectFormatter.ParseGeometry("PolygonProp", polygonDict));
            Assert.Equal(
                "Invalid 'PolygonProp' property: ring of Polygon must be closed — the first and last coordinate pairs must be exactly equal.",
                ex.Message);
        }

        [Fact]
        public void ParseGeometry_Polygon_InteriorRingTooFewCoordinates_ThrowsODataException()
        {
            var polygonDict = new Dictionary<string, object>
            {
                ["type"] = "Polygon",
                ["coordinates"] = new List<object>
                {
                    // Valid exterior ring (closed, 6 points)
                    new List<object>
                    {
                        new List<object> { 2.2945, 48.8584 },
                        new List<object> { 2.2955, 48.8589 },
                        new List<object> { 2.2960, 48.8594 },
                        new List<object> { 2.2950, 48.8600 },
                        new List<object> { 2.2935, 48.8590 },
                        new List<object> { 2.2945, 48.8584 }
                    },
                    // Invalid interior ring (only 3 points, not enough for a ring)
                    new List<object>
                    {
                        new List<object> { 2.2947, 48.8586 },
                        new List<object> { 2.2949, 48.8587 },
                        new List<object> { 2.2947, 48.8586 }
                    }
                },
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = "EPSG:4326"
                    }
                }
            };

            var ex = Assert.Throws<ODataException>(() =>
                GeoJsonSpatialObjectFormatter.ParseGeometry("PolygonProp", polygonDict));
            Assert.Equal(
                "Invalid 'PolygonProp' property: ring of Polygon must contain at least 4 coordinate pairs.",
                ex.Message);
        }

        [Fact]
        public void ParseGeometry_Polygon_ExteriorRingTooFewDistinctCoordinates_ThrowsODataException()
        {
            var polygonDict = new Dictionary<string, object>
            {
                ["type"] = "Polygon",
                ["coordinates"] = new List<object>
                {
                    // Invalid exterior ring (only 2 distinct points)
                    new List<object>
                    {
                        new List<object> { 2.2945, 48.8584 },
                        new List<object> { 2.2955, 48.8589 },
                        new List<object> { 2.2955, 48.8589 },
                        new List<object> { 2.2945, 48.8584 }
                    }
                },
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = "EPSG:4326"
                    }
                }
            };

            var ex = Assert.Throws<ODataException>(() =>
                GeoJsonSpatialObjectFormatter.ParseGeometry("PolygonProp", polygonDict));
            Assert.Equal(
                "Invalid 'PolygonProp' property: ring of Polygon must contain at least 3 distinct coordinate pairs.",
                ex.Message);
        }

        [Fact]
        public void ParseGeometry_Polygon_InteriorRingNotClosed_ThrowsODataException()
        {
            var polygonDict = new Dictionary<string, object>
            {
                ["type"] = "Polygon",
                ["coordinates"] = new List<object>
                {
                    // Valid exterior ring (closed, 6 points)
                    new List<object>
                    {
                        new List<object> { 2.2945, 48.8584 },
                        new List<object> { 2.2955, 48.8589 },
                        new List<object> { 2.2960, 48.8594 },
                        new List<object> { 2.2950, 48.8600 },
                        new List<object> { 2.2935, 48.8590 },
                        new List<object> { 2.2945, 48.8584 }
                    },
                    // Invalid interior ring (5 points, not closed)
                    new List<object>
                    {
                        new List<object> { 2.2947, 48.8586 },
                        new List<object> { 2.2949, 48.8587 },
                        new List<object> { 2.2948, 48.8589 },
                        new List<object> { 2.2946, 48.8588 },
                        new List<object> { 2.2948, 48.8587 } // Does not match first
                    }
                },
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = "EPSG:4326"
                    }
                }
            };

            var ex = Assert.Throws<ODataException>(() =>
                GeoJsonSpatialObjectFormatter.ParseGeometry("PolygonProp", polygonDict));
            Assert.Equal(
                "Invalid 'PolygonProp' property: ring of Polygon must be closed — the first and last coordinate pairs must be exactly equal.",
                ex.Message);
        }

        [Fact]
        public void ParseGeometry_Polygon_CoordinatesNotEnumerable_ThrowsODataException()
        {
            var polygonDict = new Dictionary<string, object>
            {
                ["type"] = "Polygon",
                ["coordinates"] = 2.2945, // Not an IEnumerable<object>
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = "EPSG:4326"
                    }
                }
            };

            var ex = Assert.Throws<ODataException>(() =>
                GeoJsonSpatialObjectFormatter.ParseGeometry("PolygonProp", polygonDict));
            Assert.Equal(
                "Invalid 'PolygonProp' property: expected 'coordinates' to be an array of linear rings for geometry type 'Polygon' (e.g., [[[x1, y1], [x2, y2], [x3, y3], [x1, y1]]]).",
                ex.Message);
        }

        [Fact]
        public void ParseGeometry_MultiPoint_CoordinatesNotEnumerable_ThrowsODataException()
        {
            var multiPointDict = new Dictionary<string, object>
            {
                ["type"] = "MultiPoint",
                ["coordinates"] = 2.2945, // Not an IEnumerable<object>
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = "EPSG:4326"
                    }
                }
            };

            var ex = Assert.Throws<ODataException>(() =>
                GeoJsonSpatialObjectFormatter.ParseGeometry("MultiPointProp", multiPointDict));
            Assert.Equal(
                "Invalid 'MultiPointProp' property: expected 'coordinates' to be an array of points for geometry type 'MultiPoint' (e.g., [[x1, y1], [x2, y2]]).",
                ex.Message);
        }

        [Fact]
        public void ParseGeometry_MultiLineString_CoordinatesNotEnumerable_ThrowsODataException()
        {
            var multiLineStringDict = new Dictionary<string, object>
            {
                ["type"] = "MultiLineString",
                ["coordinates"] = 2.2945, // Not an IEnumerable<object>
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = "EPSG:4326"
                    }
                }
            };

            var ex = Assert.Throws<ODataException>(() =>
                GeoJsonSpatialObjectFormatter.ParseGeometry("MultiLineStringProp", multiLineStringDict));
            Assert.Equal(
                "Invalid 'MultiLineStringProp' property: expected 'coordinates' to be an array of line strings for geometry type 'MultiLineString' (e.g., [[[x1, y1], [x2, y2]], [[x3, y3], [x4, y4]]]).",
                ex.Message);
        }

        [Fact]
        public void ParseGeometry_MultiPolygon_CoordinatesNotEnumerable_ThrowsODataException()
        {
            var multiPolygonDict = new Dictionary<string, object>
            {
                ["type"] = "MultiPolygon",
                ["coordinates"] = 2.2945, // Not an IEnumerable<object>
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = "EPSG:4326"
                    }
                }
            };

            var ex = Assert.Throws<ODataException>(() =>
                GeoJsonSpatialObjectFormatter.ParseGeometry("MultiPolygonProp", multiPolygonDict));
            Assert.Equal(
                "Invalid 'MultiPolygonProp' property: expected 'coordinates' to be an array of polygons for geometry type 'MultiPolygon' (e.g., [[[[x1, y1], [x2, y2], [x3, y3], [x1, y1]]], [[[x4, y4], [x5, y5], [x6, y6], [x4, y4]]]]).",
                ex.Message);
        }

        [Fact]
        public void ParseGeometry_GeometryCollection_CoordinatesNotEnumerable_ThrowsODataException()
        {
            var geometryCollectionDict = new Dictionary<string, object>
            {
                ["type"] = "GeometryCollection",
                ["coordinates"] = 2.2945, // Not an IEnumerable<object>
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = "EPSG:4326"
                    }
                }
            };

            var ex = Assert.Throws<ODataException>(() =>
                GeoJsonSpatialObjectFormatter.ParseGeometry("GeometryCollectionProp", geometryCollectionDict));
            Assert.Equal(
                "Invalid 'GeometryCollectionProp' property: expected 'geometries' to be an array of valid geometry objects for geometry type 'GeometryCollection' (e.g., Point, LineString, Polygon, etc.).",
                ex.Message);
        }

        [Theory]
        [InlineData("Point")]
        [InlineData("LineString")]
        [InlineData("Polygon")]
        [InlineData("MultiPoint")]
        [InlineData("MultiLineString")]
        [InlineData("MultiPolygon")]
        [InlineData("GeometryCollection")]
        public void ParseGeometry_InvalidGeometry_MissingCoordinatesAndGeometries_ThrowsODataException(string geometryType)
        {
            var invalidGeometryDict = new Dictionary<string, object>
            {
                ["type"] = geometryType, // or any valid geometry type
                // No "coordinates" or "geometries" property
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = "EPSG:4326"
                    }
                }
            };

            var ex = Assert.Throws<ODataException>(() =>
                GeoJsonSpatialObjectFormatter.ParseGeometry("PointProp", invalidGeometryDict));
            Assert.Equal(
                "The value specified for the spatial property was not valid. You must specify a valid spatial value.",
                ex.Message);
        }

        [Fact]
        public void ParseGeometry_InvalidGeometryType_ThrowsODataException()
        {
            var invalidGeometryDict = new Dictionary<string, object>
            {
                ["type"] = "Circle",
                ["coordinates"] = new List<object> { 2.2945, 48.8584 },
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = "EPSG:4326"
                    }
                }
            };

            var ex = Assert.Throws<ODataException>(() =>
                GeoJsonSpatialObjectFormatter.ParseGeometry("CircleProp", invalidGeometryDict));
            Assert.Equal(
                "Unsupported geometry type 'Circle': Supported types are Point, Polygon, LineString, MultiPoint, MultiPolygon, MultiLineString, and GeometryCollection.",
                ex.Message);
        }

        [Fact]
        public void ParseGeometry_MissingGeometryType_ThrowsODataException()
        {
            var dict = new Dictionary<string, object>
            {
                // "type" property is missing
                ["coordinates"] = new List<object> { 2.2945, 48.8584 },
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = "EPSG:4326"
                    }
                }
            };

            var ex = Assert.Throws<ODataException>(() =>
                GeoJsonSpatialObjectFormatter.ParseGeometry("MissingTypeProp", dict));
            Assert.Equal(
                "The value specified for the spatial property was not valid. You must specify a valid spatial value.",
                ex.Message);
        }

        [Theory]
        [InlineData(123)]
        [InlineData(true)]
        public void ParseGeometry_NonGeometryStringType_ThrowsODataException(object geometryType)
        {
            var dict = new Dictionary<string, object>
            {
                ["type"] = geometryType,
                ["coordinates"] = new List<object> { 2.2945, 48.8584 },
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = "EPSG:4326"
                    }
                }
            };

            var ex = Assert.Throws<ODataException>(() =>
                GeoJsonSpatialObjectFormatter.ParseGeometry("NonStringTypeProp", dict));
            Assert.Equal(
                "The value specified for the spatial property was not valid. You must specify a valid spatial value.",
                ex.Message);
        }

        #endregion Unhappy Path Tests

        private static Dictionary<string, object> CreatePointDictionary()
        {
            return new Dictionary<string, object>
            {
                ["type"] = "Point",
                ["coordinates"] = new List<object> { 2.2945, 48.8584 }, // [lon, lat]
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = "EPSG:4326"
                    }
                }
            };
        }

        private static Dictionary<string, object> CreateLineStringDictionary()
        {
            return new Dictionary<string, object>
            {
                ["type"] = "LineString",
                ["coordinates"] = new List<object>
                {
                    new List<object> { 2.2945, 48.8584 },
                    new List<object> { 2.3365, 48.8610 },
                    new List<object> { 2.3376, 48.8606 }
                },
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = "EPSG:4326"
                    }
                }
            };
        }

        private static Dictionary<string, object> CreatePolygonDictionary()
        {
            return new Dictionary<string, object>
            {
                ["type"] = "Polygon",
                ["coordinates"] = new List<object>
                {
                    new List<object>
                    {
                        new List<object> { 2.2945, 48.8584 },
                        new List<object> { 2.2955, 48.8589 },
                        new List<object> { 2.2960, 48.8594 },
                        new List<object> { 2.2950, 48.8600 },
                        new List<object> { 2.2935, 48.8590 },
                        new List<object> { 2.2945, 48.8584 }
                    }
                },
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = "EPSG:4326"
                    }
                }
            };
        }

        private static Dictionary<string, object> CreateMultiPointDictionary()
        {
            return new Dictionary<string, object>
            {
                ["type"] = "MultiPoint",
                ["coordinates"] = new List<object>
                {
                    new List<object> { 2.2945, 48.8584 },
                    new List<object> { 2.3365, 48.8610 },
                    new List<object> { 2.3376, 48.8606 }
                },
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = "EPSG:4326"
                    }
                }
            };
        }

        private static Dictionary<string, object> CreateMultiLineStringDictionary()
        {
            return new Dictionary<string, object>
            {
                ["type"] = "MultiLineString",
                ["coordinates"] = new List<object>
                {
                    new List<object>
                    {
                        new List<object> { 2.2945, 48.8584 },
                        new List<object> { 2.3365, 48.8610 },
                        new List<object> { 2.3376, 48.8606 }
                    },
                    new List<object>
                    {
                        new List<object> { 2.2950, 48.8738 },
                        new List<object> { 2.3070, 48.8698 },
                        new List<object> { 2.3212, 48.8656 }
                    }
                },
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = "EPSG:4326"
                    }
                }
            };
        }

        private static Dictionary<string, object> CreateMultiPolygonDictionary()
        {
            return new Dictionary<string, object>
            {
                ["type"] = "MultiPolygon",
                ["coordinates"] = new List<object>
                {
                    // First polygon with one linear ring (exterior ring)
                    new List<object>
                    {
                        new List<object>
                        {
                            new List<object> { 2.2945, 48.8584 },
                            new List<object> { 2.2955, 48.8589 },
                            new List<object> { 2.2960, 48.8594 },
                            new List<object> { 2.2950, 48.8600 },
                            new List<object> { 2.2935, 48.8590 },
                            new List<object> { 2.2945, 48.8584 }
                        }
                    },

                    // Second polygon with one linear ring (exterior ring)
                    new List<object>
                    {
                        new List<object>
                        {
                            new List<object> { 2.3376, 48.8606 },
                            new List<object> { 2.3365, 48.8610 },
                            new List<object> { 2.3350, 48.8600 },
                            new List<object> { 2.3376, 48.8606 }
                        }
                    }
                },
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = "EPSG:4326"
                    }
                }
            };
        }

        private static Dictionary<string, object> CreatePolygonWithHolesDictionary()
        {
            // Exterior ring (same as CreatePolygonDictionary)
            var exterior = new List<object>
            {
                new List<object> { 2.2945, 48.8584 },
                new List<object> { 2.2955, 48.8589 },
                new List<object> { 2.2960, 48.8594 },
                new List<object> { 2.2950, 48.8600 },
                new List<object> { 2.2935, 48.8590 },
                new List<object> { 2.2945, 48.8584 }
            };

            // Hole 1: small triangle inside the exterior
            var hole1 = new List<object>
            {
                new List<object> { 2.2950, 48.8587 },
                new List<object> { 2.2952, 48.8588 },
                new List<object> { 2.2948, 48.8588 },
                new List<object> { 2.2950, 48.8587 }
            };

            // Hole 2: small quadrilateral inside the exterior
            var hole2 = new List<object>
            {
                new List<object> { 2.2947, 48.8586 },
                new List<object> { 2.2949, 48.8587 },
                new List<object> { 2.2948, 48.8589 },
                new List<object> { 2.2946, 48.8588 },
                new List<object> { 2.2947, 48.8586 }
            };

            return new Dictionary<string, object>
            {
                ["type"] = "Polygon",
                ["coordinates"] = new List<object> { exterior, hole1, hole2 },
                ["crs"] = new Dictionary<string, object>
                {
                    ["type"] = "name",
                    ["properties"] = new Dictionary<string, object>
                    {
                        ["name"] = "EPSG:4326"
                    }
                }
            };
        }
    }
}

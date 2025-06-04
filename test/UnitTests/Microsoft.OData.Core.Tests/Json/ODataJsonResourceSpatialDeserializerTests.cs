//---------------------------------------------------------------------
// <copyright file="ODataJsonResourceSpatialDeserializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Microsoft.OData.Spatial;
using Xunit;
using NtsGeometryCollection = NetTopologySuite.Geometries.GeometryCollection;
using NtsLineString = NetTopologySuite.Geometries.LineString;
using NtsMultiLineString = NetTopologySuite.Geometries.MultiLineString;
using NtsMultiPoint = NetTopologySuite.Geometries.MultiPoint;
using NtsMultiPolygon = NetTopologySuite.Geometries.MultiPolygon;
using NtsPoint = NetTopologySuite.Geometries.Point;
using NtsPolygon = NetTopologySuite.Geometries.Polygon;

namespace Microsoft.OData.Tests.Json
{
    public class ODataJsonResourceSpatialDeserializerTests
    {
        [Fact]
        public async Task ReadResource_WithPoint_ReturnsExpectedResultAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PointProp\":{\"type\":\"Point\",\"coordinates\":[2.2945,48.8584],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            await SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "PointProp",
                EdmPrimitiveTypeKind.GeometryPoint,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("PointProp", propertyAt1.Name);
                    var geometryPoint = Assert.IsType<GeometryPoint>(propertyAt1.Value);
                    var point = Assert.IsType<NtsPoint>(geometryPoint.Geometry);
                    Assert.Equal(2.2945, point.X, 4); // longitude
                    Assert.Equal(48.8584, point.Y, 4); // latitude
                    Assert.Equal(4326, point.SRID);
                });
        }

        [Fact]
        public async Task ReadResource_WithPoint3D_ReturnsExpectedResultAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PointProp\":{\"type\":\"Point\",\"coordinates\":[2.2945,48.8584,100.5],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            await SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "PointProp",
                EdmPrimitiveTypeKind.GeometryPoint,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("PointProp", propertyAt1.Name);
                    var geometryPoint = Assert.IsType<GeometryPoint>(propertyAt1.Value);
                    var point = Assert.IsType<NtsPoint>(geometryPoint.Geometry);
                    Assert.Equal(2.2945, point.X, 4);
                    Assert.Equal(48.8584, point.Y, 4);
                    Assert.Equal(100.5, point.Coordinate.Z, 4);
                    Assert.Equal(4326, point.SRID);
                });
        }

        [Fact]
        public async Task ReadResource_WithPoint4D_ReturnsExpectedResultAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PointProp\":{\"type\":\"Point\",\"coordinates\":[2.2945,48.8584,100.5,7.7],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            await SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "PointProp",
                EdmPrimitiveTypeKind.GeometryPoint,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("PointProp", propertyAt1.Name);
                    var geometryPoint = Assert.IsType<GeometryPoint>(propertyAt1.Value);
                    var point = Assert.IsType<NtsPoint>(geometryPoint.Geometry);
                    Assert.Equal(2.2945, point.X, 4);
                    Assert.Equal(48.8584, point.Y, 4);
                    Assert.Equal(100.5, point.Coordinate.Z, 4);
                    Assert.Equal(7.7, point.Coordinate.M, 4);
                    Assert.Equal(4326, point.SRID);
                });
        }

        [Fact]
        public async Task ReadResource_WithLineString_ReturnsExpectedResultAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"LineStringProp\":{" +
                "\"type\":\"LineString\"," +
                "\"coordinates\":[[2.2945,48.8584],[2.3365,48.8610],[2.3376,48.8606]]," +
                "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            await SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "LineStringProp",
                EdmPrimitiveTypeKind.GeometryLineString,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("LineStringProp", propertyAt1.Name);
                    var geometryLineString = Assert.IsType<GeometryLineString>(propertyAt1.Value);
                    var lineString = Assert.IsType<NtsLineString>(geometryLineString.Geometry);
                    Assert.Equal(3, lineString.NumPoints);
                    Assert.Equal(2.2945, lineString.GetCoordinateN(0).X, 4);
                    Assert.Equal(48.8584, lineString.GetCoordinateN(0).Y, 4);
                    Assert.Equal(2.3365, lineString.GetCoordinateN(1).X, 4);
                    Assert.Equal(48.8610, lineString.GetCoordinateN(1).Y, 4);
                    Assert.Equal(2.3376, lineString.GetCoordinateN(2).X, 4);
                    Assert.Equal(48.8606, lineString.GetCoordinateN(2).Y, 4);
                    Assert.Equal(4326, lineString.SRID);
                });
        }

        [Fact]
        public async Task ReadResource_WithPolygon_ReturnsExpectedResultAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PolygonProp\":{" +
                "\"type\":\"Polygon\"," +
                "\"coordinates\":[[[2.2945,48.8584],[2.2955,48.8589],[2.2960,48.8594],[2.2950,48.8600],[2.2935,48.8590],[2.2945,48.8584]]]," +
                "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            await SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "PolygonProp",
                EdmPrimitiveTypeKind.GeometryPolygon,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("PolygonProp", propertyAt1.Name);
                    var geometryPolygon = Assert.IsType<GeometryPolygon>(propertyAt1.Value);
                    var polygon = Assert.IsType<NtsPolygon>(geometryPolygon.Geometry);
                    Assert.Equal(4326, polygon.SRID);
                    Assert.Equal(6, polygon.ExteriorRing.NumPoints);
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
                });
        }

        [Fact]
        public async Task ReadResource_WithMultiPoint_ReturnsExpectedResultAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"MultiPointProp\":{" +
                "\"type\":\"MultiPoint\"," +
                "\"coordinates\":[[2.2945,48.8584],[2.3365,48.8610],[2.3376,48.8606]]," +
                "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            await SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "MultiPointProp",
                EdmPrimitiveTypeKind.GeometryMultiPoint,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("MultiPointProp", propertyAt1.Name);
                    var geometryMultiPoint = Assert.IsType<GeometryMultiPoint>(propertyAt1.Value);
                    var multiPoint = Assert.IsType<NtsMultiPoint>(geometryMultiPoint.Geometry);
                    Assert.Equal(3, multiPoint.NumGeometries);
                    Assert.Equal(4326, multiPoint.SRID);
                    Assert.Equal(2.2945, multiPoint.Geometries[0].Coordinate.X, 4);
                    Assert.Equal(48.8584, multiPoint.Geometries[0].Coordinate.Y, 4);
                    Assert.Equal(2.3365, multiPoint.Geometries[1].Coordinate.X, 4);
                    Assert.Equal(48.8610, multiPoint.Geometries[1].Coordinate.Y, 4);
                    Assert.Equal(2.3376, multiPoint.Geometries[2].Coordinate.X, 4);
                    Assert.Equal(48.8606, multiPoint.Geometries[2].Coordinate.Y, 4);
                });
        }

        [Fact]
        public async Task ReadResource_WithEmptyMultiPoint_ReturnsExpectedResultAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"MultiPointProp\":{\"type\":\"MultiPoint\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            await SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "MultiPointProp",
                EdmPrimitiveTypeKind.GeometryMultiPoint,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("MultiPointProp", propertyAt1.Name);
                    var geometryMultiPoint = Assert.IsType<GeometryMultiPoint>(propertyAt1.Value);
                    var multiPoint = Assert.IsType<NtsMultiPoint>(geometryMultiPoint.Geometry);
                    Assert.Equal(0, multiPoint.NumGeometries);
                    Assert.Equal(4326, multiPoint.SRID);
                    Assert.Empty(multiPoint.Geometries);
                });
        }

        [Fact]
        public async Task ReadResource_WithMultiLineString_ReturnsExpectedResultAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"MultiLineStringProp\":{" +
                "\"type\":\"MultiLineString\"," +
                "\"coordinates\":[[[2.2945,48.8584],[2.3365,48.8610],[2.3376,48.8606]],[[2.2950,48.8738],[2.3070,48.8698],[2.3212,48.8656]]]," +
                "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            await SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "MultiLineStringProp",
                EdmPrimitiveTypeKind.GeometryMultiLineString,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("MultiLineStringProp", propertyAt1.Name);
                    var geometryMultiLineString = Assert.IsType<GeometryMultiLineString>(propertyAt1.Value);
                    var multiLineString = Assert.IsType<NtsMultiLineString>(geometryMultiLineString.Geometry);
                    Assert.Equal(2, multiLineString.NumGeometries);
                    Assert.Equal(4326, multiLineString.SRID);

                    var line1 = Assert.IsType<NtsLineString>(multiLineString.Geometries[0]);
                    Assert.Equal(3, line1.NumPoints);
                    Assert.Equal(2.2945, line1.GetCoordinateN(0).X, 4);
                    Assert.Equal(48.8584, line1.GetCoordinateN(0).Y, 4);
                    Assert.Equal(2.3365, line1.GetCoordinateN(1).X, 4);
                    Assert.Equal(48.8610, line1.GetCoordinateN(1).Y, 4);
                    Assert.Equal(2.3376, line1.GetCoordinateN(2).X, 4);
                    Assert.Equal(48.8606, line1.GetCoordinateN(2).Y, 4);

                    var line2 = Assert.IsType<NtsLineString>(multiLineString.Geometries[1]);
                    Assert.Equal(3, line2.NumPoints);
                    Assert.Equal(2.2950, line2.GetCoordinateN(0).X, 4);
                    Assert.Equal(48.8738, line2.GetCoordinateN(0).Y, 4);
                    Assert.Equal(2.3070, line2.GetCoordinateN(1).X, 4);
                    Assert.Equal(48.8698, line2.GetCoordinateN(1).Y, 4);
                    Assert.Equal(2.3212, line2.GetCoordinateN(2).X, 4);
                    Assert.Equal(48.8656, line2.GetCoordinateN(2).Y, 4);
                });
        }

        [Fact]
        public async Task ReadResource_WithEmptyMultiLineString_ReturnsExpectedResultAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"MultiLineStringProp\":{\"type\":\"MultiLineString\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            await SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "MultiLineStringProp",
                EdmPrimitiveTypeKind.GeometryMultiLineString,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("MultiLineStringProp", propertyAt1.Name);
                    var geometryMultiLineString = Assert.IsType<GeometryMultiLineString>(propertyAt1.Value);
                    var multiLineString = Assert.IsType<NtsMultiLineString>(geometryMultiLineString.Geometry);
                    Assert.Equal(0, multiLineString.NumGeometries);
                    Assert.Equal(4326, multiLineString.SRID);
                    Assert.Empty(multiLineString.Geometries);
                });
        }

        [Fact]
        public async Task ReadResource_WithMultiPolygon_ReturnsExpectedResultAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"MultiPolygonProp\":{" +
                "\"type\":\"MultiPolygon\"," +
                "\"coordinates\":[[[[2.2945,48.8584],[2.2955,48.8589],[2.2960,48.8594],[2.2950,48.8600],[2.2935,48.8590],[2.2945,48.8584]]],[[[2.3376,48.8606],[2.3365,48.8610],[2.3350,48.8600],[2.3376,48.8606]]]]," +
                "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            await SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "MultiPolygonProp",
                EdmPrimitiveTypeKind.GeometryMultiPolygon,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("MultiPolygonProp", propertyAt1.Name);
                    var geometryMultiPolygon = Assert.IsType<GeometryMultiPolygon>(propertyAt1.Value);
                    var multiPolygon = Assert.IsType<NtsMultiPolygon>(geometryMultiPolygon.Geometry);
                    Assert.Equal(2, multiPolygon.NumGeometries);
                    Assert.Equal(4326, multiPolygon.SRID);

                    var polygon1 = Assert.IsType<NtsPolygon>(multiPolygon.Geometries[0]);
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

                    var polygon2 = Assert.IsType<NtsPolygon>(multiPolygon.Geometries[1]);
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
                });
        }

        [Fact]
        public async Task ReadResource_WithEmptyMultiPolygon_ReturnsExpectedResultAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"MultiPolygonProp\":{\"type\":\"MultiPolygon\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            await SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "MultiPolygonProp",
                EdmPrimitiveTypeKind.GeometryMultiPolygon,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("MultiPolygonProp", propertyAt1.Name);
                    var geometryMultiPolygon = Assert.IsType<GeometryMultiPolygon>(propertyAt1.Value);
                    var multiPolygon = Assert.IsType<NtsMultiPolygon>(geometryMultiPolygon.Geometry);
                    Assert.Equal(0, multiPolygon.NumGeometries);
                    Assert.Equal(4326, multiPolygon.SRID);
                    Assert.Empty(multiPolygon.Geometries);
                });
        }

        [Fact]
        public async Task ReadResource_WithGeometryCollection_ReturnsExpectedResultAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"GeometryCollectionProp\":" +
                "{\"type\":\"GeometryCollection\"," +
                "\"geometries\":[" +
                "{\"type\":\"Point\"," +
                "\"coordinates\":[2.2945,48.8584]}," +
                "{\"type\":\"LineString\"," +
                "\"coordinates\":[[2.2945,48.8584],[2.3365,48.8610],[2.3376,48.8606]]}," +
                "{\"type\":\"Polygon\"," +
                "\"coordinates\":[[[2.2945,48.8584],[2.2955,48.8589],[2.2960,48.8594],[2.2950,48.8600],[2.2935,48.8590],[2.2945,48.8584]]]}," +
                "{\"type\":\"MultiPoint\"," +
                "\"coordinates\":[[2.2945,48.8584],[2.3365,48.8610],[2.3376,48.8606]]}," +
                "{\"type\":\"MultiLineString\"," +
                "\"coordinates\":[[[2.2945,48.8584],[2.3365,48.8610],[2.3376,48.8606]],[[2.2950,48.8738],[2.3070,48.8698],[2.3212,48.8656]]]}," +
                "{\"type\":\"MultiPolygon\"," +
                "\"coordinates\":[[[[2.2945,48.8584],[2.2955,48.8589],[2.2960,48.8594],[2.2950,48.8600],[2.2935,48.8590],[2.2945,48.8584]]],[[[2.3376,48.8606],[2.3365,48.8610],[2.3350,48.8600],[2.3376,48.8606]]]]}]," +
                "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            await SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "GeometryCollectionProp",
                EdmPrimitiveTypeKind.GeometryCollection,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("GeometryCollectionProp", propertyAt1.Name);
                    var geometryCollection = Assert.IsType<GeometryCollection>(propertyAt1.Value);
                    var wrappedCollection = Assert.IsType<NtsGeometryCollection>(geometryCollection.Geometry);
                    Assert.Equal(6, wrappedCollection.NumGeometries);
                    Assert.Equal(4326, wrappedCollection.SRID);

                    Assert.IsType<NtsPoint>(wrappedCollection.Geometries[0]);
                    Assert.IsType<NtsLineString>(wrappedCollection.Geometries[1]);
                    Assert.IsType<NtsPolygon>(wrappedCollection.Geometries[2]);
                    Assert.IsType<NtsMultiPoint>(wrappedCollection.Geometries[3]);
                    Assert.IsType<NtsMultiLineString>(wrappedCollection.Geometries[4]);
                    Assert.IsType<NtsMultiPolygon>(wrappedCollection.Geometries[5]);

                    Assert.Equal(1, ((NtsPoint)wrappedCollection.Geometries[0]).NumPoints);
                    Assert.Equal(3, ((NtsLineString)wrappedCollection.Geometries[1]).NumPoints);
                    Assert.Equal(6, ((NtsPolygon)wrappedCollection.Geometries[2]).ExteriorRing.NumPoints);
                    Assert.Equal(3, ((NtsMultiPoint)wrappedCollection.Geometries[3]).NumGeometries);
                    Assert.Equal(2, ((NtsMultiLineString)wrappedCollection.Geometries[4]).NumGeometries);
                    Assert.Equal(2, ((NtsMultiPolygon)wrappedCollection.Geometries[5]).NumGeometries);
                });
        }

        [Fact]
        public async Task ReadResource_WithEmptyGeometryCollection_ReturnsExpectedResultAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"GeometryCollectionProp\":{\"type\":\"GeometryCollection\",\"geometries\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            await SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "GeometryCollectionProp",
                EdmPrimitiveTypeKind.GeometryCollection,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("GeometryCollectionProp", propertyAt1.Name);
                    var geometryCollection = Assert.IsType<GeometryCollection>(propertyAt1.Value);
                    var wrappedCollection = Assert.IsType<NtsGeometryCollection>(geometryCollection.Geometry);
                    Assert.Equal(0, wrappedCollection.NumGeometries);
                    Assert.Equal(4326, wrappedCollection.SRID);
                    Assert.Empty(wrappedCollection.Geometries);
                });
        }

        [Fact]
        public async Task ReadResource_WithPolygonWithHoles_ReturnsExpectedResultAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PolygonProp\":{" +
                "\"type\":\"Polygon\"," +
                "\"coordinates\":[[[2.2945,48.8584],[2.2955,48.8589],[2.2960,48.8594],[2.2950,48.8600],[2.2935,48.8590],[2.2945,48.8584]],[[2.2950,48.8587],[2.2952,48.8588],[2.2948,48.8588],[2.2950,48.8587]],[[2.2947,48.8586],[2.2949,48.8587],[2.2948,48.8589],[2.2946,48.8588],[2.2947,48.8586]]]," +
                "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            await SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "PolygonProp",
                EdmPrimitiveTypeKind.GeometryPolygon,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("PolygonProp", propertyAt1.Name);
                    var geometryPolygon = Assert.IsType<GeometryPolygon>(propertyAt1.Value);
                    var polygon = Assert.IsType<NtsPolygon>(geometryPolygon.Geometry);
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
                });
        }

        [Fact]
        public async Task ReadResource_WithPoint_NonDefaultSrid_ReturnsExpectedResultAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PointProp\":{\"type\":\"Point\",\"coordinates\":[2.2945,48.8584],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:3857\"}}}}}}";

            await SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "PointProp",
                EdmPrimitiveTypeKind.GeometryPoint,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("PointProp", propertyAt1.Name);
                    var geometryPoint = Assert.IsType<GeometryPoint>(propertyAt1.Value);
                    var point = Assert.IsType<NtsPoint>(geometryPoint.Geometry);
                    Assert.Equal(2.2945, point.X, 4);
                    Assert.Equal(48.8584, point.Y, 4);
                    Assert.Equal(3857, point.SRID); // Non-default SRID
                });
        }

        [Fact]
        public async Task ReadResource_WithPoint_NoCrsProperty_ReturnsExpectedResultAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PointProp\":{\"type\":\"Point\",\"coordinates\":[2.2945,48.8584]}}";

            await SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "PointProp",
                EdmPrimitiveTypeKind.GeometryPoint,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("PointProp", propertyAt1.Name);
                    var geometryPoint = Assert.IsType<GeometryPoint>(propertyAt1.Value);
                    var point = Assert.IsType<NtsPoint>(geometryPoint.Geometry);
                    Assert.Equal(2.2945, point.X, 4);
                    Assert.Equal(48.8584, point.Y, 4);
                    Assert.Equal(4326, point.SRID); // Default SRID
                });
        }

        [Theory]
        [InlineData("EPSG:4326", 4326)]
        [InlineData("urn:ogc:def:crs:EPSG::4326", 4326)]
        [InlineData("http://www.opengis.net/def/crs/EPSG/0/3857", 3857)]
        public async Task ReadResource_WithPoint_VariousEpsgFormats_ReturnsExpectedResultAsync(string crsName, int expectedSrid)
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                $"\"PointProp\":{{\"type\":\"Point\",\"coordinates\":[2.2945,48.8584],\"crs\":{{\"type\":\"name\",\"properties\":{{\"name\":\"{crsName}\"}}}}}}}}";

            await SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "PointProp",
                EdmPrimitiveTypeKind.GeometryPoint,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("PointProp", propertyAt1.Name);
                    var geometryPoint = Assert.IsType<GeometryPoint>(propertyAt1.Value);
                    var point = Assert.IsType<NtsPoint>(geometryPoint.Geometry);
                    Assert.Equal(2.2945, point.X, 4);
                    Assert.Equal(48.8584, point.Y, 4);
                    Assert.Equal(expectedSrid, point.SRID);
                });
        }

        [Theory]
        [InlineData("urn:ogc:def:crs:OGC:1.3:CRS84")]
        [InlineData("http://www.opengis.net/def/crs/OGC/1.3/CRS84")]
        public async Task ReadResource_WithPoint_NonEpsgCrs_UsesDefaultSridAsync(string crsName)
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                $"\"PointProp\":{{\"type\":\"Point\",\"coordinates\":[2.2945,48.8584],\"crs\":{{\"type\":\"name\",\"properties\":{{\"name\":\"{crsName}\"}}}}}}}}";

            await SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "PointProp",
                EdmPrimitiveTypeKind.GeometryPoint,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("PointProp", propertyAt1.Name);
                    var geometryPoint = Assert.IsType<GeometryPoint>(propertyAt1.Value);
                    var point = Assert.IsType<NtsPoint>(geometryPoint.Geometry);
                    Assert.Equal(2.2945, point.X, 4);
                    Assert.Equal(48.8584, point.Y, 4);
                    Assert.Equal(4326, point.SRID); // Default SRID
                });
        }

        [Fact]
        public async Task ReadResource_WithPoint_MalformedCrs_UsesDefaultSridAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PointProp\":{\"type\":\"Point\",\"coordinates\":[2.2945,48.8584],\"crs\":{\"type\":\"name\"}}}";

            await SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "PointProp",
                EdmPrimitiveTypeKind.GeometryPoint,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("PointProp", propertyAt1.Name);
                    var geometryPoint = Assert.IsType<GeometryPoint>(propertyAt1.Value);
                    var point = Assert.IsType<NtsPoint>(geometryPoint.Geometry);
                    Assert.Equal(2.2945, point.X, 4);
                    Assert.Equal(48.8584, point.Y, 4);
                    Assert.Equal(4326, point.SRID); // Default SRID
                });
        }

        [Fact]
        public void ReadResource_WithPoint_ReturnsExpectedResult()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PointProp\":{\"type\":\"Point\",\"coordinates\":[2.2945,48.8584],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "PointProp",
                EdmPrimitiveTypeKind.GeometryPoint,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("PointProp", propertyAt1.Name);
                    var geometryPoint = Assert.IsType<GeometryPoint>(propertyAt1.Value);
                    var point = Assert.IsType<NtsPoint>(geometryPoint.Geometry);
                    Assert.Equal(2.2945, point.X, 4); // longitude
                    Assert.Equal(48.8584, point.Y, 4); // latitude
                    Assert.Equal(4326, point.SRID);
                });
        }

        [Fact]
        public void ReadResource_WithPoint3D_ReturnsExpectedResult()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PointProp\":{\"type\":\"Point\",\"coordinates\":[2.2945,48.8584,100.5],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "PointProp",
                EdmPrimitiveTypeKind.GeometryPoint,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("PointProp", propertyAt1.Name);
                    var geometryPoint = Assert.IsType<GeometryPoint>(propertyAt1.Value);
                    var point = Assert.IsType<NtsPoint>(geometryPoint.Geometry);
                    Assert.Equal(2.2945, point.X, 4);
                    Assert.Equal(48.8584, point.Y, 4);
                    Assert.Equal(100.5, point.Coordinate.Z, 4);
                    Assert.Equal(4326, point.SRID);
                });
        }

        [Fact]
        public void ReadResource_WithPoint4D_ReturnsExpectedResult()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PointProp\":{\"type\":\"Point\",\"coordinates\":[2.2945,48.8584,100.5,7.7],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "PointProp",
                EdmPrimitiveTypeKind.GeometryPoint,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("PointProp", propertyAt1.Name);
                    var geometryPoint = Assert.IsType<GeometryPoint>(propertyAt1.Value);
                    var point = Assert.IsType<NtsPoint>(geometryPoint.Geometry);
                    Assert.Equal(2.2945, point.X, 4);
                    Assert.Equal(48.8584, point.Y, 4);
                    Assert.Equal(100.5, point.Coordinate.Z, 4);
                    Assert.Equal(7.7, point.Coordinate.M, 4);
                    Assert.Equal(4326, point.SRID);
                });
        }

        [Fact]
        public void ReadResource_WithLineString_ReturnsExpectedResult()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"LineStringProp\":{" +
                "\"type\":\"LineString\"," +
                "\"coordinates\":[[2.2945,48.8584],[2.3365,48.8610],[2.3376,48.8606]]," +
                "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "LineStringProp",
                EdmPrimitiveTypeKind.GeometryLineString,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("LineStringProp", propertyAt1.Name);
                    var geometryLineString = Assert.IsType<GeometryLineString>(propertyAt1.Value);
                    var lineString = Assert.IsType<NtsLineString>(geometryLineString.Geometry);
                    Assert.Equal(3, lineString.NumPoints);
                    Assert.Equal(2.2945, lineString.GetCoordinateN(0).X, 4);
                    Assert.Equal(48.8584, lineString.GetCoordinateN(0).Y, 4);
                    Assert.Equal(2.3365, lineString.GetCoordinateN(1).X, 4);
                    Assert.Equal(48.8610, lineString.GetCoordinateN(1).Y, 4);
                    Assert.Equal(2.3376, lineString.GetCoordinateN(2).X, 4);
                    Assert.Equal(48.8606, lineString.GetCoordinateN(2).Y, 4);
                    Assert.Equal(4326, lineString.SRID);
                });
        }

        [Fact]
        public void ReadResource_WithPolygon_ReturnsExpectedResult()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PolygonProp\":{" +
                "\"type\":\"Polygon\"," +
                "\"coordinates\":[[[2.2945,48.8584],[2.2955,48.8589],[2.2960,48.8594],[2.2950,48.8600],[2.2935,48.8590],[2.2945,48.8584]]]," +
                "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "PolygonProp",
                EdmPrimitiveTypeKind.GeometryPolygon,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("PolygonProp", propertyAt1.Name);
                    var geometryPolygon = Assert.IsType<GeometryPolygon>(propertyAt1.Value);
                    var polygon = Assert.IsType<NtsPolygon>(geometryPolygon.Geometry);
                    Assert.Equal(4326, polygon.SRID);
                    Assert.Equal(6, polygon.ExteriorRing.NumPoints);
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
                });
        }

        [Fact]
        public void ReadResource_WithMultiPoint_ReturnsExpectedResult()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"MultiPointProp\":{" +
                "\"type\":\"MultiPoint\"," +
                "\"coordinates\":[[2.2945,48.8584],[2.3365,48.8610],[2.3376,48.8606]]," +
                "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "MultiPointProp",
                EdmPrimitiveTypeKind.GeometryMultiPoint,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("MultiPointProp", propertyAt1.Name);
                    var geometryMultiPoint = Assert.IsType<GeometryMultiPoint>(propertyAt1.Value);
                    var multiPoint = Assert.IsType<NtsMultiPoint>(geometryMultiPoint.Geometry);
                    Assert.Equal(3, multiPoint.NumGeometries);
                    Assert.Equal(4326, multiPoint.SRID);
                    Assert.Equal(2.2945, multiPoint.Geometries[0].Coordinate.X, 4);
                    Assert.Equal(48.8584, multiPoint.Geometries[0].Coordinate.Y, 4);
                    Assert.Equal(2.3365, multiPoint.Geometries[1].Coordinate.X, 4);
                    Assert.Equal(48.8610, multiPoint.Geometries[1].Coordinate.Y, 4);
                    Assert.Equal(2.3376, multiPoint.Geometries[2].Coordinate.X, 4);
                    Assert.Equal(48.8606, multiPoint.Geometries[2].Coordinate.Y, 4);
                });
        }

        [Fact]
        public void ReadResource_WithEmptyMultiPoint_ReturnsExpectedResult()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"MultiPointProp\":{\"type\":\"MultiPoint\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "MultiPointProp",
                EdmPrimitiveTypeKind.GeometryMultiPoint,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("MultiPointProp", propertyAt1.Name);
                    var geometryMultiPoint = Assert.IsType<GeometryMultiPoint>(propertyAt1.Value);
                    var multiPoint = Assert.IsType<NtsMultiPoint>(geometryMultiPoint.Geometry);
                    Assert.Equal(0, multiPoint.NumGeometries);
                    Assert.Equal(4326, multiPoint.SRID);
                    Assert.Empty(multiPoint.Geometries);
                });
        }

        [Fact]
        public void ReadResource_WithMultiLineString_ReturnsExpectedResult()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"MultiLineStringProp\":{" +
                "\"type\":\"MultiLineString\"," +
                "\"coordinates\":[[[2.2945,48.8584],[2.3365,48.8610],[2.3376,48.8606]],[[2.2950,48.8738],[2.3070,48.8698],[2.3212,48.8656]]]," +
                "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "MultiLineStringProp",
                EdmPrimitiveTypeKind.GeometryMultiLineString,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("MultiLineStringProp", propertyAt1.Name);
                    var geometryMultiLineString = Assert.IsType<GeometryMultiLineString>(propertyAt1.Value);
                    var multiLineString = Assert.IsType<NtsMultiLineString>(geometryMultiLineString.Geometry);
                    Assert.Equal(2, multiLineString.NumGeometries);
                    Assert.Equal(4326, multiLineString.SRID);

                    var line1 = Assert.IsType<NtsLineString>(multiLineString.Geometries[0]);
                    Assert.Equal(3, line1.NumPoints);
                    Assert.Equal(2.2945, line1.GetCoordinateN(0).X, 4);
                    Assert.Equal(48.8584, line1.GetCoordinateN(0).Y, 4);
                    Assert.Equal(2.3365, line1.GetCoordinateN(1).X, 4);
                    Assert.Equal(48.8610, line1.GetCoordinateN(1).Y, 4);
                    Assert.Equal(2.3376, line1.GetCoordinateN(2).X, 4);
                    Assert.Equal(48.8606, line1.GetCoordinateN(2).Y, 4);

                    var line2 = Assert.IsType<NtsLineString>(multiLineString.Geometries[1]);
                    Assert.Equal(3, line2.NumPoints);
                    Assert.Equal(2.2950, line2.GetCoordinateN(0).X, 4);
                    Assert.Equal(48.8738, line2.GetCoordinateN(0).Y, 4);
                    Assert.Equal(2.3070, line2.GetCoordinateN(1).X, 4);
                    Assert.Equal(48.8698, line2.GetCoordinateN(1).Y, 4);
                    Assert.Equal(2.3212, line2.GetCoordinateN(2).X, 4);
                    Assert.Equal(48.8656, line2.GetCoordinateN(2).Y, 4);
                });
        }

        [Fact]
        public void ReadResource_WithEmptyMultiLineString_ReturnsExpectedResult()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"MultiLineStringProp\":{\"type\":\"MultiLineString\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "MultiLineStringProp",
                EdmPrimitiveTypeKind.GeometryMultiLineString,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("MultiLineStringProp", propertyAt1.Name);
                    var geometryMultiLineString = Assert.IsType<GeometryMultiLineString>(propertyAt1.Value);
                    var multiLineString = Assert.IsType<NtsMultiLineString>(geometryMultiLineString.Geometry);
                    Assert.Equal(0, multiLineString.NumGeometries);
                    Assert.Equal(4326, multiLineString.SRID);
                    Assert.Empty(multiLineString.Geometries);
                });
        }

        [Fact]
        public void ReadResource_WithMultiPolygon_ReturnsExpectedResult()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"MultiPolygonProp\":{" +
                "\"type\":\"MultiPolygon\"," +
                "\"coordinates\":[[[[2.2945,48.8584],[2.2955,48.8589],[2.2960,48.8594],[2.2950,48.8600],[2.2935,48.8590],[2.2945,48.8584]]],[[[2.3376,48.8606],[2.3365,48.8610],[2.3350,48.8600],[2.3376,48.8606]]]]," +
                "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "MultiPolygonProp",
                EdmPrimitiveTypeKind.GeometryMultiPolygon,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("MultiPolygonProp", propertyAt1.Name);
                    var geometryMultiPolygon = Assert.IsType<GeometryMultiPolygon>(propertyAt1.Value);
                    var multiPolygon = Assert.IsType<NtsMultiPolygon>(geometryMultiPolygon.Geometry);
                    Assert.Equal(2, multiPolygon.NumGeometries);
                    Assert.Equal(4326, multiPolygon.SRID);

                    var polygon1 = Assert.IsType<NtsPolygon>(multiPolygon.Geometries[0]);
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

                    var polygon2 = Assert.IsType<NtsPolygon>(multiPolygon.Geometries[1]);
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
                });
        }

        [Fact]
        public void ReadResource_WithEmptyMultiPolygon_ReturnsExpectedResult()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"MultiPolygonProp\":{\"type\":\"MultiPolygon\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "MultiPolygonProp",
                EdmPrimitiveTypeKind.GeometryMultiPolygon,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("MultiPolygonProp", propertyAt1.Name);
                    var geometryMultiPolygon = Assert.IsType<GeometryMultiPolygon>(propertyAt1.Value);
                    var multiPolygon = Assert.IsType<NtsMultiPolygon>(geometryMultiPolygon.Geometry);
                    Assert.Equal(0, multiPolygon.NumGeometries);
                    Assert.Equal(4326, multiPolygon.SRID);
                    Assert.Empty(multiPolygon.Geometries);
                });
        }

        [Fact]
        public void ReadResource_WithGeometryCollection_ReturnsExpectedResult()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"GeometryCollectionProp\":" +
                "{\"type\":\"GeometryCollection\"," +
                "\"geometries\":[" +
                "{\"type\":\"Point\"," +
                "\"coordinates\":[2.2945,48.8584]}," +
                "{\"type\":\"LineString\"," +
                "\"coordinates\":[[2.2945,48.8584],[2.3365,48.8610],[2.3376,48.8606]]}," +
                "{\"type\":\"Polygon\"," +
                "\"coordinates\":[[[2.2945,48.8584],[2.2955,48.8589],[2.2960,48.8594],[2.2950,48.8600],[2.2935,48.8590],[2.2945,48.8584]]]}," +
                "{\"type\":\"MultiPoint\"," +
                "\"coordinates\":[[2.2945,48.8584],[2.3365,48.8610],[2.3376,48.8606]]}," +
                "{\"type\":\"MultiLineString\"," +
                "\"coordinates\":[[[2.2945,48.8584],[2.3365,48.8610],[2.3376,48.8606]],[[2.2950,48.8738],[2.3070,48.8698],[2.3212,48.8656]]]}," +
                "{\"type\":\"MultiPolygon\"," +
                "\"coordinates\":[[[[2.2945,48.8584],[2.2955,48.8589],[2.2960,48.8594],[2.2950,48.8600],[2.2935,48.8590],[2.2945,48.8584]]],[[[2.3376,48.8606],[2.3365,48.8610],[2.3350,48.8600],[2.3376,48.8606]]]]}]," +
                "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "GeometryCollectionProp",
                EdmPrimitiveTypeKind.GeometryCollection,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("GeometryCollectionProp", propertyAt1.Name);
                    var geometryCollection = Assert.IsType<GeometryCollection>(propertyAt1.Value);
                    var wrappedCollection = Assert.IsType<NtsGeometryCollection>(geometryCollection.Geometry);
                    Assert.Equal(6, wrappedCollection.NumGeometries);
                    Assert.Equal(4326, wrappedCollection.SRID);

                    Assert.IsType<NtsPoint>(wrappedCollection.Geometries[0]);
                    Assert.IsType<NtsLineString>(wrappedCollection.Geometries[1]);
                    Assert.IsType<NtsPolygon>(wrappedCollection.Geometries[2]);
                    Assert.IsType<NtsMultiPoint>(wrappedCollection.Geometries[3]);
                    Assert.IsType<NtsMultiLineString>(wrappedCollection.Geometries[4]);
                    Assert.IsType<NtsMultiPolygon>(wrappedCollection.Geometries[5]);

                    Assert.Equal(1, ((NtsPoint)wrappedCollection.Geometries[0]).NumPoints);
                    Assert.Equal(3, ((NtsLineString)wrappedCollection.Geometries[1]).NumPoints);
                    Assert.Equal(6, ((NtsPolygon)wrappedCollection.Geometries[2]).ExteriorRing.NumPoints);
                    Assert.Equal(3, ((NtsMultiPoint)wrappedCollection.Geometries[3]).NumGeometries);
                    Assert.Equal(2, ((NtsMultiLineString)wrappedCollection.Geometries[4]).NumGeometries);
                    Assert.Equal(2, ((NtsMultiPolygon)wrappedCollection.Geometries[5]).NumGeometries);
                });
        }

        [Fact]
        public void ReadResource_WithEmptyGeometryCollection_ReturnsExpectedResult()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"GeometryCollectionProp\":{\"type\":\"GeometryCollection\",\"geometries\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "GeometryCollectionProp",
                EdmPrimitiveTypeKind.GeometryCollection,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("GeometryCollectionProp", propertyAt1.Name);
                    var geometryCollection = Assert.IsType<GeometryCollection>(propertyAt1.Value);
                    var wrappedCollection = Assert.IsType<NtsGeometryCollection>(geometryCollection.Geometry);
                    Assert.Equal(0, wrappedCollection.NumGeometries);
                    Assert.Equal(4326, wrappedCollection.SRID);
                    Assert.Empty(wrappedCollection.Geometries);
                });
        }

        [Fact]
        public void ReadResource_WithPolygonWithHoles_ReturnsExpectedResult()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PolygonProp\":{" +
                "\"type\":\"Polygon\"," +
                "\"coordinates\":[[[2.2945,48.8584],[2.2955,48.8589],[2.2960,48.8594],[2.2950,48.8600],[2.2935,48.8590],[2.2945,48.8584]],[[2.2950,48.8587],[2.2952,48.8588],[2.2948,48.8588],[2.2950,48.8587]],[[2.2947,48.8586],[2.2949,48.8587],[2.2948,48.8589],[2.2946,48.8588],[2.2947,48.8586]]]," +
                "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "PolygonProp",
                EdmPrimitiveTypeKind.GeometryPolygon,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("PolygonProp", propertyAt1.Name);
                    var geometryPolygon = Assert.IsType<GeometryPolygon>(propertyAt1.Value);
                    var polygon = Assert.IsType<NtsPolygon>(geometryPolygon.Geometry);
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
                });
        }

        [Fact]
        public void ReadResource_WithPoint_NonDefaultSrid_ReturnsExpectedResult()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PointProp\":{\"type\":\"Point\",\"coordinates\":[2.2945,48.8584],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:3857\"}}}}}}";

            SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "PointProp",
                EdmPrimitiveTypeKind.GeometryPoint,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("PointProp", propertyAt1.Name);
                    var geometryPoint = Assert.IsType<GeometryPoint>(propertyAt1.Value);
                    var point = Assert.IsType<NtsPoint>(geometryPoint.Geometry);
                    Assert.Equal(2.2945, point.X, 4);
                    Assert.Equal(48.8584, point.Y, 4);
                    Assert.Equal(3857, point.SRID); // Non-default SRID
                });
        }

        [Fact]
        public void ReadResource_WithPoint_NoCrsProperty_ReturnsExpectedResult()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PointProp\":{\"type\":\"Point\",\"coordinates\":[2.2945,48.8584]}}";

            SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "PointProp",
                EdmPrimitiveTypeKind.GeometryPoint,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("PointProp", propertyAt1.Name);
                    var geometryPoint = Assert.IsType<GeometryPoint>(propertyAt1.Value);
                    var point = Assert.IsType<NtsPoint>(geometryPoint.Geometry);
                    Assert.Equal(2.2945, point.X, 4);
                    Assert.Equal(48.8584, point.Y, 4);
                    Assert.Equal(4326, point.SRID); // Default SRID
                });
        }

        [Theory]
        [InlineData("EPSG:4326", 4326)]
        [InlineData("urn:ogc:def:crs:EPSG::4326", 4326)]
        [InlineData("http://www.opengis.net/def/crs/EPSG/0/3857", 3857)]
        public void ReadResource_WithPoint_VariousEpsgFormats_ReturnsExpectedResult(string crsName, int expectedSrid)
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                $"\"PointProp\":{{\"type\":\"Point\",\"coordinates\":[2.2945,48.8584],\"crs\":{{\"type\":\"name\",\"properties\":{{\"name\":\"{crsName}\"}}}}}}}}";

            SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "PointProp",
                EdmPrimitiveTypeKind.GeometryPoint,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("PointProp", propertyAt1.Name);
                    var geometryPoint = Assert.IsType<GeometryPoint>(propertyAt1.Value);
                    var point = Assert.IsType<NtsPoint>(geometryPoint.Geometry);
                    Assert.Equal(2.2945, point.X, 4);
                    Assert.Equal(48.8584, point.Y, 4);
                    Assert.Equal(expectedSrid, point.SRID);
                });
        }

        [Theory]
        [InlineData("urn:ogc:def:crs:OGC:1.3:CRS84")]
        [InlineData("http://www.opengis.net/def/crs/OGC/1.3/CRS84")]
        public void ReadResource_WithPoint_NonEpsgCrs_UsesDefaultSrid(string crsName)
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                $"\"PointProp\":{{\"type\":\"Point\",\"coordinates\":[2.2945,48.8584],\"crs\":{{\"type\":\"name\",\"properties\":{{\"name\":\"{crsName}\"}}}}}}}}";

            SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "PointProp",
                EdmPrimitiveTypeKind.GeometryPoint,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("PointProp", propertyAt1.Name);
                    var geometryPoint = Assert.IsType<GeometryPoint>(propertyAt1.Value);
                    var point = Assert.IsType<NtsPoint>(geometryPoint.Geometry);
                    Assert.Equal(2.2945, point.X, 4);
                    Assert.Equal(48.8584, point.Y, 4);
                    Assert.Equal(4326, point.SRID); // Default SRID
                });
        }

        [Fact]
        public void ReadResource_WithPoint_MalformedCrs_UsesDefaultSrid()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PointProp\":{\"type\":\"Point\",\"coordinates\":[2.2945,48.8584],\"crs\":{\"type\":\"name\"}}}";

            SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "PointProp",
                EdmPrimitiveTypeKind.GeometryPoint,
                (resource) =>
                {
                    Assert.NotNull(resource);
                    Assert.NotNull(resource.Properties);
                    Assert.NotEmpty(resource.Properties);
                    Assert.Equal(2, resource.Properties.Count());
                    var propertyAt0 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var propertyAt1 = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", propertyAt0.Name);
                    Assert.Equal(1, propertyAt0.Value);
                    Assert.Equal("PointProp", propertyAt1.Name);
                    var geometryPoint = Assert.IsType<GeometryPoint>(propertyAt1.Value);
                    var point = Assert.IsType<NtsPoint>(geometryPoint.Geometry);
                    Assert.Equal(2.2945, point.X, 4);
                    Assert.Equal(48.8584, point.Y, 4);
                    Assert.Equal(4326, point.SRID); // Default SRID
                });
        }

        #region Unhappy Path Tests

        [Fact]
        public async Task ReadResource_WithPoint_EmptyCoordinates_ThrowsODataExceptionAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PointProp\":{\"type\":\"Point\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(() => SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "PointProp",
                EdmPrimitiveTypeKind.GeometryPoint,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'PointProp' property: expected 'coordinates' to be [x, y] for geometry type 'Point', optionally with elevation [x, y, z], or with elevation and measure [x, y, z, m].",
                exception.Message);
        }

        [Fact]
        public async Task ReadResource_WithPoint_TooFewCoordinates_ThrowsODataExceptionAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PointProp\":{\"type\":\"Point\",\"coordinates\":[2.2945],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(() => SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "PointProp",
                EdmPrimitiveTypeKind.GeometryPoint,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'PointProp' property: expected 'coordinates' to be [x, y] for geometry type 'Point', optionally with elevation [x, y, z], or with elevation and measure [x, y, z, m].",
                exception.Message);
        }

        [Fact]
        public async Task ReadResource_WithPoint_TooManyCoordinates_ThrowsODataExceptionAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PointProp\":{\"type\":\"Point\",\"coordinates\":[2.2945,48.8584,100.5,7.7,123.45],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(() => SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "PointProp",
                EdmPrimitiveTypeKind.GeometryPoint,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'PointProp' property: expected 'coordinates' to be [x, y] for geometry type 'Point', optionally with elevation [x, y, z], or with elevation and measure [x, y, z, m].",
                exception.Message);
        }

        [Fact]
        public async Task ReadResource_WithPoint_CoordinateWithFormatException_ThrowsODataExceptionAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PointProp\":{\"type\":\"Point\",\"coordinates\":[\"not-a-number\",48.8584],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(() => SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "PointProp",
                EdmPrimitiveTypeKind.GeometryPoint,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'PointProp' property: could not convert the coordinate value to double. " +
                "Error: FormatException - The input string 'not-a-number' was not in a correct format.",
                exception.Message);
        }

        [Fact]
        public async Task ReadResource_WithPoint_CoordinateWithInvalidCastException_ThrowsODataExceptionAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PointProp\":{\"type\":\"Point\",\"coordinates\":[[],48.8584],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(() => SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "PointProp",
                EdmPrimitiveTypeKind.GeometryPoint,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'PointProp' property: could not convert the coordinate value to double. " +
                "Error: InvalidCastException - Unable to cast object of type 'System.Collections.Generic.List`1[System.Object]' to type 'System.IConvertible'.",
                exception.Message);
        }

        [Fact]
        public async Task ReadResource_WithPoint_CoordinateWithNullValue_ThrowsODataExceptionAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PointProp\":{\"type\":\"Point\",\"coordinates\":[null,48.8584],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(() => SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "PointProp",
                EdmPrimitiveTypeKind.GeometryPoint,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'PointProp' property: a null value was found in a coordinate pair.",
                exception.Message);
        }

        [Fact]
        public async Task ReadResource_WithPoint_CoordinatesNotEnumerable_ThrowsODataExceptionAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PointProp\":{\"type\":\"Point\",\"coordinates\":2.2945,\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(() => SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "PointProp",
                EdmPrimitiveTypeKind.GeometryPoint,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'PointProp' property: expected 'coordinates' to be [x, y] for geometry type 'Point', optionally with elevation [x, y, z], or with elevation and measure [x, y, z, m].",
                exception.Message);
        }

        [Fact]
        public async Task ReadResource_WithLineString_TooFewCoordinates_ThrowsODataExceptionAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"LineStringProp\":{\"type\":\"LineString\",\"coordinates\":[[2.2945,48.8584]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(() => SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "LineStringProp",
                EdmPrimitiveTypeKind.GeometryLineString,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'LineStringProp' property: LineString must contain two or more coordinate pairs.",
                exception.Message);
        }

        [Fact]
        public async Task ReadResource_WithLineString_CoordinatesNotEnumerable_ThrowsODataExceptionAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"LineStringProp\":{\"type\":\"LineString\",\"coordinates\":2.2945,\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(() => SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "LineStringProp",
                EdmPrimitiveTypeKind.GeometryLineString,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'LineStringProp' property: expected 'coordinates' to be an array of positions for geometry type 'LineString' (e.g., [[x1, y1], [x2, y2]]).",
                exception.Message);
        }

        [Fact]
        public async Task ReadResource_WithPolygon_ExteriorRingTooFewCoordinates_ThrowsODataExceptionAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PolygonProp\":{" +
                "\"type\":\"Polygon\"," +
                "\"coordinates\":[[[2.2945,48.8584],[2.2955,48.8589],[2.2945,48.8584]]]," +
                "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(() => SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "PolygonProp",
                EdmPrimitiveTypeKind.GeometryPolygon,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'PolygonProp' property: ring of Polygon must contain at least 4 coordinate pairs.",
                exception.Message);
        }

        [Fact]
        public async Task ReadResource_WithPolygon_ExteriorRingNotClosed_ThrowsODataExceptionAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PolygonProp\":{" +
                "\"type\":\"Polygon\"," +
                "\"coordinates\":[[[2.2945,48.8584],[2.2955,48.8589],[2.2960,48.8594],[2.2950,48.8600],[2.2935,48.8590]]]," +
                "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(() => SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "PolygonProp",
                EdmPrimitiveTypeKind.GeometryPolygon,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'PolygonProp' property: ring of Polygon must be closed — the first and last coordinate pairs must be exactly equal.",
                exception.Message);
        }

        [Fact]
        public async Task ReadResource_WithPolygon_InteriorRingTooFewCoordinates_ThrowsODataExceptionAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PolygonProp\":{" +
                "\"type\":\"Polygon\"," +
                "\"coordinates\":[[[2.2945,48.8584],[2.2955,48.8589],[2.2960,48.8594],[2.2950,48.8600],[2.2935,48.8590],[2.2945,48.8584]],[[2.2947,48.8586],[2.2949,48.8587],[2.2947,48.8586]]]," +
                "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(() => SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "PolygonProp",
                EdmPrimitiveTypeKind.GeometryPolygon,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'PolygonProp' property: ring of Polygon must contain at least 4 coordinate pairs.",
                exception.Message);
        }

        [Fact]
        public async Task ReadResource_WithPolygon_ExteriorRingTooFewDistinctCoordinates_ThrowsODataExceptionAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PolygonProp\":{" +
                "\"type\":\"Polygon\"," +
                "\"coordinates\":[[[2.2945,48.8584],[2.2955,48.8589],[2.2955,48.8589],[2.2945,48.8584]]]," +
                "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(() => SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "PolygonProp",
                EdmPrimitiveTypeKind.GeometryPolygon,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'PolygonProp' property: ring of Polygon must contain at least 3 distinct coordinate pairs.",
                exception.Message);
        }

        [Fact]
        public async Task ReadResource_WithPolygon_InteriorRingNotClosed_ThrowsODataExceptionAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PolygonProp\":{" +
                "\"type\":\"Polygon\"," +
                "\"coordinates\":[[[2.2945,48.8584],[2.2955,48.8589],[2.2960,48.8594],[2.2950,48.8600],[2.2935,48.8590],[2.2945,48.8584]],[[2.2947,48.8586],[2.2949,48.8587],[2.2948,48.8589],[2.2946,48.8588],[2.2948,48.8587]]]," +
                "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(() => SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "PolygonProp",
                EdmPrimitiveTypeKind.GeometryPolygon,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'PolygonProp' property: ring of Polygon must be closed — the first and last coordinate pairs must be exactly equal.",
                exception.Message);
        }

        [Fact]
        public async Task ReadResource_WithPolygon_CoordinatesNotEnumerable_ThrowsODataExceptionAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PolygonProp\":{\"type\":\"Polygon\",\"coordinates\":2.2945,\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(() => SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "PolygonProp",
                EdmPrimitiveTypeKind.GeometryPolygon,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'PolygonProp' property: expected 'coordinates' to be an array of linear rings for geometry type 'Polygon' " +
                "(e.g., [[[x1, y1], [x2, y2], [x3, y3], [x1, y1]]]).",
                exception.Message);
        }

        [Fact]
        public async Task ReadResource_WithMultiPoint_CoordinatesNotEnumerable_ThrowsODataExceptionAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"MultiPointProp\":{\"type\":\"MultiPoint\",\"coordinates\":2.2945,\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(() => SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "MultiPointProp",
                EdmPrimitiveTypeKind.GeometryMultiPoint,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'MultiPointProp' property: expected 'coordinates' to be an array of points for geometry type 'MultiPoint' (e.g., [[x1, y1], [x2, y2]]).",
                exception.Message);
        }

        [Fact]
        public async Task ReadResource_WithMultiLineString_CoordinatesNotEnumerable_ThrowsODataExceptionAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"MultiLineStringProp\":{\"type\":\"MultiLineString\",\"coordinates\":2.2945,\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(() => SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "MultiLineStringProp",
                EdmPrimitiveTypeKind.GeometryMultiLineString,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'MultiLineStringProp' property: expected 'coordinates' to be an array of line strings for geometry type 'MultiLineString' " +
                "(e.g., [[[x1, y1], [x2, y2]], [[x3, y3], [x4, y4]]]).",
                exception.Message);
        }

        [Fact]
        public async Task ReadResource_WithMultiPolygon_CoordinatesNotEnumerable_ThrowsODataExceptionAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"MultiPolygonProp\":{\"type\":\"MultiPolygon\",\"coordinates\":2.2945,\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(() => SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "MultiPolygonProp",
                EdmPrimitiveTypeKind.GeometryMultiPolygon,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'MultiPolygonProp' property: expected 'coordinates' to be an array of polygons for geometry type 'MultiPolygon' " +
                "(e.g., [[[[x1, y1], [x2, y2], [x3, y3], [x1, y1]]], [[[x4, y4], [x5, y5], [x6, y6], [x4, y4]]]]).",
                exception.Message);
        }

        [Fact]
        public async Task ReadResource_WithGeometryCollection_CoordinatesNotEnumerable_ThrowsODataExceptionAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"GeometryCollectionProp\":{\"type\":\"GeometryCollection\",\"coordinates\":2.2945,\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(() => SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "GeometryCollectionProp",
                EdmPrimitiveTypeKind.GeometryCollection,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'GeometryCollectionProp' property: expected 'geometries' to be an array of valid geometry objects for geometry type 'GeometryCollection' " +
                "(e.g., Point, LineString, Polygon, etc.).",
                exception.Message);
        }

        [Theory]
        [InlineData("Point")]
        [InlineData("LineString")]
        [InlineData("Polygon")]
        [InlineData("MultiPoint")]
        [InlineData("MultiLineString")]
        [InlineData("MultiPolygon")]
        [InlineData("GeometryCollection")]
        public async Task ReadResource_WithInvalidGeometry_MissingCoordinatesAndGeometries_ThrowsODataExceptionAsync(string geometryType)
        {
            var payload = $"{{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                $"\"InvalidGeometryProp\":{{\"type\":\"{geometryType}\",\"crs\":{{\"type\":\"name\",\"properties\":{{\"name\":\"EPSG:4326\"}}}}}}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(() => SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "InvalidGeometryProp",
                EdmPrimitiveTypeKind.GeometryPoint, // The type is not used for this error
                (resource) => { }));

            Assert.Equal(
                "The value specified for the spatial property was not valid. You must specify a valid spatial value.",
                exception.Message);
        }

        [Fact]
        public async Task ReadResource_WithInvalidGeometryType_ThrowsODataExceptionAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"CircleProp\":{\"type\":\"Circle\",\"coordinates\":[2.2945,48.8584],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(() => SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "CircleProp",
                EdmPrimitiveTypeKind.GeometryPoint,
                (resource) => { }));

            Assert.Equal(
                "Unsupported geometry type 'Circle': Supported types are Point, Polygon, LineString, MultiPoint, MultiPolygon, MultiLineString, and GeometryCollection.",
                exception.Message);
        }

        [Fact]
        public async Task ReadResource_WithMissingGeometryType_ThrowsODataExceptionAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"MissingGeometryTypeProp\":{\"coordinates\":[2.2945,48.8584],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(() => SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "MissingGeometryTypeProp",
                EdmPrimitiveTypeKind.GeometryPoint,
                (resource) => { }));

            Assert.Equal(
                "The value specified for the spatial property was not valid. You must specify a valid spatial value.",
                exception.Message);
        }

        [Theory]
        [InlineData(123)]
        [InlineData(true)]
        public async Task ReadResource_WithNonStringGeometryType_ThrowsODataExceptionAsync(object geometryType)
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"NonStringTypeProp\":{" +
                $"\"type\":{geometryType.ToString().ToLowerInvariant()}," +
                "\"coordinates\":[2.2945,48.8584]," +
                "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(() => SetUpJsonResourceDeserializerAndRunTestAsync(
                payload,
                "NonStringTypeProp",
                EdmPrimitiveTypeKind.GeometryPoint,
                (resource) => { }));

            Assert.Equal(
                "The value specified for the spatial property was not valid. You must specify a valid spatial value.",
                exception.Message);
        }

        [Fact]
        public void ReadResource_WithPoint_EmptyCoordinates_ThrowsODataException()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PointProp\":{\"type\":\"Point\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = Assert.Throws<ODataException>(() => SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "PointProp",
                EdmPrimitiveTypeKind.GeometryPoint,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'PointProp' property: expected 'coordinates' to be [x, y] for geometry type 'Point', optionally with elevation [x, y, z], or with elevation and measure [x, y, z, m].",
                exception.Message);
        }

        [Fact]
        public void ReadResource_WithPoint_TooFewCoordinates_ThrowsODataException()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PointProp\":{\"type\":\"Point\",\"coordinates\":[2.2945],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = Assert.Throws<ODataException>(() => SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "PointProp",
                EdmPrimitiveTypeKind.GeometryPoint,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'PointProp' property: expected 'coordinates' to be [x, y] for geometry type 'Point', optionally with elevation [x, y, z], or with elevation and measure [x, y, z, m].",
                exception.Message);
        }

        [Fact]
        public void ReadResource_WithPoint_TooManyCoordinates_ThrowsODataException()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PointProp\":{\"type\":\"Point\",\"coordinates\":[2.2945,48.8584,100.5,7.7,123.45],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = Assert.Throws<ODataException>(() => SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "PointProp",
                EdmPrimitiveTypeKind.GeometryPoint,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'PointProp' property: expected 'coordinates' to be [x, y] for geometry type 'Point', optionally with elevation [x, y, z], or with elevation and measure [x, y, z, m].",
                exception.Message);
        }

        [Fact]
        public void ReadResource_WithPoint_CoordinateWithFormatException_ThrowsODataException()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PointProp\":{\"type\":\"Point\",\"coordinates\":[\"not-a-number\",48.8584],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = Assert.Throws<ODataException>(() => SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "PointProp",
                EdmPrimitiveTypeKind.GeometryPoint,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'PointProp' property: could not convert the coordinate value to double. " +
                "Error: FormatException - The input string 'not-a-number' was not in a correct format.",
                exception.Message);
        }

        [Fact]
        public void ReadResource_WithPoint_CoordinateWithInvalidCastException_ThrowsODataException()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PointProp\":{\"type\":\"Point\",\"coordinates\":[[],48.8584],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = Assert.Throws<ODataException>(() => SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "PointProp",
                EdmPrimitiveTypeKind.GeometryPoint,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'PointProp' property: could not convert the coordinate value to double. " +
                "Error: InvalidCastException - Unable to cast object of type 'System.Collections.Generic.List`1[System.Object]' to type 'System.IConvertible'.",
                exception.Message);
        }

        [Fact]
        public void ReadResource_WithPoint_CoordinateWithNullValue_ThrowsODataException()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PointProp\":{\"type\":\"Point\",\"coordinates\":[null,48.8584],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = Assert.Throws<ODataException>(() => SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "PointProp",
                EdmPrimitiveTypeKind.GeometryPoint,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'PointProp' property: a null value was found in a coordinate pair.",
                exception.Message);
        }

        [Fact]
        public void ReadResource_WithPoint_CoordinatesNotEnumerable_ThrowsODataException()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PointProp\":{\"type\":\"Point\",\"coordinates\":2.2945,\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = Assert.Throws<ODataException>(() => SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "PointProp",
                EdmPrimitiveTypeKind.GeometryPoint,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'PointProp' property: expected 'coordinates' to be [x, y] for geometry type 'Point', optionally with elevation [x, y, z], or with elevation and measure [x, y, z, m].",
                exception.Message);
        }

        [Fact]
        public void ReadResource_WithLineString_TooFewCoordinates_ThrowsODataException()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"LineStringProp\":{\"type\":\"LineString\",\"coordinates\":[[2.2945,48.8584]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = Assert.Throws<ODataException>(() => SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "LineStringProp",
                EdmPrimitiveTypeKind.GeometryLineString,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'LineStringProp' property: LineString must contain two or more coordinate pairs.",
                exception.Message);
        }

        [Fact]
        public void ReadResource_WithLineString_CoordinatesNotEnumerable_ThrowsODataException()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"LineStringProp\":{\"type\":\"LineString\",\"coordinates\":2.2945,\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = Assert.Throws<ODataException>(() => SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "LineStringProp",
                EdmPrimitiveTypeKind.GeometryLineString,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'LineStringProp' property: expected 'coordinates' to be an array of positions for geometry type 'LineString' (e.g., [[x1, y1], [x2, y2]]).",
                exception.Message);
        }

        [Fact]
        public void ReadResource_WithPolygon_ExteriorRingTooFewCoordinates_ThrowsODataException()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PolygonProp\":{" +
                "\"type\":\"Polygon\"," +
                "\"coordinates\":[[[2.2945,48.8584],[2.2955,48.8589],[2.2945,48.8584]]]," +
                "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = Assert.Throws<ODataException>(() => SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "PolygonProp",
                EdmPrimitiveTypeKind.GeometryPolygon,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'PolygonProp' property: ring of Polygon must contain at least 4 coordinate pairs.",
                exception.Message);
        }

        [Fact]
        public void ReadResource_WithPolygon_ExteriorRingNotClosed_ThrowsODataException()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PolygonProp\":{" +
                "\"type\":\"Polygon\"," +
                "\"coordinates\":[[[2.2945,48.8584],[2.2955,48.8589],[2.2960,48.8594],[2.2950,48.8600],[2.2935,48.8590]]]," +
                "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = Assert.Throws<ODataException>(() => SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "PolygonProp",
                EdmPrimitiveTypeKind.GeometryPolygon,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'PolygonProp' property: ring of Polygon must be closed — the first and last coordinate pairs must be exactly equal.",
                exception.Message);
        }

        [Fact]
        public void ReadResource_WithPolygon_InteriorRingTooFewCoordinates_ThrowsODataException()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PolygonProp\":{" +
                "\"type\":\"Polygon\"," +
                "\"coordinates\":[[[2.2945,48.8584],[2.2955,48.8589],[2.2960,48.8594],[2.2950,48.8600],[2.2935,48.8590],[2.2945,48.8584]],[[2.2947,48.8586],[2.2949,48.8587],[2.2947,48.8586]]]," +
                "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = Assert.Throws<ODataException>(() => SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "PolygonProp",
                EdmPrimitiveTypeKind.GeometryPolygon,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'PolygonProp' property: ring of Polygon must contain at least 4 coordinate pairs.",
                exception.Message);
        }

        [Fact]
        public void ReadResource_WithPolygon_ExteriorRingTooFewDistinctCoordinates_ThrowsODataException()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PolygonProp\":{" +
                "\"type\":\"Polygon\"," +
                "\"coordinates\":[[[2.2945,48.8584],[2.2955,48.8589],[2.2955,48.8589],[2.2945,48.8584]]]," +
                "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = Assert.Throws<ODataException>(() => SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "PolygonProp",
                EdmPrimitiveTypeKind.GeometryPolygon,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'PolygonProp' property: ring of Polygon must contain at least 3 distinct coordinate pairs.",
                exception.Message);
        }

        [Fact]
        public void ReadResource_WithPolygon_InteriorRingNotClosed_ThrowsODataException()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PolygonProp\":{" +
                "\"type\":\"Polygon\"," +
                "\"coordinates\":[[[2.2945,48.8584],[2.2955,48.8589],[2.2960,48.8594],[2.2950,48.8600],[2.2935,48.8590],[2.2945,48.8584]],[[2.2947,48.8586],[2.2949,48.8587],[2.2948,48.8589],[2.2946,48.8588],[2.2948,48.8587]]]," +
                "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = Assert.Throws<ODataException>(() => SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "PolygonProp",
                EdmPrimitiveTypeKind.GeometryPolygon,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'PolygonProp' property: ring of Polygon must be closed — the first and last coordinate pairs must be exactly equal.",
                exception.Message);
        }

        [Fact]
        public void ReadResource_WithPolygon_CoordinatesNotEnumerable_ThrowsODataException()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"PolygonProp\":{\"type\":\"Polygon\",\"coordinates\":2.2945,\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = Assert.Throws<ODataException>(() => SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "PolygonProp",
                EdmPrimitiveTypeKind.GeometryPolygon,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'PolygonProp' property: expected 'coordinates' to be an array of linear rings for geometry type 'Polygon' " +
                "(e.g., [[[x1, y1], [x2, y2], [x3, y3], [x1, y1]]]).",
                exception.Message);
        }

        [Fact]
        public void ReadResource_WithMultiPoint_CoordinatesNotEnumerable_ThrowsODataException()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"MultiPointProp\":{\"type\":\"MultiPoint\",\"coordinates\":2.2945,\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = Assert.Throws<ODataException>(() => SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "MultiPointProp",
                EdmPrimitiveTypeKind.GeometryMultiPoint,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'MultiPointProp' property: expected 'coordinates' to be an array of points for geometry type 'MultiPoint' (e.g., [[x1, y1], [x2, y2]]).",
                exception.Message);
        }

        [Fact]
        public void ReadResource_WithMultiLineString_CoordinatesNotEnumerable_ThrowsODataException()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"MultiLineStringProp\":{\"type\":\"MultiLineString\",\"coordinates\":2.2945,\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = Assert.Throws<ODataException>(() => SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "MultiLineStringProp",
                EdmPrimitiveTypeKind.GeometryMultiLineString,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'MultiLineStringProp' property: expected 'coordinates' to be an array of line strings for geometry type 'MultiLineString' " +
                "(e.g., [[[x1, y1], [x2, y2]], [[x3, y3], [x4, y4]]]).",
                exception.Message);
        }

        [Fact]
        public void ReadResource_WithMultiPolygon_CoordinatesNotEnumerable_ThrowsODataException()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"MultiPolygonProp\":{\"type\":\"MultiPolygon\",\"coordinates\":2.2945,\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = Assert.Throws<ODataException>(() => SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "MultiPolygonProp",
                EdmPrimitiveTypeKind.GeometryMultiPolygon,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'MultiPolygonProp' property: expected 'coordinates' to be an array of polygons for geometry type 'MultiPolygon' " +
                "(e.g., [[[[x1, y1], [x2, y2], [x3, y3], [x1, y1]]], [[[x4, y4], [x5, y5], [x6, y6], [x4, y4]]]]).",
                exception.Message);
        }

        [Fact]
        public void ReadResource_WithGeometryCollection_CoordinatesNotEnumerable_ThrowsODataException()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"GeometryCollectionProp\":{\"type\":\"GeometryCollection\",\"coordinates\":2.2945,\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = Assert.Throws<ODataException>(() => SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "GeometryCollectionProp",
                EdmPrimitiveTypeKind.GeometryCollection,
                (resource) => { }));

            Assert.Equal(
                "Invalid 'GeometryCollectionProp' property: expected 'geometries' to be an array of valid geometry objects for geometry type 'GeometryCollection' " +
                "(e.g., Point, LineString, Polygon, etc.).",
                exception.Message);
        }

        [Theory]
        [InlineData("Point")]
        [InlineData("LineString")]
        [InlineData("Polygon")]
        [InlineData("MultiPoint")]
        [InlineData("MultiLineString")]
        [InlineData("MultiPolygon")]
        [InlineData("GeometryCollection")]
        public void ReadResource_WithInvalidGeometry_MissingCoordinatesAndGeometries_ThrowsODataException(string geometryType)
        {
            var payload = $"{{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                $"\"InvalidGeometryProp\":{{\"type\":\"{geometryType}\",\"crs\":{{\"type\":\"name\",\"properties\":{{\"name\":\"EPSG:4326\"}}}}}}}}";

            var exception = Assert.Throws<ODataException>(() => SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "InvalidGeometryProp",
                EdmPrimitiveTypeKind.GeometryPoint, // The type is not used for this error
                (resource) => { }));

            Assert.Equal(
                "The value specified for the spatial property was not valid. You must specify a valid spatial value.",
                exception.Message);
        }

        [Fact]
        public void ReadResource_WithInvalidGeometryType_ThrowsODataException()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"CircleProp\":{\"type\":\"Circle\",\"coordinates\":[2.2945,48.8584],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = Assert.Throws<ODataException>(() => SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "CircleProp",
                EdmPrimitiveTypeKind.GeometryPoint,
                (resource) => { }));

            Assert.Equal(
                "Unsupported geometry type 'Circle': Supported types are Point, Polygon, LineString, MultiPoint, MultiPolygon, MultiLineString, and GeometryCollection.",
                exception.Message);
        }

        [Fact]
        public void ReadResource_WithMissingGeometryType_ThrowsODataException()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"MissingGeometryTypeProp\":{\"coordinates\":[2.2945,48.8584],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = Assert.Throws<ODataException>(() => SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "MissingGeometryTypeProp",
                EdmPrimitiveTypeKind.GeometryPoint,
                (resource) => { }));

            Assert.Equal(
                "The value specified for the spatial property was not valid. You must specify a valid spatial value.",
                exception.Message);
        }

        [Theory]
        [InlineData(123)]
        [InlineData(true)]
        public void ReadResource_WithNonStringGeometryType_ThrowsODataException(object geometryType)
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                "\"Id\":1," +
                "\"NonStringTypeProp\":{" +
                $"\"type\":{geometryType.ToString().ToLowerInvariant()}," +
                "\"coordinates\":[2.2945,48.8584]," +
                "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = Assert.Throws<ODataException>(() => SetUpJsonResourceDeserializerAndRunTest(
                payload,
                "NonStringTypeProp",
                EdmPrimitiveTypeKind.GeometryPoint,
                (resource) => { }));

            Assert.Equal(
                "The value specified for the spatial property was not valid. You must specify a valid spatial value.",
                exception.Message);
        }

        #endregion

        private async Task SetUpJsonResourceDeserializerAndRunTestAsync(
            string payload,
            string geometryPropertyName,
            EdmPrimitiveTypeKind geometryPrimitiveTypeKind,
            Action<ODataResourceBase> verifyResourceAction)
        {
            var (jsonResourceDeserializer, resourceState) = SetUpJsonResourceDeserializerContext(
                payload,
                geometryPropertyName,
                geometryPrimitiveTypeKind,
                isAsync: true);

            await jsonResourceDeserializer.ReadPayloadStartAsync(
                    ODataPayloadKind.Resource,
                    new PropertyAndAnnotationCollector(true),
                    false,
                    true);

            await jsonResourceDeserializer.ReadResourceContentAsync(resourceState);

            verifyResourceAction(resourceState.Resource);
        }

        private void SetUpJsonResourceDeserializerAndRunTest(
            string payload,
            string geometryPropertyName,
            EdmPrimitiveTypeKind geometryPrimitiveTypeKind,
            Action<ODataResourceBase> verifyResourceAction)
        {
            var (jsonResourceDeserializer, resourceState) = SetUpJsonResourceDeserializerContext(
                payload,
                geometryPropertyName,
                geometryPrimitiveTypeKind,
                isAsync: false);

            jsonResourceDeserializer.ReadPayloadStart(
                    ODataPayloadKind.Resource,
                    new PropertyAndAnnotationCollector(true),
                    false,
                    true);

            jsonResourceDeserializer.ReadResourceContent(resourceState);

            verifyResourceAction(resourceState.Resource);
        }

        private (ODataJsonResourceDeserializer, TestJsonReaderResourceState) SetUpJsonResourceDeserializerContext(
            string payload,
            string geometryPropertyName,
            EdmPrimitiveTypeKind geometryPrimitiveTypeKind,
            bool isAsync)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
            var model = new EdmModel();
            var spatialEntityType = model.AddEntityType("NS", "Spatial");
            var idProperty = spatialEntityType.AddStructuralProperty(
                "Id",
                EdmPrimitiveTypeKind.Int32);
            spatialEntityType.AddKeys(idProperty);
            spatialEntityType.AddStructuralProperty(
                geometryPropertyName,
                EdmCoreModel.Instance.GetSpatial(geometryPrimitiveTypeKind, false));

            var defaultEntityContainer = model.AddEntityContainer("Default", "Container");
            var spatialsEntitySet = defaultEntityContainer.AddEntitySet(
                "Spatials",
                spatialEntityType);
            var messageInfo = new ODataMessageInfo
            {
                MessageStream = stream,
                MediaType = new ODataMediaType("application", "json",
                new[]
                {
                    new KeyValuePair<string, string>("odata.metadata", "minimal"),
                    new KeyValuePair<string, string>("odata.streaming", "true"),
                    new KeyValuePair<string, string>("IEEE754Compatible", "false"),
                    new KeyValuePair<string, string>("charset", "utf-8")
                }),
                Encoding = Encoding.UTF8,
                IsResponse = true,
                IsAsync = isAsync,
                Model = model
            };

            var messageReaderSettings = new ODataMessageReaderSettings
            {
                Version = ODataVersion.V4,
                BaseUri = new Uri("http://tempuri.org")
            };

            var jsonInputContext = new ODataJsonInputContext(messageInfo, messageReaderSettings);
            var jsonResourceDeserializer = new ODataJsonResourceDeserializer(jsonInputContext);

            var resourceState = new TestJsonReaderResourceState(spatialsEntitySet, spatialEntityType);

            return (jsonResourceDeserializer, resourceState);
        }
    }
}

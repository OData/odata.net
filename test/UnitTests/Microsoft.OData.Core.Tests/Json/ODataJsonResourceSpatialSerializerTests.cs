//---------------------------------------------------------------------
// <copyright file="ODataJsonResourceSpatialSerializerTests.cs" company="Microsoft">
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
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    public class ODataJsonResourceSpatialSerializerTests
    {
        private static GeometryFactory geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(4326);

        [Fact]
        public async Task WriteResource_WithPoint_ReturnsExpectedResultAsync()
        {
            var point = geometryFactory.CreatePoint(new Coordinate(2.2945, 48.8584));

            await SetUpJsonResourceSerializerAndRunTestAsync(
                "PointProp",
                point,
                EdmPrimitiveTypeKind.GeometryPoint,
                (payload) =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"PointProp\":{" +
                        "\"type\":\"Point\"," +
                        "\"coordinates\":[2.2945,48.8584]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                        payload);
                });
        }

        [Fact]
        public async Task WriteResource_WithPoint3D_ReturnsExpectedResultAsync()
        {
            var point = geometryFactory.CreatePoint(new CoordinateZ(2.2945, 48.8584, 100.5));

            await SetUpJsonResourceSerializerAndRunTestAsync(
                "PointProp",
                point,
                EdmPrimitiveTypeKind.GeometryPoint,
                (payload) =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"PointProp\":{" +
                        "\"type\":\"Point\"," +
                        "\"coordinates\":[2.2945,48.8584,100.5]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                        payload);
                });
        }

        [Fact]
        public async Task WriteResource_WithPoint4D_ReturnsExpectedResultAsync()
        {
            var point = new Point(new CoordinateZM(2.2945, 48.8584, 100.5, 7.7)) { SRID = 4326 };

            await SetUpJsonResourceSerializerAndRunTestAsync(
                "PointProp",
                point,
                EdmPrimitiveTypeKind.GeometryPoint,
                (payload) =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"PointProp\":{" +
                        "\"type\":\"Point\"," +
                        "\"coordinates\":[2.2945,48.8584,100.5,7.7]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                        payload);
                });
        }

        [Fact]
        public async Task WriteResource_WithLineString_ReturnsExpectedResultAsync()
        {
            var lineString = geometryFactory.CreateLineString(
            [
                new Coordinate(2.2945, 48.8584),
                new Coordinate(2.3365, 48.8610),
                new Coordinate(2.3376, 48.8606)
            ]);

            await SetUpJsonResourceSerializerAndRunTestAsync(
                "LineStringProp",
                lineString,
                EdmPrimitiveTypeKind.GeometryLineString,
                (payload) =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"LineStringProp\":{" +
                        "\"type\":\"LineString\"," +
                        "\"coordinates\":[[2.2945,48.8584],[2.3365,48.861],[2.3376,48.8606]]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                        payload);
                });
        }

        [Fact]
        public async Task WriteResource_WithPolygon_ReturnsExpectedResultAsync()
        {
            var polygon = geometryFactory.CreatePolygon(
            [
                new Coordinate(2.2945, 48.8584),
                new Coordinate(2.2955, 48.8589),
                new Coordinate(2.2960, 48.8594),
                new Coordinate(2.2950, 48.8600),
                new Coordinate(2.2935, 48.8590),
                new Coordinate(2.2945, 48.8584)
            ]);

            await SetUpJsonResourceSerializerAndRunTestAsync(
                "PolygonProp",
                polygon,
                EdmPrimitiveTypeKind.GeometryPolygon,
                (payload) =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"PolygonProp\":{" +
                        "\"type\":\"Polygon\"," +
                        "\"coordinates\":[[[2.2945,48.8584],[2.2955,48.8589],[2.296,48.8594],[2.295,48.86],[2.2935,48.859],[2.2945,48.8584]]]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                        payload);
                });
        }

        [Fact]
        public async Task WriteResource_WithMultiPoint_ReturnsExpectedResultAsync()
        {
            var multiPoint = geometryFactory.CreateMultiPoint(
            [
                geometryFactory.CreatePoint(new Coordinate(2.2945, 48.8584)),
                geometryFactory.CreatePoint(new Coordinate(2.3365, 48.8610)),
                geometryFactory.CreatePoint(new Coordinate(2.3376, 48.8606))
            ]);

            await SetUpJsonResourceSerializerAndRunTestAsync(
                "MultiPointProp",
                multiPoint,
                EdmPrimitiveTypeKind.GeometryMultiPoint,
                (payload) =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"MultiPointProp\":{" +
                        "\"type\":\"MultiPoint\"," +
                        "\"coordinates\":[[2.2945,48.8584],[2.3365,48.861],[2.3376,48.8606]]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                        payload);
                });
        }

        [Fact]
        public async Task WriteResource_WithEmptyMultiPoint_ReturnsExpectedResultAsync()
        {
            var multiPoint = geometryFactory.CreateMultiPoint();

            await SetUpJsonResourceSerializerAndRunTestAsync(
                "MultiPointProp",
                multiPoint,
                EdmPrimitiveTypeKind.GeometryMultiPoint,
                (payload) =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"MultiPointProp\":{\"type\":\"MultiPoint\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                        payload);
                });
        }

        [Fact]
        public async Task WriteResource_WithMultiLineString_ReturnsExpectedResultAsync()
        {
            var multiLineString = geometryFactory.CreateMultiLineString(
            [
                geometryFactory.CreateLineString(
                [
                    new Coordinate(2.2945, 48.8584),
                    new Coordinate(2.3365, 48.8610),
                    new Coordinate(2.3376, 48.8606)
                ]),
                geometryFactory.CreateLineString(
                [
                    new Coordinate(2.2950, 48.8738),
                    new Coordinate(2.3070, 48.8698),
                    new Coordinate(2.3212, 48.8656)
                ])
            ]);

            await SetUpJsonResourceSerializerAndRunTestAsync(
                "MultiLineStringProp",
                multiLineString,
                EdmPrimitiveTypeKind.GeometryMultiLineString,
                (payload) =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"MultiLineStringProp\":{" +
                        "\"type\":\"MultiLineString\"," +
                        "\"coordinates\":[[[2.2945,48.8584],[2.3365,48.861],[2.3376,48.8606]],[[2.295,48.8738],[2.307,48.8698],[2.3212,48.8656]]]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                        payload);
                });
        }

        [Fact]
        public async Task WriteResource_WithEmptyMultiLineString_ReturnsExpectedResultAsync()
        {
            var multiLineString = geometryFactory.CreateMultiLineString();

            await SetUpJsonResourceSerializerAndRunTestAsync(
                "MultiLineStringProp",
                multiLineString,
                EdmPrimitiveTypeKind.GeometryMultiLineString,
                (payload) =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"MultiLineStringProp\":{" +
                        "\"type\":\"MultiLineString\"," +
                        "\"coordinates\":[]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                        payload);
                });
        }

        [Fact]
        public async Task WriteResource_WithMultiPolygon_ReturnsExpectedResultAsync()
        {
            var multiPolygon = geometryFactory.CreateMultiPolygon(
            [
                geometryFactory.CreatePolygon(
                [
                    new Coordinate(2.2945, 48.8584),
                    new Coordinate(2.2955, 48.8589),
                    new Coordinate(2.2960, 48.8594),
                    new Coordinate(2.2950, 48.8600),
                    new Coordinate(2.2935, 48.8590),
                    new Coordinate(2.2945, 48.8584)
                ]),
                geometryFactory.CreatePolygon(
                [
                    new Coordinate(2.3376, 48.8606),
                    new Coordinate(2.3365, 48.8610),
                    new Coordinate(2.3350, 48.8600),
                    new Coordinate(2.3376, 48.8606)
                ])
            ]);

            await SetUpJsonResourceSerializerAndRunTestAsync(
                "MultiPolygonProp",
                multiPolygon,
                EdmPrimitiveTypeKind.GeometryMultiPolygon,
                (payload) =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"MultiPolygonProp\":{" +
                        "\"type\":\"MultiPolygon\"," +
                        "\"coordinates\":[[[[2.2945,48.8584],[2.2955,48.8589],[2.296,48.8594],[2.295,48.86],[2.2935,48.859],[2.2945,48.8584]]],[[[2.3376,48.8606],[2.3365,48.861],[2.335,48.86],[2.3376,48.8606]]]]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                        payload);
                });
        }

        [Fact]
        public async Task WriteResource_WithEmptyMultiPolygon_ReturnsExpectedResultAsync()
        {
            var multiPolygon = geometryFactory.CreateMultiPolygon();

            await SetUpJsonResourceSerializerAndRunTestAsync(
                "MultiPolygonProp",
                multiPolygon,
                EdmPrimitiveTypeKind.GeometryMultiPolygon,
                (payload) =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"MultiPolygonProp\":{" +
                        "\"type\":\"MultiPolygon\"," +
                        "\"coordinates\":[]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                        payload);
                });
        }

        [Fact]
        public async Task WriteResource_WithGeometryCollection_ReturnsExpectedResultAsync()
        {
            var collection = geometryFactory.CreateGeometryCollection(
            [
                geometryFactory.CreatePoint(new Coordinate(2.2945, 48.8584)),
                geometryFactory.CreateLineString(
                [
                    new Coordinate(2.2945, 48.8584),
                    new Coordinate(2.3365, 48.8610),
                    new Coordinate(2.3376, 48.8606)
                ]),
                geometryFactory.CreatePolygon(
                [
                    new Coordinate(2.2945, 48.8584),
                    new Coordinate(2.2955, 48.8589),
                    new Coordinate(2.2960, 48.8594),
                    new Coordinate(2.2950, 48.8600),
                    new Coordinate(2.2935, 48.8590),
                    new Coordinate(2.2945, 48.8584)
                ]),
                geometryFactory.CreateMultiPoint(
                [
                    geometryFactory.CreatePoint(new Coordinate(2.2945, 48.8584)),
                    geometryFactory.CreatePoint(new Coordinate(2.3365, 48.8610)),
                    geometryFactory.CreatePoint(new Coordinate(2.3376, 48.8606))
                ]),
                geometryFactory.CreateMultiLineString(
                [
                    geometryFactory.CreateLineString(
                    [
                        new Coordinate(2.2945, 48.8584),
                        new Coordinate(2.3365, 48.8610),
                        new Coordinate(2.3376, 48.8606)
                    ]),
                    geometryFactory.CreateLineString(
                    [
                        new Coordinate(2.2950, 48.8738),
                        new Coordinate(2.3070, 48.8698),
                        new Coordinate(2.3212, 48.8656)
                    ])
                ]),
                geometryFactory.CreateMultiPolygon(
                [
                    geometryFactory.CreatePolygon(
                    [
                        new Coordinate(2.2945, 48.8584),
                        new Coordinate(2.2955, 48.8589),
                        new Coordinate(2.2960, 48.8594),
                        new Coordinate(2.2950, 48.8600),
                        new Coordinate(2.2935, 48.8590),
                        new Coordinate(2.2945, 48.8584)
                    ]),
                    geometryFactory.CreatePolygon(
                    [
                        new Coordinate(2.3376, 48.8606),
                        new Coordinate(2.3365, 48.8610),
                        new Coordinate(2.3350, 48.8600),
                        new Coordinate(2.3376, 48.8606)
                    ])
                ])
            ]);

            await SetUpJsonResourceSerializerAndRunTestAsync(
                "GeometryCollectionProp",
                collection,
                EdmPrimitiveTypeKind.GeometryCollection,
                (payload) =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"GeometryCollectionProp\":{\"type\":\"GeometryCollection\",\"geometries\":[" +
                        "{\"type\":\"Point\"," +
                        "\"coordinates\":[2.2945,48.8584]}," +
                        "{\"type\":\"LineString\"," +
                        "\"coordinates\":[[2.2945,48.8584],[2.3365,48.861],[2.3376,48.8606]]}," +
                        "{\"type\":\"Polygon\"," +
                        "\"coordinates\":[[[2.2945,48.8584],[2.2955,48.8589],[2.296,48.8594],[2.295,48.86],[2.2935,48.859],[2.2945,48.8584]]]}," +
                        "{\"type\":\"MultiPoint\"," +
                        "\"coordinates\":[[2.2945,48.8584],[2.3365,48.861],[2.3376,48.8606]]}," +
                        "{\"type\":\"MultiLineString\"," +
                        "\"coordinates\":[[[2.2945,48.8584],[2.3365,48.861],[2.3376,48.8606]],[[2.295,48.8738],[2.307,48.8698],[2.3212,48.8656]]]}," +
                        "{\"type\":\"MultiPolygon\"," +
                        "\"coordinates\":[[[[2.2945,48.8584],[2.2955,48.8589],[2.296,48.8594],[2.295,48.86],[2.2935,48.859],[2.2945,48.8584]]],[[[2.3376,48.8606],[2.3365,48.861],[2.335,48.86],[2.3376,48.8606]]]]}]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                        payload);
                });
        }

        [Fact]
        public async Task WriteResource_WithEmptyGeometryCollection_ReturnsExpectedResultAsync()
        {
            var collection = geometryFactory.CreateGeometryCollection();

            await SetUpJsonResourceSerializerAndRunTestAsync(
                "GeometryCollectionProp",
                collection,
                EdmPrimitiveTypeKind.GeometryCollection,
                (payload) =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"GeometryCollectionProp\":{" +
                        "\"type\":\"GeometryCollection\"," +
                        "\"geometries\":[]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                        payload);
                });
        }

        [Fact]
        public async Task WriteResource_WithPolygonWithHoles_ReturnsExpectedResultAsync()
        {
            var exterior = new[]
            {
                new Coordinate(2.2945, 48.8584),
                new Coordinate(2.2955, 48.8589),
                new Coordinate(2.2960, 48.8594),
                new Coordinate(2.2950, 48.8600),
                new Coordinate(2.2935, 48.8590),
                new Coordinate(2.2945, 48.8584)
            };
            var hole1 = new[]
            {
                new Coordinate(2.2950, 48.8587),
                new Coordinate(2.2952, 48.8588),
                new Coordinate(2.2948, 48.8588),
                new Coordinate(2.2950, 48.8587)
            };
            var hole2 = new[]
            {
                new Coordinate(2.2947, 48.8586),
                new Coordinate(2.2949, 48.8587),
                new Coordinate(2.2948, 48.8589),
                new Coordinate(2.2946, 48.8588),
                new Coordinate(2.2947, 48.8586)
            };

            var polygon = geometryFactory.CreatePolygon(
                geometryFactory.CreateLinearRing(exterior),
                [
                    geometryFactory.CreateLinearRing(hole1),
                    geometryFactory.CreateLinearRing(hole2)
                ]);

            await SetUpJsonResourceSerializerAndRunTestAsync(
                "PolygonProp",
                polygon,
                EdmPrimitiveTypeKind.GeometryPolygon,
                (payload) =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"PolygonProp\":{" +
                        "\"type\":\"Polygon\"," +
                        "\"coordinates\":[[[2.2945,48.8584],[2.2955,48.8589],[2.296,48.8594],[2.295,48.86],[2.2935,48.859],[2.2945,48.8584]],[[2.295,48.8587],[2.2952,48.8588],[2.2948,48.8588],[2.295,48.8587]],[[2.2947,48.8586],[2.2949,48.8587],[2.2948,48.8589],[2.2946,48.8588],[2.2947,48.8586]]]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                        payload);
                });
        }

        [Fact]
        public async Task WriteResource_WithPoint_NonDefaultSrid_ReturnsExpectedResultAsync()
        {
            var nonDefaultGeometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(3857);
            var point = nonDefaultGeometryFactory.CreatePoint(new Coordinate(2.2945, 48.8584));

            await SetUpJsonResourceSerializerAndRunTestAsync(
                "PointProp",
                point,
                EdmPrimitiveTypeKind.GeometryPoint,
                (payload) =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"PointProp\":{" +
                        "\"type\":\"Point\"," +
                        "\"coordinates\":[2.2945,48.8584]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:3857\"}}}}",
                        payload);
                });
        }

        [Fact]
        public void WriteResource_WithPoint_ReturnsExpectedResult()
        {
            var point = geometryFactory.CreatePoint(new Coordinate(2.2945, 48.8584));

            SetUpJsonResourceSerializerAndRunTest(
                "PointProp",
                point,
                EdmPrimitiveTypeKind.GeometryPoint,
                (payload) =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"PointProp\":{" +
                        "\"type\":\"Point\"," +
                        "\"coordinates\":[2.2945,48.8584]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                        payload);
                });
        }

        [Fact]
        public void WriteResource_WithPoint3D_ReturnsExpectedResult()
        {
            var point = geometryFactory.CreatePoint(new CoordinateZ(2.2945, 48.8584, 100.5));

            SetUpJsonResourceSerializerAndRunTest(
                "PointProp",
                point,
                EdmPrimitiveTypeKind.GeometryPoint,
                (payload) =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"PointProp\":{" +
                        "\"type\":\"Point\"," +
                        "\"coordinates\":[2.2945,48.8584,100.5]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                        payload);
                });
        }

        [Fact]
        public void WriteResource_WithPoint4D_ReturnsExpectedResult()
        {
            var point = new Point(new CoordinateZM(2.2945, 48.8584, 100.5, 7.7)) { SRID = 4326 };

            SetUpJsonResourceSerializerAndRunTest(
                "PointProp",
                point,
                EdmPrimitiveTypeKind.GeometryPoint,
                (payload) =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"PointProp\":{" +
                        "\"type\":\"Point\"," +
                        "\"coordinates\":[2.2945,48.8584,100.5,7.7]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                        payload);
                });
        }

        [Fact]
        public void WriteResource_WithLineString_ReturnsExpectedResult()
        {
            var lineString = geometryFactory.CreateLineString(
            [
                new Coordinate(2.2945, 48.8584),
                new Coordinate(2.3365, 48.8610),
                new Coordinate(2.3376, 48.8606)
            ]);

            SetUpJsonResourceSerializerAndRunTest(
                "LineStringProp",
                lineString,
                EdmPrimitiveTypeKind.GeometryLineString,
                (payload) =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"LineStringProp\":{" +
                        "\"type\":\"LineString\"," +
                        "\"coordinates\":[[2.2945,48.8584],[2.3365,48.861],[2.3376,48.8606]]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                        payload);
                });
        }

        [Fact]
        public void WriteResource_WithPolygon_ReturnsExpectedResult()
        {
            var polygon = geometryFactory.CreatePolygon(
            [
                new Coordinate(2.2945, 48.8584),
                new Coordinate(2.2955, 48.8589),
                new Coordinate(2.2960, 48.8594),
                new Coordinate(2.2950, 48.8600),
                new Coordinate(2.2935, 48.8590),
                new Coordinate(2.2945, 48.8584)
            ]);

            SetUpJsonResourceSerializerAndRunTest(
                "PolygonProp",
                polygon,
                EdmPrimitiveTypeKind.GeometryPolygon,
                (payload) =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"PolygonProp\":{" +
                        "\"type\":\"Polygon\"," +
                        "\"coordinates\":[[[2.2945,48.8584],[2.2955,48.8589],[2.296,48.8594],[2.295,48.86],[2.2935,48.859],[2.2945,48.8584]]]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                        payload);
                });
        }

        [Fact]
        public void WriteResource_WithMultiPoint_ReturnsExpectedResult()
        {
            var multiPoint = geometryFactory.CreateMultiPoint(
            [
                geometryFactory.CreatePoint(new Coordinate(2.2945, 48.8584)),
                geometryFactory.CreatePoint(new Coordinate(2.3365, 48.8610)),
                geometryFactory.CreatePoint(new Coordinate(2.3376, 48.8606))
            ]);

            SetUpJsonResourceSerializerAndRunTest(
                "MultiPointProp",
                multiPoint,
                EdmPrimitiveTypeKind.GeometryMultiPoint,
                (payload) =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"MultiPointProp\":{" +
                        "\"type\":\"MultiPoint\"," +
                        "\"coordinates\":[[2.2945,48.8584],[2.3365,48.861],[2.3376,48.8606]]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                        payload);
                });
        }

        [Fact]
        public void WriteResource_WithEmptyMultiPoint_ReturnsExpectedResult()
        {
            var multiPoint = geometryFactory.CreateMultiPoint();

            SetUpJsonResourceSerializerAndRunTest(
                "MultiPointProp",
                multiPoint,
                EdmPrimitiveTypeKind.GeometryMultiPoint,
                (payload) =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"MultiPointProp\":{\"type\":\"MultiPoint\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                        payload);
                });
        }

        [Fact]
        public void WriteResource_WithMultiLineString_ReturnsExpectedResult()
        {
            var multiLineString = geometryFactory.CreateMultiLineString(
            [
                geometryFactory.CreateLineString(
                [
                    new Coordinate(2.2945, 48.8584),
                    new Coordinate(2.3365, 48.8610),
                    new Coordinate(2.3376, 48.8606)
                ]),
                geometryFactory.CreateLineString(
                [
                    new Coordinate(2.2950, 48.8738),
                    new Coordinate(2.3070, 48.8698),
                    new Coordinate(2.3212, 48.8656)
                ])
            ]);

            SetUpJsonResourceSerializerAndRunTest(
                "MultiLineStringProp",
                multiLineString,
                EdmPrimitiveTypeKind.GeometryMultiLineString,
                (payload) =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"MultiLineStringProp\":{" +
                        "\"type\":\"MultiLineString\"," +
                        "\"coordinates\":[[[2.2945,48.8584],[2.3365,48.861],[2.3376,48.8606]],[[2.295,48.8738],[2.307,48.8698],[2.3212,48.8656]]]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                        payload);
                });
        }

        [Fact]
        public void WriteResource_WithEmptyMultiLineString_ReturnsExpectedResult()
        {
            var multiLineString = geometryFactory.CreateMultiLineString();

            SetUpJsonResourceSerializerAndRunTest(
                "MultiLineStringProp",
                multiLineString,
                EdmPrimitiveTypeKind.GeometryMultiLineString,
                (payload) =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"MultiLineStringProp\":{" +
                        "\"type\":\"MultiLineString\"," +
                        "\"coordinates\":[]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                        payload);
                });
        }

        [Fact]
        public void WriteResource_WithMultiPolygon_ReturnsExpectedResult()
        {
            var multiPolygon = geometryFactory.CreateMultiPolygon(
            [
                geometryFactory.CreatePolygon(
                [
                    new Coordinate(2.2945, 48.8584),
                    new Coordinate(2.2955, 48.8589),
                    new Coordinate(2.2960, 48.8594),
                    new Coordinate(2.2950, 48.8600),
                    new Coordinate(2.2935, 48.8590),
                    new Coordinate(2.2945, 48.8584)
                ]),
                geometryFactory.CreatePolygon(
                [
                    new Coordinate(2.3376, 48.8606),
                    new Coordinate(2.3365, 48.8610),
                    new Coordinate(2.3350, 48.8600),
                    new Coordinate(2.3376, 48.8606)
                ])
            ]);

            SetUpJsonResourceSerializerAndRunTest(
                "MultiPolygonProp",
                multiPolygon,
                EdmPrimitiveTypeKind.GeometryMultiPolygon,
                (payload) =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"MultiPolygonProp\":{" +
                        "\"type\":\"MultiPolygon\"," +
                        "\"coordinates\":[[[[2.2945,48.8584],[2.2955,48.8589],[2.296,48.8594],[2.295,48.86],[2.2935,48.859],[2.2945,48.8584]]],[[[2.3376,48.8606],[2.3365,48.861],[2.335,48.86],[2.3376,48.8606]]]]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                        payload);
                });
        }

        [Fact]
        public void WriteResource_WithEmptyMultiPolygon_ReturnsExpectedResult()
        {
            var multiPolygon = geometryFactory.CreateMultiPolygon();

            SetUpJsonResourceSerializerAndRunTest(
                "MultiPolygonProp",
                multiPolygon,
                EdmPrimitiveTypeKind.GeometryMultiPolygon,
                (payload) =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"MultiPolygonProp\":{" +
                        "\"type\":\"MultiPolygon\"," +
                        "\"coordinates\":[]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                        payload);
                });
        }

        [Fact]
        public void WriteResource_WithGeometryCollection_ReturnsExpectedResult()
        {
            var collection = geometryFactory.CreateGeometryCollection(
            [
                geometryFactory.CreatePoint(new Coordinate(2.2945, 48.8584)),
                geometryFactory.CreateLineString(
                [
                    new Coordinate(2.2945, 48.8584),
                    new Coordinate(2.3365, 48.8610),
                    new Coordinate(2.3376, 48.8606)
                ]),
                geometryFactory.CreatePolygon(
                [
                    new Coordinate(2.2945, 48.8584),
                    new Coordinate(2.2955, 48.8589),
                    new Coordinate(2.2960, 48.8594),
                    new Coordinate(2.2950, 48.8600),
                    new Coordinate(2.2935, 48.8590),
                    new Coordinate(2.2945, 48.8584)
                ]),
                geometryFactory.CreateMultiPoint(
                [
                    geometryFactory.CreatePoint(new Coordinate(2.2945, 48.8584)),
                    geometryFactory.CreatePoint(new Coordinate(2.3365, 48.8610)),
                    geometryFactory.CreatePoint(new Coordinate(2.3376, 48.8606))
                ]),
                geometryFactory.CreateMultiLineString(
                [
                    geometryFactory.CreateLineString(
                    [
                        new Coordinate(2.2945, 48.8584),
                        new Coordinate(2.3365, 48.8610),
                        new Coordinate(2.3376, 48.8606)
                    ]),
                    geometryFactory.CreateLineString(
                    [
                        new Coordinate(2.2950, 48.8738),
                        new Coordinate(2.3070, 48.8698),
                        new Coordinate(2.3212, 48.8656)
                    ])
                ]),
                geometryFactory.CreateMultiPolygon(
                [
                    geometryFactory.CreatePolygon(
                    [
                        new Coordinate(2.2945, 48.8584),
                        new Coordinate(2.2955, 48.8589),
                        new Coordinate(2.2960, 48.8594),
                        new Coordinate(2.2950, 48.8600),
                        new Coordinate(2.2935, 48.8590),
                        new Coordinate(2.2945, 48.8584)
                    ]),
                    geometryFactory.CreatePolygon(
                    [
                        new Coordinate(2.3376, 48.8606),
                        new Coordinate(2.3365, 48.8610),
                        new Coordinate(2.3350, 48.8600),
                        new Coordinate(2.3376, 48.8606)
                    ])
                ])
            ]);

            SetUpJsonResourceSerializerAndRunTest(
                "GeometryCollectionProp",
                collection,
                EdmPrimitiveTypeKind.GeometryCollection,
                (payload) =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"GeometryCollectionProp\":{\"type\":\"GeometryCollection\",\"geometries\":[" +
                        "{\"type\":\"Point\"," +
                        "\"coordinates\":[2.2945,48.8584]}," +
                        "{\"type\":\"LineString\"," +
                        "\"coordinates\":[[2.2945,48.8584],[2.3365,48.861],[2.3376,48.8606]]}," +
                        "{\"type\":\"Polygon\"," +
                        "\"coordinates\":[[[2.2945,48.8584],[2.2955,48.8589],[2.296,48.8594],[2.295,48.86],[2.2935,48.859],[2.2945,48.8584]]]}," +
                        "{\"type\":\"MultiPoint\"," +
                        "\"coordinates\":[[2.2945,48.8584],[2.3365,48.861],[2.3376,48.8606]]}," +
                        "{\"type\":\"MultiLineString\"," +
                        "\"coordinates\":[[[2.2945,48.8584],[2.3365,48.861],[2.3376,48.8606]],[[2.295,48.8738],[2.307,48.8698],[2.3212,48.8656]]]}," +
                        "{\"type\":\"MultiPolygon\"," +
                        "\"coordinates\":[[[[2.2945,48.8584],[2.2955,48.8589],[2.296,48.8594],[2.295,48.86],[2.2935,48.859],[2.2945,48.8584]]],[[[2.3376,48.8606],[2.3365,48.861],[2.335,48.86],[2.3376,48.8606]]]]}]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                        payload);
                });
        }

        [Fact]
        public void WriteResource_WithEmptyGeometryCollection_ReturnsExpectedResult()
        {
            var collection = geometryFactory.CreateGeometryCollection();

            SetUpJsonResourceSerializerAndRunTest(
                "GeometryCollectionProp",
                collection,
                EdmPrimitiveTypeKind.GeometryCollection,
                (payload) =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"GeometryCollectionProp\":{" +
                        "\"type\":\"GeometryCollection\"," +
                        "\"geometries\":[]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                        payload);
                });
        }

        [Fact]
        public void WriteResource_WithPolygonWithHoles_ReturnsExpectedResult()
        {
            var exterior = new[]
            {
                new Coordinate(2.2945, 48.8584),
                new Coordinate(2.2955, 48.8589),
                new Coordinate(2.2960, 48.8594),
                new Coordinate(2.2950, 48.8600),
                new Coordinate(2.2935, 48.8590),
                new Coordinate(2.2945, 48.8584)
            };
            var hole1 = new[]
            {
                new Coordinate(2.2950, 48.8587),
                new Coordinate(2.2952, 48.8588),
                new Coordinate(2.2948, 48.8588),
                new Coordinate(2.2950, 48.8587)
            };
            var hole2 = new[]
            {
                new Coordinate(2.2947, 48.8586),
                new Coordinate(2.2949, 48.8587),
                new Coordinate(2.2948, 48.8589),
                new Coordinate(2.2946, 48.8588),
                new Coordinate(2.2947, 48.8586)
            };

            var polygon = geometryFactory.CreatePolygon(
                geometryFactory.CreateLinearRing(exterior),
                [
                    geometryFactory.CreateLinearRing(hole1),
                    geometryFactory.CreateLinearRing(hole2)
                ]);

            SetUpJsonResourceSerializerAndRunTest(
                "PolygonProp",
                polygon,
                EdmPrimitiveTypeKind.GeometryPolygon,
                (payload) =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"PolygonProp\":{" +
                        "\"type\":\"Polygon\"," +
                        "\"coordinates\":[[[2.2945,48.8584],[2.2955,48.8589],[2.296,48.8594],[2.295,48.86],[2.2935,48.859],[2.2945,48.8584]],[[2.295,48.8587],[2.2952,48.8588],[2.2948,48.8588],[2.295,48.8587]],[[2.2947,48.8586],[2.2949,48.8587],[2.2948,48.8589],[2.2946,48.8588],[2.2947,48.8586]]]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                        payload);
                });
        }

        [Fact]
        public void WriteResource_WithPoint_NonDefaultSrid_ReturnsExpectedResult()
        {
            var nonDefaultGeometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(3857);
            var point = nonDefaultGeometryFactory.CreatePoint(new Coordinate(2.2945, 48.8584));

            SetUpJsonResourceSerializerAndRunTest(
                "PointProp",
                point,
                EdmPrimitiveTypeKind.GeometryPoint,
                (payload) =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"PointProp\":{" +
                        "\"type\":\"Point\"," +
                        "\"coordinates\":[2.2945,48.8584]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:3857\"}}}}",
                        payload);
                });
        }

        private async Task SetUpJsonResourceSerializerAndRunTestAsync(
            string geometryPropertyName,
            Geometry geometry,
            EdmPrimitiveTypeKind geometryPrimitiveTypeKind,
            Action<string> verifyPayloadAction)
        {
            var stream = new MemoryStream();

            var (jsonResourceSerializer, typeContext) = SetUpJsonResourceSerializerContext(
                geometryPropertyName,
                geometryPrimitiveTypeKind,
                stream,
                isAsync: true);

            await jsonResourceSerializer.JsonWriter.StartObjectScopeAsync();
            await jsonResourceSerializer.WriteResourceContextUriAsync(typeContext, null);
            await jsonResourceSerializer.JsonWriter.WriteNameAsync("Id");
            await jsonResourceSerializer.JsonWriter.WritePrimitiveValueAsync(1);
            await jsonResourceSerializer.JsonWriter.WriteNameAsync(geometryPropertyName);
            await jsonResourceSerializer.JsonValueSerializer.WritePrimitiveValueAsync(
                geometry,
                EdmCoreModel.Instance.GetSpatial(geometryPrimitiveTypeKind, false));
            await jsonResourceSerializer.JsonWriter.EndObjectScopeAsync();

            await jsonResourceSerializer.JsonWriter.FlushAsync();

            stream.Position = 0;
            var payload = await new StreamReader(stream).ReadToEndAsync();

            verifyPayloadAction(payload);
        }

        private void SetUpJsonResourceSerializerAndRunTest(
            string geometryPropertyName,
            Geometry geometry,
            EdmPrimitiveTypeKind geometryPrimitiveTypeKind,
            Action<string> verifyPayloadAction)
        {
            var stream = new MemoryStream();

            var (jsonResourceSerializer, typeContext) = SetUpJsonResourceSerializerContext(
                geometryPropertyName,
                geometryPrimitiveTypeKind,
                stream,
                isAsync: false);

            jsonResourceSerializer.JsonWriter.StartObjectScope();
            jsonResourceSerializer.WriteResourceContextUri(typeContext, null);
            jsonResourceSerializer.JsonWriter.WriteName("Id");
            jsonResourceSerializer.JsonWriter.WritePrimitiveValue(1);
            jsonResourceSerializer.JsonWriter.WriteName(geometryPropertyName);
            jsonResourceSerializer.JsonValueSerializer.WritePrimitiveValue(
                geometry,
                EdmCoreModel.Instance.GetSpatial(geometryPrimitiveTypeKind, false));
            jsonResourceSerializer.JsonWriter.EndObjectScope();

            jsonResourceSerializer.JsonWriter.Flush();

            stream.Position = 0;
            var payload = new StreamReader(stream).ReadToEnd();

            verifyPayloadAction(payload);
        }

        private (ODataJsonResourceSerializer, ODataResourceTypeContext) SetUpJsonResourceSerializerContext(
            string geometryPropertyName,
            EdmPrimitiveTypeKind geometryPrimitiveTypeKind,
            Stream stream,
            bool isAsync)
        {
            var model = new EdmModel();
            var spatialEntityType = model.AddEntityType("NS", "Spatial");
            var idProperty = spatialEntityType.AddStructuralProperty(
                "Id",
                EdmPrimitiveTypeKind.Int32);
            var spatialTypeReference = EdmCoreModel.Instance.GetSpatial(geometryPrimitiveTypeKind, false);
            spatialEntityType.AddKeys(idProperty);
            spatialEntityType.AddStructuralProperty(geometryPropertyName, spatialTypeReference);

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

            var messageWriterSettings = new ODataMessageWriterSettings
            {
                Version = ODataVersion.V4
            };
            messageWriterSettings.SetServiceDocumentUri(new Uri("http://tempuri.org"));

            var resourceSerializationInfo = new ODataResourceSerializationInfo
            {
                ExpectedTypeName = "NS.Spatial",
                NavigationSourceName = "Spatials",
                NavigationSourceEntityTypeName = "NS.Spatial",
                NavigationSourceKind = EdmNavigationSourceKind.EntitySet
            };

            var typeContext = ODataResourceTypeContext.Create(
                resourceSerializationInfo,
                spatialsEntitySet,
                spatialEntityType,
                spatialEntityType);

            var jsonOutputContext = new ODataJsonOutputContext(messageInfo, messageWriterSettings);
            var jsonResourceSerializer = new ODataJsonResourceSerializer(jsonOutputContext);

            return (jsonResourceSerializer, typeContext);
        }
    }
}

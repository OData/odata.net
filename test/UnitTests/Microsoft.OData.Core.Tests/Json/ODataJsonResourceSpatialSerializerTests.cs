//---------------------------------------------------------------------
// <copyright file="ODataJsonResourceSpatialSerializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OData.Edm;
using Microsoft.OData.Spatial;
using Xunit;
using Coordinate = NetTopologySuite.Geometries.Coordinate;
using CoordinateZ = NetTopologySuite.Geometries.CoordinateZ;
using CoordinateZM = NetTopologySuite.Geometries.CoordinateZM;

namespace Microsoft.OData.Tests.Json
{
    public class ODataJsonResourceSpatialSerializerTests
    {
        [Fact]
        public async Task WriteResource_WithPoint_ReturnsExpectedResultAsync()
        {
            var point = GeographyFactory.Default.CreatePoint(new Coordinate(2.2945, 48.8584));

            await SetUpMessageWriterAndRunTestAsync(
                "PointProp",
                point,
                EdmPrimitiveTypeKind.GeographyPoint,
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
            var point = GeographyFactory.Default.CreatePoint(new CoordinateZ(2.2945, 48.8584, 100.5));

            await SetUpMessageWriterAndRunTestAsync(
                "PointProp",
                point,
                EdmPrimitiveTypeKind.GeographyPoint,
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
            var point = GeographyFactory.Default.CreatePoint(new CoordinateZM(2.2945, 48.8584, 100.5, 7.7));

            await SetUpMessageWriterAndRunTestAsync(
                "PointProp",
                point,
                EdmPrimitiveTypeKind.GeographyPoint,
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
            var lineString = GeographyFactory.Default.CreateLineString(
            [
                new Coordinate(2.2945, 48.8584),
                new Coordinate(2.3365, 48.8610),
                new Coordinate(2.3376, 48.8606)
            ]);

            await SetUpMessageWriterAndRunTestAsync(
                "LineStringProp",
                lineString,
                EdmPrimitiveTypeKind.GeographyLineString,
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
            var polygon = GeographyFactory.Default.CreatePolygon(
            [
                new Coordinate(2.2945, 48.8584),
                new Coordinate(2.2955, 48.8589),
                new Coordinate(2.2960, 48.8594),
                new Coordinate(2.2950, 48.8600),
                new Coordinate(2.2935, 48.8590),
                new Coordinate(2.2945, 48.8584)
            ]);

            await SetUpMessageWriterAndRunTestAsync(
                "PolygonProp",
                polygon,
                EdmPrimitiveTypeKind.GeographyPolygon,
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
            var multiPoint = GeographyFactory.Default.CreateMultiPoint(
            [
                GeographyFactory.Default.CreatePoint(new Coordinate(2.2945, 48.8584)),
                GeographyFactory.Default.CreatePoint(new Coordinate(2.3365, 48.8610)),
                GeographyFactory.Default.CreatePoint(new Coordinate(2.3376, 48.8606))
            ]);

            await SetUpMessageWriterAndRunTestAsync(
                "MultiPointProp",
                multiPoint,
                EdmPrimitiveTypeKind.GeographyMultiPoint,
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
            var multiPoint = GeographyFactory.Default.CreateMultiPoint();

            await SetUpMessageWriterAndRunTestAsync(
                "MultiPointProp",
                multiPoint,
                EdmPrimitiveTypeKind.GeographyMultiPoint,
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
            var multiLineString = GeographyFactory.Default.CreateMultiLineString(
            [
                GeographyFactory.Default.CreateLineString(
                [
                    new Coordinate(2.2945, 48.8584),
                    new Coordinate(2.3365, 48.8610),
                    new Coordinate(2.3376, 48.8606)
                ]),
                GeographyFactory.Default.CreateLineString(
                [
                    new Coordinate(2.2950, 48.8738),
                    new Coordinate(2.3070, 48.8698),
                    new Coordinate(2.3212, 48.8656)
                ])
            ]);

            await SetUpMessageWriterAndRunTestAsync(
                "MultiLineStringProp",
                multiLineString,
                EdmPrimitiveTypeKind.GeographyMultiLineString,
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
            var multiLineString = GeographyFactory.Default.CreateMultiLineString();

            await SetUpMessageWriterAndRunTestAsync(
                "MultiLineStringProp",
                multiLineString,
                EdmPrimitiveTypeKind.GeographyMultiLineString,
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
            var multiPolygon = GeographyFactory.Default.CreateMultiPolygon(
            [
                GeographyFactory.Default.CreatePolygon(
                [
                    new Coordinate(2.2945, 48.8584),
                    new Coordinate(2.2955, 48.8589),
                    new Coordinate(2.2960, 48.8594),
                    new Coordinate(2.2950, 48.8600),
                    new Coordinate(2.2935, 48.8590),
                    new Coordinate(2.2945, 48.8584)
                ]),
                GeographyFactory.Default.CreatePolygon(
                [
                    new Coordinate(2.3376, 48.8606),
                    new Coordinate(2.3365, 48.8610),
                    new Coordinate(2.3350, 48.8600),
                    new Coordinate(2.3376, 48.8606)
                ])
            ]);

            await SetUpMessageWriterAndRunTestAsync(
                "MultiPolygonProp",
                multiPolygon,
                EdmPrimitiveTypeKind.GeographyMultiPolygon,
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
            var multiPolygon = GeographyFactory.Default.CreateMultiPolygon();

            await SetUpMessageWriterAndRunTestAsync(
                "MultiPolygonProp",
                multiPolygon,
                EdmPrimitiveTypeKind.GeographyMultiPolygon,
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
        public async Task WriteResource_WithGeographyCollection_ReturnsExpectedResultAsync()
        {
            var collection = GeographyFactory.Default.CreateGeographyCollection(
            [
                GeographyFactory.Default.CreatePoint(new Coordinate(2.2945, 48.8584)),
                GeographyFactory.Default.CreateLineString(
                [
                    new Coordinate(2.2945, 48.8584),
                    new Coordinate(2.3365, 48.8610),
                    new Coordinate(2.3376, 48.8606)
                ]),
                GeographyFactory.Default.CreatePolygon(
                [
                    new Coordinate(2.2945, 48.8584),
                    new Coordinate(2.2955, 48.8589),
                    new Coordinate(2.2960, 48.8594),
                    new Coordinate(2.2950, 48.8600),
                    new Coordinate(2.2935, 48.8590),
                    new Coordinate(2.2945, 48.8584)
                ]),
                GeographyFactory.Default.CreateMultiPoint(
                [
                    GeographyFactory.Default.CreatePoint(new Coordinate(2.2945, 48.8584)),
                    GeographyFactory.Default.CreatePoint(new Coordinate(2.3365, 48.8610)),
                    GeographyFactory.Default.CreatePoint(new Coordinate(2.3376, 48.8606))
                ]),
                GeographyFactory.Default.CreateMultiLineString(
                [
                    GeographyFactory.Default.CreateLineString(
                    [
                        new Coordinate(2.2945, 48.8584),
                        new Coordinate(2.3365, 48.8610),
                        new Coordinate(2.3376, 48.8606)
                    ]),
                    GeographyFactory.Default.CreateLineString(
                    [
                        new Coordinate(2.2950, 48.8738),
                        new Coordinate(2.3070, 48.8698),
                        new Coordinate(2.3212, 48.8656)
                    ])
                ]),
                GeographyFactory.Default.CreateMultiPolygon(
                [
                    GeographyFactory.Default.CreatePolygon(
                    [
                        new Coordinate(2.2945, 48.8584),
                        new Coordinate(2.2955, 48.8589),
                        new Coordinate(2.2960, 48.8594),
                        new Coordinate(2.2950, 48.8600),
                        new Coordinate(2.2935, 48.8590),
                        new Coordinate(2.2945, 48.8584)
                    ]),
                    GeographyFactory.Default.CreatePolygon(
                    [
                        new Coordinate(2.3376, 48.8606),
                        new Coordinate(2.3365, 48.8610),
                        new Coordinate(2.3350, 48.8600),
                        new Coordinate(2.3376, 48.8606)
                    ])
                ])
            ]);

            await SetUpMessageWriterAndRunTestAsync(
                "GeographyCollectionProp",
                collection,
                EdmPrimitiveTypeKind.GeographyCollection,
                (payload) =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"GeographyCollectionProp\":{\"type\":\"GeometryCollection\",\"geometries\":[" +
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
        public async Task WriteResource_WithEmptyGeographyCollection_ReturnsExpectedResultAsync()
        {
            var collection = GeographyFactory.Default.CreateGeographyCollection();

            await SetUpMessageWriterAndRunTestAsync(
                "GeographyCollectionProp",
                collection,
                EdmPrimitiveTypeKind.GeographyCollection,
                (payload) =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"GeographyCollectionProp\":{" +
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

            var polygon = GeographyFactory.Default.CreatePolygon(exterior, [hole1, hole2]);

            await SetUpMessageWriterAndRunTestAsync(
                "PolygonProp",
                polygon,
                EdmPrimitiveTypeKind.GeographyPolygon,
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
            var point = GeographyFactory.Create(3857).CreatePoint(new Coordinate(2.2945, 48.8584));

            await SetUpMessageWriterAndRunTestAsync(
                "PointProp",
                point,
                EdmPrimitiveTypeKind.GeographyPoint,
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
        public async Task WriteResource_WithPoint_InjectedSpatialPrimitiveTypeConverter_ReturnsExpectedResultAsync()
        {
            var point = GeographyFactory.Default.CreatePoint(new Coordinate(2.2945, 48.8584));

            await SetUpMessageWriterAndRunTestAsync(
                "PointProp",
                point,
                EdmPrimitiveTypeKind.GeographyPoint,
                verifyPayloadAction: payload =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"PointProp\":{" +
                        "\"type\":\"Point\"," +
                        "\"coordinates\":[2.2945,48.8584]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                        payload);
                },
                configureServices: services =>
                {
                    services.RemoveAll<ISpatialPrimitiveTypeConverter>();
                    services.AddScoped<ISpatialPrimitiveTypeConverter, SpatialPrimitiveTypeConverter>();
                });
        }

        [Fact]
        public async Task WriteResource_WithPoint_InjectedGeoJsonPrimitiveTypeConverter_ReturnsExpectedResultAsync()
        {
            var point = GeographyFactory.Default.CreatePoint(new Coordinate(2.2945, 48.8584));

            await SetUpMessageWriterAndRunTestAsync(
                "PointProp",
                point,
                EdmPrimitiveTypeKind.GeographyPoint,
                configureServices: services =>
                {
                    services.RemoveAll<ISpatialPrimitiveTypeConverter>();
                    services.AddTransient<ISpatialPrimitiveTypeConverter, GeoJsonPrimitiveTypeConverter>();
                },
                verifyPayloadAction: payload =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"PointProp\":{" +
                        "\"type\":\"Point\"," +
                        "\"coordinates\":[2.2945,48.8584]}}",
                        payload);
                });
        }

        [Fact]
        public void WriteResource_WithPoint_ReturnsExpectedResult()
        {
            var point = GeographyFactory.Default.CreatePoint(new Coordinate(2.2945, 48.8584));

            SetUpMessageWriterAndRunTest(
                "PointProp",
                point,
                EdmPrimitiveTypeKind.GeographyPoint,
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
            var point = GeographyFactory.Default.CreatePoint(new CoordinateZ(2.2945, 48.8584, 100.5));

            SetUpMessageWriterAndRunTest(
                "PointProp",
                point,
                EdmPrimitiveTypeKind.GeographyPoint,
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
            var point = GeographyFactory.Default.CreatePoint(new CoordinateZM(2.2945, 48.8584, 100.5, 7.7));

            SetUpMessageWriterAndRunTest(
                "PointProp",
                point,
                EdmPrimitiveTypeKind.GeographyPoint,
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
            var lineString = GeographyFactory.Default.CreateLineString(
            [
                new Coordinate(2.2945, 48.8584),
                new Coordinate(2.3365, 48.8610),
                new Coordinate(2.3376, 48.8606)
            ]);

            SetUpMessageWriterAndRunTest(
                "LineStringProp",
                lineString,
                EdmPrimitiveTypeKind.GeographyLineString,
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
            var polygon = GeographyFactory.Default.CreatePolygon(
            [
                new Coordinate(2.2945, 48.8584),
                new Coordinate(2.2955, 48.8589),
                new Coordinate(2.2960, 48.8594),
                new Coordinate(2.2950, 48.8600),
                new Coordinate(2.2935, 48.8590),
                new Coordinate(2.2945, 48.8584)
            ]);

            SetUpMessageWriterAndRunTest(
                "PolygonProp",
                polygon,
                EdmPrimitiveTypeKind.GeographyPolygon,
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
            var multiPoint = GeographyFactory.Default.CreateMultiPoint(
            [
                GeographyFactory.Default.CreatePoint(new Coordinate(2.2945, 48.8584)),
                GeographyFactory.Default.CreatePoint(new Coordinate(2.3365, 48.8610)),
                GeographyFactory.Default.CreatePoint(new Coordinate(2.3376, 48.8606))
            ]);

            SetUpMessageWriterAndRunTest(
                "MultiPointProp",
                multiPoint,
                EdmPrimitiveTypeKind.GeographyMultiPoint,
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
            var multiPoint = GeographyFactory.Default.CreateMultiPoint();

            SetUpMessageWriterAndRunTest(
                "MultiPointProp",
                multiPoint,
                EdmPrimitiveTypeKind.GeographyMultiPoint,
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
            var multiLineString = GeographyFactory.Default.CreateMultiLineString(
            [
                GeographyFactory.Default.CreateLineString(
                [
                    new Coordinate(2.2945, 48.8584),
                    new Coordinate(2.3365, 48.8610),
                    new Coordinate(2.3376, 48.8606)
                ]),
                GeographyFactory.Default.CreateLineString(
                [
                    new Coordinate(2.2950, 48.8738),
                    new Coordinate(2.3070, 48.8698),
                    new Coordinate(2.3212, 48.8656)
                ])
            ]);

            SetUpMessageWriterAndRunTest(
                "MultiLineStringProp",
                multiLineString,
                EdmPrimitiveTypeKind.GeographyMultiLineString,
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
            var multiLineString = GeographyFactory.Default.CreateMultiLineString();

            SetUpMessageWriterAndRunTest(
                "MultiLineStringProp",
                multiLineString,
                EdmPrimitiveTypeKind.GeographyMultiLineString,
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
            var multiPolygon = GeographyFactory.Default.CreateMultiPolygon(
            [
                GeographyFactory.Default.CreatePolygon(
                [
                    new Coordinate(2.2945, 48.8584),
                    new Coordinate(2.2955, 48.8589),
                    new Coordinate(2.2960, 48.8594),
                    new Coordinate(2.2950, 48.8600),
                    new Coordinate(2.2935, 48.8590),
                    new Coordinate(2.2945, 48.8584)
                ]),
                GeographyFactory.Default.CreatePolygon(
                [
                    new Coordinate(2.3376, 48.8606),
                    new Coordinate(2.3365, 48.8610),
                    new Coordinate(2.3350, 48.8600),
                    new Coordinate(2.3376, 48.8606)
                ])
            ]);

            SetUpMessageWriterAndRunTest(
                "MultiPolygonProp",
                multiPolygon,
                EdmPrimitiveTypeKind.GeographyMultiPolygon,
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
            var multiPolygon = GeographyFactory.Default.CreateMultiPolygon();

            SetUpMessageWriterAndRunTest(
                "MultiPolygonProp",
                multiPolygon,
                EdmPrimitiveTypeKind.GeographyMultiPolygon,
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
        public void WriteResource_WithGeographyCollection_ReturnsExpectedResult()
        {
            var collection = GeographyFactory.Default.CreateGeographyCollection(
            [
                GeographyFactory.Default.CreatePoint(new Coordinate(2.2945, 48.8584)),
                GeographyFactory.Default.CreateLineString(
                [
                    new Coordinate(2.2945, 48.8584),
                    new Coordinate(2.3365, 48.8610),
                    new Coordinate(2.3376, 48.8606)
                ]),
                GeographyFactory.Default.CreatePolygon(
                [
                    new Coordinate(2.2945, 48.8584),
                    new Coordinate(2.2955, 48.8589),
                    new Coordinate(2.2960, 48.8594),
                    new Coordinate(2.2950, 48.8600),
                    new Coordinate(2.2935, 48.8590),
                    new Coordinate(2.2945, 48.8584)
                ]),
                GeographyFactory.Default.CreateMultiPoint(
                [
                    GeographyFactory.Default.CreatePoint(new Coordinate(2.2945, 48.8584)),
                    GeographyFactory.Default.CreatePoint(new Coordinate(2.3365, 48.8610)),
                    GeographyFactory.Default.CreatePoint(new Coordinate(2.3376, 48.8606))
                ]),
                GeographyFactory.Default.CreateMultiLineString(
                [
                    GeographyFactory.Default.CreateLineString(
                    [
                        new Coordinate(2.2945, 48.8584),
                        new Coordinate(2.3365, 48.8610),
                        new Coordinate(2.3376, 48.8606)
                    ]),
                    GeographyFactory.Default.CreateLineString(
                    [
                        new Coordinate(2.2950, 48.8738),
                        new Coordinate(2.3070, 48.8698),
                        new Coordinate(2.3212, 48.8656)
                    ])
                ]),
                GeographyFactory.Default.CreateMultiPolygon(
                [
                    GeographyFactory.Default.CreatePolygon(
                    [
                        new Coordinate(2.2945, 48.8584),
                        new Coordinate(2.2955, 48.8589),
                        new Coordinate(2.2960, 48.8594),
                        new Coordinate(2.2950, 48.8600),
                        new Coordinate(2.2935, 48.8590),
                        new Coordinate(2.2945, 48.8584)
                    ]),
                    GeographyFactory.Default.CreatePolygon(
                    [
                        new Coordinate(2.3376, 48.8606),
                        new Coordinate(2.3365, 48.8610),
                        new Coordinate(2.3350, 48.8600),
                        new Coordinate(2.3376, 48.8606)
                    ])
                ])
            ]);

            SetUpMessageWriterAndRunTest(
                "GeographyCollectionProp",
                collection,
                EdmPrimitiveTypeKind.GeographyCollection,
                (payload) =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"GeographyCollectionProp\":{\"type\":\"GeometryCollection\",\"geometries\":[" +
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
        public void WriteResource_WithEmptyGeographyCollection_ReturnsExpectedResult()
        {
            var collection = GeographyFactory.Default.CreateGeographyCollection();

            SetUpMessageWriterAndRunTest(
                "GeographyCollectionProp",
                collection,
                EdmPrimitiveTypeKind.GeographyCollection,
                (payload) =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"GeographyCollectionProp\":{" +
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

            var polygon = GeographyFactory.Default.CreatePolygon(exterior, [hole1, hole2]);

            SetUpMessageWriterAndRunTest(
                "PolygonProp",
                polygon,
                EdmPrimitiveTypeKind.GeographyPolygon,
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
            var point = GeographyFactory.Create(3857).CreatePoint(new Coordinate(2.2945, 48.8584));

            SetUpMessageWriterAndRunTest(
                "PointProp",
                point,
                EdmPrimitiveTypeKind.GeographyPoint,
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
        public void WriteResource_WithPoint_InjectedSpatialPrimitiveTypeConverter_ReturnsExpectedResult()
        {
            var point = GeographyFactory.Default.CreatePoint(new Coordinate(2.2945, 48.8584));

            SetUpMessageWriterAndRunTest(
                "PointProp",
                point,
                EdmPrimitiveTypeKind.GeographyPoint,
                verifyPayloadAction: payload =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"PointProp\":{" +
                        "\"type\":\"Point\"," +
                        "\"coordinates\":[2.2945,48.8584]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                        payload);
                },
                configureServices: services =>
                {
                    services.RemoveAll<ISpatialPrimitiveTypeConverter>();
                    services.AddScoped<ISpatialPrimitiveTypeConverter, SpatialPrimitiveTypeConverter>();
                });
        }

        [Fact]
        public void WriteResource_WithPoint_InjectedGeoJsonPrimitiveTypeConverter_ReturnsExpectedResult()
        {
            var point = GeographyFactory.Default.CreatePoint(new Coordinate(2.2945, 48.8584));

            SetUpMessageWriterAndRunTest(
                "PointProp",
                point,
                EdmPrimitiveTypeKind.GeographyPoint,
                verifyPayloadAction: payload =>
                {
                    Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Spatials/$entity\"," +
                        "\"Id\":1," +
                        "\"PointProp\":{" +
                        "\"type\":\"Point\"," +
                        "\"coordinates\":[2.2945,48.8584]}}",
                        payload);
                },
                configureServices: services =>
                {
                    services.RemoveAll<ISpatialPrimitiveTypeConverter>();
                    services.AddScoped<ISpatialPrimitiveTypeConverter, GeoJsonPrimitiveTypeConverter>();
                });
        }

        private async Task SetUpMessageWriterAndRunTestAsync(
            string geographyPropertyName,
            Geography geography,
            EdmPrimitiveTypeKind geographyPrimitiveTypeKind,
            Action<string> verifyPayloadAction,
            Action<IServiceCollection> configureServices = null)
        {
            var stream = new MemoryStream();
            var (responseMessage, messageWriterSettings) = SetUpMessageWriterContext(
                geographyPropertyName,
                geographyPrimitiveTypeKind,
                stream,
                configureServices);

            var model = BuildEdmModel(geographyPropertyName, geographyPrimitiveTypeKind);
            var spatialEntityType = model.FindDeclaredType("NS.Spatial") as EdmEntityType;
            var spatialsEntitySet = model.FindDeclaredEntitySet("Spatials");
            var messageInfo = CreateMessageInfo(model, stream, isAsync: true);

            await using (var messageWriter = new ODataMessageWriter(responseMessage, messageWriterSettings, model))
            {
                var resourceWriter = await messageWriter.CreateODataResourceWriterAsync(spatialsEntitySet, spatialEntityType);
                var resource = new ODataResource()
                {
                    TypeName = "NS.Spatial",
                    Properties = new[]
                    {
                        new ODataProperty() { Name = "Id", Value = 1 },
                        new ODataProperty() { Name = geographyPropertyName, Value = geography }
                    }
                };

                await resourceWriter.WriteStartAsync(resource);
                await resourceWriter.WriteEndAsync();
            }

            stream.Position = 0;
            var payload = await new StreamReader(stream).ReadToEndAsync();

            verifyPayloadAction(payload);
        }

        private void SetUpMessageWriterAndRunTest(
            string geographyPropertyName,
            Geography geography,
            EdmPrimitiveTypeKind geographyPrimitiveTypeKind,
            Action<string> verifyPayloadAction,
            Action<IServiceCollection> configureServices = null)
        {
            var stream = new MemoryStream();
            var (responseMessage, messageWriterSettings) = SetUpMessageWriterContext(
                geographyPropertyName,
                geographyPrimitiveTypeKind,
                stream,
                configureServices);

            var model = BuildEdmModel(geographyPropertyName, geographyPrimitiveTypeKind);
            var spatialEntityType = model.FindDeclaredType("NS.Spatial") as EdmEntityType;
            var spatialsEntitySet = model.FindDeclaredEntitySet("Spatials");
            var messageInfo = CreateMessageInfo(model, stream, isAsync: false);

            using (var messageWriter = new ODataMessageWriter(responseMessage, messageWriterSettings, model))
            {
                var resourceWriter = messageWriter.CreateODataResourceWriter(spatialsEntitySet, spatialEntityType);
                var resource = new ODataResource()
                {
                    TypeName = "NS.Spatial",
                    Properties = new[]
                    {
                        new ODataProperty() { Name = "Id", Value = 1 },
                        new ODataProperty() { Name = geographyPropertyName, Value = geography }
                    }
                };

                resourceWriter.WriteStart(resource);
                resourceWriter.WriteEnd();
            }

            stream.Position = 0;
            var payload = new StreamReader(stream).ReadToEnd();

            verifyPayloadAction(payload);
        }

        private (IODataResponseMessage, ODataMessageWriterSettings) SetUpMessageWriterContext(
            string geographyPropertyName,
            EdmPrimitiveTypeKind geographyPrimitiveTypeKind,
            Stream stream,
            Action<IServiceCollection> configureServices = null)
        {
            var services = new ServiceCollection().AddDefaultODataServices();
            configureServices?.Invoke(services);

            var messageWriterSettings = new ODataMessageWriterSettings
            {
                Version = ODataVersion.V4,
                EnableMessageStreamDisposal = false
            };
            messageWriterSettings.SetServiceDocumentUri(new Uri("http://tempuri.org"));

            IODataResponseMessage responseMessage = new InMemoryMessage()
            {
                Stream = stream,
                ServiceProvider = services.BuildServiceProvider()
            };

            responseMessage.SetHeader("Content-Type", "application/json;odata.metadata=minimal");

            return (responseMessage, messageWriterSettings);
        }

        private static EdmModel BuildEdmModel(string geographyPropertyName, EdmPrimitiveTypeKind geographyPrimitiveTypeKind)
        {
            var model = new EdmModel();
            var spatialEntityType = model.AddEntityType("NS", "Spatial");
            var idProperty = spatialEntityType.AddStructuralProperty(
                "Id",
                EdmPrimitiveTypeKind.Int32);
            var spatialTypeReference = EdmCoreModel.Instance.GetSpatial(geographyPrimitiveTypeKind, false);
            spatialEntityType.AddKeys(idProperty);
            spatialEntityType.AddStructuralProperty(geographyPropertyName, spatialTypeReference);

            var defaultEntityContainer = model.AddEntityContainer("Default", "Container");
            defaultEntityContainer.AddEntitySet(
                "Spatials",
                spatialEntityType);

            return model;
        }

        private static ODataMessageInfo CreateMessageInfo(EdmModel model, Stream stream, bool isAsync)
        {
            return new ODataMessageInfo
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
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="SpatialGeographyTypeConverterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Json;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class SpatialGeographyTypeConverterTests
    {
        private static GeometryFactory geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(4326);

        [Fact]
        public async Task WritePointAsync()
        {
            var point = geometryFactory.CreatePoint(new Coordinate(2.2945, 48.8584));

            await SetUpJsonWriterAndRunTestAsync(
                point,
                (payload) =>
                {
                    Assert.Equal("{\"type\":\"Point\",\"coordinates\":[2.2945,48.8584],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}", payload);
                });
        }

        [Fact]
        public async Task WritePoint3DAsync()
        {
            var point = geometryFactory.CreatePoint(new CoordinateZ(2.2945, 48.8584, 100.5));

            await SetUpJsonWriterAndRunTestAsync(
                point,
                payload =>
                {
                    Assert.Equal("{\"type\":\"Point\",\"coordinates\":[2.2945,48.8584,100.5],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}", payload);
                });
        }

        [Fact]
        public async Task WritePoint4DAsync()
        {
            var point = geometryFactory.CreatePoint(new CoordinateZM(2.2945, 48.8584, 100.5, 7.7));

            await SetUpJsonWriterAndRunTestAsync(
                point,
                payload =>
                {
                    Assert.Equal("{\"type\":\"Point\",\"coordinates\":[2.2945,48.8584,100.5,7.7],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}", payload);
                });
        }

        [Fact]
        public async Task WriteLineStringAsync()
        {
            var lineString = geometryFactory.CreateLineString(
            [
                new Coordinate(2.2945, 48.8584),
                new Coordinate(2.3365, 48.8610),
                new Coordinate(2.3376, 48.8606)
            ]);

            await SetUpJsonWriterAndRunTestAsync(
                lineString,
                payload =>
                {
                    Assert.Equal("{\"type\":\"LineString\"," +
                        "\"coordinates\":[[2.2945,48.8584],[2.3365,48.861],[2.3376,48.8606]]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}",
                        payload);
                });
        }

        [Fact]
        public async Task WritePolygonAsync()
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

            await SetUpJsonWriterAndRunTestAsync(
                polygon,
                payload =>
                {
                    Assert.Equal("{\"type\":\"Polygon\"," +
                        "\"coordinates\":[[[2.2945,48.8584],[2.2955,48.8589],[2.296,48.8594],[2.295,48.86],[2.2935,48.859],[2.2945,48.8584]]]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}",
                        payload);
                });
        }

        [Fact]
        public async Task WriteMultiPointAsync()
        {
            var multiPoint = geometryFactory.CreateMultiPoint(
            [
                geometryFactory.CreatePoint(new Coordinate(2.2945, 48.8584)),
                geometryFactory.CreatePoint(new Coordinate(2.3365, 48.8610)),
                geometryFactory.CreatePoint(new Coordinate(2.3376, 48.8606))
            ]);

            await SetUpJsonWriterAndRunTestAsync(
                multiPoint,
                payload =>
                {
                    Assert.Equal("{\"type\":\"MultiPoint\"," +
                        "\"coordinates\":[[2.2945,48.8584],[2.3365,48.861],[2.3376,48.8606]]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}",
                        payload);
                });
        }

        [Fact]
        public async Task WriteEmptyMultiPointAsync()
        {
            var multiPoint = geometryFactory.CreateMultiPoint();

            await SetUpJsonWriterAndRunTestAsync(
                multiPoint,
                payload =>
                {
                    Assert.Equal("{\"type\":\"MultiPoint\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}", payload);
                });
        }

        [Fact]
        public async Task WriteMultiLineStringAsync()
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

            await SetUpJsonWriterAndRunTestAsync(
                multiLineString,
                payload =>
                {
                    Assert.Equal("{\"type\":\"MultiLineString\"," +
                        "\"coordinates\":[[[2.2945,48.8584],[2.3365,48.861],[2.3376,48.8606]],[[2.295,48.8738],[2.307,48.8698],[2.3212,48.8656]]]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}",
                        payload);
                });
        }

        [Fact]
        public async Task WriteEmptyMultiLineStringAsync()
        {
            var multiLineString = geometryFactory.CreateMultiLineString();

            await SetUpJsonWriterAndRunTestAsync(
                multiLineString,
                payload =>
                {
                    Assert.Equal("{\"type\":\"MultiLineString\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}", payload);
                });
        }

        [Fact]
        public async Task WriteMultiPolygonAsync()
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

            await SetUpJsonWriterAndRunTestAsync(
                multiPolygon,
                payload =>
                {
                    Assert.Equal("{\"type\":\"MultiPolygon\"," +
                        "\"coordinates\":[[[[2.2945,48.8584],[2.2955,48.8589],[2.296,48.8594],[2.295,48.86],[2.2935,48.859],[2.2945,48.8584]]],[[[2.3376,48.8606],[2.3365,48.861],[2.335,48.86],[2.3376,48.8606]]]]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}",
                        payload);
                });
        }

        [Fact]
        public async Task WriteEmptyMultiPolygonAsync()
        {
            var multiPolygon = geometryFactory.CreateMultiPolygon();

            await SetUpJsonWriterAndRunTestAsync(
                multiPolygon,
                payload =>
                {
                    Assert.Equal("{\"type\":\"MultiPolygon\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}", payload);
                });
        }

        [Fact]
        public async Task WriteGeometryCollectionAsync()
        {
            var geometryCollection = geometryFactory.CreateGeometryCollection(
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

            await SetUpJsonWriterAndRunTestAsync(
                geometryCollection,
                payload =>
                {
                    Assert.Equal("{\"type\":\"GeometryCollection\",\"geometries\":[" +
                        "{\"type\":\"Point\",\"coordinates\":[2.2945,48.8584]}," +
                        "{\"type\":\"LineString\",\"coordinates\":[[2.2945,48.8584],[2.3365,48.861],[2.3376,48.8606]]}," +
                        "{\"type\":\"Polygon\",\"coordinates\":[[[2.2945,48.8584],[2.2955,48.8589],[2.296,48.8594],[2.295,48.86],[2.2935,48.859],[2.2945,48.8584]]]}," +
                        "{\"type\":\"MultiPoint\",\"coordinates\":[[2.2945,48.8584],[2.3365,48.861],[2.3376,48.8606]]}," +
                        "{\"type\":\"MultiLineString\",\"coordinates\":[[[2.2945,48.8584],[2.3365,48.861],[2.3376,48.8606]],[[2.295,48.8738],[2.307,48.8698],[2.3212,48.8656]]]}," +
                        "{\"type\":\"MultiPolygon\"," +
                        "\"coordinates\":[[[[2.2945,48.8584],[2.2955,48.8589],[2.296,48.8594],[2.295,48.86],[2.2935,48.859],[2.2945,48.8584]]],[[[2.3376,48.8606],[2.3365,48.861],[2.335,48.86],[2.3376,48.8606]]]]}]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}",
                        payload);
                });
        }

        [Fact]
        public async Task WriteEmptyGeometryCollectionAsync()
        {
            var geometryCollection = geometryFactory.CreateGeometryCollection();

            await SetUpJsonWriterAndRunTestAsync(
                geometryCollection,
                payload =>
                {
                    Assert.Equal("{\"type\":\"GeometryCollection\",\"geometries\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}", payload);
                });
        }

        [Fact]
        public async Task WritePolygonWithHolesAsync()
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

            await SetUpJsonWriterAndRunTestAsync(
                polygon,
                payload =>
                {
                    Assert.Equal("{\"type\":\"Polygon\",\"coordinates\":[[[2.2945,48.8584],[2.2955,48.8589],[2.296,48.8594],[2.295,48.86],[2.2935,48.859],[2.2945,48.8584]]," +
                        "[[2.295,48.8587],[2.2952,48.8588],[2.2948,48.8588],[2.295,48.8587]]," +
                        "[[2.2947,48.8586],[2.2949,48.8587],[2.2948,48.8589],[2.2946,48.8588],[2.2947,48.8586]]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}",
                        payload);
                });
        }

        [Fact]
        public async Task WritePointWithNonDefaultSridAsync()
        {
            var nonDefaultGeometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(3857);
            var point = nonDefaultGeometryFactory.CreatePoint(new Coordinate(2.2945, 48.8584));

            await SetUpJsonWriterAndRunTestAsync(
                point,
                payload =>
                {
                    Assert.Equal("{\"type\":\"Point\",\"coordinates\":[2.2945,48.8584],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:3857\"}}}", payload);
                });
        }

        [Fact]
        public void WritePoint()
        {
            var point = geometryFactory.CreatePoint(new Coordinate(2.2945, 48.8584));

            SetUpJsonWriterAndRunTest(
                point,
                (payload) =>
                {
                    Assert.Equal("{\"type\":\"Point\",\"coordinates\":[2.2945,48.8584],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}", payload);
                });
        }

        [Fact]
        public void WritePoint3D()
        {
            var point = geometryFactory.CreatePoint(new CoordinateZ(2.2945, 48.8584, 100.5));

            SetUpJsonWriterAndRunTest(
                point,
                payload =>
                {
                    Assert.Equal("{\"type\":\"Point\",\"coordinates\":[2.2945,48.8584,100.5],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}", payload);
                });
        }

        [Fact]
        public void WritePoint4D()
        {
            var point = new Point(new CoordinateZM(2.2945, 48.8584, 100.5, 7.7)) { SRID = 4326 };

            SetUpJsonWriterAndRunTest(
                point,
                payload =>
                {
                    Assert.Equal("{\"type\":\"Point\",\"coordinates\":[2.2945,48.8584,100.5,7.7],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}", payload);
                });
        }

        [Fact]
        public void WriteLineString()
        {
            var lineString = geometryFactory.CreateLineString(new[]
            {
                new Coordinate(2.2945, 48.8584),
                new Coordinate(2.3365, 48.8610),
                new Coordinate(2.3376, 48.8606)
            });

            SetUpJsonWriterAndRunTest(
                lineString,
                payload =>
                {
                    Assert.Equal("{\"type\":\"LineString\"," +
                        "\"coordinates\":[[2.2945,48.8584],[2.3365,48.861],[2.3376,48.8606]]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}",
                        payload);
                });
        }

        [Fact]
        public void WritePolygon()
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

            SetUpJsonWriterAndRunTest(
                polygon,
                payload =>
                {
                    Assert.Equal("{\"type\":\"Polygon\"," +
                        "\"coordinates\":[[[2.2945,48.8584],[2.2955,48.8589],[2.296,48.8594],[2.295,48.86],[2.2935,48.859],[2.2945,48.8584]]]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}",
                        payload);
                });
        }

        [Fact]
        public void WriteMultiPoint()
        {
            var multiPoint = geometryFactory.CreateMultiPoint(
            [
                geometryFactory.CreatePoint(new Coordinate(2.2945, 48.8584)),
                geometryFactory.CreatePoint(new Coordinate(2.3365, 48.8610)),
                geometryFactory.CreatePoint(new Coordinate(2.3376, 48.8606))
            ]);

            SetUpJsonWriterAndRunTest(
                multiPoint,
                payload =>
                {
                    Assert.Equal("{\"type\":\"MultiPoint\"," +
                        "\"coordinates\":[[2.2945,48.8584],[2.3365,48.861],[2.3376,48.8606]]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}",
                        payload);
                });
        }

        [Fact]
        public void WriteEmptyMultiPoint()
        {
            var multiPoint = geometryFactory.CreateMultiPoint();

            SetUpJsonWriterAndRunTest(
                multiPoint,
                payload =>
                {
                    Assert.Equal("{\"type\":\"MultiPoint\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}", payload);
                });
        }

        [Fact]
        public void WriteMultiLineString()
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

            SetUpJsonWriterAndRunTest(
                multiLineString,
                payload =>
                {
                    Assert.Equal("{\"type\":\"MultiLineString\"," +
                        "\"coordinates\":[[[2.2945,48.8584],[2.3365,48.861],[2.3376,48.8606]],[[2.295,48.8738],[2.307,48.8698],[2.3212,48.8656]]]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}",
                        payload);
                });
        }

        [Fact]
        public void WriteEmptyMultiLineString()
        {
            var multiLineString = geometryFactory.CreateMultiLineString();

            SetUpJsonWriterAndRunTest(
                multiLineString,
                payload =>
                {
                    Assert.Equal("{\"type\":\"MultiLineString\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}", payload);
                });
        }

        [Fact]
        public void WriteMultiPolygon()
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

            SetUpJsonWriterAndRunTest(
                multiPolygon,
                payload =>
                {
                    Assert.Equal("{\"type\":\"MultiPolygon\"," +
                        "\"coordinates\":[[[[2.2945,48.8584],[2.2955,48.8589],[2.296,48.8594],[2.295,48.86],[2.2935,48.859],[2.2945,48.8584]]],[[[2.3376,48.8606],[2.3365,48.861],[2.335,48.86],[2.3376,48.8606]]]]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}",
                        payload);
                });
        }

        [Fact]
        public void WriteEmptyMultiPolygon()
        {
            var multiPolygon = geometryFactory.CreateMultiPolygon();

            SetUpJsonWriterAndRunTest(
                multiPolygon,
                payload =>
                {
                    Assert.Equal("{\"type\":\"MultiPolygon\",\"coordinates\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}", payload);
                });
        }

        [Fact]
        public void WriteGeometryCollection()
        {
            var geometryCollection = geometryFactory.CreateGeometryCollection(
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

            SetUpJsonWriterAndRunTest(
                geometryCollection,
                payload =>
                {
                    Assert.Equal("{\"type\":\"GeometryCollection\",\"geometries\":[" +
                        "{\"type\":\"Point\",\"coordinates\":[2.2945,48.8584]}," +
                        "{\"type\":\"LineString\",\"coordinates\":[[2.2945,48.8584],[2.3365,48.861],[2.3376,48.8606]]}," +
                        "{\"type\":\"Polygon\",\"coordinates\":[[[2.2945,48.8584],[2.2955,48.8589],[2.296,48.8594],[2.295,48.86],[2.2935,48.859],[2.2945,48.8584]]]}," +
                        "{\"type\":\"MultiPoint\",\"coordinates\":[[2.2945,48.8584],[2.3365,48.861],[2.3376,48.8606]]}," +
                        "{\"type\":\"MultiLineString\",\"coordinates\":[[[2.2945,48.8584],[2.3365,48.861],[2.3376,48.8606]],[[2.295,48.8738],[2.307,48.8698],[2.3212,48.8656]]]}," +
                        "{\"type\":\"MultiPolygon\"," +
                        "\"coordinates\":[[[[2.2945,48.8584],[2.2955,48.8589],[2.296,48.8594],[2.295,48.86],[2.2935,48.859],[2.2945,48.8584]]],[[[2.3376,48.8606],[2.3365,48.861],[2.335,48.86],[2.3376,48.8606]]]]}]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}",
                        payload);
                });
        }

        [Fact]
        public void WriteEmptyGeometryCollection()
        {
            var geometryCollection = geometryFactory.CreateGeometryCollection();

            SetUpJsonWriterAndRunTest(
                geometryCollection,
                payload =>
                {
                    Assert.Equal("{\"type\":\"GeometryCollection\",\"geometries\":[],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}", payload);
                });
        }

        [Fact]
        public void WritePolygonWithHoles()
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

            SetUpJsonWriterAndRunTest(
                polygon,
                payload =>
                {
                    Assert.Equal("{\"type\":\"Polygon\"," +
                        "\"coordinates\":[[[2.2945,48.8584],[2.2955,48.8589],[2.296,48.8594],[2.295,48.86],[2.2935,48.859],[2.2945,48.8584]]," +
                        "[[2.295,48.8587],[2.2952,48.8588],[2.2948,48.8588],[2.295,48.8587]],[[2.2947,48.8586],[2.2949,48.8587],[2.2948,48.8589],[2.2946,48.8588],[2.2947,48.8586]]]," +
                        "\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}",
                        payload);
                });
        }

        [Fact]
        public void WritePointWithNonDefaultSrid()
        {
            var nonDefaultGeometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(3857);
            var point = nonDefaultGeometryFactory.CreatePoint(new Coordinate(2.2945, 48.8584));

            SetUpJsonWriterAndRunTest(
                point,
                payload =>
                {
                    Assert.Equal("{\"type\":\"Point\",\"coordinates\":[2.2945,48.8584],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:3857\"}}}", payload);
                });
        }

        private async Task SetUpJsonWriterAndRunTestAsync(Geometry geometry, Action<string> verifyPayload)
        {
            var stream = new MemoryStream();
            var jsonWriter = new ODataUtf8JsonWriter(stream, isIeee754Compatible: false, encoding: Encoding.UTF8);

            var point = geometryFactory.CreatePoint(new Coordinate(2.2945, 48.8584));

            await new SpatialGeometryTypeConverter().WriteJsonAsync(geometry, jsonWriter);

            await jsonWriter.FlushAsync();
            stream.Position = 0;
            var result = await new StreamReader(stream).ReadToEndAsync();

            verifyPayload(result);
        }

        private void SetUpJsonWriterAndRunTest(Geometry geometry, Action<string> verifyPayload)
        {
            var stream = new MemoryStream();
            var jsonWriter = new ODataUtf8JsonWriter(stream, isIeee754Compatible: false, encoding: Encoding.UTF8);

            var point = geometryFactory.CreatePoint(new Coordinate(2.2945, 48.8584));

            new SpatialGeometryTypeConverter().WriteJson(geometry, jsonWriter);

            jsonWriter.Flush();
            stream.Position = 0;
            var result = new StreamReader(stream).ReadToEnd();

            verifyPayload(result);
        }
    }
}

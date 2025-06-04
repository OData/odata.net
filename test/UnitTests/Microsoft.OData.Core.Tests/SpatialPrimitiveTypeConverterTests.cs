//---------------------------------------------------------------------
// <copyright file="SpatialPrimitiveTypeConverterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Json;
using Microsoft.OData.Spatial;
using Xunit;
using Coordinate = NetTopologySuite.Geometries.Coordinate;
using CoordinateZ = NetTopologySuite.Geometries.CoordinateZ;
using CoordinateZM = NetTopologySuite.Geometries.CoordinateZM;

namespace Microsoft.OData.Tests
{
    public class SpatialPrimitiveTypeConverterTests
    {
        [Fact]
        public async Task WritePointAsync()
        {
            var point = GeographyFactory.Default.CreatePoint(new Coordinate(2.2945, 48.8584));

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
            var point = GeographyFactory.Default.CreatePoint(new CoordinateZ(2.2945, 48.8584, 100.5));

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
            var point = GeographyFactory.Default.CreatePoint(new CoordinateZM(2.2945, 48.8584, 100.5, 7.7));

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
            var lineString = GeographyFactory.Default.CreateLineString(
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
            var polygon = GeographyFactory.Default.CreatePolygon(
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
            var multiPoint = GeographyFactory.Default.CreateMultiPoint(
            [
                GeographyFactory.Default.CreatePoint(new Coordinate(2.2945, 48.8584)),
                GeographyFactory.Default.CreatePoint(new Coordinate(2.3365, 48.8610)),
                GeographyFactory.Default.CreatePoint(new Coordinate(2.3376, 48.8606))
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
            var multiPoint = GeographyFactory.Default.CreateMultiPoint();

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
            var multiLineString = GeographyFactory.Default.CreateMultiLineString();

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
            var multiPolygon = GeographyFactory.Default.CreateMultiPolygon();

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
            var geographyCollection = GeographyFactory.Default.CreateGeographyCollection(
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

            await SetUpJsonWriterAndRunTestAsync(
                geographyCollection,
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
            var geometryCollection = GeographyFactory.Default.CreateGeographyCollection();

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
            var polygon = GeographyFactory.Default.CreatePolygon(exterior, [hole1, hole2]);

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
            var point = GeographyFactory.Create(3857).CreatePoint(new Coordinate(2.2945, 48.8584));

            await SetUpJsonWriterAndRunTestAsync(
                point,
                payload =>
                {
                    Assert.Equal("{\"type\":\"Point\",\"coordinates\":[2.2945,48.8584],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:3857\"}}}", payload);
                });
        }

        [Fact]
        public async Task WritePointWithGeoJsonPrimitiveTypeConverter_CrsNotEmittedAsync()
        {
            var stream = new MemoryStream();
            var point = GeographyFactory.Default.CreatePoint(new Coordinate(2.2945, 48.8584));
            var jsonWriter = new ODataUtf8JsonWriter(stream, isIeee754Compatible: false, encoding: Encoding.UTF8);
            await new GeoJsonPrimitiveTypeConverter().WriteJsonAsync(point, jsonWriter);

            await jsonWriter.FlushAsync();
            stream.Position = 0;
            var result = await new StreamReader(stream).ReadToEndAsync();

            Assert.Equal(
                "{\"type\":\"Point\",\"coordinates\":[2.2945,48.8584]}",
                result);
        }

        [Fact]
        public void WritePoint()
        {
            var point = GeographyFactory.Default.CreatePoint(new Coordinate(2.2945, 48.8584));

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
            var point = GeographyFactory.Default.CreatePoint(new CoordinateZ(2.2945, 48.8584, 100.5));

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
            var point = GeographyFactory.Default.CreatePoint(new CoordinateZM(2.2945, 48.8584, 100.5, 7.7));

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
            var lineString = GeographyFactory.Default.CreateLineString(new[]
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
            var polygon = GeographyFactory.Default.CreatePolygon(
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
            var multiPoint = GeographyFactory.Default.CreateMultiPoint(
            [
                GeographyFactory.Default.CreatePoint(new Coordinate(2.2945, 48.8584)),
                GeographyFactory.Default.CreatePoint(new Coordinate(2.3365, 48.8610)),
                GeographyFactory.Default.CreatePoint(new Coordinate(2.3376, 48.8606))
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
            var multiPoint = GeographyFactory.Default.CreateMultiPoint();

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
            var multiLineString = GeographyFactory.Default.CreateMultiLineString();

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
            var multiPolygon = GeographyFactory.Default.CreateMultiPolygon();

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
            var geometryCollection = GeographyFactory.Default.CreateGeographyCollection(
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
            var geometryCollection = GeographyFactory.Default.CreateGeographyCollection();

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
            var polygon = GeographyFactory.Default.CreatePolygon(exterior, [hole1, hole2]);

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
            var point = GeographyFactory.Create(3857).CreatePoint(new Coordinate(2.2945, 48.8584));

            SetUpJsonWriterAndRunTest(
                point,
                payload =>
                {
                    Assert.Equal("{\"type\":\"Point\",\"coordinates\":[2.2945,48.8584],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:3857\"}}}", payload);
                });
        }

        [Fact]
        public void WritePointWithGeoJsonPrimitiveTypeConverter_CrsNotEmitted()
        {
            var stream = new MemoryStream();
            var point = GeographyFactory.Default.CreatePoint(new Coordinate(2.2945, 48.8584));
            var jsonWriter = new ODataUtf8JsonWriter(stream, isIeee754Compatible: false, encoding: Encoding.UTF8);
            new GeoJsonPrimitiveTypeConverter().WriteJson(point, jsonWriter);

            jsonWriter.Flush();
            stream.Position = 0;
            var result = new StreamReader(stream).ReadToEnd();

            Assert.Equal(
                "{\"type\":\"Point\",\"coordinates\":[2.2945,48.8584]}",
                result);
        }

        private async Task SetUpJsonWriterAndRunTestAsync(Geography geography, Action<string> verifyPayload)
        {
            var stream = new MemoryStream();
            var jsonWriter = new ODataUtf8JsonWriter(stream, isIeee754Compatible: false, encoding: Encoding.UTF8);

            var point = GeographyFactory.Default.CreatePoint(new Coordinate(2.2945, 48.8584));

            await new SpatialPrimitiveTypeConverter().WriteJsonAsync(geography, jsonWriter);

            await jsonWriter.FlushAsync();
            stream.Position = 0;
            var result = await new StreamReader(stream).ReadToEndAsync();

            verifyPayload(result);
        }

        private void SetUpJsonWriterAndRunTest(Geography geography, Action<string> verifyPayload)
        {
            var stream = new MemoryStream();
            var jsonWriter = new ODataUtf8JsonWriter(stream, isIeee754Compatible: false, encoding: Encoding.UTF8);

            var point = GeographyFactory.Default.CreatePoint(new Coordinate(2.2945, 48.8584));

            new SpatialPrimitiveTypeConverter().WriteJson(geography, jsonWriter);

            jsonWriter.Flush();
            stream.Position = 0;
            var result = new StreamReader(stream).ReadToEnd();

            verifyPayload(result);
        }
    }
}

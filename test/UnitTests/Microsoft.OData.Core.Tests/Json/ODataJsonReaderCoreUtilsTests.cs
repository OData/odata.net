//---------------------------------------------------------------------
// <copyright file="ODataJsonReaderCoreUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Microsoft.OData.Spatial;
using NtsPoint = NetTopologySuite.Geometries.Point;
using NtsLineString = NetTopologySuite.Geometries.LineString;
using NtsPolygon = NetTopologySuite.Geometries.Polygon;
using NtsMultiPoint = NetTopologySuite.Geometries.MultiPoint;
using NtsMultiLineString = NetTopologySuite.Geometries.MultiLineString;
using NtsMultiPolygon = NetTopologySuite.Geometries.MultiPolygon;
using NtsGeometryCollection = NetTopologySuite.Geometries.GeometryCollection;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    public class ODataJsonReaderCoreUtilsTests
    {
        private ODataInputContext inputContext;

        public ODataJsonReaderCoreUtilsTests()
        {
            var messageReaderSettings = new ODataMessageReaderSettings
            {
                MessageQuotas = new ODataMessageQuotas
                {
                    MaxNestingDepth = 10
                }
            };

            var messageInfo = new ODataMessageInfo
            {
                Encoding = Encoding.UTF8,
                IsResponse = true,
                MediaType = new ODataMediaType("application", "json"),
                IsAsync = false,
                Model = EdmCoreModel.Instance,
                MessageStream = new MemoryStream(),
                ServiceProvider = null
            };

            this.inputContext = new ODataJsonInputContext(messageInfo, messageReaderSettings);
        }

        public static IEnumerable<object[]> GetReadSpatialPropertyData()
        {
            yield return new object[]
            {
                "PointProp",
                "{\"PointProp\":{\"type\":\"Point\",\"coordinates\":[-110.0,33.1],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, true),
                new Action<object>((spatialValue) =>
                {
                    var geographyPoint = Assert.IsAssignableFrom<GeographyPoint>(spatialValue);
                    var point = Assert.IsType<NtsPoint>(geographyPoint.Geometry);
                    Assert.Equal(4326, point.SRID);
                    Assert.Equal(-110.0d, point.X);
                    Assert.Equal(33.1d, point.Y);
                })
            };


            yield return new object[]
            {
                "LineStringProp",
                "{\"LineStringProp\":{\"type\":\"LineString\",\"coordinates\":[[-110.0,33.1],[-110.0,35.97]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyLineString, true),
                new Action<object>((spatialValue) =>
                {
                    var geographyLineString = Assert.IsAssignableFrom<GeographyLineString>(spatialValue);
                    var lineString = Assert.IsType<NtsLineString>(geographyLineString.Geometry);
                    Assert.Equal(4326, lineString.SRID);

                    var points = new double[,] { { -110.0, 33.1 }, { -110.0, 35.97 } };
                    Assert.Equal(points.GetLength(0), lineString.Coordinates.Length);

                    for (var i = 0; i < points.GetLength(0); i++)
                    {
                        Assert.Equal(points[i,0], lineString.Coordinates[i].X);
                        Assert.Equal(points[i,1], lineString.Coordinates[i].Y);
                    }
                })
            };


            yield return new object[]
            {
                "PolygonProp",
                "{\"PolygonProp\":{\"type\":\"Polygon\",\"coordinates\":[[[-110.0,33.1],[-110.15,35.97],[87.75,11.45],[-110.0,33.1]],[[-110.0,35.97],[-110.15,36.97],[23.18,45.23],[-110.0,35.97]]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPolygon, true),
                new Action<object>((spatialValue) =>
                {
                    var geographyPolygon = Assert.IsAssignableFrom<GeographyPolygon>(spatialValue);
                    var polygon = Assert.IsType<NtsPolygon>(geographyPolygon.Geometry);
                    Assert.Equal(4326, polygon.SRID);

                    var rings = new double[2][,]{ new double[,] { { -110.0, 33.1 }, { -110.15, 35.97 }, { 87.75, 11.45 }, { -110.0, 33.1 } }, new double[,]{ { -110.0, 35.97 }, { -110.15, 36.97 }, { 23.18, 45.23 }, { -110.0, 35.97 } } };
                    Assert.Equal(rings.Length, 1 /* Exterior Ring */ + polygon.InteriorRings.Length);

                    for (var i = 0; i < rings.GetLength(0); i++)
                    {
                        NtsLineString ring = i == 0 ? polygon.ExteriorRing : polygon.InteriorRings[i - 1];

                        if (i == 0)
                        Assert.Equal(rings[i].GetLength(0), ring.Coordinates.Length);

                        for (var j = 0; j < rings[i].GetLength(0); j++)
                        {
                            Assert.Equal(rings[i][j,0], ring.Coordinates[j].X);
                            Assert.Equal(rings[i][j,1], ring.Coordinates[j].Y);
                        }
                    }
                })
            };


            yield return new object[]
            {
                "MultiPointProp",
                "{\"MultiPointProp\":{\"type\":\"MultiPoint\",\"coordinates\":[[11.2,10.2],[11.6,11.9]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyMultiPoint, true),
                new Action<object>((spatialValue) =>
                {
                    var geographyMultiPoint = Assert.IsAssignableFrom<GeographyMultiPoint>(spatialValue);
                    var multiPoint = Assert.IsType<NtsMultiPoint>(geographyMultiPoint.Geometry);
                    Assert.Equal(4326, multiPoint.SRID);

                    var points = new double[,]{ { 11.2,10.2 }, { 11.6,11.9 } };
                    Assert.Equal(points.GetLength(0), multiPoint.Geometries.Length);

                    for (var i = 0; i < points.GetLength(0); i++)
                    {
                        var geographyPoint = Assert.IsAssignableFrom<NtsPoint>(multiPoint.Geometries[i]);
                        Assert.Equal(points[i,0], geographyPoint.X);
                        Assert.Equal(points[i,1], geographyPoint.Y);
                    }
                })
            };


            yield return new object[]
            {
                "MultiLineStringProp",
                "{\"MultiLineStringProp\":{\"type\":\"MultiLineString\",\"coordinates\":[[[11.2,10.2],[11.6,11.9]],[[17.2,16.2],[19.6,18.9]]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyMultiLineString, true),
                new Action<object>((spatialValue) =>
                {
                    var geographyMultiString = Assert.IsAssignableFrom<GeographyMultiLineString>(spatialValue);
                    var multiLineString = Assert.IsType<NtsMultiLineString>(geographyMultiString.Geometry);
                    Assert.Equal(4326, multiLineString.SRID);

                    var lineStrings = new double[2][,]{ new double[,] { { 11.2, 10.2 }, { 11.6, 11.9 } }, new double[,]{ { 17.2, 16.2 },{ 19.6, 18.9 } } };
                    Assert.Equal(lineStrings.GetLength(0), multiLineString.Geometries.Length);

                    for (var i = 0; i < lineStrings.GetLength(0); i++)
                    {
                        Assert.Equal(lineStrings[i].GetLength(0), multiLineString.Geometries[i].Coordinates.Length);

                        for (var j = 0; j < lineStrings[i].GetLength(0); j++)
                        {
                            var geographyLineString = Assert.IsAssignableFrom<NtsLineString>(multiLineString.Geometries[i]);
                            Assert.Equal(lineStrings[i][j,0], geographyLineString.Coordinates[j].X);
                            Assert.Equal(lineStrings[i][j,1], geographyLineString.Coordinates[j].Y);
                        }
                    }
                })
            };


            yield return new object[]
            {
                "MultiPolygonProp",
                "{\"MultiPolygonProp\":{\"type\":\"MultiPolygon\",\"coordinates\":[[[[11.2,10.2],[11.6,11.9],[87.75,11.45],[11.2,10.2]],[[17.2,16.2],[19.6,18.9],[87.75,11.45],[17.2,16.2]]]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyMultiPolygon, true),
                new Action<object>((spatialValue) =>
                {
                    var geographyMultiPolygon = Assert.IsAssignableFrom<GeographyMultiPolygon>(spatialValue);
                    var multiPolygon = Assert.IsType<NtsMultiPolygon>(geographyMultiPolygon.Geometry);
                    Assert.Equal(4326, multiPolygon.SRID);
                    var geographyPolygon = Assert.IsType<NtsPolygon>(Assert.Single(multiPolygon.Geometries));

                    var rings = new double[2][,] { new double[,] { { 11.2, 10.2 }, { 11.6, 11.9 }, { 87.75, 11.45 }, { 11.2, 10.2 } }, new double[,]{ { 17.2, 16.2 }, { 19.6, 18.9 }, { 87.75, 11.45 }, { 17.2, 16.2 } } };
                    Assert.Equal(rings.GetLength(0), 1 /* exterior ring */ +  geographyPolygon.InteriorRings.Length);

                    for (var i = 0; i < rings.GetLength(0); i++)
                    {
                        var ring = i == 0 ? geographyPolygon.ExteriorRing : geographyPolygon.InteriorRings[i - 1];
                        Assert.Equal(rings[i].GetLength(0), ring.Coordinates.Length);

                        for (var j = 0; j < rings[i].GetLength(0); j++)
                        {
                            Assert.Equal(rings[i][j,0], ring.Coordinates[j].X);
                            Assert.Equal(rings[i][j,1], ring.Coordinates[j].Y);
                        }
                    }
                })
            };


            yield return new object[]
            {
                "GeometryCollectionProp",
                "{\"GeometryCollectionProp\":{\"type\":\"GeometryCollection\",\"geometries\":[{\"type\":\"Point\",\"coordinates\":[-12.0,-19.99]}],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryCollection, true),
                new Action<object>((spatialValue) =>
                {
                    var geographyCollection = Assert.IsAssignableFrom<GeometryCollection>(spatialValue);
                    var geometryCollection = Assert.IsType<NtsGeometryCollection>(geographyCollection.Geometry);
                    Assert.Equal(4326, geometryCollection.SRID);
                    Assert.Single(geometryCollection.Geometries);
                    var geometryPoint = Assert.IsAssignableFrom<NtsPoint>(geometryCollection.Geometries[0]);
                    Assert.Equal(-12.0, geometryPoint.X);
                    Assert.Equal(-19.99, geometryPoint.Y);
                })
            };
        }

        [Theory]
        [MemberData(nameof(GetReadSpatialPropertyData))]
        public async Task ReadSpatialPropertyAsync(string propertyName, string payload, IEdmPrimitiveTypeReference edmPrimitiveTypeReference, Action<object> assertAction)
        {
            using (var reader = await CreateJsonReaderAsync(payload))
            {
                var spatialValue = await ODataJsonReaderCoreUtils.ReadSpatialValueAsync(
                    jsonReader: reader,
                    insideJsonObjectValue: false,
                    inputContext: this.inputContext,
                    expectedValueTypeReference: edmPrimitiveTypeReference,
                    validateNullValue: false,
                    recursionDepth: -1,
                    propertyName: propertyName);

                assertAction(spatialValue);
            }
        }

        [Fact]
        public async Task ReadNullSpatialPropertyAsync()
        {
            using (var reader = await CreateJsonReaderAsync("{\"NullSpatialProp\":null}"))
            {
                var spatialValue = await ODataJsonReaderCoreUtils.ReadSpatialValueAsync(
                    jsonReader: reader,
                    insideJsonObjectValue: false,
                    inputContext: this.inputContext,
                    expectedValueTypeReference: EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, true),
                    validateNullValue: false,
                    recursionDepth: -1,
                    propertyName: "NullSpatialProp");

                Assert.Null(spatialValue);
            }
        }

        [Theory]
        [MemberData(nameof(GetReadSpatialPropertyData))]
        public void ReadSpatialProperty(string propertyName, string payload, IEdmPrimitiveTypeReference edmPrimitiveTypeReference, Action<object> assertAction)
        {
            using (var reader = CreateJsonReader(payload))
            {
                var spatialValue = ODataJsonReaderCoreUtils.ReadSpatialValue(
                    jsonReader: reader,
                    insideJsonObjectValue: false,
                    inputContext: this.inputContext,
                    expectedValueTypeReference: edmPrimitiveTypeReference,
                    validateNullValue: false,
                    recursionDepth: -1,
                    propertyName: propertyName);

                assertAction(spatialValue);
            }
        }

        [Fact]
        public void ReadNullSpatialProperty()
        {
            using (var reader = CreateJsonReader("{\"NullSpatialProp\":null}"))
            {
                var spatialValue = ODataJsonReaderCoreUtils.ReadSpatialValue(
                    jsonReader: reader,
                    insideJsonObjectValue: false,
                    inputContext: this.inputContext,
                    expectedValueTypeReference: EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, true),
                    validateNullValue: false,
                    recursionDepth: -1,
                    propertyName: "NullSpatialProp");

                Assert.Null(spatialValue);
            }
        }

        private async Task<JsonReader> CreateJsonReaderAsync(string payload, bool isIeee754Compatible = false)
        {
            JsonReader reader = new JsonReader(new StringReader(payload), isIeee754Compatible: isIeee754Compatible);

            await reader.ReadAsync();
            await reader.ReadStartObjectAsync();
            await reader.ReadPropertyNameAsync();

            return reader;
        }

        private JsonReader CreateJsonReader(string payload, bool isIeee754Compatible = false)
        {
            JsonReader reader = new JsonReader(new StringReader(payload), isIeee754Compatible: isIeee754Compatible);

            reader.Read();
            reader.ReadStartObject();
            reader.ReadPropertyName();

            return reader;
        }
    }
}

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
using NetTopologySuite.Geometries;
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
                new Action<Geometry>((spatialValue) =>
                {
                    var geographyPoint = Assert.IsAssignableFrom<Point>(spatialValue);
                    Assert.Equal(4326, geographyPoint.SRID);
                    Assert.Equal(-110.0d, geographyPoint.X);
                    Assert.Equal(33.1d, geographyPoint.Y);
                })
            };


            yield return new object[]
            {
                "LineStringProp",
                "{\"LineStringProp\":{\"type\":\"LineString\",\"coordinates\":[[-110.0,33.1],[-110.0,35.97]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyLineString, true),
                new Action<Geometry>((spatialValue) =>
                {
                    var geographyLineString = Assert.IsAssignableFrom<LineString>(spatialValue);
                    Assert.Equal(4326, geographyLineString.SRID);

                    var points = new double[,] { { -110.0, 33.1 }, { -110.0, 35.97 } };
                    Assert.Equal(points.GetLength(0), geographyLineString.Coordinates.Length);

                    for (var i = 0; i < points.GetLength(0); i++)
                    {
                        Assert.Equal(points[i,0], geographyLineString.Coordinates[i].X);
                        Assert.Equal(points[i,1], geographyLineString.Coordinates[i].Y);
                    }
                })
            };


            yield return new object[]
            {
                "PolygonProp",
                "{\"PolygonProp\":{\"type\":\"Polygon\",\"coordinates\":[[[-110.0,33.1],[-110.15,35.97],[87.75,11.45],[-110.0,33.1]],[[-110.0,35.97],[-110.15,36.97],[23.18,45.23],[-110.0,35.97]]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPolygon, true),
                new Action<Geometry>((spatialValue) =>
                {
                    var geographyPolygon = Assert.IsAssignableFrom<Polygon>(spatialValue);
                    Assert.Equal(4326, geographyPolygon.SRID);

                    var rings = new double[2][,]{ new double[,] { { -110.0, 33.1 }, { -110.15, 35.97 }, { 87.75, 11.45 }, { -110.0, 33.1 } }, new double[,]{ { -110.0, 35.97 }, { -110.15, 36.97 }, { 23.18, 45.23 }, { -110.0, 35.97 } } };
                    Assert.Equal(rings.Length, 1 /* Exterior Ring */ + geographyPolygon.InteriorRings.Length);

                    for (var i = 0; i < rings.GetLength(0); i++)
                    {
                        LineString ring = i == 0 ? geographyPolygon.ExteriorRing : geographyPolygon.InteriorRings[i - 1];

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
                new Action<Geometry>((spatialValue) =>
                {
                    var geographyMultiPoint = Assert.IsAssignableFrom<MultiPoint>(spatialValue);
                    Assert.Equal(4326, geographyMultiPoint.SRID);

                    var points = new double[,]{ { 11.2,10.2 }, { 11.6,11.9 } };
                    Assert.Equal(points.GetLength(0), geographyMultiPoint.Geometries.Length);

                    for (var i = 0; i < points.GetLength(0); i++)
                    {
                        var geographyPoint = Assert.IsAssignableFrom<Point>(geographyMultiPoint.Geometries[i]);
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
                new Action<Geometry>((spatialValue) =>
                {
                    var geographyMultiString = Assert.IsAssignableFrom<MultiLineString>(spatialValue);
                    Assert.Equal(4326, geographyMultiString.SRID);

                    var lineStrings = new double[2][,]{ new double[,] { { 11.2, 10.2 }, { 11.6, 11.9 } }, new double[,]{ { 17.2, 16.2 },{ 19.6, 18.9 } } };
                    Assert.Equal(lineStrings.GetLength(0), geographyMultiString.Geometries.Length);

                    for (var i = 0; i < lineStrings.GetLength(0); i++)
                    {
                        Assert.Equal(lineStrings[i].GetLength(0), geographyMultiString.Geometries[i].Coordinates.Length);

                        for (var j = 0; j < lineStrings[i].GetLength(0); j++)
                        {
                            var geographyLineString = Assert.IsAssignableFrom<LineString>(geographyMultiString.Geometries[i]);
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
                new Action<Geometry>((spatialValue) =>
                {
                    var geographyMultiPolygon = Assert.IsAssignableFrom<MultiPolygon>(spatialValue);
                    Assert.Equal(4326, geographyMultiPolygon.SRID);
                    var geographyPolygon = Assert.IsType<Polygon>(Assert.Single(geographyMultiPolygon.Geometries));

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
                new Action<Geometry>((spatialValue) =>
                {
                    var geometryCollection = Assert.IsAssignableFrom<GeometryCollection>(spatialValue);
                    Assert.Equal(4326, geometryCollection.SRID);
                    Assert.Single(geometryCollection.Geometries);
                    var geometryPoint = Assert.IsAssignableFrom<Point>(geometryCollection.Geometries[0]);
                    Assert.Equal(-12.0, geometryPoint.X);
                    Assert.Equal(-19.99, geometryPoint.Y);
                })
            };
        }

        [Theory]
        [MemberData(nameof(GetReadSpatialPropertyData))]
        public async Task ReadSpatialPropertyAsync(string propertyName, string payload, IEdmPrimitiveTypeReference edmPrimitiveTypeReference, Action<Geometry> assertAction)
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
        public void ReadSpatialProperty(string propertyName, string payload, IEdmPrimitiveTypeReference edmPrimitiveTypeReference, Action<Geometry> assertAction)
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

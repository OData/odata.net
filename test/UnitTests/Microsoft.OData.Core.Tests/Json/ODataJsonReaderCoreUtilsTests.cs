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
using Microsoft.Spatial;
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
                new Action<ISpatial>((spatialValue) =>
                {
                    var geographyPoint = Assert.IsAssignableFrom<GeographyPoint>(spatialValue);
                    Assert.Equal(4326, geographyPoint.CoordinateSystem.EpsgId);
                    Assert.Equal(-110.0d, geographyPoint.Longitude);
                    Assert.Equal(33.1d, geographyPoint.Latitude);
                })
            };


            yield return new object[]
            {
                "LineStringProp",
                "{\"LineStringProp\":{\"type\":\"LineString\",\"coordinates\":[[-110.0,33.1],[-110.0,35.97]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyLineString, true),
                new Action<ISpatial>((spatialValue) =>
                {
                    var geographyLineString = Assert.IsAssignableFrom<GeographyLineString>(spatialValue);
                    Assert.Equal(4326, geographyLineString.CoordinateSystem.EpsgId);

                    var points = new double[,] { { -110.0, 33.1 }, { -110.0, 35.97 } };
                    Assert.Equal(points.GetLength(0), geographyLineString.Points.Count);

                    for (var i = 0; i < points.GetLength(0); i++)
                    {
                        Assert.Equal(points[i,0], geographyLineString.Points[i].Longitude);
                        Assert.Equal(points[i,1], geographyLineString.Points[i].Latitude);
                    }
                })
            };


            yield return new object[]
            {
                "PolygonProp",
                "{\"PolygonProp\":{\"type\":\"Polygon\",\"coordinates\":[[[-110.0,33.1],[-110.15,35.97],[87.75,11.45],[-110.0,33.1]],[[-110.0,35.97],[-110.15,36.97],[23.18,45.23],[-110.0,35.97]]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPolygon, true),
                new Action<ISpatial>((spatialValue) =>
                {
                    var geographyPolygon = Assert.IsAssignableFrom<GeographyPolygon>(spatialValue);
                    Assert.Equal(4326, geographyPolygon.CoordinateSystem.EpsgId);

                    var rings = new double[2][,]{ new double[,] { { -110.0, 33.1 }, { -110.15, 35.97 }, { 87.75, 11.45 }, { -110.0, 33.1 } }, new double[,]{ { -110.0, 35.97 }, { -110.15, 36.97 }, { 23.18, 45.23 }, { -110.0, 35.97 } } };
                    Assert.Equal(rings.Length, geographyPolygon.Rings.Count);

                    for (var i = 0; i < rings.GetLength(0); i++)
                    {
                        Assert.Equal(rings[i].GetLength(0), geographyPolygon.Rings[i].Points.Count);

                        for (var j = 0; j < rings[i].GetLength(0); j++)
                        {
                            Assert.Equal(rings[i][j,0], geographyPolygon.Rings[i].Points[j].Longitude);
                            Assert.Equal(rings[i][j,1], geographyPolygon.Rings[i].Points[j].Latitude);
                        }
                    }
                })
            };


            yield return new object[]
            {
                "MultiPointProp",
                "{\"MultiPointProp\":{\"type\":\"MultiPoint\",\"coordinates\":[[11.2,10.2],[11.6,11.9]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyMultiPoint, true),
                new Action<ISpatial>((spatialValue) =>
                {
                    var geographyMultiPoint = Assert.IsAssignableFrom<GeographyMultiPoint>(spatialValue);
                    Assert.Equal(4326, geographyMultiPoint.CoordinateSystem.EpsgId);

                    var points = new double[,]{ { 11.2,10.2 }, { 11.6,11.9 } };
                    Assert.Equal(points.GetLength(0), geographyMultiPoint.Points.Count);

                    for (var i = 0; i < points.GetLength(0); i++)
                    {
                        Assert.Equal(points[i,0], geographyMultiPoint.Points[i].Longitude);
                        Assert.Equal(points[i,1], geographyMultiPoint.Points[i].Latitude);
                    }
                })
            };


            yield return new object[]
            {
                "MultiLineStringProp",
                "{\"MultiLineStringProp\":{\"type\":\"MultiLineString\",\"coordinates\":[[[11.2,10.2],[11.6,11.9]],[[17.2,16.2],[19.6,18.9]]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyMultiLineString, true),
                new Action<ISpatial>((spatialValue) =>
                {
                    var geographyMultiString = Assert.IsAssignableFrom<GeographyMultiLineString>(spatialValue);
                    Assert.Equal(4326, geographyMultiString.CoordinateSystem.EpsgId);

                    var lineStrings = new double[2][,]{ new double[,] { { 11.2, 10.2 }, { 11.6, 11.9 } }, new double[,]{ { 17.2, 16.2 },{ 19.6, 18.9 } } };
                    Assert.Equal(lineStrings.GetLength(0), geographyMultiString.LineStrings.Count);

                    for (var i = 0; i < lineStrings.GetLength(0); i++)
                    {
                        Assert.Equal(lineStrings[i].GetLength(0), geographyMultiString.LineStrings[i].Points.Count);

                        for (var j = 0; j < lineStrings[i].GetLength(0); j++)
                        {
                            Assert.Equal(lineStrings[i][j,0], geographyMultiString.LineStrings[i].Points[j].Longitude);
                            Assert.Equal(lineStrings[i][j,1], geographyMultiString.LineStrings[i].Points[j].Latitude);
                        }
                    }
                })
            };


            yield return new object[]
            {
                "MultiPolygonProp",
                "{\"MultiPolygonProp\":{\"type\":\"MultiPolygon\",\"coordinates\":[[[[11.2,10.2],[11.6,11.9],[87.75,11.45],[11.2,10.2]],[[17.2,16.2],[19.6,18.9],[87.75,11.45],[17.2,16.2]]]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyMultiPolygon, true),
                new Action<ISpatial>((spatialValue) =>
                {
                    var geographyMultiPolygon = Assert.IsAssignableFrom<GeographyMultiPolygon>(spatialValue);
                    Assert.Equal(4326, geographyMultiPolygon.CoordinateSystem.EpsgId);
                    Assert.Single(geographyMultiPolygon.Polygons);

                    var rings = new double[2][,] { new double[,] { { 11.2, 10.2 }, { 11.6, 11.9 }, { 87.75, 11.45 }, { 11.2, 10.2 } }, new double[,]{ { 17.2, 16.2 }, { 19.6, 18.9 }, { 87.75, 11.45 }, { 17.2, 16.2 } } };
                    Assert.Equal(rings.GetLength(0), geographyMultiPolygon.Polygons[0].Rings.Count);

                    for (var i = 0; i < rings.GetLength(0); i++)
                    {
                        Assert.Equal(rings[i].GetLength(0), geographyMultiPolygon.Polygons[0].Rings[i].Points.Count);

                        for (var j = 0; j < rings[i].GetLength(0); j++)
                        {
                            Assert.Equal(rings[i][j,0], geographyMultiPolygon.Polygons[0].Rings[i].Points[j].Longitude);
                            Assert.Equal(rings[i][j,1], geographyMultiPolygon.Polygons[0].Rings[i].Points[j].Latitude);
                        }
                    }
                })
            };


            yield return new object[]
            {
                "GeometryCollectionProp",
                "{\"GeometryCollectionProp\":{\"type\":\"GeometryCollection\",\"geometries\":[{\"type\":\"Point\",\"coordinates\":[-12.0,-19.99]}],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryCollection, true),
                new Action<ISpatial>((spatialValue) =>
                {
                    var geometryCollection = Assert.IsAssignableFrom<GeometryCollection>(spatialValue);
                    Assert.Equal(4326, geometryCollection.CoordinateSystem.EpsgId);
                    Assert.Single(geometryCollection.Geometries);
                    var geometryPoint = Assert.IsAssignableFrom<GeometryPoint>(geometryCollection.Geometries[0]);
                    Assert.Equal(-12.0, geometryPoint.X);
                    Assert.Equal(-19.99, geometryPoint.Y);
                })
            };
        }

        [Theory]
        [MemberData(nameof(GetReadSpatialPropertyData))]
        public async Task ReadSpatialPropertyAsync(string propertyName, string payload, IEdmPrimitiveTypeReference edmPrimitiveTypeReference, Action<ISpatial> assertAction)
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
        public void ReadSpatialProperty(string propertyName, string payload, IEdmPrimitiveTypeReference edmPrimitiveTypeReference, Action<ISpatial> assertAction)
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

//---------------------------------------------------------------------
// <copyright file="SpatialFormatterRoundTripTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.IO;
using System.Xml;
using Microsoft.Data.Spatial;
using Xunit;

namespace Microsoft.Spatial.Tests.ScenarioTests
{
    public class SpatialFormatterRoundTripTests
    {
        private readonly GeoJsonObjectFormatter jsonFormatter;
        private readonly GmlFormatter gmlFormatter;
        private readonly WellKnownTextSqlFormatter wktFormatter;
        private readonly GeoJsonObjectFormatter jsonDictionaryFormatter;
        private static DataServicesSpatialImplementation implementation = new DataServicesSpatialImplementation();
        private static readonly CoordinateSystem nonDefaultGeometry = CoordinateSystem.Geometry(1234);
        private static readonly CoordinateSystem nonDefaultGeography = CoordinateSystem.Geography(1234);

        public SpatialFormatterRoundTripTests()
        {
            this.jsonFormatter = GeoJsonObjectFormatter.Create();
            this.gmlFormatter = GmlFormatter.Create();
            this.wktFormatter = WellKnownTextSqlFormatter.Create();
            this.jsonDictionaryFormatter = GeoJsonObjectFormatter.Create();
        }

        [Fact]
        public void PointRoundTrip_Geography()
        {
            this.RoundTripTest(
                EmptyGeography(SpatialType.Point),
                GeographyFactory.Point(nonDefaultGeography, 10, 20),
                GeographyFactory.Point(10, 10),
                GeographyFactory.Point(10, 10, 10, 10),
                GeographyFactory.Point(10, 10, 10, null),
                GeographyFactory.Point(10, 10, 10, 10));
        }

        [Fact]
        public void LineStringRoundTrip_Geography()
        {
            this.RoundTripTest(
                EmptyGeography(SpatialType.LineString),
                GeographyFactory.LineString(nonDefaultGeography, 10, 10, 10, 10).LineTo(10, 10, 10, 10),
                GeographyFactory.LineString(10, 10, 10, 10).LineTo(10, 10, 10, 10),
                GeographyFactory.LineString(10, 10, 10, null).LineTo(10, 10, 10, null),
                GeographyFactory.LineString(10, 10).LineTo(10, 10));
        }

        [Fact]
        public void PolygonRoundTrip_Geography()
        {
            this.RoundTripTest(
                EmptyGeography(SpatialType.Polygon),
                GeographyFactory.Polygon(nonDefaultGeography).Ring(-10, -10).LineTo(-20, -20, -20, null).LineTo(-30, -30, -30, -30),
                GeographyFactory.Polygon().Ring(10, 10, 10, 10).LineTo(20, 20, 20, 20).LineTo(30, 30, 30, 30)
                    .Ring(-10, -10, 100, 100).LineTo(-20, -20, 200, 200).LineTo(-30, -30, 300, 300),
                GeographyFactory.Polygon().Ring(-10, -10).LineTo(-20, -20, -20, null).LineTo(-30, -30, -30, -30));
        }

        [Fact]
        public void MultiPointRoundTrip_Geography()
        {
            this.RoundTripTest(
                EmptyGeography(SpatialType.MultiPoint),
                GeographyFactory.MultiPoint(nonDefaultGeography).Point(10, 10).Point(20, 20, 20, null).Point(30, 30, 30, 30),
                GeographyFactory.MultiPoint().Point(10, 10).Point(20, 20, 20, null).Point(30, 30, 30, 30));
        }

        [Fact]
        public void MultiLineStringRoundTrip_Geography()
        {
            this.RoundTripTest(
                EmptyGeography(SpatialType.MultiLineString),
                GeographyFactory.MultiLineString(nonDefaultGeography)
                    .LineString(10, 10).LineTo(20, 20, 20, null).LineTo(30, 30, 30, 30)
                    .LineString(10, 10).LineTo(30, 30).LineTo(20, 20),
                GeographyFactory.MultiLineString()
                    .LineString(10, 10).LineTo(20, 20, 20, null).LineTo(30, 30, 30, 30)
                    .LineString(10, 10).LineTo(30, 30).LineTo(20, 20));
        }

        [Fact]
        public void MultiPolygonRoundTrip_Geography()
        {
            this.RoundTripTest(
                EmptyGeography(SpatialType.MultiPolygon),
                GeographyFactory.MultiPolygon(nonDefaultGeography)
                    .Polygon().Ring(10, 10, 10, 10).LineTo(20, 20, 20, 20).LineTo(30, 30, 30, 30)
                    .Ring(-10, -10, 100, 100).LineTo(-20, -20, 200, 200).LineTo(-30, -30, 300, 300)
                    .Polygon().Ring(-10, -10).LineTo(-20, -20, -20, null).LineTo(-30, -30, -30, -30),
                GeographyFactory.MultiPolygon()
                    .Polygon().Ring(10, 10, 10, 10).LineTo(20, 20, 20, 20).LineTo(30, 30, 30, 30)
                    .Ring(-10, -10, 100, 100).LineTo(-20, -20, 200, 200).LineTo(-30, -30, 300, 300)
                    .Polygon().Ring(-10, -10).LineTo(-20, -20, -20, null).LineTo(-30, -30, -30, -30));
        }

        [Fact]
        public void CollectionRoundTrip_Geography()
        {
            this.RoundTripTest(
                EmptyGeography(SpatialType.Collection),
                GeographyFactory.Collection(nonDefaultGeography)
                    .Point(10, 10)
                    .LineString(10, 10).LineTo(30, 30).LineTo(20, 20)
                    .Polygon().Ring(10, 10, 10, 10).LineTo(20, 20, 20, 20).LineTo(30, 30, 30, 30)
                    .Ring(-10, -10, 100, 100).LineTo(-20, -20, 200, 200).LineTo(-30, -30, 300, 300)
                    .Collection()
                    .Polygon().Ring(-10, -10).LineTo(-20, -20, -20, null).LineTo(-30, -30, -30, -30)
                    .LineString(10, 10).LineTo(20, 20, 20, null)
                    .Collection(),
                GeographyFactory.Collection()
                    .Point(10, 10)
                    .LineString(10, 10).LineTo(30, 30).LineTo(20, 20)
                    .Polygon().Ring(10, 10, 10, 10).LineTo(20, 20, 20, 20).LineTo(30, 30, 30, 30)
                    .Ring(-10, -10, 100, 100).LineTo(-20, -20, 200, 200).LineTo(-30, -30, 300, 300)
                    .Collection()
                    .Polygon().Ring(-10, -10).LineTo(-20, -20, -20, null).LineTo(-30, -30, -30, -30)
                    .LineString(10, 10).LineTo(20, 20, 20, null)
                    .Collection());
        }


        #region Geometry - Edit Geometry, Copy over and replace Geography->Geometry

        [Fact]
        public void PointRoundTrip_Geometry()
        {
            this.RoundTripTest(
                EmptyGeometry(SpatialType.Point),
                GeometryFactory.Point(nonDefaultGeometry, 10, 20),
                GeometryFactory.Point(10, 10),
                GeometryFactory.Point(10, 10, 10, 10),
                GeometryFactory.Point(10, 10, 10, null),
                GeometryFactory.Point(10, 10, 10, 10));
        }

        [Fact]
        public void LineStringRoundTrip_Geometry()
        {
            this.RoundTripTest(
                EmptyGeometry(SpatialType.LineString),
                GeometryFactory.LineString(nonDefaultGeometry, 10, 10, 10, 10).LineTo(10, 10, 10, 10),
                GeometryFactory.LineString(10, 10, 10, 10).LineTo(10, 10, 10, 10),
                GeometryFactory.LineString(10, 10, 10, null).LineTo(10, 10, 10, null),
                GeometryFactory.LineString(10, 10).LineTo(10, 10));
        }

        [Fact]
        public void PolygonRoundTrip_Geometry()
        {
            this.RoundTripTest(
                EmptyGeometry(SpatialType.Polygon),
                GeometryFactory.Polygon(nonDefaultGeometry).Ring(-10, -10).LineTo(-20, -20, -20, null).LineTo(-30, -30, -30, -30),
                GeometryFactory.Polygon().Ring(10, 10, 10, 10).LineTo(20, 20, 20, 20).LineTo(30, 30, 30, 30)
                    .Ring(-10, -10, 100, 100).LineTo(-20, -20, 200, 200).LineTo(-30, -30, 300, 300),
                GeometryFactory.Polygon().Ring(-10, -10).LineTo(-20, -20, -20, null).LineTo(-30, -30, -30, -30));
        }

        [Fact]
        public void MultiPointRoundTrip_Geometry()
        {
            this.RoundTripTest(
                EmptyGeometry(SpatialType.MultiPoint),
                GeometryFactory.MultiPoint(nonDefaultGeometry).Point(10, 10).Point(20, 20, 20, null).Point(30, 30, 30, 30),
                GeometryFactory.MultiPoint().Point(10, 10).Point(20, 20, 20, null).Point(30, 30, 30, 30));
        }

        [Fact]
        public void MultiLineStringRoundTrip_Geometry()
        {
            this.RoundTripTest(
                EmptyGeometry(SpatialType.MultiLineString),
                GeometryFactory.MultiLineString(nonDefaultGeometry)
                    .LineString(10, 10).LineTo(20, 20, 20, null).LineTo(30, 30, 30, 30)
                    .LineString(10, 10).LineTo(30, 30).LineTo(20, 20),
                GeometryFactory.MultiLineString()
                    .LineString(10, 10).LineTo(20, 20, 20, null).LineTo(30, 30, 30, 30)
                    .LineString(10, 10).LineTo(30, 30).LineTo(20, 20));
        }

        [Fact]
        public void MultiPolygonRoundTrip_Geometry()
        {
            this.RoundTripTest(
                EmptyGeometry(SpatialType.MultiPolygon),
                GeometryFactory.MultiPolygon(nonDefaultGeometry)
                    .Polygon().Ring(10, 10, 10, 10).LineTo(20, 20, 20, 20).LineTo(30, 30, 30, 30)
                    .Ring(-10, -10, 100, 100).LineTo(-20, -20, 200, 200).LineTo(-30, -30, 300, 300)
                    .Polygon().Ring(-10, -10).LineTo(-20, -20, -20, null).LineTo(-30, -30, -30, -30),
                GeometryFactory.MultiPolygon()
                    .Polygon().Ring(10, 10, 10, 10).LineTo(20, 20, 20, 20).LineTo(30, 30, 30, 30)
                    .Ring(-10, -10, 100, 100).LineTo(-20, -20, 200, 200).LineTo(-30, -30, 300, 300)
                    .Polygon().Ring(-10, -10).LineTo(-20, -20, -20, null).LineTo(-30, -30, -30, -30));
        }

        [Fact]
        public void CollectionRoundTrip_Geometry()
        {
            this.RoundTripTest(
                EmptyGeometry(SpatialType.Collection),
                GeometryFactory.Collection(nonDefaultGeometry)
                    .Point(10, 10)
                    .LineString(10, 10).LineTo(30, 30).LineTo(20, 20)
                    .Polygon().Ring(10, 10, 10, 10).LineTo(20, 20, 20, 20).LineTo(30, 30, 30, 30)
                    .Ring(-10, -10, 100, 100).LineTo(-20, -20, 200, 200).LineTo(-30, -30, 300, 300)
                    .Collection()
                    .Polygon().Ring(-10, -10).LineTo(-20, -20, -20, null).LineTo(-30, -30, -30, -30)
                    .LineString(10, 10).LineTo(20, 20, 20, null)
                    .Collection(),
                GeometryFactory.Collection()
                    .Point(10, 10)
                    .LineString(10, 10).LineTo(30, 30).LineTo(20, 20)
                    .Polygon().Ring(10, 10, 10, 10).LineTo(20, 20, 20, 20).LineTo(30, 30, 30, 30)
                    .Ring(-10, -10, 100, 100).LineTo(-20, -20, 200, 200).LineTo(-30, -30, 300, 300)
                    .Collection()
                    .Polygon().Ring(-10, -10).LineTo(-20, -20, -20, null).LineTo(-30, -30, -30, -30)
                    .LineString(10, 10).LineTo(20, 20, 20, null)
                    .Collection());
        }

        #endregion

        private void RoundTripTest_Gml(params Geography[] args)
        {
            foreach (var input in args)
            {
                var text = this.gmlFormatter.Write(input);
                TextReader reader = new StringReader(text);
                var r = XmlReader.Create(reader);
                var output = this.gmlFormatter.Read<Geography>(r);

                Assert.True(input.Equals(output));
            }
        }

        private void RoundTripTest_Json(params Geography[] args)
        {
            foreach (var input in args)
            {
                var format = this.jsonFormatter.Write(input);
                var output = this.jsonFormatter.Read<Geography>(format);

                Assert.True(input.Equals(output));
            }
        }

        private void RoundTripTest_Wkt(params Geography[] args)
        {
            foreach (var input in args)
            {
                var text = this.wktFormatter.Write(input);
                TextReader reader = new StringReader(text);
                var output = this.wktFormatter.Read<Geography>(reader);

                Assert.True(input.Equals(output));
            }
        }

        private void RoundTripTest_JsonDictionary(params Geography[] args)
        {
            foreach (var input in args)
            {
                var dictionary = this.jsonDictionaryFormatter.Write(input);
                var output = this.jsonDictionaryFormatter.Read<Geography>(dictionary);
                Assert.True(input.Equals(output));
            }
        }

        private void RoundTripTest(params Geography[] args)
        {
            RoundTripTest_Gml(args);
            RoundTripTest_Json(args);
            RoundTripTest_Wkt(args);
            RoundTripTest_JsonDictionary(args);
        }

        private static Geography EmptyGeography(SpatialType type)
        {
            var b = new GeographyBuilderImplementation(implementation);
            b.SetCoordinateSystem(CoordinateSystem.DefaultGeography);
            b.BeginGeography(type);
            b.EndGeography();

            return b.ConstructedGeography;
        }

        private void RoundTripTest_Gml(params Geometry[] args)
        {
            foreach (var input in args)
            {
                var text = this.gmlFormatter.Write(input);
                TextReader reader = new StringReader(text);
                var r = XmlReader.Create(reader);
                var output = this.gmlFormatter.Read<Geometry>(r);

                Assert.Equal(input, output);
            }
        }

        private void RoundTripTest_Json(params Geometry[] args)
        {
            foreach (var input in args)
            {
                var format = this.jsonFormatter.Write(input);
                var output = this.jsonFormatter.Read<Geometry>(format);

                Assert.Equal(input, output);
            }
        }

        private void RoundTripTest_Wkt(params Geometry[] args)
        {
            foreach (var input in args)
            {
                var text = this.wktFormatter.Write(input);
                TextReader reader = new StringReader(text);
                var output = this.wktFormatter.Read<Geometry>(reader);

                Assert.True(input.Equals(output));
            }
        }

        private void RoundTripTest_JsonDictionary(params Geometry[] args)
        {
            foreach (var input in args)
            {
                var dictionary = this.jsonDictionaryFormatter.Write(input);
                var output = this.jsonDictionaryFormatter.Read<Geometry>(dictionary);
                Assert.True(input.Equals(output));
            }
        }

        private void RoundTripTest(params Geometry[] args)
        {
            RoundTripTest_Gml(args);
            RoundTripTest_Json(args);
            RoundTripTest_Wkt(args);
            RoundTripTest_JsonDictionary(args);
        }

        private static Geometry EmptyGeometry(SpatialType type)
        {
            var b = new GeometryBuilderImplementation(implementation);
            b.SetCoordinateSystem(CoordinateSystem.DefaultGeometry);
            b.BeginGeometry(type);
            b.EndGeometry();

            return b.ConstructedGeometry;
        }
    }
}

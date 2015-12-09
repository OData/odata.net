//---------------------------------------------------------------------
// <copyright file="WellKnownTextSqlFormatterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using Microsoft.Data.Spatial;
using Microsoft.Spatial.Tests;
using Xunit;

namespace Microsoft.Spatial.Tests
{
    public class WellKnownTextSqlFormatterTests
    {
        private readonly WellKnownTextSqlFormatter d4Formatter = new WellKnownTextSqlFormatterImplementation(new DataServicesSpatialImplementation());
        private readonly WellKnownTextSqlFormatter d2Formatter = new WellKnownTextSqlFormatterImplementation(new DataServicesSpatialImplementation(), true);

        [Fact]
        public void ReadPoint_Ordering()
        {
            // ensure longitude latitude ordering
            var point = this.d4Formatter.Read<GeographyPoint>(new StringReader("POINT(10 20)"));
            Assert.Equal(20, point.Latitude);
            Assert.Equal(10, point.Longitude);
        }

        [Fact]
        public void ReadSRIDTest()
        {
            this.ReadPointTest("SRID=1234;POINT(10 20)", new PositionData(20, 10), CoordinateSystem.Geography(1234));
        }

        [Fact]
        public void NullPipelineTest()
        {
            var nullPipelineFormatter = WellKnownTextSqlFormatter.Create(true);
            var point = nullPipelineFormatter.Read<GeometryPoint>(new StringReader("POINT(12345 567890)"));
            Assert.Equal(12345, point.X);
            Assert.Equal(567890, point.Y);
        }
        
        private void ReadPointTest(string input, PositionData expected, CoordinateSystem expectedCoordinateSystem = null)
        {
            ReadPointTest(this.d4Formatter, new StringReader(input), expected, expectedCoordinateSystem);
        }

        private static void ReadPointTest(SpatialFormatter<TextReader, TextWriter> formatter, TextReader input, PositionData expected, CoordinateSystem expectedCoordinateSystem = null)
        {
            var p = formatter.Read<Geography>(input);
            Assert.NotNull(p);
            p.VerifyAsPoint(expected);

            if (expectedCoordinateSystem != null)
            {
                Assert.Equal(p.CoordinateSystem, expectedCoordinateSystem);
            }
        }

        private void ReadLineStringTest(string input, CoordinateSystem expectedReference, params PositionData[] expected)
        {
            var p = this.d4Formatter.Read<Geography>(new StringReader(input));
            Assert.NotNull(p);
            p.VerifyAsLineString(expected);

            if (expectedReference != null)
            {
                Assert.Equal(p.CoordinateSystem, expectedReference);
            }
        }
        
        private void ReadPolygonTest(string input, CoordinateSystem expectedReference, params PositionData[][] expected)
        {
            var p = this.d4Formatter.Read<Geography>(new StringReader(input));
            Assert.NotNull(p);
            p.VerifyAsPolygon(expected);

            if (expectedReference != null)
            {
                Assert.Equal(p.CoordinateSystem, expectedReference);
            }
        }

        [Fact]
        public void ReaderIgnoreWhitespace()
        {
            this.ReadPointTest("SRID = 1234; POINT\t( 10 \r\n 20 )", new PositionData(20, 10), CoordinateSystem.Geography(1234));
        }

        [Fact]
        public void ReaderIgnoreCase()
        {
            this.ReadPointTest("sRiD=1234;pOinT( 10 20 )", new PositionData(20, 10), CoordinateSystem.Geography(1234));
        }

        [Fact]
        public void ReadPoint()
        {
            this.ReadPointTest("POINT EMPTY", null);
            this.ReadPointTest("POINT (10 20)", new PositionData(20, 10));
            this.ReadPointTest("POINT (10.1 20)", new PositionData(20, 10.1));
            this.ReadPointTest("POINT (10.1 20.1)", new PositionData(20.1, 10.1));
            this.ReadPointTest("POINT (10 20.1)", new PositionData(20.1, 10));
            this.ReadPointTest("POINT (-10 20)", new PositionData(20, -10));
            this.ReadPointTest("POINT (+10.1 20)", new PositionData(20, 10.1));
            this.ReadPointTest("POINT (10.1 -20.1)", new PositionData(-20.1, 10.1));
            this.ReadPointTest("POINT (10 +20.1)", new PositionData(+20.1, 10));
            this.ReadPointTest("POINT (10 20 30)", new PositionData(20, 10, 30, null));
            this.ReadPointTest("POINT (10 20.1 30)", new PositionData(20.1, 10, 30, null));
            this.ReadPointTest("POINT (10 20 30.1)", new PositionData(20, 10, 30.1, null));
            this.ReadPointTest("POINT (10 20 -30.0)", new PositionData(20, 10, -30, null));
            this.ReadPointTest("POINT (10 20.1 +30.1)", new PositionData(20.1, 10, 30.1, null));
            this.ReadPointTest("POINT (10 20 NULL)", new PositionData(20, 10, null, null));
            this.ReadPointTest("POINT (10 20 30 40)", new PositionData(20, 10, 30, 40));
            this.ReadPointTest("POINT (10 20 30.1 40)", new PositionData(20, 10, 30.1, 40));
            this.ReadPointTest("POINT (10 20 30 40.1)", new PositionData(20, 10, 30, 40.1));
            this.ReadPointTest("POINT (10 20 30 -40)", new PositionData(20, 10, 30, -40));
            this.ReadPointTest("POINT (10 20 30.1 +40.5)", new PositionData(20, 10, 30.1, 40.5));
            this.ReadPointTest("POINT (10 20 NULL 40)", new PositionData(20, 10, null, 40));
            this.ReadPointTest("POINT (10 20 30.1 NULL)", new PositionData(20, 10, 30.1, null));
            this.ReadPointTest("POINT (10 20 NULL NULL)", new PositionData(20, 10, null, null));
        }

        [Fact]
        public void ReadLineString()
        {
            this.ReadLineStringTest("LINESTRING EMPTY", null, null);
            this.ReadLineStringTest("LINESTRING (10 20, 10.1 20.1)", null, new PositionData(20, 10, null, null), new PositionData(20.1, 10.1, null, null));
            this.ReadLineStringTest("LINESTRING (10 20 30 40, 40 30 20 10)", null, new PositionData(20, 10, 30, 40), new PositionData(30, 40, 20, 10));
            this.ReadLineStringTest("LINESTRING (10 20 30 40, 40 30 NULL NULL, 30 20 NULL 10)", null, new PositionData(20, 10, 30, 40), new PositionData(30, 40, null, null), new PositionData(20, 30, null, 10));
        }

        [Fact]
        public void ReadPolygon()
        {
            this.ReadPolygonTest("POLYGON EMPTY", null, null);
            this.ReadPolygonTest("POLYGON ((10 20, 15 25, 20 30, 10 20), (15 25, 20 30, 25 35, 15 25), EMPTY, (5 5, 6 6, 7 7, 5 5))",
                null,
                new[] { 
                    new PositionData(20, 10), new PositionData(25, 15), new PositionData(30, 20), new PositionData(20, 10) },
                new[] { 
                    new PositionData(25, 15), new PositionData(30, 20), new PositionData(35, 25), new PositionData(25, 15) }, 
                new[] { 
                    new PositionData(5, 5), new PositionData(6, 6), new PositionData(7, 7), new PositionData(5, 5) });
        }
        
        [Fact]
        public void ReadMultiPoint_Empty()
        {
            var p = this.d4Formatter.Read<Geography>(new StringReader("MULTIPOINT EMPTY"));
            Assert.NotNull(p);
            p.VerifyAsMultiPoint(null);
        }

        [Fact]
        public void ReadMultiPoint()
        {
            var p = this.d2Formatter.Read<Geography>(new StringReader("MULTIPOINT ((10 20), EMPTY, (30 40))"));
            Assert.NotNull(p);
            p.VerifyAsMultiPoint(new PositionData(20, 10), null, new PositionData(40, 30));
        }

        [Fact]
        public void ReadMultiLineString()
        {
            var p = this.d2Formatter.Read<Geography>(new StringReader("MULTILINESTRING ((10 10, 20 20), EMPTY, (30 30, 40 40, 50 50))"));
            Assert.NotNull(p);
            p.VerifyAsMultiLineString(
                new[] { 
                    new PositionData(10, 10),
                    new PositionData(20, 20),
                },
                null,
                new[] {
                    new PositionData(30, 30),
                    new PositionData(40, 40),
                    new PositionData(50, 50),
                });
        }

        [Fact]
        public void ReadMultiPolygon()
        {
            var p = this.d2Formatter.Read<Geography>(new StringReader("MULTIPOLYGON (((10 10, 20 20, 30 30, 10 10), (30 30, 40 40, 50 50, 30 30)), EMPTY, ((10 10, 20 20, 30 30, 10 10)))"));
            Assert.NotNull(p);
            p.VerifyAsMultiPolygon(
                new[]
                    {  // poly 1
                    new[] {  // ring 1
                        new PositionData(10,10),
                        new PositionData(20,20),
                        new PositionData(30,30),
                        new PositionData(10,10),
                    },
                    new[] { // ring 2
                        new PositionData(30,30),
                        new PositionData(40,40),
                        new PositionData(50,50),
                        new PositionData(30,30),
                    },
                },
                null, // poly 2
                new[]
                    {  // poly 3
                    new[] { // ring 1
                        new PositionData(10,10),
                        new PositionData(20,20),
                        new PositionData(30,30),
                        new PositionData(10,10),
                    }
                });
        }

        [Fact]
        public void ReadCollection_Empty()
        {
            var p = this.d4Formatter.Read<Geography>(new StringReader("GEOMETRYCOLLECTION EMPTY"));
            Assert.NotNull(p);
            Assert.True(p is GeographyCollection);
            Assert.True(p.IsEmpty);
        }

        [Fact]
        public void ReadCollection()
        {
            var p = this.d2Formatter.Read<Geography>(new StringReader("GEOMETRYCOLLECTION (POINT(10 10), LINESTRING(20 20, 20 20), POLYGON EMPTY, GEOMETRYCOLLECTION(POINT(30 30)))"));
            Assert.NotNull(p);
            p.VerifyAsCollection(
                (g) => g.VerifyAsPoint(new PositionData(10, 10)),
                (g) => g.VerifyAsLineString(new PositionData(20, 20), new PositionData(20, 20)),
                (g) => g.VerifyAsPolygon(null),
                (g) => g.VerifyAsCollection(
                    (g1) => g1.VerifyAsPoint(new PositionData(30, 30))));
        }
        
        [Fact]
        public void ReadGeographyPipeline()
        {
            var textReader = new StringReader("POINT(10 20 NULL 40)POINT(10 30 NULL 40)SRID=1234;POINT(10 30 NULL 40)");
            var serializer = new SpatialToPositionPipeline();
            Assert.Equal(0, serializer.Coordinates.Count);
            this.d4Formatter.Read<Geography>(textReader, serializer);
            Assert.Equal(1, serializer.Coordinates.Count);
            this.d4Formatter.Read<Geography>(textReader, serializer);
            Assert.Equal(2, serializer.Coordinates.Count);
            this.d4Formatter.Read<Geography>(textReader, serializer);
            Assert.Equal(3, serializer.Coordinates.Count);
            Assert.Equal(new PositionData(20, 10, null, 40), serializer.Coordinates[0]);
            Assert.Equal(new PositionData(30, 10, null, 40), serializer.Coordinates[1]);
            Assert.Equal(new PositionData(30, 10, null, 40), serializer.Coordinates[2]);
            Assert.Equal(CoordinateSystem.Geography(1234), serializer.CoordinateSystem);
        }

        [Fact]
        public void ErrorOnRead3DPoint()
        {
            TestErrorOn3DValueIn2DOnlyMode("SRID=1234;POINT(10 20 30)");
            TestErrorOn3DValueIn2DOnlyMode("POINT(10 20 30)");
            
            TestErrorOn3DValueIn2DOnlyMode("LINESTRING (10 20 30, 10.1 20.1)");
            TestErrorOn3DValueIn2DOnlyMode("LINESTRING (10 20, 10.1 20.1 30.1)");
            
            TestErrorOn3DValueIn2DOnlyMode("POLYGON ((10 20 30, 15 25, 20 30, 10 20), (15 25, 20 30, 25 35, 15 25), EMPTY, (5 5, 6 6, 7 7, 5 5))");
            TestErrorOn3DValueIn2DOnlyMode("POLYGON ((10 20 30, 15 25, 20 30, 10 20), (15 25, 20 30 40, 25 35, 15 25), EMPTY, (5 5, 6 6, 7 7, 5 5))");
            TestErrorOn3DValueIn2DOnlyMode("POLYGON ((10 20, 15 25, 20 30, 10 20), (15 25, 20 30, 25 35, 15 25), EMPTY, (5 5, 6 6, 7 7 7, 5 5))");
            
            TestErrorOn3DValueIn2DOnlyMode("MULTIPOINT ((10 20 30), EMPTY, (30 40))");
            TestErrorOn3DValueIn2DOnlyMode("MULTIPOINT ((10 20), EMPTY, (30 40 50))");
            
            TestErrorOn3DValueIn2DOnlyMode("MULTILINESTRING ((10 10 10, 20 20), EMPTY, (30 30, 40 40, 50 50))");
            TestErrorOn3DValueIn2DOnlyMode("MULTILINESTRING ((10 10, 20 20 20), EMPTY, (30 30, 40 40, 50 50))");
            TestErrorOn3DValueIn2DOnlyMode("MULTILINESTRING ((10 10, 20 20), EMPTY, (30 30 30, 40 40, 50 50))");
            TestErrorOn3DValueIn2DOnlyMode("MULTILINESTRING ((10 10, 20 20), EMPTY, (30 30, 40 40 40, 50 50))");
            TestErrorOn3DValueIn2DOnlyMode("MULTILINESTRING ((10 10, 20 20), EMPTY, (30 30, 40 40, 50 50 50))");

            TestErrorOn3DValueIn2DOnlyMode("MULTIPOLYGON (((10 10 10, 20 20, 30 30, 10 10), (30 30, 40 40, 50 50, 30 30)), EMPTY, ((10 10, 20 20, 30 30, 10 10)))");
            TestErrorOn3DValueIn2DOnlyMode("MULTIPOLYGON (((10 10, 20 20 20, 30 30, 10 10), (30 30, 40 40, 50 50, 30 30)), EMPTY, ((10 10, 20 20, 30 30, 10 10)))");
            TestErrorOn3DValueIn2DOnlyMode("MULTIPOLYGON (((10 10, 20 20, 30 30 30, 10 10), (30 30, 40 40, 50 50, 30 30)), EMPTY, ((10 10, 20 20, 30 30, 10 10)))");
            TestErrorOn3DValueIn2DOnlyMode("MULTIPOLYGON (((10 10, 20 20, 30 30, 10 10 10), (30 30, 40 40, 50 50, 30 30)), EMPTY, ((10 10, 20 20, 30 30, 10 10)))");
            TestErrorOn3DValueIn2DOnlyMode("MULTIPOLYGON (((10 10, 20 20, 30 30, 10 10), (30 30 30, 40 40, 50 50, 30 30)), EMPTY, ((10 10, 20 20, 30 30, 10 10)))");
            TestErrorOn3DValueIn2DOnlyMode("MULTIPOLYGON (((10 10, 20 20, 30 30, 10 10), (30 30, 40 40 40, 50 50, 30 30)), EMPTY, ((10 10, 20 20, 30 30, 10 10)))");
            TestErrorOn3DValueIn2DOnlyMode("MULTIPOLYGON (((10 10, 20 20, 30 30, 10 10), (30 30, 40 40, 50 50 50, 30 30)), EMPTY, ((10 10, 20 20, 30 30, 10 10)))");
            TestErrorOn3DValueIn2DOnlyMode("MULTIPOLYGON (((10 10, 20 20, 30 30, 10 10), (30 30, 40 40, 50 50, 30 30 30)), EMPTY, ((10 10, 20 20, 30 30, 10 10)))");
            TestErrorOn3DValueIn2DOnlyMode("MULTIPOLYGON (((10 10, 20 20, 30 30, 10 10), (30 30, 40 40, 50 50, 30 30)), EMPTY, ((10 10 10, 20 20, 30 30, 10 10)))");
            TestErrorOn3DValueIn2DOnlyMode("MULTIPOLYGON (((10 10, 20 20, 30 30, 10 10), (30 30, 40 40, 50 50, 30 30)), EMPTY, ((10 10, 20 20 20, 30 30, 10 10)))");
            TestErrorOn3DValueIn2DOnlyMode("MULTIPOLYGON (((10 10, 20 20, 30 30, 10 10), (30 30, 40 40, 50 50, 30 30)), EMPTY, ((10 10, 20 20, 30 30 30, 10 10)))");
            TestErrorOn3DValueIn2DOnlyMode("MULTIPOLYGON (((10 10, 20 20, 30 30, 10 10), (30 30, 40 40, 50 50, 30 30)), EMPTY, ((10 10, 20 20, 30 30, 10 10 10)))");

            TestErrorOn3DValueIn2DOnlyMode("GEOMETRYCOLLECTION (POINT(10 10 10), LINESTRING(20 20, 20 20), POLYGON EMPTY, GEOMETRYCOLLECTION(POINT(30 30)))");
            TestErrorOn3DValueIn2DOnlyMode("GEOMETRYCOLLECTION (POINT(10 10), LINESTRING(20 20 20, 20 20), POLYGON EMPTY, GEOMETRYCOLLECTION(POINT(30 30)))");
            TestErrorOn3DValueIn2DOnlyMode("GEOMETRYCOLLECTION (POINT(10 10), LINESTRING(20 20, 20 20 20), POLYGON EMPTY, GEOMETRYCOLLECTION(POINT(30 30)))");
            TestErrorOn3DValueIn2DOnlyMode("GEOMETRYCOLLECTION (POINT(10 10), LINESTRING(20 20, 20 20), POLYGON EMPTY, GEOMETRYCOLLECTION(POINT(30 30 30)))");

        }

        private void TestErrorOn3DValueIn2DOnlyMode(string wktValue)
        {
            Action<string> readGeography =
                (s) =>
                this.d2Formatter.Read<Geography>(new StringReader(s), new SpatialToPositionPipeline());

            // validate that it is valid in 4d mode
            this.d4Formatter.Read<Geography>(new StringReader(wktValue), new SpatialToPositionPipeline());

            Exception ex = SpatialTestUtils.RunCatching<ParseErrorException>(() => readGeography(wktValue));
            Assert.NotNull(ex);
            Assert.Equal(Strings.WellKnownText_TooManyDimensions, ex.Message);
        }

        [Fact]
        public void ReadUnknownTagTest()
        {
            Action<string> readGeography =
                (s) =>
                this.d4Formatter.Read<Geography>(new StringReader(s), new SpatialToPositionPipeline());

            Exception ex = SpatialTestUtils.RunCatching<ParseErrorException>(() => readGeography("SRID=1234;FOO(10 20)"));
            Assert.NotNull(ex);
            Assert.Equal(Strings.WellKnownText_UnknownTaggedText("FOO"), ex.Message);

            ex = SpatialTestUtils.RunCatching<ParseErrorException>(() => readGeography("FOO"));
            Assert.NotNull(ex);
            Assert.Equal(Strings.WellKnownText_UnknownTaggedText("FOO"), ex.Message);
        }

        [Fact]
        public void ReadUnexpectedCharacter()
        {
            Action<string> readGeography =
                (s) =>
                this.d4Formatter.Read<Geography>(new StringReader(s), new SpatialToPositionPipeline());

            Exception ex = SpatialTestUtils.RunCatching<ParseErrorException>(() => readGeography("POINT:10 20"));
            Assert.NotNull(ex);
            Assert.Equal(Strings.WellKnownText_UnexpectedCharacter(":"), ex.Message);
        }

        [Fact]
        public void ReadEmptyString()
        {
            Action<string> readGeography =
                (s) =>
                this.d4Formatter.Read<Geography>(new StringReader(s), new SpatialToPositionPipeline());

            Exception ex = SpatialTestUtils.RunCatching<ParseErrorException>(() => readGeography(""));
            Assert.NotNull(ex);
            Assert.Equal(Strings.WellKnownText_UnknownTaggedText(""), ex.Message);
        }

        [Fact]
        public void ReadUnexpectedToken()
        {
            Action<string> readGeography =
                (s) =>
                this.d4Formatter.Read<Geography>(new StringReader(s), new SpatialToPositionPipeline());

            Exception ex = SpatialTestUtils.RunCatching<ParseErrorException>(() => readGeography("POINT(10,20)"));
            Assert.NotNull(ex);
            Assert.Equal(Strings.WellKnownText_UnexpectedToken("Number", "", "Type:[7] Text:[,]"), ex.Message);
        }

        [Fact]
        public void OneParserMultipleStreamsOfGeographies()
        {
            ReadPointTest(this.d4Formatter, new StringReader("POINT EMPTY"), null);
            ReadPointTest(this.d4Formatter, new StringReader("POINT (10 20)"), new PositionData(20, 10));
            ReadPointTest(this.d4Formatter, new StringReader("POINT (10.1 20)"), new PositionData(20, 10.1));
            ReadPointTest(this.d4Formatter, new StringReader("POINT EMPTY"), null);
            ReadPointTest(this.d4Formatter, new StringReader("POINT (10.1 20.1)"), new PositionData(20.1, 10.1));
            ReadPointTest(this.d4Formatter, new StringReader("POINT (10 20.1)"), new PositionData(20.1, 10));
        }

        [Fact]
        public void OneParseOneMultiShapeStreamOfGeographies()
        {
            var reader = new StringReader(
                "POINT EMPTY" + " " +
                "SRID=1234;POINT (10 20)" + " " +
                "POINT (10.1 20)" + " " +
                "SRID=4321;POINT EMPTY" + " " +
                "POINT (10.1 20.1)" + " " +
                "POINT (10 20.1)" + " "
                );

            ReadPointTest(this.d4Formatter, reader, null);
            ReadPointTest(this.d4Formatter, reader, new PositionData(20, 10));
            ReadPointTest(this.d4Formatter, reader, new PositionData(20, 10.1));
            ReadPointTest(this.d4Formatter, reader, null);
            ReadPointTest(this.d4Formatter, reader, new PositionData(20.1, 10.1));
            ReadPointTest(this.d4Formatter, reader, new PositionData(20.1, 10));
        }
    }

    public class WellKnownTextWriterTests
    {
        private readonly WellKnownTextSqlFormatter d2Formatter = new WellKnownTextSqlFormatterImplementation(new DataServicesSpatialImplementation(), true);
        private readonly WellKnownTextSqlFormatter d4Formatter = new WellKnownTextSqlFormatterImplementation(new DataServicesSpatialImplementation());

        [Fact]
        public void WritePoint()
        {
            Action<GeographyPipeline> emptyCalls = (w) =>
                            {
                                w.BeginGeography(SpatialType.Point);
                                w.EndGeography();
                            };
            GeographyToWktTest(this.d2Formatter, emptyCalls, "SRID=4326;POINT EMPTY");
            GeographyToWktTest(this.d4Formatter, emptyCalls, "SRID=4326;POINT EMPTY");

            Action<GeographyPipeline> d4PointCalls = (w) =>
                                                         {
                                                             w.BeginGeography(SpatialType.Point);
                                                             w.BeginFigure(new GeographyPosition(10, 20, 30, 40));
                                                             w.EndFigure();
                                                             w.EndGeography();
                                                         };
            GeographyToWktTest(this.d2Formatter, d4PointCalls, "SRID=4326;POINT (20 10)");
            GeographyToWktTest(this.d4Formatter, d4PointCalls, "SRID=4326;POINT (20 10 30 40)");

            Action<GeographyPipeline> d3PointCalls = (w) =>
                                                      {
                                                          w.BeginGeography(SpatialType.Point);
                                                          w.BeginFigure(new GeographyPosition(10, 20, 30, null));
                                                          w.EndFigure();
                                                          w.EndGeography();
                                                      };
            GeographyToWktTest(this.d2Formatter, d3PointCalls, "SRID=4326;POINT (20 10)");
            GeographyToWktTest(this.d4Formatter, d3PointCalls, "SRID=4326;POINT (20 10 30)");

            Action<GeographyPipeline> d2PointCalls = (w) =>
                                                      {
                                                          w.BeginGeography(SpatialType.Point);
                                                          w.BeginFigure(new GeographyPosition(10, 20, null, null));
                                                          w.EndFigure();
                                                          w.EndGeography();
                                                      };
            GeographyToWktTest(this.d2Formatter, d2PointCalls, "SRID=4326;POINT (20 10)");
            GeographyToWktTest(this.d4Formatter, d2PointCalls, "SRID=4326;POINT (20 10)");

            Action<GeographyPipeline> skipPointCalls =
            (w) =>
                {
                    w.BeginGeography(SpatialType.Point);
                    w.BeginFigure(new GeographyPosition(10, 20, null, 40));
                    w.EndFigure();
                    w.EndGeography();
                };
            GeographyToWktTest(this.d2Formatter, skipPointCalls, "SRID=4326;POINT (20 10)");
            GeographyToWktTest(this.d4Formatter, skipPointCalls, "SRID=4326;POINT (20 10 NULL 40)");
        }

        [Fact]
        public void WriteLineString()
        {

            Action<GeographyPipeline> emptyCalls = (w) =>
                                                    {
                                                        w.BeginGeography(SpatialType.LineString);
                                                        w.EndGeography();
                                                    };

            GeographyToWktTest(this.d2Formatter, emptyCalls, "SRID=4326;LINESTRING EMPTY");
            GeographyToWktTest(this.d4Formatter, emptyCalls, "SRID=4326;LINESTRING EMPTY");

            Action<GeographyPipeline> twoD2Point = (w) =>
                                                    {
                                                        w.BeginGeography(SpatialType.LineString);
                                                        w.BeginFigure(new GeographyPosition(10, 20, null, null));
                                                        w.LineTo(new GeographyPosition(20, 30, null, null));
                                                        w.EndFigure();
                                                        w.EndGeography();
                                                    };

            GeographyToWktTest(this.d2Formatter, twoD2Point, "SRID=4326;LINESTRING (20 10, 30 20)");
            GeographyToWktTest(this.d4Formatter, twoD2Point, "SRID=4326;LINESTRING (20 10, 30 20)");

            Action<GeographyPipeline> threeD2Point = (w) =>
                                                      {
                                                          w.BeginGeography(SpatialType.LineString);
                                                          w.BeginFigure(new GeographyPosition(10, 20, null, null));
                                                          w.LineTo(new GeographyPosition(-20.5, -30, null, null));
                                                          w.LineTo(new GeographyPosition(30, 40, null, null));
                                                          w.EndFigure();
                                                          w.EndGeography();
                                                      };
            GeographyToWktTest(this.d2Formatter, threeD2Point, "SRID=4326;LINESTRING (20 10, -30 -20.5, 40 30)");
            GeographyToWktTest(this.d4Formatter, threeD2Point, "SRID=4326;LINESTRING (20 10, -30 -20.5, 40 30)");


            Action<GeographyPipeline> twoD4Point = (w) =>
                                                    {
                                                        w.BeginGeography(SpatialType.LineString);
                                                        w.BeginFigure(new GeographyPosition(10, 20, 30, 40));
                                                        w.LineTo(new GeographyPosition(20, 30, 40, 50));
                                                        w.EndFigure();
                                                        w.EndGeography();
                                                    };
            
            GeographyToWktTest(this.d2Formatter, twoD4Point, "SRID=4326;LINESTRING (20 10, 30 20)");
            GeographyToWktTest(this.d4Formatter, twoD4Point, "SRID=4326;LINESTRING (20 10 30 40, 30 20 40 50)");
        }

        [Fact]
        public void WritePolygon()
        {
            Action<GeographyPipeline> emptyCalls = (w) =>
                                                    {
                                                        w.BeginGeography(SpatialType.Polygon);
                                                        w.EndGeography();
                                                    };
            GeographyToWktTest(this.d2Formatter, emptyCalls, "SRID=4326;POLYGON EMPTY");
            GeographyToWktTest(this.d4Formatter, emptyCalls, "SRID=4326;POLYGON EMPTY");


            Action<GeographyPipeline> fourD2Point = (w) =>
                                                    {
                                                        w.BeginGeography(SpatialType.Polygon);
                                                        w.BeginFigure(new GeographyPosition(10, 20, null, null));
                                                        w.LineTo(new GeographyPosition(20, 30, null, null));
                                                        w.LineTo(new GeographyPosition(30, 40, null, null));
                                                        w.LineTo(new GeographyPosition(10, 20, null, null));
                                                        w.EndFigure();
                                                        w.EndGeography();
                                                    };
            GeographyToWktTest(this.d2Formatter, fourD2Point, "SRID=4326;POLYGON ((20 10, 30 20, 40 30, 20 10))");
            GeographyToWktTest(this.d4Formatter, fourD2Point, "SRID=4326;POLYGON ((20 10, 30 20, 40 30, 20 10))");

            Action<GeographyPipeline> fourD2PointWith2D2Holes = (w) =>
                                                     {
                                                         w.BeginGeography(SpatialType.Polygon);
                                                         w.BeginFigure(new GeographyPosition(10, 20, null, null));
                                                         w.LineTo(new GeographyPosition(20, 30, null, null));
                                                         w.LineTo(new GeographyPosition(30, 40, null, null));
                                                         w.LineTo(new GeographyPosition(10, 20, null, null));
                                                         w.EndFigure();

                                                         w.BeginFigure(new GeographyPosition(-10, -20, null, null));
                                                         w.LineTo(new GeographyPosition(-20, -30, null, null));
                                                         w.LineTo(new GeographyPosition(-30, -40, null, null));
                                                         w.LineTo(new GeographyPosition(-10, -20, null, null));
                                                         w.EndFigure();

                                                         w.BeginFigure(new GeographyPosition(-10.5, -20.5, null, null));
                                                         w.LineTo(new GeographyPosition(-20.5, -30.5, null, null));
                                                         w.LineTo(new GeographyPosition(-30.5, -40.5, null, null));
                                                         w.LineTo(new GeographyPosition(-10.5, -20.5, null, null));
                                                         w.EndFigure();
                                                         w.EndGeography();
                                                     };

            GeographyToWktTest(this.d2Formatter, fourD2PointWith2D2Holes, "SRID=4326;POLYGON ((20 10, 30 20, 40 30, 20 10), (-20 -10, -30 -20, -40 -30, -20 -10), (-20.5 -10.5, -30.5 -20.5, -40.5 -30.5, -20.5 -10.5))");
            GeographyToWktTest(this.d4Formatter, fourD2PointWith2D2Holes, "SRID=4326;POLYGON ((20 10, 30 20, 40 30, 20 10), (-20 -10, -30 -20, -40 -30, -20 -10), (-20.5 -10.5, -30.5 -20.5, -40.5 -30.5, -20.5 -10.5))");
        }
        
        [Fact]
        public void WriteMultiPoint()
        {
            Action<GeographyPipeline> twoEmptyPointsCalls = (w) =>
                                           {
                                               w.BeginGeography(SpatialType.MultiPoint);
                                               w.BeginGeography(SpatialType.Point);
                                               w.EndGeography();
                                               w.BeginGeography(SpatialType.Point);
                                               w.EndGeography();
                                               w.EndGeography();
                                           };
            GeographyToWktTest(this.d2Formatter, twoEmptyPointsCalls, "SRID=4326;MULTIPOINT (EMPTY, EMPTY)");
            GeographyToWktTest(this.d4Formatter, twoEmptyPointsCalls, "SRID=4326;MULTIPOINT (EMPTY, EMPTY)");

            Action<GeographyPipeline> noPointsCalls = (w) =>
                                                       {
                                                           w.BeginGeography(SpatialType.MultiPoint);
                                                           w.EndGeography();
                                                       };
            GeographyToWktTest(this.d2Formatter, noPointsCalls, "SRID=4326;MULTIPOINT EMPTY");
            GeographyToWktTest(this.d4Formatter, noPointsCalls, "SRID=4326;MULTIPOINT EMPTY");

            Action<GeographyPipeline> twoD2PointsCalls = (w) =>
                                                       {
                                                           w.BeginGeography(SpatialType.MultiPoint);
                                                           w.BeginGeography(SpatialType.Point);
                                                           w.BeginFigure(new GeographyPosition(10, 20, null, null));
                                                           w.EndFigure();
                                                           w.EndGeography();

                                                           w.BeginGeography(SpatialType.Point);
                                                           w.EndGeography();

                                                           w.BeginGeography(SpatialType.Point);
                                                           w.BeginFigure(new GeographyPosition(30, 40, null, null));
                                                           w.EndFigure();
                                                           w.EndGeography();
                                                           w.EndGeography();
                                                       };
            GeographyToWktTest(this.d2Formatter, twoD2PointsCalls, "SRID=4326;MULTIPOINT ((20 10), EMPTY, (40 30))");
            GeographyToWktTest(this.d4Formatter, twoD2PointsCalls, "SRID=4326;MULTIPOINT ((20 10), EMPTY, (40 30))");

            Action<GeographyPipeline> singleD3PointCalls = (w) =>
                                                          {
                                                              w.BeginGeography(SpatialType.MultiPoint);
                                                              w.BeginGeography(SpatialType.Point);
                                                              w.BeginFigure(new GeographyPosition(10, 20, 30, 40));
                                                              w.EndFigure();
                                                              w.EndGeography();
                                                              w.EndGeography();
                                                          };
            GeographyToWktTest(this.d2Formatter, singleD3PointCalls, "SRID=4326;MULTIPOINT ((20 10))");
            GeographyToWktTest(this.d4Formatter, singleD3PointCalls, "SRID=4326;MULTIPOINT ((20 10 30 40))");
        }

        [Fact]
        public void WriteMultiLineString()
        {
            Action<GeographyPipeline> emptyCalls = (w) =>
                                                    {
                                                        w.BeginGeography(SpatialType.MultiLineString);
                                                        w.BeginGeography(SpatialType.LineString);
                                                        w.EndGeography();
                                                        w.EndGeography();
                                                    };
            GeographyToWktTest(this.d2Formatter, emptyCalls, "SRID=4326;MULTILINESTRING (EMPTY)");
            GeographyToWktTest(this.d4Formatter, emptyCalls, "SRID=4326;MULTILINESTRING (EMPTY)");

            Action<GeographyPipeline> emptyCalls2 = (w) =>
                                                    {
                                                        w.BeginGeography(SpatialType.MultiLineString);
                                                        w.EndGeography();
                                                    };
            GeographyToWktTest(this.d2Formatter, emptyCalls2, "SRID=4326;MULTILINESTRING EMPTY");
            GeographyToWktTest(this.d4Formatter, emptyCalls2, "SRID=4326;MULTILINESTRING EMPTY");

            Action<GeographyPipeline> twoD2LineStringCalls = (w) =>
                                                    {
                                                        w.BeginGeography(SpatialType.MultiLineString);
                                                        w.BeginGeography(SpatialType.LineString);
                                                        w.BeginFigure(new GeographyPosition(10, 20, null, null));
                                                        w.LineTo(new GeographyPosition(20, 30, null, null));
                                                        w.EndFigure();
                                                        w.EndGeography();

                                                        w.BeginGeography(SpatialType.LineString);
                                                        w.EndGeography();

                                                        w.BeginGeography(SpatialType.LineString);
                                                        w.BeginFigure(new GeographyPosition(30, 40, null, null));
                                                        w.LineTo(new GeographyPosition(40, 50, null, null));
                                                        w.EndFigure();
                                                        w.EndGeography();
                                                        w.EndGeography();
                                                    };
            GeographyToWktTest(this.d2Formatter, twoD2LineStringCalls, "SRID=4326;MULTILINESTRING ((20 10, 30 20), EMPTY, (40 30, 50 40))");
            GeographyToWktTest(this.d4Formatter, twoD2LineStringCalls, "SRID=4326;MULTILINESTRING ((20 10, 30 20), EMPTY, (40 30, 50 40))");

            Action<GeographyPipeline> singleD3LineStringCalls = (w) =>
                                                    {
                                                        w.BeginGeography(SpatialType.MultiLineString);
                                                        w.BeginGeography(SpatialType.LineString);
                                                        w.BeginFigure(new GeographyPosition(10, 20, 40, null));
                                                        w.LineTo(new GeographyPosition(20, 30, 50, null));
                                                        w.EndFigure();
                                                        w.EndGeography();
                                                        w.EndGeography();
                                                    };
            GeographyToWktTest(this.d2Formatter, singleD3LineStringCalls, "SRID=4326;MULTILINESTRING ((20 10, 30 20))");
            GeographyToWktTest(this.d4Formatter, singleD3LineStringCalls, "SRID=4326;MULTILINESTRING ((20 10 40, 30 20 50))");
        }

        [Fact]
        public void WriteMultiPolygon()
        {
            Action<GeographyPipeline> emptyCalls = (w) =>
                                                    {
                                                        w.BeginGeography(SpatialType.MultiPolygon);
                                                        w.BeginGeography(SpatialType.Polygon);
                                                        w.EndGeography();
                                                        w.EndGeography();
                                                    };
            GeographyToWktTest(this.d2Formatter, emptyCalls, "SRID=4326;MULTIPOLYGON (EMPTY)");
            GeographyToWktTest(this.d4Formatter, emptyCalls, "SRID=4326;MULTIPOLYGON (EMPTY)");

            Action<GeographyPipeline> emptyCalls2 = (w) =>
                                                     {
                                                         w.BeginGeography(SpatialType.MultiPolygon);
                                                         w.EndGeography();
                                                     };
            GeographyToWktTest(this.d2Formatter, emptyCalls2, "SRID=4326;MULTIPOLYGON EMPTY");
            GeographyToWktTest(this.d4Formatter, emptyCalls2, "SRID=4326;MULTIPOLYGON EMPTY");

            Action<GeographyPipeline> threeLineD2Calls = (w) =>
                                                    {
                                                        w.BeginGeography(SpatialType.MultiPolygon);
                                                        w.BeginGeography(SpatialType.Polygon);
                                                        w.BeginFigure(new GeographyPosition(10, 20, null, null));
                                                        w.LineTo(new GeographyPosition(20, 30, null, null));
                                                        w.LineTo(new GeographyPosition(30, 40, null, null));
                                                        w.LineTo(new GeographyPosition(10, 20, null, null));
                                                        w.EndFigure();

                                                        w.BeginFigure(new GeographyPosition(-10.5, -20.5, null, null));
                                                        w.LineTo(new GeographyPosition(-20.5, -30.5, null, null));
                                                        w.LineTo(new GeographyPosition(-30.5, -40.5, null, null));
                                                        w.LineTo(new GeographyPosition(-10.5, -20.5, null, null));
                                                        w.EndFigure();
                                                        w.EndGeography();

                                                        w.BeginGeography(SpatialType.Polygon);
                                                        w.EndGeography();

                                                        w.BeginGeography(SpatialType.Polygon);

                                                        w.BeginFigure(new GeographyPosition(10, 20, null, null));
                                                        w.LineTo(new GeographyPosition(20, 30, null, null));
                                                        w.LineTo(new GeographyPosition(30, 40, null, null));
                                                        w.LineTo(new GeographyPosition(10, 20, null, null));
                                                        w.EndFigure();

                                                        w.EndGeography();
                                                        w.EndGeography();
                                                    };
            GeographyToWktTest(this.d2Formatter, threeLineD2Calls, "SRID=4326;MULTIPOLYGON (((20 10, 30 20, 40 30, 20 10), (-20.5 -10.5, -30.5 -20.5, -40.5 -30.5, -20.5 -10.5)), EMPTY, ((20 10, 30 20, 40 30, 20 10)))");
            GeographyToWktTest(this.d4Formatter, threeLineD2Calls, "SRID=4326;MULTIPOLYGON (((20 10, 30 20, 40 30, 20 10), (-20.5 -10.5, -30.5 -20.5, -40.5 -30.5, -20.5 -10.5)), EMPTY, ((20 10, 30 20, 40 30, 20 10)))");
        }

        [Fact]
        public void WriteCollection()
        {
            Action<GeographyPipeline> emptyCalls = (w) =>
                                                    {
                                                        w.BeginGeography(SpatialType.Collection);
                                                        w.EndGeography();
                                                    };
            GeographyToWktTest(this.d2Formatter, emptyCalls, "SRID=4326;GEOMETRYCOLLECTION EMPTY");
            GeographyToWktTest(this.d4Formatter, emptyCalls, "SRID=4326;GEOMETRYCOLLECTION EMPTY");

            Action<GeographyPipeline> emptyCalls2 = (w) =>
                                                     {
                                                         w.BeginGeography(SpatialType.Collection);
                                                         w.BeginGeography(SpatialType.Point);
                                                         w.EndGeography();
                                                         w.BeginGeography(SpatialType.LineString);
                                                         w.EndGeography();
                                                         w.EndGeography();
                                                     };
            GeographyToWktTest(this.d2Formatter, emptyCalls2, "SRID=4326;GEOMETRYCOLLECTION (POINT EMPTY, LINESTRING EMPTY)");
            GeographyToWktTest(this.d4Formatter, emptyCalls2, "SRID=4326;GEOMETRYCOLLECTION (POINT EMPTY, LINESTRING EMPTY)");

            Action<GeographyPipeline> nestedEmptyCalls = (w) =>
                                                          {
                                                              w.BeginGeography(SpatialType.Collection);
                                                              w.BeginGeography(SpatialType.Collection);
                                                              w.EndGeography();
                                                              w.EndGeography();
                                                          };
            GeographyToWktTest(this.d2Formatter, nestedEmptyCalls, "SRID=4326;GEOMETRYCOLLECTION (GEOMETRYCOLLECTION EMPTY)");
            GeographyToWktTest(this.d4Formatter, nestedEmptyCalls, "SRID=4326;GEOMETRYCOLLECTION (GEOMETRYCOLLECTION EMPTY)");

            Action<GeographyPipeline> singlePointCalls = (w) =>
                                                          {
                                                              w.BeginGeography(SpatialType.Collection);
                                                              w.BeginGeography(SpatialType.Point);
                                                              w.BeginFigure(new GeographyPosition(10, 20, 30, 40));
                                                              w.EndFigure();
                                                              w.EndGeography();
                                                              w.EndGeography();
                                                          };
            GeographyToWktTest(this.d2Formatter, singlePointCalls, "SRID=4326;GEOMETRYCOLLECTION (POINT (20 10))");
            GeographyToWktTest(this.d4Formatter, singlePointCalls, "SRID=4326;GEOMETRYCOLLECTION (POINT (20 10 30 40))");

            Action<GeographyPipeline> pointMultiPointCalls = (w) =>
                                                              {
                                                                  w.BeginGeography(SpatialType.Collection);
                                                                  w.BeginGeography(SpatialType.Point);
                                                                  w.BeginFigure(new GeographyPosition(10, 20, null, null));
                                                                  w.EndFigure();
                                                                  w.EndGeography();

                                                                  w.BeginGeography(SpatialType.MultiPoint);
                                                                  w.BeginGeography(SpatialType.Point);
                                                                  w.BeginFigure(new GeographyPosition(20, 30, null, null));
                                                                  w.EndFigure();
                                                                  w.EndGeography();
                                                                  w.BeginGeography(SpatialType.Point);
                                                                  w.BeginFigure(new GeographyPosition(30, 40, null, null));
                                                                  w.EndFigure();
                                                                  w.EndGeography();
                                                                  w.EndGeography();

                                                                  w.EndGeography();
                                                              };
            GeographyToWktTest(this.d2Formatter, pointMultiPointCalls, "SRID=4326;GEOMETRYCOLLECTION (POINT (20 10), MULTIPOINT ((30 20), (40 30)))");
            GeographyToWktTest(this.d4Formatter, pointMultiPointCalls, "SRID=4326;GEOMETRYCOLLECTION (POINT (20 10), MULTIPOINT ((30 20), (40 30)))");
        }

        private static void GeographyToWktTest(WellKnownTextSqlFormatter formatter, Action<GeographyPipeline> pipelineAction, string expectedWkt)
        {
            var stringWriter = new StringWriter();
            var w = formatter.CreateWriter(stringWriter);

            w.GeographyPipeline.SetCoordinateSystem(CoordinateSystem.DefaultGeography);
            pipelineAction(w);

            Assert.Equal(expectedWkt, stringWriter.GetStringBuilder().ToString());
        }
    }
}

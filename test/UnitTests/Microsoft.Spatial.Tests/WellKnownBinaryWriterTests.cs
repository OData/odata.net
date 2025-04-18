//---------------------------------------------------------------------
// <copyright file="WellKnownBinaryWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using Xunit;

namespace Microsoft.Spatial.Tests
{
    public class WellKnownBinaryWriterTests
    {
        private readonly WellKnownBinaryFormatter d2Formatter = new WellKnownBinaryFormatterImplementation(new DataServicesSpatialImplementation(), new WellKnownBinaryWriterSettings { HandleZ = false, HandleM = false });
        private readonly WellKnownBinaryFormatter d4Formatter = new WellKnownBinaryFormatterImplementation(new DataServicesSpatialImplementation(), new WellKnownBinaryWriterSettings());

        [Theory]
        [InlineData(SpatialType.Point, SpatialType.Point)]
        [InlineData(SpatialType.Point, SpatialType.LineString)]
        [InlineData(SpatialType.LineString, SpatialType.Point)]
        [InlineData(SpatialType.Polygon, SpatialType.LineString)]
        [InlineData(SpatialType.Polygon, SpatialType.Point)]
        public void CallBeginGeo_Throws_OnInvalidSubSpatialType(SpatialType parent, SpatialType child)
        {
            Action<GeometryPipeline> testCall = w =>
            {
                w.BeginGeometry(parent);
                w.BeginGeometry(child);
            };

            var stream = new MemoryStream();
            var w = d4Formatter.CreateWriter(stream);
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => testCall(w));
            Assert.NotNull(exception);
            Assert.Equal(Error.Format(SRResources.WellKnownBinary_InvalidSubSpatial, child, parent, $"{parent} cannot contain other spatial types"), exception.Message);
        }

        [Theory]
        [InlineData(SpatialType.LineString)]
        [InlineData(SpatialType.Polygon)]
        [InlineData(SpatialType.MultiPoint)]
        public void CallBeginGeoOnMultiPoint_Throws_OnInvalidSubSpatialType(SpatialType child)
        {
            Action<GeometryPipeline> testCall = w =>
            {
                w.BeginGeometry(SpatialType.MultiPoint);
                w.BeginGeometry(child);
            };

            var stream = new MemoryStream();
            var w = d4Formatter.CreateWriter(stream);
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => testCall(w));
            Assert.NotNull(exception);
            Assert.Equal(Error.Format(SRResources.WellKnownBinary_InvalidSubSpatial, child, SpatialType.MultiPoint, "MultiPoint can only contain Points."), exception.Message);
        }

        [Theory]
        [InlineData(SpatialType.Point)]
        [InlineData(SpatialType.Polygon)]
        [InlineData(SpatialType.MultiPoint)]
        public void CallBeginGeoOnMultiLineString_Throws_OnInvalidSubSpatialType(SpatialType child)
        {
            Action<GeometryPipeline> testCall = w =>
            {
                w.BeginGeometry(SpatialType.MultiLineString);
                w.BeginGeometry(child);
            };

            var stream = new MemoryStream();
            var w = d4Formatter.CreateWriter(stream);
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => testCall(w));
            Assert.NotNull(exception);
            Assert.Equal(Error.Format(SRResources.WellKnownBinary_InvalidSubSpatial, child, SpatialType.MultiLineString, "MultiLineString can only contain LineStrings."), exception.Message);
        }

        [Theory]
        [InlineData(SpatialType.Point)]
        [InlineData(SpatialType.LineString)]
        [InlineData(SpatialType.MultiPoint)]
        public void CallBeginGeoOnMultiPolygon_Throws_OnInvalidSubSpatialType(SpatialType child)
        {
            Action<GeometryPipeline> testCall = w =>
            {
                w.BeginGeometry(SpatialType.MultiPolygon);
                w.BeginGeometry(child);
            };

            var stream = new MemoryStream();
            var w = d4Formatter.CreateWriter(stream);
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => testCall(w));
            Assert.NotNull(exception);
            Assert.Equal(Error.Format(SRResources.WellKnownBinary_InvalidSubSpatial, child, SpatialType.MultiPolygon, "MultiPolygon can only contain Polygons."), exception.Message);
        }

        [Fact]
        public void CallBeginFigure_Throws_OnNoSpatialTypeBegun()
        {
            Action<GeometryPipeline> testCall = w =>
            {
                w.BeginFigure(new GeometryPosition(10, 20, 30, 40));
            };

            var stream = new MemoryStream();
            var w = d4Formatter.CreateWriter(stream);
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => testCall(w));
            Assert.NotNull(exception);
            Assert.Equal(Error.Format(SRResources.WellKnownBinary_InvalidBeginOrEndFigureOrAddLine, "BeginFigure"), exception.Message);
        }

        [Theory]
        [InlineData(SpatialType.MultiPoint)]
        [InlineData(SpatialType.MultiLineString)]
        [InlineData(SpatialType.MultiPolygon)]
        [InlineData(SpatialType.Collection)]
        public void CallBeginFigure_Throws_OnInvalidSpatialTypeBegun(SpatialType parent)
        {
            Action<GeometryPipeline> testCall = w =>
            {
                w.BeginGeometry(parent);
                w.BeginFigure(new GeometryPosition(10, 20, 30, 40));
            };

            var stream = new MemoryStream();
            var w = d4Formatter.CreateWriter(stream);
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => testCall(w));
            Assert.NotNull(exception);
            Assert.Equal(Error.Format(SRResources.WellKnownBinary_InvalidBeginFigureOnSpatial, parent), exception.Message);
        }

        [Fact]
        public void CallBeginFigure_Throws_OnMultipleBeginFigureBegun()
        {
            Action<GeometryPipeline> testCall = w =>
            {
                w.BeginGeometry(SpatialType.LineString);
                w.BeginFigure(new GeometryPosition(10, 20));
                w.BeginFigure(new GeometryPosition(30, 20));
            };

            var stream = new MemoryStream();
            var w = d4Formatter.CreateWriter(stream);
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => testCall(w));
            Assert.NotNull(exception);
            Assert.Equal(Error.Format(SRResources.WellKnownBinary_InvalidBeginFigureWithoutEndingPrevious, SpatialType.LineString), exception.Message);
        }

        [Fact]
        public void CallLineTo_Throws_OnNoSpatialTypeBegun()
        {
            Action<GeometryPipeline> testCall = w =>
            {
                w.LineTo(new GeometryPosition(10, 20, 30, 40));
            };

            var stream = new MemoryStream();
            var w = d4Formatter.CreateWriter(stream);
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => testCall(w));
            Assert.NotNull(exception);
            Assert.Equal(Error.Format(SRResources.WellKnownBinary_InvalidBeginOrEndFigureOrAddLine, "LineTo"), exception.Message);
        }

        [Theory]
        [InlineData(SpatialType.LineString, "Should call BeginFigure first on LineString.")]
        [InlineData(SpatialType.Polygon, "Should call BeginFigure first on Polygon.")]
        [InlineData(SpatialType.Point, null)]
        [InlineData(SpatialType.MultiPoint, null)]
        [InlineData(SpatialType.MultiLineString, null)]
        [InlineData(SpatialType.MultiPolygon, null)]
        [InlineData(SpatialType.Collection, null)]
        public void CallLineTo_Throws_OnFigureBegun(SpatialType type, string details)
        {
            Action<GeometryPipeline> testCall = w =>
            {
                w.BeginGeometry(type);
                w.LineTo(new GeometryPosition(10, 20, 30, 40));
            };

            var stream = new MemoryStream();
            var w = d4Formatter.CreateWriter(stream);
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => testCall(w));
            Assert.NotNull(exception);
            Assert.Equal(Error.Format(SRResources.WellKnownBinary_InvalidAddLineTo, type, details ?? string.Empty), exception.Message);
        }

        [Fact]
        public void CallEndFigure_Throws_OnNoSpatialTypeBegun()
        {
            Action<GeometryPipeline> testCall = w =>
            {
                w.EndFigure();
            };

            var stream = new MemoryStream();
            var w = d4Formatter.CreateWriter(stream);
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => testCall(w));
            Assert.NotNull(exception);
            Assert.Equal(Error.Format(SRResources.WellKnownBinary_InvalidBeginOrEndFigureOrAddLine, "EndFigure"), exception.Message);
        }

        [Theory]
        [InlineData(SpatialType.Point, "You haven't begun the figure.")]
        [InlineData(SpatialType.LineString, "You haven't begun the figure.")]
        [InlineData(SpatialType.Polygon, "You haven't begun the figure.")]
        [InlineData(SpatialType.MultiLineString, null)]
        [InlineData(SpatialType.MultiPolygon, null)]
        [InlineData(SpatialType.MultiPoint, null)]
        [InlineData(SpatialType.Collection, null)]
        public void CallEndFigure_Throws_OnSpatialTypeBegun(SpatialType type, string details)
        {
            Action<GeometryPipeline> testCall = w =>
            {
                w.BeginGeometry(type);
                w.EndFigure();
            };

            var stream = new MemoryStream();
            var w = d4Formatter.CreateWriter(stream);
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => testCall(w));
            Assert.NotNull(exception);
            Assert.Equal(Error.Format(SRResources.WellKnownBinary_InvalidEndFigure, type, details ?? string.Empty), exception.Message);
        }

        [Fact]
        public void CallEndGeo_Throws_OnNoSpatialTypeBegun()
        {
            Action<GeometryPipeline> testCall = w =>
            {
                w.EndGeometry();
            };

            var stream = new MemoryStream();
            var w = d4Formatter.CreateWriter(stream);
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => testCall(w));
            Assert.NotNull(exception);
            Assert.Equal(Error.Format(SRResources.WellKnownBinary_InvalidBeginOrEndFigureOrAddLine, "EndGeo"), exception.Message);
        }

        [Fact]
        public void WritePoint_AsWKB()
        {
            Action<GeographyPipeline> emptyCalls = (w) =>
                            {
                                w.BeginGeography(SpatialType.Point);
                                w.EndGeography();
                            };
            GeographyToWkbTest(this.d2Formatter, emptyCalls, "0101000020E6100000000000000000F8FF000000000000F8FF");
            GeographyToWkbTest(this.d4Formatter, emptyCalls, "01B90B00E0E6100000000000000000F8FF000000000000F8FF000000000000F8FF000000000000F8FF");

            Action<GeographyPipeline> d4PointCalls = (w) =>
                                                         {
                                                             w.BeginGeography(SpatialType.Point);
                                                             w.BeginFigure(new GeographyPosition(10, 20, 30, 40));
                                                             w.EndFigure();
                                                             w.EndGeography();
                                                         };
            GeographyToWkbTest(this.d2Formatter, d4PointCalls, "0101000020E610000000000000000034400000000000002440");
            GeographyToWkbTest(this.d4Formatter, d4PointCalls, "01B90B00E0E6100000000000000000344000000000000024400000000000003E400000000000004440");

            Action<GeographyPipeline> d3PointCalls = (w) =>
                                                      {
                                                          w.BeginGeography(SpatialType.Point);
                                                          w.BeginFigure(new GeographyPosition(10, 20, 30, null));
                                                          w.EndFigure();
                                                          w.EndGeography();
                                                      };
            GeographyToWkbTest(this.d2Formatter, d3PointCalls, "0101000020E610000000000000000034400000000000002440");
            GeographyToWkbTest(this.d4Formatter, d3PointCalls, "01B90B00E0E6100000000000000000344000000000000024400000000000003E40000000000000F8FF");

            Action<GeographyPipeline> d2PointCalls = (w) =>
                                                      {
                                                          w.BeginGeography(SpatialType.Point);
                                                          w.BeginFigure(new GeographyPosition(10, 20, null, null)); // y,x
                                                          w.EndFigure();
                                                          w.EndGeography();
                                                      };
            GeographyToWkbTest(this.d2Formatter, d2PointCalls, "0101000020E610000000000000000034400000000000002440");
            GeographyToWkbTest(this.d4Formatter, d2PointCalls, "01B90B00E0E610000000000000000034400000000000002440000000000000F8FF000000000000F8FF");

            Action<GeographyPipeline> skipPointCalls =
            (w) =>
                {
                    w.BeginGeography(SpatialType.Point);
                    w.BeginFigure(new GeographyPosition(10, 20, null, 40)); // latitude, longitude
                    w.EndFigure();
                    w.EndGeography();
                };
            GeographyToWkbTest(this.d2Formatter, skipPointCalls, "0101000020E610000000000000000034400000000000002440");
            GeographyToWkbTest(this.d4Formatter, skipPointCalls, "01B90B00E0E610000000000000000034400000000000002440000000000000F8FF0000000000004440");
        }

        [Fact]
        public void WriteLineString_AsWKB()
        {
            Action<GeographyPipeline> emptyCalls = (w) =>
                                                    {
                                                        w.BeginGeography(SpatialType.LineString);
                                                        w.EndGeography();
                                                    };

            GeographyToWkbTest(this.d2Formatter, emptyCalls, "0102000020E610000000000000");
            GeographyToWkbTest(this.d4Formatter, emptyCalls, "01BA0B00E0E610000000000000");

            Action<GeographyPipeline> twoD2Point = (w) =>
                                                    {
                                                        w.BeginGeography(SpatialType.LineString);
                                                        w.BeginFigure(new GeographyPosition(10, 20, null, null));
                                                        w.LineTo(new GeographyPosition(20, 30, null, null));
                                                        w.EndFigure();
                                                        w.EndGeography();
                                                    };

            GeographyToWkbTest(this.d2Formatter, twoD2Point, "0102000020E610000002000000000000000000344000000000000024400000000000003E400000000000003440");
            GeographyToWkbTest(this.d4Formatter, twoD2Point, "01BA0B00E0E61000000200000000000000000034400000000000002440000000000000F8FF000000000000F8FF0000000000003E400000000000003440000000000000F8FF000000000000F8FF");

            Action<GeographyPipeline> threeD2Point = (w) =>
                                                      {
                                                          w.BeginGeography(SpatialType.LineString);
                                                          w.BeginFigure(new GeographyPosition(10, 20, null, null));
                                                          w.LineTo(new GeographyPosition(-20.5, -30, null, null));
                                                          w.LineTo(new GeographyPosition(30, 40, null, null));
                                                          w.EndFigure();
                                                          w.EndGeography();
                                                      };
            GeographyToWkbTest(this.d2Formatter, threeD2Point, "0102000020E610000003000000000000000000344000000000000024400000000000003EC000000000008034C000000000000044400000000000003E40");
            GeographyToWkbTest(this.d4Formatter, threeD2Point, "01BA0B00E0E61000000300000000000000000034400000000000002440000000000000F8FF000000000000F8FF0000000000003EC000000000008034C0000000000000F8FF000000000000F8FF00000000000044400000000000003E40000000000000F8FF000000000000F8FF");


            Action<GeographyPipeline> twoD4Point = (w) =>
                                                    {
                                                        w.BeginGeography(SpatialType.LineString);
                                                        w.BeginFigure(new GeographyPosition(10, 20, 30, 40));
                                                        w.LineTo(new GeographyPosition(20, 30, 40, 50));
                                                        w.EndFigure();
                                                        w.EndGeography();
                                                    };
            
            GeographyToWkbTest(this.d2Formatter, twoD4Point, "0102000020E610000002000000000000000000344000000000000024400000000000003E400000000000003440");
            GeographyToWkbTest(this.d4Formatter, twoD4Point, "01BA0B00E0E610000002000000000000000000344000000000000024400000000000003E4000000000000044400000000000003E40000000000000344000000000000044400000000000004940");
        }

        [Fact]
        public void WritePolygon_AsWKB()
        {
            Action<GeographyPipeline> emptyCalls = (w) =>
                                                    {
                                                        w.BeginGeography(SpatialType.Polygon);
                                                        w.EndGeography();
                                                    };
            GeographyToWkbTest(this.d2Formatter, emptyCalls, "0103000020E610000000000000");
            GeographyToWkbTest(this.d4Formatter, emptyCalls, "01BB0B00E0E610000000000000");


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
            GeographyToWkbTest(this.d2Formatter, fourD2Point, "0103000020E61000000100000004000000000000000000344000000000000024400000000000003E40000000000000344000000000000044400000000000003E4000000000000034400000000000002440");
            GeographyToWkbTest(this.d4Formatter, fourD2Point, "01BB0B00E0E6100000010000000400000000000000000034400000000000002440000000000000F8FF000000000000F8FF0000000000003E400000000000003440000000000000F8FF000000000000F8FF00000000000044400000000000003E40000000000000F8FF000000000000F8FF00000000000034400000000000002440000000000000F8FF000000000000F8FF");

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

            GeographyToWkbTest(this.d2Formatter, fourD2PointWith2D2Holes, "0103000020E61000000300000004000000000000000000344000000000000024400000000000003E40000000000000344000000000000044400000000000003E40000000000000344000000000000024400400000000000000000034C000000000000024C00000000000003EC000000000000034C000000000000044C00000000000003EC000000000000034C000000000000024C00400000000000000008034C000000000000025C00000000000803EC000000000008034C000000000004044C00000000000803EC000000000008034C000000000000025C0");
            GeographyToWkbTest(this.d4Formatter, fourD2PointWith2D2Holes, "01BB0B00E0E6100000030000000400000000000000000034400000000000002440000000000000F8FF000000000000F8FF0000000000003E400000000000003440000000000000F8FF000000000000F8FF00000000000044400000000000003E40000000000000F8FF000000000000F8FF00000000000034400000000000002440000000000000F8FF000000000000F8FF0400000000000000000034C000000000000024C0000000000000F8FF000000000000F8FF0000000000003EC000000000000034C0000000000000F8FF000000000000F8FF00000000000044C00000000000003EC0000000000000F8FF000000000000F8FF00000000000034C000000000000024C0000000000000F8FF000000000000F8FF0400000000000000008034C000000000000025C0000000000000F8FF000000000000F8FF0000000000803EC000000000008034C0000000000000F8FF000000000000F8FF00000000004044C00000000000803EC0000000000000F8FF000000000000F8FF00000000008034C000000000000025C0000000000000F8FF000000000000F8FF");
        }
        
        [Fact]
        public void WriteMultiPoint_AsWKB()
        {
            Action<GeographyPipeline> noPointsCalls = (w) =>
            {
                w.BeginGeography(SpatialType.MultiPoint);
                w.EndGeography();
            };
            GeographyToWkbTest(this.d2Formatter, noPointsCalls, "0104000020E610000000000000");
            GeographyToWkbTest(this.d4Formatter, noPointsCalls, "01BC0B00E0E610000000000000");

            Action<GeographyPipeline> twoEmptyPointsCalls = (w) =>
                                           {
                                               w.BeginGeography(SpatialType.MultiPoint);
                                               w.BeginGeography(SpatialType.Point);
                                               w.EndGeography();
                                               w.BeginGeography(SpatialType.Point);
                                               w.EndGeography();
                                               w.EndGeography();
                                           };
            GeographyToWkbTest(this.d2Formatter, twoEmptyPointsCalls, "0104000020E6100000020000000101000000000000000000F8FF000000000000F8FF0101000000000000000000F8FF000000000000F8FF");
            GeographyToWkbTest(this.d4Formatter, twoEmptyPointsCalls, "01BC0B00E0E61000000200000001B90B00C0000000000000F8FF000000000000F8FF000000000000F8FF000000000000F8FF01B90B00C0000000000000F8FF000000000000F8FF000000000000F8FF000000000000F8FF");

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
            GeographyToWkbTest(this.d2Formatter, twoD2PointsCalls, "0104000020E6100000030000000101000000000000000000344000000000000024400101000000000000000000F8FF000000000000F8FF010100000000000000000044400000000000003E40");
            GeographyToWkbTest(this.d4Formatter, twoD2PointsCalls, "01BC0B00E0E61000000300000001B90B00C000000000000034400000000000002440000000000000F8FF000000000000F8FF01B90B00C0000000000000F8FF000000000000F8FF000000000000F8FF000000000000F8FF01B90B00C000000000000044400000000000003E40000000000000F8FF000000000000F8FF");

            Action<GeographyPipeline> singleD3PointCalls = (w) =>
                                                          {
                                                              w.BeginGeography(SpatialType.MultiPoint);
                                                              w.BeginGeography(SpatialType.Point);
                                                              w.BeginFigure(new GeographyPosition(10, 20, 30, 40));
                                                              w.EndFigure();
                                                              w.EndGeography();
                                                              w.EndGeography();
                                                          };
            GeographyToWkbTest(this.d2Formatter, singleD3PointCalls, "0104000020E610000001000000010100000000000000000034400000000000002440");
            GeographyToWkbTest(this.d4Formatter, singleD3PointCalls, "01BC0B00E0E61000000100000001B90B00C0000000000000344000000000000024400000000000003E400000000000004440");
        }

        [Fact]
        public void WriteMultiLineString_AsWKB()
        {
            Action<GeographyPipeline> emptyCalls2 = (w) =>
            {
                w.BeginGeography(SpatialType.MultiLineString);
                w.EndGeography();
            };
            GeographyToWkbTest(this.d2Formatter, emptyCalls2, "0105000020E610000000000000");
            GeographyToWkbTest(this.d4Formatter, emptyCalls2, "01BD0B00E0E610000000000000");

            Action<GeographyPipeline> emptyCalls = (w) =>
                                                    {
                                                        w.BeginGeography(SpatialType.MultiLineString);
                                                        w.BeginGeography(SpatialType.LineString);
                                                        w.EndGeography();
                                                        w.EndGeography();
                                                    };
            GeographyToWkbTest(this.d2Formatter, emptyCalls, "0105000020E610000001000000010200000000000000");
            GeographyToWkbTest(this.d4Formatter, emptyCalls, "01BD0B00E0E61000000100000001BA0B00C000000000");

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
            GeographyToWkbTest(this.d2Formatter, twoD2LineStringCalls, "0105000020E610000003000000010200000002000000000000000000344000000000000024400000000000003E40000000000000344001020000000000000001020000000200000000000000000044400000000000003E4000000000000049400000000000004440");
            GeographyToWkbTest(this.d4Formatter, twoD2LineStringCalls, "01BD0B00E0E61000000300000001BA0B00C00200000000000000000034400000000000002440000000000000F8FF000000000000F8FF0000000000003E400000000000003440000000000000F8FF000000000000F8FF01BA0B00C00000000001BA0B00C00200000000000000000044400000000000003E40000000000000F8FF000000000000F8FF00000000000049400000000000004440000000000000F8FF000000000000F8FF");

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
            GeographyToWkbTest(this.d2Formatter, singleD3LineStringCalls, "0105000020E610000001000000010200000002000000000000000000344000000000000024400000000000003E400000000000003440");
            GeographyToWkbTest(this.d4Formatter, singleD3LineStringCalls, "01BD0B00E0E61000000100000001BA0B00C002000000000000000000344000000000000024400000000000004440000000000000F8FF0000000000003E4000000000000034400000000000004940000000000000F8FF");
        }

        [Fact]
        public void WriteMultiPolygon_AsWKB()
        {
            Action<GeographyPipeline> emptyCalls2 = (w) =>
            {
                w.BeginGeography(SpatialType.MultiPolygon);
                w.EndGeography();
            };
            GeographyToWkbTest(this.d2Formatter, emptyCalls2, "0106000020E610000000000000");
            GeographyToWkbTest(this.d4Formatter, emptyCalls2, "01BE0B00E0E610000000000000");

            Action<GeographyPipeline> emptyCalls = (w) =>
                                                    {
                                                        w.BeginGeography(SpatialType.MultiPolygon);
                                                        w.BeginGeography(SpatialType.Polygon);
                                                        w.EndGeography();
                                                        w.EndGeography();
                                                    };
            GeographyToWkbTest(this.d2Formatter, emptyCalls, "0106000020E610000001000000010300000000000000");
            GeographyToWkbTest(this.d4Formatter, emptyCalls, "01BE0B00E0E61000000100000001BB0B00C000000000");

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
            GeographyToWkbTest(this.d2Formatter, threeLineD2Calls, "0106000020E61000000300000001030000000200000004000000000000000000344000000000000024400000000000003E40000000000000344000000000000044400000000000003E40000000000000344000000000000024400400000000000000008034C000000000000025C00000000000803EC000000000008034C000000000004044C00000000000803EC000000000008034C000000000000025C001030000000000000001030000000100000004000000000000000000344000000000000024400000000000003E40000000000000344000000000000044400000000000003E4000000000000034400000000000002440");
            GeographyToWkbTest(this.d4Formatter, threeLineD2Calls, "01BE0B00E0E61000000300000001BB0B00C0020000000400000000000000000034400000000000002440000000000000F8FF000000000000F8FF0000000000003E400000000000003440000000000000F8FF000000000000F8FF00000000000044400000000000003E40000000000000F8FF000000000000F8FF00000000000034400000000000002440000000000000F8FF000000000000F8FF0400000000000000008034C000000000000025C0000000000000F8FF000000000000F8FF0000000000803EC000000000008034C0000000000000F8FF000000000000F8FF00000000004044C00000000000803EC0000000000000F8FF000000000000F8FF00000000008034C000000000000025C0000000000000F8FF000000000000F8FF01BB0B00C00000000001BB0B00C0010000000400000000000000000034400000000000002440000000000000F8FF000000000000F8FF0000000000003E400000000000003440000000000000F8FF000000000000F8FF00000000000044400000000000003E40000000000000F8FF000000000000F8FF00000000000034400000000000002440000000000000F8FF000000000000F8FF");
        }

        [Fact]
        public void WriteCollection_AsWKB()
        {
            Action<GeographyPipeline> emptyCalls = (w) =>
                                                    {
                                                        w.BeginGeography(SpatialType.Collection);
                                                        w.EndGeography();
                                                    };
            GeographyToWkbTest(this.d2Formatter, emptyCalls, "0107000020E610000000000000");
            GeographyToWkbTest(this.d4Formatter, emptyCalls, "01BF0B00E0E610000000000000");

            Action<GeographyPipeline> emptyCalls2 = (w) =>
                                                     {
                                                         w.BeginGeography(SpatialType.Collection);
                                                         w.BeginGeography(SpatialType.Point);
                                                         w.EndGeography();
                                                         w.BeginGeography(SpatialType.LineString);
                                                         w.EndGeography();
                                                         w.EndGeography();
                                                     };
            GeographyToWkbTest(this.d2Formatter, emptyCalls2, "0107000020E6100000020000000101000000000000000000F8FF000000000000F8FF010200000000000000");
            GeographyToWkbTest(this.d4Formatter, emptyCalls2, "01BF0B00E0E61000000200000001B90B00C0000000000000F8FF000000000000F8FF000000000000F8FF000000000000F8FF01BA0B00C000000000");

            Action<GeographyPipeline> nestedEmptyCalls = (w) =>
                                                          {
                                                              w.BeginGeography(SpatialType.Collection);
                                                              w.BeginGeography(SpatialType.Collection);
                                                              w.EndGeography();
                                                              w.EndGeography();
                                                          };
            GeographyToWkbTest(this.d2Formatter, nestedEmptyCalls, "0107000020E610000001000000010700000000000000");
            GeographyToWkbTest(this.d4Formatter, nestedEmptyCalls, "01BF0B00E0E61000000100000001BF0B00C000000000");

            Action<GeographyPipeline> singlePointCalls = (w) =>
                                                          {
                                                              w.BeginGeography(SpatialType.Collection);
                                                              w.BeginGeography(SpatialType.Point);
                                                              w.BeginFigure(new GeographyPosition(10, 20, 30, 40));
                                                              w.EndFigure();
                                                              w.EndGeography();
                                                              w.EndGeography();
                                                          };
            GeographyToWkbTest(this.d2Formatter, singlePointCalls, "0107000020E610000001000000010100000000000000000034400000000000002440");
            GeographyToWkbTest(this.d4Formatter, singlePointCalls, "01BF0B00E0E61000000100000001B90B00C0000000000000344000000000000024400000000000003E400000000000004440");

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
            GeographyToWkbTest(this.d2Formatter, pointMultiPointCalls, "0107000020E61000000200000001010000000000000000003440000000000000244001040000000200000001010000000000000000003E400000000000003440010100000000000000000044400000000000003E40");
            GeographyToWkbTest(this.d4Formatter, pointMultiPointCalls, "01BF0B00E0E61000000200000001B90B00C000000000000034400000000000002440000000000000F8FF000000000000F8FF01BC0B00C00200000001B90B00C00000000000003E400000000000003440000000000000F8FF000000000000F8FF01B90B00C000000000000044400000000000003E40000000000000F8FF000000000000F8FF");
        }

        [Fact]
        public void WriteCollection_AsWKB_ForGeometry()
        {
            Action<GeometryPipeline> singlePointCalls = (w) =>
            {
                w.BeginGeometry(SpatialType.Collection);
                w.BeginGeometry(SpatialType.Point);
                w.BeginFigure(new GeometryPosition(10, 20, 30, 40));
                w.EndFigure();
                w.EndGeometry();
                w.EndGeometry();
            };
            GeometryToWkbTest(this.d2Formatter, singlePointCalls, "01070000200000000001000000010100000000000000000024400000000000003440");
            GeometryToWkbTest(this.d4Formatter, singlePointCalls, "01BF0B00E0000000000100000001B90B00C0000000000000244000000000000034400000000000003E400000000000004440");

            Action<GeometryPipeline> pointMultiPointCalls = (w) =>
            {
                w.BeginGeometry(SpatialType.Collection);
                w.BeginGeometry(SpatialType.Point);
                w.BeginFigure(new GeometryPosition(10, 20, null, null));
                w.EndFigure();
                w.EndGeometry();
                w.BeginGeometry(SpatialType.MultiPoint);
                w.BeginGeometry(SpatialType.Point);
                w.BeginFigure(new GeometryPosition(20, 30, null, null));
                w.EndFigure();
                w.EndGeometry();
                w.BeginGeometry(SpatialType.Point);
                w.BeginFigure(new GeometryPosition(30, 40, null, null));
                w.EndFigure();
                w.EndGeometry();
                w.EndGeometry();

                w.EndGeometry();
            };
            GeometryToWkbTest(this.d2Formatter, pointMultiPointCalls, "01070000200000000002000000010100000000000000000024400000000000003440010400000002000000010100000000000000000034400000000000003E4001010000000000000000003E400000000000004440");
            GeometryToWkbTest(this.d4Formatter, pointMultiPointCalls, "01BF0B00E0000000000200000001B90B00C000000000000024400000000000003440000000000000F8FF000000000000F8FF01BC0B00C00200000001B90B00C000000000000034400000000000003E40000000000000F8FF000000000000F8FF01B90B00C00000000000003E400000000000004440000000000000F8FF000000000000F8FF");
        }

        private static void GeographyToWkbTest(WellKnownBinaryFormatter formatter, Action<GeographyPipeline> pipelineAction, string expectedWkb)
        {
            var stream = new MemoryStream();
            var w = formatter.CreateWriter(stream);

            w.GeographyPipeline.SetCoordinateSystem(CoordinateSystem.DefaultGeography);
            pipelineAction(w);

            byte[] result = stream.ToArray();
            string actual = ToHex(result);

             Assert.Equal(expectedWkb, actual);
        }

        private static void GeometryToWkbTest(WellKnownBinaryFormatter formatter, Action<GeometryPipeline> pipelineAction, string expectedWkb)
        {
            var stream = new MemoryStream();
            var w = formatter.CreateWriter(stream);

            w.GeographyPipeline.SetCoordinateSystem(CoordinateSystem.DefaultGeometry);
            pipelineAction(w);

            byte[] result = stream.ToArray();
            string actual = ToHex(result);

            Assert.Equal(expectedWkb, actual);
        }

        public static string ToHex(byte[] bytes)
        {
            var buf = new StringBuilder(bytes.Length * 2);
            for (int i = 0; i < bytes.Length; i++)
            {
                byte b = bytes[i];
                buf.Append(ToHexDigit((b >> 4) & 0x0F));
                buf.Append(ToHexDigit(b & 0x0F));
            }
            return buf.ToString();
        }

        private static char ToHexDigit(int n)
        {
            if (n < 0 || n > 15)
                throw new ArgumentException("Value out of range: " + n);
            if (n <= 9)
                return (char)('0' + n);
            return (char)('A' + (n - 10));
        }
    }
}

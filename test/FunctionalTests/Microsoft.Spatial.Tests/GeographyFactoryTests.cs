//---------------------------------------------------------------------
// <copyright file="GeographyFactoryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.Spatial.Tests;
using Xunit;

namespace Microsoft.Spatial.Tests
{
    public class GeographyFactoryTests
    {
        private static readonly CoordinateSystem NonDefaultGeographicCoords = CoordinateSystem.Geography(1234);

        [Fact]
        public void TestUsageOfValidator()
        {
            Action action = () => GeographyFactory.Polygon().Ring(10, 10).Build();
            var exception = SpatialTestUtils.RunCatching<FormatException>(action);
            Assert.True(exception != null, "didn't get an exception the validator is not in place.");
        }

        [Fact]
        public void BuildWithCommand()
        {
            var point = GeographyFactory.Point(10, 10).Build();
            point.VerifyAsPoint(new PositionData(10, 10));
        }

        [Fact]
        public void BuildWithExplicitCast()
        {
            var point = (Geography)GeographyFactory.Point(10, 10);
            point.VerifyAsPoint(new PositionData(10, 10));
        }

        [Fact]
        public void BuildPoint()
        {
            GeographyPoint p = GeographyFactory.Point(NonDefaultGeographicCoords, 10, 20, 30, 40);
            Assert.Equal(NonDefaultGeographicCoords, p.CoordinateSystem);
            p.VerifyAsPoint(new PositionData(10, 20, 30, 40));

            p = GeographyFactory.Point(NonDefaultGeographicCoords, 10, 20);
            Assert.Equal(NonDefaultGeographicCoords, p.CoordinateSystem);
            p.VerifyAsPoint(new PositionData(10, 20));

            p = GeographyFactory.Point(10, 20, 30, 40);
            Assert.Equal(CoordinateSystem.DefaultGeography, p.CoordinateSystem);
            p.VerifyAsPoint(new PositionData(10, 20, 30, 40));

            p = GeographyFactory.Point(10, 20);
            Assert.Equal(CoordinateSystem.DefaultGeography, p.CoordinateSystem);
            p.VerifyAsPoint(new PositionData(10, 20));

            p = GeographyFactory.Point();
            Assert.Equal(CoordinateSystem.DefaultGeography, p.CoordinateSystem);
            p.VerifyAsPoint(null);

            p = GeographyFactory.Point(NonDefaultGeographicCoords);
            Assert.Equal(NonDefaultGeographicCoords, p.CoordinateSystem);
            p.VerifyAsPoint(null);

            p = GeographyFactory.Point().LineTo(10, 20);
            Assert.Equal(CoordinateSystem.DefaultGeography, p.CoordinateSystem);
            p.VerifyAsPoint(new PositionData(10, 20));
        }

        [Fact]
        public void BuildLineString()
        {
            GeographyLineString ls = GeographyFactory.LineString(NonDefaultGeographicCoords, 10, 20, 30, 40).LineTo(20, 30, 40, 50);
            Assert.Equal(NonDefaultGeographicCoords, ls.CoordinateSystem);
            ls.VerifyAsLineString(new PositionData(10, 20, 30, 40), new PositionData(20, 30, 40, 50));

            ls = GeographyFactory.LineString(NonDefaultGeographicCoords, 10, 20).LineTo(20, 30);
            Assert.Equal(NonDefaultGeographicCoords, ls.CoordinateSystem);
            ls.VerifyAsLineString(new PositionData(10, 20), new PositionData(20, 30));

            ls = GeographyFactory.LineString(10, 20, 30, 40).LineTo(20, 30, 40, 50);
            Assert.Equal(CoordinateSystem.DefaultGeography, ls.CoordinateSystem);
            ls.VerifyAsLineString(new PositionData(10, 20, 30, 40), new PositionData(20, 30, 40, 50));

            ls = GeographyFactory.LineString(10, 20).LineTo(20, 30);
            Assert.Equal(CoordinateSystem.DefaultGeography, ls.CoordinateSystem);
            ls.VerifyAsLineString(new PositionData(10, 20), new PositionData(20, 30));

            ls = GeographyFactory.LineString();
            Assert.Equal(CoordinateSystem.DefaultGeography, ls.CoordinateSystem);
            ls.VerifyAsLineString(null);

            ls = GeographyFactory.LineString(NonDefaultGeographicCoords);
            Assert.Equal(NonDefaultGeographicCoords, ls.CoordinateSystem);
            ls.VerifyAsLineString(null);

            ls = GeographyFactory.LineString().LineTo(10, 20).LineTo(20, 30);
            ls.VerifyAsLineString(new PositionData(10, 20), new PositionData(20, 30));
        }

        [Fact]
        public void BuildPolygon()
        {
            GeographyPolygon poly = GeographyFactory.Polygon(NonDefaultGeographicCoords)
                .Ring(10, 20).LineTo(20, 30).LineTo(30, 40)
                .Ring(15, 25).LineTo(25, 35).LineTo(35, 45).LineTo(15, 25)
                .Ring(10, 20).LineTo(20, 30).LineTo(30, 40).LineTo(10, 20).LineTo(40, 50);

            Assert.Equal(NonDefaultGeographicCoords, poly.CoordinateSystem);

            poly.VerifyAsPolygon(new PositionData[] {
                new PositionData(10,20),
                new PositionData(20,30),
                new PositionData(30,40),
                new PositionData(10,20)
            },
            new PositionData[] {
                new PositionData(15,25),
                new PositionData(25,35),
                new PositionData(35,45),
                new PositionData(15,25)
            },
            new PositionData[] {
                new PositionData(10,20),
                new PositionData(20,30),
                new PositionData(30,40),
                new PositionData(10,20),
                new PositionData(40,50),                
                new PositionData(10,20)
            });

            poly = GeographyFactory.Polygon()
                .Ring(10, 20).LineTo(20, 30).LineTo(30, 40);

            Assert.Equal(CoordinateSystem.DefaultGeography, poly.CoordinateSystem);

            poly.VerifyAsPolygon(new PositionData[] {
                new PositionData(10,20),
                new PositionData(20,30),
                new PositionData(30,40),
                new PositionData(10,20)
            });

            poly = GeographyFactory.Polygon();
            poly.VerifyAsPolygon(null);
        }

        [Fact]
        public void BuildMultiPoint()
        {
            GeographyMultiPoint mp = GeographyFactory.MultiPoint(NonDefaultGeographicCoords).Point(10, 20).Point(20, 30).Point(30, 40);
            Assert.Equal(NonDefaultGeographicCoords, mp.CoordinateSystem);
            mp.VerifyAsMultiPoint(new PositionData(10, 20), new PositionData(20, 30), new PositionData(30, 40));

            mp = GeographyFactory.MultiPoint().Point(10, 20).Point(20, 30).Point(30, 40);
            Assert.Equal(CoordinateSystem.DefaultGeography, mp.CoordinateSystem);
            mp.VerifyAsMultiPoint(new PositionData(10, 20), new PositionData(20, 30), new PositionData(30, 40));

            mp = GeographyFactory.MultiPoint();
            mp.VerifyAsMultiPoint(null);

            mp = GeographyFactory.MultiPoint().Point(10, 20).Point();
            mp.VerifyAsMultiPoint(new PositionData(10, 20), null);
        }

        [Fact]
        public void BuildMultiLineString()
        {
            GeographyMultiLineString mls = GeographyFactory.MultiLineString(NonDefaultGeographicCoords)
                .LineString(10, 20).LineTo(20, 30).LineTo(30, 40)
                .LineString(20, 30).LineTo(30, 40).LineTo(40, 50);

            Assert.Equal(NonDefaultGeographicCoords, mls.CoordinateSystem);
            mls.VerifyAsMultiLineString(new PositionData[] 
            {
                new PositionData(10, 20),
                new PositionData(20, 30), 
                new PositionData(30, 40),
            },
            new PositionData[] 
            {
                new PositionData(20, 30), 
                new PositionData(30, 40), 
                new PositionData(40, 50)
            });

            mls = GeographyFactory.MultiLineString()
                .LineString(10, 20).LineTo(20, 30).LineTo(30, 40);

            Assert.Equal(CoordinateSystem.DefaultGeography, mls.CoordinateSystem);
            mls.VerifyAsMultiLineString(new PositionData[] 
            {
                new PositionData(10, 20), 
                new PositionData(20, 30), 
                new PositionData(30, 40),
            });

            mls = GeographyFactory.MultiLineString();
            mls.VerifyAsMultiLineString(null);

            mls = GeographyFactory.MultiLineString().LineString().LineString();
            mls.VerifyAsMultiLineString(null, null);
        }

        [Fact]
        public void BuildMultiPolygon()
        {
            GeographyMultiPolygon mp = GeographyFactory.MultiPolygon(NonDefaultGeographicCoords)
                .Polygon().Ring(10, 20).LineTo(20, 30).LineTo(30, 40)
                .Polygon().Ring(20, 30).LineTo(30, 40).LineTo(40, 50);

            Assert.Equal(NonDefaultGeographicCoords, mp.CoordinateSystem);

            mp.VerifyAsMultiPolygon(new PositionData[][]
            {
                new PositionData[]
                {
                    new PositionData(10, 20),
                    new PositionData(20, 30),
                    new PositionData(30, 40),
                    new PositionData(10, 20),
                }
            },
            new PositionData[][]
            {
                new PositionData[]
                {
                    new PositionData(20, 30),
                    new PositionData(30, 40),
                    new PositionData(40, 50),
                    new PositionData(20, 30),
                }
            });

            mp = GeographyFactory.MultiPolygon()
                .Polygon().Ring(10, 20).LineTo(20, 30).LineTo(30, 40);

            Assert.Equal(CoordinateSystem.DefaultGeography, mp.CoordinateSystem);

            mp.VerifyAsMultiPolygon(new PositionData[][]
            {
                new PositionData[]
                {
                    new PositionData(10, 20),
                    new PositionData(20, 30),
                    new PositionData(30, 40),
                    new PositionData(10, 20),
                }
            });
            
            mp = GeographyFactory.MultiPolygon();
            mp.VerifyAsMultiPolygon(null);

            mp = GeographyFactory.MultiPolygon().Polygon().Polygon();
            mp.VerifyAsMultiPolygon(null, null);
        }

        [Fact]
        public void BuildCollection()
        {
            GeographyCollection c = GeographyFactory.Collection(NonDefaultGeographicCoords)
                .MultiPoint().Point(5, 5).Point(10, 10)
                .LineString(0, 0).LineTo(0, 5)
                .MultiPolygon()
                    .Polygon().Ring(-5, -5).LineTo(0, -5).LineTo(0, -2)
                    .Polygon().Ring(-10, -10).LineTo(-5, -10).LineTo(-5, -7)
                .Collection()
                    .Point(5, 5);

            Assert.Equal(NonDefaultGeographicCoords, c.CoordinateSystem);
            c.VerifyAsCollection(
                (mp) => mp.VerifyAsMultiPoint(new PositionData(5, 5), new PositionData(10, 10)),
                (ls) => ls.VerifyAsLineString(new PositionData(0, 0), new PositionData(0, 5)),
                (mp) => mp.VerifyAsMultiPolygon(
                    new PositionData[][] { 
                        new PositionData[] { new PositionData(-5,-5), new PositionData(0,-5), new PositionData(0, -2), new PositionData(-5,-5) }},
                    new PositionData[][]                {
                        new PositionData[] { new PositionData(-10,-10), new PositionData(-5,-10), new PositionData(-5, -7), new PositionData(-10,-10) }}),
                (col) => col.VerifyAsCollection(
                    (p) => p.VerifyAsPoint(new PositionData(5, 5))));

            c = GeographyFactory.Collection();
            c.VerifyAsCollection(null);
        }
    }
}

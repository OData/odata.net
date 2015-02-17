//---------------------------------------------------------------------
// <copyright file="GeographyFactoryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace DataSpatialUnitTests.Tests
{
    using System;
    using Microsoft.Spatial;
    using DataSpatialUnitTests.Utils;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class GeographyFactoryTests
    {
        private static readonly CoordinateSystem NonDefaultGeographicCoords = CoordinateSystem.Geography(1234);

        [TestMethod]
        public void TestUsageOfValidator()
        {
            Action action = () => GeographyFactory.Polygon().Ring(10, 10).Build();
            var exception = SpatialTestUtils.RunCatching<FormatException>(action);
            Assert.IsNotNull(exception, "didn't get an exception the validator is not in place.");
        }

        [TestMethod]
        public void BuildWithCommand()
        {
            var point = GeographyFactory.Point(10, 10).Build();
            point.VerifyAsPoint(new PositionData(10, 10));
        }

        [TestMethod]
        public void BuildWithExplicitCast()
        {
            var point = (Geography)GeographyFactory.Point(10, 10);
            point.VerifyAsPoint(new PositionData(10, 10));
        }

        [TestMethod]
        public void BuildPoint()
        {
            GeographyPoint p = GeographyFactory.Point(NonDefaultGeographicCoords, 10, 20, 30, 40);
            Assert.AreEqual(NonDefaultGeographicCoords, p.CoordinateSystem);
            p.VerifyAsPoint(new PositionData(10, 20, 30, 40));

            p = GeographyFactory.Point(NonDefaultGeographicCoords, 10, 20);
            Assert.AreEqual(NonDefaultGeographicCoords, p.CoordinateSystem);
            p.VerifyAsPoint(new PositionData(10, 20));

            p = GeographyFactory.Point(10, 20, 30, 40);
            Assert.AreEqual(CoordinateSystem.DefaultGeography, p.CoordinateSystem);
            p.VerifyAsPoint(new PositionData(10, 20, 30, 40));

            p = GeographyFactory.Point(10, 20);
            Assert.AreEqual(CoordinateSystem.DefaultGeography, p.CoordinateSystem);
            p.VerifyAsPoint(new PositionData(10, 20));

            p = GeographyFactory.Point();
            Assert.AreEqual(CoordinateSystem.DefaultGeography, p.CoordinateSystem);
            p.VerifyAsPoint(null);

            p = GeographyFactory.Point(NonDefaultGeographicCoords);
            Assert.AreEqual(NonDefaultGeographicCoords, p.CoordinateSystem);
            p.VerifyAsPoint(null);

            p = GeographyFactory.Point().LineTo(10, 20);
            Assert.AreEqual(CoordinateSystem.DefaultGeography, p.CoordinateSystem);
            p.VerifyAsPoint(new PositionData(10, 20));
        }

        [TestMethod]
        public void BuildLineString()
        {
            GeographyLineString ls = GeographyFactory.LineString(NonDefaultGeographicCoords, 10, 20, 30, 40).LineTo(20, 30, 40, 50);
            Assert.AreEqual(NonDefaultGeographicCoords, ls.CoordinateSystem);
            ls.VerifyAsLineString(new PositionData(10, 20, 30, 40), new PositionData(20, 30, 40, 50));

            ls = GeographyFactory.LineString(NonDefaultGeographicCoords, 10, 20).LineTo(20, 30);
            Assert.AreEqual(NonDefaultGeographicCoords, ls.CoordinateSystem);
            ls.VerifyAsLineString(new PositionData(10, 20), new PositionData(20, 30));

            ls = GeographyFactory.LineString(10, 20, 30, 40).LineTo(20, 30, 40, 50);
            Assert.AreEqual(CoordinateSystem.DefaultGeography, ls.CoordinateSystem);
            ls.VerifyAsLineString(new PositionData(10, 20, 30, 40), new PositionData(20, 30, 40, 50));

            ls = GeographyFactory.LineString(10, 20).LineTo(20, 30);
            Assert.AreEqual(CoordinateSystem.DefaultGeography, ls.CoordinateSystem);
            ls.VerifyAsLineString(new PositionData(10, 20), new PositionData(20, 30));

            ls = GeographyFactory.LineString();
            Assert.AreEqual(CoordinateSystem.DefaultGeography, ls.CoordinateSystem);
            ls.VerifyAsLineString(null);

            ls = GeographyFactory.LineString(NonDefaultGeographicCoords);
            Assert.AreEqual(NonDefaultGeographicCoords, ls.CoordinateSystem);
            ls.VerifyAsLineString(null);

            ls = GeographyFactory.LineString().LineTo(10, 20).LineTo(20, 30);
            ls.VerifyAsLineString(new PositionData(10, 20), new PositionData(20, 30));
        }

        [TestMethod]
        public void BuildPolygon()
        {
            GeographyPolygon poly = GeographyFactory.Polygon(NonDefaultGeographicCoords)
                .Ring(10, 20).LineTo(20, 30).LineTo(30, 40)
                .Ring(15, 25).LineTo(25, 35).LineTo(35, 45).LineTo(15, 25)
                .Ring(10, 20).LineTo(20, 30).LineTo(30, 40).LineTo(10, 20).LineTo(40, 50);

            Assert.AreEqual(NonDefaultGeographicCoords, poly.CoordinateSystem);

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

            Assert.AreEqual(CoordinateSystem.DefaultGeography, poly.CoordinateSystem);

            poly.VerifyAsPolygon(new PositionData[] {
                new PositionData(10,20),
                new PositionData(20,30),
                new PositionData(30,40),
                new PositionData(10,20)
            });

            poly = GeographyFactory.Polygon();
            poly.VerifyAsPolygon(null);
        }

        [TestMethod]
        public void BuildMultiPoint()
        {
            GeographyMultiPoint mp = GeographyFactory.MultiPoint(NonDefaultGeographicCoords).Point(10, 20).Point(20, 30).Point(30, 40);
            Assert.AreEqual(NonDefaultGeographicCoords, mp.CoordinateSystem);
            mp.VerifyAsMultiPoint(new PositionData(10, 20), new PositionData(20, 30), new PositionData(30, 40));

            mp = GeographyFactory.MultiPoint().Point(10, 20).Point(20, 30).Point(30, 40);
            Assert.AreEqual(CoordinateSystem.DefaultGeography, mp.CoordinateSystem);
            mp.VerifyAsMultiPoint(new PositionData(10, 20), new PositionData(20, 30), new PositionData(30, 40));

            mp = GeographyFactory.MultiPoint();
            mp.VerifyAsMultiPoint(null);

            mp = GeographyFactory.MultiPoint().Point(10, 20).Point();
            mp.VerifyAsMultiPoint(new PositionData(10, 20), null);
        }

        [TestMethod]
        public void BuildMultiLineString()
        {
            GeographyMultiLineString mls = GeographyFactory.MultiLineString(NonDefaultGeographicCoords)
                .LineString(10, 20).LineTo(20, 30).LineTo(30, 40)
                .LineString(20, 30).LineTo(30, 40).LineTo(40, 50);

            Assert.AreEqual(NonDefaultGeographicCoords, mls.CoordinateSystem);
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

            Assert.AreEqual(CoordinateSystem.DefaultGeography, mls.CoordinateSystem);
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

        [TestMethod]
        public void BuildMultiPolygon()
        {
            GeographyMultiPolygon mp = GeographyFactory.MultiPolygon(NonDefaultGeographicCoords)
                .Polygon().Ring(10, 20).LineTo(20, 30).LineTo(30, 40)
                .Polygon().Ring(20, 30).LineTo(30, 40).LineTo(40, 50);

            Assert.AreEqual(NonDefaultGeographicCoords, mp.CoordinateSystem);

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

            Assert.AreEqual(CoordinateSystem.DefaultGeography, mp.CoordinateSystem);

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

        [TestMethod]
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

            Assert.AreEqual(NonDefaultGeographicCoords, c.CoordinateSystem);
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

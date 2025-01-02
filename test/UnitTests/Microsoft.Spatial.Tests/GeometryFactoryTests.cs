//---------------------------------------------------------------------
// <copyright file="GeometryFactoryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.Spatial.Tests;
using Xunit;

namespace Microsoft.Spatial.Tests
{
    public class GeometryFactoryTests
    {
        private static readonly CoordinateSystem NonDefaultGeometricCoords = CoordinateSystem.Geometry(1234);

        [Fact]
        public void TestUsageOfValidator()
        {
            Action action = () => GeometryFactory.Polygon().Ring(10, 10).Build();
            var exception = SpatialTestUtils.RunCatching<FormatException>(action);
            Assert.True(exception != null, "didn't get an exception the validator is not in place.");
        }

        [Fact]
        public void BuildWithCommand()
        {
            var point = GeometryFactory.Point(10, 10).Build();
            point.VerifyAsPoint(new PositionData(10, 10));
        }

        [Fact]
        public void BuildPoint()
        {
            GeometryPoint p = GeometryFactory.Point(NonDefaultGeometricCoords, 10, 20, 30, 40);
            Assert.Equal(NonDefaultGeometricCoords, p.CoordinateSystem);
            p.VerifyAsPoint(new PositionData(10, 20, 30, 40));

            p = GeometryFactory.Point(NonDefaultGeometricCoords, 10, 20);
            Assert.Equal(NonDefaultGeometricCoords, p.CoordinateSystem);
            p.VerifyAsPoint(new PositionData(10, 20));

            p = GeometryFactory.Point(10, 20, 30, 40);
            Assert.Equal(CoordinateSystem.DefaultGeometry, p.CoordinateSystem);
            p.VerifyAsPoint(new PositionData(10, 20, 30, 40));

            p = GeometryFactory.Point(10, 20);
            Assert.Equal(CoordinateSystem.DefaultGeometry, p.CoordinateSystem);
            p.VerifyAsPoint(new PositionData(10, 20));

            p = GeometryFactory.Point();
            Assert.Equal(CoordinateSystem.DefaultGeometry, p.CoordinateSystem);
            p.VerifyAsPoint(null);

            p = GeometryFactory.Point(NonDefaultGeometricCoords);
            Assert.Equal(NonDefaultGeometricCoords, p.CoordinateSystem);
            p.VerifyAsPoint(null);

            p = GeometryFactory.Point().LineTo(10, 20);
            Assert.Equal(CoordinateSystem.DefaultGeometry, p.CoordinateSystem);
            p.VerifyAsPoint(new PositionData(10, 20));
        }

        [Fact]
        public void BuildLineString()
        {
            GeometryLineString ls = GeometryFactory.LineString(NonDefaultGeometricCoords, 10, 20, 30, 40).LineTo(20, 30, 40, 50);
            Assert.Equal(NonDefaultGeometricCoords, ls.CoordinateSystem);
            ls.VerifyAsLineString(new PositionData(10, 20, 30, 40), new PositionData(20, 30, 40, 50));

            ls = GeometryFactory.LineString(NonDefaultGeometricCoords, 10, 20).LineTo(20, 30);
            Assert.Equal(NonDefaultGeometricCoords, ls.CoordinateSystem);
            ls.VerifyAsLineString(new PositionData(10, 20), new PositionData(20, 30));

            ls = GeometryFactory.LineString(10, 20, 30, 40).LineTo(20, 30, 40, 50);
            Assert.Equal(CoordinateSystem.DefaultGeometry, ls.CoordinateSystem);
            ls.VerifyAsLineString(new PositionData(10, 20, 30, 40), new PositionData(20, 30, 40, 50));

            ls = GeometryFactory.LineString(10, 20).LineTo(20, 30);
            Assert.Equal(CoordinateSystem.DefaultGeometry, ls.CoordinateSystem);
            ls.VerifyAsLineString(new PositionData(10, 20), new PositionData(20, 30));

            ls = GeometryFactory.LineString();
            Assert.Equal(CoordinateSystem.DefaultGeometry, ls.CoordinateSystem);
            ls.VerifyAsLineString(null);

            ls = GeometryFactory.LineString(NonDefaultGeometricCoords);
            Assert.Equal(NonDefaultGeometricCoords, ls.CoordinateSystem);
            ls.VerifyAsLineString(null);

            ls = GeometryFactory.LineString().LineTo(10, 20).LineTo(20, 30);
            ls.VerifyAsLineString(new PositionData(10, 20), new PositionData(20, 30));
        }

        [Fact]
        public void BuildPolygon()
        {
            GeometryPolygon poly = GeometryFactory.Polygon(NonDefaultGeometricCoords)
                .Ring(10, 20).LineTo(20, 30).LineTo(30, 40)
                .Ring(15, 25).LineTo(25, 35).LineTo(35, 45).LineTo(15, 25)
                .Ring(10, 20).LineTo(20, 30).LineTo(30, 40).LineTo(10, 20).LineTo(40, 50);

            Assert.Equal(NonDefaultGeometricCoords, poly.CoordinateSystem);

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

            poly = GeometryFactory.Polygon()
                .Ring(10, 20).LineTo(20, 30).LineTo(30, 40);

            Assert.Equal(CoordinateSystem.DefaultGeometry, poly.CoordinateSystem);

            poly.VerifyAsPolygon(new PositionData[] {
                new PositionData(10,20),
                new PositionData(20,30),
                new PositionData(30,40),
                new PositionData(10,20)
            });

            poly = GeometryFactory.Polygon();
            poly.VerifyAsPolygon(null);
        }

        [Fact]
        public void BuildMultiPoint()
        {
            GeometryMultiPoint mp = GeometryFactory.MultiPoint(NonDefaultGeometricCoords).Point(10, 20).Point(20, 30).Point(30, 40);
            Assert.Equal(NonDefaultGeometricCoords, mp.CoordinateSystem);
            mp.VerifyAsMultiPoint(new PositionData(10, 20), new PositionData(20, 30), new PositionData(30, 40));

            mp = GeometryFactory.MultiPoint().Point(10, 20).Point(20, 30).Point(30, 40);
            Assert.Equal(CoordinateSystem.DefaultGeometry, mp.CoordinateSystem);
            mp.VerifyAsMultiPoint(new PositionData(10, 20), new PositionData(20, 30), new PositionData(30, 40));

            mp = GeometryFactory.MultiPoint();
            mp.VerifyAsMultiPoint(null);

            mp = GeometryFactory.MultiPoint().Point(10, 20).Point();
            mp.VerifyAsMultiPoint(new PositionData(10, 20), null);
        }

        [Fact]
        public void BuildMultiLineString()
        {
            GeometryMultiLineString mls = GeometryFactory.MultiLineString(NonDefaultGeometricCoords)
                .LineString(10, 20).LineTo(20, 30).LineTo(30, 40)
                .LineString(20, 30).LineTo(30, 40).LineTo(40, 50);

            Assert.Equal(NonDefaultGeometricCoords, mls.CoordinateSystem);
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

            mls = GeometryFactory.MultiLineString()
                .LineString(10, 20).LineTo(20, 30).LineTo(30, 40);

            Assert.Equal(CoordinateSystem.DefaultGeometry, mls.CoordinateSystem);
            mls.VerifyAsMultiLineString(new PositionData[] 
            {
                new PositionData(10, 20), 
                new PositionData(20, 30), 
                new PositionData(30, 40),
            });

            mls = GeometryFactory.MultiLineString();
            mls.VerifyAsMultiLineString(null);

            mls = GeometryFactory.MultiLineString().LineString().LineString();
            mls.VerifyAsMultiLineString(null, null);
        }

        [Fact]
        public void BuildMultiPolygon()
        {
            GeometryMultiPolygon mp = GeometryFactory.MultiPolygon(NonDefaultGeometricCoords)
                .Polygon().Ring(10, 20).LineTo(20, 30).LineTo(30, 40)
                .Polygon().Ring(20, 30).LineTo(30, 40).LineTo(40, 50);

            Assert.Equal(NonDefaultGeometricCoords, mp.CoordinateSystem);

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

            mp = GeometryFactory.MultiPolygon()
                .Polygon().Ring(10, 20).LineTo(20, 30).LineTo(30, 40);

            Assert.Equal(CoordinateSystem.DefaultGeometry, mp.CoordinateSystem);

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

            mp = GeometryFactory.MultiPolygon();
            mp.VerifyAsMultiPolygon(null);

            mp = GeometryFactory.MultiPolygon().Polygon().Polygon();
            mp.VerifyAsMultiPolygon(null, null);
        }

        [Fact]
        public void BuildCollection()
        {
            GeometryCollection c = GeometryFactory.Collection(NonDefaultGeometricCoords)
                .MultiPoint().Point(5, 5).Point(10, 10)
                .LineString(0, 0).LineTo(0, 5)
                .MultiPolygon()
                    .Polygon().Ring(-5, -5).LineTo(0, -5).LineTo(0, -2)
                    .Polygon().Ring(-10, -10).LineTo(-5, -10).LineTo(-5, -7)
                .Collection()
                    .Point(5, 5);

            Assert.Equal(NonDefaultGeometricCoords, c.CoordinateSystem);
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

            c = GeometryFactory.Collection();
            c.VerifyAsCollection(null);
        }
    }
}

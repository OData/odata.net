//---------------------------------------------------------------------
// <copyright file="GeographyPointTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.Spatial.Tests;
using Xunit;

namespace Microsoft.Spatial.Tests
{
    public class GeographyPointTests
    {
        [Fact]
        public void CreatePoint()
        {
            GeographyPoint p = TestData.PointG(10, 20, 30, 40);
            Assert.Equal(10, p.Latitude);
            Assert.Equal(20, p.Longitude);
            Assert.Equal(30, (double)p.Z);
            Assert.Equal(40, (double)p.M);
            Assert.Equal(1234, p.CoordinateSystem.EpsgId);

            p = TestData.PointG(10, 20, null, null);
            Assert.Equal(10, p.Latitude);
            Assert.Equal(20, p.Longitude);
            Assert.False(p.Z.HasValue);
            Assert.False(p.M.HasValue);
            Assert.Equal(1234, p.CoordinateSystem.EpsgId);
        }

        [Fact]
        public void EmptyPoint()
        {
            GeographyPoint p = GeographyFactory.Point();
            Assert.True(p.IsEmpty);
            double coord;
            NotSupportedException ex = SpatialTestUtils.RunCatching<NotSupportedException>(() => coord = p.Latitude);
            Assert.NotNull(ex);
            Assert.Equal(Strings.Point_AccessCoordinateWhenEmpty, ex.Message);

            ex = SpatialTestUtils.RunCatching<NotSupportedException>(() => coord = p.Longitude);
            Assert.NotNull(ex);
            Assert.Equal(Strings.Point_AccessCoordinateWhenEmpty, ex.Message);

            Assert.False(p.Z.HasValue);
            Assert.False(p.M.HasValue);
        }

        [Fact]
        public void TestCreateMethodAll4DimensionsDefaultCoords()
        {
            GeographyPoint point = GeographyPoint.Create(10, 20, 30, 40);
            Assert.Equal(10, point.Latitude);
            Assert.Equal(20, point.Longitude);
            Assert.Equal(30, point.Z);
            Assert.Equal(40, point.M);
            Assert.Equal(CoordinateSystem.DefaultGeography, point.CoordinateSystem);
        }

        [Fact]
        public void TestCreateMethod3DimensionsDefaultCoords()
        {
            GeographyPoint point = GeographyPoint.Create(10, 20, 30);
            Assert.Equal(10, point.Latitude);
            Assert.Equal(20, point.Longitude);
            Assert.Equal(30, point.Z);
            Assert.False(point.M.HasValue);
            Assert.Equal(CoordinateSystem.DefaultGeography, point.CoordinateSystem);
        }

        [Fact]
        public void TestCreateMethod2DimensionsDefaultCoords()
        {
            GeographyPoint point = GeographyPoint.Create(15, 25);
            Assert.Equal(15, point.Latitude);
            Assert.Equal(25, point.Longitude);
            Assert.False(point.Z.HasValue);
            Assert.False(point.M.HasValue);
            Assert.Equal(CoordinateSystem.DefaultGeography, point.CoordinateSystem);
        }

        [Fact]
        public void TestCreateMethodAll4DimensionsCustomCoords()
        {
            CoordinateSystem coords = CoordinateSystem.Geography(4326);
            GeographyPoint point = GeographyPoint.Create(coords, 10, 20, 30, 40);
            Assert.Equal(10, point.Latitude);
            Assert.Equal(20, point.Longitude);
            Assert.Equal(30, point.Z);
            Assert.Equal(40, point.M);
            Assert.Equal(CoordinateSystem.DefaultGeography, point.CoordinateSystem);
            Assert.Equal(coords, point.CoordinateSystem);
        }
    }
}

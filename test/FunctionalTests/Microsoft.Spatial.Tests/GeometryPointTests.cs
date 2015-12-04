//---------------------------------------------------------------------
// <copyright file="GeometryPointTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Xunit;

namespace Microsoft.Spatial.Tests
{
    public class GeometryPointTests
    {
        [Fact]
        public void TestCreateMethodAll4DimensionsDefaultCoords()
        {
            GeometryPoint point = GeometryPoint.Create(10, 20, 30, 40);
            Assert.Equal(10, point.X);
            Assert.Equal(20, point.Y);
            Assert.Equal(30, point.Z);
            Assert.Equal(40, point.M);
            Assert.Equal(CoordinateSystem.DefaultGeometry, point.CoordinateSystem);
        }

        [Fact]
        public void TestCreateMethod3DimensionsDefaultCoords()
        {
            CoordinateSystem coords = CoordinateSystem.Geometry(12);
            GeometryPoint point = GeometryPoint.Create(10, 20, 30);
            Assert.Equal(10, point.X);
            Assert.Equal(20, point.Y);
            Assert.Equal(30, point.Z);
            Assert.False(point.M.HasValue);
            Assert.Equal(CoordinateSystem.DefaultGeometry, point.CoordinateSystem);
        }

        [Fact]
        public void TestCreateMethod2DimensionsDefaultCoords()
        {
            GeometryPoint point = GeometryPoint.Create(15, 25);
            Assert.Equal(15, point.X);
            Assert.Equal(25, point.Y);
            Assert.False(point.Z.HasValue);
            Assert.False(point.M.HasValue);
            Assert.Equal(CoordinateSystem.DefaultGeometry, point.CoordinateSystem);
        }

        [Fact]
        public void TestCreateMethod3DimensionsCustomCoords()
        {
            CoordinateSystem coords = CoordinateSystem.Geometry(12);
            GeometryPoint point = GeometryPoint.Create(coords, 10, 20, 30, 40);
            Assert.Equal(10, point.X);
            Assert.Equal(20, point.Y);
            Assert.Equal(30, point.Z);
            Assert.Equal(40, point.M);
            Assert.Equal(coords, point.CoordinateSystem);
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="GeometryPointTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Spatial;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataSpatialUnitTests.Tests
{
    [TestClass]
    public class GeometryPointTests
    {
        [TestMethod]
        public void TestCreateMethodAll4DimensionsDefaultCoords()
        {
            GeometryPoint point = GeometryPoint.Create(10, 20, 30, 40);
            Assert.AreEqual(10, point.X);
            Assert.AreEqual(20, point.Y);
            Assert.AreEqual(30, point.Z);
            Assert.AreEqual(40, point.M);
            Assert.AreEqual(CoordinateSystem.DefaultGeometry, point.CoordinateSystem);
        }

        [TestMethod]
        public void TestCreateMethod3DimensionsDefaultCoords()
        {
            CoordinateSystem coords = CoordinateSystem.Geometry(12);
            GeometryPoint point = GeometryPoint.Create(10, 20, 30);
            Assert.AreEqual(10, point.X);
            Assert.AreEqual(20, point.Y);
            Assert.AreEqual(30, point.Z);
            Assert.IsFalse(point.M.HasValue);
            Assert.AreEqual(CoordinateSystem.DefaultGeometry, point.CoordinateSystem);
        }

        [TestMethod]
        public void TestCreateMethod2DimensionsDefaultCoords()
        {
            GeometryPoint point = GeometryPoint.Create(15, 25);
            Assert.AreEqual(15, point.X);
            Assert.AreEqual(25, point.Y);
            Assert.IsFalse(point.Z.HasValue);
            Assert.IsFalse(point.M.HasValue);
            Assert.AreEqual(CoordinateSystem.DefaultGeometry, point.CoordinateSystem);
        }

        [TestMethod]
        public void TestCreateMethod3DimensionsCustomCoords()
        {
            CoordinateSystem coords = CoordinateSystem.Geometry(12);
            GeometryPoint point = GeometryPoint.Create(coords, 10, 20, 30, 40);
            Assert.AreEqual(10, point.X);
            Assert.AreEqual(20, point.Y);
            Assert.AreEqual(30, point.Z);
            Assert.AreEqual(40, point.M);
            Assert.AreEqual(coords, point.CoordinateSystem);
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="GeographyPointTests.cs" company="Microsoft">
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
    public class GeographyPointTests
    {
        [TestMethod]
        public void TestCreateMethodAll4DimensionsDefaultCoords()
        {
            GeographyPoint point = GeographyPoint.Create(10, 20, 30, 40);
            Assert.AreEqual(10, point.Latitude);
            Assert.AreEqual(20, point.Longitude);
            Assert.AreEqual(30, point.Z);
            Assert.AreEqual(40, point.M);
            Assert.AreEqual(CoordinateSystem.DefaultGeography, point.CoordinateSystem);
        }

        [TestMethod]
        public void TestCreateMethod3DimensionsDefaultCoords()
        {
            GeographyPoint point = GeographyPoint.Create(10, 20, 30);
            Assert.AreEqual(10, point.Latitude);
            Assert.AreEqual(20, point.Longitude);
            Assert.AreEqual(30, point.Z);
            Assert.IsFalse(point.M.HasValue);
            Assert.AreEqual(CoordinateSystem.DefaultGeography, point.CoordinateSystem);
        }

        [TestMethod]
        public void TestCreateMethod2DimensionsDefaultCoords()
        {
            GeographyPoint point = GeographyPoint.Create(15, 25);
            Assert.AreEqual(15, point.Latitude);
            Assert.AreEqual(25, point.Longitude);
            Assert.IsFalse(point.Z.HasValue);
            Assert.IsFalse(point.M.HasValue);
            Assert.AreEqual(CoordinateSystem.DefaultGeography, point.CoordinateSystem);
        }

        public void TestCreateMethodAll4DimensionsCustomCoords()
        {
            CoordinateSystem coords = CoordinateSystem.Geography(12);
            GeographyPoint point = GeographyPoint.Create(coords, 10, 20, 30, 40);
            Assert.AreEqual(10, point.Latitude);
            Assert.AreEqual(20, point.Longitude);
            Assert.AreEqual(30, point.Z);
            Assert.AreEqual(40, point.M);
            Assert.AreEqual(CoordinateSystem.DefaultGeography, point.CoordinateSystem);
            Assert.AreEqual(coords, point.CoordinateSystem);
        }
    }
}

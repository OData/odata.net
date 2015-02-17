//---------------------------------------------------------------------
// <copyright file="GeographicTypesTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace DataSpatialUnitTests.Tests
{
    using System;
    using Microsoft.Spatial;
    using DataSpatialUnitTests.Utils;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test functionalities in the geography types
    /// </summary>
    [TestClass]
    public class GeographyTest
    {
        [TestMethod]
        public void CreatePoint()
        {
            GeographyPoint p = TestData.PointG(10, 20, 30, 40);
            Assert.AreEqual(10, p.Latitude);
            Assert.AreEqual(20, p.Longitude);
            Assert.AreEqual(30, (double)p.Z);
            Assert.AreEqual(40, (double)p.M);
            Assert.AreEqual(1234, p.CoordinateSystem.EpsgId);

            p = TestData.PointG(10, 20, null, null);
            Assert.AreEqual(10, p.Latitude);
            Assert.AreEqual(20, p.Longitude);
            Assert.IsFalse(p.Z.HasValue);
            Assert.IsFalse(p.M.HasValue);
            Assert.AreEqual(1234, p.CoordinateSystem.EpsgId);
        }

        [TestMethod]
        public void EmptyPoint()
        {
            GeographyPoint p = GeographyFactory.Point();
            Assert.IsTrue(p.IsEmpty);
            double coord;
            NotSupportedException ex = SpatialTestUtils.RunCatching<NotSupportedException>(() => coord = p.Latitude);
            Assert.IsNotNull(ex);
            Assert.AreEqual(Strings.Point_AccessCoordinateWhenEmpty, ex.Message);

            ex = SpatialTestUtils.RunCatching<NotSupportedException>(() => coord = p.Longitude);
            Assert.IsNotNull(ex);
            Assert.AreEqual(Strings.Point_AccessCoordinateWhenEmpty, ex.Message);

            Assert.IsFalse(p.Z.HasValue);
            Assert.IsFalse(p.M.HasValue);
        }
    }
}

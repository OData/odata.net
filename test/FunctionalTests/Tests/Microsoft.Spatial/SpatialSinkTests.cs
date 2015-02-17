//---------------------------------------------------------------------
// <copyright file="SpatialSinkTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace DataSpatialUnitTests.Tests
{
    using Microsoft.Spatial;
    using DataSpatialUnitTests.Utils;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    [TestClass]
    public class SpatialPipelineTests
    {
        [TestMethod]
        public void PointsToPositions()
        {
            GeographyPoint p = TestData.PointG();
            var s = new SpatialToPositionPipeline();
            p.SendTo(s);
            Assert.AreEqual(1, s.Coordinates.Count);
            Assert.AreEqual(new PositionData(p.Latitude, p.Longitude, p.Z, p.M), s.Coordinates[0]);
            Assert.AreEqual(p.CoordinateSystem, s.CoordinateSystem);
        }
    }
}

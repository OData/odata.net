//---------------------------------------------------------------------
// <copyright file="CoordinateSystemTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace DataSpatialUnitTests.Tests
{
    using Microsoft.Spatial;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CoordinateSystemTests
    {
        private const CoordinateSystem.Topology Geography = CoordinateSystem.Topology.Geography;
        private const CoordinateSystem.Topology Geometry = CoordinateSystem.Topology.Geometry;

        [TestMethod]
        public void NotEqualToNull()
        {
            Assert.IsFalse(CoordinateSystem.DefaultGeometry.Equals(null));
        }

        [TestMethod]
        public void EqualWhenEverythingMatches()
        {
            Assert.AreEqual(Coords(), Coords());
        }

        [TestMethod]
        public void NotEqualWhenIdsDiffer()
        {
            Assert.AreNotEqual(Coords(epsgId: 1), Coords(epsgId: 2));
        }

        [TestMethod]
        public void NotEqualWhenFromDifferentTopologies()
        {
            Assert.AreNotEqual(Coords(topology: Geography), Coords(topology: Geometry));
        }

        [TestMethod]
        public void EqualWhenOnlyNameDiffers()
        {
            Assert.IsTrue(Coords(name:"funny name").Equals(Coords(name:"typical name")));
        }

        [TestMethod]
        public void EqualWhenSameInstance()
        {
            var testSubject = Coords();
            Assert.IsTrue(testSubject.Equals(testSubject));
        }

        [TestMethod]
        public void GeometryPropertyCaches()
        {
            Assert.AreSame(CoordinateSystem.Geometry(1), CoordinateSystem.Geometry(1));
        }

        [TestMethod]
        public void GeographyPropertyCaches()
        {
            Assert.AreSame(CoordinateSystem.Geography(1), CoordinateSystem.Geography(1));
        }

        [TestMethod]
        public void GeographyAndGeometyPropertiesReturnDifferentInstancesForSameId()
        {
            Assert.AreNotEqual(CoordinateSystem.Geography(1), CoordinateSystem.Geometry(1));
        }

        [TestMethod]
        public void ToWktImplementation()
        {
            Assert.AreEqual("SRID=1;", CoordinateSystem.Geography(1).ToWktId());
        }

        [TestMethod]
        public void ToStringImplementation()
        {
            Assert.AreEqual("GeographyCoordinateSystem(EpsgId=1)", CoordinateSystem.Geography(1).ToString());
        }

        [TestMethod]
        public void GetHashCodeImplementation()
        {
            Assert.AreEqual(1234, CoordinateSystem.Geography(1234).GetHashCode());
        }

        private static CoordinateSystem Coords(int epsgId = 99, string name = "usual name", CoordinateSystem.Topology topology = Geography)
        {
            return new CoordinateSystem(epsgId, name, topology);
        }
    }
}

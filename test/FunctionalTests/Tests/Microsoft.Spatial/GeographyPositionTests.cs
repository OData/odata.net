//---------------------------------------------------------------------
// <copyright file="GeographyPositionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace DataSpatialUnitTests.Tests
{
    using Microsoft.Spatial;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class GeographyPositionTests
    {
        [TestMethod]
        public void ToStringTests()
        {
            Assert.AreEqual("GeographyPosition(latitude:10, longitude:20, z:30, m:40)",
                new GeographyPosition(latitude: 10, longitude: 20, z: 30, m: 40).ToString());

            Assert.AreEqual("GeographyPosition(latitude:10, longitude:20, z:null, m:null)",
                new GeographyPosition(latitude: 10, longitude: 20).ToString());
        }

        [TestMethod]
        public void NotEqualToNull()
        {
            GeographyPosition position1 = new GeographyPosition(10, 20, 30, 40);
            Assert.IsFalse(position1.Equals(null));
        }

        [TestMethod]
        public void EqualWhenEverythingMatches()
        {
            GeographyPosition position1 = new GeographyPosition(10, 20, 30, 40);
            object position2 = new GeographyPosition(10, 20, 30, 40);
            Assert.IsTrue(position1.Equals(position2));
        }

        [TestMethod]
        public void ConstructorVariationHasSameEffect()
        {
            Assert.AreEqual(new GeographyPosition(10, 20, null, null), new GeographyPosition(10, 20));
        }

        [TestMethod]
        public void EqualWhenSameInstance()
        {
            GeographyPosition position1 = new GeographyPosition(10, 20, 30, 40);
            var testSubject = position1;
            Assert.IsTrue(testSubject == position1);
        }

        [TestMethod]
        public void NotEqualPositions()
        {
            GeographyPosition position1 = new GeographyPosition(10, 20, 30, 40);
            GeographyPosition position2 = new GeographyPosition(10, 20, null, 40);
            Assert.IsTrue(position1 != position2);
        }

        [TestMethod]
        public void NotEqualObjects()
        {
            GeographyPosition position1 = new GeographyPosition(10, 20, 30, 40);
            GeometryPosition position2 = new GeometryPosition(10, 20, 30, 40);
            Assert.IsFalse(position1.Equals(position2));
        }

        [TestMethod]
        public void GetHashCodeImplementation()
        {
            GeographyPosition position1 = new GeographyPosition(10, 20, 30, 40);
            Assert.AreEqual(2139226112, position1.GetHashCode());

            GeographyPosition position2 = new GeographyPosition(10, 20, null, null);
            Assert.AreEqual(48234496, position2.GetHashCode());
        }
    }
}

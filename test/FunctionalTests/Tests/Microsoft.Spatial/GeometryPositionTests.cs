//---------------------------------------------------------------------
// <copyright file="GeometryPositionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace DataSpatialUnitTests.Tests
{
    using Microsoft.Spatial;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class GeometryPositionTests
    {
        [TestMethod]
        public void ToStringTests()
        {
            Assert.AreEqual("GeometryPosition(10, 20, 30, 40)",
                new GeometryPosition(10, 20, 30, 40).ToString());

            Assert.AreEqual("GeometryPosition(10, 20, null, null)",
                new GeometryPosition(10, 20).ToString());
        }

        [TestMethod]
        public void NotEqualToNull()
        {
            GeometryPosition position1 = new GeometryPosition(10, 20, 30, 40);
            Assert.IsFalse(position1.Equals(null));
        }

        [TestMethod]
        public void EqualWhenEverythingMatches()
        {
            GeometryPosition position1 = new GeometryPosition(10, 20, 30, 40);
            object position2 = new GeometryPosition(10, 20, 30, 40);
            Assert.IsTrue(position1.Equals(position2));
        }

        [TestMethod]
        public void ConstructorVariationHasSameEffect()
        {
            Assert.AreEqual(new GeometryPosition(10, 20, null, null), new GeometryPosition(10, 20));
        }

        [TestMethod]
        public void EqualWhenSameInstance()
        {
            GeometryPosition position1 = new GeometryPosition(10, 20, 30, 40);
            var testSubject = position1;
            Assert.IsTrue(testSubject == position1);
        }

        [TestMethod]
        public void NotEqualPositions()
        {
            GeometryPosition position1 = new GeometryPosition(10, 20, 30, 40);
            GeometryPosition position2 = new GeometryPosition(40, 30, 20, 10);
            Assert.IsTrue(position1 != position2);
        }

        [TestMethod]
        public void NotEqualObjects()
        {
            GeometryPosition position1 = new GeometryPosition(10, 20, 30, 40);
            GeographyPosition position2 = new GeographyPosition(10, 20, 30, 40);
            Assert.IsFalse(position1.Equals(position2));
        }
        
        [TestMethod]
        public void GetHashCodeImplementation()
        {
            GeometryPosition position1 = new GeometryPosition(10, 20, 30, 40);
            Assert.AreEqual(2139226112, position1.GetHashCode());

            GeometryPosition position2 = new GeometryPosition(10, 20, null, null);
            Assert.AreEqual(48234496, position2.GetHashCode());
        }
    }
}

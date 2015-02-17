//---------------------------------------------------------------------
// <copyright file="GeoObjectImplementationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace DataSpatialUnitTests.Tests
{
    using System;
    using Microsoft.Spatial;
    using DataSpatialUnitTests.Utils;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class GeoObjectImplementationTests
    {
        [TestMethod]
        public void DefaultEqualityTest()
        {
            // no impl for GeographyCurve
            this.DefaultEqualityTest<GeographyFullGlobe>(() => TestData.FullGlobe());
            this.DefaultEqualityTest(() => GeographyFactory.Collection().Point(-19.99, -12.0).Build());
            this.DefaultEqualityTest(() => GeographyFactory.LineString(33.1, -110.0).LineTo(35.97, -110).Build());
            this.DefaultEqualityTest(() => GeographyFactory.MultiLineString().LineString(10.2, 11.2).LineTo(11.9, 11.6).LineString(16.2, 17.2).LineTo(18.9, 19.6).Build());
            this.DefaultEqualityTest(() => GeographyFactory.MultiPoint().Point(10.2, 11.2).Point(11.9, 11.6).Build());
            this.DefaultEqualityTest(() => GeographyFactory.MultiPolygon().Polygon().Ring(10.2, 11.2).LineTo(11.9, 11.6).LineTo(11.45, 87.75).Ring(16.2, 17.2).LineTo(18.9, 19.6).LineTo(11.45, 87.75).Build());
            this.DefaultEqualityTest(() => GeographyFactory.Point(32.0, -100.0).Build());
            this.DefaultEqualityTest(() => GeographyFactory.Polygon().Ring(33.1, -110.0).LineTo(35.97, -110.15).LineTo(11.45, 87.75).Ring(35.97, -110).LineTo(36.97, -110.15).LineTo(45.23, 23.18).Build());
            
            // no impl for GeographySurface

            this.GeographyEqualityTestIncludingBaseObviousStuff(() => GeographyFactory.Point(32.0, -100.0).Build(), () => GeographyFactory.Point().Build());
            
            // no impl for GeometryCurve
            
            this.DefaultEqualityTest(() => GeometryFactory.Collection().Point(-19.99, -12.0).Build());
            this.DefaultEqualityTest(() => GeometryFactory.LineString(33.1, -11.5).LineTo(35.97, -11).Build());
            this.DefaultEqualityTest(() => GeometryFactory.MultiLineString().LineString(10.2, 11.2).LineTo(11.9, 11.6).LineString(16.2, 17.2).LineTo(18.9, 19.6).Build());
            this.DefaultEqualityTest(() => GeometryFactory.MultiPoint().Point(10.2, 11.2).Point(11.9, 11.6).Build());
            this.DefaultEqualityTest(() => GeometryFactory.MultiPolygon().Polygon().Ring(10.2, 11.2).LineTo(11.9, 11.6).LineTo(11.45, 87.75).Ring(16.2, 17.2).LineTo(18.9, 19.6).LineTo(11.45, 87.75).Build());
            this.DefaultEqualityTest(() => GeometryFactory.Point(32.0, -10.0).Build());
            this.DefaultEqualityTest(() => GeometryFactory.Polygon().Ring(33.1, -13.6).LineTo(35.97, -11.15).LineTo(11.45, 87.75).Ring(35.97, -11).LineTo(36.97, -11.15).LineTo(45.23, 23.18).Build());
            
            // no impl GeometrySurface

            this.GeometryEqualityTestIncludingBaseObviousStuff(() => GeometryFactory.Point(32.0, -10.0).Build(), () => GeometryFactory.Point().Build());
        }

        // I tried to make this generic and call it for both the Geometry, and Geography base
        // classes, but then it wouldn't call the correct == and != operators, kept calling the one on Object.
        public void GeographyEqualityTestIncludingBaseObviousStuff(Func<Geography> create, Func<Geography> createEmpty)
        {
            Geography instance = create();
            Assert.IsFalse(instance.Equals(null));
            Assert.IsTrue(instance.Equals(instance));

            var instance2 = create();
            Assert.IsTrue(instance.Equals(instance2));

            var empty = createEmpty();
            Assert.IsFalse(instance.Equals(empty));
            Assert.IsFalse(empty.Equals(instance));
            var empty2 = createEmpty();
            Assert.IsTrue(empty.Equals(empty2));
            
            Assert.AreNotEqual(0, instance.GetHashCode());
        }

        // I tried to make this generic and call it for both the Geometry, and Geography base
        // classes, but then it wouldn't call the correct == and != operators, kept calling the one on Object.
        public void GeometryEqualityTestIncludingBaseObviousStuff(Func<Geometry> create, Func<Geometry> createEmpty)
        {
            Geometry instance = create();
            Assert.IsFalse(instance.Equals(null));
            Assert.IsTrue(instance.Equals(instance));

            var instance2 = create();
            Assert.IsTrue(instance.Equals(instance2));

            var empty = createEmpty();
            Assert.IsFalse(instance.Equals(empty));
            Assert.IsFalse(empty.Equals(instance));
            var empty2 = createEmpty();
            Assert.IsTrue(empty.Equals(empty2));

            Assert.AreNotEqual(0, instance.GetHashCode());
        }

        public void DefaultEqualityTest<T>(Func<T> createT) where T : class
        {
            var instance = createT();
            var instance2 = createT();

            Assert.IsTrue(instance.Equals(instance2));
            Assert.AreNotEqual(0, instance.GetHashCode());
            Assert.AreEqual(instance.GetHashCode(), instance2.GetHashCode());
        }
    }
}

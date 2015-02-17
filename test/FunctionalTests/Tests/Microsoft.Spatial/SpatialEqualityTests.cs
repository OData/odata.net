//---------------------------------------------------------------------
// <copyright file="SpatialEqualityTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace DataSpatialUnitTests.Tests
{
    using System;
    using Microsoft.Spatial;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SpatialEqualityTests
    {
        private static Geography[] geographyTypes;
        private static Geometry[] geometryTypes;

        public enum GeographyTypeKind
        {
            Point = 0,
            LineString,
            MultiLineString,
            MultiPoint,
            Polygon,
            MultiPolygon,
            Collection,
            FullGlobe
        }

        public enum GeometryTypeKind
        {
            Point = 0,
            LineString,
            MultiLineString,
            MultiPoint,
            Polygon,
            MultiPolygon,
            Collection
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            geographyTypes = new Geography[Enum.GetValues(typeof(GeographyTypeKind)).Length];
            geographyTypes[(int)GeographyTypeKind.Point] = TestData.PointG();
            geographyTypes[(int)GeographyTypeKind.LineString] = TestData.LineStringG();
            geographyTypes[(int)GeographyTypeKind.MultiLineString] = TestData.MultiLineStringG();
            geographyTypes[(int)GeographyTypeKind.MultiPoint] = TestData.MultiPointG();
            geographyTypes[(int)GeographyTypeKind.Polygon] = TestData.PolygonG();
            geographyTypes[(int)GeographyTypeKind.MultiPolygon] = TestData.MultiPolygonG();
            geographyTypes[(int)GeographyTypeKind.Collection] = TestData.GeographyCollection();
            geographyTypes[(int)GeographyTypeKind.FullGlobe] = TestData.FullGlobe();

            geometryTypes = new Geometry[Enum.GetValues(typeof(GeometryTypeKind)).Length];
            geometryTypes[(int)GeometryTypeKind.Point] = TestData.PointM();
            geometryTypes[(int)GeometryTypeKind.LineString] = TestData.LineStringM();
            geometryTypes[(int)GeometryTypeKind.MultiLineString] = TestData.MultiLineStringM();
            geometryTypes[(int)GeometryTypeKind.MultiPoint] = TestData.MultiPointM();
            geometryTypes[(int)GeometryTypeKind.Polygon] = TestData.PolygonM();
            geometryTypes[(int)GeometryTypeKind.MultiPolygon] = TestData.MultiPolygonM();
            geometryTypes[(int)GeometryTypeKind.Collection] = TestData.GeometryCollection();
        }

        [TestMethod]
        public void GetHashCodeTests()
        {
            var gp1 = TestData.PointG();
            var gp2 = TestData.PointG();
            Assert.AreEqual(gp1.GetHashCode(), gp2.GetHashCode(), "HashCode must be equal for points with same values");

            var fullGlobe1 = TestData.FullGlobe();
            var fullGlobe2 = TestData.FullGlobe();
            var fullGlobe3 = TestData.FullGlobe(CoordinateSystem.Geography(5555));
            Assert.AreEqual(fullGlobe1.GetHashCode(), fullGlobe2.GetHashCode(), "HashCode must be equal for GeographyCollection with same values");
            Assert.AreNotEqual(fullGlobe1.GetHashCode(), fullGlobe3.GetHashCode(), "HashCode must be equal for GeographyCollection with same values");

            var geometryPoint1 = TestData.PointM();
            var geometryPoint2 = TestData.PointM();
            Assert.AreEqual(geometryPoint1.GetHashCode(), geometryPoint2.GetHashCode(), "HashCode must be equal for points with same values");

                var lineString1 = TestData.LineStringG();
                var lineString2 = TestData.LineStringG();
                Assert.AreEqual(lineString1.GetHashCode(), lineString2.GetHashCode(), "HashCode must be equal for line string with same values");

                var multiLineString1 = TestData.MultiLineStringG();
                var multiLineString2 = TestData.MultiLineStringG();
                Assert.AreEqual(multiLineString1.GetHashCode(), multiLineString2.GetHashCode(), "HashCode must be equal for multiLineString with same values");

                var multiPointImplementation1 = TestData.MultiPointG();
                var multiPointImplementation2 = TestData.MultiPointG();
                Assert.AreEqual(multiPointImplementation1.GetHashCode(), multiPointImplementation2.GetHashCode(), "HashCode must be equal for multiPoint with same values");

                var polygon1 = TestData.PolygonG();
                var polygon2 = TestData.PolygonG();
                Assert.AreEqual(polygon1.GetHashCode(), polygon2.GetHashCode(), "HashCode must be equal for polygon with same values");

                var multiPolygon1 = TestData.MultiPolygonG();
                var multiPolygon2 = TestData.MultiPolygonG();
                Assert.AreEqual(multiPolygon1.GetHashCode(), multiPolygon2.GetHashCode(), "HashCode must be equal for multiPolygon with same values");

                var geographyCollection1 = TestData.GeographyCollection();
                var geographyCollection2 = TestData.GeographyCollection();
                Assert.AreEqual(geographyCollection1.GetHashCode(), geographyCollection2.GetHashCode(), "HashCode must be equal for GeographyCollection with same values");

                var geometryLineString1 = TestData.LineStringM();
                var geometryLineString2 = TestData.LineStringM();
                Assert.AreEqual(geometryLineString1.GetHashCode(), geometryLineString2.GetHashCode(), "HashCode must be equal for line string with same values");

                var geometrymultiLineString1 = TestData.MultiLineStringM();
                var geometrymultiLineString2 = TestData.MultiLineStringM();
                Assert.AreEqual(geometrymultiLineString1.GetHashCode(), geometrymultiLineString2.GetHashCode(), "HashCode must be equal for multiLineString with same values");

                var geometrymultiPointImplementation1 = TestData.MultiPointM();
                var geometrymultiPointImplementation2 = TestData.MultiPointM();
                Assert.AreEqual(geometrymultiPointImplementation1.GetHashCode(), geometrymultiPointImplementation2.GetHashCode(), "HashCode must be equal for multiPoint with same values");

                var geometryPolygon1 = TestData.PolygonM();
                var geometryPolygon2 = TestData.PolygonM();
                Assert.AreEqual(geometryPolygon1.GetHashCode(), geometryPolygon2.GetHashCode(), "HashCode must be equal for polygon with same values");

                var geometryMultiPolygon1 = TestData.MultiPolygonM();
                var geometryMultiPolygon2 = TestData.MultiPolygonM();
                Assert.AreEqual(geometryMultiPolygon1.GetHashCode(), geometryMultiPolygon2.GetHashCode(), "HashCode must be equal for MultiPolygon with same values");

                var geometryCollection1 = TestData.GeometryCollection();
                var geometryCollection2 = TestData.GeometryCollection();
                Assert.AreEqual(geometryCollection1.GetHashCode(), geometryCollection2.GetHashCode(), "HashCode must be equal for GeometryCollection with same values");
        }

        [TestMethod]
        public void GetHashCodeTestsForEmptyTypes()
        {
            var emptyPointG = TestData.PointGEmpty();
            var samePointG = TestData.PointGEmpty();
            var differentPointG = TestData.PointGEmpty(CoordinateSystem.Geography(5555));
            Assert.AreEqual(emptyPointG.GetHashCode(), samePointG.GetHashCode());
            Assert.AreNotEqual(emptyPointG.GetHashCode(), differentPointG.GetHashCode());

            var emptyPointM = TestData.PointMEmpty();
            var samePointM = TestData.PointMEmpty();
            var differentPointM = TestData.PointMEmpty(CoordinateSystem.Geometry(5555));
            Assert.AreEqual(emptyPointM.GetHashCode(), samePointM.GetHashCode());
            Assert.AreNotEqual(emptyPointM.GetHashCode(), differentPointM.GetHashCode());
        }

        [TestMethod]
        public void RepresentationEquality_EqualsTests()
        {
            var gp1 = TestData.PointG();
            var gp2 = TestData.PointG();
            Assert.AreEqual(gp1, gp2, "Values must be equal for points with same values");

            var fullGlobe1 = TestData.FullGlobe();
            var fullGlobe2 = TestData.FullGlobe();
            var fullGlobe3 = TestData.FullGlobe(CoordinateSystem.Geography(5555));
            Assert.AreEqual(fullGlobe1, fullGlobe2, "Values must be equal for globes in same coordinate system");
            Assert.AreNotEqual(fullGlobe1, fullGlobe3, "Values must be non-equal if they are in different coordinate systems");

            var geometryPoint1 = TestData.PointM();
            var geometryPoint2 = TestData.PointM();
            Assert.AreEqual(geometryPoint1, geometryPoint2, "Values must be equal for points with same values");

                var lineString1 = TestData.LineStringG();
                var lineString2 = TestData.LineStringG();
                Assert.AreEqual(lineString1, lineString2, "Values must be equal for line string with same values");

                var multiLineString1 = TestData.MultiLineStringG();
                var multiLineString2 = TestData.MultiLineStringG();
                Assert.AreEqual(multiLineString1, multiLineString2, "Values must be equal for multiLineString with same values");

                var multiPointImplementation1 = TestData.MultiPointG();
                var multiPointImplementation2 = TestData.MultiPointG();
                Assert.AreEqual(multiPointImplementation1, multiPointImplementation2, "Values must be equal for multiPoint with same values");

                var polygon1 = TestData.PolygonG();
                var polygon2 = TestData.PolygonG();
                Assert.AreEqual(polygon1, polygon2, "Values must be equal for polygon with same values");

                var multiPolygon1 = TestData.MultiPolygonG();
                var multiPolygon2 = TestData.MultiPolygonG();
                Assert.AreEqual(multiPolygon1, multiPolygon2, "Values must be equal for multiPolygon with same values");

                var geographyCollection1 = TestData.GeographyCollection();
                var geographyCollection2 = TestData.GeographyCollection();
                Assert.AreEqual(geographyCollection1, geographyCollection2, "Values must be equal for GeographyCollection with same values");

                var geometryLineString1 = TestData.LineStringM();
                var geometryLineString2 = TestData.LineStringM();
                Assert.AreEqual(geometryLineString1, geometryLineString2, "Values must be equal for line string with same values");

                var geometryMultiLineString1 = TestData.MultiLineStringM();
                var geometryMultiLineString2 = TestData.MultiLineStringM();
                Assert.AreEqual(geometryMultiLineString1, geometryMultiLineString2, "Values must be equal for multiLineString with same values");

                var geometryMultiPointImplementation1 = TestData.MultiPointM();
                var geometryMultiPointImplementation2 = TestData.MultiPointM();
                Assert.AreEqual(geometryMultiPointImplementation1, geometryMultiPointImplementation2, "Values must be equal for multiPoint with same values");

                var geometryPolygon1 = TestData.PolygonM();
                var geometryPolygon2 = TestData.PolygonM();
                Assert.AreEqual(geometryPolygon1, geometryPolygon2, "Values must be equal for polygon with same values");

                var geometryMultiPolygon1 = TestData.MultiPolygonM();
                var geometryMultiPolygon2 = TestData.MultiPolygonM();
                Assert.AreEqual(geometryMultiPolygon1, geometryMultiPolygon2, "Values must be equal for MultiPolygon with same values");

                var geometryCollection1 = TestData.GeometryCollection();
                var geometryCollection2 = TestData.GeometryCollection();
                Assert.AreEqual(geometryCollection1, geometryCollection2, "Values must be equal for GeometryCollection with same values");
        }

        [TestMethod]
        public void SpatialEmptyTypeEquality()
        {
            GeographyPoint emptyGeographyPoint1 = TestData.PointGEmpty();
            GeographyPoint emptyGeographyPoint2 = TestData.PointGEmpty();
            GeographyPoint point = TestData.PointG();

            Assert.IsFalse(Geography.Equals(emptyGeographyPoint1, point), "empty points must not be equal to non empty points");
            Assert.IsFalse(Geography.Equals(point, emptyGeographyPoint1), "empty points must not be equal to non empty points");
            Assert.IsTrue(Geography.Equals(emptyGeographyPoint1, emptyGeographyPoint2), "empty points must be equal");

            GeometryPoint emptyGeometryPoint1 = TestData.PointMEmpty();
            GeometryPoint emptyGeometryPoint2 = TestData.PointMEmpty();
            GeometryPoint GeometryPoint = TestData.PointM();

            Assert.IsFalse(Geometry.Equals(emptyGeometryPoint1, GeometryPoint), "empty points must not be equal to non empty points");
            Assert.IsFalse(Geometry.Equals(GeometryPoint, emptyGeometryPoint1), "empty points must not be equal to non empty points");
            Assert.IsTrue(Geometry.Equals(emptyGeometryPoint1, emptyGeometryPoint2), "empty points must be equal");
        }

        [TestMethod]
        public void SpatialEmptyMixedTypeTests()
        {
            CoordinateSystem crs = CoordinateSystem.DefaultGeography;
            Assert.AreNotEqual(TestData.PointMEmpty(crs), TestData.PointGEmpty(crs), "Different types should not be equal");
            Assert.AreNotEqual(TestData.LineStringGEmpty(crs), TestData.PointGEmpty(crs), "Different types should not be equal");
            Assert.AreNotEqual(TestData.LineStringMEmpty(crs), TestData.PointMEmpty(crs), "Different types should not be equal");
        }

        [TestMethod]
        public void SpatialTypeMismatchEquality()
        {
            Assert.IsFalse(Geography.Equals(GetGeographyType<GeographyPoint>(), GetGeographyType<GeographyCollection>()), "geography point equality must fail when types are different");
            Assert.IsFalse(Geography.Equals(GetGeographyType<GeographyLineString>(), GetGeographyType<GeographyCollection>()), "geography line string equality must fail when types are different");
            Assert.IsFalse(Geography.Equals(GetGeographyType<GeographyMultiLineString>(), GetGeographyType<GeographyCollection>()), "geography multiline string equality must fail when types are different");
            Assert.IsFalse(Geography.Equals(GetGeographyType<GeographyMultiPoint>(), GetGeographyType<GeographyCollection>()), "geography multipoint equality must fail when types are different");
            Assert.IsFalse(Geography.Equals(GetGeographyType<GeographyPolygon>(), GetGeographyType<GeographyCollection>()), "geography polygon equality must fail when types are different");
            Assert.IsFalse(Geography.Equals(GetGeographyType<GeographyMultiPolygon>(), GetGeographyType<GeographyCollection>()), "geography multipolygon equality must fail when types are different");
            Assert.IsFalse(Geography.Equals(GetGeographyType<GeographyCollection>(), GetGeographyType<GeographyMultiPolygon>()), "geography collection equality must fail when types are different");
            Assert.IsFalse(Geography.Equals(GetGeographyType<GeographyFullGlobe>(), GetGeographyType<GeographyMultiPolygon>()), "full globe equality must fail when types are different");

            Assert.IsFalse(Geometry.Equals(GetGeometryType<GeometryPoint>(), GetGeometryType<GeometryCollection>()), "geometry point equality must fail when types are different");
            Assert.IsFalse(Geometry.Equals(GetGeometryType<GeometryLineString>(), GetGeometryType<GeometryCollection>()), "geometry line string equality must fail when types are different");
            Assert.IsFalse(Geometry.Equals(GetGeometryType<GeometryMultiLineString>(), GetGeometryType<GeometryCollection>()), "geometry multiline string equality must fail when types are different");
            Assert.IsFalse(Geometry.Equals(GetGeometryType<GeometryMultiPoint>(), GetGeometryType<GeometryCollection>()), "geometry multipoint equality must fail when types are different");
            Assert.IsFalse(Geometry.Equals(GetGeometryType<GeometryPolygon>(), GetGeometryType<GeometryCollection>()), "geometry polygon equality must fail when types are different");
            Assert.IsFalse(Geometry.Equals(GetGeometryType<GeometryMultiPolygon>(), GetGeometryType<GeometryCollection>()), "geometry multipolygon equality must fail when types are different");
            Assert.IsFalse(Geometry.Equals(GetGeometryType<GeometryCollection>(), GetGeometryType<GeometryMultiPolygon>()), "geometry collection equality must fail when types are different");
        }
        
        private static T GetGeographyType<T>() where T : Geography
        {
            if (typeof(GeographyPoint).IsAssignableFrom(typeof(T)))
            {
                return (T)geographyTypes[(int)GeographyTypeKind.Point];
            }
            if (typeof(GeographyLineString).IsAssignableFrom(typeof(T)))
            {
                return (T)geographyTypes[(int)GeographyTypeKind.LineString];
            }
            if (typeof(GeographyMultiLineString).IsAssignableFrom(typeof(T)))
            {
                return (T)geographyTypes[(int)GeographyTypeKind.MultiLineString];
            }
            if (typeof(GeographyMultiPoint).IsAssignableFrom(typeof(T)))
            {
                return (T)geographyTypes[(int)GeographyTypeKind.MultiPoint];
            }
            if (typeof(GeographyMultiPolygon).IsAssignableFrom(typeof(T)))
            {
                return (T)geographyTypes[(int)GeographyTypeKind.MultiPolygon];
            }
            if (typeof(GeographyPolygon).IsAssignableFrom(typeof(T)))
            {
                return (T)geographyTypes[(int)GeographyTypeKind.Polygon];
            }
            if (typeof(GeographyCollection).IsAssignableFrom(typeof(T)))
            {
                return (T)geographyTypes[(int)GeographyTypeKind.Collection];
            }
            if (typeof(GeographyFullGlobe).IsAssignableFrom(typeof(T)))
            {
                return (T)geographyTypes[(int)GeographyTypeKind.FullGlobe];
            }

            Assert.Fail(String.Format("Invalid Geography type encountered - {0}", typeof(T).FullName));
            return null;
        }

        private static T GetGeometryType<T>() where T : Geometry
        {
            if (typeof(GeometryPoint).IsAssignableFrom(typeof(T)))
            {
                return (T)geometryTypes[(int)GeometryTypeKind.Point];
            }
            if (typeof(GeometryLineString).IsAssignableFrom(typeof(T)))
            {
                return (T)geometryTypes[(int)GeometryTypeKind.LineString];
            }
            if (typeof(GeometryMultiLineString).IsAssignableFrom(typeof(T)))
            {
                return (T)geometryTypes[(int)GeometryTypeKind.MultiLineString];
            }
            if (typeof(GeometryMultiPoint).IsAssignableFrom(typeof(T)))
            {
                return (T)geometryTypes[(int)GeometryTypeKind.MultiPoint];
            }
            if (typeof(GeometryMultiPolygon).IsAssignableFrom(typeof(T)))
            {
                return (T)geometryTypes[(int)GeometryTypeKind.MultiPolygon];
            }
            if (typeof(GeometryPolygon).IsAssignableFrom(typeof(T)))
            {
                return (T)geometryTypes[(int)GeometryTypeKind.Polygon];
            }
            if (typeof(GeometryCollection).IsAssignableFrom(typeof(T)))
            {
                return (T)geometryTypes[(int)GeometryTypeKind.Collection];
            }

            Assert.Fail(String.Format("Invalid Geometry type encountered - {0}", typeof(T).FullName));
            return null;
        }
    }
}

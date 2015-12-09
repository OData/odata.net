//---------------------------------------------------------------------
// <copyright file="SpatialEqualityTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.Spatial.Tests;
using Xunit;

namespace Microsoft.Spatial.Tests.ScenarioTests
{
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

        public SpatialEqualityTests()
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

        [Fact]
        public void GetHashCodeTests()
        {
            var gp1 = TestData.PointG();
            var gp2 = TestData.PointG();
            Assert.True(gp1.GetHashCode() == gp2.GetHashCode(), "HashCode must be equal for points with same values");

            var fullGlobe1 = TestData.FullGlobe();
            var fullGlobe2 = TestData.FullGlobe();
            var fullGlobe3 = TestData.FullGlobe(CoordinateSystem.Geography(5555));
            Assert.True(fullGlobe1.GetHashCode() == fullGlobe2.GetHashCode(), "HashCode must be equal for GeographyCollection with same values");
            Assert.True(fullGlobe1.GetHashCode() != fullGlobe3.GetHashCode(), "HashCode must be equal for GeographyCollection with same values");

            var geometryPoint1 = TestData.PointM();
            var geometryPoint2 = TestData.PointM();
            Assert.True(geometryPoint1.GetHashCode() == geometryPoint2.GetHashCode(), "HashCode must be equal for points with same values");

            var lineString1 = TestData.LineStringG();
            var lineString2 = TestData.LineStringG();
            Assert.True(lineString1.GetHashCode() == lineString2.GetHashCode(), "HashCode must be equal for line string with same values");

            var multiLineString1 = TestData.MultiLineStringG();
            var multiLineString2 = TestData.MultiLineStringG();
            Assert.True(multiLineString1.GetHashCode() == multiLineString2.GetHashCode(), "HashCode must be equal for multiLineString with same values");

            var multiPointImplementation1 = TestData.MultiPointG();
            var multiPointImplementation2 = TestData.MultiPointG();
            Assert.True(multiPointImplementation1.GetHashCode() == multiPointImplementation2.GetHashCode(), "HashCode must be equal for multiPoint with same values");

            var polygon1 = TestData.PolygonG();
            var polygon2 = TestData.PolygonG();
            Assert.True(polygon1.GetHashCode() == polygon2.GetHashCode(), "HashCode must be equal for polygon with same values");

            var multiPolygon1 = TestData.MultiPolygonG();
            var multiPolygon2 = TestData.MultiPolygonG();
            Assert.True(multiPolygon1.GetHashCode() == multiPolygon2.GetHashCode(), "HashCode must be equal for multiPolygon with same values");

            var geographyCollection1 = TestData.GeographyCollection();
            var geographyCollection2 = TestData.GeographyCollection();
            Assert.True(geographyCollection1.GetHashCode() == geographyCollection2.GetHashCode(), "HashCode must be equal for GeographyCollection with same values");

            var geometryLineString1 = TestData.LineStringM();
            var geometryLineString2 = TestData.LineStringM();
            Assert.True(geometryLineString1.GetHashCode() == geometryLineString2.GetHashCode(), "HashCode must be equal for line string with same values");

            var geometrymultiLineString1 = TestData.MultiLineStringM();
            var geometrymultiLineString2 = TestData.MultiLineStringM();
            Assert.True(geometrymultiLineString1.GetHashCode() == geometrymultiLineString2.GetHashCode(), "HashCode must be equal for multiLineString with same values");

            var geometrymultiPointImplementation1 = TestData.MultiPointM();
            var geometrymultiPointImplementation2 = TestData.MultiPointM();
            Assert.True(geometrymultiPointImplementation1.GetHashCode() == geometrymultiPointImplementation2.GetHashCode(), "HashCode must be equal for multiPoint with same values");

            var geometryPolygon1 = TestData.PolygonM();
            var geometryPolygon2 = TestData.PolygonM();
            Assert.True(geometryPolygon1.GetHashCode() == geometryPolygon2.GetHashCode(), "HashCode must be equal for polygon with same values");

            var geometryMultiPolygon1 = TestData.MultiPolygonM();
            var geometryMultiPolygon2 = TestData.MultiPolygonM();
            Assert.True(geometryMultiPolygon1.GetHashCode() == geometryMultiPolygon2.GetHashCode(), "HashCode must be equal for MultiPolygon with same values");

            var geometryCollection1 = TestData.GeometryCollection();
            var geometryCollection2 = TestData.GeometryCollection();
            Assert.True(geometryCollection1.GetHashCode() == geometryCollection2.GetHashCode(), "HashCode must be equal for GeometryCollection with same values");
        }

        [Fact]
        public void GetHashCodeTestsForEmptyTypes()
        {
            var emptyPointG = TestData.PointGEmpty();
            var samePointG = TestData.PointGEmpty();
            var differentPointG = TestData.PointGEmpty(CoordinateSystem.Geography(5555));
            Assert.Equal(emptyPointG.GetHashCode(), samePointG.GetHashCode());
            Assert.NotEqual(emptyPointG.GetHashCode(), differentPointG.GetHashCode());

            var emptyPointM = TestData.PointMEmpty();
            var samePointM = TestData.PointMEmpty();
            var differentPointM = TestData.PointMEmpty(CoordinateSystem.Geometry(5555));
            Assert.Equal(emptyPointM.GetHashCode(), samePointM.GetHashCode());
            Assert.NotEqual(emptyPointM.GetHashCode(), differentPointM.GetHashCode());
        }

        [Fact]
        public void RepresentationEquality_EqualsTests()
        {
            var gp1 = TestData.PointG();
            var gp2 = TestData.PointG();
            Assert.True(gp1.Equals(gp2), "Values must be equal for points with same values");

            var fullGlobe1 = TestData.FullGlobe();
            var fullGlobe2 = TestData.FullGlobe();
            var fullGlobe3 = TestData.FullGlobe(CoordinateSystem.Geography(5555));
            Assert.True(fullGlobe1.Equals(fullGlobe2), "Values must be equal for globes in same coordinate system");
            Assert.False(fullGlobe1.Equals(fullGlobe3), "Values must be non-equal if they are in different coordinate systems");

            var geometryPoint1 = TestData.PointM();
            var geometryPoint2 = TestData.PointM();
            Assert.True(geometryPoint1.Equals(geometryPoint2), "Values must be equal for points with same values");

            var lineString1 = TestData.LineStringG();
            var lineString2 = TestData.LineStringG();
            Assert.True(lineString1.Equals(lineString2), "Values must be equal for line string with same values");

            var multiLineString1 = TestData.MultiLineStringG();
            var multiLineString2 = TestData.MultiLineStringG();
            Assert.True(multiLineString1.Equals(multiLineString2), "Values must be equal for multiLineString with same values");

            var multiPointImplementation1 = TestData.MultiPointG();
            var multiPointImplementation2 = TestData.MultiPointG();
            Assert.True(multiPointImplementation1.Equals(multiPointImplementation2), "Values must be equal for multiPoint with same values");

            var polygon1 = TestData.PolygonG();
            var polygon2 = TestData.PolygonG();
            Assert.True(polygon1.Equals(polygon2), "Values must be equal for polygon with same values");

            var multiPolygon1 = TestData.MultiPolygonG();
            var multiPolygon2 = TestData.MultiPolygonG();
            Assert.True(multiPolygon1.Equals(multiPolygon2), "Values must be equal for multiPolygon with same values");

            var geographyCollection1 = TestData.GeographyCollection();
            var geographyCollection2 = TestData.GeographyCollection();
            Assert.True(geographyCollection1.Equals(geographyCollection2), "Values must be equal for GeographyCollection with same values");

            var geometryLineString1 = TestData.LineStringM();
            var geometryLineString2 = TestData.LineStringM();
            Assert.True(geometryLineString1.Equals(geometryLineString2), "Values must be equal for line string with same values");

            var geometryMultiLineString1 = TestData.MultiLineStringM();
            var geometryMultiLineString2 = TestData.MultiLineStringM();
            Assert.True(geometryMultiLineString1.Equals(geometryMultiLineString2), "Values must be equal for multiLineString with same values");

            var geometryMultiPointImplementation1 = TestData.MultiPointM();
            var geometryMultiPointImplementation2 = TestData.MultiPointM();
            Assert.True(geometryMultiPointImplementation1.Equals(geometryMultiPointImplementation2), "Values must be equal for multiPoint with same values");

            var geometryPolygon1 = TestData.PolygonM();
            var geometryPolygon2 = TestData.PolygonM();
            Assert.True(geometryPolygon1.Equals(geometryPolygon2), "Values must be equal for polygon with same values");

            var geometryMultiPolygon1 = TestData.MultiPolygonM();
            var geometryMultiPolygon2 = TestData.MultiPolygonM();
            Assert.True(geometryMultiPolygon1.Equals(geometryMultiPolygon2), "Values must be equal for MultiPolygon with same values");

            var geometryCollection1 = TestData.GeometryCollection();
            var geometryCollection2 = TestData.GeometryCollection();
            Assert.True(geometryCollection1.Equals(geometryCollection2), "Values must be equal for GeometryCollection with same values");
        }

        [Fact]
        public void SpatialEmptyTypeEquality()
        {
            GeographyPoint emptyGeographyPoint1 = TestData.PointGEmpty();
            GeographyPoint emptyGeographyPoint2 = TestData.PointGEmpty();
            GeographyPoint point = TestData.PointG();

            Assert.False(Geography.Equals(emptyGeographyPoint1, point), "empty points must not be equal to non empty points");
            Assert.False(Geography.Equals(point, emptyGeographyPoint1), "empty points must not be equal to non empty points");
            Assert.True(Geography.Equals(emptyGeographyPoint1, emptyGeographyPoint2), "empty points must be equal");

            GeometryPoint emptyGeometryPoint1 = TestData.PointMEmpty();
            GeometryPoint emptyGeometryPoint2 = TestData.PointMEmpty();
            GeometryPoint GeometryPoint = TestData.PointM();

            Assert.False(Geometry.Equals(emptyGeometryPoint1, GeometryPoint), "empty points must not be equal to non empty points");
            Assert.False(Geometry.Equals(GeometryPoint, emptyGeometryPoint1), "empty points must not be equal to non empty points");
            Assert.True(Geometry.Equals(emptyGeometryPoint1, emptyGeometryPoint2), "empty points must be equal");
        }

        [Fact]
        public void SpatialEmptyMixedTypeTests()
        {
            CoordinateSystem crs = CoordinateSystem.DefaultGeography;
            Assert.True(!TestData.PointMEmpty(crs).Equals(TestData.PointGEmpty(crs)), "Different types should not be equal");
            Assert.True(!TestData.LineStringGEmpty(crs).Equals(TestData.PointGEmpty(crs)), "Different types should not be equal");
            Assert.True(!TestData.LineStringMEmpty(crs).Equals(TestData.PointMEmpty(crs)), "Different types should not be equal");
        }

        [Fact]
        public void SpatialTypeMismatchEquality()
        {
            Assert.False(Geography.Equals(GetGeographyType<GeographyPoint>(), GetGeographyType<GeographyCollection>()), "geography point equality must fail when types are different");
            Assert.False(Geography.Equals(GetGeographyType<GeographyLineString>(), GetGeographyType<GeographyCollection>()), "geography line string equality must fail when types are different");
            Assert.False(Geography.Equals(GetGeographyType<GeographyMultiLineString>(), GetGeographyType<GeographyCollection>()), "geography multiline string equality must fail when types are different");
            Assert.False(Geography.Equals(GetGeographyType<GeographyMultiPoint>(), GetGeographyType<GeographyCollection>()), "geography multipoint equality must fail when types are different");
            Assert.False(Geography.Equals(GetGeographyType<GeographyPolygon>(), GetGeographyType<GeographyCollection>()), "geography polygon equality must fail when types are different");
            Assert.False(Geography.Equals(GetGeographyType<GeographyMultiPolygon>(), GetGeographyType<GeographyCollection>()), "geography multipolygon equality must fail when types are different");
            Assert.False(Geography.Equals(GetGeographyType<GeographyCollection>(), GetGeographyType<GeographyMultiPolygon>()), "geography collection equality must fail when types are different");
            Assert.False(Geography.Equals(GetGeographyType<GeographyFullGlobe>(), GetGeographyType<GeographyMultiPolygon>()), "full globe equality must fail when types are different");

            Assert.False(Geometry.Equals(GetGeometryType<GeometryPoint>(), GetGeometryType<GeometryCollection>()), "geometry point equality must fail when types are different");
            Assert.False(Geometry.Equals(GetGeometryType<GeometryLineString>(), GetGeometryType<GeometryCollection>()), "geometry line string equality must fail when types are different");
            Assert.False(Geometry.Equals(GetGeometryType<GeometryMultiLineString>(), GetGeometryType<GeometryCollection>()), "geometry multiline string equality must fail when types are different");
            Assert.False(Geometry.Equals(GetGeometryType<GeometryMultiPoint>(), GetGeometryType<GeometryCollection>()), "geometry multipoint equality must fail when types are different");
            Assert.False(Geometry.Equals(GetGeometryType<GeometryPolygon>(), GetGeometryType<GeometryCollection>()), "geometry polygon equality must fail when types are different");
            Assert.False(Geometry.Equals(GetGeometryType<GeometryMultiPolygon>(), GetGeometryType<GeometryCollection>()), "geometry multipolygon equality must fail when types are different");
            Assert.False(Geometry.Equals(GetGeometryType<GeometryCollection>(), GetGeometryType<GeometryMultiPolygon>()), "geometry collection equality must fail when types are different");
        }

        [Fact]
        public void DefaultEqualityTest()
        {
            // no impl for GeographyCurve

            DefaultEqualityTest<GeographyFullGlobe>(() => TestData.FullGlobe());
            DefaultEqualityTest(() => GeographyFactory.Collection().Point(-19.99, -12.0).Build());
            DefaultEqualityTest(() => GeographyFactory.LineString(33.1, -110.0).LineTo(35.97, -110).Build());
            DefaultEqualityTest(() => GeographyFactory.MultiLineString().LineString(10.2, 11.2).LineTo(11.9, 11.6).LineString(16.2, 17.2).LineTo(18.9, 19.6).Build());
            DefaultEqualityTest(() => GeographyFactory.MultiPoint().Point(10.2, 11.2).Point(11.9, 11.6).Build());
            DefaultEqualityTest(() => GeographyFactory.MultiPolygon().Polygon().Ring(10.2, 11.2).LineTo(11.9, 11.6).LineTo(11.45, 87.75).Ring(16.2, 17.2).LineTo(18.9, 19.6).LineTo(11.45, 87.75).Build());
            DefaultEqualityTest(() => GeographyFactory.Point(32.0, -100.0).Build());
            DefaultEqualityTest(() => GeographyFactory.Polygon().Ring(33.1, -110.0).LineTo(35.97, -110.15).LineTo(11.45, 87.75).Ring(35.97, -110).LineTo(36.97, -110.15).LineTo(45.23, 23.18).Build());

            // no impl for GeographySurface

            GeographyEqualityTestIncludingBaseObviousStuff(() => GeographyFactory.Point(32.0, -100.0).Build(), () => GeographyFactory.Point().Build());

            // no impl for GeometryCurve

            DefaultEqualityTest(() => GeometryFactory.Collection().Point(-19.99, -12.0).Build());
            DefaultEqualityTest(() => GeometryFactory.LineString(33.1, -11.5).LineTo(35.97, -11).Build());
            DefaultEqualityTest(() => GeometryFactory.MultiLineString().LineString(10.2, 11.2).LineTo(11.9, 11.6).LineString(16.2, 17.2).LineTo(18.9, 19.6).Build());
            DefaultEqualityTest(() => GeometryFactory.MultiPoint().Point(10.2, 11.2).Point(11.9, 11.6).Build());
            DefaultEqualityTest(() => GeometryFactory.MultiPolygon().Polygon().Ring(10.2, 11.2).LineTo(11.9, 11.6).LineTo(11.45, 87.75).Ring(16.2, 17.2).LineTo(18.9, 19.6).LineTo(11.45, 87.75).Build());
            DefaultEqualityTest(() => GeometryFactory.Point(32.0, -10.0).Build());
            DefaultEqualityTest(() => GeometryFactory.Polygon().Ring(33.1, -13.6).LineTo(35.97, -11.15).LineTo(11.45, 87.75).Ring(35.97, -11).LineTo(36.97, -11.15).LineTo(45.23, 23.18).Build());

            // no impl GeometrySurface

            GeometryEqualityTestIncludingBaseObviousStuff(() => GeometryFactory.Point(32.0, -10.0).Build(), () => GeometryFactory.Point().Build());
        }

        // I tried to make this generic and call it for both the Geometry, and Geography base
        // classes, but then it wouldn't call the correct == and != operators, kept calling the one on Object.
        private static void GeographyEqualityTestIncludingBaseObviousStuff(Func<Geography> create, Func<Geography> createEmpty)
        {
            Geography instance = create();
            Assert.False(instance.Equals(null));
            Assert.True(instance.Equals(instance));

            var instance2 = create();
            Assert.True(instance.Equals(instance2));

            var empty = createEmpty();
            Assert.False(instance.Equals(empty));
            Assert.False(empty.Equals(instance));
            var empty2 = createEmpty();
            Assert.True(empty.Equals(empty2));

            Assert.NotEqual(0, instance.GetHashCode());
        }

        // I tried to make this generic and call it for both the Geometry, and Geography base
        // classes, but then it wouldn't call the correct == and != operators, kept calling the one on Object.
        private static void GeometryEqualityTestIncludingBaseObviousStuff(Func<Geometry> create, Func<Geometry> createEmpty)
        {
            Geometry instance = create();
            Assert.False(instance.Equals(null));
            Assert.True(instance.Equals(instance));

            var instance2 = create();
            Assert.True(instance.Equals(instance2));

            var empty = createEmpty();
            Assert.False(instance.Equals(empty));
            Assert.False(empty.Equals(instance));
            var empty2 = createEmpty();
            Assert.True(empty.Equals(empty2));

            Assert.NotEqual(0, instance.GetHashCode());
        }

        private static void DefaultEqualityTest<T>(Func<T> createT) where T : class
        {
            var instance = createT();
            var instance2 = createT();

            Assert.True(instance.Equals(instance2));
            Assert.NotEqual(0, instance.GetHashCode());
            Assert.Equal(instance.GetHashCode(), instance2.GetHashCode());
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

            Assert.True(false, String.Format("Invalid Geography type encountered - {0}", typeof(T).FullName));
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

            Assert.True(false, String.Format("Invalid Geometry type encountered - {0}", typeof(T).FullName));
            return null;
        }
    }
}

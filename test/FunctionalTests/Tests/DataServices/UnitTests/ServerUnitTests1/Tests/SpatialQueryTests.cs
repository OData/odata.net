//---------------------------------------------------------------------
// <copyright file="SpatialQueryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.Test.Astoria;
using System.Data.Test.Astoria.Util;
using Microsoft.Spatial;
using AstoriaUnitTests.Stubs.DataServiceProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AstoriaUnitTests.Tests
{
    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/868
    [TestClass]
    public class SpatialQueryTests
    {
        private class UnitTestSpatialOperationImpl : SpatialOperations
        {
            private class UnitTestGeographyDistanceCalculator : GeographyPipeline
            {
                public double sum = 0;
                public int sign = 1;

                public override void BeginGeography(SpatialType type)
                {
                    sign *= -1;
                }

                public override void BeginFigure(GeographyPosition position)
                {
                    this.sum += sign * position.Latitude;
                    this.sum += sign * position.Longitude;
                }

                public override void LineTo(GeographyPosition position)
                {
                    this.sum += sign * position.Latitude;
                    this.sum += sign * position.Longitude;
                }

                public override void EndFigure()
                {
                }

                public override void EndGeography()
                {
                }

                public override void SetCoordinateSystem(CoordinateSystem coordinateSystem)
                {
                }

                public override void Reset()
                {
                    this.sign = 1;
                    this.sum = 0;
                }
            }

            private class UnitTestGeometryDistanceCalculator : GeometryPipeline
            {
                public double sum = 0;
                public int sign = 1;

                public override void Reset()
                {
                    this.sign = 1;
                    this.sum = 0;
                }

                public override void BeginGeometry(SpatialType type)
                {
                    this.sign *= -1;
                }

                public override void BeginFigure(GeometryPosition position)
                {
                    sum += this.sign * (position.X + position.Y);
                }

                public override void LineTo(GeometryPosition position)
                {
                    sum += this.sign * (position.X + position.Y);
                }

                public override void EndGeometry()
                {
                }

                public override void EndFigure()
                {
                }

                public override void SetCoordinateSystem(CoordinateSystem coordinateSystem)
                {
                }
            }

            public override double Distance(Geography operand1, Geography operand2)
            {
                UnitTestGeographyDistanceCalculator calc = new UnitTestGeographyDistanceCalculator();
                operand1.SendTo(calc);
                operand2.SendTo(calc);
                return calc.sum;
            }

            public override double Distance(Geometry operand1, Geometry operand2)
            {
                UnitTestGeometryDistanceCalculator calc = new UnitTestGeometryDistanceCalculator();
                operand1.SendTo(calc);
                operand2.SendTo(calc);
                return calc.sum;
            }

            /// <summary>Indicates a Geography LineString's length.</summary>
            /// <returns>The operation result.</returns>
            /// <param name="operand">The Operand.</param>
            public override double Length(Geography operand)
            {
                UnitTestGeographyDistanceCalculator calc = new UnitTestGeographyDistanceCalculator();
                var tmp = ((GeographyLineString)operand).Points;
                tmp[0].SendTo(calc);
                tmp[1].SendTo(calc);
                return calc.sum;
            }

            /// <summary>Indicates the Geometry LineString's length.</summary>
            /// <returns>The operation result.</returns>
            /// <param name="operand">The Operand.</param>
            public override double Length(Geometry operand)
            {
                UnitTestGeometryDistanceCalculator calc = new UnitTestGeometryDistanceCalculator();
                var tmp = ((GeometryLineString)operand).Points;
                tmp[0].SendTo(calc);
                tmp[1].SendTo(calc);
                return calc.sum;
            }

            /// <summary>Indicates the Geometry Intersects() method.</summary>
            /// <returns>The operation result.</returns>
            /// <param name="operand1">The Operand 1, point.</param>
            /// <param name="operand2">The Operand 2, polygon.</param>
            public override bool Intersects(Geometry operand1, Geometry operand2)
            {
                var point = operand1 as GeometryPoint;
                var polygon = operand2 as GeometryPolygon;
                if ((point != null) && (polygon != null))
                {
                    // mock logic for Intersects() method 
                    return polygon.Rings.Count <= 2
                        && polygon.Rings[0].Points[0].X < 32.1;
                }

                throw new NotImplementedException();
            }

            /// <summary>Indicates a Geography Intersects() method.</summary>
            /// <returns>The operation result.</returns>
            /// <param name="operand1">The Operand 1, point.</param>
            /// <param name="operand2">The Operand 2, polygon.</param>
            public override bool Intersects(Geography operand1, Geography operand2)
            {
                var point = operand1 as GeographyPoint;
                var polygon = operand2 as GeographyPolygon;
                if ((point != null) && (polygon != null))
                {
                    // mock logic for Intersects() method
                    return polygon.Rings.Count <= 2
                        && polygon.Rings[0].Points[0].Latitude < 32.1;
                }

                throw new NotImplementedException();
            }
        }

        private DSPUnitTestServiceDefinition CreateTestService(bool openType = false)
        {
            DSPMetadata metadata = new DSPMetadata("SpatialQueryTests", "AstoriaUnitTests.Tests");
            var entityType = metadata.AddEntityType("SpatialEntity", null, null, false);
            metadata.AddKeyProperty(entityType, "ID", typeof(int));
            entityType.IsOpenType = openType;

            if (!openType)
            {
                metadata.AddPrimitiveProperty(entityType, "Geography", typeof(Geography));
                metadata.AddPrimitiveProperty(entityType, "Point", typeof(GeographyPoint));
                metadata.AddPrimitiveProperty(entityType, "Point2", typeof(GeographyPoint));
                metadata.AddPrimitiveProperty(entityType, "LineString", typeof(GeographyLineString));
                metadata.AddPrimitiveProperty(entityType, "Polygon", typeof(GeographyPolygon));
                metadata.AddPrimitiveProperty(entityType, "GeographyCollection", typeof(GeographyCollection));
                metadata.AddPrimitiveProperty(entityType, "MultiPoint", typeof(GeographyMultiPoint));
                metadata.AddPrimitiveProperty(entityType, "MultiLineString", typeof(GeographyMultiLineString));
                metadata.AddPrimitiveProperty(entityType, "MultiPolygon", typeof(GeographyMultiPolygon));

                metadata.AddPrimitiveProperty(entityType, "Geometry", typeof(Geometry));
                metadata.AddPrimitiveProperty(entityType, "GeometryPoint", typeof(GeometryPoint));
                metadata.AddPrimitiveProperty(entityType, "GeometryPoint2", typeof(GeometryPoint));
                metadata.AddPrimitiveProperty(entityType, "GeometryLineString", typeof(GeometryLineString));
                metadata.AddPrimitiveProperty(entityType, "GeometryPolygon", typeof(GeometryPolygon));
                metadata.AddPrimitiveProperty(entityType, "GeometryCollection", typeof(GeometryCollection));
                metadata.AddPrimitiveProperty(entityType, "GeometryMultiPoint", typeof(GeometryMultiPoint));
                metadata.AddPrimitiveProperty(entityType, "GeometryMultiLineString", typeof(GeometryMultiLineString));
                metadata.AddPrimitiveProperty(entityType, "GeometryMultiPolygon", typeof(GeometryMultiPolygon));
            }

            metadata.AddCollectionProperty(entityType, "CollectionOfPoints", typeof(GeographyPoint));
            metadata.AddCollectionProperty(entityType, "GeometryCollectionOfPoints", typeof(GeometryPoint));
            metadata.AddPrimitiveProperty(entityType, "GeographyNull", typeof(Geography));
            metadata.AddPrimitiveProperty(entityType, "GeometryNull", typeof(Geometry));

            metadata.AddResourceSet("Spatials", entityType);

            metadata.SetReadOnly();

            DSPContext context = new DSPContext();
            var set = context.GetResourceSetEntities("Spatials");

            for (int i = 0; i < 3; ++i)
            {
                DSPResource spatialEntity = new DSPResource(entityType);
                spatialEntity.SetValue("ID", i);
                spatialEntity.SetValue("Geography", GeographyFactory.Point(32.0 - i, -100.0).Build());
                spatialEntity.SetValue("Point", GeographyFactory.Point(33.1 - i, -110.0).Build());
                spatialEntity.SetValue("Point2", GeographyFactory.Point(32.1 - i, -110.0).Build());
                spatialEntity.SetValue("LineString", GeographyFactory.LineString(33.1 - i, -110.0).LineTo(35.97 - i, -110).Build());
                spatialEntity.SetValue("Polygon", GeographyFactory.Polygon().Ring(33.1 - i, -110.0).LineTo(35.97 - i, -110.15).LineTo(11.45 - i, 87.75).Ring(35.97 - i, -110).LineTo(36.97 - i, -110.15).LineTo(45.23 - i, 23.18).Build());
                spatialEntity.SetValue("GeographyCollection", GeographyFactory.Collection().Point(-19.99 - i, -12.0).Build());
                spatialEntity.SetValue("MultiPoint", GeographyFactory.MultiPoint().Point(10.2 - i, 11.2).Point(11.9 - i, 11.6).Build());
                spatialEntity.SetValue("MultiLineString", GeographyFactory.MultiLineString().LineString(10.2 - i, 11.2).LineTo(11.9 - i, 11.6).LineString(16.2 - i, 17.2).LineTo(18.9 - i, 19.6).Build());
                spatialEntity.SetValue("MultiPolygon", GeographyFactory.MultiPolygon().Polygon().Ring(10.2 - i, 11.2).LineTo(11.9 - i, 11.6).LineTo(11.45 - i, 87.75).Ring(16.2 - i, 17.2).LineTo(18.9 - i, 19.6).LineTo(11.45 - i, 87.75).Build());
                spatialEntity.SetValue("CollectionOfPoints", new List<GeographyPoint>()
                    {
                        GeographyFactory.Point(10.2, 99.5),
                        GeographyFactory.Point(11.2, 100.5)
                    });

                spatialEntity.SetValue("Geometry", GeometryFactory.Point(32.0 - i, -10.0).Build());
                spatialEntity.SetValue("GeometryPoint", GeometryFactory.Point(33.1 - i, -11.0).Build());
                spatialEntity.SetValue("GeometryPoint2", GeometryFactory.Point(32.1 - i, -11.0).Build());
                spatialEntity.SetValue("GeometryLineString", GeometryFactory.LineString(33.1 - i, -11.5).LineTo(35.97 - i, -11).Build());
                spatialEntity.SetValue("GeometryPolygon", GeometryFactory.Polygon().Ring(33.1 - i, -13.6).LineTo(35.97 - i, -11.15).LineTo(11.45 - i, 87.75).Ring(35.97 - i, -11).LineTo(36.97 - i, -11.15).LineTo(45.23 - i, 23.18).Build());
                spatialEntity.SetValue("GeometryCollection", GeometryFactory.Collection().Point(-19.99 - i, -12.0).Build());
                spatialEntity.SetValue("GeometryMultiPoint", GeometryFactory.MultiPoint().Point(10.2 - i, 11.2).Point(11.9 - i, 11.6).Build());
                spatialEntity.SetValue("GeometryMultiLineString", GeometryFactory.MultiLineString().LineString(10.2 - i, 11.2).LineTo(11.9 - i, 11.6).LineString(16.2 - i, 17.2).LineTo(18.9 - i, 19.6).Build());
                spatialEntity.SetValue("GeometryMultiPolygon", GeometryFactory.MultiPolygon().Polygon().Ring(10.2 - i, 11.2).LineTo(11.9 - i, 11.6).LineTo(11.45 - i, 87.75).Ring(16.2 - i, 17.2).LineTo(18.9 - i, 19.6).LineTo(11.45 - i, 87.75).Build());
                spatialEntity.SetValue("GeometryCollectionOfPoints", new List<GeometryPoint>()
                    {
                        GeometryFactory.Point(10.2, 99.5),
                        GeometryFactory.Point(11.2, 100.5)
                    });

                spatialEntity.SetValue("GeographyNull", null);
                spatialEntity.SetValue("GeometryNull", null);

                set.Add(spatialEntity);
            }

            var service = new DSPUnitTestServiceDefinition(metadata, DSPDataProviderKind.CustomProvider, context);
            service.DataServiceBehavior.AcceptSpatialLiteralsInQuery = true;
            service.Writable = true;

            return service;
        }

        #region $filter
        [Ignore] // Remove Atom
        // [TestCategory("Partition1"), TestMethod]
        public void FilterBySpatialLengthFunction()
        {
            // FilterBySpatialProperty_LineStringLiteral
            VerifySpatialFilterResult("geo.length(geography'LineString(-110.0 33.1,-110.0 33.1)') eq 0.0", 3);
            VerifySpatialFilterResult("geo.length(geography'LineString(-110.0 33.1,-110.0 33.1)') gt 0.0", 0);
            VerifySpatialFilterResult("geo.length(geometry'LineString(-110.0 33.1,-110.0 33.1)') eq 0.0", 3);
            VerifySpatialFilterResult("geo.length(geometry'LineString(-110.0 33.1,-110.0 33.1)') gt 0.0", 0);

            // FilterBySpatialProperty_LineStringProperty
            VerifySpatialFilterResult("geo.length(LineString) eq 2.8700000000000045", 3); // entity property name is 'LineString'
            VerifySpatialFilterResult("geo.length(LineString) gt 2.8700000000000045", 0); // entity property name is 'LineString'
            VerifySpatialFilterResult("geo.length(GeometryLineString) eq 3.3699999999999974", 3); // entity property name is 'GeometryLineString'
            VerifySpatialFilterResult("geo.length(GeometryLineString) gt 3.3699999999999974", 0); // entity property name is 'GeometryLineString'
        }
        [Ignore] // Remove Atom
        // [TestCategory("Partition1"), TestMethod]
        public void FilterBySpatialIntersectsFunction()
        {
            // Geography:
            // TwoLiterals
            VerifySpatialFilterResult("geo.intersects(geography'POINT(-110.0 33.1)', geography'Polygon((-1.66 1.88,2.66 2.88,3.66 3.88,-1.66 1.88))') eq true", 3);
            VerifySpatialFilterResult("geo.intersects(geography'POINT(-110.0 33.1)', geography'Polygon((-1.66 1.88,2.66 2.88,3.66 3.88,-361.66 1.88),(-11.66 1.88,2.66 22.88,33.66 3.88,-11.66 1.88),(-11.66 1.88,2.66 22.88,33.66 3.88,-11.66 1.88))') eq true", 0);

            // LiteralSecond
            VerifySpatialFilterResult("geo.intersects(Point, geography'Polygon((-1.66 1.88,2.66 2.88,3.66 3.88,-1.66 1.88))') eq true", 3);
            VerifySpatialFilterResult("geo.intersects(Point, geography'Polygon((-1.66 1.88,2.66 2.88,3.66 3.88,-361.66 1.88),(-11.66 1.88,2.66 22.88,33.66 3.88,-11.66 1.88),(-11.66 1.88,2.66 22.88,33.66 3.88,-11.66 1.88))') eq true", 0);

            // LiteralFirst
            VerifySpatialFilterResult("geo.intersects(geography'POINT(-110.0 33.1)', Polygon) eq true", 1);

            // TwoProperties
            VerifySpatialFilterResult("geo.intersects(Point, Polygon)", 1);

            // Geometry:
            // TwoLiterals
            VerifySpatialFilterResult("geo.intersects(geometry'POINT(-110.0 33.1)', geometry'Polygon((-1.66 1.88,2.66 2.88,3.66 3.88,-1.66 1.88))') eq true", 3);
            VerifySpatialFilterResult("geo.intersects(geometry'POINT(-110.0 33.1)', geometry'Polygon((-1.66 1.88,2.66 2.88,3.66 3.88,-1.66 1.88),(-11.66 1.88,2.66 22.88,33.66 3.88,-11.66 1.88),(-11.66 1.88,2.66 22.88,33.66 3.88,-11.66 1.88))') eq true", 0);

            // LiteralSecond
            VerifySpatialFilterResult("geo.intersects(GeometryPoint, geometry'Polygon((-1.66 1.88,2.66 2.88,3.66 3.88,-1.66 1.88))') eq true", 3);
            VerifySpatialFilterResult("geo.intersects(GeometryPoint, geometry'Polygon((-1.66 1.88,2.66 2.88,3.66 3.88,-1.66 1.88),(-11.66 1.88,2.66 22.88,33.66 3.88,-11.66 1.88),(-11.66 1.88,2.66 22.88,33.66 3.88,-11.66 1.88))') eq true", 0);

            // LiteralFirst
            VerifySpatialFilterResult("geo.intersects(geometry'POINT(-110.0 33.1)', GeometryPolygon) eq true", 1);

            // TwoProperties
            VerifySpatialFilterResult("geo.intersects(GeometryPoint, GeometryPolygon)", 1);
        }
        [Ignore] // Remove Atom
        // [TestCategory("Partition1"), TestMethod]
        public void FilterBySpatialProperty()
        {
            // FilterBySpatialProperty_LiteralSecond
            VerifySpatialFilterResult("geo.distance(Point, geography'POINT(-110.0 33.1)') eq 0.0", 1);
            VerifySpatialFilterResult("geo.distance(Point, geography'POINT(-120.0 32.1)') eq 0.0", 0);
            VerifySpatialFilterResult("geo.distance(Point, geography'POINT(-100.5 32.0)') lt 100000.0", 3); // 100KM

            // FilterBySpatialProperty_LiteralFirst
            VerifySpatialFilterResult("geo.distance(geography'POINT(-110.0 33.1)', Point) eq 0.0", 1);
            VerifySpatialFilterResult("geo.distance(geography'POINT(-120.0 32.1)', Point) eq 0.0", 0);
            VerifySpatialFilterResult("geo.distance(geography'POINT(-100.5 32.0)', Point) lt 100000.0", 3); // 100KM

            // FilterBySpatialProperty_TwoProperties
            VerifySpatialFilterResult("geo.distance(Point, Point2) gt 1.0d", 0); // here should know open method returns double, and make the right a double.

            // Geometry:
            // FilterBySpatialProperty_LiteralSecond
            VerifySpatialFilterResult("geo.distance(GeometryPoint, geometry'POINT(33.1 -11.0)') eq 0.0", 1);
            VerifySpatialFilterResult("geo.distance(GeometryPoint, geometry'POINT(32.1 -120.0)') eq 0.0", 0);
            VerifySpatialFilterResult("geo.distance(GeometryPoint, geometry'POINT(32.0 -10.5)') lt 100000.0", 3); // 100KM

            // FilterBySpatialProperty_LiteralFirst
            VerifySpatialFilterResult("geo.distance(geometry'POINT(33.1 -11.0)', GeometryPoint) eq 0.0", 1);
            VerifySpatialFilterResult("geo.distance(geometry'POINT(32.1 -120.0)', GeometryPoint) eq 0.0", 0);
            VerifySpatialFilterResult("geo.distance(geometry'POINT(32.0 -10.5)', GeometryPoint) lt 100000.0", 3); // 100KM

            // FilterBySpatialProperty_TwoProperties
            VerifySpatialFilterResult("geo.distance(GeometryPoint, GeometryPoint2) gt 1.0d", 0); // here should know open method returns double, and make the right a double.
        }

        private void VerifySpatialFilterResult(string filterExpr, int expectResults)
        {
            SpatialImplementation.CurrentImplementation.Operations = new UnitTestSpatialOperationImpl();

            TestUtil.RunCombinations(UnitTestsUtil.BooleanValues, (useOpenType) =>
                {
                    using (var service = CreateTestService(useOpenType))
                    using (var request = service.CreateForInProcessWcf())
                    {
                        request.HttpMethod = "GET";
                        request.RequestUriString = "/Spatials?$filter=" + filterExpr;
                        request.Accept = "application/atom+xml,application/xml";
                        request.SendRequest();

                        var response = request.GetResponseStreamAsXDocument();
                        UnitTestsUtil.VerifyXPathResultCount(response, expectResults, "atom:feed/atom:entry");
                    }
                });
        }

        [TestCategory("Partition1"), TestMethod]
        public void DisallowSpatialLiteralInUri()
        {
            using (var service = CreateTestService())
            {
                service.DataServiceBehavior.AcceptSpatialLiteralsInQuery = false;
                try
                {
                    using (var request = service.CreateForInProcess())
                    {
                        request.HttpMethod = "GET";
                        request.RequestUriString = "/Spatials?$filter=geo.distance(geography'POINT(10 20)', geography'POINT(10 20)') gt 0.0";
                        Exception ex = TestUtil.RunCatching(request.SendRequest);
                        Assert.AreEqual(DataServicesResourceUtil.GetString("RequestQueryParser_SpatialNotSupported"), ex.InnerException.Message);
                    }
                }
                finally
                {
                    service.DataServiceBehavior.AcceptSpatialLiteralsInQuery = true;
                }
            }
        }

        #endregion

        #region $orderby

        [TestCategory("Partition1"), TestMethod]
        public void OrderBySpatialProperty()
        {
            using (var service = CreateTestService())
            using (var request = service.CreateForInProcess())
            {
                request.RequestUriString = "/Spatials?$orderby=Point";
                Exception ex = TestUtil.RunCatching(request.SendRequest);

                Assert.IsNotNull(ex);
                TestUtil.AssertContains(ex.InnerException.Message, "At least one object must implement IComparable.");
            }
        }
        [Ignore] // Remove Atom
        // [TestCategory("Partition1"), TestMethod]
        public void OrderByDistanceToPoint()
        {
            SpatialImplementation.CurrentImplementation.Operations = new UnitTestSpatialOperationImpl();

            TestUtil.RunCombinations(UnitTestsUtil.BooleanValues, (useOpenType) =>
                {
                    using (var service = CreateTestService(useOpenType))
                    using (var request = service.CreateForInProcessWcf())
                    {
                        request.RequestUriString = "/Spatials?$orderby=geo.distance(Point, geography'POINT(-103.0 32.0)')&$top=1";
                        request.Accept = "application/atom+xml,application/xml";
                        request.SendRequest();
                        var response = request.GetResponseStreamAsXDocument();
                        UnitTestsUtil.VerifyXPathResultCount(response, 1, "atom:feed/atom:entry/atom:content/adsm:properties[ads:ID = 0]");
                    }
                });
        }

        #endregion
        [Ignore] // Remove Atom
        // [TestCategory("Partition1"), TestMethod]
        public void QueryEntitySetWithSpatialProperty()
        {
            using (var service = CreateTestService())
            using (var request = service.CreateForInProcess())
            {
                request.RequestUriString = "/Spatials";

                TestUtil.RunCombinations(UnitTestsUtil.ResponseFormats, (format) =>
                {
                    request.Accept = format;
                    request.SendRequest();

                    Assert.AreEqual(request.ResponseStatusCode, 200, "Status code must be 200 (OK)");
                    Assert.AreEqual("4.0;", request.ResponseVersion, "Response version must be 4.0");
                });
            }
        }
        [Ignore] // Remove Atom
        // [TestCategory("Partition1"), TestMethod]
        public void QueryAndUpdateSingleEntityWithSpatialProperty()
        {
            using (TestUtil.RestoreStaticValueOnDispose(typeof(BaseTestWebRequest), "HostInterfaceType"))
            using (var service = CreateTestService())
            using (var request = service.CreateForInProcess())
            {
                BaseTestWebRequest.HostInterfaceType = typeof(Microsoft.OData.Service.IDataServiceHost2);
                request.RequestUriString = "/Spatials(1)";

                TestUtil.RunCombinations(UnitTestsUtil.ResponseFormats, (format) =>
                {
                    request.Accept = format;
                    request.SendRequest();
                    Assert.AreEqual(request.ResponseStatusCode, 200, "Status code must be 200 (OK)");
                    Assert.AreEqual("4.0;", request.ResponseVersion, "Response version must be 4.0");

                    string payload = request.GetResponseStreamAsText();

                    if (format == UnitTestsUtil.JsonLightMimeType)
                    {
                        payload = JsonPrimitiveTypesUtil.FilterJson(payload);
                    }

                    request.SetRequestStreamAsText(payload);
                    request.HttpMethod = "PATCH";
                    request.RequestContentType = format;
                    request.RequestHeaders["Prefer"] = "return=representation";
                    request.SendRequest();

                    Assert.AreEqual(request.ResponseStatusCode, 200, "Status code must be 200 (OK)");
                    Assert.AreEqual("4.0;", request.ResponseVersion, "Response version must be 4.0");
                });
            }
        }
        [Ignore] // Remove Atom
        // [TestCategory("Partition1"), TestMethod]
        public void QuerySpatialProperty()
        {
            using (var service = CreateTestService())
            using (var request = service.CreateForInProcess())
            {
                request.RequestUriString = "/Spatials(1)/Point";
                TestUtil.RunCombinations(new string[] { UnitTestsUtil.JsonLightMimeType, UnitTestsUtil.MimeApplicationXml }, (format) =>
                {
                    request.Accept = format;
                    request.SendRequest();

                    Assert.AreEqual(request.ResponseStatusCode, 200, "Status code must be 200 (OK)");
                    Assert.AreEqual("4.0;", request.ResponseVersion, "Response version must be 4.0");
                });
            }
        }

        [TestCategory("Partition1"), TestMethod]
        public void QueryWith3DSpatialLiteral()
        {
            SpatialImplementation.CurrentImplementation.Operations = new UnitTestSpatialOperationImpl();

            using (var service = CreateTestService())
            using (var request = service.CreateForInProcess())
            {
                service.DataServiceBehavior.AcceptSpatialLiteralsInQuery = true;
                // check geography, and geometry when we are able to specify each
                request.RequestUriString = "/Spatials?$filter=geo.distance(geography'POINT(10 20 30)', geography'POINT(10 20)') eq 1.5";
                Exception ex = TestUtil.RunCatching(request.SendRequest);
                Assert.IsNull(ex);
                //Assert.AreEqual(DataServicesResourceUtil.GetString("RequestQueryParser_UnrecognizedLiteral", "Edm.Geography", "geography'POINT(10 20 30)'", "13"), ex.InnerException.Message);
            }
        }

        [TestCategory("Partition1"), TestMethod]
        public void QuerySpatialPropertyValue()
        {
            using (var service = CreateTestService())
            using (var request = service.CreateForInProcess())
            {
                request.RequestUriString = "/Spatials(1)/Point/$value";
                Exception ex = TestUtil.RunCatching(request.SendRequest);

                Assert.IsNull(ex);
                // Assert.AreEqual(ODataLibResourceUtil.GetString("BadRequest_ValuesCannotBeReturnedForSpatialTypes"), ex.InnerException.Message);
            }
        }
    }
}

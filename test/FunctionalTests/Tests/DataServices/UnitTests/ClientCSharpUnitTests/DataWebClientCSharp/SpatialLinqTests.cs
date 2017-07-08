//---------------------------------------------------------------------
// <copyright file="SpatialLinqTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.OData.Client;
using System.Data.Test.Astoria;
using System.Linq;
using System.Net;
using Microsoft.Spatial;
using AstoriaUnitTests.Stubs;
using AstoriaUnitTests.Stubs.DataServiceProvider;
using AstoriaUnitTests.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AstoriaUnitTests.DataWebClientCSharp
{
    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/881
    // [TestClass]
    public class SpatialLinqTests
    {
        [Ignore] // Remove Atom
        [TestMethod]
        public void SupportedLinqQueries_BaseGeography()
        {
            // Verify URI and results for LINQ queries that are valid for all spatial types
            var testCases = new[]
            {
                new
                {
                    SpatialType = typeof(GeographyPoint),
                    DefaultValues = TestPoint.DefaultValues
                },
                new
                {
                    SpatialType = typeof(GeographyLineString),
                    DefaultValues = TestLineString.DefaultValues
                },
            };

            TestUtil.RunCombinations(testCases, (testCase) =>
            {
                TestSupportedLinqQueries<Geography>(testCase.DefaultValues, testCase.SpatialType);
            });
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void SupportedLinqQueries_GeographyPoint()
        {
            // Verify URI and results for LINQ queries that are valid only for GeographyPoint
            TestSupportedLinqQueries<GeographyPoint>(TestPoint.DefaultValues, typeof(GeographyPoint));
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void SupportedLinqQueries_GeographyLineString()
        {
            // Verify URI and results for LINQ queries that are valid only for GeographyLineString
            TestSupportedLinqQueries<GeographyLineString>(TestLineString.DefaultValues, typeof(GeographyLineString));
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void SupportedLinqQueries_GeometryPoint_Filter()
        {
            // Verify URI and results for LINQ queries that are valid only for GeometryPoint, filter only
            Func<DataServiceContext, LinqTestCase[]> getTests =
                context =>
                {
                    var allEntities = new GeometricEntity<GeometryPoint>[]
                    {
                        new GeometricEntity<GeometryPoint>()
                        {
                            ID = 1,
                            Property1 = GeometryFactory.Point(1, 2),
                            Property2 = GeometryFactory.Point(3, 4),
                        },
                    };

                    return new LinqTestCase[]
                    {
                        new LinqTestCase()
                        {
                            Query = from t in context.CreateQuery<GeometricEntity<GeometryPoint>>("TripLegs")
                                    where t.Property1.Distance(t.Property2) > 0
                                    select t,

                            ExpectedUri = "?$filter=geo.distance(Property1,Property2) gt 0.0",
                            ExpectedResults = allEntities,
                            ServerPayload = GetSampleFeedPayload(allEntities, null),
                            ExpectKeyInUri = false,
                        },
                        new LinqTestCase()
                        {
                            Query = from t in context.CreateQuery<GeometricEntity<GeometryPoint>>("TripLegs")
                                    where t.Property1.Distance(GeometryFactory.Point(5, 6)) > 0
                                    select t,

                            ExpectedUri = "?$filter=geo.distance(Property1,geometry'SRID=0;POINT (5 6)') gt 0.0",
                            ExpectedResults = allEntities,
                            ServerPayload = GetSampleFeedPayload(allEntities, null),
                            ExpectKeyInUri = false,
                        },
                        new LinqTestCase()
                        {
                            Query = from t in context.CreateQuery<GeometricEntity<GeometryPoint>>("TripLegs")
                                    where t.Property1.Distance(null) > 0
                                    select t,

                            ExpectedUri = "?$filter=geo.distance(Property1,null) gt 0.0",
                            ExpectedResults = allEntities,
                            ServerPayload = GetSampleFeedPayload(allEntities, null),
                            ExpectKeyInUri = false,
                        },
                        // the client will try to evaluate distance between constants locally and fail because no operations are registered
                        //new LinqTestCase()
                        //{
                        //    Query = from t in context.CreateQuery<GeometricEntity<GeometryPoint>>("TripLegs")
                        //            where GeometryFactory.Point(5, 6).Build().Distance(null) > 0
                        //            select t,
        
                        //    ExpectedUri = "?$filter=geo.distance(geometry'SRID=0;POINT (5 6)', null) gt 0.0",
                        //    ExpectedResults = allEntities,
                        //    ServerPayload = GetSampleFeedPayload(allEntities, null),
                        //    ExpectKeyInUri = false,
                        //},
                        new LinqTestCase()
                        {
                            Query = from t in context.CreateQuery<GeometricEntity<GeometryPoint>>("TripLegs")
                                    where ((GeometryPoint)null).Distance(t.Property1) > 0
                                    select t,

                            ExpectedUri = "?$filter=geo.distance(null,Property1) gt 0.0",
                            ExpectedResults = allEntities,
                            ServerPayload = GetSampleFeedPayload(allEntities, null),
                            ExpectKeyInUri = false,
                        },
                        // the client will try to evaluate distance between constants locally and fail because no operations are registered
                        //new LinqTestCase()
                        //{
                        //    Query = from t in context.CreateQuery<GeometricEntity<GeometryPoint>>("TripLegs")
                        //            where ((GeometryPoint)null).Distance(null) > 0
                        //            select t,
        
                        //    ExpectedUri = "?$filter=geo.distance(null, null) gt 0.0",
                        //    ExpectedResults = allEntities,
                        //    ServerPayload = GetSampleFeedPayload(allEntities, null),
                        //    ExpectKeyInUri = false,
                        //},
                    };
                };
            TestSupportedLinqQueries(getTests);
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void SupportedLinqQueries_GeometryPoint_FilterWithDistanceFromLiteralToLiteral()
        {
            // Verify URI and results for LINQ queries that are valid only for GeometryPoint, filter with literals
            Func<DataServiceContext, LinqTestCase[]> getTests =
                context =>
                {
                    var allEntities = new GeometricEntity<GeometryPoint>[]
                    {
                        new GeometricEntity<GeometryPoint>()
                        {
                            ID = 1,
                            Property1 = GeometryFactory.Point(1, 2),
                            Property2 = GeometryFactory.Point(3, 4),
                        },
                    };

                    return new LinqTestCase[]
                    {
                        new LinqTestCase()
                        {
                            Query = from t in context.CreateQuery<GeometricEntity<GeometryPoint>>("TripLegs")
                                    where GeometryFactory.Point(5, 6).Build().Distance(GeometryFactory.Point(0, 0).Build()) > 0
                                    select t,

                            ExpectedUri = "?$filter=true",
                            ExpectedResults = allEntities,
                            ServerPayload = GetSampleFeedPayload(allEntities, null),
                            ExpectKeyInUri = false,
                        },
                        new LinqTestCase()
                        {
                            Query = from t in context.CreateQuery<GeometricEntity<GeometryPoint>>("TripLegs")
                                    where GeometryFactory.Point(5, 6).Build().Distance(null) > 0
                                    select t,

                            ExpectedUri = "?$filter=false",
                            ExpectedResults = allEntities,
                            ServerPayload = GetSampleFeedPayload(allEntities, null),
                            ExpectKeyInUri = false,
                        },
                        new LinqTestCase()
                        {
                            Query = from t in context.CreateQuery<GeometricEntity<GeometryPoint>>("TripLegs")
                                    where ((GeometryPoint)null).Distance(GeometryFactory.Point(5, 6).Build()) > 0
                                    select t,

                            ExpectedUri = "?$filter=false",
                            ExpectedResults = allEntities,
                            ServerPayload = GetSampleFeedPayload(allEntities, null),
                            ExpectKeyInUri = false,
                        },
                        new LinqTestCase()
                        {
                            Query = from t in context.CreateQuery<GeometricEntity<GeometryPoint>>("TripLegs")
                                    where ((GeometryPoint)null).Distance(null) > 0
                                    select t,

                            ExpectedUri = "?$filter=false",
                            ExpectedResults = allEntities,
                            ServerPayload = GetSampleFeedPayload(allEntities, null),
                            ExpectKeyInUri = false,
                        },
                    };
                };

            SpatialOperations previousOperations = SpatialImplementation.CurrentImplementation.Operations;
            SpatialImplementation.CurrentImplementation.Operations = new DistanceOperationImplementation();
            try
            {
                TestSupportedLinqQueries(getTests);
            }
            finally
            {
                SpatialImplementation.CurrentImplementation.Operations = previousOperations;
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void SupportedLinqQueries_GeometryPoint_OrderByDistanceFromLiteralToLiteral()
        {
            // Verify URI and results for LINQ queries that are valid only for GeometryPoint, orderby with literals
            Func<DataServiceContext, LinqTestCase[]> getTests =
                context =>
                {
                    var allEntities = new GeometricEntity<GeometryPoint>[]
                    {
                        new GeometricEntity<GeometryPoint>()
                        {
                            ID = 1,
                            Property1 = GeometryFactory.Point(1, 2),
                            Property2 = GeometryFactory.Point(3, 4),
                        },
                    };

                    return new LinqTestCase[]
                    {
                        new LinqTestCase()
                        {
                            Query = from t in context.CreateQuery<GeometricEntity<GeometryPoint>>("TripLegs")
                                    orderby GeometryFactory.Point(5, 6).Build().Distance(GeometryFactory.Point(0, 0).Build())
                                    select t,

                            ExpectedUri = "?$orderby=1.0",
                            ExpectedResults = allEntities,
                            ServerPayload = GetSampleFeedPayload(allEntities, null),
                            ExpectKeyInUri = false,
                        },
                        new LinqTestCase()
                        {
                            Query = from t in context.CreateQuery<GeometricEntity<GeometryPoint>>("TripLegs")
                                    orderby GeometryFactory.Point(5, 6).Build().Distance(null)
                                    select t,

                            ExpectedUri = "?$orderby=null",
                            ExpectedResults = allEntities,
                            ServerPayload = GetSampleFeedPayload(allEntities, null),
                            ExpectKeyInUri = false,
                        },
                        new LinqTestCase()
                        {
                            Query = from t in context.CreateQuery<GeometricEntity<GeometryPoint>>("TripLegs")
                                    orderby ((GeometryPoint)null).Distance(GeometryFactory.Point(5, 6).Build())
                                    select t,

                            ExpectedUri = "?$orderby=null",
                            ExpectedResults = allEntities,
                            ServerPayload = GetSampleFeedPayload(allEntities, null),
                            ExpectKeyInUri = false,
                        },
                        new LinqTestCase()
                        {
                            Query = from t in context.CreateQuery<GeometricEntity<GeometryPoint>>("TripLegs")
                                    orderby ((GeometryPoint)null).Distance(null)
                                    select t,

                            ExpectedUri = "?$orderby=null",
                            ExpectedResults = allEntities,
                            ServerPayload = GetSampleFeedPayload(allEntities, null),
                            ExpectKeyInUri = false,
                        },
                    };
                };

            SpatialOperations previousOperations = SpatialImplementation.CurrentImplementation.Operations;
            SpatialImplementation.CurrentImplementation.Operations = new DistanceOperationImplementation();
            try
            {
                TestSupportedLinqQueries(getTests);
            }
            finally
            {
                SpatialImplementation.CurrentImplementation.Operations = previousOperations;
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void OpenSpatialProperties()
        {
            // Verify that the client round tripping works with spatial open properties
            DSPUnitTestServiceDefinition roadTripServiceDefinition = GetRoadTripServiceDefinition(
                typeof(GeographyPoint),
                TestPoint.DefaultValues,
                false, // useComplexType
                true, // useOpenType
                null);

            using (TestWebRequest request = roadTripServiceDefinition.CreateForInProcessWcf())
            {
                request.StartService();

                DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                //context.EnableAtom = true;
                //context.Format.UseAtom();

                // Query the top level set
                List<TripLeg<Geography>> results = context.CreateQuery<TripLeg<Geography>>("TripLegs").ToList();
                Assert.IsTrue(results.Count == 1, "one trip leg should get materialized");
                Assert.IsTrue(context.Entities.Count == 1, "One trip leg instance should get populated in the context");

                // Update the property value
                results[0].GeographyProperty1 = GeographyFactory.Point(22, 45);
                context.UpdateObject(results[0]);
                context.SaveChanges();

                var data = roadTripServiceDefinition.CurrentDataSource.GetResourceSetEntities("TripLegs");
                Assert.AreEqual(1, data.Count, "there should one instance of TripLeg in the set");
                GeographyPoint point = (GeographyPoint)((DSPResource)data[0]).GetOpenPropertyValue("GeographyProperty1");
                Assert.AreEqual(45, point.Longitude, "Make sure longitude value is updated");
                Assert.AreEqual(22, point.Latitude, "Make sure latitude value is updated");
            }
        }

        private class DistanceOperationImplementation : SpatialOperations
        {
            public override double Distance(Geography operand1, Geography operand2)
            {
                return 1;
            }

            public override double Distance(Geometry operand1, Geometry operand2)
            {
                return 1;
            }
        }

        private class LinqTestCase
        {
            public LinqTestCase()
            {
                ExpectKeyInUri = true;
            }

            public IQueryable Query { get; set; }
            public string ExpectedUri { get; set; }
            public bool ExpectKeyInUri { get; set; }
            public string ServerPayload { get; set; }
            public IEnumerable ExpectedResults { get; set; }
        }

        private static void TestSupportedLinqQueries(Func<DataServiceContext, LinqTestCase[]> getTests)
        {
            PlaybackServiceDefinition playbackService = new PlaybackServiceDefinition();
            using (TestWebRequest request = playbackService.CreateForInProcessWcf())
            {
                request.StartService();

                DataServiceContext context = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                //context.EnableAtom = true;
                context.MergeOption = MergeOption.NoTracking;

                var tests = getTests(context);

                for (int i = 0; i < tests.Length; i++)
                {
                    playbackService.OverridingPlayback = tests[i].ServerPayload;
                    VerifyURI(context, tests[i].Query, tests[i].ExpectedUri, tests[i].ExpectedResults, tests[i].ExpectKeyInUri);
                }
            }
        }

        private static void TestSupportedLinqQueries<T>(GeographyPropertyValues defaultValues, Type spatialTestType) where T : Geography
        {
            TestSupportedLinqQueries(context => GetSupportedLinqQueries<T>(context, defaultValues));
        }

        private static LinqTestCase[] GetSupportedLinqQueries<T>(DataServiceContext context, GeographyPropertyValues defaultValues) where T : Geography
        {
            LinqTestCase[] tests = null;

            IQueryable<TripLeg<T>> allTripLegsActual = context.CreateQuery<TripLeg<T>>("TripLegs");

            IQueryable<TripLeg<T>> allTripLegsExpected = new List<TripLeg<T>>()
                {
                    new TripLeg<T>()
                    {
                        ID = SpatialTestUtil.DefaultId,
                        GeographyProperty1 = (T)defaultValues.TripLegGeography1.AsGeography(),
                        GeographyProperty2 = (T)defaultValues.TripLegGeography2.AsGeography()
                    }
                }.AsQueryable();

            LinqTestCase[] tests_BaseGeography = GetSupportedLinqQueries_BaseGeography(allTripLegsActual, allTripLegsExpected);

            Type testSpatialType = typeof(T);
            if (testSpatialType == typeof(Geography))
            {
                tests = tests_BaseGeography;
            }
            else if (testSpatialType == typeof(GeographyPoint))
            {
                LinqTestCase[] tests_GeographyPoint = GetSupportedLinqQueries_GeographyPoint((IQueryable<TripLeg<GeographyPoint>>)allTripLegsActual, (IQueryable<TripLeg<GeographyPoint>>)allTripLegsExpected);

                tests = tests_GeographyPoint.Concat(tests_BaseGeography).ToArray();
            }
            else if (testSpatialType == typeof(GeographyLineString))
            {
                LinqTestCase[] tests_GeographyLineString = GetSupportedLinqQueries_GeographyLineString((IQueryable<TripLeg<GeographyLineString>>)allTripLegsActual, (IQueryable<TripLeg<GeographyLineString>>)allTripLegsExpected);

                tests = tests_GeographyLineString.Concat(tests_BaseGeography).ToArray();
            }
            else
            {
                Assert.Fail("No supported LINQ queries are defined for the type {0}.", testSpatialType);
            }

            return tests;
        }

        private static LinqTestCase[] GetSupportedLinqQueries_BaseGeography<T>(IQueryable<TripLeg<T>> allTripLegsQuery, IEnumerable<TripLeg<T>> allTripLegsExpected) where T : Geography
        {
            var tripLeg1Query = allTripLegsQuery.Where(tl => tl.ID == 1);
            var tripLeg1Expected = allTripLegsExpected.Where(tl => tl.ID == 1);

            return new LinqTestCase[]
            {
                new LinqTestCase()
                {
                    Query = from t in tripLeg1Query
                            select t,

                    ExpectedUri = "",
                    ExpectedResults = tripLeg1Expected,
                    ServerPayload = GetSampleEntityPayload(tripLeg1Expected.Single(), null),
                },
                new LinqTestCase()
                {
                    Query = from t in tripLeg1Query
                            select t.GeographyProperty1,

                    ExpectedUri = "/GeographyProperty1",
                    ExpectedResults = tripLeg1Expected.Select(tl => tl.GeographyProperty1),
                    ServerPayload = GetSamplePropertyPayload("GeographyProperty1", tripLeg1Expected.Single().GeographyProperty1),
                },
                new LinqTestCase()
                {
                    Query = from t in tripLeg1Query
                            select new { t.GeographyProperty1, t.GeographyProperty2 },

                    ExpectedUri = "?$select=GeographyProperty1,GeographyProperty2",
                    ExpectedResults = tripLeg1Expected.Select(tl => new { tl.GeographyProperty1, tl.GeographyProperty2 }),
                    ServerPayload = GetSampleEntityPayload(tripLeg1Expected.Single(), new[] { "GeographyProperty1", "GeographyProperty2" }),
                },
                new LinqTestCase()
                {
                    Query = from t in tripLeg1Query
                            select new { t.GeographyProperty1.CoordinateSystem },

                    ExpectedUri = "?$select=GeographyProperty1",
                    ExpectedResults = tripLeg1Expected.Select(tl => new { tl.GeographyProperty1.CoordinateSystem }),
                    ServerPayload = GetSampleEntityPayload(tripLeg1Expected.Single(), new[] { "GeographyProperty1" }),
                },
                new LinqTestCase()
                {
                    Query = from t in tripLeg1Query
                            select new { GeoProp = t.GeographyProperty2 },

                    ExpectedUri = "?$select=GeographyProperty2",
                    ExpectedResults = tripLeg1Expected.Select(tl => new { GeoProp = tl.GeographyProperty2 }),
                    ServerPayload = GetSampleEntityPayload(tripLeg1Expected.Single(), new[] { "GeographyProperty2" }),
                },
                new LinqTestCase()
                {
                    Query = from t in tripLeg1Query
                            select new { GeoPropSrid = t.GeographyProperty2.CoordinateSystem.Id },

                    ExpectedUri = "?$select=GeographyProperty2",
                    ExpectedResults = tripLeg1Expected.Select(tl => new { GeoPropSrid = tl.GeographyProperty2.CoordinateSystem.Id }),
                    ServerPayload = GetSampleEntityPayload(tripLeg1Expected.Single(), new[] { "GeographyProperty2" }),
                },

                new LinqTestCase()
                {
                    Query = from t in tripLeg1Query
                            select new TripLeg<T>() { GeographyProperty1 = t.GeographyProperty1 },

                    ExpectedUri = "?$select=GeographyProperty1",
                    ExpectedResults = tripLeg1Expected.Select(tl => new TripLeg<T>() { GeographyProperty1 = tl.GeographyProperty1 }),
                    ServerPayload = GetSampleEntityPayload(tripLeg1Expected.Single(), new[] { "GeographyProperty1" }),
                },
            };
        }

        private static LinqTestCase[] GetSupportedLinqQueries_GeographyPoint(IQueryable<TripLeg<GeographyPoint>> allTripLegsQuery, IEnumerable<TripLeg<GeographyPoint>> allTripLegsExpected)
        {
            var tripLeg1Query = allTripLegsQuery.Where(tl => tl.ID == 1);
            var tripLeg1Expected = allTripLegsExpected.Where(tl => tl.ID == 1);

            return new LinqTestCase[]
            {
                new LinqTestCase()
                {
                    Query = from t in tripLeg1Query
                            select new
                            {
                                Lat = t.GeographyProperty2.Latitude,
                                Long = t.GeographyProperty2.Longitude,
                                Z = t.GeographyProperty2.Z,
                                M = t.GeographyProperty2.M
                            },

                    ExpectedUri = "?$select=GeographyProperty2,GeographyProperty2,GeographyProperty2,GeographyProperty2",
                    ExpectedResults = tripLeg1Expected.Select(tl => new { Lat = tl.GeographyProperty2.Latitude, Long = tl.GeographyProperty2.Longitude, Z = tl.GeographyProperty2.Z, M = tl.GeographyProperty2.M }),
                    ServerPayload = GetSampleEntityPayload(tripLeg1Expected.Single(), new[] { "GeographyProperty2" }),
                },
                new LinqTestCase()
                {
                    Query = from t in tripLeg1Query
                            select new ProjectedComplexType()
                            {
                                GeoProp = t.GeographyProperty1,
                                Lat = t.GeographyProperty2.Latitude,
                                Long = t.GeographyProperty2.Longitude,
                                Z = t.GeographyProperty2.Z,
                                M = t.GeographyProperty2.M
                            },

                    ExpectedUri = "?$select=GeographyProperty1,GeographyProperty2,GeographyProperty2,GeographyProperty2,GeographyProperty2",
                    ExpectedResults = tripLeg1Expected.Select(tl => new ProjectedComplexType() { GeoProp = tl.GeographyProperty1, Lat = tl.GeographyProperty2.Latitude, Long = tl.GeographyProperty2.Longitude, Z = tl.GeographyProperty2.Z, M = tl.GeographyProperty2.M }),
                    ServerPayload = GetSampleEntityPayload(tripLeg1Expected.Single(), new[] { "GeographyProperty1", "GeographyProperty2" }),
                },
                new LinqTestCase()
                {
                    Query = from t in tripLeg1Query
                            select new
                            {
                                GeoPoint = GeographyFactory.Point(45.8, -127.0)
                            },

                    ExpectedUri = "?",
                    ExpectedResults = tripLeg1Expected.Select(tl => new { GeoPoint = GeographyFactory.Point(45.8, -127.0) }),
                    ServerPayload = GetSampleEntityPayload(tripLeg1Expected.Single(), null),
                },
                new LinqTestCase()
                {
                    Query = from t in tripLeg1Query
                            select new
                            {
                                GeoPoint = GeographyFactory.Point(t.GeographyProperty1.CoordinateSystem, t.GeographyProperty1.Latitude, t.GeographyProperty2.Longitude, null, null)
                            },

                    ExpectedUri = "?$select=GeographyProperty1,GeographyProperty1,GeographyProperty2",
                    ExpectedResults = tripLeg1Expected.Select(tl => new { GeoPoint = GeographyFactory.Point(tl.GeographyProperty1.CoordinateSystem, tl.GeographyProperty1.Latitude, tl.GeographyProperty2.Longitude, null, null) }),
                    ServerPayload = GetSampleEntityPayload(tripLeg1Expected.Single(), new[] { "GeographyProperty1", "GeographyProperty2" }),
                },
                new LinqTestCase()
                {
                    Query = from t in allTripLegsQuery
                            where t.GeographyProperty1.Distance(GeographyFactory.Point(45.99, -127.1)) > 1.5
                            select t,

                    ExpectedUri = "?$filter=geo.distance(GeographyProperty1,geography'SRID=4326;POINT (-127.1 45.99)') gt 1.5",
                    ExpectedResults = allTripLegsExpected,
                    ServerPayload = GetSampleFeedPayload(allTripLegsExpected, null),
                    ExpectKeyInUri = false,
                },
                new LinqTestCase()
                {
                    Query = from t in allTripLegsQuery
                            where GeographyFactory.Point(45.99, -127.1).Build().Distance(t.GeographyProperty1) > 1.5
                            select t,

                    ExpectedUri = "?$filter=geo.distance(geography'SRID=4326;POINT (-127.1 45.99)',GeographyProperty1) gt 1.5",
                    ExpectedResults = allTripLegsExpected,
                    ServerPayload = GetSampleFeedPayload(allTripLegsExpected, null),
                    ExpectKeyInUri = false,
                },
                new LinqTestCase()
                {
                    Query = from t in allTripLegsQuery
                            where t.GeographyProperty1.Distance(t.GeographyProperty2) <= 1.8
                            select t,

                    ExpectedUri = "?$filter=geo.distance(GeographyProperty1,GeographyProperty2) le 1.8",
                    ExpectedResults = allTripLegsExpected,
                    ServerPayload = GetSampleFeedPayload(allTripLegsExpected, null),
                    ExpectKeyInUri = false,
                },
                new LinqTestCase()
                {
                    Query = from t in allTripLegsQuery
                            orderby t.GeographyProperty1.Distance(GeographyFactory.Point(45.99, -127.1))
                            select t,

                    ExpectedUri = "?$orderby=geo.distance(GeographyProperty1,geography'SRID=4326;POINT (-127.1 45.99)')",
                    ExpectedResults = allTripLegsExpected,
                    ServerPayload = GetSampleFeedPayload(allTripLegsExpected, null),
                    ExpectKeyInUri = false,
                },
                new LinqTestCase()
                {
                    Query = from t in allTripLegsQuery
                            orderby GeographyFactory.Point(45.99, -127.1).Build().Distance(t.GeographyProperty1)
                            select t,

                    ExpectedUri = "?$orderby=geo.distance(geography'SRID=4326;POINT (-127.1 45.99)',GeographyProperty1)",
                    ExpectedResults = allTripLegsExpected,
                    ServerPayload = GetSampleFeedPayload(allTripLegsExpected, null),
                    ExpectKeyInUri = false,
                },
                new LinqTestCase()
                {
                    Query = from t in allTripLegsQuery
                            orderby t.GeographyProperty1.Distance(t.GeographyProperty2)
                            select t,

                    ExpectedUri = "?$orderby=geo.distance(GeographyProperty1,GeographyProperty2)",
                    ExpectedResults = allTripLegsExpected,
                    ServerPayload = GetSampleFeedPayload(allTripLegsExpected, null),
                    ExpectKeyInUri = false,
                },
            };
        }

        private static LinqTestCase[] GetSupportedLinqQueries_GeographyLineString(IQueryable<TripLeg<GeographyLineString>> allTripLegsQuery, IEnumerable<TripLeg<GeographyLineString>> allTripLegsExpected)
        {
            var tripLeg1Query = allTripLegsQuery.Where(tl => tl.ID == 1);
            var tripLeg1Expected = allTripLegsExpected.Where(tl => tl.ID == 1);

            return new LinqTestCase[]
            {
                new LinqTestCase()
                {
                    Query = from t in tripLeg1Query
                            select new
                            {
                                Points = t.GeographyProperty1.Points,
                            },

                    ExpectedUri = "?$select=GeographyProperty1",
                    ExpectedResults = tripLeg1Expected.Select(tl => new { Points = tl.GeographyProperty1.Points }),
                    ServerPayload = GetSampleEntityPayload(tripLeg1Expected.Single(), new[] { "GeographyProperty1" }),
                },
                new LinqTestCase()
                {
                    Query = from t in tripLeg1Query
                            select new
                            {
                                Count = t.GeographyProperty1.Points.Count,
                            },

                    ExpectedUri = "?$select=GeographyProperty1",
                    ExpectedResults = tripLeg1Expected.Select(tl => new { Count = tl.GeographyProperty1.Points.Count }),
                    ServerPayload = GetSampleEntityPayload(tripLeg1Expected.Single(), new[] { "GeographyProperty1" }),
                },
                new LinqTestCase()
                {
                    Query = from t in tripLeg1Query
                            select new ProjectedComplexType()
                            {
                                GeoProp = t.GeographyProperty1,
                                Lat = t.GeographyProperty2.Points[0].Latitude,
                                Long = t.GeographyProperty2.Points[0].Longitude,
                                M = t.GeographyProperty1.Points[1].M,
                                Z = t.GeographyProperty1.Points[1].Z,
                            },

                    ExpectedUri = "?$select=GeographyProperty1,GeographyProperty2,GeographyProperty2,GeographyProperty1,GeographyProperty1",
                    ExpectedResults = tripLeg1Expected.Select(tl =>
                        new ProjectedComplexType()
                            {
                                GeoProp = tl.GeographyProperty1,
                                Lat = tl.GeographyProperty2.Points[0].Latitude,
                                Long = tl.GeographyProperty2.Points[0].Longitude,
                                M = tl.GeographyProperty1.Points[1].M,
                                Z = tl.GeographyProperty1.Points[1].Z,
                            }),
                    ServerPayload = GetSampleEntityPayload(tripLeg1Expected.Single(), new[] { "GeographyProperty1", "GeographyProperty2" }),
                },
                new LinqTestCase()
                {
                    Query = from t in tripLeg1Query
                            select new
                            {
                                GeoLineString = GeographyFactory.LineString(49.98, -140.4).LineTo(77.0, -177.6)
                            },

                    ExpectedUri = "?",
                    ExpectedResults = tripLeg1Expected.Select(tl => new { GeoLineString = GeographyFactory.LineString(49.98, -140.4).LineTo(77.0, -177.6) }),
                    ServerPayload = GetSampleEntityPayload(tripLeg1Expected.Single(), null),
                },
                new LinqTestCase()
                {
                    Query = from t in tripLeg1Query
                            select new
                            {
                                GeoLineString = GeographyFactory.LineString(t.GeographyProperty1.Points[0].Latitude, t.GeographyProperty1.Points[0].Longitude).LineTo(t.GeographyProperty2.Points[1].Latitude, t.GeographyProperty2.Points[1].Longitude)
                            },

                    ExpectedUri = "?$select=GeographyProperty1,GeographyProperty1,GeographyProperty2,GeographyProperty2",
                    ExpectedResults = tripLeg1Expected.Select(tl => new { GeoLineString = GeographyFactory.LineString(tl.GeographyProperty1.Points[0].Latitude, tl.GeographyProperty1.Points[0].Longitude).LineTo(tl.GeographyProperty2.Points[1].Latitude, tl.GeographyProperty2.Points[1].Longitude) }),
                    ServerPayload = GetSampleEntityPayload(tripLeg1Expected.Single(), new[] { "GeographyProperty1", "GeographyProperty2" }),
                },
            };
        }

        private static void VerifyURI(DataServiceContext context, IQueryable actualQuery, string expectedQueryUri, IEnumerable expectedResults, bool expectKeyInUri)
        {
            string expectedUri = String.Format(
                "{0}/TripLegs{1}{2}",
                context.BaseUri.AbsoluteUri,
                expectKeyInUri ? "(1)" : String.Empty,
                expectedQueryUri);

            Assert.AreEqual(Uri.UnescapeDataString(expectedUri), Uri.UnescapeDataString(actualQuery.ToString()), "LINQ query did not produce the expected URI.");

            LinqTests.RunTest(expectedResults, actualQuery, true);
        }

        private static string GetSamplePropertyPayload(string propertyName, object propertyValue)
        {
            return new UnitTestPayloadGenerator(HttpStatusCode.OK, "application/xml").GetSamplePropertyPayload(propertyName, propertyValue);
        }

        private static string GetSampleEntityPayload(object entity, IEnumerable<string> projectedProperties)
        {
            return new UnitTestPayloadGenerator(HttpStatusCode.OK, "application/atom+xml").GetSampleEntityPayload(entity, projectedProperties);
        }

        private static string GetSampleFeedPayload(IEnumerable entities, IEnumerable<string> projectedProperties)
        {
            return new UnitTestPayloadGenerator(HttpStatusCode.OK, "application/atom+xml").GetSampleFeedPayload(entities, projectedProperties);
        }

        internal static DSPUnitTestServiceDefinition GetRoadTripServiceDefinition(Type geographyType, GeographyPropertyValues defaultValues, bool useComplexType = false, bool useOpenTypes = false, Action<DSPMetadata> modifyMetadata = null)
        {
            DSPMetadata roadTripMetadata = SpatialTestUtil.CreateRoadTripMetadata(geographyType, useComplexType, useOpenTypes, modifyMetadata);
            return SpatialTestUtil.CreateRoadTripServiceDefinition(roadTripMetadata, defaultValues, DSPDataProviderKind.CustomProvider, useComplexType);
        }
    }

    public class TripLeg<T> where T : Geography
    {
        public int ID { get; set; }
        public T GeographyProperty1 { get; set; }
        public T GeographyProperty2 { get; set; }
    }

    public class GeometricEntity<T> where T : Geometry
    {
        public int ID { get; set; }
        public T Property1 { get; set; }
        public T Property2 { get; set; }
    }

    public class ProjectedComplexType
    {
        public Geography GeoProp { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
        public double? Z { get; set; }
        public double? M { get; set; }
    }
}

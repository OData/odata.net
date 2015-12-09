//---------------------------------------------------------------------
// <copyright file="RequestUriProcessorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests.Server
{
    using System;
    using System.Collections;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;
    using System.Linq;
    using System.Linq.Expressions;
    using Microsoft.Spatial;
    using Microsoft.Spatial.Tests;
    using AstoriaUnitTests.Tests.Server.Simulators;
    using DataSpatialUnitTests.Utils;
    using FluentAssertions;
    using Microsoft.Data.Spatial;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RequestUriProcessorTests
    {
        private static readonly Uri baseUri = new Uri("http://localhost/service/");

        private ServiceSimulatorFactory serviceFactory;

        public class TestEntityType
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public Geography Spatial { get; set; }
            public GeographyPoint Point { get; set; }
        }

        public class TestEntityContext : IDataSourceSimulator
        {
            TestEntityType[] entities = new[] { 
                new TestEntityType { ID = 0, Name = "Entity 1", Spatial = TestData.PointG(10, 20, null, null), Point = TestData.PointG(10, 20, null, null) }, 
                new TestEntityType { ID = 1, Name = "Entity 2", Spatial = TestData.PointG(10, 20, null, null), Point = TestData.PointG(10, 20, null, null)  }, 
                new TestEntityType { ID = 2, Name = "Entity 3", Spatial = TestData.PointG(10.1, 20, null, null), Point = TestData.PointG(10.1, 20, null, null)  },
                new TestEntityType { ID = 3, Name = "Entity 4", Spatial = TestData.PointG(10, 20, null, null, CoordinateSystem.Geography(1111)), Point = TestData.PointG(10, 20, null, null, CoordinateSystem.Geography(1111))  },             
            };

            public IQueryable GetQueryableRoot(string resourceSet)
            {
                return entities.AsQueryable();
            }

            public object GetPropertyValue(object target, string propertyName)
            {
                switch (propertyName)
                {
                    case "ID": return 0;
                    case "Name": return "Name";
                }

                return null;
            }
        }

        public RequestUriProcessorTests()
        {
            this.serviceFactory = new ServiceSimulatorFactory();
            this.PopulateTestMetadata();
            this.serviceFactory.SetDataSource(new TestEntityContext());
        }

        [TestCleanup]
        public void PerTestCleanup()
        {
            this.serviceFactory.ClearQueryArgument();
        }

        [TestMethod]
        public void QuerySpatialProperty()
        {
            this.serviceFactory.SetRequestUri("Entities(0)/Spatial");
            var service = this.serviceFactory.CreateService();

            RequestDescription d = RequestUriProcessor.ProcessRequestUri(this.serviceFactory.RequestUri, service, false);
            Assert.AreEqual(RequestTargetKind.Primitive, d.LastSegmentInfo.TargetKind);
            Assert.AreEqual("Spatial", d.LastSegmentInfo.Identifier);
        }

        [TestMethod]
        public void FilterCompareSpatialPropertyAndLiteral()
        {
            var tests = new[]
                        {
                            new { Operator = "eq", ErrorName = "Equal" },
                            new { Operator = "ne", ErrorName = "NotEqual" }, 
                            new { Operator = "lt", ErrorName = "LessThan" }, 
                            new { Operator = "gt", ErrorName = "GreaterThan" },
                            new { Operator = "le", ErrorName = "LessThanOrEqual" },
                            new { Operator = "ge", ErrorName = "GreaterThanOrEqual" }
                        };

            this.serviceFactory.SetRequestUri("Entities");

            foreach (var test in tests)
            {
                // property vs spatial literal
                this.serviceFactory.SetRequestUri("Entities?$filter=Spatial " + test.Operator + " geography'SRID=1234;POINT(10 20)'");
                var service = this.serviceFactory.CreateService();

                Action processUri = () => RequestUriProcessor.ProcessRequestUri(this.serviceFactory.RequestUri, service, false);
                processUri.ShouldThrow<DataServiceException>().WithMessage(Microsoft.OData.Core.Strings.MetadataBinder_IncompatibleOperandsError("Edm.Geography", "Edm.GeographyPoint", test.ErrorName));
            }
        }

        [TestMethod]
        public void FilterCompareSpatialLiterals()
        {
            var tests = new[]
                        {
                            new { Operator = "eq", ErrorName = "Equal" },
                            new { Operator = "ne", ErrorName = "NotEqual" }, 
                            new { Operator = "lt", ErrorName = "LessThan" }, 
                            new { Operator = "gt", ErrorName = "GreaterThan" },
                            new { Operator = "le", ErrorName = "LessThanOrEqual" },
                            new { Operator = "ge", ErrorName = "GreaterThanOrEqual" }
                        };

            this.serviceFactory.SetRequestUri("Entities");

            foreach (var test in tests)
            {
                // spatial literal vs spatial literal
                this.serviceFactory.SetRequestUri("Entities?$filter=geography'POINT(10 20)' " + test.Operator + " geography'SRID=1234;POINT(10 20)'");

                var service = this.serviceFactory.CreateService();

                Action processUri = () => RequestUriProcessor.ProcessRequestUri(this.serviceFactory.RequestUri, service, false);
                processUri.ShouldThrow<DataServiceException>().WithMessage(Microsoft.OData.Core.Strings.MetadataBinder_IncompatibleOperandsError("Edm.GeographyPoint", "Edm.GeographyPoint", test.ErrorName));
            }
        }

        [TestMethod]
        public void FilterSpatialMathOperators()
        {
            var tests = new[]
                        {
                            new { Operator = "add", ErrorName = "Add" },
                            new { Operator = "mul", ErrorName = "Multiply" }, 
                        };

            this.serviceFactory.SetRequestUri("Entities");

            foreach (var test in tests)
            {
                // property vs spatial literal
                this.serviceFactory.SetRequestUri("Entities?$filter=Spatial " + test.Operator + " geography'SRID=1234;POINT(10 20)'");
                var service = this.serviceFactory.CreateService();

                Action processUri = () => RequestUriProcessor.ProcessRequestUri(this.serviceFactory.RequestUri, service, false);
                processUri.ShouldThrow<DataServiceException>().WithMessage(Microsoft.OData.Core.Strings.MetadataBinder_IncompatibleOperandsError("Edm.Geography", "Edm.GeographyPoint", test.ErrorName));
            }
        }

        [TestMethod]
        public void FilterBySpatialFunctionWithLiterals()
        {
            Tuple<string, int>[] testCases = new Tuple<string, int>[] {
                Tuple.Create("eq 1", 4),
                Tuple.Create("ne 2", 4),
                Tuple.Create("gt 2", 0),
            };

            this.serviceFactory.SetRequestUri("Entities");
            string resourcePath = this.serviceFactory.RequestUri.OriginalString;

            foreach (var t in testCases)
            {
                this.serviceFactory.ClearQueryArgument();
                this.serviceFactory.AddQueryArgument("$filter", "geo.distance(geography'SRID=1234;POINT(20 10)', geography'SRID=1234;POINT(20 10)') " + t.Item1);
                this.serviceFactory.SetRequestUri(resourcePath + "?$filter=geo.distance(geography'SRID=1234;POINT(20 10)', geography'SRID=1234;POINT(20 10)') " + t.Item1);
                var service = this.serviceFactory.CreateService();

                RequestDescription d = RequestUriProcessor.ProcessRequestUri(this.serviceFactory.RequestUri, service, false);

                Action<GeographyPoint> verifyPoint =
                    p =>
                    {
                        Assert.AreEqual(20.0, p.Longitude);
                        Assert.AreEqual(10.0, p.Latitude);
                    };

                RunSpatialDistanceFilterTest(d, verifyPoint, verifyPoint, 1.0, t.Item2);
            }
        }


        [TestMethod]
        public void FilterBySpatialFunctionWithProperties()
        {
            var testCases = new[] {
                Tuple.Create("eq 1", 4),
                Tuple.Create("ne 2", 4),
                Tuple.Create("gt 2", 0),
            };

            foreach (var t in testCases)
            {
                this.serviceFactory.SetRequestUri("Entities?$filter=geo.distance(Point, geography'SRID=1234;POINT(20 10)') " + t.Item1);
                var service = this.serviceFactory.CreateService();

                RequestDescription d = RequestUriProcessor.ProcessRequestUri(this.serviceFactory.RequestUri, service, false);

                Action<GeographyPoint> verifyPoint =
                    p =>
                    {
                        Assert.AreEqual(20.0, p.Longitude);
                        Assert.AreEqual(10.0, p.Latitude);
                    };

                RunSpatialDistanceFilterTest(d, null, verifyPoint, 1.0, t.Item2);
            }
        }

        [TestMethod]
        [Ignore] // Currently we have overloads of geog, geogPoint; etc. because EF forces us to use properties of type Geography rather than GeographyPoint.
        public void FilterBySpatialFunctionWrongType()
        {
            this.serviceFactory.SetRequestUri("Entities");

            this.serviceFactory.AddQueryArgument("$filter", "geo.distance(Spatial, SRID=1234;POINT(10 20)) lt 10");
            var service = this.serviceFactory.CreateService();

            Action test = () => RequestUriProcessor.ProcessRequestUri(this.serviceFactory.RequestUri, service, false);

#if !EFRTM
            string expectedFunctions = "geo.distance(System.Data.Spatial.Geography.GeographyPoint, System.Data.Spatial.Geography.GeographyPoint); geo.distance(System.Data.Spatial.DbGeography, System.Data.Spatial.Geography.GeographyPoint); geo.distance(System.Data.Spatial.DbGeography, System.Data.Spatial.DbGeography); geo.distance(System.Data.Spatial.Geography.GeographyPoint, System.Data.Spatial.DbGeography)";
#else
            string expectedFunctions = "geo.distance(System.Data.Spatial.Geography.GeographyPoint, System.Data.Spatial.Geography.GeographyPoint)";
#endif

            test.ShouldThrow<DataServiceException>().WithMessage(Microsoft.OData.Service.Strings.RequestQueryParser_NoApplicableFunction(
                            "geo.distance",
                            expectedFunctions));
        }

        [TestMethod]
        public void FilterBySpatialFunctionWithNulls()
        {
            // The extra object cast is to mimic the product behavior. We've got literals that linq to objects is
            // explicitly converting to Nullable<double>. So I force an explicit cast in the URL. But the product
            // then introduces two convert nodes. So we force a double cast in the Linq to objects to match.
            RunSpatialExpressionTest("$filter", "geo.distance(cast(null, 'Edm.GeometryPoint'), cast(null,'Edm.GeometryPoint')) gt cast(0, 'Edm.Double')", q => q.Where(it => GeometryOperationsExtensions.Distance(null, null) > (double?)(object)0));
            RunSpatialExpressionTest("$filter", "geo.distance(Point, null) gt cast(0, 'Edm.Double')", q => q.Where(it => GeographyOperationsExtensions.Distance(it.Point, null) > (double?)(object)0));
            RunSpatialExpressionTest("$filter", "geo.distance(null, Point) gt cast(0, 'Edm.Double')", q => q.Where(it => GeographyOperationsExtensions.Distance(null, it.Point) > (double?)(object)0));
        }

        [TestMethod]
        public void OrderBySpatialFunctionWithNulls()
        {
            RunSpatialExpressionTest("$orderby", "geo.distance(cast(null, 'Edm.GeometryPoint'), cast(null, 'Edm.GeometryPoint')) asc", q => q.OrderBy(element => GeometryOperationsExtensions.Distance(null, null)));
            RunSpatialExpressionTest("$orderby", "geo.distance(Point, null) asc", q => q.OrderBy(element => GeographyOperationsExtensions.Distance(element.Point, null)));
            RunSpatialExpressionTest("$orderby", "geo.distance(null, Point) asc", q => q.OrderBy(element => GeographyOperationsExtensions.Distance(null, element.Point)));
        }

        private void PopulateTestMetadata()
        {
            ResourceType entityType = new ResourceType(typeof(TestEntityType), ResourceTypeKind.EntityType, null, "AstoriaUnitTests.Tests.Server", "TestEntityType", false);
            entityType.CanReflectOnInstanceType = true;
            entityType.AddProperty(new ResourceProperty("ID", ResourcePropertyKind.Key | ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(Int32))));
            entityType.AddProperty(new ResourceProperty("Name", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(String))));
            entityType.AddProperty(new ResourceProperty("Spatial", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(Geography))));
            entityType.AddProperty(new ResourceProperty("Point", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(GeographyPoint))));
            entityType.SetReadOnly();

            serviceFactory.AddResourceType(entityType);

            ResourceSet entitySet = new ResourceSet("Entities", entityType);
            entitySet.SetReadOnly();
            serviceFactory.AddResourceSet(entitySet);
        }

        private static void RunSpatialDistanceFilterTest(RequestDescription d, Action<GeographyPoint> verifyPoint1, Action<GeographyPoint> verifyPoint2, double distanceToReturn, int expectedCount)
        {
            bool called = false;
            Func<Geography, Geography, double> callback =
                (Geography geo1, Geography geo2) =>
                {
                    called = true;

                    var point1 = geo1 as GeographyPointImplementation;
                    Assert.IsNotNull(point1);

                    var point2 = geo2 as GeographyPointImplementation;
                    Assert.IsNotNull(point2);

                    if (verifyPoint1 != null)
                    {
                        verifyPoint1(point1);
                    }

                    if (verifyPoint2 != null)
                    {
                        verifyPoint2(point2);
                    }

                    return distanceToReturn;
                };

            AssertConstantExpressionsAreOfPublicType(d.RequestExpression);

            var e = Invoke(d.RequestExpression).GetEnumerator();

            using (SpatialTestUtils.RegisterOperations(new DistanceOperationImplementation(callback)))
            {
                int count = 0;
                while (e.MoveNext())
                {
                    count++;
                }
                Assert.IsTrue(called);
                Assert.AreEqual(expectedCount, count);
            }
        }

        private void RunSpatialExpressionTest(string queryOption, string queryOptionValue, Expression<Func<IQueryable<TestEntityType>, IQueryable<TestEntityType>>> expectedExpressionPattern)
        {
            this.serviceFactory.SetRequestUri("Entities?" + queryOption + "=" + queryOptionValue);
            var service = this.serviceFactory.CreateService();

            var d = RequestUriProcessor.ProcessRequestUri(this.serviceFactory.RequestUri, service, false);

            var actualQueryableMethodExpression = (MethodCallExpression)d.RequestExpression;

            var expectedString = expectedExpressionPattern.ToString();
            var actualString = actualQueryableMethodExpression.ToString();

            Console.WriteLine();
            Console.WriteLine("Expected: " + expectedString);
            Console.WriteLine("Actual: " + actualString);

            // get the portion of the expected string which represents the 'root'. it will look something like 'q => q.'
            var parameterString = expectedExpressionPattern.Parameters.Single().ToString();
            parameterString = parameterString + " => " + parameterString + ".";
            expectedString = expectedString.Substring(parameterString.Length);

            // just verify that the .Where or .OrderBy call matches. We don't verify the root of the tree.
            Assert.IsTrue(actualString.EndsWith(expectedString));
        }

        private static IEnumerable Invoke(Expression requestExpression)
        {
            Expression<Func<IEnumerable>> funcExpression = Expression.Lambda<Func<IEnumerable>>(Expression.Convert(requestExpression, typeof(IEnumerable)));
            Func<IEnumerable> requestFunc = funcExpression.Compile();
            return requestFunc();
        }

        private static void AssertConstantExpressionsAreOfPublicType(Expression requestExpression)
        {
            var constantExpressionValidator = new ConstantExpressionValidatingVisitor();
            constantExpressionValidator.Visit(requestExpression);
        }

        private class DistanceOperationImplementation : SpatialOperations
        {
            private readonly Func<Geography, Geography, double> distanceFunc;

            public DistanceOperationImplementation(Func<Geography, Geography, double> distanceFunc)
            {
                this.distanceFunc = distanceFunc;
            }

            public override double Distance(Geography operand1, Geography operand2)
            {
                return this.distanceFunc(operand1, operand2);
            }
        }
    }

    public class ConstantExpressionValidatingVisitor : ExpressionVisitor
    {
        protected override Expression VisitConstant(ConstantExpression node)
        {
            Assert.IsTrue(node.Type.IsPublic, "Node Type of constant expression should be public");
            return base.VisitConstant(node);
        }
    }
}

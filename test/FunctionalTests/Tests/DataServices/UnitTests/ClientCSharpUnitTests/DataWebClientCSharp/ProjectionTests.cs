//---------------------------------------------------------------------
// <copyright file="ProjectionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region Namespaces

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.OData.Service;
    using Microsoft.OData.Client;
    using System.Data.Test.Astoria;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using AstoriaUnitTests.Stubs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Linq.Expressions;
    using System.Text.RegularExpressions;
    using AstoriaUnitTests.Data;
    using Microsoft.OData.Core;

    #endregion Namespaces

    [DeploymentItem("Workspaces", "Workspaces")]
    [TestClass]
    public class ProjectionTests
    {
        private Uri serviceRoot;
        private DataServiceContext context;
        private const string FeedStart = AtomParserTests.FeedStart;
        private const string EmptyFeed = AtomParserTests.EmptyFeed;

        [TestInitialize]
        public void Initialize()
        {
            ReadOnlyTestContext.ClearBaselineIncludes();
            this.serviceRoot = new Uri("http://localhost/");
            this.context = new DataServiceContext(serviceRoot);
            this.context.EnableAtom = true;
            this.context.Format.UseAtom();
            this.context.IgnoreMissingProperties = false;
        }

        [TestMethod]
        public void AtomMaterializerDirectMaterializationTest()
        {
            string xml = FeedStart + "<entry><id>http://localhost/t1</id><link rel='edit' href='t1'/><content type='application/xml'><m:properties><d:TeamID>1</d:TeamID></m:properties></content></entry></feed>";
            var query = context.CreateQuery<Team>("Teams");
            foreach (var team in CreateTestMaterializeAtom(xml, query))
            {
                Assert.AreEqual(1, team.TeamID, "TeamID");
            }

            AssertEntityCount(1, "single team materialized");
        }

        #region Test Materialization

        [TestMethod]
        public void TestNullPropagtion()
        {
            string entry = @"<entry><id>http://localhost.:13403/WebServer/Northwind.svc/EntitySet(1)</id>
    <link rel='edit' title='EntityType' href='EntitySet(1)' />
    <link rel='http://docs.oasis-open.org/odata/ns/related/Member' type='application/atom+xml;type=entry' title='Member' href='EntitySet(1)/SelfReference'>
      <m:inline />
    </link>
    <category term='DataModel.EntityType' scheme='http://docs.oasis-open.org/odata/ns/scheme' />
    <content type='application/xml'>
      <m:properties>
        <d:ID m:type='Edm.Int32'>1</d:ID>
      </m:properties>
    </content>
  </entry>";
            string xml = FeedStart + entry + "</feed>";

            {
                Trace.WriteLine("Without null propagation, this fails.");
                var q = from e in this.context.CreateQuery<TypedEntity<int, TypedEntity<int, int>>>("EntitySet")
                        select new TypedEntity<int, TypedEntity<int, int>>()
                        {
                            ID = e.ID,
                            Member = new TypedEntity<int, int>()
                            {
                                ID = e.Member.ID,
                                Member = e.Member.Member
                            }
                        };
                Exception exception = TestUtil.RunCatching(() => { CreateTestMaterializeAtom(xml, q).ToList(); });
                TestUtil.AssertExceptionExpected(exception, true);
                TestUtil.AssertContains(exception.ToString(), "NullReferenceException");
            }

            {
                Trace.WriteLine("With null propagation, this succeeds.");
                var q = from e in this.context.CreateQuery<TypedEntity<int, TypedEntity<int, int>>>("EntitySet")
                        select new TypedEntity<int, TypedEntity<int, int>>()
                        {
                            ID = e.ID,
                            Member = (e.Member == null) ? null : new TypedEntity<int, int>()
                            {
                                ID = e.Member.ID,
                                Member = e.Member.Member
                            }
                        };
                foreach (var item in CreateTestMaterializeAtom(xml, q))
                {
                    Assert.IsNotNull(item, "item");
                    Assert.AreEqual(1, item.ID, "item.ID");
                    Assert.IsNull(item.Member);
                }
            }
        }

        [TestMethod]
        public void MaterializationShouldNotThrowForAnonymousTypes()
        {
            // Client Projections : materialization for anonymous types throws ArgumentException in VB
            string xml = EmptyFeed;
            var q = from c in this.context.CreateQuery<northwindClient.Categories>("C")
                    select new { PN = from p in c.Products select p.ProductName, cat = c };
            foreach (var o in CreateTestMaterializeAtom(xml, q))
            {
            }
        }

        public class Employee
        {
            public int EmployeeID { get; set; }
            public DataServiceCollection<northwindClient.Employees> Employees1 { get; set; }
        }

        [TestMethod]
        public void TypeResolutionShouldWorkForEntityTypeAttribute()
        {
            // Client Projections + SDP : Invalid type resolution when projecting into a EntityType attributed type and projecting a DSC of source entity type
            string employeeA = AnyEntry("e1", "<d:EmployeeID>1</d:EmployeeID>", null);
            string employees = FeedStart + employeeA + "</feed>";
            string employeeB = AnyEntry("e2", "<d:EmployeeID>2</d:EmployeeID>", Link(false, "Employees1", employees));
            string xml = FeedStart + employeeB + "</feed>";
            var q = from e in this.context.CreateQuery<northwindClient.Employees>("Employees")
                    select new Employee()
                    {
                        EmployeeID = e.EmployeeID,
                        Employees1 = new DataServiceCollection<northwindClient.Employees>(e.Employees1, TrackingMode.None)
                    };
            foreach (var item in CreateTestMaterializeAtom(xml, q))
            {
                Assert.IsNotNull(item, "item");

                Assert.AreEqual(2, item.EmployeeID, "item.EmployeeID");
                Assert.IsNotNull(item.Employees1, "item.Employees1");
                Assert.AreEqual(1, item.Employees1.Count, "item.Employees1.Count");

                Assert.AreEqual(1, item.Employees1[0].EmployeeID, "item.Employees1[0].EmployeeID");
            }
        }

        [TestMethod]
        public void ShouldNotThrowWhenCreateSubProjection()
        {
            // Client Projections + SDP : MissingMethodException : Cannot Create Inner DSC with Sub-Projection inside Projections
            string collection = EmptyFeed;
            string xml = FeedStart +
                AnyEntry("e1", "<d:ID>1</d:ID>", Link(false, "Member", collection)) +
                "</feed>";
            {
                Trace.WriteLine("Testing with direct creation");
                var q = from c in this.context.CreateQuery<TypedEntity<int, List<TypedEntity<int, int>>>>("C")
                        select new TypedEntity<int, DataServiceCollection<TypedEntity<int, int>>>
                        {
                            ID = c.ID,
                            Member = new DataServiceCollection<TypedEntity<int, int>>(c.Member, TrackingMode.None)
                        };
                foreach (var item in CreateTestMaterializeAtom(xml, q))
                {
                }
            }
            {
                Trace.WriteLine("Testing with identity .Select");
                var q = from c in this.context.CreateQuery<TypedEntity<int, List<TypedEntity<int, int>>>>("C")
                        select new TypedEntity<int, DataServiceCollection<TypedEntity<int, int>>>
                        {
                            ID = c.ID,
                            Member = new DataServiceCollection<TypedEntity<int, int>>(from m in c.Member select m, TrackingMode.None)
                        };
                foreach (var item in CreateTestMaterializeAtom(xml, q))
                {
                }
            }
        }

        [TestMethod]
        public void PropertyNamesForCollectionsShouldMatch()
        {
            // Client Projections + SDP : Mismatched property names for collections
            // Source type: DoubleMemberTypedEntity<int, int, List<int>>
            // Target type: TypedEntity<int, List<int>>
            string xml = EmptyFeed;
            var q = from c in this.context.CreateQuery<DoubleMemberTypedEntity<int, int, List<int>>>("C")
                    select new TypedEntity<int, List<int>>
                    {
                        ID = c.ID,
                        Member = c.Member2
                    };
            Exception exception = TestUtil.RunCatching(() =>
                {
                    foreach (var o in CreateTestMaterializeAtom(xml, q))
                    {
                    }
                });
            TestUtil.AssertExceptionExpected(exception, true);
            string message = exception.ToString();
            TestUtil.AssertContains(message, "System.NotSupportedException");
            TestUtil.AssertContains(message, "Cannot assign");
            TestUtil.AssertContains(message, "Member2");
            TestUtil.AssertContains(message, "Member");
        }

        [EntityType]
        public class SomeEntity
        {
            public int ID { get; set; }
            public DataServiceQueryContinuation<Order> Continuation { get; set; }
        }

        [TestMethod]
        public void NewDataServiceCollectionContinuationIsNotSupported()
        {
            using (TestWebRequest req = TestWebRequest.CreateForInProcessWcf())
            {
                req.DataServiceType = typeof(CustomDataContext);
                req.StartService();

                DataServiceContext ctx = new DataServiceContext(req.ServiceRoot);
                ctx.Format.UseAtom();

                // Non-entity
                var q = from e in ctx.CreateQuery<Customer>("Customers")
                        select new { Continuation = new DataServiceCollection<Order>(e.Orders, TrackingMode.None).Continuation };

                VerifyNotSupportedException(TestUtil.RunCatching(() => { foreach (var item in q) { }; }),
                "System.NotSupportedException: Constructing or initializing instances of the type <>f__AnonymousType`1[Microsoft.OData.Client.DataServiceQueryContinuation`1[AstoriaUnitTests.Stubs.Order]] with the expression new DataServiceCollection`1(e.Orders, None).Continuation is not supported");

                // Entity
                var q2 = from e in ctx.CreateQuery<Customer>("Customers")
                         select new SomeEntity { ID = e.ID, Continuation = new DataServiceCollection<Order>(e.Orders, TrackingMode.None).Continuation };

                VerifyNotSupportedException(TestUtil.RunCatching(() => { foreach (var item in q2) { }; }),
                "System.NotSupportedException: Initializing instances of the entity type AstoriaUnitTests.Tests.ProjectionTests+SomeEntity with the expression new DataServiceCollection`1(e.Orders, None).Continuation is not supported");
            }
        }

        [TestMethod]
        public void AssignValueToIncorrectDepthShouldThrow()
        {
            // Client Projections : Asserts & ArgumentOutOfRangeException when running nested projection by assigning values to references at an incorrect depth
            var q = from entity in context.CreateQuery<SelfReferenceTypedEntity<int, int>>("EntitySet")
                    select new SelfReferenceTypedEntity<int, int>()
                    {
                        Reference = new SelfReferenceTypedEntity<int, int>()
                        {
                            Reference = new SelfReferenceTypedEntity<int, int>()
                            {
                                Reference = new SelfReferenceTypedEntity<int, int>()
                                {
                                    Reference = new SelfReferenceTypedEntity<int, int>()
                                    {
                                        Reference = new SelfReferenceTypedEntity<int, int>()
                                        {
                                            Reference = new SelfReferenceTypedEntity<int, int>()
                                            {
                                                Reference = new SelfReferenceTypedEntity<int, int>()
                                                {
                                                    Reference = new SelfReferenceTypedEntity<int, int>()
                                                    {
                                                        ID = entity.Reference.Reference.Reference.Reference.ID
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    };
            Exception exception = TestUtil.RunCatching(() => { foreach (var item in CreateTestMaterializeAtom("<foo />", q)) { }; });
            TestUtil.AssertExceptionExpected(exception, true);

            // Could do better, but the exception type is correct.
            TestUtil.AssertContains(exception.ToString(),
                "NotSupportedException: Initializing instances of the entity type AstoriaUnitTests.Stubs.SelfReferenceTypedEntity`2[System.Int32,System.Int32] with the expression new SelfReferenceTypedEntity`2() {Reference = new SelfReferenceTypedEntity`2() {Reference = new SelfReferenceTypedEntity`2() {Reference = new SelfReferenceTypedEntity`2() {Reference = new SelfReferenceTypedEntity`2() {ID = entity.Reference.Reference.Reference.Reference.ID}}}}} is not supported");
        }

        [TestMethod]
        public void LambdaParameterOutOfScopeShouldThrow()
        {
            // Client Projections: InvalidOperationException , Lambda Parameter not in scope when projecting entities at incorrect levels in the query
            var q = from entity in context.CreateQuery<SelfReferenceTypedEntity<int, int>>("EntitySet")
                    select new SelfReferenceTypedEntity<int, int>()
                    {
                        Reference = new SelfReferenceTypedEntity<int, int>()
                        {
                            Reference = new SelfReferenceTypedEntity<int, int>()
                            {
                                ID = entity.Reference.ID
                            }
                        }
                    };
            Exception exception = TestUtil.RunCatching(() => { foreach (var item in CreateTestMaterializeAtom("<foo />", q)) { }; });
            TestUtil.AssertExceptionExpected(exception, true);

            // Could do better, but the exception type is correct.
            TestUtil.AssertContains(exception.ToString(),
                "NotSupportedException: Initializing instances of the entity type AstoriaUnitTests.Stubs.SelfReferenceTypedEntity`2[System.Int32,System.Int32] with the expression new SelfReferenceTypedEntity`2() {Reference = new SelfReferenceTypedEntity`2() {ID = entity.Reference.ID}} is not supported");
        }

        [TestMethod]
        public void CastListOfTToIEnumerableOfTShouldWork()
        {
            // Client Projections : Cannot Cast List<T> to IEnumerable or IEnumerable<T> in Projection
            string xml = FeedStart + "</feed>";

            {
                Trace.WriteLine("Using as IEnumerable");
                var q = from entity in context.CreateQuery<SelfReferenceTypedEntity<int, int>>("EntitySet")
                        select new { Navigation = entity.Collection.ToList() as IEnumerable };

                foreach (var item in CreateTestMaterializeAtom(xml, q)) { }
            }

            {
                Trace.WriteLine("Using as IEnumerable<T>");
                var q = from entity in context.CreateQuery<SelfReferenceTypedEntity<int, int>>("EntitySet")
                        select new { Navigation = entity.Collection.ToList() as IEnumerable<SelfReferenceTypedEntity<int, int>> };

                foreach (var item in CreateTestMaterializeAtom(xml, q)) { }
            }

            {
                Trace.WriteLine("Using as ICollection");
                var q = from entity in context.CreateQuery<SelfReferenceTypedEntity<int, int>>("EntitySet")
                        select new { Navigation = entity.Collection.ToList() as ICollection };

                foreach (var item in CreateTestMaterializeAtom(xml, q)) { }
            }

            {
                Trace.WriteLine("Using as ICollection<T>");
                var q = from entity in context.CreateQuery<SelfReferenceTypedEntity<int, int>>("EntitySet")
                        select new { Navigation = entity.Collection.ToList() as ICollection<SelfReferenceTypedEntity<int, int>> };

                foreach (var item in CreateTestMaterializeAtom(xml, q)) { }
            }
            {
                Trace.WriteLine("Using as IEnumerable in nested collection");
                var q = from entity in context.CreateQuery<SelfReferenceTypedEntity<int, int>>("EntitySet")
                        select new
                        {
                            SelfReference = new { Navigation = entity.Reference.Collection.ToList() as IEnumerable }
                        };

                foreach (var item in CreateTestMaterializeAtom(xml, q)) { }
            }
            {
                Trace.WriteLine("Using as IEnumerable<T> in nested collection");
                var q = from entity in context.CreateQuery<SelfReferenceTypedEntity<int, int>>("EntitySet")
                        select new
                        {
                            SelfReference = new { Navigation = entity.Reference.Collection.ToList() as IEnumerable<SelfReferenceTypedEntity<int, int>> }
                        };

                foreach (var item in CreateTestMaterializeAtom(xml, q)) { }
            }
        }

        [EntityType]
        public class NarrowEmployees
        {
            public int EmployeeID { get; set; }
            public NarrowEmployees Employees2 { get; set; }
            public IEnumerable<NarrowEmployees> Employees1 { get; set; }
        }

        [TestMethod]
        public void NestedProjectionQueryIntoNarrowEntityTypesShouldNotStackOverflow()
        {
            // Client Projections : StackOverflowException when running a nested projection query into narrow entity types
            string xml = EmptyFeed;
            {
                Trace.WriteLine("Single-level cast.");
                var projectIntoIEnumerable =
                    from cust in this.context.CreateQuery<northwindClient.Employees>("Employees")
                    where cust.Employees2 != null && cust.Employees2.Employees2 != null
                    select new NarrowEmployees()
                    {
                        Employees2 = new NarrowEmployees()
                        {
                            Employees2 = new NarrowEmployees()
                            {
                                Employees1 = (from emp in cust.Employees2.Employees2.Employees1
                                              select new NarrowEmployees()
                                              {
                                                  EmployeeID = emp.EmployeeID
                                              })
                                              as IEnumerable<NarrowEmployees>
                            }
                        }
                    };
                foreach (var entity in CreateTestMaterializeAtom(xml, projectIntoIEnumerable))
                {
                }
            }
            {
                Trace.WriteLine("Nested-level cast.");
                var projectIntoIEnumerable =
                    from cust in this.context.CreateQuery<northwindClient.Employees>("Employees")
                    where cust.Employees2 != null && cust.Employees2.Employees2 != null
                    select new NarrowEmployees()
                    {
                        Employees2 = new NarrowEmployees()
                        {
                            Employees2 = new NarrowEmployees()
                            {
                                Employees1 = (IEnumerable<NarrowEmployees>)
                                (from emp in cust.Employees2.Employees2.Employees1
                                 select new NarrowEmployees()
                                 {
                                     EmployeeID = emp.EmployeeID
                                 })
                                              as IEnumerable<NarrowEmployees>
                            }
                        }
                    };
                foreach (var entity in CreateTestMaterializeAtom(xml, projectIntoIEnumerable))
                {
                }
            }
        }

        private void VerifyNotSupportedException(Exception exception, string expectedExceptionString)
        {
            TestUtil.AssertExceptionExpected(exception, true);
            TestUtil.AssertContains(Regex.Replace(exception.ToString(), "<>f__AnonymousType[\\w\\d]+`", "<>f__AnonymousType`"), expectedExceptionString);
        }

        [TestMethod]
        public void IIFOperatorShouldWorkInProjection_CS()
        {
            // Client Projections + VB : IIF Operator does not work in a projection
            // ?: is lazy in C#, so this doesn't throw an exception; Iif is eager, so it does.
            string xml = FeedStart + AnyEntry("e1", "<d:ID>1</d:ID><d:Member m:null='true' />", null) + "</feed>";
            var q = from t in this.context.CreateQuery<TypedEntity<int, int?>>("T")
                    select new { v = t.Member.HasValue ? t.Member.Value : 10 };
            foreach (var item in CreateTestMaterializeAtom(xml, q))
            {
                Assert.IsNotNull(item, "item");
                Assert.AreEqual(10, item.v, "item.v");
            }
        }

        [TestMethod]
        public void StringCompareInProjectionShouldWork()
        {
            // Client Projections : Assert when running String.Compare inside a projection
            string xml = FeedStart + AnyEntry("e1", "<d:ID>1</d:ID><d:Member>test</d:Member>", null) + "</feed>";
            var q = from t in this.context.CreateQuery<TypedEntity<int, string>>("T")
                    select new
                    {
                        a = String.CompareOrdinal(t.Member, "abc") > 0,
                        b = String.Compare(t.Member, "test"),
                        c = Microsoft.VisualBasic.CompilerServices.Operators.CompareString(t.Member, "TEST", true),
                        d = Microsoft.VisualBasic.CompilerServices.Operators.CompareString(t.Member, "TEST", false),
                        e = t.Member.CompareTo("test") == 0,
                        z = t.Member
                    };
            foreach (var item in CreateTestMaterializeAtom(xml, q))
            {
                Assert.IsNotNull(item, "item");
                Assert.AreEqual("test", item.z, "item.z");
                Assert.IsTrue(item.a, "item.a");
                Assert.AreEqual(0, item.b, "item.b");
                Assert.AreEqual(Microsoft.VisualBasic.CompilerServices.Operators.CompareString("test", "TEST", true), item.c, "item.c");
                Assert.AreEqual(Microsoft.VisualBasic.CompilerServices.Operators.CompareString("test", "TEST", false), item.d, "item.d");
                Assert.IsTrue(item.e, "item.e");
            }
        }

        [TestMethod]
        public void MultiLevelSelectOfAnonymousShouldSupport_CS()
        {
            // Client Projections : Collection.Select(anonymous).Select(anonymous) throws NotSupportedException
            string xml = FeedStart + "</feed>";
            var q = from entity in this.context.CreateQuery<northwindClient.Customers>("EntitySet")
                    select new
                    {
                        MyCollection = entity.Orders.Select(
                            nav => new
                            {
                                ID = nav.OrderID,
                                DataField = nav.OrderDate,
                            }
                        ).Select(second => new { ID = second.ID })
                    };
            Exception exception = TestUtil.RunCatching(() =>
                 {
                     foreach (var item in CreateTestMaterializeAtom(xml, q))
                     {
                     }
                 });
            TestUtil.AssertExceptionExpected(exception, true);
            TestUtil.AssertContains(exception.ToString(), "NotSupportedException");
        }

        [TestMethod]
        public void ToListShouldWork_CS()
        {
            // Client Projections : Cannot Call ToList on entity(Key)/Collection(1)/Collection when  entity(Key)/Collection(1) is the Query root
            {
                Trace.WriteLine("Primitive property in SelectMany");
                string xml = FeedStart + "</feed>";
                var q = from entity in this.context.CreateQuery<northwindClient.Customers>("EntitySet")
                        where entity.CustomerID == "1"
                        from nav in entity.Orders
                        select new { id = nav.OrderID };
                string target = q.ToString();
                Assert.AreEqual("http://localhost/EntitySet('1')/Orders?$select=OrderID", target, "q.ToString()");
                foreach (var item in CreateTestMaterializeAtom(xml, q))
                {
                }
            }

            {
                Trace.WriteLine("Resource set reference in SelectMany");
                string xml = FeedStart + "</feed>";
                var q = from entity in this.context.CreateQuery<northwindClient.Customers>("EntitySet")
                        where entity.CustomerID == "1"
                        from nav in entity.Orders
                        select new { MyCollection = nav.Order_Details.ToList() };
                string target = q.ToString();
                Assert.AreEqual("http://localhost/EntitySet('1')/Orders?$expand=Order_Details", target, "q.ToString()");
                foreach (var item in CreateTestMaterializeAtom(xml, q))
                {
                }
            }
        }

        [TestMethod]
        public void TestNewTypedEntityOrAnonymous()
        {
            string emptyLink = Link(true, "Member", "");
            string xml = FeedStart + AnyEntry("e1", "<m:ID>1</m:ID>", emptyLink);
            {
                Trace.WriteLine("Using entity type.");
                Exception exception = TestUtil.RunCatching(() =>
                    {
                        var q = from t in this.context.CreateQuery<TypedEntity<int, TypedEntity<int, int>>>("T")
                                select new { the_member = new { ID = t.Member.ID, Member = t.Member.Member } };
                        foreach (var item in CreateTestMaterializeAtom(xml, q))
                        {
                        }
                    });
                TestUtil.AssertExceptionExpected(exception, true);
                TestUtil.AssertContains(exception.ToString(), "NullReferenceException");
                TestUtil.AssertContains(exception.ToString(), "check for a null value");
            }

            {
                Trace.WriteLine("Using anonymous type.");
                Exception exception = TestUtil.RunCatching(() =>
                {
                    var q = from t in this.context.CreateQuery<TypedEntity<int, TypedEntity<int, int>>>("T")
                            select new { the_member = new TypedEntity<int, int>() { ID = t.Member.ID, Member = t.Member.Member } };
                    foreach (var item in CreateTestMaterializeAtom(xml, q))
                    {
                    }
                });
                TestUtil.AssertExceptionExpected(exception, true);
                TestUtil.AssertContains(exception.ToString(), "NullReferenceException");
                TestUtil.AssertContains(exception.ToString(), "check for a null value");
            }
        }

        [TestMethod]
        public void SelectNestedAfterProjection()
        {
            // Client Projections: select many nested, query options after projection
            string xml = EmptyFeed;
            {
                Trace.WriteLine("Projection throught .Select()");
                var q = from c in this.context.CreateQuery<northwindClient.Customers>("Customers").Expand("Orders")
                        select new northwindClient.Customers
                        {
                            CustomerID = c.CustomerID
                        };
                Exception exception = TestUtil.RunCatching(() => { foreach (var item in CreateTestMaterializeAtom(xml, q)) { } });
                TestUtil.AssertExceptionExpected(exception, true);
                TestUtil.AssertContains(exception.ToString(), "NotSupportedException");
            }

            {
                Trace.WriteLine("Projection throught .SelectMany()");
                var q = from c in this.context.CreateQuery<northwindClient.Customers>("Customers").Expand("Order_Details")
                        where c.CustomerID == "ALFKI"
                        from o in c.Orders
                        select new northwindClient.Orders
                        {
                            OrderID = o.OrderID
                        };
                Exception exception = TestUtil.RunCatching(() => { foreach (var item in CreateTestMaterializeAtom(xml, q)) { } });
                TestUtil.AssertExceptionExpected(exception, true);
                TestUtil.AssertContains(exception.ToString(), "NotSupportedException");
            }
        }

        [TestMethod]
        public void QueryExpandInExpand_CS()
        {
            string detailsXml = FeedStart + AnyEntry("od2", "<d:OrderID>1</d:OrderID>", null) + "</feed>";
            string orderXml = AnyEntry("o1", "<d:OrderID>1</d:OrderID>", Link(false, "Order_Details", detailsXml));
            string topXml = AnyEntry("od1", "<d:OrderID>1</d:OrderID>", Link(true, "Orders", orderXml));
            string xml = FeedStart + topXml + "</feed>";
            var q = from c in this.context.CreateQuery<northwindClient.Customers>("Customers")
                    where c.CustomerID == "ALFKI"
                    from o in c.Orders
                    where o.OrderID == 123
                    from od in o.Order_Details
                    select new
                    {
                        OrderID = od.OrderID,
                        Details = od.Orders.Order_Details.ToList()
                    };
            string uri = q.ToString();
            Assert.AreEqual("http://localhost/Customers('ALFKI')/Orders(123)/Order_Details?$expand=Orders($expand=Order_Details)&$select=OrderID", uri, "uri");
            var response = CreateTestMaterializeAtom(EmptyFeed, q);
            foreach (var item in response)
            {
                Assert.IsNotNull(item, "item");
                Assert.AreEqual(1, item.OrderID, "item.OrderID");
                Assert.IsNotNull(item.Details, "item.Details");
                Assert.AreEqual(1, item.Details.Count, "item.Details.Count");
                Assert.AreEqual(1, item.Details.Single().OrderID, "item.Details.Single().OrderID");
            }
        }

        [TestMethod]
        public void TestDoubleMemberTypedEntity()
        {
            {
                Trace.WriteLine("Testing on entities...");
                string member1 = AnyEntry("n1", "<d:ID>11</d:ID>", null);
                string xml1 = AnyEntry("e1", "<d:ID>10</d:ID>", Link(true, "Member", member1));
                string xml = FeedStart + xml1 + "</feed>";
                var q = from t in this.context.CreateQuery<DoubleMemberTypedEntity<int, DoubleMemberTypedEntity<int, string, int>, string>>("T")
                        select new TypedEntity<int, TypedEntity<int, int>>
                        {
                            ID = t.ID,
                            Member =
                            (t.Member == null) ? null : new TypedEntity<int, int>()
                            {
                                ID = t.Member.ID
                            }
                        };
                foreach (var item in CreateTestMaterializeAtom(xml, q))
                {
                    Assert.IsNotNull(item, "item");
                    Assert.AreEqual(10, item.ID, "item.ID");
                    Assert.IsNotNull(item.Member, "item.Member");

                    Assert.AreEqual(11, item.Member.ID, "item.Member.ID");
                }
            }

            this.ClearContext();

            {
                Trace.WriteLine("Testing on entities (with null)");
                string xml1 = AnyEntry("e1", "<d:ID>10</d:ID>", Link(true, "Member", null));
                string xml = FeedStart + xml1 + "</feed>";
                var q = from t in this.context.CreateQuery<DoubleMemberTypedEntity<int, DoubleMemberTypedEntity<int, string, int>, string>>("T")
                        select new TypedEntity<int, TypedEntity<int, int>>
                        {
                            ID = t.ID,
                            Member =
                            (t.Member == null) ? null : new TypedEntity<int, int>()
                            {
                                ID = t.Member.ID
                            }
                        };
                foreach (var item in CreateTestMaterializeAtom(xml, q))
                {
                    Assert.IsNotNull(item, "item");
                    Assert.AreEqual(10, item.ID, "item.ID");
                    Assert.IsNull(item.Member, "item.Member");
                }
            }

            this.ClearContext();

            {
                Trace.WriteLine("Testing on collections (with null)...");
                string xml1 = AnyEntry("e1", "<d:ID>10</d:ID>", Link(false, "Member", null));
                string xml = FeedStart + xml1 + "</feed>";
                var q = from t in this.context.CreateQuery<DoubleMemberTypedEntity<int, DataServiceCollection<DoubleMemberTypedEntity<int, string, int>>, string>>("T")
                        select new TypedEntity<int, DataServiceCollection<TypedEntity<int, int>>>
                        {
                            ID = t.ID,
                            Member = (t.Member == null) ? null : new DataServiceCollection<TypedEntity<int, int>>(
                                t.Member.Select(m => new TypedEntity<int, int>() { ID = m.ID })
                            )
                        };
                foreach (var item in CreateTestMaterializeAtom(xml, q))
                {
                    Assert.IsNotNull(item, "item");
                    Assert.AreEqual(10, item.ID, "item.ID");
                    Assert.IsNotNull(item.Member, "item.Member");
                    Assert.AreEqual(0, item.Member.Count, "item.Member.Count");
                }
            }
        }

        [TestMethod]
        public void TestTrimEnd_CS()
        {
            string topXml = AnyEntry("od1", "<d:ContactName xml:space='preserve'> howdy </d:ContactName>", null);
            string xml = FeedStart + topXml + "</feed>";
            var q = from customer in this.context.CreateQuery<northwindClient.Customers>("Customers")
                    select new
                    {
                        the_name = customer.ContactName.TrimEnd()
                    };
            foreach (var item in CreateTestMaterializeAtom(xml, q))
            {
                Assert.IsNotNull(item, "item");
            }
        }

        [TestMethod]
        public void ChangeLevelOfKeyPredicateShouldBeSupported_CS()
        {
            // Client Linq + VB : NotSupportedException thrown when Level of Key Predicate expression is changed
            string xml = FeedStart + AnyEntry("od1", "<d:ProductID>123</d:ProductID>", null) + "</feed>";
            var q = from customer in context.CreateQuery<northwindClient.Customers>("Customers")
                    from order in customer.Orders
                    from order_Detail in order.Order_Details
                    where order.OrderID == 10643
                    where customer.CustomerID == "ALFKI"
                    select order_Detail;
            foreach (var item in CreateTestMaterializeAtom(xml, q))
            {
                Assert.IsNotNull(item, "item");
                Assert.AreEqual(123, item.ProductID, "item.ProductID");
            }
        }

        [TestMethod]
        public void TestContinuationOfDirectAndProjectedMaterialization()
        {
            string nestedFeed = FeedStart +
                AnyEntry("e2", "<d:ID>100</d:ID>", null) +
                "</feed>";
            string xml = FeedStart + AnyEntry("e1", "<d:ID>1</d:ID>", Link(false, "Member", nestedFeed)) + "</feed>";
            {
                Trace.WriteLine("Tracking through direct materialization.");
                var q = from t in this.context.CreateQuery<TypedEntity<int, List<TypedEntity<int, int>>>>("T")
                        select t;
                var response = AtomParserTests.CreateQueryResponse(this.context, AtomParserTests.EmptyHeaders, q, xml);
                foreach (var item in response)
                {
                    Assert.IsNotNull(item, "item");
                    Assert.AreEqual(1, item.ID, "item.ID");
                    Assert.IsNotNull(item.Member, "item.Member");

                    Assert.AreEqual(1, item.Member.Count, "item.Member.Count");
                    Assert.AreEqual(100, item.Member.Single().ID, "item.Member.Single().Member");
                    Assert.IsNull(response.GetContinuation(item.Member), "response.GetContinuation(item.Member)");
                }

                response = AtomParserTests.CreateQueryResponse(this.context, AtomParserTests.EmptyHeaders, q, xml);
                foreach (var item in response)
                {
                    Assert.IsNotNull(item, "item");
                    Assert.AreEqual(1, item.ID, "item.ID");
                    Assert.IsNotNull(item.Member, "item.Member");

                    Assert.AreEqual(1, item.Member.Count, "item.Member.Count");
                    Assert.AreEqual(100, item.Member.Single().ID, "item.Member.Single().Member");
                    Assert.IsNull(response.GetContinuation(item.Member), "response.GetContinuation(item.Member)");
                }
            }

            this.ClearContext();

            {
                Trace.WriteLine("Tracking through projected materialization.");
                var q = from t in this.context.CreateQuery<TypedEntity<int, List<TypedEntity<int, int>>>>("T")
                        select new TypedEntity<int, List<TypedEntity<int, int>>>()
                        {
                            ID = t.ID,
                            Member = t.Member,
                        };
                var response = AtomParserTests.CreateQueryResponse(this.context, AtomParserTests.EmptyHeaders, q, xml);
                foreach (var item in response)
                {
                    Assert.IsNotNull(item, "item");
                    Assert.AreEqual(1, item.ID, "item.ID");
                    Assert.IsNotNull(item.Member, "item.Member");

                    Assert.AreEqual(1, item.Member.Count, "item.Member.Count");
                    Assert.AreEqual(100, item.Member.Single().ID, "item.Member.Single().Member");
                    Assert.IsNull(response.GetContinuation(item.Member), "response.GetContinuation(item.Member)");
                }

                response = AtomParserTests.CreateQueryResponse(this.context, AtomParserTests.EmptyHeaders, q, xml);
                foreach (var item in response)
                {
                    Assert.IsNotNull(item, "item");
                    Assert.AreEqual(1, item.ID, "item.ID");
                    Assert.IsNotNull(item.Member, "item.Member");

                    Assert.AreEqual(1, item.Member.Count, "item.Member.Count");
                    Assert.AreEqual(100, item.Member.Single().ID, "item.Member.Single().Member");
                    Assert.IsNull(response.GetContinuation(item.Member), "response.GetContinuation(item.Member)");
                }
            }

            this.ClearContext();

            {
                Trace.WriteLine("Tracking through projected materialization with DSC.");
                var q = from t in this.context.CreateQuery<TypedEntity<int, List<TypedEntity<int, int>>>>("T")
                        select new TypedEntity<int, DataServiceCollection<TypedEntity<int, int>>>
                        {
                            ID = t.ID,
                            Member = new DataServiceCollection<TypedEntity<int, int>>(t.Member)
                        };
                var response = AtomParserTests.CreateQueryResponse(this.context, AtomParserTests.EmptyHeaders, q, xml);
                foreach (var item in response)
                {
                    Assert.IsNotNull(item, "item");
                    Assert.AreEqual(1, item.ID, "item.ID");
                    Assert.IsNotNull(item.Member, "item.Member");

                    Assert.AreEqual(1, item.Member.Count, "item.Member.Count");
                    Assert.AreEqual(100, item.Member.Single().ID, "item.Member.Single().Member");
                    Assert.IsNull(response.GetContinuation(item.Member), "response.GetContinuation(item.Member)");
                }

                response = AtomParserTests.CreateQueryResponse(this.context, AtomParserTests.EmptyHeaders, q, xml);
                foreach (var item in response)
                {
                    Assert.IsNotNull(item, "item");
                    Assert.AreEqual(1, item.ID, "item.ID");
                    Assert.IsNotNull(item.Member, "item.Member");

                    Assert.AreEqual(1, item.Member.Count, "item.Member.Count");
                    Assert.AreEqual(100, item.Member.Single().ID, "item.Member.Single().Member");
                    Assert.IsNull(response.GetContinuation(item.Member), "response.GetContinuation(item.Member)");
                }
            }

            this.ClearContext();

            {
                Trace.WriteLine("Tracking through projected materialization with DSC (with subselect).");
                var q = from t in this.context.CreateQuery<TypedEntity<int, List<TypedEntity<int, int>>>>("T")
                        select new TypedEntity<int, DataServiceCollection<TypedEntity<int, int>>>
                        {
                            ID = t.ID,
                            Member = new DataServiceCollection<TypedEntity<int, int>>(t.Member.Select(m => new TypedEntity<int, int>() { ID = m.ID }))
                        };
                var response = AtomParserTests.CreateQueryResponse(this.context, AtomParserTests.EmptyHeaders, q, xml);
                foreach (var item in response)
                {
                    Assert.IsNotNull(item, "item");
                    Assert.AreEqual(1, item.ID, "item.ID");
                    Assert.IsNotNull(item.Member, "item.Member");

                    Assert.AreEqual(1, item.Member.Count, "item.Member.Count");
                    Assert.AreEqual(100, item.Member.Single().ID, "item.Member.Single().Member");
                    Assert.IsNull(response.GetContinuation(item.Member), "response.GetContinuation(item.Member)");
                }

                response = AtomParserTests.CreateQueryResponse(this.context, AtomParserTests.EmptyHeaders, q, xml);
                foreach (var item in response)
                {
                    Assert.IsNotNull(item, "item");
                    Assert.AreEqual(1, item.ID, "item.ID");
                    Assert.IsNotNull(item.Member, "item.Member");

                    Assert.AreEqual(1, item.Member.Count, "item.Member.Count");
                    Assert.AreEqual(100, item.Member.Single().ID, "item.Member.Single().Member");
                    Assert.IsNull(response.GetContinuation(item.Member), "response.GetContinuation(item.Member)");
                }
            }

            this.ClearContext();

            {
                Trace.WriteLine("Tracking through projected materialization with DSC (anon).");
                var q = from t in this.context.CreateQuery<TypedEntity<int, List<TypedEntity<int, int>>>>("T")
                        select new
                        {
                            Member = new DataServiceCollection<TypedEntity<int, int>>(t.Member.Select(m => new TypedEntity<int, int>() { ID = m.ID }))
                        };
                var response = AtomParserTests.CreateQueryResponse(this.context, AtomParserTests.EmptyHeaders, q, xml);
                foreach (var item in response)
                {
                    Assert.IsNotNull(item, "item");
                    Assert.IsNotNull(item.Member, "item.Member");
                    Assert.AreEqual(1, item.Member.Count, "item.Member.Count");
                    Assert.AreEqual(100, item.Member.Single().ID, "item.Member.Single().Member");
                    Assert.IsNull(response.GetContinuation(item.Member), "response.GetContinuation(item.Member)");
                    Assert.IsNull(item.Member.Continuation, "item.Member.Continuation");
                }
            }
        }

        [TestMethod]
        public void OrderingNavigatedCollectionOfMultiplePropertiesShouldWork()
        {
            // Client: ordering navigated collection by more than one property throws exception
            string xml = FeedStart + AnyEntry("o1", "<d:ID>1</d:ID>", null) + "</feed>";

            {
                var q = from c in this.context.CreateQuery<Customer>("C")
                        where c.ID == 1
                        from o in c.Orders
                        orderby o.ID, o.CurrencyAmount
                        select o;
                foreach (var item in CreateTestMaterializeAtom(xml, q))
                {
                    Assert.AreEqual(1, item.ID, "item.ID");
                }
                Assert.AreEqual("http://localhost/C(1)/Orders?$orderby=ID,CurrencyAmount", q.ToString());
            }

            {
                var q = from c in this.context.CreateQuery<Customer>("C")
                        where c.ID == 1
                        from o in c.Orders
                        orderby o.ID, o.CurrencyAmount, o.Customer.ID
                        select o;
                foreach (var item in CreateTestMaterializeAtom(xml, q))
                {
                    Assert.AreEqual(1, item.ID, "item.ID");
                }
                Assert.AreEqual("http://localhost/C(1)/Orders?$orderby=ID,CurrencyAmount,Customer/ID", q.ToString());
            }

            {
                var q = from c in this.context.CreateQuery<Customer>("C")
                        where c.ID == 1
                        from o in c.Orders
                        orderby o.ID descending, o.CurrencyAmount descending, o.Customer.ID
                        select o;
                foreach (var item in CreateTestMaterializeAtom(xml, q))
                {
                    Assert.AreEqual(1, item.ID, "item.ID");
                }
                Assert.AreEqual("http://localhost/C(1)/Orders?$orderby=ID desc,CurrencyAmount desc,Customer/ID", q.ToString());
            }
        }

        #endregion Dev10 TFS bugs.

        #region MemberAssignmentAnalysis tests.

        public class MemberAssignmentAnalysisVisitMemberInit_CustomerWithFields
        {
            public int ID { get; set; }
            public string Name;
            public MemberAssignmentAnalysisVisitMemberInit_CustomerWithFields BestFriend;
        }

        [TestMethod]
        public void MemberAssignmentAnalysisVisitMemberInit()
        {
            string friend = AnyEntry("f1", "<d:Name>Best Friend</d:Name>", null);
            string xml = FeedStart + AnyEntry("c1", "<d:ID>1</d:ID>", Link(true, "BestFriend", friend)) + "</feed>";

            {
                Trace.WriteLine("Not a MemberAssignment binding.");
            }

            {
                Trace.WriteLine("No paths.");
                ParameterExpression c = Expression.Parameter(typeof(Customer), "c");
                var selector = Expression.Lambda<Func<Customer, Customer>>(
                    Expression.MemberInit(
                        Expression.New(typeof(Customer)),
                        Expression.Bind(typeof(Customer).GetProperty("ID"), Expression.MakeMemberAccess(c, typeof(Customer).GetProperty("ID"))),
                        Expression.Bind(typeof(Customer).GetProperty("BestFriend"),
                            Expression.MemberInit(Expression.New(typeof(Customer))))),
                    c);
                var q = this.context.CreateQuery<Customer>("C").Select(selector);
                //      Same tree/results as:
                //        select new Customer()
                //        {
                //            ID = c.ID,
                //            BestFriend = new Customer()
                //            {
                //            }
                //        };
                Exception e = TestUtil.RunCatching(() =>
                {
                    foreach (var item in CreateTestMaterializeAtom(xml, q))
                    {
                    }
                });
                TestUtil.AssertExceptionExpected(e, true);
                TestUtil.AssertContains(e.ToString(), "System.NotSupportedException: Initializing instances of the entity type AstoriaUnitTests.Stubs.Customer with the expression value(AstoriaUnitTests.Stubs.Customer) is not supported");
            }

            {
                Trace.WriteLine("Incompatible paths.");
                var q = from c in this.context.CreateQuery<Customer>("C")
                        select new Customer()
                        {
                            ID = c.ID,
                            BestFriend = new Customer()
                            {
                                ID = c.BestFriend.ID,
                                BestFriend = c.BestFriend.BestFriend.BestFriend
                            }
                        };
                Exception e = TestUtil.RunCatching(() =>
                {
                    foreach (var item in CreateTestMaterializeAtom(xml, q))
                    {
                    }
                });
                TestUtil.AssertExceptionExpected(e, true);
                TestUtil.AssertContains(e.ToString(), "Cannot initialize an instance of entity type 'AstoriaUnitTests.Stubs.Customer' because 'c.BestFriend' and 'c.BestFriend.BestFriend' do not refer to the same source entity");
            }

            {
                Trace.WriteLine("Multiple paths, with fields - analysis succeeds, materialization fails.");
                var q = from c in this.context.CreateQuery<Customer>("C")
                        select new MemberAssignmentAnalysisVisitMemberInit_CustomerWithFields()
                        {
                            ID = c.ID,
                            BestFriend = new MemberAssignmentAnalysisVisitMemberInit_CustomerWithFields()
                            {
                                Name = c.BestFriend.Name
                            }
                        };
                Exception e = TestUtil.RunCatching(() =>
                    {
                        foreach (var item in CreateTestMaterializeAtom(xml, q))
                        {
                            Assert.IsNotNull(item, "item");
                            Assert.AreEqual(1, item.ID, "item.ID");
                            Assert.IsNotNull(item.BestFriend, "item.BestFriend");
                            Assert.AreEqual("Best Friend", item.BestFriend.Name, "item.BestFriend.Name");
                        }
                    });
                TestUtil.AssertExceptionExpected(e, true);
                TestUtil.AssertContains(e.ToString(), "System.InvalidOperationException: The closed type AstoriaUnitTests.Tests.ProjectionTests+MemberAssignmentAnalysisVisitMemberInit_CustomerWithFields does not have a corresponding BestFriend settable property.");
            }
        }

        #endregion MemberAssignmentAnalysis tests.

        #region PatternRules tests.

        [TestMethod]
        public void PatternRulesMatchNullCheck()
        {
            const string exceptionTypeName = "NullReferenceException";
            string xml = FeedStart + AnyEntry("e1", "<d:ID>1</d:ID>", Link(true, "Member", "")) + "</feed>";

            this.context.MergeOption = MergeOption.NoTracking;
            {
                Trace.WriteLine("Accessing properties will throw an exception.");
                var q = from t in this.context.CreateQuery<TypedEntity<int, TypedEntity<int, int>>>("T")
                        select new TypedEntity<int, TypedEntity<int, int>>()
                        {
                            ID = t.ID,
                            Member = new TypedEntity<int, int>()
                            {
                                ID = t.Member.ID,
                                Member = t.Member.Member
                            }
                        };
                PatternRulesMatchNullCheck_Fail(xml, q, exceptionTypeName);
            }

            {
                Trace.WriteLine("Accessing properties can be guarded with a null check.");
                var q = from t in this.context.CreateQuery<TypedEntity<int, TypedEntity<int, int>>>("T")
                        select new TypedEntity<int, TypedEntity<int, int>>()
                        {
                            ID = t.ID,
                            Member = (t.Member == null) ? null : new TypedEntity<int, int>()
                            {
                                ID = t.Member.ID,
                                Member = t.Member.Member
                            }
                        };
                PatternRulesMatchNullCheck_Succeed(xml, q);
            }

            {
                Trace.WriteLine("Accessing properties can be guarded with a null check (not-equal variation).");
                var q = from t in this.context.CreateQuery<TypedEntity<int, TypedEntity<int, int>>>("T")
                        select new TypedEntity<int, TypedEntity<int, int>>()
                        {
                            ID = t.ID,
                            Member = (null != t.Member) ? new TypedEntity<int, int>()
                            {
                                ID = t.Member.ID,
                                Member = t.Member.Member
                            } : null
                        };
                PatternRulesMatchNullCheck_Succeed(xml, q);
            }

            {
                Trace.WriteLine("Only comparison to nulls are allowed.");
                var q = from t in this.context.CreateQuery<TypedEntity<int, TypedEntity<int, int>>>("T")
                        select new TypedEntity<int, TypedEntity<int, int>>()
                        {
                            ID = t.ID,
                            Member = (new TypedEntity<int, int>() { ID = t.Member.ID } == t.Member) ? new TypedEntity<int, int>()
                            {
                                ID = t.Member.ID,
                                Member = t.Member.Member
                            } : null
                        };
                PatternRulesMatchNullCheck_Fail(xml, q, "NotSupportedException");
            }

            {
                Trace.WriteLine("Only comparison to nulls are allowed (not-equal version).");
                var q = from t in this.context.CreateQuery<TypedEntity<int, TypedEntity<int, int>>>("T")
                        select new TypedEntity<int, TypedEntity<int, int>>()
                        {
                            ID = t.ID,
                            Member = (new TypedEntity<int, int>() { ID = t.Member.ID } != t.Member) ? new TypedEntity<int, int>()
                            {
                                ID = t.Member.ID,
                                Member = t.Member.Member
                            } : null
                        };
                PatternRulesMatchNullCheck_Fail(xml, q, "NotSupportedException");
            }

            {
                Trace.WriteLine("Can't create on both sides (not-equal version).");
                var q = from t in this.context.CreateQuery<TypedEntity<int, TypedEntity<int, int>>>("T")
                        select new TypedEntity<int, TypedEntity<int, int>>()
                        {
                            ID = t.ID,
                            Member = (null != t.Member) ? new TypedEntity<int, int>()
                            {
                                ID = t.Member.ID,
                                Member = t.Member.Member
                            } : new TypedEntity<int, int>() { ID = t.Member.ID }
                        };
                PatternRulesMatchNullCheck_Fail(xml, q, "NotSupportedException");
            }

            {
                Trace.WriteLine("Can't have a non-binary expression");
                var q = from t in this.context.CreateQuery<TypedEntity<int, TypedEntity<int, int>>>("T")
                        select new TypedEntity<int, TypedEntity<int, int>>()
                        {
                            ID = t.ID,
                            Member = object.ReferenceEquals(null, t.Member) ? null : new TypedEntity<int, int>()
                            {
                                ID = t.Member.ID,
                                Member = t.Member.Member
                            }
                        };
                PatternRulesMatchNullCheck_Succeed(xml, q);
            }

            {
                Trace.WriteLine("Only equal and not-equal allowed.");
                var q = from t in this.context.CreateQuery<TypedEntity<int, TypedEntity<int, int>>>("T")
                        select new TypedEntity<int, TypedEntity<int, int>>()
                        {
                            ID = t.ID,
                            Member = (t.Member.ID > 0) ? new TypedEntity<int, int>()
                            {
                                ID = t.Member.ID,
                                Member = t.Member.Member
                            } : null
                        };
                PatternRulesMatchNullCheck_Fail(xml, q, "NotSupportedException");
            }

            {
                Trace.WriteLine("Can't compare to things other than null.");
                var q = from t in this.context.CreateQuery<TypedEntity<int, TypedEntity<int, int>>>("T")
                        select new TypedEntity<int, TypedEntity<int, int>>()
                        {
                            ID = t.ID,
                            Member = (t.Member != t.Member) ? new TypedEntity<int, int>()
                            {
                                ID = t.Member.ID,
                                Member = t.Member.Member
                            } : null
                        };
                PatternRulesMatchNullCheck_Fail(xml, q, "NotSupportedException");
            }

            {
                Trace.WriteLine("Negation outside of check is recognized correctly");
                var q = from t in this.context.CreateQuery<TypedEntity<int, TypedEntity<int, int>>>("T")
                        select new TypedEntity<int, TypedEntity<int, int>>()
                        {
                            ID = t.ID,
                            Member = !(t.Member == null) ? new TypedEntity<int, int>()
                            {
                                ID = t.Member.ID,
                                Member = t.Member.Member
                            } : null
                        };
                PatternRulesMatchNullCheck_Succeed(xml, q);
            }

            {
                Trace.WriteLine("Negation outside of check is recognized correctly (double negation and != comparison)");
                var q = from t in this.context.CreateQuery<TypedEntity<int, TypedEntity<int, int>>>("T")
                        select new TypedEntity<int, TypedEntity<int, int>>()
                        {
                            ID = t.ID,
                            Member = !!(t.Member != null) ? new TypedEntity<int, int>()
                            {
                                ID = t.Member.ID,
                                Member = t.Member.Member
                            } : null
                        };
                PatternRulesMatchNullCheck_Succeed(xml, q);
            }

            {
                Trace.WriteLine("Many references in test expression (can't determine the correct source).");
                var q = from t in this.context.CreateQuery<TypedEntity<int, TypedEntity<int, int>>>("T")
                        select new TypedEntity<int, TypedEntity<int, int>>()
                        {
                            ID = t.ID,
                            Member = ((t.Member ?? t.Member) == null) ? null : new TypedEntity<int, int>()
                            {
                                ID = t.Member.ID,
                                Member = t.Member.Member
                            }
                        };
                PatternRulesMatchNullCheck_Fail(xml, q, "NotSupportedException");
            }

            {
                Trace.WriteLine("Many references in assignment expression (can't determine the correct source).");
                var q = from t in this.context.CreateQuery<TypedEntity<int, TypedEntity<int, int>>>("T")
                        select new TypedEntity<int, TypedEntity<int, int>>()
                        {
                            ID = t.ID,
                            Member = (t.Member == null) ? null : (t.Member ?? t.Member)
                        };
                PatternRulesMatchNullCheck_Fail(xml, q, "NotSupportedException");
            }

            {
                Trace.WriteLine("Assignment expression doesn't traverse through assigned expression.");
                var q = from t in this.context.CreateQuery<TypedEntity<int, TypedEntity<int, int>>>("T")
                        select new TypedEntity<int, TypedEntity<int, int>>()
                        {
                            ID = t.ID,
                            Member = (t.Member == null) ? null : new TypedEntity<int, int>()
                        };
                PatternRulesMatchNullCheck_Fail(xml, q, "NotSupportedException");
            }

            {
                Trace.WriteLine("Must traverse the same path down the projection tree (success).");
                var q = from t in this.context.CreateQuery<TypedEntity<int, TypedEntity<int, TypedEntity<int, int>>>>("T")
                        select new TypedEntity<int, TypedEntity<int, TypedEntity<int, int>>>()
                        {
                            ID = t.ID,
                            Member = (t.Member == null) ? null : new TypedEntity<int, TypedEntity<int, int>>()
                            {
                                ID = t.Member.ID,
                                Member = (t.Member.Member == null) ? null : new TypedEntity<int, int>()
                                {
                                    ID = t.Member.Member.ID,
                                    Member = t.Member.Member.Member
                                }
                            }
                        };
                foreach (var item in CreateTestMaterializeAtom(xml, q))
                {
                    Assert.IsNotNull(item, "item");
                    Assert.IsNull(item.Member, "item.Member");
                }
            }

            {
                Trace.WriteLine("Must traverse the same path down the projection tree (failure).");
                var q = from t in this.context.CreateQuery<DoubleMemberTypedEntity<int, TypedEntity<int, TypedEntity<int, int>>, TypedEntity<int, TypedEntity<int, int>>>>("T")
                        select new DoubleMemberTypedEntity<int, TypedEntity<int, TypedEntity<int, int>>, TypedEntity<int, TypedEntity<int, int>>>()
                        {
                            ID = t.ID,
                            Member = (t.Member == null) ? null : new TypedEntity<int, TypedEntity<int, int>>()
                            {
                                ID = t.Member.ID,
                                Member = (t.Member.Member == null) ? null : new TypedEntity<int, int>()
                                {
                                    ID = t.Member.Member.ID,
                                    Member = t.Member.Member.Member
                                }
                            },
                            Member2 = (t.Member2 == null) ? null : new TypedEntity<int, TypedEntity<int, int>>()
                            {
                                ID = t.Member2.ID,
                                Member = (t.Member2.Member == null) ? null : new TypedEntity<int, int>()
                                {
                                    ID = t.Member.Member.ID,
                                    Member = t.Member.Member.Member
                                }
                            }
                        };
                PatternRulesMatchNullCheck_Fail(xml, q, "NotSupportedException");
            }

            {
                Trace.WriteLine("No support for methods.");
                var q = from t in this.context.CreateQuery<TypedEntity<int, TypedEntity<int, int>>>("T")
                        select new TypedEntity<int, TypedEntity<int, int>>()
                        {
                            ID = t.ID,
                            Member = (t.Member == null) ? null : new TypedEntity<int, int>()
                            {
                                ID = t.Member.ID.GetHashCode()
                            }
                        };
                PatternRulesMatchNullCheck_Fail(xml, q, "NotSupportedException");
            }

            {
                Trace.WriteLine("Accessing properties can be guarded with a null check in anonymous types.");
                var q = from t in this.context.CreateQuery<TypedEntity<int, TypedEntity<int, int>>>("T")
                        select new
                        {
                            ID = t.ID,
                            Member = (t.Member == null) ? null : new TypedEntity<int, int>()
                            {
                                ID = t.Member.ID,
                                Member = t.Member.Member
                            }
                        };
                foreach (var item in CreateTestMaterializeAtom(xml, q))
                {
                    Assert.AreEqual(1, item.ID, "item.ID");
                    Assert.IsNull(item.Member, "item.Member");
                }
            }

            {
                Trace.WriteLine("Null check on anonymous types still works when non-param in scope.");
                var q = from t in this.context.CreateQuery<TypedEntity<int, TypedEntity<int, int>>>("T")
                        select new
                        {
                            ID = t.ID,
                            Member = new string[] { null, "abc" }.Select(a => a == null ? null : (int?)a.Length)
                        };
                foreach (var item in CreateTestMaterializeAtom(xml, q))
                {
                    Assert.AreEqual(1, item.ID, "item.ID");
                    Assert.IsNotNull(item.Member, "item.Member");
                    Assert.AreEqual(1, item.Member.Where(n => n == null).Count(), "single null value");
                }
            }
        }

        private void PatternRulesMatchNullCheck_Succeed(string xml, IQueryable<TypedEntity<int, TypedEntity<int, int>>> q)
        {
            foreach (var item in CreateTestMaterializeAtom(xml, q))
            {
                Assert.AreEqual(1, item.ID, "item.ID");
                Assert.IsNull(item.Member, "item.Member");
            }
        }

        private void PatternRulesMatchNullCheck_Fail<T>(string xml, IQueryable<T> q, string exceptionText)
        {
            Exception exception = TestUtil.RunCatching(() => { CreateTestMaterializeAtom(xml, q).ToList(); });
            TestUtil.AssertExceptionExpected(exception, true);
            TestUtil.AssertContains(exception.ToString(), exceptionText);
        }

        #endregion PatternRules tests.

        #region ProjectionGetEntity tests.

        [TestMethod]
        public void AtomMaterializerProjGetEntityTest()
        {
            string xml =
                FeedStart +
                TeamEntry("t1", "<d:TeamID>1</d:TeamID>") +
                "</feed>";
            var query = from t in context.CreateQuery<Team>("Teams")
                        select new Team() { TeamID = t.TeamID };
            foreach (var item in CreateTestMaterializeAtom(xml, query))
            {
                Assert.AreEqual(1, item.TeamID, "item.TeamID");
            }

            AssertEntityCount(1, "single team");
        }

        [TestMethod]
        public void AtomMaterializerCallReading()
        {
            string xml = FeedStart +
                TeamEntry("t1", "<d:TeamID>1</d:TeamID>") +
                "</feed>";

            var query = from t in context.CreateQuery<Team>("Teams")
                        select new { tid = t.TeamID, team = t };
            foreach (var item in CreateTestMaterializeAtom(xml, query))
            {
                Assert.AreEqual(1, item.tid, "item.TeamID");
            }

            AssertEntityCount(1, "single team");
        }

        [TestMethod]
        public void AtomMaterializerCallReadingDupeNs()
        {
            string xml = @"<?xml version='1.0' ?>
<feed xml:base='http://bpdtechfestjudging/ReproServer/Northwind.svc/' xmlns:d='http://docs.oasis-open.org/odata/ns/data' xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' xmlns='http://www.w3.org/2005/Atom'>
  <title type='text'>Products</title>
  <id>http://bpdtechfestjudging/ReproServer/Northwind.svc/Products</id>
  <link rel='self' title='Products' href='Products' />
  <entry>
    <id>http://bpdtechfestjudging/ReproServer/Northwind.svc/Products(1)</id>
    <link rel='edit' title='Products' href='Products(1)' />
    <link rel='http://docs.oasis-open.org/odata/ns/related/Categories' type='application/atom+xml;type=entry' title='Categories' href='Products(1)/Categories'>
      <m:inline>
        <entry>
          <id>http://bpdtechfestjudging/ReproServer/Northwind.svc/Categories(1)</id>
          <link rel='edit' title='Categories' href='Categories(1)' />
          <link rel='http://docs.oasis-open.org/odata/ns/related/Products' type='application/atom+xml;type=feed' title='Products' href='Categories(1)/Products'>
            <m:inline>
              <feed>
                <id>http://bpdtechfestjudging/ReproServer/Northwind.svc/Categories(1)/Products</id>
                <link rel='self' title='Products' href='Categories(1)/Products' />
                <entry>
                  <id>http://bpdtechfestjudging/ReproServer/Northwind.svc/Products(1)</id>
                  <link rel='edit' title='Products' href='Products(1)' />
                  <link rel='http://docs.oasis-open.org/odata/ns/related/Categories' type='application/atom+xml;type=entry' title='Categories' href='Products(1)/Categories'>
                    <m:inline>
                      <entry>
                        <id>http://bpdtechfestjudging/ReproServer/Northwind.svc/Categories(1)</id>
                        <link rel='edit' title='Categories' href='Categories(1)' />
                        <link rel='http://docs.oasis-open.org/odata/ns/related/Products' type='application/atom+xml;type=feed' title='Products' href='Categories(1)/Products' />
                        <category term='NorthwindModel.Categories' scheme='http://docs.oasis-open.org/odata/ns/scheme' />
                        <content type='application/xml'>
                          <m:properties>
                            <d:CategoryID m:type='Edm.Int32'>1</d:CategoryID>
                            <d:CategoryName>Beverages</d:CategoryName>
                            <d:Description>Soft drinks, coffees, teas, beers, and ales</d:Description>
                          </m:properties>
                        </content>
                      </entry>
                    </m:inline>
                  </link>
                  <category term='NorthwindModel.Products' scheme='http://docs.oasis-open.org/odata/ns/scheme' />
                  <content type='application/xml' />
                </entry>
              </feed>
            </m:inline>
          </link>
          <category term='NorthwindModel.Categories' scheme='http://docs.oasis-open.org/odata/ns/scheme' />
          <content type='application/xml' />
        </entry>
      </m:inline>
    </link>
    <category term='NorthwindModel.Products' scheme='http://docs.oasis-open.org/odata/ns/scheme' />
    <content type='application/xml'>
      <m:properties>
        <d:ProductName>Chai</d:ProductName>
      </m:properties>
    </content>
  </entry>
</feed>";
            XmlReader r0 = XmlReader.Create(new System.IO.StringReader(xml));
            while (r0.NodeType != XmlNodeType.Element) r0.Read();
            System.Xml.Linq.XElement e0 = System.Xml.Linq.XElement.Load(r0.ReadSubtree());
            System.Xml.Linq.XElement e1 = e0.Descendants().Where(e => e.Name.LocalName == "entry").First();

            XmlReader r1 = e1.CreateReader();
            while (r1.NodeType != XmlNodeType.Element) r1.Read();
            System.Xml.Linq.XElement e2 = System.Xml.Linq.XElement.Load(r1.ReadSubtree());
            System.Xml.Linq.XElement e3 = e2.Descendants().Where(e => e.Name.LocalName == "entry").First();

            XmlReader r3 = e3.CreateReader();
            while (r3.NodeType != XmlNodeType.Element) r3.Read();
            System.Xml.Linq.XElement e4 = System.Xml.Linq.XElement.Load(r3.ReadSubtree());

            Trace.WriteLine(e4.ToString());

            var customers = from prod in this.context.CreateQuery<northwindClient.Products>("Products")
                            where prod.Categories != null
                            select new
                            {
                                pn = prod.ProductName,
                                cats = from x in prod.Categories.Products select x.Categories
                            };

            foreach (var item in CreateTestMaterializeAtom(xml, customers))
            {
                Assert.IsNotNull(item, "item");
            }
        }

        [TestMethod]
        public void AtomMaterializerProjGetEntityNoIdTest()
        {
            const string id = "t1";
            const string properties = "<d:TeamID>1</d:TeamID>";
            const string teamNoId =
                "<entry><link rel='edit' href='" + id + "'/>" +
                "<content type='application/xml'><m:properties>" + properties +
                "</m:properties></content></entry>";
            const string xml = FeedStart + teamNoId + "</feed>";
            var query = from t in context.CreateQuery<Team>("Teams")
                        select new Team() { TeamID = t.TeamID };
            InvalidOperationException exception = (InvalidOperationException)TestUtil.RunCatching(() =>
                {
                    foreach (var item in CreateTestMaterializeAtom(xml, query))
                    {
                        Assert.AreEqual(1, item.TeamID, "item.TeamID");
                    }
                });
            TestUtil.AssertExceptionExpected(exception, true);
            TestUtil.AssertContains(exception.ToString(), "issing id");
        }

        [TestMethod]
        public void AtomMaterializerProjGetEntityWithNullsTest()
        {
            const string propertiesAllSet = "<d:ID>1</d:ID><d:NullableBoolean>true</d:NullableBoolean>" +
                "<d:NullableDateTimeOffset>2008-01-01T00:00:00Z</d:NullableDateTimeOffset>" +
                "<d:Int>10</d:Int><d:Long>10000</d:Long><d:Short>10</d:Short>" +
                "<d:NullableInt>10</d:NullableInt><d:NullableLong>10000</d:NullableLong><d:NullableShort>10</d:NullableShort>";
            const string propertiesWithNulls = "<d:ID>1</d:ID><d:NullableBoolean m:null='true' />" +
                "<d:NullableDateTimeOffset m:null='true'/>" +
                "<d:Int>10</d:Int><d:Long>10000</d:Long><d:Short>10</d:Short>" +
                "<d:NullableInt m:null='true' /><d:NullableLong m:null='true' /><d:NullableShort m:null='true' />";
            string[] properties = new string[] { propertiesAllSet, propertiesWithNulls };
            foreach (string payload in properties)
            {
                string xml = FeedStart +
                    "<entry><id>http://localhost/1</id>" +
                    "<content type='application/xml'><m:properties>" + payload +
                    "</m:properties></content></entry></feed>";
                context.MergeOption = MergeOption.NoTracking;

                Trace.WriteLine("Before nullables...");
                var query = from p in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                            select new BigCityVar1
                            {
                                ID = (int)(float)p.ID,
                                Int = (int)p.Int,
                                Long = (int)p.Long,
                                //Short = (int)p.Short,
                            };
                foreach (var item in CreateTestMaterializeAtom(xml, query))
                {
                }

                Trace.WriteLine("With nullables...");
                var q2 = from p in context.CreateQuery<YetAnotherAllPrimitiveTypesType>("Var1")
                         select new BigCityVar1
                         {
                             ID = (int)(float)p.ID,
                             Int = (int)p.Int,
                             Long = (int)p.Long,
                             Short = (int)p.Short,
                             NullableBoolean = (bool?)p.NullableBoolean,
                             NullableDateTimeOffset = p.NullableDateTimeOffset,
                             NullableInt = (long?)p.NullableInt,
                             NullableShort = (long?)p.NullableShort,
                             NullableLong = (long?)p.NullableLong
                         };
                foreach (var item in CreateTestMaterializeAtom(xml, q2))
                {
                }
            }
        }

        #endregion ProjectionGetEntity tests.

        #region ProjectionValueForPath tests.

        [TestMethod]
        public void AtomMaterializerProjectionValueForPathEntityTest()
        {
            string xml =
                FeedStart +
                TeamEntry("t1", "<d:TeamID>1</d:TeamID>") +
                "</feed>";
            var query = from t in context.CreateQuery<Team>("Teams")
                        select new { team = t, teamid = t.TeamID };
            foreach (var item in CreateTestMaterializeAtom(xml, query))
            {
                Assert.AreEqual(1, item.team.TeamID, "item.team.TeamID");
                Assert.AreEqual(1, item.teamid, "item.teamid");
            }

            AssertEntityCount(1, "single team");
        }

        [TestMethod]
        public void AtomMaterializerProjectionValueForPathEntityNullTest()
        {
            string xml =
                FeedStart +
                TeamEntry("t1", "<d:TeamID>1</d:TeamID>", Link(true, "HomeStadium", null)) +
                "</feed>";
            var query = from t in context.CreateQuery<Team>("Teams")
                        select new { team = t, teamid = t.TeamID, stadium = t.HomeStadium };
            foreach (var item in CreateTestMaterializeAtom(xml, query))
            {
                Assert.AreEqual(1, item.team.TeamID, "item.team.TeamID");
                Assert.AreEqual(1, item.teamid, "item.teamid");
                Assert.IsNull(item.stadium, "item.stadium");
            }

            AssertEntityCount(1, "single team");
            AssertLinkCount(0, "because of shallow materialization, link with null home stadium value not expected");
        }

        [TestMethod]
        public void AtomMaterializerProjectionValueForPathEntityRepeatTest()
        {
            string xml =
                FeedStart +
                TeamEntry("t1", "<d:TeamID>1</d:TeamID>") +
                "</feed>";
            var query = from t in context.CreateQuery<Team>("Teams")
                        select new { teamid = t.TeamID, team = t, teamid2 = t.TeamID, team2 = t };
            foreach (var item in CreateTestMaterializeAtom(xml, query))
            {
                Assert.AreEqual(1, item.team.TeamID, "item.team.TeamID");
                Assert.AreEqual(1, item.team2.TeamID, "item.team.TeamID");
                Assert.AreEqual(1, item.teamid, "item.teamid");
                Assert.AreEqual(1, item.teamid2, "item.teamid");
                Assert.AreSame(item.team, item.team2, "item.team and item.item2");
            }

            AssertEntityCount(1, "single team");
        }

        [TestMethod]
        public void AtomMaterializerProjectionValueForPathFeedTest()
        {
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "Players");
            string playersXml =
                FeedStart +
                PlayerEntry("p1", "<d:ID>10</d:ID>") +
                PlayerEntry("p2", "<d:ID>20</d:ID>") +
                "</feed>";
            string xml =
                FeedStart +
                TeamEntry("t1", "<d:TeamID>1</d:TeamID>", Link(false, "Players", playersXml)) +
                "</feed>";
            var query = from t in context.CreateQuery<Team>("Teams")
                        select new { teamid = t.TeamID, team = new Team() { TeamID = t.TeamID, Players = t.Players }, players = t.Players };
            foreach (var item in CreateTestMaterializeAtom(xml, query))
            {
                Assert.AreEqual(1, item.team.TeamID, "item.team.TeamID");
                Assert.AreEqual(1, item.teamid, "item.teamid");
                Assert.AreEqual(2, item.players.Count, "item.players.Count");
                Assert.AreEqual(10, item.players[0].ID, "item.players[0].ID");
                Assert.AreEqual(20, item.players[1].ID, "item.players[1].ID");
                Assert.AreSame(item.players[0], item.team.Players[0]);
                Assert.AreSame(item.players[1], item.team.Players[1]);
            }

            AssertEntityCount(3, "single team, two players");
            AssertLinkCount(2, "links from team to each player");
        }

        [TestMethod]
        public void AtomMaterializerProjectionValueForPathFeedWithCollectionTest()
        {
            string teams = FeedStart +
                TeamEntry("t1", "<d:TeamID>1</d:TeamID>") +
                TeamEntry("t2", "<d:TeamID>2</d:TeamID>") +
                "</feed>";
            string xml = FeedStart +
                AnyEntry("te1", "<d:ID>1</d:ID>", Link(false, "Member", teams)) +
                "</feed>";
            var q = from t in this.context.CreateQuery<TypedEntity<int, ObservableCollection<Team>>>("T")
                    select new
                    {
                        id = t.ID,
                        members = t.Member,
                        copy = new TypedEntity<int, ObservableCollection<Team>>()
                        {
                            ID = t.ID,
                            Member = t.Member
                        }
                    };

            foreach (var item in CreateTestMaterializeAtom(xml, q))
            {
                Assert.AreEqual(1, item.id, "item.id");
                Assert.IsNotNull(item.members, "item.members");
                Assert.IsNotNull(item.copy, "item.copy");

                Assert.AreEqual(2, item.members.Count, "item.members.Count");
                Assert.AreEqual(1, item.members[0].TeamID, "item.members[0].TeamID");
                Assert.AreEqual(2, item.members[1].TeamID, "item.members[1].TeamID");

                Assert.AreEqual(1, item.copy.ID, "item.copy.ID");
                Assert.AreEqual(1, item.copy.Member[0].TeamID, "item.copy.Member[0].TeamID");
                Assert.AreEqual(2, item.copy.Member[1].TeamID, "item.copy.Member[1].TeamID");
            }

            AssertEntityCount(3, "typed entity + 2 teams");
            AssertLinkCount(2, "typed entity to each team");

            foreach (var item in CreateTestMaterializeAtom(xml, q))
            {
            }

            AssertEntityCount(3, "typed entity + 2 teams");
            AssertLinkCount(2, "typed entity to each team");

            this.context.MergeOption = MergeOption.OverwriteChanges;
            foreach (var item in CreateTestMaterializeAtom(xml, q))
            {
            }

            AssertEntityCount(3, "typed entity + 2 teams");
            AssertLinkCount(2, "typed entity to each team");
        }

        [TestMethod]
        public void AtomMaterializerProjectionValueForPathFeedWithListTest()
        {
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "Players");
            string playersXml =
                FeedStart +
                PlayerEntry("p1", "<d:ID>10</d:ID>") +
                "</feed>";
            string xml =
                FeedStart +
                TeamEntry("t1", "<d:TeamID>1</d:TeamID>", Link(false, "Players", playersXml)) +
                "</feed>";
            var query = from t in context.CreateQuery<Team>("Teams")
                        select new
                        {
                            teamid = t.TeamID,
                            players = t.Players.ToList(),
                            players_again = t.Players,
                            players_more = t.Players
                        };
            foreach (var item in CreateTestMaterializeAtom(xml, query))
            {
                Assert.AreEqual(1, item.teamid, "item.teamid");
                Assert.AreEqual(1, item.players.Count, "item.players.Count");
                Assert.AreEqual(10, item.players[0].ID, "item.players[0].ID");
                Assert.AreEqual(item.players_again.Count, item.players.Count);
                Assert.AreEqual(item.players_more.Count, item.players_more.Count);
                Assert.AreSame(item.players_again[0], item.players[0]);
                Assert.AreSame(item.players_more[0], item.players[0]);
            }

            AssertEntityCount(1, "single player");
            AssertLinkCount(0, "no links if the parent isn't instantiated");
        }

        [TestMethod]
        public void AtomMaterializerProjectionValueForPathFeedWithMyCollectionTest()
        {
            string teams = FeedStart +
                TeamEntry("t1", "<d:TeamID>1</d:TeamID>") +
                TeamEntry("t2", "<d:TeamID>2</d:TeamID>") +
                "</feed>";
            string xml = FeedStart +
                AnyEntry("te1", "<d:ID>1</d:ID>", Link(false, "Member", teams)) +
                "</feed>";
            var q = from t in this.context.CreateQuery<TypedEntity<int, MyTeamCollection>>("T")
                    select new
                    {
                        id = t.ID,
                        members = t.Member,
                        copy = new TypedEntity<int, MyTeamCollection>()
                        {
                            ID = t.ID,
                            Member = t.Member
                        }
                    };

            foreach (var item in CreateTestMaterializeAtom(xml, q))
            {
                Assert.AreEqual(1, item.id, "item.id");
                Assert.IsNotNull(item.members, "item.members");
                Assert.IsNotNull(item.copy, "item.copy");

                Assert.AreEqual(2, item.members.Count, "item.members.Count");
                Assert.AreEqual(1, item.members[0].TeamID, "item.members[0].TeamID");
                Assert.AreEqual(2, item.members[1].TeamID, "item.members[1].TeamID");

                Assert.AreEqual(1, item.copy.ID, "item.copy.ID");
                Assert.AreEqual(1, item.copy.Member[0].TeamID, "item.copy.Member[0].TeamID");
                Assert.AreEqual(2, item.copy.Member[1].TeamID, "item.copy.Member[1].TeamID");
            }

            AssertEntityCount(3, "typed entity + 2 teams");
            AssertLinkCount(2, "typed entity to each team");
        }

        [TestMethod]
        public void AtomMaterializerProjectionValueForPathFeedWithMyCollectionNoCtorTest()
        {
            string teams = FeedStart +
                TeamEntry("t1", "<d:TeamID>1</d:TeamID>") +
                TeamEntry("t2", "<d:TeamID>2</d:TeamID>") +
                "</feed>";
            string xml = FeedStart +
                AnyEntry("te1", "<d:ID>1</d:ID>", Link(false, "Member", teams)) +
                "</feed>";
            Exception exception = TestUtil.RunCatching(() =>
                {
                    var q2 = from t in this.context.CreateQuery<TypedEntity<int, MyTeamCollectionNoCtor>>("T")
                             select t;
                    foreach (var item in CreateTestMaterializeAtom(xml, q2))
                    {
                    }
                });
            TestUtil.AssertExceptionExpected(exception, true);
            TestUtil.AssertContains(exception.ToString().ToLowerInvariant(), "no parameterless constructor");
        }

        [TestMethod]
        public void AtomMaterializerProjectionValueForPathFeedWithProjectTest()
        {
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "Players");
            string playersXml =
                FeedStart +
                PlayerEntry("p1", "<d:ID>10</d:ID><d:FirstName>first1</d:FirstName><d:Lastname>last1</d:Lastname>") +
                PlayerEntry("p2", "<d:ID>20</d:ID><d:FirstName>first2</d:FirstName><d:Lastname>last2</d:Lastname>") +
                "</feed>";
            string xml =
                FeedStart +
                TeamEntry("t1", "<d:TeamID>1</d:TeamID>", Link(false, "Players", playersXml)) +
                "</feed>";
            var query = from t in context.CreateQuery<Team>("Teams")
                        select new { teamid = t.TeamID, players = t.Players.Select(p => new { p.FirstName, p.Lastname }) };
            foreach (var item in CreateTestMaterializeAtom(xml, query))
            {
                Assert.AreEqual(1, item.teamid, "item.teamid");
                Assert.AreEqual(2, item.players.Count(), "item.players.Count()");
                Assert.AreEqual("first1", item.players.First().FirstName, "item.players.First().FirstName");
                Assert.AreEqual("first2", item.players.Last().FirstName, "item.players.Last().FirstName");
            }

            AssertEntityCount(0, "no entities if none are as-is projected");
            AssertLinkCount(0, "no links if the parent isn't instantiated");
        }

        [TestMethod]
        public void AtomMaterializerProjectionValueForPathFeedWithProjectEntityTest()
        {
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "Players");
            string playersXml =
                FeedStart +
                PlayerEntry("p1", "<d:ID>10</d:ID><d:FirstName>first1</d:FirstName><d:Lastname>last1</d:Lastname>") +
                PlayerEntry("p2", "<d:ID>20</d:ID><d:FirstName>first2</d:FirstName><d:Lastname>last2</d:Lastname>") +
                "</feed>";
            string xml =
                FeedStart +
                TeamEntry("t1", "<d:TeamID>1</d:TeamID>", Link(false, "Players", playersXml)) +
                "</feed>";
            var query = from t in context.CreateQuery<Team>("Teams")
                        select new { teamid = t.TeamID, players = t.Players.Select(p => p).ToList() };
            foreach (var item in CreateTestMaterializeAtom(xml, query))
            {
                Assert.AreEqual(1, item.teamid, "item.teamid");
                Assert.AreEqual(2, item.players.Count(), "item.players.Count()");
                Assert.AreEqual("first1", item.players.First().FirstName, "item.players.First()");
                Assert.AreEqual("first2", item.players.Last().FirstName, "item.players.Last()");
            }

            AssertEntityCount(2, "no entities if none are as-is projected");
            AssertLinkCount(0, "no links if the parent isn't instantiated");
        }

        [TestMethod]
        public void AtomMaterializerProjectionValueForPathFeedMultipleTest()
        {
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "Players");
            string playersXml =
                FeedStart +
                PlayerEntry("p1", "<d:ID>10</d:ID><d:FirstName>first1</d:FirstName><d:Lastname>last1</d:Lastname>") +
                PlayerEntry("p2", "<d:ID>20</d:ID><d:FirstName>first2</d:FirstName><d:Lastname>last2</d:Lastname>") +
                "</feed>";
            string xml =
                FeedStart +
                TeamEntry("t1", "<d:TeamID>1</d:TeamID>", Link(false, "Players", playersXml)) +
                "</feed>";
            var query = from t in context.CreateQuery<Team>("Teams")
                        select new // TeamWithNames() - PlayerNames doesn't have a counterpart, so we can't use this.
                        {
                            TeamID = t.TeamID,
                            Players = t.Players.Select(p => p).ToList(),
                            PlayerNames = t.Players.Select(p => p.FirstName).ToList()
                        };
            foreach (var item in CreateTestMaterializeAtom(xml, query))
            {
                Assert.AreEqual(1, item.TeamID, "item.teamid");
                Assert.AreEqual(2, item.Players.Count(), "item.players.Count()");
                Assert.IsNotNull(item.Players, "item.Players");
                Assert.IsNotNull(item.PlayerNames, "item.PlayerNames");
                Assert.AreEqual("first1", item.Players.First().FirstName, "item.players.First()");
                Assert.AreEqual("first2", item.Players.Last().FirstName, "item.players.Last()");
                Assert.AreEqual("first1", item.PlayerNames.First(), "item.PlayerNames.First()");
                Assert.AreEqual("first2", item.PlayerNames.Last(), "item.PlayerNames.Last()");
            }

            AssertEntityCount(2, "team (untracked) with two players (tracked)");
            AssertLinkCount(0, "no links to players");
        }

        [TestMethod]
        public void AtomMaterializerProjectionValueForPathFeedWithProjectPrimitiveTest()
        {
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "Players");
            string playersXml =
                FeedStart +
                PlayerEntry("p1", "<d:ID>10</d:ID><d:FirstName>first1</d:FirstName><d:Lastname>last1</d:Lastname>") +
                PlayerEntry("p2", "<d:ID>20</d:ID><d:FirstName>first2</d:FirstName><d:Lastname>last2</d:Lastname>") +
                "</feed>";
            string xml =
                FeedStart +
                TeamEntry("t1", "<d:TeamID>1</d:TeamID>", Link(false, "Players", playersXml)) +
                "</feed>";
            var query = from t in context.CreateQuery<Team>("Teams")
                        select new { teamid = t.TeamID, players = t.Players.Select(p => p.FirstName) };
            foreach (var item in CreateTestMaterializeAtom(xml, query))
            {
                Assert.AreEqual(1, item.teamid, "item.teamid");
                Assert.AreEqual(2, item.players.Count(), "item.players.Count()");
                Assert.AreEqual("first1", item.players.First(), "item.players.First()");
                Assert.AreEqual("first2", item.players.Last(), "item.players.Last()");
            }

            AssertEntityCount(0, "no entities if none are as-is projected");
            AssertLinkCount(0, "no links if the parent isn't instantiated");
        }

        [TestMethod]
        public void AtomMaterializerProjectionValueForPathFeedFromPayloadTest()
        {
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "Players");
            string playersXml =
                FeedStart +
                PlayerEntry("p1", "<d:ID>10</d:ID>") +
                PlayerEntry("p2", "<d:ID>20</d:ID>") +
                "</feed>";
            string xml =
                FeedStart +
                TeamEntry("t1", "<d:TeamID>1</d:TeamID>", Link(false, "Players", playersXml)) +
                "</feed>";
            var query = from t in context.CreateQuery<Team>("Teams")
                        select new { teamid = t.TeamID, team = t, players = t.Players };
            foreach (var item in CreateTestMaterializeAtom(xml, query))
            {
                Assert.IsNotNull(item.team, "item.team");
                Assert.IsNotNull(item.team.Players, "item.team.Players");
                Assert.AreEqual(1, item.team.TeamID, "item.team.TeamID");
                Assert.AreEqual(1, item.teamid, "item.teamid");
                Assert.AreEqual(0, item.team.Players.Count, "item.team.Players.Count");
                Assert.AreEqual(2, item.players.Count, "item.players.Count");
                Assert.AreEqual(10, item.players[0].ID, "item.players[0].ID");
                Assert.AreEqual(20, item.players[1].ID, "item.players[1].ID");
            }

            AssertEntityCount(3, "single team, two players");
            AssertLinkCount(0, "no links from team to each player");
        }

        [TestMethod]
        public void AtomMaterializerProjectionValueForPathFeedLinksPayloadTest()
        {
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "Players");
            string playersXml =
                FeedStart +
                NextLink("http://next-players/") +
                PlayerEntry("p1", "<d:ID>10</d:ID>") +
                PlayerEntry("p2", "<d:ID>20</d:ID>") +
                "</feed>";
            string xml =
                FeedStart +
                NextLink("http://next-team/") +
                TeamEntry("t1", "<d:TeamID>1</d:TeamID>", Link(false, "Players", playersXml)) +
                "</feed>";
            var query = from t in context.CreateQuery<Team>("Teams")
                        select new
                        {
                            teamid = t.TeamID,
                            team = new Team()
                            {
                                Players = t.Players.Select(p => p).ToList()
                            },
                            players = t.Players.ToList(),
                            players2 = new DataServiceCollection<Player>(t.Players, TrackingMode.None),
                            players_as_they_come = t.Players
                        };
            var response = AtomParserTests.CreateQueryResponse(this.context, AtomParserTests.EmptyHeaders, query, xml);
            foreach (var item in response)
            {
                Assert.AreEqual(1, item.teamid, "item.teamid");

                Assert.IsNotNull(item.team, "item.players");
                Assert.IsNotNull(item.players, "item.players");
                Assert.IsNotNull(item.players2, "item.players");

                Assert.AreNotSame(item.players, item.players2, "item.players & item.players2");
                Assert.AreNotSame(item.players, item.team.Players, "item.players & item.team.Players");
                Assert.AreNotSame(item.players2, item.team.Players, "item.players2 & item.team.Players");

                Assert.AreEqual(2, item.players.Count, "item.players.Count");
                Assert.AreEqual(2, item.players2.Count, "item.players2.Count");
                Assert.IsNotNull(item.players2.Continuation);

                // not null because happens within member-init
                Assert.IsNotNull(response.GetContinuation(item.team.Players), "queryResponse.GetNextLinkUri(item.team.Players)");
                Assert.AreEqual("http://next-players/", response.GetContinuation(item.team.Players).NextLinkUri.OriginalString, "queryResponse.GetNextLinkUri(item.team.Players).OriginalString");

                // not null because it's a "raw" projection from a list
                Assert.IsNotNull(response.GetContinuation(item.players_as_they_come), "queryResponse.GetNextLinkUri(item.players_as_they_come)");
                Assert.AreEqual("http://next-players/", response.GetContinuation(item.players_as_they_come).NextLinkUri.OriginalString, "queryResponse.GetNextLinkUri(item.players_as_they_come).OriginalString");
            }

            Assert.IsNotNull(response.GetContinuation(), "queryResponse.GetNextLinkUri()");
            Assert.AreEqual("http://next-team/", response.GetContinuation().NextLinkUri.OriginalString, "queryResponse.GetNextLinkUri().OriginalString");

            AssertEntityCount(3, "single team, two players");
            AssertLinkCount(2, "links from team to each player");
        }

        [TestMethod]
        public void Any_All_LinqQuery_Continuation()
        {
            // Regression test for Client always sends DSV=2.0 when following a continuation token
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "Players");
            string playersXml =
                FeedStart +
                NextLink("http://next-players/") +
                PlayerEntry("p1", "<d:ID>10</d:ID>") +
                PlayerEntry("p2", "<d:ID>20</d:ID>") +
                "</feed>";
            string xml =
                FeedStart +
                NextLink("http://next-team/") +
                TeamEntry("t1", "<d:TeamID>1</d:TeamID>", Link(false, "Players", playersXml)) +
                "</feed>";

            var context = new DataServiceContext(serviceRoot, ODataProtocolVersion.V4);
            context.Format.UseAtom();
            var query = from t in context.CreateQuery<Team>("Teams")
                        where t.Players.Any(playa => playa.FirstName.StartsWith("Phani"))
                        select new
                        {
                            teamid = t.TeamID,
                            team = new Team()
                            {
                                Players = t.Players.Select(p => p).ToList()
                            },
                            players = t.Players.ToList(),
                            players2 = new DataServiceCollection<Player>(t.Players, TrackingMode.None),
                            players_as_they_come = t.Players
                        };
            var response = AtomParserTests.CreateQueryResponse(this.context, AtomParserTests.EmptyHeaders, query, xml);
            foreach (var item in response)
            {
                // we enumerate the results so that we can get to the continuation token.
                // not really interested in the actual entities materialized.
            }

            Assert.IsNotNull(response.GetContinuation(), "queryResponse.GetNextLinkUri()");

            var continuationToken = response.GetContinuation();

            bool dataServiceVersionHeaderSent = true;
            context.SendingRequest2 += (sender, args) =>
            {
                dataServiceVersionHeaderSent = args.RequestMessage.Headers.Any(h => h.Key == "OData-Version");
            };

            try
            {
                context.Execute(continuationToken);
            }
            catch
            {
                // the next link doesnt really go anywhere, we're only validating the headers
                // that get sent as part of this request.
            }

            Assert.IsFalse(dataServiceVersionHeaderSent, "OData-Version header should not be sent for this request");
        }

        [TestMethod]
        public void AtomMaterializerProjectionValueForPathFeedCollectionTest()
        {
            string cities = FeedStart +
                AnyEntry("c1", "<d:BigCityID>100</d:BigCityID>", null) +
                AnyEntry("c2", "<d:BigCityID>200</d:BigCityID>", null) +
                "</feed>";
            string state = AnyEntry("s1", "<d:ID>1</d:ID>", Link(false, "Cities", cities));
            string xml = FeedStart + state + "</feed>";

            var query = from s in context.CreateQuery<LittleState2>("States")
                        select new
                        {
                            id = s.ID,
                            cities = s.Cities
                        };
            foreach (var item in this.CreateTestMaterializeAtom(xml, query))
            {
                Assert.AreEqual(1, item.id, "item.id");
                Assert.IsNotNull(item.cities, "item.cities");
                Assert.AreEqual(2, item.cities.Count, "item.cities.Count");
            }

            AssertEntityCount(2, "two cities");
            AssertLinkCount(0, "no links");
        }

        [TestMethod]
        public void AtomMaterializerProjectionValueForPathFeedRefreshTest()
        {
            string cities = FeedStart +
                AnyEntry("c1", "<d:BigCityID>100</d:BigCityID>", null) +
                AnyEntry("c2key", "<d:BigCityID>200</d:BigCityID>", null) +
                "</feed>";
            string state = AnyEntry("s1", "<d:ID>1</d:ID>", Link(false, "Cities", cities));
            string xml = FeedStart + state + "</feed>";

            var query = from s in context.CreateQuery<LittleState2>("States")
                        select new
                        {
                            citiesAnon = s.Cities,
                            citiesAnonList = s.Cities.ToList(),
                            citiesAnonCollection = new DataServiceCollection<LittleCity>(s.Cities, TrackingMode.None),
                            state = new LittleState2()
                            {
                                ID = s.ID,
                                Cities = new DataServiceCollection<LittleCity>(s.Cities, TrackingMode.None)
                            }
                        };
            LittleCity firstCity = null, secondCity = null;
            foreach (var item in this.CreateTestMaterializeAtom(xml, query))
            {
                firstCity = item.state.Cities[0];
                Assert.AreEqual(100, firstCity.BigCityID);

                secondCity = item.state.Cities[1];
                Assert.AreEqual(200, secondCity.BigCityID);
            }

            AssertEntityCount(3, "single team, two players");
            AssertLinkCount(2, "links from team to each player");

            this.context.MergeOption = MergeOption.OverwriteChanges;
            xml = xml.Replace("c2key", "c3key");
            foreach (var item in this.CreateTestMaterializeAtom(xml, query))
            {
                Assert.AreSame(firstCity, item.state.Cities[0]);
                Assert.AreNotSame(secondCity, item.state.Cities[1]);
                Assert.AreEqual(200, item.state.Cities[1].BigCityID);
            }

            AssertEntityCount(4, "single team, two players, an unconnected player");
            AssertLinkCount(2, "links from team to each player");
        }


        // TODO: write a test where we sometime materialize partly and sometimes not.

        [TestMethod]
        public void AtomMaterializerProjectionValueForPathPrimitiveTest()
        {
            string xml =
                FeedStart +
                TeamEntry("t1", "<d:TeamID>1</d:TeamID>") +
                "</feed>";
            var query = from t in context.CreateQuery<Team>("Teams")
                        select new { tid = t.TeamID };
            foreach (var id in CreateTestMaterializeAtom(xml, query))
            {
                Assert.AreEqual(1, id.tid, "Projected team ID");
            }

            AssertEntityCount(0, "no entities materialized for primitive projection");
        }

        [TestMethod]
        public void AtomMaterializerProjectionValueForPathPrimitiveConversionsTest()
        {
            // With null in payload into non-nullable property.
            Exception exception = TestUtil.RunCatching(() =>
                {
                    string xml =
                        FeedStart +
                        TeamEntry("t1", "<d:TeamID m:null='true'></d:TeamID>") +
                        "</feed>";
                    var query = from t in context.CreateQuery<Team>("Teams")
                                select new { tid = t.TeamID };
                    foreach (var id in CreateTestMaterializeAtom(xml, query))
                    {
                        Assert.AreEqual(1, id.tid, "Projected team ID");
                    }
                });
            TestUtil.AssertExceptionExpected(exception, true);
            TestUtil.AssertContains(exception.ToString(), "InvalidOperationException");

            // With null in payload into nullable property, but we still interpret as TeamID.
            exception = TestUtil.RunCatching(() =>
                {
                    string xml =
                        FeedStart +
                        TeamEntry("t1", "<d:TeamID m:null='true'></d:TeamID>") +
                        "</feed>";
                    var query = from t in context.CreateQuery<Team>("Teams")
                                select new { tid = (int?)t.TeamID };
                    foreach (var id in CreateTestMaterializeAtom(xml, query))
                    {
                        Assert.IsFalse(id.tid.HasValue, "Projected team ID");
                    }
                });

            TestUtil.AssertExceptionExpected(exception, true);

            // TODO: uncomment when the analyzer bug is fixed.
            //TestUtil.AssertContains(exception.ToString(), "InvalidOperationException");
        }

        [TestMethod]
        public void AtomMaterializerProjectionValueForPathPrimitiveMissingTest()
        {
            Exception exception = TestUtil.RunCatching(() =>
            {
                string xml =
                    FeedStart +
                    TeamEntry("t1", "<d:TeamID>1</d:TeamID>") +
                    "</feed>";
                var query = from t in context.CreateQuery<Team>("Teams")
                            select new { tid = t.TeamID, name = t.TeamName };
                foreach (var id in CreateTestMaterializeAtom(xml, query))
                {
                    Assert.Fail("Should not have enumerated anything");
                }
            });

            TestUtil.AssertExceptionExpected(exception, true);
            TestUtil.AssertContains(exception.ToString(), "InvalidOperationException");
        }

        [TestMethod]
        public void AtomMaterializerProjectionValueForPathPrimitiveChainConvertsTest()
        {
            {
                string xml =
                    FeedStart +
                    TeamEntry("t1", "<d:TeamID>1</d:TeamID>") +
                    "</feed>";
                var query = from t in context.CreateQuery<Team>("Teams")
                            select new { tsingle = (Single)t.TeamID };
                foreach (var item in CreateTestMaterializeAtom(xml, query))
                {
                    Assert.AreEqual(1f, item.tsingle, "Projected team ID as single");
                }

                AssertEntityCount(0, "no entities materialized for primitive projection");
            }

            {
                string xml =
                    FeedStart +
                    TeamEntry("t1", "<d:TeamID>1</d:TeamID>") +
                    "</feed>";
                var query = from t in context.CreateQuery<Team>("Teams")
                            select new { tsingle = (Double)(Int16)(Decimal)(Single)t.TeamID };
                foreach (var item in CreateTestMaterializeAtom(xml, query))
                {
                    Assert.AreEqual(1d, item.tsingle, "Projected team ID as single");
                }

                AssertEntityCount(0, "no entities materialized for primitive projection");
            }
        }

        #endregion ProjectionValueForPath tests.

        #region ProjectionPlanCompiler tests.

        [TestMethod]
        public void ProjectionPlanCompilerNestedRejectCorrelationsTest()
        {
            Exception exception = TestUtil.RunCatching(() =>
                {
                    string xml = EmptyFeed;
                    var q = from t in this.context.CreateQuery<Team>("Teams")
                            select new { t = t.TeamID, col = t.Players.Select(p => p.FirstName + t.TeamName) };
                    foreach (var item in CreateTestMaterializeAtom(xml, q))
                    {
                    }
                });
            TestUtil.AssertExceptionExpected(exception, true);
            TestUtil.AssertContains(exception.ToString(), "NotSupported");
        }

        [TestMethod]
        public void ProjectionPlanCompilerRejectIncorrectLevelTest()
        {
            Exception exception = TestUtil.RunCatching(() =>
                {
                    // DoubleMemberTypedEntity<int, List<TypedEntity<int, int>>, List<TypedEntity<int, int>>
                    string xml = EmptyFeed;
                    var q = from t in this.context.CreateQuery<DoubleMemberTypedEntity<int, DataServiceCollection<TypedEntity<int, int>>, DataServiceCollection<TypedEntity<int, int>>>>("T")
                            select new DoubleMemberTypedEntity<int, DataServiceCollection<TypedEntity<int, int>>, DataServiceCollection<TypedEntity<int, int>>>()
                            {
                                ID = t.ID,
                                Member = new DataServiceCollection<TypedEntity<int, int>>(t.Member, TrackingMode.None),
                                Member2 = new DataServiceCollection<TypedEntity<int, int>>(t.Member, TrackingMode.AutoChangeTracking, "ES", (o) => false, (o) => false),
                            };
                    foreach (var item in CreateTestMaterializeAtom(xml, q))
                    { }
                });
            TestUtil.AssertExceptionExpected(exception, true);
            TestUtil.AssertContains(exception.ToString(), "System.NotSupportedException");
            TestUtil.AssertContains(exception.ToString(), "Member");
            TestUtil.AssertContains(exception.ToString(), "Member2");

            // The correct version of the code, without mixing navigation properties.
            {
                string member1 = FeedStart + AnyEntry("http://id/a/1", "<d:ID>10</d:ID>", null) + "</feed>";
                string member2 = FeedStart + AnyEntry("http://id/a/2", "<d:ID>20</d:ID>", null) + "</feed>";
                string xml = FeedStart +
                    AnyEntry("http://id/a", "<d:ID>1</d:ID>", Link(false, "Member", member1) + Link(false, "Member2", member2)) +
                    "</feed>";
                var q = from t in this.context.CreateQuery<DoubleMemberTypedEntity<int, DataServiceCollection<TypedEntity<int, int>>, DataServiceCollection<TypedEntity<int, int>>>>("T")
                        select new DoubleMemberTypedEntity<int, DataServiceCollection<TypedEntity<int, int>>, DataServiceCollection<TypedEntity<int, int>>>()
                        {
                            ID = t.ID,
                            Member = new DataServiceCollection<TypedEntity<int, int>>(t.Member, TrackingMode.None),
                            Member2 = new DataServiceCollection<TypedEntity<int, int>>(t.Member2, TrackingMode.AutoChangeTracking, "ES", (o) => false, (o) => false),
                        };
                foreach (var item in CreateTestMaterializeAtom(xml, q))
                {
                    Assert.IsNotNull(item, "item");
                    Assert.AreEqual(1, item.ID, "item.ID");
                    Assert.IsNotNull(item.Member, "item.Member");
                    Assert.IsNotNull(item.Member2, "item.Member2");
                    Assert.AreNotSame(item.Member, item.Member2);

                    Assert.AreEqual(1, item.Member.Count, "item.Member.Count");
                    Assert.AreEqual(10, item.Member[0].ID, "item.Member[0].ID");

                    Assert.AreEqual(1, item.Member2.Count, "item.Member2.Count");
                    Assert.AreEqual(20, item.Member2[0].ID, "item.Member2[0].ID");
                }

                AssertEntityCount(3, "one top-level entity, two children");
            }
        }

        [TestMethod]
        public void ProjectionPlanCompilerNestedCollectionsTest()
        {
            // This test is actually more like a number of tests all put together.
            // The dimensions stated out front really are present to give more
            // of a feel for what is covered.
            TestUtil.ClearMetadataCache();
            bool[] bools = new bool[] { true, false };
            CombinatorialEngine engine = CombinatorialEngine.FromDimensions(
                new Dimension("payloadDriven", bools),
                new Dimension("topLevel", bools),
                new Dimension("entities", bools));
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            using (TestUtil.RestoreStaticMembersOnDispose(typeof(OpenWebDataServiceHelper)))
            {
                OpenWebDataServiceHelper.PageSizeCustomizer.Value = (config, type) =>
                {
                    config.SetEntitySetPageSize("Customers", 2);
                    config.SetEntitySetPageSize("Orders", 2);
                };

                Data.ServiceModelData.Northwind.EnsureDependenciesAvailable();
                request.DataServiceType = Data.ServiceModelData.Northwind.ServiceModelType;
                request.StartService();

                TestUtil.RunCombinatorialEngineFail(engine, (values) =>
                {
                    DataServiceContext ctx = new DataServiceContext(new Uri(request.BaseUri));
                    ctx.EnableAtom = true;
                    ctx.Format.UseAtom();
                    bool payloadDriven = (bool)values["payloadDriven"];
                    bool topLevel = (bool)values["topLevel"];
                    bool entities = (bool)values["entities"];

                    if (payloadDriven)
                    {
                        if (topLevel)
                        {
                            if (entities)
                            {
                                // payload-driven, top-level entities
                                var q = ctx.CreateQuery<northwindClient.Customers>("Customers").OrderBy(cust => cust.CustomerID);
                                QueryOperationResponse<northwindClient.Customers> c = (QueryOperationResponse<northwindClient.Customers>)
                                    ((DataServiceQuery<northwindClient.Customers>)q).Execute();
                                string lastCustomerID = "";
                                foreach (var item in c)
                                {
                                    Assert.IsNotNull(item, "item");
                                    Assert.IsTrue(lastCustomerID.CompareTo(item.CustomerID) < 0, "lastCustomerID < item.CustomerID -- ascending sort");
                                    lastCustomerID = item.CustomerID;
                                }

                                var continuation = c.GetContinuation();
                                Assert.IsNotNull(continuation, "c.GetContinuation()");
                                Assert.IsNotNull(continuation.NextLinkUri, "c.GetContinuation().NextLinkUri");

                                foreach (var item in ctx.Execute(c.GetContinuation()))
                                {
                                    Assert.IsNotNull(item, "item");
                                    Assert.IsTrue(lastCustomerID.CompareTo(item.CustomerID) < 0, "lastCustomerID < item.CustomerID -- ascending sort");
                                    lastCustomerID = item.CustomerID;
                                }
                            }
                            else
                            {
                                // payload-driven, top-level, non-entities
                                // payload-driven only works for non-entities, so doing nothing
                            }
                        }
                        else
                        {
                            if (entities)
                            {
                                // payload-driven, not-top-level, non-entities
                                var q = ctx.CreateQuery<northwindClient.Customers>("Customers").OrderBy(cust => cust.CustomerID).Select(cust => new { id = cust.CustomerID, orders = cust.Orders });
                                var response = GetTypedResponse(q);
                                string lastCustomerID = "";
                                foreach (var item in response)
                                {
                                    ProjectionPlanCompilerNestedCollectionsTest_ProjectedCustomer(ctx, response, item, item.id, item.orders, ref lastCustomerID);
                                }

                                Assert.IsNotNull(response.GetContinuation(), "c.GetContinuation()");
                                Assert.IsNotNull(response.GetContinuation().NextLinkUri, "c.GetContinuation().NextLinkUri");

                                var continuation = response.GetContinuation();
                                while (continuation != null)
                                {
                                    response = ctx.Execute(continuation);
                                    foreach (var item in response)
                                    {
                                        ProjectionPlanCompilerNestedCollectionsTest_ProjectedCustomer(ctx, response, item, item.id, item.orders, ref lastCustomerID);
                                    }

                                    continuation = response.GetContinuation();
                                }
                            }
                            else
                            {
                                // payload-driven, not-top-level, non-entities
                                // payload-driven only works for non-entities, so doing nothing
                            }
                        }
                    }
                    else
                    {
                        if (topLevel)
                        {
                            if (entities)
                            {
                                // not payload-driven, top-level, entities
                                var q = from cust in ctx.CreateQuery<northwindClient.Customers>("Customers")
                                        orderby cust.CustomerID
                                        select new NarrowNorthwindCustomer()
                                        {
                                            CustomerID = cust.CustomerID,
                                            Orders = cust.Orders.Select(o => new NarrowNorthwindOrder()
                                            {
                                                OrderID = o.OrderID
                                            }).ToList()
                                        };
                                var response = GetTypedResponse(q);
                                string lastCustomerID = "";
                                do
                                {
                                    foreach (var item in response)
                                    {
                                        Assert.IsNotNull(item, "item");
                                        Assert.IsNotNull(item.Orders, "item.Orders");
                                        Assert.IsTrue(lastCustomerID.CompareTo(item.CustomerID) < 0, "lastCustomerID < item -- ascending sort");

                                        var ordersContinuation = response.GetContinuation(item.Orders);
                                        while (ordersContinuation != null)
                                        {
                                            var ordersQuery = ctx.Execute(ordersContinuation);
                                            foreach (var order in ordersQuery)
                                            {
                                                Assert.IsNotNull(order, "order");
                                            }

                                            ordersContinuation = ordersQuery.GetContinuation();
                                        }
                                    }

                                    var continuation = response.GetContinuation();
                                    response = (continuation == null) ? null : ctx.Execute(continuation);
                                }
                                while (response != null);
                            }
                            else
                            {
                                // not payload-driven, top-level, not entities
                            }
                        }
                        else
                        {
                            if (entities)
                            {
                                // not payload-driven, not top-level, entities
                                // Covered under the top-level ones, as we nest narrow orders within narrow customers.
                            }
                            else
                            {
                                // not payload-driven, not top-level, not entities
                                var q = from c in ctx.CreateQuery<northwindClient.Customers>("Customers")
                                        orderby c.CustomerID
                                        select new
                                        {
                                            cid = c.CustomerID,
                                            addresses = c.Orders.Select(o => new { a = o.ShipAddress, al = o.ShipAddress.Length })
                                        };
                                var response = GetTypedResponse(q);
                                while (response != null)
                                {
                                    foreach (var item in response)
                                    {
                                        Assert.IsNotNull(item.cid);
                                        Assert.IsNotNull(item.addresses);
                                        foreach (var add in item.addresses)
                                        {
                                            Assert.AreEqual(add.a.Length, add.al, "length before/after projection");
                                        }

                                        var addressesContinutation = response.GetContinuation(item.addresses);
                                        while (addressesContinutation != null)
                                        {
                                            var more = context.Execute(addressesContinutation);
                                            foreach (var another in more)
                                            {
                                                Assert.AreEqual(another.a.Length, another.al, "length before/after projection");
                                            }

                                            addressesContinutation = more.GetContinuation();
                                        }
                                    }

                                    var continuation = response.GetContinuation();
                                    response = continuation == null ? null : ctx.Execute(continuation);
                                }
                            }
                        }
                    }
                });
            }
        }

        [Microsoft.OData.Client.Key("CustomerID")]
        public class NarrowNorthwindCustomer
        {
            public NarrowNorthwindCustomer() { this.Orders = new List<NarrowNorthwindOrder>(); }
            public string CustomerID { get; set; }
            public string CompanyName { get; set; }
            public List<NarrowNorthwindOrder> Orders { get; set; }
        }

        [Microsoft.OData.Client.Key("OrderID")]
        public class NarrowNorthwindOrder
        {
            public int OrderID { get; set; }
            public DateTimeOffset RequiredDate { get; set; }
        }

        private static void ProjectionPlanCompilerNestedCollectionsTest_ProjectedCustomer(
            DataServiceContext ctx, QueryOperationResponse response, object item, string itemId, ICollection<northwindClient.Orders> itemOrders, ref string lastCustomerID)
        {
            Assert.IsNotNull(item, "item");
            Assert.IsNotNull(itemOrders, "item.orders");
            int orderCount = 0;

            Assert.IsTrue(lastCustomerID.CompareTo(itemId) < 0, "lastCustomerID < item.id -- ascending sort");
            lastCustomerID = itemId;

            Assert.IsTrue(itemOrders.Count <= 2, "item.orders.Count <= 2 -- otherwise paging didn't kick in");
            orderCount = itemOrders.Count;

            var ordersContinuation = response.GetContinuation(itemOrders);
            while (ordersContinuation != null)
            {
                int lastOrderId = itemOrders.Last().OrderID;
                var orderResponse = ctx.Execute(ordersContinuation);
                foreach (var o in orderResponse)
                {
                    Assert.IsNotNull(o);
                    Assert.AreNotEqual(lastOrderId, o.OrderID, "lastOrderId != o.OrderID");
                    orderCount++;
                }

                ordersContinuation = orderResponse.GetContinuation();
            }

            int count = (int)ctx.CreateQuery<northwindClient.Orders>("Orders").Where(order => order.Customers.CustomerID == itemId).Count();
            Assert.AreEqual(count, orderCount, "count == orderCount");
        }

        private static QueryOperationResponse<T> GetTypedResponse<T>(IEnumerable<T> t)
        {
            DataServiceQuery query = (DataServiceQuery)t;
            return (QueryOperationResponse<T>)query.Execute();
        }

        [TestMethod]
        public void ProjectionPlanCompilerVisitLambdaIgnoreTest()
        {
            string xml =
                FeedStart +
                TeamEntry("t1", "<d:TeamID>1</d:TeamID>") +
                "</feed>";
            var query = from t in context.CreateQuery<Team>("Teams")
                        select new { tid = t.TeamID, max = new int[] { 1, 2 }.Select(n => n + 1).Max() };
            foreach (var item in CreateTestMaterializeAtom(xml, query))
            {
                Assert.AreEqual(1, item.tid, "item.teamid");
                Assert.AreEqual(3, item.max, "item.max");
            }

            AssertEntityCount(0, "nothing tracked when not materializing");
            AssertLinkCount(0, "nothing tracked when not materializing");
        }

        [TestMethod]
        public void ProjectionPlanCompilerVisitLambdaNewArrayTest()
        {
            string xml =
                FeedStart +
                TeamEntry("t1", "<d:TeamID>1</d:TeamID>") +
                "</feed>";
            var query = from t in context.CreateQuery<Team>("Teams")
                        select new { teams = new Team[] { t, t } };
            foreach (var item in CreateTestMaterializeAtom(xml, query))
            {
                Assert.AreEqual(2, item.teams.Length, "item.teams.Length");
                Assert.AreSame(item.teams[0], item.teams[1], "teams");
            }

            AssertEntityCount(1, "nothing tracked when not materializing");
            AssertLinkCount(0, "nothing tracked when not materializing");
        }

        [TestMethod]
        public void ProjectionPlanCompilerVisitMemberInitEmptyTest()
        {
            Exception exception = TestUtil.RunCatching(() =>
                {
                    string xml =
                        FeedStart +
                        TeamEntry("t1", "<d:TeamID>1</d:TeamID>") +
                        "</feed>";
                    var query = from t in context.CreateQuery<Team>("Teams")
                                select new Team() { };
                    foreach (var item in CreateTestMaterializeAtom(xml, query))
                    {
                    }
                });
            TestUtil.AssertExceptionExpected(exception, true);
            TestUtil.AssertContains(exception.ToString(), "NotSupportedException");
            AssertEntityCount(0, "nothing tracked when materialization fails");
        }

        [TestMethod]
        public void ProjectionPlanCompilerVisitMemberInitConstantTest()
        {
            Exception exception = TestUtil.RunCatching(() =>
            {
                string xml =
                    FeedStart +
                    TeamEntry("t1", "<d:TeamID>1</d:TeamID>") +
                    "</feed>";
                var query = from t in context.CreateQuery<Team>("Teams")
                            select new Team() { TeamID = 100 };
                foreach (var item in CreateTestMaterializeAtom(xml, query))
                {
                }
            });
            TestUtil.AssertExceptionExpected(exception, true);
            TestUtil.AssertContains(exception.ToString(), "NotSupportedException");
            AssertEntityCount(0, "nothing tracked when materialization fails");
        }

        [TestMethod]
        public void ProjectionPlanCompilerVisitMemberInitPrimitiveSimpleTest()
        {
            string xml =
                FeedStart +
                TeamEntry("t1", "<d:TeamID>1</d:TeamID>") +
                "</feed>";
            var query = from t in context.CreateQuery<Team>("Teams")
                        select new Team() { TeamID = t.TeamID };
            foreach (var item in CreateTestMaterializeAtom(xml, query))
            {
                Assert.AreEqual(1, item.TeamID, "item.TeamID");
            }

            AssertEntityCount(1, "single team tracked");
        }

        [TestMethod]
        public void ProjectionPlanCompilerVisitMemberInitFeedSimpleTest()
        {
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "Players");
            string playersXml =
                FeedStart +
                PlayerEntry("p1", "<d:ID>10</d:ID>") +
                PlayerEntry("p2", "<d:ID>20</d:ID>") +
                "</feed>";
            string xml =
                FeedStart +
                TeamEntry("t1", "<d:TeamID>1</d:TeamID>", Link(false, "Players", playersXml)) +
                "</feed>";
            var query = from t in context.CreateQuery<Team>("Teams")
                        select new Team() { TeamID = t.TeamID, Players = t.Players };
            foreach (var item in CreateTestMaterializeAtom(xml, query))
            {
                Assert.AreEqual(1, item.TeamID, "item.TeamID");
                Assert.IsNotNull(item.Players, "item.Players");
                Assert.AreEqual(2, item.Players.Count, "item.Players.Count");
            }

            AssertEntityCount(3, "single team and two players tracked");
            AssertLinkCount(2, "two player links tracked");
        }

        [TestMethod]
        public void ProjectionPlanCompilerVisitMemberInitEntityDirectTest()
        {
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "HomeStadium");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "Players");

            string stadium = AnyEntry("s1", "<d:ID>1</d:ID><d:City>city</d:City>", null);
            string playersXml =
                FeedStart +
                PlayerEntry("p1", "<d:ID>10</d:ID>") +
                PlayerEntry("p2", "<d:ID>20</d:ID>") +
                "</feed>";
            string xml =
                FeedStart +
                TeamEntry("t1", "<d:TeamID>1</d:TeamID>", Link(true, "HomeStadium", stadium) + Link(false, "Players", playersXml)) +
                "</feed>";
            {
                Trace.WriteLine("Payload-driven materializes link.");
                var q = from t in this.context.CreateQuery<Team>("Teams")
                        select t;
                foreach (var item in CreateTestMaterializeAtom(xml, q))
                {
                    Assert.IsNotNull(item);
                    Assert.AreEqual(1, item.TeamID, "item.TeamID");
                    Assert.IsNotNull(item.Players, "item.Players");
                    Assert.IsNotNull(item.HomeStadium, "item.HomeStadium");

                    Assert.AreEqual(1, item.HomeStadium.ID, "item.HomeStadium.ID");
                    Assert.AreEqual("city", item.HomeStadium.City, "item.HomeStadium.City");

                    Assert.AreEqual(2, item.Players.Count, "item.Players.Count");
                    Assert.AreEqual(10, item.Players[0].ID, "item.Players[0].ID");
                    Assert.AreEqual(20, item.Players[1].ID, "item.Players[0].ID");
                }
                AssertEntityCount(1 + 1 + 2, "one team, one stadium, two players");
                AssertLinkCount(1 + 2, "team->stadium, team->player x2");
                this.ClearContext();
            }

            {
                Trace.WriteLine("Plan-driven does not materialize links");
                var q = from t in this.context.CreateQuery<Team>("Teams")
                        select new { t = t };
                foreach (var item in CreateTestMaterializeAtom(xml, q))
                {
                    Assert.IsNotNull(item.t);
                    Assert.AreEqual(1, item.t.TeamID, "item.t.TeamID");
                    Assert.IsNull(item.t.HomeStadium, "item.t.HomeStadium");

                    Assert.IsNotNull(item.t.Players, "item.t.Players");
                    Assert.AreEqual(0, item.t.Players.Count, "item.t.Players.Count");
                }

                AssertEntityCount(1, "one team");
                AssertLinkCount(0, "no links");
                this.ClearContext();
            }

            {
                Trace.WriteLine("Plan-driven keeps projections independent");
                var q = from t in this.context.CreateQuery<Team>("Teams")
                        select new { t = t, players = t.Players, players_again = new DataServiceCollection<Player>(t.Players.Select(p => p), TrackingMode.None) };
                foreach (var item in CreateTestMaterializeAtom(xml, q))
                {
                    Assert.IsNotNull(item.t);
                    Assert.AreEqual(1, item.t.TeamID, "item.t.TeamID");
                    Assert.IsNull(item.t.HomeStadium, "item.t.HomeStadium");

                    Assert.IsNotNull(item.t.Players, "item.t.Players");
                    Assert.AreEqual(0, item.t.Players.Count, "item.t.Players.Count");

                    Assert.IsNotNull(item.players, "item.players");
                    Assert.AreEqual(2, item.players.Count, "item.players.Count");

                    Assert.IsNotNull(item.players_again, "item.players");
                    Assert.AreEqual(2, item.players_again.Count, "item.players.Count");

                    Assert.AreSame(item.players[0], item.players_again[0], "players[0]");
                    Assert.AreSame(item.players[1], item.players_again[1], "players[1]");
                }

                AssertEntityCount(1 + 2, "one team, two players");
                AssertLinkCount(0, "no links");
                this.ClearContext();
            }

            {
                Trace.WriteLine("Plan-driven still allows links to be followed explicitly");
                var q = from t in this.context.CreateQuery<Team>("Teams")
                        select new Team()
                        {
                            TeamID = t.TeamID,
                            HomeStadium = t.HomeStadium,
                            Players = t.Players
                        };
                foreach (var item in CreateTestMaterializeAtom(xml, q))
                {
                    Assert.IsNotNull(item);
                    Assert.AreEqual(1, item.TeamID, "item.TeamID");
                    Assert.IsNotNull(item.Players, "item.Players");
                    Assert.IsNotNull(item.HomeStadium, "item.HomeStadium");

                    Assert.AreEqual(1, item.HomeStadium.ID, "item.HomeStadium.ID");
                    Assert.AreEqual("city", item.HomeStadium.City, "item.HomeStadium.City");

                    Assert.AreEqual(2, item.Players.Count, "item.Players.Count");
                    Assert.AreEqual(10, item.Players[0].ID, "item.Players[0].ID");
                    Assert.AreEqual(20, item.Players[1].ID, "item.Players[0].ID");
                }

                AssertEntityCount(1 + 1 + 2, "one team, one stadium, two players");
                AssertLinkCount(1 + 2, "team->stadium, team->player x2");
                this.ClearContext();
            }
        }

        [TestMethod]
        public void ProjectionPlanCompilerVisitMemberInitEntityNarrowTest()
        {
            string xml =
                FeedStart +
                TeamEntry("t1", "<d:TeamID>1</d:TeamID>") +
                "</feed>";
            var query = from t in context.CreateQuery<Team>("Teams")
                        select new TeamWithNames() { TeamID = t.TeamID };
            foreach (var item in CreateTestMaterializeAtom(xml, query))
            {
                Assert.AreEqual(1, item.TeamID, "item.TeamID");
                Assert.IsNull(item.Players, "item.Players");
            }

            AssertEntityCount(1, "single team tracked");
            AssertLinkCount(0, "no links");
        }

        [TestMethod]
        public void ProjectionPlanCompilerVisitMemberInitEntityNestedTest()
        {
            // Inventing our own little hierarchy.
            // TypedEntity<int, string> leaf;
            // TypedEntity<int, TypedEntity<int, string>> leafParent;
            // TypedEntity<int, TypedEntity<int, TypedEntity<int, string>>> leafGrandparent;
            string leaf = AnyEntry("leaf", "<d:ID>1</d:ID><d:Member>Leaf</d:Member>", null);
            string leafParent = AnyEntry("parent", "<d:ID>20</d:ID>", Link(true, "Member", leaf));
            string leafGrandparent = AnyEntry("gramps", "<d:ID>300</d:ID>", Link(true, "Member", leafParent));
            string xml = FeedStart + leafGrandparent + "</feed>";
            var query = from t in context.CreateQuery<TypedEntity<int, TypedEntity<int, TypedEntity<int, string>>>>("T")
                        select new TypedEntity<int, TypedEntity<int, TypedEntity<int, string>>>
                        {
                            ID = t.ID,
                            Member = new TypedEntity<int, TypedEntity<int, string>>()
                            {
                                ID = t.Member.ID,
                                Member = new TypedEntity<int, string>()
                                {
                                    ID = t.Member.Member.ID,
                                    Member = t.Member.Member.Member
                                }
                            }
                        };
            foreach (var item in CreateTestMaterializeAtom(xml, query))
            {
                Assert.AreEqual(300, item.ID, "item.ID");
                Assert.IsNotNull(item.Member, "item.Member");

                Assert.AreEqual(20, item.Member.ID, "item.Member.ID");
                Assert.IsNotNull(item.Member.Member, "item.Member.Member");

                Assert.AreEqual(1, item.Member.Member.ID, "item.Member.Member.ID");
                Assert.AreEqual("Leaf", item.Member.Member.Member, "item.Member.Member.Member");
            }
        }

        [TestMethod]
        public void ProjectionPlanCompilerVisitMemberInitEntityNestedOffTest()
        {
            // Stepping too deep but into a compatible type.
            var q = from s in context.CreateQuery<BigState>("BigStates")
                    select new LittleState
                    {
                        ID = s.ID,
                        StateName = s.CoolestCity.State.StateName
                    };
            Exception exception = TestUtil.RunCatching(() =>
                {
                    foreach (var item in q)
                    {
                    }
                });
            TestUtil.AssertExceptionExpected(exception, true);
            TestUtil.AssertContains(exception.ToString().ToLowerInvariant(), "notsupported");
            TestUtil.AssertContains(exception.ToString().ToLowerInvariant(), "do not refer to the same source entit");
        }

        [TestMethod]
        public void ProjectionPlanCompilerVisitMemberInitEntityNestedParallelTest()
        {
            // Inventing our own little hierarchy, with multiple levels.
            // TypedEntity<int, string> leaf;
            // DoubleMemberTypedEntity<int, TypedEntity<int, string>, TypedEntity<double, double>> leafParent;
            string leaf = AnyEntry("leaf", "<d:ID>1</d:ID><d:Member>Leaf</d:Member>", null);
            string leafDouble = AnyEntry("leaf-double", "<d:ID>1.0</d:ID><d:Member>2.0</d:Member>", null);
            string leafParent = AnyEntry("parent", "<d:ID>20</d:ID>", Link(true, "Member", leaf) + Link(true, "Member2", leafDouble));
            string xml = FeedStart + leafParent + "</feed>";
            var query = from t in context.CreateQuery<DoubleMemberTypedEntity<int, TypedEntity<int, string>, TypedEntity<double, double>>>("T")
                        select new DoubleMemberTypedEntity<int, TypedEntity<int, string>, TypedEntity<double, double>>
                        {
                            ID = t.ID,
                            Member = new TypedEntity<int, string>()
                            {
                                ID = t.Member.ID,
                                Member = t.Member.Member
                            },
                            Member2 = new TypedEntity<double, double>()
                            {
                                ID = t.Member2.ID,
                                Member = t.Member2.Member
                            }
                        };
            foreach (var item in CreateTestMaterializeAtom(xml, query))
            {
                Assert.AreEqual(20, item.ID, "item.ID");
                Assert.IsNotNull(item.Member, "item.Member");
                Assert.IsNotNull(item.Member2, "item.Member2");

                Assert.AreEqual(1, item.Member.ID, "item.Member.ID");
                Assert.AreEqual("Leaf", item.Member.Member, "item.Member.Member");

                Assert.AreEqual(1.0d, item.Member2.ID, "item.Member2.ID");
                Assert.AreEqual(2.0d, item.Member2.Member, "item.Member2.Member");
            }
        }

        [TestMethod]
        public void ProjectionPlanCompilerVisitMemberInitEntityNested2Test()
        {
            // Inventing our own little hierarchy, with multiple levels.
            // TypedEntity<int, string> leaf;
            // DoubleMemberTypedEntity<int, TypedEntity<int, string>, TypedEntity<double, double>> leafParent;
            // DoubleMemberTypedEntity<int, DoubleMemberTypedEntity<int, TypedEntity<int, string>, TypedEntity<double, double>>, List<Team>> leafGrandparent;
            string leaf = AnyEntry("leaf", "<d:ID>1</d:ID><d:Member>Leaf</d:Member>", null);
            string leafDouble = AnyEntry("leaf-double", "<d:ID>1.0</d:ID><d:Member>2.0</d:Member>", null);
            string leafParent = AnyEntry("parent", "<d:ID>20</d:ID>", Link(true, "Member", leaf) + Link(true, "Member2", leafDouble));
            string teams = FeedStart + AnyEntry("t1", "<d:TeamID>999</d:TeamID>", null) + "</feed>";
            string leafGrandparent = AnyEntry("gramps", "<d:ID>300</d:ID>", Link(true, "Member", leafParent) + Link(false, "Member2", teams));
            string xml = FeedStart + leafGrandparent + "</feed>";
            var query = from t in context.CreateQuery<DoubleMemberTypedEntity<int, DoubleMemberTypedEntity<int, TypedEntity<int, string>, TypedEntity<double, double>>, List<Team>>>("T")
                        select new DoubleMemberTypedEntity<int, DoubleMemberTypedEntity<int, TypedEntity<int, string>, TypedEntity<double, double>>, List<Team>>
                        {
                            ID = t.ID,
                            Member = new DoubleMemberTypedEntity<int, TypedEntity<int, string>, TypedEntity<double, double>>()
                            {
                                ID = t.Member.ID,
                                Member = new TypedEntity<int, string>()
                                {
                                    ID = t.Member.Member.ID,
                                    Member = t.Member.Member.Member
                                },
                                Member2 = new TypedEntity<double, double>()
                                {
                                    ID = t.Member.Member2.ID,
                                    Member = t.Member.Member2.Member
                                }
                            },
                            Member2 = t.Member2.Select(m => new Team() { TeamID = m.TeamID }).ToList()
                        };
            foreach (var item in CreateTestMaterializeAtom(xml, query))
            {
                Assert.AreEqual(300, item.ID, "item.ID");
                Assert.IsNotNull(item.Member, "item.Member");
                Assert.IsNotNull(item.Member2, "item.Member2");

                Assert.AreEqual(20, item.Member.ID, "item.Member.ID");
                Assert.IsNotNull(item.Member.Member, "item.Member.Member");
                Assert.IsNotNull(item.Member.Member2, "item.Member.Member2");

                Assert.AreEqual(1, item.Member.Member.ID, "item.Member.Member.ID");
                Assert.AreEqual("Leaf", item.Member.Member.Member, "item.Member.Member.Member");

                Assert.AreEqual(1.0d, item.Member.Member2.ID, "item.Member.Member2.ID");
                Assert.AreEqual(2.0d, item.Member.Member2.Member, "item.Member.Member2.Member");

                Assert.AreEqual(1, item.Member2.Count, "item.Member2.Count");
                Assert.AreEqual(999, item.Member2[0].TeamID, "item.Member2[0].TeamID");
            }
        }

        [TestMethod]
        public void ProjectionPlanCompilerVisitMemberInitEntityNestedSkipTest()
        {
            // TypedEntity<int, DoubleMemberTypedEntity<int, string, Team>>
            string team = AnyEntry("team", "<d:TeamID>999</d:TeamID>", null);
            string leaf = AnyEntry("leaf", "<d:ID>1</d:ID><d:Member>leaf</d:Member>", Link(true, "Member2", team));
            string parent = AnyEntry("parent", "<d:ID>20</d:ID>", Link(true, "Member", leaf));
            string xml = FeedStart + parent + "</feed>";
            var q = from t in this.context.CreateQuery<TypedEntity<int, DoubleMemberTypedEntity<int, string, Team>>>("t")
                    select new
                    {
                        top = new TypedEntity<int, DoubleMemberTypedEntity<int, string, Team>>()
                        {
                            ID = t.ID,
                            Member = t.Member
                        },
                        nested = new DoubleMemberTypedEntity<int, string, Team>()
                        {
                            ID = t.Member.ID,
                            Member = t.Member.Member,
                            Member2 = t.Member.Member2
                        }
                    };
            foreach (var item in CreateTestMaterializeAtom(xml, q))
            {
                Assert.IsNotNull(item.top, "item.top");
                Assert.IsNotNull(item.nested, "item.nested");

                Assert.AreEqual(20, item.top.ID, "item.top.ID");

                Assert.AreEqual(1, item.nested.ID, "item.nested.ID");
                Assert.AreEqual("leaf", item.nested.Member, "item.nested.Member");
                Assert.IsNotNull(item.nested.Member2, "item.nested.Member2");

                Assert.AreEqual(999, item.nested.Member2.TeamID, "item.nested.Member2.TeamID");

                Assert.AreSame(item.top.Member, item.nested, "item.top.Member same as nested");
            }
        }

        [TestMethod]
        public void ProjectionPlanCompilerVisitMemberInitEntityNestedSkipOnlyEntityTest()
        {
            string team = AnyEntry("team", "<d:TeamID>999</d:TeamID>", null);
            string leaf = AnyEntry("leaf", "<d:ID>1</d:ID><d:Member>leaf</d:Member>", Link(true, "Member2", team));
            string parent = AnyEntry("parent", "<d:ID>20</d:ID>", Link(true, "Member2", leaf));
            string grandParent = AnyEntry("parent", "<d:ID>300</d:ID>", Link(true, "Member", parent));
            string xml = FeedStart + grandParent + "</feed>";
            var q = from t in this.context.CreateQuery<TypedEntity<int, DoubleMemberTypedEntity<int, string, DoubleMemberTypedEntity<int, string, Team>>>>("t")
                    select new
                    {
                        nested = new DoubleMemberTypedEntity<int, string, DoubleMemberTypedEntity<int, string, Team>>()
                        {
                            Member2 = new DoubleMemberTypedEntity<int, string, Team>()
                            {
                                Member2 = new Team()
                                {
                                    TeamID = t.Member.Member2.Member2.TeamID
                                }
                            }
                        }
                    };
            foreach (var item in CreateTestMaterializeAtom(xml, q))
            {
                Assert.IsNotNull(item.nested, "item.nested");
                Assert.AreEqual(999, item.nested.Member2.Member2.TeamID, "item.nested.Member2.TeamID");
            }
        }

        [TestMethod]
        public void ProjectionPlanCompilerVisitMemberInitEntityNestedSkipDeepTest()
        {
            // t: TypedEntity<int, DoubleMemberTypedEntity<int, string, DoubleMemberTypedEntity<int, string, Team>>>
            // t.Member: DoubleMemberTypedEntity<int, string, DoubleMemberTypedEntity<int, string, Team>>
            // t.Member.Member2: DoubleMemberTypedEntity<int, string, Team>
            string team = AnyEntry("team", "<d:TeamID>999</d:TeamID>", null);
            string leaf = AnyEntry("leaf", "<d:ID>1</d:ID><d:Member>leaf</d:Member>", Link(true, "Member2", team));
            string parent = AnyEntry("parent", "<d:ID>20</d:ID><d:Member>parent</d:Member>", Link(true, "Member2", leaf));
            string grandParent = AnyEntry("gramps", "<d:ID>300</d:ID>", Link(true, "Member", parent));
            string xml = FeedStart + grandParent + "</feed>";
            var q = from t in this.context.CreateQuery<TypedEntity<int, DoubleMemberTypedEntity<int, string, DoubleMemberTypedEntity<int, string, Team>>>>("t")
                    select new
                    {
                        top = new TypedEntity<int, DoubleMemberTypedEntity<int, string, DoubleMemberTypedEntity<int, string, Team>>>()
                        {
                            ID = t.ID,
                        },
                        nested = new DoubleMemberTypedEntity<int, string, DoubleMemberTypedEntity<int, string, Team>>()
                        {
                            ID = t.Member.ID,
                            Member = t.Member.Member,
                            Member2 = new DoubleMemberTypedEntity<int, string, Team>()
                            {
                                ID = t.Member.Member2.ID,
                                Member2 = new Team()
                                {
                                    TeamID = t.Member.Member2.Member2.TeamID
                                }
                            }
                        }
                    };
            foreach (var item in CreateTestMaterializeAtom(xml, q))
            {
                Assert.IsNotNull(item.top, "item.top");
                Assert.IsNotNull(item.nested, "item.nested");

                Assert.AreEqual(300, item.top.ID, "item.top.ID");

                Assert.AreEqual(20, item.nested.ID, "item.nested.ID");
                Assert.AreEqual("parent", item.nested.Member, "item.nested.Member");

                Assert.AreEqual(999, item.nested.Member2.Member2.TeamID, "item.nested.Member2.TeamID");
            }
        }

        [TestMethod]
        public void ProjectionPlanCompilerVisitMemberInitEntityNestedSkipDeepTest1()
        {
            // Client Projections : Lambda parameter not in scope
            string customer0 = AnyEntry("Customer", "<d:ID>999</d:ID><d:Name>Customer0</d:Name>", null);
            string customer1 = AnyEntry("Customer1", "<d:ID>1</d:ID><d:Name>Customer1</d:Name>", Link(true, "BestFriend", customer0));
            string customer2 = AnyEntry("Customer2", "<d:ID>2</d:ID><d:Name>Customer2</d:Name>", Link(true, "BestFriend", customer1));
            string customer3 = AnyEntry("Customer3", "<d:ID>3</d:ID><d:Name>Customer3</d:Name>", Link(true, "BestFriend", customer2));
            string customer4 = AnyEntry("Customer4", "<d:ID>4</d:ID><d:Name>Customer4</d:Name>", Link(true, "BestFriend", customer3));

            string xml = FeedStart + customer4 + "</feed>";
            this.context.MergeOption = MergeOption.NoTracking;
            var q = from customer in this.context.CreateQuery<Customer>("Customers")
                    select new Customer()
                    { //customer4
                        BestFriend = new Customer()
                        {//customer3
                            BestFriend = new Customer()
                            {//customer2
                                BestFriend = new Customer()
                                { //customer1
                                    BestFriend = new Customer()
                                    { //customer 0 
                                        //     4         3         2           1          0
                                        ID = customer.BestFriend.BestFriend.BestFriend.BestFriend.ID
                                    }
                                }
                            }
                        }
                    };
            foreach (var item in CreateTestMaterializeAtom(xml, q))
            {
                Assert.IsNotNull(item.BestFriend, "customer4.BestFriend");
                Assert.IsNotNull(item.BestFriend.BestFriend, "customer3.BestFriend");
                Assert.IsNotNull(item.BestFriend.BestFriend.BestFriend, "customer2.BestFriend");
                Assert.IsNotNull(item.BestFriend.BestFriend.BestFriend.BestFriend, "customer1.BestFriend");
                Assert.IsNotNull(item.BestFriend.BestFriend.BestFriend.BestFriend.ID, "customer0.ID");
                Assert.AreEqual(item.BestFriend.BestFriend.BestFriend.BestFriend.ID, 999, "item.BestFriend.BestFriend.BestFriend.BestFriend.ID == 999");
            }
        }

        public class CustomerAndFriends
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public CustomerAndFriends BestFriend { get; set; }
            public CustomerAndFriends BestFriend1 { get; set; }
            public CustomerAndFriends BestFriend2 { get; set; }
            public CustomerAndFriends BestFriend3 { get; set; }
        }

        [TestMethod]
        public void ProjectionPlanCompilerVisitMemberInitEntityNestedSkipDeepTest2()
        {
            string customer0 = AnyEntry("Customer", "<d:ID>999</d:ID><d:Name>Customer0</d:Name>", null);
            string customer1 = AnyEntry("Customer1", "<d:ID>1</d:ID><d:Name>Customer1</d:Name>", Link(true, "BestFriend", customer0));
            string customer2 = AnyEntry("Customer2", "<d:ID>2</d:ID><d:Name>Customer2</d:Name>", Link(true, "BestFriend1", customer1));
            string customer3 = AnyEntry("Customer3", "<d:ID>3</d:ID><d:Name>Customer3</d:Name>", Link(true, "BestFriend2", customer2));
            string customer4 = AnyEntry("Customer4", "<d:ID>4</d:ID><d:Name>Customer4</d:Name>", Link(true, "BestFriend3", customer3));

            string xml = FeedStart + customer4 + "</feed>";

            this.context.MergeOption = MergeOption.NoTracking;
            var q = from customer in this.context.CreateQuery<CustomerAndFriends>("Customers")
                    select new CustomerAndFriends()
                    { //customer4
                        BestFriend3 = new CustomerAndFriends()
                        {//customer3
                            BestFriend2 = new CustomerAndFriends()
                            {//customer2
                                BestFriend1 = new CustomerAndFriends()
                                { //customer1
                                    BestFriend = new CustomerAndFriends()
                                    { //customer 0 
                                        //     4         3         2           1          0
                                        ID = customer.BestFriend3.BestFriend2.BestFriend1.BestFriend.ID
                                    }
                                }
                            }
                        }
                    };
            foreach (var item in CreateTestMaterializeAtom(xml, q))
            {
                Assert.IsNotNull(item.BestFriend3, "customer4.BestFriend3");
                Assert.IsNotNull(item.BestFriend3.BestFriend2, "customer3.BestFriend2");
                Assert.IsNotNull(item.BestFriend3.BestFriend2.BestFriend1, "customer2.BestFriend1");
                Assert.IsNotNull(item.BestFriend3.BestFriend2.BestFriend1.BestFriend, "customer1.BestFriend");
                Assert.IsNotNull(item.BestFriend3.BestFriend2.BestFriend1.BestFriend.ID, "customer0.ID");
                Assert.AreEqual(item.BestFriend3.BestFriend2.BestFriend1.BestFriend.ID, 999, "item.BestFriend3.BestFriend2.BestFriend1.BestFriend.ID == 999");
            }

            var q1 = from customer in this.context.CreateQuery<CustomerAndFriends>("AnonymousCustomers")
                     select new
                     { //customer4
                         ID = customer.BestFriend3.BestFriend2.BestFriend1.BestFriend.ID,
                         SomeFriend = new CustomerAndFriends()
                         {
                             Name = customer.BestFriend3.BestFriend2.Name
                         }
                     };

            foreach (var item in CreateTestMaterializeAtom(xml, q1))
            {
                Assert.AreEqual(item.ID, 999, "item.ID == 999");
                Assert.IsNotNull(item.SomeFriend, "item.SomeFriend != null");
                Assert.AreEqual(item.SomeFriend.Name, "Customer2", "item.SomeFriend.Name == 'Customer2'");
            }
        }

        [TestMethod]
        public void ProjectionPlanCompilerVisitMemberInitEntityNestNarrowTest()
        {
            // TODO: test that things that are not member inits for nested items
            string sponsor = AnyEntry("sponsor20", "<d:StadiumSponsorID>20</d:StadiumSponsorID><d:Name>sponsor 20</d:Name>", null);
            string stadium = AnyEntry("s1", "<d:ID>1</d:ID><d:City>city</d:City>", Link(true, "Sponsor", sponsor));
            string xml = FeedStart + stadium + "</feed>";

            ReadOnlyTestContext.AddBaselineIncludes(typeof(Stadium), "Sponsor");
            ReadOnlyTestContext.AddBaselineIncludes(typeof(NarrowStadium), "Sponsor");

            var query = from s in context.CreateQuery<Stadium>("Stadiums")
                        select new NarrowStadium
                        {
                            ID = s.ID,
                            City = s.City,
                            Sponsor = new NarrowStadiumSponsor
                            {
                                StadiumSponsorID = s.Sponsor.StadiumSponsorID
                            }
                        };
            foreach (var item in this.CreateTestMaterializeAtom(xml, query))
            {
                Assert.AreEqual(1, item.ID, "item.ID");
                Assert.IsNotNull(item.City, "item.City");
                Assert.IsNotNull(item.Sponsor, "item.Sponsor");
                Assert.AreEqual(item.City, "city");
                Assert.AreEqual(20, item.Sponsor.StadiumSponsorID, "item.Sponsor.StadiumSponsorID");
            }

            AssertEntityCount(2, "standium and sponsor");
            AssertLinkCount(1, "stadium to sponsor");
        }

        [TestMethod]
        public void ProjectionPlanCompilerVisitMemberInitEntitySelectTest()
        {
            ReadOnlyTestContext.AddBaselineIncludes(typeof(Team), "Players");
            string playersXml =
                FeedStart +
                PlayerEntry("p1", "<d:ID>10</d:ID>") +
                PlayerEntry("p2", "<d:ID>20</d:ID>") +
                "</feed>";
            string xml =
                FeedStart +
                TeamEntry("t1", "<d:TeamID>1</d:TeamID>", Link(false, "Players", playersXml)) +
                "</feed>";
            var query = from t in context.CreateQuery<Team>("Teams")
                        select new Team() { TeamID = t.TeamID, Players = t.Players.Select(p => p).ToList() };
            foreach (var item in CreateTestMaterializeAtom(xml, query))
            {
                Assert.AreEqual(1, item.TeamID, "item.TeamID");
                Assert.IsNotNull(item.Players, "item.Players");
                Assert.AreEqual(2, item.Players.Count, "item.Players.Count");
            }

            AssertEntityCount(3, "single team tracked");
            AssertLinkCount(2, "team to two players");
        }


        [TestMethod]
        public void ProjectionPlanCompilerVisitMemberInitEntitySelectNarrowTest()
        {
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "Cities");
            string cities =
                FeedStart +
                AnyEntry("city1", "<d:BigCityID>1</d:BigCityID>", null) +
                AnyEntry("city2", "<d:BigCityID>2</d:BigCityID>", null) +
                "</feed>";
            string xml =
                FeedStart +
                AnyEntry("bigstate1", "<d:StateName>big state 1</d:StateName>", Link(false, "Cities", cities)) +
                "</feed>";
            BigCity localBigCity = new BigCity();
            localBigCity.Mayor = "Mr. Smith";
            var query = from s in context.CreateQuery<BigState>("BigStates")
                        select new
                        {
                            nid = s.StateName,
                            b = s.Cities.Select(c => new LittleCity { BigCityID = c.BigCityID }), // Select
                            c = localBigCity.Mayor.Length, // referencing local + member access + methodCall.
                        };
            foreach (var item in CreateTestMaterializeAtom(xml, query))
            {
                Assert.AreEqual("big state 1", item.nid, "item.nid");
                Assert.IsNotNull(item.b, "item.b");
                Assert.AreEqual(2, item.b.Count(), "item.b.Count()");
                Assert.AreEqual(localBigCity.Mayor.Length, item.c, "item.c");
            }

            AssertEntityCount(2, "two cities selected");
            AssertLinkCount(0, "no links tracked");
        }

        [TestMethod]
        public void ProjectionPlanCompilerVisitMemberInitEntitySelectNarrowMismatchTest()
        {
            ReadOnlyTestContext.AddBaselineIncludes(typeof(BigState), "Cities");
            string cities =
                FeedStart +
                AnyEntry("city1", "<d:BigCityID>1</d:BigCityID>", null) +
                AnyEntry("city2", "<d:BigCityID>2</d:BigCityID>", null) +
                "</feed>";
            string xml =
                FeedStart +
                AnyEntry("bigstate1", "<d:StateName>big state 1</d:StateName>", Link(false, "Cities", cities)) +
                "</feed>";
            BigCity localBigCity = new BigCity();
            localBigCity.Mayor = "Mr. Smith";
            var query = from s in context.CreateQuery<BigState>("BigStates")
                        select new
                        {
                            nid = s.StateName,
                            a = s.Cities.ToList(), //ToList
                            b = s.Cities.Select(c => new LittleCity { BigCityID = c.BigCityID }), // Select
                            c = localBigCity.Mayor.Length, // referencing local + member access + methodCall.
                        };
            Exception exception = TestUtil.RunCatching(() =>
                {
                    foreach (var item in CreateTestMaterializeAtom(xml, query))
                    {
                        Assert.AreEqual("big state 1", item.nid, "item.nid");
                        Assert.IsNotNull(item.a, "item.a");
                        Assert.AreEqual(2, item.a.Count, "item.a.Count");
                        Assert.IsNotNull(item.b, "item.b");
                        Assert.AreEqual(2, item.b.Count(), "item.b.Count()");
                        Assert.AreEqual(localBigCity.Mayor, item.c, "item.c");
                    }
                });
            TestUtil.AssertExceptionExpected(exception, true);
            TestUtil.AssertContains(exception.ToString(), "InvalidOperationException");

            AssertEntityCount(0, "nothing recorded when things fails");
            AssertLinkCount(0, "no links tracked when things fail");
        }

        [TestMethod]
        public void ProjectionPlanCompilerVisitMethodCallComparisonsTest()
        {
            string state = AnyEntry("s1", "<d:ID>1</d:ID><d:StateName>name</d:StateName>", null);
            string xml = FeedStart + state + "</feed>";
            var q = from s in context.CreateQuery<BigState>("S")
                    select new
                    {
                        int_constant = 1,
                        state_name = s.StateName,
                        one_if_lower = s.StateName == s.StateName.ToLowerInvariant() ? 1 : 0,
                        oned_if_long = s.StateName.Length > 4 ? 1d : 0d,
                        onef_if_null = s.StateName == null ? 1f : 0f,
                        bool_if_compares_big = String.Compare(s.StateName, "n") > 0,
                    };
            foreach (var item in CreateTestMaterializeAtom(xml, q))
            {
                Assert.AreEqual(1, item.int_constant, "item.int_constant");
                Assert.AreEqual("name", item.state_name, "item.state_name");
                Assert.AreEqual(1, item.one_if_lower, "item.one_if_lower");
                Assert.AreEqual(0d, item.oned_if_long, "item.oned_if_long");
                Assert.AreEqual(0f, item.onef_if_null, "item.onef_if_null");
                Assert.AreEqual(true, item.bool_if_compares_big, "item.bool_if_compares_big");
            }
        }

        [TestMethod]
        public void ProjectionPlanCompilerVisitMethodCallToArrayTest()
        {
            string cities = FeedStart +
                AnyEntry("c1", "<d:BigCityID>100</d:BigCityID>", null) +
                AnyEntry("c2", "<d:BigCityID>200</d:BigCityID>", null) +
                "</feed>";
            string state = AnyEntry("s1", "<d:ID>1</d:ID>", Link(false, "Cities", cities));
            string xml = FeedStart + state + "</feed>";

            Exception exception;

            // ToArray in anonymous types is not supported; projection analyzer
            // should reject these.
            exception = TestUtil.RunCatching(() =>
                {
                    var query = from s in context.CreateQuery<LittleState2>("States")
                                select new
                                {
                                    arr = s.Cities.ToArray(),
                                };
                    foreach (var i in query) { }
                });
            TestUtil.AssertExceptionExpected(exception, true);
            TestUtil.AssertContains(exception.ToString().ToLowerInvariant(), "not supported");
        }

        [TestMethod]
        public void ProjectionPlanCompilerVisitMethodCallToDataCollectionTest()
        {
            string cities = FeedStart +
                AnyEntry("c1", "<d:BigCityID>100</d:BigCityID>", null) +
                AnyEntry("c2", "<d:BigCityID>200</d:BigCityID>", null) +
                "</feed>";
            string state = AnyEntry("s1", "<d:ID>1</d:ID>", Link(false, "Cities", cities));
            string xml = FeedStart + state + "</feed>";

            var query = from s in context.CreateQuery<LittleState2>("States")
                        select new
                        {
                            citiesAnon = s.Cities,
                            citiesAnonList = s.Cities.ToList(),
                            citiesAnonCollection = new DataServiceCollection<LittleCity>(s.Cities, TrackingMode.None),
                            state = new LittleState2()
                            {
                                ID = s.ID,
                                Cities = new DataServiceCollection<LittleCity>(s.Cities, TrackingMode.None)
                            }
                        };
            foreach (var item in this.CreateTestMaterializeAtom(xml, query))
            {
                Assert.IsNotNull(item.citiesAnon, "item.citiesAnon");
                Assert.IsNotNull(item.citiesAnonList, "item.citiesAnonList");
                Assert.IsNotNull(item.citiesAnonCollection, "item.citiesAnonCollection");
                Assert.IsNotNull(item.state, "item.state");

                Assert.AreEqual(2, item.citiesAnon.Count(), "item.citiesAnon.Count()");
                Assert.AreEqual(2, item.citiesAnonList.Count, "item.citiesAnonList.Count");
                Assert.AreEqual(2, item.citiesAnonCollection.Count, "item.citiesAnonCollection.Count");

                Assert.AreEqual(1, item.state.ID, "item.state.ID");
                Assert.IsNotNull(item.state.Cities, "item.state.Cities");
                Assert.AreEqual(2, item.state.Cities.Count, "item.state.Cities.Count");
            }

            AssertEntityCount(3, "one state and two cities");
            AssertLinkCount(2, "state to each city");
        }

        [TestMethod]
        public void ProjectionPlanCompilerVisitMethodCallToConstantTest()
        {
            string xml = FeedStart + AnyEntry("c1", "<d:Name>100</d:Name>", null) + "</feed>";

            {
                const string constantText = "100";
                var q = from c in this.context.CreateQuery<LittleCity>("Cities")
                        select GetConstant<string>(constantText);
                foreach (var item in this.CreateTestMaterializeAtom(xml, q))
                {
                    Assert.AreEqual("100", item, "item");
                }
            }

            {
                const int constantInt = 100;
                var q = from c in this.context.CreateQuery<LittleCity>("Cities")
                        select GetConstant<int>(constantInt);
                foreach (var item in this.CreateTestMaterializeAtom(xml, q))
                {
                    Assert.AreEqual(100, item, "item");
                }
            }

            {
                const double constantDouble = 100d;
                var q = from c in this.context.CreateQuery<LittleCity>("Cities")
                        select new { c = GetConstant<double>(constantDouble) };
                foreach (var item in this.CreateTestMaterializeAtom(xml, q))
                {
                    Assert.AreEqual(100d, item.c, "item.c");
                }
            }
        }

        private static T GetConstant<T>(T constant)
        {
            return constant;
        }

        [TestMethod]
        public void ProjectionPlanCompilerVisitMethodCallToDataCollectionEmptyTest()
        {
            // TODO: file bug for misleading error message.
            Exception exception = TestUtil.RunCatching(() =>
                {
                    string xml = EmptyFeed;
                    var q = from s in context.CreateQuery<LittleState2>("States")
                            select new LittleState2()
                            {
                                Cities = new DataServiceCollection<LittleCity>()
                            };
                    foreach (var item in CreateTestMaterializeAtom(xml, q)) { }
                });
            TestUtil.AssertExceptionExpected(exception, true);
        }

        [TestMethod]
        public void ProjectionPlanCompilerVisitMethodCallToDataCollectionTrackingTest()
        {
            string cities = FeedStart +
                AnyEntry("c1", "<d:BigCityID>100</d:BigCityID>", null) +
                AnyEntry("c2", "<d:BigCityID>200</d:BigCityID>", null) +
                "</feed>";
            string state = AnyEntry("s1", "<d:ID>1</d:ID>", Link(false, "Cities", cities));
            string xml = FeedStart + state + "</feed>";
            var query = from s in context.CreateQuery<LittleState2>("States")
                        select new
                        {
                            citiesAnon = s.Cities,
                            citiesAnonList = s.Cities.ToList(),
                            citiesAnonCollection = new DataServiceCollection<LittleCity>(s.Cities, TrackingMode.None),
                            state = new LittleState2()
                            {
                                ID = s.ID,
                                Cities = new DataServiceCollection<LittleCity>(s.Cities)
                            }
                        };

            LittleCity city = null;
            foreach (var item in this.CreateTestMaterializeAtom(xml, query))
            {
                Assert.IsNotNull(item.citiesAnon, "item.citiesAnon");
                Assert.IsNotNull(item.citiesAnonList, "item.citiesAnonList");
                Assert.IsNotNull(item.citiesAnonCollection, "item.citiesAnonCollection");
                Assert.IsNotNull(item.state, "item.state");

                Assert.AreEqual(2, item.citiesAnon.Count(), "item.citiesAnon.Count()");
                Assert.AreEqual(2, item.citiesAnonList.Count, "item.citiesAnonList.Count");
                Assert.AreEqual(2, item.citiesAnonCollection.Count, "item.citiesAnonCollection.Count");

                Assert.AreEqual(1, item.state.ID, "item.state.ID");
                Assert.IsNotNull(item.state.Cities, "item.state.Cities");
                Assert.AreEqual(2, item.state.Cities.Count, "item.state.Cities.Count");

                city = item.state.Cities.First();
            }

            AssertEntityCount(3, "one state and two cities");
            AssertLinkCount(2, "state to each city");

            // Make sure that the city is indeed being tracked.
            city.Name = "new name";
            Assert.AreEqual(EntityStates.Modified, this.context.GetEntityDescriptor(city).State, "this.context.GetEntityDescriptor(city).State");
        }

        #endregion ProjectionPlanCompiler tests.

        #region Projected collections of primitives.

        // This is a simple scan of the following:
        // ProjectedType:              int,int?,string,Address
        // ProjectedTypeSourceType:    EntityType,ComplexType,Collection
        // CollectionType:             IEnumerable,IEnumerableOfT,ArrayList,ListOfT,CollectionOfT,HashSetOfT,ReadOnlyCollectionOfT,DataServiceCollectionOfT
        // CollectionTypeCasts:        SameType,Upcasts
        // CollectionTypeMethods:      ToArray,ToList,IntoDataServiceCollection
        // TargetType:                 IntoAnon,IntoEntity
        // SameLevelProjectionCount:   1,3
        // DepthLevelProjectionCount:  1,3
        // ProjectionOrder:            BeforeEntity,AfterEntity
        // ServerDrivenPaging:         true,false
        // ServerResults:              Empty,EmptyAfterSDP,1,5
        // NavigationBeforeSelect:     0,1,4

        [TestMethod]
        public void ProjectPrimitiveCollection_0_Negative()
        {
            // ProjectedType	int?
            // ProjectedTypeSourceType	EntityType
            // CollectionType	ReadOnlyCollectionOfT
            // CollectionTypeCasts	SameType
            // CollectionTypeMethods	IntoDataServiceCollection
            // TargetType	IntoEntity
            // SameLevelProjectionCount	3
            // DepthLevelProjectionCount	1
            // ProjectionOrder	AfterEntity
            // ServerDrivenPaging	FALSE
            // ServerResults	1
            // NavigationBeforeSelect	1
            var q = from t in this.context.CreateQuery<DoubleMemberTypedEntity<int, int, ReadOnlyCollection<int?>>>("T")
                    select new DoubleMemberTypedEntity<int, int, ReadOnlyCollection<int?>>()
                    {
                        ID = t.ID,
                        Member = t.Member,
                        Member2 = t.Member2
                    };

            // Can't do anything very interesting with this, as there is no acceptable wire format.
            foreach (var item in CreateTestMaterializeAtom(EmptyFeed, q))
            {
                Assert.IsNotNull(item, "item");
            }

            string xml = FeedStart + AnyEntry("e1", "<d:ID>1</d:ID><d:Member>2</d:Member><d:Member2>3</d:Member2>", null) + "</feed>";
            TestUtil.AssertEnumerableExceptionMessage(CreateTestMaterializeAtom(xml, q), "InvalidOperationException");
        }

        [TestMethod]
        public void ProjectPrimitiveCollection_0()
        {
            // ProjectedType: int?
            // ProjectedTypeSourceType: Collection
            // CollectionType: ReadOnlyCollectionOfT
            // CollectionTypeCasts: SameType
            // CollectionTypeMethods: IntoDataServiceCollection
            // SameLevelProjectionCount: 3
            // DepthLevelProjectionCount: 1
            // ProjectionOrder: AfterEntity
            // ServerDrivenPaging: true
            // ServerResults: 1
            // NavigationBeforeSelect: 1

            {
                // The problem with this configuration is that the DoubleMemberTypedEntity does not support
                // a ReadOnlyCollection constructor, so we'll also try with ProjectionOrder: Single
                var q = from t in this.context.CreateQuery<DoubleMemberTypedEntity<int, double, ReadOnlyCollection<TypedEntity<int, IEnumerable<int?>>>>>("T")
                        select new
                        {
                            id = t.ID,
                            e = t,
                            values = new DataServiceCollection<int?>(t.Member2.Select(e => e.Member) as IEnumerable<int?>)
                        };
                string nestedFeed = FeedStart +
                    AnyEntry("e2", "<d:Member>100</d:Member>", null) +
                    NextLink("http://next-link/") + "</feed>";
                string xml = FeedStart + AnyEntry("e1", "<d:ID>1</d:ID><d:Member>1.1</d:Member>", Link(false, "Member2", nestedFeed)) + "</feed>";
                TestUtil.AssertEnumerableExceptionMessage(CreateTestMaterializeAtom(xml, q), "InvalidOperationException");
            }

            {
                // This variation still fails, because the IEnumerable in the top-level member is not recognized as
                // a collection type (it doesn't have add/set).
                var q = from t in this.context.CreateQuery<DoubleMemberTypedEntity<int, double, IEnumerable<TypedEntity<int, int?>>>>("T")
                        select new
                        {
                            id = t.ID,
                            e = t,
                            values = new DataServiceCollection<int?>(t.Member2.Select(e => e.Member) as IEnumerable<int?>)
                        };
                string nestedFeed = FeedStart +
                    AnyEntry("e2", "<d:Member>100</d:Member>", null) +
                    NextLink("http://next-link/") + "</feed>";
                string xml = FeedStart + AnyEntry("e1", "<d:ID>1</d:ID><d:Member>1.1</d:Member>", Link(false, "Member2", nestedFeed)) + "</feed>";
                TestUtil.AssertEnumerableExceptionMessage(CreateTestMaterializeAtom(xml, q), "InvalidOperationException");
            }

            {
                // Almost there: this variation fails because TrackingMode defaults to Auto.
                var q = from t in this.context.CreateQuery<DoubleMemberTypedEntity<int, double, Collection<TypedEntity<int, int?>>>>("T")
                        select new
                        {
                            id = t.ID,
                            e = t,
                            values = new DataServiceCollection<int?>(t.Member2.Select(e => e.Member) as IEnumerable<int?>)
                        };
                string nestedFeed = FeedStart +
                    AnyEntry("e2", "<d:Member>100</d:Member>", null) +
                    NextLink("http://next-link/") + "</feed>";
                string xml = FeedStart + AnyEntry("e1", "<d:ID>1</d:ID><d:Member>1.1</d:Member>", Link(false, "Member2", nestedFeed)) + "</feed>";
                TestUtil.AssertEnumerableExceptionMessage(CreateTestMaterializeAtom(xml, q), "The DataServiceCollection to be tracked must contain entity typed elements with at least one key property.");
            }

            {
                var q = from t in this.context.CreateQuery<DoubleMemberTypedEntity<int, double, Collection<TypedEntity<int, int?>>>>("T")
                        select new
                        {
                            id = t.ID,
                            e = new DoubleMemberTypedEntity<int, double, Collection<TypedEntity<int, int?>>>()
                            {
                                ID = t.ID,
                                Member = t.Member,
                                Member2 = t.Member2
                            },
                            values = new DataServiceCollection<int?>(t.Member2.Select(e => e.Member) as IEnumerable<int?>, TrackingMode.None)
                        };
                string nestedFeed = FeedStart +
                    AnyEntry("e2", "<d:Member>100</d:Member>", null) +
                    NextLink("http://next-link/") + "</feed>";
                string xml = FeedStart + AnyEntry("e1", "<d:ID>1</d:ID><d:Member>1.1</d:Member>", Link(false, "Member2", nestedFeed)) + "</feed>";
                var response = AtomParserTests.CreateQueryResponse(this.context, AtomParserTests.EmptyHeaders, q, xml);
                foreach (var item in response)
                {
                    Assert.IsNotNull(item, "item");

                    Assert.AreEqual(1, item.id, "item.id");
                    Assert.IsNotNull(item.e, "item.e");
                    Assert.IsNotNull(item.values, "item.values");

                    Assert.AreEqual(1, item.e.ID, "item.e.ID");
                    Assert.AreEqual(1.1, item.e.Member, "item.e.Member");
                    Assert.IsNotNull(item.e.Member2, "item.e.Member2");

                    Assert.AreNotSame(item.values, item.e.Member2);

                    Assert.AreEqual(1, item.e.Member2.Count, "item.e.Member2");
                    Assert.AreEqual(1, item.values.Count, "item.values.Count");

                    Assert.IsNotNull(response.GetContinuation(item.values), "queryResponse.GetContinuation(item.values)");
                    Assert.IsNotNull(item.values.Continuation, "item.values.Continuation");
                    Assert.AreEqual(item.values.Continuation.NextLinkUri, response.GetContinuation(item.values).NextLinkUri);
                    Assert.IsNotNull(response.GetContinuation(item.e.Member2), "queryResponse.GetContinuation(item.e.Member2)");
                    Assert.AreEqual(item.values.Continuation.NextLinkUri, response.GetContinuation(item.e.Member2).NextLinkUri);
                }

                Assert.IsNull(response.GetContinuation());
            }
        }

        [TestMethod]
        public void ProjectPrimitiveCollection_1()
        {
            // ProjectedType: int
            // ProjectedTypeSourceType: EntityType
            // CollectionType: ArrayList
            // CollectionTypeCasts: Upcasts
            // CollectionTypeMethods: ToList
            // SameLevelProjectionCount: 1
            // DepthLevelProjectionCount: 3 -- actually done in _4.
            // ProjectionOrder: BeforeEntity
            // ServerDrivenPaging: false
            // ServerResults: Empty
            // NavigationBeforeSelect: 0
            {
                var q = from t in this.context.CreateQuery<DoubleMemberTypedEntity<int, double, List<TypedEntity<int, int>>>>("T")
                        select new
                        {
                            id = t.ID,
                            values = new ArrayList(t.Member2.Select(t2 => t2.Member) as ICollection),
                            e = new DoubleMemberTypedEntity<int, double, List<TypedEntity<int, int>>>()
                            {
                                ID = t.ID,
                                Member = t.Member,
                                Member2 = t.Member2
                            }
                        };
                string nestedFeed = FeedStart +
                    AnyEntry("e2", "<d:Member>100</d:Member>", null) + "</feed>";
                string xml = FeedStart + AnyEntry("e1", "<d:ID>1</d:ID><d:Member>1.1</d:Member>", Link(false, "Member2", nestedFeed)) + "</feed>";
                var response = AtomParserTests.CreateQueryResponse(this.context, AtomParserTests.EmptyHeaders, q, xml);
                foreach (var item in response)
                {
                    Assert.IsNotNull(item, "item");

                    Assert.AreEqual(1, item.id, "item.id");
                    Assert.IsNotNull(item.values, "item.values");
                    Assert.IsNotNull(item.e, "item.e");
                    Assert.IsNull(response.GetContinuation(item.e.Member2), "response.GetContinuation(item.e.Member2)");

                    Assert.AreEqual(1, item.values.Count, "item.values.Count");

                    Exception accessException = TestUtil.RunCatching(() => response.GetContinuation(item.values));
                    TestUtil.AssertExceptionExpected(accessException, true);
                }

                Assert.IsNull(response.GetContinuation());

                // One more time, with really empty results like this iteration wants.
                response = AtomParserTests.CreateQueryResponse(this.context, AtomParserTests.EmptyHeaders, q, EmptyFeed);
                foreach (var item in response)
                {
                    Assert.Fail("should never get here");
                }
            }
        }

        [TestMethod]
        public void ProjectPrimitiveCollection_2()
        {
            // ProjectedType: string
            // ProjectedTypeSourceType: ComplexType
            // CollectionType: IEnumerableOfT   -- IEnumerable<T> can't be used in the entity type
            // CollectionTypeCasts: SameType
            // CollectionTypeMethods: ToArray
            // SameLevelProjectionCount: 1
            // DepthLevelProjectionCount: 1
            // ProjectionOrder: BeforeEntity
            // ServerDrivenPaging: false
            // ServerResults: EmptyAfterSDP
            // NavigationBeforeSelect: 4
            {
                // This throws an exception early on in the normalizer, because
                // the upcast to IEnumerable<Address> is discarded and the member
                // no longer matches the argument.
                // Client Projections: removing Converts in normalizer breaks certain projections
                var q = from t in this.context.CreateQuery<TypedEntity<int, List<TypedEntity<int, List<TypedEntity<int, List<DoubleMemberTypedEntity<int, double, List<TypedEntity<int, Address>>>>>>>>>>("T")
                        from t2 in t.Member
                        from t3 in t2.Member
                        from t4 in t3.Member
                        where t2.ID == 2 && t.ID == 1 && t3.ID == 3
                        select new
                        {
                            id = t4.ID,
                            values = t4.Member2.Select(t5 => t5.Member).ToArray() as IEnumerable<Address>,
                            e = new DoubleMemberTypedEntity<int, double, List<TypedEntity<int, Address>>>()
                            {
                                ID = t4.ID,
                                Member = t4.Member,
                                Member2 = t4.Member2
                            }
                        };

                // In 3.5, we expect this to assert due to an expression bug in the frameworks.
                // TestUtil.AssertEnumerableExceptionMessage(CreateTestMaterializeAtom(EmptyFeed, q), "does not match the corresponding member type");

                // In 4.0 the expression bug is fixed and the following passes now.
                string nestedFeed = FeedStart + AnyEntry("e2", "<d:Member><d:StreetAddress>street</d:StreetAddress></d:Member>", null) + "</feed>";
                string xml = FeedStart + AnyEntry("e1", "<d:ID>1</d:ID><d:Member>1.1</d:Member>", ProjectionTests.Link(false, "Member2", nestedFeed)) + "</feed>";
                var response = AtomParserTests.CreateQueryResponse(this.context, AtomParserTests.EmptyHeaders, q, xml);
                foreach (var item in response)
                {
                    Assert.IsNotNull(item, "item");
                    Assert.AreEqual(1, item.id, "item.id");
                    Assert.IsNotNull(item.values, "item.values");
                    Assert.IsNotNull(item.e, "item.e");
                    Assert.IsNull(response.GetContinuation(item.e.Member2), "response.GetContinuation(item.e.Member2)");
                    Assert.AreEqual(1, item.values.Count(), "item.values.Count");
                    Assert.AreEqual("street", item.values.First().StreetAddress, "item.values.First().StreetAddress");
                    Exception accessException = TestUtil.RunCatching(() => response.GetContinuation(item.values));
                    TestUtil.AssertExceptionExpected(accessException, true);
                }

                Assert.IsNull(response.GetContinuation());
            }

            {
                // Same as above, without the upcast after .ToArray()
                var q = from t in this.context.CreateQuery<TypedEntity<int, List<TypedEntity<int, List<TypedEntity<int, List<DoubleMemberTypedEntity<int, double, List<TypedEntity<int, Address>>>>>>>>>>("T")
                        from t2 in t.Member
                        from t3 in t2.Member
                        from t4 in t3.Member
                        where t2.ID == 2 && t.ID == 1 && t3.ID == 3
                        select new
                        {
                            id = t4.ID,
                            values = t4.Member2.Select(t5 => t5.Member).ToArray(),
                            e = new DoubleMemberTypedEntity<int, double, List<TypedEntity<int, Address>>>()
                            {
                                ID = t4.ID,
                                Member = t4.Member,
                                Member2 = t4.Member2
                            }
                        };
                string nestedFeed = FeedStart +
                    AnyEntry("e2", "<d:Member><d:StreetAddress>street</d:StreetAddress></d:Member>", null) + "</feed>";
                string xml = FeedStart + AnyEntry("e1", "<d:ID>1</d:ID><d:Member>1.1</d:Member>", Link(false, "Member2", nestedFeed)) + "</feed>";
                var response = AtomParserTests.CreateQueryResponse(this.context, AtomParserTests.EmptyHeaders, q, xml);
                foreach (var item in response)
                {
                    Assert.IsNotNull(item, "item");

                    Assert.AreEqual(1, item.id, "item.id");
                    Assert.IsNotNull(item.values, "item.values");
                    Assert.IsNotNull(item.e, "item.e");
                    Assert.IsNull(response.GetContinuation(item.e.Member2), "response.GetContinuation(item.e.Member2)");

                    Assert.AreEqual(1, item.values.Count(), "item.values.Count");
                    Assert.AreEqual("street", item.values.First().StreetAddress, "item.values.First().StreetAddress");

                    Exception accessException = TestUtil.RunCatching(() => response.GetContinuation(item.values));
                    TestUtil.AssertExceptionExpected(accessException, true);
                }

                Assert.IsNull(response.GetContinuation());
            }
        }

        [TestMethod]
        public void ProjectPrimitiveCollection_3()
        {
            // ProjectedType: Address
            // ProjectedTypeSourceType: EntityType
            // CollectionType: ListOfT
            // CollectionTypeCasts: SameType
            // CollectionTypeMethods: IntoDataServiceCollection
            // SameLevelProjectionCount: 1
            // DepthLevelProjectionCount: 3
            // ProjectionOrder: BeforeEntity
            // ServerDrivenPaging: false
            // ServerResults: 5
            // NavigationBeforeSelect: 4
            {
                var q = from t in this.context.CreateQuery<TypedEntity<int, List<TypedEntity<int, List<TypedEntity<int, List<SelfReferenceTypedEntity<int, List<TypedEntity<int, Address>>>>>>>>>>("T")
                        from t2 in t.Member
                        from t3 in t2.Member
                        from t4 in t3.Member
                        where t2.ID == 2 && t.ID == 1 && t3.ID == 3
                        select new
                        {
                            v = new DataServiceCollection<Address>(t4.Reference.Reference.Reference.Member.Select(m => m.Member), TrackingMode.None)
                        };
                string nestedFeed = FeedStart +
                    AnyEntry("e2", "<d:Member><d:StreetAddress>street</d:StreetAddress></d:Member>", null) +
                    AnyEntry("e3", "<d:Member><d:StreetAddress>street2</d:StreetAddress></d:Member>", null) +
                    "</feed>";
                // Uncommenting the following line makes this test case a repro for:
                // Client Projection: Assert and NullReferenceException in deep projection
                // string ref1 = AnyEntry("ref1", "<d:ID>1</d:ID><d:Member>1.1</d:Member>", Link(false, "Member2", nestedFeed));
                string ref1 = AnyEntry("ref1", "<d:ID>1</d:ID>", Link(false, "Member", nestedFeed));
                string ref2 = AnyEntry("ref2", "<d:ID>2</d:ID>", Link(true, "Reference", ref1));
                string ref3 = AnyEntry("ref3", "<d:ID>3</d:ID>", Link(true, "Reference", ref2));
                string ref4 = AnyEntry("ref4", "<d:ID>3</d:ID>", Link(true, "Reference", ref3));
                string xml = FeedStart + ref4 + "</feed>";
                var response = AtomParserTests.CreateQueryResponse(this.context, AtomParserTests.EmptyHeaders, q, xml);
                foreach (var item in response)
                {
                    Assert.IsNotNull(item, "item");
                    Assert.IsNotNull(item.v, "item.v");
                    Assert.AreEqual(2, item.v.Count, "item.v.Count");
                    Assert.AreEqual("street", item.v.First().StreetAddress, "item.v.First().StreetAddress");
                    Assert.AreEqual("street2", item.v.Last().StreetAddress, "item.v.Last().StreetAddress");
                    Assert.IsNull(response.GetContinuation(item.v), "response.GetContinuation(item.v)");
                    Assert.IsNull(item.v.Continuation, "item.v.Continuation");
                }

                Assert.IsNull(response.GetContinuation());
            }
        }

        [TestMethod]
        public void ProjectPrimitiveCollection_4()
        {
            // ProjectedType: int
            // ProjectedTypeSourceType: ComplexType
            // CollectionType: DataServiceCollectionOfT
            // CollectionTypeCasts: SameType
            // CollectionTypeMethods: IntoDataServiceCollection
            // SameLevelProjectionCount: 3
            // DepthLevelProjectionCount: 1
            // ProjectionOrder: AfterEntity
            // ServerDrivenPaging: true
            // ServerResults: Empty
            // NavigationBeforeSelect: 4
            {
                var q = from t in this.context.CreateQuery<TypedEntity<int, List<TypedEntity<int, List<TypedEntity<int, List<SelfReferenceTypedEntity<int, List<TypedEntity<int, Address>>>>>>>>>>("T")
                        from t2 in t.Member
                        from t3 in t2.Member
                        from t4 in t3.Member
                        where t2.ID == 2 && t.ID == 1 && t3.ID == 3
                        select new
                        {
                            e = t4,
                            v = new DataServiceCollection<string>(t4.Reference.Reference.Reference.Member.Select(m => m.Member.StreetAddress), TrackingMode.None)
                        };

                try
                {
                    CreateTestMaterializeAtom(EmptyFeed, q).ToList();
                }
                catch (Exception e)
                {
                    Assert.Fail("Exception was not expected but thrown: " + e.ToString());
                }
            }

            {
                var q = from t in this.context.CreateQuery<TypedEntity<int, List<TypedEntity<int, List<TypedEntity<int, List<SelfReferenceTypedEntity<int, List<TypedEntity<int, Address>>>>>>>>>>("T")
                        from t2 in t.Member
                        from t3 in t2.Member
                        from t4 in t3.Member
                        where t2.ID == 2 && t.ID == 1 && t3.ID == 3
                        select new
                        {
                            e = t4,
                            v = new DataServiceCollection<int>(t4.Reference.Reference.Reference.Member.Select(m => m.ID), TrackingMode.None)
                        };
                string nestedFeed = FeedStart +
                    AnyEntry("e2", "<d:ID>1</d:ID><d:Member><d:StreetAddress>street</d:StreetAddress></d:Member>", null) +
                    NextLink("http://next-link/") +
                    "</feed>";
                string ref1 = AnyEntry("ref1", "<d:ID>1</d:ID>", Link(false, "Member", nestedFeed));
                string ref2 = AnyEntry("ref2", "<d:ID>2</d:ID>", Link(true, "Reference", ref1));
                string ref3 = AnyEntry("ref3", "<d:ID>3</d:ID>", Link(true, "Reference", ref2));
                string ref4 = AnyEntry("ref4", "<d:ID>4</d:ID>", Link(true, "Reference", ref3));
                string xml = FeedStart + ref4 + "</feed>";
                var response = AtomParserTests.CreateQueryResponse(this.context, AtomParserTests.EmptyHeaders, q, xml);
                foreach (var item in response)
                {
                    Assert.IsNotNull(item, "item");
                    Assert.IsNotNull(item.e, "item.e");
                    Assert.IsNotNull(item.v, "item.v");

                    Assert.AreEqual(4, item.e.ID, "item.e.ID");
                    Assert.IsNull(item.e.Reference, "item.e.Reference");

                    Assert.AreEqual(1, item.v.Count, "item.v.Count");
                    Assert.IsNotNull(response.GetContinuation(item.v), "response.GetContinuation(item.v)");
                    Assert.IsNotNull(item.v.Continuation, "item.v.Continuation");
                    Assert.AreEqual("http://next-link/", item.v.Continuation.NextLinkUri.OriginalString, "item.v.Continuation.NextLinkUri.OriginalString");
                }

                Assert.IsNull(response.GetContinuation());
            }
        }

        [TestMethod]
        public void ProjectPrimitiveCollection_5()
        {
            // Just finish adding coverage for .ToList() and other collection types:
            // HashSetOfT, IEnumerable
            {
                Trace.WriteLine("V1 version of HashSet support.");
                var q = from t in this.context.CreateQuery<DoubleMemberTypedEntity<int, double, HashSet<TypedEntity<int, int?>>>>("T")
                        select t;
                string nestedFeed = FeedStart +
                    AnyEntry("e2", "<d:Member>100</d:Member>", null) +
                    "</feed>";
                string xml = FeedStart + AnyEntry("e1", "<d:ID>1</d:ID><d:Member>1.1</d:Member>", Link(false, "Member2", nestedFeed)) + "</feed>";
                var response = AtomParserTests.CreateQueryResponse(this.context, AtomParserTests.EmptyHeaders, q, xml);
                foreach (var item in response)
                {
                    Assert.IsNotNull(item, "item");
                    Assert.AreEqual(1, item.ID, "item.ID");
                    Assert.AreEqual(1.1, item.Member, "item.Member");
                    Assert.IsNotNull(item.Member2, "item.Member2");

                    Assert.AreEqual(1, item.Member2.Count, "item.Member2.Count");
                    Assert.AreEqual(100, item.Member2.Single().Member, "item.Member2.Single().Member");
                    Assert.IsNull(response.GetContinuation(item.Member2), "response.GetContinuation(item.Member2)");
                }
            }

            this.ClearContext();

            {
                var q = from t in this.context.CreateQuery<DoubleMemberTypedEntity<int, double, HashSet<TypedEntity<int, int?>>>>("T")
                        select new
                        {
                            e = new DoubleMemberTypedEntity<int, double, HashSet<TypedEntity<int, int?>>>()
                            {
                                ID = t.ID,
                                Member = t.Member,
                                Member2 = t.Member2
                            },
                        };

                string nestedFeed = FeedStart +
                    AnyEntry("e2", "<d:Member>100</d:Member>", null) +
                    "</feed>";
                string xml = FeedStart + AnyEntry("e1", "<d:ID>1</d:ID><d:Member>1.1</d:Member>", Link(false, "Member2", nestedFeed)) + "</feed>";
                var response = AtomParserTests.CreateQueryResponse(this.context, AtomParserTests.EmptyHeaders, q, xml);
                foreach (var item in response)
                {
                    Assert.IsNotNull(item, "item");

                    Assert.IsNotNull(item.e, "item.e");

                    Assert.AreEqual(1, item.e.ID, "item.e.ID");
                    Assert.AreEqual(1.1, item.e.Member, "item.e.Member");
                    Assert.IsNotNull(item.e.Member2, "item.e.Member2");

                    Assert.AreEqual(1, item.e.Member2.Count, "item.e.Member2");
                    Assert.IsNull(response.GetContinuation(item.e.Member2), "queryResponse.GetContinuation(item.e.Member2)");
                }
            }

            this.ClearContext();

            {
                var q = from t in this.context.CreateQuery<DoubleMemberTypedEntity<int, double, HashSet<TypedEntity<int, int?>>>>("T")
                        select new
                        {
                            id = t.ID,
                            //values = t.Member2,
                            valuesAsList = t.Member2.Select(m => m.Member).ToList(),
                        };
                string nestedFeed = FeedStart +
                    AnyEntry("e2", "<d:Member m:null='true' />", null) +
                    NextLink("http://next-link/") + "</feed>";
                string xml = FeedStart + AnyEntry("e1", "<d:ID>1</d:ID><d:Member>1.1</d:Member>", Link(false, "Member2", nestedFeed)) + "</feed>";
                var response = AtomParserTests.CreateQueryResponse(this.context, AtomParserTests.EmptyHeaders, q, xml);
                foreach (var item in response)
                {
                    Assert.IsNotNull(item, "item");

                    Assert.AreEqual(1, item.id, "item.id");
                    //Assert.IsNotNull(item.values, "item.values");

                    //Assert.AreEqual(1, item.values.Count, "item.values.Count");
                    Assert.AreEqual(1, item.valuesAsList.Count, "item.values.Count");
                    Assert.IsFalse(item.valuesAsList.Single().HasValue, "item.valuesAsList.Single().HasValue");

                    //Assert.IsNotNull(response.GetContinuation(item.values), "queryResponse.GetContinuation(item.values)");
                    Assert.IsNotNull(response.GetContinuation(item.valuesAsList), "queryResponse.GetContinuation(item.valuesAsList)");
                }

                Assert.IsNull(response.GetContinuation());
            }
        }

        #endregion Projected collections of primitives.

        public class TypedEntityAnon<TID, TMember>
        {
            public TypedEntityAnon(TID Identifier, TMember Member)
            {
                this.Identifier = Identifier;
                this.Member = Member;
            }

            public TID Identifier { get; private set; }
            public TMember Member { get; private set; }
        }

        public static Expression ApplySelectToExpression(Expression source, LambdaExpression expression)
        {
            //System.Text.StringBuilder sb = new System.Text.StringBuilder();
            // foreach (MethodInfo m in typeof(Queryable).GetMethods()) sb.AppendLine(m.ToString());
            //foreach (MethodInfo m in typeof(Enumerable).GetMethods()) sb.AppendLine(m.ToString());
            //Trace.WriteLine(sb.ToString());

            // MethodInfo method = typeof(Queryable).GetMethods().Where(m => m.ToString() == "System.Linq.IQueryable`1[TResult] Select[TSource,TResult](System.Linq.IQueryable`1[TSource], System.Linq.Expressions.Expression`1[System.Func`2[TSource,TResult]])").Single();
            MethodInfo method = typeof(Enumerable).GetMethods().Where(m => m.ToString() == "System.Collections.Generic.IEnumerable`1[TResult] Select[TSource,TResult](System.Collections.Generic.IEnumerable`1[TSource], System.Func`2[TSource,TResult])").Single();
            Type sourceElementType = TestUtil.GetIEnumerableElement(source.Type);
            return Expression.Call(null, method.MakeGenericMethod(sourceElementType, expression.Body.Type), source, expression);
        }

        public static IQueryable ApplySelect(IQueryable source, LambdaExpression expression)
        {
            MethodInfo method = typeof(ProjectionTests).GetMethod("ApplySelectWithT");
            return method.MakeGenericMethod(source.ElementType, expression.Body.Type).Invoke(null, new object[] { source, expression }) as IQueryable;
        }

        public static IQueryable<TResult> ApplySelectWithT<TSource, TResult>(IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
        {
            return source.Select(selector);
        }

        public static DataServiceQuery CreateQueryOfT(DataServiceContext context, Type entityType, string entitySetName)
        {
            MethodInfo method = typeof(ProjectionTests).GetMethod("CreateQueryOfTWithT");
            return (DataServiceQuery)method.MakeGenericMethod(entityType).Invoke(null, new object[] { context, entitySetName });
        }

        public static DataServiceQuery<T> CreateQueryOfTWithT<T>(DataServiceContext context, string entitySetName)
        {
            return context.CreateQuery<T>(entitySetName);
        }

        private void ClearContext()
        {
            DataServiceContextTests.ClearContext(this.context);
        }

        #region Payload builder helpers.

        public static string LinkNav(bool single, string name, string href)
        {
            string typeName = single ? "entry" : "feed";
            return
                "<link rel='http://docs.oasis-open.org/odata/ns/related/" + name +
                "' type='application/atom+xml;type=" + typeName + "' title='" + name + "' href='" + href + "'/>";
        }

        public static string LinkRelationship(string name, string href)
        {
            return
                "<link rel='http://docs.oasis-open.org/odata/ns/relatedlinks/" + name +
                "' type='application/xml' title='" + name + "' href='" + href + "'/>";
        }

        public static string Link(bool single, string name, string content)
        {
            string typeName = single ? "entry" : "feed";
            // Looks like we will throw an error if we find newline between content and link or after link
            // (XNode report as text with \n)
            // Also between link and m:inline
            // TODO: file bug
            return
                "<link rel='http://docs.oasis-open.org/odata/ns/related/" + name +
                "' type='application/atom+xml;type=" + typeName + "' title='" + name + "' href='foo'>" +
                "<m:inline>" + content + "</m:inline></link>";
        }

        internal static string LinkEdit(string href)
        {
            return "<link rel='edit' href='" + href + "' />";
        }

        internal static string LinkEditMedia(string href, string etag)
        {
            string result = "<link rel='edit-media' href='" + href + "' ";
            if (etag != null)
            {
                result += "m:etag='" + etag + "' ";
            }

            return result + "/>";
        }

        internal static string LinkSelf(string href)
        {
            return "<link rel='self' href='" + href + "' />";
        }

        internal static string NextLink(string uri)
        {
            return AtomParserTests.NextLink(uri);
        }

        internal static string PlayerEntry(string id, string properties)
        {
            return "<entry><id>http://localhost/" + id + "</id>" +
                "<link rel='edit' href='" + id + "'/>" +
                "<content type='application/xml'><m:properties>" + properties +
                "</m:properties></content></entry>";
        }

        internal static string TeamEntry(string id, string properties)
        {
            return TeamEntry(id, properties, null);
        }

        internal static string TeamEntry(string id, string properties, string links)
        {
            return AnyEntry(id, properties, links);
        }

        internal static string AnyEntry(string id, string properties, string links)
        {
            return AtomParserTests.AnyEntry(id, properties, links);
        }

        #endregion Payload builder helpers.

        #region Assert helpers.

        internal static void AssertLinkCountForContext(int expectedCount, string description, DataServiceContext context)
        {
            int actualCount = context.Links.Count;
            if (expectedCount != actualCount)
            {
                string message = "Expected " + expectedCount + " link counts for " + description +
                    " but found " + actualCount;
                foreach (var l in context.Links)
                {
                    message += "\r\n" + l.Source + "." + l.SourceProperty + " = " + l.Target + " [" + l.State + "]";
                }

                Assert.Fail(message);
            }
        }

        internal static void AssertEntityCountForContext(int expectedCount, string description, DataServiceContext context)
        {
            int actualCount = context.Entities.Count;
            if (expectedCount != actualCount)
            {
                string message = "Expected " + expectedCount + " entity counts for " + description +
                    " but found " + actualCount;
                foreach (var e in context.Entities)
                {
                    message += "\r\n" + e.Entity.ToString() + " [" + e.State + "]";
                }

                Assert.Fail(message);
            }
        }

        internal void AssertLinkCount(int expectedCount, string description)
        {
            AssertLinkCountForContext(expectedCount, description, this.context);
        }

        internal void AssertEntityCount(int expectedCount, string description)
        {
            AssertEntityCountForContext(expectedCount, description, this.context);
        }

        #endregion Assert helpers.

        #region Materialization API helpers.

        public static XmlReader ToXml(string text)
        {
            return AtomParserTests.ToXml(text);
        }

        internal IEnumerable<T> CreateTestMaterializeAtom<T>(
            string text,
            IQueryable<T> query)
        {
            DataServiceQuery<T> q = (DataServiceQuery<T>)query;
            foreach (object o in this.CreateTestMaterializeAtomEnumerable(text, q))
            {
                yield return (T)o;
            }
        }

        internal IEnumerable CreateTestMaterializeAtomEnumerable(
            string text,
            DataServiceQuery query)
        {
            return CreateTestMaterializeAtomEnumerable(this.context, text, query);
        }

        internal static IEnumerable CreateTestMaterializeAtomEnumerable(
            DataServiceContext context,
            string text,
            DataServiceQuery query)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "application/atom+xml");
            var bytes = System.Text.UTF8Encoding.UTF8.GetBytes(text);
            TestResponseMessage responseMessage = new TestResponseMessage(headers, (int)System.Net.HttpStatusCode.OK, () => new System.IO.MemoryStream(bytes));

            return CreateMaterializeAtom(context, responseMessage, query, context.MergeOption);
        }


        /// <summary>
        /// No (Yield return) support for iterators  in VB, rewriting this method as a static method so that I can call this from the 
        /// VB.NET Client Projection tests.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        /// <param name="context"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static IEnumerable<T> CreateTestMaterializeAtom<T>(
             string text,
             DataServiceContext context,
             IQueryable<T> query)
        {
            DataServiceQuery<T> q = (DataServiceQuery<T>)query;
            foreach (object o in ProjectionTests.CreateTestMaterializeAtomEnumerable(context, text, q))
            {
                yield return (T)o;
            }
        }

        public static object CreateQueryComponents(Type type)
        {
            // QueryComponents(Uri uri, Version version, Type lastSegmentType, LambdaExpression projection, Dictionary<Expression, Expression> normalizerRewrites)
            Type queryComponentsType = typeof(DataServiceQuery).Assembly.GetType("Microsoft.OData.Client.QueryComponents", true);
            var constructors = queryComponentsType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
            var constructor = constructors.Where(c => c.GetParameters().Length == 5).Single();
            return constructor.Invoke(new object[] { null, new Version(0, 0), type, null, null });
        }

        public static IEnumerable CreateMaterializeAtom(
            DataServiceContext context,
            Microsoft.OData.Core.IODataResponseMessage message,
            DataServiceQuery query,
            MergeOption mergeOption)
        {
            // MaterializeAtom(ResponseInfo responseInfo, QueryComponents queryComponents, ProjectionPlan plan, IODataResponseMessage responseMessage)
            Type queryType = query.GetType();
            MethodInfo translateMethod = queryType.GetMethod("QueryComponents", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            var modelProperty = typeof(DataServiceContext).GetProperty("Model", BindingFlags.NonPublic | BindingFlags.Instance);
            var model = modelProperty.GetValue(context, null);

            object queryComponents = translateMethod.Invoke(query, new object[] { model });
            Type materializeAtomType = typeof(DataServiceQuery).Assembly.GetType("Microsoft.OData.Client.MaterializeAtom", true);
            var constructor = materializeAtomType.GetConstructors(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic).Where(c => c.GetParameters().Any(p => p.ParameterType == typeof(Microsoft.OData.Core.IODataResponseMessage))).Single();
            object plan = null;
            var payloadKindPropertyInfo = queryType.GetProperty("PayloadKind", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            ODataPayloadKind payloadKind = (ODataPayloadKind)payloadKindPropertyInfo.GetValue(query, null);
            return (IEnumerable)constructor.Invoke(new object[] { CreateDeserializationInfo(context), queryComponents, plan, message, payloadKind });
        }

        public static IEnumerable CreateMaterializeAtom(
            DataServiceContext context,
            string text,
            Type expectedType)
        {
            // MaterializeAtom(ResponseInfo responseInfo, QueryComponents queryComponents, ProjectionPlan plan, IODataResponseMessage responseMessage)
            object responseInfo = CreateDeserializationInfo(context);
            object queryComponents = CreateQueryComponents(expectedType);
            Type materializeAtomType = typeof(DataServiceQuery).Assembly.GetType("Microsoft.OData.Client.MaterializeAtom", true);
            var constructor = materializeAtomType.GetConstructors(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic).Where(c => c.GetParameters().Any(p => p.ParameterType == typeof(Microsoft.OData.Core.IODataResponseMessage))).Single();
            object plan = null;
            IODataResponseMessage message = CreateResponseMessage(text);
            return (IEnumerable)constructor.Invoke(new object[] { responseInfo, queryComponents, plan, message, ODataPayloadKind.Unsupported });
        }

        public static IODataResponseMessage CreateResponseMessage(string text)
        {
            string contentType = "text/plain";
            using (var reader = new System.IO.StringReader(text))
            using (var xmlReader = XmlReader.Create(reader, new XmlReaderSettings { IgnoreWhitespace = true }))
            {
                try
                {
                    xmlReader.Read();
                    if (xmlReader.IsStartElement() &&
                        (string.Equals(xmlReader.LocalName, "entry", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(xmlReader.LocalName, "feed", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(xmlReader.LocalName, "service", StringComparison.OrdinalIgnoreCase)))
                    {
                        contentType = "application/atom+xml";
                    }
                    else
                    {
                        contentType = "application/xml";
                    }
                }
                catch
                {
                }
            }
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-Type", contentType);
            var bytes = System.Text.UTF8Encoding.UTF8.GetBytes(text);
            return new TestResponseMessage(headers, 200, () => new System.IO.MemoryStream(bytes));
        }

        public static object CreateDeserializationInfo(DataServiceContext context)
        {
            Type requestInfoType = typeof(DataServiceContext).Assembly.GetTypes().First(t => t.Name.Contains("RequestInfo"));
            ConstructorInfo ctor = requestInfoType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).Single(c => c.GetParameters().Count() == 2);
            object requestInfo = ctor.Invoke(new object[] { context , false});
            MethodInfo method = requestInfoType.GetMethod("GetDeserializationInfo", BindingFlags.Instance | BindingFlags.NonPublic);
            return method.Invoke(requestInfo, new object[] { null });
        }

        #endregion Materialization API helpers.

        #region Inner types.

        public class MyTeamCollection : List<Team>
        {

        }

        public class MyTeamCollectionNoCtor : List<Team>
        {
            private MyTeamCollectionNoCtor() { }
        }

        [Microsoft.OData.Client.Key(new string[] { "TeamID" })]
        public class TeamWithNames // : Team -- if we make this inherit, metadata loading fails because of List<string>
        {
            public int TeamID { get; set; }

            public string TeamName { get; set; }
            public string City { get; set; }
            public List<Player> Players { get; set; }
            public List<string> PlayerNames { get; set; }
        }

        #endregion Inner types.
    }
}

namespace AstoriaUnitTests.Tests
{
    using Microsoft.OData.Core;
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// IODataResponseMessage interface implementation
    /// </summary>
    class TestResponseMessage : IODataResponseMessage
    {
        /// <summary>Cached headers.</summary>
        private Dictionary<string, string> headers;

        private int statusCode;

        /// <summary>A func which returns the response stream.</summary>
        private Func<Stream> getResponseStream;

#if DEBUG
        /// <summary>set to true once the GetStream was called.</summary>
        private bool streamReturned;
#endif

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="httpResponse">The response object to wrap.</param>
        /// <param name="getResponseStream">A func which returns the response stream.</param>
        internal TestResponseMessage(Dictionary<string, string> headers, int statusCode, Func<Stream> getResponseStream)
        {
            Debug.Assert(headers != null, "headers != null");
            Debug.Assert(getResponseStream != null, "getResponseStream != null");

            this.headers = headers;
            this.statusCode = statusCode;
            this.getResponseStream = getResponseStream;
        }

        /// <summary>
        /// Returns the collection of response headers.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Headers
        {
            get
            {
                return this.headers;
            }
        }

        /// <summary>
        /// The response status code.
        /// </summary>
        public int StatusCode
        {
            get
            {
                return this.statusCode;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Returns the value of the header with the given name.
        /// </summary>
        /// <param name="headerName">Name of the header.</param>
        /// <returns>Returns the value of the header with the given name.</returns>
        public string GetHeader(string headerName)
        {
            if (this.headers.ContainsKey(headerName))
            {
                return this.headers[headerName];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Sets the value of the header with the given name.
        /// </summary>
        /// <param name="headerName">Name of the header.</param>
        /// <param name="headerValue">Value of the header.</param>
        public void SetHeader(string headerName, string headerValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the stream to be used to read the response payload.
        /// </summary>
        /// <returns>Stream from which the response payload can be read.</returns>
        public Stream GetStream()
        {
#if DEBUG
            Debug.Assert(!this.streamReturned, "The GetStream can only be called once.");
            this.streamReturned = true;
#endif

            Stream responseStream = this.getResponseStream();
            Debug.Assert(responseStream != null, "The func to get the response stream returned null.");
            return responseStream;
        }

        ///// <summary>
        ///// Resolves the uri from payload.
        ///// </summary>
        ///// <param name="baseUri">The base URI currently in scope.</param>
        ///// <param name="payloadUri">The payload URI.</param>
        ///// <returns>The resolved URI.</returns>
        //public Uri ResolveUrl(Uri baseUri, Uri payloadUri)
        //{
        //    if (!payloadUri.IsAbsoluteUri)
        //    {
        //        if (baseUri == null)
        //        {
        //            if (this.resolver != null)
        //            {
        //                return this.resolver.CreateAbsoluteUriIfNeeded(payloadUri);
        //            }
        //            else
        //            {
        //                // TODO: Do we need a specific client behavior for URI resolution.
        //                // For now, let's use the default ODL behavior which is to basically "return new Uri(baseUri, payloadUri)".
        //                return payloadUri;
        //            }
        //        }
        //        else
        //        {
        //            return Util.AppendBaseUriAndRelativeUri(baseUri, payloadUri);
        //        }
        //    }
        //    return payloadUri;
        //}
    }
}
//---------------------------------------------------------------------
// <copyright file="InputBinderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using System.Diagnostics;
    using System.Linq;
    using System.Xml;
    using AstoriaUnitTests.Stubs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;


    #endregion Namespaces

    [TestClass]
    public class InputBinderTests
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
        }

        [TestMethod]
        public void InputBinderBindTest()
        {
            // Verify basic binding works.
            // .Bind() is called from these places:
            // - ResourceBinder.AnalyzePredicate (to add filters)
            // - ResourceBinder.MatchTransparentScopeSelector (to find nested selectors)
            // - ResourceBinder.AnalyzeProjection (to analyze SelectMany() collectors)
            // - ResourceBinder.AnalyzeSelectMany (to analyze SelectMany() collectors)
            // - ResourceBinder.TryBindToInput
            //   - ResourceBinder.AnalyzeNavigation (to recognize a projection as a navigation)
            //   - ResourceBinder.AnalyzeResourceSetMethod (to check that only the input is used in orderby-type calls)
            //
            // The transparent identifer "shapes" really depend on how many range variables
            // are in scope at a given moment (they are in scope if they are referenced
            // later - compare the first two tests).
            //
            // See the VB.NET version of this test for variation on how C# and VB.NET
            // form compound identifiers.

            this.context.MergeOption = MergeOption.NoTracking;

            string orderDetailXml = AnyEntry("od1", "<d:OrderID>1</d:OrderID>", null);
            string orderXml = AnyEntry("o1", "<d:ID>1</d:ID>", Link(false, "OrderDetails", FeedStart + orderDetailXml + "</feed>"));
            string customerXml = AnyEntry("c1", "<d:ID>1</d:ID>", Link(false, "Orders", FeedStart + orderXml + "</feed>"));

            {
                Trace.WriteLine("Simple SelectMany, no transparent scope.");
                // [C].Where(c => (c.ID = 1)).SelectMany(c => c.Orders, (c, o) => o)
                var q = from c in this.context.CreateQuery<Customer>("C")
                        where c.ID == 1
                        from o in c.Orders
                        select o;
                string xml = FeedStart + orderXml + "</feed>";
                foreach (var item in CreateTestMaterializeAtom(xml, q))
                {
                    Assert.AreEqual(item.ID, 1);
                }
                Assert.AreEqual("http://localhost/C(1)/Orders", q.ToString());
            }

            {
                Trace.WriteLine("SelectMany and OrderBy and ThenBy, with simple transparent scope (even though transparent scope isn't strictly necessary)");
                // [C].Where(c => (c.ID = 1)).SelectMany(c => c.Orders, (c, o) => new $0(c = c, o = o))
                // .OrderBy(p0 => p0.o.ID).ThenBy(p0 => p0.o.CurrencyAmount)
                // .Select(p0 => <>p0.o)
                var q = from c in this.context.CreateQuery<Customer>("C")
                        where c.ID == 1
                        from o in c.Orders
                        orderby o.ID, o.CurrencyAmount
                        select o;
                string xml = FeedStart + orderXml + "</feed>";
                foreach (var item in CreateTestMaterializeAtom(xml, q))
                {
                    Assert.AreEqual(item.ID, 1);
                }
                Assert.AreEqual("http://localhost/C(1)/Orders?$orderby=ID,CurrencyAmount", q.ToString());
            }

            {
                Trace.WriteLine("Simple SelectMany, with simple transparent scope.");
                // [C].SelectMany(c => c.Orders, (c, o) => new $0(c = c, o = o)).Where(p0 => (p0.c.ID = 1)).Select(p0 => <>p0.o)
                var q = from c in this.context.CreateQuery<Customer>("C")
                        from o in c.Orders
                        where c.ID == 1
                        select o;
                string xml = FeedStart + orderXml + "</feed>";
                foreach (var item in CreateTestMaterializeAtom(xml, q))
                {
                    Assert.AreEqual(item.ID, 1);
                }
                Assert.AreEqual("http://localhost/C(1)/Orders", q.ToString());
            }

            {
                Trace.WriteLine("SelectMany and OrderBy, with simple transparent scope.");
                // [C].SelectMany(c => c.Orders, (c, o) => new $0(c = c, o = o))
                // .OrderBy(p0 => p0.o.ID)
                // .Where(p0 => (p0.c.ID = 1)).Select(p0 => <>p0.o)
                var q = from c in this.context.CreateQuery<Customer>("C")
                        from o in c.Orders
                        where c.ID == 1
                        orderby o.ID
                        select o;
                string xml = FeedStart + orderXml + "</feed>";
                foreach (var item in CreateTestMaterializeAtom(xml, q))
                {
                    Assert.AreEqual(item.ID, 1);
                }
                Assert.AreEqual("http://localhost/C(1)/Orders?$orderby=ID", q.ToString());
            }

            {
                Trace.WriteLine("SelectMany and OrderBy and ThenBy, with simple transparent scope.");
                // [C].SelectMany(c => c.Orders, (c, o) => new $0(c = c, o = o))
                // .OrderBy(p0 => p0.o.ID)
                // .ThenBy(p0 => p0.o.OrderDetails)
                // .Where(p0 => (p0.c.ID = 1)).Select(p0 => <>p0.o)
                var q = from c in this.context.CreateQuery<Customer>("C")
                        from o in c.Orders
                        where c.ID == 1
                        orderby o.ID, o.OrderDetails
                        select o;
                string xml = FeedStart + orderXml + "</feed>";
                foreach (var item in CreateTestMaterializeAtom(xml, q))
                {
                    Assert.AreEqual(item.ID, 1);
                }
                Assert.AreEqual("http://localhost/C(1)/Orders?$orderby=ID,OrderDetails", q.ToString());
            }

            {
                Trace.WriteLine("SelectMany, with one-level compound transparent scope.");
                // [C].SelectMany(c => c.Orders, (c, o) => new $0(c = c, o = o))
                // .SelectMany(p0 => p0.o.OrderDetails, (p0, od) => $1(p0 = p0, od = od))
                // .Where(p1 => p1.p0.c.ID = 1) && (p1.p0.o.ID = 1)))
                // .Select(p1 => p1.od)
                var q = from c in this.context.CreateQuery<Customer>("C")
                        from o in c.Orders
                        from od in o.OrderDetails
                        where c.ID == 1 && o.ID == 2
                        select od;
                string xml = FeedStart + orderDetailXml + "</feed>";
                foreach (var item in CreateTestMaterializeAtom(xml, q))
                {
                    Assert.AreEqual(item.OrderID, 1);
                }
                Assert.AreEqual("http://localhost/C(1)/Orders(2)/OrderDetails", q.ToString());
            }

            {
                Trace.WriteLine("SelectMany and OrderBy, with one-level compound transparent scope.");
                // [C].SelectMany(c => c.Orders, (c, o) => new $0(c = c, o = o))
                // .SelectMany(p0 => p0.o.OrderDetails, (p0, od) => $1(p0 = p0, od = od))
                // .Where(p1 => p1.p0.c.ID = 1) && (p1.p0.o.ID = 1)))
                // .OrderBy(p1 => p1.od.Quantity)
                // .Select(p1 => p1.od)
                var q = from c in this.context.CreateQuery<Customer>("C")
                        from o in c.Orders
                        from od in o.OrderDetails
                        where c.ID == 1 && o.ID == 2
                        orderby od.Quantity
                        select od;
                string xml = FeedStart + orderDetailXml + "</feed>";
                foreach (var item in CreateTestMaterializeAtom(xml, q))
                {
                    Assert.AreEqual(item.OrderID, 1);
                }
                Assert.AreEqual("http://localhost/C(1)/Orders(2)/OrderDetails?$orderby=Quantity", q.ToString());
            }

            {
                Trace.WriteLine("SelectMany, with two-level compound transparent scope.");
                // [T]
                // .SelectMany(t => t.Member, (t, c) => new $0(t = t, c = c))
                // .SelectMany(p0 => p0.c.Orders, (p0, o) => new $1(p0 = p0, o = o))
                // .SelectMany(p1 => p1.o.OrderDetails, (p1, od) => new $2(p1 = p1, od = od))
                // .Where(p2 => (((p2.p1.p0.c.ID = 1) && (p2.p1.o.ID = 1)) && (p2.p1.p0.t.ID = 1)))
                // .Select(p2 => p2.od)
                var q = from t in this.context.CreateQuery<TypedEntity<int, List<Customer>>>("T")
                        from c in t.Member
                        from o in c.Orders
                        from od in o.OrderDetails
                        where c.ID == 1 && o.ID == 2 && t.ID == 3
                        select od;
                string xml = FeedStart + orderDetailXml + "</feed>";
                foreach (var item in CreateTestMaterializeAtom(xml, q))
                {
                    Assert.AreEqual(item.OrderID, 1);
                }
                Assert.AreEqual("http://localhost/T(3)/Member(1)/Orders(2)/OrderDetails", q.ToString());
            }

            {
                Trace.WriteLine("SelectMany and OrderBy, with two-level compound transparent scope");
                // [T]
                // .SelectMany(t => t.Member, (t, c) => new $0(t = t, c = c))
                // .SelectMany(p0 => p0.c.Orders, (p0, o) => new $1(p0 = p0, o = o))
                // .SelectMany(p1 => p1.o.OrderDetails, (p1, od) => new $2(p1 = p1, od = od))
                // .Where(p2 => (((p2.p1.p0.c.ID = 1) && (p2.p1.o.ID = 1)) && (p2.p1.p0.t.ID = 1)))
                // .Select(p2 => p2.od)
                var q = from t in this.context.CreateQuery<TypedEntity<int, List<Customer>>>("T")
                        from c in t.Member
                        from o in c.Orders
                        from od in o.OrderDetails
                        where c.ID == 1 && o.ID == 2 && t.ID == 3
                        orderby od.UnitPrice
                        select od;
                string xml = FeedStart + orderDetailXml + "</feed>";
                foreach (var item in CreateTestMaterializeAtom(xml, q))
                {
                    Assert.AreEqual(item.OrderID, 1);
                }
                Assert.AreEqual("http://localhost/T(3)/Member(1)/Orders(2)/OrderDetails?$orderby=UnitPrice", q.ToString());
            }
        }

        [TestMethod]
        public void InputBinderMemberAccessTest()
        {
            // Verify that the member access through references work.
            string orderDetailXml = AnyEntry("od1", "<d:OrderID>1</d:OrderID>", null);
            string orderXml = AnyEntry("o1", "<d:ID>1</d:ID>", Link(false, "OrderDetails", FeedStart + orderDetailXml + "</feed>"));
            string customerXml = AnyEntry("c1", "<d:ID>1</d:ID>", Link(false, "Orders", FeedStart + orderXml + "</feed>"));

            {
                Trace.WriteLine("Simple SelectMany, with simple transparent scope.");
                // [C].SelectMany(c => c.Orders, (c, o) => new $0(c = c, o = o)).Where(p0 => (p0.c.ID = 1)).Select(p0 => <>p0.o)
                var q = from c in this.context.CreateQuery<Customer>("C")
                        from o in c.Orders
                        where c.ID == 1
                        select o;
                string xml = FeedStart + orderXml + "</feed>";
                foreach (var item in CreateTestMaterializeAtom(xml, q))
                {
                    Assert.AreEqual(item.ID, 1);
                }
                Assert.AreEqual("http://localhost/C(1)/Orders", q.ToString());
            }

            {
                Trace.WriteLine("Simple SelectMany, with simple transparent scope, accessing the target in Where");
                var q = from c in this.context.CreateQuery<Customer>("C")
                        from o in c.Orders
                        where c.ID == 1 && o.ID == 2
                        select o;
                string xml = FeedStart + orderXml + "</feed>";
                foreach (var item in CreateTestMaterializeAtom(xml, q))
                {
                    Assert.AreEqual(item.ID, 1);
                }
                Assert.AreEqual("http://localhost/C(1)/Orders(2)", q.ToString());
            }

            {
                Trace.WriteLine("Simple SelectMany, with simple transparent scope, accessing the target in SelectMany");
                var q = from c in this.context.CreateQuery<Customer>("C")
                        from o in c.Orders
                        where c.ID == 1
                        from od in o.OrderDetails
                        where o.ID == 2
                        select od;
                string xml = FeedStart + orderDetailXml + "</feed>";
                foreach (var item in CreateTestMaterializeAtom(xml, q))
                {
                    Assert.AreEqual(item.OrderID, 1);
                }
                Assert.AreEqual("http://localhost/C(1)/Orders(2)/OrderDetails", q.ToString());
            }
        }

        #region Payload builder helpers.

        private static string Link(bool single, string name, string content)
        {
            return (single) ? AtomMaterializerTests.LinkEntry(name, content) : AtomMaterializerTests.LinkFeed(name, content);
        }

        private static string AnyEntry(string id, string properties, string links)
        {
            return AtomParserTests.AnyEntry(id, properties, links);
        }

        #endregion Payload builder helpers.

        #region Materialization API helpers.

        private IEnumerable<T> CreateTestMaterializeAtom<T>(
            string text,
            IQueryable<T> query)
        {
            return ProjectionTests.CreateTestMaterializeAtom(text, this.context, query);
        }

        #endregion Materialization API helpers.
    }
}

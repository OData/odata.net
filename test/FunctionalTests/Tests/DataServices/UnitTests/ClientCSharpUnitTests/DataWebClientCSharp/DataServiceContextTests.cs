//---------------------------------------------------------------------
// <copyright file="DataServiceContextTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using System.Data.Test.Astoria;
    using System.Diagnostics;
    using System.Linq;
    using AstoriaUnitTests.Stubs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    #endregion Namespaces

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/881
    [TestClass]
    public class DataServiceContextTests
    {
        private static Uri serviceRoot;
        private static HttpBasedWebRequest request;
        private bool callbackFlag;
        private DataServiceContext context;
        private List<IDisposable> cleanups;


        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            TestUtil.ClearConfiguration();
            request = (HttpBasedWebRequest)TestWebRequest.CreateForInProcessWcf();
            request.ServiceType = typeof(OpenWebDataService<CustomDataContext>);
            request.StartService();
            serviceRoot = request.ServiceRoot;
        }

        [TestInitialize]
        public void Initialize()
        {
            this.cleanups = new List<IDisposable>();
            this.cleanups.Add(OpenWebDataServiceHelper.PageSizeCustomizer.Restore());
            OpenWebDataServiceHelper.PageSizeCustomizer.Value = (config, type) => { config.SetEntitySetPageSize("Orders", 1); };
            this.callbackFlag = false;
            this.context = new DataServiceContext(serviceRoot);
            //this.context.EnableAtom = true;
            //this.context.Format.UseAtom();
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.context = null;
            foreach (var disp in this.cleanups)
            {
                disp.Dispose();
            }
            this.cleanups.Clear();

            TestUtil.ClearConfiguration();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            request.Dispose();
            request = null;
            serviceRoot = null;
            TestUtil.ClearConfiguration();
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void BeginLoadPropertyTest()
        {
            // Remove the sleeps to count reliably on callback being called
            for (int i = 0; i < 3; i++)
            {
                Trace.WriteLine("Iterating on variation #" + i);

                var q = this.context.CreateQuery<Customer>("Customers");
                var customer = q.First();

                Assert.IsNotNull(customer, "customer");

                AssertEntityCount(1, "One customer");
                AssertLinkCount(0, "No links");

                Uri uri = new Uri(serviceRoot.OriginalString + "/Customers(1)/Orders");
                this.callbackFlag = false;
                object myState = new object();
                IAsyncResult result;
                if (i == 0)
                {
                    result = this.context.BeginLoadProperty(customer, "Orders", (r) => { this.callbackFlag = true; }, myState);
                }
                else if (i == 1)
                {
                    result = this.context.BeginLoadProperty(customer, "Orders", uri, (r) => { this.callbackFlag = true; }, myState);
                }
                else
                {
                    // Try with continuation, although we can't pass null on the first "go".
                    Exception exception = TestUtil.RunCatching(() =>
                        {
                            result = this.context.BeginLoadProperty(customer, "Orders", (DataServiceQueryContinuation)null, (r) => { this.callbackFlag = true; }, myState);
                        });
                    TestUtil.AssertExceptionExpected(exception, true);
                    result = this.context.BeginLoadProperty(customer, "Orders", uri, (r) => { this.callbackFlag = true; }, myState);
                }

                result.AsyncWaitHandle.WaitOne();

                var qor = this.context.EndLoadProperty(result);
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
                Assert.IsTrue(this.callbackFlag, "this.callbackFlag");
                Assert.AreSame(myState, result.AsyncState, "myState and result.AsyncState");
                AssertEntityCount(1 + 1, "One customer with one order");
                AssertLinkCount(1, "One customer-to-order link");

                Assert.IsNotNull(qor, "qor");
                Assert.IsNotNull(qor.GetContinuation(), "qor.GetContinuation()");

                Trace.WriteLine("Loading from continuation...");
                this.callbackFlag = false;
                result = this.context.BeginLoadProperty(customer, "Orders", qor.GetContinuation(), (r) => { this.callbackFlag = true; }, myState);

                Assert.IsNotNull(result);

                result.AsyncWaitHandle.WaitOne();

                qor = this.context.EndLoadProperty(result);
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
                Assert.IsTrue(this.callbackFlag, "this.callbackFlag");
                Assert.AreSame(myState, result.AsyncState, "myState and result.AsyncState");
                Assert.IsNotNull(qor.GetContinuation(), "qor.GetContinuation()");

                result = this.context.BeginLoadProperty(customer, "Orders", qor.GetContinuation(), (r) => { this.callbackFlag = true; }, myState);
                qor = this.context.EndLoadProperty(result);
                Assert.IsNull(qor.GetContinuation(), "qor.GetContinuation()");

                AssertEntityCount(1 + 2, "One customer with two orders");
                AssertLinkCount(2, "Two customer-to-order links");

                this.ClearContext();
            }
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void LoadPropertyRemoveElementUnChangedSource()
        {
            MergeOption original = this.context.MergeOption;

            try
            {
                foreach (MergeOption mo in new[] { MergeOption.OverwriteChanges, MergeOption.PreserveChanges })
                {
                    foreach (bool usePaging in new[] { false, true })
                    {
                        using (CustomDataContext.CreateChangeScope())
                        using (OpenWebDataServiceHelper.PageSizeCustomizer.Restore())
                        {
                            OpenWebDataServiceHelper.PageSizeCustomizer.Value = (config, type) => { config.SetEntitySetPageSize("Orders", usePaging ? 1 : 5); };
                            Customer c = new Customer() { ID = 0 };
                            Customer c2 = new Customer() { ID = 2 };
                            Order o = new Order() { ID = 102 };

                            this.context.MergeOption = mo;

                            this.context.AttachTo("Customers", c);
                            this.context.AttachTo("Customers", c2);
                            this.context.AttachTo("Orders", o);

                            this.context.AddLink(c, "Orders", o);
                            this.context.SetLink(o, "Customer", c);
                            this.context.SaveChanges();

                            if (usePaging)
                            {
                                DataServiceQueryContinuation continuation = null;
                                do
                                {
                                    QueryOperationResponse response = this.context.LoadProperty(c, "Orders", continuation);
                                    continuation = response.GetContinuation();
                                }
                                while (continuation != null);
                            }
                            else
                            {
                                this.context.LoadProperty(c, "Orders");
                            }

                            this.context.LoadProperty(o, "Customer");

                            if (c.Orders != null)
                            {
                                Assert.IsTrue(c.Orders.Contains(o));
                            }
                            if (o.Customer != null)
                            {
                                Assert.IsTrue(o.Customer.ID == c.ID);
                            }

                            // Remove the link
                            this.context.DeleteLink(c, "Orders", o);
                            this.context.SetLink(o, "Customer", c2);

                            this.context.SaveChanges();

                            if (usePaging)
                            {
                                DataServiceQueryContinuation continuation = null;
                                do
                                {
                                    QueryOperationResponse response = this.context.LoadProperty(c, "Orders", continuation);
                                    continuation = response.GetContinuation();
                                }
                                while (continuation != null);
                            }
                            else
                            {
                                this.context.LoadProperty(c, "Orders");
                            }

                            this.context.LoadProperty(o, "Customer");

                            if (c.Orders != null)
                            {
                                Assert.IsFalse(c.Orders.Contains(o));
                            }
                            if (o.Customer != null)
                            {
                                Assert.IsFalse(o.Customer.ID == c.ID);
                            }
                        }

                        this.ClearContext();
                    }
                }
            }
            finally
            {
                this.context.MergeOption = original;
            }
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void LoadPropertyRemoveElementDeletedSource()
        {
            MergeOption original = this.context.MergeOption;

            try
            {
                foreach (MergeOption mo in new[] { MergeOption.OverwriteChanges, MergeOption.PreserveChanges })
                {
                    using (CustomDataContext.CreateChangeScope())
                    using (OpenWebDataServiceHelper.PageSizeCustomizer.Restore())
                    {
                        OpenWebDataServiceHelper.PageSizeCustomizer.Value = (config, type) => { config.SetEntitySetPageSize("Orders", 5); };
                        Customer c = new Customer() { ID = 0 };
                        Customer c2 = new Customer() { ID = 2 };
                        Order o = new Order() { ID = 102 };

                        this.context.MergeOption = mo;

                        this.context.AttachTo("Customers", c);
                        this.context.AttachTo("Customers", c2);
                        this.context.AttachTo("Orders", o);

                        this.context.AddLink(c, "Orders", o);
                        this.context.SetLink(o, "Customer", c);
                        this.context.SaveChanges();

                        // Mark the object as deleted
                        this.context.DeleteObject(c);

                        this.context.LoadProperty(c, "Orders");

                        var changedDescritors = this.context.Entities.Cast<Descriptor>().Union(this.context.Links.Cast<Descriptor>()).Where(d => d.State != EntityStates.Unchanged);

                        Assert.AreEqual(changedDescritors.Count(), 1);
                        Assert.IsTrue(changedDescritors.Single().State == EntityStates.Deleted);
                        Assert.IsTrue(changedDescritors.Single() is EntityDescriptor);
                    }

                    this.ClearContext();
                }
            }
            finally
            {
                this.context.MergeOption = original;
            }
        }

        private class NarrowCustomer
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public List<NarrowOrder> Orders { get; set; }
            public NarrowCustomer()
            {
            }
        }

        private class NarrowOrder
        {
            public int ID { get; set; }
            public Double DollarAmount { get; set; }
            public NarrowCustomer Customer { get; set; }
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void ExerciseApplyItemsToCollectionViaMergeLists()
        {
            MergeOption original = this.context.MergeOption;

            try
            {
                foreach (MergeOption mo in new[] { MergeOption.OverwriteChanges, MergeOption.PreserveChanges })
                {
                    using (CustomDataContext.CreateChangeScope())
                    using (OpenWebDataServiceHelper.PageSizeCustomizer.Restore())
                    {
                        OpenWebDataServiceHelper.PageSizeCustomizer.Value = (config, type) => { config.SetEntitySetPageSize("Orders", 5); };
                        NarrowCustomer c0 = new NarrowCustomer() { ID = 0 };

                        this.context.MergeOption = mo;

                        this.context.AttachTo("Customers", c0);

                        var customers = this.context.CreateQuery<NarrowCustomer>("Customers")
                            .Where(c => c.ID == 0)
                            .Select(c => new NarrowCustomer() { ID = c.ID, Orders = c.Orders.Select(o => new NarrowOrder() { DollarAmount = o.DollarAmount }).ToList() });

                        foreach (var dummy in customers)
                        {
                        }

                        var customers2 = this.context.CreateQuery<NarrowCustomer>("Customers")
                            .Where(c => c.ID == 0)
                            .Select(c => new NarrowCustomer() { ID = c.ID, Orders = c.Orders.Select(o => new NarrowOrder() { DollarAmount = o.DollarAmount }).ToList() });

                        foreach (var dummy in customers2)
                        {
                        }

                        // Ensure that we still have the same item count
                        Assert.IsTrue(c0.Orders.Count == 2);

                    }

                    this.ClearContext();
                }
            }
            finally
            {
                this.context.MergeOption = original;
            }
        }

        [Ignore] // Remove Atom
        // [TestMethod]
        public void WritingEntityWithControlChars()
        {
            this.context.ResolveName = t => typeof(Customer).FullName;

            // Try a simple exercise with and without the entity handler.

            using (CustomDataContext.CreateChangeScope())
            {
                Customer customer = new Customer();
                customer.Name = "Control character here -> \0x02";
                customer.ID = 1234;
                this.context.AddObject("Customers", customer);
                this.context.SaveChanges();
            }
        }

        public static void ClearContext(DataServiceContext context)
        {
            Debug.Assert(context != null, "context != null");
            foreach (var link in context.Links)
            {
                context.DetachLink(link.Source, link.SourceProperty, link.Target);
            }

            foreach (var entity in context.Entities)
            {
                context.Detach(entity.Entity);
            }
        }

        private void ClearContext()
        {
            ClearContext(this.context);
        }

        #region Assert helpers.

        internal void AssertEntityCount(int expectedCount, string description)
        {
            AssertEntityCountForContext(expectedCount, description, this.context);
        }

        internal void AssertLinkCount(int expectedCount, string description)
        {
            AssertLinkCountForContext(expectedCount, description, this.context);
        }

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
        #endregion Assert helpers.
    }
}

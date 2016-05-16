//---------------------------------------------------------------------
// <copyright file="EntryValueMaterializationPolicyUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Client
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using Microsoft.OData.Client.Materialization;
    using Microsoft.OData.Client.Metadata;
    using System.Linq;
    using AstoriaUnitTests.Tests;
    using Microsoft.OData;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EntryValueMaterializationPolicyUnitTests
    {
        public class TestCustomer
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public List<TestOrder> Orders { get; set; }
        }

        public class TestOrder
        {
            public int ID { get; set; }
            public int Price { get; set; }
        }

        private ClientEdmModel clientEdmModel;
        private IODataMaterializerContext materializerContext;
        private ClientPropertyAnnotation ordersProperty;

        [TestInitialize]
        public void Initialize()
        {
            this.clientEdmModel = new ClientEdmModel(ODataProtocolVersion.V4);
            this.clientEdmModel.GetOrCreateEdmType(typeof(TestCustomer));
            this.clientEdmModel.GetOrCreateEdmType(typeof(TestOrder));
            this.materializerContext = new TestMaterializerContext() { Model = this.clientEdmModel };
            this.ordersProperty = this.clientEdmModel.GetClientTypeAnnotation(typeof(TestCustomer)).GetProperty("Orders", UndeclaredPropertyBehavior.ThrowException);
        }

        [TestMethod]
        public void InputCollectionElementsShouldBePlacedIntoTargetCollection()
        {
            foreach (MergeOption mo in new[] { MergeOption.NoTracking, MergeOption.AppendOnly, MergeOption.PreserveChanges, MergeOption.OverwriteChanges })
            {
                var customer = GetCustomer();
                var orders = GetOrders();

                this.TestApplyItemsToCollection(
                    customer,
                    orders,
                    mo,
                    new TestEntityTracker()
                    {
                        GetLinksFunc = (o, s) => Enumerable.Empty<LinkDescriptor>()
                    },
                    false);

                Assert.IsTrue(customer.Orders.Count == 3);
            }
        }

        [TestMethod]
        public void NonLinkedTargetCollectionElementsShouldBeRemoved()
        {
            foreach (MergeOption mo in new[] { MergeOption.PreserveChanges, MergeOption.OverwriteChanges })
            {
                var customer = GetCustomer();
                var orders = GetOrders();

                foreach (TestOrder o in orders)
                {
                    customer.Orders.Add(o);
                }

                this.TestApplyItemsToCollection(
                    customer,
                    this.GetOverlappingOrders(customer.Orders),
                    mo,
                    new TestEntityTracker()
                    {
                        GetLinksFunc = (o, s) => Enumerable.Empty<LinkDescriptor>()
                    },
                    false);

                // Unlinked orders do not count.
                Assert.IsTrue(customer.Orders.Count == 3);
            }
        }

        [TestMethod]
        public void OverlappingInputCollectionShouldAddOnlyNewItemsToTargetCollection()
        {
            foreach (MergeOption mo in new[] { MergeOption.NoTracking, MergeOption.AppendOnly })
            {
                var customer = GetCustomer();
                var orders = GetOrders();

                foreach (TestOrder o in orders)
                {
                    customer.Orders.Add(o);
                }

                this.TestApplyItemsToCollection(
                    customer,
                    this.GetOverlappingOrders(customer.Orders),
                    mo,
                    new TestEntityTracker()
                    {
                        GetLinksFunc = (o, s) => Enumerable.Empty<LinkDescriptor>()
                    },
                    false);

                // All new orders get placed.
                Assert.IsTrue(customer.Orders.Count == 5);
            }
        }

        [TestMethod]
        public void NotPresentLinksInInputCollectionShouldBeRemovedFromTargetCollection()
        {
            foreach (MergeOption mo in new[] { MergeOption.PreserveChanges, MergeOption.OverwriteChanges })
            {
                var customer = GetCustomer();
                var orders = GetOrders();

                foreach (TestOrder o in orders)
                {
                    customer.Orders.Add(o);
                }

                var currentOrders = orders.ToArray();

                this.TestApplyItemsToCollection(
                    customer,
                    this.GetOverlappingOrders(customer.Orders),
                    mo,
                    new TestEntityTracker()
                    {
                        GetLinksFunc = (o, s) => currentOrders.Select(order => new LinkDescriptor(o, s, order, this.clientEdmModel) { State = EntityStates.Unchanged })
                    },
                    false);

                // Linked orders stay intact but get removed if they were not refreshed, add new item.
                Assert.IsTrue(customer.Orders.Count == 3);
            }
        }

        [TestMethod]
        public void AddedLinksInTargetCollectionNotRemovedFromTargetCollectionForPreserveChanges()
        {
            AddedLinksInTargetCollectionHelper(
                MergeOption.PreserveChanges,
                (c) =>
                {
                    // Added links are not touched for PreserveChanges.
                    Assert.IsTrue(c.Orders.Count == 5);
                });
        }

        [TestMethod]
        public void AddedLinksInTargetCollectionRemovedFromTargetCollectionForOverwriteChanges()
        {
            AddedLinksInTargetCollectionHelper(MergeOption.OverwriteChanges,
                (c) =>
                {
                    // Added links are overwritten for OverwriteChanges.
                    Assert.IsTrue(c.Orders.Count == 3);
                });
        }

        private void AddedLinksInTargetCollectionHelper(MergeOption mo, Action<TestCustomer> validator)
        {
            var customer = GetCustomer();
            var orders = GetOrders();

            foreach (TestOrder o in orders)
            {
                customer.Orders.Add(o);
            }

            var currentOrders = orders.ToArray();

            this.TestApplyItemsToCollection(
                customer,
                this.GetOverlappingOrders(customer.Orders),
                mo,
                new TestEntityTracker()
                {
                    GetLinksFunc = (o, s) => currentOrders.Select(order => new LinkDescriptor(o, s, order, this.clientEdmModel) { State = EntityStates.Added })
                },
                false);

            validator(customer);
        }

        [TestMethod]
        public void DeletedSourceEntityShouldNotDetachLinks()
        {
            foreach (bool isContinuation in new[] { false, true })
                foreach (MergeOption mo in new[] { MergeOption.PreserveChanges, MergeOption.OverwriteChanges })
                {
                    var customer = GetCustomer();
                    var orders = GetOrders();

                    foreach (TestOrder o in orders)
                    {
                        customer.Orders.Add(o);
                    }

                    var currentOrders = orders.ToArray();

                    int numLinkAttachments = 0;
                    int numLinkDetachments = 0;

                    this.TestApplyItemsToCollection(
                        customer,
                        this.GetOverlappingOrders(customer.Orders),
                        mo,
                        new TestEntityTracker()
                        {
                            GetLinksFunc = (o, s) => currentOrders.Select(order => new LinkDescriptor(o, s, order, this.clientEdmModel) { State = EntityStates.Added }),
                            GetEntityDescriptorFunc = (o) => o is TestCustomer ?
                            new EntityDescriptor(this.clientEdmModel) { Entity = o, State = EntityStates.Deleted } :
                            new EntityDescriptor(this.clientEdmModel) { Entity = o, State = EntityStates.Unchanged },
                            AttachLinkAction = (s, sp, t, m) => { numLinkAttachments++; },
                            DetachExistingLinkAction = (l, d) => { numLinkDetachments++; }
                        },
                        isContinuation);

                    // Deleted source should still attach the new links.
                    Assert.IsTrue(numLinkAttachments == 3);

                    if (mo == MergeOption.OverwriteChanges && !isContinuation)
                    {
                        Assert.IsTrue(customer.Orders.Count == 3);
                        // Overwrite results in link detachments.
                        Assert.IsTrue(numLinkDetachments == 2);
                    }
                    else
                    {
                        Assert.IsTrue(customer.Orders.Count == 5);
                        Assert.IsTrue(numLinkDetachments == 0);
                    }
                }
        }

        private TestCustomer GetCustomer()
        {
            return new TestCustomer { ID = 1, Name = "Foo", Orders = new List<TestOrder>() };
        }

        private IEnumerable<TestOrder> GetOrders()
        {
            return new[]
            {
                new TestOrder { ID = 1, Price = 10 },
                new TestOrder { ID = 2, Price = 20 },
                new TestOrder { ID = 3, Price = 30 }
            };
        }

        private IEnumerable<TestOrder> GetOverlappingOrders(IEnumerable<TestOrder> existingOrders)
        {
            List<TestOrder> orders = new List<TestOrder>();
            orders.Add(existingOrders.First());
            orders.Add(new TestOrder { ID = 4, Price = 40 });
            orders.Add(new TestOrder { ID = 5, Price = 50 });
            return orders;
        }

        private void TestApplyItemsToCollection(
            TestCustomer customer,
            IEnumerable orders,
            MergeOption option,
            TestEntityTracker entityTracker,
            bool isContinuation)
        {
            var customerDescriptor = new EntityDescriptor(this.clientEdmModel) { Entity = customer };

            var materializerEntry = MaterializerEntry.CreateEntryForLoadProperty(customerDescriptor, ODataFormat.Json, true);

            materializerEntry.ActualType = this.clientEdmModel.GetClientTypeAnnotation(clientEdmModel.GetOrCreateEdmType(typeof(TestCustomer)));

            var adapter = new EntityTrackingAdapter(
                entityTracker,
                option,
                clientEdmModel,
                new DataServiceContext());

            EntryValueMaterializationPolicy evmp = new EntryValueMaterializationPolicy(
                materializerContext,
                adapter,
                null,
                new Dictionary<IEnumerable, DataServiceQueryContinuation>());

            evmp.ApplyItemsToCollection(
                materializerEntry,
                ordersProperty,
                orders,
                null,
                null,
                isContinuation);

            if (entityTracker.GetEntityDescriptorFunc != null)
            {
                adapter.MaterializationLog.ApplyToContext();
            }
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="OperationT4Tests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Test.OData.Services.TestServices.OperationServiceReference;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Test.OData.Tests.Client.OperationTests
{
    [TestClass]
    public class OperationT4Tests : ODataWCFServiceTestsBase<OperationService>
    {
        public OperationT4Tests()
            : base(Microsoft.Test.OData.Services.TestServices.ServiceDescriptors.OperationServiceDescriptor)
        {
        }

        // TODO : Reactive this test cases after merging entity and complex for writer
        [TestMethod]
        public void FunctionOfEntitiesTakeComplexsReturnEntities()
        {
            var addresses = this.TestClientContext.Customers.ToList().Select(c => c.Address).ToList();
            var getCustomersForAddresses = this.TestClientContext.Customers.GetCustomersForAddresses(addresses);
            var customersForAddresses = getCustomersForAddresses.Execute();
            Assert.AreEqual(2, customersForAddresses.Count());

            var doubleFunction = getCustomersForAddresses.GetCustomersForAddresses(new Collection<Address> { addresses[0] });
            customersForAddresses = doubleFunction.Execute();
            Assert.AreEqual(1, customersForAddresses.Count());

            var tripleFunction = doubleFunction.GetCustomersForAddresses(new Collection<Address> { addresses[0] });
            customersForAddresses = tripleFunction.Execute();
            Assert.AreEqual(1, customersForAddresses.Count());
        }

        [TestMethod]
        public void FunctionOfEntitiesTakeComplexReturnEntity()
        {
            // TODO: change to first customer after GitHub issue 21 is fixed.
            var customer = this.TestClientContext.Customers.ToList()[2];
            var getCustomerForAddress = this.TestClientContext.Customers.GetCustomerForAddress(customer.Address);
            var customerFromFunction = getCustomerForAddress.GetValue();
            Assert.IsNotNull(customerFromFunction);

            var doubleFunction = getCustomerForAddress.GetOrdersFromCustomerByNotes(new Collection<string> { "1111" });
            var ordersFromDoubleFunction = doubleFunction.Execute();
            Assert.AreEqual(1, ordersFromDoubleFunction.Count());
        }

        [TestMethod]
        public void FunctionOfEntityTakeCollectionReturnEntities()
        {
            var customer = this.TestClientContext.Customers.ToList()[2];
            var getOrdersFromCustomerByNotes = customer.GetOrdersFromCustomerByNotes(new Collection<string> { "1111", "2222" });
            var ordersFromFunction = getOrdersFromCustomerByNotes.Execute();
            Assert.AreEqual(1, ordersFromFunction.Count());
        }

        [TestMethod]
        public void FunctionOfEntityTakeComplexReturnEntity()
        {
            var customer = this.TestClientContext.Customers.First();
            var verifyCustomerAddress = customer.VerifyCustomerAddress(customer.Address);
            var customerFromFunction = verifyCustomerAddress.GetValue();
            Assert.IsNotNull(customerFromFunction);

            var doubleFunction = verifyCustomerAddress.VerifyCustomerAddress(customer.Address);
            customerFromFunction = doubleFunction.GetValue();
            Assert.IsNotNull(customerFromFunction);

            var tripleFunction = doubleFunction.VerifyCustomerAddress(customer.Address);
            customerFromFunction = tripleFunction.GetValue();
            Assert.IsNotNull(customerFromFunction);
        }

        [TestMethod]
        public void FunctionOfEntitiesTakeStringReturnEntities()
        {
            var getOrdersByNote = this.TestClientContext.Orders.GetOrdersByNote("1111");
            Assert.IsTrue(getOrdersByNote.RequestUri.AbsoluteUri.EndsWith("GetOrdersByNote(note='1111')"));

            var orders = getOrdersByNote.Execute();
            Assert.AreEqual(2, orders.Count());
        }

        [TestMethod]
        public void FunctionOfEntitiesTakeEntitiesReturnEntities()
        {
            var orders = this.TestClientContext.Orders.ToList();
            var getCustomersByOrder = this.TestClientContext.Customers.GetCustomersByOrders(orders);
            var customers = getCustomersByOrder.Execute();

            Assert.IsTrue(customers.Any());
        }

        [TestMethod]
        public void FunctionOfEntitiesTakeEntityReturnEntity()
        {
            var order = this.TestClientContext.Orders.First();
            var getCustomersByOrder = this.TestClientContext.Customers.GetCustomerByOrder(order);
            var customer = getCustomersByOrder.GetValue();

            Assert.IsNotNull(customer);
        }

        [TestMethod]
        public void FunctionOfEntitiesTakeEntityReferenceReturnEntity()
        {
            var order = this.TestClientContext.Orders.ToList()[1];
            var getCustomersByOrder = this.TestClientContext.Customers.GetCustomerByOrder(order, true);
            Assert.IsTrue(getCustomersByOrder.RequestUri.AbsoluteUri.Contains("odata.id"));

            var customer = getCustomersByOrder.GetValue();

            Assert.IsNotNull(customer);
            Assert.AreEqual(2, customer.ID);
        }

        [TestMethod]
        public void FunctionTakeEntityReferenceUseLocalEntity()
        {
            var order = new Order()
            {
                ID = 1,
            };
            this.TestClientContext.AttachTo("Orders", order);
            var getCustomersByOrder = this.TestClientContext.Customers.GetCustomerByOrder(order, true /*useEntityReference*/);
            Assert.IsTrue(getCustomersByOrder.RequestUri.AbsoluteUri.Contains("odata.id"));

            var customer = getCustomersByOrder.GetValue();

            Assert.IsNotNull(customer);
            Assert.AreEqual(2, customer.ID);
        }

        [TestMethod]
        public void FunctionOfEntitiesTakeEntityReferencesReturnEntities()
        {
            var orders = this.TestClientContext.Orders.ToList();
            var getCustomersByOrders = this.TestClientContext.Customers.GetCustomersByOrders(orders, true /*useEntityReference*/);
            Assert.IsTrue(getCustomersByOrders.RequestUri.AbsoluteUri.Contains("odata.id"));
            var customers = getCustomersByOrders.Execute();

            Assert.IsTrue(customers.Count() > 1);
        }

        [TestMethod]
        public void FunctionOfEntityTakeEntityReturnEntity()
        {
            var customer = this.TestClientContext.Customers.Expand("Orders").Skip(1).First();
            var getCustomerByOrder = customer.VerifyCustomerByOrder(customer.Orders.First());
            customer = getCustomerByOrder.GetValue();

            Assert.IsNotNull(customer);
        }

        [TestMethod]
        public void FunctionImportTakeEntitiesReturnEntities()
        {
            var orders = this.TestClientContext.Orders.ToList();
            var getCustomersByOrders = this.TestClientContext.GetCustomersByOrders(orders);
            Assert.IsFalse(getCustomersByOrders.RequestUri.AbsoluteUri.Contains("odata.id"));
            var customers = getCustomersByOrders.Execute();
            int count = customers.Count();
            Assert.IsTrue(count > 1);

            getCustomersByOrders = this.TestClientContext.GetCustomersByOrders(orders, true);
            Assert.IsTrue(getCustomersByOrders.RequestUri.AbsoluteUri.Contains("odata.id"));
            customers = getCustomersByOrders.Execute();

            Assert.IsTrue(customers.Count() == count);
        }

        [TestMethod]
        public void FunctionImportTakeEntityReturnEntity()
        {
            var order = this.TestClientContext.Orders.ToList()[1];
            var getCustomerByOrder = this.TestClientContext.GetCustomerByOrder(order);
            Assert.IsFalse(getCustomerByOrder.RequestUri.AbsoluteUri.Contains("odata.id"));
            var customer = getCustomerByOrder.GetValue();
            Assert.IsNotNull(customer);
            Assert.AreEqual(2, customer.ID);

            getCustomerByOrder = this.TestClientContext.GetCustomerByOrder(order, true);
            Assert.IsTrue(getCustomerByOrder.RequestUri.AbsoluteUri.Contains("odata.id"));
            customer = getCustomerByOrder.GetValue();

            Assert.IsNotNull(customer);
            Assert.AreEqual(2, customer.ID);
        }

    }
}

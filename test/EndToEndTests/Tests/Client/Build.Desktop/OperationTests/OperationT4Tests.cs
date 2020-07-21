//---------------------------------------------------------------------
// <copyright file="OperationT4Tests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Test.OData.Services.TestServices.OperationServiceReference;
using Xunit;

namespace Microsoft.Test.OData.Tests.Client.OperationTests
{
    public class OperationT4Tests : ODataWCFServiceTestsBase<OperationService>
    {
        public OperationT4Tests()
            : base(Microsoft.Test.OData.Services.TestServices.ServiceDescriptors.OperationServiceDescriptor)
        {
        }

        // TODO : Reactive this test cases after merging entity and complex for writer
        [Fact]
        public void FunctionOfEntitiesTakeComplexsReturnEntities()
        {
            var addresses = this.TestClientContext.Customers.ToList().Select(c => c.Address).ToList();
            var getCustomersForAddresses = this.TestClientContext.Customers.GetCustomersForAddresses(addresses);
            var customersForAddresses = getCustomersForAddresses.Execute();
            Assert.Equal(2, customersForAddresses.Count());

            var doubleFunction = getCustomersForAddresses.GetCustomersForAddresses(new Collection<Address> { addresses[0] });
            customersForAddresses = doubleFunction.Execute();
            Assert.Equal(1, customersForAddresses.Count());

            var tripleFunction = doubleFunction.GetCustomersForAddresses(new Collection<Address> { addresses[0] });
            customersForAddresses = tripleFunction.Execute();
            Assert.Equal(1, customersForAddresses.Count());
        }

        [Fact]
        public void FunctionOfEntitiesTakeComplexReturnEntity()
        {
            // TODO: change to first customer after GitHub issue 21 is fixed.
            var customer = this.TestClientContext.Customers.ToList()[2];
            var getCustomerForAddress = this.TestClientContext.Customers.GetCustomerForAddress(customer.Address);
            var customerFromFunction = getCustomerForAddress.GetValue();
            Assert.NotNull(customerFromFunction);

            var doubleFunction = getCustomerForAddress.GetOrdersFromCustomerByNotes(new Collection<string> { "1111" });
            var ordersFromDoubleFunction = doubleFunction.Execute();
            Assert.Equal(1, ordersFromDoubleFunction.Count());
        }

        [Fact]
        public void FunctionOfEntityTakeCollectionReturnEntities()
        {
            var customer = this.TestClientContext.Customers.ToList()[2];
            var getOrdersFromCustomerByNotes = customer.GetOrdersFromCustomerByNotes(new Collection<string> { "1111", "2222" });
            var ordersFromFunction = getOrdersFromCustomerByNotes.Execute();
            Assert.Equal(1, ordersFromFunction.Count());
        }

        [Fact]
        public void FunctionOfEntityTakeComplexReturnEntity()
        {
            var customer = this.TestClientContext.Customers.First();
            var verifyCustomerAddress = customer.VerifyCustomerAddress(customer.Address);
            var customerFromFunction = verifyCustomerAddress.GetValue();
            Assert.NotNull(customerFromFunction);

            var doubleFunction = verifyCustomerAddress.VerifyCustomerAddress(customer.Address);
            customerFromFunction = doubleFunction.GetValue();
            Assert.NotNull(customerFromFunction);

            var tripleFunction = doubleFunction.VerifyCustomerAddress(customer.Address);
            customerFromFunction = tripleFunction.GetValue();
            Assert.NotNull(customerFromFunction);
        }

        [Fact]
        public void FunctionOfEntitiesTakeStringReturnEntities()
        {
            var getOrdersByNote = this.TestClientContext.Orders.GetOrdersByNote("1111");
            Assert.True(getOrdersByNote.RequestUri.AbsoluteUri.EndsWith("GetOrdersByNote(note='1111')"));

            var orders = getOrdersByNote.Execute();
            Assert.Equal(2, orders.Count());
        }

        [Fact]
        public void FunctionOfEntitiesTakeEntitiesReturnEntities()
        {
            var orders = this.TestClientContext.Orders.ToList();
            var getCustomersByOrder = this.TestClientContext.Customers.GetCustomersByOrders(orders);
            var customers = getCustomersByOrder.Execute();

            Assert.True(customers.Any());
        }

        [Fact]
        public void FunctionOfEntitiesTakeEntityReturnEntity()
        {
            var order = this.TestClientContext.Orders.First();
            var getCustomersByOrder = this.TestClientContext.Customers.GetCustomerByOrder(order);
            var customer = getCustomersByOrder.GetValue();

            Assert.NotNull(customer);
        }

        [Fact]
        public void FunctionOfEntitiesTakeEntityReferenceReturnEntity()
        {
            var order = this.TestClientContext.Orders.ToList()[1];
            var getCustomersByOrder = this.TestClientContext.Customers.GetCustomerByOrder(order, true);
            Assert.True(getCustomersByOrder.RequestUri.AbsoluteUri.Contains("odata.id"));

            var customer = getCustomersByOrder.GetValue();

            Assert.NotNull(customer);
            Assert.Equal(2, customer.ID);
        }

        [Fact]
        public void FunctionTakeEntityReferenceUseLocalEntity()
        {
            var order = new Order()
            {
                ID = 1,
            };
            this.TestClientContext.AttachTo("Orders", order);
            var getCustomersByOrder = this.TestClientContext.Customers.GetCustomerByOrder(order, true /*useEntityReference*/);
            Assert.True(getCustomersByOrder.RequestUri.AbsoluteUri.Contains("odata.id"));

            var customer = getCustomersByOrder.GetValue();

            Assert.NotNull(customer);
            Assert.Equal(2, customer.ID);
        }

        [Fact]
        public void FunctionOfEntitiesTakeEntityReferencesReturnEntities()
        {
            var orders = this.TestClientContext.Orders.ToList();
            var getCustomersByOrders = this.TestClientContext.Customers.GetCustomersByOrders(orders, true /*useEntityReference*/);
            Assert.True(getCustomersByOrders.RequestUri.AbsoluteUri.Contains("odata.id"));
            var customers = getCustomersByOrders.Execute();

            Assert.True(customers.Count() > 1);
        }

        [Fact]
        public void FunctionOfEntityTakeEntityReturnEntity()
        {
            var customer = this.TestClientContext.Customers.Expand("Orders").Skip(1).First();
            var getCustomerByOrder = customer.VerifyCustomerByOrder(customer.Orders.First());
            customer = getCustomerByOrder.GetValue();

            Assert.NotNull(customer);
        }

        [Fact]
        public void FunctionImportTakeEntitiesReturnEntities()
        {
            var orders = this.TestClientContext.Orders.ToList();
            var getCustomersByOrders = this.TestClientContext.GetCustomersByOrders(orders);
            Assert.False(getCustomersByOrders.RequestUri.AbsoluteUri.Contains("odata.id"));
            var customers = getCustomersByOrders.Execute();
            int count = customers.Count();
            Assert.True(count > 1);

            getCustomersByOrders = this.TestClientContext.GetCustomersByOrders(orders, true);
            Assert.True(getCustomersByOrders.RequestUri.AbsoluteUri.Contains("odata.id"));
            customers = getCustomersByOrders.Execute();

            Assert.True(customers.Count() == count);
        }

        [Fact]
        public void FunctionImportTakeEntityReturnEntity()
        {
            var order = this.TestClientContext.Orders.ToList()[1];
            var getCustomerByOrder = this.TestClientContext.GetCustomerByOrder(order);
            Assert.False(getCustomerByOrder.RequestUri.AbsoluteUri.Contains("odata.id"));
            var customer = getCustomerByOrder.GetValue();
            Assert.NotNull(customer);
            Assert.Equal(2, customer.ID);

            getCustomerByOrder = this.TestClientContext.GetCustomerByOrder(order, true);
            Assert.True(getCustomerByOrder.RequestUri.AbsoluteUri.Contains("odata.id"));
            customer = getCustomerByOrder.GetValue();

            Assert.NotNull(customer);
            Assert.Equal(2, customer.ID);
        }

    }
}

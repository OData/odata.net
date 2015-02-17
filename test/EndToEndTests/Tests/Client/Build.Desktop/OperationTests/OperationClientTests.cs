//---------------------------------------------------------------------
// <copyright file="OperationClientTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.OData.Client;
using Microsoft.Test.OData.Services.TestServices.OperationServiceReference;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Test.OData.Tests.Client.OperationTests
{
    [TestClass]
    public class OperationClientTests : ODataWCFServiceTestsBase<OperationService>
    {
        
        public OperationClientTests()
            : base(Microsoft.Test.OData.Services.TestServices.ServiceDescriptors.OperationServiceDescriptor)
        {

        }

        [TestMethod]
        public void FunctionOfEntitiesTakeComplexsReturnEntities()
        {
            var customerQuery = this.TestClientContext.CreateQuery<Customer>("Customers");
            var addresses = new[]
            {
                new Address()
                {
                    City = "Sydney",
                    PostalCode = "98052",
                    Street = "1 Microsoft Way"
                },
                new Address()
                {
                    City = "Tokyo",
                    PostalCode = "98052",
                    Street = "1 Microsoft Way"
                },
            };

            var functionQuery = customerQuery.CreateFunctionQuery<Customer>("Microsoft.Test.OData.Services.ODataOperationService.GetCustomersForAddresses", true, new UriOperationParameter("addresses", addresses));
            var customers = functionQuery.Execute();
            Assert.AreEqual(2, customers.Count());
        }

        [TestMethod]
        public void FunctionOfEntitiesTakeComplexReturnEntity()
        {
            var customerQuery = this.TestClientContext.CreateQuery<Customer>("Customers");
            Address address = new Address()
            {
                City = "Sydney",
                PostalCode = "98052",
                Street = "1 Microsoft Way"
            };
            var functionQuery = customerQuery.CreateFunctionQuerySingle<Customer>("Microsoft.Test.OData.Services.ODataOperationService.GetCustomerForAddress", true, new UriOperationParameter("address", address));
            var customer = functionQuery.GetValue();
            Assert.IsNotNull(customer);
        }

        [TestMethod]
        public void FunctionOfEntityTakeCollectionReturnEntities()
        {
            var customerQuery = new DataServiceQuerySingle<Customer>(this.TestClientContext, "Customers(3)");

            var functionQuery = customerQuery.CreateFunctionQuery<Order>("Microsoft.Test.OData.Services.ODataOperationService.GetOrdersFromCustomerByNotes", true, new UriOperationParameter("notes", new Collection<string> { "1111", "2222" }));
            var orders = functionQuery.Execute();
            Assert.AreEqual(1, orders.Count());
        }

        [TestMethod]
        public void FunctionOfEntitiesTakeStringReturnEntities()
        {
            var orderQuery = this.TestClientContext.CreateQuery<Order>("Orders");
            var functionQuery = orderQuery.CreateFunctionQuery<Order>("Microsoft.Test.OData.Services.ODataOperationService.GetOrdersByNote", true, new UriOperationParameter("note", "1111"));
            var orders = functionQuery.Execute();
            Assert.AreEqual(2, orders.Count());
        }

        [TestMethod]
        public void FunctionOfEntitiesTakeEntitiesReturnEntities()
        {
            var orders = new[]
            {
                new Order()
                {
                    ID = 1,
                    Notes = new ObservableCollection<string>() {"note1", "note2"},
                },
                new Order()
                {
                    ID = 2,
                },
            };
            this.TestClientContext.AttachTo("Orders", orders[0]);  // Do not need to call this if the order is from service.
            this.TestClientContext.AttachTo("Orders", orders[1]);
            var customerQuery = this.TestClientContext.CreateQuery<Customer>("Customers");
            var functionQuery = customerQuery.CreateFunctionQuery<Customer>("Microsoft.Test.OData.Services.ODataOperationService.GetCustomersByOrders", true, new UriOperationParameter("orders", orders));
            var customers = functionQuery.Execute();
            Assert.AreEqual(1, customers.Count());
        }

        [TestMethod]
        public void FunctionOfEntitiesTakeEntityReferenceReturnEntity()
        {
            var order = new Order()
            {
                ID = 1,
                Notes = new ObservableCollection<string>() { "note1", "note2" },
            };
            this.TestClientContext.AttachTo("Orders", order);
            var customerQuery = this.TestClientContext.CreateQuery<Customer>("Customers");
            var functionQuery = customerQuery.CreateFunctionQuery<Customer>("Microsoft.Test.OData.Services.ODataOperationService.GetCustomerByOrder", true, new UriEntityOperationParameter("order", order, true));
            var customers = functionQuery.Execute();
            Assert.AreEqual(1, customers.Count());
        }

        [TestMethod]
        public void FunctionOfEntitiesTakeEntityReturnEntities()
        {
            var order = new Order()
            {
                ID = 1,
                Notes = new ObservableCollection<string>() { "note1", "note2"},
            };
            var customerQuery = this.TestClientContext.CreateQuery<Customer>("Customers");
            var functionQuery = customerQuery.CreateFunctionQuery<Customer>("Microsoft.Test.OData.Services.ODataOperationService.GetCustomerByOrder", true, new UriOperationParameter("order", order));
            var customers = functionQuery.Execute();
            Assert.AreEqual(1, customers.Count());
        }


        [TestMethod]
        public void FunctionOfEntitiesTakeEntityReferencesReturnEntities()
        {
            var orders = new[]
            {
                new Order()
                {
                    ID = 1,
                    Notes = new ObservableCollection<string>() {"note1", "note2"},
                },
                new Order()
                {
                    ID = 2,
                },
            };
            this.TestClientContext.AttachTo("Orders", orders[0]);  // Do not need to call this if the order is from service.
            this.TestClientContext.AttachTo("Orders", orders[1]);
            var customerQuery = this.TestClientContext.CreateQuery<Customer>("Customers");
            var functionQuery = customerQuery.CreateFunctionQuery<Customer>("Microsoft.Test.OData.Services.ODataOperationService.GetCustomersByOrders", true, new UriEntityOperationParameter("orders", orders, true));
            var customers = functionQuery.Execute();
            Assert.AreEqual(1, customers.Count());
        }
    }
}

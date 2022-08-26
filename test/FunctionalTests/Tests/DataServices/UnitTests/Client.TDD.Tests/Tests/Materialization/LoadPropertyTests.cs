//---------------------------------------------------------------------
// <copyright file="LoadPropertyTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Client;
    using Microsoft.OData.Edm;
    using Xunit;

    public class LoadPropertyTests
    {
        private EdmModel model;
        private ClientEdmModel clientEdmModel;
        private DataServiceContext dataServiceContext;
        private Stack<string> mockResponseStack;

        #region Response Payloads

        private const string CustomerEntityResponsePayload = "{" +
            "\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
            "\"Id\":1," +
            "\"Name\":\"Sue\"," +
            "\"MainCategory\":\"Retail\"," +
            "\"MinorCategories\":[\"Distributor\"]," +
            "\"BillingDays\":[8,15]," +
            "\"BillingAddress\":{\"Street\":\"1 Way\",\"City\":\"Gotham\"}," +
            "\"ShippingAddresses\":[{\"Street\":\"1 Way\",\"City\":\"Gotham\"}]}";

        private const string OrderEntityResponsePayload = "{" +
            "\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$entity\"," +
            "\"Id\":1," +
            "\"Amount\":130}";

        #endregion Response Payloads

        public LoadPropertyTests()
        {
            InitializeEdmModel();

            this.clientEdmModel = new ClientEdmModel(ODataProtocolVersion.V4);

            this.dataServiceContext = new DataServiceContext(new Uri("http://tempuri.org/"), ODataProtocolVersion.V4, this.clientEdmModel);
            this.dataServiceContext.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support;
            this.dataServiceContext.Format.UseJson(this.model);
            this.dataServiceContext.ResolveName = (type) => "NS." + type.Name;
            this.dataServiceContext.ResolveType = (typeName) =>
            {
                return typeof(LoadPropertyTests).GetNestedTypes().SingleOrDefault(
                    d => d.Name.Equals(typeName.Substring(typeName.LastIndexOf('.') + 1)));
            };

            this.mockResponseStack = new Stack<string>();
        }

        [Fact]
        public void LoadPrimitiveProperty()
        {
            this.mockResponseStack.Push("{" +
                "\"@odata.context\":\"http://tempuri.org/$metadata#Customers(1)/Name\"," +
                "\"value\":\"Sue\"}");
            this.mockResponseStack.Push(CustomerEntityResponsePayload);
            this.InterceptRequestAndMockResponse();

            var query = dataServiceContext.CreateQuery<Customer>("Customers");
            var customer = query.Execute().FirstOrDefault();
            Assert.NotNull(customer);

            var queryOperationResponse = this.dataServiceContext.LoadProperty(customer, "Name");
            var result = ParseQueryOperationResponse<string>(queryOperationResponse);

            Assert.Equal("Sue", result);
            Assert.Equal("Sue", customer.Name);
        }

        [Fact]
        public void LoadPrimitiveCollectionProperty()
        {
            this.mockResponseStack.Push("{" +
                "\"@odata.context\":\"http://tempuri.org/$metadata#Collection(Edm.Int32)\"," +
                "\"value\":[7,15,22]}");
            this.mockResponseStack.Push(CustomerEntityResponsePayload);
            this.InterceptRequestAndMockResponse();

            var query = dataServiceContext.CreateQuery<Customer>("Customers");
            var customer = query.Execute().FirstOrDefault();
            Assert.NotNull(customer);

            var queryOperationResponse = this.dataServiceContext.LoadProperty(customer, "BillingDays");
            var result = ParseQueryOperationResponse<List<int>>(queryOperationResponse);

            var expected = new List<int> { 7, 15, 22 };
            Assert.Equal(expected, result);
            Assert.Equal(expected, customer.BillingDays);
        }

        [Fact]
        public void LoadEnumProperty()
        {
            this.mockResponseStack.Push("{" +
                "\"@odata.context\":\"http://tempuri.org/$metadata#Customers(1)/MainCategory\"," +
                "\"value\":\"Retail\"}");
            this.mockResponseStack.Push(CustomerEntityResponsePayload);
            this.InterceptRequestAndMockResponse();

            var query = dataServiceContext.CreateQuery<Customer>("Customers");
            var customer = query.Execute().FirstOrDefault();
            Assert.NotNull(customer);

            var queryOperationResponse = this.dataServiceContext.LoadProperty(customer, "MainCategory");
            var result = ParseQueryOperationResponse<Category>(queryOperationResponse);

            Assert.Equal(Category.Retail, result);
            Assert.Equal(Category.Retail, customer.MainCategory);
        }

        [Fact]
        public void LoadEnumCollectionProperty()
        {
            this.mockResponseStack.Push("{" +
                "\"@odata.context\":\"http://tempuri.org/$metadata#Collection(NS.Category)\"," +
                "\"value\":[\"Wholesale\",\"Distributor\"]}");
            this.mockResponseStack.Push(CustomerEntityResponsePayload);
            this.InterceptRequestAndMockResponse();

            var query = dataServiceContext.CreateQuery<Customer>("Customers");
            var customer = query.Execute().FirstOrDefault();
            Assert.NotNull(customer);

            var queryOperationResponse = this.dataServiceContext.LoadProperty(customer, "MinorCategories");
            var result = ParseQueryOperationResponse<List<Category>>(queryOperationResponse);

            var expected = new List<Category> { Category.Wholesale, Category.Distributor };
            Assert.Equal(expected, result);
            Assert.Equal(expected, customer.MinorCategories);
        }

        [Fact]
        public void LoadComplexProperty()
        {
            this.mockResponseStack.Push("{" +
                "\"@odata.context\":\"http://tempuri.org/$metadata#Customers(1)/BillingAddress\"," +
                "\"Street\":\"1 Way\"," +
                "\"City\":\"Gotham\"}");
            this.mockResponseStack.Push(CustomerEntityResponsePayload);
            this.InterceptRequestAndMockResponse();

            var query = dataServiceContext.CreateQuery<Customer>("Customers");
            var customer = query.Execute().FirstOrDefault();
            Assert.NotNull(customer);

            var queryOperationResponse = this.dataServiceContext.LoadProperty(customer, "BillingAddress");
            var result = ParseQueryOperationResponse<Address>(queryOperationResponse);

            Action<Address> verifyBillingAddressAction = (billingAddress) =>
            {
                Assert.NotNull(billingAddress);
                Assert.Equal("1 Way", billingAddress.Street);
                Assert.Equal("Gotham", billingAddress.City);
            };

            verifyBillingAddressAction(result);
            verifyBillingAddressAction(customer.BillingAddress);
        }

        [Fact]
        public void LoadComplexCollectionProperty()
        {
            this.mockResponseStack.Push("{" +
                "\"@odata.context\":\"http://tempuri.org/$metadata#Customers(1)/ShippingAddresses\"," +
                "\"value\":[{\"Street\":\"1 Way\",\"City\":\"Gotham\"},{\"Street\":\"Other Way\",\"City\":\"Atlantis\"}]}");
            this.mockResponseStack.Push(CustomerEntityResponsePayload);
            this.InterceptRequestAndMockResponse();

            var query = dataServiceContext.CreateQuery<Customer>("Customers");
            var customer = query.Execute().FirstOrDefault();
            Assert.NotNull(customer);

            var queryOperationResponse = this.dataServiceContext.LoadProperty(customer, "ShippingAddresses");
            var result = ParseQueryOperationResponse<List<Address>>(queryOperationResponse);

            Action<List<Address>> verifyShippingAddressesAction = (shippingAddresses) =>
            {
                Assert.NotNull(shippingAddresses);
                Assert.Equal(2, shippingAddresses.Count);
                Assert.Equal("1 Way", shippingAddresses[0].Street);
                Assert.Equal("Gotham", shippingAddresses[0].City);
                Assert.Equal("Other Way", shippingAddresses[1].Street);
                Assert.Equal("Atlantis", shippingAddresses[1].City);
            };

            verifyShippingAddressesAction(result);
            verifyShippingAddressesAction(customer.ShippingAddresses);
        }

        [Fact]
        public void LoadEntityProperty()
        {
            this.mockResponseStack.Push(CustomerEntityResponsePayload);
            this.mockResponseStack.Push(OrderEntityResponsePayload);
            this.InterceptRequestAndMockResponse();

            var query = dataServiceContext.CreateQuery<Order>("Orders");
            var order = query.Execute().FirstOrDefault();
            Assert.NotNull(order);

            var queryOperationResponse = this.dataServiceContext.LoadProperty(order, "Customer");
            var result = ParseQueryOperationResponse<Customer>(queryOperationResponse);

            Action<Customer> verifyCustomerAction = (customer) =>
            {
                Assert.NotNull(customer);
                Assert.Equal(1, customer.Id);
                Assert.Equal("Sue", customer.Name);
                Assert.Equal(Category.Retail, customer.MainCategory);
                Assert.Equal(new List<Category> { Category.Distributor }, customer.MinorCategories);
                Assert.Equal(new List<int> { 8, 15 }, customer.BillingDays);
                Assert.Equal("1 Way", customer.BillingAddress.Street);
                Assert.Equal("Gotham", customer.BillingAddress.City);
                Assert.Single(customer.ShippingAddresses);
                Assert.Equal("1 Way", customer.ShippingAddresses[0].Street);
                Assert.Equal("Gotham", customer.ShippingAddresses[0].City);
            };

            verifyCustomerAction(result);
            verifyCustomerAction(order.Customer);
        }

        [Fact]
        public void LoadEntityCollectionProperty()
        {
            this.mockResponseStack.Push("{" +
                "\"@odata.context\":\"http://tempuri.org/$metadata#Orders\"," +
                "\"value\":[{\"Id\":1,\"Amount\":130},{\"Id\":2,\"Amount\":170}]}");
            this.mockResponseStack.Push(CustomerEntityResponsePayload);
            this.InterceptRequestAndMockResponse();

            var query = dataServiceContext.CreateQuery<Customer>("Customers");
            var customer = query.Execute().FirstOrDefault();
            Assert.NotNull(customer);

            var result = new List<Order>();
            var queryOperationResponse = this.dataServiceContext.LoadProperty(customer, "Orders");
            var enumerator = queryOperationResponse.GetEnumerator();
            while (enumerator.MoveNext())
            {
                result.Add(Assert.IsType<Order>(enumerator.Current));
            }
            
            Action<List<Order>> verifyOrdersAction = (orders) =>
            {
                Assert.Equal(2, orders.Count);
                Assert.Equal(1, orders[0].Id);
                Assert.Equal(130M, orders[0].Amount);
                Assert.Equal(2, orders[1].Id);
                Assert.Equal(170M, orders[1].Amount);
            };

            verifyOrdersAction(result);
            verifyOrdersAction(customer.Orders);
        }

        #region Helper Methods

        private static T ParseQueryOperationResponse<T>(QueryOperationResponse queryOperationResponse)
        {
            var queryOperationResponseEnumerator = queryOperationResponse.GetEnumerator();
            queryOperationResponseEnumerator.MoveNext();
            return (T)queryOperationResponseEnumerator.Current;
        }

        private void InitializeEdmModel()
        {
            this.model = new EdmModel();

            var categoryEnumType = new EdmEnumType("NS", "Category");
            categoryEnumType.AddMember(new EdmEnumMember(categoryEnumType, "Retail", new EdmEnumMemberValue(1)));
            categoryEnumType.AddMember(new EdmEnumMember(categoryEnumType, "Wholesale", new EdmEnumMemberValue(2)));
            categoryEnumType.AddMember(new EdmEnumMember(categoryEnumType, "Distributor", new EdmEnumMemberValue(3)));
            model.AddElement(categoryEnumType);

            var addressComplexType = new EdmComplexType("NS", "Address");
            addressComplexType.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String);
            addressComplexType.AddStructuralProperty("City", EdmPrimitiveTypeKind.String);
            model.AddElement(addressComplexType);

            var orderEntityType = new EdmEntityType("NS", "Order");
            orderEntityType.AddKeys(orderEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, false));
            orderEntityType.AddStructuralProperty("Amount", EdmPrimitiveTypeKind.Decimal);
            model.AddElement(orderEntityType);

            var customerEntityType = new EdmEntityType("NS", "Customer");
            customerEntityType.AddKeys(customerEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, false));
            customerEntityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            customerEntityType.AddStructuralProperty("MainCategory", new EdmEnumTypeReference(categoryEnumType, false));
            customerEntityType.AddStructuralProperty(
                "MinorCategories",
                new EdmCollectionTypeReference(new EdmCollectionType(new EdmEnumTypeReference(categoryEnumType, false))));
            customerEntityType.AddStructuralProperty(
                "BillingDays",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false))));
            customerEntityType.AddStructuralProperty(
                "BillingAddress",
                new EdmComplexTypeReference(addressComplexType, false));
            customerEntityType.AddStructuralProperty(
                "ShippingAddresses",
                new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(addressComplexType, false))));
            model.AddElement(customerEntityType);

            var container = new EdmEntityContainer("NS", "Container");
            var customerEntitySet = container.AddEntitySet("Customers", customerEntityType);
            var orderEntitySet = container.AddEntitySet("Orders", orderEntityType);
            model.AddElement(container);

            customerEntitySet.AddNavigationTarget(
                customerEntityType.AddUnidirectionalNavigation(
                    new EdmNavigationPropertyInfo
                    {
                        Name = "Orders",
                        Target = orderEntityType,
                        TargetMultiplicity = EdmMultiplicity.Many
                    }),
                orderEntitySet);

            orderEntitySet.AddNavigationTarget(
                orderEntityType.AddUnidirectionalNavigation(
                    new EdmNavigationPropertyInfo
                    {
                        Name = "Customer",
                        Target = customerEntityType,
                        TargetMultiplicity = EdmMultiplicity.ZeroOrOne
                    }),
                customerEntitySet);
        }

        private void InterceptRequestAndMockResponse()
        {
            Assert.True(this.mockResponseStack.Count != 0);

            this.dataServiceContext.Configurations.RequestPipeline.OnMessageCreating = (args) =>
            {
                var contentTypeHeader = "application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8";
                var odataVersionHeader = "4.0";

                return new Microsoft.OData.Client.TDDUnitTests.Tests.CustomizedHttpWebRequestMessage(args,
                    mockResponseStack.Pop(),
                    new Dictionary<string, string>
                    {
                        {"Content-Type", contentTypeHeader},
                        {"OData-Version", odataVersionHeader},
                    });
            };
        }

        #endregion Helper Methods

        #region Helper Classes

        public enum Category
        {
            Retail = 1,
            Wholesale = 2,
            Distributor = 3
        }

        public class Address
        {
            public string Street { get; set; }
            public string City { get; set; }
        }

        [Key("Id")]
        public class Order
        {
            public int Id { get; set; }
            public decimal Amount { get; set; }
            public Customer Customer { get; set; }
        }

        [Key("Id")]
        public class Customer
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public Category MainCategory { get; set; }
            public List<Category> MinorCategories { get; set; }
            public List<int> BillingDays { get; set; }
            public Address BillingAddress { get; set; }
            public List<Address> ShippingAddresses { get; set; }
            public List<Order> Orders { get; set; }
        }

        #endregion Helper Classes
    }
}

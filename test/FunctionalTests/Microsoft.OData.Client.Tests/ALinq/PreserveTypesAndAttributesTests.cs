//---------------------------------------------------------------------
// <copyright file="PreserveTypesAndAttributesTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Client.Tests.ALinq
{
    /// <summary>
    /// Tests to check original resource type is used when retrieving attributes like OriginalNameAttribute
    /// even if the IQueryable<T> object is casted to another type.
    /// </summary>
    public class PreserveTypesAndAttributesTests
    {
        private const string CustomersEntitySetName = "Customers";

        private const string BaseUriStr = "https://mock.odata.service";

        private readonly DataServiceContext _ctx;

        public PreserveTypesAndAttributesTests()
        {
            _ctx = new DataServiceContext(new Uri(BaseUriStr));
            _ctx.Format.UseJson(BuildEdmModel());
            _ctx.MergeOption = MergeOption.NoTracking;
        }

        [Fact]
        public void OneLevelOfInheritaceClassQuery_UsesRightPropertyNames_WhenCastedToInterface()
        {
            var customersQuery = _ctx.CreateQuery<OneLevelCustomer1>(CustomersEntitySetName);

            IQueryable<ICustomer> queryWithInterface = customersQuery;
            queryWithInterface = queryWithInterface.Where(c => c.CorporationName != "").OrderBy(c => c.Id).Take(2);

            IQueryable<OneLevelCustomer1> queryWithClass = customersQuery;
            queryWithClass = queryWithClass.Where(c => c.CorporationName != "").OrderBy(c => c.Id).Take(2);

            Assert.Equal(GetRequestUri(queryWithClass), GetRequestUri(queryWithInterface));

            string resp = "[{\"CustomerID\":\"ALFoKI\",\"CompanyName\":\"Alfreds Futterkiste\",\"ContactName\":\"Maria Anders\",\"City\":\"Berlin\"},{\"CustomerID\":\"ANATR\",\"CompanyName\":\"Ana Trujillo Emparedados y helados\",\"ContactName\":\"Ana Trujillo\",\"City\":\"M\\u00e9xico D.F.\"}]";
            InterceptRequestAndMockResponseValue(CustomersEntitySetName, resp);

            var result1 = queryWithInterface.ToList();
            var result2 = queryWithClass.ToList();

            Assert.Equal(result1.Count, result2.Count);
            for (var i = 0; i < result1.Count; i++)
            {
                Assert.Equal(result1[i].City, result2[i].City);
                Assert.Equal(result1[i].CorporationName, result2[i].CorporationName);
                Assert.Equal(result1[i].Name, result2[i].Name);
                Assert.Equal(result1[i].Id, result2[i].Id);
            }
        }

        [Fact]
        public void OneLevelOfInheritaceClassQuery_UsesRightPropertyNames_WhenCastedToBaseClass()
        {
            var customersQuery = _ctx.CreateQuery<OneLevelCustomer2>(CustomersEntitySetName);

            IQueryable<BaseCustomer> queryWithBaseClass = customersQuery;
            var query1select = queryWithBaseClass.Where(c => c.Id != "").OrderBy(c => c.Id).Select(c => new { c.Id }).Take(2);

            IQueryable<OneLevelCustomer2> queryWithClass = customersQuery;
            var query2select = queryWithClass.Where(c => c.Id != "").OrderBy(c => c.Id).Select(c => new { c.Id }).Take(2);

            Assert.Equal(GetRequestUri(query2select), GetRequestUri(query1select));

            string resp = "[{\"CustomerID\":\"ALFKI\"},{\"CustomerID\":\"ANATR\"}]";
            InterceptRequestAndMockResponseValue(CustomersEntitySetName + "(CustomerID)", resp);

            var result1 = query1select.ToList();
            var result2 = query2select.ToList();

            Assert.Equal(result1.Count, result2.Count);
            for (var i = 0; i < result1.Count; i++)
            {
                Assert.Equal(result1[i].Id, result2[i].Id);
            }
        }

        [Fact]
        public void MoreThanOneLevelOfInheritaceClassQuery_UsesRightPropertyNames_WhenCastedToInterfaceOrAbstractClass()
        {
            var customersQuery = _ctx.CreateQuery<TwoLevelCustomer1>(CustomersEntitySetName);

            IQueryable<ICustomer> queryWithInterface = customersQuery;
            queryWithInterface = queryWithInterface.Where(c => c.Name.Contains("a"));
            queryWithInterface = queryWithInterface.Where(c => c.Id != "").OrderBy(c => c.Id).Take(2);

            IQueryable<ACustomer> queryWithAbstractClass = customersQuery;
            queryWithAbstractClass = queryWithAbstractClass.Where(c => c.Name.Contains("a"));
            queryWithAbstractClass = queryWithAbstractClass.Where(c => c.Id != "").OrderBy(c => c.Id).Take(2);

            IQueryable<TwoLevelCustomer1> queryWithClass = customersQuery;
            queryWithClass = queryWithClass.Where(c => c.Name.Contains("a"));
            queryWithClass = queryWithClass.Where(c => c.Id != "").OrderBy(c => c.Id).Take(2);

            Assert.Equal(GetRequestUri(queryWithClass), GetRequestUri(queryWithInterface));
            Assert.Equal(GetRequestUri(queryWithClass), GetRequestUri(queryWithAbstractClass));

            string resp = "[{\"CustomerID\":\"ALFKY\",\"CompanyName\":\"Alfreds Futterkiste\",\"ContactName\":\"Maria Anders\",\"City\":\"Berlin\"},{\"CustomerID\":\"ANATR\",\"CompanyName\":\"Ana Trujillo Emparedados y helados\",\"ContactName\":\"Ana Trujillo\",\"City\":\"M\\u00e9xico D.F.\"}]";
            InterceptRequestAndMockResponseValue(CustomersEntitySetName, resp);

            var result1 = queryWithInterface.ToList();
            var result2 = queryWithAbstractClass.ToList();
            var result3 = queryWithClass.ToList();

            Assert.Equal(result1.Count, result2.Count);
            for (var i = 0; i < result1.Count; i++)
            {
                Assert.Equal(result1[i].Id, result2[i].Id);
                Assert.Equal(result1[i].CorporationName, result2[i].CorporationName);
                Assert.Equal(result1[i].City, result2[i].City);
                Assert.Equal(result1[i].Name, result2[i].Name);
            }
            Assert.Equal(result3.Count, result2.Count);
            for (var i = 0; i < result1.Count; i++)
            {
                Assert.Equal(result3[i].Id, result2[i].Id);
                Assert.Equal(result3[i].CorporationName, result2[i].CorporationName);
                Assert.Equal(result3[i].City, result2[i].City);
                Assert.Equal(result3[i].Name, result2[i].Name);
            }
        }

        [Fact]
        public void MoreThanOneLevelOfInheritaceClassQuery_UsesRightPropertyNames_WhenCastedToInterfaceWithExpandibleProperties()
        {
            var customersQuery = _ctx.CreateQuery<TwoLevelCustomer2>(CustomersEntitySetName);

            IQueryable<ICustomer> queryWithInterface = customersQuery.Expand(c => c.OrdersList);
            queryWithInterface = queryWithInterface.Where(c => c.Name.Contains("a"));
            queryWithInterface = queryWithInterface.Where(c => c.Id != "").OrderBy(c => c.Id).Take(2);

            IQueryable<ICustomer2> queryWithInterfaceExpand = customersQuery.Expand(c => c.OrdersList);
            queryWithInterfaceExpand = queryWithInterfaceExpand.Where(c => c.Name.Contains("a"));
            queryWithInterfaceExpand = queryWithInterfaceExpand.Where(c => c.Id != "").OrderBy(c => c.Id).Take(2);

            IQueryable<TwoLevelCustomer2> queryWithClass = customersQuery.Expand(c => c.OrdersList);
            queryWithClass = queryWithClass.Where(c => c.Name.Contains("a"));
            queryWithClass = queryWithClass.Where(c => c.Id != "").OrderBy(c => c.Id).Take(2);

            Assert.Equal(GetRequestUri(queryWithClass), GetRequestUri(queryWithInterface));
            Assert.Equal(GetRequestUri(queryWithClass), GetRequestUri(queryWithInterfaceExpand));

            string resp = "[{\"CustomerID\":\"ALFKI\",\"CompanyName\":\"Alfreds Futterkiste\",\"ContactName\":\"Maria Anders\",\"ContactTitle\":\"Sales Representative\",\"Address\":\"Obere Str. 57\",\"City\":\"Berlin\",\"PostalCode\":\"12209\",\"Country\":\"Germany\",\"Phone\":\"030-0074321\",\"Orders\":[{\"OrderID\":10643,\"CustomerID\":\"ALFKI\",\"OrderDate\":\"1997-08-25T00:00:00Z\",\"ShipVia\":1,\"Freight\":29.4600,\"ShipName\":\"Alfreds Futterkiste\",\"ShipAddress\":\"Obere Str. 57\",\"ShipCity\":\"Berlin\",\"ShipPostalCode\":\"12209\",\"ShipCountry\":\"Germany\"}]},{\"CustomerID\":\"ANATR\",\"CompanyName\":\"Ana Trujillo Emparedados y helados\",\"ContactName\":\"Ana Trujillo\",\"ContactTitle\":\"Owner\",\"Address\":\"Avda. de la Constituci\\u00f3n 2222\",\"City\":\"M\\u00e9xico D.F.\",\"PostalCode\":\"05021\",\"Country\":\"Mexico\",\"Phone\":\"(5) 555-4729\",\"Orders\":[{\"OrderID\":10308,\"CustomerID\":\"ANATR\",\"OrderDate\":\"1996-09-18T00:00:00Z\",\"ShipVia\":3,\"Freight\":1.6100,\"ShipName\":\"Ana Trujillo Emparedados y helados\",\"ShipAddress\":\"Avda. de la Constituci\\u00f3n 2222\",\"ShipCity\":\"M\\u00e9xico D.F.\",\"ShipPostalCode\":\"05021\",\"ShipCountry\":\"Mexico\"},{\"OrderID\":10625,\"CustomerID\":\"ANATR\",\"OrderDate\":\"1997-08-08T00:00:00Z\",\"ShipVia\":1,\"Freight\":43.9000,\"ShipName\":\"Ana Trujillo Emparedados y helados\",\"ShipAddress\":\"Avda. de la Constituci\\u00f3n 2222\",\"ShipCity\":\"M\\u00e9xico D.F.\",\"ShipPostalCode\":\"05021\",\"ShipCountry\":\"Mexico\"}]}]";
            InterceptRequestAndMockResponseValue(CustomersEntitySetName, resp);

            var result1 = queryWithInterface.ToList();
            var result2 = queryWithInterfaceExpand.ToList();
            var result3 = queryWithClass.ToList();

            Assert.Equal(result1.Count, result2.Count);
            for (var i = 0; i < result1.Count; i++)
            {
                Assert.Equal(result1[i].Id, result2[i].Id);
                Assert.Equal(result1[i].CorporationName, result2[i].CorporationName);
                Assert.Equal(result1[i].City, result2[i].City);
                Assert.Equal(result1[i].Name, result2[i].Name);
            }
            Assert.Equal(result3.Count, result2.Count);
            for (var i = 0; i < result1.Count; i++)
            {
                Assert.Equal(result3[i].Id, result2[i].Id);
                Assert.Equal(result3[i].CorporationName, result2[i].CorporationName);
                Assert.Equal(result3[i].City, result2[i].City);
                Assert.Equal(result3[i].Name, result2[i].Name);
            }
        }

        private static string GetRequestUri(IQueryable query)
        {
            var dsq = query as DataServiceQuery;
            return dsq.RequestUri.ToString();
        }

        private void InterceptRequestAndMockResponseValue(string entitySetName, string mockResponseValue)
        {
            string mockResponse = "{\"@odata.context\":\"" + BaseUriStr + "/$metadata#" + entitySetName + "\",\"value\":" + mockResponseValue + "}";
            _ctx.ResolveName = (type) => $"NS.{type.Name}";
            _ctx.Configurations.RequestPipeline.OnMessageCreating = (args) =>
            {
                var contentTypeHeader = "application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8";
                var odataVersionHeader = "4.0";

                return new TestHttpWebRequestMessage(args,
                    new Dictionary<string, string>
                    {
                        {"Content-Type", contentTypeHeader},
                        {"OData-Version", odataVersionHeader},
                    },
                    () => new MemoryStream(Encoding.UTF8.GetBytes(mockResponse)));
            };
        }

        private static EdmModel BuildEdmModel()
        {
            var model = new EdmModel();

            // Create the Customer entity type
            var customerType = new EdmEntityType("NS", "Customer");
            var customerId = customerType.AddStructuralProperty("CustomerID", EdmPrimitiveTypeKind.String, false);
            customerType.AddKeys(customerId);
            customerType.AddStructuralProperty("CompanyName", EdmPrimitiveTypeKind.String, false);
            customerType.AddStructuralProperty("ContactName", EdmPrimitiveTypeKind.String, true);
            customerType.AddStructuralProperty("ContactTitle", EdmPrimitiveTypeKind.String, true);
            customerType.AddStructuralProperty("Address", EdmPrimitiveTypeKind.String, true);
            customerType.AddStructuralProperty("City", EdmPrimitiveTypeKind.String, true);
            model.AddElement(customerType);

            // Create the Order entity type
            var orderType = new EdmEntityType("NS", "Order");
            var orderId = orderType.AddStructuralProperty("OrderID", EdmPrimitiveTypeKind.Int32, false);
            orderType.AddKeys(orderId);
            orderType.AddStructuralProperty("CustomerID", EdmPrimitiveTypeKind.String, true);
            orderType.AddStructuralProperty("OrderDate", EdmPrimitiveTypeKind.DateTimeOffset, true);
            orderType.AddStructuralProperty("ShipVia", EdmPrimitiveTypeKind.Int32, true);
            orderType.AddStructuralProperty("Freight", EdmPrimitiveTypeKind.Decimal, true);
            orderType.AddStructuralProperty("ShipAddress", EdmPrimitiveTypeKind.String, true);
            model.AddElement(orderType);

            // Create the Navigation Properties
            var ordersNavProperty = customerType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "Orders",
                Target = orderType,
                TargetMultiplicity = EdmMultiplicity.Many
            });

            var customerNavProperty = orderType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
            {
                Name = "Customer",
                Target = customerType,
                TargetMultiplicity = EdmMultiplicity.ZeroOrOne,
                DependentProperties = new[] { orderType.FindProperty("CustomerID") as IEdmStructuralProperty },
                PrincipalProperties = new[] { customerId }
            });

            // Create the EntityContainer
            var container = new EdmEntityContainer("NS", "Container");
            model.AddElement(container);

            // Create Entity Sets
            var customersEntitySet = container.AddEntitySet(CustomersEntitySetName, customerType);
            var ordersEntitySet = container.AddEntitySet("Orders", orderType);

            // Bind Navigation Properties
            customersEntitySet.AddNavigationTarget(ordersNavProperty, ordersEntitySet);
            ordersEntitySet.AddNavigationTarget(customerNavProperty, customersEntitySet);

            return model;
        }


        // One level of inheritance classes

        public interface ICustomer
        {
            string Id { get; set; }
            string City { get; set; }
            string CorporationName { get; set; }
            string Name { get; set; }
        }

        [Key("CustomerID")]
        public class OneLevelCustomer1 : ICustomer
        {
            [OriginalName("CustomerID")]
            public string Id { get; set; }

            public string City { get; set; }

            [OriginalName("CompanyName")]
            public string CorporationName { get; set; }

            [OriginalName("ContactName")]
            public string Name { get; set; }
        }

        [Key("CustomerID")]
        public class BaseCustomer
        {
            [OriginalName("CustomerID")]
            public string Id { get; set; }
        }

        public class OneLevelCustomer2 : BaseCustomer
        {
            public string City { get; set; }

            [OriginalName("CompanyName")]
            public string CorporationName { get; set; }

            [OriginalName("ContactName")]
            public string Name { get; set; }
        }


        // More than one level of inheritance classes

        public abstract class ACustomer : ICustomer
        {
            public virtual string Id { get; set; }
            public virtual string City { get; set; }
            public virtual string CorporationName { get; set; }
            public virtual string Name { get; set; }
        }

        [Key("CustomerID")]
        public class TwoLevelCustomer1 : ACustomer
        {
            [OriginalName("CustomerID")]
            public override string Id { get; set; }

            public override string City { get; set; }

            [OriginalName("CompanyName")]
            public override string CorporationName { get; set; }

            [OriginalName("ContactName")]
            public override string Name { get; set; }
        }

        public interface ICustomer2 : ICustomer
        {
            List<Order> OrdersList { get; set; }
        }

        [Key("CustomerID")]
        public class TwoLevelCustomer2 : ICustomer2
        {
            [OriginalName("CustomerID")]
            public string Id { get; set; }

            public string City { get; set; }

            [OriginalName("CompanyName")]
            public string CorporationName { get; set; }

            [OriginalName("ContactName")]
            public string Name { get; set; }

            [OriginalName("Orders")]
            public List<Order> OrdersList { get; set; }
        }

        [Key("OrderID")]
        public class Order
        {
            public int OrderID { get; set; }
            public decimal Freight { get; set; }
        }
    }
}

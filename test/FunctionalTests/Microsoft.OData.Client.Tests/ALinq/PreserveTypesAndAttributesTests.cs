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
        private readonly TestContext ctx;

        public PreserveTypesAndAttributesTests()
        {
            this.ctx = new TestContext();
        }

        private static void AssertEqualRequestUri(IQueryable query1, IQueryable query2)
        {
            var dsq1 = query1 as DataServiceQuery;
            var dsq2 = query2 as DataServiceQuery;
            Assert.Equal(dsq1.RequestUri, dsq2.RequestUri);
        }

        [Fact]
        public void OneLevelCase1()
        {
            IQueryable<ICustomer> query1 = ctx.OneLevelCustomers1;
            query1 = query1.Where(c => c.CorporationName != "").OrderBy(c => c.Id).Take(2);

            IQueryable<OneLevelCustomer1> query2 = ctx.OneLevelCustomers1;
            query2 = query2.Where(c => c.CorporationName != "").OrderBy(c => c.Id).Take(2);

            AssertEqualRequestUri(query1, query2);

            string resp = "[{\"CustomerID\":\"ALFoKI\",\"CompanyName\":\"Alfreds Futterkiste\",\"ContactName\":\"Maria Anders\",\"City\":\"Berlin\",\"Region\":null},{\"CustomerID\":\"ANATR\",\"CompanyName\":\"Ana Trujillo Emparedados y helados\",\"ContactName\":\"Ana Trujillo\",\"City\":\"M\\u00e9xico D.F.\",\"Region\":null}]";
            ctx.InterceptRequestAndMockResponseValue("Customers", resp);

            var result1 = query1.ToList();
            var result2 = query2.ToList();

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
        public void OneLevelCase2()
        {
            IQueryable<BaseCustomer> query1 = ctx.OneLevelCustomers2;
            var query1s = query1.Where(c => c.Id != "").OrderBy(c => c.Id).Select(c => new { c.Id }).Take(2);

            IQueryable<OneLevelCustomer2> query2 = ctx.OneLevelCustomers2;
            var query2s = query2.Where(c => c.Id != "").OrderBy(c => c.Id).Select(c => new { c.Id }).Take(2);

            AssertEqualRequestUri(query1s, query2s);
            string resp = "[{\"CustomerID\":\"ALFKI\"},{\"CustomerID\":\"ANATR\"}]";
            ctx.InterceptRequestAndMockResponseValue("Customers(CustomerID)", resp);

            var result1 = query1s.ToList();
            var result2 = query2s.ToList();

            Assert.Equal(result1.Count, result2.Count);
            for (var i = 0; i < result1.Count; i++)
            {
                Assert.Equal(result1[i].Id, result2[i].Id);
            }
        }

        [Fact]
        public void MoreThanOneLevelLevelCase1()
        {
            IQueryable<ICustomer> query1 = ctx.TwoLevelCustomers1;
            query1 = query1.Where(c => c.Name.Contains("a"));
            query1 = query1.Where(c => c.Id != "").OrderBy(c => c.Id).Take(2);

            IQueryable<ACustomer> query2 = ctx.TwoLevelCustomers1;
            query2 = query2.Where(c => c.Name.Contains("a"));
            query2 = query2.Where(c => c.Id != "").OrderBy(c => c.Id).Take(2);

            IQueryable<TwoLevelCustomer1> query3 = ctx.TwoLevelCustomers1;
            query3 = query3.Where(c => c.Name.Contains("a"));
            query3 = query3.Where(c => c.Id != "").OrderBy(c => c.Id).Take(2);

            AssertEqualRequestUri(query1, query2);
            AssertEqualRequestUri(query2, query3);

            string resp = "[{\"CustomerID\":\"ALFKY\",\"CompanyName\":\"Alfreds Futterkiste\",\"ContactName\":\"Maria Anders\",\"City\":\"Berlin\",\"Region\":null},{\"CustomerID\":\"ANATR\",\"CompanyName\":\"Ana Trujillo Emparedados y helados\",\"ContactName\":\"Ana Trujillo\",\"City\":\"M\\u00e9xico D.F.\",\"Region\":null}]";
            ctx.InterceptRequestAndMockResponseValue("Customers", resp);

            var result1 = query1.ToList();
            var result2 = query2.ToList();
            var result3 = query3.ToList();

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
        public void MoreThanOneLevelLevelCase2()
        {
            IQueryable<ICustomer> query1 = ctx.TwoLevelCustomers2.Expand(c => c.OrdersList);
            query1 = query1.Where(c => c.Name.Contains("a"));
            query1 = query1.Where(c => c.Id != "").OrderBy(c => c.Id).Take(2);

            IQueryable<ICustomer2> query2 = ctx.TwoLevelCustomers2.Expand(c => c.OrdersList);
            query2 = query2.Where(c => c.Name.Contains("a"));
            query2 = query2.Where(c => c.Id != "").OrderBy(c => c.Id).Take(2);

            IQueryable<TwoLevelCustomer2> query3 = ctx.TwoLevelCustomers2.Expand(c => c.OrdersList);
            query3 = query3.Where(c => c.Name.Contains("a"));
            query3 = query3.Where(c => c.Id != "").OrderBy(c => c.Id).Take(2);

            AssertEqualRequestUri(query1, query2);
            AssertEqualRequestUri(query2, query3);

            string resp = "[{\"CustomerID\":\"ALFKI\",\"CompanyName\":\"Alfreds Futterkiste\",\"ContactName\":\"Maria Anders\",\"ContactTitle\":\"Sales Representative\",\"Address\":\"Obere Str. 57\",\"City\":\"Berlin\",\"Region\":null,\"PostalCode\":\"12209\",\"Country\":\"Germany\",\"Phone\":\"030-0074321\",\"Fax\":\"030-0076545\",\"Orders\":[{\"OrderID\":10643,\"CustomerID\":\"ALFKI\",\"EmployeeID\":6,\"OrderDate\":\"1997-08-25T00:00:00Z\",\"RequiredDate\":\"1997-09-22T00:00:00Z\",\"ShippedDate\":\"1997-09-02T00:00:00Z\",\"ShipVia\":1,\"Freight\":29.4600,\"ShipName\":\"Alfreds Futterkiste\",\"ShipAddress\":\"Obere Str. 57\",\"ShipCity\":\"Berlin\",\"ShipRegion\":null,\"ShipPostalCode\":\"12209\",\"ShipCountry\":\"Germany\"},{\"OrderID\":10692,\"CustomerID\":\"ALFKI\",\"EmployeeID\":4,\"OrderDate\":\"1997-10-03T00:00:00Z\",\"RequiredDate\":\"1997-10-31T00:00:00Z\",\"ShippedDate\":\"1997-10-13T00:00:00Z\",\"ShipVia\":2,\"Freight\":61.0200,\"ShipName\":\"Alfred's Futterkiste\",\"ShipAddress\":\"Obere Str. 57\",\"ShipCity\":\"Berlin\",\"ShipRegion\":null,\"ShipPostalCode\":\"12209\",\"ShipCountry\":\"Germany\"},{\"OrderID\":10702,\"CustomerID\":\"ALFKI\",\"EmployeeID\":4,\"OrderDate\":\"1997-10-13T00:00:00Z\",\"RequiredDate\":\"1997-11-24T00:00:00Z\",\"ShippedDate\":\"1997-10-21T00:00:00Z\",\"ShipVia\":1,\"Freight\":23.9400,\"ShipName\":\"Alfred's Futterkiste\",\"ShipAddress\":\"Obere Str. 57\",\"ShipCity\":\"Berlin\",\"ShipRegion\":null,\"ShipPostalCode\":\"12209\",\"ShipCountry\":\"Germany\"},{\"OrderID\":10835,\"CustomerID\":\"ALFKI\",\"EmployeeID\":1,\"OrderDate\":\"1998-01-15T00:00:00Z\",\"RequiredDate\":\"1998-02-12T00:00:00Z\",\"ShippedDate\":\"1998-01-21T00:00:00Z\",\"ShipVia\":3,\"Freight\":69.5300,\"ShipName\":\"Alfred's Futterkiste\",\"ShipAddress\":\"Obere Str. 57\",\"ShipCity\":\"Berlin\",\"ShipRegion\":null,\"ShipPostalCode\":\"12209\",\"ShipCountry\":\"Germany\"},{\"OrderID\":10952,\"CustomerID\":\"ALFKI\",\"EmployeeID\":1,\"OrderDate\":\"1998-03-16T00:00:00Z\",\"RequiredDate\":\"1998-04-27T00:00:00Z\",\"ShippedDate\":\"1998-03-24T00:00:00Z\",\"ShipVia\":1,\"Freight\":40.4200,\"ShipName\":\"Alfred's Futterkiste\",\"ShipAddress\":\"Obere Str. 57\",\"ShipCity\":\"Berlin\",\"ShipRegion\":null,\"ShipPostalCode\":\"12209\",\"ShipCountry\":\"Germany\"},{\"OrderID\":11011,\"CustomerID\":\"ALFKI\",\"EmployeeID\":3,\"OrderDate\":\"1998-04-09T00:00:00Z\",\"RequiredDate\":\"1998-05-07T00:00:00Z\",\"ShippedDate\":\"1998-04-13T00:00:00Z\",\"ShipVia\":1,\"Freight\":1.2100,\"ShipName\":\"Alfred's Futterkiste\",\"ShipAddress\":\"Obere Str. 57\",\"ShipCity\":\"Berlin\",\"ShipRegion\":null,\"ShipPostalCode\":\"12209\",\"ShipCountry\":\"Germany\"}]},{\"CustomerID\":\"ANATR\",\"CompanyName\":\"Ana Trujillo Emparedados y helados\",\"ContactName\":\"Ana Trujillo\",\"ContactTitle\":\"Owner\",\"Address\":\"Avda. de la Constituci\\u00f3n 2222\",\"City\":\"M\\u00e9xico D.F.\",\"Region\":null,\"PostalCode\":\"05021\",\"Country\":\"Mexico\",\"Phone\":\"(5) 555-4729\",\"Fax\":\"(5) 555-3745\",\"Orders\":[{\"OrderID\":10308,\"CustomerID\":\"ANATR\",\"EmployeeID\":7,\"OrderDate\":\"1996-09-18T00:00:00Z\",\"RequiredDate\":\"1996-10-16T00:00:00Z\",\"ShippedDate\":\"1996-09-24T00:00:00Z\",\"ShipVia\":3,\"Freight\":1.6100,\"ShipName\":\"Ana Trujillo Emparedados y helados\",\"ShipAddress\":\"Avda. de la Constituci\\u00f3n 2222\",\"ShipCity\":\"M\\u00e9xico D.F.\",\"ShipRegion\":null,\"ShipPostalCode\":\"05021\",\"ShipCountry\":\"Mexico\"},{\"OrderID\":10625,\"CustomerID\":\"ANATR\",\"EmployeeID\":3,\"OrderDate\":\"1997-08-08T00:00:00Z\",\"RequiredDate\":\"1997-09-05T00:00:00Z\",\"ShippedDate\":\"1997-08-14T00:00:00Z\",\"ShipVia\":1,\"Freight\":43.9000,\"ShipName\":\"Ana Trujillo Emparedados y helados\",\"ShipAddress\":\"Avda. de la Constituci\\u00f3n 2222\",\"ShipCity\":\"M\\u00e9xico D.F.\",\"ShipRegion\":null,\"ShipPostalCode\":\"05021\",\"ShipCountry\":\"Mexico\"},{\"OrderID\":10759,\"CustomerID\":\"ANATR\",\"EmployeeID\":3,\"OrderDate\":\"1997-11-28T00:00:00Z\",\"RequiredDate\":\"1997-12-26T00:00:00Z\",\"ShippedDate\":\"1997-12-12T00:00:00Z\",\"ShipVia\":3,\"Freight\":11.9900,\"ShipName\":\"Ana Trujillo Emparedados y helados\",\"ShipAddress\":\"Avda. de la Constituci\\u00f3n 2222\",\"ShipCity\":\"M\\u00e9xico D.F.\",\"ShipRegion\":null,\"ShipPostalCode\":\"05021\",\"ShipCountry\":\"Mexico\"},{\"OrderID\":10926,\"CustomerID\":\"ANATR\",\"EmployeeID\":4,\"OrderDate\":\"1998-03-04T00:00:00Z\",\"RequiredDate\":\"1998-04-01T00:00:00Z\",\"ShippedDate\":\"1998-03-11T00:00:00Z\",\"ShipVia\":3,\"Freight\":39.9200,\"ShipName\":\"Ana Trujillo Emparedados y helados\",\"ShipAddress\":\"Avda. de la Constituci\\u00f3n 2222\",\"ShipCity\":\"M\\u00e9xico D.F.\",\"ShipRegion\":null,\"ShipPostalCode\":\"05021\",\"ShipCountry\":\"Mexico\"}]}]";
            ctx.InterceptRequestAndMockResponseValue("Customers", resp);

            var result1 = query1.ToList();
            var result2 = query2.ToList();
            var result3 = query3.ToList();

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


    public class TestContext
    {
        public DataServiceQuery<OneLevelCustomer1> OneLevelCustomers1;
        public DataServiceQuery<TwoLevelCustomer1> TwoLevelCustomers1;
        public DataServiceQuery<OneLevelCustomer2> OneLevelCustomers2;
        public DataServiceQuery<TwoLevelCustomer2> TwoLevelCustomers2;

        // We have to create multiple DataServiceContext because the entitySetName is the same for every DataServiceQuery
        private readonly DataServiceContext _ctx1;
        private readonly DataServiceContext _ctx2;
        private readonly DataServiceContext _ctx3;
        private readonly DataServiceContext _ctx4;

        private readonly string _uriStr;

        public TestContext()
        {
            _uriStr = "https://mock.odata.service";
            Uri uri = new Uri(_uriStr);

            _ctx1 = new DataServiceContext(uri);
            _ctx2 = new DataServiceContext(uri);
            _ctx3 = new DataServiceContext(uri);
            _ctx4 = new DataServiceContext(uri);
            OneLevelCustomers1 = _ctx1.CreateQuery<OneLevelCustomer1>("Customers");
            TwoLevelCustomers1 = _ctx2.CreateQuery<TwoLevelCustomer1>("Customers");
            OneLevelCustomers2 = _ctx3.CreateQuery<OneLevelCustomer2>("Customers");
            TwoLevelCustomers2 = _ctx4.CreateQuery<TwoLevelCustomer2>("Customers");
        }

        private void InterceptRequestAndMockResponse(DataServiceContext ctx, string mockResponse)
        {
            ctx.ResolveName = (type) => $"NS.{type.Name}";
            ctx.Configurations.RequestPipeline.OnMessageCreating = (args) =>
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

        public void InterceptRequestAndMockResponseValue(string entitySetName, string mockResponseValue)
        {
            string mockResponse = "{\"@odata.context\":\"" + _uriStr + "/$metadata#" + entitySetName + "\",\"value\":" + mockResponseValue + "}";

            EdmModel model = BuildEdmModel();
            var contexts = new List<DataServiceContext>() { _ctx1, _ctx2, _ctx3, _ctx4 };
            foreach (var ctx in contexts)
            {
                ctx.MergeOption = MergeOption.NoTracking;
                ctx.Format.UseJson(model);
                InterceptRequestAndMockResponse(ctx, mockResponse);
            }
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
            var customersEntitySet = container.AddEntitySet("Customers", customerType);
            var ordersEntitySet = container.AddEntitySet("Orders", orderType);

            // Bind Navigation Properties
            customersEntitySet.AddNavigationTarget(ordersNavProperty, ordersEntitySet);
            ordersEntitySet.AddNavigationTarget(customerNavProperty, customersEntitySet);

            return model;
        }
    }
}

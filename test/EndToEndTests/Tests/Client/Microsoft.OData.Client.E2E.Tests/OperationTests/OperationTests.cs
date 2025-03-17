//-----------------------------------------------------------------------------
// <copyright file="OperationTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.ObjectModel;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common.Client.Operations;
using Microsoft.OData.E2E.TestCommon.Common.Server.Operations;
using Microsoft.OData.E2E.TestCommon.Common.Server.OperationTests;
using Xunit;
using Address = Microsoft.OData.E2E.TestCommon.Common.Client.Operations.Address;
using Customer = Microsoft.OData.E2E.TestCommon.Common.Client.Operations.Customer;
using InfoFromCustomer = Microsoft.OData.E2E.TestCommon.Common.Client.Operations.InfoFromCustomer;
using Order = Microsoft.OData.E2E.TestCommon.Common.Client.Operations.Order;
using OrderDetail = Microsoft.OData.E2E.TestCommon.Common.Client.Operations.OrderDetail;
using CustomerLevel = Microsoft.OData.E2E.TestCommon.Common.Client.Operations.CustomerLevel;

namespace Microsoft.OData.Client.E2E.Tests.OperationTests;

public class OperationTests : EndToEndTestBase<OperationTests.TestsStartup>
{
    private readonly Uri _baseUri;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(OperationTestsController), typeof(MetadataController));

            services.AddControllers().AddOData(opt =>
                opt.EnableQueryFeatures().AddRouteComponents("odata", OperationsEdmModel.GetEdmModel()));
        }
    }

    public OperationTests(TestWebApplicationFactory<TestsStartup> fixture) : base(fixture)
    {
        if (Client.BaseAddress == null)
        {
            throw new ArgumentNullException(nameof(Client.BaseAddress), "Base address cannot be null");
        }

        _baseUri = new Uri(Client.BaseAddress, "odata/");
    }

    [Fact]
    public async Task FunctionBoundToEntityCollection_WithCollectionOfComplexTypeAsParameter_ReturnsEntityCollection()
    {
        // Arrange
        var context = this.ContextWrapper();
        var customerQuery = context.CreateQuery<Customer>("Customers");
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

        var functionQuery = customerQuery.CreateFunctionQuery<Customer>("GetCustomersForAddresses", 
            true, 
            new UriOperationParameter("addresses", addresses));

        // Act
        var customers = (await functionQuery.ExecuteAsync()).ToList();

        // Assert
        Assert.Equal(2, customers.Count);
        Assert.Equal("1 Microsoft Way", customers[0].Address.Street);
        Assert.True(customers.Where(c => c.Address.City == "Tokyo").Any());
        Assert.True(customers.Where(c => c.Address.City == "Sydney").Any());
    }

    [Fact]
    public async Task FunctionBoundToEntityCollection_WithCollectionOfComplexTypeAsParameter_ReturnsAnEntity()
    {
        // Arrange
        var context = this.ContextWrapper();
        var customerQuery = context.CreateQuery<Customer>("Customers");
        Address address = new Address()
        {
            City = "Sydney",
            PostalCode = "98052",
            Street = "1 Microsoft Way"
        };
        var functionQuery = customerQuery.CreateFunctionQuerySingle<Customer>("GetCustomerForAddress", 
            true, 
            new UriOperationParameter("address", address));

        // Act
        var customer = await functionQuery.GetValueAsync();

        // Assert
        Assert.NotNull(customer);
        Assert.Equal("Sydney", customer.Address.City);
        Assert.Equal("98052", customer.Address.PostalCode);
    }

    [Fact]
    public async Task FunctionBoundToAnEntity_WithCollectionOfPrimitiveTypeAsParameter_ReturnsEntityCollections()
    {
        // Arrange
        var context = this.ContextWrapper();
        var customerQuery = new DataServiceQuerySingle<Customer>(context, "Customers(3)");

        var functionQuery = customerQuery.CreateFunctionQuery<Order>("GetOrdersFromCustomerByNotes", 
            true, new UriOperationParameter("notes", 
            new Collection<string> { "1111", "2222" }));

        // Act
        var orders = (await functionQuery.ExecuteAsync()).ToList();

        // Assert
        Assert.Single(orders);
        Assert.Equal(2, orders[0].Notes.Count);
        Assert.Equal("1111", orders[0].Notes[0]);
    }

    [Fact]
    public async Task FunctionBoundToEntityCollection_WithPrimitiveTypeAsParameter_ReturnsEntityCollection()
    {
        // Arrange
        var context = this.ContextWrapper();
        var orderQuery = context.CreateQuery<Order>("Orders");
        var functionQuery = orderQuery.CreateFunctionQuery<Order>("GetOrdersByNote", true, new UriOperationParameter("note", "1111"));

        // Act
        var orders = (await functionQuery.ExecuteAsync()).ToList();

        // Assert
        Assert.Equal(2, orders.Count());
        Assert.Equal(2, orders[0].Notes.Count);
        Assert.Equal(2, orders[1].Notes.Count);
        Assert.Equal("1111", orders[0].Notes[0]);
    }

    [Fact]
    public async Task FunctionBoundToEntityCollection_WithEntityCollectionAsParameter_ReturnsEntityCollection()
    {
        // Arrange
        var context = this.ContextWrapper();

        var orders = new[]
        {
            new Order()
            {
                OrderID = 1,
                Notes = new ObservableCollection<string>() {"note1", "note2"},
                OrderDetails = new ObservableCollection<OrderDetail>{ new OrderDetail{Quantity = 1, UnitPrice = 1.0f}},
                InfoFromCustomer = new InfoFromCustomer { CustomerMessage = "XXL"}
            },
            new Order()
            {
                OrderID = 2,
                OrderDetails = new ObservableCollection<OrderDetail>{ new OrderDetail{Quantity = 2, UnitPrice = 2.0f}},
                InfoFromCustomer = new InfoFromCustomer { CustomerMessage = "XXL"}
            },
        };

        context.AttachTo("Orders", orders[0]);
        context.AttachTo("Orders", orders[1]);
        var customerQuery = context.CreateQuery<Customer>("Customers");
        var functionQuery = customerQuery.CreateFunctionQuery<Customer>("GetCustomersByOrders", true, new UriOperationParameter("orders", orders));

        // Act
        var customers = (await functionQuery.ExecuteAsync()).ToList();

        // Assert
        Assert.Single(customers);
        Assert.Equal(2, customers[0].CustomerID);
        Assert.Equal("Jill", customers[0].FirstName);
        Assert.Equal("Jones", customers[0].LastName);
    }

    [Fact]
    public async Task FunctionBoundToEntityCollection_WithEntityReferenceAsParameter_ReturnsEntity()
    {
        // Arrange
        var context = this.ContextWrapper();

        var order = new Order()
        {
            OrderID = 1,
            Notes = new ObservableCollection<string>() { "note1", "note2" },
        };
        context.AttachTo("Orders", order);
        var customerQuery = context.CreateQuery<Customer>("Customers");
        var functionQuery = customerQuery.CreateFunctionQuery<Customer>("GetCustomerByOrder", true, new UriEntityOperationParameter("order", order, true));

        // Act
        var customers = (await functionQuery.ExecuteAsync()).ToList();

        // Assert
        Assert.Single(customers);
        Assert.Equal(2, customers[0].CustomerID);
        Assert.Equal("Jill", customers[0].FirstName);
        Assert.Equal(CustomerLevel.Common, customers[0].Level);
    }

    [Fact]
    public async Task FunctionBoundToEntityCollection_WithEntityAsParameter_ReturnsEntityCollection()
    {
        // Arrange
        var context = this.ContextWrapper();
        var order = new Order()
        {
            OrderID = 1,
            Notes = new ObservableCollection<string>() { "note1", "note2" },
            OrderDetails = new ObservableCollection<OrderDetail> { new OrderDetail { Quantity = 1, UnitPrice = 1.0f } },
            InfoFromCustomer = new InfoFromCustomer { CustomerMessage = "XXL" }
        };
        var customerQuery = context.CreateQuery<Customer>("Customers");
        var functionQuery = customerQuery.CreateFunctionQuery<Customer>("GetCustomerByOrder", true, new UriOperationParameter("order", order));

        // Act
        var customers = (await functionQuery.ExecuteAsync()).ToList();

        // Assert
        Assert.Single(customers);
        Assert.Equal(2, customers[0].CustomerID);
        Assert.Equal("Jill", customers[0].FirstName);
        Assert.Equal("Jones", customers[0].LastName);
    }

    [Fact]
    public async Task FunctionBoundToEntityCollection_WithEntityReferencesAsParameter_ReturnsEntityCollection()
    {
        // Arrange
        var context = this.ContextWrapper();

        var orders = new[]
        {
            new Order()
            {
                OrderID = 1,
                Notes = new ObservableCollection<string>() {"note1", "note2"},
            },
            new Order()
            {
                OrderID = 2,
            },
        };

        context.AttachTo("Orders", orders[0]);
        context.AttachTo("Orders", orders[1]);
        var customerQuery = context.CreateQuery<Customer>("Customers");
        var functionQuery = customerQuery.CreateFunctionQuery<Customer>("GetCustomersByOrders", true, new UriEntityOperationParameter("orders", orders, true));

        // Act
        var customers = (await functionQuery.ExecuteAsync()).ToList();

        // Assert
        Assert.Single(customers);
        Assert.Equal(2, customers[0].CustomerID);
        Assert.Equal("Jill", customers[0].FirstName);
        Assert.Equal("Jones", customers[0].LastName);
    }

    [Fact]
    public async Task FunctionBoundToEntityCollection_WithExpandQueryOption_ReturnEntityExpandNavigation()
    {
        // Arrange
        var context = this.ContextWrapper();

        // Act
        var query = context.Orders.GetOrderByNote(new string[] { "1111", "parent" }).Expand(o => o.Customer);
        var order = await query.GetValueAsync();

        // Assert
        Assert.NotNull(order.Customer);
        Assert.Equal(2, order.Notes.Count);
        Assert.Equal("1111", order.Notes[0]);
        Assert.Equal("Bob", order.Customer.FirstName);
        Assert.Equal("Tokyo", order.Customer.Address.City);
    }

    [Fact]
    public async Task FunctionBoundToEntityCollection_ReturnEntityWithSelectedProperties()
    {
        // Arrange
        var context = this.ContextWrapper();

        // Act
        var order = await (context.Orders.GetOrderByNote(new string[] { "1111", "parent" }).Select(o => new Order() { OrderID = o.OrderID, Notes = o.Notes })).GetValueAsync();

        // Assert
        Assert.Equal(2, order.Notes.Count);
        Assert.Equal(2, order.OrderID);
        Assert.Equal<DateTimeOffset>(default(DateTimeOffset), order.OrderDate);
    }

    [Fact]
    public void FunctionBoundToEntityCollection_ReturnEntitiesExpandNavigationProperty()
    {
        // Arrange
        var context = this.ContextWrapper();

        // Act
        var orders = context.Orders.GetOrdersByNote("1111").Expand(o => o.Customer).ToList();

        // Assert
        Assert.Equal(2, orders.Count);
        Assert.Null(orders[0].Customer);
        Assert.NotNull(orders[1].Customer);
        Assert.Equal("Bob", orders[1].Customer.FirstName);
        Assert.Equal("Tokyo", orders[1].Customer.Address.City);
    }

    [Fact]
    public void FunctionBoundToEntityCollection_WorksWithFilterQueryOption()
    {
        // Arrange
        var context = this.ContextWrapper();

        // Act
        var orders = context.Orders.GetOrdersByNote("1111").Where(o => o.OrderID < 1).ToList();

        // Assert
        Assert.Single(orders);
        Assert.Equal(0, orders[0].OrderID);
        Assert.Equal(2, orders[0].Notes.Count);
        Assert.Equal("1111", orders[0].Notes[0]);
        Assert.Equal("child", orders[0].Notes[1]);
    }

    [Fact]
    public async Task FunctionBoundToEntityCollection_WithEntityReference_ReturnEntity()
    {
        // Arrange
        var context = this.ContextWrapper();

        var order = context.Orders.ToList()[1];
        var getCustomersByOrder = context.Customers.GetCustomerByOrder(order, true);
        Assert.Contains("odata.id", getCustomersByOrder.RequestUri.AbsoluteUri);

        // Act
        var customer = await getCustomersByOrder.GetValueAsync();

        // Assert
        Assert.NotNull(customer);
        Assert.Equal(2, customer.CustomerID);
    }

    [Fact]
    public async Task FunctionBoundToEntityCollection_WithEntityReference_UseLocalEntity()
    {
        // Arrange
        var context = this.ContextWrapper();

        var order = new Order()
        {
            OrderID = 1,
        };

        context.AttachTo("Orders", order);
        var getCustomersByOrder = context.Customers.GetCustomerByOrder(order, true /*useEntityReference*/);
        Assert.Contains("odata.id", getCustomersByOrder.RequestUri.AbsoluteUri);

        // Act
        var customer = await getCustomersByOrder.GetValueAsync();

        // Assert
        Assert.NotNull(customer);
        Assert.Equal(2, customer.CustomerID);
    }

    [Fact]
    public async Task FunctionBoundToEntityCollection_WithEntityReference_ReturnEntities()
    {
        // Arrange
        var context = this.ContextWrapper();

        var orders = context.Orders.ToList();
        var getCustomersByOrders = context.Customers.GetCustomersByOrders(orders, true /*useEntityReference*/);
        Assert.Contains("odata.id", getCustomersByOrders.RequestUri.AbsoluteUri);

        // Act
        var customers = await getCustomersByOrders.ExecuteAsync();

        Assert.True(customers.Count() > 1);
    }

    [Fact]
    public async Task FunctionBoundToAnEntity_WithEntityReference_ReturnEntity()
    {
        // Arrange
        var context = this.ContextWrapper();

        var customer = context.Customers.Expand("Orders").Skip(1).First();
        var getCustomerByOrder = customer.VerifyCustomerByOrder(customer.Orders.First());

        // Act
        customer = await getCustomerByOrder.GetValueAsync();

        // Assert
        Assert.NotNull(customer);
    }

    #region Private

    private Container ContextWrapper()
    {
        var context = new Container(_baseUri)
        {
            HttpClientFactory = HttpClientFactory
        };

        ResetDefaultDataSource(context);

        return context;
    }

    private void ResetDefaultDataSource(Container context)
    {
        var actionUri = new Uri(_baseUri + "operationtests/Default.ResetDefaultDataSource", UriKind.Absolute);
        context.Execute(actionUri, "POST");
    }

    #endregion
}

//---------------------------------------------------------------------
// <copyright file="UpdateTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.TestCommon;
using Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd;
using Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd.Default;
using Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd;
using Microsoft.OData.Client.E2E.Tests.KeyAsSegmentTests.Server;
using Microsoft.OData.Edm;
using Xunit;
using Customer = Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd.Customer;
using Employee = Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd.Employee;
using Order = Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd.Order;
using Person = Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd.Person;
using SpecialEmployee = Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd.SpecialEmployee;

namespace Microsoft.OData.Client.E2E.Tests.KeyAsSegmentTests.Tests;

public class UpdateTests : EndToEndTestBase<UpdateTests.TestsStartup>
{
    private readonly Uri _baseUri;
    private readonly Container _context;
    private readonly IEdmModel _model;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(UpdateTestsController), typeof(MetadataController));

            services.AddControllers().AddOData(opt =>
                opt.EnableQueryFeatures().AddRouteComponents("odata", CommonEndToEndEdmModel.GetEdmModel(), batchHandler: new DefaultODataBatchHandler()));
        }
    }

    public UpdateTests(TestWebApplicationFactory<TestsStartup> fixture) : base(fixture)
    {
        if (Client.BaseAddress == null)
        {
            throw new ArgumentNullException(nameof(Client.BaseAddress), "Base address cannot be null");
        }

        _baseUri = new Uri(Client.BaseAddress, "odata/");

        _context = new Container(_baseUri)
        {
            HttpClientFactory = HttpClientFactory
        };

        _model = CommonEndToEndEdmModel.GetEdmModel();
        ResetDefaultDataSource();
    }

    [Fact]
    public void InsertSave()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;
        var newPerson = new Person { PersonId = 9999, Name = "Some Person" };
        var personQuery = _context.People.Where(p => p.PersonId == newPerson.PersonId);

        _context.IgnoreResourceNotFoundException = true;

        // Act & Assert
        var retrievedPerson = personQuery.SingleOrDefault();
        Assert.Null(retrievedPerson);

        _context.AddObject("People", newPerson);
        _context.SaveChanges();

        retrievedPerson = personQuery.SingleOrDefault();
        Assert.NotNull(retrievedPerson);
        Assert.True(newPerson == retrievedPerson, "New entity and retrieved entity should reference same object");
    }

    [Fact]
    public void AttachUpdateObjectSave()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;
        var specialEmployee = new SpecialEmployee { PersonId = -10 };
        _context.AttachTo("People", specialEmployee);

        specialEmployee.Bonus = Int32.MaxValue;
        _context.UpdateObject(specialEmployee);
        _context.SaveChanges();

        var retrievedPerson = _context.People.ByKey(specialEmployee.PersonId).GetValue();
        var retrievedBonus = (retrievedPerson as SpecialEmployee)?.Bonus;
        Assert.Equal(Int32.MaxValue, retrievedBonus);
    }

    [Fact]
    public void AddObjectSave()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;
        var specialEmployee = new SpecialEmployee { PersonId = 1234 };

        // Act & Assert
        _context.AddObject("People", specialEmployee);
        var savedResponse = _context.SaveChanges();
        Assert.Equal(201, (savedResponse.Single() as ChangeOperationResponse)?.StatusCode);
        Assert.NotNull(_context.People.ByKey(1234).GetValue());

        _context.DeleteObject(specialEmployee);
        var deletedResponse = _context.SaveChanges();
        Assert.Equal(204, (deletedResponse.Single() as ChangeOperationResponse)?.StatusCode);
        var exception = Record.Exception(() => _context.People.ByKey(1234).GetValue());
        Assert.NotNull(exception);
        Assert.Equal("NotFound", exception.InnerException?.Message);
    }

    [Fact]
    public void AddObjectAddRelatedObjectSave()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;
        _context.MergeOption = MergeOption.PreserveChanges;

        var customer = new Customer { CustomerId = 1234 };
        var order = new Order { OrderId = 12345 };

        // Act
        _context.AddObject("Customers", customer);
        _context.AddRelatedObject(customer, "Orders", order);
        var savedChangesResponse = _context.SaveChanges();

        // Assert
        Assert.True(savedChangesResponse.All(s => s.StatusCode == 201));
        var customerSaved = _context.Customers.ByKey(1234).Expand(c => c.Orders).GetValue();
        Assert.NotNull(customerSaved);
        Assert.Equal(1234, customerSaved.CustomerId);
        Assert.Contains(customerSaved.Orders, o => o.OrderId == 12345);
    }

    [Fact]
    public void AddDeleteLinkSave()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;
        _context.MergeOption = MergeOption.PreserveChanges;

        var customer = _context.Customers.Expand(c => c.Orders).First();
        var order = _context.Orders.ToArray().Except(customer.Orders).First();

        // Act & Assert
        _context.AddLink(customer, "Orders", order);
        _context.SaveChanges();

        // Order exists in customer's orders
        var customerWithOrders = _context.Customers.ByKey(customer.CustomerId).Expand(c => c.Orders).GetValue();
        Assert.Contains(customerWithOrders.Orders, o => o.OrderId == order.OrderId);

        _context.DeleteLink(customer, "Orders", order);
        _context.SaveChanges();

        // Order does not exist in customer's orders
        var customerWithoutOrders = _context.Customers.ByKey(customer.CustomerId).Expand(c => c.Orders).GetValue();
        Assert.DoesNotContain(customerWithoutOrders.Orders, o => o.OrderId == order.OrderId);
    }

    [Fact]
    public void AddDeleteLinkSaveBatch()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;
        _context.MergeOption = MergeOption.PreserveChanges;

        var customer = _context.Customers.Expand(c => c.Orders).First();
        var order1 = _context.Orders.ToArray().Except(customer.Orders).First();
        var order2 = _context.Orders.OrderByDescending(o => o.OrderId).ToArray().Except(customer.Orders).First();

        // Act & Assert
        _context.AddLink(customer, "Orders", order1);
        _context.AddLink(customer, "Orders", order2);

        _context.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);

        // Order exists in customer's orders
        var customerWithoutOrders = _context.Customers.ByKey(customer.CustomerId).Expand(c => c.Orders).GetValue();
        Assert.Contains(customerWithoutOrders.Orders, o => o.OrderId == order1.OrderId || o.OrderId == order2.OrderId);

        _context.DeleteLink(customer, "Orders", order1);
        _context.DeleteLink(customer, "Orders", order2);
        _context.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);

        // Order does not exist in customer's orders
        var customerWithOrders = _context.Customers.ByKey(customer.CustomerId).Expand(c => c.Orders).GetValue();
        Assert.DoesNotContain(customerWithOrders.Orders, o => o.OrderId == order1.OrderId || o.OrderId == order2.OrderId);
    }

    [Fact]
    public void SetLinkSave()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;
        _context.MergeOption = MergeOption.PreserveChanges;

        // Act & Assert
        var employee = _context.People.OfType<Employee>().First();

        _context.SetLink(employee, "Manager", employee);
        _context.SaveChanges();

        var employeeWithManager = (_context.People.OfType<Employee>().Where(s => s.PersonId == employee.PersonId) as DataServiceQuery<Employee>)?.Expand(e => e.Manager).Single();
        Assert.NotNull(employeeWithManager?.Manager);
        Assert.Equal(employee.PersonId, employeeWithManager.Manager.PersonId);
    }

    [Fact]
    public void AddRelatedObjectSave()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;
        _context.MergeOption = MergeOption.PreserveChanges;

        // Act
        var customer = _context.Customers.First();
        var order = new Order { OrderId = 1234 };

        _context.AddRelatedObject(customer, "Orders", order);
        var savedChangeResponse = _context.SaveChanges();

        // Assert
        var response = savedChangeResponse.Single() as ChangeOperationResponse;
        Assert.Equal(201, response?.StatusCode);

        var entity = response?.Descriptor as EntityDescriptor;
        Assert.EndsWith("/Orders(1234)", entity?.EditLink.OriginalString);
    }

    #region Private methods

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "keyassegmentupdatetests/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    #endregion
}

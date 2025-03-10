//-----------------------------------------------------------------------------
// <copyright file="DefaultChangeTrackingTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common.Client.Default.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.PropertyTrackingTests;
using Xunit;
using Account = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Account;
using Customer = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Customer;
using HomeAddress = Microsoft.OData.E2E.TestCommon.Common.Client.Default.HomeAddress;
using Order = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Order;
using Person = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Person;

namespace Microsoft.OData.Client.E2E.Tests.PropertyTrackingTests;

/// <summary>
/// End-to-end tests for default change tracking.
/// </summary>
public class DefaultChangeTrackingTests : EndToEndTestBase<DefaultChangeTrackingTests.TestsStartup>
{
    private readonly Uri _baseUri;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(DefaultChangeTrackingTestsController), typeof(MetadataController));

            services.AddControllers().AddOData(opt =>
                opt.EnableQueryFeatures().AddRouteComponents("odata", DefaultEdmModel.GetEdmModel(), batchHandler: new DefaultODataBatchHandler()));
        }
    }

    public DefaultChangeTrackingTests(TestWebApplicationFactory<TestsStartup> fixture) : base(fixture)
    {
        if (Client.BaseAddress == null)
        {
            throw new ArgumentNullException(nameof(Client.BaseAddress), "Base address cannot be null");
        }

        _baseUri = new Uri(Client.BaseAddress, "odata/");
    }

    [Fact]
    public async Task UpdatePrimitivePropertyUnderEntityUsingPatch()
    {
        // Arrange
        var _context = this.ContextWrapper();
        int expectedChangedPropertyCount = 0;

        _context.Configurations.RequestPipeline.OnEntryEnding(
            (arg) =>
            {
                if (arg.Entry.TypeName.EndsWith("Order"))
                {
                    Assert.Equal(expectedChangedPropertyCount, arg.Entry.Properties.Count());
                }
            });

        var orders = new DataServiceCollection<Order>(_context.Orders.Expand("OrderDetails"));

        // Act
        _context.BuildingRequest += (sender, args) =>
        {
            if (args.RequestUri.AbsoluteUri.EndsWith("odata/Orders(8)"))
            {
                Assert.Equal("PATCH", args.Method);
            }
        };

        var dateTimeOffset = DateTimeOffset.Now;
        orders[1].OrderDate = dateTimeOffset;
        expectedChangedPropertyCount = 1;
        var responses = await _context.SaveChangesAsync();

        // Assert
        Assert.Equal(204, responses.First().StatusCode);
        Assert.EndsWith("odata/Orders(8)", ((responses.ElementAt(0) as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.EditLink.AbsoluteUri);
        Assert.Equal(dateTimeOffset, _context.Orders.Where((it) => it.OrderID == orders[1].OrderID).Single().OrderDate);
    }

    [Fact]
    public async Task UpdateCollectionPropertyUnderEntityUsingPatch()
    {
        // Arrange
        var _context = this.ContextWrapper();
        int expectedChangedPropertyCount = 0;

        _context.Configurations.RequestPipeline.OnEntryEnding(
            (arg) =>
            {
                if (arg.Entry.TypeName.EndsWith("Order"))
                {
                    Assert.Equal(expectedChangedPropertyCount, arg.Entry.Properties.Count());
                }
            });

        var orders = new DataServiceCollection<Order>(_context.Orders.Expand("OrderDetails"));

        // Act
        _context.BuildingRequest += (sender, args) =>
        {
            if (args.RequestUri.AbsoluteUri.EndsWith("odata/Orders(8)"))
            {
                Assert.Equal("PATCH", args.Method);
            }
        };

        orders[1].OrderShelfLifes.Add(TimeSpan.FromHours(1.2));
        expectedChangedPropertyCount = 1;
        var responses = await _context.SaveChangesAsync();

        // Assert
        Assert.Equal(204, responses.First().StatusCode);
        Assert.EndsWith("odata/Orders(8)", ((responses.ElementAt(0) as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.EditLink.AbsoluteUri);
        Assert.Equal(2, _context.Orders.Where((it) => it.OrderID == orders[1].OrderID).Single().OrderShelfLifes.Count);
    }

    [Fact]
    public async Task UpdatePrimitivePropertyUnderInheritedEntityUsingPatch()
    {
        // Arrange
        var _context = this.ContextWrapper();
        int expectedChangedPropertyCount = 0;

        _context.Configurations.RequestPipeline.OnEntryEnding(
            (arg) =>
            {
                if (arg.Entry.TypeName.EndsWith("Person"))
                {
                    Assert.Equal(expectedChangedPropertyCount, arg.Entry.Properties.Count());
                }
            });

        var people = new DataServiceCollection<Person>(_context.People);

        // Act
        _context.BuildingRequest += (sender, args) =>
        {
            if (args.RequestUri.AbsoluteUri.EndsWith("odata/People(1)/Microsoft.OData.E2E.TestCommon.Common.Server.Default.Customer"))
            {
                Assert.Equal("PATCH", args.Method);
            }
        };

        ((Customer)people[0]).City = "Redmond";
        expectedChangedPropertyCount = 1;
        var responses = await _context.SaveChangesAsync();

        // Assert
        Assert.Equal(204, responses.First().StatusCode);
        Assert.EndsWith("odata/People(1)/Microsoft.OData.E2E.TestCommon.Common.Server.Default.Customer", ((responses.ElementAt(0) as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.EditLink.AbsoluteUri);
        Assert.Equal("Redmond", (_context.People.Where((it) => it.PersonID == people[0].PersonID).Single() as Customer)?.City);
    }

    [Fact]
    public async Task UpdatePropertyUnderComplexTypeUsingPatch()
    {
        // Arrange
        var _context = this.ContextWrapper();

        var people = new DataServiceCollection<Person>(_context.People);

        // Act
        _context.BuildingRequest += (sender, args) =>
        {
            if (args.RequestUri.AbsoluteUri.EndsWith("odata/People(1)/Microsoft.OData.E2E.TestCommon.Common.Server.Default.Customer"))
            {
                Assert.Equal("PATCH", args.Method);
            }
        };

        people[0].HomeAddress.City = "Redmond";
        var responses = await _context.SaveChangesAsync();

        // Assert
        Assert.Equal(204, responses.First().StatusCode);
        Assert.EndsWith("odata/People(1)/Microsoft.OData.E2E.TestCommon.Common.Server.Default.Customer", ((responses.ElementAt(0) as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.EditLink.AbsoluteUri);
        Assert.Equal("Redmond", _context.People.Where((it) => it.PersonID == people[0].PersonID).Single().HomeAddress.City);
        Assert.Equal("98052", _context.People.Where((it) => it.PersonID == people[0].PersonID).Single().HomeAddress.PostalCode);
    }

    [Fact]
    public async Task UpdatePropertyUnderInheritedComplexTypeUsingPatch()
    {
        // Arrange
        var _context = this.ContextWrapper();

        var people = new DataServiceCollection<Person>(_context.People);

        // Act
        _context.BuildingRequest += (sender, args) =>
        {
            if (args.RequestUri.AbsoluteUri.EndsWith("odata/People(1)/Microsoft.OData.E2E.TestCommon.Common.Server.Default.Customer"))
            {
                Assert.Equal("PATCH", args.Method);
            }
        };

        ((HomeAddress)people[0].HomeAddress).FamilyName = "Microsoft";
        var responses = await _context.SaveChangesAsync();

        // Assert
        Assert.Equal(204, responses.First().StatusCode);
        Assert.EndsWith("odata/People(1)/Microsoft.OData.E2E.TestCommon.Common.Server.Default.Customer", ((responses.ElementAt(0) as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.EditLink.AbsoluteUri);
        Assert.Equal("Microsoft", (_context.People.Where((it) => it.PersonID == people[0].PersonID).Single().HomeAddress as HomeAddress)?.FamilyName);
    }

    [Fact]
    public async Task UpdateCollectionNavigationPropertyUsingPatch()
    {
        // Arrange
        var _context = this.ContextWrapper();
        int expectedPropertyCount = 0;

        _context.Configurations.RequestPipeline.OnEntryEnding(
            (arg) =>
            {
                if (arg.Entry.TypeName.EndsWith("OrderDetail"))
                {
                    Assert.Equal(expectedPropertyCount, arg.Entry.Properties.Count());
                }
            });

        var orders = new DataServiceCollection<Order>(_context.Orders.Expand("OrderDetails"));

        // Act
        _context.BuildingRequest += (sender, args) =>
        {
            if (args.RequestUri.AbsoluteUri.EndsWith("odata/OrderDetails(OrderID=7,ProductID=6)"))
            {
                Assert.Equal("PATCH", args.Method);
            }
        };

        orders[0].OrderDetails.First().Quantity = 1;
        expectedPropertyCount = 1;
        var responses = await _context.SaveChangesAsync();

        // Assert
        Assert.Equal(204, responses.First().StatusCode);
        Assert.EndsWith("odata/OrderDetails(OrderID=7,ProductID=6)", ((responses.ElementAt(0) as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.EditLink.AbsoluteUri);
        Assert.Equal(1, _context.OrderDetails.Where((it) => it.ProductID == orders[0].OrderDetails.First().ProductID && it.OrderID == orders[0].OrderDetails.First().OrderID).Single().Quantity);
    }

    [Fact]
    public async Task UpdateSingleValueNavigationPropertyUsingPatch()
    {
        // Arrange
        var _context = this.ContextWrapper();
        int expectedPropertyCount = 0;

        _context.Configurations.RequestPipeline.OnEntryEnding(
            (arg) =>
            {
                if (arg.Entry.TypeName.EndsWith("GiftCard"))
                {
                    Assert.Equal(expectedPropertyCount, arg.Entry.Properties.Count());
                }
            });

        var accounts = new DataServiceCollection<Account>(_context.Accounts.Expand("MyGiftCard"));

        // Act
        _context.BuildingRequest += (sender, args) =>
        {
            if (args.RequestUri.AbsoluteUri.EndsWith("odata/Accounts(101)/MyGiftCard"))
            {
                Assert.Equal("PATCH", args.Method);
            }
        };

        accounts[0].MyGiftCard.ExperationDate = DateTimeOffset.Now;
        expectedPropertyCount = 1;
        var responses = await _context.SaveChangesAsync();

        // Assert
        Assert.Equal(204, responses.First().StatusCode);
        Assert.EndsWith("odata/Accounts(101)/MyGiftCard", ((responses.ElementAt(0) as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.EditLink.AbsoluteUri);
        Assert.Equal(accounts[0].MyGiftCard.ExperationDate, _context.Accounts.Expand("MyGiftCard").Where((it) => it.AccountID == accounts[0].AccountID).Single().MyGiftCard.ExperationDate);
    }

    [Fact]
    public async Task UpdatePropertyInSingletonUsingPatch()
    {
        // Arrange
        var _context = this.ContextWrapper();
        int expectedPropertyCount = 0;

        _context.Configurations.RequestPipeline.OnEntryEnding(
            (arg) =>
            {
                if (arg.Entry.TypeName.EndsWith("Person"))
                {
                    Assert.Equal(expectedPropertyCount, arg.Entry.Properties.Count());
                }
            });

        var boss = new DataServiceCollection<Person>(_context.Boss);

        // Act
        _context.BuildingRequest += (sender, args) =>
        {
            if (args.RequestUri.AbsoluteUri.EndsWith("odata/Boss/Microsoft.OData.E2E.TestCommon.Common.Server.Default.Customer"))
            {
                Assert.Equal("PATCH", args.Method);
            }
        };

        boss.Single().FirstName = "Bill";
        expectedPropertyCount = 1;
        var responses = await _context.SaveChangesAsync();

        // Assert
        Assert.Equal(204, responses.First().StatusCode);
        Assert.EndsWith("odata/Boss/Microsoft.OData.E2E.TestCommon.Common.Server.Default.Customer", ((responses.ElementAt(0) as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.EditLink.AbsoluteUri);
        Assert.Equal("Bill", _context.Boss.GetValue().FirstName);
    }

    // This test verifies that updating an object without any changes results in a PATCH request
    // that includes all properties of the entity. It ensures that the request pipeline correctly
    // handles such scenarios and includes the expected number of properties in the request.
    [Fact]
    public async Task UpdateObjectWithoutChangesUsingPatch()
    {
        // Arrange
        var _context = this.ContextWrapper();

        var people = new DataServiceCollection<Person>(_context.People);

        // Act & Assert

        // Update object by updating the object without any changes
        _context.UpdateObject(people[0]);

        _context.Configurations.RequestPipeline.OnEntryEnding(
            (arg) =>
            {
                if (arg.Entry.TypeName.EndsWith("Person"))
                {
                    Assert.Equal(11, arg.Entry.Properties.Count());
                }
            });

        _context.BuildingRequest += (sender, args) =>
        {
            Assert.Equal("PATCH", args.Method);
        };

        var responses = await _context.SaveChangesAsync();
        Assert.Single(responses);
        Assert.Equal(204, responses.First().StatusCode);
        Assert.EndsWith("odata/People(1)/Microsoft.OData.E2E.TestCommon.Common.Server.Default.Customer", ((responses.ElementAt(0) as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.EditLink.AbsoluteUri);
    }

    // This test verifies that primitive and collection properties of entities can be updated using PUT requests.
    // It covers updating properties of various entity types and ensures that the request pipeline correctly
    // handles the ReplaceOnUpdate option, which replaces the entire entity with the updated values.
    [Fact]
    public async Task UpdatePrimitiveAndCollectionPropertiesUsingPut()
    {
        // Arrange
        var _context = this.ContextWrapper();

        int expectedChangedPropertyCount = 0;

        _context.Configurations.RequestPipeline.OnEntryEnding(
            (arg) =>
            {
                Assert.Equal(expectedChangedPropertyCount, arg.Entry.Properties.Count());
            });

        var orders = new DataServiceCollection<Order>(_context.Orders.Expand("OrderDetails"));

        // Update primitive type and collection property under entity
        orders[1].OrderDate = DateTimeOffset.Now;
        orders[1].OrderShelfLifes.Add(TimeSpan.FromHours(1.2));

        _context.BuildingRequest += (sender, args) =>
        {
            Assert.Equal("PUT", args.Method);
        };

        expectedChangedPropertyCount = 7;
        var responses = await _context.SaveChangesAsync(SaveChangesOptions.ReplaceOnUpdate);

        // Assert
        Assert.Single(responses);
        Assert.Equal(204, responses.First().StatusCode);
        Assert.EndsWith("odata/Orders(8)", ((responses.ElementAt(0) as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.EditLink.AbsoluteUri);
    }

    // This test verifies that multiple entities can be updated using PATCH requests in a single batch.
    // It covers updating properties of various entity types and ensures that the request pipeline correctly
    // handles batch requests with a single changeset, updating all entities in a single transaction.
    [Fact]
    public async Task UpdateMultipleEntitiesUsingPatchInBatch()
    {
        // Arrange
        var _context = this.ContextWrapper();

        _context.Configurations.RequestPipeline.OnEntryEnding(
            (arg) =>
            {
                Assert.Single(arg.Entry.Properties);
            });

        // Query the entities
        var orders = new DataServiceCollection<Order>(_context.Orders.Expand("OrderDetails"));
        var people = new DataServiceCollection<Person>(_context.People);
        var boss = new DataServiceCollection<Person>(_context.Boss);


        _context.BuildingRequest += (sender, args) =>
        {
            if (args.RequestUri.AbsoluteUri.EndsWith("odata/$batch"))
            {
                Assert.Equal("POST", args.Method);
            }
            else if (args.RequestUri.AbsoluteUri.EndsWith("odata/Orders(8)") ||
                     args.RequestUri.AbsoluteUri.EndsWith("odata/People(1)/Microsoft.OData.E2E.TestCommon.Common.Server.Default.Customer") ||
                     args.RequestUri.AbsoluteUri.EndsWith("odata/Boss/Microsoft.OData.E2E.TestCommon.Common.Server.Default.Customer") ||
                     args.RequestUri.AbsoluteUri.EndsWith("odata/OrderDetails(OrderID=7,ProductID=6)"))
            {
                Assert.Equal("PATCH", args.Method);
            }
        };

        // Update the entities
        orders[1].OrderDate = DateTimeOffset.Now;
        ((Customer)people[0]).City = "Redmond";
        boss.Single().FirstName = "Bill";
        orders[0].OrderDetails.First().Quantity = 1;

        // Save the changes in batch with a single changeset
        var responses = await _context.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset);

        // Assert
        Assert.Equal(4, responses.Count());
        Assert.All(responses, response =>
        {
            Assert.Equal(204, response.StatusCode);
            Assert.Null(response.Error);
        });
        Assert.EndsWith("odata/Orders(8)", ((responses.ElementAt(0) as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.EditLink.AbsoluteUri);
        Assert.EndsWith("odata/People(1)/Microsoft.OData.E2E.TestCommon.Common.Server.Default.Customer", ((responses.ElementAt(1) as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.EditLink.AbsoluteUri);
        Assert.EndsWith("odata/Boss/Microsoft.OData.E2E.TestCommon.Common.Server.Default.Customer", ((responses.ElementAt(2) as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.EditLink.AbsoluteUri);
        Assert.EndsWith("odata/OrderDetails(OrderID=7,ProductID=6)", ((responses.ElementAt(3) as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.EditLink.AbsoluteUri);
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
        var actionUri = new Uri(_baseUri + "defaultchangetrackingtests/Default.ResetDefaultDataSource", UriKind.Absolute);
        context.Execute(actionUri, "POST");
    }


    #endregion
}

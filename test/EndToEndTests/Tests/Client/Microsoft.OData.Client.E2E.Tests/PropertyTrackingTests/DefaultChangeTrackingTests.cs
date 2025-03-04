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
using Microsoft.OData.Edm;
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
    private readonly Container _context;
    private readonly IEdmModel _model;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(PropertyTrackingTestsController), typeof(MetadataController));

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

        _context = new Container(_baseUri)
        {
            HttpClientFactory = HttpClientFactory
        };

        _model = DefaultEdmModel.GetEdmModel();
        ResetDefaultDataSource();
    }

    // This test verifies that primitive and collection properties of entities can be updated using PATCH requests.
    // It covers updating properties of various entity types, including inherited and complex types, as well as
    // collection navigation properties and single value navigation properties.
    [Fact]
    public void UpdatePrimitiveAndCollectionPropertiesUsingPatch()
    {
        // Arrange
        int expectedPropertyCount = 0;

        _context.Configurations.RequestPipeline.OnEntryEnding(
            (arg) =>
            {
                if (arg.Entry.TypeName.EndsWith("Order")
                    || arg.Entry.TypeName.EndsWith("Person")
                    || arg.Entry.TypeName.EndsWith("OrderDetail")
                    || arg.Entry.TypeName.EndsWith("GiftCard"))
                {
                    Assert.Equal(expectedPropertyCount, arg.Entry.Properties.Count());
                }
            });

        var orders = new DataServiceCollection<Order>(_context.Orders.Expand("OrderDetails"));
        var people = new DataServiceCollection<Person>(_context.People);
        var boss = new DataServiceCollection<Person>(_context.Boss);
        var accounts = new DataServiceCollection<Account>(_context.Accounts.Expand("MyGiftCard"));

        // Act & Assert

        // Update primitive type and collection property under entity
        orders[1].OrderDate = DateTimeOffset.Now;
        orders[1].OrderShelfLifes.Add(TimeSpan.FromHours(1.2));

        expectedPropertyCount = 2;
        _context.SaveChanges();

        Assert.Equal(2, _context.Orders.Where((it) => it.OrderID == orders[1].OrderID).Single().OrderShelfLifes.Count);

        // Update primitive type under entity (inherited)
        ((Customer)people[0]).City = "Redmond";

        expectedPropertyCount = 1;
        _context.SaveChanges();

        Assert.Equal("Redmond", (_context.People.Where((it) => it.PersonID == people[0].PersonID).Single() as Customer)?.City);

        // Update the property under complex type.
        people[0].HomeAddress.City = "Redmond";

        _context.Configurations.RequestPipeline.OnEntryEnding(
        (arg) =>
        {
            if (arg.Entry.TypeName.EndsWith("HomeAddress"))
            {
                Assert.Equal("Redmond", (arg.Entry.Properties.Single(p => p.Name.Equals("City")) as ODataProperty)?.Value);
            }
        });

        expectedPropertyCount = 0;
        _context.SaveChanges();

        Assert.Equal("Redmond", _context.People.Where((it) => it.PersonID == people[0].PersonID).Single().HomeAddress.City);
        Assert.Equal("98052", _context.People.Where((it) => it.PersonID == people[0].PersonID).Single().HomeAddress.PostalCode);

        // Update the property under complex type (inherited).
        ((HomeAddress)people[0].HomeAddress).FamilyName = "Microsoft";

        expectedPropertyCount = 0;
        _context.SaveChanges();

        Assert.Equal("Microsoft", (_context.People.Where((it) => it.PersonID == people[0].PersonID).Single().HomeAddress as HomeAddress)?.FamilyName);

        // Update collection navigation property.
        orders[0].OrderDetails.First().Quantity = 1;

        expectedPropertyCount = 1;
        _context.SaveChanges();

        Assert.Equal(1,
            _context.OrderDetails.Where(
                (it) => it.ProductID == orders[0].OrderDetails.First().ProductID && it.OrderID == orders[0].OrderDetails.First().OrderID
                ).Single().Quantity);

        // Update single value navigation property.
        accounts[0].MyGiftCard.ExperationDate = DateTimeOffset.Now;

        expectedPropertyCount = 1;
        _context.SaveChanges();

        Assert.Equal(accounts[0].MyGiftCard.ExperationDate,
            _context.Accounts.Expand("MyGiftCard").Where((it) => it.AccountID == accounts[0].AccountID).Single().MyGiftCard.ExperationDate);

        // Update property in singleton.
        boss.Single().FirstName = "Bill";

        expectedPropertyCount = 1;
        _context.SaveChanges();

        Assert.Equal(
            "Bill",
            _context.Boss.GetValue().FirstName);
    }

    // This test verifies that updating an object without any changes results in a PATCH request
    // that includes all properties of the entity. It ensures that the request pipeline correctly
    // handles such scenarios and includes the expected number of properties in the request.
    [Fact]
    public void UpdateObjectWithoutChangesUsingPatch()
    {
        // Arrange
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

        _context.SaveChanges();
    }

    // This test verifies that primitive and collection properties of entities can be updated using PUT requests.
    // It covers updating properties of various entity types and ensures that the request pipeline correctly
    // handles the ReplaceOnUpdate option, which replaces the entire entity with the updated values.
    [Fact]
    public void UpdatePrimitiveAndCollectionPropertiesUsingPut()
    {
        // Arrange
        int expectedPropertyCount = 0;

        _context.Configurations.RequestPipeline.OnEntryEnding(
            (arg) =>
            {
                Assert.Equal(expectedPropertyCount, arg.Entry.Properties.Count());
            });

        var orders = new DataServiceCollection<Order>(_context.Orders.Expand("OrderDetails"));

        // Update primitive type and collection property under entity
        orders[1].OrderDate = DateTimeOffset.Now;
        orders[1].OrderShelfLifes.Add(TimeSpan.FromHours(1.2));

        expectedPropertyCount = 7;
        _context.SaveChanges(SaveChangesOptions.ReplaceOnUpdate);
    }

    // This test verifies that multiple entities can be updated using PATCH requests in a single batch.
    // It covers updating properties of various entity types and ensures that the request pipeline correctly
    // handles batch requests with a single changeset, updating all entities in a single transaction.
    [Fact]
    public void UpdateMultipleEntitiesUsingPatchInBatch()
    {
        // Arrange
        int expectedPropertyCount = 1;

        _context.Configurations.RequestPipeline.OnEntryEnding(
            (arg) =>
            {
                Assert.Equal(expectedPropertyCount, arg.Entry.Properties.Count());
            });

        // Query the entities
        var orders = new DataServiceCollection<Order>(_context.Orders.Expand("OrderDetails"));
        var people = new DataServiceCollection<Person>(_context.People);
        var boss = new DataServiceCollection<Person>(_context.Boss);

        // Update the entities
        orders[1].OrderDate = DateTimeOffset.Now;
        ((Customer)people[0]).City = "Redmond";
        boss.Single().FirstName = "Bill";
        orders[0].OrderDetails.First().Quantity = 1;

        // Save the changes in batch with a single changeset
        _context.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);
    }

    #region Private

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "propertytrackingtests/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    #endregion
}

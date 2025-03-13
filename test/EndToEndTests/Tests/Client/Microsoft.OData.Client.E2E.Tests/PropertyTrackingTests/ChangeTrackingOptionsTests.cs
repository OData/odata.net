//-----------------------------------------------------------------------------
// <copyright file="ChangeTrackingOptionsTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.ObjectModel;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common.Client.Default.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.PropertyTrackingTests;
using Microsoft.Spatial;
using Xunit;
using AccessLevel = Microsoft.OData.E2E.TestCommon.Common.Client.Default.AccessLevel;
using Account = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Account;
using AccountInfo = Microsoft.OData.E2E.TestCommon.Common.Client.Default.AccountInfo;
using Color = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Color;
using Company = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Company;
using Customer = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Customer;
using GiftCard = Microsoft.OData.E2E.TestCommon.Common.Client.Default.GiftCard;
using HomeAddress = Microsoft.OData.E2E.TestCommon.Common.Client.Default.HomeAddress;
using Order = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Order;
using Person = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Person;
using Product = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Product;

namespace Microsoft.OData.Client.E2E.Tests.PropertyTrackingTests;

/// <summary>
/// End-to-end tests for change tracking options.
/// </summary>
public class ChangeTrackingOptionsTests : EndToEndTestBase<ChangeTrackingOptionsTests.TestsStartup>
{
    private readonly Uri _baseUri;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(ChangeTrackingOptionsTestsController), typeof(MetadataController));

            services.AddControllers().AddOData(opt =>
                opt.EnableQueryFeatures().AddRouteComponents("odata", DefaultEdmModel.GetEdmModel(), batchHandler: new DefaultODataBatchHandler()));
        }
    }

    public ChangeTrackingOptionsTests(TestWebApplicationFactory<TestsStartup> fixture) : base(fixture)
    {
        if (Client.BaseAddress == null)
        {
            throw new ArgumentNullException(nameof(Client.BaseAddress), "Base address cannot be null");
        }

        _baseUri = new Uri(Client.BaseAddress, "odata/");
    }

    // This test verifies that updating enum properties and collections of enum properties in an entity works correctly.
    [Fact]
    public async Task UpdateEnumPropertiesAndCollectionsTest()
    {
        // Arrange
        var context = this.ContextWrapper();
        var products = new DataServiceCollection<Product>(context, "Products", null, null)
        {
            new Product
            {
                Name = "Apple",
                ProductID = 1000000,
                QuantityInStock = 20,
                QuantityPerUnit = "Pound",
                UnitPrice = 0.35f,
                Discontinued = false,
                SkinColor = Color.Red,
                CoverColors = new ObservableCollection<Color> { Color.Blue },
                UserAccess = AccessLevel.Read
            }
        };

        context.Configurations.RequestPipeline.OnEntryEnding(arg =>
        {
            if (arg.Entry.TypeName.EndsWith("Product"))
            {
                Assert.Equal(10, arg.Entry.Properties.Count());
            }
        });

        // Act
        products[0].CoverColors?.Add(Color.Green);
        products[0].UserAccess = AccessLevel.Execute;
        products[0].SkinColor = Color.Green;
        var responses = await context.SaveChangesAsync();

        // Assert
        Assert.Equal(201, responses.ElementAt(0).StatusCode);
        Assert.EndsWith("odata/Products(1000000)", ((responses.ElementAt(0) as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.Identity.AbsoluteUri);

        var product = context.Products.Where(it => it.ProductID == products[0].ProductID).First();
        Assert.Equal(2, product.CoverColors.Count);
        Assert.Equal(Color.Green, product.SkinColor);
        Assert.Equal(AccessLevel.Execute, product.UserAccess);
        Assert.Equal(2, product.CoverColors.Count);
    }

    // This test verifies that updating primitive properties and collections in an entity works correctly.
    [Fact]
    public async Task UpdatePrimitivePropertiesAndCollectionsTest()
    {
        // Arrange
        var context = this.ContextWrapper();
        var people = new DataServiceCollection<Person>(context, "Customers", null, null)
        {
            new Customer
            {
                FirstName = "Nelson",
                MiddleName = "S.",
                LastName = "Black",
                Numbers = new ObservableCollection<string> { "111-111-1111" },
                Emails = new ObservableCollection<string> { "abc@abc.com" },
                PersonID = 10001,
                Birthday = new DateTimeOffset(new DateTime(1957, 4, 3)),
                City = "London",
                Home = GeographyPoint.Create(32.1, 23.1),
                TimeBetweenLastTwoOrders = new TimeSpan(1),
                HomeAddress = new HomeAddress
                {
                    City = "London",
                    PostalCode = "98052",
                    Street = "1 Microsoft Way",
                    FamilyName = "Black's Family"
                }
            }
        };

        context.Configurations.RequestPipeline.OnEntryEnding(arg =>
        {
            if (arg.Entry.TypeName.EndsWith("Person"))
            {
                Assert.Equal(2, arg.Entry.Properties.Count());
            }
        });

        // Act
        people[0].FirstName = "Balck";
        people[0].Emails.Add("test123@var1.com");
        var responses = await context.SaveChangesAsync();

        Assert.Single(responses);
        var response = responses.Single();

        // Assert
        Assert.Equal(201, response.StatusCode);
        Assert.EndsWith("odata/Customers(10001)", ((response as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.Identity.AbsoluteUri);

        var person = context.Customers.Where(c => c.PersonID == people[0].PersonID).First();
        Assert.Equal("Balck", person.FirstName);
        Assert.Equal(2, person.Emails.Count);
    }

    // This test verifies that updating properties under complex types works correctly.
    [Fact]
    public async Task UpdatePropertiesUnderComplexTypesTest()
    {
        // Arrange
        var context = this.ContextWrapper();
        var people = new DataServiceCollection<Person>(context, "Customers", null, null)
        {
            new Customer
            {
                FirstName = "Nelson",
                MiddleName = "S.",
                LastName = "Black",
                Numbers = new ObservableCollection<string> { "111-111-1111" },
                Emails = new ObservableCollection<string> { "abc@abc.com" },
                PersonID = 10001,
                Birthday = new DateTimeOffset(new DateTime(1957, 4, 3)),
                City = "London",
                Home = GeographyPoint.Create(32.1, 23.1),
                TimeBetweenLastTwoOrders = new TimeSpan(1),
                HomeAddress = new HomeAddress
                {
                    City = "London",
                    PostalCode = "98052",
                    Street = "1 Microsoft Way",
                    FamilyName = "Black's Family"
                }
            }
        };

        context.Configurations.RequestPipeline.OnEntryEnding(arg =>
        {
            if (arg.Entry.TypeName.EndsWith("HomeAddress"))
            {
                Assert.Equal(5, arg.Entry.Properties.Count());
                Assert.Equal("Redmond", (arg.Entry.Properties.Single(p => p.Name == "City") as ODataProperty)?.Value);
            }
        });

        // Act
        people[0].HomeAddress.City = "Redmond";
        var responses = await context.SaveChangesAsync();

        Assert.Single(responses);
        var response = responses.Single();

        // Assert
        Assert.Equal(201, response.StatusCode);
        Assert.EndsWith("odata/Customers(10001)", ((response as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.Identity.AbsoluteUri);

        var person = context.Customers.Where(c => c.PersonID == people[0].PersonID).First();
        Assert.Equal("Redmond", person.HomeAddress.City);
    }

    // This test verifies that updating navigation properties works correctly.
    [Fact]
    public async Task UpdateNavigationPropertiesTest()
    {
        // Arrange
        var context = this.ContextWrapper();
        var people = new DataServiceCollection<Person>(context, "Customers", null, null)
        {
            new Customer
            {
                FirstName = "Nelson",
                MiddleName = "S.",
                LastName = "Black",
                Numbers = new ObservableCollection<string> { "111-111-1111" },
                Emails = new ObservableCollection<string> { "abc@abc.com" },
                PersonID = 10001,
                Birthday = new DateTimeOffset(new DateTime(1957, 4, 3)),
                City = "London",
                Home = GeographyPoint.Create(32.1, 23.1),
                TimeBetweenLastTwoOrders = new TimeSpan(1),
                HomeAddress = new HomeAddress
                {
                    City = "London",
                    PostalCode = "98052",
                    Street = "1 Microsoft Way",
                    FamilyName = "Black's Family"
                }
            }
        };

        var orders = new DataServiceCollection<Order>(context, "Orders", null, null)
        {
            new Order
            {
                OrderID = 11111111,
                OrderDate = new DateTimeOffset(new DateTime(2011, 5, 29, 14, 21, 12)),
                ShelfLife = new TimeSpan(1),
                OrderShelfLifes = new ObservableCollection<TimeSpan> { new TimeSpan(1) }
            }
        };

        var savedDataResponses = await context.SaveChangesAsync();
        Assert.Equal(2, savedDataResponses.Count());
        Assert.EndsWith("odata/Customers(10001)", ((savedDataResponses.First() as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.Identity.AbsoluteUri);
        Assert.EndsWith("odata/Orders(11111111)", ((savedDataResponses.Last() as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.Identity.AbsoluteUri);

        var customer = context.Customers.Where(c => c.PersonID == people[0].PersonID).First();
        context.AddLink(customer, "Orders", orders[0]);
        var responses = await context.SaveChangesAsync();
        Assert.Equal(200, responses.Single().StatusCode);

        context.Configurations.RequestPipeline.OnEntryEnding(arg =>
        {
            if (arg.Entry.TypeName.EndsWith("Order"))
            {
                Assert.Equal(2, arg.Entry.Properties.Count());
            }
        });

        // Act
        context.LoadProperty(customer, "Orders");

        var datetime = new DateTimeOffset(new DateTime(1957, 4, 3));
        customer.Orders.First().OrderDate = datetime;
        customer.Orders.First().OrderShelfLifes.Add(new TimeSpan(2));
        var updateResponses = await context.SaveChangesAsync();
        Assert.EndsWith("odata/Orders(11111111)", ((updateResponses.First() as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.Identity.AbsoluteUri);

        // Assert
        var updatedCustomer = context.Customers.Expand(it => it.Orders).Where(it => it.PersonID == people[0].PersonID).Single();
        Assert.Equal(datetime, updatedCustomer.Orders.First().OrderDate);
        Assert.Equal(2, updatedCustomer.Orders.First().OrderShelfLifes.Count());
    }

    // This test verifies that updating properties in open complex types works correctly.
    [Fact]
    public async Task UpdatePropertiesInOpenComplexTypesTest()
    {
        // Arrange
        var context = this.ContextWrapper();
        var accounts = new DataServiceCollection<Account>(context, "Accounts", null, null)
        {
            new Account
            {
                AccountID = 110,
                CountryRegion = "CN",
                AccountInfo = new AccountInfo
                {
                    FirstName = "New",
                    LastName = "Boy"
                }
            }
        };

        var giftCard = new GiftCard
        {
            GiftCardID = 30000,
            GiftCardNO = "AAA123A",
            Amount = 19.9,
            ExperationDate = new DateTimeOffset(new DateTime(2013, 12, 30))
        };

        context.UpdateRelatedObject(accounts[0], "MyGiftCard", giftCard);
        var responses = await context.SaveChangesAsync();
        var createAccountResponse = responses.First();
        Assert.Equal(201, createAccountResponse.StatusCode);
        Assert.EndsWith("odata/Accounts(110)", ((createAccountResponse as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.Identity.AbsoluteUri);

        var updateAccountResponse = responses.Last();
        Assert.Equal(204, updateAccountResponse.StatusCode);
        Assert.EndsWith("odata/Accounts/MyGiftCard", ((updateAccountResponse as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.EditLink.AbsoluteUri);

        context.Configurations.RequestPipeline.OnEntryEnding(arg =>
        {
            if (arg.Entry.TypeName.EndsWith("AccountInfo"))
            {
                Assert.Equal(3, arg.Entry.Properties.Count());
                Assert.Equal("S.", (arg.Entry.Properties.Single(p => p.Name == "LastName") as ODataProperty)?.Value);
            }
        });

        var datetime = new DateTimeOffset(new DateTime(1957, 4, 3));

        context.LoadProperty(accounts[0], "MyGiftCard");

        // Act
        accounts[0].MyGiftCard.ExperationDate = datetime;
        accounts[0].AccountInfo.LastName = "S.";
        var updateResponses = await context.SaveChangesAsync();

        Assert.Equal(2, updateResponses.Count());
        Assert.EndsWith("odata/Accounts(110)/MyGiftCard", ((updateResponses.First() as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.Identity.AbsoluteUri);
        Assert.EndsWith("odata/Accounts(110)", ((updateResponses.Last() as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.Identity.AbsoluteUri);

        // Assert
        var account = context.Accounts.Expand("MyGiftCard").Where(it => it.AccountID == accounts[0].AccountID).Single();
        Assert.Equal(datetime, account.MyGiftCard.ExperationDate);
        Assert.Equal("S.", account.AccountInfo.LastName);
        Assert.Equal("New", account.AccountInfo.FirstName);
    }

    // This test verifies that updating properties in open singletons works correctly.
    [Fact]
    public async Task UpdatePropertiesInOpenSingletons()
    {
        // Arrange
        var context = this.ContextWrapper();
        var publicCompany = new DataServiceCollection<Company>(context.PublicCompany);

        context.Configurations.RequestPipeline.OnEntryEnding(arg =>
        {
            if (arg.Entry.TypeName.EndsWith("Company"))
            {
                Assert.Equal(2, arg.Entry.Properties.Count());
            }
        });

        // Act
        publicCompany.Single().Revenue = 200;
        publicCompany.Single().Name = "MS Ltd.";
        var responses = await context.SaveChangesAsync();

        Assert.Single(responses);
        Assert.Equal(204, responses.Single().StatusCode);
        Assert.EndsWith("odata/PublicCompany", ((responses.First() as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.Identity.AbsoluteUri);

        // Assert
        var company = await context.PublicCompany.GetValueAsync();
        Assert.Equal(200, company.Revenue);
        Assert.Equal("MS Ltd.", company.Name);
    }

    // This test verifies that updating an object without any changes results in a PATCH request that includes all properties.
    [Fact]
    public async Task UpdateObjectWithoutChanges()
    {
        // Arrange
        var context = this.ContextWrapper();
        var people = new DataServiceCollection<Person>(context, "Customers", null, null)
        {
            new Customer
            {
                FirstName = "Nelson",
                MiddleName = "S.",
                LastName = "Black",
                Numbers = new ObservableCollection<string> { "111-111-1111" },
                Emails = new ObservableCollection<string> { "abc@abc.com" },
                PersonID = 10001,
                Birthday = new DateTimeOffset(new DateTime(1957, 4, 3)),
                City = "London",
                Home = GeographyPoint.Create(32.1, 23.1),
                TimeBetweenLastTwoOrders = new TimeSpan(1),
                HomeAddress = new HomeAddress
                {
                    City = "London",
                    PostalCode = "98052",
                    Street = "1 Microsoft Way",
                    FamilyName = "Black's Family"
                }
            }
        };

        context.Configurations.RequestPipeline.OnEntryEnding(arg =>
        {
            if (arg.Entry.TypeName.EndsWith("Person"))
            {
                Assert.Equal(10, arg.Entry.Properties.Count());
            }
        });

        // Act
        context.UpdateObject(people[0]);
        var responses = await context.SaveChangesAsync();
        Assert.Equal(201, responses.First().StatusCode);
        Assert.EndsWith("odata/Customers(10001)", ((responses.First() as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.Identity.AbsoluteUri);

        // Assert
        var person = context.Customers.Where(c => c.PersonID == people[0].PersonID).First();
        Assert.Equal("Nelson", person.FirstName);
        Assert.Equal("Black", person.LastName);
        Assert.Single(person.Emails);
    }

    // This test verifies that updating property under inherited primitive type and complex type works correctly.
    [Fact]
    public async Task UpdateInheritedPrimitiveAndComplexTypePropertyTest()
    {
        // Arrange
        var context = this.ContextWrapper();
        var people = new DataServiceCollection<Person>(context, "Customers", null, null)
        {
            new Customer
            {
                FirstName = "Nelson",
                MiddleName = "S.",
                LastName = "Black",
                Numbers = new ObservableCollection<string> { "111-111-1111" },
                Emails = new ObservableCollection<string> { "abc@abc.com", "jkl@jkl.com" },
                PersonID = 10001,
                Birthday = new DateTimeOffset(new DateTime(1957, 4, 3)),
                City = "London",
                Home = GeographyPoint.Create(32.1, 23.1),
                TimeBetweenLastTwoOrders = new TimeSpan(1),
                HomeAddress = new HomeAddress
                {
                    City = "London",
                    PostalCode = "98052",
                    Street = "1 Microsoft Way",
                    FamilyName = "Black's Family"
                }
            }
        };


        context.Configurations.RequestPipeline.OnEntryEnding(arg =>
        {
            if (arg.Entry.TypeName.EndsWith("HomeAddress"))
            {
                Assert.Equal(5, arg.Entry.Properties.Count());
                Assert.Equal("Redmond", (arg.Entry.Properties.Single(p => p.Name == "City") as ODataProperty)?.Value);
            }
        });

        // Act
        var datetime = new DateTimeOffset(new DateTime(1957, 4, 3));

        // Update primitive type and collection property under entity (inherited)
        ((Customer)people[0]).Birthday = new DateTimeOffset(new DateTime(1957, 4, 3));

        people[0].HomeAddress.City = "Redmond";

        // Update the property under complex type (inherited).
        ((HomeAddress)people[0].HomeAddress).FamilyName = "Microsoft";

        var responses = await context.SaveChangesAsync();
        Assert.Equal(201, responses.First().StatusCode);
        Assert.EndsWith("odata/Customers(10001)", ((responses.First() as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.Identity.AbsoluteUri);

        // Assert
        var person = context.Customers.Expand(it => it.Orders).Where((it) => it.PersonID == people[0].PersonID).Single();
        Assert.Equal(2, person.Emails.Count);
        Assert.Equal(datetime, (person as Customer).Birthday);
        Assert.Equal("Redmond", person.HomeAddress.City);
        Assert.Equal("Microsoft", (person.HomeAddress as HomeAddress)?.FamilyName);
        Assert.Equal("98052", person.HomeAddress.PostalCode);
    }

    // This test verifies that partial properties of various entities can be updated using PATCH requests in a single batch.
    // It covers updating properties of derived types, complex types, and navigation properties.
    // The test ensures that the request pipeline correctly handles batch requests with a single changeset.
    [Fact]
    public async Task BatchUpdatePartialPropertiesWithPatchRequestsTest()
    {
        // Arrange
        var context = this.ContextWrapper();

        context.Configurations.RequestPipeline.OnEntryEnding(
            (arg) =>
            {
                Assert.Single(arg.Entry.Properties);
            });

        var orders = new DataServiceCollection<Order>(context.Orders.Expand("OrderDetails"));
        var people = new DataServiceCollection<Person>(context.People);
        var boss = new DataServiceCollection<Person>(context.Boss);

        var dateTime = DateTimeOffset.Now;

        // Act
        context.BuildingRequest += (sender, args) =>
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

        orders[1].OrderDate = dateTime;
        ((Customer)people[0]).City = "Redmond";
        boss.Single().FirstName = "Bill";
        orders[0].OrderDetails.First().Quantity = 1;
        var responses = await context.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset);

        // Assert
        Assert.Equal(4, responses.Count());
        Assert.EndsWith("odata/Orders(8)", ((responses.ElementAt(0) as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.EditLink.AbsoluteUri);
        Assert.EndsWith("odata/People(1)/Microsoft.OData.E2E.TestCommon.Common.Server.Default.Customer", ((responses.ElementAt(1) as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.EditLink.AbsoluteUri);
        Assert.EndsWith("odata/Boss/Microsoft.OData.E2E.TestCommon.Common.Server.Default.Customer", ((responses.ElementAt(2) as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.EditLink.AbsoluteUri);
        Assert.EndsWith("odata/OrderDetails(OrderID=7,ProductID=6)", ((responses.ElementAt(3) as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.EditLink.AbsoluteUri);

        Assert.All(responses, response => Assert.Equal(204, response.StatusCode));
        Assert.Equal(dateTime, orders[1].OrderDate);
        Assert.Equal("Bill", (await context.Boss.GetValueAsync()).FirstName);
        Assert.Equal("Redmond", ((Customer)context.People.First(p => p.PersonID == people[0].PersonID)).City);
        Assert.Equal(1, orders[0].OrderDetails.First().Quantity);
    }

    // This test verifies that full properties of various entities can be updated using PUT requests in a single batch.
    // It covers updating properties of derived types, complex types, and navigation properties.
    // The test ensures that the request pipeline correctly handles batch requests with the ReplaceOnUpdate option.
    [Fact]
    public async Task BatchUpdateFullPropertiesWithPutRequestsTest()
    {
        // Arrange
        var context = this.ContextWrapper();

        int expectedChangedPropertyCount = 0;
        var saveChangesOption = (SaveChangesOptions.ReplaceOnUpdate | SaveChangesOptions.BatchWithSingleChangeset);

        context.Configurations.RequestPipeline.OnEntryEnding(
            (arg) =>
            {
                if (arg.Entry.TypeName.EndsWith("Order")
                    || arg.Entry.TypeName.EndsWith("Customer")
                    || arg.Entry.TypeName.EndsWith("OrderDetail"))
                    Assert.Equal(expectedChangedPropertyCount, arg.Entry.Properties.Count());
            });

        var orders = new DataServiceCollection<Order>(context.Orders.Expand("OrderDetails").Take(2));
        var people = new DataServiceCollection<Person>(context.People.Take(1));
        var boss = new DataServiceCollection<Person>(context.Boss);

        context.BuildingRequest += (sender, args) =>
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
                Assert.Equal("PUT", args.Method);
            }
        };

        // Act
        var dateTime = DateTimeOffset.Now;
        orders[1].OrderDate = dateTime;
        expectedChangedPropertyCount = 7;
        await context.SaveChangesAsync(saveChangesOption);

        ((Customer)people[0]).City = "Redmond";
        boss.Single().FirstName = "Bill";
        expectedChangedPropertyCount = 11;
        await context.SaveChangesAsync(saveChangesOption);

        orders[0].OrderDetails.First().Quantity = 1;
        expectedChangedPropertyCount = 6;
        await context.SaveChangesAsync(saveChangesOption);

        context.MergeOption = MergeOption.OverwriteChanges;

        var orders2 = context.Orders.Expand(o => o.OrderDetails).Take(2).ToList();
        Assert.Equal(dateTime, orders2[1].OrderDate);
        Assert.Equal("Bill", context.Boss.GetValue().FirstName);
        Assert.Equal("Redmond", ((Customer)context.People.First(p => p.PersonID == people[0].PersonID)).City);
        Assert.Equal(1, orders2[0].OrderDetails.First().Quantity);
    }

    // This test verifies that updating properties of derived types and complex types using POST tunneling works correctly.
    [Fact]
    public async Task UpdateDerivedAndComplexTypesUsingPostTunnelingTest()
    {
        // Arrange
        var context = this.ContextWrapper();
        context.MergeOption = MergeOption.OverwriteChanges;
        context.UsePostTunneling = true;

        var people = new DataServiceCollection<Person>(context, "Customers", null, null)
        {
            new Customer
            {
                FirstName = "Nelson",
                MiddleName = "S.",
                LastName = "Black",
                Numbers = new ObservableCollection<string> { "111-111-1111" },
                Emails = new ObservableCollection<string> { "abc@abc.com" },
                PersonID = 10001,
                Birthday = new DateTimeOffset(new DateTime(1957, 4, 3)),
                City = "London",
                Home = GeographyPoint.Create(32.1, 23.1),
                TimeBetweenLastTwoOrders = new TimeSpan(1),
                HomeAddress = new HomeAddress
                {
                    City = "London",
                    PostalCode = "98052",
                    Street = "1 Microsoft Way",
                    FamilyName = "Black's Family"
                }
            }
        };

        var orders = new DataServiceCollection<Order>(context, "Orders", null, null)
        {
            new Order
            {
                OrderID = 11111111,
                OrderDate = new DateTimeOffset(new DateTime(2011, 5, 29, 14, 21, 12)),
                ShelfLife = new TimeSpan(1),
                OrderShelfLifes = new ObservableCollection<TimeSpan> { new TimeSpan(1) }
            }
        };

        await context.SaveChangesAsync();

        var customer = context.Customers.Where(c => c.PersonID == people[0].PersonID).First();
        context.AddLink(customer, "Orders", orders[0]);
        var responses = await context.SaveChangesAsync();
        Assert.Equal(200, responses.Single().StatusCode);

        int expectedChangedPropertyCount = 0;
        context.Configurations.RequestPipeline.OnEntryEnding(arg =>
        {
            if (arg.Entry.TypeName.EndsWith("Customer") || arg.Entry.TypeName.EndsWith("Order"))
            {
                Assert.Equal(expectedChangedPropertyCount, arg.Entry.Properties.Count());
            }
        });

        // Act
        people[0].FirstName = "Black";
        people[0].Emails.Add("test123@var1.com");
        expectedChangedPropertyCount = 2;
        responses = await context.SaveChangesAsync();
        Assert.Equal(204, responses.First().StatusCode);
        Assert.EndsWith("odata/Customers(10001)", ((responses.First() as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.EditLink.AbsoluteUri);

        var datetime = new DateTimeOffset(new DateTime(1957, 4, 3));
        ((Customer)people[0]).Birthday = datetime;
        expectedChangedPropertyCount = 1;
        responses = await context.SaveChangesAsync();
        Assert.Equal(204, responses.First().StatusCode);
        Assert.EndsWith("odata/Customers(10001)", ((responses.First() as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.EditLink.AbsoluteUri);

        people[0].HomeAddress.City = "Redmond";
        expectedChangedPropertyCount = 0;
        responses = await context.SaveChangesAsync();
        Assert.Equal(204, responses.First().StatusCode);
        Assert.EndsWith("odata/Customers(10001)", ((responses.First() as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.EditLink.AbsoluteUri);

        ((HomeAddress)people[0].HomeAddress).FamilyName = "Microsoft";
        responses = await context.SaveChangesAsync();
        Assert.Equal(204, responses.First().StatusCode);
        Assert.EndsWith("odata/Customers(10001)", ((responses.First() as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.EditLink.AbsoluteUri);

        context.LoadProperty(customer, "Orders");
        customer.Orders.First().OrderDate = datetime;
        customer.Orders.First().OrderShelfLifes.Add(new TimeSpan(2));
        expectedChangedPropertyCount = 2;
        responses = await context.SaveChangesAsync();
        Assert.Equal(204, responses.First().StatusCode);
        Assert.EndsWith("odata/Orders(11111111)", ((responses.First() as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.EditLink.AbsoluteUri);

        // Assert
        var updatedCustomer = context.Customers.Expand(it => it.Orders).Where(it => it.PersonID == people[0].PersonID).Single();
        Assert.Equal(2, updatedCustomer.Emails.Count);
        Assert.Equal(datetime, (updatedCustomer as Customer).Birthday);
        Assert.Equal("Redmond", updatedCustomer.HomeAddress.City);
        Assert.Equal("Microsoft", (updatedCustomer.HomeAddress as HomeAddress)?.FamilyName);
        Assert.Equal("98052", updatedCustomer.HomeAddress.PostalCode);
        Assert.Equal(datetime, updatedCustomer.Orders.First().OrderDate);
        Assert.Equal(2, updatedCustomer.Orders.First().OrderShelfLifes.Count());
    }

    // This test verifies that updating properties of open entity types and entities with enum properties using POST tunneling works correctly.
    [Fact]
    public async Task UpdateOpenEntityTypesAndEnumPropertiesUsingPostTunnelingTest()
    {
        // Arrange
        var context = this.ContextWrapper();
        context.MergeOption = MergeOption.OverwriteChanges;
        context.UsePostTunneling = true;

        var accounts = new DataServiceCollection<Account>(context, "Accounts", null, null)
        {
            new Account
            {
                AccountID = 110,
                CountryRegion = "CN",
                AccountInfo = new AccountInfo
                {
                    FirstName = "New",
                    LastName = "Boy"
                }
            }
        };

        var giftCard = new GiftCard
        {
            GiftCardID = 30000,
            GiftCardNO = "AAA123A",
            Amount = 19.9,
            ExperationDate = new DateTimeOffset(new DateTime(2013, 12, 30))
        };

        var products = new DataServiceCollection<Product>(context, "Products", null, null)
        {
            new Product
            {
                Name = "Apple",
                ProductID = 1000000,
                QuantityInStock = 20,
                QuantityPerUnit = "Pound",
                UnitPrice = 0.35f,
                Discontinued = false,
                SkinColor = Color.Red,
                CoverColors = new ObservableCollection<Color> { Color.Blue },
                UserAccess = AccessLevel.Read
            }
        };

        context.UpdateRelatedObject(accounts[0], "MyGiftCard", giftCard);
        var responses = await context.SaveChangesAsync();
        var createAccountResponse = responses.First();
        Assert.Equal(201, createAccountResponse.StatusCode);
        Assert.EndsWith("odata/Accounts(110)", ((createAccountResponse as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.Identity.AbsoluteUri);

        int expectedChangedPropertyCount = 0;
        context.Configurations.RequestPipeline.OnEntryEnding(arg =>
        {
            if (arg.Entry.TypeName.EndsWith("Product") || arg.Entry.TypeName.EndsWith("AccountInfo"))
            {
                Assert.Equal(expectedChangedPropertyCount, arg.Entry.Properties.Count());
            }
        });

        // Act
        products[0].CoverColors.Add(Color.Green);
        products[0].UserAccess = AccessLevel.Execute;
        products[0].SkinColor = Color.Green;
        expectedChangedPropertyCount = 3;
        responses = await context.SaveChangesAsync();
        Assert.Equal(204, responses.First().StatusCode);
        Assert.EndsWith("odata/Products(1000000)", ((responses.First() as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.Identity.AbsoluteUri);

        var product = context.Products.Where(it => it.ProductID == products[0].ProductID).First();
        Assert.Equal(2, product.CoverColors.Count);
        Assert.Equal(Color.Green, product.SkinColor);
        Assert.Equal(AccessLevel.Execute, product.UserAccess);
        Assert.Equal(2, product.CoverColors.Count);

        context.LoadProperty(accounts[0], "MyGiftCard");

        accounts[0].MyGiftCard.ExperationDate = new DateTimeOffset(new DateTime(1957, 4, 3));
        expectedChangedPropertyCount = 1;
        var updateResponses = await context.SaveChangesAsync();
        Assert.Equal(204, updateResponses.First().StatusCode);
        Assert.EndsWith("odata/Accounts(110)/MyGiftCard", ((updateResponses.First() as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.Identity.AbsoluteUri);

        accounts[0].AccountInfo.LastName = "S.";
        expectedChangedPropertyCount = 3;
        updateResponses = await context.SaveChangesAsync();
        Assert.Equal(204, updateResponses.First().StatusCode);
        Assert.EndsWith("odata/Accounts(110)", ((updateResponses.First() as ChangeOperationResponse)?.Descriptor as EntityDescriptor)?.Identity.AbsoluteUri);

        // Assert
        var account = context.Accounts.Expand("MyGiftCard").Where(it => it.AccountID == accounts[0].AccountID).Single();
        Assert.Equal(new DateTimeOffset(new DateTime(1957, 4, 3)), account.MyGiftCard.ExperationDate);
        Assert.Equal("S.", account.AccountInfo.LastName);
        Assert.Equal("New", account.AccountInfo.FirstName);
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
        var actionUri = new Uri(_baseUri + "changetrackingoptionstests/Default.ResetDefaultDataSource", UriKind.Absolute);
        context.Execute(actionUri, "POST");
    }

    #endregion
}

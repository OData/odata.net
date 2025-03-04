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
using Microsoft.OData.Edm;
using Microsoft.Spatial;
using Xunit;
using AccessLevel = Microsoft.OData.E2E.TestCommon.Common.Client.Default.AccessLevel;
using Account = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Account;
using AccountInfo = Microsoft.OData.E2E.TestCommon.Common.Client.Default.AccountInfo;
using Color = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Color;
using Company = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Company;
using Customer = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Customer;
using HomeAddress = Microsoft.OData.E2E.TestCommon.Common.Client.Default.HomeAddress;
using Order = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Order;
using Person = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Person;
using Product = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Product;
using GiftCard = Microsoft.OData.E2E.TestCommon.Common.Client.Default.GiftCard;

namespace Microsoft.OData.Client.E2E.Tests.PropertyTrackingTests;

/// <summary>
/// End-to-end tests for change tracking options.
/// </summary>
public class ChangeTrackingOptionsTests : EndToEndTestBase<ChangeTrackingOptionsTests.TestsStartup>
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

    public ChangeTrackingOptionsTests(TestWebApplicationFactory<TestsStartup> fixture) : base(fixture)
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

    // This test verifies that updating an entity without any changes results in a PATCH request
    // that includes all properties of the entity. It ensures that the request pipeline correctly
    // handles such scenarios and includes the expected number of properties in the request.
    [Fact]
    public void UpdatePartialPropertiesWithCustomNames()
    {
        // Arrange
        var context = this.ContextWrapper();

        context.MergeOption = MergeOption.OverwriteChanges;
        //Entity of Derived type
        //Entity has a complex property of derived type
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
                },
            }
        };

        var orders = new DataServiceCollection<Order>(context, "Orders", null, null)
        {
            new Order
            {
                OrderID = 11111111,
                OrderDate = new DateTimeOffset(new DateTime(2011, 5, 29, 14, 21, 12)),
                ShelfLife = new TimeSpan(1),
                OrderShelfLifes = new ObservableCollection<TimeSpan>{new TimeSpan(1)}
            }
        };

        //Singleton of derived type
        //Singleton is of an open entity type
        var publicCompany = new DataServiceCollection<Company>(context.PublicCompany);

        //Entity with open complex type
        //Entity with contained Navigation
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
            },
            new Account
            {
                AccountID = 111,
                CountryRegion = "US",
                AccountInfo = new AccountInfo
                {
                    FirstName = "Old",
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

        //Entity with Enum property and collection of Enum property
        var products = new DataServiceCollection<Product>(context, "Products", null, null)
        {
            new  Product
            {
                Name = "Apple",
                ProductID = 1000000,
                QuantityInStock = 20,
                QuantityPerUnit = "Pound",
                UnitPrice = 0.35f,
                Discontinued = false,
                SkinColor = Color.Red,
                CoverColors = new ObservableCollection<Color>{ Color.Blue },
                UserAccess = AccessLevel.Read
            }
        };

        context.UpdateRelatedObject(accounts[0], "MyGiftCard", giftCard);
        context.SaveChanges();

        var customer = context.Customers.Where(c => c.PersonID == people[0].PersonID).First();
        context.AddLink(customer, "Orders", orders[0]);
        context.SaveChanges();

        int expectedPropertyCount = 0;
        Action<WritingEntryArgs> onEntryEndingAction = (arg) =>
        {
            if (arg.Entry.TypeName.EndsWith("Product")
                || arg.Entry.TypeName.EndsWith("Person"))
            {
                Assert.Equal(expectedPropertyCount, arg.Entry.Properties.Count());
            }
        };
        context.Configurations.RequestPipeline.OnEntryEnding(onEntryEndingAction);

        //Update Enum type and collection of enum property in entity
        products[0].CoverColors?.Add(Color.Green);
        products[0].UserAccess = AccessLevel.Execute;
        products[0].SkinColor = Color.Green;
        expectedPropertyCount = 3;
        context.SaveChanges();

        var product = context.Products.Where((it) => it.ProductID == products[0].ProductID).First();
        Assert.Equal(2, product.CoverColors.Count);
        Assert.Equal(Color.Green.ToString(), product.SkinColor.ToString());
        Assert.Equal(AccessLevel.Execute.ToString(), product.UserAccess.ToString());
        Assert.Equal(2, product.CoverColors.Count);

        // Update primitive type and collection property under entity
        people[0].FirstName = "Balck";
        people[0].Emails.Add("test123@var1.com");

        expectedPropertyCount = 2;
        context.SaveChanges();

        // Update primitive type and collection property under entity (inherited)
        var datetime = new DateTimeOffset(new DateTime(1957, 4, 3));
        ((Customer)people[0]).Birthday = new DateTimeOffset(new DateTime(1957, 4, 3));

        expectedPropertyCount = 1;
        context.SaveChanges();

        // Update the property under complex type.
        people[0].HomeAddress.City = "Redmond";
        Action<WritingEntryArgs> onEntryEndingAction1 = (arg) =>
        {
            if (arg.Entry.TypeName.EndsWith("HomeAddress"))
            {
                Assert.Equal(5, arg.Entry.Properties.Count());
                Assert.Equal("Redmond", (arg.Entry.Properties.Single(p => p.Name == "City") as ODataProperty)?.Value);
            }
        };
        context.Configurations.RequestPipeline.OnEntryEnding(onEntryEndingAction1);
        context.SaveChanges();

        // Update the property under complex type (inherited).
        ((HomeAddress)people[0].HomeAddress).FamilyName = "Microsoft";

        expectedPropertyCount = 1;
        context.SaveChanges();

        context.LoadProperty(customer, "Orders");
        // Update Navigation property
        customer.Orders.First().OrderDate = datetime;
        customer.Orders.First().OrderShelfLifes.Add(new TimeSpan(2));
        Action<WritingEntryArgs> onEntryEndingAction2 = (arg) =>
        {
            if (arg.Entry.TypeName.EndsWith("Order"))
            {
                Assert.Equal(expectedPropertyCount, arg.Entry.Properties.Count());
            }
        };
        context.Configurations.RequestPipeline.OnEntryEnding(onEntryEndingAction2);
        expectedPropertyCount = 2;
        context.SaveChanges();

        //Verify all updated property
        var people0 = context.Customers.Expand(it => it.Orders).Where((it) => it.PersonID == people[0].PersonID).Single();

        Assert.Equal(2, people0.Emails.Count);
        Assert.Equal(datetime, (people0 as Customer).Birthday);
        Assert.Equal("Redmond", people0.HomeAddress.City);
        Assert.Equal("Microsoft", (people0.HomeAddress as HomeAddress)?.FamilyName);
        Assert.Equal("98052", people0.HomeAddress.PostalCode);
        Assert.Equal(datetime, people0.Orders.First().OrderDate);
        Assert.Equal(2, people0.Orders.First().OrderShelfLifes.Count());

        context.LoadProperty(accounts[0], "MyGiftCard");

        //  Update single value navigation property .
        Assert.NotNull(accounts[0].MyGiftCard);
        accounts[0].MyGiftCard.ExperationDate = datetime;
        Action<WritingEntryArgs> onEntryEndingAction3 = (arg) =>
        {
            if (arg.Entry.TypeName.EndsWith("GiftCard"))
            {
                Assert.Equal(expectedPropertyCount, arg.Entry.Properties.Count());
            }
        };
        context.Configurations.RequestPipeline.OnEntryEnding(onEntryEndingAction3);
        expectedPropertyCount = 1;
        context.SaveChanges();

        // Update open complex type
        accounts[0].AccountInfo.LastName = "S.";
        Action<WritingEntryArgs> onEntryEndingAction4 = (arg) =>
        {
            if (arg.Entry.TypeName.EndsWith("AccountInfo"))
            {
                Assert.Equal(expectedPropertyCount, arg.Entry.Properties.Count());
                Assert.Equal("S.", (arg.Entry.Properties.Single(p => p.Name == "LastName") as ODataProperty)?.Value);
            }
        };
        context.Configurations.RequestPipeline.OnEntryEnding(onEntryEndingAction4);

        expectedPropertyCount = 3;
        context.SaveChanges();

        var account = context.Accounts.Expand("MyGiftCard").Where((it) => it.AccountID == accounts[0].AccountID).Single();
        Assert.Equal(datetime, account.MyGiftCard.ExperationDate);
        Assert.Equal("S.", account.AccountInfo.LastName);
        Assert.Equal("New", account.AccountInfo.FirstName);

        // Update property in open singleton
        publicCompany.Single().Revenue = 200;
        publicCompany.Single().Name = "MS Ltd.";
        Action<WritingEntryArgs> onEntryEndingAction5 = (arg) =>
        {
            if (arg.Entry.TypeName.EndsWith("Company"))
            {
                Assert.Equal(expectedPropertyCount, arg.Entry.Properties.Count());
            }
        };
        context.Configurations.RequestPipeline.OnEntryEnding(onEntryEndingAction5);
        expectedPropertyCount = 2;
        context.SaveChanges();

        var company = context.PublicCompany.GetValue();
        Assert.Equal(200, company.Revenue);
        Assert.Equal("MS Ltd.", company.Name);

        // Update object by update object without change => redo the PATCH all
        context.UpdateObject(people[0]);

        expectedPropertyCount = 10;
        context.SaveChanges();
    }

    // This test verifies that updating an entity without any changes results in a PATCH request
    // that includes all properties of the entity. It ensures that the request pipeline correctly
    // handles such scenarios and includes the expected number of properties in the request.
    [Fact]
    public void UpdateEntityWithoutChanges()
    {
        // Arrange
        var context = this.ContextWrapper();

        int expectedPropertyCount = 0;
        context.Configurations.RequestPipeline.OnEntryEnding(
        (arg) =>
        {
            if (arg.Entry.TypeName.EndsWith("Company"))
            {
                Assert.Equal(expectedPropertyCount, arg.Entry.Properties.Count());
            }
        });

        var publicCompany = new DataServiceCollection<Company>(context.PublicCompany);

        var account = new DataServiceCollection<Account>(context.Accounts);

        // Update object by update object without change => redo the PATCH all
        context.UpdateObject(publicCompany.Single());
        //Properties only defined by customer will also be sent.
        expectedPropertyCount = 5;
        context.SaveChanges();

        publicCompany.Single().Revenue = 200;
        expectedPropertyCount = 1;
        context.SaveChanges();

        // Update object which contains open complex type
        context.UpdateObject(account.First());
        //Properties only defined by customer will also be sent.
        expectedPropertyCount = 3;
        context.Configurations.RequestPipeline.OnEntryEnding(
        (arg) =>
        {
            if (arg.Entry.TypeName.EndsWith("AccountInfo"))
            {
                Assert.Equal(5, arg.Entry.Properties.Count());
            }
        });
        context.SaveChanges();
    }

    // This test verifies that partial properties of various entities can be updated using PATCH requests in a single batch.
    // It covers updating properties of derived types, complex types, and navigation properties.
    // The test ensures that the request pipeline correctly handles batch requests with a single changeset.
    [Fact]
    public void BatchUpdatePartialPropertiesWithCustomNames()
    {
        // Arrange
        var context = this.ContextWrapper();

        int expectedPropertyCount = 1;

        context.Configurations.RequestPipeline.OnEntryEnding(
            (arg) =>
            {
                Assert.Equal(expectedPropertyCount, arg.Entry.Properties.Count());
            });

        var orders =
            new DataServiceCollection<Order>(context.Orders.Expand("OrderDetails"));

        var people =
            new DataServiceCollection<Person>(context.People);

        var boss =
            new DataServiceCollection<Person>(context.Boss);

        orders[1].OrderDate = DateTimeOffset.Now;
        ((Customer)people[0]).City = "Redmond";
        boss.Single().FirstName = "Bill";
        orders[0].OrderDetails.First().Quantity = 1;
        context.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);
    }

    // This test verifies that full properties of various entities can be updated using PUT requests in a single batch.
    // It covers updating properties of derived types, complex types, and navigation properties.
    // The test ensures that the request pipeline correctly handles batch requests with the ReplaceOnUpdate option.
    [Fact]
    public void BatchUpdateFullPropertiesWithPutAndCustomNames()
    {
        // Arrange
        var context = this.ContextWrapper();

        int expectedPropertyCount = 0;
        var saveChangesOption = (SaveChangesOptions.ReplaceOnUpdate | SaveChangesOptions.BatchWithSingleChangeset);

        context.Configurations.RequestPipeline.OnEntryEnding(
            (arg) =>
            {
                if (arg.Entry.TypeName.EndsWith("Order")
                    || arg.Entry.TypeName.EndsWith("Customer")
                    || arg.Entry.TypeName.EndsWith("OrderDetail"))
                    Assert.Equal(expectedPropertyCount, arg.Entry.Properties.Count());
            });

        var orders =
            new DataServiceCollection<Order>(context.Orders.Expand("OrderDetails").Take(2));

        var people =
            new DataServiceCollection<Person>(context.People.Take(1));

        var boss =
            new DataServiceCollection<Person>(context.Boss);

        var dateTime = DateTimeOffset.Now;
        orders[1].OrderDate = dateTime;
        expectedPropertyCount = 7;
        context.SaveChanges(saveChangesOption);

        ((Customer)people[0]).City = "Redmond";
        boss.Single().FirstName = "Bill";
        expectedPropertyCount = 11;
        context.SaveChanges(saveChangesOption);

        orders[0].OrderDetails.First().Quantity = 1;
        expectedPropertyCount = 6;
        context.SaveChanges(saveChangesOption);

        context.MergeOption = MergeOption.OverwriteChanges;

        var orders2 = context.Orders.Expand(o => o.OrderDetails).Take(2).ToList();
        Assert.Equal(dateTime, orders2[1].OrderDate);
        Assert.Equal("Bill", context.Boss.GetValue().FirstName);
        Assert.Equal("Redmond", ((Customer)context.People.First(p => p.PersonID == people[0].PersonID)).City);
        Assert.Equal(1, orders2[0].OrderDetails.First().Quantity);
    }

    // This test verifies that POST tunneling can be used to update various properties of entities.
    // It covers updating properties of derived types, complex types, open entity types, and entities with enum properties.
    // The test ensures that the request pipeline correctly handles POST tunneling and verifies the changes.
    [Fact]
    public void UpdateEntitiesUsingPostTunneling()
    {
        // Arrange
        var context = this.ContextWrapper();

        context.MergeOption = MergeOption.OverwriteChanges;
        context.UsePostTunneling = true;

        //Entity of Derived type
        //Entity has a complex property of derived type
        var people =
            new DataServiceCollection<Person>(context, "Customers", null, null)
            {
                    new Customer()
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
                        HomeAddress = new HomeAddress()
                        {
                            City = "London",
                            PostalCode = "98052",
                            Street = "1 Microsoft Way",
                            FamilyName = "Black's Family"
                        },
                    }
            };

        var orders = new DataServiceCollection<Order>(context, "Orders", null, null)
            {
                new Order()
                {
                    OrderID = 11111111,
                    OrderDate = new DateTimeOffset(new DateTime(2011, 5, 29, 14, 21, 12)),
                    ShelfLife = new TimeSpan(1),
                    OrderShelfLifes = new ObservableCollection<TimeSpan>(){new TimeSpan(1)}
                }
            };

        //Singleton of derived type
        //Singleton is of an open entity type
        DataServiceCollection<Company> publicCompany =
            new DataServiceCollection<Company>(context.PublicCompany);

        //Entity with open complex type
        //Entity with contained Navigation
        DataServiceCollection<Account> accounts =
            new DataServiceCollection<Account>(context, "Accounts", null, null)
            {
                    new Account()
                    {
                        AccountID = 110,
                        CountryRegion = "CN",
                        AccountInfo = new AccountInfo()
                        {
                            FirstName = "New",
                            LastName = "Boy"
                        }
                    }
            };

        var gc = new GiftCard()
        {
            GiftCardID = 30000,
            GiftCardNO = "AAA123A",
            Amount = 19.9,
            ExperationDate = new DateTimeOffset(new DateTime(2013, 12, 30))
        };

        //Entity with Enum property and collection of Enum property
        DataServiceCollection<Product> products =
            new DataServiceCollection<Product>(context, "Products", null, null)
            {
                    new  Product()
                    {
                        Name = "Apple",
                        ProductID = 1000000,
                        QuantityInStock = 20,
                        QuantityPerUnit = "Pound",
                        UnitPrice = 0.35f,
                        Discontinued = false,
                        SkinColor = Color.Red,
                        CoverColors = new ObservableCollection<Color>(){ Color.Blue },
                        UserAccess = AccessLevel.Read
                    }
            };

        context.UpdateRelatedObject(accounts[0], "MyGiftCard", gc);
        context.SaveChanges();

        var customer = context.Customers.Where(c => c.PersonID == people[0].PersonID).First();
        context.AddLink(customer, "Orders", orders[0]);
        context.SaveChanges();

        bool isEntity = true;
        int expectedPropertyCount = 0;
        Action<WritingEntryArgs> onEntryEndingAction = (arg) =>
        {
            if (isEntity)
            {
                Assert.Equal(expectedPropertyCount, arg.Entry.Properties.Count());
            }
        };
        context.Configurations.RequestPipeline.OnEntryEnding(onEntryEndingAction);

        //Update Enum type and collection of enum property in entity
        products[0].CoverColors.Add(Color.Green);
        products[0].UserAccess = AccessLevel.Execute;
        products[0].SkinColor = Color.Green;
        expectedPropertyCount = 3;
        context.SaveChanges();

        //context.Detach(products[0]);
        var product = context.Products.Where((it) => it.ProductID == products[0].ProductID).First();
        Assert.Equal(2, product.CoverColors.Count);
        Assert.Equal(Color.Green.ToString(), product.SkinColor.ToString());
        Assert.Equal(AccessLevel.Execute.ToString(), product.UserAccess.ToString());
        Assert.Equal(2, product.CoverColors.Count);

        // Update primitive type and collection property under entity
        people[0].FirstName = "Black";
        people[0].Emails.Add("test123@var1.com");

        expectedPropertyCount = 2;
        context.SaveChanges();

        // Update primitive type and collection property under entity (inherited)
        var datetime = new DateTimeOffset(new DateTime(1957, 4, 3)); ;
        ((Customer)people[0]).Birthday = new DateTimeOffset(new DateTime(1957, 4, 3));

        expectedPropertyCount = 1;
        context.SaveChanges();

        // Update the property under complex type.
        people[0].HomeAddress.City = "Redmond";

        bool isComplex = true;
        isEntity = false;
        Action<WritingEntryArgs> onEntryEndingAction1 = (arg) =>
        {
            if (isComplex && arg.Entry.TypeName.EndsWith("HomeAddress"))
            {
                Assert.Equal("Redmond", (arg.Entry.Properties.Single(p => p.Name == "City") as ODataProperty)?.Value);
            }
        };

        context.Configurations.RequestPipeline.OnEntryEnding(onEntryEndingAction1);
        context.SaveChanges();

        // Update the property under complex type (inherited).
        ((HomeAddress)people[0].HomeAddress).FamilyName = "Microsoft";

        Action<WritingEntryArgs> onEntryEndingAction2 = (arg) =>
        {
            if (isComplex && arg.Entry.TypeName.EndsWith("HomeAddress"))
            {
                Assert.Equal("Microsoft", (arg.Entry.Properties.Single(p => p.Name == "FamilyName") as ODataProperty)?.Value);
            }
        };

        context.Configurations.RequestPipeline.OnEntryEnding(onEntryEndingAction2);
        context.SaveChanges();

        isComplex = false;
        isEntity = true;

        context.LoadProperty(customer, "Orders");
        // Update Navigation property
        customer.Orders.First().OrderDate = datetime;
        customer.Orders.First().OrderShelfLifes.Add(new TimeSpan(2));
        expectedPropertyCount = 2;
        context.SaveChanges();

        //Verify all updated property
        //context.Detach(customer);
        var people0 = context.Customers.Expand(it => it.Orders).Where((it) => it.PersonID == people[0].PersonID).Single();

        Assert.Equal(2, people0.Emails.Count);
        Assert.Equal(datetime, (people0 as Customer).Birthday);
        Assert.Equal("Redmond", people0.HomeAddress.City);
        Assert.Equal("Microsoft", (people0.HomeAddress as HomeAddress)?.FamilyName);
        Assert.Equal("98052", people0.HomeAddress.PostalCode);
        Assert.Equal(datetime, people0.Orders.First().OrderDate);
        Assert.Equal(2, people0.Orders.First().OrderShelfLifes.Count());

        context.LoadProperty(accounts[0], "MyGiftCard");
        //  Update single vlue navigation property .
        accounts[0].MyGiftCard.ExperationDate = datetime;

        expectedPropertyCount = 1;
        context.SaveChanges();

        // Update open complex type
        accounts[0].AccountInfo.LastName = "S.";
        isEntity = false;
        isComplex = true;
        Action<WritingEntryArgs> onEntryEndingAction3 = (arg) =>
        {
            if (isComplex && arg.Entry.TypeName.EndsWith("AccountInfo"))
            {
                Assert.Equal("S.", (arg.Entry.Properties.Single(p => p.Name == "LastName") as ODataProperty)?.Value);
            }
        };
        context.Configurations.RequestPipeline.OnEntryEnding(onEntryEndingAction3);
        context.SaveChanges();
        isComplex = false;
        isEntity = true;

        //context.Detach(accounts[0]);
        var account = context.Accounts.Expand("MyGiftCard").Where((it) => it.AccountID == accounts[0].AccountID).Single();
        Assert.Equal(datetime, account.MyGiftCard.ExperationDate);
        Assert.Equal("S.", account.AccountInfo.LastName);
        Assert.Equal("New", account.AccountInfo.FirstName);

        // Update property in open singleton
        publicCompany.Single().Revenue = 10;
        publicCompany.Single().Name = "MS Ltd.";
        expectedPropertyCount = 2;
        context.SaveChanges();

        //context.Detach(publicCompany);
        var company = context.PublicCompany.GetValue();
        Assert.Equal(10, company.Revenue);
        Assert.Equal("MS Ltd.", company.Name);

        // Update object by update object without change => redo the PATCH all
        context.UpdateObject(people[0]);

        isEntity = false;
        Action<WritingEntryArgs> onEntryEndingAction4 = (arg) =>
        {
            if (arg.Entry.TypeName.EndsWith("Person"))
            {
                Assert.Equal(expectedPropertyCount, arg.Entry.Properties.Count());
            }
        };
        context.Configurations.RequestPipeline.OnEntryEnding(onEntryEndingAction4);

        expectedPropertyCount = 10;
        context.SaveChanges();
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

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "propertytrackingtests/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    private void ResetDefaultDataSource(Container context)
    {
        var actionUri = new Uri(_baseUri + "propertytrackingtests/Default.ResetDefaultDataSource", UriKind.Absolute);
        context.Execute(actionUri, "POST");
    }

    #endregion
}

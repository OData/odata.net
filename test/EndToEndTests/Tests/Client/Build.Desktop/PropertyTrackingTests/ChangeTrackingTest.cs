//---------------------------------------------------------------------
// <copyright file="ChangeTrackingTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.CodeGenerationTests
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.OData.Client;
    using Microsoft.OData.Core;
    using Microsoft.Spatial;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference;
    using Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReferencePlus;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ChangeTrackingTest : ODataWCFServiceTestsBase<InMemoryEntities>
    {
        public ChangeTrackingTest()
            : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {
        }

        [TestMethod]
        public void UpdateByPatchPartialProperties()
        {
            int expectedPropertyCount = 0;

            this.TestClientContext.Configurations.RequestPipeline.OnEntryEnding(
                (arg) =>
                {
                    Assert.AreEqual(expectedPropertyCount, arg.Entry.Properties.Count());
                });

            DataServiceCollection<Order> orders =
                new DataServiceCollection<Order>(this.TestClientContext.Orders.Expand("OrderDetails"));

            DataServiceCollection<Person> people =
                new DataServiceCollection<Person>(this.TestClientContext.People);

            DataServiceCollection<Person> boss =
                new DataServiceCollection<Person>(this.TestClientContext.Boss);

            DataServiceCollection<Account> accounts =
                new DataServiceCollection<Account>(this.TestClientContext.Accounts.Expand("MyGiftCard"));

            // Update primitive type and collection property under entity
            orders[1].OrderDate = DateTimeOffset.Now;
            orders[1].OrderShelfLifes.Add(TimeSpan.FromHours(1.2));

            expectedPropertyCount = 2;
            this.TestClientContext.SaveChanges();

            Assert.AreEqual(
                2,
                this.TestClientContext.Orders.Where((it) => it.OrderID == orders[1].OrderID).Single().OrderShelfLifes.Count);

            // Update primitive type under entity (inherited)
            ((Customer)people[0]).City = "Redmond";

            expectedPropertyCount = 1;
            this.TestClientContext.SaveChanges();

            Assert.AreEqual(
                "Redmond",
                (this.TestClientContext.People.Where((it) => it.PersonID == people[0].PersonID).Single() as Customer).City);

            // Update the property under complex type.
            people[0].HomeAddress.City = "Redmond";

            expectedPropertyCount = 1;
            this.TestClientContext.SaveChanges();

            Assert.AreEqual(
                "Redmond",
                this.TestClientContext.People.Where((it) => it.PersonID == people[0].PersonID).Single().HomeAddress.City);
            Assert.AreEqual(
                "98052",
                this.TestClientContext.People.Where((it) => it.PersonID == people[0].PersonID).Single().HomeAddress.PostalCode);

            // Update the property under complex type (inherited).
            ((HomeAddress)people[0].HomeAddress).FamilyName = "Microsoft";

            expectedPropertyCount = 1;
            this.TestClientContext.SaveChanges();

            Assert.AreEqual(
                "Microsoft",
                (this.TestClientContext.People.Where((it) => it.PersonID == people[0].PersonID).Single().HomeAddress as HomeAddress).FamilyName);

            // Update collection navigation property .
            orders[0].OrderDetails.First().Quantity = 1;

            expectedPropertyCount = 1;
            this.TestClientContext.SaveChanges();

            Assert.AreEqual(
                1,
                this.TestClientContext.OrderDetails.Where(
                    (it) => it.ProductID == orders[0].OrderDetails.First().ProductID && it.OrderID == orders[0].OrderDetails.First().OrderID
                    ).Single().Quantity);

            //  Update single vlue navigation property .
            accounts[0].MyGiftCard.ExperationDate = DateTimeOffset.Now;

            expectedPropertyCount = 1;
            this.TestClientContext.SaveChanges();

            Assert.AreEqual(
                accounts[0].MyGiftCard.ExperationDate,
                this.TestClientContext.Accounts.Expand("MyGiftCard").Where((it) => it.AccountID == accounts[0].AccountID).Single().MyGiftCard.ExperationDate);

            // Update property in singleton.
            boss.Single().FirstName = "Bill";

            expectedPropertyCount = 1;
            this.TestClientContext.SaveChanges();

            Assert.AreEqual(
                "Bill",
                this.TestClientContext.Boss.GetValue().FirstName);

            // Update object by update object without change => redo the PATCH all
            this.TestClientContext.UpdateObject(people[0]);

            expectedPropertyCount = 12;
            this.TestClientContext.SaveChanges();
        }

        [TestMethod]
        public void UpdateByPutFullProperties()
        {
            int expectedPropertyCount = 0;

            this.TestClientContext.Configurations.RequestPipeline.OnEntryEnding(
                (arg) =>
                {
                    Assert.AreEqual(expectedPropertyCount, arg.Entry.Properties.Count());
                });

            DataServiceCollection<Order> orders =
                new DataServiceCollection<Order>(this.TestClientContext.Orders.Expand("OrderDetails"));

            // Update primitive type and collection property under entity
            orders[1].OrderDate = DateTimeOffset.Now;
            orders[1].OrderShelfLifes.Add(TimeSpan.FromHours(1.2));

            expectedPropertyCount = 6;
            this.TestClientContext.SaveChanges(SaveChangesOptions.ReplaceOnUpdate);
        }

        [TestMethod]
        public void UpdateByPatchPartialPropertieInBatch()
        {
            int expectedPropertyCount = 1;

            this.TestClientContext.Configurations.RequestPipeline.OnEntryEnding(
                (arg) =>
                {
                    Assert.AreEqual(expectedPropertyCount, arg.Entry.Properties.Count());
                });

            DataServiceCollection<Order> orders =
                new DataServiceCollection<Order>(this.TestClientContext.Orders.Expand("OrderDetails"));

            DataServiceCollection<Person> people =
                new DataServiceCollection<Person>(this.TestClientContext.People);

            DataServiceCollection<Person> boss =
                new DataServiceCollection<Person>(this.TestClientContext.Boss);

            orders[1].OrderDate = DateTimeOffset.Now;

            ((Customer)people[0]).City = "Redmond";

            boss.Single().FirstName = "Bill";

            orders[0].OrderDetails.First().Quantity = 1;

            this.TestClientContext.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);
        }
    }

    [TestClass]
    public class ChangeTrackingTest2 : ODataWCFServiceTestsBase<InMemoryEntitiesPlus>
    {
        public ChangeTrackingTest2()
            : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {
        }

        [TestMethod]
        public void UpdateByPartialPropertiesWithCustomizedName()
        {
            this.TestClientContext.MergeOption = MergeOption.OverwriteChanges;
            //Entity of Derived type
            //Entity has a complex property of derived type
            DataServiceCollection<PersonPlus> people =
                new DataServiceCollection<PersonPlus>(this.TestClientContext, "People", null, null)
                {
                    new CustomerPlus()
                    {
                        FirstNamePlus = "Nelson",
                        MiddleNamePlus = "S.",
                        LastNamePlus = "Black",
                        NumbersPlus = new ObservableCollection<string> { "111-111-1111" },
                        EmailsPlus = new ObservableCollection<string> { "abc@abc.com" },
                        PersonIDPlus = 10001,
                        BirthdayPlus = new DateTimeOffset(new DateTime(1957, 4, 3)),
                        CityPlus = "London",
                        HomePlus = GeographyPoint.Create(32.1, 23.1),
                        TimeBetweenLastTwoOrdersPlus = new TimeSpan(1),
                        HomeAddressPlus = new HomeAddressPlus()
                        {
                            CityPlus = "London",
                            PostalCodePlus = "98052",
                            StreetPlus = "1 Microsoft Way",
                            FamilyNamePlus = "Black's Family"
                        },
                    }
                };

            DataServiceCollection<OrderPlus> orders = new DataServiceCollection<OrderPlus>(TestClientContext)
            {
                new OrderPlus()
                {
                    OrderIDPlus = 11111111,
                    OrderDatePlus = new DateTimeOffset(new DateTime(2011, 5, 29, 14, 21, 12)),
                    ShelfLifePlus = new TimeSpan(1),
                    OrderShelfLifesPlus = new ObservableCollection<TimeSpan>(){new TimeSpan(1)}
                }
            };

            //Singleton of derived type
            //Singleton is of an open entity type
            DataServiceCollection<CompanyPlus> publicCompany =
                new DataServiceCollection<CompanyPlus>(this.TestClientContext.PublicCompanyPlus);

            //Entity with open complex type
            //Entity with contained Navigation
            DataServiceCollection<AccountPlus> accounts =
                new DataServiceCollection<AccountPlus>(this.TestClientContext)
                {
                    new AccountPlus()
                    {
                        AccountIDPlus = 110,
                        CountryRegionPlus = "CN",
                        AccountInfoPlus = new AccountInfoPlus()
                        {
                            FirstNamePlus = "New",
                            LastNamePlus = "Boy"
                        }
                    }
                };

            var gc = new GiftCardPlus()
            {
                GiftCardIDPlus = 30000,
                GiftCardNOPlus = "AAA123A",
                AmountPlus = 19.9,
                ExperationDatePlus = new DateTimeOffset(new DateTime(2013, 12, 30))
            };

            //Entity with Enum property and collection of Enum property
            DataServiceCollection<ProductPlus> products =
                new DataServiceCollection<ProductPlus>(this.TestClientContext)
                {
                    new  ProductPlus()
                    {
                        NamePlus = "Apple",
                        ProductIDPlus = 1000000,
                        QuantityInStockPlus = 20,
                        QuantityPerUnitPlus = "Pound",
                        UnitPricePlus = 0.35f,
                        DiscontinuedPlus = false,
                        SkinColorPlus = ColorPlus.RedPlus,
                        CoverColorsPlus = new ObservableCollection<ColorPlus>(){ ColorPlus.BluePlus },
                        UserAccessPlus = AccessLevelPlus.ReadPlus
                    }
                };

            TestClientContext.UpdateRelatedObject(accounts[0], "MyGiftCard", gc);
            this.TestClientContext.SaveChanges();

            var customer = this.TestClientContext.CustomersPlus.Where(c => c.PersonIDPlus == people[0].PersonIDPlus).First();
            TestClientContext.AddLink(customer, "Orders", orders[0]);
            this.TestClientContext.SaveChanges();

            int expectedPropertyCount = 0;
            Action<WritingEntryArgs> onEntryEndingAction = (arg) =>
            {
                Assert.AreEqual(expectedPropertyCount, arg.Entry.Properties.Count());
            };
            this.TestClientContext.Configurations.RequestPipeline.OnEntryEnding(onEntryEndingAction);

            //Update Enum type and collection of enum property in entity
            products[0].CoverColorsPlus.Add(ColorPlus.GreenPlus);
            products[0].UserAccessPlus = AccessLevelPlus.ExecutePlus;
            products[0].SkinColorPlus = ColorPlus.GreenPlus;
            expectedPropertyCount = 3;
            this.TestClientContext.SaveChanges();

            var product = this.TestClientContext.ProductsPlus.Where((it) => it.ProductIDPlus == products[0].ProductIDPlus).First();
            Assert.AreEqual(2, product.CoverColorsPlus.Count);
            Assert.AreEqual(ColorPlus.GreenPlus, product.SkinColorPlus);
            Assert.AreEqual(AccessLevelPlus.ExecutePlus, product.UserAccessPlus);
            Assert.AreEqual(2, product.CoverColorsPlus.Count);

            // Update primitive type and collection property under entity
            people[0].FirstNamePlus = "Balck";
            people[0].EmailsPlus.Add("test123@var1.com");

            expectedPropertyCount = 2;
            this.TestClientContext.SaveChanges();

            // Update primitive type and collection property under entity (inherited)
            var datetime = new DateTimeOffset(new DateTime(1957, 4, 3)); ;
            ((CustomerPlus)people[0]).BirthdayPlus = new DateTimeOffset(new DateTime(1957, 4, 3));

            expectedPropertyCount = 1;
            this.TestClientContext.SaveChanges();

            // Update the property under complex type.
            people[0].HomeAddressPlus.CityPlus = "Redmond";

            expectedPropertyCount = 1;
            this.TestClientContext.SaveChanges();

            // Update the property under complex type (inherited).
            ((HomeAddressPlus)people[0].HomeAddressPlus).FamilyNamePlus = "Microsoft";

            expectedPropertyCount = 1;
            this.TestClientContext.SaveChanges();

            this.TestClientContext.LoadProperty(customer, "Orders");
            // Update Navigation property
            customer.OrdersPlus.First().OrderDatePlus = datetime;
            customer.OrdersPlus.First().OrderShelfLifesPlus.Add(new TimeSpan(2));
            expectedPropertyCount = 2;
            this.TestClientContext.SaveChanges();

            //Verify all updated property
            var people0 = this.TestClientContext.CustomersPlus.Expand(it => it.OrdersPlus).Where((it) => it.PersonIDPlus == people[0].PersonIDPlus).Single();

            Assert.AreEqual(2, people0.EmailsPlus.Count);
            Assert.AreEqual(datetime, (people0 as CustomerPlus).BirthdayPlus);
            Assert.AreEqual("Redmond", people0.HomeAddressPlus.CityPlus);
            Assert.AreEqual("Microsoft", (people0.HomeAddressPlus as HomeAddressPlus).FamilyNamePlus);
            Assert.AreEqual("98052", people0.HomeAddressPlus.PostalCodePlus);
            Assert.AreEqual(datetime, people0.OrdersPlus.First().OrderDatePlus);
            Assert.AreEqual(2, people0.OrdersPlus.First().OrderShelfLifesPlus.Count());

            TestClientContext.LoadProperty(accounts[0], "MyGiftCard");
            //  Update single vlue navigation property .            
            accounts[0].MyGiftCardPlus.ExperationDatePlus = datetime;

            expectedPropertyCount = 1;
            this.TestClientContext.SaveChanges();

            // Update open complex type
            accounts[0].AccountInfoPlus.MiddleNamePlus = "S.";
            expectedPropertyCount = 1;
            this.TestClientContext.SaveChanges();

            var account = this.TestClientContext.AccountsPlus.Expand("MyGiftCard").Where((it) => it.AccountIDPlus == accounts[0].AccountIDPlus).Single();
            Assert.AreEqual(datetime, account.MyGiftCardPlus.ExperationDatePlus);
            Assert.AreEqual("S.", account.AccountInfoPlus.MiddleNamePlus);
            Assert.AreEqual("New", account.AccountInfoPlus.FirstNamePlus);

            // Update property in open singleton
            publicCompany.Single().TotalAssetsPlus = 10;
            publicCompany.Single().FullNamePlus = "MS Ltd.";
            expectedPropertyCount = 2;
            this.TestClientContext.SaveChanges();

            var company = this.TestClientContext.PublicCompanyPlus.GetValue();
            Assert.AreEqual(company.TotalAssetsPlus, 10);
            Assert.AreEqual(company.FullNamePlus, "MS Ltd.");

            // Update object by update object without change => redo the PATCH all
            this.TestClientContext.UpdateObject(people[0]);

            expectedPropertyCount = 11;
            this.TestClientContext.SaveChanges();
        }

        [TestMethod]
        public void UpdateObjectWithoutChange()
        {
            int expectedPropertyCount = 0;
            this.TestClientContext.Configurations.RequestPipeline.OnEntryEnding(
            (arg) =>
            {
                Assert.AreEqual(expectedPropertyCount, arg.Entry.Properties.Count());
            });

            DataServiceCollection<CompanyPlus> publicCompany =
                new DataServiceCollection<CompanyPlus>(this.TestClientContext.PublicCompanyPlus);

            DataServiceCollection<AccountPlus> account =
               new DataServiceCollection<AccountPlus>(this.TestClientContext.AccountsPlus);

            // Update object by update object without change => redo the PATCH all
            this.TestClientContext.UpdateObject(publicCompany.Single());
            //Properties only defnied by customer will also be sent.
            expectedPropertyCount = 8;
            this.TestClientContext.SaveChanges();

            publicCompany.Single().RevenuePlus = 200;
            expectedPropertyCount = 1;
            this.TestClientContext.SaveChanges();

            // Update object which contains open complex type
            this.TestClientContext.UpdateObject(account.First());
            //Properties only defnied by customer will also be sent.
            expectedPropertyCount = 3;
            this.TestClientContext.Configurations.RequestPipeline.OnEntryEnding(
            (arg) =>
            {
                Assert.AreEqual(4, (arg.Entry.Properties.Where(p => p.Name == "AccountInfo").Single().Value as ODataComplexValue).Properties.Count());
            });
            this.TestClientContext.SaveChanges();
        }

        [TestMethod]
        public void UpdateByPatchPartialPropertieInBatchWithCustomizedName()
        {
            int expectedPropertyCount = 1;

            this.TestClientContext.Configurations.RequestPipeline.OnEntryEnding(
                (arg) =>
                {
                    Assert.AreEqual(expectedPropertyCount, arg.Entry.Properties.Count());
                });

            DataServiceCollection<OrderPlus> orders =
                new DataServiceCollection<OrderPlus>(this.TestClientContext.OrdersPlus.Expand("OrderDetails"));

            DataServiceCollection<PersonPlus> people =
                new DataServiceCollection<PersonPlus>(this.TestClientContext.PeoplePlus);

            DataServiceCollection<PersonPlus> boss =
                new DataServiceCollection<PersonPlus>(this.TestClientContext.BossPlus);

            orders[1].OrderDatePlus = DateTimeOffset.Now;

            ((CustomerPlus)people[0]).CityPlus = "Redmond";

            boss.Single().FirstNamePlus = "Bill";

            orders[0].OrderDetailsPlus.First().QuantityPlus = 1;

            this.TestClientContext.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);
        }

        [TestMethod]
        public void UpdateByPutWithFullPropertiesInBatchWithCustomizedName()
        {
            int expectedPropertyCount = 0;
            var saveChangesOption = (SaveChangesOptions.ReplaceOnUpdate | SaveChangesOptions.BatchWithSingleChangeset);

            this.TestClientContext.Configurations.RequestPipeline.OnEntryEnding(
                (arg) =>
                {
                    Assert.AreEqual(expectedPropertyCount, arg.Entry.Properties.Count());
                });

            DataServiceCollection<OrderPlus> orders =
                new DataServiceCollection<OrderPlus>(this.TestClientContext.OrdersPlus.Expand("OrderDetails").Take(2));

            DataServiceCollection<PersonPlus> people =
                new DataServiceCollection<PersonPlus>(this.TestClientContext.PeoplePlus.Take(1));

            DataServiceCollection<PersonPlus> boss =
                new DataServiceCollection<PersonPlus>(this.TestClientContext.BossPlus);

            var dateTime = DateTimeOffset.Now;
            orders[1].OrderDatePlus = dateTime;
            expectedPropertyCount = 6;
            this.TestClientContext.SaveChanges(saveChangesOption);

            ((CustomerPlus)people[0]).CityPlus = "Redmond";

            boss.Single().FirstNamePlus = "Bill";

            expectedPropertyCount = 11;
            this.TestClientContext.SaveChanges(saveChangesOption);

            orders[0].OrderDetailsPlus.First().QuantityPlus = 1;
            expectedPropertyCount = 5;
            this.TestClientContext.SaveChanges(saveChangesOption);

            this.TestClientContext.MergeOption = MergeOption.OverwriteChanges;

            var orders2 = this.TestClientContext.OrdersPlus.Expand(o => o.OrderDetailsPlus).Take(2).ToList();
            Assert.AreEqual(dateTime, orders2[1].OrderDatePlus);
            Assert.AreEqual("Redmond", ((CustomerPlus)this.TestClientContext.PeoplePlus.First()).CityPlus);
            Assert.AreEqual("Bill", this.TestClientContext.BossPlus.GetValue().FirstNamePlus);
            Assert.AreEqual(1, orders2[0].OrderDetailsPlus.First().QuantityPlus);
        }

        [TestMethod]
        public void PostTunning()
        {
            this.TestClientContext.MergeOption = MergeOption.OverwriteChanges;
            this.TestClientContext.UsePostTunneling = true;
            //Entity of Derived type
            //Entity has a complex property of derived type
            DataServiceCollection<PersonPlus> people =
                new DataServiceCollection<PersonPlus>(this.TestClientContext, "People", null, null)
                {
                    new CustomerPlus()
                    {
                        FirstNamePlus = "Nelson",
                        MiddleNamePlus = "S.",
                        LastNamePlus = "Black",
                        NumbersPlus = new ObservableCollection<string> { "111-111-1111" },
                        EmailsPlus = new ObservableCollection<string> { "abc@abc.com" },
                        PersonIDPlus = 10001,
                        BirthdayPlus = new DateTimeOffset(new DateTime(1957, 4, 3)),
                        CityPlus = "London",
                        HomePlus = GeographyPoint.Create(32.1, 23.1),
                        TimeBetweenLastTwoOrdersPlus = new TimeSpan(1),
                        HomeAddressPlus = new HomeAddressPlus()
                        {
                            CityPlus = "London",
                            PostalCodePlus = "98052",
                            StreetPlus = "1 Microsoft Way",
                            FamilyNamePlus = "Black's Family"
                        },
                    }
                };

            DataServiceCollection<OrderPlus> orders = new DataServiceCollection<OrderPlus>(TestClientContext)
            {
                new OrderPlus()
                {
                    OrderIDPlus = 11111111,
                    OrderDatePlus = new DateTimeOffset(new DateTime(2011, 5, 29, 14, 21, 12)),
                    ShelfLifePlus = new TimeSpan(1),
                    OrderShelfLifesPlus = new ObservableCollection<TimeSpan>(){new TimeSpan(1)}
                }
            };

            //Singleton of derived type
            //Singleton is of an open entity type
            DataServiceCollection<CompanyPlus> publicCompany =
                new DataServiceCollection<CompanyPlus>(this.TestClientContext.PublicCompanyPlus);

            //Entity with open complex type
            //Entity with contained Navigation
            DataServiceCollection<AccountPlus> accounts =
                new DataServiceCollection<AccountPlus>(this.TestClientContext)
                {
                    new AccountPlus()
                    {
                        AccountIDPlus = 110,
                        CountryRegionPlus = "CN",
                        AccountInfoPlus = new AccountInfoPlus()
                        {
                            FirstNamePlus = "New",
                            LastNamePlus = "Boy"
                        }
                    }
                };

            var gc = new GiftCardPlus()
            {
                GiftCardIDPlus = 30000,
                GiftCardNOPlus = "AAA123A",
                AmountPlus = 19.9,
                ExperationDatePlus = new DateTimeOffset(new DateTime(2013, 12, 30))
            };

            //Entity with Enum property and collection of Enum property
            DataServiceCollection<ProductPlus> products =
                new DataServiceCollection<ProductPlus>(this.TestClientContext)
                {
                    new  ProductPlus()
                    {
                        NamePlus = "Apple",
                        ProductIDPlus = 1000000,
                        QuantityInStockPlus = 20,
                        QuantityPerUnitPlus = "Pound",
                        UnitPricePlus = 0.35f,
                        DiscontinuedPlus = false,
                        SkinColorPlus = ColorPlus.RedPlus,
                        CoverColorsPlus = new ObservableCollection<ColorPlus>(){ ColorPlus.BluePlus },
                        UserAccessPlus = AccessLevelPlus.ReadPlus
                    }
                };

            TestClientContext.UpdateRelatedObject(accounts[0], "MyGiftCard", gc);
            this.TestClientContext.SaveChanges();

            var customer = this.TestClientContext.CustomersPlus.Where(c => c.PersonIDPlus == people[0].PersonIDPlus).First();
            TestClientContext.AddLink(customer, "Orders", orders[0]);
            this.TestClientContext.SaveChanges();

            int expectedPropertyCount = 0;
            Action<WritingEntryArgs> onEntryEndingAction = (arg) =>
            {
                Assert.AreEqual(expectedPropertyCount, arg.Entry.Properties.Count());
            };
            this.TestClientContext.Configurations.RequestPipeline.OnEntryEnding(onEntryEndingAction);

            //Update Enum type and collection of enum property in entity
            products[0].CoverColorsPlus.Add(ColorPlus.GreenPlus);
            products[0].UserAccessPlus = AccessLevelPlus.ExecutePlus;
            products[0].SkinColorPlus = ColorPlus.GreenPlus;
            expectedPropertyCount = 3;
            this.TestClientContext.SaveChanges();

            //this.TestClientContext.Detach(products[0]);
            var product = this.TestClientContext.ProductsPlus.Where((it) => it.ProductIDPlus == products[0].ProductIDPlus).First();
            Assert.AreEqual(2, product.CoverColorsPlus.Count);
            Assert.AreEqual(ColorPlus.GreenPlus, product.SkinColorPlus);
            Assert.AreEqual(AccessLevelPlus.ExecutePlus, product.UserAccessPlus);
            Assert.AreEqual(2, product.CoverColorsPlus.Count);

            // Update primitive type and collection property under entity
            people[0].FirstNamePlus = "Balck";
            people[0].EmailsPlus.Add("test123@var1.com");

            expectedPropertyCount = 2;
            this.TestClientContext.SaveChanges();

            // Update primitive type and collection property under entity (inherited)
            var datetime = new DateTimeOffset(new DateTime(1957, 4, 3)); ;
            ((CustomerPlus)people[0]).BirthdayPlus = new DateTimeOffset(new DateTime(1957, 4, 3));

            expectedPropertyCount = 1;
            this.TestClientContext.SaveChanges();

            // Update the property under complex type.
            people[0].HomeAddressPlus.CityPlus = "Redmond";

            expectedPropertyCount = 1;
            this.TestClientContext.SaveChanges();

            // Update the property under complex type (inherited).
            ((HomeAddressPlus)people[0].HomeAddressPlus).FamilyNamePlus = "Microsoft";

            expectedPropertyCount = 1;
            this.TestClientContext.SaveChanges();

            this.TestClientContext.LoadProperty(customer, "Orders");
            // Update Navigation property
            customer.OrdersPlus.First().OrderDatePlus = datetime;
            customer.OrdersPlus.First().OrderShelfLifesPlus.Add(new TimeSpan(2));
            expectedPropertyCount = 2;
            this.TestClientContext.SaveChanges();

            //Verify all updated property
            //this.TestClientContext.Detach(customer);
            var people0 = this.TestClientContext.CustomersPlus.Expand(it => it.OrdersPlus).Where((it) => it.PersonIDPlus == people[0].PersonIDPlus).Single();

            Assert.AreEqual(2, people0.EmailsPlus.Count);
            Assert.AreEqual(datetime, (people0 as CustomerPlus).BirthdayPlus);
            Assert.AreEqual("Redmond", people0.HomeAddressPlus.CityPlus);
            Assert.AreEqual("Microsoft", (people0.HomeAddressPlus as HomeAddressPlus).FamilyNamePlus);
            Assert.AreEqual("98052", people0.HomeAddressPlus.PostalCodePlus);
            Assert.AreEqual(datetime, people0.OrdersPlus.First().OrderDatePlus);
            Assert.AreEqual(2, people0.OrdersPlus.First().OrderShelfLifesPlus.Count());

            TestClientContext.LoadProperty(accounts[0], "MyGiftCard");
            //  Update single vlue navigation property .            
            accounts[0].MyGiftCardPlus.ExperationDatePlus = datetime;

            expectedPropertyCount = 1;
            this.TestClientContext.SaveChanges();

            // Update open complex type
            accounts[0].AccountInfoPlus.MiddleNamePlus = "S.";
            expectedPropertyCount = 1;
            this.TestClientContext.SaveChanges();

            //this.TestClientContext.Detach(accounts[0]);
            var account = this.TestClientContext.AccountsPlus.Expand("MyGiftCard").Where((it) => it.AccountIDPlus == accounts[0].AccountIDPlus).Single();
            Assert.AreEqual(datetime, account.MyGiftCardPlus.ExperationDatePlus);
            Assert.AreEqual("S.", account.AccountInfoPlus.MiddleNamePlus);
            Assert.AreEqual("New", account.AccountInfoPlus.FirstNamePlus);

            // Update property in open singleton
            publicCompany.Single().TotalAssetsPlus = 10;
            publicCompany.Single().FullNamePlus = "MS Ltd.";
            expectedPropertyCount = 2;
            this.TestClientContext.SaveChanges();

            //this.TestClientContext.Detach(publicCompany);
            var company = this.TestClientContext.PublicCompanyPlus.GetValue();
            Assert.AreEqual(company.TotalAssetsPlus, 10);
            Assert.AreEqual(company.FullNamePlus, "MS Ltd.");

            // Update object by update object without change => redo the PATCH all
            this.TestClientContext.UpdateObject(people[0]);

            expectedPropertyCount = 11;
            this.TestClientContext.SaveChanges();
        }
    }
}

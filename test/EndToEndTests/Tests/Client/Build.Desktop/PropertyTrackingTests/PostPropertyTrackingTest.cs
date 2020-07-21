//---------------------------------------------------------------------
// <copyright file="PostPropertyTrackingTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.PropertyTrackingTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.OData.Client;
    using Microsoft.OData;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReferencePlus;
    using Xunit;

    public class PostPropertyTrackingTest : ODataWCFServiceTestsBase<InMemoryEntitiesPlus>
    {
        public PostPropertyTrackingTest()
            : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {
        }

        [Fact]
        public void PostPartialProperties()
        {
            this.TestClientContext.MergeOption = MergeOption.OverwriteChanges;

            int expectedPropertyCount = 0;
            ODataResource address = null;
            ODataResource accountInfo = null;
            this.TestClientContext.Configurations.RequestPipeline.OnEntryEnding((arg) =>
            {
                if (arg.Entry.TypeName.EndsWith("HomeAddressPlus"))
                {
                    address = arg.Entry;
                }

                if (arg.Entry.TypeName.EndsWith("AccountInfoPlus"))
                {
                    accountInfo = arg.Entry;
                }

                if (arg.Entry.TypeName.EndsWith("CustomerPlus"))
                {
                    Assert.Equal(expectedPropertyCount, arg.Entry.Properties.Count());
                }
            });

            //Entity of Derived type
            //Entity has a complex property of derived type
            DataServiceCollection<CustomerPlus> people =
                new DataServiceCollection<CustomerPlus>(this.TestClientContext, "People", null, null);
            var person = new CustomerPlus();
            people.Add(person);

            person.FirstNamePlus = "Nelson";
            person.LastNamePlus = "Black";
            person.NumbersPlus = new ObservableCollection<string> { };
            person.EmailsPlus = new ObservableCollection<string> { "abc@abc.com" };
            person.PersonIDPlus = 10001;
            person.CityPlus = "London";
            person.TimeBetweenLastTwoOrdersPlus = new TimeSpan(1);
            person.HomeAddressPlus = new HomeAddressPlus()
            {
                CityPlus = "Redmond",
                PostalCodePlus = "98052",
                StreetPlus = "1 Microsoft Way"
            };

            //Post entity into an entity set
            expectedPropertyCount = 7;
            this.TestClientContext.SaveChanges(SaveChangesOptions.PostOnlySetProperties);

            //Validate that entire complex type was sent to server side.
            Assert.Equal(4, address.Properties.Count());

            //Post Navigation property to an entitySet
            var people1 = new DataServiceCollection<CustomerPlus>(this.TestClientContext.CustomersPlus.Where(p => p.PersonIDPlus == 10001));
            OrderPlus order = new OrderPlus();
            people1[0].OrdersPlus.Add(order);
            order.OrderIDPlus = 10001;
            order.OrderDatePlus = new DateTimeOffset(new DateTime(2011, 5, 29, 14, 21, 12));
            order.OrderShelfLifesPlus = new ObservableCollection<TimeSpan>();

            expectedPropertyCount = 3;
            this.TestClientContext.SaveChanges(SaveChangesOptions.PostOnlySetProperties);

            //Validate
            var customer = this.TestClientContext.CustomersPlus.Expand(c => c.OrdersPlus).Where(p => p.PersonIDPlus == 10001).FirstOrDefault();
            Assert.NotNull(customer);
            Assert.Equal("London", ((CustomerPlus)customer).CityPlus);
            Assert.Equal(0, customer.NumbersPlus.Count);
            Assert.Null(((HomeAddressPlus)customer.HomeAddressPlus).FamilyNamePlus);
            Assert.Null(customer.MiddleNamePlus);
            var order1 = customer.OrdersPlus.Where(o => o.OrderIDPlus == 10001).Single();
            Assert.Null(order1.ShelfLifePlus);

            //Post Entity with open complex type and with contained Navigation
            DataServiceCollection<AccountPlus> accounts =
                new DataServiceCollection<AccountPlus>(this.TestClientContext);
            var account = new AccountPlus();
            accounts.Add(account);
            account.AccountIDPlus = 110;
            account.CountryRegionPlus = "CN";
            account.AccountInfoPlus = new AccountInfoPlus()
            {
                FirstNamePlus = "New",
                LastNamePlus = "Base",
                IsActivePlus = true,
            };
            var expectedMiddleName = account.AccountInfoPlus.MiddleNamePlus;

            expectedPropertyCount = 3;
            this.TestClientContext.SaveChanges(SaveChangesOptions.PostOnlySetProperties);
            //Validate entire open complex type was sent to server side.
            Assert.Equal(4, accountInfo.Properties.Count());

            //Post Contained Navigation property
            PaymentInstrumentPlus pi = new PaymentInstrumentPlus();
            account.MyPaymentInstrumentsPlus.Add(pi);
            pi.PaymentInstrumentIDPlus = 1003;
            pi.CreatedDatePlus = DateTimeOffset.Now;
            pi.FriendlyNamePlus = "FriendlyName";

            expectedPropertyCount = 3;
            this.TestClientContext.SaveChanges(SaveChangesOptions.PostOnlySetProperties);

            //Post Contained Single Navigation property
            //var gcs = new DataServiceCollection<GiftCardPlus>(this.TestClientContext, "Accounts(110)/MyGiftCard", null, null);
            GiftCardPlus gc = new GiftCardPlus();
            account.MyGiftCardPlus = gc;
            gc.GiftCardIDPlus = 10001;
            gc.GiftCardNOPlus = "10001";
            gc.AmountPlus = 2000;
            gc.ExperationDatePlus = DateTimeOffset.Now;

            expectedPropertyCount = 4;
            this.TestClientContext.SaveChanges(SaveChangesOptions.PostOnlySetProperties);

            var account1 = this.TestClientContext.AccountsPlus.Expand(a => a.MyPaymentInstrumentsPlus).Expand(a => a.MyGiftCardPlus).Where(a => a.AccountIDPlus == 110).Single();
            Assert.Equal("CN", account1.CountryRegionPlus);
            Assert.Equal(expectedMiddleName, account1.AccountInfoPlus.MiddleNamePlus);
            Assert.Equal("Base", account1.AccountInfoPlus.LastNamePlus);
            Assert.True(account1.AccountInfoPlus.IsActivePlus);
            var pi1 = account1.MyPaymentInstrumentsPlus.Where(p => p.PaymentInstrumentIDPlus == 1003).SingleOrDefault();
            Assert.NotNull(pi1);
            Assert.Equal("FriendlyName", pi1.FriendlyNamePlus);
            Assert.Null(account1.MyGiftCardPlus.OwnerNamePlus);
            Assert.Equal(2000, account1.MyGiftCardPlus.AmountPlus);

            //Post Entity with Enum property and collection of Enum property
            DataServiceCollection<ProductPlus> products = new DataServiceCollection<ProductPlus>(this.TestClientContext);
            ProductPlus product = new ProductPlus();
            products.Add(product);
            product.NamePlus = "Apple";
            product.ProductIDPlus = 1000000;
            product.QuantityInStockPlus = 20;
            product.QuantityPerUnitPlus = "Pound";
            product.UnitPricePlus = 0.35f;
            product.DiscontinuedPlus = false;
            product.CoverColorsPlus = new ObservableCollection<ColorPlus>();
            product.SkinColorPlus = null;

            expectedPropertyCount = 8;
            this.TestClientContext.SaveChanges(SaveChangesOptions.PostOnlySetProperties);

            var product2 = this.TestClientContext.ProductsPlus.Where(p => p.ProductIDPlus == 1000000).FirstOrDefault();
            Assert.NotNull(product2);
            Assert.Equal("Apple", product2.NamePlus);

            //Post Navigation property under derived singleton
            DataServiceCollection<CompanyPlus> publicCompany = new DataServiceCollection<CompanyPlus>(this.TestClientContext.PublicCompanyPlus);
            AssetPlus asset = new AssetPlus();
            (publicCompany[0] as PublicCompanyPlus).AssetsPlus.Add(asset);
            asset.AssetIDPlus = 4;
            asset.NumberPlus = 50;

            expectedPropertyCount = 2;
            this.TestClientContext.SaveChanges(SaveChangesOptions.PostOnlySetProperties);

            var pc = this.TestClientContext.PublicCompanyPlus.Expand(p => (p as PublicCompanyPlus).AssetsPlus).GetValue() as PublicCompanyPlus;
            var asset2 = pc.AssetsPlus.Where(a => a.AssetIDPlus == 4).First();
            Assert.Equal(50, asset2.NumberPlus);
            Assert.Null(asset2.NamePlus);

            //Post Navigation property of Singleton
            DataServiceCollection<CompanyPlus> company = new DataServiceCollection<CompanyPlus>(this.TestClientContext.CompanyPlus);
            DepartmentPlus department = new DepartmentPlus();
            company[0].DepartmentsPlus.Add(department);
            department.DepartmentIDPlus = 10001;
            department.NamePlus = "D1";

            expectedPropertyCount = 2;
            this.TestClientContext.SaveChanges(SaveChangesOptions.PostOnlySetProperties);

            var company2 = this.TestClientContext.CompanyPlus.Expand(p => p.DepartmentsPlus).GetValue();
            var department2 = company2.DepartmentsPlus.Where(d => d.DepartmentIDPlus == 10001).First();
            Assert.Equal("D1", department.NamePlus);
            Assert.Null(department.DepartmentNOPlus);
        }

        [Fact]
        public void PostPartialPropertiesInBatch()
        {
            var batchFlags = new SaveChangesOptions[] { SaveChangesOptions.BatchWithSingleChangeset, SaveChangesOptions.BatchWithIndependentOperations };
            foreach (var batchFlag in batchFlags)
            {
                List<ODataResource> entries = new List<ODataResource>();
                this.TestClientContext.Configurations.RequestPipeline.OnEntryEnding((arg) =>
                {
                    entries.Add(arg.Entry);
                });

                DataServiceCollection<CustomerPlus> people =
                    new DataServiceCollection<CustomerPlus>(this.TestClientContext, "People", null, null);
                var person = new CustomerPlus();
                people.Add(person);

                person.FirstNamePlus = "Nelson";
                person.LastNamePlus = "Black";
                person.NumbersPlus = new ObservableCollection<string> { };
                person.EmailsPlus = new ObservableCollection<string> { "abc@abc.com" };
                person.PersonIDPlus = 10001;
                person.CityPlus = "London";
                person.TimeBetweenLastTwoOrdersPlus = new TimeSpan(1);
                person.HomeAddressPlus = new HomeAddressPlus()
                {
                    CityPlus = "Redmond",
                    PostalCodePlus = "98052",
                    StreetPlus = "1 Microsoft Way"
                };

                DataServiceCollection<AccountPlus> accounts =
                    new DataServiceCollection<AccountPlus>(this.TestClientContext);
                var account = new AccountPlus();
                accounts.Add(account);
                account.AccountIDPlus = 110;
                account.CountryRegionPlus = "CN";
                account.AccountInfoPlus = new AccountInfoPlus()
                {
                    FirstNamePlus = "New",
                    LastNamePlus = "Base",
                    IsActivePlus = true,
                };

                DataServiceCollection<ProductPlus> products = new DataServiceCollection<ProductPlus>(this.TestClientContext);
                ProductPlus product = new ProductPlus();
                products.Add(product);
                product.NamePlus = "Apple";
                product.ProductIDPlus = 1000000;
                product.QuantityInStockPlus = 20;
                product.QuantityPerUnitPlus = "Pound";
                product.UnitPricePlus = 0.35f;
                product.DiscontinuedPlus = false;
                product.CoverColorsPlus = new ObservableCollection<ColorPlus>();

                //Post entity into an entity set
                this.TestClientContext.SaveChanges(SaveChangesOptions.PostOnlySetProperties | batchFlag);

                Assert.Equal(7, entries.Where(e => e.TypeName.Contains("CustomerPlus")).First().Properties.Count());
                Assert.Equal(2, entries.Where(e => e.TypeName.Contains("AccountPlus")).First().Properties.Count());
                Assert.Equal(7, entries.Where(e => e.TypeName.Contains("ProductPlus")).First().Properties.Count());
            }
        }

        [Fact]
        public void PostFullProperties()
        {
            int expectedPropertyCount = 0;
            ODataResource entry = null;
            this.TestClientContext.Configurations.RequestPipeline.OnEntryEnding((arg) =>
            {
                if (arg.Entry.TypeName.EndsWith("CustomerPlus")
                    || arg.Entry.TypeName.EndsWith("OrderPlus"))
                {
                    entry = arg.Entry;
                    Assert.Equal(expectedPropertyCount, entry.Properties.Count());
                }
            });

            //Entity of Derived type
            //Entity has a complex property of derived type
            DataServiceCollection<CustomerPlus> people =
                new DataServiceCollection<CustomerPlus>(this.TestClientContext, "People", null, null);
            var person = new CustomerPlus();
            people.Add(person);

            person.FirstNamePlus = "Nelson";
            person.LastNamePlus = "Black";
            person.NumbersPlus = new ObservableCollection<string> { };
            person.EmailsPlus = new ObservableCollection<string> { "abc@abc.com" };
            person.PersonIDPlus = 10001;
            person.CityPlus = "London";
            person.TimeBetweenLastTwoOrdersPlus = new TimeSpan(1);
            person.HomeAddressPlus = new HomeAddressPlus()
            {
                CityPlus = "Redmond",
                PostalCodePlus = "98052",
                StreetPlus = "1 Microsoft Way"
            };

            //Post entity into an entity set
            expectedPropertyCount = 10;
            this.TestClientContext.SaveChanges();

            //Post Navigation property to an entitySet
            var people1 = new DataServiceCollection<CustomerPlus>(this.TestClientContext.CustomersPlus.Where(p => p.PersonIDPlus == 10001));
            OrderPlus order = new OrderPlus();
            people1[0].OrdersPlus.Add(order);
            order.OrderIDPlus = 11111111;
            order.OrderDatePlus = new DateTimeOffset(new DateTime(2011, 5, 29, 14, 21, 12));
            order.OrderShelfLifesPlus = new ObservableCollection<TimeSpan>();

            expectedPropertyCount = 6;
            this.TestClientContext.SaveChanges();
        }

        //Related Fixed When an entity's nullable enum property is set to null, Client cannot correctly serializer the entry
        [Fact]
        public void PostFullPropertiesInBatch()
        {
            var batchFlags = new SaveChangesOptions[] { SaveChangesOptions.BatchWithSingleChangeset, SaveChangesOptions.BatchWithIndependentOperations };
            foreach (var batchFlag in batchFlags)
            {
                List<ODataResource> entries = new List<ODataResource>();
                this.TestClientContext.Configurations.RequestPipeline.OnEntryEnding((arg) =>
                {
                    entries.Add(arg.Entry);
                });

                DataServiceCollection<CustomerPlus> people =
                    new DataServiceCollection<CustomerPlus>(this.TestClientContext, "People", null, null);
                var person = new CustomerPlus();
                people.Add(person);

                person.FirstNamePlus = "Nelson";
                person.LastNamePlus = "Black";
                person.NumbersPlus = new ObservableCollection<string> { };
                person.EmailsPlus = new ObservableCollection<string> { "abc@abc.com" };
                person.PersonIDPlus = 10001;
                person.CityPlus = "London";
                person.TimeBetweenLastTwoOrdersPlus = new TimeSpan(1);
                person.HomeAddressPlus = new HomeAddressPlus()
                {
                    CityPlus = "Redmond",
                    PostalCodePlus = "98052",
                    StreetPlus = "1 Microsoft Way"
                };

                DataServiceCollection<AccountPlus> accounts =
                    new DataServiceCollection<AccountPlus>(this.TestClientContext);
                var account = new AccountPlus();
                accounts.Add(account);
                account.AccountIDPlus = 110;
                account.CountryRegionPlus = "CN";

                DataServiceCollection<ProductPlus> products = new DataServiceCollection<ProductPlus>(this.TestClientContext);
                ProductPlus product = new ProductPlus();
                products.Add(product);
                product.NamePlus = "Apple";
                product.ProductIDPlus = 1000000;
                product.QuantityInStockPlus = 20;
                product.QuantityPerUnitPlus = "Pound";
                product.UnitPricePlus = 0.35f;
                product.DiscontinuedPlus = false;
                product.CoverColorsPlus = new ObservableCollection<ColorPlus>();

                //Post entity into an entity set
                this.TestClientContext.SaveChanges(batchFlag);

                Assert.Equal(10, entries.Where(e => e.TypeName.Contains("CustomerPlus")).First().Properties.Count());
                Assert.Equal(2, entries.Where(e => e.TypeName.Contains("AccountPlus")).First().Properties.Count());
                Assert.Equal(9, entries.Where(e => e.TypeName.Contains("ProductPlus")).First().Properties.Count());
            }
        }

        [Fact]
        public void PostFullPropertiesOfEntityParameterToAction()
        {
            PostPropertiesOfEntityParameterToAction(EntityParameterSendOption.SendFullProperties);
        }

        [Fact]
        public void PostOnlySetPropertiesOfEntityParameterToAction()
        {
            PostPropertiesOfEntityParameterToAction(EntityParameterSendOption.SendOnlySetProperties);
        }

        private void PostPropertiesOfEntityParameterToAction(EntityParameterSendOption option)
        {
            TestClientContext.EntityParameterSendOption = option;

            var customer = TestClientContext.CustomersPlus.First();

            OrderPlus order = new OrderPlus();

            var collection = new DataServiceCollection<OrderPlus>(TestClientContext, "OrdersPlus", null, null);
            collection.Add(order);

            int orderId = (new Random()).Next();
            order.OrderIDPlus = orderId;
            order.OrderShelfLifesPlus = new ObservableCollection<TimeSpan>();

            OrderPlus orderCreated = customer.PlaceOrderPlus(order).GetValue();
            Assert.Equal(orderId, orderCreated.OrderIDPlus);
        }
    }
}

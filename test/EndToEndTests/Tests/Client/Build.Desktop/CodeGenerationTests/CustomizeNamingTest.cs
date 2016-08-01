//---------------------------------------------------------------------
// <copyright file="CustomizeNamingTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.CodeGenerationTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Spatial;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReferencePlus;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ODataClient = Microsoft.OData.Client;

    /// <summary>
    /// T4 code generation for operations test cases.
    /// </summary>
    [TestClass]
    public class CustomizeNamingTest : ODataWCFServiceTestsBase<Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReferencePlus.InMemoryEntitiesPlus>
    {
        private const string ServerSideNameSpacePrefix = "Microsoft.Test.OData.Services.ODataWCFService.";

        public CustomizeNamingTest()
            : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {
        }

        [TestMethod]
        public void BasicQuery()
        {
            TestClientContext.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges;
            // Query a entity set
            var products1 = TestClientContext.ProductsPlus.ToList();
            Assert.AreEqual(5, products1.Count);

            // Query with expand (Linq)
            var products2 = TestClientContext.ProductsPlus.Expand(p => p.DetailsPlus).ToList();
            Assert.AreEqual(5, products2.Single(p => p.ProductIDPlus == 5).DetailsPlus.Count);

            // Query with expand (PropertyName)
            var products3 = TestClientContext.ProductsPlus.Expand("Details").ToList();
            Assert.AreEqual(5, products3.Single(p => p.ProductIDPlus == 5).DetailsPlus.Count);

            // Query a individual primitive property
            var product4 = TestClientContext.ProductsPlus.Where(p => p.ProductIDPlus == 5).Single();
            Assert.AreEqual("Cheetos", product4.NamePlus);

            // Query an Navigation Property
            TestClientContext.LoadProperty(product4, "Details");
            Assert.AreEqual(5, product4.DetailsPlus.Count);

            // Query a Derived entity.
            var people5 = TestClientContext.PeoplePlus.Where(p => p.PersonIDPlus == 1).Single();
            // Check the property from the derived type.
            Assert.AreEqual("Tokyo", people5.HomeAddressPlus.CityPlus);
            // Check the derived complex property.
            Assert.AreEqual("Cats", ((HomeAddressPlus)(people5.HomeAddressPlus)).FamilyNamePlus);
            // Check collection of PrimitiveTypes
            Assert.AreEqual(1, people5.EmailsPlus.Count);

            // Query with $select & $expand
            var accounts6 = TestClientContext.AccountsPlus
                .Where(a => a.AccountIDPlus == 103)
                .Select(a => new AccountPlus() { AccountIDPlus = a.AccountIDPlus, MyGiftCardPlus = a.MyGiftCardPlus, CountryRegionPlus = a.CountryRegionPlus });
            var account6 = accounts6.Single();
            Assert.IsNotNull(account6.MyGiftCardPlus);
            Assert.AreEqual(103, account6.AccountIDPlus);
            Assert.IsNull(account6.AccountInfoPlus);

            // Query with $filter by non-key property.
            var accounts7 = TestClientContext.AccountsPlus.Where(a => a.CountryRegionPlus == "CN").ToList();
            Assert.AreEqual(3, accounts7.Count);

            // Query with OrderBy
            var people8 = TestClientContext.PeoplePlus.OrderBy((p) => p.LastNamePlus).First();
            Assert.AreEqual(5, people8.PersonIDPlus);

            // Query with $count
            var count = TestClientContext.AccountsPlus.Count();
            Assert.AreEqual(count, 7);

            // Query with MultiKeys
            var productReview10 = TestClientContext.ProductReviewsPlus.Where(pd =>
                pd.ProductDetailIDPlus == 2
                && pd.ProductIDPlus == 5
                && pd.ReviewTitlePlus == "Special"
                && pd.RevisionIDPlus == 1).First();
            Assert.AreEqual("Andy", productReview10.AuthorPlus);
        }

        [TestMethod]
        public void BasicModify()
        {
            TestClientContext.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges;
            ///TestClientContext.UndeclaredPropertyBehavior = ODataClient.UndeclaredPropertyBehavior.Support;
            // AddRelatedObject
            AccountPlus newAccount1 = new AccountPlus()
            {
                AccountIDPlus = 110,
                CountryRegionPlus = "CN",
                AccountInfoPlus = new AccountInfoPlus()
                {
                    FirstNamePlus = "New",
                    LastNamePlus = "Boy"
                }
            };

            PaymentInstrumentPlus newPI = new PaymentInstrumentPlus()
            {
                PaymentInstrumentIDPlus = 110901,
                FriendlyNamePlus = "110's first PI",
                CreatedDatePlus = new DateTimeOffset(new DateTime(2012, 12, 10))
            };

            TestClientContext.AddToAccountsPlus(newAccount1);
            TestClientContext.AddRelatedObject(newAccount1, "MyPaymentInstruments", newPI);
            TestClientContext.SaveChanges();

            var r1 = TestClientContext.AccountsPlus.Where(account => account.AccountIDPlus == 110).Single();
            Assert.AreEqual("Boy", r1.AccountInfoPlus.LastNamePlus);
            var r2 = TestClientContext.CreateQuery<PaymentInstrumentPlus>("Accounts(110)/MyPaymentInstruments")
                .Where(pi => pi.PaymentInstrumentIDPlus == 110901).Single();
            Assert.AreEqual("110's first PI", r2.FriendlyNamePlus);

            //UpdateObject
            newAccount1.CountryRegionPlus = "US";
            TestClientContext.UpdateObject(newAccount1);
            TestClientContext.SaveChanges();

            r1 = TestClientContext.AccountsPlus.Where(account => account.AccountIDPlus == 110).Single();
            Assert.AreEqual("US", r1.CountryRegionPlus);

            //UpdateRelatedObject
            var myGiftCard = new GiftCardPlus()
            {
                GiftCardIDPlus = 11111,
                GiftCardNOPlus = "11111",
                AmountPlus = 20,
                ExperationDatePlus = new DateTimeOffset(2015, 12, 1, 0, 0, 0, new TimeSpan(0))
            };
            TestClientContext.UpdateRelatedObject(newAccount1, "MyGiftCard", myGiftCard);
            TestClientContext.SaveChanges();

            r1 = TestClientContext.AccountsPlus.Expand(account => account.MyGiftCardPlus).Where(account => account.AccountIDPlus == 110).Single();
            Assert.AreEqual(11111, r1.MyGiftCardPlus.GiftCardIDPlus);

            //Add Derived Object
            CustomerPlus customerPlus = new CustomerPlus()
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
            };

            var ordersPlus = new ODataClient.DataServiceCollection<OrderPlus>(TestClientContext)
            {
                new OrderPlus()
                {
                    OrderIDPlus = 11111111,
                    OrderDatePlus = new DateTimeOffset(new DateTime(2011, 5, 29, 14, 21, 12)),
                    ShelfLifePlus = new TimeSpan(1),
                    OrderShelfLifesPlus = new ObservableCollection<TimeSpan>(){new TimeSpan(1)}
                }
            };
            TestClientContext.AddToPeoplePlus(customerPlus);
            TestClientContext.SaveChanges();
            var customer1 = TestClientContext.CustomersPlus.Where(c => c.PersonIDPlus == 10001).Single();
            TestClientContext.AddLink(customer1, "Orders", ordersPlus[0]);
            TestClientContext.SaveChanges();

            TestClientContext.Detach(customerPlus);
            TestClientContext.SaveChanges();

            var customer = TestClientContext.CustomersPlus.Expand(p => (p as CustomerPlus).OrdersPlus).Where(p => p.PersonIDPlus == 10001).SingleOrDefault();
            Assert.AreEqual(((CustomerPlus)customer).CityPlus, "London");
            Assert.AreEqual(((HomeAddressPlus)(customer.HomeAddressPlus)).FamilyNamePlus, "Black's Family");
            Assert.AreEqual(((CustomerPlus)customer).OrdersPlus.Count, 1);

            var order = TestClientContext.OrdersPlus.Where(p => p.OrderIDPlus == 11111111).SingleOrDefault();
            Assert.AreEqual(order.OrderShelfLifesPlus.Count, 1);

            // DeleteObject
            TestClientContext.DeleteObject(newAccount1);
            TestClientContext.SaveChanges();
            var accounts = TestClientContext.AccountsPlus.ToList();
            Assert.IsTrue(!accounts.Any(ac => ac.AccountIDPlus == 110));

            // SetLink
            var person1 = TestClientContext.PeoplePlus.Where((p) => p.PersonIDPlus == 1).Single();
            var person2 = TestClientContext.PeoplePlus.Where((p) => p.PersonIDPlus == 2).Single();
            TestClientContext.SetLink(person1, "Parent", person2);
            TestClientContext.SaveChanges();

            person1 = TestClientContext.PeoplePlus.Expand(d => d.ParentPlus).Where((p) => p.PersonIDPlus == 1).Single();
            Assert.IsNotNull(person1.ParentPlus);
            Assert.IsNotNull(person1.ParentPlus.PersonIDPlus == 2);

            // SetLink : Bug, SetLink to Null will not update the client object.
            TestClientContext.SetLink(person1, "Parent", null);
            TestClientContext.SaveChanges();

            person1.ParentPlus = null;
            var person3 = TestClientContext.PeoplePlus.Expand(d => d.ParentPlus).Where((p) => p.PersonIDPlus == 1).Single();
            Assert.IsNull(person3.ParentPlus);

            //AddLink
            var companyPlus = TestClientContext.CompanyPlus.GetValue();
            DepartmentPlus department = new DepartmentPlus()
            {
                DepartmentIDPlus = 100001,
                NamePlus = "ID" + 100001,
            };
            TestClientContext.AddToDepartmentsPlus(department);
            TestClientContext.AddLink(companyPlus, "Departments", department);
            TestClientContext.SaveChanges();

            TestClientContext.LoadProperty(companyPlus, "Departments");
            Assert.IsTrue(companyPlus.DepartmentsPlus.Any(d => d.DepartmentIDPlus == department.DepartmentIDPlus));

            //Delete Link
            TestClientContext.DeleteLink(companyPlus, "Departments", department);
            TestClientContext.SaveChanges();

            TestClientContext.LoadProperty(companyPlus, "Departments");
            Assert.IsTrue(!companyPlus.DepartmentsPlus.Any(d => d.DepartmentIDPlus == department.DepartmentIDPlus));

        }

        [TestMethod]
        public void OpenComplexType()
        {
            //Update entity with open complex type
            AccountPlus account = new AccountPlus()
            {
                AccountIDPlus = 1000000,
                CountryRegionPlus = "CN",
                AccountInfoPlus = new AccountInfoPlus()
                {
                    FirstNamePlus = "Peter",
                    MiddleNamePlus = "White",
                    LastNamePlus = "Andy",
                    IsActivePlus = true
                }
            };
            TestClientContext.AddToAccountsPlus(account);
            TestClientContext.SaveChanges();

            //Check account can be correctly desirialized.
            account = TestClientContext.AccountsPlus.Where(a => a.AccountIDPlus == 1000000).Single();
            Assert.IsNotNull(account);
            Assert.AreEqual(account.AccountInfoPlus.MiddleNamePlus, "White");
            Assert.IsTrue(account.AccountInfoPlus.IsActivePlus);

            //Update entity with open complex type
            var accountWrapper = new ODataResourceWrapper()
            {
                Resource = new ODataResource()
                {
                    TypeName = ServerSideNameSpacePrefix + "Account",
                    Properties = new[]
                    {
                        new ODataProperty { Name = "AccountID", Value = 1000000 }
                    }
                },
                NestedResourceInfoWrappers = new List<ODataNestedResourceInfoWrapper>()
                {
                    new ODataNestedResourceInfoWrapper()
                    {
                        NestedResourceInfo = new ODataNestedResourceInfo()
                        {
                            Name = "AccountInfo",
                            IsCollection = false
                        },
                        NestedResourceOrResourceSet = new ODataResourceWrapper()
                        {
                            Resource =new ODataResource()
                            {
                                TypeName = ServerSideNameSpacePrefix + "AccountInfo",
                                Properties = new[]
                                {
                                    new ODataProperty
                                    {
                                        Name = "FirstName",
                                        Value = "Peter"
                                    },
                                    new ODataProperty
                                    {
                                        Name = "LastName",
                                        Value = "Andy"
                                    },
                                    //Property that exists in Customer-Defined client code.
                                    new ODataProperty
                                    {
                                        Name = "MiddleName",
                                        Value = "White2"
                                    },
                                    new ODataProperty
                                    {
                                        Name = "IsActive",
                                        Value = false,
                                    },
                                    //Property that doesn't exist in Customer-Defined client code.
                                    new ODataProperty
                                    {
                                        Name = "ShippingAddress",
                                        Value = "#999, ZiXing Road"
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var settings = new ODataMessageWriterSettings();
            settings.BaseUri = ServiceBaseUri;

            var accountType = Model.FindDeclaredType(ServerSideNameSpacePrefix + "Account") as IEdmEntityType;
            var accountSet = Model.EntityContainer.FindEntitySet("Accounts");

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "Accounts(1000000)"));
            requestMessage.SetHeader("Content-Type", MimeTypes.ApplicationJson);
            requestMessage.SetHeader("Accept", MimeTypes.ApplicationJson);
            requestMessage.Method = "PATCH";
            using (var messageWriter = new ODataMessageWriter(requestMessage, settings))
            {
                var odataWriter = messageWriter.CreateODataResourceWriter(accountSet, accountType);
                ODataWriterHelper.WriteResource(odataWriter, accountWrapper);
            }

            var responseMessage = requestMessage.GetResponse();

            TestClientContext.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges;
            //Check account can be correctly desirialized.
            account = TestClientContext.AccountsPlus.Where(a => a.AccountIDPlus == 1000000).Single();
            Assert.IsNotNull(account);
            Assert.AreEqual(account.AccountInfoPlus.MiddleNamePlus, "White2");
            Assert.IsTrue(!account.AccountInfoPlus.IsActivePlus);
        }

        [TestMethod]
        public void OpenEntityType()
        {
            //UpdateOpenTypeSingleton
            var entry = new ODataResource() { TypeName = ServerSideNameSpacePrefix + "PublicCompany" };
            entry.Properties = new[] 
            {
                new ODataProperty
                {
                    Name = "FullName",
                    Value = "MS Ltd."
                },
                new ODataProperty
                {
                    Name = "PhoneNumber",
                    Value = "123-45678"                    
                },
                
                new ODataProperty
                {
                    Name = "TotalAssets",
                    Value = 500000L,
                }
            };
            var settings = new ODataMessageWriterSettings();
            settings.BaseUri = ServiceBaseUri;
            var companyType = Model.FindDeclaredType(ServerSideNameSpacePrefix + "PublicCompany") as IEdmEntityType;
            var companySingleton = Model.EntityContainer.FindSingleton("PublicCompany");

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "PublicCompany"));
            requestMessage.SetHeader("Content-Type", MimeTypes.ApplicationJson);
            requestMessage.SetHeader("Accept", MimeTypes.ApplicationJson);
            requestMessage.Method = "PATCH";
            using (var messageWriter = new ODataMessageWriter(requestMessage, settings))
            {
                var odataWriter = messageWriter.CreateODataResourceWriter(companySingleton, companyType);
                odataWriter.WriteStart(entry);
                odataWriter.WriteEnd();
            }

            var responseMessage = requestMessage.GetResponse();
            Assert.AreEqual(204, responseMessage.StatusCode);

            //Check account can be correctly desirialized.
            var company = TestClientContext.PublicCompanyPlus.GetValue();
            Assert.IsNotNull(company);
            Assert.AreEqual("MS Ltd.", company.FullNamePlus);
            Assert.AreEqual(500000, company.TotalAssetsPlus);

            TestClientContext.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges;
            company.FullNamePlus = "MS2 Ltd.";
            company.TotalAssetsPlus = 1000000;
            TestClientContext.UpdateObject(company);
            TestClientContext.SaveChanges();

            company.FullNamePlus = null;
            company.TotalAssetsPlus = 0;
            company = TestClientContext.PublicCompanyPlus.GetValue();
            Assert.IsNotNull(company);
            Assert.AreEqual("MS2 Ltd.", company.FullNamePlus);
            Assert.AreEqual(1000000, company.TotalAssetsPlus);
        }

        [TestMethod]
        public void InvokeOperations()
        {
            TestClientContext.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges;

            // Invoke Unbounded Action
            var color1 = TestClientContext.GetDefaultColorPlus().GetValue();
            Assert.AreEqual(color1, ColorPlus.RedPlus);

            // Invoke Bounded Function on single entity
            var account = TestClientContext.AccountsPlus.Where(a => a.AccountIDPlus == 101).Single();
            var r2 = account.GetDefaultPIPlus().GetValue();
            Assert.AreEqual(101901, r2.PaymentInstrumentIDPlus);

            // Invoke bounded Function on Navigation Property
            var account3 = TestClientContext.AccountsPlus.Expand(c => c.MyGiftCardPlus).Where(a => a.AccountIDPlus == 101).Single();
            var result3 = account3.MyGiftCardPlus.GetActualAmountPlus(1).GetValue();
            Assert.AreEqual(39.8, result3);

            // Invoke bounded Action on single entity set
            var product4 = TestClientContext.ProductsPlus.Where(p => p.ProductIDPlus == 7).Single();
            var result = product4.AddAccessRightPlus(AccessLevelPlus.WritePlus).GetValue();
            Assert.AreEqual(AccessLevelPlus.ReadWritePlus, result);

            // Invoke bounded Action on Navigation Property
            var account5 = TestClientContext.AccountsPlus.Where(ac => ac.AccountIDPlus == 101).Single();
            var result5 = account5.RefreshDefaultPIPlus(DateTimeOffset.Now).GetValue();
            Assert.AreEqual(101901, result5.PaymentInstrumentIDPlus);
        }

        [TestMethod]
        public void ContainedEntityQuery()
        {
            TestClientContext.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges;
            // Query a single contained entity
            var q1 = TestClientContext.CreateQuery<PaymentInstrumentPlus>("Accounts(103)/MyPaymentInstruments(103902)");
            Assert.IsTrue(q1.RequestUri.OriginalString.EndsWith("Accounts(103)/MyPaymentInstruments(103902)", StringComparison.Ordinal));

            List<PaymentInstrumentPlus> r1 = q1.ToList();
            Assert.AreEqual(1, r1.Count);
            Assert.AreEqual(103902, r1[0].PaymentInstrumentIDPlus);
            Assert.AreEqual("103 second PI", r1[0].FriendlyNamePlus);

            // Query a contained entity set with query option
            var q2 = TestClientContext.CreateQuery<PaymentInstrumentPlus>("Accounts(103)/MyPaymentInstruments").Expand(pi => pi.BillingStatementsPlus).Where(pi => pi.PaymentInstrumentIDPlus == 103901);
            PaymentInstrumentPlus r2 = q2.Single();
            Assert.IsNotNull(r2.BillingStatementsPlus);

            // Invoke a bounded Function.
            double result = TestClientContext.Execute<double>(new Uri(ServiceBaseUri.AbsoluteUri +
                "Accounts(101)/MyGiftCard/Microsoft.Test.OData.Services.ODataWCFService.GetActualAmount(bonusRate=0.2)", UriKind.Absolute), "GET", true).Single();
            Assert.AreEqual(23.88, result);
        }

        [TestMethod]
        public void SingltonQuery()
        {
            TestClientContext.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges;
            // Invoke a bounded Function 
            var company1 = TestClientContext.CompanyPlus.GetValue();
            var result1 = company1.GetEmployeesCountPlus().GetValue();
            Assert.AreEqual(2, result1);

            // Invoke a bounded Action
            var company2 = TestClientContext.CompanyPlus.GetValue();
            var result2 = company2.IncreaseRevenuePlus(1).GetValue();
            Assert.AreEqual(100001, result2);

            // Invoke a bounded Action on derived type
            TestClientContext.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges;
            var publicCompany = TestClientContext.PublicCompanyPlus.GetValue();
            var originalRevenue = publicCompany.RevenuePlus;
            var revenue = publicCompany.IncreaseRevenuePlus(10).GetValue();
            Assert.IsTrue(originalRevenue + 10 == revenue);

            publicCompany = TestClientContext.PublicCompanyPlus.GetValue();
            Assert.IsTrue(revenue == publicCompany.RevenuePlus);

            // Invoke Unbound Action 
            TestClientContext.ResetBossAddressPlus(
                new HomeAddressPlus()
                {
                    CityPlus = "Shanghai",
                    StreetPlus = "ZiXing Road",
                    PostalCodePlus = "200100",
                    FamilyNamePlus = "White's Family"
                }).GetValue();
            TestClientContext.SaveChanges();

            var boss = TestClientContext.BossPlus.GetValue();
            Assert.AreEqual(boss.HomeAddressPlus.PostalCodePlus, "200100");
            Assert.AreEqual(((HomeAddressPlus)boss.HomeAddressPlus).FamilyNamePlus, "White's Family");
        }
    }
}

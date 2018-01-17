//---------------------------------------------------------------------
// <copyright file="T4OperationGenerationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.CodeGenerationTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Spatial;
    using Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// T4 code generation for operations test cases.
    /// </summary>
    [TestClass]
    public class T4OperationGenerationTests : ODataWCFServiceTestsBase<InMemoryEntities>
    {
        Random random = new Random();
        public T4OperationGenerationTests()
            : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {
        }

        [TestMethod]
        public void FunctionImportTest()
        {
            var result = TestClientContext.GetDefaultColor().GetValue();
            Assert.AreEqual(result, Color.Red);
        }

        [TestMethod]
        public void FunctionBoundOnSingletonTest()
        {
            var company = TestClientContext.Company.GetValue();
            var result = company.GetEmployeesCount().GetValue();
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void FunctionBoundOnEntityTypeTest()
        {
            var account = TestClientContext.Accounts.Where(a => a.AccountID == 101).Single();
            var result = account.GetDefaultPI().GetValue();
            Assert.AreEqual(101901, result.PaymentInstrumentID);
        }

        [TestMethod]
        public void FunctionBoundOnNavigationPropertyTest()
        {
            var account = TestClientContext.Accounts.Expand("MyGiftCard").Where(a => a.AccountID == 101).Single();
            var result = account.MyGiftCard.GetActualAmount(1).GetValue();
            Assert.AreEqual(39.8, result);
        }

        [TestMethod]
        public void ActionBoundOnSingletonTypeTest()
        {
            var company = TestClientContext.Company.GetValue();
            var result = company.IncreaseRevenue(1).GetValue();
            Assert.AreEqual(100001, result);
        }

        [TestMethod]
        public void ActionBoundOnEntityTypeTest()
        {
            var product = TestClientContext.Products.Where(p => p.ProductID == 7).Single();
            var result = product.AddAccessRight(AccessLevel.Write).GetValue();
            Assert.AreEqual(AccessLevel.ReadWrite, result);
        }

        [TestMethod]
        public void ActionBoundOnNavigationPropertyTest()
        {
            var account = TestClientContext.Accounts.Where(ac => ac.AccountID == 101).Single();
            var result = account.RefreshDefaultPI(DateTimeOffset.Now).GetValue();
            Assert.AreEqual(101901, result.PaymentInstrumentID);
        }

        [TestMethod]
        public void ActionBoundOnSingletonOfDerivedType()
        {
            TestClientContext.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges;
            var publicCompany = TestClientContext.PublicCompany.GetValue();
            var originalRevenue = publicCompany.Revenue;
            var revenue = publicCompany.IncreaseRevenue(10).GetValue();
            Assert.IsTrue(originalRevenue + 10 == revenue);

            publicCompany = TestClientContext.PublicCompany.GetValue();
            Assert.IsTrue(revenue == publicCompany.Revenue);
        }

        [TestMethod]
        public void ActionBoundOnEntityWithCollectionParameter()
        {
            TestClientContext.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges;
            Person newPerson = new Person()
            {
                FirstName = "Bob",
                LastName = "Cat",
                Numbers = new ObservableCollection<string> { "111-111-1111" },
                Emails = new ObservableCollection<string> { "abc@abc.com" },
                PersonID = random.Next(),
                Home = GeographyPoint.Create(32.1, 23.1),
                HomeAddress = new Address() { City = "London", PostalCode = "98052", Street = "1 Microsoft Way" }
            };
            TestClientContext.AddToPeople(newPerson);
            TestClientContext.SaveChanges();
            var customer = TestClientContext.People.Where(c => c.PersonID == newPerson.PersonID).Single();
            var person = customer.ResetAddress(
                new ObservableCollection<Address>()
                {
                    new Address() { City = "London", PostalCode = "11111", Street = "2 Microsoft Way" },
                    new Address() { City = "London", PostalCode = "22222", Street = "3 Microsoft Way" },
                }, 1).GetValue();
            Assert.AreEqual("22222", person.HomeAddress.PostalCode);

            person = TestClientContext.People.Where(c => c.PersonID == newPerson.PersonID).Single();
            Assert.AreEqual("22222", person.HomeAddress.PostalCode);

            person = customer.ResetAddress(
                new ObservableCollection<Address>()
                {
                    new Address() { City = "London", PostalCode = "11111", Street = "2 Microsoft Way" },
                    null
                }, 1).GetValue();
            Assert.AreEqual(null, person.HomeAddress);
        }

        [TestMethod]
        public void ActionBoundOnEntityWithEntityOrEntityCollectionParameter()
        {
            var customer = TestClientContext.Customers.First();

            int orderId = (new Random()).Next();
            Order order = new Order()
            {
                OrderID = orderId,
                OrderDate = new DateTimeOffset(new DateTime(2011, 3, 4, 16, 3, 57)),
                OrderShelfLifes = new ObservableCollection<TimeSpan>(),
                InfoFromCustomer = new InfoFromCustomer { CustomerMessage = "I need XXL" },
            };

            Order newOrder = customer.PlaceOrder(order).GetValue();
            Assert.AreEqual(orderId, newOrder.OrderID);

            var order2 = new Order()
            {
                OrderID = (new Random()).Next(),
                OrderShelfLifes = new ObservableCollection<TimeSpan>(),
            };
            var orderList = customer.PlaceOrders(new List<Order>() {order, order2}).Execute();
            Assert.AreEqual(2, orderList.Count());
        }

        [TestMethod]
        public void ActionImportWithPrimitiveParameter()
        {
            int percentage = 50;
            var product = TestClientContext.Products.First();
            TestClientContext.Discount(percentage);
            var product1 = TestClientContext.Products.Where(p => p.ProductID == product.ProductID).Single();
            Assert.AreEqual((product.UnitPrice * (1 - percentage / 100)), product1.UnitPrice);
        }

        [TestMethod]
        public void ActionImportWithCollectionOfPrimitiveParameter()
        {
            TestClientContext.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges;
            var boss = TestClientContext.Boss.GetValue();
            var emails = TestClientContext.ResetBossEmail(new ObservableCollection<string>() { "test1@var1.com", "test2@var1.com" }).Execute().ToList();
            Assert.IsTrue(emails.Count() == 2);
            Assert.IsTrue((string)emails.First() == "test1@var1.com" && (string)emails.Last() == "test2@var1.com");

            boss = TestClientContext.Boss.GetValue();
            Assert.IsTrue(boss.Emails.Count == 2);
            Assert.IsTrue(boss.Emails[0] == "test1@var1.com" && boss.Emails[1] == "test2@var1.com");
        }

        [TestMethod]
        public void ActionImportWithComplexTypeParameter()
        {
            TestClientContext.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges;
            var boss = TestClientContext.Boss.GetValue();
            var address = TestClientContext.ResetBossAddress(new Address() { City = "London", PostalCode = "11111", Street = "2 Microsoft Way" }).GetValue();
            Assert.IsTrue(address.PostalCode == "11111");
            boss = TestClientContext.Boss.GetValue();
            Assert.IsTrue(boss.HomeAddress.PostalCode == "11111");
        }

        //Complex type as function parameter in uri is not supported
        // [TestMethod] // github issuse: #896
        public void UnBoundFunctionReturnEntity()
        {
            TestClientContext.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges;
            var expectedPeople = TestClientContext.People.First() as Person;
            var people = TestClientContext.GetPerson(expectedPeople.HomeAddress).GetValue();

            Assert.IsTrue(people.PersonID == expectedPeople.PersonID);
        }

        //Doesn't support pass complexType in Parameter
        [TestMethod]
        public void FunctionBoundToEntityReturnCollectionOfEntity()
        {
            TestClientContext.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges;
            var actualDetails = TestClientContext.Products.First().GetProductDetails(1);

            var product = TestClientContext.Products.Expand(p => p.Details).First();
            Assert.IsTrue(product.Details.First().ProductDetailID == actualDetails.Single().ProductDetailID);
        }

        [TestMethod]
        public void UnboundFunctionReturnCollectionOfPrimitive()
        {
            TestClientContext.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges;
            var emails = TestClientContext.Boss.GetValue().Emails;
            var actualEmails = TestClientContext.GetBossEmails(0, emails.Count).Execute().ToList();

            Assert.IsTrue(emails.Count == actualEmails.Count());
        }

        [TestMethod]
        public void UnboundFunctionPrimitiveTypeReturnEntity()
        {
            TestClientContext.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges;
            var expectedPeople = TestClientContext.People.First() as Person;
            var people = TestClientContext.GetPerson2(expectedPeople.HomeAddress.City).GetValue();

            Assert.IsTrue(people.PersonID == expectedPeople.PersonID);
        }

        [TestMethod]
        public void UnboundFunctionWithoutParameterReturnCollectionOfEntity()
        {
            TestClientContext.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges;
            var expectedProducts = TestClientContext.Products.ToList();
            var products = TestClientContext.GetAllProducts().Execute();

            Assert.IsTrue(products.Count() == expectedProducts.Count());
        }

        [TestMethod]
        public void UnBoundFunctionEnumParameter()
        {
            TestClientContext.MergeOption = Microsoft.OData.Client.MergeOption.OverwriteChanges;
            var products = TestClientContext.Products.ToList();
            var productsByAccessLevel = TestClientContext.GetProductsByAccessLevel(AccessLevel.Execute).Execute();
            var expectedProducts = products.Where(p => (p.UserAccess & AccessLevel.Execute) == AccessLevel.Execute);
            Assert.IsTrue(expectedProducts.Count() == productsByAccessLevel.Count());
        }
    }
}

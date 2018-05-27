//---------------------------------------------------------------------
// <copyright file="UpdateTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.KeyAsSegmentTests
{
    using System;
    using System.Linq;
    using Microsoft.OData.Client;
    using Microsoft.Test.OData.Services.TestServices.KeyAsSegmentServiceReference;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UpdateTests : KeyAsSegmentTest
    {
        const int IdOfPerson = -10;
        
        [TestMethod]
        public void InsertSave()
        {
            var newPerson = new Person { PersonId = 9999, Name = "Some Person" };

            var contextWrapper = this.CreateWrappedContext();
            var personQuery = contextWrapper.Context.Person.Where(p => p.PersonId == newPerson.PersonId);
            
            contextWrapper.IgnoreResourceNotFoundException = true;
            var retrievedPerson = personQuery.SingleOrDefault();
            Assert.IsNull(retrievedPerson);

            contextWrapper.AddObject("Person", newPerson);
            contextWrapper.SaveChanges();
            
            retrievedPerson = personQuery.SingleOrDefault();
            Assert.IsNotNull(retrievedPerson);
            Assert.IsTrue(newPerson == retrievedPerson, "New entity and retrieved entity should reference same object");
        }

        [TestMethod]
        public void AttachUpdateObjectSave()
        {
            var contextWrapper = this.CreateWrappedContext();
            var specialEmployee = new SpecialEmployee { PersonId = IdOfPerson };
            contextWrapper.AttachTo("Person", specialEmployee);

            specialEmployee.Bonus = Int32.MaxValue;
            contextWrapper.UpdateObject(specialEmployee);
            contextWrapper.SaveChanges();

            var retrievedBonus = contextWrapper.Context.Person.OfType<SpecialEmployee>().Where(p => p.PersonId == specialEmployee.PersonId).Select(p => p.Bonus);
            Assert.AreEqual(Int32.MaxValue, retrievedBonus.Single());
        }

        [TestMethod]
        public void AddObjectSave()
        {
            var contextWrapper = this.CreateWrappedContext();

            var specialEmployee = new SpecialEmployee { PersonId = 1234 };
            contextWrapper.AddObject("Person", specialEmployee);
            contextWrapper.SaveChanges();

            contextWrapper.DeleteObject(specialEmployee);
            contextWrapper.SaveChanges();
        }

        [TestMethod]
        public void AddObjectAddRelatedObjectSave()
        {
            var contextWrapper = this.CreateWrappedContext();

            var customer = new Customer { CustomerId = 1234 };
            var order = new Order { OrderId = 12345 };

            contextWrapper.AddObject("Customer", customer);
            contextWrapper.AddRelatedObject(customer, "Orders", order);
            contextWrapper.SaveChanges();
        }

        [TestMethod]
        public void AddDeleteLinkSave()
        {
            var contextWrapper = this.CreateWrappedContext();

            var customer = contextWrapper.Context.Customer.Expand(c => c.Orders).First();
            var order = contextWrapper.Context.Order.ToArray().Except(customer.Orders).First();

            contextWrapper.AddLink(customer, "Orders", order);
            contextWrapper.SaveChanges();

            contextWrapper.DeleteLink(customer, "Orders", order);
            contextWrapper.SaveChanges();
        }

        [TestMethod]
        public void AddDeleteLinkSaveBatch()
        {
            var contextWrapper = this.CreateWrappedContext();

            var customer = contextWrapper.Context.Customer.Expand(c => c.Orders).First();
            var order1 = contextWrapper.Context.Order.ToArray().Except(customer.Orders).First();
            var order2 = contextWrapper.Context.Order.OrderByDescending(o => o.OrderId).ToArray().Except(customer.Orders).First();

            contextWrapper.AddLink(customer, "Orders", order1);
            contextWrapper.AddLink(customer, "Orders", order2);

            contextWrapper.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);

            contextWrapper.DeleteLink(customer, "Orders", order1);
            contextWrapper.DeleteLink(customer, "Orders", order2);
            contextWrapper.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);
        }

        [TestMethod]
        public void SetLinkSave()
        {
            var contextWrapper = this.CreateWrappedContext();

            var employee = contextWrapper.Context.Person.OfType<Employee>().First();

            contextWrapper.SetLink(employee, "Manager", employee);
            contextWrapper.SaveChanges();
        }

        [TestMethod]
        public void AddRelatedObjectSave()
        {
            var contextWrapper = this.CreateWrappedContext();

            var customer = contextWrapper.Context.Customer.First();
            var order = new Order { OrderId = 1234 };

            contextWrapper.AddRelatedObject(customer, "Orders", order);
            contextWrapper.SaveChanges();
        }
    }
}
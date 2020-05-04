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
    using Xunit;
    using Xunit.Abstractions;

    public class UpdateTests : KeyAsSegmentTest
    {
        const int IdOfPerson = -10;

        public UpdateTests(ITestOutputHelper helper)
            :base(helper)
        {
        }

        [Fact]
        public void InsertSave()
        {
            var newPerson = new Person { PersonId = 9999, Name = "Some Person" };

            var contextWrapper = this.CreateWrappedContext();
            var personQuery = contextWrapper.Context.Person.Where(p => p.PersonId == newPerson.PersonId);
            
            contextWrapper.IgnoreResourceNotFoundException = true;
            var retrievedPerson = personQuery.SingleOrDefault();
            Assert.Null(retrievedPerson);

            contextWrapper.AddObject("Person", newPerson);
            contextWrapper.SaveChanges();
            
            retrievedPerson = personQuery.SingleOrDefault();
            Assert.NotNull(retrievedPerson);
            Assert.True(newPerson == retrievedPerson, "New entity and retrieved entity should reference same object");
        }

        [Fact]
        public void AttachUpdateObjectSave()
        {
            var contextWrapper = this.CreateWrappedContext();
            var specialEmployee = new SpecialEmployee { PersonId = IdOfPerson };
            contextWrapper.AttachTo("Person", specialEmployee);

            specialEmployee.Bonus = Int32.MaxValue;
            contextWrapper.UpdateObject(specialEmployee);
            contextWrapper.SaveChanges();

            var retrievedBonus = contextWrapper.Context.Person.OfType<SpecialEmployee>().Where(p => p.PersonId == specialEmployee.PersonId).Select(p => p.Bonus);
            Assert.Equal(Int32.MaxValue, retrievedBonus.Single());
        }

        [Fact]
        public void AddObjectSave()
        {
            var contextWrapper = this.CreateWrappedContext();

            var specialEmployee = new SpecialEmployee { PersonId = 1234 };
            contextWrapper.AddObject("Person", specialEmployee);
            contextWrapper.SaveChanges();

            contextWrapper.DeleteObject(specialEmployee);
            contextWrapper.SaveChanges();
        }

        [Fact]
        public void AddObjectAddRelatedObjectSave()
        {
            var contextWrapper = this.CreateWrappedContext();

            var customer = new Customer { CustomerId = 1234 };
            var order = new Order { OrderId = 12345 };

            contextWrapper.AddObject("Customer", customer);
            contextWrapper.AddRelatedObject(customer, "Orders", order);
            contextWrapper.SaveChanges();
        }

        [Fact]
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

        [Fact]
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

        [Fact]
        public void SetLinkSave()
        {
            var contextWrapper = this.CreateWrappedContext();

            var employee = contextWrapper.Context.Person.OfType<Employee>().First();

            contextWrapper.SetLink(employee, "Manager", employee);
            contextWrapper.SaveChanges();
        }

        [Fact]
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
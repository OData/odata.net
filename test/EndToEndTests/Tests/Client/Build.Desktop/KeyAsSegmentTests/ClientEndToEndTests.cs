//---------------------------------------------------------------------
// <copyright file="ClientEndToEndTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.KeyAsSegmentTests
{
    using Microsoft.OData.Client;
    using System.Linq;
    using Microsoft.Test.OData.Services.TestServices.KeyAsSegmentServiceReference;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ClientEndToEndTests : KeyAsSegmentTest
    {
        const int IdOfPerson = -10;
        const int IdOfPersonMetadata = 10;
        const int IdOfEmployee = -6;
        const int IdOfEmployeeWithManager = -10;
        const int IdOfDiscontinuedProduct = -9;
        const int IdOfDiscontinuedProductWithRelatedDiscontinuedProducts = -9;
        const int IdOfDiscontinuedProductWithRelatedDiscontinuedProductsWithPhotos = -9;
        private int[] IdOfPhotoFromDiscontinuedProductWithRelatedDiscontinuedProductsWithPhotos = { -4, -4 };

        [TestMethod]
        public void GetSingleEntity()
        {
            var contextWrapper = this.CreateWrappedContext();
            var query = contextWrapper.CreateQuery<Person>("Person").Where(p => p.PersonId == IdOfPerson) as DataServiceQuery<Person>;
            var folder = query.SingleOrDefault();
            Assert.IsNotNull(folder);
        }

        [TestMethod]
        public void LinqQueryWithKeyUsingMethodSyntax()
        {
            var contextWrapper = this.CreateWrappedContext();
            var person = contextWrapper.Context.Person.Where(p => p.PersonId == IdOfPerson).SingleOrDefault();
            Assert.IsNotNull(person);
        }

        [TestMethod]
        public void LinqQueryWithNullStringInKey()
        {
            var contextWrapper = this.CreateWrappedContext();

            var query =
                from p in contextWrapper.Context.Person
                where p.Name == "\0te\0st\0"
                select p;

            var person = query.SingleOrDefault();

            Assert.IsNull(person);
        }

        [TestMethod]
        public void LinqQueryWithKey()
        {
            var contextWrapper = this.CreateWrappedContext();

            var query =
                from p in contextWrapper.Context.Person
                where p.PersonId == IdOfPerson
                select p;

            var person = query.SingleOrDefault();

            Assert.IsNotNull(person);
        }

        [TestMethod]
        public void LinqQueryWithKeyAndOfType()
        {
            var contextWrapper = this.CreateWrappedContext();

            var query = (
                from p in contextWrapper.Context.Person.OfType<Employee>()
                where p.PersonId == IdOfEmployee
                select p) as DataServiceQuery<Employee>;

            var employee = query.FirstOrDefault();

            Assert.IsNotNull(employee);
        }

        [TestMethod]
        public void MultipleNavigationAndOfTypeInQuery()
        {
            var contextWrapper = this.CreateWrappedContext();

            var photoQuery = (
                from product in contextWrapper.Context.Product.OfType<DiscontinuedProduct>()
                where product.ProductId == IdOfDiscontinuedProductWithRelatedDiscontinuedProducts

                from related in product.RelatedProducts.OfType<DiscontinuedProduct>()
                where related.ProductId == IdOfDiscontinuedProductWithRelatedDiscontinuedProductsWithPhotos

                from photo in related.Photos
                where photo.PhotoId == IdOfPhotoFromDiscontinuedProductWithRelatedDiscontinuedProductsWithPhotos[0] && photo.ProductId == IdOfPhotoFromDiscontinuedProductWithRelatedDiscontinuedProductsWithPhotos[1]

                select photo) as IQueryable<ProductPhoto>;

            Assert.IsNotNull(photoQuery.SingleOrDefault());
        }

        [TestMethod]
        public void AttachTo()
        {
            var contextWrapper = this.CreateWrappedContext();

            var person = new Person { PersonId = IdOfPerson };
            contextWrapper.AttachTo("Person", person);

            Assert.IsTrue(contextWrapper.Context.Entities.Count == 1);
        }

        [TestMethod]
        public void AttachToLoadProperty()
        {
            var contextWrapper = this.CreateWrappedContext();

            var person = new Person { PersonId = IdOfPerson };
            contextWrapper.AttachTo("Person", person);
            contextWrapper.LoadProperty(person, "PersonMetadata");

            Assert.AreEqual(3, person.PersonMetadata.Count, "person.PersonMetadata.Count == 3");
        }

        // [Ignore] // Issue: #623: Support DI in OData Client
        // [TestMethod] // github issuse: #896
        public void ContextReferencesTest()
        {
            var contextWrapper = this.CreateWrappedContext();
            Assert.AreEqual(0, contextWrapper.Context.Entities.Count(), "contextWrapper.Entities.Count() == 0");

            var entityFromAttachTo = new SpecialEmployee { PersonId = IdOfPerson };
            contextWrapper.AttachTo("Person", entityFromAttachTo);
            Assert.AreEqual(1, contextWrapper.Context.Entities.Count(), "contextWrapper.Entities.Count() == 1");

            var personMetadata = contextWrapper.Context.PersonMetadata.Where(m => m.Person.PersonId == IdOfPerson).FirstOrDefault();
            Assert.AreEqual(2, contextWrapper.Context.Entities.Count(), "contextWrapper.Entities.Count() == 2");

            contextWrapper.LoadProperty(personMetadata, "Person");
            var entityFromLoadProperty = personMetadata.Person;
            contextWrapper.Detach(personMetadata);
            Assert.AreEqual(1, contextWrapper.Context.Entities.Count(), "contextWrapper.Entities.Count() == 1");
            Assert.IsTrue(entityFromAttachTo == entityFromLoadProperty, "Both variables should reference the same object.");

            var entityFromQuery = contextWrapper.Context.Person.Where(p => p.PersonId == IdOfPerson).SingleOrDefault();
            Assert.AreEqual(1, contextWrapper.Context.Entities.Count(), "contextWrapper.Entities.Count() == 1");
            Assert.IsTrue(entityFromAttachTo == entityFromQuery, "Both variables should reference the same object.");

            var entityFromQueryWithOfType = contextWrapper.Context.Person.OfType<Employee>().Where(p => p.PersonId == IdOfPerson).SingleOrDefault();
            Assert.AreEqual(1, contextWrapper.Context.Entities.Count(), "contextWrapper.Entities.Count() == 1");
            Assert.IsTrue(entityFromAttachTo == entityFromQueryWithOfType, "Both variables should reference the same object.");
        }

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        [TestMethod]
        public void LoadPropertyWithNextLink()
        {
            var contextWrapper = this.CreateWrappedContext();

            var response = contextWrapper.Context.Customer.Expand(c => c.Orders).Execute() as QueryOperationResponse<Customer>;
            DataServiceQueryContinuation<Customer> customerContinuation = null;

            do
            {
                if (customerContinuation != null)
                {
                    response = contextWrapper.Execute<Customer>(customerContinuation);
                }

                foreach (var customer in response)
                {
                    DataServiceQueryContinuation<Order> orderContinuation = response.GetContinuation(customer.Orders);

                    while (orderContinuation != null)
                    {
                        var ordersResponse = contextWrapper.LoadProperty(customer, "Orders", orderContinuation);
                        orderContinuation = ordersResponse.GetContinuation();
                    }
                }

            } while ((customerContinuation = response.GetContinuation()) != null);
        }
#endif
    }
}

//---------------------------------------------------------------------
// <copyright file="ClientUpdateTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client
{
    using Microsoft.OData.Client;
    using System.Linq;
    using Microsoft.Test.OData.Framework;
    using Microsoft.Test.OData.Framework.Client;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.AstoriaDefaultServiceReference;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Generic client update test cases.
    /// </summary>
    [TestClass]
    public class ClientUpdateTests : EndToEndTestBase
    {
        public ClientUpdateTests()
            : base(ServiceDescriptors.AstoriaDefaultService)
        {
        }

        [TestMethod]
        public void SetEtagValueAfterQueryUpdate()
        {
            var contextWrapper = this.CreateWrappedContext();
            contextWrapper.MergeOption = MergeOption.PreserveChanges;

            var productToModify = contextWrapper.Context.Product.First();
            var productToDelete = contextWrapper.Context.Product.Skip(1).First();

            //caching Etags
            var productToModifyETag = contextWrapper.GetEntityDescriptor(productToModify).ETag;
            var productToDeleteETag = contextWrapper.GetEntityDescriptor(productToDelete).ETag;

            contextWrapper.UpdateObject(productToModify);
            productToModify.Description = "Modified Description";

            contextWrapper.DeleteObject(productToDelete);
            // We currently do not allow setting state from Deleted to Modified and LS is fine with the extra step to change State to Unchanged first
            contextWrapper.ChangeState(productToDelete, EntityStates.Unchanged);
            contextWrapper.ChangeState(productToDelete, EntityStates.Modified);

            //Updating entities in the store using a new Client context object
            var contextWrapperToUpdate = this.CreateWrappedContext();

            var product1 = contextWrapperToUpdate.Context.Product.First();
            var product2 = contextWrapperToUpdate.Context.Product.Skip(1).First();

            product1.Description = "New Description 1";
            product1.BaseConcurrency = "New Concurrency 1";
            product2.Description = "New Description 2";
            product2.BaseConcurrency = "New Concurrency 2";

            contextWrapperToUpdate.UpdateObject(product1);
            contextWrapperToUpdate.UpdateObject(product2);

            contextWrapperToUpdate.SaveChanges();

            //Quering the attached entities to update them
            contextWrapper.Context.Product.First();
            contextWrapper.Context.Product.Skip(1).First();

            Assert.AreEqual(contextWrapperToUpdate.GetEntityDescriptor(product1).ETag, contextWrapper.GetEntityDescriptor(productToModify).ETag, "ETag not updated by query");
            Assert.AreEqual(contextWrapperToUpdate.GetEntityDescriptor(product2).ETag, contextWrapper.GetEntityDescriptor(productToDelete).ETag, "ETag not updated by query");
            Assert.AreNotEqual(productToModify.Description, product1.Description, "Query updated entity in Modified State");
            Assert.AreNotEqual(productToDelete.Description, product2.Description, "Query updated entity in Modified State");

            contextWrapper.GetEntityDescriptor(productToModify).ETag = productToModifyETag;
            contextWrapper.GetEntityDescriptor(productToDelete).ETag = productToDeleteETag;

            Assert.AreEqual(productToModifyETag, contextWrapper.GetEntityDescriptor(productToModify).ETag, "Etag not updated");
            Assert.AreEqual(productToDeleteETag, contextWrapper.GetEntityDescriptor(productToDelete).ETag, "Etag not updated");

            contextWrapper.ChangeState(productToDelete, EntityStates.Deleted);
            Assert.AreEqual(EntityStates.Deleted, contextWrapper.Context.GetEntityDescriptor(productToDelete).State, "ChangeState API did not change the entity State form Modified to Deleted");
        }

        [TestMethod]
        public void BatchTest()
        {
            var contextWrapper = this.CreateWrappedContext();
            contextWrapper.MergeOption = MergeOption.PreserveChanges;
            var product1 = contextWrapper.Context.Product.First();
            var product2 = contextWrapper.Context.Product.Skip(1).First();

            product1.Description = "New Description 1";
            product2.Description = "New Description 2";

            contextWrapper.UpdateObject(product1);
            contextWrapper.UpdateObject(product2);

            contextWrapper.SaveChanges(SaveChangesOptions.BatchWithIndependentOperations);
        }

        [TestMethod]
        public void TrackEntitiesInAllPages()
        {
            var context = this.CreateWrappedContext().Context;
            var customerCount = context.Customer.Count();

            var customers = new DataServiceCollection<Customer>(context, context.Customer.GetAllPages(), TrackingMode.AutoChangeTracking, null, null, null);
            Assert.AreEqual(customerCount, customers.Count());
            context.Configurations.RequestPipeline.OnEntryEnding((args) =>
            {
                Assert.AreEqual(1, args.Entry.Properties.Count());
            });
            for (int i = 0; i < customers.Count(); i++)
            {
                customers[i].Name = "Customer" + i.ToString();
            }
            context.SaveChanges();

            //customers = new DataServiceCollection<Customer>(context.Customer.Take(1));
            //context.Configurations.RequestPipeline.OnEntryEnding((args) =>
            //{
            //    Assert.AreEqual(1, args.Entry.Properties.Count());
            //});
            //context.LoadPropertyAllPages(customers[0], "Orders");
            //for (int i = 0; i < customers[0].Orders.Count(); i++)
            //{
            //    if (customers[0].Orders[i].Concurrency == null)
            //    {
            //        customers[0].Orders[i].Concurrency = new ConcurrencyInfo();
            //    }
            //    customers[0].Orders[i].Concurrency.Token = "Order_ConCurrency_" + i.ToString();

            //}
            //context.SaveChanges();

            //context.MergeOption = MergeOption.OverwriteChanges;
            //context.LoadPropertyAllPages(customers[0], "Orders");
            //for (int i = 0; i < customers[0].Orders.Count(); i++)
            //{
            //    Assert.AreEqual(customers[0].Orders[i].Concurrency.Token, "Order_ConCurrency_" + i.ToString());
            //}
        }

        private DataServiceContextWrapper<DefaultContainer> CreateWrappedContext()
        {
            return this.CreateWrappedContext<DefaultContainer>();
        }
    }
}

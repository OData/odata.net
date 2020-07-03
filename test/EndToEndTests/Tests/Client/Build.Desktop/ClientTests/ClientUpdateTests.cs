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
    using Xunit.Abstractions;
    using Xunit;

    /// <summary>
    /// Generic client update test cases.
    /// </summary>
    public class ClientUpdateTests : EndToEndTestBase
    {
        public ClientUpdateTests(ITestOutputHelper helper)
            : base(ServiceDescriptors.AstoriaDefaultService, helper)
        {
        }

        [Fact]
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

        [Fact]
        public void TrackEntitiesInAllPages()
        {
            var context = this.CreateWrappedContext().Context;
            var customerCount = context.Customer.Count();

            var customers = new DataServiceCollection<Customer>(context, context.Customer.GetAllPages(), TrackingMode.AutoChangeTracking, null, null, null);
            Assert.Equal(customerCount, customers.Count());
            context.Configurations.RequestPipeline.OnEntryEnding((args) =>
            {
                Assert.Equal(1, args.Entry.Properties.Count());
            });
            for (int i = 0; i < customers.Count(); i++)
            {
                customers[i].Name = "Customer" + i.ToString();
            }
            context.SaveChanges();

            //customers = new DataServiceCollection<Customer>(context.Customer.Take(1));
            //context.Configurations.RequestPipeline.OnEntryEnding((args) =>
            //{
            //    Assert.Equal(1, args.Entry.Properties.Count());
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
            //    Assert.Equal(customers[0].Orders[i].Concurrency.Token, "Order_ConCurrency_" + i.ToString());
            //}
        }

        private DataServiceContextWrapper<DefaultContainer> CreateWrappedContext()
        {
            return this.CreateWrappedContext<DefaultContainer>();
        }
    }
}

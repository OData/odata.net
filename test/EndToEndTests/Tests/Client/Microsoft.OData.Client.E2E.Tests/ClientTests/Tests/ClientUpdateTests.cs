//-----------------------------------------------------------------------------
// <copyright file="ClientUpdateTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.TestCommon;
using Microsoft.OData.Client.E2E.Tests.ClientTests.Server;
using Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd.Default;
using Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd;
using Xunit;

namespace Microsoft.OData.Client.E2E.Tests.ClientTests.Tests
{
    public class ClientUpdateTests : EndToEndTestBase<ClientUpdateTests.TestsStartup>
    {
        private readonly Container _context;

        public class TestsStartup : TestStartupBase
        {
            public override void ConfigureServices(IServiceCollection services)
            {
                services.ConfigureControllers(typeof(ClientUpdateTestsController), typeof(MetadataController));

                services.AddControllers().AddOData(opt => opt.EnableQueryFeatures()
                    .AddRouteComponents("odata", CommonEndToEndEdmModel.GetEdmModel(), new DefaultODataBatchHandler()));
            }
        }

        public ClientUpdateTests(TestWebApplicationFactory<TestsStartup> fixture)
            : base(fixture)
        {
            _context = new Container(new Uri(Client.BaseAddress, "odata/"));
            _context.HttpClientFactory = HttpClientFactory;
        }

        [Fact]
        public void UpdateObject_ExecutesSuccessfully()
        { 
            _context.MergeOption = MergeOption.PreserveChanges;
            var product1 = _context.Products.First();
            var product2 = _context.Products.Skip(1).First();

            product1.Description = "New Description 1";
            product2.Description = "New Description 2";

            _context.UpdateObject(product1);
            _context.UpdateObject(product2);

            _context.SaveChanges(SaveChangesOptions.BatchWithIndependentOperations);

            var product1AterUpdate = _context.Products.First();
            Assert.Equal("New Description 1", product1AterUpdate.Description);

            var product2AfterUpdate = _context.Products.Skip(1).First();
            Assert.Equal("New Description 2", product2AfterUpdate.Description);
        }

        [Fact]
        public void TrackingAndValidatingEntitiesAcrossAllPages_ExecutesSuccessfully()
        {
            var customerCount = _context.Customers.Count();
            var customers = new DataServiceCollection<Common.Clients.EndToEnd.Customer>(_context, _context.Customers.GetAllPages(), TrackingMode.AutoChangeTracking, null, null, null);

            Assert.Equal(customerCount, customers.Count());

            _context.Configurations.RequestPipeline.OnEntryEnding((args) =>
            {
                Assert.Equal(1, args.Entry.Properties.Count());
            });

            for (int i = 0; i < customers.Count(); i++)
            {
                customers[i].Name = "Customer" + i.ToString();
            }

            _context.SaveChanges();
        }
    }
}

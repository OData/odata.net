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
        private readonly Uri _baseUri;
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
            _baseUri = new Uri(Client.BaseAddress, "odata/");
            _context = new Container(_baseUri)
            {
                HttpClientFactory = HttpClientFactory
            };

            ResetDataSource();
        }

        [Fact]
        public async Task UpdateObject_ExecutesSuccessfully()
        { 
            _context.MergeOption = MergeOption.PreserveChanges;
            var product1 = (await _context.Products.ExecuteAsync()).First();
            var product2 = (await ((DataServiceQuery<Common.Clients.EndToEnd.Product>)_context.Products.Skip(1)).ExecuteAsync()).First();

            product1.Description = "New Description 1";
            product2.Description = "New Description 2";

            _context.UpdateObject(product1);
            _context.UpdateObject(product2);

            await _context.SaveChangesAsync(SaveChangesOptions.BatchWithIndependentOperations);

            var product1AterUpdate = (await _context.Products.ExecuteAsync()).First();
            Assert.Equal("New Description 1", product1AterUpdate.Description);

            var product2AfterUpdate = (await ((DataServiceQuery<Common.Clients.EndToEnd.Product>)_context.Products.Skip(1)).ExecuteAsync()).First();
            Assert.Equal("New Description 2", product2AfterUpdate.Description);
        }

        [Fact]
        public async Task TrackingAndValidatingEntitiesAcrossAllPages_ExecutesSuccessfully()
        {
            var customerCount = (await _context.Customers.ExecuteAsync()).Count();
            var customers = new DataServiceCollection<Common.Clients.EndToEnd.Customer>(_context, await _context.Customers.GetAllPagesAsync(), TrackingMode.AutoChangeTracking, null, null, null);

            Assert.Equal(customerCount, customers.Count);

            _context.Configurations.RequestPipeline.OnEntryEnding((args) =>
            {
                Assert.Equal(1, args.Entry.Properties.Count());
            });

            for (int i = 0; i < customers.Count; i++)
            {
                Assert.NotEqual(customers[i].Name, "Customer" + i.ToString());
                customers[i].Name = "Customer" + i.ToString();
            }

            await _context.SaveChangesAsync();

            var updatedCustomers = new DataServiceCollection<Common.Clients.EndToEnd.Customer>(_context, await _context.Customers.GetAllPagesAsync(), TrackingMode.AutoChangeTracking, null, null, null);

            for (int i = 0; i < updatedCustomers.Count; i++)
            {
                customers[i].Name = "Customer" + i.ToString();
            }
        }

        private void ResetDataSource()
        {
            var actionUri = new Uri(_baseUri + "clientupdate/Default.ResetDataSource", UriKind.Absolute);
            _context.Execute(actionUri, "POST");
        }
    }
}

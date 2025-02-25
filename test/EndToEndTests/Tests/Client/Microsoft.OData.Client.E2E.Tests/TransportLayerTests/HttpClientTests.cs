//-----------------------------------------------------------------------------
// <copyright file="HttpClientTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.Tests.TransportLayerTests.Server;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd;
using Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd;
using Xunit;
using ClientEndToEndModel = Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd;

namespace Microsoft.OData.Client.E2E.Tests.TransportLayerTests
{
    public class HttpClientTests : EndToEndTestBase<HttpClientTests.TestsStartup>
    {
        private readonly Uri _baseUri;
        private readonly Container _context;

        public class TestsStartup : TestStartupBase
        {
            public override void ConfigureServices(IServiceCollection services)
            {
                services.ConfigureControllers(typeof(HttpClientController), typeof(MetadataController));

                services.AddControllers().AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(null)
                    .AddRouteComponents("odata", CommonEndToEndEdmModel.GetEdmModel(), new DefaultODataBatchHandler()));
            }
        }

        public HttpClientTests(TestWebApplicationFactory<TestsStartup> fixture)
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
        public async Task QueryForEntity_ShouldReturnResults()
        {
            var results = await _context.CreateQuery<ClientEndToEndModel.Product>("Products").ExecuteAsync();
            Assert.NotEmpty(results);
        }

        [Fact]
        public async Task InsertNewEntity_ShouldInsertEntity()
        {
            _context.AddToOrders(new ClientEndToEndModel.Order { OrderId = 993, });
            await _context.SaveChangesAsync();

            var createdOrderQuery = _context.Orders.ByKey(993);
            var createdOrder = await createdOrderQuery.GetValueAsync();
            Assert.Equal(993, createdOrder.OrderId);
        }

        [Fact]
        public async Task UpdateEntityWithPatch_ShouldUpdateEntitySuccessfully()
        {
            var product = _context.Products.First();
            var newProductDescription = "Foo " + Guid.NewGuid().ToString();
            product.Description = newProductDescription;
            _context.UpdateObject(product);

            await _context.SaveChangesAsync();

            var updatedProduct = _context.Products.First();
            Assert.Equal(newProductDescription, updatedProduct.Description);
        }

        [Fact]
        public async Task UpdateEntityWithReplaceUsingJson_ShouldUpdareTheEntity()
        {
            _context.Format.UseJson();

            var product = _context.Products.First();
            product.Description = "Foo " + Guid.NewGuid().ToString();
            _context.UpdateObject(product);

            await _context.SaveChangesAsync(SaveChangesOptions.ReplaceOnUpdate);
            var updateProduct = _context.Products.First();
            Assert.Equal(product.Description, updateProduct.Description);
        }

        [Fact]
        public async Task DeleteEntity_ShouldDeleteTheEntity()
        {
            var computer = _context.Computers.First();
            Assert.Equal(-10, computer.ComputerId);
            _context.DeleteObject(computer);
            await _context.SaveChangesAsync();

            var deletedComputer = _context.Computers.FirstOrDefault(c => c.ComputerId == -10);
            Assert.Null(deletedComputer);
        }

        [Fact]
        public async Task DoingMultipleChangesUsingBatch_ShouldExecuteSuccessfully()
        {
            _context.AddToOrders(new ClientEndToEndModel.Order { OrderId = 953, });
            var customerToDelete = _context.Customers.First();
            Assert.Equal(-10, customerToDelete.CustomerId);
            _context.DeleteObject(customerToDelete);

            var product = _context.Products.First();
            product.Description = "Foo " + Guid.NewGuid().ToString();
            _context.UpdateObject(product);

            await _context.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset);

            //Assert new order was created. 
            var createdOrder = await _context.Orders.ByKey(953).GetValueAsync();
            Assert.Equal(953, createdOrder.OrderId);

            //Assert customer was deleted
            var deletedCustomer = _context.Customers.FirstOrDefault(c => c.CustomerId == -10);
            Assert.Null(deletedCustomer);

            //Assert product was updated
            var updatedProduct = _context.Products.First();
            Assert.Equal(product.Description, updatedProduct.Description);
        }

        [Fact]
        public async Task DoingMultipleChangesWithoutBatch_ShouldExecuteSuccessfully()
        {
            _context.AddToOrders(new ClientEndToEndModel.Order { OrderId = 953, });
            _context.DeleteObject(_context.Customers.First());

            var product = _context.Products.First();
            product.Description = "Foo " + Guid.NewGuid().ToString();
            _context.UpdateObject(product);

            await _context.SaveChangesAsync();

            //Assert new order was created.
            var createdOrder = await _context.Orders.ByKey(953).GetValueAsync();
            Assert.Equal(953, createdOrder.OrderId);

            //Assert customer was deleted
            var deletedCustomer = _context.Customers.FirstOrDefault(c => c.CustomerId == -10);
            Assert.Null(deletedCustomer);

            //Assert product was updated
            var updatedProduct = _context.Products.First();
            Assert.Equal(product.Description, updatedProduct.Description);
        }

        [Fact]
        public async Task LoadProperty_ExecutesSuccessfully()
        {
            var product = _context.Products.First();

            var response = await _context.LoadPropertyAsync(product, "Description");
            Assert.Equal(200, response.StatusCode);
        }

        [Fact]
        public async Task BatchUpdatesAsync()
        {
            _context.AddObject("Products", new ClientEndToEndModel.Product { ProductId = 55443, Description = "My new product", });

            ClientEndToEndModel.Customer customerWithCustomerInfo = null;
            var query = _context.Customers.Expand(c => c.Info).Where(c => c.Info != null) as DataServiceQuery<ClientEndToEndModel.Customer>;
            var queryResult = await query.ExecuteAsync();
            customerWithCustomerInfo = queryResult.First();

            var customerInfo = customerWithCustomerInfo.Info;
            customerInfo.Information = "New Information " + Guid.NewGuid().ToString();
            _context.UpdateObject(customerInfo);

            _context.DetachLink(customerWithCustomerInfo, "Info", customerInfo);
            _context.UpdateObject(customerWithCustomerInfo);

            var response = await _context.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset);

            Assert.Equal(200, response.BatchStatusCode);
            Assert.Equal(3, response.Count());
        }

        [Fact]
        //Client should not append () to navigation property query URI
        public async Task ClientShouldNotAppendParenthesisToNavigationPropertyQueryUri()
        {
            var product = new ClientEndToEndModel.Product { ProductId = 55445, Description = "My new product" };
            _context.AddObject("Products", product);
            await _context.SaveChangesAsync();
            _context.Detach(product);
            _context.AttachTo("Products", product);
            //Collection
            var response = await _context.LoadPropertyAsync(product, "RelatedProducts");
            Assert.EndsWith("RelatedProducts", response.Query.RequestUri.AbsolutePath);
        }

        private void ResetDataSource()
        {
            var actionUri = new Uri(_baseUri + "httpclient/Default.ResetDataSource", UriKind.Absolute);
            _context.Execute(actionUri, "POST");
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="AsyncWasmCompatibilityTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.OData.Edm.Csdl;
using Xunit;

namespace Microsoft.OData.Client.Tests.Serialization
{
    /// <summary>
    /// Tests for the async-native WASM compatibility path that eliminates Task.Wait() blocking.
    /// These tests verify that GetResponseAsync, ExecuteAsync, GetAllPagesAsync, and
    /// EnumerateAllPagesAsync work correctly through the new async pipeline.
    /// </summary>
    public class AsyncWasmCompatibilityTests
    {
        private const string ServiceRoot = "http://localhost:9090";

        private const string Edmx = @"<edmx:Edmx xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"" Version=""4.0"">
  <edmx:DataServices>
    <Schema xmlns=""http://docs.oasis-open.org/odata/ns/edm"" Namespace=""Test"">
      <EntityType Name=""Product"">
        <Key>
          <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" />
      </EntityType>
    </Schema>
    <Schema xmlns=""http://docs.oasis-open.org/odata/ns/edm"" Namespace=""Default"">
      <EntityContainer Name=""Container"">
        <EntitySet Name=""Products"" EntityType=""Test.Product"" />
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

        private const string ProductsResponse = @"{
    ""@odata.context"": ""http://localhost:9090/$metadata#Products"",
    ""value"": [
        { ""Id"": 1, ""Name"": ""Widget"" },
        { ""Id"": 2, ""Name"": ""Gadget"" }
    ]
}";

        private const string ProductsPage1Response = @"{
    ""@odata.context"": ""http://localhost:9090/$metadata#Products"",
    ""value"": [
        { ""Id"": 1, ""Name"": ""Widget"" },
        { ""Id"": 2, ""Name"": ""Gadget"" }
    ],
    ""@odata.nextLink"": ""http://localhost:9090/Products?$skip=2""
}";

        private const string ProductsPage2Response = @"{
    ""@odata.context"": ""http://localhost:9090/$metadata#Products"",
    ""value"": [
        { ""Id"": 3, ""Name"": ""Doohickey"" }
    ]
}";

        #region GetResponseAsync Tests

        [Fact]
        public async Task GetResponseAsync_ReturnsResponse_WithoutBlocking()
        {
            // Arrange
            var context = CreateContext();
            SetupRequestPipeline(context, ProductsResponse);

            // Act
            var query = context.Products;
            var results = await query.ExecuteAsync();

            // Assert
            var products = results.ToList();
            Assert.Equal(2, products.Count);
            Assert.Equal("Widget", products[0].Name);
            Assert.Equal("Gadget", products[1].Name);
        }

        [Fact]
        public async Task GetResponseAsync_SupportsCancellation()
        {
            // Arrange
            var context = CreateContext();
            SetupRequestPipeline(context, ProductsResponse);
            var cts = new CancellationTokenSource();
            cts.Cancel(); // Pre-cancel

            // Act & Assert
            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => context.Products.ExecuteAsync(cts.Token));
        }

        [Fact]
        public async Task GetResponseAsync_WithHttpClientFactory_UsesAsyncPath()
        {
            // Arrange - MockHttpClientHandler returns proper OData JSON response
            using var handler = new MockHttpClientHandler(request =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(ProductsResponse, Encoding.UTF8, "application/json")
                };
                return response;
            });
            var factory = new MockHttpClientFactory(handler);

            var context = CreateContext();
            context.HttpClientFactory = factory;

            // Act
            var results = await context.Products.ExecuteAsync();

            // Assert
            var products = results.ToList();
            Assert.Equal(2, products.Count);
            Assert.Equal(1, factory.NumCalls);
            Assert.Single(handler.Requests);
            Assert.Contains("Products", handler.Requests[0]);
        }

        #endregion

        #region ExecuteAsync (DataServiceQuery) Tests

        [Fact]
        public async Task ExecuteAsync_WithCancellationToken_ReturnsResults()
        {
            // Arrange
            var context = CreateContext();
            SetupRequestPipeline(context, ProductsResponse);

            // Act
            var results = await context.Products.ExecuteAsync(CancellationToken.None);

            // Assert
            Assert.Equal(2, results.Count());
        }

        [Fact]
        public async Task ExecuteAsync_ForFunction_UsesContextExecuteAsync()
        {
            // Arrange - simulate function call via ExecuteAsync on context
            var context = CreateContext();
            SetupRequestPipeline(context, ProductsResponse);

            // Act - use Context.ExecuteAsync directly (the function path)
            var results = await context.ExecuteAsync<Product>(
                new Uri($"{ServiceRoot}/Products"), CancellationToken.None);

            // Assert
            Assert.Equal(2, results.Count());
        }

        #endregion

        #region GetAllPagesAsync Tests

        [Fact]
        public async Task GetAllPagesAsync_IteratesAllPages()
        {
            // Arrange
            int requestCount = 0;
            var context = CreateContext();
            context.Configurations.RequestPipeline.OnMessageCreating = (args) =>
            {
                requestCount++;
                string response = requestCount == 1 ? ProductsPage1Response : ProductsPage2Response;
                return new AsyncTestRequestMessage(args, response);
            };

            // Act
            var results = await context.Products.GetAllPagesAsync();

            // Assert
            var products = results.ToList();
            Assert.Equal(3, products.Count);
            Assert.Equal("Widget", products[0].Name);
            Assert.Equal("Gadget", products[1].Name);
            Assert.Equal("Doohickey", products[2].Name);
            Assert.Equal(2, requestCount);
        }

        [Fact]
        public async Task GetAllPagesAsync_WithCancellation_Throws()
        {
            // Arrange
            var context = CreateContext();
            SetupRequestPipeline(context, ProductsPage1Response);
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act & Assert
            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => context.Products.GetAllPagesAsync(cts.Token));
        }

        #endregion

        #region EnumerateAllPagesAsync Tests

        [Fact]
        public async Task EnumerateAllPagesAsync_YieldsElementsLazily()
        {
            // Arrange
            int requestCount = 0;
            var context = CreateContext();
            context.Configurations.RequestPipeline.OnMessageCreating = (args) =>
            {
                requestCount++;
                string response = requestCount == 1 ? ProductsPage1Response : ProductsPage2Response;
                return new AsyncTestRequestMessage(args, response);
            };

            // Act
            var elements = new List<Product>();
            await foreach (var product in context.Products.EnumerateAllPagesAsync())
            {
                elements.Add(product);
            }

            // Assert
            Assert.Equal(3, elements.Count);
            Assert.Equal("Doohickey", elements[2].Name);
            Assert.Equal(2, requestCount);
        }

        [Fact]
        public async Task EnumerateAllPagesAsync_SupportsCancellation()
        {
            // Arrange
            var context = CreateContext();
            SetupRequestPipeline(context, ProductsPage1Response);
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act & Assert
            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
            {
                await foreach (var _ in context.Products.EnumerateAllPagesAsync(cts.Token))
                {
                    // Should not reach here
                }
            });
        }

        #endregion

        #region DataServiceContext.ExecuteAsync Tests

        [Fact]
        public async Task Context_ExecuteAsync_WithHttpMethod_ReturnsResults()
        {
            // Arrange
            var context = CreateContext();
            SetupRequestPipeline(context, ProductsResponse);

            // Act
            var results = await context.ExecuteAsync<Product>(
                new Uri($"{ServiceRoot}/Products"), "GET", true, CancellationToken.None);

            // Assert
            Assert.Equal(2, results.Count());
        }

        [Fact]
        public async Task Context_ExecuteAsync_Continuation_ReturnsNextPage()
        {
            // Arrange
            int requestCount = 0;
            var context = CreateContext();
            context.Configurations.RequestPipeline.OnMessageCreating = (args) =>
            {
                requestCount++;
                string response = requestCount == 1 ? ProductsPage1Response : ProductsPage2Response;
                return new AsyncTestRequestMessage(args, response);
            };

            // Get first page
            var firstPage = (QueryOperationResponse<Product>)await context.Products.ExecuteAsync();
            var firstPageResults = firstPage.ToList();
            Assert.Equal(2, firstPageResults.Count);

            var continuation = firstPage.GetContinuation();
            Assert.NotNull(continuation);

            // Act
            var nextPage = await context.ExecuteAsync(continuation, CancellationToken.None);

            // Assert
            var products = nextPage.ToList();
            Assert.Single(products);
            Assert.Equal("Doohickey", products[0].Name);
        }

        #endregion

        #region DataServiceQuerySingle.GetValueAsync Tests

        [Fact]
        public async Task GetValueAsync_ReturnsEntity_WithAsyncPath()
        {
            // Arrange
            string singleProductResponse = @"{
    ""@odata.context"": ""http://localhost:9090/$metadata#Products/$entity"",
    ""Id"": 1,
    ""Name"": ""Widget""
}";
            var context = CreateContext();
            SetupRequestPipeline(context, singleProductResponse);

            var querySingle = new DataServiceQuerySingle<Product>(context, "Products(1)");

            // Act
            var product = await querySingle.GetValueAsync(CancellationToken.None);

            // Assert
            Assert.NotNull(product);
            Assert.Equal(1, product.Id);
            Assert.Equal("Widget", product.Name);
        }

        #endregion

        #region Helper Methods

        private TestContainer CreateContext()
        {
            return new TestContainer(new Uri(ServiceRoot));
        }

        private void SetupRequestPipeline(DataServiceContext context, string response)
        {
            context.Configurations.RequestPipeline.OnMessageCreating = (args) =>
                new AsyncTestRequestMessage(args, response);
        }

        #endregion

        #region Test Types

        [Key("Id")]
        public class Product
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class TestContainer : DataServiceContext
        {
            public TestContainer(Uri serviceRoot) : base(serviceRoot, ODataProtocolVersion.V4)
            {
                Format.LoadServiceModel = () => CsdlReader.Parse(XmlReader.Create(new StringReader(Edmx)));
                Format.UseJson();
                Products = base.CreateQuery<Product>("Products");
            }

            public DataServiceQuery<Product> Products { get; private set; }
        }

        /// <summary>
        /// A test request message that overrides GetResponseAsync to verify the async path works
        /// without making real HTTP calls (simulating a WASM-compatible environment).
        /// </summary>
        private class AsyncTestRequestMessage : HttpClientRequestMessage
        {
            private readonly string _response;

            public AsyncTestRequestMessage(DataServiceClientRequestMessageArgs args, string response)
                : base(args)
            {
                _response = response;
            }

            public override IODataResponseMessage GetResponse()
            {
                return CreateMockResponse();
            }

            public override Task<IODataResponseMessage> GetResponseAsync(CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();
                return Task.FromResult(GetResponse());
            }

            public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
            {
                var tcs = new TaskCompletionSource<bool>(state);
                tcs.TrySetResult(true);
                callback(tcs.Task);
                return tcs.Task;
            }

            public override IODataResponseMessage EndGetResponse(IAsyncResult asyncResult)
            {
                return GetResponse();
            }

            private IODataResponseMessage CreateMockResponse()
            {
                return new HttpWebResponseMessage(
                    new Dictionary<string, string> { { "Content-Type", "application/json;charset=utf-8" } },
                    200,
                    () => new MemoryStream(Encoding.UTF8.GetBytes(_response)),
                    null);
            }
        }

        #endregion
    }
}

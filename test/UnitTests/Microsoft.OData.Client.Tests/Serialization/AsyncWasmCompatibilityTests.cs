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
    /// These tests verify that query, paging, stream, and save operations use the native async pipeline.
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
        <Property Name=""Photo"" Type=""Edm.Stream"" />
        <NavigationProperty Name=""Category"" Type=""Test.Category"" />
        <NavigationProperty Name=""Categories"" Type=""Collection(Test.Category)"" />
      </EntityType>
      <EntityType Name=""Category"">
        <Key>
          <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" />
      </EntityType>
      <EntityType Name=""Document"" HasStream=""true"">
        <Key>
          <PropertyRef Name=""Id"" />
        </Key>
        <Property Name=""Id"" Type=""Edm.Int32"" Nullable=""false"" />
      </EntityType>
    </Schema>
    <Schema xmlns=""http://docs.oasis-open.org/odata/ns/edm"" Namespace=""Default"">
      <EntityContainer Name=""Container"">
        <EntitySet Name=""Products"" EntityType=""Test.Product"" />
        <EntitySet Name=""Categories"" EntityType=""Test.Category"" />
        <EntitySet Name=""Documents"" EntityType=""Test.Document"" />
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

        [Fact]
        public async Task GetAllPagesAsync_WithReusedCancellationToken_LeavesNoLingeringRegistrations()
        {
            // Regression test for issue #3583: repeatedly paging with a single, long-lived
            // CancellationToken must not accumulate cancellation-token registrations. Each leaked
            // registration keeps the request/response graph alive until the token source is disposed,
            // so a long-running job that reuses one token would grow memory without bound.
            // Arrange
            int requestCount = 0;
            var context = CreateContext();
            context.Configurations.RequestPipeline.OnMessageCreating = (args) =>
            {
                requestCount++;
                // Two pages per call: odd requests return page 1 (with next link), even return page 2.
                string response = (requestCount % 2 == 1) ? ProductsPage1Response : ProductsPage2Response;
                return new AsyncTestRequestMessage(args, response);
            };

            using var cts = new CancellationTokenSource();

            // Act - simulate a long-running job that reuses the same token across many calls.
            for (int i = 0; i < 5; i++)
            {
                var products = (await context.Products.GetAllPagesAsync(cts.Token)).ToList();
                Assert.Equal(3, products.Count);
            }

            // Cancelling the shared token after all requests have completed must not reach into any
            // completed request. A lingering registration would invoke CancelRequest here.
            cts.Cancel();

            // Assert
            Assert.Equal(0, context.CancelRequestCount);
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

        #region Remaining Async API Tests

        [Fact]
        public async Task SaveChangesAsync_UpdatesEntity_WithoutUsingApm()
        {
            var context = CreateContext();
            SetupRequestPipeline(context, string.Empty, 204, null);
            var product = new Product { Id = 1, Name = "Updated" };
            context.AttachTo("Products", product);
            context.UpdateObject(product);
            bool changesSaved = false;
            context.ChangesSaved += (_, _) => changesSaved = true;

            DataServiceResponse response = await context.SaveChangesAsync();

            ChangeOperationResponse operationResponse = Assert.IsType<ChangeOperationResponse>(Assert.Single(response));
            Assert.Equal(204, operationResponse.StatusCode);
            Assert.True(changesSaved);
        }

        [Fact]
        public async Task SaveChangesAsync_BatchWithPreCanceledToken_DoesNotUseApm()
        {
            var context = CreateContext();
            SetupRequestPipeline(context, string.Empty);
            context.AddObject("Products", new Product { Id = 3, Name = "New" });
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => context.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset, cancellationTokenSource.Token));
        }

        [Fact]
        public async Task ExecuteBatchAsync_WithPreCanceledToken_DoesNotUseApm()
        {
            var context = CreateContext();
            SetupRequestPipeline(context, string.Empty);
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => context.ExecuteBatchAsync(cancellationTokenSource.Token, context.Products));
        }

        [Fact]
        public async Task ExecuteBatchAsync_WithEmptyQueries_Throws()
        {
            var context = CreateContext();

            await Assert.ThrowsAsync<ArgumentException>(
                () => context.ExecuteBatchAsync(Array.Empty<DataServiceRequest>()));
        }

        [Fact]
        public async Task LoadPropertyAsync_LoadsNavigationProperty_WithoutUsingApm()
        {
            const string categoryResponse = @"{
    ""@odata.context"": ""http://localhost:9090/$metadata#Categories/$entity"",
    ""Id"": 7,
    ""Name"": ""Hardware""
}";
            var context = CreateContext();
            SetupRequestPipeline(context, categoryResponse);
            var product = new Product { Id = 1, Name = "Widget" };
            context.AttachTo("Products", product);

            QueryOperationResponse response = await context.LoadPropertyAsync(product, nameof(Product.Category));

            Assert.NotNull(response);
            Assert.NotNull(product.Category);
            Assert.Equal(7, product.Category.Id);
        }

        [Fact]
        public async Task LoadPropertyAsync_WithNextLink_LoadsCollection_WithoutUsingApm()
        {
            const string categoriesResponse = @"{
    ""@odata.context"": ""http://localhost:9090/$metadata#Categories"",
    ""value"": [
        { ""Id"": 7, ""Name"": ""Hardware"" }
    ]
}";
            var context = CreateContext();
            SetupRequestPipeline(context, categoriesResponse);
            var product = new Product { Id = 1, Name = "Widget", Categories = new List<Category>() };
            context.AttachTo("Products", product);

            QueryOperationResponse response = await context.LoadPropertyAsync(
                product,
                nameof(Product.Categories),
                new Uri($"{ServiceRoot}/Products(1)/Categories?$skip=1"));

            Assert.NotNull(response);
            Assert.Single(product.Categories);
        }

        [Fact]
        public async Task LoadPropertyAllPagesAsync_LoadsContinuations_WithoutUsingApm()
        {
            const string firstPageResponse = @"{
    ""@odata.context"": ""http://localhost:9090/$metadata#Categories"",
    ""value"": [
        { ""Id"": 7, ""Name"": ""Hardware"" }
    ],
    ""@odata.nextLink"": ""http://localhost:9090/Products(1)/Categories?$skip=1""
}";
            const string secondPageResponse = @"{
    ""@odata.context"": ""http://localhost:9090/$metadata#Categories"",
    ""value"": [
        { ""Id"": 8, ""Name"": ""Software"" }
    ]
}";
            int requestCount = 0;
            var context = CreateContext();
            context.Configurations.RequestPipeline.OnMessageCreating = args =>
                new AsyncTestRequestMessage(args, ++requestCount == 1 ? firstPageResponse : secondPageResponse);
            var product = new Product { Id = 1, Name = "Widget", Categories = new List<Category>() };
            context.AttachTo("Products", product);

            QueryOperationResponse response = await context.LoadPropertyAllPagesAsync(
                product,
                nameof(Product.Categories),
                CancellationToken.None);

            Assert.NotNull(response);
            Assert.Equal(2, product.Categories.Count);
            Assert.Equal(2, requestCount);
        }

        [Fact]
        public async Task LoadPropertyAsync_WithNullContinuation_Throws()
        {
            var context = CreateContext();
            var product = new Product { Id = 1, Name = "Widget", Categories = new List<Category>() };
            context.AttachTo("Products", product);

            await Assert.ThrowsAsync<ArgumentNullException>(
                () => context.LoadPropertyAsync(
                    product,
                    nameof(Product.Categories),
                    continuation: null,
                    CancellationToken.None));
        }

        [Fact]
        public async Task GetReadStreamAsync_ReturnsStream_WithoutUsingApm()
        {
            byte[] payload = Encoding.UTF8.GetBytes("stream content");
            var context = CreateContext();
            context.Configurations.RequestPipeline.OnMessageCreating = args =>
                new AsyncTestRequestMessage(args, payload, 200, "application/octet-stream");
            var document = new Document { Id = 1 };
            context.AttachTo("Documents", document);
            context.GetEntityDescriptor(document).ReadStreamUri = new Uri($"{ServiceRoot}/Documents(1)/$value");

            using DataServiceStreamResponse response = await context.GetReadStreamAsync(
                document,
                new DataServiceRequestArgs());
            using var reader = new StreamReader(response.Stream);

            Assert.Equal("application/octet-stream", response.ContentType);
            Assert.Equal("stream content", await reader.ReadToEndAsync());
        }

        [Fact]
        public async Task GetReadStreamAsync_WithNamedStream_ReturnsStream_WithoutUsingApm()
        {
            byte[] payload = Encoding.UTF8.GetBytes("photo content");
            var context = CreateContext();
            context.Configurations.RequestPipeline.OnMessageCreating = args =>
                new AsyncTestRequestMessage(args, payload, 200, "image/png");
            var product = new Product { Id = 1, Name = "Widget" };
            context.AttachTo("Products", product);
            context.GetEntityDescriptor(product).AddStreamInfoIfNotPresent(nameof(Product.Photo)).SelfLink =
                new Uri($"{ServiceRoot}/Products(1)/Photo");

            using DataServiceStreamResponse response = await context.GetReadStreamAsync(
                product,
                nameof(Product.Photo),
                new DataServiceRequestArgs(),
                CancellationToken.None);
            using var reader = new StreamReader(response.Stream);

            Assert.Equal("image/png", response.ContentType);
            Assert.Equal("photo content", await reader.ReadToEndAsync());
        }

        [Fact]
        public async Task BulkUpdateAsync_WithPreCanceledToken_DoesNotUseApm()
        {
            var context = CreateContext();
            SetupRequestPipeline(context, string.Empty);
            var product = new Product { Id = 1, Name = "Updated" };
            context.AttachTo("Products", product);
            context.UpdateObject(product);
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => context.BulkUpdateAsync(cancellationTokenSource.Token, product));
        }

        [Fact]
        public async Task DeepInsertAsync_WithPreCanceledToken_DoesNotUseApm()
        {
            var context = CreateContext();
            SetupRequestPipeline(context, string.Empty);
            var product = new Product { Id = 3, Name = "New" };
            context.AddObject("Products", product);
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => context.DeepInsertAsync(product, cancellationTokenSource.Token));
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

        private void SetupRequestPipeline(DataServiceContext context, string response, int statusCode, string contentType)
        {
            context.Configurations.RequestPipeline.OnMessageCreating = (args) =>
                new AsyncTestRequestMessage(args, response, statusCode, contentType);
        }

        #endregion

        #region Test Types

        [Key("Id")]
        public class Product
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DataServiceStreamLink Photo { get; set; }
            public Category Category { get; set; }
            public List<Category> Categories { get; set; }
        }

        [Key("Id")]
        public class Category
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        [Key("Id")]
        [HasStream]
        public class Document
        {
            public int Id { get; set; }
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

            /// <summary>
            /// Number of times <see cref="CancelRequest"/> was invoked. Used by tests to detect
            /// cancellation-token registrations that outlive a completed request (issue #3583).
            /// </summary>
            public int CancelRequestCount { get; private set; }

            public override void CancelRequest(IAsyncResult asyncResult)
            {
                this.CancelRequestCount++;
                base.CancelRequest(asyncResult);
            }
        }

        /// <summary>
        /// A test request message that overrides GetResponseAsync to verify the async path works
        /// without making real HTTP calls (simulating a WASM-compatible environment).
        /// </summary>
        private class AsyncTestRequestMessage : HttpClientRequestMessage
        {
            private readonly byte[] _response;
            private readonly int _statusCode;
            private readonly string _contentType;

            public AsyncTestRequestMessage(DataServiceClientRequestMessageArgs args, string response)
                : this(args, Encoding.UTF8.GetBytes(response), 200, "application/json;charset=utf-8")
            {
            }

            public AsyncTestRequestMessage(DataServiceClientRequestMessageArgs args, string response, int statusCode, string contentType = "application/json;charset=utf-8")
                : this(args, Encoding.UTF8.GetBytes(response), statusCode, contentType)
            {
            }

            public AsyncTestRequestMessage(DataServiceClientRequestMessageArgs args, byte[] response, int statusCode, string contentType)
                : base(args)
            {
                _response = response;
                _statusCode = statusCode;
                _contentType = contentType;
            }

            public override IODataResponseMessage GetResponse()
            {
                throw new NotSupportedException("Synchronous response APIs are unavailable on WebAssembly.");
            }

            public override Task<IODataResponseMessage> GetResponseAsync(CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();
                return Task.FromResult(CreateMockResponse());
            }

            public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
            {
                throw new NotSupportedException("APM response APIs are unavailable on WebAssembly.");
            }

            public override IODataResponseMessage EndGetResponse(IAsyncResult asyncResult)
            {
                throw new NotSupportedException("APM response APIs are unavailable on WebAssembly.");
            }

            private IODataResponseMessage CreateMockResponse()
            {
                var headers = new Dictionary<string, string>();
                if (_contentType != null)
                {
                    headers.Add("Content-Type", _contentType);
                }

                return new HttpWebResponseMessage(
                    headers,
                    _statusCode,
                    () => new MemoryStream(_response),
                    null);
            }
        }

        #endregion
    }
}

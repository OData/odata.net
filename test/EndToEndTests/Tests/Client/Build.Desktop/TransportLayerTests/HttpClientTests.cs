//---------------------------------------------------------------------
// <copyright file="HttpClientTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.TransportLayerTests
{
    using System;
    using Microsoft.OData.Client;
    using System.Linq;
    using Microsoft.Test.OData.Framework.Client;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.AstoriaDefaultServiceReference;
    using Xunit.Abstractions;
    using Xunit;

    public class HttpClientTests : EndToEndTestBase
    {
        public HttpClientTests(ITestOutputHelper helper)
            : base(ServiceDescriptors.AstoriaDefaultService, helper)
        {
        }

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        [Fact]
        public void SimpleQuery()
        {
            var ctx = this.CreateContext();
            var results = ctx.CreateQuery<Product>("Product").ToList();
            Assert.True(results.Count > 0, "No results returned");
        }

        [Fact]
        public void InsertNewEntity()
        {
            var ctx = this.CreateContext();
            ctx.Context.AddToOrder(new Order { OrderId = 993, });
            ctx.SaveChanges();
        }

        [Fact]
        public void UpdateEntityWithPatch()
        {
            var ctx = this.CreateContext();
            var product = ctx.Context.Product.First();
            product.Description = "Foo " + Guid.NewGuid().ToString();
            ctx.UpdateObject(product);

            ctx.SaveChanges();
        }

        [Fact]
        public void UpdateEntityWithReplaceUsingJson()
        {
            var ctx = this.CreateContext();
            ctx.Format.UseJson();

            var product = ctx.Context.Product.First();
            product.Description = "Foo " + Guid.NewGuid().ToString();
            ctx.UpdateObject(product);

            ctx.SaveChanges(SaveChangesOptions.ReplaceOnUpdate);
        }

        [Fact]
        public void DeleteEntity()
        {
            var ctx = this.CreateContext();
            ctx.DeleteObject(ctx.Context.Computer.First());
            ctx.SaveChanges();
        }

        [Fact]
        public void MultipleChangesBatch()
        {
            var ctx = this.CreateContext();
            ctx.Context.AddToOrder(new Order { OrderId = 953, });
            ctx.DeleteObject(ctx.Context.Customer.First());

            var product = ctx.Context.Product.First();
            product.Description = "Foo " + Guid.NewGuid().ToString();
            ctx.UpdateObject(product);

            ctx.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);
        }

        [Fact]
        public void MultipleChangesNonBatch()
        {
            var ctx = this.CreateContext();
            ctx.Context.AddToOrder(new Order { OrderId = 953, });
            ctx.DeleteObject(ctx.Context.Customer.First());

            var product = ctx.Context.Product.First();
            product.Description = "Foo " + Guid.NewGuid().ToString();
            ctx.UpdateObject(product);

            ctx.SaveChanges();
        }

        [Fact]
        public void LoadProperty()
        {
            var ctx = this.CreateContext();
            var product = ctx.Context.Product.First();

            var response = ctx.LoadProperty(product, "Description");
            Assert.Equal(200, response.StatusCode);
            Assert.Null(response.Error);
        }
#endif

        [Fact]
        public void SimpleQueryAsync()
        {
            var ctx = this.CreateContext();
            var query = ctx.CreateQuery<Product>("Product");
            query.BeginExecute(
                (ar) =>
                {
                    var products = query.EndExecute(ar).ToList();
                    Assert.True(products.Count > 0);
                    this.EnqueueTestComplete();
                },
                null);

            this.WaitForTestToComplete();
        }

        [Fact]
        public void UpdateEntityAsync()
        {
            var ctx = this.CreateContext();
            Computer computer = null;
            var query = ctx.Context.Computer.Take(1) as DataServiceQuery<Computer>;
            var queryResult = query.BeginExecute(null, null).EnqueueWait(this);
            computer = query.EndExecute(queryResult).Single();

            computer.Name = "New Name " + Guid.NewGuid().ToString();
            ctx.UpdateObject(computer);

            ctx.BeginSaveChanges(
                (ar) =>
                {
                    var response = ctx.EndSaveChanges(ar);
                    Assert.Equal(204, response.Single().StatusCode);
                    this.EnqueueTestComplete();
                },
                null);

            this.WaitForTestToComplete();
        }

        [Fact]
        public void BatchQueriesWithJsonAsync()
        {
            var ctx = this.CreateContext();
            ctx.Format.UseJson();

            var queries = new DataServiceQuery[]
            {
                ctx.CreateQuery<Product>("Product"),
                ctx.Context.Customer.Expand((c) => c.Orders),
            };

            ctx.BeginExecuteBatch(
                (ar) =>
                {
                    var response = ctx.EndExecuteBatch(ar);
                    Assert.True(response.IsBatchResponse, "Non-batch response received");
                    Assert.Equal(2, response.Count());
                    this.EnqueueTestComplete();
                },
                null,
                queries);

            this.WaitForTestToComplete();
        }

        [Fact]
        public void LoadPropertyAsync()
        {
            var ctx = this.CreateContext();
            Product product = null;
            var query = ctx.Context.Product.Take(1) as DataServiceQuery<Product>;
            var queryResult = query.BeginExecute(null, null).EnqueueWait(this);
            product = query.EndExecute(queryResult).Single();

            ctx.BeginLoadProperty(
                product,
                "Description",
                (ar) =>
                {
                    var propertyResponse = ctx.EndLoadProperty(ar);
                    Assert.Equal(200, propertyResponse.StatusCode);
                    Assert.Null(propertyResponse.Error);
                    this.EnqueueTestComplete();
                },
                null);

            this.WaitForTestToComplete();
        }

        [Fact]
        public void BatchUpdatesAsync()
        {
            var ctx = this.CreateContext();

            ctx.AddObject("Product", new Product { ProductId = 55443, Description = "My new product", });

            Customer customerWithCustomerInfo = null;
            var query = ctx.Context.Customer.Expand(c => c.Info).Where(c => c.Info != null) as DataServiceQuery<Customer>;
            var queryResult = query.BeginExecute(null, null).EnqueueWait(this);
            customerWithCustomerInfo = query.EndExecute(queryResult).First();

            var customerInfo = customerWithCustomerInfo.Info;
            customerInfo.Information = "New Information " + Guid.NewGuid().ToString();
            ctx.UpdateObject(customerInfo);

            ctx.DetachLink(customerWithCustomerInfo, "Info", customerInfo);
            ctx.UpdateObject(customerWithCustomerInfo);

            ctx.BeginSaveChanges(
                SaveChangesOptions.BatchWithSingleChangeset,
                (ar) =>
                { 
                    var response = ctx.EndSaveChanges(ar);
                    Assert.Equal(202, response.BatchStatusCode);
                    Assert.Equal(3, response.Count());
                    this.EnqueueTestComplete();
                },
                null);

            this.WaitForTestToComplete();
        }

        [Fact]
        public void MultipleUpdatesWithPostTunnelingAsync()
        {
            var ctx = this.CreateContext();
            ctx.UsePostTunneling = true;

            Product product = null;
            var productQuery = ctx.Context.Product as DataServiceQuery<Product>;
            var productQueryResult = productQuery.BeginExecute(null, null).EnqueueWait(this);
            product = productQuery.EndExecute(productQueryResult).First();

            Order orderWithLogin = null;
            Login anotherLogin = null;

            var orderQuery = ctx.Context.Order.Expand(o => o.Login).Where(o => o.Login != null) as DataServiceQuery<Order>;
            var orderQueryResult = orderQuery.BeginExecute(null, null).EnqueueWait(this);
            orderWithLogin = orderQuery.EndExecute(orderQueryResult).First();

            var loginQuery = ctx.Context.Login as DataServiceQuery<Login>;
            var loginQueryResult = loginQuery.BeginExecute(null, null).EnqueueWait(this);
            anotherLogin = loginQuery.EndExecute(loginQueryResult).First(l => l != orderWithLogin.Login);

            ctx.DeleteObject(product);
            ctx.SetLink(orderWithLogin, "Login", anotherLogin);

            ctx.BeginSaveChanges((ar) =>
                {
                    var response = ctx.EndSaveChanges(ar);
                    Assert.Equal(2, response.Count());
                    Assert.True(response.All(r => r.Error == null), "No errors expected");
                    Assert.True(response.All(r => r.StatusCode == 204), "Unexpected status code");
                    this.EnqueueTestComplete();
                }, 
                null);

            this.WaitForTestToComplete();
        }

        [Fact]
        //Client should not append () to navigation property query URI
        public void ClientShouldAppendParenthesisToNavigationPropertyQueryUri()
        {
            var ctx = this.CreateContext();
            var product = new Product { ProductId = 55445, Description = "My new product" };
            ctx.AddObject("Product", product);
            var addObjectResult = ctx.BeginSaveChanges(null, null).EnqueueWait(this);
            ctx.EndSaveChanges(addObjectResult);
            ctx.Detach(product);
            ctx.AttachTo("Product", product);
            //Collection
            var loadPropertyResult = ctx.BeginLoadProperty(product, "RelatedProducts", null, null).EnqueueWait(this);
            var response = ctx.EndLoadProperty(loadPropertyResult) as QueryOperationResponse;
            Assert.True(response.Query.RequestUri.AbsolutePath.EndsWith("RelatedProducts"));
        }

        private DataServiceContextWrapper<DefaultContainer> CreateContext()
        {
            var wrappedContext = this.CreateWrappedContext<DefaultContainer>();
            wrappedContext.Configurations.RequestPipeline.OnMessageCreating =
                (args) =>
                {
                    var message = new HttpClientRequestMessage(args.ActualMethod)
                    {
                        Url = args.RequestUri,
                        Method = args.Method,
                    };

                    foreach (var header in args.Headers)
                    {
                        message.SetHeader(header.Key, header.Value);
                    }

                    return message;
                };

            return wrappedContext;
        }
    }
}

//-----------------------------------------------------------------------------
// <copyright file="AsyncRegressionTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System.Reflection;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.Tests.AsyncRegressionTests.Client;
using Microsoft.OData.Client.E2E.Tests.AsyncRegressionTests.Default;
using Microsoft.OData.E2E.TestCommon;
using Xunit;

namespace Microsoft.OData.Client.E2E.Tests.AsyncRegressionTests;

public class AsyncRegressionTests : EndToEndTestBase<AsyncRegressionTests.TestsStartup>
{
    private readonly Uri baseUri;
    private readonly Container context;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(Server.AsyncRegressionTestsController), typeof(MetadataController));

            services.AddControllers().AddOData(options =>
            {
                options.EnableQueryFeatures().AddRouteComponents(
                    routePrefix: string.Empty,
                    model: Server.AsyncRegressionTestsEdmModel.GetEdmModel(),
                    configureServices: (nestedServices) =>
                    {
                        nestedServices.AddSingleton<ODataBatchHandler, DefaultODataBatchHandler>();
                        nestedServices.AddSingleton(_ => new ODataMessageReaderSettings
                        {
                            EnableMessageStreamDisposal = false,
                            Version = ODataVersion.V401,
                            MaxProtocolVersion = ODataVersion.V401
                        });
                    });
            });
        }
    }

    public AsyncRegressionTests(TestWebApplicationFactory<TestsStartup> fixture) : base(fixture)
    {
        if (Client.BaseAddress == null)
        {
            throw new ArgumentNullException(nameof(Client.BaseAddress), "Base address cannot be null");
        }

        baseUri = new Uri(Client.BaseAddress, "/");

        context = new Container(baseUri)
        {
            HttpClientFactory = HttpClientFactory
        };

        ResetDataSource();
    }

    [Fact]
    public async Task DataServiceQuerySingle_GetValueAsync_ReturnsEntity()
    {
        var ctx = new Container(baseUri)
        {
            HttpClientFactory = HttpClientFactory
        };

        var customer = await ctx.Customers.ByKey(1).GetValueAsync();

        Assert.Equal(1, customer.Id);
        Assert.Equal("Sue", customer.Name);
    }

    [Fact]
    public async Task DataServiceContext_SaveChangesAsync_AddsEntity()
    {
        var ctx = new Container(baseUri)
        {
            HttpClientFactory = HttpClientFactory
        };

        ctx.AddToCustomers(new Customer
        {
            Id = 3,
            Name = "Luc"
        });

        var response = await ctx.SaveChangesAsync();

        Assert.NotNull(response);
        var dataServiceResponse = Assert.Single(response);
        Assert.Equal(201, dataServiceResponse.StatusCode);
    }

    [Fact]
    public async Task DataServiceContext_SaveChangesAsync_BatchWithSingleChangeset_AddsEntity()
    {
        var ctx = new Container(baseUri)
        {
            HttpClientFactory = HttpClientFactory
        };

        ctx.AddToCustomers(new Customer
        {
            Id = 3,
            Name = "Luc"
        });

        var response = await ctx.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset);

        Assert.NotNull(response);
        var dataServiceResponse = Assert.Single(response);
        Assert.Equal(201, dataServiceResponse.StatusCode);
    }

    [Fact]
    public async Task DataServiceContext_BulkUpdateAsync_UpdatesEntity()
    {
        var ctx = new Container(baseUri)
        {
            HttpClientFactory = HttpClientFactory
        };

        var customer = await ctx.Customers.ByKey(1).GetValueAsync();
        customer.Name = "Sue Morgan";

        ctx.UpdateObject(customer);

        var response = await ctx.BulkUpdateAsync(customer);

        Assert.NotNull(response);
        var dataServiceResponse = Assert.Single(response);
        Assert.Equal(200, dataServiceResponse.StatusCode);
    }

    [Fact]
    public async Task DataServiceContext_DeepInsertAsync_InsertsEntityGraph()
    {
        var ctx = new Container(baseUri)
        {
            HttpClientFactory = HttpClientFactory
        };

        var customer = new Customer
        {
            Id = 3,
            Name = "Luc"
        };

        ctx.AddToCustomers(customer);

        var response = await ctx.DeepInsertAsync(customer);

        Assert.NotNull(response);
        var dataServiceResponse = Assert.Single(response);
        Assert.Equal(201, dataServiceResponse.StatusCode);
    }

    [Fact]
    public async Task DataServiceQuerySingle_GetValueAsync_RetrievesEntityByKey()
    {
        var ctx = new Container(baseUri)
        {
            HttpClientFactory = HttpClientFactory
        };

        var customer = await ctx.Customers.ByKey(1).GetValueAsync();

        Assert.NotNull(customer);
        Assert.Equal(1, customer.Id);
        Assert.Equal("Sue", customer.Name);
    }

    [Fact]
    public async Task DataServiceQuerySingle_GetValueAsync_ExecutesBoundFunction()
    {
        var ctx = new Container(baseUri)
        {
            HttpClientFactory = HttpClientFactory
        };

        var topCustomer = await ctx.Customers.GetTopCustomer().GetValueAsync();

        Assert.NotNull(topCustomer);
        Assert.True(topCustomer.Id > 0);
    }

    [Fact]
    public async Task DataServiceContext_ExecuteAsync_ExecutesFilterQuery()
    {
        var ctx = new Container(baseUri)
        {
            HttpClientFactory = HttpClientFactory
        };

        var result = (await ctx.ExecuteAsync<Customer>(new Uri("Customers?$filter=contains(Name,'u')", UriKind.Relative))).ToArray();

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.All(result, c => Assert.Contains("u", c.Name));
    }

    [Fact]
    public async Task DataServiceContext_ExecuteBatchAsync_ExecutesBatchRequest()
    {
        var ctx = new Container(baseUri)
        {
            HttpClientFactory = HttpClientFactory
        };

        var response = await ctx.ExecuteBatchAsync(new DataServiceRequest[]
        {
            ctx.Customers.AddQueryOption("$filter", "contains(Name,'o')"),
        });

        Assert.NotNull(response);
        var queryResponse = response.Single() as QueryOperationResponse<Customer>;
        Assert.NotNull(queryResponse);
        Assert.NotEmpty(queryResponse);
        Assert.Equal(200, queryResponse.StatusCode);
    }

    [Fact]
    public async Task DataServiceFunctionQuery_ExecuteAsync_ReturnsCollectionFromFunction()
    {
        var ctx = new Container(baseUri)
        {
            HttpClientFactory = HttpClientFactory
        };

        var result = await ctx.CreateFunctionQuery<Order>("Orders", "GetTop2Orders", true).ExecuteAsync();

        Assert.NotNull(result);
        var orders = result.ToList();
        Assert.Equal(2, orders.Count);
    }

    [Fact]
    public async Task DataServiceQuery_ExecuteAsync_ExecutesQuery()
    {
        var ctx = new Container(baseUri)
        {
            HttpClientFactory = HttpClientFactory
        };

        DataServiceQuery query = ctx.CreateFunctionQuery<Order>("Orders", "GetTop2Orders", true);

        var result = await query.ExecuteAsync();

        Assert.NotNull(result);
        var orders = result.Cast<Order>().ToList();
        Assert.Equal(2, orders.Count);
    }

    [Fact]
    public async Task DataServiceContext_LoadPropertyAsync_LoadsNavigationProperty()
    {
        var ctx = new Container(baseUri)
        {
            HttpClientFactory = HttpClientFactory
        };

        var order = await ctx.Orders.ByKey(1).GetValueAsync();

        var result = await ctx.LoadPropertyAsync(order, "Customer");

        Assert.NotNull(result);
        Assert.NotNull(order.Customer);
        Assert.Equal(1, order.Customer.Id);
    }

    [Fact]
    public async Task DataServiceContext_LoadPropertyAsync_LoadsCollectionWithNextLink()
    {
        var ctx = new Container(baseUri)
        {
            HttpClientFactory = HttpClientFactory
        };

        var customer = await ctx.Customers.ByKey(1).GetValueAsync();

        // Load first page
        await ctx.LoadPropertyAsync(customer, "Orders");

        // Load next page
        var result = await ctx.LoadPropertyAsync(customer, "Orders", new Uri(baseUri, "Customers(1)/Orders?$skiptoken=Id-3"));

        Assert.NotNull(result);
    }

    [Fact]
    public async Task DataServiceContext_LoadPropertyAsync_LoadsCollectionWithContinuation()
    {
        var ctx = new Container(baseUri)
        {
            HttpClientFactory = HttpClientFactory
        };

        var customer = await ctx.Customers.ByKey(1).GetValueAsync();

        var result = await ctx.LoadPropertyAsync(customer, "Orders");

        Assert.NotNull(result);
        var continuation = result.GetContinuation();
        Assert.NotNull(continuation);
        result = await ctx.LoadPropertyAsync(customer, "Orders", continuation);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task DataServiceContext_GetReadStreamAsync_ReadsMediaEntityStream()
    {
        var ctx = new Container(baseUri)
        {
            HttpClientFactory = HttpClientFactory
        };

        var mediaAsset = await ctx.Media.ByKey(1).GetValueAsync();

        var dataServiceStreamResponse = await ctx.GetReadStreamAsync(mediaAsset, new DataServiceRequestArgs());

        Assert.NotNull(dataServiceStreamResponse);
        Assert.NotNull(dataServiceStreamResponse.Stream);
        Assert.Equal("application/octet-stream", dataServiceStreamResponse.ContentType);
    }

    [Fact]
    public async Task DataServiceContext_GetReadStreamAsync_ReadsNamedStream()
    {
        var ctx = new Container(baseUri)
        {
            HttpClientFactory = HttpClientFactory
        };

        var customer = await ctx.Customers.ByKey(1).GetValueAsync();

        var dataServiceStreamResponse = await ctx.GetReadStreamAsync(customer, "Photo", new DataServiceRequestArgs());

        Assert.NotNull(dataServiceStreamResponse);
        Assert.NotNull(dataServiceStreamResponse.Stream);
        Assert.Equal("image/png", dataServiceStreamResponse.ContentType);
    }

    [Fact]
    public async Task DataServiceContext_LoadPropertyAllPagesAsync_LoadsAllPagesViaReflection()
    {
        var ctx = new Container(baseUri)
        {
            HttpClientFactory = HttpClientFactory
        };

        var customer = await ctx.Customers.ByKey(1).GetValueAsync();

        // Get the method using reflection
        MethodInfo? loadPropertyAllPagesAsyncMethod = typeof(DataServiceContext).GetMethod(
            "LoadPropertyAllPagesAsync",
            BindingFlags.NonPublic | BindingFlags.Instance,
            new[] { typeof(object), typeof(string) });

        Assert.NotNull(loadPropertyAllPagesAsyncMethod);

        var task = (Task<QueryOperationResponse>?)loadPropertyAllPagesAsyncMethod.Invoke(
            ctx,
            new object[] { customer, "Orders" });

        Assert.NotNull(task);
        var result = await task;

        Assert.NotNull(result);
        Assert.NotEmpty(customer.Orders);
    }

    [Fact]
    public async Task DataServiceQuery_GetAllPagesAsync_RetrievesAllPages()
    {
        var ctx = new Container(baseUri)
        {
            HttpClientFactory = HttpClientFactory
        };

        var result = (await ctx.Orders.GetAllPagesAsync()).ToArray();

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.True(result.Length >= 5);
    }

    [Fact]
    public async Task DataServiceActionQuerySingle_GetValueAsync_ExecutesActionReturningScalar()
    {
        var ctx = new Container(baseUri)
        {
            HttpClientFactory = HttpClientFactory
        };

        var totalAfterDiscount = await ctx.Orders.ByKey(1).ApplyDiscount(10m).GetValueAsync();

        Assert.True(totalAfterDiscount > 0);
        Assert.True(totalAfterDiscount < 190); // Original amount was 190
    }

    private void ResetDataSource()
    {
        var actionUri = new Uri(baseUri + "Default.ResetDataSource", UriKind.Absolute);
        context.Execute(actionUri, "POST");
    }
}

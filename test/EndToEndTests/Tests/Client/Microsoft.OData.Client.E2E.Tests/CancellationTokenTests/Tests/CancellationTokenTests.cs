//-----------------------------------------------------------------------------
// <copyright file="CancellationTokenTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.TestCommon;
using Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd;
using Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd.Default;
using Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd;
using Microsoft.OData.Client.E2E.Tests.DeltaTests.Server;
using Microsoft.OData.Edm;
using Xunit;
using AuditInfo = Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd.AuditInfo;
using Car = Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd.Car;
using Customer = Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd.Customer;
using Order = Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd.Order;

namespace Microsoft.OData.Client.E2E.Tests.DeltaTests.Tests;


/// <summary>
/// CancellationToken tests using asynchronous APIs
/// </summary>
public class CancellationTokenTests : EndToEndTestBase<CancellationTokenTests.TestsStartup>
{
    private readonly Uri _baseUri;
    private readonly Container _context;
    private readonly IEdmModel _model;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(CancellationTokenTestsController), typeof(MetadataController));

            services.AddControllers().AddOData(opt =>
                opt.EnableQueryFeatures().AddRouteComponents("odata", CommonEndToEndEdmModel.GetEdmModel(), batchHandler: new DefaultODataBatchHandler()));
        }
    }

    public CancellationTokenTests(TestWebApplicationFactory<TestsStartup> fixture) : base(fixture)
    {
        if (Client.BaseAddress == null)
        {
            throw new ArgumentNullException(nameof(Client.BaseAddress), "Base address cannot be null");
        }

        _baseUri = new Uri(Client.BaseAddress, "odata/");

        _context = new Container(_baseUri)
        {
            HttpClientFactory = HttpClientFactory
        };

        _model = CommonEndToEndEdmModel.GetEdmModel();
        ResetDefaultDataSource();
    }

    #region SaveChangesAsync with CancellationToken

    [Fact]
    public async Task SaveChangesAsyncCancellationTokenTest()
    {
        // Arrange
        var source = new CancellationTokenSource();
        var c1 = new Customer { CustomerId = 11, Name = "customerOne" };

        // Act & Assert
        _context.AddToCustomers(c1);

        Task response() => _context.SaveChangesAsync(source.Token);
        source.Cancel();
        var exception = await Assert.ThrowsAsync<OperationCanceledException>(response);
        Assert.Equal("The operation was canceled.", exception.Message);

        // SaveChangesAsync with SaveChangesOptions
        var c2 = new Customer { CustomerId = 22, Name = "customerTwo" };
        var c3 = new Customer { CustomerId = 33, Name = "customerThree" };
        _context.AddToCustomers(c2);
        _context.AddToCustomers(c3);

        Task response2() => _context.SaveChangesAsync(SaveChangesOptions.BatchWithIndependentOperations, source.Token);
        source.Cancel();
        var exception2 = await Assert.ThrowsAsync<OperationCanceledException>(response2);
        Assert.Equal("The operation was canceled.", exception2.Message);
    }

    #endregion

    #region GetValueAsync with CancellationToken

    [Fact]
    public async Task GetValueAsyncCancellationTokenTest()
    {
        // Arrange
        var source = new CancellationTokenSource();
        var c1 = new Customer { CustomerId = 11, Name = "customerOne" };
        _context.AddToCustomers(c1);
        await _context.SaveChangesAsync();

        // Act & Assert
        Task response() => _context.Customers.ByKey(11).GetValueAsync(source.Token);
        source.Cancel();
        var exception = await Assert.ThrowsAsync<OperationCanceledException>(response);
        Assert.Equal("The operation was canceled.", exception.Message);
    }

    #endregion

    #region ExecuteAsync with CancellationToken

    [Fact]
    public async Task ExecuteAsyncCancellationTokenTest()
    {
        // Arrange
        var source = new CancellationTokenSource();
        var c1 = new Customer { CustomerId = 11, Name = "customerOne" };

        // Act & Assert
        _context.AddToCustomers(c1);
        var c2 = new Customer { CustomerId = 22, Name = "customerTwo" };
        _context.AddToCustomers(c2);
        await _context.SaveChangesAsync();

        Task response() => _context.Customers.ExecuteAsync(source.Token);
        source.Cancel();
        var exception = await Assert.ThrowsAsync<OperationCanceledException>(response);
        Assert.Equal("The operation was canceled.", exception.Message);

        // ExecuteAsync by continuation
        var customers = (await _context.Customers.ExecuteAsync()) as QueryOperationResponse<Customer>;
        // continuation is only available when the result has been enumerated. Hence we call Count()
        Assert.NotNull(customers);
        var count = customers.Count(); 
        var continuation = customers.GetContinuation();

        Task response2() => _context.ExecuteAsync(continuation, source.Token);
        source.Cancel();
        var exception2 = await Assert.ThrowsAsync<OperationCanceledException>(response2);
        Assert.Equal("The operation was canceled.", exception2.Message);

        // ExecuteAsync by nextLink
        var customers2 = (await _context.Customers.ExecuteAsync()) as QueryOperationResponse<Customer>;
        // continuation is only available when the result has been enumerated. Hence we call Count()
        Assert.NotNull(customers2);
        var count2 = customers2.Count(); 
        var continuation2 = customers2.GetContinuation();

        Task response3() => _context.ExecuteAsync<Customer>(continuation2.NextLinkUri, source.Token);
        source.Cancel();
        var exception3 = await Assert.ThrowsAsync<OperationCanceledException>(response3);
        Assert.Equal("The operation was canceled.", exception3.Message);
    }

    #endregion

    #region GetAllPagesAsync with CancellationToken

    [Fact]
    public async Task GetAllPagesAsyncCancellationTokenTest()
    {
        // Arrange
        var source = new CancellationTokenSource();

        // Act & Assert
        var c1 = new Customer { CustomerId = 11, Name = "customerOne" };
        _context.AddToCustomers(c1);
        var c2 = new Customer { CustomerId = 22, Name = "customerTwo" };
        _context.AddToCustomers(c2);
        await _context.SaveChangesAsync();

        Task response() => _context.Customers.GetAllPagesAsync(source.Token);
        source.Cancel();
        var exception = await Assert.ThrowsAsync<OperationCanceledException>(response);
        Assert.Equal("The operation was canceled.", exception.Message);
    }

    #endregion

    #region LoadPropertyAsyn with CancellationToken

    [Fact]
    public async Task LoadPropertyAsyncCancellationTokenTest()
    {
        // Arrange
        var source = new CancellationTokenSource();
        _context.MergeOption = MergeOption.OverwriteChanges;

        // Act & Assert
        var c1 = new Customer { CustomerId = 11, Name = "customerOne" };
        _context.AddToCustomers(c1);
        await _context.SaveChangesAsync();

        for (int i = 1; i <= 9; i++)
        {
            Order order = new Order() { OrderId = 1000 + i };
            _context.AddToOrders(order);
            _context.AddLink(c1, "Orders", order);
        }

        await _context.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset);

        Task response() => _context.LoadPropertyAsync(c1, "Orders", source.Token);
        source.Cancel();
        var exception = await Assert.ThrowsAsync<OperationCanceledException>(response);
        Assert.Equal("The operation was canceled.", exception.Message);

        //Get Entity by DataServiceQuery.ExecuteAsync
        var query = _context.Customers.Expand(c => c.Orders).Where(c => c.CustomerId == 11) as DataServiceQuery<Customer>;
        Assert.NotNull(query);
        var resp = (await query.ExecuteAsync()) as QueryOperationResponse<Customer>;
        Assert.NotNull(resp);
        var customer = resp.First();

        //Load navigation property by using continuation
        var continuation = resp.GetContinuation(customer.Orders);
        Task response2() => _context.LoadPropertyAsync(customer, "Orders", continuation, source.Token);
        source.Cancel();
        var exception2 = await Assert.ThrowsAsync<OperationCanceledException>(response2);
        Assert.Equal("The operation was canceled.", exception2.Message);

        Task response3() => _context.LoadPropertyAsync(customer, "Orders", continuation.NextLinkUri, source.Token);
        source.Cancel();
        var exception3 = await Assert.ThrowsAsync<OperationCanceledException>(response3);
        Assert.Equal("The operation was canceled.", exception3.Message);
    }

    #endregion

    #region ReadStreamAsync with CancellationToken

     //[Fact] // Failing - to be fixed in a different PR
    public async Task GetReadStreamAsyncCancellationTokenTest()
    {
        // Arrange
        var source = new CancellationTokenSource();
        var car = new Car { VIN = 1000 };

        var mediaEntry = new MemoryStream(new byte[] { 64, 65, 66 });

        // Act & Assert
        _context.AddToCars(car);

        //_context.SetSaveStream(car, mediaEntry, true, "image/png", "UnitTestLogo.png");
        //await _context.SaveChangesAsync();

        //Task response() => _context.GetReadStreamAsync(car, new DataServiceRequestArgs(), source.Token);
        //source.Cancel();
        //var exception = await Assert.ThrowsAsync<OperationCanceledException>(response);
        //Assert.Equal("The operation was canceled.", exception.Message);

        _context.SetSaveStream(car, "Photo", mediaEntry, true, new DataServiceRequestArgs { ContentType = "application/binary" });
        await _context.SaveChangesAsync();

        Task response2() => _context.GetReadStreamAsync(car, "Photo", new DataServiceRequestArgs { AcceptContentType = "application/binary" }, source.Token);
        source.Cancel();
        var exception2 = await Assert.ThrowsAsync<OperationCanceledException>(response2);
        Assert.Equal("The operation was canceled.", exception2.Message);
    }

    #endregion

    #region ExecuteBatchAsync with CancellationToken

    [Fact]
    public async Task ExecuteBatchAsyncCancellationTokenTest()
    {
        // Arrange
        var source = new CancellationTokenSource();
        var c1 = new Customer { CustomerId = 11, Name = "customerOne" };
        var c2 = new Customer { CustomerId = 22, Name = "customerTwo" };

        // Act & Assert
        _context.AddToCustomers(c1);
        _context.AddToCustomers(c2);
        await _context.SaveChangesAsync();

        Task response() => _context.ExecuteBatchAsync(
            SaveChangesOptions.BatchWithIndependentOperations | SaveChangesOptions.UseRelativeUri,
            source.Token,
            new DataServiceRequest[]
            {
                    new DataServiceRequest<Customer>(((_context.Customers.Where(c => c.CustomerId == 11)) as DataServiceQuery<Customer>)?.RequestUri),
                    new DataServiceRequest<Customer>(((_context.Customers.Where(c => c.CustomerId == 22)) as DataServiceQuery<Customer>)?.RequestUri)
            });

        source.Cancel();
        var exception = await Assert.ThrowsAsync<OperationCanceledException>(response);
        Assert.Equal("The operation was canceled.", exception.Message);
    }

    #endregion

    #region DataServiceActionQuery.ExecuteAsync with CancellationToken

    [Fact]
    public async Task DataServiceActionQueryExecuteAsyncCancellationTokenTest()
    {
        // Arrange
        var source = new CancellationTokenSource();
        var c1 = new Customer { CustomerId = 11, Name = "customerOne" };
        var c2 = new Customer { CustomerId = 22, Name = "customerTwo" };

        // Act & Assert
        _context.AddToCustomers(c1);
        _context.AddToCustomers(c2);
        await _context.SaveChangesAsync();

        var auditInfo = new AuditInfo()
        {
            ModifiedDate = new DateTimeOffset()
        };

        DataServiceQuerySingle<Customer> customer = _context.Customers.ByKey(11);
        DataServiceActionQuery getComputerAction = customer.ChangeCustomerAuditInfo(auditInfo);

        Task response() => getComputerAction.ExecuteAsync(source.Token);

        source.Cancel();
        var exception = await Assert.ThrowsAsync<OperationCanceledException>(response);
        Assert.Equal("The operation was canceled.", exception.Message);
    }

    #endregion

    #region Private

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "cancellationtokentests/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    #endregion
}

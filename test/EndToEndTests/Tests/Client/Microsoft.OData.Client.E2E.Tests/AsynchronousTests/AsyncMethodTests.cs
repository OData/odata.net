//---------------------------------------------------------------------
// <copyright file="AsyncMethodTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd;
using Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.Asynchronous;
using Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd;
using Xunit;
using ConcurrencyInfo = Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.ConcurrencyInfo;
using Customer = Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.Customer;
using Driver = Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.Driver;
using Employee = Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.Employee;
using Order = Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.Order;
using SpecialEmployee = Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.SpecialEmployee;

namespace Microsoft.OData.Client.E2E.Tests.AsynchronousTests;

public class AsyncMethodTests : AsynchronousEndToEndTestBase<AsyncMethodTests.TestsStartup>
{
    private readonly Uri _baseUri;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(AsyncMethodTestsController), typeof(MetadataController));

            services.AddControllers().AddOData(opt =>
            {
                opt.EnableQueryFeatures().AddRouteComponents("odata", CommonEndToEndEdmModel.GetEdmModel(), new DefaultODataBatchHandler());
            });
        }
    }

    public AsyncMethodTests(TestWebApplicationFactory<TestsStartup> fixture) : base(fixture)
    {
        if (Client.BaseAddress == null)
        {
            throw new ArgumentNullException(nameof(Client.BaseAddress), "Base address cannot be null");
        }

        _baseUri = new Uri(Client.BaseAddress, "odata/");
    }

    [Fact]
    public async Task SaveChangesTest_ShouldHandleVariousSaveChangesOptions()
    {
        // Arrange
        var context = this.CreateWrappedContext();
        context.MergeOption = MergeOption.OverwriteChanges;
        bool checkEntry = true;
        int expectedPropertyCount = 1;
        Action<WritingEntryArgs> onEntryEnding = (args) =>
        {
            if (checkEntry)
            {
                Assert.Equal(expectedPropertyCount, args.Entry.Properties.Count());
            }
        };

        context.Configurations.RequestPipeline.OnEntryEnding(onEntryEnding);

        // Act & Assert
        DataServiceCollection<Customer> customers = new DataServiceCollection<Customer>(context, "Customers", null, null);
        Customer c1 = new Customer();
        customers.Add(c1);
        c1.CustomerId = 1;
        c1.Name = "testName";

        //Partial Post an Entity
        expectedPropertyCount = 2;
        var response = await context.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties);
        Assert.True((response.First() as ChangeOperationResponse)?.StatusCode == 201, "StatusCode == 201");

        var o1 = new Order { OrderId = 1000, CustomerId = 1, Concurrency = new ConcurrencyInfo() { Token = "token1" } };
        context.AddToOrders(o1);
        context.AddLink(c1, "Orders", o1);

        //Post with batch
        expectedPropertyCount = 2;
        var batchResponse = await context.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset);

        List<Order> orders = new List<Order>();
        for (int i = 1; i <= 9; i++)
        {
            Order order = new Order() { OrderId = 1000 + i };
            context.AddToOrders(order);
            orders.Add(order);
        }

        //Post with batch
        await context.SaveChangesAsync(SaveChangesOptions.BatchWithIndependentOperations);

        //Post $ref
        foreach (var order in orders)
        {
            context.AddLink(c1, "Orders", order);
        }

        await context.SaveChangesAsync();

        //Load property
        await context.LoadPropertyAsync(c1, "Orders");

        //Partial Update an Entity
        expectedPropertyCount = 1;
        c1.Orders[0].Concurrency.Token = "UpdatedToken";
        checkEntry = false;
        Action<WritingEntryArgs> onEntryEnding1 = (args) =>
        {
            if (args.Entry.TypeName.EndsWith("ConcurrencyInfo"))
            {
                Assert.Equal("UpdatedToken", (args.Entry.Properties.Single(p => p.Name == "Token") as ODataProperty)?.Value);
            }
        };

        context.Configurations.RequestPipeline.OnEntryEnding(onEntryEnding1);
        await context.SaveChangesAsync(SaveChangesOptions.None);

        // Batch relative URIs
        Customer c2 = new Customer { CustomerId = 11, Name = "customerTwo" };
        customers.Add(c2);

        var dataServiceResponse = await context.SaveChangesAsync(SaveChangesOptions.BatchWithIndependentOperations | SaveChangesOptions.UseRelativeUri);
        Assert.Equal(201, (dataServiceResponse.First() as ChangeOperationResponse)?.StatusCode);

        // UseJsonBatch
        c2.Name = "Customer Two Updated";
        context.UpdateObject(c2);

        // Use client hooks to check request headers
        context.SendingRequest2 += (sender, eventArgs) =>
        {
            if (!eventArgs.IsBatchPart) // Check top level headers only
            {
                Assert.Equal("application/json", eventArgs.RequestMessage.GetHeader("Content-Type"));
            }
        };

        var dscResponse = await context.SaveChangesAsync(SaveChangesOptions.BatchWithIndependentOperations | SaveChangesOptions.UseJsonBatch);
        Assert.Equal(204, (dscResponse.First() as ChangeOperationResponse)?.StatusCode);

        this.EnqueueTestComplete();
    }

    [Fact]
    public async Task JsonBatchSequencingPatchTest_ShouldUpdateEntitiesInBatch()
    {
        // Arrange
        var context = this.CreateWrappedContext();
        Customer c1 = new Customer { CustomerId = 1, Name = "customerOne" };
        Customer c2 = new Customer { CustomerId = 2, Name = "customerTwo" };

        // Act & Assert
        context.AddToCustomers(c1);
        context.AddToCustomers(c2);
        await context.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset);

        c1.Name = "customerOne updated name";
        c2.Name = "customerTwo updated name";

        context.UpdateObject(c1);
        context.UpdateObject(c2, c1);

        var response = await context.SaveChangesAsync(SaveChangesOptions.BatchWithIndependentOperations | SaveChangesOptions.UseJsonBatch);
        Assert.Equal(204, (response.Last() as ChangeOperationResponse)?.StatusCode);

        this.EnqueueTestComplete();
    }

    [Fact]
    public async Task JsonBatchSequencingDeleteTest_ShouldDeleteEntitiesInBatch()
    {
        // Arrange
        var context = this.CreateWrappedContext();
        Customer c1 = new Customer { CustomerId = 1, Name = "customerOne" };
        Customer c2 = new Customer { CustomerId = 2, Name = "customerTwo" };
        Customer c3 = new Customer { CustomerId = 3, Name = "customerThree" };

        // Act & Assert
        context.AddToCustomers(c1);
        context.AddToCustomers(c2);
        context.AddToCustomers(c3);
        await context.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset);

        c1.Name = "customerOne updated name";
        c2.Name = "customerTwo updated name";

        context.UpdateObject(c1);
        context.UpdateObject(c2, c1);
        context.DeleteObject(c3, c2);

        var response = await context.SaveChangesAsync(SaveChangesOptions.BatchWithIndependentOperations | SaveChangesOptions.UseJsonBatch);
        Assert.Equal(204, (response.Last() as ChangeOperationResponse)?.StatusCode);

        this.EnqueueTestComplete();
    }

    [Fact]
    public async Task JsonBatchSequencingSingleChangeSetTest_ShouldHandleSingleChangesetBatch()
    {
        // Arrange
        var context = this.CreateWrappedContext();
        Customer c1 = new Customer { CustomerId = 1, Name = "customerOne" };
        Customer c2 = new Customer { CustomerId = 2, Name = "customerTwo" };
        Customer c3 = new Customer { CustomerId = 3, Name = "customerThree" };

        // Act & Assert
        context.AddToCustomers(c1);
        context.AddToCustomers(c2);
        context.AddToCustomers(c3);
        await context.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset);

        c1.Name = "customerOne updated name";
        c2.Name = "customerTwo updated name";

        context.UpdateObject(c1);
        context.UpdateObject(c2, c1);
        context.DeleteObject(c3, c2);

        var response = await context.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset | SaveChangesOptions.UseJsonBatch);
        Assert.Equal(204, (response.First() as ChangeOperationResponse)?.StatusCode);

        this.EnqueueTestComplete();
    }

    [Fact]
    public async Task QueryEntitySetPagingTest_ShouldHandlePagingForEntitySet()
    {
        // Arrange
        var context = this.CreateWrappedContext();

        // Act & Assert
        var query = context.Customers.IncludeCount();
        var response = (await query.ExecuteAsync()) as QueryOperationResponse<Customer>;
        var totalCount = response.Count;
        var count = response.Count();

        //ExecuteAsync by continuation
        var continuation = response.GetContinuation();
        var response2 = await context.ExecuteAsync(continuation);
        var currentPageCount = (response2 as QueryOperationResponse<Customer>).Count();
        count += currentPageCount;
        Assert.Equal(2, currentPageCount);

        //ExecuteAsync by nextLink
        continuation = (response2 as QueryOperationResponse<Customer>)?.GetContinuation();
        response2 = await context.ExecuteAsync<Customer>(continuation.NextLinkUri);
        currentPageCount = (response2 as QueryOperationResponse<Customer>).Count();
        count += currentPageCount;
        Assert.Equal(2, currentPageCount);

        continuation = (response2 as QueryOperationResponse<Customer>).GetContinuation();
        while (continuation != null)
        {
            response2 = await context.ExecuteAsync(continuation);

            currentPageCount = (response2 as QueryOperationResponse<Customer>).Count();
            count += currentPageCount;
            continuation = (response2 as QueryOperationResponse<Customer>).GetContinuation();
        }

        Assert.Equal(totalCount, count);
        this.EnqueueTestComplete();
    }

    [Fact]
    public async Task LoadPropertyTest_ShouldLoadNavigationPropertiesCorrectly()
    {
        // Arrange
        var context = this.CreateWrappedContext();
        context.MergeOption = MergeOption.OverwriteChanges;

        // Act & Assert
        var person = (await context.People.ExecuteAsync()).First() as SpecialEmployee;
        Assert.Null(person.Car);

        //Load Derived Navigation property
        await context.LoadPropertyAsync(person, "Car");
        Assert.NotNull(person.Car);

        //var c1 = (await context.Customers.ExecuteAsync()).First();
        var c1 = new Customer() { CustomerId = -10 };
        context.AttachTo("Customers", c1);

        for (int i = 1; i <= 9; i++)
        {
            Order order = new Order() { OrderId = 1000 + i };
            context.AddToOrders(order);
            context.AddLink(c1, "Orders", order);
        }

        //Post with batch
        await context.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset);

        //Get Entity by DataServiceQuery.ExecuteAsync
        var resp = (await ((DataServiceQuery<Customer>)(context.Customers.Expand(c => c.Orders).Where(c => c.CustomerId == -10))).ExecuteAsync()) as QueryOperationResponse<Customer>;
        Assert.NotNull(resp);
        var customer = resp.First();

        //Load navigation property by using continuation
        var continuation = resp.GetContinuation(customer.Orders);
        var orderResp = await context.LoadPropertyAsync(customer, "Orders", continuation) as QueryOperationResponse<Order>;
        Assert.True(customer.Orders.Count() == 4);

        //Load navigation property by using nextLink
        Assert.NotNull(orderResp);
        continuation = orderResp.GetContinuation();
        var orderResp2 = await context.LoadPropertyAsync(customer, "Orders", continuation.NextLinkUri);
        Assert.True(customer.Orders.Count() == 6);

        this.EnqueueTestComplete();
    }

    [Fact]
    public async Task ExecuteBatchTest_ShouldExecuteBatchRequestsSuccessfully()
    {
        // Arrange
        var context = this.CreateWrappedContext();
        var countOfBatchParts = 0;
        var countOfTimesSenderCalled = 0;
        context.SendingRequest2 += ((sender, args) =>
        {
            if (args.IsBatchPart)
            {
                countOfBatchParts++;
            }

            countOfTimesSenderCalled++;
        });

        // Act & Assert
        var queryResponse = await context.ExecuteBatchAsync(new DataServiceRequest[] 
        { 
            new DataServiceRequest<Customer>(((from c in context.Customers where c.CustomerId == -8 select c) as DataServiceQuery<Customer>)?.RequestUri), 
            new DataServiceRequest<Customer>(((from c in context.Customers where c.CustomerId == -6 select c) as DataServiceQuery<Customer>)?.RequestUri), 
            new DataServiceRequest<Driver>(((from c in context.Drivers where c.Name == "1" select c) as DataServiceQuery<Driver>)?.RequestUri), 
            new DataServiceRequest<Driver>(((from c in context.Drivers where c.Name == "3" select c) as DataServiceQuery<Driver>)?.RequestUri) 
        });

        var operationResponses = queryResponse.ToList();
        Assert.Equal(4, operationResponses.Count);

        var customer1 = (operationResponses[0] as QueryOperationResponse<Customer>)?.SingleOrDefault();
        Assert.NotNull(customer1);
        Assert.Equal(-8, customer1.CustomerId);

        var customer2 = (operationResponses[1] as QueryOperationResponse<Customer>)?.SingleOrDefault();
        Assert.NotNull(customer2);
        Assert.Equal(-6, customer2.CustomerId);

        var driver1 = (operationResponses[2] as QueryOperationResponse<Driver>)?.SingleOrDefault();
        Assert.NotNull(driver1);
        Assert.Equal("1", driver1.Name);

        var driver2 = (operationResponses[3] as QueryOperationResponse<Driver>)?.SingleOrDefault();
        Assert.NotNull(driver2);
        Assert.Equal("3", driver2.Name);

        Assert.Equal(5, countOfTimesSenderCalled);
        Assert.Equal(4, countOfBatchParts);

        bool isBatchPartsValid = countOfBatchParts > 0 && (countOfTimesSenderCalled - countOfBatchParts) == 1;
        Assert.True(isBatchPartsValid, "countOfBatchParts > 0 && (countOfTimesSenderCalled - countOfBatchParts ) == 1");

        this.EnqueueTestComplete();
    }

    [Fact]
    public async Task ExecuteBatchWithSaveChangesOptionsReturnsCorrectResults_ShouldValidateBatchOptions()
    {
        // Arrange
        var context = this.CreateWrappedContext();
        var countOfBatchParts = 0;
        var countOfTimesSenderCalled = 0;
        context.SendingRequest2 += ((sender, args) =>
        {
            if (args.IsBatchPart)
            {
                countOfBatchParts++;
            }

            countOfTimesSenderCalled++;
        });

        var queryResponse = await context.ExecuteBatchAsync(SaveChangesOptions.BatchWithIndependentOperations | SaveChangesOptions.UseRelativeUri,
            new DataServiceRequest[]
            {
                    new DataServiceRequest<Customer>(((context.Customers.Where(c => c.CustomerId == -8)) as DataServiceQuery<Customer>)?.RequestUri),
                    new DataServiceRequest<Customer>(((context.Customers.Where(c => c.CustomerId == -6)) as DataServiceQuery<Customer>)?.RequestUri),
                    new DataServiceRequest<Driver>(((context.Drivers.Where(c => c.Name == "1")) as DataServiceQuery<Driver>)?.RequestUri),
                    new DataServiceRequest<Driver>(((context.Drivers.Where(c => c.Name == "3")) as DataServiceQuery<Driver>)?.RequestUri)
            });

        var operationResponses = queryResponse.ToList();
        Assert.Equal(4, operationResponses.Count);

        var customer1 = (operationResponses[0] as QueryOperationResponse<Customer>)?.SingleOrDefault();
        Assert.NotNull(customer1);
        Assert.Equal(-8, customer1.CustomerId);

        var customer2 = (operationResponses[1] as QueryOperationResponse<Customer>)?.SingleOrDefault();
        Assert.NotNull(customer2);
        Assert.Equal(-6, customer2.CustomerId);

        var driver1 = (operationResponses[2] as QueryOperationResponse<Driver>)?.SingleOrDefault();
        Assert.NotNull(driver1);
        Assert.Equal("1", driver1.Name);

        var driver2 = (operationResponses[3] as QueryOperationResponse<Driver>)?.SingleOrDefault();
        Assert.NotNull(driver2);
        Assert.Equal("3", driver2.Name);

        Assert.Equal(5, countOfTimesSenderCalled);
        Assert.Equal(4, countOfBatchParts);

        bool isBatchPartsValid = countOfBatchParts > 0 && (countOfTimesSenderCalled - countOfBatchParts) == 1;
        Assert.True(isBatchPartsValid, "countOfBatchParts > 0 && (countOfTimesSenderCalled - countOfBatchParts ) == 1");

        this.EnqueueTestComplete();
    }

    [Fact]
    public async Task ActionFunction_ShouldExecuteActionsAndFunctionsCorrectly()
    {
        var context = this.CreateWrappedContext();
        context.MergeOption = MergeOption.OverwriteChanges;

        var queryable = ((DataServiceQuery<Employee>)context.People.OfType<Employee>());
        var employees = (await queryable.ExecuteAsync()).ToList();
        var expectedEmployee0Salary = employees.First().Salary;

        //Execute Async with Uri and operation parameter
        await context.ExecuteAsync(new Uri(queryable.RequestUri.ToString() + "/Default.IncreaseSalaries"),
            "POST",
            new BodyOperationParameter("n", 5));

        var currentEmployees = await queryable.ExecuteAsync();
        Assert.Equal(expectedEmployee0Salary + 5, currentEmployees.First().Salary);

        //ExecuteAsyncOfT with Uri and operation parameter
        await context.ExecuteAsync<int>(new Uri(queryable.RequestUri.ToString() + "/Default.IncreaseSalaries"),
            "POST",
            new BodyOperationParameter("n", 5));

        currentEmployees = await queryable.ExecuteAsync();
        Assert.Equal(expectedEmployee0Salary + 10, currentEmployees.First().Salary);

        //ExecuteAsyncOfT which will return a singleResult
        int resultValue = (await context.ExecuteAsync<int>(new Uri("GetCustomerCount", UriKind.Relative), "GET", true)).Single();
        Assert.Equal(10, resultValue);
        this.EnqueueTestComplete();
    }

    [Fact]
    public async Task GetAllPagesAsyncTest_ShouldRetrieveAllPagesForQuery()
    {
        var context = this.CreateWrappedContext();

        var query = context.Customers.IncludeCount();
        var allCustomersCount = ((await query.ExecuteAsync()) as QueryOperationResponse<Customer>).Count;

        bool CheckNextLink = false;
        Uri? nextPageLink = null;

        EventHandler<SendingRequest2EventArgs> sendRequestEvent = (sender, args) =>
        {
            //The first request should not be checked.
            if (CheckNextLink)
            {
                Assert.Equal(nextPageLink?.AbsoluteUri, args.RequestMessage.Url.AbsoluteUri);
            }
            CheckNextLink = true;
        };

        context.Configurations.ResponsePipeline.OnFeedEnded((args) =>
        {
            nextPageLink = args.Feed.NextPageLink;
        });

        context.SendingRequest2 += sendRequestEvent;
        int queryCustomersCount = (await context.Customers.GetAllPagesAsync()).ToList().Count();
        Assert.Equal(allCustomersCount, queryCustomersCount);

        //$filter
        context.SendingRequest2 -= sendRequestEvent;
        query = ((DataServiceQuery<Customer>)context.Customers.Where(c => c.CustomerId > -5)).IncludeCount();
        var filterCustomersCount = ((await query.ExecuteAsync()) as QueryOperationResponse<Customer>).Count;

        context.SendingRequest2 += sendRequestEvent;
        CheckNextLink = false;
        queryCustomersCount = (await ((DataServiceQuery<Customer>)context.Customers.Where(c => c.CustomerId > -5)).GetAllPagesAsync()).ToList().Count();
        Assert.Equal(filterCustomersCount, queryCustomersCount);

        //$projection
        CheckNextLink = false;
        queryCustomersCount = (await ((DataServiceQuery<Customer>)context.Customers.Select(c => new Customer() { CustomerId = c.CustomerId, Name = c.Name })).GetAllPagesAsync()).ToList().Count();
        Assert.Equal(allCustomersCount, queryCustomersCount);

        //$expand
        CheckNextLink = false;
        queryCustomersCount = (await context.Customers.Expand(c => c.Orders).GetAllPagesAsync()).ToList().Count();
        Assert.Equal(allCustomersCount, queryCustomersCount);

        //$top
        CheckNextLink = false;
        queryCustomersCount = (await ((DataServiceQuery<Customer>)context.Customers.Take(4)).GetAllPagesAsync()).ToList().Count();
        Assert.Equal(4, queryCustomersCount);

        //$orderby
        CheckNextLink = false;
        queryCustomersCount = (await ((DataServiceQuery<Customer>)context.Customers.OrderBy(c => c.Name)).GetAllPagesAsync()).ToList().Count();
        Assert.Equal(allCustomersCount, queryCustomersCount);

        //$skip
        CheckNextLink = false;
        queryCustomersCount = (await ((DataServiceQuery<Customer>)context.Customers.Skip(4)).GetAllPagesAsync()).ToList().Count();
        Assert.Equal(allCustomersCount - 4, queryCustomersCount);
        this.EnqueueTestComplete();
    }

    [Fact]
    public async Task PagingOnNavigationProperty_ShouldHandlePagingForNavigationProperties()
    {
        var context = this.CreateWrappedContext();

        var query = context.Customers.ByKey(new Dictionary<string, object> { { "CustomerId", -10 } }).Orders.IncludeCount();
        var allOrdersCount = ((await query.ExecuteAsync()) as QueryOperationResponse<Order>).Count;

        bool CheckNextLink = false;
        Uri nextPageLink = null;

        EventHandler<SendingRequest2EventArgs> sendRequestEvent = (sender, args) =>
        {
            //The first request should not be checked.
            if (CheckNextLink)
            {
                Assert.Equal(nextPageLink.AbsoluteUri, args.RequestMessage.Url.AbsoluteUri);
            }
            CheckNextLink = true;
        };

        context.Configurations.ResponsePipeline.OnFeedEnded((args) =>
        {
            nextPageLink = args.Feed.NextPageLink;
        });

        context.SendingRequest2 += sendRequestEvent;

        //Navigation Property
        CheckNextLink = false;
        var queryOrderCount = (await context.Customers.ByKey(new Dictionary<string, object> { { "CustomerId", -10 } }).Orders.GetAllPagesAsync()).ToList().Count();
        Assert.Equal(allOrdersCount, queryOrderCount);
    }

    [Fact]
    public async Task GetPartialPagesAsyncTest_ShouldRetrievePartialPagesForQuery()
    {
        // Arrange
        var context = this.CreateWrappedContext();

        int count = 0;
        int sentRequestCount = 0;
        EventHandler<SendingRequest2EventArgs> sendRequestEvent = (sender, args) =>
        {
            sentRequestCount++;
        };

        context.SendingRequest2 += sendRequestEvent;

        // Act & Assert
        var customers = context.Customers.GetAllPagesAsync();
        Assert.Equal(1, sentRequestCount);
        foreach (var customer in await customers)
        {
            if (++count == 3)
            {
                break;
            }
        }
        //Only two Request sent
        Assert.Equal(2, sentRequestCount);
        this.EnqueueTestComplete();
    }

    #region Private

    private Container CreateWrappedContext()
    {
        var context = new Container(_baseUri)
        {
            HttpClientFactory = HttpClientFactory,
            UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses
        };

        ResetDefaultDataSource(context);

        return context;
    }

    /// <summary>
    /// Gets a dummy stream to use on MLE and Named Streams
    /// </summary>
    /// <returns>The stream</returns>
    private Stream GetStream()
    {
        return new MemoryStream(new byte[] { 64, 65, 66 });
    }

    private void ResetDefaultDataSource(Container context)
    {
        var actionUri = new Uri(_baseUri + "asyncmethodtests/Default.ResetDefaultDataSource", UriKind.Absolute);
        context.Execute(actionUri, "POST");
    }

    #endregion
}

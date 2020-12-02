//---------------------------------------------------------------------
// <copyright file="CancellationTokenTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.AsynchronousTests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.OData.Client;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.AstoriaDefaultServiceReference;
    using Xunit;
    using Xunit.Abstractions;

    /// <summary>
    /// CancellationToken tests using asynchronous APIs
    /// </summary>
    public class CancellationTokenTests : EndToEndTestBase
    {
        public CancellationTokenTests(ITestOutputHelper helper)
            : base(ServiceDescriptors.AstoriaDefaultService, helper)
        {
        }

        [Fact, Asynchronous]
        public async Task SaveChangesAsyncCancellationTokenTest()
        {
            var source = new CancellationTokenSource();
            var context = this.CreateWrappedContext<DefaultContainer>().Context;
            Customer c1 = new Customer { CustomerId = 11, Name = "customerOne" };
            context.AddToCustomer(c1);

            Task response() => context.SaveChangesAsync(source.Token);
            source.Cancel();
            var exception = await Assert.ThrowsAsync<OperationCanceledException>(response);
            Assert.Equal("The operation was canceled.", exception.Message);

            // SaveChangesAsync with SaveChangesOptions
            Customer c2 = new Customer { CustomerId = 22, Name = "customerTwo" };
            Customer c3 = new Customer { CustomerId = 33, Name = "customerThree" };
            context.AddToCustomer(c2);
            context.AddToCustomer(c3);

            Task response2() => context.SaveChangesAsync(SaveChangesOptions.BatchWithIndependentOperations, source.Token);
            source.Cancel();
            var exception2 = await Assert.ThrowsAsync<OperationCanceledException>(response2);
            Assert.Equal("The operation was canceled.", exception2.Message);

            this.EnqueueTestComplete();
        }

        [Fact, Asynchronous]
        public async Task GetValueAsyncCancellationTokenTest()
        {
            var source = new CancellationTokenSource();
            var context = this.CreateWrappedContext<DefaultContainer>().Context;
            Customer c1 = new Customer { CustomerId = 11, Name = "customerOne" };
            context.AddToCustomer(c1);
            await context.SaveChangesAsync();

            Task response() => context.Customer.ByKey(11).GetValueAsync(source.Token);
            source.Cancel();
            var exception = await Assert.ThrowsAsync<OperationCanceledException>(response);
            Assert.Equal("The operation was canceled.", exception.Message);

            this.EnqueueTestComplete();
        }

        [Fact, Asynchronous]
        public async Task ExecuteAsyncCancellationTokenTest()
        {
            var source = new CancellationTokenSource();
            var context = this.CreateWrappedContext<DefaultContainer>().Context;
            Customer c1 = new Customer { CustomerId = 11, Name = "customerOne" };
            context.AddToCustomer(c1);
            Customer c2 = new Customer { CustomerId = 22, Name = "customerTwo" };
            context.AddToCustomer(c2);
            await context.SaveChangesAsync();

            Task response() => context.Customer.ExecuteAsync(source.Token);
            source.Cancel();
            var exception = await Assert.ThrowsAsync<OperationCanceledException>(response);
            Assert.Equal("The operation was canceled.", exception.Message);

            // ExecuteAsync by continuation
            var customers = (await context.Customer.ExecuteAsync()) as QueryOperationResponse<Customer>;
            var count = customers.Count(); // continuation is only available when the result has been enumerated. Hence we call Count()
            var continuation = customers.GetContinuation();

            Task response2() => context.ExecuteAsync(continuation, source.Token);
            source.Cancel();
            var exception2 = await Assert.ThrowsAsync<OperationCanceledException>(response2);
            Assert.Equal("The operation was canceled.", exception2.Message);

            // ExecuteAsync by nextLink
            var customers2 = (await context.Customer.ExecuteAsync()) as QueryOperationResponse<Customer>;
            var count2 = customers2.Count(); // continuation is only available when the result has been enumerated. Hence we call Count()
            var continuation2 = (customers2 as QueryOperationResponse<Customer>).GetContinuation();

            Task response3() => context.ExecuteAsync<Customer>(continuation2.NextLinkUri, source.Token);
            source.Cancel();
            var exception3 = await Assert.ThrowsAsync<OperationCanceledException>(response3);
            Assert.Equal("The operation was canceled.", exception3.Message);

            this.EnqueueTestComplete();
        }

        [Fact, Asynchronous]
        public async Task GetAllPagesAsyncCancellationTokenTest()
        {
            var source = new CancellationTokenSource();
            var context = this.CreateWrappedContext<DefaultContainer>().Context;
            Customer c1 = new Customer { CustomerId = 11, Name = "customerOne" };
            context.AddToCustomer(c1);
            Customer c2 = new Customer { CustomerId = 22, Name = "customerTwo" };
            context.AddToCustomer(c2);
            await context.SaveChangesAsync();

            Task response() => context.Customer.GetAllPagesAsync(source.Token);
            source.Cancel();
            var exception = await Assert.ThrowsAsync<OperationCanceledException>(response);
            Assert.Equal("The operation was canceled.", exception.Message);

            this.EnqueueTestComplete();
        }

        [Fact, Asynchronous]
        public async Task LoadPropertyAsyncCancellationTokenTest()
        {
            var source = new CancellationTokenSource();
            var context = this.CreateWrappedContext<DefaultContainer>().Context;
            context.MergeOption = MergeOption.OverwriteChanges;
            Customer c1 = new Customer { CustomerId = 11, Name = "customerOne" };
            context.AddToCustomer(c1);
            await context.SaveChangesAsync();

            for (int i = 1; i <= 9; i++)
            {
                Order order = new Order() { OrderId = 1000 + i };
                context.AddToOrder(order);
                context.AddLink(c1, "Orders", order);
            }

            await context.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset);

            Task response() => context.LoadPropertyAsync(c1, "Orders", source.Token);
            source.Cancel();
            var exception = await Assert.ThrowsAsync<OperationCanceledException>(response);
            Assert.Equal("The operation was canceled.", exception.Message);

            //Get Entity by DataServiceQuery.ExecuteAsync
            var resp = (await ((DataServiceQuery<Customer>)(context.Customer.Expand(c => c.Orders).Where(c => c.CustomerId == 11))).ExecuteAsync()) as QueryOperationResponse<Customer>;
            var customer = resp.First();

            //Load navigation property by using continuation
            var continuation = resp.GetContinuation(customer.Orders);
            Task response2() => context.LoadPropertyAsync(customer, "Orders", continuation, source.Token);
            source.Cancel();
            var exception2 = await Assert.ThrowsAsync<OperationCanceledException>(response2);
            Assert.Equal("The operation was canceled.", exception2.Message);

            Task response3() => context.LoadPropertyAsync(customer, "Orders", continuation.NextLinkUri, source.Token);
            source.Cancel();
            var exception3 = await Assert.ThrowsAsync<OperationCanceledException>(response3);
            Assert.Equal("The operation was canceled.", exception3.Message);

            this.EnqueueTestComplete();
        }

        [Fact]
        public async Task GetReadStreamAsyncCancellationTokenTest()
        {
            var source = new CancellationTokenSource();
            var context = this.CreateWrappedContext<DefaultContainer>().Context;
            var car = new Car { VIN = 1000 };
            context.AddToCar(car);

            var mediaEntry = new MemoryStream(new byte[] { 64, 65, 66 });

            context.SetSaveStream(car, mediaEntry, true, "image/png", "UnitTestLogo.png");
            await context.SaveChangesAsync();

            Task response() => context.GetReadStreamAsync(car, new DataServiceRequestArgs(), source.Token);
            source.Cancel();
            var exception = await Assert.ThrowsAsync<OperationCanceledException>(response);
            Assert.Equal("The operation was canceled.", exception.Message);

            mediaEntry = new MemoryStream(new byte[] { 64, 65, 66 });
            context.SetSaveStream(car, "Photo", mediaEntry, true, new DataServiceRequestArgs { ContentType = "application/binary" });
            await context.SaveChangesAsync();

            Task response2() => context.GetReadStreamAsync(car, "Photo", new DataServiceRequestArgs { AcceptContentType = "application/binary" }, source.Token);
            source.Cancel();
            var exception2 = await Assert.ThrowsAsync<OperationCanceledException>(response2);
            Assert.Equal("The operation was canceled.", exception.Message);

            this.EnqueueTestComplete();
        }

        [Fact, Asynchronous]
        public async Task ExecuteBatchAsyncCancellationTokenTest()
        {
            var source = new CancellationTokenSource();
            var context = this.CreateWrappedContext<DefaultContainer>().Context;
            Customer c1 = new Customer { CustomerId = 11, Name = "customerOne" };
            context.AddToCustomer(c1);
            Customer c2 = new Customer { CustomerId = 22, Name = "customerTwo" };
            context.AddToCustomer(c2);
            await context.SaveChangesAsync();

            Task response() => context.ExecuteBatchAsync(
                SaveChangesOptions.BatchWithIndependentOperations | SaveChangesOptions.UseRelativeUri,
                source.Token,
                new DataServiceRequest[]
                {
                    new DataServiceRequest<Customer>(((context.Customer.Where(c => c.CustomerId == 11)) as DataServiceQuery<Customer>).RequestUri),
                    new DataServiceRequest<Customer>(((context.Customer.Where(c => c.CustomerId == 22)) as DataServiceQuery<Customer>).RequestUri)
                });

            source.Cancel();
            var exception = await Assert.ThrowsAsync<OperationCanceledException>(response);
            Assert.Equal("The operation was canceled.", exception.Message);

            this.EnqueueTestComplete();
        }
    }
}
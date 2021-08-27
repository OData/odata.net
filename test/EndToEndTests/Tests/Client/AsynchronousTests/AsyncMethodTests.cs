//---------------------------------------------------------------------
// <copyright file="AsyncMethodTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.AsynchronousTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.OData;
    using Microsoft.OData.Client;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.AstoriaDefaultServiceReference;
    using Xunit;
    using Xunit.Abstractions;


    /// <summary>
    /// Client update tests using asynchronous APIs
    /// </summary>
    public class AsyncMethodTests : EndToEndTestBase
    {
        public AsyncMethodTests(ITestOutputHelper helper)
            : base(ServiceDescriptors.AstoriaDefaultService, helper)
        {
        }

        [Fact, Asynchronous]
        public async Task SaveChangesTest()
        {
            var context = this.CreateWrappedContext<DefaultContainer>().Context;
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
            DataServiceCollection<Customer> customers = new DataServiceCollection<Customer>(context, "Customer", null, null);
            Customer c1 = new Customer();
            customers.Add(c1);
            c1.CustomerId = 1;
            c1.Name = "testName";

            //Partial Post an Entity
            expectedPropertyCount = 2;
            var response = await context.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties);
            Assert.True((response.First() as ChangeOperationResponse).StatusCode == 201, "StatusCode == 201");

            Order o1 = new Order { OrderId = 1000, CustomerId = 1, Concurrency = new ConcurrencyInfo() { Token = "token1" } };
            context.AddToOrder(o1);
            context.AddLink(c1, "Orders", o1);

            //Post with batch
            expectedPropertyCount = 2;
            var batchResponse = await context.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset);

            List<Order> orders = new List<Order>();
            for (int i = 1; i <= 9; i++)
            {
                Order order = new Order() { OrderId = 1000 + i };
                context.AddToOrder(order);
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
                    Assert.Equal("UpdatedToken", args.Entry.Properties.Single(p => p.Name == "Token").Value);
                }
            };
            context.Configurations.RequestPipeline.OnEntryEnding(onEntryEnding1);
            await context.SaveChangesAsync(SaveChangesOptions.None);

            // Batch relative URIs
            Customer c2 = new Customer { CustomerId = 11, Name = "customerTwo" };
            customers.Add(c2);

            var dataServiceResponse = await context.SaveChangesAsync(SaveChangesOptions.BatchWithIndependentOperations | SaveChangesOptions.UseRelativeUri);
            Assert.Equal(201, (dataServiceResponse.First() as ChangeOperationResponse).StatusCode);

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

            // Use client hooks to check response headers
            context.ReceivingResponse += (sender, eventArgs) =>
            {
                if (!eventArgs.IsBatchPart) // Check top level headers only
                {
                    Assert.Equal("application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8", eventArgs.ResponseMessage.GetHeader("Content-Type").Replace(" ", string.Empty));
                }
            };

            var dscResponse = await context.SaveChangesAsync(SaveChangesOptions.BatchWithIndependentOperations | SaveChangesOptions.UseJsonBatch);
            Assert.Equal(204, (dscResponse.First() as ChangeOperationResponse).StatusCode);

            this.EnqueueTestComplete();
        }

        [Fact, Asynchronous]
        public async Task JsonBatchSequencingPatchTest()
        {
            DefaultContainer context = this.CreateWrappedContext<DefaultContainer>().Context;
            Customer c1 = new Customer { CustomerId = 1, Name = "customerOne" };
            Customer c2 = new Customer { CustomerId = 2, Name = "customerTwo" };
            context.AddToCustomer(c1);
            context.AddToCustomer(c2);
            await context.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset);

            c1.Name = "customerOne updated name";
            c2.Name = "customerTwo updated name";

            context.UpdateObject(c1);
            context.UpdateObject(c2, c1);

            var response = await context.SaveChangesAsync(SaveChangesOptions.BatchWithIndependentOperations | SaveChangesOptions.UseJsonBatch);
            Assert.Equal(204, (response.Last() as ChangeOperationResponse).StatusCode);

            this.EnqueueTestComplete();
        }

        [Fact, Asynchronous]
        public async Task JsonBatchSequencingDeleteTest()
        {
            DefaultContainer context = this.CreateWrappedContext<DefaultContainer>().Context;
            Customer c1 = new Customer { CustomerId = 1, Name = "customerOne" };
            Customer c2 = new Customer { CustomerId = 2, Name = "customerTwo" };
            Customer c3 = new Customer { CustomerId = 3, Name = "customerThree" };
            context.AddToCustomer(c1);
            context.AddToCustomer(c2);
            context.AddToCustomer(c3);
            await context.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset);

            c1.Name = "customerOne updated name";
            c2.Name = "customerTwo updated name";

            context.UpdateObject(c1);
            context.UpdateObject(c2, c1);
            context.DeleteObject(c3, c2);

            var response = await context.SaveChangesAsync(SaveChangesOptions.BatchWithIndependentOperations | SaveChangesOptions.UseJsonBatch);
            Assert.Equal(204, (response.Last() as ChangeOperationResponse).StatusCode);

            this.EnqueueTestComplete();
        }

        [Fact, Asynchronous]
        public async Task JsonBatchSequencingSingeChangeSetTest()
        {
            DefaultContainer context = this.CreateWrappedContext<DefaultContainer>().Context;
            Customer c1 = new Customer { CustomerId = 1, Name = "customerOne" };
            Customer c2 = new Customer { CustomerId = 2, Name = "customerTwo" };
            Customer c3 = new Customer { CustomerId = 3, Name = "customerThree" };
            context.AddToCustomer(c1);
            context.AddToCustomer(c2);
            context.AddToCustomer(c3);
            await context.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset);

            c1.Name = "customerOne updated name";
            c2.Name = "customerTwo updated name";

            context.UpdateObject(c1);
            context.UpdateObject(c2, c1);
            context.DeleteObject(c3, c2);

            var response = await context.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset | SaveChangesOptions.UseJsonBatch);
            Assert.Equal(204, (response.First() as ChangeOperationResponse).StatusCode);

            this.EnqueueTestComplete();
        }

        /*[Fact, Asynchronous] */ // Activate this after solving error handling
        public async Task BatchSequencingTestFailsWithMultiPartMixed()
        {
            DefaultContainer context = this.CreateWrappedContext<DefaultContainer>().Context;
            Customer c1 = new Customer { CustomerId = 1, Name = "customerOne" };
            Customer c2 = new Customer { CustomerId = 2, Name = "customerTwo" };
            context.AddToCustomer(c1);
            context.AddToCustomer(c2);
            await context.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset);

            c1.Name = "customerOne updated name";
            c2.Name = "customerTwo updated name";

            context.UpdateObject(c1);
            context.UpdateObject(c2, c1);

            Task response() => context.SaveChangesAsync(SaveChangesOptions.BatchWithIndependentOperations);
            var exception = await Assert.ThrowsAsync<ODataException>(response);
            Assert.Contains("The dependsOn Id in request is not matching any of the request Id and atomic group Id seen so far", exception.Message);

            this.EnqueueTestComplete();
        }

        [Fact, Asynchronous]
        public async Task QueryEntitySetPagingTest()
        {
            var context = this.CreateWrappedContext<DefaultContainer>().Context;
            var query = context.Customer.IncludeCount();
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
            continuation = (response2 as QueryOperationResponse<Customer>).GetContinuation();
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

        [Fact, Asynchronous]
        public async Task LoadPropertyTest()
        {
            var context = this.CreateWrappedContext<DefaultContainer>().Context;
            context.MergeOption = MergeOption.OverwriteChanges;

            var person = (await context.Person.ExecuteAsync()).First() as SpecialEmployee;
            Assert.Null(person.Car);

            //Load Derived Navigation property
            await context.LoadPropertyAsync(person, "Car");
            Assert.NotNull(person.Car);

            //var c1 = (await context.Customer.ExecuteAsync()).First();
            var c1 = new Customer() { CustomerId = -10 };
            context.AttachTo("Customer", c1);

            for (int i = 1; i <= 9; i++)
            {
                Order order = new Order() { OrderId = 1000 + i };
                context.AddToOrder(order);
                context.AddLink(c1, "Orders", order);
            }

            //Post with batch
            await context.SaveChangesAsync(SaveChangesOptions.BatchWithSingleChangeset);

            //Get Entity by DataServiceQuery.ExecuteAsync
            var resp = (await ((DataServiceQuery<Customer>)(context.Customer.Expand(c => c.Orders).Where(c => c.CustomerId == -10))).ExecuteAsync()) as QueryOperationResponse<Customer>;
            var customer = resp.First();

            //Load navigation property by using continuation
            var continuation = resp.GetContinuation(customer.Orders);
            var orderResp = await context.LoadPropertyAsync(customer, "Orders", continuation) as QueryOperationResponse<Order>;
            Assert.True(customer.Orders.Count() == 4);

            //Load navigation property by using nextLink
            continuation = orderResp.GetContinuation();
            var orderResp2 = await context.LoadPropertyAsync(customer, "Orders", continuation.NextLinkUri);
            Assert.True(customer.Orders.Count() == 6);

            this.EnqueueTestComplete();
        }

        [Fact, Asynchronous]
        public async Task GetReadStreamTest()
        {
            var context = this.CreateWrappedContext<DefaultContainer>().Context;
            var car = new Car { VIN = 1000 };
            context.AddToCar(car);

            var mediaEntry = this.GetStream();
            var expectedString = new StreamReader(mediaEntry).ReadToEnd();

            // Get an image to set at the media entry
            mediaEntry = this.GetStream();
            context.SetSaveStream(car, mediaEntry, true, "image/png", "UnitTestLogo.png");
            await context.SaveChangesAsync();

            //gets the stream from the car in context and compares the values to what is in mediaEntry
            var receiveStream = (await context.GetReadStreamAsync(car, new DataServiceRequestArgs())).Stream;
            var sr2 = new StreamReader(receiveStream).ReadToEnd();
            Assert.Equal(expectedString, sr2);

            // Create Stream Property
            mediaEntry = this.GetStream();
            context.SetSaveStream(car, "Photo", mediaEntry, true, new DataServiceRequestArgs { ContentType = "application/binary" });
            await context.SaveChangesAsync();

            //gets the stream from the car/Photo in context
            receiveStream = (await context.GetReadStreamAsync(car, "Photo", new DataServiceRequestArgs { AcceptContentType = "application/binary" })).Stream;
            sr2 = new StreamReader(receiveStream).ReadToEnd();
            Assert.Equal(expectedString, sr2);

            this.EnqueueTestComplete();
        }

        /// <summary>
        /// Gets a dummy stream to use on MLE and Named Streams
        /// </summary>
        /// <returns>The stream</returns>
        private Stream GetStream()
        {
            return new MemoryStream(new byte[] { 64, 65, 66 });
        }

        [Fact, Asynchronous]
        public async Task ExecuteBatchTest()
        {
            var context = this.CreateWrappedContext<DefaultContainer>().Context;
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

            var qr = await context.ExecuteBatchAsync(new DataServiceRequest[] { new DataServiceRequest<Customer>(((from c in context.Customer where c.CustomerId == -8 select c) as DataServiceQuery<Customer>).RequestUri), new DataServiceRequest<Customer>(((from c in context.Customer where c.CustomerId == -6 select c) as DataServiceQuery<Customer>).RequestUri), new DataServiceRequest<Driver>(((from c in context.Driver where c.Name == "1" select c) as DataServiceQuery<Driver>).RequestUri), new DataServiceRequest<Driver>(((from c in context.Driver where c.Name == "3" select c) as DataServiceQuery<Driver>).RequestUri) });
            var actualValues = "";
            foreach (var r in qr)
            {
                if (r is QueryOperationResponse<Customer>)
                {
                    var customer = (r as QueryOperationResponse<Customer>).Single();
                    actualValues += customer.CustomerId;
                }

                if (r is QueryOperationResponse<Driver>)
                {
                    var driver = (r as QueryOperationResponse<Driver>).Single();
                    actualValues += driver.Name;
                }
            }

            Assert.Equal(actualValues, ("-8-613"));
            Assert.True(countOfBatchParts > 0 && (countOfTimesSenderCalled - countOfBatchParts) == 1, "countOfBatchParts > 0 && (countOfTimesSenderCalled - countOfBatchParts ) == 1");
            this.EnqueueTestComplete();
        }

        [Fact, Asynchronous]
        public async Task ExecuteBatchWithSaveChangesOptionsReturnsCorrectResults()
        {
            var context = this.CreateWrappedContext<DefaultContainer>().Context;
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

            var queryResponse = await context.ExecuteBatchAsync(
                SaveChangesOptions.BatchWithIndependentOperations | SaveChangesOptions.UseRelativeUri,
                new DataServiceRequest[] 
                {
                    new DataServiceRequest<Customer>(((context.Customer.Where(c => c.CustomerId == -8)) as DataServiceQuery<Customer>).RequestUri),
                    new DataServiceRequest<Customer>(((context.Customer.Where(c => c.CustomerId == -6)) as DataServiceQuery<Customer>).RequestUri),
                    new DataServiceRequest<Driver>(((context.Driver.Where(c => c.Name == "1")) as DataServiceQuery<Driver>).RequestUri),
                    new DataServiceRequest<Driver>(((context.Driver.Where(c => c.Name == "3")) as DataServiceQuery<Driver>).RequestUri)
                });
            var actualValues = "";
            foreach (var r in queryResponse)
            {
                if (r is QueryOperationResponse<Customer>)
                {
                    var customer = (r as QueryOperationResponse<Customer>).Single();
                    actualValues += customer.CustomerId;
                }

                if (r is QueryOperationResponse<Driver>)
                {
                    var driver = (r as QueryOperationResponse<Driver>).Single();
                    actualValues += driver.Name;
                }
            }

            bool isBatchPartsValid = countOfBatchParts > 0 && (countOfTimesSenderCalled - countOfBatchParts) == 1;

            Assert.Equal(actualValues, ("-8-613"));
            Assert.True(isBatchPartsValid, "countOfBatchParts > 0 && (countOfTimesSenderCalled - countOfBatchParts ) == 1");
            this.EnqueueTestComplete();
        }

        [Fact, Asynchronous]
        public async Task ActionFunction()
        {
            var context = this.CreateWrappedContext<DefaultContainer>().Context;
            context.MergeOption = MergeOption.OverwriteChanges;

            var queryable = ((DataServiceQuery<Employee>)context.Person.OfType<Employee>());
            var employees = (await queryable.ExecuteAsync()).ToList();
            var expectedEmployee0Salary = employees.First().Salary;

            //Execute Async with Uri and operation parameter
            await context.ExecuteAsync(new Uri(queryable.RequestUri.ToString() + "/Microsoft.Test.OData.Services.AstoriaDefaultService.IncreaseSalaries"),
                "POST",
                new BodyOperationParameter("n", 5));

            var currentEmployees = await queryable.ExecuteAsync();
            Assert.Equal(expectedEmployee0Salary + 5, currentEmployees.First().Salary);

            //ExecuteAsyncOfT with Uri and operation parameter
            await context.ExecuteAsync<int>(new Uri(queryable.RequestUri.ToString() + "/Microsoft.Test.OData.Services.AstoriaDefaultService.IncreaseSalaries"),
                "POST",
                new BodyOperationParameter("n", 5));

            currentEmployees = await queryable.ExecuteAsync();
            Assert.Equal(expectedEmployee0Salary + 10, currentEmployees.First().Salary);

            //ExecuteAsyncOfT which will return a singleResult
            int resultValue = (await context.ExecuteAsync<int>(new Uri("GetCustomerCount", UriKind.Relative), "GET", true)).Single();
            Assert.Equal(10, resultValue);
            this.EnqueueTestComplete();
        }

        [Fact, Asynchronous]
        public async Task GetAllPagesAsyncTest()
        {
            var context = this.CreateWrappedContext<DefaultContainer>().Context;

            var query = context.Customer.IncludeCount();
            var allCustomersCount = ((await query.ExecuteAsync()) as QueryOperationResponse<Customer>).Count;

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
            int queryCustomersCount = (await context.Customer.GetAllPagesAsync()).ToList().Count();
            Assert.Equal(allCustomersCount, queryCustomersCount);

            //$filter
            context.SendingRequest2 -= sendRequestEvent;
            query = ((DataServiceQuery<Customer>)context.Customer.Where(c => c.CustomerId > -5)).IncludeCount();
            var filterCustomersCount = ((await query.ExecuteAsync()) as QueryOperationResponse<Customer>).Count;

            context.SendingRequest2 += sendRequestEvent;
            CheckNextLink = false;
            queryCustomersCount = (await ((DataServiceQuery<Customer>)context.Customer.Where(c => c.CustomerId > -5)).GetAllPagesAsync()).ToList().Count();
            Assert.Equal(filterCustomersCount, queryCustomersCount);

            //$projection
            CheckNextLink = false;
            queryCustomersCount = (await ((DataServiceQuery<Customer>)context.Customer.Select(c => new Customer() { CustomerId = c.CustomerId, Name = c.Name })).GetAllPagesAsync()).ToList().Count();
            Assert.Equal(allCustomersCount, queryCustomersCount);

            //$expand
            CheckNextLink = false;
            queryCustomersCount = (await context.Customer.Expand(c => c.Orders).GetAllPagesAsync()).ToList().Count();
            Assert.Equal(allCustomersCount, queryCustomersCount);

            //$top
            CheckNextLink = false;
            queryCustomersCount = (await ((DataServiceQuery<Customer>)context.Customer.Take(4)).GetAllPagesAsync()).ToList().Count();
            Assert.Equal(4, queryCustomersCount);

            //$orderby
            CheckNextLink = false;
            queryCustomersCount = (await ((DataServiceQuery<Customer>)context.Customer.OrderBy(c => c.Name)).GetAllPagesAsync()).ToList().Count();
            Assert.Equal(allCustomersCount, queryCustomersCount);

            //$skip
            CheckNextLink = false;
            queryCustomersCount = (await ((DataServiceQuery<Customer>)context.Customer.Skip(4)).GetAllPagesAsync()).ToList().Count();
            Assert.Equal(allCustomersCount - 4, queryCustomersCount);
            this.EnqueueTestComplete();
        }

        [Fact]
        public async Task PagingOnNavigationProperty()
        {
            var context = this.CreateWrappedContext<DefaultContainer>().Context;

            var query = context.Customer.ByKey(new Dictionary<string, object> { { "CustomerId", -10 } }).Orders.IncludeCount();
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
            var queryOrderCount = (await context.Customer.ByKey(new Dictionary<string, object> { { "CustomerId", -10 } }).Orders.GetAllPagesAsync()).ToList().Count();
            Assert.Equal(allOrdersCount, queryOrderCount);
        }

        //[Fact, Asynchronous]
        //public async Task LoadNavigationPropertyAllPagesAsyncTest()
        //{
        //    var context = this.CreateWrappedContext<DefaultContainer>().Context;

        //    var orders = await context.CreateQuery<Order>("Customer(-10)/Orders").AddQueryOption("$count", "true").ExecuteAsync() as QueryOperationResponse<Order>;
        //    var customer = (await ((DataServiceQuery<Customer>)context.Customer.Where(c => c.CustomerId == -10)).ExecuteAsync()).Single();

        //    bool CheckNextLink = false;
        //    Uri nextPageLink = null;
        //    EventHandler<SendingRequest2EventArgs> sendRequestEvent = (sender, args) =>
        //    {
        //        //The first request should not be checked.
        //        if (CheckNextLink)
        //        {
        //            Assert.Equal(nextPageLink.AbsoluteUri, args.RequestMessage.Url.AbsoluteUri);
        //        }
        //        CheckNextLink = true;
        //    };

        //    context.Configurations.ResponsePipeline.OnFeedEnded((args) =>
        //    {
        //        nextPageLink = args.Feed.NextPageLink;
        //    });

        //    await context.LoadPropertyAllPagesAsync(customer, "Orders");
        //    int allOrdersCount = customer.Orders.Count;
        //    Assert.IsTrue(orders.TotalCount == allOrdersCount);
        //    this.EnqueueTestComplete();
        //}

        [Fact, Asynchronous]
        public async Task GetParitalPagesAsyncTest()
        {
            var context = this.CreateWrappedContext<DefaultContainer>().Context;

            int count = 0;
            int sentRequestCount = 0;
            EventHandler<SendingRequest2EventArgs> sendRequestEvent = (sender, args) =>
            {
                sentRequestCount++;
            };

            context.SendingRequest2 += sendRequestEvent;
            var customers = context.Customer.GetAllPagesAsync();
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

        [Fact, Asynchronous]
        public async Task UseDataServiceCollectionToTrackAllPages()
        {
            var context = this.CreateWrappedContext<DefaultContainer>().Context;
            var customerCount = ((await context.Customer.IncludeCount().ExecuteAsync()) as QueryOperationResponse<Customer>).Count;

            var customers = new DataServiceCollection<Customer>(context, await context.Customer.GetAllPagesAsync(), TrackingMode.AutoChangeTracking, null, null, null);
            Assert.Equal(customerCount, customers.Count());
            context.Configurations.RequestPipeline.OnEntryEnding((args) =>
            {
                Assert.Single(args.Entry.Properties);
            });
            for (int i = 0; i < customers.Count(); i++)
            {
                customers[i].Name = "Customer" + i.ToString();
            }
            await context.SaveChangesAsync();

            //customers = new DataServiceCollection<Customer>(await ((DataServiceQuery<Customer>)context.Customer.Take(1)).ExecuteAsync());
            //context.Configurations.RequestPipeline.OnEntryEnding((args) =>
            //{
            //    Assert.Equal(1, args.Entry.Properties.Count());
            //});
            //await context.LoadPropertyAllPagesAsync(customers[0], "Orders");
            //for (int i = 0; i < customers[0].Orders.Count(); i++)
            //{
            //    if (customers[0].Orders[i].Concurrency == null)
            //    {
            //        customers[0].Orders[i].Concurrency = new ConcurrencyInfo();
            //    }
            //    customers[0].Orders[i].Concurrency.Token = "Order_ConCurrency_" + i.ToString();

            //}
            //await context.SaveChangesAsync();

            //context.MergeOption = MergeOption.OverwriteChanges;
            //await context.LoadPropertyAllPagesAsync(customers[0], "Orders");
            //for (int i = 0; i < customers[0].Orders.Count(); i++)
            //{
            //    Assert.Equal(customers[0].Orders[i].Concurrency.Token, "Order_ConCurrency_" + i.ToString());
            //}
            this.EnqueueTestComplete();
        }
    }
}

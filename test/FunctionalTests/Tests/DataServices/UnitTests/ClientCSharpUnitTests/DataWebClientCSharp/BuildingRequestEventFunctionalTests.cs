//---------------------------------------------------------------------
// <copyright file="BuildingRequestEventFunctionalTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.DataWebClientCSharp
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using Microsoft.OData.Client;
    using System.Data.Test.Astoria;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.ServiceModel.Web;
    using System.Text;
    using System.Threading.Tasks;
    using AstoriaUnitTests.Stubs;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Functional tests for the BuildingRequest event. These tests add query parameters and headers in BuildingRequest and then
    /// verify that they were added in SendingRequest2 and in the service itself.
    /// </summary>
    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/881
    [Ignore] // Remove Atom
    // [TestClass]
    public class BuildingRequestEventFunctionalTests
    {
        #region Parameters and Constants

        private const string CustomQueryParameter = "Custom Query Parameter%&;#?";
        private const string SampleValue = "Test's Sample\tValue%&;#?!";
        private const string PostSpecificQueryParameter = "PostSpecificQueryParameter";
        private const string PostSpecificValue = "OtherValue";
        private const string CustomHeader = "CustomHeader";

        private const string EscapedCustomQueryParameter = "Custom%20Query%20Parameter%25%26%3B%23%3F";
        private const string EscapedSampleValue = "Test's%20Sample%09Value%25%26%3B%23%3F!";

        private const string ExpectedQueryItem = EscapedCustomQueryParameter + "=" + EscapedSampleValue;
        private const string ExpectedPostQueryItem = PostSpecificQueryParameter + "=" + PostSpecificValue;

        private int buildingRequestCallCount;
        private Queue<Descriptor> descriptors = new Queue<Descriptor>();

        private static readonly Dictionary<string, string> Headers = new Dictionary<string, string>
        {
            { CustomHeader, SampleValue},
            { "OData-Version", "4.0;" },
            { "OData-MaxVersion", "4.0;" },
            { "Accept", "bob/john" },
            //{ "Content-Type", "something/madeup" },
            { "If-Match", "my-own-guid-thing" },
            { "Prefer", "no idea" },
            { "X-Http-Method", "CONNECT" },
            //{ "Content-Length", "1" },
            { "Accept-Charset", "utf-4096" },
            //{ "Accept-Encoding", "my zipping thingy" },
            { "Accept-Language", "traditional estonian" },
            { "Accept-Datetime", "Jan 1st 2050" },
            //{ "User-Agent", "super client"},
            //{ "Transfer-Encoding", "chunked"},
        };

        #endregion

        #region Execute Tests

        [TestMethod]
        public void ExecutePostServiceOperation()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(ManyChangeValidatingService);
                web.StartService();

                var ctx = GetContextWithBuildingRequestHandler(web, false /*SendAlteredRequestToServer*/);
                AddDescriptorShouldBeNullVerifier(ctx);

                Action actionToTest = () => ctx.Execute(new Uri(web.ServiceRoot + "/VoidPostServiceOperation()"), "POST");

                RunTest(actionToTest);
                buildingRequestCallCount.Should().Be(1);
            }
        }

        [TestMethod]
        public void ExecuteGetServiceOperationWithExtraQueryItemsInExecute()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(ManyChangeValidatingService);
                web.StartService();

                var ctx = GetContextWithBuildingRequestHandler(web, false /*SendAlteredRequestToServer*/);
                AddDescriptorShouldBeNullVerifier(ctx);

                Action actionToTest = () => ctx.Execute(new Uri(web.ServiceRoot + "/VoidServiceOperation()?Foo=baz&bar= should have escaped this&NoValueThing"), "GET");

                RunTest(actionToTest);
                buildingRequestCallCount.Should().Be(1);
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void ExecuteUri()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(ManyChangeValidatingService);
                web.StartService();

                var ctx = GetContextWithBuildingRequestHandler(web);
                AddDescriptorShouldBeNullVerifier(ctx);

                Action actionToTest = () => ctx.Execute<Customer>(new Uri(web.ServiceRoot + "/Customers(1)"));

                RunTest(actionToTest);
                buildingRequestCallCount.Should().Be(1);
            }
        }

        [TestMethod]
        public void ExecuteUriAysnc()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(ManyChangeValidatingService);
                web.StartService();

                var ctx = GetContextWithBuildingRequestHandler(web, false /*SendAlteredRequestToServer*/);
                AddDescriptorShouldBeNullVerifier(ctx);

                Action actionToTest = () =>
                    {
                        IAsyncResult asyncResult = ctx.BeginExecute<Customer>(new Uri(web.ServiceRoot + "/Customers(1)"), null, null);
                        ctx.EndExecute<Customer>(asyncResult);
                    };

                RunTest(actionToTest);
                buildingRequestCallCount.Should().Be(1);
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void ExecuteUriPaging()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(ManyChangePagingValidatingService);
                web.StartService();

                var ctx = GetContextWithBuildingRequestHandler(web, args => args.RequestUri.AbsoluteUri.Contains("skiptoken"), args => args.RequestMessage.Url.AbsoluteUri.Contains("skiptoken"));
                //ctx.EnableAtom = true;
                //ctx.Format.UseAtom();
                AddDescriptorShouldBeNullVerifier(ctx);

                Action actionToTest = () =>
                    {
                        var dsq = ctx.CreateQuery<Customer>("Customers").Execute() as QueryOperationResponse<Customer>;
                        dsq.ToList();
                        DataServiceQueryContinuation<Customer> continuation = dsq.GetContinuation();
                        if (continuation != null)
                        {
                            ctx.Execute(continuation);
                        }
                    };

                RunTest(actionToTest);
                buildingRequestCallCount.Should().Be(2);
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void ExecuteUriPagingAsync()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(ManyChangePagingValidatingService);
                web.StartService();

                var ctx = GetContextWithBuildingRequestHandler(web, args => args.RequestUri.AbsoluteUri.Contains("skiptoken"), args => args.RequestMessage.Url.AbsoluteUri.Contains("skiptoken"));
                //ctx.EnableAtom = true;
                //ctx.Format.UseAtom();
                AddDescriptorShouldBeNullVerifier(ctx);

                Action actionToTest = () =>
                {
                    var dsq = ctx.CreateQuery<Customer>("Customers").Execute() as QueryOperationResponse<Customer>;
                    dsq.ToList();
                    DataServiceQueryContinuation<Customer> continuation = dsq.GetContinuation();
                    if (continuation != null)
                    {
                        IAsyncResult result = ctx.BeginExecute<Customer>(continuation, null, null);
                        ctx.EndExecute<Customer>(result);
                    }
                };

                RunTest(actionToTest);
                buildingRequestCallCount.Should().Be(2);
            }
        }
        #endregion

        #region Query, LoadPropety Tests

        [TestMethod]
        public void LinqQueryWithAddQueryOption()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(ManyChangeValidatingService);
                web.StartService();

                var ctx = GetContextWithBuildingRequestHandler(web, false /*SendAlteredRequestToServer*/);
                AddDescriptorShouldBeNullVerifier(ctx);

                var query = (DataServiceQuery<Customer>)(ctx.CreateQuery<Customer>("Customers").Where(c => c.Name.Contains("1")));
                query = query.AddQueryOption("ParameterFromLinqMethod", "value&split");

                Action actionToTest = () => query.Execute();

                RunTest(actionToTest);
                buildingRequestCallCount.Should().Be(1);
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void QueryAsync()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(ManyChangeValidatingService);
                web.StartService();

                var ctx = GetContextWithBuildingRequestHandler(web);
                AddDescriptorShouldBeNullVerifier(ctx);

                var query = (DataServiceQuery<Customer>)(ctx.CreateQuery<Customer>("Customers").Where(c => c.Name.Contains("1")));

                var taskFactory = new TaskFactory<IEnumerable<Customer>>();
                var result = taskFactory.FromAsync(query.BeginExecute, query.EndExecute, query);
                result.ContinueWith(task => Assert.Fail(), TaskContinuationOptions.NotOnFaulted);

                try
                {
                    result.Wait(10000).Should().BeTrue();
                }
                catch (AggregateException ex)
                {
                    string responseMessage = ex.InnerExceptions.Single().InnerException.Message;
                    responseMessage.Should().Contain("Server received user altered request correctly.");
                }

                buildingRequestCallCount.Should().Be(1);
            }
        }

        [TestMethod]
        public void LoadProperty()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(ManyChangeValidatingService);
                web.StartService();

                var ctx = GetContextWithBuildingRequestHandler(web, false /*SendAlteredRequestToServer*/);
                AddDescriptorShouldBeNullVerifier(ctx);

                ctx.ResolveName = T => T.FullName;
                Customer customer = new Customer() { ID = 1 };
                ctx.AttachTo("Customers", customer, "*");

                Action actionToTest = () => ctx.LoadProperty(customer, "Orders");

                RunTest(actionToTest);
                buildingRequestCallCount.Should().Be(1);
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void LoadPropertyAsync()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(ManyChangeValidatingService);
                web.StartService();

                var ctx = GetContextWithBuildingRequestHandler(web);
                AddDescriptorShouldBeNullVerifier(ctx);

                ctx.ResolveName = T => T.FullName;
                Customer customer = new Customer() { ID = 1 };
                ctx.AttachTo("Customers", customer, "*");

                Action actionToTest = () =>
                    {
                        IAsyncResult result = ctx.BeginLoadProperty(customer, "Orders", null, null);
                        ctx.EndLoadProperty(result);
                    };

                RunTest(actionToTest);
                buildingRequestCallCount.Should().Be(1);
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void LoadPropertyPaging()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(ManyChangePagingValidatingService);
                web.StartService();

                var ctx = GetContextWithBuildingRequestHandler(web, args => args.RequestUri.AbsoluteUri.Contains("skiptoken"), args => args.RequestMessage.Url.AbsoluteUri.Contains("skiptoken"));
                // ctx.EnableAtom = true;
                //ctx.Format.UseAtom();
                AddDescriptorShouldBeNullVerifier(ctx);

                ctx.ResolveName = T => T.FullName;
                Customer customer = new Customer() { ID = 1 };
                ctx.AttachTo("Customers", customer, "*");

                Action actionToTest = () =>
                    {
                        QueryOperationResponse qor = ctx.LoadProperty(customer, "Orders", new Uri(web.ServiceRoot + "/Customers(1)/Orders"));
                        QueryOperationResponse qor2 = ctx.LoadProperty(customer, "Orders", qor.GetContinuation());
                    };

                RunTest(actionToTest);
                buildingRequestCallCount.Should().Be(2);
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void LoadPropertyPagingAsync()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(ManyChangePagingValidatingService);
                web.StartService();

                var ctx = GetContextWithBuildingRequestHandler(web, args => args.RequestUri.AbsoluteUri.Contains("skiptoken"), args => args.RequestMessage.Url.AbsoluteUri.Contains("skiptoken"));
                // ctx.EnableAtom = true;
                // ctx.Format.UseAtom();
                AddDescriptorShouldBeNullVerifier(ctx);

                ctx.ResolveName = T => T.FullName;
                Customer customer = new Customer() { ID = 1 };
                ctx.AttachTo("Customers", customer, "*");

                Action actionToTest = () =>
                {
                    IAsyncResult result = ctx.BeginLoadProperty(customer, "Orders", new Uri(web.ServiceRoot + "/Customers(1)/Orders"), null, null);
                    QueryOperationResponse qor = ctx.EndLoadProperty(result);
                    IAsyncResult result2 = ctx.BeginLoadProperty(customer, "Orders", qor.GetContinuation(), null, null);
                    ctx.EndLoadProperty(result2);
                };

                RunTest(actionToTest);
                buildingRequestCallCount.Should().Be(2);
            }
        }

        [TestMethod]
        public void LinqQueryDataServiceCollection()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(ManyChangeValidatingService);
                web.StartService();

                var ctx = GetContextWithBuildingRequestHandler(web, false /*SendAlteredRequestToServer*/);
                AddDescriptorShouldBeNullVerifier(ctx);
                Action actionToTest = () =>
                {
                    var customerQuery = ctx.CreateQuery<Customer>("Customers").Expand("Orders");
                    DataServiceCollection<Customer> customers = new DataServiceCollection<Customer>(customerQuery);
                };

                RunTest(actionToTest);
                buildingRequestCallCount.Should().Be(1);
            }
        }
        #endregion

        #region CUD Tests

        [TestMethod]
        public void Delete()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(ManyChangeValidatingService);
                web.StartService();

                var ctx = GetContextWithBuildingRequestHandler(web, false /*SendAlteredRequestToServer*/);

                ctx.ResolveName = T => T.FullName;
                Customer customer = new Customer() { ID = 1 };
                ctx.AttachTo("Customers", customer, "*");

                AddDescriptorShouldBeEntityVerifier(ctx, ctx.GetEntityDescriptor(customer), descriptor =>
                {
                    descriptor.State.Should().Be(EntityStates.Deleted);
                    descriptor.Entity.Should().Be(customer);
                });

                ctx.DeleteObject(customer);
                Action actionToRun = () => ctx.SaveChanges();

                RunTest(actionToRun);
                buildingRequestCallCount.Should().Be(1);
            }
        }

        [TestMethod]
        public void Update()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(ManyChangeValidatingService);
                web.StartService();

                var ctx = GetContextWithBuildingRequestHandler(web, false /*SendAlteredRequestToServer*/);
                ctx.ResolveName = T => T.FullName;

                Customer customer = new Customer() { ID = 1 };
                ctx.AttachTo("Customers", customer, "*");

                AddDescriptorShouldBeEntityVerifier(ctx, ctx.GetEntityDescriptor(customer), descriptor =>
                {
                    descriptor.State.Should().Be(EntityStates.Modified);
                    descriptor.Entity.Should().Be(customer);
                });

                customer.Address = new Address() { City = "ABC", PostalCode = "98761", State = "HJI", StreetAddress = "111 222 Ave" };
                ctx.UpdateObject(customer); // just updating address
                Action actionToRun = () => ctx.SaveChanges();

                RunTest(actionToRun);
                buildingRequestCallCount.Should().Be(1);
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void Insert()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(ManyChangeValidatingService);
                web.StartService();

                var ctx = GetContextWithBuildingRequestHandler(web);
                ctx.ResolveName = T => T.FullName;

                Customer customer = new Customer() { ID = 3369, Name = "newly added customer" };

                AddDescriptorShouldBeEntityVerifier(ctx, descriptor =>
                {
                    descriptor.State.Should().Be(EntityStates.Added);
                    descriptor.Entity.Should().Be(customer);
                });

                ctx.AddObject("Customers", customer);
                Action actionToRun = () => ctx.SaveChanges();

                RunTest(actionToRun);
                buildingRequestCallCount.Should().Be(1);
            }
        }

        #endregion

        #region Link Tests
        [Ignore] // Remove Atom
        [TestMethod]
        public void SetLink()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(ManyChangeValidatingService);
                web.StartService();

                var ctx = GetContextWithBuildingRequestHandler(web);
                ctx.ResolveName = T => T.FullName;

                Customer customer1 = new Customer() { ID = 1 };
                ctx.AttachTo("Customers", customer1, "*");

                Customer customer2 = new Customer() { ID = 2 };
                ctx.AttachTo("Customers", customer2, "*");

                AddDescriptorShouldBeLinkVerifier(ctx, descriptor =>
                {
                    descriptor.Source.Should().BeSameAs(customer1);
                    descriptor.Target.Should().BeSameAs(customer2);
                    descriptor.State.Should().Be(EntityStates.Modified);
                });

                ctx.SetLink(customer1, "BestFriend", customer2);

                Action actionToRun = () => ctx.SaveChanges();

                RunTest(actionToRun);
                buildingRequestCallCount.Should().Be(1);
            }
        }

        [TestMethod]
        public void AddLink()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(ManyChangeValidatingService);
                web.StartService();

                var ctx = GetContextWithBuildingRequestHandler(web, false /*SendAlteredRequestToServer*/);
                ctx.ResolveName = T => T.FullName;

                Customer customer1 = new Customer() { ID = 1 };
                ctx.AttachTo("Customers", customer1, "*");

                Order order1 = new Order() { ID = 2 };
                ctx.AttachTo("Orders", order1, "*");

                AddDescriptorShouldBeLinkVerifier(ctx, descriptor =>
                {
                    descriptor.Source.Should().Be(customer1);
                    descriptor.Target.Should().Be(order1);
                    descriptor.State.Should().Be(EntityStates.Added);
                });

                ctx.AddLink(customer1, "Orders", order1);

                Action actionToRun = () => ctx.SaveChanges();

                RunTest(actionToRun);
                buildingRequestCallCount.Should().Be(1);
            }
        }

        [TestMethod]
        public void DeleteLink()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(ManyChangeValidatingService);
                web.StartService();

                var ctx = GetContextWithBuildingRequestHandler(web, false /*SendAlteredRequestToServer*/);
                ctx.ResolveName = T => T.FullName;

                Customer customer1 = new Customer() { ID = 1 };
                ctx.AttachTo("Customers", customer1, "*");

                Order order1 = new Order() { ID = 2 };
                ctx.AttachTo("Orders", order1, "*");

                AddDescriptorShouldBeLinkVerifier(ctx, descriptor =>
                {
                    descriptor.Source.Should().Be(customer1);
                    descriptor.Target.Should().Be(order1);
                    descriptor.State.Should().Be(EntityStates.Deleted);
                });

                ctx.DeleteLink(customer1, "Orders", order1);

                Action actionToRun = () => ctx.SaveChanges();

                RunTest(actionToRun);
                buildingRequestCallCount.Should().Be(1);
            }
        }

        [TestMethod]
        public void AddRelatedObject()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(ManyChangeValidatingService);
                web.StartService();

                var ctx = GetContextWithBuildingRequestHandler(web, false /*SendAlteredRequestToServer*/);
                ctx.ResolveName = T => T.FullName;

                Customer customer1 = new Customer() { ID = 1 };
                ctx.AttachTo("Customers", customer1, "*");

                Order order1 = new Order() { ID = 1239 };

                AddDescriptorShouldBeEntityVerifier(ctx, descriptor =>
                {
                    descriptor.State.Should().Be(EntityStates.Added);
                    descriptor.Entity.Should().Be(order1);
                    descriptor.ParentForInsert.Entity.Should().Be(customer1);
                });

                ctx.AddRelatedObject(customer1, "Orders", order1);
                Action actionToRun = () => ctx.SaveChanges();

                RunTest(actionToRun);
                buildingRequestCallCount.Should().Be(1);
            }
        }

        #endregion

        #region Batch Tests

        /// <summary>
        /// Inserts our plethora of query string parameters and headers onto the top level batch request and verifies that they 
        /// are added to the top level batch request (only).
        /// </summary>
        [Ignore] // Remove Atom
        [TestMethod]
        public void BatchTopLevel()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(ManyChangeValidatingService);
                web.StartService();

                var ctx = GetContextWithBuildingRequestHandler(web, args => args.RequestUri.AbsoluteUri.Contains("$batch"), args => args.RequestMessage.Url.AbsoluteUri.Contains("$batch"));
                ctx.SaveChangesDefaultOptions |= SaveChangesOptions.BatchWithSingleChangeset;

                ctx.BuildingRequest += (sender, e) =>
                {
                    if (e.RequestUri.AbsoluteUri.Contains("$batch"))
                    {
                        e.Descriptor.Should().BeNull();
                    }
                };
                ctx.SendingRequest2 += (sender, e) =>
                {
                    if (e.RequestMessage.Url.AbsoluteUri.Contains("$batch"))
                    {
                        e.Descriptor.Should().BeNull();
                    }
                };

                ctx.ResolveName = T => T.FullName;
                ctx.AddObject("Customers", new Customer() { Name = "Bob", ID = 6834 });
                ctx.AddObject("Customers", new Customer() { Name = "Sarah", ID = 4508 });

                Action actionToRun = () => ctx.SaveChanges();

                RunTest(actionToRun);
                buildingRequestCallCount.Should().Be(3); // 1 for outer batch, 2 for inner operations 
            }
        }

        /// <summary>
        /// Sends a batch request with two inserts. This test ensures that inner inserts have the additional parameters and headers added.
        /// We do NOT add the parameters and headers to the outer batch request, so the server can process it properly.
        /// </summary>
        [Ignore] // Remove Atom
        [TestMethod]
        public void BatchInserts()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(InnerBatchValidatingService);
                web.StartService();

                var ctx = GetContextWithBuildingRequestHandler(web, args => !args.RequestUri.AbsoluteUri.Contains("$batch"), args => !args.RequestMessage.Url.AbsoluteUri.Contains("$batch"));

                ctx.BuildingRequest += (sender, e) =>
                {
                    if (!e.RequestUri.AbsoluteUri.Contains("$batch"))
                    {
                        e.Descriptor.Should().NotBeNull();
                        EntityDescriptor entityDescriptor = e.Descriptor as EntityDescriptor;
                        entityDescriptor.Should().NotBeNull();
                        entityDescriptor.Entity.As<Customer>().Name.Should().Be("NewlyAddedCustomerName");
                    }
                };
                ctx.SendingRequest2 += (sender, e) =>
                {
                    if (!e.RequestMessage.Url.AbsoluteUri.Contains("$batch"))
                    {
                        e.Descriptor.Should().NotBeNull();
                        EntityDescriptor entityDescriptor = e.Descriptor as EntityDescriptor;
                        entityDescriptor.Should().NotBeNull();
                        entityDescriptor.Entity.As<Customer>().Name.Should().Be("NewlyAddedCustomerName");
                    }
                };

                ctx.ResolveName = T => T.FullName;
                ctx.AddObject("Customers", new Customer() { Name = "NewlyAddedCustomerName", ID = 6834 });
                ctx.AddObject("Customers", new Customer() { Name = "NewlyAddedCustomerName", ID = 4508 });

                Action actionToRun = () => ctx.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);

                RunTest(actionToRun);
                buildingRequestCallCount.Should().Be(3); // 1 for outer batch, 2 for the 2 inner insert operations
            }
        }

        /// <summary>
        /// Sends a batch request with a single query in it. This test ensures that inner query has the additional parameters and headers added.
        /// We do NOT add the parameters and headers to the outer batch request, so the server can process it properly.
        /// </summary>
        [Ignore] // Remove Atom
        [TestMethod]
        public void BatchQuery()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(InnerBatchValidatingService);
                web.StartService();

                var ctx = GetContextWithBuildingRequestHandler(web, args => !args.RequestUri.AbsoluteUri.Contains("$batch"), args => !args.RequestMessage.Url.AbsoluteUri.Contains("$batch"));
                AddDescriptorShouldBeNullVerifier(ctx);

                var response = ctx.ExecuteBatch((DataServiceRequest)(ctx.CreateQuery<Customer>("Customers").Where(c => c.Name.Contains("1"))));
                foreach (var result in response)
                {
                    result.StatusCode.Should().Be(418);
                    result.Error.Message.Should().Contain("Server received user altered request correctly.");
                }

                buildingRequestCallCount.Should().Be(2); // 1 for outer batch, 1 for inner query
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void BatchQueryAsync()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(InnerBatchValidatingService);
                web.StartService();

                var ctx = GetContextWithBuildingRequestHandler(web, args => !args.RequestUri.AbsoluteUri.Contains("$batch"), args => !args.RequestMessage.Url.AbsoluteUri.Contains("$batch"));
                AddDescriptorShouldBeNullVerifier(ctx);

                IAsyncResult asyncResult = ctx.BeginExecuteBatch(null, null, (DataServiceRequest)(ctx.CreateQuery<Customer>("Customers").Where(c => c.Name.Contains("1"))));
                var response = ctx.EndExecuteBatch(asyncResult);
                foreach (var result in response)
                {
                    result.StatusCode.Should().Be(418);
                    result.Error.Message.Should().Contain("Server received user altered request correctly.");
                }

                buildingRequestCallCount.Should().Be(2); // 1 for outer batch, 1 for inner query
            }
        }

        /// <summary>
        /// Call both AddObject on a Customer and AddRelatedObject on an Order then one batch SaveChanges call.
        /// </summary>
        [Ignore] // Remove Atom
        [TestMethod]
        public void BatchInsertAndAddRelatedObject()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(InnerBatchValidatingService);
                web.StartService();

                var ctx = GetContextWithBuildingRequestHandler(web, args => !args.RequestUri.AbsoluteUri.Contains("$batch"), args => !args.RequestMessage.Url.AbsoluteUri.Contains("$batch"));
                ctx.ResolveName = T => T.FullName;

                Customer customer1 = new Customer() { ID = 659 };
                Order order1 = new Order() { ID = 1239 };

                AddDescriptorShouldBeEntityVerifier(ctx, descriptor =>
                {
                    descriptor.State.Should().Be(EntityStates.Added);
                    if (this.buildingRequestCallCount == 2)
                    {
                        descriptor.Entity.Should().Be(customer1);
                    }
                    else if (this.buildingRequestCallCount == 3)
                    {
                        descriptor.Entity.Should().Be(order1);
                    }
                });

                ctx.AddObject("Customers", customer1);
                ctx.AddRelatedObject(customer1, "Orders", order1);
                Action actionToRun = () => ctx.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);

                RunTest(actionToRun);
                buildingRequestCallCount.Should().Be(3); // 1 for outer batch, 1 for insert customer, 1 for insert order
            }
        }

        #endregion

        #region Call Order Tests
        [Ignore] // Remove Atom
        [TestMethod]
        public void SimpleInsertRequestCallOrder()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(EndToEndTestService);
                web.StartService();

                var ctx = new DataServiceContext(web.ServiceRoot);
                // ctx.EnableAtom = true;
                // ctx.Format.UseAtom();
                ctx.ResolveName = T => T.FullName;
                string log = string.Empty;

                ctx.BuildingRequest += (sender, arg) => log += "_BR_";

                ctx.SendingRequest2 += (sender, arg) => log += "_SR2_";

                ctx.Configurations.RequestPipeline.OnMessageWriterSettingsCreated((c => log += "_WSC_"));

                ctx.Configurations.ResponsePipeline.OnMessageReaderSettingsCreated((c => log += "_RSC_"));

                ctx.ReceivingResponse += (sender, arg) => log += "_RR_";

                Customer customer = new Customer() { ID = 3370, Name = "new customer" };
                ctx.AddObject("Customers", customer);
                ctx.SaveChanges();

                Assert.AreEqual("_BR__SR2__WSC__RR__RSC_", log);
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void SimpleQueryCallOrder()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(EndToEndTestService);
                web.StartService();

                var ctx = new DataServiceContext(web.ServiceRoot);
                // ctx.EnableAtom = true;
                // ctx.Format.UseAtom();
                ctx.ResolveName = T => T.FullName;
                string log = string.Empty;

                ctx.BuildingRequest += (sender, arg) => log += "_BR_";

                ctx.SendingRequest2 += (sender, arg) => log += "_SR2_";

                ctx.Configurations.RequestPipeline.OnMessageWriterSettingsCreated((c => log += "_WSC_"));

                ctx.ReceivingResponse += (sender, arg) => log += "_RR_";

                ctx.Configurations.ResponsePipeline.OnMessageReaderSettingsCreated((c => log += "_RSC_"));

                ctx.CreateQuery<Customer>("Customers").ToList();

                Assert.AreEqual("_BR__SR2__RR__RSC_", log);
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void BatchQueryExecuteCallOrder()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(EndToEndTestService);
                web.StartService();

                var ctx = new DataServiceContext(web.ServiceRoot);
                // ctx.EnableAtom = true;
                // ctx.Format.UseAtom();
                ctx.ResolveName = T => T.FullName;
                string log = string.Empty;

                ctx.BuildingRequest += (sender, arg) => log += "_BR_";

                ctx.SendingRequest2 += (sender, arg) => log += "_SR2_";

                ctx.Configurations.RequestPipeline.OnMessageWriterSettingsCreated((c => log += "_WSC_"));

                ctx.ReceivingResponse += (sender, arg) => log += "_RR_";

                ctx.Configurations.ResponsePipeline.OnMessageReaderSettingsCreated((c => log += "_RSC_"));

                var response = ctx.ExecuteBatch((DataServiceRequest)(ctx.CreateQuery<Customer>("Customers")), (DataServiceRequest)(ctx.CreateQuery<Order>("Orders")));
                foreach (var result in response)
                {
                    result.Error.Should().BeNull();
                }

                var expectedLog = string.Concat("_BR__SR2__WSC_",
                    "_BR__SR2_",
                    "_BR__SR2__RR__RSC__RR__RSC__RR__RSC_");
                Assert.AreEqual(expectedLog, log);
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void BatchRequestCallOrder()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(EndToEndTestService);
                web.StartService();

                var ctx = new DataServiceContext(web.ServiceRoot);
                // ctx.EnableAtom = true;
                // ctx.Format.UseAtom();
                ctx.ResolveName = T => T.FullName;
                string log = string.Empty;

                ctx.BuildingRequest += (sender, arg) => log += "_BR_" + arg.RequestUri.Segments.Last();

                ctx.SendingRequest2 += (sender, arg) => log += "_SR2_" + arg.RequestMessage.Url.Segments.Last();

                ctx.Configurations.RequestPipeline.OnMessageWriterSettingsCreated((c => log += "_WSC_"));

                ctx.Configurations.ResponsePipeline.OnMessageReaderSettingsCreated((c => log += "_RSC_"));

                ctx.ReceivingResponse += (sender, arg) => log += "_RR_" + RetrieveLastSegmentOfRequestUri(arg);

                Customer newCustomer = new Customer() { ID = 3370, Name = "new customer" };
                ctx.AddObject("Customers", newCustomer);

                Customer existingCustomer = new Customer() { ID = 2, Name = "existing customer, new name" };
                ctx.AttachTo("Customers", existingCustomer, "*");
                ctx.UpdateObject(existingCustomer);

                Order newOrder = new Order() { ID = 399, DollarAmount = 3.99 };
                ctx.AddObject("Orders", newOrder);

                ctx.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);

                string expectedLog = string.Concat("_BR_$batch_SR2_$batch_WSC_",                 // BuildingRequest, SendingRequest2, MessageWriterSettings for $batch
                    "_BR_Customers_SR2_Customers_WSC_",                                          // BuildingRequest, SendingRequest2, MessageWriterSettings, WritingEntity for inner request 1
                    "_BR_Customers(2)_SR2_Customers(2)_WSC_",                                    // BuildingRequest, SendingRequest2, MessageWriterSettings, WritingEntity for inner request 2
                    "_BR_Orders_SR2_Orders_WSC_",                                                // BuildingRequest, SendingRequest2, MessageWriterSettings, WritingEntity for inner request 3
                    "_RR_$batch_RSC_",                                                           // ReceivingResponse, MessageReaderSettings for $batch
                    "_RR_Added_RSC_",                                                            // ReceivingResponse, MessageReaderSettings, ReadingEntity for inner request 1
                    "_RR_Modified",                                                              // ReceivingResponse for inner request 2
                    "_RR_Added_RSC_");                                                           // ReceivingResponse, MessageReaderSettings, ReadingEntity for inner request 3


                Assert.AreEqual(expectedLog, log);
            }
        }

        private string RetrieveLastSegmentOfRequestUri(ReceivingResponseEventArgs arg)
        {
            if (arg.IsBatchPart)
            {
                return arg.Descriptor.State.ToString();
            }
            else
            {
                return (arg.ResponseMessage as HttpWebResponseMessage).Response.ResponseUri.Segments.Last();
            }
        }

        #endregion

        #region Other Tests

        [Ignore] // Remove Atom
        [TestMethod]
        public void UnRegisterBuildingRequestEvent()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(EndToEndTestService);
                web.StartService();
                DataServiceContext ctx = new DataServiceContext(web.ServiceRoot);
                // ctx.EnableAtom = true;
                // ctx.Format.UseAtom();
                AddDescriptorShouldBeNullVerifier(ctx);

                ctx.BuildingRequest += SimpleBuildingRequest;
                ctx.Execute<Customer>(new Uri(web.ServiceRoot + "/Customers"));
                buildingRequestCallCount.Should().Be(1);

                buildingRequestCallCount = 0;
                ctx.BuildingRequest -= SimpleBuildingRequest;
                ctx.Execute<Customer>(new Uri(web.ServiceRoot + "/Customers"));
                buildingRequestCallCount.Should().Be(0);
            }
        }

        [TestMethod]
        public void ChangeRequestUriInBuildingRequestEvent()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(EndToEndTestService);
                web.StartService();
                DataServiceContext ctx = new DataServiceContext(web.ServiceRoot);
                AddDescriptorShouldBeNullVerifier(ctx);

                ctx.BuildingRequest += (sender, arg) => arg.RequestUri = new Uri(arg.RequestUri.AbsoluteUri + "abc");
                try
                {
                    ctx.Execute<Customer>(new Uri(web.ServiceRoot + "/Customers"));
                    Assert.Fail("Expected exception not thrown.");
                }
                catch (Exception e)
                {
                    Assert.IsTrue(e.InnerException.Message.Contains("Resource not found for the segment 'Customersabc'."));
                }
            }
        }

        [TestMethod]
        public void ChangeRequestMethodInBuildingRequestEvent()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(EndToEndTestService);
                web.StartService();
                DataServiceContext ctx = new DataServiceContext(web.ServiceRoot);
                AddDescriptorShouldBeNullVerifier(ctx);

                ctx.BuildingRequest += (sender, arg) => arg.Method = "POST";
                try
                {
                    ctx.Execute<Customer>(new Uri(web.ServiceRoot + "/Customers"));
                    Assert.Fail("Expected exception not thrown.");
                }
                catch (Exception e)
                {
                    Assert.IsTrue(e.InnerException.Message.Contains("The request must be chunked or have a content length."));
                }
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void ChangeRequestUriInBuildingRequestDoesNotOverrideOriginalQuery()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(EndToEndTestService);
                web.StartService();

                DataServiceContext ctx = new DataServiceContext(web.ServiceRoot);
                // ctx.EnableAtom = true;
                // ctx.Format.UseAtom();
                AddDescriptorShouldBeNullVerifier(ctx);

                ctx.BuildingRequest += (sender, arg) => arg.RequestUri = new Uri(web.ServiceRoot.AbsoluteUri + "/Customers(1)");

                var query = (DataServiceQuery<Customer>)(ctx.CreateQuery<Customer>("Customers"));
                query.Execute();
                Assert.AreEqual(web.ServiceRoot.AbsoluteUri + "/Customers", query.RequestUri.AbsoluteUri);
            }
        }

        [TestMethod]
        public void BuildingRequestReceivingResponseWithCancelRequest()
        {
            using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
            {
                web.DataServiceType = typeof(EndToEndTestService);
                web.StartService();
                DataServiceContext ctx = new DataServiceContext(web.ServiceRoot);
                AddDescriptorShouldBeNullVerifier(ctx);

                ctx.BuildingRequest += (sender, arg) => buildingRequestCallCount++;

                int receivingRequestCallCount = 0;
                ctx.ReceivingResponse += (sender, arg) => receivingRequestCallCount++;

                IAsyncResult result = ctx.BeginExecute<Customer>(new Uri(web.ServiceRoot + "/Customers"), null, null);
                ctx.CancelRequest(result);

                Exception expectedException = TestUtil.RunCatching(() => { ctx.EndExecute<Customer>(result); });
                Assert.AreEqual(DataServicesClientResourceUtil.GetString("Context_OperationCanceled"), expectedException.Message);

                buildingRequestCallCount.Should().Be(1);
                receivingRequestCallCount.Should().Be(0);
            }
        }

        /// <summary>
        /// This test exercises a combination of client CUD APIs with all SaveChanges options and verifies:
        /// - BuildingRequest/ReceivingResponse events called correctly
        /// - event argment descriptor
        /// - end-to-end scenario in which customer adds query string options in SendingRequest
        /// </summary>
        [Ignore] // Remove Atom
        [TestMethod]
        public void BuildingRequestReceivingResponseCUDEndToEndTest()
        {
            List<SaveChangesOptions> options = new List<SaveChangesOptions>()
                    {
                        SaveChangesOptions.None,
                        SaveChangesOptions.ContinueOnError,
                        SaveChangesOptions.ReplaceOnUpdate,
                        SaveChangesOptions.BatchWithSingleChangeset,
                    };

            foreach (SaveChangesOptions option in options)
            {
                using (TestWebRequest web = TestWebRequest.CreateForInProcessWcf())
                {
                    web.DataServiceType = typeof(EndToEndTestService);
                    web.StartService();
                    using (CustomDataContext.CreateChangeScope())
                    {
                        DataServiceContext ctx = new DataServiceContext(web.ServiceRoot, ODataProtocolVersion.V4);
                        // ctx.EnableAtom = true;
                        // ctx.Format.UseAtom();
                        ctx.ResolveName = T => T.FullName;
                        List<Descriptor> actualSendingRequestDescriptors = new List<Descriptor>() { };
                        List<Descriptor> actualReceivingResponseDescriptors = new List<Descriptor>() { };
                        ctx.BuildingRequest += (sender, arg) =>
                        {
                            // add custom query string option
                            string uriString = arg.RequestUri.IsAbsoluteUri ? arg.RequestUri.AbsoluteUri : arg.RequestUri.OriginalString;
                            arg.RequestUri = new Uri(uriString + "?CustomHeader=CustomHeaderValue&Custom_Header2=Custom_Header2_Value", UriKind.RelativeOrAbsolute);

                            if (arg.RequestUri.IsAbsoluteUri && arg.RequestUri.Segments.Last() == "$batch")
                            {
                                Assert.IsNull(arg.Descriptor);
                            }
                            else
                            {
                                if (arg.Method == "POST")
                                {
                                    Assert.AreEqual(EntityStates.Added, arg.Descriptor.State);
                                }
                                else if (arg.Method == "DELETE")
                                {
                                    Assert.AreEqual(EntityStates.Deleted, arg.Descriptor.State);
                                }
                                else
                                {
                                    Assert.AreEqual(EntityStates.Modified, arg.Descriptor.State);
                                }

                                actualSendingRequestDescriptors.Add(arg.Descriptor);
                            }
                        };

                        ctx.ReceivingResponse += (sender, arg) =>
                        {
                            // verify that the custom query option are processed by the service
                            Assert.AreEqual("CustomHeaderValue", arg.ResponseMessage.GetHeader("CustomHeader"));
                            Assert.AreEqual("Custom_Header2_Value", arg.ResponseMessage.GetHeader("Custom_Header2"));

                            if (!arg.IsBatchPart && (arg.ResponseMessage as HttpWebResponseMessage).Response.ResponseUri.Segments.Last() == "$batch")
                            {
                                Assert.IsNull(arg.Descriptor);
                            }
                            else
                            {
                                actualReceivingResponseDescriptors.Add(arg.Descriptor);
                            }
                        };

                        ctx.Timeout = (int)TimeSpan.FromMinutes(5).TotalMilliseconds;

                        Customer newCustomer = new Customer() { ID = 3369, Name = "newly added customer" };
                        Order newOrder = new Order() { ID = 3990, DollarAmount = 3.99 };
                        Order newOrder2 = new Order() { ID = 4990, DollarAmount = 4.99 };

                        ctx.AddObject("Customers", newCustomer);
                        ctx.AddObject("Orders", newOrder);

                        ctx.AddLink(newCustomer, "Orders", newOrder);
                        ctx.SetLink(newOrder, "Customer", newCustomer);
                        ctx.SaveChanges(option);

                        ctx.AddRelatedObject(newCustomer, "Orders", newOrder2);
                        newCustomer.Name = newCustomer.Name + " updated";
                        ctx.UpdateObject(newCustomer);
                        ctx.SaveChanges(option);

                        ctx.DeleteObject(newOrder2);
                        ctx.DeleteObject(newOrder);
                        ctx.DeleteObject(newCustomer);
                        ctx.SaveChanges(option);

                        // Verify that the ReceivingResponse count/descriptor is consistent with BuildingRequest count/descriptor
                        Assert.AreEqual(actualSendingRequestDescriptors.Count, actualReceivingResponseDescriptors.Count);
                        for (int i = 0; i < actualSendingRequestDescriptors.Count; i++)
                        {
                            Assert.AreSame(actualSendingRequestDescriptors[i], actualReceivingResponseDescriptors[i]);
                            if (!(option == SaveChangesOptions.BatchWithSingleChangeset && actualSendingRequestDescriptors[i].State == EntityStates.Modified && actualReceivingResponseDescriptors[i].State == EntityStates.Unchanged))
                            {
                                Assert.AreEqual(actualSendingRequestDescriptors[i].State, actualReceivingResponseDescriptors[i].State);
                            }
                        }
                    }
                }
            }
        }

        private void SimpleBuildingRequest(object sender, BuildingRequestEventArgs arg)
        {
            buildingRequestCallCount++;
        }

        #endregion       

        #region Utility Methods
        /// <summary>
        /// Runs the given action, and validates we get the special exception the dummy server throws.
        /// </summary>
        /// <param name="actionToTest">Test action to run.</param>
        private static void RunTest(Action actionToTest)
        {
            try
            {
                actionToTest();
            }
            catch (DataServiceRequestException ex)
            {
                Assert.IsTrue(ex.InnerException.Message.Contains("SendingRequest2 received user altered request correctly.") || ex.InnerException.Message.Contains("Server received user altered request correctly."));
                return;
            }
            catch (DataServiceQueryException ex)
            {
                ex.InnerException.Message.Should().Contain("Server received user altered request correctly.");
                ex.Response.StatusCode.Should().Be(418);
                return;
            }
            catch (DataServiceClientException ex)
            {
                Assert.IsTrue(ex.Message.Contains("SendingRequest2 received user altered request correctly.") || ex.Message.Contains("Server received user altered request correctly."));
                return;
            }
            catch (WebException ex)
            {
                new StreamReader(ex.Response.GetResponseStream()).ReadToEnd().Should().Contain("Server received user altered request correctly.");
                return;
            }

            Assert.Fail("Expected exception but received none.");
        }

        /// <summary>
        /// Builds a query string from a set of query items. The result will not include a leading question mark. The result is unescaped. 
        /// If queryStringItems is null or empty, this method will return an empty string.
        /// </summary>
        /// <param name="queryStringItems">Set of query string items.</param>
        /// <returns>An unescaped query string containing the query string items provided.</returns>
        private static string BuildQueryStringFromQueryParameters(IEnumerable<KeyValuePair<string, string>> queryStringItems)
        {
            if (queryStringItems == null)
            {
                return String.Empty;
            }

            bool needsAmpersand = false;
            StringBuilder queryStringBuilder = new StringBuilder();
            foreach (var parameter in queryStringItems)
            {
                if (!String.IsNullOrEmpty(parameter.Key))
                {
                    if (needsAmpersand)
                    {
                        queryStringBuilder.Append('&');
                    }

                    queryStringBuilder.Append(Uri.EscapeDataString(parameter.Key));
                    queryStringBuilder.Append('=');
                    queryStringBuilder.Append(Uri.EscapeDataString(parameter.Value));
                    needsAmpersand = true;
                }
            }

            return queryStringBuilder.ToString();
        }

        /// <summary>
        /// Builds a Uri from an string and a set of query string items. If the baseUri string contains a querystring already, then
        /// the query string items will be appended to the end of it. No duplicate checking is performed.
        /// </summary>
        /// <param name="baseUri">Base Uri to append query string items.</param>
        /// <param name="queryStringItems">Set of query string items.</param>
        /// <returns>A new Uri based on the given parameters.</returns>
        private static Uri AddQueryItemsToUri(Uri baseUri, IEnumerable<KeyValuePair<string, string>> queryStringItems)
        {
            if (baseUri == null)
            {
                throw new ArgumentException("UriPath cannot be null.");
            }

            string queryStringAdditions = BuildQueryStringFromQueryParameters(queryStringItems);

            if (String.IsNullOrEmpty(queryStringAdditions))
            {
                return baseUri;
            }

            StringBuilder builder = new StringBuilder();
            string baseUriAsString = baseUri.IsAbsoluteUri ? baseUri.AbsoluteUri : baseUri.OriginalString;
            builder.Append(baseUriAsString);

            if (String.IsNullOrEmpty(baseUri.Query))
            {
                builder.Append('?');
            }
            else if (baseUri.Query.Length > 0 && !baseUri.Query.EndsWith("&", StringComparison.Ordinal))
            {
                builder.Append('&');
            }

            builder.Append(queryStringAdditions);
            return new Uri(builder.ToString());
        }

        /// <summary>
        /// Adds code to verify that in BuildingRequest and SendingRequest2 we get a null Descriptor.
        /// </summary>
        /// <param name="ctx"></param>
        private static void AddDescriptorShouldBeNullVerifier(DataServiceContext ctx)
        {
            ctx.BuildingRequest += (sender, e) => e.Descriptor.Should().BeNull("no Descriptor should be present for this kind of request.");
            ctx.SendingRequest2 += (sender, e) => e.Descriptor.Should().BeNull("no Descriptor should be present for this kind of request.");
        }

        /// <summary>
        /// Adds code to verify that in BuildingRequest and SendingRequest2 we get a non-null EntityDescriptor that is the same object.
        /// Skips the check if $batch is in the URL.
        /// </summary>
        private static void AddDescriptorShouldBeEntityVerifier(DataServiceContext ctx, Action<EntityDescriptor> extraVerifier = null)
        {
            AddDescriptorShouldBeEntityVerifier(ctx, null, extraVerifier);
        }

        /// <summary>
        /// Adds code to verify that in BuildingRequest and SendingRequest2 we get a non-null EntityDescriptor that is the same object.
        /// Skips the check if $batch is in the URL.
        /// </summary>
        private static void AddDescriptorShouldBeEntityVerifier(DataServiceContext ctx, Descriptor expectedDescriptor, Action<EntityDescriptor> extraVerifier = null)
        {
            extraVerifier = extraVerifier ?? (d => { });
            bool expectedDescriptorProvided = expectedDescriptor != null;
            ctx.BuildingRequest += (sender, e) =>
            {
                if (!e.RequestUri.AbsoluteUri.Contains("$batch"))
                {
                    e.Descriptor.Should().NotBeNull("an EntityDescriptor was expected for this kind of request");
                    EntityDescriptor entityDescriptor = e.Descriptor as EntityDescriptor;
                    entityDescriptor.Should().NotBeNull("an EntityDescriptor was expected for this kind of request, but found: " + e.Descriptor);
                    extraVerifier(entityDescriptor);
                    expectedDescriptor = expectedDescriptorProvided ? expectedDescriptor : e.Descriptor;
                    e.Descriptor.Should().BeSameAs(expectedDescriptor);
                }
            };
            ctx.SendingRequest2 += (sender, e) =>
            {
                if (!e.RequestMessage.Url.AbsoluteUri.Contains("$batch"))
                {
                    e.Descriptor.Should().NotBeNull("an EntityDescriptor was expected for this kind of request");
                    EntityDescriptor entityDescriptor = e.Descriptor as EntityDescriptor;
                    entityDescriptor.Should().NotBeNull("an EntityDescriptor was expected for this kind of request, but found: " + e.Descriptor);
                    extraVerifier(entityDescriptor);
                    e.Descriptor.Should().BeSameAs(expectedDescriptor);
                    expectedDescriptor = expectedDescriptorProvided ? null : expectedDescriptor;
                }
            };
        }

        /// <summary>
        /// Adds code to verify that in BuildingRequest and SendingRequest2 we get a non-null StreamDescriptor that is the same object.
        /// Skips the check if $batch is in the URL.
        /// </summary>
        private void AddDescriptorShouldBeStreamVerifier(DataServiceContext ctx, Action<StreamDescriptor> extraVerifier = null)
        {
            extraVerifier = extraVerifier ?? (d => { });
            Descriptor descriptor = null;
            ctx.BuildingRequest += (sender, e) =>
            {
                if (!e.RequestUri.AbsoluteUri.Contains("$batch"))
                {
                    e.Descriptor.Should().NotBeNull("a StreamDescriptor was expected for this kind of request");
                    StreamDescriptor streamDescriptor = e.Descriptor as StreamDescriptor;
                    streamDescriptor.Should().NotBeNull("a StreamDescriptor was expected for this kind of request, but found: " + e.Descriptor);
                    extraVerifier(streamDescriptor);
                    descriptor = e.Descriptor;
                }
            };
            ctx.SendingRequest2 += (sender, e) =>
            {
                if (!e.RequestMessage.Url.AbsoluteUri.Contains("$batch"))
                {
                    e.Descriptor.Should().NotBeNull("a StreamDescriptor was expected for this kind of request");
                    StreamDescriptor streamDescriptor = e.Descriptor as StreamDescriptor;
                    streamDescriptor.Should().NotBeNull("a StreamDescriptor was expected for this kind of request, but found: " + e.Descriptor);
                    extraVerifier(streamDescriptor);
                    streamDescriptor.Should().BeSameAs(descriptor);
                }
            };

        }

        /// <summary>
        /// Adds code to verify that in BuildingRequest and SendingRequest2 we get a non-null LinkDescriptor that is the same object.
        /// </summary>
        private void AddDescriptorShouldBeLinkVerifier(DataServiceContext ctx, Action<LinkDescriptor> extraVerifier = null)
        {
            extraVerifier = extraVerifier ?? (d => { });
            Descriptor descriptor = null;
            ctx.BuildingRequest += (sender, e) =>
            {
                if (!e.RequestUri.AbsoluteUri.Contains("$batch"))
                {
                    e.Descriptor.Should().NotBeNull("a LinkDescriptor was expected for this kind of request");
                    LinkDescriptor linkDescriptor = e.Descriptor as LinkDescriptor;
                    linkDescriptor.Should().NotBeNull("a LinkDescriptor was expected for this kind of request, but found: " + e.Descriptor);
                    extraVerifier(linkDescriptor);
                    descriptor = e.Descriptor;
                }
            };
            ctx.SendingRequest2 += (sender, e) =>
            {
                if (!e.RequestMessage.Url.AbsoluteUri.Contains("$batch"))
                {
                    e.Descriptor.Should().NotBeNull("a LinkDescriptor was expected for this kind of request");
                    LinkDescriptor linkDescriptor = e.Descriptor as LinkDescriptor;
                    linkDescriptor.Should().NotBeNull("a LinkDescriptor was expected for this kind of request, but found: " + e.Descriptor);
                    extraVerifier(linkDescriptor);
                    linkDescriptor.Should().BeSameAs(descriptor);
                }
            };
        }

        private DataServiceContext GetContextWithBuildingRequestHandler(TestWebRequest web, bool SendAlteredRequestToServer = true)
        {
            return GetContextWithBuildingRequestHandler(web, args => true, args => true, SendAlteredRequestToServer);
        }

        private DataServiceContext GetContextWithBuildingRequestHandler(TestWebRequest web, Func<BuildingRequestEventArgs, bool> buildingRequestCondition, Func<SendingRequest2EventArgs, bool> sendingRequest2Condition, bool SendAlteredRequestToServer = true)
        {
            DataServiceContext ctx = new DataServiceContext(web.ServiceRoot, ODataProtocolVersion.V4);

            ctx.BuildingRequest += (sender, e) =>
            {
                buildingRequestCallCount++;

                VerifySystemHeadersAreSetBeforeBuildingRequest(e.Method, e.RequestUri, e.Headers);

                if (buildingRequestCondition(e))
                {
                    // Add a new custom query parameter, and other if it's POST
                    var queryParametersToAdd = new Dictionary<string, string> { { CustomQueryParameter, SampleValue } };
                    if (e.Method == "POST")
                    {
                        queryParametersToAdd.Add(PostSpecificQueryParameter, PostSpecificValue);
                    }

                    e.RequestUri = AddQueryItemsToUri(e.RequestUri, queryParametersToAdd);

                    // Override lots of headers and add a custom one
                    foreach (var header in Headers)
                    {
                        e.Headers[header.Key] = header.Value;
                    }
                }
            };

            // Setup Verification on client in SendingRequest2
            ctx.SendingRequest2 += (sender, e) =>
            {
                if (sendingRequest2Condition(e))
                {
                    foreach (var header in Headers)
                    {
                        e.RequestMessage.Headers.First(h => h.Key == header.Key).Value.Should().Be(header.Value);
                    }

                    Uri.UnescapeDataString(e.RequestMessage.Url.Query).Should().Contain(Uri.UnescapeDataString(ExpectedQueryItem));
                    (e.RequestMessage.Method.ToUpper() == "POST").Should().Be(e.RequestMessage.Url.Query.Contains(ExpectedPostQueryItem));

                    // To reduce the number of test requests, we verify the parameter and headers in SendingRequest2 in some tests and do not send the altered request to actual service
                    if (!SendAlteredRequestToServer)
                    {
                        throw new DataServiceClientException("SendingRequest2 received user altered request correctly.");
                    }
                }
            };

            return ctx;
        }

        private void VerifySystemHeadersAreSetBeforeBuildingRequest(string method, Uri requestUri, IDictionary<string, string> headers)
        {
            // Verify the system headers are already set when calling BuildingRequest event
            Assert.IsNotNull(headers["Accept"]);
            Assert.IsNotNull(headers["Accept-Charset"]);
            Assert.IsNotNull(headers["User-Agent"]);

            if (method != "GET" && method != "DELETE")
            {
                // In the case of a WebInvoke service operation without parameters, we don't set Content-Type for the POST request
                if (!(headers.ContainsKey("Content-Length") && headers["Content-Length"] == "0"))
                {
                    Assert.IsNotNull(headers["Content-Type"]);
                }
            }
        }

        #endregion

        #region Validation Servers

        private class ValidatingService : DataService<CustomDataContext>
        {
            private readonly Action<ProcessRequestArgs> handler;

            public ValidatingService(Action<ProcessRequestArgs> handler)
            {
                this.handler = handler;
            }

            public static void InitializeService(DataServiceConfiguration config)
            {
                config.SetEntitySetAccessRule("*", EntitySetRights.All);
                config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
                config.SetServiceActionAccessRule("*", ServiceActionRights.Invoke);
                config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
                config.UseVerboseErrors = true;
            }

            protected override void OnStartProcessingRequest(ProcessRequestArgs args)
            {
                base.OnStartProcessingRequest(args);
                handler(args);
            }

            [WebGet]
            public void VoidServiceOperation()
            {
            }

            [WebInvoke]
            public void VoidPostServiceOperation()
            {
            }
        }

        private class ManyChangeValidatingService : ValidatingService
        {
            public ManyChangeValidatingService()
                : base(RequireAllHeadersAndParameters)
            {
            }
        }

        private class ManyChangePagingValidatingService : ValidatingService
        {
            public ManyChangePagingValidatingService()
                : base(RequireAllHeadersAndParametersPagingTestsOnly)
            {
            }

            new public static void InitializeService(DataServiceConfiguration config)
            {
                config.SetEntitySetAccessRule("*", EntitySetRights.All);
                config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
                config.SetServiceActionAccessRule("*", ServiceActionRights.Invoke);
                config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
                config.UseVerboseErrors = true;
                config.SetEntitySetPageSize("Customers", 1);
                config.SetEntitySetPageSize("Orders", 1);
            }
        }

        private class InnerBatchValidatingService : ValidatingService
        {
            public InnerBatchValidatingService()
                : base(RequireAllHeadersAndParametersForInnerBatchOperationsOnly)
            {
            }
        }

        private class EndToEndTestService : ValidatingService
        {
            public EndToEndTestService()
                : base(WriteCustomResponseHeaders)
            {
            }
        }

        private static void RequireQueryParameter(ProcessRequestArgs args)
        {
            var value = args.OperationContext.GetQueryStringValue(CustomQueryParameter);

            if (value == null)
            {
                throw new DataServiceException(418, CustomQueryParameter + " was missing.");
            }

            if (value != SampleValue)
            {
                throw new DataServiceException(418, CustomQueryParameter + " is expected to always be " + SampleValue + ", but was " + value);
            }
        }

        private static void RequireAllHeadersAndParameters(ProcessRequestArgs args)
        {
            RequireQueryParameter(args);

            foreach (var header in Headers)
            {
                EnsureValueIsCorrect(header.Key, args.OperationContext.RequestHeaders.Get(header.Key), header.Value);
            }

            // If our checks have passed, we do a little hack to short circuit the rest of the server 
            throw new DataServiceException(418, "Server received user altered request correctly.");
        }

        private static void RequireAllHeadersAndParametersForInnerBatchOperationsOnly(ProcessRequestArgs args)
        {
            if (args.IsBatchOperation)
            {
                RequireAllHeadersAndParameters(args);
            }
        }

        private static void RequireAllHeadersAndParametersPagingTestsOnly(ProcessRequestArgs args)
        {
            // verify headers and parameters only for next link requests
            if (args.RequestUri.AbsoluteUri.Contains("skiptoken"))
            {
                RequireQueryParameter(args);

                foreach (var header in Headers)
                {
                    EnsureValueIsCorrect(header.Key, args.OperationContext.RequestHeaders.Get(header.Key), header.Value);
                }

                // If our checks have passed, we do a little hack to short circuit the rest of the server 
                throw new DataServiceException(418, "Server received user altered request correctly.");
            }
        }

        private static void EnsureValueIsCorrect(string headerName, string headerValue, string expected)
        {
            if (headerValue != expected)
            {
                throw new DataServiceException(418, "'" + headerName + "' is expected to always be '" + expected + "', but was: '" + headerValue + "'");
            }
        }

        private static void WriteCustomResponseHeaders(ProcessRequestArgs args)
        {
            // read custom query string headers and write corresponding response headers
            DataServiceOperationContext operationContext = args.OperationContext;
            var customHeaderValue = operationContext.GetQueryStringValue("CustomHeader");
            if (customHeaderValue != null)
            {
                operationContext.RequestHeaders.Set("CustomHeader", customHeaderValue);

                // write a new header in the response so we can verify the request headers were overridden
                operationContext.ResponseHeaders.Set("CustomHeader", operationContext.RequestHeaders["CustomHeader"]);
            }

            var customHeaderValue2 = operationContext.GetQueryStringValue("Custom_Header2");
            if (customHeaderValue2 != null)
            {
                operationContext.RequestHeaders.Set("Custom_Header2", customHeaderValue2);

                // write a new header in the response so we can verify the request headers were overridden
                operationContext.ResponseHeaders.Set("Custom_Header2", operationContext.RequestHeaders["Custom_Header2"]);
            }
        }

        #endregion
    }
}

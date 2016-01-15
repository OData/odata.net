//---------------------------------------------------------------------
// <copyright file="ClientQueryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Client;
    using Microsoft.Test.OData.Framework;
    using Microsoft.Test.OData.Framework.Client;
    using Microsoft.Test.OData.Framework.Verification;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.AstoriaDefaultServiceReference;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Generic client query test cases.
    /// </summary>
    [TestClass]
    public class ClientQueryTests : EndToEndTestBase
    {
        public ClientQueryTests()
            : base(ServiceDescriptors.AstoriaDefaultService)
        {
        }

        [TestMethod]
        public void NavigationPropertyOnEntityWithMultipleKeys()
        {
            this.RunOnAtomAndJsonFormats(
                this.CreateContext,
                (contextWrapper) =>
                {
                    // Entry Navigation Property (2 keys)
                    var entryResults = contextWrapper.Execute<Product>(new Uri(this.ServiceUri.OriginalString + "/OrderLine(OrderId=-10,ProductId=-10)/Product")).ToArray();
                    Assert.AreEqual(1, entryResults.Count(), "Unexpected number of Products returned");

                    var entryResultsLinq = contextWrapper.CreateQuery<OrderLine>("OrderLine").Where(o => o.OrderId == -10 && o.ProductId == -10).Select(o => o.Product).ToArray();
                    Assert.AreEqual(1, entryResultsLinq.Count(), "Unexpected number of Products returned");

                    var entryResultsLinqTwoWhereClauses = contextWrapper.CreateQuery<OrderLine>("OrderLine").Where(o => o.OrderId == -10).Where(o => o.ProductId == -10).Select(o => o.Product).ToArray();
                    Assert.AreEqual(1, entryResultsLinqTwoWhereClauses.Count(), "Unexpected number of Products returned");

                    // Entry Navigation Property (3 keys)
                    var entry3KeysResult = contextWrapper.Execute<Product>(new Uri(this.ServiceUri.OriginalString + "/ProductReview(ProductId=-10,ReviewId=-10,RevisionId='1')/Product")).ToArray();
                    Assert.AreEqual(1, entry3KeysResult.Count(), "Unexpected number of Products returned");

                    var entry3KeysLinqResult = contextWrapper.CreateQuery<ProductReview>("ProductReview").Where(pr => pr.ProductId == -10).Where(pr => pr.ReviewId == -10).Where(pr => pr.RevisionId == "1").Select(pr => pr.Product).ToArray();
                    Assert.AreEqual(1, entry3KeysLinqResult.Count(), "Unexpected number of Products returned");

                    // Feed Navigation Property (2 keys)
                    var feedResults = contextWrapper.Execute<MessageAttachment>(new Uri(this.ServiceUri.OriginalString + "/Message(FromUsername='1',MessageId=-10)/Attachments")).ToArray();
                    int feedResultsCount = feedResults.Count();
                    Assert.IsTrue(feedResultsCount > 1, "Expected more than one result, got {0}", feedResultsCount);

                    var feedResultsLinq = contextWrapper.CreateQuery<Message>("Message").Where(m => m.FromUsername == "1" && m.MessageId == -10).SelectMany(m => m.Attachments).ToArray();
                    Assert.AreEqual(feedResultsCount, feedResultsLinq.Count(), "Expected same number of results returned from Execute / Linq queries");
                });
        }

        private DataServiceContextWrapper<DefaultContainer> CreateContext()
        {
            return this.CreateWrappedContext<DefaultContainer>();
        }


        [TestMethod]
        public void ContainsUrlTest()
        {
            var testlists = new List<KeyValuePair<string, int>>()
                           {
                               new KeyValuePair<string, int>("/Person?$filter=contains(Name, 'm')", 10),
                               new KeyValuePair<string, int>("/Person?$filter=contains('m', Name)", 0),
                               new KeyValuePair<string, int>("/Person?$filter=not contains(Name, 'm')", 3),
                               new KeyValuePair<string, int>("/Person?$filter=contains(Name, 'm') eq true", 10),
                               new KeyValuePair<string, int>("/Person?$filter=false eq contains(Name, 'm')", 3),

                               new KeyValuePair<string, int>("/Person?$filter=contains(Name, substring('name',  2, 1))", 10),
                               new KeyValuePair<string, int>("/Person?$filter=contains(concat('User','name'), 'name')", 13),
                               new KeyValuePair<string, int>("/Person?$filter=contains(concat('User','name'), substring('name',  2, 1))", 13),
                           };

            var contextWrapper = this.CreateContext();

            foreach (var test in testlists)
            {
                var result = contextWrapper.Execute<Person>(new Uri(this.ServiceUri.OriginalString + test.Key)).ToArray();
                Assert.AreEqual(test.Value, result.Count(), "Unexpected number of Person returned");
            }
        }

        [TestMethod]
        public void ContainsLinqTest()
        {
            var context = this.CreateContext().Context;
            var result = (from c in context.Person
                          where c.Name.Contains("m")
                          select new Person() { Name = c.Name }) as DataServiceQuery<Person>;
            Assert.AreEqual(result.Count(), 10, "Unexpected number of Person returned");

            result = (DataServiceQuery<Person>)context.Person.Where(c => c.Name.Contains("m"));
            Assert.AreEqual(result.Count(), 10, "Unexpected number of Person returned");
        }

        [TestMethod]
        public void PrimitiveTypeInRequestUrlTest()
        {
            const string stringOfCast = "cast(PersonId,'Edm.Byte')";
            var context = this.CreateContext().Context;

            //GET http://jinfutanodata01:9090/AstoriaDefault635157546921762475/Person()?$filter=cast(cast(PersonId,'Edm.Byte'),'Edm.Int32')%20gt%200 HTTP/1.1
            //all the IDs in [-10, 2] except 0 are counted in.
            var result = context.Person.Where(c => (Byte)c.PersonId > 0);
            var stringOfQuery = result.ToString();
            Assert.IsTrue(stringOfQuery.Contains(stringOfCast), @"The URL should contains ""{0}"".", stringOfCast);
            Assert.AreEqual(12, result.Count(), "Unexpected number of Person returned");

            //GET http://jinfutanodata01:9090/AstoriaDefault635157551526070289/Person()?$filter=PersonId%20le%20256 HTTP/1.1
            //all the IDs in [1, 2] are counted in.
            result = context.Person.Where(c => c.PersonId > 0);
            Assert.AreEqual(2, result.Count(), "Unexpected number of Person returned");
        }

        [TestMethod]
        public void ContainsErrorTest()
        {
            string[] errorUrls =
            {
                "/Login?$filter=contains(Username, 1)", 
                "/Person?$filter=contains(Name, \"m\")",
                "/Car?$filter=contains(VIN, '12')"
            };

            var contextWrapper = this.CreateContext();
            foreach (var errorUrl in errorUrls)
            {
                try
                {
                    contextWrapper.Execute<Person>(new Uri(this.ServiceUri.OriginalString + errorUrl));
                    Assert.Fail("Expected Exception not thrown for " + errorUrl);
                }
                catch (DataServiceQueryException ex)
                {
                    Assert.AreEqual(400, ex.Response.StatusCode);
                }
            }
        }

        [TestMethod]
        public void QueryEntityNavigationWithImplicitKeys()
        {
            // this test is to baseline the WCF Data Service behavior that is not modified to support implicit keys 
            Dictionary<string, bool> testCases = new Dictionary<string, bool>()
            {
                {"Login('1')/SentMessages(FromUsername='1',MessageId=-10)", false /*expect error*/},
                {"Login('1')/SentMessages(MessageId=-10)", true /*expect error*/},
                {"Login('1')/SentMessages(-10)", true /*expect error*/},
            };

            var contextWrapper = this.CreateContext();
            foreach (var testCase in testCases)
            {
                try
                {
                    var message = contextWrapper.Execute<Message>(new Uri(this.ServiceUri.OriginalString.TrimEnd('/') + "/" + testCase.Key)).Single();
                    Assert.IsFalse(testCase.Value);
                    Assert.AreEqual(-10, message.MessageId);
                }
                catch (DataServiceQueryException ex)
                {
                    Assert.IsTrue(testCase.Value);
                    Assert.AreEqual(400, ex.Response.StatusCode);
                    StringResourceUtil.VerifyDataServicesString(ClientExceptionUtil.ExtractServerErrorMessage(ex), "BadRequest_KeyCountMismatch", "Microsoft.Test.OData.Services.AstoriaDefaultService.Message");
                }
            }
        }

        [TestMethod]
        public void MergeProjectionAndQueryOptionTest()
        {
            this.RunOnAtomAndJsonFormats(CreateContext, MergeProjectionAndQueryOptionTest);
        }

        private static void MergeProjectionAndQueryOptionTest(
            DataServiceContextWrapper<DefaultContainer> contextWrapper)
        {
            var query = from p in contextWrapper.Context.Product.AddQueryOption("$select", "ProductId")
                        where p.ProductId == -10
                        select new Product() { Description = p.Description, Photos = p.Photos };

            Uri uri = ((DataServiceQuery<Product>)query).RequestUri;
            Assert.AreEqual("?$expand=Photos&$select=Description,ProductId", uri.Query);
            Assert.IsTrue(query.ToList().Count == 1);
        }

        [TestMethod]
        public void DataServiceCollectionSubQueryTrackingItems()
        {
            this.RunOnAtomAndJsonFormats(CreateContext, DataServiceCollectionSubQueryTrackingItems);
        }

        private static void DataServiceCollectionSubQueryTrackingItems(
            DataServiceContextWrapper<DefaultContainer> contextWrapper)
        {
            var query = from p in contextWrapper.Context.Customer
                where p.Name != null
                select new Customer()
                {
                    Name = p.Name,
                    Orders = new DataServiceCollection<Order>(
                        from r in p.Orders
                        select new Order()
                        {
                            OrderId = r.OrderId,
                            CustomerId = r.CustomerId
                        })
                };
            var tmpResult0 = query.ToList()[0];
            DataServiceCollection<Order> collection = tmpResult0.Orders; // the collection tracking items
            int tmpCount = collection.Count;
            collection.Load(contextWrapper.Context.Execute(collection.Continuation));

            // for testing newly loaded item's tracking
            Assert.IsTrue(collection.Count > tmpCount, "Should have loaded another page.");
            bool someItemNotTracked = false;
            tmpResult0.Orders.ToList().ForEach(s =>
            {
                EntityStates state = contextWrapper.Context.GetEntityDescriptor(s).State;
                s.CustomerId = s.CustomerId + 1;
                state = contextWrapper.Context.GetEntityDescriptor(s).State;
                someItemNotTracked = (state == EntityStates.Unchanged) || someItemNotTracked;
            });
            Assert.IsFalse(someItemNotTracked, "All items should have been tracked.");
        }

        [TestMethod]
        public void DataServiceCollectionTrackingItems()
        {
            this.RunOnAtomAndJsonFormats(CreateContext, DataServiceCollectionTrackingItems);
        }

        private static void DataServiceCollectionTrackingItems(
            DataServiceContextWrapper<DefaultContainer> contextWrapper)
        {
            var query = from p in contextWrapper.Context.Customer
                where p.CustomerId > -100000
                // try to get many for paging
                select new Customer()
                {
                    CustomerId = p.CustomerId,
                    Name = p.Name
                };
            DataServiceCollection<Customer> collection = new DataServiceCollection<Customer>(query);

            // the collection to track items
            int tmpCount = collection.Count;
            collection.Load(contextWrapper.Context.Execute(collection.Continuation));

            // for testing newly loaded item's tracking
            Assert.IsTrue(collection.Count > tmpCount, "Should have loaded another page.");
            bool someItemNotTracked = false;
            collection.ToList().ForEach(s =>
            {
                s.Name = "value to test tracking";
                EntityStates state = contextWrapper.Context.GetEntityDescriptor(s).State;
                someItemNotTracked = (state == EntityStates.Unchanged) || someItemNotTracked;
            });
            Assert.IsFalse(someItemNotTracked, "All items should have been tracked.");
        }

        [TestMethod]
        public void GetAllPagesTest()
        {
            var context = this.CreateContext().Context;

            int allCustomersCount = context.Customer.Count();
            bool CheckNextLink = false;
            Uri nextPageLink = null;

            EventHandler<SendingRequest2EventArgs> sendRequestEvent = (sender, args) =>
            {
                //The first request should not be checked.
                if (CheckNextLink)
                {
                    Assert.AreEqual(nextPageLink.AbsoluteUri, args.RequestMessage.Url.AbsoluteUri);
                }
                CheckNextLink = true;
            };

            context.Configurations.ResponsePipeline.OnFeedEnded((args) =>
            {
                nextPageLink = args.Feed.NextPageLink;
            });

            context.SendingRequest2 += sendRequestEvent;
            int queryCustomersCount = context.Customer.GetAllPages().ToList().Count();
            Assert.AreEqual(allCustomersCount, queryCustomersCount);

            //$filter
            context.SendingRequest2 -= sendRequestEvent;
            var filterCustomersCount = context.Customer.Where(c => c.CustomerId > -5).Count();

            context.SendingRequest2 += sendRequestEvent;
            CheckNextLink = false;
            queryCustomersCount = ((DataServiceQuery<Customer>)context.Customer.Where(c => c.CustomerId > -5)).GetAllPages().ToList().Count();
            Assert.AreEqual(filterCustomersCount, queryCustomersCount);

            //$projection
            CheckNextLink = false;
            queryCustomersCount = ((DataServiceQuery<Customer>)context.Customer.Select(c => new Customer() { CustomerId = c.CustomerId, Name = c.Name })).GetAllPages().ToList().Count();
            Assert.AreEqual(allCustomersCount, queryCustomersCount);

            //$expand
            CheckNextLink = false;
            queryCustomersCount = context.Customer.Expand(c => c.Orders).GetAllPages().ToList().Count();
            Assert.AreEqual(allCustomersCount, queryCustomersCount);

            //$top
            CheckNextLink = false;
            queryCustomersCount = ((DataServiceQuery<Customer>)context.Customer.Take(4)).GetAllPages().ToList().Count();
            Assert.AreEqual(4, queryCustomersCount);

            //$orderby
            CheckNextLink = false;
            queryCustomersCount = ((DataServiceQuery<Customer>)context.Customer.OrderBy(c => c.Name)).GetAllPages().ToList().Count();
            Assert.AreEqual(allCustomersCount, queryCustomersCount);

            //$skip
            CheckNextLink = false;
            queryCustomersCount = ((DataServiceQuery<Customer>)context.Customer.Skip(4)).GetAllPages().ToList().Count();
            Assert.AreEqual(allCustomersCount - 4, queryCustomersCount);
        }

        [TestMethod]
        public void PagingOnNavigationProperty()
        {
            var context = this.CreateContext().Context;

            int allOrdersCount = context.Customer.ByKey(new Dictionary<string, object> { { "CustomerId", -10 } }).Orders.Count();
            bool CheckNextLink = false;
            Uri nextPageLink = null;

            EventHandler<SendingRequest2EventArgs> sendRequestEvent = (sender, args) =>
            {
                //The first request should not be checked.
                if (CheckNextLink)
                {
                    Assert.AreEqual(nextPageLink.AbsoluteUri, args.RequestMessage.Url.AbsoluteUri);
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
            var queryOrderCount = context.Customer.ByKey(new Dictionary<string, object> { { "CustomerId", -10 } }).Orders.GetAllPages().ToList().Count();
            Assert.AreEqual(allOrdersCount, queryOrderCount);
        }

        [TestMethod]
        public void GetParitalPagesTest()
        {
            var context = this.CreateContext().Context;

            int count = 0;
            int sentRequestCount = 0;
            EventHandler<SendingRequest2EventArgs> sendRequestEvent = (sender, args) =>
            {
                sentRequestCount++;
            };

            context.SendingRequest2 += sendRequestEvent;
            var customers = context.Customer.GetAllPages();
            Assert.AreEqual(1, sentRequestCount);
            foreach (var customer in customers)
            {
                if (++count == 3)
                {
                    break;
                }
            }
            //Only two Request sent
            Assert.AreEqual(2, sentRequestCount);
        }

        //[TestMethod]
        //public void LoadNavigationPropertyAllPagesTest()
        //{
        //    var context = this.CreateContext().Context;

        //    var orders = context.CreateQuery<Order>("Customer(-10)/Orders").AddQueryOption("$count", "true").Execute() as QueryOperationResponse<Order>;
        //    var customer = context.Customer.Where(c => c.CustomerId == -10).Single();

        //    bool CheckNextLink = false;
        //    Uri nextPageLink = null;
        //    EventHandler<SendingRequest2EventArgs> sendRequestEvent = (sender, args) =>
        //    {
        //        //The first request should not be checked.
        //        if (CheckNextLink)
        //        {
        //            Assert.AreEqual(nextPageLink.AbsoluteUri, args.RequestMessage.Url.AbsoluteUri);
        //        }
        //        CheckNextLink = true;
        //    };

        //    context.Configurations.ResponsePipeline.OnFeedEnded((args) =>
        //    {
        //        nextPageLink = args.Feed.NextPageLink;
        //    });

        //    context.LoadPropertyAllPages(customer, "Orders");
        //    int allOrdersCount = customer.Orders.Count;
        //    Assert.IsTrue(orders.TotalCount == allOrdersCount);
        //}
    }
}

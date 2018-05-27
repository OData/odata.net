//---------------------------------------------------------------------
// <copyright file="NWReadOnlyTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests
{
    using System;
    using Microsoft.OData.Client;
    using System.Diagnostics;
    using System.Net;
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading;
    using System.IO;
    using System.Collections;
    using System.Xml;
    using Microsoft.OData.Service;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public partial class ClientModule
    {
        // For comment out test cases, see github: https://github.com/OData/odata.net/issues/885
        /// <summary>
        /// Northwind database, read only tests
        /// </summary>
        [Ignore] // Remove Atom
        // [TestClass]
        public class ClientBasicsNWReadOnly
        {
            private static SimpleWorkspace northwindWorkspace;
            private static DataServiceHost northwindHost;

            private static SimpleWorkspace northwindWorkspacePaged;
            private static DataServiceHost northwindHostPaged;

            private static SimpleWorkspace nwServiceOpWorkspace;
            private static DataServiceHost nwServiceOpHost;

            private static IDisposable connection;
            private DataServiceContext ctx;

            private SimpleWorkspace NorthwindWorkspace
            {
                get
                {
                    if (northwindWorkspace == null)
                    {
                        Utils.CreateWorkspaceForType(typeof(SimpleDataService<northwind.northwindContext>), typeof(northwind.northwindContext), "Northwind", out northwindWorkspace, out northwindHost, true);
                    }
                    return northwindWorkspace;
                }
            }

            private SimpleWorkspace NorthwindWorkspacePaged
            {
                get
                {
                    if (northwindWorkspacePaged == null)
                    {
                        Utils.CreateWorkspaceForType(typeof(SimpleDataService<northwind.northwindContext>), typeof(northwind.northwindContext), "NorthwindPaged", out northwindWorkspacePaged, out northwindHostPaged, true);
                    }
                    return northwindWorkspacePaged;
                }
            }

            private SimpleWorkspace NWServiceOpWorkspace
            {
                get
                {
                    if (nwServiceOpWorkspace == null)
                    {
                        Utils.CreateWorkspaceForType(typeof(NWServiceOpService), typeof(northwind.northwindContext), "NWServiceOpWorkspace", out nwServiceOpWorkspace, out nwServiceOpHost, true);
                    }
                    return nwServiceOpWorkspace;
                }
            }

            [ClassInitialize]
            public static void ClassInitialize(TestContext testContext)
            {
                connection = CachedConnections.SetupConnection(CachedConnections.ConnectionType.Northwind);
            }

            [ClassCleanup]
            public static void ClassCleanUp()
            {
                if (connection != null)
                {
                    connection.Dispose();
                }
            }

            [TestInitialize]
            public void PerTestSetup()
            {
                SimpleWorkspace workspace = this.NorthwindWorkspace;
                Uri baseUri = new Uri(workspace.ServiceEndPoint + workspace.ServiceContainer.Name + ".svc", UriKind.Absolute);
                this.ctx = new northwindClient.northwindContext(baseUri);
                //this.ctx.EnableAtom = true;
                //this.ctx.Format.UseAtom();
                ctx.Timeout = TestConstants.MaxTestTimeout;
                Trace.WriteLine("Querying workspace at " + baseUri);
            }

            [TestCleanup]
            public void PerTestCleanup()
            {
                this.ctx = null;
            }

            [TestMethod]
            public void QueryFailureUsingExecuteBatch()
            {
                DataServiceRequest<northwindClient.Customers> request = new DataServiceRequest<northwindClient.Customers>(new Uri(ctx.BaseUri.OriginalString + "/Customers('QUICK')"));
                DataServiceRequest<northwindClient.Customers> request1 = new DataServiceRequest<northwindClient.Customers>(new Uri(ctx.BaseUri.OriginalString + "/Customers('NONEXIST')"));
                DataServiceResponse response = ctx.ExecuteBatch(request, request1);
                Utils.IsBatchResponse(response);

                List<QueryOperationResponse> responses = new List<QueryOperationResponse>();
                foreach (QueryOperationResponse queryResponse in response)
                {
                    responses.Add(queryResponse);
                }

                Assert.IsTrue(responses.Count == 2, "expecting 2 responses in batch query");

                // first one to succeed
                Utils.IsSuccessResponse(responses[0], HttpStatusCode.OK);
                Assert.IsTrue(responses[0].Query == request, "expecting the same request object");

                // expecting the second one to fail
                Utils.IsErrorResponse(responses[1], HttpStatusCode.NotFound, true);
                Assert.IsTrue(responses[1].Query == request1, "expecting the same request object1");
            }


            [TestMethod]
            public void QueryFailureUsingExecute()
            {
                Uri baseUri = ctx.BaseUri;

                Uri requestUri = new Uri(baseUri.OriginalString + "/Customers('VAR1')");
                try
                {
                    ctx.Execute<northwindClient.Customers>(requestUri);
                }
                catch (DataServiceQueryException ex)
                {
                    QueryOperationResponse response = ex.Response;
                    Utils.IsErrorResponse(response, HttpStatusCode.NotFound, false);
                    Assert.IsTrue(response.Query.RequestUri.Equals(requestUri), "expecting the same request uri");
                }
            }

            [TestMethod]
            public void QueryFailureUsingContextExecuteAsync()
            {
                Uri baseUri = ctx.BaseUri;
                Uri requestUri = new Uri(baseUri.OriginalString + "/Customers('VAR1')");

                try
                {
                    var q = ctx.EndExecute<northwindClient.Customers>(
                        ctx.BeginExecute<northwindClient.Customers>(requestUri, null, null));
                }
                catch (DataServiceQueryException ex)
                {
                    QueryOperationResponse response = ex.Response;
                    Utils.IsErrorResponse(response, HttpStatusCode.NotFound, false);
                    Assert.IsTrue(response.Query.RequestUri.Equals(requestUri), "expecting the same request uri");
                }
            }

            [TestMethod]
            public void QueryFailureUsingQueryExecute()
            {
                Uri baseUri = ctx.BaseUri;
                Uri requestUri = new Uri(baseUri.OriginalString + "/Customers('VAR1')");

                try
                {
                    var q = ctx.CreateQuery<northwindClient.Customers>("Customers('VAR1')");

                    q.Execute();
                }
                catch (DataServiceQueryException ex)
                {
                    QueryOperationResponse response = ex.Response;
                    Utils.IsErrorResponse(response, HttpStatusCode.NotFound, false);
                    Assert.IsTrue(response.Query.RequestUri.Equals(requestUri), "expecting the same request uri");
                }
            }

            [TestMethod]
            public void QueryFailureUsingQueryExecuteAsync()
            {
                Uri baseUri = ctx.BaseUri;
                Uri requestUri = new Uri(baseUri.OriginalString + "/Customers('VAR1')");

                try
                {
                    var q = ctx.CreateQuery<northwindClient.Customers>("Customers('VAR1')");

                    q.EndExecute(q.BeginExecute(null, null));
                }
                catch (DataServiceQueryException ex)
                {
                    QueryOperationResponse response = ex.Response;
                    Utils.IsErrorResponse(response, HttpStatusCode.NotFound, false);
                    Assert.IsTrue(response.Query.RequestUri.Equals(requestUri), "expecting the same request uri");
                }
            }


            [TestMethod]
            public void QueryExpandStronglyTypedProperties()
            {
                Uri baseUri = ctx.BaseUri;
                northwindClient.Products product =
                    ctx.Execute<northwindClient.Products>(new Uri(baseUri.OriginalString + "/Products?$top=1")).Single<northwindClient.Products>();

                Func<object, string, QueryOperationResponse> getResponse;

                for (int i = 0; i < 2; i++)
                {
                    if (i == 0)
                    {
                        getResponse = (o, p) =>
                        {
                            return ctx.LoadProperty(o, p);
                        };
                    }
                    else
                    {
                        // Async pattern
                        getResponse = (o, p) =>
                        {
                            IAsyncResult async = ctx.BeginLoadProperty(o, p, null, null);
                            if (!async.CompletedSynchronously)
                            {
                                Assert.IsTrue(async.AsyncWaitHandle.WaitOne(new TimeSpan(0, 0, TestConstants.MaxTestTimeout), false), "BeginLoadProperty timeout");
                            }

                            Assert.IsTrue(async.IsCompleted);
                            return ctx.EndLoadProperty(async);
                        };
                    }

                    int productID = ((QueryOperationResponse<int>)getResponse(product, "ProductID")).Single();
                    string productName = ((QueryOperationResponse<string>)getResponse(product, "ProductName")).Single();
                    short? reorderLevel = ((QueryOperationResponse<short?>)getResponse(product, "ReorderLevel")).Single();
                    northwindClient.Categories category = ((QueryOperationResponse<northwindClient.Categories>)getResponse(product, "Categories")).Single();
                    QueryOperationResponse<northwindClient.Order_Details> orderDetails = (QueryOperationResponse<northwindClient.Order_Details>)getResponse(product, "Order_Details");
                    Assert.IsTrue(orderDetails.Count() != 0, "There must be one or more order details");
                }
            }

            [TestMethod]
            public void QueryHeadersViaExecuteMethod()
            {
                Uri baseUri = ctx.BaseUri;

                // Accessing headers via DataServiceContext.Execute<> method
                Uri requestUri = new Uri(baseUri.OriginalString + "/Customers('QUICK')");
                var results = (QueryOperationResponse<northwindClient.Customers>)ctx.Execute<northwindClient.Customers>(requestUri);
                Utils.IsSuccessResponse(results, HttpStatusCode.OK);
                // assert that there is exactly one object
                results.Single<northwindClient.Customers>();

                // Accessing headers via DataServiceContext.BeginExecute/EndExecute method
                IAsyncResult asyncResult = ctx.BeginExecute<northwindClient.Customers>(requestUri, null, null);
                results = (QueryOperationResponse<northwindClient.Customers>)ctx.EndExecute<northwindClient.Customers>(asyncResult);
                Utils.IsSuccessResponse(results, HttpStatusCode.OK);
                // assert that there is exactly one object
                results.Single<northwindClient.Customers>();

                // Accessing headers via DataServiceQuery<>.Execute method
                DataServiceQuery<northwindClient.Customers> query = ctx.CreateQuery<northwindClient.Customers>("Customers");
                results = (QueryOperationResponse<northwindClient.Customers>)query.Execute();
                Utils.IsSuccessResponse(results, HttpStatusCode.OK);
                Assert.IsTrue(results.Count<northwindClient.Customers>() > 0, "expecting atleast one object");

                // Accessing headers via DataServiceQuery<>.BeginExecute/EndExecute method
                asyncResult = query.BeginExecute(null, null);
                results = (QueryOperationResponse<northwindClient.Customers>)query.EndExecute(asyncResult);
                Utils.IsSuccessResponse(results, HttpStatusCode.OK);
                Assert.IsTrue(results.Count<northwindClient.Customers>() > 0, "expecting atleast one object");
            }

            [TestMethod]
            public void QueryServiceOpTests()
            {
                var queries = new[] {
                    // Get:
                    new { uri = "/GetCustomerById?customerId='QUICK'", xpath = "/atom:entry/atom:category", method="GET", mime="application/atom+xml" },
                    new { uri = "/GetCustomersEnumerable", xpath="/atom:feed[count(atom:entry)=5]", method="GET", mime="application/atom+xml" },
                    new { uri = "/GetAllCustomers", xpath="/atom:feed[count(atom:entry)=5]", method="GET", mime="application/atom+xml" },
                    new { uri = "/GetAllCustomers()", xpath="/atom:feed[count(atom:entry)=5]", method="GET", mime="application/atom+xml" },
                    new { uri = "/GetAllCustomers('ANATR')", xpath="/atom:entry/atom:category", method="GET", mime="application/atom+xml" },
                    
                    // primitive type:
                    new { uri = "/GetCustomerNameById?customerId='QUICK'", xpath="/adsm:value", method="GET", mime="application/xml" },
                    new { uri = "/GetCustomerNamesEnumerable", xpath="/adsm:value[count(adsm:element)=5]", method="GET", mime="application/xml" },

                    // Posts
                    new { uri = "/GetCustomersByIdPOST?customerId='QUICK'", xpath="/atom:feed[count(atom:entry)=1]", method="POST", mime="application/atom+xml" },
                };

                using (Utils.ConfigurationCacheCleaner())
                {
                    SimpleWorkspace workspace = this.NWServiceOpWorkspace;
                    Uri baseUri = new Uri(workspace.ServiceEndPoint + workspace.ServiceContainer.Name + ".svc", UriKind.Absolute);

                    foreach (var q in queries)
                    {
                        Trace.WriteLine(q.uri);
                        using (Stream response = Utils.ProcessWebRequest(new Uri(baseUri + q.uri, UriKind.Absolute), q.mime, q.method))
                        {
                            XmlDocument document = new XmlDocument(Utils.TestNameTable);
                            document.Load(response);
                            Assert.IsTrue(document.SelectNodes(q.xpath, Utils.TestNamespaceManager).Count > 0, q.xpath);
                        }
                    }
                }
            }

            #region Client Row Count

            [TestMethod]
            public void QueryRowCountBatchRequest()
            {
                DataServiceRequest[] queries = new DataServiceRequest[] {
                        (DataServiceRequest)(from c in ctx.CreateQuery<northwindClient.Customers>("Customers").IncludeTotalCount() select c).Take(1),
                        (DataServiceRequest)(from c in ctx.CreateQuery<northwindClient.Customers>("Customers").IncludeTotalCount().Where(c=>c.ContactTitle=="Owner") select c).Take(1),
                        (DataServiceRequest)(from c in ctx.CreateQuery<northwindClient.Orders>("Orders").IncludeTotalCount().Expand("Order_Details") select c).Take(1),
                        (DataServiceRequest)(ctx.CreateQuery<northwindClient.Customers>("Customers").IncludeTotalCount().Where(c=>c.CustomerID=="QUICK").SelectMany(c => c.Orders)).Take(1)
                    };

                DataServiceResponse response = ctx.ExecuteBatch(queries);
                Utils.IsBatchResponse(response);

                foreach (QueryOperationResponse r in response)
                {
                    long count = r.TotalCount;
                    Assert.IsTrue(count > 0);
                }
            }

            [TestMethod]
            public void QueryRowCountWithLinks()
            {
                var q = (QueryOperationResponse<northwindClient.Customers>)
                    ctx.Execute<northwindClient.Customers>("/Customers('QUICK')/Orders/$ref?$count=true");
                long countValue = q.TotalCount;
                Assert.AreEqual(1, countValue);
            }

            [TestMethod]
            public void QueryRowCountInline()
            {
                var q = from c in ctx.CreateQuery<northwindClient.Customers>("Customers").IncludeTotalCount()
                        select c;

                var ie = ((DataServiceQuery<northwindClient.Customers>)q).Execute();
                QueryOperationResponse<northwindClient.Customers> dsr = ie as QueryOperationResponse<northwindClient.Customers>;

                long sc1 = dsr.TotalCount;      // server count 1

                int cc = 0;                     // client count
                foreach (northwindClient.Customers c in ie)
                {
                    ++cc;
                }

                long sc2 = dsr.TotalCount;      // server count 2 (after enumeration)

                Assert.AreEqual(sc1, sc2);
                Assert.AreEqual(sc1, cc, "Server count matches client count");
            }

            [TestMethod]
            public void QueryRowCountInlineAndValue()
            {
                DataServiceQuery<northwindClient.Customers> q = (DataServiceQuery<northwindClient.Customers>)(from c in ctx.CreateQuery<northwindClient.Customers>("Customers").IncludeTotalCount()
                                                                                                              select c);

                QueryOperationResponse<northwindClient.Customers> r = (QueryOperationResponse<northwindClient.Customers>)q.Execute();
                long sc1 = r.TotalCount;

                q = (DataServiceQuery<northwindClient.Customers>)(from c in ctx.CreateQuery<northwindClient.Customers>("Customers")
                                                                  select c);
                long sc2 = q.LongCount();

                Assert.AreEqual(sc1, sc2);
            }

            [TestMethod]
            public void QueryRowCountUriBuilder()
            {
                var baseQuery = ctx.CreateQuery<northwindClient.Orders>("Orders").IncludeTotalCount();
                // having both $count=inline & $count=value - should fail
                try
                {
                    long sc2 = baseQuery.LongCount();
                    Assert.Fail("Duplicating $count did not throw");
                }
                catch (NotSupportedException ex)
                {
                    string expectedMsg = DataServicesClientResourceUtil.GetString("ALinq_CannotAddCountOptionConflict");
                    Assert.AreEqual(expectedMsg, ex.Message);
                }

                try
                {
                    var customQuery = baseQuery.AddQueryOption("$count", "true");
                    customQuery.Execute();
                    Assert.Fail("Duplicating query $count did not throw");
                }
                catch (NotSupportedException ex)
                {
                    string expectedMsg = DataServicesClientResourceUtil.GetString("ALinq_CantAddAstoriaQueryOption", "$count");
                    Assert.AreEqual(expectedMsg, ex.Message);
                }
            }

            [TestMethod]
            public void QueryRowCountInlineAndValueWithKeyPredicate()
            {
                DataServiceQuery<northwindClient.Customers> q =
                    ((DataServiceQuery<northwindClient.Customers>)
                    (from c in ctx.CreateQuery<northwindClient.Customers>("Customers").Where(cc => cc.CustomerID == "ALFKI")
                     select c)).IncludeTotalCount();

                QueryOperationResponse<northwindClient.Customers> r = (QueryOperationResponse<northwindClient.Customers>)q.Execute();
                long sc1 = r.TotalCount;

                q = (DataServiceQuery<northwindClient.Customers>)
                    (from c in ctx.CreateQuery<northwindClient.Customers>("Customers").Where(cc => cc.CustomerID == "ALFKI")
                     select c);
                long sc2 = q.LongCount();

                Assert.AreEqual(sc1, sc2);
            }

            [TestMethod]
            public void QueryRowCountAsync()
            {
                bool asyncComplete = false;
                long countValue = 0;
                var q = ctx.CreateQuery<northwindClient.Customers>("Customers").IncludeTotalCount();
                q.BeginExecute(
                    ar =>
                    {
                        try
                        {
                            QueryOperationResponse<northwindClient.Customers> qor = (QueryOperationResponse<northwindClient.Customers>)q.EndExecute(ar);
                            countValue = qor.TotalCount;
                        }
                        finally
                        {
                            asyncComplete = true;
                        }
                    }, null);

                while (!asyncComplete)
                {
                    Thread.Sleep(10);
                }
                Assert.AreEqual(15, countValue);
            }

            [TestMethod]
            public void QueryRowCountNotFoundException()
            {
                var baseQuery = ctx.CreateQuery<northwindClient.Customers>("Customer");
                var inlineQuery = baseQuery.IncludeTotalCount();
                string countNotPresentMsg = DataServicesClientResourceUtil.GetString("MaterializeFromAtom_CountNotPresent");
                string resourceNotFoundCustomerMsg = DataServicesResourceUtil.GetString("RequestUriProcessor_ResourceNotFound", "Customer");
                string resourceNotFoundVar1Msg = DataServicesResourceUtil.GetString("RequestUriProcessor_ResourceNotFound", "VAR1");

                for (int i = 0; i < 2; ++i)
                {
                    ctx.IgnoreResourceNotFoundException = (i == 0);

                    // case 1: $count=inline, 404 exception gives empty set
                    try
                    {
                        QueryOperationResponse<northwindClient.Customers> qor = (QueryOperationResponse<northwindClient.Customers>)inlineQuery.Execute();
                        Assert.AreEqual(i, 0);

                        // client count:
                        Assert.AreEqual(qor.Count(), 0);

                        // server count should fail:
                        long count = qor.TotalCount;

                        Assert.Fail("Server count failed to throw on 404");
                    }
                    catch (DataServiceQueryException ex)
                    {
                        Assert.AreEqual(i, 1);
                        Assert.IsNotNull(ex.InnerException);
                        Assert.IsTrue(ex.InnerException.Message.Contains(resourceNotFoundCustomerMsg));
                    }
                    catch (InvalidOperationException ex)
                    {
                        Assert.AreEqual(i, 0);
                        Assert.AreEqual(countNotPresentMsg, ex.Message);
                    }

                    // case 2: $count=value, 404 exception 
                    try
                    {
                        long count = baseQuery.LongCount();
                        Assert.Fail("Server count failed to throw on 404");
                    }
                    catch (DataServiceQueryException ex)
                    {
                        Assert.IsNotNull(ex.InnerException);
                        Assert.IsTrue(ex.InnerException.Message.Contains(resourceNotFoundCustomerMsg));
                    }

                    // case 3: custom URI on context
                    try
                    {
                        QueryOperationResponse<northwindClient.Customers> qor = (QueryOperationResponse<northwindClient.Customers>)ctx.Execute<northwindClient.Customers>("/VAR1?$count=true");
                        Assert.AreEqual(i, 0);

                        // client count:
                        Assert.AreEqual(qor.Count(), 0);

                        // server count should fail:
                        long count = qor.TotalCount;

                        Assert.Fail("Server count failed to throw on 404");
                    }
                    catch (DataServiceQueryException ex)
                    {
                        Assert.AreEqual(i, 1);
                        Assert.IsNotNull(ex.InnerException);
                        Assert.IsTrue(ex.InnerException.Message.Contains(resourceNotFoundVar1Msg));
                    }
                    catch (InvalidOperationException ex)
                    {
                        Assert.AreEqual(i, 0);
                        Assert.AreEqual(countNotPresentMsg, ex.Message);
                    }


                    // case 4: $count=inline, ASYNC, 404 exception gives empty set
                    bool asyncComplete = false;
                    inlineQuery.BeginExecute(ar =>
                    {
                        try
                        {
                            QueryOperationResponse<northwindClient.Customers> qor = (QueryOperationResponse<northwindClient.Customers>)inlineQuery.EndExecute(ar);
                            Assert.AreEqual(qor.Count(), 0);
                            long count = qor.TotalCount;
                            Assert.Fail("Server count failed to throw on 404");
                        }
                        catch (DataServiceQueryException ex)
                        {
                            Assert.AreEqual(i, 1);
                            Assert.IsNotNull(ex.InnerException);
                            Assert.IsNotNull(ex.InnerException.InnerException);
                            Assert.IsTrue(ex.InnerException.InnerException.Message.Contains(resourceNotFoundCustomerMsg));
                        }
                        catch (InvalidOperationException ex)
                        {
                            Assert.AreEqual(i, 0);
                            Assert.AreEqual(countNotPresentMsg, ex.Message);
                        }
                        finally
                        {
                            asyncComplete = true;
                        }
                    }, null);

                    while (!asyncComplete)
                    {
                        Thread.Sleep(10);
                    }
                }
            }

            [TestMethod]
            public void QueryRowCountCustomRequest()
            {
                // .Execute() with direct URI should never sent version 
                ctx.SendingRequest2 += new EventHandler<SendingRequest2EventArgs>((o, e) =>
                {
                    string dsv = e.RequestMessage.GetHeader("OData-Version");
                    Assert.IsNull(dsv);
                });

                IEnumerable[] responses = new IEnumerable[]
                    {
                        ctx.Execute<northwindClient.Customers>("/Customers?$top=1"),
                        ctx.Execute<northwindClient.Customers>("/Customers?$count=true"),
                        ctx.Execute<northwindClient.Customers>("/Customers?$count=true&$filter=ContactTitle eq 'owner'&$top=1"),
                        ctx.Execute<northwindClient.Customers>("/Customers?$orderby=ContactTitle&$count=true"),
                        ctx.Execute<northwindClient.Customers>("/Customers?$expand=Orders&$count=true")
                    };

                for (int i = 0; i < responses.Length; ++i)
                {
                    QueryOperationResponse<northwindClient.Customers> qor = (QueryOperationResponse<northwindClient.Customers>)responses[i];
                    try
                    {
                        long count = qor.TotalCount;
                        Assert.AreNotEqual(i, 0);
                        if (i == 2)
                        {
                            Assert.AreEqual(4, count);
                        }
                        else
                        {
                            Assert.AreEqual(15, count);
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        Assert.AreEqual(i, 0);
                        string expectedMsg = DataServicesClientResourceUtil.GetString("MaterializeFromAtom_CountNotPresent");
                        Assert.AreEqual(expectedMsg, ex.Message);
                    }
                }
            }

            [TestMethod]
            public void QueryRowCountWithOptions()
            {
                IQueryable[] queries = new IQueryable[] {

                        // case 0: no count - should throw at get_CountValue
                        (from c in ctx.CreateQuery<northwindClient.Customers>("Customers") select c).Take(1),

                        // case 1: has count - should equal to 15
                        (from c in ctx.CreateQuery<northwindClient.Customers>("Customers").IncludeTotalCount() select c).Take(1),

                        // case 2: has count with expand - should equal to 15
                        (from c in ctx.CreateQuery<northwindClient.Customers>("Customers").Expand("Orders").IncludeTotalCount() select c).Take(1),
                        
                        // case 3: has count with ordering, skipping - should equal to 15
                        (from c in ctx.CreateQuery<northwindClient.Customers>("Customers").IncludeTotalCount().OrderBy( cc=>cc.ContactTitle).Skip(2).Take(1) select c),
                        
                        // case 4: has count with filtering - should equal to 4
                        (from c in ctx.CreateQuery<northwindClient.Customers>("Customers").IncludeTotalCount().Where(cc=>cc.ContactTitle.Equals("owner")).Take(1) select c),
                       
                        // case 5: has count with filtering & expanding - should equal to 4
                        (from c in ctx.CreateQuery<northwindClient.Customers>("Customers").IncludeTotalCount().Expand("Orders").Where(cc=>cc.ContactTitle.Equals("owner")).Take(1) select c),
                        
                        // case 6: has count from Custom Query Option - should equal to 15
                        (from c in ctx.CreateQuery<northwindClient.Customers>("Customers").AddQueryOption("$count", "true").Take(1) select c)
                    };

                for (int i = 0; i < queries.Length; ++i)
                {
                    IEnumerable ie = ((DataServiceQuery<northwindClient.Customers>)queries[i]).Execute();
                    QueryOperationResponse<northwindClient.Customers> qor = ie as QueryOperationResponse<northwindClient.Customers>;
                    Assert.IsNotNull(qor);

                    try
                    {
                        long count = qor.TotalCount;
                        Assert.AreNotEqual(i, 0);
                        if (i == 4 || i == 5)
                        {
                            Assert.AreEqual(4, count);
                        }
                        else
                        {
                            Assert.AreEqual(15, count);
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        Assert.AreEqual(i, 0);  // only happens in the first case
                        string expectedMsg = DataServicesClientResourceUtil.GetString("MaterializeFromAtom_CountNotPresent");
                        Assert.AreEqual(expectedMsg, ex.Message);
                    }
                }
            }

            #endregion

            #region Server Driven Paging

            private static void PageSizeCustomizer(DataServiceConfiguration config)
            {
                config.SetEntitySetPageSize("*", 2);
                config.SetEntitySetPageSize("Customers", 10);
            }

            [TestMethod]
            public void ServerDrivenPagingBasic()
            {
                using (Utils.ConfigurationCacheCleaner())
                using (Utils.RestoreStaticValueOnDispose(typeof(SimpleDataServiceHelper), "PageSizeCustomizer"))
                {
                    SimpleDataServiceHelper.PageSizeCustomizer = PageSizeCustomizer;
                    SimpleWorkspace workspace = this.NorthwindWorkspacePaged;
                    Uri baseUri = new Uri(workspace.ServiceEndPoint + workspace.ServiceContainer.Name + ".svc", UriKind.Absolute);
                    var querycountpairs = new[] { new { query = "/Customers?$top=20&$skip=1", count = 1, result = "/Customers?$top=10&$skiptoken='BSBEV'", idcount = 10 },
                                                  new { query = "/Customers", count = 1, result = "/Customers?$skiptoken='BOTTM'", idcount = 10 },
                                                };

                    foreach (var querycount in querycountpairs)
                    {
                        string uri = querycount.query;
                        using (Stream response = Utils.ProcessWebRequest(new Uri(baseUri + uri, UriKind.Absolute)))
                        {
                            XmlDocument document = new XmlDocument(Utils.TestNameTable);
                            document.Load(response);

                            string xpath = "/atom:feed/atom:link[@rel='next']";

                            XmlNodeList list = document.SelectNodes(xpath, Utils.TestNamespaceManager);
                            Assert.AreEqual(querycount.count, list.Count, "Expected number of next elements did not match the actual number of next elements");
                            // Verify that the next link is correct
                            Assert.AreEqual(baseUri.ToString() + querycount.result, list[0].Attributes["href"].Value);

                            // Verify count
                            list = document.SelectNodes("/atom:feed/atom:entry", Utils.TestNamespaceManager);
                            Assert.AreEqual(list.Count, querycount.idcount);
                        }
                    }
                }
            }

            [TestMethod]
            public void ServerDrivenPagingExpand()
            {
                using (Utils.ConfigurationCacheCleaner())
                using (Utils.RestoreStaticValueOnDispose(typeof(SimpleDataServiceHelper), "PageSizeCustomizer"))
                {
                    SimpleDataServiceHelper.PageSizeCustomizer = PageSizeCustomizer;
                    SimpleWorkspace workspace = this.NorthwindWorkspacePaged;
                    Uri baseUri = new Uri(workspace.ServiceEndPoint + workspace.ServiceContainer.Name + ".svc", UriKind.Absolute);

                    var querycountpairs = new[] { new { query = "/Employees?$expand=Orders($expand=Customers),Territories&$orderby=EmployeeID add 1", count = 1 },
                                             };
                    int i = 0;
                    foreach (var querycount in querycountpairs)
                    {
                        string uri = querycount.query;
                        using (Stream response = Utils.ProcessWebRequest(new Uri(baseUri + uri, UriKind.Absolute)))
                        {
                            XmlDocument document = new XmlDocument(Utils.TestNameTable);
                            document.Load(response);
                            String xpath = "descendant::atom:link[@rel='next']";
                            XmlNodeList list = document.SelectNodes(xpath, Utils.TestNamespaceManager);
                            Assert.AreEqual(querycount.count, list.Count, "Expected number of next elements did not match the actual number of next elements");
                            i++;
                        }
                    }
                }
            }
            #endregion

            #region Server Driven Paging Client

            private static void PageSizeCustomizerFast(DataServiceConfiguration config)
            {
                config.SetEntitySetPageSize("*", 5);
                config.SetEntitySetPageSize("Customers", 30);
            }

            [TestMethod]
            public void SDPC_DSCFullLoad()
            {
                using (Utils.ConfigurationCacheCleaner())
                using (Utils.RestoreStaticValueOnDispose(typeof(SimpleDataServiceHelper), "PageSizeCustomizer"))
                {
                    SimpleDataServiceHelper.PageSizeCustomizer = PageSizeCustomizerFast;
                    SimpleWorkspace workspace = this.NorthwindWorkspacePaged;
                    Uri baseUri = new Uri(workspace.ServiceEndPoint + workspace.ServiceContainer.Name + ".svc", UriKind.Absolute);

                    DataServiceContext ctx = new DataServiceContext(baseUri);
                    //ctx.EnableAtom = true;
                    //ctx.Format.UseAtom();
                    var q = ctx.CreateQuery<northwindBinding.Customers>("Customers").Expand("Orders");

                    int totalCustomerCount = q.Count();
                    int totalOrdersCount = ctx.CreateQuery<northwindBinding.Orders>("Orders").Count();
                    var custs = new DataServiceCollection<northwindBinding.Customers>(q, TrackingMode.None);

                    for (int i = 0; i < 2; i++)
                    {
                        if (i == 1)
                        {
                            // second iteration
                            ctx = new DataServiceContext(baseUri);
                            //ctx.EnableAtom = true;
                            //ctx.Format.UseAtom();
                            q = ctx.CreateQuery<northwindBinding.Customers>("Customers").Expand("Orders");
                            custs = new DataServiceCollection<northwindBinding.Customers>(q);
                        }

                        while (custs.Continuation != null)
                        {
                            custs.Load(ctx.Execute<northwindBinding.Customers>(custs.Continuation));
                        }

                        int linksCount = 0;
                        int ordersCount = 0;
                        foreach (var c in custs)
                        {
                            while (c.Orders.Continuation != null)
                            {
                                if (linksCount++ % 2 == 0)
                                {
                                    ctx.LoadProperty(c, "Orders", c.Orders.Continuation);
                                }
                                else
                                {
                                    c.Orders.Load(ctx.Execute<northwindBinding.Orders>(c.Orders.Continuation));
                                }
                            }
                            ordersCount += c.Orders.Count;
                        }

                        Assert.IsTrue(linksCount > 0, "links Count must be greater than 0");
                        Assert.AreEqual(totalCustomerCount, custs.Count);
                        Assert.AreEqual(totalOrdersCount, ordersCount);
                    }
                }
            }

            [TestMethod]
            public void SDPC_QORFullLoad()
            {
                using (Utils.ConfigurationCacheCleaner())
                using (Utils.RestoreStaticValueOnDispose(typeof(SimpleDataServiceHelper), "PageSizeCustomizer"))
                {
                    SimpleDataServiceHelper.PageSizeCustomizer = PageSizeCustomizerFast;
                    SimpleWorkspace workspace = this.NorthwindWorkspacePaged;
                    Uri baseUri = new Uri(workspace.ServiceEndPoint + workspace.ServiceContainer.Name + ".svc", UriKind.Absolute);

                    DataServiceContext ctx = new DataServiceContext(baseUri);
                    //ctx.EnableAtom = true;
                    //ctx.Format.UseAtom();
                    var q = ctx.CreateQuery<northwindBinding.Customers>("Customers").Expand("Orders");

                    int totalCustomerCount = q.Count();
                    int totalOrdersCount = ctx.CreateQuery<northwindBinding.Orders>("Orders").Count();

                    var qor = q.Execute() as QueryOperationResponse<northwindBinding.Customers>;

                    DataServiceQueryContinuation<northwindBinding.Customers> nextCustLink = null;
                    int custCount = 0;
                    int orderCount = 0;
                    do
                    {
                        ICollection previousOrderCollection = null;

                        foreach (var c in qor)
                        {
                            try
                            {
                                if (previousOrderCollection != null)
                                {
                                    qor.GetContinuation(previousOrderCollection);
                                    Assert.Fail("Out of scope collection did not throw");
                                }
                            }
                            catch (ArgumentException)
                            {
                            }

                            var nextOrderLink = qor.GetContinuation(c.Orders);
                            while (nextOrderLink != null)
                            {
                                if (custCount % 2 == 0)
                                {
                                    var innerQOR = ctx.Execute<northwindBinding.Orders>(nextOrderLink) as QueryOperationResponse<northwindBinding.Orders>;
                                    foreach (var innerOrder in innerQOR)
                                    {
                                        ctx.AttachLink(c, "Orders", innerOrder);
                                        c.Orders.Add(innerOrder);
                                    }
                                    nextOrderLink = innerQOR.GetContinuation();
                                }
                                else
                                {
                                    nextOrderLink = ctx.LoadProperty(c, "Orders", nextOrderLink).GetContinuation();
                                }
                            }

                            previousOrderCollection = c.Orders;

                            orderCount += c.Orders.Count;
                            custCount++;
                        }

                        nextCustLink = qor.GetContinuation();
                        if (nextCustLink != null)
                        {
                            qor = ctx.Execute<northwindBinding.Customers>(nextCustLink) as QueryOperationResponse<northwindBinding.Customers>;
                        }

                    } while (nextCustLink != null);

                    Assert.AreEqual(totalCustomerCount, custCount);
                    Assert.AreEqual(totalOrdersCount, orderCount);
                    Assert.AreEqual(totalOrdersCount, ctx.Links.Count);
                    Assert.AreEqual(totalCustomerCount + totalOrdersCount, ctx.Entities.Count);
                }
            }

            #endregion
        }
    }
}
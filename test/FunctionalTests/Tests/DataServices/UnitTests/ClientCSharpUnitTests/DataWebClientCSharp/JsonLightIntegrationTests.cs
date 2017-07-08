//---------------------------------------------------------------------
// <copyright file="JsonLightIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Microsoft.OData.Service;
    using Microsoft.OData.Client;
    using System.Data.Test.Astoria;
    using System.Linq;
    using System.Linq.Expressions;
    using AstoriaUnitTests.DataWebClientCSharp.Services;
    using AstoriaUnitTests.Stubs;
    using AstoriaUnitTests.Stubs.DataServiceProvider;
    using Microsoft.OData;
    using Microsoft.OData.UriParser;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    #endregion Namespaces

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/881
    [Ignore] // Remove Atom
    // [TestClass]
    public class JsonLightIntegrationTests
    {
        [TestMethod]
        public void SimpleQuery()
        {
            RunClientIntegrationTestWithBothTrackingAndNoTracking(ctx =>
            {
                var query = ctx.CreateQuery<Product>("Products");
                var product = query.First();

                // Basic sanity check to be sure product was materialized with data values and links
                Assert.IsFalse(String.IsNullOrEmpty(product.ProductName), "Expected ProductName to have a non-null or empty materialized value.");

                // Verify descriptor links for tracked entity
                var entityDesc = ctx.GetEntityDescriptor(product);
                if (ctx.MergeOption != MergeOption.NoTracking)
                {
                    string expectedIdLink = string.Format("{0}/Products({1})", ctx.BaseUri, product.ID);
                    Assert.AreEqual(expectedIdLink, entityDesc.EditLink.AbsoluteUri);
                    Assert.AreEqual(expectedIdLink, entityDesc.Identity.AbsoluteUri);
                    Assert.AreEqual(1, entityDesc.LinkInfos.Count);
                    var linkInfo = entityDesc.LinkInfos.Single();
                    Assert.AreEqual(expectedIdLink + "/OrderDetails/$ref", linkInfo.AssociationLink.AbsoluteUri);
                    Assert.AreEqual(expectedIdLink + "/OrderDetails", linkInfo.NavigationLink.AbsoluteUri);
                }
                else
                {
                    Assert.IsNull(entityDesc, "Expected null entity descriptor with NoTracking.");
                }
            });
        }

        [TestMethod]
        public void SimpleQueryWithOpenTypes()
        {
            using (TestUtil.MetadataCacheCleaner())
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            using (OpenWebDataServiceHelper.EnableAccess.Restore())
            {
                OpenWebDataServiceHelper.EnableAccess.Value = new List<string> { "AstoriaUnitTests.Stubs.Address" };

                request.DataServiceType = typeof(CustomRowBasedOpenTypesContext);
                request.ForceVerboseErrors = true;
                request.StartService();

                DataServiceContext ctx = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                JsonLightTestUtil.ConfigureContextForJsonLight(ctx);

                var query = ctx.CreateQuery<Customer>("Customers");
                var customer = query.First();

                // Basic sanity check to be sure product was materialized with data values and links
                Assert.IsFalse(String.IsNullOrEmpty(customer.Name), "Expected customer to have a non-null or empty materialized value.");
            }
        }

        [TestMethod]
        public void SimpleInsertAndUpdate()
        {
            RunClientIntegrationTestWithTrackingOnly(ctx =>
            {
                var product = new Product { ID = 12345 };
                ctx.AddObject("Products", product);
                ctx.SaveChanges();
                var entityDesc = ctx.GetEntityDescriptor(product);
                string expectedIdLink = string.Format("{0}/Products(12345)", ctx.BaseUri);
                Assert.AreEqual(expectedIdLink, entityDesc.EditLink.AbsoluteUri);
                Assert.AreEqual(expectedIdLink, entityDesc.Identity.AbsoluteUri);

                product = ctx.CreateQuery<Product>("Products").First();
                ctx.UpdateObject(product);
                ctx.SaveChanges();
            });
        }

        [TestMethod]
        public void QueryWithBothMinimalAndFullMetadataShouldNotCauseDuplicateIdentities()
        {
            // Repro for: EntityDescriptor identity/links key order changes for json DataServiceContext
            using (TestUtil.MetadataCacheCleaner())
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                // using the row-based context because it intentionally puts OrderDetail's key properties in non-alphabetical order in the OM.
                request.DataServiceType = typeof(CustomRowBasedContext);
                request.StartService();

                DataServiceContext ctx = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);

                JsonLightTestUtil.ConfigureContextForJsonLight(ctx);

                ConfigureContextForSendingRequest2Verification(ctx);

                var firstOrderDetail = ctx.CreateQuery<OrderDetail>("OrderDetails").First();
                var firstOrderDetailProjected = ctx.CreateQuery<OrderDetail>("OrderDetails").Select(od => new OrderDetail { Quantity = od.Quantity }).First();

                Assert.IsNotNull(firstOrderDetail);
                Assert.IsNotNull(firstOrderDetailProjected);
                Assert.AreEqual(1, ctx.Entities.Count);
            }
        }

        [TestMethod]
        public void UpdateWithAttachAndPrefer()
        {
            using (TestUtil.RestoreStaticValueOnDispose(typeof(CustomDataContext), "PreserveChanges"))
            {
                CustomDataContext.PreserveChanges = true;
                RunClientIntegrationTestWithTrackingOnly(ctx =>
                {
                    var anonymousCustomer = ctx.CreateQuery<Customer>("Customers").Select(c => new {c.ID, c.GuidValue}).First();
                    var customer = new Customer {ID = anonymousCustomer.ID, Name = "this is the new name"};

                    // resolver is still required for update case because this particular set has multiple types in it.
                    ctx.ResolveName = t => t == typeof(Customer) ? "AstoriaUnitTests.Stubs.Customer" : null;

                    ctx.AttachTo("Customers", customer, "W/\"" + ODataUriUtils.ConvertToUriLiteral(anonymousCustomer.GuidValue, ODataVersion.V4) + "\"");
                    ctx.UpdateObject(customer);
                    ctx.AddAndUpdateResponsePreference = DataServiceResponsePreference.IncludeContent;
                    ctx.SaveChanges();

                    // value should have changed in the response payload
                    Assert.AreNotEqual(customer.GuidValue, Guid.Empty);
                    Assert.AreNotEqual(customer.GuidValue, anonymousCustomer.GuidValue);
                });
            }
        }

        [TestMethod]
        public void UpdateWithComputedETag()
        {
            using (TestUtil.RestoreStaticValueOnDispose(typeof(CustomDataContext), "PreserveChanges"))
            {
                CustomDataContext.PreserveChanges = true;
                RunClientIntegrationTestWithTrackingOnly(ctx =>
                {
                    var query = (DataServiceQuery<Customer>)ctx.CreateQuery<Customer>("Customers").Take(1);

                    var results = (QueryOperationResponse<Customer>)query.Execute();
                    var customer = results.First();
                    
                    Assert.IsFalse(results.Headers.ContainsKey("ETag"), "Response should not have contained an ETag header");

                    var descriptor = ctx.GetEntityDescriptor(customer);
                    Assert.IsNotNull(descriptor);
                    Assert.IsNotNull(descriptor.ETag);

                    customer.Name += "foo";
                    ctx.UpdateObject(customer);
                    ctx.SaveChanges();

                    Assert.AreEqual(EntityStates.Unchanged, descriptor.State);
                });
            }
        }

        [TestMethod]
        public void UpdateLinks()
        {
            using (TestUtil.RestoreStaticValueOnDispose(typeof(CustomDataContext), "PreserveChanges"))
            {
                CustomDataContext.PreserveChanges = true;
                RunClientIntegrationTestWithTrackingOnly(ctx =>
                {
                    var customer = ctx.CreateQuery<Customer>("Customers").Expand("Orders").First();
                    var order = customer.Orders.First();

                    ctx.DeleteLink(customer, "Orders", order);
                    ctx.SaveChanges();

                    ctx.AddLink(customer, "Orders", order);
                    ctx.SaveChanges();

                    ctx.SetLink(order, "Customer", null);
                    ctx.SaveChanges();

                    ctx.SetLink(order, "Customer", customer);
                    ctx.SaveChanges();
                });
            }
        }

        [TestMethod]
        public void BatchedInsertWithLinks()
        {
            using (TestUtil.RestoreStaticValueOnDispose(typeof(CustomDataContext), "PreserveChanges"))
            {
                CustomDataContext.PreserveChanges = true;
                RunClientIntegrationTestWithTrackingOnly(ctx =>
                {
                    var customer = new Customer
                    {
                        ID = 123456,
                    };

                    var newOrder = new Order
                    {
                        ID = 7890,
                    };

                    var existingOrder = ctx.CreateQuery<Order>("Orders").First();

                    ctx.ResolveName = t => t.FullName;

                    ctx.AddObject("Customers", customer);
                    ctx.AddRelatedObject(customer, "Orders", newOrder);
                    ctx.SetLink(newOrder, "Customer", customer);
                    ctx.AddLink(customer, "Orders", existingOrder);

                    ctx.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);
                });
            }
        }

        [TestMethod]
        public void QueryWithExpand()
        {
            RunClientIntegrationTestWithBothTrackingAndNoTracking(ctx =>
            {
                // Verify we can do an expand with a couple levels of nesting
                var query = ((DataServiceQuery<Customer>)(ctx.CreateQuery<Customer>("Customers")
                    .Where(c => c.ID == 1)))
                    .Expand("BestFriend($expand=Orders)");

                var results = query.First();

                // Ensure the expanded entities were materialized
                Assert.IsNotNull(results.BestFriend, "Expected expanded BestFriend to be non-null.");
                Assert.IsNotNull(results.BestFriend.Orders, "Expected expanded BestFriend/Orders to be non-null.");
                Assert.AreEqual(2, results.BestFriend.Orders.Count, "Unexpected number of Orders on the expanded BestFriend.");
            });
        }

        [TestMethod]
        public void ExpandWithProjectionWithoutKey()
        {
            RunClientIntegrationTestWithBothTrackingAndNoTracking(ctx =>
            {
                var query = ((DataServiceQuery<Order>)(ctx.CreateQuery<Order>("Orders")
                    .Where(o => o.Customer.ID == 1)))
                    .Select(o => 
                        new Order()
                        {
                            Customer = new Customer()
                                       {
                                           Name = o.Customer.Name,
                                           Orders = o.Customer.Orders.Select(oChild => new Order()
                                                                                       {
                                                                                           DollarAmount = oChild.DollarAmount
                                                                                       }).ToList()
                                       }
                        });

                var results = query.ToList();

                var firstOrder = results[0];
                var secondOrder = results[1];

                var referencesAreEqualWithinEntry = firstOrder.Customer.Orders.Any(o => Object.ReferenceEquals(o, firstOrder));
                var referencesAreEqualAcrossEntries = Object.ReferenceEquals(firstOrder.Customer, secondOrder.Customer);

                // In Atom, even with NoTracking we do some level of identify resolution during materialization, within the scope of a top-level entry.
                // We can't do that with JSON Light because we don't always have the ID, so we never do it. With tracking the resolution should still be used.
                bool isTracking = ctx.MergeOption != MergeOption.NoTracking;
                Assert.AreEqual(isTracking, referencesAreEqualWithinEntry);
                Assert.AreEqual(isTracking, referencesAreEqualAcrossEntries);
            });
        }

        #region Projection with single property
        [TestMethod]
        public void V1ProjectionWithSingleKeyProperty()
        {
            RunClientIntegrationTestWithBothTrackingAndNoTracking(ctx =>
            {
                var query = ctx.CreateQuery<Order>("Orders").Where(c => c.ID == 100).Select(o => o.ID);
                var projection = query.Single();
                Assert.AreEqual(100, projection);
            });
        }

        [TestMethod]
        public void V1ProjectionWithSingleNonKeyProperty()
        {
            RunClientIntegrationTestWithBothTrackingAndNoTracking(ctx =>
            {
                var query = ctx.CreateQuery<Order>("Orders").Where(c => c.ID == 100).Select(o => o.DollarAmount);
                var projection = query.Single();
                Assert.IsTrue(projection > 0, "Expected a non-zero DollarAmount in projection query results.");
            });
        }

        [TestMethod]
        public void V1ProjectionWithSingleNavigationProperty()
        {
            RunClientIntegrationTestWithBothTrackingAndNoTracking(ctx =>
            {
                var query = ctx.CreateQuery<Order>("Orders").Where(c => c.ID == 100).Select(o => o.Customer);
                var projection = query.Single();
                Assert.IsNotNull(projection.Name, "Expected Customer.Name to have a value in projection query results.");
            });
        }
        #endregion

        #region Projecting into anonymous type
        [TestMethod]
        public void ProjectionIntoAnonymousTypeWithKey()
        {
            RunClientIntegrationTestWithBothTrackingAndNoTracking(ctx =>
            {
                var projection = this.ExecuteQueryWithProjectionOnSingleOrder(ctx, o => new { o.ID, o.CurrencyAmount, o.DollarAmount });
                Assert.AreEqual(100, projection.ID);
            });
        }

        [TestMethod]
        public void ProjectionIntoAnonymousTypeWithoutKey()
        {
            RunClientIntegrationTestWithBothTrackingAndNoTracking(ctx =>
            {
                var projection = this.ExecuteQueryWithProjectionOnSingleOrder(ctx, o => new { o.CurrencyAmount, o.DollarAmount });
                Assert.IsTrue(projection.DollarAmount > 0, "Expected a non-zero DollarAmount in projection query results.");
            });
        }

        [TestMethod]
        public void ProjectionIntoAnonymousTypeWithNestedProjectionAndBothParentAndChildKey()
        {
            RunClientIntegrationTestWithBothTrackingAndNoTracking(ctx =>
            {
                var projection = this.ExecuteQueryWithProjectionOnSingleOrder(ctx, o => new { o.ID, o.CurrencyAmount, o.DollarAmount, CustomerID = o.Customer.ID, o.Customer.Address.City });
                Assert.AreEqual(100, projection.ID);
            });
        }

        [TestMethod]
        public void ProjectionIntoAnonymousTypeWithNestedProjectionAndParentKeyOnly()
        {
            RunClientIntegrationTestWithBothTrackingAndNoTracking(ctx =>
            {
                var projection = this.ExecuteQueryWithProjectionOnSingleOrder(ctx, o => new { o.ID, o.CurrencyAmount, o.DollarAmount, o.Customer.Address.City });
                Assert.IsTrue(projection.DollarAmount > 0, "Expected a non-zero DollarAmount in projection query results.");
            });
        }

        [TestMethod]
        public void ProjectionIntoAnonymousTypeWithNestedProjectionAndChildKeyOnly()
        {
            RunClientIntegrationTestWithBothTrackingAndNoTracking(ctx =>
            {
                var projection = this.ExecuteQueryWithProjectionOnSingleOrder(ctx, o => new { o.CurrencyAmount, o.DollarAmount, o.Customer.ID, o.Customer.Name });
                Assert.IsTrue(projection.DollarAmount > 0, "Expected a non-zero DollarAmount in projection query results.");
            });
        }

        #endregion

        #region Projecting into entity type
        [TestMethod]
        public void ProjectionIntoEntityWithKey()
        {
            RunClientIntegrationTestWithBothTrackingAndNoTracking(ctx =>
            {
                var projection = this.ExecuteQueryWithProjectionOnSingleOrder(ctx, o => new Order() { ID = o.ID, CurrencyAmount = o.CurrencyAmount, DollarAmount = o.DollarAmount });
                Assert.AreEqual(100, projection.ID);
            });
        }

        [TestMethod]
        public void ProjectionIntoEntityWithoutKey()
        {
            RunClientIntegrationTestWithBothTrackingAndNoTracking(ctx =>
            {
                var projection = this.ExecuteQueryWithProjectionOnSingleOrder(ctx, o => new Order() { CurrencyAmount = o.CurrencyAmount, DollarAmount = o.DollarAmount });
                Assert.IsTrue(projection.DollarAmount > 0, "Expected a non-zero DollarAmount in projection query results.");
            });
        }

        [TestMethod]
        public void ProjectionIntoEntityWithNestedProjectionAndBothParentAndChildKey()
        {
            RunClientIntegrationTestWithBothTrackingAndNoTracking(ctx =>
            {
                var projection = this.ExecuteQueryWithProjectionOnSingleOrder(ctx, o => new Order() { ID = o.ID, CurrencyAmount = o.CurrencyAmount, DollarAmount = o.DollarAmount, Customer = new Customer() { ID = o.Customer.ID, Address = o.Customer.Address }});
                Assert.AreEqual(100, projection.ID);
            });
        }

        [TestMethod]
        public void ProjectionIntoEntityWithNestedProjectionAndParentKeyOnly()
        {
            RunClientIntegrationTestWithBothTrackingAndNoTracking(ctx =>
            {
                var projection = this.ExecuteQueryWithProjectionOnSingleOrder(ctx, o => new Order() { ID = o.ID, CurrencyAmount = o.CurrencyAmount, DollarAmount = o.DollarAmount, Customer = new Customer() { Address = o.Customer.Address }});
                Assert.IsTrue(projection.DollarAmount > 0, "Expected a non-zero DollarAmount in projection query results.");
            });
        }

        [TestMethod]
        public void ProjectionIntoEntityWithNestedProjectionAndChildKeyOnly()
        {
            RunClientIntegrationTestWithBothTrackingAndNoTracking(ctx =>
            {
                var projection = this.ExecuteQueryWithProjectionOnSingleOrder(ctx, o => new Order() { CurrencyAmount = o.CurrencyAmount, DollarAmount = o.DollarAmount, Customer = new Customer() { ID = o.Customer.ID, Name = o.Customer.Name }});
                Assert.IsTrue(projection.DollarAmount > 0, "Expected a non-zero DollarAmount in projection query results.");
            });
        }

        #endregion

        #region Projecting into complex type
        [TestMethod]
        public void ProjectionIntoComplexTypeWithKey()
        {
            RunClientIntegrationTestWithBothTrackingAndNoTracking(ctx =>
            {
                var projection = this.ExecuteQueryWithProjectionOnSingleOrder(ctx, o => new ProjectionComplexType() { OrderID = o.ID, CurrencyAmount = o.CurrencyAmount, DollarAmount = o.DollarAmount });
                Assert.AreEqual(100, projection.OrderID);
            });
        }

        [TestMethod]
        public void ProjectionIntoComplexTypeWithoutKey()
        {
            RunClientIntegrationTestWithBothTrackingAndNoTracking(ctx =>
            {
                var projection = this.ExecuteQueryWithProjectionOnSingleOrder(ctx, o => new ProjectionComplexType() { CurrencyAmount = o.CurrencyAmount, DollarAmount = o.DollarAmount });
                Assert.IsTrue(projection.DollarAmount > 0, "Expected a non-zero DollarAmount in projection query results.");
            });
        }

        [TestMethod]
        public void ProjectionIntoComplexTypeWithNestedProjectionAndBothParentAndChildKey()
        {
            RunClientIntegrationTestWithBothTrackingAndNoTracking(ctx =>
            {
                var projection = this.ExecuteQueryWithProjectionOnSingleOrder(ctx, o => new ProjectionComplexType() { OrderID = o.ID, CurrencyAmount = o.CurrencyAmount, DollarAmount = o.DollarAmount, CustomerID = o.Customer.ID, CustomerCity = o.Customer.Address.City });
                Assert.AreEqual(100, projection.OrderID);
            });
        }

        [TestMethod]
        public void ProjectionIntoComplexTypeWithNestedProjectionAndParentKeyOnly()
        {
            RunClientIntegrationTestWithBothTrackingAndNoTracking(ctx =>
            {
                var projection = this.ExecuteQueryWithProjectionOnSingleOrder(ctx, o => new ProjectionComplexType() { OrderID = o.ID, CurrencyAmount = o.CurrencyAmount, DollarAmount = o.DollarAmount, CustomerCity = o.Customer.Address.City });
                Assert.IsTrue(projection.DollarAmount > 0, "Expected a non-zero DollarAmount in projection query results.");
            });
        }

        [TestMethod]
        public void ProjectionIntoComplexTypeWithNestedProjectionAndChildKeyOnly()
        {
            RunClientIntegrationTestWithBothTrackingAndNoTracking(ctx =>
            {
                var projection = this.ExecuteQueryWithProjectionOnSingleOrder(ctx, o => new ProjectionComplexType() { CurrencyAmount = o.CurrencyAmount, DollarAmount = o.DollarAmount, CustomerID = o.Customer.ID, CustomerCity = o.Customer.Address.City });
                Assert.IsTrue(projection.DollarAmount > 0, "Expected a non-zero DollarAmount in projection query results.");
            });
        }

        public class ProjectionComplexType
        {
            public int OrderID { get; set; }
            public CurrencyAmount CurrencyAmount { get; set; }
            public double DollarAmount { get; set; }
            public int CustomerID { get; set; }
            public string CustomerCity { get; set; }
        }
        #endregion

        [TestMethod]
        public void ProjectionWithSelectInFilterString()
        {
            RunClientIntegrationTestWithBothTrackingAndNoTracking(ctx =>
            {
                // We should automatically escape the value in the filter before we inspect it for a projection, so this should not trigger asking for all metadata
                var query = ctx.CreateQuery<Customer>("Customers").Where(c => c.Name == "?$select=");

                ctx.SendingRequest2 += (c, args) => Assert.AreEqual("application/json;odata.metadata=minimal", args.RequestMessage.GetHeader("Accept"));

                // Query will return no results, but execute it so that we get the verification in SendingRequest2
                query.ToList();
            });
        }

        private T ExecuteQueryWithProjectionOnSingleOrder<T>(DataServiceContext ctx, Expression<Func<Order, T>> projection)
        {
            var query = ctx.CreateQuery<Order>("Orders").Where(o => o.ID == 100).Select(projection);
            return query.Single();
        }
        
        [TestMethod]
        public void ClientShouldRequestAllMetadataWithProjectionInExecuteWithUri()
        {
            RunClientIntegrationTestWithBothTrackingAndNoTracking(ctx =>
            {
                ctx.Execute<Order>(new Uri("/Orders(1)?$select=DollarAmount,CurrencyAmount", UriKind.Relative));
            });
        }

        [TestMethod]
        public void ClientShouldRequestAllMetadataWithProjectionInExecuteWithUriAsync()
        {
            RunClientIntegrationTestWithBothTrackingAndNoTracking(ctx =>
            {
                var asyncResult = ctx.BeginExecute<Order>(new Uri("/Orders(1)?$select=DollarAmount,CurrencyAmount", UriKind.Relative), null, null);
                ctx.EndExecute<Order>(asyncResult);
            });
        }

        [TestMethod]
        public void ClientShouldRequestAllMetadataWithProjectionInExecuteWithContinuation()
        {
            RunClientIntegrationTestWithPagingAndBothTrackingAndNoTracking(ctx =>
            {
                var response = ctx.Execute<Order>(new Uri("/Orders()?$select=Customer,DollarAmount", UriKind.Relative)) as QueryOperationResponse<Order>;
                response.ToList();
                var continuation = response.GetContinuation();
                
                ctx.Execute(continuation);
            });
        }

        [TestMethod]
        public void ClientShouldRequestAllMetadataWithProjectionInExecuteWithContinuationAsync()
        {
            RunClientIntegrationTestWithPagingAndBothTrackingAndNoTracking(ctx =>
            {
                IAsyncResult asyncResult = ctx.BeginExecute<Order>(new Uri("/Orders()?$select=Customer,DollarAmount", UriKind.Relative), null, null);
                var response = ctx.EndExecute<Order>(asyncResult) as QueryOperationResponse<Order>;
                response.ToList();
                var continuation = response.GetContinuation();

                asyncResult = ctx.BeginExecute<Order>(continuation, null, null);
                ctx.EndExecute<Order>(asyncResult);
            });
        }

        [TestMethod]
        public void ClientShouldRequestAllMetadataWithProjectionInBatchExecute()
        {
            RunClientIntegrationTestWithBothTrackingAndNoTracking(ctx =>
            {
                var requests = new DataServiceRequest[] 
                {
                    new DataServiceRequest<Order>(new Uri("/Orders(1)?$select=DollarAmount,CurrencyAmount", UriKind.Relative)),
                    new DataServiceRequest<Customer>(new Uri("/Customers(1)?$select=ID,GuidValue", UriKind.Relative)),
                };

                ctx.ExecuteBatch(requests);
            });
        }

        [TestMethod]
        public void ClientShouldRequestAllMetadataWithProjectionInBatchExecuteAsync()
        {
            RunClientIntegrationTestWithBothTrackingAndNoTracking(ctx =>
            {
                var requests = new DataServiceRequest[] 
                {
                    new DataServiceRequest<Order>(new Uri("/Orders(1)?$select=DollarAmount,CurrencyAmount", UriKind.Relative)),
                    new DataServiceRequest<Customer>(new Uri("/Customers(1)?$select=ID,GuidValue", UriKind.Relative)),
                };

                var asyncResult = ctx.BeginExecuteBatch(null, null, requests);
                ctx.EndExecuteBatch(asyncResult);
            });
        }

        [TestMethod]
        public void ClientShouldRequestAllMetadataWithProjectionIntoDataServiceCollection()
        {
            RunClientIntegrationTestWithPagingAndTrackingOnly(ctx =>
            {
                var clientType = typeof(OrderWithBinding);
                var serverType = typeof(Order);

                ctx.ResolveType = name =>
                                  {
                                      Assert.AreEqual(serverType.FullName, name);
                                      return clientType;
                                  };
                ctx.ResolveName = type =>
                                  {
                                      Assert.AreEqual(clientType, type);
                                      return serverType.FullName;
                                  };

                var dsc = new DataServiceCollection<OrderWithBinding>(ctx.CreateQuery<OrderWithBinding>("Orders"));
                Assert.IsNotNull(dsc.Continuation, "Expected first continuation to be non-null since server paging is enabled.");
                while (dsc.Continuation != null)
                {
                    dsc.Load(ctx.Execute<OrderWithBinding>(dsc.Continuation.NextLinkUri));
                }
            });
        }

        public class OrderWithBinding : Order, System.ComponentModel.INotifyPropertyChanged
        {
            public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

            public void FirePropertyChanged()
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs("property"));
            }
        }

        [TestMethod]
        public void LoadPropertyReference()
        {
            RunClientIntegrationTestWithTrackingOnly(ctx =>
            {
                var customer = ctx.CreateQuery<Customer>("Customers").Where(c => c.ID == 1).Single();
                Assert.IsNull(customer.BestFriend, "Customer.BestFriend should be null before calling LoadProperty.");
                ctx.LoadProperty(customer, "BestFriend");
                Assert.IsNotNull(customer.BestFriend, "Customer.BestFriend should not be null after calling LoadProperty.");
            });
        }

        [TestMethod]
        public void LoadPropertyCollection()
        {
            RunClientIntegrationTestWithTrackingOnly(ctx =>
            {
                var customer = ctx.CreateQuery<Customer>("Customers").Where(c => c.ID == 1).Single();
                Assert.AreEqual(0, customer.Orders.Count, "Expected Orders.Count to be 0 before calling LoadProperty.");
                ctx.LoadProperty(customer, "Orders");
                Assert.IsTrue(customer.Orders.Count > 0, "Expected Orders collection to be populated after calling LoadProperty.");
            });
        }

        [TestMethod]
        public void QueryEntityWithNamedStreams()
        {
            RunClientIntegrationTestWithNamedStreamsAndBothTrackingAndNotTracking(ctx =>
            {
                var resultWithStream = GetQueryForEntityWithNamedStreams(ctx).Single();

                var stream1 = resultWithStream.Stream1;
                Assert.IsNotNull(stream1);
                Assert.AreEqual("http://otherserver/stream", stream1.SelfLink.AbsoluteUri);
                Assert.AreEqual(ctx.BaseUri + "/MySet1(1)/Stream1", stream1.EditLink.AbsoluteUri);

                var stream2 = resultWithStream.Stream2;
                Assert.IsNotNull(stream2);
                Assert.AreEqual(ctx.BaseUri + "/MySet1(1)/Stream2", stream2.SelfLink.AbsoluteUri);
                Assert.AreEqual(ctx.BaseUri + "/MySet1(1)/Stream2", stream2.EditLink.AbsoluteUri);
            });
        }

        [TestMethod]
        public void QueryEntityWithNamedStreamsAndProjectionWithKey()
        {
            RunClientIntegrationTestWithNamedStreamsAndBothTrackingAndNotTracking(ctx =>
            {
                var resultWithStream = GetQueryForEntityWithNamedStreams(ctx)
                    .Select(e => new { e.ID, e.Stream1, e.Stream2 }).Single();

                var stream1 = resultWithStream.Stream1;
                Assert.IsNotNull(stream1);
                Assert.AreEqual("http://otherserver/stream", stream1.SelfLink.AbsoluteUri);
                Assert.AreEqual(ctx.BaseUri + "/MySet1(1)/Stream1", stream1.EditLink.AbsoluteUri);

                var stream2 = resultWithStream.Stream2;
                Assert.IsNotNull(stream2);
                Assert.AreEqual(ctx.BaseUri + "/MySet1(1)/Stream2", stream2.SelfLink.AbsoluteUri);
                Assert.AreEqual(ctx.BaseUri + "/MySet1(1)/Stream2", stream2.EditLink.AbsoluteUri);
            });
        }

        [TestMethod]
        public void QueryEntityWithNamedStreamsAndProjectionWithoutKey()
        {
            RunClientIntegrationTestWithNamedStreamsAndBothTrackingAndNotTracking(ctx =>
            {
                var resultWithStream = GetQueryForEntityWithNamedStreams(ctx)
                    .Select(e => new { e.Name, e.Stream1, e.Stream2 }).Single();

                var stream1 = resultWithStream.Stream1;
                Assert.IsNotNull(stream1);
                Assert.AreEqual("http://otherserver/stream", stream1.SelfLink.AbsoluteUri);
                Assert.AreEqual(ctx.BaseUri + "/MySet1(1)/Stream1", stream1.EditLink.AbsoluteUri);

                var stream2 = resultWithStream.Stream2;
                Assert.IsNotNull(stream2);
                Assert.AreEqual(ctx.BaseUri + "/MySet1(1)/Stream2", stream2.SelfLink.AbsoluteUri);
                Assert.AreEqual(ctx.BaseUri + "/MySet1(1)/Stream2", stream2.EditLink.AbsoluteUri);
            });
        }

        [TestMethod]
        public void QueryEntityWithNamedStreamsAndExpand()
        {
            RunClientIntegrationTestWithNamedStreamsAndBothTrackingAndNotTracking(ctx =>
            {
                var resultWithStream = GetQueryForEntityWithNamedStreams(ctx)
                    .Expand("Collection($expand=Collection1)").Single();

                var stream1 = resultWithStream.Collection.First().ColStream;
                Assert.IsNotNull(stream1);
                Assert.AreEqual("http://otherserver/stream", stream1.SelfLink.AbsoluteUri);
                Assert.AreEqual(ctx.BaseUri + "/MySet3('ABCDE')/ColStream", stream1.EditLink.AbsoluteUri);

                stream1 = resultWithStream.Collection.First().Collection1.First().RefStream1;
                Assert.IsNotNull(stream1);
                Assert.AreEqual("http://otherserver/stream", stream1.SelfLink.AbsoluteUri);
                Assert.AreEqual(ctx.BaseUri + "/MySet2(3)/RefStream1", stream1.EditLink.AbsoluteUri);
            });
        }

        [TestMethod]
        public void QueryEntityWithNamedStreamsAndProjectionWithExpandAndKeys()
        {
            RunClientIntegrationTestWithNamedStreamsAndBothTrackingAndNotTracking(ctx =>
            {
                var resultWithStream = GetQueryForEntityWithNamedStreams(ctx)
                    .Select(e => new EntityWithNamedStreams1()
                    {
                        ID = e.ID,
                        Name = e.Name,
                        Stream1 = e.Stream1,
                        Stream2 = e.Stream2,
                        Ref = new EntityWithNamedStreams1()
                        {
                            ID = e.Ref.ID,
                            Name = e.Ref.Name,
                            RefStream1 = e.Ref.RefStream1,
                        },
                        Collection = e.Collection.Select(eColl => new EntityWithNamedStreams2()
                                                                  {
                                                                      ID = eColl.ID,
                                                                      Name = eColl.Name,
                                                                      ColStream = eColl.ColStream
                                                                  }).ToList()
                    }).Single();

                var stream = resultWithStream.Stream1;
                Assert.IsNotNull(stream);
                Assert.AreEqual("http://otherserver/stream", stream.SelfLink.AbsoluteUri);
                Assert.AreEqual(ctx.BaseUri + "/MySet1(1)/Stream1", stream.EditLink.AbsoluteUri);

                stream = resultWithStream.Stream2;
                Assert.IsNotNull(stream);
                Assert.AreEqual(ctx.BaseUri + "/MySet1(1)/Stream2", stream.SelfLink.AbsoluteUri);
                Assert.AreEqual(ctx.BaseUri + "/MySet1(1)/Stream2", stream.EditLink.AbsoluteUri);

                stream = resultWithStream.Ref.RefStream1;
                Assert.IsNotNull(stream);
                Assert.AreEqual("http://otherserver/stream", stream.SelfLink.AbsoluteUri);
                Assert.AreEqual(ctx.BaseUri + "/MySet2(3)/RefStream1", stream.EditLink.AbsoluteUri);

                stream = resultWithStream.Collection.First().ColStream;
                Assert.IsNotNull(stream);
                Assert.AreEqual("http://otherserver/stream", stream.SelfLink.AbsoluteUri);
                Assert.AreEqual(ctx.BaseUri + "/MySet3('ABCDE')/ColStream", stream.EditLink.AbsoluteUri);
            });
        }

        [TestMethod]
        public void QueryEntityWithNamedStreamsAndProjectionWithExpandAndNoKeys()
        {
            RunClientIntegrationTestWithNamedStreamsAndBothTrackingAndNotTracking(ctx =>
            {
                var resultWithStream = GetQueryForEntityWithNamedStreams(ctx)
                    .Select(e => new EntityWithNamedStreams1()
                                 {
                                     Name = e.Name,
                                     Stream1 = e.Stream1,
                                     Stream2 = e.Stream2,
                                     Ref = new EntityWithNamedStreams1()
                                           {
                                               Name = e.Ref.Name,
                                               RefStream1 = e.Ref.RefStream1,
                                           },
                                     Collection = e.Collection.Select(eColl => new EntityWithNamedStreams2()
                                     {
                                         Name = eColl.Name,
                                         ColStream = eColl.ColStream
                                     }).ToList()
                                 }).Single();

                var stream = resultWithStream.Stream1;
                Assert.IsNotNull(stream);
                Assert.AreEqual("http://otherserver/stream", stream.SelfLink.AbsoluteUri);
                Assert.AreEqual(ctx.BaseUri + "/MySet1(1)/Stream1", stream.EditLink.AbsoluteUri);

                stream = resultWithStream.Stream2;
                Assert.IsNotNull(stream);
                Assert.AreEqual(ctx.BaseUri + "/MySet1(1)/Stream2", stream.SelfLink.AbsoluteUri);
                Assert.AreEqual(ctx.BaseUri + "/MySet1(1)/Stream2", stream.EditLink.AbsoluteUri);

                stream = resultWithStream.Ref.RefStream1;
                Assert.IsNotNull(stream);
                Assert.AreEqual("http://otherserver/stream", stream.SelfLink.AbsoluteUri);
                Assert.AreEqual(ctx.BaseUri + "/MySet2(3)/RefStream1", stream.EditLink.AbsoluteUri);

                stream = resultWithStream.Collection.First().ColStream;
                Assert.IsNotNull(stream);
                Assert.AreEqual("http://otherserver/stream", stream.SelfLink.AbsoluteUri);
                Assert.AreEqual(ctx.BaseUri + "/MySet3('ABCDE')/ColStream", stream.EditLink.AbsoluteUri);
            });
        }

        private DataServiceQuery<EntityWithNamedStreams1> GetQueryForEntityWithNamedStreams(DataServiceContext context)
        {
            return (DataServiceQuery<EntityWithNamedStreams1>)context.CreateQuery<EntityWithNamedStreams1>("MySet1").Where(e => e.ID == 1);
        }

        private static void RunClientIntegrationTestWithNamedStreamsAndBothTrackingAndNotTracking(Action<DataServiceContext> executeAndVerify)
        {
            RunClientIntegrationTestWithNamedStreams(executeAndVerify, MergeOption.AppendOnly);
            RunClientIntegrationTestWithNamedStreams(executeAndVerify, MergeOption.NoTracking);
        }

        private static void RunClientIntegrationTestWithNamedStreams(Action<DataServiceContext> executeAndVerify, MergeOption mergeOption)
        {
            DSPServiceDefinition service = NamedStreamService.SetUpNamedStreamService();

            // Configure one of the named streams on each type to have a different ReadStreamUri than the default, so we can verify it's correctly picked up and not built using conventions
            ConfigureReadStreamUri(service, "MySet1", "EntityWithNamedStreams", "Stream1");
            ConfigureReadStreamUri(service, "MySet2", "EntityWithNamedStreams1", "RefStream1");
            ConfigureReadStreamUri(service, "MySet3", "EntityWithNamedStreams2", "ColStream");
            
            using (TestWebRequest request = service.CreateForInProcessWcf())
            {
                request.StartService();

                DataServiceContext ctx = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                ctx.MergeOption = mergeOption;

                JsonLightTestUtil.ConfigureContextForJsonLight(ctx);

                ConfigureContextForSendingRequest2Verification(ctx);

                executeAndVerify(ctx);
            }
        }

        private static void ConfigureContextForSendingRequest2Verification(DataServiceContext context)
        {
            context.SendingRequest2 += VerifyJsonLightRequestHeaders;
        }

        private static void ConfigureReadStreamUri(DSPServiceDefinition service, string resourceSetName, string resourceTypeName, string namedStreamPropertyName)
        {
            var entityWithStream = service.DataSource.GetResourceSetEntities(resourceSetName).First();
            DSPMediaResource streamResource;
            var streamResourceProperty = service.Metadata.GetResourceType(resourceTypeName).GetNamedStreams().Single(ns => ns.Name == namedStreamPropertyName);
            service.MediaResourceStorage.TryGetMediaResource(entityWithStream, streamResourceProperty, out streamResource);
            streamResource.ReadStreamUri = new Uri("http://otherserver/stream");    
        }

        private static void RunClientIntegrationTestWithPagingAndBothTrackingAndNoTracking(Action<DataServiceContext> executeAndVerify)
        {
            Action<DataServiceConfiguration, Type> pageSizeCustomizer = (config, type) => config.SetEntitySetPageSize("*", 2);
            RunClientIntegrationTestWithBothTrackingAndNoTracking(executeAndVerify, pageSizeCustomizer);
        }

        private static void RunClientIntegrationTestWithPagingAndTrackingOnly(Action<DataServiceContext> executeAndVerify)
        {
            Action<DataServiceConfiguration, Type> pageSizeCustomizer = (config, type) => config.SetEntitySetPageSize("*", 2);
            RunClientIntegrationTestWithTrackingOnly(executeAndVerify, pageSizeCustomizer);
        }

        private static void RunClientIntegrationTestWithBothTrackingAndNoTracking(Action<DataServiceContext> executeAndVerify, Action<DataServiceConfiguration, Type> pageSizeCustomizer = null)
        {
            RunClientIntegrationTestWithMergeOption(executeAndVerify, MergeOption.AppendOnly, pageSizeCustomizer);
            RunClientIntegrationTestWithMergeOption(executeAndVerify, MergeOption.NoTracking, pageSizeCustomizer);
        }

        private static void RunClientIntegrationTestWithTrackingOnly(Action<DataServiceContext> executeAndVerify, Action<DataServiceConfiguration, Type> pageSizeCustomizer = null)
        {
            RunClientIntegrationTestWithMergeOption(executeAndVerify, MergeOption.AppendOnly, pageSizeCustomizer);
        }

        private static void RunClientIntegrationTestWithMergeOption(Action<DataServiceContext> executeAndVerify, MergeOption mergeOption, Action<DataServiceConfiguration, Type> pageSizeCustomizer)
        {
            using (TestUtil.MetadataCacheCleaner())
            using (OpenWebDataServiceHelper.PageSizeCustomizer.Restore())
            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            {
                OpenWebDataServiceHelper.PageSizeCustomizer.Value = pageSizeCustomizer;

                request.DataServiceType = typeof(CustomDataContext);
                request.ForceVerboseErrors = true;
                request.StartService();

                DataServiceContext ctx = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                ctx.MergeOption = mergeOption;

                JsonLightTestUtil.ConfigureContextForJsonLight(ctx);

                ConfigureContextForSendingRequest2Verification(ctx);

                executeAndVerify(ctx);
            }
        }

        private static void VerifyJsonLightRequestHeaders(object sender, SendingRequest2EventArgs args)
        {
            var requestMessage = args.RequestMessage;
            string accept = requestMessage.GetHeader("Accept");
            bool isBatch = requestMessage.Url.OriginalString.Contains("$batch");
            if (isBatch)
            {
                Assert.AreEqual("multipart/mixed", accept);
            }
            else
            {
                if (args.RequestMessage.Url.OriginalString.Contains("$select"))
                {
                    Assert.AreEqual("application/json;odata.metadata=full", accept);
                }
                else
                {
                    Assert.AreEqual("application/json;odata.metadata=minimal", accept);    
                }
            }

            if (requestMessage.Method != "GET" && requestMessage.Method != "DELETE" && !isBatch)
            {
                string contentType = requestMessage.GetHeader("Content-Type");
                Assert.AreEqual("application/json;odata.metadata=minimal", contentType);
            }
        }
    }
}

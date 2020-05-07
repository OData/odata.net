//---------------------------------------------------------------------
// <copyright file="ClientEntityDescripterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using Microsoft.OData.Client;
using Microsoft.OData;
using Microsoft.Test.OData.Services.TestServices;
using Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Test.OData.Tests.Client.ClientTests
{
    [TestClass]
    public class ClientEntityDescripterTests : ODataWCFServiceTestsBase<InMemoryEntities>
    {
        public ClientEntityDescripterTests()
            : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {

        }

        private Dictionary<string, List<string>> entitySetsList = new Dictionary<string, List<string>>
        {
            {"People", new List<string>{"PersonID"}},
            {"Orders", new List<string>{"OrderID"}},
            {"ProductDetails", new List<string>{"ProductID", "ProductDetailID"}}
        };

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        // NetCore: IQueryable executes a synchronous query and this is currently not implemented in the portable lib version
        // of OData Client. This test throws a System.NotSupportedException. See Microsoft.OData.Client/DataServiceQueryOfT.cs
        [TestMethod]
        public void TestRootQuery()
        {
            foreach (var entitySetKeys in entitySetsList.Keys)
            {
                IQueryable iqueryableProperty = typeof(InMemoryEntities).GetProperty(entitySetKeys).GetValue(TestClientContext, null) as IQueryable;
                foreach (var entity in iqueryableProperty)
                {
                    EntityDescriptor eDescriptor = TestClientContext.GetEntityDescriptor(entity);
                    Assert.IsNotNull(eDescriptor.SelfLink, "Self link was not read");
                    Assert.IsNotNull(eDescriptor.EditLink, "Edit link was not read");
                    Assert.IsNotNull(eDescriptor.Identity, "Identity was not read");
                }
            }
        }

        // NetCore: IQueryable executes a synchronous query and this is currently not implemented in the portable lib version
        // of OData Client. This test throws a System.NotSupportedException. See Microsoft.OData.Client/DataServiceQueryOfT.cs
        [TestMethod]
        public void TestTopOption()
        {
            foreach (var entitySetKeys in entitySetsList.Keys)
            {
                IQueryable iqueryableProperty = typeof(InMemoryEntities).GetProperty(entitySetKeys).GetValue(TestClientContext, null) as IQueryable;
                Console.WriteLine("running top query against root set '{0}'", entitySetKeys);
                var takeOneQuery = this.CreateTopQuery(iqueryableProperty, 1);
                Console.WriteLine(takeOneQuery.ToString());
                foreach (var entity in takeOneQuery)
                {
                    EntityDescriptor eDescriptor = TestClientContext.GetEntityDescriptor(entity);
                    Assert.IsNotNull(eDescriptor.SelfLink, "Self link was not read");
                    Assert.IsNotNull(eDescriptor.EditLink, "Edit link was not read");
                    Assert.IsNotNull(eDescriptor.Identity, "Identity was not read");
                }
            }
        }

        // NetCore: IQueryable executes a synchronous query and this is currently not implemented in the portable lib version
        // of OData Client. This test throws a System.NotSupportedException. See Microsoft.OData.Client/DataServiceQueryOfT.cs
        [TestMethod]
        public void TestSkipOption()
        {
            foreach (var entitySetKeys in entitySetsList.Keys)
            {
                IQueryable iqueryableProperty = typeof(InMemoryEntities).GetProperty(entitySetKeys).GetValue(TestClientContext, null) as IQueryable;
                Console.WriteLine("running top query against root set '{0}'", entitySetKeys);
                var takeOneQuery = this.CreateSkipQuery(iqueryableProperty, 1);
                Console.WriteLine(takeOneQuery.ToString());
                foreach (var entity in takeOneQuery)
                {
                    EntityDescriptor eDescriptor = TestClientContext.GetEntityDescriptor(entity);
                    Assert.IsNotNull(eDescriptor.SelfLink, "Self link was not read");
                    Assert.IsNotNull(eDescriptor.EditLink, "Edit link was not read");
                    Assert.IsNotNull(eDescriptor.Identity, "Identity was not read");
                }
            }
        }

        // NetCore: IQueryable executes a synchronous query and this is currently not implemented in the portable lib version
        // of OData Client. This test throws a System.NotSupportedException. See Microsoft.OData.Client/DataServiceQueryOfT.cs
        [TestMethod]
        public void TestOrderByOption()
        {
            foreach (var entitySet in entitySetsList)
            {
                foreach (var keyName in entitySet.Value)
                {
                    IQueryable iqueryableProperty = typeof(InMemoryEntities).GetProperty(entitySet.Key).GetValue(TestClientContext, null) as IQueryable;
                    Uri executeUri = new Uri(String.Format("{0}?$orderby={1}", iqueryableProperty.ToString(), keyName));
                    Type clientType = iqueryableProperty.ElementType;
                    var orderedResults = this.ExecuteUri(TestClientContext, executeUri, clientType);
                    foreach (var entity in orderedResults)
                    {
                        EntityDescriptor eDescriptor = TestClientContext.GetEntityDescriptor(entity);
                        Assert.IsNotNull(eDescriptor.SelfLink, "Self link was not read");
                        Assert.IsNotNull(eDescriptor.EditLink, "Edit link was not read");
                        Assert.IsNotNull(eDescriptor.Identity, "Identity was not read");
                    }
                }
            }
        }

        // NetCore: IQueryable executes a synchronous query and this is currently not implemented in the portable lib version
        // of OData Client. This test throws a System.NotSupportedException. See Microsoft.OData.Client/DataServiceQueryOfT.cs
        [TestMethod]
        public void TestOrderByThenByOption()
        {
            foreach (var entitySet in entitySetsList)
            {
                IQueryable iqueryableProperty = typeof(InMemoryEntities).GetProperty(entitySet.Key).GetValue(TestClientContext, null) as IQueryable;
                Uri executeUri = new Uri(String.Format("{0}?$orderby={1}", iqueryableProperty.ToString(), string.Join(",", entitySet.Value)));
                Type clientType = iqueryableProperty.ElementType;
                var orderedResults = this.ExecuteUri(TestClientContext, executeUri, clientType);
                foreach (var entity in orderedResults)
                {
                    EntityDescriptor eDescriptor = TestClientContext.GetEntityDescriptor(entity);
                    Assert.IsNotNull(eDescriptor.SelfLink, "Self link was not read");
                    Assert.IsNotNull(eDescriptor.EditLink, "Edit link was not read");
                    Assert.IsNotNull(eDescriptor.Identity, "Identity was not read");
                }
            }
        }

        // NetCore: IQueryable executes a synchronous query and this is currently not implemented in the portable lib version
        // of OData Client. This test throws a System.NotSupportedException. See Microsoft.OData.Client/DataServiceQueryOfT.cs
        [TestMethod]
        public void TestOrderByDescendingOption()
        {
            foreach (var entitySet in entitySetsList)
            {
                foreach (var keyName in entitySet.Value)
                {

                    IQueryable iqueryableProperty = typeof(InMemoryEntities).GetProperty(entitySet.Key).GetValue(TestClientContext, null) as IQueryable;
                    Uri executeUri = new Uri(String.Format("{0}?$orderby={1} desc", iqueryableProperty.ToString(), keyName));
                    Type clientType = iqueryableProperty.ElementType;
                    var orderedResults = this.ExecuteUri(TestClientContext, executeUri, clientType);
                    foreach (var entity in orderedResults)
                    {
                        EntityDescriptor eDescriptor = TestClientContext.GetEntityDescriptor(entity);
                        Assert.IsNotNull(eDescriptor.SelfLink, "Self link was not read");
                        Assert.IsNotNull(eDescriptor.EditLink, "Edit link was not read");
                        Assert.IsNotNull(eDescriptor.Identity, "Identity was not read");
                    }
                }
            }
        }

        [TestMethod]
        public void GetDiscontinuedProducts()
        {
            var discontinuedProducts = from product in TestClientContext.Products
                                       where product.Discontinued
                                       select product;

            Console.WriteLine(discontinuedProducts.ToString());
            foreach (var entity in discontinuedProducts)
            {
                EntityDescriptor eDescriptor = TestClientContext.GetEntityDescriptor(entity);
                Assert.IsNotNull(eDescriptor.SelfLink, "Self link was not read");
                Assert.IsNotNull(eDescriptor.EditLink, "Edit link was not read");
                Assert.IsNotNull(eDescriptor.Identity, "Identity was not read");
            }
        }

        [TestMethod]
        public void GetInCirculationProducts()
        {
            var discontinuedProducts = from product in TestClientContext.Products
                                       where !product.Discontinued
                                       select product;

            Console.WriteLine(discontinuedProducts.ToString());
            foreach (var entity in discontinuedProducts)
            {
                EntityDescriptor eDescriptor = TestClientContext.GetEntityDescriptor(entity);
                Assert.IsNotNull(eDescriptor.SelfLink, "Self link was not read");
                Assert.IsNotNull(eDescriptor.EditLink, "Edit link was not read");
                Assert.IsNotNull(eDescriptor.Identity, "Identity was not read");
            }
        }

        [TestMethod]
        public void GetOrdersOnMyBirthday()
        {
            var dateTimeType = new DateTime?(DateTime.Now).GetType();
            Console.WriteLine(dateTimeType);
            var ordersOnMyBirthday = from order in TestClientContext.Orders
                                     where order.OrderDate.Day == 29 && order.OrderDate.Month == 5
                                     select order;

            foreach (var entity in ordersOnMyBirthday)
            {
                EntityDescriptor eDescriptor = TestClientContext.GetEntityDescriptor(entity);
                Assert.IsNotNull(eDescriptor.SelfLink, "Self link was not read");
                Assert.IsNotNull(eDescriptor.EditLink, "Edit link was not read");
                Assert.IsNotNull(eDescriptor.Identity, "Identity was not read");
            }
        }

        [TestMethod]
        public void GetCustomersInLondon()
        {
            var customersInLondon = from customer in TestClientContext.Customers
                                    where customer.City == "London"
                                    select customer;

            Console.WriteLine(customersInLondon.ToString());
            foreach (var entity in customersInLondon)
            {
                EntityDescriptor eDescriptor = TestClientContext.GetEntityDescriptor(entity);
                Assert.IsNotNull(eDescriptor.SelfLink, "Self link was not read");
                Assert.IsNotNull(eDescriptor.EditLink, "Edit link was not read");
                Assert.IsNotNull(eDescriptor.Identity, "Identity was not read");
            }
        }


        [TestMethod]
        public void GetCustomersByKey()
        {
            var first5Customers = TestClientContext.Customers.Take(5).ToList();

            foreach (var customer in first5Customers)
            {
                IQueryable<Customer> customerByKey = TestClientContext.Customers.Where(c => c.PersonID == customer.PersonID);
                Console.WriteLine(customerByKey.ToString());
                var entity = customerByKey.Single();
                EntityDescriptor eDescriptor = TestClientContext.GetEntityDescriptor(entity);
                Assert.IsNotNull(eDescriptor.SelfLink, "Self link was not read");
                Assert.IsNotNull(eDescriptor.EditLink, "Edit link was not read");
                Assert.IsNotNull(eDescriptor.Identity, "Identity was not read");
            }
        }

        [TestMethod]
        public void GetOrderDetailsByKey()
        {
            var first5Details = TestClientContext.OrderDetails.Take(5).ToList();

            foreach (var order in first5Details)
            {
                IQueryable<OrderDetail> orderByKey = TestClientContext.OrderDetails.Where(c => c.OrderID == order.OrderID && c.ProductID == order.ProductID);
                Console.WriteLine(orderByKey.ToString());
                var entity = orderByKey.Single();
                EntityDescriptor eDescriptor = TestClientContext.GetEntityDescriptor(entity);
                Assert.IsNotNull(eDescriptor.SelfLink, "Self link was not read");
                Assert.IsNotNull(eDescriptor.EditLink, "Edit link was not read");
                Assert.IsNotNull(eDescriptor.Identity, "Identity was not read");
            }
        }
#endif

        [TestMethod]
        public void ParseServiceDocument()
        {
            var requestMessage = new Microsoft.Test.OData.Tests.Client.Common.HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri, UriKind.Absolute));
            var webResponse = requestMessage.GetResponse();

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            using (var messageReader = new ODataMessageReader(webResponse, readerSettings, Model))
            {
                ODataServiceDocument workSpace = messageReader.ReadServiceDocument();

                foreach (var entitySetName in entitySetsList.Keys)
                {
                    Assert.IsNotNull(workSpace.EntitySets.Single(c => c.Name == entitySetName));
                }
            }
        }

        private IQueryable CreateTopQuery(IQueryable rootQuery, int topValue)
        {
            Type[] typeArgs = new Type[] { rootQuery.ElementType };
            Expression topValueExpression = Expression.Constant(topValue);
            var result = Expression.Call(typeof(Queryable), "Take", typeArgs, rootQuery.Expression, topValueExpression);
            return rootQuery.Provider.CreateQuery(result);
        }

        private IQueryable CreateSkipQuery(IQueryable rootQuery, int skipValue)
        {
            Type[] typeArgs = new Type[] { rootQuery.ElementType };
            Expression topValueExpression = Expression.Constant(skipValue);
            var result = Expression.Call(typeof(Queryable), "Skip", typeArgs, rootQuery.Expression, topValueExpression);
            return rootQuery.Provider.CreateQuery(result);
        }

        private IEnumerable ExecuteUri(DataServiceContext clientContext, Uri uriToExecute, Type clientType)
        {
            MethodInfo executeOfTMethod = typeof(DataServiceContext).GetMethod("Execute", new[] { typeof(Uri) }).MakeGenericMethod(clientType);
            var results = executeOfTMethod.Invoke(clientContext, new object[] { uriToExecute });
            return results as IEnumerable;
        }
    }
}

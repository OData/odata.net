//-----------------------------------------------------------------------------
// <copyright file="ClientEntityDescriptorTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.TestCommon;
using Microsoft.OData.Client.E2E.Tests.ClientTests.Server;
using Microsoft.OData.Client.E2E.Tests.Common.Client.Default;
using Microsoft.OData.Client.E2E.Tests.Common.Server;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Client.E2E.Tests.ClientTests.Tests
{
    public class ClientEntityDescriptorTests : EndToEndTestBase<ClientEntityDescriptorTests.TestsStartup>
    {
        private readonly Uri _baseUri;
        private readonly Container _context;
        private IEdmModel _model = null;

        public class TestsStartup : TestStartupBase
        {
            public override void ConfigureServices(IServiceCollection services)
            {
                services.ConfigureControllers(typeof(ClientTestsController), typeof(MetadataController));

                services.AddControllers().AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(null)
                    .AddRouteComponents("odata", DefaultEdmModel.GetEdmModel()));
            }
        }

        public ClientEntityDescriptorTests(TestWebApplicationFactory<TestsStartup> fixture)
            : base(fixture)
        {
            _baseUri = new Uri(Client.BaseAddress, "odata/");
            _context = new Container(_baseUri);
            _model = DefaultEdmModel.GetEdmModel();
            _context.HttpClientFactory = HttpClientFactory;
            _context.KeyComparisonGeneratesFilterQuery = false;
        }

        private Dictionary<string, List<string>> entitySetsList = new Dictionary<string, List<string>>
        {
            {"People", new List<string>{"PersonID"}},
            {"Orders", new List<string>{"OrderID"}},
            {"ProductDetails", new List<string>{"ProductID", "ProductDetailID"}}
        };

        [Fact]
        public void TestRootQuery()
        {
            foreach (var entitySetKeys in entitySetsList.Keys)
            {
                IQueryable iqueryableProperty = typeof(Container).GetProperty(entitySetKeys).GetValue(_context, null) as IQueryable;
                foreach (var entity in iqueryableProperty)
                {
                    EntityDescriptor eDescriptor = _context.GetEntityDescriptor(entity);
                    //Self link was not read
                    Assert.NotNull(eDescriptor.SelfLink);
                    //Edit link was not read
                    Assert.NotNull(eDescriptor.EditLink);
                    //Identity was not read
                    Assert.NotNull(eDescriptor.Identity);
                }
            }
        }

        [Fact]
        public void TestTopOption()
        {
            foreach (var entitySetKeys in entitySetsList.Keys)
            {
                IQueryable iqueryableProperty = typeof(Container).GetProperty(entitySetKeys).GetValue(_context, null) as IQueryable;
                Console.WriteLine("running top query against root set '{0}'", entitySetKeys);
                var takeOneQuery = this.CreateTopQuery(iqueryableProperty, 1);
                Console.WriteLine(takeOneQuery.ToString());
                foreach (var entity in takeOneQuery)
                {
                    EntityDescriptor eDescriptor = _context.GetEntityDescriptor(entity);
                    //Self link was not read
                    Assert.NotNull(eDescriptor.SelfLink);
                    //Edit link was not read
                    Assert.NotNull(eDescriptor.EditLink);
                    //Identity was not read
                    Assert.NotNull(eDescriptor.Identity);
                }
            }
        }

        [Fact]
        public void TestSkipOption()
        {
            foreach (var entitySetKeys in entitySetsList.Keys)
            {
                IQueryable iqueryableProperty = typeof(Container).GetProperty(entitySetKeys).GetValue(_context, null) as IQueryable;
                Console.WriteLine("running top query against root set '{0}'", entitySetKeys);
                var takeOneQuery = this.CreateSkipQuery(iqueryableProperty, 1);
                Console.WriteLine(takeOneQuery.ToString());
                foreach (var entity in takeOneQuery)
                {
                    EntityDescriptor eDescriptor = _context.GetEntityDescriptor(entity);
                    //Self link was not read
                    Assert.NotNull(eDescriptor.SelfLink);
                    //Edit link was not read
                    Assert.NotNull(eDescriptor.EditLink);
                    //Identity was not read"
                    Assert.NotNull(eDescriptor.Identity);
                }
            }
        }

        [Fact]
        public void TestOrderByOption()
        {
            foreach (var entitySet in entitySetsList)
            {
                foreach (var keyName in entitySet.Value)
                {
                    IQueryable iqueryableProperty = typeof(Container).GetProperty(entitySet.Key).GetValue(_context, null) as IQueryable;
                    Uri executeUri = new Uri(String.Format("{0}?$orderby={1}", iqueryableProperty.ToString(), keyName));
                    Type clientType = iqueryableProperty.ElementType;
                    var orderedResults = this.ExecuteUri(_context, executeUri, clientType);
                    foreach (var entity in orderedResults)
                    {
                        EntityDescriptor eDescriptor = _context.GetEntityDescriptor(entity);
                        //Self link was not read
                        Assert.NotNull(eDescriptor.SelfLink);
                        //Edit link was not read
                        Assert.NotNull(eDescriptor.EditLink);
                        //Identity was not read
                        Assert.NotNull(eDescriptor.Identity);
                    }
                }
            }
        }

        [Fact]
        public void TestOrderByThenByOption()
        {
            foreach (var entitySet in entitySetsList)
            {
                IQueryable iqueryableProperty = typeof(Container).GetProperty(entitySet.Key).GetValue(_context, null) as IQueryable;
                Uri executeUri = new Uri(String.Format("{0}?$orderby={1}", iqueryableProperty.ToString(), string.Join(",", entitySet.Value)));
                Type clientType = iqueryableProperty.ElementType;
                var orderedResults = this.ExecuteUri(_context, executeUri, clientType);
                foreach (var entity in orderedResults)
                {
                    EntityDescriptor eDescriptor = _context.GetEntityDescriptor(entity);
                    //Self link was not read
                    Assert.NotNull(eDescriptor.SelfLink);
                    //Edit link was not read
                    Assert.NotNull(eDescriptor.EditLink);
                    //Identity was not read
                    Assert.NotNull(eDescriptor.Identity);
                }
            }
        }

        [Fact]
        public void TestOrderByDescendingOption()
        {
            foreach (var entitySet in entitySetsList)
            {
                foreach (var keyName in entitySet.Value)
                {
                    IQueryable iqueryableProperty = typeof(Container).GetProperty(entitySet.Key).GetValue(_context, null) as IQueryable;
                    Uri executeUri = new Uri(String.Format("{0}?$orderby={1} desc", iqueryableProperty.ToString(), keyName));
                    Type clientType = iqueryableProperty.ElementType;
                    var orderedResults = this.ExecuteUri(_context, executeUri, clientType);
                    foreach (var entity in orderedResults)
                    {
                        EntityDescriptor eDescriptor = _context.GetEntityDescriptor(entity);
                        //Self link was not read
                        Assert.NotNull(eDescriptor.SelfLink);
                        //Edit link was not read
                        Assert.NotNull(eDescriptor.EditLink);
                        //Identity was not read
                        Assert.NotNull(eDescriptor.Identity);
                    }
                }
            }
        }

        [Fact]
        public void GetDiscontinuedProducts()
        {
            var discontinuedProducts = from product in _context.Products 
                                       where product.Discontinued
                                       select product;

            Console.WriteLine(discontinuedProducts.ToString());
            foreach (var entity in discontinuedProducts)
            {
                EntityDescriptor eDescriptor = _context.GetEntityDescriptor(entity);
                //Self link was not read
                Assert.NotNull(eDescriptor.SelfLink);
                //Edit link was not read
                Assert.NotNull(eDescriptor.EditLink);
                //Identity was not read
                Assert.NotNull(eDescriptor.Identity);
            }
        }

        [Fact]
        public void GetInCirculationProducts()
        {
            var discontinuedProducts = from product in _context.Products
                                       where !product.Discontinued
                                       select product;

            Console.WriteLine(discontinuedProducts.ToString());
            foreach (var entity in discontinuedProducts)
            {
                EntityDescriptor eDescriptor = _context.GetEntityDescriptor(entity);
                //Self link was not read
                Assert.NotNull(eDescriptor.SelfLink);
                //Edit link was not read
                Assert.NotNull(eDescriptor.EditLink);
                //Identity was not read
                Assert.NotNull(eDescriptor.Identity);
            }
        }

        [Fact]
        public void GetOrdersOnMyBirthday()
        {
            var dateTimeType = new DateTime?(DateTime.Now).GetType();
            Console.WriteLine(dateTimeType);
            var ordersOnMyBirthday = from order in _context.Orders
                                     where order.OrderDate.Day == 29 && order.OrderDate.Month == 5
                                     select order;

            foreach (var entity in ordersOnMyBirthday)
            {
                EntityDescriptor eDescriptor = _context.GetEntityDescriptor(entity);
                //Self link was not read
                Assert.NotNull(eDescriptor.SelfLink);
                //Edit link was not read
                Assert.NotNull(eDescriptor.EditLink);
                //Identity was not read
                Assert.NotNull(eDescriptor.Identity);
            }
        }

        [Fact]
        public void GetCustomersInLondon()
        {
            var customersInLondon = from customer in _context.Customers
                                    where customer.City == "London"
                                    select customer;

            Console.WriteLine(customersInLondon.ToString());
            foreach (var entity in customersInLondon)
            {
                EntityDescriptor eDescriptor = _context.GetEntityDescriptor(entity);
                //Self link was not read
                Assert.NotNull(eDescriptor.SelfLink);
                //Edit link was not read
                Assert.NotNull(eDescriptor.EditLink);
                //Identity was not read
                Assert.NotNull(eDescriptor.Identity);
            }
        }

        [Fact]
        public void GetCustomersByKey()
        {
            var first5Customers = _context.Customers.Take(5).ToList();

            foreach (var customer in first5Customers)
            {
                IQueryable<Common.Client.Customer> customerByKey = _context.Customers.Where(c => c.PersonID == customer.PersonID);
                Console.WriteLine(customerByKey.ToString());
                var entity = customerByKey.Single();
                EntityDescriptor eDescriptor = _context.GetEntityDescriptor(entity);
                //Self link was not read
                Assert.NotNull(eDescriptor.SelfLink);
                //Edit link was not read
                Assert.NotNull(eDescriptor.EditLink);
                //Identity was not read
                Assert.NotNull(eDescriptor.Identity);
            }
        }

        [Fact]
        public void GetOrderDetailsByKey()
        {
            var first5Details = _context.OrderDetails.Take(5).ToList();

            foreach (var order in first5Details)
            {
                IQueryable<Common.Client.OrderDetail> orderByKey = _context.OrderDetails.Where(c => c.OrderID == order.OrderID && c.ProductID == order.ProductID);
                Console.WriteLine(orderByKey.ToString());
                var entity = orderByKey.Single();
                EntityDescriptor eDescriptor = _context.GetEntityDescriptor(entity);
                //Self link was not read
                Assert.NotNull(eDescriptor.SelfLink);
                //Edit link was not read
                Assert.NotNull(eDescriptor.EditLink);
                //Identity was not read
                Assert.NotNull(eDescriptor.Identity);
            }
        }

        [Fact]
        public void ParseServiceDocument()
        {
            var args = new DataServiceClientRequestMessageArgs(
                "GET",
                new Uri(_baseUri.AbsoluteUri, UriKind.Absolute),
                usePostTunneling: false,
                new Dictionary<string, string>(),
                HttpClientFactory);

            var requestMessage = new HttpClientRequestMessage(args);
            var webResponse = requestMessage.GetResponse();

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri };

            using (var messageReader = new ODataMessageReader(webResponse, readerSettings, _model))
            {
                ODataServiceDocument workSpace = messageReader.ReadServiceDocument();

                foreach (var entitySetName in entitySetsList.Keys)
                {
                    Assert.NotNull(workSpace.EntitySets.Single(c => c.Name == entitySetName));
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

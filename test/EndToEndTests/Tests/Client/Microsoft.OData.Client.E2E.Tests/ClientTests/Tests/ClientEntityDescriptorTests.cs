//-----------------------------------------------------------------------------
// <copyright file="ClientEntityDescriptorTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.TestCommon;
using Microsoft.OData.Client.E2E.Tests.ClientTests.Server;
using Microsoft.OData.Client.E2E.Tests.Common.Client.Default;
using Microsoft.OData.Client.E2E.Tests.Common.Client.Default.Default;
using Microsoft.OData.Client.E2E.Tests.Common.Server.Default;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Client.E2E.Tests.ClientTests.Tests
{
    public class ClientEntityDescriptorTests : EndToEndTestBase<ClientEntityDescriptorTests.TestsStartup>
    {
        private readonly Uri _baseUri;
        private readonly Container _context;
        private readonly IEdmModel _model;

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
            _model = DefaultEdmModel.GetEdmModel();
            _context = new Container(_baseUri)
            {
                HttpClientFactory = HttpClientFactory
            };

            ResetDefaultDataSource();
        }

        private static readonly List<string> entitySetsList = ["People", "Orders", "ProductDetails"];

        public static IEnumerable<object[]> GetEntitySetData()
        {
            foreach (var entitySetKey in entitySetsList)
            {
                yield return new object[] { entitySetKey };
            }
        }

        [Theory]
        [MemberData(nameof(GetEntitySetData))]
        public void EntityDescriptor_LinksAndIdentity_AreNotNull_ForAllEntities(string entitySetKey)
        {
            IQueryable iqueryableProperty = typeof(Container).GetProperty(entitySetKey).GetValue(_context, null) as IQueryable;
            foreach (var entity in iqueryableProperty)
            {
                EntityDescriptor eDescriptor = _context.GetEntityDescriptor(entity);

                Assert.NotNull(eDescriptor.SelfLink);
                Assert.NotNull(eDescriptor.EditLink);
                Assert.NotNull(eDescriptor.Identity);
            }
        }

        [Fact]
        public async Task TopQuery_ExecutesSuccessfully()
        {
            var person = _context.People.Take(1);
            Assert.Equal("http://localhost/odata/People?$top=1", person.ToString());

            var topPerson = (await ((DataServiceQuery<Common.Client.Default.Person>)person).ExecuteAsync()).Single();

            Assert.Equal("Bob", topPerson.FirstName);
        }

        [Fact]
        public async Task SkipOption_ExecutesSuccessfully()
        {
            var allPeople = (await _context.People.ExecuteAsync()).ToList();
            var skipFirstPersonQuery = _context.People.Skip(1);
            var peopleAfterSkippingFirstPerson = (await ((DataServiceQuery<Common.Client.Default.Person>)skipFirstPersonQuery).ExecuteAsync()).ToList();

            Assert.Equal("http://localhost/odata/People?$skip=1", skipFirstPersonQuery.ToString());
            Assert.Equal(5, allPeople.Count);
            Assert.Equal(4, peopleAfterSkippingFirstPerson.Count);
        }

        [Fact]
        public async Task OrderByOption_ExecutesSuccessfully()
        {
            var orderByQuery = from p in _context.People 
                         orderby p.FirstName 
                         select p;

            Assert.Equal("http://localhost/odata/People?$orderby=FirstName", orderByQuery.ToString());

            var peopleList = (await ((DataServiceQuery<Common.Client.Default.Person>)orderByQuery).ExecuteAsync()).ToList();

            var firstPerson = peopleList.First();
            Assert.Equal("Bob", firstPerson.FirstName);

            var lastPerson = peopleList.Last();
            Assert.Equal("Peter", lastPerson.FirstName);
        }

        [Fact]
        public async Task OrderByDescendingOption_ExecutesSuccessfully()
        {
            var orderByDescQuery = from p in _context.People
                         orderby p.FirstName descending
                         select p;

            Assert.Equal("http://localhost/odata/People?$orderby=FirstName desc", orderByDescQuery.ToString());

            var peopleList = (await ((DataServiceQuery<Common.Client.Default.Person>)orderByDescQuery).ExecuteAsync()).ToList();
            var firstPerson = peopleList.First();

            Assert.Equal("Peter", firstPerson.FirstName);

            var lastPerson = peopleList.Last();

            Assert.Equal("Bob", lastPerson.FirstName);
        }

        [Fact]
        public async Task FilterByBooleanProperty_ExecutesSuccessfully()
        {
            var filterByBoolQuery = from product in _context.Products
                                       where product.Discontinued
                                       select product;

            Assert.Equal("http://localhost/odata/Products?$filter=Discontinued", filterByBoolQuery.ToString());

            var discontinuedProducts = (await ((DataServiceQuery<Common.Client.Default.Product>)filterByBoolQuery).ExecuteAsync()).ToList();

            Assert.Single(discontinuedProducts);
            Assert.True(discontinuedProducts.Single().Discontinued);
        }

        [Fact]
        public async Task FilterByNegatedBooleanProperty_ExecutesSuccessfully()
        {
            var filterByBoolQuery = from product in _context.Products
                                    where !product.Discontinued
                                    select product;

            Assert.Equal("http://localhost/odata/Products?$filter=not Discontinued", filterByBoolQuery.ToString());

            var productsInCirculation = (await ((DataServiceQuery<Common.Client.Default.Product>)filterByBoolQuery).ExecuteAsync()).ToList();

            Assert.Equal(4, productsInCirculation.Count);

            foreach (var product in productsInCirculation)
            {
                Assert.False(product.Discontinued);
            }
        }

        [Fact]
        public async Task FilterByDayAndMonthOfDateProperty_ExecutesSuccessfully()
        {
            var dateTimeType = new DateTime?(DateTime.Now).GetType();
            var filterByDateTimeQuery = from order in _context.Orders
                                     where order.OrderDate.Day == 29 && order.OrderDate.Month == 5
                                     select order;

            Assert.Equal("http://localhost/odata/Orders?$filter=day(OrderDate) eq 29 and month(OrderDate) eq 5", filterByDateTimeQuery.ToString());

            var ordersOnMyBirthday = (await ((DataServiceQuery<Common.Client.Default.Order>)filterByDateTimeQuery).ExecuteAsync()).ToList();

            Assert.Single(ordersOnMyBirthday);
            Assert.Equal("5/29/2011 12:00:00 AM", ordersOnMyBirthday.Single().OrderDate.Date.ToString());
        }

        [Fact]
        public async Task FilterByStringProperty_ExecutesSuccessfully()
        {
            var filterByStringQuery = from customer in _context.Customers
                                    where customer.City == "London"
                                    select customer;

            Assert.Equal("http://localhost/odata/Customers?$filter=City eq 'London'", filterByStringQuery.ToString());

            var customersInLondon = (await ((DataServiceQuery<Common.Client.Default.Customer>)filterByStringQuery).ExecuteAsync()).ToList();

            Assert.Single(customersInLondon);
            Assert.Equal("London", customersInLondon.Single().City);
        }

        [Fact]
        public async Task FilterByKeyProperty_UsingWhereClause_ExecutesSuccessfully()
        {
            IQueryable<Common.Client.Default.Customer> filterByWhereClauseQuery = _context.Customers.Where(c => c.PersonID == 1);

            Assert.Equal("http://localhost/odata/Customers?$filter=PersonID eq 1", filterByWhereClauseQuery.ToString());

            var customer = (await ((DataServiceQuery<Common.Client.Default.Customer>)filterByWhereClauseQuery).ExecuteAsync()).Single();

            Assert.Equal("Bob", customer.FirstName);
        }

        [Fact]
        public async Task FilterByKeyProperty_UsingByKey_ExecutesSuccessfully()
        {
            var filterByKeyQuery = _context.Customers.ByKey(1);

            Assert.Equal("http://localhost/odata/Customers(1)", filterByKeyQuery.RequestUri.ToString());

            var customer = await filterByKeyQuery.GetValueAsync();
            Assert.Equal("Bob", customer.FirstName);
        }

        [Fact]
        public async Task FilterByCompositeKey_UsingWhereClause_ExecutesSuccessfully()
        {
            IQueryable<Common.Client.Default.OrderDetail> filterByWhereClauseQuery = _context.OrderDetails.Where(c => c.OrderID == 7 && c.ProductID == 5);

            Assert.Equal("http://localhost/odata/OrderDetails?$filter=OrderID eq 7 and ProductID eq 5", filterByWhereClauseQuery.ToString());

            var orderDetail = (await ((DataServiceQuery<Common.Client.Default.OrderDetail>)filterByWhereClauseQuery).ExecuteAsync()).Single();
            Assert.Equal(7, orderDetail.OrderID);
            Assert.Equal(5, orderDetail.ProductID);
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

                foreach (var entitySetName in entitySetsList)
                {
                    Assert.NotNull(workSpace.EntitySets.Single(c => c.Name == entitySetName));
                }
            }
        }

        private void ResetDefaultDataSource()
        {
            var actionUri = new Uri(_baseUri + "clienttests/Default.ResetDefaultDataSource", UriKind.Absolute);
            _context.Execute(actionUri, "POST");
        }
    }
}

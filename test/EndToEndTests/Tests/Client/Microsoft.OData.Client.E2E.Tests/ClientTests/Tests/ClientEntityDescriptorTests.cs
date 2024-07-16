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
        public void EntityDescriptor_LinksAndIdentity_AreNotNull_ForAllEntities()
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
        public void TopQuery_ExecutesSuccessfully()
        {
            var person = _context.People.Take(1);
            Assert.Equal("http://localhost/odata/People?$top=1", person.ToString());
            Assert.Single(person);
        }

        [Fact]
        public void SkipOption_ExecutesSuccessfully()
        {
            var people = _context.People.Skip(1);
            Assert.Equal("http://localhost/odata/People?$skip=1", people.ToString());
            Assert.NotNull(people.FirstOrDefault().PersonID);
        }

        [Fact]
        public void OrderByOption_ExecutesSuccessfully()
        {
            var people = from p in _context.People 
                         orderby p.FirstName 
                         select p;

            Assert.Equal("http://localhost/odata/People?$orderby=FirstName", people.ToString());
            Assert.NotNull(people.ToList());
        }

        [Fact]
        public void OrderByDescendingOption_ExecutesSuccessfully()
        {
            var people = from p in _context.People
                         orderby p.FirstName descending
                         select p;

            Assert.Equal("http://localhost/odata/People?$orderby=FirstName desc", people.ToString());

            Assert.NotNull(people.FirstOrDefault());
        }

        [Fact]
        public void GetDiscontinuedProducts()
        {
            var discontinuedProducts = from product in _context.Products
                                       where product.Discontinued
                                       select product;

            Assert.Equal("http://localhost/odata/Products?$filter=Discontinued", discontinuedProducts.ToString());

            Assert.NotNull(discontinuedProducts.FirstOrDefault());
        }

        [Fact]
        public void GetInCirculationProducts()
        {
            var productsInCirculation = from product in _context.Products
                                       where !product.Discontinued
                                       select product;

            Assert.Equal("http://localhost/odata/Products?$filter=not Discontinued", productsInCirculation.ToString());

            Assert.NotNull(productsInCirculation.FirstOrDefault());
        }

        [Fact]
        public void GetOrdersOnMyBirthday()
        {
            var dateTimeType = new DateTime?(DateTime.Now).GetType();
            var ordersOnMyBirthday = from order in _context.Orders
                                     where order.OrderDate.Day == 29 && order.OrderDate.Month == 5
                                     select order;

            Assert.Equal("http://localhost/odata/Orders?$filter=day(OrderDate) eq 29 and month(OrderDate) eq 5", ordersOnMyBirthday.ToString());

            Assert.NotNull(ordersOnMyBirthday.FirstOrDefault());
        }

        [Fact]
        public void GetCustomersInLondon()
        {
            var customersInLondon = from customer in _context.Customers
                                    where customer.City == "London"
                                    select customer;

            Assert.Equal("http://localhost/odata/Customers?$filter=City eq 'London'", customersInLondon.ToString());

            Assert.NotNull(customersInLondon.FirstOrDefault());
        }

        [Fact]
        public void GetCustomersByKey()
        {
            IQueryable<Common.Client.Default.Customer> customerByKey = _context.Customers.Where(c => c.PersonID == 1);
            Assert.Equal("Bob", customerByKey.FirstOrDefault().FirstName);
        }

        [Fact]
        public void GetOrderDetailsByKey()
        {
            IQueryable<Common.Client.Default.OrderDetail> orderByKey = _context.OrderDetails.Where(c => c.OrderID == 7 && c.ProductID == 5);
            Assert.Equal(7, orderByKey.FirstOrDefault().OrderID);
            Assert.Equal(5, orderByKey.FirstOrDefault().ProductID);
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
    }
}

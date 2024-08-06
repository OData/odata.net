//-----------------------------------------------------------------------------
// <copyright file="ClientQueryTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.TestCommon;
using Microsoft.OData.Client.E2E.Tests.ClientTests.Server;
using Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd;
using Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd.Default;
using Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd;
using Xunit;

namespace Microsoft.OData.Client.E2E.Tests.ClientTests.Tests
{
    public class ClientQueryTests : EndToEndTestBase<ClientQueryTests.TestsStartup>
    {
        private readonly Uri _baseUri;
        private readonly Container _context;

        public class TestsStartup : TestStartupBase
        {
            public override void ConfigureServices(IServiceCollection services)
            {
                services.ConfigureControllers(typeof(ClientQueryTestsController), typeof(MetadataController));

                services.AddControllers().AddOData(opt => opt.EnableQueryFeatures()
                    .AddRouteComponents("odata", CommonEndToEndEdmModel.GetEdmModel()));
            }
        }

        public ClientQueryTests(TestWebApplicationFactory<TestsStartup> fixture)
            : base(fixture)
        {
            _baseUri = new Uri(Client.BaseAddress, "odata/");
            _context = new Container(_baseUri);
            _context.HttpClientFactory = HttpClientFactory;
        }

        [Fact]
        public void DollarFilter_UsingContains_ExecutesSuccessfully()
        {
            var testlists = new List<string>()
            {
                new("People?$filter=contains(Name, 'm')"),
                new("People?$filter=contains('m', Name)"),
                new("People?$filter=not contains(Name, 'm')"),
                new("People?$filter=contains(Name, 'm') eq true"),
                new("People?$filter=false eq contains(Name, 'm')"),
                new("People?$filter=contains(Name, substring('name',  2, 1))"),
                new("People?$filter=contains(concat('User','name'), 'name')"),
                new("People?$filter=contains(concat('User','name'), substring('name',  2, 1))"),
            };

            foreach (var test in testlists)
            {
                var result = _context.Execute<Common.Clients.EndToEnd.Person>(new Uri(_baseUri.OriginalString + test)).ToArray();
                Assert.NotNull(result);
            }
        }

        [Fact]
        public void Using_LinqContains_ExecutesSuccessfully()
        {
            var result = (from c in _context.People
                          where c.Name.Contains("m")
                          select new Common.Clients.EndToEnd.Person() { Name = c.Name }) as DataServiceQuery<Common.Clients.EndToEnd.Person>;

            Assert.NotNull(result);

            result = (DataServiceQuery<Common.Clients.EndToEnd.Person>)_context.People.Where(c => c.Name.Contains("m"));
            Assert.NotNull(result);
        }

        [Fact]
        public void Using_PrimitiveTypeInRequestUrl_ExecutesSuccessfully()
        {
            const string stringOfCast = "cast(PersonId,Edm.Byte)";

            //GET http://jinfutanodata01:9090/AstoriaDefault635157546921762475/Person()?$filter=cast(cast(PersonId,Edm.Byte),Edm.Int32)%20gt%200 HTTP/1.1
            //all the IDs in [-10, 2] except 0 are counted in.
            var result = _context.People.Where(c => (Byte)c.PersonId > 0);
            var stringOfQuery = result.ToString();
            Assert.Contains(stringOfCast, stringOfQuery);
            Assert.Equal(12, result.ToList().Count());

            //GET http://jinfutanodata01:9090/AstoriaDefault635157551526070289/Person()?$filter=PersonId%20le%20256 HTTP/1.1
            //all the IDs in [1, 2] are counted in.
            result = _context.People.Where(c => c.PersonId > 0);
            Assert.Equal(2, result.ToList().Count());
        }

        [Fact]
        public void InvalidDollarFilter_UsingContains_ThrowsExceptions()
        {
            string[] errorUrls =
            {
                "Logins?$filter=contains(Username, 1)",
                "People?$filter=contains(Name, \"m\")",
                "Cars?$filter=contains(VIN, '12')"
            };

            foreach (var errorUrl in errorUrls)
            {
                try
                {
                    _context.Execute<Common.Clients.EndToEnd.Person>(new Uri(_baseUri.OriginalString + errorUrl));
                    Assert.True(false, "Expected Exception not thrown for " + errorUrl);
                }
                catch (DataServiceQueryException ex)
                {
                    Assert.Equal(400, ex.Response.StatusCode);
                }
            }
        }

        [Fact]
        public void GetAllPagesTest()
        {
            int allCustomersCount = _context.Customers.Count();
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

            _context.Configurations.ResponsePipeline.OnFeedEnded((args) =>
            {
                nextPageLink = args.Feed.NextPageLink;
            });

            _context.SendingRequest2 += sendRequestEvent;
            int queryCustomersCount = _context.Customers.GetAllPages().Count();
            Assert.Equal(allCustomersCount, queryCustomersCount);

            //$filter
            _context.SendingRequest2 -= sendRequestEvent;
            var filterCustomersCount = _context.Customers.Where(c => c.CustomerId > -5).Count();

            _context.SendingRequest2 += sendRequestEvent;
            CheckNextLink = false;
            queryCustomersCount = ((DataServiceQuery<Common.Clients.EndToEnd.Customer>)_context.Customers.Where(c => c.CustomerId > -5)).GetAllPages().ToList().Count();
            Assert.Equal(filterCustomersCount, queryCustomersCount);

            //$projection
            CheckNextLink = false;
            queryCustomersCount = ((DataServiceQuery<Common.Clients.EndToEnd.Customer>)_context.Customers.Select(c => new Common.Clients.EndToEnd.Customer() { CustomerId = c.CustomerId, Name = c.Name })).GetAllPages().ToList().Count();
            Assert.Equal(allCustomersCount, queryCustomersCount);

            //$expand
            CheckNextLink = false;
            queryCustomersCount = _context.Customers.Expand(c => c.Orders).GetAllPages().Count();
            Assert.Equal(allCustomersCount, queryCustomersCount);

            //$top
            CheckNextLink = false;
            queryCustomersCount = ((DataServiceQuery<Common.Clients.EndToEnd.Customer>)_context.Customers.Take(4)).GetAllPages().ToList().Count();
            Assert.Equal(4, queryCustomersCount);

            //$orderby
            CheckNextLink = false;
            queryCustomersCount = ((DataServiceQuery<Common.Clients.EndToEnd.Customer>)_context.Customers.OrderBy(c => c.Name)).GetAllPages().ToList().Count();
            Assert.Equal(allCustomersCount, queryCustomersCount);

            //$skip
            CheckNextLink = false;
            queryCustomersCount = ((DataServiceQuery<Common.Clients.EndToEnd.Customer>)_context.Customers.Skip(4)).GetAllPages().ToList().Count();
            Assert.Equal(allCustomersCount - 4, queryCustomersCount);
        }

        [Fact]
        public void PagingOnNavigationProperty()
        {
            int allOrdersCount = _context.Customers.ByKey(new Dictionary<string, object> { { "CustomerId", -10 } }).Orders.Count();
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

            _context.Configurations.ResponsePipeline.OnFeedEnded((args) =>
            {
                nextPageLink = args.Feed.NextPageLink;
            });

            _context.SendingRequest2 += sendRequestEvent;
            //Navigation Property
            CheckNextLink = false;
            var queryOrderCount = _context.Customers.ByKey(new Dictionary<string, object> { { "CustomerId", -10 } }).Orders.GetAllPages().ToList().Count();
            Assert.Equal(allOrdersCount, queryOrderCount);
        }

        [Fact]
        public void DuplicateQueryTest()
        {
            try
            {
                _context.Execute<Common.Clients.EndToEnd.Person>(new Uri(_baseUri.OriginalString + "People?$orderby=PersonId&$orderby=PersonId"));
                Assert.True(false, "Expected Exception not thrown for duplicate odata query options.");
            }
            catch (DataServiceQueryException ex)
            {
                Assert.Equal(400, ex.Response.StatusCode);
            }

            var entryResults = _context.Execute<Common.Clients.EndToEnd.Person>(new Uri(_baseUri.OriginalString + "People?nonODataQuery=foo&$filter=PersonId%20eq%200&nonODataQuery=bar"));
            Assert.Single(entryResults);
        }
    }
}

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
            _context = new Container(_baseUri)
            {
                HttpClientFactory = HttpClientFactory
            };

            ResetDataSource();
        }

        [Theory]
        [InlineData("People?$filter=contains(Name, 'm')", 10)]
        [InlineData("People?$filter=contains('m', Name)", 0)]
        [InlineData("People?$filter=not contains(Name, 'm')", 3)]
        [InlineData("People?$filter=contains(Name, 'm') eq true", 10)]
        [InlineData("People?$filter=false eq contains(Name, 'm')", 3)]
        [InlineData("People?$filter=contains(Name, substring('name',  2, 1))", 10)]
        [InlineData("People?$filter=contains(concat('User','name'), 'name')", 13)]
        [InlineData("People?$filter=contains(concat('User','name'), substring('name',  2, 1))", 13)]
        public async Task DollarFilter_UsingContains_ExecutesSuccessfully(string query, int expectedCount)
        {
            // Act
            var result = (await _context.ExecuteAsync<Common.Clients.EndToEnd.Person>(new Uri(_baseUri.OriginalString + query))).ToArray();

            // Assert
            Assert.Equal(expectedCount, result.Length);
        }

        [Theory]
        [InlineData("People?$filter=Name in ('')")]
        [InlineData("People?$filter=Name in ['']")]
        [InlineData("People?$filter=Name in ( '' )")]
        [InlineData("People?$filter=Name in [ '' ]")]
        [InlineData("People?$filter=Name in (\"\")")]
        [InlineData("People?$filter=Name in [\"\"]")]
        [InlineData("People?$filter=Name in ( \"\" )")]
        [InlineData("People?$filter=Name in [ \"\" ]")]
        [InlineData("People?$filter=Name in ( ' ' )")]
        [InlineData("People?$filter=Name in [ ' ' ]")]
        [InlineData("People?$filter=Name in ( \"  \" )")]
        [InlineData("People?$filter=Name in [ \"   \"]")]
        [InlineData("People?$filter=Name in ( '', ' ' )")]
        [InlineData("People?$filter=Name in [ '', ' ' ]")]
        [InlineData("People?$filter=Name in ( \"\", \" \" )")]
        [InlineData("People?$filter=Name in [ \"\", \" \" ]")]
        [InlineData("People?$filter=Name in ( '', \" \" )")]
        [InlineData("People?$filter=Name in [ '', \" \" ]")]
        [InlineData("People?$filter=Name in ( \"\", ' ' )")]
        [InlineData("People?$filter=Name in [ \"\", ' ' ]")]
        [InlineData("People?$filter=Name in [ 'null', 'null' ]")]
        public async Task DollarFilter_WithCollectionWithEmptyString_ExecutesSuccessfully(string query)
        {
            // Act
            var response = await _context.ExecuteAsync<Common.Clients.EndToEnd.Person>(new Uri(_baseUri.OriginalString + query));

            // Assert
            Assert.Empty(response.ToArray());
        }

        [Fact]
        public async Task Using_LinqContains_ExecutesSuccessfully()
        {
            var result = (from c in _context.People
                          where c.Name.Contains("m")
                          select new Common.Clients.EndToEnd.Person() { Name = c.Name }) as DataServiceQuery<Common.Clients.EndToEnd.Person>;

            Assert.Equal("http://localhost/odata/People?$filter=contains(Name,'m')&$select=Name", result.ToString());
            Assert.Equal(10, (await result.ExecuteAsync()).Count());

            result = (DataServiceQuery<Common.Clients.EndToEnd.Person>)_context.People.Where(c => c.Name.Contains("m"));

            Assert.Equal("http://localhost/odata/People?$filter=contains(Name,'m')", result.ToString());
            Assert.Equal(10, (await result.ExecuteAsync()).Count());
        }

        [Fact]
        public async Task Using_PrimitiveTypeInRequestUrl_ExecutesSuccessfully()
        {
            const string stringOfCast = "cast(PersonId,Edm.Byte)";

            //GET http://localhost/odata/People?$filter=cast(cast(PersonId,Edm.Byte),Edm.Int32) gt 0 HTTP/1.1
            //all the IDs in [-10, 2] except 0 are counted in.
            var result = _context.People.Where(c => (Byte)c.PersonId > 0);
            var stringOfQuery = result.ToString();
            Assert.Equal("http://localhost/odata/People?$filter=cast(cast(PersonId,Edm.Byte),Edm.Int32) gt 0", stringOfQuery);
            Assert.Contains(stringOfCast, stringOfQuery);
            Assert.Equal(12, (await ((DataServiceQuery<Common.Clients.EndToEnd.Person>)result).ExecuteAsync()).Count());

            //GET http://localhost/odata/People?$filter=PersonId gt 0 HTTP/1.1
            //all the IDs in [1, 2] are counted in.
            result = _context.People.Where(c => c.PersonId > 0);
            Assert.Equal("http://localhost/odata/People?$filter=PersonId gt 0", result.ToString());
            Assert.Equal(2, (await ((DataServiceQuery<Common.Clients.EndToEnd.Person>)result).ExecuteAsync()).Count());
        }

        [Theory]
        [InlineData("Logins?$filter=contains(Username, 1)")]
        [InlineData("People?$filter=contains(Name, \"m\")")]
        [InlineData("Cars?$filter=contains(VIN, '12')")]
        public async Task InvalidDollarFilter_UsingContains_ThrowsExceptions(string errorUrl)
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<DataServiceQueryException>(async () =>
                await _context.ExecuteAsync<Common.Clients.EndToEnd.Person>(new Uri(_baseUri.OriginalString + errorUrl))
            );

            // Verify that the exception contains a 400 status code
            Assert.Equal(400, exception.Response.StatusCode);
        }

        [Fact]
        public async Task GetAllPages_WithPaging_VerifiesPagingRequests()
        {
            // Arrange
            int expectedCount = (await _context.Customers.ExecuteAsync()).Count();
            bool checkNextLink = false;
            Uri nextPageLink = null;

            EventHandler<SendingRequest2EventArgs> sendRequestEvent = (sender, args) =>
            {
                if (checkNextLink)
                {
                    Assert.Equal(nextPageLink.AbsoluteUri, args.RequestMessage.Url.AbsoluteUri);
                }

                checkNextLink = true;
            };

            _context.Configurations.ResponsePipeline.OnFeedEnded((args) =>
            {
                nextPageLink = args.Feed.NextPageLink;
            });

            _context.SendingRequest2 += sendRequestEvent;

            // Act
            int resultCount = (await _context.Customers.GetAllPagesAsync()).Count();

            // Assert
            Assert.Equal(expectedCount, resultCount);

            _context.SendingRequest2 -= sendRequestEvent;
        }

        [Fact]
        public async Task GetAllPages_WithFilterAndPaging_VerifiesPagingRequests()
        {
            // Arrange
            int expectedCount = _context.Customers.Where(c => c.CustomerId > -5).Count();
            bool checkNextLink = false;
            Uri nextPageLink = null;

            EventHandler<SendingRequest2EventArgs> sendRequestEvent = (sender, args) =>
            {
                if (checkNextLink)
                {
                    Assert.Equal(nextPageLink.AbsoluteUri, args.RequestMessage.Url.AbsoluteUri);
                }

                checkNextLink = true;
            };

            _context.Configurations.ResponsePipeline.OnFeedEnded((args) =>
            {
                nextPageLink = args.Feed.NextPageLink;
            });

            _context.SendingRequest2 += sendRequestEvent;

            // Act
            int resultCount = (await ((DataServiceQuery<Common.Clients.EndToEnd.Customer>)_context.Customers.Where(c => c.CustomerId > -5)).ExecuteAsync()).Count();

            // Assert
            Assert.Equal(expectedCount, resultCount);

            _context.SendingRequest2 -= sendRequestEvent;
        }

        [Fact]
        public void GetAllPages_WithSelectAndPaging_VerifiesPagingRequests()
        {
            // Arrange
            int expectedCount = _context.Customers.Count();
            bool checkNextLink = false;
            Uri nextPageLink = null;

            EventHandler<SendingRequest2EventArgs> sendRequestEvent = (sender, args) =>
            {
                if (checkNextLink)
                {
                    Assert.Equal(nextPageLink.AbsoluteUri, args.RequestMessage.Url.AbsoluteUri);
                }

                checkNextLink = true;
            };

            _context.Configurations.ResponsePipeline.OnFeedEnded((args) =>
            {
                nextPageLink = args.Feed.NextPageLink;
            });

            _context.SendingRequest2 += sendRequestEvent;

            // Act
            int resultCount = ((DataServiceQuery<Common.Clients.EndToEnd.Customer>)_context.Customers.Select(c => new Common.Clients.EndToEnd.Customer() { CustomerId = c.CustomerId, Name = c.Name })).GetAllPages().Count();

            // Assert
            Assert.Equal(expectedCount, resultCount);

            _context.SendingRequest2 -= sendRequestEvent;
        }

        [Fact]
        public void GetAllPages_WithExpandAndPaging_VerifiesPagingRequests()
        {
            // Arrange
            int expectedCount = _context.Customers.Count();
            bool checkNextLink = false;
            Uri nextPageLink = null;

            EventHandler<SendingRequest2EventArgs> sendRequestEvent = (sender, args) =>
            {
                if (checkNextLink)
                {
                    Assert.Equal(nextPageLink.AbsoluteUri, args.RequestMessage.Url.AbsoluteUri);
                }

                checkNextLink = true;
            };

            _context.Configurations.ResponsePipeline.OnFeedEnded((args) =>
            {
                nextPageLink = args.Feed.NextPageLink;
            });

            _context.SendingRequest2 += sendRequestEvent;

            // Act
            int resultCount = _context.Customers.Expand(c => c.Orders).GetAllPages().Count();

            // Assert
            Assert.Equal(expectedCount, resultCount);

            _context.SendingRequest2 -= sendRequestEvent;
        }

        [Fact]
        public void GetAllPages_WithTopAndPaging_VerifiesPagingRequests()
        {
            // Arrange
            int expectedCount = 4;
            bool checkNextLink = false;
            Uri nextPageLink = null;

            EventHandler<SendingRequest2EventArgs> sendRequestEvent = (sender, args) =>
            {
                if (checkNextLink)
                {
                    Assert.Equal(nextPageLink.AbsoluteUri, args.RequestMessage.Url.AbsoluteUri);
                }

                checkNextLink = true;
            };

            _context.Configurations.ResponsePipeline.OnFeedEnded((args) =>
            {
                nextPageLink = args.Feed.NextPageLink;
            });

            _context.SendingRequest2 += sendRequestEvent;

            // Act
            int resultCount = ((DataServiceQuery<Common.Clients.EndToEnd.Customer>)_context.Customers.Take(4)).GetAllPages().Count();

            // Assert
            Assert.Equal(expectedCount, resultCount);

            _context.SendingRequest2 -= sendRequestEvent;
        }

        [Fact]
        public void GetAllPages_WithOrderByAndPaging_VerifiesPagingRequests()
        {
            // Arrange
            int expectedCount = _context.Customers.Count();
            bool checkNextLink = false;
            Uri nextPageLink = null;

            EventHandler<SendingRequest2EventArgs> sendRequestEvent = (sender, args) =>
            {
                if (checkNextLink)
                {
                    Assert.Equal(nextPageLink.AbsoluteUri, args.RequestMessage.Url.AbsoluteUri);
                }

                checkNextLink = true;
            };

            _context.Configurations.ResponsePipeline.OnFeedEnded((args) =>
            {
                nextPageLink = args.Feed.NextPageLink;
            });

            _context.SendingRequest2 += sendRequestEvent;

            // Act
            int resultCount = ((DataServiceQuery<Common.Clients.EndToEnd.Customer>)_context.Customers.OrderBy(c => c.Name)).GetAllPages().Count();

            // Assert
            Assert.Equal(expectedCount, resultCount);

            _context.SendingRequest2 -= sendRequestEvent;
        }

        [Fact]
        public void GetAllPages_WithSkipByAndPaging_VerifiesPagingRequests()
        {
            // Arrange
            int expectedCount = 6;
            bool checkNextLink = false;
            Uri nextPageLink = null;

            EventHandler<SendingRequest2EventArgs> sendRequestEvent = (sender, args) =>
            {
                if (checkNextLink)
                {
                    Assert.Equal(nextPageLink.AbsoluteUri, args.RequestMessage.Url.AbsoluteUri);
                }

                checkNextLink = true;
            };

            _context.Configurations.ResponsePipeline.OnFeedEnded((args) =>
            {
                nextPageLink = args.Feed.NextPageLink;
            });

            _context.SendingRequest2 += sendRequestEvent;

            // Act
            int resultCount = ((DataServiceQuery<Common.Clients.EndToEnd.Customer>)_context.Customers.Skip(4)).GetAllPages().Count();

            // Assert
            Assert.Equal(expectedCount, resultCount);

            _context.SendingRequest2 -= sendRequestEvent;
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
            // Assert that a DataServiceQueryException is thrown for duplicate OData query options.
            var exception = Assert.Throws<DataServiceQueryException>(() =>
            {
                _context.Execute<Common.Clients.EndToEnd.Person>(
                    new Uri(_baseUri.OriginalString + "People?$orderby=PersonId&$orderby=PersonId")
                );
            });

            // Verify that the exception is due to a 400 Bad Request.
            Assert.Equal(400, exception.Response.StatusCode);

            // Execute a valid query with non-OData query options and verify the result.
            var entryResults = _context.Execute<Common.Clients.EndToEnd.Person>(
                new Uri(_baseUri.OriginalString + "People?nonODataQuery=foo&$filter=PersonId%20eq%200&nonODataQuery=bar")
            );

            Assert.Single(entryResults);
        }

        private void ResetDataSource()
        {
            var actionUri = new Uri(_baseUri + "clientquery/Default.ResetDataSource", UriKind.Absolute);
            _context.Execute(actionUri, "POST");
        }
    }
}

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

                services.AddControllers().AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(null)
                    .AddRouteComponents("odata", CommonEndToEndEdmModel.GetEdmModel()));
            }
        }

        public ClientQueryTests(TestWebApplicationFactory<TestsStartup> fixture)
            : base(fixture)
        {
            _baseUri = new Uri(Client.BaseAddress, "odata/");
            _context = new Container(_baseUri);
            _context.HttpClientFactory = HttpClientFactory;
            //_context.KeyComparisonGeneratesFilterQuery = false;
        }

        [Fact]
        public void ContainsUrlTest()
        {
            var testlists = new List<KeyValuePair<string, int>>()
                           {
                               new KeyValuePair<string, int>("People?$filter=contains(Name, 'm')", 10),
                               new KeyValuePair<string, int>("People?$filter=contains('m', Name)", 0),
                               new KeyValuePair<string, int>("People?$filter=not contains(Name, 'm')", 3),
                               new KeyValuePair<string, int>("People?$filter=contains(Name, 'm') eq true", 10),
                               new KeyValuePair<string, int>("People?$filter=false eq contains(Name, 'm')", 3),

                               new KeyValuePair<string, int>("People?$filter=contains(Name, substring('name',  2, 1))", 10),
                               new KeyValuePair<string, int>("People?$filter=contains(concat('User','name'), 'name')", 13),
                               new KeyValuePair<string, int>("People?$filter=contains(concat('User','name'), substring('name',  2, 1))", 13),
                           };


            foreach (var test in testlists)
            {
                var result = _context.Execute<Common.Clients.EndToEnd.Person>(new Uri(_baseUri.OriginalString + test.Key)).ToArray();
                Assert.Equal(test.Value, result.Count());
            }
        }

        [Fact]
        public void ContainsLinqTest()
        {
            var result = (from c in _context.People
                          where c.Name.Contains("m")
                          select new Common.Clients.EndToEnd.Person() { Name = c.Name }) as DataServiceQuery<Common.Clients.EndToEnd.Person>;
            Assert.Equal(10, result.Execute().Count());

            result = (DataServiceQuery<Common.Clients.EndToEnd.Person>)_context.People.Where(c => c.Name.Contains("m"));
            Assert.Equal(10, result.Execute().Count());
        }

        [Fact]
        public void PrimitiveTypeInRequestUrlTest()
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
        public void ContainsErrorTest()
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

        [Theory]
        [InlineData("Logins('1')/SentMessages(FromUsername='1',MessageId=-10)", true, -10, "")]
        [InlineData("Logins('1')/SentMessages(MessageId = -10)", false, 0, "BadRequest_KeyMismatch")]
        [InlineData("Logins('1')/SentMessages(-10)", false, 0, "BadRequest_KeyCountMismatch")]
        public void QueryEntityNavigationWithImplicitKeys(
            string endpoint, bool expectSuccess, /* only used on success */ int messageId, /* only used on error */string errorStringIdentifier)
        {
            if (expectSuccess)
            {
                var message = _context.Execute<Message>(new Uri(_baseUri.OriginalString.TrimEnd('/') + "/" + endpoint)).Single();
                Assert.Equal(messageId, message.MessageId);
            }
            else
            {
                try
                {
                    var message = _context.Execute<Message>(new Uri(_baseUri.OriginalString.TrimEnd('/') + "/" + endpoint)).Single();
                    Assert.False(true, "This statement should not have executed because the request should have thrown an exception.");
                }
                catch (DataServiceQueryException ex)
                {
                    Assert.Equal(400, ex.Response.StatusCode);
                    //StringResourceUtil.VerifyODataLibString(ClientExceptionUtil.ExtractServerErrorMessage(ex), errorStringIdentifier, true, "Microsoft.Test.OData.Services.AstoriaDefaultService.Message");
                    //InnerException for DataServiceClientException must be set with the exception response from the server.
                    ODataErrorException oDataErrorException = ex.InnerException.InnerException as ODataErrorException;
                    Assert.True(oDataErrorException != null, "InnerException for DataServiceClientException has not been set.");
                    Assert.Equal("An error was read from the payload. See the 'Error' property for more details.", oDataErrorException.Message);

                }
            }
        }
    }
}

//-----------------------------------------------------------------------------
// <copyright file="ClientDeleteTests.cs" company=".NET Foundation">
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
using Xunit;

namespace Microsoft.OData.Client.E2E.Tests.ClientTests.Tests
{
    public class ClientDeleteTests : EndToEndTestBase<ClientDeleteTests.TestsStartup>
    {
        private readonly Container _context;
        private readonly Uri _baseUri;

        public class TestsStartup : TestStartupBase
        {
            public override void ConfigureServices(IServiceCollection services)
            {
                services.ConfigureControllers(typeof(ClientTestsController), typeof(MetadataController));

                services.AddControllers().AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(null)
                    .AddRouteComponents("odata", DefaultEdmModel.GetEdmModel()));
            }
        }

        public ClientDeleteTests(TestWebApplicationFactory<TestsStartup> fixture)
            : base(fixture)
        {
            _baseUri = new Uri(Client.BaseAddress, "odata/");
            _context = new Container(_baseUri)
            {
                HttpClientFactory = HttpClientFactory,
            };

            ResetDefaultDataSource();
        }

        [Fact]
        public async Task DeletingANavigationProperty_ExecutesSuccessfully()
        {
            var query = _context.People.ByKey(1).Select(p => p.Parent);
            Assert.Equal("http://localhost/odata/People(1)/Parent", query.RequestUri.ToString());
            var response = await _context.ExecuteAsync(query.RequestUri, HttpMethod.Delete.Method);
            Assert.Equal(204, response.StatusCode);
        }

        [Fact]
        public async Task DeletingAnEntity_ExecutesSuccessfully()
        {
            var query = _context.People.ByKey(2);
            Assert.Equal("http://localhost/odata/People(2)", query.RequestUri.ToString());
            var response = await _context.ExecuteAsync(query.RequestUri, HttpMethod.Delete.Method);
            Assert.Equal(204, response.StatusCode);
        }

        private void ResetDefaultDataSource()
        {
            var actionUri = new Uri(_baseUri + "clienttests/Default.ResetDefaultDataSource", UriKind.Absolute);
            _context.Execute(actionUri, "POST");
        }
    }
}

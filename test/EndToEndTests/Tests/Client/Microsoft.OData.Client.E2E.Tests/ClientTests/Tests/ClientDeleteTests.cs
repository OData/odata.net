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
using Microsoft.OData.Client.E2E.Tests.Common.Client.Default.Default;
using Microsoft.OData.Client.E2E.Tests.Common.Server.Default;
using Xunit;

namespace Microsoft.OData.Client.E2E.Tests.ClientTests.Tests
{
    public class ClientDeleteTests : EndToEndTestBase<ClientDeleteTests.TestsStartup>
    {
        private readonly Container _context;

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
            _context = new Container(new Uri(Client.BaseAddress, "odata/"));
            _context.HttpClientFactory = HttpClientFactory;
            _context.KeyComparisonGeneratesFilterQuery = false;
        }

        [Fact]
        public void DeleteMethod_Executes_Successfully()
        {
            DataServiceQuery query = _context.People.Where(p => p.PersonID == 1).Select(p => p.Parent) as DataServiceQuery;
            Assert.Equal("http://localhost/odata/People(1)/Parent", query.ToString());
            var response = _context.Execute(query.RequestUri, HttpMethod.Delete.Method);
            Assert.Equal(204, response.StatusCode);
        }
    }
}

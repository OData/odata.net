//---------------------------------------------------------------------
// <copyright file="ClientUrlConventionsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.TestCommon;
using Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd.Default;
using Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd;
using Microsoft.OData.Client.E2E.Tests.KeyAsSegmentTests.Server;
using Xunit;
using Customer = Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd.Customer;

namespace Microsoft.OData.Client.E2E.Tests.KeyAsSegmentTests.Tests;

public class ClientUrlConventionsTests : EndToEndTestBase<ClientUrlConventionsTests.TestsStartup>
{
    private readonly Uri _baseUri;
    private readonly Container _context;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(KeyAsSegmentTestsController), typeof(MetadataController));

            services.AddControllers().AddOData(opt =>
                opt.EnableQueryFeatures().AddRouteComponents("odata", CommonEndToEndEdmModel.GetEdmModel()));
        }
    }

    public ClientUrlConventionsTests(TestWebApplicationFactory<TestsStartup> fixture) : base(fixture)
    {
        if (Client.BaseAddress == null)
        {
            throw new ArgumentNullException(nameof(Client.BaseAddress), "Base address cannot be null");
        }

        _baseUri = new Uri(Client.BaseAddress, "odata/");

        _context = new Container(_baseUri)
        {
            HttpClientFactory = HttpClientFactory
        };

        ResetDefaultDataSource();
    }

    [Fact]
    public void ClientChangesUrlConventionsBetweenQueries()
    {
        // Arrange & Act & Assert
        var query = _context.CreateQuery<Customer>("Customers").OrderBy(c => c.CustomerId).ToList();
        Assert.Equal(10, query.Count);

        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
        var queryWithDefaultKeys = _context.CreateQuery<Customer>("Customers").OrderBy(c => c.CustomerId).ToList();
        Assert.Equal(10, queryWithDefaultKeys.Count);
        Assert.Equal(query, queryWithDefaultKeys);

        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;
        query = _context.CreateQuery<Customer>("Customers").OrderBy(c => c.CustomerId).ToList();
        Assert.Equal(10, query.Count);
        Assert.Equal(query, queryWithDefaultKeys);
    }

    #region Private methods

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "keyassegmenttests/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    #endregion
}

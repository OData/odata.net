//---------------------------------------------------------------------
// <copyright file="QueryOptionOnCollectionTypePropertyTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common.Client.Default.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.QueryOptions;
using Microsoft.OData.E2E.TestCommon.Logs;
using Microsoft.OData.Edm;
using Xunit;
using ClientDefaultModel = Microsoft.OData.E2E.TestCommon.Common.Client.Default;

namespace Microsoft.OData.Client.E2E.Tests.QueryOptionTests;

public class QueryOptionOnCollectionTypePropertyTests : EndToEndTestBase<QueryOptionOnCollectionTypePropertyTests.TestsStartup>
{
    private readonly Uri _baseUri;
    private readonly Container _context;
    private readonly IEdmModel _model;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(QueryOptionTestsController), typeof(MetadataController));

            services.AddControllers().AddOData(opt =>
                opt.EnableQueryFeatures().AddRouteComponents("odata", DefaultEdmModel.GetEdmModel()));
        }
    }

    public QueryOptionOnCollectionTypePropertyTests(TestWebApplicationFactory<TestsStartup> factory)
        : base(factory)
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

        _model = DefaultEdmModel.GetEdmModel();
        ResetDefaultDataSource();

        // Add the custom TraceListener to log assertion failures
        _ = new LogAssertTraceListener();
    }

    #region Client Tests

    [Fact]
    public void FilterOnCollectionCountTest()
    {
        // Arrange & Act & Assert
        var person = _context.People.Where(p => p.Emails.Count == 2) as DataServiceQuery<ClientDefaultModel.Person>;
        Assert.NotNull(person?.RequestUri?.OriginalString);
        Assert.EndsWith("/People?$filter=Emails/$count eq 2", person.RequestUri.OriginalString);

        var product = _context.Products.Where(p => p.CoverColors.Count == 2) as DataServiceQuery<ClientDefaultModel.Product>;
        Assert.NotNull(product?.RequestUri?.OriginalString);
        Assert.EndsWith("/Products?$filter=CoverColors/$count eq 2", product.RequestUri.OriginalString);

        person = _context.People.Where(p => p.Addresses.Count == 2) as DataServiceQuery<ClientDefaultModel.Person>;
        Assert.NotNull(person?.RequestUri?.OriginalString);
        Assert.EndsWith("/People?$filter=Addresses/$count eq 2", person.RequestUri.OriginalString);

        var customers = _context.Customers.Where(p => p.Orders.Count == 2) as DataServiceQuery<ClientDefaultModel.Customer>;
        Assert.NotNull(customers?.RequestUri?.OriginalString);
        Assert.EndsWith("/Customers?$filter=Orders/$count eq 2", customers.RequestUri.OriginalString);
    }

    [Fact]
    public void OrderbyOnCollectionCountTest()
    {
        // Arrange & Act & Assert
        var people = _context.People.OrderBy(p => p.Emails.Count) as DataServiceQuery<ClientDefaultModel.Person>;
        Assert.NotNull(people?.RequestUri?.OriginalString);
        Assert.EndsWith("/People?$orderby=Emails/$count", people.RequestUri.OriginalString);

        people = _context.People.OrderByDescending(p => p.Emails.Count) as DataServiceQuery<ClientDefaultModel.Person>;
        Assert.NotNull(people?.RequestUri?.OriginalString);
        Assert.EndsWith("/People?$orderby=Emails/$count desc", people.RequestUri.OriginalString);

        var products = _context.Products.OrderBy(p => p.CoverColors.Count) as DataServiceQuery<ClientDefaultModel.Product>;
        Assert.NotNull(products?.RequestUri?.OriginalString);
        Assert.EndsWith("/Products?$orderby=CoverColors/$count", products.RequestUri.OriginalString);

        people = _context.People.OrderBy(p => p.Addresses.Count) as DataServiceQuery<ClientDefaultModel.Person>;
        Assert.NotNull(people?.RequestUri?.OriginalString);
        Assert.EndsWith("/People?$orderby=Addresses/$count", people.RequestUri.OriginalString);

        var customers = _context.Customers.OrderBy(p => p.Orders.Count) as DataServiceQuery<ClientDefaultModel.Customer>;
        Assert.NotNull(customers?.RequestUri?.OriginalString);
        Assert.EndsWith("/Customers?$orderby=Orders/$count", customers.RequestUri.OriginalString);
    }

    #endregion

    #region Private methods

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "queryoption/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    #endregion
}

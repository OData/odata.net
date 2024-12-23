//---------------------------------------------------------------------
// <copyright file="QueryOptionOnCollectionTypePropertyTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.TestCommon;
using Microsoft.OData.Client.E2E.TestCommon.Common;
using Microsoft.OData.Client.E2E.TestCommon.Helpers;
using Microsoft.OData.Client.E2E.TestCommon.Logs;
using Microsoft.OData.Client.E2E.Tests.Common.Client.Default.Default;
using Microsoft.OData.Client.E2E.Tests.Common.Server.Default;
using Microsoft.OData.Client.E2E.Tests.QueryOptionTests.Server;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Client.E2E.Tests.QueryOptionTests.Tests;

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

    public QueryOptionOnCollectionTypePropertyTests(TestWebApplicationFactory<QueryOptionOnCollectionTypePropertyTests.TestsStartup> factory) 
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

    private const string MimeType = MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata;

    #region $skip

    [Fact]
    public async Task SkipQueryOptionTest()
    {
        // Arrange & Act
        var property = await this.TestsHelper.QueryPropertyAsync("Customers(1)/Numbers?$skip=2", MimeType);

        // Assert
        Assert.NotNull(property);

        var collectionValue = property.Value as ODataCollectionValue;
        Assert.NotNull(collectionValue);

        var items = collectionValue.Items.Cast<object>().ToList();
        Assert.Equal(3, items.Count);
        Assert.Equal("310", items[0]);
        Assert.Equal("bca", items[1]);
        Assert.Equal("ayz", items[2]);
    }

    #endregion

    #region $top

    [Fact]
    public async Task TopQueryOptionTest()
    {
        // Arrange & Act
        var property = await this.TestsHelper.QueryPropertyAsync("Customers(1)/Numbers?$top=3", MimeType);

        // Assert
        Assert.NotNull(property);

        var collectionValue = property.Value as ODataCollectionValue;
        Assert.NotNull(collectionValue);

        var items = collectionValue.Items.Cast<object>().ToList();
        Assert.Equal(3, items.Count);
        Assert.Equal("111-111-1111", items[0]);
        Assert.Equal("012", items[1]);
        Assert.Equal("310", items[2]);
    }

    #endregion

    #region Combine Query Options

    [Theory]
    [InlineData("$orderby=$it", new string[] { "012", "111-111-1111", "310", "ayz", "bca" })]
    [InlineData("$orderby=$it&$top=1", new string[] { "012" })]
    [InlineData("$skip=1&$filter=contains($it,'a')", new string[] { "ayz" })]
    [InlineData("$filter=$it eq '012'", new string[] { "012" })]
    public async Task CombineVariousQueryOptionsTest(string queryOptions, string[] collectionItems)
    {
        // Arrange & Act
        var property = await this.TestsHelper.QueryPropertyAsync($"Customers(1)/Numbers?{queryOptions}", MimeType);

        // Assert
        Assert.NotNull(property);

        var collectionValue = property.Value as ODataCollectionValue;
        Assert.NotNull(collectionValue);

        var expectedValue = new ODataCollectionValue()
        {
            TypeName = "Collection(Edm.String)",
            Items = collectionItems
        };

        ODataValueAssertEqualHelper.AssertODataValueEqual(expectedValue, collectionValue);
    }

    #endregion

    #region Client Tests

    [Fact]
    public void FilterOnCollectionCountTest()
    {
        // Arrange & Act & Assert
        var person = _context.People.Where(p => p.Emails.Count == 2) as DataServiceQuery<Common.Client.Default.Person>;
        Assert.NotNull(person?.RequestUri?.OriginalString);
        Assert.EndsWith("/People?$filter=Emails/$count eq 2", person.RequestUri.OriginalString);

        var product = _context.Products.Where(p => p.CoverColors.Count == 2) as DataServiceQuery<Common.Client.Default.Product>;
        Assert.NotNull(product?.RequestUri?.OriginalString);
        Assert.EndsWith("/Products?$filter=CoverColors/$count eq 2", product.RequestUri.OriginalString);

        person = _context.People.Where(p => p.Addresses.Count == 2) as DataServiceQuery<Common.Client.Default.Person>;
        Assert.NotNull(person?.RequestUri?.OriginalString);
        Assert.EndsWith("/People?$filter=Addresses/$count eq 2", person.RequestUri.OriginalString);

        var customers = _context.Customers.Where(p => p.Orders.Count == 2) as DataServiceQuery<Common.Client.Default.Customer>;
        Assert.NotNull(customers?.RequestUri?.OriginalString);
        Assert.EndsWith("/Customers?$filter=Orders/$count eq 2", customers.RequestUri.OriginalString);
    }

    [Fact]
    public void OrderbyOnCollectionCountTest()
    {
        // Arrange & Act & Assert
        var people = _context.People.OrderBy(p => p.Emails.Count) as DataServiceQuery<Common.Client.Default.Person>;
        Assert.NotNull(people?.RequestUri?.OriginalString);
        Assert.EndsWith("/People?$orderby=Emails/$count", people.RequestUri.OriginalString);

        people = _context.People.OrderByDescending(p => p.Emails.Count) as DataServiceQuery<Common.Client.Default.Person>;
        Assert.NotNull(people?.RequestUri?.OriginalString);
        Assert.EndsWith("/People?$orderby=Emails/$count desc", people.RequestUri.OriginalString);

        var products = _context.Products.OrderBy(p => p.CoverColors.Count) as DataServiceQuery<Common.Client.Default.Product>;
        Assert.NotNull(products?.RequestUri?.OriginalString);
        Assert.EndsWith("/Products?$orderby=CoverColors/$count", products.RequestUri.OriginalString);

        people = _context.People.OrderBy(p => p.Addresses.Count) as DataServiceQuery<Common.Client.Default.Person>;
        Assert.NotNull(people?.RequestUri?.OriginalString);
        Assert.EndsWith("/People?$orderby=Addresses/$count", people.RequestUri.OriginalString);

        var customers = _context.Customers.OrderBy(p => p.Orders.Count) as DataServiceQuery<Common.Client.Default.Customer>;
        Assert.NotNull(customers?.RequestUri?.OriginalString);
        Assert.EndsWith("/Customers?$orderby=Orders/$count", customers.RequestUri.OriginalString);
    }

    #endregion

    #region Private methods

    private QueryOptionTestsHelper TestsHelper
    {
        get
        {
            return new QueryOptionTestsHelper(_baseUri, _model, Client);
        }
    }

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "queryoption/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    #endregion
}

//---------------------------------------------------------------------
// <copyright file="QueryOptionOnCollectionTypePropertyTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common;
using Microsoft.OData.E2E.TestCommon.Common.Client.Default.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.QueryOptions;
using Microsoft.OData.E2E.TestCommon.Helpers;
using Microsoft.OData.E2E.TestCommon.Logs;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Core.E2E.Tests.QueryOptionTests;

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

    private const string MimeType = MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata;

    #region $skip

    [Fact]
    public async Task SkipQueryOptionTest()
    {
        // Arrange & Act
        var property = await TestsHelper.QueryPropertyAsync("Customers(1)/Numbers?$skip=2", MimeType);

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
        var property = await TestsHelper.QueryPropertyAsync("Customers(1)/Numbers?$top=3", MimeType);

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
        var property = await TestsHelper.QueryPropertyAsync($"Customers(1)/Numbers?{queryOptions}", MimeType);

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

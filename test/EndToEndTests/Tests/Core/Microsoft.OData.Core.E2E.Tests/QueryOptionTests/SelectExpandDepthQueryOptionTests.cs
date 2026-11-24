//---------------------------------------------------------------------
// <copyright file="SelectExpandDepthQueryOptionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Net;
using System.Text;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.QueryOptions;
using Microsoft.OData.UriParser;

namespace Microsoft.OData.Core.E2E.Tests.QueryOptionTests;

public class SelectExpandDepthQueryOptionTests : EndToEndTestBase<SelectExpandDepthQueryOptionTests.TestsStartup>
{
    private readonly Uri _baseUri;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(QueryOptionTestsController));

            services.AddControllers().AddOData(options =>
                options.EnableQueryFeatures().AddRouteComponents("odata", DefaultEdmModel.GetEdmModel()));
        }
    }

    public SelectExpandDepthQueryOptionTests(TestWebApplicationFactory<TestsStartup> fixture)
        : base(fixture)
    {
        if (Client.BaseAddress == null)
        {
            throw new ArgumentNullException(nameof(Client.BaseAddress), "Base address cannot be null");
        }

        _baseUri = new Uri(Client.BaseAddress, "odata/");
        ResetDefaultDataSource();
    }

    [Fact]
    public async Task NestedSelectBeyondDefaultSelectExpandDepthReturnsBadRequestAndHostRemainsResponsive()
    {
        string query = BuildNestedQuery(
            "$select",
            "$select",
            ODataUriParserSettings.DefaultMaximumSelectExpandDepth + 1);

        using HttpResponseMessage limitedResponse = await Client.GetAsync(new Uri(_baseUri, "Products?" + query));
        string responseBody = await limitedResponse.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, limitedResponse.StatusCode);
        Assert.Contains(Error.Format(SRResources.UriParser_SelectExpandDepthExceeded, "$select", ODataUriParserSettings.DefaultMaximumSelectExpandDepth + 1, ODataUriParserSettings.DefaultMaximumSelectExpandDepth), responseBody);

        using HttpResponseMessage controlResponse = await Client.GetAsync(new Uri(_baseUri, "Products?$select=ProductID"));
        Assert.Equal(HttpStatusCode.OK, controlResponse.StatusCode);
    }

    [Fact]
    public async Task NestedSelectAtDefaultSelectExpandDepthReachesMetadataBindingAndHostRemainsResponsive()
    {
        string query = BuildNestedQuery(
            "$select",
            "$select",
            ODataUriParserSettings.DefaultMaximumSelectExpandDepth);

        using HttpResponseMessage boundaryResponse = await Client.GetAsync(new Uri(_baseUri, "Products?" + query));
        string responseBody = await boundaryResponse.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, boundaryResponse.StatusCode);
        Assert.Contains("Could not find a property named 'Unknown'", responseBody);
        Assert.DoesNotContain(Error.Format(SRResources.UriParser_SelectExpandDepthExceeded, "$select", ODataUriParserSettings.DefaultMaximumSelectExpandDepth + 1, ODataUriParserSettings.DefaultMaximumSelectExpandDepth), responseBody);

        using HttpResponseMessage controlResponse = await Client.GetAsync(new Uri(_baseUri, "Products?$select=ProductID"));
        Assert.Equal(HttpStatusCode.OK, controlResponse.StatusCode);
    }

    [Fact]
    public async Task MixedNestingBeyondDefaultSelectExpandDepthReturnsBadRequestAndHostRemainsResponsive()
    {
        const int expandNestingDepth = 60;
        int selectNestingDepth = ODataUriParserSettings.DefaultMaximumSelectExpandDepth - expandNestingDepth + 1;
        string query = BuildMixedNestedQuery(expandNestingDepth, selectNestingDepth);

        using HttpResponseMessage limitedResponse = await Client.GetAsync(new Uri(_baseUri, "Products?" + query));
        string responseBody = await limitedResponse.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, limitedResponse.StatusCode);
        Assert.Contains(Error.Format(SRResources.UriParser_SelectExpandDepthExceeded, "$select", ODataUriParserSettings.DefaultMaximumSelectExpandDepth + 1, ODataUriParserSettings.DefaultMaximumSelectExpandDepth), responseBody);

        using HttpResponseMessage controlResponse = await Client.GetAsync(new Uri(_baseUri, "Products(5)?$expand=Details"));
        Assert.Equal(HttpStatusCode.OK, controlResponse.StatusCode);
    }

    [Fact]
    public async Task MixedNestingAtDefaultSelectExpandDepthReachesMetadataBindingAndHostRemainsResponsive()
    {
        const int expandNestingDepth = 60;
        int selectNestingDepth = ODataUriParserSettings.DefaultMaximumSelectExpandDepth - expandNestingDepth;
        string query = BuildMixedNestedQuery(expandNestingDepth, selectNestingDepth);

        using HttpResponseMessage boundaryResponse = await Client.GetAsync(new Uri(_baseUri, "Products?" + query));
        string responseBody = await boundaryResponse.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, boundaryResponse.StatusCode);
        Assert.Contains("Could not find a property named 'Unknown'", responseBody);
        Assert.DoesNotContain(Error.Format(SRResources.UriParser_SelectExpandDepthExceeded, "$select", ODataUriParserSettings.DefaultMaximumSelectExpandDepth + 1, ODataUriParserSettings.DefaultMaximumSelectExpandDepth), responseBody);

        using HttpResponseMessage controlResponse = await Client.GetAsync(new Uri(_baseUri, "Products(5)?$expand=Details"));
        Assert.Equal(HttpStatusCode.OK, controlResponse.StatusCode);
    }

    [Fact]
    public async Task NestedExpandBeyondDefaultExpansionDepthReturnsBadRequestAndHostRemainsResponsive()
    {
        string query = BuildNestedQuery(
            "$expand",
            "$expand",
            ODataUriParserSettings.DefaultMaximumSelectExpandDepth + 1);

        using HttpResponseMessage limitedResponse = await Client.GetAsync(new Uri(_baseUri, "Products?" + query));
        string responseBody = await limitedResponse.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, limitedResponse.StatusCode);
        Assert.Contains(Error.Format(SRResources.UriParser_ExpandDepthExceeded, ODataUriParserSettings.DefaultMaximumExpansionDepth + 1, ODataUriParserSettings.DefaultMaximumExpansionDepth), responseBody);

        using HttpResponseMessage controlResponse = await Client.GetAsync(new Uri(_baseUri, "Products(5)?$expand=Details"));
        Assert.Equal(HttpStatusCode.OK, controlResponse.StatusCode);
    }

    [Fact]
    public async Task CyclicExpandAtDefaultExpansionDepthReachesPostParseValidationAndHostRemainsResponsive()
    {
        string query = BuildCyclicExpandQuery(ODataUriParserSettings.DefaultMaximumExpansionDepth - 1);

        using HttpResponseMessage boundaryResponse = await Client.GetAsync(new Uri(_baseUri, "Products?" + query));
        string responseBody = await boundaryResponse.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, boundaryResponse.StatusCode);
        Assert.DoesNotContain(Error.Format(SRResources.UriParser_SelectExpandDepthExceeded, "$expand", ODataUriParserSettings.DefaultMaximumSelectExpandDepth + 1, ODataUriParserSettings.DefaultMaximumSelectExpandDepth), responseBody);

        using HttpResponseMessage controlResponse = await Client.GetAsync(new Uri(_baseUri, "Products(5)?$expand=Details"));
        Assert.Equal(HttpStatusCode.OK, controlResponse.StatusCode);
    }

    [Fact]
    public async Task CyclicExpandBeyondDefaultExpansionDepthReturnsBadRequestAndHostRemainsResponsive()
    {
        string query = BuildCyclicExpandQuery(ODataUriParserSettings.DefaultMaximumSelectExpandDepth + 1);

        using HttpResponseMessage limitedResponse = await Client.GetAsync(new Uri(_baseUri, "Products?" + query));
        string responseBody = await limitedResponse.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, limitedResponse.StatusCode);
        Assert.Contains(Error.Format(SRResources.UriParser_ExpandDepthExceeded, ODataUriParserSettings.DefaultMaximumExpansionDepth + 1, ODataUriParserSettings.DefaultMaximumExpansionDepth), responseBody);

        using HttpResponseMessage controlResponse = await Client.GetAsync(new Uri(_baseUri, "Products(5)?$expand=Details"));
        Assert.Equal(HttpStatusCode.OK, controlResponse.StatusCode);
    }

    private static string BuildNestedQuery(string rootOption, string nestedOption, int nestingDepth)
    {
        const string propertyName = "Unknown";
        StringBuilder query = new StringBuilder(rootOption).Append('=').Append(propertyName);
        for (int level = 0; level < nestingDepth; level++)
        {
            query.Append('(').Append(nestedOption).Append('=').Append(propertyName);
        }

        query.Append(')', nestingDepth);
        return query.ToString();
    }

    private static string BuildMixedNestedQuery(int expandNestingDepth, int selectNestingDepth)
    {
        const string propertyName = "Unknown";
        StringBuilder query = new StringBuilder("$expand=").Append(propertyName);
        for (int level = 0; level < expandNestingDepth; level++)
        {
            query.Append("($expand=").Append(propertyName);
        }

        for (int level = 0; level < selectNestingDepth; level++)
        {
            query.Append("($select=").Append(propertyName);
        }

        query.Append(')', expandNestingDepth + selectNestingDepth);
        return query.ToString();
    }

    private static string BuildCyclicExpandQuery(int nestingDepth)
    {
        StringBuilder query = new StringBuilder("$expand=Details");
        for (int level = 0; level < nestingDepth; level++)
        {
            string navigationProperty = level % 2 == 0 ? "RelatedProduct" : "Details";
            query.Append("($expand=").Append(navigationProperty);
        }

        query.Append(')', nestingDepth);
        return query.ToString();
    }

    private void ResetDefaultDataSource()
    {
        using HttpResponseMessage response = Client.PostAsync(
            new Uri(_baseUri, "queryoption/Default.ResetDefaultDataSource"), content: null).GetAwaiter().GetResult();
        response.EnsureSuccessStatusCode();
    }
}

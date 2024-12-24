//---------------------------------------------------------------------
// <copyright file="SearchQueryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Query.Expressions;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.TestCommon;
using Microsoft.OData.Client.E2E.TestCommon.Common;
using Microsoft.OData.Client.E2E.Tests.Common.Client.Default.Default;
using Microsoft.OData.Client.E2E.Tests.Common.Server.Default;
using Microsoft.OData.Client.E2E.Tests.QueryOptionTests.Server;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Client.E2E.Tests.QueryOptionTests.Tests;

public class SearchQueryTests : EndToEndTestBase<SearchQueryTests.TestsStartup>
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
                opt.EnableQueryFeatures().AddRouteComponents("odata", DefaultEdmModel.GetEdmModel(), services =>
                    services.AddSingleton<ISearchBinder, CustomSearchBinder>()));
        }
    }

    public SearchQueryTests(TestWebApplicationFactory<SearchQueryTests.TestsStartup> fixture)
        : base(fixture)
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
    }

    public static IEnumerable<object[]> MimeTypesData
    {
        get
        {
            yield return new object[] { MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata };
            yield return new object[] { MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata };
            yield return new object[] { MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata };
        }
    }

    #region $search query with 'AND', 'OR', and 'NOT' operators

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task SearchQueryWithAndOrNotAsync(string mimeType)
    {
        List<ODataResource> details = await this.TestsHelper.QueryResourceSetFeedAsync("ProductDetails?$search=(drink OR snack) AND (suger OR sweet) AND NOT \"0\"", mimeType);
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            Assert.Equal(2, details.Count);
        }
    }

    #endregion

    #region $search query with 'NOT' operator

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task SearchQueryWithNotOperatorAsync(string mimeType)
    {
        List<ODataResource> details = await this.TestsHelper.QueryResourceSetFeedAsync("ProductDetails?$search=NOT (drink OR snack)", mimeType);
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            Assert.Empty(details);
        }
    }

    #endregion

    #region $search query with implicit 'AND' operator

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task SearchQueryWithImplicitAndAsync(string mimeType)
    {
        List<ODataResource> details = await this.TestsHelper.QueryResourceSetFeedAsync("ProductDetails?$search=snack sweet", mimeType);
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            Assert.Single(details);

            var descriptionProperty = details.First().Properties.Single(p => p.Name == "Description") as ODataProperty;
            Assert.NotNull(descriptionProperty);
            Assert.Equal("sweet snack", descriptionProperty.Value);
        }
    }

    #endregion

    #region $search query with 'NOT' and 'AND' operators

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task SearchQueryWithNotAndAsync(string mimeType)
    {
        List<ODataResource> details = await this.TestsHelper.QueryResourceSetFeedAsync("ProductDetails?$search=snack NOT sweet", mimeType);
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            Assert.Equal(2, details.Count);
        }
    }

    #endregion

    #region $search query with priority of 'AND', 'NOT', and 'OR' operators

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task SearchQueryWithPriorityOperatorsAsync(string mimeType)
    {
        List<ODataResource> details = await this.TestsHelper.QueryResourceSetFeedAsync("ProductDetails?$search=snack OR drink AND soft AND NOT \"0\"", mimeType);
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            Assert.Equal(4, details.Count);
        }
    }

    #endregion


    #region $search Combined With $filter, and $select Query Options

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task SearchCombinedWithFilterAndSelectQueryOptionsTest(string mimeType)
    {
        List<ODataResource> details = await this.TestsHelper.QueryResourceSetFeedAsync("ProductDetails?$filter=contains(Description,'drink')&$search=suger OR spicy NOT \"0\"&$select=Description", mimeType);
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            Assert.Equal(2, details.Count);
            foreach (var detail in details)
            {
                Assert.Single(detail.Properties);

                var descriptionProperty = detail.Properties.Single(p => p.Name == "Description") as ODataProperty;
                Assert.NotNull(descriptionProperty);

                var description = Assert.IsType<string>(descriptionProperty.Value);
                Assert.Contains("drink", description);
            }
        }
    }

    #endregion

    #region $search query Combined With $orderby and $expand Query Options

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task SearchCombinedWithOrderbyAndExpandQueryOptionsTest(string mimeType)
    {
        List<ODataResource> entries = await this.TestsHelper.QueryResourceSetFeedAsync("ProductDetails?$search=suger OR sweet&$orderby=ProductName&$expand=Reviews", mimeType);
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            Assert.Equal(7, entries.Count);

            var productDetails = entries.FindAll(e => e.Id.AbsoluteUri.Contains("ProductDetails"));
            var productNameProperty = productDetails.First().Properties.Single(p => p.Name == "ProductName") as ODataProperty;
            Assert.NotNull(productNameProperty);
            Assert.Equal("Candy", productNameProperty.Value);

            var reviews = entries.FindAll(e => e.Id.AbsoluteUri.Contains("ProductReviews"));
            Assert.Equal(4, reviews.Count);
        }
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

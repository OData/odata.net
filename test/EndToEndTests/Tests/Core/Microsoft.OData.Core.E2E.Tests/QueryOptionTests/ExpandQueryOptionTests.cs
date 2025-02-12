//---------------------------------------------------------------------
// <copyright file="ExpandQueryOptionTests.cs" company="Microsoft">
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
using Microsoft.OData.Edm;

namespace Microsoft.OData.Core.E2E.Tests.QueryOptionTests;

public class ExpandQueryOptionTests : EndToEndTestBase<ExpandQueryOptionTests.TestsStartup>
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

    public ExpandQueryOptionTests(TestWebApplicationFactory<TestsStartup> fixture)
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

    #region $expand and $top Query Options

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task ExpandWithTopQueryOptionTestAsync(string mimeType)
    {
        // Arrange & Act
        var queryText = "Products(5)?$expand=Details($top=3)";
        var entries = await TestsHelper.QueryResourceEntriesAsync(queryText, mimeType);

        // Assert
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            var details = entries.FindAll(e => e.Id.AbsoluteUri.Contains("ProductDetails"));
            Assert.Equal(3, details.Count);
        }
    }

    #endregion

    #region $expand and $skip Query Options

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task ExpandWithSkipQueryOptionTestAsync(string mimeType)
    {
        // Arrange & Act
        var queryText = "Products(5)?$expand=Details($skip=2)";
        var entries = await TestsHelper.QueryResourceEntriesAsync(queryText, mimeType);

        // Assert
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            var details = entries.FindAll(e => e.Id.AbsoluteUri.Contains("ProductDetails"));
            Assert.Equal(3, details.Count);
        }
    }

    #endregion

    #region $expand and $orderby Query Options

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task ExpandWithOrderByQueryOptionTestAsync(string mimeType)
    {
        // Arrange & Act
        var queryText = "Products(5)?$expand=Details($orderby=Description desc)";
        var entries = await TestsHelper.QueryResourceEntriesAsync(queryText, mimeType);

        // Assert
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            var details = entries.FindAll(e => e.Id.AbsoluteUri.Contains("ProductDetails"));
            var productIDProperty = details.First().Properties.Single(p => p.Name == "ProductDetailID") as ODataProperty;
            var descriptionProperty = details.First().Properties.Single(p => p.Name == "Description") as ODataProperty;
            var productNameProperty = details.First().Properties.Single(p => p.Name == "ProductName") as ODataProperty;

            Assert.NotNull(productIDProperty);
            Assert.NotNull(descriptionProperty);
            Assert.NotNull(productNameProperty);

            Assert.Equal(3, productIDProperty.Value);
            Assert.Equal("suger soft drink", descriptionProperty.Value);
            Assert.Equal("CokeCola", productNameProperty.Value);
        }
    }

    #endregion

    #region $expand and $filter Query Options

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task ExpandWithFilterQueryOptionTestAsync(string mimeType)
    {
        // Arrange & Act
        var queryText = "Products(5)?$expand=Details($filter=Description eq 'spicy snack')";
        var entries = await TestsHelper.QueryResourceEntriesAsync(queryText, mimeType);

        // Assert
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            var details = entries.FindAll(e => e.Id.AbsoluteUri.Contains("ProductDetails"));
            var productIDProperty = details.First().Properties.Single(p => p.Name == "ProductDetailID") as ODataProperty;
            var productNameProperty = details.First().Properties.Single(p => p.Name == "ProductName") as ODataProperty;

            Assert.NotNull(productIDProperty);
            Assert.NotNull(productNameProperty);

            Assert.Equal(5, productIDProperty.Value);
            Assert.Equal("Mustard", productNameProperty.Value);
        }
    }

    #endregion

    #region $expand and $count Query Options

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task ExpandWithCountQueryOptionTestAsync(string mimeType)
    {
        // Arrange & Act
        var queryText = "Products(5)?$expand=Details($count=true)";
        var feed = await TestsHelper.QueryInnerFeedAsync(queryText, mimeType);

        // Assert
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata) && !mimeType.Contains(MimeTypes.ApplicationAtomXml))
        {
            Assert.NotNull(feed);
            Assert.Equal(5, feed.Count);
        }
    }

    #endregion

    #region $expand with $ref option

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task ExpandWithRefQueryOptionTestAsync(string mimeType)
    {
        // Arrange & Act
        var queryText = "Products(5)?$expand=Details/$ref";
        var entries = await TestsHelper.QueryResourceEntriesAsync(queryText, mimeType);

        // Assert
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            var details = entries.FindAll(e => e.Id.AbsoluteUri.Contains("ProductDetails"));

            Assert.Equal(5, details.Count);
            Assert.Contains("ProductDetailID=2", entries.First().Id.ToString());
        }
    }

    #endregion

    #region $expand with Nested option on $ref

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task ExpandWithNestedRefQueryOptionTestAsync(string mimeType)
    {
        // Arrange & Act
        var queryText = "Products(5)?$expand=Details/$ref($orderby=Description desc)";
        var entries = await TestsHelper.QueryResourceEntriesAsync(queryText, mimeType);

        // Assert
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            var details = entries.FindAll(e => e.Id.AbsoluteUri.Contains("ProductDetails"));

            Assert.Equal(5, details.Count);
            Assert.Contains("ProductDetailID=3", entries.First().Id.ToString());
        }
    }

    #endregion

    #region $expand query option with $orderby, $skip, $top, and $select nested options.

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task ExpandWithOrderBySkipTopSelectQueryOptionTestAsync(string mimeType)
    {
        // Arrange & Act
        var queryText = "Products(5)?$expand=Details($orderby=Description;$skip=2;$top=1)";
        var entries = await TestsHelper.QueryResourceEntriesAsync(queryText, mimeType);

        // Assert
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            var details = entries.FindAll(e => e.Id.AbsoluteUri.Contains("ProductDetails"));
            Assert.Single(details);

            var descriptionProperty = details.First().Properties.Single(p => p.Name == "Description") as ODataProperty;
            Assert.NotNull(descriptionProperty);
            Assert.Equal("fitness drink!", descriptionProperty.Value);
        }
    }

    #endregion

    #region $expand query option with nested $expand, $filter, and $select options.

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task ExpandWithNestedExpandQueryOptionTestAsync(string mimeType)
    {
        // Arrange & Act
        var queryText = "Products(5)?$expand=Details($expand=Reviews($filter=contains(Comment,'good');$select=Comment))";
        if (mimeType.Contains(MimeTypes.ODataParameterMinimalMetadata))
        {
            queryText = "Products(5)?$expand=Details($expand=Reviews($filter=contains(Comment,'good')))";
        }

        var entries = await TestsHelper.QueryResourceEntriesAsync(queryText, mimeType);

        // Assert
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            Assert.Equal(8, entries.Count);
            var reviews = entries.FindAll(e => e.Id.AbsoluteUri.Contains("ProductReviews"));
            Assert.Equal(2, reviews.Count);

            var commentProperty = reviews.First().Properties.Single(p => p.Name == "Comment") as ODataProperty;
            Assert.NotNull(commentProperty);
            Assert.Equal("Not so good as other brands", commentProperty.Value);
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

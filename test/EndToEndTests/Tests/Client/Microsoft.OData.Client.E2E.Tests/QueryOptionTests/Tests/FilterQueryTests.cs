//---------------------------------------------------------------------
// <copyright file="FilterQueryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
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

public class FilterQueryTests : EndToEndTestBase<FilterQueryTests.TestsStartup>
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

    public FilterQueryTests(TestWebApplicationFactory<FilterQueryTests.TestsStartup> fixture) : base(fixture)
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

    #region $filter with $count collection of primitive type

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task FilterWithCountCollectionOfPrimitiveTypeAsync(string mimeType)
    {
        // Arrange & Act
        var entries = await this.TestsHelper.QueryResourceSetFeedAsync("People?$filter=Emails/$count lt 2", mimeType);
        var details = entries.Where(r => r != null && r.Id != null).ToList();

        // Assert
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            Assert.Equal(4, details.Count);
        }
    }

    #endregion

    #region $filter with $count collection of enum type

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task FilterWithCountCollectionOfEnumTypeAsync(string mimeType)
    {
        // Arrange & Act
        var entries = await this.TestsHelper.QueryResourceSetFeedAsync("Products?$filter=CoverColors/$count lt 2", mimeType);
        var details = entries.Where(r => r != null && r.Id != null).ToList();

        // Assert
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            Assert.Single(details);
        }
    }

    #endregion

    #region $filter with $count collection of complex type

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task FilterWithCountCollectionOfComplexTypeAsync(string mimeType)
    {
        // Arrange & Act
        var entries = await this.TestsHelper.QueryResourceSetFeedAsync("People?$filter=Addresses/$count eq 2", mimeType);
        var details = entries.Where(r => r != null && r.Id != null).ToList();

        // Assert
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            Assert.Equal(2, details.Count);
        }
    }

    #endregion

    #region $filter with $count collection of entity type

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task FilterWithCountCollectionOfEntityTypeAsync(string mimeType)
    {
        // Arrange & Act
        var entries = await this.TestsHelper.QueryResourceSetFeedAsync("Customers?$filter=Orders/$count lt 2", mimeType);
        var details = entries.Where(r => r != null && r.Id != null).ToList();

        // Assert
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            Assert.Single(details);
        }
    }

    #endregion

    public static IEnumerable<object[]> MimeTypesData
    {
        get
        {
            yield return new object[] { MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata };
            yield return new object[] { MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata };
            yield return new object[] { MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata };
        }
    }

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

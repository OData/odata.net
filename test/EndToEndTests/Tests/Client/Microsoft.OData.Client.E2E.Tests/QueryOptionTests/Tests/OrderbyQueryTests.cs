//---------------------------------------------------------------------
// <copyright file="OrderbyQueryTests.cs" company="Microsoft">
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

public class OrderbyQueryTests : EndToEndTestBase<OrderbyQueryTests.TestsStartup>
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

    public OrderbyQueryTests(TestWebApplicationFactory<OrderbyQueryTests.TestsStartup> fixture) 
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

    #region $orderby with $count collection of primitive type

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task OrderbyWithCountCollectionOfPrimitiveTypeAsync(string mimeType)
    {
        // Arrange & Act
        Func<List<ODataResource>, List<ODataResource>> getEntries = (res) => res.Where(r => r != null &&
            (r.TypeName.Contains("Person") || r.TypeName.Contains("Customer") || r.TypeName.Contains("Product") || r.TypeName.Contains("Employee"))).ToList();

        var resources = await TestsHelper.QueryResourceSetFeedAsync("People?$orderby=Emails/$count", mimeType);
        var details = getEntries(resources);

        // Assert
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            var firstNameProperty = details.First().Properties.Single(p => p.Name == "FirstName") as ODataProperty;
            Assert.NotNull(firstNameProperty);
            Assert.Equal("Jill", firstNameProperty.Value);
        }
    }

    #endregion

    #region $orderby with $count collection of primitive type, descending

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task OrderbyWithCountCollectionOfPrimitiveTypeDescTypeAsync(string mimeType)
    {
        // Arrange & Act
        Func<List<ODataResource>, List<ODataResource>> getEntries = (res) => res.Where(r => r != null &&
            (r.TypeName.Contains("Person") || r.TypeName.Contains("Customer") || r.TypeName.Contains("Product") || r.TypeName.Contains("Employee"))).ToList();

        var resources = await TestsHelper.QueryResourceSetFeedAsync("People?$orderby=Emails/$count desc", mimeType);
        var details = getEntries(resources);

        // Assert
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            var firstNameProperty = details.First().Properties.Single(p => p.Name == "FirstName") as ODataProperty;
            Assert.NotNull(firstNameProperty);
            Assert.Equal("Elmo", firstNameProperty.Value);
        }
    }

    #endregion

    #region $orderby with $count collection of enum type

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task OrderbyWithCountCollectionOfEnumTypeAsync(string mimeType)
    {
        // Arrange & Act
        Func<List<ODataResource>, List<ODataResource>> getEntries = (res) => res.Where(r => r != null &&
            (r.TypeName.Contains("Person") || r.TypeName.Contains("Customer") || r.TypeName.Contains("Product") || r.TypeName.Contains("Employee"))).ToList();

        var resources = await TestsHelper.QueryResourceSetFeedAsync("Products?$orderby=CoverColors/$count", mimeType);
        var details = getEntries(resources);

        // Assert
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            var nameProperty = details.First().Properties.Single(p => p.Name == "Name") as ODataProperty;
            Assert.NotNull(nameProperty);
            Assert.Equal("Apple", nameProperty.Value);
        }
    }

    #endregion

    #region $orderby with $count collection of complex type

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task OrderbyWithCountCollectionOfComplexTypeAsync(string mimeType)
    {
        // Arrange & Act
        Func<List<ODataResource>, List<ODataResource>> getEntries = (res) => res.Where(r => r != null &&
            (r.TypeName.Contains("Person") || r.TypeName.Contains("Customer") || r.TypeName.Contains("Product") || r.TypeName.Contains("Employee"))).ToList();

        var resources = await TestsHelper.QueryResourceSetFeedAsync("People?$orderby=Addresses/$count", mimeType);
        var details = getEntries(resources);

        // Assert
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            var firstNameProperty = details.First().Properties.Single(p => p.Name == "FirstName") as ODataProperty;
            Assert.NotNull(firstNameProperty);
            Assert.Equal("Jill", firstNameProperty.Value);
        }
    }

    #endregion

    #region $orderby with $count collection of entity type

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task OrderbyWithCountCollectionOfEntityTypeAsync(string mimeType)
    {
        // Arrange & Act
        Func<List<ODataResource>, List<ODataResource>> getEntries = (res) => res.Where(r => r != null &&
            (r.TypeName.Contains("Person") || r.TypeName.Contains("Customer") || r.TypeName.Contains("Product") || r.TypeName.Contains("Employee"))).ToList();

        var resources = await TestsHelper.QueryResourceSetFeedAsync("Customers?$orderby=Orders/$count", mimeType);
        var details = getEntries(resources);

        // Assert
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            var firstNameProperty = details.First().Properties.Single(p => p.Name == "FirstName") as ODataProperty;
            Assert.NotNull(firstNameProperty);
            Assert.Equal("Bob", firstNameProperty.Value);
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

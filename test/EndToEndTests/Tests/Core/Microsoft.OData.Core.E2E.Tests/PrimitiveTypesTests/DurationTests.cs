//---------------------------------------------------------------------
// <copyright file="DurationTests.cs" company="Microsoft">
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
using Microsoft.OData.E2E.TestCommon.Common.Server.PrimitiveTypes;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Core.E2E.Tests.PrimitiveTypesTests;

public class DurationTests : EndToEndTestBase<DurationTests.TestsStartup>
{
    private readonly Uri _baseUri;
    private readonly Container _context;
    private readonly IEdmModel _model;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(PrimitiveTypesTestsController), typeof(MetadataController));

            services.AddControllers().AddOData(opt =>
                opt.EnableQueryFeatures().AddRouteComponents("odata", DefaultEdmModel.GetEdmModel()));
        }
    }

    public DurationTests(TestWebApplicationFactory<TestsStartup> fixture)
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

    // Constants
    private const string MimeTypeODataParameterFullMetadata = MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata;
    private const string MimeTypeODataParameterMinimalMetadata = MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata;

    #region Entity Set

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task QueryEntitySetTest(string mimeType)
    {
        // Arrange & Act
        var queryText = "Customers";
        var entries = await TestsHelper.QueryResourceSetsAsync(queryText, mimeType);

        entries = entries.Where(e => e.TypeName.EndsWith("Customer")).ToList();

        // Assert
        var timeBetweenLastTwoOrdersProperty = entries[0].Properties.Single(p => p.Name == "TimeBetweenLastTwoOrders") as ODataProperty;
        Assert.NotNull(timeBetweenLastTwoOrdersProperty);
        Assert.Equal(new TimeSpan(1), timeBetweenLastTwoOrdersProperty.Value);

        timeBetweenLastTwoOrdersProperty = entries[1].Properties.Single(p => p.Name == "TimeBetweenLastTwoOrders") as ODataProperty;
        Assert.NotNull(timeBetweenLastTwoOrdersProperty);
        Assert.Equal(new TimeSpan(2), timeBetweenLastTwoOrdersProperty.Value);
    }

    #endregion

    #region Entity instance

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task QueryEntityInstanceTest(string mimeType)
    {
        // Arrange & Act
        var queryText = "Customers(1)";
        var entries = await TestsHelper.QueryResourceEntriesAsync(queryText, mimeType);

        var entry = entries.SingleOrDefault(e => e.TypeName.EndsWith("Customer"));

        // Assert
        Assert.NotNull(entry);

        var timeBetweenLastTwoOrdersProperty = entry.Properties.Single(p => p.Name == "TimeBetweenLastTwoOrders") as ODataProperty;
        Assert.NotNull(timeBetweenLastTwoOrdersProperty);
        Assert.NotNull(timeBetweenLastTwoOrdersProperty.Value);
        Assert.Equal(new TimeSpan(1), timeBetweenLastTwoOrdersProperty.Value);
    }

    #endregion

    #region Entity property

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task QueryEntityPropertyTest(string mimeType)
    {
        // Arrange & Act
        var queryText = "Customers(1)/TimeBetweenLastTwoOrders";
        var property = await TestsHelper.QueryPropertyAsync(queryText, mimeType);

        // Assert
        Assert.NotNull(property);
        Assert.Equal(new TimeSpan(1), property.Value);
    }

    #endregion

    #region Entity navigation 

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task QueryEntityNavigationTest(string mimeType)
    {
        // Arrange & Act
        var queryText = "Orders(7)/CustomerForOrder";
        var entries = await TestsHelper.QueryResourceEntriesAsync(queryText, mimeType);

        var entry = entries.SingleOrDefault(e => e.TypeName.EndsWith("Customer"));

        // Assert
        Assert.NotNull(entry);

        var timeBetweenLastTwoOrdersProperty = entry.Properties.Single(p => p.Name == "TimeBetweenLastTwoOrders") as ODataProperty;
        Assert.NotNull(timeBetweenLastTwoOrdersProperty);
        Assert.Equal(new TimeSpan(2), timeBetweenLastTwoOrdersProperty.Value);
    }

    #endregion

    #region Entity property $value

    [Fact]
    public async Task QueryEntityPropertyValueTest()
    {
        // Arrange & Act
        var queryText = "Customers(1)/TimeBetweenLastTwoOrders/$value";
        var propertyValue = await TestsHelper.QueryPropertyValueInStringAsync(queryText);

        // Assert
        Assert.Equal("PT0.0000001S", propertyValue);
    }

    #endregion

    #region Private methods

    private PrimitiveTypesTestsHelper TestsHelper
    {
        get
        {
            return new PrimitiveTypesTestsHelper(_baseUri, _model, Client);
        }
    }

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "primitivetypes/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    #endregion
}

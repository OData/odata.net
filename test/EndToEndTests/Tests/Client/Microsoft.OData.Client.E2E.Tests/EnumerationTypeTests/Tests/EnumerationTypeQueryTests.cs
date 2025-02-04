//---------------------------------------------------------------------
// <copyright file="EnumerationTypeQueryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.TestCommon;
using Microsoft.OData.Client.E2E.TestCommon.Common;
using Microsoft.OData.Client.E2E.Tests.Common.Client.Default;
using Microsoft.OData.Client.E2E.Tests.Common.Client.Default.Default;
using Microsoft.OData.Client.E2E.Tests.Common.Server.Default;
using Microsoft.OData.Client.E2E.Tests.EnumerationTypeTests.Server;
using Microsoft.OData.Edm;
using Xunit;
using AccessLevel = Microsoft.OData.Client.E2E.Tests.Common.Client.Default.AccessLevel;
using Color = Microsoft.OData.Client.E2E.Tests.Common.Client.Default.Color;
using Product = Microsoft.OData.Client.E2E.Tests.Common.Client.Default.Product;

namespace Microsoft.OData.Client.E2E.Tests.EnumerationTypeTests.Tests;

/// <summary>
/// Contains end-to-end tests for querying enumeration type properties.
/// </summary>
public class EnumerationTypeQueryTests : EndToEndTestBase<EnumerationTypeQueryTests.TestsStartup>
{
    private readonly Uri _baseUri;
    private readonly Container _context;
    private readonly IEdmModel _model;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(EnumerationTypeTestsController), typeof(MetadataController));

            services.AddControllers().AddOData(opt =>
                opt.EnableQueryFeatures().AddRouteComponents("odata", DefaultEdmModel.GetEdmModel()));
        }
    }

    public EnumerationTypeQueryTests(TestWebApplicationFactory<TestsStartup> fixture)
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

    private static string NameSpacePrefix = typeof(DefaultEdmModel).Namespace ?? "Microsoft.OData.Client.E2E.Tests.Common.Server.Default";

    // Constants
    private const string MimeTypeODataParameterFullMetadata = MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata;
    private const string MimeTypeODataParameterMinimalMetadata = MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata;
    private const string MimeTypeApplicationAtomXml = MimeTypes.ApplicationAtomXml;

    #region Tests querying entity set and verifies the enumeration type properties

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    [InlineData(MimeTypeApplicationAtomXml)]
    public async Task QueryEntitySetAndVerifyEnumProperties(string mimeType)
    {
        // Arrange & Act
        List<ODataResource> entries = await TestsHelper.QueryResourceSetsAsync("Products", mimeType);

        // Assert
        Assert.Equal(5, entries.Count);

        var skinColorProperty = entries[1].Properties.Single(p => p.Name == "SkinColor") as ODataProperty;
        Assert.NotNull(skinColorProperty);
        var skinColor = Assert.IsType<ODataEnumValue>(skinColorProperty.Value);
        Assert.Equal("Blue", skinColor.Value);

        var userAccessProperty = entries[1].Properties.Single(p => p.Name == "UserAccess") as ODataProperty;
        Assert.NotNull(userAccessProperty);
        var userAccess = Assert.IsType<ODataEnumValue>(userAccessProperty.Value);
        Assert.Equal("ReadWrite", userAccess.Value);
    }

    #endregion

    #region Tests querying an entity and verifies the enumeration type properties

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    [InlineData(MimeTypeApplicationAtomXml)]
    public async Task QuerySpecificEntityAndVerifyEnumProperties(string mimeType)
    {
        // Arrange & Act
        List<ODataResource> entries = await TestsHelper.QueryResourceEntriesAsync("Products(6)", mimeType);
        var entry = entries.Single();

        // Assert
        Assert.Single(entries);

        var skinColorProperty = entry.Properties.Single(p => p.Name == "SkinColor") as ODataProperty;
        Assert.NotNull(skinColorProperty);
        var skinColor = Assert.IsType<ODataEnumValue>(skinColorProperty.Value);
        Assert.Equal("Blue", skinColor.Value);

        var userAccessProperty = entry.Properties.Single(p => p.Name == "UserAccess") as ODataProperty;
        Assert.NotNull(userAccessProperty);
        var userAccess = Assert.IsType<ODataEnumValue>(userAccessProperty.Value);
        Assert.Equal("ReadWrite", userAccess.Value);
    }

    #endregion

    #region Tests querying an Enum property of an entity and verifies its value.

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    [InlineData(MimeTypeApplicationAtomXml)]
    public async Task QueryEnumPropertyAndVerifyValue(string mimeType)
    {
        // Arrange & Act
        var skinColorProperty = await TestsHelper.QueryPropertyAsync("Products(5)/SkinColor", mimeType);

        // Assert
        Assert.NotNull(skinColorProperty);
        var skinColor = Assert.IsType<ODataEnumValue>(skinColorProperty.Value);
        Assert.Equal("Red", skinColor.Value);
    }

    #endregion

    #region Tests querying the value of an enum property of an entity and verifies its value.

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    [InlineData(MimeTypeApplicationAtomXml)]
    public async Task QueryEnumPropertyValueAndVerifyValue(string mimeType)
    {
        // Arrange
        var readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri };

        var uri = new Uri(_baseUri.AbsoluteUri + "Products(5)/SkinColor/$value", UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(uri, Client);
        requestMessage.SetHeader("Accept", mimeType);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
        {
            var skinColorPropertyValue = messageReader.ReadValue(EdmCoreModel.Instance.GetString(false));
            Assert.Equal("Red", skinColorPropertyValue);
        }
    }

    #endregion

    #region Tests invoking an action with an enumeration type parameter and verifies the returned enumeration type value.

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task InvokeActionWithEnumParameterAndVerifyReturnValue(string mimeType)
    {
        // Arrange
        var writerSettings = new ODataMessageWriterSettings
        {
            BaseUri = _baseUri,
            EnableMessageStreamDisposal = false, // Ensure the stream is not disposed of prematurely
        };

        var readerSettings = new ODataMessageReaderSettings
        {
            BaseUri = _baseUri
        };

        var requestUri = new Uri(_baseUri + "Products(5)/Default.AddAccessRight", UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(requestUri, Client);
        requestMessage.SetHeader("Content-Type", mimeType);
        requestMessage.SetHeader("Accept", "*/*");
        requestMessage.Method = "POST";
        var accessRight = new ODataEnumValue("Read,Execute", $"{NameSpacePrefix}.AccessLevel");
        using (var messageWriter = new ODataMessageWriter(requestMessage, writerSettings, _model))
        {
            var odataWriter = messageWriter.CreateODataParameterWriter((IEdmOperation)null);
            odataWriter.WriteStart();
            odataWriter.WriteValue("accessRight", accessRight);
            odataWriter.WriteEnd();
        }

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
        {
            var userAccessPropertyValue = messageReader.ReadProperty();
            var enumValue = userAccessPropertyValue.Value as ODataEnumValue;
            Assert.Equal("Read, Execute", enumValue?.Value);
        }
    }

    #endregion

    #region Tests calling an unbound function that returns an enumeration type value and verifies the returned value.

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    [InlineData(MimeTypeApplicationAtomXml)]
    public async Task CallUnboundFunctionAndVerifyEnumReturnValue(string mimeType)
    {
        // Arrange
        var readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri };
        var requestUri = new Uri(_baseUri + "GetDefaultColor()", UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(requestUri, Client);
        requestMessage.SetHeader("Accept", mimeType);
        if (mimeType == MimeTypes.ApplicationAtomXml)
        {
            requestMessage.SetHeader("Accept", "text/html, application/xhtml+xml, */*");
        }

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
        {
            ODataProperty colorProperty = messageReader.ReadProperty();
            var enumValue = colorProperty.Value as ODataEnumValue;
            Assert.Equal("Red", enumValue?.Value);
        }
    }

    #endregion

    #region Tests querying entity set with a filter on an enumeration type property and verifies the results.

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    [InlineData(MimeTypeApplicationAtomXml)]
    public async Task QueryEntitiesWithEnumFilterAndVerifyResults(string mimeType)
    {
        // Arrange
        var queryText = "Products?$filter=UserAccess has Microsoft.OData.Client.E2E.Tests.Common.Server.Default.AccessLevel'Read'";

        // Act
        List<ODataResource> entries = await TestsHelper.QueryResourceSetsAsync(queryText, mimeType);

        // Assert
        Assert.Equal(3, entries.Count);

        var entity0 = entries[0].Properties.ToDictionary(p => p.Name, p => p as ODataProperty);
        Assert.Equal(6, entity0["ProductID"]?.Value);
        Assert.Equal("Mushrooms", entity0["Name"]?.Value);
        Assert.Equal("Blue", (entity0["SkinColor"]?.Value as ODataEnumValue)?.Value);
        Assert.Equal("ReadWrite", (entity0["UserAccess"]?.Value as ODataEnumValue)?.Value);

        var entity1 = entries[1].Properties.ToDictionary(p => p.Name, p => p as ODataProperty);
        Assert.Equal(7, entity1["ProductID"]?.Value);
        Assert.Equal("Apple", entity1["Name"]?.Value);
        Assert.Equal("Red", (entity1["SkinColor"]?.Value as ODataEnumValue)?.Value);
        Assert.Equal("Read", (entity1["UserAccess"]?.Value as ODataEnumValue)?.Value);

        var entity2 = entries[2].Properties.ToDictionary(p => p.Name, p => p as ODataProperty);
        Assert.Equal(9, entity2["ProductID"]?.Value);
        Assert.Equal("Computer", entity2["Name"]?.Value);
        Assert.Equal("Green", (entity2["SkinColor"]?.Value as ODataEnumValue)?.Value);
        Assert.Equal("Read", (entity2["UserAccess"]?.Value as ODataEnumValue)?.Value);
    }

    #endregion

    #region Tests querying entity set ordered by an enumeration type property and verifies the results.

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    [InlineData(MimeTypeApplicationAtomXml)]
    public async Task QueryEntitiesOrderedByEnumPropertyAndVerifyResults(string mimeType)
    {
        // Arrange
        var queryText = "Products?$orderby=SkinColor";

        // Act
        List<ODataResource> entries = await TestsHelper.QueryResourceSetsAsync(queryText, mimeType);

        // Assert
        Assert.Equal(5, entries.Count);

        Assert.Equal(5, (entries[0].Properties.Single(p => p.Name == "ProductID") as ODataProperty)?.Value);
        Assert.Equal(7, (entries[1].Properties.Single(p => p.Name == "ProductID") as ODataProperty)?.Value);
        Assert.Equal(8, (entries[2].Properties.Single(p => p.Name == "ProductID") as ODataProperty)?.Value);
        Assert.Equal(9, (entries[3].Properties.Single(p => p.Name == "ProductID") as ODataProperty)?.Value);
        Assert.Equal(6, (entries[4].Properties.Single(p => p.Name == "ProductID") as ODataProperty)?.Value);
    }

    #endregion


    #region Tests querying entity set and selecting only enumeration type properties.

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    [InlineData(MimeTypeApplicationAtomXml)]
    public async Task QueryEntitiesAndSelectEnumProperties(string mimeType)
    {
        // Arrange
        var queryText = "Products?$select=SkinColor,UserAccess";

        // Act
        List<ODataResource> entries = await TestsHelper.QueryResourceSetsAsync(queryText, mimeType);

        // Assert
        Assert.Equal(5, entries.Count);

        Assert.DoesNotContain(entries[0].Properties, p => p.Name != "SkinColor" && p.Name != "UserAccess");
        Assert.All(entries[0].Properties, p => Assert.Contains(p.Name, new[] { "SkinColor", "UserAccess" }));
    }

    #endregion

    #region client operations.

    [Fact]
    public void QueryEntitySetUsingODataClientAndVerifyEnumProperties()
    {
        // Arrange
        _context.Format.UseJson(_model);

        // Act & Assert
        var queryable = _context.Products;
        Assert.EndsWith("/Products", queryable.RequestUri.OriginalString, StringComparison.Ordinal);

        List<Product> result = queryable.ToList();
        Assert.Equal(5, result.Count);

        Assert.Equal(Color.Blue, result[1].SkinColor);
        Assert.Equal(AccessLevel.ReadWrite, result[1].UserAccess);
    }

    [Fact]
    public void QueryEntityUsingODataClientAndVerifyEnumProperties()
    {
        // Arrange
        _context.Format.UseJson(_model);

        // Act & Assert
        var queryable = _context.Products.ByKey(8) as DataServiceQuerySingle<Product>;
        Assert.EndsWith("/Products(8)", queryable?.RequestUri.OriginalString, StringComparison.Ordinal);

        Assert.NotNull(queryable);
        var result = queryable.GetValue();
        Assert.NotNull(result);
        Assert.Equal(Color.Red, result.SkinColor);
        Assert.Equal(AccessLevel.Execute, result.UserAccess);
    }

    [Fact]
    public void QueryEnumPropertyUsingODataClientAndVerifyValue()
    {
        // Arrange
        _context.Format.UseJson(_model);

        // Act & Assert
        var userAccess = _context.Execute<AccessLevel>(new Uri(_baseUri.AbsoluteUri + "Products(8)/UserAccess"));
        List<AccessLevel> enumResult = userAccess.ToList();
        Assert.Single(enumResult);
        Assert.Equal(AccessLevel.Execute, enumResult[0]);
    }

    [Fact]
    public void QueryEnumCollectionPropertyUsingODataClientAndVerifyValues()
    {
        // Arrange
        _context.Format.UseJson(_model);

        // Act & Assert
        var resultTmp = _context.Execute<List<Color>>(new Uri(_baseUri.AbsoluteUri + "Products(5)/CoverColors"));
        List<List<Color>> enumResult = resultTmp.ToList();
        Assert.Single(enumResult);
        Assert.Equal(Color.Green, enumResult[0][0]);
        Assert.Equal(Color.Blue, enumResult[0][1]);
        Assert.Equal(Color.Blue, enumResult[0][2]);
    }

    [Fact]
    public void QueryEntitiesWithQueryOptionsUsingODataClientAndVerifyResults()
    {
        // MinimalMetadata: UseJson() + no $select in request uri
        _context.Format.UseJson(_model);
        var queryable = _context.CreateQuery<Product>("Products")
            .AddQueryOption("$filter", "SkinColor eq Microsoft.OData.Client.E2E.Tests.Common.Server.Default.Color'Red'");
        Assert.EndsWith("/Products?$filter=SkinColor eq Microsoft.OData.Client.E2E.Tests.Common.Server.Default.Color'Red'", queryable.RequestUri.OriginalString, StringComparison.Ordinal);
        
        List<Product> result = queryable.ToList();
        Assert.Equal(result.Select(s => s.ProductID).Distinct().Count(), result.Count);
        Assert.True(result.All(s => s.SkinColor == Color.Red));
        Assert.NotNull(result.SingleOrDefault(r => r.ProductID == 5 && r.UserAccess == AccessLevel.None));
        Assert.NotNull(result.SingleOrDefault(r => r.ProductID == 7 && r.UserAccess == AccessLevel.Read));
        Assert.NotNull(result.SingleOrDefault(r => r.ProductID == 8 && r.UserAccess == AccessLevel.Execute));

        // FullMetadata: UseJson() + have $select in request uri
        _context.Format.UseJson(_model);
        queryable = _context.CreateQuery<Product>("Products")
            .AddQueryOption("$filter", "SkinColor eq Microsoft.OData.Client.E2E.Tests.Common.Server.Default.Color'Red'")
            .AddQueryOption("$select", "SkinColor,ProductID,Name");

        Assert.EndsWith("/Products?$filter=SkinColor eq Microsoft.OData.Client.E2E.Tests.Common.Server.Default.Color'Red'&$select=SkinColor,ProductID,Name", queryable.RequestUri.OriginalString, StringComparison.Ordinal);
        result = queryable.ToList();
        Assert.Equal(result.Select(s => s.ProductID).Distinct().Count(), result.Count);
        Assert.True(result.All(s => s.SkinColor == Color.Red));
        Assert.NotNull(result.SingleOrDefault(r => r.ProductID == 5 && r.UserAccess == AccessLevel.None));
        Assert.NotNull(result.SingleOrDefault(r => r.ProductID == 7 && r.UserAccess == AccessLevel.Read));
        Assert.NotNull(result.SingleOrDefault(r => r.ProductID == 8 && r.UserAccess == AccessLevel.Execute));

        // Atom
        queryable = _context.CreateQuery<Product>("Products")
            .AddQueryOption("$filter", "SkinColor eq Microsoft.OData.Client.E2E.Tests.Common.Server.Default.Color'Red'");

        Assert.EndsWith("/Products?$filter=SkinColor eq Microsoft.OData.Client.E2E.Tests.Common.Server.Default.Color'Red'", queryable.RequestUri.OriginalString, StringComparison.Ordinal);
        result = queryable.ToList();
        Assert.Equal(result.Select(s => s.ProductID).Distinct().Count(), result.Count);
        Assert.True(result.All(s => s.SkinColor == Color.Red));
        Assert.Contains(result, s => s.UserAccess == AccessLevel.Execute);
        Assert.Contains(result, s => s.UserAccess == AccessLevel.Read);
    }

    [Fact]
    public void QueryEntitiesWithBinaryOperationFilterUsingODataClientAndVerifyResults()
    {
        // Arrange
        var products = _context.Products.ToList();
        _context.Format.UseJson(_model);

        // Act
        var queryable = _context.Products.Where(p => p.SkinColor == Color.Red).Where(p => AccessLevel.Read < p.UserAccess) as DataServiceQuery<Product>;
        Assert.NotNull(queryable);
        var result = queryable.ToList();

        // Assert
        Assert.EndsWith("/Products?$filter=SkinColor eq Microsoft.OData.Client.E2E.Tests.Common.Server.Default.Color'Red' and Microsoft.OData.Client.E2E.Tests.Common.Server.Default.AccessLevel'Read' lt UserAccess", queryable.RequestUri.OriginalString, StringComparison.Ordinal);
        Assert.Single(result);
        var product = result.Single();
        Assert.Equal(Color.Red, product.SkinColor);
        Assert.True(product.UserAccess > AccessLevel.Read);
        Assert.Equal(result.Count, products.Where(p => p.SkinColor == Color.Red && p.UserAccess > AccessLevel.Read).Count());
    }

    [Fact]
    public void QueryEntitiesWithEqualityFilterUsingODataClientAndVerifyResults()
    {
        // Arrange
        var products = _context.Products.ToList();
        _context.Format.UseJson(_model);

        // Act
        var queryable = _context.Products.Where(p => AccessLevel.Read == (AccessLevel)p.UserAccess) as DataServiceQuery<Product>;
        Assert.NotNull(queryable);
        var result = queryable.ToList();

        // Assert
        Assert.EndsWith("/Products?$filter=Microsoft.OData.Client.E2E.Tests.Common.Server.Default.AccessLevel'Read' eq UserAccess", queryable.RequestUri.OriginalString, StringComparison.Ordinal);
        Assert.True(result.All(s => s.UserAccess == AccessLevel.Read));
        Assert.Equal(result.Count, products.Where(p => p.UserAccess == AccessLevel.Read).Count());
    }

    [Fact]
    public void QueryEntitiesWithHasInFilterUsingODataClientAndVerifyResults()
    {
        // Arrange
        var products = _context.Products.ToList();
        _context.Format.UseJson(_model);

        // Act
        var queryable = _context.Products.Where(p => p.SkinColor.Value.HasFlag(Color.Red) || AccessLevel.Read.HasFlag(p.UserAccess.Value)) as DataServiceQuery<Product>;
        Assert.NotNull(queryable);
        var result = queryable.ToList();

        // Assert
        Assert.EndsWith("/Products?$filter=SkinColor has Microsoft.OData.Client.E2E.Tests.Common.Server.Default.Color'Red' or Microsoft.OData.Client.E2E.Tests.Common.Server.Default.AccessLevel'Read' has UserAccess", queryable.RequestUri.OriginalString, StringComparison.Ordinal);
        Assert.True(result.All(s => s.SkinColor.Value.HasFlag(Color.Red) || s.UserAccess.Value == AccessLevel.Read || s.UserAccess.Value == AccessLevel.None));
        Assert.Equal(products.Where(p => p.SkinColor.Value.HasFlag(Color.Red) || AccessLevel.Read.HasFlag(p.UserAccess)).Count(), result.Count);
    }

    [Fact]
    public void QueryEntitiesWithIsofInFilterUsingODataClientAndVerifyResults()
    {
        // Arrange
        var products = _context.Products.ToList();
        _context.Format.UseJson(_model);

        // Act
        var queryable = _context.Products.Where(p => p.UserAccess is AccessLevel) as DataServiceQuery<Product>;
        Assert.NotNull(queryable);
        var result = queryable.ToList();

        // Assert
        Assert.EndsWith("/Products?$filter=isof(UserAccess, 'Microsoft.OData.Client.E2E.Tests.Common.Server.Default.AccessLevel')", queryable.RequestUri.OriginalString, StringComparison.Ordinal);
        Assert.Equal(products.Count(), result.Count);
    }

    [Fact]
    public void QueryEntitiesWithAnyInFilterUsingODataClientAndVerifyResults()
    {
        // Arrange
        var products = _context.Products.ToList();
        _context.Format.UseJson(_model);

        // Act
        var queryable = _context.Products.Where(p => p.CoverColors.Any(c => c == Color.Blue)).Where(p => p.CoverColors.Any(c => c.HasFlag(Color.Blue))) as DataServiceQuery<Product>;
        Assert.NotNull(queryable);
        var result = queryable.ToList();

        // Assert
        Assert.EndsWith("/Products?$filter=CoverColors/any(c:c eq Microsoft.OData.Client.E2E.Tests.Common.Server.Default.Color'Blue') and CoverColors/any(c:c has Microsoft.OData.Client.E2E.Tests.Common.Server.Default.Color'Blue')", queryable.RequestUri.OriginalString, StringComparison.Ordinal);
        Assert.Equal(products.Where(p => p.CoverColors.Any(c => c == Color.Blue)).Count(), result.Count);
    }

    [Fact]
    public void QueryEntitiesWithAllInFilterUsingODataClientAndVerifyResults()
    {
        // Arrange
        var products = _context.Products.ToList();
        _context.Format.UseJson(_model);

        // Act
        var queryable = _context.Products.Where(p => p.CoverColors.All(c => c == Color.Blue)).Where(p => p.CoverColors.All(c => c.HasFlag(Color.Blue))) as DataServiceQuery<Product>;
        Assert.NotNull(queryable);
        var result = queryable.ToList();

        // Assert
        Assert.EndsWith("/Products?$filter=CoverColors/all(c:c eq Microsoft.OData.Client.E2E.Tests.Common.Server.Default.Color'Blue') and CoverColors/all(c:c has Microsoft.OData.Client.E2E.Tests.Common.Server.Default.Color'Blue')", queryable.RequestUri.OriginalString, StringComparison.Ordinal);
        Assert.Equal(products.Where(p => p.CoverColors.All(c => c == Color.Blue)).Count(), result.Count);
    }


    [Fact]
    public void QueryEntitiesWithOrderByUsingODataClientAndVerifyResults()
    {
        // Arrange
        var products = _context.Products.ToList();

        // Act
        var queryable = _context.Products.OrderBy(p => p.UserAccess) as DataServiceQuery<Product>;
        Assert.NotNull(queryable);
        var result = queryable.ToList();

        // Assert
        Assert.EndsWith("/Products?$orderby=UserAccess", queryable.RequestUri.OriginalString, StringComparison.Ordinal);
        for (int i = 0; i < result.Count; i++)
        {
            var orderedProducts = products.OrderBy(p => p.UserAccess);
            Assert.Equal(orderedProducts.ElementAt(i).ProductID, result[i].ProductID);
        }

        queryable = _context.Products.OrderByDescending(p => p.UserAccess) as DataServiceQuery<Product>;
        Assert.NotNull(queryable);
        result = queryable.ToList();
        Assert.EndsWith("/Products?$orderby=UserAccess desc", queryable.RequestUri.OriginalString, StringComparison.Ordinal);
        for (int i = 0; i < result.Count; i++)
        {
            var orderedProducts = products.OrderByDescending(p => p.UserAccess);
            Assert.Equal(orderedProducts.ElementAt(i).ProductID, result[i].ProductID);
        }
    }

    [Fact]
    public void QueryEntityByIdAndSelectUserAccessUsingODataClientAndVerifyResults()
    {
        // Arrange
        var products = _context.Products.ToList();
        _context.Format.UseJson(_model);

        // Act
        var queryable = _context.Products.ByKey(5).Select(p => p.UserAccess);
        Assert.NotNull(queryable);
        var result = queryable.GetValue();

        // Assert
        Assert.EndsWith("/Products(5)/UserAccess", queryable.RequestUri.OriginalString, StringComparison.Ordinal);
        Assert.Equal(products.Where(p => p.ProductID == 5).Single().UserAccess, result);
    }


    [Fact]
    public void QueryEntitiesAndSelectMultiplePropertiesUsingODataClientAndVerifyResults()
    {
        // Arrange
        var products = _context.Products.ToList();
        _context.Format.UseJson(_model);

        // Act
        var queryable = _context.Products
            .Select(p => new Product { UserAccess = p.UserAccess, CoverColors = p.CoverColors, SkinColor = p.SkinColor, ProductID = p.ProductID })
            as DataServiceQuery<Product>;
        Assert.NotNull(queryable);
        var result = queryable.ToList();

        // Assert
        Assert.EndsWith("/Products?$select=UserAccess,CoverColors,SkinColor,ProductID", queryable.RequestUri.OriginalString, StringComparison.Ordinal);
        foreach (var prod in result)
        {
            var expectedProd = products.Where(p => p.ProductID == prod.ProductID).Single();

            Assert.Equal(expectedProd.UserAccess, prod.UserAccess);
            Assert.Equal(expectedProd.SkinColor, prod.SkinColor);
            Assert.Equal(expectedProd.CoverColors.Count, prod.CoverColors.Count);
        }
    }

    #endregion

    #region Private methods

    private EnumerationTypeQueryTestsHelper TestsHelper
    {
        get
        {
            return new EnumerationTypeQueryTestsHelper(_baseUri, _model, Client);
        }
    }

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "enumerationtypetests/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    #endregion
}

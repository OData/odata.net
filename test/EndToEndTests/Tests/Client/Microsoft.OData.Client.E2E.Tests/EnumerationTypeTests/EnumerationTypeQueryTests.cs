//---------------------------------------------------------------------
// <copyright file="EnumerationTypeQueryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common.Client.Default;
using Microsoft.OData.E2E.TestCommon.Common.Client.Default.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.EnumerationTypes;
using Microsoft.OData.Edm;
using Xunit;
using AccessLevel = Microsoft.OData.E2E.TestCommon.Common.Client.Default.AccessLevel;
using Color = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Color;
using Product = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Product;

namespace Microsoft.OData.Client.E2E.Tests.EnumerationTypeTests;

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
        Assert.Equal((AccessLevel.Write | AccessLevel.Execute | AccessLevel.Read), result.UserAccess);
        Assert.Equal((AccessLevel.ReadWrite | AccessLevel.Execute), result.UserAccess);
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
        Assert.Equal((AccessLevel.Write | AccessLevel.Execute | AccessLevel.Read), enumResult[0]);
        Assert.Equal((AccessLevel.ReadWrite | AccessLevel.Execute), enumResult[0]);
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
            .AddQueryOption("$filter", "SkinColor eq Microsoft.OData.E2E.TestCommon.Common.Server.Default.Color'Red'");
        Assert.EndsWith("/Products?$filter=SkinColor eq Microsoft.OData.E2E.TestCommon.Common.Server.Default.Color'Red'", queryable.RequestUri.OriginalString, StringComparison.Ordinal);

        List<Product> result = queryable.ToList();
        Assert.Equal(result.Select(s => s.ProductID).Distinct().Count(), result.Count);
        Assert.True(result.All(s => s.SkinColor == Color.Red));
        Assert.NotNull(result.SingleOrDefault(r => r.ProductID == 5 && r.UserAccess == AccessLevel.None));
        Assert.NotNull(result.SingleOrDefault(r => r.ProductID == 7 && r.UserAccess == AccessLevel.Read));
        Assert.NotNull(result.SingleOrDefault(r => r.ProductID == 8 &&
            (r.UserAccess == (AccessLevel.Write | AccessLevel.Execute | AccessLevel.Read) || r.UserAccess == (AccessLevel.ReadWrite | AccessLevel.Execute))));

        // FullMetadata: UseJson() + have $select in request uri
        _context.Format.UseJson(_model);
        queryable = _context.CreateQuery<Product>("Products")
            .AddQueryOption("$filter", "SkinColor eq Microsoft.OData.E2E.TestCommon.Common.Server.Default.Color'Red'")
            .AddQueryOption("$select", "SkinColor,ProductID,Name");

        Assert.EndsWith("/Products?$filter=SkinColor eq Microsoft.OData.E2E.TestCommon.Common.Server.Default.Color'Red'&$select=SkinColor,ProductID,Name", queryable.RequestUri.OriginalString, StringComparison.Ordinal);
        result = queryable.ToList();
        Assert.Equal(result.Select(s => s.ProductID).Distinct().Count(), result.Count);
        Assert.True(result.All(s => s.SkinColor == Color.Red));
        Assert.NotNull(result.SingleOrDefault(r => r.ProductID == 5 && r.UserAccess == AccessLevel.None));
        Assert.NotNull(result.SingleOrDefault(r => r.ProductID == 7 && r.UserAccess == AccessLevel.Read));
        Assert.NotNull(result.SingleOrDefault(r => r.ProductID == 8 && 
            (r.UserAccess == (AccessLevel.Write | AccessLevel.Execute | AccessLevel.Read) || r.UserAccess == (AccessLevel.ReadWrite | AccessLevel.Execute))));

        // Atom
        queryable = _context.CreateQuery<Product>("Products")
            .AddQueryOption("$filter", "SkinColor eq Microsoft.OData.E2E.TestCommon.Common.Server.Default.Color'Red'");

        Assert.EndsWith("/Products?$filter=SkinColor eq Microsoft.OData.E2E.TestCommon.Common.Server.Default.Color'Red'", queryable.RequestUri.OriginalString, StringComparison.Ordinal);
        result = queryable.ToList();
        Assert.Equal(result.Select(s => s.ProductID).Distinct().Count(), result.Count);
        Assert.True(result.All(s => s.SkinColor == Color.Red));
        Assert.Contains(result, s => (s.UserAccess == (AccessLevel.Write | AccessLevel.Execute | AccessLevel.Read) || s.UserAccess == (AccessLevel.ReadWrite | AccessLevel.Execute)));
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
        Assert.EndsWith("/Products?$filter=SkinColor eq Microsoft.OData.E2E.TestCommon.Common.Server.Default.Color'Red' and Microsoft.OData.E2E.TestCommon.Common.Server.Default.AccessLevel'Read' lt UserAccess", queryable.RequestUri.OriginalString, StringComparison.Ordinal);
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
        Assert.EndsWith("/Products?$filter=Microsoft.OData.E2E.TestCommon.Common.Server.Default.AccessLevel'Read' eq UserAccess", queryable.RequestUri.OriginalString, StringComparison.Ordinal);
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
        Assert.EndsWith("/Products?$filter=SkinColor has Microsoft.OData.E2E.TestCommon.Common.Server.Default.Color'Red' or Microsoft.OData.E2E.TestCommon.Common.Server.Default.AccessLevel'Read' has UserAccess", queryable.RequestUri.OriginalString, StringComparison.Ordinal);
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
        Assert.EndsWith("/Products?$filter=isof(UserAccess, 'Microsoft.OData.E2E.TestCommon.Common.Server.Default.AccessLevel')", queryable.RequestUri.OriginalString, StringComparison.Ordinal);
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
        Assert.EndsWith("/Products?$filter=CoverColors/any(c:c eq Microsoft.OData.E2E.TestCommon.Common.Server.Default.Color'Blue') and CoverColors/any(c:c has Microsoft.OData.E2E.TestCommon.Common.Server.Default.Color'Blue')", queryable.RequestUri.OriginalString, StringComparison.Ordinal);
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
        Assert.EndsWith("/Products?$filter=CoverColors/all(c:c eq Microsoft.OData.E2E.TestCommon.Common.Server.Default.Color'Blue') and CoverColors/all(c:c has Microsoft.OData.E2E.TestCommon.Common.Server.Default.Color'Blue')", queryable.RequestUri.OriginalString, StringComparison.Ordinal);
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

    [Fact]
    public void QueryEntitiesWithCombinedFlagStringFormatUsingODataClient()
    {
        // Arrange
        _context.Format.UseJson(_model);

        // Act
        var queryable = _context.Products
            .Where(p => p.UserAccess == AccessLevel.ReadWrite) as DataServiceQuery<Product>;
        Assert.NotNull(queryable);
        var result = queryable.ToList();

        // Assert
        Assert.EndsWith("/Products?$filter=UserAccess eq Microsoft.OData.E2E.TestCommon.Common.Server.Default.AccessLevel'ReadWrite'", queryable.RequestUri.OriginalString);
        Assert.All(result, p => Assert.Equal(AccessLevel.ReadWrite, p.UserAccess));
    }

    [Fact]
    public void QueryEntitiesWithCombinedFlagNumericRepresentationUsingODataClient()
    {
        // Arrange
        _context.Format.UseJson(_model);

        // Act
        var queryable = _context.Products
            .Where(p => (int)p.UserAccess == 3) as DataServiceQuery<Product>;
        Assert.NotNull(queryable);
        var result = queryable.ToList();

        // Assert
        Assert.EndsWith("/Products?$filter=UserAccess eq Microsoft.OData.E2E.TestCommon.Common.Server.Default.AccessLevel'ReadWrite'", queryable.RequestUri.OriginalString);
        Assert.All(result, p => Assert.Equal(AccessLevel.ReadWrite, p.UserAccess));
    }

    [Fact]
    public void QueryEntitiesWithHasOperatorForCombinedFlagsUsingODataClient()
    {
        // Arrange
        _context.Format.UseJson(_model);

        // Act
        var queryable = _context.Products
            .Where(p => p.UserAccess.Value.HasFlag(AccessLevel.Read) && p.UserAccess.Value.HasFlag(AccessLevel.Write)) as DataServiceQuery<Product>;
        Assert.NotNull(queryable);
        var result = queryable.ToList();

        // Assert
        Assert.EndsWith("/Products?$filter=UserAccess has Microsoft.OData.E2E.TestCommon.Common.Server.Default.AccessLevel'Read' and UserAccess has Microsoft.OData.E2E.TestCommon.Common.Server.Default.AccessLevel'Write'", queryable.RequestUri.OriginalString);
        Assert.All(result, p => Assert.True(p.UserAccess.Value.HasFlag(AccessLevel.Read) && p.UserAccess.Value.HasFlag(AccessLevel.Write)));
    }

    [Fact]
    public void QueryEntitiesWithInOperatorForFlagsUsingODataClient()
    {
        // Arrange
        _context.Format.UseJson(_model);

        // Act
        var queryable = _context.Products
            .Where(p => new[] { AccessLevel.Read, AccessLevel.Write, AccessLevel.ReadWrite }.Contains(p.UserAccess.Value)) as DataServiceQuery<Product>;
        Assert.NotNull(queryable);
        var result = queryable.ToList();

        // Assert
        Assert.EndsWith("/Products?$filter=UserAccess in ('Read','Write','ReadWrite')", queryable.RequestUri.OriginalString);
        Assert.All(result, p => new[] { AccessLevel.Read, AccessLevel.Write, AccessLevel.ReadWrite }.Contains(p.UserAccess.Value));
    }

    [Fact]
    public void QueryEntitiesWithCombinedFlagNumericEqualityUsingODataClient()
    {
        // Arrange
        _context.Format.UseJson(_model);

        // Act
        // ReadWrite = 3
        var queryable = _context.Products
            .Where(p => (int)p.UserAccess == 3) as DataServiceQuery<Product>;
        Assert.NotNull(queryable);
        var result = queryable.ToList();

        // Assert
        Assert.EndsWith("/Products?$filter=UserAccess eq Microsoft.OData.E2E.TestCommon.Common.Server.Default.AccessLevel'ReadWrite'", queryable.RequestUri.OriginalString);
        Assert.All(result, p => Assert.Equal(AccessLevel.ReadWrite, p.UserAccess));
    }

    [Fact]
    public void QueryEntitiesWithCombinedFlagNumericGreaterThanUsingODataClient()
    {
        // Arrange
        _context.Format.UseJson(_model);

        // Act
        // Execute = 4, so this will match Execute and any higher value if present
        var queryable = _context.Products
            .Where(p => (int)p.UserAccess > 3) as DataServiceQuery<Product>;
        Assert.NotNull(queryable);
        var result = queryable.ToList();

        // Assert
        Assert.EndsWith("/Products?$filter=UserAccess gt Microsoft.OData.E2E.TestCommon.Common.Server.Default.AccessLevel'ReadWrite'", queryable.RequestUri.OriginalString);
        Assert.All(result, p => Assert.True((int)p.UserAccess > 3));
    }

    [Fact]
    public void QueryEntitiesWithCombinedFlagNumericInOperatorUsingODataClient()
    {
        // Arrange
        _context.Format.UseJson(_model);

        // Act
        // None = 0, Read = 1, Write = 2, ReadWrite = 3, Execute = 4
        var validValues = new[] { 0, 3, 4 };
        var queryable = _context.Products
            .Where(p => validValues.Contains((int)p.UserAccess)) as DataServiceQuery<Product>;
        Assert.NotNull(queryable);
        var result = queryable.ToList();

        // Assert
        Assert.EndsWith("/Products?$filter=UserAccess in (0,3,4)", queryable.RequestUri.OriginalString, StringComparison.Ordinal);
        Assert.All(result, p => Assert.Contains((int)p.UserAccess, validValues));
    }

    #region Private methods

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "enumerationtypetests/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    #endregion
}

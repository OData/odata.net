//---------------------------------------------------------------------
// <copyright file="EnumerationTypeUpdateTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
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
/// Contains end-to-end tests for creating, updating, and deleting entities with enumeration type properties.
/// </summary>
public class EnumerationTypeUpdateTests : EndToEndTestBase<EnumerationTypeUpdateTests.TestsStartup>
{
    private readonly Uri _baseUri;
    private readonly Container _context;
    private readonly IEdmModel _model;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(EnumerationTypeUpdateTestsController), typeof(MetadataController));

            services.AddControllers().AddOData(opt =>
                opt.EnableQueryFeatures().AddRouteComponents("odata", DefaultEdmModel.GetEdmModel()));
        }
    }

    public EnumerationTypeUpdateTests(TestWebApplicationFactory<TestsStartup> fixture)
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

    /// <summary>
    /// Tests creating and updating a Product entity with enumeration type properties using the OData client.
    /// </summary>

    [Fact]
    public void CreateAndUpdateEntityUsingODataClient()
    {
        // Arrange
        _context.Format.UseJson(_model);

        string tmpName = Guid.NewGuid().ToString();
        var queryable = _context.Products.AddQueryOption("$filter", string.Format("Name eq '{0}'", tmpName));

        // query and verify
        var result1 = queryable.ToList();
        Assert.Empty(result1);

        // create an entity
        var product = new Product()
        {
            ProductID = new Random().Next(),
            Name = tmpName,
            SkinColor = Color.Red,
            Discontinued = false,
            QuantityInStock = 23,
            QuantityPerUnit = "my quantity per unit",
            UnitPrice = 23.01f,
            UserAccess = AccessLevel.ReadWrite,
            CoverColors =
            [
                Color.Red,
                Color.Blue
            ]
        };
        _context.AddToProducts(product);
        _context.SaveChanges();

        // query and verify
        var result2 = queryable.ToList();
        Assert.Single(result2);
        Assert.Equal(Color.Red, result2[0].SkinColor);
        Assert.Equal(AccessLevel.ReadWrite, result2[0].UserAccess);

        // update the Enum properties
        product.SkinColor = Color.Green;
        product.UserAccess = AccessLevel.Execute;
        _context.UpdateObject(product);
        _context.SaveChanges();

        // query and verify
        var result3 = queryable.ToList();
        Assert.Single(result3);
        Assert.Equal(Color.Green, result3[0].SkinColor);
        Assert.Equal(AccessLevel.Execute, result2[0].UserAccess);
    }

    #region Private methods

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "enumerationtypeupdatetests/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    #endregion
}

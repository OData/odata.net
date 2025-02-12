//---------------------------------------------------------------------
// <copyright file="EnumerationTypeUpdateTests.cs" company="Microsoft">
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
using Microsoft.OData.E2E.TestCommon.Common.Server.EnumerationTypes;
using Microsoft.OData.Edm;
using System.Collections.ObjectModel;

namespace Microsoft.OData.Core.E2E.Tests.EnumerationTypeTests;

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

    private static string NameSpacePrefix = typeof(DefaultEdmModel).Namespace ?? "Microsoft.OData.E2E.TestCommon.Common.Server.Default";

    // Constants
    private const string MimeTypeODataParameterFullMetadata = MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata;
    private const string MimeTypeODataParameterMinimalMetadata = MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata;

    #region Tests creating and deleting an entity with enumeration type properties.

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task CreateDeleteEntityWithEnumProperties(string mimeType)
    {
        // Arrange
        // construct the request message to create an entity
        var productEntry = new ODataResource
        {
            TypeName = $"{NameSpacePrefix}.Product",
            Properties =
            [
                    new ODataProperty { Name = "Name", Value = "MoonCake" },
                    new ODataProperty { Name = "ProductID", Value = 101 },
                    new ODataProperty { Name = "QuantityInStock", Value = 20 },
                    new ODataProperty { Name = "QuantityPerUnit", Value = "100g Bag" },
                    new ODataProperty { Name = "UnitPrice", Value = 8.0f },
                    new ODataProperty { Name = "Discontinued", Value = true },
                    new ODataProperty { Name = "SkinColor", Value = new ODataEnumValue("Green", $"{NameSpacePrefix}.Color") },
                    new ODataProperty {
                        Name = "CoverColors",
                        Value = new ODataCollectionValue()
                        {
                            TypeName = $"Collection({NameSpacePrefix}.Color)",
                            Items = new Collection<object>()
                            {
                                new ODataEnumValue("Green", $"{NameSpacePrefix}.Color"),
                                new ODataEnumValue("Blue", $"{NameSpacePrefix}.Color"),
                                new ODataEnumValue("Green", $"{NameSpacePrefix}.Color")
                            }
                        }
                    },
                    new ODataProperty { Name = "UserAccess", Value = new ODataEnumValue("Read", $"{NameSpacePrefix}.AccessLevel") }
            ]
        };

        var settings = new ODataMessageWriterSettings
        {
            BaseUri = _baseUri,
            EnableMessageStreamDisposal = false
        };
        var productType = _model.FindDeclaredType($"{NameSpacePrefix}.Product") as IEdmEntityType;
        var productSet = _model.EntityContainer.FindEntitySet("Products");

        var requestMessage = new TestHttpClientRequestMessage(new Uri(_baseUri + "Products", UriKind.Absolute), Client) { Method = "POST" };
        requestMessage.SetHeader("Content-Type", mimeType);
        requestMessage.SetHeader("Accept", mimeType);
        using (var messageWriter = new ODataMessageWriter(requestMessage, settings, _model))
        {
            var odataWriter = messageWriter.CreateODataResourceWriter(productSet, productType);
            odataWriter.WriteStart(productEntry);
            odataWriter.WriteEnd();
        }

        // Act
        // send the http request
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        // verify the create
        Assert.Equal(201, responseMessage.StatusCode);

        var entries = await TestsHelper.QueryResourceEntriesAsync("Products(101)", MimeTypes.ApplicationJsonLight);
        Assert.Single(entries);

        var entry = entries.Single();

        var productID = entry.Properties.Single(p => p.Name == "ProductID") as ODataProperty;
        var skinColor = (entry.Properties.Single(p => p.Name == "SkinColor") as ODataProperty)?.Value as ODataEnumValue;
        var userAccess = (entry.Properties.Single(p => p.Name == "UserAccess") as ODataProperty)?.Value as ODataEnumValue;
        Assert.Equal(101, productID?.Value);
        Assert.Equal("Green", skinColor?.Value);
        Assert.Equal("Read", userAccess?.Value);

        // delete the entry
        var deleteRequestMessage = new TestHttpClientRequestMessage(new Uri(_baseUri + "Products(101)", UriKind.Absolute), Client) { Method = "DELETE" };

        var deleteResponseMessage = await deleteRequestMessage.GetResponseAsync();

        // verify the delete
        Assert.Equal(204, deleteResponseMessage.StatusCode);
        entries = await TestsHelper.QueryResourceEntriesAsync("Products(101)", MimeTypes.ApplicationJsonLight);
        var deletedEntry = entries.SingleOrDefault();
        Assert.Null(deletedEntry);
    }

    #endregion

    #region Tests updating the enumeration type properties of an entity.

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task UpdateEnumProperties(string mimeType)
    {
        // Arrange
        // query an entry
        var entries = await TestsHelper.QueryResourceEntriesAsync("Products(5)", MimeTypes.ApplicationJsonLight);
        ODataResource productEntry = entries.Single();

        // send a request to update an entry property
        productEntry = new ODataResource()
        {
            TypeName = $"{NameSpacePrefix}.Product",
            Properties =
            [
                new ODataProperty { Name = "SkinColor", Value = new ODataEnumValue("Green") },
                new ODataProperty { Name = "UserAccess", Value = new ODataEnumValue("Read") }
            ]
        };

        var settings = new ODataMessageWriterSettings() { BaseUri = _baseUri, EnableMessageStreamDisposal = false };
        var productType = _model.FindDeclaredType($"{NameSpacePrefix}.Product") as IEdmEntityType;
        var productSet = _model.EntityContainer.FindEntitySet("Products");

        var requestMessage = new TestHttpClientRequestMessage(new Uri(_baseUri + "Products(5)", UriKind.Absolute), Client) { Method = "PATCH" };
        requestMessage.SetHeader("Content-Type", mimeType);
        requestMessage.SetHeader("Accept", mimeType);

        using (var messageWriter = new ODataMessageWriter(requestMessage, settings, _model))
        {
            var odataWriter = messageWriter.CreateODataResourceWriter(productSet, productType);
            odataWriter.WriteStart(productEntry);
            odataWriter.WriteEnd();
        }

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // verify the update
        Assert.Equal(204, responseMessage.StatusCode);

        entries = await TestsHelper.QueryResourceEntriesAsync("Products(5)", MimeTypes.ApplicationJsonLight);
        Assert.Single(entries);

        ODataResource updatedProduct = entries.Single();
        var skinColor = (updatedProduct.Properties.Single(p => p.Name == "SkinColor") as ODataProperty)?.Value as ODataEnumValue;
        var userAccess = (updatedProduct.Properties.Single(p => p.Name == "UserAccess") as ODataProperty)?.Value as ODataEnumValue;
        Assert.Equal("Green", skinColor?.Value);
        Assert.Equal("Read", userAccess?.Value);
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
        var actionUri = new Uri(_baseUri + "enumerationtypeupdatetests/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    #endregion
}

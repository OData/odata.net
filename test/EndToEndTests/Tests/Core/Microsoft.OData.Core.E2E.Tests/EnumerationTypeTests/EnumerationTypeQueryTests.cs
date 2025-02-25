﻿//---------------------------------------------------------------------
// <copyright file="EnumerationTypeQueryTests.cs" company="Microsoft">
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

namespace Microsoft.OData.Core.E2E.Tests.EnumerationTypeTests;

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

    private static string NameSpacePrefix = typeof(DefaultEdmModel).Namespace ?? "Microsoft.OData.E2E.TestCommon.Common.Server.Default";

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
            var odataWriter = messageWriter.CreateODataParameterWriter(null);
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
        var queryText = "Products?$filter=UserAccess has Microsoft.OData.E2E.TestCommon.Common.Server.Default.AccessLevel'Read'";

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

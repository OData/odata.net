//-----------------------------------------------------------------------------
// <copyright file="IsOfAndCastTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common;
using Microsoft.OData.E2E.TestCommon.Common.Client.Default.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.IsOfAndCast;
using Microsoft.OData.Edm;
using Microsoft.Spatial;
using HomeAddress = Microsoft.OData.E2E.TestCommon.Common.Server.Default.HomeAddress;

namespace Microsoft.OData.Core.E2E.Tests.IsOfAndCastTests;

public class IsOfAndCastTests : EndToEndTestBase<IsOfAndCastTests.TestsStartup>
{
    private readonly Uri _baseUri;
    private readonly Container _context;
    private readonly IEdmModel _model;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(IsOfAndCastController), typeof(MetadataController));

            services.AddControllers().AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(null)
                .AddRouteComponents("odata", DefaultEdmModel.GetEdmModel()));
        }
    }

    public IsOfAndCastTests(TestWebApplicationFactory<TestsStartup> fixture) : base(fixture)
    {
        _baseUri = new Uri(Client.BaseAddress, "odata/");

        _context = new Container(_baseUri)
        {
            HttpClientFactory = HttpClientFactory
        };

        _model = DefaultEdmModel.GetEdmModel();
        ResetDefaultDataSource();
    }

    #region Cast Tests

    [Theory]
    [InlineData("cast(Microsoft.OData.E2E.TestCommon.Common.Server.Default.Employee) ne null")]
    [InlineData("cast('Microsoft.OData.E2E.TestCommon.Common.Server.Default.Employee') ne null")]
    public async Task CastOnEntityType_WithFilterExpression_ReturnsCorrectResults(string expression)
    {
        // Arrange
        ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

        var requestUrl = new Uri(_baseUri.AbsoluteUri + $"People?$filter={expression} ne null", UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);

        requestMessage.SetHeader("Accept", MimeTypeODataParameterMinimalMetadata);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        var entries = new List<ODataResource>();

        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
        {
            var reader = await messageReader.CreateODataResourceSetReaderAsync();

            while (await reader.ReadAsync())
            {
                if (reader.State == ODataReaderState.ResourceEnd)
                {
                    if (reader.Item is ODataResource entry && entry.TypeName.EndsWith("Employee"))
                    {
                        entries.Add(entry);
                    }
                }
            }
        }

        Assert.Equal(2, entries.Count);
        Assert.EndsWith("/People(3)/Microsoft.OData.E2E.TestCommon.Common.Server.Default.Employee", entries[0].EditLink.AbsoluteUri);
        Assert.Equal("Jacob", (entries[0].Properties.FirstOrDefault(p => p.Name == "FirstName") as ODataProperty)?.Value);
        Assert.Equal(new DateTimeOffset(new DateTime(2010, 12, 13)), (entries[0].Properties.FirstOrDefault(p => p.Name == "DateHired") as ODataProperty)?.Value);

        Assert.EndsWith("/People(4)/Microsoft.OData.E2E.TestCommon.Common.Server.Default.Employee", entries[1].EditLink.AbsoluteUri);
        Assert.Equal("Elmo", (entries[1].Properties.FirstOrDefault(p => p.Name == "FirstName") as ODataProperty)?.Value);
        Assert.Equal(GeographyPoint.Create(-15.0, -62), (entries[1].Properties.FirstOrDefault(p => p.Name == "Office") as ODataProperty)?.Value);
    }

    [Theory]
    [InlineData("cast(Microsoft.OData.E2E.TestCommon.Common.Server.Default.Customer)/City eq 'London'")]
    [InlineData("cast('Microsoft.OData.E2E.TestCommon.Common.Server.Default.Customer')/City eq 'London'")]
    public async Task CastOnEntityType_WithPropertyCheck_ReturnsCorrectResults(string filterExpression)
    {
        // Arrange
        ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

        var requestUrl = new Uri(_baseUri.AbsoluteUri + $"People?$filter={filterExpression}", UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);

        requestMessage.SetHeader("Accept", MimeTypeODataParameterMinimalMetadata);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        var entries = new List<ODataResource>();

        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
        {
            var reader = await messageReader.CreateODataResourceSetReaderAsync();

            while (await reader.ReadAsync())
            {
                if (reader.State == ODataReaderState.ResourceEnd)
                {
                    if (reader.Item is ODataResource entry && entry.TypeName.EndsWith("Customer"))
                    {
                        entries.Add(entry);
                    }
                }
            }
        }

        Assert.Single(entries);
        Assert.EndsWith("/People(1)/Microsoft.OData.E2E.TestCommon.Common.Server.Default.Customer", entries[0].EditLink.AbsoluteUri);
        Assert.Equal(1, (entries[0].Properties.FirstOrDefault(p => p.Name == "PersonID") as ODataProperty)?.Value);
        Assert.Equal("Bob", (entries[0].Properties.FirstOrDefault(p => p.Name == "FirstName") as ODataProperty)?.Value);
        Assert.Equal("London", (entries[0].Properties.FirstOrDefault(p => p.Name == "City") as ODataProperty)?.Value);
    }

    [Theory]
    [InlineData("cast(HomeAddress, Microsoft.OData.E2E.TestCommon.Common.Server.Default.HomeAddress)/City eq 'Sydney'")]
    [InlineData("cast(HomeAddress, 'Microsoft.OData.E2E.TestCommon.Common.Server.Default.HomeAddress')/City eq 'Sydney'")]
    public async Task CastOnComplexProperty_WithFilterExpression_ReturnsCorrectResults(string filterExpression)
    {
        // Arrange
        ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

        var requestUrl = new Uri(_baseUri.AbsoluteUri + $"People?$filter={filterExpression}", UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);

        requestMessage.SetHeader("Accept", MimeTypeODataParameterMinimalMetadata);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        var entries = new List<ODataResource>();

        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
        {
            var reader = await messageReader.CreateODataResourceSetReaderAsync();

            while (await reader.ReadAsync())
            {
                if (reader.State == ODataReaderState.ResourceEnd)
                {
                    if (reader.Item is ODataResource entry && entry.TypeName.EndsWith("Employee"))
                    {
                        entries.Add(entry);
                    }
                }
            }
        }

        Assert.Single(entries);
        Assert.EndsWith("/People(3)/Microsoft.OData.E2E.TestCommon.Common.Server.Default.Employee", entries[0].EditLink.AbsoluteUri);
        Assert.Equal(3, (entries[0].Properties.FirstOrDefault(p => p.Name == "PersonID") as ODataProperty)?.Value);
        Assert.Equal("Jacob", (entries[0].Properties.FirstOrDefault(p => p.Name == "FirstName") as ODataProperty)?.Value);
    }

    [Theory]
    [InlineData("Addresses/any(a: cast(a, Microsoft.OData.E2E.TestCommon.Common.Server.Default.HomeAddress) ne null)")]
    [InlineData("Addresses/any(a: cast(a, 'Microsoft.OData.E2E.TestCommon.Common.Server.Default.HomeAddress') ne null)")]
    public async Task CastInAnyClause_OnCollectionProperty_ReturnsCorrectResults(string filterExpression)
    {
        // Arrange
        ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

        var requestUrl = new Uri(_baseUri.AbsoluteUri + $"People?$filter={filterExpression}", UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);

        requestMessage.SetHeader("Accept", MimeTypeODataParameterMinimalMetadata);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        var entries = new List<ODataResource>();

        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
        {
            var reader = await messageReader.CreateODataResourceSetReaderAsync();

            while (await reader.ReadAsync())
            {
                if (reader.State == ODataReaderState.ResourceEnd)
                {
                    if (reader.Item is ODataResource entry && entry.TypeName.EndsWith("Person"))
                    {
                        entries.Add(entry);
                    }
                }
            }
        }

        Assert.Single(entries);
        Assert.EndsWith("/People(5)", entries[0].EditLink.AbsoluteUri);
        Assert.Equal(5, (entries[0].Properties.FirstOrDefault(p => p.Name == "PersonID") as ODataProperty)?.Value);
        Assert.Equal("Peter", (entries[0].Properties.FirstOrDefault(p => p.Name == "FirstName") as ODataProperty)?.Value);
    }

    #endregion

    #region IsOf Tests

    [Theory]
    [InlineData("isof(Microsoft.OData.E2E.TestCommon.Common.Server.Default.Employee)")]
    [InlineData("isof('Microsoft.OData.E2E.TestCommon.Common.Server.Default.Employee')")]
    public async Task IsofOnEntityType_WithFilterExpression_ReturnsCorrectResults(string expression)
    {
        // Arrange
        ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

        var requestUrl = new Uri(_baseUri.AbsoluteUri + $"People?$filter={expression}", UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);

        requestMessage.SetHeader("Accept", MimeTypeODataParameterMinimalMetadata);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        var entries = new List<ODataResource>();

        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
        {
            var reader = await messageReader.CreateODataResourceSetReaderAsync();

            while (await reader.ReadAsync())
            {
                if (reader.State == ODataReaderState.ResourceEnd)
                {
                    if (reader.Item is ODataResource entry && entry.TypeName.EndsWith("Employee"))
                    {
                        entries.Add(entry);
                    }
                }
            }
        }

        Assert.Equal(2, entries.Count);
        Assert.EndsWith("/People(3)/Microsoft.OData.E2E.TestCommon.Common.Server.Default.Employee", entries[0].EditLink.AbsoluteUri);
        Assert.Equal("Jacob", (entries[0].Properties.FirstOrDefault(p => p.Name == "FirstName") as ODataProperty)?.Value);
        Assert.Equal(new DateTimeOffset(new DateTime(2010, 12, 13)), (entries[0].Properties.FirstOrDefault(p => p.Name == "DateHired") as ODataProperty)?.Value);

        Assert.EndsWith("/People(4)/Microsoft.OData.E2E.TestCommon.Common.Server.Default.Employee", entries[1].EditLink.AbsoluteUri);
        Assert.Equal("Elmo", (entries[1].Properties.FirstOrDefault(p => p.Name == "FirstName") as ODataProperty)?.Value);
        Assert.Equal(GeographyPoint.Create(-15.0, -62), (entries[1].Properties.FirstOrDefault(p => p.Name == "Office") as ODataProperty)?.Value);
    }

    [Theory]
    [InlineData("isof(HomeAddress, Microsoft.OData.E2E.TestCommon.Common.Server.Default.HomeAddress) and HomeAddress/City eq 'Sydney'")]
    [InlineData("isof(HomeAddress, 'Microsoft.OData.E2E.TestCommon.Common.Server.Default.HomeAddress') and HomeAddress/City eq 'Sydney'")]
    public async Task IsofOnComplexProperty_WithFilterExpression_ReturnsCorrectResults(string filterExpression)
    {
        // Arrange
        ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

        var requestUrl = new Uri(_baseUri.AbsoluteUri + $"People?$filter={filterExpression}", UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);

        requestMessage.SetHeader("Accept", MimeTypeODataParameterMinimalMetadata);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        var entries = new List<ODataResource>();

        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
        {
            var reader = await messageReader.CreateODataResourceSetReaderAsync();

            while (await reader.ReadAsync())
            {
                if (reader.State == ODataReaderState.ResourceEnd)
                {
                    if (reader.Item is ODataResource entry && entry.TypeName.EndsWith("Employee"))
                    {
                        entries.Add(entry);
                    }
                }
            }
        }

        Assert.Single(entries);
        Assert.EndsWith("/People(3)/Microsoft.OData.E2E.TestCommon.Common.Server.Default.Employee", entries[0].EditLink.AbsoluteUri);
        Assert.Equal(3, (entries[0].Properties.FirstOrDefault(p => p.Name == "PersonID") as ODataProperty)?.Value);
        Assert.Equal("Jacob", (entries[0].Properties.FirstOrDefault(p => p.Name == "FirstName") as ODataProperty)?.Value);
    }

    [Theory]
    [InlineData("Addresses/any(a: isof(a, Microsoft.OData.E2E.TestCommon.Common.Server.Default.HomeAddress))")]
    [InlineData("Addresses/any(a: isof(a, 'Microsoft.OData.E2E.TestCommon.Common.Server.Default.HomeAddress'))")]
    public async Task IsofInAnyClause_OnCollectionProperty_ReturnsCorrectResults(string filterExpression)
    {
        // Arrange
        ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

        var requestUrl = new Uri(_baseUri.AbsoluteUri + $"People?$filter={filterExpression}", UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);

        requestMessage.SetHeader("Accept", MimeTypeODataParameterMinimalMetadata);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        var entries = new List<ODataResource>();

        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
        {
            var reader = await messageReader.CreateODataResourceSetReaderAsync();

            while (await reader.ReadAsync())
            {
                if (reader.State == ODataReaderState.ResourceEnd)
                {
                    if (reader.Item is ODataResource entry && entry.TypeName.EndsWith("Person"))
                    {
                        entries.Add(entry);
                    }
                }
            }
        }

        Assert.Single(entries);
        Assert.EndsWith("/People(5)", entries[0].EditLink.AbsoluteUri);
        Assert.Equal(5, (entries[0].Properties.FirstOrDefault(p => p.Name == "PersonID") as ODataProperty)?.Value);
        Assert.Equal("Peter", (entries[0].Properties.FirstOrDefault(p => p.Name == "FirstName") as ODataProperty)?.Value);
    }

    #endregion

    #region Private

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "isofandcasttests/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    #endregion
}

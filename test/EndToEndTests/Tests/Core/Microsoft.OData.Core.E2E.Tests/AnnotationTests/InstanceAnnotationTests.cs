//---------------------------------------------------------------------
// <copyright file="InstanceAnnotationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Formatter.Serialization;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common;
using Microsoft.OData.E2E.TestCommon.Common.Client.Default.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.Annotations;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Core.E2E.Tests.AnnotationTests;

public class InstanceAnnotationTests : EndToEndTestBase<InstanceAnnotationTests.TestsStartup>
{
    private readonly Uri _baseUri;
    private readonly IEdmModel _model;
    private readonly Container _context;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(InstanceAnnotationTestsController), typeof(MetadataController));

            services.AddControllers().AddOData(opt =>
                opt.EnableQueryFeatures().AddRouteComponents(
                    "odata", 
                    DefaultEdmModel.GetEdmModel(), 
                    configureServices: s => s.AddSingleton<ODataResourceSerializer, CustomResourceSerializer>()));
        }
    }

    public InstanceAnnotationTests(TestWebApplicationFactory<TestsStartup> fixture) : base(fixture)
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
    private const string IncludeAnnotation = "odata.include-annotations";
    private const string NameSpacePrefix = "Microsoft.OData.E2E.TestCommon.Common.Server.Default";
    private new const string MimeTypeODataParameterFullMetadata = MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata;
    private new const string MimeTypeODataParameterMinimalMetadata = MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata;

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task EntityInstanceAnnotation_WithValidMimeType_ReturnsAnnotations(string mimeType)
    {
        // Arrange
        var requestUrl = new Uri(_baseUri.AbsoluteUri + "Boss", UriKind.Absolute);

        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
        requestMessage.SetHeader("Accept", mimeType);
        requestMessage.SetHeader("Prefer", string.Format("{0}={1}", IncludeAnnotation, "*"));

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        var readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri, ShouldIncludeAnnotation = (ann) => true };
        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
        {
            var entries = new List<ODataResource>();
            var reader = await messageReader.CreateODataResourceReaderAsync();
            while (await reader.ReadAsync())
            {
                if (reader.State == ODataReaderState.ResourceEnd && reader.Item is ODataResource odataResource)
                {
                    entries.Add(odataResource);
                }
            }
            Assert.Equal(ODataReaderState.Completed, reader.State);

            var instanceAnnotations = entries.Last().InstanceAnnotations;
            Assert.NotNull(instanceAnnotations);
            Assert.Equal(true, (instanceAnnotations.First(i => i.Name.Equals(string.Format("{0}.IsBoss", NameSpacePrefix))).Value as ODataPrimitiveValue)?.Value);
        }
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task ComplexTypeInstanceAnnotation_WithValidMimeType_ReturnsAnnotations(string mimeType)
    {
        // Arrange
        var requestUrl = new Uri(_baseUri.AbsoluteUri + "People(1)", UriKind.Absolute);

        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
        requestMessage.SetHeader("Accept", mimeType);
        requestMessage.SetHeader("Prefer", string.Format("{0}={1}", IncludeAnnotation, "*"));

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        var personType = _model.FindType(NameSpacePrefix + ".Person") as IEdmEntityType;

        var readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri, ShouldIncludeAnnotation = (ann) => true };
        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
        {
            var reader = await messageReader.CreateODataResourceReaderAsync(personType);

            ODataResource? entry = null;
            bool startHomeAddress = false;
            while (await reader.ReadAsync())
            {
                if (reader.State == ODataReaderState.NestedResourceInfoStart)
                {
                    if (reader.Item is ODataNestedResourceInfo navigation && navigation.Name == "HomeAddress")
                    {
                        startHomeAddress = true;
                    }
                }
                else if (reader.State == ODataReaderState.NestedResourceInfoEnd)
                {
                    if (reader.Item is ODataNestedResourceInfo navigation && navigation.Name == "HomeAddress")
                    {
                        startHomeAddress = false;
                    }
                }
                else if (reader.State == ODataReaderState.ResourceEnd)
                {
                    entry = reader.Item as ODataResource;

                    if (startHomeAddress)
                    {
                        // Verify Annotation on Complex Type
                        var annotationOnHomeAddress = entry?.InstanceAnnotations.Last();
                        Assert.NotNull(annotationOnHomeAddress);
                        Assert.Equal(NameSpacePrefix + ".AddressType", annotationOnHomeAddress.Name);
                        Assert.Equal("Home", (annotationOnHomeAddress.Value as ODataPrimitiveValue)?.Value);
                    }
                }
            }

            // Verify Annotation on Property of Entity
            var annotationEmails = entry?.Properties.SingleOrDefault(p => p.Name.Equals("Emails"))?.InstanceAnnotations.SingleOrDefault();
            Assert.NotNull(annotationEmails);
            Assert.Equal(NameSpacePrefix + ".DisplayName", annotationEmails.Name);
            Assert.Equal("EmailAddresses", (annotationEmails.Value as ODataPrimitiveValue)?.Value);
            Assert.Equal(ODataReaderState.Completed, reader.State);
        }
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task TopLevelComplexTypeInstanceAnnotation_WithValidMimeType_ReturnsAnnotations(string mimeType)
    {
        // Arrange
        var requestUrl = new Uri(_baseUri.AbsoluteUri + "People(1)/HomeAddress", UriKind.Absolute);

        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
        requestMessage.SetHeader("Accept", mimeType);
        requestMessage.SetHeader("Prefer", string.Format("{0}={1}", IncludeAnnotation, "*"));

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        var addressType = _model.FindType(NameSpacePrefix + ".HomeAddress") as IEdmComplexType;

        var readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri, ShouldIncludeAnnotation = (ann) => true };
        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
        {
            var reader = await messageReader.CreateODataResourceReaderAsync(addressType);
            ODataResource? resource = null;
            while (await reader.ReadAsync())
            {
                if (reader.State == ODataReaderState.ResourceEnd && reader.Item is ODataResource odataResource)
                {
                    resource = odataResource;
                }
            }

            // Verify Annotation on Complex Type
            var annotations = resource?.InstanceAnnotations;
            Assert.NotNull(annotations);

            var annotationOnHomeAddress = annotations.SingleOrDefault();
            Assert.NotNull(annotationOnHomeAddress);
            Assert.Equal(NameSpacePrefix + ".AddressType", annotationOnHomeAddress.Name);
            Assert.Equal("Home", (annotationOnHomeAddress.Value as ODataPrimitiveValue)?.Value);

            // Verify Annotation on Property in Complex Type
            var annotationOnCity = resource?.Properties.SingleOrDefault(p => p.Name.Equals("City"))?.InstanceAnnotations.SingleOrDefault();
            Assert.NotNull(annotationOnCity);
            Assert.Equal(NameSpacePrefix + ".CityInfo", annotationOnCity.Name);
            Assert.Equal("BestCity", (annotationOnCity.Value as ODataPrimitiveValue)?.Value);
        }
    }

    #region Private

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "annotationstests/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    #endregion
}

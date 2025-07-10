//---------------------------------------------------------------------
// <copyright file="InstanceAnnotationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Text;
using System.Text.Json;
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

    [Fact]
    public async Task EntityInstanceAnnotation_WithValidMimeType_ReturnsAnnotations_VerifyInResponseContent()
    {
        // Arrange
        var requestUrl = new Uri(_baseUri.AbsoluteUri + "Boss", UriKind.Absolute);

        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
        requestMessage.SetHeader("Accept", MimeTypeODataParameterMinimalMetadata);
        requestMessage.SetHeader("Prefer", string.Format("{0}={1}", IncludeAnnotation, "*"));

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        var responseString = await this.ReadAsStringAsync(responseMessage);

        var responseInString = @"
{
  ""@odata.context"": ""http://localhost/odata/$metadata#Boss/Microsoft.OData.E2E.TestCommon.Common.Server.Default.Customer"",
  ""@odata.type"": ""#Microsoft.OData.E2E.TestCommon.Common.Server.Default.Customer"",
  ""@Microsoft.OData.E2E.TestCommon.Common.Server.Default.IsBoss"": true,
  ""PersonID"": 2,
  ""FirstName"": ""Jill"",
  ""LastName"": ""Jones"",
  ""MiddleName"": null,
  ""Numbers"": [],
  ""Emails@Microsoft.OData.E2E.TestCommon.Common.Server.Default.DisplayName"": ""EmailAddresses"",
  ""Emails"": [],
  ""Home"": {
    ""type"": ""Point"",
    ""coordinates"": [
      161.8,
      15.0
    ],
    ""crs"": {
      ""type"": ""name"",
      ""properties"": {
        ""name"": ""EPSG:4326""
      }
    }
  },
  ""UpdatedTime"": ""0001-01-01T00:00:00Z"",
  ""City@Microsoft.OData.E2E.TestCommon.Common.Server.Default.CityInfo"": ""BestCity"",
  ""City"": ""Sydney"",
  ""Birthday"": ""1983-01-15T00:00:00Z"",
  ""TimeBetweenLastTwoOrders"": ""PT0.0000002S"",
  ""Addresses"": [],
  ""HomeAddress"": null
}";

        Assert.NotNull(responseString);
        Assert.Equal(responseInString.Trim(), responseInString.Trim());
    }

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

    [Fact]
    public async Task ComplexTypeInstanceAnnotation_WithValidMimeType_ReturnsAnnotations_VerifyInResponseContent()
    {
        // Arrange
        var requestUrl = new Uri(_baseUri.AbsoluteUri + "People(1)", UriKind.Absolute);

        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
        requestMessage.SetHeader("Accept", MimeTypeODataParameterMinimalMetadata);
        requestMessage.SetHeader("Prefer", string.Format("{0}={1}", IncludeAnnotation, "*"));

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        var responseString = await this.ReadAsStringAsync(responseMessage);

        var responseInString = @"
{
  ""@odata.context"": ""http://localhost/odata/$metadata#People/Microsoft.OData.E2E.TestCommon.Common.Server.Default.Customer/$entity"",
  ""@odata.type"": ""#Microsoft.OData.E2E.TestCommon.Common.Server.Default.Customer"",
  ""PersonID"": 1,
  ""FirstName"": ""Bob"",
  ""LastName"": ""Cat"",
  ""MiddleName"": null,
  ""Numbers"": [
    ""111-111-1111"",
    ""012"",
    ""310"",
    ""bca"",
    ""ayz""
  ],
  ""Emails@Microsoft.OData.E2E.TestCommon.Common.Server.Default.DisplayName"": ""EmailAddresses"",
  ""Emails"": [
    ""abc@abc.com""
  ],
  ""Home"": {
    ""type"": ""Point"",
    ""coordinates"": [
      23.1,
      32.1
    ],
    ""crs"": {
      ""type"": ""name"",
      ""properties"": {
        ""name"": ""EPSG:4326""
      }
    }
  },
  ""UpdatedTime"": ""0001-01-01T00:00:00Z"",
  ""City@Microsoft.OData.E2E.TestCommon.Common.Server.Default.CityInfo"": ""BestCity"",
  ""City"": ""London"",
  ""Birthday"": ""1957-04-03T00:00:00Z"",
  ""TimeBetweenLastTwoOrders"": ""PT0.0000001S"",
  ""Addresses"": [
    {
      ""@odata.type"": ""#Microsoft.OData.E2E.TestCommon.Common.Server.Default.HomeAddress"",
      ""@Microsoft.OData.E2E.TestCommon.Common.Server.Default.AddressType"": ""Home"",
      ""Street"": ""1 Microsoft Way"",
      ""City@Microsoft.OData.E2E.TestCommon.Common.Server.Default.CityInfo"": ""BestCity"",
      ""City"": ""Tokyo"",
      ""PostalCode"": ""98052"",
      ""UpdatedTime"": ""0001-01-01T00:00:00Z"",
      ""FamilyName"": ""Cats""
    },
    {
      ""Street"": ""999 Zixing Road"",
      ""City@Microsoft.OData.E2E.TestCommon.Common.Server.Default.CityInfo"": ""BestCity"",
      ""City"": ""Shanghai"",
      ""PostalCode"": ""200000"",
      ""UpdatedTime"": ""0001-01-01T00:00:00Z""
    }
  ],
  ""HomeAddress"": {
    ""@odata.type"": ""#Microsoft.OData.E2E.TestCommon.Common.Server.Default.HomeAddress"",
    ""@Microsoft.OData.E2E.TestCommon.Common.Server.Default.AddressType"": ""Home"",
    ""Street"": ""1 Microsoft Way"",
    ""City@Microsoft.OData.E2E.TestCommon.Common.Server.Default.CityInfo"": ""BestCity"",
    ""City"": ""Tokyo"",
    ""PostalCode"": ""98052"",
    ""UpdatedTime"": ""0001-01-01T00:00:00Z"",
    ""FamilyName"": ""Cats""
  }
}";

        Assert.NotNull(responseString);
        Assert.Equal(responseInString.Trim(), responseString.Trim());
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

    [Fact]
    public async Task TopLevelComplexTypeInstanceAnnotation_WithValidMimeType_ReturnsAnnotations_VerifyInResponseContent()
    {
        // Arrange
        var requestUrl = new Uri(_baseUri.AbsoluteUri + "People(1)/HomeAddress", UriKind.Absolute);

        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
        requestMessage.SetHeader("Accept", MimeTypeODataParameterFullMetadata);
        requestMessage.SetHeader("Prefer", string.Format("{0}={1}", IncludeAnnotation, "*"));

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        var responseString = await this.ReadAsStringAsync(responseMessage);

        var responseInString = @"
{
  ""@odata.context"": ""http://localhost/odata/$metadata#People(1)/HomeAddress"",
  ""@odata.type"": ""#Microsoft.OData.E2E.TestCommon.Common.Server.Default.HomeAddress"",
  ""@Microsoft.OData.E2E.TestCommon.Common.Server.Default.AddressType"": ""Home"",
  ""Street"": ""1 Microsoft Way"",
  ""City@Microsoft.OData.E2E.TestCommon.Common.Server.Default.CityInfo"": ""BestCity"",
  ""City"": ""Tokyo"",
  ""PostalCode"": ""98052"",
  ""UpdatedTime@odata.type"": ""#DateTimeOffset"",
  ""UpdatedTime"": ""0001-01-01T00:00:00Z"",
  ""FamilyName"": ""Cats""
}";

        Assert.NotNull(responseString);
        Assert.Equal(responseInString.Trim(), responseString.Trim());
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

    private async Task<string> ReadAsStringAsync(IODataResponseMessageAsync responseMessage)
    {
        using (Stream stream = await responseMessage.GetStreamAsync())
        {
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                var content = await reader.ReadToEndAsync();

                // Format the content in JSON format
                var jsonElement = JsonSerializer.Deserialize<JsonElement>(content);
                return JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions { WriteIndented = true });
            }
        }
    }

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "annotationstests/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    #endregion
}

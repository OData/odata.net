//---------------------------------------------------------------------
// <copyright file="ModelReferenceQueryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common;
using Microsoft.OData.E2E.TestCommon.Common.Server.ODataModelRef;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Core.E2E.Tests.ModelReferenceTests;

public class ModelReferenceQueryTests : EndToEndTestBase<ModelReferenceQueryTests.TestsStartup>
{
    private readonly Uri _baseUri;
    private readonly IEdmModel _model;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(ODataModelRefTestsController), typeof(MetadataController));

            services.AddControllers().AddOData(opt =>
            {
                opt.AddRouteComponents("odata", ODataModelRefEdmModel.GetEdmModel()).EnableQueryFeatures();
            });
        }
    }

    public ModelReferenceQueryTests(TestWebApplicationFactory<TestsStartup> fixture) : base(fixture)
    {
        if (Client.BaseAddress == null)
        {
            throw new ArgumentNullException(nameof(Client.BaseAddress), "Base address cannot be null");
        }

        _baseUri = new Uri(Client.BaseAddress, "odata/");
        _model = ODataModelRefEdmModel.GetEdmModel();

        ResetDefaultDataSource();
    }

    // Constants
    private const string NameSpacePrefix = "Microsoft.OData.E2E.TestCommon.Common.Server.ODataModelRef.";

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task ServiceDocument_ReturnsExpectedEntitySetsAndFunctionImports(string mimeType)
    {
        // Arrange
        var readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri, EnableMessageStreamDisposal = false };

        var requestMessage = new TestHttpClientRequestMessage(new Uri(_baseUri.AbsoluteUri, UriKind.Absolute), Client);
        requestMessage.SetHeader("Accept", mimeType);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
        {
            ODataServiceDocument workSpace = messageReader.ReadServiceDocument();
            Assert.NotNull(workSpace.EntitySets.Single(c => c.Name == "Trucks"));
            Assert.Equal("GetDefaultOutsideGeoFenceAlarm", workSpace.FunctionImports.First().Name);
        }
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task FeedWhosePropertyDefinedInReferencedModel_ReturnsExpectedProperties(string mimeType)
    {
        // Arrange
        var readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri, EnableMessageStreamDisposal = false };
        var requestMessage = new TestHttpClientRequestMessage(new Uri(_baseUri.AbsoluteUri + "Trucks", UriKind.Absolute), Client);
        requestMessage.SetHeader("Accept", mimeType);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
        {
            var reader = messageReader.CreateODataResourceSetReader();

            ODataResource? entry = null;
            while (reader.Read())
            {
                if (reader.State == ODataReaderState.ResourceEnd)
                {
                    entry = reader.Item as ODataResource;
                }
                else if (reader.State == ODataReaderState.ResourceSetEnd)
                {
                    Assert.NotNull(reader.Item as ODataResourceSet);
                }
            }
            Assert.NotNull((entry?.Properties.Single(p => p.Name == "Key") as ODataProperty)?.Value);
            //string.Format("{0}.TruckDemo.TruckType", TestModelNameSpace)
            Assert.NotNull(entry.TypeName);
            Assert.Equal(ODataReaderState.Completed, reader.State);
        }
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task EntryWhosePropertyDefinedInReferencedModel_ReturnsExpectedProperties(string mimeType)
    {
        // Arrange
        var readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri, EnableMessageStreamDisposal = false };
        var requestMessage = new TestHttpClientRequestMessage(new Uri(_baseUri.AbsoluteUri + "Trucks('Key1')", UriKind.Absolute), Client);
        requestMessage.SetHeader("Accept", mimeType);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
        {
            var reader = messageReader.CreateODataResourceReader();
            var entries = new List<ODataResource>();
            while (reader.Read())
            {
                if (reader.State == ODataReaderState.ResourceEnd && reader.Item is ODataResource resource)
                {
                    entries.Add(resource);
                }
            }
            Assert.Equal(ODataReaderState.Completed, reader.State);

            Assert.Equal("Key1", (entries.Last().Properties.Single(p => p.Name == "Key") as ODataProperty)?.Value);
            Assert.Equal("Vin1", (entries.Last().Properties.Single(p => p.Name == "VIN") as ODataProperty)?.Value);
            Assert.Equal((double)1, (entries.Last().Properties.Single(p => p.Name == "FuelLevel") as ODataProperty)?.Value);
            Assert.Equal(true, (entries.Last().Properties.Single(p => p.Name == "ACState") as ODataProperty)?.Value);
        }
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task FeedTypeDefinedInReferencedModel_ReturnsExpectedProperties(string mimeType)
    {
        // Arrange
        var readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri, EnableMessageStreamDisposal = false };
        var requestMessage = new TestHttpClientRequestMessage(new Uri(_baseUri.AbsoluteUri + "VehicleGPSSet", UriKind.Absolute), Client);
        requestMessage.SetHeader("Accept", mimeType);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
        {
            var reader = messageReader.CreateODataResourceSetReader();
            ODataResource? entry = null;

            while (reader.Read())
            {
                if (reader.State == ODataReaderState.ResourceEnd && reader.Item is ODataResource resource)
                {
                    entry = resource;
                }
                else if (reader.State == ODataReaderState.ResourceSetEnd)
                {
                    Assert.NotNull(reader.Item as ODataResourceSet);
                }
            }

            Assert.NotNull((entry?.Properties.Single(p => p.Name == "Key") as ODataProperty)?.Value);
            Assert.Equal(ODataReaderState.Completed, reader.State);
        }
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task EntryTypeDefinedInReferencedModel_ReturnsExpectedProperties(string mimeType)
    {
        // Arrange
        var readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri, EnableMessageStreamDisposal = false };
        var requestMessage = new TestHttpClientRequestMessage(new Uri(_baseUri.AbsoluteUri + "VehicleGPSSet('VehicleKey2')", UriKind.Absolute), Client);
        requestMessage.SetHeader("Accept", mimeType);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
        {
            var reader = messageReader.CreateODataResourceReader();

            List<ODataResource> entries = new List<ODataResource>();
            while (reader.Read())
            {
                if (reader.State == ODataReaderState.ResourceEnd)
                {
                    entries.Add(reader.Item as ODataResource);
                }
            }
            Assert.Equal("VehicleKey2", (entries.Last().Properties.Single(p => p.Name == "Key") as ODataProperty)?.Value);
            Assert.Equal(ODataReaderState.Completed, reader.State);
        }
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task FeedTypeDerivedFromReferencedModel_ReturnsExpectedProperties(string mimeType)
    {
        // Arrange
        var readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri, EnableMessageStreamDisposal = false };
        var requestMessage = new TestHttpClientRequestMessage(new Uri(_baseUri.AbsoluteUri + "DerivedVehicleGPSSet", UriKind.Absolute), Client);
        requestMessage.SetHeader("Accept", mimeType);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
        {
            var reader = messageReader.CreateODataResourceSetReader();
            ODataResource? entry = null;

            while (reader.Read())
            {
                if (reader.State == ODataReaderState.ResourceEnd)
                {
                    entry = reader.Item as ODataResource;
                }
                else if (reader.State == ODataReaderState.ResourceSetEnd)
                {
                    Assert.NotNull(reader.Item as ODataResourceSet);
                }
            }

            Assert.NotNull((entry?.Properties.Single(p => p.Name == "DisplayName") as ODataProperty)?.Value);
            Assert.NotNull(entry.TypeName);
            Assert.Equal(ODataReaderState.Completed, reader.State);
        }
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task TypeCastEntryDerivedFromReferencedModel_ReturnsExpectedProperties(string mimeType)
    {
        // Arrange
        var readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri, EnableMessageStreamDisposal = false };
        var requestMessage = new TestHttpClientRequestMessage(new Uri(_baseUri.AbsoluteUri + "VehicleGPSSet('VehicleKey6')/Microsoft.OData.E2E.TestCommon.Common.Server.ODataModelRef.TruckDemo.DerivedVehicleGPSType", UriKind.Absolute), Client);
        requestMessage.SetHeader("Accept", mimeType);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
        {
            var reader = messageReader.CreateODataResourceReader();

            var entries = new List<ODataResource>();
            while (reader.Read())
            {
                if (reader.State == ODataReaderState.ResourceEnd)
                {
                    var entry = reader.Item as ODataResource;
                    if (entry != null && entry.TypeName.EndsWith("VehicleGPSType"))
                    {
                        entries.Add(entry);
                    }
                }
            }

            Assert.Equal(ODataReaderState.Completed, reader.State);

            Assert.Single(entries);
            Assert.Equal("VehicleKey6", (entries[0].Properties.Single(p => p.Name == "Key") as ODataProperty)?.Value);
            Assert.Equal("DisplayName6", (entries[0].Properties.Single(p => p.Name == "DisplayName") as ODataProperty)?.Value);
            Assert.Equal(entries[0].TypeName, string.Format("{0}TruckDemo.DerivedVehicleGPSType", NameSpacePrefix));
        }
    }

    [Fact]
    public async Task PropertyDefinedInReferencedModel_ReturnsExpectedProperties()
    {
        // Arrange
        var readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri, EnableMessageStreamDisposal = false };
        var requestMessage = new TestHttpClientRequestMessage(
            new Uri(_baseUri.AbsoluteUri + "DerivedVehicleGPSSet('VehicleKey4')/Microsoft.OData.E2E.TestCommon.Common.Server.ODataModelRef.GPS.VehicleGPSType/StartLocation", UriKind.Absolute), Client);
        requestMessage.SetHeader("Accept", MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
        {
            var reader = messageReader.CreateODataResourceReader();

            var entries = new List<ODataResource>();
            while (reader.Read())
            {
                if (reader.State == ODataReaderState.ResourceEnd && reader.Item is ODataResource resource)
                {
                    entries.Add(resource);
                }
            }

            Assert.Single(entries);
            Assert.Equal((double)19.1, (entries[0].Properties.Single(p => p.Name == "Lat") as ODataProperty)?.Value);
            Assert.Equal((double)12.3, (entries[0].Properties.Single(p => p.Name == "Long") as ODataProperty)?.Value);
        }
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task FeedDefinedInReferencedModel_ReturnsExpectedProperties(string mimeType)
    {
        // Arrange
        var readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri, EnableMessageStreamDisposal = false };
        var requestMessage = new TestHttpClientRequestMessage(new Uri(_baseUri.AbsoluteUri + "VehicleGPSSetInGPS", UriKind.Absolute), Client);
        requestMessage.SetHeader("Accept", mimeType);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
        {
            var reader = messageReader.CreateODataResourceSetReader();
            ODataResource? entry = null;
            while (reader.Read())
            {
                if (reader.State == ODataReaderState.ResourceEnd)
                {
                    entry = reader.Item as ODataResource;
                }
                else if (reader.State == ODataReaderState.ResourceSetEnd)
                {
                    Assert.NotNull(reader.Item as ODataResourceSet);
                }
            }

            Assert.Equal(ODataReaderState.Completed, reader.State);

            Assert.NotNull((entry?.Properties.Single(p => p.Name == "Key") as ODataProperty)?.Value);
            Assert.Equal((double)120, (entry?.Properties.Single(p => p.Name == "VehicleSpeed") as ODataProperty)?.Value);
            Assert.Equal("DerivedVehicleGPSInGPSDP", (entry?.Properties.Single(p => p.Name == "DisplayName") as ODataProperty)?.Value);
        }
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task TypeCastEntryInCycleReferencingModels_ReturnsExpectedProperties(string mimeType)
    {
        // Arrange
        var readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri, EnableMessageStreamDisposal = false };
        var requestMessage = new TestHttpClientRequestMessage(new Uri(_baseUri.AbsoluteUri + "VehicleGPSSetInGPS('DerivedVehicleGPSInGPSKey3')/Microsoft.OData.E2E.TestCommon.Common.Server.ODataModelRef.TruckDemo.DerivedVehicleGPSType", UriKind.Absolute), Client);
        requestMessage.SetHeader("Accept", mimeType);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
        {
            var reader = messageReader.CreateODataResourceReader();

            var entries = new List<ODataResource>();
            while (reader.Read())
            {
                if (reader.State == ODataReaderState.ResourceEnd)
                {
                    var entry = reader.Item as ODataResource;
                    if (entry != null && entry.TypeName.EndsWith("VehicleGPSType"))
                    {
                        entries.Add(entry);
                    }
                }
            }
            Assert.Equal(ODataReaderState.Completed, reader.State);

            Assert.Single(entries);
            Assert.Equal("DerivedVehicleGPSInGPSKey3", (entries[0].Properties.Single(p => p.Name == "Key") as ODataProperty)?.Value);
            Assert.Equal("DerivedVehicleGPSInGPSDP", (entries[0].Properties.Single(p => p.Name == "DisplayName") as ODataProperty)?.Value);
            Assert.Equal(entries[0].TypeName, string.Format("{0}TruckDemo.DerivedVehicleGPSType", NameSpacePrefix));
        }
    }

    #region Query Option
    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata, "Trucks('Key1')?$select=Key,VehicleGPS&$expand=VehicleGPS", new int[] { 2, 1 })]
    [InlineData(MimeTypeODataParameterMinimalMetadata, "Trucks('Key1')?$select=Key,VehicleGPS&$expand=VehicleGPS", new int[] { 2, 1 })]
    [InlineData(MimeTypeODataParameterFullMetadata, "Trucks('Key1')?$expand=HeadUnit,VehicleGPS", new int[] { 3, 4 })]
    [InlineData(MimeTypeODataParameterMinimalMetadata, "Trucks('Key1')?$expand=HeadUnit,VehicleGPS", new int[] { 3, 4 })]
    public async Task ModelReferenceWithExpandOption_ReturnsExpectedEntriesAndNavigationLinks(string mimeType, string queryText, int[] expectedValues)
    {
        // Arrange
        var readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri, EnableMessageStreamDisposal = false };

        var requestMessage = new TestHttpClientRequestMessage(new Uri(_baseUri.AbsoluteUri + queryText, UriKind.Absolute), Client);
        requestMessage.SetHeader("Accept", mimeType);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
        {
            var entries = new List<ODataResource>();
            var navigationLinks = new List<ODataNestedResourceInfo>();

            var reader = messageReader.CreateODataResourceReader();
            while (reader.Read())
            {
                if (reader.State == ODataReaderState.ResourceEnd)
                {
                    entries.Add(reader.Item as ODataResource);
                }
                else if (reader.State == ODataReaderState.NestedResourceInfoEnd)
                {
                    navigationLinks.Add(reader.Item as ODataNestedResourceInfo);
                }
            }

            Assert.Equal(ODataReaderState.Completed, reader.State);
            Assert.Equal(expectedValues[0], entries.Count(e => e.TypeName.Contains("TruckType") || e.TypeName.Contains("VehicleGPSType") || e.TypeName.Contains("HeadUnitType")));
            Assert.Equal(expectedValues[1], navigationLinks.Count(n => n.Url != null));
        }
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata, "VehicleGPSSet?$filter=VehicleSpeed gt 90", 3)]
    [InlineData(MimeTypeODataParameterMinimalMetadata, "VehicleGPSSet?$filter=VehicleSpeed gt 90", 3)]
    [InlineData(MimeTypeODataParameterFullMetadata, "DerivedVehicleGPSSet?$filter=DisplayName eq 'DisplayName4'", 1)]
    [InlineData(MimeTypeODataParameterMinimalMetadata, "DerivedVehicleGPSSet?$filter=DisplayName eq 'DisplayName4'", 1)]
    public async Task ModelReferenceWithFilterOption_ReturnsExpectedEntries(string mimeType, string queryText, int expectedValue)
    {
        // Arrange
        var readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri, EnableMessageStreamDisposal = false };

        var requestMessage = new TestHttpClientRequestMessage(new Uri(_baseUri.AbsoluteUri + queryText, UriKind.Absolute), Client);
        requestMessage.SetHeader("Accept", mimeType);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
        {
            List<ODataResource> entries = new List<ODataResource>();

            var reader = messageReader.CreateODataResourceSetReader();
            while (reader.Read())
            {
                if (reader.State == ODataReaderState.ResourceEnd)
                {
                    var entry = reader.Item as ODataResource;
                    if (entry != null && entry.TypeName.Contains("VehicleGPSType"))
                    {
                        entries.Add(reader.Item as ODataResource);
                    }
                }
            }
            Assert.Equal(ODataReaderState.Completed, reader.State);
            Assert.Equal(expectedValue, entries.Count);
        }
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata, "Trucks('Key1')?$select=Key,FuelLevel", 2)]
    [InlineData(MimeTypeODataParameterMinimalMetadata, "Trucks('Key1')?$select=Key,FuelLevel", 2)]
    [InlineData(MimeTypeODataParameterFullMetadata, "DerivedVehicleGPSSet('VehicleKey4')?$select=DisplayName,Map,StartLocation", 3)]
    [InlineData(MimeTypeODataParameterMinimalMetadata, "DerivedVehicleGPSSet('VehicleKey4')?$select=DisplayName,Map,StartLocation", 3)]
    [InlineData(MimeTypeODataParameterFullMetadata, "VehicleGPSSetInGPS('DerivedVehicleGPSInGPSKey3')/Microsoft.OData.E2E.TestCommon.Common.Server.ODataModelRef.TruckDemo.DerivedVehicleGPSType?$select=DisplayName,Map,StartLocation", 3)]
    [InlineData(MimeTypeODataParameterMinimalMetadata, "VehicleGPSSetInGPS('DerivedVehicleGPSInGPSKey3')/Microsoft.OData.E2E.TestCommon.Common.Server.ODataModelRef.TruckDemo.DerivedVehicleGPSType?$select=DisplayName,Map,StartLocation", 3)]
    public async Task ModelReferenceWithSelectOption_ReturnsExpectedProperties(string mimeType, string queryText, int expectedValue)
    {
        // Arrange
        var readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri, EnableMessageStreamDisposal = false };

        var requestMessage = new TestHttpClientRequestMessage(new Uri(_baseUri.AbsoluteUri + queryText, UriKind.Absolute), Client);
        requestMessage.SetHeader("Accept", mimeType);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
        {
            var entries = new List<ODataResource>();

            var reader = messageReader.CreateODataResourceReader();
            while (reader.Read())
            {
                if (reader.State == ODataReaderState.ResourceEnd)
                {
                    entries.Add(reader.Item as ODataResource);
                }
            }

            Assert.Equal(ODataReaderState.Completed, reader.State);
            Assert.Equal(expectedValue, entries.Last().Properties.Count() + entries.Count - 1 /*non-ComplexP + ComplexP*/);
        }
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task ModelReferenceWithOrderbyOption_ReturnsOrderedEntries(string mimeType)
    {
        // Arrange
        var readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri, EnableMessageStreamDisposal = false };

        var requestMessage = new TestHttpClientRequestMessage(
                new Uri(_baseUri.AbsoluteUri + "DerivedVehicleGPSSet?$orderby=DisplayName desc", UriKind.Absolute), Client);
        requestMessage.SetHeader("Accept", mimeType);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
        {
            var entries = new List<ODataResource>();

            var reader = messageReader.CreateODataResourceSetReader();
            while (reader.Read())
            {
                if (reader.State == ODataReaderState.ResourceEnd)
                {
                    var resource = reader.Item as ODataResource;
                    if (resource != null && resource.TypeName.Contains("VehicleGPSType"))
                    {
                        entries.Add(reader.Item as ODataResource);
                    }
                }
            }

            Assert.Equal(ODataReaderState.Completed, reader.State);
            Assert.Equal("DisplayName5", (entries[0].Properties.SingleOrDefault(p => p.Name == "DisplayName") as ODataProperty)?.Value);
            Assert.Equal("DisplayName4", (entries[1].Properties.SingleOrDefault(p => p.Name == "DisplayName") as ODataProperty)?.Value);
        }
    }

    #endregion

    #region Action/Function

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task BoundActionInReferencedModel_ExecutesSuccessfully(string mimeType)
    {
        // Arrange
        var requestMessage = new TestHttpClientRequestMessage(new Uri(_baseUri.AbsoluteUri + "VehicleGPSSet('VehicleKey6')/Microsoft.OData.E2E.TestCommon.Common.Server.ODataModelRef.GPS.ResetVehicleSpeed", UriKind.Absolute), Client);
        requestMessage.SetHeader("Accept", "*/*");
        requestMessage.SetHeader("Content-Type", mimeType);
        requestMessage.Method = "POST";

        var writerSettings = new ODataMessageWriterSettings() { BaseUri = _baseUri, EnableMessageStreamDisposal = false };
        using (var messageWriter = new ODataMessageWriter(requestMessage, writerSettings, _model))
        {
            var odataWriter = messageWriter.CreateODataParameterWriter(null);
            odataWriter.WriteStart();
            odataWriter.WriteValue("targetValue", 331122);
            odataWriter.WriteEnd();
        }

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        var entry = await this.QueryEntityItemAsync("VehicleGPSSet('VehicleKey6')");
        var actual = ((entry as ODataResource)?.Properties.Single(p => p.Name == "VehicleSpeed") as ODataProperty)?.Value;
        Assert.Equal((double)331122, actual);
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task UnBoundActionForEntryInReferencedModel_ExecutesSuccessfully(string mimeType)
    {
        // Arrange
        var requestMessage = new TestHttpClientRequestMessage(new Uri(_baseUri.AbsoluteUri + "ResetVehicleSpeedToValue", UriKind.Absolute), Client);
        requestMessage.SetHeader("Accept", "*/*");
        requestMessage.SetHeader("Content-Type", mimeType);
        requestMessage.Method = "POST";

        var writerSettings = new ODataMessageWriterSettings() { BaseUri = _baseUri, EnableMessageStreamDisposal = false };
        using (var messageWriter = new ODataMessageWriter(requestMessage, writerSettings, _model))
        {
            var odataWriter = await messageWriter.CreateODataParameterWriterAsync(null);
            await odataWriter.WriteStartAsync();
            await odataWriter.WriteValueAsync("targetValue", 7777787);
            await odataWriter.WriteEndAsync();
        }

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        var entry = await this.QueryEntityItemAsync("VehicleGPSSetInGPS('VehicleGPSSetInGPSKey2')");
        var actual = ((entry as ODataResource)?.Properties.Single(p => p.Name == "VehicleSpeed") as ODataProperty)?.Value;
        Assert.Equal((double)7777787, actual);
    }

    [Fact]
    public async Task BoundFunctionInReferencedModel_ReturnsExpectedValue()
    {
        // Arrange
        var readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri, EnableMessageStreamDisposal = false };

        var requestMessage = new TestHttpClientRequestMessage(new Uri(_baseUri.AbsoluteUri + "VehicleGPSSet('VehicleKey2')/Microsoft.OData.E2E.TestCommon.Common.Server.ODataModelRef.GPS.GetVehicleSpeed()", UriKind.Absolute), Client);
        requestMessage.SetHeader("Accept", "*/*");

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
        {
            var amount = messageReader.ReadProperty().Value;
            Assert.Equal((double)120, amount);
        }
    }

    [Fact]
    public async Task UnBoundFunctionReturnTypeInReferencedModel_ReturnsExpectedValue()
    {
        // Arrange
        var readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri, EnableMessageStreamDisposal = false };

        var requestMessage = new TestHttpClientRequestMessage(new Uri(_baseUri.AbsoluteUri + "GetDefaultOutsideGeoFenceAlarm()", UriKind.Absolute), Client);
        requestMessage.SetHeader("Accept", "*/*");

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        ODataResource? perp = null;
        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
        {
            var odataReader = messageReader.CreateODataResourceReader();
            while (odataReader.Read())
            {
                if (odataReader.State == ODataReaderState.ResourceEnd)
                {
                    perp = odataReader.Item as ODataResource;
                }
            }
        }

        Assert.Equal(1, (perp?.Properties.Single(p => p.Name == "Severity") as ODataProperty)?.Value);
    }

    #endregion

    #region Private

    private async Task<ODataItem?> QueryEntityItemAsync(string uri, int expectedStatusCode = 200)
    {
        var readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri };

        var queryRequestMessage = new TestHttpClientRequestMessage(new Uri(_baseUri.AbsoluteUri + uri, UriKind.Absolute), Client);
        queryRequestMessage.SetHeader("Accept", MimeTypes.ApplicationJsonLight);

        var queryResponseMessage = await queryRequestMessage.GetResponseAsync();

        Assert.Equal(expectedStatusCode, queryResponseMessage.StatusCode);

        ODataItem? item = null;
        if (expectedStatusCode == 200)
        {
            using (var messageReader = new ODataMessageReader(queryResponseMessage, readerSettings, _model))
            {
                var reader = messageReader.CreateODataResourceReader();
                while (reader.Read())
                {
                    if (reader.State == ODataReaderState.ResourceEnd)
                    {
                        item = reader.Item;
                    }
                }

                Assert.Equal(ODataReaderState.Completed, reader.State);
            }
        }

        return item;
    }

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "odatamodelreftests/Default.ResetDefaultDataSource", UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(actionUri, Client);
        requestMessage.Method = "POST";

        var responseMessage = requestMessage.GetResponseAsync().Result;

        Assert.Equal(200, responseMessage.StatusCode);
    }

    #endregion
}

//---------------------------------------------------------------------
// <copyright file="ModelReferenceCUDTests.cs" company="Microsoft">
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

// Apply the collection attribute to the test class
[Collection("NonParallelTests")]
public class ModelReferenceCUDTests : EndToEndTestBase<ModelReferenceCUDTests.TestsStartup>
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

    public ModelReferenceCUDTests(TestWebApplicationFactory<TestsStartup> fixture) : base(fixture)
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
    private const string NameSpacePrefix = "Microsoft.OData.E2E.TestCommon.Common.Server.ODataModelRef";

    #region CUD Testing
    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task PostDeleteTypeInReferencedModel(string mimeType)
    {
        // Arrange
        var entryWrapper = this.CreateVehicleGPS("30003", false);

        var settings = new ODataMessageWriterSettings
        {
            BaseUri = _baseUri,
            EnableMessageStreamDisposal = false
        };

        var vehicleGPSType = _model.FindType(NameSpacePrefix + ".GPS.VehicleGPSType") as IEdmEntityType;
        var vehicleGPSSet = _model.EntityContainer.FindEntitySet("VehicleGPSSet");

        var requestMessage = new TestHttpClientRequestMessage(new Uri(_baseUri + "VehicleGPSSet"), Client);

        requestMessage.SetHeader("Content-Type", mimeType);
        requestMessage.SetHeader("Accept", mimeType);
        requestMessage.Method = "POST";
        using (var messageWriter = new ODataMessageWriter(requestMessage, settings, _model))
        {
            var odataWriter = await messageWriter.CreateODataResourceWriterAsync(vehicleGPSSet, vehicleGPSType);
            await ODataWriterHelper.WriteResourceAsync(odataWriter, entryWrapper);
        }

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(201, responseMessage.StatusCode);

        var entry = await this.QueryEntityItemAsync($"VehicleGPSSet('30003')", 200 /* OK statusCode */) as ODataResource;
        Assert.Equal("30003", (entry?.Properties.Single(p => p.Name == "Key") as ODataProperty)?.Value);

        // Delete the entry
        var deleteRequestMessage = new TestHttpClientRequestMessage(new Uri(_baseUri + $"VehicleGPSSet('30003')"), Client);
        deleteRequestMessage.Method = "DELETE";
        var deleteResponseMessage = await deleteRequestMessage.GetResponseAsync();

        // Verify Deleted
        Assert.Equal(204, deleteResponseMessage.StatusCode);
        var deletedEntry = await this.QueryEntityItemAsync($"VehicleGPSSet('30003')", 204 /* NoContent statusCode */) as ODataResource;
        Assert.Null(deletedEntry);
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task PostDeleteTypeInReferencingModel(string mimeType)
    {
        // Arrange
        var entryWrapper = CreateVehicleGPS("20202", true);

        var settings = new ODataMessageWriterSettings
        {
            BaseUri = _baseUri,
            EnableMessageStreamDisposal = false
        };

        var vehicleGPSType = _model.FindType(NameSpacePrefix + ".GPS.VehicleGPSType") as IEdmEntityType;
        var vehicleGPSSet = _model.FindEntityContainer(NameSpacePrefix + ".GPS.GPSContainer").FindEntitySet("VehicleGPSSetInGPS");

        var requestMessage = new TestHttpClientRequestMessage(new Uri(_baseUri + "VehicleGPSSetInGPS"), Client);

        requestMessage.SetHeader("Content-Type", mimeType);
        requestMessage.SetHeader("Accept", mimeType);
        requestMessage.Method = "POST";
        using (var messageWriter = new ODataMessageWriter(requestMessage, settings, _model))
        {
            var odataWriter = await messageWriter.CreateODataResourceWriterAsync(vehicleGPSSet, vehicleGPSType);
            await ODataWriterHelper.WriteResourceAsync(odataWriter, entryWrapper);
        }

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(201, responseMessage.StatusCode);

        var entry = await this.QueryEntityItemAsync($"VehicleGPSSetInGPS('20202')", 200 /* OK statusCode */) as ODataResource;
        Assert.Equal("20202", (entry?.Properties.Single(p => p.Name == "Key") as ODataProperty)?.Value);

        // Delete the entry
        var deleteRequestMessage = new TestHttpClientRequestMessage(new Uri(_baseUri + $"VehicleGPSSetInGPS('20202')"), Client);
        deleteRequestMessage.Method = "DELETE";
        var deleteResponseMessage = await deleteRequestMessage.GetResponseAsync();

        // Verify Deleted
        Assert.Equal(204, deleteResponseMessage.StatusCode);

        var deletedEntry = await this.QueryEntityItemAsync($"VehicleGPSSetInGPS('20202')", 204 /* NoContent statusCode */) as ODataResource;
        Assert.Null(deletedEntry);
    }

    #endregion

    #region Private

    private ODataResourceWrapper CreateVehicleGPS(string key, bool postDeleteTypeInReferencingModel)
    {
        ODataResource newVehicleGPS = new ODataResource
        {
            TypeName = NameSpacePrefix + (postDeleteTypeInReferencingModel ? ".TruckDemo.DerivedVehicleGPSType" : ".GPS.VehicleGPSType")
        };

        var properties = new List<ODataProperty>
            {
                new ODataProperty
                {
                    Name = "Key",
                    Value = key
                },
                new ODataProperty
                {
                    Name = "VehicleSpeed",
                    Value = 999.9
                },
            };

        if (postDeleteTypeInReferencingModel)
        {
            properties.Add(new ODataProperty
            {
                Name = "DisplayName",
                Value = "NewDisplayName"
            });
        }
        newVehicleGPS.Properties = properties;

        var newVehicleGPSWrapper = new ODataResourceWrapper()
        {
            Resource = newVehicleGPS,
            NestedResourceInfoWrappers = new List<ODataNestedResourceInfoWrapper>()
                {
                    new ODataNestedResourceInfoWrapper()
                    {
                        NestedResourceInfo = new ODataNestedResourceInfo()
                        {
                            Name = "StartLocation",
                            IsCollection = false
                        },
                        NestedResourceOrResourceSet = new ODataResourceWrapper()
                        {
                            Resource = new ODataResource()
                            {
                                TypeName = NameSpacePrefix + ".Location.GeoLocation",
                                Properties = new[]
                                {
                                    new ODataProperty
                                    {
                                        Name = "Lat",
                                        Value = 99.9
                                    },
                                    new ODataProperty
                                    {
                                        Name = "Long",
                                        Value = 88.8
                                    }
                                }
                            }
                        }
                    },
                    new ODataNestedResourceInfoWrapper()
                    {
                        NestedResourceInfo = new ODataNestedResourceInfo()
                        {
                            Name = "EndLocation",
                            IsCollection = false
                        },
                        NestedResourceOrResourceSet = new ODataResourceWrapper()
                        {
                            Resource = new ODataResource()
                            {
                                TypeName = NameSpacePrefix + ".Location.GeoLocation",
                                Properties = new[]
                                {
                                    new ODataProperty
                                    {
                                        Name = "Lat",
                                        Value = 77.7
                                    },
                                    new ODataProperty
                                    {
                                        Name = "Long",
                                        Value = 88.8
                                    }
                                }
                            }
                        }
                    },
                    new ODataNestedResourceInfoWrapper()
                    {
                        NestedResourceInfo = new ODataNestedResourceInfo()
                        {
                            Name = "CurrentLocation",
                            IsCollection = false
                        },
                        NestedResourceOrResourceSet = new ODataResourceWrapper()
                        {
                            Resource = new ODataResource()
                            {
                                TypeName = NameSpacePrefix + ".Location.GeoLocation",
                                Properties = new[]
                                {
                                    new ODataProperty
                                    {
                                        Name = "Lat",
                                        Value = 88.8
                                    },
                                    new ODataProperty
                                    {
                                        Name = "Long",
                                        Value = 88.8
                                    }
                                }
                            }
                        }
                    },
                    new ODataNestedResourceInfoWrapper()
                    {
                        NestedResourceInfo = new ODataNestedResourceInfo()
                        {
                            Name = "Map",
                            IsCollection = false
                        },
                        NestedResourceOrResourceSet = new ODataResourceWrapper()
                        {
                            Resource = new ODataResource()
                            {
                                TypeName = NameSpacePrefix + ".Map.MapType",
                                Properties = new[]
                                {
                                    new ODataProperty
                                    {
                                        Name = "ProviderName",
                                        Value = "ProviderNew"
                                    },
                                    new ODataProperty
                                    {
                                        Name = "Uri",
                                        Value = "NewUri"
                                    },
                                    new ODataProperty
                                    {
                                        Name = "MBytesDownloaded",
                                        Value = 12.3
                                    }
                                }
                            }
                        }
                    },
                    new ODataNestedResourceInfoWrapper()
                    {
                        NestedResourceInfo = new ODataNestedResourceInfo()
                        {
                            Name = "LostSignalAlarm",
                            IsCollection = false
                        },
                        NestedResourceOrResourceSet = new ODataResourceWrapper()
                        {
                            Resource = new ODataResource()
                            {
                                TypeName = NameSpacePrefix + ".GPS.GPSLostSignalAlarmType",
                                Properties = new[]
                                {
                                    new ODataProperty
                                    {
                                        Name = "Severity",
                                        Value = 1
                                    }
                                }
                            },
                            NestedResourceInfoWrappers = new List<ODataNestedResourceInfoWrapper>()
                            {
                                new ODataNestedResourceInfoWrapper()
                                {
                                    NestedResourceInfo = new ODataNestedResourceInfo()
                                    {
                                        Name = "LastKnownLocation",
                                        IsCollection = false,
                                    },
                                    NestedResourceOrResourceSet = new ODataResourceWrapper()
                                    {
                                        Resource = new ODataResource()
                                        {
                                            TypeName = NameSpacePrefix + ".Location.GeoLocation",
                                            Properties = new[]
                                            {
                                                new ODataProperty
                                                {
                                                    Name = "Lat",
                                                    Value = 88.8
                                                },
                                                new ODataProperty
                                                {
                                                    Name = "Long",
                                                    Value = 88.8
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
        };

        return newVehicleGPSWrapper;
    }

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

// Collection definition to disable parallelization
[CollectionDefinition("NonParallelTests", DisableParallelization = true)]
public class NonParallelTestsCollection { }

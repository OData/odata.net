//---------------------------------------------------------------------
// <copyright file="ModelReferenceCUDTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.ModelReferenceTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ModelReferenceCUDTests : ODataWCFServiceTestsBase<InMemoryEntities>
    {
        private const string TestModelNameSpace = "Microsoft.OData.SampleService.Models.ModelRefDemo";

        public ModelReferenceCUDTests()
            : base(ServiceDescriptors.ModelRefServiceDescriptor)
        {

        }

        #region CUD Testing
        [TestMethod]
        public void PostDeleteTypeInReferencdModel()
        {
            var entryWrapper = CreateVehicleGPS(false);

            #region Create and Delete
            var settings = new ODataMessageWriterSettings();
            settings.BaseUri = ServiceBaseUri;

            var vehicleGPSType = Model.FindType(TestModelNameSpace + ".GPS.VehicleGPSType") as IEdmEntityType;
            var vehicleGPSSet = Model.EntityContainer.FindEntitySet("VehicleGPSSet");

            foreach (var mimeType in mimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "VehicleGPSSet"));

                requestMessage.SetHeader("Content-Type", mimeType);
                requestMessage.SetHeader("Accept", mimeType);
                requestMessage.Method = "POST";
                using (var messageWriter = new ODataMessageWriter(requestMessage, settings, Model))
                {
                    var odataWriter = messageWriter.CreateODataResourceWriter(vehicleGPSSet, vehicleGPSType);
                    ODataWriterHelper.WriteResource(odataWriter, entryWrapper);
                }

                // send the http request
                var responseMessage = requestMessage.GetResponse();

                // Verify Created
                Assert.AreEqual(201, responseMessage.StatusCode);
                ODataResource entry = this.QueryEntityItem("VehicleGPSSet('000')") as ODataResource;
                Assert.AreEqual("000", entry.Properties.Single(p => p.Name == "Key").Value);

                // Delete the entry
                var deleteRequestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "VehicleGPSSet('000')"));
                deleteRequestMessage.Method = "DELETE";
                var deleteResponseMessage = deleteRequestMessage.GetResponse();

                // Verift Deleted
                Assert.AreEqual(204, deleteResponseMessage.StatusCode);
                ODataResource deletedEntry = this.QueryEntityItem("VehicleGPSSet('000')", 204) as ODataResource;
                Assert.IsNull(deletedEntry);
            }
            #endregion
        }

        [TestMethod]
        public void PostDeleteTypeInReferencingModel()
        {
            var entryWrapper = CreateVehicleGPS(true);

            #region Create and Delete
            var settings = new ODataMessageWriterSettings();
            settings.BaseUri = ServiceBaseUri;

            var vehicleGPSType = Model.FindType(TestModelNameSpace + ".GPS.VehicleGPSType") as IEdmEntityType;

            var vehicleGPSSet = Model.FindEntityContainer(TestModelNameSpace + ".GPS.GPSContainer").FindEntitySet("VehicleGPSSetInGPS");

            foreach (var mimeType in mimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "VehicleGPSSetInGPS"));

                requestMessage.SetHeader("Content-Type", mimeType);
                requestMessage.SetHeader("Accept", mimeType);
                requestMessage.Method = "POST";
                using (var messageWriter = new ODataMessageWriter(requestMessage, settings, Model))
                {
                    var odataWriter = messageWriter.CreateODataResourceWriter(vehicleGPSSet, vehicleGPSType);
                    ODataWriterHelper.WriteResource(odataWriter, entryWrapper);
                }

                // send the http request
                var responseMessage = requestMessage.GetResponse();

                // Verify Created
                Assert.AreEqual(201, responseMessage.StatusCode);
                ODataResource entry = this.QueryEntityItem("VehicleGPSSetInGPS('000')") as ODataResource;
                Assert.AreEqual("000", entry.Properties.Single(p => p.Name == "Key").Value);

                // Delete the entry
                var deleteRequestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "VehicleGPSSetInGPS('000')"));
                deleteRequestMessage.Method = "DELETE";
                var deleteResponseMessage = deleteRequestMessage.GetResponse();

                // Verift Deleted
                Assert.AreEqual(204, deleteResponseMessage.StatusCode);
                ODataResource deletedEntry = this.QueryEntityItem("VehicleGPSSetInGPS('000')", 204) as ODataResource;
                Assert.IsNull(deletedEntry);
            }
            #endregion
        }

        #endregion

        private ODataResourceWrapper CreateVehicleGPS(bool postDeleteTypeInReferencingModel)
        {
            ODataResource newVehicleGPS = new ODataResource
            {
                TypeName = TestModelNameSpace + (postDeleteTypeInReferencingModel ? ".TruckDemo.DerivedVehicleGPSType" : ".GPS.VehicleGPSType")
            };

            var properties = new List<ODataProperty>
            {
                new ODataProperty
                {
                    Name = "Key", 
                    Value = "000"
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
                                TypeName = TestModelNameSpace + ".Location.GeoLocation",
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
                                TypeName = TestModelNameSpace + ".Location.GeoLocation",
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
                                TypeName = TestModelNameSpace + ".Location.GeoLocation",
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
                                TypeName = TestModelNameSpace + ".Map.MapType",
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
                                TypeName = TestModelNameSpace + ".GPS.GPSLostSignalAlarmType",
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
                                            TypeName = TestModelNameSpace + ".Location.GeoLocation",
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

        #region Private Method
        private ODataItem QueryEntityItem(string uri, int expectedStatusCode = 200)
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            var queryRequestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + uri, UriKind.Absolute));
            queryRequestMessage.SetHeader("Accept", MimeTypes.ApplicationJsonLight);
            var queryResponseMessage = queryRequestMessage.GetResponse();
            Assert.AreEqual(expectedStatusCode, queryResponseMessage.StatusCode);

            ODataItem item = null;
            if (expectedStatusCode == 200)
            {
                using (var messageReader = new ODataMessageReader(queryResponseMessage, readerSettings, Model))
                {
                    var reader = messageReader.CreateODataResourceReader();
                    while (reader.Read())
                    {
                        if (reader.State == ODataReaderState.ResourceEnd)
                        {
                            item = reader.Item;
                        }
                    }

                    Assert.AreEqual(ODataReaderState.Completed, reader.State);
                }
            }

            return item;
        }
        #endregion
    }
}

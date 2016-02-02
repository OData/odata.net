//---------------------------------------------------------------------
// <copyright file="ModelReferenceCUDTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.ModelReferenceTests
{
    using System;
    using System.Linq;
    using Microsoft.OData.Core;
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
            #region New Entry
            ODataEntry newVehicleGPS = new ODataEntry { TypeName = TestModelNameSpace + ".GPS.VehicleGPSType" };

            newVehicleGPS.Properties = new[]
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
                new ODataProperty
                {
                     Name = "StartLocation",
                     Value = new ODataComplexValue
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
                },
                new ODataProperty
                {
                    Name = "EndLocation",
                    Value = new ODataComplexValue
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
                },
                new ODataProperty
                {
                    Name = "CurrentLocation",
                    Value = new ODataComplexValue
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
                },
                new ODataProperty
                {
                    Name = "Map",
                    Value = new ODataComplexValue
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
                },
                new ODataProperty
                {
                    Name = "LostSignalAlarm",
                    Value = new ODataComplexValue
                    {
                        TypeName = TestModelNameSpace + ".GPS.GPSLostSignalAlarmType",
                        Properties = new[]
                        {
                            new ODataProperty
                            {
                                Name = "Severity",
                                Value = 1
                            },
                            new ODataProperty
                            {
                                Name = "LastKnownLocation",
                                Value = new ODataComplexValue
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
                            }, 
                        }
                    }
                }
            };
            #endregion

            #region Create and Delete
            var settings = new ODataMessageWriterSettings();
            settings.PayloadBaseUri = ServiceBaseUri;

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
                    var odataWriter = messageWriter.CreateODataEntryWriter(vehicleGPSSet, vehicleGPSType);
                    odataWriter.WriteStart(newVehicleGPS);
                    odataWriter.WriteEnd();
                }

                // send the http request
                var responseMessage = requestMessage.GetResponse();

                // Verify Created
                Assert.AreEqual(201, responseMessage.StatusCode);
                ODataEntry entry = this.QueryEntityItem("VehicleGPSSet('000')") as ODataEntry;
                Assert.AreEqual("000", entry.Properties.Single(p => p.Name == "Key").Value);

                // Delete the entry
                var deleteRequestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "VehicleGPSSet('000')"));
                deleteRequestMessage.Method = "DELETE";
                var deleteResponseMessage = deleteRequestMessage.GetResponse();

                // Verift Deleted
                Assert.AreEqual(204, deleteResponseMessage.StatusCode);
                ODataEntry deletedEntry = this.QueryEntityItem("VehicleGPSSet('000')", 204) as ODataEntry;
                Assert.IsNull(deletedEntry);
            }
            #endregion
        }

        [TestMethod]
        public void PostDeleteTypeInReferencingModel()
        {
            #region New Entry
            ODataEntry newVehicleGPS = new ODataEntry { TypeName = TestModelNameSpace + ".TruckDemo.DerivedVehicleGPSType" };

            newVehicleGPS.Properties = new[]
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
                new ODataProperty
                {
                     Name = "StartLocation",
                     Value = new ODataComplexValue
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
                },
                new ODataProperty
                {
                    Name = "EndLocation",
                    Value = new ODataComplexValue
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
                },
                new ODataProperty
                {
                    Name = "CurrentLocation",
                    Value = new ODataComplexValue
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
                },
                new ODataProperty
                {
                    Name = "Map",
                    Value = new ODataComplexValue
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
                },
                new ODataProperty
                {
                    Name = "LostSignalAlarm",
                    Value = new ODataComplexValue
                    {
                        TypeName = TestModelNameSpace + ".GPS.GPSLostSignalAlarmType",
                        Properties = new[]
                        {
                            new ODataProperty
                            {
                                Name = "Severity",
                                Value = 1
                            },
                            new ODataProperty
                            {
                                Name = "LastKnownLocation",
                                Value = new ODataComplexValue
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
                            }, 
                        }
                    }
                },
                new ODataProperty
                {
                    Name = "DisplayName",
                    Value = "NewDisplayName"
                }
            };
            #endregion

            #region Create and Delete
            var settings = new ODataMessageWriterSettings();
            settings.PayloadBaseUri = ServiceBaseUri;

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
                    var odataWriter = messageWriter.CreateODataEntryWriter(vehicleGPSSet, vehicleGPSType);
                    odataWriter.WriteStart(newVehicleGPS);
                    odataWriter.WriteEnd();
                }

                // send the http request
                var responseMessage = requestMessage.GetResponse();

                // Verify Created
                Assert.AreEqual(201, responseMessage.StatusCode);
                ODataEntry entry = this.QueryEntityItem("VehicleGPSSetInGPS('000')") as ODataEntry;
                Assert.AreEqual("000", entry.Properties.Single(p => p.Name == "Key").Value);

                // Delete the entry
                var deleteRequestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "VehicleGPSSetInGPS('000')"));
                deleteRequestMessage.Method = "DELETE";
                var deleteResponseMessage = deleteRequestMessage.GetResponse();

                // Verift Deleted
                Assert.AreEqual(204, deleteResponseMessage.StatusCode);
                ODataEntry deletedEntry = this.QueryEntityItem("VehicleGPSSetInGPS('000')", 204) as ODataEntry;
                Assert.IsNull(deletedEntry);
            }
            #endregion
        }

        [Ignore]
        [TestMethod]
        public void UpdateTypeInReferencedModel()
        {

        }

        #endregion

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
                    var reader = messageReader.CreateODataEntryReader();
                    while (reader.Read())
                    {
                        if (reader.State == ODataReaderState.EntryEnd)
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

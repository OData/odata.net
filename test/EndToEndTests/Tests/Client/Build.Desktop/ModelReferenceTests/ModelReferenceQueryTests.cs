﻿//---------------------------------------------------------------------
// <copyright file="ModelReferenceQueryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.ModelReferenceTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ModelReferenceQueryTests : ODataWCFServiceTestsBase<InMemoryEntities>
    {
        private const string TestModelNameSpace = "Microsoft.OData.SampleService.Models.ModelRefDemo";

        #region Query Path
        public ModelReferenceQueryTests()
            : base(ServiceDescriptors.ModelRefServiceDescriptor)
        { }

        [TestMethod]
        public void ServiceDocument()
        {
            string[] types = new string[]
            {
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata,
            };

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var mimeType in types)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri, UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        ODataServiceDocument workSpace = messageReader.ReadServiceDocument();
                        Assert.IsNotNull(workSpace.EntitySets.Single(c => c.Name == "Trucks"));
                        Assert.AreEqual("GetDefaultOutsideGeoFenceAlarm", workSpace.FunctionImports.First().Name);
                    }
                }
            }
        }

        [TestMethod]
        public void FeedWhosePropertyDefinedInReferencedModel()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };
            foreach (var mimeType in mimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Trucks", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        var reader = messageReader.CreateODataResourceSetReader();

                        ODataResource entry = null;
                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.ResourceEnd)
                            {
                                entry = reader.Item as ODataResource;
                            }
                            else if (reader.State == ODataReaderState.ResourceSetEnd)
                            {
                                Assert.IsNotNull(reader.Item as ODataResourceSet);
                            }
                        }
                        Assert.IsNotNull(entry.Properties.Single(p => p.Name == "Key").Value);
                        Assert.IsNotNull(entry.TypeName, string.Format("{0}.TruckDemo.TruckType", TestModelNameSpace));
                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                    }
                }
            }
        }

        [TestMethod]
        public void EntryWhosePropertyDefinedInReferencedModel()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };
            foreach (var mimeType in mimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Trucks('Key1')", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
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
                        Assert.AreEqual("Key1", entries.Last().Properties.Single(p => p.Name == "Key").Value);
                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                    }
                }
            }
        }

        [TestMethod]
        public void FeedTypeDefinedInReferencedModel()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };
            foreach (var mimeType in mimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "VehicleGPSSet", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        var reader = messageReader.CreateODataResourceSetReader();
                        ODataResource entry = null;

                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.ResourceEnd)
                            {
                                entry = reader.Item as ODataResource;
                            }
                            else if (reader.State == ODataReaderState.ResourceSetEnd)
                            {
                                Assert.IsNotNull(reader.Item as ODataResourceSet);
                            }
                        }

                        Assert.IsNotNull(entry.Properties.Single(p => p.Name == "Key").Value);
                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                    }
                }
            }
        }

        [TestMethod]
        public void EntryTypeDefinedInReferencedModel()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };
            foreach (var mimeType in mimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "VehicleGPSSet('VehicleKey2')", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
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
                        Assert.AreEqual("VehicleKey2", entries.Last().Properties.Single(p => p.Name == "Key").Value);
                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                    }
                }
            }
        }

        [TestMethod]
        public void FeedTypeDerivedFromReferencedModel()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };
            foreach (var mimeType in mimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "DerivedVehicleGPSSet", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        var reader = messageReader.CreateODataResourceSetReader();
                        ODataResource entry = null;

                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.ResourceEnd)
                            {
                                entry = reader.Item as ODataResource;
                            }
                            else if (reader.State == ODataReaderState.ResourceSetEnd)
                            {
                                Assert.IsNotNull(reader.Item as ODataResourceSet);
                            }
                        }

                        Assert.IsNotNull(entry.Properties.Single(p => p.Name == "DisplayName").Value);
                        Assert.IsNotNull(entry.TypeName, string.Format("{0}.TruckDemo.DerivedVehicleGPSType", TestModelNameSpace));
                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                    }
                }
            }
        }

        [TestMethod]
        public void TypeCastEntryDerivedFromReferencedModel()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };
            foreach (var mimeType in mimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "VehicleGPSSet('VehicleKey6')/Microsoft.OData.SampleService.Models.ModelRefDemo.TruckDemo.DerivedVehicleGPSType", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        var reader = messageReader.CreateODataResourceReader();

                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.ResourceEnd)
                            {
                                ODataResource entry = reader.Item as ODataResource;
                                if (entry != null & entry.TypeName.EndsWith("VehicleGPSType"))
                                {
                                    Assert.AreEqual("VehicleKey6", entry.Properties.Single(p => p.Name == "Key").Value);
                                    Assert.AreEqual("DisplayName6", entry.Properties.Single(p => p.Name == "DisplayName").Value);
                                    Assert.AreEqual(entry.TypeName, string.Format("{0}.TruckDemo.DerivedVehicleGPSType", TestModelNameSpace));
                                }
                            }
                        }
                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                    }
                }
            }
        }

        [TestMethod]
        public void PropertyDefinedInReferencedModel()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };
            var requestMessage = new HttpWebRequestMessage(
                new Uri(ServiceBaseUri.AbsoluteUri + "DerivedVehicleGPSSet('VehicleKey4')/Microsoft.OData.SampleService.Models.ModelRefDemo.GPS.VehicleGPSType/StartLocation", UriKind.Absolute));
            requestMessage.SetHeader("Accept", MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata);
            var responseMessage = requestMessage.GetResponse();
            Assert.AreEqual(200, responseMessage.StatusCode);

            using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
            {
                var odatareader = messageReader.CreateODataResourceReader();
                while (odatareader.Read())
                {
                    if (odatareader.State == ODataReaderState.ResourceEnd)
                    {
                        Assert.IsNotNull(odatareader.Item as ODataResource);
                    }
                }
            }
        }

        [TestMethod]
        public void FeedDefinedInReferencedModel()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };
            foreach (var mimeType in mimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "VehicleGPSSetInGPS", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        var reader = messageReader.CreateODataResourceSetReader();
                        ODataResource entry = null;
                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.ResourceEnd)
                            {
                                entry = reader.Item as ODataResource;
                            }
                            else if (reader.State == ODataReaderState.ResourceSetEnd)
                            {
                                Assert.IsNotNull(reader.Item as ODataResourceSet);
                            }
                        }

                        Assert.IsNotNull(entry.Properties.Single(p => p.Name == "Key").Value);
                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                    }
                }
            }
        }

        [TestMethod]
        public void TypeCastEntryinCycleReferencingModels()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };
            foreach (var mimeType in mimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "VehicleGPSSetInGPS('DerivedVehicleGPSInGPSKey3')/Microsoft.OData.SampleService.Models.ModelRefDemo.TruckDemo.DerivedVehicleGPSType", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        var reader = messageReader.CreateODataResourceReader();

                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.ResourceEnd)
                            {
                                ODataResource entry = reader.Item as ODataResource;
                                if (entry != null & entry.TypeName.EndsWith("VehicleGPSType"))
                                {
                                    Assert.AreEqual("DerivedVehicleGPSInGPSKey3", entry.Properties.Single(p => p.Name == "Key").Value);
                                    Assert.AreEqual("DerivedVehicleGPSInGPSDP", entry.Properties.Single(p => p.Name == "DisplayName").Value);
                                    Assert.AreEqual(entry.TypeName, string.Format("{0}.TruckDemo.DerivedVehicleGPSType", TestModelNameSpace));
                                }
                            }
                        }
                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                    }
                }
            }
        }

        #endregion

        #region Query Option
        [TestMethod]
        public void ModelReferenceWithExpandOption()
        {
            Dictionary<string, int[]> testCases = new Dictionary<string, int[]>()
            {
                { "Trucks('Key1')?$select=Key,VehicleGPS&$expand=VehicleGPS", new int[] {2, 1} },
                { "Trucks('Key1')?$expand=HeadUnit,VehicleGPS", new int[] {3, 4} },
            };

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var testCase in testCases)
            {
                foreach (var mimeType in mimeTypes)
                {
                    var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + testCase.Key, UriKind.Absolute));
                    requestMessage.SetHeader("Accept", mimeType);
                    var responseMessage = requestMessage.GetResponse();
                    Assert.AreEqual(200, responseMessage.StatusCode);

                    if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                    {
                        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                        {
                            List<ODataResource> entries = new List<ODataResource>();
                            List<ODataNestedResourceInfo> navigationLinks = new List<ODataNestedResourceInfo>();

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

                            Assert.AreEqual(ODataReaderState.Completed, reader.State);
                            Assert.AreEqual(testCase.Value[0], entries.Where(e=>e.TypeName.Contains("TruckType")
                                || e.TypeName.Contains("VehicleGPSType")
                                || e.TypeName.Contains("HeadUnitType")).Count());
                            Assert.AreEqual(testCase.Value[1], navigationLinks.Where(n => n.Url != null).Count());
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ModelReferneceWithFilterOption()
        {
            Dictionary<string, int> testCases = new Dictionary<string, int>()
            {
                { "VehicleGPSSet?$filter=VehicleSpeed gt 90", 3 },
                { "DerivedVehicleGPSSet?$filter=DisplayName eq 'DisplayName4'", 1 },
            };

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var testCase in testCases)
            {
                foreach (var mimeType in mimeTypes)
                {
                    var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + testCase.Key, UriKind.Absolute));
                    requestMessage.SetHeader("Accept", mimeType);
                    var responseMessage = requestMessage.GetResponse();
                    Assert.AreEqual(200, responseMessage.StatusCode);

                    if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                    {
                        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                        {
                            List<ODataResource> entries = new List<ODataResource>();

                            var reader = messageReader.CreateODataResourceSetReader();
                            while (reader.Read())
                            {
                                if (reader.State == ODataReaderState.ResourceEnd)
                                {
                                    var entry = reader.Item as ODataResource;
                                    if (entry.TypeName.Contains("VehicleGPSType"))
                                    {
                                        entries.Add(reader.Item as ODataResource);
                                    }
                                }
                            }
                            Assert.AreEqual(ODataReaderState.Completed, reader.State);
                            Assert.AreEqual(testCase.Value, entries.Count);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ModelReferenceWithSelectOption()
        {
            Dictionary<string, int> testCases = new Dictionary<string, int>()
            {
                { "Trucks('Key1')?$select=Key,FuelLevel", 2 },
                { "DerivedVehicleGPSSet('VehicleKey4')?$select=DisplayName,Map,StartLocation",  3},
                { "VehicleGPSSetInGPS('DerivedVehicleGPSInGPSKey3')/Microsoft.OData.SampleService.Models.ModelRefDemo.TruckDemo.DerivedVehicleGPSType?$select=DisplayName,Map,StartLocation", 3 }
            };

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var testCase in testCases)
            {
                foreach (var mimeType in mimeTypes)
                {
                    var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + testCase.Key, UriKind.Absolute));
                    requestMessage.SetHeader("Accept", mimeType);
                    var responseMessage = requestMessage.GetResponse();
                    Assert.AreEqual(200, responseMessage.StatusCode);

                    if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                    {
                        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                        {
                            List<ODataResource> entries = new List<ODataResource>();

                            var reader = messageReader.CreateODataResourceReader();
                            while (reader.Read())
                            {
                                if (reader.State == ODataReaderState.ResourceEnd)
                                {
                                    entries.Add(reader.Item as ODataResource);
                                }
                            }

                            Assert.AreEqual(ODataReaderState.Completed, reader.State);
                            Assert.AreEqual(testCase.Value, entries.Last().Properties.Count() + entries.Count -1 /*non-ComplexP + ComplexP*/);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void ModelReferenceWithOrderbyOption()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var mimeType in mimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(
                    new Uri(ServiceBaseUri.AbsoluteUri + "DerivedVehicleGPSSet?$orderby=DisplayName desc", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        List<ODataResource> entries = new List<ODataResource>();

                        var reader = messageReader.CreateODataResourceSetReader();
                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.ResourceEnd)
                            {
                                var resource = reader.Item as ODataResource;
                                if (resource.TypeName.Contains("VehicleGPSType"))
                                {
                                    entries.Add(reader.Item as ODataResource);
                                }
                            }
                        }

                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                        Assert.AreEqual("DisplayName5", entries[0].Properties.SingleOrDefault(p => p.Name == "DisplayName").Value);
                        Assert.AreEqual("DisplayName4", entries[1].Properties.SingleOrDefault(p => p.Name == "DisplayName").Value);
                    }
                }
            }
        }

        #endregion

        #region Action/Function

        [TestMethod]
        public void BoundActionInReferencedModel()
        {
            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "VehicleGPSSet('VehicleKey6')/Microsoft.OData.SampleService.Models.ModelRefDemo.GPS.ResetVehicleSpeed", UriKind.Absolute));
            requestMessage.SetHeader("Accept", "*/*");
            requestMessage.Method = "POST";

            ODataMessageWriterSettings writerSettings = new ODataMessageWriterSettings() { BaseUri = ServiceBaseUri };
            using (var messageWriter = new ODataMessageWriter(requestMessage, writerSettings, Model))
            {
                var odataWriter = messageWriter.CreateODataParameterWriter(null);
                odataWriter.WriteStart();
                odataWriter.WriteValue("targetValue", 80);
                odataWriter.WriteEnd();
            }
            var responseMessage = requestMessage.GetResponse();
            Assert.AreEqual(204, responseMessage.StatusCode);

            var actual = (this.QueryEntityItem("VehicleGPSSet('VehicleKey6')") as ODataResource).Properties.Single(p => p.Name == "VehicleSpeed").Value;
            Assert.AreEqual((double)80, actual);
        }

        [TestMethod]
        public void UnBoundActionForEntryInReferencedModel()
        {
            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "ResetVehicleSpeedToValue", UriKind.Absolute));
            requestMessage.SetHeader("Accept", "*/*");
            requestMessage.Method = "POST";

            ODataMessageWriterSettings writerSettings = new ODataMessageWriterSettings() { BaseUri = ServiceBaseUri };
            using (var messageWriter = new ODataMessageWriter(requestMessage, writerSettings, Model))
            {
                var odataWriter = messageWriter.CreateODataParameterWriter(null);
                odataWriter.WriteStart();
                odataWriter.WriteValue("targetValue", 80);
                odataWriter.WriteEnd();
            }
            var responseMessage = requestMessage.GetResponse();
            Assert.AreEqual(200, responseMessage.StatusCode);

            var actual = (this.QueryEntityItem("VehicleGPSSetInGPS('VehicleGPSSetInGPSKey2')") as ODataResource).Properties.Single(p => p.Name == "VehicleSpeed").Value;
            Assert.AreEqual((double)80, actual);
        }

        [TestMethod]
        public void BoundFunctionInReferencedModel()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "VehicleGPSSet('VehicleKey2')/Microsoft.OData.SampleService.Models.ModelRefDemo.GPS.GetVehicleSpeed()", UriKind.Absolute));
            requestMessage.SetHeader("Accept", "*/*");
            var responseMessage = requestMessage.GetResponse();
            Assert.AreEqual(200, responseMessage.StatusCode);

            using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
            {
                var amount = messageReader.ReadProperty().Value;
                Assert.AreEqual((double)120, amount);
            }
        }

        [TestMethod]
        public void UnBoundFunctionReturnTypeInReferencedModel()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "GetDefaultOutsideGeoFenceAlarm()", UriKind.Absolute));
            requestMessage.SetHeader("Accept", "*/*");
            var responseMessage = requestMessage.GetResponse();
            Assert.AreEqual(200, responseMessage.StatusCode);

            ODataResource perp = null;
            using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
            {
                var odataReader = messageReader.CreateODataResourceReader();
                while (odataReader.Read())
                {
                    if (odataReader.State == ODataReaderState.ResourceEnd)
                    {
                        perp = odataReader.Item as ODataResource;
                    }
                }

                Assert.AreEqual(1, perp.Properties.Single(p => p.Name == "Severity").Value);
            }
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

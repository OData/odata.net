//---------------------------------------------------------------------
// <copyright file="PluggableFormatServiceTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.ODataWCFServiceTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.PluggableFormat;
#if ENABLE_AVRO
    using Microsoft.Test.OData.PluggableFormat.Avro;
#endif
    using Microsoft.Test.OData.PluggableFormat.VCard;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.PluggableFormatServiceReference;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for pluggable format service
    /// </summary>
    [TestClass]
    public class PluggableFormatQueryTests : ODataWCFServiceTestsBase<PluggableFormatService>
    {
        public PluggableFormatQueryTests()
            : base(ServiceDescriptors.PluggableFormatServiceDescriptor)
        {
        }

        [TestMethod]
        public void QueryServiceDocument()
        {
            string[] types = new string[]
            {
                "text/html, application/xhtml+xml, */*",
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

                        Assert.IsNotNull(workSpace.EntitySets.Single(c => c.Name == "People"));
                    }
                }
            }
        }

        [TestMethod]
        public void QueryVCardEntityProperty()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings()
            {
                BaseUri = ServiceBaseUri,
                MediaTypeResolver = VCardMediaTypeResolver.Instance,
            };

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "People(31)/BusinessCard", UriKind.Absolute));
            requestMessage.SetHeader("Accept", "text/x-vCard");
            var responseMessage = requestMessage.GetResponse();
            Assert.AreEqual(200, responseMessage.StatusCode);

            ODataProperty property = null;
            using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
            {
                property = messageReader.ReadProperty();
            }

            var cpx = property.Value as ODataComplexValue;
            Assert.IsNotNull(cpx);
            Assert.AreEqual("Name1", cpx.Properties.Single(p => p.Name == "N").Value);
        }

#if ENABLE_AVRO
        [TestMethod]
        public void QueryAvroEntity()
        {
            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "People(31)", UriKind.Absolute));
            requestMessage.SetHeader("Accept", "avro/binary");
            var responseMessage = requestMessage.GetResponse();
            Assert.AreEqual(200, responseMessage.StatusCode);

            ODataEntry entry = null;
            using (var messageReader = new ODataMessageReader(responseMessage, GetAvroReaderSettings(), Model))
            {
                var reader = messageReader.CreateODataEntryReader();
                while (reader.Read())
                {
                    if (reader.State == ODataReaderState.EntryEnd)
                    {
                        entry = reader.Item as ODataEntry;
                    }
                }
            }

            Assert.IsNotNull(entry);
            ODataEntry expected = new ODataEntry
            {
                Properties = new[]
                {
                    new ODataProperty{  Name = "Id"          , Value = 31                   },
                    new ODataProperty{  Name = "Numbers"     , Value =
                        new ODataCollectionValue{ Items = new int[] { 3, 5, 7 } } },
                    new ODataProperty{  Name = "Picture"     , Value = new byte[] {5, 8}    },
                    new ODataProperty
                    {
                        Name = "BusinessCard", 
                        Value = new ODataComplexValue()
                        {
                            Properties = new []
                            {
                                new ODataProperty{  Name = "N"          , Value = "Name1"   },
                                new ODataProperty{  Name = "Tel_Home"   , Value = "01"      },
                            }
                        }
                    },
                },
            };

            Assert.IsTrue(TestHelper.EntryEqual(expected, entry));
        }

        [TestMethod]
        public void QueryAvroFeed()
        {
            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Products", UriKind.Absolute));
            requestMessage.SetHeader("Accept", "avro/binary");
            var responseMessage = requestMessage.GetResponse();
            Assert.AreEqual(200, responseMessage.StatusCode);

            IList<ODataEntry> entries = new List<ODataEntry>();
            using (var messageReader = new ODataMessageReader(responseMessage, GetAvroReaderSettings(), Model))
            {
                var reader = messageReader.CreateODataFeedReader();
                while (reader.Read())
                {
                    if (reader.State == ODataReaderState.EntryEnd)
                    {
                        entries.Add((ODataEntry)reader.Item);
                    }
                }
            }

            Assert.AreEqual(3, entries.Count);
            ODataEntry product2 = new ODataEntry
            {
                Properties = new[]
                {
                    new ODataProperty{  Name = "Id"     , Value = 2         },
                    new ODataProperty{  Name = "Name"   , Value = "Banana"  },
                    new ODataProperty
                    {
                        Name = "Info", 
                        Value = new ODataComplexValue()
                        {
                            Properties = new []
                            {
                                new ODataProperty{  Name = "Site"     , Value = "G2"      },
                                new ODataProperty{  Name = "Serial"   , Value = 1023L     },
                            }
                        }
                    },
                },
            };

            Assert.IsTrue(TestHelper.EntryEqual(product2, entries[1]));
        }

        [TestMethod]
        public void QueryWithAvroError()
        {
            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Products(-9)", UriKind.Absolute));
            requestMessage.SetHeader("Accept", "avro/binary");
            var responseMessage = requestMessage.GetResponse();
            
            // This is not an error case per standard, and no content should be returned. 
            Assert.AreEqual(204, responseMessage.StatusCode);
        }

        [TestMethod]
        public void InvokeAvroAction()
        {
            ODataEntry product1 = new ODataEntry
            {
                TypeName = "Microsoft.Test.OData.Services.PluggableFormat.Product",
                Properties = new[]
                {
                    new ODataProperty{  Name = "Id"     , Value = 1         },
                    new ODataProperty{  Name = "Name"   , Value = "Peach"   },
                    new ODataProperty
                    {
                        Name = "Info", 
                        Value = new ODataComplexValue()
                        {
                            TypeName = "Microsoft.Test.OData.Services.PluggableFormat.ProductInfo",
                            Properties = new []
                            {
                                new ODataProperty{  Name = "Site"     , Value = "G1"      },
                                new ODataProperty{  Name = "Serial"   , Value = 1024L     },
                            }
                        }
                    },
                },
            };

            IEdmModel model = new PluggableFormatService(null).Format.LoadServiceModel();
            IEdmOperation action = model.FindDeclaredOperations("Microsoft.Test.OData.Services.PluggableFormat.AddProduct").Single();

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Products/Microsoft.Test.OData.Services.PluggableFormat.AddProduct", UriKind.Absolute));
            requestMessage.Method = "POST";
            using (var mw = new ODataMessageWriter(requestMessage, GetAvroWriterSettings(), model))
            {
                var pw = mw.CreateODataParameterWriter(action);
                pw.WriteStart();
                var ew = pw.CreateEntryWriter("Value");
                {
                    ew.WriteStart(product1);
                    ew.WriteEnd();
                    ew.Flush();
                }

                pw.WriteValue("Override", true);
                pw.WriteEnd();
                pw.Flush();
            }

            var responseMessage = requestMessage.GetResponse();
            Assert.AreEqual(204, responseMessage.StatusCode);

            requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Products(1)", UriKind.Absolute));
            requestMessage.SetHeader("Accept", "avro/binary");
            responseMessage = requestMessage.GetResponse();
            ODataEntry entry = null;
            using (var messageReader = new ODataMessageReader(responseMessage, GetAvroReaderSettings(), Model))
            {
                var reader = messageReader.CreateODataEntryReader();
                while (reader.Read())
                {
                    if (reader.State == ODataReaderState.EntryEnd)
                    {
                        entry = reader.Item as ODataEntry;
                    }
                }
            }

            Assert.IsNotNull(entry);
            Assert.IsTrue(TestHelper.EntryEqual(product1, entry));
        }

        private ODataMessageReaderSettings GetAvroReaderSettings()
        {
            return new ODataMessageReaderSettings()
            {
                BaseUri = ServiceBaseUri,
                MediaTypeResolver = AvroMediaTypeResolver.Instance,
            };
        }

        private ODataMessageWriterSettings GetAvroWriterSettings()
        {
            var settings = new ODataMessageWriterSettings()
            {
                MediaTypeResolver = AvroMediaTypeResolver.Instance,
            };

            settings.SetContentType(AvroFormat.Avro);
            return settings;
        }
#endif
    }
}

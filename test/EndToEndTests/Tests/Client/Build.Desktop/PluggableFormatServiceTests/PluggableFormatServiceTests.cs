//---------------------------------------------------------------------
// <copyright file="PluggableFormatServiceTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.Test.OData.PluggableFormat;
#if ENABLE_AVRO
using Microsoft.Test.OData.PluggableFormat.Avro;
#endif
using Microsoft.Test.OData.PluggableFormat.VCard;
using Microsoft.Test.OData.DependencyInjection;
using Microsoft.Test.OData.Services.TestServices;
using Microsoft.Test.OData.Services.TestServices.PluggableFormatServiceReference;
using Microsoft.Test.OData.Tests.Client.Common;
using Xunit;

namespace Microsoft.Test.OData.Tests.Client.ODataWCFServiceTests
{
    /// <summary>
    /// Tests for pluggable format service
    /// </summary>
    public class PluggableFormatQueryTests : ODataWCFServiceTestsBase<PluggableFormatService>
    {
        public PluggableFormatQueryTests()
            : base(ServiceDescriptors.PluggableFormatServiceDescriptor)
        {
        }

        [Fact]
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
                Assert.Equal(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        ODataServiceDocument workSpace = messageReader.ReadServiceDocument();

                        Assert.NotNull(workSpace.EntitySets.Single(c => c.Name == "People"));
                    }
                }
            }
        }

#if ENABLE_AVRO
        [Fact]
        public void QueryVCardEntityProperty()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings()
            {
                BaseUri = ServiceBaseUri,
            };

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "People(31)/BusinessCard", UriKind.Absolute));
            SetVCardMediaTypeResolver(requestMessage);
            requestMessage.SetHeader("Accept", "text/x-vCard");
            var responseMessage = requestMessage.GetResponse();
            Assert.Equal(200, responseMessage.StatusCode);

            ODataResource resource = null;
            using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
            {
                var odataReader = messageReader.CreateODataResourceReader();
                while (odataReader.Read())
                {
                    if (odataReader.State == ODataReaderState.ResourceEnd)
                    {
                        resource = odataReader.Item as ODataResource;
                    }
                }
            }

            Assert.NotNull(resource);
            Assert.Equal("Name1", resource.Properties.Single(p => p.Name == "N").Value);
        }

        [Fact]
        public void QueryAvroEntity()
        {
            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "People(31)", UriKind.Absolute));
            SetAvroMediaTypeResolver(requestMessage);
            requestMessage.SetHeader("Accept", "avro/binary");
            var responseMessage = requestMessage.GetResponse();
            Assert.Equal(200, responseMessage.StatusCode);

            ODataResource entry = null;
            using (var messageReader = new ODataMessageReader(responseMessage, GetAvroReaderSettings(), Model))
            {
                var reader = messageReader.CreateODataResourceReader();
                while (reader.Read())
                {
                    if (reader.State == ODataReaderState.ResourceEnd)
                    {
                        entry = reader.Item as ODataResource;
                    }
                }
            }

            Assert.NotNull(entry);
            ODataResource expected = new ODataResource
            {
                Properties = new[]
                {
                    new ODataProperty{  Name = "Id"          , Value = 31                   },
                    new ODataProperty{  Name = "Numbers"     , Value =
                    new ODataCollectionValue{ Items = new object[] { 3, 5, 7 } } },
                    new ODataProperty{  Name = "Picture"     , Value = new byte[] {5, 8}    },
                },
            };

            ODataNestedResourceInfo businessCard_Info = new ODataNestedResourceInfo()
            {
                Name = "BusinessCard",
                IsCollection = false
            };

            ODataResource businessCard = new ODataResource
            {
                Properties = new[]
                {
                    new ODataProperty{  Name = "N"          , Value = "Name1"   },

                    // TODO : v7.0 Add support for open types
                    // new ODataProperty{  Name = "Tel_Home"   , Value = "01"      },
                }
            };

            Assert.True(TestHelper.EntryEqual(expected, entry));
        }

        [Fact]
        public void QueryAvroFeed()
        {
            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Products", UriKind.Absolute));
            SetAvroMediaTypeResolver(requestMessage);
            requestMessage.SetHeader("Accept", "avro/binary");
            var responseMessage = requestMessage.GetResponse();
            Assert.Equal(200, responseMessage.StatusCode);

            IList<ODataResource> entries = new List<ODataResource>();
            using (var messageReader = new ODataMessageReader(responseMessage, GetAvroReaderSettings(), Model))
            {
                var reader = messageReader.CreateODataResourceSetReader();
                while (reader.Read())
                {
                    if (reader.State == ODataReaderState.ResourceEnd)
                    {
                        entries.Add((ODataResource)reader.Item);
                    }
                }
            }

            Assert.Equal(5, entries.Count);
            ODataResource product2 = new ODataResource
            {
                Properties = new[]
                {
                    new ODataProperty{  Name = "Id"     , Value = 2         },
                    new ODataProperty{  Name = "Name"   , Value = "Banana"  },
                },
            };

            var info = new ODataResource()
            {
                Properties = new[]
                {
                    new ODataProperty{  Name = "Site"     , Value = "G2"      },
                    new ODataProperty{  Name = "Serial"   , Value = 1023L     },
                }
            };

            Assert.True(TestHelper.EntryEqual(info, entries[1]));
            Assert.True(TestHelper.EntryEqual(product2, entries[2]));
        }

        [Fact]
        public void QueryWithAvroError()
        {
            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Products(-9)", UriKind.Absolute));
            requestMessage.SetHeader("Accept", "avro/binary");
            var responseMessage = requestMessage.GetResponse();

            // This is not an error case per standard, and no content should be returned.
            Assert.Equal(204, responseMessage.StatusCode);
        }

        [Fact]
        public void InvokeAvroAction()
        {
            ODataResource product1 = new ODataResource
            {
                TypeName = "Microsoft.Test.OData.Services.PluggableFormat.Product",
                Properties = new[]
                {
                    new ODataProperty{  Name = "Id"     , Value = 1         },
                    new ODataProperty{  Name = "Name"   , Value = "Peach"   },
                },
            };

            ODataNestedResourceInfo info_nestedInfo = new ODataNestedResourceInfo()
            {
                Name = "Info",
                IsCollection = false
            };

            ODataResource info = new ODataResource()
            {
                TypeName = "Microsoft.Test.OData.Services.PluggableFormat.ProductInfo",
                Properties = new[]
                {
                    new ODataProperty{  Name = "Site"     , Value = "G1"      },
                    new ODataProperty{  Name = "Serial"   , Value = 1024L     },
                }
            };

            IEdmModel model = new PluggableFormatService(null).Format.LoadServiceModel();
            IEdmOperation action = model.FindDeclaredOperations("Microsoft.Test.OData.Services.PluggableFormat.AddProduct").Single();

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Products/Microsoft.Test.OData.Services.PluggableFormat.AddProduct", UriKind.Absolute));
            SetAvroMediaTypeResolver(requestMessage);
            requestMessage.Method = "POST";
            using (var mw = new ODataMessageWriter(requestMessage, GetAvroWriterSettings(), model))
            {
                var pw = mw.CreateODataParameterWriter(action);
                pw.WriteStart();
                var ew = pw.CreateResourceWriter("Value");
                {
                    ew.WriteStart(product1);
                    ew.WriteStart(info_nestedInfo);
                    ew.WriteStart(info);
                    ew.WriteEnd();
                    ew.WriteEnd();
                    ew.WriteEnd();
                    ew.Flush();
                }

                pw.WriteValue("Override", true);
                pw.WriteEnd();
                pw.Flush();
            }

            var responseMessage = requestMessage.GetResponse();
            Assert.Equal(204, responseMessage.StatusCode);

            requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Products(1)", UriKind.Absolute));
            SetAvroMediaTypeResolver(requestMessage);
            requestMessage.SetHeader("Accept", "avro/binary");
            responseMessage = requestMessage.GetResponse();
            ODataResource entry = null;
            using (var messageReader = new ODataMessageReader(responseMessage, GetAvroReaderSettings(), Model))
            {
                var reader = messageReader.CreateODataResourceReader();
                while (reader.Read())
                {
                    if (reader.State == ODataReaderState.ResourceEnd)
                    {
                        entry = reader.Item as ODataResource;
                    }
                }
            }

            Assert.NotNull(entry);
            Assert.True(TestHelper.EntryEqual(product1, entry));
        }

        private static void SetAvroMediaTypeResolver(HttpWebRequestMessage requestMessage)
        {
            requestMessage.Container = ContainerBuilderHelper.BuildContainer(builder =>
                builder.AddService<ODataMediaTypeResolver, AvroMediaTypeResolver>(ServiceLifetime.Singleton));
        }

        private static void SetVCardMediaTypeResolver(HttpWebRequestMessage requestMessage)
        {
            requestMessage.Container = ContainerBuilderHelper.BuildContainer(builder =>
                builder.AddService<ODataMediaTypeResolver, VCardMediaTypeResolver>(ServiceLifetime.Singleton));
        }

        private ODataMessageReaderSettings GetAvroReaderSettings()
        {
            return new ODataMessageReaderSettings()
            {
                BaseUri = ServiceBaseUri,
            };
        }

        private ODataMessageWriterSettings GetAvroWriterSettings()
        {
            var settings = new ODataMessageWriterSettings();

            settings.SetContentType(AvroFormat.Avro);
            return settings;
        }
#endif
    }
}

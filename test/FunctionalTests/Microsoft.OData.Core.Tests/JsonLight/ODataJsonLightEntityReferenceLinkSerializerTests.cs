//---------------------------------------------------------------------
// <copyright file="ODataJsonLightEntityReferenceLinkSerializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.JsonLight;
using Xunit;

namespace Microsoft.OData.Tests.JsonLight
{
    public class ODataJsonLightEntityReferenceLinkSerializerTests
    {
        private EdmModel model;
        private MemoryStream stream;
        private ODataMessageWriterSettings messageWriterSettings;

        public ODataJsonLightEntityReferenceLinkSerializerTests()
        {
            this.model = new EdmModel();
            this.stream = new MemoryStream();
            this.messageWriterSettings = new ODataMessageWriterSettings
            {
                EnableMessageStreamDisposal = false,
                Version = ODataVersion.V4
            };

            this.messageWriterSettings.SetServiceDocumentUri(new Uri("http://tempuri.org"));
        }

        [Fact]
        public async Task WriteEntityReferenceLinkAsync_WritesTopLevelUri()
        {
            ODataEntityReferenceLink entityReferenceLink = new ODataEntityReferenceLink
            {
                Url = new Uri("http://tempuri.org/Customers(1)")
            };

            var result = await SetupODataJsonLightEntityReferenceLinkSerializerAndRunTestAsync(
                (jsonLightEntityReferenceLinkSerializer) =>
                {
                    return jsonLightEntityReferenceLinkSerializer.WriteEntityReferenceLinkAsync(entityReferenceLink);
                });

            Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#$ref\"," +
                "\"@odata.id\":\"http://tempuri.org/Customers(1)\"}", result);
        }

        public static IEnumerable<object[]> GetWriteEntityReferenceLinksTestData()
        {
            var entityReferenceLinks = new List<ODataEntityReferenceLink>
            {
                new ODataEntityReferenceLink { Url = new Uri("http://tempuri.org/Customers(1)") },
                new ODataEntityReferenceLink { Url = new Uri("http://tempuri.org/Orders(1)") }
            };
            var template = "{{\"@odata.context\":\"http://tempuri.org/$metadata#Collection($ref)\",{0}" +
                "\"value\":[" +
                "{{\"@odata.id\":\"http://tempuri.org/Customers(1)\"}}," +
                "{{\"@odata.id\":\"http://tempuri.org/Orders(1)\"}}" +
                "]}}";

            // Entity reference links
            yield return new object[]
            {
                new ODataEntityReferenceLinks
                {
                    Links = entityReferenceLinks
                },
                string.Format(template, "")
            };

            // Entity reference links plus next page link
            yield return new object[]
            {
                new ODataEntityReferenceLinks
                {
                    Links = entityReferenceLinks,
                    NextPageLink = new Uri("http://tempuri.org/Customers?$skiptoken=Id-5")
                },
                string.Format(template, "\"@odata.nextLink\":\"http://tempuri.org/Customers?$skiptoken=Id-5\",")
            };

            // Entity reference links plus count
            yield return new object[]
            {
                new ODataEntityReferenceLinks
                {
                    Links = entityReferenceLinks,
                    Count = 10
                },
                string.Format(template, "\"@odata.count\":10,")
            };

            // Entity reference links plus next page link plus count
            yield return new object[]
            {
                new ODataEntityReferenceLinks
                {
                    Links = entityReferenceLinks,
                    NextPageLink = new Uri("http://tempuri.org/Customers?$skiptoken=Id-5"),
                    Count = 10
                },
                string.Format(
                    template,
                    "\"@odata.count\":10," +
                    "\"@odata.nextLink\":\"http://tempuri.org/Customers?$skiptoken=Id-5\",")
            };
        }

        [Theory]
        [MemberData(nameof(GetWriteEntityReferenceLinksTestData))]
        public async Task WriteEntityReferenceLinkAsync_WritesExpectedOutput(ODataEntityReferenceLinks entityReferenceLinks, string expected)
        {
            var result = await SetupODataJsonLightEntityReferenceLinkSerializerAndRunTestAsync(
                (jsonLightEntityReferenceLinkSerializer) =>
                {
                    return jsonLightEntityReferenceLinkSerializer.WriteEntityReferenceLinksAsync(entityReferenceLinks);
                });

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task WriteEntityReferenceLinksAsync_ExceptionThrownForNullEntityReferenceLink()
        {
            var entityReferenceLinks = new ODataEntityReferenceLinks
            {
                Links = new List<ODataEntityReferenceLink> { null }
            };

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupODataJsonLightEntityReferenceLinkSerializerAndRunTestAsync(
                    (jsonLightEntityReferenceLinkSerializer) =>
                    {
                        return jsonLightEntityReferenceLinkSerializer.WriteEntityReferenceLinksAsync(entityReferenceLinks);
                    }));

            Assert.Equal(Strings.WriterValidationUtils_EntityReferenceLinksLinkMustNotBeNull, exception.Message);
        }

        [Fact]
        public async Task WriteEntityReferenceLinkAsync_ExceptionThrownForNullUrlInEntityReferenceLink()
        {
            var entityReferenceLink = new ODataEntityReferenceLink
            {
                Url = null
            };

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupODataJsonLightEntityReferenceLinkSerializerAndRunTestAsync(
                    (jsonLightEntityReferenceLinkSerializer) =>
                    {
                        return jsonLightEntityReferenceLinkSerializer.WriteEntityReferenceLinkAsync(entityReferenceLink);
                    }));

            Assert.Equal(Strings.WriterValidationUtils_EntityReferenceLinkUrlMustNotBeNull, exception.Message);
        }

        private ODataJsonLightEntityReferenceLinkSerializer CreateODataJsonLightEntityReferenceLinkSerializer(
            bool writingResponse,
            IServiceProvider container = null,
            bool isAsync = false)
        {
            var messageInfo = new ODataMessageInfo
            {
                MessageStream = this.stream,
                MediaType = new ODataMediaType("application", "json"),
#if NETCOREAPP1_1
                Encoding = Encoding.GetEncoding(0),
#else
                Encoding = Encoding.Default,
#endif
                IsResponse = writingResponse,
                IsAsync = isAsync,
                Model = this.model,
                Container = container
            };

            var jsonLightOutputContext = new ODataJsonLightOutputContext(messageInfo, this.messageWriterSettings);
            return new ODataJsonLightEntityReferenceLinkSerializer(jsonLightOutputContext);
        }

        /// <summary>
        /// Sets up an ODataJsonLightEntityReferenceLinkSerializer,
        /// then runs the given test code asynchronously,
        /// then flushes and reads the stream back as a string for customized verification.
        /// </summary>
        private async Task<string> SetupODataJsonLightEntityReferenceLinkSerializerAndRunTestAsync(Func<ODataJsonLightEntityReferenceLinkSerializer, Task> func, IServiceProvider container = null, bool writingTopLevelCollection = false)
        {
            var jsonLightEntityReferenceLinkSerializer = CreateODataJsonLightEntityReferenceLinkSerializer(true, container, true);
            await func(jsonLightEntityReferenceLinkSerializer);
            await jsonLightEntityReferenceLinkSerializer.JsonLightOutputContext.FlushAsync();
            await jsonLightEntityReferenceLinkSerializer.AsynchronousJsonWriter.FlushAsync();

            this.stream.Position = 0;

            return await new StreamReader(this.stream).ReadToEndAsync();
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="ODataJsonLightOutputContextTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.OData.JsonLight;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.JsonLight
{
    public class ODataJsonLightOutputContextTests
    {
        #region WriteProperty
        [Fact]
        public void ShouldBeAbleToWritePropertyRequestWithoutModel()
        {
            ODataProperty property = new ODataProperty {Name = "Prop", Value = Guid.Empty};
            WriteAndValidate(outputContext => outputContext.WriteProperty(property), "{\"@odata.context\":\"http://odata.org/test/$metadata#Edm.Guid\",\"@odata.type\":\"#Guid\",\"value\":\"00000000-0000-0000-0000-000000000000\"}", writingResponse: false);
        }

        [Fact]
        public void ShouldBeAbleToWritePropertyResponseWithoutModel()
        {
            ODataProperty property = new ODataProperty { Name = "Prop", Value = Guid.Empty };
            WriteAndValidate(outputContext => outputContext.WriteProperty(property), "{\"@odata.context\":\"http://odata.org/test/$metadata#Edm.Guid\",\"value\":\"00000000-0000-0000-0000-000000000000\"}", writingResponse: true);
        }

        [Fact]
        public void ShouldBeAbleToWritePropertyRequestWithoutModelAsync()
        {
            ODataProperty property = new ODataProperty { Name = "Prop", Value = Guid.Empty };
            WriteAndValidate(outputContext => outputContext.WritePropertyAsync(property).Wait(), "{\"@odata.context\":\"http://odata.org/test/$metadata#Edm.Guid\",\"@odata.type\":\"#Guid\",\"value\":\"00000000-0000-0000-0000-000000000000\"}", writingResponse: false, synchronous: false);
        }

        [Fact]
        public void ShouldBeAbleToWritePropertyResponseWithoutModelAsync()
        {
            ODataProperty property = new ODataProperty { Name = "Prop", Value = Guid.Empty };
            WriteAndValidate(outputContext => outputContext.WritePropertyAsync(property).Wait(), "{\"@odata.context\":\"http://odata.org/test/$metadata#Edm.Guid\",\"@odata.type\":\"#Guid\",\"value\":\"00000000-0000-0000-0000-000000000000\"}", writingResponse: false, synchronous: false);
        }

        [Fact]
        public void ShouldBeAbleToWriteInstanceAnnotationsInRequest()
        {
            ODataProperty property = new ODataProperty()
            {
                Name = "Prop",
                Value = Guid.Empty,
                InstanceAnnotations = new Collection<ODataInstanceAnnotation>
                {
                    new ODataInstanceAnnotation("Annotation.1", new ODataPrimitiveValue(true)),
                    new ODataInstanceAnnotation("Annotation.2", new ODataPrimitiveValue(123))
                }
            };
            WriteAndValidate(outputContext => outputContext.WriteProperty(property), "{\"@odata.context\":\"http://odata.org/test/$metadata#Edm.Guid\",\"@Annotation.1\":true,\"@Annotation.2\":123,\"@odata.type\":\"#Guid\",\"value\":\"00000000-0000-0000-0000-000000000000\"}", writingResponse: false);
        }

        [Fact]
        public void ShouldBeAbleToWriteInstanceAnnotationsInResponse()
        {
            ODataProperty property = new ODataProperty()
            {
                Name = "Prop",
                Value = Guid.Empty,
                InstanceAnnotations = new Collection<ODataInstanceAnnotation>
                {
                    new ODataInstanceAnnotation("Annotation.1", new ODataPrimitiveValue(true)),
                    new ODataInstanceAnnotation("Annotation.2", new ODataPrimitiveValue(123))
                }
            };
            WriteAndValidate(outputContext => outputContext.WriteProperty(property), "{\"@odata.context\":\"http://odata.org/test/$metadata#Edm.Guid\",\"@Annotation.1\":true,\"@Annotation.2\":123,\"value\":\"00000000-0000-0000-0000-000000000000\"}");
        }

        #endregion WriteProperty

        #region CreateResourceSetWriter
        [Fact]
        public void ShouldBeAbleToCreateResourceSetWriterForRequestWithoutModelAndWithoutSet()
        {
            WriteAndValidate(outputContext => outputContext.CreateODataResourceSetWriter(entitySet:null, resourceType:null), "", writingResponse: false);
        }

        [Fact]
        public void ShouldBeAbleToCreateResourceSetWriterForResponseWithoutModelAndWithoutSet()
        {
            WriteAndValidate(outputContext => outputContext.CreateODataResourceSetWriter(entitySet: null, resourceType: null), "", writingResponse: true);
        }

        [Fact]
        public void ShouldBeAbleToCreateResourceSetWriterAsyncForRequestWithoutModelAndWithoutSet()
        {
            WriteAndValidate(outputContext => outputContext.CreateODataResourceSetWriterAsync(entitySet: null, resourceType: null).Result.Should().NotBeNull(), "", writingResponse: false, synchronous: false);
        }

        [Fact]
        public void ShouldBeAbleToCreateResourceSetWriterAsyncForResponseWithoutModelAndWithoutSet()
        {
            WriteAndValidate(outputContext => outputContext.CreateODataResourceSetWriterAsync(entitySet: null, resourceType: null).Result.Should().NotBeNull(), "", writingResponse: true, synchronous: false);
        }
        #endregion CreateResourceSetWriter

        #region CreateResourceWriter
        [Fact]
        public void ShouldBeAbleToCreateResourceWriterForRequestWithoutModelAndWithoutSet()
        {
            WriteAndValidate(outputContext => outputContext.CreateODataResourceWriter(navigationSource: null, resourceType: null), "", writingResponse: false);
        }

        [Fact]
        public void ShouldBeAbleToCreateResourceWriterForResponseWithoutModelAndWithoutSet()
        {
            WriteAndValidate(outputContext => outputContext.CreateODataResourceWriter(navigationSource: null, resourceType: null), "", writingResponse: true);
        }

        [Fact]
        public void ShouldBeAbleToCreateResourceWriterAsyncForRequestWithoutModelAndWithoutSet()
        {
            WriteAndValidate(outputContext => outputContext.CreateODataResourceWriterAsync(navigationSource: null, resourceType: null).Result.Should().NotBeNull(), "", writingResponse: false, synchronous: false);
        }

        [Fact]
        public void ShouldBeAbleToCreateResourceWriterAsyncForResponseWithoutModelAndWithoutSet()
        {
            WriteAndValidate(outputContext => outputContext.CreateODataResourceWriterAsync(navigationSource: null, resourceType: null).Result.Should().NotBeNull(), "", writingResponse: true, synchronous: false);
        }
        #endregion CreateResourceWriter

        #region CreateCollectionWriter
        [Fact]
        public void ShouldBeAbleToCreateCollectionWriterForRequestWithoutModelAndWithoutItemType()
        {
            WriteAndValidate(outputContext => outputContext.CreateODataCollectionWriter(itemTypeReference: null), "", writingResponse: false);
        }

        [Fact]
        public void ShouldBeAbleToCreateCollectionWriterForResponseWithoutModelAndWithoutItemType()
        {
            WriteAndValidate(outputContext => outputContext.CreateODataCollectionWriter(itemTypeReference: null), "", writingResponse: true);
        }

        [Fact]
        public void ShouldBeAbleToCreateCollectionWriterAsyncForRequestWithoutModelAndWithoutItemType()
        {
            WriteAndValidate(outputContext => outputContext.CreateODataCollectionWriterAsync(itemTypeReference: null).Result.Should().NotBeNull(), "", writingResponse: false, synchronous: false);
        }

        [Fact]
        public void ShouldBeAbleToCreateCollectionWriterAsyncForResponseWithoutModelAndWithoutItemType()
        {
            WriteAndValidate(outputContext => outputContext.CreateODataCollectionWriterAsync(itemTypeReference: null).Result.Should().NotBeNull(), "", writingResponse: true, synchronous: false);
        }
        #endregion CreateCollectionWriter

        #region CreateParameterWriter
        [Fact]
        public void ShouldBeAbleToCreateParameterWriterForRequestWithoutModelAndWithoutFunction()
        {
            WriteAndValidate(outputContext => outputContext.CreateODataParameterWriter(operation: null), "", writingResponse: false);
        }

        [Fact]
        public void ShouldBeAbleToCreateParameterWriterForResponseWithoutModelAndWithoutFunction()
        {
            WriteAndValidate(outputContext => outputContext.CreateODataParameterWriter(operation: null), "", writingResponse: true);
        }

        [Fact]
        public void ShouldBeAbleToCreateParameterWriterAsyncForRequestWithoutModelAndWithoutFunction()
        {
            WriteAndValidate(outputContext => outputContext.CreateODataParameterWriterAsync(operation: null).Result.Should().NotBeNull(), "", writingResponse: false, synchronous: false);
        }

        [Fact]
        public void ShouldBeAbleToCreateParameterWriterAsyncForResponseWithoutModelAndWithoutFunction()
        {
            WriteAndValidate(outputContext => outputContext.CreateODataParameterWriterAsync(operation: null).Result.Should().NotBeNull(), "", writingResponse: true, synchronous: false);
        }
        #endregion CreateParameterWriter

        #region WriteServiceDocument
        [Fact]
        public void ShouldWriteServiceDocumentWithoutModel()
        {
            ODataServiceDocument serviceDocument = new ODataServiceDocument();
            serviceDocument.EntitySets = new ODataEntitySetInfo[] {new ODataEntitySetInfo {Name = "Customers", Url = new Uri("http://host/Customers")}};
            WriteAndValidate(outputContext => outputContext.WriteServiceDocument(serviceDocument), "{\"@odata.context\":\"http://odata.org/test/$metadata\",\"value\":[{\"name\":\"Customers\",\"kind\":\"EntitySet\",\"url\":\"http://host/Customers\"}]}");
        }

        [Fact]
        public void ShouldWriteServiceDocumentAsyncWithoutModel()
        {
            ODataServiceDocument serviceDocument = new ODataServiceDocument();
            serviceDocument.EntitySets = new ODataEntitySetInfo[] { new ODataEntitySetInfo { Name = "Customers", Url = new Uri("http://host/Customers") } };
            WriteAndValidate(outputContext => outputContext.WriteServiceDocumentAsync(serviceDocument).Wait(), "{\"@odata.context\":\"http://odata.org/test/$metadata\",\"value\":[{\"name\":\"Customers\",\"kind\":\"EntitySet\",\"url\":\"http://host/Customers\"}]}", writingResponse: true, synchronous: false);
        }
        #endregion WriteServiceDocument

        #region WriteEntityReferenceLink
        #region sync
        [Fact]
        public void ShouldWriteContextUriForEntityReferenceLinkRequest()
        {
            ODataEntityReferenceLink referenceLink = new ODataEntityReferenceLink { Url = new Uri("http://host/Customers(1)") };
            WriteAndValidate(outputContext => outputContext.WriteEntityReferenceLink(referenceLink), "{\"@odata.context\":\"http://odata.org/test/$metadata#$ref\",\"@odata.id\":\"http://host/Customers(1)\"}", writingResponse: false);
            WriteAndValidate(outputContext => outputContext.WriteEntityReferenceLink(referenceLink), "{\"@odata.context\":\"http://odata.org/test/$metadata#$ref\",\"@odata.id\":\"http://host/Customers(1)\"}", writingResponse: true);
        }
        #endregion sync

        #region async
        [Fact]
        public void AsyncShouldWriteContextUriForEntityReferenceLinkRequest()
        {
            ODataEntityReferenceLink referenceLink = new ODataEntityReferenceLink { Url = new Uri("http://host/Customers(1)") };
            WriteAndValidate(outputContext => outputContext.WriteEntityReferenceLinkAsync(referenceLink).Wait(), "{\"@odata.context\":\"http://odata.org/test/$metadata#$ref\",\"@odata.id\":\"http://host/Customers(1)\"}", writingResponse: false, synchronous: false);
            WriteAndValidate(outputContext => outputContext.WriteEntityReferenceLinkAsync(referenceLink).Wait(), "{\"@odata.context\":\"http://odata.org/test/$metadata#$ref\",\"@odata.id\":\"http://host/Customers(1)\"}", writingResponse: true, synchronous: false);
        }
        #endregion async
        #endregion WriteEntityReferenceLink

        #region WriteEntityReferenceLinks
        #region sync
        [Fact]
        public void ShouldWriteContextUriForEntityReferenceLinksRequest()
        {
            ODataEntityReferenceLink referenceLink = new ODataEntityReferenceLink { Url = new Uri("http://host/Orders(1)") };
            ODataEntityReferenceLinks referenceLinks = new ODataEntityReferenceLinks { Links = new[] { referenceLink } };
            WriteAndValidate(outputContext => outputContext.WriteEntityReferenceLinks(referenceLinks), "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection($ref)\",\"value\":[{\"@odata.id\":\"http://host/Orders(1)\"}]}", writingResponse: false);
            WriteAndValidate(outputContext => outputContext.WriteEntityReferenceLinks(referenceLinks), "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection($ref)\",\"value\":[{\"@odata.id\":\"http://host/Orders(1)\"}]}", writingResponse: true);
        }

        [Fact]
        public void ShouldWriteNextLinkAnnotationForEntityReferenceLinksRequest()
        {
            ODataEntityReferenceLink referenceLink = new ODataEntityReferenceLink { Url = new Uri("http://host/Orders(1)") };
            ODataEntityReferenceLinks referenceLinks = new ODataEntityReferenceLinks
            {
                Links = new[] { referenceLink },
                NextPageLink = new Uri("http://odata.org/nextpage")
            };
            WriteAndValidate(outputContext => outputContext.WriteEntityReferenceLinks(referenceLinks),
                "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection($ref)\",\"@odata.nextLink\":\"http://odata.org/nextpage\",\"value\":[{\"@odata.id\":\"http://host/Orders(1)\"}]}",
                writingResponse: true);
            WriteAndValidate(outputContext => outputContext.WriteEntityReferenceLinks(referenceLinks),
                "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection($ref)\",\"@odata.nextLink\":\"http://odata.org/nextpage\",\"value\":[{\"@odata.id\":\"http://host/Orders(1)\"}]}",
                writingResponse: true);
        }

        [Fact]
        public void ShouldWriteCountAnnotationForEntityReferenceLinksRequest()
        {
            ODataEntityReferenceLink referenceLink = new ODataEntityReferenceLink { Url = new Uri("http://host/Orders(1)") };
            ODataEntityReferenceLinks referenceLinks = new ODataEntityReferenceLinks
            {
                Links = new[] { referenceLink },
                Count = 1,
                NextPageLink = new Uri("http://odata.org/nextpage")
            };
            WriteAndValidate(outputContext => outputContext.WriteEntityReferenceLinks(referenceLinks),
                "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection($ref)\",\"@odata.count\":1,\"@odata.nextLink\":\"http://odata.org/nextpage\",\"value\":[{\"@odata.id\":\"http://host/Orders(1)\"}]}",
                writingResponse: true);
            WriteAndValidate(outputContext => outputContext.WriteEntityReferenceLinks(referenceLinks),
                "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection($ref)\",\"@odata.count\":1,\"@odata.nextLink\":\"http://odata.org/nextpage\",\"value\":[{\"@odata.id\":\"http://host/Orders(1)\"}]}",
                writingResponse: true);
        }
        #endregion sync

        #region async
        [Fact]
        public void AsyncShouldWriteContextUriForEntityReferenceLinksRequest()
        {
            ODataEntityReferenceLink referenceLink = new ODataEntityReferenceLink { Url = new Uri("http://host/Orders(1)") };
            ODataEntityReferenceLinks referenceLinks = new ODataEntityReferenceLinks { Links = new[] { referenceLink } };
            WriteAndValidate(outputContext => outputContext.WriteEntityReferenceLinksAsync(referenceLinks).Wait(), "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection($ref)\",\"value\":[{\"@odata.id\":\"http://host/Orders(1)\"}]}", writingResponse: false, synchronous: false);
            WriteAndValidate(outputContext => outputContext.WriteEntityReferenceLinksAsync(referenceLinks).Wait(), "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection($ref)\",\"value\":[{\"@odata.id\":\"http://host/Orders(1)\"}]}", writingResponse: true, synchronous: false);
        }

        [Fact]
        public void AsyncWriteNextLinkAnnotationForEntityReferenceLinksRequest()
        {
            ODataEntityReferenceLink referenceLink = new ODataEntityReferenceLink { Url = new Uri("http://host/Orders(1)") };
            ODataEntityReferenceLinks referenceLinks = new ODataEntityReferenceLinks
            {
                Links = new[] { referenceLink },
                NextPageLink = new Uri("http://odata.org/nextpage")
            };
            WriteAndValidate(outputContext => outputContext.WriteEntityReferenceLinksAsync(referenceLinks).Wait(),
                "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection($ref)\",\"@odata.nextLink\":\"http://odata.org/nextpage\",\"value\":[{\"@odata.id\":\"http://host/Orders(1)\"}]}", writingResponse: false, synchronous: false);
            WriteAndValidate(outputContext => outputContext.WriteEntityReferenceLinksAsync(referenceLinks).Wait(),
                "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection($ref)\",\"@odata.nextLink\":\"http://odata.org/nextpage\",\"value\":[{\"@odata.id\":\"http://host/Orders(1)\"}]}", writingResponse: true, synchronous: false);
        }

        [Fact]
        public void AsyncShouldWriteCountAnnotationForEntityReferenceLinksRequest()
        {
            ODataEntityReferenceLink referenceLink = new ODataEntityReferenceLink { Url = new Uri("http://host/Orders(1)") };
            ODataEntityReferenceLinks referenceLinks = new ODataEntityReferenceLinks
            {
                Links = new[] { referenceLink },
                Count = 1,
                NextPageLink = new Uri("http://odata.org/nextpage")
            };
            WriteAndValidate(outputContext => outputContext.WriteEntityReferenceLinksAsync(referenceLinks).Wait(),
                "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection($ref)\",\"@odata.count\":1,\"@odata.nextLink\":\"http://odata.org/nextpage\",\"value\":[{\"@odata.id\":\"http://host/Orders(1)\"}]}", writingResponse: false, synchronous: false);
            WriteAndValidate(outputContext => outputContext.WriteEntityReferenceLinksAsync(referenceLinks).Wait(),
                "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection($ref)\",\"@odata.count\":1,\"@odata.nextLink\":\"http://odata.org/nextpage\",\"value\":[{\"@odata.id\":\"http://host/Orders(1)\"}]}", writingResponse: true, synchronous: false);
        }
        #endregion async
        #endregion WriteEntityReferenceLinks

        private static void WriteAndValidate(Action<ODataJsonLightOutputContext> test, string expectedPayload, bool writingResponse = true, bool synchronous = true)
        {
            MemoryStream stream = new MemoryStream();
            var outputContext = CreateJsonLightOutputContext(stream, writingResponse, synchronous);
            test(outputContext);
            ValidateWrittenPayload(stream, expectedPayload);
        }

        private static void ValidateWrittenPayload(MemoryStream stream, string expectedPayload)
        {
            stream.Seek(0, SeekOrigin.Begin);
            string payload = (new StreamReader(stream)).ReadToEnd();
            payload.Should().Be(expectedPayload);
        }

        private static ODataJsonLightOutputContext CreateJsonLightOutputContext(MemoryStream stream, bool writingResponse = true, bool synchronous = true)
        {
            var messageInfo = new ODataMessageInfo
            {
                MessageStream = new NonDisposingStream(stream),
                MediaType = new ODataMediaType("application", "json"),
                Encoding = Encoding.UTF8,
                IsResponse = writingResponse,
                IsAsync = !synchronous,
                Model = EdmCoreModel.Instance
            };

            var settings = new ODataMessageWriterSettings { Version = ODataVersion.V4 };
            settings.SetServiceDocumentUri(new Uri("http://odata.org/test"));
            settings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");

            return new ODataJsonLightOutputContext(messageInfo, settings);
        }
    }
}

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
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Microsoft.OData.JsonLight;
using Microsoft.Spatial;
using Microsoft.Test.OData.DependencyInjection;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.JsonLight
{
    public class ODataJsonLightOutputContextTests
    {
        private const string ServiceUri = "http://tempuri.org";
        private EdmModel model;
        private MemoryStream stream;
        private ODataMessageWriterSettings messageWriterSettings;

        private EdmEntityType orderEntityType;
        private EdmEntityType customerEntityType;
        private EdmEntitySet orderEntitySet;
        private EdmEntitySet customerEntitySet;

        private ODataError nullReferenceError;

        public ODataJsonLightOutputContextTests()
        {
            InitializeEdmModel();
            this.stream = new MemoryStream();
            this.messageWriterSettings = new ODataMessageWriterSettings { Version = ODataVersion.V4 };
            this.messageWriterSettings.SetServiceDocumentUri(new Uri(ServiceUri));
            this.messageWriterSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");
            this.nullReferenceError = new ODataError
            {
                ErrorCode = "NRE",
                Message = "Object reference not set to an instance of an object",
                InstanceAnnotations = new List<ODataInstanceAnnotation>
                {
                    new ODataInstanceAnnotation("Is.Error", new ODataPrimitiveValue(true))
                }
            };
        }

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
        public void ShouldBeAbleToWrite6xNullPropertyResponseWithoutModel()
        {
            ODataProperty property = new ODataProperty { Name = "Prop", Value = null };
            WriteAndValidate(outputContext => outputContext.WriteProperty(property), "{\"@odata.context\":\"http://odata.org/test/$metadata#Edm.Null\",\"@odata.null\":true}", writingResponse: true, use6x: true);
        }

        [Fact]
        public void ThrowsOnWriteNullPropertyResponseWithoutModel()
        {
            ODataProperty property = new ODataProperty { Name = "Prop", Value = null };
            Action test = () => WriteAndValidate(outputContext => outputContext.WriteProperty(property), "", writingResponse: true);
            test.Throws<ODataException>(ODataErrorStrings.ODataMessageWriter_CannotWriteTopLevelNull);
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
        #endregion CreateParameterWriter

        #region WriteServiceDocument
        [Fact]
        public void ShouldWriteServiceDocumentWithoutModel()
        {
            ODataServiceDocument serviceDocument = new ODataServiceDocument();
            serviceDocument.EntitySets = new ODataEntitySetInfo[] {new ODataEntitySetInfo {Name = "Customers", Url = new Uri("http://host/Customers")}};
            WriteAndValidate(outputContext => outputContext.WriteServiceDocument(serviceDocument), "{\"@odata.context\":\"http://odata.org/test/$metadata\",\"value\":[{\"name\":\"Customers\",\"kind\":\"EntitySet\",\"url\":\"http://host/Customers\"}]}");
        }
        #endregion WriteServiceDocument

        #region WriteEntityReferenceLink
        [Fact]
        public void ShouldWriteContextUriForEntityReferenceLinkRequest()
        {
            ODataEntityReferenceLink referenceLink = new ODataEntityReferenceLink { Url = new Uri("http://host/Customers(1)") };
            WriteAndValidate(outputContext => outputContext.WriteEntityReferenceLink(referenceLink), "{\"@odata.context\":\"http://odata.org/test/$metadata#$ref\",\"@odata.id\":\"http://host/Customers(1)\"}", writingResponse: false);
            WriteAndValidate(outputContext => outputContext.WriteEntityReferenceLink(referenceLink), "{\"@odata.context\":\"http://odata.org/test/$metadata#$ref\",\"@odata.id\":\"http://host/Customers(1)\"}", writingResponse: true);
        }
        #endregion WriteEntityReferenceLink

        #region WriteEntityReferenceLinks
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
        #endregion WriteEntityReferenceLinks

        #region WriteODataPrefix

        [Theory]
        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        public void ShouldDefaultToWritingPrefix40(bool isResponse)
        {
            var outputContext = CreateJsonLightOutputContext(new MemoryStream(), isResponse);
            Assert.False(outputContext.OmitODataPrefix);
        }

        [Theory]
        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        public void ShouldDefaultToOmitPrefix401(bool isResponse)
        {
            var outputContext = CreateJsonLightOutputContext(new MemoryStream(), isResponse, true, false, ODataVersion.V401);
            Assert.True(outputContext.OmitODataPrefix);
        }

        #endregion

        #region Async Tests

        [Fact]
        public async Task WriteResourceSetAsync()
        {
            var result = await SetupJsonLightOutputContextAndRunTestAsync(
                async (jsonLightOutputContext) =>
                {
                    var resourceSetWriter = await jsonLightOutputContext.CreateODataResourceSetWriterAsync(this.orderEntitySet, this.orderEntityType);
                    var resourceSet = CreateOrderResourceSet();

                    await resourceSetWriter.WriteStartAsync(resourceSet);
                    await resourceSetWriter.WriteEndAsync();
                });

            Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Orders\",\"value\":[]}", result);
        }

        [Fact]
        public async Task WriteResourceAsync()
        {
            var result = await SetupJsonLightOutputContextAndRunTestAsync(
                async (jsonLightOutputContext) =>
                {
                    var resourceWriter = await jsonLightOutputContext.CreateODataResourceWriterAsync(this.orderEntitySet, this.orderEntityType);
                    var resource = CreateOrderResource();

                    await resourceWriter.WriteStartAsync(resource);
                    await resourceWriter.WriteEndAsync();
                });

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$entity\",\"Id\":1,\"Amount\":13}",
                result);
        }

        [Fact]
        public async Task WriteCollectionAsync()
        {
            var result = await SetupJsonLightOutputContextAndRunTestAsync(
                async (jsonLightOutputContext) =>
                {
                    var collectionWriter = await jsonLightOutputContext.CreateODataCollectionWriterAsync(EdmCoreModel.Instance.GetString(false));
                    var collectionStart = new ODataCollectionStart
                    {
                        SerializationInfo = new ODataCollectionStartSerializationInfo
                        {
                            CollectionTypeName = "Collection(Edm.String)"
                        }
                    };

                    await collectionWriter.WriteStartAsync(collectionStart);
                    await collectionWriter.WriteItemAsync("Violet");
                    await collectionWriter.WriteItemAsync("Indigo");
                    await collectionWriter.WriteItemAsync("Blue");
                    await collectionWriter.WriteEndAsync();
                });

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Collection(Edm.String)\",\"value\":[\"Violet\",\"Indigo\",\"Blue\"]}",
                result);
        }

        [Fact]
        public async Task WriteParameterAsync()
        {
            var result = await SetupJsonLightOutputContextAndRunTestAsync(
                async (jsonLightOutputContext) =>
                {
                    var rateCustomerAction = new EdmAction("NS", "RateCustomer", EdmCoreModel.Instance.GetInt32(false));
                    rateCustomerAction.AddParameter("customerId", EdmCoreModel.Instance.GetInt32(false));
                    
                    var parameterWriter = await jsonLightOutputContext.CreateODataParameterWriterAsync(rateCustomerAction);

                    await parameterWriter.WriteStartAsync();
                    await parameterWriter.WriteValueAsync("customerId", 1);
                    await parameterWriter.WriteEndAsync();
                },
                /*writingResponse*/ false);

            Assert.Equal("{\"customerId\":1}", result);
        }

        [Fact]
        public async Task WriteResourceSetUriParameterAsync()
        {
            var result = await SetupJsonLightOutputContextAndRunTestAsync(
                async (jsonLightOutputContext) =>
                {
                    var resourceSetUriParameterWriter = await jsonLightOutputContext.CreateODataUriParameterResourceSetWriterAsync(
                        this.orderEntitySet, this.orderEntityType);
                    var orderResourceSet = CreateOrderResourceSet();
                    var orderResource = CreateOrderResource();

                    await resourceSetUriParameterWriter.WriteStartAsync(orderResourceSet);
                    await resourceSetUriParameterWriter.WriteStartAsync(orderResource);
                    await resourceSetUriParameterWriter.WriteEndAsync();
                    await resourceSetUriParameterWriter.WriteEndAsync();
                },
                /*writingResponse*/ false);

            Assert.Equal("[{\"Id\":1,\"Amount\":13}]", result);
        }

        [Fact]
        public async Task WriteResourceUriParameterAsync()
        {
            var result = await SetupJsonLightOutputContextAndRunTestAsync(
                async (jsonLightOutputContext) =>
                {
                    var resourceUriParameterWriter = await jsonLightOutputContext.CreateODataUriParameterResourceWriterAsync(
                        this.orderEntitySet, this.orderEntityType);
                    var orderResource = CreateOrderResource();

                    await resourceUriParameterWriter.WriteStartAsync(orderResource);
                    await resourceUriParameterWriter.WriteEndAsync();
                },
                /*writingResponse*/ false);

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$entity\",\"Id\":1,\"Amount\":13}",
                result);
        }

        [Fact]
        public async Task WriteDeltaResourceSetAsync()
        {
            var result = await SetupJsonLightOutputContextAndRunTestAsync(
                async (jsonLightOutputContext) =>
                {
                    var deltaResourceSetWriter = await jsonLightOutputContext.CreateODataDeltaResourceSetWriterAsync(this.orderEntitySet, this.orderEntityType);
                    var deltaResourceSet = CreateOrderDeltaResourceSet();

                    await deltaResourceSetWriter.WriteStartAsync(deltaResourceSet);
                    await deltaResourceSetWriter.WriteEndAsync();
                });

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$delta\"," +
                "\"@odata.deltaLink\":\"http://tempuri.org/Orders/deltaLink\",\"value\":[]}",
                result);
        }

        [Fact]
        public async Task WriteDeltaResourceAsync()
        {
            var result = await SetupJsonLightOutputContextAndRunTestAsync(
                async (jsonLightOutputContext) =>
                {
                    var deltaResourceWriter = await jsonLightOutputContext.CreateODataDeltaWriterAsync(this.orderEntitySet, this.orderEntityType);
                    var orderDeltaResourceSet = CreateOrderDeltaResourceSet();
                    var deltaResource = CreateOrderResource();

                    await deltaResourceWriter.WriteStartAsync(orderDeltaResourceSet);
                    await deltaResourceWriter.WriteStartAsync(deltaResource);
                    await deltaResourceWriter.WriteEndAsync();
                    await deltaResourceWriter.WriteEndAsync();
                });

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$delta\"," +
                "\"@odata.deltaLink\":\"http://tempuri.org/Orders/deltaLink\"," +
                "\"value\":[{\"Id\":1,\"Amount\":13}]}",
                result);
        }

        [Fact]
        public async Task WriteBatchAsync()
        {
            var result = await SetupJsonLightOutputContextAndRunTestAsync(
                async (jsonLightOutputContext) =>
                {
                    var batchWriter = await jsonLightOutputContext.CreateODataBatchWriterAsync();

                    await batchWriter.WriteStartBatchAsync();
                    var operationRequestMessage = await batchWriter.CreateOperationRequestMessageAsync(
                        "POST", new Uri($"{ServiceUri}/Orders"), "1");

                    using (var messageWriter = new ODataMessageWriter(operationRequestMessage))
                    {
                        var resourceWriter = await messageWriter.CreateODataResourceWriterAsync(this.orderEntitySet, this.orderEntityType);
                        var orderResource = CreateOrderResource();

                        await resourceWriter.WriteStartAsync(orderResource);
                        await resourceWriter.WriteEndAsync();
                    }

                    await batchWriter.WriteEndBatchAsync();
                },
                /*writingResponse*/ false);

            Assert.Equal(
                "{\"requests\":[{" +
                "\"id\":\"1\"," +
                "\"method\":\"POST\"," +
                "\"url\":\"http://tempuri.org/Orders\"," +
                "\"headers\":{\"odata-version\":\"4.0\",\"content-type\":\"application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\"}, " +
                "\"body\" :{\"Id\":1,\"Amount\":13}}]}",
                result);
        }

        [Fact]
        public async Task WritePropertyAsync()
        {
            var property = new ODataProperty
            {
                Name = "Prop",
                Value = 13,
                InstanceAnnotations = new List<ODataInstanceAnnotation>
                {
                    new ODataInstanceAnnotation("Is.LuckyNumber", new ODataPrimitiveValue(true))
                }
            };

            var result = await SetupJsonLightOutputContextAndRunTestAsync(
                (jsonLightOutputContext) => jsonLightOutputContext.WritePropertyAsync(property));

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Edm.Int32\",\"@Is.LuckyNumber\":true,\"value\":13}",
                result);
        }

        [Fact]
        public async Task WriteSpatialPropertyAsync()
        {
            var geographyValue = GeographyFactory.Point(32.0, -100.0).Build();

            var geographyProperty = new ODataProperty
            {
                Name = "GeographyProperty",
                Value = new ODataPrimitiveValue(geographyValue)
            };

            var result = await SetupJsonLightOutputContextAndRunTestAsync(
                (jsonLightOutputContext) => jsonLightOutputContext.WritePropertyAsync(geographyProperty));

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Edm.GeographyPoint\"," +
                "\"value\":{\"type\":\"Point\",\"coordinates\":[-100.0,32.0],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}",
                result);
        }

        [Fact]
        public async Task WriteSpatialCollectionPropertyAsync()
        {
            var geographyCollection = new object[]
            {
                GeographyFactory.Collection().Point(-19.99, -12.0).Build(),
                GeographyFactory.LineString(33.1, -110.0).LineTo(35.97, -110).Build(),
                GeographyFactory.MultiLineString().LineString(10.2, 11.2).LineTo(11.9, 11.6).LineString(16.2, 17.2).LineTo(18.9, 19.6).Build(),
                GeographyFactory.MultiPoint().Point(10.2, 11.2).Point(11.9, 11.6).Build(),
                GeographyFactory.MultiPolygon().Polygon().Ring(10.2, 11.2).LineTo(11.9, 11.6).LineTo(11.45, 87.75).Ring(16.2, 17.2).LineTo(18.9, 19.6).LineTo(11.45, 87.75).Build(),
                GeographyFactory.Point(33.1, -110.0).Build(),
                GeographyFactory.Polygon().Ring(33.1, -110.0).LineTo(35.97, -110.15).LineTo(11.45, 87.75).Ring(35.97, -110).LineTo(36.97, -110.15).LineTo(45.23, 23.18).Build(),
                GeographyFactory.Point(32.0, -100.0).Build()
            };

            var geographyCollectionProperty = new ODataProperty
            {
                Name = "GeographyCollectionProperty",
                Value = new ODataCollectionValue
                {
                    TypeName = "Collection(Edm.Geography)",
                    Items = geographyCollection
                }
            };

            var result = await SetupJsonLightOutputContextAndRunTestAsync(
                (jsonLightOutputContext) => jsonLightOutputContext.WritePropertyAsync(geographyCollectionProperty));

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Collection(Edm.Geography)\"," +
                "\"value\":[" +
                "{\"type\":\"GeometryCollection\",\"geometries\":[{\"type\":\"Point\",\"coordinates\":[-12.0,-19.99]}],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}," +
                "{\"type\":\"LineString\",\"coordinates\":[[-110.0,33.1],[-110.0,35.97]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}," +
                "{\"type\":\"MultiLineString\",\"coordinates\":[[[11.2,10.2],[11.6,11.9]],[[17.2,16.2],[19.6,18.9]]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}," +
                "{\"type\":\"MultiPoint\",\"coordinates\":[[11.2,10.2],[11.6,11.9]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}," +
                "{\"type\":\"MultiPolygon\",\"coordinates\":[[[[11.2,10.2],[11.6,11.9],[87.75,11.45],[11.2,10.2]],[[17.2,16.2],[19.6,18.9],[87.75,11.45],[17.2,16.2]]]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}," +
                "{\"type\":\"Point\",\"coordinates\":[-110.0,33.1],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}," +
                "{\"type\":\"Polygon\",\"coordinates\":[[[-110.0,33.1],[-110.15,35.97],[87.75,11.45],[-110.0,33.1]],[[-110.0,35.97],[-110.15,36.97],[23.18,45.23],[-110.0,35.97]]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}," +
                "{\"type\":\"Point\",\"coordinates\":[-100.0,32.0],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}" +
                "]}",
                result);
        }

        [Fact]
        public async Task WriteSpatialCollectionPropertyForInjectedJsonWriterFactoryAsync()
        {
            var messageInfo = CreateMessageInfo(this.model, /*synchronous*/ true, /*writingResponse*/ true);
            messageInfo.Container = ContainerBuilderHelper.BuildContainer(builder =>
            {
                builder.AddService<IJsonWriterFactory>(ServiceLifetime.Singleton, _ => new DefaultJsonWriterFactory());
                builder.AddService<IJsonWriterFactoryAsync>(ServiceLifetime.Singleton, _ => new DefaultJsonWriterFactory());
            });

            var jsonLightOutputContext = new ODataJsonLightOutputContext(messageInfo, this.messageWriterSettings);
            var geographyCollection = new object[]
            {
                GeographyFactory.Point(33.1, -110.0).Build(),
                GeographyFactory.LineString(33.1, -110.0).LineTo(35.97, -110).Build(),
                GeographyFactory.MultiPoint().Point(10.2, 11.2).Point(11.9, 11.6).Build()
            };

            var geographyCollectionProperty = new ODataProperty
            {
                Name = "GeographyCollectionProperty",
                Value = new ODataCollectionValue
                {
                    TypeName = "Collection(Edm.Geography)",
                    Items = geographyCollection
                }
            };

            await jsonLightOutputContext.WritePropertyAsync(geographyCollectionProperty);
            await jsonLightOutputContext.FlushAsync();

            this.stream.Position = 0;
            var result = await new StreamReader(this.stream).ReadToEndAsync();

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Collection(Edm.Geography)\"," +
                "\"value\":[" +
                "{\"type\":\"Point\",\"coordinates\":[-110.0,33.1],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}," +
                "{\"type\":\"LineString\",\"coordinates\":[[-110.0,33.1],[-110.0,35.97]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}," +
                "{\"type\":\"MultiPoint\",\"coordinates\":[[11.2,10.2],[11.6,11.9]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}" +
                "]}",
                result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task WriteEntityReferenceLinkAsync(bool writingResponse)
        {
            var entityReferenceLink = new ODataEntityReferenceLink
            {
                Url = new Uri($"{ServiceUri}/Customers(1)")
            };

            var result = await SetupJsonLightOutputContextAndRunTestAsync(
                (jsonLightOutputContext) => jsonLightOutputContext.WriteEntityReferenceLinkAsync(entityReferenceLink),
                writingResponse);

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#$ref\",\"@odata.id\":\"http://tempuri.org/Customers(1)\"}",
                result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task WriteEntityReferenceLinksAsync(bool writingResponse)
        {
            var entityReferenceLinks = new ODataEntityReferenceLinks
            {
                Links = new List<ODataEntityReferenceLink>
                {
                    new ODataEntityReferenceLink { Url = new Uri($"{ServiceUri}/Orders(1)") },
                    new ODataEntityReferenceLink { Url = new Uri($"{ServiceUri}/Orders(2)") }
                }
            };

            var result = await SetupJsonLightOutputContextAndRunTestAsync(
                (jsonLightOutputContext) => jsonLightOutputContext.WriteEntityReferenceLinksAsync(entityReferenceLinks),
                writingResponse);

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Collection($ref)\"," +
                "\"value\":[{\"@odata.id\":\"http://tempuri.org/Orders(1)\"},{\"@odata.id\":\"http://tempuri.org/Orders(2)\"}]}",
                result);
        }

        [Fact]
        public async Task WriteServiceDocumentAsync()
        {
            var serviceDocument = new ODataServiceDocument
            {
                EntitySets = new List<ODataEntitySetInfo>
                {
                    new ODataEntitySetInfo { Name = "Orders", Title = "Orders", Url = new Uri($"{ServiceUri}/Orders") },
                    new ODataEntitySetInfo { Name = "Customers", Title = "Customers", Url = new Uri($"{ServiceUri}/Customers") }
                }
            };

            var result = await SetupJsonLightOutputContextAndRunTestAsync(
                (jsonLightOutputContext) => jsonLightOutputContext.WriteServiceDocumentAsync(serviceDocument));

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata\"," +
                "\"value\":[" +
                "{\"name\":\"Orders\",\"kind\":\"EntitySet\",\"url\":\"http://tempuri.org/Orders\"}," +
                "{\"name\":\"Customers\",\"kind\":\"EntitySet\",\"url\":\"http://tempuri.org/Customers\"}]}",
                result);
        }

        [Fact]
        public async Task WriteErrorAsync()
        {
            var result = await SetupJsonLightOutputContextAndRunTestAsync(
                (jsonLightOutputContext) => jsonLightOutputContext.WriteErrorAsync(this.nullReferenceError, /*includeDebugInformation*/ false));

            Assert.Equal(
                "{\"error\":{\"code\":\"NRE\",\"message\":\"Object reference not set to an instance of an object\",\"@Is.Error\":true}}",
                result);
        }

        [Fact]
        public async Task WriteInStreamErrorWhenWritingResourceSetAsync()
        {
            var result = await SetupJsonLightOutputContextAndRunTestAsync(
                async (jsonLightOutputContext) =>
                {
                    // This should cause in-stream error listener to be initialized with an ODataJsonLightWriter instance
                    await jsonLightOutputContext.CreateODataResourceSetWriterAsync(this.orderEntitySet, this.orderEntityType);

                    await jsonLightOutputContext.WriteInStreamErrorAsync(this.nullReferenceError, /*includeDebugInformation*/ false);
                });

            Assert.Equal(
                "{\"error\":{\"code\":\"NRE\",\"message\":\"Object reference not set to an instance of an object\",\"@Is.Error\":true}}",
                result);
        }

        [Fact]
        public async Task WriteInStreamErrorWhenWritingDeltaAsync()
        {
            var result = await SetupJsonLightOutputContextAndRunTestAsync(
                async (jsonLightOutputContext) =>
                {
                    // This should cause in-stream error listener to be initialized an ODataJsonLightDeltaWriter instance
                    await jsonLightOutputContext.CreateODataDeltaWriterAsync(this.orderEntitySet, this.orderEntityType);

                    await jsonLightOutputContext.WriteInStreamErrorAsync(this.nullReferenceError, /*includeDebugInformation*/ false);
                });

            Assert.Equal(
                "{\"error\":{\"code\":\"NRE\",\"message\":\"Object reference not set to an instance of an object\",\"@Is.Error\":true}}",
                result);
        }

        [Fact]
        public async Task WriteInStreamErrorWhenWritingCollectionAsync()
        {
            var result = await SetupJsonLightOutputContextAndRunTestAsync(
                async (jsonLightOutputContext) =>
                {
                    // This should cause in-stream error listener to be initialized an ODataJsonLightCollectionWriter instance
                    await jsonLightOutputContext.CreateODataCollectionWriterAsync(new EdmEntityTypeReference(this.orderEntityType, true));

                    await jsonLightOutputContext.WriteInStreamErrorAsync(this.nullReferenceError, /*includeDebugInformation*/ false);
                });

            Assert.Equal(
                "{\"error\":{\"code\":\"NRE\",\"message\":\"Object reference not set to an instance of an object\",\"@Is.Error\":true}}",
                result);
        }

        [Fact]
        public async Task WriteInStreamError_ThrowsExceptionIfInvokedWhenWritingParamaterAsync()
        {
            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightOutputContextAndRunTestAsync(
                async (jsonLightOutputContext) =>
                {
                    // This should cause in-stream error listener to be initialized an ODataJsonLightParameterWriter instance
                    await jsonLightOutputContext.CreateODataParameterWriterAsync(/*operation*/ null);

                    await jsonLightOutputContext.WriteInStreamErrorAsync(this.nullReferenceError, /*includeDebugInformation*/ false);
                }));

            Assert.Equal(Strings.ODataParameterWriter_InStreamErrorNotSupported, exception.Message);
        }

        [Fact]
        public async Task WriteInStreamError_ThrowsExceptionIfInvokedWhenWritingBatchAsync()
        {
            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightOutputContextAndRunTestAsync(
                async (jsonLightOutputContext) =>
                {
                    // This should cause in-stream error listener to be initialized with an ODataJsonLightBatchWriter instance
                    await jsonLightOutputContext.CreateODataBatchWriterAsync();

                    await jsonLightOutputContext.WriteInStreamErrorAsync(this.nullReferenceError, /*includeDebugInformation*/ false);
                }));

            Assert.Equal(Strings.ODataBatchWriter_CannotWriteInStreamErrorForBatch, exception.Message);
        }

        #endregion Async Tests

        private static void WriteAndValidate(
            Action<ODataJsonLightOutputContext> test,
            string expectedPayload,
            bool writingResponse = true,
            bool synchronous = true,
            bool use6x = false)
        {
            MemoryStream stream = new MemoryStream();
            var outputContext = CreateJsonLightOutputContext(stream, writingResponse, synchronous, use6x);
            test(outputContext);
            ValidateWrittenPayload(stream, expectedPayload);
        }

        private static void ValidateWrittenPayload(MemoryStream stream, string expectedPayload)
        {
            stream.Seek(0, SeekOrigin.Begin);
            string payload = (new StreamReader(stream)).ReadToEnd();
            Assert.Equal(expectedPayload, payload);
        }

        private static ODataJsonLightOutputContext CreateJsonLightOutputContext(
            MemoryStream stream,
            bool writingResponse = true,
            bool synchronous = true,
            bool use6x = false,
            ODataVersion version = ODataVersion.V4)
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

            var settings = new ODataMessageWriterSettings { Version = version };
            settings.SetServiceDocumentUri(new Uri("http://odata.org/test"));
            settings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");
            
            if (use6x)
            {
                settings.LibraryCompatibility = ODataLibraryCompatibility.Version6;
            }

            return new ODataJsonLightOutputContext(messageInfo, settings);
        }

        private async Task<string> SetupJsonLightOutputContextAndRunTestAsync(
            Func<ODataJsonLightOutputContext, Task> func,
            bool writingResponse = true)
        {
            var messageInfo = CreateMessageInfo(this.model, /*synchronous*/ true, writingResponse);
            var jsonLightOutputContext = new ODataJsonLightOutputContext(messageInfo, this.messageWriterSettings);

            await func(jsonLightOutputContext);

            this.stream.Position = 0;
            return await new StreamReader(this.stream).ReadToEndAsync();
        }

        private ODataMessageInfo CreateMessageInfo(IEdmModel model, bool asynchronous = true, bool writingResponse = true)
        {
            return new ODataMessageInfo
            {
                MessageStream = this.stream,
                MediaType = new ODataMediaType("application", "json",
                new[]
                {
                    new KeyValuePair<string, string>("odata.metadata", "minimal"),
                    new KeyValuePair<string, string>("odata.streaming", "true"),
                    new KeyValuePair<string, string>("IEEE754Compatible", "false"),
                    new KeyValuePair<string, string>("charset", "utf-8")
                }),
                Encoding = Encoding.UTF8,
                IsResponse = writingResponse,
                IsAsync = asynchronous,
                Model = model
            };
        }

        private void InitializeEdmModel()
        {
            this.model = new EdmModel();

            this.orderEntityType = new EdmEntityType("NS", "Order", /*baseType*/ null, /*isAbstract*/ false, /*isOpen*/ true);
            this.customerEntityType = new EdmEntityType("NS", "Customer");

            var orderIdProperty = this.orderEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            this.orderEntityType.AddKeys(orderIdProperty);
            this.orderEntityType.AddStructuralProperty("Amount", EdmPrimitiveTypeKind.Decimal);
            var customerNavProperty = this.orderEntityType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo
                {
                    Name = "Customer",
                    Target = this.customerEntityType,
                    TargetMultiplicity = EdmMultiplicity.ZeroOrOne
                });
            this.model.AddElement(this.orderEntityType);

            var customerIdProperty = this.customerEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            this.customerEntityType.AddKeys(customerIdProperty);
            this.customerEntityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            var ordersNavProperty = this.customerEntityType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo
                {
                    Name = "Orders",
                    Target = this.orderEntityType,
                    TargetMultiplicity = EdmMultiplicity.Many
                });
            this.model.AddElement(this.customerEntityType);

            var entityContainer = new EdmEntityContainer("NS", "Container");
            this.model.AddElement(entityContainer);

            this.orderEntitySet = entityContainer.AddEntitySet("Orders", this.orderEntityType);
            this.customerEntitySet = entityContainer.AddEntitySet("Customers", this.customerEntityType);

            this.orderEntitySet.AddNavigationTarget(customerNavProperty, this.customerEntitySet);
            this.customerEntitySet.AddNavigationTarget(ordersNavProperty, this.orderEntitySet);
        }

        #region Helper Methods

        private static ODataResourceSet CreateOrderResourceSet()
        {
            return new ODataResourceSet
            {
                TypeName = "Collection(NS.Order)",
                SerializationInfo = new ODataResourceSerializationInfo
                {
                    NavigationSourceName = "Orders",
                    ExpectedTypeName = "NS.Order",
                    NavigationSourceEntityTypeName = "NS.Order",
                    NavigationSourceKind = EdmNavigationSourceKind.EntitySet
                }
            };
        }

        private static ODataDeltaResourceSet CreateOrderDeltaResourceSet()
        {
            return new ODataDeltaResourceSet
            {
                TypeName = "Collection(NS.Order)",
                DeltaLink = new Uri($"{ServiceUri}/Orders/deltaLink"),
                SerializationInfo = new ODataResourceSerializationInfo
                {
                    NavigationSourceName = "Orders",
                    ExpectedTypeName = "NS.Order",
                    NavigationSourceEntityTypeName = "NS.Order",
                    NavigationSourceKind = EdmNavigationSourceKind.EntitySet
                }
            };
        }

        private static ODataResource CreateOrderResource()
        {
            return new ODataResource
            {
                TypeName = "NS.Order",
                Properties = new List<ODataProperty>
                {
                    new ODataProperty
                    {
                        Name = "Id",
                        Value = 1,
                        SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Key }
                    },
                    new ODataProperty { Name = "Amount", Value = 13M }
                },
                SerializationInfo = new ODataResourceSerializationInfo
                {
                    NavigationSourceName = "Orders",
                    ExpectedTypeName = "NS.Order",
                    NavigationSourceEntityTypeName = "NS.Order",
                    NavigationSourceKind = EdmNavigationSourceKind.EntitySet
                }
            };
        }

        #endregion Helper Methods
    }
}

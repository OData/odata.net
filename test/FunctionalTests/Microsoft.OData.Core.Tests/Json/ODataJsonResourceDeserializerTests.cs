//---------------------------------------------------------------------
// <copyright file="ODataJsonResourceDeserializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.Evaluation;
using Microsoft.OData.Json;
using Microsoft.Spatial;
using Xunit;
using ErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.Json
{
    public class ODataJsonResourceDeserializerTests
    {
        private EdmModel model;
        private EdmEntityType customerEntityType;
        private EdmEntityType categoryEntityType;
        private EdmEntityType productEntityType;
        private EdmEntitySet customersEntitySet;
        private EdmEntitySet categoriesEntitySet;
        private EdmEntitySet productsEntitySet;
        private ODataMessageReaderSettings messageReaderSettings;

        public ODataJsonResourceDeserializerTests()
        {
            this.InitializeModel();
            this.messageReaderSettings = new ODataMessageReaderSettings();
        }

        [Fact]
        public async Task ReadResourceSetContentAsync()
        {
            using (var jsonInputContext = CreateJsonInputContext("{\"value\":[]}", model))
            {
                var jsonResourceDeserializer = new ODataJsonResourceDeserializer(jsonInputContext);
                var jsonReader = jsonResourceDeserializer.JsonReader;

                await AdvanceReaderToFirstPropertyAsync(jsonReader);
                await jsonReader.ReadAsync();

                await jsonResourceDeserializer.ReadResourceSetContentStartAsync();
                Assert.Equal(JsonNodeType.EndArray, jsonReader.NodeType);
                await jsonResourceDeserializer.ReadResourceSetContentEndAsync();
                Assert.Equal(JsonNodeType.EndObject, jsonReader.NodeType);
            }
        }

        [Theory]
        [InlineData("@odata.type")]
        [InlineData("@type")]
        public async Task ReadResourceTypeNameAsync(string odataTypeAnnotationName)
        {
            this.messageReaderSettings.Version = ODataVersion.V401;
            var payload = $"{{\"{odataTypeAnnotationName}\":\"NS.Customer\",\"Id\":1,\"Name\":\"Sue\"}}";

            using (var jsonInputContext = CreateJsonInputContext(payload, model))
            {
                var jsonResourceDeserializer = new ODataJsonResourceDeserializer(jsonInputContext);

                await AdvanceReaderToFirstPropertyAsync(jsonResourceDeserializer.JsonReader);

                var resourceState = new TestJsonReaderResourceState(this.customersEntitySet, this.customerEntityType);

                await jsonResourceDeserializer.ReadResourceTypeNameAsync(resourceState);

                Assert.Equal("NS.Customer", resourceState.Resource.TypeName);
            }
        }

        [Theory]
        [InlineData("{\"@odata.removed\":{\"reason\":\"deleted\"},\"@odata.id\":\"http://tempuri.org/Customers(1)\"}", DeltaDeletedEntryReason.Deleted, ODataVersion.V4)]
        [InlineData("{\"@odata.removed\":{\"reason\":\"changed\"},\"@odata.id\":\"http://tempuri.org/Customers(1)\"}", DeltaDeletedEntryReason.Changed, ODataVersion.V4)]
        [InlineData("{\"@removed\":{\"reason\":\"deleted\"},\"@id\":\"http://tempuri.org/Customers(1)\"}", DeltaDeletedEntryReason.Deleted, ODataVersion.V401)]
        [InlineData("{\"@removed\":{\"reason\":\"changed\"},\"@id\":\"http://tempuri.org/Customers(1)\"}", DeltaDeletedEntryReason.Changed, ODataVersion.V401)]
        public async Task ReadDeletedResourceAsync(string payload, DeltaDeletedEntryReason deletedEntryReason, ODataVersion odataVersion)
        {
            this.messageReaderSettings.Version = odataVersion;

            using (var jsonInputContext = CreateJsonInputContext(payload, model))
            {
                var jsonResourceDeserializer = new ODataJsonResourceDeserializer(jsonInputContext);

                await AdvanceReaderToFirstPropertyAsync(jsonResourceDeserializer.JsonReader);

                var deletedResource = await jsonResourceDeserializer.ReadDeletedResourceAsync();

                Assert.Equal(deletedEntryReason, deletedResource.Reason);
                Assert.Equal("http://tempuri.org/Customers(1)", deletedResource.Id.AbsoluteUri);
            }
        }

        [Theory]
        [InlineData("{\"id\":\"http://tempuri.org/Customers(1)\",\"reason\":\"deleted\"}", DeltaDeletedEntryReason.Deleted)]
        [InlineData("{\"reason\":\"deleted\",\"id\":\"http://tempuri.org/Customers(1)\"}", DeltaDeletedEntryReason.Deleted)]
        [InlineData("{\"id\":\"http://tempuri.org/Customers(1)\",\"reason\":\"changed\"}", DeltaDeletedEntryReason.Changed)]
        [InlineData("{\"reason\":\"changed\",\"id\":\"http://tempuri.org/Customers(1)\"}", DeltaDeletedEntryReason.Changed)]
        public async Task ReadDeletedEntryAsync(string payload, DeltaDeletedEntryReason deletedEntryReason)
        {
            using (var jsonInputContext = CreateJsonInputContext(payload, model))
            {
                var jsonResourceDeserializer = new ODataJsonResourceDeserializer(jsonInputContext);

                await AdvanceReaderToFirstPropertyAsync(jsonResourceDeserializer.JsonReader);

                var deletedResource = await jsonResourceDeserializer.ReadDeletedEntryAsync();

                Assert.Equal(deletedEntryReason, deletedResource.Reason);
                Assert.Equal("http://tempuri.org/Customers(1)", deletedResource.Id.AbsoluteUri);
            }
        }

        [Theory]
        [InlineData("{\"id\":\"http://tempuri.org/Customers(1)\",\"reason\":\"deleted\",\"unexpected\":true}")]
        [InlineData("{\"id\":\"http://tempuri.org/Customers(1)\",\"unexpected\":true,\"reason\":\"deleted\"}")]
        [InlineData("{\"unexpected\":true,\"id\":\"http://tempuri.org/Customers(1)\",\"reason\":\"deleted\"}")]
        public async Task ReadDeletedEntryAsync_IgnoresUnexpectedPrimitiveProperties(string payload)
        {
            using (var jsonInputContext = CreateJsonInputContext(payload, model))
            {
                var jsonResourceDeserializer = new ODataJsonResourceDeserializer(jsonInputContext);

                await AdvanceReaderToFirstPropertyAsync(jsonResourceDeserializer.JsonReader);

                var deletedResource = await jsonResourceDeserializer.ReadDeletedEntryAsync();

                Assert.Equal(DeltaDeletedEntryReason.Deleted, deletedResource.Reason);
                Assert.Equal("http://tempuri.org/Customers(1)", deletedResource.Id.AbsoluteUri);
            }
        }

        [Theory]
        [InlineData("{\"id\":\"http://tempuri.org/Customers(1)\",\"reason\":\"deleted\"}", DeltaDeletedEntryReason.Deleted)]
        [InlineData("{\"reason\":\"deleted\",\"id\":\"http://tempuri.org/Customers(1)\"}", DeltaDeletedEntryReason.Deleted)]
        [InlineData("{\"id\":\"http://tempuri.org/Customers(1)\",\"reason\":\"changed\"}", DeltaDeletedEntryReason.Changed)]
        [InlineData("{\"reason\":\"changed\",\"id\":\"http://tempuri.org/Customers(1)\"}", DeltaDeletedEntryReason.Changed)]
        public void ReadDeletedEntry(string payload, DeltaDeletedEntryReason deletedEntryReason)
        {
            using (var jsonInputContext = CreateJsonInputContext(payload: payload, model: model, isAsync: false))
            {
                var jsonResourceDeserializer = new ODataJsonResourceDeserializer(jsonInputContext);

                AdvanceReaderToFirstProperty(jsonResourceDeserializer.JsonReader);

                var deletedResource = jsonResourceDeserializer.ReadDeletedEntry();

                Assert.Equal(deletedEntryReason, deletedResource.Reason);
                Assert.Equal("http://tempuri.org/Customers(1)", deletedResource.Id.AbsoluteUri);
            }
        }

        [Theory]
        [InlineData("{\"id\":\"http://tempuri.org/Customers(1)\",\"reason\":\"deleted\",\"unexpected\":true}")]
        [InlineData("{\"id\":\"http://tempuri.org/Customers(1)\",\"unexpected\":true,\"reason\":\"deleted\"}")]
        [InlineData("{\"unexpected\":true,\"id\":\"http://tempuri.org/Customers(1)\",\"reason\":\"deleted\"}")]
        public void ReadDeletedEntry_IgnoresUnexpectedPrimitiveProperties(string payload)
        {
            using (var jsonInputContext = CreateJsonInputContext(payload: payload, model: model, isAsync: false))
            {
                var jsonResourceDeserializer = new ODataJsonResourceDeserializer(jsonInputContext);

                AdvanceReaderToFirstProperty(jsonResourceDeserializer.JsonReader);

                var deletedResource = jsonResourceDeserializer.ReadDeletedEntry();

                Assert.Equal(DeltaDeletedEntryReason.Deleted, deletedResource.Reason);
                Assert.Equal("http://tempuri.org/Customers(1)", deletedResource.Id.AbsoluteUri);
            }
        }

        [Fact]
        public async Task ReadDeltaLinkAsync()
        {
            var payload = "{\"source\":\"http://tempuri.org/Categories(1)\",\"relationship\":\"BestSeller\",\"target\":\"http://tempuri.org/Products(1)\"}";

            using (var jsonInputContext = CreateJsonInputContext(payload, model))
            {
                var jsonResourceDeserializer = new ODataJsonResourceDeserializer(jsonInputContext);

                await AdvanceReaderToFirstPropertyAsync(jsonResourceDeserializer.JsonReader);

                var deltaLink = new ODataDeltaLink(null, null, null);

                await jsonResourceDeserializer.ReadDeltaLinkSourceAsync(deltaLink);
                await jsonResourceDeserializer.ReadDeltaLinkRelationshipAsync(deltaLink);
                await jsonResourceDeserializer.ReadDeltaLinkTargetAsync(deltaLink);

                Assert.Equal("http://tempuri.org/Categories(1)", deltaLink.Source.AbsoluteUri);
                Assert.Equal("BestSeller", deltaLink.Relationship);
                Assert.Equal("http://tempuri.org/Products(1)", deltaLink.Target.AbsoluteUri);
            }
        }

        [Fact]
        public async Task ReadResourceContentAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Food\"}";

            await SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                payload,
                this.categoriesEntitySet,
                this.categoryEntityType,
                commonVerifyAction);
        }

        [Fact]
        public async Task ReadResourceContentWithUndeclaredPrimitivePropertyAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Food\"," +
                "\"YTDTotal@odata.type\":\"#Decimal\",\"YTDTotal\":7130}";

            await SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                payload,
                this.categoriesEntitySet,
                this.categoryEntityType,
                (resourceState) =>
                {
                    var resource = resourceState.Resource;
                    Assert.NotNull(resource);
                    Assert.Equal(3, resource.Properties.Count());
                    var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                    var idProperty = properties[0];
                    var nameProperty = properties[1];
                    var ytdTotalProperty = properties[2];
                    Assert.Equal("Id", idProperty.Name);
                    Assert.Equal(1, idProperty.Value);
                    Assert.Equal("Name", nameProperty.Name);
                    Assert.Equal("Food", nameProperty.Value);
                    Assert.Equal("YTDTotal", ytdTotalProperty.Name);
                    Assert.Equal(7130m, ytdTotalProperty.Value);

                    var odataPropertyAnnotations = resourceState.PropertyAndAnnotationCollector.GetODataPropertyAnnotations("YTDTotal");
                    var odataType = Assert.Contains("odata.type", odataPropertyAnnotations);
                    Assert.Equal("Edm.Decimal", odataType);
                });
        }

        [Fact]
        public async Task ReadResourceContentWithODataAnnotationsAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"," +
                "\"@odata.id\":\"http://tempuri.org/Categories(1)\"," +
                "\"@odata.editLink\":\"http://tempuri.org/Categories(1)\"," +
                "\"@odata.readLink\":\"http://tempuri.org/Categories(1)\"," +
                "\"@odata.etag\":\"etag\"," +
                "\"@odata.mediaEditLink\":\"http://tempuri.org/Categories(1)/Img\"," +
                "\"@odata.mediaReadLink\":\"http://tempuri.org/Categories(1)/Img\"," +
                "\"@odata.mediaContentType\":\"image/png\"," +
                "\"@odata.mediaEtag\":\"media-etag\"," +
                "\"Id\":1," +
                "\"Name\":\"Food\"}";

            await SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                payload,
                this.categoriesEntitySet,
                this.categoryEntityType,
                (resourceState) =>
                {
                    commonVerifyAction(resourceState);

                    var odataScopeAnnotations = resourceState.PropertyAndAnnotationCollector.GetODataScopeAnnotation();

                    var odataId = (Uri)Assert.Contains("odata.id", odataScopeAnnotations);
                    var odataEditLink = (Uri)Assert.Contains("odata.editLink", odataScopeAnnotations);
                    var odataReadLink = (Uri)Assert.Contains("odata.readLink", odataScopeAnnotations);
                    var odataEtag = Assert.Contains("odata.etag", odataScopeAnnotations);
                    var odataMediaEditLink = (Uri)Assert.Contains("odata.mediaEditLink", odataScopeAnnotations);
                    var odataMediaReadLink = (Uri)Assert.Contains("odata.mediaReadLink", odataScopeAnnotations);
                    var odataMediaContentType = Assert.Contains("odata.mediaContentType", odataScopeAnnotations);
                    var odataMediaEtag = Assert.Contains("odata.mediaEtag", odataScopeAnnotations);
                    Assert.Equal("http://tempuri.org/Categories(1)", odataId.AbsoluteUri);
                    Assert.Equal("http://tempuri.org/Categories(1)", odataEditLink.AbsoluteUri);
                    Assert.Equal("http://tempuri.org/Categories(1)", odataReadLink.AbsoluteUri);
                    Assert.Equal("etag", odataEtag);
                    Assert.Equal("http://tempuri.org/Categories(1)/Img", odataMediaEditLink.AbsoluteUri);
                    Assert.Equal("http://tempuri.org/Categories(1)/Img", odataMediaReadLink.AbsoluteUri);
                    Assert.Equal("image/png", odataMediaContentType);
                    Assert.Equal("media-etag", odataMediaEtag);
                });
        }

        [Fact]
        public async Task ReadResourceContentWithODataPropertyAnnotationsAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"," +
                "\"Id\":1," +
                "\"Name@odata.mediaEditLink\":\"http://tempuri.org/Categories(1)/Name\"," +
                "\"Name@odata.mediaReadLink\":\"http://tempuri.org/Categories(1)/Name\"," +
                "\"Name@odata.mediaContentType\":\"text/plain\"," +
                "\"Name@odata.mediaEtag\":\"media-etag\"," +
                "\"Name\":\"Food\"}";

            await SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                payload,
                this.categoriesEntitySet,
                this.categoryEntityType,
                (resourceState) =>
                {
                    commonVerifyAction(resourceState);

                    var odataPropertyAnnotations = resourceState.PropertyAndAnnotationCollector.GetODataPropertyAnnotations("Name");

                    var odataMediaEditLink = (Uri)Assert.Contains("odata.mediaEditLink", odataPropertyAnnotations);
                    var odataMediaReadLink = (Uri)Assert.Contains("odata.mediaReadLink", odataPropertyAnnotations);
                    var odataMediaContentType = Assert.Contains("odata.mediaContentType", odataPropertyAnnotations);
                    var odataMediaEtag = Assert.Contains("odata.mediaEtag", odataPropertyAnnotations);

                    Assert.Equal("http://tempuri.org/Categories(1)/Name", odataMediaEditLink.AbsoluteUri);
                    Assert.Equal("http://tempuri.org/Categories(1)/Name", odataMediaReadLink.AbsoluteUri);
                    Assert.Equal("text/plain", odataMediaContentType);
                    Assert.Equal("media-etag", odataMediaEtag);
                });
        }

        [Fact]
        public async Task ReadResourceContentWithCustomAnnotationsAsync()
        {
            this.messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("attr.remark");

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"," +
                "\"@attr.remark\":\"Perishable\"," +
                "\"Id\":1," +
                "\"Name\":\"Food\"}";

            await SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                payload,
                this.categoriesEntitySet,
                this.categoryEntityType,
                (resourceState) =>
                {
                    commonVerifyAction(resourceState);

                    var customScopeAnnotations = resourceState.PropertyAndAnnotationCollector.GetCustomScopeAnnotation();
                    Assert.Contains(new KeyValuePair<string, object>("attr.remark", "Perishable"), customScopeAnnotations);
                });
        }

        [Fact]
        public async Task ReadResourceContentWithPropertyWithoutValueButWithCustomAnnotationsAsync()
        {
            this.messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("custom.instance");

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"," +
                "\"Id\":41," +
                "\"Name@custom.instance\":\"Food\"}";

            await SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                payload,
                this.categoriesEntitySet,
                this.categoryEntityType,
                (resourceState) =>
                {
                    var resource = resourceState.Resource;
                    Assert.NotNull(resource);
                    Assert.Equal(2, resource.Properties.Count());
                    var idProperty = Assert.IsType<ODataProperty>(resource.Properties.ElementAt(0));
                    var nameProperty = Assert.IsType<ODataPropertyInfo>(resource.Properties.ElementAt(1));
                    Assert.Equal("Id", idProperty.Name);
                    Assert.Equal(41, idProperty.Value);
                    Assert.Equal("Name", nameProperty.Name);

                    var customAnnotations = resourceState.PropertyAndAnnotationCollector.GetCustomPropertyAnnotations("Name");
                    Assert.Contains(new KeyValuePair<string, object>("custom.instance", "Food"), customAnnotations);
                });
        }

        [Fact]
        public async Task ReadResourceContentWithExpandedSingletonNavigationPropertyInResponsePayloadAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Food\"," +
                "\"BestSeller@odata.type\":\"#NS.Product\"," +
                "\"BestSeller@odata.associationLink\":\"http://tempuri.org/Categories(1)/BestSeller\"," +
                "\"BestSeller@odata.navigationLink\":\"http://tempuri.org/Products(1)\"," +
                "\"BestSeller\":{\"Id\":1,\"Name\":\"Sugar\"}}";

            await SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                payload,
                this.categoriesEntitySet,
                this.categoryEntityType,
                (resourceState) =>
                {
                    commonVerifyAction(resourceState);

                    var odataPropertyAnnotations = resourceState.PropertyAndAnnotationCollector.GetODataPropertyAnnotations("BestSeller");

                    var odataType = Assert.Contains("odata.type", odataPropertyAnnotations);
                    var odataAssociationLink = (Uri)Assert.Contains("odata.associationLink", odataPropertyAnnotations);
                    var odataNavigationLink = (Uri)Assert.Contains("odata.navigationLink", odataPropertyAnnotations);
                    Assert.Equal("NS.Product", odataType);
                    Assert.Equal("http://tempuri.org/Categories(1)/BestSeller", odataAssociationLink.AbsoluteUri);
                    Assert.Equal("http://tempuri.org/Products(1)", odataNavigationLink.AbsoluteUri);
                });
        }

        [Fact]
        public async Task ReadResourceContentWithExpandedCollectionNavigationPropertyInResponsePayloadAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Food\"," +
                "\"Products@odata.type\":\"#Collection(NS.Product)\"," +
                "\"Products@odata.associationLink\":\"http://tempuri.org/Categories(1)/Products\"," +
                "\"Products@odata.navigationLink\":\"http://tempuri.org/Categories(1)/Products\"," +
                "\"Products@odata.nextLink\":\"http://tempuri.org/Categories(1)/Products/nextLink\"," +
                "\"Products@odata.count\":3," +
                "\"Products\":[{\"Id\":1,\"Name\":\"Sugar\"},{\"Id\":2,\"Name\":\"Coffee\"}]}";

            await SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                payload,
                this.categoriesEntitySet,
                this.categoryEntityType,
                (resourceState) =>
                {
                    commonVerifyAction(resourceState);

                    var odataPropertyAnnotations = resourceState.PropertyAndAnnotationCollector.GetODataPropertyAnnotations("Products");

                    var odataType = Assert.Contains("odata.type", odataPropertyAnnotations);
                    var odataAssociationLink = (Uri)Assert.Contains("odata.associationLink", odataPropertyAnnotations);
                    var odataNavigationLink = (Uri)Assert.Contains("odata.navigationLink", odataPropertyAnnotations);
                    var odataNextLink = (Uri)Assert.Contains("odata.nextLink", odataPropertyAnnotations);
                    var odataCount = (long)Assert.Contains("odata.count", odataPropertyAnnotations);
                    Assert.Equal("Collection(NS.Product)", odataType);
                    Assert.Equal("http://tempuri.org/Categories(1)/Products", odataAssociationLink.AbsoluteUri);
                    Assert.Equal("http://tempuri.org/Categories(1)/Products", odataNavigationLink.AbsoluteUri);
                    Assert.Equal("http://tempuri.org/Categories(1)/Products/nextLink", odataNextLink.AbsoluteUri);
                    Assert.Equal(3, odataCount);
                });
        }

        [Fact]
        public async Task ReadResourceContentWithNonExpandedSingletonNavigationPropertyInResponsePayloadAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Food\"," +
                "\"BestSeller@odata.type\":\"#NS.Product\"," +
                "\"BestSeller@odata.associationLink\":\"http://tempuri.org/Categories(1)/BestSeller\"," +
                "\"BestSeller@odata.navigationLink\":\"http://tempuri.org/Products(1)\"}";

            await SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                payload,
                this.categoriesEntitySet,
                this.categoryEntityType,
                (resourceState) =>
                {
                    commonVerifyAction(resourceState);

                    var odataPropertyAnnotations = resourceState.PropertyAndAnnotationCollector.GetODataPropertyAnnotations("BestSeller");

                    var odataType = Assert.Contains("odata.type", odataPropertyAnnotations);
                    var odataAssociationLink = (Uri)Assert.Contains("odata.associationLink", odataPropertyAnnotations);
                    var odataNavigationLink = (Uri)Assert.Contains("odata.navigationLink", odataPropertyAnnotations);
                    Assert.Equal("NS.Product", odataType);
                    Assert.Equal("http://tempuri.org/Categories(1)/BestSeller", odataAssociationLink.AbsoluteUri);
                    Assert.Equal("http://tempuri.org/Products(1)", odataNavigationLink.AbsoluteUri);
                });
        }

        [Fact]
        public async Task ReadResourceContentWithNonExpandedSingletonNavigationPropertyInRequestPayloadAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Food\"," +
                "\"BestSeller@odata.bind\":\"http://tempuri.org/Products(1)\"}";

            await SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                payload,
                this.categoriesEntitySet,
                this.categoryEntityType,
                (resourceState) =>
                {
                    commonVerifyAction(resourceState);

                    var odataPropertyAnnotations = resourceState.PropertyAndAnnotationCollector.GetODataPropertyAnnotations("BestSeller");

                    var entityReferenceLink = Assert.Contains("odata.bind", odataPropertyAnnotations) as ODataEntityReferenceLink;
                    Assert.NotNull(entityReferenceLink);
                    Assert.Equal("http://tempuri.org/Products(1)", entityReferenceLink.Url.AbsoluteUri);
                },
                isResponse: false);
        }

        [Fact]
        public async Task ReadResourceContentWithNonExpandedCollectionNavigationPropertyInRequestPayloadAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Food\"," +
                "\"Products@odata.bind\":[\"http://tempuri.org/Products(1)\"]}";

            await SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                payload,
                this.categoriesEntitySet,
                this.categoryEntityType,
                (resourceState) =>
                {
                    commonVerifyAction(resourceState);

                    var odataPropertyAnnotations = resourceState.PropertyAndAnnotationCollector.GetODataPropertyAnnotations("Products");

                    var entityReferenceLinks = Assert.Contains("odata.bind", odataPropertyAnnotations) as LinkedList<ODataEntityReferenceLink>;
                    Assert.NotNull(entityReferenceLinks);
                    var entityReferenceLink = Assert.Single(entityReferenceLinks);
                    Assert.Equal("http://tempuri.org/Products(1)", entityReferenceLink.Url.AbsoluteUri);
                },
                isResponse: false);
        }

        [Fact]
        public async Task ReadResourceContentWithODataActionAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Food\"," +
                "\"#NS.RateProducts\":{\"title\":\"RateProducts\",\"target\":\"http://tempuri.org/Categories(1)/RateProducts\",\"IgnoreProp\":true}}";

            await SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                payload,
                this.categoriesEntitySet,
                this.categoryEntityType,
                (resourceState) =>
                {
                    commonVerifyAction(resourceState);

                    var rateProductsAction = Assert.Single(resourceState.Resource.Actions);
                    Assert.Equal("RateProducts", rateProductsAction.Title);
                    Assert.Equal("http://tempuri.org/Categories(1)/RateProducts", rateProductsAction.Target.AbsoluteUri);
                    Assert.Equal("http://tempuri.org/$metadata#NS.RateProducts", rateProductsAction.Metadata.AbsoluteUri);
                });
        }

        [Fact]
        public async Task ReadResourceContentWithODataFunctionAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Food\"," +
                "\"#NS.Top5Products\":[" +
                "{\"title\":\"Top5Products\",\"target\":\"http://tempuri.org/Categories(1)/Top5Products\",\"IgnoreProp\":true}]}";

            await SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                payload,
                this.categoriesEntitySet,
                this.categoryEntityType,
                (resourceState) =>
                {
                    commonVerifyAction(resourceState);

                    var top5ProductsAction = Assert.Single(resourceState.Resource.Functions);
                    Assert.Equal("Top5Products", top5ProductsAction.Title);
                    Assert.Equal("http://tempuri.org/Categories(1)/Top5Products", top5ProductsAction.Target.AbsoluteUri);
                    Assert.Equal("http://tempuri.org/$metadata#NS.Top5Products", top5ProductsAction.Metadata.AbsoluteUri);
                });
        }

        [Fact]
        public async Task ReadResourceContentAsync_IgnoresUnrecognizedOperation()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Food\"," +
                "\"#NS.UnrecognizedOperation\":{\"title\":\"UnrecognizedOperation\",\"target\":\"http://tempuri.org/Categories(1)/UnrecognizedOperation\"}}";

            await SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                payload,
                this.categoriesEntitySet,
                this.categoryEntityType,
                (resourceState) =>
                {
                    commonVerifyAction(resourceState);

                    Assert.Empty(resourceState.Resource.Actions);
                });
        }

        [Fact]
        public async Task ReadResourceContentWithNestedDeltaResourceSetAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Food\"," +
                "\"Products@delta\":[{\"@odata.id\":\"http://tempuri.org/Products(1)\",\"Id\":1,\"Name\":\"Tea\"}]}";

            await SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                payload,
                this.categoriesEntitySet,
                this.categoryEntityType,
                commonVerifyAction);
        }

        [Fact]
        public async Task ReadV401ResourceContentWithNestedDeltaResourceSetAsync()
        {
            this.messageReaderSettings.Version = ODataVersion.V401;

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Food\"," +
                "\"Products@odata.type\":\"#Collection(NS.Product)\"," +
                "\"Products@odata.associationLink\":\"http://tempuri.org/Categories(1)/Products\"," +
                "\"Products@odata.navigationLink\":\"http://tempuri.org/Categories(1)/Products\"," +
                "\"Products@odata.nextLink\":\"http://tempuri.org/Categories(1)/Products/nextLink\"," +
                "\"Products@odata.count\":3," +
                "\"Products@delta\":[{\"Id\":1,\"Name\":\"Sugar\"},{\"Id\":2,\"Name\":\"Coffee\"}]}";

            await SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                payload,
                this.categoriesEntitySet,
                this.categoryEntityType,
                (resourceState) =>
                {
                    commonVerifyAction(resourceState);

                    var odataPropertyAnnotations = resourceState.PropertyAndAnnotationCollector.GetODataPropertyAnnotations("Products");

                    var odataType = Assert.Contains("odata.type", odataPropertyAnnotations);
                    var odataAssociationLink = (Uri)Assert.Contains("odata.associationLink", odataPropertyAnnotations);
                    var odataNavigationLink = (Uri)Assert.Contains("odata.navigationLink", odataPropertyAnnotations);
                    var odataNextLink = (Uri)Assert.Contains("odata.nextLink", odataPropertyAnnotations);
                    var odataCount = (long)Assert.Contains("odata.count", odataPropertyAnnotations);
                    Assert.Equal("Collection(NS.Product)", odataType);
                    Assert.Equal("http://tempuri.org/Categories(1)/Products", odataAssociationLink.AbsoluteUri);
                    Assert.Equal("http://tempuri.org/Categories(1)/Products", odataNavigationLink.AbsoluteUri);
                    Assert.Equal("http://tempuri.org/Categories(1)/Products/nextLink", odataNextLink.AbsoluteUri);
                    Assert.Equal(3, odataCount);
                });
        }

        [Fact]
        public async Task ReadResourceContentWithPrimitivePropertyReadAsStreamAsync()
        {
            this.messageReaderSettings.ReadAsStreamFunc = (primitiveType, isCollection, propertyName, edmProperty) => propertyName.Equals("Name");

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Food\"}";

            await SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                payload,
                this.categoriesEntitySet,
                this.categoryEntityType,
                (resourceState) =>
                {
                    var idProperty = Assert.IsType<ODataProperty>(Assert.Single(resourceState.Resource.Properties));
                    Assert.Equal("Id", idProperty.Name);
                    Assert.Equal(1, idProperty.Value);
                    // Name property treated as a nested property
                });
        }

        [Fact]
        public async Task ReadResourceContentWithUndeclaredStreamPropertyAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Food\"," +
                "\"Media@odata.type\":\"#Edm.Stream\"," +
                "\"Media@odata.mediaEditLink\":\"http://tempuri.org/Categories(1)/Media\"," +
                "\"Media@odata.mediaReadLink\":\"http://tempuri.org/Categories(1)/Media\"," +
                "\"Media@odata.mediaContentType\":\"text/plain\"," +
                "\"Media@odata.mediaEtag\":\"media-etag\"," +
                "\"Media\":\"AQIDBAUGBwgJAA==\"}";

            await SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                payload,
                this.categoriesEntitySet,
                this.categoryEntityType,
                (resourceState) =>
                {
                    var mediaProperty = Assert.IsType<ODataProperty>(Assert.Single(resourceState.Resource.Properties.Where(d => d.Name.Equals("Media"))));
                    var streamReferenceValue = Assert.IsType<ODataStreamReferenceValue>(mediaProperty.Value);

                    Assert.Equal("http://tempuri.org/Categories(1)/Media", streamReferenceValue.EditLink.AbsoluteUri);
                    Assert.Equal("http://tempuri.org/Categories(1)/Media", streamReferenceValue.ReadLink.AbsoluteUri);
                    Assert.Equal("text/plain", streamReferenceValue.ContentType);
                    Assert.Equal("media-etag", streamReferenceValue.ETag);

                    var odataPropertyAnnotations = resourceState.PropertyAndAnnotationCollector.GetODataPropertyAnnotations("Media");

                    var odataMediaEditLink = (Uri)Assert.Contains("odata.mediaEditLink", odataPropertyAnnotations);
                    var odataMediaReadLink = (Uri)Assert.Contains("odata.mediaReadLink", odataPropertyAnnotations);
                    var odataMediaContentType = Assert.Contains("odata.mediaContentType", odataPropertyAnnotations);
                    var odataMediaEtag = Assert.Contains("odata.mediaEtag", odataPropertyAnnotations);

                    Assert.Equal("http://tempuri.org/Categories(1)/Media", odataMediaEditLink.AbsoluteUri);
                    Assert.Equal("http://tempuri.org/Categories(1)/Media", odataMediaReadLink.AbsoluteUri);
                    Assert.Equal("text/plain", odataMediaContentType);
                    Assert.Equal("media-etag", odataMediaEtag);
                });
        }

        [Fact]
        public async Task ReadResourceContentWithUndeclaredStreamCollectionPropertyAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Food\"," +
                "\"Images@odata.type\":\"#Collection(Edm.Stream)\"," +
                "\"Images\":[\"AQIDBAUGBwgJAA==\"]}";

            await SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                payload,
                this.categoriesEntitySet,
                this.categoryEntityType,
                (resourceState) =>
                {
                    commonVerifyAction(resourceState);
                    // Images property treated as a nested property
                });
        }

        [Fact]
        public async Task ReadResourceContentWithUndeclaredSpatialPropertyAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Food\"," +
                "\"WarehousePin@odata.type\":\"#Edm.GeographyPoint\"," +
                "\"WarehousePin\":{\"type\":\"Point\",\"coordinates\":[22.2,22.2],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            await SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                payload,
                this.categoriesEntitySet,
                this.categoryEntityType,
                (resourceState) =>
                {
                    Assert.Equal(3, resourceState.Resource.Properties.Count());

                    var warehousePinProperty = Assert.IsType<ODataProperty>(Assert.Single(resourceState.Resource.Properties.Where(d => d.Name.Equals("WarehousePin"))));
                    var geographyPoint = Assert.IsAssignableFrom<GeographyPoint>(warehousePinProperty.Value);
                    Assert.Equal(22.2, geographyPoint.Latitude);
                    Assert.Equal(22.2, geographyPoint.Longitude);
                    Assert.Equal(4326, geographyPoint.CoordinateSystem.EpsgId);
                });
        }

        [Fact]
        public async Task ReadResourceContentWithUndeclaredPropertyAsUntypedAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Food\"," +
                "\"Pi\":3.1428571429}";

            await SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                payload,
                this.categoriesEntitySet,
                this.categoryEntityType,
                (resourceState) =>
                {
                    Assert.Equal(3, resourceState.Resource.Properties.Count());

                    var piProperty = Assert.IsType<ODataProperty>(Assert.Single(resourceState.Resource.Properties.Where(d => d.Name.Equals("Pi"))));
                    var untypedValue = Assert.IsType<ODataUntypedValue>(piProperty.Value);
                    Assert.Equal("3.1428571429", untypedValue.RawValue);
                });
        }

        [Fact]
        public async Task ReadResourceContentWithUndeclaredPrimitivePropertyForNonOpenTypeAsync()
        {
            this.messageReaderSettings.Validations &= ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType;

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Sue\"," +
                "\"CreditLimit\":1730}";

            await SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                payload,
                this.customersEntitySet,
                this.customerEntityType,
                (resourceState) =>
                {
                    Assert.Equal(3, resourceState.Resource.Properties.Count());

                    var creditLimitProperty = Assert.IsType<ODataProperty>(Assert.Single(resourceState.Resource.Properties.Where(d => d.Name.Equals("CreditLimit"))));
                    var untypedValue = Assert.IsType<ODataUntypedValue>(creditLimitProperty.Value);
                    Assert.Equal("1730", untypedValue.RawValue);
                });
        }

        [Fact]
        public async Task ReadResourceContentWithUndeclaredButAnnotatedPrimitivePropertyForNonOpenTypeAsync()
        {
            this.messageReaderSettings.Validations &= ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType;

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Sue\"," +
                "\"CreditLimit@odata.type\":\"#Edm.Decimal\"," +
                "\"CreditLimit\":1730}";

            await SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                payload,
                this.customersEntitySet,
                this.customerEntityType,
                (resourceState) =>
                {
                    Assert.Equal(3, resourceState.Resource.Properties.Count());

                    var creditLimitProperty = Assert.IsType<ODataProperty>(Assert.Single(resourceState.Resource.Properties.Where(d => d.Name.Equals("CreditLimit"))));
                    Assert.Equal(1730M, creditLimitProperty.Value);
                });
        }

        [Fact]
        public async Task ReadResourceContentWithUndeclaredComplexPropertySingletonForNonOpenTypeAsync()
        {
            this.messageReaderSettings.Validations &= ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType;

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Sue\"," +
                "\"HomeAddress@odata.type\":\"#NS.Address\"," +
                "\"HomeAddress\":{\"Street\":\"Street 2\",\"City\":\"City 2\"}}";

            await SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                payload,
                this.customersEntitySet,
                this.customerEntityType,
                (resourceState) =>
                {
                    Assert.Equal(2, resourceState.Resource.Properties.Count());

                    var odataTypeAnnotation = Assert.Single(resourceState.PropertyAndAnnotationCollector.GetODataPropertyAnnotations("HomeAddress"));
                    Assert.Equal("odata.type", odataTypeAnnotation.Key);
                    Assert.Equal("NS.Address", odataTypeAnnotation.Value);
                });
        }

        [Fact]
        public async Task ReadResourceContentWithUndeclaredComplexPropertySingletonForNonOpenTypeAsync_ThrowsExceptionForValueIsJsonArray()
        {
            this.messageReaderSettings.Validations &= ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType;

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Sue\"," +
                "\"HomeAddress@odata.type\":\"#NS.Address\"," +
                "\"HomeAddress\":[{\"Street\":\"Street 2\",\"City\":\"City 2\"}]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                    payload,
                    this.customersEntitySet,
                    this.customerEntityType,
                    (resourceState) => { }));

            Assert.Equal(
                ErrorStrings.ODataJsonResourceDeserializer_CannotReadSingletonNestedResource("StartArray", "HomeAddress"),
                exception.Message);
        }

        [Fact]
        public async Task ReadResourceContentWithUndeclaredComplexCollectionPropertyForNonOpenTypeAsync()
        {
            this.messageReaderSettings.Validations &= ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType;

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Sue\"," +
                "\"PhysicalAddresses@odata.type\":\"#Collection(NS.Address)\"," +
                "\"PhysicalAddresses\":[{\"Street\":\"Street 2\",\"City\":\"City 2\"}]}";

            await SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                payload,
                this.customersEntitySet,
                this.customerEntityType,
                (resourceState) =>
                {
                    Assert.Equal(2, resourceState.Resource.Properties.Count());

                    var odataTypeAnnotation = Assert.Single(resourceState.PropertyAndAnnotationCollector.GetODataPropertyAnnotations("PhysicalAddresses"));
                    Assert.Equal("odata.type", odataTypeAnnotation.Key);
                    Assert.Equal("Collection(NS.Address)", odataTypeAnnotation.Value);
                });
        }

        [Fact]
        public async Task ReadResourceContentWithUndeclaredComplexCollectionPropertyForNonOpenTypeAsync_ThrowsExceptionForValueIsJsonObject()
        {
            this.messageReaderSettings.Validations &= ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType;

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Sue\"," +
                "\"PhysicalAddresses@odata.type\":\"#Collection(NS.Address)\"," +
                "\"PhysicalAddresses\":{\"Street\":\"Street 2\",\"City\":\"City 2\"}}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                    payload,
                    this.customersEntitySet,
                    this.customerEntityType,
                    (resourceState) => { }));

            Assert.Equal(
                ErrorStrings.ReaderValidationUtils_NullNamedValueForNullableType("PhysicalAddresses", "Collection(NS.Address)"),
                exception.Message);
        }

        [Fact]
        public async Task ReadResourceContentWithUndeclaredEntityCollectionPropertyForNonOpenTypeAsync_ThrowsExceptionForValueIsJsonObject()
        {
            this.messageReaderSettings.Validations &= ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType;

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Sue\"," +
                "\"ReviewedProducts@odata.type\":\"#Collection(NS.Product)\"," +
                "\"ReviewedProducts\":{\"Id\":1,\"Name\":\"Tea\"}}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                    payload,
                    this.customersEntitySet,
                    this.customerEntityType,
                    (resourceState) => { }));

            Assert.Equal(
                ErrorStrings.ODataJsonResourceDeserializer_CannotReadCollectionNestedResource("StartObject", "ReviewedProducts"),
                exception.Message);
        }

        [Fact]
        public async Task ReadResourceContentWithUndeclaredExpandedSingletonNavigationPropertyInResponsePayloadAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Food\"," +
                "\"WorstSeller@odata.type\":\"#NS.Product\"," +
                "\"WorstSeller@odata.associationLink\":\"http://tempuri.org/Categories(1)/WorstSeller\"," +
                "\"WorstSeller@odata.navigationLink\":\"http://tempuri.org/Products(2)\"," +
                "\"WorstSeller\":{\"Id\":2,\"Name\":\"Coffee\"}}";

            await SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                payload,
                this.categoriesEntitySet,
                this.categoryEntityType,
                (resourceState) =>
                {
                    commonVerifyAction(resourceState);

                    var odataPropertyAnnotations = resourceState.PropertyAndAnnotationCollector.GetODataPropertyAnnotations("WorstSeller");

                    var odataType = Assert.Contains("odata.type", odataPropertyAnnotations);
                    var odataAssociationLink = (Uri)Assert.Contains("odata.associationLink", odataPropertyAnnotations);
                    var odataNavigationLink = (Uri)Assert.Contains("odata.navigationLink", odataPropertyAnnotations);

                    Assert.Equal("NS.Product", odataType);
                    Assert.Equal("http://tempuri.org/Categories(1)/WorstSeller", odataAssociationLink.AbsoluteUri);
                    Assert.Equal("http://tempuri.org/Products(2)", odataNavigationLink.AbsoluteUri);
                });
        }

        [Fact]
        public async Task ReadResourceContentWithUndeclaredNonExpandedSingletonNavigationPropertyInResponsePayloadAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Food\"," +
                "\"WorstSeller@odata.type\":\"#NS.Product\"," +
                "\"WorstSeller@odata.associationLink\":\"http://tempuri.org/Categories(1)/WorstSeller\"," +
                "\"WorstSeller@odata.navigationLink\":\"http://tempuri.org/Products(2)\"}";

            await SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                payload,
                this.categoriesEntitySet,
                this.categoryEntityType,
                (resourceState) =>
                {
                    commonVerifyAction(resourceState);

                    var odataPropertyAnnotations = resourceState.PropertyAndAnnotationCollector.GetODataPropertyAnnotations("WorstSeller");

                    var odataType = Assert.Contains("odata.type", odataPropertyAnnotations);
                    var odataAssociationLink = (Uri)Assert.Contains("odata.associationLink", odataPropertyAnnotations);
                    var odataNavigationLink = (Uri)Assert.Contains("odata.navigationLink", odataPropertyAnnotations);

                    Assert.Equal("NS.Product", odataType);
                    Assert.Equal("http://tempuri.org/Categories(1)/WorstSeller", odataAssociationLink.AbsoluteUri);
                    Assert.Equal("http://tempuri.org/Products(2)", odataNavigationLink.AbsoluteUri);
                });
        }

        [Fact]
        public async Task ReadResourceContentWithUndeclaredExpandedSingletonComplexPropertyInResponsePayloadAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Food\"," +
                "\"WarehouseAddress\":{\"@odata.type\":\"#NS.Address\",\"Id\":2,\"Name\":\"Coffee\"}}";

            await SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                payload,
                this.categoriesEntitySet,
                this.categoryEntityType,
                (resourceState) =>
                {
                    commonVerifyAction(resourceState);
                });
        }

        [Fact]
        public async Task ReadResourceContentWithUndeclaredExpandedCollectionComplexPropertyInResponsePayloadAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Food\"," +
                "\"WarehouseAddresses@odata.type\":\"#Collection(NS.Address)\"," +
                "\"WarehouseAddresses\":[{\"Id\":2,\"Name\":\"Coffee\"}]}";

            await SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                payload,
                this.categoriesEntitySet,
                this.categoryEntityType,
                (resourceState) =>
                {
                    commonVerifyAction(resourceState);

                    var odataPropertyAnnotations = resourceState.PropertyAndAnnotationCollector.GetODataPropertyAnnotations("WarehouseAddresses");
                    var odataType = Assert.Contains("odata.type", odataPropertyAnnotations);
                    Assert.Equal("Collection(NS.Address)", odataType);
                });
        }

        [Fact]
        public async Task ReadResourceContentWithStreamPropertyAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Products/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Tea\"," +
                "\"Photo\":\"AQIDBAUGBwgJAA==\"}";

            await SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                payload,
                this.productsEntitySet,
                this.productEntityType,
                (resourceState) =>
                {
                    Assert.Equal(3, resourceState.Resource.Properties.Count());

                    var photoProperty = Assert.IsType<ODataProperty>(Assert.Single(resourceState.Resource.Properties.Where(d => d.Name.Equals("Photo"))));
                    var streamReferenceValue = Assert.IsType<ODataStreamReferenceValue>(photoProperty.Value);

                    Assert.Equal("http://tempuri.org/Products(1)/Photo", streamReferenceValue.EditLink.AbsoluteUri);
                    Assert.Equal("http://tempuri.org/Products(1)/Photo", streamReferenceValue.ReadLink.AbsoluteUri);
                    Assert.Null(streamReferenceValue.ContentType);
                    Assert.Null(streamReferenceValue.ETag);
                });
        }

        [Fact]
        public async Task ReadResourceContentWithDeferredStreamPropertyAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Products/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Tea\"," +
                "\"Photo@odata.type\":\"#Edm.Stream\"}";

            await SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                payload,
                this.productsEntitySet,
                this.productEntityType,
                (resourceState) =>
                {
                    Assert.Equal(3, resourceState.Resource.Properties.Count());

                    var photoProperty = Assert.IsType<ODataProperty>(Assert.Single(resourceState.Resource.Properties.Where(d => d.Name.Equals("Photo"))));
                    var streamReferenceValue = Assert.IsType<ODataStreamReferenceValue>(photoProperty.Value);

                    Assert.Equal("http://tempuri.org/Products(1)/Photo", streamReferenceValue.EditLink.AbsoluteUri);
                    Assert.Equal("http://tempuri.org/Products(1)/Photo", streamReferenceValue.ReadLink.AbsoluteUri);
                    Assert.Null(streamReferenceValue.ContentType);
                    Assert.Null(streamReferenceValue.ETag);
                });
        }

        public static IEnumerable<object[]> GetReadResourceContentWithNestedComplexPropertyTestData()
        {
            yield return new object[]
            {
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Sue\"," +
                "\"BillingAddress\":{\"@odata.type\":\"#NS.Address\",\"Street\":\"Street 1\",\"City\":\"City 1\"}}"
            };

            yield return new object[]
            {
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Sue\"," +
                "\"ShippingAddresses@odata.type\":\"#Collection(NS.Address)\"," +
                "\"ShippingAddresses\":[{\"@odata.type\":\"#NS.Address\",\"Street\":\"Street 1\",\"City\":\"City 1\"}]}"
            };
        }

        [Theory]
        [MemberData(nameof(GetReadResourceContentWithNestedComplexPropertyTestData))]
        public async Task ReadResourceContentWithNestedComplexPropertyAsync(string payload)
        {
            await SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                payload,
                this.customersEntitySet,
                this.customerEntityType,
                (resourceState) =>
                {
                    var resource = resourceState.Resource;
                    Assert.NotNull(resource);
                    Assert.Equal(2, resource.Properties.Count());
                    var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                    Assert.Equal(2, properties.Length);
                    var idProperty = properties[0];
                    var nameProperty = properties[1];
                    Assert.Equal("Id", idProperty.Name);
                    Assert.Equal(1, idProperty.Value);
                    Assert.Equal("Name", nameProperty.Name);
                    Assert.Equal("Sue", nameProperty.Value);
                });
        }

        [Fact]
        public async Task ReadTopLevelResourceSetAnnotationsAsync()
        {
            this.messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("attr.remark");
            var categoriesResourceSet = CreateResourceSet("Categories", "NS.Categories");

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories\"," +
                "\"@odata.count\":2," +
                "\"@odata.type\":\"#Collection(NS.Category)\"," +
                "\"@odata.nextLink\":\"http://tempuri.org/Categories/nextLink\"," +
                "\"@attr.remark\":\"Collection\"," +
                "\"value\":[{\"Id\":1,\"Name\":\"Food\"}]}";

            await SetupJsonResourceSerializerAndRunReadTopLevelDeltaResourceSetAnnotationsAsync(
                categoriesResourceSet,
                payload,
                (resourceSet, propertyAndAnnotationCollector) =>
                {
                    var odataScopeAnnotations = propertyAndAnnotationCollector.GetODataScopeAnnotation();

                    var odataType = Assert.Contains("odata.type", odataScopeAnnotations);
                    var odataNextLink = Assert.Contains("odata.nextLink", odataScopeAnnotations);
                    var odataCount = Assert.Contains("odata.count", odataScopeAnnotations);

                    Assert.Equal("#Collection(NS.Category)", odataType);
                    Assert.Equal("http://tempuri.org/Categories/nextLink", odataNextLink);
                    Assert.Equal(2, odataCount);

                    var customInstanceAnnotation = Assert.Single(resourceSet.InstanceAnnotations);
                    Assert.Equal("attr.remark", customInstanceAnnotation.Name);
                    var annotationValue = Assert.IsType<ODataPrimitiveValue>(customInstanceAnnotation.Value);
                    Assert.Equal("Collection", annotationValue.Value);
                });
        }

        [Fact]
        public async Task ReadTopLevelResourceSetAnnotationsWithMetadataReferencePropertyAsync()
        {
            var categoriesResourceSet = CreateResourceSet("Categories", "NS.Categories");

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories\"," +
                "\"#NS.RateCategories\":[{\"title\":\"RateCategories\",\"target\":\"http://tempuri.org/RateCategories\",\"IgnoreProp\":true}]," +
                "\"#NS.Top2Categories\":{\"title\":\"Top2Categories\",\"target\":\"http://tempuri.org/Top2Categories\",\"IgnoreProp\":true}," +
                "\"value\":[{\"Id\":1,\"Name\":\"Food\"}]}";

            await SetupJsonResourceSerializerAndRunReadTopLevelDeltaResourceSetAnnotationsAsync(
                categoriesResourceSet,
                payload,
                (resourceSet, propertyAndAnnotationCollector) =>
                {
                    var rateCategoriesAction = Assert.Single(((ODataResourceSet)resourceSet).Actions);
                    var top2CategoriesFunction = Assert.Single(((ODataResourceSet)resourceSet).Functions);

                    Assert.Equal("RateCategories", rateCategoriesAction.Title);
                    Assert.Equal("http://tempuri.org/RateCategories", rateCategoriesAction.Target.AbsoluteUri);
                    Assert.Equal("Top2Categories", top2CategoriesFunction.Title);
                    Assert.Equal("http://tempuri.org/Top2Categories", top2CategoriesFunction.Target.AbsoluteUri);
                });
        }

        [Fact]
        public async Task ReadTopLevelResourceSetAnnotationsWithReorderingAsync()
        {
            this.messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("attr.remark");
            var categoriesResourceSet = CreateResourceSet("Categories", "NS.Categories");

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories\"," +
                "\"@odata.count\":2," +
                "\"value\":[{\"Id\":1,\"Name\":\"Food\"}]," +
                "\"@odata.nextLink\":\"http://tempuri.org/Categories/nextLink\"}";

            await SetupJsonResourceSerializerAndRunReadTopLevelDeltaResourceSetAnnotationsAsync(
                categoriesResourceSet,
                payload,
                (resourceSet, propertyAndAnnotationCollector) =>
                {
                    // Reading does not stop at resource set (i.e., value) property
                    var odataScopeAnnotations = propertyAndAnnotationCollector.GetODataScopeAnnotation();

                    var odataNextLink = Assert.Contains("odata.nextLink", odataScopeAnnotations);
                    var odataCount = Assert.Contains("odata.count", odataScopeAnnotations);

                    Assert.Equal("http://tempuri.org/Categories/nextLink", odataNextLink);
                    Assert.Equal(2, odataCount);
                },
                readAllResourceSetProperties: true);
        }

        [Fact]
        public async Task ReadTopLevelDeltaResourceSetAnnotationsAsync()
        {
            this.messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("attr.remark");
            var categoriesDeltaResourceSet = CreateDeltaResourceSet("Categories", "NS.Categories", new Uri("http://tempuri.org/Categories/deltaLink"));

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories\"," +
                "\"@odata.count\":2," +
                "\"@odata.type\":\"#Collection(NS.Category)\"," +
                "\"@odata.deltaLink\":\"http://tempuri.org/Categories/deltaLink\"," +
                "\"@attr.remark\":\"Collection\"," +
                "\"value\":[{\"Id\":1,\"Name\":\"Food\"}]}";

            await SetupJsonResourceSerializerAndRunReadTopLevelDeltaResourceSetAnnotationsAsync(
                categoriesDeltaResourceSet,
                payload,
                (resourceSet, propertyAndAnnotationCollector) =>
                {
                    var odataScopeAnnotations = propertyAndAnnotationCollector.GetODataScopeAnnotation();

                    var odataType = Assert.Contains("odata.type", odataScopeAnnotations);
                    var odataDeltaLink = Assert.Contains("odata.deltaLink", odataScopeAnnotations);
                    var odataCount = Assert.Contains("odata.count", odataScopeAnnotations);

                    Assert.Equal("#Collection(NS.Category)", odataType);
                    Assert.Equal("http://tempuri.org/Categories/deltaLink", odataDeltaLink);
                    Assert.Equal(2, odataCount);

                    var customInstanceAnnotation = Assert.Single(resourceSet.InstanceAnnotations);
                    Assert.Equal("attr.remark", customInstanceAnnotation.Name);
                    var annotationValue = Assert.IsType<ODataPrimitiveValue>(customInstanceAnnotation.Value);
                    Assert.Equal("Collection", annotationValue.Value);
                });
        }

        [Fact]
        public async Task ReadTopLevelResourceSetAnnotationsAsync_IgnoresUnrecognizedOperation()
        {
            var categoriesResourceSet = CreateResourceSet("Categories", "NS.Categories");
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories\"," +
                "\"#NS.UnrecognizedOperation\":{\"title\":\"UnrecognizedOperation\",\"target\":\"http://tempuri.org/UnrecognizedOperation\"}," +
                "\"value\":[{\"Id\":1,\"Name\":\"Food\"}]}";

            await SetupJsonResourceSerializerAndRunReadTopLevelDeltaResourceSetAnnotationsAsync(
                categoriesResourceSet,
                payload,
                (resourceSet, propertyAndAnnotationCollector) =>
                {
                    Assert.Empty(((ODataResourceSet)resourceSet).Actions);
                });
        }

        [Fact]
        public async Task ReadNextLinkAnnotationAtTopLevelResourceSetEndAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories\"," +
                "\"@odata.count\":2," +
                "\"value\":[{\"Id\":1,\"Name\":\"Food\"}]," +
                "\"@odata.nextLink\":\"http://tempuri.org/Categories?$skip=1\"}";

            var resourceSet = CreateResourceSet("Categories", "NS.Category");

            using (var jsonInputContext = CreateJsonInputContext(payload, this.model, isResponse: true))
            {
                var jsonResourceDeserializer = new ODataJsonResourceDeserializer(jsonInputContext);
                var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);

                await jsonResourceDeserializer.ReadPayloadStartAsync(
                    ODataPayloadKind.ResourceSet,
                    propertyAndAnnotationCollector,
                    false,
                    true);

                await jsonResourceDeserializer.ReadNextLinkAnnotationAtResourceSetEndAsync(
                    resourceSet,
                    expandedNestedResourceInfo: null,
                    propertyAndAnnotationCollector);

                var odataScopeAnnotationKvPair = Assert.Single(propertyAndAnnotationCollector.GetODataScopeAnnotation());

                Assert.Equal("odata.count", odataScopeAnnotationKvPair.Key);
                Assert.Equal(2, odataScopeAnnotationKvPair.Value);
            }
        }

        [Fact]
        public async Task ReadNextLinkAnnotationAtResourceSetEndAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories(Products())/$entity\"," +
                "\"Products\":[{\"Id\":1,\"Name\":\"Tea\"},{\"Id\":2,\"Name\":\"Coffee\"}]," +
                "\"Products@odata.nextLink\":\"http://tempuri.org/Categories(1)/Products?$skip=2\"," +
                "\"Products@odata.count\":3}";

            var productsResourceSet = CreateResourceSet("Products", "NS.Product");
            var expandedNestedResourceInfo = CreateProductsNavigationPropertyNestedResourceInfo(productsResourceSet);

            await SetupJsonResourceSerializerAndRunReadNextLinkAnnotationAtResourceSetEndAsync(
                productsResourceSet,
                expandedNestedResourceInfo,
                payload,
                (nestedResourceSet) =>
                {
                    Assert.Equal("http://tempuri.org/Categories(1)/Products?$skip=2", nestedResourceSet.NextPageLink.AbsoluteUri);
                    Assert.Equal(3, nestedResourceSet.Count);
                });
        }

        [Fact]
        public async Task ReadResourceContentAsync_CanReadDeferredPrimitiveProperty()
        {
            this.messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Products/$entity\"," +
                "\"Id\":1," +
                "\"Name@odata.type\":\"#Edm.String\"," +
                "\"Name@custom.annotation\":\"abc\"}";

               await SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                    payload,
                    this.productsEntitySet,
                    this.productEntityType,
                    (resourceState) =>
                    {
                        var odataPropertyAnnotations = resourceState.PropertyAndAnnotationCollector.GetODataPropertyAnnotations("Name");
                        KeyValuePair<string, object> odataAnnotation = Assert.Single(odataPropertyAnnotations);
                        Assert.Equal("odata.type", odataAnnotation.Key);
                        Assert.Equal("Edm.String", odataAnnotation.Value);

                        var customAnnotations = resourceState.PropertyAndAnnotationCollector.GetCustomPropertyAnnotations("Name");
                        KeyValuePair<string, object> customAnnotation = Assert.Single(customAnnotations);
                        Assert.Equal("custom.annotation", customAnnotation.Key);
                        Assert.Equal("abc", customAnnotation.Value);
                    });
        }

        [Theory]
        [InlineData("{\"title\":\"Top5Products\",\"title\":\"Top5Products\",\"target\":\"http://tempuri.org/Top5Products\"}", "title")]
        [InlineData("{\"title\":\"Top5Products\",\"target\":\"http://tempuri.org/Top5Products\",\"target\":\"http://tempuri.org/Top5Products\"}", "target")]
        public async Task ReadResourceContentAsync_ThrowsExceptionForRepeatedPropertyInOperation(string operationPart, string repeatedProperty)
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Food\"," +
                $"\"#NS.Top5Products\":{operationPart}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                    payload,
                    this.categoriesEntitySet,
                    this.categoryEntityType,
                    (resourceState) => { }));

            Assert.Equal(
                ErrorStrings.ODataJsonResourceDeserializer_MultipleOptionalPropertiesInOperation(repeatedProperty, "#NS.Top5Products"),
                exception.Message);
        }

        [Fact]
        public async Task ReadResourceContentAsync_ThrowsExceptionForOperationMissingTargetProperty()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Food\"," +
                "\"#NS.Top5Products\":[{\"title\":\"Top5Products\"}]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                    payload,
                    this.categoriesEntitySet,
                    this.categoryEntityType,
                    (resourceState) => { }));

            Assert.Equal(
                ErrorStrings.ODataJsonResourceDeserializer_OperationMissingTargetProperty("#NS.Top5Products"),
                exception.Message);
        }

        [Fact]
        public async Task ReadResourceContentAsync_ThrowsExceptionForOperationPropertyValueNotObject()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Food\"," +
                "\"#NS.Top5Products\":\"Top5Products\"}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                    payload,
                    this.categoriesEntitySet,
                    this.categoryEntityType,
                    (resourceState) => { }));

            Assert.Equal(
                ErrorStrings.ODataJsonOperationsDeserializerUtils_OperationsPropertyMustHaveObjectValue("#NS.Top5Products", JsonNodeType.PrimitiveValue),
                exception.Message);
        }

        [Fact]
        public async Task ReadResourceContentAsync_ThrowsExceptionForODataIdAnnotationPrecededByProperty()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"," +
                "\"Id\":1," +
                "\"@odata.id\":\"http://tempuri.org/Categories(1)\"," +
                "\"Name\":\"Food\"}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                    payload,
                    this.categoriesEntitySet,
                    this.categoryEntityType,
                    (resourceState) => { }));

            Assert.Equal(
                ErrorStrings.ODataJsonResourceDeserializer_ResourceInstanceAnnotationPrecededByProperty("odata.id"),
                exception.Message);
        }

        [Fact]
        public async Task ReadResourceContentAsync_ThrowsExceptionForODataEtagAnnotationPrecededByProperty()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"," +
                "\"Id\":1," +
                "\"@odata.etag\":\"etag\"," +
                "\"Name\":\"Food\"}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                    payload,
                    this.categoriesEntitySet,
                    this.categoryEntityType,
                    (resourceState) => { }));

            Assert.Equal(
                ErrorStrings.ODataJsonResourceDeserializer_ResourceInstanceAnnotationPrecededByProperty("odata.etag"),
                exception.Message);
        }

        [Fact]
        public async Task ReadResourceContentAsync_ThrowsExceptionForUnexpectedODataRemovedAnnotation()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"," +
                "\"@odata.removed\":{\"reason\":\"deleted\"}," +
                "\"Id\":1," +
                "\"Name\":\"Food\"}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                    payload,
                    this.categoriesEntitySet,
                    this.categoryEntityType,
                    (resourceState) => { }));

            Assert.Equal(
                ErrorStrings.ODataJsonResourceDeserializer_UnexpectedDeletedEntryInResponsePayload,
                exception.Message);
        }

        [Fact]
        public async Task ReadResourceContentAsync_ThrowsExceptionForODataBindPropertyAnnotationValueIsEmptyJsonArray()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Food\"," +
                "\"Products@odata.bind\":[]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                    payload,
                    this.categoriesEntitySet,
                    this.categoryEntityType,
                    (resourceState) => { }));

            Assert.Equal(
                ErrorStrings.ODataJsonResourceDeserializer_EmptyBindArray("odata.bind"),
                exception.Message);
        }

        [Fact]
        public async Task ReadResourceContentAsync_ThrowsExceptionForODataTypeAnnotationInsidePrimitiveValue()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Food\"," +
                "\"WarehousePin\":{\"@odata.type\":\"#Edm.GeographyPoint\",\"type\":\"Point\",\"coordinates\":[22.2,22.2],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                    payload,
                    this.categoriesEntitySet,
                    this.categoryEntityType,
                    (resourceState) => { }));

            Assert.Equal(
                ErrorStrings.ODataJsonPropertyAndValueDeserializer_ODataTypeAnnotationInPrimitiveValue("odata.type"),
                exception.Message);
        }

        [Fact]
        public async Task ReadEntryInstanceAnnotationAsync_ThrowsExceptionForMultipleODataTypeAnnotation()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"," +
                "\"@odata.type\":\"NS.Category\"," +
                "\"@odata.type\":\"NS.Category\"," +
                "\"Id\":1," +
                "\"Name\":\"Food\"}";

            using (var jsonInputContext = CreateJsonInputContext(payload, this.model, isResponse: true))
            {
                var jsonResourceDeserializer = new ODataJsonResourceDeserializer(jsonInputContext);

                await jsonResourceDeserializer.ReadPayloadStartAsync(
                    ODataPayloadKind.Resource,
                    new PropertyAndAnnotationCollector(true),
                    false,
                    true);

                var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);

                await jsonResourceDeserializer.JsonReader.ReadAsync(); // Read over property name
                var annotationValue = await jsonResourceDeserializer.ReadEntryInstanceAnnotationAsync(
                    annotationName: "odata.type",
                    anyPropertyFound: false,
                    typeAnnotationFound: false,
                    propertyAndAnnotationCollector);
                Assert.Equal("NS.Category", annotationValue);

                await jsonResourceDeserializer.JsonReader.ReadAsync(); // Read over property name
                var exception = await Assert.ThrowsAsync<ODataException>(
                    () => jsonResourceDeserializer.ReadEntryInstanceAnnotationAsync(
                        annotationName: "odata.type",
                        anyPropertyFound: false,
                        typeAnnotationFound: true, // Type annotation was in a previous call
                        propertyAndAnnotationCollector));

                Assert.Equal(
                    ErrorStrings.ODataJsonResourceDeserializer_ResourceTypeAnnotationNotFirst,
                    exception.Message);
            }
        }

        [Fact]
        public async Task ReadResourceContentAsync_ThrowsExceptionForUnexpectedODataDeltaPropertyAnnotation()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"," +
                "\"Id\":1," +
                "\"Name@odata.deltaLink\":\"http://tempuri.org/Categories(1)/Name/deltaLink\"," +
                "\"Name\":\"Food\"}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
                    payload,
                    this.categoriesEntitySet,
                    this.categoryEntityType,
                    (resourceState) => { }));

            Assert.Equal(
                ErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedAnnotationProperties("odata.deltaLink"),
                exception.Message);
        }

        [Fact]
        public async Task ReadTopLevelResourceSetAnnotationsAsync_ThrowsExceptionForMissingValueProperty()
        {
            this.messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("attr.remark");
            var categoriesResourceSet = CreateResourceSet("Categories", "NS.Categories");

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories\"}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonResourceSerializerAndRunReadTopLevelDeltaResourceSetAnnotationsAsync(
                    categoriesResourceSet,
                    payload,
                    (resourceSet, propertyAndAnnotationCollector) => { }));

            Assert.Equal(
                ErrorStrings.ODataJsonResourceDeserializer_ExpectedResourceSetPropertyNotFound("value"),
                exception.Message);
        }

        [Theory]
        [InlineData("{\"title\":\"RateCategories\",\"title\":\"RateCategories\",\"target\":\"http://tempuri.org/RateCategories\"}", "title")]
        [InlineData("{\"title\":\"RateCategories\",\"target\":\"http://tempuri.org/RateCategories\",\"target\":\"http://tempuri.org/RateCategories\"}", "target")]
        public async Task ReadTopLevelResourceSetAnnotationsAsync_ThrowsExceptionForRepeatedPropertyInOperation(string operationPart, string repeatedProperty)
        {
            var categoriesResourceSet = CreateResourceSet("Categories", "NS.Categories");

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories\"," +
                $"\"#NS.RateCategories\":{operationPart}," +
                "\"value\":[{\"Id\":1,\"Name\":\"Food\"}]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonResourceSerializerAndRunReadTopLevelDeltaResourceSetAnnotationsAsync(
                    categoriesResourceSet,
                    payload,
                    (resourceSet, propertyAndAnnotationCollector) => { }));

            Assert.Equal(
                ErrorStrings.ODataJsonResourceDeserializer_MultipleOptionalPropertiesInOperation(repeatedProperty, "#NS.RateCategories"),
                exception.Message);
        }

        [Fact]
        public async Task ReadTopLevelResourceSetAnnotationsAsync_ThrowsExceptionForOperationMissingTargetProperty()
        {
            var categoriesResourceSet = CreateResourceSet("Categories", "NS.Categories");

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories\"," +
                "\"#NS.RateCategories\":[{\"title\":\"RateCategories\"}]," +
                "\"value\":[{\"Id\":1,\"Name\":\"Food\"}]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonResourceSerializerAndRunReadTopLevelDeltaResourceSetAnnotationsAsync(
                    categoriesResourceSet,
                    payload,
                    (resourceSet, propertyAndAnnotationCollector) => { }));

            Assert.Equal(
                ErrorStrings.ODataJsonResourceDeserializer_OperationMissingTargetProperty("#NS.RateCategories"),
                exception.Message);
        }

        [Fact]
        public async Task ReadTopLevelResourceSetAnnotationsAsync_ThrowsExceptionForOperationPropertyValueNotObject()
        {
            var categoriesResourceSet = CreateResourceSet("Categories", "NS.Categories");

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories\"," +
                "\"#NS.RateCategories\":\"RateCategories\"," +
                "\"value\":[{\"Id\":1,\"Name\":\"Food\"}]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonResourceSerializerAndRunReadTopLevelDeltaResourceSetAnnotationsAsync(
                    categoriesResourceSet,
                    payload,
                    (resourceSet, propertyAndAnnotationCollector) => { }));

            Assert.Equal(
                ErrorStrings.ODataJsonOperationsDeserializerUtils_OperationsPropertyMustHaveObjectValue("#NS.RateCategories", JsonNodeType.PrimitiveValue),
                exception.Message);
        }

        [Fact]
        public async Task ReadTopLevelResourceSetAnnotationsAsync_ThrowsExceptionForRequiredPropertyWithoutValue()
        {
            var categoriesResourceSet = CreateResourceSet("Categories", "NS.Categories");

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories\"," +
                "\"value@attr.remark\":\"Missing value property\"}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonResourceSerializerAndRunReadTopLevelDeltaResourceSetAnnotationsAsync(
                    categoriesResourceSet,
                    payload,
                    (resourceSet, propertyAndAnnotationCollector) => { }));

            Assert.Equal(
                ErrorStrings.ODataJsonResourceDeserializer_InvalidPropertyAnnotationInTopLevelResourceSet("value"),
                exception.Message);
        }

        [Fact]
        public async Task ReadTopLevelResourceSetAnnotationsAsync_ThrowsExceptionForUnexpectedProperty()
        {
            var categoriesResourceSet = CreateResourceSet("Categories", "NS.Categories");

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories\"," +
                "\"Id\":1," +
                "\"value\":[{\"Id\":1,\"Name\":\"Food\"}]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonResourceSerializerAndRunReadTopLevelDeltaResourceSetAnnotationsAsync(
                    categoriesResourceSet,
                    payload,
                    (resourceSet, propertyAndAnnotationCollector) => { }));

            Assert.Equal(
                ErrorStrings.ODataJsonResourceDeserializer_InvalidPropertyInTopLevelResourceSet("Id", "value"),
                exception.Message);
        }

        [Fact]
        public async Task ReadTopLevelDeltaResourceSetAnnotationsAsync_ThrowsExceptionForUnexpectedMetadataReferenceProperty()
        {
            var categoriesDeltaResourceSet = CreateDeltaResourceSet("Categories", "NS.Categories", new Uri("http://tempuri.org/Categories/deltaLink"));

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories\"," +
                "\"#NS.RateCategories\":[{\"title\":\"RateCategories\",\"target\":\"http://tempuri.org/RateCategories\"}]," +
                "\"value\":[{\"Id\":1,\"Name\":\"Food\"}]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonResourceSerializerAndRunReadTopLevelDeltaResourceSetAnnotationsAsync(
                    categoriesDeltaResourceSet,
                    payload,
                    (resourceSet, propertyAndAnnotationCollector) => { }));

            Assert.Equal(
                ErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty("#NS.RateCategories"),
                exception.Message);
        }

        [Fact]
        public async Task ReadResourceSetContentAsync_ThrowsExceptionForValuePropertyNotAnArray()
        {
            using (var jsonInputContext = CreateJsonInputContext("{\"value\":{}}", model))
            {
                var jsonResourceDeserializer = new ODataJsonResourceDeserializer(jsonInputContext);
                var jsonReader = jsonResourceDeserializer.JsonReader;

                await AdvanceReaderToFirstPropertyAsync(jsonReader);
                await jsonReader.ReadAsync();

                var exception = await Assert.ThrowsAsync<ODataException>(
                    () => jsonResourceDeserializer.ReadResourceSetContentStartAsync());

                Assert.Equal(
                    ErrorStrings.ODataJsonResourceDeserializer_CannotReadResourceSetContentStart("StartObject"),
                    exception.Message);
            }
        }

        [Theory]
        [InlineData("{\"@odata.removed\":{\"reason\":\"deleted\"}}", ODataVersion.V4)]
        [InlineData("{\"@removed\":{\"reason\":\"deleted\"}}", ODataVersion.V401)]
        public async Task ReadDeletedResourceAsync_ThrowsExceptionForMissingODataIdAnnotation(string payload, ODataVersion odataVersion)
        {
            this.messageReaderSettings.Version = odataVersion;

            using (var jsonInputContext = CreateJsonInputContext(payload, model))
            {
                var jsonResourceDeserializer = new ODataJsonResourceDeserializer(jsonInputContext);

                await AdvanceReaderToFirstPropertyAsync(jsonResourceDeserializer.JsonReader);

                var exception = await Assert.ThrowsAsync<ODataException>(
                    () => jsonResourceDeserializer.ReadDeletedResourceAsync());

                Assert.Equal(
                    ErrorStrings.ODataWriterCore_DeltaResourceWithoutIdOrKeyProperties,
                    exception.Message);
            }
        }

        [Theory]
        [InlineData("{\"@odata.removed\":\"NaO\"}", ODataVersion.V4)]
        [InlineData("{\"@removed\":\"NaO\"}", ODataVersion.V401)]
        public async Task ReadDeletedResourceAsync_ThrowsExceptionForODataRemovedAnnotationValueNotAnObject(string payload, ODataVersion odataVersion)
        {
            this.messageReaderSettings.Version = odataVersion;

            using (var jsonInputContext = CreateJsonInputContext(payload, model))
            {
                var jsonResourceDeserializer = new ODataJsonResourceDeserializer(jsonInputContext);

                await AdvanceReaderToFirstPropertyAsync(jsonResourceDeserializer.JsonReader);

                var exception = await Assert.ThrowsAsync<ODataException>(
                    () => jsonResourceDeserializer.ReadDeletedResourceAsync());

                Assert.Equal(
                    ErrorStrings.ODataJsonResourceDeserializer_DeltaRemovedAnnotationMustBeObject("NaO"),
                    exception.Message);
            }
        }

        [Theory]
        [InlineData("{\"id\":\"http://tempuri.org/Customers(1)\",\"reason\":\"deleted\",\"nested\":{\"foo\":\"bar\"}}")]
        [InlineData("{\"id\":\"http://tempuri.org/Customers(1)\",\"nested\":{\"foo\":\"bar\"},\"reason\":\"deleted\"}")]
        [InlineData("{\"nested\":{\"foo\":\"bar\"},\"id\":\"http://tempuri.org/Customers(1)\",\"reason\":\"deleted\"}")]
        [InlineData("{\"id\":\"http://tempuri.org/Customers(1)\",\"reason\":\"deleted\",\"nested\":[{\"foo\":\"bar\"}]}")]
        [InlineData("{\"id\":\"http://tempuri.org/Customers(1)\",\"nested\":[{\"foo\":\"bar\"}],\"reason\":\"deleted\"}")]
        [InlineData("{\"nested\":[{\"foo\":\"bar\"}],\"id\":\"http://tempuri.org/Customers(1)\",\"reason\":\"deleted\"}")]
        public async Task ReadDeletedEntryAsync_ThrowsExceptionForNestedContent(string payload)
        {
            using (var jsonInputContext = CreateJsonInputContext(payload, model))
            {
                var jsonResourceDeserializer = new ODataJsonResourceDeserializer(jsonInputContext);

                await AdvanceReaderToFirstPropertyAsync(jsonResourceDeserializer.JsonReader);

                var exception = await Assert.ThrowsAsync<ODataException>(
                    () => jsonResourceDeserializer.ReadDeletedEntryAsync());

                Assert.Equal(
                    ErrorStrings.ODataWriterCore_NestedContentNotAllowedIn40DeletedEntry,
                    exception.Message);
            }
        }

        [Theory]
        [InlineData("{\"id\":\"http://tempuri.org/Customers(1)\",\"reason\":\"deleted\",\"nested\":{\"foo\":\"bar\"}}")]
        [InlineData("{\"id\":\"http://tempuri.org/Customers(1)\",\"nested\":{\"foo\":\"bar\"},\"reason\":\"deleted\"}")]
        [InlineData("{\"nested\":{\"foo\":\"bar\"},\"id\":\"http://tempuri.org/Customers(1)\",\"reason\":\"deleted\"}")]
        [InlineData("{\"id\":\"http://tempuri.org/Customers(1)\",\"reason\":\"deleted\",\"nested\":[{\"foo\":\"bar\"}]}")]
        [InlineData("{\"id\":\"http://tempuri.org/Customers(1)\",\"nested\":[{\"foo\":\"bar\"}],\"reason\":\"deleted\"}")]
        [InlineData("{\"nested\":[{\"foo\":\"bar\"}],\"id\":\"http://tempuri.org/Customers(1)\",\"reason\":\"deleted\"}")]
        public void ReadDeletedEntry_ThrowsExceptionForNestedContent(string payload)
        {
            using (var jsonInputContext = CreateJsonInputContext(payload: payload, model: model, isAsync: false))
            {
                var jsonResourceDeserializer = new ODataJsonResourceDeserializer(jsonInputContext);

                AdvanceReaderToFirstProperty(jsonResourceDeserializer.JsonReader);

                var exception = Assert.Throws<ODataException>(
                    () => jsonResourceDeserializer.ReadDeletedEntry());

                Assert.Equal(
                    ErrorStrings.ODataWriterCore_NestedContentNotAllowedIn40DeletedEntry,
                    exception.Message);
            }
        }

        [Fact]
        public async Task ReadNextLinkAnnotationAtResourceSetEndAsync_ThrowsExceptionForDuplicateODataNextLinkAnnotation()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories(Products())/$entity\"," +
                "\"Products\":[{\"Id\":1,\"Name\":\"Tea\"},{\"Id\":2,\"Name\":\"Coffee\"}]," +
                "\"Products@odata.nextLink\":\"http://tempuri.org/Categories(1)/Products?$skip=2\"," +
                "\"Products@odata.nextLink\":\"http://tempuri.org/Categories(1)/Products?$skip=2\"}";

            var productsResourceSet = CreateResourceSet("Products", "NS.Product");
            var expandedNestedResourceInfo = CreateProductsNavigationPropertyNestedResourceInfo(productsResourceSet);

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonResourceSerializerAndRunReadNextLinkAnnotationAtResourceSetEndAsync(
                    productsResourceSet,
                    expandedNestedResourceInfo,
                    payload,
                    (nestedResourceSet) => { }));

            Assert.Equal(
                ErrorStrings.ODataJsonResourceDeserializer_DuplicateNestedResourceSetAnnotation("odata.nextLink", "Products"),
                exception.Message);
        }

        [Fact]
        public async Task ReadNextLinkAnnotationAtResourceSetEndAsync_ThrowsExceptionForDuplicateODataCountAnnotation()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories(Products())/$entity\"," +
                "\"Products\":[{\"Id\":1,\"Name\":\"Tea\"},{\"Id\":2,\"Name\":\"Coffee\"}]," +
                "\"Products@odata.count\":3," +
                "\"Products@odata.count\":3}";

            var productsResourceSet = CreateResourceSet("Products", "NS.Product");
            var expandedNestedResourceInfo = CreateProductsNavigationPropertyNestedResourceInfo(productsResourceSet);

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonResourceSerializerAndRunReadNextLinkAnnotationAtResourceSetEndAsync(
                    productsResourceSet,
                    expandedNestedResourceInfo,
                    payload,
                    (nestedResourceSet) => { }));

            Assert.Equal(
                ErrorStrings.ODataJsonResourceDeserializer_DuplicateNestedResourceSetAnnotation("odata.count", "Products"),
                exception.Message);
        }

        [Fact]
        public async Task ReadNextLinkAnnotationAtResourceSetEndAsync_ThrowsExceptionForUnexpectedODataDeltaLinkAnnotation()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories(Products())/$entity\"," +
                "\"Products\":[{\"Id\":1,\"Name\":\"Tea\"},{\"Id\":2,\"Name\":\"Coffee\"}]," +
                "\"Products@odata.deltaLink\":\"http://tempuri.org/Categories(1)/Products/delta\"}";

            var productsResourceSet = CreateResourceSet("Products", "NS.Product");
            var expandedNestedResourceInfo = CreateProductsNavigationPropertyNestedResourceInfo(productsResourceSet);

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonResourceSerializerAndRunReadNextLinkAnnotationAtResourceSetEndAsync(
                    productsResourceSet,
                    expandedNestedResourceInfo,
                    payload,
                    (nestedResourceSet) => { }));

            Assert.Equal(
                ErrorStrings.ODataJsonResourceDeserializer_UnexpectedPropertyAnnotationAfterExpandedResourceSet("odata.deltaLink", "Products"),
                exception.Message);
        }

        [Fact]
        public async Task ReadNextLinkAnnotationAtResourceSetEndAsync_ThrowsExceptionForResourceSetEndAnnotationsInRequestPayload()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Categories(Products())/$entity\"," +
                "\"Products\":[{\"Id\":1,\"Name\":\"Tea\"},{\"Id\":2,\"Name\":\"Coffee\"}]," +
                "\"Products@odata.nextLink\":\"http://tempuri.org/Categories(1)/Products?$skip=2\"}";

            var productsResourceSet = CreateResourceSet("Products", "NS.Product");
            var expandedNestedResourceInfo = CreateProductsNavigationPropertyNestedResourceInfo(productsResourceSet);

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonResourceSerializerAndRunReadNextLinkAnnotationAtResourceSetEndAsync(
                    productsResourceSet,
                    expandedNestedResourceInfo,
                    payload,
                    (nestedResourceSet) => { },
                    isResponse: false));

            Assert.Equal(
                ErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedPropertyAnnotation("Products", "odata.nextLink"),
                exception.Message);
        }

        private Action<TestJsonReaderResourceState> commonVerifyAction = (resourceState) =>
        {
            var resource = resourceState.Resource;
            Assert.NotNull(resource);
            Assert.Equal(2, resource.Properties.Count());
            var properties = resource.Properties.OfType<ODataProperty>().ToArray();
            var idProperty = properties[0];
            var nameProperty = properties[1];
            Assert.Equal("Id", idProperty.Name);
            Assert.Equal(1, idProperty.Value);
            Assert.Equal("Name", nameProperty.Name);
            Assert.Equal("Food", nameProperty.Value);
        };

        private async Task SetupJsonResourceSerializerAndRunReadResourceContextTestAsync(
            string payload,
            IEdmNavigationSource navigationSource,
            IEdmStructuredType structuredType,
            Action<TestJsonReaderResourceState> verifyAction,
            bool isResponse = true)
        {
            using (var jsonInputContext = CreateJsonInputContext(payload, this.model, isResponse: isResponse))
            {
                var jsonResourceDeserializer = new ODataJsonResourceDeserializer(jsonInputContext);

                await jsonResourceDeserializer.ReadPayloadStartAsync(
                    ODataPayloadKind.Resource,
                    new PropertyAndAnnotationCollector(true),
                    false,
                    true);

                var resourceState = new TestJsonReaderResourceState(navigationSource, structuredType);

                await jsonResourceDeserializer.ReadResourceContentAsync(resourceState);

                verifyAction(resourceState);
            }
        }

        private async Task SetupJsonResourceSerializerAndRunReadTopLevelDeltaResourceSetAnnotationsAsync(
            ODataResourceSetBase resourceSet,
            string payload,
            Action<ODataResourceSetBase, PropertyAndAnnotationCollector> verifyAction,
            bool forResourceSetStart = true,
            bool readAllResourceSetProperties = false)
        {
            using (var jsonInputContext = CreateJsonInputContext(payload, this.model, isResponse: true))
            {
                var jsonResourceDeserializer = new ODataJsonResourceDeserializer(jsonInputContext);
                var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);

                await jsonResourceDeserializer.ReadPayloadStartAsync(
                    ODataPayloadKind.ResourceSet,
                    propertyAndAnnotationCollector,
                    false,
                    true);

                await jsonResourceDeserializer.ReadTopLevelResourceSetAnnotationsAsync(
                    resourceSet,
                    propertyAndAnnotationCollector,
                    forResourceSetStart: forResourceSetStart,
                    readAllResourceSetProperties: readAllResourceSetProperties);

                verifyAction(resourceSet, propertyAndAnnotationCollector);
            }
        }

        private async Task SetupJsonResourceSerializerAndRunReadNextLinkAnnotationAtResourceSetEndAsync(
            ODataResourceSetBase nestedResourceSet,
            ODataJsonReaderNestedResourceInfo expandedNestedResourceInfo,
            string payload,
            Action<ODataResourceSetBase> verifyAction,
            bool isResponse = true)
        {
            using (var jsonInputContext = CreateJsonInputContext(payload, this.model, isResponse: isResponse))
            {
                var jsonResourceDeserializer = new ODataJsonResourceDeserializer(jsonInputContext);
                var propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);
                var jsonReader = jsonResourceDeserializer.JsonReader;

                await jsonResourceDeserializer.ReadPayloadStartAsync(
                    ODataPayloadKind.Resource,
                    propertyAndAnnotationCollector,
                    false,
                    true);

                // Advance reader to end of resource set
                await jsonReader.ReadAsync(); // Read over expanded navigation property
                await jsonReader.SkipValueAsync();

                await jsonResourceDeserializer.ReadNextLinkAnnotationAtResourceSetEndAsync(
                    nestedResourceSet,
                    expandedNestedResourceInfo: expandedNestedResourceInfo,
                    propertyAndAnnotationCollector);

                verifyAction(nestedResourceSet);
            }
        }

        private static async Task AdvanceReaderToFirstPropertyAsync(BufferingJsonReader bufferingJsonReader)
        {
            await bufferingJsonReader.ReadAsync(); // Position the reader on the first node
            await bufferingJsonReader.ReadAsync(); // Read StartObject
            Assert.Equal(JsonNodeType.Property, bufferingJsonReader.NodeType);
        }

        private static void AdvanceReaderToFirstProperty(BufferingJsonReader bufferingJsonReader)
        {
            bufferingJsonReader.Read(); // Position the reader on the first node
            bufferingJsonReader.Read(); // Read StartObject
            Assert.Equal(JsonNodeType.Property, bufferingJsonReader.NodeType);
        }

        private void InitializeModel()
        {
            this.model = new EdmModel();

            var segmentEnumType = new EdmEnumType("NS", "Segment");
            segmentEnumType.AddMember("Retail", new EdmEnumMemberValue(0));
            segmentEnumType.AddMember("Wholesale", new EdmEnumMemberValue(1));
            this.model.AddElement(segmentEnumType);

            var addressComplexType = new EdmComplexType("NS", "Address");
            addressComplexType.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String);
            addressComplexType.AddStructuralProperty("City", EdmPrimitiveTypeKind.String);
            this.model.AddElement(addressComplexType);

            this.customerEntityType = new EdmEntityType("NS", "Customer");
            this.customerEntityType.AddKeys(this.customerEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            this.customerEntityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            this.customerEntityType.AddStructuralProperty("BillingAddress", new EdmComplexTypeReference(addressComplexType, true));
            this.customerEntityType.AddStructuralProperty(
                "ShippingAddresses",
                new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(addressComplexType, true))));
            this.model.AddElement(this.customerEntityType);

            this.categoryEntityType = new EdmEntityType("NS", "Category", baseType: null, isAbstract: false, isOpen: true, hasStream: true);
            this.productEntityType = new EdmEntityType("NS", "Product");

            this.categoryEntityType.AddKeys(this.categoryEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            this.categoryEntityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            this.categoryEntityType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo
                {
                    Name = "BestSeller",
                    Target = this.productEntityType,
                    TargetMultiplicity = EdmMultiplicity.ZeroOrOne
                });
            this.categoryEntityType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo
                {
                    Name = "Products",
                    Target = this.productEntityType,
                    TargetMultiplicity = EdmMultiplicity.Many
                });
            this.model.AddElement(this.categoryEntityType);

            this.productEntityType.AddKeys(this.productEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            this.productEntityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            this.productEntityType.AddStructuralProperty("Photo", EdmPrimitiveTypeKind.Stream);
            productEntityType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo
                {
                    Name = "Category",
                    Target = this.categoryEntityType,
                    TargetMultiplicity = EdmMultiplicity.One
                });
            this.model.AddElement(this.productEntityType);

            var defaultContainer = model.AddEntityContainer("NS", "Default");

            var rateProductsAction = new EdmAction(namespaceName: "NS", name: "RateProducts", returnType: null, isBound: true, entitySetPathExpression: null);
            rateProductsAction.AddParameter("bindingParameter", new EdmEntityTypeReference(this.categoryEntityType, true));
            model.AddElement(rateProductsAction);

            var top5ProductsFunction = new EdmFunction(
                namespaceName: "NS",
                name: "Top5Products",
                returnType: new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(this.productEntityType, true))),
                isBound: true,
                entitySetPathExpression: null,
                isComposable: false);
            top5ProductsFunction.AddParameter("bindingParameter", new EdmEntityTypeReference(this.categoryEntityType, true));
            this.model.AddElement(top5ProductsFunction);

            var rateCategoriesAction = new EdmAction(namespaceName: "NS", name: "RateCategories", returnType: null, isBound: false, entitySetPathExpression: null);
            this.model.AddElement(rateCategoriesAction);

            var top2CategoriesFunction = new EdmFunction(
                namespaceName: "NS",
                name: "Top2Categories",
                returnType: new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(this.categoryEntityType, true))),
                isBound: false,
                entitySetPathExpression: null,
                isComposable: false);
            this.model.AddElement(top2CategoriesFunction);

            defaultContainer.AddActionImport(rateProductsAction);
            defaultContainer.AddFunctionImport(top5ProductsFunction);
            defaultContainer.AddActionImport(rateCategoriesAction);
            defaultContainer.AddFunctionImport(top2CategoriesFunction);

            this.customersEntitySet = defaultContainer.AddEntitySet("Customers", this.customerEntityType);
            this.categoriesEntitySet = defaultContainer.AddEntitySet("Categories", this.categoryEntityType);
            this.productsEntitySet = defaultContainer.AddEntitySet("Products", this.productEntityType);
        }

        private ODataJsonInputContext CreateJsonInputContext(
            string payload,
            IEdmModel model,
            bool isResponse = true,
            bool isAsync = true,
            bool isIeee754Compatible = false)
        {
            var messageInfo = new ODataMessageInfo
            {
                IsResponse = isResponse,
                MediaType = CreateMediaType(isIeee754Compatible),
                IsAsync = isAsync,
                Model = model,
            };

            return new ODataJsonInputContext(
                new StringReader(payload), messageInfo, this.messageReaderSettings);
        }

        private ODataMediaType CreateMediaType(bool isIeee754Compatible = false)
        {
            return new ODataMediaType(
                MimeConstants.MimeApplicationType,
                MimeConstants.MimeJsonSubType,
                new[]{
                    new KeyValuePair<string, string>(
                        MimeConstants.MimeMetadataParameterName,
                        MimeConstants.MimeMetadataParameterValueMinimal),
                    new KeyValuePair<string, string>(
                        MimeConstants.MimeStreamingParameterName,
                        MimeConstants.MimeParameterValueTrue),
                    new KeyValuePair<string, string>(
                        MimeConstants.MimeIeee754CompatibleParameterName,
                        isIeee754Compatible ? MimeConstants.MimeParameterValueTrue : MimeConstants.MimeParameterValueFalse)
                });
        }

        #region Helper Methods

        private static ODataResourceSet CreateResourceSet(
            string navigationSourceName,
            string entityTypeName,
            EdmNavigationSourceKind navigationSourceKind = EdmNavigationSourceKind.EntitySet)
        {
            return new ODataResourceSet
            {
                TypeName = $"Collection({entityTypeName})",
                SerializationInfo = new ODataResourceSerializationInfo
                {
                    NavigationSourceName = navigationSourceName,
                    ExpectedTypeName = entityTypeName,
                    NavigationSourceEntityTypeName = entityTypeName,
                    NavigationSourceKind = navigationSourceKind
                }
            };
        }

        private static ODataDeltaResourceSet CreateDeltaResourceSet(
            string navigationSource,
            string entityTypeName,
            Uri deltaLink = null,
            EdmNavigationSourceKind navigationSourceKind = EdmNavigationSourceKind.EntitySet)
        {
            return new ODataDeltaResourceSet
            {
                TypeName = $"Collection({entityTypeName})",
                DeltaLink = deltaLink,
                SerializationInfo = new ODataResourceSerializationInfo
                {
                    NavigationSourceName = navigationSource,
                    ExpectedTypeName = entityTypeName,
                    NavigationSourceEntityTypeName = entityTypeName,
                    NavigationSourceKind = navigationSourceKind
                }
            };
        }

        private ODataJsonReaderNestedResourceInfo CreateProductsNavigationPropertyNestedResourceInfo(ODataResourceSet productsResourceSet)
        {
            var nestedResourceInfo = new ODataNestedResourceInfo
            {
                Name = "Products",
                Url = new Uri("http://tempuri.org/Categories(1)/Products"),
                IsCollection = true,
                SerializationInfo = new ODataNestedResourceInfoSerializationInfo(),
            };

            return ODataJsonReaderNestedResourceInfo.CreateResourceSetReaderNestedResourceInfo(
                nestedResourceInfo,
                this.categoryEntityType.FindProperty("Products"),
                this.productEntityType,
                productsResourceSet);
        }

        #endregion
    }

    internal class TestJsonReaderResourceState : IODataJsonReaderResourceState
    {
        private ODataResourceBase resource = ReaderUtils.CreateNewResource();
        private IEdmNavigationSource navigationSource;
        private IEdmStructuredType structuredType;
        private PropertyAndAnnotationCollector propertyAndAnnotationCollector = new PropertyAndAnnotationCollector(true);

        public TestJsonReaderResourceState(IEdmNavigationSource navigationSource, IEdmStructuredType structuredType)
        {
            this.navigationSource = navigationSource;
            this.structuredType = structuredType;
        }

        public ODataResourceBase Resource => this.resource;

        public IEdmStructuredType ResourceType => this.structuredType;

        public IEdmStructuredType ResourceTypeFromMetadata { get; set; }

        public IEdmNavigationSource NavigationSource => this.navigationSource;

        public ODataResourceMetadataBuilder MetadataBuilder { get; set; }

        public bool AnyPropertyFound { get; set; }

        public ODataJsonReaderNestedInfo FirstNestedInfo { get; set; }

        public PropertyAndAnnotationCollector PropertyAndAnnotationCollector => this.propertyAndAnnotationCollector;

        public SelectedPropertiesNode SelectedProperties => new SelectedPropertiesNode(SelectedPropertiesNode.SelectionType.EntireSubtree);

        public List<string> NavigationPropertiesRead => throw new NotImplementedException();

        public bool ProcessingMissingProjectedNestedResourceInfos { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}

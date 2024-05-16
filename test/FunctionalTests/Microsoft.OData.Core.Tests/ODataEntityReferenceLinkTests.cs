//---------------------------------------------------------------------
// <copyright file="ODataEntityReferenceLinkTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests
{
    public class ODataEntityReferenceLinkTests
    {
        private readonly static IEdmModel EdmModel;
        private readonly static ODataMessageReaderSettings MessageReaderSettingsReadAndValidateCustomInstanceAnnotations;
        private static readonly EdmEntityType EntityType;
        private static readonly EdmEntitySet EntitySet;
        private EdmModel model;
        private EdmEntityType orderEntityType;
        private EdmEntityType customerEntityType;
        private EdmEntitySet orderEntitySet;
        private EdmEntitySet customerEntitySet;

        public ODataEntityReferenceLinkTests()
        {
            this.InitializeEdmModel();
        }

        static ODataEntityReferenceLinkTests()
        {
            EdmModel tmpModel = new EdmModel();
            EdmComplexType complexType = new EdmComplexType("TestNamespace", "TestComplexType");
            complexType.AddProperty(new EdmStructuralProperty(complexType, "StringProperty", EdmCoreModel.Instance.GetString(false)));
            tmpModel.AddElement(complexType);

            EntityType = new EdmEntityType("TestNamespace", "TestEntityType");
            tmpModel.AddElement(EntityType);
            var keyProperty = new EdmStructuralProperty(EntityType, "ID", EdmCoreModel.Instance.GetInt32(false));
            EntityType.AddKeys(new IEdmStructuralProperty[] { keyProperty });
            EntityType.AddProperty(keyProperty);

            var defaultContainer = new EdmEntityContainer("TestNamespace", "DefaultContainer_sub");
            tmpModel.AddElement(defaultContainer);
            EntitySet = new EdmEntitySet(defaultContainer, "Customers", EntityType);
            defaultContainer.AddElement(EntitySet);

            EdmModel = TestUtils.WrapReferencedModelsToMainModel("TestNamespace", "DefaultContainer", tmpModel);
            MessageReaderSettingsReadAndValidateCustomInstanceAnnotations = new ODataMessageReaderSettings { ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*") };
        }

        [Fact]
        public void ShouldBeAbleToSetAndClearIdOnEntityReferenceLink()
        {
            ODataEntityReferenceLink referencelink = new ODataEntityReferenceLink();
            Assert.Null(referencelink.Url);
            referencelink.Url = new Uri("http://my/Id");
            Assert.Equal(new Uri("http://my/Id"), referencelink.Url);
            referencelink.Url = null;
            Assert.Null(referencelink.Url);
        }

        [Fact]
        public void InstanceAnnotationsPropertyShouldNotBeNullAtCreation()
        {
            ODataEntityReferenceLink referencelink = new ODataEntityReferenceLink();
            Assert.NotNull(referencelink.InstanceAnnotations);
            Assert.Empty(referencelink.InstanceAnnotations);
        }

        [Fact]
        public void InstanceAnnotationsPropertyShouldReturnAWritableCollectionAtCreation()
        {
            ODataEntityReferenceLink referencelink = new ODataEntityReferenceLink();
            Assert.NotNull(referencelink.InstanceAnnotations);
            referencelink.InstanceAnnotations.Add(new ODataInstanceAnnotation("TestNamespace.name", new ODataPrimitiveValue("value")));
            Assert.Equal(1, referencelink.InstanceAnnotations.Count);
        }

        [Fact]
        public void SetNullValueToInstanceAnnotationsPropertyShouldThrow()
        {
            ODataEntityReferenceLink referencelink = new ODataEntityReferenceLink();
            Action test = () => referencelink.InstanceAnnotations = null;
            Assert.Throws<ArgumentNullException>("value", test);
        }

        [Fact]
        public void ShouldWriteForEntityReferenceLink()
        {
            ODataEntityReferenceLink referencelink = new ODataEntityReferenceLink
            {
                Url = new Uri("http://host/Customers(1)")
            };
            referencelink.InstanceAnnotations.Add(new ODataInstanceAnnotation("Is.New", new ODataPrimitiveValue(true)));
            WriteAndValidate(referencelink, "{\"@odata.context\":\"http://odata.org/test/$metadata#$ref\",\"@odata.id\":\"http://host/Customers(1)\",\"@Is.New\":true}", writingResponse: false);
            WriteAndValidate(referencelink, "{\"@odata.context\":\"http://odata.org/test/$metadata#$ref\",\"@odata.id\":\"http://host/Customers(1)\",\"@Is.New\":true}", writingResponse: true);
        }

        [Fact]
        public void ShouldReadForEntityReferenceLink()
        {
            var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"@odata.context\":\"http://odata.org/test/$metadata#$ref\",\"@odata.id\":\"http://host/Customers(1)\",\"@TestNamespace.unknown\":123,\"@custom.annotation\":456}");
            ODataEntityReferenceLink link = deserializer.ReadEntityReferenceLink();
            Assert.Equal("http://host/Customers(1)", link.Url.ToString());
            Assert.Equal(2, link.InstanceAnnotations.Count);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(123), link.InstanceAnnotations.Single(ia => ia.Name == "TestNamespace.unknown").Value);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(456), link.InstanceAnnotations.Single(ia => ia.Name == "custom.annotation").Value);
        }

        [Fact]
        public void ReadForEntityReferenceLinkIDAppearBeforeContextShouldThrow()
        {
            var deserializer = this.CreateJsonEntryAndFeedDeserializer("{\"@odata.id\":\"http://host/Customers(1)\",\"@odata.context\":\"http://odata.org/test/$metadata#$ref\",\"@TestNamespace.unknown\":123,\"@custom.annotation\":456}");
            Action readResult = () => deserializer.ReadEntityReferenceLink();
            readResult.Throws<ODataException>(ODataErrorStrings.ODataJsonDeserializer_ContextLinkNotFoundAsFirstProperty);
        }

        [Fact]
        public void ReadForEntityReferenceLinkWithoutIDButAnnotationShouldThrow()
        {
            string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#$ref\",\"@odataa.unknown\":123}";
            var deserializer = this.CreateJsonEntryAndFeedDeserializer(payload);
            Action readResult = () => deserializer.ReadEntityReferenceLink();
            readResult.Throws<ODataException>(ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_MissingEntityReferenceLinkProperty(ODataAnnotationNames.ODataId));
        }

        [Fact]
        public void ReadForEntityReferenceLinkOnlyContextShouldThrow()
        {
            string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#$ref\"}";
            var deserializer = this.CreateJsonEntryAndFeedDeserializer(payload);
            Action readResult = () => deserializer.ReadEntityReferenceLink();
            readResult.Throws<ODataException>(ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_MissingEntityReferenceLinkProperty(ODataAnnotationNames.ODataId));
        }

        [Fact]
        public void ReadForEntityReferenceLinkWithEmptyPayloadShouldThrow()
        {
            string payload = "{}";
            var deserializer = this.CreateJsonEntryAndFeedDeserializer(payload);
            Action readResult = () => deserializer.ReadEntityReferenceLink();
            readResult.Throws<ODataException>(ODataErrorStrings.ODataJsonDeserializer_ContextLinkNotFoundAsFirstProperty);
        }

        [Fact]
        public void ReadForEntityReferenceLinkAnnotationAppearBeforeIDShouldThrow()
        {
            string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#$ref\",\"@TestNamespace.unknown\":123,\"@odata.id\":\"http://host/Customers(1)\",\"@custom.annotation\":456}";
            var deserializer = this.CreateJsonEntryAndFeedDeserializer(payload);
            Action readResult = () => deserializer.ReadEntityReferenceLink();
            readResult.Throws<ODataException>(ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_MissingEntityReferenceLinkProperty(ODataAnnotationNames.ODataId));
        }

        [Fact]
        public void ReadForEntityReferenceLinkWithDuplicateAnnotationNameShouldNotThrow()
        {
            string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#$ref\",\"@odata.id\":\"http://host/Customers(1)\",\"@TestNamespace.unknown\":123,\"@TestNamespace.unknown\":456}";
            var deserializer = this.CreateJsonEntryAndFeedDeserializer(payload);
            Action readResult = () => deserializer.ReadEntityReferenceLink();
            readResult.DoesNotThrow();
        }

        [Fact]
        public void WriteForEntityReferenceLinkWithDuplicateAnnotationNameShouldThrow()
        {
            ODataEntityReferenceLink referencelink = new ODataEntityReferenceLink
            {
                Url = new Uri("http://host/Customers(1)")
            };
            referencelink.InstanceAnnotations.Add(new ODataInstanceAnnotation("TestNamespace.unknown", new ODataPrimitiveValue(123)));
            referencelink.InstanceAnnotations.Add(new ODataInstanceAnnotation("TestNamespace.unknown", new ODataPrimitiveValue(456)));
            string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#$ref\",\"@odata.id\":\"http://host/Customers(1)\",\"@TestNamespace.unknown\":123,\"@TestNamespace.unknown\":456}";
            Action writeResult = () => WriteAndValidate(referencelink, expectedPayload, writingResponse: false);
            writeResult.Throws<ODataException>(ODataErrorStrings.JsonInstanceAnnotationWriter_DuplicateAnnotationNameInCollection("TestNamespace.unknown"));
            writeResult = () => WriteAndValidate(referencelink, expectedPayload, writingResponse: true);
            writeResult.Throws<ODataException>(ODataErrorStrings.JsonInstanceAnnotationWriter_DuplicateAnnotationNameInCollection("TestNamespace.unknown"));
        }

        [Fact]
        public void ShouldReadAndWriteForEntityReferenceLink()
        {
            string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#$ref\",\"@odata.id\":\"http://host/Customers(1)\",\"@TestNamespace.unknown\":123,\"@custom.annotation\":456}";
            var deserializer = this.CreateJsonEntryAndFeedDeserializer(payload);
            ODataEntityReferenceLink link = deserializer.ReadEntityReferenceLink();
            WriteAndValidate(link, payload, writingResponse: false);
            WriteAndValidate(link, payload, writingResponse: true);
        }

        [Fact]
        public void ShouldWriteAndReadForEntityReferenceLink()
        {
            ODataEntityReferenceLink referencelink = new ODataEntityReferenceLink
            {
                Url = new Uri("http://host/Customers(1)")
            };
            referencelink.InstanceAnnotations.Add(new ODataInstanceAnnotation("TestNamespace.unknown", new ODataPrimitiveValue(123)));
            referencelink.InstanceAnnotations.Add(new ODataInstanceAnnotation("custom.annotation", new ODataPrimitiveValue(456)));
            string midplayoad = WriteToString(referencelink, writingResponse: false);
            var deserializer = this.CreateJsonEntryAndFeedDeserializer(midplayoad);
            ODataEntityReferenceLink link = deserializer.ReadEntityReferenceLink();
            SameEntityReferenceLink(referencelink, link);
        }

        [Fact]
        public async Task ReadEntityReferenceLinkAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#$ref\"," +
                "\"@odata.id\":\"http://tempuri.org/Orders(1)\"}";

            await SetupJsonEntityReferenceLinkDeserializerAndRunTestAsync(
                payload,
                async (jsonEntityReferenceLinkDeserializer) =>
                {
                    var entityReferenceLink = await jsonEntityReferenceLinkDeserializer.ReadEntityReferenceLinkAsync();

                    Assert.NotNull(entityReferenceLink);
                    Assert.Equal("http://tempuri.org/Orders(1)", entityReferenceLink.Url.AbsoluteUri);
                });
        }

        [Fact]
        public void ReadEntityReferenceLinks_ThrowsExceptionForInvalidPropertyAnnotationInEntityReferenceLinks()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Collection($ref)\"," +
                "\"value@custom.annotation\":\"foobar\"}";

            using (var jsonInputContext = CreateJsonInputContext(
                payload,
                this.model))
            {
                var jsonEntityReferenceLinkDeserializer = new ODataJsonEntityReferenceLinkDeserializer(jsonInputContext);

                var exception = Assert.Throws<ODataException>(
                    () => jsonEntityReferenceLinkDeserializer.ReadEntityReferenceLinks());

                Assert.Equal(
                    ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_InvalidPropertyAnnotationInEntityReferenceLinks("value"),
                    exception.Message);
            }
        }

        [Theory]
        [InlineData("\"UnexpectedProp\":\"foobar\"")]
        [InlineData("\"UnexpectedProp@custom.annotation\":\"foobar\"")]
        public void ReadEntityReferenceLink_ThrowsExceptionForInvalidPropertyOrAnnotationInEntityReferenceLink(string invalidPart)
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#$ref\"," +
                "\"@odata.id\":\"http://tempuri.org/Orders(1)\"," +
                $"{invalidPart}}}";

            using (var jsonInputContext = CreateJsonInputContext(
                payload,
                this.model))
            {
                var jsonEntityReferenceLinkDeserializer = new ODataJsonEntityReferenceLinkDeserializer(jsonInputContext);

                var exception = Assert.Throws<ODataException>(
                    () => jsonEntityReferenceLinkDeserializer.ReadEntityReferenceLink());

                Assert.Equal(
                    ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_InvalidAnnotationInEntityReferenceLink("UnexpectedProp"),
                    exception.Message);
            }
        }

        [Fact]
        public async Task ReadEntityReferenceLinkWithCustomAnnotationsAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#$ref\"," +
                "\"@odata.id\":\"http://tempuri.org/Orders(1)\"," +
                "\"@custom.annotation\":\"foobar\"}";

            await SetupJsonEntityReferenceLinkDeserializerAndRunTestAsync(
                payload,
                async (jsonEntityReferenceLinkDeserializer) =>
                {
                    var entityReferenceLink = await jsonEntityReferenceLinkDeserializer.ReadEntityReferenceLinkAsync();

                    Assert.NotNull(entityReferenceLink);
                    Assert.Equal("http://tempuri.org/Orders(1)", entityReferenceLink.Url.AbsoluteUri);

                    var annotation = Assert.Single(entityReferenceLink.GetInstanceAnnotations());
                    var customAnnotation = Assert.IsType<ODataPrimitiveValue>(annotation.Value);
                    Assert.Equal("foobar", customAnnotation.Value);
                });
        }

        [Fact]
        public async Task ReadEntityReferenceLinksAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Collection($ref)\"," +
                "\"value\":[" +
                "{\"@odata.id\":\"http://tempuri.org/Orders(1)\"}," +
                "{\"@odata.id\":\"http://tempuri.org/Orders(2)\"}]}";

            await SetupJsonEntityReferenceLinkDeserializerAndRunTestAsync(
                payload,
                async (jsonEntityReferenceLinkDeserializer) =>
                {
                    var entityReferenceLinks = await jsonEntityReferenceLinkDeserializer.ReadEntityReferenceLinksAsync();

                    Assert.NotNull(entityReferenceLinks);
                    Assert.Equal(2, entityReferenceLinks.Links.Count());
                    Assert.Equal("http://tempuri.org/Orders(1)", entityReferenceLinks.Links.First().Url.AbsoluteUri);
                    Assert.Equal("http://tempuri.org/Orders(2)", entityReferenceLinks.Links.Last().Url.AbsoluteUri);
                });
        }

        [Fact]
        public async Task ReadEntityReferenceLinksWithODataAnnotationsAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Collection($ref)\"," +
                "\"@odata.count\":3," +
                "\"value\":[" +
                "{\"@odata.id\":\"http://tempuri.org/Orders(1)\"}," +
                "{\"@odata.id\":\"http://tempuri.org/Orders(2)\"}]," +
                "\"@odata.nextLink\":\"http://tempuri.org/Customers(1)/Orders/nextLink\"}";

            await SetupJsonEntityReferenceLinkDeserializerAndRunTestAsync(
                payload,
                async (jsonEntityReferenceLinkDeserializer) =>
                {
                    var entityReferenceLinks = await jsonEntityReferenceLinkDeserializer.ReadEntityReferenceLinksAsync();

                    Assert.NotNull(entityReferenceLinks);
                    Assert.Equal(2, entityReferenceLinks.Links.Count());
                    Assert.Equal("http://tempuri.org/Orders(1)", entityReferenceLinks.Links.First().Url.AbsoluteUri);
                    Assert.Equal("http://tempuri.org/Orders(2)", entityReferenceLinks.Links.Last().Url.AbsoluteUri);

                    Assert.Equal(3, entityReferenceLinks.Count);
                    Assert.Equal("http://tempuri.org/Customers(1)/Orders/nextLink", entityReferenceLinks.NextPageLink.AbsoluteUri);
                });
        }

        [Fact]
        public async Task ReadEntityReferenceLinksWithCustomAnnotationsAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Collection($ref)\"," +
                "\"@pre.annotation\":\"foo\"," +
                "\"value\":[" +
                "{\"@odata.id\":\"http://tempuri.org/Orders(1)\"}]," +
                "\"@post.annotation\":\"bar\"}";

            await SetupJsonEntityReferenceLinkDeserializerAndRunTestAsync(
                payload,
                async (jsonEntityReferenceLinkDeserializer) =>
                {
                    var entityReferenceLinks = await jsonEntityReferenceLinkDeserializer.ReadEntityReferenceLinksAsync();

                    Assert.NotNull(entityReferenceLinks);
                    var entityReferenceLink = Assert.Single(entityReferenceLinks.Links);
                    Assert.Equal("http://tempuri.org/Orders(1)", entityReferenceLink.Url.AbsoluteUri);

                    var customAnnotations = entityReferenceLinks.GetInstanceAnnotations();
                    Assert.Equal(2, customAnnotations.Count());
                    var preAnnotation = Assert.IsType<ODataPrimitiveValue>(customAnnotations.First().Value);
                    var postAnnotation = Assert.IsType<ODataPrimitiveValue>(customAnnotations.Last().Value);
                    Assert.Equal("foo", preAnnotation.Value);
                    Assert.Equal("bar", postAnnotation.Value);
                });
        }

        [Fact]
        public async Task ReadEntityReferenceLinksAsync_ThrowsExceptionForUnexpectedODataAnnotation()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Collection($ref)\"," +
                "\"value\":[" +
                "{\"@odata.id\":\"http://tempuri.org/Orders(1)\"}]," +
                "\"@odata.deltaLink\":\"http://tempuri.org/Customers(1)/Orders/deltaLink\"}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonEntityReferenceLinkDeserializerAndRunTestAsync(
                    payload,
                    (jsonEntityReferenceLinkDeserializer) => jsonEntityReferenceLinkDeserializer.ReadEntityReferenceLinksAsync()));

            Assert.Equal(
                ODataErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedAnnotationProperties("odata.deltaLink"),
                exception.Message);
        }

        [Fact]
        public async Task ReadEntityReferenceLinksAsync_ThrowsExceptionForUnexpectedProperty()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Collection($ref)\"," +
                "\"UnexpectedProp\":\"foobar\"}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonEntityReferenceLinkDeserializerAndRunTestAsync(
                    payload,
                    (jsonEntityReferenceLinkDeserializer) => jsonEntityReferenceLinkDeserializer.ReadEntityReferenceLinksAsync()));

            Assert.Equal(
                ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_InvalidEntityReferenceLinksPropertyFound("UnexpectedProp", "value"),
                exception.Message);
        }

        [Fact]
        public async Task ReadEntityReferenceLinksAsync_ThrowsExceptionForUnsupportedODataPropertyAnnotation()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Collection($ref)\"," +
                "\"value@odata.count\":3}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonEntityReferenceLinkDeserializerAndRunTestAsync(
                    payload,
                    (jsonEntityReferenceLinkDeserializer) => jsonEntityReferenceLinkDeserializer.ReadEntityReferenceLinksAsync()));

            Assert.Equal(
                ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_PropertyAnnotationForEntityReferenceLinks,
                exception.Message);
        }

        [Fact]
        public async Task ReadEntityReferenceLinksAsync_ThrowsExceptionForExpectedEntityReferenceLinksPropertyNotFound()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Collection($ref)\"}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonEntityReferenceLinkDeserializerAndRunTestAsync(
                    payload,
                    (jsonEntityReferenceLinkDeserializer) => jsonEntityReferenceLinkDeserializer.ReadEntityReferenceLinksAsync()));

            Assert.Equal(
                ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_ExpectedEntityReferenceLinksPropertyNotFound("value"),
                exception.Message);
        }

        [Fact]
        public async Task ReadEntityReferenceLinksAsync_ThrowsExceptionForMetadataReferencePropertyInPayload()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Collection($ref)\"," +
                "\"#NS.Top2Orders\":{\"title\":\"Top2Orders\",\"target\":\"http://tempuri.org/Customers(1)/Top2Orders\"}}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonEntityReferenceLinkDeserializerAndRunTestAsync(
                    payload,
                    (jsonEntityReferenceLinkDeserializer) => jsonEntityReferenceLinkDeserializer.ReadEntityReferenceLinksAsync()));

            Assert.Equal(
                ODataErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty("#NS.Top2Orders"),
                exception.Message);
        }

        [Fact]
        public async Task ReadEntityReferenceLinksAsync_ThrowsExceptionForInvalidPropertyAnnotationInEntityReferenceLinks()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Collection($ref)\"," +
                "\"value@custom.annotation\":\"foobar\"}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonEntityReferenceLinkDeserializerAndRunTestAsync(
                    payload,
                    (jsonEntityReferenceLinkDeserializer) => jsonEntityReferenceLinkDeserializer.ReadEntityReferenceLinksAsync()));

            Assert.Equal(
                    ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_InvalidPropertyAnnotationInEntityReferenceLinks("value"),
                    exception.Message);
        }

        [Fact]
        public async Task ReadEntityReferenceLinksAsync_ThrowsExceptionForEntityReferenceLinkValueNotAnObject()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Collection($ref)\"," +
                "\"value\":[\"foobar\"]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonEntityReferenceLinkDeserializerAndRunTestAsync(
                    payload,
                    (jsonEntityReferenceLinkDeserializer) => jsonEntityReferenceLinkDeserializer.ReadEntityReferenceLinksAsync()));

            Assert.Equal(
                ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_EntityReferenceLinkMustBeObjectValue("PrimitiveValue"),
                exception.Message);
        }

        [Fact]
        public async Task ReadEntityReferenceLinksAsync_ThrowsExceptionForMultipleReferenceLinkUris()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Collection($ref)\"," +
                "\"value\":[{\"@odata.id\":\"http://tempuri.org/Orders(1)\",\"@odata.id\":\"http://tempuri.org/Orders(1)\"}]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonEntityReferenceLinkDeserializerAndRunTestAsync(
                    payload,
                    (jsonEntityReferenceLinkDeserializer) => jsonEntityReferenceLinkDeserializer.ReadEntityReferenceLinksAsync()));

            Assert.Equal(
                ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_MultipleUriPropertiesInEntityReferenceLink("odata.id"),
                exception.Message);
        }

        [Fact]
        public async Task ReadEntityReferenceLinksAsync_ThrowsExceptionForUnexpectedODataAnnotationEntityReferenceLink()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Collection($ref)\"," +
                "\"value\":[{\"@odata.nextLink\":\"http://tempuri.org/Customers(1)/Orders/nextLink\"}]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonEntityReferenceLinkDeserializerAndRunTestAsync(
                    payload,
                    (jsonEntityReferenceLinkDeserializer) => jsonEntityReferenceLinkDeserializer.ReadEntityReferenceLinksAsync()));

            Assert.Equal(
                ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_InvalidPropertyInEntityReferenceLink("odata.nextLink", "odata.id"),
                exception.Message);
        }

        [Fact]
        public async Task ReadEntityReferenceLinksAsync_ThrowsExceptionForEntityReferenceLinkUriAsNull()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Collection($ref)\"," +
                "\"value\":[{\"@odata.id\":null}]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonEntityReferenceLinkDeserializerAndRunTestAsync(
                    payload,
                    (jsonEntityReferenceLinkDeserializer) => jsonEntityReferenceLinkDeserializer.ReadEntityReferenceLinksAsync()));

            Assert.Equal(
                ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_EntityReferenceLinkUrlCannotBeNull("odata.id"),
                exception.Message);
        }

        [Fact]
        public async Task ReadEntityReferenceLinkAsync_ThrowsExceptionForCustomAnnotationBeforeEntityReferenceLinkUri()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#$ref\"," +
                "\"@custom.annotation\":\"foobar\"," +
                "\"@odata.id\":\"http://tempuri.org/Orders(1)\"}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonEntityReferenceLinkDeserializerAndRunTestAsync(
                    payload,
                    (jsonEntityReferenceLinkDeserializer) => jsonEntityReferenceLinkDeserializer.ReadEntityReferenceLinkAsync()));

            Assert.Equal(
                ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_MissingEntityReferenceLinkProperty("odata.id"),
                exception.Message);
        }

        [Theory]
        [InlineData("\"UnexpectedProp\":\"foobar\"")]
        [InlineData("\"UnexpectedProp@custom.annotation\":\"foobar\"")]
        public async Task ReadEntityReferenceLinkAsync_ThrowsExceptionForInvalidPropertyOrAnnotationInEntityReferenceLink(string invalidPart)
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#$ref\"," +
                "\"@odata.id\":\"http://tempuri.org/Orders(1)\"," +
                $"{invalidPart}}}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonEntityReferenceLinkDeserializerAndRunTestAsync(
                    payload,
                    (jsonEntityReferenceLinkDeserializer) => jsonEntityReferenceLinkDeserializer.ReadEntityReferenceLinkAsync()));

            Assert.Equal(
                ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_InvalidAnnotationInEntityReferenceLink("UnexpectedProp"),
                exception.Message);
        }

        [Fact]
        public async Task ReadEntityReferenceLinkAsync_ThrowsExceptionForUnexpectedMetadataReferenceProperty()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#$ref\"," +
                "\"#NS.Top2Orders\":{\"title\":\"Top2Orders\",\"target\":\"http://tempuri.org/Customers(1)/Top2Orders\"}}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonEntityReferenceLinkDeserializerAndRunTestAsync(
                    payload,
                    (jsonEntityReferenceLinkDeserializer) => jsonEntityReferenceLinkDeserializer.ReadEntityReferenceLinkAsync()));

            Assert.Equal(
                ODataErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty("#NS.Top2Orders"),
                exception.Message);
        }

        [Fact]
        public async Task ReadEntityReferenceLinkAsync_ThrowsExceptionForMissingEntityReferenceLinkProperty()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#$ref\"}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonEntityReferenceLinkDeserializerAndRunTestAsync(
                    payload,
                    (jsonEntityReferenceLinkDeserializer) => jsonEntityReferenceLinkDeserializer.ReadEntityReferenceLinkAsync()));

            Assert.Equal(
                ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_MissingEntityReferenceLinkProperty("odata.id"),
                exception.Message);
        }

        [Fact]
        public async Task ReadEntityReferenceLinkAsync_ThrowsExceptionForUnsupportedODataPropertyAnnotation()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#$ref\"," +
                "\"odata.id@odata.type\":\"#Edm.String\"}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonEntityReferenceLinkDeserializerAndRunTestAsync(
                    payload,
                    (jsonEntityReferenceLinkDeserializer) => jsonEntityReferenceLinkDeserializer.ReadEntityReferenceLinkAsync()));

            Assert.Equal(
                ODataErrorStrings.ODataJsonEntityReferenceLinkDeserializer_PropertyAnnotationForEntityReferenceLink("odata.type"),
                exception.Message);
        }

        private static void SameEntityReferenceLink(ODataEntityReferenceLink link1, ODataEntityReferenceLink link2)
        {
            Assert.NotNull(link1);
            Assert.NotNull(link2);
            Assert.Equal(link1.Url.ToString(), link2.Url.ToString());
            Assert.Equal(link1.InstanceAnnotations.Count, link2.InstanceAnnotations.Count);
            foreach (ODataInstanceAnnotation instanceannotation in link1.InstanceAnnotations)
            {
                TestUtils.AssertODataValueAreEqual(instanceannotation.Value, link2.InstanceAnnotations.Single(ia => ia.Name == instanceannotation.Name.ToString()).Value);
            }
        }

        private static string WriteToString(ODataEntityReferenceLink referencelink, bool writingResponse = true, bool synchronous = true)
        {
            MemoryStream stream = new MemoryStream();
            var outputContext = CreateJsonOutputContext(stream, writingResponse, synchronous);
            outputContext.WriteEntityReferenceLink(referencelink);
            stream.Seek(0, SeekOrigin.Begin);
            return (new StreamReader(stream)).ReadToEnd();
        }

        private static void WriteAndValidate(ODataEntityReferenceLink referencelink, string expectedPayload, bool writingResponse = true, bool synchronous = true)
        {
            string payload = WriteToString(referencelink, writingResponse, synchronous);
            Assert.Equal(payload, expectedPayload);
        }

        private static ODataJsonOutputContext CreateJsonOutputContext(MemoryStream stream, bool writingResponse = true, bool synchronous = true)
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
            settings.ShouldIncludeAnnotationInternal = ODataUtils.CreateAnnotationFilter("*");

            return new ODataJsonOutputContext(messageInfo, settings);
        }

        private ODataJsonEntityReferenceLinkDeserializer CreateJsonEntryAndFeedDeserializer(string payload, bool isIeee754Compatible = false)
        {
            var inputContext = this.CreateJsonInputContext(payload, EdmModel, isIeee754Compatible);

            return new ODataJsonEntityReferenceLinkDeserializer(inputContext);
        }

        private ODataJsonInputContext CreateJsonInputContext(string payload, IEdmModel model, bool isIeee754Compatible = false, bool isAsync = false, bool isResponse = true)
        {
            var mediaType = isIeee754Compatible
                ? new ODataMediaType("application", "json", new KeyValuePair<string, string>("IEEE754Compatible", "true"))
                : new ODataMediaType("application", "json", new KeyValuePair<string, string>("odata.streaming", "true"));

            var messageInfo = new ODataMessageInfo
            {
                IsResponse = isResponse,
                MediaType = mediaType,
                IsAsync = isAsync,
                Model = model,
            };

            return new ODataJsonInputContext(
                new StringReader(payload),
                messageInfo,
                MessageReaderSettingsReadAndValidateCustomInstanceAnnotations);
        }

        private async Task SetupJsonEntityReferenceLinkDeserializerAndRunTestAsync(
            string payload,
            Func<ODataJsonEntityReferenceLinkDeserializer, Task> func,
            bool isResponse = false)
        {
            using (var jsonInputContext = CreateJsonInputContext(
                payload,
                this.model,
                isIeee754Compatible: false,
                isAsync: true,
                isResponse: isResponse))
            {
                var jsonEntityReferenceLinkDeserializer = new ODataJsonEntityReferenceLinkDeserializer(jsonInputContext);

                await func(jsonEntityReferenceLinkDeserializer);
            }
        }

        private void InitializeEdmModel()
        {
            this.model = new EdmModel();
            this.orderEntityType = new EdmEntityType("NS", "Order");
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
    }
}

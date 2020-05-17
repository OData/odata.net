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
using Microsoft.OData.JsonLight;
using Microsoft.OData.Edm;
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
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@odata.context\":\"http://odata.org/test/$metadata#$ref\",\"@odata.id\":\"http://host/Customers(1)\",\"@TestNamespace.unknown\":123,\"@custom.annotation\":456}");
            ODataEntityReferenceLink link = deserializer.ReadEntityReferenceLink();
            Assert.Equal("http://host/Customers(1)", link.Url.ToString());
            Assert.Equal(2, link.InstanceAnnotations.Count);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(123), link.InstanceAnnotations.Single(ia => ia.Name == "TestNamespace.unknown").Value);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(456), link.InstanceAnnotations.Single(ia => ia.Name == "custom.annotation").Value);
        }

        [Fact]
        public void ReadForEntityReferenceLinkIDAppearBeforeContextShouldThrow()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@odata.id\":\"http://host/Customers(1)\",\"@odata.context\":\"http://odata.org/test/$metadata#$ref\",\"@TestNamespace.unknown\":123,\"@custom.annotation\":456}");
            Action readResult = () => deserializer.ReadEntityReferenceLink();
            readResult.Throws<ODataException>(ODataErrorStrings.ODataJsonLightDeserializer_ContextLinkNotFoundAsFirstProperty);
        }

        [Fact]
        public void ReadForEntityReferenceLinkWithoutIDButAnnotationShouldThrow()
        {
            string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#$ref\",\"@odataa.unknown\":123}";
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer(payload);
            Action readResult = () => deserializer.ReadEntityReferenceLink();
            readResult.Throws<ODataException>(ODataErrorStrings.ODataJsonLightEntityReferenceLinkDeserializer_MissingEntityReferenceLinkProperty(ODataAnnotationNames.ODataId));
        }

        [Fact]
        public void ReadForEntityReferenceLinkOnlyContextShouldThrow()
        {
            string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#$ref\"}";
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer(payload);
            Action readResult = () => deserializer.ReadEntityReferenceLink();
            readResult.Throws<ODataException>(ODataErrorStrings.ODataJsonLightEntityReferenceLinkDeserializer_MissingEntityReferenceLinkProperty(ODataAnnotationNames.ODataId));
        }

        [Fact]
        public void ReadForEntityReferenceLinkWithEmptyPayloadShouldThrow()
        {
            string payload = "{}";
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer(payload);
            Action readResult = () => deserializer.ReadEntityReferenceLink();
            readResult.Throws<ODataException>(ODataErrorStrings.ODataJsonLightDeserializer_ContextLinkNotFoundAsFirstProperty);
        }

        [Fact]
        public void ReadForEntityReferenceLinkAnnotationAppearBeforeIDShouldThrow()
        {
            string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#$ref\",\"@TestNamespace.unknown\":123,\"@odata.id\":\"http://host/Customers(1)\",\"@custom.annotation\":456}";
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer(payload);
            Action readResult = () => deserializer.ReadEntityReferenceLink();
            readResult.Throws<ODataException>(ODataErrorStrings.ODataJsonLightEntityReferenceLinkDeserializer_MissingEntityReferenceLinkProperty(ODataAnnotationNames.ODataId));
        }

        [Fact]
        public void ReadForEntityReferenceLinkWithDuplicateAnnotationNameShouldNotThrow()
        {
            string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#$ref\",\"@odata.id\":\"http://host/Customers(1)\",\"@TestNamespace.unknown\":123,\"@TestNamespace.unknown\":456}";
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer(payload);
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
            writeResult.Throws<ODataException>(ODataErrorStrings.JsonLightInstanceAnnotationWriter_DuplicateAnnotationNameInCollection("TestNamespace.unknown"));
            writeResult = () => WriteAndValidate(referencelink, expectedPayload, writingResponse: true);
            writeResult.Throws<ODataException>(ODataErrorStrings.JsonLightInstanceAnnotationWriter_DuplicateAnnotationNameInCollection("TestNamespace.unknown"));
        }

        [Fact]
        public void ShouldReadAndWriteForEntityReferenceLink()
        {
            string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#$ref\",\"@odata.id\":\"http://host/Customers(1)\",\"@TestNamespace.unknown\":123,\"@custom.annotation\":456}";
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer(payload);
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
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer(midplayoad);
            ODataEntityReferenceLink link = deserializer.ReadEntityReferenceLink();
            SameEntityReferenceLink(referencelink, link);
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
            var outputContext = CreateJsonLightOutputContext(stream, writingResponse, synchronous);
            outputContext.WriteEntityReferenceLink(referencelink);
            stream.Seek(0, SeekOrigin.Begin);
            return (new StreamReader(stream)).ReadToEnd();
        }

        private static void WriteAndValidate(ODataEntityReferenceLink referencelink, string expectedPayload, bool writingResponse = true, bool synchronous = true)
        {
            string payload = WriteToString(referencelink, writingResponse, synchronous);
            Assert.Equal(payload, expectedPayload);
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

        private ODataJsonLightEntityReferenceLinkDeserializer CreateJsonLightEntryAndFeedDeserializer(string payload, bool isIeee754Compatible = false)
        {
            var inputContext = this.CreateJsonLightInputContext(payload, isIeee754Compatible);

            return new ODataJsonLightEntityReferenceLinkDeserializer(inputContext);
        }

        private ODataJsonLightInputContext CreateJsonLightInputContext(string payload, bool isIeee754Compatible)
        {
            var mediaType = isIeee754Compatible
                ? new ODataMediaType("application", "json", new KeyValuePair<string, string>("IEEE754Compatible", "true"))
                : new ODataMediaType("application", "json", new KeyValuePair<string, string>("odata.streaming", "true"));

            var messageInfo = new ODataMessageInfo
            {
                IsResponse = true,
                MediaType = mediaType,
                IsAsync = false,
                Model = EdmModel,
            };

            return new ODataJsonLightInputContext(
                new StringReader(payload),
                messageInfo,
                MessageReaderSettingsReadAndValidateCustomInstanceAnnotations);
        }
    }
}

﻿//---------------------------------------------------------------------
// <copyright file="ODataEntityReferenceLinkTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.ObjectModel
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using FluentAssertions;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Evaluation;
    using Microsoft.Test.OData.TDD.Tests.Evaluation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.OData.Core.JsonLight;
    using System.IO;
    using System.Text;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.TDD.Tests.Common;
    using Microsoft.OData.Core.Json;
    using System.Diagnostics;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    [TestClass]
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

        [TestMethod]
        public void ShouldBeAbleToSetAndClearIdOnEntityReferenceLink()
        {
            ODataEntityReferenceLink referencelink = new ODataEntityReferenceLink();
            referencelink.Url.Should().BeNull();
            referencelink.Url = new Uri("http://my/Id");
            referencelink.Url.ToString().Should().Be("http://my/Id");
            referencelink.Url = null;
            referencelink.Url.Should().BeNull();
        }

        [TestMethod]
        public void InstanceAnnotationsPropertyShouldNotBeNullAtCreation()
        {
            ODataEntityReferenceLink referencelink = new ODataEntityReferenceLink();
            referencelink.InstanceAnnotations.Should().NotBeNull();
            referencelink.InstanceAnnotations.Count.Should().Be(0);
        }

        [TestMethod]
        public void InstanceAnnotationsPropertyShouldReturnAWritableCollectionAtCreation()
        {
            ODataEntityReferenceLink referencelink = new ODataEntityReferenceLink();
            referencelink.InstanceAnnotations.Should().NotBeNull();
            referencelink.InstanceAnnotations.Add(new ODataInstanceAnnotation("TestNamespace.name", new ODataPrimitiveValue("value")));
            referencelink.InstanceAnnotations.Count.Should().Be(1);
        }

        [TestMethod]
        public void SetNullValueToInstanceAnnotationsPropertyShouldThrow()
        {
            ODataEntityReferenceLink referencelink = new ODataEntityReferenceLink();
            Action test = () => referencelink.InstanceAnnotations = null;
            test.ShouldThrow<ArgumentException>().WithMessage("Value cannot be null.\r\nParameter name: value");
        }

        [TestMethod]
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

        [TestMethod]
        public void ShouldReadForEntityReferenceLink()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@odata.context\":\"http://odata.org/test/$metadata#$ref\",\"@odata.id\":\"http://host/Customers(1)\",\"@TestNamespace.unknown\":123,\"@custom.annotation\":456}");
            ODataEntityReferenceLink link = deserializer.ReadEntityReferenceLink();
            link.Url.ToString().Should().Be("http://host/Customers(1)");
            Assert.AreEqual(2, link.InstanceAnnotations.Count);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(123), link.InstanceAnnotations.Single(ia => ia.Name == "TestNamespace.unknown").Value);
            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(456), link.InstanceAnnotations.Single(ia => ia.Name == "custom.annotation").Value);
        }

        [TestMethod]
        public void ReadForEntityReferenceLinkRequestIDAppearBeforeContextShouldThrow()
        {
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@odata.id\":\"http://host/Customers(1)\",\"@odata.context\":\"http://odata.org/test/$metadata#$ref\",\"@TestNamespace.unknown\":123,\"@custom.annotation\":456}");
            Action readResult = () => deserializer.ReadEntityReferenceLink();
            readResult.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ODataJsonLightDeserializer_ContextLinkNotFoundAsFirstProperty);
        }

        [TestMethod]
        public void ReadForEntityReferenceLinkRequestWithoutIDButAnnotationShouldThrow()
        {
            string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#$ref\",\"@odataa.unknown\":123}";
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer(payload);
            Action readResult = () => deserializer.ReadEntityReferenceLink();
            readResult.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ODataJsonLightEntityReferenceLinkDeserializer_MissingEntityReferenceLinkProperty(ODataAnnotationNames.ODataId));
        }

        [TestMethod]
        public void ReadForEntityReferenceLinkRequestOnlyContextShouldThrow()
        {
            string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#$ref\"}";
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer(payload);
            Action readResult = () => deserializer.ReadEntityReferenceLink();
            readResult.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ODataJsonLightEntityReferenceLinkDeserializer_MissingEntityReferenceLinkProperty(ODataAnnotationNames.ODataId));
        }

        [TestMethod]
        public void ReadForEntityReferenceLinkRequestWithEmptyPayloadShouldThrow()
        {
            string payload = "{}";
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer(payload);
            Action readResult = () => deserializer.ReadEntityReferenceLink();
            readResult.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ODataJsonLightDeserializer_ContextLinkNotFoundAsFirstProperty);
        }

        [TestMethod]
        public void ReadForEntityReferenceLinkRequestAnnotationAppearBeforeIDShouldThrow()
        {
            string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#$ref\",\"@TestNamespace.unknown\":123,\"@odata.id\":\"http://host/Customers(1)\",\"@custom.annotation\":456}";
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer(payload);
            Action readResult = () => deserializer.ReadEntityReferenceLink();
            readResult.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ODataJsonLightEntityReferenceLinkDeserializer_MissingEntityReferenceLinkProperty(ODataAnnotationNames.ODataId));
        }

        [TestMethod]
        public void ShouldReadAndWriteForEntityReferenceLink()
        {
            string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#$ref\",\"@odata.id\":\"http://host/Customers(1)\",\"@TestNamespace.unknown\":123,\"@custom.annotation\":456}";
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer(payload);
            ODataEntityReferenceLink link = deserializer.ReadEntityReferenceLink();
            WriteAndValidate(link, payload, writingResponse: false);
            WriteAndValidate(link, payload, writingResponse: true);
        }

        [TestMethod]
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
            link1.Should().NotBeNull();
            link2.Should().NotBeNull();
            link1.Url.ToString().Should().Be(link2.Url.ToString());
            link1.InstanceAnnotations.Count.Should().Equals(link2.InstanceAnnotations.Count);
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
            payload.Should().Be(expectedPayload);
        }

        private static ODataJsonLightOutputContext CreateJsonLightOutputContext(MemoryStream stream, bool writingResponse = true, bool synchronous = true)
        {
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings { Version = ODataVersion.V4 };
            settings.SetServiceDocumentUri(new Uri("http://odata.org/test"));
            settings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");
            return new ODataJsonLightOutputContext(
                ODataFormat.Json,
                new NonDisposingStream(stream),
                new ODataMediaType("application", "json"),
                Encoding.UTF8,
                settings,
                writingResponse,
                synchronous,
                EdmCoreModel.Instance,
                /*urlResolver*/ null);
        }

        private ODataJsonLightEntityReferenceLinkDeserializer CreateJsonLightEntryAndFeedDeserializer(string payload, bool isIeee754Compatible = false)
        {
            var inputContext = this.CreateJsonLightInputContext(payload, isIeee754Compatible);

            return new ODataJsonLightEntityReferenceLinkDeserializer(inputContext);
        }

        private ODataJsonLightInputContext CreateJsonLightInputContext(string payload, bool isIeee754Compatible)
        {
            ODataMediaType mediaType = isIeee754Compatible
                ? new ODataMediaType("application", "json", new KeyValuePair<string, string>("IEEE754Compatible", "true"))
                : new ODataMediaType("application", "json", new KeyValuePair<string, string>("odata.streaming", "true"));
            return new ODataJsonLightInputContext(
                ODataFormat.Json,
                new MemoryStream(Encoding.UTF8.GetBytes(payload)),
                mediaType,
                Encoding.UTF8,
                MessageReaderSettingsReadAndValidateCustomInstanceAnnotations,
                /*readingResponse*/ true,
                /*synchronous*/ true,
                EdmModel,
                /*urlResolver*/ null);
        }
    }
}

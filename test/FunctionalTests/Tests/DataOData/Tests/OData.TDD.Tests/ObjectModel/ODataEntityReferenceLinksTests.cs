﻿//---------------------------------------------------------------------
// <copyright file="ODataEntityReferenceLinksTests.cs" company="Microsoft">
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

    [TestClass]
    public class OdataEntityReferenceLinksTests
    {
        private readonly static IEdmModel EdmModel;
        private readonly static ODataMessageReaderSettings MessageReaderSettingsReadAndValidateCustomInstanceAnnotations;
        private static readonly EdmEntityType EntityType;
        private static readonly EdmEntitySet EntitySet;

        static OdataEntityReferenceLinksTests()
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
        public void TheNewEntityReferenceLinksShouldNotBeNull()
        {
            ODataEntityReferenceLinks referencelinks = new ODataEntityReferenceLinks();
            referencelinks.Should().NotBeNull();
            referencelinks.Links.Should().BeNull();
        }

        [TestMethod]
        public void ShouldBeAbleToSetLinksReferenceLinks()
        {
            ODataEntityReferenceLink link = new ODataEntityReferenceLink
            {
                Url = new Uri("http://host/Customers(1)")
            };
            ODataEntityReferenceLinks referencelinks = new ODataEntityReferenceLinks()
            {
                Links = new[] { link }
            };
            referencelinks.Should().NotBeNull();
            referencelinks.Links.Should().NotBeNull();
            referencelinks.Links.Count().Should().Be(1);
        }

        [TestMethod]
        public void ShouldWriteForEntityReferenceLinksRequestWithSingleLink()
        {
            ODataEntityReferenceLink link = new ODataEntityReferenceLink
            {
                Url = new Uri("http://host/Customers(1)")
            };
            link.InstanceAnnotations.Add(new ODataInstanceAnnotation("Is.New", new ODataPrimitiveValue(true)));
            ODataEntityReferenceLinks referencelinks = new ODataEntityReferenceLinks()
            {
                Links = new[] { link }
            };
            string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection($ref)\",\"value\":[{\"@odata.id\":\"http://host/Customers(1)\",\"@Is.New\":true}]}";
            WriteAndValidate(referencelinks, expectedPayload, writingResponse: false);
            WriteAndValidate(referencelinks, expectedPayload, writingResponse: true);
        }

        [TestMethod]
        public void ShouldWriteForEntityReferenceLinksRequestWithMultpleLinks()
        {
            ODataEntityReferenceLink link1 = new ODataEntityReferenceLink
            {
                Url = new Uri("http://host/Customers(1)")
            };
            link1.InstanceAnnotations.Add(new ODataInstanceAnnotation("Is.New", new ODataPrimitiveValue(true)));
            ODataEntityReferenceLink link2 = new ODataEntityReferenceLink
            {
                Url = new Uri("http://host/Customers(2)")
            };
            link2.InstanceAnnotations.Add(new ODataInstanceAnnotation("TestNamespace.unknown", new ODataPrimitiveValue(123)));
            link2.InstanceAnnotations.Add(new ODataInstanceAnnotation("custom.annotation", new ODataPrimitiveValue(456)));

            ODataEntityReferenceLinks referencelinks = new ODataEntityReferenceLinks()
            {
                Links = new[] { link1, link2 }
            };
            string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection($ref)\",\"value\":[{\"@odata.id\":\"http://host/Customers(1)\",\"@Is.New\":true},{\"@odata.id\":\"http://host/Customers(2)\",\"@TestNamespace.unknown\":123,\"@custom.annotation\":456}]}";
            WriteAndValidate(referencelinks, expectedPayload, writingResponse: false);
            WriteAndValidate(referencelinks, expectedPayload, writingResponse: true);
        }

        [TestMethod]
        public void ShouldReadForEntityReferenceLinkRequesttWithSingleLink()
        {
            ODataEntityReferenceLink link = new ODataEntityReferenceLink
            {
                Url = new Uri("http://host/Customers(1)")
            };
            link.InstanceAnnotations.Add(new ODataInstanceAnnotation("Is.New", new ODataPrimitiveValue(true)));
            ODataEntityReferenceLinks referencelinks = new ODataEntityReferenceLinks()
            {
                Links = new[] { link }
            };
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@odata.context\":\"http://odata.org/test/$metadata#Collection($ref)\",\"value\":[{\"@odata.id\":\"http://host/Customers(1)\",\"@Is.New\":true}]}");
            ODataEntityReferenceLinks links = deserializer.ReadEntityReferenceLinks();
            SameEntityReferenceLinks(referencelinks, links);
        }

        [TestMethod]
        public void ShouldReadForEntityReferenceLinkRequestWithMultpleLinks()
        {
            ODataEntityReferenceLink link1 = new ODataEntityReferenceLink
            {
                Url = new Uri("http://host/Customers(1)")
            };
            link1.InstanceAnnotations.Add(new ODataInstanceAnnotation("Is.New", new ODataPrimitiveValue(true)));
            ODataEntityReferenceLink link2 = new ODataEntityReferenceLink
            {
                Url = new Uri("http://host/Customers(2)")
            };
            link2.InstanceAnnotations.Add(new ODataInstanceAnnotation("TestNamespace.unknown", new ODataPrimitiveValue(123)));
            link2.InstanceAnnotations.Add(new ODataInstanceAnnotation("custom.annotation", new ODataPrimitiveValue(456)));

            ODataEntityReferenceLinks referencelinks = new ODataEntityReferenceLinks()
            {
                Links = new[] { link1, link2 }
            };
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer("{\"@odata.context\":\"http://odata.org/test/$metadata#Collection($ref)\",\"value\":[{\"@odata.id\":\"http://host/Customers(1)\",\"@Is.New\":true},{\"@odata.id\":\"http://host/Customers(2)\",\"@TestNamespace.unknown\":123,\"@custom.annotation\":456}]}");
            ODataEntityReferenceLinks links = deserializer.ReadEntityReferenceLinks();
            SameEntityReferenceLinks(referencelinks, links);
        }

        [TestMethod]
        public void ShouldReadAndWriteForEntityReferenceLinksRequest()
        {
            string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection($ref)\",\"value\":[{\"@odata.id\":\"http://host/Customers(1)\",\"@Is.New\":true}]}";
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer(payload);
            ODataEntityReferenceLinks links = deserializer.ReadEntityReferenceLinks();
            WriteAndValidate(links, payload, writingResponse: false);
            WriteAndValidate(links, payload, writingResponse: true);
        }

        [TestMethod]
        public void ShouldWriteAndReadForEntityReferenceLinkRequest()
        {
            ODataEntityReferenceLink link1 = new ODataEntityReferenceLink
            {
                Url = new Uri("http://host/Customers(1)")
            };
            link1.InstanceAnnotations.Add(new ODataInstanceAnnotation("Is.New", new ODataPrimitiveValue(true)));
            ODataEntityReferenceLink link2 = new ODataEntityReferenceLink
            {
                Url = new Uri("http://host/Customers(2)")
            };
            link2.InstanceAnnotations.Add(new ODataInstanceAnnotation("TestNamespace.unknown", new ODataPrimitiveValue(123)));
            link2.InstanceAnnotations.Add(new ODataInstanceAnnotation("custom.annotation", new ODataPrimitiveValue(456)));

            ODataEntityReferenceLinks referencelinks = new ODataEntityReferenceLinks()
            {
                Links = new[] { link1, link2 }
            };
            string midplayoad = WriteToString(referencelinks, writingResponse: false);
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer(midplayoad);
            ODataEntityReferenceLinks links = deserializer.ReadEntityReferenceLinks();
            SameEntityReferenceLinks(referencelinks, links);
        }

        [TestMethod]
        public void WriteTopLevelEntityReferenceLinks()
        {
            ODataEntityReferenceLink link1 = new ODataEntityReferenceLink
            {
                Url = new Uri("http://host/Customers(1)")
            };
            link1.InstanceAnnotations.Add(new ODataInstanceAnnotation("Is.New", new ODataPrimitiveValue(true)));
            ODataEntityReferenceLink link2 = new ODataEntityReferenceLink
            {
                Url = new Uri("http://host/Customers(2)")
            };
            link2.InstanceAnnotations.Add(new ODataInstanceAnnotation("TestNamespace.unknown", new ODataPrimitiveValue(123)));
            link2.InstanceAnnotations.Add(new ODataInstanceAnnotation("custom.annotation", new ODataPrimitiveValue(456)));

            ODataEntityReferenceLinks referencelinks = new ODataEntityReferenceLinks()
            {
                Links = new[] { link1, link2 }
            };

            var writerSettings = new ODataMessageWriterSettings { DisableMessageStreamDisposal = true };
            writerSettings.SetContentType(ODataFormat.Json);
            writerSettings.SetServiceDocumentUri(new Uri("http://odata.org/test"));
            MemoryStream stream = new MemoryStream();
            IODataResponseMessage requestMessageToWrite = new InMemoryMessage { StatusCode = 200, Stream = stream };
            requestMessageToWrite.PreferenceAppliedHeader().AnnotationFilter = "*";

            using (var messageWriter = new ODataMessageWriter(requestMessageToWrite, writerSettings, EdmModel))
            {
                messageWriter.WriteEntityReferenceLinks(referencelinks);
            }
            stream.Position = 0;
            string payload = (new StreamReader(stream)).ReadToEnd();

            string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection($ref)\",\"value\":[{\"@odata.id\":\"http://host/Customers(1)\",\"@Is.New\":true},{\"@odata.id\":\"http://host/Customers(2)\",\"@TestNamespace.unknown\":123,\"@custom.annotation\":456}]}";

            Assert.AreEqual(expectedPayload, payload);
        }

        private static void SameEntityReferenceLinks(ODataEntityReferenceLinks links1, ODataEntityReferenceLinks links2)
        {
            links1.Should().NotBeNull();
            links2.Should().NotBeNull();
            links1.Links.Should().NotBeNull();
            links2.Links.Should().NotBeNull();
            links1.Links.Count().Should().Equals(links2.Links.Count());
            for (var i = 0; i < links1.Links.Count(); ++i)
            {
                SameEntityReferenceLink(links1.Links.ElementAt(i), links2.Links.ElementAt(i));
            }
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

        private static string WriteToString(ODataEntityReferenceLinks referencelinks, bool writingResponse = true, bool synchronous = true)
        {
            MemoryStream stream = new MemoryStream();
            var outputContext = CreateJsonLightOutputContext(stream, writingResponse, synchronous);
            outputContext.WriteEntityReferenceLinks(referencelinks);
            stream.Seek(0, SeekOrigin.Begin);
            return (new StreamReader(stream)).ReadToEnd();
        }

        private static void WriteAndValidate(ODataEntityReferenceLinks referencelinks, string expectedPayload, bool writingResponse = true, bool synchronous = true)
        {
            string payload = WriteToString(referencelinks, writingResponse, synchronous);
            Console.WriteLine(payload);
            Console.WriteLine(expectedPayload);
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

        private ODataJsonLightEntityReferenceLinkDeserializer CreateJsonLightEntryAndFeedDeserializer(string payload, bool shouldReadAndValidateCustomInstanceAnnotations = true, bool isIeee754Compatible = false)
        {
            var inputContext = this.CreateJsonLightInputContext(payload, shouldReadAndValidateCustomInstanceAnnotations, isIeee754Compatible);

            return new ODataJsonLightEntityReferenceLinkDeserializer(inputContext);
        }

        private ODataJsonLightInputContext CreateJsonLightInputContext(string payload, bool shouldReadAndValidateCustomInstanceAnnotations, bool isIeee754Compatible)
        {
            ODataMediaType mediaType = isIeee754Compatible
                ? new ODataMediaType("application", "json", new KeyValuePair<string, string>("IEEE754Compatible", "true"))
                : new ODataMediaType("application", "json");
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

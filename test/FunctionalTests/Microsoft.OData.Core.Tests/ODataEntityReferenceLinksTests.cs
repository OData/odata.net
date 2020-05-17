//---------------------------------------------------------------------
// <copyright file="ODataEntityReferenceLinksTests.cs" company="Microsoft">
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
    public class ODataEntityReferenceLinksTests
    {
        private readonly static IEdmModel EdmModel;
        private readonly static ODataMessageReaderSettings MessageReaderSettingsReadAndValidateCustomInstanceAnnotations;
        private static readonly EdmEntityType EntityType;
        private static readonly EdmEntitySet EntitySet;

        static ODataEntityReferenceLinksTests()
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
        public void TheNewEntityReferenceLinksShouldNotBeNull()
        {
            ODataEntityReferenceLinks referencelinks = new ODataEntityReferenceLinks();
            Assert.NotNull(referencelinks);
            Assert.Null(referencelinks.Links);
        }

        [Fact]
        public void InstanceAnnotationsPropertyShouldNotBeNullAtCreation()
        {
            ODataEntityReferenceLinks referencelinks = new ODataEntityReferenceLinks();
            Assert.NotNull(referencelinks.InstanceAnnotations);
            Assert.Empty(referencelinks.InstanceAnnotations);
        }

        [Fact]
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

            Assert.NotNull(referencelinks);
            Assert.NotNull(referencelinks.Links);
            Assert.Single(referencelinks.Links);
        }

        [Fact]
        public void ShouldWriteForEntityReferenceLinksWithSingleLink()
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

        [Fact]
        public void ShouldWriteForEntityReferenceLinksWithMultpleLinks()
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

        [Fact]
        public void ShouldWriteForEntityReferenceLinksWithReferenceLinksAnnotaions()
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

            referencelinks.InstanceAnnotations.Add(new ODataInstanceAnnotation("TestNamespace.name", new ODataPrimitiveValue(321)));
            referencelinks.InstanceAnnotations.Add(new ODataInstanceAnnotation("custom.name", new ODataPrimitiveValue(654)));
            string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection($ref)\",\"@TestNamespace.name\":321,\"@custom.name\":654,\"value\":[{\"@odata.id\":\"http://host/Customers(1)\",\"@Is.New\":true},{\"@odata.id\":\"http://host/Customers(2)\",\"@TestNamespace.unknown\":123,\"@custom.annotation\":456}]}";
            WriteAndValidate(referencelinks, expectedPayload, writingResponse: false);
            WriteAndValidate(referencelinks, expectedPayload, writingResponse: true);
        }

        [Fact]
        public void ShouldReadForEntityReferenceLinksWithSingleLink()
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

        [Fact]
        public void ShouldReadForEntityReferenceLinksWithMultpleLinks()
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

        [Fact]
        public void ShouldReadForEntityReferenceLinksWithReferenceLinksAnnotations()
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

            referencelinks.InstanceAnnotations.Add(new ODataInstanceAnnotation("TestNamespace.name", new ODataPrimitiveValue(321)));
            referencelinks.InstanceAnnotations.Add(new ODataInstanceAnnotation("custom.name", new ODataPrimitiveValue(654)));
            string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection($ref)\",\"@TestNamespace.name\":321,\"value\":[{\"@odata.id\":\"http://host/Customers(1)\",\"@Is.New\":true},{\"@odata.id\":\"http://host/Customers(2)\",\"@TestNamespace.unknown\":123,\"@custom.annotation\":456}],\"@custom.name\":654}";

            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer(payload);
            ODataEntityReferenceLinks links = deserializer.ReadEntityReferenceLinks();
            SameEntityReferenceLinks(referencelinks, links);
        }

        [Fact]
        public void ReadForEntityReferenceLinksWithDuplicateAnnotationNameShouldNotThrow()
        {
            string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection($ref)\",\"@TestNamespace.name\":321,\"@TestNamespace.name\":654,\"value\":[{\"@odata.id\":\"http://host/Customers(1)\",\"@Is.New\":true},{\"@odata.id\":\"http://host/Customers(2)\",\"@TestNamespace.unknown\":123,\"@custom.annotation\":456}]}";
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer(payload);
            Action readResult = () => deserializer.ReadEntityReferenceLinks();
            readResult.DoesNotThrow();
        }

        [Fact]
        public void WriteForEntityReferenceLinksWithDuplicateAnnotationNameShouldThrow()
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

            referencelinks.InstanceAnnotations.Add(new ODataInstanceAnnotation("TestNamespace.name", new ODataPrimitiveValue(321)));
            referencelinks.InstanceAnnotations.Add(new ODataInstanceAnnotation("TestNamespace.name", new ODataPrimitiveValue(654)));
            string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection($ref)\",\"@TestNamespace.name\":321,\"@TestNamespace.name\":654,\"value\":[{\"@odata.id\":\"http://host/Customers(1)\",\"@Is.New\":true},{\"@odata.id\":\"http://host/Customers(2)\",\"@TestNamespace.unknown\":123,\"@custom.annotation\":456}]}";
            Action writeResult = () => WriteAndValidate(referencelinks, expectedPayload, writingResponse: false);
            writeResult.Throws<ODataException>(ODataErrorStrings.JsonLightInstanceAnnotationWriter_DuplicateAnnotationNameInCollection("TestNamespace.name"));
            writeResult = () => WriteAndValidate(referencelinks, expectedPayload, writingResponse: true);
            writeResult.Throws<ODataException>(ODataErrorStrings.JsonLightInstanceAnnotationWriter_DuplicateAnnotationNameInCollection("TestNamespace.name"));
        }

        [Fact]
        public void ShouldReadAndWriteForEntityReferenceLinks()
        {
            string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection($ref)\",\"value\":[{\"@odata.id\":\"http://host/Customers(1)\",\"@Is.New\":true}]}";
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer(payload);
            ODataEntityReferenceLinks links = deserializer.ReadEntityReferenceLinks();
            WriteAndValidate(links, payload, writingResponse: false);
            WriteAndValidate(links, payload, writingResponse: true);
        }

        [Fact]
        public void ShouldWriteAndReadForEntityReferenceLinks()
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

        [Fact]
        public void ShouldReadAndWriteForEntityReferenceLinksWithReferenceLinksAnnotation()
        {
            string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection($ref)\",\"@TestNamespace.name\":321,\"@custom.name\":654,\"value\":[{\"@odata.id\":\"http://host/Customers(1)\",\"@Is.New\":true},{\"@odata.id\":\"http://host/Customers(2)\",\"@TestNamespace.unknown\":123,\"@custom.annotation\":456}]}";
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer(payload);
            ODataEntityReferenceLinks links = deserializer.ReadEntityReferenceLinks();
            WriteAndValidate(links, payload, writingResponse: false);
            WriteAndValidate(links, payload, writingResponse: true);
        }

        [Fact]
        public void ShouldReadEntityReferenceCountAnnotationValue()
        {
            string payload = @"{
                ""@odata.count"":2,
                ""@odata.context"":""http://odata.org/test/$metadata#Collection($ref)"",
                ""@TestNamespace.name"":321,
                ""@custom.name"":654,
                ""value"":[
                    {""@odata.id"":""http://host/Customers(1)"",""@Is.New"":true},
                    {""@odata.id"":""http://host/Customers(2)"",""@TestNamespace.unknown"":123,""@custom.annotation"":456}
                ]
            }";

            ODataJsonLightEntityReferenceLinkDeserializer deserializer =
                this.CreateJsonLightEntryAndFeedDeserializer(payload);

            ODataEntityReferenceLinks links = deserializer.ReadEntityReferenceLinks();
            Assert.Equal(2, links.Count);
        }

        [Fact]
        public void ShouldReadEntityReferenceNextLinkAnnotationValue()
        {
            string payload = @"{
                ""@odata.context"":""http://odata.org/test/$metadata#Collection($ref)"",
                ""@odata.nextLink"":""http://odata.org/nextpage"",
                ""@TestNamespace.name"":321,
                ""@custom.name"":654,
                ""value"":[]
            }";

            ODataJsonLightEntityReferenceLinkDeserializer deserializer =
                this.CreateJsonLightEntryAndFeedDeserializer(payload);

            ODataEntityReferenceLinks links = deserializer.ReadEntityReferenceLinks();
            Assert.Equal(new Uri("http://odata.org/nextpage"), links.NextPageLink);
        }

        [Fact]
        public void ShouldWriteAndReadForEntityReferenceLinksWithReferenceLinksAnnotation()
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

            referencelinks.InstanceAnnotations.Add(new ODataInstanceAnnotation("TestNamespace.name", new ODataPrimitiveValue(321)));
            referencelinks.InstanceAnnotations.Add(new ODataInstanceAnnotation("custom.name", new ODataPrimitiveValue(654)));

            string midplayoad = WriteToString(referencelinks, writingResponse: false);
            var deserializer = this.CreateJsonLightEntryAndFeedDeserializer(midplayoad);
            ODataEntityReferenceLinks links = deserializer.ReadEntityReferenceLinks();
            SameEntityReferenceLinks(referencelinks, links);
        }
        [Fact]
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

            var writerSettings = new ODataMessageWriterSettings { EnableMessageStreamDisposal = false };
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

            Assert.Equal(expectedPayload, payload);
        }

        private static void SameInstanceAnnotations(ICollection<ODataInstanceAnnotation> InstanceAnnotations1, ICollection<ODataInstanceAnnotation> InstanceAnnotations2)
        {
            Assert.Equal(InstanceAnnotations1.Count, InstanceAnnotations2.Count);
            foreach (ODataInstanceAnnotation instanceannotation in InstanceAnnotations1)
            {
                ODataInstanceAnnotation annotation = InstanceAnnotations2.SingleOrDefault(ia => ia.Name == instanceannotation.Name.ToString());
                Assert.NotNull(annotation);
                TestUtils.AssertODataValueAreEqual(instanceannotation.Value, annotation.Value);
            }
        }

        private static void SameEntityReferenceLinks(ODataEntityReferenceLinks links1, ODataEntityReferenceLinks links2)
        {
            Assert.NotNull(links1);
            Assert.NotNull(links2);
            Assert.NotNull(links1.Links);
            Assert.NotNull(links2.Links);
            Assert.Equal(links1.Links.Count(), links2.Links.Count());
            for (var i = 0; i < links1.Links.Count(); ++i)
            {
                SameEntityReferenceLink(links1.Links.ElementAt(i), links2.Links.ElementAt(i));
            }
            SameInstanceAnnotations(links1.InstanceAnnotations, links2.InstanceAnnotations);
        }

        private static void SameEntityReferenceLink(ODataEntityReferenceLink link1, ODataEntityReferenceLink link2)
        {
            Assert.NotNull(link1);
            Assert.NotNull(link2);
            Assert.Equal(link1.Url.ToString(), link2.Url.ToString());
            SameInstanceAnnotations(link1.InstanceAnnotations, link2.InstanceAnnotations);
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
            Assert.Equal(expectedPayload, payload);
        }

        private static ODataJsonLightOutputContext CreateJsonLightOutputContext(MemoryStream stream, bool writingResponse = true, bool synchronous = true)
        {
            var settings = new ODataMessageWriterSettings { Version = ODataVersion.V4 };
            settings.SetServiceDocumentUri(new Uri("http://odata.org/test"));
            settings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");

            var messageInfo = new ODataMessageInfo
            {
                MessageStream = new NonDisposingStream(stream),
                MediaType = new ODataMediaType("application", "json"),
                Encoding = Encoding.UTF8,
                IsResponse = writingResponse,
                IsAsync = !synchronous,
                Model = EdmCoreModel.Instance
            };

            return new ODataJsonLightOutputContext(messageInfo, settings);
        }

        private ODataJsonLightEntityReferenceLinkDeserializer CreateJsonLightEntryAndFeedDeserializer(string payload, bool shouldReadAndValidateCustomInstanceAnnotations = true, bool isIeee754Compatible = false)
        {
            var inputContext = this.CreateJsonLightInputContext(payload, shouldReadAndValidateCustomInstanceAnnotations, isIeee754Compatible);

            return new ODataJsonLightEntityReferenceLinkDeserializer(inputContext);
        }

        private ODataJsonLightInputContext CreateJsonLightInputContext(string payload, bool shouldReadAndValidateCustomInstanceAnnotations, bool isIeee754Compatible)
        {
            var mediaType = isIeee754Compatible
                ? new ODataMediaType("application", "json", new KeyValuePair<string, string>("IEEE754Compatible", "true"))
                : new ODataMediaType("application", "json");

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

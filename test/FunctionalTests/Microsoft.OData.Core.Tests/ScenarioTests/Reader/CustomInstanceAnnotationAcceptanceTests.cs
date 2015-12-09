//---------------------------------------------------------------------
// <copyright file="CustomInstanceAnnotationAcceptanceTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.ScenarioTests.Reader
{
    public class CustomInstanceAnnotationAcceptanceTests
    {
        private static readonly ODataPrimitiveValue PrimitiveValue1 = new ODataPrimitiveValue(123);
        private static readonly ODataPrimitiveValue PrimitiveValue2 = new ODataPrimitiveValue(Guid.Empty);
        private static readonly ODataComplexValue ComplexValue1 = new ODataComplexValue { Properties = new[] { new ODataProperty { Name = "StringProperty", Value = "StringValue1" } }, TypeName = "TestNamespace.TestComplexType" };
        private static readonly ODataComplexValue ComplexValue2 = new ODataComplexValue { Properties = new[] { new ODataProperty { Name = "StringProperty", Value = "StringValue2" } }, TypeName = "TestNamespace.TestComplexType" };
        private static readonly ODataCollectionValue PrimitiveCollectionValue = new ODataCollectionValue { Items = new[] { "StringValue1", "StringValue2" }, TypeName = "Collection(String)" };
        private static readonly ODataCollectionValue ComplexCollectionValue = new ODataCollectionValue { Items = new[] { ComplexValue1, ComplexValue2 }, TypeName = "Collection(TestNamespace.TestComplexType)" };

        private static readonly EdmModel Model;
        private static readonly EdmEntitySet EntitySet;
        private static readonly EdmEntityType EntityType;
        private static readonly EdmComplexType ComplexType;

        static CustomInstanceAnnotationAcceptanceTests()
        {
            Model = new EdmModel();
            EntityType = new EdmEntityType("TestNamespace", "TestEntityType");
            Model.AddElement(EntityType);

            var keyProperty = new EdmStructuralProperty(EntityType, "ID", EdmCoreModel.Instance.GetInt32(false));
            EntityType.AddKeys(new IEdmStructuralProperty[] { keyProperty });
            EntityType.AddProperty(keyProperty);
            var resourceNavigationProperty = EntityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "ResourceNavigationProperty", Target = EntityType, TargetMultiplicity = EdmMultiplicity.ZeroOrOne });
            var resourceSetNavigationProperty = EntityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "ResourceSetNavigationProperty", Target = EntityType, TargetMultiplicity = EdmMultiplicity.Many });

            var defaultContainer = new EdmEntityContainer("TestNamespace", "DefaultContainer");
            Model.AddElement(defaultContainer);
            EntitySet = new EdmEntitySet(defaultContainer, "TestEntitySet", EntityType);
            EntitySet.AddNavigationTarget(resourceNavigationProperty, EntitySet);
            EntitySet.AddNavigationTarget(resourceSetNavigationProperty, EntitySet);
            defaultContainer.AddElement(EntitySet);

            ComplexType = new EdmComplexType("TestNamespace", "TestComplexType");
            ComplexType.AddProperty(new EdmStructuralProperty(ComplexType, "StringProperty", EdmCoreModel.Instance.GetString(false)));
            Model.AddElement(ComplexType);
        }

        #region Feed and entry with custom instance annotations.

        const string JsonLightFeedAndEntryPayloadWithCustomInstanceAnnotations =
        "{" +
            "\"@odata.context\":\"http://www.example.com/service.svc/$metadata#TestEntitySet\"," +
            "\"@Custom.FeedStartAnnotation\":1," +
            "\"value\":[" +
                "{" +
                    "\"@Custom.EntryStartAnnotation\":2," +
                    "\"ID\":1," +
                    "\"ResourceNavigationProperty@odata.navigationLink\":\"http://example.com/single\"," +
                    "\"ResourceNavigationProperty\":{" +
                        "\"@Custom.EntryStartAnnotation\":3," +
                        "\"ID\":2," +
                        "\"ResourceNavigationProperty@odata.navigationLink\":\"http://example.com/single\"," +
                        "\"@Custom.EntryMiddleAnnotation\":3," +
                        "\"ResourceSetNavigationProperty@odata.navigationLink\":\"http://example.com/multiple\"," +
                        "\"@Custom.EntryEndAnnotation\":3" +
                    "}," +
                    "\"@Custom.EntryMiddleAnnotation\":2," +
                    "\"ResourceSetNavigationProperty@odata.navigationLink\":\"http://example.com/multiple\"," +
                    "\"ResourceSetNavigationProperty\":[" +
                        "{" +
                            "\"@Custom.EntryStartAnnotation\":4," +
                            "\"ID\":3," +
                            "\"ResourceNavigationProperty@odata.navigationLink\":\"http://example.com/single\"," +
                            "\"@Custom.EntryMiddleAnnotation\":4," +
                            "\"ResourceSetNavigationProperty@odata.navigationLink\":\"http://example.com/multiple\"," +
                            "\"@Custom.EntryEndAnnotation\":4" +
                        "}" +
                    "]," +
                    "\"@Custom.EntryEndAnnotation\":2" +
                "}" +
            "]," +
            "\"@Custom.FeedEndAnnotation\":1" +
        "}";

        const string AtomFeedAndEntryPayloadWithCustomInstanceAnnotations =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<feed xml:base=""http://localhost:34402/WcfDataService1.svc/"" xmlns=""http://www.w3.org/2005/Atom"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
  <m:annotation term=""Custom.FeedStartAnnotation"" int=""1"" />
  <id>http://localhost:34402/WcfDataService1.svc/Folders</id>
  <title type=""text"">Folders</title>
  <updated>2013-03-12T00:24:18Z</updated>
  <link rel=""self"" title=""Folders"" href=""Folders"" />
  <entry>
    <m:annotation term=""Custom.EntryStartAnnotation"" int=""2"" />
    <id>http://localhost:34402/WcfDataService1.svc/Folders(1)</id>
    <category term=""#TestNamespace.TestEntityType"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" />
    <link rel=""edit"" title=""TestEntityType"" href=""Folders(1)"" />
    <link rel=""http://docs.oasis-open.org/odata/ns/related/ResourceNavigationProperty"" type=""application/atom+xml;type=entry"" title=""ResourceNavigationProperty"" href=""Folders(1)/ResourceNavigationProperty"">
      <m:inline>
        <entry>
          <m:annotation term=""Custom.EntryStartAnnotation"" int=""3"" />
          <id>http://localhost:34402/WcfDataService1.svc/Folders(2)</id>
          <category term=""#TestNamespace.TestEntityType"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" />
          <link rel=""edit"" title=""TestEntityType"" href=""Folders(2)"" />
          <link rel=""http://docs.oasis-open.org/odata/ns/related/ResourceNavigationProperty"" type=""application/atom+xml;type=entry"" title=""ResourceNavigationProperty"" href=""Folders(2)/ResourceNavigationProperty"" />
          <m:annotation term=""Custom.EntryMiddleAnnotation"" int=""3"" />
          <link rel=""http://docs.oasis-open.org/odata/ns/related/ResourceSetNavigationProperty"" type=""application/atom+xml;type=feed"" title=""ResourceSetNavigationProperty"" href=""Folders(2)/ResourceSetNavigationProperty"" />
          <title />
          <updated>2013-03-12T00:24:18Z</updated>
          <author>
            <name />
          </author>
          <content type=""application/xml"">
            <m:properties>
              <d:ID m:type=""Edm.Int32"">2</d:ID>
            </m:properties>
          </content>
          <m:annotation term=""Custom.EntryEndAnnotation"" int=""3"" />
        </entry>
      </m:inline>
    </link>
    <m:annotation term=""Custom.EntryMiddleAnnotation"" int=""2"" />
    <link rel=""http://docs.oasis-open.org/odata/ns/related/ResourceSetNavigationProperty"" type=""application/atom+xml;type=feed"" title=""ResourceSetNavigationProperty"" href=""Folders(1)/ResourceSetNavigationProperty"">
      <m:inline>
        <feed>
          <id>http://localhost:34402/WcfDataService1.svc/Folders(1)/ResourceSetNavigationProperty</id>
          <title type=""text"">ResourceSetNavigationProperty</title>
          <updated>2013-03-12T00:24:18Z</updated>
          <link rel=""self"" title=""ResourceSetNavigationProperty"" href=""Folders(1)/ResourceSetNavigationProperty"" />
          <entry>
            <m:annotation term=""Custom.EntryStartAnnotation"" int=""4"" />
            <id>http://localhost:34402/WcfDataService1.svc/Folders(3)</id>
            <category term=""#TestNamespace.TestEntityType"" scheme=""http://docs.oasis-open.org/odata/ns/scheme"" />
            <link rel=""edit"" title=""TestEntityType"" href=""Folders(3)"" />
            <link rel=""http://docs.oasis-open.org/odata/ns/related/ResourceNavigationProperty"" type=""application/atom+xml;type=entry"" title=""ResourceNavigationProperty"" href=""Folders(3)/ResourceNavigationProperty"" />
            <m:annotation term=""Custom.EntryMiddleAnnotation"" int=""4"" />
            <link rel=""http://docs.oasis-open.org/odata/ns/related/ResourceSetNavigationProperty"" type=""application/atom+xml;type=feed"" title=""ResourceSetNavigationProperty"" href=""Folders(3)/ResourceSetNavigationProperty"" />
            <title />
            <updated>2013-03-12T00:24:18Z</updated>
            <author>
              <name />
            </author>
            <content type=""application/xml"">
              <m:properties>
                <d:ID m:type=""Edm.Int32"">3</d:ID>
              </m:properties>
            </content>
            <m:annotation term=""Custom.EntryEndAnnotation"" int=""4"" />
          </entry>
        </feed>
      </m:inline>
    </link>
    <title />
    <updated>2013-03-12T00:24:18Z</updated>
    <author>
      <name />
    </author>
    <content type=""application/xml"">
      <m:properties>
        <d:ID m:type=""Edm.Int32"">1</d:ID>
      </m:properties>
    </content>
    <m:annotation term=""Custom.EntryEndAnnotation"" int=""2"" />
    <m:annotation term=""ShouldSkip.EntryEndAnnotation2"" m:type=""#TestNamespace.TestComplexType"">
      <d:StringProperty>Testing structured annotation</d:StringProperty>
    </m:annotation>
  </entry>
  <m:annotation term=""Custom.FeedEndAnnotation"" int=""1"" />
  <m:annotation term=""ShouldSkip.FeedEndAnnotation"" int=""0"" />
</feed>";

        [Fact]
        public void CustomInstanceAnnotationFromFeedAndEntryInJsonLightShouldBeSkippedByTheReaderByDefault()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(JsonLightFeedAndEntryPayloadWithCustomInstanceAnnotations));
            var readerSettings = new ODataMessageReaderSettings { DisableMessageStreamDisposal = false };
            IODataResponseMessage messageToRead = new InMemoryMessage { StatusCode = 200, Stream = stream };
            messageToRead.SetHeader("Content-Type", "application/json;odata.streaming=true");

            using (var messageReader = new ODataMessageReader(messageToRead, readerSettings, Model))
            {
                var odataReader = messageReader.CreateODataFeedReader(EntitySet, EntityType);
                while (odataReader.Read())
                {
                    switch (odataReader.State)
                    {
                        case ODataReaderState.FeedStart:
                        case ODataReaderState.FeedEnd:
                            odataReader.Item.As<ODataFeed>().InstanceAnnotations.Should().BeEmpty();
                            break;
                        case ODataReaderState.NavigationLinkStart:
                        case ODataReaderState.NavigationLinkEnd:
                            break;
                        case ODataReaderState.EntryStart:
                        case ODataReaderState.EntryEnd:
                            odataReader.Item.As<ODataEntry>().InstanceAnnotations.Should().BeEmpty();
                            break;
                    }
                }
            }
        }

        [Fact]
        public void ShouldBeAbleToReadCustomInstanceAnnotationFromFeedAndEntryInJsonLight()
        {
            ShouldBeAbleToReadCustomInstanceAnnotationFromFeedAndEntry(JsonLightFeedAndEntryPayloadWithCustomInstanceAnnotations, "application/json;odata.streaming=true");
        }

        [Fact]
        public void ShouldBeAbleToReadCustomInstanceAnnotationFromFeedAndEntryInAtom()
        {
            ShouldBeAbleToReadCustomInstanceAnnotationFromFeedAndEntry(AtomFeedAndEntryPayloadWithCustomInstanceAnnotations, "application/atom+xml;type=feed;charset=utf-8");
        }

        public void ShouldBeAbleToReadCustomInstanceAnnotationFromFeedAndEntry(string payload, string contentType)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
            var readerSettings = new ODataMessageReaderSettings { DisableMessageStreamDisposal = false, EnableAtom = true };

            IODataResponseMessage messageToRead = new InMemoryMessage { StatusCode = 200, Stream = stream };
            messageToRead.SetHeader("Content-Type", contentType);

            // Enable reading custom instance annotations.
            messageToRead.PreferenceAppliedHeader().AnnotationFilter = "Custom.*";

            Stack<ODataItem> odataItems = new Stack<ODataItem>(4);
            using (var messageReader = new ODataMessageReader(messageToRead, readerSettings, Model))
            {
                var odataReader = messageReader.CreateODataFeedReader(EntitySet, EntityType);
                ICollection<ODataInstanceAnnotation> instanceAnnotations = null;
                while (odataReader.Read())
                {
                    switch (odataReader.State)
                    {
                        case ODataReaderState.FeedStart:
                            odataItems.Push(odataReader.Item);
                            instanceAnnotations = odataItems.Peek().As<ODataFeed>().InstanceAnnotations;

                            // TODO: We only support instance annotation at the top level feed at the moment. Will remove the if statement when support on inline feed is added.
                            if (odataItems.Count == 1)
                            {
                                // Note that in streaming mode, the collection should be populated with instance annotations read so far before the beginning of the first entry.
                                // We are currently in non-streaming mode. The reader will buffer the payload and read ahead till the
                                // end of the feed to read all instance annotations.
                                instanceAnnotations.Should().HaveCount(1);
                                TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(odataItems.Count), instanceAnnotations.Single(ia => ia.Name == "Custom.FeedStartAnnotation").Value);
                            }
                            else
                            {
                                instanceAnnotations.Should().BeEmpty();
                            }

                            break;
                        case ODataReaderState.FeedEnd:
                            instanceAnnotations = odataItems.Peek().As<ODataFeed>().InstanceAnnotations;

                            // TODO: We only support instance annotation at the top level feed at the moment. Will remove the if statement when support on inline feed is added.
                            if (odataItems.Count == 1)
                            {
                                instanceAnnotations.Should().HaveCount(2);
                                TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(1), instanceAnnotations.Single(ia => ia.Name == "Custom.FeedStartAnnotation").Value);
                                TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(1), instanceAnnotations.Single(ia => ia.Name == "Custom.FeedEndAnnotation").Value);
                            }
                            else
                            {
                                instanceAnnotations.Should().BeEmpty();
                            }

                            odataItems.Pop();
                            break;
                        case ODataReaderState.NavigationLinkStart:
                            ODataNavigationLink navigationLink = (ODataNavigationLink)odataReader.Item;
                            if (navigationLink.Name == "ResourceSetNavigationProperty")
                            {
                                // The collection should be populated with instance annotations read so far before the "ResourceSetNavigationProperty".
                                instanceAnnotations = odataItems.Peek().As<ODataEntry>().InstanceAnnotations;
                                instanceAnnotations.Should().HaveCount(2);
                                TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(odataItems.Count), instanceAnnotations.Single(ia => ia.Name == "Custom.EntryStartAnnotation").Value);
                                TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(odataItems.Count), instanceAnnotations.Single(ia => ia.Name == "Custom.EntryMiddleAnnotation").Value);
                            }

                            break;
                        case ODataReaderState.NavigationLinkEnd:
                            break;
                        case ODataReaderState.EntryStart:
                            odataItems.Push(odataReader.Item);

                            // The collection should be populated with instance annotations read so far before the first navigation/association link or before the end of the entry.
                            instanceAnnotations = odataItems.Peek().As<ODataEntry>().InstanceAnnotations;
                            instanceAnnotations.Should().HaveCount(1);
                            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(odataItems.Count), instanceAnnotations.Single(ia => ia.Name == "Custom.EntryStartAnnotation").Value);
                            break;

                        case ODataReaderState.EntryEnd:
                            instanceAnnotations = odataItems.Peek().As<ODataEntry>().InstanceAnnotations;
                            instanceAnnotations.Should().HaveCount(3);
                            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(odataItems.Count), instanceAnnotations.Single(ia => ia.Name == "Custom.EntryStartAnnotation").Value);
                            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(odataItems.Count), instanceAnnotations.Single(ia => ia.Name == "Custom.EntryMiddleAnnotation").Value);
                            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(odataItems.Count), instanceAnnotations.Single(ia => ia.Name == "Custom.EntryEndAnnotation").Value);
                            odataItems.Pop();
                            break;
                    }
                }

                instanceAnnotations.Should().HaveCount(2);
                TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(1), instanceAnnotations.Single(ia => ia.Name == "Custom.FeedStartAnnotation").Value);
                TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(1), instanceAnnotations.Single(ia => ia.Name == "Custom.FeedEndAnnotation").Value);
            }
        }

        [Fact]
        public void ShouldBeAbleToWriteCustomInstanceAnnotationToFeedAndEntryInJsonLight()
        {
            const string FeedWithCustomInstanceAnnotationInJsonLight =
            "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#TestEntitySet\"," +
                "\"@Custom.Int32Annotation\":123," +
                "\"Custom.GuidAnnotation@odata.type\":\"#Guid\"," +
                "\"@Custom.GuidAnnotation\":\"00000000-0000-0000-0000-000000000000\"," +
                "\"value\":[" +
                    "{" +
                        "\"@Custom.ComplexAnnotation\":{\"@odata.type\":\"#TestNamespace.TestComplexType\",\"StringProperty\":\"StringValue1\"}," +
                        "\"ID\":1," +
                        "\"Custom.PrimitiveCollectionAnnotation@odata.type\":\"#Collection(String)\"," +
                        "\"@Custom.PrimitiveCollectionAnnotation\":[\"StringValue1\",\"StringValue2\"]" +
                    "}" +
                "]," +
                "\"Custom.ComplexCollectionAnnotation@odata.type\":\"#Collection(TestNamespace.TestComplexType)\"," +
                "\"@Custom.ComplexCollectionAnnotation\":[{\"StringProperty\":\"StringValue1\"},{\"StringProperty\":\"StringValue2\"}]" +
            "}";

            WriteCustomInstanceAnnotationToFeedAndEntry(FeedWithCustomInstanceAnnotationInJsonLight, ODataFormat.Json);
        }

        [Fact]
        public void ShouldBeAbleToWriteCustomInstanceAnnotationToFeedAndEntryInAtom()
        {
            const string FeedWithCustomInstanceAnnotationInAtom =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<feed xmlns=\"http://www.w3.org/2005/Atom\" xmlns:d=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\" xmlns:georss=\"http://www.georss.org/georss\" xmlns:gml=\"http://www.opengis.net/gml\" m:context=\"http://www.example.com/$metadata#TestEntitySet\">" +
                    "<id>urn:feedId</id>" +
                    "<title />" +
                    "<updated>2013-03-12T22:14:47Z</updated>" +
                    "<m:annotation term=\"Custom.Int32Annotation\" int=\"123\" />" +
                    "<m:annotation term=\"Custom.GuidAnnotation\" m:type=\"Guid\">00000000-0000-0000-0000-000000000000</m:annotation>" +
                    "<entry>" +
                        "<m:annotation term=\"Custom.ComplexAnnotation\" m:type=\"#TestNamespace.TestComplexType\"><d:StringProperty>StringValue1</d:StringProperty></m:annotation>" +
                        "<id />" +
                        "<title />" +
                        "<updated>2013-03-12T22:14:47Z</updated>" +
                        "<author><name /></author>" +
                        "<content type=\"application/xml\">" +
                            "<m:properties>" +
                                "<d:ID m:type=\"Int32\">1</d:ID>" +
                            "</m:properties>" +
                        "</content>" +
                        "<m:annotation term=\"Custom.PrimitiveCollectionAnnotation\" m:type=\"#Collection(String)\"><m:element>StringValue1</m:element><m:element>StringValue2</m:element></m:annotation>" +
                    "</entry>" +
                    "<m:annotation term=\"Custom.ComplexCollectionAnnotation\" m:type=\"#Collection(TestNamespace.TestComplexType)\"><m:element><d:StringProperty>StringValue1</d:StringProperty></m:element><m:element><d:StringProperty>StringValue2</d:StringProperty></m:element></m:annotation>" +
                "</feed>";
            WriteCustomInstanceAnnotationToFeedAndEntry(FeedWithCustomInstanceAnnotationInAtom, ODataFormat.Atom);
        }

        private static void WriteCustomInstanceAnnotationToFeedAndEntry(string expectedPayload, ODataFormat format)
        {
            var writerSettings = new ODataMessageWriterSettings { DisableMessageStreamDisposal = true, EnableAtom = format == ODataFormat.Atom };
            writerSettings.SetContentType(format);
            writerSettings.ODataUri = new ODataUri() { ServiceRoot = new Uri("http://www.example.com/") };

            MemoryStream stream = new MemoryStream();
            IODataResponseMessage messageToWrite = new InMemoryMessage { StatusCode = 200, Stream = stream };
            messageToWrite.PreferenceAppliedHeader().AnnotationFilter = "Custom.*";

            // Write payload
            using (var messageWriter = new ODataMessageWriter(messageToWrite, writerSettings, Model))
            {
                var odataWriter = messageWriter.CreateODataFeedWriter(EntitySet, EntityType);

                // Add instance annotations to the feed.
                var feedToWrite = new ODataFeed { Id = new Uri("urn:feedId") };
                feedToWrite.InstanceAnnotations.Add(new ODataInstanceAnnotation("Custom.Int32Annotation", PrimitiveValue1));
                feedToWrite.InstanceAnnotations.Add(new ODataInstanceAnnotation("Custom.GuidAnnotation", PrimitiveValue2));
                feedToWrite.InstanceAnnotations.Add(new ODataInstanceAnnotation("ShouldSkip.Int32Annotation", PrimitiveValue1));
                feedToWrite.InstanceAnnotations.Add(new ODataInstanceAnnotation("ShouldSkip.GuidAnnotation", PrimitiveValue2));

                // Writes instance annotations at the beginning of the feed
                odataWriter.WriteStart(feedToWrite);

                // Add instance annotations to the entry.
                var entryToWrite = new ODataEntry { Properties = new[] { new ODataProperty { Name = "ID", Value = 1 } } };
                entryToWrite.InstanceAnnotations.Add(new ODataInstanceAnnotation("Custom.ComplexAnnotation", ComplexValue1));
                entryToWrite.InstanceAnnotations.Add(new ODataInstanceAnnotation("ShouldSkip.ComplexAnnotation", ComplexValue1));

                // Writes instance annotations at the beginning of the entry
                odataWriter.WriteStart(entryToWrite);

                // Add more instance annotations to the entry.
                entryToWrite.InstanceAnnotations.Add(new ODataInstanceAnnotation("Custom.PrimitiveCollectionAnnotation", PrimitiveCollectionValue));
                entryToWrite.InstanceAnnotations.Add(new ODataInstanceAnnotation("ShouldSkip.PrimitiveCollectionAnnotation", PrimitiveCollectionValue));

                // The writer remembers which instance annotations in the collection has been written
                // and only write out the unwritten ones since WriteStart() to the end of the entry.
                odataWriter.WriteEnd();

                // Add more instance annotations to the feed.
                feedToWrite.InstanceAnnotations.Add(new ODataInstanceAnnotation("Custom.ComplexCollectionAnnotation", ComplexCollectionValue));
                feedToWrite.InstanceAnnotations.Add(new ODataInstanceAnnotation("ShouldSkip.ComplexCollectionAnnotation", ComplexCollectionValue));

                // The writer remembers which instance annotations in the collection has been written
                // and only write out the unwritten ones since WriteStart() to the end of the feed.
                odataWriter.WriteEnd();
            }

            stream.Position = 0;
            string payload = (new StreamReader(stream)).ReadToEnd();
            if (format == ODataFormat.Atom)
            {
                // The <updated> element is computed dynamically, so we remove it from the both the baseline and the actual payload.
                payload = Regex.Replace(payload, "<updated>[^<]*</updated>", "");
                expectedPayload = Regex.Replace(expectedPayload, "<updated>[^<]*</updated>", "");
            }

            payload.Should().Be(expectedPayload);
        }

        #endregion Feed and entry with custom instance annotations.

        #region Error with custom instance annotations

        [Fact]
        public void ShouldBeAbleToReadCustomInstanceAnnotationFromErrorInJsonLight()
        {
            const string payload =
            "{" +
                "\"error\":{" +
                    "\"code\":\"400\"," +
                    "\"message\":\"Resource not found for the segment 'Address'.\"," +
                    "\"@instance.annotation\":\"stringValue\"" +
                "}" +
            "}";

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));

            // Read instance annotations
            var readerSettings = new ODataMessageReaderSettings { DisableMessageStreamDisposal = false };
            IODataResponseMessage messageToRead = new InMemoryMessage { StatusCode = 400, Stream = stream };
            messageToRead.SetHeader("Content-Type", "application/json;odata.streaming=true");

            using (var messageReader = new ODataMessageReader(messageToRead, readerSettings, Model))
            {
                ODataError error = messageReader.ReadError();
                error.InstanceAnnotations.Should().HaveCount(1).And.Contain(ia => ia.Name == "instance.annotation");
            }
        }

        [Fact]
        public void ShouldBeAbleToWriteCustomInstanceAnnotationToErrorInJsonLight()
        {
            const string expectedPayload =
            "{" +
                "\"error\":{" +
                    "\"code\":\"400\"," +
                    "\"message\":\"Resource not found for the segment 'Address'.\"," +
                    "\"@instance.annotation\":\"stringValue\"" +
                "}" +
            "}";

            var writerSettings = new ODataMessageWriterSettings { DisableMessageStreamDisposal = true };
            writerSettings.SetContentType(ODataFormat.Json);
            writerSettings.ODataUri = new ODataUri() { ServiceRoot = new Uri("http://www.example.com") };

            MemoryStream stream = new MemoryStream();
            IODataResponseMessage messageToWrite = new InMemoryMessage { StatusCode = 400, Stream = stream };

            // Write payload
            using (var messageWriter = new ODataMessageWriter(messageToWrite, writerSettings, Model))
            {
                ODataError error = new ODataError { ErrorCode = "400", Message = "Resource not found for the segment 'Address'." };
                error.InstanceAnnotations.Add(new ODataInstanceAnnotation("instance.annotation", new ODataPrimitiveValue("stringValue")));
                messageWriter.WriteError(error, includeDebugInformation: true);
            }

            stream.Position = 0;
            string payload = (new StreamReader(stream)).ReadToEnd();
            payload.Should().Be(expectedPayload);
        }

        #endregion Error with custom instance annotations
    }
}

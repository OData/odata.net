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
using Xunit;

namespace Microsoft.OData.Tests.ScenarioTests.Reader
{
    public class CustomInstanceAnnotationAcceptanceTests
    {
        private static readonly ODataPrimitiveValue PrimitiveValue1 = new ODataPrimitiveValue(123);
        private static readonly ODataPrimitiveValue PrimitiveValue2 = new ODataPrimitiveValue(Guid.Empty);
        private static readonly ODataCollectionValue PrimitiveCollectionValue = new ODataCollectionValue { Items = new[] { "StringValue1", "StringValue2" }, TypeName = "Collection(String)" };

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
            "\"@Custom.ResourceSetStartAnnotation\":1," +
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

        [Fact]
        public void CustomInstanceAnnotationFromFeedAndEntryInJsonLightShouldBeSkippedByTheReaderByDefault()
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(JsonLightFeedAndEntryPayloadWithCustomInstanceAnnotations));
            var readerSettings = new ODataMessageReaderSettings { EnableMessageStreamDisposal = true };
            IODataResponseMessage messageToRead = new InMemoryMessage { StatusCode = 200, Stream = stream };
            messageToRead.SetHeader("Content-Type", "application/json;odata.streaming=true");

            using (var messageReader = new ODataMessageReader(messageToRead, readerSettings, Model))
            {
                var odataReader = messageReader.CreateODataResourceSetReader(EntitySet, EntityType);
                while (odataReader.Read())
                {
                    switch (odataReader.State)
                    {
                        case ODataReaderState.ResourceSetStart:
                        case ODataReaderState.ResourceSetEnd:
                            odataReader.Item.As<ODataResourceSet>().InstanceAnnotations.Should().BeEmpty();
                            break;
                        case ODataReaderState.NestedResourceInfoStart:
                        case ODataReaderState.NestedResourceInfoEnd:
                            break;
                        case ODataReaderState.ResourceStart:
                        case ODataReaderState.ResourceEnd:
                            odataReader.Item.As<ODataResource>().InstanceAnnotations.Should().BeEmpty();
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

        public void ShouldBeAbleToReadCustomInstanceAnnotationFromFeedAndEntry(string payload, string contentType)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
            var readerSettings = new ODataMessageReaderSettings { EnableMessageStreamDisposal = true };

            IODataResponseMessage messageToRead = new InMemoryMessage { StatusCode = 200, Stream = stream };
            messageToRead.SetHeader("Content-Type", contentType);

            // Enable reading custom instance annotations.
            messageToRead.PreferenceAppliedHeader().AnnotationFilter = "Custom.*";

            Stack<ODataItem> odataItems = new Stack<ODataItem>(4);
            using (var messageReader = new ODataMessageReader(messageToRead, readerSettings, Model))
            {
                var odataReader = messageReader.CreateODataResourceSetReader(EntitySet, EntityType);
                ICollection<ODataInstanceAnnotation> instanceAnnotations = null;
                while (odataReader.Read())
                {
                    switch (odataReader.State)
                    {
                        case ODataReaderState.ResourceSetStart:
                            odataItems.Push(odataReader.Item);
                            instanceAnnotations = odataItems.Peek().As<ODataResourceSet>().InstanceAnnotations;

                            // TODO: We only support instance annotation at the top level feed at the moment. Will remove the if statement when support on inline feed is added.
                            if (odataItems.Count == 1)
                            {
                                // Note that in streaming mode, the collection should be populated with instance annotations read so far before the beginning of the first entry.
                                // We are currently in non-streaming mode. The reader will buffer the payload and read ahead till the
                                // end of the feed to read all instance annotations.
                                instanceAnnotations.Should().HaveCount(1);
                                TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(odataItems.Count), instanceAnnotations.Single(ia => ia.Name == "Custom.ResourceSetStartAnnotation").Value);
                            }
                            else
                            {
                                instanceAnnotations.Should().BeEmpty();
                            }

                            break;
                        case ODataReaderState.ResourceSetEnd:
                            instanceAnnotations = odataItems.Peek().As<ODataResourceSet>().InstanceAnnotations;

                            // TODO: We only support instance annotation at the top level feed at the moment. Will remove the if statement when support on inline feed is added.
                            if (odataItems.Count == 1)
                            {
                                instanceAnnotations.Should().HaveCount(2);
                                TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(1), instanceAnnotations.Single(ia => ia.Name == "Custom.ResourceSetStartAnnotation").Value);
                                TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(1), instanceAnnotations.Single(ia => ia.Name == "Custom.FeedEndAnnotation").Value);
                            }
                            else
                            {
                                instanceAnnotations.Should().BeEmpty();
                            }

                            odataItems.Pop();
                            break;
                        case ODataReaderState.NestedResourceInfoStart:
                            ODataNestedResourceInfo navigationLink = (ODataNestedResourceInfo)odataReader.Item;
                            if (navigationLink.Name == "ResourceSetNavigationProperty")
                            {
                                // The collection should be populated with instance annotations read so far before the "ResourceSetNavigationProperty".
                                instanceAnnotations = odataItems.Peek().As<ODataResource>().InstanceAnnotations;
                                instanceAnnotations.Should().HaveCount(2);
                                TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(odataItems.Count), instanceAnnotations.Single(ia => ia.Name == "Custom.EntryStartAnnotation").Value);
                                TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(odataItems.Count), instanceAnnotations.Single(ia => ia.Name == "Custom.EntryMiddleAnnotation").Value);
                            }

                            break;
                        case ODataReaderState.NestedResourceInfoEnd:
                            break;
                        case ODataReaderState.ResourceStart:
                            odataItems.Push(odataReader.Item);

                            // The collection should be populated with instance annotations read so far before the first navigation/association link or before the end of the entry.
                            instanceAnnotations = odataItems.Peek().As<ODataResource>().InstanceAnnotations;
                            instanceAnnotations.Should().HaveCount(1);
                            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(odataItems.Count), instanceAnnotations.Single(ia => ia.Name == "Custom.EntryStartAnnotation").Value);
                            break;

                        case ODataReaderState.ResourceEnd:
                            instanceAnnotations = odataItems.Peek().As<ODataResource>().InstanceAnnotations;
                            instanceAnnotations.Should().HaveCount(3);
                            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(odataItems.Count), instanceAnnotations.Single(ia => ia.Name == "Custom.EntryStartAnnotation").Value);
                            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(odataItems.Count), instanceAnnotations.Single(ia => ia.Name == "Custom.EntryMiddleAnnotation").Value);
                            TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(odataItems.Count), instanceAnnotations.Single(ia => ia.Name == "Custom.EntryEndAnnotation").Value);
                            odataItems.Pop();
                            break;
                    }
                }

                instanceAnnotations.Should().HaveCount(2);
                TestUtils.AssertODataValueAreEqual(new ODataPrimitiveValue(1), instanceAnnotations.Single(ia => ia.Name == "Custom.ResourceSetStartAnnotation").Value);
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
                        "\"ID\":1," +
                        "\"Custom.PrimitiveCollectionAnnotation@odata.type\":\"#Collection(String)\"," +
                        "\"@Custom.PrimitiveCollectionAnnotation\":[\"StringValue1\",\"StringValue2\"]" +
                    "}" +
                "]" +
            "}";

            WriteCustomInstanceAnnotationToFeedAndEntry(FeedWithCustomInstanceAnnotationInJsonLight, ODataFormat.Json);
        }

        private static void WriteCustomInstanceAnnotationToFeedAndEntry(string expectedPayload, ODataFormat format)
        {
            var writerSettings = new ODataMessageWriterSettings { EnableMessageStreamDisposal = false };
            writerSettings.SetContentType(format);
            writerSettings.ODataUri = new ODataUri() { ServiceRoot = new Uri("http://www.example.com/") };

            MemoryStream stream = new MemoryStream();
            IODataResponseMessage messageToWrite = new InMemoryMessage { StatusCode = 200, Stream = stream };
            messageToWrite.PreferenceAppliedHeader().AnnotationFilter = "Custom.*";

            // Write payload
            using (var messageWriter = new ODataMessageWriter(messageToWrite, writerSettings, Model))
            {
                var odataWriter = messageWriter.CreateODataResourceSetWriter(EntitySet, EntityType);

                // Add instance annotations to the feed.
                var feedToWrite = new ODataResourceSet { Id = new Uri("urn:feedId") };
                feedToWrite.InstanceAnnotations.Add(new ODataInstanceAnnotation("Custom.Int32Annotation", PrimitiveValue1));
                feedToWrite.InstanceAnnotations.Add(new ODataInstanceAnnotation("Custom.GuidAnnotation", PrimitiveValue2));
                feedToWrite.InstanceAnnotations.Add(new ODataInstanceAnnotation("ShouldSkip.Int32Annotation", PrimitiveValue1));
                feedToWrite.InstanceAnnotations.Add(new ODataInstanceAnnotation("ShouldSkip.GuidAnnotation", PrimitiveValue2));

                // Writes instance annotations at the beginning of the feed
                odataWriter.WriteStart(feedToWrite);

                // Add instance annotations to the entry.
                var entryToWrite = new ODataResource { Properties = new[] { new ODataProperty { Name = "ID", Value = 1 } } };

                // Writes instance annotations at the beginning of the entry
                odataWriter.WriteStart(entryToWrite);

                // Add more instance annotations to the entry.
                entryToWrite.InstanceAnnotations.Add(new ODataInstanceAnnotation("Custom.PrimitiveCollectionAnnotation", PrimitiveCollectionValue));
                entryToWrite.InstanceAnnotations.Add(new ODataInstanceAnnotation("ShouldSkip.PrimitiveCollectionAnnotation", PrimitiveCollectionValue));

                // The writer remembers which instance annotations in the collection has been written
                // and only write out the unwritten ones since WriteStart() to the end of the entry.
                odataWriter.WriteEnd();

                // The writer remembers which instance annotations in the collection has been written
                // and only write out the unwritten ones since WriteStart() to the end of the feed.
                odataWriter.WriteEnd();
            }

            stream.Position = 0;
            string payload = (new StreamReader(stream)).ReadToEnd();

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
            var readerSettings = new ODataMessageReaderSettings { EnableMessageStreamDisposal = true };
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

            var writerSettings = new ODataMessageWriterSettings { EnableMessageStreamDisposal = false };
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

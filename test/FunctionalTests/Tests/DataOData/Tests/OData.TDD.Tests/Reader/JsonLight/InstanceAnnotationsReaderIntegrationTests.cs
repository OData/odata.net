﻿//---------------------------------------------------------------------
// <copyright file="InstanceAnnotationsReaderIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Reader.JsonLight
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.Test.OData.TDD.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class InstanceAnnotationsReaderIntegrationTests
    {
        private static readonly ODataPrimitiveValue PrimitiveValue123 = new ODataPrimitiveValue(123);
        private static readonly ODataPrimitiveValue PrimitiveValue456 = new ODataPrimitiveValue(456);

        private static readonly IEdmModel Model;
        private static readonly EdmEntitySet EntitySet;
        private static readonly EdmSingleton Singleton;
        private static readonly EdmEntityType EntityType;
        private static readonly EdmComplexType ComplexType;

        static InstanceAnnotationsReaderIntegrationTests()
        {
            EdmModel modelTmp = new EdmModel();
            EntityType = new EdmEntityType("TestNamespace", "TestEntityType");
            modelTmp.AddElement(EntityType);

            var keyProperty = new EdmStructuralProperty(EntityType, "ID", EdmCoreModel.Instance.GetInt32(false));
            EntityType.AddKeys(new IEdmStructuralProperty[] { keyProperty });
            EntityType.AddProperty(keyProperty);
            var resourceNavigationProperty = EntityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "ResourceNavigationProperty", Target = EntityType, TargetMultiplicity = EdmMultiplicity.ZeroOrOne });
            var resourceSetNavigationProperty = EntityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "ResourceSetNavigationProperty", Target = EntityType, TargetMultiplicity = EdmMultiplicity.Many });

            var defaultContainer = new EdmEntityContainer("TestNamespace", "DefaultContainer_sub");
            modelTmp.AddElement(defaultContainer);
            EntitySet = new EdmEntitySet(defaultContainer, "TestEntitySet", EntityType);
            EntitySet.AddNavigationTarget(resourceNavigationProperty, EntitySet);
            EntitySet.AddNavigationTarget(resourceSetNavigationProperty, EntitySet);
            defaultContainer.AddElement(EntitySet);

            Singleton = new EdmSingleton(defaultContainer, "TestSingleton", EntityType);
            Singleton.AddNavigationTarget(resourceNavigationProperty, EntitySet);
            Singleton.AddNavigationTarget(resourceSetNavigationProperty, EntitySet);
            defaultContainer.AddElement(Singleton);

            ComplexType = new EdmComplexType("TestNamespace", "TestComplexType");
            ComplexType.AddProperty(new EdmStructuralProperty(ComplexType, "StringProperty", EdmCoreModel.Instance.GetString(false)));
            modelTmp.AddElement(ComplexType);

            Model = TestUtils.WrapReferencedModelsToMainModel("TestNamespace", "DefaultContainer", modelTmp);
        }

        #region Reading Instance Annotations from Top-Level Feeds

        private const string TopLevelFeedWithInstanceAnnotation =
        "{" +
            "\"@odata.context\":\"http://www.example.com/service.svc/$metadata#TestEntitySet\"," +
            "\"@custom.Int32Annotation1\":123," +
            "\"value\":[]," +
            "\"@custom.Int32Annotation2\":456" +
        "}";

        [TestMethod]
        public void ShouldReadInstanceAnnotationsOnTopLevelFeedResponseInNonStreamingMode()
        {
            TopLevelFeedInstanceAnnotationTest(TopLevelFeedWithInstanceAnnotation, streaming: false, isResponse: true);
        }

        [TestMethod]
        public void ShouldNotReadInstanceAnnotationsOnTopLevelFeedResponseInNonStreamingMode()
        {
            TopLevelFeedInstanceAnnotationTest(TopLevelFeedWithInstanceAnnotation, streaming: false, isResponse: true, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        [TestMethod]
        public void ShouldReadInstanceAnnotationsOnTopLevelFeedRequestInNonStreamingMode()
        {
            TopLevelFeedInstanceAnnotationTest(TopLevelFeedWithInstanceAnnotation, streaming: false, isResponse: false);
        }

        [TestMethod]
        public void ShouldNotReadInstanceAnnotationsOnTopLevelFeedRequestInNonStreamingMode()
        {
            TopLevelFeedInstanceAnnotationTest(TopLevelFeedWithInstanceAnnotation, streaming: false, isResponse: false, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        [TestMethod]
        public void ShouldReadInstanceAnnotationsOnTopLevelFeedResponseInStreamingMode()
        {
            TopLevelFeedInstanceAnnotationTest(TopLevelFeedWithInstanceAnnotation, streaming: true, isResponse: true);
        }

        [TestMethod]
        public void ShouldNotReadInstanceAnnotationsOnTopLevelFeedResponseInStreamingMode()
        {
            TopLevelFeedInstanceAnnotationTest(TopLevelFeedWithInstanceAnnotation, streaming: true, isResponse: true, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        [TestMethod]
        public void ShouldReadInstanceAnnotationsOnTopLevelFeedRequestInStreamingMode()
        {
            TopLevelFeedInstanceAnnotationTest(TopLevelFeedWithInstanceAnnotation, streaming: true, isResponse: false);
        }

        [TestMethod]
        public void ShouldNotReadInstanceAnnotationsOnTopLevelFeedRequestInStreamingMode()
        {
            TopLevelFeedInstanceAnnotationTest(TopLevelFeedWithInstanceAnnotation, streaming: true, isResponse: false, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        internal static void TopLevelFeedInstanceAnnotationTest(string feedPayload, string contentType, bool isResponse, bool shouldReadAndValidateCustomInstanceAnnotations = true, bool odataSimplified = false)
        {
            ODataFeed feedFromReader = null;
            using (var messageReader = CreateODataMessageReader(feedPayload, contentType, isResponse, shouldReadAndValidateCustomInstanceAnnotations, odataSimplified))
            {
                var odataReader = messageReader.CreateODataFeedReader(EntitySet, EntityType);

                while (odataReader.Read())
                {
                    switch (odataReader.State)
                    {
                        case ODataReaderState.FeedStart:
                            feedFromReader = (ODataFeed)odataReader.Item;
                            if (IsStreaming(contentType))
                            {
                                if (shouldReadAndValidateCustomInstanceAnnotations)
                                {
                                    ValidateContainsAllExpectedInstanceAnnotationsBeforeStateChange(feedFromReader.InstanceAnnotations, shouldReadAndValidateCustomInstanceAnnotations);
                                }
                                else
                                {
                                    feedFromReader.InstanceAnnotations.Count.Should().Be(0);
                                }
                            }
                            else
                            {
                                ValidateContainsAllExpectedInstanceAnnotations(feedFromReader.InstanceAnnotations, shouldReadAndValidateCustomInstanceAnnotations);
                            }
                            break;

                        case ODataReaderState.FeedEnd:
                            feedFromReader.Should().NotBeNull();
                            ValidateContainsAllExpectedInstanceAnnotations(feedFromReader.InstanceAnnotations, shouldReadAndValidateCustomInstanceAnnotations);

                            break;
                    }
                }
            }

            feedFromReader.Should().NotBeNull();
            ValidateContainsAllExpectedInstanceAnnotations(feedFromReader.InstanceAnnotations, shouldReadAndValidateCustomInstanceAnnotations);
        }

        private static void TopLevelFeedInstanceAnnotationTest(string feedPayload, bool streaming, bool isResponse, bool shouldReadAndValidateCustomInstanceAnnotations = true, bool odataSimplified = false)
        {
            TopLevelFeedInstanceAnnotationTest(feedPayload, GetContentType(streaming), isResponse, shouldReadAndValidateCustomInstanceAnnotations, odataSimplified);
        }

        private static ODataMessageReader CreateODataMessageReader(string payload, string contentType, bool isResponse, bool shouldReadAndValidateCustomInstanceAnnotations, bool odataSimplified = false)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
            var readerSettings = new ODataMessageReaderSettings { DisableMessageStreamDisposal = false, EnableAtom = true, ODataSimplified = odataSimplified };
            ODataMessageReader messageReader;
            if (isResponse)
            {
                IODataResponseMessage responseMessage = new InMemoryMessage { StatusCode = 200, Stream = stream };
                responseMessage.SetHeader("Content-Type", contentType);
                if (shouldReadAndValidateCustomInstanceAnnotations)
                {
                    responseMessage.PreferenceAppliedHeader().AnnotationFilter = "*";
                }

                messageReader = new ODataMessageReader(responseMessage, readerSettings, Model);
            }
            else
            {
                IODataRequestMessage requestMessage = new InMemoryMessage { Method = "GET", Stream = stream };
                requestMessage.SetHeader("Content-Type", contentType);
                readerSettings.ShouldIncludeAnnotation = shouldReadAndValidateCustomInstanceAnnotations ? ODataUtils.CreateAnnotationFilter("*") : null;
                messageReader = new ODataMessageReader(requestMessage, readerSettings, Model);
            }

            return messageReader;
        }

        private static void ValidateContainsAllExpectedInstanceAnnotationsBeforeStateChange(IEnumerable<ODataInstanceAnnotation> instanceAnnotations, bool shouldReadAndValidateCustomInstanceAnnotations)
        {
            instanceAnnotations.Should().NotBeNull();
            if (shouldReadAndValidateCustomInstanceAnnotations)
            {
                instanceAnnotations.Should().HaveCount(1);
                TestUtils.AssertODataValueAreEqual(PrimitiveValue123, instanceAnnotations.Single(ia => ia.Name == "custom.Int32Annotation1").Value);
            }
            else
            {
                instanceAnnotations.Should().BeEmpty();
            }
        }

        private static void ValidateContainsAllExpectedInstanceAnnotations(IEnumerable<ODataInstanceAnnotation> instanceAnnotations, bool shouldReadAndValidateCustomInstanceAnnotations)
        {
            instanceAnnotations.Should().NotBeNull();
            if (shouldReadAndValidateCustomInstanceAnnotations)
            {
                instanceAnnotations.Should().HaveCount(2);
                TestUtils.AssertODataValueAreEqual(PrimitiveValue123, instanceAnnotations.Single(ia => ia.Name == "custom.Int32Annotation1").Value);
                TestUtils.AssertODataValueAreEqual(PrimitiveValue456, instanceAnnotations.Single(ia => ia.Name == "custom.Int32Annotation2").Value);
            }
            else
            {
                instanceAnnotations.Should().BeEmpty();
            }
        }

        #endregion Reading Instance Annotations from Top-Level Feeds

        #region Reading Instance Annotations from Expanded Feeds

        // TODO: Need to design and support these.
        [Ignore]
        [TestMethod]
        public void ShouldReadInstanceAnnotationsOnExpandedFeedResponseInNonStreamingMode()
        {
        }

        [Ignore]
        [TestMethod]
        public void ShouldReadInstanceAnnotationsOnExpandedFeedRequestInNonStreamingMode()
        {
        }

        [Ignore]
        [TestMethod]
        public void ShouldReadInstanceAnnotationsOnExpandedFeedResponseInStreamingMode()
        {
        }

        [Ignore]
        [TestMethod]
        public void ShouldReadInstanceAnnotationsOnExpandedFeedRequestInStreamingMode()
        {
        }

        #endregion Reading Instance Annotations from Expanded Feeds

        #region Reading Instance Annotations from Top-Level Entries

        private const string TopLevelEntryResponseWithInstanceAnnotation =
        "{" +
            "\"@odata.context\":\"http://www.example.com/service.svc/$metadata#TestEntitySet/$entity\"," +
            "\"@custom.Int32Annotation1\":123," +
            "\"ID\":1," +
            "\"ResourceNavigationProperty@odata.navigationLink\":\"http://example.com/foo/baz\"," +
            "\"@custom.Int32Annotation2\":456" +
        "}";

        private const string TopLevelEntryRequestWithInstanceAnnotation =
        "{" +
            "\"@odata.context\":\"http://www.example.com/service.svc/$metadata#TestEntitySet/$entity\"," +
            "\"@custom.Int32Annotation1\":123," +
            "\"ID\":1," +
            "\"ResourceNavigationProperty@odata.bind\":\"http://example.com/foo/baz\"," +
            "\"@custom.Int32Annotation2\":456" +
        "}";

        private const string SingletonResponseWithInstanceAnnotation =
        "{" +
            "\"@odata.context\":\"http://www.example.com/service.svc/$metadata#TestSingleton\"," +
            "\"@custom.Int32Annotation1\":123," +
            "\"ID\":1," +
            "\"ResourceNavigationProperty@odata.navigationLink\":\"http://example.com/foo/baz\"," +
            "\"@custom.Int32Annotation2\":456" +
        "}";

        private const string SingletonRequestWithInstanceAnnotation =
        "{" +
            "\"@odata.context\":\"http://www.example.com/service.svc/$metadata#TestSingleton\"," +
            "\"@custom.Int32Annotation1\":123," +
            "\"ID\":1," +
            "\"ResourceNavigationProperty@odata.bind\":\"http://example.com/foo/baz\"," +
            "\"@custom.Int32Annotation2\":456" +
        "}";

        [TestMethod]
        public void ShouldReadInstanceAnnotationsOnTopLevelEntryResponseInNonStreamingMode()
        {
            TopLevelEntryInstanceAnnotationTest(TopLevelEntryResponseWithInstanceAnnotation, streaming: false, isResponse: true);
        }

        [TestMethod]
        public void ShouldReadInstanceAnnotationsOnSingletonResponseInNonStreamingMode()
        {
            TopLevelEntryInstanceAnnotationTestOfSingleton(SingletonResponseWithInstanceAnnotation, streaming: false, isResponse: true);
        }

        [TestMethod]
        public void ShouldNotReadInstanceAnnotationsOnTopLevelEntryResponseInNonStreamingMode()
        {
            TopLevelEntryInstanceAnnotationTest(TopLevelEntryResponseWithInstanceAnnotation, streaming: false, isResponse: true, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        [TestMethod]
        public void ShouldReadInstanceAnnotationsOnTopLevelEntryRequestInNonStreamingMode()
        {
            TopLevelEntryInstanceAnnotationTest(TopLevelEntryRequestWithInstanceAnnotation, streaming: false, isResponse: false);
        }

        [TestMethod]
        public void ShouldNotReadInstanceAnnotationsOnTopLevelEntryRequestInNonStreamingMode()
        {
            TopLevelEntryInstanceAnnotationTest(TopLevelEntryRequestWithInstanceAnnotation, streaming: false, isResponse: false, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        [TestMethod]
        public void ShouldReadInstanceAnnotationsOnTopLevelEntryResponseInStreamingMode()
        {
            TopLevelEntryInstanceAnnotationTest(TopLevelEntryResponseWithInstanceAnnotation, streaming: true, isResponse: true);
        }

        [TestMethod]
        public void ShouldNotReadInstanceAnnotationsOnTopLevelEntryResponseInStreamingMode()
        {
            TopLevelEntryInstanceAnnotationTest(TopLevelEntryResponseWithInstanceAnnotation, streaming: true, isResponse: true, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        [TestMethod]
        public void ShouldReadInstanceAnnotationsOnTopLevelEntryRequestInStreamingMode()
        {
            TopLevelEntryInstanceAnnotationTest(TopLevelEntryRequestWithInstanceAnnotation, streaming: true, isResponse: false);
        }

        [TestMethod]
        public void ShouldNotReadInstanceAnnotationsOnTopLevelEntryRequestInStreamingMode()
        {
            TopLevelEntryInstanceAnnotationTest(TopLevelEntryRequestWithInstanceAnnotation, streaming: true, isResponse: false, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        //Singleton annotation test
        [TestMethod]
        public void ShouldNotReadInstanceAnnotationsOnSingletonResponseInNonStreamingMode()
        {
            TopLevelEntryInstanceAnnotationTestOfSingleton(SingletonResponseWithInstanceAnnotation, streaming: false, isResponse: true, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        [TestMethod]
        public void ShouldReadInstanceAnnotationsOnSingletonRequestInNonStreamingMode()
        {
            TopLevelEntryInstanceAnnotationTestOfSingleton(SingletonRequestWithInstanceAnnotation, streaming: false, isResponse: false);
        }

        [TestMethod]
        public void ShouldNotReadInstanceAnnotationsOnSingletonRequestInNonStreamingMode()
        {
            TopLevelEntryInstanceAnnotationTestOfSingleton(SingletonRequestWithInstanceAnnotation, streaming: false, isResponse: false, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        [TestMethod]
        public void ShouldReadInstanceAnnotationsOnSingletonResponseInStreamingMode()
        {
            TopLevelEntryInstanceAnnotationTestOfSingleton(SingletonResponseWithInstanceAnnotation, streaming: true, isResponse: true);
        }

        [TestMethod]
        public void ShouldNotReadInstanceAnnotationsOnSingletonResponseInStreamingMode()
        {
            TopLevelEntryInstanceAnnotationTestOfSingleton(SingletonResponseWithInstanceAnnotation, streaming: true, isResponse: true, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        [TestMethod]
        public void ShouldReadInstanceAnnotationsOnSingletonRequestInStreamingMode()
        {
            TopLevelEntryInstanceAnnotationTestOfSingleton(SingletonRequestWithInstanceAnnotation, streaming: true, isResponse: false);
        }

        [TestMethod]
        public void ShouldNotReadInstanceAnnotationsOnSingletonRequestInStreamingMode()
        {
            TopLevelEntryInstanceAnnotationTestOfSingleton(SingletonRequestWithInstanceAnnotation, streaming: true, isResponse: false, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        internal static void TopLevelEntryInstanceAnnotationTest(string payload, string contentType, bool isSingleton, bool isResponse, bool shouldReadAndValidateCustomInstanceAnnotations = true)
        {
            ODataEntry entryFromReader = null;
            using (var messageReader = CreateODataMessageReader(payload, contentType, isResponse, shouldReadAndValidateCustomInstanceAnnotations))
            {
                IEdmNavigationSource navigationSource;
                if (isSingleton)
                {
                    navigationSource = Singleton;
                }
                else
                {
                    navigationSource = EntitySet;
                }

                var odataReader = messageReader.CreateODataEntryReader(navigationSource, EntityType);

                while (odataReader.Read())
                {
                    switch (odataReader.State)
                    {
                        case ODataReaderState.EntryStart:
                            entryFromReader = (ODataEntry)odataReader.Item;
                            ValidateContainsAllExpectedInstanceAnnotationsBeforeStateChange(entryFromReader.InstanceAnnotations, shouldReadAndValidateCustomInstanceAnnotations);
                            break;

                        case ODataReaderState.EntryEnd:
                            entryFromReader.Should().NotBeNull();
                            ValidateContainsAllExpectedInstanceAnnotations(entryFromReader.InstanceAnnotations, shouldReadAndValidateCustomInstanceAnnotations);
                            break;
                    }
                }
            }

            entryFromReader.Should().NotBeNull();
            ValidateContainsAllExpectedInstanceAnnotations(entryFromReader.InstanceAnnotations, shouldReadAndValidateCustomInstanceAnnotations);
        }

        internal static void TopLevelEntryInstanceAnnotationTest(string payload, bool streaming, bool isResponse, bool shouldReadAndValidateCustomInstanceAnnotations = true)
        {
            TopLevelEntryInstanceAnnotationTest(payload, GetContentType(streaming), /*isSingleton*/false, isResponse, shouldReadAndValidateCustomInstanceAnnotations);
        }

        private static void TopLevelEntryInstanceAnnotationTestOfSingleton(string payload, bool streaming, bool isResponse, bool shouldReadAndValidateCustomInstanceAnnotations = true)
        {
            TopLevelEntryInstanceAnnotationTest(payload, GetContentType(streaming), /*isSingleton*/true, isResponse, shouldReadAndValidateCustomInstanceAnnotations);
        }

        #endregion Reading Instance Annotations from Top-Level Entries

        #region Reading Instance Annotations from Expanded Entries

        private const string ExpandedEntryResponseWithInstanceAnnotation =
        "{" +
            "\"@odata.context\":\"http://www.example.com/service.svc/$metadata#TestEntitySet/$entity\"," +
            "\"ID\":1," +
            "\"ResourceNavigationProperty\":{" +
                "\"@custom.Int32Annotation1\":123," +
                "\"ID\":2," +
                "\"ResourceNavigationProperty@odata.navigationLink\":\"http://example.com/foo/baz\"," +
                "\"@custom.Int32Annotation2\":456" +
            "}" +
        "}";

        private const string ExpandedEntryRequestWithInstanceAnnotation =
        "{" +
            "\"@odata.context\":\"http://www.example.com/service.svc/$metadata#TestEntitySet/$entity\"," +
            "\"ID\":1," +
            "\"ResourceNavigationProperty\":{" +
                "\"@custom.Int32Annotation1\":123," +
                "\"ID\":2," +
                "\"ResourceNavigationProperty@odata.bind\":\"http://example.com/foo/baz\"," +
                "\"@custom.Int32Annotation2\":456" +
            "}" +
        "}";

        [TestMethod]
        public void ShouldReadInstanceAnnotationsOnInlineEntryResponseInStreamingMode()
        {
            InlineEntryInstanceAnnotationTest(ExpandedEntryResponseWithInstanceAnnotation, streaming: true, isResponse: true);
        }

        [TestMethod]
        public void ShouldNotReadInstanceAnnotationsOnInlineEntryResponseInStreamingMode()
        {
            InlineEntryInstanceAnnotationTest(ExpandedEntryResponseWithInstanceAnnotation, streaming: true, isResponse: true, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        [TestMethod]
        public void ShouldReadInstanceAnnotationsOnInlineEntryRequestInStreamingMode()
        {
            InlineEntryInstanceAnnotationTest(ExpandedEntryRequestWithInstanceAnnotation, streaming: true, isResponse: false);
        }

        [TestMethod]
        public void ShouldNotReadInstanceAnnotationsOnInlineEntryRequestInStreamingMode()
        {
            InlineEntryInstanceAnnotationTest(ExpandedEntryRequestWithInstanceAnnotation, streaming: true, isResponse: false, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        [TestMethod]
        public void ShouldReadInstanceAnnotationsOnInlineEntryResponseInNonStreamingMode()
        {
            InlineEntryInstanceAnnotationTest(ExpandedEntryResponseWithInstanceAnnotation, streaming: false, isResponse: true);
        }

        [TestMethod]
        public void ShouldNotReadInstanceAnnotationsOnInlineEntryResponseInNonStreamingMode()
        {
            InlineEntryInstanceAnnotationTest(ExpandedEntryResponseWithInstanceAnnotation, streaming: false, isResponse: true, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        [TestMethod]
        public void ShouldReadInstanceAnnotationsOnInlineEntryRequestInNonStreamingMode()
        {
            InlineEntryInstanceAnnotationTest(ExpandedEntryRequestWithInstanceAnnotation, streaming: false, isResponse: false);
        }

        [TestMethod]
        public void ShouldNotReadInstanceAnnotationsOnInlineEntryRequestInNonStreamingMode()
        {
            InlineEntryInstanceAnnotationTest(ExpandedEntryRequestWithInstanceAnnotation, streaming: false, isResponse: false, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        private const string EntryInsideExpandedFeedResponseWithInstanceAnnotation =
        "{" +
            "\"@odata.context\":\"http://www.example.com/service.svc/$metadata#TestEntitySet/$entity\"," +
            "\"ID\":1," +
            "\"ResourceSetNavigationProperty\":[" +
                "{" +
                    "\"@custom.Int32Annotation1\":123," +
                    "\"ID\":2," +
                    "\"ResourceNavigationProperty@odata.navigationLink\":\"http://example.com/foo/baz\"," +
                    "\"@custom.Int32Annotation2\":456" +
                "}" +
            "]" +
        "}";

        private const string EntryInsideExpandedFeedRequestWithInstanceAnnotation =
        "{" +
            "\"@odata.context\":\"http://www.example.com/service.svc/$metadata#TestEntitySet/$entity\"," +
            "\"ID\":1," +
            "\"ResourceSetNavigationProperty\":[" +
                "{" +
                    "\"@custom.Int32Annotation1\":123," +
                    "\"ID\":2," +
                    "\"ResourceNavigationProperty@odata.bind\":\"http://example.com/foo/baz\"," +
                    "\"@custom.Int32Annotation2\":456" +
                "}" +
            "]" +
        "}";

        [TestMethod]
        public void ShouldReadInstanceAnnotationsOnInlineEntryInsideFeedResponseInStreamingMode()
        {
            InlineEntryInstanceAnnotationTest(EntryInsideExpandedFeedResponseWithInstanceAnnotation, streaming: true, isResponse: true);
        }

        [TestMethod]
        public void ShouldNotReadInstanceAnnotationsOnInlineEntryInsideFeedResponseInStreamingMode()
        {
            InlineEntryInstanceAnnotationTest(EntryInsideExpandedFeedResponseWithInstanceAnnotation, streaming: true, isResponse: true, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        [TestMethod]
        public void ShouldReadInstanceAnnotationsOnInlineEntryInsideFeedRequestInStreamingMode()
        {
            InlineEntryInstanceAnnotationTest(EntryInsideExpandedFeedRequestWithInstanceAnnotation, streaming: true, isResponse: false);
        }

        [TestMethod]
        public void ShouldNotReadInstanceAnnotationsOnInlineEntryInsideFeedRequestInStreamingMode()
        {
            InlineEntryInstanceAnnotationTest(EntryInsideExpandedFeedRequestWithInstanceAnnotation, streaming: true, isResponse: false, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        [TestMethod]
        public void ShouldReadInstanceAnnotationsOnInlineEntryInsideFeedResponseInNonStreamingMode()
        {
            InlineEntryInstanceAnnotationTest(EntryInsideExpandedFeedResponseWithInstanceAnnotation, streaming: false, isResponse: true);
        }

        [TestMethod]
        public void ShouldNotReadInstanceAnnotationsOnInlineEntryInsideFeedResponseInNonStreamingMode()
        {
            InlineEntryInstanceAnnotationTest(EntryInsideExpandedFeedResponseWithInstanceAnnotation, streaming: false, isResponse: true, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        [TestMethod]
        public void ShouldReadInstanceAnnotationsOnInlineEntryInsideFeedRequestInNonStreamingMode()
        {
            InlineEntryInstanceAnnotationTest(EntryInsideExpandedFeedRequestWithInstanceAnnotation, streaming: false, isResponse: false);
        }

        [TestMethod]
        public void ShouldNotReadInstanceAnnotationsOnInlineEntryInsideFeedRequestInNonStreamingMode()
        {
            InlineEntryInstanceAnnotationTest(EntryInsideExpandedFeedRequestWithInstanceAnnotation, streaming: false, isResponse: false, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        internal static void InlineEntryInstanceAnnotationTest(string payload, string contentType, bool isResponse, bool shouldReadAndValidateCustomInstanceAnnotations = true, bool odataSimplified = false)
        {
            ODataEntry entryFromReader = null;
            int depth = 0;
            using (var messageReader = CreateODataMessageReader(payload, contentType, isResponse, shouldReadAndValidateCustomInstanceAnnotations, odataSimplified))
            {
                var odataReader = messageReader.CreateODataEntryReader(EntitySet, EntityType);

                while (odataReader.Read())
                {
                    switch (odataReader.State)
                    {
                        case ODataReaderState.NavigationLinkStart:
                            depth++;
                            break;

                        case ODataReaderState.NavigationLinkEnd:
                            depth--;
                            break;

                        case ODataReaderState.EntryStart:
                            if (depth == 1)
                            {
                                entryFromReader = (ODataEntry)odataReader.Item;
                                ValidateContainsAllExpectedInstanceAnnotationsBeforeStateChange(entryFromReader.InstanceAnnotations, shouldReadAndValidateCustomInstanceAnnotations);
                            }
                            else
                            {
                                ((ODataEntry)odataReader.Item).InstanceAnnotations.Should().HaveCount(0);
                            }

                            break;

                        case ODataReaderState.EntryEnd:
                            if (depth == 1)
                            {
                                entryFromReader.Should().NotBeNull();
                                ValidateContainsAllExpectedInstanceAnnotations(entryFromReader.InstanceAnnotations, shouldReadAndValidateCustomInstanceAnnotations);
                            }
                            else
                            {
                                ((ODataEntry)odataReader.Item).InstanceAnnotations.Should().HaveCount(0);
                            }

                            break;
                    }
                }
            }

            entryFromReader.Should().NotBeNull();
            ValidateContainsAllExpectedInstanceAnnotations(entryFromReader.InstanceAnnotations, shouldReadAndValidateCustomInstanceAnnotations);
        }

        private static void InlineEntryInstanceAnnotationTest(string payload, bool streaming, bool isResponse, bool shouldReadAndValidateCustomInstanceAnnotations = true, bool odataSimplified = false)
        {
            InlineEntryInstanceAnnotationTest(payload, GetContentType(streaming), isResponse, shouldReadAndValidateCustomInstanceAnnotations, odataSimplified);
        }

        #endregion Reading Instance Annotations from Expanded Entries

        #region OData Simplified

        private const string TopLevelFeedWithSimplifiedInstanceAnnotation =
        "{" +
            "\"@context\":\"http://www.example.com/service.svc/$metadata#TestEntitySet\"," +
            "\"@custom.Int32Annotation1\":123," +
            "\"value\":[]," +
            "\"@custom.Int32Annotation2\":456" +
        "}";

        [TestMethod]
        public void ShouldReadSimplifiedInstanceAnnotationsOnTopLevelFeedResponseInNonStreamingModeODataSimplified()
        {
            // cover "@context"
            TopLevelFeedInstanceAnnotationTest(TopLevelFeedWithSimplifiedInstanceAnnotation, streaming: false, isResponse: true, odataSimplified: true);
        }

        [TestMethod]
        public void ShouldReadFullInstanceAnnotationsOnTopLevelFeedResponseInNonStreamingModeODataSimplified()
        {
            // cover "@odata.context"
            TopLevelFeedInstanceAnnotationTest(TopLevelFeedWithInstanceAnnotation, streaming: false, isResponse: true, odataSimplified: true);
        }

        private const string ExpandedEntryResponseWithSimplifiedInstanceAnnotation =
        "{" +
            "\"@context\":\"http://www.example.com/service.svc/$metadata#TestEntitySet/$entity\"," +
            "\"ID\":1," +
            "\"ResourceNavigationProperty\":{" +
                "\"@custom.Int32Annotation1\":123," +
                "\"ID\":2," +
                "\"ResourceNavigationProperty@navigationLink\":\"http://example.com/foo/baz\"," +
                "\"@custom.Int32Annotation2\":456" +
            "}" +
        "}";

        [TestMethod]
        public void ShouldReadSimplifiedInstanceAnnotationsOnInlineEntryResponseInStreamingModeODataSimplified()
        {
            // cover "@navigationLink"
            InlineEntryInstanceAnnotationTest(ExpandedEntryResponseWithSimplifiedInstanceAnnotation, streaming: true, isResponse: true, odataSimplified: true);
        }

        [TestMethod]
        public void ShouldReadFullInstanceAnnotationsOnInlineEntryResponseInStreamingModeODataSimplified()
        {
            // cover "@odata.navigationLink"
            InlineEntryInstanceAnnotationTest(ExpandedEntryResponseWithInstanceAnnotation, streaming: true, isResponse: true, odataSimplified: true);
        }

        private const string EntryInsideExpandedFeedRequestWithSimplifiedInstanceAnnotation =
        "{" +
            "\"@context\":\"http://www.example.com/service.svc/$metadata#TestEntitySet/$entity\"," +
            "\"ID\":1," +
            "\"ResourceSetNavigationProperty\":[" +
                "{" +
                    "\"@custom.Int32Annotation1\":123," +
                    "\"ID\":2," +
                    "\"ResourceNavigationProperty@bind\":\"http://example.com/foo/baz\"," +
                    "\"@custom.Int32Annotation2\":456" +
                "}" +
            "]" +
        "}";

        [TestMethod]
        public void ShouldReadSimplifiedInstanceAnnotationsOnInlineEntryInsideFeedRequestInStreamingModeODataSimplified()
        {
            // cover "@bind"
            InlineEntryInstanceAnnotationTest(EntryInsideExpandedFeedRequestWithSimplifiedInstanceAnnotation, streaming: true, isResponse: false, odataSimplified: true);
        }

        [TestMethod]
        public void ShouldReadFullInstanceAnnotationsOnInlineEntryInsideFeedRequestInStreamingModeODataSimplified()
        {
            // cover "@odata.bind"
            InlineEntryInstanceAnnotationTest(EntryInsideExpandedFeedRequestWithInstanceAnnotation, streaming: true, isResponse: false, odataSimplified: true);
        }

        #endregion

        private static string GetContentType(bool streaming)
        {
            return "application/json" + (streaming ? ";odata.streaming=true" : string.Empty);
        }

        private static bool IsStreaming(string contentType)
        {
            return contentType.Contains("streaming=true");
        }
    }
}

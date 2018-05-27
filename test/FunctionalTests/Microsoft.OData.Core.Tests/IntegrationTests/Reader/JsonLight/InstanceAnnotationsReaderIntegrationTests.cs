//---------------------------------------------------------------------
// <copyright file="InstanceAnnotationsReaderIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Edm;
using Microsoft.Test.OData.DependencyInjection;
using Xunit;

namespace Microsoft.OData.Tests.IntegrationTests.Reader.JsonLight
{
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

        [Fact]
        public void ShouldReadInstanceAnnotationsOnTopLevelFeedResponseInNonStreamingMode()
        {
            TopLevelFeedInstanceAnnotationTest(TopLevelFeedWithInstanceAnnotation, streaming: false, isResponse: true);
        }

        [Fact]
        public void ShouldNotReadInstanceAnnotationsOnTopLevelFeedResponseInNonStreamingMode()
        {
            TopLevelFeedInstanceAnnotationTest(TopLevelFeedWithInstanceAnnotation, streaming: false, isResponse: true, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        [Fact]
        public void ShouldReadInstanceAnnotationsOnTopLevelFeedRequestInNonStreamingMode()
        {
            TopLevelFeedInstanceAnnotationTest(TopLevelFeedWithInstanceAnnotation, streaming: false, isResponse: false);
        }

        [Fact]
        public void ShouldNotReadInstanceAnnotationsOnTopLevelFeedRequestInNonStreamingMode()
        {
            TopLevelFeedInstanceAnnotationTest(TopLevelFeedWithInstanceAnnotation, streaming: false, isResponse: false, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        [Fact]
        public void ShouldReadInstanceAnnotationsOnTopLevelFeedResponseInStreamingMode()
        {
            TopLevelFeedInstanceAnnotationTest(TopLevelFeedWithInstanceAnnotation, streaming: true, isResponse: true);
        }

        [Fact]
        public void ShouldNotReadInstanceAnnotationsOnTopLevelFeedResponseInStreamingMode()
        {
            TopLevelFeedInstanceAnnotationTest(TopLevelFeedWithInstanceAnnotation, streaming: true, isResponse: true, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        [Fact]
        public void ShouldReadInstanceAnnotationsOnTopLevelFeedRequestInStreamingMode()
        {
            TopLevelFeedInstanceAnnotationTest(TopLevelFeedWithInstanceAnnotation, streaming: true, isResponse: false);
        }

        [Fact]
        public void ShouldNotReadInstanceAnnotationsOnTopLevelFeedRequestInStreamingMode()
        {
            TopLevelFeedInstanceAnnotationTest(TopLevelFeedWithInstanceAnnotation, streaming: true, isResponse: false, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        internal static void TopLevelFeedInstanceAnnotationTest(string feedPayload, string contentType, bool isResponse, bool shouldReadAndValidateCustomInstanceAnnotations = true, bool enableReadingODataAnnotationWithoutPrefix = false)
        {
            ODataResourceSet feedFromReader = null;
            using (var messageReader = CreateODataMessageReader(feedPayload, contentType, isResponse, shouldReadAndValidateCustomInstanceAnnotations, enableReadingODataAnnotationWithoutPrefix))
            {
                var odataReader = messageReader.CreateODataResourceSetReader(EntitySet, EntityType);

                while (odataReader.Read())
                {
                    switch (odataReader.State)
                    {
                        case ODataReaderState.ResourceSetStart:
                            feedFromReader = (ODataResourceSet)odataReader.Item;
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

                        case ODataReaderState.ResourceSetEnd:
                            feedFromReader.Should().NotBeNull();
                            ValidateContainsAllExpectedInstanceAnnotations(feedFromReader.InstanceAnnotations, shouldReadAndValidateCustomInstanceAnnotations);

                            break;
                    }
                }
            }

            feedFromReader.Should().NotBeNull();
            ValidateContainsAllExpectedInstanceAnnotations(feedFromReader.InstanceAnnotations, shouldReadAndValidateCustomInstanceAnnotations);
        }

        private static void TopLevelFeedInstanceAnnotationTest(string feedPayload, bool streaming, bool isResponse, bool shouldReadAndValidateCustomInstanceAnnotations = true, bool enableReadingODataAnnotationWithoutPrefix = false)
        {
            TopLevelFeedInstanceAnnotationTest(feedPayload, GetContentType(streaming), isResponse, shouldReadAndValidateCustomInstanceAnnotations, enableReadingODataAnnotationWithoutPrefix);
        }

        private static ODataMessageReader CreateODataMessageReader(string payload, string contentType, bool isResponse, bool shouldReadAndValidateCustomInstanceAnnotations, bool enableReadingODataAnnotationWithoutPrefix = false)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
            var readerSettings = new ODataMessageReaderSettings { EnableMessageStreamDisposal = false };
            var container = ContainerBuilderHelper.BuildContainer(null);
            container.GetRequiredService<ODataSimplifiedOptions>().EnableReadingODataAnnotationWithoutPrefix =
                enableReadingODataAnnotationWithoutPrefix;
            ODataMessageReader messageReader;
            if (isResponse)
            {
                IODataResponseMessage responseMessage = new InMemoryMessage { StatusCode = 200, Stream = stream, Container = container };
                responseMessage.SetHeader("Content-Type", contentType);
                if (shouldReadAndValidateCustomInstanceAnnotations)
                {
                    responseMessage.PreferenceAppliedHeader().AnnotationFilter = "*";
                }

                messageReader = new ODataMessageReader(responseMessage, readerSettings, Model);
            }
            else
            {
                IODataRequestMessage requestMessage = new InMemoryMessage { Method = "GET", Stream = stream, Container = container };
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

        [Fact]
        public void ShouldReadInstanceAnnotationsOnTopLevelEntryResponseInNonStreamingMode()
        {
            TopLevelEntryInstanceAnnotationTest(TopLevelEntryResponseWithInstanceAnnotation, streaming: false, isResponse: true);
        }

        [Fact]
        public void ShouldReadInstanceAnnotationsOnSingletonResponseInNonStreamingMode()
        {
            TopLevelEntryInstanceAnnotationTestOfSingleton(SingletonResponseWithInstanceAnnotation, streaming: false, isResponse: true);
        }

        [Fact]
        public void ShouldNotReadInstanceAnnotationsOnTopLevelEntryResponseInNonStreamingMode()
        {
            TopLevelEntryInstanceAnnotationTest(TopLevelEntryResponseWithInstanceAnnotation, streaming: false, isResponse: true, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        [Fact]
        public void ShouldReadInstanceAnnotationsOnTopLevelEntryRequestInNonStreamingMode()
        {
            TopLevelEntryInstanceAnnotationTest(TopLevelEntryRequestWithInstanceAnnotation, streaming: false, isResponse: false);
        }

        [Fact]
        public void ShouldNotReadInstanceAnnotationsOnTopLevelEntryRequestInNonStreamingMode()
        {
            TopLevelEntryInstanceAnnotationTest(TopLevelEntryRequestWithInstanceAnnotation, streaming: false, isResponse: false, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        [Fact]
        public void ShouldReadInstanceAnnotationsOnTopLevelEntryResponseInStreamingMode()
        {
            TopLevelEntryInstanceAnnotationTest(TopLevelEntryResponseWithInstanceAnnotation, streaming: true, isResponse: true);
        }

        [Fact]
        public void ShouldNotReadInstanceAnnotationsOnTopLevelEntryResponseInStreamingMode()
        {
            TopLevelEntryInstanceAnnotationTest(TopLevelEntryResponseWithInstanceAnnotation, streaming: true, isResponse: true, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        [Fact]
        public void ShouldReadInstanceAnnotationsOnTopLevelEntryRequestInStreamingMode()
        {
            TopLevelEntryInstanceAnnotationTest(TopLevelEntryRequestWithInstanceAnnotation, streaming: true, isResponse: false);
        }

        [Fact]
        public void ShouldNotReadInstanceAnnotationsOnTopLevelEntryRequestInStreamingMode()
        {
            TopLevelEntryInstanceAnnotationTest(TopLevelEntryRequestWithInstanceAnnotation, streaming: true, isResponse: false, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        //Singleton annotation test
        [Fact]
        public void ShouldNotReadInstanceAnnotationsOnSingletonResponseInNonStreamingMode()
        {
            TopLevelEntryInstanceAnnotationTestOfSingleton(SingletonResponseWithInstanceAnnotation, streaming: false, isResponse: true, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        [Fact]
        public void ShouldReadInstanceAnnotationsOnSingletonRequestInNonStreamingMode()
        {
            TopLevelEntryInstanceAnnotationTestOfSingleton(SingletonRequestWithInstanceAnnotation, streaming: false, isResponse: false);
        }

        [Fact]
        public void ShouldNotReadInstanceAnnotationsOnSingletonRequestInNonStreamingMode()
        {
            TopLevelEntryInstanceAnnotationTestOfSingleton(SingletonRequestWithInstanceAnnotation, streaming: false, isResponse: false, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        [Fact]
        public void ShouldReadInstanceAnnotationsOnSingletonResponseInStreamingMode()
        {
            TopLevelEntryInstanceAnnotationTestOfSingleton(SingletonResponseWithInstanceAnnotation, streaming: true, isResponse: true);
        }

        [Fact]
        public void ShouldNotReadInstanceAnnotationsOnSingletonResponseInStreamingMode()
        {
            TopLevelEntryInstanceAnnotationTestOfSingleton(SingletonResponseWithInstanceAnnotation, streaming: true, isResponse: true, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        [Fact]
        public void ShouldReadInstanceAnnotationsOnSingletonRequestInStreamingMode()
        {
            TopLevelEntryInstanceAnnotationTestOfSingleton(SingletonRequestWithInstanceAnnotation, streaming: true, isResponse: false);
        }

        [Fact]
        public void ShouldNotReadInstanceAnnotationsOnSingletonRequestInStreamingMode()
        {
            TopLevelEntryInstanceAnnotationTestOfSingleton(SingletonRequestWithInstanceAnnotation, streaming: true, isResponse: false, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        internal static void TopLevelEntryInstanceAnnotationTest(string payload, string contentType, bool isSingleton, bool isResponse, bool shouldReadAndValidateCustomInstanceAnnotations = true)
        {
            ODataResource entryFromReader = null;
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

                var odataReader = messageReader.CreateODataResourceReader(navigationSource, EntityType);

                while (odataReader.Read())
                {
                    switch (odataReader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            entryFromReader = (ODataResource)odataReader.Item;
                            ValidateContainsAllExpectedInstanceAnnotationsBeforeStateChange(entryFromReader.InstanceAnnotations, shouldReadAndValidateCustomInstanceAnnotations);
                            break;

                        case ODataReaderState.ResourceEnd:
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

        [Fact]
        public void ShouldReadInstanceAnnotationsOnInlineEntryResponseInStreamingMode()
        {
            InlineEntryInstanceAnnotationTest(ExpandedEntryResponseWithInstanceAnnotation, streaming: true, isResponse: true);
        }

        [Fact]
        public void ShouldNotReadInstanceAnnotationsOnInlineEntryResponseInStreamingMode()
        {
            InlineEntryInstanceAnnotationTest(ExpandedEntryResponseWithInstanceAnnotation, streaming: true, isResponse: true, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        [Fact]
        public void ShouldReadInstanceAnnotationsOnInlineEntryRequestInStreamingMode()
        {
            InlineEntryInstanceAnnotationTest(ExpandedEntryRequestWithInstanceAnnotation, streaming: true, isResponse: false);
        }

        [Fact]
        public void ShouldNotReadInstanceAnnotationsOnInlineEntryRequestInStreamingMode()
        {
            InlineEntryInstanceAnnotationTest(ExpandedEntryRequestWithInstanceAnnotation, streaming: true, isResponse: false, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        [Fact]
        public void ShouldReadInstanceAnnotationsOnInlineEntryResponseInNonStreamingMode()
        {
            InlineEntryInstanceAnnotationTest(ExpandedEntryResponseWithInstanceAnnotation, streaming: false, isResponse: true);
        }

        [Fact]
        public void ShouldNotReadInstanceAnnotationsOnInlineEntryResponseInNonStreamingMode()
        {
            InlineEntryInstanceAnnotationTest(ExpandedEntryResponseWithInstanceAnnotation, streaming: false, isResponse: true, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        [Fact]
        public void ShouldReadInstanceAnnotationsOnInlineEntryRequestInNonStreamingMode()
        {
            InlineEntryInstanceAnnotationTest(ExpandedEntryRequestWithInstanceAnnotation, streaming: false, isResponse: false);
        }

        [Fact]
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

        [Fact]
        public void ShouldReadInstanceAnnotationsOnInlineEntryInsideFeedResponseInStreamingMode()
        {
            InlineEntryInstanceAnnotationTest(EntryInsideExpandedFeedResponseWithInstanceAnnotation, streaming: true, isResponse: true);
        }

        [Fact]
        public void ShouldNotReadInstanceAnnotationsOnInlineEntryInsideFeedResponseInStreamingMode()
        {
            InlineEntryInstanceAnnotationTest(EntryInsideExpandedFeedResponseWithInstanceAnnotation, streaming: true, isResponse: true, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        [Fact]
        public void ShouldReadInstanceAnnotationsOnInlineEntryInsideFeedRequestInStreamingMode()
        {
            InlineEntryInstanceAnnotationTest(EntryInsideExpandedFeedRequestWithInstanceAnnotation, streaming: true, isResponse: false);
        }

        [Fact]
        public void ShouldNotReadInstanceAnnotationsOnInlineEntryInsideFeedRequestInStreamingMode()
        {
            InlineEntryInstanceAnnotationTest(EntryInsideExpandedFeedRequestWithInstanceAnnotation, streaming: true, isResponse: false, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        [Fact]
        public void ShouldReadInstanceAnnotationsOnInlineEntryInsideFeedResponseInNonStreamingMode()
        {
            InlineEntryInstanceAnnotationTest(EntryInsideExpandedFeedResponseWithInstanceAnnotation, streaming: false, isResponse: true);
        }

        [Fact]
        public void ShouldNotReadInstanceAnnotationsOnInlineEntryInsideFeedResponseInNonStreamingMode()
        {
            InlineEntryInstanceAnnotationTest(EntryInsideExpandedFeedResponseWithInstanceAnnotation, streaming: false, isResponse: true, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        [Fact]
        public void ShouldReadInstanceAnnotationsOnInlineEntryInsideFeedRequestInNonStreamingMode()
        {
            InlineEntryInstanceAnnotationTest(EntryInsideExpandedFeedRequestWithInstanceAnnotation, streaming: false, isResponse: false);
        }

        [Fact]
        public void ShouldNotReadInstanceAnnotationsOnInlineEntryInsideFeedRequestInNonStreamingMode()
        {
            InlineEntryInstanceAnnotationTest(EntryInsideExpandedFeedRequestWithInstanceAnnotation, streaming: false, isResponse: false, shouldReadAndValidateCustomInstanceAnnotations: false);
        }

        internal static void InlineEntryInstanceAnnotationTest(string payload, string contentType, bool isResponse, bool shouldReadAndValidateCustomInstanceAnnotations = true, bool enableReadingODataAnnotationWithoutPrefix = false)
        {
            ODataResource entryFromReader = null;
            int depth = 0;
            using (var messageReader = CreateODataMessageReader(payload, contentType, isResponse, shouldReadAndValidateCustomInstanceAnnotations, enableReadingODataAnnotationWithoutPrefix))
            {
                var odataReader = messageReader.CreateODataResourceReader(EntitySet, EntityType);

                while (odataReader.Read())
                {
                    switch (odataReader.State)
                    {
                        case ODataReaderState.NestedResourceInfoStart:
                            depth++;
                            break;

                        case ODataReaderState.NestedResourceInfoEnd:
                            depth--;
                            break;

                        case ODataReaderState.ResourceStart:
                            if (depth == 1)
                            {
                                entryFromReader = (ODataResource)odataReader.Item;
                                ValidateContainsAllExpectedInstanceAnnotationsBeforeStateChange(entryFromReader.InstanceAnnotations, shouldReadAndValidateCustomInstanceAnnotations);
                            }
                            else
                            {
                                ((ODataResource)odataReader.Item).InstanceAnnotations.Should().HaveCount(0);
                            }

                            break;

                        case ODataReaderState.ResourceEnd:
                            if (depth == 1)
                            {
                                entryFromReader.Should().NotBeNull();
                                ValidateContainsAllExpectedInstanceAnnotations(entryFromReader.InstanceAnnotations, shouldReadAndValidateCustomInstanceAnnotations);
                            }
                            else
                            {
                                ((ODataResource)odataReader.Item).InstanceAnnotations.Should().HaveCount(0);
                            }

                            break;
                    }
                }
            }

            entryFromReader.Should().NotBeNull();
            ValidateContainsAllExpectedInstanceAnnotations(entryFromReader.InstanceAnnotations, shouldReadAndValidateCustomInstanceAnnotations);
        }

        private static void InlineEntryInstanceAnnotationTest(string payload, bool streaming, bool isResponse, bool shouldReadAndValidateCustomInstanceAnnotations = true, bool enableReadingODataAnnotationWithoutPrefix = false)
        {
            InlineEntryInstanceAnnotationTest(payload, GetContentType(streaming), isResponse, shouldReadAndValidateCustomInstanceAnnotations, enableReadingODataAnnotationWithoutPrefix);
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

        [Fact]
        public void ShouldReadSimplifiedInstanceAnnotationsOnTopLevelFeedResponseInNonStreamingModeODataSimplified()
        {
            // cover "@context"
            TopLevelFeedInstanceAnnotationTest(TopLevelFeedWithSimplifiedInstanceAnnotation, streaming: false, isResponse: true, enableReadingODataAnnotationWithoutPrefix: true);
        }

        [Fact]
        public void ShouldReadFullInstanceAnnotationsOnTopLevelFeedResponseInNonStreamingModeODataSimplified()
        {
            // cover "@odata.context"
            TopLevelFeedInstanceAnnotationTest(TopLevelFeedWithInstanceAnnotation, streaming: false, isResponse: true, enableReadingODataAnnotationWithoutPrefix: true);
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

        [Fact]
        public void ShouldReadSimplifiedInstanceAnnotationsOnInlineEntryResponseInStreamingModeODataSimplified()
        {
            // cover "@navigationLink"
            InlineEntryInstanceAnnotationTest(ExpandedEntryResponseWithSimplifiedInstanceAnnotation, streaming: true, isResponse: true, enableReadingODataAnnotationWithoutPrefix: true);
        }

        [Fact]
        public void ShouldReadFullInstanceAnnotationsOnInlineEntryResponseInStreamingModeODataSimplified()
        {
            // cover "@odata.navigationLink"
            InlineEntryInstanceAnnotationTest(ExpandedEntryResponseWithInstanceAnnotation, streaming: true, isResponse: true, enableReadingODataAnnotationWithoutPrefix: true);
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

        [Fact]
        public void ShouldReadSimplifiedInstanceAnnotationsOnInlineEntryInsideFeedRequestInStreamingModeODataSimplified()
        {
            // cover "@bind"
            InlineEntryInstanceAnnotationTest(EntryInsideExpandedFeedRequestWithSimplifiedInstanceAnnotation, streaming: true, isResponse: false, enableReadingODataAnnotationWithoutPrefix: true);
        }

        [Fact]
        public void ShouldReadFullInstanceAnnotationsOnInlineEntryInsideFeedRequestInStreamingModeODataSimplified()
        {
            // cover "@odata.bind"
            InlineEntryInstanceAnnotationTest(EntryInsideExpandedFeedRequestWithInstanceAnnotation, streaming: true, isResponse: false, enableReadingODataAnnotationWithoutPrefix: true);
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

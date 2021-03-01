//---------------------------------------------------------------------
// <copyright file="InstanceAnnotationWriterIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using Microsoft.OData.Edm;
using Microsoft.Test.OData.DependencyInjection;
using Xunit;

namespace Microsoft.OData.Tests.IntegrationTests.Writer
{
    public class InstanceAnnotationWriterIntegrationTests
    {
        private static readonly ODataPrimitiveValue PrimitiveValue1 = new ODataPrimitiveValue(123);
        private static readonly ODataCollectionValue PrimitiveCollectionValue = new ODataCollectionValue { Items = new[] { "StringValue1", "StringValue2" }, TypeName = "Collection(String)" };

        private static readonly EdmModel Model;
        private static readonly EdmEntitySet EntitySet;
        private static readonly EdmEntityType EntityType;
        private static readonly EdmComplexType ComplexType;
        private static readonly EdmSingleton Singleton;

        private static readonly Uri tempUri = new Uri("http://tempuri.org");

        static InstanceAnnotationWriterIntegrationTests()
        {
            Model = new EdmModel();
            EntityType = new EdmEntityType("TestNamespace", "TestEntityType");
            Model.AddElement(EntityType);

            var keyProperty = new EdmStructuralProperty(EntityType, "ID", EdmCoreModel.Instance.GetInt32(false));
            EntityType.AddKeys(new IEdmStructuralProperty[] { keyProperty });
            EntityType.AddProperty(keyProperty);
            var resourceNavigationProperty = EntityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "ResourceNavigationProperty", Target = EntityType, TargetMultiplicity = EdmMultiplicity.ZeroOrOne });
            var resourceSetNavigationProperty = EntityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "ResourceSetNavigationProperty", Target = EntityType, TargetMultiplicity = EdmMultiplicity.Many });

            ComplexType = new EdmComplexType("TestNamespace", "Address");
            Model.AddElement(ComplexType);

            var defaultContainer = new EdmEntityContainer("TestNamespace", "DefaultContainer");
            Model.AddElement(defaultContainer);
            EntitySet = new EdmEntitySet(defaultContainer, "TestEntitySet", EntityType);
            EntitySet.AddNavigationTarget(resourceNavigationProperty, EntitySet);
            EntitySet.AddNavigationTarget(resourceSetNavigationProperty, EntitySet);
            defaultContainer.AddElement(EntitySet);
            Singleton = new EdmSingleton(defaultContainer, "TestSingleton", EntityType);
            Singleton.AddNavigationTarget(resourceNavigationProperty, EntitySet);
            Singleton.AddNavigationTarget(resourceSetNavigationProperty, EntitySet);
            defaultContainer.AddElement(Singleton);
        }

        #region Writing instance annotations on top level feed

        [Fact]
        public void WriteAnnotationAtStartOnTopLevelFeedAsResponseInJsonLight()
        {
            string expectedPayload =
            "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#TestEntitySet\"," +
                "\"@odata.count\":2," +
                "\"@odata.nextLink\":\"http://tempuri.org/\"," +
                "\"@Custom.StartAnnotation\":123," +
                "\"value\":[" +
                 "]" +
            "}";

            WriteAnnotationAtStartOnTopLevelFeed(expectedPayload, ODataFormat.Json, 2, tempUri, request: false);
        }

        [Fact]
        public void WriteDeltaLinkAtStartOnTopLevelFeedAsResponseInJsonLight()
        {
            string expectedPayload =
            "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#TestEntitySet\"," +
                "\"@odata.count\":2," +
                "\"@odata.deltaLink\":\"http://tempuri.org/\"," +
                "\"@Custom.StartAnnotation\":123," +
                "\"value\":[" +
                 "]" +
            "}";

            WriteAnnotationAtStartOnTopLevelFeed(expectedPayload, ODataFormat.Json, 2, null, false, tempUri);
        }

        [Fact]
        public void WriteAnnotationAtStartOnTopLevelFeedAsRequestInJsonLight()
        {
            string expectedPayload =
            "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#TestEntitySet\"," +
                "\"@Custom.StartAnnotation\":123," +
                "\"value\":[" +
                "]" +
            "}";

            WriteAnnotationAtStartOnTopLevelFeed(expectedPayload, ODataFormat.Json, null, null, request: true);
        }

        private void WriteAnnotationAtStartOnTopLevelFeed(string expectedPayload, ODataFormat format, long? count, Uri nextLink, bool request, Uri deltaLink = null, bool enableWritingODataAnnotationWithoutPrefix = false)
        {
            Action<ODataWriter> action = (odataWriter) =>
            {
                var feedToWrite = new ODataResourceSet { Id = new Uri("urn:feedId") };
                feedToWrite.Count = count;
                feedToWrite.NextPageLink = nextLink;
                feedToWrite.DeltaLink = deltaLink;
                feedToWrite.InstanceAnnotations.Add(new ODataInstanceAnnotation("Custom.StartAnnotation", PrimitiveValue1));
                odataWriter.WriteStart(feedToWrite);
                odataWriter.WriteEnd();
            };

            WriteAnnotationsAndValidatePayload(action, EntitySet, format, expectedPayload, request, createFeedWriter: true, enableWritingODataAnnotationWithoutPrefix: enableWritingODataAnnotationWithoutPrefix);
        }

        [Fact]
        public void WriteAnnotationAtEndOnTopLevelFeedForRequestInJsonLight()
        {
            string expectedPayload =
            "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#TestEntitySet\"," +
                "\"value\":[" +
                "]," +
                "\"@Custom.EndAnnotation\":123" +
            "}";

            WriteAnnotationAtEndOnTopLevelFeed(expectedPayload, ODataFormat.Json, null, null, request: true);
        }

        [Fact]
        public void WriteAnnotationAtEndOnTopLevelFeedForResponseInJsonLight()
        {
            string expectedPayload =
            "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#TestEntitySet\"," +
                "\"value\":[" +
                "]," +
                 "\"@Custom.EndAnnotation\":123," +
                "\"@odata.nextLink\":\"http://tempuri.org/\"" +
           "}";

            WriteAnnotationAtEndOnTopLevelFeed(expectedPayload, ODataFormat.Json, null, tempUri, request: false);
        }

        private void WriteAnnotationAtEndOnTopLevelFeed(string expectedPayload, ODataFormat format, long? count, Uri nextLink, bool request)
        {
            Action<ODataWriter> action = (odataWriter) =>
            {
                var feedToWrite = new ODataResourceSet { Id = new Uri("urn:feedId") };
                odataWriter.WriteStart(feedToWrite);
                feedToWrite.InstanceAnnotations.Add(new ODataInstanceAnnotation("Custom.EndAnnotation", PrimitiveValue1));
                feedToWrite.Count = count;
                feedToWrite.NextPageLink = nextLink;
                odataWriter.WriteEnd();
            };

            WriteAnnotationsAndValidatePayload(action, EntitySet, format, expectedPayload, request, createFeedWriter: true);
        }

        #endregion Writing instance annotations on top level feed

        #region Writing instance annotations on top level entry

        [Fact]
        public void WriteAnnotationAtEndOnTopLevelEntryInJsonLight()
        {
            string expectedPayload =
            "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#TestEntitySet/$entity\"," +
                "\"ID\":1," +
                "\"@odata.editLink\":\"http://tempuri.org/\"," +
                "\"Custom.PrimitiveCollectionAnnotation@odata.type\":\"#Collection(String)\"," +
                "\"@Custom.PrimitiveCollectionAnnotation\":[\"StringValue1\",\"StringValue2\"]" +
            "}";

            this.WriteAnnotationAtEndOnTopLevelEntry(EntitySet, ODataFormat.Json, expectedPayload);
        }

        [Fact]
        public void WriteAnnotationAtEndOnSingletonInJsonLight()
        {
            string expectedPayload =
            "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#TestSingleton\"," +
                "\"ID\":1," +
                "\"@odata.editLink\":\"http://tempuri.org/\"," +
                "\"Custom.PrimitiveCollectionAnnotation@odata.type\":\"#Collection(String)\"," +
                "\"@Custom.PrimitiveCollectionAnnotation\":[\"StringValue1\",\"StringValue2\"]" +
            "}";

            this.WriteAnnotationAtEndOnTopLevelEntry(Singleton, ODataFormat.Json, expectedPayload);
        }

        private void WriteAnnotationAtEndOnTopLevelEntry(IEdmNavigationSource navigationSource, ODataFormat format, string expectedPayload)
        {
            Action<ODataWriter> action = (odataWriter) =>
            {
                var entryToWrite = new ODataResource { Properties = new[] { new ODataProperty { Name = "ID", Value = 1 } } };
                odataWriter.WriteStart(entryToWrite);
                entryToWrite.InstanceAnnotations.Add(new ODataInstanceAnnotation("Custom.PrimitiveCollectionAnnotation", PrimitiveCollectionValue));
                var editLinkUri = tempUri;
                entryToWrite.EditLink = editLinkUri;
                odataWriter.WriteEnd();
            };

            this.WriteAnnotationsAndValidatePayload(action, navigationSource, format, expectedPayload, request: false, createFeedWriter: false);
            this.WriteAnnotationsAndValidatePayload(action, navigationSource, format, expectedPayload, request: true, createFeedWriter: false);
        }

        [Fact]
        public void WriteAnnotationAtStartOnTopLevelEntryInJsonLight()
        {
            string expectedPayload =
            "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#TestEntitySet/$entity\"," +
                "\"Custom.PrimitiveCollectionAnnotation@odata.type\":\"#Collection(String)\"," +
                "\"@Custom.PrimitiveCollectionAnnotation\":[\"StringValue1\",\"StringValue2\"]," +
                "\"ID\":1," +
                "\"@odata.editLink\":\"http://tempuri.org/\"" +
            "}";

            this.WriteAnnotationAtStartOnTopLevelEntry(EntitySet, ODataFormat.Json, expectedPayload);
        }

        [Fact]
        public void WriteAnnotationAtStartOnSingletonInJsonLight()
        {
            string expectedPayload =
            "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#TestSingleton\"," +
                "\"Custom.PrimitiveCollectionAnnotation@odata.type\":\"#Collection(String)\"," +
                "\"@Custom.PrimitiveCollectionAnnotation\":[\"StringValue1\",\"StringValue2\"]," +
                "\"ID\":1," +
                "\"@odata.editLink\":\"http://tempuri.org/\"" +
            "}";

            this.WriteAnnotationAtStartOnTopLevelEntry(Singleton, ODataFormat.Json, expectedPayload);
        }

        private void WriteAnnotationAtStartOnTopLevelEntry(IEdmNavigationSource navigationSource, ODataFormat format, string expectedPayload)
        {
            Action<ODataWriter> action = (odataWriter) =>
            {
                var entryToWrite = new ODataResource { Properties = new[] { new ODataProperty { Name = "ID", Value = 1 } } };
                entryToWrite.InstanceAnnotations.Add(new ODataInstanceAnnotation("Custom.PrimitiveCollectionAnnotation", PrimitiveCollectionValue));
                odataWriter.WriteStart(entryToWrite);
                var editLinkUri = tempUri;
                entryToWrite.EditLink = editLinkUri;
                odataWriter.WriteEnd();
            };

            this.WriteAnnotationsAndValidatePayload(action, navigationSource, format, expectedPayload, request: false, createFeedWriter: false);
            this.WriteAnnotationsAndValidatePayload(action, navigationSource, format, expectedPayload, request: true, createFeedWriter: false);
        }

        #endregion Writing instance annotations on top level entry

        #region Writing instance annotations on expanded entry in expanded feed

        [Fact]
        public void WriteAnnotationOnExpandedEntryInExpandedFeedInJsonLight()
        {
            string expectedPayload =
            "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#TestEntitySet/$entity\"," +
                 "\"ID\":1," +
                "\"ResourceSetNavigationProperty\":[" +
                    "{" +
                        "\"@odata.etag\":\"ETag\"," +
                        "\"@Custom.StartAnnotation\":123," +
                        "\"ID\":1," +
                        "\"@Custom.EndAnnotation\":123" +
                    "}]" +
            "}";

            this.WriteAnnotationOnExpandedEntryInExpandedFeed(EntitySet, expectedPayload, ODataFormat.Json);
        }

        [Fact]
        public void WriteAnnotationOnExpandedEntryInExpandedFeedOfSingletonInJsonLight()
        {
            string expectedPayload =
            "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#TestSingleton\"," +
                 "\"ID\":1," +
                "\"ResourceSetNavigationProperty\":[" +
                    "{" +
                        "\"@odata.etag\":\"ETag\"," +
                        "\"@Custom.StartAnnotation\":123," +
                        "\"ID\":1," +
                        "\"@Custom.EndAnnotation\":123" +
                    "}]" +
            "}";

            this.WriteAnnotationOnExpandedEntryInExpandedFeed(Singleton, expectedPayload, ODataFormat.Json);
        }

        private void WriteAnnotationOnExpandedEntryInExpandedFeed(IEdmNavigationSource navigationSource, string expectedPayload, ODataFormat format)
        {
            Action<ODataWriter> action = (odataWriter) =>
            {
                var entryToWrite = new ODataResource { Properties = new[] { new ODataProperty { Name = "ID", Value = 1 } } };
                odataWriter.WriteStart(entryToWrite);

                ODataNestedResourceInfo navLink = new ODataNestedResourceInfo { Name = "ResourceSetNavigationProperty", IsCollection = true };

                odataWriter.WriteStart(navLink);

                var feedToWrite = new ODataResourceSet { Id = new Uri("urn:feedId") };
                odataWriter.WriteStart(feedToWrite);


                entryToWrite = new ODataResource { Properties = new[] { new ODataProperty { Name = "ID", Value = 1 } } };
                entryToWrite.InstanceAnnotations.Add(new ODataInstanceAnnotation("Custom.StartAnnotation", PrimitiveValue1));
                entryToWrite.ETag = "ETag";
                odataWriter.WriteStart(entryToWrite);
                entryToWrite.InstanceAnnotations.Add(new ODataInstanceAnnotation("Custom.EndAnnotation", PrimitiveValue1));
                odataWriter.WriteEnd();

                odataWriter.WriteEnd();

                odataWriter.WriteEnd();
                odataWriter.WriteEnd();
            };

            this.WriteAnnotationsAndValidatePayload(action, navigationSource, format, expectedPayload, request: false, createFeedWriter: false);
            this.WriteAnnotationsAndValidatePayload(action, navigationSource, format, expectedPayload, request: true, createFeedWriter: false);
        }

        [Fact]
        public void WriteAnnotationOnMultipleExpandedEntriesInExpandedFeedInJsonLight()
        {
            string expectedPayload =
            "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#TestEntitySet/$entity\"," +
                 "\"ID\":1," +
                "\"ResourceSetNavigationProperty\":[" +
                    "{" +
                        "\"@Custom.StartAnnotation\":123," +
                        "\"ID\":1," +
                        "\"@odata.editLink\":\"http://tempuri.org/\"," +
                        "\"@Custom.EndAnnotation\":123" +
                    "}," +
                    "{" +
                        "\"@Custom.StartAnnotation2\":123," +
                        "\"ID\":1," +
                        "\"@odata.readLink\":\"http://tempuri.org/\"," +
                        "\"@Custom.EndAnnotation2\":123" +
                    "}" +
                 "]" +
            "}";

            this.WriteAnnotationOnMultipleExpandedEntriesInExpandedFeed(EntitySet, expectedPayload, ODataFormat.Json);
        }

        [Fact]
        public void WriteAnnotationOnMultipleExpandedEntriesInExpandedFeedOfSingletonInJsonLight()
        {
            string expectedPayload =
            "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#TestSingleton\"," +
                 "\"ID\":1," +
                "\"ResourceSetNavigationProperty\":[" +
                    "{" +
                        "\"@Custom.StartAnnotation\":123," +
                        "\"ID\":1," +
                        "\"@odata.editLink\":\"http://tempuri.org/\"," +
                        "\"@Custom.EndAnnotation\":123" +
                    "}," +
                    "{" +
                        "\"@Custom.StartAnnotation2\":123," +
                        "\"ID\":1," +
                        "\"@odata.readLink\":\"http://tempuri.org/\"," +
                        "\"@Custom.EndAnnotation2\":123" +
                    "}" +
                 "]" +
            "}";

            this.WriteAnnotationOnMultipleExpandedEntriesInExpandedFeed(Singleton, expectedPayload, ODataFormat.Json);
        }

        private void WriteAnnotationOnMultipleExpandedEntriesInExpandedFeed(IEdmNavigationSource navigationSource, string expectedPayload, ODataFormat format)
        {
            Action<ODataWriter> action = (odataWriter) =>
            {
                var entryToWrite = new ODataResource { Properties = new[] { new ODataProperty { Name = "ID", Value = 1 } } };
                odataWriter.WriteStart(entryToWrite);

                ODataNestedResourceInfo navLink = new ODataNestedResourceInfo { Name = "ResourceSetNavigationProperty", IsCollection = true };

                odataWriter.WriteStart(navLink);

                var feedToWrite = new ODataResourceSet { Id = new Uri("urn:feedId") };
                odataWriter.WriteStart(feedToWrite);

                entryToWrite = new ODataResource { Properties = new[] { new ODataProperty { Name = "ID", Value = 1 } } };
                entryToWrite.InstanceAnnotations.Add(new ODataInstanceAnnotation("Custom.StartAnnotation", PrimitiveValue1));
                odataWriter.WriteStart(entryToWrite);
                entryToWrite.EditLink = tempUri;
                entryToWrite.InstanceAnnotations.Add(new ODataInstanceAnnotation("Custom.EndAnnotation", PrimitiveValue1));
                odataWriter.WriteEnd();

                entryToWrite = new ODataResource { Properties = new[] { new ODataProperty { Name = "ID", Value = 1 } } };
                entryToWrite.InstanceAnnotations.Add(new ODataInstanceAnnotation("Custom.StartAnnotation2", PrimitiveValue1));
                odataWriter.WriteStart(entryToWrite);
                entryToWrite.ReadLink = tempUri;
                entryToWrite.InstanceAnnotations.Add(new ODataInstanceAnnotation("Custom.EndAnnotation2", PrimitiveValue1));
                odataWriter.WriteEnd();

                odataWriter.WriteEnd();

                odataWriter.WriteEnd();
                odataWriter.WriteEnd();
            };

            this.WriteAnnotationsAndValidatePayload(action, navigationSource, format, expectedPayload, request: false, createFeedWriter: false);
            this.WriteAnnotationsAndValidatePayload(action, navigationSource, format, expectedPayload, request: true, createFeedWriter: false);
        }

        #endregion Writing instance annotations on expanded entry in expanded feed

        #region Writing instance annotations on expanded entry not in expanded feed

        [Fact]
        public void WriteAnnotationOnExpandedEntriesNotInExpandedFeedInJsonLight()
        {
            string expectedPayload =
            "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#TestEntitySet/$entity\"," +
                "\"ID\":1," +
                "\"ResourceNavigationProperty\":{" +
                    "\"@Custom.StartAnnotation\":123," +
                    "\"ID\":1," +
                    "\"@odata.editLink\":\"http://tempuri.org/\"," +
                    "\"@Custom.EndAnnotation\":123" +
                "}" +
            "}";

            this.WriteAnnotationOnExpandedEntriesNotInExpandedFeed(EntitySet, ODataFormat.Json, expectedPayload);
        }

        [Fact]
        public void WriteAnnotationOnSingletonNotInExpandedFeedInJsonLight()
        {
            string expectedPayload =
            "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#TestSingleton\"," +
                "\"ID\":1," +
                "\"ResourceNavigationProperty\":{" +
                    "\"@Custom.StartAnnotation\":123," +
                    "\"ID\":1," +
                    "\"@odata.editLink\":\"http://tempuri.org/\"," +
                    "\"@Custom.EndAnnotation\":123" +
                "}" +
            "}";

            this.WriteAnnotationOnExpandedEntriesNotInExpandedFeed(Singleton, ODataFormat.Json, expectedPayload);
        }

        private void WriteAnnotationOnExpandedEntriesNotInExpandedFeed(IEdmNavigationSource navigationSource, ODataFormat format, string expectedPayload)
        {
            Action<ODataWriter> action = (odataWriter) =>
            {
                var entryToWrite = new ODataResource { Properties = new[] { new ODataProperty { Name = "ID", Value = 1 } } };
                odataWriter.WriteStart(entryToWrite);

                ODataNestedResourceInfo navLink = new ODataNestedResourceInfo { Name = "ResourceNavigationProperty", IsCollection = false };
                odataWriter.WriteStart(navLink);

                entryToWrite = new ODataResource { Properties = new[] { new ODataProperty { Name = "ID", Value = 1 } } };
                entryToWrite.InstanceAnnotations.Add(new ODataInstanceAnnotation("Custom.StartAnnotation", PrimitiveValue1));
                odataWriter.WriteStart(entryToWrite);
                entryToWrite.EditLink = tempUri;
                entryToWrite.InstanceAnnotations.Add(new ODataInstanceAnnotation("Custom.EndAnnotation", PrimitiveValue1));
                odataWriter.WriteEnd();

                odataWriter.WriteEnd();
                odataWriter.WriteEnd();
            };

            this.WriteAnnotationsAndValidatePayload(action, navigationSource, format, expectedPayload, request: false, createFeedWriter: false);
            this.WriteAnnotationsAndValidatePayload(action, navigationSource, format, expectedPayload, request: true, createFeedWriter: false);
        }

        #endregion Writing instance annotations on expanded entry not in expanded feed

        #region Writing instance annotations on expanded feeds

        [Fact]
        public void WriteAnnotationAtStartExpandedFeedShouldFailInJsonLight()
        {
            this.WriteAnnotationAtStartExpandedFeedShouldFail(ODataFormat.Json);
        }

        private void WriteAnnotationAtStartExpandedFeedShouldFail(ODataFormat format)
        {
            Action<ODataWriter> action = (odataWriter) =>
            {
                var entryToWrite = new ODataResource { Properties = new[] { new ODataProperty { Name = "ID", Value = 1 } } };
                odataWriter.WriteStart(entryToWrite);

                ODataNestedResourceInfo navLink = new ODataNestedResourceInfo { Name = "ResourceSetNavigationProperty", IsCollection = true };

                odataWriter.WriteStart(navLink);

                var feedToWrite = new ODataResourceSet { Id = new Uri("urn:feedId") };
                feedToWrite.InstanceAnnotations.Add(new ODataInstanceAnnotation("custom.StartFeedAnnotation", PrimitiveValue1));

                odataWriter.WriteStart(feedToWrite);
            };

            Action testResponse = () => this.WriteAnnotationsAndValidatePayload(action, EntitySet, format, null, request: false, createFeedWriter: false);
            testResponse.Throws<ODataException>(Strings.ODataJsonLightWriter_InstanceAnnotationNotSupportedOnExpandedResourceSet);

            Action testRequest = () => this.WriteAnnotationsAndValidatePayload(action, EntitySet, format, null, request: true, createFeedWriter: false);
            testRequest.Throws<ODataException>(Strings.ODataJsonLightWriter_InstanceAnnotationNotSupportedOnExpandedResourceSet);
        }

        [Fact]
        public void WriteAnnotationAtEndExpandedFeedShouldFailInJsonLight()
        {
            this.WriteAnnotationAtEndExpandedFeedShouldFail(ODataFormat.Json);
        }

        private void WriteAnnotationAtEndExpandedFeedShouldFail(ODataFormat format)
        {
            Action<ODataWriter> action = (odataWriter) =>
            {
                var entryToWrite = new ODataResource { Properties = new[] { new ODataProperty { Name = "ID", Value = 1 } } };
                odataWriter.WriteStart(entryToWrite);

                ODataNestedResourceInfo navLink = new ODataNestedResourceInfo { Name = "ResourceSetNavigationProperty", Url = new Uri("http://service/navLink", UriKind.RelativeOrAbsolute), IsCollection = true };
                odataWriter.WriteStart(navLink);

                var feedToWrite = new ODataResourceSet { Id = new Uri("urn:feedId") };
                odataWriter.WriteStart(feedToWrite);

                feedToWrite.InstanceAnnotations.Add(new ODataInstanceAnnotation("custom.StartFeedAnnotation", PrimitiveValue1));
                odataWriter.WriteEnd();
            };

            Action testResponse = () => this.WriteAnnotationsAndValidatePayload(action, EntitySet, format, null, request: false, createFeedWriter: false);
            testResponse.Throws<ODataException>(Strings.ODataJsonLightWriter_InstanceAnnotationNotSupportedOnExpandedResourceSet);

            Action testResponseOfSingleton = () => this.WriteAnnotationsAndValidatePayload(action, Singleton, format, null, request: false, createFeedWriter: false);
            testResponseOfSingleton.Throws<ODataException>(Strings.ODataJsonLightWriter_InstanceAnnotationNotSupportedOnExpandedResourceSet);

            Action testRequest = () => this.WriteAnnotationsAndValidatePayload(action, EntitySet, format, null, request: true, createFeedWriter: false);
            testRequest.Throws<ODataException>(Strings.ODataJsonLightWriter_InstanceAnnotationNotSupportedOnExpandedResourceSet);

            Action testRequestOfSingleton = () => this.WriteAnnotationsAndValidatePayload(action, Singleton, format, null, request: true, createFeedWriter: false);
            testRequestOfSingleton.Throws<ODataException>(Strings.ODataJsonLightWriter_InstanceAnnotationNotSupportedOnExpandedResourceSet);
        }

        #endregion Writing instance annotations on expanded feeds

        #region Write Delta Feed

        [Fact]
        public void WriteNextLinkAtStartInDeltaFeed()
        {
            string expectedPayload =
            "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#TestEntitySet/$delta\"," +
                "\"@odata.count\":2," +
                "\"@odata.nextLink\":\"http://tempuri.org/\"," +
                "\"@Custom.StartAnnotation\":123," +
                "\"value\":[" +
                 "]" +
            "}";

            WriteAnnotationAtStartInDeltaFeed(expectedPayload, 2, tempUri, null);
        }

        [Fact]
        public void WriteDeltaLinkAtStartInDeltaFeed()
        {
            string expectedPayload =
            "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#TestEntitySet/$delta\"," +
                "\"@odata.count\":2," +
                "\"@odata.deltaLink\":\"http://tempuri.org/\"," +
                "\"@Custom.StartAnnotation\":123," +
                "\"value\":[" +
                 "]" +
            "}";

            WriteAnnotationAtStartInDeltaFeed(expectedPayload, 2, null, tempUri);
        }

        [Fact]
        public void WriteNextLinkAtEndInDeltaFeed()
        {
            string expectedPayload =
            "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#TestEntitySet/$delta\"," +
                "\"value\":[]," +
                "\"@Custom.EndAnnotation\":123," +
                "\"@odata.nextLink\":\"http://tempuri.org/\"" +
            "}";

            WriteAnnotationAtEndInDeltaFeed(expectedPayload, null, tempUri, null);
        }

        [Fact]
        public void WriteDeltaLinkAtEndInDeltaFeed()
        {
            string expectedPayload =
            "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#TestEntitySet/$delta\"," +
                "\"value\":[]," +
                "\"@Custom.EndAnnotation\":123," +
                "\"@odata.deltaLink\":\"http://tempuri.org/\"" +
            "}";

            WriteAnnotationAtEndInDeltaFeed(expectedPayload, null, null, tempUri);
        }

        #endregion

        #region OData Simplified

        [Fact]
        public void WriteSimplifiedAnnotationAtStartOnTopLevelFeedAsResponseInJsonLight()
        {
            string expectedPayload =
            "{" +
                "\"@context\":\"http://www.example.com/$metadata#TestEntitySet\"," +
                "\"@count\":2," +
                "\"@nextLink\":\"http://tempuri.org/\"," +
                "\"@Custom.StartAnnotation\":123," +
                "\"value\":[" +
                 "]" +
            "}";

            WriteAnnotationAtStartOnTopLevelFeed(expectedPayload, ODataFormat.Json, 2, tempUri, request: false, enableWritingODataAnnotationWithoutPrefix: true);
        }

        [Fact]
        public void WriteSimplifiedDeltaLinkAtStartOnTopLevelFeedAsResponseInJsonLight()
        {
            string expectedPayload =
            "{" +
                "\"@context\":\"http://www.example.com/$metadata#TestEntitySet\"," +
                "\"@count\":2," +
                "\"@deltaLink\":\"http://tempuri.org/\"," +
                "\"@Custom.StartAnnotation\":123," +
                "\"value\":[" +
                 "]" +
            "}";

            WriteAnnotationAtStartOnTopLevelFeed(expectedPayload, ODataFormat.Json, 2, null, false, tempUri, enableWritingODataAnnotationWithoutPrefix: true);
        }

        #endregion

        #region Writing instance annotations on complex resource

        [Fact]
        public void WritingInstanceAnnotationInComplexValueShouldWrite()
        {
            Action<ODataWriter> action = (odataWriter) =>
            {
                var complexResource = new ODataResource { TypeName = "TestNamespace.Address" };
                complexResource.InstanceAnnotations.Add(new ODataInstanceAnnotation("Is.ReadOnly", new ODataPrimitiveValue(true)));
                odataWriter.WriteStart(complexResource);
                odataWriter.WriteEnd();
            };

            string expectedPayload = "{\"@odata.context\":\"http://www.example.com/$metadata#TestNamespace.Address\",\"@Is.ReadOnly\":true}";

            this.WriteAnnotationsAndValidatePayload(action, null, ODataFormat.Json, expectedPayload, request: false, createFeedWriter: false, resourceType: ComplexType);
            this.WriteAnnotationsAndValidatePayload(action, null, ODataFormat.Json, expectedPayload, request: true, createFeedWriter: false, resourceType: ComplexType);

        }

        [Fact]
        public void WritingMultipleInstanceAnnotationInComplexValueShouldWrite()
        {
            Action<ODataWriter> action = (odataWriter) =>
            {
                var complexResource = new ODataResource { TypeName = "TestNamespace.Address" };
                complexResource.InstanceAnnotations.Add(new ODataInstanceAnnotation("Annotation.1", new ODataPrimitiveValue(true)));
                complexResource.InstanceAnnotations.Add(new ODataInstanceAnnotation("Annotation.2", new ODataPrimitiveValue(123)));
                complexResource.InstanceAnnotations.Add(new ODataInstanceAnnotation("Annotation.3", new ODataPrimitiveValue("annotation")));
                odataWriter.WriteStart(complexResource);
                odataWriter.WriteEnd();
            };

            var expectedPayload = "{\"@odata.context\":\"http://www.example.com/$metadata#TestNamespace.Address\",\"@Annotation.1\":true,\"@Annotation.2\":123,\"@Annotation.3\":\"annotation\"}";
            this.WriteAnnotationsAndValidatePayload(action, null, ODataFormat.Json, expectedPayload, request: false, createFeedWriter: false, resourceType: ComplexType);
            this.WriteAnnotationsAndValidatePayload(action, null, ODataFormat.Json, expectedPayload, request: true, createFeedWriter: false, resourceType: ComplexType);
        }

        #endregion

        private void WriteAnnotationsAndValidatePayload(Action<ODataWriter> action, IEdmNavigationSource navigationSource, ODataFormat format, string expectedPayload, bool request, bool createFeedWriter, bool enableWritingODataAnnotationWithoutPrefix = false, IEdmStructuredType resourceType = null)
        {
            var writerSettings = new ODataMessageWriterSettings { EnableMessageStreamDisposal = false };
            writerSettings.SetContentType(format);
            writerSettings.SetServiceDocumentUri(new Uri("http://www.example.com/"));

            var container = ContainerBuilderHelper.BuildContainer(null);
            container.GetRequiredService<ODataSimplifiedOptions>().SetOmitODataPrefix(
                enableWritingODataAnnotationWithoutPrefix);

            MemoryStream stream = new MemoryStream();
            if (request)
            {

                IODataRequestMessage requestMessageToWrite = new InMemoryMessage
                {
                    Method = "GET",
                    Stream = stream,
                    Container = container
                };
                using (var messageWriter = new ODataMessageWriter(requestMessageToWrite, writerSettings, Model))
                {
                    ODataWriter odataWriter = (createFeedWriter && !(navigationSource is EdmSingleton))
                        ? messageWriter.CreateODataResourceSetWriter(navigationSource as EdmEntitySet, EntityType)
                        : messageWriter.CreateODataResourceWriter(navigationSource, resourceType ?? EntityType);
                    action(odataWriter);
                }
            }
            else
            {
                IODataResponseMessage responseMessageToWrite = new InMemoryMessage
                {
                    StatusCode = 200,
                    Stream = stream,
                    Container = container
                };
                responseMessageToWrite.PreferenceAppliedHeader().AnnotationFilter = "*";
                using (var messageWriter = new ODataMessageWriter(responseMessageToWrite, writerSettings, Model))
                {
                    ODataWriter odataWriter = (createFeedWriter && !(navigationSource is EdmSingleton))
                        ? messageWriter.CreateODataResourceSetWriter(navigationSource as EdmEntitySet, EntityType)
                        : messageWriter.CreateODataResourceWriter(navigationSource, resourceType ?? EntityType);
                    action(odataWriter);
                }
            }

            stream.Position = 0;
            string payload = (new StreamReader(stream)).ReadToEnd();

            Assert.Equal(expectedPayload, payload);
        }

        private void WriteAnnotationAtStartInDeltaFeed(string expectedPayload, long? count, Uri nextLink, Uri deltaLink)
        {
            Action<ODataWriter> action = (odataWriter) =>
            {
                var feedToWrite = new ODataDeltaResourceSet { Id = new Uri("urn:feedId") };
                feedToWrite.InstanceAnnotations.Add(new ODataInstanceAnnotation("Custom.StartAnnotation", PrimitiveValue1));
                feedToWrite.Count = count;
                feedToWrite.NextPageLink = nextLink;
                feedToWrite.DeltaLink = deltaLink;
                odataWriter.WriteStart(feedToWrite);
                odataWriter.WriteEnd();
            };

            WriteDeltaFeedAnnotationsAndValidatePayload(action, EntitySet, expectedPayload);
        }

        private void WriteAnnotationAtEndInDeltaFeed(string expectedPayload, long? count, Uri nextLink, Uri deltaLink)
        {
            Action<ODataWriter> action = (odataWriter) =>
            {
                var feedToWrite = new ODataDeltaResourceSet { Id = new Uri("urn:feedId") };
                odataWriter.WriteStart(feedToWrite);
                feedToWrite.InstanceAnnotations.Add(new ODataInstanceAnnotation("Custom.EndAnnotation", PrimitiveValue1));
                feedToWrite.Count = count;
                feedToWrite.NextPageLink = nextLink;
                feedToWrite.DeltaLink = deltaLink;
                odataWriter.WriteEnd();
            };

            WriteDeltaFeedAnnotationsAndValidatePayload(action, EntitySet, expectedPayload);
        }

        private void WriteDeltaFeedAnnotationsAndValidatePayload(Action<ODataWriter> action, IEdmEntitySet entitySet, string expectedPayload)
        {
            var writerSettings = new ODataMessageWriterSettings { EnableMessageStreamDisposal = false };
            writerSettings.SetServiceDocumentUri(new Uri("http://www.example.com/"));

            MemoryStream stream = new MemoryStream();

            IODataResponseMessage responseMessageToWrite = new InMemoryMessage { StatusCode = 200, Stream = stream };
            responseMessageToWrite.PreferenceAppliedHeader().AnnotationFilter = "*";
            using (var messageWriter = new ODataMessageWriter(responseMessageToWrite, writerSettings, Model))
            {
                ODataWriter odataDeltaWriter = messageWriter.CreateODataDeltaResourceSetWriter(entitySet, EntityType);
                action(odataDeltaWriter);
            }

            stream.Position = 0;
            string payload = (new StreamReader(stream)).ReadToEnd();

            Assert.Equal(expectedPayload, payload);
        }
    }
}

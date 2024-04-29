//---------------------------------------------------------------------
// <copyright file="FeedAnnotationIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Core.Tests.DependencyInjection;
using Microsoft.OData.Edm;
using Xunit;
using ErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.IntegrationTests.Reader.Json
{
    public class FeedAnnotationIntegrationTests : IDisposable
    {
        private ODataMessageReader messageReader = null;
        private EdmEntitySet entitySet;
        private EdmEntityType type;
        private IEdmModel model;

        public FeedAnnotationIntegrationTests()
        {
            EdmModel tmp = new EdmModel();
            this.type = new EdmEntityType("Namespace", "Type");
            this.type.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() { Target = this.type, TargetMultiplicity = EdmMultiplicity.Many, Name = "NavProp" });
            this.type.AddProperty(new EdmStructuralProperty(this.type, "PrimitiveProp", EdmCoreModel.Instance.GetInt32(false)));
            tmp.AddElement(this.type);
            EdmEntityContainer edmEntityContainer = new EdmEntityContainer("Namespace", "Container_sub");
            this.entitySet = edmEntityContainer.AddEntitySet("EntitySet", this.type);
            tmp.AddElement(edmEntityContainer);
            this.model = TestUtils.WrapReferencedModelsToMainModel("Namespace", "Container", tmp);
        }

        public void Dispose()
        {
            if (this.messageReader != null)
            {
                this.messageReader.Dispose();
            }
        }

        #region Top-level feeds

        [Fact]
        public void NextLinkComesBeforeTopLevelFeed()
        {
            foreach (bool isResponse in new[] { true, false })
            {
                const string feedText = @"
                ""@odata.nextLink"":""http://nextLink"",
                ""value"":[]";
                var feedReader = GetFeedReader(feedText, isResponse);
                feedReader.Read();
                Assert.Equal(ODataReaderState.ResourceSetStart, feedReader.State);
                ODataResourceSet set = feedReader.Item as ODataResourceSet;
                Assert.NotNull(set);
                Assert.Equal(new Uri("http://nextLink"), set.NextPageLink);

                feedReader.Read();
                Assert.Equal(ODataReaderState.ResourceSetEnd, feedReader.State);
                set = feedReader.Item as ODataResourceSet;
                Assert.NotNull(set);
                Assert.Equal(new Uri("http://nextLink"), set.NextPageLink);
            }
        }

        #region NextLinkComesAfterTopLevelFeed

        private void NextLinkComesAfterTopLevelFeedImplementation(string feedText, bool enableReadingODataAnnotationWithoutPrefix)
        {
            foreach (bool isResponse in new[] { true, false })
            {
                var feedReader = GetFeedReader(feedText, isResponse, enableReadingODataAnnotationWithoutPrefix);
                feedReader.Read();
                Assert.Equal(ODataReaderState.ResourceSetStart, feedReader.State);
                Assert.Null((feedReader.Item as ODataResourceSet).NextPageLink);

                feedReader.Read();
                Assert.Equal(ODataReaderState.ResourceSetEnd, feedReader.State);
                Assert.Equal(new Uri("http://nextLink"), (feedReader.Item as ODataResourceSet).NextPageLink);
            }
        }

        [Fact]
        public void NextLinkComesAfterTopLevelFeed()
        {
            const string feedText = @"
                ""value"":[],
                ""@odata.nextLink"":""http://nextLink""";
            NextLinkComesAfterTopLevelFeedImplementation(feedText, enableReadingODataAnnotationWithoutPrefix: false);
        }

        [Fact]
        public void SimplifiedNextLinkComesAfterTopLevelFeedODataSimplified()
        {
            // cover "@nextLink"
            const string feedText = @"
                ""value"":[],
                ""@nextLink"":""http://nextLink""";
            NextLinkComesAfterTopLevelFeedImplementation(feedText, enableReadingODataAnnotationWithoutPrefix: true);
        }

        [Fact]
        public void FullNextLinkComesAfterTopLevelFeedODataSimplified()
        {
            // cover "@odata.nextLink"
            const string feedText = @"
                ""value"":[],
                ""@odata.nextLink"":""http://nextLink""";
            NextLinkComesAfterTopLevelFeedImplementation(feedText, enableReadingODataAnnotationWithoutPrefix: true);
        }

        #endregion

        [Fact]
        public void DeltaLinkComesBeforeTopLevelFeed()
        {
            foreach (bool isResponse in new[] { true, false })
            {
                const string feedText = @"
                ""@odata.deltaLink"":""http://deltaLink"",
                ""value"":[]";
                var feedReader = GetFeedReader(feedText, isResponse);
                feedReader.Read();
                Assert.Equal(ODataReaderState.ResourceSetStart, feedReader.State);
                Assert.Equal(new Uri("http://deltaLink"), (feedReader.Item as ODataResourceSet).DeltaLink);

                feedReader.Read();
                Assert.Equal(ODataReaderState.ResourceSetEnd, feedReader.State);
                Assert.Equal(new Uri("http://deltaLink"), (feedReader.Item as ODataResourceSet).DeltaLink);
            }
        }

        [Fact]
        public void DeltaLinkComesAfterTopLevelFeed()
        {
            foreach (bool isResponse in new[] { true, false })
            {
                const string feedText = @"
                ""value"":[],
                ""@odata.deltaLink"":""http://deltaLink""";
                var feedReader = GetFeedReader(feedText, isResponse);
                feedReader.Read();
                Assert.Equal(ODataReaderState.ResourceSetStart, feedReader.State);
                Assert.Null((feedReader.Item as ODataResourceSet).DeltaLink);

                feedReader.Read();
                Assert.Equal(ODataReaderState.ResourceSetEnd, feedReader.State);
                Assert.Equal(new Uri("http://deltaLink"), (feedReader.Item as ODataResourceSet).DeltaLink);
            }
        }

        [Fact]
        public void NextLinkComesBeforeAndAfterTopLevelFeedShouldThrow()
        {
            foreach (bool isResponse in new[] { true, false })
            {
                const string feedText = @"
                ""@odata.nextLink"" : ""http://nextlink"",
                ""value"" : [],
                ""@odata.nextLink"" : ""http://nextLink2""";

                var entryReader = this.GetFeedReader(feedText, isResponse);
                entryReader.Read();
                Assert.Equal(ODataReaderState.ResourceSetStart, entryReader.State);
                Assert.Equal(new Uri("http://nextLink"), (entryReader.Item as ODataResourceSet).NextPageLink);
                Action read = () => entryReader.Read();
                read.Throws<ODataException>(ErrorStrings.DuplicateAnnotationNotAllowed("odata.nextLink"));
            }
        }

        [Fact]
        public void DeltaLinkComesBeforeAndAfterTopLevelFeedShouldThrow()
        {
            foreach (bool isResponse in new[] { true, false })
            {
                const string feedText = @"
                ""@odata.deltaLink"" : ""http://deltalink"",
                ""value"" : [],
                ""@odata.deltaLink"" : ""http://deltaLink2""";

                var entryReader = this.GetFeedReader(feedText, isResponse);
                entryReader.Read();
                Assert.Equal(ODataReaderState.ResourceSetStart, entryReader.State);
                Assert.Equal(new Uri("http://deltaLink"), (entryReader.Item as ODataResourceSet).DeltaLink);
                Action read = () => entryReader.Read();
                read.Throws<ODataException>(ErrorStrings.DuplicateAnnotationNotAllowed("odata.deltaLink"));
            }
        }

        #endregion Top-level feeds

        #region expanded feeds

        [Fact]
        public void CountComesBeforeInnerFeedOnResponse()
        {
            const string entryText = @"
                ""NavProp@odata.count"" : 0,
                ""NavProp"" : []";

            var entryReader = GetEntryReader(entryText, isResponse: true);
            entryReader.Read();
            Assert.Equal(ODataReaderState.ResourceStart, entryReader.State);
            entryReader.Read();
            Assert.Equal(ODataReaderState.NestedResourceInfoStart, entryReader.State);
            entryReader.Read();
            Assert.Equal(ODataReaderState.ResourceSetStart, entryReader.State);
            Assert.Equal(0, (entryReader.Item as ODataResourceSet).Count);
        }

        [Fact]
        public void CountComesAfterInnerFeedOnResponse()
        {
            const string entryText = @"
                ""NavProp"" : [],
                ""NavProp@odata.count"" : 0";

            var entryReader = GetEntryReader(entryText, isResponse: true);
            entryReader.Read();
            Assert.Equal(ODataReaderState.ResourceStart, entryReader.State);
            entryReader.Read();
            Assert.Equal(ODataReaderState.NestedResourceInfoStart, entryReader.State);
            entryReader.Read();
            Assert.Equal(ODataReaderState.ResourceSetStart, entryReader.State);
            Assert.Null((entryReader.Item as ODataResourceSet).Count);
            entryReader.Read();
            Assert.Equal(ODataReaderState.ResourceSetEnd, entryReader.State);
            Assert.Equal(0, (entryReader.Item as ODataResourceSet).Count);
        }

        [Fact]
        public void CountComesBeforeAndAfterInnerFeedShouldThrow()
        {
            const string entryText = @"
                ""NavProp@odata.count"" : 0,
                ""NavProp"" : [],
                ""NavProp@odata.count"" : 0";

            var entryReader = GetEntryReader(entryText, isResponse: true);
            entryReader.Read();
            Assert.Equal(ODataReaderState.ResourceStart, entryReader.State);
            entryReader.Read();
            Assert.Equal(ODataReaderState.NestedResourceInfoStart, entryReader.State);
            entryReader.Read();
            Assert.Equal(ODataReaderState.ResourceSetStart, entryReader.State);
            Assert.Equal(0, (entryReader.Item as ODataResourceSet).Count);
            Action read = () => entryReader.Read();
            read.Throws<ODataException>(ErrorStrings.ODataJsonResourceDeserializer_DuplicateNestedResourceSetAnnotation("odata.count", "NavProp"));
        }

        [Fact]
        public void NonZeroCountComesBeforeInnerFeedOnResponse()
        {
            const string entryText = @"
                ""NavProp@odata.count"" : 2,
                ""NavProp"" : []";

            var entryReader = GetEntryReader(entryText, isResponse: true);
            entryReader.Read();
            Assert.Equal(ODataReaderState.ResourceStart, entryReader.State);
            entryReader.Read();
            Assert.Equal(ODataReaderState.NestedResourceInfoStart, entryReader.State);
            entryReader.Read();
            Assert.Equal(ODataReaderState.ResourceSetStart, entryReader.State);
            Assert.Equal(2, (entryReader.Item as ODataResourceSet).Count);
        }

        [Fact]
        public void NonZeroCountComesAfterInnerFeedOnResponse()
        {
            const string entryText = @"
                ""NavProp"" : [],
                ""NavProp@odata.count"" : 2";

            var entryReader = GetEntryReader(entryText, isResponse: true);
            entryReader.Read();
            Assert.Equal(ODataReaderState.ResourceStart, entryReader.State);
            entryReader.Read();
            Assert.Equal(ODataReaderState.NestedResourceInfoStart, entryReader.State);
            entryReader.Read();
            Assert.Equal(ODataReaderState.ResourceSetStart, entryReader.State);
            Assert.Null((entryReader.Item as ODataResourceSet).Count);
            entryReader.Read();
            Assert.Equal(ODataReaderState.ResourceSetEnd, entryReader.State);
            Assert.Equal(2, (entryReader.Item as ODataResourceSet).Count);
        }

        #region NonZeroCountAndNextLinkComesAfterInnerFeedOnResponse

        private void NonZeroCountAndNextLinkComesAfterInnerFeedOnResponseImplementation(string entryText, bool enableReadingODataAnnotationWithoutPrefix)
        {
            var entryReader = GetEntryReader(entryText, isResponse: true, enableReadingODataAnnotationWithoutPrefix: enableReadingODataAnnotationWithoutPrefix);
            entryReader.Read();
            Assert.Equal(ODataReaderState.ResourceStart, entryReader.State);
            entryReader.Read();
            Assert.Equal(ODataReaderState.NestedResourceInfoStart, entryReader.State);
            entryReader.Read();
            Assert.Equal(ODataReaderState.ResourceSetStart, entryReader.State);
            Assert.Null((entryReader.Item as ODataResourceSet).Count);
            entryReader.Read();
            Assert.Equal(ODataReaderState.ResourceSetEnd, entryReader.State);
            Assert.Equal(2, (entryReader.Item as ODataResourceSet).Count);
            Assert.Equal(new Uri("http://nextLink"), (entryReader.Item as ODataResourceSet).NextPageLink);
        }

        [Fact]
        public void NonZeroCountAndNextLinkComesAfterInnerFeedOnResponse()
        {
            const string entryText = @"
                ""NavProp"" : [],
                ""NavProp@odata.count"" : 2,
                ""NavProp@odata.nextLink"" : ""http://nextLink""";
            NonZeroCountAndNextLinkComesAfterInnerFeedOnResponseImplementation(entryText, enableReadingODataAnnotationWithoutPrefix: false);
        }

        [Fact]
        public void NonZeroSimplifiedCountAndNextLinkComesAfterInnerFeedOnResponseODataSimplified()
        {
            // cover "prop@count" and "prop@nextLink"
            const string entryText = @"
                ""NavProp"" : [],
                ""NavProp@count"" : 2,
                ""NavProp@nextLink"" : ""http://nextLink""";
            NonZeroCountAndNextLinkComesAfterInnerFeedOnResponseImplementation(entryText, enableReadingODataAnnotationWithoutPrefix: true);
        }

        [Fact]
        public void NonZeroFullCountAndNextLinkComesAfterInnerFeedOnResponseODataSimplified()
        {
            // cover "prop@odata.count" and "prop@odata.nextLink"
            const string entryText = @"
                ""NavProp"" : [],
                ""NavProp@odata.count"" : 2,
                ""NavProp@odata.nextLink"" : ""http://nextLink""";
            NonZeroCountAndNextLinkComesAfterInnerFeedOnResponseImplementation(entryText, enableReadingODataAnnotationWithoutPrefix: true);
        }

        #endregion

        [Fact]
        public void NonZeroCountComesBeforeAndAfterInnerFeedShouldThrow()
        {
            const string entryText = @"
                ""NavProp@odata.count"" : 2,
                ""NavProp"" : [],
                ""NavProp@odata.count"" : 2";

            var entryReader = GetEntryReader(entryText, isResponse: true);
            entryReader.Read();
            Assert.Equal(ODataReaderState.ResourceStart, entryReader.State);
            entryReader.Read();
            Assert.Equal(ODataReaderState.NestedResourceInfoStart, entryReader.State);
            entryReader.Read();
            Assert.Equal(ODataReaderState.ResourceSetStart, entryReader.State);
            Assert.Equal(2, (entryReader.Item as ODataResourceSet).Count);
            Action read = () => entryReader.Read();
            read.Throws<ODataException>(ErrorStrings.ODataJsonResourceDeserializer_DuplicateNestedResourceSetAnnotation("odata.count", "NavProp"));
        }

        [Fact]
        public void DifferentPropertyInBetweenInnerFeedShouldThrow()
        {
            const string entryText = @"
                ""NavProp"" : [],
                ""NavProp@odata.nextLink"" : ""http://nextLink"",
                ""PrimitiveProp"" : 1,
                ""NavProp@odata.count"" : 2";

            var entryReader = GetEntryReader(entryText, isResponse: true);
            entryReader.Read();
            Assert.Equal(ODataReaderState.ResourceStart, entryReader.State);
            entryReader.Read();
            Assert.Equal(ODataReaderState.NestedResourceInfoStart, entryReader.State);
            entryReader.Read();
            Assert.Equal(ODataReaderState.ResourceSetStart, entryReader.State);
            entryReader.Read();
            Assert.Equal(ODataReaderState.ResourceSetEnd, entryReader.State);
            Assert.Equal(new Uri("http://nextLink"), (entryReader.Item as ODataResourceSet).NextPageLink);
            Assert.Null((entryReader.Item as ODataResourceSet).Count);
            entryReader.Read();
            Assert.Equal(ODataReaderState.NestedResourceInfoEnd, entryReader.State);
            Action read = () => entryReader.Read();
            read.Throws<ODataException>(ErrorStrings.PropertyAnnotationAfterTheProperty("odata.count", "NavProp"));
        }

        [Fact]
        public void NextLinkComesBeforeInnerFeedOnResponse()
        {
            const string entryText = @"
                ""NavProp@odata.nextLink"" : ""http://nextLink"",
                ""NavProp"" : []";

            var entryReader = GetEntryReader(entryText, isResponse: true);
            entryReader.Read();
            Assert.Equal(ODataReaderState.ResourceStart, entryReader.State);
            entryReader.Read();
            Assert.Equal(ODataReaderState.NestedResourceInfoStart, entryReader.State);
            entryReader.Read();
            Assert.Equal(ODataReaderState.ResourceSetStart, entryReader.State);
            Assert.Equal(new Uri("http://nextLink"), (entryReader.Item as ODataResourceSet).NextPageLink);
        }

        [Fact]
        public void NextLinkComesBeforeInnerFeedOnRequestShouldThrow()
        {
            const string entryText = @"
                ""NavProp@odata.nextLink"" : ""http://nextLink"",
                ""NavProp"" : []";

            var entryReader = GetEntryReader(entryText, isResponse: false);
            Action test = () => entryReader.Read();
            test.Throws<ODataException>(ErrorStrings.ODataJsonResourceDeserializer_UnexpectedNavigationLinkInRequestPropertyAnnotation("NavProp", "odata.nextLink", "odata.bind"));
        }

        [Fact]
        public void DeltaLinkComesBeforeInnerFeedShouldThrow()
        {
            foreach (bool isResponse in new[] { true, false })
            {
                const string entryText = @"
                ""NavProp@odata.nextLink"" : ""http://nextLink"",
                ""NavProp@odata.deltaLink"" : ""http://deltaLink"",
                ""NavProp"" : []";

                var entryReader = GetEntryReader(entryText, isResponse);
                Action test = () => entryReader.Read();
                test.Throws<ODataException>(ErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedAnnotationProperties("odata.deltaLink"));
            }
        }

        [Fact]
        public void NextLinkComesBeforeAndAfterInnerFeedShouldThrow()
        {
            const string entryText = @"
                ""NavProp@odata.nextLink"" : ""http://nextlink"",
                ""NavProp"" : [],
                ""NavProp@odata.nextLink"" : ""http://nextLink2""";

            var entryReader = GetEntryReader(entryText, isResponse: true);
            entryReader.Read();
            Assert.Equal(ODataReaderState.ResourceStart, entryReader.State);
            entryReader.Read();
            Assert.Equal(ODataReaderState.NestedResourceInfoStart, entryReader.State);
            entryReader.Read();
            Assert.Equal(ODataReaderState.ResourceSetStart, entryReader.State);
            Assert.Equal(new Uri("http://nextLink"), (entryReader.Item as ODataResourceSet).NextPageLink);
            Action read = () => entryReader.Read();
            read.Throws<ODataException>(ErrorStrings.ODataJsonResourceDeserializer_DuplicateNestedResourceSetAnnotation("odata.nextLink", "NavProp"));
        }

        [Fact]
        public void NextLinkComesAfterInnerFeedOnResponse()
        {
            const string entryText = @"
                ""NavProp"" : [],
                ""NavProp@odata.nextLink"" : ""http://nextLink""";

            var entryReader = GetEntryReader(entryText, isResponse: true);
            entryReader.Read();
            Assert.Equal(ODataReaderState.ResourceStart, entryReader.State);
            entryReader.Read();
            Assert.Equal(ODataReaderState.NestedResourceInfoStart, entryReader.State);
            entryReader.Read();
            Assert.Equal(ODataReaderState.ResourceSetStart, entryReader.State);
            Assert.Null((entryReader.Item as ODataResourceSet).NextPageLink);
            entryReader.Read();
            Assert.Equal(ODataReaderState.ResourceSetEnd, entryReader.State);
            Assert.Equal(new Uri("http://nextLink"), (entryReader.Item as ODataResourceSet).NextPageLink);
        }

        [Fact]
        public void NextLinkComesAfterInnerFeedOnRequestShouldFail()
        {
            const string entryText = @"
                ""NavProp"" : [],
                ""NavProp@odata.nextLink"" : ""http://nextLink""";

            var entryReader = GetEntryReader(entryText, isResponse: false);
            entryReader.Read();
            Assert.Equal(ODataReaderState.ResourceStart, entryReader.State);
            entryReader.Read();
            Assert.Equal(ODataReaderState.NestedResourceInfoStart, entryReader.State);
            entryReader.Read();
            Assert.Equal(ODataReaderState.ResourceSetStart, entryReader.State);
            Assert.Null((entryReader.Item as ODataResourceSet).NextPageLink);
            Action test = () => entryReader.Read();
            test.Throws<ODataException>(ErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedPropertyAnnotation("NavProp", "odata.nextLink"));
        }

        [Fact]
        public void DeltaLinkComesAfterInnerFeedShouldThrow()
        {
            foreach (bool isResponse in new[] { true, false })
            {
                const string entryText = @"
                ""NavProp"" : [],
                ""NavProp@odata.deltaLink"" : ""http://deltaLink""";

                var entryReader = GetEntryReader(entryText, isResponse);
                entryReader.Read();
                Assert.Equal(ODataReaderState.ResourceStart, entryReader.State);
                entryReader.Read();
                Assert.Equal(ODataReaderState.NestedResourceInfoStart, entryReader.State);
                entryReader.Read();
                Assert.Equal(ODataReaderState.ResourceSetStart, entryReader.State);
                Assert.Null((entryReader.Item as ODataResourceSet).NextPageLink);
                Action test = () => entryReader.Read();
                string expectedErrorMsg = isResponse ? ErrorStrings.ODataJsonResourceDeserializer_UnexpectedPropertyAnnotationAfterExpandedResourceSet("odata.deltaLink", "NavProp") : ErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedPropertyAnnotation("NavProp", "odata.deltaLink");
                test.Throws<ODataException>(expectedErrorMsg);
            }
        }

        #endregion expanded feeds

        private ODataReader GetEntryReader(string entryText, bool isResponse, bool enableReadingODataAnnotationWithoutPrefix = false)
        {
            this.CreateMessageReader(entryText, /*forEntry*/ true, isResponse, enableReadingODataAnnotationWithoutPrefix);
            return this.messageReader.CreateODataResourceReader(this.entitySet, this.type);
        }
        private ODataReader GetFeedReader(string feedText, bool isResponse, bool enableReadingODataAnnotationWithoutPrefix = false)
        {
            this.CreateMessageReader(feedText, /*forEntry*/ false, isResponse, enableReadingODataAnnotationWithoutPrefix);
            return this.messageReader.CreateODataResourceSetReader(this.entitySet, this.type);
        }

        private void CreateMessageReader(string payloadBody, bool forEntry, bool isResponse, bool enableReadingODataAnnotationWithoutPrefix)
        {
            string payloadPrefix = @"{
  ""@odata.context"":""http://example.com/$metadata#EntitySet" + (forEntry ? "/$entity" : string.Empty) + "\",";
            const string payloadSuffix = "}";
            string payload = payloadPrefix + payloadBody + payloadSuffix;

            var serviceProvider = ServiceProviderHelper.BuildServiceProvider(null);
            serviceProvider.GetRequiredService<ODataMessageReaderSettings>().EnableReadingODataAnnotationWithoutPrefix = enableReadingODataAnnotationWithoutPrefix;

            var message = new InMemoryMessage() { ServiceProvider = serviceProvider };
            message.Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
            message.SetHeader("Content-Type", "application/json;odata.metadata=minimal;odata.streaming=true");
            var messageSettings = new ODataMessageReaderSettings();
            messageSettings.EnableReadingODataAnnotationWithoutPrefix = enableReadingODataAnnotationWithoutPrefix;

            if (isResponse)
            {
                this.messageReader = new ODataMessageReader((IODataResponseMessage)message, messageSettings, this.model);
            }
            else
            {
                this.messageReader = new ODataMessageReader((IODataRequestMessage)message, messageSettings, this.model);
            }
        }
    }
}

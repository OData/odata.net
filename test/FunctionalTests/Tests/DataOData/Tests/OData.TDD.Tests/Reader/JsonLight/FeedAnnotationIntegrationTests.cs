//---------------------------------------------------------------------
// <copyright file="FeedAnnotationIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests
{
    using System;
    using System.IO;
    using System.Text;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.Test.OData.TDD.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ErrorStrings = Microsoft.OData.Core.Strings;

    [TestClass]
    public class FeedAnnotationIntegrationTests
    {
        private ODataMessageReader messageReader = null;
        private EdmEntitySet entitySet;
        private EdmEntityType type;
        private IEdmModel model;

        [TestInitialize]
        public void Initialize()
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

        [TestCleanup]
        public void Cleanup()
        {
            if (this.messageReader != null)
            {
                this.messageReader.Dispose();
            }
        }

        #region Top-level feeds

        [TestMethod]
        public void NextLinkComesBeforeTopLevelFeed()
        {
            foreach (bool isResponse in new[] { true, false })
            {
                const string feedText = @"
                ""@odata.nextLink"":""http://nextLink"",
                ""value"":[]";
                var feedReader = GetFeedReader(feedText, isResponse);
                feedReader.Read();
                feedReader.State.Should().Be(ODataReaderState.FeedStart);
                feedReader.Item.As<ODataFeed>().NextPageLink.Should().Be(new Uri("http://nextLink"));

                feedReader.Read();
                feedReader.State.Should().Be(ODataReaderState.FeedEnd);
                feedReader.Item.As<ODataFeed>().NextPageLink.Should().Be(new Uri("http://nextLink"));
            }
        }

        [TestMethod]
        public void NextLinkComesAfterTopLevelFeed()
        {
            foreach (bool isResponse in new[] { true, false })
            {
                const string feedText = @"
                ""value"":[],
                ""@odata.nextLink"":""http://nextLink""";
                var feedReader = GetFeedReader(feedText, isResponse);
                feedReader.Read();
                feedReader.State.Should().Be(ODataReaderState.FeedStart);
                feedReader.Item.As<ODataFeed>().NextPageLink.Should().Be(null);

                feedReader.Read();
                feedReader.State.Should().Be(ODataReaderState.FeedEnd);
                feedReader.Item.As<ODataFeed>().NextPageLink.Should().Be(new Uri("http://nextLink"));
            }
        }

        [TestMethod]
        public void DeltaLinkComesBeforeTopLevelFeed()
        {
            foreach (bool isResponse in new[] { true, false })
            {
                const string feedText = @"
                ""@odata.deltaLink"":""http://deltaLink"",
                ""value"":[]";
                var feedReader = GetFeedReader(feedText, isResponse);
                feedReader.Read();
                feedReader.State.Should().Be(ODataReaderState.FeedStart);
                feedReader.Item.As<ODataFeed>().DeltaLink.Should().Be(new Uri("http://deltaLink"));

                feedReader.Read();
                feedReader.State.Should().Be(ODataReaderState.FeedEnd);
                feedReader.Item.As<ODataFeed>().DeltaLink.Should().Be(new Uri("http://deltaLink"));
            }
        }

        [TestMethod]
        public void DeltaLinkComesAfterTopLevelFeed()
        {
            foreach (bool isResponse in new[] { true, false })
            {
                const string feedText = @"
                ""value"":[],
                ""@odata.deltaLink"":""http://deltaLink""";
                var feedReader = GetFeedReader(feedText, isResponse);
                feedReader.Read();
                feedReader.State.Should().Be(ODataReaderState.FeedStart);
                feedReader.Item.As<ODataFeed>().DeltaLink.Should().Be(null);

                feedReader.Read();
                feedReader.State.Should().Be(ODataReaderState.FeedEnd);
                feedReader.Item.As<ODataFeed>().DeltaLink.Should().Be(new Uri("http://deltaLink"));
            }
        }

        [TestMethod]
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
                entryReader.State.Should().Be(ODataReaderState.FeedStart);
                entryReader.Item.As<ODataFeed>().NextPageLink.Should().Be(new Uri("http://nextLink"));
                Action read = () => entryReader.Read();
                read.ShouldThrow<ODataException>().WithMessage(ErrorStrings.DuplicatePropertyNamesChecker_DuplicateAnnotationNotAllowed("odata.nextLink"));
            }
        }

        [TestMethod]
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
                entryReader.State.Should().Be(ODataReaderState.FeedStart);
                entryReader.Item.As<ODataFeed>().DeltaLink.Should().Be(new Uri("http://deltaLink"));
                Action read = () => entryReader.Read();
                read.ShouldThrow<ODataException>().WithMessage(ErrorStrings.DuplicatePropertyNamesChecker_DuplicateAnnotationNotAllowed("odata.deltaLink"));
            }
        }

        #endregion Top-level feeds

        #region expanded feeds

        [TestMethod]
        public void CountComesBeforeInnerFeedOnResponse()
        {
            const string entryText = @"
                ""NavProp@odata.count"" : 0,
                ""NavProp"" : []";

            var entryReader = GetEntryReader(entryText, isResponse: true);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.EntryStart);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.NavigationLinkStart);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.FeedStart);
            entryReader.Item.As<ODataFeed>().Count.Should().Be(0);
        }

        [TestMethod]
        public void CountComesAfterInnerFeedOnResponse()
        {
            const string entryText = @"
                ""NavProp"" : [],
                ""NavProp@odata.count"" : 0";

            var entryReader = GetEntryReader(entryText, isResponse: true);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.EntryStart);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.NavigationLinkStart);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.FeedStart);
            entryReader.Item.As<ODataFeed>().Count.Should().Be(null);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.FeedEnd);
            entryReader.Item.As<ODataFeed>().Count.Should().Be(0);
        }

        [TestMethod]
        public void CountComesBeforeAndAfterInnerFeedShouldThrow()
        {
            const string entryText = @"
                ""NavProp@odata.count"" : 0,
                ""NavProp"" : [],
                ""NavProp@odata.count"" : 0";

            var entryReader = GetEntryReader(entryText, isResponse: true);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.EntryStart);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.NavigationLinkStart);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.FeedStart);
            entryReader.Item.As<ODataFeed>().Count.Should().Be(0);
            Action read = () => entryReader.Read();
            read.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightEntryAndFeedDeserializer_DuplicateExpandedFeedAnnotation("odata.count", "NavProp"));
        }

        [TestMethod]
        public void NonZeroCountComesBeforeInnerFeedOnResponse()
        {
            const string entryText = @"
                ""NavProp@odata.count"" : 2,
                ""NavProp"" : []";

            var entryReader = GetEntryReader(entryText, isResponse: true);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.EntryStart);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.NavigationLinkStart);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.FeedStart);
            entryReader.Item.As<ODataFeed>().Count.Should().Be(2);
        }

        [TestMethod]
        public void NonZeroCountComesAfterInnerFeedOnResponse()
        {
            const string entryText = @"
                ""NavProp"" : [],
                ""NavProp@odata.count"" : 2";

            var entryReader = GetEntryReader(entryText, isResponse: true);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.EntryStart);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.NavigationLinkStart);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.FeedStart);
            entryReader.Item.As<ODataFeed>().Count.Should().Be(null);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.FeedEnd);
            entryReader.Item.As<ODataFeed>().Count.Should().Be(2);
        }

        [TestMethod]
        public void NonZeroCountAndNextLinkComesAfterInnerFeedOnResponse()
        {
            const string entryText = @"
                ""NavProp"" : [],
                ""NavProp@odata.count"" : 2,
                ""NavProp@odata.nextLink"" : ""http://nextLink""";

            var entryReader = GetEntryReader(entryText, isResponse: true);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.EntryStart);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.NavigationLinkStart);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.FeedStart);
            entryReader.Item.As<ODataFeed>().Count.Should().Be(null);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.FeedEnd);
            entryReader.Item.As<ODataFeed>().Count.Should().Be(2);
            entryReader.Item.As<ODataFeed>().NextPageLink.Should().Be(new Uri("http://nextLink"));
        }

        [TestMethod]
        public void NonZeroCountComesBeforeAndAfterInnerFeedShouldThrow()
        {
            const string entryText = @"
                ""NavProp@odata.count"" : 2,
                ""NavProp"" : [],
                ""NavProp@odata.count"" : 2";

            var entryReader = GetEntryReader(entryText, isResponse: true);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.EntryStart);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.NavigationLinkStart);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.FeedStart);
            entryReader.Item.As<ODataFeed>().Count.Should().Be(2);
            Action read = () => entryReader.Read();
            read.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightEntryAndFeedDeserializer_DuplicateExpandedFeedAnnotation("odata.count", "NavProp"));
        }

        [TestMethod]
        public void DifferentPropertyInBetweenInnerFeedShouldThrow()
        {
            const string entryText = @"
                ""NavProp"" : [],
                ""NavProp@odata.nextLink"" : ""http://nextLink"",
                ""PrimitiveProp"" : 1,
                ""NavProp@odata.count"" : 2";

            var entryReader = GetEntryReader(entryText, isResponse: true);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.EntryStart);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.NavigationLinkStart);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.FeedStart);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.FeedEnd);
            entryReader.Item.As<ODataFeed>().NextPageLink.Should().Be(new Uri("http://nextLink"));
            entryReader.Item.As<ODataFeed>().Count.Should().Be(null);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.NavigationLinkEnd);
            Action read = () => entryReader.Read();
            read.ShouldThrow<ODataException>().WithMessage(ErrorStrings.DuplicatePropertyNamesChecker_PropertyAnnotationAfterTheProperty("odata.count", "NavProp"));
        }

        [TestMethod]
        public void NextLinkComesBeforeInnerFeedOnResponse()
        {
            const string entryText = @"
                ""NavProp@odata.nextLink"" : ""http://nextLink"",
                ""NavProp"" : []";

            var entryReader = GetEntryReader(entryText, isResponse: true);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.EntryStart);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.NavigationLinkStart);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.FeedStart);
            entryReader.Item.As<ODataFeed>().NextPageLink.Should().Be(new Uri("http://nextLink"));
        }

        [TestMethod]
        public void NextLinkComesBeforeInnerFeedOnRequestShouldThrow()
        {
            const string entryText = @"
                ""NavProp@odata.nextLink"" : ""http://nextLink"",
                ""NavProp"" : []";

            var entryReader = GetEntryReader(entryText, isResponse: false);
            Action test = () => entryReader.Read();
            test.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightEntryAndFeedDeserializer_UnexpectedNavigationLinkInRequestPropertyAnnotation("NavProp", "odata.nextLink", "odata.bind"));
        }

        [TestMethod]
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
                test.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties("odata.deltaLink"));
            }
        }

        [TestMethod]
        public void NextLinkComesBeforeAndAfterInnerFeedShouldThrow()
        {
            const string entryText = @"
                ""NavProp@odata.nextLink"" : ""http://nextlink"",
                ""NavProp"" : [],
                ""NavProp@odata.nextLink"" : ""http://nextLink2""";

            var entryReader = GetEntryReader(entryText, isResponse: true);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.EntryStart);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.NavigationLinkStart);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.FeedStart);
            entryReader.Item.As<ODataFeed>().NextPageLink.Should().Be(new Uri("http://nextLink"));
            Action read = () => entryReader.Read();
            read.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightEntryAndFeedDeserializer_DuplicateExpandedFeedAnnotation("odata.nextLink", "NavProp"));
        }

        [TestMethod]
        public void NextLinkComesAfterInnerFeedOnResponse()
        {
            const string entryText = @"
                ""NavProp"" : [],
                ""NavProp@odata.nextLink"" : ""http://nextLink""";

            var entryReader = GetEntryReader(entryText, isResponse: true);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.EntryStart);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.NavigationLinkStart);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.FeedStart);
            entryReader.Item.As<ODataFeed>().NextPageLink.Should().Be(null);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.FeedEnd);
            entryReader.Item.As<ODataFeed>().NextPageLink.Should().Be(new Uri("http://nextLink"));
        }

        [TestMethod]
        public void NextLinkComesAfterInnerFeedOnRequestShouldFail()
        {
            const string entryText = @"
                ""NavProp"" : [],
                ""NavProp@odata.nextLink"" : ""http://nextLink""";

            var entryReader = GetEntryReader(entryText, isResponse: false);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.EntryStart);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.NavigationLinkStart);
            entryReader.Read();
            entryReader.State.Should().Be(ODataReaderState.FeedStart);
            entryReader.Item.As<ODataFeed>().NextPageLink.Should().Be(null);
            Action test = () => entryReader.Read();
            test.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedPropertyAnnotation("NavProp", "odata.nextLink"));
        }

        [TestMethod]
        public void DeltaLinkComesAfterInnerFeedShouldThrow()
        {
            foreach (bool isResponse in new[] { true, false })
            {
                const string entryText = @"
                ""NavProp"" : [],
                ""NavProp@odata.deltaLink"" : ""http://deltaLink""";

                var entryReader = GetEntryReader(entryText, isResponse);
                entryReader.Read();
                entryReader.State.Should().Be(ODataReaderState.EntryStart);
                entryReader.Read();
                entryReader.State.Should().Be(ODataReaderState.NavigationLinkStart);
                entryReader.Read();
                entryReader.State.Should().Be(ODataReaderState.FeedStart);
                entryReader.Item.As<ODataFeed>().NextPageLink.Should().Be(null);
                Action test = () => entryReader.Read();
                string expectedErrorMsg = isResponse ? ErrorStrings.ODataJsonLightEntryAndFeedDeserializer_UnexpectedPropertyAnnotationAfterExpandedFeed("odata.deltaLink", "NavProp") : ErrorStrings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedPropertyAnnotation("NavProp", "odata.deltaLink");
                test.ShouldThrow<ODataException>().WithMessage(expectedErrorMsg);
            }
        }

        #endregion expanded feeds

        private ODataReader GetEntryReader(string entryText, bool isResponse)
        {
            this.CreateMessageReader(entryText, /*forEntry*/ true, isResponse);
            return this.messageReader.CreateODataEntryReader(this.entitySet, this.type);
        }
        private ODataReader GetFeedReader(string feedText, bool isResponse)
        {
            this.CreateMessageReader(feedText, /*forEntry*/ false, isResponse);
            return this.messageReader.CreateODataFeedReader(this.entitySet, this.type);
        }

        private void CreateMessageReader(string payloadBody, bool forEntry, bool isResponse)
        {
            string payloadPrefix = @"{
  ""@odata.context"":""http://example.com/$metadata#EntitySet" + (forEntry ? "/$entity" : string.Empty) + "\",";
            const string payloadSuffix = "}";
            string payload = payloadPrefix + payloadBody + payloadSuffix;

            var message = new InMemoryMessage();
            message.Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
            message.SetHeader("Content-Type", "application/json;odata.metadata=minimal;odata.streaming=true");
            var messageSettings = new ODataMessageReaderSettings();
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

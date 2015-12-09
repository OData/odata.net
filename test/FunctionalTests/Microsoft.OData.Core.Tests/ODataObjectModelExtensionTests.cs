//---------------------------------------------------------------------
// <copyright file="ODataObjectModelExtensionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Xunit;

namespace Microsoft.OData.Core.Tests
{
    public class ODataObjectModelExtensionTests
    {
        #region ODataProperty
        [Fact]
        public void SetPropertySerializationInfoShouldThrowOnNullProperty()
        {
            ODataProperty property = null;
            Action action = () => property.SetSerializationInfo(null);
            action.ShouldThrow<ArgumentNullException>().WithMessage("property", ComparisonMode.Substring);
        }

        [Fact]
        public void ShouldBeAbleToSetThePropertySerializationInfo()
        {
            ODataProperty property = new ODataProperty();
            ODataPropertySerializationInfo serializationInfo = new ODataPropertySerializationInfo();
            property.SetSerializationInfo(serializationInfo);
            property.SerializationInfo.Should().BeSameAs(serializationInfo);
        }

        [Fact]
        public void ShouldBeAbleToClearThePropertySerializationInfo()
        {
            ODataProperty property = new ODataProperty();
            ODataPropertySerializationInfo serializationInfo = new ODataPropertySerializationInfo();
            property.SerializationInfo = serializationInfo;
            property.SetSerializationInfo(null);
            property.SerializationInfo.Should().BeNull();
        }
        #endregion ODataProperty

        #region ODataFeed
        [Fact]
        public void SetFeedSerializationInfoShouldThrowOnNullFeed()
        {
            ODataFeed feed = null;
            Action action = () => feed.SetSerializationInfo(null);
            action.ShouldThrow<ArgumentNullException>().WithMessage("feed", ComparisonMode.Substring);
        }

        [Fact]
        public void ShouldBeAbleToSetTheFeedSerializationInfo()
        {
            ODataFeed feed = new ODataFeed();
            ODataFeedAndEntrySerializationInfo serializationInfo = new ODataFeedAndEntrySerializationInfo { NavigationSourceName = "Set", NavigationSourceEntityTypeName = "ns.base", ExpectedTypeName = "ns.expected" };
            feed.SetSerializationInfo(serializationInfo);
            feed.SerializationInfo.Should().BeSameAs(serializationInfo);
        }

        [Fact]
        public void ShouldBeAbleToClearTheFeedSerializationInfo()
        {
            ODataFeed feed = new ODataFeed();
            ODataFeedAndEntrySerializationInfo serializationInfo = new ODataFeedAndEntrySerializationInfo { NavigationSourceName = "Set", NavigationSourceEntityTypeName = "ns.base", ExpectedTypeName = "ns.expected" };
            feed.SerializationInfo = serializationInfo;
            feed.SetSerializationInfo(null);
            feed.SerializationInfo.Should().BeNull();
        }
        #endregion ODataFeed

        #region ODataEntry
        [Fact]
        public void SetEntrySerializationInfoShouldThrowOnNullEntry()
        {
            ODataEntry entry = null;
            Action action = () => entry.SetSerializationInfo(null);
            action.ShouldThrow<ArgumentNullException>().WithMessage("entry", ComparisonMode.Substring);
        }

        [Fact]
        public void ShouldBeAbleToSetTheEntrySerializationInfo()
        {
            ODataEntry entry = new ODataEntry();
            ODataFeedAndEntrySerializationInfo serializationInfo = new ODataFeedAndEntrySerializationInfo { NavigationSourceName = "Set", NavigationSourceEntityTypeName = "ns.base", ExpectedTypeName = "ns.expected" };
            entry.SetSerializationInfo(serializationInfo);
            entry.SerializationInfo.Should().BeSameAs(serializationInfo);
        }

        [Fact]
        public void ShouldBeAbleToClearTheEntrySerializationInfo()
        {
            ODataEntry entry = new ODataEntry();
            ODataFeedAndEntrySerializationInfo serializationInfo = new ODataFeedAndEntrySerializationInfo { NavigationSourceName = "Set", NavigationSourceEntityTypeName = "ns.base", ExpectedTypeName = "ns.expected" };
            entry.SerializationInfo = serializationInfo;
            entry.SetSerializationInfo(null);
            entry.SerializationInfo.Should().BeNull();
        }
        #endregion ODataEntry

        #region ODataCollectionStart
        [Fact]
        public void SetCollectionStartSerializationInfoShouldThrowOnNullCollectionStart()
        {
            ODataCollectionStart collectionStart = null;
            Action action = () => collectionStart.SetSerializationInfo(null);
            action.ShouldThrow<ArgumentNullException>().WithMessage("collectionStart", ComparisonMode.Substring);
        }

        [Fact]
        public void ShouldBeAbleToSetTheCollectionStartSerializationInfo()
        {
            ODataCollectionStart collectionStart = new ODataCollectionStart();
            ODataCollectionStartSerializationInfo serializationInfo = new ODataCollectionStartSerializationInfo { CollectionTypeName = "Collection(Edm.String)" };
            collectionStart.SetSerializationInfo(serializationInfo);
            collectionStart.SerializationInfo.Should().BeSameAs(serializationInfo);
        }

        [Fact]
        public void ShouldBeAbleToClearTheCollectionStartSerializationInfo()
        {
            ODataCollectionStart collectionStart = new ODataCollectionStart();
            ODataCollectionStartSerializationInfo serializationInfo = new ODataCollectionStartSerializationInfo { CollectionTypeName = "Collection(Edm.String)" };
            collectionStart.SerializationInfo = serializationInfo;
            collectionStart.SetSerializationInfo(null);
            collectionStart.SerializationInfo.Should().BeNull();
        }
        #endregion ODataCollectionStart
    }
}

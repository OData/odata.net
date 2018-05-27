//---------------------------------------------------------------------
// <copyright file="ODataObjectModelExtensionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class ODataObjectModelExtensionTests
    {
        #region ODataProperty
        [Fact]
        public void SetPropertySerializationInfoShouldThrowOnNullProperty()
        {
            ODataProperty property = null;
            Action action = () => property.SetSerializationInfo(null);
            action.ShouldThrow<ArgumentNullException>().Where(e => e.Message.Contains("property"));
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
            ODataResourceSet resourceCollection = null;
            Action action = () => resourceCollection.SetSerializationInfo(null);
            action.ShouldThrow<ArgumentNullException>().Where(e => e.Message.Contains("resourceSet"));
        }

        [Fact]
        public void ShouldBeAbleToSetTheFeedSerializationInfo()
        {
            ODataResourceSet resourceCollection = new ODataResourceSet();
            ODataResourceSerializationInfo serializationInfo = new ODataResourceSerializationInfo { NavigationSourceName = "Set", NavigationSourceEntityTypeName = "ns.base", ExpectedTypeName = "ns.expected" };
            resourceCollection.SetSerializationInfo(serializationInfo);
            resourceCollection.SerializationInfo.Should().BeSameAs(serializationInfo);
        }

        [Fact]
        public void ShouldBeAbleToClearTheFeedSerializationInfo()
        {
            ODataResourceSet resourceCollection = new ODataResourceSet();
            ODataResourceSerializationInfo serializationInfo = new ODataResourceSerializationInfo { NavigationSourceName = "Set", NavigationSourceEntityTypeName = "ns.base", ExpectedTypeName = "ns.expected" };
            resourceCollection.SerializationInfo = serializationInfo;
            resourceCollection.SetSerializationInfo(null);
            resourceCollection.SerializationInfo.Should().BeNull();
        }
        #endregion ODataFeed

        #region ODataResource
        [Fact]
        public void SetEntrySerializationInfoShouldThrowOnNullEntry()
        {
            ODataResource entry = null;
            Action action = () => entry.SetSerializationInfo(null);
            action.ShouldThrow<ArgumentNullException>().Where(e => e.Message.Contains("resource"));
        }

        [Fact]
        public void ShouldBeAbleToSetTheEntrySerializationInfo()
        {
            ODataResource entry = new ODataResource();
            ODataResourceSerializationInfo serializationInfo = new ODataResourceSerializationInfo { NavigationSourceName = "Set", NavigationSourceEntityTypeName = "ns.base", ExpectedTypeName = "ns.expected" };
            entry.SetSerializationInfo(serializationInfo);
            entry.SerializationInfo.Should().BeSameAs(serializationInfo);
        }

        [Fact]
        public void ShouldBeAbleToClearTheEntrySerializationInfo()
        {
            ODataResource entry = new ODataResource();
            ODataResourceSerializationInfo serializationInfo = new ODataResourceSerializationInfo { NavigationSourceName = "Set", NavigationSourceEntityTypeName = "ns.base", ExpectedTypeName = "ns.expected" };
            entry.SerializationInfo = serializationInfo;
            entry.SetSerializationInfo(null);
            entry.SerializationInfo.Should().BeNull();
        }
        #endregion ODataResource

        #region ODataCollectionStart
        [Fact]
        public void SetCollectionStartSerializationInfoShouldThrowOnNullCollectionStart()
        {
            ODataCollectionStart collectionStart = null;
            Action action = () => collectionStart.SetSerializationInfo(null);
            action.ShouldThrow<ArgumentNullException>().Where(e => e.Message.Contains("collectionStart"));
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

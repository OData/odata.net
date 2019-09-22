//---------------------------------------------------------------------
// <copyright file="ODataObjectModelExtensionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
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
            Assert.Throws<ArgumentNullException>("property", action);
        }

        [Fact]
        public void ShouldBeAbleToSetThePropertySerializationInfo()
        {
            ODataProperty property = new ODataProperty();
            ODataPropertySerializationInfo serializationInfo = new ODataPropertySerializationInfo();
            property.SetSerializationInfo(serializationInfo);
            Assert.Same(serializationInfo, property.SerializationInfo);
        }

        [Fact]
        public void ShouldBeAbleToClearThePropertySerializationInfo()
        {
            ODataProperty property = new ODataProperty();
            ODataPropertySerializationInfo serializationInfo = new ODataPropertySerializationInfo();
            property.SerializationInfo = serializationInfo;
            property.SetSerializationInfo(null);
            Assert.Null(property.SerializationInfo);
        }
        #endregion ODataProperty

        #region ODataFeed
        [Fact]
        public void SetFeedSerializationInfoShouldThrowOnNullFeed()
        {
            ODataResourceSet resourceCollection = null;
            Action action = () => resourceCollection.SetSerializationInfo(null);
            Assert.Throws<ArgumentNullException>("resourceSet", action);
        }

        [Fact]
        public void ShouldBeAbleToSetTheFeedSerializationInfo()
        {
            ODataResourceSet resourceCollection = new ODataResourceSet();
            ODataResourceSerializationInfo serializationInfo = new ODataResourceSerializationInfo { NavigationSourceName = "Set", NavigationSourceEntityTypeName = "ns.base", ExpectedTypeName = "ns.expected" };
            resourceCollection.SetSerializationInfo(serializationInfo);
            Assert.Same(serializationInfo, resourceCollection.SerializationInfo);
        }

        [Fact]
        public void ShouldBeAbleToClearTheFeedSerializationInfo()
        {
            ODataResourceSet resourceCollection = new ODataResourceSet();
            ODataResourceSerializationInfo serializationInfo = new ODataResourceSerializationInfo { NavigationSourceName = "Set", NavigationSourceEntityTypeName = "ns.base", ExpectedTypeName = "ns.expected" };
            resourceCollection.SerializationInfo = serializationInfo;
            resourceCollection.SetSerializationInfo(null);
            Assert.Null(resourceCollection.SerializationInfo);
        }
        #endregion ODataFeed

        #region ODataResource
        [Fact]
        public void SetEntrySerializationInfoShouldThrowOnNullEntry()
        {
            ODataResource entry = null;
            Action action = () => entry.SetSerializationInfo(null);
            Assert.Throws<ArgumentNullException>("resource", action);
        }

        [Fact]
        public void ShouldBeAbleToSetTheEntrySerializationInfo()
        {
            ODataResource entry = new ODataResource();
            ODataResourceSerializationInfo serializationInfo = new ODataResourceSerializationInfo { NavigationSourceName = "Set", NavigationSourceEntityTypeName = "ns.base", ExpectedTypeName = "ns.expected" };
            entry.SetSerializationInfo(serializationInfo);
            Assert.Same(serializationInfo, entry.SerializationInfo);
        }

        [Fact]
        public void ShouldBeAbleToClearTheEntrySerializationInfo()
        {
            ODataResource entry = new ODataResource();
            ODataResourceSerializationInfo serializationInfo = new ODataResourceSerializationInfo { NavigationSourceName = "Set", NavigationSourceEntityTypeName = "ns.base", ExpectedTypeName = "ns.expected" };
            entry.SerializationInfo = serializationInfo;
            entry.SetSerializationInfo(null);
            Assert.Null(entry.SerializationInfo);
        }
        #endregion ODataResource

        #region ODataCollectionStart
        [Fact]
        public void SetCollectionStartSerializationInfoShouldThrowOnNullCollectionStart()
        {
            ODataCollectionStart collectionStart = null;
            Action action = () => collectionStart.SetSerializationInfo(null);
            Assert.Throws<ArgumentNullException>("collectionStart", action);
        }

        [Fact]
        public void ShouldBeAbleToSetTheCollectionStartSerializationInfo()
        {
            ODataCollectionStart collectionStart = new ODataCollectionStart();
            ODataCollectionStartSerializationInfo serializationInfo = new ODataCollectionStartSerializationInfo { CollectionTypeName = "Collection(Edm.String)" };
            collectionStart.SetSerializationInfo(serializationInfo);
            Assert.Same(serializationInfo, collectionStart.SerializationInfo);
        }

        [Fact]
        public void ShouldBeAbleToClearTheCollectionStartSerializationInfo()
        {
            ODataCollectionStart collectionStart = new ODataCollectionStart();
            ODataCollectionStartSerializationInfo serializationInfo = new ODataCollectionStartSerializationInfo { CollectionTypeName = "Collection(Edm.String)" };
            collectionStart.SerializationInfo = serializationInfo;
            collectionStart.SetSerializationInfo(null);
            Assert.Null(collectionStart.SerializationInfo);
        }
        #endregion ODataCollectionStart
    }
}

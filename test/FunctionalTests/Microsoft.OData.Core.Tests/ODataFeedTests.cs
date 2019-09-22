//---------------------------------------------------------------------
// <copyright file="ODataFeedTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class ODataFeedTests
    {
        private ODataResourceSet odataFeed;

        public ODataFeedTests()
        {
            this.odataFeed = new ODataResourceSet();
        }

        [Fact]
        public void InstanceAnnotationsPropertyShouldNotBeNullAtCreation()
        {
            Assert.NotNull(this.odataFeed.InstanceAnnotations);
        }

        [Fact]
        public void InstanceAnnotationsPropertyShouldReturnAWritableCollectionAtCreation()
        {
            Assert.NotNull(this.odataFeed.InstanceAnnotations);
            this.odataFeed.InstanceAnnotations.Add(new ODataInstanceAnnotation("namespace.name", new ODataPrimitiveValue("value")));
            Assert.Single(this.odataFeed.InstanceAnnotations);
        }

        [Fact]
        public void SetNullValueToInstanceAnnotationsPropertyShouldThrow()
        {
            Action test = () => this.odataFeed.InstanceAnnotations = null;
            Assert.Throws<ArgumentNullException>("value", test);
        }

        [Fact]
        public void SetListValueToInstanceAnnotationsPropertyShouldPass()
        {
            ICollection<ODataInstanceAnnotation> initialCollection = this.odataFeed.InstanceAnnotations;
            ICollection<ODataInstanceAnnotation> newCollection = new List<ODataInstanceAnnotation>();
            this.odataFeed.InstanceAnnotations = newCollection;
            Assert.Same(this.odataFeed.InstanceAnnotations, newCollection);
            Assert.NotSame(this.odataFeed.InstanceAnnotations, initialCollection);
        }

        [Fact]
        public void DeltaLinkPropertyShouldBeNullAtCreation()
        {
            Assert.Null(this.odataFeed.DeltaLink);
        }

        [Fact]
        public void ShouldBeAbleToSetDeltaLinkValue()
        {
            Uri deltaLink = new Uri("http://www.example.com/deltaLink");
            this.odataFeed.DeltaLink = deltaLink;
            Assert.Same(deltaLink, this.odataFeed.DeltaLink);
        }

        [Fact]
        public void SettingDeltaLinkWhenNextPageLinkIsAlreadySetShouldThrow()
        {
            this.odataFeed.NextPageLink = new Uri("http://www.example.com/nextPageLink");
            Action test = () => this.odataFeed.DeltaLink = new Uri("http://www.example.com/deltaLink");
            test.Throws<ODataException>(Strings.ODataResourceSet_MustNotContainBothNextPageLinkAndDeltaLink);
        }

        [Fact]
        public void SettingNextPageLinkWhenDeltaLinkIsAlreadySetShouldThrow()
        {
            this.odataFeed.DeltaLink = new Uri("http://www.example.com/deltaLink");
            Action test = () => this.odataFeed.NextPageLink = new Uri("http://www.example.com/nextPageLink");
            test.Throws<ODataException>(Strings.ODataResourceSet_MustNotContainBothNextPageLinkAndDeltaLink);
        }

        [Fact]
        public void NewODataFeedShouldContainNullSerializationInfo()
        {
            Assert.Null(this.odataFeed.SerializationInfo);
        }

        [Fact]
        public void ShouldBeAbleToSetSerializationInfo()
        {
            this.odataFeed.SerializationInfo = new ODataResourceSerializationInfo { NavigationSourceName = "Set", NavigationSourceEntityTypeName = "ns.base", ExpectedTypeName = "ns.expected" };
            Assert.Equal("Set", this.odataFeed.SerializationInfo.NavigationSourceName);
        }

        [Fact]
        public void ShouldBeAbleToSetSerializationInfoWithEdmUnknowEntitySet()
        {
            this.odataFeed.SerializationInfo = new ODataResourceSerializationInfo { NavigationSourceName = null, NavigationSourceKind = EdmNavigationSourceKind.UnknownEntitySet, NavigationSourceEntityTypeName = "ns.base", ExpectedTypeName = "ns.expected" };
            Assert.Null(this.odataFeed.SerializationInfo.NavigationSourceName);
        }
    }
}

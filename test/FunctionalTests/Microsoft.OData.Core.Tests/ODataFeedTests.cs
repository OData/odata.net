//---------------------------------------------------------------------
// <copyright file="ODataFeedTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Core.Tests
{
    public class ODataFeedTests
    {
        private ODataFeed odataFeed;

        public ODataFeedTests()
        {
            this.odataFeed = new ODataFeed();
        }

        [Fact]
        public void InstanceAnnotationsPropertyShouldNotBeNullAtCreation()
        {
            this.odataFeed.InstanceAnnotations.Should().NotBeNull();
        }

        [Fact]
        public void InstanceAnnotationsPropertyShouldReturnAWritableCollectionAtCreation()
        {
            this.odataFeed.InstanceAnnotations.Should().NotBeNull();
            this.odataFeed.InstanceAnnotations.Add(new ODataInstanceAnnotation("namespace.name", new ODataPrimitiveValue("value")));
            this.odataFeed.InstanceAnnotations.Count.Should().Be(1);
        }

        [Fact]
        public void SetNullValueToInstanceAnnotationsPropertyShouldThrow()
        {
            Action test = () => this.odataFeed.InstanceAnnotations = null;
            test.ShouldThrow<ArgumentException>().WithMessage("Value cannot be null.\r\nParameter name: value");
        }

        [Fact]
        public void SetListValueToInstanceAnnotationsPropertyShouldPass()
        {
            ICollection<ODataInstanceAnnotation> initialCollection = this.odataFeed.InstanceAnnotations;
            ICollection<ODataInstanceAnnotation> newCollection = new List<ODataInstanceAnnotation>();
            this.odataFeed.InstanceAnnotations = newCollection;
            this.odataFeed.InstanceAnnotations.As<object>().Should().BeSameAs(newCollection).And.NotBeSameAs(initialCollection);
        }

        [Fact]
        public void DeltaLinkPropertyShouldBeNullAtCreation()
        {
            this.odataFeed.DeltaLink.Should().BeNull();
        }

        [Fact]
        public void ShouldBeAbleToSetDeltaLinkValue()
        {
            Uri deltaLink = new Uri("http://www.example.com/deltaLink");
            this.odataFeed.DeltaLink = deltaLink;
            this.odataFeed.DeltaLink.Should().BeSameAs(deltaLink);
        }

        [Fact]
        public void SettingDeltaLinkWhenNextPageLinkIsAlreadySetShouldThrow()
        {
            this.odataFeed.NextPageLink = new Uri("http://www.example.com/nextPageLink");
            Action test = () => this.odataFeed.DeltaLink = new Uri("http://www.example.com/deltaLink");
            test.ShouldThrow<ODataException>().WithMessage(Strings.ODataFeed_MustNotContainBothNextPageLinkAndDeltaLink);
        }

        [Fact]
        public void SettingNextPageLinkWhenDeltaLinkIsAlreadySetShouldThrow()
        {
            this.odataFeed.DeltaLink = new Uri("http://www.example.com/deltaLink");
            Action test = () => this.odataFeed.NextPageLink = new Uri("http://www.example.com/nextPageLink");
            test.ShouldThrow<ODataException>().WithMessage(Strings.ODataFeed_MustNotContainBothNextPageLinkAndDeltaLink);
        }

        [Fact]
        public void NewODataFeedShouldContainNullSerializationInfo()
        {
            this.odataFeed.SerializationInfo.Should().BeNull();
        }

        [Fact]
        public void SerializationInfoShouldBeValidatedByTheSetter()
        {
            Action action = () => this.odataFeed.SerializationInfo = new ODataFeedAndEntrySerializationInfo();
            action.ShouldThrow<ArgumentNullException>().WithMessage("serializationInfo.NavigationSourceName", ComparisonMode.Substring);
        }

        [Fact]
        public void ShouldBeAbleToSetSerializationInfo()
        {
            this.odataFeed.SerializationInfo = new ODataFeedAndEntrySerializationInfo { NavigationSourceName = "Set", NavigationSourceEntityTypeName = "ns.base", ExpectedTypeName = "ns.expected" };
            this.odataFeed.SerializationInfo.NavigationSourceName.Should().Be("Set");
        }

        [Fact]
        public void ShouldBeAbleToSetSerializationInfoWithEdmUnknowEntitySet()
        {
            this.odataFeed.SerializationInfo = new ODataFeedAndEntrySerializationInfo { NavigationSourceName = null, NavigationSourceKind = EdmNavigationSourceKind.UnknownEntitySet, NavigationSourceEntityTypeName = "ns.base", ExpectedTypeName = "ns.expected" };
            this.odataFeed.SerializationInfo.NavigationSourceName.Should().BeNull();
        }
    }
}

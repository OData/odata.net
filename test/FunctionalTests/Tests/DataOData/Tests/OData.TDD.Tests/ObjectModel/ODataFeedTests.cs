//---------------------------------------------------------------------
// <copyright file="ODataFeedTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.ObjecetModel
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ODataFeedTests
    {
        private ODataFeed odataFeed;

        [TestInitialize]
        public void InitTest()
        {
            this.odataFeed = new ODataFeed();
        }

        [TestMethod]
        public void InstanceAnnotationsPropertyShouldNotBeNullAtCreation()
        {
            this.odataFeed.InstanceAnnotations.Should().NotBeNull();
        }

        [TestMethod]
        public void InstanceAnnotationsPropertyShouldReturnAWritableCollectionAtCreation()
        {
            this.odataFeed.InstanceAnnotations.Should().NotBeNull();
            this.odataFeed.InstanceAnnotations.Add(new ODataInstanceAnnotation("namespace.name", new ODataPrimitiveValue("value")));
            this.odataFeed.InstanceAnnotations.Count.Should().Be(1);
        }

        [TestMethod]
        public void SetNullValueToInstanceAnnotationsPropertyShouldThrow()
        {
            Action test = () => this.odataFeed.InstanceAnnotations = null;
            test.ShouldThrow<ArgumentException>().WithMessage("Value cannot be null.\r\nParameter name: value");
        }

        [TestMethod]
        public void SetListValueToInstanceAnnotationsPropertyShouldPass()
        {
            ICollection<ODataInstanceAnnotation> initialCollection = this.odataFeed.InstanceAnnotations;
            ICollection<ODataInstanceAnnotation> newCollection = new List<ODataInstanceAnnotation>();
            this.odataFeed.InstanceAnnotations = newCollection;
            this.odataFeed.InstanceAnnotations.As<object>().Should().BeSameAs(newCollection).And.NotBeSameAs(initialCollection);
        }

        [TestMethod]
        public void DeltaLinkPropertyShouldBeNullAtCreation()
        {
            this.odataFeed.DeltaLink.Should().BeNull();
        }

        [TestMethod]
        public void ShouldBeAbleToSetDeltaLinkValue()
        {
            Uri deltaLink = new Uri("http://www.example.com/deltaLink");
            this.odataFeed.DeltaLink = deltaLink;
            this.odataFeed.DeltaLink.Should().BeSameAs(deltaLink);
        }

        [TestMethod]
        public void SettingDeltaLinkWhenNextPageLinkIsAlreadySetShouldThrow()
        {
            this.odataFeed.NextPageLink = new Uri("http://www.example.com/nextPageLink");
            Action test = () => this.odataFeed.DeltaLink = new Uri("http://www.example.com/deltaLink");
            test.ShouldThrow<ODataException>().WithMessage(Strings.ODataFeed_MustNotContainBothNextPageLinkAndDeltaLink);
        }

        [TestMethod]
        public void SettingNextPageLinkWhenDeltaLinkIsAlreadySetShouldThrow()
        {
            this.odataFeed.DeltaLink = new Uri("http://www.example.com/deltaLink");
            Action test = () => this.odataFeed.NextPageLink = new Uri("http://www.example.com/nextPageLink");
            test.ShouldThrow<ODataException>().WithMessage(Strings.ODataFeed_MustNotContainBothNextPageLinkAndDeltaLink);
        }

        [TestMethod]
        public void NewODataFeedShouldContainNullSerializationInfo()
        {
            this.odataFeed.SerializationInfo.Should().BeNull();
        }

        [TestMethod]
        public void SerializationInfoShouldBeValidatedByTheSetter()
        {
            Action action = () => this.odataFeed.SerializationInfo = new ODataFeedAndEntrySerializationInfo();
            action.ShouldThrow<ArgumentNullException>().WithMessage("serializationInfo.NavigationSourceName", ComparisonMode.Substring);
        }

        [TestMethod]
        public void ShouldBeAbleToSetSerializationInfo()
        {
            this.odataFeed.SerializationInfo = new ODataFeedAndEntrySerializationInfo { NavigationSourceName = "Set", NavigationSourceEntityTypeName = "ns.base", ExpectedTypeName = "ns.expected"};
            this.odataFeed.SerializationInfo.NavigationSourceName.Should().Be("Set");
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="ODataFeedAndEntrySerializationInfoTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Core.Tests
{
    public class ODataFeedAndEntrySerializationInfoTests
    {
        private ODataFeedAndEntrySerializationInfo testSubject;

        public ODataFeedAndEntrySerializationInfoTests()
        {
            this.testSubject = new ODataFeedAndEntrySerializationInfo();
        }

        [Fact]
        public void AllPropertiesShouldBeNullOrFalseOnCreation()
        {
            this.testSubject.NavigationSourceName.Should().BeNull();
            this.testSubject.NavigationSourceEntityTypeName.Should().BeNull();
            this.testSubject.ExpectedTypeName.Should().BeNull();
        }

        [Fact]
        public void SettingNullEntitySetNameShouldNotThrow()
        {
            this.testSubject.NavigationSourceName = null;
            this.testSubject.NavigationSourceName.Should().Be(null);
        }

        [Fact]
        public void SettingEmptyEntitySetNameShouldNotThrow()
        {
            this.testSubject.NavigationSourceName = string.Empty;
            this.testSubject.NavigationSourceName.Should().Be(string.Empty);
        }

        [Fact]
        public void ShouldBeAbleToSetEntitySetName()
        {
            this.testSubject.NavigationSourceName = "Set";
            this.testSubject.NavigationSourceName.Should().Be("Set");
        }

        [Fact]
        public void SettingNullBaseTypeNameShouldThrow()
        {
            Action action = () => this.testSubject.NavigationSourceEntityTypeName = null;
            action.ShouldThrow<ArgumentNullException>().WithMessage("NavigationSourceEntityTypeName", ComparisonMode.Substring);
        }

        [Fact]
        public void SettingEmptyBaseTypeNameShouldThrow()
        {
            Action action = () => this.testSubject.NavigationSourceEntityTypeName = "";
            action.ShouldThrow<ArgumentNullException>().WithMessage("NavigationSourceEntityTypeName", ComparisonMode.Substring);
        }

        [Fact]
        public void ShouldBeAbleToSetBaseTypeName()
        {
            this.testSubject.NavigationSourceEntityTypeName = "NavigationSourceEntityTypeName";
            this.testSubject.NavigationSourceEntityTypeName.Should().Be("NavigationSourceEntityTypeName");
        }

        [Fact]
        public void ShouldBeAbleToSetNullExpectedTypeName()
        {
            this.testSubject.ExpectedTypeName = null;
            this.testSubject.ExpectedTypeName.Should().BeNull();
        }

        [Fact]
        public void SettingEmptyExpectedTypeNameShouldThrow()
        {
            Action action = () => this.testSubject.ExpectedTypeName = "";
            action.ShouldThrow<ArgumentException>().WithMessage("ExpectedTypeName", ComparisonMode.Substring);
        }

        [Fact]
        public void ShouldBeAbleToSetExpectedTypeName()
        {
            this.testSubject.ExpectedTypeName = "ExpectedTypeName";
            this.testSubject.ExpectedTypeName.Should().Be("ExpectedTypeName");
        }

        [Fact]
        public void ExpectedTypeNameShouldBeDefaultToBaseTypeName()
        {
            this.testSubject.NavigationSourceEntityTypeName = "EntitySetElementTypeName";
            this.testSubject.ExpectedTypeName.Should().Be("EntitySetElementTypeName");
        }

        [Fact]
        public void ValidateNullSerializationInfoShouldReturnNull()
        {
            ODataFeedAndEntrySerializationInfo.Validate(null).Should().BeNull();
        }

        [Fact]
        public void ValidatingSerializationInfoShouldThrowIfEntitySetNameNotSet()
        {
            Action action = () => ODataFeedAndEntrySerializationInfo.Validate(new ODataFeedAndEntrySerializationInfo());
            action.ShouldThrow<ArgumentNullException>().WithMessage("serializationInfo.NavigationSourceName", ComparisonMode.Substring);
        }

        [Fact]
        public void ValidatingSerializationInfoShouldThrowIfBaseTypeNameNotSet()
        {
            Action action = () => ODataFeedAndEntrySerializationInfo.Validate(new ODataFeedAndEntrySerializationInfo { NavigationSourceName = "Set" });
            action.ShouldThrow<ArgumentNullException>().WithMessage("serializationInfo.NavigationSourceEntityTypeName", ComparisonMode.Substring);
        }

        [Fact]
        public void ValidatingSerializationInfoShouldAllowExpectedTypeNameNotSet()
        {
            ODataFeedAndEntrySerializationInfo.Validate(new ODataFeedAndEntrySerializationInfo { NavigationSourceName = "Set", NavigationSourceEntityTypeName = "EntitySetElementTypeName" }).Should().NotBeNull();
        }

        [Fact]
        public void ValdatingSerializationInfoShouldAllowIfEntitySetNameNotSetWithEdmUnknownEntitySet()
        {
            ODataFeedAndEntrySerializationInfo.Validate(new ODataFeedAndEntrySerializationInfo()
            {
                ExpectedTypeName = "NS.Type",
                IsFromCollection = true,
                NavigationSourceEntityTypeName = "NS.Type",
                NavigationSourceKind = EdmNavigationSourceKind.UnknownEntitySet,
                NavigationSourceName = null
            });
        }
    }
}

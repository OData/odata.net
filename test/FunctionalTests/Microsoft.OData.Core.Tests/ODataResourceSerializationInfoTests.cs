//---------------------------------------------------------------------
// <copyright file="ODataResourceSerializationInfoTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class ODataResourceSerializationInfoTests
    {
        private ODataResourceSerializationInfo testSubject;

        public ODataResourceSerializationInfoTests()
        {
            this.testSubject = new ODataResourceSerializationInfo();
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
            action.ShouldThrow<ArgumentNullException>().Where(e => e.Message.Contains("NavigationSourceEntityTypeName"));
        }

        [Fact]
        public void SettingEmptyBaseTypeNameShouldThrow()
        {
            Action action = () => this.testSubject.NavigationSourceEntityTypeName = "";
            action.ShouldThrow<ArgumentNullException>().Where(e => e.Message.Contains("NavigationSourceEntityTypeName"));
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
            action.ShouldThrow<ArgumentException>().Where(e => e.Message.Contains("ExpectedTypeName"));
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
            ODataResourceSerializationInfo.Validate(null).Should().BeNull();
        }

        [Fact]
        public void ValidatingSerializationInfoShouldThrowIfEntitySetNameNotSet()
        {
            Action action = () => ODataResourceSerializationInfo.Validate(
                new ODataResourceSerializationInfo() { NavigationSourceKind = EdmNavigationSourceKind.EntitySet });
            action.ShouldThrow<ArgumentNullException>().Where(e => e.Message.Contains("serializationInfo.NavigationSourceName"));
        }

        [Fact]
        public void ValidatingSerializationInfoShouldThrowIfBaseTypeNameNotSet()
        {
            Action action = () => ODataResourceSerializationInfo.Validate(new ODataResourceSerializationInfo { NavigationSourceName = "Set", NavigationSourceKind = EdmNavigationSourceKind.EntitySet });
            action.ShouldThrow<ArgumentNullException>().Where(e => e.Message.Contains("serializationInfo.NavigationSourceEntityTypeName"));
        }

        [Fact]
        public void ValidatingSerializationInfoShouldAllowExpectedTypeNameNotSet()
        {
            ODataResourceSerializationInfo.Validate(new ODataResourceSerializationInfo { NavigationSourceName = "Set", NavigationSourceEntityTypeName = "EntitySetElementTypeName" }).Should().NotBeNull();
        }

        [Fact]
        public void ValdatingSerializationInfoShouldAllowIfEntitySetNameNotSetWithEdmUnknownEntitySet()
        {
            ODataResourceSerializationInfo.Validate(new ODataResourceSerializationInfo()
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

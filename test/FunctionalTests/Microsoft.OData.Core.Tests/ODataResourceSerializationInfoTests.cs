//---------------------------------------------------------------------
// <copyright file="ODataResourceSerializationInfoTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
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
            Assert.Null(this.testSubject.NavigationSourceName);
            Assert.Null(this.testSubject.NavigationSourceEntityTypeName);
            Assert.Null(this.testSubject.ExpectedTypeName);
        }

        [Fact]
        public void SettingNullEntitySetNameShouldNotThrow()
        {
            this.testSubject.NavigationSourceName = null;
            Assert.Null(this.testSubject.NavigationSourceName);
        }

        [Fact]
        public void SettingEmptyEntitySetNameShouldNotThrow()
        {
            this.testSubject.NavigationSourceName = string.Empty;
            Assert.Equal(string.Empty, this.testSubject.NavigationSourceName);
        }

        [Fact]
        public void ShouldBeAbleToSetEntitySetName()
        {
            this.testSubject.NavigationSourceName = "Set";
            Assert.Equal("Set", this.testSubject.NavigationSourceName);
        }

        [Fact]
        public void SettingNullBaseTypeNameShouldThrow()
        {
            Action action = () => this.testSubject.NavigationSourceEntityTypeName = null;
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(action);
            Assert.Contains("NavigationSourceEntityTypeName", exception.Message);
        }

        [Fact]
        public void SettingEmptyBaseTypeNameShouldThrow()
        {
            Action action = () => this.testSubject.NavigationSourceEntityTypeName = "";
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(action);
            Assert.Contains("NavigationSourceEntityTypeName", exception.Message);
        }

        [Fact]
        public void ShouldBeAbleToSetBaseTypeName()
        {
            this.testSubject.NavigationSourceEntityTypeName = "NavigationSourceEntityTypeName";
            Assert.Equal("NavigationSourceEntityTypeName", this.testSubject.NavigationSourceEntityTypeName);
        }

        [Fact]
        public void ShouldBeAbleToSetNullExpectedTypeName()
        {
            this.testSubject.ExpectedTypeName = null;
            Assert.Null(this.testSubject.ExpectedTypeName);
        }

        [Fact]
        public void SettingEmptyExpectedTypeNameShouldThrow()
        {
            Action action = () => this.testSubject.ExpectedTypeName = "";
            ArgumentException exception = Assert.Throws<ArgumentException>(action);
            Assert.Contains("ExpectedTypeName", exception.Message);
        }

        [Fact]
        public void ShouldBeAbleToSetExpectedTypeName()
        {
            this.testSubject.ExpectedTypeName = "ExpectedTypeName";
            Assert.Equal("ExpectedTypeName", this.testSubject.ExpectedTypeName);
        }

        [Fact]
        public void ExpectedTypeNameShouldBeDefaultToBaseTypeName()
        {
            this.testSubject.NavigationSourceEntityTypeName = "EntitySetElementTypeName";
            Assert.Equal("EntitySetElementTypeName", this.testSubject.ExpectedTypeName);
        }

        [Fact]
        public void ValidateNullSerializationInfoShouldReturnNull()
        {
            Assert.Null(ODataResourceSerializationInfo.Validate(null));
        }

        [Fact]
        public void ValidatingSerializationInfoShouldThrowIfEntitySetNameNotSet()
        {
            Action action = () => ODataResourceSerializationInfo.Validate(
                new ODataResourceSerializationInfo() { NavigationSourceKind = EdmNavigationSourceKind.EntitySet });

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(action);
            Assert.Contains("serializationInfo.NavigationSourceName", exception.Message);
        }

        [Fact]
        public void ValidatingSerializationInfoShouldThrowIfBaseTypeNameNotSet()
        {
            Action action = () => ODataResourceSerializationInfo.Validate(new ODataResourceSerializationInfo { NavigationSourceName = "Set", NavigationSourceKind = EdmNavigationSourceKind.EntitySet });

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(action);
            Assert.Contains("serializationInfo.NavigationSourceEntityTypeName", exception.Message);
        }

        [Fact]
        public void ValidatingSerializationInfoShouldAllowExpectedTypeNameNotSet()
        {
            Assert.NotNull(ODataResourceSerializationInfo.Validate(new ODataResourceSerializationInfo { NavigationSourceName = "Set", NavigationSourceEntityTypeName = "EntitySetElementTypeName" }));
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

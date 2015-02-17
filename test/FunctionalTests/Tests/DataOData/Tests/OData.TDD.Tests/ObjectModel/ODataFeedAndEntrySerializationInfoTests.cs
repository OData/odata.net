//---------------------------------------------------------------------
// <copyright file="ODataFeedAndEntrySerializationInfoTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.ObjecetModel
{
    using System;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ODataFeedAndEntrySerializationInfoTests
    {
        private ODataFeedAndEntrySerializationInfo testSubject;

        [TestInitialize]
        public void TestInit()
        {
            this.testSubject = new ODataFeedAndEntrySerializationInfo();
        }

        [TestMethod]
        public void AllPropertiesShouldBeNullOrFalseOnCreation()
        {
            this.testSubject.NavigationSourceName.Should().BeNull();
            this.testSubject.NavigationSourceEntityTypeName.Should().BeNull();
            this.testSubject.ExpectedTypeName.Should().BeNull();
        }

        [TestMethod]
        public void SettingNullEntitySetNameShouldThrow()
        {
            Action action = () => this.testSubject.NavigationSourceName = null;
            action.ShouldThrow<ArgumentNullException>().WithMessage("NavigationSourceName", ComparisonMode.Substring);
        }

        [TestMethod]
        public void SettingEmptyEntitySetNameShouldThrow()
        {
            Action action = () => this.testSubject.NavigationSourceName = "";
            action.ShouldThrow<ArgumentNullException>().WithMessage("NavigationSourceName", ComparisonMode.Substring);
        }

        [TestMethod]
        public void ShouldBeAbleToSetEntitySetName()
        {
            this.testSubject.NavigationSourceName = "Set";
            this.testSubject.NavigationSourceName.Should().Be("Set");
        }

        [TestMethod]
        public void SettingNullBaseTypeNameShouldThrow()
        {
            Action action = () => this.testSubject.NavigationSourceEntityTypeName = null;
            action.ShouldThrow<ArgumentNullException>().WithMessage("NavigationSourceEntityTypeName", ComparisonMode.Substring);
        }

        [TestMethod]
        public void SettingEmptyBaseTypeNameShouldThrow()
        {
            Action action = () => this.testSubject.NavigationSourceEntityTypeName = "";
            action.ShouldThrow<ArgumentNullException>().WithMessage("NavigationSourceEntityTypeName", ComparisonMode.Substring);
        }

        [TestMethod]
        public void ShouldBeAbleToSetBaseTypeName()
        {
            this.testSubject.NavigationSourceEntityTypeName = "NavigationSourceEntityTypeName";
            this.testSubject.NavigationSourceEntityTypeName.Should().Be("NavigationSourceEntityTypeName");
        }

        [TestMethod]
        public void ShouldBeAbleToSetNullExpectedTypeName()
        {
            this.testSubject.ExpectedTypeName = null;
            this.testSubject.ExpectedTypeName.Should().BeNull();
        }

        [TestMethod]
        public void SettingEmptyExpectedTypeNameShouldThrow()
        {
            Action action = () => this.testSubject.ExpectedTypeName = "";
            action.ShouldThrow<ArgumentException>().WithMessage("ExpectedTypeName", ComparisonMode.Substring);
        }

        [TestMethod]
        public void ShouldBeAbleToSetExpectedTypeName()
        {
            this.testSubject.ExpectedTypeName = "ExpectedTypeName";
            this.testSubject.ExpectedTypeName.Should().Be("ExpectedTypeName");
        }

        [TestMethod]
        public void ExpectedTypeNameShouldBeDefaultToBaseTypeName()
        {
            this.testSubject.NavigationSourceEntityTypeName = "EntitySetElementTypeName";
            this.testSubject.ExpectedTypeName.Should().Be("EntitySetElementTypeName");
        }

        [TestMethod]
        public void ValidateNullSerializationInfoShouldReturnNull()
        {
            ODataFeedAndEntrySerializationInfo.Validate(null).Should().BeNull();
        }

        [TestMethod]
        public void ValidatingSerializationInfoShouldThrowIfEntitySetNameNotSet()
        {
            Action action = () => ODataFeedAndEntrySerializationInfo.Validate(new ODataFeedAndEntrySerializationInfo());
            action.ShouldThrow<ArgumentNullException>().WithMessage("serializationInfo.NavigationSourceName", ComparisonMode.Substring);
        }

        [TestMethod]
        public void ValidatingSerializationInfoShouldThrowIfBaseTypeNameNotSet()
        {
            Action action = () => ODataFeedAndEntrySerializationInfo.Validate(new ODataFeedAndEntrySerializationInfo { NavigationSourceName = "Set" });
            action.ShouldThrow<ArgumentNullException>().WithMessage("serializationInfo.NavigationSourceEntityTypeName", ComparisonMode.Substring);
        }

        [TestMethod]
        public void ValidatingSerializationInfoShouldAllowExpectedTypeNameNotSet()
        {
            ODataFeedAndEntrySerializationInfo.Validate(new ODataFeedAndEntrySerializationInfo { NavigationSourceName = "Set", NavigationSourceEntityTypeName = "EntitySetElementTypeName"}).Should().NotBeNull();
        }
    }
}

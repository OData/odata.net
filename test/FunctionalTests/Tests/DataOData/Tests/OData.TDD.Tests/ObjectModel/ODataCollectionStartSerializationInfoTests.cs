//---------------------------------------------------------------------
// <copyright file="ODataCollectionStartSerializationInfoTests.cs" company="Microsoft">
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
    public class ODataCollectionStartSerializationInfoTests
    {
        private ODataCollectionStartSerializationInfo testSubject;

        [TestInitialize]
        public void TestInit()
        {
            this.testSubject = new ODataCollectionStartSerializationInfo();
        }

        [TestMethod]
        public void AllPropertiesShouldBeNullOnCreation()
        {
            this.testSubject.CollectionTypeName.Should().BeNull();
        }

        [TestMethod]
        public void SettingNullCollectionTypeNameShouldThrow()
        {
            Action action = () => this.testSubject.CollectionTypeName = null;
            action.ShouldThrow<ArgumentNullException>().WithMessage("CollectionTypeName", ComparisonMode.Substring);
        }

        [TestMethod]
        public void SettingEmptyCollectionTypeNameShouldThrow()
        {
            Action action = () => this.testSubject.CollectionTypeName = "";
            action.ShouldThrow<ArgumentNullException>().WithMessage("CollectionTypeName", ComparisonMode.Substring);
        }

        [TestMethod]
        public void SettingTypeNameWithoutCollectionWrapperToCollectionTypeNameShouldThrow()
        {
            Action action = () => this.testSubject.CollectionTypeName = "Edm.String";
            action.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_InvalidCollectionTypeName("Edm.String"));
        }

        [TestMethod]
        public void ShouldBeAbleToSetCollectionTypeName()
        {
            this.testSubject.CollectionTypeName = "Collection(ns.foo)";
            this.testSubject.CollectionTypeName.Should().Be("Collection(ns.foo)");
        }

        [TestMethod]
        public void ValidateNullSerializationInfoShouldReturnNull()
        {
            ODataCollectionStartSerializationInfo.Validate(null).Should().BeNull();
        }

        [TestMethod]
        public void ValidatingSerializationInfoShouldThrowIfCollectionTypeNameNotSet()
        {
            Action action = () => ODataCollectionStartSerializationInfo.Validate(new ODataCollectionStartSerializationInfo());
            action.ShouldThrow<ArgumentNullException>().WithMessage("serializationInfo.CollectionTypeName", ComparisonMode.Substring);
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="ODataCollectionStartSerializationInfoTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Xunit;

namespace Microsoft.OData.Core.Tests
{
    public class ODataCollectionStartSerializationInfoTests
    {
        private ODataCollectionStartSerializationInfo testSubject;

        public ODataCollectionStartSerializationInfoTests()
        {
            this.testSubject = new ODataCollectionStartSerializationInfo();
        }

        [Fact]
        public void AllPropertiesShouldBeNullOnCreation()
        {
            this.testSubject.CollectionTypeName.Should().BeNull();
        }

        [Fact]
        public void SettingNullCollectionTypeNameShouldThrow()
        {
            Action action = () => this.testSubject.CollectionTypeName = null;
            action.ShouldThrow<ArgumentNullException>().WithMessage("CollectionTypeName", ComparisonMode.Substring);
        }

        [Fact]
        public void SettingEmptyCollectionTypeNameShouldThrow()
        {
            Action action = () => this.testSubject.CollectionTypeName = "";
            action.ShouldThrow<ArgumentNullException>().WithMessage("CollectionTypeName", ComparisonMode.Substring);
        }

        [Fact]
        public void SettingTypeNameWithoutCollectionWrapperToCollectionTypeNameShouldThrow()
        {
            Action action = () => this.testSubject.CollectionTypeName = "Edm.String";
            action.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_InvalidCollectionTypeName("Edm.String"));
        }

        [Fact]
        public void ShouldBeAbleToSetCollectionTypeName()
        {
            this.testSubject.CollectionTypeName = "Collection(ns.foo)";
            this.testSubject.CollectionTypeName.Should().Be("Collection(ns.foo)");
        }

        [Fact]
        public void ValidateNullSerializationInfoShouldReturnNull()
        {
            ODataCollectionStartSerializationInfo.Validate(null).Should().BeNull();
        }

        [Fact]
        public void ValidatingSerializationInfoShouldThrowIfCollectionTypeNameNotSet()
        {
            Action action = () => ODataCollectionStartSerializationInfo.Validate(new ODataCollectionStartSerializationInfo());
            action.ShouldThrow<ArgumentNullException>().WithMessage("serializationInfo.CollectionTypeName", ComparisonMode.Substring);
        }
    }
}

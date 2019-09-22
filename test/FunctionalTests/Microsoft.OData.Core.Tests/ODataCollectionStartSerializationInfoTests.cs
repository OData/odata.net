//---------------------------------------------------------------------
// <copyright file="ODataCollectionStartSerializationInfoTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Xunit;

namespace Microsoft.OData.Tests
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
            Assert.Null(this.testSubject.CollectionTypeName);
        }

        [Fact]
        public void SettingNullCollectionTypeNameShouldThrow()
        {
            Action action = () => this.testSubject.CollectionTypeName = null;
            Assert.Throws<ArgumentNullException>("CollectionTypeName", action);
        }

        [Fact]
        public void SettingEmptyCollectionTypeNameShouldThrow()
        {
            Action action = () => this.testSubject.CollectionTypeName = "";
            Assert.Throws<ArgumentNullException>("CollectionTypeName", action);
        }

        [Fact]
        public void SettingTypeNameWithoutCollectionWrapperToCollectionTypeNameShouldThrow()
        {
            Action action = () => this.testSubject.CollectionTypeName = "Edm.String";
            action.Throws<ODataException>(Strings.ValidationUtils_InvalidCollectionTypeName("Edm.String"));
        }

        [Fact]
        public void ShouldBeAbleToSetCollectionTypeName()
        {
            this.testSubject.CollectionTypeName = "Collection(ns.foo)";
            Assert.Equal("Collection(ns.foo)", this.testSubject.CollectionTypeName);
        }

        [Fact]
        public void ValidateNullSerializationInfoShouldReturnNull()
        {
            Assert.Null(ODataCollectionStartSerializationInfo.Validate(null));
        }

        [Fact]
        public void ValidatingSerializationInfoShouldThrowIfCollectionTypeNameNotSet()
        {
            Action action = () => ODataCollectionStartSerializationInfo.Validate(new ODataCollectionStartSerializationInfo());
            Assert.Throws<ArgumentNullException>("serializationInfo.CollectionTypeName", action);
        }
    }
}

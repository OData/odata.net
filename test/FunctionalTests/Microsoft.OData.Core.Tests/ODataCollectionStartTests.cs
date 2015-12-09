//---------------------------------------------------------------------
// <copyright file="ODataCollectionStartTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Xunit;

namespace Microsoft.OData.Core.Tests
{
    public class ODataCollectionStartTests
    {
        private ODataCollectionStart collectionStart;

        public ODataCollectionStartTests()
        {
            this.collectionStart = new ODataCollectionStart();
        }

        [Fact]
        public void NewODataCollectionStartShouldContainNullSerializationInfo()
        {
            this.collectionStart.SerializationInfo.Should().BeNull();
        }

        [Fact]
        public void SerializationInfoShouldBeValidatedByTheSetter()
        {
            Action action = () => this.collectionStart.SerializationInfo = new ODataCollectionStartSerializationInfo();
            action.ShouldThrow<ArgumentNullException>().WithMessage("serializationInfo.CollectionTypeName", ComparisonMode.Substring);
        }

        [Fact]
        public void ShouldBeAbleToSetSerializationInfo()
        {
            this.collectionStart.SerializationInfo = new ODataCollectionStartSerializationInfo { CollectionTypeName = "Collection(Edm.String)" };
            this.collectionStart.SerializationInfo.CollectionTypeName.Should().Be("Collection(Edm.String)");
        }
    }
}

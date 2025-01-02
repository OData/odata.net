//---------------------------------------------------------------------
// <copyright file="ODataCollectionStartTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Xunit;

namespace Microsoft.OData.Tests
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
            Assert.Null(this.collectionStart.SerializationInfo);
        }

        [Fact]
        public void SerializationInfoShouldBeValidatedByTheSetter()
        {
            Action action = () => this.collectionStart.SerializationInfo = new ODataCollectionStartSerializationInfo();
            Assert.Throws<ArgumentNullException>("serializationInfo.CollectionTypeName", action);
        }

        [Fact]
        public void ShouldBeAbleToSetSerializationInfo()
        {
            this.collectionStart.SerializationInfo = new ODataCollectionStartSerializationInfo { CollectionTypeName = "Collection(Edm.String)" };
            Assert.Equal("Collection(Edm.String)", this.collectionStart.SerializationInfo.CollectionTypeName);
        }
    }
}

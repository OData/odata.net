//---------------------------------------------------------------------
// <copyright file="ODataCollectionStartTests.cs" company="Microsoft">
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
    public class ODataCollectionStartTests
    {
        private ODataCollectionStart collectionStart;

        [TestInitialize]
        public void InitTest()
        {
            this.collectionStart = new ODataCollectionStart();
        }

        [TestMethod]
        public void NewODataCollectionStartShouldContainNullSerializationInfo()
        {
            this.collectionStart.SerializationInfo.Should().BeNull();
        }

        [TestMethod]
        public void SerializationInfoShouldBeValidatedByTheSetter()
        {
            Action action = () => this.collectionStart.SerializationInfo = new ODataCollectionStartSerializationInfo();
            action.ShouldThrow<ArgumentNullException>().WithMessage("serializationInfo.CollectionTypeName", ComparisonMode.Substring);
        }

        [TestMethod]
        public void ShouldBeAbleToSetSerializationInfo()
        {
            this.collectionStart.SerializationInfo = new ODataCollectionStartSerializationInfo { CollectionTypeName = "Collection(Edm.String)" };
            this.collectionStart.SerializationInfo.CollectionTypeName.Should().Be("Collection(Edm.String)");
        }
    }
}

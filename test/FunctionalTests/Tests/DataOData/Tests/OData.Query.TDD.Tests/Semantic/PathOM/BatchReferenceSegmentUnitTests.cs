//---------------------------------------------------------------------
// <copyright file="BatchReferenceSegmentUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    [TestClass]
    public class BatchReferenceSegmentTests
    {
        [TestMethod]
        public void TypeCannotBeNull()
        {
            Action createWithNullType = () => new BatchReferenceSegment("stuff", null, HardCodedTestModel.GetPeopleSet());
            createWithNullType.ShouldThrow<Exception>(Error.ArgumentNull("resultingType").ToString());
        }

        [TestMethod]
        public void ContentIdCannotBeNull()
        {
            Action createWithNullId = () => new BatchReferenceSegment(null, ModelBuildingHelpers.BuildValidEntityType(), HardCodedTestModel.GetPeopleSet());
            createWithNullId.ShouldThrow<Exception>(Error.ArgumentNull("contentId").ToString());
        }

        [TestMethod]
        public void ContentIdMustBeDollarNumber()
        {
            IEdmEntityType type = ModelBuildingHelpers.BuildValidEntityType();
            Action createWitnInvalidContentId1 = () => new BatchReferenceSegment("stuff", type, HardCodedTestModel.GetPeopleSet());
            Action createWitnInvalidContentId2 = () => new BatchReferenceSegment("1$2", type, HardCodedTestModel.GetPeopleSet());
            Action createWitnInvalidContentId3 = () => new BatchReferenceSegment("$", type, HardCodedTestModel.GetPeopleSet());
            Action createWitnInvalidContentId4 = () => new BatchReferenceSegment("$0a1", type, HardCodedTestModel.GetPeopleSet());
            createWitnInvalidContentId1.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.BatchReferenceSegment_InvalidContentID("stuff"));
            createWitnInvalidContentId2.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.BatchReferenceSegment_InvalidContentID("1$2"));
            createWitnInvalidContentId3.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.BatchReferenceSegment_InvalidContentID("$"));
            createWitnInvalidContentId4.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.BatchReferenceSegment_InvalidContentID("$0a1"));
        }

        [TestMethod]
        public void TypeIsSetCorrectly()
        {
            IEdmEntityType type = HardCodedTestModel.GetPersonType();
            BatchReferenceSegment batchReferenceSegment = new BatchReferenceSegment("$0", type, HardCodedTestModel.GetPeopleSet());
            batchReferenceSegment.ShouldBeBatchReferenceSegment(type);
            IEdmEntityType dogType = HardCodedTestModel.GetDogType();
            BatchReferenceSegment containedBatchReferenceSegment = new BatchReferenceSegment("$0", dogType, HardCodedTestModel.GetContainedDogEntitySet());
            containedBatchReferenceSegment.ShouldBeBatchReferenceSegment(dogType);
        }

        [TestMethod]
        public void ContentIDSetCorrectly()
        {
            IEdmEntityType type = HardCodedTestModel.GetPersonType();
            BatchReferenceSegment batchReferenceSegment = new BatchReferenceSegment("$40", type, HardCodedTestModel.GetPeopleSet());
            batchReferenceSegment.ShouldBeBatchReferenceSegment(type).And.ContentId.Should().Be("$40");
            IEdmEntityType dogType = HardCodedTestModel.GetDogType();
            BatchReferenceSegment containedBatchReferenceSegment = new BatchReferenceSegment("$40", dogType, HardCodedTestModel.GetContainedDogEntitySet());
            containedBatchReferenceSegment.ShouldBeBatchReferenceSegment(dogType).And.ContentId.Should().Be("$40");
        }

        [TestMethod]
        public void EntitySetSetCorrectly()
        {
            IEdmEntityType type = HardCodedTestModel.GetPersonType();
            BatchReferenceSegment batchReferenceSegment = new BatchReferenceSegment("$0", type, HardCodedTestModel.GetPeopleSet());
            batchReferenceSegment.EntitySet.Should().BeSameAs(HardCodedTestModel.GetPeopleSet());
            IEdmEntityType dogType = HardCodedTestModel.GetDogType();
            BatchReferenceSegment containedBatchReferenceSegment = new BatchReferenceSegment("$40", dogType, HardCodedTestModel.GetContainedDogEntitySet());
            containedBatchReferenceSegment.EntitySet.Should().Be(HardCodedTestModel.GetContainedDogEntitySet());
        }

        [TestMethod]
        public void EqualityIsCorrect() 
        {
            IEdmEntityType type = HardCodedTestModel.GetPersonType();
            BatchReferenceSegment batchReferenceSegment1 = new BatchReferenceSegment("$0", type, HardCodedTestModel.GetPeopleSet());
            BatchReferenceSegment batchReferenceSegment2 = new BatchReferenceSegment("$0", type, HardCodedTestModel.GetPeopleSet());
            batchReferenceSegment1.Equals(batchReferenceSegment2).Should().BeTrue();
        }

        [TestMethod]
        public void InEqualityIsCorrect()
        {
            IEdmEntityType type1 = HardCodedTestModel.GetPersonType();
            IEdmEntityType type2 = HardCodedTestModel.GetDogType();
            BatchReferenceSegment batchReferenceSegment1 = new BatchReferenceSegment("$0", type1, HardCodedTestModel.GetPeopleSet());
            BatchReferenceSegment batchReferenceSegment2 = new BatchReferenceSegment("$0", type2, HardCodedTestModel.GetDogsSet());
            BatchReferenceSegment batchReferenceSegment3 = new BatchReferenceSegment("$10", type1, HardCodedTestModel.GetPeopleSet());
            BatchReferenceSegment batchReferenceSegment4 = new BatchReferenceSegment("$10", type2, HardCodedTestModel.GetContainedDogEntitySet());
            batchReferenceSegment1.Equals(batchReferenceSegment2).Should().BeFalse();
            batchReferenceSegment1.Equals(batchReferenceSegment3).Should().BeFalse();
            batchReferenceSegment2.Equals(batchReferenceSegment4).Should().BeFalse();
        }
    }
}

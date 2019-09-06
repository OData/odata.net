//---------------------------------------------------------------------
// <copyright file="BatchReferenceSegmentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    public class BatchReferenceSegmentTests
    {
        [Fact]
        public void TypeCannotBeNull()
        {
            Action createWithNullType = () => new BatchReferenceSegment("stuff", null, HardCodedTestModel.GetPeopleSet());
            Assert.Throws<ArgumentNullException>("edmType", createWithNullType);
        }

        [Fact]
        public void ContentIdCannotBeNull()
        {
            Action createWithNullId = () => new BatchReferenceSegment(null, ModelBuildingHelpers.BuildValidEntityType(), HardCodedTestModel.GetPeopleSet());
            Assert.Throws<ArgumentNullException>("contentId", createWithNullId);
        }

        [Fact]
        public void ContentIdMustBeDollarNumber()
        {
            IEdmEntityType type = ModelBuildingHelpers.BuildValidEntityType();
            Action createWitnInvalidContentId1 = () => new BatchReferenceSegment("stuff", type, HardCodedTestModel.GetPeopleSet());
            Action createWitnInvalidContentId2 = () => new BatchReferenceSegment("1$2", type, HardCodedTestModel.GetPeopleSet());
            Action createWitnInvalidContentId3 = () => new BatchReferenceSegment("$", type, HardCodedTestModel.GetPeopleSet());
            Action createWitnInvalidContentId4 = () => new BatchReferenceSegment("$0a1", type, HardCodedTestModel.GetPeopleSet());
            createWitnInvalidContentId1.Throws<ODataException>(ODataErrorStrings.BatchReferenceSegment_InvalidContentID("stuff"));
            createWitnInvalidContentId2.Throws<ODataException>(ODataErrorStrings.BatchReferenceSegment_InvalidContentID("1$2"));
            createWitnInvalidContentId3.Throws<ODataException>(ODataErrorStrings.BatchReferenceSegment_InvalidContentID("$"));
            createWitnInvalidContentId4.Throws<ODataException>(ODataErrorStrings.BatchReferenceSegment_InvalidContentID("$0a1"));
        }

        [Fact]
        public void TypeIsSetCorrectly()
        {
            IEdmEntityType type = HardCodedTestModel.GetPersonType();
            BatchReferenceSegment batchReferenceSegment = new BatchReferenceSegment("$0", type, HardCodedTestModel.GetPeopleSet());
            batchReferenceSegment.ShouldBeBatchReferenceSegment(type);
            IEdmEntityType dogType = HardCodedTestModel.GetDogType();
            BatchReferenceSegment containedBatchReferenceSegment = new BatchReferenceSegment("$0", dogType, HardCodedTestModel.GetContainedDogEntitySet());
            containedBatchReferenceSegment.ShouldBeBatchReferenceSegment(dogType);
        }

        [Fact]
        public void ContentIDSetCorrectly()
        {
            IEdmEntityType type = HardCodedTestModel.GetPersonType();
            BatchReferenceSegment batchReferenceSegment = new BatchReferenceSegment("$40", type, HardCodedTestModel.GetPeopleSet());
            Assert.Equal("$40", batchReferenceSegment.ShouldBeBatchReferenceSegment(type).ContentId);
            IEdmEntityType dogType = HardCodedTestModel.GetDogType();
            BatchReferenceSegment containedBatchReferenceSegment = new BatchReferenceSegment("$40", dogType, HardCodedTestModel.GetContainedDogEntitySet());
            Assert.Equal("$40", containedBatchReferenceSegment.ShouldBeBatchReferenceSegment(dogType).ContentId);
        }

        [Fact]
        public void EntitySetSetCorrectly()
        {
            IEdmEntityType type = HardCodedTestModel.GetPersonType();
            BatchReferenceSegment batchReferenceSegment = new BatchReferenceSegment("$0", type, HardCodedTestModel.GetPeopleSet());
            Assert.Same(HardCodedTestModel.GetPeopleSet(), batchReferenceSegment.EntitySet);
            IEdmEntityType dogType = HardCodedTestModel.GetDogType();
            BatchReferenceSegment containedBatchReferenceSegment = new BatchReferenceSegment("$40", dogType, HardCodedTestModel.GetContainedDogEntitySet());
            Assert.Same(HardCodedTestModel.GetContainedDogEntitySet(), containedBatchReferenceSegment.EntitySet);
        }

        [Fact]
        public void EqualityIsCorrect() 
        {
            IEdmEntityType type = HardCodedTestModel.GetPersonType();
            BatchReferenceSegment batchReferenceSegment1 = new BatchReferenceSegment("$0", type, HardCodedTestModel.GetPeopleSet());
            BatchReferenceSegment batchReferenceSegment2 = new BatchReferenceSegment("$0", type, HardCodedTestModel.GetPeopleSet());
            Assert.True(batchReferenceSegment1.Equals(batchReferenceSegment2));
        }

        [Fact]
        public void InEqualityIsCorrect()
        {
            IEdmEntityType type1 = HardCodedTestModel.GetPersonType();
            IEdmEntityType type2 = HardCodedTestModel.GetDogType();
            BatchReferenceSegment batchReferenceSegment1 = new BatchReferenceSegment("$0", type1, HardCodedTestModel.GetPeopleSet());
            BatchReferenceSegment batchReferenceSegment2 = new BatchReferenceSegment("$0", type2, HardCodedTestModel.GetDogsSet());
            BatchReferenceSegment batchReferenceSegment3 = new BatchReferenceSegment("$10", type1, HardCodedTestModel.GetPeopleSet());
            BatchReferenceSegment batchReferenceSegment4 = new BatchReferenceSegment("$10", type2, HardCodedTestModel.GetContainedDogEntitySet());
            Assert.False(batchReferenceSegment1.Equals(batchReferenceSegment2));
            Assert.False(batchReferenceSegment1.Equals(batchReferenceSegment3));
            Assert.False(batchReferenceSegment2.Equals(batchReferenceSegment4));
        }
    }
}

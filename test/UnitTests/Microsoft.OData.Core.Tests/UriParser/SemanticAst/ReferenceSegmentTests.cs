//---------------------------------------------------------------------
// <copyright file="ReferenceSegmentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    public class ReferenceSegmentTests
    {
        #region Test Cases
        [Fact]
        public void ReferenceSegmentConstructWithNullNavigationSourceShouldThrowException()
        {
            Action create = () => new ReferenceSegment(null);
            Assert.Throws<ArgumentNullException>("navigationSource", create);
        }

        [Fact]
        public void ReferenceSegmentWithCollectionValueNavigationSourceConstructsSuccessfully()
        {
            ReferenceSegment referenceSegment = new ReferenceSegment(HardCodedTestModel.GetPet1Set());

            Assert.Equal(UriQueryConstants.RefSegment, referenceSegment.Identifier);
            Assert.False(referenceSegment.SingleResult);
            Assert.Same(HardCodedTestModel.GetPet1Set(), referenceSegment.TargetEdmNavigationSource);
            Assert.Equal(RequestTargetKind.Resource, referenceSegment.TargetKind);
            Assert.Null(referenceSegment.EdmType);
        }

        [Fact]
        public void ReferenceSegmentWithSingleValuedNavigationSourceConstructsSuccessfully()
        {
            ReferenceSegment referenceSegment = new ReferenceSegment(HardCodedTestModel.GetBossSingleton());

            Assert.Equal(UriQueryConstants.RefSegment, referenceSegment.Identifier);
            Assert.True(referenceSegment.SingleResult);
            Assert.Same(HardCodedTestModel.GetBossSingleton(), referenceSegment.TargetEdmNavigationSource);
            Assert.Equal(RequestTargetKind.Resource, referenceSegment.TargetKind);
            Assert.Null(referenceSegment.EdmType);
        }

        [Fact]
        public void ReferenceSegmentsWithSameNavigationSourcesShouldBeEqual()
        {
            ReferenceSegment referenceSegment1 = new ReferenceSegment(HardCodedTestModel.GetPet1Set());
            ReferenceSegment referenceSegment2 = new ReferenceSegment(HardCodedTestModel.GetPet1Set());

            Assert.True(referenceSegment1.Equals(referenceSegment2));
            Assert.True(referenceSegment2.Equals(referenceSegment1));
        }

        [Fact]
        public void ReferenceSegmentsWithDifferenceNavigationSourcesShouldNotBeEqual()
        {
            ReferenceSegment referenceSegment1 = new ReferenceSegment(HardCodedTestModel.GetPeopleSet());
            ReferenceSegment referenceSegment2 = new ReferenceSegment(HardCodedTestModel.GetPet1Set());

            Assert.False(referenceSegment1.Equals(referenceSegment2));
            Assert.False(referenceSegment2.Equals(referenceSegment1));
        }
        #endregion
    }
}
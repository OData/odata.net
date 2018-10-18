//---------------------------------------------------------------------
// <copyright file="ReferenceSegmentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
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
            create.ShouldThrow<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: navigationSource");
        }

        [Fact]
        public void ReferenceSegmentWithCollectionValueNavigationSourceConstructsSuccessfully()
        {
            ReferenceSegment referenceSegment = new ReferenceSegment(HardCodedTestModel.GetPet1Set());

            referenceSegment.Identifier.Should().Be(UriQueryConstants.RefSegment);
            referenceSegment.SingleResult.Should().BeFalse();
            referenceSegment.TargetEdmNavigationSource.Should().Be(HardCodedTestModel.GetPet1Set());
            referenceSegment.TargetKind.Should().Be(RequestTargetKind.Resource);
            referenceSegment.EdmType.Should().BeNull();
        }

        [Fact]
        public void ReferenceSegmentWithSingleValuedNavigationSourceConstructsSuccessfully()
        {
            ReferenceSegment referenceSegment = new ReferenceSegment(HardCodedTestModel.GetBossSingleton());

            referenceSegment.Identifier.Should().Be(UriQueryConstants.RefSegment);
            referenceSegment.SingleResult.Should().BeTrue();
            referenceSegment.TargetEdmNavigationSource.Should().Be(HardCodedTestModel.GetBossSingleton());
            referenceSegment.TargetKind.Should().Be(RequestTargetKind.Resource);
            referenceSegment.EdmType.Should().BeNull();
        }

        [Fact]
        public void ReferenceSegmentsWithSameNavigationSourcesShouldBeEqual()
        {
            ReferenceSegment referenceSegment1 = new ReferenceSegment(HardCodedTestModel.GetPet1Set());
            ReferenceSegment referenceSegment2 = new ReferenceSegment(HardCodedTestModel.GetPet1Set());

            referenceSegment1.Equals(referenceSegment2).Should().BeTrue();
            referenceSegment2.Equals(referenceSegment1).Should().BeTrue();
        }

        [Fact]
        public void ReferenceSegmentsWithDifferenceNavigationSourcesShouldNotBeEqual()
        {
            ReferenceSegment referenceSegment1 = new ReferenceSegment(HardCodedTestModel.GetPeopleSet());
            ReferenceSegment referenceSegment2 = new ReferenceSegment(HardCodedTestModel.GetPet1Set());

            referenceSegment1.Equals(referenceSegment2).Should().BeFalse();
            referenceSegment2.Equals(referenceSegment1).Should().BeFalse();
        }
        #endregion
    }
}
//---------------------------------------------------------------------
// <copyright file="ValueSegmentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.UriParser.SemanticAst
{
    public class ValueSegmentTests
    {
        [Fact]
        public void IdentifierIsValueSegment()
        {
            ValueSegment segment = new ValueSegment(HardCodedTestModel.GetPersonType());
            segment.Identifier.Should().Be(UriQueryConstants.ValueSegment);
        }

        [Fact]
        public void SingleResultIsTrue()
        {
            ValueSegment segment = new ValueSegment(HardCodedTestModel.GetPersonType());
            segment.SingleResult.Should().BeTrue();
        }

        [Fact]
        public void CollectionTypeThrows()
        {
            Action createWithCollectionType = () => new ValueSegment(ModelBuildingHelpers.BuildValidCollectionType());
            createWithCollectionType.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.PathParser_CannotUseValueOnCollection);
        }

        [Fact]
        public void EntityTypeIsConvertedToStream()
        {
            ValueSegment segment = new ValueSegment(ModelBuildingHelpers.BuildValidEntityType());
            segment.EdmType.Should().BeSameAs(EdmCoreModel.Instance.GetStream(false).Definition);
        }

        [Fact]
        public void NonEntityTypeIsPassedThrough()
        {
            IEdmType nonEntityType = ModelBuildingHelpers.BuildValidComplexType();
            ValueSegment segment = new ValueSegment(nonEntityType);
            segment.EdmType.Should().BeSameAs(nonEntityType);
        }

        [Fact]
        public void EqualityIsCorrect()
        {
            IEdmEntityType entityType = ModelBuildingHelpers.BuildValidEntityType();
            ValueSegment segment1 = new ValueSegment(entityType);
            ValueSegment segment2 = new ValueSegment(entityType);
            segment1.Equals(segment2).Should().BeTrue();
        }

        [Fact]
        public void InequalityIsCorrect()
        {
            ValueSegment segment1 = new ValueSegment(ModelBuildingHelpers.BuildValidEntityType());
            ValueSegment segment2 = new ValueSegment(ModelBuildingHelpers.BuildValidComplexType());
            BatchSegment segment3 = BatchSegment.Instance;
            segment1.Equals(segment2).Should().BeFalse();
            segment1.Equals(segment3).Should().BeFalse();
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="ValueSegmentTests.cs" company="Microsoft">
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
    public class ValueSegmentTests
    {
        [Fact]
        public void IdentifierIsValueSegment()
        {
            ValueSegment segment = new ValueSegment(HardCodedTestModel.GetPersonType());
            Assert.Equal(UriQueryConstants.ValueSegment, segment.Identifier);
        }

        [Fact]
        public void SingleResultIsTrue()
        {
            ValueSegment segment = new ValueSegment(HardCodedTestModel.GetPersonType());
            Assert.True(segment.SingleResult);
        }

        [Fact]
        public void CollectionTypeThrows()
        {
            Action createWithCollectionType = () => new ValueSegment(ModelBuildingHelpers.BuildValidCollectionType());
            createWithCollectionType.Throws<ODataException>(ODataErrorStrings.PathParser_CannotUseValueOnCollection);
        }

        [Fact]
        public void EntityTypeIsConvertedToStream()
        {
            ValueSegment segment = new ValueSegment(ModelBuildingHelpers.BuildValidEntityType());
            Assert.Same(EdmCoreModel.Instance.GetStream(false).Definition, segment.EdmType);
        }

        [Fact]
        public void NonEntityTypeIsPassedThrough()
        {
            IEdmType nonEntityType = ModelBuildingHelpers.BuildValidComplexType();
            ValueSegment segment = new ValueSegment(nonEntityType);
            Assert.Same(nonEntityType, segment.EdmType);
        }

        [Fact]
        public void EqualityIsCorrect()
        {
            IEdmEntityType entityType = ModelBuildingHelpers.BuildValidEntityType();
            ValueSegment segment1 = new ValueSegment(entityType);
            ValueSegment segment2 = new ValueSegment(entityType);
            Assert.True(segment1.Equals(segment2));
        }

        [Fact]
        public void InequalityIsCorrect()
        {
            ValueSegment segment1 = new ValueSegment(ModelBuildingHelpers.BuildValidEntityType());
            ValueSegment segment2 = new ValueSegment(ModelBuildingHelpers.BuildValidComplexType());
            BatchSegment segment3 = BatchSegment.Instance;
            Assert.False(segment1.Equals(segment2));
            Assert.False(segment1.Equals(segment3));
        }
    }
}

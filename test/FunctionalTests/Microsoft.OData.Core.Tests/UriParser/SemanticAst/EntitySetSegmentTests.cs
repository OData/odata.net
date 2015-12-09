//---------------------------------------------------------------------
// <copyright file="EntitySetSegmentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.SemanticAst
{
    public class EntitySetSegmentTests
    {
        [Fact]
        public void TargetEdmEntitySetIsEntitySet()
        {
            EntitySetSegment segment = new EntitySetSegment(HardCodedTestModel.GetPeopleSet());
            segment.TargetEdmNavigationSource.Should().BeSameAs(HardCodedTestModel.GetPeopleSet());
        }

        [Fact]
        public void TargetEdmTypeIsTypeOfEntitySet()
        {
            EntitySetSegment segment = new EntitySetSegment(HardCodedTestModel.GetPeopleSet());
            segment.TargetEdmType.Should().BeSameAs(HardCodedTestModel.GetPeopleSet().EntityType());
        }

        [Fact]
        public void TargetKindIsResource()
        {
            EntitySetSegment segment = new EntitySetSegment(HardCodedTestModel.GetPeopleSet());
            segment.TargetKind.Should().Be(RequestTargetKind.Resource);
        }

        [Fact]
        public void SingleResultIsFalse()
        {
            EntitySetSegment segment = new EntitySetSegment(HardCodedTestModel.GetPeopleSet());
            segment.SingleResult.Should().BeFalse();
        }

        [Fact]
        public void EntitySetCannotBeNull()
        {
            Action createWithNullEntitySet = () => new EntitySetSegment(null);
            createWithNullEntitySet.ShouldThrow<Exception>(Error.ArgumentNull("identifier").ToString());
        }

        [Fact]
        public void EntitySetSetCorrectly()
        {
            EntitySetSegment segment = new EntitySetSegment(HardCodedTestModel.GetPeopleSet());
            segment.EntitySet.Name.Should().Be(HardCodedTestModel.GetPeopleSet().Name);
        }

        [Fact]
        public void TypeComuptedCorrecretly()
        {
            EntitySetSegment segment = new EntitySetSegment(HardCodedTestModel.GetPeopleSet());
            segment.EdmType.Should().BeOfType<EdmCollectionType>();
            segment.EdmType.As<EdmCollectionType>().ShouldBeEquivalentTo<IEdmType>(new EdmCollectionType(new EdmEntityTypeReference(HardCodedTestModel.GetPersonType(), false)));
        }

        [Fact]
        public void EqualityIsCorrect()
        {
            EntitySetSegment segment1 = new EntitySetSegment(HardCodedTestModel.GetPeopleSet());
            EntitySetSegment segment2 = new EntitySetSegment(HardCodedTestModel.GetPeopleSet());
            segment1.Equals(segment2).Should().BeTrue();
        }

        [Fact]
        public void InequalityIsCorrect()
        {
            EntitySetSegment segment1 = new EntitySetSegment(HardCodedTestModel.GetPeopleSet());
            EntitySetSegment segment2 = new EntitySetSegment(HardCodedTestModel.GetDogsSet());
            BatchSegment segment3 = BatchSegment.Instance;
            segment1.Equals(segment2).Should().BeFalse();
            segment1.Equals(segment3).Should().BeFalse();
        }
    }
}

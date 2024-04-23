//---------------------------------------------------------------------
// <copyright file="EntitySetSegmentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    public class EntitySetSegmentTests
    {
        [Fact]
        public void IdentifierByDefaultIsEntitySetName()
        {
            EntitySetSegment segment = new EntitySetSegment(HardCodedTestModel.GetPeopleSet());
            Assert.Equal("People", segment.Identifier);
        }

        [Fact]
        public void TargetEdmEntitySetIsEntitySet()
        {
            EntitySetSegment segment = new EntitySetSegment(HardCodedTestModel.GetPeopleSet());
            Assert.Same(HardCodedTestModel.GetPeopleSet(), segment.TargetEdmNavigationSource);
        }

        [Fact]
        public void TargetEdmTypeIsTypeOfEntitySet()
        {
            EntitySetSegment segment = new EntitySetSegment(HardCodedTestModel.GetPeopleSet());
            Assert.Same(HardCodedTestModel.GetPeopleSet().EntityType, segment.TargetEdmType);
        }

        [Fact]
        public void TargetKindIsResource()
        {
            EntitySetSegment segment = new EntitySetSegment(HardCodedTestModel.GetPeopleSet());
            Assert.Equal(RequestTargetKind.Resource, segment.TargetKind);
        }

        [Fact]
        public void SingleResultIsFalse()
        {
            EntitySetSegment segment = new EntitySetSegment(HardCodedTestModel.GetPeopleSet());
            Assert.False(segment.SingleResult);
        }

        [Fact]
        public void EntitySetCannotBeNull()
        {
            Action createWithNullEntitySet = () => new EntitySetSegment(null);
            Assert.Throws<ArgumentNullException>("entitySet", createWithNullEntitySet);
        }

        [Fact]
        public void EntitySetSetCorrectly()
        {
            EntitySetSegment segment = new EntitySetSegment(HardCodedTestModel.GetPeopleSet());
            Assert.Equal(HardCodedTestModel.GetPeopleSet().Name, segment.EntitySet.Name);
        }

        [Fact]
        public void TypeComuptedCorrecretly()
        {
            EntitySetSegment segment = new EntitySetSegment(HardCodedTestModel.GetPeopleSet());
            EdmCollectionType collectionType = Assert.IsType<EdmCollectionType>(segment.EdmType);
            Assert.True(collectionType.IsEquivalentTo(new EdmCollectionType(new EdmEntityTypeReference(HardCodedTestModel.GetPersonType(), false))));
        }

        [Fact]
        public void EqualityIsCorrect()
        {
            EntitySetSegment segment1 = new EntitySetSegment(HardCodedTestModel.GetPeopleSet());
            EntitySetSegment segment2 = new EntitySetSegment(HardCodedTestModel.GetPeopleSet());
            Assert.True(segment1.Equals(segment2));
        }

        [Fact]
        public void InequalityIsCorrect()
        {
            EntitySetSegment segment1 = new EntitySetSegment(HardCodedTestModel.GetPeopleSet());
            EntitySetSegment segment2 = new EntitySetSegment(HardCodedTestModel.GetDogsSet());
            BatchSegment segment3 = BatchSegment.Instance;
            Assert.False(segment1.Equals(segment2));
            Assert.False(segment1.Equals(segment3));
        }

        [Fact]
        public void NavigationSourceIsCorrect()
        {
            IEdmEntitySet peopleEntitySet = HardCodedTestModel.GetPeopleSet();
            EntitySetSegment segment = new EntitySetSegment(peopleEntitySet);
            Assert.Same(peopleEntitySet, segment.NavigationSource);
        }
    }
}

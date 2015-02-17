//---------------------------------------------------------------------
// <copyright file="EntitySetSegmentUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    using System;
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EntitySetSegmentUnitTests
    {
        [TestMethod]
        public void TargetEdmEntitySetIsEntitySet()
        {
            EntitySetSegment segment = new EntitySetSegment(HardCodedTestModel.GetPeopleSet());
            segment.TargetEdmNavigationSource.Should().BeSameAs(HardCodedTestModel.GetPeopleSet());
        }

        [TestMethod]
        public void TargetEdmTypeIsTypeOfEntitySet()
        {
            EntitySetSegment segment = new EntitySetSegment(HardCodedTestModel.GetPeopleSet());
            segment.TargetEdmType.Should().BeSameAs(HardCodedTestModel.GetPeopleSet().EntityType());
        }

        [TestMethod]
        public void TargetKindIsResource()
        {
            EntitySetSegment segment = new EntitySetSegment(HardCodedTestModel.GetPeopleSet());
            segment.TargetKind.Should().Be(RequestTargetKind.Resource);
        }

        [TestMethod]
        public void SingleResultIsFalse()
        {
            EntitySetSegment segment = new EntitySetSegment(HardCodedTestModel.GetPeopleSet());
            segment.SingleResult.Should().BeFalse();
        }

        [TestMethod]
        public void EntitySetCannotBeNull()
        {
            Action createWithNullEntitySet = () => new EntitySetSegment(null);
            createWithNullEntitySet.ShouldThrow<Exception>(Error.ArgumentNull("identifier").ToString());
        }

        [TestMethod]
        public void EntitySetSetCorrectly()
        {
            EntitySetSegment segment = new EntitySetSegment(HardCodedTestModel.GetPeopleSet());
            segment.EntitySet.Name.Should().Be(HardCodedTestModel.GetPeopleSet().Name);
        }

        [TestMethod]
        public void TypeComuptedCorrecretly()
        {
            EntitySetSegment segment = new EntitySetSegment(HardCodedTestModel.GetPeopleSet());
            segment.EdmType.Should().BeOfType<EdmCollectionType>();
            segment.EdmType.As<EdmCollectionType>().ShouldBeEquivalentTo<IEdmType>(new EdmCollectionType(new EdmEntityTypeReference(HardCodedTestModel.GetPersonType(), false)));
        }

        [TestMethod]
        public void EqualityIsCorrect()
        {
            EntitySetSegment segment1 = new EntitySetSegment(HardCodedTestModel.GetPeopleSet());
            EntitySetSegment segment2 = new EntitySetSegment(HardCodedTestModel.GetPeopleSet());
            segment1.Equals(segment2).Should().BeTrue();
        }

        [TestMethod]
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

//---------------------------------------------------------------------
// <copyright file="TypeSegmentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.SemanticAst
{
    public class TypeSegmentTests
    {
        [Fact]
        public void TargetTypeCannotBeNull()
        {
            Action createWithNullTargetType = () => new TypeSegment(null, null);
            createWithNullTargetType.ShouldThrow<Exception>(Error.ArgumentNull("identifier").ToString());
        }

        [Fact]
        public void TargetEdmTypeIsTargetType()
        {
            TypeSegment typeSegment = new TypeSegment(HardCodedTestModel.GetPersonType(), null);
            typeSegment.TargetEdmType.Should().BeSameAs(HardCodedTestModel.GetPersonType());
        }

        [Fact]
        public void TypeSetCorrectly()
        {
            IEdmType type = ModelBuildingHelpers.BuildValidEntityType();
            TypeSegment segment = new TypeSegment(type, null);
            segment.EdmType.Should().BeSameAs(type);
        }

        [Fact]
        public void SetIsSetCorrectly()
        {
            var set = ModelBuildingHelpers.BuildValidEntitySet();
            IEdmType type = set.EntityType();
            TypeSegment segment = new TypeSegment(type, set);
            segment.NavigationSource.Should().BeSameAs(set);
        }

        [Fact]
        public void TypeMustBeRelatedToSet()
        {
            Action create = () => new TypeSegment(HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetDogsSet());
            create.ShouldThrow<ODataException>().WithMessage(Strings.PathParser_TypeMustBeRelatedToSet(HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetDogType(), "TypeSegments"));
        }

        [Fact]
        public void EqualityIsCorrect()
        {
            TypeSegment typeSegment1 = new TypeSegment(HardCodedTestModel.GetPersonType(), null);
            TypeSegment typeSegment2 = new TypeSegment(HardCodedTestModel.GetPersonType(), null);
            typeSegment1.Equals(typeSegment2).Should().BeTrue();
        }

        [Fact]
        public void InequalityIsCorrect()
        {
            TypeSegment typeSegment1 = new TypeSegment(HardCodedTestModel.GetPersonType(), null);
            TypeSegment typeSegment2 = new TypeSegment(HardCodedTestModel.GetDogType(), null);
            BatchSegment batchSegment = BatchSegment.Instance;
            typeSegment1.Equals(typeSegment2).Should().BeFalse();
            typeSegment1.Equals(batchSegment).Should().BeFalse();
        }
    }
}

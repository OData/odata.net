//---------------------------------------------------------------------
// <copyright file="DynamicPathSegmentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    public class DynamicPathSegmentTests
    {
        [Fact]
        public void IdentifierIsPropertyName()
        {
            DynamicPathSegment openPropertySegment = new DynamicPathSegment("stuff");
            openPropertySegment.Identifier.Should().Be("stuff");
        }
        
        [Fact]
        public void TargetEdmTypeIsNull()
        {
            DynamicPathSegment openPropertySegment = new DynamicPathSegment("evenmoreawesomestuff");
            openPropertySegment.TargetEdmType.Should().BeNull();
        }

        [Fact]
        public void TargetKindIsOpenProperty()
        {
            DynamicPathSegment openPropertySegment = new DynamicPathSegment("anincredibleamountofstuff");
            openPropertySegment.TargetKind.Should().Be(RequestTargetKind.Dynamic);
        }

        [Fact]
        public void SingleResultIsTrue()
        {
            DynamicPathSegment openPropertySegment = new DynamicPathSegment("itblowsmymindhowmuchstuffthereisinhere");
            openPropertySegment.SingleResult.Should().BeTrue();
        }

        [Fact]
        public void PropertyNameSetCorrectly()
        {
            DynamicPathSegment openPropertySegment = new DynamicPathSegment("beans");
            openPropertySegment.Identifier.Should().Be("beans");
        }

        [Fact]
        public void EqualityIsCorrect()
        {
            DynamicPathSegment openPropertySegment1 = new DynamicPathSegment("superbeans");
            DynamicPathSegment openPropertySegment2 = new DynamicPathSegment("superbeans");
            openPropertySegment1.Equals(openPropertySegment2).Should().BeTrue();
        }

        [Fact]
        public void EqualityIsCorrect_withTypeInfo()
        {
            DynamicPathSegment segment1 = new DynamicPathSegment("superbeans", HardCodedTestModel.GetPersonType(), null, true);
            DynamicPathSegment segment2 = new DynamicPathSegment("superbeans", HardCodedTestModel.GetPersonType(), null, true);
            segment1.Equals(segment2).Should().BeTrue();
        }

        [Fact]
        public void EqualityIsCorrect_withNavigationSourceInfo()
        {
            DynamicPathSegment segment1 = new DynamicPathSegment("superbeans", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(), true);
            DynamicPathSegment segment2 = new DynamicPathSegment("superbeans", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(), true);
            segment1.Equals(segment2).Should().BeTrue();
        }

        [Fact]
        public void InequalityIsCorrect()
        {
            DynamicPathSegment segment1 = new DynamicPathSegment("superbeans");
            DynamicPathSegment segment2 = new DynamicPathSegment("incredibeans");
            DynamicPathSegment segment3 = new DynamicPathSegment("superbeans", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(), true);
            DynamicPathSegment segment4 = new DynamicPathSegment("incredibeans", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(), true);
            DynamicPathSegment segment5 = new DynamicPathSegment("superbeans", HardCodedTestModel.GetDogType(), HardCodedTestModel.GetPeopleSet(), true);
            DynamicPathSegment segment6 = new DynamicPathSegment("superbeans", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPaintingsSet(), true);
            DynamicPathSegment segment7 = new DynamicPathSegment("superbeans", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(), false);
            BatchSegment segment = BatchSegment.Instance;
            segment1.Equals(segment2).Should().BeFalse();
            segment2.Equals(segment).Should().BeFalse();
            segment1.Equals(segment3).Should().BeFalse();
            segment3.Equals(segment4).Should().BeFalse();
            segment3.Equals(segment5).Should().BeFalse();
            segment3.Equals(segment6).Should().BeFalse();
            segment3.Equals(segment7).Should().BeFalse();
        }
    }
}

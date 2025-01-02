//---------------------------------------------------------------------
// <copyright file="DynamicPathSegmentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

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
            Assert.Equal("stuff", openPropertySegment.Identifier);
        }

        [Fact]
        public void TargetEdmTypeIsNull()
        {
            DynamicPathSegment openPropertySegment = new DynamicPathSegment("evenmoreawesomestuff");
            Assert.Null(openPropertySegment.TargetEdmType);
        }

        [Fact]
        public void TargetKindIsOpenProperty()
        {
            DynamicPathSegment openPropertySegment = new DynamicPathSegment("anincredibleamountofstuff");
            Assert.Equal(RequestTargetKind.Dynamic, openPropertySegment.TargetKind);
        }

        [Fact]
        public void SingleResultIsTrue()
        {
            DynamicPathSegment openPropertySegment = new DynamicPathSegment("itblowsmymindhowmuchstuffthereisinhere");
            Assert.True(openPropertySegment.SingleResult);
        }

        [Fact]
        public void PropertyNameSetCorrectly()
        {
            DynamicPathSegment openPropertySegment = new DynamicPathSegment("beans");
            Assert.Equal("beans", openPropertySegment.Identifier);
        }

        [Fact]
        public void EqualityIsCorrect()
        {
            DynamicPathSegment openPropertySegment1 = new DynamicPathSegment("superbeans");
            DynamicPathSegment openPropertySegment2 = new DynamicPathSegment("superbeans");
            Assert.True(openPropertySegment1.Equals(openPropertySegment2));
        }

        [Fact]
        public void EqualityIsCorrect_withTypeInfo()
        {
            DynamicPathSegment segment1 = new DynamicPathSegment("superbeans", HardCodedTestModel.GetPersonType(), null, true);
            DynamicPathSegment segment2 = new DynamicPathSegment("superbeans", HardCodedTestModel.GetPersonType(), null, true);
            Assert.True(segment1.Equals(segment2));
        }

        [Fact]
        public void EqualityIsCorrect_withNavigationSourceInfo()
        {
            DynamicPathSegment segment1 = new DynamicPathSegment("superbeans", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(), true);
            DynamicPathSegment segment2 = new DynamicPathSegment("superbeans", HardCodedTestModel.GetPersonType(), HardCodedTestModel.GetPeopleSet(), true);
            Assert.True(segment1.Equals(segment2));
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
            Assert.False(segment1.Equals(segment2));
            Assert.False(segment2.Equals(segment));
            Assert.False(segment1.Equals(segment3));
            Assert.False(segment3.Equals(segment4));
            Assert.False(segment3.Equals(segment5));
            Assert.False(segment3.Equals(segment6));
            Assert.False(segment3.Equals(segment7));
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="IndexSegmentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    public class IndexSegmentTests
    {
        [Fact]
        public void IndexSegment_IdentifierIsIndexValue()
        {
            IndexSegment segment = new IndexSegment(1, EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32));
            Assert.Equal("1", segment.Identifier);
        }

        [Fact]
        public void IndexSegment_TargetEdmTypeIsTypeOfInput()
        {
            IEdmType edmType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32);
            IndexSegment segment = new IndexSegment(1, edmType);
            Assert.Same(edmType, segment.TargetEdmType);
            Assert.Same(edmType, segment.EdmType);
        }

        [Fact]
        public void IndexSegment_TargetKindIsResource()
        {
            IndexSegment segment = new IndexSegment(42, HardCodedTestModel.GetPersonType());
            Assert.Equal(RequestTargetKind.Resource, segment.TargetKind);
        }

        [Fact]
        public void IndexSegment_SingleResultIsTrue()
        {
            IndexSegment segment = new IndexSegment(1, HardCodedTestModel.GetPersonType());
            Assert.True(segment.SingleResult);
        }

        [Fact]
        public void IndexSegment_IndexProperytIsInputValue()
        {
            IndexSegment segment = new IndexSegment(42, HardCodedTestModel.GetPersonType());
            Assert.Equal(42, segment.Index);
        }

        [Fact]
        public void IndexSegment_EqualityIsCorrect()
        {
            IndexSegment segment1 = new IndexSegment(42, HardCodedTestModel.GetPersonType());
            IndexSegment segment2 = new IndexSegment(42, HardCodedTestModel.GetPersonType());
            Assert.True(segment1.Equals(segment2));
        }

        [Fact]
        public void IndexSegment_InequalityIsCorrect()
        {
            IndexSegment segment1 = new IndexSegment(42, HardCodedTestModel.GetPersonType());
            IndexSegment segment2 = new IndexSegment(42, HardCodedTestModel.GetDogType());
            BatchSegment segment3 = BatchSegment.Instance;
            Assert.False(segment1.Equals(segment2));
            Assert.False(segment1.Equals(segment3));
        }
    }
}

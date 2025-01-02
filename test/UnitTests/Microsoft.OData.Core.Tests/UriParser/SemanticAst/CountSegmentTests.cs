//---------------------------------------------------------------------
// <copyright file="CountSegmentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    public class CountSegmentTests
    {
        [Fact]
        public void IdentifierIsCount()
        {
            Assert.Equal(UriQueryConstants.CountSegment, CountSegment.Instance.Identifier);
        }

        [Fact]
        public void SingleResultIsTrue()
        {
            Assert.True(CountSegment.Instance.SingleResult);
        }

        [Fact]
        public void TargetKindIsPrimitive()
        {
            Assert.Equal(RequestTargetKind.PrimitiveValue, CountSegment.Instance.TargetKind);
        }

        [Fact]
        public void EqualityIsCorrect()
        {
            CountSegment segment1 = CountSegment.Instance;
            CountSegment segment2 = CountSegment.Instance;
            Assert.Same(segment1, segment2);
        }

        [Fact]
        public void InequalityIsCorrect()
        {
            CountSegment segment1 = CountSegment.Instance;
            BatchSegment segment2 = BatchSegment.Instance;
            Assert.NotSame(segment1, segment2);
        }
    }
}

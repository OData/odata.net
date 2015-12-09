//---------------------------------------------------------------------
// <copyright file="CountSegmentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.SemanticAst
{
    public class CountSegmentTests
    {
        [Fact]
        public void IdentifierIsCount()
        {
            CountSegment.Instance.Identifier.Should().Be(UriQueryConstants.CountSegment);
        }

        [Fact]
        public void SingleResultIsTrue()
        {
            CountSegment.Instance.SingleResult.Should().BeTrue();
        }

        [Fact]
        public void TargetKindIsPrimitive()
        {
            CountSegment.Instance.TargetKind.Should().Be(RequestTargetKind.PrimitiveValue);
        }

        [Fact]
        public void EqualityIsCorrect()
        {
            CountSegment segment1 = CountSegment.Instance;
            CountSegment segment2 = CountSegment.Instance;
            segment1.Equals(segment2).Should().BeTrue();
        }

        [Fact]
        public void InequalityIsCorrect()
        {
            CountSegment segment1 = CountSegment.Instance;
            BatchSegment segment2 = BatchSegment.Instance;
            segment1.Equals(segment2).Should().BeFalse();
        }
    }
}

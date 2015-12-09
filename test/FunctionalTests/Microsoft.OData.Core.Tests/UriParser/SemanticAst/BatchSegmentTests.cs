//---------------------------------------------------------------------
// <copyright file="BatchSegmentTests.cs" company="Microsoft">
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
    public class BatchSegmentTests
    {
        [Fact]
        public void IdentifierIsBatchSegment()
        {
            BatchSegment.Instance.Identifier.Should().Be(UriQueryConstants.BatchSegment);
        }

        [Fact]
        public void TargetKindIsBatch()
        {
            BatchSegment.Instance.TargetKind.Should().Be(RequestTargetKind.Batch);
        }

        [Fact]
        public void EqualityIsCorrect()
        {
            BatchSegment segment1 = BatchSegment.Instance;
            BatchSegment segment2 = BatchSegment.Instance;
            segment1.Equals(segment2).Should().BeTrue();
        }

        [Fact]
        public void InequalityIsCorrect()
        {
            BatchSegment segment1 = BatchSegment.Instance;
            CountSegment segment2 = CountSegment.Instance;
            segment1.Equals(segment2).Should().BeFalse();
        }
    }
}

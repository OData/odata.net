//---------------------------------------------------------------------
// <copyright file="BatchSegmentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    public class BatchSegmentTests
    {
        [Fact]
        public void IdentifierIsBatchSegment()
        {
            Assert.Equal(UriQueryConstants.BatchSegment, BatchSegment.Instance.Identifier);
        }

        [Fact]
        public void TargetKindIsBatch()
        {
            Assert.Equal(RequestTargetKind.Batch, BatchSegment.Instance.TargetKind);
        }

        [Fact]
        public void EqualityIsCorrect()
        {
            BatchSegment segment1 = BatchSegment.Instance;
            BatchSegment segment2 = BatchSegment.Instance;
            Assert.True(segment1.Equals(segment2));
        }

        [Fact]
        public void InequalityIsCorrect()
        {
            BatchSegment segment1 = BatchSegment.Instance;
            CountSegment segment2 = CountSegment.Instance;
            Assert.False(segment1.Equals(segment2));
        }
    }
}

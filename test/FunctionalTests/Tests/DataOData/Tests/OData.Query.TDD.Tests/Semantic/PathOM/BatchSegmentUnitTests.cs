//---------------------------------------------------------------------
// <copyright file="BatchSegmentUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BatchSegmentUnitTests
    {
        [TestMethod]
        public void IdentifierIsBatchSegment()
        {
            BatchSegment.Instance.Identifier.Should().Be(UriQueryConstants.BatchSegment);
        }

        [TestMethod]
        public void TargetKindIsBatch()
        {
            BatchSegment.Instance.TargetKind.Should().Be(RequestTargetKind.Batch);
        }

        [TestMethod]
        public void EqualityIsCorrect()
        {
            BatchSegment segment1 = BatchSegment.Instance;
            BatchSegment segment2 = BatchSegment.Instance;
            segment1.Equals(segment2).Should().BeTrue();
        }

        [TestMethod]
        public void InequalityIsCorrect()
        {
            BatchSegment segment1 = BatchSegment.Instance;
            CountSegment segment2 = CountSegment.Instance;
            segment1.Equals(segment2).Should().BeFalse();
        }
    }
}

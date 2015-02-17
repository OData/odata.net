//---------------------------------------------------------------------
// <copyright file="CountSegmentUnitTests.cs" company="Microsoft">
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
    public class CountSegmentUnitTests
    {
        [TestMethod]
        public void IdentifierIsCount()
        {
            CountSegment.Instance.Identifier.Should().Be(UriQueryConstants.CountSegment);
        }

        [TestMethod]
        public void SingleResultIsTrue()
        {
            CountSegment.Instance.SingleResult.Should().BeTrue();
        }

        [TestMethod]
        public void TargetKindIsPrimitive()
        {
            CountSegment.Instance.TargetKind.Should().Be(RequestTargetKind.PrimitiveValue);
        }

        [TestMethod]
        public void EqualityIsCorrect()
        {
            CountSegment segment1 = CountSegment.Instance;
            CountSegment segment2 = CountSegment.Instance;
            segment1.Equals(segment2).Should().BeTrue();
        }

        [TestMethod]
        public void InequalityIsCorrect()
        {
            CountSegment segment1 = CountSegment.Instance;
            BatchSegment segment2 = BatchSegment.Instance;
            segment1.Equals(segment2).Should().BeFalse();
        }
    }
}

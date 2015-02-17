//---------------------------------------------------------------------
// <copyright file="MetadataSegmentUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    using System.Collections.Generic;
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MetadataSegmentUnitTests
    {
        [TestMethod]
        public void IdentifierShouldBeMetadataSegment()
        {
            MetadataSegment.Instance.Identifier.Should().Be(UriQueryConstants.MetadataSegment);
        }

        [TestMethod]
        public void TargetKindShouldBeMetadata()
        {
            MetadataSegment.Instance.TargetKind.Should().Be(RequestTargetKind.Metadata);
        }

        [TestMethod]
        public void EqualityIsCorrect()
        {
            MetadataSegment segment1 = MetadataSegment.Instance;
            MetadataSegment segment2 = MetadataSegment.Instance;
            segment1.Equals(segment2).Should().BeTrue();
        }

        [TestMethod]
        public void InequalityIsCorrect()
        {
            MetadataSegment segment1 = MetadataSegment.Instance;
            CountSegment segment2 = CountSegment.Instance;
            segment1.Equals(segment2).Should().BeFalse();
        }
    }
}

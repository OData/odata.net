//---------------------------------------------------------------------
// <copyright file="MetadataSegmentTests.cs" company="Microsoft">
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
    public class MetadataSegmentTests
    {
        [Fact]
        public void IdentifierShouldBeMetadataSegment()
        {
            MetadataSegment.Instance.Identifier.Should().Be(UriQueryConstants.MetadataSegment);
        }

        [Fact]
        public void TargetKindShouldBeMetadata()
        {
            MetadataSegment.Instance.TargetKind.Should().Be(RequestTargetKind.Metadata);
        }

        [Fact]
        public void EqualityIsCorrect()
        {
            MetadataSegment segment1 = MetadataSegment.Instance;
            MetadataSegment segment2 = MetadataSegment.Instance;
            segment1.Equals(segment2).Should().BeTrue();
        }

        [Fact]
        public void InequalityIsCorrect()
        {
            MetadataSegment segment1 = MetadataSegment.Instance;
            CountSegment segment2 = CountSegment.Instance;
            segment1.Equals(segment2).Should().BeFalse();
        }
    }
}

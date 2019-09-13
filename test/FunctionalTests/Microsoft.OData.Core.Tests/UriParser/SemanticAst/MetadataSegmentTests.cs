//---------------------------------------------------------------------
// <copyright file="MetadataSegmentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SemanticAst
{
    public class MetadataSegmentTests
    {
        [Fact]
        public void IdentifierShouldBeMetadataSegment()
        {
            Assert.Equal(UriQueryConstants.MetadataSegment, MetadataSegment.Instance.Identifier);
        }

        [Fact]
        public void TargetKindShouldBeMetadata()
        {
            Assert.Equal(RequestTargetKind.Metadata, MetadataSegment.Instance.TargetKind);
        }

        [Fact]
        public void EqualityIsCorrect()
        {
            MetadataSegment segment1 = MetadataSegment.Instance;
            MetadataSegment segment2 = MetadataSegment.Instance;
            Assert.True(segment1.Equals(segment2));
        }

        [Fact]
        public void InequalityIsCorrect()
        {
            MetadataSegment segment1 = MetadataSegment.Instance;
            CountSegment segment2 = CountSegment.Instance;
            Assert.False(segment1.Equals(segment2));
        }
    }
}

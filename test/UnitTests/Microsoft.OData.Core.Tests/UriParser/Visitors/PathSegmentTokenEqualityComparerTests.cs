//---------------------------------------------------------------------
// <copyright file="PathSegmentTokenEqualityComparerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Visitors
{
    public class PathSegmentTokenEqualityComparerTests
    {
        private readonly PathSegmentTokenEqualityComparer testSubject = new PathSegmentTokenEqualityComparer();

        [Fact]
        public void PathSegmentTokensAreEquivalent()
        {
            var token1 = new NonSystemToken("foo", null, null);
            var token2 = new NonSystemToken("foo", null, null);
            Assert.True(this.testSubject.Equals(token1, token2));
        }

        [Fact]
        public void PathSegmentTokensAreNotEquivalent()
        {
            var token1 = new NonSystemToken("foo", null, null);
            var token2 = new NonSystemToken("bar", null, null);
            Assert.False(this.testSubject.Equals(token1, token2));
        }

        [Fact]
        public void PathSegmentTokensAreNotEquivalentDueToLength()
        {
            var token1 = new NonSystemToken("foo", null, null);
            var token2 = new NonSystemToken("foo", null, new SystemToken("$batch", null));
            Assert.False(this.testSubject.Equals(token1, token2));
        }

        [Fact]
        public void PathSegmentTokensAreNotEquivalentAtSecondPosition()
        {
            var token1 = new NonSystemToken("foo", null, new SystemToken("$metadata", null));
            var token2 = new NonSystemToken("foo", null, new SystemToken("$batch", null));
            Assert.False(this.testSubject.Equals(token1, token2));
        }

        [Fact]
        public void NullPathSegmentTokensAreEquivalent()
        {
            Assert.True(this.testSubject.Equals(null, null));
        }

        [Fact]
        public void NullPathSegmentTokensAreNotEquivalentToAnyNonNullToken()
        {
            var token = new NonSystemToken("foo", null, null);
            Assert.False(this.testSubject.Equals(null, token));
            Assert.False(this.testSubject.Equals(token, null));
        }

        [Fact]
        public void PathSegmentTokenHashCodeForNullShouldBeZero()
        {
            Assert.Equal(0, this.testSubject.GetHashCode(null));
        }

        [Fact]
        public void PathSegmentTokenHashCodesShouldBeDifferent()
        {
            var token1 = new NonSystemToken("foo", null, null);
            var token2 = new NonSystemToken("bar", null, null);
            Assert.NotEqual(this.testSubject.GetHashCode(token1), this.testSubject.GetHashCode(token2));
        }
        [Fact]
        public void PathSegmentTokenHashCodesShouldBeTheSame()
        {
            var token1 = new NonSystemToken("foo", null, new SystemToken("$metadata", null));
            var token2 = new NonSystemToken("foo", null, new SystemToken("$metadata", null));
            Assert.Equal(this.testSubject.GetHashCode(token1), this.testSubject.GetHashCode(token2));
        }
    }
}

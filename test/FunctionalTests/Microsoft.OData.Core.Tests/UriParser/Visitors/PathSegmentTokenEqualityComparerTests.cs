//---------------------------------------------------------------------
// <copyright file="PathSegmentTokenEqualityComparerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Microsoft.OData.Core.UriParser.Syntactic;
using Microsoft.OData.Core.UriParser.Visitors;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.UriParser.Visitors
{
    public class PathSegmentTokenEqualityComparerTests
    {
        private readonly PathSegmentTokenEqualityComparer testSubject = new PathSegmentTokenEqualityComparer();

        [Fact]
        public void PathSegmentTokensAreEquivalent()
        {
            var token1 = new NonSystemToken("foo", null, null);
            var token2 = new NonSystemToken("foo", null, null);
            this.testSubject.Equals(token1, token2).Should().BeTrue();
        }

        [Fact]
        public void PathSegmentTokensAreNotEquivalent()
        {
            var token1 = new NonSystemToken("foo", null, null);
            var token2 = new NonSystemToken("bar", null, null);
            this.testSubject.Equals(token1, token2).Should().BeFalse();
        }

        [Fact]
        public void PathSegmentTokensAreNotEquivalentDueToLength()
        {
            var token1 = new NonSystemToken("foo", null, null);
            var token2 = new NonSystemToken("foo", null, new SystemToken("$batch", null));
            this.testSubject.Equals(token1, token2).Should().BeFalse();
        }

        [Fact]
        public void PathSegmentTokensAreNotEquivalentAtSecondPosition()
        {
            var token1 = new NonSystemToken("foo", null, new SystemToken("$metadata", null));
            var token2 = new NonSystemToken("foo", null, new SystemToken("$batch", null));
            this.testSubject.Equals(token1, token2).Should().BeFalse();
        }

        [Fact]
        public void NullPathSegmentTokensAreEquivalent()
        {
            this.testSubject.Equals(null, null).Should().BeTrue();
        }

        [Fact]
        public void NullPathSegmentTokensAreNotEquivalentToAnyNonNullToken()
        {
            var token = new NonSystemToken("foo", null, null);
            this.testSubject.Equals(null, token).Should().BeFalse();
            this.testSubject.Equals(token, null).Should().BeFalse();
        }

        [Fact]
        public void PathSegmentTokenHashCodeForNullShouldBeZero()
        {
            this.testSubject.GetHashCode(null).Should().Be(0);
        }

        [Fact]
        public void PathSegmentTokenHashCodesShouldBeDifferent()
        {
            var token1 = new NonSystemToken("foo", null, null);
            var token2 = new NonSystemToken("bar", null, null);
            this.testSubject.GetHashCode(token1).Should().NotBe(this.testSubject.GetHashCode(token2));
        }
        [Fact]
        public void PathSegmentTokenHashCodesShouldBeTheSame()
        {
            var token1 = new NonSystemToken("foo", null, new SystemToken("$metadata", null));
            var token2 = new NonSystemToken("foo", null, new SystemToken("$metadata", null));
            this.testSubject.GetHashCode(token1).Should().Be(this.testSubject.GetHashCode(token2));
        }
    }
}

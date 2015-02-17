//---------------------------------------------------------------------
// <copyright file="PathSegmentTokenEqualityComparerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.OData.Core.UriParser.Visitors;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    [TestClass]
    public class PathSegmentTokenEqualityComparerTests
    {
        private readonly PathSegmentTokenEqualityComparer testSubject = new PathSegmentTokenEqualityComparer();

        [TestMethod]
        public void PathSegmentTokensAreEquivalent()
        {
            var token1 = new NonSystemToken("foo", null, null);
            var token2 = new NonSystemToken("foo", null, null);
            this.testSubject.Equals(token1, token2).Should().BeTrue();
        }

        [TestMethod]
        public void PathSegmentTokensAreNotEquivalent()
        {
            var token1 = new NonSystemToken("foo", null, null);
            var token2 = new NonSystemToken("bar", null, null);
            this.testSubject.Equals(token1, token2).Should().BeFalse();
        }

        [TestMethod]
        public void PathSegmentTokensAreNotEquivalentDueToLength()
        {
            var token1 = new NonSystemToken("foo", null, null);
            var token2 = new NonSystemToken("foo", null, new SystemToken("$batch", null));
            this.testSubject.Equals(token1, token2).Should().BeFalse();
        }

        [TestMethod]
        public void PathSegmentTokensAreNotEquivalentAtSecondPosition()
        {
            var token1 = new NonSystemToken("foo", null, new SystemToken("$metadata", null));
            var token2 = new NonSystemToken("foo", null, new SystemToken("$batch", null));
            this.testSubject.Equals(token1, token2).Should().BeFalse();
        }

        [TestMethod]
        public void NullPathSegmentTokensAreEquivalent()
        {
            this.testSubject.Equals(null, null).Should().BeTrue();
        }

        [TestMethod]
        public void NullPathSegmentTokensAreNotEquivalentToAnyNonNullToken()
        {
            var token = new NonSystemToken("foo", null, null);
            this.testSubject.Equals(null, token).Should().BeFalse();
            this.testSubject.Equals(token, null).Should().BeFalse();
        }

        [TestMethod]
        public void PathSegmentTokenHashCodeForNullShouldBeZero()
        {
            this.testSubject.GetHashCode(null).Should().Be(0);
        }

        [TestMethod]
        public void PathSegmentTokenHashCodesShouldBeDifferent()
        {
            var token1 = new NonSystemToken("foo", null, null);
            var token2 = new NonSystemToken("bar", null, null);
            this.testSubject.GetHashCode(token1).Should().NotBe(this.testSubject.GetHashCode(token2));
        }
        [TestMethod]
        public void PathSegmentTokenHashCodesShouldBeTheSame()
        {
            var token1 = new NonSystemToken("foo", null, new SystemToken("$metadata", null));
            var token2 = new NonSystemToken("foo", null, new SystemToken("$metadata", null));
            this.testSubject.GetHashCode(token1).Should().Be(this.testSubject.GetHashCode(token2));
        }
    }
}

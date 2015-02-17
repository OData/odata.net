//---------------------------------------------------------------------
// <copyright file="PathSegmentTokenUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Syntactic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.OData.Core.UriParser.Visitors;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PathSegmentTokenUnitTests
    {
        private class DummyPathSegmentToken : PathSegmentToken
        {
            public DummyPathSegmentToken(PathSegmentToken nextToken) : base(nextToken)
            {
            }

            public override string Identifier
            {
                get { throw new NotImplementedException(); }
            }

            public override bool IsNamespaceOrContainerQualified()
            {
                throw new NotImplementedException();
            }

            public override T Accept<T>(IPathSegmentTokenVisitor<T> visitor)
            {
                throw new NotImplementedException();
            }

            public override void Accept(IPathSegmentTokenVisitor visitor)
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void NextTokenSetCorrectly()
        {
            DummyPathSegmentToken token = new DummyPathSegmentToken(new SystemToken("bob", null));
            token.NextToken.ShouldBeSystemToken("bob").And.NextToken.Should().BeNull();
        }
    }
}

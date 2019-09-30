﻿//---------------------------------------------------------------------
// <copyright file="PathSegmentTokenTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SyntacticAst
{
    public class PathSegmentTokenTests
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

        [Fact]
        public void NextTokenSetCorrectly()
        {
            DummyPathSegmentToken token = new DummyPathSegmentToken(new SystemToken("bob", null));
            Assert.Null(token.NextToken.ShouldBeSystemToken("bob").NextToken);
        }
    }
}

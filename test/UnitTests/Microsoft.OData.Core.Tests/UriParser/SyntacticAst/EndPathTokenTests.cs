//---------------------------------------------------------------------
// <copyright file="EndPathTokenTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SyntacticAst
{
    public class EndPathTokenTests
    {
        [Fact]
        public void NameCannotBeNull()
        {
            Action createWithNullName = () => new EndPathToken(null, new LiteralToken(1));
            Assert.Throws<ArgumentNullException>("identifier", createWithNullName);
        }

        [Fact]
        public void ParentSetCorrectly()
        {
            EndPathToken endPathToken = new EndPathToken("stuff", new LiteralToken(1));
            endPathToken.NextToken.ShouldBeLiteralQueryToken(1);
        }

        [Fact]
        public void NameSetCorrectly()
        {
            EndPathToken endPathToken = new EndPathToken("stuff", new LiteralToken(1));
            Assert.Equal("stuff", endPathToken.Identifier);
        }


        [Fact]
        public void KindIsInnerPath()
        {
            EndPathToken endPathToken = new EndPathToken("stuff", new LiteralToken(1));
            Assert.Equal(QueryTokenKind.EndPath, endPathToken.Kind);
        }
    }
}


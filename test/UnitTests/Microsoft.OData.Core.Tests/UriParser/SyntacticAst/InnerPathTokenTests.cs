//---------------------------------------------------------------------
// <copyright file="InnerPathTokenTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SyntacticAst
{
    public class InnerPathTokenTests
    {
        [Fact]
        public void NameCannotBeNull()
        {
            Action createWithNullName = () => new InnerPathToken(null, new LiteralToken(1), new NamedValue[] { new NamedValue("blah", new LiteralToken(1)) });
            Assert.Throws<ArgumentNullException>("identifier", createWithNullName);
        }

        [Fact]
        public void ParentSetCorrectly()
        {
            InnerPathToken innerPathToken = new InnerPathToken("stuff", new LiteralToken(1), new NamedValue[] { new NamedValue("blah", new LiteralToken(1)) });
            innerPathToken.NextToken.ShouldBeLiteralQueryToken(1);
        }

        [Fact]
        public void NameSetCorrectly()
        {
            InnerPathToken innerPathToken = new InnerPathToken("stuff", new LiteralToken(1), new NamedValue[] { new NamedValue("blah", new LiteralToken(1)) });
            Assert.Equal("stuff", innerPathToken.Identifier);
        }

        [Fact]
        public void NamedValuesSetCorrectly()
        {
            InnerPathToken innerPathToken = new InnerPathToken("stuff", new LiteralToken(1), new NamedValue[] { new NamedValue("blah", new LiteralToken(1)) });
            Assert.Single(innerPathToken.NamedValues);
            Assert.Equal("blah", innerPathToken.NamedValues.ElementAt(0).Name);
            innerPathToken.NamedValues.ElementAt(0).Value.ShouldBeLiteralQueryToken(1);
        }

        [Fact]
        public void KindIsInnerPath()
        {
            InnerPathToken innerPathToken = new InnerPathToken("stuff", new LiteralToken(1), new NamedValue[] { new NamedValue("blah", new LiteralToken(1)) });
            Assert.Equal(QueryTokenKind.InnerPath, innerPathToken.Kind);
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="InnerPathTokenTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Syntactic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.SyntacticAst
{
    public class InnerPathTokenTests
    {
        [Fact]
        public void NameCannotBeNull()
        {
            Action createWithNullName = () => new InnerPathToken(null, new LiteralToken(1), new NamedValue[] { new NamedValue("blah", new LiteralToken(1)) });
            createWithNullName.ShouldThrow<Exception>(Error.ArgumentNull("name").ToString());
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
            innerPathToken.Identifier.Should().Be("stuff");
        }

        [Fact]
        public void NamedValuesSetCorrectly()
        {
            InnerPathToken innerPathToken = new InnerPathToken("stuff", new LiteralToken(1), new NamedValue[] { new NamedValue("blah", new LiteralToken(1)) });
            innerPathToken.NamedValues.Count().Should().Be(1);
            innerPathToken.NamedValues.ElementAt(0).Name.Should().Be("blah");
            innerPathToken.NamedValues.ElementAt(0).Value.ShouldBeLiteralQueryToken(1);
        }

        [Fact]
        public void KindIsInnerPath()
        {
            InnerPathToken innerPathToken = new InnerPathToken("stuff", new LiteralToken(1), new NamedValue[] { new NamedValue("blah", new LiteralToken(1)) });
            innerPathToken.Kind.Should().Be(QueryTokenKind.InnerPath);
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="EndPathTokenTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Syntactic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.SyntacticAst
{
    public class EndPathTokenTests
    {
        [Fact]
        public void NameCannotBeNull()
        {
            Action createWithNullName = () => new EndPathToken(null, new LiteralToken(1));
            createWithNullName.ShouldThrow<Exception>(Error.ArgumentNull("name").ToString());
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
            endPathToken.Identifier.Should().Be("stuff");
        }


        [Fact]
        public void KindIsInnerPath()
        {
            EndPathToken endPathToken = new EndPathToken("stuff", new LiteralToken(1));
            endPathToken.Kind.Should().Be(QueryTokenKind.EndPath);
        }
    }
}


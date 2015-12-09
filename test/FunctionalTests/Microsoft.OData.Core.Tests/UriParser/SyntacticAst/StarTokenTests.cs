//---------------------------------------------------------------------
// <copyright file="StarTokenTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Microsoft.OData.Core.UriParser.Syntactic;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.SyntacticAst
{
    public class StarTokenTests
    {
        [Fact]
        public void ParentCanBeNull()
        {
            StarToken starToken = new StarToken(null);
            starToken.NextToken.Should().BeNull();
        }

        [Fact]
        public void ParentIsSetCorrectly()
        {
            StarToken starToken = new StarToken(new LiteralToken(1));
            starToken.NextToken.ShouldBeLiteralQueryToken(1);
        }

        [Fact]
        public void NameIsAlwaysStar()
        {
            StarToken starToken = new StarToken(null);
            starToken.Identifier.Should().Be("*");
        }
    }
}

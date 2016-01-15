//---------------------------------------------------------------------
// <copyright file="AggregateStatementTokenTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Extensions;
using Microsoft.OData.Core.UriParser.Extensions.Syntactic;
using Microsoft.OData.Core.UriParser.Syntactic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.Extensions.SyntacticAst
{
    public class AggregateStatementTokenTests
    {
        QueryToken expressionToken = new EndPathToken("Expression", null);

        [Fact]
        public void ExpressionCannotBeNull()
        {
            Action action = () => new AggregateStatementToken(null, AggregationVerb.Sum, "Alias");
            action.ShouldThrow<Exception>(Error.ArgumentNull("expression").ToString());
        }

        [Fact]
        public void AliasCannotBeNull()
        {
            Action action = () => new AggregateStatementToken(expressionToken, AggregationVerb.Sum, null);
            action.ShouldThrow<Exception>(Error.ArgumentNull("alias").ToString());
        }

        [Fact]
        public void ExpressionSetCorrectly()
        {
            var token = new AggregateStatementToken(expressionToken, AggregationVerb.Sum, "Alias");
            token.Expression.Should().Be(expressionToken);
        }

        [Fact]
        public void WithVerbSetCorrectly()
        {
            var token = new AggregateStatementToken(expressionToken, AggregationVerb.CountDistinct, "Alias");
            token.WithVerb.Should().Be(AggregationVerb.CountDistinct);
        }

        [Fact]
        public void AliasSetCorrectly()
        {
            var token = new AggregateStatementToken(expressionToken, AggregationVerb.CountDistinct, "Alias");
            token.AsAlias.Should().BeEquivalentTo("Alias");
        }

        [Fact]
        public void KindIsSetCorrectly()
        {
            var token = new AggregateStatementToken(expressionToken, AggregationVerb.CountDistinct, "Alias");
            token.Kind.Should().Be(QueryTokenKind.AggregateStatement);
        }
    }
}


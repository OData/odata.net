﻿//---------------------------------------------------------------------
// <copyright file="AggregateExpressionTokenTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.UriParser;
using Microsoft.OData.UriParser.Aggregation;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Extensions.SyntacticAst
{
    public class AggregateExpressionTokenTests
    {
        QueryToken expressionToken = new EndPathToken("Expression", null);

        [Fact]
        public void ExpressionCannotBeNull()
        {
            Action action = () => new AggregateExpressionToken(null, AggregationMethod.Sum, "Alias");
            action.ShouldThrow<Exception>(Error.ArgumentNull("expression").ToString());
        }

        [Fact]
        public void AliasCannotBeNull()
        {
            Action action = () => new AggregateExpressionToken(expressionToken, AggregationMethod.Sum, null);
            action.ShouldThrow<Exception>(Error.ArgumentNull("alias").ToString());
        }

        [Fact]
        public void ExpressionSetCorrectly()
        {
            AggregateExpressionToken token = new AggregateExpressionToken(expressionToken, AggregationMethod.Sum, "Alias");
            token.Expression.Should().Be(expressionToken);
        }

        [Fact]
        public void WithMethodSetCorrectly()
        {
            AggregateExpressionToken token = new AggregateExpressionToken(expressionToken, AggregationMethod.CountDistinct, "Alias");
            token.Method.Should().Be(AggregationMethod.CountDistinct);
        }

        [Fact]
        public void AliasSetCorrectly()
        {
            AggregateExpressionToken token = new AggregateExpressionToken(expressionToken, AggregationMethod.CountDistinct, "Alias");
            token.Alias.Should().BeEquivalentTo("Alias");
        }

        [Fact]
        public void KindIsSetCorrectly()
        {
            AggregateExpressionToken token = new AggregateExpressionToken(expressionToken, AggregationMethod.CountDistinct, "Alias");
            token.Kind.Should().Be(QueryTokenKind.AggregateExpression);
        }
    }
}


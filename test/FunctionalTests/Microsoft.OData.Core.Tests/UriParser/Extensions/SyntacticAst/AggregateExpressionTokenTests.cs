//---------------------------------------------------------------------
// <copyright file="AggregateExpressionTokenTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
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
            Assert.Throws<ArgumentNullException>("expression", action);
        }

        [Fact]
        public void AliasCannotBeNull()
        {
            Action action = () => new AggregateExpressionToken(expressionToken, AggregationMethod.Sum, null);
            Assert.Throws<ArgumentNullException>("alias", action);
        }

        [Fact]
        public void ExpressionSetCorrectly()
        {
            AggregateExpressionToken token = new AggregateExpressionToken(expressionToken, AggregationMethod.Sum, "Alias");
            Assert.Same(expressionToken, token.Expression);
        }

        [Fact]
        public void WithMethodSetCorrectly()
        {
            AggregateExpressionToken token = new AggregateExpressionToken(expressionToken, AggregationMethod.CountDistinct, "Alias");
            Assert.Equal(AggregationMethod.CountDistinct, token.Method);
        }

        [Fact]
        public void AliasSetCorrectly()
        {
            AggregateExpressionToken token = new AggregateExpressionToken(expressionToken, AggregationMethod.CountDistinct, "Alias");
            Assert.Equal("Alias", token.Alias);
        }

        [Fact]
        public void KindIsSetCorrectly()
        {
            AggregateExpressionToken token = new AggregateExpressionToken(expressionToken, AggregationMethod.CountDistinct, "Alias");
            Assert.Equal(QueryTokenKind.AggregateExpression, token.Kind);
        }
    }
}


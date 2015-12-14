//---------------------------------------------------------------------
// <copyright file="EndPathTokenUnitTests.cs" company="Microsoft">
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
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.OData.Core.UriParser.Visitors;

    [TestClass]
    public class AggregateStatementTokenUnitTests
    {
        QueryToken expressionToken = new EndPathToken("Expression", null);

        [TestMethod]
        public void ExpressionCannotBeNull()
        {
            Action action = () => new AggregateStatementToken(null, AggregationVerb.Sum, "Alias");
            action.ShouldThrow<Exception>(Error.ArgumentNull("expression").ToString());
        }      

        [TestMethod]
        public void AliasCannotBeNull()
        {
            Action action = () => new AggregateStatementToken(expressionToken, AggregationVerb.Sum, null);
            action.ShouldThrow<Exception>(Error.ArgumentNull("alias").ToString());
        }

        [TestMethod]
        public void ExpressionSetCorrectly()
        {            
            var token = new AggregateStatementToken(expressionToken, AggregationVerb.Sum, "Alias");
            token.Expression.Should().Be(expressionToken);
        }

        [TestMethod]
        public void WithVerbSetCorrectly()
        {
            var token = new AggregateStatementToken(expressionToken, AggregationVerb.CountDistinct, "Alias");
            token.WithVerb.Should().Be(AggregationVerb.CountDistinct);
        }

        [TestMethod]
        public void AliasSetCorrectly()
        {
            var token = new AggregateStatementToken(expressionToken, AggregationVerb.CountDistinct, "Alias");
            token.AsAlias.Should().BeEquivalentTo("Alias");
        }

        [TestMethod]
        public void KindIsSetCorrectly()
        {            
            var token = new AggregateStatementToken(expressionToken, AggregationVerb.CountDistinct, "Alias");
            token.Kind.Should().Be(QueryTokenKind.AggregateStatement);
        }
    }
}


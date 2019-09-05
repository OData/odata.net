//---------------------------------------------------------------------
// <copyright file="AggregateTokenTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.UriParser;
using Microsoft.OData.UriParser.Aggregation;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Extensions.SyntacticAst
{
    public class AggregateTokenTests
    {
        private IEnumerable<AggregateTokenBase> statements = new List<AggregateTokenBase>();

        [Fact]
        public void StatementsCannotBeNull()
        {
            Action action = () => new AggregateToken(null);
            Assert.Throws<ArgumentNullException>("expressions", action);
        }

        [Fact]
        public void StatementsSetCorrectly()
        {
            AggregateToken token = new AggregateToken(statements);
            Assert.Same(statements, token.AggregateExpressions);
        }

        [Fact]
        public void KindIsSetCorrectly()
        {
            AggregateToken token = new AggregateToken(statements);
            Assert.Equal(QueryTokenKind.Aggregate, token.Kind);
        }
    }
}


//---------------------------------------------------------------------
// <copyright file="AggregateTokenTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Extensions.Syntactic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.Extensions.SyntacticAst
{
    public class AggregateTokenTests
    {
        private IEnumerable<AggregateStatementToken> statements = new List<AggregateStatementToken>();

        [Fact]
        public void StatementsCannotBeNull()
        {
            Action action = () => new AggregateToken(null);
            action.ShouldThrow<Exception>(Error.ArgumentNull("statements").ToString());
        }

        [Fact]
        public void StatementsSetCorrectly()
        {
            var token = new AggregateToken(statements);
            ((object)token.Statements).Should().Be(statements);
        }

        [Fact]
        public void KindIsSetCorrectly()
        {
            var token = new AggregateToken(statements);
            token.Kind.Should().Be(QueryTokenKind.Aggregate);
        }
    }
}


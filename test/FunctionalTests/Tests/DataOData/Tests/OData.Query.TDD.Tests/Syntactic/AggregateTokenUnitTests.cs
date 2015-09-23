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
    public class AggregateTokenUnitTests
    {
        private IEnumerable<AggregateStatementToken> statements = new List<AggregateStatementToken>();

        [TestMethod]
        public void StatementsCannotBeNull()
        {
            Action action = () => new AggregateToken(null);
            action.ShouldThrow<Exception>(Error.ArgumentNull("statements").ToString());
        }

        [TestMethod]
        public void StatementsSetCorrectly()
        {            
            var token = new AggregateToken(statements);
            ((object)token.Statements).Should().Be(statements);
        }

        [TestMethod]
        public void KindIsSetCorrectly()
        {
            var token = new AggregateToken(statements);
            token.Kind.Should().Be(QueryTokenKind.Aggregate);
        }        
    }
}


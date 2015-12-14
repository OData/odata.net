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
    public class GroupByTokenUnitTests
    {
        private IEnumerable<EndPathToken> properties = new List<EndPathToken>();

        private AggregateToken aggregate = new AggregateToken(new List<AggregateStatementToken>());

        [TestMethod]
        public void PropertiesCannotBeNull()
        {
            Action action = () => new GroupByToken(null, aggregate);
            action.ShouldThrow<Exception>(Error.ArgumentNull("properties").ToString());
        }

        [TestMethod]
        public void AggregateCanBeNull()
        {
            Action action = () => new GroupByToken(properties, null);
            action.ShouldNotThrow();
        }

        [TestMethod]
        public void PropertiesSetCorrectly()
        {
            var token = new GroupByToken(properties, null);
            ((object)token.Properties).Should().Be(properties);
        }

        [TestMethod]
        public void AggregateSetCorrectly()
        {
            var token = new GroupByToken(properties, aggregate);
            ((object)token.Child).Should().Be(aggregate);
        }

        [TestMethod]
        public void KindIsSetCorrectly()
        {
            var token = new GroupByToken(properties, null);

            token.Kind.Should().Be(QueryTokenKind.GroupBy);
        }        
    }
}


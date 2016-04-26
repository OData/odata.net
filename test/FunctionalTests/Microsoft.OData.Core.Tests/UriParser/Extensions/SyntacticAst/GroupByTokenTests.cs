//---------------------------------------------------------------------
// <copyright file="GroupByTokenTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OData.UriParser;
using Microsoft.OData.UriParser.Aggregation;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Extensions.SyntacticAst
{
    public class GroupByTokenTests
    {
        private IEnumerable<EndPathToken> properties = new List<EndPathToken>();

        private AggregateToken aggregate = new AggregateToken(new List<AggregateExpressionToken>());

        [Fact]
        public void PropertiesCannotBeNull()
        {
            Action action = () => new GroupByToken(null, aggregate);
            action.ShouldThrow<Exception>(Error.ArgumentNull("properties").ToString());
        }

        [Fact]
        public void AggregateCanBeNull()
        {
            Action action = () => new GroupByToken(properties, null);
            action.ShouldNotThrow();
        }

        [Fact]
        public void PropertiesSetCorrectly()
        {
            var token = new GroupByToken(properties, null);
            ((object)token.Properties).Should().Be(properties);
        }

        [Fact]
        public void AggregateSetCorrectly()
        {
            var token = new GroupByToken(properties, aggregate);
            ((object)token.Child).Should().Be(aggregate);
        }

        [Fact]
        public void KindIsSetCorrectly()
        {
            var token = new GroupByToken(properties, null);

            token.Kind.Should().Be(QueryTokenKind.AggregateGroupBy);
        }
    }
}


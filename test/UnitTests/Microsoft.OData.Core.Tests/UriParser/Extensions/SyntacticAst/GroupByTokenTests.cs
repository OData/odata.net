//---------------------------------------------------------------------
// <copyright file="GroupByTokenTests.cs" company="Microsoft">
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
    public class GroupByTokenTests
    {
        private IEnumerable<EndPathToken> properties = new List<EndPathToken>();

        private AggregateToken aggregate = new AggregateToken(new List<AggregateTokenBase>());

        [Fact]
        public void PropertiesCannotBeNull()
        {
            Action action = () => new GroupByToken(null, aggregate);
            Assert.Throws<ArgumentNullException>("properties", action);
        }

        [Fact]
        public void AggregateCanBeNull()
        {
            Action action = () => new GroupByToken(properties, null);
            action.DoesNotThrow();
        }

        [Fact]
        public void PropertiesSetCorrectly()
        {
            var token = new GroupByToken(properties, null);
            Assert.Same(properties, ((object)token.Properties));
        }

        [Fact]
        public void AggregateSetCorrectly()
        {
            var token = new GroupByToken(properties, aggregate);
            Assert.Same(aggregate, ((object)token.Child));
        }

        [Fact]
        public void KindIsSetCorrectly()
        {
            var token = new GroupByToken(properties, null);

            Assert.Equal(QueryTokenKind.AggregateGroupBy, token.Kind);
        }
    }
}


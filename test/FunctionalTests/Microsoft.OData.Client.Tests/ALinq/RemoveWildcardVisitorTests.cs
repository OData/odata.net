//---------------------------------------------------------------------
// <copyright file="RemoveWildcardVisitorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Client.ALinq.UriParser;
using Microsoft.OData.Core.UriParser;
using Xunit;

namespace Microsoft.OData.Client.Tests.ALinq
{
    /// <summary>
    /// Tests ClientEdmModel functionalities
    /// </summary>
    public class RemoveWildcardVisitorTests
    {
        [Fact]
        public void SystemTokenThrows()
        {
            RemoveWildcardVisitor visitor = new RemoveWildcardVisitor();
            SystemToken token = new SystemToken(ExpressionConstants.It, null);
            Action visitSystemToken = () => token.Accept(visitor);
            visitSystemToken.ShouldThrow<NotSupportedException>().WithMessage(Strings.ALinq_IllegalSystemQueryOption(ExpressionConstants.It));
        }

        [Fact]
        public void WildcardIsRemoved()
        {
            RemoveWildcardVisitor visitor = new RemoveWildcardVisitor();
            NonSystemToken token = new NonSystemToken("stuff", null, new NonSystemToken("*", null, null));
            token.Accept(visitor);
            token.Identifier.Should().Be("stuff");
            token.NextToken.Should().BeNull();
        }

        [Fact]
        public void PathWithoutWildcardIsInvariant()
        {
            RemoveWildcardVisitor visitor = new RemoveWildcardVisitor();
            NonSystemToken token = new NonSystemToken("stuff", null, null);
            token.Accept(visitor);
            token.Identifier.Should().Be("stuff");
            token.NextToken.Should().BeNull();
        }
    }
}

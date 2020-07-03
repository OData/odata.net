//---------------------------------------------------------------------
// <copyright file="RemoveWildcardVisitorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Client.ALinq.UriParser;
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
            NotSupportedException exception = Assert.Throws<NotSupportedException>(visitSystemToken);
            Assert.Equal(Strings.ALinq_IllegalSystemQueryOption(ExpressionConstants.It), exception.Message);
        }

        [Fact]
        public void WildcardIsRemoved()
        {
            RemoveWildcardVisitor visitor = new RemoveWildcardVisitor();
            NonSystemToken token = new NonSystemToken("stuff", null, new NonSystemToken("*", null, null));
            token.Accept(visitor);
            Assert.Equal("stuff", token.Identifier);
            Assert.Null(token.NextToken);
        }

        [Fact]
        public void PathWithoutWildcardIsInvariant()
        {
            RemoveWildcardVisitor visitor = new RemoveWildcardVisitor();
            NonSystemToken token = new NonSystemToken("stuff", null, null);
            token.Accept(visitor);

            Assert.Equal("stuff", token.Identifier);
            Assert.Null(token.NextToken);
        }
    }
}

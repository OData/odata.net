//---------------------------------------------------------------------
// <copyright file="AddNewEndingTokenVisitorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Client.ALinq.UriParser;
using Microsoft.OData;
using Xunit;

namespace Microsoft.OData.Client.Tests.ALinq
{
    /// <summary>
    /// Tests ClientEdmModel functionalities
    /// </summary>
    public class AddNewEndingTokenVisitorTests
    {
        [Fact]
        public void SystemTokensThrowNotSupportedError()
        {
            AddNewEndingTokenVisitor visitor = new AddNewEndingTokenVisitor(new NonSystemToken("stuff", null, null));
            SystemToken token = new SystemToken(ExpressionConstants.It, null);
            Action visitSystemToken = () => token.Accept(visitor);

            NotSupportedException exception = Assert.Throws<NotSupportedException>(visitSystemToken);
            Assert.Equal(Strings.ALinq_IllegalSystemQueryOption(ExpressionConstants.It), exception.Message);
        }

        [Fact]
        public void IfNewTokenIsNullInputIsInvariant()
        {
            AddNewEndingTokenVisitor visitor = new AddNewEndingTokenVisitor(null);
            NonSystemToken token = new NonSystemToken("stuff", null, null);
            token.Accept(visitor);
            Assert.Equal("stuff", token.Identifier);
            Assert.Null(token.NextToken);
        }

        [Fact]
        public void IfNewTokenIsPresentItIsAddedToEndOfPath()
        {
            AddNewEndingTokenVisitor visitor = new AddNewEndingTokenVisitor(new NonSystemToken("moreStuff", null, null));
            NonSystemToken token = new NonSystemToken("stuff", null, null);
            token.Accept(visitor);

            Assert.Equal("stuff", token.Identifier);
            Assert.Equal("moreStuff", token.NextToken.Identifier);
            Assert.Null(token.NextToken.NextToken);
        }
    }
}

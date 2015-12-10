//---------------------------------------------------------------------
// <copyright file="AddNewEndingTokenVisitorTests.cs" company="Microsoft">
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
    public class AddNewEndingTokenVisitorTests
    {
        [Fact]
        public void SystemTokensThrowNotSupportedError()
        {
            AddNewEndingTokenVisitor visitor = new AddNewEndingTokenVisitor(new NonSystemToken("stuff", null, null));
            SystemToken token = new SystemToken(ExpressionConstants.It, null);
            Action visitSystemToken = () => token.Accept(visitor);
            visitSystemToken.ShouldThrow<NotSupportedException>().WithMessage(Strings.ALinq_IllegalSystemQueryOption(ExpressionConstants.It));
        }

        [Fact]
        public void IfNewTokenIsNullInputIsInvariant()
        {
            AddNewEndingTokenVisitor visitor = new AddNewEndingTokenVisitor(null);
            NonSystemToken token = new NonSystemToken("stuff", null, null);
            token.Accept(visitor);
            token.Identifier.Should().Be("stuff");
            token.NextToken.Should().BeNull();
        }

        [Fact]
        public void IfNewTokenIsPresentItIsAddedToEndOfPath()
        {
            AddNewEndingTokenVisitor visitor = new AddNewEndingTokenVisitor(new NonSystemToken("moreStuff", null, null));
            NonSystemToken token = new NonSystemToken("stuff", null, null);
            token.Accept(visitor);
            token.Identifier.Should().Be("stuff");
            token.NextToken.Identifier.Should().Be("moreStuff");
            token.NextToken.NextToken.Should().BeNull();
        }
    }
}

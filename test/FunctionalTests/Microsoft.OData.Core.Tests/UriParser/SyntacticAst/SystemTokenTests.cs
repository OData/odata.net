//---------------------------------------------------------------------
// <copyright file="SystemTokenTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Syntactic;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.SyntacticAst
{
    public class SystemTokenTests
    {
        [Fact]
        public void IdentifierCannotBeNull()
        {
            Action createWithNullIdentifier = () => new SystemToken(null, null);
            createWithNullIdentifier.ShouldThrow<Exception>(Error.ArgumentNull("identifier").ToString());
        }

        [Fact]
        public void IdentifierSetCorrectly()
        {
            SystemToken token = new SystemToken("stuff", null);
            token.Identifier.Should().Be("stuff");
        }

        [Fact]
        public void IsNamespaceOrContainerQualifiedIsFalse()
        {
            SystemToken token = new SystemToken("more stuff", null);
            token.IsNamespaceOrContainerQualified().Should().BeFalse();
        }
    }
}

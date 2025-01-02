//---------------------------------------------------------------------
// <copyright file="SystemTokenTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SyntacticAst
{
    public class SystemTokenTests
    {
        [Fact]
        public void IdentifierCannotBeNull()
        {
            Action createWithNullIdentifier = () => new SystemToken(null, null);
            Assert.Throws<ArgumentNullException>("identifier", createWithNullIdentifier);
        }

        [Fact]
        public void IdentifierSetCorrectly()
        {
            SystemToken token = new SystemToken("stuff", null);
            Assert.Equal("stuff", token.Identifier);
        }

        [Fact]
        public void IsNamespaceOrContainerQualifiedIsFalse()
        {
            SystemToken token = new SystemToken("more stuff", null);
            Assert.False(token.IsNamespaceOrContainerQualified());
        }
    }
}

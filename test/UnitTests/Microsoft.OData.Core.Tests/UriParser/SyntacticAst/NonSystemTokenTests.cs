//---------------------------------------------------------------------
// <copyright file="NonSystemTokenTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.SyntacticAst
{
    public class NonSystemTokenTests
    {
        [Fact]
        public void IdentifierCannotBeNull()
        {
            Action createWithNullIdentifier = () => new NonSystemToken(null, null, null);
            Assert.Throws<ArgumentNullException>("identifier", createWithNullIdentifier);
        }

        [Fact]
        public void IdentifierSetCorrectly()
        {
            NonSystemToken token = new NonSystemToken("stuff", null, null);
            Assert.Equal("stuff", token.Identifier);
        }

        [Fact]
        public void NamedValuesSetCorrectly()
        {
            List<NamedValue> namedValues = new List<NamedValue>();
            namedValues.Add(new NamedValue("name", new LiteralToken("value")));

            NonSystemToken token = new NonSystemToken("stuff", namedValues, null);
            var nameValue = Assert.Single(token.NamedValues);
            Assert.Equal("name", nameValue.Name);
            Assert.Equal("value", Assert.IsType<LiteralToken>(nameValue.Value).Value);
        }

        [Fact]
        public void IsNamespaceOrContainerQualifiedIsCorrect()
        {
            NonSystemToken token1 = new NonSystemToken("Fully.Qualified.Namespace", null, null);
            Assert.True(token1.IsNamespaceOrContainerQualified());
            NonSystemToken token2 = new NonSystemToken("namespace", null, null);
            Assert.False(token2.IsNamespaceOrContainerQualified());
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="NonSystemTokenTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Syntactic;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.SyntacticAst
{
    public class NonSystemTokenTests
    {
        [Fact]
        public void IdentifierCannotBeNull()
        {
            Action createWithNullIdentifier = () => new NonSystemToken(null, null, null);
            createWithNullIdentifier.ShouldThrow<Exception>(Error.ArgumentNull("identifier").ToString());
        }

        [Fact]
        public void IdentifierSetCorrectly()
        {
            NonSystemToken token = new NonSystemToken("stuff", null, null);
            token.Identifier.Should().Be("stuff");
        }

        [Fact]
        public void NamedValuesSetCorrectly()
        {
            List<NamedValue> namedValues = new List<NamedValue>();
            namedValues.Add(new NamedValue("name", new LiteralToken("value")));

            NonSystemToken token = new NonSystemToken("stuff", namedValues, null);
            token.NamedValues.Should().OnlyContain(x => x.Name == "name" && x.Value.Value.As<string>() == "value");
        }

        [Fact]
        public void IsNamespaceOrContainerQualifiedIsCorrect()
        {
            NonSystemToken token1 = new NonSystemToken("Fully.Qualified.Namespace", null, null);
            token1.IsNamespaceOrContainerQualified().Should().BeTrue();
            NonSystemToken token2 = new NonSystemToken("namespace", null, null);
            token2.IsNamespaceOrContainerQualified().Should().BeFalse();
        }
    }
}

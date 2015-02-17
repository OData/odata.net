//---------------------------------------------------------------------
// <copyright file="EndPathTokenUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Syntactic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EndPathTokenUnitTests
    {
        [TestMethod]
        public void NameCannotBeNull()
        {
            Action createWithNullName = () => new EndPathToken(null, new LiteralToken(1));
            createWithNullName.ShouldThrow<Exception>(Error.ArgumentNull("name").ToString());
        }

        [TestMethod]
        public void ParentSetCorrectly()
        {
            EndPathToken endPathToken = new EndPathToken("stuff", new LiteralToken(1));
            endPathToken.NextToken.ShouldBeLiteralQueryToken(1);
        }

        [TestMethod]
        public void NameSetCorrectly()
        {
            EndPathToken endPathToken = new EndPathToken("stuff", new LiteralToken(1));
            endPathToken.Identifier.Should().Be("stuff");
        }


        [TestMethod]
        public void KindIsInnerPath()
        {
            EndPathToken endPathToken = new EndPathToken("stuff", new LiteralToken(1));
            endPathToken.Kind.Should().Be(QueryTokenKind.EndPath);
        }
    }
}


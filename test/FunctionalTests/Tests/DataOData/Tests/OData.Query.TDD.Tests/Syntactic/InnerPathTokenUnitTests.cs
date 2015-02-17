//---------------------------------------------------------------------
// <copyright file="InnerPathTokenUnitTests.cs" company="Microsoft">
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
    public class InnerPathTokenUnitTests
    {
        [TestMethod]
        public void NameCannotBeNull()
        {
            Action createWithNullName = () => new InnerPathToken(null, new LiteralToken(1), new NamedValue[] { new NamedValue("blah", new LiteralToken(1)) });
            createWithNullName.ShouldThrow<Exception>(Error.ArgumentNull("name").ToString());
        }

        [TestMethod]
        public void ParentSetCorrectly()
        {
            InnerPathToken innerPathToken = new InnerPathToken("stuff", new LiteralToken(1), new NamedValue[] { new NamedValue("blah", new LiteralToken(1)) });
            innerPathToken.NextToken.ShouldBeLiteralQueryToken(1);
        }

        [TestMethod]
        public void NameSetCorrectly()
        {
            InnerPathToken innerPathToken = new InnerPathToken("stuff", new LiteralToken(1), new NamedValue[] { new NamedValue("blah", new LiteralToken(1)) });
            innerPathToken.Identifier.Should().Be("stuff");
        }

        [TestMethod]
        public void NamedValuesSetCorrectly()
        {
            InnerPathToken innerPathToken = new InnerPathToken("stuff", new LiteralToken(1), new NamedValue[] { new NamedValue("blah", new LiteralToken(1)) });
            innerPathToken.NamedValues.Count().Should().Be(1);
            innerPathToken.NamedValues.ElementAt(0).Name.Should().Be("blah");
            innerPathToken.NamedValues.ElementAt(0).Value.ShouldBeLiteralQueryToken(1);
        }

        [TestMethod]
        public void KindIsInnerPath()
        {
            InnerPathToken innerPathToken = new InnerPathToken("stuff", new LiteralToken(1), new NamedValue[] { new NamedValue("blah", new LiteralToken(1)) });
            innerPathToken.Kind.Should().Be(QueryTokenKind.InnerPath);
        }
    }
}

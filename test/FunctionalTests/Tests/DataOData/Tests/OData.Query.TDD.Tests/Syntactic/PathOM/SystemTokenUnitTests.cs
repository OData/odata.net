//---------------------------------------------------------------------
// <copyright file="SystemTokenUnitTests.cs" company="Microsoft">
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
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SystemTokenUnitTests
    {
        [TestMethod]
        public void IdentifierCannotBeNull()
        {
            Action createWithNullIdentifier = () => new SystemToken(null, null);
            createWithNullIdentifier.ShouldThrow<Exception>(Error.ArgumentNull("identifier").ToString());
        }

        [TestMethod]
        public void IdentifierSetCorrectly()
        {
            SystemToken token = new SystemToken("stuff", null);
            token.Identifier.Should().Be("stuff");
        }

        [TestMethod]
        public void IsNamespaceOrContainerQualifiedIsFalse()
        {
            SystemToken token = new SystemToken("more stuff", null);
            token.IsNamespaceOrContainerQualified().Should().BeFalse();
        }
    }
}

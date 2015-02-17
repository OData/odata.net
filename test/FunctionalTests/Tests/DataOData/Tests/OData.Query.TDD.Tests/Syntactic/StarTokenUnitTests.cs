//---------------------------------------------------------------------
// <copyright file="StarTokenUnitTests.cs" company="Microsoft">
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
    public class StarTokenUnitTests
    {
        [TestMethod]
        public void ParentCanBeNull()
        {
            StarToken starToken = new StarToken(null);
            starToken.NextToken.Should().BeNull();
        }

        [TestMethod]
        public void ParentIsSetCorrectly()
        {
            StarToken starToken = new StarToken(new LiteralToken(1));
            starToken.NextToken.ShouldBeLiteralQueryToken(1);
        }

        [TestMethod]
        public void NameIsAlwaysStar()
        {
            StarToken starToken = new StarToken(null);
            starToken.Identifier.Should().Be("*");
        }
    }
}

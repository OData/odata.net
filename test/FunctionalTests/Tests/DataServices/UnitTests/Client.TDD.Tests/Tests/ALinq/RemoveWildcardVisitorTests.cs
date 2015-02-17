//---------------------------------------------------------------------
// <copyright file="RemoveWildcardVisitorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Reflection;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Microsoft.OData.Client;
using Microsoft.OData.Client.Metadata;
using System.Linq;
using Microsoft.CSharp;
using Microsoft.OData.Edm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace AstoriaUnitTests.Tests.ALinq
{
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.OData.Client.ALinq.UriParser;
    using Microsoft.OData.Core.UriParser;

    /// <summary>
    /// Tests ClientEdmModel functionalities
    /// </summary>

    [TestClass]
    public class RemoveWildcardVisitorTests
    {
        [TestMethod]
        public void SystemTokenThrows()
        {
            RemoveWildcardVisitor visitor = new RemoveWildcardVisitor();
            SystemToken token = new SystemToken(ExpressionConstants.It, null);
            Action visitSystemToken = () => token.Accept(visitor);
            visitSystemToken.ShouldThrow<NotSupportedException>().WithMessage(Strings.ALinq_IllegalSystemQueryOption(ExpressionConstants.It));
        }

        [TestMethod]
        public void WildcardIsRemoved()
        {
            RemoveWildcardVisitor visitor = new RemoveWildcardVisitor();
            NonSystemToken token = new NonSystemToken("stuff", null, new NonSystemToken("*", null, null));
            token.Accept(visitor);
            token.Identifier.Should().Be("stuff");
            token.NextToken.Should().BeNull();
        }

        [TestMethod]
        public void PathWithoutWildcardIsInvariant()
        {
            RemoveWildcardVisitor visitor = new RemoveWildcardVisitor();
            NonSystemToken token = new NonSystemToken("stuff", null, null);
            token.Accept(visitor);
            token.Identifier.Should().Be("stuff");
            token.NextToken.Should().BeNull();
        }
    }
}

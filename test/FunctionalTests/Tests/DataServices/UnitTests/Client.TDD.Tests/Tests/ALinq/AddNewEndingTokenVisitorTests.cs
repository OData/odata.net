//---------------------------------------------------------------------
// <copyright file="AddNewEndingTokenVisitorTests.cs" company="Microsoft">
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
    public class AddNewEndingTokenVisitorTests
    {
        [TestMethod]
        public void SystemTokensThrowNotSupportedError()
        {
            AddNewEndingTokenVisitor visitor = new AddNewEndingTokenVisitor(new NonSystemToken("stuff", null, null));
            SystemToken token = new SystemToken(ExpressionConstants.It, null);
            Action visitSystemToken = () => token.Accept(visitor);
            visitSystemToken.ShouldThrow<NotSupportedException>().WithMessage(Strings.ALinq_IllegalSystemQueryOption(ExpressionConstants.It));
        }

        [TestMethod]
        public void IfNewTokenIsNullInputIsInvariant()
        {
            AddNewEndingTokenVisitor visitor = new AddNewEndingTokenVisitor(null);
            NonSystemToken token = new NonSystemToken("stuff", null, null);
            token.Accept(visitor);
            token.Identifier.Should().Be("stuff");
            token.NextToken.Should().BeNull();
        }

        [TestMethod]
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

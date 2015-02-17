//---------------------------------------------------------------------
// <copyright file="ExpandPathToStringVisitorTests.cs" company="Microsoft">
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
    public class ExpandPathToStringVisitorTests
    {
        [TestMethod]
        public void SystemTokenThrows()
        {
            SelectExpandPathToStringVisitor visitor = new SelectExpandPathToStringVisitor();
            SystemToken systemToken = new SystemToken(ExpressionConstants.It, null);
            Action visitSystemToken = () => systemToken.Accept(visitor);
            visitSystemToken.ShouldThrow<NotSupportedException>().WithMessage(Strings.ALinq_IllegalSystemQueryOption(ExpressionConstants.It));
        }

        [TestMethod]
        public void ExpandOnlyPathResultsInExpandOnlyTree()
        {
            SelectExpandPathToStringVisitor visitor = new SelectExpandPathToStringVisitor();
            NonSystemToken path = new NonSystemToken("NavProp", null, new NonSystemToken("NavProp1", null, null));
            path.IsStructuralProperty = false;
            path.NextToken.IsStructuralProperty = false;
            path.Accept(visitor).Should().Be("NavProp($expand=NavProp1)");
        }

        [TestMethod]
        public void SingleExpandResultsInSingleExpandTree()
        {
            SelectExpandPathToStringVisitor visitor = new SelectExpandPathToStringVisitor();
            NonSystemToken path = new NonSystemToken("NavProp", null, null);
            path.IsStructuralProperty = false;
            path.Accept(visitor).Should().Be("NavProp");
        }

        [TestMethod]
        public void SelectAtTheEndOfPathResultsInSelectQueryOption()
        {
            SelectExpandPathToStringVisitor visitor = new SelectExpandPathToStringVisitor();
            NonSystemToken path = new NonSystemToken("NavProp", null, new NonSystemToken("StructuralProp", null, null));
            path.IsStructuralProperty = false;
            path.NextToken.IsStructuralProperty = true;
            path.Accept(visitor).Should().Be("NavProp($select=StructuralProp)");
        }
    }
}

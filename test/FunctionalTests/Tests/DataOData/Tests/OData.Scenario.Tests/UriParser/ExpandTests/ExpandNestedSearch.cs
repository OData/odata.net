//---------------------------------------------------------------------
// <copyright file="ExpandNestedSearch.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Scenario.Tests.UriParser.ExpandTests
{
    #region namespaces
    using System;
    using System.Runtime.CompilerServices;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion namespaces

    [TestClass]
    public class ExpandNestedSearch : UriParserTestsBase
    {
        [TestMethod]
        public void Word()
        {
            this.ApprovalVerifyExpandParser(orderBase, "CustomerForOrder($search=bike)");
        }

        [TestMethod]
        public void Phrase()
        {
            this.ApprovalVerifyExpandParser(orderBase, "CustomerForOrder($search=\"mountain bike\")");
        }

        [TestMethod]
        public void ImplicitAnd()
        {
            this.ApprovalVerifyExpandParser(orderBase, "CustomerForOrder($search=mountain bike)");
        }

        [TestMethod]
        public void KeyWord()
        {
            this.ApprovalVerifyExpandParser(orderBase, "CustomerForOrder($search=mountain OR bike AND NOT clothing)");

            this.TestAllInOneExtensionExpand(
               orderBase,
               "customerfororder($SEARCH=mountain OR bike AND NOT clothing)",
               "CustomerForOrder($search=mountain OR bike AND NOT clothing)");
        }

        [TestMethod]
        public void KeywordWithParenthesis()
        {
            this.ApprovalVerifyExpandParser(orderBase, "CustomerForOrder($search=(mountain OR bike) AND NOT clothing)");
        }
    }
}

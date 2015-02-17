//---------------------------------------------------------------------
// <copyright file="SearchTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Scenario.Tests.UriParser.Search
{
    using System;
    using System.Runtime.CompilerServices;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Scenario.Tests.UriParser;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// End user tests for ParseFilter method on the UriParser
    /// </summary>
    [TestClass]
    public class SearchTests : UriParserTestsBase
    {
        [TestMethod]
        public void Word()
        {
            this.ApprovalVerifySearchParser(peopleBase, "bike");
        }

        [TestMethod]
        public void Phrase()
        {
            this.ApprovalVerifySearchParser(peopleBase, "\"mountain bike\"");
        }

        [TestMethod]
        public void ImplicitAnd()
        {
            this.ApprovalVerifySearchParser(peopleBase, "mountain bike");
        }

        [TestMethod]
        public void ImplicitAndWithParenThesis()
        {
            this.ApprovalVerifySearchParser(peopleBase, "(mountain car) (bike NOT P)");
        }

        [TestMethod]
        public void KeyWord()
        {
            this.ApprovalVerifySearchParser(peopleBase, "mountain OR bike AND NOT clothing");
        }

        [TestMethod]
        public void KeywordWithParenthesis()
        {
            this.ApprovalVerifySearchParser(peopleBase, "(mountain OR bike) AND NOT clothing");
        }
    }
}

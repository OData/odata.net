//---------------------------------------------------------------------
// <copyright file="SearchTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Scenario.Tests.UriParser.Search
{
    using System.Runtime.CompilerServices;
    using Microsoft.Test.Taupo.OData.Scenario.Tests.UriParser;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// End user tests for ParseFilter method on the UriParser
    /// </summary>
    [TestClass]
    public class SearchTests : UriParserTestsBase
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        [TestMethod]
        public void Word()
        {
            this.ApprovalVerifySearchParser(peopleBase, "bike");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [TestMethod]
        public void Phrase()
        {
            this.ApprovalVerifySearchParser(peopleBase, "\"mountain bike\"");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [TestMethod]
        public void ImplicitAnd()
        {
            this.ApprovalVerifySearchParser(peopleBase, "mountain bike");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [TestMethod]
        public void ImplicitAndWithParenThesis()
        {
            this.ApprovalVerifySearchParser(peopleBase, "(mountain car) (bike NOT P)");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [TestMethod]
        public void KeyWord()
        {
            this.ApprovalVerifySearchParser(peopleBase, "mountain OR bike AND NOT clothing");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [TestMethod]
        public void KeywordWithParenthesis()
        {
            this.ApprovalVerifySearchParser(peopleBase, "(mountain OR bike) AND NOT clothing");
        }
    }
}

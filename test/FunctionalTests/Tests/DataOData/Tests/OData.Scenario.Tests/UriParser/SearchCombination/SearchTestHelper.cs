//---------------------------------------------------------------------
// <copyright file="SearchTestHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.OData.UriParser;

namespace Microsoft.Test.Taupo.OData.Scenario.Tests.UriParser.SearchCombination
{
    static class SearchTestHelper
    {
        public static Func<string, bool> GetMatchFunc(this SearchClause search)
        {
            return new SearchVisitor().TranslateNode(search.Expression).Compile();
        }
    }

    enum SearchOperator
    {
        Or = 0,
        And = 1,
        Not = 2,
        None = 3
    }

    class SearchTestCase
    {
        public string RawQuery { get; set; }

        public SearchOperator TopOperator { get; set; }

        public static readonly string[] Words = new string[] { "A", "B", "C", "D", "E" };

        public static string[] BuildLexicon(int num)
        {
            List<string> list = new List<string>  { string.Empty };

            for (int i = 0; i < num; i++)
            {
                list.AddRange(list.Select(item => item + Words[i] /* + "_" */ ).ToList());
            }

            return list.ToArray();
        }

        public string ParenedQuery(SearchOperator searchOperator)
        {
            return this.TopOperator < searchOperator ? "(" + RawQuery + ")" : RawQuery;
        }

        public override string ToString()
        {
            return "[" + RawQuery + "]";
        }
    }

    class SearchVisitor : QueryNodeVisitor<Expression<Func<string, bool>>>
    {
        public override Expression<Func<string, bool>> Visit(BinaryOperatorNode nodeIn)
        {
            var left = this.TranslateNode(nodeIn.Left).Compile();
            var right = this.TranslateNode(nodeIn.Right).Compile();
            switch (nodeIn.OperatorKind)
            {
                case BinaryOperatorKind.And:
                    return _ => left(_) & right(_);

                case BinaryOperatorKind.Or:
                    return _ => left(_) | right(_);
            }

            Debug.Fail("Should not reach here.");
            return base.Visit(nodeIn);
        }

        public override Expression<Func<string, bool>> Visit(UnaryOperatorNode nodeIn)
        {
            Debug.Assert(nodeIn.OperatorKind == UnaryOperatorKind.Not, "nodeIn.OperatorKind == UnaryOperatorKind.Not");
            var eval = this.TranslateNode(nodeIn.Operand).Compile();
            return _ => !eval(_);
        }

        public override Expression<Func<string, bool>> Visit(SearchTermNode nodeIn)
        {
            return _ => _.Contains(nodeIn.Text);
        }

        internal Expression<Func<string, bool>> TranslateNode(QueryNode node)
        {
            Debug.Assert(node != null, "node != null");
            return node.Accept(this);
        }
    }
}

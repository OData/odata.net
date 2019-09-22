//---------------------------------------------------------------------
// <copyright file="SearchFunctionalTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.Tests.UriParser;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.ScenarioTests.UriParser
{
    public class SearchFunctionalTests
    {
        [Fact]
        public void WordTest()
        {
            var result = this.RunSearchTest("bike單車");
            result.Expression.ShouldBeSearchTermNode("bike單車");
        }

        [Fact]
        public void PhraseTest()
        {
            var result = this.RunSearchTest("\"mountain bike\"");
            result.Expression.ShouldBeSearchTermNode("mountain bike");
        }

        [Fact]
        public void AndTest()
        {
            var result = this.RunSearchTest("mountain bike");
            var node1 = result.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.And);
            node1.Left.ShouldBeSearchTermNode("mountain");
            node1.Right.ShouldBeSearchTermNode("bike");
        }

        [Fact]
        public void ImplicitAndTest()
        {
            var result = this.RunSearchTest("mountain NOT bike");
            var node1 = result.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.And);
            node1.Left.ShouldBeSearchTermNode("mountain");
            node1.Right.ShouldBeUnaryOperatorNode(UnaryOperatorKind.Not).Operand.ShouldBeSearchTermNode("bike");
        }

        [Fact]
        public void ErrorProneImplicitAndTest()
        {
            var result = this.RunSearchTest("mountain or bike");
            var node1 = result.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.And);
            var node2 = node1.Left.ShouldBeBinaryOperatorNode(BinaryOperatorKind.And);
            node2.Left.ShouldBeSearchTermNode("mountain");
            node2.Right.ShouldBeSearchTermNode("or");
            node1.Right.ShouldBeSearchTermNode("bike");
        }

        [Fact]
        public void OrTest()
        {
            var result = this.RunSearchTest("mountain OR bike");
            var node1 = result.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Or);
            node1.Left.ShouldBeSearchTermNode("mountain");
            node1.Right.ShouldBeSearchTermNode("bike");
        }

        [Fact]
        public void NotTest()
        {
            var result = this.RunSearchTest("NOT bike");
            var node1 = result.Expression.ShouldBeUnaryOperatorNode(UnaryOperatorKind.Not);
            node1.Operand.ShouldBeSearchTermNode("bike");
        }

        [Fact]
        public void CombinationTest()
        {
            var result = this.RunSearchTest("NOT Tis AND (in OR \"my memory\") \"lock'd\"");
            var node1 = result.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.And);
            var node2 = node1.Left.ShouldBeBinaryOperatorNode(BinaryOperatorKind.And);
            node2.Left.ShouldBeUnaryOperatorNode(UnaryOperatorKind.Not).Operand.ShouldBeSearchTermNode("Tis");
            var node3 = node2.Right.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Or);
            node3.Left.ShouldBeSearchTermNode("in");
            node3.Right.ShouldBeSearchTermNode("my memory");
            node1.Right.ShouldBeSearchTermNode("lock'd");
        }

        [Fact]
        public void ErrorTest()
        {
            Action action = () => this.RunSearchTest("NOT");
            action.Throws<ODataException>(Strings.UriQueryExpressionParser_ExpressionExpected(3, "NOT"));
            action = () => this.RunSearchTest("(");
            action.Throws<ODataException>(Strings.UriQueryExpressionParser_ExpressionExpected(1, "("));
            action = () => this.RunSearchTest("(something");
            action.Throws<ODataException>(Strings.UriQueryExpressionParser_CloseParenOrOperatorExpected(10, "(something"));
            action = () => this.RunSearchTest("AND OR");
            action.Throws<ODataException>(Strings.UriQueryExpressionParser_ExpressionExpected(0, "AND OR"));
            action = () => this.RunSearchTest("kit (");
            action.Throws<ODataException>(Strings.UriQueryExpressionParser_ExpressionExpected(5, "kit ("));
            action = () => this.RunSearchTest("kit ( A");
            action.Throws<ODataException>(Strings.UriQueryExpressionParser_CloseParenOrOperatorExpected(7, "kit ( A"));
            action = () => this.RunSearchTest("kit )");
            action.Throws<ODataException>(Strings.ExpressionLexer_SyntaxError(5, "kit )"));
        }

        [Fact]
        public void LexerErrorTest()
        {
            Action action = () => this.RunSearchTest("\"");
            action.Throws<ODataException>(Strings.ExpressionLexer_UnterminatedStringLiteral(1, "\""));
            action = () => this.RunSearchTest("A \"");
            action.Throws<ODataException>(Strings.ExpressionLexer_UnterminatedStringLiteral(3, "A \""));
            action = () => this.RunSearchTest("A \" BC");
            action.Throws<ODataException>(Strings.ExpressionLexer_UnterminatedStringLiteral(6, "A \" BC"));
            action = () => this.RunSearchTest("\\\"");
            action.Throws<ODataException>(Strings.ExpressionLexer_InvalidCharacter("\\", 0, "\\\""));
            action = () => this.RunSearchTest("\"\\t\"");
            action.Throws<ODataException>(Strings.ExpressionLexer_InvalidEscapeSequence("t", 2, "\"\\t\""));
        }

        private SearchClause RunSearchTest(string search)
        {
            ODataQueryOptionParser queryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel, null, null, new Dictionary<string, string> { { "$search", search } });
            return queryOptionParser.ParseSearch();
        }
    }
}

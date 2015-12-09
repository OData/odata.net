//---------------------------------------------------------------------
// <copyright file="SearchFunctionalTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OData.Core.Tests.UriParser;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Xunit;

namespace Microsoft.OData.Core.Tests.ScenarioTests.UriParser
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
            var node1 = result.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.And).And;
            node1.Left.ShouldBeSearchTermNode("mountain");
            node1.Right.ShouldBeSearchTermNode("bike");
        }

        [Fact]
        public void ImplicitAndTest()
        {
            var result = this.RunSearchTest("mountain NOT bike");
            var node1 = result.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.And).And;
            node1.Left.ShouldBeSearchTermNode("mountain");
            node1.Right.ShouldBeUnaryOperatorNode(UnaryOperatorKind.Not).And.Operand.ShouldBeSearchTermNode("bike");
        }

        [Fact]
        public void ErrorProneImplicitAndTest()
        {
            var result = this.RunSearchTest("mountain or bike");
            var node1 = result.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.And).And;
            var node2 = node1.Left.ShouldBeBinaryOperatorNode(BinaryOperatorKind.And).And;
            node2.Left.ShouldBeSearchTermNode("mountain");
            node2.Right.ShouldBeSearchTermNode("or");
            node1.Right.ShouldBeSearchTermNode("bike");
        }

        [Fact]
        public void OrTest()
        {
            var result = this.RunSearchTest("mountain OR bike");
            var node1 = result.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Or).And;
            node1.Left.ShouldBeSearchTermNode("mountain");
            node1.Right.ShouldBeSearchTermNode("bike");
        }

        [Fact]
        public void NotTest()
        {
            var result = this.RunSearchTest("NOT bike");
            var node1 = result.Expression.ShouldBeUnaryOperatorNode(UnaryOperatorKind.Not).And;
            node1.Operand.ShouldBeSearchTermNode("bike");
        }

        [Fact]
        public void CombinationTest()
        {
            var result = this.RunSearchTest("NOT Tis AND (in OR \"my memory\") \"lock'd\"");
            var node1 = result.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.And).And;
            var node2 = node1.Left.ShouldBeBinaryOperatorNode(BinaryOperatorKind.And).And;
            node2.Left.ShouldBeUnaryOperatorNode(UnaryOperatorKind.Not).And.Operand.ShouldBeSearchTermNode("Tis");
            var node3 = node2.Right.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Or).And;
            node3.Left.ShouldBeSearchTermNode("in");
            node3.Right.ShouldBeSearchTermNode("my memory");
            node1.Right.ShouldBeSearchTermNode("lock'd");
        }

        [Fact]
        public void ErrorTest()
        {
            Action action = () => this.RunSearchTest("NOT");
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriQueryExpressionParser_ExpressionExpected(3, "NOT"));
            action = () => this.RunSearchTest("(");
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriQueryExpressionParser_ExpressionExpected(1, "("));
            action = () => this.RunSearchTest("(something");
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriQueryExpressionParser_CloseParenOrOperatorExpected(10, "(something"));
            action = () => this.RunSearchTest("AND OR");
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriQueryExpressionParser_ExpressionExpected(0, "AND OR"));
            action = () => this.RunSearchTest("kit (");
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriQueryExpressionParser_ExpressionExpected(5, "kit ("));
            action = () => this.RunSearchTest("kit ( A");
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriQueryExpressionParser_CloseParenOrOperatorExpected(7, "kit ( A"));
            action = () => this.RunSearchTest("kit )");
            action.ShouldThrow<ODataException>().WithMessage(Strings.ExpressionLexer_SyntaxError(5, "kit )"));
        }

        [Fact]
        public void LexerErrorTest()
        {
            Action action = () => this.RunSearchTest("\"");
            action.ShouldThrow<ODataException>().WithMessage(Strings.ExpressionLexer_UnterminatedStringLiteral(1, "\""));
            action = () => this.RunSearchTest("A \"");
            action.ShouldThrow<ODataException>().WithMessage(Strings.ExpressionLexer_UnterminatedStringLiteral(3, "A \""));
            action = () => this.RunSearchTest("A \" BC");
            action.ShouldThrow<ODataException>().WithMessage(Strings.ExpressionLexer_UnterminatedStringLiteral(6, "A \" BC"));
            action = () => this.RunSearchTest("\\\"");
            action.ShouldThrow<ODataException>().WithMessage(Strings.ExpressionLexer_InvalidCharacter("\\", 0, "\\\""));
            action = () => this.RunSearchTest("\"\\t\"");
            action.ShouldThrow<ODataException>().WithMessage(Strings.ExpressionLexer_InvalidEscapeSequence("t", 2, "\"\\t\""));
        }

        private SearchClause RunSearchTest(string search)
        {
            ODataQueryOptionParser queryOptionParser = new ODataQueryOptionParser(HardCodedTestModel.TestModel, null, null, new Dictionary<string, string> { { "$search", search } });
            return queryOptionParser.ParseSearch();
        }
    }
}

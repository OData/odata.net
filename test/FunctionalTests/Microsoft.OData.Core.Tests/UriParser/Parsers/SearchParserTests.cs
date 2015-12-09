//---------------------------------------------------------------------
// <copyright file="SearchParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser.Parsers;
using Microsoft.OData.Core.UriParser.Syntactic;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser.Parsers
{
    public class SearchParserTests
    {
        private readonly SearchParser searchParser = new SearchParser(50);

        [Fact]
        public void SearchWordTest()
        {
            QueryToken token = searchParser.ParseSearch("zlexico");
            token.ShouldBeStringLiteralToken("zlexico");
        }

        [Fact]
        public void SearchPhraseTest()
        {
            QueryToken token = searchParser.ParseSearch("\"A AND BC AND DEF\"");
            token.ShouldBeStringLiteralToken("A AND BC AND DEF");
        }

        [Fact]
        public void SearchAndTest()
        {
            QueryToken token = searchParser.ParseSearch("A AND BC AND DEF");
            var binaryToken1 = token.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And).And;
            var binaryToken11 = binaryToken1.Left.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And).And;
            binaryToken11.Left.ShouldBeStringLiteralToken("A");
            binaryToken11.Right.ShouldBeStringLiteralToken("BC");
            binaryToken1.Right.ShouldBeStringLiteralToken("DEF");
        }

        [Fact]
        public void SearchSpaceImpliesAndTest()
        {
            QueryToken token = searchParser.ParseSearch("A BC DEF");
            var binaryToken1 = token.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And).And;
            var binaryToken11 = binaryToken1.Left.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And).And;
            binaryToken11.Left.ShouldBeStringLiteralToken("A");
            binaryToken11.Right.ShouldBeStringLiteralToken("BC");
            binaryToken1.Right.ShouldBeStringLiteralToken("DEF");
        }

        [Fact]
        public void SearchOrTest()
        {
            QueryToken token = searchParser.ParseSearch("foo OR bar");
            var binaryToken = token.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.Or).And;
            binaryToken.Left.ShouldBeStringLiteralToken("foo");
            binaryToken.Right.ShouldBeStringLiteralToken("bar");
        }

        [Fact]
        public void SearchParenthesesTest()
        {
            QueryToken token = searchParser.ParseSearch("(A  OR BC) AND DEF");
            var binaryToken1 = token.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And).And;
            var binaryToken11 = binaryToken1.Left.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.Or).And;
            binaryToken11.Left.ShouldBeStringLiteralToken("A");
            binaryToken11.Right.ShouldBeStringLiteralToken("BC");
            binaryToken1.Right.ShouldBeStringLiteralToken("DEF");
        }

        [Fact]
        public void SearchSpaceInParenthesesImpliesAndTest()
        {
            QueryToken token = searchParser.ParseSearch("(A BC) DEF");
            var binaryToken1 = token.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And).And;
            var binaryToken11 = binaryToken1.Left.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And).And;
            binaryToken11.Left.ShouldBeStringLiteralToken("A");
            binaryToken11.Right.ShouldBeStringLiteralToken("BC");
            binaryToken1.Right.ShouldBeStringLiteralToken("DEF");
        }

        [Fact]
        public void SearchNotTest()
        {
            QueryToken token = searchParser.ParseSearch("NOT foo");
            var unaryToken = token.ShouldBeUnaryOperatorQueryToken(UnaryOperatorKind.Not).And;
            unaryToken.Operand.ShouldBeStringLiteralToken("foo");
        }


        [Fact]
        public void SearchCombinedTest()
        {
            QueryToken token = searchParser.ParseSearch("a AND bc OR def AND NOT (ghij AND klmno AND pqrstu)");
            var binaryToken1 = token.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.Or).And;
            var binaryToken21 = binaryToken1.Left.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And).And;
            var binaryToken22 = binaryToken1.Right.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And).And;
            binaryToken21.Left.ShouldBeStringLiteralToken("a");
            binaryToken21.Right.ShouldBeStringLiteralToken("bc");
            binaryToken22.Left.ShouldBeStringLiteralToken("def");
            var unaryToken222 = binaryToken22.Right.ShouldBeUnaryOperatorQueryToken(UnaryOperatorKind.Not).And;
            var binaryToken222 = unaryToken222.Operand.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And).And;
            var binaryToken2221 = binaryToken222.Left.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And).And;
            binaryToken2221.Left.ShouldBeStringLiteralToken("ghij");
            binaryToken2221.Right.ShouldBeStringLiteralToken("klmno");
            binaryToken222.Right.ShouldBeStringLiteralToken("pqrstu");
        }

        [Fact]
        public void SearchUnMatchedParenthesisTest()
        {
            Action action = ()=>searchParser.ParseSearch("(A BC DEF");
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriQueryExpressionParser_CloseParenOrOperatorExpected(9,"(A BC DEF"));
        }

        [Fact]
        public void SearchOperandMissingTest()
        {
            Action action = () => searchParser.ParseSearch("A AND");
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriQueryExpressionParser_ExpressionExpected(5, "A AND"));
        }

        [Fact]
        public void SearchOperandMissingInParenthesisTest()
        {
            Action action = () => searchParser.ParseSearch("(A AND)");
            action.ShouldThrow<ODataException>().WithMessage(Strings.UriQueryExpressionParser_ExpressionExpected(6, "(A AND)"));
        }

        [Fact]
        public void SearchEmptyPhrase()
        {
            Action action = () => searchParser.ParseSearch("A \"\"");
            action.ShouldThrow<ODataException>().WithMessage(Strings.ExpressionToken_IdentifierExpected(2));
        }
    }
}

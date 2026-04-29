//---------------------------------------------------------------------
// <copyright file="SearchParserTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Core;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Parsers
{
    public class SearchParserTests
    {
        private readonly SearchParser searchParser = new SearchParser(HardCodedTestModel.TestModel, 50);

        [Fact]
        public void SearchWordTest()
        {
            QueryToken token = searchParser.ParseSearch("zlexico");
            token.ShouldBeSearchTermToken("zlexico");
        }

        [Fact]
        public void SearchPhraseTest()
        {
            QueryToken token = searchParser.ParseSearch("\"A AND BC AND DEF\"");
            token.ShouldBeSearchTermToken("A AND BC AND DEF");
        }

        [Fact]
        public void SearchAndTest()
        {
            QueryToken token = searchParser.ParseSearch("A AND BC AND DEF");
            var binaryToken1 = token.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And);
            var binaryToken11 = binaryToken1.Left.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And);
            binaryToken11.Left.ShouldBeSearchTermToken("A");
            binaryToken11.Right.ShouldBeSearchTermToken("BC");
            binaryToken1.Right.ShouldBeSearchTermToken("DEF");
        }

        [Fact]
        public void SearchSpaceImpliesAndTest()
        {
            QueryToken token = searchParser.ParseSearch("A BC DEF");
            var binaryToken1 = token.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And);
            var binaryToken11 = binaryToken1.Left.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And);
            binaryToken11.Left.ShouldBeSearchTermToken("A");
            binaryToken11.Right.ShouldBeSearchTermToken("BC");
            binaryToken1.Right.ShouldBeSearchTermToken("DEF");
        }

        [Fact]
        public void SearchOrTest()
        {
            QueryToken token = searchParser.ParseSearch("foo OR bar");
            var binaryToken = token.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.Or);
            binaryToken.Left.ShouldBeSearchTermToken("foo");
            binaryToken.Right.ShouldBeSearchTermToken("bar");
        }

        [Fact]
        public void SearchParenthesesTest()
        {
            QueryToken token = searchParser.ParseSearch("(A  OR BC) AND DEF");
            var binaryToken1 = token.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And);
            var binaryToken11 = binaryToken1.Left.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.Or);
            binaryToken11.Left.ShouldBeSearchTermToken("A");
            binaryToken11.Right.ShouldBeSearchTermToken("BC");
            binaryToken1.Right.ShouldBeSearchTermToken("DEF");
        }

        [Fact]
        public void SearchSpaceInParenthesesImpliesAndTest()
        {
            QueryToken token = searchParser.ParseSearch("(A BC) DEF");
            var binaryToken1 = token.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And);
            var binaryToken11 = binaryToken1.Left.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And);
            binaryToken11.Left.ShouldBeSearchTermToken("A");
            binaryToken11.Right.ShouldBeSearchTermToken("BC");
            binaryToken1.Right.ShouldBeSearchTermToken("DEF");
        }

        [Fact]
        public void SearchNotTest()
        {
            QueryToken token = searchParser.ParseSearch("NOT foo");
            var unaryToken = token.ShouldBeUnaryOperatorQueryToken(UnaryOperatorKind.Not);
            unaryToken.Operand.ShouldBeSearchTermToken("foo");
        }


        [Fact]
        public void SearchCombinedTest()
        {
            QueryToken token = searchParser.ParseSearch("a AND bc OR def AND NOT (ghij AND klmno AND pqrstu)");
            var binaryToken1 = token.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.Or);
            var binaryToken21 = binaryToken1.Left.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And);
            var binaryToken22 = binaryToken1.Right.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And);
            binaryToken21.Left.ShouldBeSearchTermToken("a");
            binaryToken21.Right.ShouldBeSearchTermToken("bc");
            binaryToken22.Left.ShouldBeSearchTermToken("def");
            var unaryToken222 = binaryToken22.Right.ShouldBeUnaryOperatorQueryToken(UnaryOperatorKind.Not);
            var binaryToken222 = unaryToken222.Operand.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And);
            var binaryToken2221 = binaryToken222.Left.ShouldBeBinaryOperatorQueryToken(BinaryOperatorKind.And);
            binaryToken2221.Left.ShouldBeSearchTermToken("ghij");
            binaryToken2221.Right.ShouldBeSearchTermToken("klmno");
            binaryToken222.Right.ShouldBeSearchTermToken("pqrstu");
        }

        [Fact]
        public void SearchUnMatchedParenthesisTest()
        {
            Action action = ()=>searchParser.ParseSearch("(A BC DEF");
            action.Throws<ODataException>(Error.Format(SRResources.UriQueryExpressionParser_ExpressionTokenExpected, ")", 9,"(A BC DEF"));
        }

        [Fact]
        public void SearchOperandMissingTest()
        {
            Action action = () => searchParser.ParseSearch("A AND");
            action.Throws<ODataException>(Error.Format(SRResources.UriQueryExpressionParser_ExpressionExpected, 5, "A AND"));
        }

        [Fact]
        public void SearchOperandMissingInParenthesisTest()
        {
            Action action = () => searchParser.ParseSearch("(A AND)");
            action.Throws<ODataException>(Error.Format(SRResources.UriQueryExpressionParser_ExpressionExpected, 6, "(A AND)"));
        }

        [Fact]
        public void SearchEmptyPhrase()
        {
            Action action = () => searchParser.ParseSearch("A \"\"");
            action.Throws<ODataException>(Error.Format(SRResources.ExpressionToken_IdentifierExpected, 2));
        }
    }
}

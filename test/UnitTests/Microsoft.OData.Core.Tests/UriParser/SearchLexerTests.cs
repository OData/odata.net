//---------------------------------------------------------------------
// <copyright file="SearchLexerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Core;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser
{
    public class SearchLexerTests
    {
        private readonly IEdmModel model;

        public SearchLexerTests()
        {
            this.model = HardCodedTestModel.TestModel;
        }

        [Fact]
        public void BasicTest()
        {
            ExpressionLexer lexer = new SearchLexer(this.model, "rd");
            ValidateStringLiteralToken(lexer, "rd");
            ValidateTokenKind(lexer, ExpressionTokenKind.End);
        }

        [Fact]
        public void SpaceTest()
        {
            ExpressionLexer lexer = new SearchLexer(this.model, "st    dn rd");
            ValidateStringLiteralToken(lexer, "st");
            ValidateStringLiteralToken(lexer, "dn");
            ValidateStringLiteralToken(lexer, "rd");
            ValidateTokenKind(lexer, ExpressionTokenKind.End);
        }

        [Fact]
        public void ParenTest()
        {
            ExpressionLexer lexer = new SearchLexer(this.model, "st    (dn   rd)");
            ValidateStringLiteralToken(lexer, "st");
            ValidateTokenKind(lexer, ExpressionTokenKind.OpenParen);
            ValidateStringLiteralToken(lexer, "dn");
            ValidateStringLiteralToken(lexer, "rd");
            ValidateTokenKind(lexer, ExpressionTokenKind.CloseParen);
            ValidateTokenKind(lexer, ExpressionTokenKind.End);
        }

        [Fact]
        public void QuoteTest()
        {
            ExpressionLexer lexer = new SearchLexer(this.model, "a \"AND bc OR \" def");
            ValidateStringLiteralToken(lexer, "a");
            ValidateStringLiteralToken(lexer, "AND bc OR ");
            ValidateStringLiteralToken(lexer, "def");
            ValidateTokenKind(lexer, ExpressionTokenKind.End);
        }

        [Fact]
        public void QuoteAndEscapeTest()
        {
            ExpressionLexer lexer = new SearchLexer(this.model, "a \"AND \\\"bc\\\\ OR \" def");
            ValidateStringLiteralToken(lexer, "a");
            ValidateStringLiteralToken(lexer, "AND \"bc\\ OR ");
            ValidateStringLiteralToken(lexer, "def");
            ValidateTokenKind(lexer, ExpressionTokenKind.End);
        }

        [Fact]
        public void ContinuousQuoteTest()
        {
            ExpressionLexer lexer = new SearchLexer(this.model, "a \"AND \\\"bc\\\\ OR \"\"VV\" def");
            ValidateStringLiteralToken(lexer, "a");
            ValidateStringLiteralToken(lexer, "AND \"bc\\ OR ");
            ValidateStringLiteralToken(lexer, "VV");
            ValidateStringLiteralToken(lexer, "def");
            ValidateTokenKind(lexer, ExpressionTokenKind.End);
        }

        [Fact]
        public void TermTest()
        {
            ExpressionLexer lexer = new SearchLexer(this.model, "a AND bc OR  def");
            ValidateStringLiteralToken(lexer, "a");
            ValidateIdentifierToken(lexer, "AND");
            ValidateStringLiteralToken(lexer, "bc");
            ValidateIdentifierToken(lexer, "OR");
            ValidateStringLiteralToken(lexer, "def");
            ValidateTokenKind(lexer, ExpressionTokenKind.End);
        }

        [Fact]
        public void InvalidCharWithOutQuoteTest()
        {
            ExpressionLexer lexer = new SearchLexer(this.model, "\"b\\\"cd a3\"");
            ValidateStringLiteralToken(lexer, "b\"cd a3");
            ValidateTokenKind(lexer, ExpressionTokenKind.End);
            lexer = new SearchLexer(this.model, "\"bcd za3\"");
            ValidateStringLiteralToken(lexer, "bcd za3");
            ValidateTokenKind(lexer, ExpressionTokenKind.End);

            Action action = () => new SearchLexer(this.model, "b\\\"cd a3").NextToken();
            action.Throws<ODataException>(Error.Format(SRResources.ExpressionLexer_InvalidCharacter, "\\", 1, "b\\\"cd a3"));
            action = () => new SearchLexer(this.model, "bcd za3").NextToken();
            action.Throws<ODataException>(Error.Format(SRResources.ExpressionLexer_InvalidCharacter, "3", 6, "bcd za3"));
            action = () => new SearchLexer(this.model, "\" za\"\\").NextToken();
            action.Throws<ODataException>(Error.Format(SRResources.ExpressionLexer_InvalidCharacter, "\\", 5, "\" za\"\\"));
        }

        [Fact]
        public void InvalidEscapeTest()
        {
            Action action = () => new SearchLexer(this.model, "\"t a\\A\"").NextToken();
            action.Throws<ODataException>(Error.Format(SRResources.ExpressionLexer_InvalidEscapeSequence, "A", 5, "\"t a\\A\""));
            action = () => new SearchLexer(this.model, "\"a\\A t\"");
            action.Throws<ODataException>(Error.Format(SRResources.ExpressionLexer_InvalidEscapeSequence, "A", 3, "\"a\\A t\""));
            action = () => new SearchLexer(this.model, "\"\\Aa t\"");
            action.Throws<ODataException>(Error.Format(SRResources.ExpressionLexer_InvalidEscapeSequence, "A", 2, "\"\\Aa t\""));
        }

        [Fact]
        public void EmptyInputTest()
        {
            ExpressionLexer lexer = new SearchLexer(this.model, "");
            ValidateTokenKind(lexer, ExpressionTokenKind.End);
        }

        [Fact]
        public void EmptyPhraseInputTest()
        {
            ExpressionLexer lexer = new SearchLexer(this.model, "A \"\"");
            Action action = () => lexer.NextToken();
            action.Throws<ODataException>(Error.Format(SRResources.ExpressionToken_IdentifierExpected, 2));
            action = () => new SearchLexer(this.model, "\"\" A");
            action.Throws<ODataException>(Error.Format(SRResources.ExpressionToken_IdentifierExpected, 0));
        }

        [Fact]
        public void UnmatchedParenTest()
        {
            ExpressionLexer lexer = new SearchLexer(this.model, "kit )");
            ValidateStringLiteralToken(lexer, "kit");
            ValidateTokenKind(lexer, ExpressionTokenKind.CloseParen);
            ValidateTokenKind(lexer, ExpressionTokenKind.End);
        }

        private static void ValidateStringLiteralToken(ExpressionLexer lexer, string text)
        {
            Assert.Equal(ExpressionTokenKind.StringLiteral, lexer.CurrentToken.Kind);
            Assert.Equal(text, lexer.CurrentToken.Span.ToString());
            lexer.NextToken();
        }

        private static void ValidateIdentifierToken(ExpressionLexer lexer, string text)
        {
            Assert.Equal(ExpressionTokenKind.Identifier, lexer.CurrentToken.Kind);
            Assert.Equal(text, lexer.CurrentToken.Span.ToString());
            lexer.NextToken();
        }

        private static void ValidateTokenKind(ExpressionLexer lexer, ExpressionTokenKind kind)
        {
            Assert.Equal(kind, lexer.CurrentToken.Kind);
            lexer.NextToken();
        }
    }
}

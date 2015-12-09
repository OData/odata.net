//---------------------------------------------------------------------
// <copyright file="SearchLexerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.TreeNodeKinds;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser
{
    public class SearchLexerTests
    {
        [Fact]
        public void BasicTest()
        {
            ExpressionLexer lexer = new SearchLexer("rd");
            ValidateStringLiteralToken(lexer, "rd");
            ValidateTokenKind(lexer, ExpressionTokenKind.End);
        }

        [Fact]
        public void SpaceTest()
        {
            ExpressionLexer lexer = new SearchLexer("st    dn rd");
            ValidateStringLiteralToken(lexer, "st");
            ValidateStringLiteralToken(lexer, "dn");
            ValidateStringLiteralToken(lexer, "rd");
            ValidateTokenKind(lexer, ExpressionTokenKind.End);
        }

        [Fact]
        public void ParenTest()
        {
            ExpressionLexer lexer = new SearchLexer("st    (dn   rd)");
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
            ExpressionLexer lexer = new SearchLexer("a \"AND bc OR \" def");
            ValidateStringLiteralToken(lexer, "a");
            ValidateStringLiteralToken(lexer, "AND bc OR ");
            ValidateStringLiteralToken(lexer, "def");
            ValidateTokenKind(lexer, ExpressionTokenKind.End);
        }

        [Fact]
        public void QuoteAndEscapeTest()
        {
            ExpressionLexer lexer = new SearchLexer("a \"AND \\\"bc\\\\ OR \" def");
            ValidateStringLiteralToken(lexer, "a");
            ValidateStringLiteralToken(lexer, "AND \"bc\\ OR ");
            ValidateStringLiteralToken(lexer, "def");
            ValidateTokenKind(lexer, ExpressionTokenKind.End);
        }

        [Fact]
        public void ContinuousQuoteTest()
        {
            ExpressionLexer lexer = new SearchLexer("a \"AND \\\"bc\\\\ OR \"\"VV\" def");
            ValidateStringLiteralToken(lexer, "a");
            ValidateStringLiteralToken(lexer, "AND \"bc\\ OR ");
            ValidateStringLiteralToken(lexer, "VV");
            ValidateStringLiteralToken(lexer, "def");
            ValidateTokenKind(lexer, ExpressionTokenKind.End);
        }

        [Fact]
        public void TermTest()
        {
            ExpressionLexer lexer = new SearchLexer("a AND bc OR  def");
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
            ExpressionLexer lexer = new SearchLexer("\"b\\\"cd a3\"");
            ValidateStringLiteralToken(lexer, "b\"cd a3");
            ValidateTokenKind(lexer, ExpressionTokenKind.End);
            lexer = new SearchLexer("\"bcd za3\"");
            ValidateStringLiteralToken(lexer, "bcd za3");
            ValidateTokenKind(lexer, ExpressionTokenKind.End);

            Action action = () => new SearchLexer("b\\\"cd a3").NextToken();
            action.ShouldThrow<ODataException>().WithMessage(Strings.ExpressionLexer_InvalidCharacter("\\", 1, "b\\\"cd a3"));
            action = () => new SearchLexer("bcd za3").NextToken();
            action.ShouldThrow<ODataException>().WithMessage(Strings.ExpressionLexer_InvalidCharacter("3", 6, "bcd za3"));
            action = () => new SearchLexer("\" za\"\\").NextToken();
            action.ShouldThrow<ODataException>().WithMessage(Strings.ExpressionLexer_InvalidCharacter("\\", 5, "\" za\"\\"));
        }

        [Fact]
        public void InvalidEscapeTest()
        {
            Action action = () => new SearchLexer("\"t a\\A\"").NextToken();
            action.ShouldThrow<ODataException>().WithMessage(Strings.ExpressionLexer_InvalidEscapeSequence("A", 5, "\"t a\\A\""));
            action = () => new SearchLexer("\"a\\A t\"");
            action.ShouldThrow<ODataException>().WithMessage(Strings.ExpressionLexer_InvalidEscapeSequence("A", 3, "\"a\\A t\""));
            action = () => new SearchLexer("\"\\Aa t\"");
            action.ShouldThrow<ODataException>().WithMessage(Strings.ExpressionLexer_InvalidEscapeSequence("A", 2, "\"\\Aa t\""));
        }

        [Fact]
        public void EmptyInputTest()
        {
            ExpressionLexer lexer = new SearchLexer("");
            ValidateTokenKind(lexer, ExpressionTokenKind.End);
        }

        [Fact]
        public void EmptyPhraseInputTest()
        {
            ExpressionLexer lexer = new SearchLexer("A \"\"");
            Action action = () => lexer.NextToken();
            action.ShouldThrow<ODataException>().WithMessage(Strings.ExpressionToken_IdentifierExpected((2)));
            action = () => new SearchLexer("\"\" A");
            action.ShouldThrow<ODataException>().WithMessage(Strings.ExpressionToken_IdentifierExpected((0)));
        }

        [Fact]
        public void UnmatchedParenTest()
        {
            ExpressionLexer lexer = new SearchLexer("kit )");
            ValidateStringLiteralToken(lexer, "kit");
            ValidateTokenKind(lexer, ExpressionTokenKind.CloseParen);
            ValidateTokenKind(lexer, ExpressionTokenKind.End);
        }

        private static void ValidateStringLiteralToken(ExpressionLexer lexer, string text)
        {
            lexer.CurrentToken.Kind.Should().Be(ExpressionTokenKind.StringLiteral);
            lexer.CurrentToken.Text.Should().Be(text);
            lexer.NextToken();
        }

        private static void ValidateIdentifierToken(ExpressionLexer lexer, string text)
        {
            lexer.CurrentToken.Kind.Should().Be(ExpressionTokenKind.Identifier);
            lexer.CurrentToken.Text.Should().Be(text);
            lexer.NextToken();
        }

        private static void ValidateTokenKind(ExpressionLexer lexer, ExpressionTokenKind kind)
        {
            lexer.CurrentToken.Kind.Should().Be(kind);
            lexer.NextToken();
        }
    }
}

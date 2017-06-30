//---------------------------------------------------------------------
// <copyright file="ExpressionLexerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using FluentAssertions;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.UriParser
{
    public class ExpressionLexerTests
    {
        private static readonly ExpressionToken CommaToken = new ExpressionToken() { Kind = ExpressionTokenKind.Comma, Text = "," };
        private static readonly ExpressionToken OpenParenToken = new ExpressionToken() { Kind = ExpressionTokenKind.OpenParen, Text = "(" };
        private static readonly ExpressionToken CloseParenToken = new ExpressionToken() { Kind = ExpressionTokenKind.CloseParen, Text = ")" };
        private static readonly ExpressionToken EqualsToken = new ExpressionToken() { Kind = ExpressionTokenKind.Equal, Text = "=" };
        private static readonly ExpressionToken SemiColonToken = new ExpressionToken() { Kind = ExpressionTokenKind.SemiColon, Text = ";" };
        private static readonly ExpressionToken MinusToken = new ExpressionToken() { Kind = ExpressionTokenKind.Minus, Text = "-" };
        private static readonly ExpressionToken SlashToken = new ExpressionToken() { Kind = ExpressionTokenKind.Slash, Text = "/" };
        private static readonly ExpressionToken QuestionToken = new ExpressionToken() { Kind = ExpressionTokenKind.Question, Text = "?" };
        private static readonly ExpressionToken DotToken = new ExpressionToken() { Kind = ExpressionTokenKind.Dot, Text = "." };
        private static readonly ExpressionToken StarToken = new ExpressionToken() { Kind = ExpressionTokenKind.Star, Text = "*" };
        private static readonly ExpressionToken ColonToken = new ExpressionToken() { Kind = ExpressionTokenKind.Colon, Text = ":" };
        private static readonly ExpressionToken ItToken = new ExpressionToken() { Kind = ExpressionTokenKind.Identifier, Text = "$it" };
        private static readonly ExpressionToken NullLiteralToken = new ExpressionToken() { Kind = ExpressionTokenKind.NullLiteral, Text = "null" };

        // internal static bool IsNumeric(ExpressionTokenKind id) tests
        [Fact]
        public void ShouldReturnTrueIfNumericToken()
        {
            ExpressionLexerUtils.IsNumeric(ExpressionTokenKind.DecimalLiteral).Should().BeTrue();
            ExpressionLexerUtils.IsNumeric(ExpressionTokenKind.IntegerLiteral).Should().BeTrue();
            ExpressionLexerUtils.IsNumeric(ExpressionTokenKind.DoubleLiteral).Should().BeTrue();
            ExpressionLexerUtils.IsNumeric(ExpressionTokenKind.Int64Literal).Should().BeTrue();
            ExpressionLexerUtils.IsNumeric(ExpressionTokenKind.SingleLiteral).Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnFalseIfNotNumericToken()
        {
            ExpressionLexerUtils.IsNumeric(ExpressionTokenKind.Colon).Should().BeFalse();
            ExpressionLexerUtils.IsNumeric(ExpressionTokenKind.DateTimeLiteral).Should().BeFalse();
            ExpressionLexerUtils.IsNumeric(ExpressionTokenKind.GeographyLiteral).Should().BeFalse();
            ExpressionLexerUtils.IsNumeric(ExpressionTokenKind.Identifier).Should().BeFalse();
            ExpressionLexerUtils.IsNumeric(ExpressionTokenKind.Unknown).Should().BeFalse();
        }

        // internal static Boolean IsLiteralType(ExpressionTokenKind tokenKind)
        [Fact]
        public void ShouldReturnTrueIfLiteralToken()
        {
            ExpressionTokenKind.BooleanLiteral.IsLiteralType().Should().BeTrue();
            ExpressionTokenKind.DateTimeLiteral.IsLiteralType().Should().BeTrue();
            ExpressionTokenKind.DecimalLiteral.IsLiteralType().Should().BeTrue();
            ExpressionTokenKind.DoubleLiteral.IsLiteralType().Should().BeTrue();
            ExpressionTokenKind.GuidLiteral.IsLiteralType().Should().BeTrue();
            ExpressionTokenKind.Int64Literal.IsLiteralType().Should().BeTrue();
            ExpressionTokenKind.IntegerLiteral.IsLiteralType().Should().BeTrue();
            ExpressionTokenKind.NullLiteral.IsLiteralType().Should().BeTrue();
            ExpressionTokenKind.SingleLiteral.IsLiteralType().Should().BeTrue();
            ExpressionTokenKind.StringLiteral.IsLiteralType().Should().BeTrue();
            ExpressionTokenKind.TimeOfDayLiteral.IsLiteralType().Should().BeTrue();
            ExpressionTokenKind.DateLiteral.IsLiteralType().Should().BeTrue();
            ExpressionTokenKind.DateTimeOffsetLiteral.IsLiteralType().Should().BeTrue();
            ExpressionTokenKind.DurationLiteral.IsLiteralType().Should().BeTrue();
            ExpressionTokenKind.GeographyLiteral.IsLiteralType().Should().BeTrue();
            ExpressionTokenKind.GeometryLiteral.IsLiteralType().Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnFalseIfNotLiteralToken()
        {
            ExpressionTokenKind.Colon.IsLiteralType().Should().BeFalse();
            ExpressionTokenKind.Slash.IsLiteralType().Should().BeFalse();
            ExpressionTokenKind.OpenParen.IsLiteralType().Should().BeFalse();
            ExpressionTokenKind.Identifier.IsLiteralType().Should().BeFalse();
            ExpressionTokenKind.Unknown.IsLiteralType().Should().BeFalse();
        }

        [Fact]
        public void CommaTokenIsKeyValueShouldReturnFalse()
        {
            CommaToken.IsKeyValueToken.Should().BeFalse();
        }

        [Fact]
        public void CommaTokenIsFunctionParameterTokenShouldReturnFalse()
        {
            CommaToken.IsFunctionParameterToken.Should().BeFalse();
        }

        // internal static bool IsInfinityOrNaNDouble(string tokenText)
        [Fact]
        public void IsInfinityOrNaNDoubleShouldReturnTrueForINF()
        {
            bool result = ExpressionLexerUtils.IsInfinityOrNaNDouble("INF");
            result.Should().BeTrue();
        }

        [Fact]
        public void IsInfinityOrNaNDoubleShouldReturnTrueForNaN()
        {
            bool result = ExpressionLexerUtils.IsInfinityOrNaNDouble("NaN");
            result.Should().BeTrue();
        }

        [Fact]
        public void IsInfinityOrNaNDoubleShouldReturnFalseForInz()
        {
            bool result = ExpressionLexerUtils.IsInfinityOrNaNDouble("Inz");
            result.Should().BeFalse();
        }

        [Fact]
        public void IsInfinityOrNaNDoubleShouldReturnFalseForNaB()
        {
            bool result = ExpressionLexerUtils.IsInfinityOrNaNDouble("NaB");
            result.Should().BeFalse();
        }

        [Fact]
        public void IsInfinityOrNaNDoubleShouldReturnFalseForBlarg()
        {
            bool result = ExpressionLexerUtils.IsInfinityOrNaNDouble("Blarg");
            result.Should().BeFalse();
        }

        // internal static bool IsInfinityLiteralDouble(string text)
        [Fact]
        public void IsInfinityLiteralDoubleShouldReturnTrueForINF()
        {
            bool result = ExpressionLexerUtils.IsInfinityLiteralDouble("INF");
            result.Should().BeTrue();
        }

        [Fact]
        public void IsInfinityLiteralDoubleShouldReturnTrueForINz()
        {
            bool result = ExpressionLexerUtils.IsInfinityLiteralDouble("INz");
            result.Should().BeFalse();
        }


        // internal static bool IsInfinityOrNanSingle(string tokenText)
        [Fact]
        public void IsInfinityOrNanSingleShouldReturnTrueForINFf()
        {
            bool result = ExpressionLexerUtils.IsInfinityOrNanSingle("INFf");
            result.Should().BeTrue();
        }

        [Fact]
        public void IsInfinityOrNaNSingleShouldReturnTrueForNaNF()
        {
            bool result = ExpressionLexerUtils.IsInfinityOrNanSingle("NaNF");
            result.Should().BeTrue();
        }

        [Fact]
        public void IsInfinityOrNaNSingleShouldReturnFalseForInzf()
        {
            bool result = ExpressionLexerUtils.IsInfinityOrNanSingle("Inzf");
            result.Should().BeFalse();
        }

        [Fact]
        public void IsInfinityOrNaNSingleShouldReturnFalseForNaBf()
        {
            bool result = ExpressionLexerUtils.IsInfinityOrNanSingle("NaBf");
            result.Should().BeFalse();
        }

        [Fact]
        public void IsInfinityOrNaNSingleShouldReturnFalseForBlarg()
        {
            bool result = ExpressionLexerUtils.IsInfinityOrNanSingle("Blarg");
            result.Should().BeFalse();
        }

        // internal static bool IsInfinityLiteralSingle(string text)
        [Fact]
        public void IsInfinityLiteralSingleShouldReturnTrueForINF()
        {
            bool result = ExpressionLexerUtils.IsInfinityLiteralSingle("INFf");
            result.Should().BeTrue();
        }

        [Fact]
        public void IsInfinityLiteralSingleShouldReturnTrueForINz()
        {
            bool result = ExpressionLexerUtils.IsInfinityLiteralSingle("INzf");
            result.Should().BeFalse();
        }

        // internal bool TryPeekNextToken(out ExpressionToken resultToken, out Exception error)
        [Fact]
        public void ShouldOutputTokenAndTrueWhenNoError()
        {
            ExpressionLexer lexer = new ExpressionLexer("null", false, false);
            ExpressionToken resultToken;
            Exception error = null;
            bool result = lexer.TryPeekNextToken(out resultToken, out error);
            lexer.CurrentToken.Should().NotBe(resultToken);
            result.Should().BeTrue();
            resultToken.Kind.Should().Be(ExpressionTokenKind.NullLiteral);
            error.Should().BeNull();
        }

        [Fact]
        public void ShouldErrorWhenIncorrectCharacterAtStart()
        {
            ExpressionLexer lexer = new ExpressionLexer("#$*@#", false, false);
            ExpressionToken resultToken;
            Exception error = null;
            bool result = lexer.TryPeekNextToken(out resultToken, out error);
            result.Should().BeFalse();
            error.Should().NotBeNull();
            error.Message.Should().Be(ODataErrorStrings.ExpressionLexer_InvalidCharacter("#", "0", "#$*@#"));
        }

        // internal ExpressionToken NextToken()
        [Fact]
        public void ShouldOutputNextTokenWhenItExists()
        {
            ExpressionLexer lexer = new ExpressionLexer("5", false, false);

            ExpressionToken result = lexer.NextToken();
            result.Kind.Should().Be(ExpressionTokenKind.IntegerLiteral);
            lexer.CurrentToken.Should().Be(result);
            result.Text.Should().Be("5");
        }

        [Fact]
        public void ShouldThrowWhenIncorrectCharacterAtStart()
        {
            ExpressionLexer lexer = new ExpressionLexer("#$*@#", false, false);
            Action nextToken = () => lexer.NextToken();
            nextToken.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ExpressionLexer_InvalidCharacter("#", "0", "#$*@#"));
        }

        // internal object ReadLiteralToken()
        [Fact]
        public void ShouldReturnLiteralWhenNoSuffixLiteralTokenInt()
        {
            ExpressionLexer lexer = new ExpressionLexer("5", false, false);
            object result = lexer.ReadLiteralToken();
            result.Should().BeOfType<int>().And.Be(5);
        }

        [Fact]
        public void ShouldReturnDateTimeOffSetLiteralWhenNoSuffixDateLiteralToken()
        {
            ExpressionLexer lexer = new ExpressionLexer("2014-09-19T12:13:14+00:00", false, false);
            object result = lexer.ReadLiteralToken();
            result.Should()
                .BeOfType<DateTimeOffset>()
                .And.Be(new DateTimeOffset(2014, 9, 19, 12, 13, 14, new TimeSpan(0, 0, 0)));
        }

        [Fact]
        public void ShouldReturnDateLiteralWhenNoSuffixDateLiteralToken()
        {
            ExpressionLexer lexer = new ExpressionLexer("2014-09-19", false, false);
            object result = lexer.ReadLiteralToken();
            result.Should().BeOfType<Date>().And.Be(new Date(2014, 9, 19));
        }

        [Fact]
        public void ShouldReturnLiteralWhenNoSuffixLiteralTokenTimeOfDay()
        {
            ExpressionLexer lexer = new ExpressionLexer("12:30:03.900", false, false);
            object result = lexer.ReadLiteralToken();
            result.Should().BeOfType<TimeOfDay>().And.Be((new TimeOfDay(12, 30, 3, 900)));
        }

        [Fact]
        public void ShouldReturnLiteralWhenNoSuffixLiteralTokenShortTimeOfDay()
        {
            ExpressionLexer lexer = new ExpressionLexer("12:30:03", false, false);
            object result = lexer.ReadLiteralToken();
            result.Should().BeOfType<TimeOfDay>().And.Be((new TimeOfDay(12, 30, 3, 0)));
        }

        [Fact]
        public void ShouldReturnLiteralWhenNoSuffixLiteralTokenLong()
        {
            ExpressionLexer lexer = new ExpressionLexer(int.MaxValue + "000", false, false);
            object result = lexer.ReadLiteralToken();
            result.Should().BeOfType<long>().And.Be(((long)int.MaxValue) * 1000);
        }

        [Fact]
        public void ShouldReturnLiteralWhenNoSuffixLiteralTokenFloat()
        {
            // significant figures: float is 7, double is 15/16, decimal is 28
            ExpressionLexer lexer = new ExpressionLexer("123.001", false, false);
            object result = lexer.ReadLiteralToken();
            result.Should().BeOfType<float>().And.Be(123.001f);
        }

        [Fact]
        public void ShouldReturnLiteralWhenNoSuffixLiteralTokenDouble()
        {
            // significant figures: float is 7, double is 15/16, decimal is 28
            ExpressionLexer lexer = new ExpressionLexer("1234567.001", false, false);
            object result = lexer.ReadLiteralToken();
            result.Should().BeOfType<double>().And.Be(1234567.001d);
        }

        [Fact]
        public void ShouldReturnLiteralWhenNoSuffixLiteralTokenDecimal()
        {
            ExpressionLexer lexer = new ExpressionLexer("3258.678765765489753678965390", false, false);
            object result = lexer.ReadLiteralToken();
            result.Should().BeOfType<decimal>().And.Be(3258.678765765489753678965390m);
        }

        [Fact]
        public void ShouldThrowWhenNotLiteralToken()
        {
            ExpressionLexer lexer = new ExpressionLexer("potato", false, false);
            Action read = () => lexer.ReadLiteralToken();
            read.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ExpressionLexer_ExpectedLiteralToken("potato"));
        }

        // internal string ReadDottedIdentifier()
        [Fact]
        public void ShouldReturnStringIdentifierWhenGivenIdentifierToken()
        {
            ExpressionLexer lexer = new ExpressionLexer("misomethingk", true, false);
            string result = lexer.ReadDottedIdentifier(false);
            result.Should().Be("misomethingk");
        }

        [Fact]
        public void ShouldReturnStringIdentifierWhenGivenIdentifierTokenContainingDot()
        {
            ExpressionLexer lexer = new ExpressionLexer("m.i.something.k", true, false);
            string result = lexer.ReadDottedIdentifier(false);
            result.Should().Be("m.i.something.k");
        }

        [Fact]
        public void ShouldReturnStringIdentifierWhenGivenIdentifierTokenContainingWhitespace()
        {
            ExpressionLexer lexer = new ExpressionLexer("    m.i.something.k", true, false);
            string result = lexer.ReadDottedIdentifier(false);
            result.Should().Be("m.i.something.k");
        }

        [Fact]
        public void ShouldThrowWhenNotGivenIdentifierToken()
        {
            ExpressionLexer lexer = new ExpressionLexer("2.43", false, false);
            Action read = () => lexer.ReadDottedIdentifier(false);
            read.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ExpressionLexer_SyntaxError("0", "2.43"));
        }

        [Fact]
        public void ShouldNotThrowWhenGivenStarInAcceptStarMode()
        {
            ExpressionLexer lexer = new ExpressionLexer("m.*", true, false);
            string result = lexer.ReadDottedIdentifier(true);
            result.Should().Be("m.*");
        }

        [Fact]
        public void ShouldThrowWhenGivenStarInDontAcceptStarMode()
        {
            ExpressionLexer lexer = new ExpressionLexer("m.*", true, false);
            Action read = () => lexer.ReadDottedIdentifier(false);
            read.ShouldThrow<ODataException>(ODataErrorStrings.ExpressionLexer_SyntaxError("2", "m.*"));
        }

        [Fact]
        public void StarMustBeLastTokenInDottedIdentifier()
        {
            ExpressionLexer lexer = new ExpressionLexer("m.*.blah", true, false);
            Action read = () => lexer.ReadDottedIdentifier(true);
            read.ShouldThrow<ODataException>(ODataErrorStrings.ExpressionLexer_SyntaxError("2", "m.*.blah"));
        }

        // internal ExpressionToken PeekNextToken()
        [Fact]
        public void ShouldOutputTokenWhenNoError()
        {
            ExpressionLexer lexer = new ExpressionLexer("null", false, false);
            ExpressionToken result = lexer.PeekNextToken();
            lexer.CurrentToken.Should().NotBe(result);
            result.Kind.Should().Be(ExpressionTokenKind.NullLiteral);
        }

        [Fact]
        public void PeekingShouldThrowWhenIncorrectCharacterAtStart()
        {
            ExpressionLexer lexer = new ExpressionLexer("#$*@#", false, false);
            Action peek = () => lexer.PeekNextToken();
            peek.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ExpressionLexer_InvalidCharacter("#", "0", "#$*@#"));
        }

        // internal void ValidateToken(ExpressionTokenKind t)
        [Fact]
        public void ShouldNotThrowWhenCurrentTokenIsExpressionKind()
        {
            ExpressionLexer lexer = new ExpressionLexer("null", true, false);
            Action validate = () => lexer.ValidateToken(ExpressionTokenKind.NullLiteral);
            validate.ShouldNotThrow();
        }

        [Fact]
        public void ShouldThrowWhenCurrentTokenIsNotExpressionKind()
        {
            ExpressionLexer lexer = new ExpressionLexer("null", true, false);
            Action validate = () => lexer.ValidateToken(ExpressionTokenKind.Question);
            validate.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ExpressionLexer_SyntaxError(4, "null"));
        }

        [Fact]
        public void SpatialLiteralInBinaryExprTest()
        {
            ValidateTokenSequence("Property eq geography'SRID=1234;POINT(10 20)'",
                IdentifierToken("Property"),
                ExpressionToken.EqualsTo,
                SpatialLiteralToken("geography'SRID=1234;POINT(10 20)'"));

            ValidateTokenSequence("geography'SRID=1234;POINT(10 20)' eq Property",
                SpatialLiteralToken("geography'SRID=1234;POINT(10 20)'"),
                ExpressionToken.EqualsTo,
                IdentifierToken("Property"));
        }

        [Fact]
        public void SpatialGeographyLiteralTests()
        {
            string[] testCases =
            {
                "geography'\0fo\0o\0'",
                "geography'foo'",
                // Quoted string with single quote in it.
                "geography'f''o''o'",
                "geography''",
                "GeOgRapHY'SRID=5; POINT(1 2)'",
            };

            foreach (string s in testCases)
            {
                ValidateTokenSequence(s, SpatialLiteralToken(s));
            }
        }

        [Fact]
        public void SpatialGeometryLiteralTests()
        {
            string[] testCases =
            {
                "geometry'foo'",
                "geometry''",
                "gEomETRy'SRID=5; POINT(1 2)'",
            };

            foreach (string s in testCases)
            {
                ValidateTokenSequence(s, SpatialLiteralToken(s, geography: false));
            }
        }

        [Fact]
        public void SpatialLiteralNegative()
        {
            ValidateTokenSequence("POINT 10 20",
                IdentifierToken("POINT"),
                IntegerToken("10"),
                IntegerToken("20"));
        }

        [Fact]
        public void SpatialLiteralNegative_InvalidSrid()
        {
            // invalid SRID sequence should not be expanded into the spatial token
            ValidateTokenSequence("SRID=1234(POINT(10 20))",
                IdentifierToken("SRID"),
                EqualsToken,
                IntegerToken("1234"),
                OpenParenToken,
                IdentifierToken("POINT"),
                OpenParenToken,
                IntegerToken("10"),
                IntegerToken("20"),
                CloseParenToken,
                CloseParenToken);
        }

        [Fact]
        public void SpatialLiteralNegative_MissingQuotes()
        {
            ValidateTokenSequence("geography",
                IdentifierToken("geography"));

            ValidateTokenSequence("geometry",
                IdentifierToken("geometry"));
        }

        [Fact]
        public void SpatialLiteralNegative_WrongQuotes()
        {
            ValidateLexerException<ODataException>("geography\"foo\"", Strings.ExpressionLexer_InvalidCharacter("\"", 9, "geography\"foo\""));
            ValidateLexerException<ODataException>("geometry\"foo\"", Strings.ExpressionLexer_InvalidCharacter("\"", 8, "geometry\"foo\""));
        }

        [Fact]
        public void SpatialLiteralNegative_UnterminatedQuote()
        {
            ValidateLexerException<ODataException>("geography'foo", Strings.ExpressionLexer_UnterminatedLiteral(13, "geography'foo"));
            ValidateLexerException<ODataException>("geometry'foo", Strings.ExpressionLexer_UnterminatedLiteral(12, "geometry'foo"));
        }

        [Fact]
        public void ExpandIdAsFunctionWithDot()
        {
            ExpressionLexer l = new ExpressionLexer("id1.id2.id3(", moveToFirstToken: true, useSemicolonDelimeter: false);
            Assert.True(l.ExpandIdentifierAsFunction());
            Assert.Equal("id1.id2.id3", l.CurrentToken.Text);
            Assert.Equal(0, l.CurrentToken.Position);
        }

        [Fact]
        public void ExpandIdAsFunction()
        {
            ExpressionLexer l = new ExpressionLexer("id1(", moveToFirstToken: true, useSemicolonDelimeter: false);
            Assert.True(l.ExpandIdentifierAsFunction());
            Assert.Equal("id1", l.CurrentToken.Text);
            Assert.Equal(0, l.CurrentToken.Position);
        }

        [Fact]
        public void ExpandIdAsFunctionFail_DoesNotEndWithId()
        {
            ExpressionLexer l = new ExpressionLexer("id1.(", moveToFirstToken: true, useSemicolonDelimeter: false);
            Assert.False(l.ExpandIdentifierAsFunction());
            Assert.Equal("id1", l.CurrentToken.Text);
            Assert.Equal(0, l.CurrentToken.Position);
        }

        [Fact]
        public void ExpandIdAsFunctionFail_DoesNotEndWithParen()
        {
            ExpressionLexer l = new ExpressionLexer("id1.id2.id3", moveToFirstToken: true, useSemicolonDelimeter: false);
            Assert.False(l.ExpandIdentifierAsFunction());
            Assert.Equal("id1", l.CurrentToken.Text);
            Assert.Equal(0, l.CurrentToken.Position);
        }

        [Fact]
        public void ExpandIdAsFunctionFail_WhitespaceBeforeParen()
        {
            ExpressionLexer l = new ExpressionLexer("id1.id2.id3 (", moveToFirstToken: true, useSemicolonDelimeter: false);
            Assert.False(l.ExpandIdentifierAsFunction());
            Assert.Equal("id1", l.CurrentToken.Text);
            Assert.Equal(0, l.CurrentToken.Position);
        }

        [Fact]
        public void ExpandIdAsFunctionFail_WhitespaceInBetween()
        {
            ExpressionLexer l = new ExpressionLexer("id1.id2 .id3(", moveToFirstToken: true, useSemicolonDelimeter: false);
            Assert.False(l.ExpandIdentifierAsFunction());
            Assert.Equal("id1", l.CurrentToken.Text);
            Assert.Equal(0, l.CurrentToken.Position);
        }

        [Fact]
        public void SpecialCharsTest()
        {
            string identifier = "Pròjè_x00A2_tÎð瑞갂థ్క_x0020_Iiلإَّ";

            ExpressionToken[] specialTokens = new ExpressionToken[]
            {
               CommaToken,
               OpenParenToken,
               CloseParenToken,
               EqualsToken, 
               //SemiColonToken,
               MinusToken,
               SlashToken,
               QuestionToken,
               DotToken,
               StarToken,
               ColonToken,
               ItToken
            };

            foreach (var token in specialTokens)
            {
                ValidateTokenSequence(identifier + token.Text, IdentifierToken(identifier), token);
            }
        }

        /// <summary>
        ///  The following are allowed by EDM:
        ///     For staring char: [\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Lm}\p{Nl}].
        ///     For other chars   [\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Lm}\p{Nl}\p{Mn}\p{Mc}\p{Nd}\p{Pc}\p{Cf}]
        ///     
        /// Note: Letters: \p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Lm} should already be covered.
        /// 
        /// </summary>
        [Fact]
        public void EdmValidNamesNotAllowedInUri_1()
        {
            EdmValidNamesNotAllowedInUri("Pròjè_x00A2_tÎð瑞갂థ్క_x0020_Iiلإَّ");
        }

        [Fact]
        public void EdmValidNamesNotAllowedInUri_2()
        {
            EdmValidNamesNotAllowedInUri("PròⅡ");
        }

        [Fact]
        public void EdmValidNamesNotAllowedInUri_UndersocreAllowedAsStartingChar_Regression()
        {
            EdmValidNamesNotAllowedInUri("_some_name");
        }

#if !NETCOREAPP1_0
        [Fact]
        public void EdmValidNamesNotAllowedInUri_Combinations()
        {
            // For staring char: [\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Lm}\p{Nl}].
            var startingCharSupportedCategories = new UnicodeCategory[] {
                UnicodeCategory.LowercaseLetter,
                UnicodeCategory.UppercaseLetter,
                UnicodeCategory.TitlecaseLetter,
                UnicodeCategory.OtherLetter,
                UnicodeCategory.ModifierLetter,
                UnicodeCategory.LetterNumber
            };

            // For other chars   [\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Lm}\p{Nl}\p{Mn}\p{Mc}\p{Nd}\p{Pc}\p{Cf}]
            var nonStartingCharSupportedCategories = new UnicodeCategory[] {
                UnicodeCategory.LowercaseLetter,
                UnicodeCategory.UppercaseLetter,
                UnicodeCategory.TitlecaseLetter,
                UnicodeCategory.OtherLetter,
                UnicodeCategory.ModifierLetter,
                UnicodeCategory.LetterNumber,
                UnicodeCategory.NonSpacingMark,
                UnicodeCategory.SpacingCombiningMark,
                UnicodeCategory.DecimalDigitNumber,
                UnicodeCategory.ConnectorPunctuation,
                UnicodeCategory.Format
            };

            Dictionary<UnicodeCategory, char> categoryToChar = new Dictionary<UnicodeCategory, char>(nonStartingCharSupportedCategories.Length);
            foreach (var category in nonStartingCharSupportedCategories)
            {
                categoryToChar.Add(category, FindMatchingChar(category));
            }

            foreach (var startingCharCategory in startingCharSupportedCategories)
            {
                StringBuilder propertyNameSB = new StringBuilder();
                propertyNameSB.Append(categoryToChar[startingCharCategory]);
                foreach (var category in nonStartingCharSupportedCategories)
                {
                    propertyNameSB.Append(categoryToChar[category]);
                }
                EdmValidNamesNotAllowedInUri(propertyNameSB.ToString());
            }
        }
#endif

        [Fact]
        public void ExpressionLexerShouldFailByDefaultForAtSymbol()
        {
            Action lex = () => new ExpressionLexer("@", moveToFirstToken: true, useSemicolonDelimeter: false);
            lex.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ExpressionLexer_InvalidCharacter("@", 0, "@"));
        }

        [Fact]
        public void ExpressionLexerShouldFailAtSymbolIsLastCharacter()
        {
            Action lex = () => new ExpressionLexer("@", moveToFirstToken: true, useSemicolonDelimeter: false, parsingFunctionParameters: true);
            lex.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ExpressionLexer_SyntaxError(1, "@"));
        }

        [Fact]
        public void ExpressionLexerShouldExpectIdentifierStartAfterAtSymbol()
        {
            Action lex = () => new ExpressionLexer("@1", moveToFirstToken: true, useSemicolonDelimeter: false, parsingFunctionParameters: true);
            lex.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ExpressionLexer_InvalidCharacter("1", 1, "@1"));
        }

        [Fact]
        public void ExpressionLexerShouldParseValidAliasCorrectly()
        {
            ValidateTokenSequence("@foo", true /*parsingFunctionParameters*/, ParameterAliasToken("@foo"));
        }

        [Fact]
        public void ExpressionLexerShouldGrabEntireIdentifierForAliasUntilANonIdentifierCharacter()
        {
            ValidateTokenSequence(
                "@a?b",
                true /*parsingFunctionParameters*/,
                ParameterAliasToken("@a"),
                QuestionToken,
                IdentifierToken("b"));
        }

        [Fact]
        public void ExpressionLexerShouldParseFunctionCallWithAliases()
        {
            ValidateTokenSequence(
                "Function(foo=@x,bar=1,baz=@y)",
                true /*parsingFunctionParameters*/,
                IdentifierToken("Function"),
                OpenParenToken,
                IdentifierToken("foo"),
                EqualsToken,
                ParameterAliasToken("@x"),
                CommaToken,
                IdentifierToken("bar"),
                EqualsToken,
                IntegerToken("1"),
                CommaToken,
                IdentifierToken("baz"),
                EqualsToken,
                ParameterAliasToken("@y"),
                CloseParenToken);

        }

        [Fact]
        public void BracesIsParsedAsBracketedExpression()
        {
            ValidateTokenSequence("{complex:value}", BracedToken("{complex:value}"));
        }

        [Fact]
        public void BracesWithInnerBracesIsOneToken()
        {
            ValidateTokenSequence("{complex:value, subComplex : {subComplexParameter : subComplexValue}}",
                BracedToken("{complex:value, subComplex : {subComplexParameter : subComplexValue}}"));
        }

        [Fact]
        public void BracesWithInnerBracketsIsParsedAsOneToken()
        {
            ValidateTokenSequence("{complex:value,InnerArray:[1,2,3]}", BracedToken("{complex:value,InnerArray:[1,2,3]}"));
        }

        [Fact]
        public void BracketsIsParsedAsBracketedExpression()
        {
            ValidateTokenSequence("[1,2,3]",
                BracketToken("[1,2,3]"));
        }

        [Fact]
        public void BracketsWithInnerBracketsIsOneToken()
        {
            ValidateTokenSequence("[[1,2],[30,40],[500,600]]",
                BracketToken("[[1,2],[30,40],[500,600]]"));
        }

        [Fact]
        public void BracketsWithInnerBracesIsOneToken()
        {
            ValidateTokenSequence("[{complex:value},{complex:value}]",
                BracketToken("[{complex:value},{complex:value}]"));
        }

        [Fact]
        public void BracketedExpressionsCanHaveCrazyStuffInsideStringLiteral()
        {
            ValidateTokenSequence("{ 'asdf!@#$%^&*()[]{}<>?:\";,./%1%2%3%4%5\t\n\r' }",
                BracedToken("{ 'asdf!@#$%^&*()[]{}<>?:\";,./%1%2%3%4%5\t\n\r' }"));
        }

        [Fact]
        public void FunctionWithComplexParameter()
        {
            ValidateTokenSequence("Function(param={complex : value})",
                IdentifierToken("Function"),
                OpenParenToken,
                IdentifierToken("param"),
                EqualsToken,
                BracedToken("{complex : value}"),
                CloseParenToken);
        }

        [Fact]
        public void FunctionWithCollectionParameter()
        {
            ValidateTokenSequence("Function(param=[1,2,3,4])",
                IdentifierToken("Function"),
                OpenParenToken,
                IdentifierToken("param"),
                EqualsToken,
                BracketToken("[1,2,3,4]"),
                CloseParenToken);
        }

        [Fact]
        public void ComplexValueMustBeEndedByBracket()
        {
            ValidateLexerException<ODataException>("{stuff : morestuff", ODataErrorStrings.ExpressionLexer_UnbalancedBracketExpression);
        }

        [Fact]
        public void OverClosedBracketsThrow()
        {
            ValidateLexerException<ODataException>("{stuff: morestuff}}", ODataErrorStrings.ExpressionLexer_InvalidCharacter("}", "18", "{stuff: morestuff}}"));
        }

        [Fact]
        public void ArbitraryTextCanGoBetweenDoubleQuotesInComplex()
        {
            ValidateTokenSequence("{\'}}}}}}}}}}}}}}}}}}}}}}}}}}}}}\'}",
                false,
                BracedToken("{\'}}}}}}}}}}}}}}}}}}}}}}}}}}}}}\'}"));
        }

        [Fact]
        public void AdvanceThroughExpandOptionStopsAtSemi()
        {
            var lexer = CreateLexerForAdvanceThroughExpandOptionTest("abc;def");
            string result = lexer.AdvanceThroughExpandOption();
            result.Should().Be("abc");
            lexer.Position.Should().Be(3);
            lexer.CurrentToken.Kind.Should().Be(ExpressionTokenKind.SemiColon);
        }

        [Fact]
        public void AdvanceThroughExpandOptionStopsAtCloseParen()
        {
            var lexer = CreateLexerForAdvanceThroughExpandOptionTest("abc)def");
            string result = lexer.AdvanceThroughExpandOption();
            result.Should().Be("abc");
            lexer.Position.Should().Be(3);
            lexer.CurrentToken.Kind.Should().Be(ExpressionTokenKind.CloseParen);
        }

        [Fact]
        public void AdvanceThroughExpandOptionWillReadUntilEnd()
        {
            var lexer = CreateLexerForAdvanceThroughExpandOptionTest("entirestring");
            string result = lexer.AdvanceThroughExpandOption();
            result.Should().Be("entirestring");
            lexer.CurrentToken.Kind.Should().Be(ExpressionTokenKind.End);
        }

        [Fact]
        public void AdvanceThroughExpandOptionWorksWhenDelimiterIsAtEnd()
        {
            var lexer = CreateLexerForAdvanceThroughExpandOptionTest("foo;");
            string result = lexer.AdvanceThroughExpandOption();
            result.Should().Be("foo");
            lexer.CurrentToken.Kind.Should().Be(ExpressionTokenKind.SemiColon);
        }

        [Fact]
        public void AdvanceThroughExpandOptionSkipsBalancedParens()
        {
            var lexer = CreateLexerForAdvanceThroughExpandOptionTest("abc()def;");
            string result = lexer.AdvanceThroughExpandOption();
            result.Should().Be("abc()def");
            lexer.Position.Should().Be(8);
            lexer.CurrentToken.Kind.Should().Be(ExpressionTokenKind.SemiColon);
        }

        [Fact]
        public void AdvanceThroughExpandOptionHandlesNestedParensRightNextToCheckOther()
        {
            var lexer = CreateLexerForAdvanceThroughExpandOptionTest("abc(())def;");
            string result = lexer.AdvanceThroughExpandOption();
            result.Should().Be("abc(())def");
            lexer.Position.Should().Be(10);
            lexer.CurrentToken.Kind.Should().Be(ExpressionTokenKind.SemiColon);
        }

        [Fact]
        public void AdvanceThroughExpandOptionHandlesNestedParensRightNextToFinalClosingParen()
        {
            var lexer = CreateLexerForAdvanceThroughExpandOptionTest("abc(()))");
            string result = lexer.AdvanceThroughExpandOption();
            result.Should().Be("abc(())");
            lexer.Position.Should().Be(7);
            lexer.CurrentToken.Kind.Should().Be(ExpressionTokenKind.CloseParen);
        }

        [Fact]
        public void AdvanceThroughExpandOptionSkipsSemisInParenthesis()
        {
            var lexer = CreateLexerForAdvanceThroughExpandOptionTest("abc(;;;)def;next");
            string result = lexer.AdvanceThroughExpandOption();
            result.Should().Be("abc(;;;)def");
            lexer.Position.Should().Be(11);
            lexer.CurrentToken.Kind.Should().Be(ExpressionTokenKind.SemiColon);
        }

        [Fact]
        public void AdvanceThroughExpandOptionSkipsOverInnerOptionsSuccessfully()
        {
            var lexer = CreateLexerForAdvanceThroughExpandOptionTest("prop(inner(a=b;c=d(e=deep)))");
            string result = lexer.AdvanceThroughExpandOption();
            result.Should().Be("prop(inner(a=b;c=d(e=deep)))");
            lexer.CurrentToken.Kind.Should().Be(ExpressionTokenKind.End);
        }

        [Fact]
        public void AdvanceThroughExpandOptionThrowsIfTooManyOpenParenthesis()
        {
            var lexer = CreateLexerForAdvanceThroughExpandOptionTest("abc(q(w(e)r)");
            Action advance = () => lexer.AdvanceThroughExpandOption();
            advance.ShouldThrow<ODataException>();
        }

        [Fact]
        public void AdvanceUntilRecognizesSkipsStringLiterals()
        {
            var lexer = CreateLexerForAdvanceThroughExpandOptionTest("'str with ; and () in it' eq StringProperty)");
            string result = lexer.AdvanceThroughExpandOption();
            result.Should().Be("'str with ; and () in it' eq StringProperty");
            lexer.Position.Should().Be(43);
            lexer.CurrentToken.Text.Should().StartWith(")");
        }

        // TODO: more unit tests for this method
        [Fact]
        public void AdvanceThroughBalancedParentheticalExpressionWorks()
        {
            ExpressionLexer lexer = new ExpressionLexer("(expression)next", moveToFirstToken: true, useSemicolonDelimeter: true, parsingFunctionParameters: false);
            string result = lexer.AdvanceThroughBalancedParentheticalExpression();
            result.Should().Be("(expression)");
            // TODO: the state of the lexer is weird right now, see note in AdvanceThroughBalancedParentheticalExpression.

            lexer.NextToken().Text.Should().Be("next");
        }

#if !NETCOREAPP1_0
        private char FindMatchingChar(UnicodeCategory category)
        {
            for (int i = 0; i <= 0xffff; i++)
            {
                char ch = (char)i;

                if (Char.GetUnicodeCategory(ch) == category)
                {
                    return ch;
                }
            }
            Assert.True(false, "Should never get here");
            return (char)0;
        }
#endif

        private void EdmValidNamesNotAllowedInUri(string propertyName)
        {
            ValidateTokenSequence(propertyName, IdentifierToken(propertyName));
        }

        private static ExpressionToken IdentifierToken(string id)
        {
            return new ExpressionToken() { Kind = ExpressionTokenKind.Identifier, Text = id };
        }

        private static ExpressionToken IntegerToken(string integer)
        {
            return new ExpressionToken() { Kind = ExpressionTokenKind.IntegerLiteral, Text = integer };
        }

        private static ExpressionToken ParameterAliasToken(string text)
        {
            return new ExpressionToken() { Kind = ExpressionTokenKind.ParameterAlias, Text = text };
        }

        private static ExpressionToken SpatialLiteralToken(string literal, bool geography = true)
        {
            return new ExpressionToken() { Kind = geography ? ExpressionTokenKind.GeographyLiteral : ExpressionTokenKind.GeometryLiteral, Text = literal };
        }

        private static ExpressionToken BracketToken(string text)
        {
            return new ExpressionToken() { Kind = ExpressionTokenKind.BracketedExpression, Text = text };
        }

        private static ExpressionToken BracedToken(string text)
        {
            return new ExpressionToken() { Kind = ExpressionTokenKind.BracedExpression, Text = text };
        }

        private static ExpressionToken StringToken(string text)
        {
            return new ExpressionToken() { Kind = ExpressionTokenKind.StringLiteral, Text = text };
        }

        private static void ValidateLexerException<T>(string expression, string message) where T : Exception
        {
            Action test = () =>
            {
                ExpressionLexer l = new ExpressionLexer(expression, moveToFirstToken: true, useSemicolonDelimeter: false);
                while (l.CurrentToken.Kind != ExpressionTokenKind.End)
                {
                    l.NextToken();
                }
            };

            test.ShouldThrow<T>().WithMessage(message);
        }

        private static void ValidateTokenSequence(string expression, params ExpressionToken[] expectTokens)
        {
            ValidateTokenSequence(expression, false, expectTokens);
        }

        private static void ValidateTokenSequence(string expression, bool parsingFunctionParameters, params ExpressionToken[] expectTokens)
        {
            ExpressionLexer l = new ExpressionLexer(expression, moveToFirstToken: true, useSemicolonDelimeter: false, parsingFunctionParameters: parsingFunctionParameters);
            for (int i = 0; i < expectTokens.Length; ++i)
            {
                Assert.Equal(expectTokens[i].Kind, l.CurrentToken.Kind);
                Assert.Equal(expectTokens[i].Text, l.CurrentToken.Text);
                l.NextToken();
            }

            Assert.Equal(ExpressionTokenKind.End, l.CurrentToken.Kind);
        }

        private static ExpressionLexer CreateLexerForAdvanceThroughExpandOptionTest(string text)
        {
            return new ExpressionLexer(text, moveToFirstToken: false, useSemicolonDelimeter: true, parsingFunctionParameters: false);
        }

    }
}

//---------------------------------------------------------------------
// <copyright file="ExpressionLexerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.UriParser
{
    public class ExpressionLexerTests
    {
        private static readonly ExpressionToken CommaToken = new ExpressionToken() { Kind = ExpressionTokenKind.Comma, Text = ",".AsMemory() };
        private static readonly ExpressionToken OpenParenToken = new ExpressionToken() { Kind = ExpressionTokenKind.OpenParen, Text = "(".AsMemory() };
        private static readonly ExpressionToken CloseParenToken = new ExpressionToken() { Kind = ExpressionTokenKind.CloseParen, Text = ")".AsMemory() };
        private static readonly ExpressionToken EqualsToken = new ExpressionToken() { Kind = ExpressionTokenKind.Equal, Text = "=".AsMemory() };
        private static readonly ExpressionToken SemiColonToken = new ExpressionToken() { Kind = ExpressionTokenKind.SemiColon, Text = ";".AsMemory() };
        private static readonly ExpressionToken MinusToken = new ExpressionToken() { Kind = ExpressionTokenKind.Minus, Text = "-".AsMemory() };
        private static readonly ExpressionToken SlashToken = new ExpressionToken() { Kind = ExpressionTokenKind.Slash, Text = "/".AsMemory() };
        private static readonly ExpressionToken QuestionToken = new ExpressionToken() { Kind = ExpressionTokenKind.Question, Text = "?".AsMemory() };
        private static readonly ExpressionToken DotToken = new ExpressionToken() { Kind = ExpressionTokenKind.Dot, Text = ".".AsMemory() };
        private static readonly ExpressionToken StarToken = new ExpressionToken() { Kind = ExpressionTokenKind.Star, Text = "*".AsMemory() };
        private static readonly ExpressionToken ColonToken = new ExpressionToken() { Kind = ExpressionTokenKind.Colon, Text = ":".AsMemory() };
        private static readonly ExpressionToken ItToken = new ExpressionToken() { Kind = ExpressionTokenKind.Identifier, Text = "$it".AsMemory() };
        private static readonly ExpressionToken NullLiteralToken = new ExpressionToken() { Kind = ExpressionTokenKind.NullLiteral, Text = "null".AsMemory() };

        // internal static bool IsNumeric(ExpressionTokenKind id) tests
        [Fact]
        public void ShouldReturnTrueIfNumericToken()
        {
            Assert.True(ExpressionLexerUtils.IsNumeric(ExpressionTokenKind.DecimalLiteral));
            Assert.True(ExpressionLexerUtils.IsNumeric(ExpressionTokenKind.IntegerLiteral));
            Assert.True(ExpressionLexerUtils.IsNumeric(ExpressionTokenKind.DoubleLiteral));
            Assert.True(ExpressionLexerUtils.IsNumeric(ExpressionTokenKind.Int64Literal));
            Assert.True(ExpressionLexerUtils.IsNumeric(ExpressionTokenKind.SingleLiteral));
        }

        [Fact]
        public void ShouldReturnFalseIfNotNumericToken()
        {
            Assert.False(ExpressionLexerUtils.IsNumeric(ExpressionTokenKind.Colon));
            Assert.False(ExpressionLexerUtils.IsNumeric(ExpressionTokenKind.DateTimeLiteral));
            Assert.False(ExpressionLexerUtils.IsNumeric(ExpressionTokenKind.GeographyLiteral));
            Assert.False(ExpressionLexerUtils.IsNumeric(ExpressionTokenKind.Identifier));
            Assert.False(ExpressionLexerUtils.IsNumeric(ExpressionTokenKind.Unknown));
        }

        // internal static Boolean IsLiteralType(ExpressionTokenKind tokenKind)
        [Fact]
        public void ShouldReturnTrueIfLiteralToken()
        {
            Assert.True(ExpressionTokenKind.BooleanLiteral.IsLiteralType());
            Assert.True(ExpressionTokenKind.DateTimeLiteral.IsLiteralType());
            Assert.True(ExpressionTokenKind.DecimalLiteral.IsLiteralType());
            Assert.True(ExpressionTokenKind.DoubleLiteral.IsLiteralType());
            Assert.True(ExpressionTokenKind.GuidLiteral.IsLiteralType());
            Assert.True(ExpressionTokenKind.Int64Literal.IsLiteralType());
            Assert.True(ExpressionTokenKind.IntegerLiteral.IsLiteralType());
            Assert.True(ExpressionTokenKind.NullLiteral.IsLiteralType());
            Assert.True(ExpressionTokenKind.SingleLiteral.IsLiteralType());
            Assert.True(ExpressionTokenKind.StringLiteral.IsLiteralType());
            Assert.True(ExpressionTokenKind.TimeOfDayLiteral.IsLiteralType());
            Assert.True(ExpressionTokenKind.DateLiteral.IsLiteralType());
            Assert.True(ExpressionTokenKind.DateTimeOffsetLiteral.IsLiteralType());
            Assert.True(ExpressionTokenKind.DurationLiteral.IsLiteralType());
            Assert.True(ExpressionTokenKind.GeographyLiteral.IsLiteralType());
            Assert.True(ExpressionTokenKind.GeometryLiteral.IsLiteralType());
        }

        [Fact]
        public void ShouldReturnFalseIfNotLiteralToken()
        {
            Assert.False(ExpressionTokenKind.Colon.IsLiteralType());
            Assert.False(ExpressionTokenKind.Slash.IsLiteralType());
            Assert.False(ExpressionTokenKind.OpenParen.IsLiteralType());
            Assert.False(ExpressionTokenKind.Identifier.IsLiteralType());
            Assert.False(ExpressionTokenKind.Unknown.IsLiteralType());
        }

        [Fact]
        public void CommaTokenIsKeyValueShouldReturnFalse()
        {
            Assert.False(CommaToken.IsKeyValueToken);
        }

        [Fact]
        public void CommaTokenIsFunctionParameterTokenShouldReturnFalse()
        {
            Assert.False(CommaToken.IsFunctionParameterToken);
        }

        // internal static bool IsInfinityOrNaNDouble(string tokenText)
        [Fact]
        public void IsInfinityOrNaNDoubleShouldReturnTrueForINF()
        {
            bool result = ExpressionLexerUtils.IsInfinityOrNaNDouble("INF");
            Assert.True(result);
        }

        [Fact]
        public void IsInfinityOrNaNDoubleShouldReturnTrueForNaN()
        {
            bool result = ExpressionLexerUtils.IsInfinityOrNaNDouble("NaN");
            Assert.True(result);
        }

        [Fact]
        public void IsInfinityOrNaNDoubleShouldReturnFalseForInz()
        {
            bool result = ExpressionLexerUtils.IsInfinityOrNaNDouble("Inz");
            Assert.False(result);
        }

        [Fact]
        public void IsInfinityOrNaNDoubleShouldReturnFalseForNaB()
        {
            bool result = ExpressionLexerUtils.IsInfinityOrNaNDouble("NaB");
            Assert.False(result);
        }

        [Fact]
        public void IsInfinityOrNaNDoubleShouldReturnFalseForBlarg()
        {
            bool result = ExpressionLexerUtils.IsInfinityOrNaNDouble("Blarg");
            Assert.False(result);
        }

        // internal static bool IsInfinityLiteralDouble(string text)
        [Fact]
        public void IsInfinityLiteralDoubleShouldReturnTrueForINF()
        {
            bool result = ExpressionLexerUtils.IsInfinityLiteralDouble("INF");
            Assert.True(result);
        }

        [Fact]
        public void IsInfinityLiteralDoubleShouldReturnTrueForINz()
        {
            bool result = ExpressionLexerUtils.IsInfinityLiteralDouble("INz");
            Assert.False(result);
        }

        // internal static bool IsInfinityOrNanSingle(string tokenText)
        [Fact]
        public void IsInfinityOrNanSingleShouldReturnTrueForINFf()
        {
            bool result = ExpressionLexerUtils.IsInfinityOrNanSingle("INFf");
            Assert.True(result);
        }

        [Fact]
        public void IsInfinityOrNaNSingleShouldReturnTrueForNaNF()
        {
            bool result = ExpressionLexerUtils.IsInfinityOrNanSingle("NaNF");
            Assert.True(result);
        }

        [Fact]
        public void IsInfinityOrNaNSingleShouldReturnFalseForInzf()
        {
            bool result = ExpressionLexerUtils.IsInfinityOrNanSingle("Inzf");
            Assert.False(result);
        }

        [Fact]
        public void IsInfinityOrNaNSingleShouldReturnFalseForNaBf()
        {
            bool result = ExpressionLexerUtils.IsInfinityOrNanSingle("NaBf");
            Assert.False(result);
        }

        [Fact]
        public void IsInfinityOrNaNSingleShouldReturnFalseForBlarg()
        {
            bool result = ExpressionLexerUtils.IsInfinityOrNanSingle("Blarg");
            Assert.False(result);
        }

        // internal static bool IsInfinityLiteralSingle(string text)
        [Fact]
        public void IsInfinityLiteralSingleShouldReturnTrueForINF()
        {
            bool result = ExpressionLexerUtils.IsInfinityLiteralSingle("INFf");
            Assert.True(result);
        }

        [Fact]
        public void IsInfinityLiteralSingleShouldReturnTrueForINz()
        {
            bool result = ExpressionLexerUtils.IsInfinityLiteralSingle("INzf");
            Assert.False(result);
        }

        // internal bool TryPeekNextToken(out ExpressionToken resultToken, out Exception error)
        [Fact]
        public void ShouldOutputTokenAndTrueWhenNoError()
        {
            ExpressionLexer lexer = new ExpressionLexer("null", false, false);
            ExpressionToken resultToken;
            Exception error = null;
            bool result = lexer.TryPeekNextToken(out resultToken, out error);
            Assert.NotEqual(resultToken, lexer.CurrentToken);
            Assert.True(result);
            Assert.Equal(ExpressionTokenKind.NullLiteral, resultToken.Kind);
            Assert.Null(error);
        }

        [Fact]
        public void ShouldErrorWhenIncorrectCharacterAtStart()
        {
            ExpressionLexer lexer = new ExpressionLexer("#$*@#", false, false);
            ExpressionToken resultToken;
            Exception error = null;
            bool result = lexer.TryPeekNextToken(out resultToken, out error);
            Assert.False(result);
            Assert.NotNull(error);
            Assert.Equal(ODataErrorStrings.ExpressionLexer_InvalidCharacter("#", "0", "#$*@#"), error.Message);
        }

        // internal ExpressionToken NextToken()
        [Fact]
        public void ShouldOutputNextTokenWhenItExists()
        {
            ExpressionLexer lexer = new ExpressionLexer("5", false, false);

            ExpressionToken result = lexer.NextToken();
            Assert.Equal(ExpressionTokenKind.IntegerLiteral, result.Kind);
            Assert.Equal(result, lexer.CurrentToken);
            Assert.Equal("5", result.Span.ToString());
        }

        [Fact]
        public void ShouldThrowWhenIncorrectCharacterAtStart()
        {
            ExpressionLexer lexer = new ExpressionLexer("#$*@#", false, false);
            Action nextToken = () => lexer.NextToken();
            nextToken.Throws<ODataException>(ODataErrorStrings.ExpressionLexer_InvalidCharacter("#", "0", "#$*@#"));
        }

        // internal object ReadLiteralToken()
        [Fact]
        public void ShouldReturnLiteralWhenNoSuffixLiteralTokenInt()
        {
            ExpressionLexer lexer = new ExpressionLexer("5", false, false);
            object result = lexer.ReadLiteralToken();
            int intResult = Assert.IsType<int>(result);
            Assert.Equal(5, intResult);
        }

        [Fact]
        public void ShouldReturnDateTimeOffSetLiteralWhenNoSuffixDateLiteralToken()
        {
            ExpressionLexer lexer = new ExpressionLexer("2014-09-19T12:13:14+00:00", false, false);
            object result = lexer.ReadLiteralToken();
            var dto = Assert.IsType<DateTimeOffset>(result);
            Assert.Equal(new DateTimeOffset(2014, 9, 19, 12, 13, 14, new TimeSpan(0, 0, 0)), dto);
        }

        [Fact]
        public void ShouldReturnDateLiteralWhenNoSuffixDateLiteralToken()
        {
            ExpressionLexer lexer = new ExpressionLexer("2014-09-19", false, false);
            object result = lexer.ReadLiteralToken();
            var date = Assert.IsType<Date>(result);
            Assert.Equal(new Date(2014, 9, 19), date);
        }

        [Fact]
        public void ShouldReturnLiteralWhenNoSuffixLiteralTokenTimeOfDay()
        {
            ExpressionLexer lexer = new ExpressionLexer("12:30:03.900", false, false);
            object result = lexer.ReadLiteralToken();
            var timeOfDay = Assert.IsType<TimeOfDay>(result);
            Assert.Equal(new TimeOfDay(12, 30, 3, 900), timeOfDay);
        }

        [Fact]
        public void ShouldReturnLiteralWhenNoSuffixLiteralTokenShortTimeOfDay()
        {
            ExpressionLexer lexer = new ExpressionLexer("12:30:03", false, false);
            object result = lexer.ReadLiteralToken();
            var timeOfDay = Assert.IsType<TimeOfDay>(result);
            Assert.Equal(new TimeOfDay(12, 30, 3, 0), timeOfDay);
        }

        [Fact]
        public void ShouldReturnLiteralWhenNoSuffixLiteralTokenLong()
        {
            ExpressionLexer lexer = new ExpressionLexer(int.MaxValue + "000", false, false);
            object result = lexer.ReadLiteralToken();
            var longValue = Assert.IsType<long>(result);
            Assert.Equal(((long)int.MaxValue) * 1000, longValue);
        }

        [Fact]
        public void ShouldReturnLiteralWhenNoSuffixLiteralTokenFloat()
        {
            // significant figures: float is 7, double is 15/16, decimal is 28
            ExpressionLexer lexer = new ExpressionLexer("123.001", false, false);
            object result = lexer.ReadLiteralToken();
            var floatValue = Assert.IsType<float>(result);
            Assert.Equal(123.001f, floatValue);
        }

        [Fact]
        public void ShouldReturnLiteralWhenNoSuffixLiteralTokenDouble()
        {
            // significant figures: float is 7, double is 15/16, decimal is 28
            ExpressionLexer lexer = new ExpressionLexer("1234567.001", false, false);
            object result = lexer.ReadLiteralToken();
            var doubleValue = Assert.IsType<double>(result);
            Assert.Equal(1234567.001d, doubleValue);
        }

        [Fact]
        public void ShouldReturnLiteralWhenNoSuffixLiteralTokenDecimal()
        {
            ExpressionLexer lexer = new ExpressionLexer("3258.678765765489753678965390", false, false);
            object result = lexer.ReadLiteralToken();
            var decimalValue = Assert.IsType<decimal>(result);
            Assert.Equal(3258.678765765489753678965390m, decimalValue);
        }

        [Fact]
        public void ShouldThrowWhenNotLiteralToken()
        {
            ExpressionLexer lexer = new ExpressionLexer("potato", false, false);
            Action read = () => lexer.ReadLiteralToken();
            read.Throws<ODataException>(ODataErrorStrings.ExpressionLexer_ExpectedLiteralToken("potato"));
        }

        // internal string ReadDottedIdentifier()
        [Fact]
        public void ShouldReturnStringIdentifierWhenGivenIdentifierToken()
        {
            ExpressionLexer lexer = new ExpressionLexer("misomethingk", true, false);
            string result = lexer.ReadDottedIdentifier(false).ToString();
            Assert.Equal("misomethingk", result);
        }

        [Fact]
        public void ShouldReturnStringIdentifierWhenGivenIdentifierTokenContainingDot()
        {
            ExpressionLexer lexer = new ExpressionLexer("m.i.something.k", true, false);
            string result = lexer.ReadDottedIdentifier(false).ToString();
            Assert.Equal("m.i.something.k", result);
        }

        [Fact]
        public void ShouldReturnStringIdentifierWhenGivenIdentifierTokenContainingWhitespace()
        {
            ExpressionLexer lexer = new ExpressionLexer("    m.i.something.k", true, false);
            string result = lexer.ReadDottedIdentifier(false).ToString();
            Assert.Equal("m.i.something.k", result);
        }

        [Fact]
        public void ShouldThrowWhenNotGivenIdentifierToken()
        {
            ExpressionLexer lexer = new ExpressionLexer("2.43", false, false);
            Action read = () => lexer.ReadDottedIdentifier(false);
            read.Throws<ODataException>(ODataErrorStrings.ExpressionLexer_SyntaxError("0", "2.43"));
        }

        [Fact]
        public void ShouldNotThrowWhenGivenStarInAcceptStarMode()
        {
            ExpressionLexer lexer = new ExpressionLexer("m.*", true, false);
            string result = lexer.ReadDottedIdentifier(true).ToString();
            Assert.Equal("m.*", result);
        }

        [Fact]
        public void ShouldThrowWhenGivenStarInDontAcceptStarMode()
        {
            ExpressionLexer lexer = new ExpressionLexer("m.*", true, false);
            Action read = () => lexer.ReadDottedIdentifier(false);
            read.Throws<ODataException>(ODataErrorStrings.ExpressionLexer_SyntaxError("3", "m.*"));
        }

        [Fact]
        public void StarMustBeLastTokenInDottedIdentifier()
        {
            ExpressionLexer lexer = new ExpressionLexer("m.*.blah", true, false);
            Action read = () => lexer.ReadDottedIdentifier(true);
            read.Throws<ODataException>(ODataErrorStrings.ExpressionLexer_SyntaxError("3", "m.*.blah"));
        }

        // internal ExpressionToken PeekNextToken()
        [Fact]
        public void ShouldOutputTokenWhenNoError()
        {
            ExpressionLexer lexer = new ExpressionLexer("null", false, false);
            ExpressionToken result = lexer.PeekNextToken();
            Assert.NotEqual(result, lexer.CurrentToken);
            Assert.Equal(ExpressionTokenKind.NullLiteral, result.Kind);
        }

        [Fact]
        public void PeekingShouldThrowWhenIncorrectCharacterAtStart()
        {
            ExpressionLexer lexer = new ExpressionLexer("#$*@#", false, false);
            Action peek = () => lexer.PeekNextToken();
            peek.Throws<ODataException>(ODataErrorStrings.ExpressionLexer_InvalidCharacter("#", "0", "#$*@#"));
        }

        // internal void ValidateToken(ExpressionTokenKind t)
        [Fact]
        public void ShouldNotThrowWhenCurrentTokenIsExpressionKind()
        {
            ExpressionLexer lexer = new ExpressionLexer("null", true, false);
            Action validate = () => lexer.ValidateToken(ExpressionTokenKind.NullLiteral);
            validate.DoesNotThrow();
        }

        [Fact]
        public void ShouldThrowWhenCurrentTokenIsNotExpressionKind()
        {
            ExpressionLexer lexer = new ExpressionLexer("null", true, false);
            Action validate = () => lexer.ValidateToken(ExpressionTokenKind.Question);
            validate.Throws<ODataException>(ODataErrorStrings.ExpressionLexer_SyntaxError(4, "null"));
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
            ExpressionLexer l = new ExpressionLexer("id1.id2.id3(", moveToFirstToken: true, useSemicolonDelimiter: false);
            Assert.True(l.ExpandIdentifierAsFunction());
            Assert.Equal("id1.id2.id3", l.CurrentToken.Span.ToString());
            Assert.Equal(0, l.CurrentToken.Position);
        }

        [Fact]
        public void ExpandIdAsFunction()
        {
            ExpressionLexer l = new ExpressionLexer("id1(", moveToFirstToken: true, useSemicolonDelimiter: false);
            Assert.True(l.ExpandIdentifierAsFunction());
            Assert.Equal("id1", l.CurrentToken.Span.ToString());
            Assert.Equal(0, l.CurrentToken.Position);
        }

        [Fact]
        public void ExpandIdAsFunctionFail_DoesNotEndWithId()
        {
            ExpressionLexer l = new ExpressionLexer("id1.(", moveToFirstToken: true, useSemicolonDelimiter: false);
            Assert.False(l.ExpandIdentifierAsFunction());
            Assert.Equal("id1", l.CurrentToken.Span.ToString());
            Assert.Equal(0, l.CurrentToken.Position);
        }

        [Fact]
        public void ExpandIdAsFunctionFail_DoesNotEndWithParen()
        {
            ExpressionLexer l = new ExpressionLexer("id1.id2.id3", moveToFirstToken: true, useSemicolonDelimiter: false);
            Assert.False(l.ExpandIdentifierAsFunction());
            Assert.Equal("id1", l.CurrentToken.Span.ToString());
            Assert.Equal(0, l.CurrentToken.Position);
        }

        [Fact]
        public void ExpandIdAsFunctionFail_WhitespaceBeforeParen()
        {
            ExpressionLexer l = new ExpressionLexer("id1.id2.id3 (", moveToFirstToken: true, useSemicolonDelimiter: false);
            Assert.False(l.ExpandIdentifierAsFunction());
            Assert.Equal("id1", l.CurrentToken.Span.ToString());
            Assert.Equal(0, l.CurrentToken.Position);
        }

        [Fact]
        public void ExpandIdAsFunctionFail_WhitespaceInBetween()
        {
            ExpressionLexer l = new ExpressionLexer("id1.id2 .id3(", moveToFirstToken: true, useSemicolonDelimiter: false);
            Assert.False(l.ExpandIdentifierAsFunction());
            Assert.Equal("id1", l.CurrentToken.Span.ToString());
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

        [Fact]
        public void ExpressionLexerShouldFailByDefaultForAtSymbol()
        {
            Action lex = () => new ExpressionLexer("@", moveToFirstToken: true, useSemicolonDelimiter: false);
            lex.Throws<ODataException>(ODataErrorStrings.ExpressionLexer_SyntaxError(1, "@"));
        }

        [Fact]
        public void ExpressionLexerShouldFailAtSymbolIsLastCharacter()
        {
            Action lex = () => new ExpressionLexer("@", moveToFirstToken: true, useSemicolonDelimiter: false, parsingFunctionParameters: true);
            lex.Throws<ODataException>(ODataErrorStrings.ExpressionLexer_SyntaxError(1, "@"));
        }

        [Fact]
        public void ExpressionLexerShouldExpectIdentifierStartAfterAtSymbol()
        {
            Action lex = () => new ExpressionLexer("@1", moveToFirstToken: true, useSemicolonDelimiter: false, parsingFunctionParameters: true);
            lex.Throws<ODataException>(ODataErrorStrings.ExpressionLexer_InvalidCharacter("1", 1, "@1"));
        }

        [Fact]
        public void ExpressionLexerShouldParseValidAliasCorrectly()
        {
            ValidateTokenSequence("@foo", true /*parsingFunctionParameters*/, ParameterAliasToken("@foo"));
        }

        [Fact]
        public void ExpressionLexerShouldParseValidAliasWithDotInExpressionCorrectly()
        {
            foreach (string expr in new string[]
                {
                    "@foo eq 1.23",
                    "  @foo  eq  1.23  " // with arbitrary paddings.
                }
            )
            {
                ValidateTokenSequence(expr, true /*parsingFunctionParameters*/,
                    ParameterAliasToken("@foo"),
                    IdentifierToken("eq"),
                    SingleLiteralToken("1.23"));
            }
        }

        [Fact]
        public void ExpressionLexerShouldParseValidAnnotationCorrectly()
        {
            string exprAnnotation = "@NS.myAnnotation1";
            ValidateTokenSequence(exprAnnotation, true /*parsingFunctionParameters*/,
                IdentifierToken("@NS.myAnnotation1"));
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
            Assert.Equal("abc", result);
            Assert.Equal(3, lexer.Position);
            Assert.Equal(ExpressionTokenKind.SemiColon, lexer.CurrentToken.Kind);
        }

        [Fact]
        public void AdvanceThroughExpandOptionStopsAtCloseParen()
        {
            var lexer = CreateLexerForAdvanceThroughExpandOptionTest("abc)def");
            string result = lexer.AdvanceThroughExpandOption();
            Assert.Equal("abc", result);
            Assert.Equal(3, lexer.Position);
            Assert.Equal(ExpressionTokenKind.CloseParen, lexer.CurrentToken.Kind);
        }

        [Fact]
        public void AdvanceThroughExpandOptionWillReadUntilEnd()
        {
            var lexer = CreateLexerForAdvanceThroughExpandOptionTest("entirestring");
            string result = lexer.AdvanceThroughExpandOption();
            Assert.Equal("entirestring", result);
            Assert.Equal(ExpressionTokenKind.End, lexer.CurrentToken.Kind);
        }

        [Fact]
        public void AdvanceThroughExpandOptionWorksWhenDelimiterIsAtEnd()
        {
            var lexer = CreateLexerForAdvanceThroughExpandOptionTest("foo;");
            string result = lexer.AdvanceThroughExpandOption();
            Assert.Equal("foo", result);
            Assert.Equal(ExpressionTokenKind.SemiColon, lexer.CurrentToken.Kind);
        }

        [Fact]
        public void AdvanceThroughExpandOptionSkipsBalancedParens()
        {
            var lexer = CreateLexerForAdvanceThroughExpandOptionTest("abc()def;");
            string result = lexer.AdvanceThroughExpandOption();
            Assert.Equal("abc()def", result);
            Assert.Equal(8, lexer.Position);
            Assert.Equal(ExpressionTokenKind.SemiColon, lexer.CurrentToken.Kind);
        }

        [Fact]
        public void AdvanceThroughExpandOptionHandlesNestedParensRightNextToCheckOther()
        {
            var lexer = CreateLexerForAdvanceThroughExpandOptionTest("abc(())def;");
            string result = lexer.AdvanceThroughExpandOption();
            Assert.Equal("abc(())def", result);
            Assert.Equal(10, lexer.Position);
            Assert.Equal(ExpressionTokenKind.SemiColon, lexer.CurrentToken.Kind);
        }

        [Fact]
        public void AdvanceThroughExpandOptionHandlesNestedParensRightNextToFinalClosingParen()
        {
            var lexer = CreateLexerForAdvanceThroughExpandOptionTest("abc(()))");
            string result = lexer.AdvanceThroughExpandOption();
            Assert.Equal("abc(())", result);
            Assert.Equal(7, lexer.Position);
            Assert.Equal(ExpressionTokenKind.CloseParen, lexer.CurrentToken.Kind);
        }

        [Fact]
        public void AdvanceThroughExpandOptionSkipsSemisInParenthesis()
        {
            var lexer = CreateLexerForAdvanceThroughExpandOptionTest("abc(;;;)def;next");
            string result = lexer.AdvanceThroughExpandOption();
            Assert.Equal("abc(;;;)def", result);
            Assert.Equal(11, lexer.Position);
            Assert.Equal(ExpressionTokenKind.SemiColon, lexer.CurrentToken.Kind);
        }

        [Fact]
        public void AdvanceThroughExpandOptionSkipsOverInnerOptionsSuccessfully()
        {
            var lexer = CreateLexerForAdvanceThroughExpandOptionTest("prop(inner(a=b;c=d(e=deep)))");
            string result = lexer.AdvanceThroughExpandOption();
            Assert.Equal("prop(inner(a=b;c=d(e=deep)))", result);
            Assert.Equal(ExpressionTokenKind.End, lexer.CurrentToken.Kind);
        }

        [Fact]
        public void AdvanceThroughExpandOptionThrowsIfTooManyOpenParenthesis()
        {
            var lexer = CreateLexerForAdvanceThroughExpandOptionTest("abc(q(w(e)r)");
            Action advance = () => lexer.AdvanceThroughExpandOption();
            Assert.Throws<ODataException>(advance);
        }

        [Fact]
        public void AdvanceUntilRecognizesSkipsStringLiterals()
        {
            var lexer = CreateLexerForAdvanceThroughExpandOptionTest("'str with ; and () in it' eq StringProperty)");
            string result = lexer.AdvanceThroughExpandOption();
            Assert.Equal("'str with ; and () in it' eq StringProperty", result);
            Assert.Equal(43, lexer.Position);
            Assert.StartsWith(")", lexer.CurrentToken.Span.ToString());
        }

        // TODO: more unit tests for this method
        [Fact]
        public void AdvanceThroughBalancedParentheticalExpressionWorks()
        {
            ExpressionLexer lexer = new ExpressionLexer("(expression)next", moveToFirstToken: true, useSemicolonDelimiter: true, parsingFunctionParameters: false);
            string result = lexer.AdvanceThroughBalancedParentheticalExpression();
            Assert.Equal("(expression)", result);
            // TODO: the state of the lexer is weird right now, see note in AdvanceThroughBalancedParentheticalExpression.

            Assert.Equal("next", lexer.NextToken().Span.ToString());
        }

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

        private void EdmValidNamesNotAllowedInUri(string propertyName)
        {
            ValidateTokenSequence(propertyName, IdentifierToken(propertyName));
        }

        private static ExpressionToken IdentifierToken(string id)
        {
            return new ExpressionToken() { Kind = ExpressionTokenKind.Identifier, Text = id.AsMemory() };
        }

        private static ExpressionToken IntegerToken(string integer)
        {
            return new ExpressionToken() { Kind = ExpressionTokenKind.IntegerLiteral, Text = integer.AsMemory() };
        }

        private static ExpressionToken ParameterAliasToken(string text)
        {
            return new ExpressionToken() { Kind = ExpressionTokenKind.ParameterAlias, Text = text.AsMemory() };
        }

        private static ExpressionToken SingleLiteralToken(string singleString )
        {
            return new ExpressionToken() { Kind = ExpressionTokenKind.SingleLiteral, Text = singleString.AsMemory() };
        }

        private static ExpressionToken SpatialLiteralToken(string literal, bool geography = true)
        {
            return new ExpressionToken() { Kind = geography ? ExpressionTokenKind.GeographyLiteral : ExpressionTokenKind.GeometryLiteral, Text = literal.AsMemory() };
        }

        private static ExpressionToken BracketToken(string text)
        {
            return new ExpressionToken() { Kind = ExpressionTokenKind.BracketedExpression, Text = text.AsMemory() };
        }

        private static ExpressionToken BracedToken(string text)
        {
            return new ExpressionToken() { Kind = ExpressionTokenKind.BracedExpression, Text = text.AsMemory() };
        }

        private static ExpressionToken StringToken(string text)
        {
            return new ExpressionToken() { Kind = ExpressionTokenKind.StringLiteral, Text = text.AsMemory() };
        }

        private static void ValidateLexerException<T>(string expression, string message) where T : Exception
        {
            Action test = () =>
            {
                ExpressionLexer l = new ExpressionLexer(expression, moveToFirstToken: true, useSemicolonDelimiter: false);
                while (l.CurrentToken.Kind != ExpressionTokenKind.End)
                {
                    l.NextToken();
                }
            };

            test.Throws<T>(message);
        }

        private static void ValidateTokenSequence(string expression, params ExpressionToken[] expectTokens)
        {
            ValidateTokenSequence(expression, false, expectTokens);
        }

        private static void ValidateTokenSequence(string expression, bool parsingFunctionParameters, params ExpressionToken[] expectTokens)
        {
            ExpressionLexer l = new ExpressionLexer(expression, moveToFirstToken: true, useSemicolonDelimiter: false, parsingFunctionParameters: parsingFunctionParameters);
            for (int i = 0; i < expectTokens.Length; ++i)
            {
                Assert.Equal(expectTokens[i].Kind, l.CurrentToken.Kind);
                Assert.Equal(expectTokens[i].Text.ToString(), l.CurrentToken.Text.ToString());
                l.NextToken();
            }

            Assert.Equal(ExpressionTokenKind.End, l.CurrentToken.Kind);
        }

        private static ExpressionLexer CreateLexerForAdvanceThroughExpandOptionTest(string text)
        {
            return new ExpressionLexer(text, moveToFirstToken: false, useSemicolonDelimiter: true, parsingFunctionParameters: false);
        }

    }
}

//---------------------------------------------------------------------
// <copyright file="ExpressionLexerTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.Service;
using Microsoft.OData.Service.Parsing;
using System.Globalization;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace AstoriaUnitTests.Tests.Server
{
    [TestClass]
    public class ExpressionLexerTest
    {
        private static readonly ExpressionToken CommaToken = new ExpressionToken() { Kind = ExpressionTokenKind.Comma, Text = "," };
        private static readonly ExpressionToken OpenParenToken = new ExpressionToken() { Kind = ExpressionTokenKind.OpenParen, Text = "(" };
        private static readonly ExpressionToken CloseParenToken = new ExpressionToken() { Kind = ExpressionTokenKind.CloseParen, Text = ")" };
        private static readonly ExpressionToken EqualsToken = new ExpressionToken() { Kind = ExpressionTokenKind.Equal, Text = "=" };
        private static readonly ExpressionToken SemiColonToken = new ExpressionToken() { Kind = ExpressionTokenKind.Semicolon, Text = ";" };
        private static readonly ExpressionToken MinusToken = new ExpressionToken() { Kind = ExpressionTokenKind.Minus, Text = "-" };
        private static readonly ExpressionToken SlashToken = new ExpressionToken() { Kind = ExpressionTokenKind.Slash, Text = "/" };
        private static readonly ExpressionToken QuestionToken = new ExpressionToken() { Kind = ExpressionTokenKind.Question, Text = "?" };
        private static readonly ExpressionToken DotToken = new ExpressionToken() { Kind = ExpressionTokenKind.Dot, Text = "." };
        private static readonly ExpressionToken StarToken = new ExpressionToken() { Kind = ExpressionTokenKind.Star, Text = "*" };
        private static readonly ExpressionToken ColonToken = new ExpressionToken() { Kind = ExpressionTokenKind.Colon, Text = ":" };
        private static readonly ExpressionToken ItToken = new ExpressionToken() { Kind = ExpressionTokenKind.Identifier, Text = "$it" };

        [TestMethod]
        public void SpatialLiteralInBinaryExprTest()
        {
            ValidateTokenSequence("Property eq geography'SRID=1234;POINT(10 20)'",
                this.IdentifierToken("Property"),
                ExpressionToken.EqualsTo,
                this.SpatialLiteralToken("geography'SRID=1234;POINT(10 20)'"));

            ValidateTokenSequence("geography'SRID=1234;POINT(10 20)' eq Property",
                this.SpatialLiteralToken("geography'SRID=1234;POINT(10 20)'"),
                ExpressionToken.EqualsTo,
                this.IdentifierToken("Property"));
        }

        [TestMethod]
        public void SpatialGeographyLiteralTests()
        {
            string[] testCases = 
            { 
                "geography'foo'",
                "geography''",
                "GeOgRapHY'SRID=5; POINT(1 2)'",
            };

            foreach (string s in testCases)
            {
                ValidateTokenSequence(s, this.SpatialLiteralToken(s));
            }
        }

        [TestMethod]
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
                ValidateTokenSequence(s, this.SpatialLiteralToken(s, geography:false));
            }
        }

        [TestMethod]
        public void SpatialLiteralNegative()
        {
            ValidateTokenSequence("POINT 10 20", 
                this.IdentifierToken("POINT"), 
                this.IntegerToken("10"),
                this.IntegerToken("20"));
        }

        [TestMethod]
        public void SpatialLiteralNegative_InvalidSrid()
        {
            // invalid SRID sequence should not be expanded into the spatial token
            ValidateTokenSequence("SRID=1234(POINT(10 20))",
                this.IdentifierToken("SRID"),
                EqualsToken,
                this.IntegerToken("1234"),
                OpenParenToken,
                this.IdentifierToken("POINT"),
                OpenParenToken,
                this.IntegerToken("10"),
                this.IntegerToken("20"),
                CloseParenToken,
                CloseParenToken);
        }

        [TestMethod]
        public void SpatialLiteralNegative_MissingQuotes()
        {
            ValidateTokenSequence("geography",
                this.IdentifierToken("geography"));

            ValidateTokenSequence("geometry",
                this.IdentifierToken("geometry"));
        }

        [TestMethod]
        public void SpatialLiteralNegative_WrongQuotes()
        {
            ValidateLexerException<DataServiceException>("geography\"foo\"", Strings.RequestQueryParser_InvalidCharacter("\""));
            ValidateLexerException<DataServiceException>("geometry\"foo\"", Strings.RequestQueryParser_InvalidCharacter("\""));
        }

        [TestMethod]
        public void SpatialLiteralNegative_UnterminatedQuote()
        {
            ValidateLexerException<DataServiceException>("geography'foo", Strings.RequestQueryParser_UnterminatedLiteral("geography'foo"));
            ValidateLexerException<DataServiceException>("geometry'foo", Strings.RequestQueryParser_UnterminatedLiteral("geometry'foo"));
        }

        [TestMethod]
        public void ExpandIdAsFunctionWithDot()
        {
            ExpressionLexer l = new ExpressionLexer("id1.id2.id3(");
            Assert.IsTrue(l.ExpandIdentifierAsFunction());
            Assert.AreEqual("id1.id2.id3", l.CurrentToken.Text);
            Assert.AreEqual(0, l.CurrentToken.Position);
        }

        [TestMethod]
        public void ExpandIdAsFunction()
        {
            ExpressionLexer l = new ExpressionLexer("id1(");
            Assert.IsTrue(l.ExpandIdentifierAsFunction());
            Assert.AreEqual("id1", l.CurrentToken.Text);
            Assert.AreEqual(0, l.CurrentToken.Position);
        }

        [TestMethod]
        public void ExpandIdAsFunctionFail_DoesNotEndWithId()
        {
            ExpressionLexer l = new ExpressionLexer("id1.(");
            Assert.IsFalse(l.ExpandIdentifierAsFunction());
            Assert.AreEqual("id1", l.CurrentToken.Text);
            Assert.AreEqual(0, l.CurrentToken.Position);
        }

        [TestMethod]
        public void ExpandIdAsFunctionFail_DoesNotEndWithParen()
        {
            ExpressionLexer l = new ExpressionLexer("id1.id2.id3");
            Assert.IsFalse(l.ExpandIdentifierAsFunction());
            Assert.AreEqual("id1", l.CurrentToken.Text);
            Assert.AreEqual(0, l.CurrentToken.Position);
        }

        [TestMethod]
        public void ExpandIdAsFunctionFail_WhitespaceBeforeParen()
        {
            ExpressionLexer l = new ExpressionLexer("id1.id2.id3 (");
            Assert.IsFalse(l.ExpandIdentifierAsFunction());
            Assert.AreEqual("id1", l.CurrentToken.Text);
            Assert.AreEqual(0, l.CurrentToken.Position);
        }

        [TestMethod]
        public void ExpandIdAsFunctionFail_WhitespaceInBetween()
        {
            ExpressionLexer l = new ExpressionLexer("id1.id2 .id3(");
            Assert.IsFalse(l.ExpandIdentifierAsFunction());
            Assert.AreEqual("id1", l.CurrentToken.Text);
            Assert.AreEqual(0, l.CurrentToken.Position);
        }

        [TestMethod]
        public void SpecialCharsTest()
        {
            string identifier = "Pròjè_x00A2_tÎð瑞갂థ్క_x0020_Iiلإَّ";

            ExpressionToken[] specialTokens = new ExpressionToken[]
            {
               CommaToken,
               OpenParenToken,
               CloseParenToken,
               EqualsToken, 
               SemiColonToken,
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
                ValidateTokenSequence(identifier + token.Text, this.IdentifierToken(identifier), token);
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
        [TestMethod]
        public void EdmValidNamesNotAllowedInUri_1()
        {
            EdmValidNamesNotAllowedInUri("Pròjè_x00A2_tÎð瑞갂థ్క_x0020_Iiلإَّ");
        }

        [TestMethod]
        public void EdmValidNamesNotAllowedInUri_2()
        {
            EdmValidNamesNotAllowedInUri("PròⅡ");
        }

        [TestMethod]
        public void EdmValidNamesNotAllowedInUri_UndersocreAllowedAsStartingChar_Regression()
        {
            EdmValidNamesNotAllowedInUri("_some_name");
        }

        [TestMethod]
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

        private char FindMatchingChar(UnicodeCategory cateogry)
        {
            for (int i = 0; i <= 0xffff; i++)
            {
                char ch = (char)i;

                if (Char.GetUnicodeCategory(ch) == cateogry)
                {
                    return ch;
                }
            }
            Assert.Fail("Should never get here");
            return (char)0;
        }

        private void EdmValidNamesNotAllowedInUri(string propertyName)
        {
            ValidateTokenSequence(propertyName, this.IdentifierToken(propertyName));
        }

        private ExpressionToken IdentifierToken(string id)
        {
            return new ExpressionToken() { Kind = ExpressionTokenKind.Identifier, Text = id };
        }

        private ExpressionToken IntegerToken(string integer)
        {
            return new ExpressionToken() { Kind = ExpressionTokenKind.IntegerLiteral, Text = integer };
        }

        private ExpressionToken SpatialLiteralToken(string literal, bool geography = true)
        {
            return new ExpressionToken() { Kind = geography ? ExpressionTokenKind.GeographylLiteral : ExpressionTokenKind.GeometryLiteral, Text = literal };
        }

        private void ValidateLexerException<T>(string expression, string message) where T : Exception
        {
            Action test = () =>
            {
                ExpressionLexer l = new ExpressionLexer(expression);
                while (l.CurrentToken.Kind != ExpressionTokenKind.End)
                {
                    l.NextToken();
                }
            };

            test.ShouldThrow<T>().WithMessage(message);
        }

        private void ValidateTokenSequence(string expression, params ExpressionToken[] expectTokens)
        {
            ExpressionLexer l = new ExpressionLexer(expression);
            for (int i = 0; i < expectTokens.Length; ++i)
            {
                Assert.AreEqual(expectTokens[i].Kind, l.CurrentToken.Kind);
                Assert.AreEqual(expectTokens[i].Text, l.CurrentToken.Text);
                l.NextToken();
            }

            Assert.AreEqual(ExpressionTokenKind.End, l.CurrentToken.Kind);
        }
    }
}

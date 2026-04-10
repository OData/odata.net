//---------------------------------------------------------------------
// <copyright file="UriParserHelperTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser.Parsers
{
    public class UriParserHelperTests
    {
        #region IsCharHexDigit Tests

        [Theory]
        [InlineData('0', true)]
        [InlineData('5', true)]
        [InlineData('9', true)]
        [InlineData('a', true)]
        [InlineData('f', true)]
        [InlineData('A', true)]
        [InlineData('F', true)]
        [InlineData('g', false)]
        [InlineData('G', false)]
        [InlineData('z', false)]
        [InlineData('Z', false)]
        [InlineData('@', false)]
        [InlineData(' ', false)]
        public void IsCharHexDigit_VariousCharacters_ReturnsExpectedResult(char c, bool expected)
        {
            // Act
            bool result = UriParserHelper.IsCharHexDigit(c);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion

        #region TryRemoveLiteralPrefix Tests

        [Fact]
        public void TryRemoveLiteralPrefix_ValidPrefix_ReturnsTrueAndRemovesPrefix()
        {
            // Arrange
            ReadOnlySpan<char> prefix = "binary".AsSpan();
            ReadOnlySpan<char> text = "binary'AABBCC'".AsSpan();

            // Act
            bool result = UriParserHelper.TryRemoveLiteralPrefix(prefix, ref text);

            // Assert
            Assert.True(result);
            Assert.Equal("'AABBCC'", text.ToString());
        }

        [Fact]
        public void TryRemoveLiteralPrefix_CaseInsensitive_ReturnsTrueAndRemovesPrefix()
        {
            // Arrange
            ReadOnlySpan<char> prefix = "binary".AsSpan();
            ReadOnlySpan<char> text = "BINARY'AABBCC'".AsSpan();

            // Act
            bool result = UriParserHelper.TryRemoveLiteralPrefix(prefix, ref text);

            // Assert
            Assert.True(result);
            Assert.Equal("'AABBCC'", text.ToString());
        }

        [Fact]
        public void TryRemoveLiteralPrefix_InvalidPrefix_ReturnsFalse()
        {
            // Arrange
            ReadOnlySpan<char> prefix = "binary".AsSpan();
            ReadOnlySpan<char> text = "duration'P1D'".AsSpan();
            ReadOnlySpan<char> originalText = text;

            // Act
            bool result = UriParserHelper.TryRemoveLiteralPrefix(prefix, ref text);

            // Assert
            Assert.False(result);
            Assert.Equal(originalText.ToString(), text.ToString());
        }

        [Fact]
        public void TryRemoveLiteralPrefix_EmptyText_ReturnsFalse()
        {
            // Arrange
            ReadOnlySpan<char> prefix = "binary".AsSpan();
            ReadOnlySpan<char> text = "bin".AsSpan();
            ReadOnlySpan<char> originalText = text;

            // Act
            bool result = UriParserHelper.TryRemoveLiteralPrefix(prefix, ref text);

            // Assert
            Assert.False(result);
            Assert.Equal(originalText.ToString(), text.ToString());
        }

        #endregion

        #region TryRemoveSingleQuotes Tests

        [Fact]
        public void TryRemoveSingleQuotes_ValidSingleQuotedString_ReturnsTrueAndRemovesQuotes()
        {
            // Arrange
            ReadOnlySpan<char> text = "'Hello World'".AsSpan();

            // Act
            bool result = UriParserHelper.TryRemoveSingleQuotes(ref text);

            // Assert
            Assert.True(result);
            Assert.Equal("Hello World", text.ToString());
        }

        [Fact]
        public void TryRemoveSingleQuotes_WithEscapedQuotes_ReturnsTrueAndUnescapes()
        {
            // Arrange
            ReadOnlySpan<char> text = "'Hello''World'".AsSpan();

            // Act
            bool result = UriParserHelper.TryRemoveSingleQuotes(ref text, out string value);

            // Assert
            Assert.True(result);
            Assert.Equal("Hello'World", value);
            Assert.Equal("Hello'World", text.ToString());
        }

        [Fact]
        public void TryRemoveSingleQuotes_WithMultipleEscapedQuotes_ReturnsTrueAndUnescapes()
        {
            // Arrange
            ReadOnlySpan<char> text = "'It''s a ''test'''".AsSpan();

            // Act
            bool result = UriParserHelper.TryRemoveSingleQuotes(ref text, out string value);

            // Assert
            Assert.True(result);
            Assert.Equal("It's a 'test'", value);
        }

        [Fact]
        public void TryRemoveSingleQuotes_NoEscapedQuotes_ReturnsTrueAndValueIsNull()
        {
            // Arrange
            ReadOnlySpan<char> text = "'Simple'".AsSpan();

            // Act
            bool result = UriParserHelper.TryRemoveSingleQuotes(ref text, out string value);

            // Assert
            Assert.True(result);
            Assert.Null(value);
            Assert.Equal("Simple", text.ToString());
        }

        [Fact]
        public void TryRemoveSingleQuotes_MissingClosingQuote_ReturnsFalse()
        {
            // Arrange
            ReadOnlySpan<char> text = "'Hello".AsSpan();

            // Act
            bool result = UriParserHelper.TryRemoveSingleQuotes(ref text);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TryRemoveSingleQuotes_MissingOpeningQuote_ReturnsFalse()
        {
            // Arrange
            ReadOnlySpan<char> text = "Hello'".AsSpan();

            // Act
            bool result = UriParserHelper.TryRemoveSingleQuotes(ref text);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TryRemoveSingleQuotes_SingleCharacter_ReturnsFalse()
        {
            // Arrange
            ReadOnlySpan<char> text = "a".AsSpan();

            // Act
            bool result = UriParserHelper.TryRemoveSingleQuotes(ref text);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TryRemoveSingleQuotes_InvalidEscapedQuote_ReturnsFalse()
        {
            // Arrange
            ReadOnlySpan<char> text = "'Hello'World'".AsSpan();

            // Act
            bool result = UriParserHelper.TryRemoveSingleQuotes(ref text);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TryRemoveSingleQuotes_EmptyQuotes_ReturnsTrue()
        {
            // Arrange
            ReadOnlySpan<char> text = "''".AsSpan();

            // Act
            bool result = UriParserHelper.TryRemoveSingleQuotes(ref text);

            // Assert
            Assert.True(result);
            Assert.Equal("", text.ToString());
        }

        #endregion

        #region TryRemoveLiteralSuffix Tests

        [Fact]
        public void TryRemoveLiteralSuffix_ValidSuffix_ReturnsTrueAndRemovesSuffix()
        {
            // Arrange
            ReadOnlySpan<char> suffix = "L".AsSpan();
            ReadOnlySpan<char> text = "123L".AsSpan();

            // Act
            bool result = UriParserHelper.TryRemoveLiteralSuffix(suffix, ref text);

            // Assert
            Assert.True(result);
            Assert.Equal("123", text.ToString());
        }

        [Fact]
        public void TryRemoveLiteralSuffix_CaseInsensitive_ReturnsTrueAndRemovesSuffix()
        {
            // Arrange
            ReadOnlySpan<char> suffix = "L".AsSpan();
            ReadOnlySpan<char> text = "123l".AsSpan();

            // Act
            bool result = UriParserHelper.TryRemoveLiteralSuffix(suffix, ref text);

            // Assert
            Assert.True(result);
            Assert.Equal("123", text.ToString());
        }

        [Fact]
        public void TryRemoveLiteralSuffix_WithWhitespace_TrimsAndRemovesSuffix()
        {
            // Arrange
            ReadOnlySpan<char> suffix = "f".AsSpan();
            ReadOnlySpan<char> text = "  3.14f  ".AsSpan();

            // Act
            bool result = UriParserHelper.TryRemoveLiteralSuffix(suffix, ref text);

            // Assert
            Assert.True(result);
            Assert.Equal("3.14", text.ToString());
        }

        [Fact]
        public void TryRemoveLiteralSuffix_InvalidSuffix_ReturnsFalse()
        {
            // Arrange
            ReadOnlySpan<char> suffix = "L".AsSpan();
            ReadOnlySpan<char> text = "123M".AsSpan();

            // Act
            bool result = UriParserHelper.TryRemoveLiteralSuffix(suffix, ref text);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TryRemoveLiteralSuffix_NumericConstantINF_ReturnsFalse()
        {
            // Arrange
            ReadOnlySpan<char> suffix = "f".AsSpan();
            ReadOnlySpan<char> text = "INF".AsSpan();

            // Act
            bool result = UriParserHelper.TryRemoveLiteralSuffix(suffix, ref text);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TryRemoveLiteralSuffix_NumericConstantNaN_ReturnsFalse()
        {
            // Arrange
            ReadOnlySpan<char> suffix = "f".AsSpan();
            ReadOnlySpan<char> text = "NaN".AsSpan();

            // Act
            bool result = UriParserHelper.TryRemoveLiteralSuffix(suffix, ref text);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TryRemoveLiteralSuffix_NumericConstantNegativeINF_ReturnsFalse()
        {
            // Arrange
            ReadOnlySpan<char> suffix = "f".AsSpan();
            ReadOnlySpan<char> text = "-INF".AsSpan();

            // Act
            bool result = UriParserHelper.TryRemoveLiteralSuffix(suffix, ref text);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TryRemoveLiteralSuffix_TextTooShort_ReturnsFalse()
        {
            // Arrange
            ReadOnlySpan<char> suffix = "L".AsSpan();
            ReadOnlySpan<char> text = "L".AsSpan();

            // Act
            bool result = UriParserHelper.TryRemoveLiteralSuffix(suffix, ref text);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region IsUriValueSingleQuoted Tests

        [Fact]
        public void IsUriValueSingleQuoted_ValidSingleQuotedValue_ReturnsTrue()
        {
            // Arrange
            ReadOnlySpan<char> text = "'Hello'".AsSpan();

            // Act
            bool result = UriParserHelper.IsUriValueSingleQuoted(text);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsUriValueSingleQuoted_WithEscapedQuotes_ReturnsTrue()
        {
            // Arrange
            ReadOnlySpan<char> text = "'It''s valid'".AsSpan();

            // Act
            bool result = UriParserHelper.IsUriValueSingleQuoted(text);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsUriValueSingleQuoted_MissingClosingQuote_ReturnsFalse()
        {
            // Arrange
            ReadOnlySpan<char> text = "'Hello".AsSpan();

            // Act
            bool result = UriParserHelper.IsUriValueSingleQuoted(text);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsUriValueSingleQuoted_MissingOpeningQuote_ReturnsFalse()
        {
            // Arrange
            ReadOnlySpan<char> text = "Hello'".AsSpan();

            // Act
            bool result = UriParserHelper.IsUriValueSingleQuoted(text);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsUriValueSingleQuoted_InvalidEscapedQuote_ReturnsFalse()
        {
            // Arrange
            ReadOnlySpan<char> text = "'Hello'World'".AsSpan();

            // Act
            bool result = UriParserHelper.IsUriValueSingleQuoted(text);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsUriValueSingleQuoted_SingleCharacter_ReturnsFalse()
        {
            // Arrange
            ReadOnlySpan<char> text = "a".AsSpan();

            // Act
            bool result = UriParserHelper.IsUriValueSingleQuoted(text);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsUriValueSingleQuoted_EmptyQuotes_ReturnsTrue()
        {
            // Arrange
            ReadOnlySpan<char> text = "''".AsSpan();

            // Act
            bool result = UriParserHelper.IsUriValueSingleQuoted(text);

            // Assert
            Assert.True(result);
        }

        #endregion

        #region IsValidNumericConstant Tests

        [Theory]
        [InlineData("INF", true)]
        [InlineData("inf", true)]
        [InlineData("Inf", true)]
        [InlineData("-INF", true)]
        [InlineData("-inf", true)]
        [InlineData("-Inf", true)]
        [InlineData("NaN", true)]
        [InlineData("nan", true)]
        [InlineData("NAN", true)]
        [InlineData("123", false)]
        [InlineData("123.45", false)]
        [InlineData("INFINITY", false)]
        [InlineData("-INFINITY", false)]
        [InlineData("", false)]
        public void IsValidNumericConstant_VariousInputs_ReturnsExpectedResult(string input, bool expected)
        {
            // Arrange
            ReadOnlySpan<char> text = input.AsSpan();

            // Act
            bool result = UriParserHelper.IsValidNumericConstant(text);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion

        #region IsAnnotation Tests

        [Theory]
        [InlineData("@odata.count", true)]
        [InlineData("@custom.annotation", true)]
        [InlineData("@my.namespace.term", true)]
        [InlineData("odata.count", false)]         // Missing @ prefix
        [InlineData("@count", false)]              // Missing dot
        [InlineData("@", false)]                   // Only @ symbol
        [InlineData("", false)]                    // Empty string
        [InlineData("normal", false)]              // Normal identifier
        public void IsAnnotation_VariousIdentifiers_ReturnsExpectedResult(string identifier, bool expected)
        {
            // Arrange
            ReadOnlySpan<char> text = identifier.AsSpan();

            // Act
            bool result = UriParserHelper.IsAnnotation(text);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion

        #region ValidatePrefixLiteral Tests

        [Theory]
        [InlineData("binary")]
        [InlineData("duration")]
        [InlineData("Geography")]
        [InlineData("Edm.String")]
        [InlineData("My.Custom.Type")]
        public void ValidatePrefixLiteral_ValidPrefixes_DoesNotThrow(string prefix)
        {
            // Act & Assert
            var exception = Record.Exception(() => UriParserHelper.ValidatePrefixLiteral(prefix));
            Assert.Null(exception);
        }

        [Theory]
        [InlineData("binary123")]
        [InlineData("123binary")]
        [InlineData("bi-nary")]
        [InlineData("binary_value")]
        [InlineData("binary!")]
        [InlineData("binary ")]
        public void ValidatePrefixLiteral_InvalidPrefixes_ThrowsArgumentException(string prefix)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => UriParserHelper.ValidatePrefixLiteral(prefix));
        }

        #endregion

        #region TryRemovePrefix Tests (alias for TryRemoveLiteralPrefix)

        [Fact]
        public void TryRemovePrefix_ValidPrefix_ReturnsTrueAndRemovesPrefix()
        {
            // Arrange
            ReadOnlySpan<char> prefix = "duration".AsSpan();
            ReadOnlySpan<char> text = "duration'P1D'".AsSpan();

            // Act
            bool result = UriParserHelper.TryRemovePrefix(prefix, ref text);

            // Assert
            Assert.True(result);
            Assert.Equal("'P1D'", text.ToString());
        }

        [Fact]
        public void TryRemovePrefix_InvalidPrefix_ReturnsFalse()
        {
            // Arrange
            ReadOnlySpan<char> prefix = "duration".AsSpan();
            ReadOnlySpan<char> text = "binary'AABB'".AsSpan();
            ReadOnlySpan<char> originalText = text;

            // Act
            bool result = UriParserHelper.TryRemovePrefix(prefix, ref text);

            // Assert
            Assert.False(result);
            Assert.Equal(originalText.ToString(), text.ToString());
        }

        #endregion

        #region Edge Cases and Complex Scenarios

        [Fact]
        public void TryRemoveSingleQuotes_ConsecutiveEscapedQuotesAtStart_ReturnsFalse()
        {
            // Arrange
            // This is invalid - after removing outer quotes we have ''test which has an unescaped quote at position 1
            ReadOnlySpan<char> text = "''''test'".AsSpan();

            // Act
            bool result = UriParserHelper.TryRemoveSingleQuotes(ref text, out string value);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TryRemoveSingleQuotes_ConsecutiveEscapedQuotesAtEnd_ReturnsTrue()
        {
            // Arrange
            ReadOnlySpan<char> text = "'test'''".AsSpan();

            // Act
            bool result = UriParserHelper.TryRemoveSingleQuotes(ref text, out string value);

            // Assert
            Assert.True(result);
            Assert.Equal("test'", value);
        }

        [Fact]
        public void TryRemoveSingleQuotes_OnlyEscapedQuotes_ReturnsTrue()
        {
            // Arrange
            ReadOnlySpan<char> text = "''''''".AsSpan();

            // Act
            bool result = UriParserHelper.TryRemoveSingleQuotes(ref text, out string value);

            // Assert
            Assert.True(result);
            Assert.Equal("''", value);
        }

        [Fact]
        public void TryRemoveLiteralSuffix_MultiCharacterSuffix_Works()
        {
            // Arrange
            ReadOnlySpan<char> suffix = "ABC".AsSpan();
            ReadOnlySpan<char> text = "testABC".AsSpan();

            // Act
            bool result = UriParserHelper.TryRemoveLiteralSuffix(suffix, ref text);

            // Assert
            Assert.True(result);
            Assert.Equal("test", text.ToString());
        }

        #endregion
    }
}

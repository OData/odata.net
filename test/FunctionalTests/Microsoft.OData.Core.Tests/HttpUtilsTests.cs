//---------------------------------------------------------------------
// <copyright file="HttpUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class HttpUtilsTests
    {
        [Theory]
        [InlineData("token")]
        [InlineData("token**")]
        [InlineData("toke$")]
        public void ReadTokenOrQuotedStringValueShouldReadTokenValue(string headerValue)
        {
            int textIndex = 0;
            bool isQuotedString;
            ReadOnlyMemory<char> token = HttpUtils.ReadTokenOrQuotedStringValue("any", headerValue, ref textIndex, out isQuotedString, null);

            Assert.False(isQuotedString);
            Assert.Equal(headerValue.Length, textIndex);
            Assert.True(token.Span.Equals(headerValue, StringComparison.Ordinal));
        }

        [Theory]
        [InlineData("token(Next")]
        [InlineData("token)Next")]
        [InlineData("token>Next")]
        [InlineData("token<Next")]
        [InlineData("token@Next")]
        [InlineData("token,Next")]
        [InlineData("token;Next")]
        [InlineData("token:Next")]
        [InlineData("token[Next")]
        [InlineData("token]Next")]
        [InlineData("token/Next")]
        [InlineData("token?Next")]
        [InlineData("token=Next")]
        public void ReadTokenOrQuotedStringValueShouldReadTokenValueBeforeSeparator(string headerValue)
        {
            int textIndex = 0;
            bool isQuotedString;
            ReadOnlyMemory<char> token = HttpUtils.ReadTokenOrQuotedStringValue("any", headerValue, ref textIndex, out isQuotedString, null);

            Assert.False(isQuotedString);
            Assert.Equal(5, textIndex);
            Assert.True(token.Span.Equals("token", StringComparison.Ordinal));
        }

        [Theory]
        [InlineData("token\\Next", '\\')]
        [InlineData("token\"Next", '\"')]
        public void ReadTokenOrQuotedStringValueShouldThrowForEscapeCharInUnquotedString(string headerValue, char ch)
        {
            string headerName = "any";
            int textIndex = 0;
            bool isQuotedString;
            Action test = () => HttpUtils.ReadTokenOrQuotedStringValue(headerName, headerValue, ref textIndex, out isQuotedString, (s) => new Exception(s));

            Exception exception = Assert.Throws<Exception>(test);
            Assert.Equal(Strings.HttpUtils_EscapeCharWithoutQuotes(headerName, headerValue, 5, ch), exception.Message);
        }

        [Theory]
        [InlineData("\"token\"")]
        [InlineData("\"token**\"")]
        [InlineData("\"toke$\"")]
        public void ReadTokenOrQuotedStringValueShouldReadQuotedStringWithoutEscaped(string headerValue)
        {
            int textIndex = 0;
            bool isQuotedString;
            ReadOnlyMemory<char> token = HttpUtils.ReadTokenOrQuotedStringValue("any", headerValue, ref textIndex, out isQuotedString, null);

            Assert.True(isQuotedString);
            Assert.Equal(headerValue.Length, textIndex);
            Assert.True(token.Span.Equals(headerValue.Substring(1, headerValue.Length - 2), StringComparison.Ordinal));
        }

        [Theory]
        [InlineData("\"\\a_token\"", "a_token")]
        [InlineData("\"token_\\b\"", "token_b")]
        [InlineData("\"tok\\den\"", "tokden")]
        [InlineData("\"to\\t\\\\ken\"", "tot\\ken")]
        public void ReadTokenOrQuotedStringValueShouldReadQuotedStringWithEscaped(string headerValue, string expect)
        {
            int textIndex = 0;
            bool isQuotedString;
            ReadOnlyMemory<char> token = HttpUtils.ReadTokenOrQuotedStringValue("any", headerValue, ref textIndex, out isQuotedString, null);

            Assert.True(isQuotedString);
            Assert.Equal(headerValue.Length, textIndex);
            Assert.True(token.Span.Equals(expect, StringComparison.Ordinal));
        }

        [Fact]
        public void ReadTokenOrQuotedStringValueShouldThrowEscapeCharAtEnd()
        {
            string headerName = "any";
            string headerValue = "\"token\\";
            int textIndex = 0;
            Action test = () => HttpUtils.ReadTokenOrQuotedStringValue(headerName, headerValue, ref textIndex, out _, (s) => new Exception(s));

            Exception exception = Assert.Throws<Exception>(test);
            Assert.Equal(Strings.HttpUtils_EscapeCharAtEnd(headerName, headerValue, 7, '\\'), exception.Message);
        }

        [Fact]
        public void ReadTokenOrQuotedStringValueShouldThrowInvalidCharacterInQuoted()
        {
            string headerName = "any";
            string headerValue = "\"tok\u001Ben\"";
            int textIndex = 0;
            Action test = () => HttpUtils.ReadTokenOrQuotedStringValue(headerName, headerValue, ref textIndex, out _, (s) => new Exception(s));

            Exception exception = Assert.Throws<Exception>(test);
            Assert.Equal(Strings.HttpUtils_InvalidCharacterInQuotedParameterValue(headerName, headerValue, 4, '\u001b'), exception.Message);
        }

        [Fact]
        public void ReadTokenOrQuotedStringValueShouldThrowClosingQuoteNotFound()
        {
            string headerName = "any";
            string headerValue = "\"token";
            int textIndex = 0;
            Action test = () => HttpUtils.ReadTokenOrQuotedStringValue(headerName, headerValue, ref textIndex, out _, (s) => new Exception(s));

            Exception exception = Assert.Throws<Exception>(test);
            Assert.Equal(Strings.HttpUtils_ClosingQuoteNotFound(headerName, headerValue, 6), exception.Message);
        }
    }
}

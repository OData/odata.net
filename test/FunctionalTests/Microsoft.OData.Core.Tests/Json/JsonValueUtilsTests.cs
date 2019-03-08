//---------------------------------------------------------------------
// <copyright file="JsonValueUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FluentAssertions;
using Microsoft.OData.Json;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    public class JsonValueUtilsTests
    {
        private MemoryStream stream;
        private NonIndentedTextWriter writer;
        private char[] buffer;
        private IDictionary<string, string> escapedCharMap;
        private IDictionary<string, string> controlCharsMap;

        public JsonValueUtilsTests()
        {
            controlCharsMap = new Dictionary<string, string>
            {
                {string.Format("{0}",(char)0),string.Format(CultureInfo.InvariantCulture, "\\u{0:x4}", 0)},
                {"\r\n","\\r\\n"},
                {"\r","\\r"},
                {"\t","\\t"},
                {"\"","\\\""},
                {"\\","\\\\"},
                {"\n","\\n"},
                {"\b","\\b"},
                {"\f","\\f"},
            };

            escapedCharMap = new Dictionary<string, string>
            {
                {string.Format("{0}",(char)0x80), string.Format(CultureInfo.InvariantCulture, "\\u{0:x4}", 0x80)},
                {"и",  "\\u0438"}
            };

            foreach (var item in controlCharsMap)
            {
                escapedCharMap.Add(item);
            }
        }

        [Theory]
        [InlineData(ODataStringEscapeOption.EscapeNonAscii)]
        [InlineData(ODataStringEscapeOption.EscapeOnlyControls)]
        public void WriteEmptyStringShouldWork(ODataStringEscapeOption stringEscapeOption)
        {
            this.TestInit();
            JsonValueUtils.WriteEscapedJsonString(this.writer, string.Empty, stringEscapeOption, ref this.buffer);
            this.StreamToString().Should().Be("\"\"");
        }

        [Theory]
        [InlineData(ODataStringEscapeOption.EscapeNonAscii)]
        [InlineData(ODataStringEscapeOption.EscapeOnlyControls)]
        public void WriteNonSpecialCharactersShouldWork(ODataStringEscapeOption stringEscapeOption)
        {
            this.TestInit();
            JsonValueUtils.WriteEscapedJsonString(this.writer, "abcdefg123", stringEscapeOption, ref this.buffer);
            this.StreamToString().Should().Be("\"abcdefg123\"");
        }

        [Theory]
        [InlineData(ODataStringEscapeOption.EscapeNonAscii)]
        [InlineData(ODataStringEscapeOption.EscapeOnlyControls)]
        public void WriteLowSpecialCharactersShouldWorkForEscapeOption(ODataStringEscapeOption stringEscapeOption)
        {
            this.TestInit();
            JsonValueUtils.WriteEscapedJsonString(this.writer, "cA_\n\r\b", stringEscapeOption, ref this.buffer);
            this.StreamToString().Should().Be("\"cA_\\n\\r\\b\"");
        }

        [Fact]
        public void WriteHighSpecialCharactersShouldWorkForEscapeNonAsciiOption()
        {
            this.TestInit();
            JsonValueUtils.WriteEscapedJsonString(this.writer, "cA_Россия", ODataStringEscapeOption.EscapeNonAscii, ref this.buffer);
            this.StreamToString().Should().Be("\"cA_\\u0420\\u043e\\u0441\\u0441\\u0438\\u044f\"");
        }

        [Fact]
        public void WriteHighSpecialCharactersShouldWorkForEscapeOnlyControlsOption()
        {
            this.TestInit();
            JsonValueUtils.WriteEscapedJsonString(this.writer, "cA_Россия", ODataStringEscapeOption.EscapeOnlyControls, ref this.buffer);
            this.StreamToString().Should().Be("\"cA_Россия\"");
        }

        [Fact]
        public void WriteSpecialCharactersShouldWork()
        {
            foreach (string specialChar in this.escapedCharMap.Keys)
            {
                this.TestInit();
                JsonValueUtils.WriteEscapedJsonString(this.writer, string.Format("{0}", specialChar),
                    ODataStringEscapeOption.EscapeNonAscii, ref this.buffer);
                this.StreamToString().Should().Be(string.Format("\"{0}\"", this.escapedCharMap[specialChar]));
            }
        }

        [Fact]
        public void WriteSpecialCharactersAtStartOfStringShouldWork()
        {
            foreach (string specialChar in this.escapedCharMap.Keys)
            {
                this.TestInit();
                JsonValueUtils.WriteEscapedJsonString(this.writer, string.Format("{0}MiddleEnd", specialChar),
                    ODataStringEscapeOption.EscapeNonAscii, ref this.buffer);
                this.StreamToString().Should().Be(string.Format("\"{0}MiddleEnd\"", this.escapedCharMap[specialChar]));
            }
        }

        [Fact]
        public void WriteSpecialCharactersAtMiddleOfStringShouldWork()
        {
            foreach (string specialChar in this.escapedCharMap.Keys)
            {
                this.TestInit();
                JsonValueUtils.WriteEscapedJsonString(this.writer, string.Format("Start{0}End", specialChar),
                    ODataStringEscapeOption.EscapeNonAscii, ref this.buffer);
                this.StreamToString().Should().Be(string.Format("\"Start{0}End\"", this.escapedCharMap[specialChar]));
            }
        }

        [Fact]
        public void WriteSpecialCharactersAtEndOfStringShouldWork()
        {
            foreach (string specialChar in this.escapedCharMap.Keys)
            {
                this.TestInit();
                JsonValueUtils.WriteEscapedJsonString(this.writer, string.Format("StartMiddle{0}", specialChar),
                    ODataStringEscapeOption.EscapeNonAscii, ref this.buffer);
                this.StreamToString().Should().Be(string.Format("\"StartMiddle{0}\"", this.escapedCharMap[specialChar]));
            }
        }

        [Fact]
        public void WriteMultipleSpecialCharactersShouldWork()
        {
            foreach (string specialChar in this.escapedCharMap.Keys)
            {
                this.TestInit();
                JsonValueUtils.WriteEscapedJsonString(this.writer, string.Format("{0}Start{0}Middle{0}End", specialChar),
                    ODataStringEscapeOption.EscapeNonAscii, ref this.buffer);
                this.StreamToString().Should().Be(string.Format("\"{0}Start{0}Middle{0}End\"", this.escapedCharMap[specialChar]));
            }
        }

        [Fact]
        public void WriteSpecialCharactersAtStartOfBufferLengthShouldWork()
        {
            foreach (string specialChar in this.escapedCharMap.Keys)
            {
                this.TestInit();
                char[] charBuffer = new char[10];
                JsonValueUtils.WriteEscapedJsonString(this.writer, string.Format("StartMiddle{0}End", specialChar),
                    ODataStringEscapeOption.EscapeNonAscii, ref charBuffer);
                this.StreamToString().Should().Be(string.Format("\"StartMiddle{0}End\"", this.escapedCharMap[specialChar]));
            }
        }

        [Fact]
        public void WriteMultipleSpecialCharactersAtEndOfBufferLengthShouldWork()
        {
            foreach (string specialChar in this.escapedCharMap.Keys)
            {
                this.TestInit();
                char[] charBuffer = new char[6];
                JsonValueUtils.WriteEscapedJsonString(this.writer, string.Format("Start{0}Middle{0}End{0}", specialChar),
                    ODataStringEscapeOption.EscapeNonAscii, ref charBuffer);
                this.StreamToString().Should().Be(string.Format("\"Start{0}Middle{0}End{0}\"", this.escapedCharMap[specialChar]));
            }
        }

        [Fact]
        public void WriteSpecialCharactersAtEndOfBufferLengthShouldWork()
        {
            foreach (string specialChar in this.escapedCharMap.Keys)
            {
                this.TestInit();
                char[] charBuffer = new char[6];
                JsonValueUtils.WriteEscapedJsonString(this.writer, string.Format("Start{0}", specialChar),
                    ODataStringEscapeOption.EscapeNonAscii, ref charBuffer);
                this.StreamToString().Should().Be(string.Format("\"Start{0}\"", this.escapedCharMap[specialChar]));
            }
        }

        [Theory]
        [InlineData(ODataStringEscapeOption.EscapeOnlyControls)]
        [InlineData(ODataStringEscapeOption.EscapeNonAscii)]
        public void WriteControllCharactersWithStringEscapeOptionShouldWork(ODataStringEscapeOption escapeOption)
        {
            foreach (string specialChar in this.controlCharsMap.Keys)
            {
                this.TestInit();
                char[] charBuffer = new char[6];
                JsonValueUtils.WriteEscapedJsonString(this.writer, string.Format("S{0}M{0}E{0}", specialChar),
                    escapeOption, ref charBuffer);
                this.StreamToString().Should().Be(string.Format("\"S{0}M{0}E{0}\"", this.controlCharsMap[specialChar]));
            }
        }

        [Theory]
        [InlineData(ODataStringEscapeOption.EscapeOnlyControls)]
        [InlineData(ODataStringEscapeOption.EscapeNonAscii)]
        public void WriteStringWithNoSpecialCharShouldLeaveBufferUntouched(ODataStringEscapeOption stringEscapeOption)
        {
            this.TestInit();
            char[] charBuffer = null;
            JsonValueUtils.WriteEscapedJsonString(this.writer, "StartMiddleEnd", stringEscapeOption, ref charBuffer);
            this.StreamToString().Should().Be("\"StartMiddleEnd\"");
            charBuffer.Should().BeNull("Char Buffer for cases with zero special characters should need to use buffer");
        }

        [Theory]
        [InlineData(ODataStringEscapeOption.EscapeOnlyControls)]
        [InlineData(ODataStringEscapeOption.EscapeNonAscii)]
        public void WriteStringShouldIgnoreExistingContentsOfTheBuffer(ODataStringEscapeOption stringEscapeOption)
        {
            this.TestInit();
            char[] charBuffer = new char[128];
            for (int index = 0; index < 128; index++)
            {
                charBuffer[index] = (char)index;
            }

            JsonValueUtils.WriteEscapedJsonString(this.writer, "StartVeryVeryLongMiddleEnd", stringEscapeOption, ref charBuffer);
            this.StreamToString().Should().Be("\"StartVeryVeryLongMiddleEnd\"");
        }

        [Fact]
        public void WriteByteShouldWork()
        {
            this.TestInit();
            var byteArray = new byte[] { 1, 2, 3 };
            JsonValueUtils.WriteValue(this.writer, byteArray, ref this.buffer);
            this.StreamToString().Should().Be("\"AQID\"");
        }

        [Fact]
        public void WriteLongBytesShouldWork()
        {
            // Arrange
            this.TestInit();
            byte[] byteArray = new byte[1000];
            for (int i = 0; i < byteArray.Length; i++)
            {
                byteArray[i] = (byte)(i % 255);
            }

            // Act, Get Base64 string from OData library
            JsonValueUtils.WriteValue(this.writer, byteArray, ref this.buffer);
            string convertedBase64String = this.StreamToString();

            // Get Base64 string directly calling the converter.
            string actualBase64String = JsonConstants.QuoteCharacter + Convert.ToBase64String(byteArray) + JsonConstants.QuoteCharacter;

            // Assert
            Assert.Equal(convertedBase64String, actualBase64String);
            Assert.Equal("\"AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGxwdHh8gISIjJCUmJygpKissLS4vMDEyMzQ1Njc4OTo7PD0+P0BBQkNERUZHSElKS0xNTk9QUVJTVFVWV1hZWltcXV5fYGFiY2RlZmdoaWprbG1ub3BxcnN0dXZ3eHl6e3x9fn+AgYKDhIWGh4iJiouMjY6PkJGSk5SVlpeYmZqbnJ2en6ChoqOkpaanqKmqq6ytrq+wsbKztLW2t7i5uru8vb6/wMHCw8TFxsfIycrLzM3Oz9DR0tPU1dbX2Nna29zd3t/g4eLj5OXm5+jp6uvs7e7v8PHy8/T19vf4+fr7/P3+AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGxwdHh8gISIjJCUmJygpKissLS4vMDEyMzQ1Njc4OTo7PD0+P0BBQkNERUZHSElKS0xNTk9QUVJTVFVWV1hZWltcXV5fYGFiY2RlZmdoaWprbG1ub3BxcnN0dXZ3eHl6e3x9fn+AgYKDhIWGh4iJiouMjY6PkJGSk5SVlpeYmZqbnJ2en6ChoqOkpaanqKmqq6ytrq+wsbKztLW2t7i5uru8vb6/wMHCw8TFxsfIycrLzM3Oz9DR0tPU1dbX2Nna29zd3t/g4eLj5OXm5+jp6uvs7e7v8PHy8/T19vf4+fr7/P3+AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGxwdHh8gISIjJCUmJygpKissLS4vMDEyMzQ1Njc4OTo7PD0+P0BBQkNERUZHSElKS0xNTk9QUVJTVFVWV1hZWltcXV5fYGFiY2RlZmdoaWprbG1ub3BxcnN0dXZ3eHl6e3x9fn+AgYKDhIWGh4iJiouMjY6PkJGSk5SVlpeYmZqbnJ2en6ChoqOkpaanqKmqq6ytrq+wsbKztLW2t7i5uru8vb6/wMHCw8TFxsfIycrLzM3Oz9DR0tPU1dbX2Nna29zd3t/g4eLj5OXm5+jp6uvs7e7v8PHy8/T19vf4+fr7/P3+AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGxwdHh8gISIjJCUmJygpKissLS4vMDEyMzQ1Njc4OTo7PD0+P0BBQkNERUZHSElKS0xNTk9QUVJTVFVWV1hZWltcXV5fYGFiY2RlZmdoaWprbG1ub3BxcnN0dXZ3eHl6e3x9fn+AgYKDhIWGh4iJiouMjY6PkJGSk5SVlpeYmZqbnJ2en6ChoqOkpaanqKmqq6ytrq+wsbKztLW2t7i5uru8vb6/wMHCw8TFxsfIycrLzM3Oz9DR0tPU1dbX2Nna29zd3t/g4eLj5OXm5+jp6g==\"", convertedBase64String);
        }

        [Fact]
        public void WriteBytesLengthSameAsBufferSizeShouldWork()
        {
            // Arrange
            this.TestInit();

            // Initialize the buffer
            int bufferLength = 40;
            this.buffer = new char[bufferLength];
            int byteSize = bufferLength * 3 / 4;

            // Make the output Base64 string length same as the buffer size.
            byte[] byteArray = new byte[byteSize];
            for (int i = 0; i < byteArray.Length; i++)
            {
                byteArray[i] = (byte)(i % 255);
            }

            // Act, Get Base64 string from OData library
            JsonValueUtils.WriteValue(this.writer, byteArray, ref this.buffer);
            string convertedBase64String = this.StreamToString();

            // Get Base64 string directly calling the converter.
            string actualBase64String = JsonConstants.QuoteCharacter + Convert.ToBase64String(byteArray) + JsonConstants.QuoteCharacter;

            // Assert
            Assert.Equal(convertedBase64String, actualBase64String);
            Assert.Equal("\"AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGxwd\"", convertedBase64String);
        }

        [Fact]
        public void WriteEmptyByteShouldWork()
        {
            this.TestInit();
            var byteArray = new byte[] { };
            JsonValueUtils.WriteValue(this.writer, byteArray, ref this.buffer);
            this.StreamToString().Should().Be("\"\"");
        }

        [Fact]
        public void WriteNullByteShouldWork()
        {
            this.TestInit();
            JsonValueUtils.WriteValue(this.writer, (byte[])null, ref this.buffer);
            this.StreamToString().Should().Be("null");
        }

        private void TestInit()
        {
            this.stream = new MemoryStream();
            StreamWriter innerWriter = new StreamWriter(this.stream);
            innerWriter.AutoFlush = true;
            this.writer = new NonIndentedTextWriter(innerWriter);
        }

        private string StreamToString()
        {
            this.stream.Position = 0;
            return (new StreamReader(this.stream)).ReadToEnd();
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="JsonValueUtilsAsyncTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OData.Json;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    public class JsonValueUtilsAsyncTests
    {
        private Stream stream;
        private NonIndentedTextWriter writer;
        private Ref<char[]> buffer;
        private IDictionary<string, string> escapedCharMap;
        private IDictionary<string, string> controlCharsMap;

        public JsonValueUtilsAsyncTests()
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

            // Ran before each test
            this.Reset();
        }

        [Theory]
        [InlineData(ODataStringEscapeOption.EscapeNonAscii)]
        [InlineData(ODataStringEscapeOption.EscapeOnlyControls)]
        public async Task WriteEmptyStringShouldWork(ODataStringEscapeOption stringEscapeOption)
        {
            await JsonValueUtils.WriteEscapedJsonStringAsync(this.writer, string.Empty, stringEscapeOption, this.buffer);
            Assert.Equal("\"\"", await this.StreamToStringAsync());
        }

        [Theory]
        [InlineData(ODataStringEscapeOption.EscapeNonAscii)]
        [InlineData(ODataStringEscapeOption.EscapeOnlyControls)]
        public async Task WriteNonSpecialCharactersShouldWork(ODataStringEscapeOption stringEscapeOption)
        {
            await JsonValueUtils.WriteEscapedJsonStringAsync(this.writer, "abcdefg123", stringEscapeOption, this.buffer);
            Assert.Equal("\"abcdefg123\"", await this.StreamToStringAsync());
        }

        [Theory]
        [InlineData(ODataStringEscapeOption.EscapeNonAscii)]
        [InlineData(ODataStringEscapeOption.EscapeOnlyControls)]
        public async Task WriteLowSpecialCharactersShouldWorkForEscapeOption(ODataStringEscapeOption stringEscapeOption)
        {
            await JsonValueUtils.WriteEscapedJsonStringAsync(this.writer, "cA_\n\r\b", stringEscapeOption, this.buffer);
            Assert.Equal("\"cA_\\n\\r\\b\"", await this.StreamToStringAsync());
        }

        [Fact]
        public async Task WriteHighSpecialCharactersShouldWorkForEscapeNonAsciiOption()
        {
            await JsonValueUtils.WriteEscapedJsonStringAsync(this.writer, "cA_Россия", ODataStringEscapeOption.EscapeNonAscii, this.buffer);
            Assert.Equal("\"cA_\\u0420\\u043e\\u0441\\u0441\\u0438\\u044f\"", await this.StreamToStringAsync());
        }

        [Fact]
        public async Task WriteHighSpecialCharactersShouldWorkForEscapeOnlyControlsOption()
        {
            await JsonValueUtils.WriteEscapedJsonStringAsync(this.writer, "cA_Россия", ODataStringEscapeOption.EscapeOnlyControls, this.buffer);
            Assert.Equal("\"cA_Россия\"", await this.StreamToStringAsync());
        }

        [Fact]
        public async Task WriteSpecialCharactersShouldWork()
        {
            foreach (string specialChar in this.escapedCharMap.Keys)
            {
                this.Reset();
                await JsonValueUtils.WriteEscapedJsonStringAsync(this.writer, string.Format("{0}", specialChar),
                    ODataStringEscapeOption.EscapeNonAscii, this.buffer);
                Assert.Equal(string.Format("\"{0}\"", this.escapedCharMap[specialChar]), await this.StreamToStringAsync());
            }
        }

        [Fact]
        public async Task WriteSpecialCharactersAtStartOfStringShouldWork()
        {
            foreach (string specialChar in this.escapedCharMap.Keys)
            {
                this.Reset();
                await JsonValueUtils.WriteEscapedJsonStringAsync(this.writer, string.Format("{0}MiddleEnd", specialChar),
                    ODataStringEscapeOption.EscapeNonAscii, this.buffer);
                Assert.Equal(string.Format("\"{0}MiddleEnd\"", this.escapedCharMap[specialChar]), await this.StreamToStringAsync());
            }
        }

        [Fact]
        public async Task WriteSpecialCharactersAtMiddleOfStringShouldWork()
        {
            foreach (string specialChar in this.escapedCharMap.Keys)
            {
                this.Reset();
                await JsonValueUtils.WriteEscapedJsonStringAsync(this.writer, string.Format("Start{0}End", specialChar),
                    ODataStringEscapeOption.EscapeNonAscii, this.buffer);
                Assert.Equal(string.Format("\"Start{0}End\"", this.escapedCharMap[specialChar]), await this.StreamToStringAsync());
            }
        }

        [Fact]
        public async Task WriteSpecialCharactersAtEndOfStringShouldWork()
        {
            foreach (string specialChar in this.escapedCharMap.Keys)
            {
                this.Reset();
                await JsonValueUtils.WriteEscapedJsonStringAsync(this.writer, string.Format("StartMiddle{0}", specialChar),
                    ODataStringEscapeOption.EscapeNonAscii, this.buffer);
                Assert.Equal(string.Format("\"StartMiddle{0}\"", this.escapedCharMap[specialChar]), await this.StreamToStringAsync());
            }
        }

        [Fact]
        public async Task WriteMultipleSpecialCharactersShouldWork()
        {
            foreach (string specialChar in this.escapedCharMap.Keys)
            {
                this.Reset();
                await JsonValueUtils.WriteEscapedJsonStringAsync(this.writer, string.Format("{0}Start{0}Middle{0}End", specialChar),
                    ODataStringEscapeOption.EscapeNonAscii, this.buffer);
                Assert.Equal(string.Format("\"{0}Start{0}Middle{0}End\"", this.escapedCharMap[specialChar]), await this.StreamToStringAsync());
            }
        }

        [Fact]
        public async Task WriteSpecialCharactersAtStartOfBufferLengthShouldWork()
        {
            foreach (string specialChar in this.escapedCharMap.Keys)
            {
                this.Reset();
                Ref<char[]> charBuffer = new Ref<char[]>(new char[10]);
                await JsonValueUtils.WriteEscapedJsonStringAsync(this.writer, string.Format("StartMiddle{0}End", specialChar),
                    ODataStringEscapeOption.EscapeNonAscii, charBuffer);
                Assert.Equal(string.Format("\"StartMiddle{0}End\"", this.escapedCharMap[specialChar]), await this.StreamToStringAsync());
            }
        }

        [Fact]
        public async Task WriteMultipleSpecialCharactersAtEndOfBufferLengthShouldWork()
        {
            foreach (string specialChar in this.escapedCharMap.Keys)
            {
                this.Reset();
                Ref<char[]> charBuffer = new Ref<char[]>(new char[6]);
                await JsonValueUtils.WriteEscapedJsonStringAsync(this.writer, string.Format("Start{0}Middle{0}End{0}", specialChar),
                    ODataStringEscapeOption.EscapeNonAscii, charBuffer);
                Assert.Equal(string.Format("\"Start{0}Middle{0}End{0}\"", this.escapedCharMap[specialChar]), await this.StreamToStringAsync());
            }
        }

        [Fact]
        public async Task WriteSpecialCharactersAtEndOfBufferLengthShouldWork()
        {
            foreach (string specialChar in this.escapedCharMap.Keys)
            {
                this.Reset();
                Ref<char[]> charBuffer = new Ref<char[]>(new char[6]);
                await JsonValueUtils.WriteEscapedJsonStringAsync(this.writer, string.Format("Start{0}", specialChar),
                    ODataStringEscapeOption.EscapeNonAscii, charBuffer);
                Assert.Equal(string.Format("\"Start{0}\"", this.escapedCharMap[specialChar]), await this.StreamToStringAsync());
            }
        }

        [Theory]
        [InlineData(ODataStringEscapeOption.EscapeOnlyControls)]
        [InlineData(ODataStringEscapeOption.EscapeNonAscii)]
        public async Task WriteControllCharactersWithStringEscapeOptionShouldWork(ODataStringEscapeOption escapeOption)
        {
            foreach (string specialChar in this.controlCharsMap.Keys)
            {
                this.Reset();
                Ref<char[]> charBuffer = new Ref<char[]>(new char[6]);
                await JsonValueUtils.WriteEscapedJsonStringAsync(this.writer, string.Format("S{0}M{0}E{0}", specialChar),
                    escapeOption, charBuffer);
                Assert.Equal(string.Format("\"S{0}M{0}E{0}\"", this.controlCharsMap[specialChar]), await this.StreamToStringAsync());
            }
        }

        [Fact]
        public async Task WriteLongEscapedStringShouldWork()
        {
            this.Reset();
            Ref<char[]> charBuffer = new Ref<char[]>(new char[6]);
            string value = string.Join("\t", Enumerable.Repeat('\n', 95000));
            await JsonValueUtils.WriteEscapedJsonStringAsync(this.writer, value,
                ODataStringEscapeOption.EscapeNonAscii, charBuffer);
            string expected = "\"" + string.Join("\\t", Enumerable.Repeat("\\n", 95000)) + "\"";
            Assert.Equal(expected, await this.StreamToStringAsync());
        }

        [Fact]
        public async Task WriteLongEscapedCharArrayShouldWork()
        {
            Ref<char[]> charBuffer = new Ref<char[]>(new char[6]);
            string value = string.Join("\t", Enumerable.Repeat('\n', 95000));
            await JsonValueUtils.WriteEscapedCharArrayAsync(
                this.writer,
                value.ToCharArray(),
                0,
                value.Length,
                ODataStringEscapeOption.EscapeNonAscii,
                charBuffer,
                null);
            string expected = string.Join("\\t", Enumerable.Repeat("\\n", 95000));
            Assert.Equal(expected, await this.StreamToStringAsync());
        }

        [Theory]
        [InlineData(ODataStringEscapeOption.EscapeOnlyControls)]
        [InlineData(ODataStringEscapeOption.EscapeNonAscii)]
        public async Task WriteStringWithNoSpecialCharShouldLeaveBufferUntouched(ODataStringEscapeOption stringEscapeOption)
        {
            this.Reset();
            Ref<char[]> charBuffer = new Ref<char[]>(null);
            await JsonValueUtils.WriteEscapedJsonStringAsync(this.writer, "StartMiddleEnd", stringEscapeOption, charBuffer);
            Assert.Equal("\"StartMiddleEnd\"", await this.StreamToStringAsync());
            Assert.Null(charBuffer.Value);
        }

        [Theory]
        [InlineData(ODataStringEscapeOption.EscapeOnlyControls)]
        [InlineData(ODataStringEscapeOption.EscapeNonAscii)]
        public async Task WriteStringShouldIgnoreExistingContentsOfTheBuffer(ODataStringEscapeOption stringEscapeOption)
        {
            Ref<char[]> charBuffer = new Ref<char[]>(new char[128]);
            for (int index = 0; index < 128; index++)
            {
                charBuffer.Value[index] = (char)index;
            }

            await JsonValueUtils.WriteEscapedJsonStringAsync(this.writer, "StartVeryVeryLongMiddleEnd", stringEscapeOption, charBuffer);
            Assert.Equal("\"StartVeryVeryLongMiddleEnd\"", await this.StreamToStringAsync());
        }

        [Fact]
        public async Task WriteByteShouldWork()
        {
            var byteArray = new byte[] { 1, 2, 3 };
            await JsonValueUtils.WriteValueAsync(this.writer, byteArray, this.buffer);
            Assert.Equal("\"AQID\"", await this.StreamToStringAsync());
        }

        [Fact]
        public async Task WriteLongBytesShouldWork()
        {
            // Arrange
            byte[] byteArray = new byte[1000];
            for (int i = 0; i < byteArray.Length; i++)
            {
                byteArray[i] = (byte)(i % 255);
            }

            // Act, Get Base64 string from OData library
            await JsonValueUtils.WriteValueAsync(this.writer, byteArray, this.buffer);
            string convertedBase64String = await this.StreamToStringAsync();

            // Get Base64 string directly calling the converter.
            string actualBase64String = JsonConstants.QuoteCharacter + Convert.ToBase64String(byteArray) + JsonConstants.QuoteCharacter;

            // Assert
            Assert.Equal(convertedBase64String, actualBase64String);
            Assert.Equal("\"AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGxwdHh8gISIjJCUmJygpKissLS4vMDEyMzQ1Njc4OTo7PD0+P0BBQkNERUZHSElKS0xNTk9QUVJTVFVWV1hZWltcXV5fYGFiY2RlZmdoaWprbG1ub3BxcnN0dXZ3eHl6e3x9fn+AgYKDhIWGh4iJiouMjY6PkJGSk5SVlpeYmZqbnJ2en6ChoqOkpaanqKmqq6ytrq+wsbKztLW2t7i5uru8vb6/wMHCw8TFxsfIycrLzM3Oz9DR0tPU1dbX2Nna29zd3t/g4eLj5OXm5+jp6uvs7e7v8PHy8/T19vf4+fr7/P3+AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGxwdHh8gISIjJCUmJygpKissLS4vMDEyMzQ1Njc4OTo7PD0+P0BBQkNERUZHSElKS0xNTk9QUVJTVFVWV1hZWltcXV5fYGFiY2RlZmdoaWprbG1ub3BxcnN0dXZ3eHl6e3x9fn+AgYKDhIWGh4iJiouMjY6PkJGSk5SVlpeYmZqbnJ2en6ChoqOkpaanqKmqq6ytrq+wsbKztLW2t7i5uru8vb6/wMHCw8TFxsfIycrLzM3Oz9DR0tPU1dbX2Nna29zd3t/g4eLj5OXm5+jp6uvs7e7v8PHy8/T19vf4+fr7/P3+AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGxwdHh8gISIjJCUmJygpKissLS4vMDEyMzQ1Njc4OTo7PD0+P0BBQkNERUZHSElKS0xNTk9QUVJTVFVWV1hZWltcXV5fYGFiY2RlZmdoaWprbG1ub3BxcnN0dXZ3eHl6e3x9fn+AgYKDhIWGh4iJiouMjY6PkJGSk5SVlpeYmZqbnJ2en6ChoqOkpaanqKmqq6ytrq+wsbKztLW2t7i5uru8vb6/wMHCw8TFxsfIycrLzM3Oz9DR0tPU1dbX2Nna29zd3t/g4eLj5OXm5+jp6uvs7e7v8PHy8/T19vf4+fr7/P3+AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGxwdHh8gISIjJCUmJygpKissLS4vMDEyMzQ1Njc4OTo7PD0+P0BBQkNERUZHSElKS0xNTk9QUVJTVFVWV1hZWltcXV5fYGFiY2RlZmdoaWprbG1ub3BxcnN0dXZ3eHl6e3x9fn+AgYKDhIWGh4iJiouMjY6PkJGSk5SVlpeYmZqbnJ2en6ChoqOkpaanqKmqq6ytrq+wsbKztLW2t7i5uru8vb6/wMHCw8TFxsfIycrLzM3Oz9DR0tPU1dbX2Nna29zd3t/g4eLj5OXm5+jp6g==\"", convertedBase64String);
        }

        [Fact]
        public async Task WriteBytesLengthSameAsBufferSizeShouldWork()
        {
            // Arrange

            // Initialize the buffer
            int bufferLength = 40;
            this.buffer = new Ref<char[]>(new char[bufferLength]);
            int byteSize = bufferLength * 3 / 4;

            // Make the output Base64 string length same as the buffer size.
            byte[] byteArray = new byte[byteSize];
            for (int i = 0; i < byteArray.Length; i++)
            {
                byteArray[i] = (byte)(i % 255);
            }

            // Act, Get Base64 string from OData library
            await JsonValueUtils.WriteValueAsync(this.writer, byteArray, this.buffer);
            string convertedBase64String = await this.StreamToStringAsync();

            // Get Base64 string directly calling the converter.
            string actualBase64String = JsonConstants.QuoteCharacter + Convert.ToBase64String(byteArray) + JsonConstants.QuoteCharacter;

            // Assert
            Assert.Equal(convertedBase64String, actualBase64String);
            Assert.Equal("\"AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGxwd\"", convertedBase64String);
        }

        [Fact]
        public async Task WriteEmptyByteShouldWork()
        {
            var byteArray = new byte[] { };
            await JsonValueUtils.WriteValueAsync(this.writer, byteArray, this.buffer);
            Assert.Equal("\"\"", await this.StreamToStringAsync());
        }

        [Fact]
        public async Task WriteNullByteShouldWork()
        {
            await JsonValueUtils.WriteValueAsync(this.writer, (byte[])null, this.buffer);
            Assert.Equal("null", await this.StreamToStringAsync());
        }

        private void Reset()
        {
            this.buffer = new Ref<char[]>();
            this.stream = new AsyncStream(new MemoryStream());
            StreamWriter innerWriter = new StreamWriter(this.stream);
            this.writer = new NonIndentedTextWriter(innerWriter);
        }

        private async Task<string> StreamToStringAsync()
        {
            await this.writer.FlushAsync();
            this.stream.Position = 0;
            string result = await (new StreamReader(this.stream)).ReadToEndAsync();
            return result;
        }
    }
}

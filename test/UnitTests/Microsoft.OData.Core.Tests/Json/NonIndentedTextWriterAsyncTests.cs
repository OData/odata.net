//---------------------------------------------------------------------
// <copyright file="NonIndentedTextWriterAsyncTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.OData.Json;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    public class NonIndentedTextWriterAsyncTests
    {
        private Stream stream;
        private TextWriterWrapper writer;
        private Ref<char[]> buffer;
        private Dictionary<string, string> escapedCharMap;

        public NonIndentedTextWriterAsyncTests()
        {
            escapedCharMap = new Dictionary<string, string>()
            {
                {"\r\n", "\\r\\n"},
                {"\r", "\\r"},
                {"\t", "\\t"},
                {"\"", "\\\""},
                {"\\", "\\\\"},
                {"\n", "\\n"},
                {"\b", "\\b"},
                {"\f", "\\f"},
                {string.Format("{0}", (char)0), string.Format(CultureInfo.InvariantCulture, "\\u{0:x4}", 0)},
                {string.Format("{0}", (char)0x80), string.Format(CultureInfo.InvariantCulture, "\\u{0:x4}", 0x80)},
            };

            Reset();
        }

        [Theory]
        [InlineData(ODataStringEscapeOption.EscapeOnlyControls)]
        [InlineData(ODataStringEscapeOption.EscapeNonAscii)]
        public async Task WriteEmptyStringShouldWork(ODataStringEscapeOption stringEscapeOption)
        {
            await JsonValueUtils.WriteEscapedJsonStringAsync(this.writer, string.Empty, stringEscapeOption, this.buffer);
            Assert.Equal("\"\"", await this.StreamToStringAsync());
        }

        [Theory]
        [InlineData(ODataStringEscapeOption.EscapeOnlyControls)]
        [InlineData(ODataStringEscapeOption.EscapeNonAscii)]
        public async Task WriteNonSpecialCharactersShouldWork(ODataStringEscapeOption stringEscapeOption)
        {
            await JsonValueUtils.WriteEscapedJsonStringAsync(this.writer, "abcdefg123", stringEscapeOption, this.buffer);
            Assert.Equal("\"abcdefg123\"", await this.StreamToStringAsync());
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
        public async Task WriteByteShouldWork()
        {
            var byteArray = new byte[] { 1, 2, 3 };
            await JsonValueUtils.WriteValueAsync(this.writer, byteArray, this.buffer);
            Assert.Equal("\"AQID\"", await this.StreamToStringAsync());
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
            var result = await (new StreamReader(this.stream)).ReadToEndAsync();
            return result;
        }
    }
}

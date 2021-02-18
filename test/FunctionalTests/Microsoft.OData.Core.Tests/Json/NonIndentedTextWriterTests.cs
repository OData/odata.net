//---------------------------------------------------------------------
// <copyright file="NonIndentedTextWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.OData.Json;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    public class NonIndentedTextWriterTests
    {
        private MemoryStream stream;
        private TextWriterWrapper writer;
        private Ref<char[]> buffer;
        private Dictionary<string, string> escapedCharMap = new Dictionary<string, string>()
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

        [Theory]
        [InlineData(ODataStringEscapeOption.EscapeOnlyControls)]
        [InlineData(ODataStringEscapeOption.EscapeNonAscii)]
        public void WriteEmptyStringShouldWork(ODataStringEscapeOption stringEscapeOption)
        {
            this.TestInit();
            JsonValueUtils.WriteEscapedJsonString(this.writer, string.Empty, stringEscapeOption, this.buffer);
            Assert.Equal("\"\"", this.StreamToString());
        }

        [Theory]
        [InlineData(ODataStringEscapeOption.EscapeOnlyControls)]
        [InlineData(ODataStringEscapeOption.EscapeNonAscii)]
        public void WriteNonSpecialCharactersShouldWork(ODataStringEscapeOption stringEscapeOption)
        {
            this.TestInit();
            JsonValueUtils.WriteEscapedJsonString(this.writer, "abcdefg123", stringEscapeOption, this.buffer);
            Assert.Equal("\"abcdefg123\"", this.StreamToString());
        }

        [Fact]
        public void WriteSpecialCharactersShouldWork()
        {
            foreach (string specialChar in this.escapedCharMap.Keys)
            {
                this.TestInit();
                JsonValueUtils.WriteEscapedJsonString(this.writer, string.Format("{0}", specialChar),
                    ODataStringEscapeOption.EscapeNonAscii, this.buffer);
                Assert.Equal(string.Format("\"{0}\"", this.escapedCharMap[specialChar]), this.StreamToString());
            }
        }

        [Fact]
        public void WriteHighSpecialCharactersShouldWorkForEscapeNonAsciiOption()
        {
            this.TestInit();
            JsonValueUtils.WriteEscapedJsonString(this.writer, "cA_Россия", ODataStringEscapeOption.EscapeNonAscii, this.buffer);
            Assert.Equal("\"cA_\\u0420\\u043e\\u0441\\u0441\\u0438\\u044f\"", this.StreamToString());
        }

        [Fact]
        public void WriteHighSpecialCharactersShouldWorkForEscapeOnlyControlsOption()
        {
            this.TestInit();
            JsonValueUtils.WriteEscapedJsonString(this.writer, "cA_Россия", ODataStringEscapeOption.EscapeOnlyControls, this.buffer);
            Assert.Equal("\"cA_Россия\"", this.StreamToString());
        }

        [Fact]
        public void WriteByteShouldWork()
        {
            this.TestInit();
            var byteArray = new byte[] { 1, 2, 3 };
            JsonValueUtils.WriteValue(this.writer, byteArray, this.buffer);
            Assert.Equal("\"AQID\"", this.StreamToString());
        }

        private void TestInit()
        {
            this.stream = new MemoryStream();
            StreamWriter innerWriter = new StreamWriter(this.stream);
            innerWriter.AutoFlush = true;
            this.writer = new NonIndentedTextWriter(innerWriter);
            this.buffer = new Ref<char[]>();
        }

        private string StreamToString()
        {
            this.stream.Position = 0;
            return (new StreamReader(this.stream)).ReadToEnd();
        }
    }
}
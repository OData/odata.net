//---------------------------------------------------------------------
// <copyright file="NonIndentedTextWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FluentAssertions;
using Microsoft.OData.Json;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    public class NonIndentedTextWriterTests
    {
        private MemoryStream stream;
        private TextWriterWrapper writer;
        private char[] buffer;
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
            JsonValueUtils.WriteEscapedJsonString(this.writer, string.Empty, stringEscapeOption, ref this.buffer);
            this.StreamToString().Should().Be("\"\"");
        }

        [Theory]
        [InlineData(ODataStringEscapeOption.EscapeOnlyControls)]
        [InlineData(ODataStringEscapeOption.EscapeNonAscii)]
        public void WriteNonSpecialCharactersShouldWork(ODataStringEscapeOption stringEscapeOption)
        {
            this.TestInit();
            JsonValueUtils.WriteEscapedJsonString(this.writer, "abcdefg123", stringEscapeOption, ref this.buffer);
            this.StreamToString().Should().Be("\"abcdefg123\"");
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
        public void WriteByteShouldWork()
        {
            this.TestInit();
            var byteArray = new byte[] { 1, 2, 3 };
            JsonValueUtils.WriteValue(this.writer, byteArray);
            this.StreamToString().Should().Be("\"AQID\"");
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
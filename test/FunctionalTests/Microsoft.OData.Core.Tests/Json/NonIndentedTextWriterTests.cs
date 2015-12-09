//---------------------------------------------------------------------
// <copyright file="NonIndentedTextWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FluentAssertions;
using Microsoft.OData.Core.Json;
using Xunit;

namespace Microsoft.OData.Core.Tests.Json
{
    public class NonIndentedTextWriterTests
    {
        private MemoryStream stream;
        private TextWriterWrapper writer;

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

        [Fact]
        public void WriteEmptyStringShouldWork()
        {
            this.TestInit();
            JsonValueUtils.WriteEscapedJsonString(this.writer, string.Empty);
            this.StreamToString().Should().Be("\"\"");
        }

        [Fact]
        public void WriteNonSpecialCharactersShouldWork()
        {
            this.TestInit();
            JsonValueUtils.WriteEscapedJsonString(this.writer, "abcdefg123");
            this.StreamToString().Should().Be("\"abcdefg123\"");
        }

        [Fact]
        public void WriteSpecialCharactersShouldWork()
        {
            foreach (string specialChar in this.escapedCharMap.Keys)
            {
                this.TestInit();
                JsonValueUtils.WriteEscapedJsonString(this.writer, string.Format("{0}", specialChar));
                this.StreamToString().Should().Be(string.Format("\"{0}\"", this.escapedCharMap[specialChar]));
            }
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
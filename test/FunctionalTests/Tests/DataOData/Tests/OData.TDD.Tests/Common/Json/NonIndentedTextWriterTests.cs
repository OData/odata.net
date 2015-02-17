//---------------------------------------------------------------------
// <copyright file="NonIndentedTextWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Common.Json
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using FluentAssertions;
    using Microsoft.OData.Core.Json;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
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

        [TestInitialize]
        public void TestInit()
        {
            this.stream = new MemoryStream();
            StreamWriter innerWriter = new StreamWriter(this.stream);
            innerWriter.AutoFlush = true;
            this.writer = new NonIndentedTextWriter(innerWriter);
        }

        [TestMethod]
        public void WriteEmptyStringShouldWork()
        {
            JsonValueUtils.WriteEscapedJsonString(this.writer, string.Empty);
            this.StreamToString().Should().Be("\"\"");
        }

        [TestMethod]
        public void WriteNonSpecialCharactersShouldWork()
        {
            JsonValueUtils.WriteEscapedJsonString(this.writer, "abcdefg123");
            this.StreamToString().Should().Be("\"abcdefg123\"");
        }

        [TestMethod]
        public void WriteSpecialCharactersShouldWork()
        {
            foreach (string specialChar in this.escapedCharMap.Keys)
            {
                this.TestInit();
                JsonValueUtils.WriteEscapedJsonString(this.writer, string.Format("{0}", specialChar));
                this.StreamToString().Should().Be(string.Format("\"{0}\"", this.escapedCharMap[specialChar]));
            }
        }

        [TestMethod]
        public void WriteByteShouldWork()
        {
            this.TestInit();
            var byteArray = new byte[] { 1, 2, 3 };
            JsonValueUtils.WriteValue(this.writer, byteArray);
            this.StreamToString().Should().Be("\"AQID\"");
        }

        private string StreamToString()
        {
            this.stream.Position = 0;
            return (new StreamReader(this.stream)).ReadToEnd();
        }
    }
}
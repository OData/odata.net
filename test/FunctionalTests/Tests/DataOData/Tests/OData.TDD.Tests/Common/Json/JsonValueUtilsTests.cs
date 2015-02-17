//---------------------------------------------------------------------
// <copyright file="JsonValueUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Test.OData.TDD.Tests.Common.Json
{
    using System.Globalization;
    using System.IO;
    using FluentAssertions;
    using Microsoft.OData.Core.Json;

    [TestClass]
    public class JsonValueUtilsTests
    {
        private MemoryStream stream;
        private IndentedTextWriter writer;

        private Dictionary<string, string> escapedCharMap = new Dictionary<string, string>()
        {
            {"\r\n","\\r\\n"},
            {"\r","\\r"},
            {"\t","\\t"},
            {"\"","\\\""},
            {"\\","\\\\"},
            {"\n","\\n"},
            {"\b","\\b"},
            {"\f","\\f"},
            {string.Format("{0}",(char)0),string.Format(CultureInfo.InvariantCulture, "\\u{0:x4}", 0)},
            {string.Format("{0}",(char)0x80), string.Format(CultureInfo.InvariantCulture, "\\u{0:x4}", 0x80)},
        };

        [TestInitialize]
        public void TestInit()
        {
            this.stream = new MemoryStream();
            StreamWriter innerWriter = new StreamWriter(this.stream);
            innerWriter.AutoFlush = true;
            this.writer = new IndentedTextWriter(innerWriter);
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
        public void WriteSpecialCharactersAtStartOfStringShouldWork()
        {
            foreach (string specialChar in this.escapedCharMap.Keys)
            {
                this.TestInit();
                JsonValueUtils.WriteEscapedJsonString(this.writer, string.Format("{0}MiddleEnd", specialChar));
                this.StreamToString().Should().Be(string.Format("\"{0}MiddleEnd\"", this.escapedCharMap[specialChar]));
            }
        }

        [TestMethod]
        public void WriteSpecialCharactersAtMiddleOfStringShouldWork()
        {
            foreach (string specialChar in this.escapedCharMap.Keys)
            {
                this.TestInit();
                JsonValueUtils.WriteEscapedJsonString(this.writer, string.Format("Start{0}End", specialChar));
                this.StreamToString().Should().Be(string.Format("\"Start{0}End\"", this.escapedCharMap[specialChar]));
            }
        }
        
        [TestMethod]
        public void WriteSpecialCharactersAtEndOfStringShouldWork()
        {
            foreach (string specialChar in this.escapedCharMap.Keys)
            {
                this.TestInit();
                JsonValueUtils.WriteEscapedJsonString(this.writer, string.Format("StartMiddle{0}", specialChar));
                this.StreamToString().Should().Be(string.Format("\"StartMiddle{0}\"", this.escapedCharMap[specialChar]));
            }
        }

        [TestMethod]
        public void WriteMultipleSpecialCharactersShouldWork()
        {
            foreach (string specialChar in this.escapedCharMap.Keys)
            {
                this.TestInit();
                JsonValueUtils.WriteEscapedJsonString(this.writer, string.Format("{0}Start{0}Middle{0}End", specialChar));
                this.StreamToString().Should().Be(string.Format("\"{0}Start{0}Middle{0}End\"", this.escapedCharMap[specialChar]));
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

        [TestMethod]
        public void WriteEmptyByteShouldWork()
        {
            this.TestInit();
            var byteArray = new byte[]{};
            JsonValueUtils.WriteValue(this.writer, byteArray);
            this.StreamToString().Should().Be("\"\"");
        }

        [TestMethod]
        public void WriteNullByteShouldWork()
        {
            this.TestInit();
            JsonValueUtils.WriteValue(this.writer, (byte[])null);
            this.StreamToString().Should().Be("null");
        }

        private string StreamToString()
        {
            this.stream.Position = 0;
            return (new StreamReader(this.stream)).ReadToEnd();
        }
    }
}

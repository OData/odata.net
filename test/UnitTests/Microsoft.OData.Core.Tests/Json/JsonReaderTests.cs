﻿//---------------------------------------------------------------------
// <copyright file="JsonReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Buffers;
using Microsoft.OData.Core;
using Microsoft.OData.Json;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    public class JsonReaderTests
    {
        [Fact]
        public void DottedNumberShouldBeReadAsDecimal()
        {
            Assert.IsType<Decimal>(this.CreateJsonReader("42.0").ReadPrimitiveValue());
        }

        [Fact]
        public void NonDottedNumberShouldBeReadAsInt()
        {
            Assert.IsType<Int32>(this.CreateJsonReader("42").ReadPrimitiveValue());
        }

        [Fact]
        public void TrueShouldBeReadAsBoolean()
        {
            Assert.IsType<Boolean>(this.CreateJsonReader("true").ReadPrimitiveValue());
        }

        [Fact]
        public void FalseShouldBeReadAsBoolean()
        {
            Assert.IsType<Boolean>(this.CreateJsonReader("false").ReadPrimitiveValue());
        }

        [Fact]
        public void NullShouldBeReadAsNull()
        {
            Assert.Null(this.CreateJsonReader("null").ReadPrimitiveValue());
        }

        [Fact]
        public void QuotedNumberShouldBeReadAsString()
        {
            Assert.IsType<String>(this.CreateJsonReader("\"42\"").ReadPrimitiveValue());
        }

        [Fact]
        public void QuotedISO8601DateTimeShouldBeReadAsString()
        {
            Assert.IsType<String>(this.CreateJsonReader("\"2012-08-14T19:39Z\"").ReadPrimitiveValue());
        }

        [Fact]
        public void QuotedNullShouldBeReadAsString()
        {
            Assert.IsType<String>(this.CreateJsonReader("\"null\"").ReadPrimitiveValue());
        }

        [Fact]
        public void QuotedBooleanValueShouldBeReadAsString()
        {
            Assert.IsType<String>(this.CreateJsonReader("\"true\"").ReadPrimitiveValue());
        }

        [Fact]
        public void QuotedAspNetDateTimeValueShouldBeReadAsStringInJson()
        {
            Assert.IsType<String>(this.CreateJsonReader("\"\\/Date(628318530718)\\/\"").ReadPrimitiveValue());
        }

        [Theory]
        [InlineData("\"abc\u6211xyz\"")]
        [InlineData("\"abcxyz\u6211\"")]
        [InlineData("\"\u6211abcxyz\"")]
        public void EscapeStringShouldBeReadAsString(string value)
        {
            // Arrange & Act
            object actual = this.CreateJsonReader(value).ReadPrimitiveValue();

            // Assert
            Assert.IsType<string>(actual);
        }

        [Fact]
        public void ShouldUseArrayPoolIfSet()
        {
            // Arrange
            TestArrayPool pool = new TestArrayPool();
            Assert.Equal(0, pool.RentCount); // guard
            Assert.Equal(0, pool.ReturnCount); // guard
            IJsonReader reader = new JsonReader(new StringReader("[]"), isIeee754Compatible: false)
            {
                ArrayPool = pool
            };

            // Act
            while (reader.Read())
            { }

            // Assert
            Assert.Equal(JsonNodeType.EndOfInput, reader.NodeType);
            Assert.Equal(1, pool.RentCount);
            Assert.Equal(1, pool.ReturnCount);
        }

        [Theory]
        [InlineData("\"\"", typeof(string))]
        [InlineData("\"foo\"", typeof(string))]
        [InlineData("\"100\"", typeof(string))]
        [InlineData("\"true\"", typeof(string))]
        [InlineData("\"\\/Date(628318530718)\\/\"", typeof(string))]
        [InlineData("\"2012-08-14T19:39Z\"", typeof(string))]
        [InlineData("\"null\"", typeof(string))]
        [InlineData("13", typeof(int))]
        [InlineData("-13", typeof(int))]
        [InlineData("13.0", typeof(decimal))]
        [InlineData("-13.0", typeof(decimal))]
        [InlineData("3.1428571428571428571428571428571", typeof(decimal), false)]
        [InlineData("-3.1428571428571428571428571428571", typeof(decimal), false)]
        [InlineData("3.1428571428571428571428571428571", typeof(double), true)]
        [InlineData("-3.1428571428571428571428571428571", typeof(double), true)]
        [InlineData("6.0221409e+23", typeof(double))]
        [InlineData("-6.0221409e+23", typeof(double))]
        [InlineData("6.0221409E+23", typeof(double))]
        [InlineData("-6.0221409E+23", typeof(double))]
        [InlineData("6.0221409e-23", typeof(double))]
        [InlineData("-6.0221409e-23", typeof(double))]
        [InlineData("6.0221409E-23", typeof(double))]
        [InlineData("-6.0221409E-23", typeof(double))]
        [InlineData("true", typeof(bool))]
        [InlineData("false", typeof(bool))]
        public async Task ReadPrimitiveValue(string payload, Type expectedType, bool isIeee754Compatible = false)
        {
            using (var reader = new JsonReader(new StringReader(payload), isIeee754Compatible: isIeee754Compatible))
            {
                Assert.True(await reader.ReadAsync());
                Assert.IsType(expectedType, await reader.GetValueAsync());
            }
        }

        [Theory]
        [InlineData("{ \"greeting\": \"\\u0048\\u0065\\u006C\\u006C\\u006F\" }", "Hello")]
        [InlineData("{ \"emoji\": \"\\uD83D\\uDE03\" }", "😃")]
        [InlineData("{ \"chinese\": \"\\u6211\\u662F\\u4E2D\\u6587\" }", "我是中文")]
        [InlineData("{ \"symbol\": \"\\u00A9\" }", "©")]
        [InlineData("{ \"currency\": \"\\u20AC\" }", "€")]
        [InlineData("{ \"greek\": \"\\u03A9\" }", "Ω")]
        [InlineData("{ \"cyrillic\": \"\\u0416\" }", "Ж")]
        [InlineData("{ \"arabic\": \"\\u0627\" }", "ا")]
        [InlineData("{ \"hebrew\": \"\\u05D0\" }", "א")]
        [InlineData("{ \"chinese\": \"\\u4E2D\" }", "中")]
        [InlineData("{ \"hiragana\": \"\\u3042\" }", "あ")]
        [InlineData("{ \"math\": \"\\u221E\" }", "∞")]
        [InlineData("{ \"arrow\": \"\\u2192\" }", "→")]
        [InlineData("{ \"box\": \"\\u25A0\" }", "■")]
        [InlineData("{ \"music\": \"\\u266B\" }", "♫")]
        [InlineData("{ \"latin\": \"\\u00E9\" }", "é")]
        [InlineData("{ \"emoji\": \"\\uD83D\\uDE0A\" }", "😊")]
        [InlineData("{ \"rocket\": \"\\uD83D\\uDE80\" }", "🚀")]
        [InlineData("{ \"sentence\": \"\\u0048\\u0065\\u006C\\u006C\\u006F, \\u4E16\\u754C!\" }", "Hello, 世界!")]
        [InlineData("{ \"word\": \"\\u4E16\\u754C\" }", "世界")]
        [InlineData("{ \"word\": \"\\u03A9\\u006D\\u0065\\u0067\\u0061\" }", "Ωmega")]
        [InlineData("{ \"word\": \"\\u0045\\u0073\\u0070\\u0061\\u00F1\\u0061\" }", "España")]
        [InlineData("{ \"word\": \"\\u05E9\\u05DC\\u05D5\\u05DD\" }", "שלום")]
        public async Task ReadUnicodeHexValueAsync(string payload, string expected)
        {
            using (var reader = new JsonReader(new StringReader(payload), isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // Read start of object
                await reader.ReadAsync(); // Read property name - Data
                await reader.ReadAsync(); // Position reader at the beginning of string
                Assert.Equal(expected, await reader.GetValueAsync());
            }
        }

        [Theory]
        [InlineData("{ \"greeting\": \"\\u0048\\u0065\\u006C\\u006C\\u006F\" }", "Hello")]
        [InlineData("{ \"emoji\": \"\\uD83D\\uDE03\" }", "😃")]
        [InlineData("{ \"chinese\": \"\\u6211\\u662F\\u4E2D\\u6587\" }", "我是中文")]
        [InlineData("{ \"symbol\": \"\\u00A9\" }", "©")]
        [InlineData("{ \"currency\": \"\\u20AC\" }", "€")]
        [InlineData("{ \"greek\": \"\\u03A9\" }", "Ω")]
        [InlineData("{ \"cyrillic\": \"\\u0416\" }", "Ж")]
        [InlineData("{ \"arabic\": \"\\u0627\" }", "ا")]
        [InlineData("{ \"hebrew\": \"\\u05D0\" }", "א")]
        [InlineData("{ \"chinese\": \"\\u4E2D\" }", "中")]
        [InlineData("{ \"hiragana\": \"\\u3042\" }", "あ")]
        [InlineData("{ \"math\": \"\\u221E\" }", "∞")]
        [InlineData("{ \"arrow\": \"\\u2192\" }", "→")]
        [InlineData("{ \"box\": \"\\u25A0\" }", "■")]
        [InlineData("{ \"music\": \"\\u266B\" }", "♫")]
        [InlineData("{ \"latin\": \"\\u00E9\" }", "é")]
        [InlineData("{ \"emoji\": \"\\uD83D\\uDE0A\" }", "😊")]
        [InlineData("{ \"rocket\": \"\\uD83D\\uDE80\" }", "🚀")]
        [InlineData("{ \"sentence\": \"\\u0048\\u0065\\u006C\\u006C\\u006F, \\u4E16\\u754C!\" }", "Hello, 世界!")]
        [InlineData("{ \"word\": \"\\u4E16\\u754C\" }", "世界")]
        [InlineData("{ \"word\": \"\\u03A9\\u006D\\u0065\\u0067\\u0061\" }", "Ωmega")]
        [InlineData("{ \"word\": \"\\u0045\\u0073\\u0070\\u0061\\u00F1\\u0061\" }", "España")]
        [InlineData("{ \"word\": \"\\u05E9\\u05DC\\u05D5\\u05DD\" }", "שלום")]
        public void ReadUnicodeHexValue(string payload, string expected)
        {
            using (var reader = new JsonReader(new StringReader(payload), isIeee754Compatible: false))
            {
                reader.Read(); // Read start of object
                reader.Read(); // Read property name - Data
                reader.Read(); // Position reader at the beginning of string
                Assert.Equal(expected, reader.GetValue());
            }
        }

        [Theory]
        [InlineData("{\"Data\":\"The \\r character\"}", "The \r character")]
        [InlineData("{\"Data\":\"The \\n character\"}", "The \n character")]
        [InlineData("{\"Data\":\"The \\t character\"}", "The \t character")]
        [InlineData("{\"Data\":\"The \\b character\"}", "The \b character")]
        [InlineData("{\"Data\":\"The \\f character\"}", "The \f character")]
        [InlineData("{\"Data\":\"The \\\\ character\"}", "The \\ character")]
        [InlineData("{\"Data\":\"The \\\" character\"}", "The \" character")]
        [InlineData("{\"Data\":\"The \\\' character\"}", "The \' character")]
        [InlineData("{\"Data\":\"The \\u6211 character\"}", "The \u6211 character")]
        public async Task ReadPrimitiveValueWithSpecialCharacters(string payload, string expected)
        {
            using (var reader = new JsonReader(new StringReader(payload), isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // Read start of object
                await reader.ReadAsync(); // Read property name - Data
                await reader.ReadAsync(); // Position reader at the beginning of string value
                Assert.Equal(expected, await reader.GetValueAsync());
            }
        }

        [Theory]
        [InlineData("\"simple\"", "simple")]
        [InlineData("\"with spaces\"", "with spaces")]
        [InlineData("\"with \\\"escaped quotes\\\"\"", "with \"escaped quotes\"")]
        [InlineData("\"with \\\\ backslash\"", "with \\ backslash")]
        [InlineData("\"with \\n newline\"", "with \n newline")]
        [InlineData("\"with \\r carriage\"", "with \r carriage")]
        [InlineData("\"with \\t tab\"", "with \t tab")]
        [InlineData("\"with \\b backspace\"", "with \b backspace")]
        [InlineData("\"with \\f formfeed\"", "with \f formfeed")]
        [InlineData("\"unicode \\u0041\"", "unicode A")]
        [InlineData("\"mix \\u0041\\n\\t\\\"\"", "mix A\n\t\"")]
        [InlineData("\"12345\"", "12345")]
        [InlineData("\"true\"", "true")]
        [InlineData("\"false\"", "false")]
        [InlineData("\"null\"", "null")]
        [InlineData("\"\"", "")]
        public void ParseStringPrimitiveValue_Sync(string json, string expected)
        {
            var reader = new JsonReader(new StringReader($"{{\"PropertyName\":{json}}}"), isIeee754Compatible: false);
            reader.Read(); // Start object
            reader.Read(); // Property name
            Assert.Equal("PropertyName", reader.GetValue());

            reader.Read(); // Value
            Assert.Equal(expected, reader.GetValue());
        }

        [Theory]
        [InlineData("\"simple\"", "simple")]
        [InlineData("\"with spaces\"", "with spaces")]
        [InlineData("\"with \\\"escaped quotes\\\"\"", "with \"escaped quotes\"")]
        [InlineData("\"with \\\\ backslash\"", "with \\ backslash")]
        [InlineData("\"with \\n newline\"", "with \n newline")]
        [InlineData("\"with \\r carriage\"", "with \r carriage")]
        [InlineData("\"with \\t tab\"", "with \t tab")]
        [InlineData("\"with \\b backspace\"", "with \b backspace")]
        [InlineData("\"with \\f formfeed\"", "with \f formfeed")]
        [InlineData("\"unicode \\u0041\"", "unicode A")]
        [InlineData("\"mix \\u0041\\n\\t\\\"\"", "mix A\n\t\"")]
        [InlineData("\"12345\"", "12345")]
        [InlineData("\"true\"", "true")]
        [InlineData("\"false\"", "false")]
        [InlineData("\"null\"", "null")]
        [InlineData("\"\"", "")]
        public async Task ParseStringPrimitiveValue_Async(string json, string expected)
        {
            var reader = new JsonReader(new StringReader($"{{\"PropertyName\":{json}}}"), isIeee754Compatible: false);
            await reader.ReadAsync(); // Start object
            await reader.ReadAsync(); // Property name
            Assert.Equal("PropertyName", await reader.GetValueAsync());

            await reader.ReadAsync(); // Value
            Assert.Equal(expected, await reader.GetValueAsync());
        }

        [Theory]
        [InlineData("42", typeof(int), 42)]
        [InlineData("-42", typeof(int), -42)]
        [InlineData("3.14e2", typeof(double), 314.0)]
        [InlineData("true", typeof(bool), true)]
        [InlineData("false", typeof(bool), false)]
        public void ParsePrimitiveValue_Sync(string input, Type expectedType, object expectedValue)
        {
            var reader = new JsonReader(new StringReader($"{{\"Number\":{input}, \"Name\": \"John Doe\"}}"), isIeee754Compatible: false);
            reader.Read(); // Start object
            reader.Read(); // Property name
            Assert.Equal("Number", reader.GetValue());

            reader.Read(); // Value
            var value = reader.GetValue();

            Assert.IsType(expectedType, value);
            Assert.Equal(expectedValue, value);

            reader.Read(); // Property name
            reader.Read(); // Value
            Assert.Equal("John Doe", reader.GetValue());
        }

        [Theory]
        [InlineData("42", typeof(int), 42)]
        [InlineData("-42", typeof(int), -42)]
        [InlineData("3.14e2", typeof(double), 314.0)]
        [InlineData("true", typeof(bool), true)]
        [InlineData("false", typeof(bool), false)]
        public async Task ParsePrimitiveValue_Async(string input, Type expectedType, object expectedValue)
        {
            var reader = new JsonReader(new StringReader($"{{\"Number\":{input}, \"Name\": \"John Doe\"}}"), isIeee754Compatible: false);
            await reader.ReadAsync(); // Start object
            await reader.ReadAsync(); // Property name
            Assert.Equal("Number", await reader.GetValueAsync());

            await reader.ReadAsync(); // Value
            var value = await reader.GetValueAsync();

            Assert.IsType(expectedType, value);
            Assert.Equal(expectedValue, value);

            await reader.ReadAsync(); // Property name
            await reader.ReadAsync(); // Value
            Assert.Equal("John Doe", await reader.GetValueAsync());
        }

        [Fact]
        public async Task ReadNullValue()
        {
            using (var reader = new JsonReader(new StringReader("null"), isIeee754Compatible: false))
            {
                Assert.True(await reader.ReadAsync());
                Assert.Null(await reader.GetValueAsync());
            }
        }

        [Theory]
        [InlineData( // Property names in double quotes
            "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
            "\"Id\":1," +
            "\"Name\":\"Sue\"," +
            "\"Orders\":[{\"Id\":1,\"Amount\":100},{\"Id\":2,\"Amount\":130}]," +
            "\"Active\":true," +
            "\"CustomerSince\":null}")]
        [InlineData( // Superfluous spaces
            " { \"@odata.context\" : \"http://tempuri.org/$metadata#Customers/$entity\" ," +
            " \"Id\" : 1 ," +
            " \"Name\" : \"Sue\" ," +
            " \"Orders\" : [ { \"Id\" : 1 , \"Amount\" : 100 } , { \"Id\" : 2 , \"Amount\" : 130 } ]," +
            " \"Active\" : true ," +
            " \"CustomerSince\" : null } ")]
        [InlineData( // Property names in single quotes
            "{'@odata.context':'http://tempuri.org/$metadata#Customers/$entity'," +
            "'Id':1," +
            "'Name':'Sue'," +
            "'Orders':[{'Id':1,'Amount':100},{'Id':2,'Amount':130}]," +
            "'Active':true," +
            "'CustomerSince':null}")]
        [InlineData( // Property names not in quotes
            "{@odata.context:\"http://tempuri.org/$metadata#Customers/$entity\"," +
            "Id:1," +
            "Name:\"Sue\"," +
            "Orders:[{Id:1,Amount:100},{Id:2,Amount:130}]," +
            "Active:true," +
            "CustomerSince:null}")]
        public async Task ReadValidPayload(string payload)
        {
            using (var reader = new JsonReader(new StringReader(payload), isIeee754Compatible: false))
            {
                Assert.True(await reader.ReadAsync()); // Read start of object

                Assert.True(await reader.ReadAsync()); // Read property name: @odata.context
                Assert.Equal("@odata.context", await reader.GetValueAsync());
                Assert.True(await reader.ReadAsync()); // Read property value
                Assert.Equal("http://tempuri.org/$metadata#Customers/$entity", await reader.GetValueAsync());
                Assert.True(await reader.ReadAsync()); // Read property name: Id
                Assert.Equal("Id", await reader.GetValueAsync());
                Assert.True(await reader.ReadAsync()); // Read property value
                Assert.Equal(1, await reader.GetValueAsync());
                Assert.True(await reader.ReadAsync()); // Read property name: Name
                Assert.Equal("Name", await reader.GetValueAsync());
                Assert.True(await reader.ReadAsync()); // Position reader at the beginning of string value
                Assert.Equal("Sue", await reader.GetValueAsync());
                Assert.True(await reader.ReadAsync()); // Read property name: Orders
                Assert.Equal("Orders", await reader.GetValueAsync());

                Assert.True(await reader.ReadAsync()); // Read start of array

                Assert.True(await reader.ReadAsync()); // Read start of 1st object in array
                Assert.True(await reader.ReadAsync()); // Read property name: Id
                Assert.Equal("Id", await reader.GetValueAsync());
                Assert.True(await reader.ReadAsync()); // Read property value
                Assert.Equal(1, await reader.GetValueAsync());
                Assert.True(await reader.ReadAsync()); // Read property name: Amount
                Assert.Equal("Amount", await reader.GetValueAsync());
                Assert.True(await reader.ReadAsync()); // Read property value
                Assert.Equal(100, await reader.GetValueAsync());
                Assert.True(await reader.ReadAsync()); // Read end of 1st object in array

                Assert.True(await reader.ReadAsync()); // Read start of 2nd object in array
                Assert.True(await reader.ReadAsync()); // Read property name: Id
                Assert.Equal("Id", await reader.GetValueAsync());
                Assert.True(await reader.ReadAsync()); // Read property value
                Assert.Equal(2, await reader.GetValueAsync());
                Assert.True(await reader.ReadAsync()); // Read property name: Amount
                Assert.Equal("Amount", await reader.GetValueAsync());
                Assert.True(await reader.ReadAsync()); // Read property value
                Assert.Equal(130, await reader.GetValueAsync());
                Assert.True(await reader.ReadAsync()); // Read end of 2nd object in array

                Assert.True(await reader.ReadAsync()); // Read end of array

                Assert.True(await reader.ReadAsync()); // Read property name: Active
                Assert.Equal("Active", await reader.GetValueAsync());
                Assert.True(await reader.ReadAsync()); // Read property value
                Assert.Equal(true, await reader.GetValueAsync());
                Assert.True(await reader.ReadAsync()); // Read property name: CustomerSince
                Assert.Equal("CustomerSince", await reader.GetValueAsync());
                Assert.True(await reader.ReadAsync()); // Read property value
                Assert.Null(await reader.GetValueAsync());

                Assert.True(await reader.ReadAsync()); // Read end of object
            }
        }

        [Theory]
        [InlineData("")]
        [InlineData(",")]
        public async Task ReadInvalidPayload(string payload)
        {
            using (var reader = new JsonReader(new StringReader(payload), isIeee754Compatible: false))
            {
                Assert.False(await reader.ReadAsync()); // Try to read
            }
        }

        [Fact]
        public async Task ReadBinaryValue()
        {
            using (var reader = new JsonReader(new StringReader("{\"Binary\":\"AQIDBAUGBwgJAA==\",\"Other\":\"\"}"), isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // Read start of object
                await reader.ReadAsync(); // Read property name: Binary
                await reader.ReadAsync(); // Position reader at the beginning of binary value
                using (var binaryStream = await reader.CreateReadStreamAsync())
                {
                    var maxLength = 10;
                    var buffer = new byte[maxLength];

                    var bytesRead = await binaryStream.ReadAsync(buffer, 0, maxLength);
                    Assert.Equal(bytesRead, maxLength);
                    Assert.Equal(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }, buffer);
                }

                // The reader will be at the next node after reading a stream - ReadCharsAsync
                Assert.Equal("Other", await reader.GetValueAsync());
                await reader.ReadAsync(); // Read property value
                Assert.Equal("", await reader.GetValueAsync());
            }
        }

        [Fact]
        public async Task ReadNullBinaryValue()
        {
            using (var reader = new JsonReader(new StringReader("{\"Binary\":null,\"Other\":\"\"}"), isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // Read start of object
                await reader.ReadAsync(); // Read property name: Binary
                await reader.ReadAsync(); // Position reader at the beginning of binary value
                using (var binaryStream = await reader.CreateReadStreamAsync())
                {
                    var maxLength = 0;
                    var buffer = new byte[maxLength];

                    var bytesRead = await binaryStream.ReadAsync(buffer, 0, maxLength);
                    Assert.Equal(bytesRead, maxLength);
                    Assert.Equal(new byte[maxLength], buffer);
                }

                // The reader will be at the next node after reading a stream - ReadCharsAsync
                Assert.Equal("Other", await reader.GetValueAsync());
                await reader.ReadAsync(); // Try read property value
                Assert.Equal("", await reader.GetValueAsync());
            }
        }

        [Fact]
        public async Task ReadStringValue()
        {
            var pangram = "The quick brown fox jumps over the lazy dog.";
            using (var reader = new JsonReader(
                new StringReader(string.Format("{{\"Text\":\"{0}\",\"Other\":\"\"}}", pangram)),
                isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // Read start of object
                await reader.ReadAsync(); // Read property name: Text
                await reader.ReadAsync(); // Position reader at the beginning of string value
                using (var textReader = await reader.CreateTextReaderAsync())
                {
                    var strLength = pangram.Length;
                    var maxLength = strLength + 1; // + 1 to cater for character for closing stream
                    var chars = new char[maxLength];

                    // The reader will be at the next node after reading a stream - ReadCharsAsync
                    var charsRead = await textReader.ReadAsync(chars, 0, maxLength);
                    Assert.Equal(charsRead, strLength);
                    Assert.Equal(pangram, new string(chars, 0, charsRead));
                }

                Assert.Equal("Other", await reader.GetValueAsync());
                await reader.ReadAsync(); // Read property value
                Assert.Equal("", await reader.GetValueAsync());
            }
        }

        [Fact]
        public async Task ReadNullStringValue()
        {
            using (var reader = new JsonReader(new StringReader("{\"Text\":null,\"Other\":\"\"}"), isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // Read start of object
                await reader.ReadAsync(); // Read property name: Text
                await reader.ReadAsync(); // Position reader at the beginning of string value
                using (var textReader = await reader.CreateTextReaderAsync())
                {
                    var maxLength = 0;
                    var chars = new char[maxLength];

                    // The reader will be at the next node after reading a stream - ReadCharsAsync
                    var charsRead = await textReader.ReadAsync(chars, 0, maxLength);
                    Assert.Equal(charsRead, maxLength);
                    Assert.Equal("", new string(chars, 0, charsRead));
                }

                Assert.Equal("Other", await reader.GetValueAsync());
                await reader.ReadAsync(); // Try read property value
                Assert.Equal("", await reader.GetValueAsync());
            }
        }

        [Theory]
        [InlineData("{\"Data\":\"The \\r character\"}", "The \r character")]
        [InlineData("{\"Data\":\"The \\n character\"}", "The \n character")]
        [InlineData("{\"Data\":\"The \\t character\"}", "The \t character")]
        [InlineData("{\"Data\":\"The \\b character\"}", "The \b character")]
        [InlineData("{\"Data\":\"The \\f character\"}", "The \f character")]
        [InlineData("{\"Data\":\"The \\\\ character\"}", "The \\ character")]
        [InlineData("{\"Data\":\"The \\\" character\"}", "The \" character")]
        [InlineData("{\"Data\":\"The \\\' character\"}", "The \' character")]
        [InlineData("{\"Data\":\"The \\u6211 character\"}", "The \u6211 character")]
        public async Task ReadStringValueWithSpecialCharacters(string payload, string expected)
        {
            using (var reader = new JsonReader(new StringReader(payload), isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // Read start of object
                await reader.ReadAsync(); // Read property name - Data
                await reader.ReadAsync(); // Position reader at the beginning of string value
                using (var textReader = await reader.CreateTextReaderAsync())
                {
                    var maxLength = 32;
                    var chars = new char[maxLength];

                    var charsRead = await textReader.ReadAsync(chars, 0, maxLength);
                    Assert.Equal(expected, new string(chars, 0, charsRead));
                }
            }
        }

        [Theory]
        [InlineData("13", false)]
        [InlineData("null", true)]
        [InlineData("\"The quick brown fox jumps over the lazy dog.\"", true)]
        public async Task CanStreamAsync_ReturnsExpectedResult(string payload, bool expected)
        {
            using (var reader = await CreateJsonReaderAsync(string.Format("{{\"Data\":{0}}}", payload)))
            {
                Assert.Equal(expected, await reader.CanStreamAsync());
            }
        }

        [Fact]
        public async Task ReadStringValueThrowsExceptionForUnexpectedEndOfString()
        {
            var pangram = "The quick brown fox jumps over the lazy dog.";
            using (var reader = new JsonReader(
                new StringReader(string.Format("{{\"Data\":\"{0}", pangram)), isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // Read start of object
                await reader.ReadAsync(); // Read property name - Data
                await reader.ReadAsync(); // Position reader at the beginning of string value

                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    using (var textReader = await reader.CreateTextReaderAsync())
                    {
                        // Length less than the string
                        var maxLength = pangram.Length + 1;
                        var chars = new char[maxLength];

                        await textReader.ReadAsync(chars, 0, maxLength);
                    }
                });
            }
        }

        [Fact]
        public async Task ReadStringPrimitiveValueExceedingInitialCharacterBufferSize()
        {
            var initialCharacterBufferSize = ((4 * 1024) / 2) - 8; // Based on JsonReader
            var pangram = "The quick brown dog jumps over the lazy dog. ";
            var count = (initialCharacterBufferSize / pangram.Length) + 1;
            var stringValue = string.Concat(Enumerable.Repeat(pangram, count));

            using (var reader = new JsonReader(new StringReader(string.Format("{{\"Value\":\"{0}\"}}", stringValue)), isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // Read start of object
                await reader.ReadAsync(); // Read property name: Value
                await reader.ReadAsync(); // Position reader at the beginning of string value
                Assert.Equal(stringValue, await reader.GetValueAsync());
            }
        }

        [Fact]
        public async Task ReadMultipleStringPrimitiveValuesWithSpecialCharacters()
        {
            using (var reader = new JsonReader(
                new StringReader("{\"Prop1\":\"The \\r character\",\"Prop2\":\"The \\n character\"}"),
                isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // Read start of object
                await reader.ReadAsync(); // Read property name: Prop1
                await reader.ReadAsync(); // Position reader at the beginning of string value
                Assert.Equal("The \r character", await reader.GetValueAsync());
                await reader.ReadAsync(); // Read property name: Prop2
                await reader.ReadAsync(); // Position reader at the beginning of string value
                Assert.Equal("The \n character", await reader.GetValueAsync());
            }
        }

        [Fact]
        public async Task UnexpectedCommaAtRootThrowsException()
        {
            using (var reader = new JsonReader(new StringReader(",{}"), isIeee754Compatible: false))
            {
                var exception = await Assert.ThrowsAsync<ODataException>(async () => await reader.ReadAsync());
                Assert.Equal(Error.Format(SRResources.JsonReader_UnexpectedComma, "Root"), exception.Message);
            }
        }

        [Fact]
        public async Task UnexpectedCommaAtObjectStartThrowsException()
        {
            using (var reader = new JsonReader(new StringReader("{,}"), isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // Read start of object
                var exception = await Assert.ThrowsAsync<ODataException>(async () => await reader.ReadAsync());
                Assert.Equal(Error.Format(SRResources.JsonReader_UnexpectedComma, "Object"), exception.Message);
            }
        }

        [Fact]
        public async Task UnexpectedCommaAtObjectEndThrowsException()
        {
            using (var reader = new JsonReader(new StringReader("{\"Id\":1,}"), isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // Read start of object
                await reader.ReadAsync(); // Read property name: Id
                await reader.ReadAsync(); // Read property value
                var exception = await Assert.ThrowsAsync<ODataException>(async () => await reader.ReadAsync());
                Assert.Equal(Error.Format(SRResources.JsonReader_UnexpectedComma, "Object"), exception.Message);
            }
        }

        [Fact]
        public async Task MissingCommaInObjectThrowsException()
        {
            using (var reader = new JsonReader(new StringReader("{\"Id\":1]"), isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // Read start of object
                await reader.ReadAsync(); // Read property name: Id
                await reader.ReadAsync(); // Read property value
                var exception = await Assert.ThrowsAsync<ODataException>(async () => await reader.ReadAsync());
                Assert.Equal(Error.Format(SRResources.JsonReader_MissingComma, "Object"), exception.Message);
            }
        }

        [Fact]
        public async Task EmptyPropertyNameThrowsException()
        {
            using (var reader = new JsonReader(new StringReader("{\"\":1}"), isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // Read start of object
                var exception = await Assert.ThrowsAsync<ODataException>(async () => await reader.ReadAsync()); // Try to read property name
                Assert.Equal(Error.Format(SRResources.JsonReader_InvalidPropertyNameOrUnexpectedComma, string.Empty), exception.Message);
            }
        }

        [Fact]
        public async Task MissingColonAfterPropertyNameThrowsException()
        {
            using (var reader = new JsonReader(new StringReader("{\"Id\"}"), isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // Read start of object
                var exception = await Assert.ThrowsAsync<ODataException>(async () => await reader.ReadAsync());
                Assert.Equal(Error.Format(SRResources.JsonReader_MissingColon, "Id"), exception.Message);
            }
        }

        [Fact]
        public async Task MissingValueAfterPropertyNameThrowsException()
        {
            using (var reader = new JsonReader(new StringReader("{\"Id\":}"), isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // Read start of object
                await reader.ReadAsync(); // Read property name: Id
                var exception = await Assert.ThrowsAsync<ODataException>(async () => await reader.ReadAsync());
                Assert.Equal(SRResources.JsonReader_UnrecognizedToken, exception.Message);
            }
        }

        [Fact]
        public async Task UnexpectedCommaAfterPropertyNameThrowsException()
        {
            using (var reader = new JsonReader(new StringReader("{\"Id\":,}"), isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // Read start of object
                await reader.ReadAsync(); // Read property name: Id
                var exception = await Assert.ThrowsAsync<ODataException>(async () => await reader.ReadAsync());
                Assert.Equal(Error.Format(SRResources.JsonReader_UnexpectedComma, "Property"), exception.Message);
            }
        }

        [Fact]
        public async Task MultipleTopLevelValuesThrowsException()
        {
            using (var reader = new JsonReader(new StringReader("{\"Id\":1}{"), isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // Read start of object
                await reader.ReadAsync(); // Read property name: Id
                await reader.ReadAsync(); // Read property value
                await reader.ReadAsync(); // Read end of object
                var exception = await Assert.ThrowsAsync<ODataException>(async () => await reader.ReadAsync());
                Assert.Equal(SRResources.JsonReader_MultipleTopLevelValues, exception.Message);
            }
        }

        [Fact]
        public async Task UnexpectedCommaInArrayThrowsException()
        {
            using (var reader = new JsonReader(new StringReader("{\"Orders\":[,]}"), isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // Read start of object
                await reader.ReadAsync(); // Read property name: Orders
                await reader.ReadAsync(); // Read start of array
                var exception = await Assert.ThrowsAsync<ODataException>(async () => await reader.ReadAsync());
                Assert.Equal(Error.Format(SRResources.JsonReader_UnexpectedComma, "Array"), exception.Message);
            }
        }

        [Fact]
        public async Task UnexpectedCommaBeforeArrayEndThrowsException2()
        {
            using (var reader = new JsonReader(new StringReader("{\"Orders\":[{\"Amount\":100},]}"), isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // Read start of object
                await reader.ReadAsync(); // Read property name: Orders
                await reader.ReadAsync(); // Read start of array
                await reader.ReadAsync(); // Read start of 1st array object
                await reader.ReadAsync(); // Read property name: Amount
                await reader.ReadAsync(); // Read property value
                await reader.ReadAsync(); // Read end of 1st array object
                var exception = await Assert.ThrowsAsync<ODataException>(async () => await reader.ReadAsync());
                Assert.Equal(Error.Format(SRResources.JsonReader_UnexpectedComma, "Array"), exception.Message);
            }
        }

        [Fact]
        public async Task MissingCommaInArrayThrowsException()
        {
            using (var reader = new JsonReader(new StringReader("{\"Orders\":[{\"Amount\":100}{\"Amount\":130}]}"), isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // Read start of object
                await reader.ReadAsync(); // Read property name: Orders
                await reader.ReadAsync(); // Read start of array
                await reader.ReadAsync(); // Read start of 1st array object
                await reader.ReadAsync(); // Read property name: Amount
                await reader.ReadAsync(); // Read property value
                await reader.ReadAsync(); // Read end of 1st array object
                var exception = await Assert.ThrowsAsync<ODataException>(async () => await reader.ReadAsync());
                Assert.Equal(Error.Format(SRResources.JsonReader_MissingComma, "Array"), exception.Message);
            }
        }

        [Fact]
        public async Task UnexpectedNullTokenThrowsException()
        {
            using (var reader = new JsonReader(new StringReader("nil"), isIeee754Compatible: false))
            {
                await reader.ReadAsync();
                var exception = await Assert.ThrowsAsync<ODataException>(async () => await reader.GetValueAsync());
                Assert.Equal(Error.Format(SRResources.JsonReader_UnexpectedToken, "nil"), exception.Message);
            }
        }

        [Theory]
        [InlineData("{\"Data\":\"The \\ r character\"}", "\\ ")]
        [InlineData("{\"Data\":\"The \\", "\\")]
        [InlineData("{\"Data\":\"The \\u621", "\\uXXXX")]
        [InlineData("{\"Data\":\"The \\u62 character\"}", "\\u62 c")]
        public async Task UnrecognizedEscapeSequenceThrowsException(string payload, string expected)
        {
            using (var reader = new JsonReader(new StringReader(payload), isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // Read start of object
                await reader.ReadAsync(); // Read property name - Data
                await reader.ReadAsync(); // Position reader at the beginning of string value
                var exception = await Assert.ThrowsAsync<ODataException>(async () => await reader.GetValueAsync());
                Assert.Equal(Error.Format(SRResources.JsonReader_UnrecognizedEscapeSequence, expected), exception.Message);
            }
        }

        [Theory]
        [InlineData("{\"Data\":\"The \\ r character\"}", "\\ ")]
        [InlineData("{\"Data\":\"The \\", "\\uXXXX")]
        [InlineData("{\"Data\":\"The \\u621", "\\uXXXX")]
        [InlineData("{\"Data\":\"The \\u62 character\"}", "\\u62 c")]
        public async Task ReadStringValueWithUnrecognizedEscapeSequenceThrowsException(string payload, string expected)
        {
            using (var reader = new JsonReader(new StringReader(payload), isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // Read start of object
                await reader.ReadAsync(); // Read property name - Data
                await reader.ReadAsync(); // Position reader at the beginning of string value
                using (var textReader = await reader.CreateTextReaderAsync())
                {
                    var maxLength = 32;
                    var chars = new char[maxLength];

                    var exception = await Assert.ThrowsAsync<ODataException>(async () => await textReader.ReadAsync(chars, 0, maxLength));
                    Assert.Equal(Error.Format(SRResources.JsonReader_UnrecognizedEscapeSequence, expected), exception.Message);
                }
            }
        }

        [Fact]
        public async Task CreateReadStreamAsyncThrowsExceptionForReaderNotInStreamState()
        {
            using (var reader = new JsonReader(new StringReader("{\"Binary\":\"AQIDBAUGBwgJAA==\",\"Other\":\"\"}"), isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // Read start of object
                await reader.ReadAsync(); // Read property name: Binary
                                          // await reader.ReadAsync(); // Position reader at the beginning of binary value
                var exception = await Assert.ThrowsAsync<ODataException>(async () => await reader.CreateReadStreamAsync());
                Assert.Equal(SRResources.JsonReader_CannotCreateReadStream, exception.Message);
            }
        }

        [Fact]
        public async Task CreateTextReaderAsyncThrowsExceptionForReaderNotInStreamState()
        {
            using (var reader = new JsonReader(
                new StringReader("{\"Text\":\"The quick brown fox jumps over the lazy dog.\",\"Other\":\"\"}"), isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // Read start of object
                await reader.ReadAsync(); // Read property name: Text
                                          // await reader.ReadAsync(); // Position reader at the beginning of string value
                var exception = await Assert.ThrowsAsync<ODataException>(async () => await reader.CreateTextReaderAsync());
                Assert.Equal(SRResources.JsonReader_CannotCreateTextReader, exception.Message);
            }
        }

        [Fact]
        public async Task ReadAsyncThrowsExceptionForReaderInStreamState()
        {
            using (var reader = new JsonReader(new StringReader("{\"Binary\":\"AQIDBAUGBwgJAA==\",\"Other\":\"\"}"), isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // Read start of object
                await reader.ReadAsync(); // Read property name: Binary
                await reader.ReadAsync(); // Position reader at the beginning of binary value
                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    using (var binaryStream = await reader.CreateReadStreamAsync())
                    {
                        await reader.ReadAsync(); // Try to read when in stream state
                    }
                });

                Assert.Equal(SRResources.JsonReader_CannotCallReadInStreamState, exception.Message);
            }
        }

        [Fact]
        public async Task GetValueAsyncThrowsExceptionForReaderInStreamState()
        {
            using (var reader = new JsonReader(new StringReader("{\"Binary\":\"AQIDBAUGBwgJAA==\",\"Other\":\"\"}"), isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // Read start of object
                await reader.ReadAsync(); // Read property name: Binary
                await reader.ReadAsync(); // Position reader at the beginning of binary value
                var exception = await Assert.ThrowsAsync<ODataException>(async () =>
                {
                    using (var binaryStream = await reader.CreateReadStreamAsync())
                    {
                        await reader.GetValueAsync(); // Try to get value when in stream state
                    }
                });

                Assert.Equal(SRResources.JsonReader_CannotAccessValueInStreamState, exception.Message);
            }
        }

        [Fact]
        public async Task UnexpectedEndOfStringThrowsException()
        {
            using (var reader = new JsonReader(
                new StringReader("{\"Text\":\"The quick brown fox jumps over the lazy dog.}"), isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // Read start of object
                await reader.ReadAsync(); // Read property name: Text
                await reader.ReadAsync(); // Position reader at the beginning of string value
                var exception = await Assert.ThrowsAsync<ODataException>(async () => await reader.GetValueAsync());
                Assert.Equal(SRResources.JsonReader_UnexpectedEndOfString, exception.Message);
            }
        }

        [Fact]
        public async Task InvalidBooleanPrimitiveValueThrowsException()
        {
            using (var reader = new JsonReader(new StringReader("{\"Value\":tRue}"), isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // Read start of object
                await reader.ReadAsync(); // Read property name: Value
                                          // Try to read boolean value
                var exception = await Assert.ThrowsAsync<ODataException>(async () => await reader.ReadAsync());
                Assert.Equal(Error.Format(SRResources.JsonReader_UnexpectedToken, "tRue"), exception.Message);
            }
        }

        [Fact]
        public async Task InvalidNumericPrimitiveValueThrowsException()
        {
            using (var reader = new JsonReader(new StringReader("{\"Value\":6.0221409-e23}"), isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // Read start of object
                await reader.ReadAsync(); // Read property name: Value
                                          // Try to read numeric value
                var exception = await Assert.ThrowsAsync<ODataException>(async () => await reader.ReadAsync());
                Assert.Equal(Error.Format(SRResources.JsonReader_InvalidNumberFormat, "6.0221409-e23"), exception.Message);
            }
        }

        [Theory]
        [InlineData("\"\"", typeof(string))]
        [InlineData("\"foo\"", typeof(string))]
        [InlineData("\"100\"", typeof(string))]
        [InlineData("\"true\"", typeof(string))]
        [InlineData("\"\\/Date(628318530718)\\/\"", typeof(string))]
        [InlineData("\"2012-08-14T19:39Z\"", typeof(string))]
        [InlineData("\"null\"", typeof(string))]
        [InlineData("13", typeof(int))]
        [InlineData("-13", typeof(int))]
        [InlineData("13.0", typeof(decimal))]
        [InlineData("-13.0", typeof(decimal))]
        [InlineData("3.1428571428571428571428571428571", typeof(decimal), false)]
        [InlineData("-3.1428571428571428571428571428571", typeof(decimal), false)]
        [InlineData("3.1428571428571428571428571428571", typeof(double), true)]
        [InlineData("-3.1428571428571428571428571428571", typeof(double), true)]
        [InlineData("6.0221409e+23", typeof(double))]
        [InlineData("-6.0221409e+23", typeof(double))]
        [InlineData("6.0221409E+23", typeof(double))]
        [InlineData("-6.0221409E+23", typeof(double))]
        [InlineData("6.0221409e-23", typeof(double))]
        [InlineData("-6.0221409e-23", typeof(double))]
        [InlineData("6.0221409E-23", typeof(double))]
        [InlineData("-6.0221409E-23", typeof(double))]
        [InlineData("true", typeof(bool))]
        [InlineData("false", typeof(bool))]
        public async Task ReadPrimitiveValueAsync(string payload, Type expectedType, bool isIeee754Compatible = false)
        {
            using (var reader = await CreateJsonReaderAsync($"{{\"Data\":{payload}}}", isIeee754Compatible))
            {
                Assert.Equal(JsonNodeType.PrimitiveValue, reader.NodeType);
                Assert.IsType(expectedType, await reader.ReadPrimitiveValueAsync());
                await reader.ReadEndObjectAsync();
            }
        }

        [Fact]
        public async Task ReadPrimitiveArrayAsync()
        {
            using (var reader = await CreateJsonReaderAsync("{\"Colors\":[\"Black\",\"White\"]}"))
            {
                Assert.Equal(JsonNodeType.StartArray, reader.NodeType);
                await reader.ReadStartArrayAsync();
                Assert.Equal(JsonNodeType.PrimitiveValue, reader.NodeType);
                Assert.Equal("Black", await reader.ReadPrimitiveValueAsync());
                Assert.Equal("White", await reader.ReadPrimitiveValueAsync());
                Assert.Equal(JsonNodeType.EndArray, reader.NodeType);
                await reader.ReadEndArrayAsync();
                await reader.ReadEndObjectAsync();
            }
        }

        [Theory]
        [InlineData("\"The quick brown fox jumps over the lazy dog.\"", "The quick brown fox jumps over the lazy dog.")]
        [InlineData("null", null)]
        public async Task ReadStringValueAsync(string data, string expected)
        {
            using (var reader = await CreateJsonReaderAsync($"{{\"Data\":{data}}}"))
            {
                Assert.Equal(JsonNodeType.PrimitiveValue, reader.NodeType);
                Assert.Equal(expected, await reader.ReadStringValueAsync());
            }
        }

        [Theory]
        [InlineData("\"The quick brown fox jumps over the lazy dog.\"", "The quick brown fox jumps over the lazy dog.")]
        [InlineData("null", null)]
        public async Task ReadStringPropertyValueAsync(string data, string expected)
        {
            using (var reader = await CreateJsonReaderAsync($"{{\"Data\":{data}}}"))
            {
                Assert.Equal(JsonNodeType.PrimitiveValue, reader.NodeType);
                Assert.Equal(expected, await reader.ReadStringValueAsync("Data"));
            }
        }

        [Fact]
        public async Task ReadUriValueAsync()
        {
            using (var reader = await CreateJsonReaderAsync(
                $"{{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"}}",
                isIeee754Compatible: false))
            {
                Assert.Equal(JsonNodeType.PrimitiveValue, reader.NodeType);
                Assert.Equal(new Uri("http://tempuri.org/$metadata#Customers/$entity"), await reader.ReadUriValueAsync());
            }
        }

        public static IEnumerable<object[]> GetReadDoubleValueTestData()
        {
            yield return new object[] { 13, 13D };
            yield return new object[] { 4.2e199, 4.2e199 };
            yield return new object[] { 2000.5M, 2000.5D };
        }

        [Theory]
        [MemberData(nameof(GetReadDoubleValueTestData))]
        public async Task ReadDoubleValueAsync(object data, double expected)
        {
            using (var reader = await CreateJsonReaderAsync($"{{\"Data\":{data}}}"))
            {
                Assert.Equal(JsonNodeType.PrimitiveValue, reader.NodeType);
                Assert.Equal(expected, await reader.ReadDoubleValueAsync());
            }
        }

        [Fact]
        public async Task ReadUntypedValueAsync()
        {
            using (var reader = await CreateJsonReaderAsync(
                $"{{\"Products\":[{{\"Id\":1,\"Name\":\"Pencil\"}},{{\"Id\":2,\"Name\":null}}]}}"))
            {
                var odataValue = await reader.ReadAsUntypedOrNullValueAsync();
                var untypedValue = Assert.IsType<ODataUntypedValue>(odataValue);
                Assert.Equal("[{\"Id\":1,\"Name\":\"Pencil\"},{\"Id\":2,\"Name\":null}]", untypedValue.RawValue);
            }
        }

        [Fact]
        public async Task ReadODataPrimitiveValueAsync()
        {
            using (var reader = await CreateJsonReaderAsync($"{{\"Data\":\"foo\"}}"))
            {
                var odataValue = await reader.ReadODataValueAsync();
                var primitiveValue = Assert.IsType<ODataPrimitiveValue>(odataValue);
                Assert.Equal("foo", primitiveValue.Value);
            }
        }

        [Fact]
        public async Task ReadNullValueAsync()
        {
            using (var reader = await CreateJsonReaderAsync($"{{\"Data\":null}}"))
            {
                var odataValue = await reader.ReadODataValueAsync();
                Assert.IsType<ODataNullValue>(odataValue);
            }
        }

        [Fact]
        public async Task ReadODataResourceValueAsync()
        {
            using (var reader = await CreateJsonReaderAsync($"{{\"Customer\":{{\"Id\":1,\"Name\":\"Sue\"}}}}"))
            {
                var odataValue = await reader.ReadODataValueAsync();
                var resourceValue = Assert.IsType<ODataResourceValue>(odataValue);
                Assert.Equal(2, resourceValue.Properties.Count());
                var prop1 = resourceValue.Properties.First();
                var prop2 = resourceValue.Properties.Last();
                Assert.Equal("Id", prop1.Name);
                Assert.Equal(1, prop1.Value);
                Assert.Equal("Name", prop2.Name);
                Assert.Equal("Sue", prop2.Value);
            }
        }

        [Fact]
        public async Task ReadODataCollectionValueAsync()
        {
            using (var reader = await CreateJsonReaderAsync(
                $"{{\"Orders\":[{{\"Id\":1,\"Amount\":65}},{{\"Id\":2,\"Amount\":80}}]}}"))
            {
                var odataValue = await reader.ReadODataValueAsync();
                var collectionValue = Assert.IsType<ODataCollectionValue>(odataValue);
                Assert.Equal(2, collectionValue.Items.Count());
                var resourceValue1 = Assert.IsType<ODataResourceValue>(collectionValue.Items.First());
                var resourceValue2 = Assert.IsType<ODataResourceValue>(collectionValue.Items.Last());

                var prop11 = resourceValue1.Properties.First();
                var prop12 = resourceValue1.Properties.Last();
                Assert.Equal("Id", prop11.Name);
                Assert.Equal(1, prop11.Value);
                Assert.Equal("Amount", prop12.Name);
                Assert.Equal(65, prop12.Value);

                var prop21 = resourceValue2.Properties.First();
                var prop22 = resourceValue2.Properties.Last();
                Assert.Equal("Id", prop21.Name);
                Assert.Equal(2, prop21.Value);
                Assert.Equal("Amount", prop22.Name);
                Assert.Equal(80, prop22.Value);
            }
        }

        [Fact]
        public async Task SkipValueAsync()
        {
            using (var reader = await CreateJsonReaderAsync(
                $"{{\"Products\":[{{\"Id\":1,\"Name\":\"Pencil\"}},{{\"Id\":2,\"Name\":null}}]}}",
                isIeee754Compatible: false))
            {
                await reader.SkipValueAsync();
                Assert.Equal(JsonNodeType.EndObject, reader.NodeType);
            }
        }

        [Fact]
        public async Task ReadStringValueAsyncThrowsExceptionForNonStringValue()
        {
            using (var reader = await CreateJsonReaderAsync($"{{\"Data\":13}}"))
            {
                Assert.True(reader.IsOnValueNode());
                var exception = await Assert.ThrowsAsync<ODataException>(() => reader.ReadStringValueAsync());
                Assert.Equal(Error.Format(SRResources.JsonReaderExtensions_CannotReadValueAsString, 13), exception.Message);
            }
        }

        [Fact]
        public async Task ReadStringPropertyValueAsyncThrowsExceptionForNonStringValue()
        {
            using (var reader = await CreateJsonReaderAsync($"{{\"Data\":13}}"))
            {
                Assert.True(reader.IsOnValueNode());
                var exception = await Assert.ThrowsAsync<ODataException>(() => reader.ReadStringValueAsync("Data"));
                Assert.Equal(
                    Error.Format(SRResources.JsonReaderExtensions_CannotReadPropertyValueAsString, 13, "Data"),
                    exception.Message);
            }
        }

        [Fact]
        public async Task ReadDoubleValueAsyncThrowsExceptionForValueNotReadableAsDouble()
        {
            using (var reader = await CreateJsonReaderAsync($"{{\"Data\":\"Thirteen\"}}"))
            {
                Assert.True(reader.IsOnValueNode());
                var exception = await Assert.ThrowsAsync<ODataException>(() => reader.ReadDoubleValueAsync());
                Assert.Equal(
                    Error.Format(SRResources.JsonReaderExtensions_CannotReadValueAsDouble, "Thirteen"),
                    exception.Message);
            }
        }

        [Fact]
        public async Task SkipValueAsyncThrowsExceptionForUnbalancedScopes()
        {
            using (var reader = await CreateJsonReaderAsync($"{{\"Product\":{{\"Id\":1,\"Name\":\"Pencil\""))
            {
                var exception = await Assert.ThrowsAsync<ODataException>(() => reader.SkipValueAsync());
                Assert.Equal(SRResources.JsonReader_EndOfInputWithOpenScope, exception.Message);
            }
        }

        [Fact]
        public async Task SkipValueAsyncWithStringBuilderThrowsExceptionForUnbalancedScopes()
        {
            using (var reader = await CreateJsonReaderAsync($"{{\"Product\":{{\"Id\":1,\"Name\":\"Pencil\""))
            {
                var exception = await Assert.ThrowsAsync<ODataException>(() => reader.SkipValueAsync(new StringBuilder()));
                Assert.Equal(SRResources.JsonReader_EndOfInputWithOpenScope, exception.Message);
            }
        }

        [Fact]
        public void ReadLongJson_AllScenarios()
        {
            string json = @"
            {
                ""str"": ""simple string"",
                ""strEsc"": ""escaped \\n newline \\t tab \\r carriage \\b backspace \\f formfeed \\u0041 unicode A \\\""quote\\\"" \\'single\\'"",
                ""strEmpty"": """",
                ""strUnicode"": ""\u6211\u662F\u4E2D\u6587"",
                ""strQuotedNumber"": ""42"",
                ""strQuotedBool"": ""true"",
                ""strQuotedNull"": ""null"",
                ""int"": 123,
                ""negInt"": -456,
                ""dec"": 123.456,
                ""negDec"": -789.01,
                ""dbl"": 1.23e4,
                ""negDbl"": -5.67E-8,
                ""boolTrue"": true,
                ""boolFalse"": false,
                ""nullVal"": null,
                ""arrPrimitives"": [""a"", 1, false, null, ""b""],
                ""arrObjects"": [
                    { ""id"": 1, ""name"": ""first"" },
                    { ""id"": 2, ""name"": ""second"" }
                ],
                ""nestedObj"": {
                    ""innerStr"": ""inside"",
                    ""innerNum"": 99,
                    ""innerBool"": false,
                    ""innerNull"": null
                },
                ""singleQuoteProp"": ""single quoted value"",
                ""unquotedProp"": 321
            }";

            using var reader = new JsonReader(new StringReader(json), isIeee754Compatible: false);

            // Start object
            Assert.True(reader.Read());
            Assert.Equal(JsonNodeType.StartObject, reader.NodeType);

            // str
            Assert.True(reader.Read());
            Assert.Equal("str", reader.GetValue());
            Assert.True(reader.Read());
            Assert.Equal("simple string", reader.GetValue());

            // strEsc
            Assert.True(reader.Read());
            Assert.Equal("strEsc", reader.GetValue());
            Assert.True(reader.Read());
            Assert.Equal("escaped \\n newline \\t tab \\r carriage \\b backspace \\f formfeed \\u0041 unicode A \\\"quote\\\" \\'single\\'", reader.GetValue());

            // strEmpty
            Assert.True(reader.Read());
            Assert.Equal("strEmpty", reader.GetValue());
            Assert.True(reader.Read());
            Assert.Equal("", reader.GetValue());

            // strUnicode
            Assert.True(reader.Read());
            Assert.Equal("strUnicode", reader.GetValue());
            Assert.True(reader.Read());
            Assert.Equal("我是中文", reader.GetValue());

            // strQuotedNumber
            Assert.True(reader.Read());
            Assert.Equal("strQuotedNumber", reader.GetValue());
            Assert.True(reader.Read());
            Assert.Equal("42", reader.GetValue());

            // strQuotedBool
            Assert.True(reader.Read());
            Assert.Equal("strQuotedBool", reader.GetValue());
            Assert.True(reader.Read());
            Assert.Equal("true", reader.GetValue());

            // strQuotedNull
            Assert.True(reader.Read());
            Assert.Equal("strQuotedNull", reader.GetValue());
            Assert.True(reader.Read());
            Assert.Equal("null", reader.GetValue());

            // int
            Assert.True(reader.Read());
            Assert.Equal("int", reader.GetValue());
            Assert.True(reader.Read());
            Assert.Equal(123, reader.GetValue());

            // negInt
            Assert.True(reader.Read());
            Assert.Equal("negInt", reader.GetValue());
            Assert.True(reader.Read());
            Assert.Equal(-456, reader.GetValue());

            // dec
            Assert.True(reader.Read());
            Assert.Equal("dec", reader.GetValue());
            Assert.True(reader.Read());
            Assert.Equal(123.456m, reader.GetValue());

            // negDec
            Assert.True(reader.Read());
            Assert.Equal("negDec", reader.GetValue());
            Assert.True(reader.Read());
            Assert.Equal(-789.01m, reader.GetValue());

            // dbl
            Assert.True(reader.Read());
            Assert.Equal("dbl", reader.GetValue());
            Assert.True(reader.Read());
            Assert.Equal(1.23e4, reader.GetValue());

            // negDbl
            Assert.True(reader.Read());
            Assert.Equal("negDbl", reader.GetValue());
            Assert.True(reader.Read());
            Assert.Equal(-5.67E-8, reader.GetValue());

            // boolTrue
            Assert.True(reader.Read());
            Assert.Equal("boolTrue", reader.GetValue());
            Assert.True(reader.Read());
            Assert.Equal(true, reader.GetValue());

            // boolFalse
            Assert.True(reader.Read());
            Assert.Equal("boolFalse", reader.GetValue());
            Assert.True(reader.Read());
            Assert.Equal(false, reader.GetValue());

            // nullVal
            Assert.True(reader.Read());
            Assert.Equal("nullVal", reader.GetValue());
            Assert.True(reader.Read());
            Assert.Null(reader.GetValue());

            // arrPrimitives
            Assert.True(reader.Read());
            Assert.Equal("arrPrimitives", reader.GetValue());
            Assert.True(reader.Read());
            Assert.Equal(JsonNodeType.StartArray, reader.NodeType);

            // Array elements
            Assert.True(reader.Read());
            Assert.Equal("a", reader.GetValue());
            Assert.True(reader.Read());
            Assert.Equal(1, reader.GetValue());
            Assert.True(reader.Read());
            Assert.Equal(false, reader.GetValue());
            Assert.True(reader.Read());
            Assert.Null(reader.GetValue());
            Assert.True(reader.Read());
            Assert.Equal("b", reader.GetValue());
            Assert.True(reader.Read());
            Assert.Equal(JsonNodeType.EndArray, reader.NodeType);

            // arrObjects
            Assert.True(reader.Read());
            Assert.Equal("arrObjects", reader.GetValue());
            Assert.True(reader.Read());
            Assert.Equal(JsonNodeType.StartArray, reader.NodeType);

            // First object in array
            Assert.True(reader.Read());
            Assert.Equal(JsonNodeType.StartObject, reader.NodeType);
            Assert.True(reader.Read());
            Assert.Equal("id", reader.GetValue());
            Assert.True(reader.Read());
            Assert.Equal(1, reader.GetValue());
            Assert.True(reader.Read());
            Assert.Equal("name", reader.GetValue());
            Assert.True(reader.Read());
            Assert.Equal("first", reader.GetValue());
            Assert.True(reader.Read());
            Assert.Equal(JsonNodeType.EndObject, reader.NodeType);

            // Second object in array
            Assert.True(reader.Read());
            Assert.Equal(JsonNodeType.StartObject, reader.NodeType);
            Assert.True(reader.Read());
            Assert.Equal("id", reader.GetValue());
            Assert.True(reader.Read());
            Assert.Equal(2, reader.GetValue());
            Assert.True(reader.Read());
            Assert.Equal("name", reader.GetValue());
            Assert.True(reader.Read());
            Assert.Equal("second", reader.GetValue());
            Assert.True(reader.Read());
            Assert.Equal(JsonNodeType.EndObject, reader.NodeType);

            Assert.True(reader.Read());
            Assert.Equal(JsonNodeType.EndArray, reader.NodeType);

            // nestedObj
            Assert.True(reader.Read());
            Assert.Equal("nestedObj", reader.GetValue());
            Assert.True(reader.Read());
            Assert.Equal(JsonNodeType.StartObject, reader.NodeType);

            Assert.True(reader.Read());
            Assert.Equal("innerStr", reader.GetValue());
            Assert.True(reader.Read());
            Assert.Equal("inside", reader.GetValue());

            Assert.True(reader.Read());
            Assert.Equal("innerNum", reader.GetValue());
            Assert.True(reader.Read());
            Assert.Equal(99, reader.GetValue());

            Assert.True(reader.Read());
            Assert.Equal("innerBool", reader.GetValue());
            Assert.True(reader.Read());
            Assert.Equal(false, reader.GetValue());

            Assert.True(reader.Read());
            Assert.Equal("innerNull", reader.GetValue());
            Assert.True(reader.Read());
            Assert.Null(reader.GetValue());

            Assert.True(reader.Read());
            Assert.Equal(JsonNodeType.EndObject, reader.NodeType);

            // singleQuoteProp
            Assert.True(reader.Read());
            Assert.Equal("singleQuoteProp", reader.GetValue());
            Assert.True(reader.Read());
            Assert.Equal("single quoted value", reader.GetValue());

            // unquotedProp
            Assert.True(reader.Read());
            Assert.Equal("unquotedProp", reader.GetValue());
            Assert.True(reader.Read());
            Assert.Equal(321, reader.GetValue());

            // End object
            Assert.True(reader.Read());
            Assert.Equal(JsonNodeType.EndObject, reader.NodeType);

            // End of input
            Assert.False(reader.Read());
            Assert.Equal(JsonNodeType.EndOfInput, reader.NodeType);
        }

        [Fact]
        public async Task ReadLongJson_AllScenarios_Async()
        {
            string json = @"
            {
                ""str"": ""simple string"",
                ""strEsc"": ""escaped \\n newline \\t tab \\r carriage \\b backspace \\f formfeed \\u0041 unicode A \\\""quote\\\"" \\'single\\'"",
                ""strEmpty"": """",
                ""strUnicode"": ""\u6211\u662F\u4E2D\u6587"",
                ""strQuotedNumber"": ""42"",
                ""strQuotedBool"": ""true"",
                ""strQuotedNull"": ""null"",
                ""int"": 123,
                ""negInt"": -456,
                ""dec"": 123.456,
                ""negDec"": -789.01,
                ""dbl"": 1.23e4,
                ""negDbl"": -5.67E-8,
                ""boolTrue"": true,
                ""boolFalse"": false,
                ""nullVal"": null,
                ""arrPrimitives"": [""a"", 1, false, null, ""b""],
                ""arrObjects"": [
                    { ""id"": 1, ""name"": ""first"" },
                    { ""id"": 2, ""name"": ""second"" }
                ],
                ""nestedObj"": {
                    ""innerStr"": ""inside"",
                    ""innerNum"": 99,
                    ""innerBool"": false,
                    ""innerNull"": null
                },
                ""singleQuoteProp"": ""single quoted value"",
                ""unquotedProp"": 321
            }";

            using var reader = new JsonReader(new StringReader(json), isIeee754Compatible: false);

            // Start object
            Assert.True(await reader.ReadAsync());
            Assert.Equal(JsonNodeType.StartObject, reader.NodeType);

            // str
            Assert.True(await reader.ReadAsync());
            Assert.Equal("str", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Equal("simple string", await reader.GetValueAsync());

            // strEsc
            Assert.True(await reader.ReadAsync());
            Assert.Equal("strEsc", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Equal("escaped \\n newline \\t tab \\r carriage \\b backspace \\f formfeed \\u0041 unicode A \\\"quote\\\" \\'single\\'", await reader.GetValueAsync());

            // strEmpty
            Assert.True(await reader.ReadAsync());
            Assert.Equal("strEmpty", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Equal("", await reader.GetValueAsync());

            // strUnicode
            Assert.True(await reader.ReadAsync());
            Assert.Equal("strUnicode", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Equal("我是中文", await reader.GetValueAsync());

            // strQuotedNumber
            Assert.True(await reader.ReadAsync());
            Assert.Equal("strQuotedNumber", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Equal("42", await reader.GetValueAsync());

            // strQuotedBool
            Assert.True(await reader.ReadAsync());
            Assert.Equal("strQuotedBool", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Equal("true", await reader.GetValueAsync());

            // strQuotedNull
            Assert.True(await reader.ReadAsync());
            Assert.Equal("strQuotedNull", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Equal("null", await reader.GetValueAsync());

            // int
            Assert.True(await reader.ReadAsync());
            Assert.Equal("int", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Equal(123, await reader.GetValueAsync());

            // negInt
            Assert.True(await reader.ReadAsync());
            Assert.Equal("negInt", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Equal(-456, await reader.GetValueAsync());

            // dec
            Assert.True(await reader.ReadAsync());
            Assert.Equal("dec", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Equal(123.456m, await reader.GetValueAsync());

            // negDec
            Assert.True(await reader.ReadAsync());
            Assert.Equal("negDec", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Equal(-789.01m, await reader.GetValueAsync());

            // dbl
            Assert.True(await reader.ReadAsync());
            Assert.Equal("dbl", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Equal(1.23e4, await reader.GetValueAsync());

            // negDbl
            Assert.True(await reader.ReadAsync());
            Assert.Equal("negDbl", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Equal(-5.67E-8, await reader.GetValueAsync());

            // boolTrue
            Assert.True(await reader.ReadAsync());
            Assert.Equal("boolTrue", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Equal(true, await reader.GetValueAsync());

            // boolFalse
            Assert.True(await reader.ReadAsync());
            Assert.Equal("boolFalse", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Equal(false, await reader.GetValueAsync());

            // nullVal
            Assert.True(await reader.ReadAsync());
            Assert.Equal("nullVal", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Null(await reader.GetValueAsync());

            // arrPrimitives
            Assert.True(await reader.ReadAsync());
            Assert.Equal("arrPrimitives", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Equal(JsonNodeType.StartArray, reader.NodeType);

            // Array elements
            Assert.True(await reader.ReadAsync());
            Assert.Equal("a", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Equal(1, await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Equal(false, await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Null(await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Equal("b", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Equal(JsonNodeType.EndArray, reader.NodeType);

            // arrObjects
            Assert.True(await reader.ReadAsync());
            Assert.Equal("arrObjects", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Equal(JsonNodeType.StartArray, reader.NodeType);

            // First object in array
            Assert.True(await reader.ReadAsync());
            Assert.Equal(JsonNodeType.StartObject, reader.NodeType);
            Assert.True(await reader.ReadAsync());
            Assert.Equal("id", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Equal(1, await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Equal("name", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Equal("first", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Equal(JsonNodeType.EndObject, reader.NodeType);

            // Second object in array
            Assert.True(await reader.ReadAsync());
            Assert.Equal(JsonNodeType.StartObject, reader.NodeType);
            Assert.True(await reader.ReadAsync());
            Assert.Equal("id", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Equal(2, await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Equal("name", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Equal("second", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Equal(JsonNodeType.EndObject, reader.NodeType);

            Assert.True(await reader.ReadAsync());
            Assert.Equal(JsonNodeType.EndArray, reader.NodeType);

            // nestedObj
            Assert.True(await reader.ReadAsync());
            Assert.Equal("nestedObj", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Equal(JsonNodeType.StartObject, reader.NodeType);

            Assert.True(await reader.ReadAsync());
            Assert.Equal("innerStr", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Equal("inside", await reader.GetValueAsync());

            Assert.True(await reader.ReadAsync());
            Assert.Equal("innerNum", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Equal(99, await reader.GetValueAsync());

            Assert.True(await reader.ReadAsync());
            Assert.Equal("innerBool", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Equal(false, await reader.GetValueAsync());

            Assert.True(await reader.ReadAsync());
            Assert.Equal("innerNull", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Null(await reader.GetValueAsync());

            Assert.True(await reader.ReadAsync());
            Assert.Equal(JsonNodeType.EndObject, reader.NodeType);

            // singleQuoteProp
            Assert.True(await reader.ReadAsync());
            Assert.Equal("singleQuoteProp", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Equal("single quoted value", await reader.GetValueAsync());

            // unquotedProp
            Assert.True(await reader.ReadAsync());
            Assert.Equal("unquotedProp", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync());
            Assert.Equal(321, await reader.GetValueAsync());

            // End object
            Assert.True(await reader.ReadAsync());
            Assert.Equal(JsonNodeType.EndObject, reader.NodeType);

            // End of input
            Assert.False(await reader.ReadAsync());
            Assert.Equal(JsonNodeType.EndOfInput, reader.NodeType);
        }

        private JsonReader CreateJsonReader(string jsonValue)
        {
            JsonReader reader = new JsonReader(
                new StringReader(String.Format("{{ \"data\" : {0} }}", jsonValue)),
                isIeee754Compatible: false);
            reader.Read();
            reader.ReadStartObject();
            reader.ReadPropertyName();
            Assert.Equal(JsonNodeType.PrimitiveValue, reader.NodeType);

            return reader;
        }

        private async Task<JsonReader> CreateJsonReaderAsync(string payload, bool isIeee754Compatible = false)
        {
            JsonReader reader = new JsonReader(new StringReader(payload), isIeee754Compatible: isIeee754Compatible);

            await reader.ReadAsync();
            await reader.ReadStartObjectAsync();
            await reader.ReadPropertyNameAsync();

            return reader;
        }

        public class TestArrayPool : ICharArrayPool
        {
            public int RentCount { get; set; }

            public int ReturnCount { get; set; }

            public char[] Rent(int minSize)
            {
                RentCount++;
                return new char[minSize];
            }

            public void Return(char[] array)
            {
                ReturnCount++;
            }
        }
    }
}

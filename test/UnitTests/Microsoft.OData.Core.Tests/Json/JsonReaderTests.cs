//---------------------------------------------------------------------
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
        [InlineData(" {\"Data\":   \"The \\r character\"}    ", "The \r character")]
        [InlineData("   {\"Data\":\"The \\n   \\t\r character\"   }", "The \n   \t\r character")]
        [InlineData("{    \"Data\":\"The \\t character  \n\r   \r\"}", "The \t character  \n\r   \r")]
        [InlineData("{\"Data\"      :\"    The    character     \"}", "    The    character     ")]
        public void ReadPrimitiveValueWithWhitespaces(string payload, string expected)
        {
            using (var reader = new JsonReader(new StringReader(payload), isIeee754Compatible: false))
            {
                reader.Read(); // Read start of object
                reader.Read(); // Read property name - Data
                reader.Read(); // Position reader at the beginning of string value
                Assert.Equal(expected, reader.GetValue());
            }
        }

        [Theory]
        [InlineData(" {\"Data\":   \"The \\r character\"}    ", "The \r character")]
        [InlineData("   {\"Data\":\"The \\n   \\t\r character\"   }", "The \n   \t\r character")]
        [InlineData("{    \"Data\":\"The \\t character  \n\r   \r\"}", "The \t character  \n\r   \r")]
        [InlineData("{\"Data\"      :\"    The    character     \"}", "    The    character     ")]
        public async Task ReadPrimitiveValueAsyncWithWhitespaces(string payload, string expected)
        {
            using (var reader = new JsonReader(new StringReader(payload), isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // Read start of object
                await reader.ReadAsync(); // Read property name - Data
                await reader.ReadAsync(); // Position reader at the beginning of string value
                Assert.Equal(expected, await reader.GetValueAsync());
            }
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
        [InlineData("{\"Data\":\"The \\u621", "\\u621")]
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

        [Theory]
        [InlineData("", 0, 0, -1)] // Empty buffer
        [InlineData("    ", 0, 4, -1)] // All spaces
        [InlineData("\t\t\t", 0, 3, -1)] // All tabs
        [InlineData("\n\n", 0, 2, -1)] // All newlines
        [InlineData("\r\r", 0, 2, -1)] // All carriage returns
        [InlineData(" \t\r\n", 0, 4, -1)] // All JSON whitespace chars
        [InlineData("a", 0, 1, 0)] // Single non-whitespace
        [InlineData(" a", 0, 2, 1)] // Leading space
        [InlineData("\t\n\rb", 0, 4, 3)] // Leading mixed whitespace
        [InlineData("c ", 0, 2, 0)] // Trailing whitespace
        [InlineData(" \t\r\nd", 0, 5, 4)] // All whitespace then non-whitespace
        [InlineData(" \t\r\n", 1, 4, -1)] // Subspan, all whitespace
        [InlineData(" \t\r\nx", 0, 5, 4)] // Non-whitespace at end
        [InlineData(" \t\r\nx", 1, 5, 3)] // Subspan, non-whitespace at end
        [InlineData(" \t\r\nx", 0, 4, -1)] // Subspan, all whitespace
        [InlineData(" \t\r\nx", 2, 5, 2)] // Subspan, non-whitespace at end
        public void FindFirstNonWhitespace_CoversAllCases(string input, int start, int end, int expected)
        {
            // Arrange
            char[] buffer = input.ToCharArray();

            // Act
            int result = typeof(JsonReader)
                .GetMethod("FindFirstNonWhitespace", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                .Invoke(null, new object[] { buffer, start, end }) as int? ?? -2;

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("   42", 42)] // Leading spaces
        [InlineData("\t\n\r42", 42)] // Leading mixed whitespace
        [InlineData("42   ", 42)] // Trailing spaces
        [InlineData("   42   ", 42)] // Leading and trailing spaces
        [InlineData("\t\n\r 42 \t\n\r", 42)] // Leading/trailing mixed whitespace
        [InlineData("   \"foo\"   ", "foo")] // Quoted string with whitespace
        [InlineData("   null   ", null)] // Null with whitespace
        [InlineData("   true   ", true)] // Boolean true with whitespace
        [InlineData("   false   ", false)] // Boolean false with whitespace
        [InlineData("   -42   ", -42)] // Negative int with whitespace
        [InlineData("     \n\n\n\n\n\n3\r\r\r\r\r     \n\n\n\n\n!", 3)]
        [InlineData("     \n\n\n\n\n\n-3\r\r\r\r\r     \n\n\n\n\n!", -3)]
        [InlineData("     \n\n\n\n\n\ntrue\r\r\r\r\r     \n\n\n\n\n!", true)]
        [InlineData("     \n\n\n\n\n\nnull\r\r\r\r\r     \n\n\n\n\n!", null)]
        public void Read_SkipWhitespaces_CorrectValue(string payload, object expected)
        {
            var reader = new JsonReader(new StringReader(payload), isIeee754Compatible: false);
            Assert.True(reader.Read());
            Assert.Equal(expected, reader.GetValue());
        }

        [Theory]
        [InlineData("   42", 42)]
        [InlineData("\t\n\r42", 42)]
        [InlineData("42   ", 42)]
        [InlineData("   42   ", 42)]
        [InlineData("\t\n\r 42 \t\n\r", 42)]
        [InlineData("   \"foo\"   ", "foo")]
        [InlineData("   null   ", null)]
        [InlineData("   true   ", true)]
        [InlineData("   false   ", false)]
        [InlineData("   -42   ", -42)]
        [InlineData("     \n\n\n\n\n\n3\r\r\r\r\r     \n\n\n\n\n!", 3)]
        [InlineData("     \n\n\n\n\n\n-3\r\r\r\r\r     \n\n\n\n\n!", -3)]
        [InlineData("     \n\n\n\n\n\ntrue\r\r\r\r\r     \n\n\n\n\n!", true)]
        [InlineData("     \n\n\n\n\n\nnull\r\r\r\r\r     \n\n\n\n\n!", null)]
        public async Task ReadAsync_SkipWhitespaces_CorrectValue(string payload, object expected)
        {
            var reader = new JsonReader(new StringReader(payload), isIeee754Compatible: false);
            Assert.True(await reader.ReadAsync());
            Assert.Equal(expected, await reader.GetValueAsync());
        }

        public static TheoryData<string, string> LargeStringsWithWhitespace()
        {
            return new TheoryData<string, string>
            {
                { new string(' ', 10000) + "\"HelloWorld\"" + new string(' ', 10000), "HelloWorld" },
                { new string('\t', 5000) + "\"TestString\"" + new string('\n', 5000), "TestString" },
                { new string('\r', 8000) + "\"WhitespaceTest\"" + new string(' ', 2000), "WhitespaceTest" },
                { new string(' ', 2500) + new string('\n', 2500) + "\"MixedWhitespace\"" + new string('\t', 2500) + new string('\r', 2500), "MixedWhitespace" },
                { new string(' ', 5) + new string('\n', 5) + "\"Hello\"" + new string(' ', 5) + new string('\t', 5) + "\"World\"" + new string('\r', 5) + new string(' ', 5) + new string('\n', 5) + "!", "Hello" },
                { new string(' ', 5) + new string('\n', 5) + "\"Hello" + new string(' ', 5) + new string('\t', 5) + "World" + new string('\r', 5) + new string(' ', 5) + new string('\n', 5) + "!\"", "Hello     \t\t\t\t\tWorld\r\r\r\r\r     \n\n\n\n\n!" },
            };
        }

        [Theory]
        [MemberData(nameof(LargeStringsWithWhitespace))]
        public void Read_LargeStringWithWhitespaces(string largeString, string expected)
        {
            var reader = new JsonReader(new StringReader(largeString), isIeee754Compatible: false);
            Assert.True(reader.Read());
            Assert.Equal(expected, reader.GetValue());
        }

        [Theory]
        [MemberData(nameof(LargeStringsWithWhitespace))]
        public async Task ReadAsync_LargeStringWithWhitespaces(string largeString, string expected)
        {
            var reader = new JsonReader(new StringReader(largeString), isIeee754Compatible: false);
            Assert.True(await reader.ReadAsync());
            Assert.Equal(expected, await reader.GetValueAsync());
        }

        [Fact]
        public void Read_LargeEmptyStringWithWhitespaces()
        {
            string largeString = new string(' ', 2500) + new string('\n', 2500) + new string('\t', 2500) + new string('\r', 2500);
            var reader = new JsonReader(new StringReader(largeString), isIeee754Compatible: false);
            Assert.False(reader.Read());
            Assert.Null(reader.GetValue());
        }

        [Fact]
        public async Task ReadAsync_LargeEmptyStringWithWhitespaces()
        {
            string largeString = new string(' ', 2500) + new string('\n', 2500) + new string('\t', 2500) + new string('\r', 2500);
            var reader = new JsonReader(new StringReader(largeString), isIeee754Compatible: false);
            Assert.False(await reader.ReadAsync());
            Assert.Null(await reader.GetValueAsync());
        }

        [Fact]
        public void Read_MixedWhitespaceAndNewlines()
        {
            var payload = "\n\n\t   \r\n\"abc\"\n\t\r";
            var reader = new JsonReader(new StringReader(payload), isIeee754Compatible: false);
            Assert.True(reader.Read());
            Assert.Equal("abc", reader.GetValue());
        }

        [Fact]
        public async Task ReadAsync_MixedWhitespaceAndNewlines()
        {
            var payload = "\n\n\t   \r\n\"abc\"\n\t\r";
            var reader = new JsonReader(new StringReader(payload), isIeee754Compatible: false);
            Assert.True(await reader.ReadAsync());
            Assert.Equal("abc", await reader.GetValueAsync());
        }

        [Fact]
        public void Read_OnlyWhitespace_ReturnsFalse()
        {
            var reader = new JsonReader(new StringReader("   \t\n\r   "), isIeee754Compatible: false);
            Assert.False(reader.Read());
        }

        [Fact]
        public async Task ReadAsync_OnlyWhitespace_ReturnsFalse()
        {
            var reader = new JsonReader(new StringReader("   \t\n\r   "), isIeee754Compatible: false);
            Assert.False(await reader.ReadAsync());
        }

        [Fact]
        public void Read_WhitespaceBetweenTokens_Object()
        {
            var payload = "{   \"foo\"   :   \"bar\"   }";
            var reader = new JsonReader(new StringReader(payload), isIeee754Compatible: false);
            Assert.True(reader.Read()); // StartObject
            Assert.True(reader.Read()); // Property
            Assert.Equal("foo", reader.GetValue());
            Assert.True(reader.Read()); // Value
            Assert.Equal("bar", reader.GetValue());
            Assert.True(reader.Read()); // EndObject
        }

        [Fact]
        public async Task ReadAsync_WhitespaceBetweenTokens_Object()
        {
            var payload = "{   \"foo\"   :   \"bar\"   }";
            var reader = new JsonReader(new StringReader(payload), isIeee754Compatible: false);
            Assert.True(await reader.ReadAsync()); // StartObject
            Assert.True(await reader.ReadAsync()); // Property
            Assert.Equal("foo", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync()); // Value
            Assert.Equal("bar", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync()); // EndObject
        }

        [Fact]
        public void Read_WhitespaceBetweenTokens_Array()
        {
            var payload = "[   \"a\"   ,   \"b\"   ,   \"c\"   ]";
            var reader = new JsonReader(new StringReader(payload), isIeee754Compatible: false);
            Assert.True(reader.Read()); // StartArray
            Assert.True(reader.Read()); // Value
            Assert.Equal("a", reader.GetValue());
            Assert.True(reader.Read()); // Value
            Assert.Equal("b", reader.GetValue());
            Assert.True(reader.Read()); // Value
            Assert.Equal("c", reader.GetValue());
            Assert.True(reader.Read()); // EndArray
        }

        [Fact]
        public async Task ReadAsync_WhitespaceBetweenTokens_Array()
        {
            var payload = "[   \"a\"   ,   \"b\"   ,   \"c\"   ]";
            var reader = new JsonReader(new StringReader(payload), isIeee754Compatible: false);
            Assert.True(await reader.ReadAsync()); // StartArray
            Assert.True(await reader.ReadAsync()); // Value
            Assert.Equal("a", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync()); // Value
            Assert.Equal("b", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync()); // Value
            Assert.Equal("c", await reader.GetValueAsync());
            Assert.True(await reader.ReadAsync()); // EndArray
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

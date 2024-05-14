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
                Assert.Equal(Strings.JsonReader_UnexpectedComma("Root"), exception.Message);
            }
        }

        [Fact]
        public async Task UnexpectedCommaAtObjectStartThrowsException()
        {
            using (var reader = new JsonReader(new StringReader("{,}"), isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // Read start of object
                var exception = await Assert.ThrowsAsync<ODataException>(async () => await reader.ReadAsync());
                Assert.Equal(Strings.JsonReader_UnexpectedComma("Object"), exception.Message);
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
                Assert.Equal(Strings.JsonReader_UnexpectedComma("Object"), exception.Message);
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
                Assert.Equal(Strings.JsonReader_MissingComma("Object"), exception.Message);
            }
        }

        [Fact]
        public async Task EmptyPropertyNameThrowsException()
        {
            using (var reader = new JsonReader(new StringReader("{\"\":1}"), isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // Read start of object
                var exception = await Assert.ThrowsAsync<ODataException>(async () => await reader.ReadAsync()); // Try to read property name
                Assert.Equal(Strings.JsonReader_InvalidPropertyNameOrUnexpectedComma(string.Empty), exception.Message);
            }
        }

        [Fact]
        public async Task MissingColonAfterPropertyNameThrowsException()
        {
            using (var reader = new JsonReader(new StringReader("{\"Id\"}"), isIeee754Compatible: false))
            {
                await reader.ReadAsync(); // Read start of object
                var exception = await Assert.ThrowsAsync<ODataException>(async () => await reader.ReadAsync());
                Assert.Equal(Strings.JsonReader_MissingColon("Id"), exception.Message);
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
                Assert.Equal(Strings.JsonReader_UnrecognizedToken, exception.Message);
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
                Assert.Equal(Strings.JsonReader_UnexpectedComma("Property"), exception.Message);
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
                Assert.Equal(Strings.JsonReader_MultipleTopLevelValues, exception.Message);
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
                Assert.Equal(Strings.JsonReader_UnexpectedComma("Array"), exception.Message);
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
                Assert.Equal(Strings.JsonReader_UnexpectedComma("Array"), exception.Message);
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
                Assert.Equal(Strings.JsonReader_MissingComma("Array"), exception.Message);
            }
        }

        [Fact]
        public async Task UnexpectedNullTokenThrowsException()
        {
            using (var reader = new JsonReader(new StringReader("nil"), isIeee754Compatible: false))
            {
                await reader.ReadAsync();
                var exception = await Assert.ThrowsAsync<ODataException>(async () => await reader.GetValueAsync());
                Assert.Equal(Strings.JsonReader_UnexpectedToken("nil"), exception.Message);
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
                Assert.Equal(Strings.JsonReader_UnrecognizedEscapeSequence(expected), exception.Message);
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
                    Assert.Equal(Strings.JsonReader_UnrecognizedEscapeSequence(expected), exception.Message);
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
                Assert.Equal(Strings.JsonReader_CannotCreateReadStream, exception.Message);
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
                Assert.Equal(Strings.JsonReader_CannotCreateTextReader, exception.Message);
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

                Assert.Equal(Strings.JsonReader_CannotCallReadInStreamState, exception.Message);
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

                Assert.Equal(Strings.JsonReader_CannotAccessValueInStreamState, exception.Message);
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
                Assert.Equal(Strings.JsonReader_UnexpectedEndOfString, exception.Message);
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
                Assert.Equal(Strings.JsonReader_UnexpectedToken("tRue"), exception.Message);
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
                Assert.Equal(Strings.JsonReader_InvalidNumberFormat("6.0221409-e23"), exception.Message);
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
                Assert.Equal(Strings.JsonReaderExtensions_CannotReadValueAsString(13), exception.Message);
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
                    Strings.JsonReaderExtensions_CannotReadPropertyValueAsString(13, "Data"),
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
                    Strings.JsonReaderExtensions_CannotReadValueAsDouble("Thirteen"),
                    exception.Message);
            }
        }

        [Fact]
        public async Task SkipValueAsyncThrowsExceptionForUnbalancedScopes()
        {
            using (var reader = await CreateJsonReaderAsync($"{{\"Product\":{{\"Id\":1,\"Name\":\"Pencil\""))
            {
                var exception = await Assert.ThrowsAsync<ODataException>(() => reader.SkipValueAsync());
                Assert.Equal(Strings.JsonReader_EndOfInputWithOpenScope, exception.Message);
            }
        }

        [Fact]
        public async Task SkipValueAsyncWithStringBuilderThrowsExceptionForUnbalancedScopes()
        {
            using (var reader = await CreateJsonReaderAsync($"{{\"Product\":{{\"Id\":1,\"Name\":\"Pencil\""))
            {
                var exception = await Assert.ThrowsAsync<ODataException>(() => reader.SkipValueAsync(new StringBuilder()));
                Assert.Equal(Strings.JsonReader_EndOfInputWithOpenScope, exception.Message);
            }
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

//---------------------------------------------------------------------
// <copyright file="JsonWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    /// <summary>
    /// Unit tests for the JsonWriter class.
    /// TODO: write unit tests for the remaining functions on JsonWriter.
    /// </summary>
    public class JsonWriterTests : JsonWriterBaseTests
    {
        private StringBuilder builder;
        private IJsonWriter writer;

        public JsonWriterTests()
        {
            this.builder = new StringBuilder();
            this.writer = new JsonWriter(new StringWriter(builder), isIeee754Compatible: true);
            
        }

        [Fact]
        public void StartPaddingFunctionScopeWritesParenthesis()
        {
            this.writer.StartPaddingFunctionScope();
            Assert.Equal("(", this.builder.ToString());
        }

        [Fact]
        public void EndPaddingFunctionScopeWritesParenthesis()
        {
            this.writer.StartPaddingFunctionScope();
            this.writer.EndPaddingFunctionScope();
            Assert.Equal("()", this.builder.ToString());
        }

        [Fact]
        public void WritePaddingFunctionNameWritesName()
        {
            this.writer.WritePaddingFunctionName("example");
            Assert.Equal("example", this.builder.ToString());
        }

        #region WritePrimitiveValue

        [Fact]
        public void WritePrimitiveValueBoolean()
        {
            this.VerifyWritePrimitiveValue(false, "false");
        }

        [Fact]
        public void WritePrimitiveValueByte()
        {
            this.VerifyWritePrimitiveValue((byte)4, "4");
        }

        [Fact]
        public void WritePrimitiveValueDecimalWithIeee754CompatibleTrue()
        {
            this.VerifyWriterPrimitiveValueWithIeee754Compatible(42.2m, "\"42.2\"", isIeee754Compatible: true);
        }

        [Fact]
        public void WritePrimitiveValueDecimalWithIeee754CompatibleFalse()
        {
            this.VerifyWriterPrimitiveValueWithIeee754Compatible(42.2m, "42.2", isIeee754Compatible: false);
        }

        [Fact]
        public void WritePrimitiveValueDouble()
        {
            this.VerifyWritePrimitiveValue(42.2d, "42.2");
        }

        [Fact]
        public void WritePrimitiveValueDoubleNaN()
        {
            this.VerifyWritePrimitiveValue(double.NaN, "\"NaN\"");
        }

        [Fact]
        public void WritePrimitiveValueDoublePositiveInfinity()
        {
            this.VerifyWritePrimitiveValue(double.PositiveInfinity, "\"INF\"");
        }

        [Fact]
        public void WritePrimitiveValueDoubleNegativeInfinity()
        {
            this.VerifyWritePrimitiveValue(double.NegativeInfinity, "\"-INF\"");
        }

        [Fact]
        public void WritePrimitiveValueInt16()
        {
            this.VerifyWritePrimitiveValue((short)876, "876");
        }

        [Fact]
        public void WritePrimitiveValueInt32()
        {
            this.VerifyWritePrimitiveValue((int)876, "876");
        }

        [Fact]
        public void WritePrimitiveValueInt64WithIeee754CompatibleTrue()
        {
            this.VerifyWriterPrimitiveValueWithIeee754Compatible((long)876, "\"876\"", isIeee754Compatible: true);
        }

        [Fact]
        public void WritePrimitiveValueInt64WithIeee754CompatibleFalse()
        {
            this.VerifyWriterPrimitiveValueWithIeee754Compatible((long)876, "876", isIeee754Compatible: false);
        }

        [Fact]
        public void WritePrimitiveValueSByte()
        {
            this.VerifyWritePrimitiveValue((sbyte)4, "4");
        }

        [Fact]
        public void WritePrimitiveValueSingle()
        {
            this.VerifyWritePrimitiveValue((Single)876, "876");
        }

        [Fact]
        public void WritePrimitiveValueString()
        {
            this.VerifyWritePrimitiveValue("string", "\"string\"");
        }

        [Fact]
        public void WritePrimitiveValueStringWritesNullIfArgumentIsNull()
        {
            this.writer.WriteValue((string)null);
            Assert.Equal("null", this.builder.ToString());
        }

        [Theory]
        [InlineData("Foo \uD800\udc05 \u00e4", "\"Foo \\ud800\\udc05 \\u00e4\"")]
        [InlineData("Foo \nBar\t\"Baz\"", "\"Foo \\nBar\\t\\\"Baz\\\"\"")]
        [InlineData("Foo ия", "\"Foo \\u0438\\u044f\"")]
        [InlineData("<script>", "\"<script>\"")]
        public void WritePrimitiveValueStringEscapesStrings(string input, string expectedOutput)
        {
            this.VerifyWritePrimitiveValue(input, expectedOutput);
        }

        [Fact]
        public void WritePrimitiveValueByteArray()
        {
            this.VerifyWritePrimitiveValue(new byte[] { 0 }, "\"" + Convert.ToBase64String(new byte[] { 0 }) + "\"");
        }

        [Fact]
        public void WritePrimitiveValueByteArrayWritesNullIfArgumentIsNull()
        {
            this.writer.WriteValue((byte[])null);
            Assert.Equal("null", this.builder.ToString());
        }

        [Fact]
        public void WritePrimitiveValueDateTimeOffset()
        {
            this.VerifyWritePrimitiveValue(new DateTimeOffset(1, 2, 3, 4, 5, 6, 7, new TimeSpan(1, 2, 0)), "\"0001-02-03T04:05:06.007+01:02\"");
        }

        [Fact]
        public void WritePrimitiveValueDateTimeOffsetWithZeroOffset()
        {
            this.VerifyWritePrimitiveValue(new DateTimeOffset(1, 2, 3, 4, 5, 6, 7, TimeSpan.Zero), "\"0001-02-03T04:05:06.007Z\"");
        }

        [Fact]
        public void WritePrimitiveValueGuid()
        {
            this.VerifyWritePrimitiveValue(new Guid("00000012-0000-0000-0000-012345678900"), "\"00000012-0000-0000-0000-012345678900\"");
        }

        [Fact]
        public void WritePrimitiveValueTimeSpan()
        {
            this.VerifyWritePrimitiveValue(new TimeSpan(1, 2, 3, 4, 5), "\"P1DT2H3M4.005S\"");
        }

        [Fact]
        public void WritePrimitiveValueDate()
        {
            this.VerifyWritePrimitiveValue(new Date(2014, 12, 31), "\"2014-12-31\"");
        }

        [Fact]
        public void WritePrimitiveValueDateOnly()
        {
            this.VerifyWritePrimitiveValue(new DateOnly(2024, 10, 1), "\"2024-10-01\"");
        }

        [Fact]
        public void WritePrimitiveValueTimeOfDay()
        {
            this.VerifyWritePrimitiveValue(new TimeOfDay(12, 30, 5, 10), "\"12:30:05.0100000\"");
        }

        [Fact]
        public void WritePrimitiveValueTimeOnly()
        {
            this.VerifyWritePrimitiveValue(new TimeOnly(4, 3, 2, 1), "\"04:03:02.0010000\"");
        }

        [Fact]
        public void WriteRawValueWritesValue()
        {
            this.writer.WriteRawValue("Raw\t\"Value ия");
            Assert.Equal("Raw\t\"Value ия", this.builder.ToString());
        }

        [Fact]
        public void WriteRawValueWritesNothingWhenNull()
        {
            this.writer.WriteRawValue(null);
            Assert.Equal("", this.builder.ToString());
        }

        private void VerifyWritePrimitiveValue<T>(T parameter, string expected)
        {
            this.writer.WritePrimitiveValue(parameter);
            Assert.Equal(expected, this.builder.ToString());
        }

        private void VerifyWriterPrimitiveValueWithIeee754Compatible<T>(T parameter, string expected, bool isIeee754Compatible)
        {
            this.writer = new JsonWriter(new StringWriter(builder), isIeee754Compatible);
            this.writer.WritePrimitiveValue(parameter);
            Assert.Equal(expected, this.builder.ToString());
        }

        #endregion

        #region JsonWriter Extension Methods

        [Fact]
        public void WriteJsonObjectValueWritesJsonObject()
        {
            var properties = new Dictionary<string, object>
            {
                { "Name", "Sue" },
                { "Attributes",
                    new Dictionary<string, object>
                    {
                        { "Height", 1.77 },
                        { "Weight", 80.7 }
                    }
                }
            };
            this.writer.WriteJsonObjectValue(properties, null);
            Assert.Equal("{\"Name\":\"Sue\",\"Attributes\":{\"Height\":1.77,\"Weight\":80.7}}", this.builder.ToString());
        }

        [Fact]
        public void WriteJsonObjectValueCallsInjectedPropertyAction()
        {
            var properties = new Dictionary<string, object> { { "Name", "Sue" } };
            Action<IJsonWriter> injectPropertyDelegate = (IJsonWriter actionWriter) =>
            {
                actionWriter.WriteName("Id");
                actionWriter.WriteValue(7);
            };

            this.writer.WriteJsonObjectValue(properties, injectPropertyDelegate);
            Assert.Equal("{\"Id\":7,\"Name\":\"Sue\"}", this.builder.ToString());
        }

        [Fact]
        public void WriteJsonObjectValueWritesPrimitiveCollection()
        {
            var properties = new Dictionary<string, object>
            {
                { "Names", new string[] { "Sue", "Joe", null } }
            };

            this.writer.WriteJsonObjectValue(properties, null);
            Assert.Equal("{\"Names\":[\"Sue\",\"Joe\",null]}", this.builder.ToString());
        }

        [Fact]
        public void WriteODataValueWritesODataValue()
        {
            var value = new ODataPrimitiveValue(3.14);
            this.writer.WriteODataValue(value);
            Assert.Equal("3.14", this.builder.ToString());
        }

        [Fact]
        public void WriteODataValueWritesNullValue()
        {
            var value = new ODataNullValue();
            this.writer.WriteODataValue(value);
            Assert.Equal("null", this.builder.ToString());
        }

        [Fact]
        public void WriteODataValueWritesODataResourceValue()
        {
            var resourceValue = new ODataResourceValue
            {
                Properties = new List<ODataProperty>
                {
                    new ODataProperty { Name = "Name", Value = "Sue" },
                    new ODataProperty { Name = "Age", Value = 19 }
                }
            };
            
            this.writer.WriteODataValue(resourceValue);
            Assert.Equal("{\"Name\":\"Sue\",\"Age\":19}", this.builder.ToString());
        }

        [Fact]
        public void WriteODataValueWritesODataCollectionValue()
        {
            var collectionValue = new ODataCollectionValue
            {
                Items = new List<ODataResourceValue>
                {
                    new ODataResourceValue
                    {
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Name", Value = "Sue" },
                            new ODataProperty { Name = "Age", Value = 19 }
                        }
                    },
                    new ODataResourceValue
                    {
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Name", Value = "Joe" },
                            new ODataProperty { Name = "Age", Value = 23 }
                        }
                    }
                }
            };

            this.writer.WriteODataValue(collectionValue);
            Assert.Equal("[{\"Name\":\"Sue\",\"Age\":19},{\"Name\":\"Joe\",\"Age\":23}]", this.builder.ToString());
        }

        #endregion JsonWriter Extension Methods

        [Fact]
        public void WriteNameUsesProvidedCharArrayPool()
        {
            // Note: CharArrayPool is used if string has special chars
            // This test is mostly theoretical since special characters are not allowed in names
            SetupJsonWriterRunTestAndVerifyRent(
                (jsonWriter) => jsonWriter.WriteName("foo\tbar"));
        }

        [Fact]
        public void WriteStringValueUsesProvidedCharArrayPool()
        {
            SetupJsonWriterRunTestAndVerifyRent(
                (jsonWriter) => jsonWriter.WriteValue("foo\tbar"));
        }

        [Theory]
        [InlineData("text/plain")]
        [InlineData("text/html")]
        public void CorrectlyStreams_NonAsciiCharacters_ToOutput(string contentType)
        {
            string input = "😊😊😊😊😊😊😊";
            string expectedOutput = "😊😊😊😊😊😊😊";

            using (MemoryStream stream = new MemoryStream())
            {
                IJsonWriter jsonWriter = CreateJsonWriter(stream, false, Encoding.UTF8);

                var tw = jsonWriter.StartTextWriterValueScope(contentType);

                WriteSpecialCharsInChunksOfOddStringInChunks(tw, input);

                jsonWriter.EndTextWriterValueScope();
                jsonWriter.Flush();

                stream.Seek(0, SeekOrigin.Begin);

                using (StreamReader reader = new StreamReader(stream, encoding: Encoding.UTF8))
                {
                    string rawOutput = reader.ReadToEnd();
                    Assert.Equal($"\"{expectedOutput}\"", rawOutput);
                }
            }
        }

        [Theory]
        [InlineData("text/plain")]
        [InlineData("text/html")]
        public void CorrectlyStreamsLargeStringsWithSpecialCharactersToOutput(string contentType)
        {
            int inputLength = 1024 * 1024; // 1MB
            string input = new string('a', inputLength);

            // Append special characters to the input string
            input += "😊";

            using (MemoryStream stream = new MemoryStream())
            {
                IJsonWriter jsonWriter = CreateJsonWriter(stream, false, Encoding.UTF8);

                var tw = jsonWriter.StartTextWriterValueScope(contentType);

                WriteLargeStringInChunks(tw, input);

                jsonWriter.EndTextWriterValueScope();
                jsonWriter.Flush();

                stream.Seek(0, SeekOrigin.Begin);

                using (StreamReader reader = new StreamReader(stream, encoding: Encoding.UTF8))
                {
                    string rawOutput = reader.ReadToEnd();
                    Assert.Equal($"\"{input}\"", rawOutput);
                }
            }
        }

        [Theory]
        [InlineData("text/plain")]
        [InlineData("text/html")]
        public void CorrectlyStreamsLargeStrings_WithOnlySpecialCharacters_ToOutput(string contentType)
        {
            string input = "\n\n\n\n\"\"\n\n\n\n\"\"";
            string expectedOutput = "\\n\\n\\n\\n\\\"\\\"\\n\\n\\n\\n\\\"\\\"";
            using (MemoryStream stream = new MemoryStream())
            {
                IJsonWriter jsonWriter = CreateJsonWriter(stream, false, Encoding.UTF8);

                var tw = jsonWriter.StartTextWriterValueScope(contentType);

                WriteSpecialCharsInChunksOfOddStringInChunks(tw, input);

                jsonWriter.EndTextWriterValueScope();
                jsonWriter.Flush();

                stream.Seek(0, SeekOrigin.Begin);

                using (StreamReader reader = new StreamReader(stream, encoding: Encoding.UTF8))
                {
                    string rawOutput = reader.ReadToEnd();
                    Assert.Equal($"\"{expectedOutput}\"", rawOutput);
                }
            }
        }

        [Theory]
        [InlineData("text/plain")]
        [InlineData("text/html")]
        public void CorrectlyStreamsLargeStringsToOutput(string contentType)
        {
            int inputLength = 1024 * 1024; // 1MB
            string input = new string('a', inputLength);
            string expectedOutput = ExpectedOutPutStringWithSpecialCharacters(input);

            using (MemoryStream stream = new MemoryStream())
            {
                IJsonWriter jsonWriter = CreateJsonWriter(stream, false, Encoding.UTF8);

                var tw = jsonWriter.StartTextWriterValueScope(contentType);

                WriteLargeStringsWithSpecialCharactersInChunks(tw, input);

                jsonWriter.EndTextWriterValueScope();
                jsonWriter.Flush();

                stream.Seek(0, SeekOrigin.Begin);

                using (StreamReader reader = new StreamReader(stream, encoding: Encoding.UTF8))
                {
                    string rawOutput = reader.ReadToEnd();
                    Assert.Equal($"\"{expectedOutput}\"", rawOutput);
                }
            }
        }

        [Theory]
        [InlineData("application/json", "🐂")]
        [InlineData("text/html", "\"🐂\"")]
        [InlineData("text/plain", "\"🐂\"")]
        public void TextWriter_CorrectlyHandlesSurrogatePairs(string contentType, string expectedOutput)
        {
            using MemoryStream stream = new MemoryStream();
            IJsonWriter jsonWriter = CreateJsonWriter(stream, isIeee754Compatible: false, Encoding.UTF8);
            var tw = jsonWriter.StartTextWriterValueScope(contentType);
            tw.Write('\ud83d');
            tw.Write('\udc02');
            jsonWriter.EndTextWriterValueScope();
            jsonWriter.Flush();

            stream.Seek(0, SeekOrigin.Begin);

            using StreamReader reader = new StreamReader(stream, encoding: Encoding.UTF8);
            string rawOutput = reader.ReadToEnd();
            Assert.Equal(expectedOutput, rawOutput);
        }

        private void SetupJsonWriterRunTestAndVerifyRent(Action<JsonWriter> action)
        {
            var jsonWriter = new JsonWriter(new StringWriter(builder), isIeee754Compatible: true);
            bool rentVerified = false;

            Action<int> rentVerifier = (minSize) => { rentVerified = true; };
            jsonWriter.ArrayPool = new MockCharArrayPool { RentVerifier = rentVerifier };

            jsonWriter.StartObjectScope();
            action(jsonWriter);

            Assert.True(rentVerified);
        }

        protected override IJsonWriter CreateJsonWriter(Stream stream, bool isIeee754Compatible, Encoding encoding)
        {
            return new JsonWriter(new StreamWriter(stream, encoding), isIeee754Compatible);
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="ODataUtf8JsonWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    /// <summary>
    /// Unit tests for the ODataUtf8JsonWriter class
    /// </summary>
    public sealed class ODataUtf8JsonWriterTests: JsonWriterBaseTests, IDisposable
    {
        private IJsonWriter writer;
        private MemoryStream stream;
        private bool disposed;

        private const int bufferSize = 4 * 1024;

        public ODataUtf8JsonWriterTests()
        {
            this.stream = new MemoryStream();

            try
            {
                this.writer = new ODataUtf8JsonWriter(stream, isIeee754Compatible: true, encoding: Encoding.UTF8, leaveStreamOpen: true);
            }
            catch
            {
                this.stream.Dispose();
                throw;
            }

            this.disposed = false;
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.stream.Dispose();
            (this.writer as ODataUtf8JsonWriter).Dispose();

            this.disposed = true;
        }

        [Fact]
        public void StartPaddingFunctionScopeWritesParenthesis()
        {
            this.writer.StartPaddingFunctionScope();
            Assert.Equal("(", this.ReadStream());
        }

        [Fact]
        public void EndPaddingFunctionScopeWritesParenthesis()
        {
            this.writer.StartPaddingFunctionScope();
            this.writer.EndPaddingFunctionScope();
            Assert.Equal("()", this.ReadStream());
        }

        [Fact]
        public void WritePaddingFunctionNameWritesName()
        {
            this.writer.WritePaddingFunctionName("example");
            Assert.Equal("example", this.ReadStream());
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
        public void WritePrimitiveValueFloatNaN()
        {
            this.VerifyWritePrimitiveValue(float.NaN, "\"NaN\"");
        }

        [Fact]
        public void WritePrimitiveValueFloatPositiveInfinity()
        {
            this.VerifyWritePrimitiveValue(float.PositiveInfinity, "\"INF\"");
        }

        [Fact]
        public void WritePrimitiveValueFloatNegativeInfinity()
        {
            this.VerifyWritePrimitiveValue(float.NegativeInfinity, "\"-INF\"");
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
        public void WritePrimitiveValueLargeString()
        {
            this.VerifyWritePrimitiveValue(
                new string('x', 30000),
                "\"" + new string('x', 30000) + "\""
                );
        }

        [Fact]
        public void WritePrimitiveValueLargeStringWithSpecialChars()
        {
            this.VerifyWritePrimitiveValue(
                new string('x', 20000) + "Foo ия" + new string('x', 10000),
                "\"" + new string('x', 20000) + "Foo ия" + new string('x', 10000) + "\""
                );
        }

        [Fact]
        public void WritePrimitiveValueLargeStringWithSurrogatePairs()
        {
            this.VerifyWritePrimitiveValue(
                new string('x', 2010) + "Foo ия" + char.ConvertFromUtf32(0x1F60A) + new string('x', 10000) + char.ConvertFromUtf32(0x1F60A),
                "\"" + new string('x', 2010) + "Foo ия" + "\\uD83D\\uDE0A" + new string('x', 10000) + "\\uD83D\\uDE0A" + "\""
                );
        }

        [Fact]
        public void WritePrimitiveValueStringWritesNullIfArgumentIsNull()
        {
            this.writer.WriteValue((string)null);
            Assert.Equal("null", this.ReadStream());
        }

        [Theory]
        // Utf8JsonWriter uses uppercase character in unicode literals, i.e. uD800 instead of ud800
        [InlineData("Foo \uD800\udc05 \u00e4", "\"Foo \\uD800\\uDC05 ä\"")]
        [InlineData("Foo \nBar\t\"Baz\"", "\"Foo \\nBar\\t\\\"Baz\\\"\"")]
        [InlineData("Foo ия", "\"Foo ия\"")]
        [InlineData("<script>", "\"<script>\"")]
        public void WritePrimitiveValueStringEscapesStrings(string input, string expectedOutput)
        {
            this.VerifyWritePrimitiveValue(input, expectedOutput);
        }

        [Fact]
        public void WritePrimitiveValueEmptyByteArray()
        {
            this.VerifyWritePrimitiveValue(new byte[] { 0 }, "\"" + Convert.ToBase64String(new byte[] { 0 }) + "\"");
        }

        [Fact]
        public void WritePrimitiveValueByteArrayWritesNullIfArgumentIsNull()
        {
            this.writer.WriteValue((byte[])null);
            Assert.Equal("null", this.ReadStream());
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
        public void WritePrimitiveValueTimeOfDay()
        {
            this.VerifyWritePrimitiveValue(new TimeOfDay(12, 30, 5, 10), "\"12:30:05.0100000\"");
        }

        private void VerifyWritePrimitiveValue<T>(T parameter, string expected)
        {
            this.writer.WritePrimitiveValue(parameter);
            Assert.Equal(expected, this.ReadStream());
        }

        private void VerifyWriterPrimitiveValueWithIeee754Compatible<T>(T parameter, string expected, bool isIeee754Compatible)
        {
            this.writer = new ODataUtf8JsonWriter(this.stream, isIeee754Compatible, Encoding.UTF8);
            this.writer.WritePrimitiveValue(parameter);
            Assert.Equal(expected, this.ReadStream());
        }

        #endregion

        [Fact]
        public void WriteRawValueWritesValue()
        {
            this.writer.WriteRawValue("Raw\t\"Value ия");
            Assert.Equal("Raw\t\"Value ия", this.ReadStream());
        }

        [Fact]
        public void WriteRawValueWritesNothingWhenNull()
        {
            this.writer.WriteRawValue(null);
            Assert.Equal("", this.ReadStream());
        }

        [Fact]
        public void WriteNameWritesName()
        {
            this.writer.StartObjectScope();
            this.writer.WriteName("Name");
            Assert.Equal("{\"Name\":", this.ReadStream());
        }

        [Fact]
        public void WriteNameWritesNameWithObjectMemberSeparator()
        {
            this.writer.StartObjectScope();
            this.writer.WriteName("Name");
            this.writer.WritePrimitiveValue("Sue");
            this.writer.WriteName("Age");
            Assert.Equal("{\"Name\":\"Sue\",\"Age\":", this.ReadStream());
        }

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
            Assert.Equal("{\"Name\":\"Sue\",\"Attributes\":{\"Height\":1.77,\"Weight\":80.7}}", this.ReadStream());
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
            Assert.Equal("{\"Id\":7,\"Name\":\"Sue\"}", this.ReadStream());
        }

        [Fact]
        public void WriteJsonObjectValueWritesPrimitiveCollection()
        {
            var properties = new Dictionary<string, object>
            {
                { "Names", new string[] { "Sue", "Joe", null } }
            };

            this.writer.WriteJsonObjectValue(properties, null);
            Assert.Equal("{\"Names\":[\"Sue\",\"Joe\",null]}", this.ReadStream());
        }

        [Fact]
        public void WriteODataValueWritesODataValue()
        {
            var value = new ODataPrimitiveValue(3.14);
            this.writer.WriteODataValue(value);
            Assert.Equal("3.14", this.ReadStream());
        }

        [Fact]
        public void WriteODataValueWritesNullValue()
        {
            var value = ODataNullValue.Instance;
            this.writer.WriteODataValue(value);
            Assert.Equal("null", this.ReadStream());
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
            Assert.Equal("{\"Name\":\"Sue\",\"Age\":19}", this.ReadStream());
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
            Assert.Equal("[{\"Name\":\"Sue\",\"Age\":19},{\"Name\":\"Joe\",\"Age\":23}]", this.ReadStream());
        }

        #endregion JsonWriter Extension Methods

        #region Support for other encodings

        public static IEnumerable<object[]> Encodings { get; } = new object[][]
           {
                new object[] { Encoding.UTF8 },
                new object[] { Encoding.Unicode },
                new object[] { Encoding.UTF32 },
                new object[] { Encoding.BigEndianUnicode }
           };

        [Theory]
        [MemberData(nameof(Encodings))]
        public void SupportsOtherEncodings(Encoding encoding)
        {
            var collectionValue = new ODataCollectionValue
            {
                Items = new List<ODataResourceValue>
                {
                    new ODataResourceValue
                    {
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Name", Value = "Sue\uD800\udc05 \u00e4" },
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

            this.writer = new ODataUtf8JsonWriter(this.stream, false, encoding);
            this.writer.WriteODataValue(collectionValue);
            Assert.Equal("[{\"Name\":\"Sue\\uD800\\uDC05 ä\",\"Age\":19},{\"Name\":\"Joe\",\"Age\":23}]", this.ReadStream(encoding));
        }

        [Fact]
        public void SupportsAsciiEncodingWhenEscaped()
        {
            var collectionValue = new ODataCollectionValue
            {
                Items = new List<ODataResourceValue>
                {
                    new ODataResourceValue
                    {
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Name", Value = "Sue\uD800\udc05 \u00e4" },
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

            Encoding encoding = Encoding.ASCII;
            this.writer = new ODataUtf8JsonWriter(this.stream, isIeee754Compatible: false, encoding: encoding, encoder: JavaScriptEncoder.Default);
            this.writer.WriteODataValue(collectionValue);
            Assert.Equal("[{\"Name\":\"Sue\\uD800\\uDC05 \\u00E4\",\"Age\":19},{\"Name\":\"Joe\",\"Age\":23}]", this.ReadStream(encoding));
        }

        #endregion Support for other Encodings

        #region Custom JavaScriptEncoder

        [Fact]
        public void AllowsCustomJavaScriptEncoder()
        {
            string input = "test<>\"ия\n\t";
            string expected = "\"test<>\\\"ия\\n\\t\"";

            this.writer = new ODataUtf8JsonWriter(this.stream, false, Encoding.UTF8, encoder: JavaScriptEncoder.UnsafeRelaxedJsonEscaping);
            this.writer.WritePrimitiveValue(input);

            Assert.Equal(expected, this.ReadStream());
        }

        #endregion Custom JavaScriptEncoder

        #region Large strings
        [Fact]
        public void WritesLargeStringsWithEscapingCorrectly()
        {
            string baseString = "Foo 𐀅 ä Foo \nBar\t\"Baz\" Foo ия <script>";
            string baseExpectedString = "Foo \\uD800\\uDC05 ä Foo \\nBar\\t\\\"Baz\\\" Foo ия <script>";
            var inputBuilder = new StringBuilder();
            var expectedBuilder = new StringBuilder();

            expectedBuilder.Append("\"");
            while (inputBuilder.Length < (1024 * 1024))
            {
                inputBuilder.Append(baseString);
                expectedBuilder.Append(baseExpectedString);
            }
            expectedBuilder.Append("\"");

            var input = inputBuilder.ToString();
            var expected = expectedBuilder.ToString();

            this.writer.WritePrimitiveValue(input);

            Assert.Equal(expected, this.ReadStream());
        }

        [Fact]
        public void WritesLargeStringsWithEscapingWithRelaxedEncoderCorrectly()
        {
            string baseString = "test<>\"ия\n\t";
            string baseExpectedString = "test<>\\\"ия\\n\\t";
            var inputBuilder = new StringBuilder();
            var expectedBuilder = new StringBuilder();

            expectedBuilder.Append("\"");
            while (inputBuilder.Length < (1024 * 1024))
            {
                inputBuilder.Append(baseString);
                expectedBuilder.Append(baseExpectedString);
            }
            expectedBuilder.Append("\"");

            var input = inputBuilder.ToString();
            var expected = expectedBuilder.ToString();

            this.writer = new ODataUtf8JsonWriter(this.stream, false, Encoding.UTF8, encoder: JavaScriptEncoder.UnsafeRelaxedJsonEscaping);
            this.writer.WritePrimitiveValue(input);

            Assert.Equal(expected, this.ReadStream());
        }

        #endregion

        [Fact]
        public void FlushesWhenBufferThresholdIsReached()
        {
            using var stream = new MemoryStream();
            // set buffer size to 10, in current implementation, buffer threshold will be 9
            using var jsonWriter = new ODataUtf8JsonWriter(stream, true, Encoding.UTF8, bufferSize: 10, leaveStreamOpen: true);
            jsonWriter.StartArrayScope();
            jsonWriter.WriteValue("well"); // 7 total bytes written: ["well"
            Assert.Equal(0, stream.Length); // should not have flushed since threshold not reached

            jsonWriter.WriteValue(1); // 9 total bytes written: ["well",1
            Assert.Equal(9, stream.Length); // buffer was flushed to stream and cleared due to hitting threshold

            jsonWriter.WriteValue("well");
            Assert.Equal(9, stream.Length); // not yet flushed

            jsonWriter.WriteValue("well");
            Assert.Equal(23, stream.Length); // data flushed. Stream contents: ["well",1,"well","well"

            jsonWriter.WriteValue("Hello, World"); // write data larger than buffer size
            Assert.Equal(38, stream.Length); // stream contents: ["well",1,"well","well","Hello, World"

            jsonWriter.EndArrayScope();
            jsonWriter.Flush();

            Assert.Equal(@"[""well"",1,""well"",""well"",""Hello, World""]", this.ReadStream(jsonWriter, stream, Encoding.UTF8));
        }

        [Fact]
        public void WritesToStreamWhenBufferThresholdIsReached_WithRawValues()
        {
            using var stream = new MemoryStream();
            // set buffer size to 10, in current implementation, buffer threshold will be 9
            using var jsonWriter = new ODataUtf8JsonWriter(stream, true, Encoding.UTF8, bufferSize: 10, leaveStreamOpen: true);
            jsonWriter.StartArrayScope();
            jsonWriter.WriteRawValue(@"""well"""); // 7 total bytes written: ["well"
            Assert.Equal(0, stream.Length); // should not have flushed since threshold not reached

            jsonWriter.WriteValue(1); // 9 total bytes written: ["well",1
            Assert.Equal(9, stream.Length); // buffer was flushed to stream and cleared due to hitting threshold

            jsonWriter.WriteValue("well");
            Assert.Equal(9, stream.Length); // not yet flushed

            jsonWriter.WriteRawValue(@"""well""");
            Assert.Equal(23, stream.Length); // data flushed. Stream contents: ["well",1,"well","well"

            jsonWriter.WriteRawValue(@"""Hello, World"""); // write data larger than buffer size
            Assert.Equal(38, stream.Length); // stream contents: ["well",1,"well","well","Hello, World"

            jsonWriter.EndArrayScope();
            jsonWriter.Flush();

            Assert.Equal(@"[""well"",1,""well"",""well"",""Hello, World""]", this.ReadStream(jsonWriter, stream, Encoding.UTF8));
        }

        [Fact]
        public void FlushingWriterBeforeBufferIsFullShouldBeOkay()
        {
            using var stream = new MemoryStream();
            using var jsonWriter = new ODataUtf8JsonWriter(stream, true, Encoding.UTF8, bufferSize: 10, leaveStreamOpen: true);
            jsonWriter.StartArrayScope();

            jsonWriter.WriteValue("foo");
            Assert.Equal(0, stream.Length); // not yet flushed

            jsonWriter.Flush();
            Assert.Equal(6, stream.Length); // buffer contents: ["foo"

            jsonWriter.WriteValue("fooba");
            Assert.Equal(6, stream.Length); // buffer was reset, so we're still below automatic flush threshold

            jsonWriter.EndArrayScope();

            Assert.Equal(@"[""foo"",""fooba""]", this.ReadStream(jsonWriter, stream, Encoding.UTF8));
        }

        [Fact]
        public void FlushingEmptyWriterShouldBeOkay()
        {
            using var stream = new MemoryStream();
            using var jsonWriter = new ODataUtf8JsonWriter(stream, true, Encoding.UTF8, leaveStreamOpen: true);
            jsonWriter.Flush();
            Assert.Equal(0, stream.Length);
        }

        [Fact]
        public void WritingWhenBufferIsFullShouldNotForceFlush()
        {
            using var finalStream = new MemoryStream();
            using var bufStream = new BufferedStream(finalStream, bufferSize: 1024);

            using var jsonWriter = new ODataUtf8JsonWriter(bufStream, true, Encoding.UTF8, bufferSize: 16, leaveStreamOpen: true);
            jsonWriter.StartArrayScope();

            jsonWriter.WriteValue("foofoo");

            // should not have written to stream because we have not reached the writer's buffer threshold
            Assert.Equal(0, bufStream.Position);
            Assert.Equal(0, finalStream.Position);

            jsonWriter.WriteValue("foofoofoofoo");
            jsonWriter.EndArrayScope();

            // should write to bufStream since we exceeded the threshold
            Assert.Equal(24, bufStream.Position);
            // should not have written to final stream because we have not reached intermediate stream's buffer size
            Assert.Equal(0, finalStream.Position);

            jsonWriter.Flush();
            // should flush content to final stream
            Assert.Equal(@"[""foofoo"",""foofoofoofoo""]", this.ReadStream(jsonWriter, finalStream, Encoding.UTF8));
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
        public void CorrectlyStreams_NonAsciiCharacters_ToOutput(string contentType)
        {
            string input = "😊😊😊😊😊😊😊😊";
            string expectedOutput = "\\uD83D\\uDE0A\\uD83D\\uDE0A\\uD83D\\uDE0A\\uD83D\\uDE0A\\uD83D\\uDE0A\\uD83D\\uDE0A\\uD83D\\uDE0A\\uD83D\\uDE0A";

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
            string input = new string('a', inputLength) + "😀";

            string expectedOutput = new string('a', inputLength) + "\\uD83D\\uDE00";

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
            string expectedOutput = ExpectedOutPutStringWithSpecialCharacters_ODataUtf8Encoding(input);

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

        [Fact]
        public void CorrectlyStreams_NonAsciiCharacters_ToOutput_UsingApplicationJson()
        {
            string input = "😊😊😊😊😊😊😊😊";
            string expectedOutput = "😊😊😊😊😊😊😊😊";

            using (MemoryStream stream = new MemoryStream())
            {
                IJsonWriter jsonWriter = CreateJsonWriter(stream, false, Encoding.UTF8);

                var tw = jsonWriter.StartTextWriterValueScope("application/json");

                WriteSpecialCharsInChunksOfOddStringInChunks(tw, input);

                jsonWriter.EndTextWriterValueScope();
                jsonWriter.Flush();

                stream.Seek(0, SeekOrigin.Begin);

                using (StreamReader reader = new StreamReader(stream, encoding: Encoding.UTF8))
                {
                    string rawOutput = reader.ReadToEnd();
                    Assert.Equal(expectedOutput, rawOutput);
                }
            }
        }


        [Theory]
        // both the escaped and non-escaped versions are valid
        // and compliant JSON parsers should be able to handle both
        [InlineData("application/json", "🐂")]
        [InlineData("text/html", "\"\\uD83D\\uDC02\"")]
        [InlineData("text/plain", "\"\\uD83D\\uDC02\"")]
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

        [Theory]
        [InlineData("utf-8")]
        [InlineData("utf-16")]
        [InlineData("utf-32")]
        [InlineData("us-ascii")]
        [InlineData("iso-8859-1")]
        public void WritesJsonWithCorrectEncoding(string encodingName)
        {
            Encoding encoding = Encoding.GetEncoding(encodingName);
            using MemoryStream stream = new MemoryStream();
            IJsonWriter writer = CreateJsonWriter(stream, isIeee754Compatible: false, encoding);

            writer.StartObjectScope();
            writer.WriteName("greeting");
            writer.WriteValue("hello");
            writer.EndObjectScope();
            writer.Flush();

            var bytes = stream.ToArray();
            var text = encoding.GetString(bytes);
            Assert.Equal("{\"greeting\":\"hello\"}", text);
        }

        [Fact]
        public void WritesUtf8DirectlyIfEncodingIsUtf8()
        {
            Encoding encoding = Encoding.UTF8;
            using MemoryStream stream = new MemoryStream();
            IJsonWriter writer = CreateJsonWriter(stream, isIeee754Compatible: false, encoding);

            writer.StartObjectScope();
            writer.WriteName("utf8");
            writer.WriteValue("✓");
            writer.EndObjectScope();
            writer.Flush();

            var bytes = stream.ToArray();
            var text = encoding.GetString(bytes);
            Assert.Contains("✓", text);
        }

        [Theory]
        [InlineData("utf-16")]
        [InlineData("utf-32")]
        public void WritesUnicodeCharactersWithNonUtf8Encoding(string encodingName)
        {
            Encoding encoding = Encoding.GetEncoding(encodingName);
            using MemoryStream stream = new MemoryStream();
            IJsonWriter writer = CreateJsonWriter(stream, isIeee754Compatible: false, encoding);

            writer.StartObjectScope();
            writer.WriteName("unicode");
            writer.WriteValue("你好");
            writer.EndObjectScope();
            writer.Flush();

            var bytes = stream.ToArray();
            var text = encoding.GetString(bytes);
            Assert.Equal("{\"unicode\":\"你好\"}", text);
        }

        [Fact]
        public async Task AsyncWriteWithEncodingWorks()
        {
            Encoding encoding = Encoding.UTF32;
            using MemoryStream stream = new MemoryStream();
            IJsonWriter writer = CreateJsonWriter(stream, isIeee754Compatible: false, encoding);

            await writer.StartObjectScopeAsync();
            await writer.WriteNameAsync("async");
            await writer.WriteValueAsync("test");
            await writer.EndObjectScopeAsync();
            await writer.FlushAsync();

            var bytes = stream.ToArray();
            var text = encoding.GetString(bytes);
            Assert.Equal("{\"async\":\"test\"}", text);
        }

        [Fact]
        public void WritesRawValueWithEncoding()
        {
            var encoding = Encoding.ASCII;
            using MemoryStream stream = new MemoryStream();
            IJsonWriter writer = CreateJsonWriter(stream, isIeee754Compatible: false, encoding);

            writer.WriteRawValue("12345");
            writer.Flush();

            var bytes = stream.ToArray();
            var text = encoding.GetString(bytes);
            Assert.Contains("12345", text);
        }

        [Fact]
        public void WritesArrayScopeWithEncoding()
        {
            Encoding encoding = Encoding.UTF8;
            using MemoryStream stream = new MemoryStream();
            IJsonWriter writer = CreateJsonWriter(stream, isIeee754Compatible: false, encoding);

            writer.StartArrayScope();
            writer.WriteValue("a");
            writer.WriteValue("b");
            writer.EndArrayScope();
            writer.Flush();

            var bytes = stream.ToArray();
            var text = encoding.GetString(bytes);
            Assert.Equal("[\"a\",\"b\"]", text);
        }

        [Fact]
        public void WritesNullValueWithEncoding()
        {
            Encoding encoding = Encoding.UTF8;
            using MemoryStream stream = new MemoryStream();
            IJsonWriter writer = CreateJsonWriter(stream, isIeee754Compatible: false, encoding);

            writer.StartObjectScope();
            writer.WriteName("nullValue");
            writer.WriteValue((string)null);
            writer.EndObjectScope();
            writer.Flush();

            var bytes = stream.ToArray();
            var text = encoding.GetString(bytes);
            Assert.Equal("{\"nullValue\":null}", text);
        }

        [Theory]
        [MemberData(nameof(GetTextInput))]
        [MemberData(nameof(GetUnicodeText))]
        public void WriteAsync_Works_WhenOutputIs_UTF32(string message)
        {
            Encoding encoding = Encoding.UTF32;

            using MemoryStream stream = new MemoryStream();
            IJsonWriter writer = CreateJsonWriter(stream, isIeee754Compatible: false, encoding);

            writer.WriteValue(message);
            writer.Flush();

            var bytes = stream.ToArray();
            var text = encoding.GetString(bytes);
            Assert.Equal($"\"{message}\"", text);
        }

        [Theory]
        [MemberData(nameof(GetTextInput))]
        [MemberData(nameof(GetUnicodeText))]
        public async Task WriteAsync_Works_WhenOutputIs_UTF32_Async(string message)
        {
            Encoding encoding = Encoding.UTF32;

            using MemoryStream stream = new MemoryStream();
            IJsonWriter writer = CreateJsonWriter(stream, isIeee754Compatible: false, encoding);

            await writer.WriteValueAsync(message);
            await writer.FlushAsync();

            var bytes = stream.ToArray();
            var text = encoding.GetString(bytes);
            Assert.Equal($"\"{message}\"", text);
        }

        [Theory]
        [MemberData(nameof(GetTextInput))]
        [MemberData(nameof(GetUnicodeText))]
        public async Task CreateTranscodingStreamWriteAsync_Works_WhenOutputIs_UTF32(string message)
        {
            Encoding targetEncoding = Encoding.UTF32;
            string expected = JavaScriptEncoder.Default.Encode(message);

            var memoryStream = new MemoryStream();
            var stream = new AsyncStream(memoryStream);

            await using var transcodingStream = Encoding.CreateTranscodingStream(stream, targetEncoding, Encoding.UTF8);
            await transcodingStream.WriteAsync(Encoding.UTF8.GetBytes(JavaScriptEncoder.Default.Encode(message)), default);
            await transcodingStream.FlushAsync();

            string actual = targetEncoding.GetString(memoryStream.ToArray());
            Assert.Equal(expected, actual, StringComparer.OrdinalIgnoreCase);
        }

        [Theory]
        [MemberData(nameof(GetTextInput))]
        [MemberData(nameof(GetUnicodeText))]
        public void WriteAsync_Works_WhenOutputIs_Unicode(string message)
        {
            Encoding encoding = Encoding.Unicode;

            using MemoryStream stream = new MemoryStream();
            IJsonWriter writer = CreateJsonWriter(stream, isIeee754Compatible: false, encoding);

            writer.WriteRawValue(message);
            writer.Flush();

            var bytes = stream.ToArray();
            var text = encoding.GetString(bytes);
            Assert.Equal(message, text);
        }

        [Theory]
        [MemberData(nameof(GetTextInput))]
        [MemberData(nameof(GetUnicodeText))]
        public async Task WriteAsync_Works_WhenOutputIs_Unicode_Async(string message)
        {
            Encoding encoding = Encoding.Unicode;

            using MemoryStream stream = new MemoryStream();
            IJsonWriter writer = CreateJsonWriter(stream, isIeee754Compatible: false, encoding);

            await writer.WriteRawValueAsync(message);
            await writer.FlushAsync();

            var bytes = stream.ToArray();
            var text = encoding.GetString(bytes);
            Assert.Equal(message, text);
        }

        [Theory]
        [MemberData(nameof(GetTextInput))]
        [MemberData(nameof(GetUnicodeText))]
        public async Task CreateTranscodingWriteAsync_Works_WhenOutputIs_Unicode(string message)
        {
            Encoding targetEncoding = Encoding.Unicode;
            string expected = JavaScriptEncoder.Default.Encode(message);

            var memoryStream = new MemoryStream();
            var stream = new AsyncStream(memoryStream);

            await using var transcodingStream = Encoding.CreateTranscodingStream(stream, targetEncoding, Encoding.UTF8);
            await transcodingStream.WriteAsync(Encoding.UTF8.GetBytes(JavaScriptEncoder.Default.Encode(message)), default);
            await transcodingStream.FlushAsync();

            string actual = targetEncoding.GetString(memoryStream.ToArray());
            Assert.Equal(expected, actual, StringComparer.OrdinalIgnoreCase);
        }

        [Theory]
        [MemberData(nameof(GetTextInput))]
        public void WriteAsync_Works_WhenOutputIs_WesternEuropeanEncoding(string message)
        {
            // Arrange
            Encoding encoding = Encoding.GetEncoding(28591);
            using MemoryStream stream = new MemoryStream();
            IJsonWriter writer = CreateJsonWriter(stream, isIeee754Compatible: false, encoding);

            writer.WriteName(message);
            writer.WriteValue("data");
            writer.Flush();

            var bytes = stream.ToArray();
            var text = encoding.GetString(bytes);
            Assert.Equal($"\"{message}\":\"data\"", text);
        }

        [Theory]
        [MemberData(nameof(GetTextInput))]
        public async Task WriteAsync_Works_WhenOutputIs_WesternEuropeanEncoding_Async(string message)
        {
            // Arrange
            Encoding encoding = Encoding.GetEncoding(28591);
            using MemoryStream stream = new MemoryStream();
            IJsonWriter writer = CreateJsonWriter(stream, isIeee754Compatible: false, encoding);

            await writer.WriteNameAsync(message);
            await writer.WriteValueAsync("some-value");
            await writer.FlushAsync();

            var bytes = stream.ToArray();
            var text = encoding.GetString(bytes);
            Assert.Equal($"\"{message}\":\"some-value\"", text);
        }

        [Theory]
        [MemberData(nameof(GetTextInput))]
        public async Task CreateTranscodingWriteAsync_Works_WhenOutputIs_WesternEuropeanEncoding(string message)
        {
            // Arrange
            Encoding targetEncoding = Encoding.GetEncoding(28591);
            string expected = JavaScriptEncoder.Default.Encode(message);

            var memoryStream = new MemoryStream();
            var stream = new AsyncStream(memoryStream);

            await using var transcodingStream = Encoding.CreateTranscodingStream(stream, targetEncoding, Encoding.UTF8);
            await transcodingStream.WriteAsync(Encoding.UTF8.GetBytes(JavaScriptEncoder.Default.Encode(message)), default);
            await transcodingStream.FlushAsync();

            string actual = targetEncoding.GetString(memoryStream.ToArray());
            Assert.Equal(expected, actual, StringComparer.OrdinalIgnoreCase);
        }

        [Theory]
        [MemberData(nameof(GetTextInput))]
        public void WriteAsync_Works_WhenOutputIs_ASCII(string message)
        {
            // Arrange
            Encoding encoding = Encoding.ASCII;
            using MemoryStream stream = new MemoryStream();
            IJsonWriter writer = CreateJsonWriter(stream, isIeee754Compatible: false, encoding);

            writer.WriteName("data");
            writer.WriteValue(message);
            writer.Flush();

            var bytes = stream.ToArray();
            var text = encoding.GetString(bytes);
            Assert.Equal($"\"data\":\"{message}\"", text);
        }

        [Theory]
        [MemberData(nameof(GetTextInput))]
        public async Task WriteAsync_Works_WhenOutputIs_ASCII_Async(string message)
        {
            // Arrange
            Encoding encoding = Encoding.ASCII;
            using MemoryStream stream = new MemoryStream();
            IJsonWriter writer = CreateJsonWriter(stream, isIeee754Compatible: false, encoding);

            await writer.WriteNameAsync("data");
            await writer.WriteValueAsync(message);
            await writer.FlushAsync();

            var bytes = stream.ToArray();
            var text = encoding.GetString(bytes);
            Assert.Equal($"\"data\":\"{message}\"", text);
        }

        [Theory]
        [MemberData(nameof(GetTextInput))]
        public async Task CreateTranscodingStreamWriteAsync_Works_WhenOutputIs_ASCII(string message)
        {
            // Arrange
            Encoding targetEncoding = Encoding.ASCII;
            string expected = JavaScriptEncoder.Default.Encode(message);

            var memoryStream = new MemoryStream();
            var stream = new AsyncStream(memoryStream);

            await using var transcodingStream = Encoding.CreateTranscodingStream(stream, targetEncoding, Encoding.UTF8);
            await transcodingStream.WriteAsync(Encoding.UTF8.GetBytes(JavaScriptEncoder.Default.Encode(message)), default);
            await transcodingStream.FlushAsync();

            string actual = targetEncoding.GetString(memoryStream.ToArray());
            Assert.Equal(expected, actual, StringComparer.OrdinalIgnoreCase);
        }

        [Fact]
        public void WriteAsync_Works_WithEmptyInput()
        {
            Encoding encoding = Encoding.UTF32;
            string message = string.Empty;
            using MemoryStream stream = new MemoryStream();
            IJsonWriter writer = CreateJsonWriter(stream, isIeee754Compatible: false, encoding);

            writer.WriteValue(message);
            writer.Flush();

            var bytes = stream.ToArray();
            var text = encoding.GetString(bytes);
            Assert.Equal("\"\"", text);
        }

        [Fact]
        public async Task WriteAsync_Works_WithEmptyInput_Async()
        {
            Encoding encoding = Encoding.UTF32;
            string message = string.Empty;
            using MemoryStream stream = new MemoryStream();
            IJsonWriter writer = CreateJsonWriter(stream, isIeee754Compatible: false, encoding);

            await writer.WriteValueAsync(message);
            await writer.FlushAsync();

            var bytes = stream.ToArray();
            var text = encoding.GetString(bytes);
            Assert.Equal("\"\"", text);
        }

        [Fact]
        public void WriteAsync_WriteRawValue_Works_WithEmptyInput()
        {
            Encoding encoding = Encoding.UTF32;
            string message = string.Empty;
            using MemoryStream stream = new MemoryStream();
            IJsonWriter writer = CreateJsonWriter(stream, isIeee754Compatible: false, encoding);

            writer.WriteRawValue(message);
            writer.Flush();

            var bytes = stream.ToArray();
            var text = encoding.GetString(bytes);
            Assert.Equal(string.Empty, text);
        }

        [Fact]
        public async Task WriteAsync_WriteRawValue_Works_WithEmptyInput_Async()
        {
            Encoding encoding = Encoding.UTF32;
            string message = string.Empty;
            using MemoryStream stream = new MemoryStream();
            IJsonWriter writer = CreateJsonWriter(stream, isIeee754Compatible: false, encoding);

            await writer.WriteRawValueAsync(message);
            await writer.FlushAsync();

            var bytes = stream.ToArray();
            var text = encoding.GetString(bytes);
            Assert.Equal(string.Empty, text);
        }

        [Fact]
        public async Task Async_WriteAllPrimitiveTypes_Works()
        {
            var stream = new MemoryStream();
            var writer = new ODataUtf8JsonWriter(stream, true, Encoding.UTF8, leaveStreamOpen: true);

            await writer.StartArrayScopeAsync();
            await writer.WriteValueAsync(true);
            await writer.WriteValueAsync(42);
            await writer.WriteValueAsync(42.5f);
            await writer.WriteValueAsync(1234567890123456789L);
            await writer.WriteValueAsync(3.14);
            await writer.WriteValueAsync(Guid.Parse("00000012-0000-0000-0000-012345678900"));
            await writer.WriteValueAsync(42.2m);
            await writer.WriteValueAsync(new DateTimeOffset(2020, 1, 2, 3, 4, 5, TimeSpan.Zero));
            await writer.WriteValueAsync(new TimeSpan(1, 2, 3, 4, 5));
            await writer.WriteValueAsync((byte)7);
            await writer.WriteValueAsync((sbyte)-7);
            await writer.WriteValueAsync("test");
            await writer.WriteValueAsync((byte[])null);
            await writer.WriteValueAsync(new Date(2022, 12, 31));
            await writer.WriteValueAsync(new TimeOfDay(12, 30, 5, 10));
            await writer.EndArrayScopeAsync();
            await writer.FlushAsync();

            stream.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(stream, Encoding.UTF8, leaveOpen: true).ReadToEndAsync();
            Assert.Contains("\"test\"", text);
            Assert.Contains("null", text);
            Assert.Contains("\"00000012-0000-0000-0000-012345678900\"", text);
            Assert.Contains("\"2022-12-31\"", text);
            Assert.Contains("\"12:30:05.0100000\"", text);
        }

        [Fact]
        public async Task Async_WriteJsonElement_Works()
        {
            var stream = new MemoryStream();
            var writer = new ODataUtf8JsonWriter(stream, false, Encoding.UTF8, leaveStreamOpen: true);

            using var doc = JsonDocument.Parse("{\"foo\":123}");
            await writer.WriteValueAsync(doc.RootElement);
            await writer.FlushAsync();

            stream.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(stream, Encoding.UTF8, leaveOpen: true).ReadToEndAsync();
            Assert.Equal("{\"foo\":123}", text);
        }

        [Fact]
        public async Task Async_WriteNullStringValue_WritesNull()
        {
            var stream = new MemoryStream();
            var writer = new ODataUtf8JsonWriter(stream, false, Encoding.UTF8, leaveStreamOpen: true);

            await writer.WriteValueAsync((string)null);
            await writer.FlushAsync();

            stream.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(stream, Encoding.UTF8, leaveOpen: true).ReadToEndAsync();
            Assert.Equal("null", text);
        }

        [Fact]
        public async Task Async_WriteEmptyArrayAndObject_Works()
        {
            var stream = new MemoryStream();
            var writer = new ODataUtf8JsonWriter(stream, false, Encoding.UTF8, leaveStreamOpen: true);

            await writer.StartArrayScopeAsync();
            await writer.EndArrayScopeAsync();
            await writer.StartObjectScopeAsync();
            await writer.EndObjectScopeAsync();
            await writer.FlushAsync();

            stream.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(stream, Encoding.UTF8, leaveOpen: true).ReadToEndAsync();
            Assert.Equal("[],{}", text);
        }

        [Fact]
        public async Task Async_WriteRawValue_Null_DoesNothing()
        {
            var stream = new MemoryStream();
            var writer = new ODataUtf8JsonWriter(stream, false, Encoding.UTF8, leaveStreamOpen: true);

            await writer.WriteRawValueAsync(null);
            await writer.FlushAsync();

            stream.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(stream, Encoding.UTF8, leaveOpen: true).ReadToEndAsync();
            Assert.Equal(string.Empty, text);
        }

        [Fact]
        public async Task Async_FlushAfterPartialWrite_WritesPartialContent()
        {
            var stream = new MemoryStream();
            var writer = new ODataUtf8JsonWriter(stream, false, Encoding.UTF8, leaveStreamOpen: true);

            await writer.StartArrayScopeAsync();
            await writer.WriteValueAsync("foo");
            await writer.FlushAsync();

            stream.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(stream, Encoding.UTF8, leaveOpen: true).ReadToEndAsync();
            Assert.Equal("[\"foo\"", text);
        }

        [Fact]
        public async Task AsyncApi_WritesObjectCorrectly()
        {
            var stream = new MemoryStream();
            var writer = new ODataUtf8JsonWriter(stream, false, Encoding.UTF8, leaveStreamOpen: true);

            await writer.StartObjectScopeAsync();
            await writer.WriteNameAsync("foo");
            await writer.WriteValueAsync("bar");
            await writer.EndObjectScopeAsync();
            await writer.FlushAsync();

            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
            var result = await reader.ReadToEndAsync();
            Assert.Equal("{\"foo\":\"bar\"}", result);
        }

        [Fact]
        public async Task AsyncApi_WritesArrayWithRawValueCorrectly()
        {
            var stream = new MemoryStream();
            var writer = new ODataUtf8JsonWriter(stream, false, Encoding.UTF8, leaveStreamOpen: true);

            await writer.StartArrayScopeAsync();
            await writer.WriteRawValueAsync("\"raw\"");
            await writer.WriteValueAsync(1);
            await writer.WriteValueAsync("foo");
            await writer.EndArrayScopeAsync();
            await writer.FlushAsync();

            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
            var result = await reader.ReadToEndAsync();
            Assert.Equal("[\"raw\",1,\"foo\"]", result);
        }

        [Fact]
        public async Task AsyncApi_WritesLargeByteArrayCorrectly()
        {
            var stream = new MemoryStream();
            var writer = new ODataUtf8JsonWriter(stream, false, Encoding.UTF8, leaveStreamOpen: true);

            var bytes = Enumerable.Repeat((byte)0xAB, 100_000).ToArray();
            await writer.StartArrayScopeAsync();
            await writer.WriteValueAsync(bytes);
            await writer.EndArrayScopeAsync();
            await writer.FlushAsync();

            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
            var result = await reader.ReadToEndAsync();
            // Should be a single base64 string in an array
            Assert.StartsWith("[\"", result);
            Assert.EndsWith("\"]", result);
            var base64 = result.Substring(2, result.Length - 4);
            Assert.Equal(Convert.ToBase64String(bytes), base64);
        }

        [Fact]
        public async Task WriteAsync_Works_WithFlushAndDispose()
        {
            string message = "Flush and Dispose test";
            Encoding inputEncoding = Encoding.UTF8;
            Encoding targetEncoding = Encoding.UTF8;
            var memoryStream = new MemoryStream();
            var stream = new AsyncStream(memoryStream);

            await using (var transcodingStream = Encoding.CreateTranscodingStream(stream, targetEncoding, inputEncoding))
            {
                await transcodingStream.WriteAsync(inputEncoding.GetBytes(message), default);
                await transcodingStream.FlushAsync();
            }

            string actual = targetEncoding.GetString(memoryStream.ToArray());
            Assert.Contains("Flush", actual);
            Assert.Contains("Dispose", actual);
        }

        [Fact]
        public async Task WriteAsync_Throws_OnDisposedStream()
        {
            string message = "Disposed";
            Encoding inputEncoding = Encoding.UTF8;
            Encoding targetEncoding = Encoding.UTF8;
            var memoryStream = new MemoryStream();
            var stream = new AsyncStream(memoryStream);

            var transcodingStream = Encoding.CreateTranscodingStream(stream, targetEncoding, inputEncoding);
            await transcodingStream.DisposeAsync();

            await Assert.ThrowsAsync<ObjectDisposedException>(async () =>
            {
                await transcodingStream.WriteAsync(inputEncoding.GetBytes(message), default);
            });
        }

        [Fact]
        public void WriteRawValue_MultipleInArray_ManualSeparator()
        {
            using var stream = new MemoryStream();
            using var writer = new ODataUtf8JsonWriter(stream, false, Encoding.UTF8, leaveStreamOpen: true);

            writer.StartArrayScope();
            writer.WriteRawValue("1");
            writer.WriteRawValue("2");
            writer.WriteRawValue("3");
            writer.WriteValue("foo");
            writer.EndArrayScope();
            writer.Flush();

            stream.Seek(0, SeekOrigin.Begin);
            var result = new StreamReader(stream).ReadToEnd();
            Assert.Equal("[1,2,3,\"foo\"]", result);
        }

        [Fact]
        public void WriteRawValue_InObject_ManualSeparator()
        {
            using var stream = new MemoryStream();
            using var writer = new ODataUtf8JsonWriter(stream, false, Encoding.UTF8, leaveStreamOpen: true);

            writer.StartObjectScope();
            writer.WriteRawValue("\"raw\"");
            writer.WriteName("foo");
            writer.WriteValue("bar");
            writer.EndObjectScope();
            writer.Flush();

            stream.Seek(0, SeekOrigin.Begin);
            var result = new StreamReader(stream).ReadToEnd();
            // Should be: {"raw","foo":"bar"}
            Assert.Equal("{\"raw\",\"foo\":\"bar\"}", result);
        }

        [Fact]
        public void WriteStringValueInChunks_WritesCorrectly()
        {
            var longString = new string('a', 5000) + "\"\n\t";
            using var stream = new MemoryStream();
            using var writer = new ODataUtf8JsonWriter(stream, false, Encoding.UTF8, bufferSize: 128, leaveStreamOpen: true);

            writer.WriteValue(longString);
            writer.Flush();

            stream.Seek(0, SeekOrigin.Begin);
            var result = new StreamReader(stream).ReadToEnd();
            Assert.StartsWith("\"", result);
            Assert.EndsWith("\"", result);
            Assert.Contains("\\\"", result);
            Assert.Contains("\\n", result);
            Assert.Contains("\\t", result);
        }

        [Fact]
        public void WriteByteValueInChunks_WritesCorrectly()
        {
            var bytes = Enumerable.Repeat((byte)0xFF, 10000).ToArray();
            using var stream = new MemoryStream();
            using var writer = new ODataUtf8JsonWriter(stream, false, Encoding.UTF8, bufferSize: 128, leaveStreamOpen: true);

            writer.WriteValue(bytes);
            writer.Flush();

            stream.Seek(0, SeekOrigin.Begin);
            var result = new StreamReader(stream).ReadToEnd();
            Assert.StartsWith("\"", result);
            Assert.EndsWith("\"", result);
            var base64 = result.Substring(1, result.Length - 2);
            Assert.Equal(Convert.ToBase64String(bytes), base64);
        }

        [Fact]
        public void Dispose_IsIdempotent()
        {
            var stream = new MemoryStream();
            var writer = new ODataUtf8JsonWriter(stream, false, Encoding.UTF8, leaveStreamOpen: true);

            writer.Dispose();
            writer.Dispose(); // Should not throw
        }

        [Fact]
        public async Task DisposeAsync_IsIdempotent()
        {
            var stream = new MemoryStream();
            var writer = new ODataUtf8JsonWriter(stream, false, Encoding.UTF8, leaveStreamOpen: true);

            await writer.DisposeAsync();
            await writer.DisposeAsync(); // Should not throw
        }

        [Fact]
        public void CustomEncoder_AsciiEscaping()
        {
            var input = "abc<>";
            var expected = "\"abc\\u003C\\u003E\"";
            var encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin);
            using var stream = new MemoryStream();
            using var writer = new ODataUtf8JsonWriter(stream, false, Encoding.UTF8, encoder: encoder, leaveStreamOpen: true);

            writer.WriteValue(input);
            writer.Flush();

            stream.Seek(0, SeekOrigin.Begin);
            var result = new StreamReader(stream).ReadToEnd();
            Assert.Equal(expected, result);
        }

        [Fact]
        public void NonUtf8Encoding_WritesCorrectly()
        {
            var encoding = Encoding.Unicode;
            using var stream = new MemoryStream();
            using var writer = new ODataUtf8JsonWriter(stream, false, encoding, leaveStreamOpen: true);

            writer.StartObjectScope();
            writer.WriteName("foo");
            writer.WriteValue("bar");
            writer.EndObjectScope();
            writer.Flush();

            stream.Seek(0, SeekOrigin.Begin);
            var result = new StreamReader(stream, encoding).ReadToEnd();
            Assert.Equal("{\"foo\":\"bar\"}", result);
        }

        public static TheoryData<string> GetTextInput =>
            new TheoryData<string>
            {
                "Hello world",
                string.Join(string.Empty, Enumerable.Repeat("AB", 9000)),
                new string ('A', count: bufferSize),
                new string ('A', count: bufferSize),
                new string ('A', count: bufferSize + 1),
                new string ('A', count: bufferSize + 1),
                new string ('Z', count: bufferSize * 10)
            };

        public static TheoryData<string> GetUnicodeText =>
            new TheoryData<string>
            {
                new string('\u00c6', count: 7),
                new string('A', count: bufferSize - 1) + '\u00c6',
                "Ab\u0100\u0101\u0102\u0103\u0104\u0105\u0106\u014a\u014b\u014c\u014d\u014e\u014f\u0150\u0151\u0152\u0153\u0154\u0155\u0156\u0157\u0158\u0159\u015a\u015f\u0160\u0161\u0162\u0163\u0164\u0165\u0166\u0167\u0168\u0169\u016a\u016b\u016c\u016d\u016e\u016f\u0170\u0171\u0172\u0173\u0174\u0175\u0176\u0177\u0178\u0179\u017a\u017b\u017c\u017d\u017e\u017fAbc",
               "Abc\u0b90\u0b92\u0b93\u0b94\u0b95\u0b99\u0b9a\u0b9c\u0b9e\u0b9f\u0ba3\u0ba4\u0ba8\u0ba9\u0baa\u0bae\u0baf\u0bb0\u0bb1\u0bb2\u0bb3\u0bb4\u0bb5\u0bb7\u0bb8\u0bb9",
               "\u2600\u2601\u2602\u2603\u2604\u2605\u2606\u2607\u2608\u2609\u260a\u260b\u260c\u260d\u260e\u260f\u2610\u2611\u2612\u2613\u261a\u261b\u261c\u261d\u261e\u261f\u2620\u2621\u2622\u2623\u2624\u2625\u2626\u2627\u2628\u2629\u262a\u262b\u262c\u262d\u262e\u262f\u2630\u2631\u2632\u2633\u2634\u2635\u2636\u2637\u2638",
                new string('\u00c6', count: 64 * 1024),
                new string('\u00c6', count: 64 * 1024 + 1),
               "ping\u00fcino",
                new string('\u0904', count: bufferSize + 1), // This uses 3 bytes to represent in UTF8
            };

        /// <summary>
        /// Reads the test class's default stream with UTF8 encoding.
        /// </summary>
        /// <returns>The text content in the stream.</returns>
        private string ReadStream()
        {
            return this.ReadStream(Encoding.UTF8);
        }

        /// <summary>
        /// Reads the test class's default stream with the specified <paramref name="encoding"/>.
        /// </summary>
        /// <param name="encoding">Encoding of the data in the stream.</param>
        /// <returns>The text content in the stream.</returns>
        private string ReadStream(Encoding encoding)
        {
            return this.ReadStream(this.writer, this.stream, encoding);
        }

        /// <summary>
        /// Reads the contents of the specified <paramref name="stream"/>.
        /// </summary>
        /// <param name="writer">The writer that was used to write to the stream.</param>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="encoding">The encoding of the data in the stream.</param>
        /// <returns>The text content in the stream.</returns>
        private string ReadStream(IJsonWriter writer, Stream stream, Encoding encoding)
        {
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            // leave open since the this.stream is disposed separately
            using StreamReader reader = new StreamReader(stream, encoding, leaveOpen: true);
            return reader.ReadToEnd();
        }

        protected override IJsonWriter CreateJsonWriter(Stream stream, bool isIeee754Compatible, Encoding encoding)
        {
            return new ODataUtf8JsonWriter(stream, isIeee754Compatible, encoding);
        }
    }
}

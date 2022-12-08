//---------------------------------------------------------------------
// <copyright file="ODataUtf8JsonWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if NETCOREAPP3_1_OR_GREATER
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
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
            Assert.Equal("null", this.ReadStream());
        }

        [Theory]
        // Utf8JsonWriter uses uppercase character in unicode literals, i.e. uD800 instead of ud800
        [InlineData("Foo \uD800\udc05 \u00e4", "\"Foo \\uD800\\uDC05 \\u00E4\"")]
        // Utf8JsonWriter escapes double-quotes using \u0022
        [InlineData("Foo \nBar\t\"Baz\"", "\"Foo \\nBar\\t\\u0022Baz\\u0022\"")]
        [InlineData("Foo ия", "\"Foo \\u0438\\u044F\"")]
        [InlineData("<script>", "\"\\u003Cscript\\u003E\"")]
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
            var value = new ODataNullValue();
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
                new object[] { Encoding.BigEndianUnicode },
                new object[] { Encoding.ASCII }
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
        public void FlushesWhenBufferThresholdIsReached_WithRawValues()
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

#endif

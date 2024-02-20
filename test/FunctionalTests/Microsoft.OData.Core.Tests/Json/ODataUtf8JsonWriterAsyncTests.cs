﻿//---------------------------------------------------------------------
// <copyright file="ODataUtf8JsonWriterAsyncTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if NETCOREAPP
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    /// <summary>
    /// Unit tests for the ODataUtf8JsonWriter class
    /// </summary>
    public sealed class ODataUtf8JsonWriterAsyncTests: JsonWriterAsyncBaseTests, IAsyncDisposable
    {
        private IJsonWriter writer;
        private Stream stream;
        private bool disposed;

        public ODataUtf8JsonWriterAsyncTests()
        {
            this.stream = new AsyncStream(new MemoryStream());

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

        public async ValueTask DisposeAsync()
        {
            if (this.disposed)
            {
                return;
            }

            await (this.writer as ODataUtf8JsonWriter).DisposeAsync();
            await this.stream.DisposeAsync();
        }

        [Fact]
        public async Task StartPaddingFunctionScopeAsync_WritesParenthesis()
        {
            await this.writer.StartPaddingFunctionScopeAsync();
            Assert.Equal("(", await this.ReadStreamAsync());
        }

        [Fact]
        public async Task EndPaddingFunctionScopeAsync_WritesParenthesis()
        {
            await this.writer.StartPaddingFunctionScopeAsync();
            await this.writer.EndPaddingFunctionScopeAsync();
            Assert.Equal("()", await this.ReadStreamAsync());
        }

        [Fact]
        public async Task WritePaddingFunctionNameAsync_WritesName()
        {
            await this.writer.WritePaddingFunctionNameAsync("example");
            Assert.Equal("example", await this.ReadStreamAsync());
        }

        #region WritePrimitiveValue

        [Fact]
        public async Task WritePrimitiveValueAsync_Boolean()
        {
            await this.VerifyWritePrimitiveValueAsync(false, "false");
        }

        [Fact]
        public async Task WritePrimitiveValueAsync_Byte()
        {
            await this.VerifyWritePrimitiveValueAsync((byte)4, "4");
        }

        [Fact]
        public async Task WritePrimitiveValueAsync_DecimalWithIeee754CompatibleTrue()
        {
            await this.VerifyWriterPrimitiveValueWithIeee754CompatibleAsync(42.2m, "\"42.2\"", isIeee754Compatible: true);
        }

        [Fact]
        public async Task WritePrimitiveValueAsync_DecimalWithIeee754CompatibleFalse()
        {
            await this.VerifyWriterPrimitiveValueWithIeee754CompatibleAsync(42.2m, "42.2", isIeee754Compatible: false);
        }

        [Fact]
        public async Task WritePrimitiveValueAsync_Double()
        {
            await this.VerifyWritePrimitiveValueAsync(42.2d, "42.2");
        }

        [Fact]
        public async Task WritePrimitiveValueAsyncDoubleNaN()
        {
            await this.VerifyWritePrimitiveValueAsync(double.NaN, "\"NaN\"");
        }

        [Fact]
        public async Task WritePrimitiveValueAsyncDoublePositiveInfinity()
        {
            await this.VerifyWritePrimitiveValueAsync(double.PositiveInfinity, "\"INF\"");
        }

        [Fact]
        public async Task WritePrimitiveValueAsyncDoubleNegativeInfinity()
        {
            await this.VerifyWritePrimitiveValueAsync(double.NegativeInfinity, "\"-INF\"");
        }

        [Fact]
        public async Task WritePrimitiveValueAsync_Int16()
        {
            await this.VerifyWritePrimitiveValueAsync((short)876, "876");
        }

        [Fact]
        public async Task WritePrimitiveValueAsync_Int32()
        {
            await this.VerifyWritePrimitiveValueAsync((int)876, "876");
        }

        [Fact]
        public async Task WritePrimitiveValueAsync_Int64WithIeee754CompatibleTrue()
        {
            await this.VerifyWriterPrimitiveValueWithIeee754CompatibleAsync((long)876, "\"876\"", isIeee754Compatible: true);
        }

        [Fact]
        public async Task WritePrimitiveValueAsync_WithIeee754CompatibleFalse()
        {
            await this.VerifyWriterPrimitiveValueWithIeee754CompatibleAsync((long)876, "876", isIeee754Compatible: false);
        }

        [Fact]
        public async Task WritePrimitiveValueAsync_SByte()
        {
            await this.VerifyWritePrimitiveValueAsync((sbyte)4, "4");
        }

        [Fact]
        public async Task WritePrimitiveValueAsync_Single()
        {
            await this.VerifyWritePrimitiveValueAsync((Single)876, "876");
        }

        [Fact]
        public async Task WritePrimitiveValueAsync_String()
        {
            await this.VerifyWritePrimitiveValueAsync("string", "\"string\"");
        }

        [Fact]
        public async Task WritePrimitiveValueAsync_String_WritesNullIfArgumentIsNull()
        {
            await this.writer.WriteValueAsync((string)null);
            Assert.Equal("null", await this.ReadStreamAsync());
        }

        [Theory]
        // Utf8JsonWriter uses uppercase character in unicode literals, i.e. uD800 instead of ud800
        [InlineData("Foo \uD800\udc05 \u00e4", "\"Foo \\uD800\\uDC05 \\u00E4\"")]
        // Utf8JsonWriter escapes double-quotes using \u0022
        [InlineData("Foo \nBar\t\"Baz\"", "\"Foo \\nBar\\t\\u0022Baz\\u0022\"")]
        [InlineData("Foo ия", "\"Foo \\u0438\\u044F\"")]
        [InlineData("<script>", "\"\\u003Cscript\\u003E\"")]
        public async Task WritePrimitiveValueAsync_String_EscapesStrings(string input, string expectedOutput)
        {
            await this.VerifyWritePrimitiveValueAsync(input, expectedOutput);
        }

        [Fact]
        public async Task WritePrimitiveValueAsync_EmptyByteArray()
        {
            await this.VerifyWritePrimitiveValueAsync(new byte[] { 0 }, "\"" + Convert.ToBase64String(new byte[] { 0 }) + "\"");
        }

        [Fact]
        public async Task WritePrimitiveValueAsync_ByteArray_WritesNullIfArgumentIsNull()
        {
            await this.writer.WriteValueAsync((byte[])null);
            Assert.Equal("null", await this.ReadStreamAsync());
        }

        [Fact]
        public async Task WritePrimitiveValueAsync_DateTimeOffset()
        {
            await this.VerifyWritePrimitiveValueAsync(new DateTimeOffset(1, 2, 3, 4, 5, 6, 7, new TimeSpan(1, 2, 0)), "\"0001-02-03T04:05:06.007+01:02\"");
        }

        [Fact]
        public async Task WritePrimitiveValueAsync_DateTimeOffset_WithZeroOffsetWithZeroOffset()
        {
            await this.VerifyWritePrimitiveValueAsync(new DateTimeOffset(1, 2, 3, 4, 5, 6, 7, TimeSpan.Zero), "\"0001-02-03T04:05:06.007Z\"");
        }

        [Fact]
        public async Task WritePrimitiveValueAsync_Guid()
        {
            await this.VerifyWritePrimitiveValueAsync(new Guid("00000012-0000-0000-0000-012345678900"), "\"00000012-0000-0000-0000-012345678900\"");
        }

        [Fact]
        public async Task WritePrimitiveValueAsync_TimeSpan()
        {
            await this.VerifyWritePrimitiveValueAsync(new TimeSpan(1, 2, 3, 4, 5), "\"P1DT2H3M4.005S\"");
        }

        [Fact]
        public async Task WritePrimitiveValueAsync_Date()
        {
            await this.VerifyWritePrimitiveValueAsync(new Date(2014, 12, 31), "\"2014-12-31\"");
        }

        [Fact]
        public async Task WritePrimitiveValueAsync_TimeOfDay()
        {
            await this.VerifyWritePrimitiveValueAsync(new TimeOfDay(12, 30, 5, 10), "\"12:30:05.0100000\"");
        }

        private async Task VerifyWritePrimitiveValueAsync<T>(T parameter, string expected)
        {
            await this.writer.WritePrimitiveValueAsync(parameter);
            Assert.Equal(expected, await this.ReadStreamAsync());
        }

        private async Task VerifyWriterPrimitiveValueWithIeee754CompatibleAsync<T>(T parameter, string expected, bool isIeee754Compatible)
        {
            this.writer = new ODataUtf8JsonWriter(this.stream, isIeee754Compatible, Encoding.UTF8);
            await this.writer.WritePrimitiveValueAsync(parameter);
            Assert.Equal(expected, await this.ReadStreamAsync());
        }

        #endregion

        [Fact]
        public async Task WriteRawValueAsync_WritesValue()
        {
            await this.writer.WriteRawValueAsync("Raw\t\"Value ия");
            Assert.Equal("Raw\t\"Value ия", await this.ReadStreamAsync());
        }

        [Fact]
        public async Task WriteRawValueAsync_WritesNothingWhenNull()
        {
            await this.writer.WriteRawValueAsync(null);
            Assert.Equal("", await this.ReadStreamAsync());
        }

        [Fact]
        public async Task WriteNameAsync_WritesName()
        {
            await this.writer.StartObjectScopeAsync();
            await this.writer.WriteNameAsync("Name");
            Assert.Equal("{\"Name\":", await this.ReadStreamAsync());
        }

        [Fact]
        public async Task WriteNameAsync_WritesNameWithObjectMemberSeparator()
        {
            await this.writer.StartObjectScopeAsync();
            await this.writer.WriteNameAsync("Name");
            await this.writer.WritePrimitiveValueAsync("Sue");
            await this.writer.WriteNameAsync("Age");
            Assert.Equal("{\"Name\":\"Sue\",\"Age\":", await this.ReadStreamAsync());
        }

        #region JsonWriter Extension Methods

        [Fact]
        public async Task WriteJsonObjectValueAsync_WritesJsonObject()
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

            await this.writer.WriteJsonObjectValueAsync(properties, null);
            Assert.Equal("{\"Name\":\"Sue\",\"Attributes\":{\"Height\":1.77,\"Weight\":80.7}}", await this.ReadStreamAsync());
        }

        [Fact]
        public async Task WriteJsonObjectValueAsync_CallsInjectedPropertyAction()
        {
            var properties = new Dictionary<string, object> { { "Name", "Sue" } };
            Func<IJsonWriter, Task> injectPropertyDelegate = async (IJsonWriter actionWriter) =>
            {
                await actionWriter.WriteNameAsync("Id");
                await actionWriter.WriteValueAsync(7);
            };

            await this.writer.WriteJsonObjectValueAsync(properties, injectPropertyDelegate);
            Assert.Equal("{\"Id\":7,\"Name\":\"Sue\"}", await this.ReadStreamAsync());
        }

        [Fact]
        public async Task WriteJsonObjectValueAsync_WritesPrimitiveCollection()
        {
            var properties = new Dictionary<string, object>
            {
                { "Names", new string[] { "Sue", "Joe", null } }
            };

            await this.writer.WriteJsonObjectValueAsync(properties, null);
            Assert.Equal("{\"Names\":[\"Sue\",\"Joe\",null]}", await this.ReadStreamAsync());
        }

        [Fact]
        public async Task WriteODataValueAsync_WritesODataValue()
        {
            var value = new ODataPrimitiveValue(3.14);
            await this.writer.WriteODataValueAsync(value);
            Assert.Equal("3.14", await this.ReadStreamAsync());
        }

        [Fact]
        public async Task WriteODataValueAsync_WritesNullValue()
        {
            var value = new ODataNullValue();
            await this.writer.WriteODataValueAsync(value);
            Assert.Equal("null", await this.ReadStreamAsync());
        }

        [Fact]
        public async Task WriteODataValueAsync_WritesODataResourceValue()
        {
            var resourceValue = new ODataResourceValue
            {
                Properties = new List<ODataProperty>
                {
                    new ODataProperty { Name = "Name", Value = "Sue" },
                    new ODataProperty { Name = "Age", Value = 19 }
                }
            };

            await this.writer.WriteODataValueAsync(resourceValue);
            Assert.Equal("{\"Name\":\"Sue\",\"Age\":19}", await this.ReadStreamAsync());
        }

        [Fact]
        public async Task WriteODataValueAsync_WritesODataCollectionValue()
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

            await this.writer.WriteODataValueAsync(collectionValue);
            Assert.Equal("[{\"Name\":\"Sue\",\"Age\":19},{\"Name\":\"Joe\",\"Age\":23}]", await this.ReadStreamAsync());
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
        public async Task SupportsOtherEncodings(Encoding encoding)
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
            await this.writer.WriteODataValueAsync(collectionValue);
            Assert.Equal("[{\"Name\":\"Sue\\uD800\\uDC05 \\u00E4\",\"Age\":19},{\"Name\":\"Joe\",\"Age\":23}]", await this.ReadStreamAsync(encoding));
        }

        #endregion Support for other Encodings

        #region Custom JavaScriptEncoder

        [Fact]
        public async Task AllowsCustomJavaScriptEncoder()
        {
            string input = "test<>\"ия\n\t";
            string expected = "\"test<>\\\"ия\\n\\t\"";

            this.writer = new ODataUtf8JsonWriter(this.stream, false, Encoding.UTF8, encoder: JavaScriptEncoder.UnsafeRelaxedJsonEscaping);
            await this.writer.WritePrimitiveValueAsync(input);

            Assert.Equal(expected, await this.ReadStreamAsync());
        }

        #endregion Custom JavaScriptEncoder

        [Fact]
        public async Task FlushesWhenBufferThresholdIsReachedAsync()
        {
            using var stream = new MemoryStream();
            // set buffer size to 10, in current implementation, buffer threshold will be 9
            using var jsonWriter = new ODataUtf8JsonWriter(stream, true, Encoding.UTF8, bufferSize: 10, leaveStreamOpen: true);
            await jsonWriter.StartArrayScopeAsync();
            await jsonWriter.WriteValueAsync("well"); // 7 total bytes written: ["well"
            Assert.Equal(0, stream.Length); // should not have flushed since threshold not reached

            await jsonWriter.WriteValueAsync(1); // 9 total bytes written: ["well",1
            Assert.Equal(9, stream.Length); // buffer was flushed to stream and cleared due to hitting threshold

            await jsonWriter.WriteValueAsync("well");
            Assert.Equal(9, stream.Length); // not yet flushed

            await jsonWriter.WriteValueAsync("well");
            Assert.Equal(23, stream.Length); // data flushed. Stream contents: ["well",1,"well","well"

            await jsonWriter.WriteValueAsync("Hello, World"); // write data larger than buffer size
            Assert.Equal(38, stream.Length); // stream contents: ["well",1,"well","well","Hello, World"

            await jsonWriter.EndArrayScopeAsync();
            await jsonWriter.FlushAsync();

            Assert.Equal(@"[""well"",1,""well"",""well"",""Hello, World""]", await this.ReadStreamAsync(jsonWriter, stream, Encoding.UTF8));
        }

        [Fact]
        public async Task FlushesWhenBufferThresholdIsReached_WithRawValues_Async()
        {
            using var stream = new MemoryStream();
            // set buffer size to 10, in current implementation, buffer threshold will be 9
            using var jsonWriter = new ODataUtf8JsonWriter(stream, true, Encoding.UTF8, bufferSize: 10, leaveStreamOpen: true);
            await jsonWriter.StartArrayScopeAsync();
            await jsonWriter.WriteRawValueAsync(@"""well"""); // 7 total bytes written: ["well"
            Assert.Equal(0, stream.Length); // should not have flushed since threshold not reached

            await jsonWriter.WriteValueAsync(1); // 9 total bytes written: ["well",1
            Assert.Equal(9, stream.Length); // buffer was flushed to stream and cleared due to hitting threshold

            await jsonWriter.WriteValueAsync("well");
            Assert.Equal(9, stream.Length); // not yet flushed

            await jsonWriter.WriteRawValueAsync(@"""well""");
            Assert.Equal(23, stream.Length); // data flushed. Stream contents: ["well",1,"well","well"

            await jsonWriter.WriteRawValueAsync(@"""Hello, World"""); // write data larger than buffer size
            Assert.Equal(38, stream.Length); // stream contents: ["well",1,"well","well","Hello, World"

            await jsonWriter.EndArrayScopeAsync();
            await jsonWriter.FlushAsync();

            Assert.Equal(@"[""well"",1,""well"",""well"",""Hello, World""]", await this.ReadStreamAsync(jsonWriter, stream, Encoding.UTF8));
        }

        [Fact]
        public async Task FlushingWriterAsyncBeforeBufferIsFullShouldBeOkay()
        {
            using var stream = new MemoryStream();
            using var jsonWriter = new ODataUtf8JsonWriter(stream, true, Encoding.UTF8, bufferSize: 10, leaveStreamOpen: true);
            await jsonWriter.StartArrayScopeAsync();

            await jsonWriter.WriteValueAsync("foo");
            Assert.Equal(0, stream.Length); // not yet flushed

            await jsonWriter.FlushAsync();
            Assert.Equal(6, stream.Length); // buffer contents: ["foo"

            await jsonWriter.WriteValueAsync("fooba");
            Assert.Equal(6, stream.Length); // buffer was reset, so we're still below automatic flush threshold

            await jsonWriter.EndArrayScopeAsync();

            Assert.Equal(@"[""foo"",""fooba""]", await this.ReadStreamAsync(jsonWriter, stream, Encoding.UTF8));
        }

        [Fact]
        public async Task FlushingEmptyWriterAsyncShouldBeOkay()
        {
            using var stream = new MemoryStream();
            using var jsonWriter = new ODataUtf8JsonWriter(stream, true, Encoding.UTF8, leaveStreamOpen: true);
            await jsonWriter.FlushAsync();
            Assert.Equal(0, stream.Length);
        }

        /// <summary>
        /// Reads the test class's default stream with UTF8 encoding.
        /// </summary>
        /// <returns>The text content in the stream.</returns>
        private Task<string> ReadStreamAsync()
        {
            return this.ReadStreamAsync(Encoding.UTF8);
        }

        /// <summary>
        /// Reads the test class's default stream with the specified <paramref name="encoding"/>.
        /// </summary>
        /// <param name="encoding">Encoding of the data in the stream.</param>
        /// <returns>The text content in the stream.</returns>
        private Task<string> ReadStreamAsync(Encoding encoding)
        {
            return this.ReadStreamAsync(this.writer, this.stream, encoding);
        }

        /// <summary>
        /// Reads the contents of the specified <paramref name="stream"/>.
        /// </summary>
        /// <param name="writer">The writer that was used to write to the stream.</param>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="encoding">The encoding of the data in the stream.</param>
        /// <returns>The text content in the stream.</returns>
        private async Task<string> ReadStreamAsync(IJsonWriter writer, Stream stream, Encoding encoding)
        {
            await writer.FlushAsync();
            stream.Seek(0, SeekOrigin.Begin);
            // leave open since the this.stream is disposed separately
            using StreamReader reader = new StreamReader(stream, encoding, leaveOpen: true);
            string contents = await reader.ReadToEndAsync();
            return contents;
        }

        protected override IJsonWriter CreateJsonWriter(Stream stream, bool isIeee754Compatible, Encoding encoding)
        {
            return new ODataUtf8JsonWriter(stream, isIeee754Compatible, encoding);
        }
    }
}

#endif

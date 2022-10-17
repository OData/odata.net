//---------------------------------------------------------------------
// <copyright file="ODataUtf8JsonWriterAsyncTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if NETCOREAPP3_1_OR_GREATER
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
    public sealed class ODataUtf8JsonWriterAsyncTests: JsonWriterAsyncBaseTests, IDisposable
    {
        private IJsonWriterAsync writer;
        private MemoryStream stream;
        private bool disposed;

        public ODataUtf8JsonWriterAsyncTests()
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
            Func<IJsonWriterAsync, Task> injectPropertyDelegate = async (IJsonWriterAsync actionWriter) =>
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

        private Task<string> ReadStreamAsync()
        {
            return this.ReadStreamAsync(Encoding.UTF8);
        }

        private async Task<string> ReadStreamAsync(Encoding encoding)
        {
            await this.writer.FlushAsync();
            this.stream.Seek(0, SeekOrigin.Begin);
            // leave open since the this.stream is disposed separately
            using StreamReader reader = new StreamReader(this.stream, encoding, leaveOpen: true);
            string result = await reader.ReadToEndAsync();
            return result;
        }

        protected override IJsonWriterAsync CreateJsonWriterAsync(Stream stream, bool isIeee754Compatible, Encoding encoding)
        {
            return new ODataUtf8JsonWriter(stream, isIeee754Compatible, encoding);
        }
    }
}

#endif

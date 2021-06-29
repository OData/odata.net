//---------------------------------------------------------------------
// <copyright file="JsonWriterAsyncTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    /// <summary>
    /// Unit tests for the JsonWriter class.
    /// </summary>
    public class JsonWriterAsyncTests
    {
        private StringBuilder builder;
        private IJsonWriterAsync writer;

        public JsonWriterAsyncTests()
        {
            this.builder = new StringBuilder();
            this.writer = new JsonWriter(new StringWriter(builder), isIeee754Compatible: true);
        }

        [Fact]
        public async Task StartPaddingFunctionScopeAsyncWritesParenthesis()
        {
            await this.writer.StartPaddingFunctionScopeAsync();
            Assert.Equal("(", this.builder.ToString());
        }

        [Fact]
        public async Task EndPaddingFunctionScopeAsyncWritesParenthesis()
        {
            await this.writer.StartPaddingFunctionScopeAsync();
            await this.writer.EndPaddingFunctionScopeAsync();
            Assert.Equal("()", this.builder.ToString());
        }

        [Fact]
        public async Task WritePaddingFunctionNameAsyncWritesName()
        {
            await this.writer.WritePaddingFunctionNameAsync("example");
            Assert.Equal("example", this.builder.ToString());
        }

        [Fact]
        public async Task StartObjectScopeAsyncWritesCurlyBraces()
        {
            await this.writer.StartObjectScopeAsync();
            Assert.Equal("{", this.builder.ToString());
        }

        [Fact]
        public async Task EndObjectScopeAsyncCurlyBraces()
        {
            await this.writer.StartObjectScopeAsync();
            await this.writer.EndObjectScopeAsync();
            Assert.Equal("{}", this.builder.ToString());
        }

        [Fact]
        public async Task StartArrayScopeAsyncWritesSquareBrackets()
        {
            await this.writer.StartArrayScopeAsync();
            Assert.Equal("[", this.builder.ToString());
        }

        [Fact]
        public async Task EndArrayScopeAsyncSquareBrackets()
        {
            await this.writer.StartArrayScopeAsync();
            await this.writer.EndArrayScopeAsync();
            Assert.Equal("[]", this.builder.ToString());
        }

        [Fact]
        public async Task WriteArrayElementSeparatorAsync()
        {
            await this.writer.StartArrayScopeAsync();
            await this.writer.StartObjectScopeAsync();
            await this.writer.EndObjectScopeAsync();
            await this.writer.StartObjectScopeAsync();
            Assert.Equal("[{},{", this.builder.ToString());
        }

        [Fact]
        public async Task WriteNameAsyncWritesName()
        {
            await this.writer.StartObjectScopeAsync();
            await this.writer.WriteNameAsync("Name");
            Assert.Equal("{\"Name\":", this.builder.ToString());
        }

        [Fact]
        public async Task WriteNameAsyncWritesNameWithObjectMemberSeparator()
        {
            await this.writer.StartObjectScopeAsync();
            await this.writer.WriteNameAsync("Name");
            await this.writer.WritePrimitiveValueAsync("Sue");
            await this.writer.WriteNameAsync("Age");
            Assert.Equal("{\"Name\":\"Sue\",\"Age\":", this.builder.ToString());
        }

        [Fact]
        public async Task WriteRawValueAsyncWritesValue()
        {
            await this.writer.WriteRawValueAsync("Raw\tValue");
            await this.writer.FlushAsync();
            Assert.Equal("Raw\tValue", this.builder.ToString());
        }

        [Fact]
        public async Task TestStartTextWriterValueScopeAsync()
        {
            var stringBuilder = new StringBuilder();
            var jsonWriter = new JsonWriter(new StringWriter(stringBuilder), isIeee754Compatible: true);
            var jsonTextWriter = await jsonWriter.StartTextWriterValueScopeAsync(MimeConstants.MimeTextPlain);
            await jsonTextWriter.WriteLineAsync("Быстрая коричневая лиса прыгает через ленивую собаку");
            await jsonWriter.EndTextWriterValueScopeAsync();

            Assert.Equal("\"Быстрая коричневая лиса прыгает через ленивую собаку\"", stringBuilder.ToString());
        }

        #region JsonWriter Extension Methods

        [Fact]
        public async Task WriteJsonObjectValueAsyncWritesJsonObject()
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
            Assert.Equal("{\"Name\":\"Sue\",\"Attributes\":{\"Height\":1.77,\"Weight\":80.7}}", this.builder.ToString());
        }

        [Fact]
        public async Task WriteJsonObjectValueAsyncCallsInjectedPropertyAction()
        {
            var properties = new Dictionary<string, object> { { "Name", "Sue" } };
            Func<IJsonWriterAsync, Task> injectPropertyDelegate = async (IJsonWriterAsync actionWriter) =>
            {
                await actionWriter.WriteNameAsync("Id");
                await actionWriter.WriteValueAsync(7);
            };
            await this.writer.WriteJsonObjectValueAsync(properties, injectPropertyDelegate);
            Assert.Equal("{\"Id\":7,\"Name\":\"Sue\"}", this.builder.ToString());
        }

        [Fact]
        public async Task WriteJsonObjectValueAsyncWritesPrimitiveCollection()
        {
            var properties = new Dictionary<string, object>
            {
                { "Names", new string[] { "Sue", "Joe", null } }
            };
            await this.writer.WriteJsonObjectValueAsync(properties, null);
            Assert.Equal("{\"Names\":[\"Sue\",\"Joe\",null]}", this.builder.ToString());
        }

        [Fact]
        public async Task WriteODataValueAsyncWritesODataValue()
        {
            var value = new ODataPrimitiveValue(3.14);
            await this.writer.WriteODataValueAsync(value);
            Assert.Equal("3.14", this.builder.ToString());
        }

        [Fact]
        public async Task WriteODataValueAsyncWritesNullValue()
        {
            var value = new ODataNullValue();
            await this.writer.WriteODataValueAsync(value);
            Assert.Equal("null", this.builder.ToString());
        }

        [Fact]
        public async Task WriteODataValueAsyncWritesODataResourceValue()
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
            Assert.Equal("{\"Name\":\"Sue\",\"Age\":19}", this.builder.ToString());
        }

        [Fact]
        public async Task WriteODataValueAsyncWritesODataCollectionValue()
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
            Assert.Equal("[{\"Name\":\"Sue\",\"Age\":19},{\"Name\":\"Joe\",\"Age\":23}]", this.builder.ToString());
        }

        #endregion JsonWriter Extension Methods

        #region WritePrimitiveValue

        [Fact]
        public async Task WritePrimitiveValueAsyncBoolean()
        {
            await this.VerifyWritePrimitiveValueAsync(false, "false");
        }

        [Fact]
        public async Task WritePrimitiveValueAsyncByte()
        {
            await this.VerifyWritePrimitiveValueAsync((byte)4, "4");
        }

        [Fact]
        public async Task WritePrimitiveValueAsyncDecimalWithIeee754CompatibleTrue()
        {
            await this.VerifyWriterPrimitiveValueWithIeee754CompatibleAsync(42.2m, "\"42.2\"", isIeee754Compatible: true);
        }

        [Fact]
        public async Task WritePrimitiveValueAsyncDecimalWithIeee754CompatibleFalse()
        {
            await this.VerifyWriterPrimitiveValueWithIeee754CompatibleAsync(42.2m, "42.2", isIeee754Compatible: false);
        }

        [Fact]
        public async Task WritePrimitiveValueAsyncDouble()
        {
            await this.VerifyWritePrimitiveValueAsync(42.2d, "42.2");
        }

        [Fact]
        public async Task WritePrimitiveValueAsyncInt16()
        {
            await this.VerifyWritePrimitiveValueAsync((short)876, "876");
        }

        [Fact]
        public async Task WritePrimitiveValueAsyncInt32()
        {
            await this.VerifyWritePrimitiveValueAsync((int)876, "876");
        }

        [Fact]
        public async Task WritePrimitiveValueAsyncInt64WithIeee754CompatibleTrue()
        {
            await this.VerifyWriterPrimitiveValueWithIeee754CompatibleAsync((long)876, "\"876\"", isIeee754Compatible: true);
        }

        [Fact]
        public async Task WritePrimitiveValueAsyncInt64WithIeee754CompatibleFalse()
        {
            await this.VerifyWriterPrimitiveValueWithIeee754CompatibleAsync((long)876, "876", isIeee754Compatible: false);
        }

        [Fact]
        public async Task WritePrimitiveValueAsyncSByte()
        {
            await this.VerifyWritePrimitiveValueAsync((sbyte)4, "4");
        }

        [Fact]
        public async Task WritePrimitiveValueAsyncSingle()
        {
            await this.VerifyWritePrimitiveValueAsync((Single)876, "876");
        }

        [Fact]
        public async Task WritePrimitiveValueAsyncString()
        {
            await this.VerifyWritePrimitiveValueAsync("string", "\"string\"");
        }

        [Fact]
        public async Task WritePrimitiveValueAsyncByteArray()
        {
            await this.VerifyWritePrimitiveValueAsync(new byte[] { 0 }, "\"" + Convert.ToBase64String(new byte[] { 0 }) + "\"");
        }

        [Fact]
        public async Task WritePrimitiveValueAsyncDateTimeOffset()
        {
            await this.VerifyWritePrimitiveValueAsync(new DateTimeOffset(1, 2, 3, 4, 5, 6, 7, new TimeSpan(1, 2, 0)), "\"0001-02-03T04:05:06.007+01:02\"");
        }

        [Fact]
        public async Task WritePrimitiveValueAsyncGuid()
        {
            await this.VerifyWritePrimitiveValueAsync(new Guid("00000012-0000-0000-0000-012345678900"), "\"00000012-0000-0000-0000-012345678900\"");
        }

        [Fact]
        public async Task WritePrimitiveValueAsyncTimeSpan()
        {
            await this.VerifyWritePrimitiveValueAsync(new TimeSpan(1, 2, 3, 4, 5), "\"P1DT2H3M4.005S\"");
        }

        [Fact]
        public async Task WritePrimitiveValueAsyncDate()
        {
            await this.VerifyWritePrimitiveValueAsync(new Date(2014, 12, 31), "\"2014-12-31\"");
        }

        [Fact]
        public async Task WritePrimitiveValueAsyncTimeOfDay()
        {
            await this.VerifyWritePrimitiveValueAsync(new TimeOfDay(12, 30, 5, 10), "\"12:30:05.0100000\"");
        }

        private async Task VerifyWritePrimitiveValueAsync<T>(T parameter, string expected)
        {
            await this.writer.WritePrimitiveValueAsync(parameter);
            Assert.Equal(expected, this.builder.ToString());
        }

        private async Task VerifyWriterPrimitiveValueWithIeee754CompatibleAsync<T>(T parameter, string expected, bool isIeee754Compatible)
        {
            this.writer = new JsonWriter(new StringWriter(builder), isIeee754Compatible);
            await this.writer.WritePrimitiveValueAsync(parameter);
            Assert.Equal(expected, this.builder.ToString());
        }

        #endregion

        [Fact]
        public Task WriteNameAsyncUsesProvidedCharArrayPool()
        {
            // Note: CharArrayPool is used if string has special chars
            // This test is mostly theoretical since special characters are not allowed in names
            return SetupJsonWriterRunTestAndVerifyRentAsync(
                (jsonWriter) => jsonWriter.WriteNameAsync("foo\tbar"));
        }

        [Fact]
        public Task WriteStringValueUsesProvidedCharArrayPool()
        {
            return SetupJsonWriterRunTestAndVerifyRentAsync(
                (jsonWriter) => jsonWriter.WriteValueAsync("foo\tbar"));
        }

        private async Task SetupJsonWriterRunTestAndVerifyRentAsync(Func<JsonWriter, Task> func)
        {
            var jsonWriter = new JsonWriter(new StringWriter(builder), isIeee754Compatible: true);
            bool rentVerified = false;

            Action<int> rentVerifier = (minSize) => { rentVerified = true; };
            jsonWriter.ArrayPool = new MockCharArrayPool { RentVerifier = rentVerifier };

            await jsonWriter.StartObjectScopeAsync();
            await func(jsonWriter);

            Assert.True(rentVerified);
        }
    }
}

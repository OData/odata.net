//---------------------------------------------------------------------
// <copyright file="ReorderingJsonReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.OData.Core;
using Microsoft.OData.Json;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    public class ReorderingJsonReaderTests
    {
        [Fact]
        public void TypeShouldBeMovedToTop()
        {
            var json = @"
            { 
                ""@odata.editlink"":""RelativeUrl"",
                ""@foo.bla"": 4,
                ""@odata.type"": ""SomeEntityType""
            }";

            var reader = CreateReorderingReaderPositionedOnFirstProperty(json);
            Assert.Equal("@odata.type", reader.ReadPropertyName());
            Assert.Equal("SomeEntityType", reader.ReadPrimitiveValue());
        }

        [Fact]
        public void ETagShouldBeMovedToTop()
        {
            var json = @"
            { 
                ""@odata.editlink"":""RelativeUrl"",
                ""@foo.bla"": 4,
                ""@odata.etag"": ""etag-val""
            }";

            var reader = CreateReorderingReaderPositionedOnFirstProperty(json);
            Assert.Equal("@odata.etag", reader.ReadPropertyName());
            Assert.Equal("etag-val", reader.ReadPrimitiveValue());
        }

        [Fact]
        public void IdShouldBeMovedToTop()
        {
            var json = @"
            { 
                ""@odata.editlink"":""RelativeUrl"",
                ""@foo.bla"": 4,
                ""@odata.id"": 42
            }";

            var reader = CreateReorderingReaderPositionedOnFirstProperty(json);
            Assert.Equal("@odata.id", reader.ReadPropertyName());
            Assert.Equal(42, reader.ReadPrimitiveValue());
        }

        [Fact]
        public void CorrectOrderShouldBeTypeThenIdThenEtag()
        {
            const string json = @"
            { 
                ""@odata.editlink"":""RelativeUrl"",
                ""@foo.bla"": 4,
                ""data"": 3.1,
                ""@odata.etag"": ""etag-val"",
                ""@odata.type"": ""SomeEntityType"",
                ""@odata.id"": 42
            }";

            var reader = CreateReorderingReaderPositionedOnFirstProperty(json);

            // Expect type name first.
            Assert.Equal("@odata.type", reader.ReadPropertyName());
            Assert.Equal("SomeEntityType", reader.ReadPrimitiveValue());

            // Per the protocol, odata.id and odata.etag can be in either order relative to each other,
            // but we'll (arbitarily) lock down "id" before "etag" for our reordering reader.
            Assert.Equal("@odata.id", reader.ReadPropertyName());
            Assert.Equal(42, reader.ReadPrimitiveValue());
            Assert.Equal("@odata.etag", reader.ReadPropertyName());
            Assert.Equal("etag-val", reader.ReadPrimitiveValue());
        }

        [Fact]
        public async Task ReadReorderedPayloadAsync()
        {
            var payload = "{\"Id\":1," +
                "\"Name@attr.maxlength\":32," +
                "\"Name\":\"Sue\"," +
                "\"Photo@odata.mediaReadLink\":\"http://tempuri.org/Customers(1)/Images/Photo\"," +
                "\"Photo@odata.mediaEditLink\":\"http://tempuri.org/Customers(1)/Images/Photo\"," +
                "\"Photo\":\"AQIDBAUGBwgJAA==\"," +
                "\"Orders@odata.count\":1," +
                "\"Photo@attr.alt\":\"Profile Photo\"," +
                "\"Photo@odata.mediaContentType\":\"image/png\"," +
                "\"@odata.deltaLink\":\"http://tempuri.org/Customers(1)/deltaLink\"," +
                "\"@odata.readLink\":\"http://tempuri.org/Customers(1)/readLink\"," +
                "\"@attr.note\":\"Platinum Partner\"," +
                "\"@odata.editLink\":\"http://tempuri.org/Customers(1)/editLink\"," +
                "\"@odata.mediaContentType\":\"image/png\"," +
                "\"Name@odata.type\":\"#Edm.String\"," +
                "\"@odata.etag\":\"etag\"," +
                "\"@odata.id\":\"http://tempuri.org/Customers(1)\"," +
                "\"@odata.type\":\"#NS.Customer\"," +
                "\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Orders@odata.nextLink\":\"http://tempuri.org/Customers(1)/Orders/nextLink\"," +
                "\"Orders@odata.delta\":[{\"@odata.id\":\"http://tempuri.org/Orders(1)\",\"Id\":1,\"Amount\":130}]," +
                "\"Orders@odata.navigationLink\":\"http://tempuri.org/Customers(1)/Orders/navigationLink\"," +
                "\"Orders@odata.type\":\"#Collection(NS.Order)\"," +
                "\"Photo@odata.mediaEtag\":\"media-etag\"," +
                "\"#NS.RateCustomer\":{\"title\":\"RateCustomer\",\"target\":\"http://tempuri.org/Customers(1)\"}}";

            await SetupReorderingJsonReaderAndRunTestAsync(
                payload,
                async (reorderingReader) =>
                {
                    Assert.Equal(
                        "@odata.context",
                        await reorderingReader.ReadPropertyNameAsync());
                    Assert.Equal(
                        "http://tempuri.org/$metadata#Customers/$entity",
                        await reorderingReader.ReadPrimitiveValueAsync());
                    Assert.Equal(
                        "@odata.type",
                        await reorderingReader.ReadPropertyNameAsync());
                    Assert.Equal(
                        "#NS.Customer",
                        await reorderingReader.ReadPrimitiveValueAsync());
                    Assert.Equal(
                        "@odata.id",
                        await reorderingReader.ReadPropertyNameAsync());
                    Assert.Equal(
                        "http://tempuri.org/Customers(1)",
                        await reorderingReader.ReadPrimitiveValueAsync());
                    Assert.Equal(
                        "@odata.etag",
                        await reorderingReader.ReadPropertyNameAsync());
                    Assert.Equal(
                        "etag",
                        await reorderingReader.ReadPrimitiveValueAsync());
                    Assert.Equal(
                        "@odata.deltaLink",
                        await reorderingReader.ReadPropertyNameAsync());
                    Assert.Equal(
                        "http://tempuri.org/Customers(1)/deltaLink",
                        await reorderingReader.ReadPrimitiveValueAsync());
                    Assert.Equal(
                        "@odata.readLink",
                        await reorderingReader.ReadPropertyNameAsync());
                    Assert.Equal(
                        "http://tempuri.org/Customers(1)/readLink",
                        await reorderingReader.ReadPrimitiveValueAsync());
                    Assert.Equal(
                        "@odata.editLink",
                        await reorderingReader.ReadPropertyNameAsync());
                    Assert.Equal(
                        "http://tempuri.org/Customers(1)/editLink",
                        await reorderingReader.ReadPrimitiveValueAsync());
                    Assert.Equal(
                        "@odata.mediaContentType",
                        await reorderingReader.ReadPropertyNameAsync());
                    Assert.Equal(
                        "image/png",
                        await reorderingReader.ReadPrimitiveValueAsync());
                    Assert.Equal(
                        "Id",
                        await reorderingReader.ReadPropertyNameAsync());
                    Assert.Equal(
                        1,
                        await reorderingReader.ReadPrimitiveValueAsync());
                    Assert.Equal(
                        "Name@odata.type",
                        await reorderingReader.ReadPropertyNameAsync());
                    Assert.Equal(
                        "#Edm.String",
                        await reorderingReader.ReadPrimitiveValueAsync());
                    Assert.Equal(
                        "Name@attr.maxlength",
                        await reorderingReader.ReadPropertyNameAsync());
                    Assert.Equal(
                        32,
                        await reorderingReader.ReadPrimitiveValueAsync());
                    Assert.Equal(
                        "Name",
                        await reorderingReader.ReadPropertyNameAsync());
                    Assert.Equal(
                        "Sue",
                        await reorderingReader.ReadPrimitiveValueAsync());
                    Assert.Equal(
                        "Photo@odata.mediaReadLink",
                        await reorderingReader.ReadPropertyNameAsync());
                    Assert.Equal(
                        "http://tempuri.org/Customers(1)/Images/Photo",
                        await reorderingReader.ReadPrimitiveValueAsync());
                    Assert.Equal(
                        "Photo@odata.mediaEditLink",
                        await reorderingReader.ReadPropertyNameAsync());
                    Assert.Equal(
                        "http://tempuri.org/Customers(1)/Images/Photo",
                        await reorderingReader.ReadPrimitiveValueAsync());
                    Assert.Equal(
                        "Photo@odata.mediaContentType",
                        await reorderingReader.ReadPropertyNameAsync());
                    Assert.Equal(
                        "image/png",
                        await reorderingReader.ReadPrimitiveValueAsync());
                    Assert.Equal(
                        "Photo@odata.mediaEtag",
                        await reorderingReader.ReadPropertyNameAsync());
                    Assert.Equal(
                        "media-etag",
                        await reorderingReader.ReadPrimitiveValueAsync());
                    Assert.Equal(
                        "Photo",
                        await reorderingReader.ReadPropertyNameAsync());
                    Assert.Equal(
                        "AQIDBAUGBwgJAA==",
                        await reorderingReader.ReadPrimitiveValueAsync());
                    Assert.Equal(
                        "Photo@attr.alt",
                        await reorderingReader.ReadPropertyNameAsync());
                    Assert.Equal(
                        "Profile Photo",
                        await reorderingReader.ReadPrimitiveValueAsync());
                    Assert.Equal(
                        "Orders@odata.count",
                        await reorderingReader.ReadPropertyNameAsync());
                    Assert.Equal(
                        1,
                        await reorderingReader.ReadPrimitiveValueAsync());
                    Assert.Equal(
                        "Orders@odata.nextLink",
                        await reorderingReader.ReadPropertyNameAsync());
                    Assert.Equal(
                        "http://tempuri.org/Customers(1)/Orders/nextLink",
                        await reorderingReader.ReadPrimitiveValueAsync());
                    Assert.Equal(
                        "Orders@odata.delta",
                        await reorderingReader.ReadPropertyNameAsync());
                    await reorderingReader.SkipValueAsync(); // Skip array value
                    Assert.Equal(
                        "Orders@odata.navigationLink",
                        await reorderingReader.ReadPropertyNameAsync());
                    Assert.Equal(
                        "http://tempuri.org/Customers(1)/Orders/navigationLink",
                        await reorderingReader.ReadPrimitiveValueAsync());
                    Assert.Equal(
                        "Orders@odata.type",
                        await reorderingReader.ReadPropertyNameAsync());
                    Assert.Equal(
                        "#Collection(NS.Order)",
                        await reorderingReader.ReadPrimitiveValueAsync());
                    Assert.Equal(
                        "@attr.note",
                        await reorderingReader.ReadPropertyNameAsync());
                    Assert.Equal(
                        "Platinum Partner",
                        await reorderingReader.ReadPrimitiveValueAsync());
                    Assert.Equal(
                        "#NS.RateCustomer",
                        await reorderingReader.ReadPropertyNameAsync());
                    await reorderingReader.SkipValueAsync(); // Skip object value
                    Assert.Equal(JsonNodeType.EndObject, reorderingReader.NodeType);
                });
        }

        [Fact]
        public async Task ReadReorderedPayloadContainingODataRemovedAnnotationAsync()
        {
            var payload = "{" +
                "\"Id\":1," +
                "\"@odata.id\":\"http://tempuri.org/Customers(1)\"," +
                "\"Name\":\"Sue\"," +
                "\"@odata.removed\":{\"reason\":\"deleted\"}}";

            await SetupReorderingJsonReaderAndRunTestAsync(
                payload,
                async (reorderingReader) =>
                {
                    Assert.Equal(
                        "@odata.removed",
                        await reorderingReader.ReadPropertyNameAsync());
                    await reorderingReader.SkipValueAsync(); // Skip object value
                    Assert.Equal(
                        "@odata.id",
                        await reorderingReader.ReadPropertyNameAsync());
                    Assert.Equal(
                        "http://tempuri.org/Customers(1)",
                        await reorderingReader.ReadPrimitiveValueAsync());
                    Assert.Equal(
                        "Id",
                        await reorderingReader.ReadPropertyNameAsync());
                    Assert.Equal(
                        1,
                        await reorderingReader.ReadPrimitiveValueAsync());
                    Assert.Equal(
                        "Name",
                        await reorderingReader.ReadPropertyNameAsync());
                    Assert.Equal(
                        "Sue",
                        await reorderingReader.ReadPrimitiveValueAsync());
                });
        }
        [Fact]
        public async Task ReadReorderedPayloadContainingSimplifiedODataAnnotationsAsync()
        {
            var payload = "{" +
                "\"Id\":1," +
                "\"@id\":\"http://tempuri.org/Customers(1)\"," +
                "\"Name\":\"Sue\"," +
                "\"@removed\":{\"reason\":\"deleted\"}}";

            await SetupReorderingJsonReaderAndRunTestAsync(
                payload,
                async (reorderingReader) =>
                {
                    Assert.Equal(
                        "@removed",
                        await reorderingReader.ReadPropertyNameAsync());
                    await reorderingReader.SkipValueAsync(); // Skip object value
                    Assert.Equal(
                        "@id",
                        await reorderingReader.ReadPropertyNameAsync());
                    Assert.Equal(
                        "http://tempuri.org/Customers(1)",
                        await reorderingReader.ReadPrimitiveValueAsync());
                    Assert.Equal(
                        "Id",
                        await reorderingReader.ReadPropertyNameAsync());
                    Assert.Equal(
                        1,
                        await reorderingReader.ReadPrimitiveValueAsync());
                    Assert.Equal(
                        "Name",
                        await reorderingReader.ReadPropertyNameAsync());
                    Assert.Equal(
                        "Sue",
                        await reorderingReader.ReadPrimitiveValueAsync());
                });
        }

        [Fact]
        public async Task ReadBinaryValueAsync()
        {
            var payload = "{\"Binary\":\"AQIDBAUGBwgJAA==\"}";

            await SetupReorderingJsonReaderAndRunTestAsync(
                payload,
                async (reorderingReader) =>
                {
                    Assert.Equal(
                        "Binary",
                        await reorderingReader.ReadPropertyNameAsync());

                    using (var binaryStream = await reorderingReader.CreateReadStreamAsync())
                    {
                        var maxLength = 10;
                        var buffer = new byte[maxLength];

                        var bytesRead = await binaryStream.ReadAsync(buffer, 0, maxLength);
                        Assert.Equal(bytesRead, maxLength);
                        Assert.Equal(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }, buffer);
                    }
                });
        }

        [Fact]
        public async Task ReadNullBinaryValueAsync()
        {
            var payload = "{\"Binary\":null}";

            await SetupReorderingJsonReaderAndRunTestAsync(
                payload,
                async (reorderingReader) =>
                {
                    Assert.Equal(
                        "Binary",
                        await reorderingReader.ReadPropertyNameAsync());

                    using (var binaryStream = await reorderingReader.CreateReadStreamAsync())
                    {
                        var maxLength = 0;
                        var buffer = new byte[maxLength];

                        var bytesRead = await binaryStream.ReadAsync(buffer, 0, maxLength);
                        Assert.Equal(bytesRead, maxLength);
                        Assert.Equal(new byte[maxLength], buffer);
                    }
                });
        }

        [Fact]
        public async Task ReadStringValueAsync()
        {
            var pangram = "The quick brown fox jumps over the lazy dog.";
            var payload = $"{{\"Text\":\"{pangram}\"}}";

            await SetupReorderingJsonReaderAndRunTestAsync(
                payload,
                async (reorderingReader) =>
                {
                    Assert.Equal(
                        "Text",
                        await reorderingReader.ReadPropertyNameAsync());

                    using (var textReader = await reorderingReader.CreateTextReaderAsync())
                    {
                        var strLength = pangram.Length;
                        var maxLength = strLength + 1; // + 1 to cater for character for closing stream
                        var chars = new char[maxLength];

                        // The reader will be at the next node after reading a stream - ReadCharsAsync
                        var charsRead = await textReader.ReadAsync(chars, 0, maxLength);
                        Assert.Equal(charsRead, strLength);
                        Assert.Equal(pangram, new string(chars, 0, charsRead));
                    }
                });
        }

        [Fact]
        public async Task ReadNullStringValueAsync()
        {
            var payload = "{\"Text\":null}";

            await SetupReorderingJsonReaderAndRunTestAsync(
                payload,
                async (reorderingReader) =>
                {
                    Assert.Equal(
                        "Text",
                        await reorderingReader.ReadPropertyNameAsync());

                    using (var textReader = await reorderingReader.CreateTextReaderAsync())
                    {
                        var maxLength = 0;
                        var chars = new char[maxLength];

                        // The reader will be at the next node after reading a stream - ReadCharsAsync
                        var charsRead = await textReader.ReadAsync(chars, 0, maxLength);
                        Assert.Equal(charsRead, maxLength);
                        Assert.Equal("", new string(chars, 0, charsRead));
                    }
                });
        }

        [Theory]
        [InlineData("13", false)]
        [InlineData("null", true)]
        [InlineData("\"The quick brown fox jumps over the lazy dog.\"", true)]
        public async Task CanStreamAsync_ReturnsExpectedResult(string data, bool expected)
        {
            var payload = $"{{\"Data\":{data}}}";

            await SetupReorderingJsonReaderAndRunTestAsync(
                payload,
                async (reorderingReader) =>
                {
                    Assert.Equal(
                        "Data",
                        await reorderingReader.ReadPropertyNameAsync());

                    Assert.Equal(expected, await reorderingReader.CanStreamAsync());
                });
        }

        [Fact]
        public async Task CreateReadStreamAsync_ThrowsExceptionForInvalidBinaryFormat()
        {
            var payload = "{\"Binary\":\"AQIDBAUGBwgJAA\"}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupReorderingJsonReaderAndRunTestAsync(
                    payload,
                    async (reorderingReader) =>
                    {
                        Assert.Equal(
                            "Binary",
                            await reorderingReader.ReadPropertyNameAsync());

                        using (var binaryStream = await reorderingReader.CreateReadStreamAsync())
                        {
                        }
                    }));

            Assert.Equal(
                Error.Format(SRResources.JsonReader_InvalidBinaryFormat, "AQIDBAUGBwgJAA"),
                exception.Message);
        }

        [Fact]
        public async Task CreateTextStreamAsync_ThrowsExceptionForReaderNotOnPropertyNode()
        {
            var payload = "{\"Text\":null}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupReorderingJsonReaderAndRunTestAsync(
                    payload,
                    (reorderingReader) => reorderingReader.CreateTextReaderAsync()));

            Assert.Equal(
                SRResources.JsonReader_CannotCreateTextReader,
                exception.Message);
        }

        [Fact]
        public async Task ReadReorderedPayloadAsync_ThrowsExceptionForInvalidInstanceAnnotation()
        {
            var payload = "{\"invalid.annotation\":\"foobar\"}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupReorderingJsonReaderAndRunTestAsync(
                    payload,
                    async (reorderingReader) =>
                    {
                        await reorderingReader.ReadPropertyNameAsync();
                        await reorderingReader.ReadPrimitiveValueAsync();
                    }));

            Assert.Equal(
                Error.Format(SRResources.JsonReaderExtensions_UnexpectedInstanceAnnotationName, "invalid.annotation"),
                exception.Message);
        }

        public static IEnumerable<object> GetInStreamErrorPayloadTestData()
        {
            yield return $"{{\"error\":{{\"code\":\"{new string('c', 16)}\",\"message\":\"{new string('m', 16)}\",\"target\":\"{new string('t', 16)}\"}}}}";

            yield return $"{{\"error\":{{\"code\":\"{new string('c', 2048)}\",\"message\":\"{new string('m', 2048)}\",\"target\":\"{new string('t', 2048)}\"}}}}";
        }

        public static IEnumerable<object[]> GetInStreamErrorPayloadTestData_WithReaderKinds =>
            ExpandWithReaderKinds(GetInStreamErrorPayloadTestData());

        [Theory]
        [MemberData(nameof(GetInStreamErrorPayloadTestData_WithReaderKinds))]
        public async Task ReadInStreamError_TopLevelInStreamErrorPayload_ThrowsAsync(string payload, ReaderSourceKind sourceKind)
        {
            // Arrange
            var textReader = CreateTextReader(payload, sourceKind);

            using (var jsonReader = new JsonReader(textReader, false))
            {
                var reorderingReader = new ReorderingJsonReader(jsonReader, 4);

                // Act & Assert
                var exception = await Assert.ThrowsAsync<ODataErrorException>(reorderingReader.ReadAsync);
                VerifyODataError(payload, exception.Error);
            }
        }

        [Theory]
        [InlineData(ReaderSourceKind.Buffered)]
        [InlineData(ReaderSourceKind.Chunked)]
        public async Task ReadInStreamError_NonTopLevelInStreamErrorPayload_ThrowsAsync(ReaderSourceKind sourceKind)
        {
            // Arrange
            var payload = $"{{\"first\":\"{new string('f', 8192)}\",\"next\":{{\"error\":{{\"code\":\"{new string('c', 2048)}\",\"message\":\"{new string('m', 2048)}\",\"target\":\"{new string('t', 2048)}\"}}}}}}";
            var textReader = CreateTextReader(payload, sourceKind);

            using (var jsonReader = new JsonReader(textReader, false))
            {
                var reorderingReader = new ReorderingJsonReader(jsonReader, 4);

                // Act & Assert
                var exception = await Assert.ThrowsAsync<ODataErrorException>(reorderingReader.ReadAsync); // StartObject of "next" property value with a single "error" property

                using JsonDocument doc = JsonDocument.Parse(payload);
                JsonElement error = doc.RootElement.GetProperty("next").GetProperty("error");

                string code = error.GetProperty("code").GetString();
                string message = error.GetProperty("message").GetString();
                string target = error.GetProperty("target").GetString();

                var odataError = exception.Error;
                Assert.NotNull(odataError);
                Assert.Equal(code, odataError.Code);
                Assert.Equal(message, odataError.Message);
                Assert.Equal(target, odataError.Target);
            }
        }

        [Theory]
        [InlineData(ReaderSourceKind.Buffered)]
        [InlineData(ReaderSourceKind.Chunked)]
        public async Task Read_SlowPathNestedObjectEndAsync(ReaderSourceKind sourceKind)
        {
            // Arrange
            var payload = $"{{\"First\":{{\"StringProp\":\"{new string('s', 256)}\"}}{new string(' ', 8192)},{new string(' ', 8192)}\"Next\":{{\"NumberProp\":{new string('9', 256)}}}}}";
            var textReader = CreateTextReader(payload, sourceKind);

            using (var jsonReader = new JsonReader(textReader, false))
            {
                var reorderingReader = new ReorderingJsonReader(jsonReader, 4);

                // Act & Assert
                Assert.True(await reorderingReader.ReadAsync()); // {
                Assert.True(await reorderingReader.ReadAsync()); // First
                Assert.Equal("First", await reorderingReader.GetValueAsync());
                Assert.True(await reorderingReader.ReadAsync()); // {
                Assert.True(await reorderingReader.ReadAsync()); // StringProp
                Assert.Equal("StringProp", await reorderingReader.GetValueAsync());
                Assert.True(await reorderingReader.ReadAsync()); // StringProp value
                Assert.Equal(new string('s', 256), await reorderingReader.GetValueAsync());
                Assert.True(await reorderingReader.ReadAsync()); // }
                Assert.True(await reorderingReader.ReadAsync()); // Next
                Assert.Equal("Next", await reorderingReader.GetValueAsync());
                Assert.True(await reorderingReader.ReadAsync()); // {
                Assert.True(await reorderingReader.ReadAsync()); // NumberProp
                Assert.Equal("NumberProp", await reorderingReader.GetValueAsync());
                Assert.True(await reorderingReader.ReadAsync()); // NumberProp value
                Assert.True(await reorderingReader.ReadAsync()); // }
                await reorderingReader.ReadAsync(); // }
            }
        }

        [Theory]
        [InlineData(ReaderSourceKind.Buffered)]
        [InlineData(ReaderSourceKind.Chunked)]
        public async Task Read_SlowPathBufferedValueAsync(ReaderSourceKind sourceKind)
        {
            // Arrange
            var payload = $"{{\"Data@attr.large\":{{{new string(' ', 4096)}\"First\":[1,2,3,4,5,6,7,8,9]{new string(' ', 4096)},{new string(' ', 4096)}\"Next\":{{\"A\":1,\"B\":2,\"C\":3}}}},\"After\":42}}";
            var textReader = CreateTextReader(payload, sourceKind);

            using (var jsonReader = new JsonReader(textReader, false))
            {
                var reorderingReader = new ReorderingJsonReader(jsonReader, 4);

                // Act & Assert
                Assert.True(await reorderingReader.ReadAsync()); // {
                Assert.True(await reorderingReader.ReadAsync()); // Data@attr.large property name
                Assert.Equal("Data@attr.large", await reorderingReader.GetValueAsync());
                
                await reorderingReader.ReadAsync(); // {
                // Skip over Data@attr.large property value
                await reorderingReader.SkipValueAsync();
                
                Assert.Equal("After", await reorderingReader.GetValueAsync());
                Assert.True(await reorderingReader.ReadAsync()); // After property value
                Assert.Equal(42, await reorderingReader.GetValueAsync());
                await reorderingReader.ReadAsync(); // }
            }
        }

        /// <summary>
        /// Creates a new <see cref="ReorderingJsonReader"/> and advances it to the first property node
        /// </summary>
        /// <param name="json">The json string to potentially reorder and read.</param>
        /// <returns>The created json reader.</returns>
        private static ReorderingJsonReader CreateReorderingReaderPositionedOnFirstProperty(string json)
        {
            var stringReader = new StringReader(json);
            var innerReader = new JsonReader(stringReader, isIeee754Compatible: true);
            var reader = new ReorderingJsonReader(innerReader, maxInnerErrorDepth: 0);

            Assert.Equal(JsonNodeType.None, reader.NodeType);
            reader.Read();
            Assert.Equal(JsonNodeType.StartObject, reader.NodeType);
            reader.Read();
            Assert.Equal(JsonNodeType.Property, reader.NodeType);
            return reader;
        }

        private async Task SetupReorderingJsonReaderAndRunTestAsync(
            string payload,
            Func<ReorderingJsonReader, Task> func)
        {
            using (var stringReader = new StringReader(payload))
            {
                using (var jsonReader = new JsonReader(stringReader, isIeee754Compatible: false))
                {
                    var reorderingReader = new ReorderingJsonReader(jsonReader, maxInnerErrorDepth: 0);

                    await reorderingReader.ReadAsync(); // Position the reader on the first node
                    await reorderingReader.ReadAsync(); // Read StartObject

                    await func(reorderingReader);
                }
            }
        }

        private void VerifyODataError(string json, ODataError odataError)
        {
            using JsonDocument doc = JsonDocument.Parse(json);
            JsonElement error = doc.RootElement.GetProperty("error");

            string code = error.GetProperty("code").GetString();
            string message = error.GetProperty("message").GetString();
            string target = error.GetProperty("target").GetString();

            Assert.Equal(code, odataError.Code);
            Assert.Equal(message, odataError.Message);
            Assert.Equal(target, odataError.Target);
        }

        private static IEnumerable<object[]> ExpandWithReaderKinds(IEnumerable<object> testData)
        {
            foreach (var dataRow in testData)
            {
                yield return new object[] { dataRow, ReaderSourceKind.Buffered }; // baseline buffered StringReader
                yield return new object[] { dataRow, ReaderSourceKind.Chunked }; // chunked/refill ChunkedStringReader
            }
        }

        private static TextReader CreateTextReader(string payload, ReaderSourceKind readerKind, int chunkSize = 128) =>
            readerKind == ReaderSourceKind.Buffered ? new StringReader(payload) : new ChunkedStringReader(payload, chunkSize);

        public enum ReaderSourceKind
        {
            Buffered,   // StringReader (baseline)
            Chunked     // ChunkedStringReader (forced refills / async completion)
        }
    }
}

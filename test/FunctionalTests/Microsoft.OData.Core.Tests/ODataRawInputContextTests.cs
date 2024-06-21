//---------------------------------------------------------------------
// <copyright file="ODataRawInputContextTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Xunit;
using ErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests
{
    /// <summary>
    /// Unit tests for ODataRawInputContext class.
    ///</summary>
    public class ODataRawInputContextTests
    {
        private ODataMessageReaderSettings messageReaderSettings;
        private EdmModel model;
        private EdmEntityType customerEntityType;
        private EdmEntitySet customerEntitySet;

        public ODataRawInputContextTests()
        {
            this.InitializeEdmModel();
            this.messageReaderSettings = new ODataMessageReaderSettings { Version = ODataVersion.V4 };
        }

        [Fact]
        public async Task ReadValueAsync()
        {
            await SetupRawInputContextAndRunTestAsync(
                "foobar",
                async (rawInputContext) =>
                {
                    var value = await rawInputContext.ReadValueAsync(EdmCoreModel.Instance.GetString(false));

                    Assert.Equal("foobar", value);
                });
        }

        [Fact]
        public async Task ReadValueAsync_ForExpectedPrimitiveTypeReferenceIsNull()
        {
            await SetupRawInputContextAndRunTestAsync(
                "foobar",
                async (rawInputContext) =>
                {
                    var value = await rawInputContext.ReadValueAsync(null);

                    Assert.Equal("foobar", value);
                });
        }

        [Fact]
        public async Task ReadBinaryValueAsync()
        {
            await SetupRawInputContextAndRunTestAsync(
                "\u0001\u0002\u0003\u0004\u0005\u0006\a\b\t\0",
                async (rawInputContext) =>
                {
                    var binaryValue = await rawInputContext.ReadValueAsync(EdmCoreModel.Instance.GetBinary(false));

                    Assert.Equal(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }, binaryValue);
                });
        }

        [Fact]
        public void ReadBinaryValue_ForNonSeekableStream()
        {
            var bufferSize = 4097;
            var bytes = new byte[bufferSize];
            for (var i = 0; i < bufferSize; i++)
            {
                bytes[i] = (byte)(i % 10);
            }

            var payload = Encoding.UTF8.GetString(bytes);

            var encoding = MediaTypeUtils.EncodingUtf8NoPreamble;
            var messageInfo = new ODataMessageInfo
            {
                MessageStream = new NonSeekableStream(new MemoryStream(encoding.GetBytes(payload))),
                MediaType = new ODataMediaType(MimeConstants.MimeTextType, MimeConstants.MimePlainSubType),
                Encoding = encoding,
                IsResponse = true,
                IsAsync = false,
                Model = EdmCoreModel.Instance
            };

            using (var rawInputContext = new ODataRawInputContext(
                ODataFormat.RawValue,
                messageInfo,
                this.messageReaderSettings))
            {
                var binaryValue = rawInputContext.ReadValue(EdmCoreModel.Instance.GetBinary(false));

                Assert.Equal(bytes, binaryValue);
            }
        }

        [Fact]
        public async Task ReadBinaryValueAsync_ForNonSeekableStream()
        {
            var bufferSize = 4097;
            var bytes = new byte[bufferSize];
            for (var i = 0; i < bufferSize; i++)
            {
                bytes[i] = (byte)(i % 10);
            }

            var payload = Encoding.UTF8.GetString(bytes);

            var encoding = MediaTypeUtils.EncodingUtf8NoPreamble;
            var messageInfo = new ODataMessageInfo
            {
                MessageStream = new NonSeekableStream(new MemoryStream(encoding.GetBytes(payload))),
                MediaType = new ODataMediaType(MimeConstants.MimeTextType, MimeConstants.MimePlainSubType),
                Encoding = encoding,
                IsResponse = true,
                IsAsync = true,
                Model = EdmCoreModel.Instance
            };

            using (var rawInputContext = new ODataRawInputContext(
                ODataFormat.RawValue,
                messageInfo,
                this.messageReaderSettings))
            {
                var binaryValue = await rawInputContext.ReadValueAsync(EdmCoreModel.Instance.GetBinary(false));

                Assert.Equal(bytes, binaryValue);
            }  
        }

        [Fact]
        public async Task ReadResourceInResponseMessageAsync()
        {
            var payload = @"HTTP/1.1 200 OK
Content-Type: application/json
OData-Version: 4.0

{""@odata.context"":""http://tempuri.org/$metadata#Customers/$entity"",""Id"":1,""Name"":""Customer 1""}";

            await SetupRawInputContextAndRunTestAsync(
                payload,
                async (rawInputContext) =>
                {
                    var asynchronousReader = await rawInputContext.CreateAsynchronousReaderAsync();
                    var responseMessage = await asynchronousReader.CreateResponseMessageAsync();

                    Assert.Equal(200, responseMessage.StatusCode);
                    Assert.Contains(new KeyValuePair<string, string>("Content-Type", "application/json"), responseMessage.Headers);
                    Assert.Contains(new KeyValuePair<string, string>("OData-Version", "4.0"), responseMessage.Headers);

                    using (var messageReader = new ODataMessageReader(responseMessage, new ODataMessageReaderSettings(), this.model))
                    {
                        var jsonReader = await messageReader.CreateODataResourceReaderAsync(this.customerEntitySet, this.customerEntityType);

                        await DoReadAsync(
                            jsonReader as ODataJsonReader,
                            verifyResourceAction: (resource) =>
                            {
                                Assert.NotNull(resource);
                                Assert.Equal("NS.Customer", resource.TypeName);
                                Assert.Equal(2, resource.Properties.Count());
                                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                                Assert.Equal("Id", properties[0].Name);
                                Assert.Equal(1, properties[0].Value);
                                Assert.Equal("Name", properties[1].Name);
                                Assert.Equal("Customer 1", properties[1].Value);
                            });
                    }
                });
        }

        [Fact]
        public async Task ReadResourceSetInResponseMessageAsync()
        {
            var payload = @"
HTTP/1.1 200 OK
Content-Type: application/json
OData-Version: 4.0

{""@odata.context"":""http://tempuri.org/$metadata#Customers"",""@odata.count"":1,""value"":[{""Id"":1,""Name"":""Customer 1""}]}";

            await SetupRawInputContextAndRunTestAsync(
                payload,
                async (rawInputContext) =>
                {
                    var asynchronousReader = await rawInputContext.CreateAsynchronousReaderAsync();
                    var responseMessage = await asynchronousReader.CreateResponseMessageAsync();

                    Assert.Equal(200, responseMessage.StatusCode);
                    Assert.Contains(new KeyValuePair<string, string>("Content-Type", "application/json"), responseMessage.Headers);
                    Assert.Contains(new KeyValuePair<string, string>("OData-Version", "4.0"), responseMessage.Headers);

                    using (var messageReader = new ODataMessageReader(responseMessage, new ODataMessageReaderSettings(), this.model))
                    {
                        var jsonReader = await messageReader.CreateODataResourceSetReaderAsync(this.customerEntitySet, this.customerEntityType);

                        await DoReadAsync(
                            jsonReader as ODataJsonReader,
                            verifyResourceSetAction: (resourceSet) =>
                            {
                                Assert.NotNull(resourceSet);
                                Assert.Equal(1, resourceSet.Count);
                            },
                            verifyResourceAction: (resource) =>
                            {
                                Assert.NotNull(resource);
                                Assert.Equal("NS.Customer", resource.TypeName);
                                Assert.Equal(2, resource.Properties.Count());
                                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                                Assert.Equal("Id", properties[0].Name);
                                Assert.Equal(1, properties[0].Value);
                                Assert.Equal("Name", properties[1].Name);
                                Assert.Equal("Customer 1", properties[1].Value);
                            });
                    }
                });
        }

        [Theory]
        [InlineData("HTTP/1.1 200 OK\n", '\n')]
        [InlineData("HTTP/1.1 200 OK\r", '\r')]
        public async Task ReadRawInputAsync_ThrowsExceptionForInvalidNewLine(string payload, char lastChar)
        {
            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupRawInputContextAndRunTestAsync(
                    payload,
                    async (rawInputContext) =>
                    {
                        var asynchronousReader = await rawInputContext.CreateAsynchronousReaderAsync();
                        var responseMessage = await asynchronousReader.CreateResponseMessageAsync();
                    }));

            Assert.Equal(
                ErrorStrings.ODataAsyncReader_InvalidNewLineEncountered(lastChar),
                exception.Message);
        }

        [Fact]
        public async Task ReadRawInputAsync_ThrowsExceptionForInvalidInput()
        {
            var payload = "HTTP/1.1 200 OK";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupRawInputContextAndRunTestAsync(
                    payload,
                    async (rawInputContext) =>
                    {
                        var asynchronousReader = await rawInputContext.CreateAsynchronousReaderAsync();
                        var responseMessage = await asynchronousReader.CreateResponseMessageAsync();
                    }));

            Assert.Equal(
                ErrorStrings.ODataAsyncReader_UnexpectedEndOfInput,
                exception.Message);
        }

        [Fact]
        public async Task ReadRawInputAsync_ThrowsExceptionForDuplicateHeaders()
        {
            var payload = @"HTTP/1.1 200 OK
Content-Type: application/json
Content-Type: application/json
";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupRawInputContextAndRunTestAsync(
                    payload,
                    async (rawInputContext) =>
                    {
                        var asynchronousReader = await rawInputContext.CreateAsynchronousReaderAsync();
                        var responseMessage = await asynchronousReader.CreateResponseMessageAsync();
                    }));

            Assert.Equal(
                ErrorStrings.ODataAsyncReader_DuplicateHeaderFound("Content-Type"),
                exception.Message);
        }

        private async Task SetupRawInputContextAndRunTestAsync(
            string payload,
            Func<ODataRawInputContext, Task> func)
        {
            var encoding = MediaTypeUtils.EncodingUtf8NoPreamble;
            var messageInfo = new ODataMessageInfo
            {
                MessageStream = new MemoryStream(encoding.GetBytes(payload)),
                MediaType = new ODataMediaType(MimeConstants.MimeTextType, MimeConstants.MimePlainSubType),
                Encoding = encoding,
                IsResponse = true,
                IsAsync = true,
                Model = EdmCoreModel.Instance
            };

            using (var rawInputContext = new ODataRawInputContext(
                ODataFormat.RawValue,
                messageInfo,
                this.messageReaderSettings))
            {
                await func(rawInputContext);
            }
        }

        private async Task DoReadAsync(
           ODataJsonReader jsonReader,
           Action<ODataResourceSet> verifyResourceSetAction = null,
           Action<ODataResource> verifyResourceAction = null)
        {
            while (await jsonReader.ReadAsync())
            {
                switch (jsonReader.State)
                {
                    case ODataReaderState.ResourceSetStart:
                        break;
                    case ODataReaderState.ResourceSetEnd:
                        if (verifyResourceSetAction != null)
                        {
                            verifyResourceSetAction(jsonReader.Item as ODataResourceSet);
                        }

                        break;
                    case ODataReaderState.ResourceStart:
                        break;
                    case ODataReaderState.ResourceEnd:
                        if (verifyResourceAction != null)
                        {
                            verifyResourceAction(jsonReader.Item as ODataResource);
                        }

                        break;
                    default:
                        break;
                }
            }
        }

        private void InitializeEdmModel()
        {
            this.model = new EdmModel();

            this.customerEntityType = new EdmEntityType("NS", "Customer");

            var customerIdProperty = this.customerEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            this.customerEntityType.AddKeys(customerIdProperty);
            this.customerEntityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            this.model.AddElement(this.customerEntityType);

            var entityContainer = new EdmEntityContainer("NS", "Container");
            this.model.AddElement(entityContainer);

            this.customerEntitySet = entityContainer.AddEntitySet("Customers", this.customerEntityType);
        }
    }

    public class NonSeekableStream : Stream
    {
        private readonly Stream stream;

        public NonSeekableStream(Stream stream)
        {
            this.stream = stream;
        }

        public override bool CanRead => this.stream.CanRead;

        public override bool CanSeek => false;

        public override bool CanWrite => this.stream.CanWrite;

        public override long Position { get => this.stream.Position; set => throw new NotSupportedException(); }

        public override long Length => throw new NotSupportedException();

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.stream.Read(buffer, offset, count);
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return this.stream.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.stream.Write(buffer, offset, count);
        }

        public override void Flush()
        {
            this.stream.Flush();
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return this.stream.FlushAsync(cancellationToken);
        }
    }
}

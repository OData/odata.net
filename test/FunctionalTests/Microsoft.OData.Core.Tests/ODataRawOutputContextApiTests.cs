//---------------------------------------------------------------------
// <copyright file="ODataRawOutputContextApiTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.Spatial;
using Xunit;

namespace Microsoft.OData.Tests
{
    /// <summary>
    /// Unit tests for congruence between synchronous and asynchronous API exposed by ODataRawOutputContext class.
    ///</summary>
    public class ODataRawOutputContextApiTests
    {
        private const string ServiceUri = "http://tempuri.org";
        private readonly MemoryStream asyncStream;
        private readonly MemoryStream syncStream;
        private readonly ODataMessageWriterSettings writerSettings;

        private EdmModel model;
        private EdmEntityType customerEntityType;
        private EdmEntitySet customerEntitySet;

        public ODataRawOutputContextApiTests()
        {
            this.InitializeEdmModel();
            this.asyncStream = new MemoryStream();
            this.syncStream = new MemoryStream();
            this.writerSettings = new ODataMessageWriterSettings
            {
                Version = ODataVersion.V4,
                EnableMessageStreamDisposal = false
            };

            this.writerSettings.SetServiceDocumentUri(new Uri(ServiceUri));
            this.writerSettings.SetContentType(ODataFormat.RawValue);
        }

        [Fact]
        public async Task WriteResponseMessage_APIsShouldYieldSameResult()
        {
            var nestedWriterSettings = new ODataMessageWriterSettings
            {
                Version = ODataVersion.V4,
                EnableMessageStreamDisposal = false
            };

            nestedWriterSettings.SetServiceDocumentUri(new Uri(ServiceUri));
            var customerResponse = new ODataResource
            {
                TypeName = "NS.Customer",
                Properties = new List<ODataProperty>
                {
                    new ODataProperty
                    {
                        Name = "Id",
                        Value = 1,
                        SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Key }
                    },
                    new ODataProperty { Name = "Name", Value = "Customer 1" }
                }
            };

            IODataResponseMessage asyncResponseMessage = new InMemoryMessage { StatusCode = 200, Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, writerSettings))
            {
                var asynchronousWriter = await messageWriter.CreateODataAsynchronousWriterAsync();
                var responseMessage = await asynchronousWriter.CreateResponseMessageAsync();
                responseMessage.StatusCode = 200;

                using (var nestedMessageWriter = new ODataMessageWriter(responseMessage, nestedWriterSettings, this.model))
                {
                    var writer = await nestedMessageWriter.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);

                    await writer.WriteStartAsync(customerResponse);
                    await writer.WriteEndAsync();
                    await writer.FlushAsync();
                }

                await asynchronousWriter.FlushAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(() =>
            {
                IODataResponseMessage syncResponseMessage = new InMemoryMessage { StatusCode = 200, Stream = this.syncStream };
                using (var messageWriter = new ODataMessageWriter(syncResponseMessage, writerSettings))
                {
                    var asynchronousWriter = messageWriter.CreateODataAsynchronousWriter();
                    var responseMessage = asynchronousWriter.CreateResponseMessage();
                    responseMessage.StatusCode = 200;

                    using (var nestedMessageWriter = new ODataMessageWriter(responseMessage, nestedWriterSettings, this.model))
                    {
                        var writer = nestedMessageWriter.CreateODataResourceWriter(this.customerEntitySet, this.customerEntityType);

                        writer.WriteStart(customerResponse);
                        writer.WriteEnd();
                        writer.Flush();
                    }

                    asynchronousWriter.Flush();
                }

                this.syncStream.Position = 0;
                return new StreamReader(this.syncStream).ReadToEnd();
            });

            var expected = @"HTTP/1.1 200 OK
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""@odata.context"":""http://tempuri.org/$metadata#Customers/$entity"",""Id"":1,""Name"":""Customer 1""}";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        public static IEnumerable<object[]> GetWriteValueTestData()
        {
            // Enum
            yield return new object[]
            {
                new ODataEnumValue("Black", "NS.Color"),
                "Black"
            };

            // Primitive
            yield return new object[]
            {
                "foobar",
                "foobar"
            };

            // Spatial
            yield return new object[]
            {
                GeographyPoint.Create(22.2, 22.2),
                "{\"type\":\"Point\",\"coordinates\":[22.2,22.2],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}"
            };

            // byte array
            yield return new object[]
            {
                new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 },
                "\u0001\u0002\u0003\u0004\u0005\u0006\a\b\t\0"
            };
        }

        [Theory]
        [MemberData(nameof(GetWriteValueTestData))]
        public async Task WriteValue_APIsShouldYieldSameResult(object value, string expected)
        {
            IODataResponseMessage asyncResponseMessage = new InMemoryMessage { StatusCode = 200, Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, writerSettings))
            {
                await messageWriter.WriteValueAsync(value);
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(() =>
            {
                IODataResponseMessage syncResponseMessage = new InMemoryMessage { StatusCode = 200, Stream = this.syncStream };
                using (var messageWriter = new ODataMessageWriter(syncResponseMessage, writerSettings))
                {
                    messageWriter.WriteValue(value);
                }

                this.syncStream.Position = 0;
                return new StreamReader(this.syncStream).ReadToEnd();
            });

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        public async Task WriteInStreamError_APIsShouldYieldSameResult()
        {
            var nestedWriterSettings = new ODataMessageWriterSettings
            {
                Version = ODataVersion.V4,
                EnableMessageStreamDisposal = false
            };

            nestedWriterSettings.SetServiceDocumentUri(new Uri(ServiceUri));

            var asyncException = await Assert.ThrowsAsync<ODataException>(async () =>
            {
                IODataResponseMessage asyncResponseMessage = new InMemoryMessage { StatusCode = 200, Stream = this.asyncStream };
                using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, writerSettings))
                {
                    // Call to CreateODataAsynchronousWriterAsync triggers setting of output in-stream error listener
                    var asynchronousWriter = await messageWriter.CreateODataAsynchronousWriterAsync();
                    var responseMessage = await asynchronousWriter.CreateResponseMessageAsync();
                    responseMessage.StatusCode = 200;

                    // Next section added is to demonstrate that what was already written is flushed to the buffer before exception is thrown
                    using (var nestedMessageWriter = new ODataMessageWriter(responseMessage, nestedWriterSettings))
                    {
                        var writer = await nestedMessageWriter.CreateODataResourceWriterAsync();
                    }

                    await messageWriter.WriteErrorAsync(
                        new ODataError { ErrorCode = "NRE", Message = "Object reference not set to an instance of an object." },
                        /*includeDebugInformation*/ true);
                }
            });

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(() =>
                {
                    IODataResponseMessage syncResponseMessage = new InMemoryMessage { StatusCode = 200, Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncResponseMessage, writerSettings))
                    {
                        // Call to CreateODataAsynchronousWriterAsync triggers setting of output in-stream error listener
                        var asynchronousWriter = messageWriter.CreateODataAsynchronousWriter();
                        var responseMessage = asynchronousWriter.CreateResponseMessage();
                        responseMessage.StatusCode = 200;

                        // Next section is added to demonstrate that what was already written is flushed to the buffer before exception is thrown
                        using (var nestedMessageWriter = new ODataMessageWriter(responseMessage, nestedWriterSettings))
                        {
                            var writer = nestedMessageWriter.CreateODataResourceWriter();
                        }

                        messageWriter.WriteError(
                            new ODataError { ErrorCode = "NRE", Message = "Object reference not set to an instance of an object." },
                            /*includeDebugInformation*/ true);
                    }
                }));

            this.syncStream.Position = 0;
            var syncResult = await new StreamReader(this.syncStream).ReadToEndAsync();

            var expected = @"HTTP/1.1 200 OK
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

";

            Assert.Equal(Strings.ODataAsyncWriter_CannotWriteInStreamErrorForAsync, asyncException.Message);
            Assert.Equal(Strings.ODataAsyncWriter_CannotWriteInStreamErrorForAsync, syncException.Message);
            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
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
}

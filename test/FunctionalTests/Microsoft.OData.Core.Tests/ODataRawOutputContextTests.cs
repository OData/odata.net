//---------------------------------------------------------------------
// <copyright file="ODataRawOutputContextTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests
{
    /// <summary>
    /// Unit tests for ODataRawOutputContext class.
    ///</summary>
    public class ODataRawOutputContextTests
    {
        private const string ServiceUri = "http://tempuri.org";
        private MemoryStream stream;
        private ODataMessageWriterSettings settings;
        private ODataError nullReferenceError;

        private EdmModel model;
        private EdmEntityType customerEntityType;
        private EdmEntitySet customerEntitySet;


        public ODataRawOutputContextTests()
        {
            this.InitializeEdmModel();
            this.stream = new MemoryStream();
            this.settings = new ODataMessageWriterSettings { Version = ODataVersion.V4 };
            this.settings.SetServiceDocumentUri(new Uri(ServiceUri));
            this.nullReferenceError = new ODataError
            {
                ErrorCode = "NRE",
                Message = "Object reference not set to an instance of an object"
            };
        }

        [Fact]
        public async Task WriteValueAsync()
        {
            var result = await SetupRawOutputContextAndRunTestAsync(
                (rawOutputContext) => rawOutputContext.WriteValueAsync("foobar"));

            Assert.Equal("foobar", result);
        }

        [Fact]
        public async Task WriteResponseMessageAsync()
        {
            var result = await SetupRawOutputContextAndRunTestAsync(
                async (rawOutputContext) =>
                {
                    var asynchronousWriter = await rawOutputContext.CreateODataAsynchronousWriterAsync();
                    var responseMessage = await asynchronousWriter.CreateResponseMessageAsync();
                    responseMessage.StatusCode = 200;
                    responseMessage.SetHeader(ODataConstants.ContentTypeHeader, MimeConstants.MimeApplicationJson);

                    var writerSettings = new ODataMessageWriterSettings
                    {
                        Version = ODataVersion.V4,
                        EnableMessageStreamDisposal = false
                    };

                    writerSettings.SetServiceDocumentUri(new Uri(ServiceUri));

                    using (var messageWriter = new ODataMessageWriter(responseMessage, writerSettings, this.model))
                    {
                        var jsonLightWriter = await messageWriter.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);
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

                        await jsonLightWriter.WriteStartAsync(customerResponse);
                        await jsonLightWriter.WriteEndAsync();
                    }
                });

            var expected = @"HTTP/1.1 200 OK
Content-Type: application/json
OData-Version: 4.0

{""@odata.context"":""http://tempuri.org/$metadata#Customers/$entity"",""Id"":1,""Name"":""Customer 1""}";

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task WriteInStreamErrorAsync_ThrowsExceptionForODataAsynchronousWriterAsOutputInStreamErrorListener()
        {
            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupRawOutputContextAndRunTestAsync(
                    async (rawOutputContext) =>
                    {
                        // This should cause the ODataAsynchronousWriter instance to be set as the OutputInStreamErrorListener
                        var asynchronousWriter = await rawOutputContext.CreateODataAsynchronousWriterAsync();
                        // Trigger WriteInStreamErrorAsync
                        await rawOutputContext.WriteInStreamErrorAsync(this.nullReferenceError, false);
                    }));

            Assert.Equal(Strings.ODataAsyncWriter_CannotWriteInStreamErrorForAsync, exception.Message);
        }

        [Fact]
        public async Task WriteInStreamErrorAsync_ThrowsExceptionForOutputInStreamErrorListenerNotSet()
        {
            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupRawOutputContextAndRunTestAsync(
                    async (rawOutputContext) =>
                    {
                        // Trigger WriteInStreamErrorAsync
                        await rawOutputContext.WriteInStreamErrorAsync(this.nullReferenceError, false);
                    }));

            Assert.Equal(Strings.ODataMessageWriter_CannotWriteInStreamErrorForRawValues, exception.Message);
        }

        private async Task<string> SetupRawOutputContextAndRunTestAsync(Func<ODataRawOutputContext, Task> func)
        {
            var messageInfo = new ODataMessageInfo
            {
                MessageStream = this.stream,
                MediaType = new ODataMediaType(MimeConstants.MimeTextType, MimeConstants.MimePlainSubType),
                Encoding = MediaTypeUtils.EncodingUtf8NoPreamble,
                IsResponse = true,
                IsAsync = true,
                Model = EdmCoreModel.Instance
            };

            var rawOutputContext = new ODataRawOutputContext(ODataFormat.RawValue, messageInfo, this.settings);

            await func(rawOutputContext);

            await rawOutputContext.FlushAsync();

            this.stream.Position = 0;
            return await new StreamReader(this.stream).ReadToEndAsync();
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

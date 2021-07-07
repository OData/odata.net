//---------------------------------------------------------------------
// <copyright file="ODataAsynchronousWriterTests.cs" company="Microsoft">
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
    public class ODataAsynchronousWriterTests
    {
        private const string ServiceDocumentUri = "http://host/service";

        private Stream responseStream;
        private IODataResponseMessage responseMessage;
        private ODataMessageWriter messageWriter;

        private EdmModel userModel;
        private EdmEntityType testType;
        private EdmSingleton singleton;

        private ODataMessageWriterSettings settings;

        public ODataAsynchronousWriterTests()
        {
            this.settings = new ODataMessageWriterSettings { Version = ODataVersion.V4 };
            this.responseStream = new MemoryStream();
            InitializeEdmModel();
        }

        [Fact]
        public void WriteCompletedAsyncResponse()
        {
            var asyncWriter = this.TestInit();

            var innerMessage = asyncWriter.CreateResponseMessage();
            innerMessage.StatusCode = 200;
            innerMessage.SetHeader("Content-Type", "application/json");

            var settings = new ODataMessageWriterSettings();
            settings.SetServiceDocumentUri(new Uri(ServiceDocumentUri));
            settings.EnableMessageStreamDisposal = false;

            using (var innerMessageWriter = new ODataMessageWriter(innerMessage, settings, this.userModel))
            {
                var entryWriter = innerMessageWriter.CreateODataResourceWriter(singleton, testType);
                var entry = new ODataResource() { TypeName = "NS.Test", Properties = new[] { new ODataProperty() { Name = "Id", Value = 1 } } };
                entryWriter.WriteStart(entry);
                entryWriter.WriteEnd();
            }

            asyncWriter.Flush();

            var payload = this.TestFinish();
            Assert.Equal("HTTP/1.1 200 OK\r\nContent-Type: application/json\r\nOData-Version: 4.0\r\n\r\n{\"@odata.context\":\"http://host/service/$metadata#MySingleton\",\"Id\":1}", payload);
        }

        [Fact]
        public void CallCreateResponseMessageMoreThanOnceShouldFail()
        {
            var asyncWriter = this.TestInit();
            asyncWriter.CreateResponseMessage();
            Action test = () => asyncWriter.CreateResponseMessage();
            test.Throws<ODataException>(Strings.ODataAsyncWriter_CannotCreateResponseMoreThanOnce);
        }

        [Fact]
        public async Task WriteResponseMessageAsync()
        {
            var result = await SetupAsynchronousWriterAndRunTestAsync(
                async (asynchronousWriter) =>
                {
                    var responseMessage = await asynchronousWriter.CreateResponseMessageAsync();
                    responseMessage.StatusCode = 200;
                    responseMessage.SetHeader(ODataConstants.ContentTypeHeader, MimeConstants.MimeApplicationJson);

                    var writerSettings = new ODataMessageWriterSettings
                    {
                        Version = ODataVersion.V4,
                        EnableMessageStreamDisposal = false
                    };

                    writerSettings.SetServiceDocumentUri(new Uri(ServiceDocumentUri));

                    using (var messageWriter = new ODataMessageWriter(responseMessage, writerSettings, this.userModel))
                    {
                        var jsonLightWriter = await messageWriter.CreateODataResourceWriterAsync(this.singleton, this.testType);
                        var testResponse = new ODataResource
                        {
                            TypeName = "NS.Test",
                            Properties = new List<ODataProperty>
                            {
                                new ODataProperty { Name = "Id", Value = 1 }
                            }
                        };

                        await jsonLightWriter.WriteStartAsync(testResponse);
                        await jsonLightWriter.WriteEndAsync();
                    }
                });

            var expected = @"HTTP/1.1 200 OK
Content-Type: application/json
OData-Version: 4.0

{""@odata.context"":""http://host/service/$metadata#MySingleton"",""Id"":1}";

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task CreateResponseMessageAsync_ThrowsExceptionForMultipleInvocations()
        {
            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupAsynchronousWriterAndRunTestAsync(
                    async (asynchronousWriter) =>
                    {
                        await asynchronousWriter.CreateResponseMessageAsync();
                        await asynchronousWriter.CreateResponseMessageAsync();
                    }));

            Assert.Equal(Strings.ODataAsyncWriter_CannotCreateResponseMoreThanOnce, exception.Message);
        }

        [Fact]
        public async Task OnInStreamErrorAsync_ThrowsException()
        {
            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupAsynchronousWriterAndRunTestAsync(
                    (asynchronousWriter) => ((IODataOutputInStreamErrorListener)asynchronousWriter).OnInStreamErrorAsync()));

            Assert.Equal(Strings.ODataAsyncWriter_CannotWriteInStreamErrorForAsync, exception.Message);
        }

        private ODataAsynchronousWriter TestInit()
        {
            responseMessage = new InMemoryMessage { Stream = responseStream };
            messageWriter = new ODataMessageWriter(responseMessage);
            return messageWriter.CreateODataAsynchronousWriter();
        }

        private void InitializeEdmModel()
        {
            this.userModel = new EdmModel();

            testType = new EdmEntityType("NS", "Test");
            testType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            this.userModel.AddElement(testType);

            var defaultContainer = new EdmEntityContainer("NS", "DefaultContainer");
            this.userModel.AddElement(defaultContainer);

            this.singleton = new EdmSingleton(defaultContainer, "MySingleton", this.testType);
            defaultContainer.AddElement(this.singleton);
        }

        private string TestFinish()
        {
            responseStream.Seek(0, SeekOrigin.Begin);
            var streamReader = new StreamReader(responseStream);
            return streamReader.ReadToEnd();
        }

        private async Task<string> SetupAsynchronousWriterAndRunTestAsync(Func<ODataAsynchronousWriter, Task> func)
        {
            var messageInfo = new ODataMessageInfo
            {
                MessageStream = this.responseStream,
                MediaType = new ODataMediaType(MimeConstants.MimeTextType, MimeConstants.MimePlainSubType),
                Encoding = MediaTypeUtils.EncodingUtf8NoPreamble,
                IsResponse = true,
                IsAsync = true,
                Model = EdmCoreModel.Instance
            };

            var rawOutputContext = new ODataRawOutputContext(ODataFormat.RawValue, messageInfo, this.settings);
            var asynchronousWriter = new ODataAsynchronousWriter(rawOutputContext);

            await func(asynchronousWriter);

            await asynchronousWriter.FlushAsync();
            await this.responseStream.FlushAsync();

            this.responseStream.Position = 0;
            return await new StreamReader(this.responseStream).ReadToEndAsync();
        }
    }
}

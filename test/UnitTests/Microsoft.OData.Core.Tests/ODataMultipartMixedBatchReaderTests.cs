//---------------------------------------------------------------------
// <copyright file="ODataMultipartMixedBatchReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.MultipartMixed;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class ODataMultipartMixedBatchReaderTests
    {
        private const string batchRequestBoundary = "batch_aed653ab";
        private const string batchResponseBoundary = "batchresponse_aed653ab";
        private readonly ODataMediaType mediaType;
        private readonly ODataMessageReaderSettings messageReaderSettings;
        private readonly ODataBatchOperationHeaders batchOperationHeaders;

        #region Batch Paylods

        private const string batchRequestPayload = @"--batch_aed653ab
Content-Type: multipart/mixed; boundary=changeset_5e368128

--changeset_5e368128
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

POST http://tempuri.org/Customers HTTP/1.1
OData-Version: 4.0
OData-MaxVersion: 4.0
Content-Type: application/json;odata.metadata=minimal
Accept: application/json;odata.metadata=minimal
Accept-Charset: UTF-8

{""@odata.type"":""NS.Customer"",""Id"":1,""Name"":""Sue""}
--changeset_5e368128
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2

POST http://tempuri.org/Orders HTTP/1.1
OData-Version: 4.0
OData-MaxVersion: 4.0
Content-Type: application/json;odata.metadata=minimal
Accept: application/json;odata.metadata=minimal
Accept-Charset: UTF-8

{""@odata.type"":""NS.Order"",""Id"":1,""Amount"":13}
--changeset_5e368128
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 3

POST $1/Orders/$ref HTTP/1.1
OData-Version: 4.0
OData-MaxVersion: 4.0
Content-Type: application/json;odata.metadata=minimal
Accept: application/json;odata.metadata=minimal
Accept-Charset: UTF-8

{""@odata.id"":""$2""}
--changeset_5e368128--
--batch_aed653ab--
";

        private const string batchResponsePayload = @"--batchresponse_aed653ab
Content-Type: application/http
Content-Transfer-Encoding: binary

HTTP/1.1 200 OK
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true
OData-Version: 4.0

{""@odata.context"":""http://tempuri.org/$metadata#Customers/$entity"",""Id"":1,""Name"":""Sue""}
--batchresponse_aed653ab--
";

        #endregion Batch Payloads

        private EdmModel model;

        public ODataMultipartMixedBatchReaderTests()
        {
            this.InitializeEdmModel();
            this.messageReaderSettings = new ODataMessageReaderSettings();
            this.mediaType = new ODataMediaType(
                "Multipart",
                "Mixed",
                new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("boundary", batchRequestBoundary) });

            this.batchOperationHeaders = new ODataBatchOperationHeaders
            {
                { "OData-Version", "4.0" },
                { "OData-MaxVersion", "4.0" },
                { "Content-Type", "application/json;odata.metadata=minimal" },
                { "Accept", "application/json;odata.metadata=minimal" },
                { "Accept-Charset", "UTF-8" }
            };
        }

        [Fact]
        public void ReadMultipartMixedBatchRequest()
        {
            var verifyUrlStack = new Stack<string>(new[] { "$1/Orders/$ref", "http://tempuri.org/Orders", "http://tempuri.org/Customers" });

            var verifyDependsOnIdsStack = new Stack<Action<IEnumerable<string>>>();
            verifyDependsOnIdsStack.Push((dependsOnIds) =>
            {
                Assert.Equal(2, dependsOnIds.Count());
                Assert.Equal("1", dependsOnIds.First());
                Assert.Equal("2", dependsOnIds.Last());
            });
            verifyDependsOnIdsStack.Push((dependsOnIds) => Assert.Equal("1", Assert.Single(dependsOnIds)));
            verifyDependsOnIdsStack.Push((dependsOnIds) => Assert.Empty(dependsOnIds));

            var verifyResourceStack = new Stack<Action<ODataResource>>();
            verifyResourceStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Order", resource.TypeName);
                Assert.Equal(2, resource.Properties.Count());
                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Amount", properties[1].Name);
                Assert.Equal(13M, properties[1].Value);
            });
            verifyResourceStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Customer", resource.TypeName);
                Assert.Equal(2, resource.Properties.Count());
                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Name", properties[1].Name);
                Assert.Equal("Sue", properties[1].Value);
            });

            SetupMultipartMixedBatchReaderAndRunTest(
                batchRequestPayload,
                (multipartMixedBatchReader) =>
                {
                    var operationCount = 0;

                    while (multipartMixedBatchReader.Read())
                    {
                        switch (multipartMixedBatchReader.State)
                        {
                            case ODataBatchReaderState.Operation:
                                var operationRequestMessage = multipartMixedBatchReader.CreateOperationRequestMessage();
                                operationCount++;

                                Assert.Equal("POST", operationRequestMessage.Method);

                                Assert.NotEmpty(verifyUrlStack);
                                Assert.NotNull(operationRequestMessage.Url);
                                Assert.Equal(verifyUrlStack.Pop(), operationRequestMessage.Url.OriginalString);

                                Assert.NotEmpty(verifyDependsOnIdsStack);
                                var verifyDependsOnId = verifyDependsOnIdsStack.Pop();
                                verifyDependsOnId(operationRequestMessage.DependsOnIds);

                                using (var messageReader = new ODataMessageReader(operationRequestMessage, new ODataMessageReaderSettings(), this.model))
                                {
                                    if (operationCount == 3)
                                    {
                                        var entityReferenceLink = messageReader.ReadEntityReferenceLink();

                                        Assert.Equal("$2", entityReferenceLink.Url.OriginalString);
                                    }
                                    else
                                    {
                                        var jsonResourceReader = messageReader.CreateODataResourceReader();

                                        while (jsonResourceReader.Read())
                                        {
                                            switch (jsonResourceReader.State)
                                            {
                                                case ODataReaderState.ResourceEnd:
                                                    Assert.NotEmpty(verifyResourceStack);
                                                    var innerVerifyResourceStack = verifyResourceStack.Pop();
                                                    innerVerifyResourceStack(jsonResourceReader.Item as ODataResource);
                                                    break;
                                            }
                                        }
                                    }
                                }
                                break;
                        }
                    }
                },
                batchRequestBoundary);

            Assert.Empty(verifyUrlStack);
            Assert.Empty(verifyDependsOnIdsStack);
            Assert.Empty(verifyResourceStack);
        }

        [Fact]
        public async Task ReadMultipartMixedBatchRequestAsync()
        {
            var verifyUrlStack = new Stack<string>(new[] { "$1/Orders/$ref", "http://tempuri.org/Orders", "http://tempuri.org/Customers" });

            var verifyDependsOnIdsStack = new Stack<Action<IEnumerable<string>>>();
            verifyDependsOnIdsStack.Push((dependsOnIds) =>
            {
                Assert.Equal(2, dependsOnIds.Count());
                Assert.Equal("1", dependsOnIds.First());
                Assert.Equal("2", dependsOnIds.Last());
            });
            verifyDependsOnIdsStack.Push((dependsOnIds) => Assert.Equal("1", Assert.Single(dependsOnIds)));
            verifyDependsOnIdsStack.Push((dependsOnIds) => Assert.Empty(dependsOnIds));

            var verifyResourceStack = new Stack<Action<ODataResource>>();
            verifyResourceStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Order", resource.TypeName);
                Assert.Equal(2, resource.Properties.Count());
                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Amount", properties[1].Name);
                Assert.Equal(13M, properties[1].Value);
            });
            verifyResourceStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Customer", resource.TypeName);
                Assert.Equal(2, resource.Properties.Count());
                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Name", properties[1].Name);
                Assert.Equal("Sue", properties[1].Value);
            });

            await SetupMultipartMixedBatchReaderAndRunTestAsync(
                batchRequestPayload,
                async (multipartMixedBatchReader) =>
                {
                    var operationCount = 0;

                    while (await multipartMixedBatchReader.ReadAsync())
                    {
                        switch (multipartMixedBatchReader.State)
                        {
                            case ODataBatchReaderState.Operation:
                                var operationRequestMessage = await multipartMixedBatchReader.CreateOperationRequestMessageAsync();
                                operationCount++;

                                Assert.Equal("POST", operationRequestMessage.Method);

                                Assert.NotEmpty(verifyUrlStack);
                                Assert.NotNull(operationRequestMessage.Url);
                                Assert.Equal(verifyUrlStack.Pop(), operationRequestMessage.Url.OriginalString);

                                Assert.NotEmpty(verifyDependsOnIdsStack);
                                var verifyDependsOnId = verifyDependsOnIdsStack.Pop();
                                verifyDependsOnId(operationRequestMessage.DependsOnIds);

                                using (var messageReader = new ODataMessageReader(operationRequestMessage, new ODataMessageReaderSettings(), this.model))
                                {
                                    if (operationCount == 3)
                                    {
                                        var entityReferenceLink = await messageReader.ReadEntityReferenceLinkAsync();

                                        Assert.Equal("$2", entityReferenceLink.Url.OriginalString);
                                    }
                                    else
                                    {
                                        var jsonResourceReader = await messageReader.CreateODataResourceReaderAsync();

                                        while (await jsonResourceReader.ReadAsync())
                                        {
                                            switch (jsonResourceReader.State)
                                            {
                                                case ODataReaderState.ResourceEnd:
                                                    Assert.NotEmpty(verifyResourceStack);
                                                    var innerVerifyResourceStack = verifyResourceStack.Pop();
                                                    innerVerifyResourceStack(jsonResourceReader.Item as ODataResource);
                                                    break;
                                            }
                                        }
                                    }
                                }
                                break;
                        }
                    }
                },
                batchRequestBoundary);

            Assert.Empty(verifyUrlStack);
            Assert.Empty(verifyDependsOnIdsStack);
            Assert.Empty(verifyResourceStack);
        }

        [Fact]
        public void ReadMultipartMixedBatchResponse()
        {
            bool resourceRead = false;

            SetupMultipartMixedBatchReaderAndRunTest(
                batchResponsePayload,
                (multipartMixedBatchReader) =>
                {
                    while (multipartMixedBatchReader.Read())
                    {
                        switch (multipartMixedBatchReader.State)
                        {
                            case ODataBatchReaderState.Operation:
                                var operationResponseMessage = multipartMixedBatchReader.CreateOperationResponseMessage();

                                using (var messageReader = new ODataMessageReader(operationResponseMessage, new ODataMessageReaderSettings(), this.model))
                                {
                                    var jsonResourceReader = messageReader.CreateODataResourceReader();

                                    while (jsonResourceReader.Read())
                                    {
                                        switch (jsonResourceReader.State)
                                        {
                                            case ODataReaderState.ResourceEnd:
                                                var resource = jsonResourceReader.Item as ODataResource;
                                                resourceRead = true;
                                                Assert.NotNull(resource);
                                                Assert.Equal("NS.Customer", resource.TypeName);
                                                Assert.Equal(2, resource.Properties.Count());
                                                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                                                Assert.Equal("Id", properties[0].Name);
                                                Assert.Equal(1, properties[0].Value);
                                                Assert.Equal("Name", properties[1].Name);
                                                Assert.Equal("Sue", properties[1].Value);

                                                break;
                                        }
                                    }
                                }
                                break;
                        }
                    }
                },
                batchResponseBoundary,
                isRequest: false);

            Assert.True(resourceRead);
        }

        [Fact]
        public async Task ReadMultipartMixedBatchResponseAsync()
        {
            bool resourceRead = false;

            await SetupMultipartMixedBatchReaderAndRunTestAsync(
                batchResponsePayload,
                async (multipartMixedBatchReader) =>
                {
                    while (await multipartMixedBatchReader.ReadAsync())
                    {
                        switch (multipartMixedBatchReader.State)
                        {
                            case ODataBatchReaderState.Operation:
                                var operationResponseMessage = await multipartMixedBatchReader.CreateOperationResponseMessageAsync();

                                using (var messageReader = new ODataMessageReader(operationResponseMessage, new ODataMessageReaderSettings(), this.model))
                                {
                                    var jsonResourceReader = await messageReader.CreateODataResourceReaderAsync();

                                    while (await jsonResourceReader.ReadAsync())
                                    {
                                        switch (jsonResourceReader.State)
                                        {
                                            case ODataReaderState.ResourceEnd:
                                                var resource = jsonResourceReader.Item as ODataResource;
                                                resourceRead = true;
                                                Assert.NotNull(resource);
                                                Assert.Equal("NS.Customer", resource.TypeName);
                                                Assert.Equal(2, resource.Properties.Count());
                                                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                                                Assert.Equal("Id", properties[0].Name);
                                                Assert.Equal(1, properties[0].Value);
                                                Assert.Equal("Name", properties[1].Name);
                                                Assert.Equal("Sue", properties[1].Value);

                                                break;
                                        }
                                    }
                                }
                                break;
                        }
                    }
                },
                batchResponseBoundary,
                isRequest: false);

            Assert.True(resourceRead);
        }

        #region BatchOperationReadStream

        [Theory]
        [InlineData(true, "StreamDisposed")]
        [InlineData(false, "StreamDisposedAsync")]
        public void ODataBatchReaderStreamDisposeShouldInvokeStreamDisposed(bool synchronous, string expected)
        {
            var payload = @"

{""@odata.type"":""NS.Customer"",""Id"":1,""Name"":""Sue""}
--batch_aed653ab--
";
            var stream = new MemoryStream();
            using (Stream batchReaderStream = ODataBatchUtils.CreateBatchOperationReadStream(
                CreateBatchReaderStream(payload),
                this.batchOperationHeaders,
                new MockODataStreamListener(new StreamWriter(stream)),
                synchronous: synchronous))
            {
            }

            stream.Position = 0;
            var contents = new StreamReader(stream).ReadToEnd();

            Assert.Equal(expected, contents);
        }

        [Theory]
        [InlineData(true, "StreamDisposed")]
        [InlineData(false, "StreamDisposedAsync")]
        public void ODataBatchReaderStreamDisposeShouldBeIdempotent(bool synchronous, string expected)
        {
            var payload = @"

{""@odata.type"":""NS.Customer"",""Id"":1,""Name"":""Sue""}
--batch_aed653ab--
";
            var stream = new MemoryStream();
            Stream batchReaderStream = ODataBatchUtils.CreateBatchOperationReadStream(
                CreateBatchReaderStream(payload),
                this.batchOperationHeaders,
                new MockODataStreamListener(new StreamWriter(stream)),
                synchronous: synchronous);

            // 1st call to Dispose
            batchReaderStream.Dispose();
            // 2nd call to Dispose
            batchReaderStream.Dispose();

            stream.Position = 0;
            var contents = new StreamReader(stream).ReadToEnd();

            // StreamDisposed/StreamDisposeAsync was written only once
            Assert.Equal(expected, contents);
        }

        [Fact]
        public async Task ODataBatchReaderStreamDisposeAsyncShouldInvokeStreamDisposedAsync()
        {
            var payload = @"

{""@odata.type"":""NS.Customer"",""Id"":1,""Name"":""Sue""}
--batch_aed653ab--
";
            var stream = new MemoryStream();
            await using (Stream batchReaderStream = ODataBatchUtils.CreateBatchOperationReadStream(
                CreateBatchReaderStream(payload),
                this.batchOperationHeaders,
                new MockODataStreamListener(new StreamWriter(stream))))// `synchronous` argument becomes irrelevant since we'll directly call DisposeAsync
            {
            }

            stream.Position = 0;
            var contents = await new StreamReader(stream).ReadToEndAsync();

            Assert.Equal("StreamDisposedAsync", contents);
        }

        [Fact]
        public async Task ODataBatchReaderStreamDisposeAsyncShouldBeIdempotentAsync()
        {
            var payload = @"

{""@odata.type"":""NS.Customer"",""Id"":1,""Name"":""Sue""}
--batch_aed653ab--
";
            var stream = new MemoryStream();
            Stream batchReaderStream = ODataBatchUtils.CreateBatchOperationReadStream(
                CreateBatchReaderStream(payload),
                this.batchOperationHeaders,
                new MockODataStreamListener(new StreamWriter(stream)));// `synchronous` argument becomes irrelevant since we'll directly call DisposeAsync

            // 1st call to DisposeAsync
            await batchReaderStream.DisposeAsync();
            // 2nd call to DisposeAsync
            await batchReaderStream.DisposeAsync();

            stream.Position = 0;
            var contents = await new StreamReader(stream).ReadToEndAsync();

            Assert.Equal("StreamDisposedAsync", contents);
        }

        #endregion BatchOperationReadStream
        #region BatchOperationWriteStream

        [Theory]
        [InlineData(true, "StreamDisposed")]
        [InlineData(false, "StreamDisposedAsync")]
        public void ODataBatchWriteStreamDisposeShouldInvokeStreamDisposed(bool synchronous, string expected)
        {
            var stream = new MemoryStream();
            using (Stream batchWriteStream = ODataBatchUtils.CreateBatchOperationWriteStream(
                new MemoryStream(),
                new MockODataStreamListener(new StreamWriter(stream)),
                synchronous: synchronous))
            {
            }

            stream.Position = 0;
            var contents = new StreamReader(stream).ReadToEnd();

            Assert.Equal(expected, contents);
        }

        [Theory]
        [InlineData(true, "StreamDisposed")]
        [InlineData(false, "StreamDisposedAsync")]
        public void ODataBatchWriterStreamDisposeShouldBeIdempotent(bool synchronous, string expected)
        {
            var stream = new MemoryStream();
            Stream batchWriteStream = ODataBatchUtils.CreateBatchOperationWriteStream(
                new MemoryStream(),
                new MockODataStreamListener(new StreamWriter(stream)),
                synchronous: synchronous);

            // 1st call to Dispose
            batchWriteStream.Dispose();
            // 2nd call to Dispose
            batchWriteStream.Dispose();

            stream.Position = 0;
            var contents = new StreamReader(stream).ReadToEnd();

            Assert.Equal(expected, contents);
        }

        [Fact]
        public async Task ODataBatchWriteStreamDisposeAsyncShouldInvokeStreamDisposedAsync()
        {
            var stream = new MemoryStream();
            await using (Stream batchWriteStream = ODataBatchUtils.CreateBatchOperationWriteStream(
                new MemoryStream(),
                new MockODataStreamListener(new StreamWriter(stream))))// `synchronous` argument becomes irrelevant since we'll directly call DisposeAsync
            {
            }

            stream.Position = 0;
            var contents = await new StreamReader(stream).ReadToEndAsync();

            Assert.Equal("StreamDisposedAsync", contents);
        }

        [Fact]
        public async Task ODataBatchWriteStreamDisposeAsyncShouldBeIdempotentAsync()
        {
            var stream = new MemoryStream();
            Stream batchWriteStream = ODataBatchUtils.CreateBatchOperationWriteStream(
                new MemoryStream(),
                new MockODataStreamListener(new StreamWriter(stream)));// `synchronous` argument becomes irrelevant since we'll directly call DisposeAsync

            // 1st call to DisposeAsync
            await batchWriteStream.DisposeAsync();
            // 2nd call to DisposeAsync
            await batchWriteStream.DisposeAsync();

            stream.Position = 0;
            var contents = await new StreamReader(stream).ReadToEndAsync();

            Assert.Equal("StreamDisposedAsync", contents);
        }

        #endregion BatchOperationWriteStream

        private void InitializeEdmModel()
        {
            this.model = new EdmModel();

            var orderEntityType = new EdmEntityType("NS", "Order", /*baseType*/ null, /*isAbstract*/ false, /*isOpen*/ true);
            var customerEntityType = new EdmEntityType("NS", "Customer");

            var orderIdProperty = orderEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            orderEntityType.AddKeys(orderIdProperty);
            orderEntityType.AddStructuralProperty("CustomerId", EdmPrimitiveTypeKind.Int32);
            orderEntityType.AddStructuralProperty("Amount", EdmPrimitiveTypeKind.Decimal);
            var customerNavProperty = orderEntityType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo
                {
                    Name = "Customer",
                    Target = customerEntityType,
                    TargetMultiplicity = EdmMultiplicity.ZeroOrOne
                });
            model.AddElement(orderEntityType);

            var customerIdProperty = customerEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            customerEntityType.AddKeys(customerIdProperty);
            customerEntityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            var ordersNavProperty = customerEntityType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo
                {
                    Name = "Orders",
                    Target = orderEntityType,
                    TargetMultiplicity = EdmMultiplicity.Many
                });
            this.model.AddElement(customerEntityType);

            var entityContainer = new EdmEntityContainer("NS", "Container");
            this.model.AddElement(entityContainer);

            var orderEntitySet = entityContainer.AddEntitySet("Orders", orderEntityType);
            var customerEntitySet = entityContainer.AddEntitySet("Customers", customerEntityType);

            orderEntitySet.AddNavigationTarget(customerNavProperty, customerEntitySet);
            customerEntitySet.AddNavigationTarget(ordersNavProperty, orderEntitySet);
        }

        private ODataMultipartMixedBatchReaderStream CreateBatchReaderStream(string payload, bool synchronous = true, bool isRequest = true)
        {
            var multipartMixedBatchInputContext = CreateMultipartMixedBatchInputContext(
                payload,
                isRequest,
                synchronous: synchronous);

            return new ODataMultipartMixedBatchReaderStream(
                multipartMixedBatchInputContext,
                batchRequestBoundary,
                MediaTypeUtils.EncodingUtf8NoPreamble);
        }

        /// <summary>
        /// Sets up an ODataMultipartMixedBatchReader, then runs the given test code
        /// </summary>
        private void SetupMultipartMixedBatchReaderAndRunTest(
            string payload,
            Action<ODataMultipartMixedBatchReader> action,
            string batchBoundary,
            bool isRequest = true)
        {
            var multipartMixedBatchInputContext = CreateMultipartMixedBatchInputContext(
                payload,
                isRequest,
                synchronous: true);
            var multipartMixedBatchReader = new ODataMultipartMixedBatchReader(
                multipartMixedBatchInputContext,
                batchBoundary,
                MediaTypeUtils.EncodingUtf8NoPreamble,
                synchronous: true);

            action(multipartMixedBatchReader);
        }

        /// <summary>
        /// Sets up an ODataMultipartMixedBatchReader, then runs the given test code asynchronously
        /// </summary>
        private async Task SetupMultipartMixedBatchReaderAndRunTestAsync(
            string payload,
            Func<ODataMultipartMixedBatchReader, Task> func,
            string batchBoundary,
            bool isRequest = true)
        {
            var multipartMixedBatchInputContext = CreateMultipartMixedBatchInputContext(
                payload,
                isRequest,
                synchronous: false);
            var multipartMixedBatchReader = new ODataMultipartMixedBatchReader(
                multipartMixedBatchInputContext,
                batchBoundary,
                MediaTypeUtils.EncodingUtf8NoPreamble,
                synchronous: false);

            await func(multipartMixedBatchReader);
        }

        private ODataMultipartMixedBatchInputContext CreateMultipartMixedBatchInputContext(
            string payload,
            bool isRequest = true,
            bool synchronous = true)
        {
            var encoding = MediaTypeUtils.EncodingUtf8NoPreamble;
            var messageInfo = new ODataMessageInfo
            {
                MessageStream = new MemoryStream(encoding.GetBytes(payload)),
                MediaType = this.mediaType,
                Encoding = encoding,
                IsResponse = !isRequest,
                IsAsync = !synchronous
            };

            return new ODataMultipartMixedBatchInputContext(ODataFormat.Batch, messageInfo, this.messageReaderSettings);
        }

        private class MockODataStreamListener : IODataStreamListener
        {
            private TextWriter writer;

            public MockODataStreamListener(TextWriter writer)
            {
                this.writer = writer;
            }

            public void StreamDisposed()
            {
                writer.Write("StreamDisposed");
                writer.Flush();
            }

            public async Task StreamDisposedAsync()
            {
                await writer.WriteAsync("StreamDisposedAsync").ConfigureAwait(false);
                await writer.FlushAsync().ConfigureAwait(false);
            }

            public void StreamRequested()
            {
                throw new NotImplementedException();
            }

            public Task StreamRequestedAsync()
            {
                throw new NotImplementedException();
            }
        }
    }
}

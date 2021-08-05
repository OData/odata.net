//---------------------------------------------------------------------
// <copyright file="ODataMultipartMixedBatchOutputContextApiTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class ODataMultipartMixedBatchOutputContextApiTests
    {
        private const string ServiceUri = "http://tempuri.org";
        private readonly MemoryStream asyncStream;
        private readonly MemoryStream syncStream;
        private readonly ODataMessageWriterSettings writerSettings;
        private const string batchBoundary = "batch_aed653ab";
        private const string batchResponseBoundary = "batchresponse_aed653ab";
        // Regex used to replace the random batch boundary before equivalence check
        private const string batchGuidRegex = @"batch[a-z]*_[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}";

        private EdmModel model;
        private EdmEntityType orderEntityType;
        private EdmEntityType customerEntityType;
        private EdmEntitySet orderEntitySet;
        private EdmEntitySet customerEntitySet;

        public ODataMultipartMixedBatchOutputContextApiTests()
        {
            this.InitializeEdmModel();
            this.asyncStream = new MemoryStream();
            this.syncStream = new MemoryStream();
            this.writerSettings = new ODataMessageWriterSettings
            {
                Version = ODataVersion.V4,
                EnableMessageStreamDisposal = false,
                BaseUri = new Uri(ServiceUri)
            };

            this.writerSettings.SetServiceDocumentUri(new Uri(ServiceUri));
            this.writerSettings.SetContentType(ODataFormat.Batch);
        }

        [Fact]
        public async Task WriteMultipartMixedBatchRequest_APIsShouldYieldSameResult()
        {
            var orderResource = CreateOrderResource();

            IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
            {
                var multipartMixedBatchWriter = await messageWriter.CreateODataBatchWriterAsync();
                await multipartMixedBatchWriter.WriteStartBatchAsync();

                var operationRequestMessage = await multipartMixedBatchWriter.CreateOperationRequestMessageAsync(
                    "POST", new Uri($"{ServiceUri}/Orders"), "1");

                using (var nestedMessageWriter = new ODataMessageWriter(operationRequestMessage))
                {
                    var writer = await nestedMessageWriter.CreateODataResourceWriterAsync(this.orderEntitySet, this.orderEntityType);
                    await writer.WriteStartAsync(orderResource);
                    await writer.WriteEndAsync();
                }

                await multipartMixedBatchWriter.WriteEndBatchAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();
            asyncResult = Regex.Replace(asyncResult, batchGuidRegex, batchBoundary);

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(() =>
            {
                IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                {
                    var multipartMixedBatchWriter = messageWriter.CreateODataBatchWriter();
                    multipartMixedBatchWriter.WriteStartBatch();

                    var operationRequestMessage = multipartMixedBatchWriter.CreateOperationRequestMessage(
                        "POST", new Uri($"{ServiceUri}/Orders"), "1");

                    using (var nestedMessageWriter = new ODataMessageWriter(operationRequestMessage))
                    {
                        var writer = nestedMessageWriter.CreateODataResourceWriter(this.orderEntitySet, this.orderEntityType);
                        writer.WriteStart(orderResource);
                        writer.WriteEnd();
                    }

                    multipartMixedBatchWriter.WriteEndBatch();
                }

                this.syncStream.Position = 0;
                var result = new StreamReader(this.syncStream).ReadToEnd();

                return Regex.Replace(result, batchGuidRegex, batchBoundary);
            });

            var expected = @"--batch_aed653ab
Content-Type: application/http
Content-Transfer-Encoding: binary

POST http://tempuri.org/Orders HTTP/1.1
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""Id"":1,""Amount"":13}
--batch_aed653ab--
";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        public async Task WriteMultipartMixedBatchRequestWithSingleChangeset_APIsShouldYieldSameResult()
        {
            var orderResource = CreateOrderResource();
            var customerResource = CreateCustomerResource();

            IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
            {
                var multipartMixedBatchWriter = await messageWriter.CreateODataBatchWriterAsync();
                await multipartMixedBatchWriter.WriteStartBatchAsync();
                await multipartMixedBatchWriter.WriteStartChangesetAsync("ec3a8d4f");

                var operationRequestMessage1 = await multipartMixedBatchWriter.CreateOperationRequestMessageAsync(
                    "POST", new Uri($"{ServiceUri}/Orders"), "1");

                using (var messageWriter1 = new ODataMessageWriter(operationRequestMessage1))
                {
                    var jsonLightWriter = await messageWriter1.CreateODataResourceWriterAsync(this.orderEntitySet, this.orderEntityType);
                    await jsonLightWriter.WriteStartAsync(orderResource);
                    await jsonLightWriter.WriteEndAsync();
                }

                var dependsOnIds = new List<string> { "1" };
                var operationRequestMessage2 = await multipartMixedBatchWriter.CreateOperationRequestMessageAsync(
                    "POST", new Uri($"{ServiceUri}/Customers"), "2", BatchPayloadUriOption.AbsoluteUri, dependsOnIds);

                using (var messageWriter2 = new ODataMessageWriter(operationRequestMessage2))
                {
                    var jsonLightWriter = await messageWriter2.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);
                    await jsonLightWriter.WriteStartAsync(customerResource);
                    await jsonLightWriter.WriteEndAsync();
                }

                await multipartMixedBatchWriter.WriteEndChangesetAsync();
                await multipartMixedBatchWriter.WriteEndBatchAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();
            asyncResult = Regex.Replace(asyncResult, batchGuidRegex, batchBoundary);

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(() =>
            {
                IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                {
                    var multipartMixedBatchWriter = messageWriter.CreateODataBatchWriter();
                    multipartMixedBatchWriter.WriteStartBatch();
                    multipartMixedBatchWriter.WriteStartChangeset("ec3a8d4f");

                    var operationRequestMessage1 = multipartMixedBatchWriter.CreateOperationRequestMessage(
                        "POST", new Uri($"{ServiceUri}/Orders"), "1");

                    using (var messageWriter1 = new ODataMessageWriter(operationRequestMessage1))
                    {
                        var jsonLightWriter = messageWriter1.CreateODataResourceWriter(this.orderEntitySet, this.orderEntityType);
                        jsonLightWriter.WriteStart(orderResource);
                        jsonLightWriter.WriteEnd();
                    }

                    var dependsOnIds = new List<string> { "1" };
                    var operationRequestMessage2 = multipartMixedBatchWriter.CreateOperationRequestMessage(
                        "POST", new Uri($"{ServiceUri}/Customers"), "2", BatchPayloadUriOption.AbsoluteUri, dependsOnIds);

                    using (var messageWriter2 = new ODataMessageWriter(operationRequestMessage2))
                    {
                        var jsonLightWriter = messageWriter2.CreateODataResourceWriter(this.customerEntitySet, this.customerEntityType);
                        jsonLightWriter.WriteStart(customerResource);
                        jsonLightWriter.WriteEnd();
                    }

                    multipartMixedBatchWriter.WriteEndChangeset();
                    multipartMixedBatchWriter.WriteEndBatch();
                }

                this.syncStream.Position = 0;
                var result = new StreamReader(this.syncStream).ReadToEnd();

                return Regex.Replace(result, batchGuidRegex, batchBoundary);
            });

            var expected = @"--batch_aed653ab
Content-Type: multipart/mixed; boundary=changeset_ec3a8d4f

--changeset_ec3a8d4f
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

POST http://tempuri.org/Orders HTTP/1.1
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""Id"":1,""Amount"":13}
--changeset_ec3a8d4f
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2

POST http://tempuri.org/Customers HTTP/1.1
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""Id"":1,""Name"":""Customer 1""}
--changeset_ec3a8d4f--
--batch_aed653ab--
";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        public async Task WriteMultipartMixedBatchRequestWithMultipleChangeset_APIsYieldSameResult()
        {
            var orderResource = CreateOrderResource();
            var customerResource = CreateCustomerResource();

            IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
            {
                var multipartMixedBatchWriter = await messageWriter.CreateODataBatchWriterAsync();
                await multipartMixedBatchWriter.WriteStartBatchAsync();
                await multipartMixedBatchWriter.WriteStartChangesetAsync("ec3a8d4f");

                var operationRequestMessage1 = await multipartMixedBatchWriter.CreateOperationRequestMessageAsync(
                    "PUT", new Uri($"{ServiceUri}/Orders(1)"), "1");

                using (var messageWriter1 = new ODataMessageWriter(operationRequestMessage1))
                {
                    var jsonLightWriter = await messageWriter1.CreateODataResourceWriterAsync(this.orderEntitySet, this.orderEntityType);
                    await jsonLightWriter.WriteStartAsync(orderResource);
                    await jsonLightWriter.WriteEndAsync();
                }

                await multipartMixedBatchWriter.WriteEndChangesetAsync();
                await multipartMixedBatchWriter.WriteStartChangesetAsync("f46c46e2");

                var operationRequestMessage2 = await multipartMixedBatchWriter.CreateOperationRequestMessageAsync(
                    "PUT", new Uri($"{ServiceUri}/Customers(1)"), "2");

                using (var messageWriter2 = new ODataMessageWriter(operationRequestMessage2))
                {
                    var jsonLightWriter = await messageWriter2.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);
                    await jsonLightWriter.WriteStartAsync(customerResource);
                    await jsonLightWriter.WriteEndAsync();
                }

                await multipartMixedBatchWriter.WriteEndChangesetAsync();
                await multipartMixedBatchWriter.WriteEndBatchAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();
            asyncResult = Regex.Replace(asyncResult, batchGuidRegex, batchBoundary);

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(() =>
            {
                IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                {
                    var multipartMixedBatchWriter = messageWriter.CreateODataBatchWriter();
                    multipartMixedBatchWriter.WriteStartBatch();
                    multipartMixedBatchWriter.WriteStartChangeset("ec3a8d4f");

                    var operationRequestMessage1 = multipartMixedBatchWriter.CreateOperationRequestMessage(
                        "PUT", new Uri($"{ServiceUri}/Orders(1)"), "1");

                    using (var messageWriter1 = new ODataMessageWriter(operationRequestMessage1))
                    {
                        var jsonLightWriter = messageWriter1.CreateODataResourceWriter(this.orderEntitySet, this.orderEntityType);
                        jsonLightWriter.WriteStart(orderResource);
                        jsonLightWriter.WriteEnd();
                    }

                    multipartMixedBatchWriter.WriteEndChangeset();
                    multipartMixedBatchWriter.WriteStartChangeset("f46c46e2");

                    var operationRequestMessage2 = multipartMixedBatchWriter.CreateOperationRequestMessage(
                        "PUT", new Uri($"{ServiceUri}/Customers(1)"), "2");

                    using (var messageWriter2 = new ODataMessageWriter(operationRequestMessage2))
                    {
                        var jsonLightWriter = messageWriter2.CreateODataResourceWriter(this.customerEntitySet, this.customerEntityType);
                        jsonLightWriter.WriteStart(customerResource);
                        jsonLightWriter.WriteEnd();
                    }

                    multipartMixedBatchWriter.WriteEndChangeset();
                    multipartMixedBatchWriter.WriteEndBatch();
                }

                this.syncStream.Position = 0;
                var result = new StreamReader(this.syncStream).ReadToEnd();

                return Regex.Replace(result, batchGuidRegex, batchBoundary);
            });

            var expected = @"--batch_aed653ab
Content-Type: multipart/mixed; boundary=changeset_ec3a8d4f

--changeset_ec3a8d4f
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

PUT http://tempuri.org/Orders(1) HTTP/1.1
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""Id"":1,""Amount"":13}
--changeset_ec3a8d4f--
--batch_aed653ab
Content-Type: multipart/mixed; boundary=changeset_f46c46e2

--changeset_f46c46e2
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2

PUT http://tempuri.org/Customers(1) HTTP/1.1
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""Id"":1,""Name"":""Customer 1""}
--changeset_f46c46e2--
--batch_aed653ab--
";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        public async Task WriteMultipartMixedBatchRequest_APIsYieldSameResultForChangesetIdNotSpecified()
        {
            var orderResource = CreateOrderResource();
            var changesetGuidRegex = batchGuidRegex.Replace("batch", "changeset");
            var changesetBoundary = "changeset_ec3a8d4f";

            IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
            {
                var multipartMixedBatchWriter = await messageWriter.CreateODataBatchWriterAsync();
                await multipartMixedBatchWriter.WriteStartBatchAsync();
                await multipartMixedBatchWriter.WriteStartChangesetAsync();

                var operationRequestMessage = await multipartMixedBatchWriter.CreateOperationRequestMessageAsync(
                    "POST", new Uri($"{ServiceUri}/Orders"), "1");

                using (var nestedMessageWriter = new ODataMessageWriter(operationRequestMessage))
                {
                    var jsonLightWriter = await nestedMessageWriter.CreateODataResourceWriterAsync(this.orderEntitySet, this.orderEntityType);
                    await jsonLightWriter.WriteStartAsync(orderResource);
                    await jsonLightWriter.WriteEndAsync();
                }

                await multipartMixedBatchWriter.WriteEndChangesetAsync();
                await multipartMixedBatchWriter.WriteEndBatchAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();
            asyncResult = Regex.Replace(asyncResult, batchGuidRegex, batchBoundary);
            // Changeset group id will be a random guid
            asyncResult = Regex.Replace(asyncResult, changesetGuidRegex, changesetBoundary);

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(() =>
            {
                IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                {
                    var multipartMixedBatchWriter = messageWriter.CreateODataBatchWriter();
                    multipartMixedBatchWriter.WriteStartBatch();
                    multipartMixedBatchWriter.WriteStartChangeset();

                    var operationRequestMessage = multipartMixedBatchWriter.CreateOperationRequestMessage(
                        "POST", new Uri($"{ServiceUri}/Orders"), "1");

                    using (var nestedMessageWriter = new ODataMessageWriter(operationRequestMessage))
                    {
                        var jsonLightWriter = nestedMessageWriter.CreateODataResourceWriter(this.orderEntitySet, this.orderEntityType);
                        jsonLightWriter.WriteStart(orderResource);
                        jsonLightWriter.WriteEnd();
                    }

                    multipartMixedBatchWriter.WriteEndChangeset();
                    multipartMixedBatchWriter.WriteEndBatch();
                }

                this.syncStream.Position = 0;
                var result = new StreamReader(this.syncStream).ReadToEnd();

                result = Regex.Replace(result, batchGuidRegex, batchBoundary);
                // Changeset group id will be a random guid
                return Regex.Replace(result, changesetGuidRegex, changesetBoundary);
            });

            var expected = @"--batch_aed653ab
Content-Type: multipart/mixed; boundary=changeset_ec3a8d4f

--changeset_ec3a8d4f
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

POST http://tempuri.org/Orders HTTP/1.1
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""Id"":1,""Amount"":13}
--changeset_ec3a8d4f--
--batch_aed653ab--
";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        public async Task WriteMultipartMixedBatchRequest_APIsYieldSameResultForContentIdIsNull()
        {
            // NOTE: Content IDs are not written into the payload when there are no changesets so null is allowed
            var orderResource = CreateOrderResource();

            IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
            {
                var multipartMixedBatchWriter = await messageWriter.CreateODataBatchWriterAsync();
                await multipartMixedBatchWriter.WriteStartBatchAsync();

                var operationRequestMessage = await multipartMixedBatchWriter.CreateOperationRequestMessageAsync(
                    "POST", new Uri($"{ServiceUri}/Orders"), /*contentId*/ null);

                using (var nestedMessageWriter = new ODataMessageWriter(operationRequestMessage))
                {
                    var jsonLightWriter = await nestedMessageWriter.CreateODataResourceWriterAsync(this.orderEntitySet, this.orderEntityType);
                    await jsonLightWriter.WriteStartAsync(orderResource);
                    await jsonLightWriter.WriteEndAsync();
                }

                await multipartMixedBatchWriter.WriteEndBatchAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();
            asyncResult = Regex.Replace(asyncResult, batchGuidRegex, batchBoundary);

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(() =>
            {
                IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                {
                    var multipartMixedBatchWriter = messageWriter.CreateODataBatchWriter();
                    multipartMixedBatchWriter.WriteStartBatch();

                    var operationRequestMessage = multipartMixedBatchWriter.CreateOperationRequestMessage(
                        "POST", new Uri($"{ServiceUri}/Orders"), /*contentId*/ null);

                    using (var nestedMessageWriter = new ODataMessageWriter(operationRequestMessage))
                    {
                        var jsonLightWriter = nestedMessageWriter.CreateODataResourceWriter(this.orderEntitySet, this.orderEntityType);
                        jsonLightWriter.WriteStart(orderResource);
                        jsonLightWriter.WriteEnd();
                    }

                    multipartMixedBatchWriter.WriteEndBatch();
                }

                this.syncStream.Position = 0;
                var result = new StreamReader(this.syncStream).ReadToEnd();

                return Regex.Replace(result, batchGuidRegex, batchBoundary);
            });

            var expected = @"--batch_aed653ab
Content-Type: application/http
Content-Transfer-Encoding: binary

POST http://tempuri.org/Orders HTTP/1.1
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""Id"":1,""Amount"":13}
--batch_aed653ab--
";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        public async Task WriteMultipartMixedBatchRequest_APIsYieldSameResultForAbsoluteUriUsingHostHeader()
        {
            var orderResource = CreateOrderResource();

            IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
            {
                var multipartMixedBatchWriter = await messageWriter.CreateODataBatchWriterAsync();
                await multipartMixedBatchWriter.WriteStartBatchAsync();

                var operationRequestMessage = await multipartMixedBatchWriter.CreateOperationRequestMessageAsync(
                    "POST", new Uri($"{ServiceUri}/Orders"), /*contentId*/ null, BatchPayloadUriOption.AbsoluteUriUsingHostHeader);

                using (var nestedMessageWriter = new ODataMessageWriter(operationRequestMessage))
                {
                    var jsonLightWriter = await nestedMessageWriter.CreateODataResourceWriterAsync(this.orderEntitySet, this.orderEntityType);
                    await jsonLightWriter.WriteStartAsync(orderResource);
                    await jsonLightWriter.WriteEndAsync();
                }

                await multipartMixedBatchWriter.WriteEndBatchAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();
            asyncResult = Regex.Replace(asyncResult, batchGuidRegex, batchBoundary);

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(() =>
            {
                IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                {
                    var multipartMixedBatchWriter = messageWriter.CreateODataBatchWriter();
                    multipartMixedBatchWriter.WriteStartBatch();

                    var operationRequestMessage = multipartMixedBatchWriter.CreateOperationRequestMessage(
                    "POST", new Uri($"{ServiceUri}/Orders"), /*contentId*/ null, BatchPayloadUriOption.AbsoluteUriUsingHostHeader);

                    using (var nestedMessageWriter = new ODataMessageWriter(operationRequestMessage))
                    {
                        var jsonLightWriter = nestedMessageWriter.CreateODataResourceWriter(this.orderEntitySet, this.orderEntityType);
                        jsonLightWriter.WriteStart(orderResource);
                        jsonLightWriter.WriteEnd();
                    }

                    multipartMixedBatchWriter.WriteEndBatch();
                }

                this.syncStream.Position = 0;
                var result = new StreamReader(this.syncStream).ReadToEnd();

                return Regex.Replace(result, batchGuidRegex, batchBoundary);
            });

            var expected = @"--batch_aed653ab
Content-Type: application/http
Content-Transfer-Encoding: binary

POST /Orders HTTP/1.1
Host: tempuri.org:80
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""Id"":1,""Amount"":13}
--batch_aed653ab--
";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        public async Task WriteMultipartMixedBatchRequest_APIsYieldSameResultForRelativeUri()
        {
            var orderResource = CreateOrderResource();

            IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
            {
                var multipartMixedBatchWriter = await messageWriter.CreateODataBatchWriterAsync();
                await multipartMixedBatchWriter.WriteStartBatchAsync();

                var operationRequestMessage = await multipartMixedBatchWriter.CreateOperationRequestMessageAsync(
                    "POST", new Uri("/Orders", UriKind.Relative), /*contentId*/ null, BatchPayloadUriOption.RelativeUri);

                using (var nestedMessageWriter = new ODataMessageWriter(operationRequestMessage))
                {
                    var jsonLightWriter = await nestedMessageWriter.CreateODataResourceWriterAsync(this.orderEntitySet, this.orderEntityType);
                    await jsonLightWriter.WriteStartAsync(orderResource);
                    await jsonLightWriter.WriteEndAsync();
                }

                await multipartMixedBatchWriter.WriteEndBatchAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();
            asyncResult = Regex.Replace(asyncResult, batchGuidRegex, batchBoundary);

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(() =>
            {
                IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                {
                    var multipartMixedBatchWriter = messageWriter.CreateODataBatchWriter();
                    multipartMixedBatchWriter.WriteStartBatch();

                    var operationRequestMessage = multipartMixedBatchWriter.CreateOperationRequestMessage(
                    "POST", new Uri("/Orders", UriKind.Relative), /*contentId*/ null, BatchPayloadUriOption.RelativeUri);

                    using (var nestedMessageWriter = new ODataMessageWriter(operationRequestMessage))
                    {
                        var jsonLightWriter = nestedMessageWriter.CreateODataResourceWriter(this.orderEntitySet, this.orderEntityType);
                        jsonLightWriter.WriteStart(orderResource);
                        jsonLightWriter.WriteEnd();
                    }

                    multipartMixedBatchWriter.WriteEndBatch();
                }

                this.syncStream.Position = 0;
                var result = new StreamReader(this.syncStream).ReadToEnd();

                return Regex.Replace(result, batchGuidRegex, batchBoundary);
            });

            var expected = @"--batch_aed653ab
Content-Type: application/http
Content-Transfer-Encoding: binary

POST Orders HTTP/1.1
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""Id"":1,""Amount"":13}
--batch_aed653ab--
";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        public async Task WriteMultipartMixedBatchRequest_APIsYieldSameResultForReportMessageCompleted()
        {
            IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
            {
                var multipartMixedBatchWriter = await messageWriter.CreateODataBatchWriterAsync();
                await multipartMixedBatchWriter.WriteStartBatchAsync();

                await multipartMixedBatchWriter.CreateOperationRequestMessageAsync(
                    "POST", new Uri($"{ServiceUri}/Orders"), /*contentId*/ null);
                // No writer created for the request message
                await multipartMixedBatchWriter.WriteEndBatchAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();
            asyncResult = Regex.Replace(asyncResult, batchGuidRegex, batchBoundary);

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(() =>
            {
                IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                {
                    var multipartMixedBatchWriter = messageWriter.CreateODataBatchWriter();
                    multipartMixedBatchWriter.WriteStartBatch();

                    multipartMixedBatchWriter.CreateOperationRequestMessage(
                    "POST", new Uri($"{ServiceUri}/Orders"), /*contentId*/ null);
                    // No writer created for the request message
                    multipartMixedBatchWriter.WriteEndBatch();
                }

                this.syncStream.Position = 0;
                var result = new StreamReader(this.syncStream).ReadToEnd();

                return Regex.Replace(result, batchGuidRegex, batchBoundary);
            });

            var expected = @"--batch_aed653ab
Content-Type: application/http
Content-Transfer-Encoding: binary

POST http://tempuri.org/Orders HTTP/1.1


--batch_aed653ab--
";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        public async Task WriteMultipartMixedBatchResponse_APIsYieldSameResult()
        {
            var orderResource = CreateOrderResource();
            var nestedWriterSettings = new ODataMessageWriterSettings
            {
                ODataUri = new ODataUri { ServiceRoot = new Uri(ServiceUri) }
            };

            IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
            {
                var multipartMixedBatchWriter = await messageWriter.CreateODataBatchWriterAsync();
                await multipartMixedBatchWriter.WriteStartBatchAsync();

                var operationResponseMessage = await multipartMixedBatchWriter.CreateOperationResponseMessageAsync("1");
                operationResponseMessage.StatusCode = 200;

                using (var nestedMessageWriter = new ODataMessageWriter(operationResponseMessage, nestedWriterSettings))
                {
                    var jsonLightWriter = await nestedMessageWriter.CreateODataResourceWriterAsync(this.orderEntitySet, this.orderEntityType);
                    await jsonLightWriter.WriteStartAsync(orderResource);
                    await jsonLightWriter.WriteEndAsync();
                }

                await multipartMixedBatchWriter.WriteEndBatchAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();
            asyncResult = Regex.Replace(asyncResult, batchGuidRegex, batchResponseBoundary);

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(() =>
            {
                IODataResponseMessage syncResponseMessage = new InMemoryMessage { Stream = this.syncStream };
                using (var messageWriter = new ODataMessageWriter(syncResponseMessage, this.writerSettings))
                {
                    var multipartMixedBatchWriter = messageWriter.CreateODataBatchWriter();
                    multipartMixedBatchWriter.WriteStartBatch();

                    var operationResponseMessage = multipartMixedBatchWriter.CreateOperationResponseMessage("1");
                    operationResponseMessage.StatusCode = 200;

                    using (var nestedMessageWriter = new ODataMessageWriter(operationResponseMessage, nestedWriterSettings))
                    {
                        var jsonLightWriter = nestedMessageWriter.CreateODataResourceWriter(this.orderEntitySet, this.orderEntityType);
                        jsonLightWriter.WriteStart(orderResource);
                        jsonLightWriter.WriteEnd();
                    }

                    multipartMixedBatchWriter.WriteEndBatch();
                }

                this.syncStream.Position = 0;
                var result = new StreamReader(this.syncStream).ReadToEnd();

                return Regex.Replace(result, batchGuidRegex, batchResponseBoundary);
            });

            var expected = @"--batchresponse_aed653ab
Content-Type: application/http
Content-Transfer-Encoding: binary

HTTP/1.1 200 OK
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""@odata.context"":""http://tempuri.org/$metadata#Orders/$entity"",""Id"":1,""Amount"":13}
--batchresponse_aed653ab--
";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        public async Task WriteMultipartMixedBatchResponseWithChangeset_APIsYieldSameResult()
        {
            var orderResource = CreateOrderResource();
            var customerResource = CreateCustomerResource();

            var nestedWriterSettings = new ODataMessageWriterSettings
            {
                ODataUri = new ODataUri { ServiceRoot = new Uri(ServiceUri) }
            };

            IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
            {
                var multipartMixedBatchWriter = await messageWriter.CreateODataBatchWriterAsync();
                await multipartMixedBatchWriter.WriteStartBatchAsync();
                await multipartMixedBatchWriter.WriteStartChangesetAsync("ec3a8d4f");

                var operationResponseMessage1 = await multipartMixedBatchWriter.CreateOperationResponseMessageAsync("1");
                operationResponseMessage1.StatusCode = 200;

                using (var messageWriter1 = new ODataMessageWriter(operationResponseMessage1, nestedWriterSettings))
                {
                    var jsonLightWriter = await messageWriter1.CreateODataResourceWriterAsync(this.orderEntitySet, this.orderEntityType);
                    await jsonLightWriter.WriteStartAsync(orderResource);
                    await jsonLightWriter.WriteEndAsync();
                }

                await multipartMixedBatchWriter.WriteEndChangesetAsync();
                await multipartMixedBatchWriter.WriteStartChangesetAsync("f46c46e2");

                var operationResponseMessage2 = await multipartMixedBatchWriter.CreateOperationResponseMessageAsync("2");
                operationResponseMessage2.StatusCode = 200;

                using (var messageWriter2 = new ODataMessageWriter(operationResponseMessage2, nestedWriterSettings))
                {
                    var jsonLightWriter = await messageWriter2.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);
                    await jsonLightWriter.WriteStartAsync(customerResource);
                    await jsonLightWriter.WriteEndAsync();
                }

                await multipartMixedBatchWriter.WriteEndChangesetAsync();
                await multipartMixedBatchWriter.WriteEndBatchAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();
            asyncResult = Regex.Replace(asyncResult, batchGuidRegex, batchResponseBoundary);

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(() =>
            {
                IODataResponseMessage syncResponseMessage = new InMemoryMessage { Stream = this.syncStream };
                using (var messageWriter = new ODataMessageWriter(syncResponseMessage, this.writerSettings))
                {
                    var multipartMixedBatchWriter = messageWriter.CreateODataBatchWriter();
                    multipartMixedBatchWriter.WriteStartBatch();
                    multipartMixedBatchWriter.WriteStartChangeset("ec3a8d4f");

                    var operationResponseMessage1 = multipartMixedBatchWriter.CreateOperationResponseMessage("1");
                    operationResponseMessage1.StatusCode = 200;

                    using (var messageWriter1 = new ODataMessageWriter(operationResponseMessage1, nestedWriterSettings))
                    {
                        var jsonLightWriter = messageWriter1.CreateODataResourceWriter(this.orderEntitySet, this.orderEntityType);
                        jsonLightWriter.WriteStart(orderResource);
                        jsonLightWriter.WriteEnd();
                    }

                    multipartMixedBatchWriter.WriteEndChangeset();
                    multipartMixedBatchWriter.WriteStartChangeset("f46c46e2");

                    var operationResponseMessage2 = multipartMixedBatchWriter.CreateOperationResponseMessage("2");
                    operationResponseMessage2.StatusCode = 200;

                    using (var messageWriter2 = new ODataMessageWriter(operationResponseMessage2, nestedWriterSettings))
                    {
                        var jsonLightWriter = messageWriter2.CreateODataResourceWriter(this.customerEntitySet, this.customerEntityType);
                        jsonLightWriter.WriteStart(customerResource);
                        jsonLightWriter.WriteEnd();
                    }

                    multipartMixedBatchWriter.WriteEndChangeset();
                    multipartMixedBatchWriter.WriteEndBatch();
                }

                this.syncStream.Position = 0;
                var result = new StreamReader(this.syncStream).ReadToEnd();

                return Regex.Replace(result, batchGuidRegex, batchResponseBoundary);
            });

            var expected = @"--batchresponse_aed653ab
Content-Type: multipart/mixed; boundary=changesetresponse_ec3a8d4f

--changesetresponse_ec3a8d4f
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

HTTP/1.1 200 OK
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""@odata.context"":""http://tempuri.org/$metadata#Orders/$entity"",""Id"":1,""Amount"":13}
--changesetresponse_ec3a8d4f--
--batchresponse_aed653ab
Content-Type: multipart/mixed; boundary=changesetresponse_f46c46e2

--changesetresponse_f46c46e2
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2

HTTP/1.1 200 OK
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""@odata.context"":""http://tempuri.org/$metadata#Customers/$entity"",""Id"":1,""Name"":""Customer 1""}
--changesetresponse_f46c46e2--
--batchresponse_aed653ab--
";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        #region Exception Cases

        [Fact]
        public async Task WriteMultipartMixedBatchRequestWithChangeset_APIsYieldSameResultForContentIdIsNull()
        {
            // NOTE: Content IDs are mandatory when writing changesets. Otherwise an exception is thrown if not provided
            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
                    {
                        var multipartMixedBatchWriter = await messageWriter.CreateODataBatchWriterAsync();
                        await multipartMixedBatchWriter.WriteStartBatchAsync();
                        await multipartMixedBatchWriter.WriteStartChangesetAsync();

                        var operationRequestMessage = await multipartMixedBatchWriter.CreateOperationRequestMessageAsync(
                            "POST", new Uri($"{ServiceUri}/Orders"), /*contentId*/ null);
                        // Try to create operation request message with null content id
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(() =>
                {
                    IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                    {
                        var multipartMixedBatchWriter = messageWriter.CreateODataBatchWriter();
                        multipartMixedBatchWriter.WriteStartBatch();
                        multipartMixedBatchWriter.WriteStartChangeset();

                        var operationRequestMessage = multipartMixedBatchWriter.CreateOperationRequestMessage(
                            "POST", new Uri($"{ServiceUri}/Orders"), /*contentId*/ null);
                        // Try to create operation request message with null content id
                    }
                }));

            Assert.Equal(Strings.ODataBatchOperationHeaderDictionary_KeyNotFound("Content-ID"), asyncException.Message);
            Assert.Equal(Strings.ODataBatchOperationHeaderDictionary_KeyNotFound("Content-ID"), syncException.Message);
        }

        [Fact]
        public async Task WriteMultipartMixedBatchRequest_APIsYieldSameResultForDependsOnIdNotFound()
        {
            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
                    {
                        var multipartMixedBatchWriter = await messageWriter.CreateODataBatchWriterAsync();
                        await multipartMixedBatchWriter.WriteStartBatchAsync();
                        await multipartMixedBatchWriter.WriteStartChangesetAsync("ec3a8d4f");

                        var operationRequestMessage1 = await multipartMixedBatchWriter.CreateOperationRequestMessageAsync(
                            "POST", new Uri($"{ServiceUri}/Customers"), "1");

                        // Try to use a content id not matched to a preceding request
                        var dependsOnIds = new List<string> { "3" };
                        var operationRequestMessage2 = await multipartMixedBatchWriter.CreateOperationRequestMessageAsync(
                            "POST", new Uri($"{ServiceUri}/Orders"), "2", BatchPayloadUriOption.AbsoluteUri, dependsOnIds);
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(() =>
                {
                    IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                    {
                        var multipartMixedBatchWriter = messageWriter.CreateODataBatchWriter();
                        multipartMixedBatchWriter.WriteStartBatch();
                        multipartMixedBatchWriter.WriteStartChangeset("ec3a8d4f");

                        var operationRequestMessage1 = multipartMixedBatchWriter.CreateOperationRequestMessage(
                            "POST", new Uri($"{ServiceUri}/Customers"), "1");

                        // Try to use a content id not matched to a preceding request
                        var dependsOnIds = new List<string> { "3" };
                        var operationRequestMessage2 = multipartMixedBatchWriter.CreateOperationRequestMessage(
                            "POST", new Uri($"{ServiceUri}/Orders"), "2", BatchPayloadUriOption.AbsoluteUri, dependsOnIds);
                    }
                }));

            Assert.Equal(Strings.ODataBatchReader_DependsOnIdNotFound(3, 2), asyncException.Message);
            Assert.Equal(Strings.ODataBatchReader_DependsOnIdNotFound(3, 2), syncException.Message);
        }

        [Fact]
        public async Task OnInStreamError_APIsYieldSameResult()
        {
            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
                    {
                        var multipartMixedBatchWriter = await messageWriter.CreateODataBatchWriterAsync();
                        await multipartMixedBatchWriter.WriteStartBatchAsync();
                        await multipartMixedBatchWriter.OnInStreamErrorAsync();
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(() =>
                {
                    IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                    {
                        var multipartMixedBatchWriter = messageWriter.CreateODataBatchWriter();
                        multipartMixedBatchWriter.WriteStartBatch();
                        multipartMixedBatchWriter.OnInStreamError();
                    }
                }));

            Assert.Equal(Strings.ODataBatchWriter_CannotWriteInStreamErrorForBatch, asyncException.Message);
            Assert.Equal(Strings.ODataBatchWriter_CannotWriteInStreamErrorForBatch, syncException.Message);
        }

        #endregion

        private void InitializeEdmModel()
        {
            this.model = new EdmModel();

            this.orderEntityType = new EdmEntityType("NS", "Order", /*baseType*/ null, /*isAbstract*/ false, /*isOpen*/ true);
            this.customerEntityType = new EdmEntityType("NS", "Customer");

            var orderIdProperty = this.orderEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            this.orderEntityType.AddKeys(orderIdProperty);
            this.orderEntityType.AddStructuralProperty("Amount", EdmPrimitiveTypeKind.Decimal);
            var customerNavProperty = this.orderEntityType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo
                {
                    Name = "Customer",
                    Target = this.customerEntityType,
                    TargetMultiplicity = EdmMultiplicity.ZeroOrOne
                });
            this.model.AddElement(this.orderEntityType);

            var customerIdProperty = this.customerEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            this.customerEntityType.AddKeys(customerIdProperty);
            this.customerEntityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            var ordersNavProperty = this.customerEntityType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo
                {
                    Name = "Orders",
                    Target = this.orderEntityType,
                    TargetMultiplicity = EdmMultiplicity.Many
                });
            this.model.AddElement(this.customerEntityType);

            var entityContainer = new EdmEntityContainer("NS", "Container");
            this.model.AddElement(entityContainer);

            this.orderEntitySet = entityContainer.AddEntitySet("Orders", this.orderEntityType);
            this.customerEntitySet = entityContainer.AddEntitySet("Customers", this.customerEntityType);

            this.orderEntitySet.AddNavigationTarget(customerNavProperty, this.customerEntitySet);
            this.customerEntitySet.AddNavigationTarget(ordersNavProperty, this.orderEntitySet);
        }

        #region Helper Methods

        private static ODataResource CreateOrderResource()
        {
            return new ODataResource
            {
                TypeName = "NS.Order",
                Properties = new List<ODataProperty>
                {
                    new ODataProperty
                    {
                        Name = "Id",
                        Value = 1,
                        SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Key }
                    },
                    new ODataProperty { Name = "Amount", Value = 13M }
                },
                SerializationInfo = new ODataResourceSerializationInfo
                {
                    NavigationSourceName = "Orders",
                    ExpectedTypeName = "NS.Order",
                    NavigationSourceEntityTypeName = "NS.Order",
                    NavigationSourceKind = EdmNavigationSourceKind.EntitySet
                }
            };
        }

        private static ODataResource CreateCustomerResource()
        {
            return new ODataResource
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
                },
                SerializationInfo = new ODataResourceSerializationInfo
                {
                    NavigationSourceName = "Customers",
                    ExpectedTypeName = "NS.Customer",
                    NavigationSourceEntityTypeName = "NS.Customer",
                    NavigationSourceKind = EdmNavigationSourceKind.EntitySet
                }
            };
        }

        #endregion Helper Methods
    }
}

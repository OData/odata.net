//---------------------------------------------------------------------
// <copyright file="ODataMultipartMixedBatchWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.MultipartMixed;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class ODataMultipartMixedBatchWriterTests
    {
        private const string ServiceUri = "http://tempuri.org";
        private const string batchBoundary = "batch_aed653ab";
        private ODataMediaType mediaType;
        private MemoryStream stream;
        private ODataMessageWriterSettings settings;

        private EdmModel model;
        private EdmEntityType orderEntityType;
        private EdmEntityType customerEntityType;
        private EdmEntitySet orderEntitySet;
        private EdmEntitySet customerEntitySet;

        public ODataMultipartMixedBatchWriterTests()
        {
            this.InitializeEdmModel();
            this.stream = new MemoryStream();
            this.settings = new ODataMessageWriterSettings
            {
                ODataUri = new ODataUri { ServiceRoot = new Uri(ServiceUri) }
            };
            this.settings.SetServiceDocumentUri(new Uri(ServiceUri));
            this.mediaType = new ODataMediaType(
                "Multipart",
                "Mixed",
                new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("boundary", batchBoundary) });
        }

        [Fact]
        public async Task WriteMultipartMixedBatchAsync()
        {
            var result = await SetupMultipartMixedBatchWriterAndRunTestAsync(
                async (multipartMixedBatchWriter) =>
                {
                    await multipartMixedBatchWriter.WriteStartBatchAsync();
                    await multipartMixedBatchWriter.CreateOperationRequestMessageAsync(
                        "POST", new Uri($"{ServiceUri}/Orders"), "1");
                    await multipartMixedBatchWriter.WriteEndBatchAsync();
                });

            var expected = @"--batch_aed653ab
Content-Type: application/http
Content-Transfer-Encoding: binary

POST http://tempuri.org/Orders HTTP/1.1


--batch_aed653ab--
";

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task WriteMultipartMixedBatchRequestAsync()
        {
            var result = await SetupMultipartMixedBatchWriterAndRunTestAsync(
                async (multipartMixedBatchWriter) =>
                {
                    await multipartMixedBatchWriter.WriteStartBatchAsync();

                    var operationRequestMessage = await multipartMixedBatchWriter.CreateOperationRequestMessageAsync(
                        "POST", new Uri($"{ServiceUri}/Orders"), "1");

                    using (var messageWriter = new ODataMessageWriter(operationRequestMessage))
                    {
                        var jsonLightWriter = await messageWriter.CreateODataResourceWriterAsync(this.orderEntitySet, this.orderEntityType);

                        var orderResource = CreateOrderResource();
                        await jsonLightWriter.WriteStartAsync(orderResource);
                        await jsonLightWriter.WriteEndAsync();
                    }

                    await multipartMixedBatchWriter.WriteEndBatchAsync();
                },
                writingRequest: true);

            var expected = @"--batch_aed653ab
Content-Type: application/http
Content-Transfer-Encoding: binary

POST http://tempuri.org/Orders HTTP/1.1
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""Id"":1,""Amount"":13}
--batch_aed653ab--
";

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task WriteMultipartMixedBatchRequestWithSingleChangesetAsync()
        {
            var result = await SetupMultipartMixedBatchWriterAndRunTestAsync(
                async (multipartMixedBatchWriter) =>
                {
                    await multipartMixedBatchWriter.WriteStartBatchAsync();
                    await multipartMixedBatchWriter.WriteStartChangesetAsync("ec3a8d4f");

                    var operationRequestMessage1 = await multipartMixedBatchWriter.CreateOperationRequestMessageAsync(
                        "POST", new Uri($"{ServiceUri}/Orders"), "1");

                    using (var messageWriter1 = new ODataMessageWriter(operationRequestMessage1))
                    {
                        var jsonLightWriter = await messageWriter1.CreateODataResourceWriterAsync(this.orderEntitySet, this.orderEntityType);

                        var orderResource = CreateOrderResource();
                        await jsonLightWriter.WriteStartAsync(orderResource);
                        await jsonLightWriter.WriteEndAsync();
                    }

                    var operationRequestMessage2 = await multipartMixedBatchWriter.CreateOperationRequestMessageAsync(
                        "POST", new Uri($"{ServiceUri}/Customers"), "2");

                    using (var messageWriter2 = new ODataMessageWriter(operationRequestMessage2))
                    {
                        var jsonLightWriter = await messageWriter2.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);

                        var customerResource = CreateCustomerResource();
                        await jsonLightWriter.WriteStartAsync(customerResource);
                        await jsonLightWriter.WriteEndAsync();
                    }

                    await multipartMixedBatchWriter.WriteEndChangesetAsync();
                    await multipartMixedBatchWriter.WriteEndBatchAsync();
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

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task WriteMultipartMixedBatchRequestWithMultipleChangesetAsync()
        {
            var result = await SetupMultipartMixedBatchWriterAndRunTestAsync(
                async (multipartMixedBatchWriter) =>
                {
                    await multipartMixedBatchWriter.WriteStartBatchAsync();
                    await multipartMixedBatchWriter.WriteStartChangesetAsync("ec3a8d4f");

                    var operationRequestMessage1 = await multipartMixedBatchWriter.CreateOperationRequestMessageAsync(
                        "PUT", new Uri($"{ServiceUri}/Orders(1)"), "1");

                    using (var messageWriter1 = new ODataMessageWriter(operationRequestMessage1))
                    {
                        var jsonLightWriter = await messageWriter1.CreateODataResourceWriterAsync(this.orderEntitySet, this.orderEntityType);

                        var orderResource = CreateOrderResource();
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

                        var customerResource = CreateCustomerResource();
                        await jsonLightWriter.WriteStartAsync(customerResource);
                        await jsonLightWriter.WriteEndAsync();
                    }

                    await multipartMixedBatchWriter.WriteEndChangesetAsync();
                    await multipartMixedBatchWriter.WriteEndBatchAsync();
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

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task WriteMultipartMixedBatchRequestWithDependsOnIdsAsync()
        {
            var result = await SetupMultipartMixedBatchWriterAndRunTestAsync(
                async (multipartMixedBatchWriter) =>
                {
                    await multipartMixedBatchWriter.WriteStartBatchAsync();
                    await multipartMixedBatchWriter.WriteStartChangesetAsync("ec3a8d4f");

                    var operationRequestMessage1 = await multipartMixedBatchWriter.CreateOperationRequestMessageAsync(
                        "POST", new Uri($"{ServiceUri}/Customers"), "1");

                    using (var messageWriter1 = new ODataMessageWriter(operationRequestMessage1))
                    {
                        var jsonLightWriter = await messageWriter1.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);

                        var customerResource = CreateCustomerResource();
                        await jsonLightWriter.WriteStartAsync(customerResource);
                        await jsonLightWriter.WriteEndAsync();
                    }

                    // Operation request depends on the previous (Content ID: 1)
                    var dependsOnIds = new List<string> { "1" };
                    var operationRequestMessage2 = await multipartMixedBatchWriter.CreateOperationRequestMessageAsync(
                        "POST", new Uri($"{ServiceUri}/Orders"), "2", BatchPayloadUriOption.AbsoluteUri, dependsOnIds);

                    using (var messageWriter1 = new ODataMessageWriter(operationRequestMessage2))
                    {
                        var jsonLightWriter = await messageWriter1.CreateODataResourceWriterAsync(this.orderEntitySet, this.orderEntityType);

                        var orderResource = CreateOrderResource();
                        await jsonLightWriter.WriteStartAsync(orderResource);
                        await jsonLightWriter.WriteEndAsync();
                    }

                    await multipartMixedBatchWriter.WriteEndChangesetAsync();
                    await multipartMixedBatchWriter.WriteEndBatchAsync();
                });

            var expected = @"--batch_aed653ab
Content-Type: multipart/mixed; boundary=changeset_ec3a8d4f

--changeset_ec3a8d4f
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

POST http://tempuri.org/Customers HTTP/1.1
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""Id"":1,""Name"":""Customer 1""}
--changeset_ec3a8d4f
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2

POST http://tempuri.org/Orders HTTP/1.1
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""Id"":1,""Amount"":13}
--changeset_ec3a8d4f--
--batch_aed653ab--
";

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task WriteMultipartMixedBatchRequestWithGroupIdForChangesetNotSpecifiedAsync()
        {
            var result = await SetupMultipartMixedBatchWriterAndRunTestAsync(
                async (multipartMixedBatchWriter) =>
                {
                    await multipartMixedBatchWriter.WriteStartBatchAsync();
                    await multipartMixedBatchWriter.WriteStartChangesetAsync();

                    var operationRequestMessage = await multipartMixedBatchWriter.CreateOperationRequestMessageAsync(
                        "POST", new Uri($"{ServiceUri}/Orders"), "1");

                    using (var messageWriter = new ODataMessageWriter(operationRequestMessage))
                    {
                        var jsonLightWriter = await messageWriter.CreateODataResourceWriterAsync(this.orderEntitySet, this.orderEntityType);

                        var orderResource = CreateOrderResource();
                        await jsonLightWriter.WriteStartAsync(orderResource);
                        await jsonLightWriter.WriteEndAsync();
                    }

                    await multipartMixedBatchWriter.WriteEndChangesetAsync();
                    await multipartMixedBatchWriter.WriteEndBatchAsync();
                });

            var expectedPart1 = @"--batch_aed653ab
Content-Type: multipart/mixed; boundary=changeset_"; // atomicityGroup is a random Guid
            var expectedPart2 = @"
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

POST http://tempuri.org/Orders HTTP/1.1
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""Id"":1,""Amount"":13}
--changeset_"; // atomicityGroup is a random Guid
            var expectedPart3 = @"--
--batch_aed653ab--
";

            Assert.StartsWith(expectedPart1, result);
            Assert.Contains(expectedPart2, result);
            Assert.EndsWith(expectedPart3, result);
        }

        [Fact]
        public async Task WriteMultipartMixedBatchRequestWithContentIdNullAsync()
        {
            var result = await SetupMultipartMixedBatchWriterAndRunTestAsync(
                async (multipartMixedBatchWriter) =>
                {
                    await multipartMixedBatchWriter.WriteStartBatchAsync();

                    var operationRequestMessage = await multipartMixedBatchWriter.CreateOperationRequestMessageAsync(
                        "POST", new Uri($"{ServiceUri}/Orders"), /*contentId*/ null);

                    using (var messageWriter = new ODataMessageWriter(operationRequestMessage))
                    {
                        var jsonLightWriter = await messageWriter.CreateODataResourceWriterAsync(this.orderEntitySet, this.orderEntityType);

                        var orderResource = CreateOrderResource();
                        await jsonLightWriter.WriteStartAsync(orderResource);
                        await jsonLightWriter.WriteEndAsync();
                    }

                    await multipartMixedBatchWriter.WriteEndBatchAsync();
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

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task WriteMultipartMixedBatchRequestWithAbsoluteUriUsingHostHeaderAsync()
        {
            var result = await SetupMultipartMixedBatchWriterAndRunTestAsync(
                async (multipartMixedBatchWriter) =>
                {
                    await multipartMixedBatchWriter.WriteStartBatchAsync();

                    var operationRequestMessage = await multipartMixedBatchWriter.CreateOperationRequestMessageAsync(
                        "POST", new Uri($"{ServiceUri}/Orders"), /*contentId*/ null, BatchPayloadUriOption.AbsoluteUriUsingHostHeader);

                    using (var messageWriter = new ODataMessageWriter(operationRequestMessage))
                    {
                        var jsonLightWriter = await messageWriter.CreateODataResourceWriterAsync(this.orderEntitySet, this.orderEntityType);

                        var orderResource = CreateOrderResource();
                        await jsonLightWriter.WriteStartAsync(orderResource);
                        await jsonLightWriter.WriteEndAsync();
                    }

                    await multipartMixedBatchWriter.WriteEndBatchAsync();
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

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task WriteMultipartMixedBatchRequestWithRelativeUriAsync()
        {
            this.settings.BaseUri = new Uri(ServiceUri);

            var result = await SetupMultipartMixedBatchWriterAndRunTestAsync(
                async (multipartMixedBatchWriter) =>
                {
                    await multipartMixedBatchWriter.WriteStartBatchAsync();

                    var operationRequestMessage = await multipartMixedBatchWriter.CreateOperationRequestMessageAsync(
                        "POST", new Uri("/Orders", UriKind.Relative), /*contentId*/ null, BatchPayloadUriOption.RelativeUri);

                    using (var messageWriter = new ODataMessageWriter(operationRequestMessage))
                    {
                        var jsonLightWriter = await messageWriter.CreateODataResourceWriterAsync(this.orderEntitySet, this.orderEntityType);

                        var orderResource = CreateOrderResource();
                        await jsonLightWriter.WriteStartAsync(orderResource);
                        await jsonLightWriter.WriteEndAsync();
                    }

                    await multipartMixedBatchWriter.WriteEndBatchAsync();
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

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task WriteMultipartMixedBatchRequestAsync_ReportMessageCompleted()
        {
            var result = await SetupMultipartMixedBatchWriterAndRunTestAsync(
                async (multipartMixedBatchWriter) =>
                {
                    await multipartMixedBatchWriter.WriteStartBatchAsync();

                    var operationRequestMessage = await multipartMixedBatchWriter.CreateOperationRequestMessageAsync(
                        "POST", new Uri($"{ServiceUri}/Orders"), /*contentId*/ null);
                    // No writer created for the request message
                    await multipartMixedBatchWriter.WriteEndBatchAsync();
                });

            var expected = @"--batch_aed653ab
Content-Type: application/http
Content-Transfer-Encoding: binary

POST http://tempuri.org/Orders HTTP/1.1


--batch_aed653ab--
";

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task WriteMultipartMixedBatchResponseAsync()
        {
            var result = await SetupMultipartMixedBatchWriterAndRunTestAsync(
                async (multipartMixedBatchWriter) =>
                {
                    await multipartMixedBatchWriter.WriteStartBatchAsync();

                    var operationResponseMessage = await multipartMixedBatchWriter.CreateOperationResponseMessageAsync("1");
                    operationResponseMessage.StatusCode = 200;

                    using (var messageWriter = new ODataMessageWriter(operationResponseMessage, this.settings))
                    {
                        var jsonLightWriter = await messageWriter.CreateODataResourceWriterAsync(this.orderEntitySet, this.orderEntityType);

                        var orderResource = CreateOrderResource();
                        await jsonLightWriter.WriteStartAsync(orderResource);
                        await jsonLightWriter.WriteEndAsync();
                    }

                    await multipartMixedBatchWriter.WriteEndBatchAsync();
                },
                writingRequest: false);

            var expected = @"--batch_aed653ab
Content-Type: application/http
Content-Transfer-Encoding: binary

HTTP/1.1 200 OK
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""@odata.context"":""http://tempuri.org/$metadata#Orders/$entity"",""Id"":1,""Amount"":13}
--batch_aed653ab--
";

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task WriteMultipartMixedBatchResponseWithChangesetAsync()
        {
            var result = await SetupMultipartMixedBatchWriterAndRunTestAsync(
                async (multipartMixedBatchWriter) =>
                {
                    await multipartMixedBatchWriter.WriteStartBatchAsync();
                    await multipartMixedBatchWriter.WriteStartChangesetAsync("ec3a8d4f");

                    var operationResponseMessage1 = await multipartMixedBatchWriter.CreateOperationResponseMessageAsync("1");
                    operationResponseMessage1.StatusCode = 200;

                    using (var messageWriter1 = new ODataMessageWriter(operationResponseMessage1, this.settings))
                    {
                        var jsonLightWriter = await messageWriter1.CreateODataResourceWriterAsync(this.orderEntitySet, this.orderEntityType);

                        var orderResource = CreateOrderResource();
                        await jsonLightWriter.WriteStartAsync(orderResource);
                        await jsonLightWriter.WriteEndAsync();
                    }

                    await multipartMixedBatchWriter.WriteEndChangesetAsync();
                    await multipartMixedBatchWriter.WriteStartChangesetAsync("f46c46e2");

                    var operationResponseMessage2 = await multipartMixedBatchWriter.CreateOperationResponseMessageAsync("2");
                    operationResponseMessage2.StatusCode = 200;

                    using (var messageWriter2 = new ODataMessageWriter(operationResponseMessage2, this.settings))
                    {
                        var jsonLightWriter = await messageWriter2.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);

                        var customerResource = CreateCustomerResource();
                        await jsonLightWriter.WriteStartAsync(customerResource);
                        await jsonLightWriter.WriteEndAsync();
                    }

                    await multipartMixedBatchWriter.WriteEndChangesetAsync();
                    await multipartMixedBatchWriter.WriteEndBatchAsync();
                },
                writingRequest: false);

            var expected = @"--batch_aed653ab
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
--batch_aed653ab
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
--batch_aed653ab--
";

            Assert.Equal(expected, result);
        }

        #region Exception Cases

        [Fact]
        public async Task WriteMultipartMixedBatchRequestWithChangesetAsync_ThrowsExceptionForContentIdIsNull()
        {
            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupMultipartMixedBatchWriterAndRunTestAsync(
                    async (multipartMixedBatchWriter) =>
                    {
                        await multipartMixedBatchWriter.WriteStartBatchAsync();
                        await multipartMixedBatchWriter.WriteStartChangesetAsync();

                        var operationRequestMessage = await multipartMixedBatchWriter.CreateOperationRequestMessageAsync(
                            "POST", new Uri($"{ServiceUri}/Orders"), /*contentId*/ null);
                        // Try to create operation request message with null content id
                    }));

            Assert.Equal(Strings.ODataBatchOperationHeaderDictionary_KeyNotFound(ODataConstants.ContentIdHeader), exception.Message);
        }

        [Fact]
        public async Task WriteMultipartMixedBatchRequestAsync_ThrowsExceptionForDependsOnIdNotFound()
        {
            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupMultipartMixedBatchWriterAndRunTestAsync(
                    async (multipartMixedBatchWriter) =>
                    {
                        await multipartMixedBatchWriter.WriteStartBatchAsync();
                        await multipartMixedBatchWriter.WriteStartChangesetAsync("ec3a8d4f");

                        var operationRequestMessage1 = await multipartMixedBatchWriter.CreateOperationRequestMessageAsync(
                            "POST", new Uri($"{ServiceUri}/Customers"), "1");

                        // Try to use a content id not matched to a preceding request
                        var dependsOnIds = new List<string> { "3" };
                        var operationRequestMessage2 = await multipartMixedBatchWriter.CreateOperationRequestMessageAsync(
                            "POST", new Uri($"{ServiceUri}/Orders"), "2", BatchPayloadUriOption.AbsoluteUri, dependsOnIds);
                    }));

            Assert.Equal(Strings.ODataBatchReader_DependsOnIdNotFound("3", "2"), exception.Message);
        }

        [Fact]
        public async Task OnInStreamErrorAsync_ThrowsException()
        {
            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupMultipartMixedBatchWriterAndRunTestAsync(
                    (multipartMixedBatchWriter) => multipartMixedBatchWriter.OnInStreamErrorAsync()));

            Assert.Equal(Strings.ODataBatchWriter_CannotWriteInStreamErrorForBatch, exception.Message);
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

        /// <summary>
        /// Sets up an ODataMultipartMixedBatchWriter,
        /// then runs the given test code asynchronously,
        /// then flushes and reads the stream back as a string for customized verification.
        /// </summary>
        private async Task<string> SetupMultipartMixedBatchWriterAndRunTestAsync(
            Func<ODataMultipartMixedBatchWriter, Task> func,
            bool writingRequest = true)
        {
            var multipartMixedBatchOutputContext = CreateMultipartMixedBatchOutputContext(writingRequest, /*asynchronous*/ true);
            var multipartMixedBatchWriter = new ODataMultipartMixedBatchWriter(multipartMixedBatchOutputContext, batchBoundary);

            await func(multipartMixedBatchWriter);

            this.stream.Position = 0;
            return await new StreamReader(this.stream).ReadToEndAsync();
        }

        private ODataMultipartMixedBatchOutputContext CreateMultipartMixedBatchOutputContext(bool writingRequest = true, bool asynchronous = false)
        {
            var messageInfo = new ODataMessageInfo
            {
                MessageStream = this.stream,
                MediaType = this.mediaType,
                // Async writer needs the default encoding to not use the preamble.
                Encoding = MediaTypeUtils.EncodingUtf8NoPreamble,
                IsResponse = !writingRequest,
                IsAsync = asynchronous
            };

            return new ODataMultipartMixedBatchOutputContext(ODataFormat.Batch, messageInfo, this.settings);
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

//---------------------------------------------------------------------
// <copyright file="ODataJsonBatchWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Core.Tests.DependencyInjection;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Microsoft.OData.Tests;
using Xunit;

namespace Microsoft.OData.Core.Tests.Json
{
    public class ODataJsonBatchWriterTests
    {
        private const string ServiceUri = "http://tempuri.org";
        private EdmModel model;
        private Stream stream;
        private ODataMessageWriterSettings settings;
        private ODataMediaType mediaType;
        private Encoding encoding;

        private EdmEnumType customerTypeEnumType;
        private EdmEntityType customerEntityType;
        private EdmEntityType orderEntityType;
        private EdmEntitySet customerEntitySet;
        private EdmEntitySet orderEntitySet;

        public ODataJsonBatchWriterTests()
        {
            InitializeEdmModel();
            this.stream = new MemoryStream();
            this.settings = new ODataMessageWriterSettings { Version = ODataVersion.V4 };
            this.settings.SetServiceDocumentUri(new Uri(ServiceUri));
            this.mediaType = new ODataMediaType("application", "json",
                new[]
                {
                    new KeyValuePair<string, string>("odata.metadata", "minimal"),
                    new KeyValuePair<string, string>("odata.streaming", "true"),
                    new KeyValuePair<string, string>("IEEE754Compatible", "false"),
                    new KeyValuePair<string, string>("charset", "utf-8")
                });
            this.encoding = Encoding.UTF8;
        }

        private void InitializeEdmModel()
        {
            this.model = new EdmModel();

            this.customerTypeEnumType = new EdmEnumType("NS", "CustomerType");
            this.customerEntityType = new EdmEntityType("NS", "Customer");
            this.orderEntityType = new EdmEntityType("NS", "Order");

            this.customerTypeEnumType.AddMember(new EdmEnumMember(this.customerTypeEnumType, "Retail", new EdmEnumMemberValue(0)));
            this.customerTypeEnumType.AddMember(new EdmEnumMember(this.customerTypeEnumType, "Wholesale", new EdmEnumMemberValue(1)));
            this.model.AddElement(this.customerTypeEnumType);

            var customerIdProperty = this.customerEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            this.customerEntityType.AddKeys(customerIdProperty);
            this.customerEntityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            this.customerEntityType.AddStructuralProperty("Type", new EdmEnumTypeReference(this.customerTypeEnumType, false));
            this.model.AddElement(this.customerEntityType);

            var orderIdProperty = this.orderEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            this.orderEntityType.AddKeys(orderIdProperty);
            this.orderEntityType.AddStructuralProperty("CustomerId", EdmPrimitiveTypeKind.Int32);
            this.orderEntityType.AddStructuralProperty("Amount", EdmPrimitiveTypeKind.Decimal);
            this.model.AddElement(this.orderEntityType);

            var entityContainer = new EdmEntityContainer("NS", "Container");
            this.model.AddElement(entityContainer);

            this.customerEntitySet = entityContainer.AddEntitySet("Customers", this.customerEntityType);
            this.orderEntitySet = entityContainer.AddEntitySet("Orders", this.orderEntityType);
        }

        [Theory]
        [InlineData(true, "{\"requests\":[]}")]
        [InlineData(false, "{\"responses\":[]}")]
        public async Task WriteBatchAsync(bool writingRequest, string expected)
        {
            var result = await SetupJsonBatchWriterAndRunTestAsync(
                async (jsonBatchWriter) =>
                {
                    await jsonBatchWriter.WriteStartBatchAsync();
                    await jsonBatchWriter.WriteEndBatchAsync();
                },
                writingRequest: writingRequest);

            Assert.Equal(result, expected);
        }

        [Fact]
        public async Task WriteBatchRequestAsync()
        {
            var result = await SetupJsonBatchWriterAndRunTestAsync(
                async (jsonBatchWriter) =>
                {
                    await jsonBatchWriter.WriteStartBatchAsync();

                    var operationRequestMessage = await jsonBatchWriter.CreateOperationRequestMessageAsync(
                        "POST", new Uri($"{ServiceUri}/Customers"), "1");

                    await using (var messageWriter = new ODataMessageWriter(operationRequestMessage))
                    {
                        var jsonWriter = await messageWriter.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);

                        var customerResource = CreateCustomerResource(1);
                        await jsonWriter.WriteStartAsync(customerResource);
                        await jsonWriter.WriteEndAsync();
                    }

                    await jsonBatchWriter.WriteEndBatchAsync();
                });

            Assert.Equal("{\"requests\":[{" +
                "\"id\":\"1\"," +
                "\"method\":\"POST\"," +
                "\"url\":\"http://tempuri.org/Customers\"," +
                "\"headers\":{\"odata-version\":\"4.0\",\"content-type\":\"application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\"}, " +
                "\"body\" :{\"Id\":1,\"Name\":\"Customer 1\",\"Type\":\"Retail\"}}]}",
                result);
        }

        [Fact]
        public async Task WriteBatchRequestWithChangesetAsync()
        {
            var result = await SetupJsonBatchWriterAndRunTestAsync(
                async (jsonBatchWriter) =>
                {
                    await jsonBatchWriter.WriteStartBatchAsync();
                    await jsonBatchWriter.WriteStartChangesetAsync("69028f2c-f57b-4850-89f0-b7e5e002d4bc");

                    var operationRequestMessage = await jsonBatchWriter.CreateOperationRequestMessageAsync(
                        "POST", new Uri($"{ServiceUri}/Customers"), "1");

                    await using (var messageWriter = new ODataMessageWriter(operationRequestMessage))
                    {
                        var jsonWriter = await messageWriter.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);

                        var customerResource = CreateCustomerResource(1);
                        await jsonWriter.WriteStartAsync(customerResource);
                        await jsonWriter.WriteEndAsync();
                    }

                    await jsonBatchWriter.WriteEndChangesetAsync();
                    await jsonBatchWriter.WriteEndBatchAsync();
                });

            Assert.Equal("{\"requests\":[{" +
                "\"id\":\"1\"," +
                "\"atomicityGroup\":\"69028f2c-f57b-4850-89f0-b7e5e002d4bc\"," +
                "\"method\":\"POST\"," +
                "\"url\":\"http://tempuri.org/Customers\"," +
                "\"headers\":{\"odata-version\":\"4.0\",\"content-type\":\"application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\"}, " +
                "\"body\" :{\"Id\":1,\"Name\":\"Customer 1\",\"Type\":\"Retail\"}}]}",
                result);
        }

        [Fact]
        public async Task WriteBatchRequestWithDependsOnIdsAsync()
        {
            var result = await SetupJsonBatchWriterAndRunTestAsync(
                async (jsonBatchWriter) =>
                {
                    await jsonBatchWriter.WriteStartBatchAsync();

                    var operationRequestMessage1 = await jsonBatchWriter.CreateOperationRequestMessageAsync(
                        "POST", new Uri($"{ServiceUri}/Customers"), "1");


                    await using (var messageWriter1 = new ODataMessageWriter(operationRequestMessage1))
                    {
                        var jsonWriter = await messageWriter1.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);

                        var customerResource = CreateCustomerResource(1);
                        await jsonWriter.WriteStartAsync(customerResource);
                        await jsonWriter.WriteEndAsync();
                    }

                    // Operation request depends on the previous (Content ID: 1)
                    var dependsOnIds = new List<string> { "1" };
                    var operationRequestMessage2 = await jsonBatchWriter.CreateOperationRequestMessageAsync(
                        "POST", new Uri($"{ServiceUri}/Orders"), "2", BatchPayloadUriOption.AbsoluteUri, dependsOnIds);

                    await using (var messageWriter2 = new ODataMessageWriter(operationRequestMessage2))
                    {
                        var jsonWriter = await messageWriter2.CreateODataResourceWriterAsync(this.orderEntitySet, this.orderEntityType);

                        var orderResource = CreateOrderResource(1);
                        await jsonWriter.WriteStartAsync(orderResource);
                        await jsonWriter.WriteEndAsync();
                    }

                    await jsonBatchWriter.WriteEndBatchAsync();
                });

            Assert.Equal("{\"requests\":[" +
                "{\"id\":\"1\"," +
                "\"method\":\"POST\"," +
                "\"url\":\"http://tempuri.org/Customers\"," +
                "\"headers\":{\"odata-version\":\"4.0\",\"content-type\":\"application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\"}, " +
                "\"body\" :{\"Id\":1,\"Name\":\"Customer 1\",\"Type\":\"Retail\"}}," +
                "{\"id\":\"2\"," +
                "\"dependsOn\":[\"1\"]," +
                "\"method\":\"POST\"," +
                "\"url\":\"http://tempuri.org/Orders\"," +
                "\"headers\":{\"odata-version\":\"4.0\",\"content-type\":\"application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\"}, " +
                "\"body\" :{\"Id\":1,\"CustomerId\":1,\"Amount\":13}}]}",
                result);
        }

        [Fact]
        public async Task WriteBatchRequestWithChangesetAndDependsOnIdsAsync()
        {
            var result = await SetupJsonBatchWriterAndRunTestAsync(
                async (jsonBatchWriter) =>
                {
                    await jsonBatchWriter.WriteStartBatchAsync();
                    await jsonBatchWriter.WriteStartChangesetAsync("69028f2c-f57b-4850-89f0-b7e5e002d4bc");

                    var operationRequestMessage1 = await jsonBatchWriter.CreateOperationRequestMessageAsync(
                        "POST", new Uri($"{ServiceUri}/Customers"), "1");

                    await using (var messageWriter1 = new ODataMessageWriter(operationRequestMessage1))
                    {
                        var jsonWriter = await messageWriter1.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);

                        var customerResource = CreateCustomerResource(1);
                        await jsonWriter.WriteStartAsync(customerResource);
                        await jsonWriter.WriteEndAsync();
                    }

                    // Operation request depends on the previous (Content ID: 1)
                    var dependsOnIds = new List<string> { "1" };
                    var operationRequestMessage2 = await jsonBatchWriter.CreateOperationRequestMessageAsync(
                        "POST", new Uri($"{ServiceUri}/Orders"), "2", BatchPayloadUriOption.AbsoluteUri, dependsOnIds);

                    await using (var messageWriter2 = new ODataMessageWriter(operationRequestMessage2))
                    {
                        var jsonWriter = await messageWriter2.CreateODataResourceWriterAsync(this.orderEntitySet, this.orderEntityType);

                        var orderResource = CreateOrderResource(1);
                        await jsonWriter.WriteStartAsync(orderResource);
                        await jsonWriter.WriteEndAsync();
                    }

                    await jsonBatchWriter.WriteEndChangesetAsync();
                    await jsonBatchWriter.WriteEndBatchAsync();
                });

            Assert.Equal(
                "{\"requests\":[" +
                "{\"id\":\"1\"," +
                "\"atomicityGroup\":\"69028f2c-f57b-4850-89f0-b7e5e002d4bc\"," +
                "\"method\":\"POST\"," +
                "\"url\":\"http://tempuri.org/Customers\"," +
                "\"headers\":{\"odata-version\":\"4.0\",\"content-type\":\"application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\"}, " +
                "\"body\" :{\"Id\":1,\"Name\":\"Customer 1\",\"Type\":\"Retail\"}}," +
                "{\"id\":\"2\"," +
                "\"atomicityGroup\":\"69028f2c-f57b-4850-89f0-b7e5e002d4bc\"," +
                "\"dependsOn\":[\"1\"]," +
                "\"method\":\"POST\"," +
                "\"url\":\"http://tempuri.org/Orders\"," +
                "\"headers\":{\"odata-version\":\"4.0\",\"content-type\":\"application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\"}, " +
                "\"body\" :{\"Id\":1,\"CustomerId\":1,\"Amount\":13}}]}",
                result);
        }

        [Fact]
        public async Task WriteBatchRequestWithGroupIdForChangesetNotSpecifiedAsync()
        {
            var result = await SetupJsonBatchWriterAndRunTestAsync(
                async (jsonBatchWriter) =>
                {
                    await jsonBatchWriter.WriteStartBatchAsync();
                    await jsonBatchWriter.WriteStartChangesetAsync();

                    var operationRequestMessage = await jsonBatchWriter.CreateOperationRequestMessageAsync(
                        "POST", new Uri($"{ServiceUri}/Customers"), "1");

                    await using (var messageWriter = new ODataMessageWriter(operationRequestMessage))
                    {
                        var jsonWriter = await messageWriter.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);

                        var customerResource = CreateCustomerResource(1);
                        await jsonWriter.WriteStartAsync(customerResource);
                        await jsonWriter.WriteEndAsync();
                    }

                    await jsonBatchWriter.WriteEndChangesetAsync();
                    await jsonBatchWriter.WriteEndBatchAsync();
                });

            Assert.StartsWith("{\"requests\":[{" +
                "\"id\":\"1\"," +
                "\"atomicityGroup\":\"", // atomicityGroup is a random Guid
                result);
            Assert.EndsWith("\"," +
                "\"method\":\"POST\"," +
                "\"url\":\"http://tempuri.org/Customers\"," +
                "\"headers\":{\"odata-version\":\"4.0\",\"content-type\":\"application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\"}, " +
                "\"body\" :{\"Id\":1,\"Name\":\"Customer 1\",\"Type\":\"Retail\"}}]}",
                result);
        }

        [Fact]
        public async Task WriteBatchRequestWithContentIdNullAsync()
        {
            var result = await SetupJsonBatchWriterAndRunTestAsync(
                async (jsonBatchWriter) =>
                {
                    await jsonBatchWriter.WriteStartBatchAsync();

                    var operationRequestMessage = await jsonBatchWriter.CreateOperationRequestMessageAsync(
                        "POST", new Uri($"{ServiceUri}/Customers"), /*contentId*/ null);

                    await using (var messageWriter = new ODataMessageWriter(operationRequestMessage))
                    {
                        var jsonWriter = await messageWriter.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);

                        var customerResource = CreateCustomerResource(1);
                        await jsonWriter.WriteStartAsync(customerResource);
                        await jsonWriter.WriteEndAsync();
                    }

                    await jsonBatchWriter.WriteEndBatchAsync();
                });

            Assert.StartsWith("{\"requests\":[{\"id\":\"", // id is a random Guid
                result);
            Assert.EndsWith("\"," +
                "\"method\":\"POST\"," +
                "\"url\":\"http://tempuri.org/Customers\"," +
                "\"headers\":{\"odata-version\":\"4.0\",\"content-type\":\"application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\"}, " +
                "\"body\" :{\"Id\":1,\"Name\":\"Customer 1\",\"Type\":\"Retail\"}}]}",
                result);
        }

        [Fact]
        public async Task WriteBatchResponseAsync()
        {
            var result = await SetupJsonBatchWriterAndRunTestAsync(
                async (jsonBatchWriter) =>
                {
                    await jsonBatchWriter.WriteStartBatchAsync();

                    var operationResponseMessage = await jsonBatchWriter.CreateOperationResponseMessageAsync("1");

                    await using (var messageWriter = new ODataMessageWriter(operationResponseMessage, this.settings, this.model))
                    {
                        var jsonWriter = await messageWriter.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);

                        var customerResource = CreateCustomerResource(1);
                        await jsonWriter.WriteStartAsync(customerResource);
                        await jsonWriter.WriteEndAsync();
                    }

                    await jsonBatchWriter.WriteEndBatchAsync();
                },
                /*writingRequest*/ false);

            Assert.Equal("{\"responses\":[{" +
                "\"id\":\"1\"," +
                "\"status\":0," +
                "\"headers\":{\"odata-version\":\"4.0\",\"content-type\":\"application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\"}, " +
                "\"body\" :{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\",\"Id\":1,\"Name\":\"Customer 1\",\"Type\":\"Retail\"}}]}",
                result);
        }

        [Fact]
        public async Task WriteBatchResponseWithChangesetAsync()
        {
            var result = await SetupJsonBatchWriterAndRunTestAsync(
                async (jsonBatchWriter) =>
                {
                    await jsonBatchWriter.WriteStartBatchAsync();
                    await jsonBatchWriter.WriteStartChangesetAsync("69028f2c-f57b-4850-89f0-b7e5e002d4bc");

                    var operationResponseMessage = await jsonBatchWriter.CreateOperationResponseMessageAsync("1");

                    await using (var messageWriter = new ODataMessageWriter(operationResponseMessage, this.settings, this.model))
                    {
                        var jsonWriter = await messageWriter.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);

                        var customerResource = CreateCustomerResource(1);
                        await jsonWriter.WriteStartAsync(customerResource);
                        await jsonWriter.WriteEndAsync();
                    }

                    await jsonBatchWriter.WriteEndChangesetAsync();
                    await jsonBatchWriter.WriteEndBatchAsync();
                },
                /*writingRequest*/ false);

            Assert.Equal("{\"responses\":[{" +
                "\"id\":\"1\"," +
                "\"atomicityGroup\":\"69028f2c-f57b-4850-89f0-b7e5e002d4bc\"," +
                "\"status\":0," +
                "\"headers\":{\"odata-version\":\"4.0\",\"content-type\":\"application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\"}, " +
                "\"body\" :{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\",\"Id\":1,\"Name\":\"Customer 1\",\"Type\":\"Retail\"}}]}",
                result);
        }

        [Fact]
        public async Task WriteBatchResponseAsync_WithStreamCopy()
        {
            var result = await SetupJsonBatchWriterAndRunTestAsync(
                async (jsonBatchWriter) =>
                {
                    await jsonBatchWriter.WriteStartBatchAsync();

                    var customerOperationMessage = await jsonBatchWriter.CreateOperationResponseMessageAsync("1");
                    customerOperationMessage.SetHeader(ODataConstants.ODataVersionHeader, "4.0");
                    customerOperationMessage.SetHeader("Content-Type", "application/json");

                    // Copies operation's response body to the main batch response's stream
                    // This is similar to how the DefaultODataBatchHandler in WebApi writes batch responses.  
                    using (var customerResponseStream = CreateCustomerResponseStream(customerId: "1"))
                    using (var stream = await customerOperationMessage.GetStreamAsync())
                    {
                        customerResponseStream.Seek(0L, SeekOrigin.Begin);
                        await customerResponseStream.CopyToAsync(stream);
                    }

                    var orderOperationMessage = await jsonBatchWriter.CreateOperationResponseMessageAsync("2");
                    orderOperationMessage.SetHeader(ODataConstants.ODataVersionHeader, "4.0");
                    orderOperationMessage.SetHeader("Content-Type", "application/json");

                    using (var orderOperationStream = CreateOrderResponseStream(orderId: "1"))
                    using (var stream = await orderOperationMessage.GetStreamAsync())
                    {
                        orderOperationStream.Seek(0L, SeekOrigin.Begin);
                        await orderOperationStream.CopyToAsync(stream);
                    }

                    await jsonBatchWriter.WriteEndBatchAsync();
                },
                writingRequest: false);

            Assert.Equal("{\"responses\":[" +
                "{\"id\":\"1\"," +
                "\"status\":0," +
                "\"headers\":{\"odata-version\":\"4.0\",\"content-type\":\"application/json\"}, " +
                "\"body\" :{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\",\"Id\":1,\"Name\":\"Customer 1\",\"Type\":\"Retail\"}}," +
                "{\"id\":\"2\"," +
                "\"status\":0," +
                "\"headers\":{\"odata-version\":\"4.0\",\"content-type\":\"application/json\"}, " +
                "\"body\" :{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$entity\",\"Id\":1,\"CustomerId\":1,\"Amount\":13}}" +
                "]}",
                result);
        }

        [Fact]
        public async Task WriteBatchResponseAsync_WithStreamCopy_UsingODataUtf8JsonWriter()
        {
            Action<IServiceCollection> configureServices = (builder) =>
            {
                builder.AddSingleton<IJsonWriterFactory>(_ => ODataUtf8JsonWriterFactory.Default);
            };

            var result = await SetupJsonBatchWriterAndRunTestAsync(
                async (jsonBatchWriter) =>
                {
                    await jsonBatchWriter.WriteStartBatchAsync();

                    var customerOperationMessage = await jsonBatchWriter.CreateOperationResponseMessageAsync("1");
                    customerOperationMessage.SetHeader(ODataConstants.ODataVersionHeader, "4.0");
                    customerOperationMessage.SetHeader("Content-Type", "application/json");

                    // Copies operation's response body to the main batch response's stream
                    // This is similar to how the DefaultODataBatchHandler in WebApi writes batch responses.  
                    using (var customerResponseStream = CreateCustomerResponseStream(customerId: "1"))
                    using (var stream = await customerOperationMessage.GetStreamAsync())
                    {
                        customerResponseStream.Seek(0L, SeekOrigin.Begin);
                        await customerResponseStream.CopyToAsync(stream);
                    }

                    var orderOperationMessage = await jsonBatchWriter.CreateOperationResponseMessageAsync("2");
                    orderOperationMessage.SetHeader(ODataConstants.ODataVersionHeader, "4.0");
                    orderOperationMessage.SetHeader("Content-Type", "application/json");

                    using (var orderOperationStream = CreateOrderResponseStream(orderId: "1"))
                    using (var stream = await orderOperationMessage.GetStreamAsync())
                    {
                        orderOperationStream.Seek(0L, SeekOrigin.Begin);
                        await orderOperationStream.CopyToAsync(stream);
                    }

                    await jsonBatchWriter.WriteEndBatchAsync();
                },
                writingRequest: false,
                configureServices);

            Assert.Equal("{\"responses\":[" +
                "{\"id\":\"1\"," +
                "\"status\":0," +
                "\"headers\":{\"odata-version\":\"4.0\",\"content-type\":\"application/json\"}, " +
                "\"body\" :{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\",\"Id\":1,\"Name\":\"Customer 1\",\"Type\":\"Retail\"}}," +
                "{\"id\":\"2\"," +
                "\"status\":0," +
                "\"headers\":{\"odata-version\":\"4.0\",\"content-type\":\"application/json\"}, " +
                "\"body\" :{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$entity\",\"Id\":1,\"CustomerId\":1,\"Amount\":13}}" +
                "]}",
                result);
        }

        [Fact]
        public async Task WriteBatchRequestWithAbsoluteUriUsingHostHeaderAsync()
        {
            var result = await SetupJsonBatchWriterAndRunTestAsync(
                async (jsonBatchWriter) =>
                {
                    await jsonBatchWriter.WriteStartBatchAsync();

                    var operationRequestMessage = await jsonBatchWriter.CreateOperationRequestMessageAsync(
                        "POST", new Uri($"{ServiceUri}/odata/Customers"), "1", BatchPayloadUriOption.AbsoluteUriUsingHostHeader);

                    await using (var messageWriter = new ODataMessageWriter(operationRequestMessage))
                    {
                        var jsonWriter = await messageWriter.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);

                        var customerResource = CreateCustomerResource(1);
                        await jsonWriter.WriteStartAsync(customerResource);
                        await jsonWriter.WriteEndAsync();
                    }

                    await jsonBatchWriter.WriteEndBatchAsync();
                });

            Assert.Equal("{\"requests\":[{" +
                "\"id\":\"1\"," +
                "\"method\":\"POST\"," +
                "\"url\":\"/odata/Customers\"," +
                "\"headers\":{\"host\":\"tempuri.org:80\",\"odata-version\":\"4.0\",\"content-type\":\"application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\"}, " +
                "\"body\" :{\"Id\":1,\"Name\":\"Customer 1\",\"Type\":\"Retail\"}}]}",
                result);
        }

        [Fact]
        public async Task WriteBatchRequestWithRelativeUriAsync()
        {
            this.settings.BaseUri = new Uri(ServiceUri);

            var result = await SetupJsonBatchWriterAndRunTestAsync(
                async (jsonBatchWriter) =>
                {
                    await jsonBatchWriter.WriteStartBatchAsync();

                    var operationRequestMessage = await jsonBatchWriter.CreateOperationRequestMessageAsync(
                        "POST", new Uri("/odata/Customers", UriKind.Relative), "1", BatchPayloadUriOption.RelativeUri);

                    await using (var messageWriter = new ODataMessageWriter(operationRequestMessage))
                    {
                        var jsonWriter = await messageWriter.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);

                        var customerResource = CreateCustomerResource(1);
                        await jsonWriter.WriteStartAsync(customerResource);
                        await jsonWriter.WriteEndAsync();
                    }

                    await jsonBatchWriter.WriteEndBatchAsync();
                });

            Assert.Equal(
                "{\"requests\":[{" +
                "\"id\":\"1\"," +
                "\"method\":\"POST\"," +
                "\"url\":\"odata/Customers\"," +
                "\"headers\":{\"odata-version\":\"4.0\",\"content-type\":\"application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\"}, " +
                "\"body\" :{\"Id\":1,\"Name\":\"Customer 1\",\"Type\":\"Retail\"}}]}",
                result);
        }

        [Fact]
        public async Task WriteBatchRequestAsync_ReportMessageCompleted()
        {
            var result = await SetupJsonBatchWriterAndRunTestAsync(
                async (jsonBatchWriter) =>
                {
                    await jsonBatchWriter.WriteStartBatchAsync();
                    var operationRequestMessage = await jsonBatchWriter.CreateOperationRequestMessageAsync(
                        "POST", new Uri($"{ServiceUri}/Customers"), "1");
                    // No writer created for the request message
                    await jsonBatchWriter.WriteEndBatchAsync();
                });

            Assert.Equal(
                "{\"requests\":[{\"id\":\"1\",\"method\":\"POST\",\"url\":\"http://tempuri.org/Customers\",\"headers\":{}}]}",
                result);
        }

        #region Exception Cases

        [Fact]
        public async Task WriteBatchRequestAsync_ThrowsExceptionForChangesetStartedWithinChangeset()
        {
            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonBatchWriterAndRunTestAsync(
                    async (jsonBatchWriter) =>
                    {
                        await jsonBatchWriter.WriteStartBatchAsync();
                        await jsonBatchWriter.WriteStartChangesetAsync();
                        // Try to start writing a changeset when another is active
                        await jsonBatchWriter.WriteStartChangesetAsync();
                    }));

            Assert.Equal(Strings.ODataBatchWriter_CannotStartChangeSetWithActiveChangeSet, exception.Message);
        }

        [Fact]
        public async Task WriteBatchRequestAsync_ThrowsExceptionForEndChangesetNotPrecededByStartChangeset()
        {
            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonBatchWriterAndRunTestAsync(
                    async (jsonBatchWriter) =>
                    {
                        await jsonBatchWriter.WriteStartBatchAsync();
                        // Try to end changeset when there's none active
                        await jsonBatchWriter.WriteEndChangesetAsync();
                    }));

            Assert.Equal(Strings.ODataBatchWriter_CannotCompleteChangeSetWithoutActiveChangeSet, exception.Message);
        }

        [Fact]
        public async Task WriteBatchRequestAsync_ThrowsExceptionForEndBatchBeforeEndChangeset()
        {
            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonBatchWriterAndRunTestAsync(
                    async (jsonBatchWriter) =>
                    {
                        await jsonBatchWriter.WriteStartBatchAsync();
                        await jsonBatchWriter.WriteStartChangesetAsync();
                        // Try to stop writing batch before changeset end
                        await jsonBatchWriter.WriteEndBatchAsync();
                    }));

            Assert.Equal(Strings.ODataBatchWriter_CannotCompleteBatchWithActiveChangeSet, exception.Message);
        }

        [Fact]
        public async Task WriteBatchRequestAsync_ThrowsExceptionForNoStartBatch()
        {
            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonBatchWriterAndRunTestAsync(
                async (jsonBatchWriter) =>
                {
                    // Try to start writing changeset before batch start
                    await jsonBatchWriter.WriteStartChangesetAsync();
                }));

            Assert.Equal(Strings.ODataBatchWriter_InvalidTransitionFromStart, exception.Message);
        }

        [Fact]
        public async Task WriteBatchRequestAsync_ThrowsExceptionForMultipleStartBatch()
        {
            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonBatchWriterAndRunTestAsync(
                async (jsonBatchWriter) =>
                {
                    await jsonBatchWriter.WriteStartBatchAsync();
                    // Try to start writing batch again
                    await jsonBatchWriter.WriteStartBatchAsync();
                }));

            Assert.Equal(Strings.ODataBatchWriter_InvalidTransitionFromBatchStarted, exception.Message);
        }

        [Fact]
        public async Task WriteBatchRequestAsync_ThrowsExceptionForStateTransitionsAfterEndBatch()
        {
            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonBatchWriterAndRunTestAsync(
                async (jsonBatchWriter) =>
                {
                    await jsonBatchWriter.WriteStartBatchAsync();
                    await jsonBatchWriter.WriteEndBatchAsync();
                    // Try to start writing batch after batch end
                    await jsonBatchWriter.WriteStartBatchAsync();
                }));

            Assert.Equal(Strings.ODataBatchWriter_InvalidTransitionFromBatchCompleted, exception.Message);
        }

        [Fact]
        public async Task WriteBatchRequestAsync_ThrowsExceptionForResponseOperationMessage()
        {
            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonBatchWriterAndRunTestAsync(
                async (jsonBatchWriter) =>
                {
                    await jsonBatchWriter.WriteStartBatchAsync();
                    await jsonBatchWriter.CreateOperationResponseMessageAsync("1");
                },
                /*writingRequest*/ true));

            Assert.Equal(Strings.ODataBatchWriter_CannotCreateResponseOperationWhenWritingRequest, exception.Message);
        }

        [Fact]
        public async Task WriteBatchResponseAsync_ThrowsExceptionForRequestOperationMessage()
        {
            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonBatchWriterAndRunTestAsync(
                async (jsonBatchWriter) =>
                {
                    await jsonBatchWriter.WriteStartBatchAsync();
                    await jsonBatchWriter.CreateOperationRequestMessageAsync(
                        "POST", new Uri($"{ServiceUri}/Customers"), "1");
                },
                /*writingRequest*/ false));

            Assert.Equal(Strings.ODataBatchWriter_CannotCreateRequestOperationWhenWritingResponse, exception.Message);
        }

        [Fact]
        public async Task WriteBatchRequestAsync_ThrowsExceptionForInvalidTransitionFromEndChangeset()
        {
            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonBatchWriterAndRunTestAsync(
                async (jsonBatchWriter) =>
                {
                    await jsonBatchWriter.WriteStartBatchAsync();
                    await jsonBatchWriter.WriteStartChangesetAsync();
                    await jsonBatchWriter.WriteEndChangesetAsync();
                    // // Try to start writing batch after changeset end
                    await jsonBatchWriter.WriteStartBatchAsync();
                }));

            Assert.Equal(Strings.ODataBatchWriter_InvalidTransitionFromChangeSetCompleted, exception.Message);
        }

        [Fact]
        public async Task WriteBatchRequestAsync_ThrowsExceptionForInvalidTransitionFromOperationStreamDisposed()
        {
            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonBatchWriterAndRunTestAsync(
                async (jsonBatchWriter) =>
                {
                    await jsonBatchWriter.WriteStartBatchAsync();

                    var operationRequestMessage = await jsonBatchWriter.CreateOperationRequestMessageAsync(
                        "POST", new Uri($"{ServiceUri}/Customers"), "1");

                    await using (var messageWriter = new ODataMessageWriter(operationRequestMessage))
                    {
                        var jsonWriter = await messageWriter.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);

                        var customerResource = CreateCustomerResource(1);
                        await jsonWriter.WriteStartAsync(customerResource);
                        await jsonWriter.WriteEndAsync();
                    }

                    // Try to start writing batch after operation stream disposed
                    await jsonBatchWriter.WriteStartBatchAsync();
                }));

            Assert.Equal(Strings.ODataBatchWriter_InvalidTransitionFromOperationContentStreamDisposed, exception.Message);
        }

        [Fact]
        public async Task WriteBatchRequestAsync_ThrowsExceptionForInvalidTransitionFromOperationStreamRequested()
        {
            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonBatchWriterAndRunTestAsync(
                async (jsonBatchWriter) =>
                {
                    await jsonBatchWriter.WriteStartBatchAsync();

                    var operationRequestMessage = await jsonBatchWriter.CreateOperationRequestMessageAsync(
                        "POST", new Uri($"{ServiceUri}/Customers"), "1");

                    await using (var messageWriter = new ODataMessageWriter(operationRequestMessage))
                    {
                        await messageWriter.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);

                        // Try to end writing batch after operation stream requested
                        await jsonBatchWriter.WriteEndBatchAsync();
                    }
                }));

            Assert.Equal(Strings.ODataBatchWriter_InvalidTransitionFromOperationContentStreamRequested, exception.Message);
        }

        [Fact]
        public async Task WriteBatchRequestAsync_ThrowsExceptionForInvalidTransitionFromOperationCreated()
        {
            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonBatchWriterAndRunTestAsync(
                async (jsonBatchWriter) =>
                {
                    await jsonBatchWriter.WriteStartBatchAsync();
                    await jsonBatchWriter.CreateOperationRequestMessageAsync(
                        "POST", new Uri($"{ServiceUri}/Customers"), "1");
                    // Try to start writing batch after operation created
                    await jsonBatchWriter.WriteStartBatchAsync();
                }));

            Assert.Equal(Strings.ODataBatchWriter_InvalidTransitionFromOperationCreated, exception.Message);
        }

        [Fact]
        public async Task WriteBatchRequestAsync_ThrowsExceptionForInvalidTransitionFromChangesetStarted()
        {
            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonBatchWriterAndRunTestAsync(
                async (jsonBatchWriter) =>
                {
                    await jsonBatchWriter.WriteStartBatchAsync();
                    await jsonBatchWriter.WriteStartChangesetAsync();
                    // Try to start writing batch after operation created
                    await jsonBatchWriter.WriteStartBatchAsync();
                }));

            Assert.Equal(Strings.ODataBatchWriter_InvalidTransitionFromChangeSetStarted, exception.Message);
        }

        [Fact]
        public async Task WriteBatchRequestAsync_ThrowsExceptionForSynchronousOutputContext()
        {
            var jsonOutputContext = CreateJsonOutputContext(/*writingRequest*/ true, /*asynchronous*/ false);
            var jsonBatchWriter = new ODataJsonBatchWriter(jsonOutputContext);
            // Try to asynchronously start writing batch with an output context intended for synchronous writing
            var exception = await Assert.ThrowsAsync<ODataException>(
                () => jsonBatchWriter.WriteStartBatchAsync());

            Assert.Equal(Strings.ODataBatchWriter_AsyncCallOnSyncWriter, exception.Message);
        }

        [Fact]
        public async Task WriteBatchRequestAsync_ThrowsExceptionForGetMethodInChangeset()
        {
            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonBatchWriterAndRunTestAsync(
                async (jsonBatchWriter) =>
                {
                    await jsonBatchWriter.WriteStartBatchAsync();
                    await jsonBatchWriter.WriteStartChangesetAsync();
                    // Try to create operation request message for a GET
                    await jsonBatchWriter.CreateOperationRequestMessageAsync(
                        "GET", new Uri($"{ServiceUri}/Customers(1)"), "1");
                }));

            Assert.Equal(Strings.ODataBatch_InvalidHttpMethodForChangeSetRequest("GET"), exception.Message);
        }

        [Fact]
        public async Task WriteBatchRequestWithChangesetAsync_ThrowsExceptionForContentIdIsNull()
        {
            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonBatchWriterAndRunTestAsync(
                async (jsonBatchWriter) =>
                {
                    await jsonBatchWriter.WriteStartBatchAsync();
                    await jsonBatchWriter.WriteStartChangesetAsync();
                    // Try to create operation request message with null content id
                    await jsonBatchWriter.CreateOperationRequestMessageAsync(
                        "PUT", new Uri($"{ServiceUri}/Customers(1)"), /*contentId*/ null);
                }));

            Assert.Equal(Strings.ODataBatchOperationHeaderDictionary_KeyNotFound(ODataConstants.ContentIdHeader), exception.Message);
        }

        [Fact]
        public async Task WriteBatchRequestAsync_ThrowsExceptionForMaxPartsPerBatchLimitExceeded()
        {
            this.settings.MessageQuotas.MaxPartsPerBatch = 1;

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonBatchWriterAndRunTestAsync(
                async (jsonBatchWriter) =>
                {
                    await jsonBatchWriter.WriteStartBatchAsync();
                    await jsonBatchWriter.WriteStartChangesetAsync();
                    await jsonBatchWriter.WriteEndChangesetAsync();
                    await jsonBatchWriter.WriteStartChangesetAsync();
                }));

            Assert.Equal(Strings.ODataBatchWriter_MaxBatchSizeExceeded(1), exception.Message);
        }

        [Fact]
        public async Task WriteBatchRequestAsync_ThrowsExceptionForMaxOperationsPerChangesetLimitExceeded()
        {
            this.settings.MessageQuotas.MaxOperationsPerChangeset = 1;

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonBatchWriterAndRunTestAsync(
                async (jsonBatchWriter) =>
                {
                    await jsonBatchWriter.WriteStartBatchAsync();
                    await jsonBatchWriter.WriteStartChangesetAsync();
                    await jsonBatchWriter.CreateOperationRequestMessageAsync(
                        "PUT", new Uri($"{ServiceUri}/Customers(1)"), "1");
                    await jsonBatchWriter.CreateOperationRequestMessageAsync(
                        "PUT", new Uri($"{ServiceUri}/Customers(1)"), "2");
                }));

            Assert.Equal(Strings.ODataBatchWriter_MaxChangeSetSizeExceeded(1), exception.Message);
        }

        [Fact]
        public async Task WriteBatchRequestAsync_ThrowsExceptionForDuplicateContentId()
        {
            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonBatchWriterAndRunTestAsync(
                async (jsonBatchWriter) =>
                {
                    await jsonBatchWriter.WriteStartBatchAsync();
                    await jsonBatchWriter.CreateOperationRequestMessageAsync(
                        "PUT", new Uri($"{ServiceUri}/Customers(1)"), "1");
                    // Try to create operation request message with similar content id
                    await jsonBatchWriter.CreateOperationRequestMessageAsync(
                        "PUT", new Uri($"{ServiceUri}/Customers(1)"), "1");
                }));

            Assert.Equal(Strings.ODataBatchWriter_DuplicateContentIDsNotAllowed("1"), exception.Message);
        }

        [Fact]
        public async Task WriteBatchRequestAsync_ThrowsExceptionForContentIdNotInTheSameAtomicityGroup()
        {
            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonBatchWriterAndRunTestAsync(
                async (jsonBatchWriter) =>
                {
                    await jsonBatchWriter.WriteStartBatchAsync();
                    await jsonBatchWriter.WriteStartChangesetAsync("fd04fc24");

                    await jsonBatchWriter.CreateOperationRequestMessageAsync(
                        "POST", new Uri($"{ServiceUri}/Customers"), "1");

                    await jsonBatchWriter.WriteEndChangesetAsync();
                    await jsonBatchWriter.WriteStartChangesetAsync("b62a2456");

                    // Operation request depends on content id from different atomicity group
                    var dependsOnIds = new List<string> { "1" };
                    await jsonBatchWriter.CreateOperationRequestMessageAsync(
                        "POST", new Uri($"{ServiceUri}/Orders"), "2", BatchPayloadUriOption.AbsoluteUri, dependsOnIds);
                }));

            Assert.Equal(Strings.ODataBatchReader_DependsOnRequestIdIsPartOfAtomicityGroupNotAllowed("2", "fd04fc24"), exception.Message);
        }

        #endregion Exception Cases

        /// <summary>
        /// Sets up an ODataJsonBatchWriter,
        /// then runs the given test code asynchronously,
        /// then flushes and reads the stream back as a string for customized verification.
        /// </summary>
        private async Task<string> SetupJsonBatchWriterAndRunTestAsync(
            Func<ODataJsonBatchWriter, Task> func,
            bool writingRequest = true,
            Action<IServiceCollection> configureServices = null)
        {
            this.stream = new AsyncStream(this.stream);
            var jsonOutputContext = CreateJsonOutputContext(writingRequest, asynchronous: true, configureServices: configureServices);
            var jsonBatchWriter = new ODataJsonBatchWriter(jsonOutputContext);

            await func(jsonBatchWriter);

            this.stream.Position = 0;
            return await new StreamReader(this.stream).ReadToEndAsync();
        }

        private ODataJsonOutputContext CreateJsonOutputContext(bool writingRequest = true, bool asynchronous = false, Action<IServiceCollection> configureServices = null)
        {
            IServiceProvider serviceProvider = configureServices == null ? null : CreateServiceProvider(configureServices);

            var messageInfo = new ODataMessageInfo
            {
                MessageStream = this.stream,
                MediaType = this.mediaType,
                Encoding = this.encoding,
                IsResponse = !writingRequest,
                IsAsync = asynchronous,
                Model = this.model,
                ServiceProvider = serviceProvider,
            };

            return new ODataJsonOutputContext(messageInfo, this.settings);
        }

        #region Helper Methods

        private static ODataResource CreateCustomerResource(int customerId)
        {
            return new ODataResource
            {
                TypeName = "NS.Customer",
                Properties = new List<ODataProperty>
                {
                    new ODataProperty
                    {
                        Name = "Id",
                        Value = customerId,
                        SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Key }
                    },
                    new ODataProperty { Name = "Name", Value = $"Customer {customerId}" },
                    new ODataProperty { Name = "Type", Value = new ODataEnumValue("Retail") }
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

        private static ODataResource CreateOrderResource(int orderId)
        {
            return new ODataResource
            {
                TypeName = "NS.Order",
                Properties = new List<ODataProperty>
                {
                    new ODataProperty
                    {
                        Name = "Id",
                        Value = orderId,
                        SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Key }
                    },
                    new ODataProperty { Name = "CustomerId", Value = 1 },
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

        private static Stream CreateCustomerResponseStream(string customerId)
        {
            string content = $"{{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\",\"Id\":{customerId},\"Name\":\"Customer 1\",\"Type\":\"Retail\"}}";
            return CreateStreamWithContents(content);
        }

        private static Stream CreateOrderResponseStream(string orderId)
        {
            string content = $"{{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$entity\",\"Id\":{orderId},\"CustomerId\":1,\"Amount\":13}}";
            return CreateStreamWithContents(content);
        }

        /// <summary>
        /// Creates a stream that contains the specified content in the specified encoding.
        /// </summary>
        /// <param name="content">Content to fill the stream with.</param>
        /// <returns>The stream with the specified content.</returns>
        /// <remarks>The stream's cursor is positioned at the end.
        /// You will need to seek or reposition the cursor if you want to read the stream from the beginning.
        /// </remarks>
        private static Stream CreateStreamWithContents(string content, Encoding encoding = null)
        {
            MemoryStream stream = new MemoryStream();
            byte[] encoded = (encoding ?? Encoding.UTF8).GetBytes(content);
            stream.Write(encoded, 0, encoded.Length);
            return stream;
        }

        private static IServiceProvider CreateServiceProvider(Action<IServiceCollection> configureServices)
        {
            return ServiceProviderHelper.BuildServiceProvider(configureServices);
        }

        #endregion Helper Methods
    }
}

//---------------------------------------------------------------------
// <copyright file="ODataJsonBatchReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Microsoft.OData.Tests;
using Xunit;

namespace Microsoft.OData.Core.Tests.Json
{
    public class ODataJsonBatchReaderTests
    {
        private const string ServiceUri = "http://tempuri.org";
        private EdmModel model;
        private ODataMessageReaderSettings messageReaderSettings;
        private ODataMediaType mediaType;

        private EdmEnumType customerTypeEnumType;
        private EdmEntityType customerEntityType;
        private EdmEntityType orderEntityType;
        private EdmEntitySet customerEntitySet;
        private EdmEntitySet orderEntitySet;

        public ODataJsonBatchReaderTests()
        {
            InitializeEdmModel();
            this.messageReaderSettings = new ODataMessageReaderSettings { Version = ODataVersion.V4 };
            this.mediaType = new ODataMediaType("application", "json",
                new[]
                {
                    new KeyValuePair<string, string>("odata.metadata", "minimal"),
                    new KeyValuePair<string, string>("odata.streaming", "true"),
                    new KeyValuePair<string, string>("IEEE754Compatible", "false"),
                    new KeyValuePair<string, string>("charset", "utf-8")
                });
        }

        [Fact]
        public async Task ReadBatchRequestAsync()
        {
            var payload = "{\"requests\":[{" +
                "\"id\":\"1\"," +
                "\"method\":\"POST\"," +
                "\"url\":\"http://tempuri.org/Customers\"," +
                "\"headers\":{\"odata-version\":\"4.0\",\"content-type\":\"application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\"}, " +
                "\"body\" :{\"@odata.type\":\"#NS.Customer\",\"Id\":1,\"Name\":\"Customer 1\",\"Type\":\"Retail\"}}]}";

            await SetupJsonBatchReaderAndRunTestAsync(
                payload,
                async (jsonBatchReader) =>
                {
                    while (await jsonBatchReader.ReadAsync())
                    {
                        switch (jsonBatchReader.State)
                        {
                            case ODataBatchReaderState.Operation:
                                var operationRequestMessage = await jsonBatchReader.CreateOperationRequestMessageAsync();

                                Assert.Equal("1", operationRequestMessage.ContentId);
                                Assert.NotNull(operationRequestMessage.Url);
                                Assert.Equal("http://tempuri.org/Customers", operationRequestMessage.Url.AbsoluteUri);
                                Assert.Equal("POST", operationRequestMessage.Method);

                                using (var messageReader = new ODataMessageReader(operationRequestMessage, new ODataMessageReaderSettings(), this.model))
                                {
                                    var jsonResourceReader = await messageReader.CreateODataResourceReaderAsync();

                                    await DoReadAsync(
                                        jsonResourceReader,
                                        verifyResourceAction: (resource) =>
                                        {
                                            Assert.NotNull(resource);
                                            Assert.Equal("NS.Customer", resource.TypeName);
                                            Assert.Equal(3, resource.Properties.Count());
                                            var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                                            Assert.Equal("Id", properties[0].Name);
                                            Assert.Equal(1, properties[0].Value);
                                            Assert.Equal("Name", properties[1].Name);
                                            Assert.Equal("Customer 1", properties[1].Value);
                                            Assert.Equal("Type", properties[2].Name);
                                            var customerTypeEnumValue = Assert.IsType<ODataEnumValue>(properties[2].Value);
                                            Assert.Equal("Retail", customerTypeEnumValue.Value);
                                        });
                                }

                                break;
                            default:
                                break;
                        }
                    }
                },
                isResponse: false);
        }

        [Fact]
        public async Task ReadBatchRequestWithChangesetAsync()
        {
            var payload = "{\"requests\":[{" +
                "\"id\":\"1\"," +
                "\"atomicityGroup\":\"69028f2c-f57b-4850-89f0-b7e5e002d4bc\"," +
                "\"method\":\"POST\"," +
                "\"url\":\"http://tempuri.org/Customers\"," +
                "\"headers\":{\"odata-version\":\"4.0\",\"content-type\":\"application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\"}, " +
                "\"body\" :{\"@odata.type\":\"#NS.Customer\",\"Id\":1,\"Name\":\"Customer 1\",\"Type\":\"Retail\"}}]}";

            await SetupJsonBatchReaderAndRunTestAsync(
                payload,
                async (jsonBatchReader) =>
                {
                    while (await jsonBatchReader.ReadAsync())
                    {
                        switch (jsonBatchReader.State)
                        {
                            case ODataBatchReaderState.Operation:
                                var operationRequestMessage = await jsonBatchReader.CreateOperationRequestMessageAsync();

                                Assert.Equal("1", operationRequestMessage.ContentId);
                                Assert.NotNull(operationRequestMessage.Url);
                                Assert.Equal("http://tempuri.org/Customers", operationRequestMessage.Url.AbsoluteUri);
                                Assert.Equal("POST", operationRequestMessage.Method);

                                using (var messageReader = new ODataMessageReader(operationRequestMessage, new ODataMessageReaderSettings(), this.model))
                                {
                                    var jsonResourceReader = await messageReader.CreateODataResourceReaderAsync();

                                    await DoReadAsync(
                                        jsonResourceReader,
                                        verifyResourceAction: (resource) =>
                                        {
                                            Assert.NotNull(resource);
                                            Assert.Equal("NS.Customer", resource.TypeName);
                                            Assert.Equal(3, resource.Properties.Count());
                                            var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                                            Assert.Equal("Id", properties[0].Name);
                                            Assert.Equal(1, properties[0].Value);
                                            Assert.Equal("Name", properties[1].Name);
                                            Assert.Equal("Customer 1", properties[1].Value);
                                            Assert.Equal("Type", properties[2].Name);
                                            var customerTypeEnumValue = Assert.IsType<ODataEnumValue>(properties[2].Value);
                                            Assert.Equal("Retail", customerTypeEnumValue.Value);
                                        });
                                }

                                break;
                            case ODataBatchReaderState.ChangesetStart:
                                Assert.Equal("69028f2c-f57b-4850-89f0-b7e5e002d4bc", jsonBatchReader.CurrentGroupId);
                                break;
                            default:
                                break;
                        }
                    }
                },
                isResponse: false);
        }

        [Fact]
        public async Task ReadBatchRequestWithDependsOnIdsAsync()
        {
            var payload = "{\"requests\":[" +
                "{\"id\":\"1\"," +
                "\"method\":\"POST\"," +
                "\"url\":\"http://tempuri.org/Customers\"," +
                "\"headers\":{\"odata-version\":\"4.0\",\"content-type\":\"application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\"}, " +
                "\"body\" :{\"@odata.type\":\"#NS.Customer\",\"Id\":1,\"Name\":\"Customer 1\",\"Type\":\"Retail\"}}," +
                "{\"id\":\"2\"," +
                "\"dependsOn\":[\"1\"]," +
                "\"method\":\"POST\"," +
                "\"url\":\"http://tempuri.org/Orders\"," +
                "\"headers\":{\"odata-version\":\"4.0\",\"content-type\":\"application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\"}, " +
                "\"body\" :{\"@odata.type\":\"#NS.Order\",\"Id\":1,\"CustomerId\":1,\"Amount\":13}}]}";

            var verifyContentIdStack = new Stack<string>(new[] { "2", "1" });
            var verifyUrlStack = new Stack<string>(new[] { "http://tempuri.org/Orders", "http://tempuri.org/Customers" });

            var verifyDependsOnIdsStack = new Stack<Action<IEnumerable<string>>>();
            verifyDependsOnIdsStack.Push((dependsOnIds) => Assert.Equal("1", Assert.Single(dependsOnIds)));
            verifyDependsOnIdsStack.Push((dependsOnIds) => Assert.Empty(dependsOnIds));

            var verifyResourceStack = new Stack<Action<ODataResource>>();
            verifyResourceStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Order", resource.TypeName);
                Assert.Equal(3, resource.Properties.Count());
                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("CustomerId", properties[1].Name);
                Assert.Equal(1, properties[1].Value);
                Assert.Equal("Amount", properties[2].Name);
                Assert.Equal(13M, properties[2].Value);
            });
            verifyResourceStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Customer", resource.TypeName);
                Assert.Equal(3, resource.Properties.Count());
                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Name", properties[1].Name);
                Assert.Equal("Customer 1", properties[1].Value);
                Assert.Equal("Type", properties[2].Name);
                var customerTypeEnumValue = Assert.IsType<ODataEnumValue>(properties[2].Value);
                Assert.Equal("Retail", customerTypeEnumValue.Value);
            });

            await SetupJsonBatchReaderAndRunTestAsync(
                payload,
                async (jsonBatchReader) =>
                {
                    while (await jsonBatchReader.ReadAsync())
                    {
                        switch (jsonBatchReader.State)
                        {
                            case ODataBatchReaderState.Operation:
                                var operationRequestMessage = await jsonBatchReader.CreateOperationRequestMessageAsync();

                                Assert.NotEmpty(verifyContentIdStack);
                                Assert.Equal(verifyContentIdStack.Pop(), operationRequestMessage.ContentId);

                                Assert.NotEmpty(verifyUrlStack);
                                Assert.NotNull(operationRequestMessage.Url);
                                Assert.Equal(verifyUrlStack.Pop(), operationRequestMessage.Url.AbsoluteUri);

                                Assert.NotEmpty(verifyDependsOnIdsStack);
                                var verifyDependsOnId = verifyDependsOnIdsStack.Pop();
                                verifyDependsOnId(operationRequestMessage.DependsOnIds);

                                using (var messageReader = new ODataMessageReader(operationRequestMessage, new ODataMessageReaderSettings(), this.model))
                                {
                                    var jsonResourceReader = await messageReader.CreateODataResourceReaderAsync();

                                    await DoReadAsync(
                                        jsonResourceReader,
                                        verifyResourceAction: (resource) =>
                                        {
                                            Assert.NotEmpty(verifyResourceStack);
                                            var innerVerifyResourceStack = verifyResourceStack.Pop();
                                            innerVerifyResourceStack(resource);
                                        });
                                }

                                break;
                            default:
                                break;
                        }
                    }
                },
                isResponse: false);
        }

        [Fact]
        public async Task ReadBatchRequestWithChangesetAndDependsOnIdsAsync()
        {
            var payload = "{\"requests\":[" +
                "{\"id\":\"1\"," +
                "\"atomicityGroup\":\"69028f2c-f57b-4850-89f0-b7e5e002d4bc\"," +
                "\"method\":\"POST\"," +
                "\"url\":\"http://tempuri.org/Customers\"," +
                "\"headers\":{\"odata-version\":\"4.0\",\"content-type\":\"application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\"}, " +
                "\"body\" :{\"@odata.type\":\"#NS.Customer\",\"Id\":1,\"Name\":\"Customer 1\",\"Type\":\"Retail\"}}," +
                "{\"id\":\"2\"," +
                "\"atomicityGroup\":\"69028f2c-f57b-4850-89f0-b7e5e002d4bc\"," +
                "\"dependsOn\":[\"1\"]," +
                "\"method\":\"POST\"," +
                "\"url\":\"http://tempuri.org/Orders\"," +
                "\"headers\":{\"odata-version\":\"4.0\",\"content-type\":\"application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\"}, " +
                "\"body\" :{\"@odata.type\":\"#NS.Order\",\"Id\":1,\"CustomerId\":1,\"Amount\":13}}]}";

            var verifyContentIdStack = new Stack<string>(new[] { "2", "1" });
            var verifyUrlStack = new Stack<string>(new[] { "http://tempuri.org/Orders", "http://tempuri.org/Customers" });

            var verifyDependsOnIdsStack = new Stack<Action<IEnumerable<string>>>();
            verifyDependsOnIdsStack.Push((dependsOnIds) => Assert.Equal("1", Assert.Single(dependsOnIds)));
            verifyDependsOnIdsStack.Push((dependsOnIds) => Assert.Empty(dependsOnIds));

            var verifyResourceStack = new Stack<Action<ODataResource>>();
            verifyResourceStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Order", resource.TypeName);
                Assert.Equal(3, resource.Properties.Count());
                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("CustomerId", properties[1].Name);
                Assert.Equal(1, properties[1].Value);
                Assert.Equal("Amount", properties[2].Name);
                Assert.Equal(13M, properties[2].Value);
            });
            verifyResourceStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Customer", resource.TypeName);
                Assert.Equal(3, resource.Properties.Count());
                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Name", properties[1].Name);
                Assert.Equal("Customer 1", properties[1].Value);
                Assert.Equal("Type", properties[2].Name);
                var customerTypeEnumValue = Assert.IsType<ODataEnumValue>(properties[2].Value);
                Assert.Equal("Retail", customerTypeEnumValue.Value);
            });

            await SetupJsonBatchReaderAndRunTestAsync(
                payload,
                async (jsonBatchReader) =>
                {
                    while (await jsonBatchReader.ReadAsync())
                    {
                        switch (jsonBatchReader.State)
                        {
                            case ODataBatchReaderState.Operation:
                                var operationRequestMessage = await jsonBatchReader.CreateOperationRequestMessageAsync();

                                Assert.NotEmpty(verifyContentIdStack);
                                Assert.Equal(verifyContentIdStack.Pop(), operationRequestMessage.ContentId);

                                Assert.NotEmpty(verifyUrlStack);
                                Assert.NotNull(operationRequestMessage.Url);
                                Assert.Equal(verifyUrlStack.Pop(), operationRequestMessage.Url.AbsoluteUri);

                                Assert.NotEmpty(verifyDependsOnIdsStack);
                                var verifyDependsOnId = verifyDependsOnIdsStack.Pop();
                                verifyDependsOnId(operationRequestMessage.DependsOnIds);

                                Assert.Equal("POST", operationRequestMessage.Method);

                                using (var messageReader = new ODataMessageReader(operationRequestMessage, new ODataMessageReaderSettings(), this.model))
                                {
                                    var jsonResourceReader = await messageReader.CreateODataResourceReaderAsync();

                                    await DoReadAsync(
                                        jsonResourceReader,
                                        verifyResourceAction: (resource) =>
                                        {
                                            Assert.NotEmpty(verifyResourceStack);
                                            var innerVerifyResourceStack = verifyResourceStack.Pop();
                                            innerVerifyResourceStack(resource);
                                        });
                                }

                                break;
                            case ODataBatchReaderState.ChangesetStart:
                                Assert.Equal("69028f2c-f57b-4850-89f0-b7e5e002d4bc", jsonBatchReader.CurrentGroupId);
                                break;
                            default:
                                break;
                        }
                    }
                },
                isResponse: false);
        }

        [Fact]
        public void ReadBatchRequestWithNullHeaders()
        {
            var payload = "{\"requests\": [{" +
                "\"id\": \"1\"," +
                "\"method\": \"POST\"," +
                "\"url\": \"http://tempuri.org/Customers\"," +
                "\"headers\": {\"odata-version\":\"4.0\",\"content-type\":\"application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\",\"null-header\":null}, " +
                "\"body\": {\"@odata.type\":\"#NS.Customer\",\"Id\":1,\"Name\":\"Customer 1\",\"Type\":\"Retail\"}}]}";

            SetupJsonBatchReaderAndRunTest(
                payload,
                (jsonBatchReader) =>
                {
                    try
                    {
                        while (jsonBatchReader.Read())
                        {
                            if (jsonBatchReader.State == ODataBatchReaderState.Operation)
                            {
                                var operationRequestMessage = jsonBatchReader.CreateOperationRequestMessage();
                                // Verify that the Property "null-header" exists and it's value is set to NULL
                                var nullHeaderProperty = operationRequestMessage.Headers.FirstOrDefault(p => p.Key == "null-header");
                                Assert.NotNull(nullHeaderProperty.Key);
                                Assert.Null(nullHeaderProperty.Value);
                            }
                        }
                    }
                    catch (NullReferenceException ex)
                    {
                        Assert.Fail(ex.Message);
                    }
                },
                isResponse: false);
        }

        [Fact]
        public async Task ReadBatchRequestWithNullHeadersAsync()
        {
            var payload = "{\"requests\": [{" +
                "\"id\": \"1\"," +
                "\"method\": \"POST\"," +
                "\"url\": \"http://tempuri.org/Customers\"," +
                "\"headers\": {\"odata-version\":\"4.0\",\"content-type\":\"application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\",\"null-header\":null}, " +
                "\"body\": {\"@odata.type\":\"#NS.Customer\",\"Id\":1,\"Name\":\"Customer 1\",\"Type\":\"Retail\"}}]}";

            await SetupJsonBatchReaderAndRunTestAsync(
                payload,
                async (jsonBatchReader) =>
                {
                    try
                    {
                        while (await jsonBatchReader.ReadAsync())
                        {
                            if (jsonBatchReader.State == ODataBatchReaderState.Operation)
                            {
                                var operationRequestMessage = await jsonBatchReader.CreateOperationRequestMessageAsync();
                                // Verify that the Property "null-header" exists and it's value is set to NULL
                                var nullHeaderProperty = operationRequestMessage.Headers.FirstOrDefault(p => p.Key == "null-header");
                                Assert.NotNull(nullHeaderProperty.Key);
                                Assert.Null(nullHeaderProperty.Value);
                            }
                        }
                    }
                    catch (NullReferenceException ex)
                    {
                        Assert.Fail(ex.Message);
                    }
                },
                isResponse: false);
        }

        [Fact]
        public void ReadBatchRequestWithDuplicateProperties()
        {
            var payload = "{\"requests\": [{" +
                "\"id\": \"1\"," +
                "\"atomicityGroup\": \"g1\"," +
                "\"atomicityGroup\": \"g2\"," +
                "\"method\": \"POST\"," +
                "\"url\": \"http://tempuri.org/Customers\"," +
                "\"headers\": {\"odata-version\":\"4.0\",\"content-type\":\"application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\"}, " +
                "\"body\": {\"@odata.type\":\"#NS.Customer\",\"Id\":1,\"Name\":\"Customer 1\",\"Type\":\"Retail\"}}]}";

            var exception = Assert.Throws<ODataException>(
                () => SetupJsonBatchReaderAndRunTest(
                payload,
                (jsonBatchReader) =>
                {
                    while (jsonBatchReader.Read())
                    {
                        if (jsonBatchReader.State == ODataBatchReaderState.Operation)
                        {
                            // The json properties are just iterated through and have no purpose for this test case
                            jsonBatchReader.CreateOperationRequestMessage();
                        }
                    }
                    Assert.Fail("The test failed, because the duplicate header has not thrown an ODataException");
                },
                isResponse: false));

            // Verify that the correct duplicate property has raised the ODataException
            Assert.Equal(Error.Format(SRResources.ODataJsonBatchPayloadItemPropertiesCache_DuplicatePropertyForRequestInBatch, "ATOMICITYGROUP"), exception.Message);
        }

        [Fact]
        public async Task ReadBatchRequestWithDuplicatePropertiesAsync()
        {
            var payload = "{\"requests\": [{" +
                "\"id\": \"1\"," +
                "\"atomicityGroup\": \"g1\"," +
                "\"atomicityGroup\": \"g2\"," +
                "\"method\": \"POST\"," +
                "\"url\": \"http://tempuri.org/Customers\"," +
                "\"headers\": {\"odata-version\":\"4.0\",\"content-type\":\"application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\"}, " +
                "\"body\": {\"@odata.type\":\"#NS.Customer\",\"Id\":1,\"Name\":\"Customer 1\",\"Type\":\"Retail\"}}]}";
            
            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonBatchReaderAndRunTestAsync(
                payload,
                async (jsonBatchReader) =>
                {
                    while (await jsonBatchReader.ReadAsync())
                    {
                        if (jsonBatchReader.State == ODataBatchReaderState.Operation)
                        {
                            // The json properties are just iterated through and have no purpose for this test case
                            await jsonBatchReader.CreateOperationRequestMessageAsync();
                        }
                    }
                    Assert.Fail("The test failed, because the duplicate header has not thrown an ODataException");
                },
                isResponse: false));

            // Verify that the correct duplicate property has raised the ODataException
            Assert.Equal(Error.Format(SRResources.ODataJsonBatchPayloadItemPropertiesCache_DuplicatePropertyForRequestInBatch, "ATOMICITYGROUP"), exception.Message);
        }

        [Fact]
        public void ReadBatchRequestWithDuplicateHeaders()
        {
            var payload = "{\"requests\": [{" +
                "\"id\": \"1\"," +
                "\"method\": \"POST\"," +
                "\"url\": \"http://tempuri.org/Customers\"," +
                "\"headers\": {\"odata-version\":\"4.0\",\"content-type\":\"application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\",\"duplicate-header\":\"value1\",\"duplicate-header\":\"value2\"}, " +
                "\"body\": {\"@odata.type\":\"#NS.Customer\",\"Id\":1,\"Name\":\"Customer 1\",\"Type\":\"Retail\"}}]}";

            var exception = Assert.Throws<ODataException>(
                () => SetupJsonBatchReaderAndRunTest(
                payload,
                (jsonBatchReader) =>
                {
                    while (jsonBatchReader.Read())
                    {
                        if (jsonBatchReader.State == ODataBatchReaderState.Operation)
                        {
                            // The json properties are just iterated through and have no purpose for this test case
                            jsonBatchReader.CreateOperationRequestMessage();
                        }
                    }
                    Assert.Fail("The test failed, because the duplicate header has thrown no ODataException");
                },
                isResponse: false));

            // Verify that the correct duplicate header has raised the ODataException
            Assert.Equal(Error.Format(SRResources.ODataJsonBatchPayloadItemPropertiesCache_DuplicateHeaderForRequestInBatch, "duplicate-header"), exception.Message);
        }

        [Fact]
        public async Task ReadBatchRequestWithDuplicateHeadersAsync()
        {
            var payload = "{\"requests\": [{" +
                "\"id\": \"1\"," +
                "\"method\": \"POST\"," +
                "\"url\": \"http://tempuri.org/Customers\"," +
                "\"headers\": {\"odata-version\":\"4.0\",\"content-type\":\"application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\",\"duplicate-header\":\"value1\",\"duplicate-header\":\"value2\"}, " +
                "\"body\": {\"@odata.type\":\"#NS.Customer\",\"Id\":1,\"Name\":\"Customer 1\",\"Type\":\"Retail\"}}]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonBatchReaderAndRunTestAsync(
                payload,
                async (jsonBatchReader) =>
                {
                        while (await jsonBatchReader.ReadAsync())
                        {
                            if (jsonBatchReader.State == ODataBatchReaderState.Operation)
                            {
                                // The json properties are just iterated through and have no purpose for this test case
                                await jsonBatchReader.CreateOperationRequestMessageAsync();
                            }
                        }
                        Assert.Fail("The test failed, because the duplicate header has thrown no ODataException");
                },
                isResponse: false));

            // Verify that the correct duplicate header has raised the ODataException
            Assert.Equal(Error.Format(SRResources.ODataJsonBatchPayloadItemPropertiesCache_DuplicateHeaderForRequestInBatch, "duplicate-header"), exception.Message);
        }

        [Fact]
        public async Task ReadBatchResponseAsync()
        {
            var payload = "{\"responses\":[{" +
                "\"id\":\"1\"," +
                "\"status\":0," +
                "\"headers\":{\"odata-version\":\"4.0\",\"content-type\":\"application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\"}, " +
                "\"body\" :{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\",\"Id\":1,\"Name\":\"Customer 1\",\"Type\":\"Retail\"}}]}";

            await SetupJsonBatchReaderAndRunTestAsync(
                payload,
                async (jsonBatchReader) =>
                {
                    while (await jsonBatchReader.ReadAsync())
                    {
                        switch (jsonBatchReader.State)
                        {
                            case ODataBatchReaderState.Operation:
                                var operationResponseMessage = await jsonBatchReader.CreateOperationResponseMessageAsync();

                                using (var messageReader = new ODataMessageReader(operationResponseMessage, new ODataMessageReaderSettings(), this.model))
                                {
                                    var jsonResourceReader = await messageReader.CreateODataResourceReaderAsync();

                                    await DoReadAsync(
                                        jsonResourceReader,
                                        verifyResourceAction: (resource) =>
                                        {
                                            Assert.NotNull(resource);
                                            Assert.Equal("NS.Customer", resource.TypeName);
                                            Assert.Equal(3, resource.Properties.Count());
                                            var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                                            Assert.Equal("Id", properties[0].Name);
                                            Assert.Equal(1, properties[0].Value);
                                            Assert.Equal("Name", properties[1].Name);
                                            Assert.Equal("Customer 1", properties[1].Value);
                                            Assert.Equal("Type", properties[2].Name);
                                            var customerTypeEnumValue = Assert.IsType<ODataEnumValue>(properties[2].Value);
                                            Assert.Equal("Retail", customerTypeEnumValue.Value);
                                        });
                                }

                                break;
                            default:
                                break;
                        }
                    }
                },
                isResponse: true);
        }

        [Fact]
        public async Task ReadBatchResponseWithChangesetAsync()
        {
            var payload = "{\"responses\":[{" +
                "\"id\":\"1\"," +
                "\"atomicityGroup\":\"69028f2c-f57b-4850-89f0-b7e5e002d4bc\"," +
                "\"status\":0," +
                "\"headers\":{\"odata-version\":\"4.0\",\"content-type\":\"application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\"}, " +
                "\"body\" :{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\",\"Id\":1,\"Name\":\"Customer 1\",\"Type\":\"Retail\"}}]}";

            await SetupJsonBatchReaderAndRunTestAsync(
                payload,
                async (jsonBatchReader) =>
                {
                    while (await jsonBatchReader.ReadAsync())
                    {
                        switch (jsonBatchReader.State)
                        {
                            case ODataBatchReaderState.Operation:
                                var operationResponseMessage = await jsonBatchReader.CreateOperationResponseMessageAsync();

                                using (var messageReader = new ODataMessageReader(operationResponseMessage, new ODataMessageReaderSettings(), this.model))
                                {
                                    var jsonResourceReader = await messageReader.CreateODataResourceReaderAsync();

                                    await DoReadAsync(
                                        jsonResourceReader,
                                        verifyResourceAction: (resource) =>
                                        {
                                            Assert.NotNull(resource);
                                            Assert.Equal("NS.Customer", resource.TypeName);
                                            Assert.Equal(3, resource.Properties.Count());
                                            var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                                            Assert.Equal("Id", properties[0].Name);
                                            Assert.Equal(1, properties[0].Value);
                                            Assert.Equal("Name", properties[1].Name);
                                            Assert.Equal("Customer 1", properties[1].Value);
                                            Assert.Equal("Type", properties[2].Name);
                                            var customerTypeEnumValue = Assert.IsType<ODataEnumValue>(properties[2].Value);
                                            Assert.Equal("Retail", customerTypeEnumValue.Value);
                                        });
                                }

                                break;
                            case ODataBatchReaderState.ChangesetStart:
                                Assert.Equal("69028f2c-f57b-4850-89f0-b7e5e002d4bc", jsonBatchReader.CurrentGroupId);
                                break;
                            default:
                                break;
                        }
                    }
                },
                isResponse: true);
        }

        [Fact]
        public async Task ReadBatchRequestWithAbsoluteUriUsingHostHeaderAsync()
        {
            this.messageReaderSettings.BaseUri = new Uri("http://tempuri.org");

            var payload = "{\"requests\":[{" +
                "\"id\":\"1\"," +
                "\"method\":\"POST\"," +
                "\"url\":\"/Customers\"," +
                "\"headers\":{\"host\":\"tempuri.org:80\",\"odata-version\":\"4.0\",\"content-type\":\"application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\"}, " +
                "\"body\" :{\"@odata.type\":\"#NS.Customer\",\"Id\":1,\"Name\":\"Customer 1\",\"Type\":\"Retail\"}}]}";

            await SetupJsonBatchReaderAndRunTestAsync(
                payload,
                async (jsonBatchReader) =>
                {
                    while (await jsonBatchReader.ReadAsync())
                    {
                        switch (jsonBatchReader.State)
                        {
                            case ODataBatchReaderState.Operation:
                                var operationRequestMessage = await jsonBatchReader.CreateOperationRequestMessageAsync();

                                Assert.Equal("1", operationRequestMessage.ContentId);
                                Assert.NotNull(operationRequestMessage.Url);
                                Assert.Equal("http://tempuri.org/Customers", operationRequestMessage.Url.AbsoluteUri);
                                Assert.Equal("POST", operationRequestMessage.Method);

                                using (var messageReader = new ODataMessageReader(operationRequestMessage, new ODataMessageReaderSettings(), this.model))
                                {
                                    var jsonResourceReader = await messageReader.CreateODataResourceReaderAsync();

                                    await DoReadAsync(
                                        jsonResourceReader,
                                        verifyResourceAction: (resource) =>
                                        {
                                            Assert.NotNull(resource);
                                            Assert.Equal("NS.Customer", resource.TypeName);
                                            Assert.Equal(3, resource.Properties.Count());
                                            var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                                            Assert.Equal("Id", properties[0].Name);
                                            Assert.Equal(1, properties[0].Value);
                                            Assert.Equal("Name", properties[1].Name);
                                            Assert.Equal("Customer 1", properties[1].Value);
                                            Assert.Equal("Type", properties[2].Name);
                                            var customerTypeEnumValue = Assert.IsType<ODataEnumValue>(properties[2].Value);
                                            Assert.Equal("Retail", customerTypeEnumValue.Value);
                                        });
                                }

                                break;
                            default:
                                break;
                        }
                    }
                },
                isResponse: false);
        }

        [Fact]
        public async Task ReadBatchRequestWithRelativeUriAsync()
        {
            this.messageReaderSettings.BaseUri = new Uri(ServiceUri);

            var payload = "{\"requests\":[{" +
                "\"id\":\"1\"," +
                "\"method\":\"POST\"," +
                "\"url\":\"/Customers\"," +
                "\"headers\":{\"odata-version\":\"4.0\",\"content-type\":\"application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\"}, " +
                "\"body\" :{\"@odata.type\":\"#NS.Customer\",\"Id\":1,\"Name\":\"Customer 1\",\"Type\":\"Retail\"}}]}";

            await SetupJsonBatchReaderAndRunTestAsync(
                payload,
                async (jsonBatchReader) =>
                {
                    while (await jsonBatchReader.ReadAsync())
                    {
                        switch (jsonBatchReader.State)
                        {
                            case ODataBatchReaderState.Operation:
                                var operationRequestMessage = await jsonBatchReader.CreateOperationRequestMessageAsync();

                                Assert.Equal("1", operationRequestMessage.ContentId);
                                Assert.NotNull(operationRequestMessage.Url);
                                Assert.Equal("http://tempuri.org/Customers", operationRequestMessage.Url.AbsoluteUri);
                                Assert.Equal("POST", operationRequestMessage.Method);

                                using (var messageReader = new ODataMessageReader(operationRequestMessage, new ODataMessageReaderSettings(), this.model))
                                {
                                    var jsonResourceReader = await messageReader.CreateODataResourceReaderAsync();

                                    await DoReadAsync(
                                        jsonResourceReader,
                                        verifyResourceAction: (resource) =>
                                        {
                                            Assert.NotNull(resource);
                                            Assert.Equal("NS.Customer", resource.TypeName);
                                            Assert.Equal(3, resource.Properties.Count());
                                            var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                                            Assert.Equal("Id", properties[0].Name);
                                            Assert.Equal(1, properties[0].Value);
                                            Assert.Equal("Name", properties[1].Name);
                                            Assert.Equal("Customer 1", properties[1].Value);
                                            Assert.Equal("Type", properties[2].Name);
                                            var customerTypeEnumValue = Assert.IsType<ODataEnumValue>(properties[2].Value);
                                            Assert.Equal("Retail", customerTypeEnumValue.Value);
                                        });
                                }

                                break;
                            default:
                                break;
                        }
                    }
                },
                isResponse: false);
        }

        [Fact]
        public async Task ReadBatchRequestMissingMessageBodyAsync()
        {
            var payload = "{\"requests\":[{\"id\":\"1\",\"method\":\"POST\",\"url\":\"http://tempuri.org/Customers\",\"headers\":{}}]}";

            await SetupJsonBatchReaderAndRunTestAsync(
                payload,
                async (jsonBatchReader) =>
                {
                    while (await jsonBatchReader.ReadAsync())
                    {
                        switch (jsonBatchReader.State)
                        {
                            case ODataBatchReaderState.Operation:
                                var operationRequestMessage = await jsonBatchReader.CreateOperationRequestMessageAsync();

                                Assert.Equal("1", operationRequestMessage.ContentId);
                                Assert.NotNull(operationRequestMessage.Url);
                                Assert.Equal("http://tempuri.org/Customers", operationRequestMessage.Url.AbsoluteUri);
                                Assert.Equal("POST", operationRequestMessage.Method);

                                using (var messageReader = new ODataMessageReader(operationRequestMessage, new ODataMessageReaderSettings(), this.model))
                                {
                                }

                                break;
                            default:
                                break;
                        }
                    }
                },
                isResponse: false);
        }

        [Fact]
        public async Task ReadBatchRequestMissingODataTypeAnnotationInMessageBodyAsync()
        {
            var payload = "{\"requests\":[{" +
                "\"id\":\"1\"," +
                "\"method\":\"POST\"," +
                "\"url\":\"http://tempuri.org/Customers\"," +
                "\"headers\":{\"odata-version\":\"4.0\",\"content-type\":\"application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\"}, " +
                "\"body\" :{\"Id\":1,\"Name\":\"Customer 1\",\"Type\":\"Retail\"}}]}";

            await SetupJsonBatchReaderAndRunTestAsync(
                payload,
                async (jsonBatchReader) =>
                {
                    while (await jsonBatchReader.ReadAsync())
                    {
                        switch (jsonBatchReader.State)
                        {
                            case ODataBatchReaderState.Operation:
                                var operationRequestMessage = await jsonBatchReader.CreateOperationRequestMessageAsync();

                                Assert.Equal("1", operationRequestMessage.ContentId);
                                Assert.NotNull(operationRequestMessage.Url);
                                Assert.Equal("http://tempuri.org/Customers", operationRequestMessage.Url.AbsoluteUri);
                                Assert.Equal("POST", operationRequestMessage.Method);

                                using (var messageReader = new ODataMessageReader(operationRequestMessage, new ODataMessageReaderSettings(), this.model))
                                {
                                    var jsonResourceReader = await messageReader.CreateODataResourceReaderAsync(this.customerEntitySet, this.customerEntityType);

                                    await DoReadAsync(
                                        jsonResourceReader,
                                        verifyResourceAction: (resource) =>
                                        {
                                            Assert.NotNull(resource);
                                            Assert.Equal("NS.Customer", resource.TypeName);
                                            Assert.Equal(3, resource.Properties.Count());
                                            var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                                            Assert.Equal("Id", properties[0].Name);
                                            Assert.Equal(1, properties[0].Value);
                                            Assert.Equal("Name", properties[1].Name);
                                            Assert.Equal("Customer 1", properties[1].Value);
                                            Assert.Equal("Type", properties[2].Name);
                                            var customerTypeEnumValue = Assert.IsType<ODataEnumValue>(properties[2].Value);
                                            Assert.Equal("Retail", customerTypeEnumValue.Value);
                                        });
                                }

                                break;
                            default:
                                break;
                        }
                    }
                },
                isResponse: false);
        }

        [Theory]
        [InlineData("\"headers\":{\"odata-version\":\"4.0\",\"content-type\":\"text/plain\"},\"body\" :\"the lazy dog\"")]
        [InlineData("\"body\" :\"the lazy dog\",\"headers\":{\"odata-version\":\"4.0\",\"content-type\":\"text/plain\"}")]
        public async Task ReadBatchRequestWithPlainTextBodyAsync(string fragment)
        {
            var payload = "{\"requests\":[{" +
                "\"id\":\"1\"," +
                "\"method\":\"POST\"," +
                "\"url\":\"http://tempuri.org\"," +
                $"{fragment}" +
                "}]}";

            await SetupJsonBatchReaderAndRunTestAsync(
                payload,
                async (jsonBatchReader) =>
                {
                    while (await jsonBatchReader.ReadAsync())
                    {
                        switch (jsonBatchReader.State)
                        {
                            case ODataBatchReaderState.Operation:
                                var operationRequestMessage = await jsonBatchReader.CreateOperationRequestMessageAsync();

                                using (var messageReader = new ODataMessageReader(operationRequestMessage, new ODataMessageReaderSettings(), this.model))
                                {
                                    var pangram = await messageReader.ReadValueAsync(EdmCoreModel.Instance.GetString(true));

                                    Assert.Equal("\"the lazy dog\"", pangram);
                                }

                                break;
                            default:
                                break;
                        }
                    }
                },
                isResponse: false);
        }

        [Theory]
        [InlineData("\"headers\":{\"odata-version\":\"4.0\",\"content-type\":\"application/octet-stream\"},\"body\" :\"AQIDBAUGBwgJAA==\"")]
        [InlineData("\"body\" :\"AQIDBAUGBwgJAA==\",\"headers\":{\"odata-version\":\"4.0\",\"content-type\":\"application/octet-stream\"}")]
        public async Task ReadBatchRequestWithBinaryBodyAsync(string payloadFragment)
        {
            var payload = "{\"requests\":[{" +
                "\"id\":\"1\"," +
                "\"method\":\"POST\"," +
                "\"url\":\"http://tempuri.org\"," +
                $"{payloadFragment}" +
                "}]}";

            await SetupJsonBatchReaderAndRunTestAsync(
                payload,
                async (jsonBatchReader) =>
                {
                    while (await jsonBatchReader.ReadAsync())
                    {
                        switch (jsonBatchReader.State)
                        {
                            case ODataBatchReaderState.Operation:
                                var operationRequestMessage = await jsonBatchReader.CreateOperationRequestMessageAsync();

                                using (var messageReader = new ODataMessageReader(operationRequestMessage, new ODataMessageReaderSettings(), this.model))
                                {
                                    var binaryValue = await messageReader.ReadValueAsync(EdmCoreModel.Instance.GetBinary(true));

                                    Assert.Equal(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }, binaryValue);
                                }

                                break;
                            default:
                                break;
                        }
                    }
                },
                isResponse: false);
        }

        [Fact]
        public async Task ReadBatchRequestAsync_ThrowsExceptionForUnknownPropertyForMessageInBatch()
        {
            var payload = "{\"requests\":[{\"id\":\"1\",\"method\":\"POST\",\"url\":\"http://tempuri.org/Customers\",\"headers\":{},\"forbidden\":\"foobar\"}]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonBatchReaderAndRunTestAsync(
                    payload,
                    async (jsonBatchReader) =>
                    {
                        while (await jsonBatchReader.ReadAsync())
                        {
                        }
                    },
                    isResponse: false));

            Assert.Equal(
                Error.Format(SRResources.ODataJsonBatchPayloadItemPropertiesCache_UnknownPropertyForMessageInBatch, "FORBIDDEN"),
                exception.Message);
        }

        [Fact]
        public async Task ReadBatchAsync_ThrowsExceptionForJsonBatchTopLevelPropertyMissing()
        {
            var payload = "{\"unexpected\":[]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonBatchReaderAndRunTestAsync(
                    payload,
                    async (jsonBatchReader) =>
                    {
                        while (await jsonBatchReader.ReadAsync())
                        {
                        }
                    },
                    isResponse: false));

            Assert.Equal(
                SRResources.ODataBatchReader_JsonBatchTopLevelPropertyMissing,
                exception.Message);
        }

        [Fact]
        public async Task ReadBatchRequestAsync_ThrowsExceptionForRequestMessageNotCreatedForOperation()
        {
            var payload = "{\"requests\":[{" +
                "\"id\":\"1\"," +
                "\"method\":\"POST\"," +
                "\"url\":\"http://tempuri.org/Customers\"," +
                "\"headers\":{\"odata-version\":\"4.0\",\"content-type\":\"application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\"}, " +
                "\"body\" :{\"@odata.type\":\"#NS.Customer\",\"Id\":1,\"Name\":\"Customer 1\",\"Type\":\"Retail\"}}]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonBatchReaderAndRunTestAsync(
                    payload,
                    async (jsonBatchReader) =>
                    {
                        while (await jsonBatchReader.ReadAsync())
                        {
                            // No call to JsonBatchReader.CreateOperationRequestMessageAsync()
                        }
                    },
                    isResponse: false));

            Assert.Equal(
                SRResources.ODataBatchReader_NoMessageWasCreatedForOperation,
                exception.Message);
        }

        [Fact]
        public async Task ReadBatchRequestAsync_ThrowsExceptionForRepeatedContentId()
        {
            var payload = "{\"requests\":[" +
                "{\"id\":\"1\"," +
                "\"method\":\"POST\"," +
                "\"url\":\"http://tempuri.org/Customers\"," +
                "\"headers\":{\"odata-version\":\"4.0\",\"content-type\":\"application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\"}, " +
                "\"body\" :{\"@odata.type\":\"#NS.Customer\",\"Id\":1,\"Name\":\"Customer 1\",\"Type\":\"Retail\"}}," +
                "{\"id\":\"1\"," +
                "\"method\":\"POST\"," +
                "\"url\":\"http://tempuri.org/Orders\"," +
                "\"headers\":{\"odata-version\":\"4.0\",\"content-type\":\"application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\"}, " +
                "\"body\" :{\"@odata.type\":\"#NS.Order\",\"Id\":1,\"CustomerId\":1,\"Amount\":13}}]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonBatchReaderAndRunTestAsync(
                    payload,
                    async (jsonBatchReader) =>
                    {
                        while (await jsonBatchReader.ReadAsync())
                        {
                            switch (jsonBatchReader.State)
                            {
                                case ODataBatchReaderState.Operation:
                                    var operationRequestMessage = await jsonBatchReader.CreateOperationRequestMessageAsync();

                                    using (var messageReader = new ODataMessageReader(operationRequestMessage, new ODataMessageReaderSettings(), this.model))
                                    {
                                        var jsonResourceReader = await messageReader.CreateODataResourceReaderAsync();

                                        await DoReadAsync(jsonResourceReader);
                                    }

                                    break;
                                default:
                                    break;
                            }
                        }
                    },
                    isResponse: false));

            Assert.Equal(
                Error.Format(SRResources.ODataBatchReader_DuplicateContentIDsNotAllowed, "1"),
                exception.Message);
        }

        [Fact]
        public async Task ReadBatchRequestAsync_ThrowsExceptionForDependsOnIdNotFound()
        {
            var payload = "{\"requests\":[{" +
                "\"id\":\"1\"," +
                "\"dependsOn\":[\"0\"]," +
                "\"method\":\"POST\"," +
                "\"url\":\"http://tempuri.org/Customers\"," +
                "\"headers\":{\"odata-version\":\"4.0\",\"content-type\":\"application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\"}, " +
                "\"body\" :{\"@odata.type\":\"#NS.Customer\",\"Id\":1,\"Name\":\"Customer 1\",\"Type\":\"Retail\"}}]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonBatchReaderAndRunTestAsync(
                    payload,
                    async (jsonBatchReader) =>
                    {
                        while (await jsonBatchReader.ReadAsync())
                        {
                            switch (jsonBatchReader.State)
                            {
                                case ODataBatchReaderState.Operation:
                                    var operationRequestMessage = await jsonBatchReader.CreateOperationRequestMessageAsync();

                                    break;
                                default:
                                    break;
                            }
                        }
                    },
                    isResponse: false));

            Assert.Equal(
                Error.Format(SRResources.ODataBatchReader_DependsOnIdNotFound, "0", "1"),
                exception.Message);
        }

        [Fact]
        public async Task ReadBatchRequestAsync_ThrowsExceptionForDependsOnIdSameAsContentId()
        {
            var payload = "{\"requests\":[{" +
                "\"id\":\"1\"," +
                "\"dependsOn\":[\"1\"]," +
                "\"method\":\"POST\"," +
                "\"url\":\"http://tempuri.org/Customers\"," +
                "\"headers\":{\"odata-version\":\"4.0\",\"content-type\":\"application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\"}, " +
                "\"body\" :{\"@odata.type\":\"#NS.Customer\",\"Id\":1,\"Name\":\"Customer 1\",\"Type\":\"Retail\"}}]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonBatchReaderAndRunTestAsync(
                    payload,
                    async (jsonBatchReader) =>
                    {
                        while (await jsonBatchReader.ReadAsync())
                        {
                            switch (jsonBatchReader.State)
                            {
                                case ODataBatchReaderState.Operation:
                                    var operationRequestMessage = await jsonBatchReader.CreateOperationRequestMessageAsync();

                                    break;
                                default:
                                    break;
                            }
                        }
                    },
                    isResponse: false));

            Assert.Equal(
                Error.Format(SRResources.ODataBatchReader_SelfReferenceDependsOnRequestIdNotAllowed, "1", "1"),
                exception.Message);
        }

        [Theory]
        [InlineData(false, 0, 0, 3)]
        [InlineData(true, 0, 0, 3)]
        [InlineData(false, 1, 1, 3)]
        [InlineData(true, 1, 1, 3)]
        [InlineData(false, 3, 1, 3)]
        [InlineData(true, 3, 1, 3)]
        [InlineData(false, 0, 0, 0)]
        [InlineData(true, 0, 0, 0)]
        [InlineData(false, 0, 0, 1)]
        [InlineData(true, 0, 0, 1)]
        [InlineData(false, 0, 0, 1000)]
        [InlineData(true, 0, 0, 1000)]
        [InlineData(false, 1, 100, 1000)]
        [InlineData(true, 1, 100, 1000)]
        [InlineData(false, 3, 100, 0)]
        [InlineData(true, 3, 100, 0)]
        [InlineData(false, 3, 100, 1)]
        [InlineData(true, 3, 100, 1)]
        [InlineData(false, 3, 100, 1000)]
        [InlineData(true, 3, 100, 1000)]
        public async Task ReadBatchPreservesRepeatedTopLevelDependencies(bool asynchronous, int groupCount, int membersPerGroup, int dependencyCount)
        {
            var payload = new StringBuilder("""{"requests":[""");
            var expectedIds = new List<string>();
            for (int group = 0; group < groupCount; group++)
            {
                for (int member = 0; member < membersPerGroup; member++)
                {
                    string id = $"m{group}_{member}";
                    expectedIds.Add(id);
                    payload.Append($$$"""{"id":"{{{id}}}","atomicityGroup":"g{{{group}}}","method":"PATCH","url":"Customers(1)","headers":{}},""");
                }
            }

            expectedIds.Add("target");
            expectedIds.Add("dependent");
            payload.Append("""{"id":"target","method":"GET","url":"Customers","headers":{}},""");
            payload.Append("""{"id":"dependent","dependsOn":[""").AppendJoin(',', Enumerable.Repeat("\"target\"", dependencyCount)).Append("""],"method":"GET","url":"Orders","headers":{}}]}""");
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload.ToString()));
            var message = new InMemoryMessage { Stream = stream };
            message.SetHeader("Content-Type", "application/json");
            var settings = new ODataMessageReaderSettings { BaseUri = new Uri($"{ServiceUri}/") };
            using var messageReader = new ODataMessageReader((IODataRequestMessage)message, settings, this.model);
            var reader = asynchronous ? await messageReader.CreateODataBatchReaderAsync() : messageReader.CreateODataBatchReader();
            int operationCount = 0;

            while (asynchronous ? await reader.ReadAsync() : reader.Read())
            {
                if (reader.State != ODataBatchReaderState.Operation)
                {
                    continue;
                }

                var operation = asynchronous ? await reader.CreateOperationRequestMessageAsync() : reader.CreateOperationRequestMessage();
                Assert.Equal(expectedIds[operationCount], operation.ContentId);
                Assert.Equal(operationCount < groupCount * membersPerGroup ? $"g{operationCount / membersPerGroup}" : null, operation.GroupId);
                if (operation.ContentId == "dependent")
                {
                    Assert.Equal(Enumerable.Repeat("target", dependencyCount), operation.DependsOnIds);
                }
                else
                {
                    Assert.Empty(operation.DependsOnIds);
                }

                operationCount++;
            }

            Assert.Equal(expectedIds.Count, operationCount);
            Assert.Equal(ODataBatchReaderState.Completed, reader.State);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task ReadBatchPreservesRepeatedGroupExpansion(bool asynchronous)
        {
            const string payload = """{"requests":[{"id":"top","method":"GET","url":"Customers","headers":{}},{"id":"one","atomicityGroup":"g1","method":"PATCH","url":"Customers(1)","headers":{}},{"id":"two","atomicityGroup":"g1","method":"PATCH","url":"Customers(2)","headers":{}},{"id":"dependent","dependsOn":["g1","top","g1","top"],"method":"GET","url":"Orders","headers":{}}]}""";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
            var message = new InMemoryMessage { Stream = stream };
            message.SetHeader("Content-Type", "application/json");
            using var messageReader = new ODataMessageReader((IODataRequestMessage)message, new ODataMessageReaderSettings { BaseUri = new Uri($"{ServiceUri}/") }, this.model);
            var reader = asynchronous ? await messageReader.CreateODataBatchReaderAsync() : messageReader.CreateODataBatchReader();
            int operationCount = 0;

            while (asynchronous ? await reader.ReadAsync() : reader.Read())
            {
                if (reader.State == ODataBatchReaderState.Operation)
                {
                    var operation = asynchronous ? await reader.CreateOperationRequestMessageAsync() : reader.CreateOperationRequestMessage();
                    if (operation.ContentId == "dependent")
                    {
                        Assert.Equal(new[] { "one", "two", "top", "one", "two", "top" }, operation.DependsOnIds);
                    }

                    operationCount++;
                }
            }

            Assert.Equal(4, operationCount);
            Assert.Equal(ODataBatchReaderState.Completed, reader.State);
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(true, true)]
        public async Task ReadBatchRejectsDependencyOnMemberOfCompletedGroup(bool asynchronous, bool includeDependentId)
        {
            string dependentIdProperty = includeDependentId ? "\"id\":\"dependent\"," : "";
            string payload = $$$"""{"requests":[{"id":"member","atomicityGroup":"group","method":"PATCH","url":"Customers(1)","headers":{}},{{{{dependentIdProperty}}}"dependsOn":["member"],"method":"GET","url":"Orders","headers":{}}]}""";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
            var message = new InMemoryMessage { Stream = stream };
            message.SetHeader("Content-Type", "application/json");
            using var messageReader = new ODataMessageReader((IODataRequestMessage)message, new ODataMessageReaderSettings { BaseUri = new Uri($"{ServiceUri}/") }, this.model);
            var reader = asynchronous ? await messageReader.CreateODataBatchReaderAsync() : messageReader.CreateODataBatchReader();
            var exception = await Assert.ThrowsAsync<ODataException>(async () =>
            {
                while (asynchronous ? await reader.ReadAsync() : reader.Read())
                {
                    if (reader.State == ODataBatchReaderState.Operation)
                    {
                        if (asynchronous)
                        {
                            await reader.CreateOperationRequestMessageAsync();
                        }
                        else
                        {
                            reader.CreateOperationRequestMessage();
                        }
                    }
                }
            });

            Assert.Equal(Error.Format(SRResources.ODataBatchReader_DependsOnRequestIdIsPartOfAtomicityGroupNotAllowed, "member", "group"), exception.Message);
        }

        [Theory]
        [InlineData(false, 1)]
        [InlineData(true, 1)]
        [InlineData(false, 100)]
        [InlineData(true, 100)]
        [InlineData(false, 499)]
        [InlineData(true, 499)]
        public async Task ReadBatchAcceptsEarlierMembersOfSameGroup(bool asynchronous, int memberCount)
        {
            var payload = new StringBuilder("""{"requests":[""");
            var expected = new List<string>();
            for (int member = 0; member < memberCount; member++)
            {
                string id = $"r{member}";
                expected.Add(id);
                payload.Append($$$"""{"id":"{{{id}}}","atomicityGroup":"group","method":"PATCH","url":"Customers(1)","headers":{}},""");
            }

            payload.Append("{\"id\":\"dependent\",\"atomicityGroup\":\"group\",\"dependsOn\":[\"").AppendJoin("\",\"", expected).Append("\"],\"method\":\"PATCH\",\"url\":\"Customers(1)\",\"headers\":{}}]}");
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload.ToString()));
            var message = new InMemoryMessage { Stream = stream };
            message.SetHeader("Content-Type", "application/json");
            using var messageReader = new ODataMessageReader((IODataRequestMessage)message, new ODataMessageReaderSettings { BaseUri = new Uri($"{ServiceUri}/") }, this.model);
            var reader = asynchronous ? await messageReader.CreateODataBatchReaderAsync() : messageReader.CreateODataBatchReader();
            int operationCount = 0;

            while (asynchronous ? await reader.ReadAsync() : reader.Read())
            {
                if (reader.State == ODataBatchReaderState.Operation)
                {
                    var operation = asynchronous ? await reader.CreateOperationRequestMessageAsync() : reader.CreateOperationRequestMessage();
                    Assert.Equal(operationCount < memberCount ? expected[operationCount] : "dependent", operation.ContentId);
                    Assert.Equal("group", operation.GroupId);
                    if (operation.ContentId == "dependent")
                    {
                        Assert.Equal(expected, operation.DependsOnIds);
                    }
                    else
                    {
                        Assert.Empty(operation.DependsOnIds);
                    }

                    operationCount++;
                }
            }

            Assert.Equal(memberCount + 1, operationCount);
            Assert.Equal(ODataBatchReaderState.Completed, reader.State);
        }

        [Theory]
        [InlineData(false, "")]
        [InlineData(true, "")]
        [InlineData(false, "\"dependsOn\":[],")]
        [InlineData(true, "\"dependsOn\":[],")]
        public async Task ReadBatchAcceptsUnconfiguredDependencies(bool asynchronous, string dependsOnProperty)
        {
            string payload = $$$"""{"requests":[{"id":"one",{{{dependsOnProperty}}}"method":"GET","url":"Customers","headers":{}}]}""";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
            var message = new InMemoryMessage { Stream = stream };
            message.SetHeader("Content-Type", "application/json");
            using var messageReader = new ODataMessageReader((IODataRequestMessage)message, new ODataMessageReaderSettings { BaseUri = new Uri($"{ServiceUri}/") }, this.model);
            var reader = asynchronous ? await messageReader.CreateODataBatchReaderAsync() : messageReader.CreateODataBatchReader();

            int operationCount = 0;
            while (asynchronous ? await reader.ReadAsync() : reader.Read())
            {
                if (reader.State == ODataBatchReaderState.Operation)
                {
                    var operation = asynchronous ? await reader.CreateOperationRequestMessageAsync() : reader.CreateOperationRequestMessage();
                    Assert.Equal("one", operation.ContentId);
                    Assert.Empty(operation.DependsOnIds);
                    operationCount++;
                }
            }

            Assert.Equal(1, operationCount);
            Assert.Equal(ODataBatchReaderState.Completed, reader.State);
        }

        [Theory]
        [InlineData(false, "self")]
        [InlineData(true, "self")]
        [InlineData(false, "own-group")]
        [InlineData(true, "own-group")]
        [InlineData(false, "missing")]
        [InlineData(true, "missing")]
        [InlineData(false, "forward")]
        [InlineData(true, "forward")]
        [InlineData(false, "case")]
        [InlineData(true, "case")]
        [InlineData(false, "duplicate")]
        [InlineData(true, "duplicate")]
        [InlineData(false, "reopened")]
        [InlineData(true, "reopened")]
        [InlineData(false, "other-group")]
        [InlineData(true, "other-group")]
        public async Task ReadBatchPreservesDependencyValidationErrors(bool asynchronous, string scenario)
        {
            string dependentId = scenario == "duplicate" ? "target" : "dependent";
            string groupProperty = scenario == "own-group" || scenario == "other-group" ? "\"atomicityGroup\":\"g2\"," : scenario == "reopened" ? "\"atomicityGroup\":\"g1\"," : "";
            string dependency = scenario == "self" ? "dependent" : scenario == "own-group" ? "g2" : scenario == "case" ? "TARGET" : scenario == "other-group" ? "member" : scenario == "forward" ? "future" : "missing";
            string dependencies = scenario == "duplicate" || scenario == "reopened" ? "" : $"\"dependsOn\":[\"target\",\"{dependency}\",\"second-missing\"],";
            string payload = $$$"""{"requests":[{"id":"member","atomicityGroup":"g1","method":"PATCH","url":"Customers(1)","headers":{}},{"id":"target","method":"GET","url":"Customers","headers":{}},{"id":"{{{dependentId}}}",{{{groupProperty}}}{{{dependencies}}}"method":"PATCH","url":"Customers(1)","headers":{}},{"id":"future","method":"GET","url":"Customers","headers":{}}]}""";
            string expectedMessage;
            switch (scenario)
            {
                case "self":
                    expectedMessage = Error.Format(SRResources.ODataBatchReader_SelfReferenceDependsOnRequestIdNotAllowed, "dependent", "dependent");
                    break;
                case "own-group":
                    expectedMessage = Error.Format(SRResources.ODataBatchReader_SameRequestIdAsAtomicityGroupIdNotAllowed, "g2", "g2");
                    break;
                case "duplicate":
                    expectedMessage = Error.Format(SRResources.ODataBatchReader_DuplicateContentIDsNotAllowed, "target");
                    break;
                case "reopened":
                    expectedMessage = Error.Format(SRResources.ODataBatchReader_MessageIdPositionedIncorrectly, "dependent", "g1");
                    break;
                case "other-group":
                    expectedMessage = Error.Format(SRResources.ODataBatchReader_DependsOnRequestIdIsPartOfAtomicityGroupNotAllowed, "member", "g1");
                    break;
                default:
                    expectedMessage = Error.Format(SRResources.ODataBatchReader_DependsOnIdNotFound, dependency, "dependent");
                    break;
            }

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
            var message = new InMemoryMessage { Stream = stream };
            message.SetHeader("Content-Type", "application/json");
            using var messageReader = new ODataMessageReader((IODataRequestMessage)message, new ODataMessageReaderSettings { BaseUri = new Uri($"{ServiceUri}/") }, this.model);
            var reader = asynchronous ? await messageReader.CreateODataBatchReaderAsync() : messageReader.CreateODataBatchReader();
            var exception = await Assert.ThrowsAsync<ODataException>(async () =>
            {
                while (asynchronous ? await reader.ReadAsync() : reader.Read())
                {
                    if (reader.State == ODataBatchReaderState.Operation)
                    {
                        if (asynchronous)
                        {
                            await reader.CreateOperationRequestMessageAsync();
                        }
                        else
                        {
                            reader.CreateOperationRequestMessage();
                        }
                    }
                }
            });

            Assert.Equal(expectedMessage, exception.Message);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task ReadEmptyBatchPreservesExistingRejection(bool asynchronous)
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes("""{"requests":[]}"""));
            var message = new InMemoryMessage { Stream = stream };
            message.SetHeader("Content-Type", "application/json");
            using var messageReader = new ODataMessageReader((IODataRequestMessage)message, new ODataMessageReaderSettings(), this.model);
            var reader = asynchronous ? await messageReader.CreateODataBatchReaderAsync() : messageReader.CreateODataBatchReader();

            var exception = await Assert.ThrowsAsync<ODataException>(async () =>
            {
                while (asynchronous ? await reader.ReadAsync() : reader.Read())
                {
                }
            });

            Assert.Equal(Error.Format(SRResources.JsonReaderExtensions_UnexpectedNodeDetected, JsonNodeType.StartObject, JsonNodeType.EndArray), exception.Message);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task ReadBatchPreservesNullDependencyArrayRejection(bool asynchronous)
        {
            const string payload = """{"requests":[{"id":"one","dependsOn":null,"method":"GET","url":"Customers","headers":{}}]}""";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
            var message = new InMemoryMessage { Stream = stream };
            message.SetHeader("Content-Type", "application/json");
            using var messageReader = new ODataMessageReader((IODataRequestMessage)message, new ODataMessageReaderSettings { BaseUri = new Uri($"{ServiceUri}/") }, this.model);
            var reader = asynchronous ? await messageReader.CreateODataBatchReaderAsync() : messageReader.CreateODataBatchReader();
            var exception = await Assert.ThrowsAsync<ODataException>(async () =>
            {
                while (asynchronous ? await reader.ReadAsync() : reader.Read())
                {
                }
            });

            Assert.Equal(Error.Format(SRResources.JsonReaderExtensions_UnexpectedNodeDetected, JsonNodeType.StartArray, JsonNodeType.PrimitiveValue), exception.Message);
        }

        [Theory]
        [InlineData(false, "parts", false)]
        [InlineData(true, "parts", false)]
        [InlineData(false, "parts", true)]
        [InlineData(true, "parts", true)]
        [InlineData(false, "changeset", false)]
        [InlineData(true, "changeset", false)]
        [InlineData(false, "changeset", true)]
        [InlineData(true, "changeset", true)]
        [InlineData(false, "bytes", false)]
        [InlineData(true, "bytes", false)]
        [InlineData(false, "bytes", true)]
        [InlineData(true, "bytes", true)]
        public async Task ReadBatchPreservesConfiguredQuotaBoundaries(bool asynchronous, string quota, bool exceeded)
        {
            string groupProperty = quota == "changeset" ? "\"atomicityGroup\":\"group\"," : "";
            string payload = $$$"""{"requests":[{"id":"one",{{{groupProperty}}}"method":"PATCH","url":"Customers(1)","headers":{}},{"id":"two",{{{groupProperty}}}"dependsOn":["one"],"method":"PATCH","url":"Customers(1)","headers":{}}]}""";
            byte[] bytes = Encoding.UTF8.GetBytes(payload);
            using var stream = new MemoryStream(bytes);
            var message = new InMemoryMessage { Stream = stream };
            message.SetHeader("Content-Type", "application/json");
            var settings = new ODataMessageReaderSettings { BaseUri = new Uri($"{ServiceUri}/") };
            if (quota == "parts")
            {
                settings.MessageQuotas.MaxPartsPerBatch = exceeded ? 1 : 2;
            }
            else if (quota == "changeset")
            {
                settings.MessageQuotas.MaxOperationsPerChangeset = exceeded ? 1 : 2;
            }
            else
            {
                settings.MessageQuotas.MaxReceivedMessageSize = exceeded ? bytes.Length - 1 : bytes.Length;
            }

            int operationCount = 0;
            Exception exception = await Record.ExceptionAsync(async () =>
            {
                using var messageReader = new ODataMessageReader((IODataRequestMessage)message, settings, this.model);
                var reader = asynchronous ? await messageReader.CreateODataBatchReaderAsync() : messageReader.CreateODataBatchReader();
                while (asynchronous ? await reader.ReadAsync() : reader.Read())
                {
                    if (reader.State == ODataBatchReaderState.Operation)
                    {
                        var operation = asynchronous ? await reader.CreateOperationRequestMessageAsync() : reader.CreateOperationRequestMessage();
                        if (operation.ContentId == "two")
                        {
                            Assert.Equal(new[] { "one" }, operation.DependsOnIds);
                        }

                        operationCount++;
                    }
                }

                Assert.Equal(ODataBatchReaderState.Completed, reader.State);
            });

            if (exceeded)
            {
                var quotaException = Assert.IsType<ODataException>(exception);
                if (quota == "parts")
                {
                    Assert.Equal(Error.Format(SRResources.ODataBatchReader_MaxBatchSizeExceeded, 1), quotaException.Message);
                }
                else if (quota == "changeset")
                {
                    Assert.Equal(Error.Format(SRResources.ODataBatchReader_MaxChangeSetSizeExceeded, 1), quotaException.Message);
                }
                else
                {
                    Assert.Equal(Error.Format(SRResources.MessageStreamWrappingStream_ByteLimitExceeded, bytes.Length, bytes.Length - 1), quotaException.Message);
                }
            }
            else
            {
                Assert.Null(exception);
                Assert.Equal(2, operationCount);
            }
        }

#if !DEBUG
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task ReadBatchPreservesNullDependencyElementRejection(bool asynchronous)
        {
            const string payload = """{"requests":[{"id":"one","dependsOn":[null],"method":"GET","url":"Customers","headers":{}}]}""";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
            var message = new InMemoryMessage { Stream = stream };
            message.SetHeader("Content-Type", "application/json");
            using var messageReader = new ODataMessageReader((IODataRequestMessage)message, new ODataMessageReaderSettings { BaseUri = new Uri($"{ServiceUri}/") }, this.model);
            var reader = asynchronous ? await messageReader.CreateODataBatchReaderAsync() : messageReader.CreateODataBatchReader();

            await Assert.ThrowsAsync<NullReferenceException>(async () =>
            {
                while (asynchronous ? await reader.ReadAsync() : reader.Read())
                {
                    if (reader.State == ODataBatchReaderState.Operation)
                    {
                        if (asynchronous)
                        {
                            await reader.CreateOperationRequestMessageAsync();
                        }
                        else
                        {
                            reader.CreateOperationRequestMessage();
                        }
                    }
                }
            });
        }
#endif

        private async Task DoReadAsync(
            ODataReader jsonReader,
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

        /// <summary>
        /// Sets up an ODataJsonBatchReader, then runs the given test code synchronously
        /// </summary>
        private void SetupJsonBatchReaderAndRunTest(
            string payload,
            Action<ODataJsonBatchReader> func,
            bool isResponse = true)
        {
            using (var jsonInputContext = CreateJsonInputContext(payload, isAsync: false, isResponse: isResponse))
            {
                var jsonBatchReader = new ODataJsonBatchReader(jsonInputContext, synchronous: true);

                func(jsonBatchReader);
            }
        }

        /// <summary>
        /// Sets up an ODataJsonBatchReader, then runs the given test code asynchronously
        /// </summary>
        private async Task SetupJsonBatchReaderAndRunTestAsync(
            string payload,
            Func<ODataJsonBatchReader, Task> func,
            bool isResponse = true)
        {
            using (var jsonInputContext = CreateJsonInputContext(payload, isAsync: true, isResponse: isResponse))
            {
                var jsonBatchReader = new ODataJsonBatchReader(jsonInputContext, synchronous: false);

                await func(jsonBatchReader);
            }
        }

        private ODataJsonInputContext CreateJsonInputContext(string payload, bool isAsync = false, bool isResponse = true)
        {
            var messageInfo = new ODataMessageInfo
            {
                MediaType = this.mediaType,
                Encoding = Encoding.Default,
                IsResponse = isResponse,
                IsAsync = isAsync,
                Model = this.model
            };

            return new ODataJsonInputContext(new StringReader(payload), messageInfo, this.messageReaderSettings);
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
    }
}

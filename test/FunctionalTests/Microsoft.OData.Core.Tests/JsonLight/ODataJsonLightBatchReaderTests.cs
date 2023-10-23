//---------------------------------------------------------------------
// <copyright file="ODataJsonLightBatchReaderTests.cs" company="Microsoft">
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
using Microsoft.OData.JsonLight;
using Xunit;
using ErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Core.Tests.JsonLight
{
    public class ODataJsonLightBatchReaderTests
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

        public ODataJsonLightBatchReaderTests()
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

            await SetupJsonLightBatchReaderAndRunTestAsync(
                payload,
                async (jsonLightBatchReader) =>
                {
                    while (await jsonLightBatchReader.ReadAsync())
                    {
                        switch (jsonLightBatchReader.State)
                        {
                            case ODataBatchReaderState.Operation:
                                var operationRequestMessage = await jsonLightBatchReader.CreateOperationRequestMessageAsync();

                                Assert.Equal("1", operationRequestMessage.ContentId);
                                Assert.NotNull(operationRequestMessage.Url);
                                Assert.Equal("http://tempuri.org/Customers", operationRequestMessage.Url.AbsoluteUri);
                                Assert.Equal("POST", operationRequestMessage.Method);

                                using (var messageReader = new ODataMessageReader(operationRequestMessage, new ODataMessageReaderSettings(), this.model))
                                {
                                    var jsonLightResourceReader = await messageReader.CreateODataResourceReaderAsync();

                                    await DoReadAsync(
                                        jsonLightResourceReader,
                                        verifyResourceAction: (resource) =>
                                        {
                                            Assert.NotNull(resource);
                                            Assert.Equal("NS.Customer", resource.TypeName);
                                            var properties = resource.Properties.ToArray();
                                            Assert.Equal(3, properties.Length);
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

            await SetupJsonLightBatchReaderAndRunTestAsync(
                payload,
                async (jsonLightBatchReader) =>
                {
                    while (await jsonLightBatchReader.ReadAsync())
                    {
                        switch (jsonLightBatchReader.State)
                        {
                            case ODataBatchReaderState.Operation:
                                var operationRequestMessage = await jsonLightBatchReader.CreateOperationRequestMessageAsync();

                                Assert.Equal("1", operationRequestMessage.ContentId);
                                Assert.NotNull(operationRequestMessage.Url);
                                Assert.Equal("http://tempuri.org/Customers", operationRequestMessage.Url.AbsoluteUri);
                                Assert.Equal("POST", operationRequestMessage.Method);

                                using (var messageReader = new ODataMessageReader(operationRequestMessage, new ODataMessageReaderSettings(), this.model))
                                {
                                    var jsonLightResourceReader = await messageReader.CreateODataResourceReaderAsync();

                                    await DoReadAsync(
                                        jsonLightResourceReader,
                                        verifyResourceAction: (resource) =>
                                        {
                                            Assert.NotNull(resource);
                                            Assert.Equal("NS.Customer", resource.TypeName);
                                            var properties = resource.Properties.ToArray();
                                            Assert.Equal(3, properties.Length);
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
                                Assert.Equal("69028f2c-f57b-4850-89f0-b7e5e002d4bc", jsonLightBatchReader.CurrentGroupId);
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
                var properties = resource.Properties.ToArray();
                Assert.Equal(3, properties.Length);
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
                var properties = resource.Properties.ToArray();
                Assert.Equal(3, properties.Length);
                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Name", properties[1].Name);
                Assert.Equal("Customer 1", properties[1].Value);
                Assert.Equal("Type", properties[2].Name);
                var customerTypeEnumValue = Assert.IsType<ODataEnumValue>(properties[2].Value);
                Assert.Equal("Retail", customerTypeEnumValue.Value);
            });

            await SetupJsonLightBatchReaderAndRunTestAsync(
                payload,
                async (jsonLightBatchReader) =>
                {
                    while (await jsonLightBatchReader.ReadAsync())
                    {
                        switch (jsonLightBatchReader.State)
                        {
                            case ODataBatchReaderState.Operation:
                                var operationRequestMessage = await jsonLightBatchReader.CreateOperationRequestMessageAsync();

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
                                    var jsonLightResourceReader = await messageReader.CreateODataResourceReaderAsync();

                                    await DoReadAsync(
                                        jsonLightResourceReader,
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
                var properties = resource.Properties.ToArray();
                Assert.Equal(3, properties.Length);
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
                var properties = resource.Properties.ToArray();
                Assert.Equal(3, properties.Length);
                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Name", properties[1].Name);
                Assert.Equal("Customer 1", properties[1].Value);
                Assert.Equal("Type", properties[2].Name);
                var customerTypeEnumValue = Assert.IsType<ODataEnumValue>(properties[2].Value);
                Assert.Equal("Retail", customerTypeEnumValue.Value);
            });

            await SetupJsonLightBatchReaderAndRunTestAsync(
                payload,
                async (jsonLightBatchReader) =>
                {
                    while (await jsonLightBatchReader.ReadAsync())
                    {
                        switch (jsonLightBatchReader.State)
                        {
                            case ODataBatchReaderState.Operation:
                                var operationRequestMessage = await jsonLightBatchReader.CreateOperationRequestMessageAsync();

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
                                    var jsonLightResourceReader = await messageReader.CreateODataResourceReaderAsync();

                                    await DoReadAsync(
                                        jsonLightResourceReader,
                                        verifyResourceAction: (resource) =>
                                        {
                                            Assert.NotEmpty(verifyResourceStack);
                                            var innerVerifyResourceStack = verifyResourceStack.Pop();
                                            innerVerifyResourceStack(resource);
                                        });
                                }

                                break;
                            case ODataBatchReaderState.ChangesetStart:
                                Assert.Equal("69028f2c-f57b-4850-89f0-b7e5e002d4bc", jsonLightBatchReader.CurrentGroupId);
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

            SetupJsonLightBatchReaderAndRunTest(
                payload,
                (jsonLightBatchReader) =>
                {
                    try
                    {
                        while (jsonLightBatchReader.Read())
                        {
                            if (jsonLightBatchReader.State == ODataBatchReaderState.Operation)
                            {
                                var operationRequestMessage = jsonLightBatchReader.CreateOperationRequestMessage();
                                // Verify that the Property "null-header" exists and it's value is set to NULL
                                var nullHeaderProperty = operationRequestMessage.Headers.FirstOrDefault(p => p.Key == "null-header");
                                Assert.NotNull(nullHeaderProperty.Key);
                                Assert.Null(nullHeaderProperty.Value);
                            }
                        }
                    }
                    catch (NullReferenceException ex)
                    {
                        Assert.False(true, ex.Message);
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

            await SetupJsonLightBatchReaderAndRunTestAsync(
                payload,
                async (jsonLightBatchReader) =>
                {
                    try
                    {
                        while (await jsonLightBatchReader.ReadAsync())
                        {
                            if (jsonLightBatchReader.State == ODataBatchReaderState.Operation)
                            {
                                var operationRequestMessage = await jsonLightBatchReader.CreateOperationRequestMessageAsync();
                                // Verify that the Property "null-header" exists and it's value is set to NULL
                                var nullHeaderProperty = operationRequestMessage.Headers.FirstOrDefault(p => p.Key == "null-header");
                                Assert.NotNull(nullHeaderProperty.Key);
                                Assert.Null(nullHeaderProperty.Value);
                            }
                        }
                    }
                    catch (NullReferenceException ex)
                    {
                        Assert.False(true, ex.Message);
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
                () => SetupJsonLightBatchReaderAndRunTest(
                payload,
                (jsonLightBatchReader) =>
                {
                    while (jsonLightBatchReader.Read())
                    {
                        if (jsonLightBatchReader.State == ODataBatchReaderState.Operation)
                        {
                            // The json properties are just iterated through and have no purpose for this test case
                            jsonLightBatchReader.CreateOperationRequestMessage();
                        }
                    }
                    Assert.False(true, "The test failed, because the duplicate header has not thrown an ODataException");
                },
                isResponse: false));

            // Verify that the correct duplicate property has raised the ODataException
            Assert.Equal(Strings.ODataJsonLightBatchPayloadItemPropertiesCache_DuplicatePropertyForRequestInBatch("ATOMICITYGROUP"), exception.Message);
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
                () => SetupJsonLightBatchReaderAndRunTestAsync(
                payload,
                async (jsonLightBatchReader) =>
                {
                    while (await jsonLightBatchReader.ReadAsync())
                    {
                        if (jsonLightBatchReader.State == ODataBatchReaderState.Operation)
                        {
                            // The json properties are just iterated through and have no purpose for this test case
                            await jsonLightBatchReader.CreateOperationRequestMessageAsync();
                        }
                    }
                    Assert.False(true, "The test failed, because the duplicate header has not thrown an ODataException");
                },
                isResponse: false));

            // Verify that the correct duplicate property has raised the ODataException
            Assert.Equal(Strings.ODataJsonLightBatchPayloadItemPropertiesCache_DuplicatePropertyForRequestInBatch("ATOMICITYGROUP"), exception.Message);
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
                () => SetupJsonLightBatchReaderAndRunTest(
                payload,
                (jsonLightBatchReader) =>
                {
                    while (jsonLightBatchReader.Read())
                    {
                        if (jsonLightBatchReader.State == ODataBatchReaderState.Operation)
                        {
                            // The json properties are just iterated through and have no purpose for this test case
                            jsonLightBatchReader.CreateOperationRequestMessage();
                        }
                    }
                    Assert.False(true, "The test failed, because the duplicate header has thrown no ODataException");
                },
                isResponse: false));

            // Verify that the correct duplicate header has raised the ODataException
            Assert.Equal(Strings.ODataJsonLightBatchPayloadItemPropertiesCache_DuplicateHeaderForRequestInBatch("duplicate-header"), exception.Message);
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
                () => SetupJsonLightBatchReaderAndRunTestAsync(
                payload,
                async (jsonLightBatchReader) =>
                {
                        while (await jsonLightBatchReader.ReadAsync())
                        {
                            if (jsonLightBatchReader.State == ODataBatchReaderState.Operation)
                            {
                                // The json properties are just iterated through and have no purpose for this test case
                                await jsonLightBatchReader.CreateOperationRequestMessageAsync();
                            }
                        }
                        Assert.False(true, "The test failed, because the duplicate header has thrown no ODataException");
                },
                isResponse: false));

            // Verify that the correct duplicate header has raised the ODataException
            Assert.Equal(Strings.ODataJsonLightBatchPayloadItemPropertiesCache_DuplicateHeaderForRequestInBatch("duplicate-header"), exception.Message);
        }

        [Fact]
        public async Task ReadBatchResponseAsync()
        {
            var payload = "{\"responses\":[{" +
                "\"id\":\"1\"," +
                "\"status\":0," +
                "\"headers\":{\"odata-version\":\"4.0\",\"content-type\":\"application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8\"}, " +
                "\"body\" :{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\",\"Id\":1,\"Name\":\"Customer 1\",\"Type\":\"Retail\"}}]}";

            await SetupJsonLightBatchReaderAndRunTestAsync(
                payload,
                async (jsonLightBatchReader) =>
                {
                    while (await jsonLightBatchReader.ReadAsync())
                    {
                        switch (jsonLightBatchReader.State)
                        {
                            case ODataBatchReaderState.Operation:
                                var operationResponseMessage = await jsonLightBatchReader.CreateOperationResponseMessageAsync();

                                using (var messageReader = new ODataMessageReader(operationResponseMessage, new ODataMessageReaderSettings(), this.model))
                                {
                                    var jsonLightResourceReader = await messageReader.CreateODataResourceReaderAsync();

                                    await DoReadAsync(
                                        jsonLightResourceReader,
                                        verifyResourceAction: (resource) =>
                                        {
                                            Assert.NotNull(resource);
                                            Assert.Equal("NS.Customer", resource.TypeName);
                                            var properties = resource.Properties.ToArray();
                                            Assert.Equal(3, properties.Length);
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

            await SetupJsonLightBatchReaderAndRunTestAsync(
                payload,
                async (jsonLightBatchReader) =>
                {
                    while (await jsonLightBatchReader.ReadAsync())
                    {
                        switch (jsonLightBatchReader.State)
                        {
                            case ODataBatchReaderState.Operation:
                                var operationResponseMessage = await jsonLightBatchReader.CreateOperationResponseMessageAsync();

                                using (var messageReader = new ODataMessageReader(operationResponseMessage, new ODataMessageReaderSettings(), this.model))
                                {
                                    var jsonLightResourceReader = await messageReader.CreateODataResourceReaderAsync();

                                    await DoReadAsync(
                                        jsonLightResourceReader,
                                        verifyResourceAction: (resource) =>
                                        {
                                            Assert.NotNull(resource);
                                            Assert.Equal("NS.Customer", resource.TypeName);
                                            var properties = resource.Properties.ToArray();
                                            Assert.Equal(3, properties.Length);
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
                                Assert.Equal("69028f2c-f57b-4850-89f0-b7e5e002d4bc", jsonLightBatchReader.CurrentGroupId);
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

            await SetupJsonLightBatchReaderAndRunTestAsync(
                payload,
                async (jsonLightBatchReader) =>
                {
                    while (await jsonLightBatchReader.ReadAsync())
                    {
                        switch (jsonLightBatchReader.State)
                        {
                            case ODataBatchReaderState.Operation:
                                var operationRequestMessage = await jsonLightBatchReader.CreateOperationRequestMessageAsync();

                                Assert.Equal("1", operationRequestMessage.ContentId);
                                Assert.NotNull(operationRequestMessage.Url);
                                Assert.Equal("http://tempuri.org/Customers", operationRequestMessage.Url.AbsoluteUri);
                                Assert.Equal("POST", operationRequestMessage.Method);

                                using (var messageReader = new ODataMessageReader(operationRequestMessage, new ODataMessageReaderSettings(), this.model))
                                {
                                    var jsonLightResourceReader = await messageReader.CreateODataResourceReaderAsync();

                                    await DoReadAsync(
                                        jsonLightResourceReader,
                                        verifyResourceAction: (resource) =>
                                        {
                                            Assert.NotNull(resource);
                                            Assert.Equal("NS.Customer", resource.TypeName);
                                            var properties = resource.Properties.ToArray();
                                            Assert.Equal(3, properties.Length);
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

            await SetupJsonLightBatchReaderAndRunTestAsync(
                payload,
                async (jsonLightBatchReader) =>
                {
                    while (await jsonLightBatchReader.ReadAsync())
                    {
                        switch (jsonLightBatchReader.State)
                        {
                            case ODataBatchReaderState.Operation:
                                var operationRequestMessage = await jsonLightBatchReader.CreateOperationRequestMessageAsync();

                                Assert.Equal("1", operationRequestMessage.ContentId);
                                Assert.NotNull(operationRequestMessage.Url);
                                Assert.Equal("http://tempuri.org/Customers", operationRequestMessage.Url.AbsoluteUri);
                                Assert.Equal("POST", operationRequestMessage.Method);

                                using (var messageReader = new ODataMessageReader(operationRequestMessage, new ODataMessageReaderSettings(), this.model))
                                {
                                    var jsonLightResourceReader = await messageReader.CreateODataResourceReaderAsync();

                                    await DoReadAsync(
                                        jsonLightResourceReader,
                                        verifyResourceAction: (resource) =>
                                        {
                                            Assert.NotNull(resource);
                                            Assert.Equal("NS.Customer", resource.TypeName);
                                            var properties = resource.Properties.ToArray();
                                            Assert.Equal(3, properties.Length);
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

            await SetupJsonLightBatchReaderAndRunTestAsync(
                payload,
                async (jsonLightBatchReader) =>
                {
                    while (await jsonLightBatchReader.ReadAsync())
                    {
                        switch (jsonLightBatchReader.State)
                        {
                            case ODataBatchReaderState.Operation:
                                var operationRequestMessage = await jsonLightBatchReader.CreateOperationRequestMessageAsync();

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

            await SetupJsonLightBatchReaderAndRunTestAsync(
                payload,
                async (jsonLightBatchReader) =>
                {
                    while (await jsonLightBatchReader.ReadAsync())
                    {
                        switch (jsonLightBatchReader.State)
                        {
                            case ODataBatchReaderState.Operation:
                                var operationRequestMessage = await jsonLightBatchReader.CreateOperationRequestMessageAsync();

                                Assert.Equal("1", operationRequestMessage.ContentId);
                                Assert.NotNull(operationRequestMessage.Url);
                                Assert.Equal("http://tempuri.org/Customers", operationRequestMessage.Url.AbsoluteUri);
                                Assert.Equal("POST", operationRequestMessage.Method);

                                using (var messageReader = new ODataMessageReader(operationRequestMessage, new ODataMessageReaderSettings(), this.model))
                                {
                                    var jsonLightResourceReader = await messageReader.CreateODataResourceReaderAsync(this.customerEntitySet, this.customerEntityType);

                                    await DoReadAsync(
                                        jsonLightResourceReader,
                                        verifyResourceAction: (resource) =>
                                        {
                                            Assert.NotNull(resource);
                                            Assert.Equal("NS.Customer", resource.TypeName);
                                            var properties = resource.Properties.ToArray();
                                            Assert.Equal(3, properties.Length);
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

            await SetupJsonLightBatchReaderAndRunTestAsync(
                payload,
                async (jsonLightBatchReader) =>
                {
                    while (await jsonLightBatchReader.ReadAsync())
                    {
                        switch (jsonLightBatchReader.State)
                        {
                            case ODataBatchReaderState.Operation:
                                var operationRequestMessage = await jsonLightBatchReader.CreateOperationRequestMessageAsync();

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

            await SetupJsonLightBatchReaderAndRunTestAsync(
                payload,
                async (jsonLightBatchReader) =>
                {
                    while (await jsonLightBatchReader.ReadAsync())
                    {
                        switch (jsonLightBatchReader.State)
                        {
                            case ODataBatchReaderState.Operation:
                                var operationRequestMessage = await jsonLightBatchReader.CreateOperationRequestMessageAsync();

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
                () => SetupJsonLightBatchReaderAndRunTestAsync(
                    payload,
                    async (jsonLightBatchReader) =>
                    {
                        while (await jsonLightBatchReader.ReadAsync())
                        {
                        }
                    },
                    isResponse: false));

            Assert.Equal(
                ErrorStrings.ODataJsonLightBatchPayloadItemPropertiesCache_UnknownPropertyForMessageInBatch("FORBIDDEN"),
                exception.Message);
        }

        [Fact]
        public async Task ReadBatchAsync_ThrowsExceptionForJsonBatchTopLevelPropertyMissing()
        {
            var payload = "{\"unexpected\":[]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightBatchReaderAndRunTestAsync(
                    payload,
                    async (jsonLightBatchReader) =>
                    {
                        while (await jsonLightBatchReader.ReadAsync())
                        {
                        }
                    },
                    isResponse: false));

            Assert.Equal(
                ErrorStrings.ODataBatchReader_JsonBatchTopLevelPropertyMissing,
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
                () => SetupJsonLightBatchReaderAndRunTestAsync(
                    payload,
                    async (jsonLightBatchReader) =>
                    {
                        while (await jsonLightBatchReader.ReadAsync())
                        {
                            // No call to jsonLightBatchReader.CreateOperationRequestMessageAsync()
                        }
                    },
                    isResponse: false));

            Assert.Equal(
                ErrorStrings.ODataBatchReader_NoMessageWasCreatedForOperation,
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
                () => SetupJsonLightBatchReaderAndRunTestAsync(
                    payload,
                    async (jsonLightBatchReader) =>
                    {
                        while (await jsonLightBatchReader.ReadAsync())
                        {
                            switch (jsonLightBatchReader.State)
                            {
                                case ODataBatchReaderState.Operation:
                                    var operationRequestMessage = await jsonLightBatchReader.CreateOperationRequestMessageAsync();

                                    using (var messageReader = new ODataMessageReader(operationRequestMessage, new ODataMessageReaderSettings(), this.model))
                                    {
                                        var jsonLightResourceReader = await messageReader.CreateODataResourceReaderAsync();

                                        await DoReadAsync(jsonLightResourceReader);
                                    }

                                    break;
                                default:
                                    break;
                            }
                        }
                    },
                    isResponse: false));

            Assert.Equal(
                ErrorStrings.ODataBatchReader_DuplicateContentIDsNotAllowed("1"),
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
                () => SetupJsonLightBatchReaderAndRunTestAsync(
                    payload,
                    async (jsonLightBatchReader) =>
                    {
                        while (await jsonLightBatchReader.ReadAsync())
                        {
                            switch (jsonLightBatchReader.State)
                            {
                                case ODataBatchReaderState.Operation:
                                    var operationRequestMessage = await jsonLightBatchReader.CreateOperationRequestMessageAsync();

                                    break;
                                default:
                                    break;
                            }
                        }
                    },
                    isResponse: false));

            Assert.Equal(
                ErrorStrings.ODataBatchReader_DependsOnIdNotFound("0", "1"),
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
                () => SetupJsonLightBatchReaderAndRunTestAsync(
                    payload,
                    async (jsonLightBatchReader) =>
                    {
                        while (await jsonLightBatchReader.ReadAsync())
                        {
                            switch (jsonLightBatchReader.State)
                            {
                                case ODataBatchReaderState.Operation:
                                    var operationRequestMessage = await jsonLightBatchReader.CreateOperationRequestMessageAsync();

                                    break;
                                default:
                                    break;
                            }
                        }
                    },
                    isResponse: false));

            Assert.Equal(
                ErrorStrings.ODataBatchReader_SelfReferenceDependsOnRequestIdNotAllowed("1", "1"),
                exception.Message);
        }

        private async Task DoReadAsync(
            ODataReader jsonLightReader,
            Action<ODataResourceSet> verifyResourceSetAction = null,
            Action<ODataResource> verifyResourceAction = null)
        {
            while (await jsonLightReader.ReadAsync())
            {
                switch (jsonLightReader.State)
                {
                    case ODataReaderState.ResourceSetStart:
                        break;
                    case ODataReaderState.ResourceSetEnd:
                        if (verifyResourceSetAction != null)
                        {
                            verifyResourceSetAction(jsonLightReader.Item as ODataResourceSet);
                        }

                        break;
                    case ODataReaderState.ResourceStart:
                        break;
                    case ODataReaderState.ResourceEnd:
                        if (verifyResourceAction != null)
                        {
                            verifyResourceAction(jsonLightReader.Item as ODataResource);
                        }

                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Sets up an ODataJsonLightBatchReader, then runs the given test code synchronously
        /// </summary>
        private void SetupJsonLightBatchReaderAndRunTest(
            string payload,
            Action<ODataJsonLightBatchReader> func,
            bool isResponse = true)
        {
            using (var jsonLightInputContext = CreateJsonLightInputContext(payload, isAsync: false, isResponse: isResponse))
            {
                var jsonLightBatchReader = new ODataJsonLightBatchReader(jsonLightInputContext, synchronous: true);

                func(jsonLightBatchReader);
            }
        }

        /// <summary>
        /// Sets up an ODataJsonLightBatchReader, then runs the given test code asynchronously
        /// </summary>
        private async Task SetupJsonLightBatchReaderAndRunTestAsync(
            string payload,
            Func<ODataJsonLightBatchReader, Task> func,
            bool isResponse = true)
        {
            using (var jsonLightInputContext = CreateJsonLightInputContext(payload, isAsync: true, isResponse: isResponse))
            {
                var jsonLightBatchReader = new ODataJsonLightBatchReader(jsonLightInputContext, synchronous: false);

                await func(jsonLightBatchReader);
            }
        }

        private ODataJsonLightInputContext CreateJsonLightInputContext(string payload, bool isAsync = false, bool isResponse = true)
        {
            var messageInfo = new ODataMessageInfo
            {
                MediaType = this.mediaType,
#if NETCOREAPP1_1
                Encoding = Encoding.GetEncoding(0),
#else
                Encoding = Encoding.Default,
#endif
                IsResponse = isResponse,
                IsAsync = isAsync,
                Model = this.model
            };

            return new ODataJsonLightInputContext(new StringReader(payload), messageInfo, this.messageReaderSettings);
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

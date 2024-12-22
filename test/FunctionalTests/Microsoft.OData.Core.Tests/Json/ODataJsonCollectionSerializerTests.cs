//---------------------------------------------------------------------
// <copyright file="ODataJsonCollectionSerializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    /// <summary>
    /// Unit tests for ODataJsonCollectionSerializer.
    /// </summary>
    public class ODataJsonCollectionSerializerTests
    {
        private EdmModel model;
        private Stream stream;
        private ODataMessageWriterSettings settings;
        private EdmEntityType entityType;

        public ODataJsonCollectionSerializerTests()
        {
            this.model = new EdmModel();

            this.entityType = new EdmEntityType("NS", "Product");
            this.entityType.AddKeys(this.entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            this.entityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);

            this.model.AddElement(this.entityType);

            this.stream = new MemoryStream();
            this.settings = new ODataMessageWriterSettings
            {
                EnableMessageStreamDisposal = false,
                Version = ODataVersion.V4
            };
            this.settings.SetServiceDocumentUri(new Uri("http://tempuri.org"));
        }

        [Fact]
        public async Task WriteCollectionStartAsync_ForNonTopLevelCollection()
        {
            var collectionStart = new ODataCollectionStart
            {
                SerializationInfo = new ODataCollectionStartSerializationInfo
                {
                    CollectionTypeName = "Collection(String)"
                }
            };

            var itemTypeReference = EdmCoreModel.Instance.GetString(false);

            var result = await SetupODataJsonCollectionSerializerAndRunTestAsync(
                (jsonCollectionSerializer) =>
                {
                    return jsonCollectionSerializer.WriteCollectionStartAsync(collectionStart, itemTypeReference);
                });

            Assert.Equal("[", result);
        }

        [Fact]
        public async Task WriteCollectionEndAsync_ForNonTopLevelCollection()
        {
            var collectionStart = new ODataCollectionStart
            {
                SerializationInfo = new ODataCollectionStartSerializationInfo
                {
                    CollectionTypeName = "Collection(String)"
                }
            };

            var itemTypeReference = EdmCoreModel.Instance.GetString(false);

            var result = await SetupODataJsonCollectionSerializerAndRunTestAsync(
                async (jsonCollectionSerializer) =>
                {
                    await jsonCollectionSerializer.WriteCollectionStartAsync(collectionStart, itemTypeReference);
                    await jsonCollectionSerializer.WriteCollectionEndAsync();
                });

            Assert.Equal("[]", result);
        }

        [Fact]
        public async Task WriteCollectionStartAsync_ForTopLevelCollection()
        {
            var collectionStart = new ODataCollectionStart
            {
                SerializationInfo = new ODataCollectionStartSerializationInfo
                {
                    CollectionTypeName = "Collection(Product)"
                }
            };

            var itemTypeReference = new EdmEntityTypeReference(this.entityType, false);

            var result = await SetupODataJsonCollectionSerializerAndRunTestAsync(
                (jsonCollectionSerializer) =>
                {
                    return jsonCollectionSerializer.WriteCollectionStartAsync(collectionStart, itemTypeReference);
                },
                /* container */ null,
                /* writeTopLevelCollection */ true);

            Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Collection(Product)\",\"value\":[", result);
        }

        [Fact]
        public async Task WriteCollectionStartAsync_ForTopLevelCollectionWithNextPageLink()
        {
            var collectionStart = new ODataCollectionStart
            {
                SerializationInfo = new ODataCollectionStartSerializationInfo
                {
                    CollectionTypeName = "Collection(Product)"
                },
                NextPageLink = new Uri("http://tempuri.org/Products?$skiptoken=Id-5")
            };

            var itemTypeReference = new EdmEntityTypeReference(this.entityType, false);

            var result = await SetupODataJsonCollectionSerializerAndRunTestAsync(
                (jsonCollectionSerializer) =>
                {
                    return jsonCollectionSerializer.WriteCollectionStartAsync(collectionStart, itemTypeReference);
                },
                /* container */ null,
                /* writeTopLevelCollection */ true);

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Collection(Product)\"," +
                "\"@odata.nextLink\":\"http://tempuri.org/Products?$skiptoken=Id-5\"," +
                "\"value\":[",
                result);
        }

        [Fact]
        public async Task WriteCollectionStartAsync_ForTopLevelCollectionWithItemCount()
        {
            var collectionStart = new ODataCollectionStart
            {
                SerializationInfo = new ODataCollectionStartSerializationInfo
                {
                    CollectionTypeName = "Collection(Product)"
                },
                Count = 10
            };

            var itemTypeReference = new EdmEntityTypeReference(this.entityType, false);

            var result = await SetupODataJsonCollectionSerializerAndRunTestAsync(
                (jsonCollectionSerializer) =>
                {
                    return jsonCollectionSerializer.WriteCollectionStartAsync(collectionStart, itemTypeReference);
                },
                /* container */ null,
                /* writeTopLevelCollection */ true);

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Collection(Product)\"," +
                "\"@odata.count\":10," +
                "\"value\":[",
                result);
        }

        [Fact]
        public async Task WriteCollectionEndAsync_ForTopLevelCollection()
        {
            var collectionStart = new ODataCollectionStart
            {
                Name = "Products",
                SerializationInfo = new ODataCollectionStartSerializationInfo
                {
                    CollectionTypeName = "Collection(Product)"
                },
                NextPageLink = new Uri("http://tempuri.org/Products?$skiptoken=Id-5"),
                Count = 10
            };

            var itemTypeReference = new EdmEntityTypeReference(this.entityType, false);

            var result = await SetupODataJsonCollectionSerializerAndRunTestAsync(
                async (jsonCollectionSerializer) =>
                {
                    await jsonCollectionSerializer.WriteCollectionStartAsync(collectionStart, itemTypeReference);
                    await jsonCollectionSerializer.WriteCollectionEndAsync();
                },
                /* container */ null,
                /* writeTopLevelCollection */ true);

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Collection(Product)\"," +
                "\"@odata.count\":10," +
                "\"@odata.nextLink\":\"http://tempuri.org/Products?$skiptoken=Id-5\"," +
                "\"value\":[]}",
                result);
        }

        private ODataJsonCollectionSerializer CreateODataJsonCollectionSerializer(bool writingResponse, IServiceProvider serviceProvider = null, bool isAsync = false, bool writingTopLevelCollection = false)
        {
            var messageInfo = new ODataMessageInfo
            {
                MessageStream = this.stream,
                MediaType = new ODataMediaType("application", "json"),
                Encoding = Encoding.Default,
                IsResponse = writingResponse,
                IsAsync = isAsync,
                Model = model,
                ServiceProvider = serviceProvider
            };
            var context = new ODataJsonOutputContext(messageInfo, this.settings);
            return new ODataJsonCollectionSerializer(context, writingTopLevelCollection);
        }

        /// <summary>
        /// Sets up an ODataJsonCollectionSerializer,
        /// then runs the given test code asynchronously,
        /// then flushes and reads the stream back as a string for customized verification.
        /// </summary>
        private async Task<string> SetupODataJsonCollectionSerializerAndRunTestAsync(Func<ODataJsonCollectionSerializer, Task> func, IServiceProvider container = null, bool writingTopLevelCollection = false)
        {
            this.stream = new AsyncStream(this.stream);
            var jsonCollectionSerializer = CreateODataJsonCollectionSerializer(true, container, true, writingTopLevelCollection);
            await func(jsonCollectionSerializer);
            await jsonCollectionSerializer.JsonOutputContext.FlushAsync();
            await jsonCollectionSerializer.JsonWriter.FlushAsync();

            this.stream.Position = 0;

            return await new StreamReader(this.stream).ReadToEndAsync();
        }
    }
}
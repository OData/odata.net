//---------------------------------------------------------------------
// <copyright file="ODataJsonLightCollectionWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.JsonLight;
using Xunit;

namespace Microsoft.OData.Tests.JsonLight
{
    public class ODataJsonLightCollectionWriterTests
    {
        private IEdmModel model = EdmCoreModel.Instance;

        [Fact]
        public void ShouldWriteDynamicNullableCollectionValuedProperty()
        {
            // setup model
            var model = new EdmModel();
            var entityType = new EdmEntityType("NS", "EntityType", null, false, true);
            entityType.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32);
            var container = new EdmEntityContainer("NS", "Container");
            var entitySet = container.AddEntitySet("EntitySet", entityType);
            var complexType = new EdmComplexType("NS", "ComplexType");
            complexType.AddStructuralProperty("Prop1", EdmPrimitiveTypeKind.Int32);
            complexType.AddStructuralProperty("Prop2", EdmPrimitiveTypeKind.Int32);
            model.AddElements(new IEdmSchemaElement[] { entityType, complexType, container });

            // setup writer
            var stream = new MemoryStream();
            var message = new InMemoryMessage { Stream = stream };
            var settings = new ODataMessageWriterSettings
            {
                ODataUri = new ODataUri
                {
                    ServiceRoot = new Uri("http://svc/")
                }
            };
            var writer = new ODataMessageWriter((IODataResponseMessage)message, settings, model)
                         .CreateODataResourceWriter(entitySet, entityType);

            // write payload
            writer.Write(new ODataResource
            {
                Properties = new[]
                {
                    new ODataProperty { Name = "ID", Value = 1 },
                    new ODataProperty
                    {
                        Name = "DynamicPrimitive",
                        Value = new ODataCollectionValue
                        {
                            TypeName = "Collection(Edm.Int64)",
                            Items = new object[] { 1L, 2L, null }
                        }
                    }
                }
            }, () => writer
                .Write(new ODataNestedResourceInfo
                {
                    Name = "DynamicComplex",
                    IsCollection = true,
                }, () => writer
                    .Write(new ODataResourceSet
                    {
                        TypeName = "Collection(NS.ComplexType)"
                    }, () => writer
                        .Write((ODataResource)null)
                        .Write((ODataResource)null)
                        .Write(new ODataResource
                        {
                            Properties = new[]
                            {
                                new ODataProperty { Name = "Prop1", Value = 1 },
                                new ODataProperty { Name = "Prop2", Value = 2 }
                            }
                        }))));
            var str = Encoding.UTF8.GetString(stream.ToArray());
            Assert.Equal(str,
                "{" +
                    "\"@odata.context\":\"http://svc/$metadata#EntitySet/$entity\"," +
                    "\"ID\":1," +
                    "\"DynamicPrimitive@odata.type\":\"#Collection(Int64)\"," +
                    "\"DynamicPrimitive\":[1,2,null]," +
                    "\"DynamicComplex@odata.type\":\"#Collection(NS.ComplexType)\"," +
                    "\"DynamicComplex\":[null,null,{\"Prop1\":1,\"Prop2\":2}]" +
                "}");
        }

        [Fact]
        public void ShouldWriteCollectionOfTypeDefinitionItemType()
        {
            ODataCollectionStart collectionStart = new ODataCollectionStart();
            WriteAndValidate(collectionStart, new object[] { 123 }, "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(NS.Test)\",\"value\":[123]}", true, new EdmTypeDefinitionReference(new EdmTypeDefinition("NS", "Test", EdmPrimitiveTypeKind.Int32), false));
        }

        [Fact]
        public void ShouldWriteCollectionOfResourceValueItem()
        {
            EdmModel currentModel = new EdmModel();
            EdmComplexType addressType = new EdmComplexType("ns", "Address");
            addressType.AddProperty(new EdmStructuralProperty(addressType, "Street", EdmCoreModel.Instance.GetString(isNullable: true)));
            currentModel.AddElement(addressType);
            this.model = currentModel;
            var address = new ODataResourceValue
            {
                TypeName = "ns.Address",
                Properties = new[] { new ODataProperty { Name = "Street", Value = "1 Microsoft Way" } }
            };

            WriteAndValidate(new ODataCollectionStart(),
                new object[] { address },
                "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(ns.Address)\",\"value\":[{\"Street\":\"1 Microsoft Way\"}]}",
                true,
                new EdmComplexTypeReference(addressType, false));
        }

        [Fact]
        public void ShouldWriteCollectionOfDerivedResourceValueItem()
        {
            EdmModel currentModel = new EdmModel();
            EdmComplexType addressType = new EdmComplexType("ns", "Address");
            addressType.AddProperty(new EdmStructuralProperty(addressType, "Street", EdmCoreModel.Instance.GetString(isNullable: true)));
            currentModel.AddElement(addressType);
            EdmComplexType homeAddressType = new EdmComplexType("ns", "HomeAddress", addressType);
            homeAddressType.AddProperty(new EdmStructuralProperty(homeAddressType, "City", EdmCoreModel.Instance.GetString(isNullable: true)));
            currentModel.AddElement(homeAddressType);
            this.model = currentModel;

            var address = new ODataResourceValue
            {
                TypeName = "ns.HomeAddress",
                Properties = new[]
                {
                    new ODataProperty { Name = "Street", Value = "1 Microsoft Way" },
                    new ODataProperty { Name = "City", Value = "Redmond" },
                }
            };

            WriteAndValidate(new ODataCollectionStart(),
                new object[] { address },
                "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(ns.Address)\",\"value\":[{\"@odata.type\":\"#ns.HomeAddress\",\"Street\":\"1 Microsoft Way\",\"City\":\"Redmond\"}]}",
                true,
                new EdmComplexTypeReference(addressType, false));
        }

        [Fact]
        public async Task WriteStartAsync_WritesCollectionStart()
        {
            var model = new EdmModel();
            var productEntityType = new EdmEntityType("NS", "Product");
            productEntityType.AddKeys(productEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            productEntityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);

            model.AddElement(productEntityType);

            var collectionStart = new ODataCollectionStart
            {
                SerializationInfo = new ODataCollectionStartSerializationInfo
                {
                    CollectionTypeName = "Collection(NS.Product)"
                }
            };

            var itemTypeReference = new EdmEntityTypeReference(productEntityType, true);

            var result = await SetupJsonLightCollectionWriterAndRunTestAsync(
                async (jsonLightCollectionWriter) =>
                {
                    await jsonLightCollectionWriter.WriteStartAsync(collectionStart);
                    await jsonLightCollectionWriter.FlushAsync();
                },
                model,
                itemTypeReference);

            Assert.Equal("{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(NS.Product)\"," +
                "\"value\":[", result);
        }

        [Fact]
        public async Task WriteItemAsync_WritesODataResourceValueCollectionItem()
        {
            var model = new EdmModel();
            var productEntityType = CreateProductEntityType();
            model.AddElement(productEntityType);

            var collectionStart = CreateProductCollectionStart();
            var itemTypeReference = new EdmEntityTypeReference(productEntityType, true);
            var odataResource = new ODataResourceValue
            {
                Properties = new List<ODataProperty>
                {
                    new ODataProperty { Name = "Id", Value = 1 },
                    new ODataProperty { Name = "Name", Value = "Pencil" }
                },
                TypeName = "NS.Product"
            };

            var result = await SetupJsonLightCollectionWriterAndRunTestAsync(
                async (jsonLightCollectionWriter) =>
                {
                    await jsonLightCollectionWriter.WriteStartAsync(collectionStart);
                    await jsonLightCollectionWriter.WriteItemAsync(odataResource);
                    await jsonLightCollectionWriter.FlushAsync();
                },
                model,
                itemTypeReference);

            Assert.Equal("{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(NS.Product)\"," +
                "\"value\":[{\"Id\":1,\"Name\":\"Pencil\"}", result);
        }

        [Fact]
        public async Task WriteItemAsync_WritesPrimitiveCollectionItem()
        {
            var model = new EdmModel();

            var collectionStart = CreateStringCollectionStart();
            var itemTypeReference = EdmCoreModel.Instance.GetString(true);

            var result = await SetupJsonLightCollectionWriterAndRunTestAsync(
                async (jsonLightCollectionWriter) =>
                {
                    await jsonLightCollectionWriter.WriteStartAsync(collectionStart);
                    await jsonLightCollectionWriter.WriteItemAsync("Foo");
                    await jsonLightCollectionWriter.WriteEndAsync();
                },
                model,
                itemTypeReference);

            Assert.Equal("{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(String)\"," +
                "\"value\":[\"Foo\"]}", result);
        }

        [Theory]
        [InlineData("Black", "\"Black\"")]
        [InlineData(null, "null")]
        public async Task WriteItemAsync_WritesEnumCollectionItem(string enumValue, string expected)
        {
            var model = new EdmModel();

            var colorEnumType = new EdmEnumType("NS", "Color");
            colorEnumType.AddMember(new EdmEnumMember(colorEnumType, "Black", new EdmEnumMemberValue(0)));
            colorEnumType.AddMember(new EdmEnumMember(colorEnumType, "White", new EdmEnumMemberValue(0)));
            model.AddElement(colorEnumType);

            var collectionStart = new ODataCollectionStart
            {
                SerializationInfo = new ODataCollectionStartSerializationInfo
                {
                    CollectionTypeName = "Collection(NS.Color)"
                }
            };
            var itemTypeReference = new EdmEnumTypeReference(colorEnumType, true);

            var result = await SetupJsonLightCollectionWriterAndRunTestAsync(
                async (jsonLightCollectionWriter) =>
                {
                    await jsonLightCollectionWriter.WriteStartAsync(collectionStart);
                    await jsonLightCollectionWriter.WriteItemAsync(new ODataEnumValue(enumValue));
                    await jsonLightCollectionWriter.WriteEndAsync();
                },
                model,
                itemTypeReference);

            Assert.Equal("{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(NS.Color)\"," +
                $"\"value\":[{expected}]}}", result);
        }

        [Fact]
        public async Task WriteEndAsync_WritesCollectionEnd()
        {
            var model = new EdmModel();
            var productEntityType = CreateProductEntityType();
            model.AddElement(productEntityType);

            var collectionStart = CreateProductCollectionStart();
            var itemTypeReference = new EdmEntityTypeReference(productEntityType, true);
            var odataResource1 = new ODataResourceValue
            {
                Properties = new List<ODataProperty>
                {
                    new ODataProperty { Name = "Id", Value = 1 },
                    new ODataProperty { Name = "Name", Value = "Pencil" }
                },
                TypeName = "NS.Product"
            };
            var odataResource2 = new ODataResourceValue
            {
                Properties = new List<ODataProperty>
                {
                    new ODataProperty { Name = "Id", Value = 2 },
                    new ODataProperty { Name = "Name", Value = "Paper" }
                },
                TypeName = "NS.Product"
            };

            var result = await SetupJsonLightCollectionWriterAndRunTestAsync(
                async (jsonLightCollectionWriter) =>
                {
                    await jsonLightCollectionWriter.WriteStartAsync(collectionStart);
                    await jsonLightCollectionWriter.WriteItemAsync(odataResource1);
                    await jsonLightCollectionWriter.WriteItemAsync(odataResource2);
                    await jsonLightCollectionWriter.WriteEndAsync();
                },
                model,
                itemTypeReference);

            Assert.Equal("{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(NS.Product)\"," +
                "\"value\":[{\"Id\":1,\"Name\":\"Pencil\"},{\"Id\":2,\"Name\":\"Paper\"}]}", result);
        }

        [Fact]
        public async Task WritesEmptyCollection()
        {
            var model = new EdmModel();
            var collectionStart = CreateStringCollectionStart();
            var itemTypeReference = EdmCoreModel.Instance.GetString(true);

            var result = await SetupJsonLightCollectionWriterAndRunTestAsync(
                async (jsonLightCollectionWriter) =>
                {
                    await jsonLightCollectionWriter.WriteStartAsync(collectionStart);
                    await jsonLightCollectionWriter.WriteEndAsync();
                },
                model,
                itemTypeReference);

            Assert.Equal("{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(String)\"," +
                "\"value\":[]}", result);
        }

        [Fact]
        public async Task WritesNullCollectionItem()
        {
            var model = new EdmModel();
            var collectionStart = CreateStringCollectionStart();
            var itemTypeReference = EdmCoreModel.Instance.GetString(true);

            var result = await SetupJsonLightCollectionWriterAndRunTestAsync(
                async (jsonLightCollectionWriter) =>
                {
                    await jsonLightCollectionWriter.WriteStartAsync(collectionStart);
                    await jsonLightCollectionWriter.WriteItemAsync(null);
                    await jsonLightCollectionWriter.WriteEndAsync();
                },
                model,
                itemTypeReference);

            Assert.Equal("{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(String)\"," +
                "\"value\":[null]}", result);
        }

        [Fact]
        public async Task WriteStartAsync_ThrowsExceptionForWriterInCompletedState()
        {
            var collectionStart = CreateStringCollectionStart();

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightCollectionWriterAndRunTestAsync(
                    async (jsonLightCollectionWriter) =>
                    {
                        await jsonLightCollectionWriter.WriteStartAsync(collectionStart);
                        await jsonLightCollectionWriter.WriteEndAsync();
                        await jsonLightCollectionWriter.WriteStartAsync(collectionStart);
                    },
                    new EdmModel(),
                    EdmCoreModel.Instance.GetString(true)));

            Assert.Equal(
                Strings.ODataWriterCore_InvalidTransitionFromCompleted("Completed", "Collection"),
                exception.Message);
        }

        [Fact]
        public async Task WriteStartAsync_ThrowsExceptionForWriterInCollectionState()
        {
            var collectionStart = CreateStringCollectionStart();

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightCollectionWriterAndRunTestAsync(
                    async (jsonLightCollectionWriter) =>
                    {
                        await jsonLightCollectionWriter.WriteStartAsync(collectionStart);
                        await jsonLightCollectionWriter.WriteStartAsync(collectionStart);
                    },
                    new EdmModel(),
                    EdmCoreModel.Instance.GetString(true)));

            Assert.Equal(
                Strings.ODataCollectionWriterCore_InvalidTransitionFromCollection("Collection", "Collection"),
                exception.Message);
        }

        [Fact]
        public async Task WriteStartAsync_ThrowsExceptionForWriterInItemState()
        {
            var collectionStart = CreateStringCollectionStart();

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightCollectionWriterAndRunTestAsync(
                    async (jsonLightCollectionWriter) =>
                    {
                        await jsonLightCollectionWriter.WriteStartAsync(collectionStart);
                        await jsonLightCollectionWriter.WriteItemAsync("Foo");
                        await jsonLightCollectionWriter.WriteStartAsync(collectionStart);
                    },
                    new EdmModel(),
                    EdmCoreModel.Instance.GetString(true)));

            Assert.Equal(
                Strings.ODataCollectionWriterCore_InvalidTransitionFromItem("Item", "Collection"),
                exception.Message);
        }

        [Fact]
        public async Task WriteItemAsync_ThrowsExceptionForWriterInCompletedState()
        {
            var collectionStart = CreateStringCollectionStart();

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightCollectionWriterAndRunTestAsync(
                    async (jsonLightCollectionWriter) =>
                    {
                        await jsonLightCollectionWriter.WriteStartAsync(collectionStart);
                        await jsonLightCollectionWriter.WriteEndAsync();
                        await jsonLightCollectionWriter.WriteItemAsync("Foo");
                    },
                    new EdmModel(),
                    EdmCoreModel.Instance.GetString(true)));

            Assert.Equal(
                Strings.ODataWriterCore_InvalidTransitionFromCompleted("Completed", "Item"),
                exception.Message);
        }

        [Fact]
        public async Task WriteItemAsync_ThrowsExceptionForWriterInStartState()
        {
            var collectionStart = CreateStringCollectionStart();

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightCollectionWriterAndRunTestAsync(
                    async (jsonLightCollectionWriter) =>
                    {
                        await jsonLightCollectionWriter.WriteItemAsync("Foo");
                    },
                    new EdmModel(),
                    EdmCoreModel.Instance.GetString(true)));

            Assert.Equal(
                Strings.ODataCollectionWriterCore_InvalidTransitionFromStart("Start", "Item"),
                exception.Message);
        }

        [Fact]
        public async Task WriteEndAsync_ThrowsExceptionForWriterInCompletedState()
        {
            var collectionStart = CreateStringCollectionStart();

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightCollectionWriterAndRunTestAsync(
                    async (jsonLightCollectionWriter) =>
                    {
                        await jsonLightCollectionWriter.WriteStartAsync(collectionStart);
                        await jsonLightCollectionWriter.WriteEndAsync();
                        await jsonLightCollectionWriter.WriteEndAsync();
                    },
                    new EdmModel(),
                    EdmCoreModel.Instance.GetString(true)));

            Assert.Equal(
                Strings.ODataCollectionWriterCore_WriteEndCalledInInvalidState("Completed"),
                exception.Message);
        }

        [Fact]
        public async Task WriteEndAsync_ThrowsExceptionForWriterInStartState()
        {
            var collectionStart = CreateStringCollectionStart();

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightCollectionWriterAndRunTestAsync(
                    async (jsonLightCollectionWriter) =>
                    {
                        await jsonLightCollectionWriter.WriteEndAsync();
                    },
                    new EdmModel(),
                    EdmCoreModel.Instance.GetString(true)));

            Assert.Equal(
                Strings.ODataCollectionWriterCore_WriteEndCalledInInvalidState("Start"),
                exception.Message);
        }

        private void WriteAndValidate(ODataCollectionStart collectionStart, IEnumerable<object> items, string expectedPayload, bool writingResponse = true, IEdmTypeReference itemTypeReference = null)
        {
            WriteAndValidateSync(itemTypeReference, collectionStart, items, expectedPayload, writingResponse);
        }

        private void WriteAndValidateSync(IEdmTypeReference itemTypeReference, ODataCollectionStart collectionStart, IEnumerable<object> items, string expectedPayload, bool writingResponse)
        {
            MemoryStream stream = new MemoryStream();
            var outputContext = CreateJsonLightOutputContext(stream, this.model, writingResponse, synchronous: true);
            var collectionWriter = new ODataJsonLightCollectionWriter(outputContext, itemTypeReference);
            collectionWriter.WriteStart(collectionStart);
            foreach (object item in items)
            {
                collectionWriter.WriteItem(item);
            }

            collectionWriter.WriteEnd();
            ValidateWrittenPayload(stream, expectedPayload);
        }

        private static void ValidateWrittenPayload(MemoryStream stream, string expectedPayload)
        {
            stream.Seek(0, SeekOrigin.Begin);
            string payload = (new StreamReader(stream)).ReadToEnd();
            Assert.Equal(payload, expectedPayload);
        }

        private static ODataJsonLightOutputContext CreateJsonLightOutputContext(MemoryStream stream, IEdmModel model, bool writingResponse = true, bool synchronous = true)
        {
            var messageInfo = new ODataMessageInfo
            {
                MessageStream = new NonDisposingStream(stream),
                MediaType = new ODataMediaType("application", "json"),
                Encoding = Encoding.UTF8,
                IsResponse = writingResponse,
                IsAsync = !synchronous,
                Model = model
            };

            var settings = new ODataMessageWriterSettings { Version = ODataVersion.V4 };
            settings.SetServiceDocumentUri(new Uri("http://odata.org/test/"));

            return new ODataJsonLightOutputContext(messageInfo, settings);
        }

        /// <summary>
        /// Sets up an ODataJsonLightCollectionSerializer,
        /// then runs the given test code asynchronously,
        /// then flushes and reads the stream back as a string for customized verification.
        /// </summary>
        private async Task<string> SetupJsonLightCollectionWriterAndRunTestAsync(
            Func<ODataJsonLightCollectionWriter, Task> func,
            IEdmModel model,
            IEdmTypeReference itemTypeReference)
        {
            var stream = new MemoryStream();
            var jsonLightOutputContext = CreateJsonLightOutputContext(stream, model, /* writingResponse */ true, /* synchronous */ false);
            var jsonLightCollectionWriter = new ODataJsonLightCollectionWriter(jsonLightOutputContext, itemTypeReference);
            await func(jsonLightCollectionWriter);

            stream.Position = 0;
            return await new StreamReader(stream).ReadToEndAsync();
        }

        private IEdmEntityType CreateProductEntityType()
        {
            var productEntityType = new EdmEntityType("NS", "Product");
            productEntityType.AddKeys(productEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            productEntityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);

            return productEntityType;
        }

        private ODataCollectionStart CreateProductCollectionStart()
        {
            return new ODataCollectionStart
            {
                SerializationInfo = new ODataCollectionStartSerializationInfo
                {
                    CollectionTypeName = "Collection(NS.Product)"
                }
            };
        }

        private ODataCollectionStart CreateStringCollectionStart()
        {
            return new ODataCollectionStart
            {
                SerializationInfo = new ODataCollectionStartSerializationInfo
                {
                    CollectionTypeName = "Collection(String)"
                }
            };
        }
    }
}

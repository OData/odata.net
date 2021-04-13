//---------------------------------------------------------------------
// <copyright file="ODataJsonLightValueSerializerAsyncTests.cs" company="Microsoft">
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
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.JsonLight
{
    /// <summary>
    /// Unit tests ODataJsonLightValueSerializer asynchronous API.
    /// </summary>
    public class ODataJsonLightValueSerializerAsyncTests
    {
        private EdmModel model;
        private EdmEnumType colorEnumType;
        private EdmComplexType attributesComplexType;
        private EdmEntityType entityType;
        private MemoryStream stream;
        private ODataMessageWriterSettings settings;

        public ODataJsonLightValueSerializerAsyncTests()
        {
            this.model = new EdmModel();

            this.colorEnumType = new EdmEnumType("NS", "Color");
            colorEnumType.AddMember(new EdmEnumMember(colorEnumType, "Black", new EdmEnumMemberValue(0)));
            colorEnumType.AddMember(new EdmEnumMember(colorEnumType, "White", new EdmEnumMemberValue(1)));
            this.model.AddElement(this.colorEnumType);

            attributesComplexType = new EdmComplexType("NS", "Attributes");
            attributesComplexType.AddStructuralProperty("LuckyNumber", EdmPrimitiveTypeKind.Int32);
            attributesComplexType.AddStructuralProperty("FavoriteColor", new EdmEnumTypeReference(colorEnumType, false));
            this.model.AddElement(this.attributesComplexType);

            EdmComplexTypeReference attributesComplexTypeRef = new EdmComplexTypeReference(attributesComplexType, true);

            this.entityType = new EdmEntityType("NS", "EntityType");
            entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            entityType.AddStructuralProperty("Attributes", new EdmCollectionTypeReference(new EdmCollectionType(attributesComplexTypeRef)));
            this.model.AddElement(this.entityType);

            this.stream = new MemoryStream();
            this.settings = new ODataMessageWriterSettings { EnableMessageStreamDisposal = false, Version = ODataVersion.V4 };
            this.settings.SetServiceDocumentUri(new Uri("http://tempuri.org"));
        }

        [Fact]
        public async Task WriteNullValueAsync_WritesNull()
        {
            var result = await SetupJsonLightValueSerializerAndRunTestAsync(
                (jsonLightValueSerializer) =>
                {
                    return jsonLightValueSerializer.WriteNullValueAsync();
                });

            Assert.Equal("null", result);
        }

        [Fact]
        public async Task WriteEnumValueAsync_WritesEnum()
        {
            var colorEnumValue = new ODataEnumValue("White", "NS.Color");
            var colorEdmEnumTypeReference = new EdmEnumTypeReference(this.colorEnumType, false);

            var result = await SetupJsonLightValueSerializerAndRunTestAsync(
                (jsonLightValueSerializer) =>
                {
                    return jsonLightValueSerializer.WriteEnumValueAsync(colorEnumValue, colorEdmEnumTypeReference);
                });

            Assert.Equal("\"White\"", result);
        }

        [Fact]
        public async Task WriteEnumValueAsync_WritesNullForEnumValueAsNull()
        {
            var colorEnumValue = new ODataEnumValue(null);
            var colorEdmEnumTypeReference = new EdmEnumTypeReference(this.colorEnumType, false);

            var result = await SetupJsonLightValueSerializerAndRunTestAsync(
                (jsonLightValueSerializer) =>
                {
                    return jsonLightValueSerializer.WriteEnumValueAsync(colorEnumValue, colorEdmEnumTypeReference);
                });

            Assert.Equal("null", result);
        }

        [Fact]
        public async Task WriteResourceValueAsync_WritesExpectedValue()
        {
            var resourceValue = new ODataResourceValue
            {
                Properties = new List<ODataProperty>
                {
                    new ODataProperty { Name = "LuckyNumber", Value = new ODataPrimitiveValue(13) },
                    new ODataProperty { Name = "FavoriteColor", Value = new ODataEnumValue("Black") }
                }
            };

            var result = await SetupJsonLightValueSerializerAndRunTestAsync(
                (jsonLightValueSerializer) =>
                {
                    return jsonLightValueSerializer.WriteResourceValueAsync(
                        resourceValue,
                        /* metadataTypeReference */ null,
                        /* isOpenProperty */ false,
                        new NullDuplicatePropertyNameChecker());
                });

            Assert.Equal("{\"LuckyNumber\":13,\"FavoriteColor\":\"Black\"}", result);
        }

        [Fact]
        public async Task WriteResourceValueAsync_WritesExpectedValueForOpenProperty()
        {
            var resourceValue = new ODataResourceValue
            {
                Properties = new List<ODataProperty>
                {
                    new ODataProperty { Name = "LuckyNumber", Value = new ODataPrimitiveValue(13) },
                    new ODataProperty { Name = "FavoriteColor", Value = new ODataEnumValue("Black") }
                },
                TypeName = "NS.Attributes"
            };

            var metadataTypeReference = new EdmComplexTypeReference(this.attributesComplexType, false);

            var result = await SetupJsonLightValueSerializerAndRunTestAsync(
                (jsonLightValueSerializer) =>
                {
                    return jsonLightValueSerializer.WriteResourceValueAsync(
                        resourceValue,
                        metadataTypeReference,
                        /* isOpenProperty */ true,
                        new NullDuplicatePropertyNameChecker());
                });

            Assert.Equal("{\"@odata.type\":\"#NS.Attributes\",\"LuckyNumber\":13,\"FavoriteColor\":\"Black\"}", result);
        }

        public static IEnumerable<object[]> GetWriteCollectionValueTestData()
        {
            yield return new object[]
            {
                new ODataCollectionValue
                {
                    Items = new List<ODataResourceValue>
                    {
                        new ODataResourceValue
                        {
                            Properties = new List<ODataProperty>
                            {
                                new ODataProperty { Name = "LuckyNumber", Value = new ODataPrimitiveValue(13) },
                                new ODataProperty { Name = "FavoriteColor", Value = new ODataEnumValue("Black") }
                            }
                        },
                        new ODataResourceValue
                        {
                            Properties = new List<ODataProperty>
                            {
                                new ODataProperty { Name = "LuckyNumber", Value = new ODataPrimitiveValue(7) },
                                new ODataProperty { Name = "FavoriteColor", Value = new ODataEnumValue("White") }
                            }
                        }
                    }
                },
                "[{\"LuckyNumber\":13,\"FavoriteColor\":\"Black\"},{\"LuckyNumber\":7,\"FavoriteColor\":\"White\"}]"
            };


            yield return new object[]
            {
                new ODataCollectionValue
                {
                    Items = new List<ODataEnumValue> { new ODataEnumValue("Black"), new ODataEnumValue("White") }
                },
                "[\"Black\",\"White\"]"
            };

            yield return new object[]
            {
                new ODataCollectionValue
                {
                    Items = new List<object> { 13.7m, 17.3m },
                    TypeName = "Collection(Edm.Decimal)"
                },
                "[13.7,17.3]"
            };

            yield return new object[]
            {
                new ODataCollectionValue
                {
                    Items = new List<ODataUntypedValue>
                    {
                        new ODataUntypedValue { RawValue = "2021-01-08T12:16:45+00:00" },
                        new ODataUntypedValue { RawValue = "2021-04-01T08:45:09+00:00" }
                    }
                },
                "[2021-01-08T12:16:45+00:00,2021-04-01T08:45:09+00:00]"
            };

            yield return new object[]
            {
                new ODataCollectionValue
                {
                    Items = new List<object>
                    {
                        null,
                        null
                    }
                },
                "[null,null]"
            };
        }

        [Theory]
        [MemberData(nameof(GetWriteCollectionValueTestData))]
        public async Task WriteCollectionValueAsync_WritesExpectedValue(ODataCollectionValue collectionValue, string expected)
        {
            var result = await SetupJsonLightValueSerializerAndRunTestAsync(
                (jsonLightValueSerializer) =>
                {
                    return jsonLightValueSerializer.WriteCollectionValueAsync(
                        collectionValue,
                        /* metadataTypeReference */ null,
                        /* valueTypeReference */ null,
                        /* isTopLevelProperty */ false,
                        /* isInUri */ false,
                        /* isOpenProperty */ false);
                });

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task WriteCollectionValueAsync_WithMetadataTypeReference_WritesExpectedValue()
        {
            var collectionValue = new ODataCollectionValue
            {
                Items = new List<ODataResourceValue>
                {
                    new ODataResourceValue
                    {
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "LuckyNumber", Value = new ODataPrimitiveValue(13) },
                            new ODataProperty { Name = "FavoriteColor", Value = new ODataEnumValue("Black") }
                        }
                    },
                    new ODataResourceValue
                    {
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "LuckyNumber", Value = new ODataPrimitiveValue(7) },
                            new ODataProperty { Name = "FavoriteColor", Value = new ODataEnumValue("White") }
                        }
                    }
                },
                TypeName = "Collection(NS.Attributes)"
            };

            var metadataTypeReference = new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(this.attributesComplexType, false)));

            var result = await SetupJsonLightValueSerializerAndRunTestAsync(
                (jsonLightValueSerializer) =>
                {
                    return jsonLightValueSerializer.WriteCollectionValueAsync(
                        collectionValue,
                        metadataTypeReference,
                        /* valueTypeReference */ null,
                        /* isTopLevelProperty */ false,
                        /* isInUri */ false,
                        /* isOpenProperty */ false);
                });

            Assert.Equal("[{\"LuckyNumber\":13,\"FavoriteColor\":\"Black\"},{\"LuckyNumber\":7,\"FavoriteColor\":\"White\"}]", result);
        }

        [Fact]
        public async Task WriteCollectionValueAsync_WritesExpectedValueForTopLevelProperty()
        {
            var collectionValue = new ODataCollectionValue
            {
                Items = new List<ODataEnumValue> { new ODataEnumValue("Black"), new ODataEnumValue("White") },
                TypeName = "Collection(NS.Color)" // Type name expected for top-level properties
            };

            var result = await SetupJsonLightValueSerializerAndRunTestAsync(
                (jsonLightValueSerializer) =>
                {
                    return jsonLightValueSerializer.WriteCollectionValueAsync(
                        collectionValue,
                        /* metadataTypeReference */ null, // No metadata type for top-level properties
                        /* valueTypeReference */ null,
                        /* isTopLevelProperty */ true,
                        /* isInUri */ false,
                        /* isOpenProperty */ false);
                });

            Assert.Equal("[\"Black\",\"White\"]", result);
        }

        [Fact]
        public async Task WriteCollectionValueAsync_WritesExpectedValueForUri()
        {
            var collectionValue = new ODataCollectionValue
            {
                Items = new List<object> { 7, 13 },
                TypeName = "Collection(Edm.Int32)"
            };

            var metadataTypeReference = EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(false));

            var result = await SetupJsonLightValueSerializerAndRunTestAsync(
                (jsonLightValueSerializer) =>
                {
                    return jsonLightValueSerializer.WriteCollectionValueAsync(
                        collectionValue,
                        metadataTypeReference,
                        /* valueTypeReference */ null,
                        /* isTopLevelProperty */ false,
                        /* isInUri */ true,
                        /* isOpenProperty */ true);
                });

            Assert.Equal("{\"@odata.type\":\"#Collection(Int32)\",\"value\":[7,13]}", result);
        }

        [Fact]
        public async Task WriteCollectionValueAsync_WritesExpectedValueForOpenProperty()
        {
            var collectionValue = new ODataCollectionValue
            {
                Items = new List<object> { "Foo", "Bar" },
                TypeName = "Collection(Edm.String)"
            };

            var result = await SetupJsonLightValueSerializerAndRunTestAsync(
                (jsonLightValueSerializer) =>
                {
                    return jsonLightValueSerializer.WriteCollectionValueAsync(
                        collectionValue,
                        /* metadataTypeReference */ null,
                        /* valueTypeReference */ null,
                        /* isTopLevelProperty */ false,
                        /* isInUri */ false,
                        /* isOpenProperty */ true);
                });

            Assert.Equal("[\"Foo\",\"Bar\"]", result);
        }

        [Fact]
        public async Task WriteCollectionValueAsync_ThrowsExceptionForOpenProperty_ForTypeNameNotSpecified()
        {
            var collectionValue = new ODataCollectionValue
            {
                Items = new List<object> { "Foo", "Bar" }
            };

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightValueSerializerAndRunTestAsync(
                    (jsonLightValueSerializer) =>
                    {
                        return jsonLightValueSerializer.WriteCollectionValueAsync(
                            collectionValue,
                            /* metadataTypeReference */ null,
                            /* valueTypeReference */ null,
                            /* isTopLevelProperty */ false,
                            /* isInUri */ false,
                            /* isOpenProperty */ true);
                    }));

            Assert.Equal(ODataErrorStrings.WriterValidationUtils_MissingTypeNameWithMetadata, exception.Message);
        }

        [Fact]
        public async Task WriteCollectionValueAsync_ThrowsExceptionForTopLevelProperty_ForTypeNameNotSpecified()
        {
            var collectionValue = new ODataCollectionValue
            {
                Items = new List<object> { "Foo", "Bar" }
            };

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightValueSerializerAndRunTestAsync(
                    (jsonLightValueSerializer) =>
                    {
                        return jsonLightValueSerializer.WriteCollectionValueAsync(
                            collectionValue,
                            /* metadataTypeReference */ null,
                            /* valueTypeReference */ null,
                            /* isTopLevelProperty */ true,
                            /* isInUri */ false,
                            /* isOpenProperty */ false);
                    }));

            Assert.Equal(ODataErrorStrings.ODataJsonLightValueSerializer_MissingTypeNameOnCollection, exception.Message);
        }

        [Fact]
        public async Task WritePrimitiveValueAsync_WritesExpectedValue()
        {
            var date = new Date(2014, 9, 17);
            var dateEdmTypeReference = EdmCoreModel.Instance.GetDate(false);

            var result = await SetupJsonLightValueSerializerAndRunTestAsync(
                (jsonLightValueSerializer) =>
                {
                    return jsonLightValueSerializer.WritePrimitiveValueAsync(date, dateEdmTypeReference);
                });

            Assert.Equal("\"2014-09-17\"", result);
        }

        [Fact]
        public async Task WriteUntypedValueAsync_ThrowsExceptionForRawValueNullOrEmpty()
        {
            var untypedValue = new ODataUntypedValue { RawValue = "" };

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => {
                    return SetupJsonLightValueSerializerAndRunTestAsync(
                        (jsonLightValueSerializer) =>
                        {
                            return jsonLightValueSerializer.WriteUntypedValueAsync(untypedValue);
                        });
                });

            Assert.Equal(ODataErrorStrings.ODataJsonLightValueSerializer_MissingRawValueOnUntyped, exception.Message);
        }

        [Fact]
        public async Task WriteUntypedValueAsync_WritesExpectedValue()
        {
            var untypedValue = new ODataUntypedValue { RawValue = "3.141592653589793238" };

            var result = await SetupJsonLightValueSerializerAndRunTestAsync(
                (jsonLightValueSerializer) =>
                {
                    return jsonLightValueSerializer.WriteUntypedValueAsync(untypedValue);
                });

            Assert.Equal("3.141592653589793238", result);
        }

        [Fact]
        public async Task WriteStreamValueAsync_WritesStreamValue()
        {
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            writer.Write("1234567890");
            writer.Flush();
            stream.Position = 0;
            var streamValue = new ODataBinaryStreamValue(stream);

            var result = await SetupJsonLightValueSerializerAndRunTestAsync(
                (jsonLightValueSerializer) =>
                {
                    return jsonLightValueSerializer.WriteStreamValueAsync(streamValue);
                });

            Assert.Equal("\"CjEyMzQ1Njc4OTA=\"", result);
        }

        private ODataJsonLightValueSerializer CreateODataJsonLightValueSerializer(bool writingResponse, IServiceProvider container = null, bool isAsync = false)
        {
            var messageInfo = new ODataMessageInfo
            {
                MessageStream = this.stream,
                MediaType = new ODataMediaType("application", "json"),
#if NETCOREAPP1_1
                Encoding = Encoding.GetEncoding(0),
#else
                Encoding = Encoding.Default,
#endif
                IsResponse = writingResponse,
                IsAsync = isAsync,
                Model = this.model,
                Container = container
            };
            var context = new ODataJsonLightOutputContext(messageInfo, this.settings);
            return new ODataJsonLightValueSerializer(context);
        }

        /// <summary>
        /// Sets up an ODataJsonLightValueSerializer,
        /// then runs the given test code asynchronously,
        /// then flushes and reads the stream back as a string for customized verification.
        /// </summary>
        private async Task<string> SetupJsonLightValueSerializerAndRunTestAsync(Func<ODataJsonLightValueSerializer, Task> func, IServiceProvider container = null)
        {
            var jsonLightValueSerializer = CreateODataJsonLightValueSerializer(true, container, true);
            await func(jsonLightValueSerializer);
            await jsonLightValueSerializer.JsonLightOutputContext.FlushAsync();
            await jsonLightValueSerializer.AsynchronousJsonWriter.FlushAsync();
            
            this.stream.Position = 0;
            
            return await new StreamReader(this.stream).ReadToEndAsync();
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="ODataJsonValueSerializerAsyncTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.Json
{
    /// <summary>
    /// Unit tests ODataJsonValueSerializer asynchronous API.
    /// </summary>
    public class ODataJsonValueSerializerAsyncTests
    {
        private EdmModel model;
        private EdmEnumType colorEnumType;
        private EdmComplexType attributesComplexType;
        private EdmEntityType entityType;
        private Stream stream;
        private ODataMessageWriterSettings settings;

        public ODataJsonValueSerializerAsyncTests()
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

            this.stream = new AsyncStream(new MemoryStream());
            this.settings = new ODataMessageWriterSettings { EnableMessageStreamDisposal = false, Version = ODataVersion.V4 };
            this.settings.SetServiceDocumentUri(new Uri("http://tempuri.org"));
        }

        [Fact]
        public async Task WriteNullValueAsync_WritesNull()
        {
            var result = await SetupJsonValueSerializerAndRunTestAsync(
                (jsonValueSerializer) =>
                {
                    return jsonValueSerializer.WriteNullValueAsync();
                });

            Assert.Equal("null", result);
        }

        [Fact]
        public async Task WriteEnumValueAsync_WritesEnum()
        {
            var colorEnumValue = new ODataEnumValue("White", "NS.Color");
            var colorEdmEnumTypeReference = new EdmEnumTypeReference(this.colorEnumType, false);

            var result = await SetupJsonValueSerializerAndRunTestAsync(
                (jsonValueSerializer) =>
                {
                    return jsonValueSerializer.WriteEnumValueAsync(colorEnumValue, colorEdmEnumTypeReference);
                });

            Assert.Equal("\"White\"", result);
        }

        [Fact]
        public async Task WriteEnumValueAsync_WritesNullForEnumValueAsNull()
        {
            var colorEnumValue = new ODataEnumValue(null);
            var colorEdmEnumTypeReference = new EdmEnumTypeReference(this.colorEnumType, false);

            var result = await SetupJsonValueSerializerAndRunTestAsync(
                (jsonValueSerializer) =>
                {
                    return jsonValueSerializer.WriteEnumValueAsync(colorEnumValue, colorEdmEnumTypeReference);
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

            var result = await SetupJsonValueSerializerAndRunTestAsync(
                (jsonValueSerializer) =>
                {
                    return jsonValueSerializer.WriteResourceValueAsync(
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

            var result = await SetupJsonValueSerializerAndRunTestAsync(
                (jsonValueSerializer) =>
                {
                    return jsonValueSerializer.WriteResourceValueAsync(
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
            var result = await SetupJsonValueSerializerAndRunTestAsync(
                (jsonValueSerializer) =>
                {
                    return jsonValueSerializer.WriteCollectionValueAsync(
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

            var result = await SetupJsonValueSerializerAndRunTestAsync(
                (jsonValueSerializer) =>
                {
                    return jsonValueSerializer.WriteCollectionValueAsync(
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

            var result = await SetupJsonValueSerializerAndRunTestAsync(
                (jsonValueSerializer) =>
                {
                    return jsonValueSerializer.WriteCollectionValueAsync(
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

            var result = await SetupJsonValueSerializerAndRunTestAsync(
                (jsonValueSerializer) =>
                {
                    return jsonValueSerializer.WriteCollectionValueAsync(
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

            var result = await SetupJsonValueSerializerAndRunTestAsync(
                (jsonValueSerializer) =>
                {
                    return jsonValueSerializer.WriteCollectionValueAsync(
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
                () => SetupJsonValueSerializerAndRunTestAsync(
                    (jsonValueSerializer) =>
                    {
                        return jsonValueSerializer.WriteCollectionValueAsync(
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
                () => SetupJsonValueSerializerAndRunTestAsync(
                    (jsonValueSerializer) =>
                    {
                        return jsonValueSerializer.WriteCollectionValueAsync(
                            collectionValue,
                            /* metadataTypeReference */ null,
                            /* valueTypeReference */ null,
                            /* isTopLevelProperty */ true,
                            /* isInUri */ false,
                            /* isOpenProperty */ false);
                    }));

            Assert.Equal(ODataErrorStrings.ODataJsonValueSerializer_MissingTypeNameOnCollection, exception.Message);
        }

        [Fact]
        public async Task WritePrimitiveValueAsync_WritesExpectedValue()
        {
            var date = new Date(2014, 9, 17);
            var dateEdmTypeReference = EdmCoreModel.Instance.GetDate(false);

            var result = await SetupJsonValueSerializerAndRunTestAsync(
                (jsonValueSerializer) =>
                {
                    return jsonValueSerializer.WritePrimitiveValueAsync(date, dateEdmTypeReference);
                });

            Assert.Equal("\"2014-09-17\"", result);
        }

        [Fact]
        public async Task WriteDateOnlyValueAsync_WritesExpectedValue()
        {
            var date = new DateOnly(2024, 9, 17);
            var dateEdmTypeReference = EdmCoreModel.Instance.GetDate(false);

            var result = await SetupJsonValueSerializerAndRunTestAsync(
                (jsonValueSerializer) =>
                {
                    return jsonValueSerializer.WritePrimitiveValueAsync(date, dateEdmTypeReference);
                });

            Assert.Equal("\"2024-09-17\"", result);
        }

        [Fact]
        public async Task WriteTimeOnlyValueAsync_WritesExpectedValue()
        {
            var timeOnly = new TimeOfDay(14, 9, 17, 2);
            var dateEdmTypeReference = EdmCoreModel.Instance.GetTimeOfDay(false);

            var result = await SetupJsonValueSerializerAndRunTestAsync(
                (jsonValueSerializer) =>
                {
                    return jsonValueSerializer.WritePrimitiveValueAsync(timeOnly, dateEdmTypeReference);
                });

            Assert.Equal("\"14:09:17.0020000\"", result);
        }

        [Fact]
        public async Task WriteUntypedValueAsync_ThrowsExceptionForRawValueNullOrEmpty()
        {
            var untypedValue = new ODataUntypedValue { RawValue = "" };

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => {
                    return SetupJsonValueSerializerAndRunTestAsync(
                        (jsonValueSerializer) =>
                        {
                            return jsonValueSerializer.WriteUntypedValueAsync(untypedValue);
                        });
                });

            Assert.Equal(ODataErrorStrings.ODataJsonValueSerializer_MissingRawValueOnUntyped, exception.Message);
        }

        [Fact]
        public async Task WriteUntypedValueAsync_WritesExpectedValue()
        {
            var untypedValue = new ODataUntypedValue { RawValue = "3.141592653589793238" };

            var result = await SetupJsonValueSerializerAndRunTestAsync(
                (jsonValueSerializer) =>
                {
                    return jsonValueSerializer.WriteUntypedValueAsync(untypedValue);
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

            var result = await SetupJsonValueSerializerAndRunTestAsync(
                (jsonValueSerializer) =>
                {
                    return jsonValueSerializer.WriteStreamValueAsync(streamValue);
                });

            Assert.Equal("\"CjEyMzQ1Njc4OTA=\"", result);
        }

        private ODataJsonValueSerializer CreateODataJsonValueSerializer(bool writingResponse, IServiceProvider serviceProvider = null, bool isAsync = false)
        {
            var messageInfo = new ODataMessageInfo
            {
                MessageStream = this.stream,
                MediaType = new ODataMediaType("application", "json"),
                Encoding = Encoding.Default,
                IsResponse = writingResponse,
                IsAsync = isAsync,
                Model = this.model,
                ServiceProvider = serviceProvider
            };
            var context = new ODataJsonOutputContext(messageInfo, this.settings);
            return new ODataJsonValueSerializer(context);
        }

        /// <summary>
        /// Sets up an ODataJsonValueSerializer,
        /// then runs the given test code asynchronously,
        /// then flushes and reads the stream back as a string for customized verification.
        /// </summary>
        private async Task<string> SetupJsonValueSerializerAndRunTestAsync(Func<ODataJsonValueSerializer, Task> func, IServiceProvider serviceProvider = null)
        {
            var jsonValueSerializer = CreateODataJsonValueSerializer(true, serviceProvider, true);
            await func(jsonValueSerializer);
            await jsonValueSerializer.JsonOutputContext.FlushAsync();
            await jsonValueSerializer.JsonWriter.FlushAsync();
            
            this.stream.Position = 0;
            
            return await new StreamReader(this.stream).ReadToEndAsync();
        }
    }
}

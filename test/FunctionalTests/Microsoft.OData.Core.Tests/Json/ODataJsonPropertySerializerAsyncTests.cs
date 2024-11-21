//---------------------------------------------------------------------
// <copyright file="ODataJsonPropertySerializerAsyncTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Core.Tests.DependencyInjection;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    public class ODataJsonPropertySerializerAsyncTests
    {
        private IEdmModel model;
        private IEdmEntityType entityType;
        private ODataProperty declaredPropertyWithInstanceAnnotations;
        private ODataProperty undeclaredPropertyWithInstanceAnnotations;
        private ODataPropertyInfo streamProperty;

        public ODataJsonPropertySerializerAsyncTests()
        {
            // Initialize open EntityType: EntityType.
            EdmModel edmModel = new EdmModel();

            EdmEntityType edmEntityType = new EdmEntityType("NS", "EntityType", baseType: null, isAbstract: false, isOpen: true);
            edmEntityType.AddStructuralProperty("DeclaredProperty", EdmPrimitiveTypeKind.Int32);

            edmModel.AddElement(edmEntityType);

            this.model = TestUtils.WrapReferencedModelsToMainModel(edmModel);
            this.entityType = edmEntityType;

            this.declaredPropertyWithInstanceAnnotations = new ODataProperty()
            {
                Name = "DeclaredProperty",
                Value = 12345,
                InstanceAnnotations = new Collection<ODataInstanceAnnotation>
                {
                    new ODataInstanceAnnotation("Is.Numeric", new ODataPrimitiveValue(true)),
                    new ODataInstanceAnnotation("Is.ReadOnly", new ODataPrimitiveValue(false))
                }
            };

            this.undeclaredPropertyWithInstanceAnnotations = new ODataProperty
            {
                Name = "UndeclaredProperty",
                Value = (long)13,
                TypeAnnotation = new ODataTypeAnnotation(EdmCoreModel.Instance.GetInt64(false).FullName()),
                InstanceAnnotations = new Collection<ODataInstanceAnnotation>
                {
                    new ODataInstanceAnnotation("Is.LuckyNumber", new ODataPrimitiveValue(true))
                }
            };

            this.streamProperty = new ODataStreamPropertyInfo
            {
                Name = "StreamProp",
                ContentType = "image/jpeg",
                EditLink = new Uri("http://tempuri.org/stream/edit"),
                ReadLink = new Uri("http://tempuri.org/stream/read"),
                ETag = "Stream Etag"
            };
        }

        [Theory]
        [InlineData(true, "{\"@odata.type\":\"#Int64\",\"@Is.LuckyNumber\":true")]
        [InlineData(false, "{\"UndeclaredProperty@odata.type\":\"#Int64\",\"UndeclaredProperty@Is.LuckyNumber\":true")]
        public async Task WritePropertyInfoAsync_WritesUndeclaredProperty(bool isTopLevel, string expected)
        {
            var result = await this.SetupSerializerAndRunTestAsync(
                async (jsonPropertySerializer) =>
                {
                    await jsonPropertySerializer.JsonWriter.StartObjectScopeAsync();
                    await jsonPropertySerializer.WritePropertyInfoAsync(
                        this.undeclaredPropertyWithInstanceAnnotations,
                        /* owningType */ null,
                        isTopLevel,
                        new NullDuplicatePropertyNameChecker(),
                        /* metadataBuilder */ null);
                });

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(true, "{\"@Is.Numeric\":true,\"@Is.ReadOnly\":false")]
        [InlineData(false, "{\"DeclaredProperty@Is.Numeric\":true,\"DeclaredProperty@Is.ReadOnly\":false")]
        public async Task WritePropertyInfoAsync_WritesDeclaredProperty(bool isTopLevel, string expected)
        {
            var result = await this.SetupSerializerAndRunTestAsync(
                async (jsonPropertySerializer) =>
                {
                    await jsonPropertySerializer.JsonWriter.StartObjectScopeAsync();
                    await jsonPropertySerializer.WritePropertyInfoAsync(
                        this.declaredPropertyWithInstanceAnnotations,
                        this.entityType,
                        isTopLevel,
                        new NullDuplicatePropertyNameChecker(),
                        /* metadataBuilder */ null);
                });

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task WritePropertyInfoAsync_WritesStreamProperty()
        {
            var result = await this.SetupSerializerAndRunTestAsync(
                async (jsonPropertySerializer) =>
                {
                    await jsonPropertySerializer.JsonWriter.StartObjectScopeAsync();
                    await jsonPropertySerializer.WritePropertyInfoAsync(
                        this.streamProperty,
                        /* owningType */ null,
                        /* isTopLevel */ false, // Stream properties are not allowed at the top level
                        new NullDuplicatePropertyNameChecker(),
                        /* metadataBuilder */ null);
                });

            Assert.Equal("{\"StreamProp@odata.mediaEditLink\":\"http://tempuri.org/stream/edit\"," +
                "\"StreamProp@odata.mediaReadLink\":\"http://tempuri.org/stream/read\"," +
                "\"StreamProp@odata.mediaContentType\":\"image/jpeg\"," +
                "\"StreamProp@odata.mediaEtag\":\"Stream Etag\"", result);
        }

        public static IEnumerable<object[]> GetWritePropertyTestData()
        {
            // ODataUntypedValue
            yield return new object[]
            {
                new ODataProperty { Name = "LuckyNumber", Value = new ODataUntypedValue { RawValue = "13" } },
                /* owningType */ null,
                "{\"LuckyNumber\":13"
            };

            // ODataNullValue
            yield return new object[]
            {
                new ODataProperty { Name = "LuckyNumber", Value = null },
                /* owningType */ null,
                "{\"LuckyNumber\":null"
            };

            // ODataPrimitiveValue
            yield return new object[]
            {
                new ODataProperty { Name = "LuckyNumber", Value = new ODataPrimitiveValue(13) },
                /* owningType */ null,
                "{\"LuckyNumber\":13"
            };

            // ODataEnumValue
            yield return new object[]
            {
                new ODataProperty { Name = "FavoriteColor", Value = new ODataEnumValue("Black", "NS.Color") },
                /* owningType */ null,
                "{\"FavoriteColor\":\"Black\""
            };

            // ODataResourceValue
            yield return new object[]
            {
                new ODataProperty
                {
                    Name = "Attributes",
                    Value = new ODataResourceValue
                    {
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "LuckyNumber", Value = new ODataPrimitiveValue(13) },
                            new ODataProperty { Name = "FavoriteColor", Value = new ODataEnumValue("Black") }
                        }
                    }
                },
                /* owningType */ null,
                "{\"Attributes\":{\"LuckyNumber\":13,\"FavoriteColor\":\"Black\"}"
            };

            // ODataCollectionValue
            yield return new object[]
            {
                new ODataProperty
                {
                    Name = "Colors",
                    Value = new ODataCollectionValue
                    {
                        Items = new List<ODataEnumValue>
                        {
                            new ODataEnumValue("Black"),
                            new ODataEnumValue("White"),
                        }
                    }
                },
                /* owningType */ null,
                "{\"Colors\":[\"Black\",\"White\"]"
            };

            // ODataStreamReferenceValue
            yield return new object[]
            {
                new ODataProperty
                {
                    Name = "StreamProp",
                    Value = new ODataStreamReferenceValue
                    {
                        ContentType = "image/jpeg",
                        EditLink = new Uri("http://tempuri.org/stream/edit"),
                        ReadLink = new Uri("http://tempuri.org/stream/read"),
                        ETag = "Stream Etag"
                    }
                },
                /* owningType */ null,
                "{\"StreamProp@odata.mediaEditLink\":\"http://tempuri.org/stream/edit\"," +
                "\"StreamProp@odata.mediaReadLink\":\"http://tempuri.org/stream/read\"," +
                "\"StreamProp@odata.mediaContentType\":\"image/jpeg\"," +
                "\"StreamProp@odata.mediaEtag\":\"Stream Etag\""
            };

            // ODataBinaryStreamValue
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            writer.Write("1234567890");
            writer.Flush();
            stream.Position = 0;

            yield return new object[]
            {
                new ODataProperty { Name = "BinaryAsStream", Value = new ODataBinaryStreamValue(stream) },
                /* owningType */ null,
                "{\"BinaryAsStream\":\"CjEyMzQ1Njc4OTA=\""
            };
        }

        [Theory]
        [MemberData(nameof(GetWritePropertyTestData))]
        public async Task WritePropertyAsync_WritesExpectedOutput(ODataProperty property, IEdmStructuredType owningType, string expected)
        {
            var result = await this.SetupSerializerAndRunTestAsync(
                async (jsonPropertySerializer) =>
                {
                    await jsonPropertySerializer.JsonWriter.StartObjectScopeAsync();
                    await jsonPropertySerializer.WritePropertyAsync(
                        property,
                        owningType,
                        /* isTopLevel */ false,
                        new NullDuplicatePropertyNameChecker(),
                        /* metadataBuilder */ null);
                });

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task WriteTopLevelPropertyAsync_WritesTopLevelProperty()
        {
            var result = await this.SetupSerializerAndRunTestAsync(
                (jsonPropertySerializer) =>
                {
                    return jsonPropertySerializer.WriteTopLevelPropertyAsync(
                        new ODataProperty { Name = "LuckyNumber", Value = new ODataPrimitiveValue(13) }
                    );
                });

            Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Edm.Int32\",\"value\":13}", result);
        }

        [Fact]
        public async Task WritingJsonElementPropertiesAsync_ShouldSerializeJsonInput()
        {
            string jsonInputString = "{\"foo\":\"bar\"}";
            var jsonDocument = System.Text.Json.JsonDocument.Parse(jsonInputString);

            var property = new ODataProperty()
            {
                Name = "JsonProp",
                Value = new ODataJsonElementValue(jsonDocument.RootElement)
            };

            var result = await this.SetupSerializerAndRunTestAsync(
                async (jsonPropertySerializer) =>
                {
                    await jsonPropertySerializer.JsonWriter.StartObjectScopeAsync();
                    await jsonPropertySerializer.WritePropertyAsync(
                        property,
                        owningType: null,
                        isTopLevel: false,
                        duplicatePropertyNameChecker: new DuplicatePropertyNameChecker(),
                        metadataBuilder: null);
                    await jsonPropertySerializer.JsonWriter.EndObjectScopeAsync();
                });

            Assert.Equal("{\"JsonProp\":{\"foo\":\"bar\"}}", result);
        }

        [Fact]
        public async Task WritingJsonElementPropertiesAsync_ShouldSerializeJsonInput_WithODataUtf8JsonWriter()
        {
            string jsonInputString = "{\"foo\":\"bar\"}";
            var jsonDocument = System.Text.Json.JsonDocument.Parse(jsonInputString);

            var property = new ODataProperty()
            {
                Name = "JsonProp",
                Value = new ODataJsonElementValue(jsonDocument.RootElement)
            };

            Action<IServiceCollection> configureServices = (IServiceCollection builder) =>
            {
                builder.AddSingleton<IJsonWriterFactory>(_ => ODataUtf8JsonWriterFactory.Default);
            };

            var result = await this.SetupSerializerAndRunTestAsync(
               async (jsonPropertySerializer) =>
               {
                   await jsonPropertySerializer.JsonWriter.StartObjectScopeAsync();
                   await jsonPropertySerializer.WritePropertyAsync(
                       property,
                       owningType: null,
                       isTopLevel: false,
                       duplicatePropertyNameChecker: new DuplicatePropertyNameChecker(),
                       metadataBuilder: null);
                   await jsonPropertySerializer.JsonWriter.EndObjectScopeAsync();
               }, configureServices);

            Assert.Equal("{\"JsonProp\":{\"foo\":\"bar\"}}", result);
        }

        private async Task<string> SetupSerializerAndRunTestAsync(Func<ODataJsonPropertySerializer, Task> func, Action<IServiceCollection> configureServices = null)
        {
            Stream outputStream = new AsyncStream(new MemoryStream());
            ODataJsonOutputContext jsonOutputContext = this.CreateJsonOutputContext(outputStream, configureServices);
            var jsonPropertySerializer = new ODataJsonPropertySerializer(jsonOutputContext, /* initContextUriBuilder */ true);

            await func(jsonPropertySerializer);
            await jsonPropertySerializer.JsonOutputContext.FlushAsync();
            await jsonPropertySerializer.JsonWriter.FlushAsync();

            outputStream.Position = 0;

            return await new StreamReader(outputStream).ReadToEndAsync();
        }

        private ODataJsonOutputContext CreateJsonOutputContext(Stream stream, Action<IServiceCollection> configureServices = null)
        {
            var settings = new ODataMessageWriterSettings { Version = ODataVersion.V4 };
            settings.ShouldIncludeAnnotationInternal = ODataUtils.CreateAnnotationFilter("*");
            settings.SetServiceDocumentUri(new Uri("http://tempuri.org/"));

            var messageInfo = new ODataMessageInfo
            {
                MessageStream = new NonDisposingStream(stream),
                MediaType = new ODataMediaType("application", "json"),
                Encoding = Encoding.UTF8,
                IsResponse = true,
                IsAsync = true,
                Model = this.model
            };

            if (configureServices != null)
            {
                messageInfo.ServiceProvider = ServiceProviderHelper.BuildServiceProvider(configureServices);
            }

            return new ODataJsonOutputContext(messageInfo, settings);
        }
    }
}

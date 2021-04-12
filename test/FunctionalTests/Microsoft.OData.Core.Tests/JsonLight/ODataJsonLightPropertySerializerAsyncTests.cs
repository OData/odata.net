//---------------------------------------------------------------------
// <copyright file="ODataJsonLightPropertySerializerAsyncTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.JsonLight;
using Xunit;

namespace Microsoft.OData.Tests.JsonLight
{
    public class ODataJsonLightPropertySerializerAsyncTests
    {
        private IEdmModel model;
        private IEdmEntityType entityType;
        private ODataProperty declaredPropertyWithInstanceAnnotations;
        private ODataProperty undeclaredPropertyWithInstanceAnnotations;
        private ODataPropertyInfo streamProperty;

        public ODataJsonLightPropertySerializerAsyncTests()
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
                async (jsonLightPropertySerializer) =>
                {
                    await jsonLightPropertySerializer.AsynchronousJsonWriter.StartObjectScopeAsync();
                    await jsonLightPropertySerializer.WritePropertyInfoAsync(
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
                async (jsonLightPropertySerializer) =>
                {
                    await jsonLightPropertySerializer.AsynchronousJsonWriter.StartObjectScopeAsync();
                    await jsonLightPropertySerializer.WritePropertyInfoAsync(
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
                async (jsonLightPropertySerializer) =>
                {
                    await jsonLightPropertySerializer.AsynchronousJsonWriter.StartObjectScopeAsync();
                    await jsonLightPropertySerializer.WritePropertyInfoAsync(
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
                async (jsonLightPropertySerializer) =>
                {
                    await jsonLightPropertySerializer.AsynchronousJsonWriter.StartObjectScopeAsync();
                    await jsonLightPropertySerializer.WritePropertyAsync(
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
                (jsonLightPropertySerializer) =>
                {
                    return jsonLightPropertySerializer.WriteTopLevelPropertyAsync(
                        new ODataProperty { Name = "LuckyNumber", Value = new ODataPrimitiveValue(13) }
                    );
                });

            Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Edm.Int32\",\"value\":13}", result);
        }

        private async Task<string> SetupSerializerAndRunTestAsync(Func<ODataJsonLightPropertySerializer, Task> func)
        {
            MemoryStream outputStream = new MemoryStream();
            ODataJsonLightOutputContext jsonLightOutputContext = this.CreateJsonLightOutputContext(outputStream);
            var jsonLightPropertySerializer = new ODataJsonLightPropertySerializer(jsonLightOutputContext, /* initContextUriBuilder */ true);

            await func(jsonLightPropertySerializer);
            await jsonLightPropertySerializer.JsonLightOutputContext.FlushAsync();
            await jsonLightPropertySerializer.AsynchronousJsonWriter.FlushAsync();

            outputStream.Position = 0;

            return await new StreamReader(outputStream).ReadToEndAsync();
        }

        private ODataJsonLightOutputContext CreateJsonLightOutputContext(MemoryStream stream)
        {
            var settings = new ODataMessageWriterSettings { Version = ODataVersion.V4 };
            settings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");
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

            return new ODataJsonLightOutputContext(messageInfo, settings);
        }
    }
}

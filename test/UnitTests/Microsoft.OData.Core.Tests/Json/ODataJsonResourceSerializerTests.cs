//---------------------------------------------------------------------
// <copyright file="ODataJsonResourceSerializerTests.cs" company="Microsoft">
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
using Microsoft.OData.Evaluation;
using Microsoft.OData.Json;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    public class ODataJsonResourceSerializerTests
    {
        private const string ServiceUri = "http://tempuri.org";
        private EdmModel model;
        private MemoryStream stream;
        private ODataMessageWriterSettings messageWriterSettings;
        private EdmEntityType productEntityType;
        private EdmEntityType categoryEntityType;
        private EdmEntitySet categoriesEntitySet;

        public ODataJsonResourceSerializerTests()
        {
            this.model = new EdmModel();

            this.categoryEntityType = new EdmEntityType("NS", "Category", /* baseType */ null, /* isAbstract */ false, /* isOpen */ true, /* hasStream */ true);
            this.productEntityType = new EdmEntityType("NS", "Product");

            this.categoryEntityType.AddKeys(this.categoryEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            this.categoryEntityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            this.categoryEntityType.AddStructuralProperty("BestSeller", new EdmEntityTypeReference(this.productEntityType, true));

            this.productEntityType.AddKeys(this.productEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            this.productEntityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            this.productEntityType.AddStructuralProperty("Category", new EdmEntityTypeReference(this.categoryEntityType, true));

            this.model.AddElement(this.categoryEntityType);
            this.model.AddElement(this.productEntityType);

            var entityContainer = new EdmEntityContainer("NS", "Container");
            this.model.AddElement(entityContainer);

            this.categoriesEntitySet = new EdmEntitySet(entityContainer, "Categories", this.categoryEntityType, true);

            this.stream = new MemoryStream();
            this.messageWriterSettings = new ODataMessageWriterSettings
            {
                EnableMessageStreamDisposal = false,
                Version = ODataVersion.V4,
                ShouldIncludeAnnotationInternal = ODataUtils.CreateAnnotationFilter("*")
            };

            this.messageWriterSettings.SetServiceDocumentUri(new Uri(ServiceUri));
        }

        [Theory]
        [InlineData(false, "Name", "{")]
        [InlineData(true, null, "{\"@odata.type\":\"#Collection(NS.Category)\"")]
        [InlineData(true, "Prop", "{\"Prop@odata.type\":\"#Collection(NS.Category)\"")]
        public async Task WriteResourceSetStartMetadataPropertiesAsync_WritesResourceSetMetadataProperties(bool isUndeclared, string propertyName, string expected)
        {
            var result = await SetupJsonResourceSerializerAndRunTestAsync(
                (jsonResourceSerializer) =>
                {
                    return jsonResourceSerializer.WriteResourceSetStartMetadataPropertiesAsync(
                        CreateCategoryResourceSet(),
                        propertyName,
                        "NS.Category",
                        isUndeclared);
                });

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(false, "{\"@odata.id\":\"http://tempuri.org/Categories(1)\"")]
        [InlineData(true, "{\"@odata.type\":\"#NS.Category\",\"@odata.id\":\"http://tempuri.org/Categories(1)\"")]
        public async Task WriteResourceStartMetadataPropertiesAsync_WritesResourceMetadataProperties(bool isUndeclared, string expected)
        {
            var jsonWriterResourceState = new TestODataJsonWriterResourceState(
                CreateCategoryResource(),
                this.categoryEntityType,
                CreateCategorySerializationInfo(),
                this.categoriesEntitySet,
                isUndeclared);

            var result = await SetupJsonResourceSerializerAndRunTestAsync(
                (jsonResourceSerializer) =>
                {
                    return jsonResourceSerializer.WriteResourceStartMetadataPropertiesAsync(
                        jsonWriterResourceState);
                });

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task WriteResourceStartMetadataPropertiesAsync_WritesETagMetadataProperty()
        {
            var resource = CreateCategoryResource();
            resource.ETag = "etag";

            var jsonWriterResourceState = new TestODataJsonWriterResourceState(
                resource,
                this.categoryEntityType,
                CreateCategorySerializationInfo(),
                this.categoriesEntitySet,
                false);

            var result = await SetupJsonResourceSerializerAndRunTestAsync(
                (jsonResourceSerializer) =>
                {
                    return jsonResourceSerializer.WriteResourceStartMetadataPropertiesAsync(
                        jsonWriterResourceState);
                });

            Assert.Equal("{\"@odata.id\":\"http://tempuri.org/Categories(1)\",\"@odata.etag\":\"etag\"", result);
        }

        public static IEnumerable<object[]> GetWriteResourceMetadataPropertiesTestData()
        {
            ODataResource resource;

            // Resource without media resource
            resource = CreateCategoryResource();

            yield return new object[]
            {
                resource,
                "{\"@odata.editLink\":\"http://tempuri.org/Categories(1)/editLink\"," +
                "\"@odata.readLink\":\"http://tempuri.org/Categories(1)/readLink\""
            };

            // Resource with media resource
            resource = CreateCategoryResource();
            resource.MediaResource = CreateCategoryMediaResource();

            yield return new object[]
            {
                resource,
                "{\"@odata.editLink\":\"http://tempuri.org/Categories(1)/editLink\"," +
                "\"@odata.readLink\":\"http://tempuri.org/Categories(1)/readLink\"," +
                "\"@odata.mediaEditLink\":\"http://tempuri.org/Categories(1)/blob/editLink\"," +
                "\"@odata.mediaReadLink\":\"http://tempuri.org/Categories(1)/blob/readLink\"," +
                "\"@odata.mediaContentType\":\"image/png\"," +
                "\"@odata.mediaEtag\":\"media-etag\""
            };

            // Resource with ReadLink identical to EditLink
            resource = CreateCategoryResource();
            resource.MediaResource = CreateCategoryMediaResource();
            resource.ReadLink = new Uri($"{ServiceUri}/Categories(1)/link");
            resource.EditLink = new Uri($"{ServiceUri}/Categories(1)/link");
            resource.MediaResource.ReadLink = new Uri($"{ServiceUri}/Categories(1)/blob/link");
            resource.MediaResource.EditLink = new Uri($"{ServiceUri}/Categories(1)/blob/link");

            yield return new object[]
            {
                resource,
                "{\"@odata.editLink\":\"http://tempuri.org/Categories(1)/link\"," +
                "\"@odata.mediaEditLink\":\"http://tempuri.org/Categories(1)/blob/link\"," +
                "\"@odata.mediaContentType\":\"image/png\"," +
                "\"@odata.mediaEtag\":\"media-etag\""
            };
        }

        [Theory]
        [MemberData(nameof(GetWriteResourceMetadataPropertiesTestData))]
        public async Task WriteResourceMetadataPropertiesAsync_WritesResourceMetadataProperties(ODataResource resource, string expected)
        {
            var jsonWriterResourceState = new TestODataJsonWriterResourceState(
                resource,
                this.categoryEntityType,
                CreateCategorySerializationInfo(),
                this.categoriesEntitySet,
                false);

            var result = await SetupJsonResourceSerializerAndRunTestAsync(
                (jsonResourceSerializer) =>
                {
                    return jsonResourceSerializer.WriteResourceMetadataPropertiesAsync(
                        jsonWriterResourceState);
                });

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task WriteResourceEndMetadataPropertiesAsync_WritesNavigationAndStreamMetadataProperties()
        {
            var resource = CreateCategoryResource();
            resource.MetadataBuilder = new TestODataResourceMetadataBuilder(
                resource.Id,
                unprocessedNavigationLinksFactory: () => GetNavigationLinks(resource.Id),
                unprocessedStreamPropertiesFactory: () => GetStreamProperties(resource.Id));

            var jsonWriterResourceState = new TestODataJsonWriterResourceState(
                resource,
                this.categoryEntityType,
                CreateCategorySerializationInfo(),
                this.categoriesEntitySet,
                false);

            var result = await SetupJsonResourceSerializerAndRunTestAsync(
                (jsonResourceSerializer) =>
                {
                    return jsonResourceSerializer.WriteResourceEndMetadataPropertiesAsync(
                        jsonWriterResourceState,
                        new NullDuplicatePropertyNameChecker());
                });

            Assert.Equal("{\"BestSeller@odata.associationLink\":\"http://tempuri.org/Categories(1)/BestSeller/$ref\"," +
                "\"BestSeller@odata.navigationLink\":\"http://tempuri.org/Categories(1)/BestSeller\"," +
                "\"Img@odata.mediaEditLink\":\"http://tempuri.org/Categories(1)/Img/Edit\"," +
                "\"Img@odata.mediaReadLink\":\"http://tempuri.org/Categories(1)/Img\"," +
                "\"Img@odata.mediaContentType\":\"image/png\"," +
                "\"Img@odata.mediaEtag\":\"img-etag\"",
                result);
        }

        public static IEnumerable<object[]> GetWriteResourceEndMetadataPropertiesTestData()
        {
            ODataResource resource;

            // Single action and single function
            resource = CreateCategoryResource();

            resource.AddAction(CreateODataAction(resource.Id, "Action1"));
            resource.AddFunction(CreateODataFunction(resource.Id, "Function1"));

            yield return new object[]
            {
                resource,

                "{\"#Action\":" +
                "{\"title\":\"Action1\",\"target\":\"http://tempuri.org/Categories(1)/Action1\"}," +
                "\"#Function\":" +
                "{\"title\":\"Function1\",\"target\":\"http://tempuri.org/Categories(1)/Function1\"}"
            };

            // Action and function greater than 1
            resource = CreateCategoryResource();

            resource.AddAction(CreateODataAction(resource.Id, "Action1"));
            resource.AddAction(CreateODataAction(resource.Id, "Action2"));
            resource.AddFunction(CreateODataFunction(resource.Id, "Function1"));
            resource.AddFunction(CreateODataFunction(resource.Id, "Function2"));

            yield return new object[]
            {
                resource,
                "{\"#Action\":[" +
                "{\"title\":\"Action1\",\"target\":\"http://tempuri.org/Categories(1)/Action1\"}," +
                "{\"title\":\"Action2\",\"target\":\"http://tempuri.org/Categories(1)/Action2\"}]," +
                "\"#Function\":[" +
                "{\"title\":\"Function1\",\"target\":\"http://tempuri.org/Categories(1)/Function1\"}," +
                "{\"title\":\"Function2\",\"target\":\"http://tempuri.org/Categories(1)/Function2\"}]"
            };
        }

        [Theory]
        [MemberData(nameof(GetWriteResourceEndMetadataPropertiesTestData))]
        public async Task WriteResourceEndMetadataPropertiesAsync_WritesActionsAndFunctionsMetadataProperties(ODataResource resource, string expected)
        {
            var jsonWriterResourceState = new TestODataJsonWriterResourceState(
                resource,
                this.categoryEntityType,
                CreateCategorySerializationInfo(),
                this.categoriesEntitySet,
                false);

            var result = await SetupJsonResourceSerializerAndRunTestAsync(
                (jsonResourceSerializer) =>
                {
                    return jsonResourceSerializer.WriteResourceEndMetadataPropertiesAsync(
                        jsonWriterResourceState,
                        new NullDuplicatePropertyNameChecker());
                });

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task WriteNavigationLinkMetadataAsync_WritesNavigationLinkMetadata()
        {
            var nestedResourceInfo = new ODataNestedResourceInfo
            {
                Name = "BestSeller",
                Url = new Uri($"{ServiceUri}/Categories(1)/BestSeller"),
                IsCollection = false,
                AssociationLinkUrl = new Uri($"{ServiceUri}/Categories(1)/associationLink")
            };

            var result = await SetupJsonResourceSerializerAndRunTestAsync(
                (jsonResourceSerializer) =>
                {
                    return jsonResourceSerializer.WriteNavigationLinkMetadataAsync(
                        nestedResourceInfo,
                        new NullDuplicatePropertyNameChecker());
                });

            Assert.Equal("{\"BestSeller@odata.associationLink\":\"http://tempuri.org/Categories(1)/associationLink\"," +
                "\"BestSeller@odata.navigationLink\":\"http://tempuri.org/Categories(1)/BestSeller\"", result);
        }

        [Fact]
        public async Task WriteNavigationLinkMetadataAsync_WritesCountMetadata()
        {
            var nestedResourceInfo = new ODataNestedResourceInfo
            {
                Name = "BestSeller",
                IsCollection = true,
                Count = 42
            };

            var result = await SetupJsonResourceSerializerAndRunTestAsync(
                (jsonResourceSerializer) =>
                {
                    return jsonResourceSerializer.WriteNavigationLinkMetadataAsync(
                        nestedResourceInfo,
                        new NullDuplicatePropertyNameChecker(), true);
                });

            Assert.Equal("{\"BestSeller@odata.count\":42", result);
        }

        [Fact]
        public async Task WriteNestedResourceInfoContextUrlAsync_WritesNestedResourceInfoContextUrl()
        {
            var nestedResourceInfo = new ODataNestedResourceInfo
            {
                Name = "BestSeller",
                Url = new Uri($"{ServiceUri}/Categories(1)/BestSeller"),
                IsCollection = false,
                AssociationLinkUrl = new Uri($"{ServiceUri}/Categories(1)/associationLink")
            };
            var typeContext = ODataResourceTypeContext.Create(
                CreateCategorySerializationInfo(),
                /* navigationSource */ null,
                /* navigationSourceEntityType */ null,
                /* expectedResourceType */ null);
            var contextUrlInfo = ODataContextUrlInfo.Create(
                typeContext,
                this.messageWriterSettings.Version ?? ODataVersion.V4,
                ODataDeltaKind.Resource,
                this.messageWriterSettings.ODataUri);

            var result = await SetupJsonResourceSerializerAndRunTestAsync(
                (jsonResourceSerializer) =>
                {
                    return jsonResourceSerializer.WriteNestedResourceInfoContextUrlAsync(
                        nestedResourceInfo,
                        contextUrlInfo);
                });

            Assert.Equal("{\"BestSeller@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"", result);
        }

        public static IEnumerable<object[]> GetWriteOperationsTestData()
        {
            // Single action
            yield return new object[]
            {
                true,
                new string[] { "Action1" },
                "{\"#Action\":{\"title\":\"Action1\",\"target\":\"http://tempuri.org/Categories(1)/Action1\"}"
            };

            // Single function
            yield return new object[]
            {
                false,
                new string[] { "Function1" },
                "{\"#Function\":{\"title\":\"Function1\",\"target\":\"http://tempuri.org/Categories(1)/Function1\"}"
            };

            // Actions greater than 1
            yield return new object[]
            {
                true,
                new string[] { "Action1", "Action2" },
                "{\"#Action\":[" +
                "{\"title\":\"Action1\",\"target\":\"http://tempuri.org/Categories(1)/Action1\"}," +
                "{\"title\":\"Action2\",\"target\":\"http://tempuri.org/Categories(1)/Action2\"}]"
            };

            // Functions greater than 1
            yield return new object[]
            {
                false,
                new string[] { "Function1", "Function2" },
                "{\"#Function\":[" +
                "{\"title\":\"Function1\",\"target\":\"http://tempuri.org/Categories(1)/Function1\"}," +
                "{\"title\":\"Function2\",\"target\":\"http://tempuri.org/Categories(1)/Function2\"}]"
            };
        }

        [Theory]
        [MemberData(nameof(GetWriteOperationsTestData))]
        public async Task WriteOperationsAsync_WritesOperations(bool isAction, string[] operations, string expected)
        {
            var resourceId = new Uri($"{ServiceUri}/Categories(1)");

            Func<string, ODataOperation> operationFactory;
            if (isAction)
            {
                operationFactory = (operation) => CreateODataAction(resourceId, operation);
            }
            else
            {
                operationFactory = (operation) => CreateODataFunction(resourceId, operation);
            }

            var result = await SetupJsonResourceSerializerAndRunTestAsync(
                (jsonResourceSerializer) =>
                {
                    return jsonResourceSerializer.WriteOperationsAsync(
                        operations.Select(operationFactory),
                        isAction);
                });

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task WriteDeltaContextUriAsync_WritesDeltaContextUri()
        {
            var typeContext = ODataResourceTypeContext.Create(
                /* serializationInfo */ null,
                this.categoriesEntitySet,
                this.categoryEntityType,
                this.categoryEntityType);

            var result = await SetupJsonResourceSerializerAndRunTestAsync(
                (jsonResourceSerializer) =>
                {
                    return jsonResourceSerializer.WriteDeltaContextUriAsync(
                        typeContext,
                        ODataDeltaKind.Resource,
                        /* parentContextUrlInfo */ null);
                });

            Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"", result);
        }

        [Fact]
        public async Task WriteResourceContextUriAsync_WritesResourceContextUri()
        {
            var typeContext = ODataResourceTypeContext.Create(
                CreateCategorySerializationInfo(),
                /* navigationSource */ null,
                /* navigationSourceEntityType */ null,
                /* expectedResourceType */ null);

            var result = await SetupJsonResourceSerializerAndRunTestAsync(
                (jsonResourceSerializer) =>
                {
                    return jsonResourceSerializer.WriteResourceContextUriAsync(
                        typeContext,
                        /* parentContextUrlInfo */ null);
                });

            Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Categories/$entity\"", result);
        }

        [Fact]
        public async Task WriteResourceSetContextUriAsync_WritesResourceSetContextUri()
        {
            var typeContext = ODataResourceTypeContext.Create(
                CreateCategorySerializationInfo(),
                /* navigationSource */ null,
                /* navigationSourceEntityType */ null,
                /* expectedResourceType */ null);

            var result = await SetupJsonResourceSerializerAndRunTestAsync(
                (jsonResourceSerializer) =>
                {
                    return jsonResourceSerializer.WriteResourceSetContextUriAsync(typeContext);
                });

            Assert.Equal("{\"@odata.context\":\"http://tempuri.org/$metadata#Categories\"", result);
        }

        private async Task<string> SetupJsonResourceSerializerAndRunTestAsync(Func<ODataJsonResourceSerializer, Task> func)
        {
            ODataJsonOutputContext jsonOutputContext = this.CreateJsonOutputContext();
            var jsonResourceSerializer = new ODataJsonResourceSerializer(jsonOutputContext);

            await jsonResourceSerializer.JsonWriter.StartObjectScopeAsync();
            await func(jsonResourceSerializer);
            await jsonResourceSerializer.JsonOutputContext.FlushAsync();
            await jsonResourceSerializer.JsonWriter.FlushAsync();

            this.stream.Position = 0;

            return await new StreamReader(this.stream).ReadToEndAsync();
        }

        private ODataJsonOutputContext CreateJsonOutputContext(bool isAsync = true)
        {
            var messageInfo = new ODataMessageInfo
            {
                MessageStream = new NonDisposingStream(this.stream),
                MediaType = new ODataMediaType("application", "json"),
                Encoding = Encoding.UTF8,
                IsResponse = true,
                IsAsync = isAsync,
                Model = this.model
            };

            return new ODataJsonOutputContext(messageInfo, this.messageWriterSettings);
        }

        #region Helper Methods

        private static ODataResourceSerializationInfo CreateCategorySerializationInfo()
        {
            return new ODataResourceSerializationInfo
            {
                ExpectedTypeName = "NS.Category",
                NavigationSourceName = "Categories",
                NavigationSourceEntityTypeName = "NS.Category",
                NavigationSourceKind = EdmNavigationSourceKind.EntitySet
            };
        }

        private static ODataResourceSet CreateCategoryResourceSet()
        {
            return new ODataResourceSet
            {
                Id = new Uri($"{ServiceUri}/Categories"),
                TypeName = "Collection(NS.Category)",
                Count = 7,
                NextPageLink = new Uri($"{ServiceUri}/nextLink"),
                InstanceAnnotations = new List<ODataInstanceAnnotation>
                {
                    new ODataInstanceAnnotation("Is.ResourceSet", new ODataPrimitiveValue(true))
                },
                SerializationInfo = CreateCategorySerializationInfo()
            };
        }

        private static ODataResource CreateCategoryResource()
        {
            return new ODataResource
            {
                Id = new Uri($"{ServiceUri}/Categories(1)"),
                TypeName = "NS.Category",
                EditLink = new Uri($"{ServiceUri}/Categories(1)/editLink"),
                ReadLink = new Uri($"{ServiceUri}/Categories(1)/readLink"),
                InstanceAnnotations = new List<ODataInstanceAnnotation>
                    {
                        new ODataInstanceAnnotation("Is.Resource", new ODataPrimitiveValue(true))
                    },
                Properties = new List<ODataProperty>
                    {
                        new ODataProperty { Name = "Id", Value = new ODataPrimitiveValue(1) },
                        new ODataProperty { Name = "Name", Value = new ODataPrimitiveValue("Food") },
                    },
                SerializationInfo = CreateCategorySerializationInfo()
            };
        }

        private static ODataStreamReferenceValue CreateCategoryMediaResource()
        {
            return new ODataStreamReferenceValue
            {
                EditLink = new Uri($"{ServiceUri}/Categories(1)/blob/editLink"),
                ReadLink = new Uri($"{ServiceUri}/Categories(1)/blob/readLink"),
                ContentType = "image/png",
                ETag = "media-etag"
            };
        }

        private IEnumerable<ODataJsonReaderNestedResourceInfo> GetNavigationLinks(Uri resourceId)
        {
            var uriString = resourceId.ToString();

            var jsonReaderNestedResourceInfo = ODataJsonReaderNestedResourceInfo.CreateResourceReaderNestedResourceInfo(
                nestedResourceInfo: new ODataNestedResourceInfo
                {
                    Name = "BestSeller",
                    Url = new Uri($"{uriString}/BestSeller"),
                    IsCollection = false,
                    AssociationLinkUrl = new Uri($"{uriString}/BestSeller/$ref")
                },
                nestedProperty: this.categoryEntityType.FindProperty("BestSeller"),
                nestedResourceType: this.productEntityType);

            yield return jsonReaderNestedResourceInfo;
        }

        private IEnumerable<ODataProperty> GetStreamProperties(Uri resourceId)
        {
            var uriString = resourceId.ToString();

            yield return new ODataProperty
            {
                Name = "Img",
                Value = new ODataStreamReferenceValue
                {
                    EditLink = new Uri($"{uriString}/Img/Edit"),
                    ReadLink = new Uri($"{uriString}/Img"),
                    ETag = "img-etag",
                    ContentType = "image/png"
                }
            };
        }

        private static ODataAction CreateODataAction(Uri resourceId, string actionName)
        {
            return new ODataAction
            {
                Title = actionName,
                Target = new Uri($"{resourceId}/{actionName}"),
                Metadata = new Uri($"{ServiceUri}/$metadata/#Action")
            };
        }

        private static ODataFunction CreateODataFunction(Uri resourceId, string functionName)
        {
            return new ODataFunction
            {
                Title = functionName,
                Target = new Uri($"{resourceId}/{functionName}"),
                Metadata = new Uri($"{ServiceUri}/$metadata/#Function")
            };
        }

        #endregion Helper Methods
    }
}

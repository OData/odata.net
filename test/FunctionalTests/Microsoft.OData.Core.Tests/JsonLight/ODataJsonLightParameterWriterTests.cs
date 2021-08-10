//---------------------------------------------------------------------
// <copyright file="ODataJsonLightParameterWriterTests.cs" company="Microsoft">
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
    public class ODataJsonLightParameterWriterTests
    {
        [Fact]
        public void ShouldWriteParameterPayloadInRequestWithoutModelAndWithoutFunctionImport()
        {
            Action<ODataJsonLightOutputContext> test = outputContext =>
            {
                var parameterWriter = new ODataJsonLightParameterWriter(outputContext, operation: null);
                parameterWriter.WriteStart();
                parameterWriter.WriteValue("primitive", Guid.Empty);
                var resourceWriter = parameterWriter.CreateResourceWriter("complex");
                resourceWriter.WriteStart(new ODataResource() { Properties = new[] { new ODataProperty { Name = "prop1", Value = 1 } } });
                resourceWriter.WriteEnd();
                var collectionWriter = parameterWriter.CreateCollectionWriter("collection");
                collectionWriter.WriteStart(new ODataCollectionStart());
                collectionWriter.WriteItem("item1");
                collectionWriter.WriteEnd();
                parameterWriter.WriteEnd();
                parameterWriter.Flush();
            };

            WriteAndValidate(test, "{\"primitive\":\"00000000-0000-0000-0000-000000000000\",\"complex\":{\"prop1\":1},\"collection\":[\"item1\"]}", writingResponse: false);
        }

        [Fact]
        public void WriteEntryWithoutOperation()
        {
            Action<ODataJsonLightOutputContext> test = outputContext =>
            {
                var entry = new ODataResource();
                entry.Properties = new List<ODataProperty>() { new ODataProperty() { Name = "ID", Value = 1 } };

                var parameterWriter = new ODataJsonLightParameterWriter(outputContext, operation: null);
                parameterWriter.WriteStart();
                var entryWriter = parameterWriter.CreateResourceWriter("entry");
                entryWriter.WriteStart(entry);
                entryWriter.WriteEnd();
                parameterWriter.WriteEnd();
                parameterWriter.Flush();
            };

            WriteAndValidate(test, "{\"entry\":{\"ID\":1}}", writingResponse: false);
        }

        [Fact]
        public void WriteEntryAndComplex()
        {
            Action<ODataJsonLightOutputContext> test = outputContext =>
            {
                var entry = new ODataResource();

                var complex = new ODataResource() { Properties = new List<ODataProperty>() { new ODataProperty() { Name = "Name", Value = "ComplexName" } } };
                entry.Properties = new List<ODataProperty>() { new ODataProperty() { Name = "ID", Value = 1 } };
                var nestedComplexInfo = new ODataNestedResourceInfo() { Name = "complexProperty", IsCollection = false };
                var parameterWriter = new ODataJsonLightParameterWriter(outputContext, operation: null);
                parameterWriter.WriteStart();
                var entryWriter = parameterWriter.CreateResourceWriter("entry");
                entryWriter.WriteStart(entry);
                entryWriter.WriteStart(nestedComplexInfo);
                entryWriter.WriteStart(complex);
                entryWriter.WriteEnd();
                entryWriter.WriteEnd();
                entryWriter.WriteEnd();

                var complexWriter = parameterWriter.CreateResourceWriter("complex");
                complexWriter.WriteStart(complex);
                complexWriter.WriteEnd();
                parameterWriter.WriteEnd();
                parameterWriter.Flush();
            };

            WriteAndValidate(test, "{\"entry\":{\"ID\":1,\"complexProperty\":{\"Name\":\"ComplexName\"}},\"complex\":{\"Name\":\"ComplexName\"}}", writingResponse: false);
        }

        [Fact]
        public void WriteFeedWithoutOperation()
        {
            Action<ODataJsonLightOutputContext> test = outputContext =>
            {
                var entry = new ODataResource();
                entry.Properties = new List<ODataProperty>() { new ODataProperty() { Name = "ID", Value = 1 } };

                var parameterWriter = new ODataJsonLightParameterWriter(outputContext, operation: null);
                parameterWriter.WriteStart();
                var entryWriter = parameterWriter.CreateResourceSetWriter("feed");
                entryWriter.WriteStart(new ODataResourceSet());
                entryWriter.WriteStart(entry);
                entryWriter.WriteEnd();
                entryWriter.WriteEnd();
                parameterWriter.WriteEnd();
                parameterWriter.Flush();
            };

            WriteAndValidate(test, "{\"feed\":[{\"ID\":1}]}", writingResponse: false);
        }

        [Fact]
        public void WriteEntryWithOperation()
        {
            EdmEntityType entityType = new EdmEntityType("NS", "Entity");
            IEdmStructuralProperty keyProp = new EdmStructuralProperty(entityType, "Id", EdmCoreModel.Instance.GetInt32(false));
            entityType.AddProperty(keyProp);
            entityType.AddKeys(keyProp);

            EdmOperation operation = new EdmFunction("NS", "Foo", EdmCoreModel.Instance.GetInt16(true));
            operation.AddParameter("entry", new EdmEntityTypeReference(entityType, false));

            Action<ODataJsonLightOutputContext> test = outputContext =>
            {
                var entry = new ODataResource();
                entry.Properties = new List<ODataProperty>() { new ODataProperty() { Name = "ID", Value = 1 } };

                var parameterWriter = new ODataJsonLightParameterWriter(outputContext, operation);
                parameterWriter.WriteStart();
                var entryWriter = parameterWriter.CreateResourceWriter("entry");
                entryWriter.WriteStart(entry);
                entryWriter.WriteEnd();
                parameterWriter.WriteEnd();
                parameterWriter.Flush();
            };

            WriteAndValidate(test, "{\"entry\":{\"ID\":1}}", writingResponse: false);
        }

        [Fact]
        public void WriteOpenEntry()
        {
            EdmEntityType entityType = new EdmEntityType("NS", "Entity", null, false, true);
            IEdmStructuralProperty keyProp = new EdmStructuralProperty(entityType, "Id", EdmCoreModel.Instance.GetInt32(false));
            entityType.AddProperty(keyProp);
            entityType.AddKeys(keyProp);

            EdmOperation operation = new EdmFunction("NS", "Foo", EdmCoreModel.Instance.GetInt16(true));
            operation.AddParameter("entry", new EdmEntityTypeReference(entityType, false));

            Action<ODataJsonLightOutputContext> test = outputContext =>
            {
                var entry = new ODataResource();
                entry.Properties = new List<ODataProperty>() { new ODataProperty() { Name = "ID", Value = 1 }, new ODataProperty() { Name = "DynamicProperty", Value = "DynamicValue" } };

                var parameterWriter = new ODataJsonLightParameterWriter(outputContext, operation);
                parameterWriter.WriteStart();
                var entryWriter = parameterWriter.CreateResourceWriter("entry");
                entryWriter.WriteStart(entry);
                entryWriter.WriteEnd();
                parameterWriter.WriteEnd();
                parameterWriter.Flush();
            };

            WriteAndValidate(test, "{\"entry\":{\"ID\":1,\"DynamicProperty\":\"DynamicValue\"}}", writingResponse: false);
        }


        [Fact]
        public void WriteFeedWithOperation()
        {
            EdmEntityType entityType = new EdmEntityType("NS", "Entity");
            IEdmStructuralProperty keyProp = new EdmStructuralProperty(entityType, "Id", EdmCoreModel.Instance.GetInt32(false));
            entityType.AddProperty(keyProp);
            entityType.AddKeys(keyProp);

            EdmOperation operation = new EdmFunction("NS", "Foo", EdmCoreModel.Instance.GetInt16(true));
            operation.AddParameter("feed", new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(entityType, false))));

            Action<ODataJsonLightOutputContext> test = outputContext =>
            {
                var entry = new ODataResource();
                entry.Properties = new List<ODataProperty>() { new ODataProperty() { Name = "ID", Value = 1 } };

                var parameterWriter = new ODataJsonLightParameterWriter(outputContext, operation: null);
                parameterWriter.WriteStart();
                var entryWriter = parameterWriter.CreateResourceSetWriter("feed");
                entryWriter.WriteStart(new ODataResourceSet());
                entryWriter.WriteStart(entry);
                entryWriter.WriteEnd();
                entryWriter.WriteEnd();
                parameterWriter.WriteEnd();
                parameterWriter.Flush();
            };

            WriteAndValidate(test, "{\"feed\":[{\"ID\":1}]}", writingResponse: false);
        }

        [Fact]
        public void WriteFeedWithMultipleEntries()
        {
            EdmEntityType entityType = new EdmEntityType("NS", "Entity");
            IEdmStructuralProperty keyProp = new EdmStructuralProperty(entityType, "Id", EdmCoreModel.Instance.GetInt32(false));
            entityType.AddProperty(keyProp);
            entityType.AddKeys(keyProp);
            EdmEntityType derivedType = new EdmEntityType("NS", "DerivedType", entityType);
            derivedType.AddProperty(new EdmStructuralProperty(derivedType, "Name", EdmCoreModel.Instance.GetString(false)));

            EdmOperation operation = new EdmFunction("NS", "Foo", EdmCoreModel.Instance.GetInt16(true));
            operation.AddParameter("feed", new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(entityType, false))));

            Action<ODataJsonLightOutputContext> test = outputContext =>
            {
                var entry = new ODataResource();
                entry.Properties = new List<ODataProperty>() { new ODataProperty() { Name = "ID", Value = 1 } };

                var entry2 = new ODataResource()
                {
                    TypeName = "NS.DerivedType",
                };
                entry2.Properties = new List<ODataProperty>()
                {
                    new ODataProperty() { Name = "ID", Value = 1 },
                    new ODataProperty() { Name = "Name", Value = "TestName"}
                };

                var parameterWriter = new ODataJsonLightParameterWriter(outputContext, operation: null);
                parameterWriter.WriteStart();
                var entryWriter = parameterWriter.CreateResourceSetWriter("feed");
                entryWriter.WriteStart(new ODataResourceSet());
                entryWriter.WriteStart(entry);
                entryWriter.WriteEnd();
                entryWriter.WriteStart(entry2);
                entryWriter.WriteEnd();
                entryWriter.WriteEnd();
                parameterWriter.WriteEnd();
                parameterWriter.Flush();
            };

            WriteAndValidate(test, "{\"feed\":[{\"ID\":1},{\"@odata.type\":\"#NS.DerivedType\",\"ID\":1,\"Name\":\"TestName\"}]}", writingResponse: false);
        }

        [Fact]
        public void WriteNullEntryWithOperation()
        {
            EdmEntityType entityType = new EdmEntityType("NS", "Entity");
            IEdmStructuralProperty keyProp = new EdmStructuralProperty(entityType, "Id", EdmCoreModel.Instance.GetInt32(false));
            entityType.AddProperty(keyProp);
            entityType.AddKeys(keyProp);

            EdmOperation operation = new EdmFunction("NS", "Foo", EdmCoreModel.Instance.GetInt16(true));
            operation.AddParameter("entry", new EdmEntityTypeReference(entityType, true));

            Action<ODataJsonLightOutputContext> test = outputContext =>
            {
                var parameterWriter = new ODataJsonLightParameterWriter(outputContext, operation);
                parameterWriter.WriteStart();

                var entryWriter = parameterWriter.CreateResourceWriter("entry");
                entryWriter.WriteStart((ODataResource)null);
                entryWriter.WriteEnd();

                parameterWriter.WriteEnd();
                parameterWriter.Flush();
            };

            WriteAndValidate(test, "{\"entry\":null}", writingResponse: false);
        }

        [Fact]
        public void WriteDerivedEntryWithOperation()
        {
            EdmEntityType entityType = new EdmEntityType("NS", "Entity");
            IEdmStructuralProperty keyProp = new EdmStructuralProperty(entityType, "Id", EdmCoreModel.Instance.GetInt32(false));
            entityType.AddProperty(keyProp);
            entityType.AddKeys(keyProp);
            EdmEntityType derivedType = new EdmEntityType("NS", "DerivedType", entityType);
            derivedType.AddProperty(new EdmStructuralProperty(derivedType, "Name", EdmCoreModel.Instance.GetString(false)));

            EdmOperation operation = new EdmFunction("NS", "Foo", EdmCoreModel.Instance.GetInt16(true));
            operation.AddParameter("entry", new EdmEntityTypeReference(entityType, true));

            Action<ODataJsonLightOutputContext> test = outputContext =>
            {
                var entry = new ODataResource()
                {
                    TypeName = "NS.DerivedType",
                };
                entry.Properties = new List<ODataProperty>()
                {
                    new ODataProperty() { Name = "ID", Value = 1 },
                    new ODataProperty() { Name = "Name", Value = "TestName"}
                };

                var parameterWriter = new ODataJsonLightParameterWriter(outputContext, operation);
                parameterWriter.WriteStart();

                var entryWriter = parameterWriter.CreateResourceWriter("entry");
                entryWriter.WriteStart(entry);
                entryWriter.WriteEnd();


                parameterWriter.WriteEnd();
                parameterWriter.Flush();
            };

            WriteAndValidate(test, "{\"entry\":{\"@odata.type\":\"#NS.DerivedType\",\"ID\":1,\"Name\":\"TestName\"}}", writingResponse: false);
        }

        [Fact]
        public void WriteDerivedEntityInFeed()
        {
            EdmEntityType entityType = new EdmEntityType("NS", "Entity");
            IEdmStructuralProperty keyProp = new EdmStructuralProperty(entityType, "Id", EdmCoreModel.Instance.GetInt32(false));
            entityType.AddProperty(keyProp);
            entityType.AddKeys(keyProp);

            EdmEntityType derivedType = new EdmEntityType("NS", "DerivedType", entityType);
            derivedType.AddProperty(new EdmStructuralProperty(derivedType, "Name", EdmCoreModel.Instance.GetString(false)));

            EdmOperation operation = new EdmFunction("NS", "Foo", EdmCoreModel.Instance.GetInt16(true));
            operation.AddParameter("feed", new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(entityType, false))));

            Action<ODataJsonLightOutputContext> test = outputContext =>
            {
                var entry = new ODataResource()
                {
                    TypeName = "NS.DerivedType",
                };
                entry.Properties = new List<ODataProperty>()
                {
                    new ODataProperty() { Name = "ID", Value = 1 },
                    new ODataProperty() { Name = "Name", Value = "TestName"}
                };

                var parameterWriter = new ODataJsonLightParameterWriter(outputContext, operation: null);
                parameterWriter.WriteStart();
                var entryWriter = parameterWriter.CreateResourceSetWriter("feed");
                entryWriter.WriteStart(new ODataResourceSet());
                entryWriter.WriteStart(entry);
                entryWriter.WriteEnd();
                entryWriter.WriteEnd();
                parameterWriter.WriteEnd();
                parameterWriter.Flush();
            };

            WriteAndValidate(test, "{\"feed\":[{\"@odata.type\":\"#NS.DerivedType\",\"ID\":1,\"Name\":\"TestName\"}]}", writingResponse: false);
        }

        [Fact]
        public void WriteNestedEntity()
        {
            EdmEntityType entityType = new EdmEntityType("NS", "Entity");
            entityType.AddProperty(new EdmStructuralProperty(entityType, "Id", EdmCoreModel.Instance.GetInt32(false)));

            EdmEntityType expandEntityType = new EdmEntityType("NS", "ExpandEntity");
            expandEntityType.AddProperty(new EdmStructuralProperty(expandEntityType, "Id", EdmCoreModel.Instance.GetInt32(false)));
            expandEntityType.AddProperty(new EdmStructuralProperty(expandEntityType, "Name", EdmCoreModel.Instance.GetString(false)));

            entityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() { Name = "Property1", Target = expandEntityType, TargetMultiplicity = EdmMultiplicity.One });

            EdmOperation operation = new EdmFunction("NS", "Foo", EdmCoreModel.Instance.GetInt16(true));
            operation.AddParameter("entry", new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(entityType, false))));

            Action<ODataJsonLightOutputContext> test = outputContext =>
            {
                var entry1 = new ODataResource();
                entry1.Properties = new List<ODataProperty>()
                {
                    new ODataProperty() { Name = "ID", Value = 1 },
                };

                var entry2 = new ODataResource();
                entry2.Properties = new List<ODataProperty>()
                {
                    new ODataProperty() { Name = "ID", Value = 1 },
                    new ODataProperty() { Name = "Name", Value = "TestName"}
                };

                var parameterWriter = new ODataJsonLightParameterWriter(outputContext, operation: null);
                parameterWriter.WriteStart();
                var entryWriter = parameterWriter.CreateResourceSetWriter("feed");
                entryWriter.WriteStart(new ODataResourceSet());
                entryWriter.WriteStart(entry1);
                entryWriter.WriteStart(new ODataNestedResourceInfo()
                {
                    Name = "Property1",
                    IsCollection = false
                });
                entryWriter.WriteStart(entry2);
                entryWriter.WriteEnd();
                entryWriter.WriteEnd();
                entryWriter.WriteEnd();
                entryWriter.WriteEnd();
                parameterWriter.WriteEnd();
                parameterWriter.Flush();
            };

            WriteAndValidate(test, "{\"feed\":[{\"ID\":1,\"Property1\":{\"ID\":1,\"Name\":\"TestName\"}}]}", writingResponse: false);
        }

        [Fact]
        public void WriteContainedEntity()
        {
            EdmEntityType entityType = new EdmEntityType("NS", "Entity");
            entityType.AddProperty(new EdmStructuralProperty(entityType, "Id", EdmCoreModel.Instance.GetInt32(false)));

            EdmEntityType expandEntityType = new EdmEntityType("NS", "ExpandEntity");
            expandEntityType.AddProperty(new EdmStructuralProperty(expandEntityType, "Id", EdmCoreModel.Instance.GetInt32(false)));
            expandEntityType.AddProperty(new EdmStructuralProperty(expandEntityType, "Name", EdmCoreModel.Instance.GetString(false)));

            entityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() { ContainsTarget = true, Name = "Property1", Target = expandEntityType, TargetMultiplicity = EdmMultiplicity.One });

            EdmOperation operation = new EdmFunction("NS", "Foo", EdmCoreModel.Instance.GetInt16(true));
            operation.AddParameter("entry", new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(entityType, false))));

            Action<ODataJsonLightOutputContext> test = outputContext =>
            {
                var entry1 = new ODataResource();
                entry1.Properties = new List<ODataProperty>()
                {
                    new ODataProperty() { Name = "ID", Value = 1 },
                };

                var entry2 = new ODataResource();
                entry2.Properties = new List<ODataProperty>()
                {
                    new ODataProperty() { Name = "ID", Value = 1 },
                    new ODataProperty() { Name = "Name", Value = "TestName"}
                };

                var parameterWriter = new ODataJsonLightParameterWriter(outputContext, operation: null);
                parameterWriter.WriteStart();
                var entryWriter = parameterWriter.CreateResourceSetWriter("feed");
                entryWriter.WriteStart(new ODataResourceSet());
                entryWriter.WriteStart(entry1);
                entryWriter.WriteStart(new ODataNestedResourceInfo()
                {
                    Name = "Property1",
                    IsCollection = false
                });
                entryWriter.WriteStart(entry2);
                entryWriter.WriteEnd();
                entryWriter.WriteEnd();
                entryWriter.WriteEnd();
                entryWriter.WriteEnd();
                parameterWriter.WriteEnd();
                parameterWriter.Flush();
            };

            WriteAndValidate(test, "{\"feed\":[{\"ID\":1,\"Property1\":{\"ID\":1,\"Name\":\"TestName\"}}]}", writingResponse: false);
        }

        [Fact]
        public void WriteNestedFeed()
        {
            EdmEntityType entityType = new EdmEntityType("NS", "Entity");
            entityType.AddProperty(new EdmStructuralProperty(entityType, "Id", EdmCoreModel.Instance.GetInt32(false)));

            EdmEntityType expandEntityType = new EdmEntityType("NS", "ExpandEntity");
            expandEntityType.AddProperty(new EdmStructuralProperty(expandEntityType, "Id", EdmCoreModel.Instance.GetInt32(false)));
            expandEntityType.AddProperty(new EdmStructuralProperty(expandEntityType, "Name", EdmCoreModel.Instance.GetString(false)));

            entityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() { Name = "Property1", Target = expandEntityType, TargetMultiplicity = EdmMultiplicity.Many });

            EdmOperation operation = new EdmFunction("NS", "Foo", EdmCoreModel.Instance.GetInt16(true));
            operation.AddParameter("entry", new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(entityType, false))));

            Action<ODataJsonLightOutputContext> test = outputContext =>
            {
                var entry1 = new ODataResource();
                entry1.Properties = new List<ODataProperty>()
                {
                    new ODataProperty() { Name = "ID", Value = 1 },
                };

                var entry2 = new ODataResource();
                entry2.Properties = new List<ODataProperty>()
                {
                    new ODataProperty() { Name = "ID", Value = 1 },
                    new ODataProperty() { Name = "Name", Value = "TestName"}
                };

                var parameterWriter = new ODataJsonLightParameterWriter(outputContext, operation: null);
                parameterWriter.WriteStart();
                var entryWriter = parameterWriter.CreateResourceSetWriter("feed");
                entryWriter.WriteStart(new ODataResourceSet());
                entryWriter.WriteStart(entry1);
                entryWriter.WriteStart(new ODataNestedResourceInfo()
                {
                    Name = "Property1",
                    IsCollection = true
                });
                entryWriter.WriteStart(new ODataResourceSet());
                entryWriter.WriteStart(entry2);
                entryWriter.WriteEnd();
                entryWriter.WriteEnd();
                entryWriter.WriteEnd();
                entryWriter.WriteEnd();
                entryWriter.WriteEnd();
                parameterWriter.WriteEnd();
                parameterWriter.Flush();
            };

            WriteAndValidate(test, "{\"feed\":[{\"ID\":1,\"Property1\":[{\"ID\":1,\"Name\":\"TestName\"}]}]}", writingResponse: false);
        }

        [Fact]
        public void WriteContainedFeed()
        {
            EdmEntityType entityType = new EdmEntityType("NS", "Entity");
            entityType.AddProperty(new EdmStructuralProperty(entityType, "Id", EdmCoreModel.Instance.GetInt32(false)));

            EdmEntityType expandEntityType = new EdmEntityType("NS", "ExpandEntity");
            expandEntityType.AddProperty(new EdmStructuralProperty(expandEntityType, "Id", EdmCoreModel.Instance.GetInt32(false)));
            expandEntityType.AddProperty(new EdmStructuralProperty(expandEntityType, "Name", EdmCoreModel.Instance.GetString(false)));

            entityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() { ContainsTarget = true, Name = "Property1", Target = expandEntityType, TargetMultiplicity = EdmMultiplicity.Many });

            EdmOperation operation = new EdmFunction("NS", "Foo", EdmCoreModel.Instance.GetInt16(true));
            operation.AddParameter("entry", new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(entityType, false))));

            Action<ODataJsonLightOutputContext> test = outputContext =>
            {
                var entry1 = new ODataResource();
                entry1.Properties = new List<ODataProperty>()
                {
                    new ODataProperty() { Name = "ID", Value = 1 },
                };

                var entry2 = new ODataResource();
                entry2.Properties = new List<ODataProperty>()
                {
                    new ODataProperty() { Name = "ID", Value = 1 },
                    new ODataProperty() { Name = "Name", Value = "TestName"}
                };

                var parameterWriter = new ODataJsonLightParameterWriter(outputContext, operation: null);
                parameterWriter.WriteStart();
                var entryWriter = parameterWriter.CreateResourceSetWriter("feed");
                entryWriter.WriteStart(new ODataResourceSet());
                entryWriter.WriteStart(entry1);
                entryWriter.WriteStart(new ODataNestedResourceInfo()
                {
                    Name = "Property1",
                    IsCollection = true
                });
                entryWriter.WriteStart(new ODataResourceSet());
                entryWriter.WriteStart(entry2);
                entryWriter.WriteEnd();
                entryWriter.WriteEnd();
                entryWriter.WriteEnd();
                entryWriter.WriteEnd();
                entryWriter.WriteEnd();
                parameterWriter.WriteEnd();
                parameterWriter.Flush();
            };

            WriteAndValidate(test, "{\"feed\":[{\"ID\":1,\"Property1\":[{\"ID\":1,\"Name\":\"TestName\"}]}]}", writingResponse: false);
        }

        //[Fact]
        //public void AsyncShouldWriteParameterPayloadInRequestWithoutModelAndWithoutFunctionImport()
        //{
        //    Action<ODataJsonLightOutputContext> test = outputContext =>
        //    {
        //        var parameterWriter = new ODataJsonLightParameterWriter(outputContext, operation: null);
        //        parameterWriter.WriteStartAsync().Wait();
        //        parameterWriter.WriteValueAsync("primitive", Guid.Empty).Wait();
        //        var complexWriter = parameterWriter.CreateResourceWriterAsync("complex").Result;
        //        complexWriter.WriteStartAsync(new ODataResource { Properties = new[] { new ODataProperty { Name = "prop1", Value = 1 } } }).Wait();
        //        complexWriter.WriteEndAsync().Wait();
        //        var collectionWriter = parameterWriter.CreateCollectionWriterAsync("collection").Result;
        //        collectionWriter.WriteStartAsync(new ODataCollectionStart()).Wait();
        //        collectionWriter.WriteItemAsync("item1").Wait();
        //        collectionWriter.WriteEndAsync().Wait();
        //        parameterWriter.WriteEndAsync().Wait();
        //        parameterWriter.FlushAsync().Wait();
        //    };

        //    WriteAndValidate(test, "{\"primitive\":\"00000000-0000-0000-0000-000000000000\",\"complex\":{\"prop1\":1},\"collection\":[\"item1\"]}", writingResponse: false, synchronous: false);
        //}

        [Fact]
        public void ShouldWriteParameterPayloadInRequestWithTypeDefinition()
        {
            EdmOperation operation = new EdmFunction("NS", "Foo", EdmCoreModel.Instance.GetInt16(true));
            operation.AddParameter("Length", new EdmTypeDefinitionReference(new EdmTypeDefinition("NS", "Length", EdmPrimitiveTypeKind.Int32), false));
            Action<ODataJsonLightOutputContext> test = outputContext =>
            {
                var parameterWriter = new ODataJsonLightParameterWriter(outputContext, operation);
                parameterWriter.WriteStart();
                parameterWriter.WriteValue("Length", 123);
                parameterWriter.WriteEnd();
                parameterWriter.Flush();
            };

            WriteAndValidate(test, "{\"Length\":123}", writingResponse: false);
        }

        [Theory]
        [InlineData(13, "13")]
        [InlineData(null, "null")]
        public async Task WriteValueParameterAsync(int? luckyNumber, string expected)
        {
            var result = await SetupJsonLightParameterWriterAndRunTestAsync(
                (jsonLightParameterWriter) => jsonLightParameterWriter.WriteValueAsync("LuckyNumber", luckyNumber));

            Assert.Equal($"{{\"LuckyNumber\":{expected}}}", result);
        }

        [Fact]
        public async Task WriteEnumValueParameterAsync()
        {
            var favoriteColor = new ODataEnumValue("Black", "NS.Color");

            var result = await SetupJsonLightParameterWriterAndRunTestAsync(
                (jsonLightParameterWriter) => jsonLightParameterWriter.WriteValueAsync("FavoriteColor", favoriteColor));
        }

        [Fact]
        public async Task CreateCollectionWriterAsync()
        {
            var collectionStart = new ODataCollectionStart
            {
                SerializationInfo = new ODataCollectionStartSerializationInfo
                {
                    CollectionTypeName = "Collection(NS.Color)"
                }
            };

            var result = await SetupJsonLightParameterWriterAndRunTestAsync(
                async (jsonLightParameterWriter) =>
                {
                    var collectionWriter = await jsonLightParameterWriter.CreateCollectionWriterAsync("favoriteColors");
                    await collectionWriter.WriteStartAsync(collectionStart);
                    await collectionWriter.WriteItemAsync(new ODataEnumValue("Black", "NS.Color"));
                    await collectionWriter.WriteItemAsync(new ODataEnumValue("White", "NS.Color"));
                    await collectionWriter.WriteEndAsync();
                },
                writingResponse: false);

            Assert.Equal("{\"favoriteColors\":[\"Black\",\"White\"]}", result);
        }

        [Fact]
        public async Task CreateResourceWriterAsync()
        {
            var resource = new ODataResource
            {
                Properties = new List<ODataProperty>
                {
                    new ODataProperty { Name = "Id", Value = 1 },
                    new ODataProperty { Name = "Name", Value = "Pencil" }
                }
            };

            var result = await SetupJsonLightParameterWriterAndRunTestAsync(
                async (jsonLightParameterWriter) =>
                {
                    var odataWriter = await jsonLightParameterWriter.CreateResourceWriterAsync("product");
                    await odataWriter.WriteStartAsync(resource);
                    await odataWriter.WriteEndAsync();
                },
                writingResponse: false);

            Assert.Equal("{\"product\":{\"Id\":1,\"Name\":\"Pencil\"}}", result);
        }

        [Fact]
        public async Task CreateResourceSetWriterAsync()
        {
            var resourceSet = new ODataResourceSet { };
            var resource1 = new ODataResource
            {
                Properties = new List<ODataProperty>
                {
                    new ODataProperty { Name = "Id", Value = 1 },
                    new ODataProperty { Name = "Name", Value = "Pencil" }
                }
            };

            var resource2 = new ODataResource
            {
                Properties = new List<ODataProperty>
                {
                    new ODataProperty { Name = "Id", Value = 2 },
                    new ODataProperty { Name = "Name", Value = "Paper" }
                }
            };

            var result = await SetupJsonLightParameterWriterAndRunTestAsync(
                async (jsonLightParameterWriter) =>
                {
                    var odataWriter = await jsonLightParameterWriter.CreateResourceSetWriterAsync("products");
                    await odataWriter.WriteStartAsync(new ODataResourceSet());
                    await odataWriter.WriteStartAsync(resource1);
                    await odataWriter.WriteEndAsync();
                    await odataWriter.WriteStartAsync(resource2);
                    await odataWriter.WriteEndAsync();
                    await odataWriter.WriteEndAsync();
                },
                writingResponse: false);

            Assert.Equal("{\"products\":[{\"Id\":1,\"Name\":\"Pencil\"},{\"Id\":2,\"Name\":\"Paper\"}]}", result);
        }

        [Fact]
        public async Task WriteParameterForEdmOperationAsync()
        {
            var productEntityType = new EdmEntityType("NS", "Product");
            productEntityType.AddKeys(productEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            productEntityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);

            var edmFunction = new EdmFunction("NS", "RateProduct", EdmCoreModel.Instance.GetInt32(false));
            edmFunction.AddParameter("product", new EdmEntityTypeReference(productEntityType, false));

            var resource = new ODataResource
            {
                Properties = new List<ODataProperty>
                {
                    new ODataProperty { Name = "Id", Value = 1 },
                    new ODataProperty { Name = "Name", Value = "Pencil" }
                }
            };

            var result = await SetupJsonLightParameterWriterAndRunTestAsync(
                async (jsonLightParameterWriter) =>
                {
                    var odataWriter = await jsonLightParameterWriter.CreateResourceWriterAsync("product");
                    await odataWriter.WriteStartAsync(resource);
                    await odataWriter.WriteEndAsync();
                },
                edmOperation: edmFunction,
                writingResponse: false);

            Assert.Equal("{\"product\":{\"Id\":1,\"Name\":\"Pencil\"}}", result);
        }

        [Fact]
        public async Task WriteParameterAsync_ThrowsExceptionForDuplicatedParameterName()
        {
            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightParameterWriterAndRunTestAsync(
                    async (jsonLightParameterWriter) =>
                    {
                        await jsonLightParameterWriter.WriteValueAsync("LuckyNumber", 7);
                        await jsonLightParameterWriter.WriteValueAsync("LuckyNumber", 13);
                    }));

            Assert.Equal(
                Strings.ODataParameterWriterCore_DuplicatedParameterNameNotAllowed("LuckyNumber"),
                exception.Message);
        }

        [Fact]
        public async Task WriteParameterAsync_ThrowsExceptionForParameterNameNotFoundInEdmOperation()
        {
            var edmFunction = new EdmFunction("NS", "GetRating", EdmCoreModel.Instance.GetInt32(false));
            edmFunction.AddParameter("productId", EdmCoreModel.Instance.GetInt32(false));

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightParameterWriterAndRunTestAsync(
                    (jsonLightParameterWriter) => jsonLightParameterWriter.WriteValueAsync("prodId", 1),
                    edmOperation: edmFunction,
                    writingResponse: false));

            Assert.Equal(
                Strings.ODataParameterWriterCore_ParameterNameNotFoundInOperation("prodId", "GetRating"),
                exception.Message);
        }

        [Fact]
        public async Task WriteParameterAsync_ThrowsExceptionForWriterInCompletedState()
        {
            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightParameterWriterAndRunTestAsync(
                    async (jsonLightParameterWriter) =>
                    {
                        await jsonLightParameterWriter.WriteEndAsync(); // Finish writing
                        await jsonLightParameterWriter.WriteValueAsync("LuckyNumber", 13);
                    }));

            Assert.Equal(
                Strings.ODataParameterWriterCore_CannotWriteInErrorOrCompletedState,
                exception.Message);
        }

        [Fact]
        public async Task WriteParameterAsync_ThrowsExceptionForWriterNotInParameterWriteState()
        {
            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightParameterWriterAndRunTestAsync(
                    async (jsonLightParameterWriter) =>
                    {
                        var resourceSetWriter = await jsonLightParameterWriter.CreateResourceSetWriterAsync("products");
                        // Try to write a parameter - writer should be in ActiveSubWriter state
                        await jsonLightParameterWriter.WriteValueAsync("LuckyNumber", 13);
                    }));

            Assert.Equal(
                Strings.ODataParameterWriterCore_CannotWriteParameter,
                exception.Message);
        }

        [Fact]
        public async Task CreateCollectionWriterAsync_ThrowsExceptionForEntityCollectionType()
        {
            var productEntityType = new EdmEntityType("NS", "Product");
            productEntityType.AddKeys(productEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            productEntityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);

            var edmFunction = new EdmFunction("NS", "RateProduct", EdmCoreModel.Instance.GetInt32(false));
            edmFunction.AddParameter("product", new EdmEntityTypeReference(productEntityType, false));

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightParameterWriterAndRunTestAsync(
                    (jsonLightParameterWriter) => jsonLightParameterWriter.CreateCollectionWriterAsync("product"),
                    edmOperation: edmFunction,
                    writingResponse: false));

            Assert.Equal(
                Strings.ODataParameterWriterCore_CannotCreateCollectionWriterOnNonCollectionTypeKind("product", "Entity"),
                exception.Message);
        }

        [Fact]
        public async Task CreateResourceWriterAsync_ThrowsExceptionForNonStructuredType()
        {
            var colorEnumType = new EdmEnumType("NS", "Color");
            colorEnumType.AddMember(new EdmEnumMember(colorEnumType, "Black", new EdmEnumMemberValue(1)));
            colorEnumType.AddMember(new EdmEnumMember(colorEnumType, "Black", new EdmEnumMemberValue(2)));

            var edmAction = new EdmAction("NS", "SetColor", null);
            edmAction.AddParameter("color", new EdmEnumTypeReference(colorEnumType, false));

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightParameterWriterAndRunTestAsync(
                    (jsonLightParameterWriter) => jsonLightParameterWriter.CreateResourceWriterAsync("color"),
                    edmOperation: edmAction,
                    writingResponse: false));

            Assert.Equal(
                Strings.ODataParameterWriterCore_CannotCreateResourceWriterOnNonEntityOrComplexTypeKind("color", "Enum"),
                exception.Message);
        }

        [Fact]
        public async Task CreateResourceSetWriterAsync_ThrowsExceptionForNonStructuredType()
        {
            var colorEnumType = new EdmEnumType("NS", "Color");
            colorEnumType.AddMember(new EdmEnumMember(colorEnumType, "Black", new EdmEnumMemberValue(1)));
            colorEnumType.AddMember(new EdmEnumMember(colorEnumType, "Black", new EdmEnumMemberValue(2)));

            var edmAction = new EdmAction("NS", "SetColor", null);
            edmAction.AddParameter("color", new EdmEnumTypeReference(colorEnumType, false));

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightParameterWriterAndRunTestAsync(
                    (jsonLightParameterWriter) => jsonLightParameterWriter.CreateResourceSetWriterAsync("color"),
                    edmOperation: edmAction,
                    writingResponse: false));

            Assert.Equal(
                Strings.ODataParameterWriterCore_CannotCreateResourceSetWriterOnNonStructuredCollectionTypeKind("color", "Enum"),
                exception.Message);
        }

        [Fact]
        public async Task WriteParameterAsync_ThrowsExceptionForMissingParameterInParameterPayload()
        {
            var edmFunction = new EdmFunction("NS", "Add", EdmCoreModel.Instance.GetInt32(false));
            edmFunction.AddParameter("a", EdmCoreModel.Instance.GetInt32(false));
            edmFunction.AddParameter("b", EdmCoreModel.Instance.GetInt32(false));

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightParameterWriterAndRunTestAsync(
                    (jsonLightParameterWriter) => jsonLightParameterWriter.WriteValueAsync("a", 1),
                    edmOperation: edmFunction,
                    writingResponse: false));

            Assert.Equal(
                Strings.ODataParameterWriterCore_MissingParameterInParameterPayload("'b'", "Add"),
                exception.Message);
        }

        [Fact]
        public async Task WriteValueParameterAsync_ThrowsExceptionForParameterNameIsNull()
        {
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(
                () => SetupJsonLightParameterWriterAndRunTestAsync(
                    (jsonLightParameterWriter) => jsonLightParameterWriter.WriteValueAsync(null, 13)));
        }

        [Fact]
        public async Task CreateCollectionWriterAsync_ThrowsExceptionForParameterNameIsNull()
        {
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(
                () => SetupJsonLightParameterWriterAndRunTestAsync(
                    (jsonLightParameterWriter) => jsonLightParameterWriter.CreateCollectionWriterAsync(null)));
        }

        [Fact]
        public async Task CreateResourceWriterAsync_ThrowsExceptionForParameterNameIsNull()
        {
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(
                () => SetupJsonLightParameterWriterAndRunTestAsync(
                    (jsonLightParameterWriter) => jsonLightParameterWriter.CreateResourceWriterAsync(null)));
        }

        [Fact]
        public async Task CreateResourceSetWriterAsync_ThrowsExceptionForParameterNameIsNull()
        {
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(
                () => SetupJsonLightParameterWriterAndRunTestAsync(
                    (jsonLightParameterWriter) => jsonLightParameterWriter.CreateResourceSetWriterAsync(null)));
        }

        [Fact]
        public async Task WriteStartAsync_ThrowsExceptionForWriterInCanWriteParameterState()
        {
            var exception = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    var jsonLightOutputContext = CreateJsonLightOutputContext(new MemoryStream(), writingResponse: false, synchronous: false);
                    var jsonLightParameterWriter = new ODataJsonLightParameterWriter(jsonLightOutputContext, operation: null);

                    await jsonLightParameterWriter.WriteStartAsync();
                    await jsonLightParameterWriter.WriteStartAsync();
                });

            Assert.Equal(Strings.ODataParameterWriterCore_CannotWriteStart, exception.Message);
        }

        [Fact]
        public async Task WriteEndAsync_ThrowsExceptionForWriterInStartState()
        {
            var exception = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    var jsonLightOutputContext = CreateJsonLightOutputContext(new MemoryStream(), writingResponse: false, synchronous: false);
                    var jsonLightParameterWriter = new ODataJsonLightParameterWriter(jsonLightOutputContext, operation: null);

                    await jsonLightParameterWriter.WriteEndAsync();
                });

            Assert.Equal(Strings.ODataParameterWriterCore_CannotWriteEnd, exception.Message);
        }

        /// <summary>
        /// Sets up an ODataJsonLightParameterWriter,
        /// then runs the given test code asynchronously,
        /// then flushes and reads the stream back as a string for customized verification.
        /// </summary>
        private async Task<string> SetupJsonLightParameterWriterAndRunTestAsync(
            Func<ODataJsonLightParameterWriter, Task> func,
            IEdmOperation edmOperation = null,
            bool writingResponse = true)
        {
            var stream = new MemoryStream();

            var jsonLightOutputContext = CreateJsonLightOutputContext(
                stream,
                /*writingResponse*/ writingResponse,
                /*synchronous*/ false);

            var jsonLightParameterWriter = new ODataJsonLightParameterWriter(
                jsonLightOutputContext,
                edmOperation);

            await jsonLightParameterWriter.WriteStartAsync();
            await func(jsonLightParameterWriter);
            await jsonLightParameterWriter.WriteEndAsync();

            stream.Position = 0;
            return await new StreamReader(stream).ReadToEndAsync();
        }

        private static void WriteAndValidate(Action<ODataJsonLightOutputContext> test, string expectedPayload, bool writingResponse = true, bool synchronous = true)
        {
            MemoryStream stream = new MemoryStream();
            var outputContext = CreateJsonLightOutputContext(stream, writingResponse, synchronous);
            test(outputContext);
            ValidateWrittenPayload(stream, expectedPayload);
        }

        private static void ValidateWrittenPayload(MemoryStream stream, string expectedPayload)
        {
            stream.Seek(0, SeekOrigin.Begin);
            string payload = (new StreamReader(stream)).ReadToEnd();
            Assert.Equal(expectedPayload, payload);
        }

        private static ODataJsonLightOutputContext CreateJsonLightOutputContext(MemoryStream stream, bool writingResponse = true, bool synchronous = true)
        {
            var settings = new ODataMessageWriterSettings { Version = ODataVersion.V4 };
            settings.SetServiceDocumentUri(new Uri("http://odata.org/test/"));

            var messageInfo = new ODataMessageInfo
            {
                MessageStream = new NonDisposingStream(stream),
                MediaType = new ODataMediaType("application", "json"),
                Encoding = Encoding.UTF8,
                IsResponse = writingResponse,
                IsAsync = !synchronous,
                Model = EdmCoreModel.Instance
            };

            return new ODataJsonLightOutputContext(messageInfo, settings);
        }
    }
}

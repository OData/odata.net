//---------------------------------------------------------------------
// <copyright file="ODataJsonLightParameterWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Core.JsonLight;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.JsonLight
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
                parameterWriter.WriteValue("complex", new ODataComplexValue { Properties = new[] { new ODataProperty { Name = "prop1", Value = 1 } } });
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
                var entry = new ODataEntry();
                entry.Properties = new List<ODataProperty>() { new ODataProperty() { Name = "ID", Value = 1 } };

                var parameterWriter = new ODataJsonLightParameterWriter(outputContext, operation: null);
                parameterWriter.WriteStart();
                var entryWriter = parameterWriter.CreateEntryWriter("entry");
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
                var entry = new ODataEntry();

                var complex = new ODataComplexValue() {Properties = new List<ODataProperty>() {new ODataProperty() {Name = "Name", Value = "ComplexName"}}};
                entry.Properties = new List<ODataProperty>() {new ODataProperty() {Name = "ID", Value = 1}, new ODataProperty() {Name = "complexProperty", Value = complex}};
                var parameterWriter = new ODataJsonLightParameterWriter(outputContext, operation: null);
                parameterWriter.WriteStart();
                var entryWriter = parameterWriter.CreateEntryWriter("entry");
                entryWriter.WriteStart(entry);
                entryWriter.WriteEnd();
                parameterWriter.WriteValue("complex", complex);
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
                var entry = new ODataEntry();
                entry.Properties = new List<ODataProperty>() { new ODataProperty() { Name = "ID", Value = 1 } };

                var parameterWriter = new ODataJsonLightParameterWriter(outputContext, operation: null);
                parameterWriter.WriteStart();
                var entryWriter = parameterWriter.CreateFeedWriter("feed");
                entryWriter.WriteStart(new ODataFeed());
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
                var entry = new ODataEntry();
                entry.Properties = new List<ODataProperty>() { new ODataProperty() { Name = "ID", Value = 1 } };

                var parameterWriter = new ODataJsonLightParameterWriter(outputContext, operation);
                parameterWriter.WriteStart();
                var entryWriter = parameterWriter.CreateEntryWriter("entry");
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
                var entry = new ODataEntry();
                entry.Properties = new List<ODataProperty>() { new ODataProperty() { Name = "ID", Value = 1 }, new ODataProperty() { Name = "DynamicProperty", Value = "DynamicValue" } };

                var parameterWriter = new ODataJsonLightParameterWriter(outputContext, operation);
                parameterWriter.WriteStart();
                var entryWriter = parameterWriter.CreateEntryWriter("entry");
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
                var entry = new ODataEntry();
                entry.Properties = new List<ODataProperty>() { new ODataProperty() { Name = "ID", Value = 1 } };

                var parameterWriter = new ODataJsonLightParameterWriter(outputContext, operation: null);
                parameterWriter.WriteStart();
                var entryWriter = parameterWriter.CreateFeedWriter("feed");
                entryWriter.WriteStart(new ODataFeed());
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
                var entry = new ODataEntry();
                entry.Properties = new List<ODataProperty>() { new ODataProperty() { Name = "ID", Value = 1 } };

                var entry2 = new ODataEntry()
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
                var entryWriter = parameterWriter.CreateFeedWriter("feed");
                entryWriter.WriteStart(new ODataFeed());
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

        [Fact(Skip = "This test currently fails.")]
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

                var entryWriter = parameterWriter.CreateEntryWriter("entry");
                entryWriter.WriteStart((ODataEntry)null);
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
                var entry = new ODataEntry()
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

                var entryWriter = parameterWriter.CreateEntryWriter("entry");
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
                var entry = new ODataEntry()
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
                var entryWriter = parameterWriter.CreateFeedWriter("feed");
                entryWriter.WriteStart(new ODataFeed());
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

            entityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() {Name = "Property1", Target = expandEntityType, TargetMultiplicity = EdmMultiplicity.One});

            EdmOperation operation = new EdmFunction("NS", "Foo", EdmCoreModel.Instance.GetInt16(true));
            operation.AddParameter("entry", new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(entityType, false))));

            Action<ODataJsonLightOutputContext> test = outputContext =>
            {
                var entry1 = new ODataEntry();
                entry1.Properties = new List<ODataProperty>()
                {
                    new ODataProperty() { Name = "ID", Value = 1 }, 
                };

                var entry2 = new ODataEntry();
                entry2.Properties = new List<ODataProperty>()
                {
                    new ODataProperty() { Name = "ID", Value = 1 }, 
                    new ODataProperty() { Name = "Name", Value = "TestName"}
                };

                var parameterWriter = new ODataJsonLightParameterWriter(outputContext, operation: null);
                parameterWriter.WriteStart();
                var entryWriter = parameterWriter.CreateFeedWriter("feed");
                entryWriter.WriteStart(new ODataFeed());
                entryWriter.WriteStart(entry1);
                entryWriter.WriteStart(new ODataNavigationLink()
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
                var entry1 = new ODataEntry();
                entry1.Properties = new List<ODataProperty>()
                {
                    new ODataProperty() { Name = "ID", Value = 1 }, 
                };

                var entry2 = new ODataEntry();
                entry2.Properties = new List<ODataProperty>()
                {
                    new ODataProperty() { Name = "ID", Value = 1 }, 
                    new ODataProperty() { Name = "Name", Value = "TestName"}
                };

                var parameterWriter = new ODataJsonLightParameterWriter(outputContext, operation: null);
                parameterWriter.WriteStart();
                var entryWriter = parameterWriter.CreateFeedWriter("feed");
                entryWriter.WriteStart(new ODataFeed());
                entryWriter.WriteStart(entry1);
                entryWriter.WriteStart(new ODataNavigationLink()
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
                var entry1 = new ODataEntry();
                entry1.Properties = new List<ODataProperty>()
                {
                    new ODataProperty() { Name = "ID", Value = 1 }, 
                };

                var entry2 = new ODataEntry();
                entry2.Properties = new List<ODataProperty>()
                {
                    new ODataProperty() { Name = "ID", Value = 1 }, 
                    new ODataProperty() { Name = "Name", Value = "TestName"}
                };

                var parameterWriter = new ODataJsonLightParameterWriter(outputContext, operation: null);
                parameterWriter.WriteStart();
                var entryWriter = parameterWriter.CreateFeedWriter("feed");
                entryWriter.WriteStart(new ODataFeed());
                entryWriter.WriteStart(entry1);
                entryWriter.WriteStart(new ODataNavigationLink()
                {
                    Name = "Property1",
                    IsCollection = true
                });
                entryWriter.WriteStart(new ODataFeed());
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
                var entry1 = new ODataEntry();
                entry1.Properties = new List<ODataProperty>()
                {
                    new ODataProperty() { Name = "ID", Value = 1 }, 
                };

                var entry2 = new ODataEntry();
                entry2.Properties = new List<ODataProperty>()
                {
                    new ODataProperty() { Name = "ID", Value = 1 }, 
                    new ODataProperty() { Name = "Name", Value = "TestName"}
                };

                var parameterWriter = new ODataJsonLightParameterWriter(outputContext, operation: null);
                parameterWriter.WriteStart();
                var entryWriter = parameterWriter.CreateFeedWriter("feed");
                entryWriter.WriteStart(new ODataFeed());
                entryWriter.WriteStart(entry1);
                entryWriter.WriteStart(new ODataNavigationLink()
                {
                    Name = "Property1",
                    IsCollection = true
                });
                entryWriter.WriteStart(new ODataFeed());
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
        public void AsyncShouldWriteParameterPayloadInRequestWithoutModelAndWithoutFunctionImport()
        {
            Action<ODataJsonLightOutputContext> test = outputContext =>
            {
                var parameterWriter = new ODataJsonLightParameterWriter(outputContext, operation: null);
                parameterWriter.WriteStartAsync().Wait();
                parameterWriter.WriteValueAsync("primitive", Guid.Empty).Wait();
                parameterWriter.WriteValueAsync("complex", new ODataComplexValue { Properties = new[] { new ODataProperty { Name = "prop1", Value = 1 } } }).Wait();
                var collectionWriter = parameterWriter.CreateCollectionWriterAsync("collection").Result;
                collectionWriter.WriteStartAsync(new ODataCollectionStart()).Wait();
                collectionWriter.WriteItemAsync("item1").Wait();
                collectionWriter.WriteEndAsync().Wait();
                parameterWriter.WriteEndAsync().Wait();
                parameterWriter.FlushAsync().Wait();
            };

            WriteAndValidate(test, "{\"primitive\":\"00000000-0000-0000-0000-000000000000\",\"complex\":{\"prop1\":1},\"collection\":[\"item1\"]}", writingResponse: false, synchronous: false);
        }

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
            payload.Should().Be(expectedPayload);
        }

        private static ODataJsonLightOutputContext CreateJsonLightOutputContext(MemoryStream stream, bool writingResponse = true, bool synchronous = true)
        {
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings { Version = ODataVersion.V4 };
            settings.SetServiceDocumentUri(new Uri("http://odata.org/test/"));

            return new ODataJsonLightOutputContext(
                ODataFormat.Json,
                new NonDisposingStream(stream),
                new ODataMediaType("application", "json"),
                Encoding.UTF8,
                settings,
                writingResponse,
                synchronous,
                EdmCoreModel.Instance,
                /*urlResolver*/ null);
        }
    }
}
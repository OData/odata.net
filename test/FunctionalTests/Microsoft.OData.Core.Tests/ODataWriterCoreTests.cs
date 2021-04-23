//---------------------------------------------------------------------
// <copyright file="ODataWriterCoreTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests
{
    /// <summary>
    /// Unit tests for ODataWriterCore.
    /// TODO: These unit tests do not provide complete coverage of ODataWriterCoreTests.
    /// </summary>
    public class ODataWriterCoreTests
    {
        [Fact]
        public void ValidateWriteMethodGroup()
        {
            // setup model
            var model = new EdmModel();
            var complexType = new EdmComplexType("NS", "ComplexType");
            complexType.AddStructuralProperty("PrimitiveProperty1", EdmPrimitiveTypeKind.Int64);
            complexType.AddStructuralProperty("PrimitiveProperty2", EdmPrimitiveTypeKind.Int64);
            var entityType = new EdmEntityType("NS", "EntityType", null, false, true);
            entityType.AddKeys(
                entityType.AddStructuralProperty("PrimitiveProperty", EdmPrimitiveTypeKind.Int64));
            var container = new EdmEntityContainer("NS", "Container");
            var entitySet = container.AddEntitySet("EntitySet", entityType);
            model.AddElements(new IEdmSchemaElement[] { complexType, entityType, container });

            // write payload using new API
            string str1;
            {
                // setup
                var stream = new MemoryStream();
                var message = new InMemoryMessage { Stream = stream };
                message.SetHeader("Content-Type", "application/json;odata.metadata=full");
                var settings = new ODataMessageWriterSettings
                {
                    ODataUri = new ODataUri
                    {
                        ServiceRoot = new Uri("http://svc/")
                    },
                };
                var writer = new ODataMessageWriter((IODataResponseMessage)message, settings, model);

                var entitySetWriter = writer.CreateODataResourceSetWriter(entitySet);
                entitySetWriter.Write(new ODataResourceSet(), () => entitySetWriter
                    .Write(new ODataResource
                    {
                        Properties = new[] { new ODataProperty { Name = "PrimitiveProperty", Value = 1L } }
                    })
                    .Write(new ODataResource
                    {
                        Properties = new[] { new ODataProperty { Name = "PrimitiveProperty", Value = 2L } }
                    }, () => entitySetWriter
                        .Write(new ODataNestedResourceInfo { Name = "DynamicNavProperty" })
                        .Write(new ODataNestedResourceInfo
                        {
                            Name = "DynamicCollectionProperty",
                            IsCollection = true
                        }, () => entitySetWriter
                            .Write(new ODataResourceSet { TypeName = "Collection(NS.ComplexType)" })))
                );
                str1 = Encoding.UTF8.GetString(stream.ToArray());
            }
            // write payload using old API
            string str2;
            {
                // setup
                var stream = new MemoryStream();
                var message = new InMemoryMessage { Stream = stream };
                message.SetHeader("Content-Type", "application/json;odata.metadata=full");
                var settings = new ODataMessageWriterSettings
                {
                    ODataUri = new ODataUri
                    {
                        ServiceRoot = new Uri("http://svc/")
                    },
                };
                var writer = new ODataMessageWriter((IODataResponseMessage)message, settings, model);

                var entitySetWriter = writer.CreateODataResourceSetWriter(entitySet);
                entitySetWriter.WriteStart(new ODataResourceSet());
                    entitySetWriter.WriteStart(new ODataResource
                    {
                        Properties = new[] { new ODataProperty { Name = "PrimitiveProperty", Value = 1L } }
                    });
                    entitySetWriter.WriteEnd();
                    entitySetWriter.WriteStart(new ODataResource
                    {
                        Properties = new[] { new ODataProperty { Name = "PrimitiveProperty", Value = 2L } }
                    });
                    entitySetWriter.WriteStart(new ODataNestedResourceInfo { Name = "DynamicNavProperty" });
                    entitySetWriter.WriteEnd();
                    entitySetWriter.WriteStart(new ODataNestedResourceInfo
                    {
                        Name = "DynamicCollectionProperty",
                        IsCollection = true
                    });
                        entitySetWriter.WriteStart(new ODataResourceSet { TypeName = "Collection(NS.ComplexType)" });
                        entitySetWriter.WriteEnd();
                    entitySetWriter.WriteEnd();    
                    entitySetWriter.WriteEnd();
                entitySetWriter.WriteEnd();
                str2 = Encoding.UTF8.GetString(stream.ToArray());
            }
            Assert.Equal(str1, str2);
        }

        [Fact]
        public void ValidateEntityTypeShouldAlwaysReturnSpecifiedTypeName()
        {
            var model = CreateTestModel();
            var set = model.EntityContainer.FindEntitySet("Objects");
            var objectType = (IEdmEntityType)model.FindDeclaredType("DefaultNamespace.Object");
            var coreWriter = CreateODataWriterCore(ODataFormat.Json, true, model, set, objectType, false);

            var entry = new ODataResource() { TypeName = "DefaultNamespace.Person" };
            var entityType = coreWriter.GetEntityType2(entry);
            Assert.Same(model.FindDeclaredType("DefaultNamespace.Person"), entityType);
        }

        [Fact]
        public void ValidateEntityTypeShouldReturnEntityTypeIfTypeNameNonExistant()
        {
            var model = CreateTestModel();
            var set = model.EntityContainer.FindEntitySet("Objects");
            var objectType = (IEdmEntityType)model.FindDeclaredType("DefaultNamespace.Object");

            var coreWriter = CreateODataWriterCore(ODataFormat.Json, true, model, set, objectType, false);

            var entry = new ODataResource();
            var entityType = coreWriter.GetEntityType2(entry);
            Assert.Same(objectType, entityType);
        }

        [Fact]
        public void ValidateEntityTypeShouldReturnEntityTypeOfSet()
        {
            var model = CreateTestModel();
            var peopleSet = model.EntityContainer.FindEntitySet("People");
            var personType = (IEdmEntityType)model.FindDeclaredType("DefaultNamespace.Person");

            var coreWriter = CreateODataWriterCore(ODataFormat.Json, true, model, peopleSet, null, false);

            var entry = new ODataResource();
            var entityType = coreWriter.GetEntityType2(entry);
            Assert.Same(personType, entityType);
        }

        [Fact]
        public void ValidateNoMetadataShouldThrow()
        {
            var model = CreateTestModel();
            var coreWriter = CreateODataWriterCore(ODataFormat.Json, true, model, null, null, false);

            var entry = new ODataResource();
            Action test = () => coreWriter.GetEntityType2(entry);

            test.Throws<ODataException>(Strings.WriterValidationUtils_MissingTypeNameWithMetadata);
        }

        private static EdmModel CreateTestModel()
        {
            var objectType = new EdmEntityType("DefaultNamespace", "Object");
            var personType = new EdmEntityType("DefaultNamespace", "Person", objectType);
            var container = new EdmEntityContainer("DefaultNamespace", "Container");
            container.AddEntitySet("Objects", objectType);
            container.AddEntitySet("People", personType);
            var model = new EdmModel();
            model.AddElement(objectType);
            model.AddElement(personType);
            model.AddElement(container);
            return model;
        }

        private static TestODataWriterCore CreateODataWriterCore(ODataFormat format, bool writingResponse, IEdmModel model, IEdmEntitySet writerSet, IEdmEntityType writerEntityType, bool writeFeed)
        {
            var resolver = new TestUrlResolver();
            var settings = new ODataMessageWriterSettings { EnableMessageStreamDisposal = false, Version = ODataVersion.V4 };
            settings.SetServiceDocumentUri(new Uri("http://example.com"));

            var outputContext = new TestODataOutputContext(format, settings, writingResponse, false, model, resolver);
            return new TestODataWriterCore(outputContext, writerSet, writerEntityType, writeFeed);
        }

        internal class TestODataWriterCore : ODataWriterCore
        {
            public TestODataWriterCore(ODataOutputContext outputContext, IEdmEntitySet navigationSource, IEdmEntityType entityType, bool writingFeed) :
                base(outputContext, navigationSource, entityType, writingFeed)
            {
            }

            public IEdmStructuredType GetEntityType2(ODataResource entry)
            {
                return this.GetResourceType(entry);
            }

            #region Non-implemented abstract methods
            protected override void VerifyNotDisposed()
            {
                throw new NotImplementedException();
            }

            protected override void FlushSynchronously()
            {
                throw new NotImplementedException();
            }

            protected override Task FlushAsynchronously()
            {
                throw new NotImplementedException();
            }

            protected override void StartPayload()
            {
                throw new NotImplementedException();
            }

            protected override void StartResource(ODataResource entry)
            {
                throw new NotImplementedException();
            }

            protected override void EndResource(ODataResource entry)
            {
                throw new NotImplementedException();
            }

            protected override void StartResourceSet(ODataResourceSet resourceCollection)
            {
                throw new NotImplementedException();
            }

            protected override void EndPayload()
            {
                throw new NotImplementedException();
            }

            protected override void EndResourceSet(ODataResourceSet resourceCollection)
            {
                throw new NotImplementedException();
            }

            protected override void WriteDeferredNestedResourceInfo(ODataNestedResourceInfo navigationLink)
            {
                throw new NotImplementedException();
            }

            protected override void StartNestedResourceInfoWithContent(ODataNestedResourceInfo navigationLink)
            {
                throw new NotImplementedException();
            }

            protected override void EndNestedResourceInfoWithContent(ODataNestedResourceInfo navigationLink)
            {
                throw new NotImplementedException();
            }

            protected override void WriteEntityReferenceInNavigationLinkContent(ODataNestedResourceInfo parentNavigationLink, ODataEntityReferenceLink entityReferenceLink)
            {
                throw new NotImplementedException();
            }

            protected override ODataWriterCore.ResourceSetScope CreateResourceSetScope(ODataResourceSet resourceCollection, IEdmNavigationSource navigationSource, IEdmType itemType, bool skipWriting, SelectedPropertiesNode selectedProperties, ODataUri odataUri, bool isUndeclared)
            {
                throw new NotImplementedException();
            }

            protected override ODataWriterCore.ResourceScope CreateResourceScope(ODataResource entry, IEdmNavigationSource navigationSource, IEdmStructuredType resourceType, bool skipWriting, SelectedPropertiesNode selectedProperties, ODataUri odataUri, bool isUndeclared)
            {
                throw new NotImplementedException();
            }

            protected override Task StartPayloadAsync()
            {
                throw new NotImplementedException();
            }

            protected override Task EndPayloadAsync()
            {
                throw new NotImplementedException();
            }

            protected override Task StartResourceAsync(ODataResource resource)
            {
                throw new NotImplementedException();
            }

            protected override Task EndResourceAsync(ODataResource resource)
            {
                throw new NotImplementedException();
            }

            protected override Task StartResourceSetAsync(ODataResourceSet resourceSet)
            {
                throw new NotImplementedException();
            }

            protected override Task EndResourceSetAsync(ODataResourceSet resourceSet)
            {
                throw new NotImplementedException();
            }

            protected override Task WriteDeferredNestedResourceInfoAsync(ODataNestedResourceInfo nestedResourceInfo)
            {
                throw new NotImplementedException();
            }

            protected override Task StartNestedResourceInfoWithContentAsync(ODataNestedResourceInfo nestedResourceInfo)
            {
                throw new NotImplementedException();
            }

            protected override Task EndNestedResourceInfoWithContentAsync(ODataNestedResourceInfo nestedResourceInfo)
            {
                throw new NotImplementedException();
            }

            protected override Task WriteEntityReferenceInNavigationLinkContentAsync(ODataNestedResourceInfo parentNestedResourceInfo, ODataEntityReferenceLink entityReferenceLink)
            {
                throw new NotImplementedException();
            }
            #endregion
        }

        internal class TestODataOutputContext : ODataOutputContext
        {
            public TestODataOutputContext(ODataFormat format, ODataMessageWriterSettings messageWriterSettings, bool writingResponse, bool synchronous, IEdmModel model, IODataPayloadUriConverter urlResolver)
                : base(format,
                    new ODataMessageInfo
                    {
                        IsAsync = !synchronous,
                        IsResponse = writingResponse,
                        Model = model,
                        PayloadUriConverter = urlResolver
                    }, messageWriterSettings)
            {
            }
        }

        internal class TestUrlResolver : IODataPayloadUriConverter
        {
            public Func<Uri, Uri, Uri> ResolveUrlFunc { get; set; }
            public Uri ConvertPayloadUri(Uri baseUri, Uri payloadUri)
            {
                return this.ResolveUrlFunc(baseUri, payloadUri);
            }
        }
    }
}

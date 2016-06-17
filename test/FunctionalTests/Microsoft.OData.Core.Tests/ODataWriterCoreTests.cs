//---------------------------------------------------------------------
// <copyright file="ODataWriterCoreTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;
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
        public void ValidateEntityTypeShouldAlwaysReturnSpecifiedTypeName()
        {
            var model = CreateTestModel();
            var set = model.EntityContainer.FindEntitySet("Objects");
            var objectType = (IEdmEntityType)model.FindDeclaredType("DefaultNamespace.Object");
            var coreWriter = CreateODataWriterCore(ODataFormat.Json, true, model, set, objectType, false);

            var entry = new ODataResource() { TypeName = "DefaultNamespace.Person" };
            var entityType = coreWriter.GetEntityType2(entry);
            entityType.Should().BeSameAs(model.FindDeclaredType("DefaultNamespace.Person"));
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
            entityType.Should().BeSameAs(objectType);
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
            entityType.Should().BeSameAs(personType);
        }

        [Fact]
        public void ValidateNoMetadataShouldThrow()
        {
            var model = CreateTestModel();
            var coreWriter = CreateODataWriterCore(ODataFormat.Json, true, model, null, null, false);

            var entry = new ODataResource();
            Action test = () => coreWriter.GetEntityType2(entry);

            test.ShouldThrow<ODataException>().WithMessage(Strings.WriterValidationUtils_MissingTypeNameWithMetadata);
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

            protected override ODataWriterCore.ResourceSetScope CreateResourceSetScope(ODataResourceSet resourceCollection, IEdmNavigationSource navigationSource, IEdmStructuredType resourceType, bool skipWriting, SelectedPropertiesNode selectedProperties, ODataUri odataUri)
            {
                throw new NotImplementedException();
            }

            protected override ODataWriterCore.ResourceScope CreateResourceScope(ODataResource entry, IEdmNavigationSource navigationSource, IEdmStructuredType resourceType, bool skipWriting, SelectedPropertiesNode selectedProperties, ODataUri odataUri)
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

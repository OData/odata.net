//---------------------------------------------------------------------
// <copyright file="ODataJsonLightWriterIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;
using ErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.IntegrationTests.Writer.JsonLight
{
    public class ODataJsonLightWriterIntegrationTests
    {
        [Fact]
        public void ShouldBeAbleToWriteEntryWithIdOnly()
        {
            IEdmEntityType entityType = GetEntityType();
            IEdmEntitySet entitySet = GetEntitySet(entityType);
            const string expected = "{\"@odata.id\":\"http://test.org/EntitySet('1')\"}";
            var actual = WriteJsonLightEntry(true, null, false, new ODataEntry() {Id = new Uri("http://test.org/EntitySet('1')")}, entitySet, entityType);
            actual.Should().Be(expected);
        }

        [Fact]
        public void ShouldBeAbleToWriteEntryInRequestWithoutSpecifyingEntitySetOrMetadataDocumentUri()
        {
            IEdmEntityType entityType = GetEntityType();
            IEdmEntitySet entitySet = GetEntitySet(entityType);
            const string expected = "{}";
            var actual = WriteJsonLightEntry(true, null, false, new ODataEntry(), entitySet, entityType);
            actual.Should().Be(expected);
        }

        [Fact]
        public void ShouldNotBeAbleToWriteEntryInResponseWithoutSpecifyingEntitySet()
        {
            IEdmEntityType entityType = GetEntityType();
            IEdmEntitySet entitySet = GetEntitySet(entityType);
            Action writeEmptyEntry = () => WriteJsonLightEntry(false, new Uri("http://temp.org/"), false, new ODataEntry(), entitySet, entityType);
            writeEmptyEntry.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataFeedAndEntryTypeContext_MetadataOrSerializationInfoMissing);
        }

        [Fact]
        public void ShouldNotBeAbleToWriteEntryInResponseWithoutSpecifyingMetadataDocumentUri()
        {
            IEdmEntityType entityType = GetEntityType();
            IEdmEntitySet entitySet = GetEntitySet(entityType);
            Action writeEmptyEntry = () => WriteJsonLightEntry(false, null, true, new ODataEntry(), entitySet, entityType);
            writeEmptyEntry.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataOutputContext_MetadataDocumentUriMissing);
        }

        [Fact]
        public void ShouldBeAbleToWriteTransientEntry()
        {
            IEdmEntityType entityType = GetEntityType();
            IEdmEntitySet entitySet = GetEntitySet(entityType);
            ODataEntry transientEntry = new ODataEntry() { IsTransient = true };
            var actual = WriteJsonLightEntry(true, null, false, transientEntry, entitySet, entityType);
            actual.Should().Contain("\"@odata.id\":null");
        }

        [Fact]
        public void WriteContainedEntitySet()
        {
            EdmModel model = new EdmModel();
            EdmEntityType entityType = GetEntityType();
            model.AddElement(entityType);
            IEdmNavigationSource containedEntitySet = GetContainedEntitySet(model, entityType);
            var requestUri = new Uri("http://temp.org/FakeSet('parent')/nav");
            var odataUri = new ODataUri { RequestUri = requestUri };
            odataUri.Path = new ODataUriParser(model, new Uri("http://temp.org/"), requestUri).ParsePath();

            ODataEntry entry = new ODataEntry() { Properties = new[] { new ODataProperty { Name = "Key", Value = "son" }, } };
            var actual = WriteJsonLightEntry(
                isRequest: false,
                serviceDocumentUri: new Uri("http://temp.org/"),
                specifySet: true,
                odataEntry: entry,
                entitySet: containedEntitySet,
                entityType: containedEntitySet.Type as IEdmEntityType,
                odataUri: odataUri);
            actual.Should().Contain("\"@odata.id\":\"FakeSet('parent')/nav('son')\"");
        }

        [Fact]
        public void WriteContainedEntitySetWithoutODataUriShouldThrow()
        {
            EdmModel model = new EdmModel();
            EdmEntityType entityType = GetEntityType();
            model.AddElement(entityType);
            IEdmNavigationSource containedEntitySet = GetContainedEntitySet(model, entityType);

            ODataEntry entry = new ODataEntry() { Properties = new[] { new ODataProperty { Name = "Key", Value = "son" }, } };
            Action writeContainedEntry = () => WriteJsonLightEntry(
                isRequest: false,
                serviceDocumentUri: new Uri("http://temp.org/"),
                specifySet: true,
                odataEntry: entry,
                entitySet: containedEntitySet,
                entityType: containedEntitySet.Type as IEdmEntityType,
                odataUri: null);
            writeContainedEntry.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataMetadataBuilder_MissingODataUri);
        }

        [Fact]
        public void EditLinkShouldNotBeWrittenIfItsReadonlyEntry()
        {
            IEdmEntityType entityType = GetEntityType();
            IEdmEntitySet entitySet = GetEntitySet(entityType);

            var entry = new ODataEntry
            {
                Id = new Uri("http://test.org/EntitySet('1')"),
                ReadLink = new Uri("http://test.org/EntitySet('1')/read")
            };

            string actual = WriteJsonLightEntry(
                isRequest: false,
                serviceDocumentUri: new Uri("http://test.org"),
                specifySet: true,
                odataEntry: entry,
                entitySet: entitySet,
                entityType: entityType
                );

            actual.Should().Contain("\"@odata.readLink\":\"http://test.org/EntitySet('1')/read\"");
            actual.Should().NotContain("\"@odata.editLink\"");
        }

        [Fact]
        public void ReadLinkShouldNotBeOmittedWhenNotIdentical()
        {
            IEdmEntityType entityType = GetEntityType();
            IEdmEntitySet entitySet = GetEntitySet(entityType);

            var entry = new ODataEntry
            {
                Id = new Uri("http://test.org/EntitySet('1')"),
                EditLink = new Uri("http://test.org/EntitySet('1')/edit"),
                ReadLink = new Uri("http://test.org/EntitySet('1')/read")
            };

            string actual = WriteJsonLightEntry(
                isRequest: false,
                serviceDocumentUri: new Uri("http://test.org"),
                specifySet: true,
                odataEntry: entry,
                entitySet: entitySet,
                entityType: entityType
                );

            actual.Should().Contain("\"@odata.readLink\":\"http://test.org/EntitySet('1')/read\"");
            actual.Should().Contain("\"@odata.editLink\":\"http://test.org/EntitySet('1')/edit\"");
        }

        [Fact]
        public void ReadLinkShouldBeOmittedWhenIdentical()
        {
            IEdmEntityType entityType = GetEntityType();
            IEdmEntitySet entitySet = GetEntitySet(entityType);

            var entry = new ODataEntry
            {
                Id = new Uri("http://test.org/EntitySet('1')"),
                EditLink = new Uri("http://test.org/EntitySet('1')"),
                ReadLink = new Uri("http://test.org/EntitySet('1')")
            };

            string actual = WriteJsonLightEntry(
                isRequest: false,
                serviceDocumentUri: new Uri("http://test.org"),
                specifySet: true,
                odataEntry: entry,
                entitySet: entitySet,
                entityType: entityType
                );

            actual.Should().Contain("\"@odata.editLink\":\"http://test.org/EntitySet('1')\"");
            actual.Should().NotContain("\"@odata.readLink\"");
        }

        private static string WriteJsonLightEntry(bool isRequest, Uri serviceDocumentUri, bool specifySet, ODataEntry odataEntry, IEdmNavigationSource entitySet, IEdmEntityType entityType)
        {
            return WriteJsonLightEntry(isRequest, serviceDocumentUri, specifySet, odataEntry, entitySet, entityType, odataUri: null);
        }

        private static string WriteJsonLightEntry(bool isRequest, Uri serviceDocumentUri, bool specifySet,
            ODataEntry odataEntry, IEdmNavigationSource entitySet, IEdmEntityType entityType, ODataUri odataUri)
        {
            var model = new EdmModel();
            model.AddElement(new EdmEntityContainer("Fake", "Container_sub"));
            var stream = new MemoryStream();
            var message = new InMemoryMessage { Stream = stream };

            var settings = new ODataMessageWriterSettings { Version = ODataVersion.V4, AutoComputePayloadMetadataInJson = true };
            settings.ODataUri = odataUri;
            settings.SetServiceDocumentUri(serviceDocumentUri);

            settings.SetContentType(ODataFormat.Json);
            settings.SetContentType("application/json;odata.metadata=full", null);

            ODataMessageWriter messageWriter;
            if (isRequest)
            {
                messageWriter = new ODataMessageWriter((IODataRequestMessage)message, settings, TestUtils.WrapReferencedModelsToMainModel("Fake", "Container", model));
            }
            else
            {
                messageWriter = new ODataMessageWriter((IODataResponseMessage)message, settings, TestUtils.WrapReferencedModelsToMainModel("Fake", "Container", model));
            }

            var entryWriter = messageWriter.CreateODataEntryWriter(specifySet ? entitySet : null, entityType);
            entryWriter.WriteStart(odataEntry);
            entryWriter.WriteEnd();
            entryWriter.Flush();

            var actual = Encoding.UTF8.GetString(stream.ToArray());
            return actual;
        }

        private static EdmEntitySet GetEntitySet(IEdmEntityType type)
        {
            EdmEntitySet entitySet = new EdmEntitySet(new EdmEntityContainer("Fake", "Container"), "FakeSet", type);
            return entitySet;
        }

        private static EdmEntitySet GetEntitySet(EdmModel model, IEdmEntityType type)
        {
            var container = new EdmEntityContainer("Fake", "Container");
            var entitySet = container.AddEntitySet("FakeSet", type);
            model.AddElement(container);
            return entitySet;
        }

        private static IEdmNavigationSource GetContainedEntitySet(EdmModel model, EdmEntityType type)
        {
            EdmEntitySet entitySet = GetEntitySet(model, type);
            var containedEntity = GetContainedEntityType(model);
            var navigationPropertyInfo = new EdmNavigationPropertyInfo()
            {
                Name = "nav",
                Target = containedEntity,
                TargetMultiplicity = EdmMultiplicity.Many, // todo: TargetMultiplicity info seems lost in V4
                ContainsTarget = true
            };
            EdmNavigationProperty navigationProperty = EdmNavigationProperty.CreateNavigationProperty(type, navigationPropertyInfo);
            type.AddUnidirectionalNavigation(navigationPropertyInfo);
            IEdmNavigationSource containedEntitySet = entitySet.FindNavigationTarget(navigationProperty);
            return containedEntitySet;
        }

        private static EdmEntityType GetEntityType()
        {
            EdmEntityType type = new EdmEntityType("Fake", "Type");
            type.AddKeys(type.AddStructuralProperty("Key", EdmPrimitiveTypeKind.String));
            return type;
        }

        private static EdmEntityType GetContainedEntityType(EdmModel model)
        {
            EdmEntityType type = new EdmEntityType("Fake", "ContainedType");
            type.AddKeys(type.AddStructuralProperty("Key", EdmPrimitiveTypeKind.String));
            model.AddElement(type);
            return type;
        }
    }
}
//---------------------------------------------------------------------
// <copyright file="ODataJsonLightReaderIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Edm;
using Microsoft.Test.OData.Utils.ODataLibTest;
using Xunit;

namespace Microsoft.OData.Tests.IntegrationTests.Reader.JsonLight
{
    public class ODataJsonLightReaderIntegrationTests
    {
        private static readonly IEdmModel Model;

        private static readonly EdmEntityType EntityType;

        private static readonly EdmEntitySet EntitySet;

        private const string ContextUrl = "\"@odata.context\":\"http://www.example.com/defaultService.svc/$metadata#Namespace.Container.EntitySet/$entity\"";

        private const string FullMetadata = "application/json;odata.metadata=full";

        private const string MinimalMetadata = "application/json;odata.metadata=minimal";

        static ODataJsonLightReaderIntegrationTests()
        {
            EdmModel tmp = new EdmModel();
            EntityType = new EdmEntityType("Namespace", "EntityType");
            EdmEntityContainer edmEntityContainer = new EdmEntityContainer("Namespace", "Container_sub");
            EntitySet = edmEntityContainer.AddEntitySet("EntitySet", EntityType);
            tmp.AddElement(edmEntityContainer);
            tmp.Fixup();
            Model = TestUtils.WrapReferencedModelsToMainModel("Namespace", "Container", tmp);
        }

        #region ShouldBeAbleToReadEntryWithIdOnly

        private void ShouldBeAbleToReadEntryWithIdOnlyImplementation(string payload, bool odataSimplified)
        {
            var actual = ReadJsonLightEntry(payload, FullMetadata, readingResponse: true, odataSimplified: odataSimplified);
            actual.Id.Should().Be("http://www.example.com/defaultService.svc/entryId");
        }

        [Fact]
        public void ShouldBeAbleToReadEntryWithIdOnly()
        {
            const string payload = "{" + ContextUrl + ",\"@odata.id\":\"entryId\"}";
            ShouldBeAbleToReadEntryWithIdOnlyImplementation(payload, false);
        }

        [Fact]
        public void ShouldBeAbleToReadEntryWithSimplifiedIdOnlyODataSimplified()
        {
            // cover "@id"
            const string payload = "{" + ContextUrl + ",\"@id\":\"entryId\"}";
            ShouldBeAbleToReadEntryWithIdOnlyImplementation(payload, true);
        }

        [Fact]
        public void ShouldBeAbleToReadEntryWithFullIdOnlyODataSimplified()
        {
            // cover "@odata.id"
            const string payload = "{" + ContextUrl + ",\"@odata.id\":\"entryId\"}";
            ShouldBeAbleToReadEntryWithIdOnlyImplementation(payload, true);
        }

        #endregion

        [Fact]
        public void ShouldBeAbleToReadTransientEntryInFullMetadataLevel()
        {
            const string payload = "{" + ContextUrl + ",\"@odata.id\":null}";
            var actual = ReadJsonLightEntry(payload, FullMetadata, readingResponse: true);
            actual.IsTransient.Should().BeTrue();
        }

        [Fact]
        public void ShouldBeAbleToReadTransientEntryInMinimalMetadataLevel()
        {
            const string payload = "{" + ContextUrl + ",\"@odata.id\":null}";
            var actual = ReadJsonLightEntry(payload, MinimalMetadata, readingResponse: true);
            actual.IsTransient.Should().BeTrue();
        }

        #region LinkReadingRequestShouldBeTheSameForJsonEntry

        private void LinkReadingRequestShouldBeTheSameForJsonEntryImplementation(string payload, bool odataSimplified)
        {
            var actual = this.ReadJsonLightEntry(payload, FullMetadata, readingResponse: false, odataSimplified: odataSimplified);
            actual.Id.Should().Be(CreateUri("entryId"));
            actual.EditLink.Should().Be(CreateUri("http://www.example.com/defaultService.svc/edit"));
        }

        [Fact]
        public void LinkReadingRequestShouldBeTheSameForJsonEntry()
        {
            const string payload = "{" + ContextUrl + ", " +
                "\"@odata.editLink\": \"http://www.example.com/defaultService.svc/edit\", " +
                "\"@odata.id\":\"entryId\"}";
            LinkReadingRequestShouldBeTheSameForJsonEntryImplementation(payload, false);
        }

        [Fact]
        public void LinkReadingRequestShouldBeTheSameForJsonEntrySimplifiedEditLinkODataSimplified()
        {
            // cover "@editLink"
            const string payload = "{" + ContextUrl + ", " +
                "\"@editLink\": \"http://www.example.com/defaultService.svc/edit\", " +
                "\"@id\":\"entryId\"}";
            LinkReadingRequestShouldBeTheSameForJsonEntryImplementation(payload, true);
        }

        [Fact]
        public void LinkReadingRequestShouldBeTheSameForJsonEntryFullEditLinkODataSimplified()
        {
            // cover "@odata.editLink"
            const string payload = "{" + ContextUrl + ", " +
                "\"@odata.editLink\": \"http://www.example.com/defaultService.svc/edit\", " +
                "\"@odata.id\":\"entryId\"}";
            LinkReadingRequestShouldBeTheSameForJsonEntryImplementation(payload, true);
        }

        #endregion

        [Fact]
        public void LinkReadingResponseShouldBeReadAsAbsoluteForJsonEntry()
        {
            const string payload = "{" + ContextUrl + ", " +
                "\"@odata.editLink\": \"http://www.example.com/defaultService.svc/edit\", " +
                "\"@odata.id\":\"entryId\"}";
            var actual = this.ReadJsonLightEntry(payload, FullMetadata, readingResponse: true);
            actual.Id.Should().Be(CreateUri("http://www.example.com/defaultService.svc/entryId"));
            actual.EditLink.Should().Be(CreateUri("http://www.example.com/defaultService.svc/edit"));
        }

        #region EditLinkShouldBeNullIfItsReadonlyEntry

        private void EditLinkShouldBeNullIfItsReadonlyEntryImplmentation(string payload, bool odataSimplified)
        {
            var actual = this.ReadJsonLightEntry(payload, FullMetadata, readingResponse: true, odataSimplified: odataSimplified);
            actual.Id.Should().Be(CreateUri("http://www.example.com/defaultService.svc/entryId"));
            actual.EditLink.Should().BeNull();
            actual.ReadLink.Should().Be(CreateUri("http://www.example.com/defaultService.svc/readonlyEntity"));
        }

        [Fact]
        public void EditLinkShouldBeNullIfItsReadonlyEntry()
        {
            const string payload = "{" + ContextUrl + ", " +
                "\"@odata.readLink\": \"http://www.example.com/defaultService.svc/readonlyEntity\", " +
                "\"@odata.id\":\"entryId\"}";
            EditLinkShouldBeNullIfItsReadonlyEntryImplmentation(payload, odataSimplified: false);
        }

        [Fact]
        public void EditLinkShouldBeNullIfItsReadonlyEntrySimplifiedReadLinkODataSimplified()
        {
            // cover "@readLink"
            const string payload = "{" + ContextUrl + ", " +
                "\"@readLink\": \"http://www.example.com/defaultService.svc/readonlyEntity\", " +
                "\"@id\":\"entryId\"}";
            EditLinkShouldBeNullIfItsReadonlyEntryImplmentation(payload, odataSimplified: true);
        }

        [Fact]
        public void EditLinkShouldBeNullIfItsReadonlyEntryFullReadLinkODataSimplified()
        {
            // cover "@odata.readLink"
            const string payload = "{" + ContextUrl + ", " +
                "\"@odata.readLink\": \"http://www.example.com/defaultService.svc/readonlyEntity\", " +
                "\"@odata.id\":\"entryId\"}";
            EditLinkShouldBeNullIfItsReadonlyEntryImplmentation(payload, odataSimplified: true);
        }

        #endregion

        [Fact]
        public void LinkReadingRequestShouldBeTheSameForJsonFeed()
        {
            const string payload =
                "{" +
                    "\"@odata.context\":\"http://www.example.com/defaultService.svc/$metadata#Namespace.Container.EntitySet\", " +
                    "\"value\":[" +
                        "{" +
                          "\"@odata.context\":\"http://www.example.com/defaultService.svc/$metadata#Namespace.Container.EntitySet/$entity\", " +
                          "\"@odata.editLink\":\"http://www.example.com/defaultService.svc/edit1\", " +
                          "\"@odata.id\":\"entryId1\"" +
                     "}," +
                      "{" +
                          "\"@odata.editLink\":\"edit2\", " +
                          "\"@odata.id\":\"http://www.example.com/defaultService.svc/entryId2\"" +
                       "}" +
                    "]" +
                "}";
            var entries = this.ReadJsonLightFeed(payload, MinimalMetadata, readingResponse: false);
            entries[0].Id.Should().Be(CreateUri("entryId1"));
            entries[0].EditLink.Should().Be(CreateUri("http://www.example.com/defaultService.svc/edit1"));
            entries[1].Id.Should().Be(CreateUri("http://www.example.com/defaultService.svc/entryId2"));
            entries[1].EditLink.Should().Be(CreateUri("edit2"));
        }

        [Fact]
        public void LinkReadingResponseShouldBeReadAsAbsoluteForJsonFeed()
        {
            const string payload =
                "{" +
                    "\"@odata.context\":\"http://www.example.com/defaultService.svc/$metadata#Namespace.Container.EntitySet\", " +
                    "\"value\":[" +
                        "{" +
                          "\"@odata.context\":\"http://www.example.com/defaultService.svc/$metadata#Namespace.Container.EntitySet/$entity\", " +
                          "\"@odata.editLink\":\"edit1\", " +
                          "\"@odata.id\":\"http://www.example.com/defaultService.svc/entryId1\"" +
                     "}," +
                      "{" +
                          "\"@odata.editLink\":\"http://www.example.com/defaultService.svc/edit2\", " +
                          "\"@odata.id\":\"entryId2\"" +
                       "}" +
                    "]" +
                "}";
            var entries = this.ReadJsonLightFeed(payload, MinimalMetadata, readingResponse: true);
            entries[0].Id.Should().Be(CreateUri("http://www.example.com/defaultService.svc/entryId1"));
            entries[0].EditLink.Should().Be(CreateUri("http://www.example.com/defaultService.svc/edit1"));
            entries[1].Id.Should().Be(CreateUri("http://www.example.com/defaultService.svc/entryId2"));
            entries[1].EditLink.Should().Be(CreateUri("http://www.example.com/defaultService.svc/edit2"));
        }

        [Fact]
        public void UnknownAnnotationRequestShouldBeIgnored()
        {
            const string payload = "{" + ContextUrl + ", " +
                "\"@odata.unknown\": \"unknown\", " +
                "\"@odata.metadataEtag\": \"W\\\"A1FF3E230954908F\", " +
                "\"@odata.id\":\"entryId\"}";
            var actual = this.ReadJsonLightEntry(payload, FullMetadata, readingResponse: false);
            actual.Id.Should().Be(CreateUri("entryId"));
        }

        [Fact]
        public void UnknownAnnotationResponseShouldBeIgnored()
        {
            const string payload = "{" + ContextUrl + ", " +
                "\"@odata.unknown\": \"unknown\", " +
                "\"@odata.metadataEtag\": \"W/A1FF3E230954908F\", " +
                "\"@odata.id\":\"entryId\"}";
            var actual = this.ReadJsonLightEntry(payload, FullMetadata, readingResponse: true);
            actual.Id.Should().Be(CreateUri("http://www.example.com/defaultService.svc/entryId"));
        }

        [Fact]
        public void ReadingComplexInheritWithModel()
        {
            EdmModel model = new EdmModel();
            EdmEntityType entityType = new EdmEntityType("NS", "MyTestEntity");
            EdmStructuralProperty key = entityType.AddStructuralProperty("LongId", EdmPrimitiveTypeKind.Int64, false);
            entityType.AddKeys(key);

            EdmComplexType complexType = new EdmComplexType("NS", "MyComplexType");
            complexType.AddStructuralProperty("CLongId", EdmPrimitiveTypeKind.Int64, false);

            EdmComplexType derivedComplexType = new EdmComplexType("NS", "MyDerivedComplexType", complexType, false);
            derivedComplexType.AddStructuralProperty("CFloatId", EdmPrimitiveTypeKind.Single, false);

            EdmComplexType derivedDerivedComplexType = new EdmComplexType("NS", "MyDerivedDerivedComplexType", derivedComplexType, false);
            derivedDerivedComplexType.AddStructuralProperty("DerivedCFloatId", EdmPrimitiveTypeKind.Single, false);

            model.AddElement(complexType);
            model.AddElement(derivedComplexType);
            model.AddElement(derivedDerivedComplexType);

            EdmComplexTypeReference complexTypeRef = new EdmComplexTypeReference(complexType, true);
            entityType.AddStructuralProperty("ComplexProperty", complexTypeRef);
            model.AddElement(entityType);

            EdmEntityContainer container = new EdmEntityContainer("EntityNs", "MyContainer_sub");
            EdmEntitySet entitySet = container.AddEntitySet("MyTestEntitySet", entityType);
            model.AddElement(container);

            string payload =
                "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.MyTestEntitySet/$entity\"," +
                "\"@odata.id\":\"http://MyTestEntity\"," +
                "\"LongId\":\"12\"," +
                "\"ComplexProperty\":{\"@odata.type\":\"#NS.MyDerivedComplexType\",\"CLongId\":\"1\",\"CFloatId\":1}" +
                "}";

            IEdmModel mainModel = TestUtils.WrapReferencedModelsToMainModel("EntityNs", "MyContainer", model);
            List<ODataResource> entries = new List<ODataResource>();
            ODataResourceSet feed = null;
            List<ODataNestedResourceInfo> navigations = new List<ODataNestedResourceInfo>();
            Action read = () => this.ReadEntryPayload(mainModel, payload, entitySet, entityType, reader =>
            {
                switch (reader.State)
                {
                    case ODataReaderState.ResourceStart:
                        entries.Add(reader.Item as ODataResource);
                        break;
                    case ODataReaderState.ResourceSetStart:
                        feed = reader.Item as ODataResourceSet;
                        break;
                    case ODataReaderState.NestedResourceInfoStart:
                        navigations.Add(reader.Item as ODataNestedResourceInfo);
                        break;
                    default:
                        break;
                }
            });
            read();
            Assert.Equal(2, entries.Count);
            entries[0].Properties.FirstOrDefault(s => string.Equals(s.Name, "LongId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(12L, "value should be in correct type.");

            navigations[0].Name.Should().Be("ComplexProperty");
            entries[1].Properties.FirstOrDefault(s => string.Equals(s.Name, "CLongId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(1L, "value should be in correct type.");
            entries[1].Properties.FirstOrDefault(s => string.Equals(s.Name, "CFloatId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(1.0F, "value should be in correct type.");

            payload =
                "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.MyTestEntitySet/$entity\"," +
                "\"@odata.id\":\"http://MyTestEntity\"," +
                "\"LongId\":\"12\"," +
                "\"ComplexProperty\":{\"@odata.type\":\"#NS.MyDerivedDerivedComplexType\",\"CLongId\":\"1\",\"CFloatId\":1,\"DerivedCFloatId\":1}" +
                "}";

            entries.Clear();
            navigations.Clear();
            read();
            Assert.Equal(2, entries.Count);
            entries[0].Properties.FirstOrDefault(s => string.Equals(s.Name, "LongId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(12L, "value should be in correct type.");

            navigations[0].Name.Should().Be("ComplexProperty");
            entries[1].Properties.FirstOrDefault(s => string.Equals(s.Name, "CLongId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(1L, "value should be in correct type.");
            entries[1].Properties.FirstOrDefault(s => string.Equals(s.Name, "DerivedCFloatId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(1.0F, "value should be in correct type.");
        }

        [Fact]
        public void ReadingComplexInheritInCollection()
        {
            EdmModel model = new EdmModel();
            EdmEntityType entityType = new EdmEntityType("NS", "MyTestEntity");
            EdmStructuralProperty key = entityType.AddStructuralProperty("LongId", EdmPrimitiveTypeKind.Int64, false);
            entityType.AddKeys(key);

            EdmComplexType complexType = new EdmComplexType("NS", "MyComplexType");
            complexType.AddStructuralProperty("CLongId", EdmPrimitiveTypeKind.Int64, false);

            EdmComplexType derivedComplexType = new EdmComplexType("NS", "MyDerivedComplexType", complexType, false);
            derivedComplexType.AddStructuralProperty("CFloatId", EdmPrimitiveTypeKind.Single, false);

            model.AddElement(complexType);
            model.AddElement(derivedComplexType);

            EdmComplexTypeReference complexTypeRef = new EdmComplexTypeReference(complexType, true);
            entityType.AddStructuralProperty("ComplexCollectionProperty", new EdmCollectionTypeReference(new EdmCollectionType(complexTypeRef)));
            model.AddElement(entityType);

            EdmEntityContainer container = new EdmEntityContainer("EntityNs", "MyContainer_sub");
            EdmEntitySet entitySet = container.AddEntitySet("MyTestEntitySet", entityType);
            model.AddElement(container);

            const string payload =
                "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.MyTestEntitySet/$entity\"," +
                "\"@odata.id\":\"http://MyTestEntity\"," +
                "\"LongId\":\"12\"," +
                "\"ComplexCollectionProperty\":[{\"@odata.type\":\"#NS.MyDerivedComplexType\",\"CLongId\":\"1\",\"CFloatId\":1},{\"CLongId\":\"1\"},{\"CLongId\":\"1\",\"CFloatId\":1,\"@odata.type\":\"#NS.MyDerivedComplexType\"}]" +
                "}";

            IEdmModel mainModel = TestUtils.WrapReferencedModelsToMainModel("EntityNs", "MyContainer", model);
            List<ODataResource> entries = new List<ODataResource>();
            this.ReadEntryPayload(mainModel, payload, entitySet, entityType, reader =>
            {
                switch (reader.State)
                {
                    case ODataReaderState.ResourceStart:
                        entries.Add(reader.Item as ODataResource);
                        break;
                    default:
                        break;
                }
            });
            Assert.NotNull(entries);
            Assert.Equal(4, entries.Count);

            var complexCollection = entries[1];

            Boolean derived = true;
            foreach (var complex in entries.Skip(1))
            {
                complex.Properties.FirstOrDefault(s => string.Equals(s.Name, "CLongId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(1L, "value should be in correct type.");
                if (derived) complex.Properties.FirstOrDefault(s => string.Equals(s.Name, "CFloatId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(1.0F, "value should be in correct type.");
                derived = false;
            }

        }

        [Fact]
        public void ReadingPayloadOpenComplexTypeJsonLight()
        {
            EdmModel model = new EdmModel();

            EdmEntityType entityType = new EdmEntityType("NS", "Person");
            entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);

            EdmComplexType complexType = new EdmComplexType("NS", "OpenAddress", null, false, true);
            complexType.AddStructuralProperty("CountryRegion", EdmPrimitiveTypeKind.String, false);
            EdmComplexTypeReference complexTypeRef = new EdmComplexTypeReference(complexType, true);

            entityType.AddStructuralProperty("Address", complexTypeRef);

            model.AddElement(complexType);
            model.AddElement(entityType);

            EdmEntityContainer container = new EdmEntityContainer("EntityNs", "MyContainer_sub");
            EdmEntitySet entitySet = container.AddEntitySet("People", entityType);
            model.AddElement(container);

            const string payload =
                "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.People/$entity\"," +
                "\"@odata.id\":\"http://mytest\"," +
                "\"Id\":\"0\"," +
                "\"Address\":{\"CountryRegion\":\"China\",\"City\":\"Shanghai\"}" +
                "}";

            IEdmModel mainModel = TestUtils.WrapReferencedModelsToMainModel("EntityNs", "MyContainer", model);
            List<ODataResource> entries = new List<ODataResource>();
            ODataNestedResourceInfo navigationLink;
            this.ReadEntryPayload(mainModel, payload, entitySet, entityType,
                reader =>
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            entries.Add(reader.Item as ODataResource);
                            break;
                        case ODataReaderState.NestedResourceInfoStart:
                            navigationLink = (ODataNestedResourceInfo)reader.Item;
                            break;
                        default:
                            break;
                    }
                });

            var address = entries[1];
            address.Properties.FirstOrDefault(s => string.Equals(s.Name, "CountryRegion", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo("China", "value should be in correct type.");
            address.Properties.FirstOrDefault(s => string.Equals(s.Name, "City", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo("Shanghai", "value should be in correct type.");
        }

        [Fact]
        public void ReadingTopLevelComplexProperty()
        {
            EdmModel model = new EdmModel();

            EdmComplexType complexType = new EdmComplexType("NS", "Address", null, false, true);
            complexType.AddStructuralProperty("CountryRegion", EdmPrimitiveTypeKind.String);
            model.AddElement(complexType);

            var complexTypeReference = new EdmComplexTypeReference(complexType, false);

            EdmComplexType derivedComplexType = new EdmComplexType("NS", "AddressWithCity", complexType, false);
            derivedComplexType.AddStructuralProperty("City", EdmPrimitiveTypeKind.String, false);
            model.AddElement(derivedComplexType);

            EdmEntityType entityType = new EdmEntityType("NS", "Person", null, false, true);
            entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            entityType.AddStructuralProperty("Address", complexTypeReference);
            model.AddElement(entityType);

            EdmEntityContainer container = new EdmEntityContainer("EntityNs", "MyContainer");
            EdmEntitySet entitySet = container.AddEntitySet("People", entityType);
            model.AddElement(container);

            const string payload1 =
                "{" +
                    "\"@odata.context\":\"http://www.example.com/$metadata#People(0)/Address\"," +
                    "\"CountryRegion\":\"China\"" +
                "}";

            const string payload2 =
            "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#People(0)/Address\"," +
                "\"@odata.type\":\"#NS.AddressWithCity\"," +
                "\"CountryRegion\":\"China\"," +
                "\"City\":\"Shanghai\"" +
            "}";

            const string payload3 =
            "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#NS.Address\"," +
                "\"CountryRegion\":\"China\"" +
            "}";

            const string payload4 =
            "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#NS.Address\"," +
                "\"@odata.type\":\"#NS.AddressWithCity\"," +
                "\"CountryRegion\":\"China\"," +
                "\"City\":\"Shanghai\"" +
            "}";

            const string payload5 =
            "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#NS.AddressWithCity\"," +
                "\"CountryRegion\":\"China\"," +
                "\"City\":\"Shanghai\"" +
            "}";

            Action<string> RunCase = (payload) =>
            {
                ODataResourceSet feed = null;
                ODataResource entry = null;

                this.ReadEntryPayload(model, payload, null, complexType, reader =>
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceSetStart:
                            feed = (ODataResourceSet)reader.Item;
                            break;
                        case ODataReaderState.ResourceStart:
                            entry = reader.Item as ODataResource;
                            break;
                        default:
                            break;
                    }
                });

                Assert.NotNull(entry);
                entry.Properties.FirstOrDefault(s => string.Equals(s.Name, "CountryRegion", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo("China", "value should be in correct type.");

                if (payload.Contains("NS.AddressWithCity"))
                {
                    entry.Properties.FirstOrDefault(s => string.Equals(s.Name, "City", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo("Shanghai", "value should be in correct type.");
                }
            };

            RunCase(payload1);
            RunCase(payload2);
            RunCase(payload3);
            RunCase(payload4);
            RunCase(payload5);
        }

        [Fact]
        public void ReadingTopLevelCollectionComplexProperty()
        {
            EdmModel model = new EdmModel();

            EdmComplexType complexType = new EdmComplexType("NS", "Address", null, false, true);
            complexType.AddStructuralProperty("CountryRegion", EdmPrimitiveTypeKind.String);
            model.AddElement(complexType);

            var complexTypeReference = new EdmComplexTypeReference(complexType, false);
            var complexCollectionTypeReference = new EdmCollectionTypeReference(new EdmCollectionType(complexTypeReference));

            EdmComplexType derivedComplexType = new EdmComplexType("NS", "AddressWithCity", complexType, false);
            derivedComplexType.AddStructuralProperty("City", EdmPrimitiveTypeKind.String, false);
            model.AddElement(derivedComplexType);

            EdmEntityType entityType = new EdmEntityType("NS", "Person", null, false, true);
            entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            entityType.AddStructuralProperty("Addresses", complexCollectionTypeReference);
            model.AddElement(entityType);

            EdmEntityContainer container = new EdmEntityContainer("EntityNs", "MyContainer");
            EdmEntitySet entitySet = container.AddEntitySet("People", entityType);
            model.AddElement(container);

            const string payload1 =
                "{" +
                    "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.People(0)/Addresses\"," +
                    "\"value\":[" +
                      "{" +
                          "\"CountryRegion\":\"China\"" +
                      "}," +
                      "{" +
                          "\"@odata.type\":\"#NS.AddressWithCity\"," +
                          "\"CountryRegion\":\"Korea\"," +
                          "\"City\":\"Seoul\"" +
                      "}" +
                    "]" +

                "}";

            const string payload2 =
                "{" +
                    "\"@odata.context\":\"http://www.example.com/$metadata#Collection(NS.Address)\"," +
                    "\"value\":[" +
                      "{" +
                          "\"CountryRegion\":\"China\"" +
                      "}," +
                      "{" +
                          "\"@odata.type\":\"#NS.AddressWithCity\"," +
                          "\"CountryRegion\":\"Korea\"," +
                          "\"City\":\"Seoul\"" +
                      "}" +
                    "]" +

                "}";

            Action<string> RunCase = (payload) =>
            {
                ODataResourceSet feed = null;
                List<ODataResource> entries = new List<ODataResource>();

                this.ReadTopLevleCollectionPayload(model, payload, complexType, reader =>
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceSetStart:
                            feed = (ODataResourceSet)reader.Item;
                            break;
                        case ODataReaderState.ResourceStart:
                            entries.Add(reader.Item as ODataResource);
                            break;
                        default:
                            break;
                    }
                });

                Assert.Equal(2, entries.Count);
                entries[0].Properties.FirstOrDefault(s => string.Equals(s.Name, "CountryRegion", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo("China", "value should be in correct type.");
                entries[1].Properties.FirstOrDefault(s => string.Equals(s.Name, "CountryRegion", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo("Korea", "value should be in correct type.");
                entries[1].Properties.FirstOrDefault(s => string.Equals(s.Name, "City", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo("Seoul", "value should be in correct type.");
            };

            RunCase(payload1);
            RunCase(payload2);
        }

        private void ReadEntryPayload(IEdmModel userModel, string payload, EdmEntitySet entitySet, IEdmStructuredType entityType, Action<ODataReader> action, bool isIeee754Compatible = true)
        {
            var message = new InMemoryMessage() { Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)) };
            string contentType = isIeee754Compatible
                ? "application/json;odata.metadata=minimal;IEEE754Compatible=true"
                : "application/json;odata.metadata=minimal;IEEE754Compatible=false";
            message.SetHeader("Content-Type", contentType);
            var readerSettings = new ODataMessageReaderSettings { DisableMessageStreamDisposal = false };
            using (var msgReader = new ODataMessageReader((IODataResponseMessage)message, readerSettings, userModel))
            {
                var reader = msgReader.CreateODataResourceReader(entitySet, entityType);
                while (reader.Read())
                {
                    action(reader);
                }
            }
        }

        private void ReadTopLevleCollectionPayload(IEdmModel userModel, string payload, IEdmStructuredType complexType, Action<ODataReader> action, bool isIeee754Compatible = true)
        {
            var message = new InMemoryMessage() { Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)) };
            string contentType = isIeee754Compatible
                ? "application/json;odata.metadata=minimal;IEEE754Compatible=true"
                : "application/json;odata.metadata=minimal;IEEE754Compatible=false";
            message.SetHeader("Content-Type", contentType);
            var readerSettings = new ODataMessageReaderSettings { DisableMessageStreamDisposal = false };
            using (var msgReader = new ODataMessageReader((IODataResponseMessage)message, readerSettings, userModel))
            {
                var reader = msgReader.CreateODataResourceSetReader(complexType);
                while (reader.Read())
                {
                    action(reader);
                }
            }
        }

        private ODataResource ReadJsonLightEntry(string payload, string contentType, bool readingResponse, bool odataSimplified = false)
        {
            InMemoryMessage message = new InMemoryMessage();
            message.SetHeader("Content-Type", contentType);
            message.Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));

            ODataResource topLevelEntry = null;

            ODataMessageReaderSettings settings = new ODataMessageReaderSettings { ODataSimplified = odataSimplified };

            using (var messageReader = readingResponse
                ? new ODataMessageReader((IODataResponseMessage)message, settings, Model)
                : new ODataMessageReader((IODataRequestMessage)message, settings, Model))
            {
                var reader = messageReader.CreateODataResourceReader(EntitySet, EntityType);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceEnd:
                            topLevelEntry = (ODataResource)reader.Item;
                            break;
                    }
                }
            }

            return topLevelEntry;
        }

        private IList<ODataResource> ReadJsonLightFeed(string payload, string contentType, bool readingResponse)
        {
            InMemoryMessage message = new InMemoryMessage();
            message.SetHeader("Content-Type", contentType);
            message.Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));

            IList<ODataResource> entries = new List<ODataResource>();

            using (var messageReader = readingResponse
                ? new ODataMessageReader((IODataResponseMessage)message, null, Model)
                : new ODataMessageReader((IODataRequestMessage)message, null, Model))
            {
                var reader = messageReader.CreateODataResourceSetReader(EntitySet, EntityType);
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceEnd:
                            entries.Add((ODataResource)reader.Item);
                            break;
                    }
                }
            }

            return entries;
        }

        private static Uri CreateUri(string uriValue)
        {
            return new Uri(uriValue, UriKind.RelativeOrAbsolute);
        }
    }
}

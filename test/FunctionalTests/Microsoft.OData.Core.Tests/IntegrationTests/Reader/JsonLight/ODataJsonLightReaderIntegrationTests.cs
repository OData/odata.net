//---------------------------------------------------------------------
// <copyright file="ODataJsonLightReaderIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.OData.Edm;
using Microsoft.Test.OData.DependencyInjection;
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

        private void ShouldBeAbleToReadEntryWithIdOnlyImplementation(string payload, bool enableReadingODataAnnotationWithoutPrefix)
        {
            var actual = ReadJsonLightEntry(payload, FullMetadata, readingResponse: true, enableReadingODataAnnotationWithoutPrefix: enableReadingODataAnnotationWithoutPrefix);
            Assert.Equal("http://www.example.com/defaultService.svc/entryId", actual.Id.ToString());
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
        public void ShouldReadDynamicNullableCollectionValuedProperty()
        {
            // setup model
            var model = new EdmModel();
            var entityType = new EdmEntityType("NS", "EntityType", null, false, true);
            entityType.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32);
            var container = new EdmEntityContainer("NS", "Container");
            var entitySet = container.AddEntitySet("EntitySet", entityType);
            var complexType = new EdmComplexType("NS", "ComplexType");
            complexType.AddStructuralProperty("Prop1", EdmPrimitiveTypeKind.Int32);
            complexType.AddStructuralProperty("Prop2", EdmPrimitiveTypeKind.Int32);
            model.AddElements(new IEdmSchemaElement[] { entityType, complexType, container });

            const string payload
                = "{" +
                      "\"@odata.context\":\"http://svc/$metadata#EntitySet/$entity\"," +
                      "\"ID\":1," +
                      "\"DynamicPrimitive@odata.type\":\"#Collection(Int64)\"," +
                      "\"DynamicPrimitive\":[1,2,null]," +
                      "\"DynamicComplex@odata.type\":\"#Collection(NS.ComplexType)\"," +
                      "\"DynamicComplex\":[null,{\"Prop1\":1,\"Prop2\":2}]" +
                  "}";

            var message = new InMemoryMessage { Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)) };

            Action<ODataReader> read = (reader) =>
            {
                // read payload
                ODataResource resource;
                var count = 0;
                while (reader.Read())
                {
                    if (reader.State == ODataReaderState.ResourceEnd)
                    {
                        resource = (ODataResource)reader.Item;
                        switch (++count)
                        {
                            case 1:
                                Assert.Null(resource);
                                break;
                            case 3:
                                var enumerator = ((ODataCollectionValue)(resource.Properties.Skip(1).Single().Value)).Items.GetEnumerator();
                                enumerator.MoveNext();
                                Assert.Equal(1L, enumerator.Current);
                                enumerator.MoveNext();
                                Assert.Equal(2L, enumerator.Current);
                                enumerator.MoveNext();
                                Assert.Null(enumerator.Current);
                                break;
                        }
                    }
                    else if (reader.State == ODataReaderState.ResourceSetEnd)
                    {
                        var resourceSet = reader.Item as ODataResourceSet;
                        Assert.NotNull(resourceSet);
                        Assert.Equal("Collection(NS.ComplexType)", resourceSet.TypeName);
                    }
                }
            };

            // setup reader for response message
            var responseReader = new ODataMessageReader((IODataResponseMessage)message, new ODataMessageReaderSettings(), model)
                         .CreateODataResourceReader(entitySet, entityType);
            read(responseReader);

            // setup reader for request message
            message.Stream.Seek(0, SeekOrigin.Begin);
            var requestReader = new ODataMessageReader((IODataRequestMessage)message, new ODataMessageReaderSettings(), model)
                         .CreateODataResourceReader(entitySet, entityType);
            read(requestReader);
        }

        [Fact]
        public void ShouldBeAbleToReadTransientEntryInFullMetadataLevel()
        {
            const string payload = "{" + ContextUrl + ",\"@odata.id\":null}";
            var actual = ReadJsonLightEntry(payload, FullMetadata, readingResponse: true);
            Assert.True(actual.IsTransient);
        }

        [Fact]
        public void ShouldBeAbleToReadTransientEntryInMinimalMetadataLevel()
        {
            const string payload = "{" + ContextUrl + ",\"@odata.id\":null}";
            var actual = ReadJsonLightEntry(payload, MinimalMetadata, readingResponse: true);
            Assert.True(actual.IsTransient);
        }

        #region LinkReadingRequestShouldBeTheSameForJsonEntry

        private void LinkReadingRequestShouldBeTheSameForJsonEntryImplementation(string payload, bool odataSimplified)
        {
            var actual = this.ReadJsonLightEntry(payload, FullMetadata, readingResponse: false, enableReadingODataAnnotationWithoutPrefix: odataSimplified);
            Assert.Equal(CreateUri("entryId"), actual.Id);
            Assert.Equal(CreateUri("http://www.example.com/defaultService.svc/edit"), actual.EditLink);
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
            Assert.Equal(CreateUri("http://www.example.com/defaultService.svc/entryId"), actual.Id);
            Assert.Equal(CreateUri("http://www.example.com/defaultService.svc/edit"), actual.EditLink);
        }

        #region EditLinkShouldBeNullIfItsReadonlyEntry

        private void EditLinkShouldBeNullIfItsReadonlyEntryImplmentation(string payload, bool enableReadingODataAnnotationWithoutPrefix)
        {
            var actual = this.ReadJsonLightEntry(payload, FullMetadata, readingResponse: true, enableReadingODataAnnotationWithoutPrefix: enableReadingODataAnnotationWithoutPrefix);
            Assert.Equal(CreateUri("http://www.example.com/defaultService.svc/entryId"), actual.Id);
            Assert.Null(actual.EditLink);
            Assert.Equal(CreateUri("http://www.example.com/defaultService.svc/readonlyEntity"), actual.ReadLink);
        }

        [Fact]
        public void EditLinkShouldBeNullIfItsReadonlyEntry()
        {
            const string payload = "{" + ContextUrl + ", " +
                "\"@odata.readLink\": \"http://www.example.com/defaultService.svc/readonlyEntity\", " +
                "\"@odata.id\":\"entryId\"}";
            EditLinkShouldBeNullIfItsReadonlyEntryImplmentation(payload, enableReadingODataAnnotationWithoutPrefix: false);
        }

        [Fact]
        public void EditLinkShouldBeNullIfItsReadonlyEntrySimplifiedReadLinkODataSimplified()
        {
            // cover "@readLink"
            const string payload = "{" + ContextUrl + ", " +
                "\"@readLink\": \"http://www.example.com/defaultService.svc/readonlyEntity\", " +
                "\"@id\":\"entryId\"}";
            EditLinkShouldBeNullIfItsReadonlyEntryImplmentation(payload, enableReadingODataAnnotationWithoutPrefix: true);
        }

        [Fact]
        public void EditLinkShouldBeNullIfItsReadonlyEntryFullReadLinkODataSimplified()
        {
            // cover "@odata.readLink"
            const string payload = "{" + ContextUrl + ", " +
                "\"@odata.readLink\": \"http://www.example.com/defaultService.svc/readonlyEntity\", " +
                "\"@odata.id\":\"entryId\"}";
            EditLinkShouldBeNullIfItsReadonlyEntryImplmentation(payload, enableReadingODataAnnotationWithoutPrefix: true);
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
            Assert.Equal(CreateUri("entryId1"), entries[0].Id);
            Assert.Equal(CreateUri("http://www.example.com/defaultService.svc/edit1"), entries[0].EditLink);
            Assert.Equal(CreateUri("http://www.example.com/defaultService.svc/entryId2"), entries[1].Id);
            Assert.Equal(CreateUri("edit2"), entries[1].EditLink);
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
            Assert.Equal(CreateUri("http://www.example.com/defaultService.svc/entryId1"), entries[0].Id);
            Assert.Equal(CreateUri("http://www.example.com/defaultService.svc/edit1"), entries[0].EditLink);
            Assert.Equal(CreateUri("http://www.example.com/defaultService.svc/entryId2"), entries[1].Id);
            Assert.Equal(CreateUri("http://www.example.com/defaultService.svc/edit2"), entries[1].EditLink);
        }

        [Fact]
        public void UnknownAnnotationRequestShouldBeIgnored()
        {
            const string payload = "{" + ContextUrl + ", " +
                "\"@odata.unknown\": \"unknown\", " +
                "\"@odata.metadataEtag\": \"W\\\"A1FF3E230954908F\", " +
                "\"@odata.id\":\"entryId\"}";
            var actual = this.ReadJsonLightEntry(payload, FullMetadata, readingResponse: false);
            Assert.Equal(CreateUri("entryId"), actual.Id);
        }

        [Fact]
        public void UnknownAnnotationResponseShouldBeIgnored()
        {
            const string payload = "{" + ContextUrl + ", " +
                "\"@odata.unknown\": \"unknown\", " +
                "\"@odata.metadataEtag\": \"W/A1FF3E230954908F\", " +
                "\"@odata.id\":\"entryId\"}";
            var actual = this.ReadJsonLightEntry(payload, FullMetadata, readingResponse: true);
            Assert.Equal(CreateUri("http://www.example.com/defaultService.svc/entryId"), actual.Id);
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
            Assert.Equal(12L, entries[0].Properties.FirstOrDefault(s => string.Equals(s.Name, "LongId", StringComparison.OrdinalIgnoreCase)).Value);

            Assert.Equal("ComplexProperty", navigations[0].Name);
            Assert.Equal(1L, entries[1].Properties.FirstOrDefault(s => string.Equals(s.Name, "CLongId", StringComparison.OrdinalIgnoreCase)).Value);
            Assert.Equal(1.0F, entries[1].Properties.FirstOrDefault(s => string.Equals(s.Name, "CFloatId", StringComparison.OrdinalIgnoreCase)).Value);

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
            Assert.Equal(12L, entries[0].Properties.FirstOrDefault(s => string.Equals(s.Name, "LongId", StringComparison.OrdinalIgnoreCase)).Value);

            Assert.Equal("ComplexProperty", navigations[0].Name);
            Assert.Equal(1L, entries[1].Properties.FirstOrDefault(s => string.Equals(s.Name, "CLongId", StringComparison.OrdinalIgnoreCase)).Value);
            Assert.Equal(1.0F, entries[1].Properties.FirstOrDefault(s => string.Equals(s.Name, "DerivedCFloatId", StringComparison.OrdinalIgnoreCase)).Value);
        }

        [Fact]
        public void ReadingComplexInheritInCollectionWithODataType()
        {
            const string payload =
                "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.MyTestEntitySet/$entity\"," +
                "\"@odata.id\":\"http://MyTestEntity\"," +
                "\"LongId\":\"12\"," +
                "\"ComplexCollectionProperty@odata.type\":\"#Collection(NS.MyComplexType)\"," +
                "\"ComplexCollectionProperty\":[{\"@odata.type\":\"#NS.MyDerivedComplexType\",\"CLongId\":\"1\",\"CFloatId\":1},{\"CLongId\":\"1\"},{\"CLongId\":\"1\",\"CFloatId\":1,\"@odata.type\":\"#NS.MyDerivedComplexType\"}]" +
                "}";

            ReadingComplexInheritInCollectionImpl(payload);
        }

        [Fact]
        public void ReadingComplexInheritInCollection()
        {
            const string payload =
                "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.MyTestEntitySet/$entity\"," +
                "\"@odata.id\":\"http://MyTestEntity\"," +
                "\"LongId\":\"12\"," +
                "\"ComplexCollectionProperty\":[{\"@odata.type\":\"#NS.MyDerivedComplexType\",\"CLongId\":\"1\",\"CFloatId\":1},{\"CLongId\":\"1\"},{\"CLongId\":\"1\",\"CFloatId\":1,\"@odata.type\":\"#NS.MyDerivedComplexType\"}]" +
                "}";

            ReadingComplexInheritInCollectionImpl(payload);
        }

        private void ReadingComplexInheritInCollectionImpl(string payload)
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
                Assert.Equal(1L, complex.Properties.FirstOrDefault(s => string.Equals(s.Name, "CLongId", StringComparison.OrdinalIgnoreCase)).Value);
                if (derived)
                {
                    Assert.Equal(1.0F, complex.Properties.FirstOrDefault(s => string.Equals(s.Name, "CFloatId", StringComparison.OrdinalIgnoreCase)).Value);
                }

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
            Assert.Equal("China", address.Properties.FirstOrDefault(s => string.Equals(s.Name, "CountryRegion", StringComparison.OrdinalIgnoreCase)).Value);
            var property = address.Properties.FirstOrDefault(s => string.Equals(s.Name, "City", StringComparison.OrdinalIgnoreCase));
            var value = Assert.IsType<ODataUntypedValue>(property.Value);
            Assert.Equal("\"Shanghai\"", value.RawValue);
        }

        [Fact]
        public void ReadingPayloadOpenComplexTypeJsonLight_WithComplex()
        {
            EdmModel model = new EdmModel();

            EdmEntityType entityType = new EdmEntityType("NS", "Person", null, false, true);
            entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);

            EdmComplexType cityType = new EdmComplexType("NS", "City");
            cityType.AddStructuralProperty("CityName", EdmPrimitiveTypeKind.String);
            model.AddElement(cityType);

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
                "\"Address\":{\"CountryRegion\":\"China\"}," +
                "\"City\":{\"@odata.type\":\"#NS.City\", \"CityName\":\"Shanghai\"}" +
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
            Assert.Equal("China", address.Properties.FirstOrDefault(s => string.Equals(s.Name, "CountryRegion", StringComparison.OrdinalIgnoreCase)).Value);
            Assert.Equal("Shanghai", entries[2].Properties.FirstOrDefault().Value);
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
                ODataResource entry = null;

                this.ReadEntryPayload(model, payload, null, complexType, reader =>
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            entry = reader.Item as ODataResource;
                            break;
                        default:
                            break;
                    }
                });

                Assert.NotNull(entry);
                Assert.Equal("China", entry.Properties.FirstOrDefault(s => string.Equals(s.Name, "CountryRegion", StringComparison.OrdinalIgnoreCase)).Value);

                if (payload.Contains("NS.AddressWithCity"))
                {
                    Assert.Equal("Shanghai", entry.Properties.FirstOrDefault(s => string.Equals(s.Name, "City", StringComparison.OrdinalIgnoreCase)).Value);
                }
            };

            RunCase(payload1);
            RunCase(payload2);
            RunCase(payload3);
            RunCase(payload4);
            RunCase(payload5);
        }

        [Fact]
        public void ReadingTopLevelComplex_WithoutProperties()
        {
            EdmModel model = new EdmModel();

            EdmComplexType complexType = new EdmComplexType("NS", "Address", null, false, true);
            complexType.AddStructuralProperty("CountryRegion", EdmPrimitiveTypeKind.String);
            model.AddElement(complexType);

            const string payload = "{}";

            this.ReadEntryPayload(model, payload, null, complexType, reader =>
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    var resource = reader.Item as ODataResource;
                    Assert.Equal("NS.Address", resource.TypeName);
                    Assert.Empty(resource.Properties);
                }
            },
            true);
        }

        [Fact]
        public void ReadingTopLevelComplexCollection_ItemType_TypeContextUri_Inconsistent()
        {
            const string payload =
                "{" +
                    "\"@odata.context\":\"http://www.example.com/$metadata#Collection(NS.AddressWithCity)\"," +
                    "\"value\":[" +
                      "{" +
                          "\"@odata.type\":\"#NS.Address\"," +
                          "\"CountryRegion\":\"China\"" +
                      "}," +
                      "{" +
                          "\"@odata.type\":\"#NS.AddressWithCity\"," +
                          "\"CountryRegion\":\"Korea\"," +
                          "\"City\":" +
                          "{" +
                              "\"CityName\":\"Seoul\"" +
                          "}" +
                      "}" +
                    "]" +
                "}";
            List<ODataResource> resources = null;
            List<ODataNestedResourceInfo> nestedResourceInfos = null;
            ReadingTopLevelComplexCollectionProperty(payload, ref resources, ref nestedResourceInfos, false, false, Strings.ValidationUtils_IncompatibleType("NS.Address", "NS.AddressWithCity"));
        }

        [Fact]
        public void ReadingTopLevelComplexCollection_CollectionType_ItemType_Inconsistent()
        {
            const string payload =
                "{" +
                    "\"value@odata.type\":\"#Collection(NS.AddressWithCity)\"," +
                    "\"value\":[" +
                      "{" +
                          "\"@odata.type\":\"#NS.Address\"," +
                          "\"CountryRegion\":\"China\"" +
                      "}," +
                      "{" +
                          "\"@odata.type\":\"#NS.AddressWithCity\"," +
                          "\"CountryRegion\":\"Korea\"," +
                          "\"City\":" +
                          "{" +
                              "\"CityName\":\"Seoul\"" +
                          "}" +
                      "}" +
                    "]" +
                "}";
            List<ODataResource> resources = null;
            List<ODataNestedResourceInfo> nestedResourceInfos = null;
            ReadingTopLevelComplexCollectionProperty(payload, ref resources, ref nestedResourceInfos, true, true, Strings.ValidationUtils_IncompatibleType("NS.Address", "NS.AddressWithCity"));
        }

        [Fact]
        public void ReadingTopLevelComplexCollection_without_odatatype()
        {
            const string payload =
                "{" +
                    "\"value\":[" +
                      "{" +
                          "\"CountryRegion\":\"China\"" +
                      "}," +
                      "{" +
                          "\"CountryRegion\":\"Korea\"," +
                          "\"City\":" +
                          "{" +
                              "\"CityName\":\"Seoul\"" +
                          "}" +
                      "}" +
                    "]" +
                "}";
            List<ODataResource> resources = null;
            List<ODataNestedResourceInfo> nestedResourceInfos = null;
            ReadingTopLevelComplexCollectionProperty(payload, ref resources, ref nestedResourceInfos, true, true);

            Assert.Equal(3, resources.Count);
            Assert.Equal("China", resources[0].Properties.FirstOrDefault(s => string.Equals(s.Name, "CountryRegion", StringComparison.OrdinalIgnoreCase)).Value);
            Assert.Equal("Korea", resources[1].Properties.FirstOrDefault(s => string.Equals(s.Name, "CountryRegion", StringComparison.OrdinalIgnoreCase)).Value);
            Assert.Equal("Seoul", resources[2].Properties.FirstOrDefault(s => string.Equals(s.Name, "CityName", StringComparison.OrdinalIgnoreCase)).Value);
            Assert.Equal("City", nestedResourceInfos.First().Name);
        }

        [Fact]
        public void ReadingTopLevelComplexCollection_OpenPropertyContextUri()
        {
            const string payload =
                "{" +
                    "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.People(0)/OpenAddresses\"," +
                    "\"value@odata.type\":\"#Collection(NS.AddressWithCity)\"," +
                    "\"value\":[" +
                      "{" +
                          "\"CountryRegion\":\"China\"" +
                      "}," +
                      "{" +
                          "\"@odata.type\":\"#NS.AddressWithCity\"," +
                          "\"CountryRegion\":\"Korea\"," +
                          "\"City\":" +
                          "{" +
                              "\"CityName\":\"Seoul\"" +
                          "}" +
                      "}" +
                    "]" +

                "}";

            List<ODataResource> resources = null;
            List<ODataNestedResourceInfo> nestedResourceInfos = null;
            ReadingTopLevelComplexCollectionProperty(payload, ref resources, ref nestedResourceInfos);

            Assert.Equal(3, resources.Count);
            Assert.Equal("China", resources[0].Properties.FirstOrDefault(s => string.Equals(s.Name, "CountryRegion", StringComparison.OrdinalIgnoreCase)).Value);
            Assert.Equal("Korea", resources[1].Properties.FirstOrDefault(s => string.Equals(s.Name, "CountryRegion", StringComparison.OrdinalIgnoreCase)).Value);
            Assert.Equal("Seoul", resources[2].Properties.FirstOrDefault(s => string.Equals(s.Name, "CityName", StringComparison.OrdinalIgnoreCase)).Value);
            Assert.Equal("City", nestedResourceInfos.First().Name);
        }

        [Fact]
        public void ReadingTopLevelComplexCollection_PropertyContextUri()
        {
            const string payload =
                "{" +
                    "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.People(0)/Addresses\"," +
                    "\"value\":[" +
                      "{" +
                          "\"CountryRegion\":\"China\"" +
                      "}," +
                      "{" +
                          "\"@odata.type\":\"#NS.AddressWithCity\"," +
                          "\"CountryRegion\":\"Korea\"," +
                          "\"City\":" +
                          "{" +
                              "\"CityName\":\"Seoul\"" +
                          "}" +
                      "}" +
                    "]" +

                "}";
            List<ODataResource> resources = null;
            List<ODataNestedResourceInfo> nestedResourceInfos = null;
            ReadingTopLevelComplexCollectionProperty(payload, ref resources, ref nestedResourceInfos);

            Assert.Equal(3, resources.Count);
            Assert.Equal("China", resources[0].Properties.FirstOrDefault(s => string.Equals(s.Name, "CountryRegion", StringComparison.OrdinalIgnoreCase)).Value);
            Assert.Equal("Korea", resources[1].Properties.FirstOrDefault(s => string.Equals(s.Name, "CountryRegion", StringComparison.OrdinalIgnoreCase)).Value);
            Assert.Equal("Seoul", resources[2].Properties.FirstOrDefault(s => string.Equals(s.Name, "CityName", StringComparison.OrdinalIgnoreCase)).Value);
            Assert.Equal("City", nestedResourceInfos.First().Name);
        }

        [Fact]
        public void ReadingTopLevelComplexCollection_TypeContextUri()
        {
            const string payload =
                "{" +
                    "\"@odata.context\":\"http://www.example.com/$metadata#Collection(NS.Address)\"," +
                    "\"value\":[" +
                      "{" +
                          "\"CountryRegion\":\"China\"" +
                      "}," +
                      "{" +
                          "\"@odata.type\":\"#NS.AddressWithCity\"," +
                          "\"CountryRegion\":\"Korea\"," +
                          "\"City\":" +
                          "{" +
                              "\"CityName\":\"Seoul\"" +
                          "}" +
                      "}" +
                    "]" +
                "}";
            List<ODataResource> resources = null;
            List<ODataNestedResourceInfo> nestedResourceInfos = null;
            ReadingTopLevelComplexCollectionProperty(payload, ref resources, ref nestedResourceInfos);

            Assert.Equal(3, resources.Count);
            Assert.Equal("China", resources[0].Properties.FirstOrDefault(s => string.Equals(s.Name, "CountryRegion", StringComparison.OrdinalIgnoreCase)).Value);
            Assert.Equal("Korea", resources[1].Properties.FirstOrDefault(s => string.Equals(s.Name, "CountryRegion", StringComparison.OrdinalIgnoreCase)).Value);
            Assert.Equal("Seoul", resources[2].Properties.FirstOrDefault(s => string.Equals(s.Name, "CityName", StringComparison.OrdinalIgnoreCase)).Value);
            Assert.Equal("City", nestedResourceInfos.First().Name);
        }

        private void ReadingTopLevelComplexCollectionProperty(
            string payload,
            ref List<ODataResource> resources,
            ref List<ODataNestedResourceInfo> nestedResourceInfos,
            bool isRequest = false,
            bool isDerivedCollection = false,
            string expectedException = null)
        {
            EdmModel model = new EdmModel();

            EdmComplexType complexType = new EdmComplexType("NS", "Address", null, false, true);
            complexType.AddStructuralProperty("CountryRegion", EdmPrimitiveTypeKind.String);
            model.AddElement(complexType);

            EdmComplexType cityComplexType = new EdmComplexType("NS", "City");
            cityComplexType.AddStructuralProperty("CityName", EdmPrimitiveTypeKind.String, false);
            model.AddElement(cityComplexType);

            var complexTypeReference = new EdmComplexTypeReference(complexType, false);
            var complexCollectionTypeReference = new EdmCollectionTypeReference(new EdmCollectionType(complexTypeReference));

            EdmComplexType derivedComplexType = new EdmComplexType("NS", "AddressWithCity", complexType, false);
            derivedComplexType.AddStructuralProperty("City", new EdmComplexTypeReference(cityComplexType, false));
            model.AddElement(derivedComplexType);

            EdmEntityType entityType = new EdmEntityType("NS", "Person", null, false, true);
            entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
            entityType.AddStructuralProperty("Addresses", complexCollectionTypeReference);
            model.AddElement(entityType);

            EdmEntityContainer container = new EdmEntityContainer("EntityNs", "MyContainer");
            EdmEntitySet entitySet = container.AddEntitySet("People", entityType);
            model.AddElement(container);

            var currentResources = new List<ODataResource>();
            var currentNestedResourceInfos = new List<ODataNestedResourceInfo>();

            Action read = () =>
            {
                this.ReadTopLevleCollectionPayload(model, payload, isDerivedCollection ? derivedComplexType : complexType, reader =>
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            currentResources.Add(reader.Item as ODataResource);
                            break;
                        case ODataReaderState.NestedResourceInfoStart:
                            currentNestedResourceInfos.Add(reader.Item as ODataNestedResourceInfo);
                            break;
                        default:
                            break;
                    }
                },
                isRequest);
            };

            if (expectedException != null)
            {
                read.Throws<ODataException>(expectedException);
            }
            else
            {
                read();
                resources = currentResources;
                nestedResourceInfos = currentNestedResourceInfos;
            }
        }

        [Fact]
        public void ReadUndeclaredPropertyInOpenType()
        {
            const string payload =
                "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.People/$entity\"," +
                "\"@odata.id\":\"People(0)\"," +
                "\"Id\":\"0\"," +
                "\"OpenProperty@odata.type\":\"#String\"," +
                "\"OpenProperty\":\"0\"," +
                "\"OpenComplex\":{\"@odata.type\":\"#NS.Address\",\"CountryRegion\":\"China\"}," +
                "\"OpenNavigationProperty\":{\"@odata.type\":\"#NS.Person\",\"Id\":\"0\"}" +
                "}";

            List<ODataResource> entries = new List<ODataResource>();
            ODataNestedResourceInfo navigationLink;
            this.ReadEntryPayloadForUndeclared(payload,
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
                }, true, true);

            var address = entries[1];
            Assert.Equal("China", address.Properties.FirstOrDefault(s => string.Equals(s.Name, "CountryRegion", StringComparison.OrdinalIgnoreCase)).Value);

            var openNavigation = entries[2];
            Assert.Equal(0, openNavigation.Properties.FirstOrDefault(s => string.Equals(s.Name, "Id", StringComparison.OrdinalIgnoreCase)).Value);
        }

        [Fact]
        public void ReadUndeclaredPropertyInNonOpenType()
        {
            string payload =
                "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.People/$entity\"," +
                "\"@odata.id\":\"People(0)\"," +
                "\"Id\":\"0\"," +
                "\"OpenProperty@odata.type\":\"#String\"," +
                "\"OpenProperty\":\"0\"," +
                "\"OpenComplex\":{\"@odata.type\":\"#NS.Address\",\"CountryRegion\":\"China\"}," +
                "\"OpenNavigationProperty\":{\"@odata.type\":\"#NS.Person\",\"Id\":\"0\"}" +
                "}";

            List<ODataResource> entries = new List<ODataResource>();
            ODataNestedResourceInfo navigationLink;
            this.ReadEntryPayloadForUndeclared(payload,
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
                }, false, false);

            var address = entries[1];
            Assert.Equal("China", address.Properties.FirstOrDefault(s => string.Equals(s.Name, "CountryRegion", StringComparison.OrdinalIgnoreCase)).Value);

            var openNavigation = entries[2];
            Assert.Equal(0, openNavigation.Properties.FirstOrDefault(s => string.Equals(s.Name, "Id", StringComparison.OrdinalIgnoreCase)).Value);

            payload = "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.People/$entity\"," +
                "\"@odata.id\":\"People(0)\"," +
                "\"Id\":\"0\"," +
                "\"OpenProperty@odata.type\":\"#String\"," +
                "\"OpenProperty\":\"0\"" +
                "}";

            Action readEntry = () => this.ReadEntryPayloadForUndeclared(payload, reader => { }, false, true);

            readEntry.Throws<ODataException>(Strings.ValidationUtils_PropertyDoesNotExistOnType("OpenProperty", "NS.Person"));

            payload = "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.People/$entity\"," +
                "\"@odata.id\":\"People(0)\"," +
                "\"Id\":\"0\"," +
                "\"OpenComplex\":{\"@odata.type\":\"#NS.Address\",\"CountryRegion\":\"China\"}" +
                "}";

            readEntry = () => this.ReadEntryPayloadForUndeclared(payload, reader => { }, false, true);

            readEntry.Throws<ODataException>(Strings.ValidationUtils_PropertyDoesNotExistOnType("OpenComplex", "NS.Person"));

            payload = "{" +
                "\"@odata.context\":\"http://www.example.com/$metadata#EntityNs.MyContainer.People/$entity\"," +
                "\"@odata.id\":\"People(0)\"," +
                "\"Id\":\"0\"," +
                "\"OpenNavigationProperty\":{\"@odata.type\":\"#NS.Person\",\"Id\":\"0\"}" +
                "}";

            readEntry = () => this.ReadEntryPayloadForUndeclared(payload, reader => { }, false, true);

            readEntry.Throws<ODataException>(Strings.ValidationUtils_PropertyDoesNotExistOnType("OpenNavigationProperty", "NS.Person"));
        }

        private void ReadEntryPayload(IEdmModel userModel, string payload, EdmEntitySet entitySet, IEdmStructuredType entityType, Action<ODataReader> action, bool isRequest = false, bool isIeee754Compatible = true)
        {
            var message = new InMemoryMessage() { Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)) };
            string contentType = isIeee754Compatible
                ? "application/json;odata.metadata=minimal;IEEE754Compatible=true"
                : "application/json;odata.metadata=minimal;IEEE754Compatible=false";
            message.SetHeader("Content-Type", contentType);
            var readerSettings = new ODataMessageReaderSettings { EnableMessageStreamDisposal = true };
            using (var msgReader = isRequest ? new ODataMessageReader((IODataRequestMessage)message, readerSettings, userModel)
                                             : new ODataMessageReader((IODataResponseMessage)message, readerSettings, userModel))
            {
                var reader = msgReader.CreateODataResourceReader(entitySet, entityType);
                while (reader.Read())
                {
                    action(reader);
                }
            }
        }

        private void ReadEntryPayloadForUndeclared(string payload, Action<ODataReader> action, bool isOpenProperty, bool throwOnUndeclaredProperty)
        {
            EdmModel model = new EdmModel();

            EdmEntityType entityType = new EdmEntityType("NS", "Person", null, false, isOpenProperty);
            entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);

            EdmComplexType complexType = new EdmComplexType("NS", "Address", null, false, true);
            complexType.AddStructuralProperty("CountryRegion", EdmPrimitiveTypeKind.String, false);

            model.AddElement(complexType);
            model.AddElement(entityType);

            EdmEntityContainer container = new EdmEntityContainer("EntityNs", "MyContainer_sub");
            EdmEntitySet entitySet = container.AddEntitySet("People", entityType);
            model.AddElement(container);

            var message = new InMemoryMessage() { Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)) };
            var readerSettings = new ODataMessageReaderSettings { EnableMessageStreamDisposal = true };
            if (!throwOnUndeclaredProperty)
            {
                readerSettings.Validations &= ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType;
            }

            IEdmModel mainModel = TestUtils.WrapReferencedModelsToMainModel("EntityNs", "MyContainer", model);

            using (var msgReader = new ODataMessageReader((IODataResponseMessage)message, readerSettings, mainModel))
            {
                var reader = msgReader.CreateODataResourceReader(entitySet, entityType);
                while (reader.Read())
                {
                    action(reader);
                }
            }
        }

        private void ReadTopLevleCollectionPayload(IEdmModel userModel, string payload, IEdmStructuredType complexType, Action<ODataReader> action, bool isRequest = false, bool isIeee754Compatible = true)
        {
            var message = new InMemoryMessage() { Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)) };
            string contentType = isIeee754Compatible
                ? "application/json;odata.metadata=minimal;IEEE754Compatible=true"
                : "application/json;odata.metadata=minimal;IEEE754Compatible=false";
            message.SetHeader("Content-Type", contentType);

            var readerSettings = new ODataMessageReaderSettings { EnableMessageStreamDisposal = true };
            using (var msgReader = isRequest ? new ODataMessageReader((IODataRequestMessage)message, readerSettings, userModel)
                                             : new ODataMessageReader((IODataResponseMessage)message, readerSettings, userModel))
            {
                var reader = msgReader.CreateODataResourceSetReader(complexType);
                while (reader.Read())
                {
                    action(reader);
                }
            }
        }

        private ODataResource ReadJsonLightEntry(string payload, string contentType, bool readingResponse, bool enableReadingODataAnnotationWithoutPrefix = false)
        {
            var container = ContainerBuilderHelper.BuildContainer(null);
            container.GetRequiredService<ODataSimplifiedOptions>().EnableReadingODataAnnotationWithoutPrefix =
                enableReadingODataAnnotationWithoutPrefix;

            InMemoryMessage message = new InMemoryMessage() { Container = container };
            message.SetHeader("Content-Type", contentType);
            message.Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));

            ODataResource topLevelEntry = null;
            ODataMessageReaderSettings settings = new ODataMessageReaderSettings();

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

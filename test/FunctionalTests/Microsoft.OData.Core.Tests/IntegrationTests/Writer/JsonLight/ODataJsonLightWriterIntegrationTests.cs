//---------------------------------------------------------------------
// <copyright file="ODataJsonLightWriterIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;
using ErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.IntegrationTests.Writer.JsonLight
{
    public class ODataJsonLightWriterIntegrationTests
    {
        [Fact]
        public void ShouldBeAbleToWriteEntryWithIdOnly()
        {
            IEdmEntityType entityType = GetEntityType();
            IEdmEntitySet entitySet = GetEntitySet(entityType);
            const string expected = "{\"@odata.id\":\"http://test.org/EntitySet('1')\"}";
            var actual = WriteJsonLightEntry(true, null, false, new ODataResource() { Id = new Uri("http://test.org/EntitySet('1')") }, entitySet, entityType);
            actual.Should().Be(expected);
        }

        [Fact]
        public void ShouldBeAbleToWriteEntryInRequestWithoutSpecifyingEntitySetOrMetadataDocumentUri()
        {
            IEdmEntityType entityType = GetEntityType();
            IEdmEntitySet entitySet = GetEntitySet(entityType);
            const string expected = "{}";
            var actual = WriteJsonLightEntry(true, null, false, new ODataResource(), entitySet, entityType);
            actual.Should().Be(expected);
        }

        [Fact]
        public void ShouldNotBeAbleToWriteEntryInResponseWithoutSpecifyingEntitySet()
        {
            IEdmEntityType entityType = GetEntityType();
            IEdmEntitySet entitySet = GetEntitySet(entityType);
            Action writeEmptyEntry = () => WriteJsonLightEntry(false, new Uri("http://temp.org/"), false, new ODataResource(), entitySet, entityType);
            writeEmptyEntry.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataResourceTypeContext_MetadataOrSerializationInfoMissing);
        }

        [Fact]
        public void ShouldNotBeAbleToWriteEntryInResponseWithoutSpecifyingMetadataDocumentUri()
        {
            IEdmEntityType entityType = GetEntityType();
            IEdmEntitySet entitySet = GetEntitySet(entityType);
            Action writeEmptyEntry = () => WriteJsonLightEntry(false, null, true, new ODataResource(), entitySet, entityType);
            writeEmptyEntry.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataOutputContext_MetadataDocumentUriMissing);
        }

        [Fact]
        public void ShouldBeAbleToWriteTransientEntry()
        {
            IEdmEntityType entityType = GetEntityType();
            IEdmEntitySet entitySet = GetEntitySet(entityType);
            ODataResource transientEntry = new ODataResource() { IsTransient = true };
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

            ODataResource entry = new ODataResource() { Properties = new[] { new ODataProperty { Name = "Key", Value = "son" }, } };
            var actual = WriteJsonLightEntry(
                isRequest: false,
                serviceDocumentUri: new Uri("http://temp.org/"),
                specifySet: true,
                odataEntry: entry,
                entitySet: containedEntitySet,
                resourceType: containedEntitySet.Type as IEdmEntityType,
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

            ODataResource entry = new ODataResource() { Properties = new[] { new ODataProperty { Name = "Key", Value = "son" }, } };
            Action writeContainedEntry = () => WriteJsonLightEntry(
                isRequest: false,
                serviceDocumentUri: new Uri("http://temp.org/"),
                specifySet: true,
                odataEntry: entry,
                entitySet: containedEntitySet,
                resourceType: containedEntitySet.Type as IEdmEntityType,
                odataUri: null);
            writeContainedEntry.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataMetadataBuilder_MissingParentIdOrContextUrl);
        }

        [Fact]
        public void EditLinkShouldNotBeWrittenIfItsReadonlyEntry()
        {
            IEdmEntityType entityType = GetEntityType();
            IEdmEntitySet entitySet = GetEntitySet(entityType);

            var entry = new ODataResource
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

            var entry = new ODataResource
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

            var entry = new ODataResource
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

        [Fact]
        public void WriteEntityWithInvalidPropertyNames()
        {
            EdmModel model = new EdmModel();
            EdmEntityType entityType = AddAndGetEntityType(model);
            var entitySet = GetEntitySet(model, entityType);

            var requestUri = new Uri("http://temp.org/FakeSet('parent')");
            var odataUri = new ODataUri { RequestUri = requestUri };
            odataUri.Path = new ODataUriParser(model, new Uri("http://temp.org/"), requestUri).ParsePath();

            ODataResource res = new ODataResource() { Properties = new[] 
                {
                    new ODataProperty { Name = "Key", Value = "son" },
                    new ODataProperty { Name = "@invalid", Value = "@invalid" },
                    new ODataProperty { Name = "in.valid", Value = "in.valid" },
                    new ODataProperty { Name = "in@val.id", Value = "in@val.id" },
                    new ODataProperty { Name = "odata@odata.odata", Value = "odata@odata.odata" },
                }
            };

            var actual = WriteJsonLightEntry(
                isRequest: false,
                serviceDocumentUri: new Uri("http://temp.org/"),
                specifySet: true,
                odataEntry: null,
                entitySet: entitySet,
                resourceType: entityType,
                odataUri: odataUri,
                writeAction: (writer) =>
                {
                    writer.WriteStart(res);
                    writer.WriteEnd();
                },
                ValidatePropertyNames:false
                );

            var expected = "{\"@odata.context\":\"http://temp.org/$metadata#FakeSet/$entity\",\"@odata.id\":\"FakeSet('son')\",\"@odata.editLink\":\"FakeSet('son')\",\"Key\":\"son\",\"@invalid\":\"@invalid\",\"in.valid\":\"in.valid\",\"in@val.id\":\"in@val.id\",\"odata@odata.odata\":\"odata@odata.odata\"}";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void WriteEntityWithComplexProperty()
        {
            EdmModel model = new EdmModel();
            EdmComplexType complexType = AddAndGetComplexType(model);
            EdmEntityType entityType = AddAndGetEntityType(model);
            entityType.AddStructuralProperty("ComplexP", new EdmComplexTypeReference(complexType, false));
            var entitySet = GetEntitySet(model, entityType);

            var requestUri = new Uri("http://temp.org/FakeSet('parent')");
            var odataUri = new ODataUri { RequestUri = requestUri };
            odataUri.Path = new ODataUriParser(model, new Uri("http://temp.org/"), requestUri).ParsePath();

            ODataResource res = new ODataResource() { Properties = new[] { new ODataProperty { Name = "Key", Value = "son" } } };
            ODataNestedResourceInfo nestedResInfo = new ODataNestedResourceInfo() { Name = "ComplexP" };
            ODataResource nestedRes = new ODataResource() { Properties = new[] { new ODataProperty { Name = "P1", Value = "cv" } } };

            var actual = WriteJsonLightEntry(
                isRequest: false,
                serviceDocumentUri: new Uri("http://temp.org/"),
                specifySet: true,
                odataEntry: null,
                entitySet: entitySet,
                resourceType: entityType,
                odataUri: odataUri,
                writeAction: (writer) =>
                {
                    writer.WriteStart(res);
                    writer.WriteStart(nestedResInfo);
                    writer.WriteStart(nestedRes);
                    writer.WriteEnd();
                    writer.WriteEnd();
                    writer.WriteEnd();
                });

            var expected = "{\"@odata.context\":\"http://temp.org/$metadata#FakeSet/$entity\",\"@odata.id\":\"FakeSet('son')\",\"@odata.editLink\":\"FakeSet('son')\",\"Key\":\"son\",\"ComplexP\":{\"P1\":\"cv\"}}";

            Assert.Equal(expected, actual);

        }

        [Fact]
        public void WriteEntityWithCollectionOfComplexProperty()
        {
            EdmModel model = new EdmModel();
            EdmComplexType complexType = AddAndGetComplexType(model);
            EdmEntityType entityType = AddAndGetEntityType(model);
            entityType.AddStructuralProperty("CollectionOfComplexP",
                new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(complexType, true))));
            var entitySet = GetEntitySet(model, entityType);

            var requestUri = new Uri("http://temp.org/FakeSet('parent')/");
            var odataUri = new ODataUri { RequestUri = requestUri };
            odataUri.Path = new ODataUriParser(model, new Uri("http://temp.org/"), requestUri).ParsePath();

            ODataResource entry = new ODataResource() { Properties = new[] { new ODataProperty { Name = "Key", Value = "parent" }, } };

            ODataNestedResourceInfo nestedResourceInfoForCollectionP = new ODataNestedResourceInfo
            {
                IsCollection = true,
                Name = "CollectionOfComplexP"
            };

            ODataResourceSet collectionP = new ODataResourceSet();
            ODataResource complexP = new ODataResource()
            {
                Properties = new[] { new ODataProperty { Name = "P1", Value = "complexValue" }, }
            };

            var actual = WriteJsonLightEntry(
                isRequest: false,
                serviceDocumentUri: new Uri("http://temp.org/"),
                specifySet: true,
                odataEntry: null,
                entitySet: entitySet,
                resourceType: entityType,
                odataUri: odataUri,
                writeAction: (writer) =>
                {
                    writer.WriteStart(entry);
                    writer.WriteStart(nestedResourceInfoForCollectionP);
                    writer.WriteStart(collectionP);
                    writer.WriteStart(complexP);
                    writer.WriteEnd();
                    writer.WriteEnd();
                    writer.WriteEnd();
                    writer.WriteEnd();
                });

            var expected = "{" +
  "\"@odata.context\":\"http://temp.org/$metadata#FakeSet/$entity\"," +
  "\"@odata.id\":\"FakeSet('parent')\"," +
  "\"@odata.editLink\":\"FakeSet('parent')\"," +
  "\"Key\":\"parent\"," +
  "\"CollectionOfComplexP\":" +
  "[" +
    "{" +
      "\"P1\":\"complexValue\"" +
    "}" +
  "]" +
"}";
            actual.Should().Be(expected);
        }

        [Fact]
        public void WriteEntityWithCollectionOfComplexPropertyInherit()
        {
            EdmModel model = new EdmModel();
            EdmComplexType complexType = AddAndGetComplexType(model);
            EdmComplexType derivedComplexType = new EdmComplexType("Fake", "DerivedComplexType", complexType);
            derivedComplexType.AddStructuralProperty("D1", EdmPrimitiveTypeKind.String);
            model.AddElement(derivedComplexType);
            EdmEntityType entityType = AddAndGetEntityType(model);
            entityType.AddStructuralProperty("CollectionOfComplexP",
                new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(complexType, true))));
            var entitySet = GetEntitySet(model, entityType);

            var requestUri = new Uri("http://temp.org/FakeSet('parent')/");
            var odataUri = new ODataUri { RequestUri = requestUri };
            odataUri.Path = new ODataUriParser(model, new Uri("http://temp.org/"), requestUri).ParsePath();

            ODataResource entry = new ODataResource() { Properties = new[] { new ODataProperty { Name = "Key", Value = "parent" }, } };

            ODataNestedResourceInfo nestedResourceInfoForCollectionP = new ODataNestedResourceInfo
            {
                IsCollection = true,
                Name = "CollectionOfComplexP"
            };

            ODataResourceSet collectionP = new ODataResourceSet();
            ODataResource complexP = new ODataResource()
            {
                Properties = new[] { new ODataProperty { Name = "P1", Value = "complexValue" }, }
            };

            ODataResource derivedComplexP = new ODataResource()
            {
                TypeName = "Fake.DerivedComplexType",
                Properties = new[] { new ODataProperty { Name = "P1", Value = "complexValue" }, new ODataProperty { Name = "D1", Value = "derivedComplexValue" }, }
            };

            var actual = WriteJsonLightEntry(
                isRequest: false,
                serviceDocumentUri: new Uri("http://temp.org/"),
                specifySet: true,
                odataEntry: null,
                entitySet: entitySet,
                resourceType: entityType,
                odataUri: odataUri,
                writeAction: (writer) =>
                {
                    writer.WriteStart(entry);
                    writer.WriteStart(nestedResourceInfoForCollectionP);
                    writer.WriteStart(collectionP);
                    writer.WriteStart(complexP);
                    writer.WriteEnd();
                    writer.WriteStart(derivedComplexP);
                    writer.WriteEnd();
                    writer.WriteEnd();
                    writer.WriteEnd();
                    writer.WriteEnd();
                },
                model: model);

            var expected = "{" +
  "\"@odata.context\":\"http://temp.org/$metadata#FakeSet/$entity\"," +
  "\"@odata.id\":\"FakeSet('parent')\"," +
  "\"@odata.editLink\":\"FakeSet('parent')\"," +
  "\"Key\":\"parent\"," +
  "\"CollectionOfComplexP\":" +
  "[" +
    "{" +
      "\"P1\":\"complexValue\"" +
    "}," +
    "{" +
      "\"@odata.type\":\"#Fake.DerivedComplexType\"," +
      "\"P1\":\"complexValue\"," +
      "\"D1\":\"derivedComplexValue\"" +
    "}" +
  "]" +
"}";
            actual.Should().Be(expected);
        }

        [Fact]
        public void WriteTopLevelComplexProperty()
        {
            EdmModel model = new EdmModel();
            EdmComplexType complexType = AddAndGetComplexType(model);
            EdmEntityType entityType = AddAndGetEntityType(model);
            entityType.AddStructuralProperty("ComplexP", new EdmComplexTypeReference(complexType, false));
            var entitySet = GetEntitySet(model, entityType);

            var requestUri = new Uri("http://temp.org/FakeSet('parent')/ComplexP");
            var odataUri = new ODataUri { RequestUri = requestUri };
            odataUri.Path = new ODataUriParser(model, new Uri("http://temp.org/"), requestUri).ParsePath();

            ODataResource nestedRes = new ODataResource() { Properties = new[] { new ODataProperty { Name = "P1", Value = "cv" } } };

            var actual = WriteJsonLightEntry(
                isRequest: false,
                serviceDocumentUri: new Uri("http://temp.org/"),
                specifySet: false,
                odataEntry: null,
                entitySet: null,
                resourceType: complexType,
                odataUri: odataUri,
                writeAction: (writer) =>
                {
                    writer.WriteStart(nestedRes);
                    writer.WriteEnd();
                });

            var expected = "{\"@odata.context\":\"http://temp.org/$metadata#FakeSet('parent')/ComplexP\",\"P1\":\"cv\"}";

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        private void WriteTopLevelComplexPropertyInDerivedType(bool isRequest)
        {
            EdmModel model = new EdmModel();
            EdmComplexType complexType = AddAndGetComplexType(model);
            EdmComplexType derviedComplexType = new EdmComplexType(complexType.Namespace, "DerivedComplexType", complexType);
            derviedComplexType.AddStructuralProperty("P2", EdmPrimitiveTypeKind.String);
            model.AddElement(derviedComplexType);
            EdmEntityType entityType = AddAndGetEntityType(model);
            entityType.AddStructuralProperty("ComplexP", new EdmComplexTypeReference(complexType, false));
            var entitySet = GetEntitySet(model, entityType);

            var requestUri = new Uri("http://temp.org/FakeSet('parent')/ComplexP");
            var odataUri = new ODataUri { RequestUri = requestUri };
            odataUri.Path = new ODataUriParser(model, new Uri("http://temp.org/"), requestUri).ParsePath();

            ODataResource nestedRes = new ODataResource()
            {
                TypeName = "Fake.DerivedComplexType",
                Properties = new[] { new ODataProperty { Name = "P1", Value = "cv" } }
            };

            var actual = WriteJsonLightEntry(
                isRequest: isRequest,
                serviceDocumentUri: (isRequest ? null : new Uri("http://temp.org/")),
                specifySet: false,
                odataEntry: null,
                entitySet: null,
                resourceType: isRequest ? null : complexType,
                odataUri: isRequest ? null : odataUri,
                writeAction: (writer) =>
                {
                    writer.WriteStart(nestedRes);
                    writer.WriteEnd();
                },
                model:model);

            var expected = isRequest ? "{\"@odata.type\":\"#Fake.DerivedComplexType\",\"P1\":\"cv\"}"
                : "{\"@odata.context\":\"http://temp.org/$metadata#FakeSet('parent')/ComplexP\",\"@odata.type\":\"#Fake.DerivedComplexType\",\"P1\":\"cv\"}";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void WriteTopLevelComplexCollectionProperty()
        {
            WriteTopLevelComplexCollectionProperty(false);
        }

        [Fact]
        public void WriteTopLevelComplexCollectionProperty_Inherit()
        {
            WriteTopLevelComplexCollectionProperty(true);
        }

        private void WriteTopLevelComplexCollectionProperty(bool inherit)
        {
            EdmModel model = new EdmModel();
            EdmComplexType complexType = AddAndGetComplexType(model);

            if (inherit)
            {
                EdmComplexType derivedComplexType = new EdmComplexType("Fake", "DerivedComplexType", complexType);
                derivedComplexType.AddStructuralProperty("D1", EdmPrimitiveTypeKind.String);
                model.AddElement(derivedComplexType);
            }

            EdmEntityType entityType = AddAndGetEntityType(model);
            var collectionOfComplexType = new EdmCollectionType(new EdmComplexTypeReference(complexType, true));
            entityType.AddStructuralProperty("CollectionOfComplexP",
                new EdmCollectionTypeReference(collectionOfComplexType));
            var entitySet = GetEntitySet(model, entityType);

            var requestUri = new Uri("http://temp.org/FakeSet('parent')/CollectionOfComplexP");
            var odataUri = new ODataUri { RequestUri = requestUri };
            odataUri.Path = new ODataUriParser(model, new Uri("http://temp.org/"), requestUri).ParsePath();

            ODataResourceSet collectionP = new ODataResourceSet();
            ODataResource complexP = new ODataResource()
            {
                Properties = new[] { new ODataProperty { Name = "P1", Value = "complexValue" }, }
            };

            Action<ODataWriter> write = (writer) =>
            {
                writer.WriteStart(collectionP);
                writer.WriteStart(complexP);
                writer.WriteEnd();
                writer.WriteEnd();
            };

            var expected = "{" +
                  "\"@odata.context\":\"http://temp.org/$metadata#FakeSet('parent')/CollectionOfComplexP\"," +
                  "\"value\":" +
                  "[" +
                    "{" +
                      "\"P1\":\"complexValue\"" +
                    "}" +
                  "]" +
                "}";

            if (inherit)
            {
                ODataResource dComplexP = new ODataResource()
                {
                    TypeName = "Fake.DerivedComplexType",
                    Properties = new[]
                    {
                        new ODataProperty { Name = "P1", Value = "complexValue"},
                        new ODataProperty { Name = "D1", Value = "derivedComplexValue" }
                    }
                };

                write = (writer) =>
                {
                    writer.WriteStart(collectionP);
                    writer.WriteStart(complexP);
                    writer.WriteEnd();
                    writer.WriteStart(dComplexP);
                    writer.WriteEnd();
                    writer.WriteEnd();
                };

                expected = "{" +
                  "\"@odata.context\":\"http://temp.org/$metadata#FakeSet('parent')/CollectionOfComplexP\"," +
                  "\"value\":" +
                  "[" +
                    "{" +
                      "\"P1\":\"complexValue\"" +
                    "}," +
                    "{" +
                      "\"@odata.type\":\"#Fake.DerivedComplexType\"," +
                      "\"P1\":\"complexValue\"," +
                      "\"D1\":\"derivedComplexValue\"" +
                    "}" +
                  "]" +
                "}";
            }

            var actual = WriteJsonLightEntry(
                isRequest: false,
                serviceDocumentUri: new Uri("http://temp.org/"),
                specifySet: false,
                odataEntry: null,
                entitySet: null,
                resourceType: complexType,
                odataUri: odataUri,
                writeAction: write,
                isResourceSet: true,
                model: model);

            actual.Should().Be(expected);
        }

        [Fact]
        public void WriteOpenEntryWithUndeclaredProperty()
        {
            ODataResource res = new ODataResource() { Properties = new[] { new ODataProperty { Name = "Key", Value = "son" }, new ODataProperty { Name = "OpenProperty", Value = "Open" } } };
            ODataNestedResourceInfo nestedComplexInfo = new ODataNestedResourceInfo() { Name = "OpenComplex" };
            ODataResource nestedComplex = new ODataResource() { TypeName = "Fake.ComplexType", Properties = new[] { new ODataProperty { Name = "P1", Value = "cv" } } };
            ODataNestedResourceInfo nestedResInfo = new ODataNestedResourceInfo() { Name = "OpenNavigationProperty", IsCollection = false };
            ODataResource nestedRes = new ODataResource()
            {
                Id = new Uri("http://temp.org/Type"),
                EditLink = new Uri("http://temp.org/Type"),
                ReadLink = new Uri("http://temp.org/Type"),
                ETag = "etag",
                TypeName = "Fake.Type",
                Properties = new[] { new ODataProperty { Name = "Key", Value = "son" } }
            };

            var actual = WriteJsonLightEntryForUndeclared(
                isOpenType: true,
                writeAction: (writer) =>
                {
                    writer.WriteStart(res);
                    writer.WriteStart(nestedComplexInfo);
                    writer.WriteStart(nestedComplex);
                    writer.WriteEnd();
                    writer.WriteEnd();
                    writer.WriteStart(nestedResInfo);
                    writer.WriteStart(nestedRes);
                    writer.WriteEnd();
                    writer.WriteEnd();
                    writer.WriteEnd();
                });

            var expected = "{" +
                           "\"@odata.context\":\"http://temp.org/$metadata#FakeSet/$entity\"," +
                           "\"@odata.id\":\"FakeSet('son')\"," +
                           "\"@odata.editLink\":\"FakeSet('son')\"," +
                           "\"Key\":\"son\"," +
                           "\"OpenProperty\":\"Open\"," +
                           "\"OpenComplex\":" +
                           "{" +
                               "\"@odata.type\":\"#Fake.ComplexType\"," +
                               "\"P1\":\"cv\"" +
                           "}," +
                           "\"OpenNavigationProperty@odata.associationLink\":\"http://temp.org/FakeSet('son')/OpenNavigationProperty/$ref\"," +
                           "\"OpenNavigationProperty@odata.navigationLink\":\"http://temp.org/FakeSet('son')/OpenNavigationProperty\"," +
                           "\"OpenNavigationProperty\":" +
                           "{" +
                               "\"@odata.type\":\"#Fake.Type\"," +
                               "\"@odata.id\":\"http://temp.org/Type\"," +
                               "\"@odata.etag\":\"etag\"," +
                               "\"@odata.editLink\":\"http://temp.org/Type\"," +
                               "\"Key\":\"son\"" +
                           "}" +
                           "}";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void WriteNonOpenEntryWithUndeclaredProperty()
        {
            ODataResource res = new ODataResource() { Properties = new[] { new ODataProperty { Name = "Key", Value = "son" }, new ODataProperty { Name = "OpenProperty", Value = "Open" } } };
            ODataNestedResourceInfo nestedComplexInfo = new ODataNestedResourceInfo() { Name = "OpenComplex" };
            ODataResource nestedComplex = new ODataResource() { TypeName = "Fake.ComplexType", Properties = new[] { new ODataProperty { Name = "P1", Value = "cv" } } };
            ODataNestedResourceInfo nestedResInfo = new ODataNestedResourceInfo() { Name = "OpenNavigationProperty", IsCollection = false };
            ODataResource nestedRes = new ODataResource()
            {
                Id = new Uri("http://temp.org/Type"),
                EditLink = new Uri("http://temp.org/Type"),
                ReadLink = new Uri("http://temp.org/Type"),
                ETag = "etag",
                TypeName = "Fake.Type",
                Properties = new[] { new ODataProperty { Name = "Key", Value = "son" } }
            };

            var actual = WriteJsonLightEntryForUndeclared(
                isOpenType: false,
                writeAction: (writer) =>
                {
                    writer.WriteStart(res);
                    writer.WriteStart(nestedComplexInfo);
                    writer.WriteStart(nestedComplex);
                    writer.WriteEnd();
                    writer.WriteEnd();
                    writer.WriteStart(nestedResInfo);
                    writer.WriteStart(nestedRes);
                    writer.WriteEnd();
                    writer.WriteEnd();
                    writer.WriteEnd();
                },
                throwOnUndeclaredProperty: false
                );

            var expected = "{" +
                           "\"@odata.context\":\"http://temp.org/$metadata#FakeSet/$entity\"," +
                           "\"@odata.id\":\"FakeSet('son')\"," +
                           "\"@odata.editLink\":\"FakeSet('son')\"," +
                           "\"Key\":\"son\"," +
                           "\"OpenProperty\":\"Open\"," +
                           "\"OpenComplex\":" +
                           "{" +
                               "\"@odata.type\":\"#Fake.ComplexType\"," +
                               "\"P1\":\"cv\"" +
                           "}," +
                           "\"OpenNavigationProperty@odata.associationLink\":\"http://temp.org/FakeSet('son')/OpenNavigationProperty/$ref\"," +
                           "\"OpenNavigationProperty@odata.navigationLink\":\"http://temp.org/FakeSet('son')/OpenNavigationProperty\"," +
                           "\"OpenNavigationProperty\":" +
                           "{" +
                               "\"@odata.type\":\"#Fake.Type\"," +
                               "\"@odata.id\":\"http://temp.org/Type\"," +
                               "\"@odata.etag\":\"etag\"," +
                               "\"@odata.editLink\":\"http://temp.org/Type\"," +
                               "\"Key\":\"son\"" +
                           "}" +
                           "}";
            Assert.Equal(expected, actual);

            // Should throw on undeclared primitive value property
            res = new ODataResource() { Properties = new[] { new ODataProperty { Name = "Key", Value = "son" }, new ODataProperty { Name = "OpenProperty", Value = "Open" } } };

            Action writeEntry = () => WriteJsonLightEntryForUndeclared(
                isOpenType: false,
                writeAction: (writer) =>
                {
                    writer.WriteStart(res);
                    writer.WriteEnd();
                },
                throwOnUndeclaredProperty: true
                );
            writeEntry.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ValidationUtils_PropertyDoesNotExistOnType("OpenProperty", "Fake.Type"));

            // Should throw on undeclared complex value property
            res = new ODataResource() { Properties = new[] { new ODataProperty { Name = "Key", Value = "son" } } };
            nestedComplexInfo = new ODataNestedResourceInfo() { Name = "OpenComplex" };
            nestedComplex = new ODataResource() { TypeName = "Fake.ComplexType", Properties = new[] { new ODataProperty { Name = "P1", Value = "cv" } } };

            writeEntry = () => WriteJsonLightEntryForUndeclared(
                isOpenType: false,
                writeAction: (writer) =>
                {
                    writer.WriteStart(res);
                    writer.WriteStart(nestedComplexInfo);
                    writer.WriteStart(nestedComplex);
                    writer.WriteEnd();
                    writer.WriteEnd();
                    writer.WriteEnd();
                },
                throwOnUndeclaredProperty: true
                );
            writeEntry.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ValidationUtils_PropertyDoesNotExistOnType("OpenComplex", "Fake.Type"));

            // Should throw on undeclared navigation property
            res = new ODataResource() { Properties = new[] { new ODataProperty { Name = "Key", Value = "son" } } };
            nestedResInfo = new ODataNestedResourceInfo() { Name = "OpenNavigationProperty", IsCollection = false };
            nestedRes = new ODataResource()
            {
                Id = new Uri("http://temp.org/Type"),
                EditLink = new Uri("http://temp.org/Type"),
                ReadLink = new Uri("http://temp.org/Type"),
                ETag = "etag",
                TypeName = "Fake.Type",
                Properties = new[] { new ODataProperty { Name = "Key", Value = "son" } }
            };

            writeEntry = () => WriteJsonLightEntryForUndeclared(
                isOpenType: false,
                writeAction: (writer) =>
                {
                    writer.WriteStart(res);
                    writer.WriteStart(nestedResInfo);
                    writer.WriteStart(nestedRes);
                    writer.WriteEnd();
                    writer.WriteEnd();
                    writer.WriteEnd();
                },
                throwOnUndeclaredProperty: true
                );
            writeEntry.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ValidationUtils_PropertyDoesNotExistOnType("OpenNavigationProperty", "Fake.Type"));
        }

        private static string WriteJsonLightEntry(bool isRequest, Uri serviceDocumentUri, bool specifySet, ODataResource odataEntry, IEdmNavigationSource entitySet, IEdmEntityType entityType)
        {
            return WriteJsonLightEntry(isRequest, serviceDocumentUri, specifySet, odataEntry, entitySet, entityType, odataUri: null, writeAction: null, isResourceSet: false);
        }

        private static string WriteJsonLightEntry(bool isRequest, Uri serviceDocumentUri, bool specifySet,
            ODataResource odataEntry, IEdmNavigationSource entitySet, IEdmStructuredType resourceType,
            ODataUri odataUri, Action<ODataWriter> writeAction = null, bool isResourceSet = false,
            EdmModel model = null,
            bool ValidatePropertyNames = true)
        {
            if (model == null)
            {
                model = new EdmModel();
                model.AddElement(new EdmEntityContainer("Fake", "Container_sub"));
            }

            var stream = new MemoryStream();
            var message = new InMemoryMessage { Stream = stream };

            var settings = new ODataMessageWriterSettings { Version = ODataVersion.V4 };
            settings.ODataUri = odataUri;
            settings.SetServiceDocumentUri(serviceDocumentUri);

            settings.SetContentType(ODataFormat.Json);
            settings.SetContentType("application/json;odata.metadata=full", null);

            if (!ValidatePropertyNames)
            {
                settings.Validations = ValidationKinds.None;
            }

            ODataMessageWriter messageWriter;
            if (isRequest)
            {
                messageWriter = new ODataMessageWriter((IODataRequestMessage)message, settings, model ?? TestUtils.WrapReferencedModelsToMainModel("Fake", "Container", model));
            }
            else
            {
                messageWriter = new ODataMessageWriter((IODataResponseMessage)message, settings, model ?? TestUtils.WrapReferencedModelsToMainModel("Fake", "Container", model));
            }

            ODataWriter writer = null;
            if (isResourceSet)
            {
                writer = messageWriter.CreateODataResourceSetWriter(entitySet as IEdmEntitySetBase, resourceType);
            }
            else
            {
                writer = messageWriter.CreateODataResourceWriter(specifySet ? entitySet : null, resourceType);
            }

            if (writeAction != null)
            {
                writeAction(writer);
            }
            else if (!isResourceSet)
            {
                writer.WriteStart(odataEntry);
                writer.WriteEnd();
            }

            writer.Flush();

            var actual = Encoding.UTF8.GetString(stream.ToArray());
            return actual;
        }

        private static string WriteJsonLightEntryForUndeclared(bool isOpenType, Action<ODataWriter> writeAction, bool throwOnUndeclaredProperty = true)
        {
            EdmModel model = new EdmModel();
            AddAndGetComplexType(model);
            EdmEntityType entityType = AddAndGetEntityType(model);
            if (isOpenType)
            {
                entityType = AddAndGetOpenEntityType(model);
            }
            var entitySet = GetEntitySet(model, entityType);

            var requestUri = new Uri("http://temp.org/FakeSet('parent')");
            var odataUri = new ODataUri { RequestUri = requestUri };
            odataUri.Path = new ODataUriParser(model, new Uri("http://temp.org/"), requestUri).ParsePath();

            var stream = new MemoryStream();
            var message = new InMemoryMessage { Stream = stream };

            var settings = new ODataMessageWriterSettings { Version = ODataVersion.V4 };
            if (!throwOnUndeclaredProperty)
            {
                settings.Validations &= ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType;
            }
            settings.ODataUri = odataUri;
            settings.SetServiceDocumentUri(new Uri("http://temp.org"));

            settings.SetContentType(ODataFormat.Json);
            settings.SetContentType("application/json;odata.metadata=full", null);

            ODataMessageWriter messageWriter;

            messageWriter = new ODataMessageWriter((IODataResponseMessage)message, settings, model);
            ODataWriter writer = null;
            writer = messageWriter.CreateODataResourceWriter(entitySet, entityType);

            if (writeAction != null)
            {
                writeAction(writer);
            }

            writer.Flush();

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

        private static EdmEntityType AddAndGetEntityType(EdmModel model)
        {
            var type = GetEntityType();
            model.AddElement(type);
            return type;
        }

        private static EdmEntityType GetOpenEntityType()
        {
            EdmEntityType type = new EdmEntityType("Fake", "OpenType", null, false, true);
            type.AddKeys(type.AddStructuralProperty("Key", EdmPrimitiveTypeKind.String));
            return type;
        }

        private static EdmEntityType AddAndGetOpenEntityType(EdmModel model)
        {
            var type = GetOpenEntityType();
            model.AddElement(type);
            return type;
        }

        private static EdmComplexType GetComplexType()
        {
            EdmComplexType type = new EdmComplexType("Fake", "ComplexType");
            type.AddStructuralProperty("P1", EdmPrimitiveTypeKind.String);
            return type;
        }

        private static EdmComplexType AddAndGetComplexType(EdmModel model)
        {
            var type = GetComplexType();
            model.AddElement(type);
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
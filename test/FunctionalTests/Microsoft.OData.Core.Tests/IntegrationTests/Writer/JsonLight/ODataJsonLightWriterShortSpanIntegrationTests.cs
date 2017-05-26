//---------------------------------------------------------------------
// <copyright file="ODataJsonLightWriterShortSpanIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.OData.JsonLight;
using Microsoft.OData.Edm;
using Microsoft.Spatial;
using Xunit;

namespace Microsoft.OData.Tests.IntegrationTests.Writer.JsonLight
{
    public class ODataJsonLightWriterShortSpanIntegrationTests
    {
        private readonly Uri metadataDocumentUri = new Uri("http://odata.org/test/");
        private readonly IEdmModel userModel;
        private readonly EdmEntitySet entitySet;
        private readonly EdmEntityType entityType;
        private readonly EdmEntityType derivedEntityType;
        private readonly EdmEntityType mleEntityType;

        public ODataJsonLightWriterShortSpanIntegrationTests()
        {
            EdmModel tmpModel = new EdmModel();

            EdmComplexType complexType = new EdmComplexType("NS", "MyComplexType");
            EdmComplexTypeReference complexTypeReference = new EdmComplexTypeReference(complexType, isNullable: true);
            complexType.AddProperty(new EdmStructuralProperty(complexType, "StringProperty", EdmCoreModel.Instance.GetString(isNullable: true)));
            complexType.AddProperty(new EdmStructuralProperty(complexType, "ComplexProperty", complexTypeReference));

            EdmComplexType derivedComplexType = new EdmComplexType("NS", "MyDerivedComplexType", complexType, false);
            derivedComplexType.AddProperty(new EdmStructuralProperty(derivedComplexType, "DerivedStringProperty", EdmCoreModel.Instance.GetString(isNullable: true)));

            this.entityType = new EdmEntityType("NS", "MyEntityType", isAbstract: false, isOpen: true, baseType: null);
            this.entityType.AddProperty(new EdmStructuralProperty(this.entityType, "StreamProperty", EdmCoreModel.Instance.GetStream(isNullable: true)));
            this.entityType.AddProperty(new EdmStructuralProperty(this.entityType, "StringProperty", EdmCoreModel.Instance.GetString(isNullable: true)));
            this.entityType.AddProperty(new EdmStructuralProperty(this.entityType, "ComplexProperty", complexTypeReference));
            EdmCollectionTypeReference stringCollectionType = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(isNullable: false)));
            this.entityType.AddProperty(new EdmStructuralProperty(this.entityType, "PrimitiveCollectionProperty", stringCollectionType));
            EdmCollectionTypeReference nullableStringCollectionType = new EdmCollectionTypeReference(
                new EdmCollectionType(EdmCoreModel.Instance.GetString(isNullable: true)));
            this.entityType.AddProperty(new EdmStructuralProperty(
                this.entityType,
                "NullablePrimitiveCollectionProperty",
                nullableStringCollectionType));
            EdmCollectionTypeReference nullableIntCollectionType = new EdmCollectionTypeReference(
                new EdmCollectionType(EdmCoreModel.Instance.GetInt32(isNullable: true)));
            this.entityType.AddProperty(new EdmStructuralProperty(
                this.entityType,
                "NullableIntCollectionProperty",
                nullableIntCollectionType));
            EdmCollectionTypeReference complexCollectionType = new EdmCollectionTypeReference(new EdmCollectionType(complexTypeReference));
            this.entityType.AddProperty(new EdmStructuralProperty(this.entityType, "ComplexCollectionProperty", complexCollectionType));

            this.entityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "EntityReferenceProperty", Target = this.entityType, TargetMultiplicity = EdmMultiplicity.ZeroOrOne });
            this.entityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "EntitySetReferenceProperty", Target = this.entityType, TargetMultiplicity = EdmMultiplicity.Many });

            this.derivedEntityType = new EdmEntityType("NS", "MyDerivedEntityType", isAbstract: false, isOpen: true, baseType: this.entityType);
            this.mleEntityType = new EdmEntityType("NS", "MyMleEntityType", isAbstract: false, isOpen: true, hasStream: true, baseType: this.derivedEntityType);

            tmpModel.AddElement(this.entityType);
            tmpModel.AddElement(this.derivedEntityType);
            tmpModel.AddElement(this.mleEntityType);
            tmpModel.AddElement(complexType);
            tmpModel.AddElement(derivedComplexType);

            var defaultContainer = new EdmEntityContainer("NS", "DefaultContainer_sub");
            tmpModel.AddElement(defaultContainer);

            this.entitySet = new EdmEntitySet(defaultContainer, "MySet", this.entityType);

            var entityTypeReference = new EdmEntityTypeReference(this.entityType, isNullable: false);

            EdmAction action = new EdmAction("NS", "Action1", null /*returnType*/, true /*isBound*/, null /*entitySetPath*/);
            action.AddParameter( "bindingParameter", entityTypeReference);
            tmpModel.AddElement(action);
            defaultContainer.AddActionImport("Action1", action);

            EdmFunction function = new EdmFunction("NS", "Action1", EdmCoreModel.Instance.GetInt32(true) /*returnType*/, true /*isBound*/, null /*entitySetPath*/, false /*isComposable*/);
            function.AddParameter("bindingParameter", entityTypeReference);
            tmpModel.AddElement(function);
            defaultContainer.AddFunctionImport("Function1", function);

            this.userModel = TestUtils.WrapReferencedModelsToMainModel("NS", "DefaultContainer", tmpModel);
        }

        #region Context Uri tests

        #region Without Metadata Document Uri
        [Fact]
        public void ShouldThrowWhenCreatingResponseWriterWithoutMetadataDocumentUri()
        {
            var stream = new MemoryStream();
            ODataJsonLightOutputContext outputContext = CreateJsonLightOutputContext(stream, writingResponse: true, userModel: null, serviceDocumentUri: null);
            Action action = () => new ODataJsonLightWriter(outputContext, navigationSource: null, resourceType: null, writingResourceSet: true);
            action.ShouldThrow<ODataException>().WithMessage(Strings.ODataOutputContext_MetadataDocumentUriMissing);
        }

        [Fact]
        public void ShouldNotWriteContextUriForFeedRequestWithoutUserModelAndWithoutMetadataDocumentUri()
        {
            this.WriteNestedItemsAndValidatePayload("MySet", "NS.MyDerivedEntitytype", nestedItemToWrite: new[] { new ODataResourceSet() }, expectedPayload: "{\"value\":[]}", writingResponse: false, setMetadataDocumentUri: false);
        }

        [Fact]
        public void ShouldNotWriteContextUriForEntryRequestWithoutUserModelAndWithoutMetadataDocumentUri()
        {
            this.WriteNestedItemsAndValidatePayload("MySet", "NS.MyDerivedEntitytype", nestedItemToWrite: new[] { new ODataResource() }, expectedPayload: "{}", writingResponse: false, setMetadataDocumentUri: false);
        }

        [Fact]
        public void ShouldNotWriteContextUriForFeedRequestWithUserModelAndWithoutMetadataDocumentUri()
        {
            this.WriteNestedItemsAndValidatePayload(this.entitySet, this.derivedEntityType, nestedItemToWrite: new[] { new ODataResourceSet() }, expectedPayload: "{\"value\":[]}", writingResponse: false, setMetadataDocumentUri: false);
        }

        [Fact]
        public void ShouldNotWriteContextUriForEntryRequestWithUserModelAndWithoutMetadataDocumentUri()
        {
            this.WriteNestedItemsAndValidatePayload(this.entitySet, this.derivedEntityType, nestedItemToWrite: new[] { new ODataResource() }, expectedPayload: "{}", writingResponse: false, setMetadataDocumentUri: false);
        }
        #endregion Without Metadata Document Uri

        [Fact]
        public void ShouldWriteContextUriForFeedRequestWithoutUserModel()
        {
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: "MySet", derivedEntityTypeFullName: null, nestedItemToWrite: new[] { new ODataResourceSet() }, expectedPayload: "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet\",\"value\":[]}", writingResponse: false);
        }

        [Fact]
        public void ShouldNotWriteContextUriForFeedRequestWithoutUserModel()
        {
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: null, derivedEntityTypeFullName: null, nestedItemToWrite: new[] { new ODataResourceSet() }, expectedPayload: "{\"value\":[]}", writingResponse: false);
        }

        [Fact]
        public void ShouldWriteContextUriForFeedResponseWithoutUserModel()
        {
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: "MySet", derivedEntityTypeFullName: "NS.MyDerivedEntityType", nestedItemToWrite: new[] { new ODataResourceSet() }, expectedPayload: "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/NS.MyDerivedEntityType\",\"value\":[]}", writingResponse: true);
        }

        [Fact]
        public void ShouldThrowWhenWritingFeedResponseWithoutUserModelAndWithoutSetName()
        {
            Action action = () => this.WriteNestedItemsAndValidatePayload(entitySetFullName: null, derivedEntityTypeFullName: "NS.MyDerivedEntityType", nestedItemToWrite: new[] { new ODataResourceSet() }, expectedPayload: "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/NS.MyDerivedEntityType\",\"value\":[]}", writingResponse: true);
            action.ShouldThrow<ODataException>().WithMessage(Strings.ODataResourceTypeContext_MetadataOrSerializationInfoMissing);
        }

        [Fact]
        public void ShouldWriteContextUriForEntryRequestWithoutUserModel()
        {
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: "MySet", derivedEntityTypeFullName: null, nestedItemToWrite: new[] { new ODataResource() }, expectedPayload: "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\"}", writingResponse: false);
        }

        [Fact]
        public void ShouldNotWriteContextUriForEntryRequestWithoutUserModel()
        {
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: null, derivedEntityTypeFullName: null, nestedItemToWrite: new[] { new ODataResource() }, expectedPayload: "{}", writingResponse: false);
        }

        [Fact]
        public void ShouldWriteContextUriForEntryResponseWithoutUserModel()
        {
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: "MySet", derivedEntityTypeFullName: "NS.MyDerivedEntityType", nestedItemToWrite: new[] { new ODataResource() }, expectedPayload: "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/NS.MyDerivedEntityType/$entity\"}", writingResponse: true);
        }

        [Fact]
        public void ShouldThrowWhenWritingEntryResponseWithoutUserModelAndWithoutSetName()
        {
            Action action = () => this.WriteNestedItemsAndValidatePayload(entitySetFullName: null, derivedEntityTypeFullName: "NS.MyDerivedEntityType", nestedItemToWrite: new[] { new ODataResource() }, expectedPayload: "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/NS.MyDerivedEntityType/$entity\"}", writingResponse: true);
            action.ShouldThrow<ODataException>().WithMessage(Strings.ODataResourceTypeContext_MetadataOrSerializationInfoMissing);
        }

        [Fact]
        public void ShouldWriteContextUriForFeedRequestWithUserModel()
        {
            this.WriteNestedItemsAndValidatePayload(this.entitySet, this.entityType, nestedItemToWrite: new[] { new ODataResourceSet() }, expectedPayload: "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet\",\"value\":[]}", writingResponse: false);
        }

        [Fact]
        public void ShouldNotWriteContextUriForFeedRequestWithUserModel()
        {
            this.WriteNestedItemsAndValidatePayload(entitySet: null, entityType: null, nestedItemToWrite: new[] { new ODataResourceSet() }, expectedPayload: "{\"value\":[]}", writingResponse: false);
        }

        [Fact]
        public void ShouldWriteContextUriForFeedResponseWithUserModel()
        {
            this.WriteNestedItemsAndValidatePayload(this.entitySet, this.derivedEntityType, nestedItemToWrite: new[] { new ODataResourceSet() }, expectedPayload: "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/NS.MyDerivedEntityType\",\"value\":[]}", writingResponse: true);
        }

        [Fact]
        public void ShouldThrowWhenWritingFeedResponseWithUserModelAndWithoutSet()
        {
            Action action = () => this.WriteNestedItemsAndValidatePayload(/*entitySet*/ null, this.derivedEntityType, nestedItemToWrite: new[] { new ODataResourceSet() }, expectedPayload: "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/NS.MyDerivedEntityType\",\"value\":[]}", writingResponse: true);
            action.ShouldThrow<ODataException>().WithMessage(Strings.ODataResourceTypeContext_MetadataOrSerializationInfoMissing);
        }

        [Fact]
        public void ShouldWriteContextUriForEntryRequestWithUserModel()
        {
            this.WriteNestedItemsAndValidatePayload(this.entitySet, this.entityType, nestedItemToWrite: new[] { new ODataResource() }, expectedPayload: "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\"}", writingResponse: false);
        }

        [Fact]
        public void ShouldWriteContextUriForEntryResponseWithUserModel()
        {
            this.WriteNestedItemsAndValidatePayload(this.entitySet, this.derivedEntityType, nestedItemToWrite: new[] { new ODataResource() }, expectedPayload: "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/NS.MyDerivedEntityType/$entity\"}", writingResponse: true);
        }

        [Fact]
        public void ShouldThrowWhenWritingEntryResponseWithUserModelAndWithoutSet()
        {
            Action action = () => this.WriteNestedItemsAndValidatePayload(/*entitySet*/ null, this.derivedEntityType, nestedItemToWrite: new[] { new ODataResource() }, expectedPayload: "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/NS.MyDerivedEntityType/$entity\"}", writingResponse: true);
            action.ShouldThrow<ODataException>().WithMessage(Strings.ODataResourceTypeContext_MetadataOrSerializationInfoMissing);
        }
        #endregion Context Uri tests

        #region Media Link Entry tests
        [Fact]
        public void ShouldWriteDefaultStreamForResponseEntryPayloadWithoutUserModel()
        {
            var entry = new ODataResource { TypeName = "NS.MyMleEntityType", MediaResource = new ODataStreamReferenceValue { ReadLink = new Uri("http://odata.org/test/Blob") } };
            const string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyMleEntityType\",\"@odata.mediaReadLink\":\"http://odata.org/test/Blob\"}";
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: "MySet", derivedEntityTypeFullName: null, nestedItemToWrite: new[] { entry }, expectedPayload: payload, writingResponse: true);
        }

        [Fact]
        public void ShouldWriteDefaultStreamForResponseEntryPayloadWithUserModel()
        {
            var entry = new ODataResource { TypeName = "NS.MyMleEntityType", MediaResource = new ODataStreamReferenceValue { ReadLink = new Uri("http://odata.org/test/Blob") } };
            const string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyMleEntityType\",\"@odata.mediaReadLink\":\"http://odata.org/test/Blob\"}";
            this.WriteNestedItemsAndValidatePayload(this.entitySet, entityType: null, nestedItemToWrite: new[] { entry }, expectedPayload: payload, writingResponse: true);
        }

        [Fact]
        public void StreamReadLinkShouldNotBeOmittedWhenNotIdenticalToEditLink()
        {
            var entry = new ODataResource { TypeName = "NS.MyMleEntityType", MediaResource = new ODataStreamReferenceValue { ReadLink = new Uri("http://odata.org/test/Blob/read"), EditLink = new Uri("http://odata.org/test/Blob/edit") } };
            const string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyMleEntityType\",\"@odata.mediaEditLink\":\"http://odata.org/test/Blob/edit\",\"@odata.mediaReadLink\":\"http://odata.org/test/Blob/read\"}";
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: "MySet", derivedEntityTypeFullName: null, nestedItemToWrite: new[] { entry }, expectedPayload: payload, writingResponse: true);
        }

        [Fact]
        public void StreamReadLinkShouldBeOmittedWhenIdenticalToEditLink()
        {
            var entry = new ODataResource { TypeName = "NS.MyMleEntityType", MediaResource = new ODataStreamReferenceValue { ReadLink = new Uri("http://odata.org/test/Blob"), EditLink = new Uri("http://odata.org/test/Blob") } };
            const string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyMleEntityType\",\"@odata.mediaEditLink\":\"http://odata.org/test/Blob\"}";
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: "MySet", derivedEntityTypeFullName: null, nestedItemToWrite: new[] { entry }, expectedPayload: payload, writingResponse: true);
        }

        [Fact]
        public void ShouldWriteDefaultStreamForRequestEntryPayloadWithoutUserModel()
        {
            var entry = new ODataResource { TypeName = "NS.MyMleEntityType", MediaResource = new ODataStreamReferenceValue { ReadLink = new Uri("http://odata.org/test/Blob") } };
            const string payload = "{\"@odata.type\":\"#NS.MyMleEntityType\",\"@odata.mediaReadLink\":\"http://odata.org/test/Blob\"}";
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: null, derivedEntityTypeFullName: null, nestedItemToWrite: new[] { entry }, expectedPayload: payload, writingResponse: false);
        }

        [Fact]
        public void ShouldWriteDefaultStreamForRequestEntryPayloadWithUserModel()
        {
            var entry = new ODataResource { TypeName = "NS.MyMleEntityType", MediaResource = new ODataStreamReferenceValue { ReadLink = new Uri("http://odata.org/test/Blob") } };
            const string payload = "{\"@odata.context\":\"http://odata.org/test/$metadata#NS.MyMleEntityType\",\"@odata.type\":\"#NS.MyMleEntityType\",\"@odata.mediaReadLink\":\"http://odata.org/test/Blob\"}";
            this.WriteNestedItemsAndValidatePayload(entitySet: null, entityType: null, nestedItemToWrite: new[] { entry }, expectedPayload: payload, writingResponse: false);
        }
        #endregion Media Link Entry tests

        #region Stream Property tests
        [Fact]
        public void ShouldWriteStreamPropertyForResponseEntryPayloadWithoutUserModel()
        {
            var entry = new ODataResource { TypeName = "NS.MyDerivedEntityType", Properties = new[] { new ODataProperty { Name = "StreamProperty", Value = new ODataStreamReferenceValue { ReadLink = new Uri("http://odata.org/test/Blob") } } } };
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"StreamProperty@odata.mediaReadLink\":\"http://odata.org/test/Blob\"}";
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: "MySet", derivedEntityTypeFullName: null, nestedItemToWrite: new[] { entry }, expectedPayload: expectedPayload, writingResponse: true);
        }

        [Fact]
        public void ShouldWriteStreamPropertyForResponseEntryPayloadWithUserModel()
        {
            var entry = new ODataResource { TypeName = "NS.MyDerivedEntityType", Properties = new[] { new ODataProperty { Name = "StreamProperty", Value = new ODataStreamReferenceValue { ReadLink = new Uri("http://odata.org/test/Blob") } } } };
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"StreamProperty@odata.mediaReadLink\":\"http://odata.org/test/Blob\"}";
            this.WriteNestedItemsAndValidatePayload(this.entitySet, entityType: null, nestedItemToWrite: new[] { entry }, expectedPayload: expectedPayload, writingResponse: true);
        }

        [Fact]
        public void ShouldWriteStreamPropertyForRequestEntryPayloadWithUserModel()
        {
            var entry = new ODataResource { TypeName = "NS.MyDerivedEntityType", Properties = new[] { new ODataProperty { Name = "StreamProperty", Value = new ODataStreamReferenceValue { ReadLink = new Uri("http://odata.org/test/Blob") } } } };
            Action action = () => this.WriteNestedItemsAndValidatePayload(entitySet: null, entityType: null, nestedItemToWrite: new[] { entry }, expectedPayload: "", writingResponse: false);
            action.ShouldThrow<ODataException>().WithMessage(Strings.WriterValidationUtils_StreamPropertyInRequest("StreamProperty"));
        }

        [Fact]
        public void ShouldWriteStreamPropertyForRequestEntryPayloadWithoutUserModel()
        {
            var entry = new ODataResource { TypeName = "NS.MyDerivedEntityType", Properties = new[] { new ODataProperty { Name = "StreamProperty", Value = new ODataStreamReferenceValue { ReadLink = new Uri("http://odata.org/test/Blob") } } } };
            Action action = () => this.WriteNestedItemsAndValidatePayload(entitySetFullName: null, derivedEntityTypeFullName: null, nestedItemToWrite: new[] { entry }, expectedPayload: "", writingResponse: false);
            action.ShouldThrow<ODataException>().WithMessage(Strings.WriterValidationUtils_StreamPropertyInRequest("StreamProperty"));
        }
        #endregion Stream Property tests

        #region Null Primitive Property Value tests
        [Fact]
        public void ShouldWriteNullPropertyValueForResponseEntryPayloadWithoutUserModel()
        {
            var entry = new ODataResource { TypeName = "NS.MyDerivedEntityType", Properties = new[] { new ODataProperty { Name = "StringProperty", Value = new ODataNullValue() } } };
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"StringProperty\":null}";
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: "MySet", derivedEntityTypeFullName: null, nestedItemToWrite: new[] { entry }, expectedPayload: expectedPayload, writingResponse: true);
        }

        [Fact]
        public void ShouldWriteNullPropertyValueForResponseEntryPayloadWithUserModel()
        {
            var entry = new ODataResource { TypeName = "NS.MyDerivedEntityType", Properties = new[] { new ODataProperty { Name = "StringProperty", Value = new ODataNullValue() } } };
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"StringProperty\":null}";
            this.WriteNestedItemsAndValidatePayload(entitySet: this.entitySet, entityType: null, nestedItemToWrite: new[] { entry }, expectedPayload: expectedPayload, writingResponse: true);
        }

        [Fact]
        public void ShouldWriteNullPropertyValueForRequestEntryPayloadWithoutUserModel()
        {
            var entry = new ODataResource { TypeName = "NS.MyDerivedEntityType", Properties = new[] { new ODataProperty { Name = "StringProperty", Value = new ODataNullValue() } } };
            const string expectedPayload = "{\"@odata.type\":\"#NS.MyDerivedEntityType\",\"StringProperty\":null}";
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: null, derivedEntityTypeFullName: null, nestedItemToWrite: new[] { entry }, expectedPayload: expectedPayload, writingResponse: false);
        }

        [Fact]
        public void ShouldWriteNullPropertyValueForRequestEntryPayloadWithUserModel()
        {
            var entry = new ODataResource { TypeName = "NS.MyDerivedEntityType", Properties = new[] { new ODataProperty { Name = "StringProperty", Value = new ODataNullValue() } } };
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#NS.MyDerivedEntityType\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"StringProperty\":null}";
            this.WriteNestedItemsAndValidatePayload(entitySet: null, entityType: null, nestedItemToWrite: new[] { entry }, expectedPayload: expectedPayload, writingResponse: false);
        }
        #endregion Null Primitive Property Value tests

        #region Open Primitive Property tests
        [Fact]
        public void ShouldWriteOpenPrimitivePropertyWithTypeNameForResponseEntryPayloadWithoutUserModel()
        {
            var property = new ODataProperty { Name = "OpenGuidProperty", Value = new ODataPrimitiveValue(Guid.Empty) };
            property.SetSerializationInfo(new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Open });
            var entry = new ODataResource { TypeName = "NS.MyDerivedEntityType", Properties = new[] { property } };
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"OpenGuidProperty@odata.type\":\"#Guid\",\"OpenGuidProperty\":\"00000000-0000-0000-0000-000000000000\"}";
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: "MySet", derivedEntityTypeFullName: null, nestedItemToWrite: new[] { entry }, expectedPayload: expectedPayload, writingResponse: true);
        }

        [Fact]
        public void ShouldWriteOpenPrimitivePropertyWithTypeNameForResponseEntryPayloadWithUserModel()
        {
            var entry = new ODataResource { TypeName = "NS.MyDerivedEntityType", Properties = new[] { new ODataProperty { Name = "OpenGuidProperty", Value = new ODataPrimitiveValue(Guid.Empty) } } };
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"OpenGuidProperty@odata.type\":\"#Guid\",\"OpenGuidProperty\":\"00000000-0000-0000-0000-000000000000\"}";
            this.WriteNestedItemsAndValidatePayload(entitySet: this.entitySet, entityType: null, nestedItemToWrite: new[] { entry }, expectedPayload: expectedPayload, writingResponse: true);
        }

        [Fact]
        public void ShouldWriteOpenPrimitivePropertyWithTypeNameForRequestEntryPayloadWithoutUserModel()
        {
            var property = new ODataProperty { Name = "OpenGuidProperty", Value = new ODataPrimitiveValue(Guid.Empty) };
            property.SetSerializationInfo(new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Open });
            var entry = new ODataResource { TypeName = "NS.MyDerivedEntityType", Properties = new[] { property } };
            const string expectedPayload = "{\"@odata.type\":\"#NS.MyDerivedEntityType\",\"OpenGuidProperty@odata.type\":\"#Guid\",\"OpenGuidProperty\":\"00000000-0000-0000-0000-000000000000\"}";
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: "MySet", derivedEntityTypeFullName: null, nestedItemToWrite: new[] { entry }, expectedPayload: expectedPayload, writingResponse: false, setMetadataDocumentUri: false);
        }

        [Fact]
        public void ShouldWriteOpenPrimitivePropertyWithTypeNameForRequestEntryPayloadWithUserModel()
        {
            var entry = new ODataResource { TypeName = "NS.MyDerivedEntityType", Properties = new[] { new ODataProperty { Name = "OpenGuidProperty", Value = new ODataPrimitiveValue(Guid.Empty) } } };
            const string expectedPayload = "{\"@odata.type\":\"#NS.MyDerivedEntityType\",\"OpenGuidProperty@odata.type\":\"#Guid\",\"OpenGuidProperty\":\"00000000-0000-0000-0000-000000000000\"}";
            this.WriteNestedItemsAndValidatePayload(entitySet: this.entitySet, entityType: null, nestedItemToWrite: new[] { entry }, expectedPayload: expectedPayload, writingResponse: false, setMetadataDocumentUri: false);
        }

        [Fact]
        public void ShouldWriteOpenPrimitivePropertyWithoutTypeNameForResponseEntryPayloadWithoutUserModel()
        {
            var property = new ODataProperty { Name = "OpenStringProperty", Value = new ODataPrimitiveValue(String.Empty + "K\uFFFF") };
            property.SetSerializationInfo(new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Open });
            var entry = new ODataResource { TypeName = "NS.MyDerivedEntityType", Properties = new[] { property } };
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/NS.MyDerivedEntityType/$entity\",\"OpenStringProperty\":\"K\\uffff\"}";
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: "MySet", derivedEntityTypeFullName: "NS.MyDerivedEntityType", nestedItemToWrite: new[] { entry }, expectedPayload: expectedPayload, writingResponse: true);
        }

        [Fact]
        public void ShouldWriteOpenPrimitivePropertyWithoutTypeNameForResponseEntryPayloadWithUserModel()
        {
            var entry = new ODataResource { TypeName = "NS.MyDerivedEntityType", Properties = new[] { new ODataProperty { Name = "OpenStringProperty", Value = new ODataPrimitiveValue(String.Empty) } } };
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/NS.MyDerivedEntityType/$entity\",\"OpenStringProperty\":\"\"}";
            this.WriteNestedItemsAndValidatePayload(entitySet: this.entitySet, entityType: this.derivedEntityType, nestedItemToWrite: new[] { entry }, expectedPayload: expectedPayload, writingResponse: true);
        }

        [Fact]
        public void ShouldWriteOpenPrimitivePropertyWithoutTypeNameForRequestEntryPayloadWithoutUserModel()
        {
            var property = new ODataProperty { Name = "OpenStringProperty", Value = new ODataPrimitiveValue(String.Empty) };
            property.SetSerializationInfo(new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Open });
            var entry = new ODataResource { TypeName = "NS.MyDerivedEntityType", Properties = new[] { property } };
            const string expectedPayload = "{\"OpenStringProperty\":\"\"}";
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: "MySet", derivedEntityTypeFullName: "NS.MyDerivedEntityType", nestedItemToWrite: new[] { entry }, expectedPayload: expectedPayload, writingResponse: false, setMetadataDocumentUri: false);
        }

        [Fact]
        public void ShouldWriteOpenPrimitivePropertyWithoutTypeNameForRequestEntryPayloadWithUserModel()
        {
            var entry = new ODataResource { TypeName = "NS.MyDerivedEntityType", Properties = new[] { new ODataProperty { Name = "OpenStringProperty", Value = new ODataPrimitiveValue(String.Empty) } } };
            const string expectedPayload = "{\"OpenStringProperty\":\"\"}";
            this.WriteNestedItemsAndValidatePayload(entitySet: this.entitySet, entityType: this.derivedEntityType, nestedItemToWrite: new[] { entry }, expectedPayload: expectedPayload, writingResponse: false, setMetadataDocumentUri: false);
        }
        #endregion Open Primitive Property tests

        #region Complex Property tests
        [Fact]
        public void ShouldWriteComplexPropertyForResponseEntryPayloadWithoutUserModel()
        {
            var items = CreateResourceWithComplexProperties();
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"ComplexProperty\":{\"ComplexProperty\":{}}}";
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: "MySet", derivedEntityTypeFullName: null, nestedItemToWrite: items, expectedPayload: expectedPayload, writingResponse: true);
        }

        [Fact]
        public void ShouldWriteComplexPropertyForResponseEntryPayloadWithUserModel()
        {
            var items = CreateResourceWithComplexProperties();
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"ComplexProperty\":{\"ComplexProperty\":{}}}";
            this.WriteNestedItemsAndValidatePayload(entitySet: this.entitySet, entityType: null, nestedItemToWrite: items, expectedPayload: expectedPayload, writingResponse: true);
        }

        [Fact]
        public void ShouldWriteComplexPropertyForRequestEntryPayloadWithoutUserModel()
        {
            var items = CreateResourceWithComplexProperties();
            const string expectedPayload = "{\"@odata.type\":\"#NS.MyDerivedEntityType\",\"ComplexProperty\":{\"ComplexProperty\":{}}}";
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: null, derivedEntityTypeFullName: null, nestedItemToWrite: items, expectedPayload: expectedPayload, writingResponse: false);
        }

        [Fact]
        public void ShouldWriteComplexPropertyForRequestEntryPayloadWithUserModel()
        {
            var items = CreateResourceWithComplexProperties();
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#NS.MyDerivedEntityType\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"ComplexProperty\":{\"ComplexProperty\":{}}}";
            this.WriteNestedItemsAndValidatePayload(entitySet: null, entityType: null, nestedItemToWrite: items, expectedPayload: expectedPayload, writingResponse: false);
        }

        #endregion Complex Property tests

        #region Complex Property Inheritance tests
        [Fact]
        public void ShouldWriteComplexPropertyInheritForResponseEntryPayloadWithoutUserModel()
        {
            var items = CreateResourceWithComplexProperties(true, false);
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"ComplexProperty\":{\"DerivedStringProperty\":\"deriveString\",\"ComplexProperty\":{}}}";
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: "MySet", derivedEntityTypeFullName: null, nestedItemToWrite: items, expectedPayload: expectedPayload, writingResponse: true);
        }

        [Fact]
        public void ShouldWriteComplexPropertyInheritForResponseEntryPayloadWithUserModel()
        {
            var items = CreateResourceWithComplexProperties(true, true);
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"ComplexProperty\":{\"@odata.type\":\"#NS.MyDerivedComplexType\",\"DerivedStringProperty\":\"deriveString\",\"ComplexProperty\":{}}}";
            this.WriteNestedItemsAndValidatePayload(entitySet: this.entitySet, entityType: null, nestedItemToWrite: items, expectedPayload: expectedPayload, writingResponse: true);
        }

        [Fact]
        public void ShouldWriteComplexPropertyInheritForRequestEntryPayloadWithoutUserModel()
        {
            var items = CreateResourceWithComplexProperties(true, true);
            const string expectedPayload = "{\"@odata.type\":\"#NS.MyDerivedEntityType\",\"ComplexProperty\":{\"@odata.type\":\"#NS.MyDerivedComplexType\",\"DerivedStringProperty\":\"deriveString\",\"ComplexProperty\":{}}}";
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: null, derivedEntityTypeFullName: null, nestedItemToWrite: items, expectedPayload: expectedPayload, writingResponse: false);
        }

        [Fact]
        public void ShouldWriteComplexPropertyInheritForRequestEntryPayloadWithUserModel()
        {
            var items = CreateResourceWithComplexProperties(true, true);
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#NS.MyDerivedEntityType\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"ComplexProperty\":{\"@odata.type\":\"#NS.MyDerivedComplexType\",\"DerivedStringProperty\":\"deriveString\",\"ComplexProperty\":{}}}";
            this.WriteNestedItemsAndValidatePayload(entitySet: null, entityType: null, nestedItemToWrite: items, expectedPayload: expectedPayload, writingResponse: false);
        }
        #endregion Complex Property Inheritance tests

        private ODataItem[] CreateResourceWithComplexProperties(bool inherit = false, bool withTypeName = false)
        {
            var nestedResourceInfo = new ODataNestedResourceInfo() { Name = "ComplexProperty", IsCollection = false };
            var nestedResource = new ODataResource();
            if (inherit)
            {
                if (withTypeName)
                {
                    nestedResource.TypeName = "NS.MyDerivedComplexType";
                }

                nestedResource.Properties = new ODataProperty[]
                {
                    new ODataProperty { Name = "DerivedStringProperty", Value = "deriveString" }
                };
            }

            var nestedResourceInfo1 = new ODataNestedResourceInfo() { Name = "ComplexProperty", IsCollection = false };
            var nestedResource1 = new ODataResource();
            var resource = new ODataResource { TypeName = "NS.MyDerivedEntityType" };

            return new ODataItem[] { resource, nestedResourceInfo, nestedResource, nestedResourceInfo1, nestedResource1 };
        }

        #region Open Complex Property tests
        [Fact]
        public void ShouldWriteOpenComplexPropertyWithoutTypeNameForResponseEntryPayloadWithoutUserModel()
        {
            var items = CreateEntityWithOpenComplexProperties(false);
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"OpenComplexProperty\":{\"ComplexProperty\":{}}}";
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: "MySet", derivedEntityTypeFullName: null, nestedItemToWrite: items, expectedPayload: expectedPayload, writingResponse: true);
        }

        [Fact]
        public void ShouldWriteOpenComplexPropertyForResponseEntryPayloadWithoutUserModel()
        {
            var items = CreateEntityWithOpenComplexProperties(true);
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"OpenComplexProperty\":{\"@odata.type\":\"#NS.MyComplexType\",\"ComplexProperty\":{}}}";
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: "MySet", derivedEntityTypeFullName: null, nestedItemToWrite: items, expectedPayload: expectedPayload, writingResponse: true);
        }

        [Fact]
        public void ShouldWriteOpenComplexPropertyForResponseEntryPayloadWithUserModel()
        {
            var items = CreateEntityWithOpenComplexProperties(true);
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"OpenComplexProperty\":{\"@odata.type\":\"#NS.MyComplexType\",\"ComplexProperty\":{}}}";
            this.WriteNestedItemsAndValidatePayload(entitySet: this.entitySet, entityType: null, nestedItemToWrite: items, expectedPayload: expectedPayload, writingResponse: true);
        }

        [Fact]
        public void ShouldWriteOpenComplexPropertyWithoutTypeNameForRequestEntryPayloadWithoutUserModel()
        {
            var items = CreateEntityWithOpenComplexProperties(false);
            const string expectedPayload = "{\"@odata.type\":\"#NS.MyDerivedEntityType\",\"OpenComplexProperty\":{\"ComplexProperty\":{}}}";
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: null, derivedEntityTypeFullName: null, nestedItemToWrite: items, expectedPayload: expectedPayload, writingResponse: false);
        }

        [Fact]
        public void ShouldWriteOpenComplexPropertyForRequestEntryPayloadWithoutUserModel()
        {
            var items = CreateEntityWithOpenComplexProperties(true);
            const string expectedPayload = "{\"@odata.type\":\"#NS.MyDerivedEntityType\",\"OpenComplexProperty\":{\"@odata.type\":\"#NS.MyComplexType\",\"ComplexProperty\":{}}}";
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: null, derivedEntityTypeFullName: null, nestedItemToWrite: items, expectedPayload: expectedPayload, writingResponse: false);
        }

        [Fact]
        public void ShouldWriteOpenComplexPropertyForRequestEntryPayloadWithUserModel()
        {
            var items = CreateEntityWithOpenComplexProperties(true);
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#NS.MyDerivedEntityType\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"OpenComplexProperty\":{\"@odata.type\":\"#NS.MyComplexType\",\"ComplexProperty\":{}}}";
            this.WriteNestedItemsAndValidatePayload(entitySet: null, entityType: null, nestedItemToWrite: items, expectedPayload: expectedPayload, writingResponse: false);
        }

        private ODataItem[] CreateEntityWithOpenComplexProperties(bool withTypeName)
        {
            var nestedResourceInfo = new ODataNestedResourceInfo() { Name = "OpenComplexProperty", IsCollection = false };
            var nestedResource = new ODataResource();
            var nestedResourceInfo1 = new ODataNestedResourceInfo() { Name = "ComplexProperty", IsCollection = false };
            var nestedResource1 = new ODataResource();
            var resource = new ODataResource { TypeName = "NS.MyDerivedEntityType" };

            if (withTypeName)
            {
                nestedResource.TypeName = "NS.MyComplexType";
            }
            return new ODataItem[] { resource, nestedResourceInfo, nestedResource, nestedResourceInfo1, nestedResource1 };
        }

        #endregion Open Complex Property tests

        #region Primitive Collection property tests
        [Fact]
        public void ShouldWritePrimitiveCollectionPropertyForResponseEntryPayloadWithoutUserModel()
        {
            var entry = new ODataResource { TypeName = "NS.MyDerivedEntityType", Properties = new[] { new ODataProperty { Name = "PrimitiveCollectionProperty", Value = new ODataCollectionValue { Items = new[] { "string1", "string2" } } } } };
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"PrimitiveCollectionProperty\":[\"string1\",\"string2\"]}";
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: "MySet", derivedEntityTypeFullName: null, nestedItemToWrite: new[] { entry }, expectedPayload: expectedPayload, writingResponse: true);
        }

        [Fact]
        public void ShouldWritePrimitiveCollectionPropertyForResponseEntryPayloadWithUserModel()
        {
            var entry = new ODataResource { TypeName = "NS.MyDerivedEntityType", Properties = new[] { new ODataProperty { Name = "PrimitiveCollectionProperty", Value = new ODataCollectionValue { Items = new[] { "string1", "string2" } } } } };
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"PrimitiveCollectionProperty\":[\"string1\",\"string2\"]}";
            this.WriteNestedItemsAndValidatePayload(entitySet: this.entitySet, entityType: null, nestedItemToWrite: new[] { entry }, expectedPayload: expectedPayload, writingResponse: true);
        }

        [Fact]
        public void ShouldWritePrimitiveCollectionPropertyForRequestEntryPayloadWithoutUserModel()
        {
            var entry = new ODataResource { TypeName = "NS.MyDerivedEntityType", Properties = new[] { new ODataProperty { Name = "PrimitiveCollectionProperty", Value = new ODataCollectionValue { Items = new[] { "string1", "string2" } } } } };
            const string expectedPayload = "{\"@odata.type\":\"#NS.MyDerivedEntityType\",\"PrimitiveCollectionProperty\":[\"string1\",\"string2\"]}";
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: null, derivedEntityTypeFullName: null, nestedItemToWrite: new[] { entry }, expectedPayload: expectedPayload, writingResponse: false);
        }

        [Fact]
        public void ShouldWritePrimitiveCollectionPropertyForRequestEntryPayloadWithUserModel()
        {
            var entry = new ODataResource { TypeName = "NS.MyDerivedEntityType", Properties = new[] { new ODataProperty { Name = "PrimitiveCollectionProperty", Value = new ODataCollectionValue { Items = new[] { "string1", "string2" } } } } };
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#NS.MyDerivedEntityType\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"PrimitiveCollectionProperty\":[\"string1\",\"string2\"]}";
            this.WriteNestedItemsAndValidatePayload(entitySet: null, entityType: null, nestedItemToWrite: new[] { entry }, expectedPayload: expectedPayload, writingResponse: false);
        }

        [Fact]
        public void ShouldWriteNullablePrimitiveCollectionPropertyForResponseEntryPayloadWithUserModel()
        {
            var entry = new ODataResource
            {
                TypeName = "NS.MyDerivedEntityType",
                Properties = new[]
                {
                    new ODataProperty
                    {
                        Name = "NullablePrimitiveCollectionProperty",
                        Value = new ODataCollectionValue { Items = new[] { null, "string2" } }
                    }
                }
            };
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"NullablePrimitiveCollectionProperty\":[null,\"string2\"]}";
            this.WriteNestedItemsAndValidatePayload(entitySet: this.entitySet, entityType: null, nestedItemToWrite: new[] { entry }, expectedPayload: expectedPayload, writingResponse: true);
        }

        [Fact]
        public void ShouldWriteNullableIntCollectionPropertyForResponseEntryPayloadWithUserModel()
        {
            var entry = new ODataResource
            {
                TypeName = "NS.MyDerivedEntityType",
                Properties = new[]
                {
                    new ODataProperty
                    {
                        Name = "NullableIntCollectionProperty",
                        Value = new ODataCollectionValue { Items = new object[] { null, 1 } }
                    }
                }
            };
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"NullableIntCollectionProperty\":[null,1]}";
            this.WriteNestedItemsAndValidatePayload(entitySet: this.entitySet, entityType: null, nestedItemToWrite: new[] { entry }, expectedPayload: expectedPayload, writingResponse: true);
        }
        #endregion Primitive Collection property tests

        #region Open Primitive Collection property tests
        [Fact]
        public void ShouldWriteOpenPrimitiveCollectionPropertyForResponseEntryPayloadWithoutUserModel()
        {
            var property = new ODataProperty { Name = "OpenPrimitiveCollectionProperty", Value = new ODataCollectionValue { Items = new[] { "string1", "string2" } } };
            property.SetSerializationInfo(new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Open });
            var entry = new ODataResource { TypeName = "NS.MyDerivedEntityType", Properties = new[] { property } };
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"OpenPrimitiveCollectionProperty\":[\"string1\",\"string2\"]}";
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: "MySet", derivedEntityTypeFullName: null, nestedItemToWrite: new[] { entry }, expectedPayload: expectedPayload, writingResponse: true);
        }

        [Fact]
        public void ShouldWriteOpenPrimitiveCollectionPropertyForResponseEntryPayloadWithUserModel()
        {
            var primitiveCollectionValue = new ODataCollectionValue { TypeName = "Collection(String)", Items = new[] { "string1", "string2" } };
            var entry = new ODataResource { TypeName = "NS.MyDerivedEntityType", Properties = new[] { new ODataProperty { Name = "OpenPrimitiveCollectionProperty", Value = primitiveCollectionValue } } };
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"OpenPrimitiveCollectionProperty@odata.type\":\"#Collection(String)\",\"OpenPrimitiveCollectionProperty\":[\"string1\",\"string2\"]}";
            this.WriteNestedItemsAndValidatePayload(entitySet: this.entitySet, entityType: null, nestedItemToWrite: new[] { entry }, expectedPayload: expectedPayload, writingResponse: true);
        }

        [Fact]
        public void ShouldWriteOpenPrimitiveCollectionPropertyForRequestEntryPayloadWithoutUserModel()
        {
            var property = new ODataProperty { Name = "OpenPrimitiveCollectionProperty", Value = new ODataCollectionValue { Items = new[] { "string1", "string2" } } };
            property.SetSerializationInfo(new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Open });
            var entry = new ODataResource { TypeName = "NS.MyDerivedEntityType", Properties = new[] { property } };
            const string expectedPayload = "{\"@odata.type\":\"#NS.MyDerivedEntityType\",\"OpenPrimitiveCollectionProperty\":[\"string1\",\"string2\"]}";
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: null, derivedEntityTypeFullName: null, nestedItemToWrite: new[] { entry }, expectedPayload: expectedPayload, writingResponse: false);
        }

        [Fact]
        public void ShouldWriteOpenPrimitiveCollectionPropertyForRequestEntryPayloadWithUserModel()
        {
            var primitiveCollectionValue = new ODataCollectionValue { TypeName = "Collection(String)", Items = new[] { "string1", "string2" } };
            var entry = new ODataResource { TypeName = "NS.MyDerivedEntityType", Properties = new[] { new ODataProperty { Name = "OpenPrimitiveCollectionProperty", Value = primitiveCollectionValue } } };
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#NS.MyDerivedEntityType\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"OpenPrimitiveCollectionProperty@odata.type\":\"#Collection(String)\",\"OpenPrimitiveCollectionProperty\":[\"string1\",\"string2\"]}";
            this.WriteNestedItemsAndValidatePayload(entitySet: null, entityType: null, nestedItemToWrite: new[] { entry }, expectedPayload: expectedPayload, writingResponse: false);
        }
        #endregion Open Primitive Collection property tests

        #region Complex Collection property tests
        [Fact]
        public void ShouldWriteComplexCollectionPropertyForResponseEntryPayloadWithoutUserModel()
        {
            var items = CreateResources(true, false);
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"ComplexCollectionProperty\":[{\"ComplexProperty\":{}}]}";
            this.WriteNestedItemsAndValidatePayload(
                entitySetFullName: "MySet",
                derivedEntityTypeFullName: null,
                nestedItemToWrite: items,
                expectedPayload: expectedPayload,
                writingResponse: true);
        }

        [Fact]
        public void ShouldWriteComplexCollectionPropertyForResponseEntryPayloadWithUserModel()
        {
            Action<ODataWriter> write;
            ODataResource resource = CreateResources(out write);

            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"ComplexCollectionProperty\":[{\"@odata.type\":\"#NS.MyDerivedComplexType\",\"DerivedStringProperty\":\"deriveString\",\"ComplexProperty\":{}},{\"ComplexProperty\":{}}]}";
            this.WriteNestedItemsAndValidatePayload(entitySet: this.entitySet, entityType: null, topLevelItem: resource, writeItems: write, expectedPayload: expectedPayload, writingResponse: true);
        }

        [Fact]
        public void ShouldWriteComplexCollectionPropertyForRequestEntryPayloadWithoutUserModel()
        {
            var items = CreateResources(true, false);
            const string expectedPayload = "{\"@odata.type\":\"#NS.MyDerivedEntityType\",\"ComplexCollectionProperty\":[{\"ComplexProperty\":{}}]}";
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: null, derivedEntityTypeFullName: null, nestedItemToWrite: items, expectedPayload: expectedPayload, writingResponse: false);
        }

        [Fact]
        public void ShouldWriteComplexCollectionPropertyForRequestEntryPayloadWithUserModel()
        {
            Action<ODataWriter> write;
            ODataResource resource = CreateResources(out write);

            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#NS.MyDerivedEntityType\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"ComplexCollectionProperty\":[{\"@odata.type\":\"#NS.MyDerivedComplexType\",\"DerivedStringProperty\":\"deriveString\",\"ComplexProperty\":{}},{\"ComplexProperty\":{}}]}";
            this.WriteNestedItemsAndValidatePayload(entitySet: null, entityType: null, topLevelItem: resource, writeItems:write, expectedPayload: expectedPayload, writingResponse: false);
        }

        private  ODataResource CreateResources(out Action<ODataWriter> write)
        {
            ODataResource topLeveResource = new ODataResource { TypeName = "NS.MyDerivedEntityType" };
            var nestedResourceInfo1 = new ODataNestedResourceInfo() { Name = "ComplexCollectionProperty", IsCollection = true };
            var nestedResourceSet = new ODataResourceSet();
            var nestedResource1 = new ODataResource()
            {
                TypeName = "NS.MyDerivedComplexType",
                Properties = new[] { new ODataProperty { Name = "DerivedStringProperty", Value = "deriveString" } }
            };

            var nestedResourceInfo11 = new ODataNestedResourceInfo() { Name = "ComplexProperty", IsCollection = false };
            var nestedResource11 = new ODataResource();

            var nestedResource2 = new ODataResource();
            var nestedResourceInfo21 = new ODataNestedResourceInfo() { Name = "ComplexProperty", IsCollection = false };
            var nestedResource21 = new ODataResource();

            write = (writer) =>
            {
                writer.WriteStart(topLeveResource);
                writer.WriteStart(nestedResourceInfo1);
                writer.WriteStart(nestedResourceSet);

                writer.WriteStart(nestedResource1);
                writer.WriteStart(nestedResourceInfo11);
                writer.WriteStart(nestedResource11);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteStart(nestedResource2);
                writer.WriteStart(nestedResourceInfo21);
                writer.WriteStart(nestedResource21);
                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();

                writer.WriteEnd();
                writer.WriteEnd();
                writer.WriteEnd();
            };

            return topLeveResource;

        }
        #endregion Complex Collection property tests

        #region Open Complex Collection property tests

        [Fact]
        public void ShouldWriteOpenComplexCollectionPropertyForResponseEntryPayloadWithoutUserModel()
        {
            var items = CreateResources(true, true);
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"OpenComplexCollectionProperty@odata.type\":\"#Collection(NS.MyComplexType)\",\"OpenComplexCollectionProperty\":[{\"ComplexProperty\":{}}]}";
            this.WriteNestedItemsAndValidatePayload(
                entitySetFullName: "MySet",
                derivedEntityTypeFullName: null,
                nestedItemToWrite: items,
                expectedPayload: expectedPayload,
                writingResponse: true);
        }

        [Fact]
        public void ShouldWriteOpenComplexCollectionPropertyForResponseEntryPayloadWithUserModel()
        {
            var items = CreateResources(false, true);
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"OpenComplexCollectionProperty@odata.type\":\"#Collection(NS.MyComplexType)\",\"OpenComplexCollectionProperty\":[{\"ComplexProperty\":{}}]}";
            this.WriteNestedItemsAndValidatePayload(
                entitySet: this.entitySet,
                entityType: null,
                nestedItemToWrite: items,
                expectedPayload: expectedPayload,
                writingResponse: true);
        }

        [Fact]
        public void ShouldWriteOpenComplexCollectionPropertyForRequestEntryPayloadWithoutUserModel()
        {
            var items = CreateResources(true, true);
            const string expectedPayload = "{\"@odata.type\":\"#NS.MyDerivedEntityType\",\"OpenComplexCollectionProperty@odata.type\":\"#Collection(NS.MyComplexType)\",\"OpenComplexCollectionProperty\":[{\"ComplexProperty\":{}}]}";
            this.WriteNestedItemsAndValidatePayload(
                entitySetFullName: null,
                derivedEntityTypeFullName: null,
                nestedItemToWrite: items,
                expectedPayload: expectedPayload,
                writingResponse: false);
        }

        [Fact]
        public void ShouldWriteOpenComplexCollectionPropertyForRequestEntryPayloadWithUserModel()
        {
            var items = CreateResources(false, true);
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#NS.MyDerivedEntityType\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"OpenComplexCollectionProperty@odata.type\":\"#Collection(NS.MyComplexType)\",\"OpenComplexCollectionProperty\":[{\"ComplexProperty\":{}}]}";
            this.WriteNestedItemsAndValidatePayload(
                entitySet: null,
                entityType: null,
                nestedItemToWrite: items,
                expectedPayload: expectedPayload,
                writingResponse: false);
        }

        public ODataItem[] CreateResources(bool withoutModel, bool forOpen)
        {
            ODataResource resource = new ODataResource { TypeName = "NS.MyDerivedEntityType" };
            var nestedResourceInfo1 = new ODataNestedResourceInfo() { Name = "ComplexCollectionProperty", IsCollection = true };
            var nestedResourceSet = new ODataResourceSet();
            var nestedResource1 = new ODataResource();
            var nestedResourceInfo2 = new ODataNestedResourceInfo() { Name = "ComplexProperty", IsCollection = false };
            var nestedResource2 = new ODataResource();

            if (withoutModel)
            {
                if (forOpen)
                {
                    nestedResourceInfo1.SetSerializationInfo(new ODataNestedResourceInfoSerializationInfo() { IsUndeclared = true });
                    nestedResourceSet.TypeName = "Collection(NS.MyComplexType)";
                    nestedResourceSet.SetSerializationInfo(new ODataResourceSerializationInfo { ExpectedTypeName = "NS.MyComplexType", IsFromCollection = true });
                }
            }
            else
            {
                nestedResourceSet.TypeName = "Collection(NS.MyComplexType)";
            }

            if (forOpen)
            {
                nestedResourceInfo1.Name = "OpenComplexCollectionProperty";
            }

            return new ODataItem[] { resource, nestedResourceInfo1, nestedResourceSet, nestedResource1, nestedResourceInfo2, nestedResource2 };
        }

        #endregion Open Complex Collection property tests

        #region Actions tests
        [Fact]
        public void ShouldWriteActionForResponseEntryPayloadWithoutUserModel()
        {
            ODataResource entry = new ODataResource { TypeName = "NS.MyDerivedEntityType" };
            entry.AddAction(new ODataAction { Metadata = new Uri("#Action1", UriKind.Relative) });
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"#Action1\":{}}";
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: "MySet", derivedEntityTypeFullName: null, nestedItemToWrite: new[] { entry }, expectedPayload: expectedPayload, writingResponse: true);
        }

        [Fact]
        public void ShouldWriteActionForResponseEntryPayloadWithUserModel()
        {
            ODataResource entry = new ODataResource { TypeName = "NS.MyDerivedEntityType" };
            entry.AddAction(new ODataAction { Metadata = new Uri("#Action1", UriKind.Relative) });
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"#Action1\":{}}";
            this.WriteNestedItemsAndValidatePayload(entitySet: this.entitySet, entityType: null, nestedItemToWrite: new[] { entry }, expectedPayload: expectedPayload, writingResponse: true);
        }

        [Fact]
        public void ShouldWriteActionForRequestEntryPayloadWithoutUserModel()
        {
            ODataResource entry = new ODataResource { TypeName = "NS.MyDerivedEntityType" };
            entry.AddAction(new ODataAction { Metadata = new Uri("#Action1", UriKind.Relative) });
            const string expectedPayload = "{\"@odata.type\":\"#NS.MyDerivedEntityType\",\"#Action1\":{}}";
            Action action = () => this.WriteNestedItemsAndValidatePayload(entitySetFullName: null, derivedEntityTypeFullName: null, nestedItemToWrite: new[] { entry }, expectedPayload: expectedPayload, writingResponse: false);
            action.ShouldThrow<ODataException>().WithMessage(Strings.WriterValidationUtils_OperationInRequest("#Action1"));
        }

        [Fact]
        public void ShouldWriteActionForRequestEntryPayloadWithUserModel()
        {
            ODataResource entry = new ODataResource { TypeName = "NS.MyDerivedEntityType" };
            entry.AddAction(new ODataAction { Metadata = new Uri("#Action1", UriKind.Relative) });
            const string expectedPayload = "{\"@odata.type\":\"#NS.MyDerivedEntityType\",\"#Action1\":{}}";
            Action action = () => this.WriteNestedItemsAndValidatePayload(entitySet: null, entityType: null, nestedItemToWrite: new[] { entry }, expectedPayload: expectedPayload, writingResponse: false);
            action.ShouldThrow<ODataException>().WithMessage(Strings.WriterValidationUtils_OperationInRequest("#Action1"));
        }
        #endregion Actions tests

        #region Functions tests
        [Fact]
        public void ShouldWriteFunctionForResponseEntryPayloadWithoutUserModel()
        {
            ODataResource entry = new ODataResource { TypeName = "NS.MyDerivedEntityType" };
            entry.AddFunction(new ODataFunction { Metadata = new Uri("#Function1", UriKind.Relative) });
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"#Function1\":{}}";
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: "MySet", derivedEntityTypeFullName: null, nestedItemToWrite: new[] { entry }, expectedPayload: expectedPayload, writingResponse: true);
        }

        [Fact]
        public void ShouldWriteFunctionForResponseEntryPayloadWithUserModel()
        {
            ODataResource entry = new ODataResource { TypeName = "NS.MyDerivedEntityType" };
            entry.AddFunction(new ODataFunction { Metadata = new Uri("#Function1", UriKind.Relative) });
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"#Function1\":{}}";
            this.WriteNestedItemsAndValidatePayload(entitySet: this.entitySet, entityType: null, nestedItemToWrite: new[] { entry }, expectedPayload: expectedPayload, writingResponse: true);
        }

        [Fact]
        public void ShouldWriteFunctionForRequestEntryPayloadWithoutUserModel()
        {
            ODataResource entry = new ODataResource { TypeName = "NS.MyDerivedEntityType" };
            entry.AddFunction(new ODataFunction {Metadata = new Uri("#Function1", UriKind.Relative)});
            const string expectedPayload = "{\"@odata.type\":\"#NS.MyDerivedEntityType\",\"#Function1\":{}}";
            Action action = () => this.WriteNestedItemsAndValidatePayload(entitySetFullName: null, derivedEntityTypeFullName: null, nestedItemToWrite: new[] { entry }, expectedPayload: expectedPayload, writingResponse: false);
            action.ShouldThrow<ODataException>().WithMessage(Strings.WriterValidationUtils_OperationInRequest("#Function1"));
        }

        [Fact]
        public void ShouldWriteFunctionForRequestEntryPayloadWithUserModel()
        {
            ODataResource entry = new ODataResource { TypeName = "NS.MyDerivedEntityType" };
            entry.AddFunction(new ODataFunction {Metadata = new Uri("#Function1", UriKind.Relative)});
            const string expectedPayload = "{\"@odata.type\":\"#NS.MyDerivedEntityType\",\"#Function1\":{}}";
            Action action = () => this.WriteNestedItemsAndValidatePayload(entitySet: null, entityType: null, nestedItemToWrite: new[] { entry }, expectedPayload: expectedPayload, writingResponse: false);
            action.ShouldThrow<ODataException>().WithMessage(Strings.WriterValidationUtils_OperationInRequest("#Function1"));
        }
        #endregion Functions tests

        #region Expanded Entry tests
        [Fact]
        public void ShouldWriteExpandedEntryForResponseFeedPayloadWithoutUserModel()
        {
            List<ODataItem> itemsToWrite = new List<ODataItem>();
            itemsToWrite.Add(new ODataResourceSet());
            itemsToWrite.Add(new ODataResource());
            itemsToWrite.Add(new ODataNestedResourceInfo { Name = "EntityReferenceProperty", IsCollection = false });
            itemsToWrite.Add(new ODataResource());
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet\",\"value\":[{\"EntityReferenceProperty\":{}}]}";
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: "MySet", derivedEntityTypeFullName: null, nestedItemToWrite: itemsToWrite.ToArray(), expectedPayload: expectedPayload, writingResponse: true);
        }

        [Fact]
        public void ShouldWriteExpandedEntryForResponseFeedPayloadWithUserModel()
        {
            List<ODataItem> itemsToWrite = new List<ODataItem>();
            itemsToWrite.Add(new ODataResourceSet());
            itemsToWrite.Add(new ODataResource());
            itemsToWrite.Add(new ODataNestedResourceInfo { Name = "EntityReferenceProperty", IsCollection = false });
            itemsToWrite.Add(new ODataResource());
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet\",\"value\":[{\"EntityReferenceProperty\":{}}]}";
            this.WriteNestedItemsAndValidatePayload(entitySet: this.entitySet, entityType: null, nestedItemToWrite: itemsToWrite.ToArray(), expectedPayload: expectedPayload, writingResponse: true);
        }

        [Fact]
        public void ShouldWriteExpandedEntryForRequestFeedPayloadWithoutUserModel()
        {
            List<ODataItem> itemsToWrite = new List<ODataItem>();
            itemsToWrite.Add(new ODataResourceSet());
            itemsToWrite.Add(new ODataResource());
            itemsToWrite.Add(new ODataNestedResourceInfo { Name = "EntityReferenceProperty", IsCollection = false });
            itemsToWrite.Add(new ODataResource());
            const string expectedPayload = "{\"value\":[{\"EntityReferenceProperty\":{}}]}";
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: null, derivedEntityTypeFullName: null, nestedItemToWrite: itemsToWrite.ToArray(), expectedPayload: expectedPayload, writingResponse: false);
        }

        [Fact]
        public void ShouldWriteExpandedEntryForRequestFeedPayloadWithUserModel()
        {
            List<ODataItem> itemsToWrite = new List<ODataItem>();
            itemsToWrite.Add(new ODataResourceSet());
            itemsToWrite.Add(new ODataResource());
            itemsToWrite.Add(new ODataNestedResourceInfo { Name = "EntityReferenceProperty", IsCollection = false });
            itemsToWrite.Add(new ODataResource());
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#Collection(NS.MyEntityType)\",\"value\":[{\"EntityReferenceProperty\":{}}]}";
            this.WriteNestedItemsAndValidatePayload(entitySet: null, entityType: this.entityType, nestedItemToWrite: itemsToWrite.ToArray(), expectedPayload: expectedPayload, writingResponse: false);
        }
        #endregion Expanded Entry tests

        #region Expanded Feed tests
        [Fact]
        public void ShouldWriteExpandedFeedForResponseEntryPayloadWithoutUserModel()
        {
            List<ODataItem> itemsToWrite = new List<ODataItem>();
            itemsToWrite.Add(new ODataResource());
            itemsToWrite.Add(new ODataNestedResourceInfo { Name = "EntitySetReferenceProperty", IsCollection = true });
            itemsToWrite.Add(new ODataResourceSet());
            itemsToWrite.Add(new ODataResource());
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"EntitySetReferenceProperty\":[{}]}";
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: "MySet", derivedEntityTypeFullName: null, nestedItemToWrite: itemsToWrite.ToArray(), expectedPayload: expectedPayload, writingResponse: true);
        }

        [Fact]
        public void ShouldWriteExpandedFeedForResponseEntryPayloadWithUserModel()
        {
            List<ODataItem> itemsToWrite = new List<ODataItem>();
            itemsToWrite.Add(new ODataResource());
            itemsToWrite.Add(new ODataNestedResourceInfo { Name = "EntitySetReferenceProperty", IsCollection = true });
            itemsToWrite.Add(new ODataResourceSet());
            itemsToWrite.Add(new ODataResource());
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"EntitySetReferenceProperty\":[{}]}";
            this.WriteNestedItemsAndValidatePayload(entitySet: this.entitySet, entityType: null, nestedItemToWrite: itemsToWrite.ToArray(), expectedPayload: expectedPayload, writingResponse: true);
        }

        [Fact]
        public void ShouldWriteExpandedFeedForRequestEntryPayloadWithoutUserModel()
        {
            List<ODataItem> itemsToWrite = new List<ODataItem>();
            itemsToWrite.Add(new ODataResource());
            itemsToWrite.Add(new ODataNestedResourceInfo { Name = "EntitySetReferenceProperty", IsCollection = true });
            itemsToWrite.Add(new ODataResourceSet());
            itemsToWrite.Add(new ODataResource());
            const string expectedPayload = "{\"EntitySetReferenceProperty\":[{}]}";
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: null, derivedEntityTypeFullName: null, nestedItemToWrite: itemsToWrite.ToArray(), expectedPayload: expectedPayload, writingResponse: false);
        }

        [Fact]
        public void ShouldWriteExpandedFeedForRequestEntryPayloadWithUserModel()
        {
            List<ODataItem> itemsToWrite = new List<ODataItem>();
            itemsToWrite.Add(new ODataResource());
            itemsToWrite.Add(new ODataNestedResourceInfo { Name = "EntitySetReferenceProperty", IsCollection = true });
            itemsToWrite.Add(new ODataResourceSet());
            itemsToWrite.Add(new ODataResource());
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#NS.MyEntityType\",\"EntitySetReferenceProperty\":[{}]}";
            this.WriteNestedItemsAndValidatePayload(entitySet: null, entityType: this.entityType, nestedItemToWrite: itemsToWrite.ToArray(), expectedPayload: expectedPayload, writingResponse: false);
        }

        [Fact]
        public void ShouldWritePayloadWhenFeedAndEntryHasSerializationInfo()
        {
            var feed = new ODataResourceSet();
            feed.SetSerializationInfo(new ODataResourceSerializationInfo { NavigationSourceName = "MySet", NavigationSourceEntityTypeName = "NS.MyEntityType" });
            var entry = new ODataResource();
            entry.SetSerializationInfo(new ODataResourceSerializationInfo { NavigationSourceName = "MySet2", NavigationSourceEntityTypeName = "NS.MyEntityType2" });
            List<ODataItem> itemsToWrite = new List<ODataItem>() { feed, entry };
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet\",\"value\":[{}]}";
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: null, derivedEntityTypeFullName: null, nestedItemToWrite: itemsToWrite.ToArray(), expectedPayload: expectedPayload, writingResponse: true);
        }

        [Fact]
        public void ShouldWritePayloadWhenExpandedFeedAndEntryHasSerializationInfo()
        {
            List<ODataItem> itemsToWrite = new List<ODataItem>();
            var entry1 = new ODataResource();
            entry1.SetSerializationInfo(new ODataResourceSerializationInfo { NavigationSourceName = "MySet", NavigationSourceEntityTypeName = "NS.MyEntityType" });
            itemsToWrite.Add(entry1);
            itemsToWrite.Add(new ODataNestedResourceInfo { Name = "EntitySetReferenceProperty", IsCollection = true });
            var feed = new ODataResourceSet();
            feed.SetSerializationInfo(new ODataResourceSerializationInfo { NavigationSourceName = "MySet", NavigationSourceEntityTypeName = "NS.MyEntityType"});
            itemsToWrite.Add(feed);
            var entry2 = new ODataResource();
            entry2.SetSerializationInfo(new ODataResourceSerializationInfo { NavigationSourceName = "MySet2", NavigationSourceEntityTypeName = "NS.MyEntityType2" });
            itemsToWrite.Add(entry2);
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"EntitySetReferenceProperty\":[{}]}";
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: null, derivedEntityTypeFullName: null, nestedItemToWrite: itemsToWrite.ToArray(), expectedPayload: expectedPayload, writingResponse: true);
        }
        #endregion Expanded Feed tests

        #region Enum tests
        public enum Color
        {
            Red = 1,
            Green = 2,
            Blue = 3
        }

        [Fact]
        public void WriteEnumWithoutModel()
        {
            var entry = new ODataResource
                {
                    TypeName = "NS.MyDerivedEntityType",
                    Properties = new[]
                        {
                            new ODataProperty
                            {
                                Name = "Color",
                                Value = new ODataEnumValue(Color.Green.ToString())
                            }
                        }
                };

            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"Color\":\"Green\"}";
            this.WriteNestedItemsAndValidatePayload(
                entitySetFullName: "MySet",
                derivedEntityTypeFullName: null,
                nestedItemToWrite: new[] { entry },
                expectedPayload: expectedPayload,
                writingResponse: true);
        }

        [Fact]
        public void WriteEnumIntWithoutModel()
        {
            var entry = new ODataResource
            {
                TypeName = "NS.MyDerivedEntityType",
                Properties = new[]
                        {
                            new ODataProperty
                            {
                                Name = "Color",
                                Value = new ODataEnumValue(((int)Color.Green).ToString())
                            }
                        }
            };

            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"Color\":\"2\"}";
            this.WriteNestedItemsAndValidatePayload(
                entitySetFullName: "MySet",
                derivedEntityTypeFullName: null,
                nestedItemToWrite: new[] { entry },
                expectedPayload: expectedPayload,
                writingResponse: true);
        }

        #endregion

        #region Prefixing type tests

        [Fact]
        public void WritePrimitivePropertyWithDurationWithUserModel()
        {
            var property = new ODataProperty { Name = "DurationProperty", Value = new ODataPrimitiveValue(new TimeSpan(1, 1, 1)) };
            var entry = new ODataResource { TypeName = "NS.MyDerivedEntityType", Properties = new[] { property } };
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"DurationProperty@odata.type\":\"#Duration\",\"DurationProperty\":\"PT1H1M1S\"}";
            this.WriteNestedItemsAndValidatePayload(entitySet: this.entitySet, entityType: null, nestedItemToWrite: new[] { entry }, expectedPayload: expectedPayload, writingResponse: true);
        }

        [Fact]
        public void WritePrimitivePropertyWithByteWithoutUserModel()
        {
            var property = new ODataProperty { Name = "ByteProperty", Value = new ODataPrimitiveValue(Byte.MaxValue) };
            property.SetSerializationInfo(new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Open });
            var entry = new ODataResource { TypeName = "NS.MyDerivedEntityType", Properties = new[] { property } };
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\",\"@odata.type\":\"#NS.MyDerivedEntityType\",\"ByteProperty@odata.type\":\"#Byte\",\"ByteProperty\":255}";
            this.WriteNestedItemsAndValidatePayload(entitySetFullName: "MySet", derivedEntityTypeFullName: null, nestedItemToWrite: new[] { entry }, expectedPayload: expectedPayload, writingResponse: true);
        }

        [Fact]
        public void WritePrimitivePropertyWithGeographyWithUserModel()
        {
            var property = new ODataProperty { Name = "GeographyProperty", Value = new ODataPrimitiveValue(GeographyFactory.MultiPoint().Point(1.5, 1.0).Point(2.5, 2.0).Build()) };
            var entry = new ODataResource { TypeName = "NS.MyDerivedEntityType", Properties = new[] { property } };
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\","
                + "\"@odata.type\":\"#NS.MyDerivedEntityType\","
                + "\"GeographyProperty@odata.type\":\"#GeographyMultiPoint\","
                + "\"GeographyProperty\":{\"type\":\"MultiPoint\",\"coordinates\":[[1.0,1.5],[2.0,2.5]],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}}";
            this.WriteNestedItemsAndValidatePayload(entitySet: this.entitySet, entityType: null, nestedItemToWrite: new[] { entry }, expectedPayload: expectedPayload, writingResponse: true);
        }

        [Fact]
        public void WritePrimitivePropertyWithGeometryWithUserModel()
        {
            var property = new ODataProperty { Name = "GeometryProperty", Value = new ODataPrimitiveValue(GeometryFactory.Collection().Point(-19.99, -12.0).Build()) };
            var entry = new ODataResource { TypeName = "NS.MyDerivedEntityType", Properties = new[] { property } };
            const string expectedPayload = "{\"@odata.context\":\"http://odata.org/test/$metadata#MySet/$entity\","
                + "\"@odata.type\":\"#NS.MyDerivedEntityType\","
                + "\"GeometryProperty@odata.type\":\"#GeometryCollection\","
                + "\"GeometryProperty\":{\"type\":\"GeometryCollection\",\"geometries\":[{\"type\":\"Point\",\"coordinates\":[-19.99,-12.0]}],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}}}";
            this.WriteNestedItemsAndValidatePayload(entitySet: this.entitySet, entityType: null, nestedItemToWrite: new[] { entry }, expectedPayload: expectedPayload, writingResponse: true);
        }

        #endregion

        private void WriteNestedItemsAndValidatePayload(IEdmEntitySet entitySet, IEdmEntityType entityType, ODataItem[] nestedItemToWrite, string expectedPayload, bool writingResponse = true, bool setMetadataDocumentUri = true)
        {
            MemoryStream stream = new MemoryStream();
            ODataJsonLightOutputContext outputContext = CreateJsonLightOutputContext(stream, writingResponse, this.userModel, setMetadataDocumentUri ? this.metadataDocumentUri : null);
            ODataJsonLightWriter writer = new ODataJsonLightWriter(outputContext, entitySet, entityType, nestedItemToWrite[0] is ODataResourceSet);
            WriteNestedItems(nestedItemToWrite, writer);
            ValidateWrittenPayload(stream, writer, expectedPayload);
        }

        private void WriteNestedItemsAndValidatePayload(IEdmEntitySet entitySet, IEdmEntityType entityType, ODataItem topLevelItem, Action<ODataWriter> writeItems, string expectedPayload, bool writingResponse = true, bool setMetadataDocumentUri = true)
        {
            MemoryStream stream = new MemoryStream();
            ODataJsonLightOutputContext outputContext = CreateJsonLightOutputContext(stream, writingResponse, this.userModel, setMetadataDocumentUri ? this.metadataDocumentUri : null);
            ODataJsonLightWriter writer = new ODataJsonLightWriter(outputContext, entitySet, entityType, topLevelItem is ODataResourceSet);
            writeItems(writer);
            ValidateWrittenPayload(stream, writer, expectedPayload);
        }

        private void WriteNestedItemsAndValidatePayload(string entitySetFullName, string derivedEntityTypeFullName, ODataItem[] nestedItemToWrite, string expectedPayload, bool writingResponse = true, bool setMetadataDocumentUri = true)
        {
            MemoryStream stream = new MemoryStream();
            ODataJsonLightOutputContext outputContext = CreateJsonLightOutputContext(stream, writingResponse, EdmCoreModel.Instance, setMetadataDocumentUri ? this.metadataDocumentUri : null);

            ODataItem topLevelItem = nestedItemToWrite[0];
            ODataResourceSet topLevelFeed = topLevelItem as ODataResourceSet;

            if (entitySetFullName != null)
            {
                ODataResourceSerializationInfo serializationInfo = entitySetFullName == null ? null : new ODataResourceSerializationInfo { NavigationSourceName = entitySetFullName, NavigationSourceEntityTypeName = "NS.MyEntityType", ExpectedTypeName = derivedEntityTypeFullName ?? "NS.MyEntityType", NavigationSourceKind = EdmNavigationSourceKind.EntitySet };
                if (topLevelFeed != null)
                {
                    topLevelFeed.SetSerializationInfo(serializationInfo);
                }
                else
                {
                    ((ODataResource)topLevelItem).SetSerializationInfo(serializationInfo);
                }
            }

            ODataJsonLightWriter writer = new ODataJsonLightWriter(outputContext, /*entitySet*/ null, /*entityType*/ null, /*writingFeed*/ topLevelFeed != null);
            WriteNestedItems(nestedItemToWrite, writer);
            ValidateWrittenPayload(stream, writer, expectedPayload);
        }

        private void WriteNestedItemsAndValidatePayload(string entitySetFullName, string derivedEntityTypeFullName, ODataItem topLevelItem, Action<ODataWriter> writeItems, string expectedPayload, bool writingResponse = true, bool setMetadataDocumentUri = true)
        {
            MemoryStream stream = new MemoryStream();
            ODataJsonLightOutputContext outputContext = CreateJsonLightOutputContext(stream, writingResponse, EdmCoreModel.Instance, setMetadataDocumentUri ? this.metadataDocumentUri : null);

            ODataResourceSet topLevelFeed = topLevelItem as ODataResourceSet;

            if (entitySetFullName != null)
            {
                ODataResourceSerializationInfo serializationInfo = entitySetFullName == null ? null : new ODataResourceSerializationInfo { NavigationSourceName = entitySetFullName, NavigationSourceEntityTypeName = "NS.MyEntityType", ExpectedTypeName = derivedEntityTypeFullName ?? "NS.MyEntityType" };
                if (topLevelFeed != null)
                {
                    topLevelFeed.SetSerializationInfo(serializationInfo);
                }
                else
                {
                    ((ODataResource)topLevelItem).SetSerializationInfo(serializationInfo);
                }
            }

            ODataJsonLightWriter writer = new ODataJsonLightWriter(outputContext, /*entitySet*/ null, /*entityType*/ null, /*writingFeed*/ topLevelFeed != null);
            writeItems(writer);
            ValidateWrittenPayload(stream, writer, expectedPayload);
        }

        private static void WriteNestedItems(ODataItem[] nestedItemsToWrite, ODataJsonLightWriter writer)
        {
            foreach (ODataItem itemToWrite in nestedItemsToWrite)
            {
                ODataResourceSet feedToWrite = itemToWrite as ODataResourceSet;
                if (feedToWrite != null)
                {
                    writer.WriteStart(feedToWrite);
                }
                else
                {
                    ODataResource entryToWrite = itemToWrite as ODataResource;
                    if (entryToWrite != null)
                    {
                        writer.WriteStart(entryToWrite);
                    }
                    else
                    {
                        writer.WriteStart((ODataNestedResourceInfo)itemToWrite);
                    }
                }
            }

            for (int count = 0; count < nestedItemsToWrite.Length; count++)
            {
                writer.WriteEnd();
            }
        }

        private static void ValidateWrittenPayload(MemoryStream stream, ODataJsonLightWriter writer, string expectedPayload)
        {
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            string payload = (new StreamReader(stream)).ReadToEnd();
            payload.Should().Be(expectedPayload);
        }

        private static ODataJsonLightOutputContext CreateJsonLightOutputContext(MemoryStream stream, bool writingResponse = true, IEdmModel userModel = null, Uri serviceDocumentUri = null)
        {
            var settings = new ODataMessageWriterSettings { Version = ODataVersion.V4 };
            if (serviceDocumentUri != null)
            {
                settings.SetServiceDocumentUri(serviceDocumentUri);
            }

            var messageInfo = new ODataMessageInfo
            {
                MessageStream = new NonDisposingStream(stream),
                MediaType = new ODataMediaType("application", "json"),
                Encoding = Encoding.UTF8,
                IsResponse = writingResponse,
                IsAsync = false,
                Model = userModel ?? EdmCoreModel.Instance
            };

            return new ODataJsonLightOutputContext(messageInfo, settings);
        }
    }
}

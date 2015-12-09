//---------------------------------------------------------------------
// <copyright file="ODataContextUriBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Microsoft.Spatial;
using Xunit;

namespace Microsoft.OData.Core.Tests
{
    public class ODataContextUriBuilderTests
    {
        private const string ServiceDocumentUriString = "http://odata.org/service/";
        private const string MetadataDocumentUriString = "http://odata.org/service/$metadata";
        private static readonly ODataFeedAndEntryTypeContext ResponseTypeContextWithoutTypeInfo = ODataFeedAndEntryTypeContext.Create(serializationInfo: null, navigationSource: null, navigationSourceEntityType: null, expectedEntityType: null, model: EdmCoreModel.Instance, throwIfMissingTypeInfo: true);
        private static readonly ODataFeedAndEntryTypeContext RequestTypeContextWithoutTypeInfo = ODataFeedAndEntryTypeContext.Create(serializationInfo: null, navigationSource: null, navigationSourceEntityType: null, expectedEntityType: null, model: EdmCoreModel.Instance, throwIfMissingTypeInfo: false);

        private Uri metadataDocumentBaseUri;
        private EdmModel edmModel;
        private EdmEntitySet citySet;
        private EdmSingleton singletonCity;
        private EdmEntityType cityType;
        private EdmEntityType capitolCityType;
        private ODataContextUriBuilder responseContextUriBuilder;
        private ODataContextUriBuilder requestContextUriBuilder;
        private ODataContextUriBuilder builderWithNoMetadataDocumentUri;
        private ODataFeedAndEntryTypeContext responseCityTypeContextWithoutSerializationInfo;
        private ODataFeedAndEntryTypeContext responseCapitolCityTypeContextWithoutSerializationInfo;

        public ODataContextUriBuilderTests()
        {
            this.InitializeEdmModel();
            this.InitalizeBuilder();
            this.InitializeTypeContext();
        }

        [Fact]
        public void BuildContextUrlforUnsupportedPayloadKindShouldThrowException()
        {
            Action action = () => this.responseContextUriBuilder.BuildContextUri(ODataPayloadKind.MetadataDocument);
            action.ShouldThrow<ODataException>().WithMessage(Microsoft.OData.Core.Strings.ODataContextUriBuilder_UnsupportedPayloadKind(ODataPayloadKind.MetadataDocument.ToString()));
        }

        [Fact]
        public void WriteServiceDocumentUri()
        {
            this.responseContextUriBuilder.BuildContextUri(ODataPayloadKind.ServiceDocument).OriginalString.Should().Be(BuildExpectedContextUri(""));
        }

        [Fact]
        public void ShouldRequireMetadataDocumentUriInResponses()
        {
            Action action = () => ODataContextUriBuilder.Create(null, true);
            action.ShouldThrow<ODataException>().WithMessage(Microsoft.OData.Core.Strings.ODataOutputContext_MetadataDocumentUriMissing);
        }

        #region context uri with $select and $expand
        [Fact]
        public void FeedContextUriWithNoSelectString()
        {
            this.CreateFeedContextUri(default(string), null).OriginalString.Should().Be(BuildExpectedContextUri("#Cities"));
        }

        [Fact]
        public void FeedContextUriWithEmptySelectString()
        {
            this.CreateFeedContextUri(string.Empty, null).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", false, string.Empty));
        }

        [Fact]
        public void FeedContextUriWithSelect()
        {
            // Select single structure property
            string selectClause = "Name,Name";
            string expectClause = "Name";

            this.CreateFeedContextUri(selectClause, null).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", false, expectClause));

            // Select single navigation property
            selectClause = "Districts, Districts";
            expectClause = "Districts";

            this.CreateFeedContextUri(selectClause, null).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", false, expectClause));
        }

        [Fact]
        public void FeedContextUriWithWildcardSelectString()
        {
            const string selectClause = "Id,Name,*";
            const string expectClause = "*";

            this.CreateFeedContextUri(selectClause, null).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", false, expectClause));
        }

        [Fact]
        public void EntryContextUriWithNoSelectString()
        {
            this.CreateEntryContextUri(default(string), null).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", true));
        }

        [Fact]
        public void EntryContextUriWithEmptySelectString()
        {
            this.CreateEntryContextUri(string.Empty, null).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", true, string.Empty));
        }

        [Fact]
        public void EntryContextUriWithSelectString()
        {
            const string selectClause = "Id,Name,*";
            const string expectClause = "*";
            this.CreateEntryContextUri(selectClause, null).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", true, expectClause));
        }

        [Fact]
        public void FeedContextUriWithSingleExpandString()
        {
            // expand without select, $expand=A
            string selectClause = "";
            string expandClause = "TestModel.CapitolCity/Districts";
            string expectedClause = "";
            this.CreateFeedContextUri(selectClause, expandClause).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", false, expectedClause));

            // expand and select, $expand=A,B
            selectClause = "";
            expandClause = "TestModel.CapitolCity/CapitolDistrict,TestModel.CapitolCity/Districts";
            expectedClause = "";
            this.CreateFeedContextUri(selectClause, expandClause).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", false, expectedClause));
        }

        [Fact]
        public void FeedContextUriWithSelectAndExpandString()
        {
            // $select=A&$expand=B
            string selectClause = "Name";
            string expandClause = "Districts";
            string expectedClause = "Name,Districts";
            this.CreateFeedContextUri(selectClause, expandClause).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", false, expectedClause));

            // $select=A&$expand=A
            selectClause = "Districts";
            expandClause = "Districts";
            expectedClause = "Districts";
            this.CreateFeedContextUri(selectClause, expandClause).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", false, expectedClause));

            // $select=A,B,C&$expand=A
            selectClause = "Name,Districts,Size";
            expandClause = "Districts,Districts";
            expectedClause = "Name,Districts,Size";
            this.CreateFeedContextUri(selectClause, expandClause).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", false, expectedClause));

            // $select=A&$expand=A,B
            selectClause = "Districts";
            expandClause = "Districts,TestModel.CapitolCity/CapitolDistrict";
            expectedClause = "Districts,TestModel.CapitolCity/CapitolDistrict";
            this.CreateFeedContextUri(selectClause, expandClause).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", false, expectedClause));

            // $select=A,B&$expand=B($select=C)
            selectClause = "Name,Districts";
            expandClause = "Districts($select=Name)";
            expectedClause = "Name,Districts,Districts(Name)";
            this.CreateFeedContextUri(selectClause, expandClause).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", false, expectedClause));
        }

        [Fact]
        public void EntryContextUriWithExpandNestedSelectString()
        {
            // With out $select in same level, $expand=A($select=B)
            string selectClause = "";
            string expandClause = "Districts($select=Name,Zip)";
            string expectedClause = "Districts(Name,Zip)";
            this.CreateEntryContextUri(selectClause, expandClause).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", true, expectedClause));

            // With $select in same level, $select=A&$expand=A($select=B,C)
            selectClause = "Districts";
            expandClause = "Districts($select=Name,Zip)";
            expectedClause = "Districts,Districts(Name,Zip)";
            this.CreateEntryContextUri(selectClause, expandClause).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", true, expectedClause));
        }

        [Fact]
        public void EntryContextUriWithExpandNestedExpandString()
        {
            // Without inner $select, $expand=A($expand=B($expand=C))
            string expandClause = "Districts($expand=City($expand=Districts))";
            string expectedClause = "";
            this.CreateEntryContextUri(null, expandClause).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", true, expectedClause));

            // With inner $select, $expand=A($expand=B($select=C))
            expandClause = "Districts($expand=City($select=Districts))";
            expectedClause = "Districts(City(Districts))";
            this.CreateEntryContextUri(null, expandClause).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", true, expectedClause));

            // $expand=A($select=B;$expand=C($select=D)))
            expandClause = "Districts($select=Name;$expand=City($select=Districts))";
            expectedClause = "Districts(Name,City,City(Districts))";
            this.CreateEntryContextUri(null, expandClause).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", true, expectedClause));
        }

        [Fact]
        public void FeedContextUriWithMixedSelectAndExpandString()
        {
            const string selectClause = "Size,Name";
            const string expandClause = "Districts($select=Zip,City;$expand=City($expand=Districts;$select=Name))";
            const string expectedClause = "Size,Name,Districts,Districts(Zip,City,City(Name,Districts))";
            this.CreateFeedContextUri(selectClause, expandClause).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", false, expectedClause));
        }
        #endregion context uri with $select and $expand

        #region feed context uri
        [Fact]
        public void ShouldWriteFeedContextUriWithoutTypecast()
        {
            this.CreateFeedContextUri(this.responseCityTypeContextWithoutSerializationInfo).OriginalString.Should().Be(BuildExpectedContextUri("#Cities"));
        }

        [Fact]
        public void ShouldWriteFeedContextUriWithTypecast()
        {
            this.CreateFeedContextUri(this.responseCapitolCityTypeContextWithoutSerializationInfo).OriginalString.Should().Be(BuildExpectedContextUri("#Cities/TestModel.CapitolCity"));
        }

        [Fact]
        public void ShouldThrowIfEntitySetIsMissingOnFeedResponse()
        {
            Action test = () => this.CreateFeedContextUri(ResponseTypeContextWithoutTypeInfo);
            test.ShouldThrow<ODataException>().WithMessage(Strings.ODataFeedAndEntryTypeContext_MetadataOrSerializationInfoMissing);
        }

        [Fact]
        public void ShouldNotWriteContextUriIfEntitySetIsMissingOnFeedRequest()
        {
            this.CreateFeedContextUri(RequestTypeContextWithoutTypeInfo, false).Should().BeNull();
        }
        #endregion feed context uri

        #region entry context uri
        [Fact]
        public void ShouldWriteEntryContextUriWithoutTypecast()
        {
            this.CreateEntryContextUri(this.responseCityTypeContextWithoutSerializationInfo).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", true));
        }

        [Fact]
        public void ShouldWriteEntryContextUriWithTypecast()
        {
            this.CreateEntryContextUri(this.responseCapitolCityTypeContextWithoutSerializationInfo).OriginalString.Should().Be(BuildExpectedContextUri("#Cities/TestModel.CapitolCity", true));
        }

        [Fact]
        public void ShouldThrowIfEntitySetIsMissingOnEntryResponse()
        {
            Action test = () => this.CreateEntryContextUri(ResponseTypeContextWithoutTypeInfo);
            test.ShouldThrow<ODataException>().WithMessage(Strings.ODataFeedAndEntryTypeContext_MetadataOrSerializationInfoMissing);
        }

        [Fact]
        public void ShouldThrowIfEntitySetIsMissingOnEntryRequest()
        {
            this.CreateEntryContextUri(RequestTypeContextWithoutTypeInfo, false).Should().BeNull();
        }

        [Fact]
        public void ShouldNotIncludeFragmentItemSelectorOnSingleton()
        {
            var singletonTypeContextWithModel = ODataFeedAndEntryTypeContext.Create( /*serializationInfo*/null, this.singletonCity, this.cityType, this.cityType, this.edmModel, throwIfMissingTypeInfo: true);
            this.CreateEntryContextUri(singletonTypeContextWithModel).OriginalString.Should().Be(BuildExpectedContextUri("#SingletonCity", false));
        }

        [Fact]
        public void ShouldNotIncludeEntityOnSingletonWithoutModel()
        {
            ODataFeedAndEntrySerializationInfo serializationInfo = new ODataFeedAndEntrySerializationInfo() { ExpectedTypeName = "People", NavigationSourceEntityTypeName = "People", NavigationSourceName = "Boss", NavigationSourceKind = EdmNavigationSourceKind.Singleton, };
            var requestSingletonTypeContextWithoutModel = ODataFeedAndEntryTypeContext.Create(serializationInfo, /*navigationSource*/null, /*navigationSourceEntityType*/null, /*expectedEntityType*/null, EdmCoreModel.Instance, true);
            this.CreateEntryContextUri(requestSingletonTypeContextWithoutModel).OriginalString.Should().Be(BuildExpectedContextUri("#Boss", false));
        }

        #endregion entry context uri

        #region link context uri
        [Fact]
        public void ShouldWriteLinkContextUriWithoutSerializationInfo()
        {
            this.CreateEntityReferenceLinkContextUri().OriginalString.Should().Be(BuildExpectedContextUri("#$ref"));
        }

        [Fact]
        public void ShouldWriteLinkContextUriWithSerializationInfo()
        {
            this.CreateEntityReferenceLinkContextUri().OriginalString.Should().Be(BuildExpectedContextUri(ODataConstants.SingleEntityReferencesContextUrlSegment));
        }
        #endregion link context uri

        #region links context uri
        [Fact]
        public void ShouldWriteLinksContextUriWithoutSerializationInfo()
        {
            this.CreateEntityReferenceLinksContextUri().OriginalString.Should().Be(BuildExpectedContextUri(ODataConstants.CollectionOfEntityReferencesContextUrlSegment));
        }

        [Fact]
        public void ShouldWriteLinksContextUriWithSerializationInfo()
        {
            this.CreateEntityReferenceLinksContextUri().OriginalString.Should().Be(BuildExpectedContextUri(ODataConstants.CollectionOfEntityReferencesContextUrlSegment));
        }
        #endregion links context uri

        #region value context uri
        [Fact]
        public void BuildPropertyContextUriForNullPropertyValue()
        {
            var contextUri = this.CreatePropertyContextUri();
            contextUri.OriginalString.Should().Be(BuildExpectedContextUri("#Edm.Null"));
        }

        [Fact]
        public void BuildPropertyContextUriForIntegerPropertyValue()
        {
            var contextUri = this.CreatePropertyContextUri(1);
            contextUri.OriginalString.Should().Be(BuildExpectedContextUri("#Edm.Int32"));
        }

        [Fact]
        public void BuildPropertyContextUriForSpatialPropertyValue()
        {
            var contextUri = this.CreatePropertyContextUri(GeometryPoint.Create(1, 2));
            contextUri.OriginalString.Should().Be(BuildExpectedContextUri("#Edm.GeometryPoint"));
        }

        [Fact]
        public void BuildPropertyContextUriForComplexPropertyValue()
        {
            ODataComplexValue value = new ODataComplexValue { TypeName = "FQNS.FakeType" };
            var contextUri = this.CreatePropertyContextUri(value);
            contextUri.OriginalString.Should().Be(BuildExpectedContextUri("#FQNS.FakeType"));
        }

        [Fact]
        public void BuildPropertyContextUriForCollectionPropertyValue()
        {
            ODataCollectionValue value = new ODataCollectionValue { TypeName = "FQNS.FakeType" };
            var contextUri = this.CreatePropertyContextUri(value);
            contextUri.OriginalString.Should().Be(BuildExpectedContextUri("#FQNS.FakeType"));
        }

        [Fact]
        public void BuildPropertyContextUriForComplexPropertyValueWithNullAnnotation()
        {
            ODataComplexValue value = new ODataComplexValue { TypeName = "FQNS.FakeType" };
            value.SetAnnotation(new SerializationTypeNameAnnotation { TypeName = null });
            var contextUri = this.CreatePropertyContextUri(value);
            contextUri.OriginalString.Should().Be(BuildExpectedContextUri("#FQNS.FakeType"));
        }

        [Fact]
        public void BuildPropertyContextUriForCollectionPropertyValueWithNullAnnotation()
        {
            ODataCollectionValue value = new ODataCollectionValue { TypeName = "FQNS.FakeType" };
            value.SetAnnotation(new SerializationTypeNameAnnotation { TypeName = null });
            var contextUri = this.CreatePropertyContextUri(value);
            contextUri.OriginalString.Should().Be(BuildExpectedContextUri("#FQNS.FakeType"));
        }

        [Fact]
        public void BuildPropertyContextUriForIntegerPropertyValueWithNullAnnotation()
        {
            ODataValue value = new ODataPrimitiveValue(1);
            value.SetAnnotation(new SerializationTypeNameAnnotation { TypeName = null });
            var contextUri = this.CreatePropertyContextUri(value);
            contextUri.OriginalString.Should().Be(BuildExpectedContextUri("#Edm.Int32"));
        }

        [Fact]
        public void BuildPropertyContextUriForComplexPropertyValueWithNonNullAnnotation()
        {
            ODataComplexValue value = new ODataComplexValue { TypeName = "FQNS.FromObject" };
            value.SetAnnotation(new SerializationTypeNameAnnotation { TypeName = "FQNS.FromAnnotation" });
            var contextUri = this.CreatePropertyContextUri(value);
            contextUri.OriginalString.Should().Be(BuildExpectedContextUri("#FQNS.FromAnnotation"));
        }

        [Fact]
        public void BuildPropertyContextUriForCollectionPropertyValueWithNonNullAnnotation()
        {
            ODataCollectionValue value = new ODataCollectionValue { TypeName = "FQNS.FromObject" };
            value.SetAnnotation(new SerializationTypeNameAnnotation { TypeName = "FQNS.FromAnnotation" });
            var contextUri = this.CreatePropertyContextUri(value);
            contextUri.OriginalString.Should().Be(BuildExpectedContextUri("#FQNS.FromAnnotation"));
        }

        [Fact]
        public void BuildPropertyContextUriForIntegerPropertyValueWithNonNullAnnotation()
        {
            ODataValue value = new ODataPrimitiveValue(1);
            value.SetAnnotation(new SerializationTypeNameAnnotation { TypeName = "FQNS.FromAnnotation" });
            var contextUri = this.CreatePropertyContextUri(value);
            contextUri.OriginalString.Should().Be(BuildExpectedContextUri("#FQNS.FromAnnotation"));
        }

        [Fact]
        public void BuildPropertyContextUriForComplexPropertyValueWithNoNameShouldFail()
        {
            Action withStream = () => this.CreatePropertyContextUri(new ODataComplexValue());
            withStream.ShouldThrow<ODataException>().WithMessage(Microsoft.OData.Core.Strings.ODataContextUriBuilder_TypeNameMissingForProperty);
        }

        [Fact]
        public void BuildPropertyContextUriForCollectionPropertyValueWithNoNameShouldFail()
        {
            Action withStream = () => this.CreatePropertyContextUri(new ODataCollectionValue());
            withStream.ShouldThrow<ODataException>().WithMessage(Microsoft.OData.Core.Strings.ODataContextUriBuilder_TypeNameMissingForProperty);
        }


        [Fact]
        public void BuildPropertyContextUriForStreamValueShouldFail()
        {
            Action withStream = () => this.CreatePropertyContextUri(new ODataStreamReferenceValue());
            withStream.ShouldThrow<ODataException>().WithMessage(Microsoft.OData.Core.Strings.ODataContextUriBuilder_StreamValueMustBePropertiesOfODataEntry);
        }
        #endregion value context uri

        #region collection context uri
        [Fact]
        public void ShouldWriteCollectionContextUri()
        {
            this.CreateCollectionContextUri(null, EdmCoreModel.Instance.GetString(isNullable: false)).OriginalString.Should().Be(BuildExpectedContextUri("#Collection(Edm.String)"));
        }

        [Fact]
        public void CollectionSerializationInfoShouldOverrideEdmMetadata()
        {
            var collectionStartSerializationInfo1 = new ODataCollectionStartSerializationInfo { CollectionTypeName = "Collection(Edm.Guid)" };
            this.CreateCollectionContextUri(collectionStartSerializationInfo1, EdmCoreModel.Instance.GetString(isNullable: false)).OriginalString.Should().Be(BuildExpectedContextUri("#Collection(Edm.Guid)"));
        }

        [Fact]
        public void ShouldThrowIfTypeNameIsMissingOnCollectionResponse()
        {
            Action action = () => this.CreateCollectionContextUri(serializationInfo: null, itemTypeReference: null).OriginalString.Should().Be(BuildExpectedContextUri("#Collection(Edm.Guid)"));
            action.ShouldThrow<ODataException>().WithMessage(Strings.ODataContextUriBuilder_TypeNameMissingForTopLevelCollection);
        }
        #endregion collection context uri

        #region individual property context uri
        [Fact]
        public void ShouldWriteIndividualPropertyContextUriForPrimitiveType()
        {
            const string name = "IAmName";
            ODataValue value = name.ToODataValue();
            this.CreateIndividualPropertyContextUri(value, "Cities(9)/Name").OriginalString.Should().Be(BuildExpectedContextUri("#Cities(9)/Name"));
        }

        [Fact]
        public void ShouldWriteIndividualPropertyContextUriForCollectionType()
        {
            ODataValue value = new ODataCollectionValue();
            this.CreateIndividualPropertyContextUri(value, "Cities(9)/Restaurants").OriginalString.Should().Be(BuildExpectedContextUri("#Cities(9)/Restaurants"));
        }
        #endregion individual property context uri

        #region delta context uri
        [Fact]
        public void ShouldWriteDeltaFeedContextUri()
        {
            this.CreateDeltaResponseContextUri(responseCityTypeContextWithoutSerializationInfo, ODataDeltaKind.Feed).OriginalString.Should().Be(BuildExpectedContextUri("#Cities/$delta"));
            this.CreateDeltaResponseContextUri(responseCapitolCityTypeContextWithoutSerializationInfo, ODataDeltaKind.Feed).OriginalString.Should().Be(BuildExpectedContextUri("#Cities/TestModel.CapitolCity/$delta"));
        }

        [Fact]
        public void ShouldWriteDeltaEntryContextUri()
        {
            this.CreateDeltaResponseContextUri(responseCityTypeContextWithoutSerializationInfo, ODataDeltaKind.Entry).OriginalString.Should().Be(BuildExpectedContextUri("#Cities/$entity"));
            this.CreateDeltaResponseContextUri(responseCapitolCityTypeContextWithoutSerializationInfo, ODataDeltaKind.Entry).OriginalString.Should().Be(BuildExpectedContextUri("#Cities/TestModel.CapitolCity/$entity"));
        }

        [Fact]
        public void ShouldWriteDeltaDeletedEntryContextUri()
        {
            this.CreateDeltaResponseContextUri(responseCityTypeContextWithoutSerializationInfo, ODataDeltaKind.DeletedEntry).OriginalString.Should().Be(BuildExpectedContextUri("#Cities/$deletedEntity"));
            this.CreateDeltaResponseContextUri(responseCapitolCityTypeContextWithoutSerializationInfo, ODataDeltaKind.DeletedEntry).OriginalString.Should().Be(BuildExpectedContextUri("#Cities/$deletedEntity"));
        }

        [Fact]
        public void ShouldWriteDeltaLinkContextUri()
        {
            this.CreateDeltaResponseContextUri(responseCityTypeContextWithoutSerializationInfo, ODataDeltaKind.Link).OriginalString.Should().Be(BuildExpectedContextUri("#Cities/$link"));
            this.CreateDeltaResponseContextUri(responseCapitolCityTypeContextWithoutSerializationInfo, ODataDeltaKind.Link).OriginalString.Should().Be(BuildExpectedContextUri("#Cities/$link"));
        }

        [Fact]
        public void ShouldWriteDeltaDeletedLinkContextUri()
        {
            this.CreateDeltaResponseContextUri(responseCityTypeContextWithoutSerializationInfo, ODataDeltaKind.DeletedLink).OriginalString.Should().Be(BuildExpectedContextUri("#Cities/$deletedLink"));
            this.CreateDeltaResponseContextUri(responseCapitolCityTypeContextWithoutSerializationInfo, ODataDeltaKind.DeletedLink).OriginalString.Should().Be(BuildExpectedContextUri("#Cities/$deletedLink"));
        }
        #endregion delta context uri

        #region NoMetadata
        [Fact]
        public void ShouldNotRequireContextUriInResponsesForNoMetadata()
        {
            ODataContextUriBuilder.Create(null, false).Should().NotBeNull();

        }

        [Fact]
        public void FeedContextUriShouldNotBeWrittenIfNotProvided()
        {
            var serializationInfo = new ODataFeedAndEntrySerializationInfo { NavigationSourceName = "MyContainer.MyCities", NavigationSourceEntityTypeName = "TestModel.MyCity", ExpectedTypeName = "TestModel.MyCity" };
            var typeContext = ODataFeedAndEntryTypeContext.Create(serializationInfo, null, null, null, EdmCoreModel.Instance, true);
            this.builderWithNoMetadataDocumentUri.BuildContextUri(ODataPayloadKind.Feed, ODataContextUrlInfo.Create(typeContext, false)).Should().BeNull();
        }

        [Fact]
        public void EntryContextUriShouldNotBeWrittenIfNotProvided()
        {
            var serializationInfo = new ODataFeedAndEntrySerializationInfo { NavigationSourceName = "MyContainer.MyCities", NavigationSourceEntityTypeName = "TestModel.MyCity", ExpectedTypeName = "TestModel.MyCity" };
            var typeContext = ODataFeedAndEntryTypeContext.Create(serializationInfo, null, null, null, EdmCoreModel.Instance, true);
            this.builderWithNoMetadataDocumentUri.BuildContextUri(ODataPayloadKind.Entry, ODataContextUrlInfo.Create(typeContext, true)).Should().BeNull();
        }

        [Fact]
        public void CollectionContextUriShouldNotBeWrittenIfNotProvided()
        {
            var contextInfo = ODataContextUrlInfo.Create(new ODataCollectionStartSerializationInfo { CollectionTypeName = "Collection(Edm.Guid)" }, EdmCoreModel.Instance.GetString(false));
            this.builderWithNoMetadataDocumentUri.BuildContextUri(ODataPayloadKind.Collection, contextInfo).Should().BeNull();
        }

        [Fact]
        public void ValueContextUriShouldNotBeWrittenIfNotProvided()
        {
            var contextInfo = ODataContextUrlInfo.Create(new ODataProperty().ODataValue);
            this.builderWithNoMetadataDocumentUri.BuildContextUri(ODataPayloadKind.Property, contextInfo).Should().BeNull();
        }

        [Fact]
        public void LinkContextUriShouldNotBeWrittenIfNotProvided()
        {
            this.builderWithNoMetadataDocumentUri.BuildContextUri(ODataPayloadKind.EntityReferenceLink).Should().BeNull();
        }

        [Fact]
        public void LinksContextUriShouldNotBeWrittenIfNotProvided()
        {
            this.builderWithNoMetadataDocumentUri.BuildContextUri(ODataPayloadKind.EntityReferenceLinks).Should().BeNull();
        }

        [Fact]
        public void ServiceDocumentContextUriShouldNotBeWrittenIfNotProvided()
        {
            this.builderWithNoMetadataDocumentUri.BuildContextUri(ODataPayloadKind.ServiceDocument).Should().BeNull();
        }
        #endregion NoMetadata

        #region Helper methods
        private void InitalizeBuilder()
        {
            this.metadataDocumentBaseUri = new Uri(MetadataDocumentUriString);
            this.responseContextUriBuilder = ODataContextUriBuilder.Create(this.metadataDocumentBaseUri, true);
            this.requestContextUriBuilder = ODataContextUriBuilder.Create(this.metadataDocumentBaseUri, false);
            this.builderWithNoMetadataDocumentUri = ODataContextUriBuilder.Create(null, false);
        }

        private void InitializeEdmModel()
        {
            this.edmModel = new EdmModel();

            EdmEntityContainer defaultContainer = new EdmEntityContainer("TestModel", "DefaultContainer");
            this.edmModel.AddElement(defaultContainer);

            this.cityType = new EdmEntityType("TestModel", "City");
            EdmStructuralProperty cityIdProperty = cityType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(/*isNullable*/false));
            cityType.AddKeys(cityIdProperty);
            cityType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(/*isNullable*/false));
            cityType.AddStructuralProperty("Size", EdmCoreModel.Instance.GetInt32(/*isNullable*/false));
            cityType.AddStructuralProperty("Restaurants", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(/*isNullable*/false)));
            this.edmModel.AddElement(cityType);

            this.capitolCityType = new EdmEntityType("TestModel", "CapitolCity", cityType);
            capitolCityType.AddStructuralProperty("CapitolType", EdmCoreModel.Instance.GetString( /*isNullable*/false));
            this.edmModel.AddElement(capitolCityType);

            EdmEntityType districtType = new EdmEntityType("TestModel", "District");
            EdmStructuralProperty districtIdProperty = districtType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(/*isNullable*/false));
            districtType.AddKeys(districtIdProperty);
            districtType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(/*isNullable*/false));
            districtType.AddStructuralProperty("Zip", EdmCoreModel.Instance.GetInt32(/*isNullable*/false));
            this.edmModel.AddElement(districtType);

            cityType.AddBidirectionalNavigation(
                new EdmNavigationPropertyInfo { Name = "Districts", Target = districtType, TargetMultiplicity = EdmMultiplicity.Many },
                new EdmNavigationPropertyInfo { Name = "City", Target = cityType, TargetMultiplicity = EdmMultiplicity.One });

            cityType.NavigationProperties().Single(np => np.Name == "Districts");
            capitolCityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "CapitolDistrict", Target = districtType, TargetMultiplicity = EdmMultiplicity.One });
            capitolCityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "OutlyingDistricts", Target = districtType, TargetMultiplicity = EdmMultiplicity.Many });

            this.citySet = defaultContainer.AddEntitySet("Cities", cityType);
            defaultContainer.AddEntitySet("Districts", districtType);

            this.singletonCity = defaultContainer.AddSingleton("SingletonCity", cityType);
        }

        private void InitializeTypeContext()
        {
            this.responseCityTypeContextWithoutSerializationInfo = ODataFeedAndEntryTypeContext.Create( /*serializationInfo*/null, this.citySet, this.cityType, this.cityType, this.edmModel, throwIfMissingTypeInfo: true);
            this.responseCapitolCityTypeContextWithoutSerializationInfo = ODataFeedAndEntryTypeContext.Create( /*serializationInfo*/null, this.citySet, this.cityType, this.capitolCityType, this.edmModel, throwIfMissingTypeInfo: true);
        }

        private Uri CreateCollectionContextUri(ODataCollectionStartSerializationInfo serializationInfo, IEdmTypeReference itemTypeReference)
        {
            ODataContextUrlInfo info = ODataContextUrlInfo.Create(serializationInfo, itemTypeReference);
            Uri contextUrl = this.responseContextUriBuilder.BuildContextUri(ODataPayloadKind.Collection, info);
            contextUrl.Should().NotBeNull();
            return contextUrl;
        }

        private Uri CreatePropertyContextUri(object value = null)
        {
            ODataProperty property = new ODataProperty() { Value = value };
            ODataContextUrlInfo info = ODataContextUrlInfo.Create(property.ODataValue);
            Uri contextUrl = this.responseContextUriBuilder.BuildContextUri(ODataPayloadKind.Property, info);
            contextUrl.Should().NotBeNull();
            return contextUrl;
        }

        private Uri CreateIndividualPropertyContextUri(ODataValue value, string resourcePath)
        {
            ODataContextUrlInfo info = ODataContextUrlInfo.Create(value, new ODataUri() { Path = new ODataUriParser(edmModel, new Uri(ServiceDocumentUriString), new Uri(ServiceDocumentUriString + resourcePath)).ParsePath() });
            Uri contextUrl = this.responseContextUriBuilder.BuildContextUri(ODataPayloadKind.IndividualProperty, info);
            contextUrl.Should().NotBeNull();
            return contextUrl;
        }

        private Uri CreateFeedContextUri(string selectClause, string expandClause)
        {
            SelectExpandClause selectExpandClause = new ODataQueryOptionParser(edmModel, this.cityType, this.citySet, new Dictionary<string, string> { { "$expand", expandClause }, { "$select", selectClause } }).ParseSelectAndExpand();
            ODataFeedAndEntryTypeContext typeContext = ODataFeedAndEntryTypeContext.Create( /*serializationInfo*/null, this.citySet, this.cityType, this.cityType, this.edmModel, true);
            ODataContextUrlInfo info = ODataContextUrlInfo.Create(typeContext, false, new ODataUri() { SelectAndExpand = selectExpandClause });
            Uri contextUrl = this.responseContextUriBuilder.BuildContextUri(ODataPayloadKind.Feed, info);
            return contextUrl;
        }

        private Uri CreateEntryContextUri(string selectClause, string expandClause)
        {
            SelectExpandClause selectExpandClause = new ODataQueryOptionParser(edmModel, this.cityType, this.citySet, new Dictionary<string, string> { { "$expand", expandClause }, { "$select", selectClause } }).ParseSelectAndExpand();
            ODataFeedAndEntryTypeContext typeContext = ODataFeedAndEntryTypeContext.Create( /*serializationInfo*/null, this.citySet, this.cityType, this.cityType, this.edmModel, true);
            ODataContextUrlInfo info = ODataContextUrlInfo.Create(typeContext, true, new ODataUri() { SelectAndExpand = selectExpandClause });
            Uri contextUrl = this.responseContextUriBuilder.BuildContextUri(ODataPayloadKind.Feed, info);
            return contextUrl;
        }

        private Uri CreateFeedContextUri(ODataFeedAndEntryTypeContext typeContext, bool isResponse = true)
        {
            ODataContextUrlInfo info = ODataContextUrlInfo.Create(typeContext, false);
            Uri contextUrl = isResponse ?
                this.responseContextUriBuilder.BuildContextUri(ODataPayloadKind.Feed, info) :
                this.requestContextUriBuilder.BuildContextUri(ODataPayloadKind.Feed, info);
            return contextUrl;
        }

        private Uri CreateEntryContextUri(ODataFeedAndEntryTypeContext typeContext, bool isResponse = true)
        {
            ODataContextUrlInfo info = ODataContextUrlInfo.Create(typeContext, true);
            Uri contextUrl = isResponse ?
                this.responseContextUriBuilder.BuildContextUri(ODataPayloadKind.Entry, info) :
                this.requestContextUriBuilder.BuildContextUri(ODataPayloadKind.Entry, info);

            return contextUrl;
        }

        private Uri CreateEntityReferenceLinkContextUri()
        {
            Uri contextUrl = this.responseContextUriBuilder.BuildContextUri(ODataPayloadKind.EntityReferenceLink);
            contextUrl.Should().NotBeNull();
            return contextUrl;
        }

        private Uri CreateEntityReferenceLinksContextUri()
        {
            Uri contextUrl = this.responseContextUriBuilder.BuildContextUri(ODataPayloadKind.EntityReferenceLinks);
            contextUrl.Should().NotBeNull();
            return contextUrl;
        }

        private Uri CreateDeltaResponseContextUri(ODataFeedAndEntryTypeContext typeContext, ODataDeltaKind kind)
        {
            ODataContextUrlInfo info = ODataContextUrlInfo.Create(typeContext, kind);
            Uri contextUrl = this.responseContextUriBuilder.BuildContextUri(ODataPayloadKind.Delta, info);
            contextUrl.Should().NotBeNull();
            return contextUrl;
        }

        private static string BuildExpectedContextUri(string expectedFragment, bool isEntity = false, string selectExpandValue = null)
        {
            if (!string.IsNullOrEmpty(selectExpandValue))
            {
                expectedFragment += "(" + selectExpandValue + ")";
            }

            if (isEntity)
            {
                expectedFragment += "/$entity";
            }

            return MetadataDocumentUriString + expectedFragment;
        }
        #endregion Helper methods
    }
}

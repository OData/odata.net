//---------------------------------------------------------------------
// <copyright file="ODataContextUriBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.UriParser.Aggregation;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Microsoft.Spatial;
using Xunit;
using Xunit.Sdk;

namespace Microsoft.OData.Tests
{
    public class ODataContextUriBuilderTests
    {
        private const string ServiceDocumentUriString = "http://odata.org/service/";
        private const string MetadataDocumentUriString = "http://odata.org/service/$metadata";
        private static readonly ODataResourceTypeContext ResponseTypeContextWithoutTypeInfo = ODataResourceTypeContext.Create(serializationInfo: null, navigationSource: null, navigationSourceEntityType: null, expectedResourceType: null, throwIfMissingTypeInfo: true);
        private static readonly ODataResourceTypeContext RequestTypeContextWithoutTypeInfo = ODataResourceTypeContext.Create(serializationInfo: null, navigationSource: null, navigationSourceEntityType: null, expectedResourceType: null, throwIfMissingTypeInfo: false);

        private Uri metadataDocumentBaseUri;
        private EdmModel edmModel;
        private EdmEntitySet citySet;
        private EdmEntitySet districtSet;
        private EdmSingleton singletonCity;
        private EdmComplexType addressType;
        private EdmEntityType cityType;
        private EdmEntityType districtType;
        private EdmEntityType capitolCityType;
        private ODataContextUriBuilder responseContextUriBuilder;
        private ODataContextUriBuilder requestContextUriBuilder;
        private ODataContextUriBuilder builderWithNoMetadataDocumentUri;
        private ODataResourceTypeContext responseCityTypeContextWithoutSerializationInfo;
        private ODataResourceTypeContext responseCapitolCityTypeContextWithoutSerializationInfo;

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
            action.ShouldThrow<ODataException>().WithMessage(Strings.ODataContextUriBuilder_UnsupportedPayloadKind(ODataPayloadKind.MetadataDocument.ToString()));
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
            action.ShouldThrow<ODataException>().WithMessage(Strings.ODataOutputContext_MetadataDocumentUriMissing);
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
        public void FeedContextUriWithApplyAggreagate()
        {
            string applyClause = "aggregate(Id with sum as TotalId)";

            this.CreateFeedContextUri(applyClause).OriginalString.Should().Be(MetadataDocumentUriString + "#Cities(TotalId)");
        }

        [Theory]
        [InlineData("sum")]
        [InlineData("average")]
        [InlineData("max")]
        [InlineData("min")]
        [InlineData("countdistinct")]
        public void FeedContextUriWithApplyAggreagateOnDynamicProperty(string method)
        {
            string applyClause = "aggregate(DynamicProperty with " + method + " as DynamicPropertyTotal)";

            this.CreateFeedContextUri(applyClause).OriginalString.Should().Be(MetadataDocumentUriString + "#Cities(DynamicPropertyTotal)");
        }

        [Fact]
        public void FeedContextUriWithApplyGroupBy()
        {
            string applyClause = "groupby((Name, Address/Street))";

            this.CreateFeedContextUri(applyClause).OriginalString.Should().Be(MetadataDocumentUriString + "#Cities(Name,Address(Street))");
        }

        [Fact]
        public void FeedContextUriWithApplyGroupByDynamicProperty()
        {
            string applyClause = "groupby((Name, DynamicProperty, Address/Street))";

            this.CreateFeedContextUri(applyClause).OriginalString.Should().Be(MetadataDocumentUriString + "#Cities(Name,DynamicProperty,Address(Street))");
        }

        [Fact]
        public void FeedContextUriWithApplyFilter()
        {
            string applyClause = "filter(Id eq 1)";

            this.CreateFeedContextUri(applyClause).OriginalString.Should().Be(MetadataDocumentUriString + "#Cities");
        }

        [Fact]
        public void FeedContextUriWithApply()
        {
            string applyClause = "groupby((Name), aggregate(Id with sum as TotalId))";

            this.CreateFeedContextUri(applyClause).OriginalString.Should().Be(MetadataDocumentUriString + "#Cities(Name,TotalId)");
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
            expectedClause = "Name,Districts(Name)";
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
            expectedClause = "Districts(Name,Zip)";
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
            expectedClause = "Districts(Name,City(Districts))";
            this.CreateEntryContextUri(null, expandClause).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", true, expectedClause));
        }

        [Fact]
        public void FeedContextUriWithMixedSelectAndExpandString()
        {
            const string selectClause = "Size,Name";
            const string expandClause = "Districts($select=Zip,City;$expand=City($expand=Districts;$select=Name))";
            const string expectedClause = "Size,Name,Districts(Zip,City(Name,Districts))";
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
        public void ShouldThrowIfEntitySetIsMissingWithoutSerializationInfoOnFeedResponse()
        {
            Action test = () => this.CreateFeedContextUri(ResponseTypeContextWithoutTypeInfo);
            test.ShouldThrow<ODataException>().WithMessage(Strings.ODataResourceTypeContext_MetadataOrSerializationInfoMissing);
        }

        [Fact]
        public void ShouldNotWriteContextUriIfEntitySetIsMissingOnFeedRequest()
        {
            this.CreateFeedContextUri(RequestTypeContextWithoutTypeInfo, false).Should().BeNull();
        }

        [Fact]
        public void ShouldWriteIfSerializationInfoWithoutNavigationSourceButUnknownSetOnFeedResponse()
        {
            this.CreateFeedContextUri(ODataResourceTypeContext.Create(
                serializationInfo: new ODataResourceSerializationInfo()
                {
                    ExpectedTypeName = "NS.Type",
                    IsFromCollection = true,
                    NavigationSourceEntityTypeName = "NS.Type",
                    NavigationSourceKind = EdmNavigationSourceKind.UnknownEntitySet,
                    NavigationSourceName = null
                },
                navigationSource: null,
                navigationSourceEntityType: null,
                expectedResourceType: null,
                throwIfMissingTypeInfo: true),
                isResponse: true);
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
            test.ShouldThrow<ODataException>().WithMessage(Strings.ODataResourceTypeContext_MetadataOrSerializationInfoMissing);
        }

        [Fact]
        public void ShouldThrowIfEntitySetIsMissingOnEntryRequest()
        {
            this.CreateEntryContextUri(RequestTypeContextWithoutTypeInfo, false).Should().BeNull();
        }

        [Fact]
        public void ShouldNotIncludeFragmentItemSelectorOnSingleton()
        {
            var singletonTypeContextWithModel = ODataResourceTypeContext.Create( /*serializationInfo*/null, this.singletonCity, this.cityType, this.cityType, throwIfMissingTypeInfo: true);
            this.CreateEntryContextUri(singletonTypeContextWithModel).OriginalString.Should().Be(BuildExpectedContextUri("#SingletonCity", false));
        }

        [Fact]
        public void ShouldNotIncludeEntityOnSingletonWithoutModel()
        {
            ODataResourceSerializationInfo serializationInfo = new ODataResourceSerializationInfo() { ExpectedTypeName = "People", NavigationSourceEntityTypeName = "People", NavigationSourceName = "Boss", NavigationSourceKind = EdmNavigationSourceKind.Singleton, };
            var requestSingletonTypeContextWithoutModel = ODataResourceTypeContext.Create(serializationInfo, /*navigationSource*/null, /*navigationSourceEntityType*/null, /*expectedEntityType*/null, true);
            this.CreateEntryContextUri(requestSingletonTypeContextWithoutModel).OriginalString.Should().Be(BuildExpectedContextUri("#Boss", false));
        }

        [Fact]
        public void ShouldWriteEntryContextUriWithOperationSegment()
        {
            var entitySetSegment = new EntitySetSegment(this.citySet);
            var keys = new[] { new KeyValuePair<string, object>("Id", 123) };
            var keySegment = new KeySegment(keys, cityType, citySet);
            var operation = edmModel.SchemaElements.OfType<IEdmFunction>().First(f => f.Name == "GetOneDistrict");
            OperationSegment operationSegment = new OperationSegment(operation, districtSet);

            ODataPath path = new ODataPath(entitySetSegment, keySegment, operationSegment);
            ODataUri odataUri = new ODataUri
            {
                Path = path
            };
            ODataContextUrlInfo info = ODataContextUrlInfo.Create(this.districtSet, "TestModel.District", true, odataUri);

            Uri uri = this.responseContextUriBuilder.BuildContextUri(ODataPayloadKind.Resource, info);
            uri.OriginalString.Should().Be(BuildExpectedContextUri("#Districts/$entity", false));
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
        public void BuildResourceContextUriForComplexResource()
        {
            var typeContext = ODataResourceTypeContext.Create( /*serializationInfo*/null,
                null, null, this.addressType, throwIfMissingTypeInfo: false);
            ODataResource value = new ODataResource { TypeName = "TestModel.Address" };
            var contextUri = this.CreateEntryContextUri(typeContext, true);
            contextUri.OriginalString.Should().Be(BuildExpectedContextUri("#TestModel.Address"));
        }

        [Fact]
        public void BuildPropertyContextUriForCollectionPropertyValue()
        {
            ODataCollectionValue value = new ODataCollectionValue { TypeName = "FQNS.FakeType" };
            var contextUri = this.CreatePropertyContextUri(value);
            contextUri.OriginalString.Should().Be(BuildExpectedContextUri("#FQNS.FakeType"));
        }

        [Fact]
        public void BuildResourceContextUriForComplexWithNullAnnotation()
        {
            var typeContext = ODataResourceTypeContext.Create( /*serializationInfo*/null,
                null, null, this.addressType, throwIfMissingTypeInfo: true);
            ODataResource value = new ODataResource { TypeName = "TestModel.Address" };
            value.TypeAnnotation = new ODataTypeAnnotation();
            var contextUri = this.CreateEntryContextUri(typeContext, true);
            contextUri.OriginalString.Should().Be(BuildExpectedContextUri("#TestModel.Address"));
        }

        [Fact]
        public void BuildPropertyContextUriForCollectionPropertyValueWithNullAnnotation()
        {
            ODataCollectionValue value = new ODataCollectionValue { TypeName = "FQNS.FakeType" };
            value.TypeAnnotation = new ODataTypeAnnotation();
            var contextUri = this.CreatePropertyContextUri(value);
            contextUri.OriginalString.Should().Be(BuildExpectedContextUri("#FQNS.FakeType"));
        }

        [Fact]
        public void BuildPropertyContextUriForIntegerPropertyValueWithNullAnnotation()
        {
            ODataValue value = new ODataPrimitiveValue(1);
            value.TypeAnnotation = new ODataTypeAnnotation();
            var contextUri = this.CreatePropertyContextUri(value);
            contextUri.OriginalString.Should().Be(BuildExpectedContextUri("#Edm.Int32"));
        }

        [Fact]
        public void BuildPropertyContextUriForCollectionPropertyValueWithNonNullAnnotation()
        {
            ODataCollectionValue value = new ODataCollectionValue { TypeName = "FQNS.FromObject" };
            value.TypeAnnotation = new ODataTypeAnnotation("FQNS.FromAnnotation");
            var contextUri = this.CreatePropertyContextUri(value);
            contextUri.OriginalString.Should().Be(BuildExpectedContextUri("#FQNS.FromAnnotation"));
        }

        [Fact]
        public void BuildPropertyContextUriForIntegerPropertyValueWithNonNullAnnotation()
        {
            ODataValue value = new ODataPrimitiveValue(1);
            value.TypeAnnotation = new ODataTypeAnnotation("FQNS.FromAnnotation");
            var contextUri = this.CreatePropertyContextUri(value);
            contextUri.OriginalString.Should().Be(BuildExpectedContextUri("#FQNS.FromAnnotation"));
        }

        [Fact]
        public void BuildPropertyContextUriForCollectionPropertyValueWithNoNameShouldFail()
        {
            Action withStream = () => this.CreatePropertyContextUri(new ODataCollectionValue());
            withStream.ShouldThrow<ODataException>().WithMessage(Strings.ODataContextUriBuilder_TypeNameMissingForProperty);
        }

        [Fact]
        public void BuildPropertyContextUriForStreamValueShouldFail()
        {
            Action withStream = () => this.CreatePropertyContextUri(new ODataStreamReferenceValue());
            withStream.ShouldThrow<ODataException>().WithMessage(Strings.ODataContextUriBuilder_StreamValueMustBePropertiesOfODataResource);
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
            this.CreateDeltaResponseContextUri(responseCityTypeContextWithoutSerializationInfo, ODataDeltaKind.ResourceSet).OriginalString.Should().Be(BuildExpectedContextUri("#Cities/$delta"));
            this.CreateDeltaResponseContextUri(responseCapitolCityTypeContextWithoutSerializationInfo, ODataDeltaKind.ResourceSet).OriginalString.Should().Be(BuildExpectedContextUri("#Cities/TestModel.CapitolCity/$delta"));
        }

        [Fact]
        public void ShouldWriteDeltaEntryContextUri()
        {
            this.CreateDeltaResponseContextUri(responseCityTypeContextWithoutSerializationInfo, ODataDeltaKind.Resource).OriginalString.Should().Be(BuildExpectedContextUri("#Cities/$entity"));
            this.CreateDeltaResponseContextUri(responseCapitolCityTypeContextWithoutSerializationInfo, ODataDeltaKind.Resource).OriginalString.Should().Be(BuildExpectedContextUri("#Cities/TestModel.CapitolCity/$entity"));
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
            var serializationInfo = new ODataResourceSerializationInfo { NavigationSourceName = "MyContainer.MyCities", NavigationSourceEntityTypeName = "TestModel.MyCity", ExpectedTypeName = "TestModel.MyCity" };
            var typeContext = ODataResourceTypeContext.Create(serializationInfo, null, null, null, true);
            this.builderWithNoMetadataDocumentUri.BuildContextUri(ODataPayloadKind.ResourceSet, ODataContextUrlInfo.Create(typeContext, false)).Should().BeNull();
        }

        [Fact]
        public void EntryContextUriShouldNotBeWrittenIfNotProvided()
        {
            var serializationInfo = new ODataResourceSerializationInfo { NavigationSourceName = "MyContainer.MyCities", NavigationSourceEntityTypeName = "TestModel.MyCity", ExpectedTypeName = "TestModel.MyCity" };
            var typeContext = ODataResourceTypeContext.Create(serializationInfo, null, null, null, true);
            this.builderWithNoMetadataDocumentUri.BuildContextUri(ODataPayloadKind.Resource, ODataContextUrlInfo.Create(typeContext, true)).Should().BeNull();
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

            addressType = new EdmComplexType("TestModel", "Address");
            addressType.AddStructuralProperty("Street", EdmCoreModel.Instance.GetString(/*isNullable*/false));
            addressType.AddStructuralProperty("Zip", EdmCoreModel.Instance.GetString(/*isNullable*/false));

            this.cityType = new EdmEntityType("TestModel", "City", baseType: null, isAbstract: false, isOpen: true);
            EdmStructuralProperty cityIdProperty = cityType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(/*isNullable*/false));
            cityType.AddKeys(cityIdProperty);
            cityType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(/*isNullable*/false));
            cityType.AddStructuralProperty("Size", EdmCoreModel.Instance.GetInt32(/*isNullable*/false));
            cityType.AddStructuralProperty("Restaurants", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(/*isNullable*/false)));
            cityType.AddStructuralProperty("Address", new EdmComplexTypeReference(addressType, true));
            this.edmModel.AddElement(cityType);

            this.capitolCityType = new EdmEntityType("TestModel", "CapitolCity", cityType);
            capitolCityType.AddStructuralProperty("CapitolType", EdmCoreModel.Instance.GetString( /*isNullable*/false));
            this.edmModel.AddElement(capitolCityType);

            this.districtType = new EdmEntityType("TestModel", "District");
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
            this.districtSet = defaultContainer.AddEntitySet("Districts", districtType);

            this.singletonCity = defaultContainer.AddSingleton("SingletonCity", cityType);

            // operations
            var cityReference = new EdmEntityTypeReference(cityType, true);
            var districtReference = new EdmEntityTypeReference(districtType, true);
            IEdmPathExpression path = new EdmPathExpression("binding/Districts");
            var function = new EdmFunction("TestModel", "GetOneDistrict", districtReference, true, path, true /*isComposable*/);
            function.AddParameter("binding", cityReference);
            edmModel.AddElement(function);
        }

        private void InitializeTypeContext()
        {
            this.responseCityTypeContextWithoutSerializationInfo = ODataResourceTypeContext.Create( /*serializationInfo*/null, this.citySet, this.cityType, this.cityType, throwIfMissingTypeInfo: true);
            this.responseCapitolCityTypeContextWithoutSerializationInfo = ODataResourceTypeContext.Create( /*serializationInfo*/null, this.citySet, this.cityType, this.capitolCityType, throwIfMissingTypeInfo: true);
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
            ODataResourceTypeContext typeContext = ODataResourceTypeContext.Create( /*serializationInfo*/null, this.citySet, this.cityType, this.cityType, true);
            ODataContextUrlInfo info = ODataContextUrlInfo.Create(typeContext, false, new ODataUri() { SelectAndExpand = selectExpandClause });
            Uri contextUrl = this.responseContextUriBuilder.BuildContextUri(ODataPayloadKind.ResourceSet, info);
            return contextUrl;
        }
        private Uri CreateFeedContextUri(string applyClauseString)
        {
            ApplyClause applyClause = new ODataQueryOptionParser(edmModel, this.cityType, this.citySet, new Dictionary<string, string> { { "$apply", applyClauseString } }).ParseApply();
            ODataResourceTypeContext typeContext = ODataResourceTypeContext.Create( /*serializationInfo*/null, this.citySet, this.cityType, this.cityType, true);
            ODataContextUrlInfo info = ODataContextUrlInfo.Create(typeContext, false, new ODataUri() { Apply = applyClause });
            Uri contextUrl = this.responseContextUriBuilder.BuildContextUri(ODataPayloadKind.ResourceSet, info);
            return contextUrl;
        }

        private Uri CreateEntryContextUri(string selectClause, string expandClause)
        {
            SelectExpandClause selectExpandClause = new ODataQueryOptionParser(edmModel, this.cityType, this.citySet, new Dictionary<string, string> { { "$expand", expandClause }, { "$select", selectClause } }).ParseSelectAndExpand();
            ODataResourceTypeContext typeContext = ODataResourceTypeContext.Create( /*serializationInfo*/null, this.citySet, this.cityType, this.cityType, true);
            ODataContextUrlInfo info = ODataContextUrlInfo.Create(typeContext, true, new ODataUri() { SelectAndExpand = selectExpandClause });
            Uri contextUrl = this.responseContextUriBuilder.BuildContextUri(ODataPayloadKind.ResourceSet, info);
            return contextUrl;
        }

        private Uri CreateFeedContextUri(ODataResourceTypeContext typeContext, bool isResponse = true)
        {
            ODataContextUrlInfo info = ODataContextUrlInfo.Create(typeContext, false);
            Uri contextUrl = isResponse ?
                this.responseContextUriBuilder.BuildContextUri(ODataPayloadKind.ResourceSet, info) :
                this.requestContextUriBuilder.BuildContextUri(ODataPayloadKind.ResourceSet, info);
            return contextUrl;
        }

        private Uri CreateEntryContextUri(ODataResourceTypeContext typeContext, bool isResponse = true)
        {
            ODataContextUrlInfo info = ODataContextUrlInfo.Create(typeContext, true);
            Uri contextUrl = isResponse ?
                this.responseContextUriBuilder.BuildContextUri(ODataPayloadKind.Resource, info) :
                this.requestContextUriBuilder.BuildContextUri(ODataPayloadKind.Resource, info);

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

        private Uri CreateDeltaResponseContextUri(ODataResourceTypeContext typeContext, ODataDeltaKind kind)
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

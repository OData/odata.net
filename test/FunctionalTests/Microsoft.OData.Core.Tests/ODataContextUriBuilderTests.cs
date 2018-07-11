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
        private static readonly ODataVersion[] Versions = new ODataVersion[] { ODataVersion.V4, ODataVersion.V401 };

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
            foreach (ODataVersion version in Versions)
            {
                this.CreateFeedContextUri(default(string), null, version).OriginalString.Should().Be(BuildExpectedContextUri("#Cities"));
            }
        }

        [Fact]
        public void FeedContextUriWithEmptySelectString()
        {
            foreach (ODataVersion version in Versions)
            {
                this.CreateFeedContextUri(string.Empty, null, version).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", false, string.Empty));
            }
        }

        [Fact]
        public void FeedContextUriWithSelect()
        {
            // Select single structure property
            string selectClause = "Name,Name";
            string expectClause = "Name";
            foreach (ODataVersion version in Versions)
            {
                this.CreateFeedContextUri(selectClause, null, version).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", false, expectClause));
            }

            // Select single navigation property
            selectClause = "Districts, Districts";
            expectClause = "Districts";
            foreach (ODataVersion version in Versions)
            {
                this.CreateFeedContextUri(selectClause, null, version).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", false, expectClause));
            }
        }
        [Fact]
        public void FeedContextUriWithApplyAggreagate()
        {
            string applyClause = "aggregate(Id with sum as TotalId)";
            foreach (ODataVersion version in Versions)
            {
                this.CreateFeedContextUri(applyClause, version).OriginalString.Should().Be(MetadataDocumentUriString + "#Cities(TotalId)");
            }
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
            foreach (ODataVersion version in Versions)
            {
                this.CreateFeedContextUri(applyClause, version).OriginalString.Should().Be(MetadataDocumentUriString + "#Cities(DynamicPropertyTotal)");
            }
        }

        [Fact]
        public void FeedContextUriWithApplyGroupBy()
        {
            string applyClause = "groupby((Name, Address/Street))";
            foreach (ODataVersion version in Versions)
            {
                this.CreateFeedContextUri(applyClause, version).OriginalString.Should().Be(MetadataDocumentUriString + "#Cities(Name,Address(Street))");
            }
        }

        [Fact]
        public void FeedContextUriWithApplyGroupByDynamicProperty()
        {
            string applyClause = "groupby((Name, DynamicProperty, Address/Street))";
            foreach (ODataVersion version in Versions)
            {
                this.CreateFeedContextUri(applyClause, version).OriginalString.Should().Be(MetadataDocumentUriString + "#Cities(Name,DynamicProperty,Address(Street))");
            }
        }

        [Fact]
        public void FeedContextUriWithApplyFilter()
        {
            string applyClause = "filter(Id eq 1)";
            foreach (ODataVersion version in Versions)
            {
                this.CreateFeedContextUri(applyClause, version).OriginalString.Should().Be(MetadataDocumentUriString + "#Cities");
            }
        }
        [Fact]
        public void FeedContextUriWithApply()
        {
            string applyClause = "groupby((Name), aggregate(Id with sum as TotalId))";
            foreach (ODataVersion version in Versions)
            {
                this.CreateFeedContextUri(applyClause, version).OriginalString.Should().Be(MetadataDocumentUriString + "#Cities(Name,TotalId)");
            }
        }

        [Fact]
        public void FeedContextUriWithWildcardSelectString()
        {
            const string selectClause = "Id,Name,*";
            const string expectClause = "*";
            foreach (ODataVersion version in Versions)
            {
                this.CreateFeedContextUri(selectClause, null, version).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", false, expectClause));
            }
        }

        [Fact]
        public void EntryContextUriWithNoSelectString()
        {
            foreach (ODataVersion version in Versions)
            {
                this.CreateEntryContextUri(default(string), null, version).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", true));
            }
        }

        [Fact]
        public void EntryContextUriWithEmptySelectString()
        {
            foreach (ODataVersion version in Versions)
            {
                this.CreateEntryContextUri(string.Empty, null, version).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", true, string.Empty));
            }
        }

        [Fact]
        public void EntryContextUriWithSelectString()
        {
            const string selectClause = "Id,Name,*";
            const string expectClause = "*";
            foreach (ODataVersion version in Versions)
            {
                this.CreateEntryContextUri(selectClause, null, version).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", true, expectClause));
            }
        }

        [Theory]
        // expand without select, $expand=A
        [InlineData(ODataVersion.V4, "TestModel.CapitolCity/Districts", "")]
        [InlineData(ODataVersion.V401, "TestModel.CapitolCity/Districts", "TestModel.CapitolCity/Districts()")]
        // expands without select, $expand=A,B
        [InlineData(ODataVersion.V4, "TestModel.CapitolCity/CapitolDistrict,TestModel.CapitolCity/Districts", "")]
        [InlineData(ODataVersion.V401, "TestModel.CapitolCity/CapitolDistrict,TestModel.CapitolCity/Districts", "TestModel.CapitolCity/CapitolDistrict(),TestModel.CapitolCity/Districts()")]
        // expand with nested select, $expand=A($select=B)
        [InlineData(ODataVersion.V4, "TestModel.CapitolCity/Districts($select=Name)", "TestModel.CapitolCity/Districts(Name)")]
        [InlineData(ODataVersion.V401, "TestModel.CapitolCity/Districts($select=Name)", "TestModel.CapitolCity/Districts(Name)")]

        public void FeedContextUriWithSingleExpandString(ODataVersion version, string expandClause, string expectedClause)
        {
            this.CreateFeedContextUri("", expandClause, version).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", false, expectedClause));
        }

        [Theory]
        // $select=A&$expand=B
        [InlineData(ODataVersion.V4, "Name", "Districts", "Name,Districts")]
        [InlineData(ODataVersion.V401, "Name", "Districts", "Name,Districts()")]
        // $select=A&$expand=A
        [InlineData(ODataVersion.V4, "Districts", "Districts", "Districts")]
        [InlineData(ODataVersion.V401, "Districts", "Districts", "Districts()")]
        // $select=A,B,C&$expand=A
        [InlineData(ODataVersion.V4, "Name,Districts,Size", "Districts", "Name,Districts,Size")]
        [InlineData(ODataVersion.V401, "Name,Districts,Size", "Districts", "Name,Size,Districts()")]
        // $select=A&$expand=A,B
        [InlineData(ODataVersion.V4, "Districts", "Districts,TestModel.CapitolCity/CapitolDistrict", "Districts,TestModel.CapitolCity/CapitolDistrict")]
        [InlineData(ODataVersion.V401, "Districts", "Districts,TestModel.CapitolCity/CapitolDistrict", "Districts(),TestModel.CapitolCity/CapitolDistrict()")]
        // $select=A,B&$expand=B($select=C)
        [InlineData(ODataVersion.V4, "Name,Districts", "Districts($select=Name)", "Name,Districts(Name)")]
        [InlineData(ODataVersion.V401, "Name,Districts", "Districts($select=Name)", "Name,Districts(Name)")]
        public void FeedContextUriWithSelectAndExpandString(ODataVersion version, string selectClause, string expandClause, string expectedClause)
        {
            this.CreateFeedContextUri(selectClause, expandClause, version).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", false, expectedClause));
        }

        [Fact]
        public void EntryContextUriWithExpandNestedSelectString()
        {
            foreach (ODataVersion version in Versions)
            {
                // With out $select in same level, $expand=A($select=B)
                string selectClause = "";
                string expandClause = "Districts($select=Name,Zip)";
                string expectedClause = "Districts(Name,Zip)";
                this.CreateEntryContextUri(selectClause, expandClause, version).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", true, expectedClause));

                // With $select in same level, $select=A&$expand=A($select=B,C)
                selectClause = "Districts";
                expandClause = "Districts($select=Name,Zip)";
                expectedClause = "Districts(Name,Zip)";
                this.CreateEntryContextUri(selectClause, expandClause, version).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", true, expectedClause));
            }
        }

        [Theory]
        [InlineData(ODataVersion.V4, "")]
        [InlineData(ODataVersion.V401, "Districts(City(Districts()))")]
        public void EntryContextUriWithExpandNestedExpandString(ODataVersion version, string expectedExpandWithoutNesting)
        {
            // Without inner $select, $expand=A($expand=B($expand=C))
            string expandClause = "Districts($expand=City($expand=Districts))";
            this.CreateEntryContextUri(null, expandClause, version).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", true, expectedExpandWithoutNesting));

            // With inner $select, $expand=A($expand=B($select=C))
            expandClause = "Districts($expand=City($select=Districts))";
            string expectedClause = "Districts(City(Districts))";
            this.CreateEntryContextUri(null, expandClause, version).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", true, expectedClause));

            // With inner $expand, $expand=A($expand=C($select=D)))
            expandClause = "Districts($expand=City($select=Districts))";
            expectedClause = "Districts(City(Districts))";
            this.CreateEntryContextUri(null, expandClause, version).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", true, expectedClause));

            // With inner $select and $expand, $expand=A($select=B;$expand=C($select=D)))
            expandClause = "Districts($select=Name;$expand=City($select=Districts))";
            expectedClause = "Districts(Name,City(Districts))";
            this.CreateEntryContextUri(null, expandClause, version).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", true, expectedClause));
        }

        [Theory]
        [InlineData(ODataVersion.V4, "Size,Name,Districts(Zip,City(Name,Districts))")]
        [InlineData(ODataVersion.V401, "Size,Name,Districts(Zip,City(Name,Districts()))")]
        public void FeedContextUriWithMixedSelectAndExpandString(ODataVersion version, string expectedClause)
        {
            const string selectClause = "Size,Name";
            const string expandClause = "Districts($select=Zip,City;$expand=City($expand=Districts;$select=Name))";
            this.CreateFeedContextUri(selectClause, expandClause, version).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", false, expectedClause));
        }
        #endregion context uri with $select and $expand

        #region feed context uri
        [Fact]
        public void ShouldWriteFeedContextUriWithoutTypecast()
        {
            foreach (ODataVersion version in Versions)
            {
                this.CreateFeedContextUri(this.responseCityTypeContextWithoutSerializationInfo, version).OriginalString.Should().Be(BuildExpectedContextUri("#Cities"));
            }
        }

        [Fact]
        public void ShouldWriteFeedContextUriWithTypecast()
        {
            foreach (ODataVersion version in Versions)
            {
                this.CreateFeedContextUri(this.responseCapitolCityTypeContextWithoutSerializationInfo, version).OriginalString.Should().Be(BuildExpectedContextUri("#Cities/TestModel.CapitolCity"));
            }
        }

        [Fact]
        public void ShouldThrowIfEntitySetIsMissingWithoutSerializationInfoOnFeedResponse()
        {
            foreach (ODataVersion version in Versions)
            {
                Action test = () => this.CreateFeedContextUri(ResponseTypeContextWithoutTypeInfo, version);
                test.ShouldThrow<ODataException>().WithMessage(Strings.ODataResourceTypeContext_MetadataOrSerializationInfoMissing);
            }
        }

        [Fact]
        public void ShouldNotWriteContextUriIfEntitySetIsMissingOnFeedRequest()
        {
            foreach (ODataVersion version in Versions)
            {
                this.CreateFeedContextUri(RequestTypeContextWithoutTypeInfo, version, false).Should().BeNull();
            }
        }

        [Fact]
        public void ShouldWriteIfSerializationInfoWithoutNavigationSourceButUnknownSetOnFeedResponse()
        {
            foreach (ODataVersion version in Versions)
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
                version,
                isResponse: true);
            }
        }
        #endregion feed context uri

        #region entry context uri
        [Fact]
        public void ShouldWriteEntryContextUriWithoutTypecast()
        {
            foreach (ODataVersion version in Versions)
            {
                this.CreateEntryContextUri(this.responseCityTypeContextWithoutSerializationInfo, version).OriginalString.Should().Be(BuildExpectedContextUri("#Cities", true));
            }
        }

        [Fact]
        public void ShouldWriteEntryContextUriWithTypecast()
        {
            foreach (ODataVersion version in Versions)
            {
                this.CreateEntryContextUri(this.responseCapitolCityTypeContextWithoutSerializationInfo, version).OriginalString.Should().Be(BuildExpectedContextUri("#Cities/TestModel.CapitolCity", true));
            }
        }

        [Fact]
        public void ShouldThrowIfEntitySetIsMissingOnEntryResponse()
        {
            foreach (ODataVersion version in Versions)
            {
                Action test = () => this.CreateEntryContextUri(ResponseTypeContextWithoutTypeInfo, version);
                test.ShouldThrow<ODataException>().WithMessage(Strings.ODataResourceTypeContext_MetadataOrSerializationInfoMissing);
            }
        }

        [Fact]
        public void ShouldThrowIfEntitySetIsMissingOnEntryRequest()
        {
            foreach (ODataVersion version in Versions)
            {
                this.CreateEntryContextUri(RequestTypeContextWithoutTypeInfo, version, false).Should().BeNull();
            }
        }

        [Fact]
        public void ShouldNotIncludeFragmentItemSelectorOnSingleton()
        {
            foreach (ODataVersion version in Versions)
            {
                var singletonTypeContextWithModel = ODataResourceTypeContext.Create( /*serializationInfo*/null, this.singletonCity, this.cityType, this.cityType, throwIfMissingTypeInfo: true);
                this.CreateEntryContextUri(singletonTypeContextWithModel, version).OriginalString.Should().Be(BuildExpectedContextUri("#SingletonCity", false));
            }
        }

        [Fact]
        public void ShouldNotIncludeEntityOnSingletonWithoutModel()
        {
            ODataResourceSerializationInfo serializationInfo = new ODataResourceSerializationInfo() { ExpectedTypeName = "People", NavigationSourceEntityTypeName = "People", NavigationSourceName = "Boss", NavigationSourceKind = EdmNavigationSourceKind.Singleton, };
            var requestSingletonTypeContextWithoutModel = ODataResourceTypeContext.Create(serializationInfo, /*navigationSource*/null, /*navigationSourceEntityType*/null, /*expectedEntityType*/null, true);
            foreach (ODataVersion version in Versions)
            {
                this.CreateEntryContextUri(requestSingletonTypeContextWithoutModel, version).OriginalString.Should().Be(BuildExpectedContextUri("#Boss", false));
            }
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
            ODataContextUrlInfo info = ODataContextUrlInfo.Create(this.districtSet, "TestModel.District", true, odataUri, ODataVersion.V4);

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
            foreach (ODataVersion version in Versions)
            {
                var contextUri = this.CreatePropertyContextUri(1, version);
                contextUri.OriginalString.Should().Be(BuildExpectedContextUri("#Edm.Int32"));
            }
        }

        [Fact]
        public void BuildPropertyContextUriForSpatialPropertyValue()
        {
            foreach (ODataVersion version in Versions)
            {
                var contextUri = this.CreatePropertyContextUri(GeometryPoint.Create(1, 2), version);
                contextUri.OriginalString.Should().Be(BuildExpectedContextUri("#Edm.GeometryPoint"));
            }
        }

        [Fact]
        public void BuildResourceContextUriForComplexResource()
        {
            var typeContext = ODataResourceTypeContext.Create( /*serializationInfo*/null,
                null, null, this.addressType, throwIfMissingTypeInfo: false);
            ODataResource value = new ODataResource { TypeName = "TestModel.Address" };
            foreach (ODataVersion version in Versions)
            {
                var contextUri = this.CreateEntryContextUri(typeContext, version, true);
                contextUri.OriginalString.Should().Be(BuildExpectedContextUri("#TestModel.Address"));
            }
        }

        [Fact]
        public void BuildPropertyContextUriForCollectionPropertyValue()
        {
            foreach (ODataVersion version in Versions)
            {
                ODataCollectionValue value = new ODataCollectionValue { TypeName = "FQNS.FakeType" };
                var contextUri = this.CreatePropertyContextUri(value, version);
                contextUri.OriginalString.Should().Be(BuildExpectedContextUri("#FQNS.FakeType"));
            }
        }

        [Fact]
        public void BuildResourceContextUriForComplexWithNullAnnotation()
        {
            var typeContext = ODataResourceTypeContext.Create( /*serializationInfo*/null,
                null, null, this.addressType, throwIfMissingTypeInfo: true);
            ODataResource value = new ODataResource { TypeName = "TestModel.Address" };
            value.TypeAnnotation = new ODataTypeAnnotation();
            foreach (ODataVersion version in Versions)
            {
                var contextUri = this.CreateEntryContextUri(typeContext, version, true);
                contextUri.OriginalString.Should().Be(BuildExpectedContextUri("#TestModel.Address"));
            }
        }

        [Fact]
        public void BuildPropertyContextUriForCollectionPropertyValueWithNullAnnotation()
        {
            foreach (ODataVersion version in Versions)
            {
                ODataCollectionValue value = new ODataCollectionValue { TypeName = "FQNS.FakeType" };
                value.TypeAnnotation = new ODataTypeAnnotation();
                var contextUri = this.CreatePropertyContextUri(value, version);
                contextUri.OriginalString.Should().Be(BuildExpectedContextUri("#FQNS.FakeType"));
            }
        }

        [Fact]
        public void BuildPropertyContextUriForIntegerPropertyValueWithNullAnnotation()
        {
            foreach (ODataVersion version in Versions)
            {
                ODataValue value = new ODataPrimitiveValue(1);
                value.TypeAnnotation = new ODataTypeAnnotation();
                var contextUri = this.CreatePropertyContextUri(value, version);
                contextUri.OriginalString.Should().Be(BuildExpectedContextUri("#Edm.Int32"));
            }
        }

        [Fact]
        public void BuildPropertyContextUriForCollectionPropertyValueWithNonNullAnnotation()
        {
            foreach (ODataVersion version in Versions)
            {
                ODataCollectionValue value = new ODataCollectionValue { TypeName = "FQNS.FromObject" };
                value.TypeAnnotation = new ODataTypeAnnotation("FQNS.FromAnnotation");
                var contextUri = this.CreatePropertyContextUri(value, version);
                contextUri.OriginalString.Should().Be(BuildExpectedContextUri("#FQNS.FromAnnotation"));
            }
        }

        [Fact]
        public void BuildPropertyContextUriForIntegerPropertyValueWithNonNullAnnotation()
        {
            foreach (ODataVersion version in Versions)
            {
                ODataValue value = new ODataPrimitiveValue(1);
                value.TypeAnnotation = new ODataTypeAnnotation("FQNS.FromAnnotation");
                var contextUri = this.CreatePropertyContextUri(value, version);
                contextUri.OriginalString.Should().Be(BuildExpectedContextUri("#FQNS.FromAnnotation"));
            }
        }

        [Fact]
        public void BuildPropertyContextUriForCollectionPropertyValueWithNoNameShouldFail()
        {
            foreach (ODataVersion version in Versions)
            {
                Action withStream = () => this.CreatePropertyContextUri(new ODataCollectionValue(), version);
                withStream.ShouldThrow<ODataException>().WithMessage(Strings.ODataContextUriBuilder_TypeNameMissingForProperty);
            }
        }

        [Fact]
        public void BuildPropertyContextUriForStreamValueShouldFail()
        {
            foreach (ODataVersion version in Versions)
            {
                Action withStream = () => this.CreatePropertyContextUri(new ODataStreamReferenceValue(), version);
                withStream.ShouldThrow<ODataException>().WithMessage(Strings.ODataContextUriBuilder_StreamValueMustBePropertiesOfODataResource);
            }
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
            foreach (ODataVersion version in Versions)
            {
                const string name = "IAmName";
                ODataValue value = name.ToODataValue();
                this.CreateIndividualPropertyContextUri(value, "Cities(9)/Name", version).OriginalString.Should().Be(BuildExpectedContextUri("#Cities(9)/Name"));
            }
        }

        [Fact]
        public void ShouldWriteIndividualPropertyContextUriForCollectionType()
        {
            foreach (ODataVersion version in Versions)
            {
                ODataValue value = new ODataCollectionValue();
                this.CreateIndividualPropertyContextUri(value, "Cities(9)/Restaurants", version).OriginalString.Should().Be(BuildExpectedContextUri("#Cities(9)/Restaurants"));
            }
        }
        #endregion individual property context uri

        #region delta context uri
        [Fact]
        public void ShouldWriteDeltaFeedContextUri()
        {
            foreach (ODataVersion version in Versions)
            {
                this.CreateDeltaResponseContextUri(responseCityTypeContextWithoutSerializationInfo, ODataDeltaKind.ResourceSet, version).OriginalString.Should().Be(BuildExpectedContextUri("#Cities/$delta"));
                this.CreateDeltaResponseContextUri(responseCapitolCityTypeContextWithoutSerializationInfo, ODataDeltaKind.ResourceSet, version).OriginalString.Should().Be(BuildExpectedContextUri("#Cities/TestModel.CapitolCity/$delta"));
            }
        }

        [Fact]
        public void ShouldWriteDeltaEntryContextUri()
        {
            foreach (ODataVersion version in Versions)
            {
                this.CreateDeltaResponseContextUri(responseCityTypeContextWithoutSerializationInfo, ODataDeltaKind.Resource, version).OriginalString.Should().Be(BuildExpectedContextUri("#Cities/$entity"));
                this.CreateDeltaResponseContextUri(responseCapitolCityTypeContextWithoutSerializationInfo, ODataDeltaKind.Resource, version).OriginalString.Should().Be(BuildExpectedContextUri("#Cities/TestModel.CapitolCity/$entity"));
            }
        }

        [Fact]
        public void ShouldWriteDeltaDeletedEntryContextUri()
        {
            foreach (ODataVersion version in Versions)
            {
                this.CreateDeltaResponseContextUri(responseCityTypeContextWithoutSerializationInfo, ODataDeltaKind.DeletedEntry, version).OriginalString.Should().Be(BuildExpectedContextUri("#Cities/$deletedEntity"));
                this.CreateDeltaResponseContextUri(responseCapitolCityTypeContextWithoutSerializationInfo, ODataDeltaKind.DeletedEntry, version).OriginalString.Should().Be(BuildExpectedContextUri("#Cities/$deletedEntity"));
            }
        }

        [Fact]
        public void ShouldWriteDeltaLinkContextUri()
        {
            foreach (ODataVersion version in Versions)
            {
                this.CreateDeltaResponseContextUri(responseCityTypeContextWithoutSerializationInfo, ODataDeltaKind.Link, version).OriginalString.Should().Be(BuildExpectedContextUri("#Cities/$link"));
                this.CreateDeltaResponseContextUri(responseCapitolCityTypeContextWithoutSerializationInfo, ODataDeltaKind.Link, version).OriginalString.Should().Be(BuildExpectedContextUri("#Cities/$link"));
            }
        }

        [Fact]
        public void ShouldWriteDeltaDeletedLinkContextUri()
        {
            foreach (ODataVersion version in Versions)
            {
                this.CreateDeltaResponseContextUri(responseCityTypeContextWithoutSerializationInfo, ODataDeltaKind.DeletedLink, version).OriginalString.Should().Be(BuildExpectedContextUri("#Cities/$deletedLink"));
                this.CreateDeltaResponseContextUri(responseCapitolCityTypeContextWithoutSerializationInfo, ODataDeltaKind.DeletedLink, version).OriginalString.Should().Be(BuildExpectedContextUri("#Cities/$deletedLink"));
            }
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
            foreach (ODataVersion version in Versions)
            {
                var typeContext = ODataResourceTypeContext.Create(serializationInfo, null, null, null, true);
                this.builderWithNoMetadataDocumentUri.BuildContextUri(ODataPayloadKind.ResourceSet, ODataContextUrlInfo.Create(typeContext, version, false)).Should().BeNull();
            }
        }

        [Fact]
        public void EntryContextUriShouldNotBeWrittenIfNotProvided()
        {
            var serializationInfo = new ODataResourceSerializationInfo { NavigationSourceName = "MyContainer.MyCities", NavigationSourceEntityTypeName = "TestModel.MyCity", ExpectedTypeName = "TestModel.MyCity" };
            foreach (ODataVersion version in Versions)
            {
                var typeContext = ODataResourceTypeContext.Create(serializationInfo, null, null, null, true);
                this.builderWithNoMetadataDocumentUri.BuildContextUri(ODataPayloadKind.Resource, ODataContextUrlInfo.Create(typeContext, version, true)).Should().BeNull();
            }
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
            foreach (ODataVersion version in Versions)
            {
                var contextInfo = ODataContextUrlInfo.Create(new ODataProperty().ODataValue, version);
                this.builderWithNoMetadataDocumentUri.BuildContextUri(ODataPayloadKind.Property, contextInfo).Should().BeNull();
            }
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

        private Uri CreatePropertyContextUri(object value, ODataVersion version)
        {
            ODataProperty property = new ODataProperty() { Value = value };
            ODataContextUrlInfo info = ODataContextUrlInfo.Create(property.ODataValue, version);
            Uri contextUrl = this.responseContextUriBuilder.BuildContextUri(ODataPayloadKind.Property, info);
            contextUrl.Should().NotBeNull();
            return contextUrl;
        }

        private Uri CreateIndividualPropertyContextUri(ODataValue value, string resourcePath, ODataVersion version)
        {
            ODataContextUrlInfo info = ODataContextUrlInfo.Create(value, version, new ODataUri() { Path = new ODataUriParser(edmModel, new Uri(ServiceDocumentUriString), new Uri(ServiceDocumentUriString + resourcePath)).ParsePath() });
            Uri contextUrl = this.responseContextUriBuilder.BuildContextUri(ODataPayloadKind.IndividualProperty, info);
            contextUrl.Should().NotBeNull();
            return contextUrl;
        }

        private Uri CreateFeedContextUri(string selectClause, string expandClause, ODataVersion version)
        {
            SelectExpandClause selectExpandClause = new ODataQueryOptionParser(edmModel, this.cityType, this.citySet, new Dictionary<string, string> { { "$expand", expandClause }, { "$select", selectClause } }).ParseSelectAndExpand();
            ODataResourceTypeContext typeContext = ODataResourceTypeContext.Create( /*serializationInfo*/null, this.citySet, this.cityType, this.cityType, true);
            ODataContextUrlInfo info = ODataContextUrlInfo.Create(typeContext, version, false, new ODataUri() { SelectAndExpand = selectExpandClause });
            Uri contextUrl = this.responseContextUriBuilder.BuildContextUri(ODataPayloadKind.ResourceSet, info);
            return contextUrl;
        }
        private Uri CreateFeedContextUri(string applyClauseString, ODataVersion version)
        {
            ApplyClause applyClause = new ODataQueryOptionParser(edmModel, this.cityType, this.citySet, new Dictionary<string, string> { { "$apply", applyClauseString } }).ParseApply();
            ODataResourceTypeContext typeContext = ODataResourceTypeContext.Create( /*serializationInfo*/null, this.citySet, this.cityType, this.cityType, true);
            ODataContextUrlInfo info = ODataContextUrlInfo.Create(typeContext, version, false, new ODataUri() { Apply = applyClause });
            Uri contextUrl = this.responseContextUriBuilder.BuildContextUri(ODataPayloadKind.ResourceSet, info);
            return contextUrl;
        }

        private Uri CreateEntryContextUri(string selectClause, string expandClause, ODataVersion version)
        {
            SelectExpandClause selectExpandClause = new ODataQueryOptionParser(edmModel, this.cityType, this.citySet, new Dictionary<string, string> { { "$expand", expandClause }, { "$select", selectClause } }).ParseSelectAndExpand();
            ODataResourceTypeContext typeContext = ODataResourceTypeContext.Create( /*serializationInfo*/null, this.citySet, this.cityType, this.cityType, true);
            ODataContextUrlInfo info = ODataContextUrlInfo.Create(typeContext, version, true, new ODataUri() { SelectAndExpand = selectExpandClause });
            Uri contextUrl = this.responseContextUriBuilder.BuildContextUri(ODataPayloadKind.ResourceSet, info);
            return contextUrl;
        }

        private Uri CreateFeedContextUri(ODataResourceTypeContext typeContext, ODataVersion version, bool isResponse = true)
        {
            ODataContextUrlInfo info = ODataContextUrlInfo.Create(typeContext, version, false);
            Uri contextUrl = isResponse ?
                this.responseContextUriBuilder.BuildContextUri(ODataPayloadKind.ResourceSet, info) :
                this.requestContextUriBuilder.BuildContextUri(ODataPayloadKind.ResourceSet, info);
            return contextUrl;
        }

        private Uri CreateEntryContextUri(ODataResourceTypeContext typeContext, ODataVersion version, bool isResponse = true)
        {
            ODataContextUrlInfo info = ODataContextUrlInfo.Create(typeContext, version, true);
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

        private Uri CreateDeltaResponseContextUri(ODataResourceTypeContext typeContext, ODataDeltaKind kind, ODataVersion version)
        {
            ODataContextUrlInfo info = ODataContextUrlInfo.Create(typeContext, version, kind);
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

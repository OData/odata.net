//---------------------------------------------------------------------
// <copyright file="ODataContextUriBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.UriParser.Aggregation;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Microsoft.Spatial;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class ODataContextUriBuilderTests
    {
        private const string ServiceDocumentUriString = "http://odata.org/service/";
        private const string MetadataDocumentUriString = "http://odata.org/service/$metadata";
        private static readonly ODataResourceTypeContext ResponseTypeContextWithoutTypeInfo = ODataResourceTypeContext.Create(serializationInfo: null, navigationSource: null, navigationSourceEntityType: null, expectedResourceType: null);
        private static readonly ODataResourceTypeContext RequestTypeContextWithoutTypeInfo = ODataResourceTypeContext.Create(serializationInfo: null, navigationSource: null, navigationSourceEntityType: null, expectedResourceType: null);
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
            action.Throws<ODataException>(Strings.ODataContextUriBuilder_UnsupportedPayloadKind(ODataPayloadKind.MetadataDocument.ToString()));
        }

        [Fact]
        public void WriteServiceDocumentUri()
        {
            Assert.Equal(this.responseContextUriBuilder.BuildContextUri(ODataPayloadKind.ServiceDocument).OriginalString, BuildExpectedContextUri(""));
        }

        [Fact]
        public void ShouldRequireMetadataDocumentUriInResponses()
        {
            Action action = () => ODataContextUriBuilder.Create(null, true);
            action.Throws<ODataException>(Strings.ODataOutputContext_MetadataDocumentUriMissing);
        }

        #region context uri with $select and $expand
        [Fact]
        public void FeedContextUriWithNoSelectString()
        {
            foreach (ODataVersion version in Versions)
            {
                Assert.Equal(this.CreateFeedContextUri(default(string), null, null, null,version).OriginalString, BuildExpectedContextUri("#Cities"));
            }
        }

        [Fact]
        public void FeedContextUriWithEmptySelectString()
        {
            foreach (ODataVersion version in Versions)
            {
                Assert.Equal(this.CreateFeedContextUri(string.Empty, null, null, null, version).OriginalString, BuildExpectedContextUri("#Cities", false, string.Empty));
            }
        }

        [Fact]
        public void FeedContextUriWithSelect()
        {
            // Select single structure property
            string selectClause = "Name";
            string expectClause = "Name";
            foreach (ODataVersion version in Versions)
            {
                Assert.Equal(this.CreateFeedContextUri(selectClause, null, null, null, version).OriginalString, BuildExpectedContextUri("#Cities", false, expectClause));
            }

            // Select single navigation property
            selectClause = "Districts";
            expectClause = "Districts";
            foreach (ODataVersion version in Versions)
            {
                Assert.Equal(this.CreateFeedContextUri(selectClause, null, null, null, version).OriginalString, BuildExpectedContextUri("#Cities", false, expectClause));
            }
        }

        [Fact]
        public void FeedContextUriWithExpandApply()
        {
            string expectClause = "Districts($apply=aggregate($count as Count))";
            foreach (ODataVersion version in Versions)
            {
                Assert.Equal(this.CreateFeedContextUri(null, expectClause, null, null, version).OriginalString, BuildExpectedContextUri("#Cities", false, "Districts(Count)"));
            }
        }

        [Fact]
        public void FeedContextUriWithApplyAggreagate()
        {
            string applyClause = "aggregate(Id with sum as TotalId)";
            foreach (ODataVersion version in Versions)
            {
                Assert.Equal(this.CreateFeedContextUri(null, null, applyClause, null, version).OriginalString, MetadataDocumentUriString + "#Cities(TotalId)");
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
                Assert.Equal(this.CreateFeedContextUri(null, null, applyClause, null, version).OriginalString, MetadataDocumentUriString + "#Cities(DynamicPropertyTotal)");
            }
        }

        [Fact]
        public void FeedContextUriWithApplyGroupBy()
        {
            string applyClause = "groupby((Name, Address/Street))";
            foreach (ODataVersion version in Versions)
            {
                Assert.Equal(this.CreateFeedContextUri(null, null, applyClause, null, version).OriginalString, MetadataDocumentUriString + "#Cities(Name,Address(Street))");
            }
        }

        [Fact]
        public void FeedContextUriWithApplyGroupByDynamicProperty()
        {
            string applyClause = "groupby((Name, DynamicProperty, Address/Street))";
            foreach (ODataVersion version in Versions)
            {
                Assert.Equal(this.CreateFeedContextUri(null, null, applyClause, null, version).OriginalString, MetadataDocumentUriString + "#Cities(Name,DynamicProperty,Address(Street))");
            }
        }

        [Fact]
        public void FeedContextUriWithApplyFilter()
        {
            string applyClause = "filter(Id eq 1)";
            foreach (ODataVersion version in Versions)
            {
                Assert.Equal(this.CreateFeedContextUri(null, null, applyClause, null, version).OriginalString, MetadataDocumentUriString + "#Cities");
            }
        }
        [Fact]
        public void FeedContextUriWithApply()
        {
            string applyClause = "groupby((Name), aggregate(Id with sum as TotalId))";
            foreach (ODataVersion version in Versions)
            {
                Assert.Equal(this.CreateFeedContextUri(null, null, applyClause, null, version).OriginalString, MetadataDocumentUriString + "#Cities(Name,TotalId)");
            }
        }

        [Fact]
        public void FeedContextUriWithApplyAndSelect()
        {
            string applyClause = "groupby((Name), aggregate(Id with sum as TotalId))";
            foreach (ODataVersion version in Versions)
            {
                Assert.Equal(this.CreateFeedContextUri("Name", null, applyClause, null, version).OriginalString, MetadataDocumentUriString + "#Cities(Name)");
            }
        }

        [Fact]
        public void FeedContextUriWithApplyExpand()
        {
            string applyClause = "expand(Districts, filter(true))";
            foreach (ODataVersion version in Versions)
            {
                Assert.Equal(this.CreateFeedContextUri(null, null, applyClause, null, version).OriginalString, MetadataDocumentUriString + "#Cities");
            }
        }


        [Fact]
        public void FeedContextUriWithApplyCompute()
        {
            string computeClause = "compute('Test' as NewColumn)";
            foreach (ODataVersion version in Versions)
            {
                Assert.Equal(this.CreateFeedContextUri(null, null, computeClause, null, version).OriginalString, MetadataDocumentUriString + "#Cities");
            }
        }

        [Fact]
        public void FeedContextUriWithApplyComputeAndSelect()
        {
            string computeClause = "compute('Test' as NewColumn)";
            foreach (ODataVersion version in Versions)
            {
                Assert.Equal(this.CreateFeedContextUri("Id,Name,NewColumn", null, computeClause, null, version).OriginalString, MetadataDocumentUriString + "#Cities(Id,Name,NewColumn)");
            }
        }

        [Fact]
        public void FeedContextUriWithCompute()
        {
            string computeClause = "'Test' as NewColumn";
            foreach (ODataVersion version in Versions)
            {
                Assert.Equal(this.CreateFeedContextUri(null, null, null, computeClause, version).OriginalString, MetadataDocumentUriString + "#Cities");
            }
        }

        [Fact]
        public void FeedContextUriWithComputeAndSelect()
        {
            string computeClause = "'Test' as NewColumn";
            foreach (ODataVersion version in Versions)
            {
                Assert.Equal(this.CreateFeedContextUri("Id,Name,NewColumn", null, null, computeClause, version).OriginalString, MetadataDocumentUriString + "#Cities(Id,Name,NewColumn)");
            }
        }

        [Fact]
        public void FeedContextUriWithWildcardSelectString()
        {
            const string selectClause = "Id,Name,*";
            const string expectClause = "*";
            foreach (ODataVersion version in Versions)
            {
                Assert.Equal(this.CreateFeedContextUri(selectClause, null, null, null, version).OriginalString, BuildExpectedContextUri("#Cities", false, expectClause));
            }
        }

        [Fact]
        public void EntryContextUriWithNoSelectString()
        {
            foreach (ODataVersion version in Versions)
            {
                Assert.Equal(this.CreateEntryContextUri(default(string), null, version).OriginalString, BuildExpectedContextUri("#Cities", true));
            }
        }

        [Fact]
        public void EntryContextUriWithEmptySelectString()
        {
            foreach (ODataVersion version in Versions)
            {
                Assert.Equal(this.CreateEntryContextUri(string.Empty, null, version).OriginalString, BuildExpectedContextUri("#Cities", true, string.Empty));
            }
        }

        [Fact]
        public void EntryContextUriWithSelectString()
        {
            const string selectClause = "Id,Name,*";
            const string expectClause = "*";
            foreach (ODataVersion version in Versions)
            {
                Assert.Equal(this.CreateEntryContextUri(selectClause, null, version).OriginalString, BuildExpectedContextUri("#Cities", true, expectClause));
            }
        }

        [Theory]
        // expand without select, $expand=A
        [InlineData("TestModel.CapitolCity/Districts", "TestModel.CapitolCity/Districts()")]
        // expands without select, $expand=A,B
        [InlineData("TestModel.CapitolCity/CapitolDistrict,TestModel.CapitolCity/Districts", "TestModel.CapitolCity/CapitolDistrict(),TestModel.CapitolCity/Districts()")]
        // expand with nested select, $expand=A($select=B)
        [InlineData("TestModel.CapitolCity/Districts($select=Name)", "TestModel.CapitolCity/Districts(Name)")]

        public void FeedContextUriWithSingleExpandString(string expandClause, string expectedClause)
        {
            foreach (ODataVersion version in Versions)
            {
                Assert.Equal(this.CreateFeedContextUri("", expandClause, null, null, version).OriginalString, BuildExpectedContextUri("#Cities", false, expectedClause));
            }
        }

        [Theory]
        // $select=A&$expand=B
        [InlineData( "Name", "Districts", "Name,Districts()")]
        // $select=A&$expand=A
        [InlineData( "Districts", "Districts", "Districts,Districts()")]
        // $select=A,B,C&$expand=A
        [InlineData( "Name,Districts,Size", "Districts", "Name,Districts,Size,Districts()")]
        // $select=A&$expand=A,B
        [InlineData( "Districts", "Districts,TestModel.CapitolCity/CapitolDistrict", "Districts,Districts(),TestModel.CapitolCity/CapitolDistrict()")]
        // $select=A,B&$expand=B($select=C)
        [InlineData( "Name,Districts", "Districts($select=Name)", "Name,Districts,Districts(Name)")]
        public void FeedContextUriWithSelectAndExpandString(string selectClause, string expandClause, string expectedClause)
        {
            foreach (ODataVersion version in Versions)
            {
                string uriString = this.CreateFeedContextUri(selectClause, expandClause, null, null, version).OriginalString;
                Assert.Equal(uriString, BuildExpectedContextUri("#Cities", false, expectedClause));
            }
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
                Assert.Equal(this.CreateEntryContextUri(selectClause, expandClause, version).OriginalString, BuildExpectedContextUri("#Cities", true, expectedClause));

                // With $select in same level, $select=A&$expand=A($select=B,C)
                selectClause = "Districts";
                expandClause = "Districts($select=Name,Zip)";
                expectedClause = "Districts,Districts(Name,Zip)";
                Assert.Equal(this.CreateEntryContextUri(selectClause, expandClause, version).OriginalString, BuildExpectedContextUri("#Cities", true, expectedClause));
            }
        }

        [Theory]
        [InlineData(ODataVersion.V4)]
        [InlineData(ODataVersion.V401)]
        public void EntryContextUriWithExpandNestedExpandString(ODataVersion version)
        {
            // Without inner $select, $expand=A($expand=B($expand=C))
            string expandClause = "Districts($expand=City($expand=Districts))";
            string expectedClause = "Districts(City(Districts()))";
            string urlString = this.CreateEntryContextUri(null, expandClause, version).OriginalString;
            Assert.Equal(urlString, BuildExpectedContextUri("#Cities", true, expectedClause));

            // With inner $select, $expand=A($expand=B($select=C))
            expandClause = "Districts($expand=City($select=Districts))";
            expectedClause = "Districts(City(Districts))";
            Assert.Equal(this.CreateEntryContextUri(null, expandClause, version).OriginalString, BuildExpectedContextUri("#Cities", true, expectedClause));

            // With inner $expand, $expand=A($expand=C($select=D)))
            expandClause = "Districts($expand=City($select=Districts))";
            expectedClause = "Districts(City(Districts))";
            Assert.Equal(this.CreateEntryContextUri(null, expandClause, version).OriginalString, BuildExpectedContextUri("#Cities", true, expectedClause));

            // With inner $select and $expand, $expand=A($select=B;$expand=C($select=D)))
            expandClause = "Districts($select=Name;$expand=City($select=Districts))";
            expectedClause = "Districts(Name,City(Districts))";
            Assert.Equal(this.CreateEntryContextUri(null, expandClause, version).OriginalString, BuildExpectedContextUri("#Cities", true, expectedClause));
        }

        [Theory]
        [InlineData(ODataVersion.V4)]
        [InlineData(ODataVersion.V401)]
        public void FeedContextUriWithMixedSelectAndExpandString(ODataVersion version)
        {
            const string selectClause = "Size,Name";
            const string expandClause = "Districts($select=Zip,City;$expand=City($expand=Districts;$select=Name))";
            const string expectedClause = "Size,Name,Districts(Zip,City,City(Name,Districts()))";
            Assert.Equal(this.CreateFeedContextUri(selectClause, expandClause, null, null, version).OriginalString, BuildExpectedContextUri("#Cities", false, expectedClause));
        }
        #endregion context uri with $select and $expand

        #region feed context uri
        [Fact]
        public void ShouldWriteFeedContextUriWithoutTypecast()
        {
            foreach (ODataVersion version in Versions)
            {
                Assert.Equal(this.CreateFeedContextUri(this.responseCityTypeContextWithoutSerializationInfo, version).OriginalString, BuildExpectedContextUri("#Cities"));
            }
        }

        [Fact]
        public void ShouldWriteFeedContextUriWithTypecast()
        {
            foreach (ODataVersion version in Versions)
            {
                Assert.Equal(this.CreateFeedContextUri(this.responseCapitolCityTypeContextWithoutSerializationInfo, version).OriginalString, BuildExpectedContextUri("#Cities/TestModel.CapitolCity"));
            }
        }

        [Fact]
        public void ShouldThrowIfEntitySetAndTypeInfoIsMissingWithoutSerializationInfoOnFeedResponse()
        {
            foreach (ODataVersion version in Versions)
            {
                Action test = () => this.CreateFeedContextUri(ResponseTypeContextWithoutTypeInfo, version);
                test.Throws<ODataException>(Strings.ODataContextUriBuilder_NavigationSourceOrTypeNameMissingForResourceOrResourceSet);
            }
        }

        [Fact]
        public void ShouldNotWriteContextUriIfEntitySetIsMissingOnFeedRequest()
        {
            foreach (ODataVersion version in Versions)
            {
                Assert.Null(this.CreateFeedContextUri(RequestTypeContextWithoutTypeInfo, version, false));
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
                expectedResourceType: null),
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
                Assert.Equal(this.CreateEntryContextUri(this.responseCityTypeContextWithoutSerializationInfo, version).OriginalString, BuildExpectedContextUri("#Cities", true));
            }
        }

        [Fact]
        public void ShouldWriteEntryContextUriWithTypecast()
        {
            foreach (ODataVersion version in Versions)
            {
                Assert.Equal(this.CreateEntryContextUri(this.responseCapitolCityTypeContextWithoutSerializationInfo, version).OriginalString, BuildExpectedContextUri("#Cities/TestModel.CapitolCity", true));
            }
        }

        [Fact]
        public void ShouldThrowIfEntitySetAndTypeInfoIsMissingOnEntryResponse()
        {
            foreach (ODataVersion version in Versions)
            {
                Action test = () => this.CreateEntryContextUri(ResponseTypeContextWithoutTypeInfo, version);
                test.Throws<ODataException>(Strings.ODataContextUriBuilder_NavigationSourceOrTypeNameMissingForResourceOrResourceSet);
            }
        }

        [Fact]
        public void ShouldThrowIfEntitySetIsMissingOnEntryRequest()
        {
            foreach (ODataVersion version in Versions)
            {
                Assert.Null(this.CreateEntryContextUri(RequestTypeContextWithoutTypeInfo, version, false));
            }
        }

        [Fact]
        public void ShouldNotIncludeFragmentItemSelectorOnSingleton()
        {
            foreach (ODataVersion version in Versions)
            {
                var singletonTypeContextWithModel = ODataResourceTypeContext.Create( /*serializationInfo*/null, this.singletonCity, this.cityType, this.cityType);
                Assert.Equal(this.CreateEntryContextUri(singletonTypeContextWithModel, version).OriginalString, BuildExpectedContextUri("#SingletonCity", false));
            }
        }

        [Fact]
        public void ShouldNotIncludeEntityOnSingletonWithoutModel()
        {
            ODataResourceSerializationInfo serializationInfo = new ODataResourceSerializationInfo() { ExpectedTypeName = "People", NavigationSourceEntityTypeName = "People", NavigationSourceName = "Boss", NavigationSourceKind = EdmNavigationSourceKind.Singleton, };
            var requestSingletonTypeContextWithoutModel = ODataResourceTypeContext.Create(serializationInfo, /*navigationSource*/null, /*navigationSourceEntityType*/null, /*expectedEntityType*/null);
            foreach (ODataVersion version in Versions)
            {
                Assert.Equal(this.CreateEntryContextUri(requestSingletonTypeContextWithoutModel, version).OriginalString, BuildExpectedContextUri("#Boss", false));
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
            Assert.Equal(uri.OriginalString, BuildExpectedContextUri("#Districts/$entity", false));
        }

        #endregion entry context uri

        #region link context uri
        [Fact]
        public void ShouldWriteLinkContextUriWithoutSerializationInfo()
        {
            Assert.Equal(this.CreateEntityReferenceLinkContextUri().OriginalString, BuildExpectedContextUri("#$ref"));
        }

        [Fact]
        public void ShouldWriteLinkContextUriWithSerializationInfo()
        {
            Assert.Equal(this.CreateEntityReferenceLinkContextUri().OriginalString, BuildExpectedContextUri(ODataConstants.SingleEntityReferencesContextUrlSegment));
        }
        #endregion link context uri

        #region links context uri
        [Fact]
        public void ShouldWriteLinksContextUriWithoutSerializationInfo()
        {
            Assert.Equal(this.CreateEntityReferenceLinksContextUri().OriginalString, BuildExpectedContextUri(ODataConstants.CollectionOfEntityReferencesContextUrlSegment));
        }

        [Fact]
        public void ShouldWriteLinksContextUriWithSerializationInfo()
        {
            Assert.Equal(this.CreateEntityReferenceLinksContextUri().OriginalString, BuildExpectedContextUri(ODataConstants.CollectionOfEntityReferencesContextUrlSegment));
        }
        #endregion links context uri

        #region value context uri

        [Fact]
        public void BuildPropertyContextUriForIntegerPropertyValue()
        {
            foreach (ODataVersion version in Versions)
            {
                var contextUri = this.CreatePropertyContextUri(1, version);
                Assert.Equal(contextUri.OriginalString, BuildExpectedContextUri("#Edm.Int32"));
            }
        }

        [Fact]
        public void BuildPropertyContextUriForSpatialPropertyValue()
        {
            foreach (ODataVersion version in Versions)
            {
                var contextUri = this.CreatePropertyContextUri(GeometryPoint.Create(1, 2), version);
                Assert.Equal(contextUri.OriginalString, BuildExpectedContextUri("#Edm.GeometryPoint"));
            }
        }

        [Fact]
        public void BuildResourceContextUriForComplexResource()
        {
            var typeContext = ODataResourceTypeContext.Create( /*serializationInfo*/null,
                null, null, this.addressType);
            ODataResource value = new ODataResource { TypeName = "TestModel.Address" };
            foreach (ODataVersion version in Versions)
            {
                var contextUri = this.CreateEntryContextUri(typeContext, version, true);
                Assert.Equal(contextUri.OriginalString, BuildExpectedContextUri("#TestModel.Address"));
            }
        }

        [Fact]
        public void BuildPropertyContextUriForCollectionPropertyValue()
        {
            foreach (ODataVersion version in Versions)
            {
                ODataCollectionValue value = new ODataCollectionValue { TypeName = "FQNS.FakeType" };
                var contextUri = this.CreatePropertyContextUri(value, version);
                Assert.Equal(contextUri.OriginalString, BuildExpectedContextUri("#FQNS.FakeType"));
            }
        }

        [Fact]
        public void BuildResourceContextUriForComplexWithNullAnnotation()
        {
            var typeContext = ODataResourceTypeContext.Create( /*serializationInfo*/null,
                null, null, this.addressType);
            ODataResource value = new ODataResource { TypeName = "TestModel.Address" };
            value.TypeAnnotation = new ODataTypeAnnotation();
            foreach (ODataVersion version in Versions)
            {
                var contextUri = this.CreateEntryContextUri(typeContext, version, true);
                Assert.Equal(contextUri.OriginalString, BuildExpectedContextUri("#TestModel.Address"));
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
                Assert.Equal(contextUri.OriginalString, BuildExpectedContextUri("#FQNS.FakeType"));
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
                Assert.Equal(contextUri.OriginalString, BuildExpectedContextUri("#Edm.Int32"));
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
                Assert.Equal(contextUri.OriginalString, BuildExpectedContextUri("#FQNS.FromAnnotation"));
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
                Assert.Equal(contextUri.OriginalString, BuildExpectedContextUri("#FQNS.FromAnnotation"));
            }
        }

        [Fact]
        public void BuildPropertyContextUriForCollectionPropertyValueWithNoNameShouldFail()
        {
            foreach (ODataVersion version in Versions)
            {
                Action withStream = () => this.CreatePropertyContextUri(new ODataCollectionValue(), version);
                withStream.Throws<ODataException>(Strings.ODataContextUriBuilder_TypeNameMissingForProperty);
            }
        }

        [Fact]
        public void BuildPropertyContextUriForStreamValueShouldFail()
        {
            foreach (ODataVersion version in Versions)
            {
                Action withStream = () => this.CreatePropertyContextUri(new ODataStreamReferenceValue(), version);
                withStream.Throws<ODataException>(Strings.ODataContextUriBuilder_StreamValueMustBePropertiesOfODataResource);
            }
        }
        #endregion value context uri

        #region collection context uri
        [Fact]
        public void ShouldWriteCollectionContextUri()
        {
            Assert.Equal(this.CreateCollectionContextUri(null, EdmCoreModel.Instance.GetString(isNullable: false)).OriginalString, BuildExpectedContextUri("#Collection(Edm.String)"));
        }

        [Fact]
        public void CollectionSerializationInfoShouldOverrideEdmMetadata()
        {
            var collectionStartSerializationInfo1 = new ODataCollectionStartSerializationInfo { CollectionTypeName = "Collection(Edm.Guid)" };
            Assert.Equal(this.CreateCollectionContextUri(collectionStartSerializationInfo1, EdmCoreModel.Instance.GetString(isNullable: false)).OriginalString, BuildExpectedContextUri("#Collection(Edm.Guid)"));
        }

        [Fact]
        public void ShouldThrowIfTypeNameIsMissingOnCollectionResponse()
        {
            Action action = () => Assert.Equal(this.CreateCollectionContextUri(serializationInfo: null, itemTypeReference: null).OriginalString, BuildExpectedContextUri("#Collection(Edm.Guid)"));
            action.Throws<ODataException>(Strings.ODataContextUriBuilder_TypeNameMissingForTopLevelCollection);
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
                Assert.Equal(this.CreateIndividualPropertyContextUri(value, "Cities(9)/Name", version).OriginalString, BuildExpectedContextUri("#Cities(9)/Name"));
            }
        }

        [Fact]
        public void ShouldWriteIndividualPropertyContextUriForCollectionType()
        {
            foreach (ODataVersion version in Versions)
            {
                ODataValue value = new ODataCollectionValue();
                Assert.Equal(this.CreateIndividualPropertyContextUri(value, "Cities(9)/Restaurants", version).OriginalString, BuildExpectedContextUri("#Cities(9)/Restaurants"));
            }
        }
        #endregion individual property context uri

        #region delta context uri
        [Fact]
        public void ShouldWriteDeltaFeedContextUri()
        {
            foreach (ODataVersion version in Versions)
            {
                Assert.Equal(this.CreateDeltaResponseContextUri(responseCityTypeContextWithoutSerializationInfo, ODataDeltaKind.ResourceSet, version).OriginalString, BuildExpectedContextUri("#Cities/$delta"));
                Assert.Equal(this.CreateDeltaResponseContextUri(responseCapitolCityTypeContextWithoutSerializationInfo, ODataDeltaKind.ResourceSet, version).OriginalString, BuildExpectedContextUri("#Cities/TestModel.CapitolCity/$delta"));
            }
        }

        [Fact]
        public void ShouldWriteDeltaEntryContextUri()
        {
            foreach (ODataVersion version in Versions)
            {
                Assert.Equal(this.CreateDeltaResponseContextUri(responseCityTypeContextWithoutSerializationInfo, ODataDeltaKind.Resource, version).OriginalString, BuildExpectedContextUri("#Cities/$entity"));
                Assert.Equal(this.CreateDeltaResponseContextUri(responseCapitolCityTypeContextWithoutSerializationInfo, ODataDeltaKind.Resource, version).OriginalString, BuildExpectedContextUri("#Cities/TestModel.CapitolCity/$entity"));
            }
        }

        [Fact]
        public void ShouldWriteDeltaDeletedEntryContextUri()
        {
            foreach (ODataVersion version in Versions)
            {
                Assert.Equal(this.CreateDeltaResponseContextUri(responseCityTypeContextWithoutSerializationInfo, ODataDeltaKind.DeletedEntry, version).OriginalString, BuildExpectedContextUri("#Cities/$deletedEntity"));
                Assert.Equal(this.CreateDeltaResponseContextUri(responseCapitolCityTypeContextWithoutSerializationInfo, ODataDeltaKind.DeletedEntry, version).OriginalString, BuildExpectedContextUri("#Cities/$deletedEntity"));
            }
        }

        [Fact]
        public void ShouldWriteDeltaLinkContextUri()
        {
            foreach (ODataVersion version in Versions)
            {
                Assert.Equal(this.CreateDeltaResponseContextUri(responseCityTypeContextWithoutSerializationInfo, ODataDeltaKind.Link, version).OriginalString, BuildExpectedContextUri("#Cities/$link"));
                Assert.Equal(this.CreateDeltaResponseContextUri(responseCapitolCityTypeContextWithoutSerializationInfo, ODataDeltaKind.Link, version).OriginalString, BuildExpectedContextUri("#Cities/$link"));
            }
        }

        [Fact]
        public void ShouldWriteDeltaDeletedLinkContextUri()
        {
            foreach (ODataVersion version in Versions)
            {
                Assert.Equal(this.CreateDeltaResponseContextUri(responseCityTypeContextWithoutSerializationInfo, ODataDeltaKind.DeletedLink, version).OriginalString, BuildExpectedContextUri("#Cities/$deletedLink"));
                Assert.Equal(this.CreateDeltaResponseContextUri(responseCapitolCityTypeContextWithoutSerializationInfo, ODataDeltaKind.DeletedLink, version).OriginalString, BuildExpectedContextUri("#Cities/$deletedLink"));
            }
        }
        #endregion delta context uri

        #region NoMetadata
        [Fact]
        public void ShouldNotRequireContextUriInResponsesForNoMetadata()
        {
            Assert.NotNull(ODataContextUriBuilder.Create(null, false));
        }

        [Fact]
        public void FeedContextUriShouldNotBeWrittenIfNotProvided()
        {
            var serializationInfo = new ODataResourceSerializationInfo { NavigationSourceName = "MyContainer.MyCities", NavigationSourceEntityTypeName = "TestModel.MyCity", ExpectedTypeName = "TestModel.MyCity" };
            foreach (ODataVersion version in Versions)
            {
                var typeContext = ODataResourceTypeContext.Create(serializationInfo, null, null, null);
                Assert.Null(this.builderWithNoMetadataDocumentUri.BuildContextUri(ODataPayloadKind.ResourceSet, ODataContextUrlInfo.Create(typeContext, version, false)));
            }
        }

        [Fact]
        public void EntryContextUriShouldNotBeWrittenIfNotProvided()
        {
            var serializationInfo = new ODataResourceSerializationInfo { NavigationSourceName = "MyContainer.MyCities", NavigationSourceEntityTypeName = "TestModel.MyCity", ExpectedTypeName = "TestModel.MyCity" };
            foreach (ODataVersion version in Versions)
            {
                var typeContext = ODataResourceTypeContext.Create(serializationInfo, null, null, null);
                Assert.Null(this.builderWithNoMetadataDocumentUri.BuildContextUri(ODataPayloadKind.Resource, ODataContextUrlInfo.Create(typeContext, version, true)));
            }
        }

        [Fact]
        public void CollectionContextUriShouldNotBeWrittenIfNotProvided()
        {
            var contextInfo = ODataContextUrlInfo.Create(new ODataCollectionStartSerializationInfo { CollectionTypeName = "Collection(Edm.Guid)" }, EdmCoreModel.Instance.GetString(false));
            Assert.Null(this.builderWithNoMetadataDocumentUri.BuildContextUri(ODataPayloadKind.Collection, contextInfo));
        }

        [Fact]
        public void ValueContextUriShouldNotBeWrittenIfNotProvided()
        {
            foreach (ODataVersion version in Versions)
            {
                var contextInfo = ODataContextUrlInfo.Create(new ODataProperty().ODataValue, version);
                Assert.Null(this.builderWithNoMetadataDocumentUri.BuildContextUri(ODataPayloadKind.Property, contextInfo));
            }
        }

        [Fact]
        public void LinkContextUriShouldNotBeWrittenIfNotProvided()
        {
            Assert.Null(this.builderWithNoMetadataDocumentUri.BuildContextUri(ODataPayloadKind.EntityReferenceLink));
        }

        [Fact]
        public void LinksContextUriShouldNotBeWrittenIfNotProvided()
        {
            Assert.Null(this.builderWithNoMetadataDocumentUri.BuildContextUri(ODataPayloadKind.EntityReferenceLinks));
        }

        [Fact]
        public void ServiceDocumentContextUriShouldNotBeWrittenIfNotProvided()
        {
            Assert.Null(this.builderWithNoMetadataDocumentUri.BuildContextUri(ODataPayloadKind.ServiceDocument));
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
            this.responseCityTypeContextWithoutSerializationInfo = ODataResourceTypeContext.Create( /*serializationInfo*/null, this.citySet, this.cityType, this.cityType);
            this.responseCapitolCityTypeContextWithoutSerializationInfo = ODataResourceTypeContext.Create( /*serializationInfo*/null, this.citySet, this.cityType, this.capitolCityType);
        }

        private Uri CreateCollectionContextUri(ODataCollectionStartSerializationInfo serializationInfo, IEdmTypeReference itemTypeReference)
        {
            ODataContextUrlInfo info = ODataContextUrlInfo.Create(serializationInfo, itemTypeReference);
            Uri contextUrl = this.responseContextUriBuilder.BuildContextUri(ODataPayloadKind.Collection, info);
            Assert.NotNull(contextUrl);
            return contextUrl;
        }

        private Uri CreatePropertyContextUri(object value, ODataVersion version)
        {
            ODataProperty property = new ODataProperty() { Value = value };
            ODataContextUrlInfo info = ODataContextUrlInfo.Create(property.ODataValue, version);
            Uri contextUrl = this.responseContextUriBuilder.BuildContextUri(ODataPayloadKind.Property, info);
            Assert.NotNull(contextUrl);
            return contextUrl;
        }

        private Uri CreateIndividualPropertyContextUri(ODataValue value, string resourcePath, ODataVersion version)
        {
            ODataContextUrlInfo info = ODataContextUrlInfo.Create(value, version, new ODataUri() { Path = new ODataUriParser(edmModel, new Uri(ServiceDocumentUriString), new Uri(ServiceDocumentUriString + resourcePath)).ParsePath() });
            Uri contextUrl = this.responseContextUriBuilder.BuildContextUri(ODataPayloadKind.IndividualProperty, info);
            Assert.NotNull(contextUrl);
            return contextUrl;
        }

        private Uri CreateFeedContextUri(string selectClause, string expandClause, string applyClauseString, string computeClauseString, ODataVersion version)
        {
            var parser = new ODataQueryOptionParser(edmModel, this.cityType, this.citySet, new Dictionary<string, string> {
                { "$expand", expandClause },
                { "$select", selectClause },
                { "$apply", applyClauseString },
                { "$compute", computeClauseString } });
            ApplyClause applyClause = null;
            ComputeClause computeClause = null;
            SelectExpandClause selectExpandClause = null;
            if (applyClauseString != null)
            {
                applyClause = parser.ParseApply();
            }

            if (computeClauseString != null)
            {
                computeClause = parser.ParseCompute();
            }

            if (selectClause != null || expandClause != null)
            {
                selectExpandClause = parser.ParseSelectAndExpand();
            }

            
            ODataResourceTypeContext typeContext = ODataResourceTypeContext.Create( /*serializationInfo*/null, this.citySet, this.cityType, this.cityType);
            ODataUri odataUri = new ODataUri() { SelectAndExpand = selectExpandClause, Apply = applyClause, Compute = computeClause };
            ODataContextUrlInfo info = ODataContextUrlInfo.Create(typeContext, version, false,  odataUri);
            Uri contextUrl = this.responseContextUriBuilder.BuildContextUri(ODataPayloadKind.ResourceSet, info);
            return contextUrl;
        }

        private Uri CreateEntryContextUri(string selectClause, string expandClause, ODataVersion version)
        {
            SelectExpandClause selectExpandClause = new ODataQueryOptionParser(edmModel, this.cityType, this.citySet, new Dictionary<string, string> { { "$expand", expandClause }, { "$select", selectClause } }).ParseSelectAndExpand();
            ODataResourceTypeContext typeContext = ODataResourceTypeContext.Create( /*serializationInfo*/null, this.citySet, this.cityType, this.cityType);
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
            Assert.NotNull(contextUrl);
            return contextUrl;
        }

        private Uri CreateEntityReferenceLinksContextUri()
        {
            Uri contextUrl = this.responseContextUriBuilder.BuildContextUri(ODataPayloadKind.EntityReferenceLinks);
            Assert.NotNull(contextUrl);
            return contextUrl;
        }

        private Uri CreateDeltaResponseContextUri(ODataResourceTypeContext typeContext, ODataDeltaKind kind, ODataVersion version)
        {
            ODataContextUrlInfo info = ODataContextUrlInfo.Create(typeContext, version, kind);
            Uri contextUrl = this.responseContextUriBuilder.BuildContextUri(ODataPayloadKind.Delta, info);
            Assert.NotNull(contextUrl);
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

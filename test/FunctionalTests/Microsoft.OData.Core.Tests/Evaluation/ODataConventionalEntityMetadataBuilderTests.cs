//---------------------------------------------------------------------
// <copyright file="ODataConventionalEntityMetadataBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.Evaluation;
using Xunit;

namespace Microsoft.OData.Tests.Evaluation
{
    public class ODataConventionalEntityMetadataBuilderTests
    {
        private static readonly Uri DefaultBaseUri = new Uri("http://odata.org/base/");
        private static readonly Uri MetadataDocumentUri = new Uri(DefaultBaseUri, "$metadata");
        private static readonly TestModel TestModel = TestModel.Initialize();

        private readonly ODataConventionalUriBuilder uriBuilder = new ODataConventionalUriBuilder(DefaultBaseUri,
            ODataUrlKeyDelimiter.Parentheses);
        private readonly TestMetadataContext metadataContext = new TestMetadataContext { GetMetadataDocumentUriFunc = () => MetadataDocumentUri, GetModelFunc = () => TestModel.Model, OperationsBoundToStructuredTypeMustBeContainerQualifiedFunc = type => false };
        private ODataResource productEntry;
        private Dictionary<string, object> sinlgeKeyCollection;
        private Dictionary<string, object> multiKeysCollection;
        private ODataConventionalEntityMetadataBuilder productConventionalEntityMetadataBuilder;
        private ODataResource derivedMultiKeyMultiEtagMleEntry;
        private ODataConventionalEntityMetadataBuilder derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder;
        private ODataResource containedCollectionProductEntry;
        private ODataConventionalEntityMetadataBuilder containedCollectionProductConventionalEntityMetadataBuilder;
        private ODataResource containedProductEntry;
        private ODataConventionalEntityMetadataBuilder containedProductConventionalEntityMetadataBuilder;
        private Dictionary<string, object> containedSinlgeKeyCollection;
        private Dictionary<string, object> containedMultiKeysCollection;

        private const string EntitySetName = "Products";
        private const string EntityTypeName = "TestModel.Product";
        private const string DerivedEntityTypeName = "TestModel.DerivedProduct";
        private const string DerivedMleEntityTypeName = "TestModel.DerivedMleProduct";

        public ODataConventionalEntityMetadataBuilderTests()
        {
            #region Product Entry

            this.productEntry = new ODataResource();
            this.sinlgeKeyCollection = new Dictionary<string, object>() { { "Id", 42 } };
            this.multiKeysCollection = new Dictionary<string, object>() { { "KeyA", "keya" }, { "KeyB", 1 } };

            TestFeedAndEntryTypeContext productTypeContext = new TestFeedAndEntryTypeContext
            {
                NavigationSourceName = EntitySetName,
                NavigationSourceEntityTypeName = EntityTypeName,
                ExpectedResourceTypeName = EntityTypeName,
                IsMediaLinkEntry = false,
                IsFromCollection = false,
                NavigationSourceKind = EdmNavigationSourceKind.EntitySet
            };

            TestEntryMetadataContext productEntryMetadataContext = new TestEntryMetadataContext
            {
                TypeContext = productTypeContext,
                Resource = this.productEntry,
                ETagProperties = new[] { new KeyValuePair<string, object>("Name", "Value") },
                KeyProperties = this.sinlgeKeyCollection,
                ActualResourceTypeName = EntityTypeName,
                SelectedBindableOperations = new IEdmOperation[0],
                SelectedNavigationProperties = new IEdmNavigationProperty[0],
                SelectedStreamProperties = new Dictionary<string, IEdmStructuralProperty>()
            };

            this.productConventionalEntityMetadataBuilder = new ODataConventionalEntityMetadataBuilder(productEntryMetadataContext, this.metadataContext, this.uriBuilder);
            this.productEntry.MetadataBuilder = this.productConventionalEntityMetadataBuilder;

            #endregion Product Entry

            #region Derived, MultiKey, Multi ETag, MLE Entry

            var action = new EdmAction("TestModel", "Action", /*returnType*/ null, /*isBindable*/ true, /*entitySet*/ null);
            var actionImport = new EdmActionImport(TestModel.Container, "Action", action);

            var function = new EdmFunction("TestModel", "Function", /*returnType*/ EdmCoreModel.Instance.GetInt32(true), /*isBindable*/ true, /*entitySet*/ null, false /*isComposable*/);
            var functionImport = new EdmFunctionImport(TestModel.Container, "Function", function);
            this.derivedMultiKeyMultiEtagMleEntry = new ODataResource();
            TestFeedAndEntryTypeContext derivedMultiKeyMultiEtagMleTypeContext = new TestFeedAndEntryTypeContext
            {
                NavigationSourceName = EntitySetName,
                NavigationSourceEntityTypeName = EntityTypeName,
                ExpectedResourceTypeName = DerivedEntityTypeName,
                IsMediaLinkEntry = true,
                IsFromCollection = false
            };
            TestEntryMetadataContext derivedProductMleEntryMetadataContext = new TestEntryMetadataContext
            {
                TypeContext = derivedMultiKeyMultiEtagMleTypeContext,
                Resource = this.derivedMultiKeyMultiEtagMleEntry,
                ETagProperties = new[] { new KeyValuePair<string, object>("ETag1", "ETagValue1"), new KeyValuePair<string, object>("ETag2", "ETagValue2") },
                KeyProperties = this.multiKeysCollection,
                ActualResourceTypeName = DerivedMleEntityTypeName,
                SelectedBindableOperations = new IEdmOperation[]
                {
                    action,
                    function
                },
                SelectedNavigationProperties = TestModel.ProductWithNavPropsType.NavigationProperties(),
                SelectedStreamProperties = new Dictionary<string, IEdmStructuralProperty>
                {
                    {"Photo", new EdmStructuralProperty(TestModel.ProductType, "Photo", EdmCoreModel.Instance.GetStream( /*isNullable*/true))}
                },
            };

            this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder = new ODataConventionalEntityMetadataBuilder(derivedProductMleEntryMetadataContext, this.metadataContext, this.uriBuilder);
            this.derivedMultiKeyMultiEtagMleEntry.MetadataBuilder = this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder;

            #endregion Derived, MultiKey, Multi ETag, MLE Entry

            #region Contained Product Entry

            this.containedCollectionProductEntry = new ODataResource();
            this.containedSinlgeKeyCollection = new Dictionary<string, object>() { { "Id", 43 } };
            this.containedMultiKeysCollection = new Dictionary<string, object>() { { "KeyA", "keya" }, { "KeyB", 2 } };

            TestFeedAndEntryTypeContext containedCollectionProductTypeContext = new TestFeedAndEntryTypeContext
            {
                NavigationSourceName = EntitySetName,
                NavigationSourceEntityTypeName = EntityTypeName,
                ExpectedResourceTypeName = EntityTypeName,
                IsMediaLinkEntry = false,
                NavigationSourceKind = EdmNavigationSourceKind.ContainedEntitySet,
                IsFromCollection = true
            };

            TestEntryMetadataContext containedCollectionProductEntryMetadataContext = new TestEntryMetadataContext
            {
                TypeContext = containedCollectionProductTypeContext,
                Resource = this.containedCollectionProductEntry,
                ETagProperties = new[] { new KeyValuePair<string, object>("Name", "Value") },
                KeyProperties = this.containedSinlgeKeyCollection,
                ActualResourceTypeName = EntityTypeName,
                SelectedBindableOperations = new IEdmOperation[0],
                SelectedNavigationProperties = new IEdmNavigationProperty[0],
                SelectedStreamProperties = new Dictionary<string, IEdmStructuralProperty>()
            };

            this.containedCollectionProductConventionalEntityMetadataBuilder = new ODataConventionalEntityMetadataBuilder(containedCollectionProductEntryMetadataContext, this.metadataContext, this.uriBuilder);
            this.containedCollectionProductEntry.MetadataBuilder = this.containedCollectionProductConventionalEntityMetadataBuilder;
            this.containedCollectionProductEntry.MetadataBuilder.ParentMetadataBuilder = this.productConventionalEntityMetadataBuilder;

            this.containedProductEntry = new ODataResource();
            this.containedSinlgeKeyCollection = new Dictionary<string, object>() { { "Id", 43 } };
            this.containedMultiKeysCollection = new Dictionary<string, object>() { { "KeyA", "keya" }, { "KeyB", 2 } };

            TestFeedAndEntryTypeContext containedProductTypeContext = new TestFeedAndEntryTypeContext
            {
                NavigationSourceName = EntitySetName,
                NavigationSourceEntityTypeName = EntityTypeName,
                ExpectedResourceTypeName = EntityTypeName,
                IsMediaLinkEntry = false,
                NavigationSourceKind = EdmNavigationSourceKind.ContainedEntitySet,
                IsFromCollection = false
            };

            TestEntryMetadataContext containedProductEntryMetadataContext = new TestEntryMetadataContext
            {
                TypeContext = containedProductTypeContext,
                Resource = this.containedCollectionProductEntry,
                ETagProperties = new[] { new KeyValuePair<string, object>("Name", "Value") },
                KeyProperties = this.containedSinlgeKeyCollection,
                ActualResourceTypeName = EntityTypeName,
                SelectedBindableOperations = new IEdmOperation[0],
                SelectedNavigationProperties = new IEdmNavigationProperty[0],
                SelectedStreamProperties = new Dictionary<string, IEdmStructuralProperty>()
            };

            this.containedProductConventionalEntityMetadataBuilder = new ODataConventionalEntityMetadataBuilder(containedProductEntryMetadataContext, this.metadataContext, this.uriBuilder);
            this.containedProductEntry.MetadataBuilder = this.containedProductConventionalEntityMetadataBuilder;
            this.containedProductEntry.MetadataBuilder.ParentMetadataBuilder = this.productConventionalEntityMetadataBuilder;

            #endregion
        }

        #region Argument validation tests
        [Fact]
        public void GetStreamEditLinkShouldValidateArguments()
        {
            ODataEntityMetadataBuilderTestsUtils.GetStreamEditLinkShouldValidateArguments(this.productConventionalEntityMetadataBuilder);
        }

        [Fact]
        public void GetStreamReadLinkShouldValidateArguments()
        {
            ODataEntityMetadataBuilderTestsUtils.GetStreamReadLinkShouldValidateArguments(this.productConventionalEntityMetadataBuilder);
        }

        [Fact]
        public void GetNavigationLinkUriShouldValidateArguments()
        {
            ODataEntityMetadataBuilderTestsUtils.GetNavigationLinkUriShouldValidateArguments(this.productConventionalEntityMetadataBuilder);
        }

        [Fact]
        public void GetAssociationLinkUriShouldValidateArguments()
        {
            ODataEntityMetadataBuilderTestsUtils.GetAssociationLinkUriShouldValidateArguments(this.productConventionalEntityMetadataBuilder);
        }

        [Fact]
        public void GetOperationTargetUriShouldValidateArguments()
        {
            ODataEntityMetadataBuilderTestsUtils.GetOperationTargetUriShouldValidateArguments(this.productConventionalEntityMetadataBuilder);
        }

        [Fact]
        public void GetOperationTitleShouldValidateArguments()
        {
            ODataEntityMetadataBuilderTestsUtils.GetOperationTitleShouldValidateArguments(this.productConventionalEntityMetadataBuilder);
        }
        #endregion Argument validation tests

        #region Tests for GetEditLink()
        [Fact]
        public void GetEditLinkWithSingleKey()
        {
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetEditLink(), new Uri("http://odata.org/base/Products(42)"));
        }

        [Fact]
        public void GetEditLinkWithMultipleKeys()
        {
            Assert.Equal(this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetEditLink(), new Uri("http://odata.org/base/Products(KeyA='keya',KeyB=1)/TestModel.DerivedMleProduct"));
        }

        [Fact]
        public void GetEditLinkFromEntryInsteadOfBuilding()
        {
            var uri = this.SetProductEntryEditLink();
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetEditLink(), uri);
        }

        [Fact]
        public void EditLinkShouldBeNullIfReadLinkIsSetButEditLinkIsNotSet()
        {
            var uri = this.SetProductEntryReadLink();
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetReadLink(), uri);
            Assert.Null(this.productConventionalEntityMetadataBuilder.GetEditLink());
        }

        [Fact]
        public void EditLinkShouldContainTypeSegmentIfInstanceTypeIsMoreDerviedThanSet()
        {
            // Verify that the last segment of the edit link is the expected type segment.
            string[] uriSegments = this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetEditLink().Segments;
            Assert.Equal("TestModel.DerivedMleProduct", uriSegments[uriSegments.Length - 1]);
        }


        [Fact]
        public void GetEditLinkShouldReturnComputedIdWithTypeCastForDerivedEntity()
        {
            Uri id = this.derivedMultiKeyMultiEtagMleEntry.Id;
            Uri expectedEditLink = this.uriBuilder.AppendTypeSegment(id, DerivedMleEntityTypeName);
            Assert.Equal(this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetEditLink(), expectedEditLink);
        }

        [Fact]
        public void GetEditLinkShouldReturnNonComputedIdUriWithTypeCastForDerivedEntityWhenNonComputedIdIsSet()
        {
            var id = new Uri("http://anotherodata.org/serviceBase/SomeType('xyz')");
            this.derivedMultiKeyMultiEtagMleEntry.Id = id;
            Uri expectedEditLink = this.uriBuilder.AppendTypeSegment(id, DerivedMleEntityTypeName);
            Assert.Equal(this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetEditLink(), expectedEditLink);
        }

        [Fact]
        public void GetEditLinkShouldReturnNonComputedIdUriWhenNonComputedIdIsSet()
        {
            var id = new Uri("http://anotherodata.org/serviceBase/SomeType('xyz')");
            this.productEntry.Id = id;
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetEditLink(), id);
        }

        [Fact]
        public void EditLinkShouldNotContainTypeSegmentIfInstanceTypeMatchesSetType()
        {
            Assert.DoesNotContain("TestModel.Product", this.productConventionalEntityMetadataBuilder.GetEditLink().AbsolutePath);
        }

        [Fact]
        public void GetEditLinkWithSingleKeyWhenKeyisInt64()
        {
            this.SetSingleKeyPropertie("Id", 1L);
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetEditLink(), new Uri("http://odata.org/base/Products(1)"));
        }

        [Fact]
        public void GetEditLinkWithSingleKeyWhenKeyisFloat()
        {
            this.SetSingleKeyPropertie("Id", -1.0f);
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetEditLink(), new Uri("http://odata.org/base/Products(-1)"));
        }

        [Fact]
        public void GetEditLinkWithSingleKeyWhenKeyisDouble()
        {
            this.SetSingleKeyPropertie("Id", 1.0d);
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetEditLink(), new Uri("http://odata.org/base/Products(1.0)"));
        }

        [Fact]
        public void GetEditLinkWithSingleKeyWhenKeyisDecimal()
        {
            this.SetSingleKeyPropertie("Id", 0.0m);
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetEditLink(), new Uri("http://odata.org/base/Products(0.0)"));
        }

        [Fact]
        public void GetEditLinkWithMultiKeysWhenKeyisLFDM()
        {
            var entitySetInstanceId = SetMultiKeyProperties();
            Assert.Equal(this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetEditLink(), new Uri(string.Format(@"http://odata.org/base/Products({0})/TestModel.DerivedMleProduct", entitySetInstanceId)));
        }

        [Fact]
        public void EditlinkShouldBeNullWhenEntryIsATransientEntry()
        {
            this.productEntry.IsTransient = true;
            Assert.Null(this.productConventionalEntityMetadataBuilder.GetEditLink());
        }

        [Fact]
        public void EditlinkShouldBeNonComputedEditLinkWhenEntryIsNotATransientEntryAndHaveNonComputedEditLink()
        {
            this.productEntry.IsTransient = false;
            var nonComputedEditLinkUri = new Uri("http://anotherodata.org/serviceBaseEdit/SomeType('xyz')");
            this.productEntry.EditLink = nonComputedEditLinkUri;
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetEditLink(), nonComputedEditLinkUri);
        }

        [Fact]
        public void EditlinkShouldBeNonComputedEditLinkWhenEntryIsATransientEntryAndHaveNonComputedEditLink()
        {
            this.productEntry.IsTransient = true;
            this.productEntry.EditLink = new Uri("http://anotherodata.org/serviceBaseEdit/SomeType('xyz')");
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetEditLink(), this.productEntry.EditLink);
        }

        #endregion Tests for GetEditLink()

        #region Tests for GetId()
        [Fact]
        public void GetIdForContainedCollectionProperty()
        {
            Uri id = this.containedCollectionProductConventionalEntityMetadataBuilder.GetId();
            Assert.Contains("Products(42)/Products(43)", id.ToString());
        }

        [Fact]
        public void GetIdForContainedIndividualProperty()
        {
            Uri id = this.containedProductConventionalEntityMetadataBuilder.GetId();
            Assert.EndsWith("Products(42)/Products", id.ToString());
        }

        [Fact]
        public void GetIdShouldBeGeneratedIdWhenEntryDoesNotContainIdEditOrReadLink()
        {
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetId(), new Uri("http://odata.org/base/Products(42)"));
        }

        [Fact]
        public void GetIdShouldBeEntryIdWhenEntryContainsIdOnly()
        {
            var uri = new Uri("http://anotherodata.org/serviceBase/SomeType('xyz')");
            this.productEntry.Id = uri;
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetId(), uri);
        }

        [Fact]
        public void GetIdShouldHonorUserOverridingId()
        {
            Assert.Equal(new Uri("http://odata.org/base/Products(42)"), this.productEntry.Id);
            Uri uri = new Uri("http://overwrite");
            this.productEntry.Id = uri;
            Assert.Equal(this.productEntry.Id, uri);
        }

        [Fact]
        public void GetIdShouldBeEntryIdWhenEntryContainsIdEditAndReadLink()
        {
            var id = new Uri("http://anotherodata.org/serviceBase/SomeType('xyz')");
            this.productEntry.Id = id;
            var readLinkUri = new Uri("http://anotherodata.org/serviceBaseRead/SomeType('xyz')");
            this.productEntry.ReadLink = readLinkUri;
            var editLinkUri = new Uri("http://anotherodata.org/serviceBaseEdit/SomeType('xyz')");
            this.productEntry.EditLink = editLinkUri;

            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetId(), id);
        }

        [Fact]
        public void GetIdShouldBeCanonicalUrlWhenEntryContainsReadLinkOnly()
        {
            var canonicalUrl = this.productConventionalEntityMetadataBuilder.GetId();
            var uri = new Uri("http://anotherodata.org/serviceBase/SomeType('xyz')");
            this.productEntry.ReadLink = uri;
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetId(), canonicalUrl);
        }

        [Fact]
        public void GetIdShouldBeCanonicalUrlWhenEntryContainsEditLinkOnly()
        {
            var canonicalUrl = this.productConventionalEntityMetadataBuilder.GetId();
            var uri = new Uri("http://anotherodata.org/serviceBase/SomeType('xyz')");
            this.productEntry.EditLink = uri;
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetId(), canonicalUrl);
        }

        [Fact]
        public void GetIdShouldBeCanonicalUrlWhenEntryContainsBothReadLinkAndEditLink()
        {
            var canonicalUrl = this.productConventionalEntityMetadataBuilder.GetId();
            var readLinkUri = new Uri("http://anotherodata.org/serviceBaseRead/SomeType('xyz')");
            this.productEntry.ReadLink = readLinkUri;
            var editLinkUri = new Uri("http://anotherodata.org/serviceBaseEdit/SomeType('xyz')");
            this.productEntry.EditLink = editLinkUri;
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetId(), canonicalUrl);
        }

        [Fact]
        public void TryGetIdForSerializationShouldBeNullWhenEntryIsTransient()
        {
            this.productEntry.IsTransient = true;
            Uri id;
            Assert.True(productConventionalEntityMetadataBuilder.TryGetIdForSerialization(out id));
            Assert.Null(id);
        }

        [Fact]
        public void TryGetIdForSerializationShouldBeCanonicalUrlWhenEntryIsNotTransientAndDoNotHaveNoComputedId()
        {
            this.productEntry.IsTransient = false;
            var canonicalUrl = this.productEntry.Id;
            Uri id;
            Assert.True(productConventionalEntityMetadataBuilder.TryGetIdForSerialization(out id));
            Assert.Equal(id, canonicalUrl);
        }

        [Fact]
        public void GetIdShouldBeNullWhenEntryIsTransient()
        {
            this.productEntry.IsTransient = true;
            Assert.Null(productConventionalEntityMetadataBuilder.GetId());
        }

        [Fact]
        public void GetIdWithSingleKeyWhenKeyisInt64AndEntryDoesNotContainIdEditOrReadLink()
        {
            this.SetSingleKeyPropertie("Id", -1L);
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetId(), new Uri(@"http://odata.org/base/Products(-1)"));
        }

        [Fact]
        public void GetIdWithSingleKeyWhenKeyisFloatAndEntryDoesNotContainIdEditOrReadLink()
        {
            this.SetSingleKeyPropertie("Id", -1.0f);
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetId(), new Uri(@"http://odata.org/base/Products(-1)"));
        }

        [Fact]
        public void GetIdWithSingleKeyWhenKeyisDoubleAndEntryDoesNotContainIdEditOrReadLink()
        {
            this.SetSingleKeyPropertie("Id", 1.0d);
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetId(), new Uri(@"http://odata.org/base/Products(1.0)"));
        }

        [Fact]
        public void GetIdWithSingleKeyWhenKeyisDecimalAndEntryDoesNotContainIdEditOrReadLink()
        {
            this.SetSingleKeyPropertie("Id", 0.0m);
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetId(), new Uri(@"http://odata.org/base/Products(0.0)"));
        }

        [Fact]
        public void GetIdWithMultiKeysWhenKeyisLongLFDM()
        {
            var entitySetInstanceId = SetMultiKeyProperties();
            Assert.Equal(this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetId(), new Uri(string.Format(@"http://odata.org/base/Products({0})", entitySetInstanceId)));
        }

        #endregion Tests for GetId()

        #region Tests for GetReadLink()
        [Fact]
        public void GetReadLinkWithSingleKey()
        {
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetReadLink(), new Uri("http://odata.org/base/Products(42)"));
        }

        [Fact]
        public void GetReadLinkWithMultipleKeys()
        {
            Assert.Equal(this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetReadLink(), new Uri("http://odata.org/base/Products(KeyA='keya',KeyB=1)/TestModel.DerivedMleProduct"));
        }

        [Fact]
        public void GetReadLinkFromEntryInsteadOfBuilding()
        {
            var uri = this.SetProductEntryReadLink();
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetReadLink(), uri);
        }

        [Fact]
        public void GetReadLinkWhenEntryHasEditLinkButNotReadLink()
        {
            var uri = this.SetProductEntryEditLink();
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetReadLink(), uri);
        }

        [Fact]
        public void GetReadLinkWhenEntryHasBothEditLinkAndReadLink()
        {
            this.SetProductEntryEditLink();
            var uri = this.SetProductEntryReadLink();
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetReadLink(), uri);
        }

        [Fact]
        public void GetReadLinkWithSingleKeyWhenKeyisInt64()
        {
            this.SetSingleKeyPropertie("Id", 1L);
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetReadLink(), new Uri("http://odata.org/base/Products(1)"));
        }

        [Fact]
        public void GetReadLinkWithSingleKeyWhenKeyisFloat()
        {
            this.SetSingleKeyPropertie("Id", -1.0f);
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetReadLink(), new Uri("http://odata.org/base/Products(-1)"));
        }

        [Fact]
        public void GetReadLinkWithSingleKeyWhenKeyisDouble()
        {
            this.SetSingleKeyPropertie("Id", 1.0d);
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetReadLink(), new Uri("http://odata.org/base/Products(1.0)"));
        }

        [Fact]
        public void GetReadLinkWithSingleKeyWhenKeyisDecimal()
        {
            this.SetSingleKeyPropertie("Id", 0.0m);
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetReadLink(), new Uri("http://odata.org/base/Products(0.0)"));
        }

        [Fact]
        public void GetReadLinkWithMultiKeysWhenKeyisLFDM()
        {
            var entitySetInstanceId = SetMultiKeyProperties();
            Assert.Equal(this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetReadLink(), new Uri(string.Format(@"http://odata.org/base/Products({0})/TestModel.DerivedMleProduct", entitySetInstanceId)));
        }

        [Fact]
        public void ReadLinkShouldBeNullWhenEntryIsATransientEntry()
        {
            this.productEntry.IsTransient = true;
            Assert.Null(this.productConventionalEntityMetadataBuilder.GetReadLink());
        }

        [Fact]
        public void ReadLinkShouldBeNonComputedReadLinkWhenEntryIsNotATransientEntryAndHaveNonComputedReadLink()
        {
            this.productEntry.IsTransient = false;
            var readLinkUri = new Uri("http://anotherodata.org/serviceBaseRead/SomeType('xyz')");
            this.productEntry.ReadLink = readLinkUri;
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetReadLink(), readLinkUri);
        }

        [Fact]
        public void ReadLinkShouldBeComputedReadLinkWhenEntryIsNotATransientEntry()
        {
            this.productEntry.IsTransient = false;
            var computedReadLinkUri = this.productEntry.ReadLink;
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetReadLink(), computedReadLinkUri);
        }

        #endregion Tests for GetReadLink()

        #region Tests for GetETag()
        [Fact]
        public void ETagShouldBeNullForTypeWithoutConcurrencyTokens()
        {
            var testSubject = new ODataConventionalEntityMetadataBuilder(new TestEntryMetadataContext { Resource = new ODataResource(), ETagProperties = new KeyValuePair<string, object>[0] }, this.metadataContext, this.uriBuilder);
            Assert.Null(testSubject.GetETag());
        }

        [Fact]
        public void EtagShouldBeUriEscaped()
        {
            // if this fails System.Uri has changed its behavior and we may need to adjust how we encode our strings for JsonLight
            // .net 45 changed this behavior initially to escape ' to a value, but was changed. below test
            // validates that important uri literal values that OData uses don't change, and that we escape characters when
            // producing the etag for JsonLight
            var escapedStrings = Uri.EscapeUriString(@".:''-");
            Assert.Equal(@".:''-", escapedStrings);

            var testSubject = new ODataConventionalEntityMetadataBuilder(new TestEntryMetadataContext { Resource = new ODataResource(), ETagProperties = new[] { new KeyValuePair<string, object>("ETag", "Value ") } }, this.metadataContext, this.uriBuilder);
            Assert.Equal(@"W/""'Value%20'""", testSubject.GetETag());
        }

        [Fact]
        public void ETagShouldBeCorrectForTypeWithOneConcurrencyToken()
        {
            var testSubject = new ODataConventionalEntityMetadataBuilder(new TestEntryMetadataContext { Resource = new ODataResource(), ETagProperties = new[] { new KeyValuePair<string, object>("ETag", "Value") } }, this.metadataContext, this.uriBuilder);
            Assert.Equal(@"W/""'Value'""", testSubject.GetETag());
        }

        [Fact]
        public void ETagShouldBeCorrectForNullConcurrencyToken()
        {
            var testSubject = new ODataConventionalEntityMetadataBuilder(new TestEntryMetadataContext { Resource = new ODataResource(), ETagProperties = new[] { new KeyValuePair<string, object>("ETag", default(string)) } }, this.metadataContext, this.uriBuilder);
            Assert.Equal(@"W/""null""", testSubject.GetETag());
        }

        [Fact]
        public void ETagShouldBeCorrectForTypeWithManyConcurrencyTokens()
        {
            var values = new[]
            {
                new KeyValuePair<string, object>("ETag1", 1.2345e+45),
                new KeyValuePair<string, object>("ETag2", new byte[] { 1, 2, 3 }),
                new KeyValuePair<string, object>("ETag3", 2.3M)
            };

            var testSubject = new ODataConventionalEntityMetadataBuilder(new TestEntryMetadataContext { Resource = new ODataResource(), ETagProperties = values }, this.metadataContext, this.uriBuilder);
            Assert.Equal(@"W/""1.2345E%2B45,binary'AQID',2.3""", testSubject.GetETag());
        }

        [Fact]
        public void ETagShouldBeCorrectForLDMFWithManyConcurrencyTokens()
        {
            var values = new[]
            {
                new KeyValuePair<string, object>("ETagLong", 1L),
                new KeyValuePair<string, object>("ETagFloat", 1.0F),
                new KeyValuePair<string, object>("ETagDouble", 1.0D),
                new KeyValuePair<string, object>("ETagDecimal", 1.0M)
            };

            var testSubject = new ODataConventionalEntityMetadataBuilder(new TestEntryMetadataContext { Resource = new ODataResource(), ETagProperties = values }, this.metadataContext, this.uriBuilder);
            Assert.Equal(@"W/""1,1,1.0,1.0""", testSubject.GetETag());
        }
        #endregion Tests for GetETag()

        #region Tests for GetStreamEditLink()
        [Fact]
        public void GetStreamEditLinkForDefaultStream()
        {
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetStreamEditLink(null), new Uri("http://odata.org/base/Products(42)/$value"));
        }

        [Fact]
        public void GetStreamEditLinkForDefaultStreamWhenEntryHasEditLink()
        {
            Uri uri = this.SetProductEntryEditLink();
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetStreamEditLink(null), new Uri(uri.AbsoluteUri + "/$value"));
        }

        [Fact]
        public void GetStreamEditLinkForStreamProperty()
        {
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetStreamEditLink("StreamProperty"), new Uri("http://odata.org/base/Products(42)/StreamProperty"));
        }

        [Fact]
        public void GetStreamEditLinkForStreamPropertyWhenEntryHasEditLink()
        {
            Uri uri = this.SetProductEntryEditLink();
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetStreamEditLink("StreamProperty"), new Uri(uri.AbsoluteUri + "/StreamProperty"));
        }

        [Fact]
        public void GetDefaultStreamEditLinkWithMultiKeysWhenKeyisLFDM()
        {
            var entitySetInstanceId = SetMultiKeyProperties();
            Assert.Equal(this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetStreamEditLink(null), new Uri(string.Format(@"http://odata.org/base/Products({0})/TestModel.DerivedMleProduct/$value", entitySetInstanceId)));
        }
        #endregion Tests for GetStreamEditLink()

        #region Tests for GetMediaResource
        [Fact]
        public void ShouldNotComputeMrForNonMle()
        {
            Assert.Null(this.productConventionalEntityMetadataBuilder.GetMediaResource());
        }

        [Fact]
        public void ShouldComputeMrForMle()
        {
            var mr = this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetMediaResource();
            Assert.NotNull(mr);
            Assert.Equal(mr.EditLink, new Uri("http://odata.org/base/Products(KeyA='keya',KeyB=1)/TestModel.DerivedMleProduct/$value"));
            Assert.Equal(mr.ReadLink, new Uri("http://odata.org/base/Products(KeyA='keya',KeyB=1)/TestModel.DerivedMleProduct/$value"));
        }

        [Fact]
        public void ShouldUseNonComputedMediaResourceIfSet()
        {
            this.derivedMultiKeyMultiEtagMleEntry.MediaResource = new ODataStreamReferenceValue();
            Assert.Same(this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetMediaResource(), this.derivedMultiKeyMultiEtagMleEntry.MediaResource);
        }

        [Fact]
        public void ShouldUseUserSetEditLinkAndReadLinkInComputedMr()
        {
            this.derivedMultiKeyMultiEtagMleEntry.EditLink = new Uri("http://someeditlink");
            this.derivedMultiKeyMultiEtagMleEntry.ReadLink = new Uri("http://somereadlink");
            var mr = this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetMediaResource();
            Assert.NotNull(mr);
            Assert.Equal(mr.EditLink, new Uri("http://someeditlink/$value"));
            Assert.Equal(mr.ReadLink, new Uri("http://somereadlink/$value"));
        }
        #endregion Tests for GetMediaResource

        #region Tests for GetStreamReadLink()
        [Fact]
        public void GetStreamReadLinkForDefaultStream()
        {
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetStreamReadLink(null), new Uri("http://odata.org/base/Products(42)/$value"));
        }

        [Fact]
        public void GetStreamReadLinkForDefaultStreamWhenEntryHasReadLinkAndNotEditLink()
        {
            Uri uri = this.SetProductEntryReadLink();
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetStreamReadLink(null), new Uri(uri.AbsoluteUri + "/$value"));
        }

        [Fact]
        public void GetStreamReadLinkForDefaultStreamWhenEntryHasEditLinkAndNotReadLink()
        {
            Uri uri = this.SetProductEntryEditLink();
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetStreamReadLink(null), new Uri(uri.AbsoluteUri + "/$value"));
        }

        [Fact]
        public void GetStreamReadLinkForDefaultStreamWhenEntryHasBothEditLinkAndReadLink()
        {
            this.SetProductEntryEditLink();
            Uri uri = this.SetProductEntryReadLink();
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetStreamReadLink(null), new Uri(uri.AbsoluteUri + "/$value"));
        }

        [Fact]
        public void GetStreamReadLinkForStreamProperty()
        {
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetStreamReadLink("StreamProperty"), new Uri("http://odata.org/base/Products(42)/StreamProperty"));
        }

        [Fact]
        public void GetStreamReadLinkForStreamPropertyWhenEntryHasReadLinkAndNotEditLink()
        {
            Uri uri = this.SetProductEntryReadLink();
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetStreamReadLink("StreamProperty"), new Uri(uri.AbsoluteUri + "/StreamProperty"));
        }

        [Fact]
        public void GetStreamReadLinkForStreamPropertyWhenEntryHasEditLinkAndNotReadLink()
        {
            Uri uri = this.SetProductEntryEditLink();
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetStreamReadLink("StreamProperty"), new Uri(uri.AbsoluteUri + "/StreamProperty"));
        }

        [Fact]
        public void GetStreamReadLinkForStreamPropertyWhenEntryHasBothReadLinkAndEditLink()
        {
            this.SetProductEntryEditLink();
            Uri uri = this.SetProductEntryReadLink();
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetStreamReadLink("StreamProperty"), new Uri(uri.AbsoluteUri + "/StreamProperty"));
        }

        [Fact]
        public void GetDefaultStreamReadLinkWhenKeyisLFDM()
        {
            var entitySetInstanceId = SetMultiKeyProperties();
            Assert.Equal(this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetStreamReadLink(null), new Uri(string.Format(@"http://odata.org/base/Products({0})/TestModel.DerivedMleProduct/$value", entitySetInstanceId)));
        }
        #endregion Tests for GetStreamReadLink()

        #region Tests for GetNavigationLinkUri()
        [Fact]
        public void GetNavigationLinkUri()
        {
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetNavigationLinkUri("NavigationProperty", null, false), new Uri("http://odata.org/base/Products(42)/NavigationProperty"));
            Assert.Null(this.productConventionalEntityMetadataBuilder.GetNavigationLinkUri("NavigationProperty", null, true));
        }

        [Fact]
        public void GetNavigationLinkUriWhenLinkAlreadyHasValue()
        {
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetNavigationLinkUri("NavigationProperty", new Uri("http://example.com/override"), false),
                new Uri("http://odata.org/base/Products(42)/NavigationProperty"));
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetNavigationLinkUri("NavigationProperty", new Uri("http://example.com/override"), true),
                new Uri("http://example.com/override"));
        }

        [Fact]
        public void GetNavigationLinkUriWhenEntryHasEditLink()
        {
            Uri uri = this.SetProductEntryEditLink();
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetNavigationLinkUri("NavigationProperty", null, false),
                new Uri(uri.AbsoluteUri + "/NavigationProperty"));
        }

        [Fact]
        public void GetNavigationLinkUriWhenEntryHasReadLink()
        {
            Uri uri = this.SetProductEntryReadLink();
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetNavigationLinkUri("NavigationProperty", null, false),
                new Uri(uri.AbsoluteUri + "/NavigationProperty"));
        }

        [Fact]
        public void GetNavigationLinkUriShouldFollowReadLinkWhenEntryHasBothReadLinkAndEditLink()
        {
            Uri uri = this.SetProductEntryReadLink();
            this.SetProductEntryEditLink();
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetNavigationLinkUri("NavigationProperty", null, false),
                new Uri(uri.AbsoluteUri + "/NavigationProperty"));
        }

        [Fact]
        public void GetNavigationLinkUriWhenKeyisLFDM()
        {
            var entitySetInstanceId = SetMultiKeyProperties();
            Assert.Equal(this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetNavigationLinkUri("NavigationProperty", null, false),
                new Uri(string.Format(@"http://odata.org/base/Products({0})/TestModel.DerivedMleProduct/NavigationProperty", entitySetInstanceId)));
        }
        #endregion Tests for GetNavigationLinkUri()

        #region Tests for GetAssociationLinkUri()
        [Fact]
        public void GetAssociationLinkUri()
        {
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetAssociationLinkUri("NavigationProperty", null, false), new Uri("http://odata.org/base/Products(42)/NavigationProperty/$ref"));
            Assert.Null(this.productConventionalEntityMetadataBuilder.GetAssociationLinkUri("NavigationProperty", null, true));
        }

        [Fact]
        public void GetAssociationLinkUriWhenLinkAlreadyHasValue()
        {
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetAssociationLinkUri("NavigationProperty", new Uri("http://example.com/override"), false), new Uri("http://odata.org/base/Products(42)/NavigationProperty/$ref"));
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetAssociationLinkUri("NavigationProperty", new Uri("http://example.com/override"), true), new Uri("http://example.com/override"));
        }

        [Fact]
        public void GetAssociationLinkUriWhenEntryHasEditLink()
        {
            Uri uri = this.SetProductEntryEditLink();
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetAssociationLinkUri("NavigationProperty", null, false), new Uri(uri.AbsoluteUri + "/NavigationProperty/$ref"));
        }

        [Fact]
        public void GetAssociationLinkUriWhenEntryHasReadLink()
        {
            Uri uri = this.SetProductEntryReadLink();
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetAssociationLinkUri("NavigationProperty", null, false), new Uri(uri.AbsoluteUri + "/NavigationProperty/$ref"));
        }

        [Fact]
        public void GetAssociationLinkUriShouldFollowReadLinkWhenEntryHasBothReadLinkAndEditLink()
        {
            Uri uri = this.SetProductEntryReadLink();
            this.SetProductEntryEditLink();
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetAssociationLinkUri("NavigationProperty", null, false), new Uri(uri.AbsoluteUri + "/NavigationProperty/$ref"));
        }

        [Fact]
        public void GetAssociationLinkUriWhenKeyisLFDM()
        {
            var entitySetInstanceId = SetMultiKeyProperties();
            Assert.Equal(this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetAssociationLinkUri("NavigationProperty", null, false), new Uri(string.Format(@"http://odata.org/base/Products({0})/TestModel.DerivedMleProduct/NavigationProperty/$ref", entitySetInstanceId)));
        }
        #endregion Tests for GetAssociationLinkUri()

        #region Tests for MarkNestedResourceInfoAsProcessed() and GetNextUnprocessedNestedResourceInfo()
        [Fact]
        public void GetNextUnprocessedNestedResourceInfoShouldBeNullIfTypeHasNoNavProps()
        {
            Assert.Null(this.productConventionalEntityMetadataBuilder.GetNextUnprocessedNavigationLink());
        }

        [Fact]
        public void GetNextUnprocessedNestedResourceInfoShouldReturnNavProps()
        {
            var nextNavProp = this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetNextUnprocessedNavigationLink();
            Assert.Equal("RelatedProducts", nextNavProp.NestedResourceInfo.Name);

            nextNavProp = this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetNextUnprocessedNavigationLink();
            Assert.Equal("RelatedDerivedProduct", nextNavProp.NestedResourceInfo.Name);
        }

        [Fact]
        public void GetNextUnprocessedNestedResourceInfoShouldNotReturnNavPropsThatWerePreviouslyMarkedAsProcessed()
        {
            this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.MarkNestedResourceInfoProcessed("RelatedDerivedProduct");

            var nextNavProp = this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetNextUnprocessedNavigationLink();
            Assert.Equal("RelatedProducts", nextNavProp.NestedResourceInfo.Name);

            nextNavProp = this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetNextUnprocessedNavigationLink();
            Assert.Null(nextNavProp);
        }

        [Fact]
        public void GetNextUnprocessedNestedResourceInfoShouldReturnANavPropWithoutUrls()
        {
            // Note: it is up to the reader and writer to later add a metadata builder to navigation links generated this way.
            var nextNavProp = this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetNextUnprocessedNavigationLink();
            Assert.Null(nextNavProp.NestedResourceInfo.Url);
            Assert.Null(nextNavProp.NestedResourceInfo.AssociationLinkUrl);
        }
        #endregion Tests for MarkNestedResourceInfoAsProcessed() and GetNextUnprocessedNestedResourceInfo()

        #region Tests for GetOperationTargetUri()
        [Fact]
        public void GetOperationTargetUri()
        {
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetOperationTargetUri("OperationName", null, null), new Uri("http://odata.org/base/Products(42)/OperationName"));
        }

        [Fact]
        public void GetOperationTargetUriWhenEntryHasEditLink()
        {
            Uri uri = this.SetProductEntryEditLink();
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetOperationTargetUri("OperationName", null, null), new Uri(uri.AbsoluteUri + "/OperationName"));
        }

        [Fact]
        public void GetOperationTargetUriWithInheritance()
        {
            Assert.Equal(this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetOperationTargetUri("OperationName", null, null), new Uri("http://odata.org/base/Products(KeyA='keya',KeyB=1)/TestModel.DerivedMleProduct/OperationName"));
        }

        [Fact]
        public void GetOperationTargetUriWithInheritanceWhenEntryHasEditLink()
        {
            Uri uri = this.SetDerivedProductEntryEditLink();
            Assert.Equal(this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetOperationTargetUri("OperationName", null, null), new Uri(uri.AbsoluteUri + "/OperationName"));
        }

        [Fact]
        public void GetOperationTargetUriWithParameterType()
        {
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetOperationTargetUri("OperationName", null, "p1"), new Uri("http://odata.org/base/Products(42)/OperationName(p1=@p1)"));
        }

        [Fact]
        public void GetOperationTargetUriWithParameterTypeWhenEntryHasEditLink()
        {
            Uri editLink = this.SetProductEntryEditLink();
            Assert.Equal(this.productConventionalEntityMetadataBuilder.GetOperationTargetUri("OperationName", null, "p1"), new Uri(editLink.AbsoluteUri + "/OperationName(p1=@p1)"));
        }

        [Fact]
        public void GetOperationTargetUriWithParameterTypeAndInheritance()
        {
            Assert.Equal(this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetOperationTargetUri("OperationName", null, "p1"), new Uri("http://odata.org/base/Products(KeyA='keya',KeyB=1)/TestModel.DerivedMleProduct/OperationName(p1=@p1)"));
        }

        [Fact]
        public void GetOperationTargetUriWithParameterTypeAndInheritanceWhenEntryHasEditLink()
        {
            Uri editLink = this.SetDerivedProductEntryEditLink();

            // note that the type segment and operation name are appended onto the opaque edit-link, and may result in multiple type segments in the final target link.
            Assert.Equal(this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetOperationTargetUri("OperationName", null, "p1"), new Uri(editLink.AbsoluteUri + "/OperationName(p1=@p1)"));
        }

        [Fact]
        public void GetOperationTargetUriWhenKeyisLFDM()
        {
            var entitySetInstanceId = SetMultiKeyProperties();
            Assert.Equal(this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetOperationTargetUri("OperationName", null, null), new Uri(string.Format(@"http://odata.org/base/Products({0})/TestModel.DerivedMleProduct/OperationName", entitySetInstanceId)));
        }
        #endregion Tests for GetOperationTargetUri()

        #region Tests for GetOperationTitle()
        [Fact]
        public void GetOperationTitle()
        {
            Assert.Equal("OperationName", this.productConventionalEntityMetadataBuilder.GetOperationTitle("OperationName"));
        }
        #endregion Tests for GetOperationTitle()

        #region Tests for GetProperties()
        [Fact]
        public void ProductShouldNotContainComputedNamedStreams()
        {
            Assert.Empty(this.productConventionalEntityMetadataBuilder.GetProperties(/*nonComputedProperties*/null));
        }

        [Fact]
        public void DerivedProductShouldContainComputedNamedStreams()
        {
            var photoProperty = this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetProperties( /*nonComputedProperties*/null).Single();
            Assert.Equal("Photo", photoProperty.Name);
            var photo = (ODataStreamReferenceValue)photoProperty.Value;
            Assert.NotNull(photo);
            Assert.Equal(photo.EditLink, new Uri("http://odata.org/base/Products(KeyA='keya',KeyB=1)/TestModel.DerivedMleProduct/Photo"));
            Assert.Equal(photo.ReadLink, new Uri("http://odata.org/base/Products(KeyA='keya',KeyB=1)/TestModel.DerivedMleProduct/Photo"));
        }

        [Fact]
        public void ComputedNamedStreamsShouldUseUserSetEditLinkAndReadLink()
        {
            this.derivedMultiKeyMultiEtagMleEntry.EditLink = new Uri("http://someeditlink");
            this.derivedMultiKeyMultiEtagMleEntry.ReadLink = new Uri("http://somereadlink");
            var photoProperty = this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetProperties( /*nonComputedProperties*/null).Single();
            Assert.Equal("Photo", photoProperty.Name);
            var photo = (ODataStreamReferenceValue)photoProperty.Value;
            Assert.NotNull(photo);
            Assert.Equal(photo.EditLink, new Uri("http://someeditlink/Photo"));
            Assert.Equal(photo.ReadLink, new Uri("http://somereadlink/Photo"));
        }
        #endregion Tests for GetProperties()

        #region Tests for computed Actions
        [Fact]
        public void ProductShouldNotContainComputedActions()
        {
            Assert.Empty(this.productConventionalEntityMetadataBuilder.GetActions());
        }

        [Fact]
        public void DerivedProductShouldContainComputedActions()
        {
            var action = this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetActions().Single();
            Assert.Equal("TestModel.Action", action.Title);
            Assert.Equal(action.Metadata, new Uri(MetadataDocumentUri, "#TestModel.Action"));
            Assert.Equal(action.Target, new Uri("http://odata.org/base/Products(KeyA='keya',KeyB=1)/TestModel.DerivedMleProduct/TestModel.Action"));
        }

        [Fact]
        public void GetActionsWhenKeyisLFDM()
        {
            var entitySetInstanceId = SetMultiKeyProperties();
            var action = this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetActions().Single();
            Assert.Equal(action.Target, new Uri(string.Format(@"http://odata.org/base/Products({0})/TestModel.DerivedMleProduct/TestModel.Action", entitySetInstanceId)));
        }

        [Fact]
        public void ComputedActionsTargetShouldBeBasedOnUserSetEditLink()
        {
            this.derivedMultiKeyMultiEtagMleEntry.EditLink = new Uri("http://someeditlink");
            var action = this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetActions().Single();
            Assert.Equal("TestModel.Action", action.Title);
            Assert.Equal(action.Metadata, new Uri(MetadataDocumentUri, "#TestModel.Action"));
            Assert.Equal(action.Target, new Uri("http://someeditlink/TestModel.Action"));
        }
        #endregion Tests for computed Actions

        #region Tests for computed Functions
        [Fact]
        public void ProductShouldNotContainComputedFunctions()
        {
            Assert.Empty(this.productConventionalEntityMetadataBuilder.GetFunctions());
        }

        [Fact]
        public void DerivedProductShouldContainComputedFunctions()
        {
            var function = this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetFunctions().Single();
            Assert.Equal("TestModel.Function", function.Title);
            Assert.Equal(function.Metadata, new Uri(MetadataDocumentUri, "#TestModel.Function"));
            Assert.Equal(function.Target, new Uri("http://odata.org/base/Products(KeyA='keya',KeyB=1)/TestModel.DerivedMleProduct/TestModel.Function"));
        }

        [Fact]
        public void ComputedFunctionsTargetShouldBeBasedOnUserSetEditLink()
        {
            this.derivedMultiKeyMultiEtagMleEntry.EditLink = new Uri("http://someeditlink");
            var function = this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetFunctions().Single();
            Assert.Equal("TestModel.Function", function.Title);
            Assert.Equal(function.Metadata, new Uri(MetadataDocumentUri, "#TestModel.Function"));
            Assert.Equal(function.Target, new Uri("http://someeditlink/TestModel.Function"));
        }

        [Fact]
        public void GetFunctionsWhenKeyisLFDM()
        {
            var entitySetInstanceId = SetMultiKeyProperties();
            var function = this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetFunctions().Single();
            Assert.Equal(function.Target, new Uri(string.Format(@"http://odata.org/base/Products({0})/TestModel.DerivedMleProduct/TestModel.Function", entitySetInstanceId)));
        }
        #endregion Tests for computed Functions

        #region Tests for singleton
        [Fact]
        public void TestSingletonIdAndEditLink()
        {
            var singletonEntryTypeContext = new TestFeedAndEntryTypeContext
            {
                NavigationSourceName = "Boss",
                NavigationSourceEntityTypeName = "BossType",
                ExpectedResourceTypeName = "BossType",
                NavigationSourceKind = EdmNavigationSourceKind.Singleton,
            };

            var singletonEntryMetadataContext = new TestEntryMetadataContext
            {
                TypeContext = singletonEntryTypeContext,
                Resource = new ODataResource(),
                ETagProperties = new[] { new KeyValuePair<string, object>("Name", "Value") },
                KeyProperties = this.sinlgeKeyCollection,
                ActualResourceTypeName = "BossType",
                SelectedBindableOperations = new IEdmOperation[0],
                SelectedNavigationProperties = new IEdmNavigationProperty[0],
                SelectedStreamProperties = new Dictionary<string, IEdmStructuralProperty>(),
            };

            var singletonEntityMetadataBuilder = new ODataConventionalEntityMetadataBuilder(singletonEntryMetadataContext, this.metadataContext, this.uriBuilder);
            Assert.Equal(singletonEntityMetadataBuilder.GetId(), new Uri("http://odata.org/base/Boss"));
            Assert.Equal(singletonEntityMetadataBuilder.GetEditLink(), new Uri("http://odata.org/base/Boss"));
            Assert.Equal(singletonEntityMetadataBuilder.GetReadLink(), new Uri("http://odata.org/base/Boss"));
        }
        #endregion

        private Uri SetProductEntryEditLink()
        {
            var uri = new Uri("http://anotherodata.org/serviceBase/SomeType(Key1=24,Key2='abc')");
            this.productEntry.EditLink = uri;
            return uri;
        }

        private Uri SetProductEntryReadLink()
        {
            var uri = new Uri("http://anotherodata.org/serviceBase/SomeType(54)");
            this.productEntry.ReadLink = uri;
            return uri;
        }

        private Uri SetDerivedProductEntryEditLink()
        {
            var uri = new Uri("http://anotherodata.org/serviceBase/SomeType(Key1=24,Key2='abc')/FQ.NS.Type");
            this.derivedMultiKeyMultiEtagMleEntry.EditLink = uri;
            return uri;
        }

        private void SetSingleKeyPropertie(string name, object value)
        {
            this.sinlgeKeyCollection.Clear();
            this.sinlgeKeyCollection.Add(name, value);
        }

        private string SetMultiKeyProperties()
        {
            this.multiKeysCollection.Clear();
            this.multiKeysCollection.Add("LongId", -1L);
            this.multiKeysCollection.Add("FloatId", 1.0f);
            this.multiKeysCollection.Add("DoubleId", -1.0d);
            this.multiKeysCollection.Add("DecimalId", -1.0m);
            return "LongId=-1,FloatId=1,DoubleId=-1.0,DecimalId=-1.0";
        }
    }

    internal class TestEntryMetadataContext : IODataResourceMetadataContext
    {
        public ODataResourceBase Resource { get; set; }

        public IODataResourceTypeContext TypeContext { get; set; }

        public string ActualResourceTypeName { get; set; }

        public ICollection<KeyValuePair<string, object>> KeyProperties { get; set; }

        public IEnumerable<KeyValuePair<string, object>> ETagProperties { get; set; }

        public IEnumerable<IEdmNavigationProperty> SelectedNavigationProperties { get; set; }

        public IDictionary<string, IEdmStructuralProperty> SelectedStreamProperties { get; set; }

        public IEnumerable<IEdmOperation> SelectedBindableOperations { get; set; }

        public IEdmStructuredType ActualResourceType { get; set; }
    }

    internal class TestFeedAndEntryTypeContext : IODataResourceTypeContext
    {
        public string NavigationSourceName { get; set; }

        public string NavigationSourceEntityTypeName { get; set; }

        public string NavigationSourceFullTypeName { get; set; }

        public string ExpectedResourceTypeName { get; set; }

        public bool IsMediaLinkEntry { get; set; }

        public EdmNavigationSourceKind NavigationSourceKind { get; set; }

        public bool IsFromCollection { get; set; }
    }
}

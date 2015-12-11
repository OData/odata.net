﻿//---------------------------------------------------------------------
// <copyright file="ODataConventionalEntityMetadataBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Evaluation
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Evaluation;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.Test.OData.TDD.Tests.Reader.JsonLight;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ODataConventionalEntityMetadataBuilderTests
    {
        private static readonly Uri DefaultBaseUri = new Uri("http://odata.org/base/");
        private static readonly Uri MetadataDocumentUri = new Uri(DefaultBaseUri, "$metadata");
        private static readonly TestModel TestModel = TestModel.Initialize();
        private readonly SelectedPropertiesNode selectedProperties = SelectedPropertiesNode.EntireSubtree;

        private readonly ODataConventionalUriBuilder uriBuilder = new ODataConventionalUriBuilder(DefaultBaseUri, UrlConvention.CreateWithExplicitValue(false));
        private readonly TestMetadataContext metadataContext = new TestMetadataContext { GetMetadataDocumentUriFunc = () => MetadataDocumentUri, GetModelFunc = () => TestModel.Model, OperationsBoundToEntityTypeMustBeContainerQualifiedFunc = type => false };
        private ODataEntry productEntry;
        private Dictionary<string, object> sinlgeKeyCollection;
        private Dictionary<string, object> multiKeysCollection;
        private ODataConventionalEntityMetadataBuilder productConventionalEntityMetadataBuilder;
        private ODataEntry derivedMultiKeyMultiEtagMleEntry;
        private ODataConventionalEntityMetadataBuilder derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder;
        private ODataEntry containedCollectionProductEntry;
        private ODataConventionalEntityMetadataBuilder containedCollectionProductConventionalEntityMetadataBuilder;
        private ODataEntry containedProductEntry;
        private ODataConventionalEntityMetadataBuilder containedProductConventionalEntityMetadataBuilder;
        private Dictionary<string, object> containedSinlgeKeyCollection;
        private Dictionary<string, object> containedMultiKeysCollection;

        private const string EntitySetName = "Products";
        private const string EntityTypeName = "TestModel.Product";
        private const string DerivedEntityTypeName = "TestModel.DerivedProduct";
        private const string DerivedMleEntityTypeName = "TestModel.DerivedMleProduct";

        [TestInitialize]
        public void Init()
        {
            #region Product Entry

            this.productEntry = new ODataEntry();
            this.sinlgeKeyCollection = new Dictionary<string, object>() { { "Id", 42 } };
            this.multiKeysCollection = new Dictionary<string, object>() { { "KeyA", "keya" }, { "KeyB", 1 } };

            TestFeedAndEntryTypeContext productTypeContext = new TestFeedAndEntryTypeContext
            {
                NavigationSourceName = EntitySetName,
                NavigationSourceEntityTypeName = EntityTypeName,
                ExpectedEntityTypeName = EntityTypeName,
                IsMediaLinkEntry = false,
                UrlConvention = UrlConvention.CreateWithExplicitValue(/*generateKeyAsSegment*/ false),
                IsFromCollection = false,
                NavigationSourceKind = EdmNavigationSourceKind.EntitySet
            };

            TestEntryMetadataContext productEntryMetadataContext = new TestEntryMetadataContext
            {
                TypeContext = productTypeContext,
                Entry = this.productEntry,
                ETagProperties = new[] { new KeyValuePair<string, object>("Name", "Value") },
                KeyProperties = this.sinlgeKeyCollection,
                ActualEntityTypeName = EntityTypeName,
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
            this.derivedMultiKeyMultiEtagMleEntry = new ODataEntry();
            TestFeedAndEntryTypeContext derivedMultiKeyMultiEtagMleTypeContext = new TestFeedAndEntryTypeContext
            {
                NavigationSourceName = EntitySetName,
                NavigationSourceEntityTypeName = EntityTypeName,
                ExpectedEntityTypeName = DerivedEntityTypeName,
                IsMediaLinkEntry = true,
                UrlConvention = UrlConvention.CreateWithExplicitValue(/*generateKeyAsSegment*/ false),
                IsFromCollection = false
            };
            TestEntryMetadataContext derivedProductMleEntryMetadataContext = new TestEntryMetadataContext
            {
                TypeContext = derivedMultiKeyMultiEtagMleTypeContext,
                Entry = this.derivedMultiKeyMultiEtagMleEntry,
                ETagProperties = new[] { new KeyValuePair<string, object>("ETag1", "ETagValue1"), new KeyValuePair<string, object>("ETag2", "ETagValue2") },
                KeyProperties = this.multiKeysCollection,
                ActualEntityTypeName = DerivedMleEntityTypeName,
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

            this.containedCollectionProductEntry = new ODataEntry();
            this.containedSinlgeKeyCollection = new Dictionary<string, object>() { { "Id", 43 } };
            this.containedMultiKeysCollection = new Dictionary<string, object>() { { "KeyA", "keya" }, { "KeyB", 2 } };

            TestFeedAndEntryTypeContext containedCollectionProductTypeContext = new TestFeedAndEntryTypeContext
            {
                NavigationSourceName = EntitySetName,
                NavigationSourceEntityTypeName = EntityTypeName,
                ExpectedEntityTypeName = EntityTypeName,
                IsMediaLinkEntry = false,
                UrlConvention = UrlConvention.CreateWithExplicitValue(/*generateKeyAsSegment*/ false),
                NavigationSourceKind = EdmNavigationSourceKind.ContainedEntitySet,
                IsFromCollection = true
            };

            TestEntryMetadataContext containedCollectionProductEntryMetadataContext = new TestEntryMetadataContext
            {
                TypeContext = containedCollectionProductTypeContext,
                Entry = this.containedCollectionProductEntry,
                ETagProperties = new[] { new KeyValuePair<string, object>("Name", "Value") },
                KeyProperties = this.containedSinlgeKeyCollection,
                ActualEntityTypeName = EntityTypeName,
                SelectedBindableOperations = new IEdmOperation[0],
                SelectedNavigationProperties = new IEdmNavigationProperty[0],
                SelectedStreamProperties = new Dictionary<string, IEdmStructuralProperty>()
            };

            this.containedCollectionProductConventionalEntityMetadataBuilder = new ODataConventionalEntityMetadataBuilder(containedCollectionProductEntryMetadataContext, this.metadataContext, this.uriBuilder);
            this.containedCollectionProductEntry.MetadataBuilder = this.containedCollectionProductConventionalEntityMetadataBuilder;
            this.containedCollectionProductEntry.MetadataBuilder.ParentMetadataBuilder = this.productConventionalEntityMetadataBuilder;

            this.containedProductEntry = new ODataEntry();
            this.containedSinlgeKeyCollection = new Dictionary<string, object>() { { "Id", 43 } };
            this.containedMultiKeysCollection = new Dictionary<string, object>() { { "KeyA", "keya" }, { "KeyB", 2 } };

            TestFeedAndEntryTypeContext containedProductTypeContext = new TestFeedAndEntryTypeContext
            {
                NavigationSourceName = EntitySetName,
                NavigationSourceEntityTypeName = EntityTypeName,
                ExpectedEntityTypeName = EntityTypeName,
                IsMediaLinkEntry = false,
                UrlConvention = UrlConvention.CreateWithExplicitValue(/*generateKeyAsSegment*/ false),
                NavigationSourceKind = EdmNavigationSourceKind.ContainedEntitySet,
                IsFromCollection = false
            };

            TestEntryMetadataContext containedProductEntryMetadataContext = new TestEntryMetadataContext
            {
                TypeContext = containedProductTypeContext,
                Entry = this.containedCollectionProductEntry,
                ETagProperties = new[] { new KeyValuePair<string, object>("Name", "Value") },
                KeyProperties = this.containedSinlgeKeyCollection,
                ActualEntityTypeName = EntityTypeName,
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
        [TestMethod]
        public void GetStreamEditLinkShouldValidateArguments()
        {
            ODataEntityMetadataBuilderTestUtils.GetStreamEditLinkShouldValidateArguments(this.productConventionalEntityMetadataBuilder);
        }

        [TestMethod]
        public void GetStreamReadLinkShouldValidateArguments()
        {
            ODataEntityMetadataBuilderTestUtils.GetStreamReadLinkShouldValidateArguments(this.productConventionalEntityMetadataBuilder);
        }

        [TestMethod]
        public void GetNavigationLinkUriShouldValidateArguments()
        {
            ODataEntityMetadataBuilderTestUtils.GetNavigationLinkUriShouldValidateArguments(this.productConventionalEntityMetadataBuilder);
        }

        [TestMethod]
        public void GetAssociationLinkUriShouldValidateArguments()
        {
            ODataEntityMetadataBuilderTestUtils.GetAssociationLinkUriShouldValidateArguments(this.productConventionalEntityMetadataBuilder);
        }

        [TestMethod]
        public void GetOperationTargetUriShouldValidateArguments()
        {
            ODataEntityMetadataBuilderTestUtils.GetOperationTargetUriShouldValidateArguments(this.productConventionalEntityMetadataBuilder);
        }

        [TestMethod]
        public void GetOperationTitleShouldValidateArguments()
        {
            ODataEntityMetadataBuilderTestUtils.GetOperationTitleShouldValidateArguments(this.productConventionalEntityMetadataBuilder);
        }
        #endregion Argument validation tests

        #region Tests for GetEditLink()
        [TestMethod]
        public void GetEditLinkWithSingleKey()
        {
            this.productConventionalEntityMetadataBuilder.GetEditLink().Should().Be(new Uri("http://odata.org/base/Products(42)"));
        }

        [TestMethod]
        public void GetEditLinkWithMultipleKeys()
        {
            this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetEditLink().Should().Be(new Uri("http://odata.org/base/Products(KeyA='keya',KeyB=1)/TestModel.DerivedMleProduct"));
        }

        [TestMethod]
        public void GetEditLinkFromEntryInsteadOfBuilding()
        {
            var uri = this.SetProductEntryEditLink();
            this.productConventionalEntityMetadataBuilder.GetEditLink().Should().Be(uri);
        }

        [TestMethod]
        public void EditLinkShouldBeNullIfReadLinkIsSetButEditLinkIsNotSet()
        {
            var uri = this.SetProductEntryReadLink();
            this.productConventionalEntityMetadataBuilder.GetReadLink().Should().Be(uri);
            this.productConventionalEntityMetadataBuilder.GetEditLink().Should().BeNull();
        }

        [TestMethod]
        public void EditLinkShouldContainTypeSegmentIfInstanceTypeIsMoreDerviedThanSet()
        {
            // Verify that the last segment of the edit link is the expected type segment.
            string[] uriSegments = this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetEditLink().Segments;
            uriSegments[uriSegments.Length - 1].Should().Be("TestModel.DerivedMleProduct");
        }


        [TestMethod]
        public void GetEditLinkShouldReturnComputedIdWithTypeCastForDerivedEntity()
        {
            Uri id = this.derivedMultiKeyMultiEtagMleEntry.Id;
            Uri expectedEditLink = this.uriBuilder.AppendTypeSegment(id, DerivedMleEntityTypeName);
            this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetEditLink().Should().Be(expectedEditLink);
        }

        [TestMethod]
        public void GetEditLinkShouldReturnNonComputedIdUriWithTypeCastForDerivedEntityWhenNonComputedIdIsSet()
        {
            var id = new Uri("http://anotherodata.org/serviceBase/SomeType('xyz')");
            this.derivedMultiKeyMultiEtagMleEntry.Id = id;
            Uri expectedEditLink = this.uriBuilder.AppendTypeSegment(id, DerivedMleEntityTypeName);
            this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetEditLink().Should().Be(expectedEditLink);
        }

        [TestMethod]
        public void GetEditLinkShouldReturnNonComputedIdUriWhenNonComputedIdIsSet()
        {
            var id = new Uri("http://anotherodata.org/serviceBase/SomeType('xyz')");
            this.productEntry.Id = id;
            this.productConventionalEntityMetadataBuilder.GetEditLink().Should().Be(id);
        }

        [TestMethod]
        public void EditLinkShouldNotContainTypeSegmentIfInstanceTypeMatchesSetType()
        {
            this.productConventionalEntityMetadataBuilder.GetEditLink().AbsolutePath.Should().NotContain("TestModel.Product");
        }

        [TestMethod]
        public void GetEditLinkWithSingleKeyWhenKeyisInt64()
        {
            this.SetSingleKeyPropertie("Id", 1L);
            this.productConventionalEntityMetadataBuilder.GetEditLink().Should().Be(new Uri("http://odata.org/base/Products(1)"));
        }

        [TestMethod]
        public void GetEditLinkWithSingleKeyWhenKeyisFloat()
        {
            this.SetSingleKeyPropertie("Id", -1.0f);
            this.productConventionalEntityMetadataBuilder.GetEditLink().Should().Be(new Uri("http://odata.org/base/Products(-1)"));
        }

        [TestMethod]
        public void GetEditLinkWithSingleKeyWhenKeyisDouble()
        {
            this.SetSingleKeyPropertie("Id", 1.0d);
            this.productConventionalEntityMetadataBuilder.GetEditLink().Should().Be(new Uri("http://odata.org/base/Products(1.0)"));
        }

        [TestMethod]
        public void GetEditLinkWithSingleKeyWhenKeyisDecimal()
        {
            this.SetSingleKeyPropertie("Id", 0.0m);
            this.productConventionalEntityMetadataBuilder.GetEditLink().Should().Be(new Uri("http://odata.org/base/Products(0.0)"));
        }

        [TestMethod]
        public void GetEditLinkWithMultiKeysWhenKeyisLFDM()
        {
            var entitySetInstanceId = SetMultiKeyProperties();
            this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetEditLink().Should().Be(string.Format(@"http://odata.org/base/Products({0})/TestModel.DerivedMleProduct", entitySetInstanceId));
        }

        [TestMethod]
        public void EditlinkShouldBeNullWhenEntryIsATransientEntry()
        {
            this.productEntry.IsTransient = true;
            this.productConventionalEntityMetadataBuilder.GetEditLink().Should().BeNull();
        }

        [TestMethod]
        public void EditlinkShouldBeNonComputedEditLinkWhenEntryIsNotATransientEntryAndHaveNonComputedEditLink()
        {
            this.productEntry.IsTransient = false;
            var nonComputedEditLinkUri = new Uri("http://anotherodata.org/serviceBaseEdit/SomeType('xyz')");
            this.productEntry.EditLink = nonComputedEditLinkUri;
            this.productConventionalEntityMetadataBuilder.GetEditLink().Should().Be(nonComputedEditLinkUri);
        }

        [TestMethod]
        public void EditlinkShouldBeNonComputedEditLinkWhenEntryIsATransientEntryAndHaveNonComputedEditLink()
        {
            this.productEntry.IsTransient = true;
            this.productEntry.EditLink = new Uri("http://anotherodata.org/serviceBaseEdit/SomeType('xyz')");
            this.productConventionalEntityMetadataBuilder.GetEditLink().Should().Be(this.productEntry.EditLink);
        }

        #endregion Tests for GetEditLink()

        #region Tests for GetId()
        [TestMethod]
        public void GetIdForContainedCollectionProperty()
        {
            Uri id = this.containedCollectionProductConventionalEntityMetadataBuilder.GetId();
            id.Should().ToString().Contains("Products(42)/Products(43)");
        }

        [TestMethod]
        public void GetIdForContainedIndividualProperty()
        {
            Uri id = this.containedProductConventionalEntityMetadataBuilder.GetId();
            id.Should().ToString().EndsWith("Products(42)/Products");
        }

        [TestMethod]
        public void GetIdShouldBeGeneratedIdWhenEntryDoesNotContainIdEditOrReadLink()
        {
            this.productConventionalEntityMetadataBuilder.GetId().Should().Be("http://odata.org/base/Products(42)");
        }

        [TestMethod]
        public void GetIdShouldBeEntryIdWhenEntryContainsIdOnly()
        {
            var uri = new Uri("http://anotherodata.org/serviceBase/SomeType('xyz')");
            this.productEntry.Id = uri;
            this.productConventionalEntityMetadataBuilder.GetId().Should().Be(uri);
        }

        [TestMethod]
        public void GetIdShouldHonorUserOverridingId()
        {
            this.productEntry.Id.Should().Be("http://odata.org/base/Products(42)");
            Uri uri = new Uri("http://overwrite");
            this.productEntry.Id = uri;
            this.productEntry.Id.Should().Be(uri);
        }

        [TestMethod]
        public void GetIdShouldBeEntryIdWhenEntryContainsIdEditAndReadLink()
        {
            var id = new Uri("http://anotherodata.org/serviceBase/SomeType('xyz')");
            this.productEntry.Id = id;
            var readLinkUri = new Uri("http://anotherodata.org/serviceBaseRead/SomeType('xyz')");
            this.productEntry.ReadLink = readLinkUri;
            var editLinkUri = new Uri("http://anotherodata.org/serviceBaseEdit/SomeType('xyz')");
            this.productEntry.EditLink = editLinkUri;

            this.productConventionalEntityMetadataBuilder.GetId().Should().Be(id);
        }

        [TestMethod]
        public void GetIdShouldBeCanonicalUrlWhenEntryContainsReadLinkOnly()
        {
            var canonicalUrl = this.productConventionalEntityMetadataBuilder.GetId();
            var uri = new Uri("http://anotherodata.org/serviceBase/SomeType('xyz')");
            this.productEntry.ReadLink = uri;
            this.productConventionalEntityMetadataBuilder.GetId().Should().Be(canonicalUrl);
        }

        [TestMethod]
        public void GetIdShouldBeCanonicalUrlWhenEntryContainsEditLinkOnly()
        {
            var canonicalUrl = this.productConventionalEntityMetadataBuilder.GetId();
            var uri = new Uri("http://anotherodata.org/serviceBase/SomeType('xyz')");
            this.productEntry.EditLink = uri;
            this.productConventionalEntityMetadataBuilder.GetId().Should().Be(canonicalUrl);
        }

        [TestMethod]
        public void GetIdShouldBeCanonicalUrlWhenEntryContainsBothReadLinkAndEditLink()
        {
            var canonicalUrl = this.productConventionalEntityMetadataBuilder.GetId();
            var readLinkUri = new Uri("http://anotherodata.org/serviceBaseRead/SomeType('xyz')");
            this.productEntry.ReadLink = readLinkUri;
            var editLinkUri = new Uri("http://anotherodata.org/serviceBaseEdit/SomeType('xyz')");
            this.productEntry.EditLink = editLinkUri;
            this.productConventionalEntityMetadataBuilder.GetId().Should().Be(canonicalUrl);
        }

        [TestMethod]
        public void TryGetIdForSerializationShouldBeNullWhenEntryIsTransient()
        {
            this.productEntry.IsTransient = true;
            Uri id;
            productConventionalEntityMetadataBuilder.TryGetIdForSerialization(out id).Should().BeTrue();
            id.Should().BeNull();
        }

        [TestMethod]
        public void TryGetIdForSerializationShouldBeCanonicalUrlWhenEntryIsNotTransientAndDoNotHaveNoComputedId()
        {
            this.productEntry.IsTransient = false;
            var canonicalUrl = this.productEntry.Id;
            Uri id;
            productConventionalEntityMetadataBuilder.TryGetIdForSerialization(out id).Should().BeTrue();
            id.Should().Be(canonicalUrl);
        }

        [TestMethod]
        public void GetIdShouldBeNullWhenEntryIsTransient()
        {
            this.productEntry.IsTransient = true;
            productConventionalEntityMetadataBuilder.GetId().Should().BeNull();
        }

        [TestMethod]
        public void GetIdWithSingleKeyWhenKeyisInt64AndEntryDoesNotContainIdEditOrReadLink()
        {
            this.SetSingleKeyPropertie("Id", -1L);
            this.productConventionalEntityMetadataBuilder.GetId().Should().Be(new Uri(@"http://odata.org/base/Products(-1)"));
        }

        [TestMethod]
        public void GetIdWithSingleKeyWhenKeyisFloatAndEntryDoesNotContainIdEditOrReadLink()
        {
            this.SetSingleKeyPropertie("Id", -1.0f);
            this.productConventionalEntityMetadataBuilder.GetId().Should().Be(@"http://odata.org/base/Products(-1)");
        }

        [TestMethod]
        public void GetIdWithSingleKeyWhenKeyisDoubleAndEntryDoesNotContainIdEditOrReadLink()
        {
            this.SetSingleKeyPropertie("Id", 1.0d);
            this.productConventionalEntityMetadataBuilder.GetId().Should().Be(new Uri(@"http://odata.org/base/Products(1.0)"));
        }

        [TestMethod]
        public void GetIdWithSingleKeyWhenKeyisDecimalAndEntryDoesNotContainIdEditOrReadLink()
        {
            this.SetSingleKeyPropertie("Id", 0.0m);
            this.productConventionalEntityMetadataBuilder.GetId().Should().Be(new Uri(@"http://odata.org/base/Products(0.0)"));
        }

        [TestMethod]
        public void GetIdWithMultiKeysWhenKeyisLongLFDM()
        {
            var entitySetInstanceId = SetMultiKeyProperties();
            this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetId().Should().Be(new Uri(string.Format(@"http://odata.org/base/Products({0})", entitySetInstanceId)));
        }

        #endregion Tests for GetId()

        #region Tests for GetReadLink()
        [TestMethod]
        public void GetReadLinkWithSingleKey()
        {
            this.productConventionalEntityMetadataBuilder.GetReadLink().Should().Be(new Uri("http://odata.org/base/Products(42)"));
        }

        [TestMethod]
        public void GetReadLinkWithMultipleKeys()
        {
            this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetReadLink().Should().Be(new Uri("http://odata.org/base/Products(KeyA='keya',KeyB=1)/TestModel.DerivedMleProduct"));
        }

        [TestMethod]
        public void GetReadLinkFromEntryInsteadOfBuilding()
        {
            var uri = this.SetProductEntryReadLink();
            this.productConventionalEntityMetadataBuilder.GetReadLink().Should().Be(uri);
        }

        [TestMethod]
        public void GetReadLinkWhenEntryHasEditLinkButNotReadLink()
        {
            var uri = this.SetProductEntryEditLink();
            this.productConventionalEntityMetadataBuilder.GetReadLink().Should().Be(uri);
        }

        [TestMethod]
        public void GetReadLinkWhenEntryHasBothEditLinkAndReadLink()
        {
            this.SetProductEntryEditLink();
            var uri = this.SetProductEntryReadLink();
            this.productConventionalEntityMetadataBuilder.GetReadLink().Should().Be(uri);
        }

        [TestMethod]
        public void GetReadLinkWithSingleKeyWhenKeyisInt64()
        {
            this.SetSingleKeyPropertie("Id", 1L);
            this.productConventionalEntityMetadataBuilder.GetReadLink().Should().Be(new Uri("http://odata.org/base/Products(1)"));
        }

        [TestMethod]
        public void GetReadLinkWithSingleKeyWhenKeyisFloat()
        {
            this.SetSingleKeyPropertie("Id", -1.0f);
            this.productConventionalEntityMetadataBuilder.GetReadLink().Should().Be(new Uri("http://odata.org/base/Products(-1)"));
        }

        [TestMethod]
        public void GetReadLinkWithSingleKeyWhenKeyisDouble()
        {
            this.SetSingleKeyPropertie("Id", 1.0d);
            this.productConventionalEntityMetadataBuilder.GetReadLink().Should().Be(new Uri("http://odata.org/base/Products(1.0)"));
        }

        [TestMethod]
        public void GetReadLinkWithSingleKeyWhenKeyisDecimal()
        {
            this.SetSingleKeyPropertie("Id", 0.0m);
            this.productConventionalEntityMetadataBuilder.GetReadLink().Should().Be(new Uri("http://odata.org/base/Products(0.0)"));
        }

        [TestMethod]
        public void GetReadLinkWithMultiKeysWhenKeyisLFDM()
        {
            var entitySetInstanceId = SetMultiKeyProperties();
            this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetReadLink().Should().Be(string.Format(@"http://odata.org/base/Products({0})/TestModel.DerivedMleProduct", entitySetInstanceId));
        }

        [TestMethod]
        public void ReadLinkShouldBeNullWhenEntryIsATransientEntry()
        {
            this.productEntry.IsTransient = true;
            this.productConventionalEntityMetadataBuilder.GetReadLink().Should().BeNull();
        }

        [TestMethod]
        public void ReadLinkShouldBeNonComputedReadLinkWhenEntryIsNotATransientEntryAndHaveNonComputedReadLink()
        {
            this.productEntry.IsTransient = false;
            var readLinkUri = new Uri("http://anotherodata.org/serviceBaseRead/SomeType('xyz')");
            this.productEntry.ReadLink = readLinkUri;
            this.productConventionalEntityMetadataBuilder.GetReadLink().Should().Be(readLinkUri);
        }

        [TestMethod]
        public void ReadLinkShouldBeComputedReadLinkWhenEntryIsNotATransientEntry()
        {
            this.productEntry.IsTransient = false;
            var computedReadLinkUri = this.productEntry.ReadLink;
            this.productConventionalEntityMetadataBuilder.GetReadLink().Should().Be(computedReadLinkUri);
        }

        #endregion Tests for GetReadLink()

        #region Tests for GetETag()
        [TestMethod]
        public void ETagShouldBeNullForTypeWithoutConcurrencyTokens()
        {
            var testSubject = new ODataConventionalEntityMetadataBuilder(new TestEntryMetadataContext { Entry = new ODataEntry(), ETagProperties = new KeyValuePair<string, object>[0] }, this.metadataContext, this.uriBuilder);
            testSubject.GetETag().Should().Be((string)null);
        }

        [TestMethod]
        public void EtagShouldBeUriEscaped()
        {
            // if this fails System.Uri has changed its behavior and we may need to adjust how we encode our strings for JsonLight
            // .net 45 changed this behavior initially to escape ' to a value, but was changed. below test 
            // validates that important uri literal values that OData uses don't change, and that we escape characters when 
            // producing the etag for JsonLight
            var escapedStrings = Uri.EscapeUriString(@".:''-");
            escapedStrings.Should().Be(@".:''-");

            var testSubject = new ODataConventionalEntityMetadataBuilder(new TestEntryMetadataContext { Entry = new ODataEntry(), ETagProperties = new[] { new KeyValuePair<string, object>("ETag", "Value ") } }, this.metadataContext, this.uriBuilder);
            testSubject.GetETag().Should().Be(@"W/""'Value%20'""");
        }

        [TestMethod]
        public void ETagShouldBeCorrectForTypeWithOneConcurrencyToken()
        {
            var testSubject = new ODataConventionalEntityMetadataBuilder(new TestEntryMetadataContext { Entry = new ODataEntry(), ETagProperties = new[] { new KeyValuePair<string, object>("ETag", "Value") } }, this.metadataContext, this.uriBuilder);
            testSubject.GetETag().Should().Be(@"W/""'Value'""");
        }

        [TestMethod]
        public void ETagShouldBeCorrectForNullConcurrencyToken()
        {
            var testSubject = new ODataConventionalEntityMetadataBuilder(new TestEntryMetadataContext { Entry = new ODataEntry(), ETagProperties = new[] { new KeyValuePair<string, object>("ETag", default(string)) } }, this.metadataContext, this.uriBuilder);
            testSubject.GetETag().Should().Be(@"W/""null""");
        }

        [TestMethod]
        public void ETagShouldBeCorrectForTypeWithManyConcurrencyTokens()
        {
            var values = new[]
            {
                new KeyValuePair<string, object>("ETag1", 1.2345e+45), 
                new KeyValuePair<string, object>("ETag2", new byte[] { 1, 2, 3 }), 
                new KeyValuePair<string, object>("ETag3", 2.3M)
            };

            var testSubject = new ODataConventionalEntityMetadataBuilder(new TestEntryMetadataContext { Entry = new ODataEntry(), ETagProperties = values }, this.metadataContext, this.uriBuilder);
            testSubject.GetETag().Should().Be(@"W/""1.2345E%2B45,binary'AQID',2.3""");
        }

        [TestMethod]
        public void ETagShouldBeCorrectForLDMFWithManyConcurrencyTokens()
        {
            var values = new[]
            {
                new KeyValuePair<string, object>("ETagLong", 1L), 
                new KeyValuePair<string, object>("ETagFloat", 1.0F), 
                new KeyValuePair<string, object>("ETagDouble", 1.0D),
                new KeyValuePair<string, object>("ETagDecimal", 1.0M)
            };

            var testSubject = new ODataConventionalEntityMetadataBuilder(new TestEntryMetadataContext { Entry = new ODataEntry(), ETagProperties = values }, this.metadataContext, this.uriBuilder);
            testSubject.GetETag().Should().Be(@"W/""1,1,1.0,1.0""");
        }
        #endregion Tests for GetETag()

        #region Tests for GetStreamEditLink()
        [TestMethod]
        public void GetStreamEditLinkForDefaultStream()
        {
            this.productConventionalEntityMetadataBuilder.GetStreamEditLink(null).Should().Be(new Uri("http://odata.org/base/Products(42)/$value"));
        }

        [TestMethod]
        public void GetStreamEditLinkForDefaultStreamWhenEntryHasEditLink()
        {
            Uri uri = this.SetProductEntryEditLink();
            this.productConventionalEntityMetadataBuilder.GetStreamEditLink(null).Should().Be(new Uri(uri.AbsoluteUri + "/$value"));
        }

        [TestMethod]
        public void GetStreamEditLinkForStreamProperty()
        {
            this.productConventionalEntityMetadataBuilder.GetStreamEditLink("StreamProperty").Should().Be(new Uri("http://odata.org/base/Products(42)/StreamProperty"));
        }

        [TestMethod]
        public void GetStreamEditLinkForStreamPropertyWhenEntryHasEditLink()
        {
            Uri uri = this.SetProductEntryEditLink();
            this.productConventionalEntityMetadataBuilder.GetStreamEditLink("StreamProperty").Should().Be(new Uri(uri.AbsoluteUri + "/StreamProperty"));
        }

        [TestMethod]
        public void GetDefaultStreamEditLinkWithMultiKeysWhenKeyisLFDM()
        {
            var entitySetInstanceId = SetMultiKeyProperties();
            this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetStreamEditLink(null).Should().Be(string.Format(@"http://odata.org/base/Products({0})/TestModel.DerivedMleProduct/$value", entitySetInstanceId));
        }
        #endregion Tests for GetStreamEditLink()

        #region Tests for GetMediaResource
        [TestMethod]
        public void ShouldNotComputeMrForNonMle()
        {
            this.productConventionalEntityMetadataBuilder.GetMediaResource().Should().BeNull();
        }

        [TestMethod]
        public void ShouldComputeMrForMle()
        {
            var mr = this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetMediaResource();
            mr.Should().NotBeNull();
            mr.EditLink.Should().Be(new Uri("http://odata.org/base/Products(KeyA='keya',KeyB=1)/TestModel.DerivedMleProduct/$value"));
            mr.ReadLink.Should().Be(new Uri("http://odata.org/base/Products(KeyA='keya',KeyB=1)/TestModel.DerivedMleProduct/$value"));
        }

        [TestMethod]
        public void ShouldUseNonComputedMediaResourceIfSet()
        {
            this.derivedMultiKeyMultiEtagMleEntry.MediaResource = new ODataStreamReferenceValue();
            this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetMediaResource().Should().BeSameAs(this.derivedMultiKeyMultiEtagMleEntry.MediaResource);
        }

        [TestMethod]
        public void ShouldUseUserSetEditLinkAndReadLinkInComputedMr()
        {
            this.derivedMultiKeyMultiEtagMleEntry.EditLink = new Uri("http://someeditlink");
            this.derivedMultiKeyMultiEtagMleEntry.ReadLink = new Uri("http://somereadlink");
            var mr = this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetMediaResource();
            mr.Should().NotBeNull();
            mr.EditLink.Should().Be(new Uri("http://someeditlink/$value"));
            mr.ReadLink.Should().Be(new Uri("http://somereadlink/$value"));
        }
        #endregion Tests for GetMediaResource

        #region Tests for GetStreamReadLink()
        [TestMethod]
        public void GetStreamReadLinkForDefaultStream()
        {
            this.productConventionalEntityMetadataBuilder.GetStreamReadLink(null).Should().Be(new Uri("http://odata.org/base/Products(42)/$value"));
        }

        [TestMethod]
        public void GetStreamReadLinkForDefaultStreamWhenEntryHasReadLinkAndNotEditLink()
        {
            Uri uri = this.SetProductEntryReadLink();
            this.productConventionalEntityMetadataBuilder.GetStreamReadLink(null).Should().Be(new Uri(uri.AbsoluteUri + "/$value"));
        }

        [TestMethod]
        public void GetStreamReadLinkForDefaultStreamWhenEntryHasEditLinkAndNotReadLink()
        {
            Uri uri = this.SetProductEntryEditLink();
            this.productConventionalEntityMetadataBuilder.GetStreamReadLink(null).Should().Be(new Uri(uri.AbsoluteUri + "/$value"));
        }

        [TestMethod]
        public void GetStreamReadLinkForDefaultStreamWhenEntryHasBothEditLinkAndReadLink()
        {
            this.SetProductEntryEditLink();
            Uri uri = this.SetProductEntryReadLink();
            this.productConventionalEntityMetadataBuilder.GetStreamReadLink(null).Should().Be(new Uri(uri.AbsoluteUri + "/$value"));
        }

        [TestMethod]
        public void GetStreamReadLinkForStreamProperty()
        {
            this.productConventionalEntityMetadataBuilder.GetStreamReadLink("StreamProperty").Should().Be(new Uri("http://odata.org/base/Products(42)/StreamProperty"));
        }

        [TestMethod]
        public void GetStreamReadLinkForStreamPropertyWhenEntryHasReadLinkAndNotEditLink()
        {
            Uri uri = this.SetProductEntryReadLink();
            this.productConventionalEntityMetadataBuilder.GetStreamReadLink("StreamProperty").Should().Be(new Uri(uri.AbsoluteUri + "/StreamProperty"));
        }

        [TestMethod]
        public void GetStreamReadLinkForStreamPropertyWhenEntryHasEditLinkAndNotReadLink()
        {
            Uri uri = this.SetProductEntryEditLink();
            this.productConventionalEntityMetadataBuilder.GetStreamReadLink("StreamProperty").Should().Be(new Uri(uri.AbsoluteUri + "/StreamProperty"));
        }

        [TestMethod]
        public void GetStreamReadLinkForStreamPropertyWhenEntryHasBothReadLinkAndEditLink()
        {
            this.SetProductEntryEditLink();
            Uri uri = this.SetProductEntryReadLink();
            this.productConventionalEntityMetadataBuilder.GetStreamReadLink("StreamProperty").Should().Be(new Uri(uri.AbsoluteUri + "/StreamProperty"));
        }

        [TestMethod]
        public void GetDefaultStreamReadLinkWhenKeyisLFDM()
        {
            var entitySetInstanceId = SetMultiKeyProperties();
            this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetStreamReadLink(null).Should().Be(string.Format(@"http://odata.org/base/Products({0})/TestModel.DerivedMleProduct/$value", entitySetInstanceId));
        }
        #endregion Tests for GetStreamReadLink()

        #region Tests for GetNavigationLinkUri()
        [TestMethod]
        public void GetNavigationLinkUri()
        {
            this.productConventionalEntityMetadataBuilder.GetNavigationLinkUri("NavigationProperty", null, false).Should().Be(new Uri("http://odata.org/base/Products(42)/NavigationProperty"));
            this.productConventionalEntityMetadataBuilder.GetNavigationLinkUri("NavigationProperty", null, true).Should().BeNull();
        }

        [TestMethod]
        public void GetNavigationLinkUriWhenLinkAlreadyHasValue()
        {
            this.productConventionalEntityMetadataBuilder.GetNavigationLinkUri("NavigationProperty", new Uri("http://example.com/override"), false).Should().Be(new Uri("http://odata.org/base/Products(42)/NavigationProperty"));
            this.productConventionalEntityMetadataBuilder.GetNavigationLinkUri("NavigationProperty", new Uri("http://example.com/override"), true).Should().Be(new Uri("http://example.com/override"));
        }

        [TestMethod]
        public void GetNavigationLinkUriWhenEntryHasEditLink()
        {
            Uri uri = this.SetProductEntryEditLink();
            this.productConventionalEntityMetadataBuilder.GetNavigationLinkUri("NavigationProperty", null, false).Should().Be(new Uri(uri.AbsoluteUri + "/NavigationProperty"));
        }

        [TestMethod]
        public void GetNavigationLinkUriWhenEntryHasReadLink()
        {
            Uri uri = this.SetProductEntryReadLink();
            this.productConventionalEntityMetadataBuilder.GetNavigationLinkUri("NavigationProperty", null, false).Should().Be(new Uri(uri.AbsoluteUri + "/NavigationProperty"));
        }

        [TestMethod]
        public void GetNavigationLinkUriShouldFollowReadLinkWhenEntryHasBothReadLinkAndEditLink()
        {
            Uri uri = this.SetProductEntryReadLink();
            this.SetProductEntryEditLink();
            this.productConventionalEntityMetadataBuilder.GetNavigationLinkUri("NavigationProperty", null, false).Should().Be(new Uri(uri.AbsoluteUri + "/NavigationProperty"));
        }

        [TestMethod]
        public void GetNavigationLinkUriWhenKeyisLFDM()
        {
            var entitySetInstanceId = SetMultiKeyProperties();
            this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetNavigationLinkUri("NavigationProperty", null, false).Should().Be(string.Format(@"http://odata.org/base/Products({0})/TestModel.DerivedMleProduct/NavigationProperty", entitySetInstanceId));
        }
        #endregion Tests for GetNavigationLinkUri()

        #region Tests for GetAssociationLinkUri()
        [TestMethod]
        public void GetAssociationLinkUri()
        {
            this.productConventionalEntityMetadataBuilder.GetAssociationLinkUri("NavigationProperty", null, false).Should().Be(new Uri("http://odata.org/base/Products(42)/NavigationProperty/$ref"));
            this.productConventionalEntityMetadataBuilder.GetAssociationLinkUri("NavigationProperty", null, true).Should().BeNull();
        }

        [TestMethod]
        public void GetAssociationLinkUriWhenLinkAlreadyHasValue()
        {
            this.productConventionalEntityMetadataBuilder.GetAssociationLinkUri("NavigationProperty", new Uri("http://example.com/override"), false).Should().Be(new Uri("http://odata.org/base/Products(42)/NavigationProperty/$ref"));
            this.productConventionalEntityMetadataBuilder.GetAssociationLinkUri("NavigationProperty", new Uri("http://example.com/override"), true).Should().Be(new Uri("http://example.com/override"));
        }

        [TestMethod]
        public void GetAssociationLinkUriWhenEntryHasEditLink()
        {
            Uri uri = this.SetProductEntryEditLink();
            this.productConventionalEntityMetadataBuilder.GetAssociationLinkUri("NavigationProperty", null, false).Should().Be(new Uri(uri.AbsoluteUri + "/NavigationProperty/$ref"));
        }

        [TestMethod]
        public void GetAssociationLinkUriWhenEntryHasReadLink()
        {
            Uri uri = this.SetProductEntryReadLink();
            this.productConventionalEntityMetadataBuilder.GetAssociationLinkUri("NavigationProperty", null, false).Should().Be(new Uri(uri.AbsoluteUri + "/NavigationProperty/$ref"));
        }

        [TestMethod]
        public void GetAssociationLinkUriShouldFollowReadLinkWhenEntryHasBothReadLinkAndEditLink()
        {
            Uri uri = this.SetProductEntryReadLink();
            this.SetProductEntryEditLink();
            this.productConventionalEntityMetadataBuilder.GetAssociationLinkUri("NavigationProperty", null, false).Should().Be(new Uri(uri.AbsoluteUri + "/NavigationProperty/$ref"));
        }

        [TestMethod]
        public void GetAssociationLinkUriWhenKeyisLFDM()
        {
            var entitySetInstanceId = SetMultiKeyProperties();
            this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetAssociationLinkUri("NavigationProperty", null, false).Should().Be(string.Format(@"http://odata.org/base/Products({0})/TestModel.DerivedMleProduct/NavigationProperty/$ref", entitySetInstanceId));
        }
        #endregion Tests for GetAssociationLinkUri()

        #region Tests for MarkNavigationLinkAsProcessed() and GetNextUnprocessedNavigationLink()
        [TestMethod]
        public void GetNextUnprocessedNavigationLinkShouldBeNullIfTypeHasNoNavProps()
        {
            this.productConventionalEntityMetadataBuilder.GetNextUnprocessedNavigationLink()
                .Should().BeNull();
        }

        [TestMethod]
        public void GetNextUnprocessedNavigationLinkShouldReturnNavProps()
        {
            var nextNavProp = this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetNextUnprocessedNavigationLink();
            nextNavProp.NavigationLink.Name.Should().Be("RelatedProducts");

            nextNavProp = this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetNextUnprocessedNavigationLink();
            nextNavProp.NavigationLink.Name.Should().Be("RelatedDerivedProduct");
        }

        [TestMethod]
        public void GetNextUnprocessedNavigationLinkShouldNotReturnNavPropsThatWerePreviouslyMarkedAsProcessed()
        {
            this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.MarkNavigationLinkProcessed("RelatedDerivedProduct");

            var nextNavProp = this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetNextUnprocessedNavigationLink();
            nextNavProp.NavigationLink.Name.Should().Be("RelatedProducts");

            nextNavProp = this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetNextUnprocessedNavigationLink();
            nextNavProp.Should().BeNull();
        }

        [TestMethod]
        public void GetNextUnprocessedNavigationLinkShouldReturnANavPropWithoutUrls()
        {
            // Note: it is up to the reader and writer to later add a metadata builder to navigation links generated this way.
            var nextNavProp = this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetNextUnprocessedNavigationLink();
            nextNavProp.NavigationLink.Url.Should().BeNull();
            nextNavProp.NavigationLink.AssociationLinkUrl.Should().BeNull();
        }
        #endregion Tests for MarkNavigationLinkAsProcessed() and GetNextUnprocessedNavigationLink()

        #region Tests for GetOperationTargetUri()
        [TestMethod]
        public void GetOperationTargetUri()
        {
            this.productConventionalEntityMetadataBuilder.GetOperationTargetUri("OperationName", null, null).Should().Be(new Uri("http://odata.org/base/Products(42)/OperationName"));
        }

        [TestMethod]
        public void GetOperationTargetUriWhenEntryHasEditLink()
        {
            Uri uri = this.SetProductEntryEditLink();
            this.productConventionalEntityMetadataBuilder.GetOperationTargetUri("OperationName", null, null).Should().Be(new Uri(uri.AbsoluteUri + "/OperationName"));
        }

        [TestMethod]
        public void GetOperationTargetUriWithInheritance()
        {
            this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetOperationTargetUri("OperationName", null, null).Should().Be(new Uri("http://odata.org/base/Products(KeyA='keya',KeyB=1)/TestModel.DerivedMleProduct/OperationName"));
        }

        [TestMethod]
        public void GetOperationTargetUriWithInheritanceWhenEntryHasEditLink()
        {
            Uri uri = this.SetDerivedProductEntryEditLink();
            this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetOperationTargetUri("OperationName", null, null).Should().Be(new Uri(uri.AbsoluteUri + "/OperationName"));
        }

        [TestMethod]
        public void GetOperationTargetUriWithParameterType()
        {
            this.productConventionalEntityMetadataBuilder.GetOperationTargetUri("OperationName", null, "p1").Should().Be(new Uri("http://odata.org/base/Products(42)/OperationName(p1=@p1)"));
        }

        [TestMethod]
        public void GetOperationTargetUriWithParameterTypeWhenEntryHasEditLink()
        {
            Uri editLink = this.SetProductEntryEditLink();
            this.productConventionalEntityMetadataBuilder.GetOperationTargetUri("OperationName", null, "p1").Should().Be(new Uri(editLink.AbsoluteUri + "/OperationName(p1=@p1)"));
        }

        [TestMethod]
        public void GetOperationTargetUriWithParameterTypeAndInheritance()
        {
            this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetOperationTargetUri("OperationName", null, "p1").Should().Be(new Uri("http://odata.org/base/Products(KeyA='keya',KeyB=1)/TestModel.DerivedMleProduct/OperationName(p1=@p1)"));
        }

        [TestMethod]
        public void GetOperationTargetUriWithParameterTypeAndInheritanceWhenEntryHasEditLink()
        {
            Uri editLink = this.SetDerivedProductEntryEditLink();

            // note that the type segment and operation name are appended onto the opaque edit-link, and may result in multiple type segments in the final target link.
            this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetOperationTargetUri("OperationName", null, "p1").Should().Be(new Uri(editLink.AbsoluteUri + "/OperationName(p1=@p1)"));
        }

        [TestMethod]
        public void GetOperationTargetUriWhenKeyisLFDM()
        {
            var entitySetInstanceId = SetMultiKeyProperties();
            this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetOperationTargetUri("OperationName", null, null).Should().Be(string.Format(@"http://odata.org/base/Products({0})/TestModel.DerivedMleProduct/OperationName", entitySetInstanceId));
        }
        #endregion Tests for GetOperationTargetUri()

        #region Tests for GetOperationTitle()
        [TestMethod]
        public void GetOperationTitle()
        {
            this.productConventionalEntityMetadataBuilder.GetOperationTitle("OperationName").Should().Be("OperationName");
        }
        #endregion Tests for GetOperationTitle()

        #region Tests for GetProperties()
        [TestMethod]
        public void ProductShouldNotContainComputedNamedStreams()
        {
            this.productConventionalEntityMetadataBuilder.GetProperties(/*nonComputedProperties*/null).Should().BeEmpty();
        }

        [TestMethod]
        public void DerivedProductShouldContainComputedNamedStreams()
        {
            var photoProperty = this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetProperties( /*nonComputedProperties*/null).Single();
            photoProperty.Name.Should().Be("Photo");
            var photo = (ODataStreamReferenceValue)photoProperty.Value;
            photo.Should().NotBeNull();
            photo.EditLink.Should().Be(new Uri("http://odata.org/base/Products(KeyA='keya',KeyB=1)/TestModel.DerivedMleProduct/Photo"));
            photo.ReadLink.Should().Be(new Uri("http://odata.org/base/Products(KeyA='keya',KeyB=1)/TestModel.DerivedMleProduct/Photo"));
        }

        [TestMethod]
        public void ComputedNamedStreamsShouldUseUserSetEditLinkAndReadLink()
        {
            this.derivedMultiKeyMultiEtagMleEntry.EditLink = new Uri("http://someeditlink");
            this.derivedMultiKeyMultiEtagMleEntry.ReadLink = new Uri("http://somereadlink");
            var photoProperty = this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetProperties( /*nonComputedProperties*/null).Single();
            photoProperty.Name.Should().Be("Photo");
            var photo = (ODataStreamReferenceValue)photoProperty.Value;
            photo.Should().NotBeNull();
            photo.EditLink.Should().Be(new Uri("http://someeditlink/Photo"));
            photo.ReadLink.Should().Be(new Uri("http://somereadlink/Photo"));
        }
        #endregion Tests for GetProperties()

        #region Tests for computed Actions
        [TestMethod]
        public void ProductShouldNotContainComputedActions()
        {
            this.productConventionalEntityMetadataBuilder.GetActions().Should().BeEmpty();
        }

        [TestMethod]
        public void DerivedProductShouldContainComputedActions()
        {
            var action = this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetActions().Single();
            action.Title.Should().Be("TestModel.Action");
            action.Metadata.Should().Be(new Uri(MetadataDocumentUri, "#TestModel.Action"));
            action.Target.Should().Be(new Uri("http://odata.org/base/Products(KeyA='keya',KeyB=1)/TestModel.DerivedMleProduct/TestModel.Action"));
        }

        [TestMethod]
        public void GetActionsWhenKeyisLFDM()
        {
            var entitySetInstanceId = SetMultiKeyProperties();
            var action = this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetActions().Single();
            action.Target.Should().Be(new Uri(string.Format(@"http://odata.org/base/Products({0})/TestModel.DerivedMleProduct/TestModel.Action", entitySetInstanceId)));
        }

        [TestMethod]
        public void ComputedActionsTargetShouldBeBasedOnUserSetEditLink()
        {
            this.derivedMultiKeyMultiEtagMleEntry.EditLink = new Uri("http://someeditlink");
            var action = this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetActions().Single();
            action.Title.Should().Be("TestModel.Action");
            action.Metadata.Should().Be(new Uri(MetadataDocumentUri, "#TestModel.Action"));
            action.Target.Should().Be(new Uri("http://someeditlink/TestModel.Action"));
        }
        #endregion Tests for computed Actions

        #region Tests for computed Functions
        [TestMethod]
        public void ProductShouldNotContainComputedFunctions()
        {
            this.productConventionalEntityMetadataBuilder.GetFunctions().Should().BeEmpty();
        }

        [TestMethod]
        public void DerivedProductShouldContainComputedFunctions()
        {
            var function = this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetFunctions().Single();
            function.Title.Should().Be("TestModel.Function");
            function.Metadata.Should().Be(new Uri(MetadataDocumentUri, "#TestModel.Function"));
            function.Target.Should().Be(new Uri("http://odata.org/base/Products(KeyA='keya',KeyB=1)/TestModel.DerivedMleProduct/TestModel.Function"));
        }

        [TestMethod]
        public void ComputedFunctionsTargetShouldBeBasedOnUserSetEditLink()
        {
            this.derivedMultiKeyMultiEtagMleEntry.EditLink = new Uri("http://someeditlink");
            var function = this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetFunctions().Single();
            function.Title.Should().Be("TestModel.Function");
            function.Metadata.Should().Be(new Uri(MetadataDocumentUri, "#TestModel.Function"));
            function.Target.Should().Be(new Uri("http://someeditlink/TestModel.Function"));
        }

        [TestMethod]
        public void GetFunctionsWhenKeyisLFDM()
        {
            var entitySetInstanceId = SetMultiKeyProperties();
            var function = this.derivedMultiKeyMultiEtagMleConventionalEntityMetadataBuilder.GetFunctions().Single();
            function.Target.Should().Be(new Uri(string.Format(@"http://odata.org/base/Products({0})/TestModel.DerivedMleProduct/TestModel.Function", entitySetInstanceId)));
        }
        #endregion Tests for computed Functions

        #region Tests for singleton
        [TestMethod]
        public void TestSingletonIdAndEditLink()
        {
            var singletonEntryTypeContext = new TestFeedAndEntryTypeContext
            {
                NavigationSourceName = "Boss",
                NavigationSourceEntityTypeName = "BossType",
                ExpectedEntityTypeName = "BossType",
                UrlConvention = UrlConvention.CreateWithExplicitValue(/*generateKeyAsSegment*/ false),
                NavigationSourceKind = EdmNavigationSourceKind.Singleton,
            };

            var singletonEntryMetadataContext = new TestEntryMetadataContext
            {
                TypeContext = singletonEntryTypeContext,
                Entry = new ODataEntry(),
                ETagProperties = new[] { new KeyValuePair<string, object>("Name", "Value") },
                KeyProperties = this.sinlgeKeyCollection,
                ActualEntityTypeName = "BossType",
                SelectedBindableOperations = new IEdmOperation[0],
                SelectedNavigationProperties = new IEdmNavigationProperty[0],
                SelectedStreamProperties = new Dictionary<string, IEdmStructuralProperty>(),
            };

            var singletonEntityMetadataBuilder = new ODataConventionalEntityMetadataBuilder(singletonEntryMetadataContext, this.metadataContext, this.uriBuilder);
            singletonEntityMetadataBuilder.GetId().Should().Be(new Uri("http://odata.org/base/Boss"));
            singletonEntityMetadataBuilder.GetEditLink().Should().Be(new Uri("http://odata.org/base/Boss"));
            singletonEntityMetadataBuilder.GetReadLink().Should().Be(new Uri("http://odata.org/base/Boss"));
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

    internal class TestEntryMetadataContext : IODataEntryMetadataContext
    {
        public ODataEntry Entry { get; set; }

        public IODataFeedAndEntryTypeContext TypeContext { get; set; }

        public string ActualEntityTypeName { get; set; }

        public ICollection<KeyValuePair<string, object>> KeyProperties { get; set; }

        public IEnumerable<KeyValuePair<string, object>> ETagProperties { get; set; }

        public IEnumerable<IEdmNavigationProperty> SelectedNavigationProperties { get; set; }

        public IDictionary<string, IEdmStructuralProperty> SelectedStreamProperties { get; set; }

        public IEnumerable<IEdmOperation> SelectedBindableOperations { get; set; }
    }

    internal class TestFeedAndEntryTypeContext : IODataFeedAndEntryTypeContext
    {
        public string NavigationSourceName { get; set; }

        public string NavigationSourceEntityTypeName { get; set; }

        public string NavigationSourceFullTypeName { get; set; }

        public string ExpectedEntityTypeName { get; set; }

        public bool IsMediaLinkEntry { get; set; }

        public UrlConvention UrlConvention { get; set; }

        public EdmNavigationSourceKind NavigationSourceKind { get; set; }

        public bool IsFromCollection { get; set; }
    }
}

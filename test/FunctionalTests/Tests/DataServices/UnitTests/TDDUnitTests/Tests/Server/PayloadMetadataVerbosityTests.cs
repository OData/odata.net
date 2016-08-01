//---------------------------------------------------------------------
// <copyright file="PayloadMetadataVerbosityTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Service;
using Microsoft.OData.Service.Caching;
using Microsoft.OData.Service.Providers;
using Microsoft.OData.Service.Serializers;
using System.IO;
using System.Linq;
using System.Text;
using AstoriaUnitTests.TDD.Tests.Server.Simulators;
using AstoriaUnitTests.Tests.Server.Simulators;
using Microsoft.OData.Edm;
using Microsoft.OData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using FluentAssertions.Primitives;

namespace AstoriaUnitTests.TDD.Tests.Server
{
    [TestClass]
    public class PayloadMetadataVerbosityTests
    {
        #region fields and constructor
        private readonly PayloadMetadataKind.Entry[] allEntryKinds = Enum.GetValues(typeof(PayloadMetadataKind.Entry)).Cast<PayloadMetadataKind.Entry>().ToArray();
        private readonly PayloadMetadataKind.Stream[] allStreamKinds = Enum.GetValues(typeof(PayloadMetadataKind.Stream)).Cast<PayloadMetadataKind.Stream>().ToArray();
        private readonly PayloadMetadataKind.Navigation[] allNavigationKinds = Enum.GetValues(typeof(PayloadMetadataKind.Navigation)).Cast<PayloadMetadataKind.Navigation>().ToArray();
        private readonly PayloadMetadataKind.Association[] allAssociationKinds = Enum.GetValues(typeof(PayloadMetadataKind.Association)).Cast<PayloadMetadataKind.Association>().ToArray();
        private readonly PayloadMetadataKind.Feed[] allFeedKinds = Enum.GetValues(typeof(PayloadMetadataKind.Feed)).Cast<PayloadMetadataKind.Feed>().ToArray();
        private readonly PayloadMetadataKind.Operation[] allOperationKinds = Enum.GetValues(typeof(PayloadMetadataKind.Operation)).Cast<PayloadMetadataKind.Operation>().ToArray();
        private readonly PayloadMetadataParameterInterpreter defaultInterpreter = new PayloadMetadataParameterInterpreter(ODataFormat.Json, "minimal");
        private readonly PayloadMetadataParameterInterpreter noneInterpreter = new PayloadMetadataParameterInterpreter(ODataFormat.Json, "none");
        private readonly PayloadMetadataParameterInterpreter allInterpreter = new PayloadMetadataParameterInterpreter(ODataFormat.Json, "full");
        private readonly PayloadMetadataPropertyManager noneManager;
        private readonly PayloadMetadataPropertyManager defaultManager;
        private readonly PayloadMetadataPropertyManager allManager;
        private readonly Uri tempUri = new Uri("http://real.org/");
        private const string RelativeNextLink = "foo?$skiptoken=bar";
        private readonly Uri absoluteNextLinkUri;
        private static readonly ResourceType StringResourceType = ResourceType.GetPrimitiveResourceType(typeof(string));
        private static readonly ResourceType ComplexResourceType = new ResourceType(typeof(object), ResourceTypeKind.ComplexType, null, "namespace", "complex", false);

        public PayloadMetadataVerbosityTests()
        {
            this.noneManager = new PayloadMetadataPropertyManager(this.noneInterpreter);
            this.defaultManager = new PayloadMetadataPropertyManager(this.defaultInterpreter);
            this.allManager = new PayloadMetadataPropertyManager(this.allInterpreter);
            this.absoluteNextLinkUri = new Uri(this.tempUri, RelativeNextLink);
        }
        #endregion

        #region initialization/parsing tests
        [TestMethod]
        public void DefaultBehaviorForJsonLight()
        {
            CompareInterpreters(new PayloadMetadataParameterInterpreter(ODataFormat.Json, null), this.defaultInterpreter);
        }

        [TestMethod]
        public void MediaTypeParameterIntegrationTest()
        {
            CompareInterpreters(PayloadMetadataParameterInterpreter.Create(new ODataFormatWithParameters(ODataFormat.Json, "application/json;odata.metadata=full")), this.allInterpreter);
            CompareInterpreters(PayloadMetadataParameterInterpreter.Create(new ODataFormatWithParameters(ODataFormat.Json, "application/json;odata.metadata=minimal")), this.defaultInterpreter);
        }
        #endregion

        #region interpreter tests
        [TestMethod]
        public void AllMetadataShouldBeIncludedForTheAllOption()
        {
            foreach (var kind in this.allEntryKinds)
            {
                if (kind != PayloadMetadataKind.Entry.TypeName)
                {
                    this.allInterpreter.ShouldIncludeEntryMetadata(kind).Should().BeTrue();
                }
            }

            this.allInterpreter.ShouldIncludeEntryTypeName("Derived", "Base").Should().BeTrue();

            foreach (var kind in this.allStreamKinds)
            {
                this.allInterpreter.ShouldIncludeStreamMetadata(kind).Should().BeTrue();
            }

            foreach (var kind in this.allNavigationKinds)
            {
                this.allInterpreter.ShouldIncludeNavigationMetadata(kind).Should().BeTrue();
            }

            foreach (var kind in this.allAssociationKinds)
            {
                this.allInterpreter.ShouldIncludeAssociationMetadata(kind).Should().BeTrue();
            }

            foreach (var kind in this.allFeedKinds)
            {
                this.allInterpreter.ShouldIncludeFeedMetadata(kind).Should().BeTrue();
            }

            foreach (var kind in this.allOperationKinds)
            {
                this.allInterpreter.ShouldIncludeOperationMetadata(kind, null).Should().BeTrue();
                this.allInterpreter.ShouldIncludeOperationMetadata(kind, () => false).Should().BeTrue();
                this.allInterpreter.ShouldIncludeOperationMetadata(kind, () => true).Should().BeTrue();
            }
        }

        [TestMethod]
        public void NoMetadataKindsShouldBeIncludedForTheNoneOption()
        {
            foreach (var kind in this.allEntryKinds)
            {
                if (kind != PayloadMetadataKind.Entry.TypeName)
                {
                    this.noneInterpreter.ShouldIncludeEntryMetadata(kind).Should().BeFalse();
                }
            }

            this.noneInterpreter.ShouldIncludeEntryTypeName("Derived", "Base").Should().BeFalse();

            foreach (var kind in this.allStreamKinds)
            {
                this.noneInterpreter.ShouldIncludeStreamMetadata(kind).Should().BeFalse();
            }

            foreach (var kind in this.allNavigationKinds)
            {
                this.noneInterpreter.ShouldIncludeNavigationMetadata(kind).Should().BeFalse();
            }

            foreach (var kind in this.allAssociationKinds)
            {
                this.noneInterpreter.ShouldIncludeAssociationMetadata(kind).Should().BeFalse();
            }

            foreach (var kind in this.allFeedKinds)
            {
                this.noneInterpreter.ShouldIncludeFeedMetadata(kind).Should().BeFalse();
            }

            foreach (var kind in this.allOperationKinds)
            {
                this.noneInterpreter.ShouldIncludeOperationMetadata(kind, null).Should().BeFalse();
                this.noneInterpreter.ShouldIncludeOperationMetadata(kind, () => false).Should().BeFalse();
                this.noneInterpreter.ShouldIncludeOperationMetadata(kind, () => true).Should().BeFalse();
            }
        }

        [TestMethod]
        public void BaseTypeNameShouldNotBeIncludedForTheDefaultOption()
        {
            this.defaultInterpreter.ShouldIncludeEntryTypeName("Base", "Base").Should().BeFalse();
        }

        [TestMethod]
        public void DerivedTypeNameShouldBeIncludedForTheDefaultOption()
        {
            this.defaultInterpreter.ShouldIncludeEntryTypeName("Derived", "Base").Should().BeTrue();
        }

        [TestMethod]
        public void OtherEntityMetadataKindsShouldNotBeIncludedForTheDefaultOption()
        {
            foreach (var kind in this.allEntryKinds)
            {
                if (kind != PayloadMetadataKind.Entry.TypeName && kind != PayloadMetadataKind.Entry.ETag)
                {
                    this.defaultInterpreter.ShouldIncludeEntryMetadata(kind).Should().BeFalse();
                }
            }
        }

        [TestMethod]
        public void StreamEditLinkShouldNotBeIncludedForTheDefaultOption()
        {
            this.defaultInterpreter.ShouldIncludeStreamMetadata(PayloadMetadataKind.Stream.EditLink).Should().BeFalse();
        }

        [TestMethod]
        public void OtherStreamMetadataShouldBeIncludedForTheDefaultOption()
        {
            foreach (var kind in this.allStreamKinds)
            {
                if (kind != PayloadMetadataKind.Stream.EditLink)
                {
                    this.defaultInterpreter.ShouldIncludeStreamMetadata(kind).Should().BeTrue();
                }
            }
        }

        [TestMethod]
        public void NavigationMetadataKindsShouldNotBeIncludedForTheDefaultOption()
        {
            foreach (var kind in this.allNavigationKinds)
            {
                this.defaultInterpreter.ShouldIncludeNavigationMetadata(kind).Should().BeFalse();
            }
        }

        [TestMethod]
        public void AssociationMetadataKindsShouldNotBeIncludedForTheDefaultOption()
        {
            foreach (var kind in this.allAssociationKinds)
            {
                this.defaultInterpreter.ShouldIncludeAssociationMetadata(kind).Should().BeFalse();
            }
        }

        [TestMethod]
        public void FeedIdShouldNotBeIncludedForTheDefaultOption()
        {
            this.defaultInterpreter.ShouldIncludeFeedMetadata(PayloadMetadataKind.Feed.Id).Should().BeFalse();
        }

        [TestMethod]
        public void OperationMetadataKindsShouldBeNotIncludedForTheDefaultOption()
        {
            foreach (var kind in this.allOperationKinds)
            {
                this.defaultInterpreter.ShouldIncludeOperationMetadata(kind, () => false).Should().BeFalse();
                this.defaultInterpreter.ShouldIncludeOperationMetadata(kind, () => true).Should().BeTrue();
            }
        }

        [TestMethod]
        public void TypeNameAnnotationShouldNotBeIncludedForBasicJsonTypesForTheAllOption()
        {
            string typeNameToWrite;
            CallShouldSpecifyTypeNameAnnotation(this.allInterpreter, new ODataPrimitiveValue("stringValue"), out typeNameToWrite).Should().BeFalse();
            typeNameToWrite.Should().BeNull();

            CallShouldSpecifyTypeNameAnnotation(this.allInterpreter, new ODataPrimitiveValue(42), out typeNameToWrite).Should().BeFalse();
            typeNameToWrite.Should().BeNull();

            CallShouldSpecifyTypeNameAnnotation(this.allInterpreter, new ODataPrimitiveValue(true), out typeNameToWrite).Should().BeFalse();
            typeNameToWrite.Should().BeNull();

            CallShouldSpecifyTypeNameAnnotation(this.allInterpreter, new ODataPrimitiveValue(1.2), out typeNameToWrite).Should().BeFalse();
            typeNameToWrite.Should().BeNull();
        }

        [TestMethod]
        public void TypeNameAnnotationShouldBeIncludedForSpecialDoubleValuesForTheAllOption()
        {
            string typeNameToWrite;
            CallShouldSpecifyTypeNameAnnotation(this.allInterpreter, new ODataPrimitiveValue(Double.NaN), out typeNameToWrite).Should().BeTrue();
            typeNameToWrite.Should().Be("Edm.Double");

            CallShouldSpecifyTypeNameAnnotation(this.allInterpreter, new ODataPrimitiveValue(Double.NegativeInfinity), out typeNameToWrite).Should().BeTrue();
            typeNameToWrite.Should().Be("Edm.Double");

            CallShouldSpecifyTypeNameAnnotation(this.allInterpreter, new ODataPrimitiveValue(Double.PositiveInfinity), out typeNameToWrite).Should().BeTrue();
            typeNameToWrite.Should().Be("Edm.Double");
        }

        [TestMethod]
        public void TypeNameAnnotationShouldBeIncludedForNullVAluesForTheAllOption()
        {
            string typeNameToWrite;
            this.allInterpreter.ShouldSpecifyTypeNameAnnotation(new ODataNullValue(), StringResourceType, out typeNameToWrite).Should().BeTrue();
            typeNameToWrite.Should().Be("Edm.String");

            this.allInterpreter.ShouldSpecifyTypeNameAnnotation(new ODataNullValue(), ComplexResourceType, out typeNameToWrite).Should().BeTrue();
            typeNameToWrite.Should().Be("namespace.complex");
        }

        [TestMethod]
        public void TypeNameAnnotationShouldBeIncludedForNonBasicJsonTypesForTheAllOption()
        {
            string typeNameToWrite;
            CallShouldSpecifyTypeNameAnnotation(this.allInterpreter, new ODataPrimitiveValue(Guid.NewGuid()), out typeNameToWrite).Should().BeTrue();
            typeNameToWrite.Should().Be("Edm.Guid");
        }

        [TestMethod]
        public void TypeNameAnnotationShouldBeIncludedWithNullNameForTheNoneOption()
        {
            string typeNameToWrite;
            CallShouldSpecifyTypeNameAnnotation(this.noneInterpreter, new ODataPrimitiveValue("stringValue"), out typeNameToWrite).Should().BeTrue();
            typeNameToWrite.Should().BeNull();

            CallShouldSpecifyTypeNameAnnotation(this.noneInterpreter, new ODataPrimitiveValue(42), out typeNameToWrite).Should().BeTrue();
            typeNameToWrite.Should().BeNull();

            CallShouldSpecifyTypeNameAnnotation(this.noneInterpreter, new ODataPrimitiveValue(true), out typeNameToWrite).Should().BeTrue();
            typeNameToWrite.Should().BeNull();

            CallShouldSpecifyTypeNameAnnotation(this.noneInterpreter, new ODataPrimitiveValue(1.2), out typeNameToWrite).Should().BeTrue();
            typeNameToWrite.Should().BeNull();

            CallShouldSpecifyTypeNameAnnotation(this.noneInterpreter, new ODataPrimitiveValue(Double.NaN), out typeNameToWrite).Should().BeTrue();
            typeNameToWrite.Should().BeNull();

            CallShouldSpecifyTypeNameAnnotation(this.noneInterpreter, new ODataPrimitiveValue(Double.NegativeInfinity), out typeNameToWrite).Should().BeTrue();
            typeNameToWrite.Should().BeNull();

            CallShouldSpecifyTypeNameAnnotation(this.noneInterpreter, new ODataPrimitiveValue(Double.PositiveInfinity), out typeNameToWrite).Should().BeTrue();
            typeNameToWrite.Should().BeNull();

            CallShouldSpecifyTypeNameAnnotation(this.noneInterpreter, new ODataPrimitiveValue(Guid.NewGuid()), out typeNameToWrite).Should().BeTrue();
            typeNameToWrite.Should().BeNull();

            this.noneInterpreter.ShouldSpecifyTypeNameAnnotation(new ODataNullValue(), StringResourceType, out typeNameToWrite).Should().BeTrue();
            typeNameToWrite.Should().BeNull();

            this.noneInterpreter.ShouldSpecifyTypeNameAnnotation(new ODataNullValue(), ComplexResourceType, out typeNameToWrite).Should().BeTrue();
            typeNameToWrite.Should().BeNull();
        }

        [TestMethod]
        public void TypeNameAnnotationShouldNotBeIncludedForTheDefaultOption()
        {
            string typeNameToWrite;
            CallShouldSpecifyTypeNameAnnotation(this.defaultInterpreter, new ODataPrimitiveValue("stringValue"), out typeNameToWrite).Should().BeFalse();
            typeNameToWrite.Should().BeNull();

            CallShouldSpecifyTypeNameAnnotation(this.defaultInterpreter, new ODataPrimitiveValue(42), out typeNameToWrite).Should().BeFalse();
            typeNameToWrite.Should().BeNull();

            CallShouldSpecifyTypeNameAnnotation(this.defaultInterpreter, new ODataPrimitiveValue(true), out typeNameToWrite).Should().BeFalse();
            typeNameToWrite.Should().BeNull();

            CallShouldSpecifyTypeNameAnnotation(this.defaultInterpreter, new ODataPrimitiveValue(1.2), out typeNameToWrite).Should().BeFalse();
            typeNameToWrite.Should().BeNull();

            CallShouldSpecifyTypeNameAnnotation(this.defaultInterpreter, new ODataPrimitiveValue(Double.NaN), out typeNameToWrite).Should().BeFalse();
            typeNameToWrite.Should().BeNull();

            CallShouldSpecifyTypeNameAnnotation(this.defaultInterpreter, new ODataPrimitiveValue(Double.NegativeInfinity), out typeNameToWrite).Should().BeFalse();
            typeNameToWrite.Should().BeNull();

            CallShouldSpecifyTypeNameAnnotation(this.defaultInterpreter, new ODataPrimitiveValue(Double.PositiveInfinity), out typeNameToWrite).Should().BeFalse();
            typeNameToWrite.Should().BeNull();

            CallShouldSpecifyTypeNameAnnotation(this.defaultInterpreter, new ODataPrimitiveValue(Guid.NewGuid()), out typeNameToWrite).Should().BeFalse();
            typeNameToWrite.Should().BeNull();

            this.defaultInterpreter.ShouldSpecifyTypeNameAnnotation(new ODataNullValue(), StringResourceType, out typeNameToWrite).Should().BeFalse();
            typeNameToWrite.Should().BeNull();

            this.defaultInterpreter.ShouldSpecifyTypeNameAnnotation(new ODataNullValue(), ComplexResourceType, out typeNameToWrite).Should().BeFalse();
            typeNameToWrite.Should().BeNull();
        }

        [TestMethod]
        public void ShouldNotSetAbsoluteNextLinkUriForAllIntepreter()
        {
            this.allInterpreter.ShouldNextPageLinkBeAbsolute().Should().BeFalse();
        }

        [TestMethod]
        public void ShouldNotSetAbsoluteNextLinkUriForDefaultIntepreter()
        {
            this.defaultInterpreter.ShouldNextPageLinkBeAbsolute().Should().BeFalse();
        }
        #endregion

        #region entry object model integration tests
        [TestMethod]
        public void EntryETagShouldBeWrittenByDefault()
        {
            TestEntry(e => this.defaultManager.SetETag(e, () => "shouldBeSet"), e => e.ETag, "shouldBeSet");
        }

        [TestMethod]
        public void EntryIdShouldNotBeWrittenByDefault()
        {
            TestEntry(e => this.defaultManager.SetId(e, () => { throw new Exception(); }), e => e.Id, null);
        }

        [TestMethod]
        public void EntryTypeShouldNotBeWrittenForBaseTypeByDefault()
        {
            const string entitySetBaseTypeName = "baseType";
            const string entryTypeName = "baseType";
            this.defaultInterpreter.ShouldIncludeEntryTypeName(entitySetBaseTypeName, entryTypeName).Should().BeFalse();
        }

        [TestMethod]
        public void EntryShouldHaveTypeNameAnnotationWithNullTypeNameForBaseTypeByDefault()
        {
            const string entitySetBaseTypeName = "baseType";
            const string entryTypeName = "baseType";
            var entry = new ODataResource();
            this.defaultManager.SetTypeName(entry, entitySetBaseTypeName, entryTypeName);
            entry.TypeAnnotation.TypeName.Should().BeNull();
            entry.TypeName.Should().Be(entryTypeName);
        }

        [TestMethod]
        public void EntryTypeShouldBeWrittenForDerivedTypeByDefault()
        {
            const string entitySetBaseTypeName = "baseType";
            const string entryTypeName = "derivedType";
            this.defaultInterpreter.ShouldIncludeEntryTypeName(entitySetBaseTypeName, entryTypeName).Should().BeTrue();
        }

        public void EntryShouldHaveTypeNameAnnotationWithTypeNameForDerivedTypeByDefault()
        {
            const string entitySetBaseTypeName = "baseType";
            const string entryTypeName = "derivedType";
            var entry = new ODataResource();
            this.defaultManager.SetTypeName(entry, entitySetBaseTypeName, entryTypeName);
            entry.TypeAnnotation.TypeName.Should().Be(entryTypeName);
            entry.TypeName.Should().Be(entryTypeName);
        }

        [TestMethod]
        public void EntryEditLinkShouldNotBeWrittenByDefault()
        {
            TestEntry(e => this.defaultManager.SetEditLink(e, () => { throw new Exception(); }), e => e.EditLink, null);
        }

        [TestMethod]
        public void EntryETagShouldBeWrittenForAllOption()
        {
            TestEntry(e => this.allManager.SetETag(e, () => "shouldBeSet"), e => e.ETag, "shouldBeSet");
        }

        [TestMethod]
        public void EntryIdShouldBeWrittenForAllOption()
        {
            TestEntry(e => this.allManager.SetId(e, () => this.tempUri), e => e.Id, this.tempUri);
        }

        [TestMethod]
        public void EntryTypeShouldBeWrittenForBaseTypeForAllOption()
        {
            const string entitySetBaseTypeName = "baseType";
            const string entryTypeName = "baseType";
            this.allInterpreter.ShouldIncludeEntryTypeName(entitySetBaseTypeName, entryTypeName).Should().BeTrue();
        }

        public void EntryShouldHaveTypeNameAnnotationWithTypeNameForBaseTypeForAllOption()
        {
            const string entitySetBaseTypeName = "baseType";
            const string entryTypeName = "baseType";
            var entry = new ODataResource();
            this.allManager.SetTypeName(entry, entitySetBaseTypeName, entryTypeName);
            entry.TypeAnnotation.TypeName.Should().Be(entryTypeName);
            entry.TypeName.Should().Be(entryTypeName);
        }

        [TestMethod]
        public void EntryTypeShouldBeWrittenForDerivedTypeForAllOption()
        {
            const string entitySetBaseTypeName = "baseType";
            const string entryTypeName = "derivedType";
            this.allInterpreter.ShouldIncludeEntryTypeName(entitySetBaseTypeName, entryTypeName).Should().BeTrue();
        }

        public void EntryShouldHaveTypeNameAnnotationWithTypeNameForDerivedTypeForAllOption()
        {
            const string entitySetBaseTypeName = "baseType";
            const string entryTypeName = "derivedType";
            var entry = new ODataResource();
            this.allManager.SetTypeName(entry, entitySetBaseTypeName, entryTypeName);
            entry.TypeAnnotation.TypeName.Should().Be(entryTypeName);
            entry.TypeName.Should().Be(entryTypeName);
        }

        [TestMethod]
        public void EntryEditLinkShouldBeWrittenForAllOption()
        {
            TestEntry(e => this.allManager.SetEditLink(e, () => this.tempUri), e => e.EditLink, this.tempUri);
        }

        [TestMethod]
        public void EntryETagShouldNotBeWrittenForTheNoneOption()
        {
            TestEntry(e => this.noneManager.SetETag(e, () => { throw new Exception(); }), e => e.ETag, null);
        }

        [TestMethod]
        public void EntryIdShouldNotBeWrittenForTheNoneOption()
        {
            TestEntry(e => this.noneManager.SetId(e, () => { throw new Exception(); }), e => e.Id, null);
        }

        [TestMethod]
        public void EntryTypeShouldNotBeWrittenForBaseTypeForTheNoneOption()
        {
            var entry = new ODataResource();
            this.noneManager.SetTypeName(entry, "baseType", "baseType");
            entry.TypeAnnotation.TypeName.Should().BeNull(); // annotation must be set with null type name
            entry.TypeName.Should().Be("baseType");
        }

        [TestMethod]
        public void EntryTypeShouldNotBeWrittenForDerivedTypeForTheNoneOption()
        {
            var entry = new ODataResource();
            this.noneManager.SetTypeName(entry, "baseType", "derivedType");
            entry.TypeAnnotation.TypeName.Should().BeNull(); // annotation must be set with null type name
            entry.TypeName.Should().Be("derivedType");
        }

        [TestMethod]
        public void EntryEditLinkShouldNotBeWrittenForTheNoneOption()
        {
            TestEntry(e => this.noneManager.SetEditLink(e, () => { throw new Exception(); }), e => e.EditLink, null);
        }
        #endregion

        #region stream object model integration tests
        [TestMethod]
        public void StreamEditLinkShouldNotBeWrittenByDefault()
        {
            TestStream(s => this.defaultManager.SetEditLink(s, () => { throw new Exception(); }), s => s.EditLink, null);
        }

        [TestMethod]
        public void StreamContentTypeShouldBeWrittenByDefault()
        {
            TestStream(s => this.defaultManager.SetContentType(s, "application/real"), s => s.ContentType, "application/real");
        }

        [TestMethod]
        public void StreamETagShouldBeWrittenByDefault()
        {
            TestStream(s => this.defaultManager.SetETag(s, "shouldBeWritten"), s => s.ETag, "shouldBeWritten");
        }

        [TestMethod]
        public void StreamReadLinkShouldBeWrittenByDefault()
        {
            TestStream(s => this.defaultManager.SetReadLink(s, () => this.tempUri), s => s.ReadLink, this.tempUri);
        }

        [TestMethod]
        public void StreamEditLinkShouldBeWrittenForAllOption()
        {
            TestStream(s => this.allManager.SetEditLink(s, () => this.tempUri), s => s.EditLink, this.tempUri);
        }

        [TestMethod]
        public void StreamContentTypeShouldBeWrittenForAllOption()
        {
            TestStream(s => this.allManager.SetContentType(s, "application/real"), s => s.ContentType, "application/real");
        }

        [TestMethod]
        public void StreamETagShouldBeWrittenForAllOption()
        {
            TestStream(s => this.allManager.SetETag(s, "shouldBeWritten"), s => s.ETag, "shouldBeWritten");
        }

        [TestMethod]
        public void StreamReadLinkShouldBeWrittenForAllOption()
        {
            TestStream(s => this.allManager.SetReadLink(s, () => this.tempUri), s => s.ReadLink, this.tempUri);
        }

        [TestMethod]
        public void StreamEditLinkShouldNotBeWrittenForTheNoneOption()
        {
            TestStream(s => this.noneManager.SetEditLink(s, () => { throw new Exception(); }), s => s.EditLink, null);
        }

        [TestMethod]
        public void StreamETagShouldNotBeWrittenForTheNoneOption()
        {
            TestStream(s => this.noneManager.SetETag(s, "shouldNotBeWritten"), s => s.ETag, null);
        }

        [TestMethod]
        public void StreamContentTypeShouldNotBeWrittenForTheNoneOption()
        {
            TestStream(s => this.noneManager.SetContentType(s, "application/fake"), s => s.ContentType, null);
        }

        [TestMethod]
        public void StreamReadLinkShouldNotBeWrittenForTheNoneOption()
        {
            TestStream(s => this.noneManager.SetReadLink(s, () => { throw new Exception(); }), s => s.ReadLink, null);
        }

        #endregion

        #region navigation object model integration tests

        [TestMethod]
        public void NavigationLinkUrlShouldNotBeWrittenByDefault()
        {
            TestNavigationLink(l => this.defaultManager.SetUrl(l, () => { throw new Exception(); }), l => l.Url, null);
        }

        [TestMethod]
        public void NavigationLinkAssociationLinkUrlShouldNotBeWrittenByDefault()
        {
            TestNavigationLink(l => this.defaultManager.SetAssociationLinkUrl(l, () => { throw new Exception(); }), l => l.AssociationLinkUrl, null);
        }

        [TestMethod]
        public void NavigationLinkUrlShouldBeWrittenForAllOption()
        {
            TestNavigationLink(l => this.allManager.SetUrl(l, () => this.tempUri), l => l.Url, this.tempUri);
        }

        [TestMethod]
        public void NavigationLinkAssociationLinkUrlShouldBeWrittenForAllOption()
        {
            TestNavigationLink(l => this.allManager.SetAssociationLinkUrl(l, () => this.tempUri), l => l.AssociationLinkUrl, this.tempUri);
        }

        [TestMethod]
        public void NavigationLinkUrlShouldNotBeWrittenForNoneOption()
        {
            TestNavigationLink(l => this.noneManager.SetUrl(l, () => { throw new Exception(); }), l => l.Url, null);
        }

        [TestMethod]
        public void NavigationLinkAssociationLinkUrlShouldNotBeWrittenForNoneOption()
        {
            TestNavigationLink(l => this.noneManager.SetAssociationLinkUrl(l, () => { throw new Exception(); }), l => l.AssociationLinkUrl, null);
        }
        #endregion

        #region feed object model integration tests
        [TestMethod]
        public void FeedIdShouldNotBeWrittenByDefault()
        {
            TestFeed(f => this.defaultManager.SetId(f, () => { throw new Exception(); }), l => l.Id, null);
        }

        [TestMethod]
        public void FeedIdShouldBeWrittenForAllOption()
        {
            TestFeed(f => this.allManager.SetId(f, () => new Uri("http://shouldBeWritten")), l => l.Id, new Uri("http://shouldBeWritten"));
        }

        [TestMethod]
        public void FeedIdShouldNotBeWrittenForTheNoneOption()
        {
            TestFeed(f => this.noneManager.SetId(f, () => { throw new Exception(); }), l => l.Id, null);
        }

        [TestMethod]
        public void FeedNextLinkShouldBeRelativeForAllMetadata()
        {
            ODataResourceSet feed = new ODataResourceSet();
            this.allManager.SetNextPageLink(feed, this.tempUri, this.absoluteNextLinkUri);
            feed.NextPageLink.OriginalString.Should().Be(RelativeNextLink);
        }

        [TestMethod]
        public void FeedNextLinkShouldBeRelativeForMinimalMetadata()
        {
            ODataResourceSet feed = new ODataResourceSet();
            this.defaultManager.SetNextPageLink(feed, this.tempUri, this.absoluteNextLinkUri);
            feed.NextPageLink.OriginalString.Should().Be(RelativeNextLink);
        }

        [TestMethod]
        public void FeedNextLinkShouldBeAbsoluteForNoMetadata()
        {
            ODataResourceSet feed = new ODataResourceSet();
            this.noneManager.SetNextPageLink(feed, this.tempUri, this.absoluteNextLinkUri);
            feed.NextPageLink.Should().BeSameAs(this.absoluteNextLinkUri);
        }
        #endregion

        #region primitive value object model integration tests
        [TestMethod]
        public void PrimitiveValueTypeNameAnnotationShouldNotBeSpecifiedByDefault()
        {
            TestPrimitive("stringValue", (p, t) => this.defaultManager.SetTypeName(p, t), null);
            TestPrimitive(42, (p, t) => this.defaultManager.SetTypeName(p, t), null);
            TestPrimitive(true, (p, t) => this.defaultManager.SetTypeName(p, t), null);
            TestPrimitive(1.2, (p, t) => this.defaultManager.SetTypeName(p, t), null);
            TestPrimitive(Double.NaN, (p, t) => this.defaultManager.SetTypeName(p, t), null);
            TestPrimitive(Double.NegativeInfinity, (p, t) => this.defaultManager.SetTypeName(p, t), null);
            TestPrimitive(Double.PositiveInfinity, (p, t) => this.defaultManager.SetTypeName(p, t), null);
            TestPrimitive(Guid.NewGuid(), (p, t) => this.defaultManager.SetTypeName(p, t), null);
        }

        [TestMethod]
        public void NullValueTypeNameAnnotationShouldNotBeSpecifiedByDefault()
        {
            ODataNullValue nullValue = new ODataNullValue();
            this.defaultManager.SetTypeName(nullValue, StringResourceType);
            CompareSerializationTypeNameAnnotation(nullValue, null);

            nullValue = new ODataNullValue();
            this.defaultManager.SetTypeName(nullValue, ComplexResourceType);
            CompareSerializationTypeNameAnnotation(nullValue, null);
        }

        [TestMethod]
        public void PrimitiveValueTypeNameShouldNotBeWrittenForNoneOption()
        {
            TestPrimitive("stringValue", (p, t) => this.noneManager.SetTypeName(p, t), new ODataTypeAnnotation());
            TestPrimitive(42, (p, t) => this.noneManager.SetTypeName(p, t), new ODataTypeAnnotation());
            TestPrimitive(true, (p, t) => this.noneManager.SetTypeName(p, t), new ODataTypeAnnotation());
            TestPrimitive(1.2, (p, t) => this.noneManager.SetTypeName(p, t), new ODataTypeAnnotation());
            TestPrimitive(Double.NaN, (p, t) => this.noneManager.SetTypeName(p, t), new ODataTypeAnnotation());
            TestPrimitive(Double.NegativeInfinity, (p, t) => this.noneManager.SetTypeName(p, t), new ODataTypeAnnotation());
            TestPrimitive(Double.PositiveInfinity, (p, t) => this.noneManager.SetTypeName(p, t), new ODataTypeAnnotation());
            TestPrimitive(Guid.NewGuid(), (p, t) => this.noneManager.SetTypeName(p, t), new ODataTypeAnnotation());
        }

        [TestMethod]
        public void NullValueTypeNameShouldNotBeWrittenForNoneOption()
        {
            ODataNullValue nullValue = new ODataNullValue();
            this.noneManager.SetTypeName(nullValue, StringResourceType);
            CompareSerializationTypeNameAnnotation(nullValue, new ODataTypeAnnotation());

            nullValue = new ODataNullValue();
            this.noneManager.SetTypeName(nullValue, ComplexResourceType);
            CompareSerializationTypeNameAnnotation(nullValue, new ODataTypeAnnotation());
        }

        [TestMethod]
        public void PrimitiveValueTypeNameShouldNotBeWrittenForAllOptionForBasicJsonTypes()
        {
            TestPrimitive("stringValue", (p, t) => this.allManager.SetTypeName(p, t), null);
            TestPrimitive(42, (p, t) => this.allManager.SetTypeName(p, t), null);
            TestPrimitive(true, (p, t) => this.allManager.SetTypeName(p, t), null);
            TestPrimitive(1.2, (p, t) => this.allManager.SetTypeName(p, t), null);
        }

        [TestMethod]
        public void PrimitiveValueTypeNameShouldBeWrittenForAllOptionForSpecialDoubleValues()
        {
            TestPrimitive(Double.NaN, (p, t) => this.allManager.SetTypeName(p, t), new ODataTypeAnnotation("Edm.Double"));
            TestPrimitive(Double.NegativeInfinity, (p, t) => this.allManager.SetTypeName(p, t), new ODataTypeAnnotation("Edm.Double"));
            TestPrimitive(Double.PositiveInfinity, (p, t) => this.allManager.SetTypeName(p, t), new ODataTypeAnnotation("Edm.Double"));
        }

        [TestMethod]
        public void PrimitiveValueTypeNameShouldBeWrittenForAllOptionForNonBasicJsonTypes()
        {
            TestPrimitive(Guid.NewGuid(), (p, t) => this.allManager.SetTypeName(p, t), new ODataTypeAnnotation("Edm.Guid"));
        }

        [TestMethod]
        public void NullValueTypeNameShouldBeWrittenForAllOption()
        {
            ODataNullValue nullValue = new ODataNullValue();
            this.allManager.SetTypeName(nullValue, StringResourceType);
            CompareSerializationTypeNameAnnotation(nullValue, new ODataTypeAnnotation("Edm.String"));

            nullValue = new ODataNullValue();
            this.allManager.SetTypeName(nullValue, ComplexResourceType);
            CompareSerializationTypeNameAnnotation(nullValue, new ODataTypeAnnotation("namespace.complex"));
        }
        #endregion primitive value object model integration tests

        #region collection value object model integration tests
        [TestMethod]
        public void CollectionTypeNameShouldNotBeWrittenByDefault()
        {
            // We leave it up to ODataLib to decide whether or not to write the type name in default case.
            TestCollection((c, t) => this.defaultManager.SetTypeName(c, t), null);
        }

        [TestMethod]
        public void CollectionTypeNameShouldBeWrittenForAllOption()
        {
            TestCollection((c, t) => this.allManager.SetTypeName(c, t), new ODataTypeAnnotation("Collection(Edm.String)"));
        }

        [TestMethod]
        public void CollectionTypeNameShouldNotBeWrittenForTheNoneOption()
        {
            TestCollection((c, t) => this.noneManager.SetTypeName(c, t), new ODataTypeAnnotation());
        }

        [TestMethod]
        public void DynamicCollectionTypeNameShouldBeWrittenByDefault()
        {
            TestCollection((c, t) => this.defaultManager.SetTypeName(c, t), null);
        }

        [TestMethod]
        public void DynamicCollectionTypeNameShouldBeWrittenForAllOption()
        {
            TestCollection((c, t) => this.allManager.SetTypeName(c, t), new ODataTypeAnnotation("Collection(Edm.String)"));
        }
        #endregion

        #region action object model integration tests
        [TestMethod]
        public void ShouldIncludeAlwaysAvailableOperationMetadataWhenMetadataQueryOptionIsAll()
        {
            this.allInterpreter.ShouldIncludeAlwaysAvailableOperation().Should().BeTrue();
        }

        [TestMethod]
        public void ShouldNotIncludeAlwaysAvailableOperationMetadataWhenMetadataQueryOptionIsNone()
        {
            this.noneInterpreter.ShouldIncludeAlwaysAvailableOperation().Should().BeFalse();
        }

        [TestMethod]
        public void ShouldNotIncludeAlwaysAvailableOperationMetadataWhenMetadataQueryOptionIsDefault()
        {
            this.defaultInterpreter.ShouldIncludeAlwaysAvailableOperation().Should().BeFalse();
        }

        [TestMethod]
        public void ActionTitleShouldNotBeSetByDefaultIfAlwaysAdvertised()
        {
            TestAction(a => this.defaultManager.SetTitle(a, true, "shouldNotBeWritten"), l => l.Title, null);
        }

        [TestMethod]
        public void ActionTitleShouldBeSetByDefaultIfNotAlwaysAdvertised()
        {
            TestAction(a => this.defaultManager.SetTitle(a, false, "shouldBeWritten"), l => l.Title, "shouldBeWritten");
        }

        [TestMethod]
        public void ActionTargetShouldNotBeSetByDefaultIfAlwaysAdvertised()
        {
            TestAction(a => this.defaultManager.SetTarget(a, true, () => { throw new Exception(); }), l => l.Target, null);
        }

        [TestMethod]
        public void ActionTargetShouldBeSetByDefaultIfNotAlwaysAdvertised()
        {
            TestAction(a => this.defaultManager.SetTarget(a, false, () => this.tempUri), l => l.Target, this.tempUri);
        }

        [TestMethod]
        public void ActionTitleShouldNotBeClearedByDefaultIfValueChanges()
        {
            TestAction(
                a =>
                {
                    a.Title = "bar";
                    this.defaultManager.CheckForUnmodifiedTitle(a, "foo");
                },
                l => l.Title,
                "bar");
        }

        [TestMethod]
        public void ActionTitleShouldBeClearedByDefaultIfUnchanged()
        {
            TestAction(
                a =>
                {
                    a.Title = "foo";
                    this.defaultManager.CheckForUnmodifiedTitle(a, "foo");
                },
                l => l.Title,
                null);
        }

        [TestMethod]
        public void ActionTargetShouldNotBeClearedByDefaultIfValueChanges()
        {
            var customUri = new Uri("http://custom.org/");
            TestAction(
                a =>
                {
                    a.Target = customUri;
                    this.defaultManager.CheckForUnmodifiedTarget(a, () => this.tempUri);
                },
                l => l.Target,
                customUri);
        }

        [TestMethod]
        public void ActionTargetShouldBeClearedByDefaultIfUnchanged()
        {
            TestAction(
                a =>
                {
                    a.Target = this.tempUri;
                    this.defaultManager.CheckForUnmodifiedTarget(a, () => this.tempUri);
                },
                l => l.Target,
                null);
        }

        [TestMethod]
        public void ActionTitleShouldBeSetForAllOption()
        {
            TestAction(a => this.allManager.SetTitle(a, true, "shouldBeSet"), l => l.Title, "shouldBeSet");
        }

        [TestMethod]
        public void ActionTargetShouldBeSetForAllOption()
        {
            TestAction(a => this.allManager.SetTarget(a, true, () => this.tempUri), l => l.Target, this.tempUri);
        }

        [TestMethod]
        public void ActionTitleShouldNotBeClearedForAllOption()
        {
            TestAction(
                a =>
                {
                    a.Title = "foo";
                    this.allManager.CheckForUnmodifiedTitle(a, "foo");
                },
                l => l.Title,
                "foo");
        }

        [TestMethod]
        public void ActionTargetShouldNotBeClearedForAllOption()
        {
            TestAction(
                a =>
                {
                    a.Target = this.tempUri;
                    this.allManager.CheckForUnmodifiedTarget(a, () => { throw new Exception(); });
                },
                l => l.Target,
                this.tempUri);
        }

        [TestMethod]
        public void ActionTitleShouldBeSetForNoneOptionIfAlwaysAdvertised()
        {
            TestAction(a => this.noneManager.SetTitle(a, true, "shouldNotBeSet"), l => l.Title, null);
        }

        [TestMethod]
        public void ActionTitleShouldBeSetForNoneOptionIfNotAlwaysAdvertised()
        {
            TestAction(a => this.noneManager.SetTitle(a, false, "shouldBeSet"), l => l.Title, "shouldBeSet");
        }

        [TestMethod]
        public void ActionTargetShouldNotBeSetForNoneOptionIfAlwaysAdvertised()
        {
            TestAction(a => this.noneManager.SetTarget(a, true, () => { throw new Exception(); }), l => l.Target, null);
        }

        [TestMethod]
        public void ActionTargetShouldBeSetForNoneOptionIfNotAlwaysAdvertised()
        {
            TestAction(a => this.noneManager.SetTarget(a, false, () => this.tempUri), l => l.Target, this.tempUri);
        }

        [TestMethod]
        public void ActionTitleShouldBeClearedForNoneOptionIfValueChanges()
        {
            TestAction(
                a =>
                {
                    a.Title = "bar";
                    this.noneManager.CheckForUnmodifiedTitle(a, "foo");
                },
                l => l.Title,
                null);
        }

        [TestMethod]
        public void ActionTitleShouldBeClearedForNoneOptionIfUnchanged()
        {
            TestAction(
                a =>
                {
                    a.Title = "foo";
                    this.noneManager.CheckForUnmodifiedTitle(a, "foo");
                },
                l => l.Title,
                null);
        }

        [TestMethod]
        public void ActionTargetShouldBeClearedForNoneOptionIfValueChanges()
        {
            var customUri = new Uri("http://custom.org/");
            TestAction(
                a =>
                {
                    a.Target = customUri;
                    this.noneManager.CheckForUnmodifiedTarget(a, () => this.tempUri);
                },
                l => l.Target,
                null);
        }

        [TestMethod]
        public void ActionTargetShouldBeClearedForNoneOptionIfUnchanged()
        {
            TestAction(
                a =>
                {
                    a.Target = this.tempUri;
                    this.noneManager.CheckForUnmodifiedTarget(a, () => this.tempUri);
                },
                l => l.Target,
                null);
        }

        #endregion

        #region enum sanity checks
        [TestMethod]
        public void EntityMetadataKindsShouldMatchProperties()
        {
            foreach (var kind in this.allEntryKinds)
            {
                (typeof(ODataResource).GetProperty(kind.ToString()) as object).Should().NotBeNull();
            }
        }

        [TestMethod]
        public void FeedMetadataKindsShouldMatchProperties()
        {
            foreach (var kind in this.allFeedKinds)
            {
                (typeof(ODataResourceSet).GetProperty(kind.ToString()) as object).Should().NotBeNull();
            }
        }

        [TestMethod]
        public void StreamMetadataKindsShouldMatchProperties()
        {
            foreach (var kind in this.allStreamKinds)
            {
                (typeof(ODataStreamReferenceValue).GetProperty(kind.ToString()) as object).Should().NotBeNull();
            }
        }

        [TestMethod]
        public void NavigationMetadataKindsShouldMatchProperties()
        {
            foreach (var kind in this.allNavigationKinds)
            {
                (typeof(ODataNestedResourceInfo).GetProperty(kind.ToString()) as object).Should().NotBeNull();
            }
        }

        [TestMethod]
        public void OperationMetadataKindsShouldMatchProperties()
        {
            foreach (var kind in this.allOperationKinds)
            {
                (typeof(ODataOperation).GetProperty(kind.ToString()) as object).Should().NotBeNull();
            }
        }
        #endregion

        #region long span integration tests
        [TestMethod]
        public void WritingFeedInNoMetadataModeShouldMatchExpectedPayload()
        {
            WriteFeedAndVerifyPayload(noMetadata: true);
        }

        [TestMethod]
        public void WritingFeedInMinimalMetadataModeShouldMatchExpectedPayload()
        {
            WriteFeedAndVerifyPayload(noMetadata: false);
        }

        private void WriteFeedAndVerifyPayload(bool noMetadata)
        {
            string expectedPayload =
                "{" +
                (noMetadata ? "" : "\"@odata.context\":\"http://fake.org/$metadata#Fake\",") +
                    "\"value\":" +
                    "[" +
                        "{" +
                            "\"DynamicPrimitive\":3," +
                            "\"DynamicNull\":null," +
                            "\"Complex\":{}," +
                            "\"DynamicComplex\":{}" +
                            (noMetadata
                            ? "" :
                            ",\"#Action\":" +
                                "{" +
                                    "\"target\":\"http://real.org/Action\"" +
                                "}") +
                        "}" +
                    "]" +
                "}";

            Action<IEdmEntitySet, ODataMessageWriter> write = (entitySet, writer) =>
            {
                var feedWriter = writer.CreateODataResourceSetWriter(entitySet);
                var feed = new ODataResourceSet();
                feedWriter.WriteStart(feed);

                // ODL requires type name on dynamic complex values, but we can still omit it from the payload using an annotation
                ODataResource dynamicComplex = new ODataResource { TypeName = "Fake.Complex", };
                dynamicComplex.TypeAnnotation = new ODataTypeAnnotation();

                var entry = new ODataResource
                {
                    MediaResource = new ODataStreamReferenceValue(),
                    Properties = new[]
                    {
                        new ODataProperty { Name = "Thumbnail", Value = new ODataStreamReferenceValue() },
                        new ODataProperty { Name = "DynamicPrimitive", Value = 3 },
                        new ODataProperty { Name = "DynamicNull", Value = null }
                    }
                };
                entry.AddAction(new ODataAction { Metadata = new Uri("http://fake.org/$metadata#Action"), Target = new Uri("http://real.org/Action") });

                feedWriter.WriteStart(entry);

                var complexP = new ODataNestedResourceInfo { Name = "Complex" };
                var complex = new ODataResource();
                feedWriter.WriteStart(complexP);
                feedWriter.WriteStart(complex);
                feedWriter.WriteEnd();
                feedWriter.WriteEnd();

                var dynamicComplexP = new ODataNestedResourceInfo { Name = "DynamicComplex" };
                feedWriter.WriteStart(dynamicComplexP);
                feedWriter.WriteStart(dynamicComplex);
                feedWriter.WriteEnd();
                feedWriter.WriteEnd();

                var navigation = new ODataNestedResourceInfo { Name = "Navigation" };
                feedWriter.WriteStart(navigation);
                feedWriter.WriteEnd();
                feedWriter.WriteEnd();
                feedWriter.WriteEnd();
            };

            RunODataLibIntegrationTest(expectedPayload, write, noMetadata);
        }

        [TestMethod]
        public void RequestQueryProcessorIntegrationTest()
        {
            DataServiceHostSimulator host = new DataServiceHostSimulator
            {
                RequestAccept = "application/json;odata.metadata=none",
                RequestMaxVersion = "4.0",
                RequestVersion = "4.0",
                RequestHttpMethod = "GET",
                AbsoluteServiceUri = new Uri("http://fake.org/service/"),
                AbsoluteRequestUri = new Uri("http://fake.org/service/"),
            };

            DataServiceProviderSimulator provider = new DataServiceProviderSimulator();

            var resourceType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "SelectTestNamespace", "Fake", false) { CanReflectOnInstanceType = false, IsOpenType = true };
            resourceType.AddProperty(new ResourceProperty("Id", ResourcePropertyKind.Key | ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(int))) { CanReflectOnInstanceTypeProperty = false });
            var resourceSet = new ResourceSet("FakeSet", resourceType);
            resourceSet.SetReadOnly();

            provider.AddResourceSet(resourceSet);

            var configuration = new DataServiceConfiguration(provider);
            configuration.SetEntitySetAccessRule("*", EntitySetRights.All);

            DataServiceSimulator service = new DataServiceSimulator
            {
                OperationContext = new DataServiceOperationContext(host),
                Configuration = configuration,
            };

            DataServiceStaticConfiguration staticConfiguration = new DataServiceStaticConfiguration(service.Instance.GetType(), provider);
            IDataServiceProviderBehavior providerBehavior = DataServiceProviderBehavior.CustomDataServiceProviderBehavior;

            service.ProcessingPipeline = new DataServiceProcessingPipeline();
            service.Provider = new DataServiceProviderWrapper(
                new DataServiceCacheItem(
                    service.Configuration,
                    staticConfiguration),
                provider,
                provider,
                service,
                false);

            service.ActionProvider = DataServiceActionProviderWrapper.Create(service);

            service.Provider.ProviderBehavior = providerBehavior;
            service.Configuration.DataServiceBehavior.MaxProtocolVersion = Microsoft.OData.Client.ODataProtocolVersion.V4;
            service.OperationContext.InitializeAndCacheHeaders(service);
            service.OperationContext.RequestMessage.InitializeRequestVersionHeaders(Microsoft.OData.Client.ODataProtocolVersion.V4.ToVersion());

            var description = RequestUriProcessor.ProcessRequestUri(new Uri("http://fake.org/service/"), service, false);
            CompareInterpreters(description.PayloadMetadataParameterInterpreter, this.noneInterpreter);
        }
        #endregion

        #region helper methods and classes

        private static void CompareInterpreters(PayloadMetadataParameterInterpreter actual, PayloadMetadataParameterInterpreter expected)
        {
            expected.IsEquivalentTo(actual).Should().BeTrue();
        }

        private static void RunODataLibIntegrationTest(string expectedPayload, Action<IEdmEntitySet, ODataMessageWriter> write, bool noMetadataMode)
        {
            var model = new EdmModel();
            var type = new EdmEntityType("Fake", "Fake", null, false, true, true);
            type.AddProperty(new EdmStructuralProperty(type, "Thumbnail", EdmCoreModel.Instance.GetStream(true)));

            var complexType = new EdmComplexType("Fake", "Complex");
            type.AddProperty(new EdmStructuralProperty(type, "Complex", new EdmComplexTypeReference(complexType, true)));
            type.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "Navigation", Target = type, TargetMultiplicity = EdmMultiplicity.ZeroOrOne });

            var container = new EdmEntityContainer("Fake", "Fake");
            var entitySet = new EdmEntitySet(container, "Fake", type);

            model.AddElement(container);
            model.AddElement(type);
            model.AddElement(complexType);
            container.AddElement(entitySet);

            var message = new ODataResponseMessageSimulator();
            var settings = new ODataMessageWriterSettings();

            if (noMetadataMode)
            {
                message.SetHeader("Content-Type", "application/json;odata.metadata=none");
            }
            else
            {
                settings.SetContentType(ODataFormat.Json);
                settings.SetServiceDocumentUri(new Uri("http://fake.org/"));
            }

            var messageWriter = new ODataMessageWriter(message, settings, model);

            string actualPayload;
            using (var stream = new MemoryStream())
            {
                message.Stream = stream;

                write(entitySet, messageWriter);

                actualPayload = Encoding.UTF8.GetString(stream.ToArray());
            }

            actualPayload.Should().Be(expectedPayload);
        }

        private static void TestEntry<TValue>(Action<ODataResource> setValue, Func<ODataResource, TValue> getValue, TValue expectedValue)
        {
            var entry = new ODataResource();
            setValue(entry);
            getValue(entry).Should().Be(expectedValue);
        }

        private static void TestStream<TValue>(Action<ODataStreamReferenceValue> setValue, Func<ODataStreamReferenceValue, TValue> getValue, TValue expectedValue)
        {
            var streamReference = new ODataStreamReferenceValue();
            setValue(streamReference);
            getValue(streamReference).Should().Be(expectedValue);
        }

        private static void TestNavigationLink<TValue>(Action<ODataNestedResourceInfo> setValue, Func<ODataNestedResourceInfo, TValue> getValue, TValue expectedValue)
        {
            var link = new ODataNestedResourceInfo();
            setValue(link);
            getValue(link).Should().Be(expectedValue);
        }

        private static void TestFeed<TValue>(Action<ODataResourceSet> setValue, Func<ODataResourceSet, TValue> getValue, TValue expectedValue)
        {
            var feed = new ODataResourceSet();
            setValue(feed);
            getValue(feed).Should().Be(expectedValue);
        }

        private static void TestAction<TValue>(Action<ODataAction> setValue, Func<ODataAction, TValue> getValue, TValue expectedValue)
        {
            var action = new ODataAction();
            setValue(action);
            getValue(action).Should().Be(expectedValue);
        }

        private static void TestCollection(Action<ODataCollectionValue, ResourceType> setValue, ODataTypeAnnotation expectedTypeNameAnnotation)
        {
            var collection = new ODataCollectionValue();
            collection.TypeName = "Collection(Edm.String)";

            setValue(collection, ResourceType.GetCollectionResourceType(StringResourceType));
            CompareSerializationTypeNameAnnotation(collection, expectedTypeNameAnnotation);
        }

        private static void TestPrimitive(object value, Action<ODataPrimitiveValue, ResourceType> setValue, ODataTypeAnnotation expectedTypeNameAnnotation)
        {
            ODataPrimitiveValue primitive = new ODataPrimitiveValue(value);
            setValue(primitive, ResourceType.GetPrimitiveResourceType(value.GetType()));
            CompareSerializationTypeNameAnnotation(primitive, expectedTypeNameAnnotation);
        }

        private static bool CallShouldSpecifyTypeNameAnnotation(PayloadMetadataParameterInterpreter interpreter, ODataValue odataValue, out string typeNameToWrite)
        {
            ResourceType resourceType;

            resourceType = ResourceType.GetPrimitiveResourceType(((ODataPrimitiveValue)odataValue).Value.GetType());

            return interpreter.ShouldSpecifyTypeNameAnnotation(odataValue, resourceType, out typeNameToWrite);
        }

        private static void CompareSerializationTypeNameAnnotation(ODataValue value, ODataTypeAnnotation expectedTypeNameAnnotation)
        {
            if (expectedTypeNameAnnotation == null)
            {
                value.TypeAnnotation.Should().BeNull();
            }
            else
            {
                value.TypeAnnotation.TypeName.Should().Be(expectedTypeNameAnnotation.TypeName);
            }
        }

        #endregion
    }
}

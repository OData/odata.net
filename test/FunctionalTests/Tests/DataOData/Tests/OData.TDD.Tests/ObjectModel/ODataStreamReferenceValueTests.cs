//---------------------------------------------------------------------
// <copyright file="ODataStreamReferenceValueTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.ObjecetModel
{
    using System;
    using FluentAssertions;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Evaluation;
    using Microsoft.Test.OData.TDD.Tests.Evaluation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ODataStreamReferenceValueTests
    {
        private static readonly Uri ServiceUri = new Uri("http://service/", UriKind.Absolute);
        private static readonly Uri EditLink = new Uri("http://www.examples.com/editlink", UriKind.Absolute);
        private static readonly Uri ReadLink = new Uri("http://www.examples.com/readlink", UriKind.Absolute);
        private ODataStreamReferenceValue testSubject;
        private ODataStreamReferenceValue streamWithFullBuilder;

        [TestInitialize]
        public void InitTest()
        {
            this.testSubject = new ODataStreamReferenceValue();

            var entry = new ODataEntry
            {
                TypeName = "ns.DerivedType",
                Properties = new[]
                {
                    new ODataProperty{Name = "Id", Value = 1, SerializationInfo = new ODataPropertySerializationInfo{PropertyKind = ODataPropertyKind.Key}},
                    new ODataProperty{Name = "Name", Value = "Bob", SerializationInfo = new ODataPropertySerializationInfo{PropertyKind = ODataPropertyKind.ETag}}
                }
            };

            var serializationInfo = new ODataFeedAndEntrySerializationInfo { NavigationSourceName = "Set", NavigationSourceEntityTypeName = "ns.BaseType", ExpectedTypeName = "ns.BaseType" };
            var typeContext = ODataFeedAndEntryTypeContext.Create(serializationInfo, null, null, null, EdmCoreModel.Instance, true);
            var metadataContext = new TestMetadataContext();
            var entryMetadataContext = ODataEntryMetadataContext.Create(entry, typeContext, serializationInfo, null, metadataContext, SelectedPropertiesNode.EntireSubtree);
            var fullMetadataBuilder = new ODataConventionalEntityMetadataBuilder(entryMetadataContext, metadataContext, new ODataConventionalUriBuilder(ServiceUri, UrlConvention.CreateWithExplicitValue(false)));
            this.streamWithFullBuilder = new ODataStreamReferenceValue();
            this.streamWithFullBuilder.SetMetadataBuilder(fullMetadataBuilder, "Stream");
        }

        [TestMethod]
        public void NewODataStreamReferenceValueShouldHaveNullEditLink()
        {
            this.testSubject.EditLink.Should().BeNull();
        }

        [TestMethod]
        public void NewODataStreamReferenceValueShouldHaveNullReadLink()
        {
            this.testSubject.ReadLink.Should().BeNull();
        }

        [TestMethod]
        public void NewODataStreamReferenceValueShouldHaveNullETag()
        {
            this.testSubject.ETag.Should().BeNull();
        }

        [TestMethod]
        public void NewODataStreamReferenceValueShouldHaveNullContentType()
        {
            this.testSubject.ContentType.Should().BeNull();
        }

        [TestMethod]
        public void ShouldBeAbleToSetEditLinkToODataStreamReferenceValue()
        {
            this.testSubject.EditLink = EditLink;
            this.testSubject.EditLink.Should().BeSameAs(EditLink);
        }

        [TestMethod]
        public void ShouldBeAbleToSetReadLinkToODataStreamReferenceValue()
        {
            this.testSubject.ReadLink = ReadLink;
            this.testSubject.ReadLink.Should().BeSameAs(ReadLink);
        }

        [TestMethod]
        public void ShouldBeAbleToSetETagToODataStreamReferenceValue()
        {
            this.testSubject.ETag = "ETag";
            this.testSubject.ETag.Should().Be("ETag");
        }

        [TestMethod]
        public void ShouldBeAbleToSetContentTypeToODataStreamReferenceValue()
        {
            this.testSubject.ContentType = "ContentType";
            this.testSubject.ContentType.Should().Be("ContentType");
        }

        [TestMethod]
        public void MetadataBuilderShouldNotAffectUserAssignedEditLink()
        {
            this.testSubject.EditLink = EditLink;
            this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, "propertyName");
            this.testSubject.EditLink.Should().BeSameAs(EditLink);
        }

        [TestMethod]
        public void MetadataBuilderShouldNotAffectUserAssignedReadLink()
        {
            this.testSubject.ReadLink = ReadLink;
            this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, "propertyName");
            this.testSubject.ReadLink.Should().BeSameAs(ReadLink);
        }

        [TestMethod]
        public void MetadataBuilderShouldNotAffectUserAssignedETag()
        {
            this.testSubject.ETag = "ETag";
            this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, "propertyName");
            this.testSubject.ETag.Should().Be("ETag");
        }

        [TestMethod]
        public void MetadataBuilderShouldNotAffectUserAssignedContentType()
        {
            this.testSubject.ContentType = "ContentType";
            this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, "propertyName");
            this.testSubject.ContentType.Should().Be("ContentType");
        }

        [TestMethod]
        public void GetMetadataBuilderShouldReturnCurrentBuilder()
        {
            this.testSubject.GetMetadataBuilder().Should().BeNull();
            this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, "propertyName");
            this.testSubject.GetMetadataBuilder().Should().BeSameAs(ODataEntityMetadataBuilder.Null);
        }
        
        [TestMethod]
        public void ChangingMetadataBuilderShouldUpdateCalculatedEditLink()
        {
            this.testSubject.SetMetadataBuilder(new TestEntityMetadataBuilder(new ODataEntry()), "propertyName");
            this.testSubject.EditLink.OriginalString.Should().Be("http://service/ComputedStreamEditLink/propertyName");
            this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, "propertyName");
            this.testSubject.EditLink.Should().BeNull();
        }

        [TestMethod]
        public void ChangingMetadataBuilderShouldUpdateCalculatedReadLink()
        {
            this.testSubject.SetMetadataBuilder(new TestEntityMetadataBuilder(new ODataEntry()), "propertyName");
            this.testSubject.ReadLink.OriginalString.Should().Be("http://service/ComputedStreamReadLink/propertyName");
            this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, "propertyName");
            this.testSubject.ReadLink.Should().BeNull();
        }

        [TestMethod]
        public void SettingEditLinkToNullShouldOverrideEditLinkFromFullBuilder()
        {
            this.streamWithFullBuilder.EditLink.OriginalString.Should().Be("http://service/Set(1)/ns.DerivedType/Stream");
            this.streamWithFullBuilder.EditLink = null;
            this.streamWithFullBuilder.EditLink.Should().BeNull();
        }

        [TestMethod]
        public void SettingReadLinkToNullShouldOverrideReadLinkFromFullBuilder()
        {
            this.streamWithFullBuilder.ReadLink.OriginalString.Should().Be("http://service/Set(1)/ns.DerivedType/Stream");
            this.streamWithFullBuilder.ReadLink = null;
            this.streamWithFullBuilder.ReadLink.Should().BeNull();
        }
    }
}

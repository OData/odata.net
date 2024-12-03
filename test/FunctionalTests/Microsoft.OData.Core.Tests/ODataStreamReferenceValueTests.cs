//---------------------------------------------------------------------
// <copyright file="ODataStreamReferenceValueTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Evaluation;
using Microsoft.OData.Tests.Evaluation;
using Microsoft.OData.Tests.Json;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class ODataStreamReferenceValueTests
    {
        private static readonly Uri ServiceUri = new Uri("http://service/", UriKind.Absolute);
        private static readonly Uri EditLink = new Uri("http://www.examples.com/editlink", UriKind.Absolute);
        private static readonly Uri ReadLink = new Uri("http://www.examples.com/readlink", UriKind.Absolute);
        private ODataStreamReferenceValue testSubject;
        private ODataStreamReferenceValue streamWithFullBuilder;

        public ODataStreamReferenceValueTests()
        {
            this.testSubject = new ODataStreamReferenceValue();

            var entry = new ODataResource
            {
                TypeName = "ns.DerivedType",
                Properties = new[]
                {
                    new ODataProperty{Name = "Id", Value = 1, SerializationInfo = new ODataPropertySerializationInfo{PropertyKind = ODataPropertyKind.Key}},
                    new ODataProperty{Name = "Name", Value = "Bob", SerializationInfo = new ODataPropertySerializationInfo{PropertyKind = ODataPropertyKind.ETag}}
                }
            };

            var serializationInfo = new ODataResourceSerializationInfo { NavigationSourceName = "Set", NavigationSourceEntityTypeName = "ns.BaseType", ExpectedTypeName = "ns.BaseType" };
            var typeContext = ODataResourceTypeContext.Create(serializationInfo, null, null, null);
            var metadataContext = new TestMetadataContext();
            var entryMetadataContext = ODataResourceMetadataContext.Create(entry, typeContext, serializationInfo, null, metadataContext, new SelectedPropertiesNode(SelectedPropertiesNode.SelectionType.EntireSubtree), null);
            var fullMetadataBuilder = new ODataConventionalEntityMetadataBuilder(entryMetadataContext, metadataContext,
                new ODataConventionalUriBuilder(ServiceUri, ODataUrlKeyDelimiter.Parentheses));
            this.streamWithFullBuilder = new ODataStreamReferenceValue();
            this.streamWithFullBuilder.SetMetadataBuilder(fullMetadataBuilder, "Stream");
        }

        [Fact]
        public void NewODataStreamReferenceValueShouldHaveNullEditLink()
        {
            Assert.Null(this.testSubject.EditLink);
        }

        [Fact]
        public void NewODataStreamReferenceValueShouldHaveNullReadLink()
        {
            Assert.Null(this.testSubject.ReadLink);
        }

        [Fact]
        public void NewODataStreamReferenceValueShouldHaveNullETag()
        {
            Assert.Null(this.testSubject.ETag);
        }

        [Fact]
        public void NewODataStreamReferenceValueShouldHaveNullContentType()
        {
            Assert.Null(this.testSubject.ContentType);
        }

        [Fact]
        public void ShouldBeAbleToSetEditLinkToODataStreamReferenceValue()
        {
            this.testSubject.EditLink = EditLink;
            Assert.Same(EditLink, this.testSubject.EditLink);
        }

        [Fact]
        public void ShouldBeAbleToSetReadLinkToODataStreamReferenceValue()
        {
            this.testSubject.ReadLink = ReadLink;
            Assert.Same(ReadLink, this.testSubject.ReadLink);
        }

        [Fact]
        public void ShouldBeAbleToSetETagToODataStreamReferenceValue()
        {
            this.testSubject.ETag = "ETag";
            Assert.Equal("ETag", this.testSubject.ETag);
        }

        [Fact]
        public void ShouldBeAbleToSetContentTypeToODataStreamReferenceValue()
        {
            this.testSubject.ContentType = "ContentType";
            Assert.Equal("ContentType", this.testSubject.ContentType);
        }

        [Fact]
        public void MetadataBuilderShouldNotAffectUserAssignedEditLink()
        {
            this.testSubject.EditLink = EditLink;
            this.testSubject.SetMetadataBuilder(ODataResourceMetadataBuilder.Null, "propertyName");
            Assert.Same(EditLink, this.testSubject.EditLink);
        }

        [Fact]
        public void MetadataBuilderShouldNotAffectUserAssignedReadLink()
        {
            this.testSubject.ReadLink = ReadLink;
            this.testSubject.SetMetadataBuilder(ODataResourceMetadataBuilder.Null, "propertyName");
            Assert.Same(ReadLink, this.testSubject.ReadLink);
        }

        [Fact]
        public void MetadataBuilderShouldNotAffectUserAssignedETag()
        {
            this.testSubject.ETag = "ETag";
            this.testSubject.SetMetadataBuilder(ODataResourceMetadataBuilder.Null, "propertyName");
            Assert.Equal("ETag", this.testSubject.ETag);
        }

        [Fact]
        public void MetadataBuilderShouldNotAffectUserAssignedContentType()
        {
            this.testSubject.ContentType = "ContentType";
            this.testSubject.SetMetadataBuilder(ODataResourceMetadataBuilder.Null, "propertyName");
            Assert.Equal("ContentType", this.testSubject.ContentType);
        }

        [Fact]
        public void GetMetadataBuilderShouldReturnCurrentBuilder()
        {
            Assert.Null(this.testSubject.GetMetadataBuilder());
            this.testSubject.SetMetadataBuilder(ODataResourceMetadataBuilder.Null, "propertyName");
            Assert.Same(ODataResourceMetadataBuilder.Null, this.testSubject.GetMetadataBuilder());
        }

        [Fact]
        public void ChangingMetadataBuilderShouldUpdateCalculatedEditLink()
        {
            this.testSubject.SetMetadataBuilder(new TestEntityMetadataBuilder(new ODataResource()), "propertyName");
            Assert.Equal("http://service/ComputedStreamEditLink/propertyName", this.testSubject.EditLink.OriginalString);
            this.testSubject.SetMetadataBuilder(ODataResourceMetadataBuilder.Null, "propertyName");
            Assert.Null(this.testSubject.EditLink);
        }

        [Fact]
        public void ChangingMetadataBuilderShouldUpdateCalculatedReadLink()
        {
            this.testSubject.SetMetadataBuilder(new TestEntityMetadataBuilder(new ODataResource()), "propertyName");
            Assert.Equal("http://service/ComputedStreamReadLink/propertyName", this.testSubject.ReadLink.OriginalString);
            this.testSubject.SetMetadataBuilder(ODataResourceMetadataBuilder.Null, "propertyName");
            Assert.Null(this.testSubject.ReadLink);
        }

        [Fact]
        public void SettingEditLinkToNullShouldOverrideEditLinkFromFullBuilder()
        {
            Assert.Equal("http://service/Set(1)/ns.DerivedType/Stream", this.streamWithFullBuilder.EditLink.OriginalString);
            this.streamWithFullBuilder.EditLink = null;
            Assert.Null(this.streamWithFullBuilder.EditLink);
        }

        [Fact]
        public void SettingReadLinkToNullShouldOverrideReadLinkFromFullBuilder()
        {
            Assert.Equal("http://service/Set(1)/ns.DerivedType/Stream", this.streamWithFullBuilder.ReadLink.OriginalString);
            this.streamWithFullBuilder.ReadLink = null;
            Assert.Null(this.streamWithFullBuilder.ReadLink);
        }
    }
}

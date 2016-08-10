//---------------------------------------------------------------------
// <copyright file="ODataStreamReferenceValueTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Edm;
using Microsoft.OData.Evaluation;
using Microsoft.OData.Tests.Evaluation;
using Microsoft.OData.Tests.JsonLight;
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
            var typeContext = ODataResourceTypeContext.Create(serializationInfo, null, null, null, true);
            var metadataContext = new TestMetadataContext();
            var entryMetadataContext = ODataResourceMetadataContext.Create(entry, typeContext, serializationInfo, null, metadataContext, SelectedPropertiesNode.EntireSubtree);
            var fullMetadataBuilder = new ODataConventionalEntityMetadataBuilder(entryMetadataContext, metadataContext,
                new ODataConventionalUriBuilder(ServiceUri, ODataUrlKeyDelimiter.Parentheses));
            this.streamWithFullBuilder = new ODataStreamReferenceValue();
            this.streamWithFullBuilder.SetMetadataBuilder(fullMetadataBuilder, "Stream");
        }

        [Fact]
        public void NewODataStreamReferenceValueShouldHaveNullEditLink()
        {
            this.testSubject.EditLink.Should().BeNull();
        }

        [Fact]
        public void NewODataStreamReferenceValueShouldHaveNullReadLink()
        {
            this.testSubject.ReadLink.Should().BeNull();
        }

        [Fact]
        public void NewODataStreamReferenceValueShouldHaveNullETag()
        {
            this.testSubject.ETag.Should().BeNull();
        }

        [Fact]
        public void NewODataStreamReferenceValueShouldHaveNullContentType()
        {
            this.testSubject.ContentType.Should().BeNull();
        }

        [Fact]
        public void ShouldBeAbleToSetEditLinkToODataStreamReferenceValue()
        {
            this.testSubject.EditLink = EditLink;
            this.testSubject.EditLink.Should().BeSameAs(EditLink);
        }

        [Fact]
        public void ShouldBeAbleToSetReadLinkToODataStreamReferenceValue()
        {
            this.testSubject.ReadLink = ReadLink;
            this.testSubject.ReadLink.Should().BeSameAs(ReadLink);
        }

        [Fact]
        public void ShouldBeAbleToSetETagToODataStreamReferenceValue()
        {
            this.testSubject.ETag = "ETag";
            this.testSubject.ETag.Should().Be("ETag");
        }

        [Fact]
        public void ShouldBeAbleToSetContentTypeToODataStreamReferenceValue()
        {
            this.testSubject.ContentType = "ContentType";
            this.testSubject.ContentType.Should().Be("ContentType");
        }

        [Fact]
        public void MetadataBuilderShouldNotAffectUserAssignedEditLink()
        {
            this.testSubject.EditLink = EditLink;
            this.testSubject.SetMetadataBuilder(ODataResourceMetadataBuilder.Null, "propertyName");
            this.testSubject.EditLink.Should().BeSameAs(EditLink);
        }

        [Fact]
        public void MetadataBuilderShouldNotAffectUserAssignedReadLink()
        {
            this.testSubject.ReadLink = ReadLink;
            this.testSubject.SetMetadataBuilder(ODataResourceMetadataBuilder.Null, "propertyName");
            this.testSubject.ReadLink.Should().BeSameAs(ReadLink);
        }

        [Fact]
        public void MetadataBuilderShouldNotAffectUserAssignedETag()
        {
            this.testSubject.ETag = "ETag";
            this.testSubject.SetMetadataBuilder(ODataResourceMetadataBuilder.Null, "propertyName");
            this.testSubject.ETag.Should().Be("ETag");
        }

        [Fact]
        public void MetadataBuilderShouldNotAffectUserAssignedContentType()
        {
            this.testSubject.ContentType = "ContentType";
            this.testSubject.SetMetadataBuilder(ODataResourceMetadataBuilder.Null, "propertyName");
            this.testSubject.ContentType.Should().Be("ContentType");
        }

        [Fact]
        public void GetMetadataBuilderShouldReturnCurrentBuilder()
        {
            this.testSubject.GetMetadataBuilder().Should().BeNull();
            this.testSubject.SetMetadataBuilder(ODataResourceMetadataBuilder.Null, "propertyName");
            this.testSubject.GetMetadataBuilder().Should().BeSameAs(ODataResourceMetadataBuilder.Null);
        }

        [Fact]
        public void ChangingMetadataBuilderShouldUpdateCalculatedEditLink()
        {
            this.testSubject.SetMetadataBuilder(new TestEntityMetadataBuilder(new ODataResource()), "propertyName");
            this.testSubject.EditLink.OriginalString.Should().Be("http://service/ComputedStreamEditLink/propertyName");
            this.testSubject.SetMetadataBuilder(ODataResourceMetadataBuilder.Null, "propertyName");
            this.testSubject.EditLink.Should().BeNull();
        }

        [Fact]
        public void ChangingMetadataBuilderShouldUpdateCalculatedReadLink()
        {
            this.testSubject.SetMetadataBuilder(new TestEntityMetadataBuilder(new ODataResource()), "propertyName");
            this.testSubject.ReadLink.OriginalString.Should().Be("http://service/ComputedStreamReadLink/propertyName");
            this.testSubject.SetMetadataBuilder(ODataResourceMetadataBuilder.Null, "propertyName");
            this.testSubject.ReadLink.Should().BeNull();
        }

        [Fact]
        public void SettingEditLinkToNullShouldOverrideEditLinkFromFullBuilder()
        {
            this.streamWithFullBuilder.EditLink.OriginalString.Should().Be("http://service/Set(1)/ns.DerivedType/Stream");
            this.streamWithFullBuilder.EditLink = null;
            this.streamWithFullBuilder.EditLink.Should().BeNull();
        }

        [Fact]
        public void SettingReadLinkToNullShouldOverrideReadLinkFromFullBuilder()
        {
            this.streamWithFullBuilder.ReadLink.OriginalString.Should().Be("http://service/Set(1)/ns.DerivedType/Stream");
            this.streamWithFullBuilder.ReadLink = null;
            this.streamWithFullBuilder.ReadLink.Should().BeNull();
        }
    }
}

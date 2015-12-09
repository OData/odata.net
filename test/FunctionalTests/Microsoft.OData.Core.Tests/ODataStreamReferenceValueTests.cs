//---------------------------------------------------------------------
// <copyright file="ODataStreamReferenceValueTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.Evaluation;
using Microsoft.OData.Core.Tests.Evaluation;
using Microsoft.OData.Core.Tests.JsonLight;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests
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
            this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, "propertyName");
            this.testSubject.EditLink.Should().BeSameAs(EditLink);
        }

        [Fact]
        public void MetadataBuilderShouldNotAffectUserAssignedReadLink()
        {
            this.testSubject.ReadLink = ReadLink;
            this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, "propertyName");
            this.testSubject.ReadLink.Should().BeSameAs(ReadLink);
        }

        [Fact]
        public void MetadataBuilderShouldNotAffectUserAssignedETag()
        {
            this.testSubject.ETag = "ETag";
            this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, "propertyName");
            this.testSubject.ETag.Should().Be("ETag");
        }

        [Fact]
        public void MetadataBuilderShouldNotAffectUserAssignedContentType()
        {
            this.testSubject.ContentType = "ContentType";
            this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, "propertyName");
            this.testSubject.ContentType.Should().Be("ContentType");
        }

        [Fact]
        public void GetMetadataBuilderShouldReturnCurrentBuilder()
        {
            this.testSubject.GetMetadataBuilder().Should().BeNull();
            this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, "propertyName");
            this.testSubject.GetMetadataBuilder().Should().BeSameAs(ODataEntityMetadataBuilder.Null);
        }
        
        [Fact]
        public void ChangingMetadataBuilderShouldUpdateCalculatedEditLink()
        {
            this.testSubject.SetMetadataBuilder(new TestEntityMetadataBuilder(new ODataEntry()), "propertyName");
            this.testSubject.EditLink.OriginalString.Should().Be("http://service/ComputedStreamEditLink/propertyName");
            this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, "propertyName");
            this.testSubject.EditLink.Should().BeNull();
        }

        [Fact]
        public void ChangingMetadataBuilderShouldUpdateCalculatedReadLink()
        {
            this.testSubject.SetMetadataBuilder(new TestEntityMetadataBuilder(new ODataEntry()), "propertyName");
            this.testSubject.ReadLink.OriginalString.Should().Be("http://service/ComputedStreamReadLink/propertyName");
            this.testSubject.SetMetadataBuilder(ODataEntityMetadataBuilder.Null, "propertyName");
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

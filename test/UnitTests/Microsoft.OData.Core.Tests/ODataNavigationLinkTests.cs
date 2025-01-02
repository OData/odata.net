//---------------------------------------------------------------------
// <copyright file="ODataNavigationLinkTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Evaluation;
using Microsoft.OData.Tests.Evaluation;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class ODataNavigationLinkTests
    {
        private static readonly Uri ServiceUri = new Uri("http://service/", UriKind.Absolute);
        private ODataNestedResourceInfo navigationLink;
        private ODataNestedResourceInfo navigationLinkWithFullBuilder;
        private ODataNestedResourceInfo navigationLinkWithNoOpBuilder;
        private ODataNestedResourceInfo navigationLinkWithNullBuilder;

        public ODataNavigationLinkTests()
        {
            this.navigationLink = new ODataNestedResourceInfo();

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
            var metadataBuilder = new ODataConventionalEntityMetadataBuilder(entryMetadataContext, metadataContext,
                new ODataConventionalUriBuilder(ServiceUri, ODataUrlKeyDelimiter.Parentheses));
            this.navigationLinkWithFullBuilder = new ODataNestedResourceInfo { Name = "NavProp" };
            this.navigationLinkWithFullBuilder.MetadataBuilder = metadataBuilder;

            this.navigationLinkWithNoOpBuilder = new ODataNestedResourceInfo { Name = "NavProp" };
            this.navigationLinkWithNoOpBuilder.MetadataBuilder = new NoOpResourceMetadataBuilder(entry);

            this.navigationLinkWithNullBuilder = new ODataNestedResourceInfo { Name = "NavProp" };
            this.navigationLinkWithNullBuilder.MetadataBuilder = ODataResourceMetadataBuilder.Null;
        }

        [Fact]
        public void NewODataNavigationLinkShouldHaveNullName()
        {
            Assert.Null(this.navigationLink.Name);
        }

        [Fact]
        public void NewODataNavigationLinkShouldHaveNullUrl()
        {
            Assert.Null(this.navigationLink.Url);
        }

        [Fact]
        public void NewODataNavigationLinkShouldHaveNullAssociationLinkUrl()
        {
            Assert.Null(this.navigationLink.AssociationLinkUrl);
        }

        [Fact]
        public void ShouldBeAbleToSetNameToODataNavigationLink()
        {
            this.navigationLink.Name = "Name";
            Assert.Equal("Name", this.navigationLink.Name);
        }

        [Fact]
        public void ShouldBeAbleToSetUrlToODataNavigationLink()
        {
            var url = new Uri("http://foo", UriKind.Absolute);
            this.navigationLink.Url = url;
            Assert.Equal(url, this.navigationLink.Url);
        }

        [Fact]
        public void ShouldBeAbleToSetAssociationLinkUrlToODataNavigationLink()
        {
            var associationLinkUrl = new Uri("http://foo", UriKind.Absolute);
            this.navigationLink.AssociationLinkUrl = associationLinkUrl;
            Assert.Same(associationLinkUrl, this.navigationLink.AssociationLinkUrl);
        }

        [Fact]
        public void MetadataBuilderShouldNotAffectUserAssignedName()
        {
            this.navigationLink.Name = "Name";
            this.navigationLink.MetadataBuilder = ODataResourceMetadataBuilder.Null;
            Assert.Equal("Name", this.navigationLink.Name);
        }

        [Fact]
        public void SettingUrlToNullShouldOverrideUrlFromFullBuilder()
        {
            Assert.Equal(new Uri("http://service/Set(1)/ns.DerivedType/NavProp"), this.navigationLinkWithFullBuilder.Url);
            this.navigationLinkWithFullBuilder.Url = null;
            Assert.Null(this.navigationLinkWithFullBuilder.Url);
        }

        [Fact]
        public void SettingAssociationLinkUrlToNullShouldOverrideAssociationLinkUrlFromFullBuilder()
        {
            Assert.Equal(new Uri("http://service/Set(1)/ns.DerivedType/NavProp/$ref"), this.navigationLinkWithFullBuilder.AssociationLinkUrl);
            this.navigationLinkWithFullBuilder.AssociationLinkUrl = null;
            Assert.Null(this.navigationLinkWithFullBuilder.AssociationLinkUrl);
        }

        [Fact]
        public void ShouldBeAbleToSetAndClearUrlOnNavigationLinkWithNoOpBuilder()
        {
            var link = new Uri("http://link", UriKind.Absolute);
            Assert.Null(this.navigationLinkWithNoOpBuilder.Url);
            this.navigationLinkWithNoOpBuilder.Url = link;
            Assert.Same(link, this.navigationLinkWithNoOpBuilder.Url);
            this.navigationLinkWithNoOpBuilder.Url = null;
            Assert.Null(this.navigationLinkWithNoOpBuilder.Url);
        }

        [Fact]
        public void ShouldBeAbleToSetAndClearAssociationLinkUrlOnNavigationLinkWithNoOpBuilder()
        {
            var link = new Uri("http://link", UriKind.Absolute);
            Assert.Null(this.navigationLinkWithNoOpBuilder.AssociationLinkUrl);
            this.navigationLinkWithNoOpBuilder.AssociationLinkUrl = link;
            Assert.Same(link, this.navigationLinkWithNoOpBuilder.AssociationLinkUrl);
            this.navigationLinkWithNoOpBuilder.AssociationLinkUrl = null;
            Assert.Null(this.navigationLinkWithNoOpBuilder.AssociationLinkUrl);
        }

        [Fact]
        public void UrlShouldAlwaysBeNullOnNavigationLinkWithNullBuilder()
        {
            Assert.Null(this.navigationLinkWithNullBuilder.Url);
            this.navigationLinkWithNullBuilder.Url = new Uri("http://link", UriKind.Absolute);
            Assert.Null(this.navigationLinkWithNullBuilder.Url);
        }

        [Fact]
        public void AssociationLinkUrlShouldAlwaysBeNullOnNavigationLinkWithNullBuilder()
        {
            Assert.Null(this.navigationLinkWithNullBuilder.AssociationLinkUrl);
            this.navigationLinkWithNullBuilder.AssociationLinkUrl = new Uri("http://link", UriKind.Absolute);
            Assert.Null(this.navigationLinkWithNullBuilder.AssociationLinkUrl);
        }
    }
}

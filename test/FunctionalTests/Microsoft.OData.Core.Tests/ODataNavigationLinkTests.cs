//---------------------------------------------------------------------
// <copyright file="ODataNavigationLinkTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.Evaluation;
using Microsoft.OData.Core.Tests.Evaluation;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests
{
    public class ODataNavigationLinkTests
    {
        private static readonly Uri ServiceUri = new Uri("http://service/", UriKind.Absolute);
        private ODataNavigationLink navigationLink;
        private ODataNavigationLink navigationLinkWithFullBuilder;
        private ODataNavigationLink navigationLinkWithNoOpBuilder;
        private ODataNavigationLink navigationLinkWithNullBuilder;

        public ODataNavigationLinkTests()
        {
            this.navigationLink = new ODataNavigationLink();

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
            var metadataBuilder = new ODataConventionalEntityMetadataBuilder(entryMetadataContext, metadataContext, new ODataConventionalUriBuilder(ServiceUri, UrlConvention.CreateWithExplicitValue(false)));
            this.navigationLinkWithFullBuilder = new ODataNavigationLink { Name = "NavProp" };
            this.navigationLinkWithFullBuilder.MetadataBuilder = metadataBuilder;

            this.navigationLinkWithNoOpBuilder = new ODataNavigationLink { Name = "NavProp" };
            this.navigationLinkWithNoOpBuilder.MetadataBuilder = new NoOpEntityMetadataBuilder(entry);

            this.navigationLinkWithNullBuilder = new ODataNavigationLink { Name = "NavProp" };
            this.navigationLinkWithNullBuilder.MetadataBuilder = ODataEntityMetadataBuilder.Null;
        }

        [Fact]
        public void NewODataNavigationLinkShouldHaveNullName()
        {
            this.navigationLink.Name.Should().BeNull();
        }

        [Fact]
        public void NewODataNavigationLinkShouldHaveNullUrl()
        {
            this.navigationLink.Url.Should().BeNull();
        }

        [Fact]
        public void NewODataNavigationLinkShouldHaveNullAssociationLinkUrl()
        {
            this.navigationLink.AssociationLinkUrl.Should().BeNull();
        }

        [Fact]
        public void ShouldBeAbleToSetNameToODataNavigationLink()
        {
            this.navigationLink.Name = "Name";
            this.navigationLink.Name.Should().Be("Name");
        }

        [Fact]
        public void ShouldBeAbleToSetUrlToODataNavigationLink()
        {
            var url = new Uri("http://foo", UriKind.Absolute);
            this.navigationLink.Url = url;
            this.navigationLink.Url.Should().Be(url);
        }

        [Fact]
        public void ShouldBeAbleToSetAssociationLinkUrlToODataNavigationLink()
        {
            var associationLinkUrl = new Uri("http://foo", UriKind.Absolute);
            this.navigationLink.AssociationLinkUrl = associationLinkUrl;
            this.navigationLink.AssociationLinkUrl.Should().BeSameAs(associationLinkUrl);
        }

        [Fact]
        public void MetadataBuilderShouldNotAffectUserAssignedName()
        {
            this.navigationLink.Name = "Name";
            this.navigationLink.MetadataBuilder = ODataEntityMetadataBuilder.Null;
            this.navigationLink.Name.Should().Be("Name");
        }

        [Fact]
        public void SettingUrlToNullShouldOverrideUrlFromFullBuilder()
        {
            this.navigationLinkWithFullBuilder.Url.Should().Be("http://service/Set(1)/ns.DerivedType/NavProp");
            this.navigationLinkWithFullBuilder.Url = null;
            this.navigationLinkWithFullBuilder.Url.Should().BeNull();
        }

        [Fact]
        public void SettingAssociationLinkUrlToNullShouldOverrideAssociationLinkUrlFromFullBuilder()
        {
            this.navigationLinkWithFullBuilder.AssociationLinkUrl.Should().Be("http://service/Set(1)/ns.DerivedType/NavProp/$ref");
            this.navigationLinkWithFullBuilder.AssociationLinkUrl = null;
            this.navigationLinkWithFullBuilder.AssociationLinkUrl.Should().BeNull();
        }

        [Fact]
        public void ShouldBeAbleToSetAndClearUrlOnNavigationLinkWithNoOpBuilder()
        {
            var link = new Uri("http://link", UriKind.Absolute);
            this.navigationLinkWithNoOpBuilder.Url.Should().BeNull();
            this.navigationLinkWithNoOpBuilder.Url = link;
            this.navigationLinkWithNoOpBuilder.Url.Should().BeSameAs(link);
            this.navigationLinkWithNoOpBuilder.Url = null;
            this.navigationLinkWithNoOpBuilder.Url.Should().BeNull();
        }

        [Fact]
        public void ShouldBeAbleToSetAndClearAssociationLinkUrlOnNavigationLinkWithNoOpBuilder()
        {
            var link = new Uri("http://link", UriKind.Absolute);
            this.navigationLinkWithNoOpBuilder.AssociationLinkUrl.Should().BeNull();
            this.navigationLinkWithNoOpBuilder.AssociationLinkUrl = link;
            this.navigationLinkWithNoOpBuilder.AssociationLinkUrl.Should().BeSameAs(link);
            this.navigationLinkWithNoOpBuilder.AssociationLinkUrl = null;
            this.navigationLinkWithNoOpBuilder.AssociationLinkUrl.Should().BeNull();
        }

        [Fact]
        public void UrlShouldAlwaysBeNullOnNavigationLinkWithNullBuilder()
        {
            this.navigationLinkWithNullBuilder.Url.Should().BeNull();
            this.navigationLinkWithNullBuilder.Url = new Uri("http://link", UriKind.Absolute);
            this.navigationLinkWithNullBuilder.Url.Should().BeNull();
        }

        [Fact]
        public void AssociationLinkUrlShouldAlwaysBeNullOnNavigationLinkWithNullBuilder()
        {
            this.navigationLinkWithNullBuilder.AssociationLinkUrl.Should().BeNull();
            this.navigationLinkWithNullBuilder.AssociationLinkUrl = new Uri("http://link", UriKind.Absolute);
            this.navigationLinkWithNullBuilder.AssociationLinkUrl.Should().BeNull();
        }
    }
}

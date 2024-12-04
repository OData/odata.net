//---------------------------------------------------------------------
// <copyright file="ODataResourceTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm;
using Microsoft.OData.Evaluation;
using Microsoft.OData.Tests.Evaluation;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class ODataResourceTests
    {
        private ODataResource odataEntry;
        private ODataResource odataEntryWithFullBuilder;
        private ODataResource odataEntryWithNullBuilder;

        public ODataResourceTests()
        {
            this.odataEntry = new ODataResource();

            this.odataEntryWithFullBuilder = new ODataResource
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
            var entryMetadataContext = ODataResourceMetadataContext.Create(this.odataEntryWithFullBuilder, typeContext, serializationInfo, null, metadataContext, new SelectedPropertiesNode(SelectedPropertiesNode.SelectionType.EntireSubtree), null);
            this.odataEntryWithFullBuilder.MetadataBuilder =
                new ODataConventionalEntityMetadataBuilder(entryMetadataContext, metadataContext,
                    new ODataConventionalUriBuilder(new Uri("http://service/", UriKind.Absolute),
                        ODataUrlKeyDelimiter.Parentheses));

            this.odataEntryWithNullBuilder = new ODataResource { MetadataBuilder = ODataResourceMetadataBuilder.Null };
        }

        [Fact]
        public void SettingIdToNullShouldOverrideIdFromFullBuilder()
        {
            Assert.Equal(new Uri("http://service/Set(1)"), this.odataEntryWithFullBuilder.Id);
            Assert.False(this.odataEntryWithFullBuilder.HasNonComputedId);
            this.odataEntryWithFullBuilder.Id = null;
            Assert.True(this.odataEntryWithFullBuilder.HasNonComputedId);
            Assert.Null(this.odataEntryWithFullBuilder.Id);
        }

        [Fact]
        public void SettingETagToNullShouldOverrideETagFromFullBuilder()
        {
            Assert.Equal("W/\"'Bob'\"", this.odataEntryWithFullBuilder.ETag);
            Assert.False(this.odataEntryWithFullBuilder.HasNonComputedETag);
            this.odataEntryWithFullBuilder.ETag = null;
            Assert.True(this.odataEntryWithFullBuilder.HasNonComputedETag);
            Assert.Null(this.odataEntryWithFullBuilder.ETag);
        }

        [Fact]
        public void SettingReadLinkToNullShouldOverrideReadLinkFromFullBuilder()
        {
            Assert.Equal("http://service/Set(1)/ns.DerivedType", this.odataEntryWithFullBuilder.ReadLink.OriginalString);
            Assert.False(this.odataEntryWithFullBuilder.HasNonComputedReadLink);
            this.odataEntryWithFullBuilder.ReadLink = null;
            Assert.True(this.odataEntryWithFullBuilder.HasNonComputedReadLink);
            Assert.Null(this.odataEntryWithFullBuilder.ReadLink);
        }

        [Fact]
        public void SettingEditLinkToNullShouldOverrideEditLinkFromFullBuilder()
        {
            Assert.Equal("http://service/Set(1)/ns.DerivedType", this.odataEntryWithFullBuilder.EditLink.OriginalString);
            Assert.False(this.odataEntryWithFullBuilder.HasNonComputedEditLink);
            this.odataEntryWithFullBuilder.EditLink = null;
            Assert.True(this.odataEntryWithFullBuilder.HasNonComputedEditLink);
            Assert.Null(this.odataEntryWithFullBuilder.EditLink);
        }

        [Fact]
        public void EntryWithNullBuilderShouldAlwaysReturnNullId()
        {
            this.odataEntryWithNullBuilder.Id = null;
            Assert.Null(this.odataEntryWithNullBuilder.Id);
        }

        [Fact]
        public void EntryWithNullBuilderShouldAlwaysReturnNullETag()
        {
            this.odataEntryWithNullBuilder.ETag = "ETag";
            Assert.Null(this.odataEntryWithNullBuilder.ETag);
        }

        [Fact]
        public void EntryWithNullBuilderShouldAlwaysReturnNullReadLink()
        {
            this.odataEntryWithNullBuilder.ReadLink = new Uri("http://link", UriKind.Absolute);
            Assert.Null(this.odataEntryWithNullBuilder.ReadLink);
        }

        [Fact]
        public void EntryWithNullBuilderShouldAlwaysReturnNullEditLink()
        {
            this.odataEntryWithNullBuilder.EditLink = new Uri("http://link", UriKind.Absolute);
            Assert.Null(this.odataEntryWithNullBuilder.EditLink);
        }

        [Fact]
        public void DefaultEntryShouldHasNoOpBuilder()
        {
            Assert.IsType<NoOpResourceMetadataBuilder>(this.odataEntry.MetadataBuilder);
        }

        [Fact]
        public void ShouldBeAbleToSetAndClearIdOnEntryWithNoOpBuilder()
        {
            Assert.Null(this.odataEntry.Id);
            this.odataEntry.Id = new Uri("http://my/Id");
            Assert.Equal("http://my/Id", this.odataEntry.Id.ToString());
            this.odataEntry.Id = null;
            Assert.Null(this.odataEntry.Id);
        }

        [Fact]
        public void ShouldBeAbleToSetAndClearETagOnEntryWithNoOpBuilder()
        {
            Assert.Null(this.odataEntry.ETag);
            this.odataEntry.ETag = "ETag";
            Assert.Equal("ETag", this.odataEntry.ETag);
            this.odataEntry.ETag = null;
            Assert.Null(this.odataEntry.ETag);
        }

        [Fact]
        public void ShouldBeAbleToSetAndClearReadLinkOnEntryWithNoOpBuilder()
        {
            Assert.Null(this.odataEntry.ReadLink);
            this.odataEntry.ReadLink = new Uri("http://link", UriKind.Absolute);
            Assert.Equal(new Uri("http://link", UriKind.Absolute), this.odataEntry.ReadLink);
            this.odataEntry.ReadLink = null;
            Assert.Null(this.odataEntry.ReadLink);
        }

        [Fact]
        public void ShouldBeAbleToSetAndClearEditLinkOnEntryWithNoOpBuilder()
        {
            Assert.Null(this.odataEntry.EditLink);
            this.odataEntry.EditLink = new Uri("http://link", UriKind.Absolute);
            Assert.Equal(new Uri("http://link", UriKind.Absolute), this.odataEntry.EditLink);
            this.odataEntry.EditLink = null;
            Assert.Null(this.odataEntry.EditLink);
        }

        [Fact]
        public void InstanceAnnotationsPropertyShouldNotBeNullAtCreation()
        {
            Assert.NotNull(this.odataEntry.InstanceAnnotations);
        }

        [Fact]
        public void InstanceAnnotationsPropertyShouldReturnAWritableCollectionAtCreation()
        {
            Assert.NotNull(this.odataEntry.InstanceAnnotations);
            this.odataEntry.InstanceAnnotations.Add(new ODataInstanceAnnotation("namespace.name", new ODataPrimitiveValue("value")));
            Assert.Single(this.odataEntry.InstanceAnnotations);
        }

        [Fact]
        public void SetNullValueToInstanceAnnotationsPropertyShouldThrow()
        {
            Action test = () => this.odataEntry.InstanceAnnotations = null;
            test.Throws<ArgumentNullException>("Value cannot be null. (Parameter 'value')");
        }

        [Fact]
        public void SetListValueToInstanceAnnotationsPropertyShouldPass()
        {
            ICollection<ODataInstanceAnnotation> initialCollection = this.odataEntry.InstanceAnnotations;
            ICollection<ODataInstanceAnnotation> newCollection = new List<ODataInstanceAnnotation>();
            this.odataEntry.InstanceAnnotations = newCollection;
            Assert.Same(newCollection, this.odataEntry.InstanceAnnotations);
            Assert.NotSame(initialCollection, this.odataEntry.InstanceAnnotations);
        }

        [Fact]
        public void NewODataEntryShouldContainNullSerializationInfo()
        {
            Assert.Null(this.odataEntry.SerializationInfo);
        }

        [Fact]
        public void SerializationInfoShouldBeValidatedByTheSetter()
        {
            Action action = () => this.odataEntry.SerializationInfo = new ODataResourceSerializationInfo() { NavigationSourceKind = EdmNavigationSourceKind.Singleton };
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(action);
            Assert.Contains("serializationInfo.NavigationSourceName", exception.Message);
        }

        [Fact]
        public void ShouldBeAbleToSetSerializationInfo()
        {
            this.odataEntry.SerializationInfo = new ODataResourceSerializationInfo { NavigationSourceName = "Set", NavigationSourceEntityTypeName = "ns.base", ExpectedTypeName = "ns.expected" };
            Assert.Equal("Set", this.odataEntry.SerializationInfo.NavigationSourceName);
        }

        [Fact]
        public void AddNullActionShouldThrow()
        {
            Action action = () => this.odataEntry.AddAction(null);
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(action);
            Assert.Contains("action", exception.Message);
        }

        [Fact]
        public void AddActionShouldWork()
        {
            var action = new ODataAction() { Metadata = new Uri("http://odata.org/metadata"), Target = new Uri("http://odata.org/target"), Title = "TestAction", };
            this.odataEntry.AddAction(action);

            var odataAction = Assert.Single(this.odataEntry.Actions);
            Assert.Same(action, odataAction);
        }

        [Fact]
        public void AddNullFunctionShouldThrow()
        {
            Action action = () => this.odataEntry.AddFunction(null);
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(action);
            Assert.Contains("function", exception.Message);
        }

        [Fact]
        public void AddFunctionShouldWork()
        {
            var function = new ODataFunction() { Metadata = new Uri("http://odata.org/metadata"), Target = new Uri("http://odata.org/target"), Title = "TestFunction", };
            this.odataEntry.AddFunction(function);

            var odataFunction = Assert.Single(this.odataEntry.Functions);
            Assert.Same(function, odataFunction);
        }

        [Fact]
        public void AddDuplicateAction()
        {
            var action = new ODataAction() { Metadata = new Uri("http://odata.org/metadata"), Target = new Uri("http://odata.org/target"), Title = "TestAction", };
            this.odataEntry.AddAction(action);
            this.odataEntry.AddAction(action);

            var odataAction = Assert.Single(this.odataEntry.Actions);
            Assert.Same(action, odataAction);
        }

        [Fact]
        public void AddDuplicateFunction()
        {
            var function = new ODataFunction() { Metadata = new Uri("http://odata.org/metadata"), Target = new Uri("http://odata.org/target"), Title = "TestFunction", };
            this.odataEntry.AddFunction(function);
            this.odataEntry.AddFunction(function);

            var odataFunction = Assert.Single(this.odataEntry.Functions);
            Assert.Same(function, odataFunction);
        }

        [Fact]
        public void ODataResourcePropertyWithODataResourceValueThrows()
        {
            ODataResource resource = new ODataResource
            {
                TypeName = "NS.Resource",
            };

            Action test = () => resource.Properties = new[]
            {
                new ODataProperty { Name = "ResourceProperty", Value = new ODataResourceValue() }
            };

            var exception = Assert.Throws<ODataException>(test);
            Assert.Equal(Strings.ODataResource_PropertyValueCannotBeODataResourceValue("ResourceProperty"), exception.Message);
        }

        [Fact]
        public void ODataResourcePropertyWithCollectionODataResourceValueThrows()
        {
            ODataResource resource = new ODataResource
            {
                TypeName = "NS.Resource",
            };

            Action test = () => resource.Properties = new[]
            {
                new ODataProperty
                {
                    Name = "CollectionProperty",
                    Value = new ODataCollectionValue
                    {
                        Items = new [] { new ODataResourceValue() }
                    }
                }
            };

            var exception = Assert.Throws<ODataException>(test);
            Assert.Equal(Strings.ODataResource_PropertyValueCannotBeODataResourceValue("CollectionProperty"), exception.Message);
        }

        [Fact]
        public void WhenSkipPropertyVerificatonIsTrue_ODataResourcePropertyWithODataResourceValue_DoesNotThrow()
        {
            ODataResource resource = new ODataResource
            {
                TypeName = "NS.Resource",
                SkipPropertyVerification = true
            };

            resource.Properties = new[]
            {
                new ODataProperty { Name = "ResourceProperty", Value = new ODataResourceValue() }
            };
        }

        [Fact]
        public void WhenSkipPropertyVerificatonIsTrue_ODataResourcePropertyWithCollectionODataResourceValue_DoesNotThrow()
        {
            ODataResource resource = new ODataResource
            {
                TypeName = "NS.Resource",
                SkipPropertyVerification = true
            };

            resource.Properties = new[]
            {
                new ODataProperty
                {
                    Name = "CollectionProperty",
                    Value = new ODataCollectionValue
                    {
                        Items = new [] { new ODataResourceValue() }
                    }
                }
            };
        }
    }
}

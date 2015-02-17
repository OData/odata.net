//---------------------------------------------------------------------
// <copyright file="ODataEntryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.ObjecetModel
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using FluentAssertions;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Evaluation;
    using Microsoft.Test.OData.TDD.Tests.Evaluation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ODataEntryTests
    {
        private ODataEntry odataEntry;
        private ODataEntry odataEntryWithFullBuilder;
        private ODataEntry odataEntryWithNullBuilder;

        [TestInitialize]
        public void InitTest()
        {
            this.odataEntry = new ODataEntry();

            this.odataEntryWithFullBuilder = new ODataEntry
            {
                TypeName = "ns.DerivedType",
                Properties = new[]
                {
                    new ODataProperty{Name = "Id", Value = 1, SerializationInfo = new ODataPropertySerializationInfo{PropertyKind = ODataPropertyKind.Key}},
                    new ODataProperty{Name = "Name", Value = "Bob", SerializationInfo = new ODataPropertySerializationInfo{PropertyKind = ODataPropertyKind.ETag}}
                }
            };
            var serializationInfo = new ODataFeedAndEntrySerializationInfo {NavigationSourceName = "Set", NavigationSourceEntityTypeName = "ns.BaseType", ExpectedTypeName = "ns.BaseType"};
            var typeContext = ODataFeedAndEntryTypeContext.Create(serializationInfo, null, null, null, EdmCoreModel.Instance, true);
            var metadataContext = new TestMetadataContext();
            var entryMetadataContext = ODataEntryMetadataContext.Create(this.odataEntryWithFullBuilder, typeContext, serializationInfo, null, metadataContext, SelectedPropertiesNode.EntireSubtree);
            this.odataEntryWithFullBuilder.MetadataBuilder = new ODataConventionalEntityMetadataBuilder(entryMetadataContext, metadataContext, new ODataConventionalUriBuilder(new Uri("http://service/", UriKind.Absolute), UrlConvention.CreateWithExplicitValue(false)));

            this.odataEntryWithNullBuilder = new ODataEntry {MetadataBuilder = ODataEntityMetadataBuilder.Null};
        }

        [TestMethod]
        public void SettingIdToNullShouldOverrideIdFromFullBuilder()
        {
            this.odataEntryWithFullBuilder.Id.Should().Be("http://service/Set(1)");
            this.odataEntryWithFullBuilder.HasNonComputedId.Should().BeFalse();
            this.odataEntryWithFullBuilder.Id = null;
            this.odataEntryWithFullBuilder.HasNonComputedId.Should().BeTrue();
            this.odataEntryWithFullBuilder.Id.Should().BeNull();
        }

        [TestMethod]
        public void SettingETagToNullShouldOverrideETagFromFullBuilder()
        {
            this.odataEntryWithFullBuilder.ETag.Should().Be("W/\"'Bob'\"");
            this.odataEntryWithFullBuilder.HasNonComputedETag.Should().BeFalse();
            this.odataEntryWithFullBuilder.ETag = null;
            this.odataEntryWithFullBuilder.HasNonComputedETag.Should().BeTrue();
            this.odataEntryWithFullBuilder.ETag.Should().BeNull();
        }

        [TestMethod]
        public void SettingReadLinkToNullShouldOverrideReadLinkFromFullBuilder()
        {
            this.odataEntryWithFullBuilder.ReadLink.OriginalString.Should().Be("http://service/Set(1)/ns.DerivedType");
            this.odataEntryWithFullBuilder.HasNonComputedReadLink.Should().BeFalse();
            this.odataEntryWithFullBuilder.ReadLink = null;
            this.odataEntryWithFullBuilder.HasNonComputedReadLink.Should().BeTrue();
            this.odataEntryWithFullBuilder.ReadLink.Should().BeNull();
        }

        [TestMethod]
        public void SettingEditLinkToNullShouldOverrideEditLinkFromFullBuilder()
        {
            this.odataEntryWithFullBuilder.EditLink.OriginalString.Should().Be("http://service/Set(1)/ns.DerivedType");
            this.odataEntryWithFullBuilder.HasNonComputedEditLink.Should().BeFalse();
            this.odataEntryWithFullBuilder.EditLink = null;
            this.odataEntryWithFullBuilder.HasNonComputedEditLink.Should().BeTrue();
            this.odataEntryWithFullBuilder.EditLink.Should().BeNull();
        }

        [TestMethod]
        public void EntryWithNullBuilderShouldAlwaysReturnNullId()
        {
            this.odataEntryWithNullBuilder.Id = null;
            this.odataEntryWithNullBuilder.Id.Should().BeNull();
        }

        [TestMethod]
        public void EntryWithNullBuilderShouldAlwaysReturnNullETag()
        {
            this.odataEntryWithNullBuilder.ETag = "ETag";
            this.odataEntryWithNullBuilder.ETag.Should().BeNull();
        }

        [TestMethod]
        public void EntryWithNullBuilderShouldAlwaysReturnNullReadLink()
        {
            this.odataEntryWithNullBuilder.ReadLink = new Uri("http://link", UriKind.Absolute);
            this.odataEntryWithNullBuilder.ReadLink.Should().BeNull();
        }

        [TestMethod]
        public void EntryWithNullBuilderShouldAlwaysReturnNullEditLink()
        {
            this.odataEntryWithNullBuilder.EditLink = new Uri("http://link", UriKind.Absolute);
            this.odataEntryWithNullBuilder.EditLink.Should().BeNull();
        }

        [TestMethod]
        public void DefaultEntryShouldHasNoOpBuilder()
        {
            this.odataEntry.MetadataBuilder.Should().BeOfType<NoOpEntityMetadataBuilder>();
        }

        [TestMethod]
        public void ShouldBeAbleToSetAndClearIdOnEntryWithNoOpBuilder()
        {
            this.odataEntry.Id.Should().BeNull();
            this.odataEntry.Id = new Uri("http://my/Id");
            this.odataEntry.Id.ToString().Should().Be("http://my/Id");
            this.odataEntry.Id = null;
            this.odataEntry.Id.Should().BeNull();
        }

        [TestMethod]
        public void ShouldBeAbleToSetAndClearETagOnEntryWithNoOpBuilder()
        {
            this.odataEntry.ETag.Should().BeNull();
            this.odataEntry.ETag = "ETag";
            this.odataEntry.ETag.Should().Be("ETag");
            this.odataEntry.ETag = null;
            this.odataEntry.ETag.Should().BeNull();
        }

        [TestMethod]
        public void ShouldBeAbleToSetAndClearReadLinkOnEntryWithNoOpBuilder()
        {
            this.odataEntry.ReadLink.Should().BeNull();
            this.odataEntry.ReadLink = new Uri("http://link", UriKind.Absolute);
            this.odataEntry.ReadLink.Should().Be(new Uri("http://link", UriKind.Absolute));
            this.odataEntry.ReadLink = null;
            this.odataEntry.ReadLink.Should().BeNull();
        }

        [TestMethod]
        public void ShouldBeAbleToSetAndClearEditLinkOnEntryWithNoOpBuilder()
        {
            this.odataEntry.EditLink.Should().BeNull();
            this.odataEntry.EditLink = new Uri("http://link", UriKind.Absolute);
            this.odataEntry.EditLink.Should().Be(new Uri("http://link", UriKind.Absolute));
            this.odataEntry.EditLink = null;
            this.odataEntry.EditLink.Should().BeNull();
        }

        [TestMethod]
        public void InstanceAnnotationsPropertyShouldNotBeNullAtCreation()
        {
            this.odataEntry.InstanceAnnotations.Should().NotBeNull();
        }

        [TestMethod]
        public void InstanceAnnotationsPropertyShouldReturnAWritableCollectionAtCreation()
        {
            this.odataEntry.InstanceAnnotations.Should().NotBeNull();
            this.odataEntry.InstanceAnnotations.Add(new ODataInstanceAnnotation("namespace.name", new ODataPrimitiveValue("value")));
            this.odataEntry.InstanceAnnotations.Count.Should().Be(1);
        }

        [TestMethod]
        public void SetNullValueToInstanceAnnotationsPropertyShouldThrow()
        {
            Action test = () => this.odataEntry.InstanceAnnotations = null;
            test.ShouldThrow<ArgumentException>().WithMessage("Value cannot be null.\r\nParameter name: value");
        }

        [TestMethod]
        public void SetListValueToInstanceAnnotationsPropertyShouldPass()
        {
            ICollection<ODataInstanceAnnotation> initialCollection = this.odataEntry.InstanceAnnotations;
            ICollection<ODataInstanceAnnotation> newCollection = new List<ODataInstanceAnnotation>();
            this.odataEntry.InstanceAnnotations = newCollection;
            this.odataEntry.InstanceAnnotations.As<object>().Should().BeSameAs(newCollection).And.NotBeSameAs(initialCollection);
        }
        
        [TestMethod]
        public void NewODataEntryShouldContainNullSerializationInfo()
        {
            this.odataEntry.SerializationInfo.Should().BeNull();
        }

        [TestMethod]
        public void SerializationInfoShouldBeValidatedByTheSetter()
        {
            Action action = () => this.odataEntry.SerializationInfo = new ODataFeedAndEntrySerializationInfo();
            action.ShouldThrow<ArgumentNullException>().WithMessage("serializationInfo.NavigationSourceName", ComparisonMode.Substring);
        }

        [TestMethod]
        public void ShouldBeAbleToSetSerializationInfo()
        {
            this.odataEntry.SerializationInfo = new ODataFeedAndEntrySerializationInfo { NavigationSourceName = "Set", NavigationSourceEntityTypeName = "ns.base", ExpectedTypeName = "ns.expected" };
            this.odataEntry.SerializationInfo.NavigationSourceName.Should().Be("Set");
        }

        [TestMethod]
        public void AddNullActionShouldThrow()
        {
            Action action = () => this.odataEntry.AddAction(null);
            action.ShouldThrow<ArgumentNullException>().WithMessage("action", ComparisonMode.Substring);
        }

        [TestMethod]
        public void AddActionShouldWork()
        {
            var action = new ODataAction() {Metadata = new Uri("http://odata.org/metadata"), Target = new Uri("http://odata.org/target"), Title = "TestAction",};
            this.odataEntry.AddAction(action);

            this.odataEntry.Actions.Count().Should().Be(1);
            this.odataEntry.Actions.First().Should().Be(action);
        }

        [TestMethod]
        public void AddNullFunctionShouldThrow()
        {
            Action action = () => this.odataEntry.AddFunction(null);
            action.ShouldThrow<ArgumentNullException>().WithMessage("function", ComparisonMode.Substring);
        }

        [TestMethod]
        public void AddFunctionShouldWork()
        {
            var function = new ODataFunction() { Metadata = new Uri("http://odata.org/metadata"), Target = new Uri("http://odata.org/target"), Title = "TestFunction", };
            this.odataEntry.AddFunction(function);

            this.odataEntry.Functions.Count().Should().Be(1);
            this.odataEntry.Functions.First().Should().Be(function);
        }

        [TestMethod]
        public void AddDuplicateAction()
        {
            var action = new ODataAction() { Metadata = new Uri("http://odata.org/metadata"), Target = new Uri("http://odata.org/target"), Title = "TestAction", };
            this.odataEntry.AddAction(action);
            this.odataEntry.AddAction(action);
            this.odataEntry.Actions.Count().Should().Be(1);
            this.odataEntry.Actions.First().Should().Be(action);
        }

        [TestMethod]
        public void AddDuplicateFunction()
        {
            var function = new ODataFunction() { Metadata = new Uri("http://odata.org/metadata"), Target = new Uri("http://odata.org/target"), Title = "TestFunction", };
            this.odataEntry.AddFunction(function);
            this.odataEntry.AddFunction(function);
            this.odataEntry.Functions.Count().Should().Be(1);
            this.odataEntry.Functions.First().Should().Be(function);
        }
    }
}

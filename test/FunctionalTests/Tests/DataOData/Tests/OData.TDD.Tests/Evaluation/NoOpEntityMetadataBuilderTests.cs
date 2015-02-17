//---------------------------------------------------------------------
// <copyright file="NoOpEntityMetadataBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OData.Core;
using Microsoft.OData.Core.Evaluation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Test.OData.TDD.Tests.Evaluation
{
    [TestClass]
    public class NoOpEntityMetadataBuilderTests
    {
        [TestMethod]
        public void NoOpMetadataBuilderShouldReturnEditLinkSetByUser()
        {
            var editLink = new Uri("http://example.com");
            new NoOpEntityMetadataBuilder(new ODataEntry {EditLink = editLink}).GetEditLink().Should().Be(editLink);
        }

        [TestMethod]
        public void NoOpMetadataBuilderShouldReturnReadLinkSetByUser()
        {
            var readLink = new Uri("http://example.com");
            new NoOpEntityMetadataBuilder(new ODataEntry {ReadLink = readLink}).GetReadLink().Should().Be(readLink);
        }

        [TestMethod]
        public void NoOpMetadataBuilderShouldReturnIdSetByUser()
        {
            var id = new Uri("http://example.com");
            new NoOpEntityMetadataBuilder(new ODataEntry {Id = id}).GetId().Should().Be(id);
        }

        [TestMethod]
        public void GetIdShouldBeNullWhenEntryIsTransient()
        {
            Uri id = new Uri("http://example.com");
            ODataEntry odataEntry = new ODataEntry()
            {
                IsTransient = true,
                Id = id
            };
            new NoOpEntityMetadataBuilder(odataEntry).GetId().Should().BeNull();
        }

        [TestMethod]
        public void NoOpMetadataBuilderShouldReturnETagSetByUser()
        {
            const string etag = "etag value";
            new NoOpEntityMetadataBuilder(new ODataEntry {ETag = etag}).GetETag().Should().Be(etag);
        }

        [TestMethod]
        public void NoOpMetadataBuilderShouldReturnMediaResourceSetByUser()
        {
            var mediaResource = new ODataStreamReferenceValue
                {
                    ContentType = "image/jpeg",
                    EditLink = new Uri("http://example.com/stream/edit"),
                    ReadLink = new Uri("http://example.com/stream/read"),
                    ETag = "stream etag"
                };

            new NoOpEntityMetadataBuilder(new ODataEntry { MediaResource = mediaResource }).GetMediaResource()
                .Should().Be(mediaResource);
        }

        [TestMethod]
        public void NoOpMetadataBuilderShouldReturnAllPropertiesSetByUser()
        {
            var properties = new List<ODataProperty>
                {
                    new ODataProperty {Name = "PrimitiveProperty", Value = 42.2m},
                    new ODataProperty {Name = "StreamProperty", Value = new ODataStreamReferenceValue()},
                    new ODataProperty {Name = "ComplexProperty", Value = new ODataComplexValue()},
                    new ODataProperty {Name = "CollectionProperty", Value = new ODataCollectionValue()}
                };

            new NoOpEntityMetadataBuilder(new ODataEntry {Properties = properties}).GetProperties(properties)
                .Should().HaveCount(4);
        }

        [TestMethod]
        public void NoOpMetadataBuilderShouldReturnActionsSetByUser()
        {
            ODataAction action = new ODataAction()
            {
                Metadata = new Uri("http://example.com/$metadata#Action"),
                Target = new Uri("http://example.com/Action"),
                Title = "ActionTitle"
            };

            ODataEntry entry = new ODataEntry();
            entry.AddAction(action);
            new NoOpEntityMetadataBuilder(entry).GetActions()
                .Should().ContainSingle(a => a == action);
            
            // Verify that the action information wasn't removed or changed.
            action.Metadata.Should().Be(new Uri("http://example.com/$metadata#Action"));
            action.Target.Should().Be(new Uri("http://example.com/Action"));
            action.Title.Should().Be("ActionTitle");
        }

        [TestMethod]
        public void NoOpMetadataBuilderShouldReturnFunctionsSetByUser()
        {
            ODataFunction function = new ODataFunction()
            {
                Metadata = new Uri("http://example.com/$metadata#Function"),
                Target = new Uri("http://example.com/Function"),
                Title = "FunctionTitle"
            };

            ODataEntry entry = new ODataEntry();
            entry.AddFunction(function);

            new NoOpEntityMetadataBuilder(entry).GetFunctions()
                .Should().ContainSingle(f => f == function);
            
            // Verify that the Function information wasn't removed or changed.
            function.Metadata.Should().Be(new Uri("http://example.com/$metadata#Function"));
            function.Target.Should().Be(new Uri("http://example.com/Function"));
            function.Title.Should().Be("FunctionTitle");
        }

        [TestMethod]
        public void NoOpMetadataBuilderShouldReturnNavigationLinkSetByUser()
        {
            new NoOpEntityMetadataBuilder(new ODataEntry()).GetNavigationLinkUri("navProp", new Uri("http://example.com/navLink"), false)
                .Should().Be(new Uri("http://example.com/navLink"));
            new NoOpEntityMetadataBuilder(new ODataEntry()).GetNavigationLinkUri("navProp", new Uri("http://example.com/navLink"), true)
                .Should().Be(new Uri("http://example.com/navLink"));
        }

        [TestMethod]
        public void NoOpMetadataBuilderShouldReturnAssociationLinkSetByUser()
        {
            new NoOpEntityMetadataBuilder(new ODataEntry()).GetAssociationLinkUri("navProp", new Uri("http://example.com/associationLink"), false)
                .Should().Be(new Uri("http://example.com/associationLink"));
            new NoOpEntityMetadataBuilder(new ODataEntry()).GetAssociationLinkUri("navProp", new Uri("http://example.com/associationLink"), true)
                .Should().Be(new Uri("http://example.com/associationLink"));
        }

        [TestMethod]
        public void NoOpMetadataBuilderShouldNotConstructEditLink()
        {
            new NoOpEntityMetadataBuilder(new ODataEntry()).GetEditLink().Should().BeNull();
        }

        [TestMethod]
        public void NoOpMetadataBuilderShouldNotConstructReadLink()
        {
            new NoOpEntityMetadataBuilder(new ODataEntry()).GetReadLink().Should().BeNull();
        }

        [TestMethod]
        public void NoOpMetadataBuilderShouldNotConstructId()
        {
            new NoOpEntityMetadataBuilder(new ODataEntry()).GetId().Should().BeNull();
        }

        [TestMethod]
        public void NoOpMetadataBuilderShouldNotConstructETag()
        {
            new NoOpEntityMetadataBuilder(new ODataEntry()).GetETag().Should().BeNull();
        }

        [TestMethod]
        public void NoOpMetadataBuilderShouldNotConstructMediaResource()
        {
            new NoOpEntityMetadataBuilder(new ODataEntry()).GetMediaResource().Should().BeNull();
        }

        [TestMethod]
        public void NoOpMetadataBuilderShouldNotConstructProperties()
        {
            new NoOpEntityMetadataBuilder(new ODataEntry()).GetProperties(new ODataProperty[]{})
                .Should().BeEmpty();
        }

        [TestMethod]
        public void NoOpMetadataBuilderShouldNotConstructActions()
        {
            new NoOpEntityMetadataBuilder(new ODataEntry()).GetActions().Count().Should().Be(0);
        }

        [TestMethod]
        public void NoOpMetadataBuilderShouldNotConstructFunctions()
        {
            new NoOpEntityMetadataBuilder(new ODataEntry()).GetFunctions().Count().Should().Be(0);
        }

        [TestMethod]
        public void NoOpMetadataBuilderShouldNotConstructNavigationLink()
        {
            new NoOpEntityMetadataBuilder(new ODataEntry()).GetNavigationLinkUri("navProp", null, false).Should().BeNull();
            new NoOpEntityMetadataBuilder(new ODataEntry()).GetNavigationLinkUri("navProp", null, true).Should().BeNull();
        }

        [TestMethod]
        public void NoOpMetadataBuilderShouldNotConstructAssociationLink()
        {
            new NoOpEntityMetadataBuilder(new ODataEntry()).GetAssociationLinkUri("navProp", null, false).Should().BeNull();
            new NoOpEntityMetadataBuilder(new ODataEntry()).GetAssociationLinkUri("navProp", null, true).Should().BeNull();
        }

        [TestMethod]
        public void TryGetIdForSerializationShouldBeNullWhenEntryIsTransient()
        {
            ODataEntry odataEntry = new ODataEntry()
            {
                IsTransient =  true
            };
            Uri id;
            new NoOpEntityMetadataBuilder(odataEntry).TryGetIdForSerialization(out id).Should().BeTrue();
            id.Should().BeNull();
        }

        [TestMethod]
        public void TryGetIdForSerializationShouldBeNullWhenEntryIsNotTransient()
        {
            ODataEntry odataEntry = new ODataEntry()
            {
                IsTransient = false
            };
            Uri id;
            new NoOpEntityMetadataBuilder(odataEntry).TryGetIdForSerialization(out id).Should().BeFalse();
            id.Should().BeNull();
        }
    }
}

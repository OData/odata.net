//---------------------------------------------------------------------
// <copyright file="NoOpEntityMetadataBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OData.Core.Evaluation;
using Xunit;

namespace Microsoft.OData.Core.Tests.Evaluation
{
    public class NoOpEntityMetadataBuilderTests
    {
        [Fact]
        public void NoOpMetadataBuilderShouldReturnEditLinkSetByUser()
        {
            var editLink = new Uri("http://example.com");
            new NoOpEntityMetadataBuilder(new ODataEntry {EditLink = editLink}).GetEditLink().Should().Be(editLink);
        }

        [Fact]
        public void NoOpMetadataBuilderShouldReturnReadLinkSetByUser()
        {
            var readLink = new Uri("http://example.com");
            new NoOpEntityMetadataBuilder(new ODataEntry {ReadLink = readLink}).GetReadLink().Should().Be(readLink);
        }

        [Fact]
        public void NoOpMetadataBuilderShouldReturnIdSetByUser()
        {
            var id = new Uri("http://example.com");
            new NoOpEntityMetadataBuilder(new ODataEntry {Id = id}).GetId().Should().Be(id);
        }

        [Fact]
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

        [Fact]
        public void NoOpMetadataBuilderShouldReturnETagSetByUser()
        {
            const string etag = "etag value";
            new NoOpEntityMetadataBuilder(new ODataEntry {ETag = etag}).GetETag().Should().Be(etag);
        }

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
        public void NoOpMetadataBuilderShouldReturnNavigationLinkSetByUser()
        {
            new NoOpEntityMetadataBuilder(new ODataEntry()).GetNavigationLinkUri("navProp", new Uri("http://example.com/navLink"), false)
                .Should().Be(new Uri("http://example.com/navLink"));
            new NoOpEntityMetadataBuilder(new ODataEntry()).GetNavigationLinkUri("navProp", new Uri("http://example.com/navLink"), true)
                .Should().Be(new Uri("http://example.com/navLink"));
        }

        [Fact]
        public void NoOpMetadataBuilderShouldReturnAssociationLinkSetByUser()
        {
            new NoOpEntityMetadataBuilder(new ODataEntry()).GetAssociationLinkUri("navProp", new Uri("http://example.com/associationLink"), false)
                .Should().Be(new Uri("http://example.com/associationLink"));
            new NoOpEntityMetadataBuilder(new ODataEntry()).GetAssociationLinkUri("navProp", new Uri("http://example.com/associationLink"), true)
                .Should().Be(new Uri("http://example.com/associationLink"));
        }

        [Fact]
        public void NoOpMetadataBuilderShouldNotConstructEditLink()
        {
            new NoOpEntityMetadataBuilder(new ODataEntry()).GetEditLink().Should().BeNull();
        }

        [Fact]
        public void NoOpMetadataBuilderShouldNotConstructReadLink()
        {
            new NoOpEntityMetadataBuilder(new ODataEntry()).GetReadLink().Should().BeNull();
        }

        [Fact]
        public void NoOpMetadataBuilderShouldNotConstructId()
        {
            new NoOpEntityMetadataBuilder(new ODataEntry()).GetId().Should().BeNull();
        }

        [Fact]
        public void NoOpMetadataBuilderShouldNotConstructETag()
        {
            new NoOpEntityMetadataBuilder(new ODataEntry()).GetETag().Should().BeNull();
        }

        [Fact]
        public void NoOpMetadataBuilderShouldNotConstructMediaResource()
        {
            new NoOpEntityMetadataBuilder(new ODataEntry()).GetMediaResource().Should().BeNull();
        }

        [Fact]
        public void NoOpMetadataBuilderShouldNotConstructProperties()
        {
            new NoOpEntityMetadataBuilder(new ODataEntry()).GetProperties(new ODataProperty[]{})
                .Should().BeEmpty();
        }

        [Fact]
        public void NoOpMetadataBuilderShouldNotConstructActions()
        {
            new NoOpEntityMetadataBuilder(new ODataEntry()).GetActions().Count().Should().Be(0);
        }

        [Fact]
        public void NoOpMetadataBuilderShouldNotConstructFunctions()
        {
            new NoOpEntityMetadataBuilder(new ODataEntry()).GetFunctions().Count().Should().Be(0);
        }

        [Fact]
        public void NoOpMetadataBuilderShouldNotConstructNavigationLink()
        {
            new NoOpEntityMetadataBuilder(new ODataEntry()).GetNavigationLinkUri("navProp", null, false).Should().BeNull();
            new NoOpEntityMetadataBuilder(new ODataEntry()).GetNavigationLinkUri("navProp", null, true).Should().BeNull();
        }

        [Fact]
        public void NoOpMetadataBuilderShouldNotConstructAssociationLink()
        {
            new NoOpEntityMetadataBuilder(new ODataEntry()).GetAssociationLinkUri("navProp", null, false).Should().BeNull();
            new NoOpEntityMetadataBuilder(new ODataEntry()).GetAssociationLinkUri("navProp", null, true).Should().BeNull();
        }

        [Fact]
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

        [Fact]
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

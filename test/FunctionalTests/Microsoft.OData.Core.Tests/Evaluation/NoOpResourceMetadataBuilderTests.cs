//---------------------------------------------------------------------
// <copyright file="NoOpEntityMetadataBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OData.Evaluation;
using Xunit;

namespace Microsoft.OData.Tests.Evaluation
{
    public class NoOpResourceMetadataBuilderTests
    {
        [Fact]
        public void NoOpMetadataBuilderShouldReturnEditLinkSetByUser()
        {
            var editLink = new Uri("http://example.com");
            new NoOpResourceMetadataBuilder(new ODataResource {EditLink = editLink}).GetEditLink().Should().Be(editLink);
        }

        [Fact]
        public void NoOpMetadataBuilderShouldReturnReadLinkSetByUser()
        {
            var readLink = new Uri("http://example.com");
            new NoOpResourceMetadataBuilder(new ODataResource {ReadLink = readLink}).GetReadLink().Should().Be(readLink);
        }

        [Fact]
        public void NoOpMetadataBuilderShouldReturnIdSetByUser()
        {
            var id = new Uri("http://example.com");
            new NoOpResourceMetadataBuilder(new ODataResource {Id = id}).GetId().Should().Be(id);
        }

        [Fact]
        public void GetIdShouldBeNullWhenEntryIsTransient()
        {
            Uri id = new Uri("http://example.com");
            ODataResource odataEntry = new ODataResource()
            {
                IsTransient = true,
                Id = id
            };
            new NoOpResourceMetadataBuilder(odataEntry).GetId().Should().BeNull();
        }

        [Fact]
        public void NoOpMetadataBuilderShouldReturnETagSetByUser()
        {
            const string etag = "etag value";
            new NoOpResourceMetadataBuilder(new ODataResource {ETag = etag}).GetETag().Should().Be(etag);
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

            new NoOpResourceMetadataBuilder(new ODataResource { MediaResource = mediaResource }).GetMediaResource()
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

            new NoOpResourceMetadataBuilder(new ODataResource {Properties = properties}).GetProperties(properties)
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

            ODataResource entry = new ODataResource();
            entry.AddAction(action);
            new NoOpResourceMetadataBuilder(entry).GetActions()
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

            ODataResource entry = new ODataResource();
            entry.AddFunction(function);

            new NoOpResourceMetadataBuilder(entry).GetFunctions()
                .Should().ContainSingle(f => f == function);
            
            // Verify that the Function information wasn't removed or changed.
            function.Metadata.Should().Be(new Uri("http://example.com/$metadata#Function"));
            function.Target.Should().Be(new Uri("http://example.com/Function"));
            function.Title.Should().Be("FunctionTitle");
        }

        [Fact]
        public void NoOpMetadataBuilderShouldReturnNavigationLinkSetByUser()
        {
            new NoOpResourceMetadataBuilder(new ODataResource()).GetNavigationLinkUri("navProp", new Uri("http://example.com/navLink"), false)
                .Should().Be(new Uri("http://example.com/navLink"));
            new NoOpResourceMetadataBuilder(new ODataResource()).GetNavigationLinkUri("navProp", new Uri("http://example.com/navLink"), true)
                .Should().Be(new Uri("http://example.com/navLink"));
        }

        [Fact]
        public void NoOpMetadataBuilderShouldReturnAssociationLinkSetByUser()
        {
            new NoOpResourceMetadataBuilder(new ODataResource()).GetAssociationLinkUri("navProp", new Uri("http://example.com/associationLink"), false)
                .Should().Be(new Uri("http://example.com/associationLink"));
            new NoOpResourceMetadataBuilder(new ODataResource()).GetAssociationLinkUri("navProp", new Uri("http://example.com/associationLink"), true)
                .Should().Be(new Uri("http://example.com/associationLink"));
        }

        [Fact]
        public void NoOpMetadataBuilderShouldNotConstructEditLink()
        {
            new NoOpResourceMetadataBuilder(new ODataResource()).GetEditLink().Should().BeNull();
        }

        [Fact]
        public void NoOpMetadataBuilderShouldNotConstructReadLink()
        {
            new NoOpResourceMetadataBuilder(new ODataResource()).GetReadLink().Should().BeNull();
        }

        [Fact]
        public void NoOpMetadataBuilderShouldNotConstructId()
        {
            new NoOpResourceMetadataBuilder(new ODataResource()).GetId().Should().BeNull();
        }

        [Fact]
        public void NoOpMetadataBuilderShouldNotConstructETag()
        {
            new NoOpResourceMetadataBuilder(new ODataResource()).GetETag().Should().BeNull();
        }

        [Fact]
        public void NoOpMetadataBuilderShouldNotConstructMediaResource()
        {
            new NoOpResourceMetadataBuilder(new ODataResource()).GetMediaResource().Should().BeNull();
        }

        [Fact]
        public void NoOpMetadataBuilderShouldNotConstructProperties()
        {
            new NoOpResourceMetadataBuilder(new ODataResource()).GetProperties(new ODataProperty[]{})
                .Should().BeEmpty();
        }

        [Fact]
        public void NoOpMetadataBuilderShouldNotConstructActions()
        {
            new NoOpResourceMetadataBuilder(new ODataResource()).GetActions().Count().Should().Be(0);
        }

        [Fact]
        public void NoOpMetadataBuilderShouldNotConstructFunctions()
        {
            new NoOpResourceMetadataBuilder(new ODataResource()).GetFunctions().Count().Should().Be(0);
        }

        [Fact]
        public void NoOpMetadataBuilderShouldNotConstructNavigationLink()
        {
            new NoOpResourceMetadataBuilder(new ODataResource()).GetNavigationLinkUri("navProp", null, false).Should().BeNull();
            new NoOpResourceMetadataBuilder(new ODataResource()).GetNavigationLinkUri("navProp", null, true).Should().BeNull();
        }

        [Fact]
        public void NoOpMetadataBuilderShouldNotConstructAssociationLink()
        {
            new NoOpResourceMetadataBuilder(new ODataResource()).GetAssociationLinkUri("navProp", null, false).Should().BeNull();
            new NoOpResourceMetadataBuilder(new ODataResource()).GetAssociationLinkUri("navProp", null, true).Should().BeNull();
        }

        [Fact]
        public void TryGetIdForSerializationShouldBeNullWhenEntryIsTransient()
        {
            ODataResource odataEntry = new ODataResource()
            {
                IsTransient =  true
            };
            Uri id;
            new NoOpResourceMetadataBuilder(odataEntry).TryGetIdForSerialization(out id).Should().BeTrue();
            id.Should().BeNull();
        }

        [Fact]
        public void TryGetIdForSerializationShouldBeNullWhenEntryIsNotTransient()
        {
            ODataResource odataEntry = new ODataResource()
            {
                IsTransient = false
            };
            Uri id;
            new NoOpResourceMetadataBuilder(odataEntry).TryGetIdForSerialization(out id).Should().BeFalse();
            id.Should().BeNull();
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="NoOpEntityMetadataBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;
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
            Assert.Equal(editLink, new NoOpResourceMetadataBuilder(new ODataResource {EditLink = editLink}).GetEditLink());
        }

        [Fact]
        public void NoOpMetadataBuilderShouldReturnReadLinkSetByUser()
        {
            var readLink = new Uri("http://example.com");
            Assert.Equal(readLink, new NoOpResourceMetadataBuilder(new ODataResource {ReadLink = readLink}).GetReadLink());
        }

        [Fact]
        public void NoOpMetadataBuilderShouldReturnIdSetByUser()
        {
            var id = new Uri("http://example.com");
            Assert.Equal(id, new NoOpResourceMetadataBuilder(new ODataResource {Id = id}).GetId());
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

            Assert.Null(new NoOpResourceMetadataBuilder(odataEntry).GetId());
        }

        [Fact]
        public void NoOpMetadataBuilderShouldReturnETagSetByUser()
        {
            const string etag = "etag value";
            Assert.Equal(etag, new NoOpResourceMetadataBuilder(new ODataResource {ETag = etag}).GetETag());
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

            Assert.Same(mediaResource, new NoOpResourceMetadataBuilder(new ODataResource { MediaResource = mediaResource }).GetMediaResource());
        }

        [Fact]
        public void NoOpMetadataBuilderShouldReturnAllPropertiesSetByUser()
        {
            var properties = new List<ODataProperty>
                {
                    new ODataProperty {Name = "PrimitiveProperty", Value = 42.2m},
                    new ODataProperty {Name = "StreamProperty", Value = new ODataStreamReferenceValue()},
                    new ODataProperty {Name = "CollectionProperty", Value = new ODataCollectionValue()}
                };

            Assert.Equal(3, new NoOpResourceMetadataBuilder(new ODataResource { Properties = properties }).GetProperties(properties).Count());
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
            Assert.Single(new NoOpResourceMetadataBuilder(entry).GetActions().Where(a => a == action));

            // Verify that the action information wasn't removed or changed.
            Assert.Equal(new Uri("http://example.com/$metadata#Action"), action.Metadata);
            Assert.Equal(new Uri("http://example.com/Action"), action.Target);
            Assert.Equal("ActionTitle", action.Title);
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

            Assert.Single(new NoOpResourceMetadataBuilder(entry).GetFunctions().Where(f => f == function));

            // Verify that the Function information wasn't removed or changed.
            Assert.Equal(new Uri("http://example.com/$metadata#Function"), function.Metadata);
            Assert.Equal(new Uri("http://example.com/Function"), function.Target);
            Assert.Equal("FunctionTitle", function.Title);
        }

        [Fact]
        public void NoOpMetadataBuilderShouldReturnNavigationLinkSetByUser()
        {
            Assert.Equal(new Uri("http://example.com/navLink"),
                new NoOpResourceMetadataBuilder(new ODataResource()).GetNavigationLinkUri("navProp", new Uri("http://example.com/navLink"), false));

            Assert.Equal(new Uri("http://example.com/navLink"),
                new NoOpResourceMetadataBuilder(new ODataResource()).GetNavigationLinkUri("navProp", new Uri("http://example.com/navLink"), true));
        }

        [Fact]
        public void NoOpMetadataBuilderShouldReturnAssociationLinkSetByUser()
        {
            Assert.Equal(new Uri("http://example.com/associationLink"),
                new NoOpResourceMetadataBuilder(new ODataResource()).GetAssociationLinkUri("navProp", new Uri("http://example.com/associationLink"), false));

            Assert.Equal(new Uri("http://example.com/associationLink"),
                new NoOpResourceMetadataBuilder(new ODataResource()).GetAssociationLinkUri("navProp", new Uri("http://example.com/associationLink"), true));
        }

        [Fact]
        public void NoOpMetadataBuilderShouldNotConstructEditLink()
        {
            Assert.Null(new NoOpResourceMetadataBuilder(new ODataResource()).GetEditLink());
        }

        [Fact]
        public void NoOpMetadataBuilderShouldNotConstructReadLink()
        {
            Assert.Null(new NoOpResourceMetadataBuilder(new ODataResource()).GetReadLink());
        }

        [Fact]
        public void NoOpMetadataBuilderShouldNotConstructId()
        {
            Assert.Null(new NoOpResourceMetadataBuilder(new ODataResource()).GetId());
        }

        [Fact]
        public void NoOpMetadataBuilderShouldNotConstructETag()
        {
            Assert.Null(new NoOpResourceMetadataBuilder(new ODataResource()).GetETag());
        }

        [Fact]
        public void NoOpMetadataBuilderShouldNotConstructMediaResource()
        {
            Assert.Null(new NoOpResourceMetadataBuilder(new ODataResource()).GetMediaResource());
        }

        [Fact]
        public void NoOpMetadataBuilderShouldNotConstructProperties()
        {
            Assert.Empty(new NoOpResourceMetadataBuilder(new ODataResource()).GetProperties(new ODataProperty[] { }));
        }

        [Fact]
        public void NoOpMetadataBuilderShouldNotConstructActions()
        {
            Assert.Empty(new NoOpResourceMetadataBuilder(new ODataResource()).GetActions());
        }

        [Fact]
        public void NoOpMetadataBuilderShouldNotConstructFunctions()
        {
            Assert.Empty(new NoOpResourceMetadataBuilder(new ODataResource()).GetFunctions());
        }

        [Fact]
        public void NoOpMetadataBuilderShouldNotConstructNavigationLink()
        {
            Assert.Null(new NoOpResourceMetadataBuilder(new ODataResource()).GetNavigationLinkUri("navProp", null, false));
            Assert.Null(new NoOpResourceMetadataBuilder(new ODataResource()).GetNavigationLinkUri("navProp", null, true));
        }

        [Fact]
        public void NoOpMetadataBuilderShouldNotConstructAssociationLink()
        {
            Assert.Null(new NoOpResourceMetadataBuilder(new ODataResource()).GetAssociationLinkUri("navProp", null, false));
            Assert.Null(new NoOpResourceMetadataBuilder(new ODataResource()).GetAssociationLinkUri("navProp", null, true));
        }

        [Fact]
        public void TryGetIdForSerializationShouldBeNullWhenEntryIsTransient()
        {
            ODataResource odataEntry = new ODataResource()
            {
                IsTransient =  true
            };
            Uri id;
            Assert.True(new NoOpResourceMetadataBuilder(odataEntry).TryGetIdForSerialization(out id));
            Assert.Null(id);
        }

        [Fact]
        public void TryGetIdForSerializationShouldBeNullWhenEntryIsNotTransient()
        {
            ODataResource odataEntry = new ODataResource()
            {
                IsTransient = false
            };
            Uri id;
            Assert.False(new NoOpResourceMetadataBuilder(odataEntry).TryGetIdForSerialization(out id));
            Assert.Null(id);
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="NullEntityMetadataBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Evaluation;
using Xunit;

namespace Microsoft.OData.Tests.Evaluation
{
    public class NullEntityMetadataBuilderTests
    {
        private readonly ODataResourceMetadataBuilder testSubject = ODataResourceMetadataBuilder.Null;

        [Fact]
        public void NullEntityMetadataBuilderShouldReturnNullEditLink()
        {
            Assert.Null(this.testSubject.GetEditLink());
        }

        [Fact]
        public void NullEntityMetadataBuilderShouldReturnNullId()
        {
            Assert.Null(this.testSubject.GetId());
        }

        [Fact]
        public void NullEntityMetadataBuilderShouldReturnNullReadLink()
        {
            Assert.Null(this.testSubject.GetReadLink());
        }

        [Fact]
        public void NullEntityMetadataBuilderShouldReturnNullETag()
        {
            Assert.Null(this.testSubject.GetETag());
        }

        [Fact]
        public void NullEntityMetadataBuilderShouldReturnNullStreamEditLink()
        {
            Assert.Null(this.testSubject.GetStreamEditLink("streamName"));
        }

        [Fact]
        public void NullEntityMetadataBuilderShouldReturnNullStreamReadLink()
        {
            Assert.Null(this.testSubject.GetStreamReadLink("streamName"));
        }

        [Fact]
        public void NullEntityMetadataBuilderShouldOmitStreamPropertiesFromPropertiesEnumerable()
        {
            List<ODataProperty> properties = new List<ODataProperty>()
                {
                    new ODataProperty() {Name = "IntegerProperty", Value = 42},
                    new ODataProperty() {Name = "StreamProperty", Value = new ODataStreamReferenceValue()}
                };

            Assert.Single(this.testSubject.GetProperties(properties).Where(p => p.Name == "IntegerProperty"));
        }

        [Fact]
        public void NullEntityMetadataBuilderShouldReturnPropertiesOfTypeODataPropertyInfo()
        {
            var properties = new List<ODataPropertyInfo>
            {
                new ODataProperty { Name = "Id", Value = 1 },
                new ODataProperty { Name = "Name", Value = "foobar" },
                new ODataPropertyInfo { Name = "Age" }
            };

            Assert.Equal(3, this.testSubject.GetProperties(properties).Count());
        }

        [Fact]
        public void NullEntityMetadataBuilderShouldNotReturnPropertiesOfTypeODataStreamPropertyInfo()
        {
            var properties = new List<ODataPropertyInfo>
            {
                new ODataProperty { Name = "Id", Value = 1 },
                new ODataProperty { Name = "Name", Value = "foobar" },
                new ODataStreamPropertyInfo { Name = "Photo" }
            };

            Assert.Equal(2, this.testSubject.GetProperties(properties).Count());
        }

        [Fact]
        public void NullEntityMetadataBuilderShouldReturnNullMediaResource()
        {
            Assert.Null(this.testSubject.GetMediaResource());
        }

        [Fact]
        public void NullEntityMetadataBuilderShouldReturnNullActions()
        {
            Assert.Null(this.testSubject.GetActions());
        }

        [Fact]
        public void NullEntityMetadataBuilderShouldReturnNullFunctions()
        {
            Assert.Null(this.testSubject.GetFunctions());
        }
    }
}

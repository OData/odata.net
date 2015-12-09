//---------------------------------------------------------------------
// <copyright file="NullEntityMetadataBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OData.Core.Evaluation;
using Xunit;

namespace Microsoft.OData.Core.Tests.Evaluation
{
    public class NullEntityMetadataBuilderTests
    {
        private readonly ODataEntityMetadataBuilder testSubject = ODataEntityMetadataBuilder.Null;

        [Fact]
        public void NullEntityMetadataBuilderShouldReturnNullEditLink()
        {
            this.testSubject.GetEditLink().Should().BeNull();
        }

        [Fact]
        public void NullEntityMetadataBuilderShouldReturnNullId()
        {
            this.testSubject.GetId().Should().BeNull();
        }

        [Fact]
        public void NullEntityMetadataBuilderShouldReturnNullReadLink()
        {
            this.testSubject.GetReadLink().Should().BeNull();
        }

        [Fact]
        public void NullEntityMetadataBuilderShouldReturnNullETag()
        {
            this.testSubject.GetETag().Should().BeNull();
        }

        [Fact]
        public void NullEntityMetadataBuilderShouldReturnNullStreamEditLink()
        {
            this.testSubject.GetStreamEditLink("streamName").Should().BeNull();
        }

        [Fact]
        public void NullEntityMetadataBuilderShouldReturnNullStreamReadLink()
        {
            this.testSubject.GetStreamReadLink("streamName").Should().BeNull();
        }

        [Fact]
        public void NullEntityMetadataBuilderShouldOmitStreamPropertiesFromPropertiesEnumerable()
        {
            List<ODataProperty> properties = new List<ODataProperty>()
                {
                    new ODataProperty() {Name = "IntegerProperty", Value = 42},
                    new ODataProperty() {Name = "StreamProperty", Value = new ODataStreamReferenceValue()}
                };

            this.testSubject.GetProperties(properties).Should().OnlyContain(p => p.Name == "IntegerProperty");
        }

        [Fact]
        public void NullEntityMetadataBuilderShouldReturnNullMediaResource()
        {
            this.testSubject.GetMediaResource().Should().BeNull();
        }

        [Fact]
        public void NullEntityMetadataBuilderShouldReturnNullActions()
        {
            this.testSubject.GetActions().Should().BeNull();
        }

        [Fact]
        public void NullEntityMetadataBuilderShouldReturnNullFunctions()
        {
            this.testSubject.GetFunctions().Should().BeNull();
        }
    }
}

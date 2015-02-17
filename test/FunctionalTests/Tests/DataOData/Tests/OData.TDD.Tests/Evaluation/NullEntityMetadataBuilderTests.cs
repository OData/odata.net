//---------------------------------------------------------------------
// <copyright file="NullEntityMetadataBuilderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OData.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.OData.Core.Evaluation;

namespace Microsoft.Test.OData.TDD.Tests.Evaluation
{
    [TestClass]
    public class NullEntityMetadataBuilderTests
    {
        private readonly ODataEntityMetadataBuilder testSubject = ODataEntityMetadataBuilder.Null;

        [TestMethod]
        public void NullEntityMetadataBuilderShouldReturnNullEditLink()
        {
            this.testSubject.GetEditLink().Should().BeNull();
        }

        [TestMethod]
        public void NullEntityMetadataBuilderShouldReturnNullId()
        {
            this.testSubject.GetId().Should().BeNull();
        }

        [TestMethod]
        public void NullEntityMetadataBuilderShouldReturnNullReadLink()
        {
            this.testSubject.GetReadLink().Should().BeNull();
        }

        [TestMethod]
        public void NullEntityMetadataBuilderShouldReturnNullETag()
        {
            this.testSubject.GetETag().Should().BeNull();
        }

        [TestMethod]
        public void NullEntityMetadataBuilderShouldReturnNullStreamEditLink()
        {
            this.testSubject.GetStreamEditLink("streamName").Should().BeNull();
        }

        [TestMethod]
        public void NullEntityMetadataBuilderShouldReturnNullStreamReadLink()
        {
            this.testSubject.GetStreamReadLink("streamName").Should().BeNull();
        }

        [TestMethod]
        public void NullEntityMetadataBuilderShouldOmitStreamPropertiesFromPropertiesEnumerable()
        {
            List<ODataProperty> properties = new List<ODataProperty>()
                {
                    new ODataProperty() {Name = "IntegerProperty", Value = 42},
                    new ODataProperty() {Name = "StreamProperty", Value = new ODataStreamReferenceValue()}
                };

            this.testSubject.GetProperties(properties).Should().OnlyContain(p => p.Name == "IntegerProperty");
        }

        [TestMethod]
        public void NullEntityMetadataBuilderShouldReturnNullMediaResource()
        {
            this.testSubject.GetMediaResource().Should().BeNull();
        }

        [TestMethod]
        public void NullEntityMetadataBuilderShouldReturnNullActions()
        {
            this.testSubject.GetActions().Should().BeNull();
        }

        [TestMethod]
        public void NullEntityMetadataBuilderShouldReturnNullFunctions()
        {
            this.testSubject.GetFunctions().Should().BeNull();
        }
    }
}

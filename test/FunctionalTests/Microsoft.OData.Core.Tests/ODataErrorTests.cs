//---------------------------------------------------------------------
// <copyright file="ODataErrorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Microsoft.OData.Core.Tests
{
    public class ODataErrorTests
    {
        private ODataError odataError;

        public ODataErrorTests()
        {
            this.odataError = new ODataError();
        }

        [Fact]
        public void InstanceAnnotationsPropertyShouldNotBeNullAtCreation()
        {
            this.odataError.InstanceAnnotations.Should().NotBeNull();
        }

        [Fact]
        public void InstanceAnnotationsPropertyShouldReturnAWritableCollectionAtCreation()
        {
            this.odataError.InstanceAnnotations.Should().NotBeNull();
            this.odataError.InstanceAnnotations.Add(new ODataInstanceAnnotation("namespace.name", new ODataPrimitiveValue("value")));
            this.odataError.InstanceAnnotations.Count.Should().Be(1);
        }

        [Fact]
        public void SetNullValueToInstanceAnnotationsPropertyShouldThrow()
        {
            Action test = () => this.odataError.InstanceAnnotations = null;
            test.ShouldThrow<ArgumentException>().WithMessage("Value cannot be null.\r\nParameter name: value");
        }

        [Fact]
        public void SetListValueToInstanceAnnotationsPropertyShouldPass()
        {
            ICollection<ODataInstanceAnnotation> initialCollection = this.odataError.InstanceAnnotations;
            ICollection<ODataInstanceAnnotation> newCollection = new List<ODataInstanceAnnotation>();
            this.odataError.InstanceAnnotations = newCollection;
            this.odataError.InstanceAnnotations.As<object>().Should().BeSameAs(newCollection).And.NotBeSameAs(initialCollection);
        }

        [Fact]
        public void GetInstanceAnnotationsForWritingShouldReturnEmptyInstanceAnnotationsFromNewODataError()
        {
            this.odataError.InstanceAnnotations.Should().NotBeNull().And.BeEmpty();
        }

        [Fact]
        public void GetInstanceAnnotationsForWritingShouldReturnInstanceAnnotationsFromODataErrorWithInstanceAnnotations()
        {
            ODataInstanceAnnotation instanceAnnotation = new ODataInstanceAnnotation("namespace.name", new ODataPrimitiveValue("value"));
            this.odataError.InstanceAnnotations.Add(instanceAnnotation);
            this.odataError.InstanceAnnotations.Should().Contain(instanceAnnotation).And.HaveCount(1);
        }
    }
}

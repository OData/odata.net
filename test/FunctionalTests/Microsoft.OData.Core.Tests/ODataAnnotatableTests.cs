//---------------------------------------------------------------------
// <copyright file="ODataAnnotatableTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Microsoft.OData.Core.Tests
{
    public class ODataAnnotatableTests
    {
        private ODataAnnotatable annotatable;

        public ODataAnnotatableTests()
        {
            this.annotatable = new ODataFeed();
        }

        [Fact]
        public void GetInstanceAnnotationsShouldReturnNonNullCollectionAtConstruction()
        {
            this.annotatable.GetInstanceAnnotations().Should().BeEmpty().And.Should().NotBeNull();
        }

        [Fact]
        public void SetInstanceAnnotationsShouldThrowOnNull()
        {
            Action test = () => this.annotatable.SetInstanceAnnotations(null);
            test.ShouldThrow<ArgumentNullException>("Value cannot be null.\r\nParameter name: instanceAnnotations");
        }

        [Fact]
        public void SetInstanceAnnotationsShouldUpdateValue()
        {
            var instanceAnnotations = new List<ODataInstanceAnnotation>();
            this.annotatable.SetInstanceAnnotations(instanceAnnotations);
            this.annotatable.GetInstanceAnnotations().As<object>().Should().BeSameAs(instanceAnnotations);
        }
    }
}

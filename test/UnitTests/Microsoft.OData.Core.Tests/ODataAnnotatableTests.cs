//---------------------------------------------------------------------
// <copyright file="ODataAnnotatableTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class ODataAnnotatableTests
    {
        private ODataAnnotatable annotatable;

        public ODataAnnotatableTests()
        {
            this.annotatable = new ODataResourceSet();
        }

        [Fact]
        public void GetInstanceAnnotationsShouldReturnNonNullCollectionAtConstruction()
        {
            var annotations = this.annotatable.GetInstanceAnnotations();
            Assert.NotNull(annotations);
            Assert.Empty(annotations);
        }

        [Fact]
        public void SetInstanceAnnotationsShouldThrowOnNull()
        {
            Action test = () => this.annotatable.SetInstanceAnnotations(null);
            Assert.Throws<ArgumentNullException>("value", test);
        }

        [Fact]
        public void SetInstanceAnnotationsShouldUpdateValue()
        {
            var instanceAnnotations = new List<ODataInstanceAnnotation>();
            this.annotatable.SetInstanceAnnotations(instanceAnnotations);
            var annotations = this.annotatable.GetInstanceAnnotations();
            Assert.Same(annotations, instanceAnnotations);
        }
    }
}

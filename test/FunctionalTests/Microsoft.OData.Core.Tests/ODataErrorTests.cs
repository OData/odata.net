//---------------------------------------------------------------------
// <copyright file="ODataErrorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Xunit;

namespace Microsoft.OData.Tests
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
            Assert.NotNull(this.odataError.InstanceAnnotations);
        }

        [Fact]
        public void InstanceAnnotationsPropertyShouldReturnAWritableCollectionAtCreation()
        {
            Assert.NotNull(this.odataError.InstanceAnnotations);
            this.odataError.InstanceAnnotations.Add(new ODataInstanceAnnotation("namespace.name", new ODataPrimitiveValue("value")));
            Assert.Single(this.odataError.InstanceAnnotations);
        }

        [Fact]
        public void SetNullValueToInstanceAnnotationsPropertyShouldThrow()
        {
            Action test = () => this.odataError.InstanceAnnotations = null;
            Assert.Throws<ArgumentNullException>("value", test);
        }

        [Fact]
        public void SetListValueToInstanceAnnotationsPropertyShouldPass()
        {
            ICollection<ODataInstanceAnnotation> initialCollection = this.odataError.InstanceAnnotations;
            ICollection<ODataInstanceAnnotation> newCollection = new List<ODataInstanceAnnotation>();
            this.odataError.InstanceAnnotations = newCollection;
            Assert.Same(this.odataError.InstanceAnnotations, newCollection);
            Assert.NotSame(this.odataError.InstanceAnnotations, initialCollection);
        }

        [Fact]
        public void GetInstanceAnnotationsForWritingShouldReturnEmptyInstanceAnnotationsFromNewODataError()
        {
            Assert.NotNull(this.odataError.InstanceAnnotations);
            Assert.Empty(this.odataError.InstanceAnnotations);
        }

        [Fact]
        public void GetInstanceAnnotationsForWritingShouldReturnInstanceAnnotationsFromODataErrorWithInstanceAnnotations()
        {
            ODataInstanceAnnotation instanceAnnotation = new ODataInstanceAnnotation("namespace.name", new ODataPrimitiveValue("value"));
            this.odataError.InstanceAnnotations.Add(instanceAnnotation);
            var annotation = Assert.Single(this.odataError.InstanceAnnotations);
            Assert.Same(annotation, instanceAnnotation);
        }
    }
}

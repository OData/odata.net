//---------------------------------------------------------------------
// <copyright file="ODataErrorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.ObjecetModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ODataErrorTests
    {
        private static readonly ICollection<ODataInstanceAnnotation> instanceAnnotations = new Collection<ODataInstanceAnnotation>();
        private ODataError odataError;

        [TestInitialize]
        public void InitTest()
        {
            this.odataError = new ODataError();
        }

        [TestMethod]
        public void InstanceAnnotationsPropertyShouldNotBeNullAtCreation()
        {
            this.odataError.InstanceAnnotations.Should().NotBeNull();
        }

        [TestMethod]
        public void InstanceAnnotationsPropertyShouldReturnAWritableCollectionAtCreation()
        {
            this.odataError.InstanceAnnotations.Should().NotBeNull();
            this.odataError.InstanceAnnotations.Add(new ODataInstanceAnnotation("namespace.name", new ODataPrimitiveValue("value")));
            this.odataError.InstanceAnnotations.Count.Should().Be(1);
        }

        [TestMethod]
        public void SetNullValueToInstanceAnnotationsPropertyShouldThrow()
        {
            Action test = () => this.odataError.InstanceAnnotations = null;
            test.ShouldThrow<ArgumentException>().WithMessage("Value cannot be null.\r\nParameter name: value");
        }

        [TestMethod]
        public void SetListValueToInstanceAnnotationsPropertyShouldPass()
        {
            ICollection<ODataInstanceAnnotation> initialCollection = this.odataError.InstanceAnnotations;
            ICollection<ODataInstanceAnnotation> newCollection = new List<ODataInstanceAnnotation>();
            this.odataError.InstanceAnnotations = newCollection;
            this.odataError.InstanceAnnotations.As<object>().Should().BeSameAs(newCollection).And.NotBeSameAs(initialCollection);
        }

        [TestMethod]
        public void GetInstanceAnnotationsForWritingShouldReturnEmptyInstanceAnnotationsFromNewODataError()
        {
            this.odataError.InstanceAnnotations.Should().NotBeNull().And.BeEmpty();
        }

        [TestMethod]
        public void GetInstanceAnnotationsForWritingShouldReturnInstanceAnnotationsFromODataErrorWithInstanceAnnotations()
        {
            ODataInstanceAnnotation instanceAnnotation = new ODataInstanceAnnotation("namespace.name", new ODataPrimitiveValue("value"));
            this.odataError.InstanceAnnotations.Add(instanceAnnotation);
            this.odataError.InstanceAnnotations.Should().Contain(instanceAnnotation).And.HaveCount(1);
        }

    }
}

//---------------------------------------------------------------------
// <copyright file="ODataAnnotatableTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.ObjecetModel
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ODataAnnotatableTests
    {
        private ODataAnnotatable annotatable;

        [TestInitialize]
        public void TestInit()
        {
            this.annotatable = new ODataFeed();
        }

        [TestMethod]
        public void GetInstanceAnnotationsShouldReturnNonNullCollectionAtConstruction()
        {
            this.annotatable.GetInstanceAnnotations().Should().BeEmpty().And.Should().NotBeNull();
        }

        [TestMethod]
        public void SetInstanceAnnotationsShouldThrowOnNull()
        {
            Action test = () => this.annotatable.SetInstanceAnnotations(null);
            test.ShouldThrow<ArgumentNullException>("Value cannot be null.\r\nParameter name: instanceAnnotations");
        }

        [TestMethod]
        public void SetInstanceAnnotationsShouldUpdateValue()
        {
            var instanceAnnotations = new List<ODataInstanceAnnotation>();
            this.annotatable.SetInstanceAnnotations(instanceAnnotations);
            this.annotatable.GetInstanceAnnotations().As<object>().Should().BeSameAs(instanceAnnotations);
        }
    }
}

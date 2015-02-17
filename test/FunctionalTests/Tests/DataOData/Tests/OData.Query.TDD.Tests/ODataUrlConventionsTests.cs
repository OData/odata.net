//---------------------------------------------------------------------
// <copyright file="ODataUrlConventionsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests
{
    using System;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ODataUrlConventionsTests
    {
        [TestMethod]
        public void DefaultInstanceIsSingleton()
        {
            ODataUrlConventions.Default.Should().BeSameAs(ODataUrlConventions.Default);
        }

        [TestMethod]
        public void KeyAsSegmentInstanceIsSingleton()
        {
            ODataUrlConventions.KeyAsSegment.Should().BeSameAs(ODataUrlConventions.KeyAsSegment);
        }

        [TestMethod]
        public void DefaultInstanceShouldHaveCorrectInternalRepresentation()
        {
            ODataUrlConventions.Default.UrlConvention.GenerateKeyAsSegment.Should().BeFalse();
        }

        [TestMethod]
        public void KeyAsSegmentInstanceShouldHaveCorrectInternalRepresentation()
        {
            ODataUrlConventions.KeyAsSegment.UrlConvention.GenerateKeyAsSegment.Should().BeTrue();
        }
    }
}

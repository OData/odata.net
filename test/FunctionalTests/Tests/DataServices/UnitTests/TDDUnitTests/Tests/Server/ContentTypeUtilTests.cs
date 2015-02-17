//---------------------------------------------------------------------
// <copyright file="ContentTypeUtilTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server
{
    using System;
    using Microsoft.OData.Service;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// TODO: add tests for the rest of ContentTypeUtils' methods
    /// </summary>
    [TestClass]
    public class ContentTypeUtilTests
    {
        private Version V4 = new Version(4, 0);

        [TestMethod]
        public void UnqualifiedJsonShouldBeConsideredJsonLightIfMaxVersionIs3()
        {
            ContentTypeUtil.IsResponseMediaTypeJsonLight("application/json", true, V4).Should().BeTrue();
        }

        [TestMethod]
        public void UnqualifiedJsonShouldBeConsideredJsonLightIfMaxVersionIsGreaterThan3()
        {
            ContentTypeUtil.IsResponseMediaTypeJsonLight("application/json", true, new Version(5, 0)).Should().BeTrue();
        }

        [TestMethod]
        public void UnqualifiedJsonShouldBeConsideredJsonLightForNonEntityPayloadsIfMaxVersionIs3()
        {
            ContentTypeUtil.IsResponseMediaTypeJsonLight("application/json", false, V4).Should().BeTrue();
        }

        [TestMethod]
        public void JsonWithFullMetadataShouldBeConsideredJsonLight()
        {
            ContentTypeUtil.IsResponseMediaTypeJsonLight("application/json;odata.metadata=full", true, V4).Should().BeTrue();
        }

        [TestMethod]
        public void JsonWithMinimalMetadataShouldBeConsideredJsonLight()
        {
            ContentTypeUtil.IsResponseMediaTypeJsonLight("application/json;odata.metadata=minimal", true, V4).Should().BeTrue();
        }

        [TestMethod]
        public void JsonWithNoMetadataShouldBeConsideredJsonLight()
        {
            ContentTypeUtil.IsResponseMediaTypeJsonLight("application/json;odata.metadata=none", true, V4).Should().BeTrue();
        }

        [TestMethod]
        public void AtomShouldNotBeConsideredJsonLight()
        {
            ContentTypeUtil.IsResponseMediaTypeJsonLight("application/atom+xml", true, V4).Should().BeFalse();
        }

        [TestMethod]
        public void CheckForBeingJsonLightShouldBeCaseInsensitive()
        {
            ContentTypeUtil.IsResponseMediaTypeJsonLight("AppLICation/JSoN;ODatA.MeTaDAtA=FulL", true, V4).Should().BeTrue();
        }

        [TestMethod]
        public void UnspecifiedJsonShouldBeConsideredJson()
        {
            ContentTypeUtil.IsNotJson("application/json").Should().BeFalse();
        }

        [TestMethod]
        public void JsonWithMinimalMetadataShouldBeConsideredJson()
        {
            ContentTypeUtil.IsNotJson("application/json").Should().BeFalse();
        }

        [TestMethod]
        public void JsonWithFullMetadataShouldBeConsideredJson()
        {
            ContentTypeUtil.IsNotJson("application/json").Should().BeFalse();
        }

        [TestMethod]
        public void JsonWithNoMetadataShouldBeConsideredJson()
        {
            ContentTypeUtil.IsNotJson("application/json").Should().BeFalse();
        }

        [TestMethod]
        public void AtomShouldNotBeConsideredJson()
        {
            ContentTypeUtil.IsNotJson("application/atom+xml").Should().BeTrue();
        }

        [TestMethod]
        public void XmlShouldNotBeConsideredJson()
        {
            ContentTypeUtil.IsNotJson("application/xml").Should().BeTrue();
        }

        [TestMethod]
        public void CheckForBeingJsonShouldBeCaseInsensitive()
        {
            ContentTypeUtil.IsNotJson("aPPLiCATioN/jSOn").Should().BeFalse();
            ContentTypeUtil.IsNotJson("aPPLiCATioN/ATom+XMl").Should().BeTrue();
        }

        [TestMethod]
        public void NullContentTypeShouldNotBeConsideredJson()
        {
            ContentTypeUtil.IsNotJson(null).Should().BeTrue();
        }
    }
}
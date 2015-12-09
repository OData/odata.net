//---------------------------------------------------------------------
// <copyright file="ClientUrlConventionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Client
{
    using Microsoft.OData.Client;
    using Microsoft.OData.Core.Tests;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ClientUrlConventionTests
    {
        private readonly UrlConvention defaultConvention = UrlConvention.CreateWithExplicitValue(false);
        private readonly UrlConvention keyAsSegmentConvention = UrlConvention.CreateWithExplicitValue(true);
        
        [TestMethod]
        public void DefaultConventionShouldNotAddAnyHeaders()
        {
            var headers = new HeaderCollection();
            this.defaultConvention.AddRequiredHeaders(headers);
            headers.UnderlyingDictionary.Should().BeEmpty();
        }

        [TestMethod]
        public void KeyAsSegmentConventionShouldAddHeader()
        {
            var headers = new HeaderCollection();
            this.keyAsSegmentConvention.AddRequiredHeaders(headers);
            headers.UnderlyingDictionary.Should().ContainKey(UrlConventionsConstants.UrlConventionHeaderName, "KeyAsSegment");
        }
    }
}
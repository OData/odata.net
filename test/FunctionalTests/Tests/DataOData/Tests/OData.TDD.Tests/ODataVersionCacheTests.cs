//---------------------------------------------------------------------
// <copyright file="ODataVersionCacheTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Test.OData.TDD.Tests
{
    using FluentAssertions;
    using Microsoft.OData.Core;

    [TestClass]
    public class ODataVersionCacheTests
    {
        private readonly ODataVersionCache<ODataVersion> cache = new ODataVersionCache<ODataVersion>(version => version);

        [TestMethod]
        public void CacheV3ShouldBeODataVersionV3()
        {
            this.cache[ODataVersion.V4].Should().Be(ODataVersion.V4);
        }
    }
}

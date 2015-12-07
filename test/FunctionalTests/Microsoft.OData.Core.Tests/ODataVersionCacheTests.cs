//---------------------------------------------------------------------
// <copyright file="ODataVersionCacheTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Xunit;

namespace Microsoft.OData.Core.Tests
{
    public class ODataVersionCacheTests
    {
        private readonly ODataVersionCache<ODataVersion> cache = new ODataVersionCache<ODataVersion>(version => version);

        [Fact]
        public void CacheV3ShouldBeODataVersionV3()
        {
            this.cache[ODataVersion.V4].Should().Be(ODataVersion.V4);
        }
    }
}

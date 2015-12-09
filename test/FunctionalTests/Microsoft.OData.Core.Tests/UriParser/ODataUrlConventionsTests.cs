//---------------------------------------------------------------------
// <copyright file="ODataUrlConventionsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Microsoft.OData.Core.UriParser;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser
{
    public class ODataUrlConventionsTests
    {
        [Fact]
        public void DefaultInstanceIsSingleton()
        {
            ODataUrlConventions.Default.Should().BeSameAs(ODataUrlConventions.Default);
        }

        [Fact]
        public void KeyAsSegmentInstanceIsSingleton()
        {
            ODataUrlConventions.KeyAsSegment.Should().BeSameAs(ODataUrlConventions.KeyAsSegment);
        }

        [Fact]
        public void DefaultInstanceShouldHaveCorrectInternalRepresentation()
        {
            ODataUrlConventions.Default.UrlConvention.GenerateKeyAsSegment.Should().BeFalse();
        }

        [Fact]
        public void KeyAsSegmentInstanceShouldHaveCorrectInternalRepresentation()
        {
            ODataUrlConventions.KeyAsSegment.UrlConvention.GenerateKeyAsSegment.Should().BeTrue();
        }
    }
}

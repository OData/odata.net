//---------------------------------------------------------------------
// <copyright file="ODataUrlKeyDelimiterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Xunit;

namespace Microsoft.OData.Tests.UriParser
{
    public class ODataUrlKeyDelimiterTests
    {
        [Fact]
        public void DefaultInstanceIsSingleton()
        {
            ODataUrlKeyDelimiter.Parentheses.Should().BeSameAs(ODataUrlKeyDelimiter.Parentheses);
        }

        [Fact]
        public void KeyAsSegmentInstanceIsSingleton()
        {
            ODataUrlKeyDelimiter.Slash.Should().BeSameAs(ODataUrlKeyDelimiter.Slash);
        }

        [Fact]
        public void DefaultInstanceShouldHaveCorrectInternalRepresentation()
        {
            ODataUrlKeyDelimiter.Parentheses.EnableKeyAsSegment.Should().BeFalse();
        }

        [Fact]
        public void KeyAsSegmentInstanceShouldHaveCorrectInternalRepresentation()
        {
            ODataUrlKeyDelimiter.Slash.EnableKeyAsSegment.Should().BeTrue();
        }
    }
}

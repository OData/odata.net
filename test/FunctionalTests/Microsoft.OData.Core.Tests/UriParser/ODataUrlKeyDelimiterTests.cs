//---------------------------------------------------------------------
// <copyright file="ODataUrlKeyDelimiterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Xunit;

namespace Microsoft.OData.Tests.UriParser
{
    public class ODataUrlKeyDelimiterTests
    {
        [Fact]
        public void DefaultInstanceIsSingleton()
        {
            Assert.Same(ODataUrlKeyDelimiter.Parentheses, ODataUrlKeyDelimiter.Parentheses);
        }

        [Fact]
        public void KeyAsSegmentInstanceIsSingleton()
        {
            Assert.Same(ODataUrlKeyDelimiter.Slash, ODataUrlKeyDelimiter.Slash);
        }

        [Fact]
        public void DefaultInstanceShouldHaveCorrectInternalRepresentation()
        {
            Assert.False(ODataUrlKeyDelimiter.Parentheses.EnableKeyAsSegment);
        }

        [Fact]
        public void KeyAsSegmentInstanceShouldHaveCorrectInternalRepresentation()
        {
            Assert.True(ODataUrlKeyDelimiter.Slash.EnableKeyAsSegment);
        }
    }
}

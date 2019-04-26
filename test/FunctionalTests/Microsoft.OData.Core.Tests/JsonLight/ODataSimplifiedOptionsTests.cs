//---------------------------------------------------------------------
// <copyright file="ODataSimplifiedOptionsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Xunit;

namespace Microsoft.OData.Tests.JsonLight
{
    public class ODataSimplifiedOptionsTests
    {
        [Fact]
        public void ValidateDefaultOptions()
        {
            var options = new ODataSimplifiedOptions();
            options.EnableWritingKeyAsSegment.Should().BeFalse();
            options.EnableParsingKeyAsSegmentUrl.Should().BeTrue();
            options.EnableReadingODataAnnotationWithoutPrefix.Should().BeFalse();
            options.EnableWritingODataAnnotationWithoutPrefix.Should().BeFalse();
            options.EnableReadingKeyAsSegment.Should().BeFalse();
            options.GetOmitODataPrefix().Should().BeFalse();
            options.GetOmitODataPrefix(ODataVersion.V4).Should().BeFalse();
            options.GetOmitODataPrefix(ODataVersion.V401).Should().BeTrue();
        }

        [Fact]
        public void Validate40Options()
        {
            var options = new ODataSimplifiedOptions(ODataVersion.V4);
            options.EnableWritingKeyAsSegment.Should().BeFalse();
            options.EnableParsingKeyAsSegmentUrl.Should().BeTrue();
            options.EnableReadingODataAnnotationWithoutPrefix.Should().BeFalse();
            options.EnableWritingODataAnnotationWithoutPrefix.Should().BeFalse();
            options.EnableReadingKeyAsSegment.Should().BeFalse();
            options.GetOmitODataPrefix().Should().BeFalse();
            options.GetOmitODataPrefix(ODataVersion.V4).Should().BeFalse();
            options.GetOmitODataPrefix(ODataVersion.V401).Should().BeTrue();
        }

        [Fact]
        public void Validate401Options()
        {
            var options = new ODataSimplifiedOptions(ODataVersion.V401);
            options.EnableWritingKeyAsSegment.Should().BeFalse();
            options.EnableReadingKeyAsSegment.Should().BeFalse();
            options.EnableParsingKeyAsSegmentUrl.Should().BeTrue();
            options.EnableReadingODataAnnotationWithoutPrefix.Should().BeTrue();
            options.EnableWritingODataAnnotationWithoutPrefix.Should().BeTrue();
            options.GetOmitODataPrefix().Should().BeTrue();
            options.GetOmitODataPrefix(ODataVersion.V4).Should().BeFalse();
            options.GetOmitODataPrefix(ODataVersion.V401).Should().BeTrue();
        }

        [Theory]
        [InlineData(/*SimplifiedOptionsVersion*/ ODataVersion.V4)]
        [InlineData(/*SimplifiedOptionsVersion*/ ODataVersion.V401)]
        public void SetDefaultODataPrefixWritingTrue(ODataVersion version)
        {
            var options = new ODataSimplifiedOptions(version);
            options.SetOmitODataPrefix(true);
            options.GetOmitODataPrefix().Should().BeTrue();
            options.GetOmitODataPrefix(ODataVersion.V4).Should().BeTrue();
            options.GetOmitODataPrefix(ODataVersion.V401).Should().BeTrue();
            options.EnableWritingODataAnnotationWithoutPrefix.Should().BeTrue();
        }

        [Theory]
        [InlineData(/*SimplifiedOptionsVersion*/ ODataVersion.V4)]
        [InlineData(/*SimplifiedOptionsVersion*/ ODataVersion.V401)]
        public void SetDefaultODataPrefixWritingFalse(ODataVersion version)
        {
            var options = new ODataSimplifiedOptions(version);
            options.SetOmitODataPrefix(false);
            options.GetOmitODataPrefix().Should().BeFalse();
            options.GetOmitODataPrefix(ODataVersion.V4).Should().BeFalse();
            options.GetOmitODataPrefix(ODataVersion.V401).Should().BeFalse();
            options.EnableWritingODataAnnotationWithoutPrefix.Should().BeFalse();
        }

        [Theory]
        [InlineData(/*SimplifiedOptionsVersion*/ ODataVersion.V4)]
        [InlineData(/*SimplifiedOptionsVersion*/ ODataVersion.V401)]
        public void SetV4ODataPrefixWritingTrue(ODataVersion version)
        {
            var options = new ODataSimplifiedOptions(version);
            options.SetOmitODataPrefix(true, ODataVersion.V4);
            options.GetOmitODataPrefix().Should().Be(version != ODataVersion.V4);
            options.GetOmitODataPrefix(ODataVersion.V4).Should().BeTrue();
            options.GetOmitODataPrefix(ODataVersion.V401).Should().BeTrue();
            options.EnableWritingODataAnnotationWithoutPrefix.Should().Be(version != ODataVersion.V4);
        }

        [Theory]
        [InlineData(/*SimplifiedOptionsVersion*/ ODataVersion.V4)]
        [InlineData(/*SimplifiedOptionsVersion*/ ODataVersion.V401)]
        public void SetV401ODataPrefixWritingTrue(ODataVersion version)
        {
            var options = new ODataSimplifiedOptions(version);
            options.SetOmitODataPrefix(true, ODataVersion.V401);
            options.GetOmitODataPrefix().Should().Be(version != ODataVersion.V4);
            options.GetOmitODataPrefix(ODataVersion.V4).Should().BeFalse();
            options.GetOmitODataPrefix(ODataVersion.V401).Should().BeTrue();
            options.EnableWritingODataAnnotationWithoutPrefix.Should().Be(version != ODataVersion.V4);
        }

        [Theory]
        [InlineData(/*SimplifiedOptionsVersion*/ ODataVersion.V4)]
        [InlineData(/*SimplifiedOptionsVersion*/ ODataVersion.V401)]
        public void SetV4ODataPrefixWritingFalse(ODataVersion version)
        {
            var options = new ODataSimplifiedOptions(version);
            options.SetOmitODataPrefix(false, ODataVersion.V4);
            options.GetOmitODataPrefix().Should().Be(version != ODataVersion.V4);
            options.GetOmitODataPrefix(ODataVersion.V4).Should().BeFalse();
            options.GetOmitODataPrefix(ODataVersion.V401).Should().BeTrue();
            options.EnableWritingODataAnnotationWithoutPrefix.Should().Be(version != ODataVersion.V4);
        }

        [Theory]
        [InlineData(/*SimplifiedOptionsVersion*/ ODataVersion.V4)]
        [InlineData(/*SimplifiedOptionsVersion*/ ODataVersion.V401)]

        public void SetV401ODataPrefixWritingFalse(ODataVersion version)
        {
            var options = new ODataSimplifiedOptions(version);
            options.SetOmitODataPrefix(false, ODataVersion.V401);
            options.GetOmitODataPrefix().Should().Be(version != ODataVersion.V4);
            options.GetOmitODataPrefix(ODataVersion.V4).Should().BeFalse();
            options.GetOmitODataPrefix(ODataVersion.V401).Should().BeFalse();
            options.EnableWritingODataAnnotationWithoutPrefix.Should().Be(version != ODataVersion.V4);
        }

        [Theory]
        [InlineData(/*SimplifiedOptionsVersion*/ ODataVersion.V4)]
        [InlineData(/*SimplifiedOptionsVersion*/ ODataVersion.V401)]

        public void SetEnableWritingODataAnnotationWithoutPrefixTrue(ODataVersion version)
        {
            var options = new ODataSimplifiedOptions(version);
            options.EnableWritingODataAnnotationWithoutPrefix = true;
            options.GetOmitODataPrefix().Should().BeTrue();
            options.GetOmitODataPrefix(ODataVersion.V4).Should().BeTrue();
            options.GetOmitODataPrefix(ODataVersion.V401).Should().BeTrue();
            options.EnableWritingODataAnnotationWithoutPrefix.Should().BeTrue();
        }

        [Fact]
        public void SetEnableWritingODataAnnotationWithoutPrefixFalse()
        {
            var options = new ODataSimplifiedOptions();
            options.EnableWritingODataAnnotationWithoutPrefix = false;
            options.GetOmitODataPrefix().Should().BeFalse();
            options.GetOmitODataPrefix(ODataVersion.V4).Should().BeFalse();
            options.GetOmitODataPrefix(ODataVersion.V401).Should().BeFalse();
            options.EnableWritingODataAnnotationWithoutPrefix.Should().BeFalse();
        }
    }
}

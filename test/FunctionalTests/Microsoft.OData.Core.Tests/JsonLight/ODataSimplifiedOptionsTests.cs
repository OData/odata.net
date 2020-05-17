//---------------------------------------------------------------------
// <copyright file="ODataSimplifiedOptionsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Xunit;

namespace Microsoft.OData.Tests.JsonLight
{
    public class ODataSimplifiedOptionsTests
    {
        [Fact]
        public void ValidateDefaultOptions()
        {
            var options = new ODataSimplifiedOptions();
            Assert.False(options.EnableWritingKeyAsSegment);
            Assert.True(options.EnableParsingKeyAsSegmentUrl);
            Assert.False(options.EnableReadingODataAnnotationWithoutPrefix);
            Assert.False(options.GetOmitODataPrefix());
            Assert.False(options.EnableReadingKeyAsSegment);
            Assert.False(options.GetOmitODataPrefix());
            Assert.False(options.GetOmitODataPrefix(ODataVersion.V4));
            Assert.True(options.GetOmitODataPrefix(ODataVersion.V401));
        }

        [Fact]
        public void Validate40Options()
        {
            var options = new ODataSimplifiedOptions(ODataVersion.V4);
            Assert.False(options.EnableWritingKeyAsSegment);
            Assert.True(options.EnableParsingKeyAsSegmentUrl);
            Assert.False(options.EnableReadingODataAnnotationWithoutPrefix);
            Assert.False(options.GetOmitODataPrefix());
            Assert.False(options.EnableReadingKeyAsSegment);
            Assert.False(options.GetOmitODataPrefix());
            Assert.False(options.GetOmitODataPrefix(ODataVersion.V4));
            Assert.True(options.GetOmitODataPrefix(ODataVersion.V401));
        }

        [Fact]
        public void Validate401Options()
        {
            var options = new ODataSimplifiedOptions(ODataVersion.V401);
            Assert.False(options.EnableWritingKeyAsSegment);
            Assert.False(options.EnableReadingKeyAsSegment);
            Assert.True(options.EnableParsingKeyAsSegmentUrl);
            Assert.True(options.EnableReadingODataAnnotationWithoutPrefix);
            Assert.True(options.GetOmitODataPrefix());
            Assert.True(options.GetOmitODataPrefix());
            Assert.False(options.GetOmitODataPrefix(ODataVersion.V4));
            Assert.True(options.GetOmitODataPrefix(ODataVersion.V401));
        }

        [Theory]
        [InlineData(/*SimplifiedOptionsVersion*/ ODataVersion.V4)]
        [InlineData(/*SimplifiedOptionsVersion*/ ODataVersion.V401)]
        [InlineData(/*SimplifiedOptionsVersion*/ null)]
        public void SetDefaultODataPrefixWritingTrue(ODataVersion? version)
        {
            var options = new ODataSimplifiedOptions(version);
            options.SetOmitODataPrefix(true);
            Assert.True(options.GetOmitODataPrefix());
            Assert.True(options.GetOmitODataPrefix(ODataVersion.V4));
            Assert.True(options.GetOmitODataPrefix(ODataVersion.V401));
            Assert.True(options.GetOmitODataPrefix());
        }

        [Theory]
        [InlineData(/*SimplifiedOptionsVersion*/ ODataVersion.V4)]
        [InlineData(/*SimplifiedOptionsVersion*/ ODataVersion.V401)]
        [InlineData(/*SimplifiedOptionsVersion*/ null)]
        public void SetDefaultODataPrefixWritingFalse(ODataVersion? version)
        {
            var options = new ODataSimplifiedOptions(version);
            options.SetOmitODataPrefix(false);
            Assert.False(options.GetOmitODataPrefix());
            Assert.False(options.GetOmitODataPrefix(ODataVersion.V4));
            Assert.False(options.GetOmitODataPrefix(ODataVersion.V401));
            Assert.False(options.GetOmitODataPrefix());
        }

        [Theory]
        [InlineData(/*SimplifiedOptionsVersion*/ ODataVersion.V4)]
        [InlineData(/*SimplifiedOptionsVersion*/ ODataVersion.V401)]
        [InlineData(/*SimplifiedOptionsVersion*/ null)]
        public void SetV4ODataPrefixWritingTrue(ODataVersion? version)
        {
            var options = new ODataSimplifiedOptions(version);
            options.SetOmitODataPrefix(true, ODataVersion.V4);
            Assert.Equal(version != null && version != ODataVersion.V4, options.GetOmitODataPrefix());
            Assert.True(options.GetOmitODataPrefix(ODataVersion.V4));
            Assert.True(options.GetOmitODataPrefix(ODataVersion.V401));
            Assert.Equal(version != null && version != ODataVersion.V4, options.GetOmitODataPrefix());
        }

        [Theory]
        [InlineData(/*SimplifiedOptionsVersion*/ ODataVersion.V4)]
        [InlineData(/*SimplifiedOptionsVersion*/ ODataVersion.V401)]
        [InlineData(/*SimplifiedOptionsVersion*/ null)]
        public void SetV401ODataPrefixWritingTrue(ODataVersion? version)
        {
            var options = new ODataSimplifiedOptions(version);
            options.SetOmitODataPrefix(true, ODataVersion.V401);
            Assert.Equal(version != null && version != ODataVersion.V4, options.GetOmitODataPrefix());
            Assert.False(options.GetOmitODataPrefix(ODataVersion.V4));
            Assert.True(options.GetOmitODataPrefix(ODataVersion.V401));
            Assert.Equal(version != null && version != ODataVersion.V4, options.GetOmitODataPrefix());
        }

        [Theory]
        [InlineData(/*SimplifiedOptionsVersion*/ ODataVersion.V4)]
        [InlineData(/*SimplifiedOptionsVersion*/ ODataVersion.V401)]
        [InlineData(/*SimplifiedOptionsVersion*/ null)]
        public void SetV4ODataPrefixWritingFalse(ODataVersion? version)
        {
            var options = new ODataSimplifiedOptions(version);
            options.SetOmitODataPrefix(false, ODataVersion.V4);
            Assert.Equal(version != null && version != ODataVersion.V4, options.GetOmitODataPrefix());
            Assert.False(options.GetOmitODataPrefix(ODataVersion.V4));
            Assert.True(options.GetOmitODataPrefix(ODataVersion.V401));
            Assert.Equal(version != null && version != ODataVersion.V4, options.GetOmitODataPrefix());
        }

        [Theory]
        [InlineData(/*SimplifiedOptionsVersion*/ ODataVersion.V4)]
        [InlineData(/*SimplifiedOptionsVersion*/ ODataVersion.V401)]
        [InlineData(/*SimplifiedOptionsVersion*/ null)]

        public void SetV401ODataPrefixWritingFalse(ODataVersion? version)
        {
            var options = new ODataSimplifiedOptions(version);
            options.SetOmitODataPrefix(false, ODataVersion.V401);
            Assert.Equal(version != null && version != ODataVersion.V4, options.GetOmitODataPrefix());
            Assert.False(options.GetOmitODataPrefix(ODataVersion.V4));
            Assert.False(options.GetOmitODataPrefix(ODataVersion.V401));
            Assert.Equal(version != null && version != ODataVersion.V4, options.GetOmitODataPrefix());
        }

        [Theory]
        [InlineData(/*SimplifiedOptionsVersion*/ ODataVersion.V4)]
        [InlineData(/*SimplifiedOptionsVersion*/ ODataVersion.V401)]
        [InlineData(/*SimplifiedOptionsVersion*/ null)]

        public void SetEnableWritingODataAnnotationWithoutPrefixTrue(ODataVersion? version)
        {
            var options = new ODataSimplifiedOptions(version);
            options.SetOmitODataPrefix(true);
            Assert.True(options.GetOmitODataPrefix());
            Assert.True(options.GetOmitODataPrefix(ODataVersion.V4));
            Assert.True(options.GetOmitODataPrefix(ODataVersion.V401));
            Assert.True(options.GetOmitODataPrefix());
        }

        [Theory]
        [InlineData(/*SimplifiedOptionsVersion*/ ODataVersion.V4)]
        [InlineData(/*SimplifiedOptionsVersion*/ ODataVersion.V401)]
        [InlineData(/*SimplifiedOptionsVersion*/ null)]
        public void SetEnableWritingODataAnnotationWithoutPrefixFalse(ODataVersion? version)
        {
            var options = new ODataSimplifiedOptions(version);
            options.SetOmitODataPrefix(false);
            Assert.False(options.GetOmitODataPrefix());
            Assert.False(options.GetOmitODataPrefix(ODataVersion.V4));
            Assert.False(options.GetOmitODataPrefix(ODataVersion.V401));
            Assert.False(options.GetOmitODataPrefix());
        }

        [Fact]
        public void SetDefaultEnableWritingODataAnnotationWithoutPrefixFalse()
        {
            var options = new ODataSimplifiedOptions();
            options.SetOmitODataPrefix(false);
            Assert.False(options.GetOmitODataPrefix());
            Assert.False(options.GetOmitODataPrefix(ODataVersion.V4));
            Assert.False(options.GetOmitODataPrefix(ODataVersion.V401));
            Assert.False(options.GetOmitODataPrefix());
        }

        [Fact]
        public void SetDefaultEnableWritingODataAnnotationWithoutPrefixTrue()
        {
            var options = new ODataSimplifiedOptions();
            options.SetOmitODataPrefix(true);
            Assert.True(options.GetOmitODataPrefix());
            Assert.True(options.GetOmitODataPrefix(ODataVersion.V4));
            Assert.True(options.GetOmitODataPrefix(ODataVersion.V401));
            Assert.True(options.GetOmitODataPrefix());
        }
    }
}

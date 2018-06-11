//---------------------------------------------------------------------
// <copyright file="BatchReferenceSegmentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------


namespace Microsoft.OData.Tests
{
    using FluentAssertions;

    using Microsoft.OData.Buffers;

    using Xunit;

    public class BufferUtilsTests
    {
        [Fact]
        public void BufferUtilsShouldCreateANewBufferIfPassedInBufferIsNull()
        {
            char[] buffer = null;
            BufferUtils.InitializeBufferIfRequired(buffer).Should()
                .NotBeNull("If null buffer is passed then a new buffer should be created");
        }

        [Fact]
        public void BufferUtilsShouldCreateNonZeroLengthBuffer()
        {
            char[] buffer = null;
            buffer = BufferUtils.InitializeBufferIfRequired(buffer);
            buffer.Length.Should().BeGreaterThan(0, "Created Buffer should be greater than zero");
        }

        [Fact]
        public void BufferUtilsShouldNotCreateANewBufferIfPassedInBuferIsNotNull()
        {
            char[] buffer = new char[10];
            buffer = BufferUtils.InitializeBufferIfRequired(buffer);
            buffer.ShouldBeEquivalentTo(buffer);
        }
    }
}

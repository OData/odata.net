//---------------------------------------------------------------------
// <copyright file="BufferUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Tests
{
    using System;
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

        [Fact]
        public void RentFromBufferShouldThrowsIfNullBufferReturns()
        {
            // Arrange
            Action test = () => BufferUtils.RentFromBuffer(new BadCharArrayPool(), 1024);

            // Act & Assert
            var exception = Assert.Throws<ODataException>(test);
            Assert.Equal(Strings.BufferUtils_InvalidBufferOrSize(1024), exception.Message);
        }

        public class BadCharArrayPool : ICharArrayPool
        {
            public char[] Rent(int minSize)
            {
                return null;
            }

            public void Return(char[] array)
            {
            }
        }

        [Fact]
        public void RentFromBufferShouldThrowsIfStringyBufferReturns()
        {
            // Arrange
            Action test = () => BufferUtils.RentFromBuffer(new StingyCharArrayPool(), 1024);

            // Act & Assert
            var exception = Assert.Throws<ODataException>(test);
            Assert.Equal(Strings.BufferUtils_InvalidBufferOrSize(1024), exception.Message);
        }

        public class StingyCharArrayPool : ICharArrayPool
        {
            public char[] Rent(int minSize)
            {
                return new char[minSize - 1];
            }

            public void Return(char[] array)
            {
            }
        }
    }
}

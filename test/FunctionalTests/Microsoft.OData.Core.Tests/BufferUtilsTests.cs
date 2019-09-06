//---------------------------------------------------------------------
// <copyright file="BufferUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Buffers;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class BufferUtilsTests
    {
        [Fact]
        public void BufferUtilsShouldCreateANewBufferIfPassedInBufferIsNull()
        {
            char[] buffer = null;
            Assert.NotNull(BufferUtils.InitializeBufferIfRequired(buffer));
        }

        [Fact]
        public void BufferUtilsShouldCreateNonZeroLengthBuffer()
        {
            char[] buffer = null;
            buffer = BufferUtils.InitializeBufferIfRequired(buffer);
            Assert.True(buffer.Length > 0);
        }

        [Fact]
        public void BufferUtilsShouldNotCreateANewBufferIfPassedInBuferIsNotNull()
        {
            char[] buffer = new char[10];
            var newBuffer = BufferUtils.InitializeBufferIfRequired(buffer);
            Assert.Same(newBuffer, buffer);
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

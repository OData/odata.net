//---------------------------------------------------------------------
// <copyright file="ODataBatchUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class ODataBatchUtilsTests
    {
        [Theory]
        [InlineData(0, 0, 0, 0)]
        [InlineData(0, 0, 1, 1)]
        [InlineData(8, 4, 0, 8)]
        [InlineData(8, 4, 4, 8)]
        [InlineData(8, 4, 5, 16)]
        [InlineData(8, 8, 1, 16)]
        [InlineData(8, 8, 40, 48)]
        public void EnsureArraySizePreservesDataAndGrowsGeometrically(int capacity, int used, int additional, int expectedCapacity)
        {
            byte[] buffer = new byte[capacity];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)(i + 1);
            }

            byte[] originalBuffer = buffer;
            ODataBatchUtils.EnsureArraySize(ref buffer, used, additional);

            Assert.Equal(expectedCapacity, buffer.Length);
            if (expectedCapacity == capacity)
            {
                Assert.Same(originalBuffer, buffer);
            }
            else
            {
                Assert.NotSame(originalBuffer, buffer);
                for (int i = used; i < buffer.Length; i++)
                {
                    Assert.Equal((byte)0, buffer[i]);
                }
            }

            for (int i = 0; i < used; i++)
            {
                Assert.Equal(originalBuffer[i], buffer[i]);
            }
        }

        [Fact]
        public void EnsureArraySizeRepeatedAppendsAllocateLinearTotalCapacity()
        {
            const int chunkSize = ODataBatchReaderStreamBuffer.BufferLength;
            const int totalSize = 128 * chunkSize;
            byte[] buffer = new byte[chunkSize];
            long totalAllocatedCapacity = buffer.Length;

            for (int used = 0; used < totalSize; used += chunkSize)
            {
                byte[] originalBuffer = buffer;
                ODataBatchUtils.EnsureArraySize(ref buffer, used, chunkSize);
                if (!ReferenceEquals(originalBuffer, buffer))
                {
                    totalAllocatedCapacity += buffer.Length;
                }

                buffer[used] = (byte)(used / chunkSize);
            }

            Assert.InRange(totalAllocatedCapacity, totalSize, 4L * totalSize);
            for (int used = 0; used < totalSize; used += chunkSize)
            {
                Assert.Equal((byte)(used / chunkSize), buffer[used]);
            }
        }

        [Theory]
        [InlineData(int.MaxValue)]
        [InlineData(0X7FFFFFC7)]
        public void EnsureArraySizeRejectsUnrepresentableCapacityWithoutChangingBuffer(int additional)
        {
            byte[] buffer = new byte[] { 42 };
            byte[] originalBuffer = buffer;

            Assert.Throws<OutOfMemoryException>(() => ODataBatchUtils.EnsureArraySize(ref buffer, 1, additional));

            Assert.Same(originalBuffer, buffer);
            Assert.Equal((byte)42, buffer[0]);
        }
    }
}

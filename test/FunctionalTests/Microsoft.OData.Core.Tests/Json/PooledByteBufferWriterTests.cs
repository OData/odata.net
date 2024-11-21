//---------------------------------------------------------------------
// <copyright file="PooledByteBufferWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Json;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    public class PooledByteBufferWriterTests
    {
        [Fact]
        public void Constructor_InitializesBufferToInititalCapacity()
        {
            using PooledByteBufferWriter bufferWriter = new PooledByteBufferWriter(512);
            Assert.Equal(0, bufferWriter.WrittenMemory.Span.Length);
            Assert.Equal(0, bufferWriter.WrittenCount);
            Assert.Equal(512, bufferWriter.Capacity);
            Assert.Equal(512, bufferWriter.FreeCapacity);
            Assert.Equal(0, bufferWriter.WrittenMemory.Length);
        }

        [Fact]
        public void GetSpan_WhenBufferIsEmpty_ReturnsSpanWithDefaultCapacity()
        {
            using PooledByteBufferWriter bufferWriter = new PooledByteBufferWriter(512);
            Span<byte> span = bufferWriter.GetSpan();
            Assert.Equal(512, span.Length);
        }

        [Fact]
        public void GetMemory_WhenBufferIsEmpty_ReturnsMemoryWithDefaultCapacity()
        {
            using PooledByteBufferWriter bufferWriter = new PooledByteBufferWriter(512);
            Memory<byte> memory = bufferWriter.GetMemory();
            Assert.Equal(512, memory.Length);
        }

        [Fact]
        public void GetSpan_WhenSizeHintIsZero_ReturnsNonEmptyBuffer()
        {
            using PooledByteBufferWriter bufferWriter = new PooledByteBufferWriter(512);
            Span<byte> buffer = bufferWriter.GetSpan(0);
            Assert.Equal(512, buffer.Length);
        }

        [Fact]
        public void GetMemory_WhenSizeHintIsZero_ReturnsNonEmptyBuffer()
        {
            using PooledByteBufferWriter bufferWriter = new PooledByteBufferWriter(512);
            Memory<byte> buffer = bufferWriter.GetMemory(0);
            Assert.Equal(512, buffer.Length);
        }

        [Fact]
        public void Advance_UpdatesPositionOfTheBuffer()
        {
            using PooledByteBufferWriter bufferWriter = new PooledByteBufferWriter(512);
            Span<byte> buffer = bufferWriter.GetSpan(10);
            buffer.Slice(0, 10).Fill(1);

            bufferWriter.Advance(10);

            Assert.Equal(502, bufferWriter.FreeCapacity);
            Assert.Equal(10, bufferWriter.WrittenCount);
            Assert.Equal(512, bufferWriter.Capacity);

            bufferWriter.Advance(10);

            Assert.Equal(492, bufferWriter.FreeCapacity);
            Assert.Equal(20, bufferWriter.WrittenCount);
            Assert.Equal(512, bufferWriter.Capacity);
        }

        [Fact]
        public void GetSpan_ResizesBufferWhenCapacityExceeded()
        {
            using PooledByteBufferWriter bufferWriter = new PooledByteBufferWriter(256);

            Span<byte> buffer = bufferWriter.GetSpan(200);
            buffer.Slice(0, 200).Fill(1);

            bufferWriter.Advance(200);
            Assert.Equal(56, bufferWriter.FreeCapacity);
            Assert.Equal(200, bufferWriter.WrittenCount);
            Assert.Equal(256, bufferWriter.Capacity);

            buffer = bufferWriter.GetSpan(200);
            buffer.Slice(0, 200).Fill(1);
            bufferWriter.Advance(200);

            Assert.Equal(112, bufferWriter.FreeCapacity);
            Assert.Equal(400, bufferWriter.WrittenCount);
            Assert.Equal(512, bufferWriter.Capacity);
        }

        [Fact]
        public void GetMemory_ResizesBufferWhenCapacityExceeded()
        {
            using PooledByteBufferWriter bufferWriter = new PooledByteBufferWriter(256);

            Memory<byte> buffer = bufferWriter.GetMemory(200);
            buffer.Slice(0, 200).Span.Fill(1);

            bufferWriter.Advance(200);
            Assert.Equal(56, bufferWriter.FreeCapacity);
            Assert.Equal(200, bufferWriter.WrittenCount);
            Assert.Equal(256, bufferWriter.Capacity);

            buffer = bufferWriter.GetMemory(200);
            buffer.Slice(0, 200).Span.Fill(1);
            bufferWriter.Advance(200);

            Assert.Equal(112, bufferWriter.FreeCapacity);
            Assert.Equal(400, bufferWriter.WrittenCount);
            Assert.Equal(512, bufferWriter.Capacity);
        }

        [Fact]
        public void GetSpan_ThrowsExceptionIfExceedsMaxBufferSize()
        {
            int initialBufferSize = 256;
            using PooledByteBufferWriter bufferWriter = new PooledByteBufferWriter(initialBufferSize);

            Action action = () => bufferWriter.GetSpan(PooledByteBufferWriter.MaximumBufferSize);
            int newSize = initialBufferSize + PooledByteBufferWriter.MaximumBufferSize;
            action.Throws<OutOfMemoryException>(Strings.ODataMessageWriter_Buffer_Maximum_Size_Exceeded(newSize));
        }

        [Fact]
        public void GetMemory_ThrowsExceptionIfExceedsMaxBufferSize()
        {
            int initialBufferSize = 256;
            using PooledByteBufferWriter bufferWriter = new PooledByteBufferWriter(initialBufferSize);

            Action action = () => bufferWriter.GetMemory(PooledByteBufferWriter.MaximumBufferSize);
            int newSize = initialBufferSize + PooledByteBufferWriter.MaximumBufferSize;
            action.Throws<OutOfMemoryException>(Strings.ODataMessageWriter_Buffer_Maximum_Size_Exceeded(newSize));
        }

        [Fact]
        public void GetSpan_NewBufferDoNotOverwritePreviousOne()
        {
            using PooledByteBufferWriter bufferWriter = new PooledByteBufferWriter(256);
            Span<byte> buffer = bufferWriter.GetSpan(1);
            buffer[0] = 143;

            bufferWriter.Advance(1);

            buffer = bufferWriter.GetSpan(1);
            Assert.NotEqual(143, buffer[0]);
        }

        [Fact]
        public void GetMemory_NewBufferDoNotOverwritePreviousOne()
        {
            using PooledByteBufferWriter bufferWriter = new PooledByteBufferWriter(256);
            Memory<byte> buffer = bufferWriter.GetMemory(1);
            buffer.Span[0] = 143;

            bufferWriter.Advance(1);

            buffer = bufferWriter.GetMemory(1);
            Assert.NotEqual(143, buffer.Span[0]);
        }

        [Fact]
        public void Clear_ResetsTheBuffer()
        {
            using PooledByteBufferWriter bufferWriter = new PooledByteBufferWriter(256);
            Span<byte> buffer = bufferWriter.GetSpan(200);
            bufferWriter.Advance(200);

            Assert.Equal(200, bufferWriter.WrittenCount);
            Assert.Equal(56, bufferWriter.FreeCapacity);
            Assert.Equal(256, bufferWriter.Capacity);

            bufferWriter.Clear();

            Assert.Equal(0, bufferWriter.WrittenCount);
            Assert.Equal(256, bufferWriter.FreeCapacity);
            Assert.Equal(256, bufferWriter.Capacity);
            Assert.Equal(0, bufferWriter.WrittenMemory.Length);
        }

        [Fact]
        public void WrittenMemory_ReturnsDataWrittenSoFar()
        {
            using PooledByteBufferWriter bufferWriter = new PooledByteBufferWriter(256);
            Span<byte> buffer = bufferWriter.GetSpan(5);
            for (int i = 0; i < 5; i++)
            {
                buffer[i] = (byte)(i + 1);
            }

            bufferWriter.Advance(5);

            buffer = bufferWriter.GetSpan(5);
            for (int i = 0; i < 5; i++)
            {
                buffer[i] = (byte)(i + 6);
            }

            bufferWriter.Advance(5);

            ReadOnlySpan<byte> data = bufferWriter.WrittenMemory.Span;
            Assert.Equal(10, data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                Assert.Equal(i + 1, data[i]);
            }
        }
    }
}

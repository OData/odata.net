//---------------------------------------------------------------------
// <copyright file="BufferedReadStreamTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OData.Tests
{
    /// <summary>
    /// Tests for BufferedReadStream.
    /// </summary>
    public class BufferedReadStreamTests
    {
        private const int OneBufferSize = 64 * 1024; // Mirrors private BufferSize (64KB) in DataBuffer.

        [Fact]
        public async Task BufferStreamAsync_EmptyStream_ReturnsStreamThatImmediatelyEof()
        {
            var sourceStream = new DisposingMemoryStream(Array.Empty<byte>());
            var bufferedStream = await BufferedReadStream.BufferStreamAsync(sourceStream);

            Assert.True(sourceStream.Disposed, "Source stream should be disposed after buffering.");

            byte[] readBuf = new byte[8];
            int bytesRead = bufferedStream.Read(readBuf, 0, readBuf.Length);
            Assert.Equal(0, bytesRead);
        }

        [Fact]
        public async Task BufferStreamAsync_SmallStream_RoundTripsAllBytes()
        {
            byte[] data = CreatePattern(10000);
            var sourceStream = new DisposingMemoryStream(data);
            var bufferedStream = await BufferedReadStream.BufferStreamAsync(sourceStream);

            Assert.True(sourceStream.Disposed);

            byte[] sink = new byte[data.Length];
            int total = ReadAllBytes(bufferedStream, sink);

            Assert.Equal(data.Length, total);
            Assert.True(data.SequenceEqual(sink));
        }

        [Fact]
        public async Task BufferStreamAsync_MultiBufferStream_RoundTripsAllBytes()
        {
            int size = OneBufferSize * 2 + 17; // Force >2 buffers.
            byte[] data = CreatePattern(size);
            var sourceStream = new DisposingMemoryStream(data);
            var bufferedStream = await BufferedReadStream.BufferStreamAsync(sourceStream);

            byte[] sink = new byte[size];
            int total = ReadAllBytes(bufferedStream, sink, chunk: 13_333); // Odd chunk to cross boundaries.

            Assert.Equal(size, total);
            Assert.True(data.SequenceEqual(sink));
        }

        [Fact]
        public async Task BufferStreamAsync_ReadAfterEofReturnsZero()
        {
            byte[] data = CreatePattern(5000);
            var sourceStream = new DisposingMemoryStream(data);
            var bufferedStream = await BufferedReadStream.BufferStreamAsync(sourceStream);

            byte[] sink = new byte[data.Length];
            int first = ReadAllBytes(bufferedStream, sink);
            Assert.Equal(data.Length, first);

            byte[] extra = new byte[32];
            int second = bufferedStream.Read(extra, 0, extra.Length);
            Assert.Equal(0, second);
        }

        [Fact]
        public async Task BufferStreamAsync_PartialReadsRespectRequestedCount()
        {
            byte[] data = CreatePattern(4096);
            var sourceStream = new DisposingMemoryStream(data);
            var bufferedStream = await BufferedReadStream.BufferStreamAsync(sourceStream);

            byte[] aggregate = new byte[data.Length];
            int offset = 0;
            int[] requests = { 7, 31, 255, 1024, 900, 512, 700, 660, 10 }; // arbitrary
            foreach (int request in requests)
            {
                if (offset >= data.Length) break;
                int remaining = data.Length - offset;
                int expected = Math.Min(request, remaining);
                byte[] temp = new byte[request];
                int bytesRead = bufferedStream.Read(temp, 0, request);
                Assert.Equal(expected, bytesRead);
                Array.Copy(temp, 0, aggregate, offset, bytesRead);
                offset += bytesRead;
            }

            // Finish remaining
            offset += ReadAllBytes(bufferedStream, aggregate, offset);

            Assert.Equal(data.Length, offset);
            Assert.True(data.SequenceEqual(aggregate));
        }

        [Fact]
        public async Task BufferStreamAsync_ResetForReading_ReplaysContent()
        {
            byte[] data = CreatePattern(25_000);
            var sourceStream = new DisposingMemoryStream(data);
            var bufferedStream = await BufferedReadStream.BufferStreamAsync(sourceStream);

            // First pass
            byte[] first = new byte[data.Length];
            ReadAllBytes(bufferedStream, first);

            // Use internal ResetForReading via reflection (internal method)
            var resetForReadingMethod = typeof(BufferedReadStream).GetMethod(
                "ResetForReading",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            Assert.NotNull(resetForReadingMethod);
            resetForReadingMethod.Invoke(bufferedStream, null);

            byte[] second = new byte[data.Length];
            ReadAllBytes(bufferedStream, second);

            Assert.True(first.SequenceEqual(second));
            Assert.True(data.SequenceEqual(second));
        }

        [Fact]
        public async Task BufferStreamAsync_ThrowingStream_PropagatesCatchableException_AndDisposesSource()
        {
            var exception = new InvalidDataException("Boom!");
            var throwingStream = new ThrowingStream(bytesBeforeThrow: 5000, exceptionToThrow: exception);
            await Assert.ThrowsAsync<InvalidDataException>(() => BufferedReadStream.BufferStreamAsync(throwingStream));
            Assert.True(throwingStream.Disposed, "Source stream should be disposed on exception.");
        }

        [Fact]
        public async Task BufferStreamAsync_LargeExactBufferBoundary()
        {
            // Exactly N * BufferSize bytes -> Ensure final buffer recognized without extra allocation
            int size = OneBufferSize * 3;
            byte[] data = CreatePattern(size);
            var sourceStream = new DisposingMemoryStream(data);
            var bufferedStream = await BufferedReadStream.BufferStreamAsync(sourceStream);

            byte[] sink = new byte[size];
            ReadAllBytes(bufferedStream, sink, chunk: OneBufferSize / 3);
            Assert.True(data.SequenceEqual(sink));
        }

        #region Helper Methods

        private static byte[] CreatePattern(int size)
        {
            var data = new byte[size];
            for (int i = 0; i < size; i++)
            {
                data[i] = (byte)(i % 251); // prime-ish cycle
            }

            return data;
        }

        private static int ReadAllBytes(Stream stream, byte[] target, int startOffset = 0, int chunk = 8192)
        {
            int total = 0;
            int offset = startOffset;
            while (offset < target.Length)
            {
                int bytesToRead = Math.Min(chunk, target.Length - offset);
                int bytesRead = stream.Read(target, offset, bytesToRead);
                if (bytesRead == 0) break;
                offset += bytesRead;
                total += bytesRead;
            }

            return total;
        }

        #endregion Helper Methods

        #region Helper Classes

        // Simple memory stream with dispose tracking
        private sealed class DisposingMemoryStream : MemoryStream
        {
            public bool Disposed { get; private set; }
            public DisposingMemoryStream(byte[] data) : base(data, writable: false) { }
            protected override void Dispose(bool disposing)
            {
                Disposed = true;
                base.Dispose(disposing);
            }
        }

        private sealed class ThrowingStream : Stream
        {
            private readonly int bytesBeforeThrow;
            private readonly Exception exceptionToThrow;
            private int emitted;
            public bool Disposed { get; private set; }

            public ThrowingStream(int bytesBeforeThrow, Exception exceptionToThrow)
            {
                this.bytesBeforeThrow = bytesBeforeThrow;
                this.exceptionToThrow = exceptionToThrow;
            }

            public override bool CanRead => true;
            public override bool CanSeek => false;
            public override bool CanWrite => false;
            public override long Length => throw new NotSupportedException();
            public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
            public override void Flush() => throw new NotSupportedException();
            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
            public override void SetLength(long value) => throw new NotSupportedException();
            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

            public override int Read(byte[] buffer, int offset, int count)
                => throw new NotSupportedException("Synchronous reads not used in buffering.");

            public override async ValueTask<int> ReadAsync(Memory<byte> destination, CancellationToken cancellationToken = default)
            {
                await Task.Yield();
                if (emitted >= bytesBeforeThrow)
                {
                    throw exceptionToThrow;
                }
                int remaining = bytesBeforeThrow - emitted;
                int write = Math.Min(remaining, destination.Length);
                for (int i = 0; i < write; i++)
                {
                    destination.Span[i] = (byte)((emitted + i) % 253);
                }
                emitted += write;
                return write;
            }

            protected override void Dispose(bool disposing)
            {
                Disposed = true;
                base.Dispose(disposing);
            }
        }

        #endregion Helper Classes
    }
}

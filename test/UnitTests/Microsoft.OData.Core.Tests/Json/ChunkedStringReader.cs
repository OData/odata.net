//---------------------------------------------------------------------
// <copyright file="ChunkedStringReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.OData.Tests.Json
{
    /// <summary>
    /// Text reader that that simulates a chunked / streaming input source so JsonReader
    /// paths that depend on partial buffers and truly asynchronous completion are exercised.
    /// - Returns at most <c>chunkSize</c> characters per read to force multiple buffer refills.
    /// - All asynchronous APIs (`ReadAsync`, `ReadAsync(Memory<char>)`, `ReadToEndAsync`) first await <see cref="Task.Yield"/> so
    ///   the returned <see cref="ValueTask"/> / <see cref="Task"/> is not completed synchronously (forces “slow” paths).
    /// - Tracks whether any async operation actually completed asynchronously via <see cref="ObservedAsyncCompletion"/> for assertions.
    /// Use <see cref="ChunkedStringReader"/> to validate boundary conditions,
    /// incremental parsing, and correct handling of partially available data.
    /// </summary>
    internal sealed class ChunkedStringReader : TextReader
    {
        private readonly ReadOnlyMemory<char> data;
        private readonly int chunkSize;
        private int pos;
        private bool disposed;

        /// <summary>
        /// True if an async read completed asynchronously at least once.
        /// Useful for asserting the "slow path" was taken in tests.
        /// </summary>
        public bool ObservedAsyncCompletion { get; private set; }

        /// <param name="data">The source text to read.</param>
        /// <param name="chunkSize">
        /// Maximum number of characters to return per read.
        /// Use a small value (e.g., 64–256) to force refills during parsing.
        /// </param>
        public ChunkedStringReader(string data, int chunkSize = 128)
            : this((data ?? throw new ArgumentNullException(nameof(data))).AsMemory(), chunkSize)
        {
        }

        public ChunkedStringReader(ReadOnlyMemory<char> data, int chunkSize = 128)
        {
            if (chunkSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(chunkSize));
            }

            this.data = data;
            this.chunkSize = chunkSize;
            this.pos = 0;
        }

        public override void Close() => Dispose(true);

        protected override void Dispose(bool disposing)
        {
            this.disposed = true;
            base.Dispose(disposing);
        }

        private void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(ChunkedStringReader));
            }
        }

        public override int Peek()
        {
            ThrowIfDisposed();

            return this.pos < this.data.Length ? this.data.Span[this.pos] : -1;
        }

        public override int Read()
        {
            ThrowIfDisposed();
            if (this.pos >= this.data.Length) return -1;

            return this.data.Span[this.pos++];
        }

        public override int Read(char[] buffer, int index, int count)
        {
            ThrowIfDisposed();

            ArgumentNullException.ThrowIfNull(buffer);
            if ((uint)index > (uint)buffer.Length) throw new ArgumentOutOfRangeException(nameof(index));
            if ((uint)count > (uint)(buffer.Length - index)) throw new ArgumentOutOfRangeException(nameof(count));

            if (this.pos >= this.data.Length) return 0;

            int toCopy = Math.Min(Math.Min(count, this.chunkSize), this.data.Length - this.pos);
            this.data.Span.Slice(this.pos, toCopy).CopyTo(buffer.AsSpan(index, toCopy));
            this.pos += toCopy;

            return toCopy;
        }

        public override int Read(Span<char> buffer)
        {
            ThrowIfDisposed();
            if (this.pos >= this.data.Length) return 0;

            int toCopy = Math.Min(Math.Min(buffer.Length, this.chunkSize), this.data.Length - this.pos);
            this.data.Span.Slice(this.pos, toCopy).CopyTo(buffer[..toCopy]);
            this.pos += toCopy;

            return toCopy;
        }

        public override Task<int> ReadAsync(char[] buffer, int index, int count)
        {
            // Delegate to the Memory<char>-based implementation, which guarantees async completion.
            return ReadAsync(new Memory<char>(buffer, index, count), CancellationToken.None).AsTask();
        }

        public override async ValueTask<int> ReadAsync(Memory<char> buffer, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            // Ensure this ValueTask is *not* IsCompletedSuccessfully at the call-site.
            // Task.Yield() is lightweight and reliable for tests.
            await Task.Yield();

            cancellationToken.ThrowIfCancellationRequested();
            ObservedAsyncCompletion = true;

            if (this.pos >= this.data.Length) return 0;

            int toCopy = Math.Min(Math.Min(buffer.Length, this.chunkSize), this.data.Length - this.pos);
            this.data.Span.Slice(this.pos, toCopy).CopyTo(buffer.Span[..toCopy]);
            this.pos += toCopy;

            return toCopy;
        }

        public override async Task<string> ReadToEndAsync()
        {
            ThrowIfDisposed();

            // Force asynchrony once.
            await Task.Yield();
            ObservedAsyncCompletion = true;

            if (this.pos >= this.data.Length) return string.Empty;

            string str = new string(this.data.Span.Slice(this.pos));
            this.pos = this.data.Length;

            return str;
        }
    }
}

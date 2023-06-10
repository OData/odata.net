//---------------------------------------------------------------------
// <copyright file="AsyncOnlyStreamWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.OData.Tests
{
    /// <summary>
    /// A test stream wrapper that that throws an
    /// exception when synchronous I/O methods are called.
    /// This is meant for testing asynchronous code paths
    /// where synchronous I/O is not allowed.
    /// </summary>
    internal class AsyncOnlyStreamWrapper : Stream
    {
        private Stream innerStream;

        /// <summary>
        /// Creates a new instance of <see cref="AsyncOnlyStreamWrapper"/>.
        /// </summary>
        /// <param name="innerStream">The stream to be wrapped. All I/O will be handled by this stream.</param>
        public AsyncOnlyStreamWrapper(Stream innerStream)
        {
            this.innerStream = innerStream;
        }
        public override bool CanRead => this.innerStream.CanRead;

        public override bool CanSeek => this.innerStream.CanSeek;

        public override bool CanWrite => this.innerStream.CanWrite;

        public override long Length => this.innerStream.Length;

        public override long Position {
            get => this.innerStream.Position;
            set => this.innerStream.Position = value;
        }

        public override void Flush()
        {
            ThrowSyncIOException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            ThrowSyncIOException();
            // The return statement won't be reached
            // because an exception will be thrown. But the compiler can't see that.
            return -1;
        }

        public override void WriteByte(byte value) => ThrowSyncIOException();

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.ThrowSyncIOException();
        }

#if NETCOREAPP3_1_OR_GREATER
        public override void CopyTo(Stream destination, int bufferSize) => ThrowSyncIOException();
#endif


#if NETCOREAPP3_1_OR_GREATER
        public override void Write(ReadOnlySpan<byte> buffer) => ThrowSyncIOException();
#endif

#if NETCOREAPP3_1_OR_GREATER
        protected override void Dispose(bool disposing) => ThrowSyncIOException();
#else
        // in .NET Core <= 3.1 we don't support the async alternative DisposeAsync()
        protected override void Dispose(bool disposing) => this.innerStream.Dispose();
#endif

#if NETCOREAPP3_1_OR_GREATER
        public override void Close() => ThrowSyncIOException();
#endif

        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.innerStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            this.innerStream.SetLength(value);
        }

#if NETCOREAPP3_1_OR_GREATER
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) =>
            this.innerStream.ReadAsync(buffer, cancellationToken);
#endif

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => this.innerStream.ReadAsync(buffer, offset, count, cancellationToken);

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => this.innerStream.WriteAsync(buffer, offset, count, cancellationToken);

#if NETCOREAPP3_1_OR_GREATER

        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
            => this.innerStream.WriteAsync(buffer, cancellationToken);
#endif

        public override Task FlushAsync(CancellationToken cancellationToken) => this.innerStream.FlushAsync(cancellationToken);

#if NETCOREAPP3_1_OR_GREATER
        public override ValueTask DisposeAsync() => this.innerStream.DisposeAsync();
#endif
        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken) =>
            this.innerStream.CopyToAsync(destination, bufferSize, cancellationToken);

        private void ThrowSyncIOException()
        {
            throw new Exception("Synchronous I/O is not allowed for asynchronous stream.");
        }
    }
}


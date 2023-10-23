//---------------------------------------------------------------------
// <copyright file="AsyncStream.cs" company="Microsoft">
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
    internal class AsyncStream : Stream
    {
        private Stream innerStream;

        /// <summary>
        /// Creates a new instance of <see cref="AsyncStream"/>,
        /// a test stream wrapper that that throws an
        /// exception when synchronous I/O methods are called.
        /// This is meant for testing asynchronous code paths
        /// where synchronous I/O is not allowed.
        /// </summary>
        /// <param name="innerStream">The stream to be wrapped. All I/O will be handled by this stream.</param>
        public AsyncStream(Stream innerStream)
        {
            this.innerStream = innerStream;
        }
        public override bool CanRead => this.innerStream.CanRead;

        public override bool CanSeek => this.innerStream.CanSeek;

        public override bool CanWrite => this.innerStream.CanWrite;

        public override bool CanTimeout => this.innerStream.CanTimeout;

        public override long Length => this.innerStream.Length;

        public bool Disposed { get; private set; }

        public override long Position {
            get => this.innerStream.Position;
            set => this.innerStream.Position = value;
        }

        public override int ReadTimeout
        {
            get => this.innerStream.ReadTimeout;
            set => this.innerStream.ReadTimeout = value;
        }

        public override int WriteTimeout
        {
            get => this.innerStream.WriteTimeout;
            set => this.innerStream.ReadTimeout = value;
        }

        public override void Flush()
        {
#if NETCOREAPP3_1_OR_GREATER
            throw new SynchronousIOException();
#else
            // We allow synchronous flushing in older frameworks
            // because we also allow synchronous Dispose()
            // which often calls Flush()
            this.innerStream.Flush();
#endif
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new SynchronousIOException();
        }
        public override int ReadByte()
        {
            throw new SynchronousIOException();
        }

#if NETCOREAPP3_1_OR_GREATER
        public override int Read(Span<byte> buffer)
        {
            throw new SynchronousIOException();
        }
#endif

        public override void WriteByte(byte value) => throw new SynchronousIOException();

        public override void Write(byte[] buffer, int offset, int count)
        {
#if NETCOREAPP3_1_OR_GREATER
            throw new SynchronousIOException();
#else
            // We allow synchronous flushing in older frameworks
            // because we also allow synchronous Dispose()
            // which often calls Flush()
            this.innerStream.Write(buffer, offset, count);
#endif
        }

#if NETCOREAPP3_1_OR_GREATER
        public override void CopyTo(Stream destination, int bufferSize) => throw new SynchronousIOException();
#endif


#if NETCOREAPP3_1_OR_GREATER
        public override void Write(ReadOnlySpan<byte> buffer) => throw new SynchronousIOException();
#endif

#if NETCOREAPP3_1_OR_GREATER
        protected override void Dispose(bool disposing) => throw new SynchronousIOException();
#else
        // In .NET Core <= 3.1 we don't support the async alternative DisposeAsync()
        // So let's allow sync Dispose there cause there's no alternative
        protected override void Dispose(bool disposing) => this.innerStream.Dispose();
#endif

#if NETCOREAPP3_1_OR_GREATER
        public override void Close() => throw new SynchronousIOException();
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
        public override async ValueTask DisposeAsync()
        {
            await this.innerStream.DisposeAsync();
            this.Disposed = true;
        }
#endif
        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken) =>
            this.innerStream.CopyToAsync(destination, bufferSize, cancellationToken);

#if NETCOREAPP3_1_OR_GREATER
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state) =>
            this.innerStream.BeginRead(buffer, offset, count, callback, state);

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state) =>
            this.innerStream.BeginWrite(buffer, offset, count, callback, state);

        public override int EndRead(IAsyncResult asyncResult) => this.innerStream.EndRead(asyncResult);

        public override void EndWrite(IAsyncResult asyncResult) => this.innerStream.EndWrite(asyncResult);
#endif

        public override string ToString() => this.innerStream.ToString();
    }
}


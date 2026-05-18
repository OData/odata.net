//---------------------------------------------------------------------
// <copyright file="AsyncReadOnlyStream.cs" company="Microsoft">
// Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.OData.Client.Tests.Tracking
{
    internal sealed class AsyncReadOnlyStream : Stream
    {
        private readonly Stream innerStream;

        internal AsyncReadOnlyStream(Stream innerStream)
        {
            this.innerStream = innerStream ?? throw new ArgumentNullException(nameof(innerStream));
        }

        public override bool CanRead => this.innerStream.CanRead;

        public override bool CanSeek => this.innerStream.CanSeek;

        public override bool CanWrite => this.innerStream.CanWrite;

        public override long Length => this.innerStream.Length;

        public override long Position
        {
            get => this.innerStream.Position;
            set => this.innerStream.Position = value;
        }

        public override void Flush() => this.innerStream.Flush();

        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException("net_http_synchronous_reads_not_supported");

        public override int Read(Span<byte> buffer) => throw new NotSupportedException("net_http_synchronous_reads_not_supported");

        public override int ReadByte() => throw new NotSupportedException("net_http_synchronous_reads_not_supported");

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return this.innerStream.BeginRead(buffer, offset, count, callback, state);
        }

        public override int EndRead(IAsyncResult asyncResult) => throw new NotSupportedException("net_http_synchronous_reads_not_supported");

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
            this.innerStream.ReadAsync(buffer, offset, count, cancellationToken);

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) =>
            this.innerStream.ReadAsync(buffer, cancellationToken);

        public override long Seek(long offset, SeekOrigin origin) => this.innerStream.Seek(offset, origin);

        public override void SetLength(long value) => this.innerStream.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count) => this.innerStream.Write(buffer, offset, count);

        public override void Write(ReadOnlySpan<byte> buffer) => this.innerStream.Write(buffer);

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
            this.innerStream.WriteAsync(buffer, offset, count, cancellationToken);

        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default) =>
            this.innerStream.WriteAsync(buffer, cancellationToken);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.innerStream.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}

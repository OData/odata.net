using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.OData.Tests
{
    /// <summary>
    /// Stream wrapper that yields in every async method
    /// to ensure the read/write operations complete asynchronously
    /// even when the operate completely in-memory. This helps simulate
    /// async I/O in unit tests without making requests out of the process.
    /// </summary>
    internal class AsyncYieldStream : Stream
    {
        private Stream stream;
        public AsyncYieldStream(Stream stream)
        {
            this.stream = stream;
        }

        public override bool CanRead => this.stream.CanRead;

        public override bool CanSeek => this.stream.CanSeek;

        public override bool CanWrite => this.stream.CanWrite;

        public override long Length => this.stream.Length;

        public override int ReadTimeout { get => this.stream.ReadTimeout; set => this.stream.ReadTimeout = value; }

        public override int WriteTimeout { get => this.stream.WriteTimeout; set => this.stream.WriteTimeout = value; }

        public override bool CanTimeout => this.stream.CanTimeout;

        public override void Close()
        {
            this.stream.Close();
        }

        public override long Position { get => this.stream.Position; set => this.stream.Position = value; }

        public override void Flush()
        {
            this.stream.Flush();
        }

        public override async Task FlushAsync(CancellationToken cancellationToken)
        {
            await Task.Yield();
            await base.FlushAsync(cancellationToken);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.stream.Read(buffer, offset, count);
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            await Task.Yield();
            int result = await this.stream.ReadAsync(buffer, offset, count, cancellationToken);
            return result;
        }

        public override int ReadByte()
        {
            return this.stream.ReadByte();
        }

        public override void WriteByte(byte value)
        {
            this.stream.WriteByte(value);
        }

        public override async Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            await Task.Yield();
            await this.stream.CopyToAsync(destination, bufferSize, cancellationToken);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            this.stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.stream.Write(buffer, offset, count);
        }

        public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            await base.WriteAsync(buffer, cancellationToken);
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return this.stream.BeginRead(buffer, offset, count, callback, state);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return this.stream.BeginWrite(buffer, offset, count, callback, state);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            return this.stream.EndRead(asyncResult);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            this.stream.EndWrite(asyncResult);
        }

        public override string ToString()
        {
            return this.stream.ToString();
        }
    }
}
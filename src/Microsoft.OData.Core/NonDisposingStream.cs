//---------------------------------------------------------------------
// <copyright file="NonDisposingStream.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.IO;
#if PORTABLELIB
    using System.Threading;
    using System.Threading.Tasks;
#endif
    #endregion Namespaces

    /// <summary>
    /// Stream wrapper for the message stream to ignore the Stream.Dispose method so that readers/writers on top of
    /// it can be disposed without affecting it.
    /// </summary>
    internal sealed class NonDisposingStream : Stream
    {
        /// <summary>
        /// Stream that is being wrapped.
        /// </summary>
        private readonly Stream innerStream;

        /// <summary>
        /// Constructs an instance of the stream wrapper class.
        /// </summary>
        /// <param name="innerStream">Stream that is being wrapped.</param>
        internal NonDisposingStream(Stream innerStream)
        {
            Debug.Assert(innerStream != null, "innerStream != null");
            this.innerStream = innerStream;
        }

        /// <summary>
        /// Determines if the stream can read.
        /// </summary>
        public override bool CanRead
        {
            get { return this.innerStream.CanRead; }
        }

        /// <summary>
        /// Determines if the stream can seek.
        /// </summary>
        public override bool CanSeek
        {
            get { return this.innerStream.CanSeek; }
        }

        /// <summary>
        /// Determines if the stream can write.
        /// </summary>
        public override bool CanWrite
        {
            get { return this.innerStream.CanWrite; }
        }

        /// <summary>
        /// Returns the length of the stream.
        /// </summary>
        public override long Length
        {
            get { return this.innerStream.Length; }
        }

        /// <summary>
        /// Gets or sets the position in the stream.
        /// </summary>
        public override long Position
        {
            get { return this.innerStream.Position; }
            set { this.innerStream.Position = value; }
        }

        /// <summary>
        /// Flush the stream to the underlying storage.
        /// </summary>
        public override void Flush()
        {
            this.innerStream.Flush();
        }

        /// <summary>
        /// Reads data from the stream.
        /// </summary>
        /// <param name="buffer">The buffer to read the data to.</param>
        /// <param name="offset">The offset in the buffer to write to.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The number of bytes actually read.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.innerStream.Read(buffer, offset, count);
        }

#if PORTABLELIB
        /// <inheritdoc />
        public async override Task<int> ReadAsync(
            byte[] buffer,
            int offset,
            int count,
            CancellationToken cancellationToken)
        {
            int bytesRead = await this.innerStream.ReadAsync(buffer, offset, count, cancellationToken);
            return bytesRead;
        }
#else
        /// <summary>
        /// Begins a read operation from the stream.
        /// </summary>
        /// <param name="buffer">The buffer to read the data to.</param>
        /// <param name="offset">The offset in the buffer to write to.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <param name="callback">The async callback.</param>
        /// <param name="state">The async state.</param>
        /// <returns>Async result representing the asynchornous operation.</returns>
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return this.innerStream.BeginRead(buffer, offset, count, callback, state);
        }

        /// <summary>
        /// Ends a read operation from the stream.
        /// </summary>
        /// <param name="asyncResult">The async result representing the read operation.</param>
        /// <returns>The number of bytes actually read.</returns>
        public override int EndRead(IAsyncResult asyncResult)
        {
            return this.innerStream.EndRead(asyncResult);
        }
#endif

        /// <summary>
        /// Seeks the stream.
        /// </summary>
        /// <param name="offset">The offset to seek to.</param>
        /// <param name="origin">The origin of the seek operation.</param>
        /// <returns>The new position in the stream.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.innerStream.Seek(offset, origin);
        }

        /// <summary>
        /// Sets the length of the stream.
        /// </summary>
        /// <param name="value">The length in bytes to set.</param>
        public override void SetLength(long value)
        {
            this.innerStream.SetLength(value);
        }

        /// <summary>
        /// Writes to the stream.
        /// </summary>
        /// <param name="buffer">The buffer to get data from.</param>
        /// <param name="offset">The offset in the buffer to start from.</param>
        /// <param name="count">The number of bytes to write.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            this.innerStream.Write(buffer, offset, count);
        }

#if PORTABLELIB
        /// <inheritdoc />
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return this.innerStream.WriteAsync(buffer, offset, count, cancellationToken);
        }
#else
        /// <summary>
        /// Begins an asynchronous write operation to the stream.
        /// </summary>
        /// <param name="buffer">The buffer to get data from.</param>
        /// <param name="offset">The offset in the buffer to start from.</param>
        /// <param name="count">The number of bytes to write.</param>
        /// <param name="callback">The async callback.</param>
        /// <param name="state">The async state.</param>
        /// <returns>Async result representing the write operation.</returns>
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return this.innerStream.BeginWrite(buffer, offset, count, callback, state);
        }

        /// <summary>
        /// Ends the asynchronous write operation.
        /// </summary>
        /// <param name="asyncResult">Async result representing the write operation.</param>
        public override void EndWrite(IAsyncResult asyncResult)
        {
            this.innerStream.EndWrite(asyncResult);
        }
#endif
    }
}

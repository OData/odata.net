//---------------------------------------------------------------------
// <copyright file="ODataWriteStream.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.OData
{
    /// <summary>
    /// A stream handed to clients from ODataBatchOperationMessage.GetStream or ODataBatchOperationMessage.GetStreamAsync,
    /// or to write an inline stream value.
    /// This stream communicates status changes to the owning writer (via IODataStreamListener)
    /// to properly flush buffered data and move the writer's state machine forward.
    /// </summary>
    internal sealed class ODataWriteStream : ODataStream
    {
        /// <summary>The batch stream underlying this operation stream.</summary>
        private Stream stream;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="stream">The underlying stream to write the message to.</param>
        /// <param name="listener">Listener interface to be notified of operation changes.</param>
        internal ODataWriteStream(Stream stream, IODataStreamListener listener, bool synchronous = true)
            : base(listener, synchronous)
        {
            Debug.Assert(stream != null, "stream != null");
            this.stream = stream;
        }

        /// <summary>
        /// Determines if the stream can read - this one can't
        /// </summary>
        public override bool CanRead
        {
            get { return false; }
        }

        /// <summary>
        /// Determines if the stream can seek - this one can't
        /// </summary>
        public override bool CanSeek
        {
            get { return false; }
        }

        /// <summary>
        /// Determines if the stream can write - this one can
        /// </summary>
        public override bool CanWrite
        {
            get { return true; }
        }

        /// <summary>
        /// Returns the length of the stream.
        /// </summary>
        public override long Length
        {
            get
            {
                this.ValidateNotDisposed();
                return this.stream.Length;
            }
        }

        /// <summary>
        /// Gets or sets the position in the stream. Setting of the position is not supported since the stream doesn't support seeking.
        /// </summary>
        public override long Position
        {
            get
            {
                this.ValidateNotDisposed();
                return this.stream.Position;
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Sets the length of the stream.
        /// </summary>
        /// <param name="value">The length in bytes to set.</param>
        public override void SetLength(long value)
        {
            this.ValidateNotDisposed();
            this.stream.SetLength(value);
        }

        /// <summary>
        /// Writes to the stream.
        /// </summary>
        /// <param name="buffer">The buffer to get data from.</param>
        /// <param name="offset">The offset in the buffer to start from.</param>
        /// <param name="count">The number of bytes to write.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            this.ValidateNotDisposed();
            this.stream.Write(buffer, offset, count);
        }

        /// <inheritdoc />
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            this.ValidateNotDisposed();
            return this.stream.WriteAsync(buffer, offset, count, cancellationToken);
        }

        /// <summary>
        /// Reads data from the stream. This operation is not supported by this stream.
        /// </summary>
        /// <param name="buffer">The buffer to read the data to.</param>
        /// <param name="offset">The offset in the buffer to write to.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The number of bytes actually read.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Flush the stream to the underlying batch stream.
        /// </summary>
        public override void Flush()
        {
            this.ValidateNotDisposed();
            this.stream.Flush();
        }

        /// <summary>
        /// Asynchronously flush the stream to the underlying stream
        /// </summary>
        public override async Task FlushAsync(CancellationToken cancellationToken)
        {
            this.ValidateNotDisposed();
            await this.stream.FlushAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Dispose the operation stream.
        /// </summary>
        /// <param name="disposing">If 'true' this method is called from user code; if 'false' it is called by the runtime.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // NOTE: don't dispose the batch stream since this instance does not own it.
                this.stream = null;
            }

            base.Dispose(disposing);
        }

#if NETSTANDARD2_0
        /// <summary>
        /// Encapsulates the common asynchronous cleanup operations.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        protected override ValueTask DisposeAsyncCore()
        {
            this.stream = null;

            return base.DisposeAsyncCore();
        }
#endif
    }
}

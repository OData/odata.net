//---------------------------------------------------------------------
// <copyright file="ODataBatchOperationReadStream.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Diagnostics;

    #endregion Namespaces

    /// <summary>
    /// A stream handed to clients from ODataBatchOperationMessage.GetStream or ODataBatchOperationMessage.GetStreamAsync.
    /// This stream communicates status changes to the owning batch reader (via IODataBatchOperationListener)
    /// to prevent clients to use the batch reader while a content stream is still in use.
    /// </summary>
    internal abstract class ODataBatchOperationReadStream : ODataBatchOperationStream
    {
        /// <summary>
        /// The batch stream underlying this operation stream.
        /// </summary>
        protected ODataBatchReaderStream batchReaderStream;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="batchReaderStream">The underlying stream to read from.</param>
        /// <param name="listener">Listener interface to be notified of operation changes.</param>
        private ODataBatchOperationReadStream(ODataBatchReaderStream batchReaderStream, IODataBatchOperationListener listener)
            : base(listener)
        {
            Debug.Assert(batchReaderStream != null, "batchReaderStream != null");
            this.batchReaderStream = batchReaderStream;
        }

        /// <summary>
        /// Determines if the stream can read - this one can
        /// </summary>
        public override bool CanRead
        {
            get { return true; }
        }

        /// <summary>
        /// Determines if the stream can seek - this one can't
        /// </summary>
        public override bool CanSeek
        {
            get { return false; }
        }

        /// <summary>
        /// Determines if the stream can write - this one can't
        /// </summary>
        public override bool CanWrite
        {
            get { return false; }
        }

        /// <summary>
        /// Returns the length of the stream. Not supported by this stream.
        /// </summary>
        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets or sets the position in the stream. Not supported by this stream.
        /// </summary>
        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Flush the stream; not supported for a read stream.
        /// </summary>
        public override void Flush()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Sets the length of the stream.
        /// </summary>
        /// <param name="value">The length in bytes to set.</param>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Writes to the stream.
        /// </summary>
        /// <param name="buffer">The buffer to get data from.</param>
        /// <param name="offset">The offset in the buffer to start from.</param>
        /// <param name="count">The number of bytes to write.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Create a batch operation read stream over the specified batch stream with a given content length.
        /// </summary>
        /// <param name="batchReaderStream">The batch stream underlying the operation stream to create.</param>
        /// <param name="listener">The batch operation listener.</param>
        /// <param name="length">The content length of the operation stream.</param>
        /// <returns>A <see cref="ODataBatchOperationReadStream"/> to read the content of a batch operation from.</returns>
        internal static ODataBatchOperationReadStream Create(ODataBatchReaderStream batchReaderStream, IODataBatchOperationListener listener, int length)
        {
            return new ODataBatchOperationReadStreamWithLength(batchReaderStream, listener, length);
        }

        /// <summary>
        /// Create a batch operation read stream over the specified batch stream using the batch delimiter to detect the end of the stream.
        /// </summary>
        /// <param name="batchReaderStream">The batch stream underlying the operation stream to create.</param>
        /// <param name="listener">The batch operation listener.</param>
        /// <returns>A <see cref="ODataBatchOperationReadStream"/> to read the content of a batch operation from.</returns>
        internal static ODataBatchOperationReadStream Create(ODataBatchReaderStream batchReaderStream, IODataBatchOperationListener listener)
        {
            return new ODataBatchOperationReadStreamWithDelimiter(batchReaderStream, listener);
        }

        /// <summary>
        /// A batch operation stream with the content length specified.
        /// </summary>
        private sealed class ODataBatchOperationReadStreamWithLength : ODataBatchOperationReadStream
        {
            /// <summary>The length of the operation content.</summary>
            private int length;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="batchReaderStream">The underlying batch stream to write the message to.</param>
            /// <param name="listener">Listener interface to be notified of operation changes.</param>
            /// <param name="length">The total length of the stream.</param>
            internal ODataBatchOperationReadStreamWithLength(ODataBatchReaderStream batchReaderStream, IODataBatchOperationListener listener, int length)
                : base(batchReaderStream, listener)
            {
                ExceptionUtils.CheckIntegerNotNegative(length, "length");
                this.length = length;
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
                ExceptionUtils.CheckArgumentNotNull(buffer, "buffer");
                ExceptionUtils.CheckIntegerNotNegative(offset, "offset");
                ExceptionUtils.CheckIntegerNotNegative(count, "count");
                this.ValidateNotDisposed();

                if (this.length == 0)
                {
                    // Nothing left to read.
                    return 0;
                }

                int bytesRead = this.batchReaderStream.ReadWithLength(buffer, offset, Math.Min(count, this.length));
                this.length -= bytesRead;
                Debug.Assert(this.length >= 0, "Read beyond expected length.");
                return bytesRead;
            }
        }

        /// <summary>
        /// A batch operation read stream with no content length so we have to check for the boundary.
        /// </summary>
        private sealed class ODataBatchOperationReadStreamWithDelimiter : ODataBatchOperationReadStream
        {
            /// <summary>true if the stream has been exhausted and no further reads can happen; otherwise false.</summary>
            private bool exhausted;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="batchReaderStream">The underlying batch stream to write the message to.</param>
            /// <param name="listener">Listener interface to be notified of operation changes.</param>
            internal ODataBatchOperationReadStreamWithDelimiter(ODataBatchReaderStream batchReaderStream, IODataBatchOperationListener listener)
                : base(batchReaderStream, listener)
            {
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
                ExceptionUtils.CheckArgumentNotNull(buffer, "buffer");
                ExceptionUtils.CheckIntegerNotNegative(offset, "offset");
                ExceptionUtils.CheckIntegerNotNegative(count, "count");

                this.ValidateNotDisposed();

                if (this.exhausted)
                {
                    return 0;
                }

                int bytesRead = this.batchReaderStream.ReadWithDelimiter(buffer, offset, count);
                if (bytesRead < count)
                {
                    this.exhausted = true;
                }

                return bytesRead;
            }
        }
    }
}

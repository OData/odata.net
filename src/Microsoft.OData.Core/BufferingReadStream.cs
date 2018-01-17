//---------------------------------------------------------------------
// <copyright file="BufferingReadStream.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    #endregion Namespaces

    /// <summary>
    /// Read-only stream which initially buffers all read data in order to replay it later.
    /// Once no more buffered data exists it reads from the underlying stream directly.
    /// </summary>
    internal sealed class BufferingReadStream : Stream
    {
        /// <summary>The list of buffered chunks of bytes as requested by callers.</summary>
        private readonly LinkedList<byte[]> buffers;

        /// <summary>
        /// The stream being wrapped.sdfasdf
        /// </summary>
        private Stream innerStream;

        /// <summary>The read position in the current buffer.</summary>
        private int positionInCurrentBuffer;

        /// <summary>
        /// true if the reader is not in buffering mode; otherwise false.
        /// </summary>
        private bool bufferingModeDisabled;

        /// <summary>
        /// The current node in the buffer list to read from.
        /// </summary>
        private LinkedListNode<byte[]> currentReadNode;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stream">The underlying stream to wrap. Note that only read operations will be invoked on this stream.</param>
        internal BufferingReadStream(Stream stream)
        {
            Debug.Assert(stream != null, "stream != null");

            this.innerStream = stream;
            this.buffers = new LinkedList<byte[]>();
        }

        /// <summary>
        /// Determines if the stream can read - this one can.
        /// </summary>
        public override bool CanRead
        {
            get { return true; }
        }

        /// <summary>
        /// Determines if the stream can seek - this one cannot
        /// </summary>
        public override bool CanSeek
        {
            get { return false; }
        }

        /// <summary>
        /// Determines if the stream can write - this one cannot
        /// </summary>
        public override bool CanWrite
        {
            get { return false; }
        }

        /// <summary>
        /// Returns the length of the stream, which this implementation doesn't support.
        /// </summary>
        public override long Length
        {
            get
            {
                Debug.Assert(false, "Should never get here.");
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets or sets the position in the stream, this stream doesn't support seeking, so position is also unsupported.
        /// </summary>
        public override long Position
        {
            get
            {
                Debug.Assert(false, "Should never get here.");
                throw new NotSupportedException();
            }

            set
            {
                Debug.Assert(false, "Should never get here.");
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// true if the stream is in buffering mode; otherwise false.
        /// </summary>
        internal bool IsBuffering
        {
            get
            {
                return !this.bufferingModeDisabled;
            }
        }

        /// <summary>
        /// Not supported since the stream only allows reading.
        /// </summary>
        public override void Flush()
        {
            Debug.Assert(false, "Should never get here.");
            throw new NotSupportedException();
        }

        /// <summary>
        /// Reads data from the buffer or the underlying stream.
        /// </summary>
        /// <param name="userBuffer">The buffer to read the data to.</param>
        /// <param name="offset">The offset in the buffer to write to.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The number of bytes actually read.</returns>
        public override int Read(byte[] userBuffer, int offset, int count)
        {
            ExceptionUtils.CheckArgumentNotNull(userBuffer, "userBuffer");
            ExceptionUtils.CheckIntegerNotNegative(offset, "offset");
            ExceptionUtils.CheckIntegerPositive(count, "count");

            int bytesRead = 0;

            // See whether we still have buffered data and read from it if we have;
            // NOTE When not reading from the buffer the currentReadNode must be null.
            while (this.currentReadNode != null && count > 0)
            {
                byte[] currentBytes = this.currentReadNode.Value;
                int bytesInCurrentBuffer = currentBytes.Length - this.positionInCurrentBuffer;
                if (bytesInCurrentBuffer == count)
                {
                    // Copy all the remaining bytes of the current buffer to the user buffer
                    // and move to the next buffer
                    Buffer.BlockCopy(currentBytes, this.positionInCurrentBuffer, userBuffer, offset, count);
                    bytesRead += count;

                    this.MoveToNextBuffer();
                    return bytesRead;
                }

                if (bytesInCurrentBuffer > count)
                {
                    // Copy the requested number of bytes to the user buffer
                    // and update the position in the current buffer
                    Buffer.BlockCopy(currentBytes, this.positionInCurrentBuffer, userBuffer, offset, count);
                    bytesRead += count;
                    this.positionInCurrentBuffer += count;
                    return bytesRead;
                }

                // Copy the remaining bytes of the current buffer to the user buffer and
                // move to the next buffer
                Buffer.BlockCopy(currentBytes, this.positionInCurrentBuffer, userBuffer, offset, bytesInCurrentBuffer);
                bytesRead += bytesInCurrentBuffer;
                offset += bytesInCurrentBuffer;
                count -= bytesInCurrentBuffer;

                this.MoveToNextBuffer();
            }

            // When we get here we either could not satisfy the requested number of bytes
            // from the buffers or are in buffering mode.
            Debug.Assert(this.currentReadNode == null, "No current read node should exist if we are not working off the buffers.");
            int bytesReadFromInnerStream = this.innerStream.Read(userBuffer, offset, count);

            // If we are in buffering mode, store the read bytes in our buffer
            if (!this.bufferingModeDisabled && bytesReadFromInnerStream > 0)
            {
                byte[] newDataBuffer = new byte[bytesReadFromInnerStream];
                Buffer.BlockCopy(userBuffer, offset, newDataBuffer, 0, bytesReadFromInnerStream);
                this.buffers.AddLast(newDataBuffer);
            }

            return bytesRead + bytesReadFromInnerStream;
        }

        /// <summary>
        /// Seeks the stream. This operation is not supported by this stream.
        /// </summary>
        /// <param name="offset">The offset to seek to.</param>
        /// <param name="origin">The origin of the seek operation.</param>
        /// <returns>The new position in the stream.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            Debug.Assert(false, "Should never get here.");
            throw new NotSupportedException();
        }

        /// <summary>
        /// Sets the length of the stream. This operation is not supported by this stream.
        /// </summary>
        /// <param name="value">The length in bytes to set.</param>
        public override void SetLength(long value)
        {
            Debug.Assert(false, "Should never get here.");
            throw new NotSupportedException();
        }

        /// <summary>
        /// Writes to the stream. This operation is not supported by this stream.
        /// </summary>
        /// <param name="buffer">The buffer to get data from.</param>
        /// <param name="offset">The offset in the buffer to start from.</param>
        /// <param name="count">The number of bytes to write.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            Debug.Assert(false, "Should never get here.");
            throw new NotSupportedException();
        }

        /// <summary>
        /// Stops the buffering mode and turns the reader into normal read mode where first
        /// the buffered data is re-read before the reads are performed on the underlying stream.
        /// </summary>
        internal void ResetStream()
        {
            Debug.Assert(!this.bufferingModeDisabled, "Cannot reset the stream again once buffering has been turned off!");

            this.currentReadNode = this.buffers.First;
            this.positionInCurrentBuffer = 0;
        }

        /// <summary>
        /// Stop buffering.
        /// </summary>
        internal void StopBuffering()
        {
            Debug.Assert(!this.bufferingModeDisabled, "Cannot stop buffering if the stream is not buffering!");

            this.ResetStream();
            this.bufferingModeDisabled = true;
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">True if called from Dispose; false if called from the finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            // Only honor dispose calls once buffering has been stopped.
            if (this.bufferingModeDisabled)
            {
                if (disposing)
                {
                    if (this.innerStream != null)
                    {
                        this.innerStream.Dispose();
                        this.innerStream = null;
                    }
                }

                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Moves the reader to the next buffer and drops already consumed
        /// data if not in buffering mode.
        /// </summary>
        private void MoveToNextBuffer()
        {
            // Drop the consumed data if not in buffering mode and continue
            // reading from the buffer if more data is available; if in
            // buffering mode, don't drop any data just move to the next buffer.
            if (this.bufferingModeDisabled)
            {
                this.buffers.RemoveFirst();
                this.currentReadNode = this.buffers.First;
            }
            else
            {
                this.currentReadNode = this.currentReadNode.Next;
            }

            this.positionInCurrentBuffer = 0;
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="BufferedReadStream.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
#if PORTABLELIB
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;
    #endregion Namespaces

    /// <summary>
    /// Class which takes an input stream, buffers the entire content asynchronously and exposes it as a stream
    /// which can be read synchronously.
    /// </summary>
    internal sealed class BufferedReadStream : Stream
    {
        /// <summary>
        /// List of buffers which store the data.
        /// </summary>
        private readonly List<DataBuffer> buffers;

        /// <summary>
        /// The input stream to read from. This is used only during the buffering and is set to null once we've buffered everything.
        /// </summary>
        private Stream inputStream;

        /// <summary>
        /// Points to the buffer currently being processed.
        /// When writing into the buffers this points to the last buffer to which the bytes should be written.
        /// When reading from the buffers this points to the buffer from which we are currently reading.
        /// </summary>
        private int currentBufferIndex;

        /// <summary>
        /// Number of bytes read from the current buffer.
        /// </summary>
        private int currentBufferReadCount;

        /// <summary>
        /// Private constructor.
        /// </summary>
        /// <param name="inputStream">The stream to read from.</param>
        private BufferedReadStream(Stream inputStream)
        {
            this.buffers = new List<DataBuffer>();
            this.inputStream = inputStream;
            this.currentBufferIndex = -1;
        }

        /// <summary>
        /// Determines if the stream can read - this one can
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
        /// Flush the stream to the underlying storage. This operation is not supported by this stream.
        /// </summary>
        public override void Flush()
        {
            Debug.Assert(false, "Should never get here.");
            throw new NotSupportedException();
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
            // Note that it's OK to assert the inputs as this stream should never be exposed to the user
            // it will only be used internally.
            ExceptionUtils.CheckArgumentNotNull(buffer, "buffer");
            Debug.Assert(offset < buffer.Length, "offset < buffer.Length");
            Debug.Assert(count <= buffer.Length - offset, "count <= buffer.Length - offset");
            Debug.Assert(this.inputStream == null, "Can't start reading until the buffering is finished.");
            Debug.Assert(this.currentBufferIndex >= -1 && this.currentBufferIndex < this.buffers.Count, "The currentBufferIndex is outside of the valid range.");

            if (this.currentBufferIndex == -1)
            {
                return 0;
            }

            DataBuffer currentBuffer = this.buffers[this.currentBufferIndex];
            Debug.Assert(this.currentBufferReadCount <= currentBuffer.StoredCount, "this.currentBufferReadCount <= currentBuffer.StoredCount");
            while (this.currentBufferReadCount >= currentBuffer.StoredCount)
            {
                // We've read all the data in the current buffer - move to the next one
                this.currentBufferIndex++;
                if (this.currentBufferIndex >= this.buffers.Count)
                {
                    this.currentBufferIndex = -1;
                    return 0;
                }

                currentBuffer = this.buffers[this.currentBufferIndex];
                this.currentBufferReadCount = 0;
            }

            int readCount = count;
            if (count > (currentBuffer.StoredCount - this.currentBufferReadCount))
            {
                readCount = currentBuffer.StoredCount - this.currentBufferReadCount;
            }

            Array.Copy(currentBuffer.Buffer, this.currentBufferReadCount, buffer, offset, readCount);
            this.currentBufferReadCount += readCount;
            Debug.Assert(this.currentBufferReadCount <= currentBuffer.StoredCount, "this.currentBufferReadCount <= currentBuffer.StoredCount");

            return readCount;
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
        /// Given the <paramref name="inputStream"/> this method returns a task which will asynchronously
        /// read the entire content of that stream and return a new synchronous stream from which the data can be read.
        /// </summary>
        /// <param name="inputStream">The input stream to asynchronously buffer.</param>
        /// <returns>A task which returns the buffered stream.</returns>
        internal static Task<BufferedReadStream> BufferStreamAsync(Stream inputStream)
        {
            Debug.Assert(inputStream != null, "inputStream != null");

            BufferedReadStream bufferedReadStream = new BufferedReadStream(inputStream);

            // Note that this relies on lazy eval of the enumerator
            return Task.Factory.Iterate(bufferedReadStream.BufferInputStream())
                .FollowAlwaysWith((task) => inputStream.Dispose())
                .FollowOnSuccessWith(
                    (task) =>
                    {
                        bufferedReadStream.ResetForReading();
                        return bufferedReadStream;
                    });
        }

        /// <summary>
        /// Resets the stream to the begining and prepares it for reading.
        /// </summary>
        internal void ResetForReading()
        {
            Debug.Assert(this.inputStream == null, "Can't start reading until the buffering is finished.");

            this.currentBufferIndex = this.buffers.Count == 0 ? -1 : 0;
            this.currentBufferReadCount = 0;
        }

        /// <summary>
        /// Returns enumeration of tasks to run to buffer the entire input stream.
        /// </summary>
        /// <returns>Enumeration of tasks to run to buffer the input stream.</returns>
        /// <remarks>This method relies on lazy eval of the enumerator, never enumerate through it synchronously.</remarks>
        private IEnumerable<Task> BufferInputStream()
        {
            while (this.inputStream != null)
            {
                Debug.Assert(this.currentBufferIndex >= -1 && this.currentBufferIndex < this.buffers.Count, "The currentBufferIndex is outside of the valid range.");

                DataBuffer currentBuffer = this.currentBufferIndex == -1 ? null : this.buffers[this.currentBufferIndex];

                // Here we intentionally leave some memory unused (smaller than MinReadBufferSize)
                // in order to issue big enough read requests. This is a perf optimization.
                if (currentBuffer != null && currentBuffer.FreeBytes < DataBuffer.MinReadBufferSize)
                {
                    currentBuffer = null;
                }

                if (currentBuffer == null)
                {
                    currentBuffer = this.AddNewBuffer();
                }

#if PORTABLELIB
                yield return inputStream.ReadAsync(currentBuffer.Buffer, currentBuffer.OffsetToWriteTo, currentBuffer.FreeBytes)
                    .ContinueWith(t =>
                    {
                        try
                        {
                            int bytesRead = t.Result;
                            if (bytesRead == 0)
                            {
                                this.inputStream = null;
                            }
                            else
                            {
                                currentBuffer.MarkBytesAsWritten(bytesRead);
                            }
                        }
                        catch (Exception exception)
                        {
                            if (!ExceptionUtils.IsCatchableExceptionType(exception))
                            {
                                throw;
                            }

                            this.inputStream = null;
                            throw;
                        }
                    });

#else
                yield return Task.Factory.FromAsync(
                    (asyncCallback, asyncState) => this.inputStream.BeginRead(
                        currentBuffer.Buffer,
                        currentBuffer.OffsetToWriteTo,
                        currentBuffer.FreeBytes,
                        asyncCallback,
                        asyncState),
                    (asyncResult) =>
                    {
                        try
                        {
                            int bytesRead = this.inputStream.EndRead(asyncResult);
                            if (bytesRead == 0)
                            {
                                this.inputStream = null;
                            }
                            else
                            {
                                currentBuffer.MarkBytesAsWritten(bytesRead);
                            }
                        }
                        catch (Exception exception)
                        {
                            if (!ExceptionUtils.IsCatchableExceptionType(exception))
                            {
                                throw;
                            }

                            this.inputStream = null;
                            throw;
                        }
                    },
                    null);
#endif
            }
        }

        /// <summary>
        /// Adds a new buffer to the list and makes it the current buffer.
        /// </summary>
        /// <returns>The newly added buffer.</returns>
        private DataBuffer AddNewBuffer()
        {
            Debug.Assert(this.currentBufferIndex == this.buffers.Count - 1, "We should only be adding new buffer if we're at the last buffer.");
            DataBuffer buffer = new DataBuffer();
            this.buffers.Add(buffer);
            this.currentBufferIndex = this.buffers.Count - 1;
            return buffer;
        }

        /// <summary>
        /// Class to wrap a byte buffer used to store portion of the buffered data.
        /// </summary>
        private sealed class DataBuffer
        {
            /// <summary>
            /// The minimum size to ask for when reading from underlying stream.
            /// </summary>
            internal const int MinReadBufferSize = 1024;

            /// <summary>
            /// The size of a buffer to allocate - use 64KB to be aligned which makes it likely that the underlying levels
            /// will be able to process the request in one go.
            /// </summary>
            private const int BufferSize = 64 * 1024;

            /// <summary>
            /// The byte buffer which stored the data.
            /// </summary>
            private readonly byte[] buffer;

            /// <summary>
            /// Constructor - creates a new buffer;
            /// </summary>
            public DataBuffer()
            {
                Debug.Assert(BufferSize >= MinReadBufferSize, "BufferSize >= MinReadBufferSize");
                this.buffer = new byte[BufferSize];
                this.StoredCount = 0;
            }

            /// <summary>
            /// The byte buffer.
            /// </summary>
            public byte[] Buffer
            {
                get { return this.buffer; }
            }

            /// <summary>
            /// The offset into the buffer to which more data can be written.
            /// </summary>
            public int OffsetToWriteTo
            {
                get { return this.StoredCount; }
            }

            /// <summary>
            /// The number of bytes stored in the buffer.
            /// </summary>
            public int StoredCount { get; private set; }

            /// <summary>
            /// The number of bytes not yet used in the buffer.
            /// </summary>
            public int FreeBytes
            {
                get { return this.buffer.Length - this.StoredCount; }
            }

            /// <summary>
            /// Marks specified count of bytes as written starting at the OffsetToWriteTo.
            /// </summary>
            /// <param name="count">The number of bytes to mark as written.</param>
            public void MarkBytesAsWritten(int count)
            {
                Debug.Assert(this.StoredCount + count <= this.buffer.Length, "Trying to write more bytes than available.");
                this.StoredCount += count;
            }
        }
    }
#endif
}

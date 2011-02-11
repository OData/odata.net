//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Data.OData
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    #endregion Namespaces

    /// <summary>
    /// Write-only stream which buffers all synchronous write operations until FlushAsync is called.
    /// </summary>
    internal sealed class AsyncBufferedStream : Stream
    {
        /// <summary>
        /// True if calls to IDisposable.Dispose are to be ignored; false otherwise.
        /// This is needed in order to be able to create and dispose multipe text writers on top
        /// of this stream. Use DisposeExplicitly to force disposing this stream.
        /// </summary>
        private readonly bool ignoreDispose;

        /// <summary>
        /// The stream being wrapped.
        /// </summary>
        private Stream innerStream;

        /// <summary>
        /// Queue of buffers to write.
        /// </summary>
        private Queue<DataBuffer> bufferQueue;

        /// <summary>
        /// The last buffer in the bufferQueue. This is the buffer we're writing into.
        /// </summary>
        private DataBuffer bufferToAppendTo;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stream">The underlying async stream to wrap. Note that only asynchronous write operation will be invoked on this stream.</param>
        internal AsyncBufferedStream(Stream stream)
            : this(stream, false)
        {
            DebugUtils.CheckNoExternalCallers();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stream">The underlying async stream to wrap. Note that only asynchronous write operation will be invoked on this stream.</param>
        /// <param name="ignoreDispose">
        /// True if calls to IDisposable.Dispose should be ignored (e.g., because multiple writers are used sequentially on
        /// the same AsyncBufferedStream; they don't own the stream but when disposed would attempt to disposed the
        /// AsyncBufferedStream as well. False otherwise.
        /// </param>
        /// <remarks>
        /// If <paramref name="ignoreDispose"/> is set to true you can use DisposeExplicitly to force a dispose of the
        /// AsyncBufferedStream.
        /// </remarks>
        internal AsyncBufferedStream(Stream stream, bool ignoreDispose)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(stream != null, "stream != null");

            this.innerStream = stream;
            this.ignoreDispose = ignoreDispose;
            this.bufferQueue = new Queue<DataBuffer>();
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
        /// Flush the stream to the underlying storage.
        /// </summary>
        public override void Flush()
        {
            // no-op
            // This can be called from writers that are put on top of this stream when
            // they are closed/disposed
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
            Debug.Assert(false, "Should never get here.");
            throw new NotSupportedException();
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
        /// Writes to the stream.
        /// </summary>
        /// <param name="buffer">The buffer to get data from.</param>
        /// <param name="offset">The offset in the buffer to start from.</param>
        /// <param name="count">The number of bytes to write.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (count > 0)
            {
                if (this.bufferToAppendTo == null)
                {
                    this.QueueNewBuffer();
                }

                while (count > 0)
                {
                    int written = this.bufferToAppendTo.Write(buffer, offset, count);
                    if (written < count)
                    {
                        this.QueueNewBuffer();
                    }

                    count -= written;
                    offset += written;
                }
            }
        }

        /// <summary>
        /// Clears any internal buffers without writing them to the underlying stream.
        /// </summary>
        internal void Clear()
        {
            DebugUtils.CheckNoExternalCallers();

            this.bufferQueue.Clear();
            this.bufferToAppendTo = null;
        }

        /// <summary>
        /// Synchronous flush operation. This will flush all buffered bytes to the underlying stream through synchronous writes.
        /// </summary>
        internal void FlushSync()
        {
            DebugUtils.CheckNoExternalCallers();

            Queue<DataBuffer> buffers = this.PrepareFlushBuffers();
            if (buffers == null)
            {
                return;
            }

            while (buffers.Count > 0)
            {
                DataBuffer buffer = buffers.Dequeue();
                buffer.WriteToStream(this.innerStream);
            }
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronous flush operation. This will flush all buffered bytes to the underlying stream through asynchronous writes.
        /// </summary>
        /// <returns>The task representing the asynchronous flush operation.</returns>
        internal Task FlushAsync()
        {
            DebugUtils.CheckNoExternalCallers();

            Queue<DataBuffer> buffers = this.PrepareFlushBuffers();
            if (buffers == null)
            {
                return TaskUtils.CompletedTask;
            }

            // Note that this relies on lazy eval of the enumerator
            // NOTE: it is important to use 'AttachedToParent' here to ensure that all exceptions
            //       are bubbled up to the parent thread.
            return Task.Factory.Iterate(
                this.FlushBuffersAsync(buffers),
                null,
                Task.Factory.CancellationToken,
                Task.Factory.CreationOptions | TaskCreationOptions.AttachedToParent,
                Task.Factory.GetTargetScheduler());
        }
#endif

        /// <summary>
        /// Disposes this instance independently of the 'ignoreDisposable' setting.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA1816:CallGCSuppressFinalizeCorrectly", 
            Justification = "AsyncBufferedStream supports an extended dispose protocol where SuppressFinalize should be called here.")]
        internal void DisposeExplicitly()
        {
            DebugUtils.CheckNoExternalCallers();

            this.DisposeImplementation(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">True if called from Dispose; false if called from the finalizer.</param>
        [SuppressMessage("Microsoft.Usage", "CA2215:Dispose methods should call base class dispose", 
            Justification = "AsyncBufferedStream supports an extended dispose protocol; the base class's dispose is called in DisposeInternal.")]
        protected override void Dispose(bool disposing)
        {
            if (this.ignoreDispose)
            {
                return;
            }

            this.DisposeImplementation(disposing);
        }

        /// <summary>
        /// Actual code to dispose this instance; called from DisposeExplicitly and Dispose(bool disposing).
        /// </summary>
        /// <param name="disposing">True if called from Dispose; false if called from the finalizer.</param>
        private void DisposeImplementation(bool disposing)
        {
            if (disposing)
            {
                if (this.bufferQueue.Count > 0)
                {
                    throw new ODataException(Strings.AsyncBufferedStream_WriterDisposedWithoutFlush);
                }

                this.innerStream.Dispose();
            }

            this.innerStream = null;

            base.Dispose(disposing);
        }

        /// <summary>
        /// Queues a new buffer to the queue of buffers
        /// </summary>
        private void QueueNewBuffer()
        {
            this.bufferToAppendTo = new DataBuffer();
            this.bufferQueue.Enqueue(this.bufferToAppendTo);
        }

        /// <summary>
        /// Prepares all buffers for flushing and returns the queue of buffers to flush.
        /// </summary>
        /// <returns>The queue of buffer to flush.</returns>
        private Queue<DataBuffer> PrepareFlushBuffers()
        {
            if (this.bufferQueue.Count == 0)
            {
                return null;
            }

            this.bufferToAppendTo = null;

            // clear the buffer queue to leave the stream in a 'clean' state even if 
            // flushing fails
            Queue<DataBuffer> buffers = this.bufferQueue;
            this.bufferQueue = new Queue<DataBuffer>();
            return buffers;
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Returns enumeration of tasks to run to flush all pending buffers to the underlying stream.
        /// </summary>
        /// <param name="buffers">The queue of buffers that need to be flushed.</param>
        /// <returns>Enumeration of tasks to run to flush all buffers.</returns>
        /// <remarks>This method relies on lazy eval of the enumerator, never enumerate through it synchronously.</remarks>
        private IEnumerable<Task> FlushBuffersAsync(Queue<DataBuffer> buffers)
        {
            while (buffers.Count > 0)
            {
                DataBuffer buffer = buffers.Dequeue();
                yield return buffer.WriteToStreamAsync(this.innerStream);
            }
        }
#endif

        /// <summary>
        /// Class to wrap a byte buffer used to store portion of the buffered data.
        /// </summary>
        private class DataBuffer
        {
            /// <summary>
            /// The size of a buffer to allocate (80 KB is the limit for large object heap, so use 79 to be sure to avoid LOB)
            /// </summary>
            private const int BufferSize = 79 * 1024;

            /// <summary>
            /// The byte buffer used to store the data.
            /// </summary>
            private byte[] buffer;

            /// <summary>
            /// Number of bytes being stored.
            /// </summary>
            private int storedCount;

            /// <summary>
            /// Constructor - creates a new buffer
            /// </summary>
            public DataBuffer()
            {
                this.buffer = new byte[BufferSize];
                this.storedCount = 0;
            }

            /// <summary>
            /// Writes data into the buffer.
            /// </summary>
            /// <param name="data">The buffer containing the data to write.</param>
            /// <param name="index">The index to start at.</param>
            /// <param name="count">Number of bytes to write.</param>
            /// <returns>How many bytes were written.</returns>
            public int Write(byte[] data, int index, int count)
            {
                int countToCopy = count;
                if (countToCopy > (this.buffer.Length - this.storedCount))
                {
                    countToCopy = this.buffer.Length - this.storedCount;
                }

                if (countToCopy > 0)
                {
                    Array.Copy(data, index, this.buffer, this.storedCount, countToCopy);
                    this.storedCount += countToCopy;
                }

                return countToCopy;
            }

            /// <summary>
            /// Writes the buffer to the specified stream.
            /// </summary>
            /// <param name="stream">The stream to write the data into.</param>
            public void WriteToStream(Stream stream)
            {
                stream.Write(this.buffer, 0, this.storedCount);
            }

#if ODATALIB_ASYNC
            /// <summary>
            /// Creates a task which writes the buffer to the specified stream.
            /// </summary>
            /// <param name="stream">The stream to write the data into.</param>
            /// <returns>The task which represent the asynchronous write operation.</returns>
            public Task WriteToStreamAsync(Stream stream)
            {
                return Task.Factory.FromAsync(
                    (callback, state) => stream.BeginWrite(this.buffer, 0, this.storedCount, callback, state),
                    stream.EndWrite,
                    null);
            }
#endif
        }
    }
}

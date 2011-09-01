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

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.IO;
    #endregion Namespaces

    /// <summary>
    /// A stream handed to clients from ODataBatchOperationMessage.GetStreamAsync. This stream communicates status changes
    /// to the owning batch writer (via IODataBatchOperationListener) to properly flush buffered data
    /// and move the batch writer's state machine forward.
    /// </summary>
    internal sealed class ODataBatchOperationStream : Stream
    {
        /// <summary>Listener interface to be notified of operation changes.</summary>
        private readonly IODataBatchOperationListener listener;

        /// <summary>The underlying stream to write to.</summary>
        private Stream outputStream;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="outputStream">The underlying stream to write the message to.</param>
        /// <param name="listener">Listener interface to be notified of operation changes.</param>
        internal ODataBatchOperationStream(Stream outputStream, IODataBatchOperationListener listener)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(outputStream != null, "outputStream != null");
            Debug.Assert(listener != null, "listener != null");

            this.outputStream = outputStream;
            this.listener = listener;
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
                this.ValidateNotDisposed();
                return this.outputStream.Length; 
            }
        }

        /// <summary>
        /// Gets or sets the position in the stream, this stream doesn't support seeking, so position is also unsupported.
        /// </summary>
        public override long Position
        {
            get
            {
                this.ValidateNotDisposed();
                return this.outputStream.Position;
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Flush the stream to the underlying storage.
        /// </summary>
        public override void Flush()
        {
            this.ValidateNotDisposed();
            this.outputStream.Flush();
        }

        /// <summary>
        /// Writes to the stream.
        /// </summary>
        /// <param name="buffer">The buffer to get data from.</param>
        /// <param name="offset">The offset in the buffer to start from.</param>
        /// <param name="count">The number of bytes to write.</param>
        /// <param name="callback">The callback to be called when the asynchronous operation completes.</param>
        /// <param name="state">A custom state object to be associated with the asynchronous operation.</param>
        /// <returns>An <see cref="IAsyncResult"/> for the asynchronous writing of the buffer to the stream.</returns>
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            this.ValidateNotDisposed();
            return this.outputStream.BeginWrite(buffer, offset, count, callback, state);
        }

        /// <summary>
        /// Finish the asynchronous write operation.
        /// </summary>
        /// <param name="asyncResult">The <see cref="IAsyncResult"/> returned from BaginWrite.</param>
        public override void EndWrite(IAsyncResult asyncResult)
        {
            this.ValidateNotDisposed();
            this.outputStream.EndWrite(asyncResult);
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
        /// Seeks the stream. This operation is not supported by this stream.
        /// </summary>
        /// <param name="offset">The offset to seek to.</param>
        /// <param name="origin">The origin of the seek operation.</param>
        /// <returns>The new position in the stream.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Sets the length of the stream.
        /// </summary>
        /// <param name="value">The length in bytes to set.</param>
        public override void SetLength(long value)
        {
            this.ValidateNotDisposed();
            this.outputStream.SetLength(value);
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
            this.outputStream.Write(buffer, offset, count);
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">True if called from Dispose; false if called form the finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.ValidateNotDisposed();

                // Tell the listener that the stream is being disposed; we expect
                // that no asynchronous action is triggered by doing so.
                this.listener.BatchOperationContentStreamDisposed();

                // Don't dispose the underlying stream here since it is not owned by this instance!
                this.outputStream = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Validates that the stream was not already disposed.
        /// </summary>
        private void ValidateNotDisposed()
        {
            if (this.outputStream == null)
            {
                throw new ObjectDisposedException(null, Strings.ODataBatchOperationStream_Disposed);
            }
        }
    }
}

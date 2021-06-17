//---------------------------------------------------------------------
// <copyright file="ODataStream.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;
    #endregion Namespaces

    /// <summary>
    /// A stream handed to clients from ODataBatchOperationMessage.GetStream or ODataBatchOperationMessage.GetStreamAsync.
    /// or representing a stream value.
    /// This stream communicates status changes to an IODataStreamListener instance.
    /// </summary>
#if NETSTANDARD2_0
    internal abstract class ODataStream : Stream, IAsyncDisposable
#else
    internal abstract class ODataStream : Stream
#endif
    {
        /// <summary>Listener interface to be notified of operation changes.</summary>
        private IODataStreamListener listener;

        /// <summary>true if the Dispose has been called; false for asynchronous.</summary>
        private bool disposed = false;

        /// <summary>true if the stream was created for synchronous operation; false for asynchronous.</summary>
        private bool synchronous;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="listener">Listener interface to be notified of operation changes.</param>
        internal ODataStream(IODataStreamListener listener, bool synchronous = true)
        {
            Debug.Assert(listener != null, "listener != null");

            this.listener = listener;
            this.synchronous = synchronous;
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
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">True if called from Dispose; false if called form the finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed && disposing)
            {
                // Tell the listener that the stream is being disposed; we expect
                // that no asynchronous action is triggered by doing so.
                if (this.synchronous)
                {
                    this.listener?.StreamDisposed();
                }
                else
                {
                    this.listener?.StreamDisposedAsync().Wait();
                }

                this.listener = null;
            }

            this.disposed = true;
            base.Dispose(disposing);
        }

#if NETSTANDARD2_0
        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore()
                .ConfigureAwait(false);

            // Dispose unmanaged resources
            // Pass `false` to ensure functional equivalence with the synchronous dispose pattern
            this.Dispose(false);
        }

        /// <summary>
        /// Encapsulates the common asynchronous cleanup operations.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (!this.disposed && this.listener != null)
            {
                await this.listener.StreamDisposedAsync()
                    .ConfigureAwait(false);

                this.listener = null;
            }
        }
#endif

        /// <summary>
        /// Validates that the stream was not already disposed.
        /// </summary>
        protected void ValidateNotDisposed()
        {
            if (this.listener == null)
            {
                throw new ObjectDisposedException(null, Strings.ODataBatchOperationStream_Disposed);
            }
        }
    }
}

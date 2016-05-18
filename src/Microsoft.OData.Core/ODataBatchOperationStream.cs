//---------------------------------------------------------------------
// <copyright file="ODataBatchOperationStream.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.IO;
    #endregion Namespaces

    /// <summary>
    /// A stream handed to clients from ODataBatchOperationMessage.GetStream or ODataBatchOperationMessage.GetStreamAsync.
    /// This stream communicates status changes to an IODataBatchOperationListener instance.
    /// </summary>
    internal abstract class ODataBatchOperationStream : Stream
    {
        /// <summary>Listener interface to be notified of operation changes.</summary>
        private IODataBatchOperationListener listener;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="listener">Listener interface to be notified of operation changes.</param>
        internal ODataBatchOperationStream(IODataBatchOperationListener listener)
        {
            Debug.Assert(listener != null, "listener != null");

            this.listener = listener;
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
            if (disposing)
            {
                if (this.listener != null)
                {
                    // Tell the listener that the stream is being disposed; we expect
                    // that no asynchronous action is triggered by doing so.
                    this.listener.BatchOperationContentStreamDisposed();
                    this.listener = null;
                }
            }

            base.Dispose(disposing);
        }

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

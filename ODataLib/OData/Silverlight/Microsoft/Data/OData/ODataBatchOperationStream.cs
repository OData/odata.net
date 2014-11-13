//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.Data.OData
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
            DebugUtils.CheckNoExternalCallers();
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

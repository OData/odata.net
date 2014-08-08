//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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

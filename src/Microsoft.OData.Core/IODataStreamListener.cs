//---------------------------------------------------------------------
// <copyright file="IODataStreamListener.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System.Threading.Tasks;

    #endregion

    /// <summary>
    /// An interface that allows creators of a stream to listen for status changes
    /// of the operation stream.
    /// </summary>
    internal interface IODataStreamListener
    {
        /// <summary>
        /// This method notifies the implementer of this interface that the content stream for a batch operation has been requested.
        /// </summary>
        void StreamRequested();

        /// <summary>
        /// This method notifies the implementer of this interface that the content stream for a batch operation has been requested.
        /// </summary>
        /// <returns>
        /// A task representing any async operation that is running in reaction to the
        /// status change (or null if no such action is required).
        /// </returns>
        Task StreamRequestedAsync();

        /// <summary>
        /// This method notifies the implementer of this interface that the content stream of a batch operation has been disposed.
        /// </summary>
        void StreamDisposed();

        /// <summary>
        /// Asynchronously notifies the implementer of this interface that the content stream of an operation has been disposed.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task StreamDisposedAsync();
    }
}

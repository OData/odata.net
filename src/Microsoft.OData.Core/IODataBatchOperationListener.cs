//---------------------------------------------------------------------
// <copyright file="IODataBatchOperationListener.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
#if PORTABLELIB
    using System.Threading.Tasks;
#endif
    #endregion

    /// <summary>
    /// An interface that allows creators of a <see cref="ODataBatchOperationStream"/> to listen for status changes
    /// of the operation stream.
    /// </summary>
    internal interface IODataBatchOperationListener
    {
        /// <summary>
        /// This method notifies the implementer of this interface that the content stream for a batch operation has been requested.
        /// </summary>
        void BatchOperationContentStreamRequested();

#if PORTABLELIB
        /// <summary>
        /// This method notifies the implementer of this interface that the content stream for a batch operation has been requested.
        /// </summary>
        /// <returns>
        /// A task representing any async operation that is running in reaction to the
        /// status change (or null if no such action is required).
        /// </returns>
        Task BatchOperationContentStreamRequestedAsync();
#endif

        /// <summary>
        /// This method notifies the implementer of this interface that the content stream of a batch operation has been disposed.
        /// </summary>
        void BatchOperationContentStreamDisposed();
    }
}

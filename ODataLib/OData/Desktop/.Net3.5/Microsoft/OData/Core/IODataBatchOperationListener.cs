//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    #region Namespaces
#if ODATALIB_ASYNC
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

#if ODATALIB_ASYNC
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

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

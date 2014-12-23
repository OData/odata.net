//   OData .NET Libraries ver. 6.9
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

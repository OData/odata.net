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
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using Microsoft.Data.Edm;
    #endregion Namespaces

    /// <summary>
    /// Base class for OData collection readers that verifies a proper sequence of read calls on the reader and which support true async operations.
    /// </summary>
    internal abstract class ODataCollectionReaderCoreAsync : ODataCollectionReaderCore
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputContext">The input to read from.</param>
        /// <param name="expectedItemTypeReference">The expected type reference for the items in the collection.</param>
        /// <param name="listener">If not null, the reader will notify the implementer of the interface of relevant state changes in the reader.</param>
        protected ODataCollectionReaderCoreAsync(
            ODataInputContext inputContext,
            IEdmTypeReference expectedItemTypeReference,
            IODataReaderWriterListener listener)
            : base(inputContext, expectedItemTypeReference, listener)
        {
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Implementation of the collection reader logic when in state 'Start'.
        /// </summary>
        /// <returns>Task which returns true if more items can be read from the reader; otherwise false.</returns>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
        protected abstract Task<bool> ReadAtStartImplementationAsync();

        /// <summary>
        /// Implementation of the reader logic when in state 'CollectionStart'.
        /// </summary>
        /// <returns>Task which returns true if more nodes can be read from the reader; otherwise false.</returns>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
        protected abstract Task<bool> ReadAtCollectionStartImplementationAsync();

        /// <summary>
        /// Implementation of the reader logic when in state 'Value'.
        /// </summary>
        /// <returns>Task which returns true if more nodes can be read from the reader; otherwise false.</returns>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
        protected abstract Task<bool> ReadAtValueImplementationAsync();

        /// <summary>
        /// Implementation of the reader logic when in state 'CollectionEnd'.
        /// </summary>
        /// <returns>Task which should return false since no more nodes can be read from the reader after the collection ends.</returns>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
        protected abstract Task<bool> ReadAtCollectionEndImplementationAsync();

        /// <summary>
        /// Asynchronously reads the next <see cref="ODataItem"/> from the message payload.
        /// </summary>
        /// <returns>A task that when completed indicates whether more items were read.</returns>
        /// <remarks>The base class already implements this but only for fully synchronous readers, the implementation here
        /// allows fully asynchronous readers.</remarks>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
        protected override Task<bool> ReadAsynchronously()
        {
            switch (this.State)
            {
                case ODataCollectionReaderState.Start:
                    return this.ReadAtStartImplementationAsync();

                case ODataCollectionReaderState.CollectionStart:
                    return this.ReadAtCollectionStartImplementationAsync();

                case ODataCollectionReaderState.Value:
                    return this.ReadAtValueImplementationAsync();

                case ODataCollectionReaderState.CollectionEnd:
                    return this.ReadAtCollectionEndImplementationAsync();

                case ODataCollectionReaderState.Exception:    // fall through
                case ODataCollectionReaderState.Completed:
                    Debug.Assert(false, "This case should have been caught earlier.");
                    return TaskUtils.GetFaultedTask<bool>(new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataCollectionReaderCoreAsync_ReadAsynchronously)));

                default:
                    Debug.Assert(false, "Unsupported collection reader state " + this.State + " detected.");
                    return TaskUtils.GetFaultedTask<bool>(new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataCollectionReaderCoreAsync_ReadAsynchronously)));
            }
        }
#endif
    }
}

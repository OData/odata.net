//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// Base class for OData parameter readers that verifies a proper sequence of read calls on the reader with truly async operations.
    /// </summary>
    internal abstract class ODataParameterReaderCoreAsync : ODataParameterReaderCore
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputContext">The input to read from.</param>
        /// <param name="operation">The operation whose parameters are being read.</param>
        protected ODataParameterReaderCoreAsync(
            ODataInputContext inputContext,
            IEdmOperation operation)
            : base(inputContext, operation)
        {
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Implementation of the parameter reader logic when in state 'Start'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
        protected abstract Task<bool> ReadAtStartImplementationAsync();

        /// <summary>
        /// Implementation of the reader logic when in state Value, Entry, Feed or Collection state.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
        protected abstract Task<bool> ReadNextParameterImplementationAsync();

#if SUPPORT_ENTITY_PARAMETER
        /// <summary>
        /// Creates an <see cref="ODataReader"/> to read the entry value of type <paramref name="expectedEntityType"/>.
        /// </summary>
        /// <param name="expectedEntityType">Expected entity type to read.</param>
        /// <returns>An <see cref="ODataReader"/> to read the entry value of type <paramref name="expectedEntityType"/>.</returns>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
        protected abstract Task<ODataReader> CreateEntryReaderAsync(IEdmEntityType expectedEntityType);

        /// <summary>
        /// Cretes an <see cref="ODataReader"/> to read the feed value of type <paramref name="expectedEntityType"/>.
        /// </summary>
        /// <param name="expectedEntityType">Expected feed element type to read.</param>
        /// <returns>An <see cref="ODataReader"/> to read the feed value of type <paramref name="expectedEntityType"/>.</returns>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
        protected abstract Task<ODataReader> CreateFeedReaderAsync(IEdmEntityType expectedEntityType);
#endif

        /// <summary>
        /// Creates an <see cref="ODataCollectionReader"/> to read the collection with type <paramref name="expectedItemTypeReference"/>.
        /// </summary>
        /// <param name="expectedItemTypeReference">Expected item type reference of the collection to read.</param>
        /// <returns>An <see cref="ODataCollectionReader"/> to read the collection with type <paramref name="expectedItemTypeReference"/>.</returns>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
        protected abstract Task<ODataCollectionReader> CreateCollectionReaderAsync(IEdmTypeReference expectedItemTypeReference);

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
                case ODataParameterReaderState.Start:
#if DEBUG
                    return this.ReadAtStartImplementationAsync()
                        .FollowOnSuccessWith(t =>
                            {
                                Debug.Assert(
                                    this.State == ODataParameterReaderState.Value ||
#if SUPPORT_ENTITY_PARAMETER
                                    this.State == ODataParameterReaderState.Entry ||
                                    this.State == ODataParameterReaderState.Feed ||
#endif
                                    this.State == ODataParameterReaderState.Collection ||
                                    this.State == ODataParameterReaderState.Completed,
                                    "ReadAtStartImplementationAsync should transition the state to ODataParameterReaderState.Value, ODataParameterReaderState.Entry, ODataParameterReaderState.Feed, ODataParameterReaderState.Collection or ODataParameterReaderState.Completed. The current state is: " + this.State);
                                return t.Result;
                            });
#else
                    return this.ReadAtStartImplementationAsync();
#endif

                case ODataParameterReaderState.Value:   // fall through
#if SUPPORT_ENTITY_PARAMETER
                case ODataParameterReaderState.Entry:
                case ODataParameterReaderState.Feed:
#endif
                case ODataParameterReaderState.Collection:
                    this.OnParameterCompleted();
#if DEBUG
                    return this.ReadNextParameterImplementationAsync()
                        .FollowOnSuccessWith(t =>
                            {
                                Debug.Assert(
                                    this.State == ODataParameterReaderState.Value ||
#if SUPPORT_ENTITY_PARAMETER
                                    this.State == ODataParameterReaderState.Entry ||
                                    this.State == ODataParameterReaderState.Feed ||
#endif
                                    this.State == ODataParameterReaderState.Collection ||
                                    this.State == ODataParameterReaderState.Completed,
                                    "ReadNextParameterImplementationAsync should transition the state to ODataParameterReaderState.Value, ODataParameterReaderState.Entry, ODataParameterReaderState.Feed, ODataParameterReaderState.Collection or ODataParameterReaderState.Completed. The current state is: " + this.State);
                                return t.Result;
                            });
#else
                    return this.ReadNextParameterImplementationAsync();
#endif

                case ODataParameterReaderState.Exception:    // fall through
                case ODataParameterReaderState.Completed:
                    Debug.Assert(false, "This case should have been caught earlier.");
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataParameterReaderCoreAsync_ReadAsynchronously));

                default:
                    Debug.Assert(false, "Unsupported parameter reader state " + this.State + " detected.");
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataParameterReaderCoreAsync_ReadAsynchronously));
            }
        }
#endif
    }
}

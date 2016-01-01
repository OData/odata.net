//---------------------------------------------------------------------
// <copyright file="ODataReaderCoreAsync.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    #endregion Namespaces

    /// <summary>
    /// Base class for OData readers that verifies a proper sequence of read calls on the reader with true async operations.
    /// </summary>
    internal abstract class ODataReaderCoreAsync : ODataReaderCore
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputContext">The input to read the payload from.</param>
        /// <param name="readingFeed">true if the reader is created for reading a feed; false when it is created for reading an entry.</param>
        /// <param name="readingDelta">true if the reader is created for reading expanded navigation property in delta response; false otherwise.</param>
        /// <param name="listener">If not null, the reader will notify the implementer of the interface of relevant state changes in the reader.</param>
        protected ODataReaderCoreAsync(
            ODataInputContext inputContext, 
            bool readingFeed,
            bool readingDelta,
            IODataReaderWriterListener listener)
            : base(inputContext, readingFeed, readingDelta, listener)
        {
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Implementation of the reader logic when in state 'Start'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
        protected abstract Task<bool> ReadAtStartImplementationAsync();

        /// <summary>
        /// Implementation of the reader logic when in state 'FeedStart'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
        protected abstract Task<bool> ReadAtFeedStartImplementationAsync();

        /// <summary>
        /// Implementation of the reader logic when in state 'FeedEnd'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
        protected abstract Task<bool> ReadAtFeedEndImplementationAsync();

        /// <summary>
        /// Implementation of the reader logic when in state 'EntryStart'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
        protected abstract Task<bool> ReadAtEntryStartImplementationAsync();

        /// <summary>
        /// Implementation of the reader logic when in state 'EntryEnd'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
        protected abstract Task<bool> ReadAtEntryEndImplementationAsync();

        /// <summary>
        /// Implementation of the reader logic when in state 'NavigationLinkStart'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
        protected abstract Task<bool> ReadAtNavigationLinkStartImplementationAsync();

        /// <summary>
        /// Implementation of the reader logic when in state 'NavigationLinkEnd'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
        protected abstract Task<bool> ReadAtNavigationLinkEndImplementationAsync();

        /// <summary>
        /// Implementation of the reader logic when in state 'EntityReferenceLink'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
        protected abstract Task<bool> ReadAtEntityReferenceLinkAsync();

        /// <summary>
        /// Asynchronously reads the next <see cref="ODataItem"/> from the message payload.
        /// </summary>
        /// <returns>A task that when completed indicates whether more items were read.</returns>
        /// <remarks>The base class already implements this but only for fully synchronous readers, the implementation here
        /// allows fully asynchronous readers.</remarks>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
        protected override Task<bool> ReadAsynchronously()
        {
            Task<bool> result;
            switch (this.State)
            {
                case ODataReaderState.Start:
                    result = this.ReadAtStartImplementationAsync();
                    break;

                case ODataReaderState.FeedStart:
                    result = this.ReadAtFeedStartImplementationAsync();
                    break;

                case ODataReaderState.FeedEnd:
                    result = this.ReadAtFeedEndImplementationAsync();
                    break;

                case ODataReaderState.EntryStart:
                    result = TaskUtils.GetTaskForSynchronousOperation(() => this.IncreaseEntryDepth())
                        .FollowOnSuccessWithTask(t => this.ReadAtEntryStartImplementationAsync());
                    break;

                case ODataReaderState.EntryEnd:
                    result = TaskUtils.GetTaskForSynchronousOperation(() => this.DecreaseEntryDepth())
                        .FollowOnSuccessWithTask(t => this.ReadAtEntryEndImplementationAsync());
                    break;

                case ODataReaderState.NavigationLinkStart:
                    result = this.ReadAtNavigationLinkStartImplementationAsync();
                    break;

                case ODataReaderState.NavigationLinkEnd:
                    result = this.ReadAtNavigationLinkEndImplementationAsync();
                    break;

                case ODataReaderState.EntityReferenceLink:
                    result = this.ReadAtEntityReferenceLinkAsync();
                    break;

                case ODataReaderState.Exception:    // fall through
                case ODataReaderState.Completed:
                    result = TaskUtils.GetFaultedTask<bool>(new ODataException(Strings.ODataReaderCore_NoReadCallsAllowed(this.State)));
                    break;

                default:
                    Debug.Assert(false, "Unsupported reader state " + this.State + " detected.");
                    result = TaskUtils.GetFaultedTask<bool>(new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataReaderCoreAsync_ReadAsynchronously)));
                    break;
            }

            return result.FollowOnSuccessWith(t =>
                {
                    if ((this.State == ODataReaderState.EntryStart || this.State == ODataReaderState.EntryEnd) && this.Item != null)
                    {
                        ReaderValidationUtils.ValidateEntry(this.CurrentEntry);
                    }

                    return t.Result;
                });
        }
#endif
    }
}
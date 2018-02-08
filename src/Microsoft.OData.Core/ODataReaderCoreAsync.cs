//---------------------------------------------------------------------
// <copyright file="ODataReaderCoreAsync.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
#if PORTABLELIB
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
        /// <param name="readingResourceSet">true if the reader is created for reading a resource set; false when it is created for reading a resource.</param>
        /// <param name="readingDelta">true if the reader is created for reading expanded navigation property in delta response; false otherwise.</param>
        /// <param name="listener">If not null, the reader will notify the implementer of the interface of relevant state changes in the reader.</param>
        protected ODataReaderCoreAsync(
            ODataInputContext inputContext,
            bool readingResourceSet,
            bool readingDelta,
            IODataReaderWriterListener listener)
            : base(inputContext, readingResourceSet, readingDelta, listener)
        {
        }

#if PORTABLELIB
        /// <summary>
        /// Implementation of the reader logic when in state 'Start'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        protected abstract Task<bool> ReadAtStartImplementationAsync();

        /// <summary>
        /// Implementation of the reader logic when in state 'ResourceSetStart'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        protected abstract Task<bool> ReadAtResourceSetStartImplementationAsync();

        /// <summary>
        /// Implementation of the reader logic when in state 'ResourceSetEnd'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        protected abstract Task<bool> ReadAtResourceSetEndImplementationAsync();

        /// <summary>
        /// Implementation of the reader logic when in state 'EntryStart'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        protected abstract Task<bool> ReadAtResourceStartImplementationAsync();

        /// <summary>
        /// Implementation of the reader logic when in state 'EntryEnd'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        protected abstract Task<bool> ReadAtResourceEndImplementationAsync();

        /// <summary>
        /// Implementation of the reader logic when in state 'DeletedResourceEnd'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        protected abstract Task<bool> ReadAtDeletedResourceEndImplementationAsync();

        /// <summary>
        /// Implementation of the reader logic when in state 'Primitive'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        protected abstract Task<bool> ReadAtPrimitiveImplementationAsync();

        /// <summary>
        /// Implementation of the reader logic when in state 'NestedResourceInfoStart'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        protected abstract Task<bool> ReadAtNestedResourceInfoStartImplementationAsync();

        /// <summary>
        /// Implementation of the reader logic when in state 'NestedResourceInfoEnd'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        protected abstract Task<bool> ReadAtNestedResourceInfoEndImplementationAsync();

        /// <summary>
        /// Implementation of the reader logic when in state 'EntityReferenceLink'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        protected abstract Task<bool> ReadAtEntityReferenceLinkAsync();

        /// <summary>
        /// Implementation of the reader logic when in state 'DeltaResourceSetStart'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        protected virtual Task<bool> ReadAtDeltaResourceSetStartImplementationAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadAtDeltaResourceSetStartImplementation);
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'DeltaResourceSetEnd'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        protected virtual Task<bool> ReadAtDeltaResourceSetEndImplementationAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadAtDeltaResourceSetEndImplementation);
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'DeletedResourceStart'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        protected virtual Task<bool> ReadAtDeletedResourceStartImplementationAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadAtDeletedResourceStartImplementation);
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'DeletedResourceEnd'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        protected virtual Task<bool> ReadDeletedResourceEndImplementationAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadAtDeletedResourceEndImplementation);
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'DeltaLink'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        protected virtual Task<bool> ReadAtDeltaLinkImplementationAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadAtDeltaLinkImplementation);
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'DeltaDeletedLink'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        protected virtual Task<bool> ReadAtDeltaDeletedLinkImplementationAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadAtDeltaDeletedLinkImplementation);
        }

        /// <summary>
        /// Asynchronously reads the next <see cref="ODataItem"/> from the message payload.
        /// </summary>
        /// <returns>A task that when completed indicates whether more items were read.</returns>
        /// <remarks>The base class already implements this but only for fully synchronous readers, the implementation here
        /// allows fully asynchronous readers.</remarks>
        protected override Task<bool> ReadAsynchronously()
        {
            Task<bool> result;
            switch (this.State)
            {
                case ODataReaderState.Start:
                    result = this.ReadAtStartImplementationAsync();
                    break;

                case ODataReaderState.ResourceSetStart:
                    result = this.ReadAtResourceSetStartImplementationAsync();
                    break;

                case ODataReaderState.ResourceSetEnd:
                    result = this.ReadAtResourceSetEndImplementationAsync();
                    break;

                case ODataReaderState.ResourceStart:
                    result = TaskUtils.GetTaskForSynchronousOperation(() => this.IncreaseResourceDepth())
                        .FollowOnSuccessWithTask(t => this.ReadAtResourceStartImplementationAsync());
                    break;

                case ODataReaderState.ResourceEnd:
                    result = TaskUtils.GetTaskForSynchronousOperation(() => this.DecreaseResourceDepth())
                        .FollowOnSuccessWithTask(t => this.ReadAtResourceEndImplementationAsync());
                    break;

                case ODataReaderState.Primitive:
                    result = this.ReadAtPrimitiveImplementationAsync();
                    break;

                case ODataReaderState.NestedResourceInfoStart:
                    result = this.ReadAtNestedResourceInfoStartImplementationAsync();
                    break;

                case ODataReaderState.NestedResourceInfoEnd:
                    result = this.ReadAtNestedResourceInfoEndImplementationAsync();
                    break;

                case ODataReaderState.EntityReferenceLink:
                    result = this.ReadAtEntityReferenceLinkAsync();
                    break;

                case ODataReaderState.DeltaResourceSetStart:
                    result = this.ReadAtDeltaResourceSetStartImplementationAsync();
                    break;

                case ODataReaderState.DeltaResourceSetEnd:
                    result = this.ReadAtDeltaResourceSetEndImplementationAsync();
                    break;

                case ODataReaderState.DeletedResourceStart:
                    result = TaskUtils.GetTaskForSynchronousOperation(() => this.IncreaseResourceDepth())
                        .FollowOnSuccessWithTask(t => this.ReadAtDeletedResourceStartImplementationAsync());
                    break;

                case ODataReaderState.DeletedResourceEnd:
                    result = TaskUtils.GetTaskForSynchronousOperation(() => this.DecreaseResourceDepth())
                        .FollowOnSuccessWithTask(t => this.ReadAtDeletedResourceEndImplementationAsync());
                    break;

                case ODataReaderState.DeltaLink:
                    result = this.ReadAtDeltaLinkImplementationAsync();
                    break;

                case ODataReaderState.DeltaDeletedLink:
                    result = this.ReadAtDeltaDeletedLinkImplementationAsync();
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

            return result.FollowOnSuccessWith(t => t.Result);
        }
#endif
    }
}
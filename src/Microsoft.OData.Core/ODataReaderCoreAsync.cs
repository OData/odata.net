//---------------------------------------------------------------------
// <copyright file="ODataReaderCoreAsync.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces

    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;

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
        protected virtual Task<bool> ReadAtPrimitiveImplementationAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadAtPrimitiveImplementation);
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'PropertyInfo'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        protected virtual Task<bool> ReadAtNestedPropertyInfoImplementationAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadAtNestedPropertyInfoImplementation);
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'Stream'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        protected virtual Task<bool> ReadAtStreamImplementationAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadAtStreamImplementation);
        }

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
        protected override async Task<bool> ReadAsynchronously()
        {
            bool result;

            switch (this.State)
            {
                case ODataReaderState.Start:
                    result = await this.ReadAtStartImplementationAsync()
                        .ConfigureAwait(false);
                    break;

                case ODataReaderState.ResourceSetStart:
                    result = await this.ReadAtResourceSetStartImplementationAsync()
                        .ConfigureAwait(false);
                    break;

                case ODataReaderState.ResourceSetEnd:
                    result = await this.ReadAtResourceSetEndImplementationAsync()
                        .ConfigureAwait(false);
                    break;

                case ODataReaderState.ResourceStart:
                    this.IncreaseResourceDepth();
                    result = await this.ReadAtResourceStartImplementationAsync()
                        .ConfigureAwait(false);
                    break;

                case ODataReaderState.ResourceEnd:
                    this.DecreaseResourceDepth();
                    result = await this.ReadAtResourceEndImplementationAsync()
                        .ConfigureAwait(false);
                    break;

                case ODataReaderState.Primitive:
                    result = await this.ReadAtPrimitiveImplementationAsync()
                        .ConfigureAwait(false);
                    break;

                case ODataReaderState.Stream:
                    result = await this.ReadAtStreamImplementationAsync()
                        .ConfigureAwait(false);
                    break;

                case ODataReaderState.NestedProperty:
                    result = await this.ReadAtNestedPropertyInfoImplementationAsync()
                        .ConfigureAwait(false);
                    break;

                case ODataReaderState.NestedResourceInfoStart:
                    result = await this.ReadAtNestedResourceInfoStartImplementationAsync()
                        .ConfigureAwait(false);
                    break;

                case ODataReaderState.NestedResourceInfoEnd:
                    result = await this.ReadAtNestedResourceInfoEndImplementationAsync()
                        .ConfigureAwait(false);
                    break;

                case ODataReaderState.EntityReferenceLink:
                    result = await this.ReadAtEntityReferenceLinkAsync()
                        .ConfigureAwait(false);
                    break;

                case ODataReaderState.DeltaResourceSetStart:
                    result = await this.ReadAtDeltaResourceSetStartImplementationAsync()
                        .ConfigureAwait(false);
                    break;

                case ODataReaderState.DeltaResourceSetEnd:
                    result = await this.ReadAtDeltaResourceSetEndImplementationAsync()
                        .ConfigureAwait(false);
                    break;

                case ODataReaderState.DeletedResourceStart:
                    this.IncreaseResourceDepth();
                    result = await this.ReadAtDeletedResourceStartImplementationAsync()
                        .ConfigureAwait(false);
                    break;

                case ODataReaderState.DeletedResourceEnd:
                    this.DecreaseResourceDepth();
                    result = await this.ReadAtDeletedResourceEndImplementationAsync()
                        .ConfigureAwait(false);
                    break;

                case ODataReaderState.DeltaLink:
                    result = await this.ReadAtDeltaLinkImplementationAsync()
                        .ConfigureAwait(false);
                    break;

                case ODataReaderState.DeltaDeletedLink:
                    result = await this.ReadAtDeltaDeletedLinkImplementationAsync()
                        .ConfigureAwait(false);
                    break;

                case ODataReaderState.Exception:    // fall through
                case ODataReaderState.Completed:
                    throw new ODataException(Strings.ODataReaderCore_NoReadCallsAllowed(this.State));

                default:
                    Debug.Assert(false, "Unsupported reader state " + this.State + " detected.");
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataReaderCoreAsync_ReadAsynchronously));
            }

            return result;
        }

        /// <summary>
        /// Asynchronously creates a <see cref="Stream"/> for reading a stream property
        /// when reader in state <see cref="ODataReaderState.Stream"/>.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains a <see cref="Stream"/> for reading the stream property.
        /// </returns>
        protected virtual Task<Stream> CreateReadStreamImplementationAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Asynchronously creates a <see cref="TextReader"/> for reading a string property
        /// when reader in state <see cref="ODataReaderState.Stream"/>.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains a <see cref="TextReader"/> for reading the string property..
        /// </returns>
        protected virtual Task<TextReader> CreateTextReaderImplementationAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>Asynchronously creates a <see cref="Stream"/> for reading an inline stream property.</summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains a <see cref="Stream"/> for reading the stream property.
        /// </returns>
        public override async Task<Stream> CreateReadStreamAsync()
        {
            if (this.State != ODataReaderState.Stream)
            {
                throw new ODataException(Strings.ODataReaderCore_CreateReadStreamCalledInInvalidState);
            }

            StreamScope scope = this.CurrentScope as StreamScope;
            Debug.Assert(scope != null, "ODataReaderState.Stream when Scope is not a StreamScope");
            if (scope.StreamingState != StreamingState.None)
            {
                throw new ODataException(Strings.ODataReaderCore_CreateReadStreamCalledInInvalidState);
            }

            scope.StreamingState = StreamingState.Streaming;
            return new ODataNotificationStream(
                underlyingStream: await this.InterceptExceptionAsync(thisParam => thisParam.CreateReadStreamImplementationAsync()).ConfigureAwait(false),
                listener: this,
                synchronous: false);
        }

        /// <summary>Asynchronously creates a <see cref="TextReader"/> for reading an inline string property.</summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains a <see cref="TextReader"/> for reading the string property.
        /// </returns>
        public override async Task<TextReader> CreateTextReaderAsync()
        {
            if (this.State != ODataReaderState.Stream)
            {
                throw new ODataException(Strings.ODataReaderCore_CreateTextReaderCalledInInvalidState);
            }

            StreamScope scope = this.CurrentScope as StreamScope;
            Debug.Assert(scope != null, "ODataReaderState.Stream when Scope is not a StreamScope");
            if (scope.StreamingState != StreamingState.None)
            {
                throw new ODataException(Strings.ODataReaderCore_CreateReadStreamCalledInInvalidState);
            }

            scope.StreamingState = StreamingState.Streaming;
            return new ODataNotificationReader(
                textReader: await this.InterceptExceptionAsync(thisParam => thisParam.CreateTextReaderImplementationAsync()).ConfigureAwait(false),
                listener: this,
                synchronous: false);
        }

        /// <summary>
        /// Catch any exception thrown by the action passed in; in the exception case move the reader into
        /// state <see cref="ODataReaderState.Exception"/> and then rethrow the exception.
        /// </summary>
        /// <typeparam name="TResult">The action return type.</typeparam>
        /// <param name="action">The action to execute.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains the result of executing the <paramref name="action"/>.
        /// </returns>
        private async Task<TResult> InterceptExceptionAsync<TResult>(Func<ODataReaderCoreAsync, Task<TResult>> action)
        {
            try
            {
                return await action(this).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                if (ExceptionUtils.IsCatchableExceptionType(e))
                {
                    this.EnterScope(new Scope(ODataReaderState.Exception, null, null));
                }

                throw;
            }
        }
    }
}

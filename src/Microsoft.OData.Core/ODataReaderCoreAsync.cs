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
        protected abstract ValueTask<bool> ReadAtStartImplementationAsync();

        /// <summary>
        /// Implementation of the reader logic when in state 'ResourceSetStart'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        protected abstract ValueTask<bool> ReadAtResourceSetStartImplementationAsync();

        /// <summary>
        /// Implementation of the reader logic when in state 'ResourceSetEnd'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        protected abstract ValueTask<bool> ReadAtResourceSetEndImplementationAsync();

        /// <summary>
        /// Implementation of the reader logic when in state 'EntryStart'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        protected abstract ValueTask<bool> ReadAtResourceStartImplementationAsync();

        /// <summary>
        /// Implementation of the reader logic when in state 'EntryEnd'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        protected abstract ValueTask<bool> ReadAtResourceEndImplementationAsync();

        /// <summary>
        /// Implementation of the reader logic when in state 'DeletedResourceEnd'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        protected abstract ValueTask<bool> ReadAtDeletedResourceEndImplementationAsync();

        /// <summary>
        /// Implementation of the reader logic when in state 'Primitive'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        protected virtual ValueTask<bool> ReadAtPrimitiveImplementationAsync()
        {
            return ValueTask.FromResult(this.ReadAtPrimitiveImplementation());
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'PropertyInfo'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        protected virtual ValueTask<bool> ReadAtNestedPropertyInfoImplementationAsync()
        {
            return ValueTask.FromResult(this.ReadAtNestedPropertyInfoImplementation());
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'Stream'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        protected virtual ValueTask<bool> ReadAtStreamImplementationAsync()
        {
            return ValueTask.FromResult(this.ReadAtStreamImplementation());
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'NestedResourceInfoStart'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        protected abstract ValueTask<bool> ReadAtNestedResourceInfoStartImplementationAsync();

        /// <summary>
        /// Implementation of the reader logic when in state 'NestedResourceInfoEnd'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        protected abstract ValueTask<bool> ReadAtNestedResourceInfoEndImplementationAsync();

        /// <summary>
        /// Implementation of the reader logic when in state 'EntityReferenceLink'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        protected abstract ValueTask<bool> ReadAtEntityReferenceLinkAsync();

        /// <summary>
        /// Implementation of the reader logic when in state 'DeltaResourceSetStart'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        protected virtual ValueTask<bool> ReadAtDeltaResourceSetStartImplementationAsync()
        {
            return ValueTask.FromResult(this.ReadAtDeltaResourceSetStartImplementation());
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'DeltaResourceSetEnd'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        protected virtual ValueTask<bool> ReadAtDeltaResourceSetEndImplementationAsync()
        {
            return ValueTask.FromResult(this.ReadAtDeltaResourceSetEndImplementation());
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'DeletedResourceStart'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        protected virtual ValueTask<bool> ReadAtDeletedResourceStartImplementationAsync()
        {
            return ValueTask.FromResult(this.ReadAtDeletedResourceStartImplementation());
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'DeletedResourceEnd'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        protected virtual ValueTask<bool> ReadDeletedResourceEndImplementationAsync()
        {
            return ValueTask.FromResult(this.ReadAtDeletedResourceEndImplementation());
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'DeltaLink'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        protected virtual ValueTask<bool> ReadAtDeltaLinkImplementationAsync()
        {
            return ValueTask.FromResult(this.ReadAtDeltaLinkImplementation());
        }

        /// <summary>
        /// Implementation of the reader logic when in state 'DeltaDeletedLink'.
        /// </summary>
        /// <returns>A task which returns true if more items can be read from the reader; otherwise false.</returns>
        protected virtual ValueTask<bool> ReadAtDeltaDeletedLinkImplementationAsync()
        {
            return ValueTask.FromResult(this.ReadAtDeltaDeletedLinkImplementation());
        }

        /// <summary>
        /// Asynchronously reads the next <see cref="ODataItem"/> from the message payload.
        /// </summary>
        /// <returns>A task that when completed indicates whether more items were read.</returns>
        /// <remarks>The base class already implements this but only for fully synchronous readers, the implementation here
        /// allows fully asynchronous readers.</remarks>
        protected override ValueTask<bool> ReadAsynchronously()
        {
            switch (this.State)
            {
                case ODataReaderState.Start:
                    return this.ReadAtStartImplementationAsync();

                case ODataReaderState.ResourceSetStart:
                    return this.ReadAtResourceSetStartImplementationAsync();

                case ODataReaderState.ResourceSetEnd:
                    return this.ReadAtResourceSetEndImplementationAsync();

                case ODataReaderState.ResourceStart:
                    this.IncreaseResourceDepth();
                    return this.ReadAtResourceStartImplementationAsync();

                case ODataReaderState.ResourceEnd:
                    this.DecreaseResourceDepth();
                    return this.ReadAtResourceEndImplementationAsync();

                case ODataReaderState.Primitive:
                    return this.ReadAtPrimitiveImplementationAsync();

                case ODataReaderState.Stream:
                    return this.ReadAtStreamImplementationAsync();

                case ODataReaderState.NestedProperty:
                    return this.ReadAtNestedPropertyInfoImplementationAsync();

                case ODataReaderState.NestedResourceInfoStart:
                    return this.ReadAtNestedResourceInfoStartImplementationAsync();

                case ODataReaderState.NestedResourceInfoEnd:
                    return this.ReadAtNestedResourceInfoEndImplementationAsync();

                case ODataReaderState.EntityReferenceLink:
                    return this.ReadAtEntityReferenceLinkAsync();

                case ODataReaderState.DeltaResourceSetStart:
                    return this.ReadAtDeltaResourceSetStartImplementationAsync();

                case ODataReaderState.DeltaResourceSetEnd:
                    return this.ReadAtDeltaResourceSetEndImplementationAsync();

                case ODataReaderState.DeletedResourceStart:
                    this.IncreaseResourceDepth();
                    return this.ReadAtDeletedResourceStartImplementationAsync();

                case ODataReaderState.DeletedResourceEnd:
                    this.DecreaseResourceDepth();
                    return this.ReadAtDeletedResourceEndImplementationAsync();

                case ODataReaderState.DeltaLink:
                    return this.ReadAtDeltaLinkImplementationAsync();

                case ODataReaderState.DeltaDeletedLink:
                    return this.ReadAtDeltaDeletedLinkImplementationAsync();

                case ODataReaderState.Exception:    // fall through
                case ODataReaderState.Completed:
                    return ValueTask.FromException<bool>(new ODataException(Strings.ODataReaderCore_NoReadCallsAllowed(this.State)));

                default:
                    Debug.Assert(false, "Unsupported reader state " + this.State + " detected.");
                    return ValueTask.FromException<bool>(
                        new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataReaderCoreAsync_ReadAsynchronously)));
            }
        }

        /// <summary>
        /// Asynchronously creates a <see cref="Stream"/> for reading a stream property
        /// when reader in state <see cref="ODataReaderState.Stream"/>.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains a <see cref="Stream"/> for reading the stream property.
        /// </returns>
        protected virtual ValueTask<Stream> CreateReadStreamImplementationAsync()
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
        protected virtual ValueTask<TextReader> CreateTextReaderImplementationAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>Asynchronously creates a <see cref="Stream"/> for reading an inline stream property.</summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The value of the TResult parameter contains a <see cref="Stream"/> for reading the stream property.
        /// </returns>
        public override async ValueTask<Stream> CreateReadStreamAsync()
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
        public override async ValueTask<TextReader> CreateTextReaderAsync()
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
        private async ValueTask<TResult> InterceptExceptionAsync<TResult>(Func<ODataReaderCoreAsync, ValueTask<TResult>> action)
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

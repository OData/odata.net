//   Copyright 2011 Microsoft Corporation
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

namespace System.Data.OData
{
    #region Namespaces.
    using System.Collections.Generic;
    using System.Data.Services.Providers;
    using System.Diagnostics;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    #endregion Namespaces.

    /// <summary>
    /// Base class for OData collection writers that verifies a proper sequence of write calls on the writer.
    /// </summary>
    internal abstract class ODataCollectionWriterCore : ODataCollectionWriter
    {
        /// <summary>Version of the OData protocol to use.</summary>
        private readonly ODataVersion version;

        /// <summary>The metadata provider to use.</summary>
        private readonly DataServiceMetadataProviderWrapper metadataProvider;

        /// <summary>True if the writer was created for synchronous operation; false for asynchronous.</summary>
        private readonly bool synchronous;

        /// <summary>Stack of writer scopes to keep track of the current context of the writer.</summary>
        private Stack<Scope> scopes = new Stack<Scope>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="version">The version of the OData protocol to use.</param>
        /// <param name="metadataProvider">The metadata provider to use.</param>
        /// <param name="synchronous">True if the writer is created for synchronous operation; false for asynchronous.</param>
        protected ODataCollectionWriterCore(ODataVersion version, DataServiceMetadataProviderWrapper metadataProvider, bool synchronous)
        {
            DebugUtils.CheckNoExternalCallers();
            this.version = version;
            this.metadataProvider = metadataProvider;
            this.synchronous = synchronous;
            this.scopes.Push(new Scope(CollectionWriterState.Start, null));
        }

        /// <summary>
        /// An enumeration representing the current state of the writer.
        /// </summary>
        internal enum CollectionWriterState
        {
            /// <summary>The writer is at the start; nothing has been written yet.</summary>
            Start,

            /// <summary>
            /// The writer has started writing and is writing the wrapper elements for the 
            /// collection items (if any). No or all items have been written.
            /// </summary>
            Collection,

            /// <summary>The writer is in a state where collection items can be written.</summary>
            Item,

            /// <summary>The writer has completed; nothing can be written anymore.</summary>
            Completed,

            /// <summary>The writer has thrown an ODataException; it is only valid to write an error at this time.</summary>
            ODataExceptionThrown,

            /// <summary>The writer has thrown a fatal exception; nothing can be written to the writer anymore.</summary>
            FatalExceptionThrown,

            /// <summary>Writer has written an error; nothing can be written anymore.</summary>
            Error
        }

        /// <summary>
        /// The metadata provider to use (or null if no metadata is available).
        /// </summary>
        protected DataServiceMetadataProviderWrapper MetadataProvider
        {
            get
            {
                return this.metadataProvider;
            }
        }

        /// <summary>
        /// The current state of the writer.
        /// </summary>
        protected CollectionWriterState State
        {
            get
            {
                return this.scopes.Peek().State;
            }
        }

        /// <summary>
        /// The version of the OData protocol to use.
        /// </summary>
        protected ODataVersion Version
        {
            get
            {
                return this.version;
            }
        }

        /// <summary>
        /// Flushes the write buffer to the underlying stream.
        /// </summary>
        public sealed override void Flush()
        {
            this.VerifySynchronousCallAllowed();
            this.VerifyFlushAllowed();

            // make sure we switch to writer state FatalExceptionThrown if an exception is thrown during flushing.
            try
            {
                this.FlushSynchronously();
            }
            catch
            {
                this.ReplaceScope(CollectionWriterState.FatalExceptionThrown, null);
                throw;
            }
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously flushes the write buffer to the underlying stream.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous operation.</returns>
        public sealed override Task FlushAsync()
        {
            this.VerifyAsynchronousCallAllowed();
            this.VerifyFlushAllowed();

            // make sure we switch to writer state FatalExceptionThrown if an exception is thrown during flushing.
            return this.FlushAsynchronously()
                .ContinueWith(
                    t =>
                    {
                        // TODO, ckerer: if we use TaskContinuationOptions.OnlyOnFaulted instead of this check,
                        //               we always get a TaskCanceledException and it is unclear where it is thrown; review.
                        if (t.IsFaulted)
                        {
                            this.ReplaceScope(CollectionWriterState.FatalExceptionThrown, null);

                            // to avoid nested aggregate exceptions only because we changed the internal state
                            // we re-throw the inner exception if there is only one. Otherwise we have to live
                            // with the nesting.
                            throw t.Exception.InnerExceptions.Count == 1 ? t.Exception.InnerException : t.Exception;
                        }
                    });
        }
#endif

        /// <summary>
        /// Start writing a collection.
        /// </summary>
        /// <param name="collectionName">The name of the collection.</param>
        public sealed override void WriteStart(string collectionName)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(collectionName, "collectionName");

            this.VerifyNotDisposed();
            this.CheckStartPayload();
            this.EnterScope(CollectionWriterState.Collection, collectionName);
            this.InterceptException(() => this.StartCollection(collectionName));
        }

        /// <summary>
        /// Write a collection item.
        /// </summary>
        /// <param name="item">The collection item to write.</param>
        public sealed override void WriteItem(object item)
        {
            this.VerifyNotDisposed();
            if (this.scopes.Peek().State != CollectionWriterState.Item)
            {
                this.EnterScope(CollectionWriterState.Item, item);
            }

            this.InterceptException(() => this.WriteItemImplementation(item));
        }

        /// <summary>
        /// Write an OData error.
        /// </summary>
        /// <param name='errorInstance'>The error information to write.</param>
        /// <param name="includeDebugInformation">If in debug mode error details will be included (if present).</param>
        public sealed override void WriteError(ODataError errorInstance, bool includeDebugInformation)
        {
            ExceptionUtils.CheckArgumentNotNull(errorInstance, "errorInstance");

            this.VerifyNotDisposed();
            this.CheckStartPayload();
            try
            {
                this.EnterScope(CollectionWriterState.Error, this.scopes.Peek().Item);
                this.SerializeError(errorInstance, includeDebugInformation);
            }
            catch (Exception e)
            {
                if (!ExceptionUtils.IsCatchableExceptionType(e))
                {
                    throw;
                }

                // any exception is considered fatal; we transition into FatalExceptionThrown state and
                // do not allow any further writes.
                this.EnterScope(CollectionWriterState.FatalExceptionThrown, this.scopes.Peek().Item);
                throw;
            }
        }

        /// <summary>
        /// Finish writing a collection.
        /// </summary>
        public sealed override void WriteEnd()
        {
            this.VerifyNotDisposed();
            this.InterceptException(this.WriteEndImplementation);
        }

        /// <summary>
        /// Determines whether a given writer state is considered an error state.
        /// </summary>
        /// <param name="state">The writer state to check.</param>
        /// <returns>True if the writer state is an error state; otherwise false.</returns>
        protected static bool IsErrorState(CollectionWriterState state)
        {
            return state == CollectionWriterState.Error || state == CollectionWriterState.ODataExceptionThrown || state == CollectionWriterState.FatalExceptionThrown;
        }

        /// <summary>
        /// Flushes the write buffer to the underlying stream.
        /// </summary>
        protected abstract void FlushSynchronously();

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously flushes the write buffer to the underlying stream.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous operation.</returns>
        protected abstract Task FlushAsynchronously();
#endif

        /// <summary>
        /// Flushes and closes the writer. This method is only called during disposing the ODataCollectionWriter.
        /// </summary>
        /// <param name="discardBufferedData">
        /// If this parameter is true the close of the writer should not throw if some data is still buffered.
        /// If the argument is false the writer is expected to throw if data is still buffered and the writer is closed.
        /// </param>
        protected abstract void FlushAndCloseWriter(bool discardBufferedData);

        /// <summary>
        /// Start writing an OData payload.
        /// </summary>
        protected abstract void StartPayload();

        /// <summary>
        /// Finish writing an OData payload.
        /// </summary>
        protected abstract void EndPayload();

        /// <summary>
        /// Start writing a collection.
        /// </summary>
        /// <param name="collectionName">The name of the collection.</param>
        protected abstract void StartCollection(string collectionName);

        /// <summary>
        /// Finish writing a collection.
        /// </summary>
        protected abstract void EndCollection();

        /// <summary>
        /// Writes a collection item (either primitive or complex)
        /// </summary>
        /// <param name="item">The collection item to write.</param>
        protected abstract void WriteItemImplementation(object item);

        /// <summary>
        /// Write an error message.
        /// </summary>
        /// <param name='error'>The error to write.</param>
        /// <param name="includeDebugInformation">If in debug mode error details will be included (if present).</param>
        protected abstract void SerializeError(ODataError error, bool includeDebugInformation);

        /// <summary>
        /// Perform the actual cleanup work.
        /// </summary>
        /// <param name="disposing">If 'true' this method is called from user code; if 'false' it is called by the runtime.</param>
        /// <remarks>
        /// Classes inheriting from this base class should implement the dispose logic in FlushAndCloseWriter.
        /// </remarks>
        protected sealed override void Dispose(bool disposing)
        {
            if (this.scopes != null)
            {
                if (disposing)
                {
                    if (!IsErrorState(this.State) && this.State != CollectionWriterState.Completed && this.State != CollectionWriterState.Start)
                    {
                        throw new ODataException(Strings.ODataWriterCore_WriterDisposedWithoutAllWriteEnds);
                    }

                    // if the writer is disposed after a fatal exception has been thrown discard all buffered data
                    // of the underlying output stream so we can safely dispose it (below).
                    // if the writer is disposed after an OData exception and no error payload was written 
                    // (i.e., it is in state ODataExceptionThrown) also discard all buffered data
                    bool discardBufferedData = this.State == CollectionWriterState.ODataExceptionThrown || this.State == CollectionWriterState.FatalExceptionThrown || this.State == CollectionWriterState.Start;

                    this.FlushAndCloseWriter(discardBufferedData);
                }
            }

            this.scopes = null;
        }

        /// <summary>
        /// Verifies that the writer is ready for a Flush operation.
        /// </summary>
        private void VerifyFlushAllowed()
        {
            this.VerifyNotDisposed();
            if (this.State == CollectionWriterState.FatalExceptionThrown)
            {
                throw new ODataException(Strings.ODataWriterCore_FlushAsyncCalledInFatalErrorState);
            }
        }

        /// <summary>
        /// Verifies that a synchronous operation is allowed on this writer.
        /// </summary>
        private void VerifySynchronousCallAllowed()
        {
            if (!this.synchronous)
            {
                throw new ODataException(Strings.ODataCollectionWriterCore_SyncCallOnAsyncWriter);
            }
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Verifies that an asynchronous operation is allowed on this writer.
        /// </summary>
        private void VerifyAsynchronousCallAllowed()
        {
            if (this.synchronous)
            {
                throw new ODataException(Strings.ODataCollectionWriterCore_AsyncCallOnSyncWriter);
            }
        }
#endif

        /// <summary>
        /// Finish writing a collection.
        /// </summary>
        private void WriteEndImplementation()
        {
            Scope currentScope = this.scopes.Peek();

            switch (currentScope.State)
            {
                case CollectionWriterState.Collection:
                    this.EndCollection();
                    break;
                case CollectionWriterState.Item:
                    this.LeaveScope();
                    Debug.Assert(this.scopes.Peek().State == CollectionWriterState.Collection, "Expected to find collection state after popping from item state.");
                    this.EndCollection();
                    break;
                case CollectionWriterState.Start:                 // fall through
                case CollectionWriterState.Completed:             // fall through
                case CollectionWriterState.Error:                 // fall through
                case CollectionWriterState.ODataExceptionThrown:  // fall through
                case CollectionWriterState.FatalExceptionThrown:
                    throw new ODataException(Strings.ODataCollectionWriterCore_WriteEndCalledInInvalidState(currentScope.State.ToString()));
                default:
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataCollectionWriterCore_WriteEnd_UnreachableCodePath));
            }

            this.LeaveScope();
        }

        /// <summary>
        /// Checks whether we are currently writing the first top-level element; if so call StartPayload
        /// </summary>
        private void CheckStartPayload()
        {
            Scope current = this.scopes.Peek();
            if (current.State == CollectionWriterState.Start)
            {
                this.InterceptException(this.StartPayload);
            }
        }

        /// <summary>
        /// Catch any exception thrown by the action passed in; in the exception case move the writer into
        /// state ExceptionThrown and then rethrow the exception.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        private void InterceptException(Action action)
        {
            try
            {
                action();
            }
            catch (ODataException)
            {
                // if an ODataException is thrown we change to the ODataExceptionThrown state in which
                // errors can still be written. This indicates a problem with the payload but not with the
                // underlying stream.
                if (!IsErrorState(this.State))
                {
                    this.EnterScope(CollectionWriterState.ODataExceptionThrown, this.scopes.Peek().Item);
                }

                throw;
            }
            catch (Exception e)
            {
                if (!ExceptionUtils.IsCatchableExceptionType(e))
                {
                    throw;
                }

                // any non-ODataException is considered fatal; we transition into FatalExceptionThrown state and
                // do not allow any further writes.
                this.EnterScope(CollectionWriterState.FatalExceptionThrown, this.scopes.Peek().Item);
                throw;
            }
        }

        /// <summary>
        /// Check if the object has been disposed; called from all public API methods. Throws an ObjectDisposedException if the object
        /// has already been disposed.
        /// </summary>
        private void VerifyNotDisposed()
        {
            if (this.scopes == null)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        /// <summary>
        /// Enter a new writer scope; verifies that the transition from the current state into new state is valid
        /// and attaches the item to the new scope.
        /// </summary>
        /// <param name="newState">The writer state to transition into.</param>
        /// <param name="item">The item to associate with the new scope.</param>
        private void EnterScope(CollectionWriterState newState, object item)
        {
            this.InterceptException(() => this.ValidateTransition(newState));
            this.scopes.Push(new Scope(newState, item));
        }

        /// <summary>
        /// Leave the current writer scope and return to the previous scope. 
        /// When reaching the top-level replace the 'Started' scope with a 'Completed' scope.
        /// </summary>
        /// <remarks>Note that this method is never called once an error has been written or a fatal exception has been thrown.</remarks>
        private void LeaveScope()
        {
            Debug.Assert(this.State != CollectionWriterState.Error, "this.State != WriterState.Error");
            Debug.Assert(this.State != CollectionWriterState.FatalExceptionThrown, "this.State != WriterState.FatalExceptionThrown");

            this.scopes.Pop();

            // if we are back at the root replace the 'Start' state with the 'Completed' state
            if (this.scopes.Count == 1)
            {
                this.scopes.Pop();
                this.scopes.Push(new Scope(CollectionWriterState.Completed, null));
                this.InterceptException(this.EndPayload);
            }
        }

        /// <summary>
        /// Replaces the current scope with a new scope; checks that the transition is valid.
        /// </summary>
        /// <param name="newState">The new state to transition into.</param>
        /// <param name="item">The item associated with the new state.</param>
        private void ReplaceScope(CollectionWriterState newState, ODataItem item)
        {
            this.ValidateTransition(newState);
            this.scopes.Pop();
            this.scopes.Push(new Scope(newState, item));
        }

        /// <summary>
        /// Verify that the transition from the current state into new state is valid .
        /// </summary>
        /// <param name="newState">The new writer state to transition into.</param>
        private void ValidateTransition(CollectionWriterState newState)
        {
            if (!IsErrorState(this.State) && IsErrorState(newState) &&
                !(this.State == CollectionWriterState.Completed && newState == CollectionWriterState.Error))
            {
                // we can always transition into an error state if we are not already in an error state
                // unless we're in a completed state trying to write an error (we can't write error after the payload
                // was finished as it might introduce another top-level element in XML)
                return;
            }

            switch (this.State)
            {
                case CollectionWriterState.Start:
                    if (newState != CollectionWriterState.Collection && newState != CollectionWriterState.Completed)
                    {
                        throw new ODataException(Strings.ODataCollectionWriterCore_InvalidTransitionFromStart(this.State.ToString(), newState.ToString()));
                    }

                    break;
                case CollectionWriterState.Collection:
                    if (newState != CollectionWriterState.Item && newState != CollectionWriterState.Completed)
                    {
                        throw new ODataException(Strings.ODataCollectionWriterCore_InvalidTransitionFromCollection(this.State.ToString(), newState.ToString()));
                    }

                    break;
                case CollectionWriterState.Item:
                    if (newState != CollectionWriterState.Completed)
                    {
                        throw new ODataException(Strings.ODataCollectionWriterCore_InvalidTransitionFromItem(this.State.ToString(), newState.ToString()));
                    }

                    break;
                case CollectionWriterState.Completed:
                    // we should never see a state transition when in state 'Completed'
                    throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromCompleted(this.State.ToString(), newState.ToString()));
                case CollectionWriterState.ODataExceptionThrown:
                    if (!IsErrorState(newState))
                    {
                        // once an exception has been thrown we only allow clients to write an error (or we detect a fatal exception)
                        throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromODataExceptionThrown(this.State.ToString(), newState.ToString()));
                    }

                    break;
                case CollectionWriterState.Error:
                    // No more state transitions once we are in error state except for the fatal error
                    if (newState != CollectionWriterState.FatalExceptionThrown)
                    {
                        throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromError(this.State.ToString(), newState.ToString()));
                    }

                    break;
                case CollectionWriterState.FatalExceptionThrown:
                    if (newState != CollectionWriterState.FatalExceptionThrown)
                    {
                        throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromFatalExceptionThrown(this.State.ToString(), newState.ToString()));
                    }

                    break;
                default:
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataCollectionWriterCore_ValidateTransition_UnreachableCodePath));
            }
        }

        /// <summary>
        /// A writer scope; keeping track of the current writer state and an item associated with this state.
        /// </summary>
        private sealed class Scope
        {
            /// <summary>The writer state of this scope.</summary>
            private readonly CollectionWriterState state;

            /// <summary>The item attached to this scope.</summary>
            private readonly object item;

            /// <summary>
            /// Constructor creating a new writer scope.
            /// </summary>
            /// <param name="state">The writer state of this scope.</param>
            /// <param name="item">The item attached to this scope.</param>
            public Scope(CollectionWriterState state, object item)
            {
                this.state = state;
                this.item = item;
            }

            /// <summary>
            /// The writer state of this scope.
            /// </summary>
            public CollectionWriterState State
            {
                get
                {
                    return this.state;
                }
            }

            /// <summary>
            /// The item attached to this scope.
            /// </summary>
            public object Item
            {
                get
                {
                    return this.item;
                }
            }
        }
    }
}

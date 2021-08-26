//---------------------------------------------------------------------
// <copyright file="ODataCollectionWriterCore.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// Base class for OData collection writers that verifies a proper sequence of write calls on the writer.
    /// </summary>
    internal abstract class ODataCollectionWriterCore : ODataCollectionWriter, IODataOutputInStreamErrorListener
    {
        /// <summary>The output context to write to.</summary>
        private readonly ODataOutputContext outputContext;

        /// <summary>If not null, the writer will notify the implementer of the interface of relevant state changes in the writer.</summary>
        private readonly IODataReaderWriterListener listener;

        /// <summary>Stack of writer scopes to keep track of the current context of the writer.</summary>
        private readonly Stack<Scope> scopes = new Stack<Scope>();

        /// <summary>The expected type of the items in the collection or null if no expected item type exists.</summary>
        private readonly IEdmTypeReference expectedItemType;

        /// <summary>Checker to detect duplicate property names on complex collection items.</summary>
        private IDuplicatePropertyNameChecker duplicatePropertyNameChecker;

        /// <summary>The collection validator instance if no expected item type has been specified; otherwise null.</summary>
        private CollectionWithoutExpectedTypeValidator collectionValidator;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="outputContext">The output context to write to.</param>
        /// <param name="itemTypeReference">The item type of the collection being written or null if no metadata is available.</param>
        protected ODataCollectionWriterCore(ODataOutputContext outputContext, IEdmTypeReference itemTypeReference)
            : this(outputContext, itemTypeReference, null)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="outputContext">The output context to write to.</param>
        /// <param name="expectedItemType">The type reference of the expected item type or null if no expected item type exists.</param>
        /// <param name="listener">If not null, the writer will notify the implementer of the interface of relevant state changes in the writer.</param>
        protected ODataCollectionWriterCore(ODataOutputContext outputContext, IEdmTypeReference expectedItemType, IODataReaderWriterListener listener)
        {
            Debug.Assert(outputContext != null, "outputContext != null");

            this.outputContext = outputContext;
            this.expectedItemType = expectedItemType;
            this.listener = listener;
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

            /// <summary>Writer has written an error; nothing can be written anymore.</summary>
            Error
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

        /// <summary>Checker to detect duplicate property names on complex collection items.</summary>
        protected IDuplicatePropertyNameChecker DuplicatePropertyNameChecker
        {
            get
            {
                return duplicatePropertyNameChecker
                       ?? (duplicatePropertyNameChecker
                           = outputContext.MessageWriterSettings.Validator.CreateDuplicatePropertyNameChecker());
            }
        }

        /// <summary>
        /// The collection validator instance.
        /// </summary>
        protected CollectionWithoutExpectedTypeValidator CollectionValidator
        {
            get
            {
                return this.collectionValidator;
            }
        }

        /// <summary>
        /// The item type of the collection being written or null if no metadata is available.
        /// </summary>
        protected IEdmTypeReference ItemTypeReference
        {
            get
            {
                return this.expectedItemType;
            }
        }

        /// <summary>
        /// Flushes the write buffer to the underlying stream.
        /// </summary>
        public sealed override void Flush()
        {
            this.VerifyCanFlush(true);

            // make sure we switch to writer state Error if an exception is thrown during flushing.
            try
            {
                this.FlushSynchronously();
            }
            catch
            {
                this.ReplaceScope(CollectionWriterState.Error, null);
                throw;
            }
        }

        /// <summary>
        /// Asynchronously flushes the write buffer to the underlying stream.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous operation.</returns>
        public sealed override Task FlushAsync()
        {
            this.VerifyCanFlush(false);

            // make sure we switch to writer state Error if an exception is thrown during flushing.
            return this.FlushAsynchronously().FollowOnFaultWith(t => this.ReplaceScope(CollectionWriterState.Error, null));
        }

        /// <summary>
        /// Start writing a collection.
        /// </summary>
        /// <param name="collectionStart">The <see cref="ODataCollectionStart"/> representing the collection.</param>
        public sealed override void WriteStart(ODataCollectionStart collectionStart)
        {
            this.VerifyCanWriteStart(true, collectionStart);
            this.WriteStartImplementation(collectionStart);
        }

        /// <summary>
        /// Asynchronously start writing a collection.
        /// </summary>
        /// <param name="collection">The <see cref="ODataCollectionStart"/> representing the collection.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public sealed override async Task WriteStartAsync(ODataCollectionStart collection)
        {
            this.VerifyCanWriteStart(false, collection);
            await this.WriteStartImplementationAsync(collection)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Write a collection item.
        /// </summary>
        /// <param name="item">The collection item to write.</param>
        public sealed override void WriteItem(object item)
        {
            this.VerifyCanWriteItem(true);
            this.WriteItemImplementation(item);
        }

        /// <summary>
        /// Asynchronously start writing a collection item.
        /// </summary>
        /// <param name="item">The collection item to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public sealed override async Task WriteItemAsync(object item)
        {
            this.VerifyCanWriteItem(false);
            await this.WriteItemImplementationAsync(item)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Finish writing a collection.
        /// </summary>
        public sealed override void WriteEnd()
        {
            this.VerifyCanWriteEnd(true);
            this.WriteEndImplementation();

            if (this.scopes.Peek().State == CollectionWriterState.Completed)
            {
                // Note that we intentionally go through the public API so that if the Flush fails the writer moves to the Error state.
                this.Flush();
            }
        }

        /// <summary>
        /// Asynchronously finish writing a collection.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public sealed override async Task WriteEndAsync()
        {
            this.VerifyCanWriteEnd(false);
            await this.WriteEndImplementationAsync()
                .ConfigureAwait(false);

            if (this.scopes.Peek().State == CollectionWriterState.Completed)
            {
                // Note that we intentionally go through the public API so that if the FlushAsync fails the writer moves to the Error state.
                await this.FlushAsync()
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// This method notifies the listener, that an in-stream error is to be written.
        /// </summary>
        /// <remarks>
        /// This listener can choose to fail, if the currently written payload doesn't support in-stream error at this position.
        /// If the listener returns, the writer should not allow any more writing, since the in-stream error is the last thing in the payload.
        /// </remarks>
        void IODataOutputInStreamErrorListener.OnInStreamError()
        {
            this.VerifyNotDisposed();

            // We're in a completed state trying to write an error (we can't write error after the payload was finished as it might
            // introduce another top-level element in XML)
            if (this.State == CollectionWriterState.Completed)
            {
                throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromCompleted(this.State.ToString(), CollectionWriterState.Error.ToString()));
            }

            this.StartPayloadInStartState();
            this.EnterScope(CollectionWriterState.Error, this.scopes.Peek().Item);
        }

        /// <inheritdoc/>
        async Task IODataOutputInStreamErrorListener.OnInStreamErrorAsync()
        {
            this.VerifyNotDisposed();

            // We're in a completed state trying to write an error
            // We can't write error after the payload was finished as it might introduce another top-level element in XML
            if (this.State == CollectionWriterState.Completed)
            {
                throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromCompleted(this.State.ToString(), CollectionWriterState.Error.ToString()));
            }

            await this.StartPayloadInStartStateAsync()
                .ConfigureAwait(false);
            this.EnterScope(CollectionWriterState.Error, this.scopes.Peek().Item);
        }

        /// <summary>
        /// Determines whether a given writer state is considered an error state.
        /// </summary>
        /// <param name="state">The writer state to check.</param>
        /// <returns>True if the writer state is an error state; otherwise false.</returns>
        protected static bool IsErrorState(CollectionWriterState state)
        {
            return state == CollectionWriterState.Error;
        }

        /// <summary>
        /// Check if the object has been disposed; called from all public API methods. Throws an ObjectDisposedException if the object
        /// has already been disposed.
        /// </summary>
        protected abstract void VerifyNotDisposed();

        /// <summary>
        /// Flush the output.
        /// </summary>
        protected abstract void FlushSynchronously();

        /// <summary>
        /// Flush the output.
        /// </summary>
        /// <returns>Task representing the pending flush operation.</returns>
        protected abstract Task FlushAsynchronously();

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
        /// <param name="collectionStart">The <see cref="ODataCollectionStart"/> representing the collection.</param>
        protected abstract void StartCollection(ODataCollectionStart collectionStart);

        /// <summary>
        /// Finish writing a collection.
        /// </summary>
        protected abstract void EndCollection();

        /// <summary>
        /// Writes a collection item (either primitive or complex)
        /// </summary>
        /// <param name="item">The collection item to write.</param>
        /// <param name="expectedItemTypeReference">The expected type of the collection item or null if no expected item type exists.</param>
        protected abstract void WriteCollectionItem(object item, IEdmTypeReference expectedItemTypeReference);

        /// <summary>
        /// Asynchronously start writing an OData payload.
        /// </summary>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected abstract Task StartPayloadAsync();

        /// <summary>
        /// Asynchronously finish writing an OData payload.
        /// </summary>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected abstract Task EndPayloadAsync();

        /// <summary>
        /// Asynchronously start writing a collection.
        /// </summary>
        /// <param name="collectionStart">The <see cref="ODataCollectionStart"/> representing the collection.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected abstract Task StartCollectionAsync(ODataCollectionStart collectionStart);

        /// <summary>
        /// Asynchronously finish writing a collection.
        /// </summary>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected abstract Task EndCollectionAsync();

        /// <summary>
        /// Asynchronously writes a collection item (either primitive or complex)
        /// </summary>
        /// <param name="item">The collection item to write.</param>
        /// <param name="expectedItemTypeReference">The expected type of the collection item or null if no expected item type exists.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        protected abstract Task WriteCollectionItemAsync(object item, IEdmTypeReference expectedItemTypeReference);

        /// <summary>
        /// Verifies that calling WriteStart is valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        /// <param name="collectionStart">The <see cref="ODataCollectionStart"/> representing the collection.</param>
        private void VerifyCanWriteStart(bool synchronousCall, ODataCollectionStart collectionStart)
        {
            ExceptionUtils.CheckArgumentNotNull(collectionStart, "collection");

            this.VerifyNotDisposed();
            this.VerifyCallAllowed(synchronousCall);
        }

        /// <summary>
        /// Start writing a collection - implementation of the actual functionality.
        /// </summary>
        /// <param name="collectionStart">The <see cref="ODataCollectionStart"/> representing the collection.</param>
        private void WriteStartImplementation(ODataCollectionStart collectionStart)
        {
            this.StartPayloadInStartState();
            this.EnterScope(CollectionWriterState.Collection, collectionStart);
            this.InterceptException(
                (thisParam, collectionStartParam) =>
                {
                    if (thisParam.expectedItemType == null)
                    {
                        thisParam.collectionValidator = new CollectionWithoutExpectedTypeValidator(/*expectedItemTypeName*/ null);
                    }

                    thisParam.StartCollection(collectionStartParam);
                },
                collectionStart);
        }

        /// <summary>
        /// Verify that calling WriteItem is valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        private void VerifyCanWriteItem(bool synchronousCall)
        {
            this.VerifyNotDisposed();
            this.VerifyCallAllowed(synchronousCall);
        }

        /// <summary>
        /// Write a collection item - implementation of the actual functionality.
        /// </summary>
        /// <param name="item">The collection item to write.</param>
        private void WriteItemImplementation(object item)
        {
            if (this.scopes.Peek().State != CollectionWriterState.Item)
            {
                this.EnterScope(CollectionWriterState.Item, item);
            }

            this.InterceptException(
                (thisParam, itemParam) =>
                {
                    ValidationUtils.ValidateCollectionItem(itemParam, true /* isNullable */);
                    thisParam.WriteCollectionItem(itemParam, thisParam.expectedItemType);
                }, item);
        }

        /// <summary>
        /// Verifies that calling WriteEnd is valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        private void VerifyCanWriteEnd(bool synchronousCall)
        {
            this.VerifyNotDisposed();
            this.VerifyCallAllowed(synchronousCall);
        }

        /// <summary>
        /// Finish writing a collection - implementation of the actual functionality.
        /// </summary>
        private void WriteEndImplementation()
        {
            this.InterceptException(
                (thisParam) =>
                {
                    Scope currentScope = thisParam.scopes.Peek();

                    switch (currentScope.State)
                    {
                        case CollectionWriterState.Collection:
                            thisParam.EndCollection();
                            break;
                        case CollectionWriterState.Item:
                            thisParam.LeaveScope();
                            Debug.Assert(thisParam.scopes.Peek().State == CollectionWriterState.Collection, "Expected to find collection state after popping from item state.");
                            thisParam.EndCollection();
                            break;
                        case CollectionWriterState.Start:                 // fall through
                        case CollectionWriterState.Completed:             // fall through
                        case CollectionWriterState.Error:                 // fall through
                            throw new ODataException(Strings.ODataCollectionWriterCore_WriteEndCalledInInvalidState(currentScope.State.ToString()));
                        default:
                            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataCollectionWriterCore_WriteEnd_UnreachableCodePath));
                    }

                    thisParam.LeaveScope();
                });
        }

        /// <summary>
        /// Verifies that calling Flush is valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        private void VerifyCanFlush(bool synchronousCall)
        {
            this.VerifyNotDisposed();
            this.VerifyCallAllowed(synchronousCall);
        }

        /// <summary>
        /// Verifies that a call is allowed to the writer.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        private void VerifyCallAllowed(bool synchronousCall)
        {
            if (synchronousCall)
            {
                if (!this.outputContext.Synchronous)
                {
                    throw new ODataException(Strings.ODataCollectionWriterCore_SyncCallOnAsyncWriter);
                }
            }
            else
            {
                if (this.outputContext.Synchronous)
                {
                    throw new ODataException(Strings.ODataCollectionWriterCore_AsyncCallOnSyncWriter);
                }
            }
        }

        /// <summary>
        /// Checks whether we are currently writing the first top-level element; if so call StartPayload
        /// </summary>
        private void StartPayloadInStartState()
        {
            Scope current = this.scopes.Peek();
            if (current.State == CollectionWriterState.Start)
            {
                this.InterceptException((thisParam) => thisParam.StartPayload());
            }
        }

        /// <summary>
        /// Catch any exception thrown by the action passed in; in the exception case move the writer into
        /// state Error and then re-throw the exception.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <remarks>
        /// Make sure to only use anonymous functions that don't capture state from the enclosing context, 
        /// so the compiler optimizes the code to avoid delegate and closure allocations on every call to this method.
        /// </remarks>
        private void InterceptException(Action<ODataCollectionWriterCore> action)
        {
            try
            {
                action(this);
            }
            catch
            {
                if (!IsErrorState(this.State))
                {
                    this.EnterScope(CollectionWriterState.Error, this.scopes.Peek().Item);
                }

                throw;
            }
        }

        /// <summary>
        /// Catch any exception thrown by the action passed in; in the exception case move the writer into
        /// state Error and then rethrow the exception.
        /// </summary>
        /// <typeparam name="TArg0">The action argument type.</typeparam>
        /// <param name="action">The action to execute.</param>
        /// <param name="arg0">The argument value provided to the action.</param>
        /// <remarks>
        /// Make sure to only use anonymous functions that don't capture state from the enclosing context, 
        /// so the compiler optimizes the code to avoid delegate and closure allocations on every call to this method.
        /// </remarks>
        private void InterceptException<TArg0>(Action<ODataCollectionWriterCore, TArg0> action, TArg0 arg0)
        {
            try
            {
                action(this, arg0);
            }
            catch
            {
                if (!IsErrorState(this.State))
                {
                    this.EnterScope(CollectionWriterState.Error, this.scopes.Peek().Item);
                }

                throw;
            }
        }

        /// <summary>
        /// Notifies the implementer of the <see cref="IODataReaderWriterListener"/> interface of relevant state changes in the writer.
        /// </summary>
        /// <param name="newState">The new writer state.</param>
        private void NotifyListener(CollectionWriterState newState)
        {
            if (this.listener != null)
            {
                if (IsErrorState(newState))
                {
                    this.listener.OnException();
                }
                else if (newState == CollectionWriterState.Completed)
                {
                    this.listener.OnCompleted();
                }
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
            this.InterceptException((thisParam, newStateParam) => thisParam.ValidateTransition(newStateParam), newState);
            this.scopes.Push(new Scope(newState, item));
            this.NotifyListener(newState);
        }

        /// <summary>
        /// Leave the current writer scope and return to the previous scope.
        /// When reaching the top-level replace the 'Started' scope with a 'Completed' scope.
        /// </summary>
        /// <remarks>Note that this method is never called once an error has been written or a fatal exception has been thrown.</remarks>
        private void LeaveScope()
        {
            Debug.Assert(this.State != CollectionWriterState.Error, "this.State != WriterState.Error");

            this.scopes.Pop();

            // if we are back at the root replace the 'Start' state with the 'Completed' state
            if (this.scopes.Count == 1)
            {
                this.scopes.Pop();
                this.scopes.Push(new Scope(CollectionWriterState.Completed, null));
                this.InterceptException((thisParam) => thisParam.EndPayload());
                this.NotifyListener(CollectionWriterState.Completed);
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
            this.NotifyListener(newState);
        }

        /// <summary>
        /// Verify that the transition from the current state into new state is valid .
        /// </summary>
        /// <param name="newState">The new writer state to transition into.</param>
        private void ValidateTransition(CollectionWriterState newState)
        {
            if (!IsErrorState(this.State) && IsErrorState(newState))
            {
                // we can always transition into an error state if we are not already in an error state
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
                case CollectionWriterState.Error:
                    if (newState != CollectionWriterState.Error)
                    {
                        // No more state transitions once we are in error state
                        throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromError(this.State.ToString(), newState.ToString()));
                    }

                    break;
                default:
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataCollectionWriterCore_ValidateTransition_UnreachableCodePath));
            }
        }

        /// <summary>
        /// Catch any exception thrown by the action passed in; in the exception case move the writer into
        /// state Error and then rethrow the exception.
        /// </summary>
        /// <param name="action">The action to execute asynchronously.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <remarks>
        /// Make sure to only use anonymous functions that don't capture state from the enclosing context, 
        /// so the compiler optimizes the code to avoid delegate and closure allocations on every call to this method.
        /// </remarks>
        private async Task InterceptExceptionAsync(Func<ODataCollectionWriterCore, Task> action)
        {
            try
            {
                await action(this).ConfigureAwait(false);
            }
            catch
            {
                if (!IsErrorState(this.State))
                {
                    this.EnterScope(CollectionWriterState.Error, this.scopes.Peek().Item);
                }

                throw;
            }
        }

        /// <summary>
        /// Catch any exception thrown by the action passed in; in the exception case move the writer into
        /// state Error and then rethrow the exception.
        /// </summary>
        /// <typeparam name="TArg0">The action argument type.</typeparam>
        /// <param name="action">The action to execute asynchronously.</param>
        /// <param name="arg0">The argument value provided to the action.</param>
        /// <remarks>
        /// Make sure to only use anonymous functions that don't capture state from the enclosing context, 
        /// so the compiler optimizes the code to avoid delegate and closure allocations on every call to this method.
        /// </remarks>
        private async Task InterceptExceptionAsync<TArg0>(Func<ODataCollectionWriterCore, TArg0, Task> action, TArg0 arg0)
        {
            try
            {
                await action(this, arg0).ConfigureAwait(false);
            }
            catch
            {
                if (!IsErrorState(this.State))
                {
                    this.EnterScope(CollectionWriterState.Error, this.scopes.Peek().Item);
                }

                throw;
            }
        }

        /// <summary>
        /// Asynchronously checks whether we are currently writing the first top-level element; if so call StartPayload
        /// </summary>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private Task StartPayloadInStartStateAsync()
        {
            Scope current = this.scopes.Peek();
            if (current.State == CollectionWriterState.Start)
            {
                return this.InterceptExceptionAsync((thisParam) => thisParam.StartPayloadAsync());
            }

            return TaskUtils.CompletedTask;
        }

        /// <summary>
        /// Asynchronously starts writing a collection - implementation of the actual functionality.
        /// </summary>
        /// <param name="collectionStart">The <see cref="ODataCollectionStart"/> representing the collection.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private async Task WriteStartImplementationAsync(ODataCollectionStart collectionStart)
        {
            await this.StartPayloadInStartStateAsync()
                .ConfigureAwait(false);
            this.EnterScope(CollectionWriterState.Collection, collectionStart);
            await this.InterceptExceptionAsync(
                async (thisParam, collectionStartParam) =>
                {
                    if (thisParam.expectedItemType == null)
                    {
                        thisParam.collectionValidator = new CollectionWithoutExpectedTypeValidator(/*expectedItemTypeName*/ null);
                    }

                    await thisParam.StartCollectionAsync(collectionStartParam)
                        .ConfigureAwait(false);
                },
                collectionStart).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously finishes writing a collection - implementation of the actual functionality.
        /// </summary>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private Task WriteEndImplementationAsync()
        {
            return this.InterceptExceptionAsync(async (thisParam) =>
            {
                Scope currentScope = thisParam.scopes.Peek();

                switch (currentScope.State)
                {
                    case CollectionWriterState.Collection:
                        await thisParam.EndCollectionAsync().ConfigureAwait(false);
                        break;
                    case CollectionWriterState.Item:
                        await thisParam.LeaveScopeAsync().ConfigureAwait(false);
                        Debug.Assert(
                            thisParam.scopes.Peek().State == CollectionWriterState.Collection,
                            "Expected to find collection state after popping from item state.");
                        await thisParam.EndCollectionAsync().ConfigureAwait(false);
                        break;
                    case CollectionWriterState.Start:                 // fall through
                    case CollectionWriterState.Completed:             // fall through
                    case CollectionWriterState.Error:                 // fall through
                        throw new ODataException(Strings.ODataCollectionWriterCore_WriteEndCalledInInvalidState(currentScope.State.ToString()));
                    default:
                        throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataCollectionWriterCore_WriteEnd_UnreachableCodePath));
                }

                await thisParam.LeaveScopeAsync().ConfigureAwait(false);
            });
        }

        /// <summary>
        /// Asynchronously writes a collection item - implementation of the actual functionality.
        /// </summary>
        /// <param name="item">The collection item to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private async Task WriteItemImplementationAsync(object item)
        {
            if (this.scopes.Peek().State != CollectionWriterState.Item)
            {
                this.EnterScope(CollectionWriterState.Item, item);
            }

            await this.InterceptExceptionAsync(
                async(thisParam, itemParam) =>
                {
                    ValidationUtils.ValidateCollectionItem(itemParam, true /* isNullable */);
                    await thisParam.WriteCollectionItemAsync(itemParam, thisParam.expectedItemType)
                        .ConfigureAwait(false);
                },
                item).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously leaves the current writer scope and return to the previous scope.
        /// When reaching the top-level replace the 'Started' scope with a 'Completed' scope.
        /// </summary>
        /// <remarks>Note that this method is never called once an error has been written or a fatal exception has been thrown.</remarks>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task LeaveScopeAsync()
        {
            Debug.Assert(this.State != CollectionWriterState.Error, "this.State != WriterState.Error");

            this.scopes.Pop();

            // if we are back at the root replace the 'Start' state with the 'Completed' state
            if (this.scopes.Count == 1)
            {
                this.scopes.Pop();
                this.scopes.Push(new Scope(CollectionWriterState.Completed, null));
                await this.InterceptExceptionAsync((thisParam) => thisParam.EndPayloadAsync())
                    .ConfigureAwait(false);
                this.NotifyListener(CollectionWriterState.Completed);
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

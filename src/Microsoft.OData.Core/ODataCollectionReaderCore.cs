//---------------------------------------------------------------------
// <copyright file="ODataCollectionReaderCore.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
#if PORTABLELIB
    using System.Threading.Tasks;
#endif
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Base class for OData collection readers that verifies a proper sequence of read calls on the reader.
    /// </summary>
    internal abstract class ODataCollectionReaderCore : ODataCollectionReader
    {
        /// <summary>The input context to read from.</summary>
        private readonly ODataInputContext inputContext;

        /// <summary>Stack of reader scopes to keep track of the current context of the reader.</summary>
        private readonly Stack<Scope> scopes = new Stack<Scope>();

        /// <summary>If not null, the reader will notify the implementer of the interface of relevant state changes in the reader.</summary>
        private readonly IODataReaderWriterListener listener;

        /// <summary>The collection validator instance if no expected item type has been specified; otherwise null.</summary>
        private CollectionWithoutExpectedTypeValidator collectionValidator;

        /// <summary>The expected item type reference for the items in the collection.</summary>
        /// <remarks>If an expected type is specified the collection has to be homogeneous.</remarks>
        private IEdmTypeReference expectedItemTypeReference;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputContext">The input to read from.</param>
        /// <param name="expectedItemTypeReference">The expected type reference for the items in the collection.</param>
        /// <param name="listener">If not null, the reader will notify the implementer of the interface of relevant state changes in the reader.</param>
        protected ODataCollectionReaderCore(
            ODataInputContext inputContext,
            IEdmTypeReference expectedItemTypeReference,
            IODataReaderWriterListener listener)
        {
            this.inputContext = inputContext;
            this.expectedItemTypeReference = expectedItemTypeReference;

            if (this.expectedItemTypeReference == null)
            {
                // NOTE: collections cannot specify a type name for the collection itself, so always passing null.
                this.collectionValidator = new CollectionWithoutExpectedTypeValidator(/*expectedItemTypeName*/ null);
            }

            this.listener = listener;
            this.EnterScope(ODataCollectionReaderState.Start, null);
        }

        /// <summary>
        /// The current state of the reader.
        /// </summary>
        public override sealed ODataCollectionReaderState State
        {
            get
            {
                this.inputContext.VerifyNotDisposed();
                Debug.Assert(this.scopes != null && this.scopes.Count > 0, "A scope must always exist.");
                return this.scopes.Peek().State;
            }
        }

        /// <summary>
        /// The most recent item that has been read.
        /// </summary>
        public override sealed object Item
        {
            get
            {
                this.inputContext.VerifyNotDisposed();
                Debug.Assert(this.scopes != null && this.scopes.Count > 0, "A scope must always exist.");
                return this.scopes.Peek().Item;
            }
        }

        /// <summary>
        /// The expected item type for the items in the collection.
        /// </summary>
        protected IEdmTypeReference ExpectedItemTypeReference
        {
            get
            {
                return this.expectedItemTypeReference;
            }

            set
            {
                ExceptionUtils.CheckArgumentNotNull(value, "value");

                Debug.Assert(this.State == ODataCollectionReaderState.Start, "this.State == ODataCollectionReaderState.Start");

                if (this.expectedItemTypeReference != value)
                {
                    this.expectedItemTypeReference = value;

                    // If we set an expected item type reference (e.g., from the
                    // Json Light context URI), we need to reset the collection validator.
                    this.collectionValidator = null;
                }
            }
        }

        /// <summary>
        /// The collection validator instance if no expected item type has been specified; otherwise null.
        /// </summary>
        protected CollectionWithoutExpectedTypeValidator CollectionValidator
        {
            get
            {
                return this.collectionValidator;
            }
        }

        /// <summary>
        /// Returns true if we are reading a nested payload, e.g. a resource, a resource set or a collection within a parameters payload.
        /// </summary>
        protected bool IsReadingNestedPayload
        {
            get
            {
                return this.listener != null;
            }
        }

        /// <summary>
        /// Reads the next item from the message payload.
        /// </summary>
        /// <returns>true if more items were read; otherwise false.</returns>
        public override sealed bool Read()
        {
            this.VerifyCanRead(true);
            return this.InterceptException(this.ReadSynchronously);
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously reads the next item from the message payload.
        /// </summary>
        /// <returns>A task that when completed indicates whether more items were read.</returns>
        public override sealed Task<bool> ReadAsync()
        {
            this.VerifyCanRead(false);
            return this.ReadAsynchronously().FollowOnFaultWith(t => this.EnterScope(ODataCollectionReaderState.Exception, null));
        }
#endif

        /// <summary>
        /// Reads the next <see cref="ODataItem"/> from the message payload.
        /// </summary>
        /// <returns>true if more items were read; otherwise false.</returns>
        protected bool ReadImplementation()
        {
            bool result;
            switch (this.State)
            {
                case ODataCollectionReaderState.Start:
                    result = this.ReadAtStartImplementation();
                    break;

                case ODataCollectionReaderState.CollectionStart:
                    result = this.ReadAtCollectionStartImplementation();
                    break;

                case ODataCollectionReaderState.Value:
                    result = this.ReadAtValueImplementation();
                    break;

                case ODataCollectionReaderState.CollectionEnd:
                    result = this.ReadAtCollectionEndImplementation();
                    break;

                default:
                    Debug.Assert(false, "Unsupported collection reader state " + this.State + " detected.");
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataCollectionReaderCore_ReadImplementation));
            }

            return result;
        }

        /// <summary>
        /// Implementation of the collection reader logic when in state 'Start'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        protected abstract bool ReadAtStartImplementation();

        /// <summary>
        /// Implementation of the reader logic when in state 'CollectionStart'.
        /// </summary>
        /// <returns>true if more nodes can be read from the reader; otherwise false.</returns>
        protected abstract bool ReadAtCollectionStartImplementation();

        /// <summary>
        /// Implementation of the reader logic when in state 'Value'.
        /// </summary>
        /// <returns>true if more nodes can be read from the reader; otherwise false.</returns>
        protected abstract bool ReadAtValueImplementation();

        /// <summary>
        /// Implementation of the reader logic when in state 'CollectionEnd'.
        /// </summary>
        /// <returns>Should be false since no more nodes can be read from the reader after the collection ends.</returns>
        protected abstract bool ReadAtCollectionEndImplementation();

        /// <summary>
        /// Reads the next <see cref="ODataItem"/> from the message payload.
        /// </summary>
        /// <returns>true if more items were read; otherwise false.</returns>
        protected bool ReadSynchronously()
        {
            return this.ReadImplementation();
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously reads the next <see cref="ODataItem"/> from the message payload.
        /// </summary>
        /// <returns>A task that when completed indicates whether more items were read.</returns>
        protected virtual Task<bool> ReadAsynchronously()
        {
            // We are reading from the fully buffered read stream here; thus it is ok
            // to use synchronous reads and then return a completed task
            // NOTE: once we switch to fully async reading this will have to change
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadImplementation);
        }
#endif

        /// <summary>
        /// Creates a new <see cref="Scope"/> for the specified <paramref name="state"/> and
        /// with the provided <paramref name="item"/> and pushes it on the stack of scopes.
        /// </summary>
        /// <param name="state">The <see cref="ODataCollectionReaderState"/> to use for the new scope.</param>
        /// <param name="item">The item to attach with the state in the new scope.</param>
        protected void EnterScope(ODataCollectionReaderState state, object item)
        {
            this.EnterScope(state, item, false);
        }

        /// <summary>
        /// Creates a new <see cref="Scope"/> for the specified <paramref name="state"/> and
        /// with the provided <paramref name="item"/> and pushes it on the stack of scopes.
        /// </summary>
        /// <param name="state">The <see cref="ODataCollectionReaderState"/> to use for the new scope.</param>
        /// <param name="item">The item to attach with the state in the new scope.</param>
        /// <param name="isCollectionElementEmpty">The state of the collection element - empty or not-empty.</param>
        protected void EnterScope(ODataCollectionReaderState state, object item, bool isCollectionElementEmpty)
        {
            if (state == ODataCollectionReaderState.Value)
            {
                ValidationUtils.ValidateCollectionItem(item, true /* isNullable */);
            }

            this.scopes.Push(new Scope(state, item, isCollectionElementEmpty));
            if (this.listener != null)
            {
                if (state == ODataCollectionReaderState.Exception)
                {
                    this.listener.OnException();
                }
                else if (state == ODataCollectionReaderState.Completed)
                {
                    this.listener.OnCompleted();
                }
            }
        }

        /// <summary>
        /// Replaces the current scope with a new <see cref="Scope"/> with the specified <paramref name="state"/> and
        /// the item of the current scope.
        /// </summary>
        /// <param name="state">The <see cref="ODataCollectionReaderState"/> to use for the new scope.</param>
        /// <param name="item">The item associated with the replacement state.</param>
        protected void ReplaceScope(ODataCollectionReaderState state, object item)
        {
            Debug.Assert(this.scopes.Count > 0, "Stack must always be non-empty.");

            if (state == ODataCollectionReaderState.Value)
            {
                ValidationUtils.ValidateCollectionItem(item, true /* isNullable */);
            }

            this.scopes.Pop();
            this.EnterScope(state, item);
        }

        /// <summary>
        /// Removes the current scope from the stack of all scopes.
        /// </summary>
        /// <param name="state">The expected state of the current scope (to be popped).</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "state", Justification = "Used in debug builds in assertions.")]
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "scope", Justification = "Used in debug builds in assertions.")]
        protected void PopScope(ODataCollectionReaderState state)
        {
            Debug.Assert(this.scopes.Count > 1, "Stack must have more than 1 items in order to pop an item.");

            Scope scope = this.scopes.Pop();
            Debug.Assert(scope.State == state, "scope.State == state");
        }

        /// <summary>
        /// Catch any exception thrown by the action passed in; in the exception case move the reader into
        /// state ExceptionThrown and then rethrow the exception.
        /// </summary>
        /// <typeparam name="T">The type returned from the <paramref name="action"/> to execute.</typeparam>
        /// <param name="action">The action to execute.</param>
        /// <returns>The result of executing the <paramref name="action"/>.</returns>
        private T InterceptException<T>(Func<T> action)
        {
            try
            {
                return action();
            }
            catch (Exception e)
            {
                if (ExceptionUtils.IsCatchableExceptionType(e))
                {
                    this.EnterScope(ODataCollectionReaderState.Exception, null);
                }

                throw;
            }
        }

        /// <summary>
        /// Verifies that calling Read is valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        private void VerifyCanRead(bool synchronousCall)
        {
            this.inputContext.VerifyNotDisposed();
            this.VerifyCallAllowed(synchronousCall);

            if (this.State == ODataCollectionReaderState.Exception || this.State == ODataCollectionReaderState.Completed)
            {
                throw new ODataException(Strings.ODataCollectionReaderCore_ReadOrReadAsyncCalledInInvalidState(this.State));
            }
        }

        /// <summary>
        /// Verifies that a call is allowed to the reader.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        private void VerifyCallAllowed(bool synchronousCall)
        {
            if (synchronousCall)
            {
                this.VerifySynchronousCallAllowed();
            }
            else
            {
#if PORTABLELIB
                this.VerifyAsynchronousCallAllowed();
#else
                Debug.Assert(false, "Async calls are not allowed in this build.");
#endif
            }
        }

        /// <summary>
        /// Verifies that a synchronous operation is allowed on this reader.
        /// </summary>
        private void VerifySynchronousCallAllowed()
        {
            if (!this.inputContext.Synchronous)
            {
                throw new ODataException(Strings.ODataCollectionReaderCore_SyncCallOnAsyncReader);
            }
        }

#if PORTABLELIB
        /// <summary>
        /// Verifies that an asynchronous operation is allowed on this reader.
        /// </summary>
        private void VerifyAsynchronousCallAllowed()
        {
            if (this.inputContext.Synchronous)
            {
                throw new ODataException(Strings.ODataCollectionReaderCore_AsyncCallOnSyncReader);
            }
        }
#endif

        /// <summary>
        /// A collection reader scope; keeping track of the current reader state and an item associated with this state.
        /// </summary>
        protected sealed class Scope
        {
            /// <summary>The reader state of this scope.</summary>
            private readonly ODataCollectionReaderState state;

            /// <summary>The item attached to this scope.</summary>
            private readonly object item;

            /// <summary>True, if the collection element attached to this scope is empty. False otherwise.</summary>
            [SuppressMessage("Microsoft.Performance", "CA1823", Justification = "isCollectionElementEmpty is used in debug.")]
            private readonly bool isCollectionElementEmpty;

            /// <summary>
            /// Constructor creating a new reader scope.
            /// </summary>
            /// <param name="state">The reader state of this scope.</param>
            /// <param name="item">The item attached to this scope.</param>
            [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Debug.Assert check only.")]
            public Scope(ODataCollectionReaderState state, object item) : this(state, item, false)
            {
            }

            /// <summary>
            /// Constructor creating a new reader scope.
            /// </summary>
            /// <param name="state">The reader state of this scope.</param>
            /// <param name="item">The item attached to this scope.</param>
            /// <param name="isCollectionElementEmpty">The state of the collection element - empty or not-empty</param>
            [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Debug.Assert check only.")]
            public Scope(ODataCollectionReaderState state, object item, bool isCollectionElementEmpty)
            {
                Debug.Assert(
                   state == ODataCollectionReaderState.Start && item == null ||
                   state == ODataCollectionReaderState.CollectionStart && item is ODataCollectionStart ||
                   state == ODataCollectionReaderState.Value && (item == null || EdmLibraryExtensions.IsPrimitiveType(item.GetType()) || item is ODataEnumValue) ||
                   state == ODataCollectionReaderState.CollectionEnd && item is ODataCollectionStart ||
                   state == ODataCollectionReaderState.Exception && item == null ||
                   state == ODataCollectionReaderState.Completed && item == null,
                   "Reader state and associated item do not match.");

                this.state = state;
                this.item = item;
                this.isCollectionElementEmpty = isCollectionElementEmpty;


                // When isCollectionElementEmpty is true, Reader needs to be in CollectionStart state.
                Debug.Assert(!this.isCollectionElementEmpty ||
                        (this.isCollectionElementEmpty && state == ODataCollectionReaderState.CollectionStart),
                        "Expected state to be CollectionStart if isCollectionElementyEmpty is true.");
            }

            /// <summary>
            /// The reader state of this scope.
            /// </summary>
            public ODataCollectionReaderState State
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

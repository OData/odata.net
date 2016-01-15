//---------------------------------------------------------------------
// <copyright file="ODataReaderCore.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Evaluation;
    using Microsoft.OData.Core.JsonLight;
    using Microsoft.OData.Core.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Base class for OData readers that verifies a proper sequence of read calls on the reader.
    /// </summary>
    internal abstract class ODataReaderCore : ODataReader
    {
        /// <summary>The input to read the payload from.</summary>
        private readonly ODataInputContext inputContext;

        /// <summary>true if the reader is created for reading a feed; false when it is created for reading an entry.</summary>
        private readonly bool readingFeed;

        /// <summary>true if the reader is created for reading expanded navigation property in delta response; false otherwise.</summary>
        private readonly bool readingDelta;

        /// <summary>Stack of reader scopes to keep track of the current context of the reader.</summary>
        private readonly Stack<Scope> scopes = new Stack<Scope>();

        /// <summary>If not null, the reader will notify the implementer of the interface of relevant state changes in the reader.</summary>
        private readonly IODataReaderWriterListener listener;

        /// <summary>
        /// The <see cref="FeedWithoutExpectedTypeValidator"/> to use for entries in this feed.
        /// Only applies when reading a top-level feed; otherwise null.
        /// </summary>
        private readonly FeedWithoutExpectedTypeValidator feedValidator;

        /// <summary>The number of entries which have been started but not yet ended.</summary>
        private int currentEntryDepth;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputContext">The input to read the payload from.</param>
        /// <param name="readingFeed">true if the reader is created for reading a feed; false when it is created for reading an entry.</param>
        /// <param name="readingDelta">true if the reader is created for reading expanded navigation property in delta response; false otherwise.</param>
        /// <param name="listener">If not null, the reader will notify the implementer of the interface of relevant state changes in the reader.</param>
        protected ODataReaderCore(
            ODataInputContext inputContext,
            bool readingFeed,
            bool readingDelta,
            IODataReaderWriterListener listener)
        {
            Debug.Assert(inputContext != null, "inputContext != null");

            this.inputContext = inputContext;
            this.readingFeed = readingFeed;
            this.readingDelta = readingDelta;
            this.listener = listener;
            this.currentEntryDepth = 0;

            // create a collection validator when reading a top-level feed and a user model is present
            if (this.readingFeed && this.inputContext.Model.IsUserModel())
            {
                this.feedValidator = new FeedWithoutExpectedTypeValidator();
            }
        }

        /// <summary>
        /// The current state of the reader.
        /// </summary>
        public override sealed ODataReaderState State
        {
            get
            {
                this.inputContext.VerifyNotDisposed();
                Debug.Assert(this.scopes != null && this.scopes.Count > 0, "A scope must always exist.");
                return this.scopes.Peek().State;
            }
        }

        /// <summary>
        /// The most recent <see cref="ODataItem"/> that has been read.
        /// </summary>
        public override sealed ODataItem Item
        {
            get
            {
                this.inputContext.VerifyNotDisposed();
                Debug.Assert(this.scopes != null && this.scopes.Count > 0, "A scope must always exist.");
                return this.scopes.Peek().Item;
            }
        }

        /// <summary>
        /// Returns the current item as <see cref="ODataEntry"/>. Must only be called if the item actually is an entry.
        /// </summary>
        protected ODataEntry CurrentEntry
        {
            get
            {
                Debug.Assert(this.Item == null || this.Item is ODataEntry, "this.Item is ODataEntry");
                return (ODataEntry)this.Item;
            }
        }

        /// <summary>
        /// Returns the current item as <see cref="ODataFeed"/>. Must only be called if the item actually is a feed.
        /// </summary>
        protected ODataFeed CurrentFeed
        {
            get
            {
                Debug.Assert(this.Item is ODataFeed, "this.Item is ODataFeed");
                return (ODataFeed)this.Item;
            }
        }

        /// <summary>
        /// Returns the current entry depth.
        /// </summary>
        protected int CurrentEntryDepth
        {
            get
            {
                return this.currentEntryDepth;
            }
        }

        /// <summary>
        /// Returns the current item as <see cref="ODataNavigationLink"/>. Must only be called if the item actually is a navigation link.
        /// </summary>
        protected ODataNavigationLink CurrentNavigationLink
        {
            get
            {
                Debug.Assert(this.Item is ODataNavigationLink, "this.Item is ODataNavigationLink");
                return (ODataNavigationLink)this.Item;
            }
        }

        /// <summary>
        /// Returns the current item as <see cref="ODataEntityReferenceLink"/>. Must only be called if the item actually is an entity reference link.
        /// </summary>
        protected ODataEntityReferenceLink CurrentEntityReferenceLink
        {
            get
            {
                Debug.Assert(this.Item is ODataEntityReferenceLink, "this.Item is ODataEntityReferenceLink");
                return (ODataEntityReferenceLink)this.Item;
            }
        }

        /// <summary>
        /// Returns the expected entity type for the current scope.
        /// </summary>
        protected IEdmEntityType CurrentEntityType
        {
            get
            {
                Debug.Assert(this.scopes != null && this.scopes.Count > 0, "A scope must always exist.");
                IEdmEntityType entityType = this.scopes.Peek().EntityType;
                Debug.Assert(entityType == null || this.inputContext.Model.IsUserModel(), "We can only have entity type if we also have metadata.");
                return entityType;
            }

            set
            {
                this.scopes.Peek().EntityType = value;
            }
        }

        /// <summary>
        /// Returns the navigation source for the current scope.
        /// </summary>
        protected IEdmNavigationSource CurrentNavigationSource
        {
            get
            {
                Debug.Assert(this.scopes != null && this.scopes.Count > 0, "A scope must always exist.");
                IEdmNavigationSource navigationSource = this.scopes.Peek().NavigationSource;
                Debug.Assert(navigationSource == null || this.inputContext.Model.IsUserModel(), "We can only have navigation source if we also have metadata.");
                return navigationSource;
            }
        }

        /// <summary>
        /// Returns the current scope.
        /// </summary>
        protected Scope CurrentScope
        {
            get
            {
                Debug.Assert(this.scopes != null && this.scopes.Count > 0, "A scope must always exist.");
                return this.scopes.Peek();
            }
        }

        /// <summary>
        /// Returns the scope of the entity owning the current link.
        /// </summary>
        protected Scope LinkParentEntityScope
        {
            get
            {
                Debug.Assert(this.scopes != null && this.scopes.Count > 1, "We must have at least two scoped for LinkParentEntityScope to be called.");
                Debug.Assert(this.scopes.Peek().State == ODataReaderState.NavigationLinkStart, "The LinkParentEntityScope can only be accessed when in NavigationLinkStart state.");
                return this.scopes.Skip(1).First();
            }
        }

        /// <summary>
        /// A flag indicating whether the reader is at the top level.
        /// </summary>
        protected bool IsTopLevel
        {
            get
            {
                Debug.Assert(this.scopes != null, "Scopes must exist.");

                // there is the root scope at the top (when the writer has not started or has completed) 
                // and then the top-level scope (the top-level entry/feed item) as the second scope on the stack
                return this.scopes.Count <= 2;
            }
        }

        /// <summary>
        /// If the current scope is a content of an expanded link, this returns the parent navigation link scope, otherwise null.
        /// </summary>
        protected Scope ExpandedLinkContentParentScope
        {
            get
            {
                Debug.Assert(this.scopes != null, "this.scopes != null");
                if (this.scopes.Count > 1)
                {
                    Scope parentScope = this.scopes.Skip(1).First();
                    if (parentScope.State == ODataReaderState.NavigationLinkStart)
                    {
                        return parentScope;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// True if we are reading an entry or feed that is the direct content of an expanded link. Otherwise false.
        /// </summary>
        protected bool IsExpandedLinkContent
        {
            get
            {
                return this.ExpandedLinkContentParentScope != null;
            }
        }

        /// <summary>
        /// Set to true if a feed is being read.
        /// </summary>
        protected bool ReadingFeed
        {
            get
            {
                return this.readingFeed;
            }
        }

        /// <summary>
        /// Returns true if we are reading a nested payload,
        /// e.g. an expanded entry or feed within a delta payload, 
        /// or an entry or a feed within a parameters payload.
        /// </summary>
        protected bool IsReadingNestedPayload
        {
            get
            {
                return this.readingDelta || this.listener != null;
            }
        }

        /// <summary>
        /// Validator to validate consistency of entries in top-level feeds.
        /// </summary>
        /// <remarks>We only use this for top-level feeds since we support collection validation for 
        /// feeds only when metadata is available and in these cases we already validate the 
        /// types of the entries in nested feeds.</remarks>
        protected FeedWithoutExpectedTypeValidator CurrentFeedValidator
        {
            get
            {
                Debug.Assert(this.State == ODataReaderState.EntryStart, "CurrentFeedValidator should only be called while reading an entry.");

                // Only return the collection validator for entries in top-level feeds
                return this.scopes.Count == 3 ? this.feedValidator : null;
            }
        }

        /// <summary>
        /// Reads the next <see cref="ODataItem"/> from the message payload.
        /// </summary>
        /// <returns>true if more items were read; otherwise false.</returns>
        public override sealed bool Read()
        {
            this.VerifyCanRead(true);
            return this.InterceptException(this.ReadSynchronously);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously reads the next <see cref="ODataItem"/> from the message payload.
        /// </summary>
        /// <returns>A task that when completed indicates whether more items were read.</returns>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
        public override sealed Task<bool> ReadAsync()
        {
            this.VerifyCanRead(false);
            return this.ReadAsynchronously().FollowOnFaultWith(t => this.EnterScope(new Scope(ODataReaderState.Exception, null, null, null, null)));
        }
#endif

        /// <summary>
        /// Implementation of the reader logic when in state 'Start'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        protected abstract bool ReadAtStartImplementation();

        /// <summary>
        /// Implementation of the reader logic when in state 'FeedStart'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        protected abstract bool ReadAtFeedStartImplementation();

        /// <summary>
        /// Implementation of the reader logic when in state 'FeedEnd'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        protected abstract bool ReadAtFeedEndImplementation();

        /// <summary>
        /// Implementation of the reader logic when in state 'EntryStart'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        protected abstract bool ReadAtEntryStartImplementation();

        /// <summary>
        /// Implementation of the reader logic when in state 'EntryEnd'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        protected abstract bool ReadAtEntryEndImplementation();

        /// <summary>
        /// Implementation of the reader logic when in state 'NavigationLinkStart'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        protected abstract bool ReadAtNavigationLinkStartImplementation();

        /// <summary>
        /// Implementation of the reader logic when in state 'NavigationLinkEnd'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        protected abstract bool ReadAtNavigationLinkEndImplementation();

        /// <summary>
        /// Implementation of the reader logic when in state 'EntityReferenceLink'.
        /// </summary>
        /// <returns>true if more items can be read from the reader; otherwise false.</returns>
        protected abstract bool ReadAtEntityReferenceLink();

        /// <summary>
        /// Pushes the <paramref name="scope"/> on the stack of scopes.
        /// </summary>
        /// <param name="scope">The scope to enter.</param>
        protected void EnterScope(Scope scope)
        {
            Debug.Assert(scope != null, "scope != null");

            // TODO: implement some basic validation that the transitions are ok
            this.scopes.Push(scope);
            if (this.listener != null)
            {
                if (scope.State == ODataReaderState.Exception)
                {
                    this.listener.OnException();
                }
                else if (scope.State == ODataReaderState.Completed)
                {
                    this.listener.OnCompleted();
                }
            }
        }

        /// <summary>
        /// Replaces the current scope with the specified <paramref name="scope"/>.
        /// </summary>
        /// <param name="scope">The scope to replace the current scope with.</param>
        protected void ReplaceScope(Scope scope)
        {
            Debug.Assert(this.scopes.Count > 0, "Stack must always be non-empty.");
            Debug.Assert(scope != null, "scope != null");
            Debug.Assert(scope.State != ODataReaderState.EntryEnd, "Call EndEntry instead.");

            // TODO: implement some basic validation that the transitions are ok
            this.scopes.Pop();
            this.EnterScope(scope);
        }

        /// <summary>
        /// Removes the current scope from the stack of all scopes.
        /// </summary>
        /// <param name="state">The expected state of the current scope (to be popped).</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "state", Justification = "Used in debug builds in assertions.")]
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "scope", Justification = "Used in debug builds in assertions.")]
        protected void PopScope(ODataReaderState state)
        {
            Debug.Assert(this.scopes.Count > 1, "Stack must have more than 1 items in order to pop an item.");

            Scope scope = this.scopes.Pop();
            Debug.Assert(scope.State == state, "scope.State == state");
        }

        /// <summary>
        /// Called to transition into the EntryEnd state.
        /// </summary>
        /// <param name="scope">The scope for the EntryEnd state.</param>
        protected void EndEntry(Scope scope)
        {
            Debug.Assert(this.scopes.Count > 0, "Stack must always be non-empty.");
            Debug.Assert(scope != null, "scope != null");
            Debug.Assert(scope.State == ODataReaderState.EntryEnd, "scope.State == ODataReaderState.EntryEnd");

            this.scopes.Pop();
            this.EnterScope(scope);
        }

        /// <summary>
        /// If an entity type name is found in the payload this method is called to apply it to the current scope.
        /// This method should be called even if the type name was not found in which case a null should be passed in.
        /// The method validates that some type will be available as the current entity type after it returns (if we are parsing using metadata).
        /// </summary>
        /// <param name="entityTypeNameFromPayload">The entity type name found in the payload or null if no type was specified in the payload.</param>
        protected void ApplyEntityTypeNameFromPayload(string entityTypeNameFromPayload)
        {
            Debug.Assert(
                this.scopes.Count > 0 && this.scopes.Peek().Item is ODataEntry,
                "Entity type can be applied only when in entry scope.");

            SerializationTypeNameAnnotation serializationTypeNameAnnotation;
            EdmTypeKind targetTypeKind;
            IEdmEntityTypeReference targetEntityTypeReference =
                (IEdmEntityTypeReference)ReaderValidationUtils.ResolvePayloadTypeNameAndComputeTargetType(
                    EdmTypeKind.Entity,
                    /*defaultPrimitivePayloadType*/ null,
                    this.CurrentEntityType.ToTypeReference(),
                    entityTypeNameFromPayload,
                    this.inputContext.Model,
                    this.inputContext.MessageReaderSettings,
                    () => EdmTypeKind.Entity,
                    out targetTypeKind,
                    out serializationTypeNameAnnotation);

            IEdmEntityType targetEntityType = null;
            ODataEntry entry = this.CurrentEntry;
            if (targetEntityTypeReference != null)
            {
                targetEntityType = targetEntityTypeReference.EntityDefinition();
                entry.TypeName = targetEntityType.FullTypeName();

                if (serializationTypeNameAnnotation != null)
                {
                    entry.SetAnnotation(serializationTypeNameAnnotation);
                }
            }
            else if (entityTypeNameFromPayload != null)
            {
                entry.TypeName = entityTypeNameFromPayload;
            }

            // Set the current entity type since the type from payload might be more derived than
            // the expected one.
            this.CurrentEntityType = targetEntityType;
        }

        /// <summary>
        /// Reads the next <see cref="ODataItem"/> from the message payload.
        /// </summary>
        /// <returns>true if more items were read; otherwise false.</returns>
        protected bool ReadSynchronously()
        {
            return this.ReadImplementation();
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously reads the next <see cref="ODataItem"/> from the message payload.
        /// </summary>
        /// <returns>A task that when completed indicates whether more items were read.</returns>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
        protected virtual Task<bool> ReadAsynchronously()
        {
            // We are reading from the fully buffered read stream here; thus it is ok
            // to use synchronous reads and then return a completed task
            // NOTE: once we switch to fully async reading this will have to change
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadImplementation);
        }
#endif

        /// <summary>
        /// Increments the nested entry count by one and fails if the new value exceeds the maxiumum nested entry depth limit.
        /// </summary>
        protected void IncreaseEntryDepth()
        {
            this.currentEntryDepth++;

            if (this.currentEntryDepth > this.inputContext.MessageReaderSettings.MessageQuotas.MaxNestingDepth)
            {
                throw new ODataException(Strings.ValidationUtils_MaxDepthOfNestedEntriesExceeded(this.inputContext.MessageReaderSettings.MessageQuotas.MaxNestingDepth));
            }
        }

        /// <summary>
        /// Decrements the nested entry count by one.
        /// </summary>
        protected void DecreaseEntryDepth()
        {
            Debug.Assert(this.currentEntryDepth > 0, "Entry depth should never become negative.");

            this.currentEntryDepth--;
        }

        /// <summary>
        /// Reads the next <see cref="ODataItem"/> from the message payload.
        /// </summary>
        /// <returns>true if more items were read; otherwise false.</returns>
        private bool ReadImplementation()
        {
            bool result;
            switch (this.State)
            {
                case ODataReaderState.Start:
                    result = this.ReadAtStartImplementation();
                    break;

                case ODataReaderState.FeedStart:
                    result = this.ReadAtFeedStartImplementation();
                    break;

                case ODataReaderState.FeedEnd:
                    result = this.ReadAtFeedEndImplementation();
                    break;

                case ODataReaderState.EntryStart:
                    this.IncreaseEntryDepth();
                    result = this.ReadAtEntryStartImplementation();
                    break;

                case ODataReaderState.EntryEnd:
                    this.DecreaseEntryDepth();
                    result = this.ReadAtEntryEndImplementation();
                    break;

                case ODataReaderState.NavigationLinkStart:
                    result = this.ReadAtNavigationLinkStartImplementation();
                    break;

                case ODataReaderState.NavigationLinkEnd:
                    result = this.ReadAtNavigationLinkEndImplementation();
                    break;

                case ODataReaderState.EntityReferenceLink:
                    result = this.ReadAtEntityReferenceLink();
                    break;

                case ODataReaderState.Exception:    // fall through
                case ODataReaderState.Completed:
                    throw new ODataException(Strings.ODataReaderCore_NoReadCallsAllowed(this.State));

                default:
                    Debug.Assert(false, "Unsupported reader state " + this.State + " detected.");
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataReaderCore_ReadImplementation));
            }

            if ((this.State == ODataReaderState.EntryStart || this.State == ODataReaderState.EntryEnd) && this.Item != null)
            {
                ReaderValidationUtils.ValidateEntry(this.CurrentEntry);
            }

            return result;
        }

        /// <summary>
        /// Catch any exception thrown by the action passed in; in the exception case move the reader into
        /// state ExceptionThrown and then rethrow the exception.
        /// </summary>
        /// <typeparam name="T">The type returned from the <paramref name="action"/> to execute.</typeparam>
        /// <param name="action">The action to execute.</param>
        /// <returns>The result of executing the <paramref name="action"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("DataWeb.Usage", "AC0014", Justification = "Throws every time")]
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
                    this.EnterScope(new Scope(ODataReaderState.Exception, null, null, null, null));
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

            if (this.State == ODataReaderState.Exception || this.State == ODataReaderState.Completed)
            {
                throw new ODataException(Strings.ODataReaderCore_ReadOrReadAsyncCalledInInvalidState(this.State));
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
                if (!this.inputContext.Synchronous)
                {
                    throw new ODataException(Strings.ODataReaderCore_SyncCallOnAsyncReader);
                }
            }
            else
            {
#if ODATALIB_ASYNC
                if (this.inputContext.Synchronous)
                {
                    throw new ODataException(Strings.ODataReaderCore_AsyncCallOnSyncReader);
                }
#else
                Debug.Assert(false, "Async calls are not allowed in this build.");
#endif
            }
        }

        /// <summary>
        /// A reader scope; keeping track of the current reader state and an item associated with this state.
        /// </summary>
        protected internal class Scope
        {
            /// <summary>The reader state of this scope.</summary>
            private readonly ODataReaderState state;

            /// <summary>The item attached to this scope.</summary>
            private readonly ODataItem item;

            /// <summary>The odataUri parsed based on the context uri attached to this scope.</summary>
            private readonly ODataUri odataUri;

            /// <summary>
            /// Constructor creating a new reader scope.
            /// </summary>
            /// <param name="state">The reader state of this scope.</param>
            /// <param name="item">The item attached to this scope.</param>
            /// <param name="navigationSource">The navigation source we are going to read entities for.</param>
            /// <param name="expectedEntityType">The expected entity type for the scope.</param>
            /// <param name="odataUri">The odataUri parsed based on the context uri for current scope</param>
            /// <remarks>The <paramref name="expectedEntityType"/> has the following meanings for given state:
            /// Start -               it's the expected base type of the top-level entry or entries in the top-level feed.
            /// FeedStart -           it's the expected base type of the entries in the feed.
            ///                       note that it might be a more derived type than the base type of the entity set for the feed.
            /// EntryStart -          it's the expected base type of the entry. If the entry has no type name specified
            ///                       this type will be assumed. Otherwise the specified type name must be
            ///                       the expected type or a more derived type.
            /// NavigationLinkStart - it's the expected base type the entries in the expanded link (either the single entry
            ///                       or entries in the expanded feed).
            /// EntityReferenceLink - it's null, no need for types on entity reference links.
            /// In all cases the specified type must be an entity type.</remarks>
            [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Debug.Assert check only.")]
            internal Scope(ODataReaderState state, ODataItem item, IEdmNavigationSource navigationSource, IEdmEntityType expectedEntityType, ODataUri odataUri)
            {
                Debug.Assert(
                    state == ODataReaderState.Exception && item == null ||
                    state == ODataReaderState.EntryStart && (item == null || item is ODataEntry) ||
                    state == ODataReaderState.EntryEnd && (item == null || item is ODataEntry) ||
                    state == ODataReaderState.FeedStart && item is ODataFeed ||
                    state == ODataReaderState.FeedEnd && item is ODataFeed ||
                    state == ODataReaderState.NavigationLinkStart && item is ODataNavigationLink ||
                    state == ODataReaderState.NavigationLinkEnd && item is ODataNavigationLink ||
                    state == ODataReaderState.EntityReferenceLink && item is ODataEntityReferenceLink ||
                    state == ODataReaderState.Start && item == null ||
                    state == ODataReaderState.Completed && item == null,
                    "Reader state and associated item do not match.");

                this.state = state;
                this.item = item;
                this.EntityType = expectedEntityType;
                this.NavigationSource = navigationSource;
                this.odataUri = odataUri;
            }

            /// <summary>
            /// The reader state of this scope.
            /// </summary>
            internal ODataReaderState State
            {
                get
                {
                    return this.state;
                }
            }

            /// <summary>
            /// The item attached to this scope.
            /// </summary>
            internal ODataItem Item
            {
                get
                {
                    return this.item;
                }
            }

            /// <summary>
            /// The odataUri parsed based on the context url to this scope.
            /// </summary>
            internal ODataUri ODataUri
            {
                get
                {
                    return this.odataUri;
                }
            }

            /// <summary>
            /// The navigation source we are reading entries from (possibly null).
            /// </summary>
            internal IEdmNavigationSource NavigationSource { get; set; }

            /// <summary>
            /// The entity type for this scope. Can be either the expected one if the real one
            /// was not found yet, or the one specified in the payload itself (the real one).
            /// </summary>
            internal IEdmEntityType EntityType { get; set; }
        }
    }
}

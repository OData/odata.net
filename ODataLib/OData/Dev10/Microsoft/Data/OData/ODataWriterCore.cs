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

namespace Microsoft.Data.OData
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
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Base class for OData writers that verifies a proper sequence of write calls on the writer.
    /// </summary>
    internal abstract class ODataWriterCore : ODataWriter, IODataOutputInStreamErrorListener
    {
        /// <summary>The output context to write to.</summary>
        private readonly ODataOutputContext outputContext;

        /// <summary>True if the writer was created for writing a feed; false when it was created for writing an entry.</summary>
        private readonly bool writingFeed;

        /// <summary>
        /// The <see cref="FeedWithoutExpectedTypeValidator"/> to use for entries in this feed.
        /// Only applies when writing a top-level feed; otherwise null.
        /// </summary>
        private readonly FeedWithoutExpectedTypeValidator feedValidator;

        /// <summary>The number of entries which have been started but not yet ended.</summary>
        private int currentEntryDepth;

        /// <summary>Stack of writer scopes to keep track of the current context of the writer.</summary>
        private Stack<Scope> scopes = new Stack<Scope>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="outputContext">The output context to write to.</param>
        /// <param name="writingFeed">True if the writer is created for writing a feed; false when it is created for writing an entry.</param>
        protected ODataWriterCore(ODataOutputContext outputContext, bool writingFeed)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(outputContext != null, "outputContext != null");

            this.outputContext = outputContext;
            this.writingFeed = writingFeed;

            // create a collection validator when writing a top-level feed and a user model is present
            if (this.writingFeed && this.outputContext.Model.IsUserModel())
            {
                this.feedValidator = new FeedWithoutExpectedTypeValidator();
            }

            this.scopes.Push(new Scope(WriterState.Start, null, false));
        }

        /// <summary>
        /// An enumeration representing the current state of the writer.
        /// </summary>
        internal enum WriterState
        {
            /// <summary>The writer is at the start; nothing has been written yet.</summary>
            Start,

            /// <summary>The writer is currently writing an entry.</summary>
            Entry,

            /// <summary>The writer is currently writing a feed.</summary>
            Feed,

            /// <summary>The writer is currently writing a navigation link (possibly an expanded link but we don't know yet).</summary>
            /// <remarks>
            /// This state is used when a navigation link was started but we didn't see any children for it yet.
            /// </remarks>
            NavigationLink,

            /// <summary>The writer is currently writing a navigation link with content.</summary>
            /// <remarks>
            /// This state is used when a navigation link with either an entity reference link or expanded feed/entry was written.
            /// </remarks>
            NavigationLinkWithContent,

            /// <summary>The writer has completed; nothing can be written anymore.</summary>
            Completed,

            /// <summary>The writer is in error state; nothing can be written anymore.</summary>
            Error
        }

        /// <summary>
        /// The current scope for the writer.
        /// </summary>
        protected Scope CurrentScope
        {
            get
            {
                Debug.Assert(this.scopes.Count > 0, "We should have at least one active scope all the time.");
                return this.scopes.Peek();
            }
        }

        /// <summary>
        /// The current state of the writer.
        /// </summary>
        protected WriterState State
        {
            get
            {
                return this.CurrentScope.State;
            }
        }

        /// <summary>
        /// true if the writer should not write any input specified and should just skip it.
        /// </summary>
        protected bool SkipWriting
        {
            get
            {
                return this.CurrentScope.SkipWriting;
            }
        }

        /// <summary>
        /// A flag indicating whether the writer is at the top level.
        /// </summary>
        protected bool IsTopLevel
        {
            get
            {
                Debug.Assert(this.State != WriterState.Start && this.State != WriterState.Completed, "IsTopLevel should only be called while writing the payload.");

                // there is the root scope at the top (when the writer has not started or has completed) 
                // and then the top-level scope (the top-level entry/feed item) as the second scope on the stack
                return this.scopes.Count == 2;
            }
        }

        /// <summary>
        /// Returns the immediate parent link which is being expanded, or null if no such link exists
        /// </summary>
        protected ODataNavigationLink ParentNavigationLink
        {
            get
            {
                Debug.Assert(this.State == WriterState.Entry || this.State == WriterState.Feed, "ParentNavigationLink should only be called while writing an entry or a feed.");

                Scope linkScope = this.scopes.Skip(1).FirstOrDefault();
                return linkScope == null ? null : (linkScope.Item as ODataNavigationLink);
            }
        }

        /// <summary>
        /// Returns the entity type of the immediate parent entry for which a navigation link is being written.
        /// </summary>
        protected IEdmEntityType ParentEntryEntityType
        {
            get
            {
                Debug.Assert(
                    this.State == WriterState.NavigationLink || this.State == WriterState.NavigationLinkWithContent, 
                    "ParentEntryEntityType should only be called while writing a navigation link (with or without content).");
                EntryScope entryScope = (EntryScope)this.scopes.Skip(1).First();
                return entryScope.EntityType;
            }
        }

        /// <summary>
        /// Returns the number of entries seen so far on the current feed scope.
        /// </summary>
        /// <remarks>Can only be accessed on a feed scope.</remarks>
        protected int FeedScopeEntryCount
        {
            get
            {
                Debug.Assert(this.State == WriterState.Feed, "FeedScopeEntryCount should only be called while writing a feed.");
                return ((FeedScope)this.CurrentScope).EntryCount;
            }
        }

        /// <summary>
        /// Checker to detect duplicate property names.
        /// </summary>
        protected DuplicatePropertyNamesChecker DuplicatePropertyNamesChecker
        {
            get
            {
                Debug.Assert(
                    this.State == WriterState.Entry || this.State == WriterState.NavigationLink || this.State == WriterState.NavigationLinkWithContent, 
                    "DuplicatePropertyNamesChecker should only be called while writing an entry or an (expanded or deferred) navigation link.");

                EntryScope entryScope;
                switch (this.State)
                {
                    case WriterState.Entry:
                        entryScope = (EntryScope)this.CurrentScope;
                        break;
                    case WriterState.NavigationLink:
                    case WriterState.NavigationLinkWithContent:
                        entryScope = (EntryScope)this.scopes.Skip(1).First();
                        break;
                    default:
                        throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataWriterCore_DuplicatePropertyNamesChecker));
                }

                return entryScope.DuplicatePropertyNamesChecker;
            }
        }

        /// <summary>
        /// The entity type of the current entry.
        /// </summary>
        protected IEdmEntityType EntryEntityType
        {
            get
            {
                Debug.Assert(this.State == WriterState.Entry, "EntryEntityType should only be called while writing an entry.");
                return ((EntryScope)this.CurrentScope).EntityType;
            }
        }

        /// <summary>
        /// Returns the parent navigation link scope of an entry in an expanded link (if it exists).
        /// The entry can either be the content of the expanded link directly or nested inside a feed.
        /// </summary>
        /// <returns>The parent navigation scope of an entry in an expanded link (if it exists).</returns>
        private NavigationLinkScope ParentNavigationLinkScope
        {
            get
            {
                Debug.Assert(this.State == WriterState.Entry, "ParentNavigationLinkScope should only be called while writing an entry.");
                Debug.Assert(this.scopes.Count >= 2, "We should have at least the entry scope and the start scope on the stack.");

                IEnumerable<Scope> parentScopes = this.scopes.Skip(1);
                Scope parentScope = parentScopes.First();
                if (parentScope.State == WriterState.Start)
                {
                    // Top-level entry.
                    return null;
                }

                if (parentScope.State == WriterState.Feed)
                {
                    Debug.Assert(this.scopes.Count >= 3, "We should have at least the entry scope, the feed scope and the start scope on the stack.");

                    // Get the feed's parent (if any)
                    parentScope = parentScopes.Skip(1).First();
                    if (parentScope.State == WriterState.Start)
                    {
                        // Top-level feed.
                        return null;
                    }
                }

                if (parentScope.State == WriterState.NavigationLinkWithContent)
                {
                    // Get the scope of the navigation link
                    return (NavigationLinkScope)parentScope;
                }

                // The parent scope of an entry can only be a feed or an expanded nav link
                throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataWriterCore_ParentNavigationLinkScope));
            }
        }

        /// <summary>
        /// Validator to validate consistency of collection items (or null if no such validator applies to the current scope).
        /// </summary>
        private FeedWithoutExpectedTypeValidator CurrentFeedValidator
        {
            get
            {
                Debug.Assert(this.State == WriterState.Entry, "CurrentCollectionValidator should only be called while writing an entry.");

                // Only return the collection validator for entries in top-level feeds
                return this.scopes.Count == 3 ? this.feedValidator : null;
            }
        }

        /// <summary>
        /// Flushes the write buffer to the underlying stream.
        /// </summary>
        public sealed override void Flush()
        {
            this.VerifyCanFlush(true);

            // Make sure we switch to writer state FatalExceptionThrown if an exception is thrown during flushing.
            try
            {
                this.FlushSynchronously();
            }
            catch
            {
                this.EnterScope(WriterState.Error, null);
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
            this.VerifyCanFlush(false);

            // Make sure we switch to writer state Error if an exception is thrown during flushing.
            return this.FlushAsynchronously().FollowOnFaultWith(t => this.EnterScope(WriterState.Error, null));
        }
#endif

        /// <summary>
        /// Start writing a feed.
        /// </summary>
        /// <param name="feed">Feed/collection to write.</param>
        public sealed override void WriteStart(ODataFeed feed)
        {
            this.VerifyCanWriteStartFeed(true, feed);
            this.WriteStartFeedImplementation(feed);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously start writing a feed.
        /// </summary>
        /// <param name="feed">Feed/collection to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public sealed override Task WriteStartAsync(ODataFeed feed)
        {
            this.VerifyCanWriteStartFeed(false, feed);
            return TaskUtils.GetTaskForSynchronousOperation(() => this.WriteStartFeedImplementation(feed));
        }
#endif

        /// <summary>
        /// Start writing an entry.
        /// </summary>
        /// <param name="entry">Entry/item to write.</param>
        public sealed override void WriteStart(ODataEntry entry)
        {
            this.VerifyCanWriteStartEntry(true, entry);
            this.WriteStartEntryImplementation(entry);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously start writing an entry.
        /// </summary>
        /// <param name="entry">Entry/item to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public sealed override Task WriteStartAsync(ODataEntry entry)
        {
            this.VerifyCanWriteStartEntry(false, entry);
            return TaskUtils.GetTaskForSynchronousOperation(() => this.WriteStartEntryImplementation(entry));
        }
#endif

        /// <summary>
        /// Start writing a navigation link.
        /// </summary>
        /// <param name="navigationLink">Navigation link to write.</param>
        public sealed override void WriteStart(ODataNavigationLink navigationLink)
        {
            this.VerifyCanWriteStartNavigationLink(true, navigationLink);
            this.WriteStartNavigationLinkImplementation(navigationLink);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously start writing a navigation link.
        /// </summary>
        /// <param name="navigationLink">Navigation link to writer.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public sealed override Task WriteStartAsync(ODataNavigationLink navigationLink)
        {
            this.VerifyCanWriteStartNavigationLink(false, navigationLink);
            return TaskUtils.GetTaskForSynchronousOperation(() => this.WriteStartNavigationLinkImplementation(navigationLink));
        }
#endif

        /// <summary>
        /// Finish writing a feed/entry/navigation link.
        /// </summary>
        public sealed override void WriteEnd()
        {
            this.VerifyCanWriteEnd(true);
            this.WriteEndImplementation();
            if (this.CurrentScope.State == WriterState.Completed)
            {
                // Note that we intentionally go through the public API so that if the Flush fails the writer moves to the Error state.
                this.Flush();
            }
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously finish writing a feed/entry/navigation link.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public sealed override Task WriteEndAsync()
        {
            this.VerifyCanWriteEnd(false);
            return TaskUtils.GetTaskForSynchronousOperation(this.WriteEndImplementation)
                .FollowOnSuccessWithTask(
                    task =>
                    {
                        if (this.CurrentScope.State == WriterState.Completed)
                        {
                            // Note that we intentionally go through the public API so that if the Flush fails the writer moves to the Error state.
                            return this.FlushAsync();
                        }
                        else
                        {
                            return TaskUtils.CompletedTask;
                        }
                    });
        }
#endif

        /// <summary>
        /// Writes an entity reference link, which is used to represent binding to an existing resource in a request payload.
        /// </summary>
        /// <param name="entityReferenceLink">The entity reference link to write.</param>
        /// <remarks>
        /// This method can only be called for writing request messages. The entity reference link must be surrounded
        /// by a navigation link written through WriteStart/WriteEnd.
        /// The <see cref="ODataNavigationLink.Url"/> will be ignored in that case and the Uri from the <see cref="ODataEntityReferenceLink.Url"/> will be used
        /// as the binding URL to be written.
        /// </remarks>
        public sealed override void WriteEntityReferenceLink(ODataEntityReferenceLink entityReferenceLink)
        {
            this.VerifyCanWriteEntityReferenceLink(entityReferenceLink, true);
            this.WriteEntityReferenceLinkImplementation(entityReferenceLink);
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously writes an entity reference link, which is used to represent binding to an existing resource in a request payload.
        /// </summary>
        /// <param name="entityReferenceLink">The entity reference link to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        /// <remarks>
        /// This method can only be called for writing request messages. The entity reference link must be surrounded
        /// by a navigation link written through WriteStart/WriteEnd.
        /// The <see cref="ODataNavigationLink.Url"/> will be ignored in that case and the Uri from the <see cref="ODataEntityReferenceLink.Url"/> will be used
        /// as the binding URL to be written.
        /// </remarks>
        public sealed override Task WriteEntityReferenceLinkAsync(ODataEntityReferenceLink entityReferenceLink)
        {
            this.VerifyCanWriteEntityReferenceLink(entityReferenceLink, false);
            return TaskUtils.GetTaskForSynchronousOperation(() => this.WriteEntityReferenceLinkImplementation(entityReferenceLink));
        }
#endif

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
            if (this.State == WriterState.Completed)
            {
                throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromCompleted(this.State.ToString(), WriterState.Error.ToString()));
            }

            this.StartPayloadInStartState();
            this.EnterScope(WriterState.Error, this.CurrentScope.Item);
        }

        /// <summary>
        /// Determines whether a given writer state is considered an error state.
        /// </summary>
        /// <param name="state">The writer state to check.</param>
        /// <returns>True if the writer state is an error state; otherwise false.</returns>
        protected static bool IsErrorState(WriterState state)
        {
            return state == WriterState.Error;
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

#if ODATALIB_ASYNC
        /// <summary>
        /// Flush the output.
        /// </summary>
        /// <returns>Task representing the pending flush operation.</returns>
        protected abstract Task FlushAsynchronously();
#endif

        /// <summary>
        /// Start writing an OData payload.
        /// </summary>
        protected abstract void StartPayload();

        /// <summary>
        /// Start writing an entry.
        /// </summary>
        /// <param name="entry">The entry to write.</param>
        protected abstract void StartEntry(ODataEntry entry);

        /// <summary>
        /// Finish writing an entry.
        /// </summary>
        /// <param name="entry">The entry to write.</param>
        protected abstract void EndEntry(ODataEntry entry);

        /// <summary>
        /// Start writing a feed.
        /// </summary>
        /// <param name="feed">The feed to write.</param>
        protected abstract void StartFeed(ODataFeed feed);

        /// <summary>
        /// Finish writing an OData payload.
        /// </summary>
        protected abstract void EndPayload();

        /// <summary>
        /// Finish writing a feed.
        /// </summary>
        /// <param name="feed">The feed to write.</param>
        protected abstract void EndFeed(ODataFeed feed);
        
        /// <summary>
        /// Write a deferred (non-expanded) navigation link.
        /// </summary>
        /// <param name="navigationLink">The navigation link to write.</param>
        protected abstract void WriteDeferredNavigationLink(ODataNavigationLink navigationLink);

        /// <summary>
        /// Start writing a navigation link with content.
        /// </summary>
        /// <param name="navigationLink">The navigation link to write.</param>
        protected abstract void StartNavigationLinkWithContent(ODataNavigationLink navigationLink);

        /// <summary>
        /// Finish writing a navigation link with content.
        /// </summary>
        /// <param name="navigationLink">The navigation link to write.</param>
        protected abstract void EndNavigationLinkWithContent(ODataNavigationLink navigationLink);

        /// <summary>
        /// Write an entity reference link into a navigation link content.
        /// </summary>
        /// <param name="parentNavigationLink">The parent navigation link which is being written around the entity reference link.</param>
        /// <param name="entityReferenceLink">The entity reference link to write.</param>
        protected abstract void WriteEntityReferenceInNavigationLinkContent(ODataNavigationLink parentNavigationLink, ODataEntityReferenceLink entityReferenceLink);

        /// <summary>
        /// Create a new feed scope.
        /// </summary>
        /// <param name="feed">The feed for the new scope.</param>
        /// <param name="skipWriting">true if the content of the scope to create should not be written.</param>
        /// <returns>The newly create scope.</returns>
        protected abstract FeedScope CreateFeedScope(ODataFeed feed, bool skipWriting);

        /// <summary>
        /// Create a new entry scope.
        /// </summary>
        /// <param name="entry">The entry for the new scope.</param>
        /// <param name="skipWriting">true if the content of the scope to create should not be written.</param>
        /// <returns>The newly create scope.</returns>
        protected abstract EntryScope CreateEntryScope(ODataEntry entry, bool skipWriting);

        /// <summary>
        /// Verifies that calling WriteStart feed is valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        /// <param name="feed">Feed/collection to write.</param>
        private void VerifyCanWriteStartFeed(bool synchronousCall, ODataFeed feed)
        {
            ExceptionUtils.CheckArgumentNotNull(feed, "feed");

            this.VerifyNotDisposed();
            this.VerifyCallAllowed(synchronousCall);
            this.StartPayloadInStartState();
        }

        /// <summary>
        /// Start writing a feed - implementation of the actual functionality.
        /// </summary>
        /// <param name="feed">The feed to write.</param>
        private void WriteStartFeedImplementation(ODataFeed feed)
        {
            this.CheckForNavigationLinkWithContent(ODataPayloadKind.Feed);
            this.EnterScope(WriterState.Feed, feed);

            if (!this.SkipWriting)
            {
                this.InterceptException(() =>
                {
                    WriterValidationUtils.ValidateFeedAtStart(feed);

                    // Verify inline count
                    if (feed.Count.HasValue)
                    {
                        // Check that Count is not set for expanded links
                        if (!this.IsTopLevel)
                        {
                            throw new ODataException(Strings.ODataWriterCore_OnlyTopLevelFeedsSupportInlineCount);
                        }

                        // Check that Count is not set for requests
                        if (!this.outputContext.WritingResponse)
                        {
                            this.ThrowODataException(Strings.ODataWriterCore_InlineCountInRequest, feed);
                        }

                        // Verify version requirements
                        ODataVersionChecker.CheckCount(this.outputContext.Version);
                    }

                    this.StartFeed(feed);
                });
            }
        }

        /// <summary>
        /// Verifies that calling WriteStart entry is valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        /// <param name="entry">Entry/item to write.</param>
        private void VerifyCanWriteStartEntry(bool synchronousCall, ODataEntry entry)
        {
            this.VerifyNotDisposed();
            this.VerifyCallAllowed(synchronousCall);

            if (this.State != WriterState.NavigationLink)
            {
                ExceptionUtils.CheckArgumentNotNull(entry, "entry");
            }
        }

        /// <summary>
        /// Start writing an entry - implementation of the actual functionality.
        /// </summary>
        /// <param name="entry">Entry/item to write.</param>
        private void WriteStartEntryImplementation(ODataEntry entry)
        {
            this.StartPayloadInStartState();
            this.CheckForNavigationLinkWithContent(ODataPayloadKind.Entry);
            this.EnterScope(WriterState.Entry, entry);
            if (!this.SkipWriting)
            {
                this.IncreaseEntryDepth();
                this.InterceptException(() =>
                {
                    if (entry != null)
                    {
                        IEdmEntityType entityType = WriterValidationUtils.ValidateEntityTypeName(this.outputContext.Model, entry.TypeName);

                        // By default validate media resource
                        // In WCF DS Server mode, validate media resource (in writers)
                        // In WCF DS Client mode, do not validate media resource
                        bool validateMediaResource = this.outputContext.UseDefaultFormatBehavior || this.outputContext.UseServerFormatBehavior;
                        ValidationUtils.ValidateEntryMetadata(entry, entityType, this.outputContext.Model, validateMediaResource);

                        NavigationLinkScope parentNavigationLinkScope = this.ParentNavigationLinkScope;
                        if (parentNavigationLinkScope != null)
                        {
                            WriterValidationUtils.ValidateEntryInExpandedLink(entityType, parentNavigationLinkScope.NavigationPropertyType);
                        }

                        // Validate the consistenty of entity types in feeds
                        if (this.CurrentFeedValidator != null)
                        {
                            this.CurrentFeedValidator.ValidateEntry(entityType);
                        }

                        ((EntryScope)this.CurrentScope).EntityType = entityType;

                        WriterValidationUtils.ValidateEntryAtStart(entry);
                    }

                    this.StartEntry(entry);
                });
            }
        }

        /// <summary>
        /// Verifies that calling WriteStart navigation link is valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        /// <param name="navigationLink">Navigation link to write.</param>
        private void VerifyCanWriteStartNavigationLink(bool synchronousCall, ODataNavigationLink navigationLink)
        {
            ExceptionUtils.CheckArgumentNotNull(navigationLink, "navigationLink");

            this.VerifyNotDisposed();
            this.VerifyCallAllowed(synchronousCall);
        }

        /// <summary>
        /// Start writing a navigation link - implementation of the actual functionality.
        /// </summary>
        /// <param name="navigationLink">Navigation link to write.</param>
        private void WriteStartNavigationLinkImplementation(ODataNavigationLink navigationLink)
        {
            this.EnterScope(WriterState.NavigationLink, navigationLink);
        }

        /// <summary>
        /// Verify that calling WriteEnd is valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        private void VerifyCanWriteEnd(bool synchronousCall)
        {
            this.VerifyNotDisposed();
            this.VerifyCallAllowed(synchronousCall);
        }

        /// <summary>
        /// Finish writing a feed/entry/navigation link.
        /// </summary>
        private void WriteEndImplementation()
        {
            this.InterceptException(() =>
            {
                Scope currentScope = this.CurrentScope;

                switch (currentScope.State)
                {
                    case WriterState.Entry:
                        if (!this.SkipWriting)
                        {
                            ODataEntry entry = (ODataEntry)currentScope.Item;
                            Debug.Assert(
                                entry != null || this.ParentNavigationLink != null && !this.ParentNavigationLink.IsCollection.Value,
                                "when entry == null, it has to be an expanded single entry navigation");

                            if (entry != null)
                            {
                                WriterValidationUtils.ValidateEntryAtEnd(entry);
                            }

                            this.EndEntry(entry);
                            this.DecreaseEntryDepth();
                        }

                        break;
                    case WriterState.Feed:
                        if (!this.SkipWriting)
                        {
                            ODataFeed feed = (ODataFeed)currentScope.Item;
                            WriterValidationUtils.ValidateFeedAtEnd(feed, !this.outputContext.WritingResponse, this.outputContext.Version);
                            this.EndFeed(feed);
                        }

                        break;
                    case WriterState.NavigationLink:
                        if (!this.outputContext.WritingResponse)
                        {
                            throw new ODataException(Strings.ODataWriterCore_DeferredLinkInRequest);
                        }

                        if (!this.SkipWriting)
                        {
                            ODataNavigationLink link = (ODataNavigationLink)currentScope.Item;
                            ((NavigationLinkScope)this.CurrentScope).NavigationPropertyType = 
                                WriterValidationUtils.ValidateNavigationLink(link, this.ParentEntryEntityType, /*payloadKind*/null);
                            this.DuplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(link, false, link.IsCollection);
                            this.WriteDeferredNavigationLink(link);
                        }

                        break;
                    case WriterState.NavigationLinkWithContent:
                        if (!this.SkipWriting)
                        {
                            this.EndNavigationLinkWithContent((ODataNavigationLink)currentScope.Item);
                        }

                        break;
                    case WriterState.Start:                 // fall through
                    case WriterState.Completed:             // fall through
                    case WriterState.Error:                 // fall through
                        throw new ODataException(Strings.ODataWriterCore_WriteEndCalledInInvalidState(currentScope.State.ToString()));
                    default:
                        throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataWriterCore_WriteEnd_UnreachableCodePath));
                }

                this.LeaveScope();
            });
        }

        /// <summary>
        /// Verifies that calling WriteEntityReferenceLink is valid.
        /// </summary>
        /// <param name="entityReferenceLink">The entity reference link to write.</param>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        private void VerifyCanWriteEntityReferenceLink(ODataEntityReferenceLink entityReferenceLink, bool synchronousCall)
        {
            ExceptionUtils.CheckArgumentNotNull(entityReferenceLink, "entityReferenceLink");

            this.VerifyNotDisposed();
            this.VerifyCallAllowed(synchronousCall);
        }

        /// <summary>
        /// Write an entity reference link.
        /// </summary>
        /// <param name="entityReferenceLink">The entity reference link to write.</param>
        private void WriteEntityReferenceLinkImplementation(ODataEntityReferenceLink entityReferenceLink)
        {
            Debug.Assert(entityReferenceLink != null, "entityReferenceLink != null");

            if (this.outputContext.WritingResponse)
            {
                this.ThrowODataException(Strings.ODataWriterCore_EntityReferenceLinkInResponse, null);
            }

            this.CheckForNavigationLinkWithContent(ODataPayloadKind.EntityReferenceLink);
            Debug.Assert(
                this.CurrentScope.Item is ODataNavigationLink,
                "The CheckForNavigationLinkWithContent should have verified that entity reference link can only be written inside a navigation link.");

            if (!this.SkipWriting)
            {
                this.InterceptException(() =>
                {
                    WriterValidationUtils.ValidateEntityReferenceLink(entityReferenceLink);
                    this.WriteEntityReferenceInNavigationLinkContent((ODataNavigationLink)this.CurrentScope.Item, entityReferenceLink);
                });
            }
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
                    throw new ODataException(Strings.ODataWriterCore_SyncCallOnAsyncWriter);
                }
            }
            else
            {
#if ODATALIB_ASYNC
                if (this.outputContext.Synchronous)
                {
                    throw new ODataException(Strings.ODataWriterCore_AsyncCallOnSyncWriter);
                }
#else
                Debug.Assert(false, "Async calls are not allowed in this build.");
#endif
            }
        }

        /// <summary>
        /// Enters the 'ExceptionThrown' state and then throws an ODataException with the specified error message.
        /// </summary>
        /// <param name="errorMessage">The error message for the exception.</param>
        /// <param name="item">The OData item to associate with the 'ExceptionThrown' state.</param>
        private void ThrowODataException(string errorMessage, ODataItem item)
        {
            this.EnterScope(WriterState.Error, item);
            throw new ODataException(errorMessage);
        }

        /// <summary>
        /// Checks whether we are currently writing the first top-level element; if so call StartPayload
        /// </summary>
        private void StartPayloadInStartState()
        {
            if (this.State == WriterState.Start)
            {
                this.InterceptException(this.StartPayload);
            }
        }

        /// <summary>
        /// Checks whether we are currently writing a navigation link and switches to NavigationLinkWithContent state if we do.
        /// </summary>
        /// <param name="contentPayloadKind">
        /// What kind of payload kind is being written as the content of a navigation link.
        /// Only Feed, Entry or EntityRefernceLink are allowed.
        /// </param>
        private void CheckForNavigationLinkWithContent(ODataPayloadKind contentPayloadKind)
        {
            Debug.Assert(
                contentPayloadKind == ODataPayloadKind.Feed || contentPayloadKind == ODataPayloadKind.Entry || contentPayloadKind == ODataPayloadKind.EntityReferenceLink,
                "Only Feed, Entry or EntityReferenceLink can be specified as a payload kind for a navigation link content.");

            Scope currentScope = this.CurrentScope;
            if (currentScope.State == WriterState.NavigationLink || currentScope.State == WriterState.NavigationLinkWithContent)
            {
                ODataNavigationLink currentNavigationLink = (ODataNavigationLink)currentScope.Item;
                IEdmType navigationPropertyType = null;
                this.InterceptException(() =>
                    {
                        navigationPropertyType = WriterValidationUtils.ValidateNavigationLink(currentNavigationLink, this.ParentEntryEntityType, contentPayloadKind);
                        ((NavigationLinkScope)this.CurrentScope).NavigationPropertyType = navigationPropertyType;
                    });

                if (currentScope.State == WriterState.NavigationLinkWithContent)
                {
                    // If we are already in the NavigationLinkWithContent state, it means the caller is trying to write two items
                    // into the navigation link content. This is only allowed for collection navigation property in request.
                    if (this.outputContext.WritingResponse || currentNavigationLink.IsCollection != true)
                    {
                        this.ThrowODataException(Strings.ODataWriterCore_MultipleItemsInNavigationLinkContent, currentNavigationLink);
                    }

                    // Note that we don't invoke duplicate property checker in this case as it's not necessary.
                    // What happens inside the navigation link was already validated by the condition above.
                    // For collection in request we allow any combination anyway.
                    // For everything else we only allow a single item in the content and thus we will fail above.
                }
                else
                {
                    // we are writing a navigation link with content; change the state
                    this.PromoteNavigationLinkScope();

                    if (!this.SkipWriting)
                    {
                        this.InterceptException(() =>
                        {
                            this.DuplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(
                                currentNavigationLink,
                                contentPayloadKind != ODataPayloadKind.EntityReferenceLink,
                                contentPayloadKind == ODataPayloadKind.Feed);
                            this.StartNavigationLinkWithContent(currentNavigationLink);
                        });
                    }
                }
            }
            else
            {
                if (contentPayloadKind == ODataPayloadKind.EntityReferenceLink)
                {
                    this.ThrowODataException(Strings.ODataWriterCore_EntityReferenceLinkWithoutNavigationLink, null);
                }
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
            catch
            {
                if (!IsErrorState(this.State))
                {
                    this.EnterScope(WriterState.Error, this.CurrentScope.Item);
                }

                throw;
            }
        }

        /// <summary>
        /// Increments the nested entry count by one and fails if the new value exceeds the maxiumum nested entry depth limit.
        /// </summary>
        private void IncreaseEntryDepth()
        {
            this.currentEntryDepth++;

            if (this.currentEntryDepth > this.outputContext.MessageWriterSettings.MessageQuotas.MaxNestingDepth)
            {
                this.ThrowODataException(Strings.ValidationUtils_MaxDepthOfNestedEntriesExceeded(this.outputContext.MessageWriterSettings.MessageQuotas.MaxNestingDepth), null);
            }
        }

        /// <summary>
        /// Decrements the nested entry count by one.
        /// </summary>
        private void DecreaseEntryDepth()
        {
            Debug.Assert(this.currentEntryDepth > 0, "Entry depth should never become negative.");

            this.currentEntryDepth--;
        }

        /// <summary>
        /// Enter a new writer scope; verifies that the transition from the current state into new state is valid
        /// and attaches the item to the new scope.
        /// </summary>
        /// <param name="newState">The writer state to transition into.</param>
        /// <param name="item">The item to associate with the new scope.</param>
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Debug only cast.")]
        private void EnterScope(WriterState newState, ODataItem item)
        {
            this.InterceptException(() => this.ValidateTransition(newState));
            
            // If the parent scope was marked for skipping content, the new child scope should be as well.
            bool skipWriting = this.SkipWriting;

            Scope currentScope = this.CurrentScope;

            // When writing a navigation link, check if the link is being projected.
            // If we are projecting properties, but the nav. link is not projected mark it to skip its content.
            if (currentScope.State == WriterState.Entry && newState == WriterState.NavigationLink && !skipWriting)
            {
                Debug.Assert(currentScope.Item is ODataEntry, "If the current state is Entry the current Item must be entry as well (and not null either).");
                Debug.Assert(item is ODataNavigationLink, "If the new state is NavigationLink the new item must be a navigation link as well (and not null either).");

                ProjectedPropertiesAnnotation projectedProperties = currentScope.Item.GetAnnotation<ProjectedPropertiesAnnotation>();
                ODataNavigationLink navigationLink = (ODataNavigationLink)item;
                skipWriting = projectedProperties.ShouldSkipProperty(navigationLink.Name);
            }
            else if (currentScope.State == WriterState.Feed && newState == WriterState.Entry)
            {
                // When we're entering an entry scope on a feed, increment the count of entries on that feed.
                ((FeedScope)currentScope).EntryCount++;
            }

            this.PushScope(newState, item, skipWriting);
        }

        /// <summary>
        /// Leave the current writer scope and return to the previous scope. 
        /// When reaching the top-level replace the 'Started' scope with a 'Completed' scope.
        /// </summary>
        /// <remarks>Note that this method is never called once an error has been written or a fatal exception has been thrown.</remarks>
        private void LeaveScope()
        {
            Debug.Assert(this.State != WriterState.Error, "this.State != WriterState.Error");

            this.scopes.Pop();

            // if we are back at the root replace the 'Start' state with the 'Completed' state
            if (this.scopes.Count == 1)
            {
                this.scopes.Pop();
                this.PushScope(WriterState.Completed, null, false);
                this.InterceptException(this.EndPayload);
            }
        }

        /// <summary>
        /// Promotes the current navigation link scope to a navigation link scope with content.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Second cast only in debug.")]
        private void PromoteNavigationLinkScope()
        {
            Debug.Assert(
                this.State == WriterState.NavigationLink, 
                "Only a NavigationLink state can be promoted right now. If this changes please review the scope replacement code below.");
            Debug.Assert(
                this.CurrentScope.Item != null && this.CurrentScope.Item is ODataNavigationLink, 
                "Item must be a non-null navigation link.");

            this.ValidateTransition(WriterState.NavigationLinkWithContent);
            NavigationLinkScope previousScope = (NavigationLinkScope)this.scopes.Pop();
            NavigationLinkScope newScope = new NavigationLinkScope(previousScope);
            this.scopes.Push(newScope);
        }

        /// <summary>
        /// Verify that the transition from the current state into new state is valid .
        /// </summary>
        /// <param name="newState">The new writer state to transition into.</param>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "All the transition checks are encapsulated in this method.")]
        private void ValidateTransition(WriterState newState)
        {
            if (!IsErrorState(this.State) && IsErrorState(newState))
            {
                // we can always transition into an error state if we are not already in an error state
                return;
            }

            switch (this.State)
            {
                case WriterState.Start:
                    if (newState != WriterState.Feed && newState != WriterState.Entry)
                    {
                        throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromStart(this.State.ToString(), newState.ToString()));
                    }

                    if (newState == WriterState.Feed && !this.writingFeed)
                    {
                        throw new ODataException(Strings.ODataWriterCore_CannotWriteTopLevelFeedWithEntryWriter);
                    }

                    if (newState == WriterState.Entry && this.writingFeed)
                    {
                        throw new ODataException(Strings.ODataWriterCore_CannotWriteTopLevelEntryWithFeedWriter);
                    }

                    break;
                case WriterState.Entry:
                    {
                        if (this.CurrentScope.Item == null)
                        {
                            throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromNullEntry(this.State.ToString(), newState.ToString()));
                        }

                        if (newState != WriterState.NavigationLink)
                        {
                            throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromEntry(this.State.ToString(), newState.ToString()));
                        }
                    }

                    break;
                case WriterState.Feed:
                    if (newState != WriterState.Entry)
                    {
                        throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromFeed(this.State.ToString(), newState.ToString()));
                    }

                    break;
                case WriterState.NavigationLink:
                    if (newState != WriterState.NavigationLinkWithContent)
                    {
                        throw new ODataException(Strings.ODataWriterCore_InvalidStateTransition(this.State.ToString(), newState.ToString()));
                    }

                    break;
                case WriterState.NavigationLinkWithContent:
                    if (newState != WriterState.Feed && newState != WriterState.Entry)
                    {
                        throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromExpandedLink(this.State.ToString(), newState.ToString()));
                    }

                    break;
                case WriterState.Completed:
                    // we should never see a state transition when in state 'Completed'
                    throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromCompleted(this.State.ToString(), newState.ToString()));
                case WriterState.Error:
                    if (newState != WriterState.Error)
                    {
                        // No more state transitions once we are in error state except for the fatal error
                        throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromError(this.State.ToString(), newState.ToString()));
                    }

                    break;
                default:
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataWriterCore_ValidateTransition_UnreachableCodePath));
            }
        }

        /// <summary>
        /// Create a new writer scope.
        /// </summary>
        /// <param name="state">The writer state of the scope to create.</param>
        /// <param name="item">The item attached to the scope to create.</param>
        /// <param name="skipWriting">true if the content of the scope to create should not be written.</param>
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Debug.Assert check only.")]
        private void PushScope(WriterState state, ODataItem item, bool skipWriting)
        {
            Debug.Assert(
                state == WriterState.Error ||
                state == WriterState.Entry && (item == null || item is ODataEntry) ||
                state == WriterState.Feed && item is ODataFeed ||
                state == WriterState.NavigationLink && item is ODataNavigationLink ||
                state == WriterState.NavigationLinkWithContent && item is ODataNavigationLink ||
                state == WriterState.Start && item == null ||
                state == WriterState.Completed && item == null,
                "Writer state and associated item do not match.");

            Scope scope;
            switch (state)
            {
                case WriterState.Entry:
                    scope = this.CreateEntryScope((ODataEntry)item, skipWriting);
                    break;
                case WriterState.Feed:
                    scope = this.CreateFeedScope((ODataFeed)item, skipWriting);
                    break;
                case WriterState.NavigationLink:            // fall through
                case WriterState.NavigationLinkWithContent:
                    scope = new NavigationLinkScope(state, (ODataNavigationLink)item, skipWriting);
                    break;
                case WriterState.Start:                     // fall through
                case WriterState.Completed:                 // fall through
                case WriterState.Error:
                    scope = new Scope(state, item, skipWriting);
                    break;
                default:
                    string errorMessage = Strings.General_InternalError(InternalErrorCodes.ODataWriterCore_Scope_Create_UnreachableCodePath);
                    Debug.Assert(false, errorMessage);
                    throw new ODataException(errorMessage);
            }

            this.scopes.Push(scope);
        }

        /// <summary>
        /// A writer scope; keeping track of the current writer state and an item associated with this state.
        /// </summary>
        internal class Scope
        {
            /// <summary>The writer state of this scope.</summary>
            private readonly WriterState state;

            /// <summary>The item attached to this scope.</summary>
            private readonly ODataItem item;

            /// <summary>Set to true if the content of the scope should not be written.</summary>
            /// <remarks>This is used when writing navigation links which were not projected on the owning entry.</remarks>
            private readonly bool skipWriting;

            /// <summary>
            /// Constructor creating a new writer scope.
            /// </summary>
            /// <param name="state">The writer state of this scope.</param>
            /// <param name="item">The item attached to this scope.</param>
            /// <param name="skipWriting">true if the content of this scope should not be written.</param>
            internal Scope(WriterState state, ODataItem item, bool skipWriting)
            {
                DebugUtils.CheckNoExternalCallers();

                this.state = state;
                this.item = item;
                this.skipWriting = skipWriting;
            }

            /// <summary>
            /// The writer state of this scope.
            /// </summary>
            internal WriterState State
            {
                get
                {
                    DebugUtils.CheckNoExternalCallers();
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
                    DebugUtils.CheckNoExternalCallers();
                    return this.item;
                }
            }

            /// <summary>
            /// Set to true if the content of this scope should not be written.
            /// </summary>
            internal bool SkipWriting
            {
                get
                {
                    DebugUtils.CheckNoExternalCallers();
                    return this.skipWriting;
                }
            }
        }

        /// <summary>
        /// A scope for an feed.
        /// </summary>
        internal abstract class FeedScope : Scope
        {
            /// <summary>The number of entries in this feed seen so far.</summary>
            private int entryCount;

            /// <summary>
            /// Constructor to create a new feed scope.
            /// </summary>
            /// <param name="feed">The feed for the new scope.</param>
            /// <param name="skipWriting">true if the content of the scope to create should not be written.</param>
            internal FeedScope(ODataFeed feed, bool skipWriting)
                : base(WriterState.Feed, feed, skipWriting)
            {
                DebugUtils.CheckNoExternalCallers();
            }

            /// <summary>
            /// The number of entries in this feed seen so far.
            /// </summary>
            internal int EntryCount
            {
                get
                {
                    DebugUtils.CheckNoExternalCallers();
                    return this.entryCount;
                }

                set
                {
                    DebugUtils.CheckNoExternalCallers();
                    this.entryCount = value;
                }
            }
        }

        /// <summary>
        /// A scope for an entry.
        /// </summary>
        internal class EntryScope : Scope
        {
            /// <summary>Checker to detect duplicate property names.</summary>
            private readonly DuplicatePropertyNamesChecker duplicatePropertyNamesChecker;

            /// <summary>The type of the entity (if we have metadata).</summary>
            private IEdmEntityType entityType;

            /// <summary>
            /// Constructor to create a new entry scope.
            /// </summary>
            /// <param name="entry">The entry for the new scope.</param>
            /// <param name="skipWriting">true if the content of the scope to create should not be written.</param>
            /// <param name="writingResponse">true if we are writing a response, false if it's a request.</param>
            /// <param name="writerBehavior">The <see cref="ODataWriterBehavior"/> instance controlling the behavior of the writer.</param>
            internal EntryScope(ODataEntry entry, bool skipWriting, bool writingResponse, ODataWriterBehavior writerBehavior)
                : base(WriterState.Entry, entry, skipWriting)
            {
                DebugUtils.CheckNoExternalCallers();
                Debug.Assert(writerBehavior != null, "writerBehavior != null");

                if (entry != null)
                {
                    this.duplicatePropertyNamesChecker = new DuplicatePropertyNamesChecker(writerBehavior.AllowDuplicatePropertyNames, writingResponse);
                }
            }

            /// <summary>
            /// Checker to detect duplicate property names.
            /// </summary>
            internal DuplicatePropertyNamesChecker DuplicatePropertyNamesChecker
            {
                get
                {
                    DebugUtils.CheckNoExternalCallers();
                    return this.duplicatePropertyNamesChecker;
                }
            }

            /// <summary>
            /// The type of the entity (if we have metadata).
            /// </summary>
            internal IEdmEntityType EntityType
            {
                get
                {
                    DebugUtils.CheckNoExternalCallers();
                    return this.entityType;
                }

                set
                {
                    DebugUtils.CheckNoExternalCallers();
                    this.entityType = value;
                }
            }
        }

        /// <summary>
        /// A scope for a navigation link.
        /// </summary>
        private sealed class NavigationLinkScope : Scope
        {
            /// <summary>The type of the navigation property (if we have metadata).</summary>
            private IEdmType navigationPropertyType;

            /// <summary>
            /// Constructor to create a new navigation link scope.
            /// </summary>
            /// <param name="writerState">The writer state for the new scope.</param>
            /// <param name="navLink">The navigation link for the new scope.</param>
            /// <param name="skipWriting">true if the content of the scope to create should not be written.</param>
            internal NavigationLinkScope(WriterState writerState, ODataNavigationLink navLink, bool skipWriting)
                : base(writerState, navLink, skipWriting)
            {
                DebugUtils.CheckNoExternalCallers();
            }

            /// <summary>
            /// Copy constructor to create a navigation link with content scope from an existing navigation link scope.
            /// </summary>
            /// <param name="other">The navigation link scope to copy.</param>
            internal NavigationLinkScope(NavigationLinkScope other)
                : base(WriterState.NavigationLinkWithContent, other.Item, other.SkipWriting)
            {
                DebugUtils.CheckNoExternalCallers();
                this.navigationPropertyType = other.navigationPropertyType;
            }

            /// <summary>
            /// The type of the navigation property (if we have metadata).
            /// </summary>
            internal IEdmType NavigationPropertyType
            {
                get
                {
                    DebugUtils.CheckNoExternalCallers();
                    return this.navigationPropertyType;
                }

                set
                {
                    DebugUtils.CheckNoExternalCallers();
                    this.navigationPropertyType = value;
                }
            }
        }
    }
}

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

        /// <summary>Stack of writer scopes to keep track of the current context of the writer.</summary>
        private readonly ScopeStack scopes = new ScopeStack();

        /// <summary>
        /// The <see cref="FeedWithoutExpectedTypeValidator"/> to use for entries in this feed.
        /// Only applies when writing a top-level feed; otherwise null.
        /// </summary>
        private readonly FeedWithoutExpectedTypeValidator feedValidator;

        /// <summary>The number of entries which have been started but not yet ended.</summary>
        private int currentEntryDepth;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="outputContext">The output context to write to.</param>
        /// <param name="entitySet">The entity set we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the feed to be written (or null if the entity set base type should be used).</param>
        /// <param name="writingFeed">True if the writer is created for writing a feed; false when it is created for writing an entry.</param>
        protected ODataWriterCore(
            ODataOutputContext outputContext,
            IEdmEntitySet entitySet,
            IEdmEntityType entityType,
            bool writingFeed)
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

            if (entitySet != null && entityType == null)
            {
                entityType = this.outputContext.EdmTypeResolver.GetElementType(entitySet);
            }

            this.scopes.Push(new Scope(WriterState.Start, /*item*/null, entitySet, entityType, /*skipWriting*/false, outputContext.MessageWriterSettings.MetadataDocumentUri.SelectedProperties()));
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

                Scope linkScope = this.scopes.ParentOrNull;
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
                Scope entryScope = this.scopes.Parent;
                return entryScope.EntityType;
            }
        }

        /// <summary>
        /// Returns the entity type of the immediate parent entry for which a navigation link is being written.
        /// </summary>
        protected IEdmEntitySet ParentEntryEntitySet
        {
            get
            {
                Debug.Assert(
                    this.State == WriterState.NavigationLink || this.State == WriterState.NavigationLinkWithContent,
                    "ParentEntryEntityType should only be called while writing a navigation link (with or without content).");
                Scope entryScope = this.scopes.Parent;
                return entryScope.EntitySet;
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
                        entryScope = (EntryScope)this.scopes.Parent;
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
                return this.CurrentScope.EntityType;
            }
        }

        /// <summary>
        /// Returns the parent navigation link scope of an entry in an expanded link (if it exists).
        /// The entry can either be the content of the expanded link directly or nested inside a feed.
        /// </summary>
        /// <returns>The parent navigation scope of an entry in an expanded link (if it exists).</returns>
        protected NavigationLinkScope ParentNavigationLinkScope
        {
            get
            {
                Debug.Assert(this.State == WriterState.Entry || this.State == WriterState.Feed, "ParentNavigationLinkScope should only be called while writing an entry or a feed.");
                Debug.Assert(this.scopes.Count >= 2, "We should have at least the entry scope and the start scope on the stack.");

                Scope parentScope = this.scopes.Parent;
                if (parentScope.State == WriterState.Start)
                {
                    // Top-level entry.
                    return null;
                }

                if (parentScope.State == WriterState.Feed)
                {
                    Debug.Assert(this.scopes.Count >= 3, "We should have at least the entry scope, the feed scope and the start scope on the stack.");

                    // Get the feed's parent (if any)
                    parentScope = this.scopes.ParentOfParent;
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
        /// Gets the projected properties annotation for the specified scope.
        /// </summary>
        /// <param name="currentScope">The scope to get the projected properties annotation for.</param>
        /// <returns>The projected properties annotation for <paramref name="currentScope"/>.</returns>
        protected static ProjectedPropertiesAnnotation GetProjectedPropertiesAnnotation(Scope currentScope)
        {
            ExceptionUtils.CheckArgumentNotNull(currentScope, "currentScope");

            ODataItem currentItem = currentScope.Item;
            return currentItem == null ? null : currentItem.GetAnnotation<ProjectedPropertiesAnnotation>();
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
        /// <param name="entitySet">The entity set we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the feed to be written (or null if the entity set base type should be used).</param>
        /// <param name="skipWriting">true if the content of the scope to create should not be written.</param>
        /// <param name="selectedProperties">The selected properties of this scope.</param>
        /// <returns>The newly create scope.</returns>
        protected abstract FeedScope CreateFeedScope(ODataFeed feed, IEdmEntitySet entitySet, IEdmEntityType entityType, bool skipWriting, SelectedPropertiesNode selectedProperties);

        /// <summary>
        /// Create a new entry scope.
        /// </summary>
        /// <param name="entry">The entry for the new scope.</param>
        /// <param name="entitySet">The entity set we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the feed to be written (or null if the entity set base type should be used).</param>
        /// <param name="skipWriting">true if the content of the scope to create should not be written.</param>
        /// <param name="selectedProperties">The selected properties of this scope.</param>
        /// <returns>The newly create scope.</returns>
        protected abstract EntryScope CreateEntryScope(ODataEntry entry, IEdmEntitySet entitySet, IEdmEntityType entityType, bool skipWriting, SelectedPropertiesNode selectedProperties);

        /// <summary>
        /// Gets the serialization info for the given entry.
        /// </summary>
        /// <param name="entry">The entry to get the serialization info for.</param>
        /// <returns>The serialization info for the given entry.</returns>
        protected ODataFeedAndEntrySerializationInfo GetEntrySerializationInfo(ODataEntry entry)
        {
            // Need to check for null for the entry since we can be writing a null reference to a navigation property.
            ODataFeedAndEntrySerializationInfo serializationInfo = entry == null ? null : entry.SerializationInfo;

            // Always try to use the serialization info from the entry first. If it is not found on the entry, use the one inherited from the parent feed.
            // Note that we don't try to guard against inconsistent serialization info between entries and their parent feed.
            if (serializationInfo != null)
            {
                return serializationInfo;
            }

            FeedScope parentFeedScope = this.CurrentScope as FeedScope;
            if (parentFeedScope != null)
            {
                ODataFeed feed = (ODataFeed)parentFeedScope.Item;
                Debug.Assert(feed != null, "feed != null");

                return feed.SerializationInfo;
            }

            return null;
        }

        /// <summary>
        /// Creates a new navigation link scope.
        /// </summary>
        /// <param name="writerState">The writer state for the new scope.</param>
        /// <param name="navLink">The navigation link for the new scope.</param>
        /// <param name="entitySet">The entity set we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the feed to be written (or null if the entity set base type should be used).</param>
        /// <param name="skipWriting">true if the content of the scope to create should not be written.</param>
        /// <param name="selectedProperties">The selected properties of this scope.</param>
        /// <returns>The newly created navigation link scope.</returns>
        protected virtual NavigationLinkScope CreateNavigationLinkScope(WriterState writerState, ODataNavigationLink navLink, IEdmEntitySet entitySet, IEdmEntityType entityType, bool skipWriting, SelectedPropertiesNode selectedProperties)
        {
            return new NavigationLinkScope(writerState, navLink, entitySet, entityType, skipWriting, selectedProperties);
        }

        /// <summary>
        /// Place where derived writers can perform custom steps before the entry is writen, at the begining of WriteStartEntryImplementation.
        /// </summary>
        /// <param name="entry">Entry to write.</param>
        /// <param name="typeContext">The context object to answer basic questions regarding the type of the entry or feed.</param>
        /// <param name="selectedProperties">The selected properties of this scope.</param>
        protected virtual void PrepareEntryForWriteStart(ODataEntry entry, ODataFeedAndEntryTypeContext typeContext, SelectedPropertiesNode selectedProperties)
        {
            // No-op Atom and Verbose JSON. The JSON Light writer will override this method and inject the appropriate metadata builder
            // into the entry before writing.
            // When we support AutoComputePayloadMetadata for all formats in the future, we can inject the metadata builder in here and
            // remove virtual from this method.
        }

        /// <summary>
        /// Validates the media resource on the entry.
        /// </summary>
        /// <param name="entry">The entry to validate.</param>
        /// <param name="entityType">The entity type of the entry.</param>
        protected virtual void ValidateEntryMediaResource(ODataEntry entry, IEdmEntityType entityType)
        {
            // By default validate media resource
            // In WCF DS Server mode, validate media resource (in writers)
            // In WCF DS Client mode, do not validate media resource
            bool validateMediaResource = this.outputContext.UseDefaultFormatBehavior || this.outputContext.UseServerFormatBehavior;
            ValidationUtils.ValidateEntryMetadataResource(entry, entityType, this.outputContext.Model, validateMediaResource);
        }

        /// <summary>
        /// Gets the type of the entry and validates it against the model.
        /// </summary>
        /// <param name="entry">The entry to get the type for.</param>
        /// <returns>The validated entity type.</returns>
        protected IEdmEntityType ValidateEntryType(ODataEntry entry)
        {
            if (entry.TypeName == null && this.CurrentScope.EntityType != null)
            {
                return this.CurrentScope.EntityType;
            }

            // TODO TASK 884340 Clean up handling of expected types/sets during writing
            return (IEdmEntityType)TypeNameOracle.ResolveAndValidateTypeName(this.outputContext.Model, entry.TypeName, EdmTypeKind.Entity);
        }

        /// <summary>
        /// Validates that the ODataFeed.DeltaLink is null for the given expanded feed.
        /// </summary>
        /// <param name="feed">The expanded feed in question.</param>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "An instance field is used in a debug assert.")]
        protected void ValidateNoDeltaLinkForExpandedFeed(ODataFeed feed)
        {
            Debug.Assert(feed != null, "feed != null");
            Debug.Assert(
                this.ParentNavigationLink != null && this.ParentNavigationLink.IsCollection.HasValue && this.ParentNavigationLink.IsCollection.Value == true,
                "This should only be called when writing an expanded feed.");

            if (feed.DeltaLink != null)
            {
                throw new ODataException(Strings.ODataWriterCore_DeltaLinkNotSupportedOnExpandedFeed);
            }
        }

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
                        EntryScope entryScope = (EntryScope)this.CurrentScope;
                        IEdmEntityType entityType = this.ValidateEntryType(entry);
                        entryScope.EntityTypeFromMetadata = entryScope.EntityType;

                        NavigationLinkScope parentNavigationLinkScope = this.ParentNavigationLinkScope;
                        if (parentNavigationLinkScope != null)
                        {
                            // Validate the consistency of entity types in the expanded feed/entry
                            WriterValidationUtils.ValidateEntryInExpandedLink(entityType, parentNavigationLinkScope.EntityType);
                            entryScope.EntityTypeFromMetadata = parentNavigationLinkScope.EntityType;
                        }
                        else if (this.CurrentFeedValidator != null)
                        {
                            // Validate the consistency of entity types in the top-level feeds
                            this.CurrentFeedValidator.ValidateEntry(entityType);
                        }

                        entryScope.EntityType = entityType;

                        this.PrepareEntryForWriteStart(entry, entryScope.GetOrCreateTypeContext(this.outputContext.Model, this.outputContext.WritingResponse), entryScope.SelectedProperties);
                        this.ValidateEntryMediaResource(entry, entityType);
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

            // If the parent entry has a metadata builder, use that metadatabuilder on the navigation link as well.
            Debug.Assert(this.scopes.Parent != null, "Navigation link scopes must have a parent scope.");
            Debug.Assert(this.scopes.Parent.Item is ODataEntry, "The parent of a navigation link scope should always be an entry");
            ODataEntry parentEntry = (ODataEntry)this.scopes.Parent.Item;
            if (parentEntry.MetadataBuilder != null)
            {
                navigationLink.SetMetadataBuilder(parentEntry.MetadataBuilder);
            }
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
                            this.DuplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(link, false, link.IsCollection);
                            this.WriteDeferredNavigationLink(link);

                            this.MarkNavigationLinkAsProcessed(link);
                        }

                        break;
                    case WriterState.NavigationLinkWithContent:
                        if (!this.SkipWriting)
                        {
                            ODataNavigationLink link = (ODataNavigationLink)currentScope.Item;
                            this.EndNavigationLinkWithContent(link);

                            this.MarkNavigationLinkAsProcessed(link);
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
        /// Marks the navigation currently being written as processed in the parent entity's metadata builder.
        /// This is needed so that at the end of writing the entry we can query for all the unwritten navigation properties
        /// defined on the entity type and write out their metadata in fullmetadata mode.
        /// </summary>
        /// <param name="link">The navigation link being written.</param>
        private void MarkNavigationLinkAsProcessed(ODataNavigationLink link)
        {
            Debug.Assert(
                this.CurrentScope.State == WriterState.NavigationLink || this.CurrentScope.State == WriterState.NavigationLinkWithContent,
                "This method should only be called when we're writing a navigation link.");

            ODataEntry parent = (ODataEntry)this.scopes.Parent.Item;
            Debug.Assert(parent.MetadataBuilder != null, "parent.MetadataBuilder != null");
            parent.MetadataBuilder.MarkNavigationLinkProcessed(link.Name);
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
                this.InterceptException(() =>
                {
                    IEdmNavigationProperty navigationProperty =
                        WriterValidationUtils.ValidateNavigationLink(currentNavigationLink, this.ParentEntryEntityType, contentPayloadKind);
                    if (navigationProperty != null)
                    {
                        this.CurrentScope.EntityType = navigationProperty.ToEntityType();
                        IEdmEntitySet parentEntitySet = this.ParentEntryEntitySet;
                        this.CurrentScope.EntitySet = parentEntitySet == null ? null : parentEntitySet.FindNavigationTarget(navigationProperty);
                    }
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

            IEdmEntitySet entitySet = null;
            IEdmEntityType entityType = null;
            SelectedPropertiesNode selectedProperties = currentScope.SelectedProperties;
            if (newState == WriterState.Entry || newState == WriterState.Feed)
            {
                entitySet = currentScope.EntitySet;
                entityType = currentScope.EntityType;
            }

            WriterState currentState = currentScope.State;

            // When writing a navigation link, check if the link is being projected.
            // If we are projecting properties, but the nav. link is not projected mark it to skip its content.
            if (currentState == WriterState.Entry && newState == WriterState.NavigationLink)
            {
                Debug.Assert(currentScope.Item is ODataEntry, "If the current state is Entry the current Item must be entry as well (and not null either).");
                Debug.Assert(item is ODataNavigationLink, "If the new state is NavigationLink the new item must be a navigation link as well (and not null either).");
                ODataNavigationLink navigationLink = (ODataNavigationLink)item;

                if (!skipWriting)
                {
                    ProjectedPropertiesAnnotation projectedProperties = GetProjectedPropertiesAnnotation(currentScope);
                    skipWriting = projectedProperties.ShouldSkipProperty(navigationLink.Name);
                    selectedProperties = currentScope.SelectedProperties.GetSelectedPropertiesForNavigationProperty(currentScope.EntityType, navigationLink.Name);

                    if (this.outputContext.WritingResponse)
                    {
                        IEdmEntityType currentEntityType = currentScope.EntityType;
                        IEdmNavigationProperty navigationProperty = WriterValidationUtils.ValidateNavigationLink(navigationLink, currentEntityType, /*payloadKind*/null);
                        if (navigationProperty != null)
                        {
                            entityType = navigationProperty.ToEntityType();
                            IEdmEntitySet currentEntitySet = currentScope.EntitySet;
                            entitySet = currentEntitySet == null ? null : currentEntitySet.FindNavigationTarget(navigationProperty);
                        }
                    }
                }
            }
            else if (newState == WriterState.Entry && currentState == WriterState.Feed)
            {
                // When we're entering an entry scope on a feed, increment the count of entries on that feed.
                ((FeedScope)currentScope).EntryCount++;
            }

            this.PushScope(newState, item, entitySet, entityType, skipWriting, selectedProperties);
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
                Scope startScope = this.scopes.Pop();
                Debug.Assert(startScope.State == WriterState.Start, "startScope.State == WriterState.Start");
                this.PushScope(WriterState.Completed, /*item*/null, startScope.EntitySet, startScope.EntityType, /*skipWriting*/false, startScope.SelectedProperties);
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
            NavigationLinkScope newScope = previousScope.Clone(WriterState.NavigationLinkWithContent);
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
        /// <param name="entitySet">The entity set we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the feed to be written (or null if the entity set base type should be used).</param>
        /// <param name="skipWriting">true if the content of the scope to create should not be written.</param>
        /// <param name="selectedProperties">The selected properties of this scope.</param>
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Debug.Assert check only.")]
        private void PushScope(WriterState state, ODataItem item, IEdmEntitySet entitySet, IEdmEntityType entityType, bool skipWriting, SelectedPropertiesNode selectedProperties)
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
                    scope = this.CreateEntryScope((ODataEntry)item, entitySet, entityType, skipWriting, selectedProperties);
                    break;
                case WriterState.Feed:
                    scope = this.CreateFeedScope((ODataFeed)item, entitySet, entityType, skipWriting, selectedProperties);
                    break;
                case WriterState.NavigationLink:            // fall through
                case WriterState.NavigationLinkWithContent:
                    scope = this.CreateNavigationLinkScope(state, (ODataNavigationLink)item, entitySet, entityType, skipWriting, selectedProperties);
                    break;
                case WriterState.Start:                     // fall through
                case WriterState.Completed:                 // fall through
                case WriterState.Error:
                    scope = new Scope(state, item, entitySet, entityType, skipWriting, selectedProperties);
                    break;
                default:
                    string errorMessage = Strings.General_InternalError(InternalErrorCodes.ODataWriterCore_Scope_Create_UnreachableCodePath);
                    Debug.Assert(false, errorMessage);
                    throw new ODataException(errorMessage);
            }

            this.scopes.Push(scope);
        }

        /// <summary>
        /// Lightweight wrapper for the stack of scopes which exposes a few helper properties for getting parent scopes.
        /// </summary>
        internal sealed class ScopeStack
        {
            /// <summary>
            /// Use a list to store the scopes instead of a true stack so that parent/grandparent lookups will be fast.
            /// </summary>
            private readonly Stack<Scope> scopes = new Stack<Scope>();

            /// <summary>
            /// Initializes a new instance of the <see cref="ScopeStack"/> class.
            /// </summary>
            internal ScopeStack()
            {
                DebugUtils.CheckNoExternalCallers();
            }

            /// <summary>
            /// Gets the count of items in the stack.
            /// </summary>
            internal int Count
            {
                get
                {
                    DebugUtils.CheckNoExternalCallers();
                    return this.scopes.Count;
                }
            }

            /// <summary>
            /// Gets the scope below the current scope on top of the stack.
            /// </summary>
            internal Scope Parent
            {
                get
                {
                    DebugUtils.CheckNoExternalCallers();
                    Debug.Assert(this.scopes.Count > 1, "this.scopes.Count > 1");
                    Scope current = this.scopes.Pop();
                    Scope parent = this.scopes.Peek();
                    this.scopes.Push(current);
                    return parent;
                }
            }

            /// <summary>
            /// Gets the scope below the parent of the current scope on top of the stack.
            /// </summary>
            internal Scope ParentOfParent
            {
                get
                {
                    DebugUtils.CheckNoExternalCallers();
                    Debug.Assert(this.scopes.Count > 2, "this.scopes.Count > 2");
                    Scope current = this.scopes.Pop();
                    Scope parent = this.scopes.Pop();
                    Scope parentOfParent = this.scopes.Peek();
                    this.scopes.Push(parent);
                    this.scopes.Push(current);
                    return parentOfParent;
                }
            }

            /// <summary>
            /// Gets the scope below the current scope on top of the stack or null if there is only one item on the stack or the stack is empty.
            /// </summary>
            internal Scope ParentOrNull
            {
                get
                {
                    DebugUtils.CheckNoExternalCallers();
                    return this.Count == 0 ? null : this.Parent;
                }
            }

            /// <summary>
            /// Pushes the specified scope onto the stack.
            /// </summary>
            /// <param name="scope">The scope.</param>
            internal void Push(Scope scope)
            {
                DebugUtils.CheckNoExternalCallers();
                Debug.Assert(scope != null, "scope != null");
                this.scopes.Push(scope);
            }

            /// <summary>
            /// Pops the current scope off the stack.
            /// </summary>
            /// <returns>The popped scope.</returns>
            internal Scope Pop()
            {
                DebugUtils.CheckNoExternalCallers();
                Debug.Assert(this.scopes.Count > 0, "this.scopes.Count > 0");
                return this.scopes.Pop();
            }

            /// <summary>
            /// Peeks at the current scope on the top of the stack.
            /// </summary>
            /// <returns>The current scope at the top of the stack.</returns>
            internal Scope Peek()
            {
                DebugUtils.CheckNoExternalCallers();
                Debug.Assert(this.scopes.Count > 0, "this.scopes.Count > 0");
                return this.scopes.Peek();
            }
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

            /// <summary>The selected properties for the current scope.</summary>
            private readonly SelectedPropertiesNode selectedProperties;

            /// <summary>The entity set we are going to write entities for.</summary>
            private IEdmEntitySet entitySet;

            /// <summary>The entity type for the entries in the feed to be written (or null if the entity set base type should be used).</summary>
            private IEdmEntityType entityType;

            /// <summary>
            /// Constructor creating a new writer scope.
            /// </summary>
            /// <param name="state">The writer state of this scope.</param>
            /// <param name="item">The item attached to this scope.</param>
            /// <param name="entitySet">The entity set we are going to write entities for.</param>
            /// <param name="entityType">The entity type for the entries in the feed to be written (or null if the entity set base type should be used).</param>
            /// <param name="skipWriting">true if the content of this scope should not be written.</param>
            /// <param name="selectedProperties">The selected properties of this scope.</param>
            internal Scope(WriterState state, ODataItem item, IEdmEntitySet entitySet, IEdmEntityType entityType, bool skipWriting, SelectedPropertiesNode selectedProperties)
            {
                DebugUtils.CheckNoExternalCallers();

                this.state = state;
                this.item = item;
                this.entityType = entityType;
                this.entitySet = entitySet;
                this.skipWriting = skipWriting;
                this.selectedProperties = selectedProperties;
            }

            /// <summary>
            /// The entity type for the entries in the feed to be written (or null if the entity set base type should be used).
            /// </summary>
            public IEdmEntityType EntityType
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

            /// <summary>The entity set we are going to write entities for.</summary>
            internal IEdmEntitySet EntitySet
            {
                get
                {
                    DebugUtils.CheckNoExternalCallers();
                    return this.entitySet;
                }

                set
                {
                    DebugUtils.CheckNoExternalCallers();
                    this.entitySet = value;
                }
            }

            /// <summary>The selected properties for the current scope.</summary>
            internal SelectedPropertiesNode SelectedProperties
            {
                get
                {
                    DebugUtils.CheckNoExternalCallers();
                    Debug.Assert(this.selectedProperties != null, "this.selectedProperties != null");
                    return this.selectedProperties;
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
            /// <summary>The serialization info for the current feed.</summary>
            private readonly ODataFeedAndEntrySerializationInfo serializationInfo;

            /// <summary>The number of entries in this feed seen so far.</summary>
            private int entryCount;

            /// <summary>Maintains the write status for each annotation using its key.</summary>
            private InstanceAnnotationWriteTracker instanceAnnotationWriteTracker;

            /// <summary>The type context to answer basic questions regarding the type info of the entry.</summary>
            private ODataFeedAndEntryTypeContext typeContext;

            /// <summary>
            /// Constructor to create a new feed scope.
            /// </summary>
            /// <param name="feed">The feed for the new scope.</param>
            /// <param name="entitySet">The entity set we are going to write entities for.</param>
            /// <param name="entityType">The entity type for the entries in the feed to be written (or null if the entity set base type should be used).</param>
            /// <param name="skipWriting">true if the content of the scope to create should not be written.</param>
            /// <param name="selectedProperties">The selected properties of this scope.</param>
            internal FeedScope(ODataFeed feed, IEdmEntitySet entitySet, IEdmEntityType entityType, bool skipWriting, SelectedPropertiesNode selectedProperties)
                : base(WriterState.Feed, feed, entitySet, entityType, skipWriting, selectedProperties)
            {
                DebugUtils.CheckNoExternalCallers();
                this.serializationInfo = feed.SerializationInfo;
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

            /// <summary>
            /// Tracks the write status of the annotations.
            /// </summary>
            internal InstanceAnnotationWriteTracker InstanceAnnotationWriteTracker
            {
                get
                {
                    DebugUtils.CheckNoExternalCallers();
                    if (this.instanceAnnotationWriteTracker == null)
                    {
                        this.instanceAnnotationWriteTracker = new InstanceAnnotationWriteTracker();
                    }

                    return this.instanceAnnotationWriteTracker;
                }
            }

            /// <summary>
            /// Gets or creates the type context to answer basic questions regarding the type info of the entry.
            /// </summary>
            /// <param name="model">The Edm model to use.</param>
            /// <param name="writingResponse">True if writing a response payload, false otherwise.</param>
            /// <returns>The type context to answer basic questions regarding the type info of the entry.</returns>
            internal ODataFeedAndEntryTypeContext GetOrCreateTypeContext(IEdmModel model, bool writingResponse)
            {
                DebugUtils.CheckNoExternalCallers();
                if (this.typeContext == null)
                {
                    this.typeContext = ODataFeedAndEntryTypeContext.Create(
                        this.serializationInfo,
                        this.EntitySet,
                        EdmTypeWriterResolver.Instance.GetElementType(this.EntitySet),
                        this.EntityType,
                        model,
                        writingResponse);
                }

                return this.typeContext;
            }
        }

        /// <summary>
        /// A scope for an entry.
        /// </summary>
        internal class EntryScope : Scope
        {
            /// <summary>Checker to detect duplicate property names.</summary>
            private readonly DuplicatePropertyNamesChecker duplicatePropertyNamesChecker;

            /// <summary>The serialization info for the current entry.</summary>
            private readonly ODataFeedAndEntrySerializationInfo serializationInfo;

            /// <summary>The value from ODataEntry.TypeName.</summary>
            private readonly string odataEntryTypeName;

            /// <summary>The entity type which was derived from the model (may be either the same as entity type or its base type.</summary>
            private IEdmEntityType entityTypeFromMetadata;

            /// <summary>The type context to answer basic questions regarding the type info of the entry.</summary>
            private ODataFeedAndEntryTypeContext typeContext;

            /// <summary>Maintains the write status for each annotation using its key.</summary>
            private InstanceAnnotationWriteTracker instanceAnnotationWriteTracker;

            /// <summary>
            /// Constructor to create a new entry scope.
            /// </summary>
            /// <param name="entry">The entry for the new scope.</param>
            /// <param name="serializationInfo">The serialization info for the current entry.</param>
            /// <param name="entitySet">The entity set we are going to write entities for.</param>
            /// <param name="entityType">The entity type for the entries in the feed to be written (or null if the entity set base type should be used).</param>
            /// <param name="skipWriting">true if the content of the scope to create should not be written.</param>
            /// <param name="writingResponse">true if we are writing a response, false if it's a request.</param>
            /// <param name="writerBehavior">The <see cref="ODataWriterBehavior"/> instance controlling the behavior of the writer.</param>
            /// <param name="selectedProperties">The selected properties of this scope.</param>
            internal EntryScope(ODataEntry entry, ODataFeedAndEntrySerializationInfo serializationInfo, IEdmEntitySet entitySet, IEdmEntityType entityType, bool skipWriting, bool writingResponse, ODataWriterBehavior writerBehavior, SelectedPropertiesNode selectedProperties)
                : base(WriterState.Entry, entry, entitySet, entityType, skipWriting, selectedProperties)
            {
                DebugUtils.CheckNoExternalCallers();
                Debug.Assert(writerBehavior != null, "writerBehavior != null");

                if (entry != null)
                {
                    this.duplicatePropertyNamesChecker = new DuplicatePropertyNamesChecker(writerBehavior.AllowDuplicatePropertyNames, writingResponse);
                    this.odataEntryTypeName = entry.TypeName;
                }

                this.serializationInfo = serializationInfo;
            }

            /// <summary>
            /// The entity type which was derived from the model, i.e. the expected entity type, which may be either the same as entity type or its base type.
            /// For example, if we are writing a feed of Customers and the current entry is of DerivedCustomer, this.EntityTypeFromMetadata would be Customer and this.EntityType would be DerivedCustomer.
            /// </summary>
            public IEdmEntityType EntityTypeFromMetadata
            {
                get
                {
                    DebugUtils.CheckNoExternalCallers();
                    return this.entityTypeFromMetadata;
                }

                internal set
                {
                    DebugUtils.CheckNoExternalCallers();
                    this.entityTypeFromMetadata = value;
                }
            }

            /// <summary>
            /// The serialization info for the current entry.
            /// </summary>
            public ODataFeedAndEntrySerializationInfo SerializationInfo
            {
                get { return this.serializationInfo; }
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
            /// Tracks the write status of the annotations.
            /// </summary>
            internal InstanceAnnotationWriteTracker InstanceAnnotationWriteTracker
            {
                get
                {
                    DebugUtils.CheckNoExternalCallers();
                    if (this.instanceAnnotationWriteTracker == null)
                    {
                        this.instanceAnnotationWriteTracker = new InstanceAnnotationWriteTracker();
                    }

                    return this.instanceAnnotationWriteTracker;
                }
            }

            /// <summary>
            /// Gets or creates the type context to answer basic questions regarding the type info of the entry.
            /// </summary>
            /// <param name="model">The Edm model to use.</param>
            /// <param name="writingResponse">True if writing a response payload, false otherwise.</param>
            /// <returns>The type context to answer basic questions regarding the type info of the entry.</returns>
            public ODataFeedAndEntryTypeContext GetOrCreateTypeContext(IEdmModel model, bool writingResponse)
            {
                DebugUtils.CheckNoExternalCallers();
                if (this.typeContext == null)
                {
                    this.typeContext = ODataFeedAndEntryTypeContext.Create(
                        this.serializationInfo,
                        this.EntitySet,
                        EdmTypeWriterResolver.Instance.GetElementType(this.EntitySet),
                        this.EntityTypeFromMetadata,
                        model,
                        writingResponse);
                }

                return this.typeContext;
            }
        }

        /// <summary>
        /// A scope for a navigation link.
        /// </summary>
        internal class NavigationLinkScope : Scope
        {
            /// <summary>
            /// Constructor to create a new navigation link scope.
            /// </summary>
            /// <param name="writerState">The writer state for the new scope.</param>
            /// <param name="navLink">The navigation link for the new scope.</param>
            /// <param name="entitySet">The entity set we are going to write entities for.</param>
            /// <param name="entityType">The entity type for the entries in the feed to be written (or null if the entity set base type should be used).</param>
            /// <param name="skipWriting">true if the content of the scope to create should not be written.</param>
            /// <param name="selectedProperties">The selected properties of this scope.</param>
            internal NavigationLinkScope(WriterState writerState, ODataNavigationLink navLink, IEdmEntitySet entitySet, IEdmEntityType entityType, bool skipWriting, SelectedPropertiesNode selectedProperties)
                : base(writerState, navLink, entitySet, entityType, skipWriting, selectedProperties)
            {
                DebugUtils.CheckNoExternalCallers();
            }

            /// <summary>
            /// Clones this navigation link scope and sets a new writer state.
            /// </summary>
            /// <param name="newWriterState">The <see cref="WriterState"/> to set.</param>
            /// <returns>The cloned navigation link scope with the specified writer state.</returns>
            internal virtual NavigationLinkScope Clone(WriterState newWriterState)
            {
                DebugUtils.CheckNoExternalCallers();
                return new NavigationLinkScope(newWriterState, (ODataNavigationLink)this.Item, this.EntitySet, this.EntityType, this.SkipWriting, this.SelectedProperties);
            }
        }
    }
}

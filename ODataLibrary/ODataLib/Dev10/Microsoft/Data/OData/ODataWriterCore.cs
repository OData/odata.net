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
    #endregion Namespaces

    /// <summary>
    /// Base class for OData writers that verifies a proper sequence of write calls on the writer.
    /// </summary>
    internal abstract class ODataWriterCore : ODataWriter
    {
        /// <summary>The Base URI to use for all the URIs being written.</summary>
        private readonly Uri baseUri;

        /// <summary>Version of the OData protocol to use.</summary>
        private readonly ODataVersion version;

        /// <summary>Set to true if this writer is writing a response payload.</summary>
        private readonly bool writingResponse;

        /// <summary>True if the writer was created for writing a feed; false when it was created for writing an entry.</summary>
        private readonly bool writingFeed;

        /// <summary>The model to use.</summary>
        private readonly IEdmModel model;

        /// <summary>True if the writer was created for synchronous operation; false for asynchronous.</summary>
        private readonly bool synchronous;

        /// <summary>The <see cref="ODataWriterBehavior"/> instance configuring the writer.</summary>
        private readonly ODataWriterBehavior writerBehavior;

        /// <summary>Stack of writer scopes to keep track of the current context of the writer.</summary>
        private Stack<Scope> scopes = new Stack<Scope>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="urlResolver">The optional URL resolver to perform custom URL resolution for URLs written to the payload.</param>
        /// <param name="version">The version of the OData protocol to use.</param>
        /// <param name="baseUri">The Base URI to use for all the URIs being written.</param>
        /// <param name="writerBehavior">The <see cref="ODataWriterBehavior"/> instance controlling the behavior of the writer.</param>
        /// <param name="writingResponse">True if the writer is to write a response payload; false if it's to write a request payload.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="writingFeed">True if the writer is created for writing a feed; false when it is created for writing an entry.</param>
        /// <param name="synchronous">True if the writer is created for synchronous operation; false for asynchronous.</param>
        protected ODataWriterCore(
            IODataUrlResolver urlResolver,
            ODataVersion version, 
            Uri baseUri,
            ODataWriterBehavior writerBehavior,
            bool writingResponse, 
            IEdmModel model, 
            bool writingFeed, 
            bool synchronous)
            : base(urlResolver)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(baseUri == null || baseUri.IsAbsoluteUri, "baseUri must be either null or absolute");
            Debug.Assert(writerBehavior != null, "writerBehavior  != null");

            this.version = version;
            this.baseUri = baseUri;
            this.writingResponse = writingResponse;
            this.writerBehavior = writerBehavior;
            this.model = model;
            this.writingFeed = writingFeed;
            this.synchronous = synchronous;

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
            NavigationLink,

            /// <summary>The writer is currently writing an expanded link.</summary>
            ExpandedLink,

            /// <summary>The writer has completed; nothing can be written anymore.</summary>
            Completed,

            /// <summary>The writer is in error state; nothing can be written anymore.</summary>
            Error
        }

        /// <summary>
        /// The Base URI to use for all the URIs being written
        /// </summary>
        protected Uri BaseUri
        {
            get
            {
                return this.baseUri;
            }
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
        /// Returns the immediate link which is being expanded, or null if no such link exists
        /// </summary>
        protected ODataNavigationLink ParentExpandedLink
        {
            get
            {
                Debug.Assert(this.State == WriterState.Entry || this.State == WriterState.Feed, "ParentExpandedLink should only be called while writing an entry or a feed.");

                Scope linkScope = this.scopes.Skip(1).FirstOrDefault();
                return linkScope == null ? null : (linkScope.Item as ODataNavigationLink);
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
                    this.State == WriterState.Entry || this.State == WriterState.NavigationLink || this.State == WriterState.ExpandedLink, 
                    "DuplicatePropertyNamesChecker should only be called while writing an entry or an (expanded or deferred) navigation link.");

                EntryScope entryScope;
                switch (this.State)
                {
                    case WriterState.Entry:
                        entryScope = (EntryScope)this.CurrentScope;
                        break;
                    case WriterState.ExpandedLink:
                    case WriterState.NavigationLink:
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
        /// Set to true if a response is being written.
        /// </summary>
        protected bool WritingResponse
        {
            get
            {
                return this.writingResponse;
            }
        }

        /// <summary>
        /// The <see cref="ODataWriterBehavior"/> instance configuring the writer.
        /// </summary>
        protected ODataWriterBehavior WriterBehavior
        {
            get
            {
                return this.writerBehavior;
            }
        }

        /// <summary>
        /// The model to use (or null if no metadata is available).
        /// </summary>
        protected IEdmModel Model
        {
            get
            {
                return this.model;
            }
        }

        /// <summary>
        /// Flushes the write buffer to the underlying stream.
        /// </summary>
        public sealed override void Flush()
        {
            this.VerifyCanFlush(true);

            // make sure we switch to writer state FatalExceptionThrown if an exception is thrown during flushing.
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

            // make sure we switch to writer state Error if an exception is thrown during flushing.
            return this.FlushAsynchronously().ContinueWithOnFault(t => this.EnterScope(WriterState.Error, null));
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
        }

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously finish writing a feed/entry/navigation link.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public sealed override Task WriteEndAsync()
        {
            this.VerifyCanWriteEnd(false);
            return TaskUtils.GetTaskForSynchronousOperation(this.WriteEndImplementation);
        }
#endif

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
        /// Flushes and closes the writer. This method is only called during disposing the ODataWriter.
        /// </summary>
        protected abstract void FlushAndCloseWriter();

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
        /// Start writing an expanded navigation link.
        /// </summary>
        /// <param name="navigationLink">The navigation link to write.</param>
        protected abstract void StartExpandedLink(ODataNavigationLink navigationLink);

        /// <summary>
        /// Finish writing an expanded navigation link.
        /// </summary>
        /// <param name="navigationLink">The navigation link to write.</param>
        protected abstract void EndExpandedLink(ODataNavigationLink navigationLink);

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
        /// Write an OData error.
        /// </summary>
        /// <param name='errorInstance'>The error information to write.</param>
        /// <param name="includeDebugInformation">If in debug mode error details will be included in the error.</param>
        protected sealed override void WriteErrorImplementation(ODataError errorInstance, bool includeDebugInformation)
        {
            ExceptionUtils.CheckArgumentNotNull(errorInstance, "errorInstance");

            this.VerifyNotDisposed();

            // We're in a completed state trying to write an error (we can't write error after the payload was finished as it might
            // introduce another top-level element in XML)
            if (this.State == WriterState.Completed)
            {
                throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromCompleted(this.State.ToString(), WriterState.Error.ToString()));
            }

            this.StartPayloadInStartState();
            this.EnterScope(WriterState.Error, this.CurrentScope.Item);
            this.SerializeError(errorInstance, includeDebugInformation);
        }

        /// <summary>
        /// Write an error message.
        /// </summary>
        /// <param name='error'>The error to write.</param>
        /// <param name="includeDebugInformation">If in debug mode error details will be included (if present).</param>
        protected abstract void SerializeError(ODataError error, bool includeDebugInformation);

        /// <summary>
        /// Perform the actual cleanup work.
        /// </summary>
        /// <remarks>
        /// Classes inheriting from this base class should implement the dispose logic in FlushAndCloseWriter.
        /// </remarks>
        protected override void DisposeWriterImplementation()
        {
            if (this.scopes != null)
            {
                // NOTE: we do not check the writer state here (to require it to be either an error state or start or completed)
                //       because the writer can be disposed due to an exception in user code. In that case we still have to flush any pending data.
                this.FlushAndCloseWriter();
            }

            this.scopes = null;
        }

        /// <summary>
        /// Validates association link before writing.
        /// </summary>
        /// <param name="associationLink">The association link to validate.</param>
        protected void ValidateAssociationLink(ODataAssociationLink associationLink)
        {
            Debug.Assert(associationLink != null, "associationLink != null");

            WriterValidationUtils.ValidateAssociationLink(associationLink, this.Version);

            // We don't need the returned IEdmProperty since it was already validated to be a navigation property.
            ValidationUtils.ValidateNavigationPropertyDefined(associationLink.Name, this.EntryEntityType);
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
            this.CheckForExpandedLink(true);
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
                        if (!this.WritingResponse)
                        {
                            this.ThrowODataException(Strings.ODataWriterCore_InlineCountInRequest, feed);
                        }

                        // Verify version requirements
                        ODataVersionChecker.CheckInlineCount(this.Version);
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
            this.CheckForExpandedLink(false);
            this.EnterScope(WriterState.Entry, entry);
            if (!this.SkipWriting)
            {
                this.InterceptException(() =>
                {
                    if (entry != null)
                    {
                        IEdmEntityType entityType = WriterValidationUtils.ValidateEntityTypeName(this.Model, entry.TypeName);

                        // By default validate media resource
                        // In WCF DS Server mode, validate media resource (in writers)
                        // In WCF DS Client mode, do not validate media resource
                        ODataBehaviorKind behaviorKind = this.WriterBehavior.BehaviorKind;
                        bool validateMediaResource =
                            behaviorKind == ODataBehaviorKind.Default ||
                            behaviorKind == ODataBehaviorKind.WcfDataServicesServer;
                        ValidationUtils.ValidateEntryMetadata(entry, entityType, validateMediaResource);

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
                                entry != null || this.ParentExpandedLink != null && !this.ParentExpandedLink.IsCollection.Value,
                                "when entry == null, it has to be and expanded single entry navigation");

                            if (entry != null)
                            {
                                WriterValidationUtils.ValidateEntryAtEnd(entry);
                            }

                            this.EndEntry(entry);
                        }

                        break;
                    case WriterState.Feed:
                        if (!this.SkipWriting)
                        {
                            ODataFeed feed = (ODataFeed)currentScope.Item;
                            WriterValidationUtils.ValidateFeedAtEnd(feed, !this.writingResponse, this.version);
                            this.EndFeed(feed);
                        }

                        break;
                    case WriterState.NavigationLink:
                        if (!this.SkipWriting)
                        {
                            ODataNavigationLink link = (ODataNavigationLink)currentScope.Item;
                            ValidationUtils.ValidateNavigationLink(link);
                            this.DuplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(link, false, link.IsCollection);
                            this.WriteDeferredNavigationLink(link);
                        }

                        break;
                    case WriterState.ExpandedLink:
                        if (!this.SkipWriting)
                        {
                            this.EndExpandedLink((ODataNavigationLink)currentScope.Item);
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
                if (!this.synchronous)
                {
                    throw new ODataException(Strings.ODataWriterCore_SyncCallOnAsyncWriter);
                }
            }
            else
            {
#if ODATALIB_ASYNC
                if (this.synchronous)
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
        /// Checks whether we are currently writing a navigation link and switches to ExpandedLink state if we do.
        /// </summary>
        /// <param name="writingExpandedLinkForFeed">Flag indicating whether we are currently trying to write a feed or an entry.</param>
        private void CheckForExpandedLink(bool writingExpandedLinkForFeed)
        {
            Scope currentScope = this.CurrentScope;
            if (currentScope.State == WriterState.NavigationLink)
            {
                ODataNavigationLink currentNavigationLink = (ODataNavigationLink)currentScope.Item;

                // make sure the IsCollection property is set correctly on the navigation link
                // if the navigation link specifies an IsCollection value
                if (currentNavigationLink.IsCollection.HasValue && writingExpandedLinkForFeed != currentNavigationLink.IsCollection.Value && !this.SkipWriting)
                {
                    string uri = currentNavigationLink.Url == null ? "null" : UriUtilsCommon.UriToString(currentNavigationLink.Url);
                    string error = writingExpandedLinkForFeed
                        ? Strings.ODataWriterCore_EntryExpandedLinkWithFeedContent(uri)
                        : Strings.ODataWriterCore_FeedExpandedLinkWithEntryContent(uri);
                    this.ThrowODataException(error, currentNavigationLink);
                }

                // we are writing an expanded navigation link; change the state
                this.ReplaceScope(WriterState.ExpandedLink, currentNavigationLink);
                if (!this.SkipWriting)
                {
                    this.InterceptException(() => 
                    {
                        ValidationUtils.ValidateNavigationLink(currentNavigationLink);
                        this.DuplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(currentNavigationLink, true, writingExpandedLinkForFeed);
                        this.StartExpandedLink(currentNavigationLink);
                    });
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
        /// Replaces the current scope with a new scope; checks that the transition is valid.
        /// </summary>
        /// <param name="newState">The new state to transition into.</param>
        /// <param name="item">The item associated with the new state.</param>
        private void ReplaceScope(WriterState newState, ODataItem item)
        {
            Debug.Assert(
                newState == WriterState.ExpandedLink, 
                "Only ExpandedLink state can be replaced right now. If this changes please review the SkipWriting propagation below.");

            this.ValidateTransition(newState);
            Scope previousScope = this.scopes.Pop();
            this.PushScope(newState, item, previousScope.SkipWriting);
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
                    if (newState != WriterState.ExpandedLink)
                    {
                        throw new ODataException(Strings.ODataWriterCore_InvalidStateTransition(this.State.ToString(), newState.ToString()));
                    }

                    break;
                case WriterState.ExpandedLink:
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
                state == WriterState.ExpandedLink && item is ODataNavigationLink ||
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
                case WriterState.NavigationLink:        // fall through
                case WriterState.Start:                 // fall through
                case WriterState.ExpandedLink:          // fall through
                case WriterState.Completed:             // fall through
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
    }
}

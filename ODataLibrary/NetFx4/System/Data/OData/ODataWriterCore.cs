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
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    #endregion Namespaces.

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
        /// <param name="baseUri">The Base URI to use for all the URIs being written.</param>
        /// <param name="writingResponse">True if the writer is to write a response payload; false if it's to write a request payload.</param>
        /// <param name="metadataProvider">The metadata provider to use.</param>
        /// <param name="writingFeed">True if the writer is created for writing a feed; false when it is created for writing an entry.</param>
        /// <param name="synchronous">True if the writer is created for synchronous operation; false for asynchronous.</param>
        protected ODataWriterCore(
            ODataVersion version, 
            Uri baseUri, 
            bool writingResponse, 
            DataServiceMetadataProviderWrapper metadataProvider, 
            bool writingFeed, 
            bool synchronous)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(baseUri == null || baseUri.IsAbsoluteUri, "baseUri must be either null or absolute");

            this.version = version;
            this.baseUri = baseUri;
            this.writingResponse = writingResponse;
            this.metadataProvider = metadataProvider;
            this.writingFeed = writingFeed;
            this.synchronous = synchronous;
            this.scopes.Push(Scope.Create(WriterState.Start, null));
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

            /// <summary>The writer is currently writing a link (possibly an expanded link but we don't know yet).</summary>
            Link,

            /// <summary>The writer is currently writing an expanded link.</summary>
            ExpandedLink,

            /// <summary>The writer has completed; nothing can be written anymore.</summary>
            Completed,

            /// <summary>The writer has thrown an ODataException; it is only valid to write an error at this time.</summary>
            ODataExceptionThrown,

            /// <summary>The writer has thrown a fatal exception; nothing can be written to the writer anymore.</summary>
            FatalExceptionThrown,

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
        /// The current state of the writer.
        /// </summary>
        protected WriterState State
        {
            get
            {
                return this.scopes.Peek().State;
            }
        }

        /// <summary>
        /// A flag indicating whether the reader is at the top level.
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
        protected ODataLink ParentExpandedLink
        {
            get
            {
                Debug.Assert(this.State == WriterState.Entry || this.State == WriterState.Feed, "ParentExpandedLink should only be called while writing an entry or feed.");

                Scope linkScope = this.scopes.Skip(1).FirstOrDefault();
                return linkScope == null ? null : (linkScope.Item as ODataLink);
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
        /// Set to true if response is being written.
        /// </summary>
        protected bool WritingResponse
        {
            get
            {
                return this.writingResponse;
            }
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
                this.ReplaceScope(WriterState.FatalExceptionThrown, null);
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
                            this.ReplaceScope(WriterState.FatalExceptionThrown, null);

                            // to avoid nested aggregate exceptions only because we changed the internal state
                            // we re-throw the inner exception if there is only one. Otherwise we have to live
                            // with the nesting.
                            throw t.Exception.InnerExceptions.Count == 1 ? t.Exception.InnerException : t.Exception;
                        }
                    });
        }
#endif

        /// <summary>
        /// Start writing a feed.
        /// </summary>
        /// <param name="feed">Feed to write.</param>
        public sealed override void WriteStart(ODataFeed feed)
        {
            ExceptionUtils.CheckArgumentNotNull(feed, "feed");

            this.VerifyNotDisposed();
            this.CheckStartPayload();
            this.CheckForExpandedLink(true);
            this.EnterScope(WriterState.Feed, feed);
            this.InterceptException(() => this.WriteStartImplementation(feed));
        }

        /// <summary>
        /// Start writing an entry.
        /// </summary>
        /// <param name="entry">Entry to write.</param>
        public sealed override void WriteStart(ODataEntry entry)
        {
            ExceptionUtils.CheckArgumentNotNull(entry, "entry");

            this.VerifyNotDisposed();
            this.CheckStartPayload();
            this.CheckForExpandedLink(false);
            this.EnterScope(WriterState.Entry, entry);
            this.InterceptException(() => this.StartEntry(entry));
        }

        /// <summary>
        /// Start writing a link.
        /// </summary>
        /// <param name="link">Link to write.</param>
        public sealed override void WriteStart(ODataLink link)
        {
            ExceptionUtils.CheckArgumentNotNull(link, "link");

            this.VerifyNotDisposed();
            this.EnterScope(WriterState.Link, link);
        }

        /// <summary>
        /// Write an OData error.
        /// </summary>
        /// <param name='errorInstance'>The error information to write.</param>
        /// <param name="includeDebugInformation">If in debug mode error details will be included in the error.</param>
        public sealed override void WriteError(ODataError errorInstance, bool includeDebugInformation)
        {
            ExceptionUtils.CheckArgumentNotNull(errorInstance, "errorInstance");

            this.VerifyNotDisposed();
            this.CheckStartPayload();
            try
            {
                this.EnterScope(WriterState.Error, this.scopes.Peek().Item);
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
                this.EnterScope(WriterState.FatalExceptionThrown, this.scopes.Peek().Item);
                throw;
            }
        }

        /// <summary>
        /// Finish writing a feed/entry/link.
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
        protected static bool IsErrorState(WriterState state)
        {
            return state == WriterState.Error || state == WriterState.ODataExceptionThrown || state == WriterState.FatalExceptionThrown;
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
        /// Write a regular (non-expanded) link.
        /// </summary>
        /// <param name="link">The link to write.</param>
        protected abstract void WriteLink(ODataLink link);

        /// <summary>
        /// Start writing am expanded link.
        /// </summary>
        /// <param name="link">The link to write.</param>
        protected abstract void StartExpandedLink(ODataLink link);

        /// <summary>
        /// Finish writing an expanded link.
        /// </summary>
        /// <param name="link">The link to write.</param>
        protected abstract void EndExpandedLink(ODataLink link);

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
                    if (!IsErrorState(this.State) && this.State != WriterState.Completed && this.State != WriterState.Start)
                    {
                        throw new ODataException(Strings.ODataWriterCore_WriterDisposedWithoutAllWriteEnds);
                    }

                    // if the writer is disposed after a fatal exception has been thrown discard all buffered data
                    // of the underlying output stream so we can safely dispose it (below).
                    // if the writer is disposed after an OData exception and no error payload was written 
                    // (i.e., it is in state ODataExceptionThrown) also discard all buffered data
                    bool discardBufferedData = this.State == WriterState.ODataExceptionThrown || this.State == WriterState.FatalExceptionThrown || this.State == WriterState.Start;

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
            if (this.State == WriterState.FatalExceptionThrown)
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
                throw new ODataException(Strings.ODataWriterCore_SyncCallOnAsyncWriter);
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
                throw new ODataException(Strings.ODataWriterCore_AsyncCallOnSyncWriter);
            }
        }
#endif

        /// <summary>
        /// Enters the 'ExceptionThrown' state and then throws an ODataException with the specified error message.
        /// </summary>
        /// <param name="errorMessage">The error message for the exception.</param>
        /// <param name="item">The OData item to associate with the 'ExceptionThrown' state.</param>
        private void ThrowODataException(string errorMessage, ODataItem item)
        {
            this.EnterScope(WriterState.ODataExceptionThrown, item);
            throw new ODataException(errorMessage);
        }

        /// <summary>
        /// Checks whether we are currently writing the first top-level element; if so call StartPayload
        /// </summary>
        private void CheckStartPayload()
        {
            Scope current = this.scopes.Peek();
            if (current.State == WriterState.Start)
            {
                this.InterceptException(this.StartPayload);
            }
        }

        /// <summary>
        /// Checks whether we are currently writing a link and switches to ExpandedLink state if we do.
        /// </summary>
        /// <param name="writingExpandedLinkForFeed">Flag indicating whether we are currently trying to write a feed or an entry.</param>
        private void CheckForExpandedLink(bool writingExpandedLinkForFeed)
        {
            Scope current = this.scopes.Peek();
            if (current.State == WriterState.Link)
            {
                ODataLink currentLink = (ODataLink)current.Item;

                // make sure the IsCollection property is set correctly on the link
                bool isCollection = currentLink.IsCollection;
                if (writingExpandedLinkForFeed != isCollection)
                {
                    string uri = currentLink.Url == null ? "null" : UriUtils.UriToString(currentLink.Url);
                    string error = writingExpandedLinkForFeed
                        ? Strings.ODataWriterCore_EntryExpandedLinkWithFeedContent(uri)
                        : Strings.ODataWriterCore_FeedExpandedLinkWithEntryContent(uri);
                    this.ThrowODataException(error, currentLink);
                }

                // we are writing an expanded link; change the state
                this.ReplaceScope(WriterState.ExpandedLink, currentLink);
                this.InterceptException(() => this.StartExpandedLink(currentLink));
            }
        }

        /// <summary>
        /// Starts writing a feed
        /// </summary>
        /// <param name="feed">The feed to write.</param>
        private void WriteStartImplementation(ODataFeed feed)
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
                if (!this.WritingResponse)
                {
                    this.ThrowODataException(Strings.ODataWriterCore_InlineCountInRequest, feed);
                }

                // Verify version requirements
                ODataVersionChecker.CheckInlineCount(this.Version);
            }

            this.StartFeed(feed);
        }

        /// <summary>
        /// Finish writing a feed/entry/link.
        /// </summary>
        private void WriteEndImplementation()
        {
            Scope currentScope = this.scopes.Peek();

            string changedPropertyName = currentScope.ChangedPropertyName;
            if (changedPropertyName != null)
            {
                throw new ODataException(Strings.ODataWriterCore_ItemHasChangedBetweenStartAndEndWrite(changedPropertyName));
            }

            switch (currentScope.State)
            {
                case WriterState.Entry:
                    ODataEntry entry = (ODataEntry)currentScope.Item;
                    ValidationUtils.ValidateEntry(entry);
                    this.EndEntry(entry);
                    break;
                case WriterState.Feed:
                    ODataFeed feed = (ODataFeed)currentScope.Item;
                    ValidationUtils.ValidateFeed(feed, !this.writingResponse, this.version);
                    this.EndFeed(feed);
                    break;
                case WriterState.Link:
                    this.WriteLink((ODataLink)currentScope.Item);
                    break;
                case WriterState.ExpandedLink:
                    this.EndExpandedLink((ODataLink)currentScope.Item);
                    break;
                case WriterState.Start:                 // fall through
                case WriterState.Completed:             // fall through
                case WriterState.Error:                 // fall through
                case WriterState.ODataExceptionThrown:  // fall through
                case WriterState.FatalExceptionThrown:
                    throw new ODataException(Strings.ODataWriterCore_WriteEndCalledInInvalidState(currentScope.State.ToString()));
                default:
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataWriterCore_WriteEnd_UnreachableCodePath));
            }

            this.LeaveScope();
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
                    this.EnterScope(WriterState.ODataExceptionThrown, this.scopes.Peek().Item);
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
                this.EnterScope(WriterState.FatalExceptionThrown, this.scopes.Peek().Item);
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
        private void EnterScope(WriterState newState, ODataItem item)
        {
            this.InterceptException(() => this.ValidateTransition(newState));
            this.scopes.Push(Scope.Create(newState, item));
        }

        /// <summary>
        /// Leave the current writer scope and return to the previous scope. 
        /// When reaching the top-level replace the 'Started' scope with a 'Completed' scope.
        /// </summary>
        /// <remarks>Note that this method is never called once an error has been written or a fatal exception has been thrown.</remarks>
        private void LeaveScope()
        {
            Debug.Assert(this.State != WriterState.Error, "this.State != WriterState.Error");
            Debug.Assert(this.State != WriterState.FatalExceptionThrown, "this.State != WriterState.FatalExceptionThrown");

            this.scopes.Pop();

            // if we are back at the root replace the 'Start' state with the 'Completed' state
            if (this.scopes.Count == 1)
            {
                this.scopes.Pop();
                this.scopes.Push(Scope.Create(WriterState.Completed, null));
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
            this.ValidateTransition(newState);
            this.scopes.Pop();
            this.scopes.Push(Scope.Create(newState, item));
        }

        /// <summary>
        /// Verify that the transition from the current state into new state is valid .
        /// </summary>
        /// <param name="newState">The new writer state to transition into.</param>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "All the transition checks are encapsulated in this method.")]
        private void ValidateTransition(WriterState newState)
        {
            if (!IsErrorState(this.State) && IsErrorState(newState) && 
                !(this.State == WriterState.Completed && newState == WriterState.Error))
            {
                // we can always transition into an error state if we are not already in an error state
                // unless we're in a completed state trying to write an error (we can't write error after the payload
                // was finished as it might introduce another top-level element in XML)
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
                    if (newState != WriterState.Link)
                    {
                        throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromEntry(this.State.ToString(), newState.ToString()));
                    }

                    break;
                case WriterState.Feed:
                    if (newState != WriterState.Entry)
                    {
                        throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromFeed(this.State.ToString(), newState.ToString()));
                    }

                    break;
                case WriterState.Link:
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
                case WriterState.ODataExceptionThrown:
                    if (!IsErrorState(newState))
                    {
                        // once an exception has been thrown we only allow clients to write an error (or we detect a fatal exception)
                        throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromODataExceptionThrown(this.State.ToString(), newState.ToString()));
                    }

                    break;
                case WriterState.Error:
                    // No more state transitions once we are in error state except for the fatal error
                    if (newState != WriterState.FatalExceptionThrown)
                    {
                        throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromError(this.State.ToString(), newState.ToString()));
                    }

                    break;
                case WriterState.FatalExceptionThrown:
                    if (newState != WriterState.FatalExceptionThrown)
                    {
                        throw new ODataException(Strings.ODataWriterCore_InvalidTransitionFromFatalExceptionThrown(this.State.ToString(), newState.ToString()));
                    }

                    break;
                default:
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataWriterCore_ValidateTransition_UnreachableCodePath));
            }
        }

        /// <summary>
        /// A writer scope; keeping track of the current writer state and an item associated with this state.
        /// </summary>
        private class Scope
        {
            /// <summary>The writer state of this scope.</summary>
            private readonly WriterState state;

            /// <summary>The item attached to this scope.</summary>
            private readonly ODataItem item;

            /// <summary>
            /// Constructor creating a new writer scope.
            /// </summary>
            /// <param name="state">The writer state of this scope.</param>
            /// <param name="item">The item attached to this scope.</param>
            protected Scope(WriterState state, ODataItem item)
            {
                this.state = state;
                this.item = item;
            }

            /// <summary>
            /// The writer state of this scope.
            /// </summary>
            public WriterState State
            {
                get
                {
                    return this.state;
                }
            }

            /// <summary>
            /// The item attached to this scope.
            /// </summary>
            public ODataItem Item
            {
                get
                {
                    return this.item;
                }
            }

            /// <summary>
            /// Returns the name of the property that has changed or 'null' if nothing has changed.
            /// </summary>
            public virtual string ChangedPropertyName
            {
                get
                {
                    return null;
                }
            }

            /// <summary>
            /// Factory method to create a new writer scope.
            /// </summary>
            /// <param name="state">The writer state of the scope to create.</param>
            /// <param name="item">The item attached to the scope to create.</param>
            /// <returns>The newly created scope.</returns>
            [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Debug.Assert check only.")]
            public static Scope Create(WriterState state, ODataItem item)
            {
                Debug.Assert(
                    state == WriterState.Error ||
                    state == WriterState.ODataExceptionThrown ||
                    state == WriterState.FatalExceptionThrown ||
                    state == WriterState.Entry && item is ODataEntry ||
                    state == WriterState.Feed && item is ODataFeed ||
                    state == WriterState.Link && item is ODataLink ||
                    state == WriterState.ExpandedLink && item is ODataLink ||
                    state == WriterState.Start && item == null ||
                    state == WriterState.Completed && item == null,
                    "Writer state and associated item do not match.");

                switch (state)
                {
                    case WriterState.Entry:
                        return new EntryScope(state, (ODataEntry)item);
                    case WriterState.Feed:
                        return new FeedScope(state, (ODataFeed)item);
                    case WriterState.Start:                 // fall through
                    case WriterState.Link:                  // fall through
                    case WriterState.ExpandedLink:          // fall through
                    case WriterState.Completed:             // fall through
                    case WriterState.ODataExceptionThrown:  // fall through
                    case WriterState.FatalExceptionThrown:  // fall through
                    case WriterState.Error:
                        return new Scope(state, item);
                    default:
                        string errorMessage = Strings.General_InternalError(InternalErrorCodes.ODataWriterCore_Scope_Create_UnreachableCodePath);
                        Debug.Assert(false, errorMessage);
                        throw new ODataException(errorMessage);
                }
            }

            /// <summary>
            /// A scope for an feed.
            /// </summary>
            private sealed class FeedScope : Scope
            {
                /// <summary>Name of the property on the ODataFeed that represents the 'count' value.</summary>
                private const string FeedCountPropertyName = "Count";

                /// <summary>The count captured upon scope creation.</summary>
                private readonly long? count;

                /// <summary>
                /// Constructor to create a new feed scope.
                /// </summary>
                /// <param name="state">The writer state of the new scope.</param>
                /// <param name="feed">The feed for the new scope.</param>
                internal FeedScope(WriterState state, ODataFeed feed)
                    : base(state, feed)
                {
                    this.count = feed.Count;
                }

                /// <summary>
                /// Returns the name of the property that has changed or 'null' if nothing has changed.
                /// </summary>
                public override string ChangedPropertyName
                {
                    get
                    {
                        ODataFeed feed = (ODataFeed)this.Item;
                        if (!this.count.Equals(feed.Count))
                        {
                            return FeedCountPropertyName;
                        }

                        return null;
                    }
                }
            }

            /// <summary>
            /// A scope for a entry.
            /// </summary>
            private sealed class EntryScope : Scope
            {
                /// <summary>Name of the property on the ODataEntry that represents the 'etag' value.</summary>
                private const string EntryETagPropertyName = "ETag";

                /// <summary>The ETag captured upon scope creation.</summary>
                private readonly string etag;

                /// <summary>
                /// Constructor to create a new entry scope.
                /// </summary>
                /// <param name="state">The writer state of the new scope.</param>
                /// <param name="entry">The entry for the new scope.</param>
                internal EntryScope(WriterState state, ODataEntry entry)
                    : base(state, entry)
                {
                    this.etag = entry.ETag;
                }

                /// <summary>
                /// Returns the name of the property that has changed or 'null' if nothing has changed.
                /// </summary>
                public override string ChangedPropertyName
                {
                    get
                    {
                        ODataEntry entry = (ODataEntry)this.Item;
                        if (!object.ReferenceEquals(this.etag, entry.ETag))
                        {
                            return EntryETagPropertyName;
                        }

                        return null;
                    }
                }
            }
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="ODataBatchReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
#if PORTABLELIB
    using System.Threading.Tasks;
#endif
    #endregion Namespaces

    /// <summary>
    /// Class for reading OData batch messages; also verifies the proper sequence of read calls on the reader.
    /// </summary>
    public sealed class ODataBatchReader : IODataBatchOperationListener
    {
        /// <summary>The input context to read the content from.</summary>
        private readonly ODataRawInputContext inputContext;

        /// <summary>The batch stream used by the batch reader to devide a batch payload into parts.</summary>
        private readonly ODataBatchReaderStream batchStream;

        /// <summary>True if the writer was created for synchronous operation; false for asynchronous.</summary>
        private readonly bool synchronous;

        /// <summary>The batch-specific URL converter that stores the content IDs found in a changeset and supports resolving cross-referencing URLs.</summary>
        private readonly ODataBatchPayloadUriConverter payloadUriConverter;

        /// <summary>The dependency injection container to get related services.</summary>
        private readonly IServiceProvider container;

        /// <summary>The current state of the batch reader.</summary>
        private ODataBatchReaderState batchReaderState;

        /// <summary>The current size of the batch message, i.e., how many query operations and changesets have been read.</summary>
        private uint currentBatchSize;

        /// <summary>The current size of the active changeset, i.e., how many operations have been read for the changeset.</summary>
        private uint currentChangeSetSize;

        /// <summary>An enumeration tracking the state of the current batch operation.</summary>
        private OperationState operationState;

        /// <summary>The value of the content ID header of the current part.</summary>
        /// <remarks>
        /// The content ID header of the current part should only be visible to subsequent parts
        /// so we can only add it to the URL resolver once we are done with the current part.
        /// </remarks>
        private string contentIdToAddOnNextRead;

        /// <summary>
        /// Internal switch for whether we support reading Content-ID header appear in HTTP head instead of ChangeSet head.
        /// </summary>
        private bool allowLegacyContentIdBehaviour;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputContext">The input context to read the content from.</param>
        /// <param name="batchBoundary">The boundary string for the batch structure itself.</param>
        /// <param name="batchEncoding">The encoding to use to read from the batch stream.</param>
        /// <param name="synchronous">true if the reader is created for synchronous operation; false for asynchronous.</param>
        internal ODataBatchReader(ODataRawInputContext inputContext, string batchBoundary, Encoding batchEncoding, bool synchronous)
        {
            Debug.Assert(inputContext != null, "inputContext != null");
            Debug.Assert(!string.IsNullOrEmpty(batchBoundary), "!string.IsNullOrEmpty(batchBoundary)");

            this.inputContext = inputContext;
            this.container = inputContext.Container;
            this.synchronous = synchronous;
            this.payloadUriConverter = new ODataBatchPayloadUriConverter(inputContext.PayloadUriConverter);
            this.batchStream = new ODataBatchReaderStream(inputContext, batchBoundary, batchEncoding);
            this.allowLegacyContentIdBehaviour = true;
        }

        /// <summary>
        /// An enumeration to track the state of a batch operation.
        /// </summary>
        private enum OperationState
        {
            /// <summary>No action has been performed on the operation.</summary>
            None,

            /// <summary>The batch message for the operation has been created and returned to the caller.</summary>
            MessageCreated,

            /// <summary>The stream of the batch operation message has been requested.</summary>
            StreamRequested,

            /// <summary>The stream of the batch operation message has been disposed.</summary>
            StreamDisposed,
        }

        /// <summary>Gets the current state of the batch reader.</summary>
        /// <returns>The current state of the batch reader.</returns>
        public ODataBatchReaderState State
        {
            get
            {
                this.inputContext.VerifyNotDisposed();
                return this.batchReaderState;
            }

            private set
            {
                this.batchReaderState = value;
            }
        }

        /// <summary> Reads the next part from the batch message payload. </summary>
        /// <returns>True if more items were read; otherwise false.</returns>
        public bool Read()
        {
            this.VerifyCanRead(true);
            return this.InterceptException((Func<bool>)this.ReadSynchronously);
        }

#if PORTABLELIB
        /// <summary>Asynchronously reads the next part from the batch message payload.</summary>
        /// <returns>A task that when completed indicates whether more items were read.</returns>
        public Task<bool> ReadAsync()
        {
            this.VerifyCanRead(false);
            return this.ReadAsynchronously().FollowOnFaultWith(t => this.State = ODataBatchReaderState.Exception);
        }
#endif

        /// <summary>Returns an <see cref="T:Microsoft.OData.ODataBatchOperationRequestMessage" /> for reading the content of a batch operation.</summary>
        /// <returns>A request message for reading the content of a batch operation.</returns>
        public ODataBatchOperationRequestMessage CreateOperationRequestMessage()
        {
            this.VerifyCanCreateOperationRequestMessage(/*synchronousCall*/ true);
            return this.InterceptException((Func<ODataBatchOperationRequestMessage>)this.CreateOperationRequestMessageImplementation);
        }

#if PORTABLELIB
        /// <summary>Asynchronously returns an <see cref="T:Microsoft.OData.ODataBatchOperationRequestMessage" /> for reading the content of a batch operation.</summary>
        /// <returns>A task that when completed returns a request message for reading the content of a batch operation.</returns>
        public Task<ODataBatchOperationRequestMessage> CreateOperationRequestMessageAsync()
        {
            this.VerifyCanCreateOperationRequestMessage(/*synchronousCall*/ false);
            return TaskUtils.GetTaskForSynchronousOperation<ODataBatchOperationRequestMessage>(
                this.CreateOperationRequestMessageImplementation)
                .FollowOnFaultWith(t => this.State = ODataBatchReaderState.Exception);
        }
#endif

        /// <summary>Returns an <see cref="T:Microsoft.OData.ODataBatchOperationResponseMessage" /> for reading the content of a batch operation.</summary>
        /// <returns>A response message for reading the content of a batch operation.</returns>
        public ODataBatchOperationResponseMessage CreateOperationResponseMessage()
        {
            this.VerifyCanCreateOperationResponseMessage(/*synchronousCall*/ true);
            return this.InterceptException((Func<ODataBatchOperationResponseMessage>)this.CreateOperationResponseMessageImplementation);
        }

#if PORTABLELIB
        /// <summary>Asynchronously returns an <see cref="T:Microsoft.OData.ODataBatchOperationResponseMessage" /> for reading the content of a batch operation.</summary>
        /// <returns>A task that when completed returns a response message for reading the content of a batch operation.</returns>
        public Task<ODataBatchOperationResponseMessage> CreateOperationResponseMessageAsync()
        {
            this.VerifyCanCreateOperationResponseMessage(/*synchronousCall*/ false);
            return TaskUtils.GetTaskForSynchronousOperation<ODataBatchOperationResponseMessage>(
                this.CreateOperationResponseMessageImplementation)
                .FollowOnFaultWith(t => this.State = ODataBatchReaderState.Exception);
        }
#endif

        /// <summary>
        /// This method is called to notify that the content stream for a batch operation has been requested.
        /// </summary>
        void IODataBatchOperationListener.BatchOperationContentStreamRequested()
        {
            this.operationState = OperationState.StreamRequested;
        }

#if PORTABLELIB
        /// <summary>
        /// This method is called to notify that the content stream for a batch operation has been requested.
        /// </summary>
        /// <returns>
        /// A task representing any action that is running as part of the status change of the reader;
        /// null if no such action exists.
        /// </returns>
        Task IODataBatchOperationListener.BatchOperationContentStreamRequestedAsync()
        {
            this.operationState = OperationState.StreamRequested;
            return TaskUtils.CompletedTask;
        }
#endif

        /// <summary>
        /// This method is called to notify that the content stream of a batch operation has been disposed.
        /// </summary>
        void IODataBatchOperationListener.BatchOperationContentStreamDisposed()
        {
            this.operationState = OperationState.StreamDisposed;
        }

        /// <summary>
        /// Returns the next state of the batch reader after an end boundary has been found.
        /// </summary>
        /// <returns>The next state of the batch reader.</returns>
        private ODataBatchReaderState GetEndBoundaryState()
        {
            switch (this.batchReaderState)
            {
                case ODataBatchReaderState.Initial:
                    // If we find an end boundary when in state 'Initial' it means that we
                    // have an empty batch. The next state will be 'Completed'.
                    return ODataBatchReaderState.Completed;

                case ODataBatchReaderState.Operation:
                    // If we find an end boundary in state 'Operation' we have finished
                    // processing an operation and found the end boundary of either the
                    // current changeset or the batch.
                    return this.batchStream.ChangeSetBoundary == null
                        ? ODataBatchReaderState.Completed
                        : ODataBatchReaderState.ChangesetEnd;

                case ODataBatchReaderState.ChangesetStart:
                    // If we find an end boundary when in state 'ChangeSetStart' it means that
                    // we have an empty changeset. The next state will be 'ChangeSetEnd'
                    return ODataBatchReaderState.ChangesetEnd;

                case ODataBatchReaderState.ChangesetEnd:
                    // If we are at the end of a changeset and find an end boundary
                    // we reached the end of the batch
                    return ODataBatchReaderState.Completed;

                case ODataBatchReaderState.Completed:
                    // We should never get here when in Completed state.
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataBatchReader_GetEndBoundary_Completed));

                case ODataBatchReaderState.Exception:
                    // We should never get here when in Exception state.
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataBatchReader_GetEndBoundary_Exception));

                default:
                    // Invalid enum value
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataBatchReader_GetEndBoundary_UnknownValue));
            }
        }

        /// <summary>
        /// Reads the next part from the batch message payload.
        /// </summary>
        /// <returns>true if more information was read; otherwise false.</returns>
        private bool ReadSynchronously()
        {
            return this.ReadImplementation();
        }

#if PORTABLELIB
        /// <summary>
        /// Asynchronously reads the next part from the batch message payload.
        /// </summary>
        /// <returns>A task that when completed indicates whether more information was read.</returns>
        private Task<bool> ReadAsynchronously()
        {
            // We are reading from the fully buffered read stream here; thus it is ok
            // to use synchronous reads and then return a completed task
            // NOTE: once we switch to fully async reading this will have to change
            return TaskUtils.GetTaskForSynchronousOperation<bool>(this.ReadImplementation);
        }
#endif

        /// <summary>
        /// Continues reading from the batch message payload.
        /// </summary>
        /// <returns>true if more items were read; otherwise false.</returns>
        private bool ReadImplementation()
        {
            Debug.Assert(this.operationState != OperationState.StreamRequested, "Should have verified that no operation stream is still active.");

            switch (this.State)
            {
                case ODataBatchReaderState.Initial:
                    // The stream should be positioned at the beginning of the batch content,
                    // that is before the first boundary (or the preamble if there is one).
                    this.batchReaderState = this.SkipToNextPartAndReadHeaders();
                    break;

                case ODataBatchReaderState.Operation:
                    // When reaching this state we already read the MIME headers of the operation.
                    // Clients MUST call CreateOperationRequestMessage
                    // or CreateOperationResponseMessage to read at least the headers of the operation.
                    // This is important since we need to read the ContentId header (if present) and
                    // add it to the URL resolver.
                    if (this.operationState == OperationState.None)
                    {
                        // No message was created; fail
                        throw new ODataException(Strings.ODataBatchReader_NoMessageWasCreatedForOperation);
                    }

                    // Reset the operation state; the operation state only
                    // tracks the state of a batch operation while in state Operation.
                    this.operationState = OperationState.None;

                    // Also add a pending ContentId header to the URL resolver now. We ensured above
                    // that a message has been created for this operation and thus the headers (incl.
                    // a potential content ID header) have been read.
                    if (this.contentIdToAddOnNextRead != null)
                    {
                        this.payloadUriConverter.AddContentId(this.contentIdToAddOnNextRead);
                        this.contentIdToAddOnNextRead = null;
                    }

                    // When we are done with an operation, we have to skip ahead to the next part
                    // when Read is called again. Note that if the operation stream was never requested
                    // and the content of the operation has not been read, we'll skip it here.
                    Debug.Assert(this.operationState == OperationState.None, "Operation state must be 'None' at the end of the operation.");
                    this.batchReaderState = this.SkipToNextPartAndReadHeaders();
                    break;

                case ODataBatchReaderState.ChangesetStart:
                    // When at the start of a changeset, skip ahead to the first operation in the
                    // changeset (or the end boundary of the changeset).
                    Debug.Assert(this.batchStream.ChangeSetBoundary != null, "Changeset boundary must have been set by now.");
                    this.batchReaderState = this.SkipToNextPartAndReadHeaders();
                    break;

                case ODataBatchReaderState.ChangesetEnd:
                    // When at the end of a changeset, reset the changeset boundary and the
                    // changeset size and then skip to the next part.
                    this.ResetChangeSetSize();
                    this.batchStream.ResetChangeSetBoundary();
                    this.batchReaderState = this.SkipToNextPartAndReadHeaders();
                    break;

                case ODataBatchReaderState.Exception:    // fall through
                case ODataBatchReaderState.Completed:
                    Debug.Assert(false, "Should have checked in VerifyCanRead that we are not in one of these states.");
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataBatchReader_ReadImplementation));

                default:
                    Debug.Assert(false, "Unsupported reader state " + this.State + " detected.");
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataBatchReader_ReadImplementation));
            }

            return this.batchReaderState != ODataBatchReaderState.Completed && this.batchReaderState != ODataBatchReaderState.Exception;
        }

        /// <summary>
        /// Skips all data in the stream until the next part is detected; then reads the part's request/response line and headers.
        /// </summary>
        /// <returns>The next state of the batch reader after skipping to the next part and reading the part's beginning.</returns>
        private ODataBatchReaderState SkipToNextPartAndReadHeaders()
        {
            bool isEndBoundary, isParentBoundary;
            bool foundBoundary = this.batchStream.SkipToBoundary(out isEndBoundary, out isParentBoundary);

            if (!foundBoundary)
            {
                // We did not find the expected boundary at all in the payload;
                // we are done reading. Depending on where we are report changeset end or completed state
                if (this.batchStream.ChangeSetBoundary == null)
                {
                    return ODataBatchReaderState.Completed;
                }
                else
                {
                    return ODataBatchReaderState.ChangesetEnd;
                }
            }

            ODataBatchReaderState nextState;
            if (isEndBoundary || isParentBoundary)
            {
                // We detected an end boundary or detected that the end boundary is missing
                // because we found a parent boundary
                nextState = this.GetEndBoundaryState();

                if (nextState == ODataBatchReaderState.ChangesetEnd)
                {
                    // Reset the URL resolver at the end of a changeset; Content IDs are
                    // unique within a given changeset.
                    this.payloadUriConverter.Reset();
                }
            }
            else
            {
                bool currentlyInChangeSet = this.batchStream.ChangeSetBoundary != null;
                string contentId;

                // If we did not find an end boundary, we found another part
                bool isChangeSetPart = this.batchStream.ProcessPartHeader(out contentId);

                // Compute the next reader state
                if (currentlyInChangeSet)
                {
                    Debug.Assert(!isChangeSetPart, "Should have validated that nested changesets are not allowed.");
                    nextState = ODataBatchReaderState.Operation;
                    this.IncreaseChangeSetSize();
                }
                else
                {
                    // We are at the top level (not inside a changeset)
                    nextState = isChangeSetPart
                        ? ODataBatchReaderState.ChangesetStart
                        : ODataBatchReaderState.Operation;
                    this.IncreaseBatchSize();
                }

                if (!isChangeSetPart)
                {
                    // Add a potential Content-ID header to the URL resolver so that it will be available
                    // to subsequent operations.
                    Debug.Assert(this.contentIdToAddOnNextRead == null, "Must not have a content ID to be added to a part.");

                    if (contentId != null && this.payloadUriConverter.ContainsContentId(contentId))
                    {
                        throw new ODataException(Strings.ODataBatchReader_DuplicateContentIDsNotAllowed(contentId));
                    }

                    this.contentIdToAddOnNextRead = contentId;
                }
            }

            return nextState;
        }

        /// <summary>
        /// Returns the cached <see cref="ODataBatchOperationRequestMessage"/> for reading the content of an operation
        /// in a batch request.
        /// </summary>
        /// <returns>The message that can be used to read the content of the batch request operation from.</returns>
        private ODataBatchOperationRequestMessage CreateOperationRequestMessageImplementation()
        {
            this.operationState = OperationState.MessageCreated;

            string requestLine = this.batchStream.ReadFirstNonEmptyLine();

            string httpMethod;
            Uri requestUri;
            this.ParseRequestLine(requestLine, out httpMethod, out requestUri);

            // Read all headers and create the request message
            ODataBatchOperationHeaders headers = this.batchStream.ReadHeaders();

            if (this.batchStream.ChangeSetBoundary != null)
            {
                if (this.allowLegacyContentIdBehaviour)
                {
                    // Add a potential Content-ID header to the URL resolver so that it will be available
                    // to subsequent operations.
                    string contentId;
                    if (this.contentIdToAddOnNextRead == null && headers.TryGetValue(ODataConstants.ContentIdHeader, out contentId))
                    {
                        if (contentId != null && this.payloadUriConverter.ContainsContentId(contentId))
                        {
                            throw new ODataException(Strings.ODataBatchReader_DuplicateContentIDsNotAllowed(contentId));
                        }

                        this.contentIdToAddOnNextRead = contentId;
                    }
                }

                if (this.contentIdToAddOnNextRead == null)
                {
                    throw new ODataException(Strings.ODataBatchOperationHeaderDictionary_KeyNotFound(ODataConstants.ContentIdHeader));
                }
            }

            ODataBatchOperationRequestMessage requestMessage = ODataBatchOperationRequestMessage.CreateReadMessage(
                this.batchStream,
                httpMethod,
                requestUri,
                headers,
                /*operationListener*/ this,
                this.contentIdToAddOnNextRead,
                this.payloadUriConverter,
                this.container);

            return requestMessage;
        }

        /// <summary>
        /// Returns the cached <see cref="ODataBatchOperationRequestMessage"/> for reading the content of an operation
        /// in a batch request.
        /// </summary>
        /// <returns>The message that can be used to read the content of the batch request operation from.</returns>
        private ODataBatchOperationResponseMessage CreateOperationResponseMessageImplementation()
        {
            this.operationState = OperationState.MessageCreated;

            string responseLine = this.batchStream.ReadFirstNonEmptyLine();

            int statusCode = this.ParseResponseLine(responseLine);

            // Read all headers and create the response message
            ODataBatchOperationHeaders headers = this.batchStream.ReadHeaders();

            if (this.batchStream.ChangeSetBoundary != null)
            {
                if (this.allowLegacyContentIdBehaviour)
                {
                    // Add a potential Content-ID header to the URL resolver so that it will be available
                    // to subsequent operations.
                    string contentId;
                    if (this.contentIdToAddOnNextRead == null && headers.TryGetValue(ODataConstants.ContentIdHeader, out contentId))
                    {
                        if (contentId != null && this.payloadUriConverter.ContainsContentId(contentId))
                        {
                            throw new ODataException(Strings.ODataBatchReader_DuplicateContentIDsNotAllowed(contentId));
                        }

                        this.contentIdToAddOnNextRead = contentId;
                    }
                }
            }

            // In responses we don't need to use our batch URL resolver, since there are no cross referencing URLs
            // so use the URL resolver from the batch message instead.
            ODataBatchOperationResponseMessage responseMessage = ODataBatchOperationResponseMessage.CreateReadMessage(
                this.batchStream,
                statusCode,
                headers,
                this.contentIdToAddOnNextRead,
                /*operationListener*/ this,
                this.payloadUriConverter.BatchMessagePayloadUriConverter,
                this.container);

            //// NOTE: Content-IDs for cross referencing are only supported in request messages; in responses
            ////       we allow a Content-ID header but don't process it (i.e., don't add the content ID to the URL resolver).

            return responseMessage;
        }

        /// <summary>
        /// Parses the request line of a batch operation request.
        /// </summary>
        /// <param name="requestLine">The request line as a string.</param>
        /// <param name="httpMethod">The parsed HTTP method of the request.</param>
        /// <param name="requestUri">The parsed <see cref="Uri"/> of the request.</param>
        private void ParseRequestLine(string requestLine, out string httpMethod, out Uri requestUri)
        {
            Debug.Assert(!this.inputContext.ReadingResponse, "Must only be called for requests.");

            // Batch Request: POST /Customers HTTP/1.1
            // Since the uri can contain spaces, the only way to read the request url, is to
            // check for first space character and last space character and anything between
            // them.
            int firstSpaceIndex = requestLine.IndexOf(' ');

            // Check whether there are enough characters after the first space for the 2nd and 3rd segments
            // (and a whitespace in between)
            if (firstSpaceIndex <= 0 || requestLine.Length - 3 <= firstSpaceIndex)
            {
                // only 1 segment or empty first segment or not enough left for 2nd and 3rd segments
                throw new ODataException(Strings.ODataBatchReaderStream_InvalidRequestLine(requestLine));
            }

            int lastSpaceIndex = requestLine.LastIndexOf(' ');
            if (lastSpaceIndex < 0 || lastSpaceIndex - firstSpaceIndex - 1 <= 0 || requestLine.Length - 1 <= lastSpaceIndex)
            {
                // only 2 segments or empty 2nd or 3rd segments
                // only 1 segment or empty first segment or not enough left for 2nd and 3rd segments
                throw new ODataException(Strings.ODataBatchReaderStream_InvalidRequestLine(requestLine));
            }

            httpMethod = requestLine.Substring(0, firstSpaceIndex);               // Request - Http method
            string uriSegment = requestLine.Substring(firstSpaceIndex + 1, lastSpaceIndex - firstSpaceIndex - 1);      // Request - Request uri
            string httpVersionSegment = requestLine.Substring(lastSpaceIndex + 1);             // Request - Http version

            // Validate HttpVersion
            if (string.CompareOrdinal(ODataConstants.HttpVersionInBatching, httpVersionSegment) != 0)
            {
                throw new ODataException(Strings.ODataBatchReaderStream_InvalidHttpVersionSpecified(httpVersionSegment, ODataConstants.HttpVersionInBatching));
            }

            // NOTE: this method will throw if the method is not recognized.
            HttpUtils.ValidateHttpMethod(httpMethod);

            // Validate the HTTP method when reading a request
            if (this.batchStream.ChangeSetBoundary != null)
            {
                // allow all methods except for GET
                if (HttpUtils.IsQueryMethod(httpMethod))
                {
                    throw new ODataException(Strings.ODataBatch_InvalidHttpMethodForChangeSetRequest(httpMethod));
                }
            }

            requestUri = new Uri(uriSegment, UriKind.RelativeOrAbsolute);
            requestUri = ODataBatchUtils.CreateOperationRequestUri(requestUri, this.inputContext.MessageReaderSettings.BaseUri, this.payloadUriConverter);
        }

        /// <summary>
        /// Parses the response line of a batch operation response.
        /// </summary>
        /// <param name="responseLine">The response line as a string.</param>
        /// <returns>The parsed status code from the response line.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "'this' is used when built in debug")]
        private int ParseResponseLine(string responseLine)
        {
            Debug.Assert(this.inputContext.ReadingResponse, "Must only be called for responses.");

            // Batch Response: HTTP/1.1 200 Ok
            // Since the http status code strings have spaces in them, we cannot use the same
            // logic. We need to check for the second space and anything after that is the error
            // message.
            int firstSpaceIndex = responseLine.IndexOf(' ');
            if (firstSpaceIndex <= 0 || responseLine.Length - 3 <= firstSpaceIndex)
            {
                // only 1 segment or empty first segment or not enough left for 2nd and 3rd segments
                throw new ODataException(Strings.ODataBatchReaderStream_InvalidResponseLine(responseLine));
            }

            int secondSpaceIndex = responseLine.IndexOf(' ', firstSpaceIndex + 1);
            if (secondSpaceIndex < 0 || secondSpaceIndex - firstSpaceIndex - 1 <= 0 || responseLine.Length - 1 <= secondSpaceIndex)
            {
                // only 2 segments or empty 2nd or 3rd segments
                // only 1 segment or empty first segment or not enough left for 2nd and 3rd segments
                throw new ODataException(Strings.ODataBatchReaderStream_InvalidResponseLine(responseLine));
            }

            string httpVersionSegment = responseLine.Substring(0, firstSpaceIndex);
            string statusCodeSegment = responseLine.Substring(firstSpaceIndex + 1, secondSpaceIndex - firstSpaceIndex - 1);

            // Validate HttpVersion
            if (string.CompareOrdinal(ODataConstants.HttpVersionInBatching, httpVersionSegment) != 0)
            {
                throw new ODataException(Strings.ODataBatchReaderStream_InvalidHttpVersionSpecified(httpVersionSegment, ODataConstants.HttpVersionInBatching));
            }

            int intResult;
            if (!Int32.TryParse(statusCodeSegment, out intResult))
            {
                throw new ODataException(Strings.ODataBatchReaderStream_NonIntegerHttpStatusCode(statusCodeSegment));
            }

            return intResult;
        }

        /// <summary>
        /// Verifies that calling CreateOperationRequestMessage if valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        private void VerifyCanCreateOperationRequestMessage(bool synchronousCall)
        {
            this.VerifyReaderReady();
            this.VerifyCallAllowed(synchronousCall);

            if (this.inputContext.ReadingResponse)
            {
                this.ThrowODataException(Strings.ODataBatchReader_CannotCreateRequestOperationWhenReadingResponse);
            }

            if (this.State != ODataBatchReaderState.Operation)
            {
                this.ThrowODataException(Strings.ODataBatchReader_InvalidStateForCreateOperationRequestMessage(this.State));
            }

            if (this.operationState != OperationState.None)
            {
                this.ThrowODataException(Strings.ODataBatchReader_OperationRequestMessageAlreadyCreated);
            }
        }

        /// <summary>
        /// Verifies that calling CreateOperationResponseMessage if valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        private void VerifyCanCreateOperationResponseMessage(bool synchronousCall)
        {
            this.VerifyReaderReady();
            this.VerifyCallAllowed(synchronousCall);

            if (!this.inputContext.ReadingResponse)
            {
                this.ThrowODataException(Strings.ODataBatchReader_CannotCreateResponseOperationWhenReadingRequest);
            }

            if (this.State != ODataBatchReaderState.Operation)
            {
                this.ThrowODataException(Strings.ODataBatchReader_InvalidStateForCreateOperationResponseMessage(this.State));
            }

            if (this.operationState != OperationState.None)
            {
                this.ThrowODataException(Strings.ODataBatchReader_OperationResponseMessageAlreadyCreated);
            }
        }

        /// <summary>
        /// Verifies that calling Read is valid.
        /// </summary>
        /// <param name="synchronousCall">true if the call is to be synchronous; false otherwise.</param>
        private void VerifyCanRead(bool synchronousCall)
        {
            this.VerifyReaderReady();
            this.VerifyCallAllowed(synchronousCall);

            if (this.State == ODataBatchReaderState.Exception || this.State == ODataBatchReaderState.Completed)
            {
                throw new ODataException(Strings.ODataBatchReader_ReadOrReadAsyncCalledInInvalidState(this.State));
            }
        }

        /// <summary>
        /// Validates that the batch reader is ready to process a new read or create message request.
        /// </summary>
        private void VerifyReaderReady()
        {
            this.inputContext.VerifyNotDisposed();

            // If the operation stream was requested but not yet disposed, the batch reader can't be used to do anything.
            if (this.operationState == OperationState.StreamRequested)
            {
                throw new ODataException(Strings.ODataBatchReader_CannotUseReaderWhileOperationStreamActive);
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
                if (!this.synchronous)
                {
                    throw new ODataException(Strings.ODataBatchReader_SyncCallOnAsyncReader);
                }
            }
            else
            {
#if PORTABLELIB
                if (this.synchronous)
                {
                    throw new ODataException(Strings.ODataBatchReader_AsyncCallOnSyncReader);
                }
#else
                Debug.Assert(false, "Async calls are not allowed in this build.");
#endif
            }
        }

        /// <summary>
        /// Increases the size of the current batch message; throws if the allowed limit is exceeded.
        /// </summary>
        private void IncreaseBatchSize()
        {
            this.currentBatchSize++;

            if (this.currentBatchSize > this.inputContext.MessageReaderSettings.MessageQuotas.MaxPartsPerBatch)
            {
                throw new ODataException(Strings.ODataBatchReader_MaxBatchSizeExceeded(this.inputContext.MessageReaderSettings.MessageQuotas.MaxPartsPerBatch));
            }
        }

        /// <summary>
        /// Increases the size of the current change set; throws if the allowed limit is exceeded.
        /// </summary>
        private void IncreaseChangeSetSize()
        {
            this.currentChangeSetSize++;

            if (this.currentChangeSetSize > this.inputContext.MessageReaderSettings.MessageQuotas.MaxOperationsPerChangeset)
            {
                throw new ODataException(Strings.ODataBatchReader_MaxChangeSetSizeExceeded(this.inputContext.MessageReaderSettings.MessageQuotas.MaxOperationsPerChangeset));
            }
        }

        /// <summary>
        /// Resets the size of the current change set to 0.
        /// </summary>
        private void ResetChangeSetSize()
        {
            this.currentChangeSetSize = 0;
        }

        /// <summary>
        /// Sets the 'Exception' state and then throws an ODataException with the specified error message.
        /// </summary>
        /// <param name="errorMessage">The error message for the exception.</param>
        private void ThrowODataException(string errorMessage)
        {
            this.State = ODataBatchReaderState.Exception;
            throw new ODataException(errorMessage);
        }

        /// <summary>
        /// Catch any exception thrown by the action passed in; in the exception case move the writer into
        /// state Exception and then rethrow the exception.
        /// </summary>
        /// <typeparam name="T">The type of the result returned from the <paramref name="action"/>.</typeparam>
        /// <param name="action">The action to execute.</param>
        /// <returns>The result of the <paramref name="action"/>.</returns>
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
                    this.State = ODataBatchReaderState.Exception;
                }

                throw;
            }
        }
    }
}

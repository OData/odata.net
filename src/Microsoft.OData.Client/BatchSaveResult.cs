//---------------------------------------------------------------------
// <copyright file="BatchSaveResult.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using Microsoft.OData;
    using Microsoft.OData.Client.Metadata;

    #endregion Namespaces

    /// <summary>
    /// Handles the batch requests and responses (both sync and async)
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "The response stream is disposed by the message reader we create over it which we dispose inside the enumerator.")]
    internal class BatchSaveResult : BaseSaveResult
    {
        #region Private Fields

        /// <summary>The size of the copy buffer to create.</summary>
        private const int StreamCopyBufferSize = 4000;

        /// <summary>Array of queries being executed</summary>
        private readonly DataServiceRequest[] Queries;

        /// <summary>Response stream containing the entire batch response.</summary>
        private Stream responseStream;

        /// <summary>Instance of ODataBatchWriter used to write current batch request.</summary>
        private ODataBatchWriter batchWriter;

        /// <summary>The message reader used to read the batch response.</summary>
        private ODataMessageReader batchMessageReader;

        /// <summary>Object representing the current operation response.</summary>
        private CurrentOperationResponse currentOperationResponse;

        /// <summary>Buffer used for caching operation response body streams.</summary>
        private byte[] streamCopyBuffer;

        #endregion

        /// <summary>
        /// constructor for BatchSaveResult
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="method">method</param>
        /// <param name="queries">queries</param>
        /// <param name="options">options</param>
        /// <param name="callback">user callback</param>
        /// <param name="state">user state object</param>
        internal BatchSaveResult(DataServiceContext context, string method, DataServiceRequest[] queries, SaveChangesOptions options, AsyncCallback callback, object state)
            : base(context, method, queries, options, callback, state)
        {
            Debug.Assert(Util.IsBatch(options), "the options must have batch  flag set");
            this.Queries = queries;
            this.streamCopyBuffer = new byte[StreamCopyBufferSize];
        }

        /// <summary>returns true since this class handles batch requests.</summary>
        internal override bool IsBatchRequest
        {
            get { return true; }
        }

        /// <summary>
        /// In async case, this is a memory stream used to cache responses, as we are reading async from the underlying http web response stream.
        /// In non-async case, this is the actual response stream, as returned by the http request.
        /// </summary>
        /// <remarks>
        /// This is the stream which holds the entire batch response, when we process any given part those streams are enumerated through
        /// a different field (currentOperationResponseContentStream).
        /// </remarks>
        protected override Stream ResponseStream
        {
            get { return this.responseStream; }
        }

        /// <summary>
        /// returns true if the response payload needs to be processed.
        /// </summary>
        protected override bool ProcessResponsePayload
        {
            get
            {
                Debug.Assert(this.currentOperationResponse != null, "There must be an active operation response for this property to work correctly.");
                return !this.currentOperationResponse.HasEmptyContent;
            }
        }

        /// <summary>initial the async batch save changeset</summary>
        internal void BatchBeginRequest()
        {
            PerRequest pereq = null;
            try
            {
                ODataRequestMessageWrapper batchRequestMessage = this.GenerateBatchRequest();
                this.Abortable = batchRequestMessage;

                if (batchRequestMessage != null)
                {
                    batchRequestMessage.SetContentLengthHeader();
                    this.perRequest = pereq = new PerRequest();
                    pereq.Request = batchRequestMessage;
                    pereq.RequestContentStream = batchRequestMessage.CachedRequestStream;

                    AsyncStateBag asyncStateBag = new AsyncStateBag(pereq);

                    this.responseStream = new MemoryStream();
                    IAsyncResult asyncResult = BaseAsyncResult.InvokeAsync(batchRequestMessage.BeginGetRequestStream, this.AsyncEndGetRequestStream, asyncStateBag);
                    pereq.SetRequestCompletedSynchronously(asyncResult.CompletedSynchronously);
                }
                else
                {
                    Debug.Assert(this.CompletedSynchronously, "completedSynchronously");
                    Debug.Assert(this.IsCompletedInternally, "completed");
                }
            }
            catch (Exception e)
            {
                this.HandleFailure(pereq, e);
                throw; // to user on BeginSaveChangeSet, will still invoke Callback
            }
            finally
            {
                this.HandleCompleted(pereq); // will invoke user callback
            }

            Debug.Assert((this.CompletedSynchronously && this.IsCompleted) || !this.CompletedSynchronously, "sync without complete");
        }

#if !PORTABLELIB  // Synchronous methods not available
        /// <summary>
        /// Synchronous batch request
        /// </summary>
        internal void BatchRequest()
        {
            ODataRequestMessageWrapper batchRequestMessage = this.GenerateBatchRequest();

            if (batchRequestMessage != null)
            {
                batchRequestMessage.SetRequestStream(batchRequestMessage.CachedRequestStream);

                try
                {
                    this.batchResponseMessage = this.RequestInfo.GetSyncronousResponse(batchRequestMessage, false);
                }
                catch (DataServiceTransportException ex)
                {
                    InvalidOperationException exception = WebUtil.GetHttpWebResponse(ex, ref this.batchResponseMessage);

                    // For non-async batch requests we rethrow the WebException.  This is shipped behavior.
                    throw exception;
                }
                finally
                {
                    if (this.batchResponseMessage != null)
                    {
                        // For non-async batch requests we call the test hook to get the response stream but we cannot consume it
                        // because we rethrow what we caught and the customer need to be able to read the response stream from the WebException.
                        // Note that on the async batch code path we do consume the response stream and throw a DataServiceRequestException.
                        this.responseStream = this.batchResponseMessage.GetStream();
                    }
                }
            }
        }
#endif

        /// <summary>Read and store response data for the current change</summary>
        /// <param name="pereq">The completed per request object</param>
        /// <remarks>This is called only from the async code paths, when the response to the batch has been read fully.</remarks>
        protected override void FinishCurrentChange(PerRequest pereq)
        {
            base.FinishCurrentChange(pereq);

            // This resets the position in the buffered response stream to the beginning
            // so that we can start reading the response.
            // In this case the ResponseStream is always a MemoryStream since we cache the async response.
            this.ResponseStream.Position = 0;
            this.perRequest = null;
            this.SetCompleted();
        }

        /// <summary>IODataResponseMessage contain response for the operation.</summary>
        /// <param name="responseMessage">IODataResponseMessage instance.</param>
        protected override void HandleOperationResponse(IODataResponseMessage responseMessage)
        {
            Debug.Assert(false, "This method should never be called for batch scenarios");
            Error.ThrowInternalError(InternalError.InvalidHandleOperationResponse);
        }

        /// <summary>
        /// Handle response.
        /// </summary>
        /// <returns>an instance of the DataServiceResponse, containing individual operation responses for this batch request.</returns>
        protected override DataServiceResponse HandleResponse()
        {
            Debug.Assert(this.currentOperationResponse == null, "Batch response processing is already in-flight, we shouldn't get here now.");

            // This will process the responses and throw if failure was detected
            if (this.ResponseStream != null)
            {
                return this.HandleBatchResponse();
            }

            return new DataServiceResponse(null, (int)WebExceptionStatus.Success, new List<OperationResponse>(0), true /*batchResponse*/);
        }

        /// <summary>
        /// Get the materializer to process the response.
        /// </summary>
        /// <param name="entityDescriptor">entity descriptor whose response is getting materialized.</param>
        /// <param name="responseInfo">information about the response to be materialized.</param>
        /// <returns>an instance of MaterializeAtom, that can be used to materialize the response.</returns>
        /// <remarks>
        /// This can only be called from inside the HandleBatchResponse or during enumeration of the responses.
        /// This is used when processing responses for update operations.
        /// </remarks>
        protected override MaterializeAtom GetMaterializer(EntityDescriptor entityDescriptor, ResponseInfo responseInfo)
        {
            // check if the batch stream is empty or not
            Debug.Assert(this.currentOperationResponse != null, "There must be an active operation response for this method to work correctly.");
            Debug.Assert(!this.currentOperationResponse.HasEmptyContent, "We should not get here if the response is empty.");

            // Since this is used for processing responses to update operations there are no projections to apply.
            QueryComponents queryComponents = new QueryComponents(
                /*uri*/ null,
                Util.ODataVersionEmpty,
                entityDescriptor.Entity.GetType(),
                /*projection*/ null,
                /*normalizerRewrites*/ null);
            return new MaterializeAtom(
                responseInfo,
                queryComponents,
                /*projectionPlan*/ null,
                this.currentOperationResponse.CreateResponseMessage(),
                ODataPayloadKind.Resource);
        }

        /// <summary>
        /// Returns the request message to write the headers and payload into.
        /// </summary>
        /// <param name="method">Http method for the request.</param>
        /// <param name="requestUri">Base Uri for the request.</param>
        /// <param name="headers">Request headers.</param>
        /// <param name="httpStack">HttpStack to use.</param>
        /// <param name="descriptor">Descriptor for the request, if there is one.</param>
        /// <param name="contentId">Content-ID header that could be used in batch request.</param>
        /// <returns>an instance of IODataRequestMessage.</returns>
        protected override ODataRequestMessageWrapper CreateRequestMessage(string method, Uri requestUri, HeaderCollection headers, HttpStack httpStack, Descriptor descriptor, string contentId)
        {
            BuildingRequestEventArgs args = this.RequestInfo.CreateRequestArgsAndFireBuildingRequest(method, requestUri, headers, this.RequestInfo.HttpStack, descriptor);
            return ODataRequestMessageWrapper.CreateBatchPartRequestMessage(this.batchWriter, args, this.RequestInfo, contentId);
        }

        /// <summary>
        /// Creates the type of the multi part MIME content.
        /// </summary>
        /// <returns>A multipart mime header with a generated batch boundary</returns>
        private static string CreateMultiPartMimeContentType()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}; {1}={2}_{3}", XmlConstants.MimeMultiPartMixed, XmlConstants.HttpMultipartBoundary, XmlConstants.HttpMultipartBoundaryBatch, Guid.NewGuid());
        }

        /// <summary>
        /// Creates a ODataRequestMessage for batch request.
        /// </summary>
        /// <returns>Returns an instance of ODataRequestMessage for the batch request.</returns>
        private ODataRequestMessageWrapper CreateBatchRequest()
        {
            Uri requestUri = UriUtil.CreateUri(this.RequestInfo.BaseUriResolver.GetBaseUriWithSlash(), UriUtil.CreateUri("$batch", UriKind.Relative));
            HeaderCollection headers = new HeaderCollection();
            headers.SetRequestVersion(Util.ODataVersion4, this.RequestInfo.MaxProtocolVersionAsVersion);
            headers.SetHeader(XmlConstants.HttpContentType, CreateMultiPartMimeContentType());
            this.RequestInfo.Format.SetRequestAcceptHeaderForBatch(headers);

            return this.CreateTopLevelRequest(XmlConstants.HttpMethodPost, requestUri, headers, this.RequestInfo.HttpStack, null /*descriptor*/);
        }

        /// <summary>
        /// Generate the batch request for all changes to save.
        /// </summary>
        /// <returns>Returns the instance of ODataRequestMessage containing all the headers and payload for the batch request.</returns>
        private ODataRequestMessageWrapper GenerateBatchRequest()
        {
            if (this.ChangedEntries.Count == 0 && this.Queries == null)
            {
                this.SetCompleted();
                return null;
            }

            ODataRequestMessageWrapper batchRequestMessage = this.CreateBatchRequest();

            // we need to fire request after the headers have been written, but before we write the payload
            batchRequestMessage.FireSendingRequest2(null);

            using (ODataMessageWriter messageWriter = Serializer.CreateMessageWriter(batchRequestMessage, this.RequestInfo, false /*isParameterPayload*/))
            {
                this.batchWriter = messageWriter.CreateODataBatchWriter();
                this.batchWriter.WriteStartBatch();

                if (this.Queries != null)
                {
                    foreach (DataServiceRequest query in this.Queries)
                    {
                        QueryComponents queryComponents = query.QueryComponents(this.RequestInfo.Model);
                        Uri requestUri = this.RequestInfo.BaseUriResolver.GetOrCreateAbsoluteUri(queryComponents.Uri);

                        Debug.Assert(requestUri != null, "request uri is null");
                        Debug.Assert(requestUri.IsAbsoluteUri, "request uri is not absolute uri");

                        HeaderCollection headers = new HeaderCollection();

                        headers.SetRequestVersion(queryComponents.Version, this.RequestInfo.MaxProtocolVersionAsVersion);

                        this.RequestInfo.Format.SetRequestAcceptHeaderForQuery(headers, queryComponents);

                        ODataRequestMessageWrapper batchOperationRequestMessage = this.CreateRequestMessage(XmlConstants.HttpMethodGet, requestUri, headers, this.RequestInfo.HttpStack, null /*descriptor*/, null /*contentId*/);

                        batchOperationRequestMessage.FireSendingEventHandlers(null /*descriptor*/);
                    }
                }
                else if (0 < this.ChangedEntries.Count)
                {
                    if (Util.IsBatchWithSingleChangeset(this.Options))
                    {
                        this.batchWriter.WriteStartChangeset();
                    }

                    var model = this.RequestInfo.Model;

                    for (int i = 0; i < this.ChangedEntries.Count; ++i)
                    {
                        if (Util.IsBatchWithIndependentOperations(this.Options))
                        {
                            this.batchWriter.WriteStartChangeset();
                        }

                        Descriptor descriptor = this.ChangedEntries[i];
                        if (descriptor.ContentGeneratedForSave)
                        {
                            continue;
                        }

                        EntityDescriptor entityDescriptor = descriptor as EntityDescriptor;
                        if (descriptor.DescriptorKind == DescriptorKind.Entity)
                        {
                            if (entityDescriptor.State == EntityStates.Added)
                            {
                                // We don't support adding MLE/MR in batch mode
                                ClientTypeAnnotation type = model.GetClientTypeAnnotation(model.GetOrCreateEdmType(entityDescriptor.Entity.GetType()));
                                if (type.IsMediaLinkEntry || entityDescriptor.IsMediaLinkEntry)
                                {
                                    throw Error.NotSupported(Strings.Context_BatchNotSupportedForMediaLink);
                                }
                            }
                            else if (entityDescriptor.State == EntityStates.Unchanged || entityDescriptor.State == EntityStates.Modified)
                            {
                                // We don't support PUT for the MR in batch mode
                                // It's OK to PUT the MLE alone inside a batch mode though
                                if (entityDescriptor.SaveStream != null)
                                {
                                    throw Error.NotSupported(Strings.Context_BatchNotSupportedForMediaLink);
                                }
                            }
                        }
                        else if (descriptor.DescriptorKind == DescriptorKind.NamedStream)
                        {
                            // Similar to MR, we do not support adding named streams in batch mode.
                            throw Error.NotSupported(Strings.Context_BatchNotSupportedForNamedStreams);
                        }

                        ODataRequestMessageWrapper operationRequestMessage;
                        if (descriptor.DescriptorKind == DescriptorKind.Entity)
                        {
                            operationRequestMessage = this.CreateRequest(entityDescriptor);
                        }
                        else
                        {
                            operationRequestMessage = this.CreateRequest((LinkDescriptor)descriptor);
                        }

                        // we need to fire request after the headers have been written, but before we write the payload
                        operationRequestMessage.FireSendingRequest2(descriptor);

                        this.CreateChangeData(i, operationRequestMessage);

                        if (Util.IsBatchWithIndependentOperations(this.Options))
                        {
                            this.batchWriter.WriteEndChangeset();
                        }
                    }

                    if (Util.IsBatchWithSingleChangeset(this.Options))
                    {
                        this.batchWriter.WriteEndChangeset();
                    }
                }

                this.batchWriter.WriteEndBatch();
                this.batchWriter.Flush();
            }

            Debug.Assert(this.ChangedEntries.All(o => o.ContentGeneratedForSave), "didn't generated content for all entities/links");
            return batchRequestMessage;
        }

        /// <summary>
        /// process the batch response
        /// </summary>
        /// <returns>an instance of the DataServiceResponse, containing individual operation responses for this batch request.</returns>
        private DataServiceResponse HandleBatchResponse()
        {
            bool batchMessageReaderOwned = true;

            try
            {
                if ((this.batchResponseMessage == null) || (this.batchResponseMessage.StatusCode == (int)HttpStatusCode.NoContent))
                {   // we always expect a response to our batch POST request
                    throw Error.InvalidOperation(Strings.Batch_ExpectedResponse(1));
                }

                Func<Stream> getResponseStream = () => this.ResponseStream;

                // We are not going to use the responseVersion returned from this call, as the $batch request itself doesn't apply versioning
                // of the responses on the root level. The responses are versioned on the part level. (Note that the version on the $batch level
                // is actually used to version the batch itself, but we for now we only recognize a single version so to keep it backward compatible
                // we don't check this here. Also note that the HandleResponse method will verify that we can support the version, that is it's
                // lower than the highest version we understand).
                Version responseVersion;
                BaseSaveResult.HandleResponse(
                    this.RequestInfo,
                    (HttpStatusCode)this.batchResponseMessage.StatusCode,                      // statusCode
                    this.batchResponseMessage.GetHeader(XmlConstants.HttpODataVersion),  // responseVersion
                    getResponseStream,                                                  // getResponseStream
                    true,                                                               // throwOnFailure
                    out responseVersion);

                if (this.ResponseStream == null)
                {
                    Error.ThrowBatchExpectedResponse(InternalError.NullResponseStream);
                }

                // Create the message and the message reader.
                this.batchResponseMessage = new HttpWebResponseMessage(new HeaderCollection(this.batchResponseMessage), this.batchResponseMessage.StatusCode, getResponseStream);
                ODataMessageReaderSettings messageReaderSettings = this.RequestInfo.GetDeserializationInfo(/*mergeOption*/ null).ReadHelper.CreateSettings();

                // No need to pass in any model to the batch reader.
                this.batchMessageReader = new ODataMessageReader(this.batchResponseMessage, messageReaderSettings);
                ODataBatchReader batchReader;
                try
                {
                    batchReader = this.batchMessageReader.CreateODataBatchReader();
                }
                catch (ODataContentTypeException contentTypeException)
                {
                    string mime;
                    Encoding encoding;
                    Exception inner = contentTypeException;
                    ContentTypeUtil.ReadContentType(this.batchResponseMessage.GetHeader(XmlConstants.HttpContentType), out mime, out encoding);
                    if (String.Equals(XmlConstants.MimeTextPlain, mime))
                    {
                        inner = GetResponseText(
                            this.batchResponseMessage.GetStream,
                            (HttpStatusCode)this.batchResponseMessage.StatusCode);
                    }

                    throw Error.InvalidOperation(Strings.Batch_ExpectedContentType(this.batchResponseMessage.GetHeader(XmlConstants.HttpContentType)), inner);
                }

                DataServiceResponse response = this.HandleBatchResponseInternal(batchReader);

                // In case of successful processing of at least the beginning of the batch, the message reader is owned by the returned response
                // (or rather by the IEnumerable of operation responses inside it).
                // It will be disposed once the operation responses are enumerated (since the IEnumerator should be disposed once used).
                // In that case we must NOT dispose it here, since that enumeration can exist long after we return from this method.
                batchMessageReaderOwned = false;

                return response;
            }
            catch (DataServiceRequestException)
            {
                throw;
            }
            catch (InvalidOperationException ex)
            {
                HeaderCollection headers = new HeaderCollection(this.batchResponseMessage);
                int statusCode = this.batchResponseMessage == null ? (int)HttpStatusCode.InternalServerError : (int)this.batchResponseMessage.StatusCode;
                DataServiceResponse response = new DataServiceResponse(headers, statusCode, new OperationResponse[0], this.IsBatchRequest);
                throw new DataServiceRequestException(Strings.DataServiceException_GeneralError, ex, response);
            }
            finally
            {
                if (batchMessageReaderOwned)
                {
                    Util.Dispose(ref this.batchMessageReader);
                }
            }
        }

        /// <summary>
        /// process the batch response
        /// </summary>
        /// <param name="batchReader">The batch reader to use for reading the batch response.</param>
        /// <returns>an instance of the DataServiceResponse, containing individual operation responses for this batch request.</returns>
        /// <remarks>
        /// The message reader for the entire batch response is stored in the this.batchMessageReader.
        /// The message reader is disposable, but this method should not dispose it itself. It will be either disposed by the caller (in case of exception)
        /// or the ownership will be passed to the returned response object (in case of success).
        /// In could also be diposed indirectly by this method when it enumerates through the responses.
        /// </remarks>
        private DataServiceResponse HandleBatchResponseInternal(ODataBatchReader batchReader)
        {
            Debug.Assert(this.batchMessageReader != null, "this.batchMessageReader != null");
            Debug.Assert(batchReader != null, "batchReader != null");

            DataServiceResponse response;
            HeaderCollection headers = new HeaderCollection(this.batchResponseMessage);

            IEnumerable<OperationResponse> responses = this.HandleBatchResponse(batchReader);
            if (this.Queries != null)
            {
                // ExecuteBatch, EndExecuteBatch
                response = new DataServiceResponse(
                    headers,
                    (int)this.batchResponseMessage.StatusCode,
                    responses,
                    true /*batchResponse*/);
            }
            else
            {
                List<OperationResponse> operationResponses = new List<OperationResponse>();
                response = new DataServiceResponse(headers, (int)this.batchResponseMessage.StatusCode, operationResponses, true /*batchResponse*/);
                Exception exception = null;

                // SaveChanges, EndSaveChanges
                // enumerate the entire response
                foreach (ChangeOperationResponse changeOperationResponse in responses)
                {
                    operationResponses.Add(changeOperationResponse);
                    if (Util.IsBatchWithSingleChangeset(this.Options) && exception == null && changeOperationResponse.Error != null)
                    {
                        exception = changeOperationResponse.Error;
                    }

                    // Note that this will dispose the enumerator and this release the batch message reader which is owned
                    // by the enumerable of responses by now.
                }

                // Note that if we encounter any error in a batch request with a single changeset,
                // we throw here since all change operations in the changeset are rolled back on the server.
                // If we encounter any error in a batch request with independent operations, we don't want to throw
                // since some of the operations might succeed.
                // Users need to inspect each OperationResponse to get the exception information from the failed operations.
                if (exception != null)
                {
                    throw new DataServiceRequestException(Strings.DataServiceException_GeneralError, exception, response);
                }
            }

            return response;
        }

        /// <summary>
        /// process the batch response
        /// </summary>
        /// <param name="batchReader">The batch reader to use for reading the batch response.</param>
        /// <returns>enumerable of QueryResponse or null</returns>
        /// <remarks>
        /// The batch message reader for the entire batch response is stored in this.batchMessageReader.
        /// Note that this method takes over the ownership of this reader and must Dispose it if it successfully returns.
        /// </remarks>
        private IEnumerable<OperationResponse> HandleBatchResponse(ODataBatchReader batchReader)
        {
            try
            {
                if (this.batchMessageReader == null)
                {
                    // The enumerable returned by this method can be enumerated multiple times.
                    // In that case it looks like if the method is called multiple times.
                    // This didn't fail in previous versions, it simply returned no results, so we need to do the same.
                    yield break;
                }

                Debug.Assert(batchReader != null, "batchReader != null");

                bool changesetFound = false;
                bool insideChangeset = false;
                int queryCount = 0;
                int operationCount = 0;
                this.entryIndex = 0;
                while (batchReader.Read())
                {
                    switch (batchReader.State)
                    {
                        #region ChangesetStart
                        case ODataBatchReaderState.ChangesetStart:
                            if ((Util.IsBatchWithSingleChangeset(this.Options) && changesetFound) || (operationCount != 0))
                            {
                                // Throw if we encounter multiple changesets when running in batch with single changeset mode
                                // or if we encounter operations outside of a changeset.
                                Error.ThrowBatchUnexpectedContent(InternalError.UnexpectedBeginChangeSet);
                            }

                            insideChangeset = true;
                            break;
                        #endregion

                        #region ChangesetEnd
                        case ODataBatchReaderState.ChangesetEnd:
                            changesetFound = true;
                            operationCount = 0;
                            insideChangeset = false;
                            break;
                        #endregion

                        #region Operation
                        case ODataBatchReaderState.Operation:
                            Exception exception = this.ProcessCurrentOperationResponse(batchReader, insideChangeset);
                            if (!insideChangeset)
                            {
                                #region Get response
                                Debug.Assert(operationCount == 0, "missing an EndChangeSet 2");

                                QueryOperationResponse qresponse = null;
                                try
                                {
                                    if (exception == null)
                                    {
                                        DataServiceRequest query = this.Queries[queryCount];
                                        ResponseInfo responseInfo = this.RequestInfo.GetDeserializationInfo(null /*mergeOption*/);
                                        MaterializeAtom materializer = DataServiceRequest.Materialize(
                                            responseInfo,
                                            query.QueryComponents(this.RequestInfo.Model),
                                            null,
                                            this.currentOperationResponse.Headers.GetHeader(XmlConstants.HttpContentType),
                                            this.currentOperationResponse.CreateResponseMessage(),
                                            query.PayloadKind);
                                        qresponse = QueryOperationResponse.GetInstance(query.ElementType, this.currentOperationResponse.Headers, query, materializer);
                                    }
                                }
                                catch (ArgumentException e)
                                {
                                    exception = e;
                                }
                                catch (FormatException e)
                                {
                                    exception = e;
                                }
                                catch (InvalidOperationException e)
                                {
                                    exception = e;
                                }

                                if (qresponse == null)
                                {
                                    if (this.Queries != null)
                                    {
                                        // this is the normal ExecuteBatch response
                                        DataServiceRequest query = this.Queries[queryCount];

                                        if (this.RequestInfo.IgnoreResourceNotFoundException && this.currentOperationResponse.StatusCode == HttpStatusCode.NotFound)
                                        {
                                            qresponse = QueryOperationResponse.GetInstance(query.ElementType, this.currentOperationResponse.Headers, query, MaterializeAtom.EmptyResults);
                                        }
                                        else
                                        {
                                            qresponse = QueryOperationResponse.GetInstance(query.ElementType, this.currentOperationResponse.Headers, query, MaterializeAtom.EmptyResults);
                                            qresponse.Error = exception;
                                        }
                                    }
                                    else
                                    {
                                        // This is top-level failure for SaveChanges(SaveChangesOptions.BatchWithSingleChangeset) or SaveChanges(SaveChangesOptions.BatchWithIndependentOperations) operations.
                                        // example: server doesn't support batching or number of batch objects exceeded an allowed limit.
                                        // ex could be null if the server responded to SaveChanges with an unexpected success with
                                        // response of batched GETS that did not correspond the original POST/PATCH/PUT/DELETE requests.
                                        // we expect non-null since server should have failed with a non-success code
                                        // and HandleResponse(status, ...) should generate the exception object
                                        throw exception;
                                    }
                                }

                                qresponse.StatusCode = (int)this.currentOperationResponse.StatusCode;
                                queryCount++;
                                yield return qresponse;
                                #endregion
                            }
                            else
                            {
                                #region Update response
                                try
                                {
                                    Descriptor descriptor = this.ChangedEntries[this.entryIndex];
                                    operationCount += this.SaveResultProcessed(descriptor);

                                    if (exception != null)
                                    {
                                        throw exception;
                                    }

                                    this.HandleOperationResponseHeaders(this.currentOperationResponse.StatusCode, this.currentOperationResponse.Headers);
#if DEBUG
                                    this.HandleOperationResponse(descriptor, this.currentOperationResponse.Headers, this.currentOperationResponse.StatusCode);
#else
                                    this.HandleOperationResponse(descriptor, this.currentOperationResponse.Headers);
#endif
                                }
                                catch (Exception e)
                                {
                                    this.ChangedEntries[this.entryIndex].SaveError = e;
                                    exception = e;

                                    if (!CommonUtil.IsCatchableExceptionType(e))
                                    {
                                        throw;
                                    }
                                }

                                ChangeOperationResponse changeOperationResponse =
                                    new ChangeOperationResponse(this.currentOperationResponse.Headers, this.ChangedEntries[this.entryIndex]);
                                changeOperationResponse.StatusCode = (int)this.currentOperationResponse.StatusCode;
                                if (exception != null)
                                {
                                    changeOperationResponse.Error = exception;
                                }

                                operationCount++;
                                this.entryIndex++;
                                yield return changeOperationResponse;
                                #endregion
                            }

                            break;
                        #endregion

                        default:
                            Error.ThrowBatchExpectedResponse(InternalError.UnexpectedBatchState);
                            break;
                    }
                }

                Debug.Assert(batchReader.State == ODataBatchReaderState.Completed, "unexpected batch state");

                // Check for a changeset without response (first line) or GET request without response (second line).
                // either all saved entries must be processed or it was a batch and one of the entries has the error
                if ((this.Queries == null &&
                    (!changesetFound ||
                     0 < queryCount ||
                     this.ChangedEntries.Any(o => o.ContentGeneratedForSave && o.SaveResultWasProcessed == 0) &&
                     (!this.IsBatchRequest || this.ChangedEntries.FirstOrDefault(o => o.SaveError != null) == null))) ||
                    (this.Queries != null && queryCount != this.Queries.Length))
                {
                    throw Error.InvalidOperation(Strings.Batch_IncompleteResponseCount);
                }
            }
            finally
            {
                // Note that this will be called only once the enumeration of all responses is finished and the Dispose
                // was called on the IEnumerator used for that enumeration. It is not called when the method returns,
                // since the compiler change this method to return the compiler-generated IEnumerable.
                Util.Dispose(ref this.batchMessageReader);
            }
        }

        /// <summary>
        /// Processed the operation response reported by the batch reader.
        /// This is a side-effecting method that is tied deeply to how it is used in the batch processing pipeline.
        /// </summary>
        /// <param name="batchReader">The batch reader to get the operation response from.</param>
        /// <param name="isChangesetOperation">True if the current operation is inside a changeset (implying CUD, not query)</param>
        /// <returns>An exception if the operation response is an error response, null for success response.</returns>
        private Exception ProcessCurrentOperationResponse(ODataBatchReader batchReader, bool isChangesetOperation)
        {
            Debug.Assert(batchReader != null, "batchReader != null");
            Debug.Assert(batchReader.State == ODataBatchReaderState.Operation, "This method requires the batch reader to be on an operation.");

            ODataBatchOperationResponseMessage operationResponseMessage = batchReader.CreateOperationResponseMessage();
            Descriptor descriptor = null;

            if (isChangesetOperation)
            {
                // We need to peek at the content-Id before handing the response to the user, so we can expose the Descriptor them.
                // We're OK with this exception to our general rule of not using them before ReceivingResponse event is fired.
                this.entryIndex = this.ValidateContentID(operationResponseMessage.ContentId);
                descriptor = this.ChangedEntries[entryIndex];
            }

            // If we hit en error inside a batch, we will never expose a descriptor since we don't know which one to return.
            // The descriptor we fetched above based on the content-ID is bogus because the server returns an errounous content-id when
            // it hits an error inside batch.
            if (!WebUtil.SuccessStatusCode((HttpStatusCode)operationResponseMessage.StatusCode))
            {
                descriptor = null;
            }

            this.RequestInfo.Context.FireReceivingResponseEvent(new ReceivingResponseEventArgs(operationResponseMessage, descriptor, true));

            // We need to know if the content of the operation response is empty or not.
            // We also need to cache the entire content, since in case of GET response the response itself will be parsed
            // lazily and so it can happen that we will move the batch reader after this operation before we actually read
            // the content of the operation.
            Stream originalOperationResponseContentStream = operationResponseMessage.GetStream();
            if (originalOperationResponseContentStream == null)
            {
                Error.ThrowBatchExpectedResponse(InternalError.NullResponseStream);
            }

            MemoryStream operationResponseContentStream;
            try
            {
                operationResponseContentStream = new MemoryStream();
                WebUtil.CopyStream(originalOperationResponseContentStream, operationResponseContentStream, ref this.streamCopyBuffer);
                operationResponseContentStream.Position = 0;
            }
            finally
            {
                originalOperationResponseContentStream.Dispose();
            }

            this.currentOperationResponse = new CurrentOperationResponse(
                (HttpStatusCode)operationResponseMessage.StatusCode,
                operationResponseMessage.Headers,
                operationResponseContentStream);

            Version responseVersion;
            string headerName = XmlConstants.HttpODataVersion;
            return BaseSaveResult.HandleResponse(
                this.RequestInfo,
                this.currentOperationResponse.StatusCode,
                this.currentOperationResponse.Headers.GetHeader(headerName),
                () => this.currentOperationResponse.ContentStream,
                false,
                out responseVersion);
        }

        /// <summary>
        /// Validate the content-id.
        /// </summary>
        /// <param name="contentIdStr">The contentId read from ChangeSetHead.</param>
        /// <returns>Returns the correct ChangedEntries index.</returns>
        private int ValidateContentID(string contentIdStr)
        {
            int contentID = 0;

            if (string.IsNullOrEmpty(contentIdStr) ||
                !Int32.TryParse(contentIdStr, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out contentID))
            {
                Error.ThrowBatchUnexpectedContent(InternalError.ChangeResponseMissingContentID);
            }

            for (int i = 0; i < this.ChangedEntries.Count; ++i)
            {
                if (this.ChangedEntries[i].ChangeOrder == contentID)
                {
                    return i;
                }
            }

            Error.ThrowBatchUnexpectedContent(InternalError.ChangeResponseUnknownContentID);
            return -1;
        }

        /// <summary>
        /// Stores information about the currenly processed operation response.
        /// </summary>
        private sealed class CurrentOperationResponse
        {
            /// <summary>The HTTP response status code for the current operation response.</summary>
            private readonly HttpStatusCode statusCode;

            /// <summary>The HTTP headers for the current operation response.</summary>
            private readonly HeaderCollection headers;

            /// <summary>The content stream for the current operation response.</summary>
            private readonly MemoryStream contentStream;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="statusCode">The status code of the response.</param>
            /// <param name="headers">The response headers.</param>
            /// <param name="contentStream">An in-memory copy of the response stream.</param>
            public CurrentOperationResponse(HttpStatusCode statusCode, IEnumerable<KeyValuePair<string, string>> headers, MemoryStream contentStream)
            {
                Debug.Assert(headers != null, "headers != null");
                Debug.Assert(contentStream != null, "contentStream != null");
                Debug.Assert(contentStream.Position == 0, "The stream should have been reset to the begining.");

                this.statusCode = statusCode;
                this.contentStream = contentStream;

                this.headers = new HeaderCollection();
                foreach (KeyValuePair<string, string> operationResponseHeader in headers)
                {
                    this.headers.SetHeader(operationResponseHeader.Key, operationResponseHeader.Value);
                }
            }

            /// <summary>
            /// The status code of the operation response.
            /// </summary>
            public HttpStatusCode StatusCode
            {
                get
                {
                    return this.statusCode;
                }
            }

            /// <summary>
            /// The content stream of the operation response.
            /// </summary>
            public Stream ContentStream
            {
                get
                {
                    return this.contentStream;
                }
            }

            /// <summary>
            /// true if the content stream is empty, false otherwise.
            /// </summary>
            public bool HasEmptyContent
            {
                get
                {
                    return this.contentStream.Length == 0;
                }
            }

            /// <summary>
            /// The response headers for the operation response.
            /// </summary>
            public HeaderCollection Headers
            {
                get
                {
                    return this.headers;
                }
            }

            /// <summary>
            /// Creates IODataResponseMessage for the operation response.
            /// </summary>
            /// <returns>
            /// IODataResponseMessage for the operation response.
            /// null if the operation response has empty content.
            /// </returns>
            public IODataResponseMessage CreateResponseMessage()
            {
                return this.HasEmptyContent
                    ? null
                    : new HttpWebResponseMessage(this.headers, (int)this.statusCode, () => this.contentStream);
            }
        }
    }
}

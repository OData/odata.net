//---------------------------------------------------------------------
// <copyright file="SaveResult.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    #region Namespaces

    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Collections.Generic;
    using Microsoft.OData;
    using Microsoft.OData.Client.Materialization;
    using Microsoft.OData.Client.Metadata;

    #endregion Namespaces

    /// <summary>
    /// Handle the request (both sync and async) for non batch scenarios
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Pending")]
    internal class SaveResult : BaseSaveResult
    {
        /// <summary>keeps track of all the parsed responses.</summary>
        private readonly List<CachedResponse> cachedResponses;

        /// <summary>
        /// We cache the current response and then parse it. we need to do this for the async case only.
        /// </summary>
        private MemoryStream inMemoryResponseStream;

        /// <summary>http web response</summary>
        private IODataResponseMessage responseMessage;

        /// <summary>remove it later</summary>
        private CachedResponse cachedResponse;

        /// <summary>
        /// constructor for SaveResult
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="method">method</param>
        /// <param name="options">options</param>
        /// <param name="callback">user callback</param>
        /// <param name="state">user state object</param>
        internal SaveResult(DataServiceContext context, string method, SaveChangesOptions options, AsyncCallback callback, object state)
            : base(context, method, null, options, callback, state)
        {
            Debug.Assert(!Util.IsBatch(this.Options), "Util.IsBatch(this.Options) is not set");

            this.cachedResponses = new List<CachedResponse>();
        }

        /// <summary>returns false since this class handles only non-batch scenarios</summary>
        internal override bool IsBatchRequest
        {
            get { return false; }
        }

        /// <summary>
        /// returns true if the payload needs to be processed.
        /// </summary>
        protected override bool ProcessResponsePayload
        {
            get
            {
                Debug.Assert(this.cachedResponse.Exception == null, "no exception should have been encountered");
                return this.cachedResponse.MaterializerEntry != null;
            }
        }

        /// <summary>
        /// In async case, this is a memory stream used to cache responses, as we are reading async from the underlying http web response stream.
        /// In non-async case, this is the actual response stream, as returned by the http request.
        /// </summary>
        protected override Stream ResponseStream
        {
            get { return this.inMemoryResponseStream; }
        }

        /// <summary>
        /// This starts the next change
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Pending")]
        internal void BeginCreateNextChange()
        {
            Debug.Assert(!this.IsCompletedInternally, "why being called if already completed?");

            // create the memory stream required to cache the responses as we read async from the underlying http web response
            this.inMemoryResponseStream = new MemoryStream();

            // SaveCallback can't chain synchronously completed responses, caller will loop the to next change
            PerRequest pereq = null;
            IAsyncResult asyncResult = null;
            do
            {
                IODataResponseMessage responseMsg = null;
                ODataRequestMessageWrapper requestMessage = null;
                try
                {
                    if (null != this.perRequest)
                    {
                        this.SetCompleted();
                        Error.ThrowInternalError(InternalError.InvalidBeginNextChange);
                    }

                    requestMessage = this.CreateNextRequest();

                    // Keeping the old behavior (V1/V2) where the abortable was set to null,
                    // if CreateNextRequest returned null.
                    if (requestMessage == null)
                    {
                        this.Abortable = null;
                    }

                    if ((null != requestMessage) || (this.entryIndex < this.ChangedEntries.Count))
                    {
                        if (this.ChangedEntries[this.entryIndex].ContentGeneratedForSave)
                        {
                            Debug.Assert(this.ChangedEntries[this.entryIndex] is LinkDescriptor, "only expected RelatedEnd to presave");
                            Debug.Assert(
                                this.ChangedEntries[this.entryIndex].State == EntityStates.Added ||
                                this.ChangedEntries[this.entryIndex].State == EntityStates.Modified,
                                "only expected added to presave");
                            continue;
                        }

                        this.Abortable = requestMessage;
                        ContentStream contentStream = this.CreateNonBatchChangeData(this.entryIndex, requestMessage);
                        this.perRequest = pereq = new PerRequest();
                        pereq.Request = requestMessage;

                        AsyncStateBag asyncStateBag = new AsyncStateBag(pereq);

                        if (null == contentStream || null == contentStream.Stream)
                        {
                            asyncResult = BaseAsyncResult.InvokeAsync(requestMessage.BeginGetResponse, this.AsyncEndGetResponse, asyncStateBag);
                        }
                        else
                        {
                            if (contentStream.IsKnownMemoryStream)
                            {
                                requestMessage.SetContentLengthHeader();
                            }

                            pereq.RequestContentStream = contentStream;
                            asyncResult = BaseAsyncResult.InvokeAsync(requestMessage.BeginGetRequestStream, this.AsyncEndGetRequestStream, asyncStateBag);
                        }

                        pereq.SetRequestCompletedSynchronously(asyncResult.CompletedSynchronously);
                        this.SetCompletedSynchronously(pereq.RequestCompletedSynchronously);
                    }
                    else
                    {
                        this.SetCompleted();

                        if (this.CompletedSynchronously)
                        {
                            this.HandleCompleted(pereq);
                        }
                    }
                }
                catch (InvalidOperationException e)
                {
                    e = WebUtil.GetHttpWebResponse(e, ref responseMsg);
                    this.HandleOperationException(e, responseMsg);
                    this.HandleCompleted(pereq);
                }
                finally
                {
                    WebUtil.DisposeMessage(responseMsg);
                }

                // If the current request is completed synchronously, we need to call FinishCurrentChange() to process the response payload.
                // FinishCurrentChange() will not call BeginCreateNextChange() when the request is synchronous.
                // If the current request is completed asynchronously, an async thread will call FinishCurrentChange() to process the response payload.
                // FinishCurrentchange() will then call BeginCreateNextChange() from the async thread and we need to exit this loop.
                // If requestMessage = this.CreateNextRequest() returns null, we would have called this.SetCompleted() above and this.IsCompletedInternally
                // would be true. This means we are done processing all changed entries and we should not call this.FinishCurrentChange().
                if (null != pereq && pereq.RequestCompleted && pereq.RequestCompletedSynchronously && !this.IsCompletedInternally)
                {
                    Debug.Assert(requestMessage != null, "httpWebRequest != null");
                    this.FinishCurrentChange(pereq);
                }

                // In the condition for the do-while loop we must test for pereq.RequestCompletedSynchronously and not asyncResult.CompletedSynchronously.
                // pereq.RequestCompletedSynchronously is true only if all async calls completed synchronously.  If we don't exit this loop when
                // pereq.RequestCompletedSynchronously is false, the current thread and an async thread will both re-enter BeginCreateNextChange()
                // and we will fail.  We can only process one request at a given time.
            }
            while (((null == pereq) || (pereq.RequestCompleted && pereq.RequestCompletedSynchronously)) && !this.IsCompletedInternally);
            Debug.Assert((this.CompletedSynchronously && this.IsCompleted) || !this.CompletedSynchronously, "sync without complete");
            Debug.Assert(this.entryIndex < this.ChangedEntries.Count || this.ChangedEntries.All(o => o.ContentGeneratedForSave), "didn't generate content for all entities/links");
        }

#if !PORTABLELIB// Synchronous methods not available
        /// <summary>
        /// This starts the next change
        /// </summary>
        internal void CreateNextChange()
        {
            ODataRequestMessageWrapper requestMessage = null;

            do
            {
                IODataResponseMessage responseMsg = null;

                try
                {
                    requestMessage = this.CreateNextRequest();
                    if ((null != requestMessage) || (this.entryIndex < this.ChangedEntries.Count))
                    {
                        if (this.ChangedEntries[this.entryIndex].ContentGeneratedForSave)
                        {
                            Debug.Assert(this.ChangedEntries[this.entryIndex] is LinkDescriptor, "only expected RelatedEnd to presave");
                            Debug.Assert(
                                this.ChangedEntries[this.entryIndex].State == EntityStates.Added ||
                                this.ChangedEntries[this.entryIndex].State == EntityStates.Modified,
                                "only expected added to presave");
                            continue;
                        }

                        ContentStream contentStream = this.CreateNonBatchChangeData(this.entryIndex, requestMessage);
                        if (null != contentStream && null != contentStream.Stream)
                        {
                            requestMessage.SetRequestStream(contentStream);
                        }

                        responseMsg = this.RequestInfo.GetSyncronousResponse(requestMessage, false);

                        this.HandleOperationResponse(responseMsg);
                        this.HandleOperationResponseHeaders((HttpStatusCode)responseMsg.StatusCode, new HeaderCollection(responseMsg));
                        this.HandleOperationResponseData(responseMsg);
                        this.perRequest = null;
                    }
                }
                catch (InvalidOperationException e)
                {
                    e = WebUtil.GetHttpWebResponse(e, ref responseMsg);
                    this.HandleOperationException(e, responseMsg);
                }
                finally
                {
                    WebUtil.DisposeMessage(responseMsg);
                }

                // we have no more pending requests or there has been an error in the previous request and we decided not to continue
                // (When an error occurs and we are not going to continue on error, we call SetCompleted
            }
            while (this.entryIndex < this.ChangedEntries.Count && !this.IsCompletedInternally);

            Debug.Assert(this.entryIndex < this.ChangedEntries.Count || this.ChangedEntries.All(o => o.ContentGeneratedForSave), "didn't generate content for all entities/links");
        }
#endif

        /// <summary>Read and store response data for the current change, and try to start the next one</summary>
        /// <param name="pereq">the completed per request object</param>
        protected override void FinishCurrentChange(PerRequest pereq)
        {
            base.FinishCurrentChange(pereq);

            Debug.Assert(this.ResponseStream != null, "this.HttpWebResponseStream != null");
            Debug.Assert((this.ResponseStream as MemoryStream) != null, "(this.HttpWebResponseStream as MemoryStream) != null");

            if (this.ResponseStream.Position != 0)
            {
                // Set the stream to the start position and then parse the response and cache it
                this.ResponseStream.Position = 0;
                this.HandleOperationResponseData(this.responseMessage, this.ResponseStream);
            }
            else
            {
                this.HandleOperationResponseData(this.responseMessage, null);
            }

            pereq.Dispose();
            this.perRequest = null;

            if (!pereq.RequestCompletedSynchronously)
            {   // you can't chain synchronously completed responses without risking StackOverflow, caller will loop to next
                if (!this.IsCompletedInternally)
                {
                    this.BeginCreateNextChange();
                }
            }
        }

        /// <summary>IODataResponseMessage contain response for the operation.</summary>
        /// <param name="responseMsg">IODataResponseMessage instance.</param>
        protected override void HandleOperationResponse(IODataResponseMessage responseMsg)
        {
            this.responseMessage = responseMsg;
        }

        /// <summary>
        /// Handle the response.
        /// </summary>
        /// <returns>an instance of the DataServiceResponse, containing individual responses for all the requests made during this SaveChanges call.</returns>
        protected override DataServiceResponse HandleResponse()
        {
            List<OperationResponse> responses = new List<OperationResponse>(this.cachedResponses != null ? this.cachedResponses.Count : 0);
            DataServiceResponse service = new DataServiceResponse(null, -1, responses, false /*isBatch*/);
            Exception ex = null;

            try
            {
                foreach (CachedResponse response in this.cachedResponses)
                {
                    Descriptor descriptor = response.Descriptor;
                    this.SaveResultProcessed(descriptor);
                    OperationResponse operationResponse = new ChangeOperationResponse(response.Headers, descriptor);
                    operationResponse.StatusCode = (int)response.StatusCode;
                    if (response.Exception != null)
                    {
                        operationResponse.Error = response.Exception;

                        if (ex == null)
                        {
                            ex = response.Exception;
                        }
                    }
                    else
                    {
                        this.cachedResponse = response;
#if DEBUG
                        this.HandleOperationResponse(descriptor, response.Headers, response.StatusCode);
#else
                        this.HandleOperationResponse(descriptor, response.Headers);
#endif
                    }

                    responses.Add(operationResponse);
                }
            }
            catch (InvalidOperationException e)
            {
                ex = e;
            }

            if (ex != null)
            {
                throw new DataServiceRequestException(Strings.DataServiceException_GeneralError, ex, service);
            }

            return service;
        }

        /// <summary>
        /// Get the materializer to process the response.
        /// </summary>
        /// <param name="entityDescriptor">entity descriptor whose response is getting materialized.</param>
        /// <param name="responseInfo">information about the response to be materialized.</param>
        /// <returns>an instance of MaterializeAtom, that can be used to materialize the response.</returns>
        protected override MaterializeAtom GetMaterializer(EntityDescriptor entityDescriptor, ResponseInfo responseInfo)
        {
            Debug.Assert(this.cachedResponse.Exception == null && this.cachedResponse.MaterializerEntry != null, "this.cachedResponse.Exception == null && this.cachedResponse.Entry != null");
            ODataResource entry = this.cachedResponse.MaterializerEntry == null ? null : this.cachedResponse.MaterializerEntry.Entry;
            return new MaterializeAtom(responseInfo, new[] { entry }, entityDescriptor.Entity.GetType(), this.cachedResponse.MaterializerEntry.Format);
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
            return this.CreateTopLevelRequest(method, requestUri, headers, httpStack, descriptor);
        }

        /// <summary>
        /// Create memory stream for descriptor (entity or link or MR or named stream).
        /// </summary>
        /// <param name="index">Index into changed entries.</param>
        /// <param name="requestMessage">RequestMessage to be used to generate the payload.</param>
        /// <returns>Stream of data for descriptor.</returns>
        protected ContentStream CreateNonBatchChangeData(int index, ODataRequestMessageWrapper requestMessage)
        {
            Descriptor descriptor = this.ChangedEntries[index];
            Debug.Assert(!descriptor.ContentGeneratedForSave, "already saved entity/link");
            Debug.Assert(!this.IsBatchRequest, "we do not support MR requests in batch");

            if (descriptor.DescriptorKind == DescriptorKind.Entity && this.streamRequestKind != StreamRequestKind.None)
            {
                if (this.streamRequestKind != StreamRequestKind.None)
                {
                    Debug.Assert(
                        this.streamRequestKind == StreamRequestKind.PutMediaResource || descriptor.State == EntityStates.Modified,
                        "We should have modified the MLE state to Modified when we've created the MR POST request.");
                    Debug.Assert(
                        this.streamRequestKind != StreamRequestKind.PutMediaResource || (descriptor.State == EntityStates.Unchanged || descriptor.State == EntityStates.Modified),
                        "If we're processing MR PUT the entity must be either in Unchanged or Modified state.");

                    // media resource request - we already precreated the body of the request
                    // in the CheckAndProcessMediaEntryPost or CheckAndProcessMediaEntryPut method.
                    Debug.Assert(this.mediaResourceRequestStream != null, "We should have precreated the MR stream already.");
                    return new ContentStream(this.mediaResourceRequestStream, false);
                }
            }
            else if (descriptor.DescriptorKind == DescriptorKind.NamedStream)
            {
                Debug.Assert(!this.IsBatchRequest, "we do not support named stream requests in batch");

                // named stream request - we already precreated the body of the request in CreateNamedStreamRequest method
                Debug.Assert(this.mediaResourceRequestStream != null, "We should have precreated the MR stream already.");
                descriptor.ContentGeneratedForSave = true;
                return new ContentStream(this.mediaResourceRequestStream, false);
            }
            else if (this.CreateChangeData(index, requestMessage))
            {
                return requestMessage.CachedRequestStream;
            }

            return null;
        }

        /// <summary>
        /// Create request message from the next change.
        /// </summary>
        /// <returns>An instance of ODataRequestMessage for the next change.</returns>
        private ODataRequestMessageWrapper CreateNextRequest()
        {
            bool moveForward = this.streamRequestKind == StreamRequestKind.None;
            if (unchecked((uint)this.entryIndex < (uint)this.ChangedEntries.Count))
            {
                Descriptor previousDescriptor = this.ChangedEntries[this.entryIndex];
                if (previousDescriptor.DescriptorKind == DescriptorKind.Entity)
                {
                    EntityDescriptor entityDescriptor = (EntityDescriptor)previousDescriptor;

                    // In any case also close the save stream if there's any and forget about it
                    // for POST this is just a good practice to do so as soon as possible
                    // for PUT it's actually required for us to recognize that we already processed the MR part of the change
                    entityDescriptor.CloseSaveStream();

                    // If the previous request was an MR request the next one might be a PUT for the MLE
                    //   but if the entity was not changed (just the MR changed) no PUT for MLE should be sent
                    if (this.streamRequestKind == StreamRequestKind.PutMediaResource && EntityStates.Unchanged == entityDescriptor.State)
                    {
                        // Only the MR changed. In this case we also need to mark the descriptor as processed to notify
                        //   that the content for save has been generated as there's not going to be another request for it.
                        entityDescriptor.ContentGeneratedForSave = true;
                        moveForward = true;
                    }
                }
                else if (previousDescriptor.DescriptorKind == DescriptorKind.NamedStream)
                {
                    ((StreamDescriptor)previousDescriptor).CloseSaveStream();
                }
            }

            if (moveForward)
            {
                this.entryIndex++;
            }

            ODataRequestMessageWrapper requestMessage = null;
            if (unchecked((uint)this.entryIndex < (uint)this.ChangedEntries.Count))
            {
                Descriptor descriptor = this.ChangedEntries[this.entryIndex];
                Descriptor descriptorForSendingRequest2 = descriptor;
                if (descriptor.DescriptorKind == DescriptorKind.Entity)
                {
                    EntityDescriptor entityDescriptor = (EntityDescriptor)descriptor;

                    if (((EntityStates.Unchanged == descriptor.State) || (EntityStates.Modified == descriptor.State)) &&
                        (null != (requestMessage = this.CheckAndProcessMediaEntryPut(entityDescriptor))))
                    {
                        this.streamRequestKind = StreamRequestKind.PutMediaResource;
                        descriptorForSendingRequest2 = entityDescriptor.DefaultStreamDescriptor;  // We want to give SendingRequest2 the StreamDecriptor
                    }
                    else if ((EntityStates.Added == descriptor.State) && (null != (requestMessage = this.CheckAndProcessMediaEntryPost(entityDescriptor))))
                    {
                        this.streamRequestKind = StreamRequestKind.PostMediaResource;

                        Debug.Assert(entityDescriptor.SaveStream == null || entityDescriptor.StreamState == EntityStates.Added, "Either this is a V1 MR or the stream must be in added state");

                        // Set the stream state to added
                        // For V1 scenarios, when SetSaveStream was not called, this might not be in Added state
                        entityDescriptor.StreamState = EntityStates.Added;
                    }
                    else
                    {
                        this.streamRequestKind = StreamRequestKind.None;
                        Debug.Assert(descriptor.State != EntityStates.Unchanged, "descriptor.State != EntityStates.Unchanged");
                        requestMessage = this.CreateRequest(entityDescriptor);
                    }
                }
                else if (descriptor.DescriptorKind == DescriptorKind.NamedStream)
                {
                    requestMessage = this.CreateNamedStreamRequest((StreamDescriptor)descriptor);
                }
                else
                {
                    requestMessage = this.CreateRequest((LinkDescriptor)descriptor);
                }

                if (requestMessage != null)
                {
                    // we need to fire request after the headers have been written, but before we write the payload
                    // also in the prototype we are firing SendingRequest2 before SendingRequest.
                    requestMessage.FireSendingEventHandlers(descriptorForSendingRequest2);
                }
            }

            return requestMessage;
        }

        /// <summary>
        /// Check to see if the resource to be inserted is a media descriptor, and if so
        /// setup a POST request for the media content first and turn the rest of
        /// the operation into a PUT to update the rest of the properties.
        /// </summary>
        /// <param name="entityDescriptor">The resource to check/process</param>
        /// <returns>An instance of ODataRequestMessage to do POST to the media resource</returns>
        private ODataRequestMessageWrapper CheckAndProcessMediaEntryPost(EntityDescriptor entityDescriptor)
        {
            // TODO: Revisit the design of how media link entries are handled during update
            ClientEdmModel model = this.RequestInfo.Model;
            ClientTypeAnnotation type = model.GetClientTypeAnnotation(model.GetOrCreateEdmType(entityDescriptor.Entity.GetType()));

            if (!type.IsMediaLinkEntry && !entityDescriptor.IsMediaLinkEntry)
            {
                // this is not a media link descriptor, process normally
                return null;
            }

            if (type.MediaDataMember == null && entityDescriptor.SaveStream == null)
            {
                // The entity is marked as MLE but we don't have the content property
                //   and the user didn't set the save stream.
                throw Error.InvalidOperation(Strings.Context_MLEWithoutSaveStream(type.ElementTypeName));
            }

            Debug.Assert(
                (type.MediaDataMember != null && entityDescriptor.SaveStream == null) ||
                (type.MediaDataMember == null && entityDescriptor.SaveStream != null),
                "Only one way of specifying the MR content is allowed.");

            ODataRequestMessageWrapper mediaRequest = null;
            if (type.MediaDataMember != null)
            {
                string contentType = null;
                int contentLength = 0;

                if (type.MediaDataMember.MimeTypeProperty == null)
                {
                    contentType = XmlConstants.MimeApplicationOctetStream;
                }
                else
                {
                    object mimeTypeValue = type.MediaDataMember.MimeTypeProperty.GetValue(entityDescriptor.Entity);
                    String mimeType = mimeTypeValue != null ? mimeTypeValue.ToString() : null;

                    if (String.IsNullOrEmpty(mimeType))
                    {
                        throw Error.InvalidOperation(
                            Strings.Context_NoContentTypeForMediaLink(
                                type.ElementTypeName,
                                type.MediaDataMember.MimeTypeProperty.PropertyName));
                    }

                    contentType = mimeType;
                }

                object value = type.MediaDataMember.GetValue(entityDescriptor.Entity);
                if (value == null)
                {
                    this.mediaResourceRequestStream = null;
                }
                else
                {
                    byte[] buffer = value as byte[];
                    if (buffer == null)
                    {
                        string mime;
                        Encoding encoding;
                        ContentTypeUtil.ReadContentType(contentType, out mime, out encoding);

                        if (encoding == null)
                        {
                            encoding = Encoding.UTF8;
                            contentType += XmlConstants.MimeTypeUtf8Encoding;
                        }

                        buffer = encoding.GetBytes(ClientConvert.ToString(value));
                    }

                    contentLength = buffer.Length;

#if PORTABLELIB
                    // Win8 doesn't allow accessing the buffer, so the constructor we normally use doesn't exist
                    this.mediaResourceRequestStream = new MemoryStream(buffer, 0, buffer.Length, false);
#else
                    // Need to specify that the buffer is publicly visible as we need to access it later on
                    this.mediaResourceRequestStream = new MemoryStream(buffer, 0, buffer.Length, false, true);
#endif
                }

                HeaderCollection headers = new HeaderCollection();
                headers.SetHeader(XmlConstants.HttpContentLength, contentLength.ToString(CultureInfo.InvariantCulture));
                headers.SetHeader(XmlConstants.HttpContentType, contentType);

                mediaRequest = this.CreateMediaResourceRequest(
                    entityDescriptor.GetResourceUri(this.RequestInfo.BaseUriResolver, false /*queryLink*/),
                    XmlConstants.HttpMethodPost,
                    Util.ODataVersion4,
                    type.MediaDataMember == null, // sendChunked
                    true, // applyResponsePreference
                    headers,
                    entityDescriptor);
            }
            else
            {
                HeaderCollection headers = new HeaderCollection();
                this.SetupMediaResourceRequest(headers, entityDescriptor.SaveStream, null /*etag*/);

                mediaRequest = this.CreateMediaResourceRequest(
                    entityDescriptor.GetResourceUri(this.RequestInfo.BaseUriResolver, false /*queryLink*/),
                    XmlConstants.HttpMethodPost,
                    Util.ODataVersion4,
                    type.MediaDataMember == null, // sendChunked
                    true, // applyResponsePreference
                    headers,
                    entityDescriptor);
            }

            // Convert the insert into an update for the media link descriptor we just created
            // (note that the identity still needs to be fixed up on the resbox once
            // the response comes with the 'location' header; that happens during processing
            // of the response in SavedResource())
            entityDescriptor.State = EntityStates.Modified;

            return mediaRequest;
        }

        /// <summary>
        /// Checks if the entity descriptor represents an MLE with modified MR and if so creates a PUT request
        ///   to update the MR.
        /// </summary>
        /// <param name="entityDescriptor">The entity descriptor for the entity to be checked.</param>
        /// <returns>An instance of ODataRequestMessage for the newly created MR PUT request or null if the entity is not MLE or its MR hasn't changed.</returns>
        private ODataRequestMessageWrapper CheckAndProcessMediaEntryPut(EntityDescriptor entityDescriptor)
        {
            // If there's no save stream associated with the entity it's not MLE or its MR hasn't changed
            //  (which for purposes of PUT is the same anyway)
            if (entityDescriptor.SaveStream == null)
            {
                return null;
            }

            Uri requestUri = entityDescriptor.GetLatestEditStreamUri();
            if (requestUri == null)
            {
                throw Error.InvalidOperation(
                    Strings.Context_SetSaveStreamWithoutEditMediaLink);
            }

            HeaderCollection headers = new HeaderCollection();
            this.SetupMediaResourceRequest(headers, entityDescriptor.SaveStream, entityDescriptor.GetLatestStreamETag());

            return this.CreateMediaResourceRequest(
                requestUri,
                XmlConstants.HttpMethodPut,
                Util.ODataVersion4,
                true, // sendChunked
                false, // applyResponsePreference
                headers,
                entityDescriptor.DefaultStreamDescriptor);
        }

        /// <summary>
        /// Creates HTTP request for the media resource (MR)
        /// </summary>
        /// <param name="requestUri">The URI to request</param>
        /// <param name="method">The HTTP method to use (POST or PUT)</param>
        /// <param name="version">version to be sent in the DSV request header.</param>
        /// <param name="sendChunked">Send the request using chunked encoding to avoid buffering.</param>
        /// <param name="applyResponsePreference">If the response preference setting should be applied to the request
        /// (basically means if the response is expected to contain an entity or not).</param>
        /// <param name="headers">Collection of request headers</param>
        /// <param name="descriptor">Descriptor for this media resource request.</param>
        /// <returns>An instance of ODataRequestMessage.</returns>
        private ODataRequestMessageWrapper CreateMediaResourceRequest(Uri requestUri, string method, Version version, bool sendChunked, bool applyResponsePreference, HeaderCollection headers, Descriptor descriptor)
        {
            headers.SetHeaderIfUnset(XmlConstants.HttpContentType, XmlConstants.MimeAny);

            if (applyResponsePreference)
            {
                ApplyPreferences(headers, method, this.RequestInfo.AddAndUpdateResponsePreference, ref version);
            }

            // Set the request DSV and request MDSV headers
            headers.SetRequestVersion(version, this.RequestInfo.MaxProtocolVersionAsVersion);
            this.RequestInfo.Format.SetRequestAcceptHeader(headers);

            ODataRequestMessageWrapper requestMessage = this.CreateRequestMessage(method, requestUri, headers, this.RequestInfo.HttpStack, descriptor, null /*contentId*/);

            // TODO: since under the hood this is a header, we should put it in our dictionary of headers that the user gets in BuildingRequest
            // and later on handle the setting of the strongly named property on the underlying request
            requestMessage.SendChunked = sendChunked;
            return requestMessage;
        }

        /// <summary>
        /// Sets the content and the headers of the media resource request
        /// </summary>
        /// <param name="headers">The header collection to setup.</param>
        /// <param name="saveStream">DataServiceSaveStream instance containing all information about the stream.</param>
        /// <param name="etag">ETag header value to be set. If passed null, etag header is not set.</param>
        /// <remarks>This only works with the V2 MR support (SetSaveStream), this will not setup
        /// the request for V1 property based MRs.</remarks>
        private void SetupMediaResourceRequest(HeaderCollection headers, DataServiceSaveStream saveStream, string etag)
        {
            // Get the write stream for this MR
            this.mediaResourceRequestStream = saveStream.Stream;

            // Copy over headers for the request, except the accept header
            headers.SetHeaders(saveStream.Args.Headers.Where(h => !string.Equals(h.Key, XmlConstants.HttpRequestAccept, StringComparison.OrdinalIgnoreCase)));

            if (etag != null)
            {
                headers.SetHeader(XmlConstants.HttpRequestIfMatch, etag);
            }

            // Do NOT set the ContentLength since we don't know if the stream even supports reporting its length
        }

        #region generate batch response from non-batch

        /// <summary>operation with exception</summary>
        /// <param name="e">exception object</param>
        /// <param name="response">response object</param>
        private void HandleOperationException(InvalidOperationException e, IODataResponseMessage response)
        {
            Debug.Assert(this.entryIndex >= 0 && this.entryIndex < this.ChangedEntries.Count(), string.Format(System.Globalization.CultureInfo.InvariantCulture, "this.entryIndex = '{0}', this.ChangedEntries.Count = '{1}'", this.entryIndex, this.ChangedEntries.Count()));

            Descriptor current = this.ChangedEntries[this.entryIndex];
            HeaderCollection headers = null;
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

            Version responseVersion = null;
            if (null != response)
            {
                headers = new HeaderCollection(response);
                statusCode = (HttpStatusCode)response.StatusCode;

                this.HandleOperationResponseHeaders(statusCode, headers);
                e = BaseSaveResult.HandleResponse(
                    this.RequestInfo,
                    statusCode,
                    response.GetHeader(XmlConstants.HttpODataVersion),
                    response.GetStream,
                    false/*throwOnFailure*/,
                    out responseVersion);
            }
            else
            {
                headers = new HeaderCollection();
                headers.SetHeader(XmlConstants.HttpContentType, XmlConstants.MimeTextPlain);

                // In V2 we used to merge individual responses from a call to SaveChanges() into a single batch response payload and then process that.
                // When we encounter an exception at this point in V2, we used to write the exception to the batch response payload and later on when we
                // process through the batch response, we create a DataServiceClientException for each failed operation.
                // For backcompat reason, we will always convert the exception type to DataServiceClientException here.
                Debug.Assert(e != null, "e != null");
                if (e.GetType() != typeof(DataServiceClientException))
                {
                    e = new DataServiceClientException(e.Message, e);
                }
            }

            // For error scenarios, we never invoke the ReadingEntity event.
            this.cachedResponses.Add(new CachedResponse(current, headers, statusCode, responseVersion, null, e));
            this.perRequest = null;
            this.CheckContinueOnError();
        }

        /// <summary>
        /// Decide whether we should continue when there is an error thrown
        /// </summary>
        private void CheckContinueOnError()
        {
            if (!Util.IsFlagSet(this.Options, SaveChangesOptions.ContinueOnError))
            {
                this.SetCompleted();
            }
            else
            {
                // if it was a media link descriptor don't even try to do a PUT if the POST didn't succeed
                this.streamRequestKind = StreamRequestKind.None;

                // Need to set this to true since we check this even on error cases, but we're here
                //   because exception was thrown during preparation of the request, so we might not have a chance
                //   to generate the content for save yet.
                this.ChangedEntries[this.entryIndex].ContentGeneratedForSave = true;
            }
        }

#if !PORTABLELIB
        /// <summary>
        /// copy the response data
        /// </summary>
        /// <param name="response">response object</param>
        private void HandleOperationResponseData(IODataResponseMessage response)
        {
            Debug.Assert(response != null, "response != null");

            using (Stream stream = response.GetStream())
            {
                if (null != stream)
                {
                    // we need to check for whether the incoming stream was data or not. Hence we need to copy it to a temporary memory stream
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        if (WebUtil.CopyStream(stream, memoryStream, ref this.buildBatchBuffer) != 0)
                        {
                            // set the memory stream position to zero again.
                            memoryStream.Position = 0;
                            this.HandleOperationResponseData(response, memoryStream);
                        }
                        else
                        {
                            this.HandleOperationResponseData(response, null);
                        }
                    }
                }
            }
        }
#endif

        /// <summary>
        /// Handle the response payload.
        /// </summary>
        /// <param name="responseMsg">httpwebresponse instance.</param>
        /// <param name="responseStream">stream containing the response payload.</param>
        private void HandleOperationResponseData(IODataResponseMessage responseMsg, Stream responseStream)
        {
            Debug.Assert(this.entryIndex >= 0 && this.entryIndex < this.ChangedEntries.Count(), string.Format(System.Globalization.CultureInfo.InvariantCulture, "this.entryIndex = '{0}', this.ChangedEntries.Count() = '{1}'", this.entryIndex, this.ChangedEntries.Count()));

            // Parse the response
            Descriptor current = this.ChangedEntries[this.entryIndex];
            MaterializerEntry entry = default(MaterializerEntry);
            Version responseVersion;
            Exception exception = BaseSaveResult.HandleResponse(this.RequestInfo, (HttpStatusCode)responseMsg.StatusCode, responseMsg.GetHeader(XmlConstants.HttpODataVersion), () => { return responseStream; }, false/*throwOnFailure*/, out responseVersion);

            var headers = new HeaderCollection(responseMsg);
            if (responseStream != null && current.DescriptorKind == DescriptorKind.Entity && exception == null)
            {
                // Only process the response if the current resource is an entity and it's an insert or update scenario
                EntityDescriptor entityDescriptor = (EntityDescriptor)current;

                // We were ignoring the payload for non-insert and non-update scenarios. We need to keep doing that.
                if (entityDescriptor.State == EntityStates.Added || entityDescriptor.StreamState == EntityStates.Added ||
                    entityDescriptor.State == EntityStates.Modified || entityDescriptor.StreamState == EntityStates.Modified)
                {
                    try
                    {
                        ResponseInfo responseInfo = this.CreateResponseInfo(entityDescriptor);
                        var responseMessageWrapper = new HttpWebResponseMessage(
                            headers,
                            responseMsg.StatusCode,
                            () => responseStream);

                        entry = ODataReaderEntityMaterializer.ParseSingleEntityPayload(responseMessageWrapper, responseInfo, entityDescriptor.Entity.GetType());
                        entityDescriptor.TransientEntityDescriptor = entry.EntityDescriptor;
                    }
                    catch (Exception ex)
                    {
                        exception = ex;

                        if (!CommonUtil.IsCatchableExceptionType(ex))
                        {
                            throw;
                        }
                    }
                }
            }

            this.cachedResponses.Add(new CachedResponse(
                current,
                headers,
                (HttpStatusCode)responseMsg.StatusCode,
                responseVersion,
                entry,
                exception));

            if (exception != null)
            {
                current.SaveError = exception;

                // DEVNOTE(pqian):
                // There are two possible scenario here:
                // 1. We are in the sync code path, and there's an in stream error on the server side, or there are bad xml thrown
                // 2. We are in the async code path, there's a error thrown on the server side (any error)
                // Ideally, we need to check whether we want to continue to the next changeset. (Call this.CheckContinueOnError)
                // However, in V1/V2, we did not do this. Thus we will always continue on error on these scenarios
            }
        }

        #endregion

        /// <summary>
        /// Creates a request for the given named stream.
        /// </summary>
        /// <param name="namedStreamInfo">NamedStreamInfo instance containing information about the stream.</param>
        /// <returns>An instance of ODataRequestMessage for the given named stream.</returns>
        private ODataRequestMessageWrapper CreateNamedStreamRequest(StreamDescriptor namedStreamInfo)
        {
            Debug.Assert(namedStreamInfo.SaveStream != null, "The named stream must have an associated stream");
            Debug.Assert(!string.IsNullOrEmpty(namedStreamInfo.SaveStream.Args.ContentType), "ContentType must not be null or empty");

            Uri requestUri = namedStreamInfo.GetLatestEditLink();
            if (requestUri == null)
            {
                throw Error.InvalidOperation(Strings.Context_SetSaveStreamWithoutNamedStreamEditLink(namedStreamInfo.Name));
            }

            HeaderCollection headers = new HeaderCollection();
            this.SetupMediaResourceRequest(headers, namedStreamInfo.SaveStream, namedStreamInfo.GetLatestETag());

            ODataRequestMessageWrapper mediaRequestMessage = this.CreateMediaResourceRequest(
                requestUri,
                XmlConstants.HttpMethodPut,
                Util.ODataVersion4,
                true, // sendChunked
                false, // applyResponsePreference
                headers,
                namedStreamInfo);

#if DEBUG
            Debug.Assert(mediaRequestMessage.GetHeader("Content-Type") == namedStreamInfo.SaveStream.Args.ContentType, "ContentType not set correctly for the named stream");
#endif

            return mediaRequestMessage;
        }

        /// <summary>
        /// cached response
        /// </summary>
        private struct CachedResponse
        {
            /// <summary>response headers</summary>
            public readonly HeaderCollection Headers;

            /// <summary>response status code</summary>
            public readonly HttpStatusCode StatusCode;

            /// <summary>Parsed response OData-Version header.</summary>
            public readonly Version Version;

            /// <summary>entry containing the parsed response.</summary>
            public readonly MaterializerEntry MaterializerEntry;

            /// <summary>Exception if encountered.</summary>
            public readonly Exception Exception;

            /// <summary>descriptor for which the response is getting parsed.</summary>
            public readonly Descriptor Descriptor;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="descriptor">descriptor whose response is getting processed.</param>
            /// <param name="headers">headers</param>
            /// <param name="statusCode">status code</param>
            /// <param name="responseVersion">Parsed response OData-Version header.</param>
            /// <param name="entry">atom entry, if there is a non-error response payload.</param>
            /// <param name="exception">exception, if the request threw an exception.</param>
            internal CachedResponse(Descriptor descriptor, HeaderCollection headers, HttpStatusCode statusCode, Version responseVersion, MaterializerEntry entry, Exception exception)
            {
                Debug.Assert(descriptor != null, "descriptor != null");
                Debug.Assert(headers != null, "headers != null");
                Debug.Assert(entry == null || (exception == null && descriptor.DescriptorKind == DescriptorKind.Entity), "if entry is specified, exception cannot be specified and entry must be a resource, since we expect responses only for entities");

                this.Descriptor = descriptor;
                this.MaterializerEntry = entry;
                this.Exception = exception;
                this.Headers = headers;
                this.StatusCode = statusCode;
                this.Version = responseVersion;
            }
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="BaseSaveResult.cs" company="Microsoft">
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
    using System.Threading;
#if DNXCORE50
    using System.Threading.Tasks;
#endif
    using Microsoft.OData.Core;
    using Microsoft.OData.Client.Metadata;

    #endregion Namespaces

    /// <summary>
    /// Base class for building the request and handling the response
    /// </summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Pending")]
    internal abstract class BaseSaveResult : BaseAsyncResult
    {
        #region Private Fields

        /// <summary>where to pull the changes from</summary>
        protected readonly RequestInfo RequestInfo;

        /// <summary>serializer to serialize the request data.</summary>
        protected readonly Serializer SerializerInstance;

        /// <summary>sorted list of entries by change order</summary>
        protected readonly List<Descriptor> ChangedEntries;

        /// <summary>option in use for SaveChanges</summary>
        protected readonly SaveChangesOptions Options;

        /// <summary>batch web response</summary>
        protected IODataResponseMessage batchResponseMessage;

        /// <summary>The ResourceBox or RelatedEnd currently in flight</summary>
        protected int entryIndex = -1;

        /// <summary>what kind of request are we processing - POST MR or PUT MR</summary>
        protected StreamRequestKind streamRequestKind;

        /// <summary>
        /// If the <see cref="streamRequestKind"/> is set to anything but None,
        /// this field holds a stream needs to be send in the request.
        /// This can be null in the case where the content of MR is empty. (In which case
        /// we will not try to open the request stream and thus avoid additional async call).
        /// </summary>
        protected Stream mediaResourceRequestStream;

        /// <summary>temporary buffer when cache results from CUD op in non-batching save changes</summary>
        protected byte[] buildBatchBuffer;

        #endregion Private Fields

        /// <summary>
        /// constructor for operations
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="method">method</param>
        /// <param name="queries">queries</param>
        /// <param name="options">options</param>
        /// <param name="callback">user callback</param>
        /// <param name="state">user state object</param>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "IsBatch returns a constant value and hence safe to be invoked from the constructor")]
        internal BaseSaveResult(DataServiceContext context, string method, DataServiceRequest[] queries, SaveChangesOptions options, AsyncCallback callback, object state)
            : base(context, method, callback, state)
        {
            this.RequestInfo = new RequestInfo(context);
            this.Options = options;
            this.SerializerInstance = new Serializer(this.RequestInfo, options);

            if (null == queries)
            {
                #region changed entries
                this.ChangedEntries = context.EntityTracker.Entities.Cast<Descriptor>()
                                      .Union(context.EntityTracker.Links.Cast<Descriptor>())
                                      .Union(context.EntityTracker.Entities.SelectMany(e => e.StreamDescriptors).Cast<Descriptor>())
                                      .Where(o => o.IsModified && o.ChangeOrder != UInt32.MaxValue)
                                      .OrderBy(o => o.ChangeOrder)
                                      .ToList();

                foreach (Descriptor e in this.ChangedEntries)
                {
                    e.ContentGeneratedForSave = false;
                    e.SaveResultWasProcessed = 0;
                    e.SaveError = null;

                    if (e.DescriptorKind == DescriptorKind.Link)
                    {
                        object target = ((LinkDescriptor)e).Target;
                        if (null != target)
                        {
                            Descriptor f = context.EntityTracker.GetEntityDescriptor(target);
                            if (EntityStates.Unchanged == f.State)
                            {
                                f.ContentGeneratedForSave = false;
                                f.SaveResultWasProcessed = 0;
                                f.SaveError = null;
                            }
                        }
                    }
                }
                #endregion
            }
            else
            {
                this.ChangedEntries = new List<Descriptor>();
            }
        }

        /// <summary>
        /// enum which says what kind of request we are currently processing
        /// </summary>
        protected enum StreamRequestKind
        {
            /// <summary>This request doesn't involve Media Resource or named stream.</summary>
            None = 0,

            /// <summary>This request is a POST to a MLE and the body contains the content of the MR.</summary>
            PostMediaResource,

            /// <summary>This request is a PUT to MR and the body contains the content of the MR.</summary>
            PutMediaResource,
        }

        /// <summary>returns true if its a batch, otherwise returns false.</summary>
        internal abstract bool IsBatchRequest { get; }

        /// <summary>
        /// In async case, this is a memory stream used to cache responses, as we are reading async from the underlying http web response stream.
        /// In sync case, this is the actual response stream, as returned by the http request.
        /// </summary>
        protected abstract Stream ResponseStream
        {
            get;
        }

        /// <summary>
        /// returns true if the response payload needs to be processed.
        /// </summary>
        protected abstract bool ProcessResponsePayload
        {
            get;
        }

        /// <summary>
        /// factory method for SaveResult
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="method">method</param>
        /// <param name="queries">queries</param>
        /// <param name="options">options</param>
        /// <param name="callback">user callback</param>
        /// <param name="state">user state object</param>
        /// <returns>a new instance of SaveResult or BatchSaveResult, depending on the options value.</returns>
        internal static BaseSaveResult CreateSaveResult(DataServiceContext context, string method, DataServiceRequest[] queries, SaveChangesOptions options, AsyncCallback callback, object state)
        {
            if (!Util.IsBatch(options))
            {
                Debug.Assert(queries == null, "In non-batch case, queries must be null");
                return new SaveResult(context, method, options, callback, state);
            }
            else
            {
                return new BatchSaveResult(context, method, queries, options, callback, state);
            }
        }

        /// <summary>
        /// Handle response by looking at status and possibly throwing an exception.
        /// </summary>
        /// <param name="requestInfo">The request info.</param>
        /// <param name="statusCode">response status code</param>
        /// <param name="responseVersion">Version string on the response header; possibly null.</param>
        /// <param name="getResponseStream">delegate to get response stream</param>
        /// <param name="throwOnFailure">throw or return on failure</param>
        /// <param name="parsedResponseVersion">Parsed response version (null if no version was specified).</param>
        /// <returns>exception on failure</returns>
        internal static InvalidOperationException HandleResponse(
            RequestInfo requestInfo,
            HttpStatusCode statusCode,
            string responseVersion,
            Func<Stream> getResponseStream,
            bool throwOnFailure,
            out Version parsedResponseVersion)
        {
            InvalidOperationException failure = null;
            if (!CanHandleResponseVersion(responseVersion, out parsedResponseVersion))
            {
                string description = Strings.Context_VersionNotSupported(responseVersion, SerializeSupportedVersions());
                failure = Error.InvalidOperation(description);
            }

            if (failure == null)
            {
                failure = requestInfo.ValidateResponseVersion(parsedResponseVersion);
            }

            if (failure == null && !WebUtil.SuccessStatusCode(statusCode))
            {
                failure = GetResponseText(getResponseStream, statusCode);
            }

            if (failure != null && throwOnFailure)
            {
                throw failure;
            }

            return failure;
        }

        /// <summary>
        /// get the response text into a string
        /// </summary>
        /// <param name="getResponseStream">method to get response stream</param>
        /// <param name="statusCode">status code</param>
        /// <returns>text</returns>
        [SuppressMessage("Microsoft.Design", "CA1031", Justification = "Cache exception so user can examine it later")]
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "underlying stream is disposed so wrapping StreamReader doesn't need to be disposed")]
        internal static DataServiceClientException GetResponseText(Func<Stream> getResponseStream, HttpStatusCode statusCode)
        {
            string message = null;
            using (Stream stream = getResponseStream())
            {
                if ((null != stream) && stream.CanRead)
                {
                    // this StreamReader can go out of scope without dispose because the underly stream is disposed of
                    message = new StreamReader(stream).ReadToEnd();
                }
            }

            if (string.IsNullOrEmpty(message))
            {
                message = statusCode.ToString();
            }

            return new DataServiceClientException(message, (int)statusCode);
        }

        /// <summary>process the batch</summary>
        /// <returns>data service response</returns>
        internal DataServiceResponse EndRequest()
        {
            // Close all Save streams before we return (and do this before we throw for any errors below)
            foreach (Descriptor descriptor in this.ChangedEntries)
            {
                descriptor.ClearChanges();
            }

            return this.HandleResponse();
        }

        /// <summary>Get the value of HttpMethod enum from link resource state</summary>
        /// <param name="link">Instance of LinkDescriptor containing the link state and type of link.</param>
        /// <returns>HttpMethod enum value for the link descriptor state.</returns>
        protected static string GetLinkHttpMethod(LinkDescriptor link)
        {
            if (!link.IsSourcePropertyCollection)
            {
                Debug.Assert(EntityStates.Modified == link.State, "not Modified state");
                if (null == link.Target)
                {   // REMOVE/DELETE a reference
                    return XmlConstants.HttpMethodDelete;
                }
                else
                {   // UPDATE/PUT a reference
                    return XmlConstants.HttpMethodPut;
                }
            }
            else if (EntityStates.Deleted == link.State)
            {   // you call DELETE on $ref
                return XmlConstants.HttpMethodDelete;
            }
            else
            {   // you INSERT/POST into a collection
                Debug.Assert(EntityStates.Added == link.State, "not Added state");
                return XmlConstants.HttpMethodPost;
            }
        }

        /// <summary>
        /// Apply the response preferences for the client.
        /// </summary>
        /// <param name="headers">Headers to which preferences will be added.</param>
        /// <param name="method">HTTP method.</param>
        /// <param name="responsePreference">Response preference.</param>
        /// <param name="requestVersion">Request version so far for the request. The method may modify it.</param>
        protected static void ApplyPreferences(HeaderCollection headers, string method, DataServiceResponsePreference responsePreference, ref Version requestVersion)
        {
            // The AddAndUpdateResponsePreference only applies to POST/PUT/PATCH requests
            if (string.CompareOrdinal(XmlConstants.HttpMethodPost, method) != 0 &&
                string.CompareOrdinal(XmlConstants.HttpMethodPut, method) != 0 &&
                string.CompareOrdinal(XmlConstants.HttpMethodPatch, method) != 0)
            {
                return;
            }

            string preferHeaderValue = WebUtil.GetPreferHeaderAndRequestVersion(responsePreference, ref requestVersion);
            if (preferHeaderValue != null)
            {
                headers.SetHeader(XmlConstants.HttpPrefer, preferHeaderValue);
            }
        }

        /// <summary>
        /// Handle response.
        /// </summary>
        /// <returns>an instance of the DataServiceResponse.</returns>
        protected abstract DataServiceResponse HandleResponse();

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
        protected abstract ODataRequestMessageWrapper CreateRequestMessage(string method, Uri requestUri, HeaderCollection headers, HttpStack httpStack, Descriptor descriptor, string contentId);

        /// <summary>Get the value of the HttpMethod enum from entity resource state</summary>
        /// <param name="state">resource state</param>
        /// <param name="requestVersion">The version of the request determined so far. The method may modify this if needed.</param>
        /// <returns>HttpMethod value from the entity resource state.</returns>
        protected string GetHttpMethod(EntityStates state, ref Version requestVersion)
        {
            switch (state)
            {
                case EntityStates.Deleted:
                    return XmlConstants.HttpMethodDelete;
                case EntityStates.Modified:
                    if (Util.IsFlagSet(this.Options, SaveChangesOptions.ReplaceOnUpdate))
                    {
                        return XmlConstants.HttpMethodPut;
                    }
                    else
                    {
                        // Default update is PATCH not PUT
                        return XmlConstants.HttpMethodPatch;
                    }

                case EntityStates.Added:
                    return XmlConstants.HttpMethodPost;
                default:
                    throw Error.InternalError(InternalError.UnvalidatedEntityState);
            }
        }

        /// <summary>
        /// Create request message for the descriptor at the given index.
        /// </summary>
        /// <param name="index">Index into changed entries</param>
        /// <param name="requestMessage">IODataRequestMessage that needs to be used for writing the payload.</param>
        /// <returns>true, if any request payload was generated, else false.</returns>
        protected bool CreateChangeData(int index, ODataRequestMessageWrapper requestMessage)
        {
            Descriptor descriptor = this.ChangedEntries[index];
            Debug.Assert(!descriptor.ContentGeneratedForSave, "already saved entity/link");

            // Since batch payloads do not support MR and Named Stream, the code for handling that has been moved to
            // SaveResult.CreateNonBatchChangeData. In this method, we only handle entity and link payload.
            Debug.Assert(descriptor.DescriptorKind != DescriptorKind.NamedStream, "NamedStream payload is not supported in batch");

            if (descriptor.DescriptorKind == DescriptorKind.Entity)
            {
                EntityDescriptor entityDescriptor = (EntityDescriptor)descriptor;
                Debug.Assert(this.streamRequestKind == StreamRequestKind.None, "Batch does not support stream payloads. Hence this code should not get called at all");

                // either normal entity or second call for media link entity, generate content payload
                // else first call of media link descriptor where we only send the default value
                descriptor.ContentGeneratedForSave = true;
                return this.CreateRequestData(entityDescriptor, requestMessage);
            }
            else
            {
                descriptor.ContentGeneratedForSave = true;
                LinkDescriptor link = (LinkDescriptor)descriptor;
                if ((EntityStates.Added == link.State) ||
                    ((EntityStates.Modified == link.State) && (null != link.Target)))
                {
                    this.CreateRequestData(link, requestMessage);
                    return true;
                }
            }

            return false;
        }

        /// <summary>Set the AsyncWait and invoke the user callback.</summary>
        /// <param name="pereq">the request object</param>
        protected override void HandleCompleted(PerRequest pereq)
        {
            if (null != pereq)
            {
                this.SetCompletedSynchronously(pereq.RequestCompletedSynchronously);

                if (pereq.RequestCompleted)
                {
                    Interlocked.CompareExchange(ref this.perRequest, null, pereq);
                    if (this.IsBatchRequest)
                    {   // all competing thread must complete this before user callback is invoked
                        Interlocked.CompareExchange(ref this.batchResponseMessage, pereq.ResponseMessage, null);
                        pereq.ResponseMessage = null;
                    }

                    pereq.Dispose();
                }
            }

            this.HandleCompleted();
        }

        /// <summary>handle request.BeginGetResponse with request.EndGetResponse and then copy response stream</summary>
        /// <param name="asyncResult">async result</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "required for this feature")]
        [SuppressMessage("DataWeb.Usage", "AC0014", Justification = "Throws every time")]
        protected override void AsyncEndGetResponse(IAsyncResult asyncResult)
        {
            Debug.Assert(asyncResult != null && asyncResult.IsCompleted, "asyncResult.IsCompleted");
            AsyncStateBag asyncStateBag = asyncResult.AsyncState as AsyncStateBag;

            PerRequest pereq = asyncStateBag == null ? null : asyncStateBag.PerRequest;

            try
            {
                this.CompleteCheck(pereq, InternalError.InvalidEndGetResponseCompleted);
                pereq.SetRequestCompletedSynchronously(asyncResult.CompletedSynchronously); // BeginGetResponse

                EqualRefCheck(this.perRequest, pereq, InternalError.InvalidEndGetResponse);
                ODataRequestMessageWrapper requestMessage = Util.NullCheck(pereq.Request, InternalError.InvalidEndGetResponseRequest);

                // the httpWebResponse is kept for batching, discarded by non-batch
                IODataResponseMessage responseMessage = null;
                Util.DebugInjectFault("SaveAsyncResult::AsyncEndGetResponse::BeforeEndGetResponse");

                responseMessage = this.RequestInfo.EndGetResponse(requestMessage, asyncResult);
                pereq.ResponseMessage = Util.NullCheck(responseMessage, InternalError.InvalidEndGetResponseResponse);

                if (!this.IsBatchRequest)
                {
                    this.HandleOperationResponse(responseMessage);
                    this.HandleOperationResponseHeaders((HttpStatusCode)responseMessage.StatusCode, new HeaderCollection(responseMessage));
                }

                Util.DebugInjectFault("SaveAsyncResult::AsyncEndGetResponse_BeforeGetStream");
                Stream httpResponseStream = responseMessage.GetStream();
                pereq.ResponseStream = httpResponseStream;

                if ((null != httpResponseStream) && httpResponseStream.CanRead)
                {
                    if (null == this.buildBatchBuffer)
                    {
                        this.buildBatchBuffer = new byte[8000];
                    }

                    do
                    {
                        Util.DebugInjectFault("SaveAsyncResult::AsyncEndGetResponse_BeforeBeginRead");
#if DNXCORE50
                        asyncResult = BaseAsyncResult.InvokeTask(httpResponseStream.ReadAsync, this.buildBatchBuffer, 0, this.buildBatchBuffer.Length, this.AsyncEndRead, new AsyncReadState(pereq));
#else
                        asyncResult = InvokeAsync(httpResponseStream.BeginRead, this.buildBatchBuffer, 0, this.buildBatchBuffer.Length, this.AsyncEndRead, new AsyncReadState(pereq));
#endif
                        pereq.SetRequestCompletedSynchronously(asyncResult.CompletedSynchronously); // BeginRead
                    }
                    while (asyncResult.CompletedSynchronously && !pereq.RequestCompleted && !this.IsCompletedInternally && httpResponseStream.CanRead);
                }
                else
                {
                    pereq.SetComplete();

                    // BeginGetResponse could fail and callback still invoked
                    // if pereq.RequestCompletedSynchronously is true, this.FinishCurrentChange() will be
                    // invoked by the current thread in SaveResult.BeginCreateNextChange().
                    // if pereq.RequestCompletedSynchronously is false, we will call this.FinishCurrentChange() here and
                    // the parent thread will not call this.FinishCurrentChange() in SaveResult.BeginCreateNextChange().
                    if (!this.IsCompletedInternally && !pereq.RequestCompletedSynchronously)
                    {
                        this.FinishCurrentChange(pereq);
                    }
                }
            }
            catch (Exception e)
            {
                if (this.HandleFailure(pereq, e))
                {
                    throw;
                }
            }
            finally
            {
                this.HandleCompleted(pereq);
            }
        }

        /// <summary>IODataResponseMessage contain response for the operation.</summary>
        /// <param name="responseMessage">IODataResponseMessage instance.</param>
        protected abstract void HandleOperationResponse(IODataResponseMessage responseMessage);

        /// <summary>operation with HttpWebResponse</summary>
        /// <param name="statusCode">status code of the response.</param>
        /// <param name="headers">response headers.</param>
        protected void HandleOperationResponseHeaders(HttpStatusCode statusCode, HeaderCollection headers)
        {
            Descriptor descriptor = this.ChangedEntries[this.entryIndex];

            // in the first pass, the http response is packaged into a batch response (which is then processed in second pass).
            // in this first pass, (all added entities and first call of modified media link entities) update their edit location
            // added entities - so entities that have not sent content yet w/ reference links can inline those reference links in their payload
            // media entities - because they can change edit location which is then necessary for second call that includes property content
            if (descriptor.DescriptorKind == DescriptorKind.Entity)
            {
                EntityDescriptor entityDescriptor = (EntityDescriptor)descriptor;
                Debug.Assert(this.streamRequestKind != StreamRequestKind.PostMediaResource || descriptor.State == EntityStates.Modified, "For the POST MR, the entity state must be modified");

                // For POST and PATCH scenarios
                if (descriptor.State == EntityStates.Added ||
                    this.streamRequestKind == StreamRequestKind.PostMediaResource ||
                    !Util.IsFlagSet(this.Options, SaveChangesOptions.ReplaceOnUpdate))
                {
                    if (WebUtil.SuccessStatusCode(statusCode))
                    {
                        string location;
                        string odataEntityId;
                        Uri editLink = null;
                        headers.TryGetHeader(XmlConstants.HttpResponseLocation, out location);
                        headers.TryGetHeader(XmlConstants.HttpODataEntityId, out odataEntityId);

                        if (location != null)
                        {
                            // Verify the location header is an absolute uri
                            editLink = WebUtil.ValidateLocationHeader(location);
                        }
                        else if (descriptor.State == EntityStates.Added ||
                                 this.streamRequestKind == StreamRequestKind.PostMediaResource)
                        {
                            // For POST scenarios, location header must be specified.
                            throw Error.NotSupported(Strings.Deserialize_NoLocationHeader);
                        }

                        // Verify the id value if present. Otherwise we should use the location header
                        // as identity. This was done to avoid breaking change, since in V1/V2, we used
                        // to do this.
                        Uri odataId = null;
                        if (odataEntityId != null)
                        {
                            odataId = WebUtil.ValidateIdentityValue(odataEntityId);
                            if (location == null)
                            {
                                throw Error.NotSupported(Strings.Context_BothLocationAndIdMustBeSpecified);
                            }
                        }
                        else
                        {
                            // we already verified that the location must be an absolute uri
                            odataId = UriUtil.CreateUri(location, UriKind.Absolute);
                        }

                        if (null != editLink)
                        {
                            this.RequestInfo.EntityTracker.AttachLocation(entityDescriptor.Entity, odataId, editLink);
                        }
                    }
                }

                if (this.streamRequestKind != StreamRequestKind.None)
                {
                    if (!WebUtil.SuccessStatusCode(statusCode))
                    {
                        // If the request failed and it was the MR request we should not try to send the PUT MLE after it
                        // for one we don't have the location to send it to (if it was POST MR)
                        if (this.streamRequestKind == StreamRequestKind.PostMediaResource)
                        {
                            // If this was the POST MR it means we tried to add the entity. Now its state is Modified but we need
                            //   to revert back to Added so that user can retry by calling SaveChanges again.
                            Debug.Assert(descriptor.State == EntityStates.Modified, "Entity state should be set to Modified once we've sent the POST MR");
                            descriptor.State = EntityStates.Added;
                        }

                        // Just reset the streamRequestKind flag - that means that we will not try to PUT the MLE and instead skip over
                        //   to the next change (if we are to ignore errors that is)
                        this.streamRequestKind = StreamRequestKind.None;

                        // And we also need to mark it such that we generated the save content (which we did before the POST request in fact)
                        // to workaround the fact that we use the same descriptor object to track two requests.
                        descriptor.ContentGeneratedForSave = true;
                    }
                    else if (this.streamRequestKind == StreamRequestKind.PostMediaResource)
                    {
                        // We just finished a POST MR request and the PUT MLE coming immediately after it will
                        // need the new etag value from the server to succeed.
                        string etag;
                        if (headers.TryGetHeader(XmlConstants.HttpResponseETag, out etag))
                        {
                            entityDescriptor.ETag = etag;
                        }

                        // else is not interesting and we intentionally do nothing.
                    }
                }
            }
        }

#if DEBUG
        /// <summary>
        /// Handle operation response
        /// </summary>
        /// <param name="descriptor">descriptor whose response is getting processed.</param>
        /// <param name="contentHeaders">content headers as returned in the response.</param>
        /// <param name="statusCode">status code.</param>
        protected void HandleOperationResponse(Descriptor descriptor, HeaderCollection contentHeaders, HttpStatusCode statusCode)
#else
        /// <summary>
        /// Handle operation response
        /// </summary>
        /// <param name="descriptor">descriptor whose response is getting processed.</param>
        /// <param name="contentHeaders">content headers as returned in the response.</param>
        protected void HandleOperationResponse(Descriptor descriptor, HeaderCollection contentHeaders)
#endif
        {
            EntityStates streamState = EntityStates.Unchanged;
            if (descriptor.DescriptorKind == DescriptorKind.Entity)
            {
                EntityDescriptor entityDescriptor = (EntityDescriptor)descriptor;
                streamState = entityDescriptor.StreamState;
#if DEBUG
                if (entityDescriptor.StreamState == EntityStates.Added)
                {
                    // We do not depend anywhere for the status code to be Created (201). Hence changing the assert from checking for a specific status code
                    // to just checking for success status code.
                    Debug.Assert(
                        WebUtil.SuccessStatusCode(statusCode) && entityDescriptor.State == EntityStates.Modified && entityDescriptor.IsMediaLinkEntry,
                        "WebUtil.SuccessStatusCode(statusCode) && descriptor.State == EntityStates.Modified && descriptor.IsMediaLinkEntry -- Processing Post MR");
                }
                else if (entityDescriptor.StreamState == EntityStates.Modified)
                {
                    // We do not depend anywhere for the status code to be Created (201). Hence changing the assert from checking for a specific status code
                    // to just checking for success status code.
                    Debug.Assert(
                        WebUtil.SuccessStatusCode(statusCode) && entityDescriptor.IsMediaLinkEntry,
                        "WebUtil.SuccessStatusCode(statusCode) && descriptor.IsMediaLinkEntry -- Processing Put MR");
                }

                // if the entity is added state or modified state with patch requests
                if (streamState == EntityStates.Added || descriptor.State == EntityStates.Added ||
                    (descriptor.State == EntityStates.Modified && !Util.IsFlagSet(this.Options, SaveChangesOptions.ReplaceOnUpdate)))
                {
                    string location;
                    string odataEntityId;
                    contentHeaders.TryGetHeader(XmlConstants.HttpResponseLocation, out location);
                    contentHeaders.TryGetHeader(XmlConstants.HttpODataEntityId, out odataEntityId);

                    if (location != null && location != entityDescriptor.GetLatestEditLink().AbsoluteUri)
                    {
                        Uri locationUri;
                        bool isLocationValidUri = Uri.TryCreate(location, UriKind.Absolute, out locationUri);

                        Debug.Assert(isLocationValidUri && locationUri == entityDescriptor.GetLatestEditLink(), "edit link must already be set to location header");
                    }

                    if ((location != null || odataEntityId != null) && (odataEntityId ?? location) != UriUtil.UriToString(entityDescriptor.GetLatestIdentity()))
                    {
                        if (odataEntityId != null)
                        {
                            Uri entityIdUri;
                            bool isEntityIdValidUri = Uri.TryCreate(odataEntityId, UriKind.RelativeOrAbsolute, out entityIdUri);

                            Debug.Assert(isEntityIdValidUri && entityIdUri == entityDescriptor.GetLatestIdentity(), "Identity must already be set");
                        }
                        else
                        {
                            Uri locationUri;
                            bool isLocationValidUri = Uri.TryCreate(location, UriKind.Absolute, out locationUri);

                            Debug.Assert(isLocationValidUri && locationUri == entityDescriptor.GetLatestIdentity(), "Identity must already be set");
                        }
                    }
                }
#endif
            }

            if (streamState == EntityStates.Added || descriptor.State == EntityStates.Added)
            {
                this.HandleResponsePost(descriptor, contentHeaders);
            }
            else if (streamState == EntityStates.Modified || descriptor.State == EntityStates.Modified)
            {
                this.HandleResponsePut(descriptor, contentHeaders);
            }
            else if (descriptor.State == EntityStates.Deleted)
            {
                this.HandleResponseDelete(descriptor);
            }

            // else condition is not interesting here and we intentionally do nothing.
        }

        /// <summary>
        /// Get the materializer to process the response.
        /// </summary>
        /// <param name="entityDescriptor">entity descriptor whose response is getting materialized.</param>
        /// <param name="responseInfo">information about the response to be materialized.</param>
        /// <returns>an instance of MaterializeAtom, that can be used to materialize the response.</returns>
        protected abstract MaterializeAtom GetMaterializer(EntityDescriptor entityDescriptor, ResponseInfo responseInfo);

        /// <summary>cleanup work to do once the batch / savechanges is complete</summary>
        protected override void CompletedRequest()
        {
            this.buildBatchBuffer = null;
        }

        /// <summary>
        /// Create the response info instance to be passed to the materializer.
        /// </summary>
        /// <param name="entityDescriptor">entity descriptor whose response is getting handled.</param>
        /// <returns>instance of the response info class.</returns>
        protected ResponseInfo CreateResponseInfo(EntityDescriptor entityDescriptor)
        {
            MergeOption mergeOption = MergeOption.OverwriteChanges;

            // If we are processing a POST MR, we want to materialize the payload to get the metadata for the stream.
            // However we must not modify the MLE properties with the server initialized properties.  The next request
            // will be a Put MLE operation and we will set the server properties with values from the client entity.
            if (entityDescriptor.StreamState == EntityStates.Added)
            {
                mergeOption = MergeOption.PreserveChanges;
                Debug.Assert(entityDescriptor.State == EntityStates.Modified, "The MLE state must be Modified.");
            }

            return this.RequestInfo.GetDeserializationInfo(mergeOption);
        }

        /// <summary>
        /// enumerate the related Modified/Unchanged links for an added item
        /// </summary>
        /// <param name="entityDescriptor">entity</param>
        /// <returns>related links</returns>
        /// <remarks>
        /// During a non-batch SaveChanges, an Added entity can become an Unchanged entity
        /// and should be included in the set of related links for the second Added entity.
        /// </remarks>
        protected IEnumerable<LinkDescriptor> RelatedLinks(EntityDescriptor entityDescriptor)
        {
            foreach (LinkDescriptor end in this.RequestInfo.EntityTracker.Links)
            {
                if (end.Source == entityDescriptor.Entity)
                {
                    if (null != end.Target)
                    {   // null TargetResource is equivalent to Deleted
                        EntityDescriptor target = this.RequestInfo.EntityTracker.GetEntityDescriptor(end.Target);

                        // assumption: the source entity started in the Added state
                        // note: SaveChanges operates with two passes
                        //      a) first send the request and then attach identity and append the result into a batch response  (Example: BeginSaveChanges)
                        //      b) process the batch response (shared code with SaveChanges(BatchWithSingleChangeset))  (Example: EndSaveChanges)
                        // note: SaveResultWasProcessed is set when to the pre-save state when the save result is sucessfully processed

                        // scenario #1 when target entity started in modified or unchanged state
                        // 1) the link target entity was modified and now implicitly assumed to be unchanged (this is true in second pass)
                        // 2) or link target entity has not been saved is in the modified or unchanged state (this is true in first pass)

                        // scenario #2 when target entity started in added state
                        // 1) target entity has an identity (true in first pass for non-batch)
                        // 2) target entity is processed before source to qualify (1) better during the second pass
                        // 3) the link target has not been saved and is in the added state
                        // 4) or the link target has been saved and was in the added state
                        if (Util.IncludeLinkState(target.SaveResultWasProcessed) || ((0 == target.SaveResultWasProcessed) && Util.IncludeLinkState(target.State)) ||
                            ((null != target.Identity) && (target.ChangeOrder < entityDescriptor.ChangeOrder) &&
                             ((0 == target.SaveResultWasProcessed && EntityStates.Added == target.State) ||
                              (EntityStates.Added == target.SaveResultWasProcessed))))
                        {
                            Debug.Assert(entityDescriptor.ChangeOrder < end.ChangeOrder, "saving is out of order");
                            yield return end;
                        }
                    }
                }
            }
        }

        /// <summary>flag results as being processed</summary>
        /// <param name="descriptor">result descriptor being processed</param>
        /// <returns>count of related links that were also processed</returns>
        protected int SaveResultProcessed(Descriptor descriptor)
        {
            // media links will be processed twice
            descriptor.SaveResultWasProcessed = descriptor.State;

            int count = 0;
            if (descriptor.DescriptorKind == DescriptorKind.Entity && (EntityStates.Added == descriptor.State))
            {
                foreach (LinkDescriptor end in this.RelatedLinks((EntityDescriptor)descriptor))
                {
                    if (end.ContentGeneratedForSave)
                    {
                        Debug.Assert(0 == end.SaveResultWasProcessed, "this link already had a result");
                        end.SaveResultWasProcessed = end.State;
                        count++;
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// Generate the link payload.
        /// </summary>
        /// <param name="binding">binding</param>
        /// <returns>An instance of ODataRequestMessage for the link request.</returns>
        protected ODataRequestMessageWrapper CreateRequest(LinkDescriptor binding)
        {
            Debug.Assert(null != binding, "null binding");
            if (binding.ContentGeneratedForSave)
            {
                return null;
            }

            EntityDescriptor sourceEntityDescriptor = this.RequestInfo.EntityTracker.GetEntityDescriptor(binding.Source);
            EntityDescriptor targetEntityDescriptor = (null != binding.Target) ? this.RequestInfo.EntityTracker.GetEntityDescriptor(binding.Target) : null;

            // We allow the source and target to be in Added state, i.e. without identities, for batch with single changeset.
            if (!Util.IsBatchWithSingleChangeset(this.Options))
            {
                ValidateLinkDescriptorSourceAndTargetHaveIdentities(binding, sourceEntityDescriptor, targetEntityDescriptor);
            }

            Debug.Assert(this.IsBatchRequest || null != sourceEntityDescriptor.GetLatestIdentity(), "missing sourceResource.Identity in non-batch");

            Uri requestUri = null;
            LinkInfo linkInfo = null;

            if (sourceEntityDescriptor.TryGetLinkInfo(binding.SourceProperty, out linkInfo)
                && linkInfo.AssociationLink != null)
            {
                Debug.Assert(null != sourceEntityDescriptor.GetLatestIdentity(), "Source must have an identity in order to have link info");

                // If there is already an Association link from the payload, use that
                requestUri = linkInfo.AssociationLink;
            }
            else
            {
                Uri sourceEntityUri;
                if (null == sourceEntityDescriptor.GetLatestIdentity())
                {
                    Debug.Assert(this.IsBatchRequest && Util.IsBatchWithSingleChangeset(this.Options), "Source must have an identity outside of batch with single changeset");

                    // if the source hasn't yet been inserted (because its in batch), then create a uri based on its content-ID
                    sourceEntityUri = UriUtil.CreateUri("$" + sourceEntityDescriptor.ChangeOrder.ToString(CultureInfo.InvariantCulture), UriKind.Relative);
                }
                else
                {
                    // otherwise use the edit link of the source
                    sourceEntityUri = sourceEntityDescriptor.GetResourceUri(this.RequestInfo.BaseUriResolver, false /*queryLink*/);
                }

                // get the source property Uri
                string sourcePropertyUri = GetSourcePropertyUri(binding, sourceEntityDescriptor);

                // get the convention-based relative uri for the association
                Uri conventionalRelativeUri = UriUtil.CreateUri(sourcePropertyUri, UriKind.Relative);

                // add $ref at the end
                conventionalRelativeUri = UriUtil.CreateUri(UriUtil.UriToString(conventionalRelativeUri) + UriHelper.FORWARDSLASH + XmlConstants.UriLinkSegment, UriKind.Relative);

                // combine the association uri with the source entity uri
                requestUri = UriUtil.CreateUri(sourceEntityUri, conventionalRelativeUri);
            }

            // in the case of deleting a link from a collection, the key of the target must be appended
            requestUri = AppendTargetEntityKeyIfNeeded(requestUri, binding, targetEntityDescriptor);

            string method = GetLinkHttpMethod(binding);

            HeaderCollection headers = new HeaderCollection();

            headers.SetRequestVersion(Util.ODataVersion4, this.RequestInfo.MaxProtocolVersionAsVersion);
            this.RequestInfo.Format.SetRequestAcceptHeader(headers);

            // if (EntityStates.Deleted || (EntityState.Modifed && null == TargetResource))
            // then the server will fail the batch section if content type exists
            if ((EntityStates.Added == binding.State) || (EntityStates.Modified == binding.State && (null != binding.Target)))
            {
                this.RequestInfo.Format.SetRequestContentTypeForLinks(headers);
            }

            return this.CreateRequestMessage(method, requestUri, headers, this.RequestInfo.HttpStack, binding, this.IsBatchRequest ? binding.ChangeOrder.ToString(CultureInfo.InvariantCulture) : null);
        }

        /// <summary>
        /// Create ODataRequestMessage for the given entity.
        /// </summary>
        /// <param name="entityDescriptor">resource</param>
        /// <returns>An instance of ODataRequestMessage for the given entity.</returns>
        protected ODataRequestMessageWrapper CreateRequest(EntityDescriptor entityDescriptor)
        {
            Debug.Assert(null != entityDescriptor, "null entityDescriptor");
            Debug.Assert(entityDescriptor.State == EntityStates.Added || entityDescriptor.State == EntityStates.Deleted || entityDescriptor.State == EntityStates.Modified, "the entity must be in one of the 3 possible states");

            EntityStates state = entityDescriptor.State;
            Uri requestUri = entityDescriptor.GetResourceUri(this.RequestInfo.BaseUriResolver, false /*queryLink*/);

            Debug.Assert(null != requestUri, "request uri is null");
            Debug.Assert(requestUri.IsAbsoluteUri, "request uri is not absolute uri");

            ClientEdmModel model = this.RequestInfo.Model;
            ClientTypeAnnotation clientType = model.GetClientTypeAnnotation(model.GetOrCreateEdmType(entityDescriptor.Entity.GetType()));
            Version requestVersion = DetermineRequestVersion(clientType);
            string httpMethod = this.GetHttpMethod(state, ref requestVersion);

            HeaderCollection headers = new HeaderCollection();

            // Set the content type
            if (EntityStates.Deleted != entityDescriptor.State)
            {
                this.RequestInfo.Context.Format.SetRequestContentTypeForEntry(headers);
            }

            // Set IfMatch (etag) header for update and delete requests
            if ((EntityStates.Deleted == state) || (EntityStates.Modified == state))
            {
                string etag = entityDescriptor.GetLatestETag();
                if (etag != null)
                {
                    headers.SetHeader(XmlConstants.HttpRequestIfMatch, etag);
                }
            }

            // Set the prefer header if required
            ApplyPreferences(headers, httpMethod, this.RequestInfo.AddAndUpdateResponsePreference, ref requestVersion);

            // Set the request DSV and request MDSV headers
            headers.SetRequestVersion(requestVersion, this.RequestInfo.MaxProtocolVersionAsVersion);

            this.RequestInfo.Format.SetRequestAcceptHeader(headers);
            return this.CreateRequestMessage(httpMethod, requestUri, headers, this.RequestInfo.HttpStack, entityDescriptor, this.IsBatchRequest ? entityDescriptor.ChangeOrder.ToString(CultureInfo.InvariantCulture) : null);
        }

        /// <summary>
        /// Returns the request message to write the headers and payload into.
        /// </summary>
        /// <param name="method">Http method for the request.</param>
        /// <param name="requestUri">Base Uri for the request.</param>
        /// <param name="headers">Request headers.</param>
        /// <param name="httpStack">HttpStack to use.</param>
        /// <param name="descriptor">Descriptor for the request, if there is one.</param>
        /// <returns>an instance of IODataRequestMessage.</returns>
        protected ODataRequestMessageWrapper CreateTopLevelRequest(string method, Uri requestUri, HeaderCollection headers, HttpStack httpStack, Descriptor descriptor)
        {
            BuildingRequestEventArgs args = this.RequestInfo.CreateRequestArgsAndFireBuildingRequest(method, requestUri, headers, httpStack, descriptor);
            return this.RequestInfo.WriteHelper.CreateRequestMessage(args);
        }

        /// <summary>
        /// Figures out value to be written in OData-Version HTTP header for the given entity based on features used in this entity.
        /// </summary>
        /// <param name="clientType">Entity type for which data service version needs to be determined.</param>
        /// <returns>Data service version for the given entity and state.</returns>
        private static Version DetermineRequestVersion(ClientTypeAnnotation clientType)
        {
            Debug.Assert(clientType != null, "clientType != null");
            Debug.Assert(clientType.IsEntityType, "This method should be called only for entities");

            // Determine what the version is based on the client type.
            Version requestVersion = Util.ODataVersion4;

            WebUtil.RaiseVersion(ref requestVersion, clientType.GetMetadataVersion());
            return requestVersion;
        }

        /// <summary>Checks whether a WCF Data Service version string can be handled.</summary>
        /// <param name="responseVersion">Version string on the response header; possibly null.</param>
        /// <param name="parsedResponseVersion">The response version parsed into a <see cref="Version"/> instance
        /// if the version was valid and can be handled, otherwise null.</param>
        /// <returns>true if the version can be handled; false otherwise.</returns>
        private static bool CanHandleResponseVersion(string responseVersion, out Version parsedResponseVersion)
        {
            parsedResponseVersion = null;

            if (!string.IsNullOrEmpty(responseVersion))
            {
                KeyValuePair<Version, string> version;
                if (!CommonUtil.TryReadVersion(responseVersion, out version))
                {
                    return false;
                }

                if (!Util.SupportedResponseVersions.Contains(version.Key))
                {
                    return false;
                }

                parsedResponseVersion = version.Key;
            }

            return true;
        }

        /// <summary>Handle changeset response.</summary>
        /// <param name="linkDescriptor">headers of changeset response</param>
        private static void HandleResponsePost(LinkDescriptor linkDescriptor)
        {
            if (!((EntityStates.Added == linkDescriptor.State) || (EntityStates.Modified == linkDescriptor.State && null != linkDescriptor.Target)))
            {
                Error.ThrowBatchUnexpectedContent(InternalError.LinkNotAddedState);
            }

            linkDescriptor.State = EntityStates.Unchanged;
        }

        /// <summary>
        /// Validates that the link descriptor source and target have identities.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="sourceResource">The source resource.</param>
        /// <param name="targetResource">The target resource.</param>
        private static void ValidateLinkDescriptorSourceAndTargetHaveIdentities(LinkDescriptor binding, EntityDescriptor sourceResource, EntityDescriptor targetResource)
        {
            Debug.Assert(!binding.ContentGeneratedForSave, "already saved link");

            // In non-batch scenarios, the source should always have an identity
            if (null == sourceResource.GetLatestIdentity())
            {
                binding.ContentGeneratedForSave = true;
                Debug.Assert(EntityStates.Added == sourceResource.State, "expected added state");
                throw Error.InvalidOperation(Strings.Context_LinkResourceInsertFailure, sourceResource.SaveError);
            }

            if (null != targetResource && null == targetResource.GetLatestIdentity())
            {
                binding.ContentGeneratedForSave = true;
                Debug.Assert(EntityStates.Added == targetResource.State, "expected added state");
                throw Error.InvalidOperation(Strings.Context_LinkResourceInsertFailure, targetResource.SaveError);
            }
        }

        /// <summary>
        /// Serialize supported data service versions to a string that will be used in the exception message.
        /// The string contains versions in single quotes separated by comma followed by a single space (e.g. "'1.0', '2.0'").
        /// </summary>
        /// <returns>Supported data service versions in single quotes separated by comma followed by a space.</returns>
        private static string SerializeSupportedVersions()
        {
            Debug.Assert(Util.SupportedResponseVersions.Length > 0, "At least one supported version must exist.");

            StringBuilder supportedVersions = new StringBuilder("'").Append(Util.SupportedResponseVersions[0].ToString());
            for (int versionIdx = 1; versionIdx < Util.SupportedResponseVersions.Length; versionIdx++)
            {
                supportedVersions.Append("', '");
                supportedVersions.Append(Util.SupportedResponseVersions[versionIdx].ToString());
            }

            supportedVersions.Append("'");

            return supportedVersions.ToString();
        }

        /// <summary>
        /// Appends the target entity key to the uri if the binding is in the deleted state and the property is a collection.
        /// </summary>
        /// <param name="linkUri">The link URI so far.</param>
        /// <param name="binding">The binding.</param>
        /// <param name="targetResource">The target's entity descriptor.</param>
        /// <returns>The original link uri or one with the target entity key appended.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0011:EntityDescriptorPublicPropertiesRule",
             Justification = "This property has been limited by get/set")]
        private static Uri AppendTargetEntityKeyIfNeeded(Uri linkUri, LinkDescriptor binding, EntityDescriptor targetResource)
        {
            // To delete from a collection, we need to append the key.
            // For example: if the navigation property name is "Purchases" and the resource type is Order with key '1', then this method will generate 'baseuri/Purchases(1)'
            if (!binding.IsSourcePropertyCollection || EntityStates.Deleted != binding.State)
            {
                return linkUri;
            }

            Debug.Assert(targetResource != null, "targetResource != null");
            StringBuilder builder = new StringBuilder();
            builder.Append(UriUtil.UriToString(linkUri));
            builder.Append(UriHelper.QUESTIONMARK + XmlConstants.HttpQueryStringId + UriHelper.EQUALSSIGN + targetResource.Identity);
            return UriUtil.CreateUri(builder.ToString(), UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        /// Generate a request for the given entity.
        /// </summary>
        /// <param name="entityDescriptor">Instance of EntityDescriptor.</param>
        /// <param name="requestMessage">Instance of IODataRequestMessage to be used to generate the payload.</param>
        /// <returns>True if the payload was generated, otherwise false.</returns>
        private bool CreateRequestData(EntityDescriptor entityDescriptor, ODataRequestMessageWrapper requestMessage)
        {
            Debug.Assert(null != entityDescriptor, "null entityDescriptor");
            bool generateRequestPayload = false;
            switch (entityDescriptor.State)
            {
                case EntityStates.Deleted:
                    break;
                case EntityStates.Modified:
                case EntityStates.Added:
                    generateRequestPayload = true;
                    break;
                default:
                    Error.ThrowInternalError(InternalError.UnvalidatedEntityState);
                    break;
            }

            if (generateRequestPayload)
            {
                Debug.Assert(this.SerializerInstance != null, "this.SerializerInstance != null");
                this.SerializerInstance.WriteEntry(entityDescriptor, this.RelatedLinks(entityDescriptor), requestMessage);
            }

            return generateRequestPayload;
        }

        /// <summary>
        /// Generate a request for the given link.
        /// </summary>
        /// <param name="binding">Instance of LinkDescriptor.</param>
        /// <param name="requestMessage">Instance of IODataRequestMessage to be used to generate the payload.</param>
        private void CreateRequestData(LinkDescriptor binding, ODataRequestMessageWrapper requestMessage)
        {
            Debug.Assert(
                (binding.State == EntityStates.Added) ||
                (binding.State == EntityStates.Modified && null != binding.Target),
                "This method must be called only when a binding is added or put");
#if DEBUG
            Debug.Assert(!Util.IsBatchWithSingleChangeset(this.Options) || this.IsBatchRequest, "If this.Options.IsBatchWithSingleChangeset() is true, this.IsBatchRequest must also be true.");
            this.SerializerInstance.WriteEntityReferenceLink(binding, requestMessage, Util.IsBatchWithSingleChangeset(this.Options));
#else
            this.SerializerInstance.WriteEntityReferenceLink(binding, requestMessage);
#endif
        }

        /// <summary>Handle changeset response.</summary>
        /// <param name="descriptor">descriptor whose response is getting handled.</param>
        /// <param name="contentHeaders">response headers.</param>
        private void HandleResponsePost(Descriptor descriptor, HeaderCollection contentHeaders)
        {
            if (descriptor.DescriptorKind == DescriptorKind.Entity)
            {
                string etag;
                contentHeaders.TryGetHeader(XmlConstants.HttpResponseETag, out etag);
                this.HandleResponsePost((EntityDescriptor)descriptor, etag);
            }
            else
            {
                HandleResponsePost((LinkDescriptor)descriptor);
            }
        }

        /// <summary>Handle changeset response for the given entity descriptor.</summary>
        /// <param name="entityDescriptor">entity descriptor whose response is getting handled.</param>
        /// <param name="etag">ETag header value from the server response (or null if no etag or if there is an actual response)</param>
        private void HandleResponsePost(EntityDescriptor entityDescriptor, string etag)
        {
            try
            {
                if (EntityStates.Added != entityDescriptor.State && EntityStates.Added != entityDescriptor.StreamState)
                {
                    Error.ThrowBatchUnexpectedContent(InternalError.EntityNotAddedState);
                }

                if (this.ProcessResponsePayload)
                {
                    this.MaterializeResponse(entityDescriptor, this.CreateResponseInfo(entityDescriptor), etag);
                }
                else
                {
                    entityDescriptor.ETag = etag;
                    entityDescriptor.State = EntityStates.Unchanged;
                    entityDescriptor.PropertiesToSerialize.Clear();
                }

                if (entityDescriptor.StreamState != EntityStates.Added)
                {
                    // For MR - entityDescriptor.State is merged, we don't need to do link folding since MR will never fold links.
                    foreach (LinkDescriptor end in this.RelatedLinks(entityDescriptor))
                    {
                        Debug.Assert(0 != end.SaveResultWasProcessed, "link should have been saved with the enty");

                        // Since we allow link folding on collection properties also, we need to check if the link
                        // was in added state also, and make sure we put that link in unchanged state.
                        if (Util.IncludeLinkState(end.SaveResultWasProcessed) || end.SaveResultWasProcessed == EntityStates.Added)
                        {
                            HandleResponsePost(end);
                        }
                    }
                }
            }
            finally
            {
                if (entityDescriptor.StreamState == EntityStates.Added)
                {
                    // The materializer will always set the entity state to Unchanged.  We just processed Post MR, we
                    // need to restore the entity state to Modified to process the Put MLE.
                    Debug.Assert(entityDescriptor.State == EntityStates.Unchanged, "The materializer should always set the entity state to Unchanged.");
                    entityDescriptor.State = EntityStates.Modified;

                    // Need to clear the stream state so the next iteration we will always process the Put MLE operation.
                    entityDescriptor.StreamState = EntityStates.Unchanged;
                }
            }
        }

        /// <summary>
        /// Handle the PUT response sent by the server
        /// </summary>
        /// <param name="descriptor">descriptor, whose response is getting handled.</param>
        /// <param name="responseHeaders">response headers.</param>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "instance is used in Assert")]
        private void HandleResponsePut(Descriptor descriptor, HeaderCollection responseHeaders)
        {
            Debug.Assert(descriptor != null, "descriptor != null");
            if (descriptor.DescriptorKind == DescriptorKind.Entity)
            {
                string etag;
                responseHeaders.TryGetHeader(XmlConstants.HttpResponseETag, out etag);
                EntityDescriptor entityDescriptor = (EntityDescriptor)descriptor;

                // Only process the response if the resource is an entity resource and process update response is set to true
                if (this.ProcessResponsePayload)
                {
                    this.MaterializeResponse(entityDescriptor, this.CreateResponseInfo(entityDescriptor), etag);
                }
                else
                {
                    if (EntityStates.Modified != entityDescriptor.State && EntityStates.Modified != entityDescriptor.StreamState)
                    {
                        Error.ThrowBatchUnexpectedContent(InternalError.EntryNotModified);
                    }

                    // We MUST process the MR before the MLE since we always issue the requests in that order.
                    if (entityDescriptor.StreamState == EntityStates.Modified)
                    {
                        entityDescriptor.StreamETag = etag;
                        entityDescriptor.StreamState = EntityStates.Unchanged;
                    }
                    else
                    {
                        Debug.Assert(entityDescriptor.State == EntityStates.Modified, "descriptor.State == EntityStates.Modified");
                        entityDescriptor.ETag = etag;
                        entityDescriptor.State = EntityStates.Unchanged;
                        entityDescriptor.PropertiesToSerialize.Clear();
                    }
                }
            }
            else if (descriptor.DescriptorKind == DescriptorKind.Link)
            {
                if ((EntityStates.Added == descriptor.State) || (EntityStates.Modified == descriptor.State))
                {
                    descriptor.State = EntityStates.Unchanged;
                }
                else if (EntityStates.Detached != descriptor.State)
                {   // this link may have been previously detached by a detaching entity
                    Error.ThrowBatchUnexpectedContent(InternalError.LinkBadState);
                }
            }
            else
            {
                Debug.Assert(descriptor.DescriptorKind == DescriptorKind.NamedStream, "it must be named stream");
                Debug.Assert(descriptor.State == EntityStates.Modified, "named stream must only be in modified state");
                descriptor.State = EntityStates.Unchanged;

                StreamDescriptor streamDescriptor = (StreamDescriptor)descriptor;

                // The named stream has been updated, so the old ETag value is stale. Replace
                // it with the new value or clear it if no value was specified.
                string etag;
                responseHeaders.TryGetHeader(XmlConstants.HttpResponseETag, out etag);
                streamDescriptor.ETag = etag;
            }
        }

        /// <summary>Handle response to deleted entity.</summary>
        /// <param name="descriptor">deleted entity</param>
        private void HandleResponseDelete(Descriptor descriptor)
        {
            if (EntityStates.Deleted != descriptor.State)
            {
                Error.ThrowBatchUnexpectedContent(InternalError.EntityNotDeleted);
            }

            if (descriptor.DescriptorKind == DescriptorKind.Entity)
            {
                EntityDescriptor resource = (EntityDescriptor)descriptor;
                this.RequestInfo.EntityTracker.DetachResource(resource);
            }
            else
            {
                this.RequestInfo.EntityTracker.DetachExistingLink((LinkDescriptor)descriptor, false);
            }
        }

#if DNXCORE50
        /// <summary>Handle responseStream.ReadAsync and complete the read operation.</summary>
        /// <param name="task">Task that has completed.</param>
        /// <param name="asyncState">State associated with the Task.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "required for this feature")]
        private void AsyncEndRead(Task task, object asyncState)
#else

        /// <summary>handle responseStream.BeginRead with responseStream.EndRead</summary>
        /// <param name="asyncResult">async result</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "required for this feature")]
        [SuppressMessage("DataWeb.Usage", "AC0014", Justification = "Throws every time")]
        private void AsyncEndRead(IAsyncResult asyncResult)
#endif
        {
#if DNXCORE50
            IAsyncResult asyncResult = (IAsyncResult)task;
#endif
            Debug.Assert(asyncResult != null && asyncResult.IsCompleted, "asyncResult.IsCompleted");
#if DNXCORE50
            AsyncReadState state = (AsyncReadState)asyncState;
#else
            AsyncReadState state = (AsyncReadState)asyncResult.AsyncState;
#endif
            PerRequest pereq = state.Pereq;
            int count = 0;
            try
            {
                this.CompleteCheck(pereq, InternalError.InvalidEndReadCompleted);
                pereq.SetRequestCompletedSynchronously(asyncResult.CompletedSynchronously); // BeginRead

                EqualRefCheck(this.perRequest, pereq, InternalError.InvalidEndRead);
                Stream httpResponseStream = Util.NullCheck(pereq.ResponseStream, InternalError.InvalidEndReadStream);

                Util.DebugInjectFault("SaveAsyncResult::AsyncEndRead_BeforeEndRead");
#if DNXCORE50
                count = ((Task<int>)task).Result;
#else
                count = httpResponseStream.EndRead(asyncResult);
#endif
                if (0 < count)
                {
                    Stream outputResponse = Util.NullCheck(this.ResponseStream, InternalError.InvalidEndReadCopy);
                    outputResponse.Write(this.buildBatchBuffer, 0, count);
                    state.TotalByteCopied += count;

                    if (!asyncResult.CompletedSynchronously && httpResponseStream.CanRead)
                    {
                        // if CompletedSynchronously then caller will call and we reduce risk of stack overflow
                        do
                        {
#if DNXCORE50
                            asyncResult = BaseAsyncResult.InvokeTask(httpResponseStream.ReadAsync, this.buildBatchBuffer, 0, this.buildBatchBuffer.Length, this.AsyncEndRead, new AsyncReadState(pereq));                            
#else
                            asyncResult = InvokeAsync(httpResponseStream.BeginRead, this.buildBatchBuffer, 0, this.buildBatchBuffer.Length, this.AsyncEndRead, state);
#endif
                            pereq.SetRequestCompletedSynchronously(asyncResult.CompletedSynchronously); // BeginRead
                        }
                        while (asyncResult.CompletedSynchronously && !pereq.RequestCompleted && !this.IsCompletedInternally && httpResponseStream.CanRead);
                    }
                }
                else
                {
                    pereq.SetComplete();

                    // BeginRead could fail and callback still invoked
                    // if pereq.RequestCompletedSynchronously is true, this.FinishCurrentChange() will be
                    // invoked by the current thread in SaveResult.BeginCreateNextChange().
                    // if pereq.RequestCompletedSynchronously is false, we will call this.FinishCurrentChange() here and
                    // the parent thread will not call this.FinishCurrentChange() in SaveResult.BeginCreateNextChange().
                    if (!this.IsCompletedInternally && !pereq.RequestCompletedSynchronously)
                    {
                        this.FinishCurrentChange(pereq);
                    }
                }
            }
            catch (Exception e)
            {
                if (this.HandleFailure(pereq, e))
                {
                    throw;
                }
            }
            finally
            {
                this.HandleCompleted(pereq);
            }
        }

        /// <summary>
        /// Materialize the response payload.
        /// </summary>
        /// <param name="entityDescriptor">entity descriptor whose response is getting materialized.</param>
        /// <param name="responseInfo">information about the response to be materialized.</param>
        /// <param name="etag">etag value, if specified in the response header.</param>
        private void MaterializeResponse(EntityDescriptor entityDescriptor, ResponseInfo responseInfo, string etag)
        {
            using (MaterializeAtom materializer = this.GetMaterializer(entityDescriptor, responseInfo))
            {
                materializer.SetInsertingObject(entityDescriptor.Entity);

                object materializedEntity = null;
                foreach (object x in materializer)
                {
                    Debug.Assert(materializedEntity == null, "entity == null");
                    if (materializedEntity != null)
                    {
                        Error.ThrowInternalError(InternalError.MaterializerReturningMoreThanOneEntity);
                    }

                    materializedEntity = x;
                }

                Debug.Assert(null != entityDescriptor.GetLatestIdentity(), "updated inserted should always gain an identity");
                Debug.Assert(materializedEntity == entityDescriptor.Entity, "x == entityDescriptor.Entity, should have same object generated by response");
                Debug.Assert(EntityStates.Unchanged == entityDescriptor.State, "should have moved out of insert");
                Debug.Assert(this.RequestInfo.EntityTracker.TryGetEntityDescriptor(entityDescriptor.GetLatestIdentity()) != null, "should have identity tracked");

                // If there was no etag specified in the payload, then we need to set the etag from the header
                if (entityDescriptor.GetLatestETag() == null)
                {
                    entityDescriptor.ETag = etag;
                }
            }
        }

        /// <summary>
        /// Get the source property Uri for the link URL
        /// </summary>
        /// <param name="binding">Link descriptor object of the binding</param>
        /// <param name="sourceEntityDescriptor">entity descriptor for source</param>
        /// <returns>source property Uri string</returns>
        private string GetSourcePropertyUri(LinkDescriptor binding, EntityDescriptor sourceEntityDescriptor)
        {
            Debug.Assert(binding != null, "binding != null");
            Debug.Assert(sourceEntityDescriptor != null, "sourceEntityDescriptor != null");

            if (string.IsNullOrEmpty(binding.SourceProperty))
            {
                return null;
            }

            string sourcePropertyUri = binding.SourceProperty;

            // Add type segment in the link URL for the derived entity type on which a navigation property is defined.
            // e.g. cxt.Attachto("<entitySetname>",<EntityToBeSource>)
            //      cxt.AddLink(<EntityToBeSource>, "<NavigationPropertyName>" <EntityToBeTarget>)
            // Get entity type name from model (here service model instead of client model should be used)
            string entityTypeFullName = this.RequestInfo.TypeResolver.ResolveServiceEntityTypeFullName(binding.Source.GetType());
            if (string.IsNullOrEmpty(entityTypeFullName))
            {
                return sourcePropertyUri;
            }

            // Get the type of entityset from service model.
            string sourceEntitySetTypeName = null;
            if (!string.IsNullOrEmpty(sourceEntityDescriptor.EntitySetName) && this.RequestInfo.TypeResolver.TryResolveEntitySetBaseTypeName(sourceEntityDescriptor.EntitySetName, out sourceEntitySetTypeName))
            {
                // Check whether the entity type and the entity set type are matched. if not matched, set the dervied entity type name as a key segment in the URL.
                if (!string.IsNullOrEmpty(sourceEntitySetTypeName) && !string.Equals(entityTypeFullName, sourceEntitySetTypeName, StringComparison.OrdinalIgnoreCase))
                {
                    sourcePropertyUri = entityTypeFullName + UriHelper.FORWARDSLASH + sourcePropertyUri;
                }
            }

            return sourcePropertyUri;
        }

        /// <summary>
        /// Async read state
        /// </summary>
        private struct AsyncReadState
        {
            /// <summary>PerRequest class which tracks the request and response stream </summary>
            internal readonly PerRequest Pereq;

            /// <summary>total number of byte copied.</summary>
            private int totalByteCopied;

            /// <summary>
            /// constructor
            /// </summary>
            /// <param name="pereq">Perrequest class</param>
            internal AsyncReadState(PerRequest pereq)
            {
                this.Pereq = pereq;
                this.totalByteCopied = 0;
            }

            /// <summary>
            /// Returns the total number of byte copied till now.
            /// </summary>
            internal int TotalByteCopied
            {
                get { return this.totalByteCopied; }
                set { this.totalByteCopied = value; }
            }
        }
    }
}

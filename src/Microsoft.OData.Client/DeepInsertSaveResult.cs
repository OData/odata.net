//---------------------------------------------------------------------
// <copyright file="DeepInsertSaveResult.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.OData.Client.Materialization;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Client
{
    /// <summary>
    /// Handles the deep insert requests and responses (both sync and async).
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "The response stream is disposed by the message reader we create over it which we dispose inside the enumerator.")]
    internal class DeepInsertSaveResult : BaseSaveResult
    {
        #region Private Fields

        /// <summary>Instance of ODataWriter used to write the deep insert request.</summary>
        private ODataWriterWrapper writer;

        /// <summary>
        /// Instance of the BulkUpdateGraph that contains descriptors with their 
        /// related descriptors to be used in writing deep insert requests.
        /// </summary>
        private readonly BulkUpdateGraph bulkUpdateGraph;

        /// <summary>A list of all the operation responses for a deep insert response.</summary>
        private readonly List<OperationResponse> operationResponses;

        /// <summary>The descriptor associated with a materializer state.</summary>
        private readonly Dictionary<Descriptor, IMaterializerState> materializerStateForDescriptor;

        /// <summary>
        /// We cache the current response and then parse it. We need to do this for the async case only.
        /// </summary>
        private Stream responseStream;

        /// <summary>The response headers</summary>
        private HeaderCollection headers;

        /// <summary>Top level entry.</summary>
        private MaterializerEntry topLevelEntry;

        #endregion

        /// <summary>
        /// Constructor for DeepInsertSaveResult.
        /// </summary>
        /// <param name="context">The runtime context of the data service.</param>
        /// <param name="method">Method name for the SaveChanges method.</param>
        /// <param name="options">Options when saving changes.</param>
        /// <param name="callback">The user callback.</param>
        /// <param name="state">The user state object.</param>
        internal DeepInsertSaveResult(DataServiceContext context, string method, SaveChangesOptions options, AsyncCallback callback, object state)
            : base(context, method, null, options, callback, state)
        {
            Debug.Assert(Util.IsDeepInsert(options), "the options must have deep insert flag set");
            this.bulkUpdateGraph = new BulkUpdateGraph(this.RequestInfo);
            this.operationResponses = new List<OperationResponse>();
            this.materializerStateForDescriptor = new Dictionary<Descriptor, IMaterializerState>();
        }

        /// <summary>
        /// Returns an instance of the <see cref="Client.BulkUpdateGraph"/>
        /// </summary>
        internal BulkUpdateGraph BulkUpdateGraph
        {
            get { return this.bulkUpdateGraph; }
        }

        protected override Stream ResponseStream
        {
            get { return this.responseStream; }
        }

        /// <summary>
        /// Synchronous deep insert request.
        /// </summary>
        /// <typeparam name="T"> The type of the top-level object to be deep inserted.</typeparam>
        /// <param name="resource"> The top-level object to be deep inserted.</param>
        internal void DeepInsertRequest<T>(T resource)
        {
            if (resource == null)
            {
                throw Error.ArgumentNull(nameof(resource));
            }

            BuildDescriptorGraph(this.ChangedEntries, true, resource);

            ODataRequestMessageWrapper deepInsertRequestMessage = this.GenerateDeepInsertRequest();

            if (deepInsertRequestMessage == null)
            {
                return;
            }

            deepInsertRequestMessage.SetRequestStream(deepInsertRequestMessage.CachedRequestStream);

            try
            {
                this.batchResponseMessage = this.RequestInfo.GetSynchronousResponse(deepInsertRequestMessage, false);
                this.responseStream = this.batchResponseMessage.GetStream();
            }
            catch (DataServiceTransportException ex)
            {
                InvalidOperationException exception = WebUtil.GetHttpWebResponse(ex, ref this.batchResponseMessage);

                throw exception;
            }
        }

        /// <summary>
        /// Begins an asynchronous deep insert request.
        /// </summary>
        /// <typeparam name="T">The type of the top-level object to be deep inserted.</typeparam>
        /// <param name="resource">The top-level object of the type to be deep inserted.</param>
        internal void BeginDeepInsertRequest<T>(T resource)
        {
            if (resource == null)
            {
                throw Error.ArgumentNull(nameof(resource));
            }

            PerRequest peReq = null;

            try
            {
                BuildDescriptorGraph(this.ChangedEntries, true, resource);
                ODataRequestMessageWrapper deepInsertRequestMessage = this.GenerateDeepInsertRequest();
                this.Abortable = deepInsertRequestMessage;

                deepInsertRequestMessage.SetContentLengthHeader();
                this.perRequest = peReq = new PerRequest();
                peReq.Request = deepInsertRequestMessage;
                peReq.RequestContentStream = deepInsertRequestMessage.CachedRequestStream;

                AsyncStateBag asyncStateBag = new AsyncStateBag(peReq);

                this.responseStream = new MemoryStream();

                IAsyncResult asyncResult = BaseAsyncResult.InvokeAsync(deepInsertRequestMessage.BeginGetRequestStream, this.AsyncEndGetRequestStream, asyncStateBag);

                peReq.SetRequestCompletedSynchronously(asyncResult.CompletedSynchronously);
            }
            catch (Exception e)
            {
                this.HandleFailure(peReq, e);
                throw;
            }
            finally
            {
                this.HandleCompleted(peReq);
            }

            Debug.Assert((this.CompletedSynchronously && this.IsCompleted) || !this.CompletedSynchronously, "sync without complete");
        }

        /// <summary>
        /// This method processes all the changed descriptors in the entity tracker.
        /// It loops through all the descriptors and creates relationships between the descriptors if any.
        /// </summary>
        /// <typeparam name="T">The type of the top-level resource we want to do a deep insert.</typeparam>
        /// <param name="descriptors">The list of descriptors in the entity tracker.</param>
        /// <param name="isRootObject">True if the <paramref name="objects"/> are root or top-level objects, i.e., they were passed directly by the client, otherwise false.</param>
        /// <param name="objects">Objects of the top-level type we want to perform a deep insert on.</param>
        /// <exception cref="InvalidOperationException"></exception>
        internal void BuildDescriptorGraph<T>(IEnumerable<Descriptor> descriptors, bool isRootObject, params T[] objects)
        {
            foreach (object obj in objects)
            {
                EntityDescriptor topLevelDescriptor = this.RequestInfo.EntityTracker.GetEntityDescriptor(obj);

                // If it's a top-level object sent by the user.
                if (isRootObject)
                {
                    // Validate that we can only have one top level entity in Deep Insert.
                    if (bulkUpdateGraph.TopLevelDescriptors.Count > 0)
                    {
                        throw Error.InvalidOperation(Strings.Context_DeepInsertOneTopLevelEntity);
                    }

                    topLevelDescriptor.ContentGeneratedForSave = true;
                    bulkUpdateGraph.AddTopLevelDescriptor(topLevelDescriptor);
                }

                foreach (Descriptor descriptor in descriptors)
                {
                    if (descriptor is EntityDescriptor entityDescriptor)
                    {
                        if (entityDescriptor.ParentEntity != null &&
                            entityDescriptor.ParentEntity.Equals(topLevelDescriptor.Entity) &&
                            !bulkUpdateGraph.Contains(entityDescriptor))
                        {
                            // We don't support delete and update in deep insert.
                            if (entityDescriptor.State == EntityStates.Deleted || entityDescriptor.State == EntityStates.Modified)
                            {
                                string entitySetAndKey = GetEntitySetAndKey(entityDescriptor);
                                throw Error.InvalidOperation(Strings.Context_DeepInsertDeletedOrModified(entitySetAndKey));
                            }

                            entityDescriptor.ContentGeneratedForSave = true;
                            bulkUpdateGraph.AddRelatedDescriptor(topLevelDescriptor, entityDescriptor);
                            this.BuildDescriptorGraph(descriptors, false, entityDescriptor.Entity);
                        }
                    }
                    else if (descriptor is LinkDescriptor linkDescriptor)
                    {
                        if (linkDescriptor.Source.Equals(topLevelDescriptor.Entity) && !bulkUpdateGraph.Contains(linkDescriptor))
                        {
                            EntityDescriptor targetDescriptor = this.RequestInfo.Context.GetEntityDescriptor(linkDescriptor.Target);

                            // We don't support delete and update in deep insert.
                            if (linkDescriptor.State == EntityStates.Deleted || linkDescriptor.State == EntityStates.Modified)
                            {
                                string entitySetAndKey = GetEntitySetAndKey(targetDescriptor);
                                throw Error.InvalidOperation(Strings.Context_DeepInsertDeletedOrModified(entitySetAndKey));
                            }

                            if (targetDescriptor != null && (targetDescriptor.State == EntityStates.Deleted || targetDescriptor.State == EntityStates.Modified))
                            {
                                string entitySetAndKey = GetEntitySetAndKey(targetDescriptor);
                                throw Error.InvalidOperation(Strings.Context_DeepInsertDeletedOrModified(entitySetAndKey));
                            }

                            linkDescriptor.ContentGeneratedForSave = true;
                            bulkUpdateGraph.AddRelatedDescriptor(topLevelDescriptor, linkDescriptor);
                            this.BuildDescriptorGraph(descriptors, false, linkDescriptor.Target);
                        }
                    }
                }
            }
        }

        private string GetEntitySetAndKey(EntityDescriptor entityDescriptor)
        {
            var baseUriString = this.RequestInfo.BaseUriResolver.GetBaseUriWithSlash().ToString();
            IEdmModel model = this.RequestInfo.Format.ServiceModel;
            string entitySetUri = entityDescriptor.EditLink.ToString().TrimStart(baseUriString.ToCharArray());

            return entitySetUri;
        }

        /// <summary>
        /// Creates an ODataRequestMessage for a deep insert request.
        /// </summary>
        /// <returns>Returns an instance of the ODataRequestMessage for the deep insert request.</returns>
        private ODataRequestMessageWrapper CreateDeepInsertRequest(BulkUpdateGraph bulkUpdateGraph)
        {
            EntityDescriptor baseDescriptor = bulkUpdateGraph.TopLevelDescriptors[0];
            Uri requestUri = UriUtil.CreateUri(this.RequestInfo.BaseUriResolver.GetBaseUriWithSlash(), UriUtil.CreateUri(bulkUpdateGraph.EntitySetName, UriKind.Relative));
            string httpMethod = XmlConstants.HttpMethodPost;

            HeaderCollection headers = new HeaderCollection();

            this.RequestInfo.Context.Format.SetRequestContentTypeForEntry(headers);

            headers.SetHeader(XmlConstants.HttpODataVersion, XmlConstants.ODataVersion401);
            headers.SetHeader(XmlConstants.HttpODataMaxVersion, XmlConstants.ODataVersion401);

            this.RequestInfo.Format.SetRequestAcceptHeader(headers);

            return this.CreateRequestMessage(httpMethod, requestUri, headers, this.RequestInfo.HttpStack, baseDescriptor, null);
        }

        /// <summary>
        /// Generate the deep insert request for all changes to save.
        /// </summary>
        /// <returns>Returns the instance of ODataRequestMessage containing all the headers and payload for the delta request.</returns>
        private ODataRequestMessageWrapper GenerateDeepInsertRequest()
        {
            ODataRequestMessageWrapper deepInsertRequestMessage = this.CreateDeepInsertRequest(this.bulkUpdateGraph);

            // we need to fire request after the headers have been written, but before we write the payload
            deepInsertRequestMessage.FireSendingRequest2(descriptor: null, true);

            using (ODataMessageWriter messageWriter = Serializer.CreateMessageWriter(deepInsertRequestMessage, this.RequestInfo, isParameterPayload: false))
            {
                this.writer = ODataWriterWrapper.CreateForEntry(messageWriter, this.RequestInfo.Configurations.RequestPipeline);
                EntityDescriptor topLevelDescriptor = this.bulkUpdateGraph.TopLevelDescriptors[0];
                this.SerializerInstance.WriteDeepInsertEntry(topLevelDescriptor, this.bulkUpdateGraph, this.writer);
            }

            return deepInsertRequestMessage;
        }

        /// <summary>
        /// returns true if the response payload needs to be processed.
        /// </summary>
        protected override bool ProcessResponsePayload
        {
            get
            {
                Debug.Assert(this.topLevelEntry != null, "There must be a top level entry.");
                return this.topLevelEntry != null;
            }
        }

        internal override bool IsBatchRequest
        {
            get { return false; }
        }

        protected override ODataRequestMessageWrapper CreateRequestMessage(string method, Uri requestUri, HeaderCollection headers, HttpStack httpStack, Descriptor descriptor, string contentId)
        {
            return this.CreateTopLevelRequest(method, requestUri, headers, httpStack, descriptor);
        }

        /// <summary>Read and store response data for the current change</summary>
        /// <param name="peReq">The completed <see cref="BaseAsyncResult.PerRequest"/> object</param>
        /// <remarks>This is called only from the async code paths, when the response to the deep insert request has been read fully.</remarks>
        protected override void FinishCurrentChange(PerRequest peReq)
        {
            base.FinishCurrentChange(peReq);

            // This resets the position in the buffered response stream to the beginning
            // so that we can start reading the response.
            // In this case the ResponseStream is always a MemoryStream since we cache the async response.
            if (this.responseStream.Position != 0)
            {
                this.responseStream.Position = 0;
            }

            this.perRequest = null;
            this.SetCompleted();
        }

        /// <summary>
        /// Gets the materializer state to process the response.
        /// </summary>
        /// <param name="entityDescriptor">The entity descriptor whose response is getting materialized.</param>
        /// <param name="responseInfo">Information about the response to be materialized.</param>
        /// <returns>An instance of <see cref="ObjectMaterializer"/> that can be used to materialize the response.</returns>
        protected override ObjectMaterializer GetMaterializer(EntityDescriptor entityDescriptor, ResponseInfo responseInfo)
        {
            Debug.Assert(this.materializerStateForDescriptor != null, "this.materializerStateForDescriptor != null");

            if (this.materializerStateForDescriptor.TryGetValue(entityDescriptor, out IMaterializerState materializerState) && materializerState is MaterializerEntry materializerEntry)
            {
                return new ObjectMaterializer(responseInfo, new[] { materializerEntry.Entry }, entityDescriptor.Entity.GetType(), materializerEntry.Format, base.MaterializerCache, false);
            }

            return null;
        }

        protected override void HandleOperationResponse(IODataResponseMessage responseMessage)
        {
            this.batchResponseMessage = responseMessage;
        }

        protected override DataServiceResponse HandleResponse()
        {
            // This will process the responses and throw if failure was detected
            if (this.ResponseStream != null)
            {
                return this.HandleDeepInsertResponse();
            }

            return new DataServiceResponse(null, (int)WebExceptionStatus.Success, new List<OperationResponse>(0), batchResponse: this.IsBatchRequest);
        }

        private DataServiceResponse HandleDeepInsertResponse()
        {
            EntityDescriptor entityDescriptor = this.bulkUpdateGraph.TopLevelDescriptors[0];
            MaterializerEntry entry = default;
            DataServiceResponse response = new DataServiceResponse(headers, (int)this.batchResponseMessage.StatusCode, operationResponses, this.IsBatchRequest);

            try
            {
                // We are setting the MaxProtocolVersion to v4.01 when reading deep insert responses.
                this.RequestInfo.Context.MaxProtocolVersion = ODataProtocolVersion.V401;
                Version responseVersion;
                HandleResponse(
                    this.RequestInfo,
                    (HttpStatusCode)this.batchResponseMessage.StatusCode,
                    this.batchResponseMessage.GetHeader(XmlConstants.HttpODataVersion),
                    () => this.ResponseStream,
                    throwOnFailure: true,
                    out responseVersion);

                HeaderCollection headers = new HeaderCollection(this.batchResponseMessage);

                ResponseInfo responseInfo = this.CreateResponseInfo(entityDescriptor);
                HttpWebResponseMessage responseMessageWrapper = new HttpWebResponseMessage(
                    headers,
                    this.batchResponseMessage.StatusCode,
                    () => this.ResponseStream);

                ODataMaterializerContext materializerContext = new ODataMaterializerContext(responseInfo, base.MaterializerCache);
                entry = ODataReaderEntityMaterializer.ParseSingleEntityPayload(responseMessageWrapper, responseInfo, entityDescriptor.Entity.GetType(), materializerContext);

                this.headers = headers;
                this.topLevelEntry = entry;
            }
            catch (InvalidOperationException ex)
            {
                HeaderCollection headers = new HeaderCollection(this.batchResponseMessage);
                int statusCode = this.batchResponseMessage == null ? (int)HttpStatusCode.InternalServerError : (int)this.batchResponseMessage.StatusCode;
                DataServiceResponse dataServiceResponse = new DataServiceResponse(headers, statusCode, Enumerable.Empty<OperationResponse>(), this.IsBatchRequest);
                throw new DataServiceRequestException(Strings.DataServiceException_GeneralError, ex, dataServiceResponse);
            }

            HandleDeepInsertResponseInternal(entry : entry, isTopLevelDescriptor : true, descriptor : entityDescriptor, parentOperationResponse : null);

            return response;
        }

        private void HandleDeepInsertResponseInternal(MaterializerEntry entry, bool isTopLevelDescriptor, Descriptor descriptor, OperationResponse parentOperationResponse)
        {
            OperationResponse response = this.CreateOperationResponse(entry, isTopLevelDescriptor, descriptor, parentOperationResponse);

            List<Descriptor> relatedDescriptors = this.BulkUpdateGraph.GetRelatedDescriptors(descriptor);

            int currentRelatedIndex = 0; // Keep track of the index of the current related Descriptor we are trying to match with the entries in NestedItems

            foreach (IMaterializerState nestedItem in entry?.NestedItems)
            {
                if (nestedItem is MaterializerNestedEntry materializerNestedEntry)
                {
                    foreach (IMaterializerState nestedEntries in materializerNestedEntry.NestedItems)
                    {
                        if (nestedEntries is MaterializerFeed feed)
                        {
                            for (int i = 0; i < feed.Items.Count; i++)
                            {
                                MaterializerEntry nestedEntry = feed.Items[i] as MaterializerEntry;
                                if (nestedEntry == null || !nestedEntry.IsTracking || nestedEntry.EntityHasBeenResolved || currentRelatedIndex >= relatedDescriptors.Count)
                                {
                                    break;
                                }

                                HandleDeepInsertResponseInternal(nestedEntry, false, relatedDescriptors[currentRelatedIndex++], response);
                            }
                        }
                        else if (nestedEntries is MaterializerEntry nestedEntry)
                        {
                            if (nestedEntry.EntityHasBeenResolved || !nestedEntry.IsTracking || currentRelatedIndex >= relatedDescriptors.Count)
                            {
                                break;
                            }

                            HandleDeepInsertResponseInternal(nestedEntry, false, relatedDescriptors[currentRelatedIndex++], response);
                        }
                    }
                }
            }
        }

        private OperationResponse CreateOperationResponse(
            MaterializerEntry materializerState,
            bool isTopLevelResponse,
            Descriptor descriptor,
            OperationResponse parentOperationResponse)
        {
            Exception ex = null;
            OperationResponse operationResponse = null;
            DataServiceResponse dataServiceResponse = new DataServiceResponse(
                headers: null,
                statusCode: -1,
                this.operationResponses,
                batchResponse: false);

            try
            {
                if (descriptor == null)
                {
                    return null;
                }

                this.SaveResultProcessed(descriptor, HasFailedOperation(materializerState));

                operationResponse = new ChangeOperationResponse(this.headers, descriptor)
                {
                    StatusCode = this.batchResponseMessage.StatusCode
                };

                this.materializerStateForDescriptor.Add(descriptor, materializerState);

                if (isTopLevelResponse)
                {
                    this.operationResponses.Add(operationResponse);
                    this.HandleLocationHeaders(descriptor, (HttpStatusCode)this.batchResponseMessage.StatusCode, this.headers);
                }
                else
                {
                    // In POST request, we have a location header from the response.
                    // We should not use the location header for nested descriptors since the location header only applies for the top level resource/descriptor.
                    string location;
                    this.headers.TryGetHeader(XmlConstants.HttpResponseLocation, out location);

                    if (location != null)
                    {
                        this.headers.UnderlyingDictionary.Remove(XmlConstants.HttpResponseLocation);
                    }

                    parentOperationResponse.NestedResponses.Add(operationResponse);
                }
#if DEBUG
                this.HandleOperationResponse(descriptor, this.headers, (HttpStatusCode)this.batchResponseMessage.StatusCode);
#else
                this.HandleOperationResponse(descriptor, this.headers);
#endif
                return operationResponse;
            }
            catch (InvalidOperationException e)
            {
                ex = e;
            }

            if (ex != null)
            {
                throw new DataServiceRequestException(Strings.DataServiceException_GeneralError, ex, dataServiceResponse);
            }

            return null;
        }

        /// <summary>
        /// Checks whether an entry has a data modification exception instance annotation.
        /// </summary>
        /// <param name="entry">The materializer entry to check for the data modification exception instance annotation.</param>
        /// <returns>true if the entry has the data modification exception, otherwise false.</returns>
        private static bool HasFailedOperation(MaterializerEntry entry)
        {
            return entry.Entry.InstanceAnnotations.Any(a => a.Name == XmlConstants.DataModificationException);
        }

        private void HandleLocationHeaders(Descriptor descriptor, HttpStatusCode statusCode, HeaderCollection headers)
        {
            if (descriptor.DescriptorKind == DescriptorKind.Entity)
            {
                EntityDescriptor entityDescriptor = (EntityDescriptor)descriptor;

                if (descriptor.State == EntityStates.Added && WebUtil.SuccessStatusCode(statusCode))
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
                    else if (descriptor.State == EntityStates.Added)
                    {
                        // For POST scenarios, location header must be specified.
                        // Except for preflight requests
                        if (headers.HasHeader("Content-Type") && statusCode != HttpStatusCode.Created)
                        {
                            throw Error.NotSupported(Strings.Deserialize_NoLocationHeader);
                        }
                    }

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

                    if (editLink != null)
                    {
                        this.RequestInfo.EntityTracker.AttachLocation(entityDescriptor.Entity, odataId, editLink);
                    }
                }
            }
        }
    }
}

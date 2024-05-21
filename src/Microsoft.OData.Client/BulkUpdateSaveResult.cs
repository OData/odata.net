//---------------------------------------------------------------------
// <copyright file="BulkUpdateSaveResult.cs" company="Microsoft">
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
using Microsoft.OData.Client.Materialization;

namespace Microsoft.OData.Client
{
    /// <summary>
    /// Handles the bulk update requests and responses (both sync and async)
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "The response stream is disposed by the message reader we create over it which we dispose inside the enumerator.")]
    internal class BulkUpdateSaveResult : BaseSaveResult
    {
        #region Private Fields

        /// <summary>Instance of ODataWriter used to write the bulk update request.</summary>
        private ODataWriterWrapper writer;

        /// <summary>
        /// Instance of the BulkUpdateGraph that contains descriptors with their
        /// related descriptors to be used in writing bulk update requests.
        /// </summary>
        private readonly BulkUpdateGraph bulkUpdateGraph;

        /// <summary> 
        /// Link descriptors associated with entity descriptors that were added to the entity tracker during an $expand operation.
        /// </summary>
        /// <remarks>
        /// If we expand the related resources to some resources when we are fetching data from an OData endpoint, 
        /// OData client creates descriptors for the retrieved descriptors and creates links for 
        /// the related descriptors. If we want to update the related objects' we'll get the related resources
        /// make changes to them then call the `UpdateObject` to update the descriptors then save the changes.
        /// The updated descriptors do not have any information that routes them to the parent descriptors but 
        /// we can get that information from the Links object in the entitytracker.
        /// </remarks>
        private readonly Dictionary<Descriptor, List<LinkDescriptor>> linkDescriptors;

        /// <summary>The bulk update response cache.</summary>
        private CachedResponse cachedResponse;

        /// <summary>The descriptor associated with a materializer state.</summary>
        private readonly Dictionary<Descriptor, IMaterializerState> materializerStateForDescriptor;

        /// <summary>A list of all the operation responses for a bulk update response.</summary>
        private readonly List<OperationResponse> operationResponses;

        /// <summary>
        /// We cache the current response and then parse it. We need to do this for the async case only.
        /// </summary>
        private Stream responseStream;

        #endregion

        /// <summary>
        /// Constructor for BulkUpdateSaveResult
        /// </summary>
        /// <param name="context">The runtime context of the data service.</param>
        /// <param name="method">Method name for the SaveChanges method.</param>
        /// <param name="options">Options when saving changes.</param>
        /// <param name="callback">The user callback.</param>
        /// <param name="state">The user state object.</param>
        internal BulkUpdateSaveResult(DataServiceContext context, string method, SaveChangesOptions options, AsyncCallback callback, object state)
            : base(context, method, null, options, callback, state)
        {
            Debug.Assert(Util.IsBulkUpdate(options), "the options must have bulk update flag set");
            this.bulkUpdateGraph = new BulkUpdateGraph(this.RequestInfo);
            this.linkDescriptors = new Dictionary<Descriptor, List<LinkDescriptor>>();
            this.materializerStateForDescriptor = new Dictionary<Descriptor, IMaterializerState>();
            this.operationResponses = new List<OperationResponse>();
        }

        /// <summary>
        /// Returns an instance of the <see cref="Client.BulkUpdateGraph"/>
        /// </summary>
        internal BulkUpdateGraph BulkUpdateGraph
        {
            get { return this.bulkUpdateGraph; }
        }

        /// <summary>
        /// Returns an instance of the link descriptors.
        /// </summary>
        internal Dictionary<Descriptor, List<LinkDescriptor>> LinkDescriptors
        {
            get { return this.linkDescriptors; }
        }

        protected override Stream ResponseStream
        {
            get { return this.responseStream; }
        }

        /// <summary>
        /// Asynchronous bulk update request.
        /// </summary>
        /// <typeparam name="T">The type of the top-level objects to be deep-updated.</typeparam>
        /// <param name="objects">The top-level objects of the type to be deep updated.</param>
        internal void BeginBulkUpdateRequest<T>(params T[] objects)
        {
            PerRequest pereq = null;

            if (objects == null || objects.Length == 0)
            {
                throw Error.Argument(Strings.Util_EmptyArray, nameof(objects));
            }

            BuildDescriptorGraph(this.ChangedEntries, true, objects);
            
            try 
            {
                ODataRequestMessageWrapper bulkUpdateRequestMessage = this.GenerateBulkUpdateRequest();
                this.Abortable = bulkUpdateRequestMessage;

                if (bulkUpdateRequestMessage != null)
                {
                    bulkUpdateRequestMessage.SetContentLengthHeader();
                    this.perRequest = pereq = new PerRequest();
                    pereq.Request = bulkUpdateRequestMessage;
                    pereq.RequestContentStream = bulkUpdateRequestMessage.CachedRequestStream;

                    AsyncStateBag asyncStateBag = new AsyncStateBag(pereq);

                    this.responseStream = new MemoryStream();

                    IAsyncResult asyncResult = BaseAsyncResult.InvokeAsync(bulkUpdateRequestMessage.BeginGetRequestStream, this.AsyncEndGetRequestStream, asyncStateBag);
                    
                    pereq.SetRequestCompletedSynchronously(asyncResult.CompletedSynchronously);
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
            catch (Exception e)
            {
                this.HandleFailure(pereq, e);
                throw; 
            }
            finally
            {
                this.HandleCompleted(pereq);
            }

            Debug.Assert((this.CompletedSynchronously && this.IsCompleted) || !this.CompletedSynchronously, "sync without complete");
        }

        /// <summary>
        /// Synchronous bulk update request.
        /// </summary>
        /// <typeparam name="T">The type of the top-level objects to be deep-updated.</typeparam>
        /// <param name="objects">The top-level objects of the type to be deep updated.</param>
        internal void BulkUpdateRequest<T>(params T[] objects)
        {
            if (objects == null || objects.Length == 0)
            {
                throw Error.Argument(Strings.Util_EmptyArray, nameof(objects));
            }

            BuildDescriptorGraph(this.ChangedEntries, true, objects);

            ODataRequestMessageWrapper bulkUpdateRequestMessage = this.GenerateBulkUpdateRequest();

            if (bulkUpdateRequestMessage == null)
            {
                return;
            }

            bulkUpdateRequestMessage.SetRequestStream(bulkUpdateRequestMessage.CachedRequestStream);

            try
            {
                this.batchResponseMessage = this.RequestInfo.GetSynchronousResponse(bulkUpdateRequestMessage, false);
                this.responseStream = this.batchResponseMessage.GetStream();
                this.HandleBulkUpdateResponse(this.batchResponseMessage, responseStream);
            }
            catch (DataServiceTransportException ex)
            {
                InvalidOperationException exception = WebUtil.GetHttpWebResponse(ex, ref this.batchResponseMessage);

                throw exception;
            }
        }

        /// <summary>Reads and stores response data for the bulk update request.</summary>
        /// <param name="pereq">The completed per request object.</param>
        protected override void FinishCurrentChange(PerRequest pereq)
        {
            base.FinishCurrentChange(pereq);

            Debug.Assert(this.ResponseStream != null, "this.HttpWebResponseStream != null");
            Debug.Assert((this.ResponseStream as MemoryStream) != null, "(this.HttpWebResponseStream as MemoryStream) != null");

            if (this.ResponseStream.Position != 0)
            {
                // Set the stream to the start position and then parse the response and cache it
                this.ResponseStream.Position = 0;
                this.HandleBulkUpdateResponse(this.batchResponseMessage, this.ResponseStream);
            }
            else
            {
                this.HandleBulkUpdateResponse(this.batchResponseMessage, null);
            }

            pereq.Dispose();
            this.perRequest = null;
            this.SetCompleted();
        }

        /// <summary>
        /// This method processes all the changed descriptors in the entity tracker.
        /// It loops through all the descriptors and creates relationships between the descriptors if any.
        /// </summary>
        /// <typeparam name="T">The type of the top-level resources we want to do a deep update on.</typeparam>
        /// <param name="descriptors">The list of descriptors in the entity tracker.</param>
        /// <param name="isRootObject">True if the <paramref name="objects"/> are root or top-level objects, i.e., they were passed directly by the client, otherwise false.</param>
        /// <param name="objects">Objects of the top-level type we want to perform a deep update on.</param>
        /// <exception cref="InvalidOperationException"></exception>
        internal void BuildDescriptorGraph<T>(IEnumerable<Descriptor> descriptors, bool isRootObject, params T[] objects)
        {
            foreach (object obj in objects)
            {
                EntityDescriptor topLevelDescriptor = this.RequestInfo.EntityTracker.GetEntityDescriptor(obj);

                // If we do not find an EntityDescriptor in the entitytracker
                // for any of the provided objects then we throw an exception. 
                if (topLevelDescriptor == null)
                {
                    throw Error.InvalidOperation(Strings.Context_EntityNotContained);
                }

                // If it is a top-level object,
                // meaning that it was sent as top-level object by the user
                // then we add it to graph's top-level objects. 
                if (isRootObject)
                {
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
                            bulkUpdateGraph.AddRelatedDescriptor(topLevelDescriptor, entityDescriptor);
                            this.BuildDescriptorGraph(descriptors, false, entityDescriptor.Entity);
                        }
                        else
                        {
                            // We check if the link representation of the descriptor exists in the entity tracker and it is unchanged. 
                            LinkDescriptor linkDescriptor = this.RequestInfo.EntityTracker.Links.Where(
                                a => a.Source == topLevelDescriptor.Entity && 
                                this.RequestInfo.EntityTracker.GetEntityDescriptor(a.Target) == entityDescriptor &&
                                a.State == EntityStates.Unchanged).FirstOrDefault();
                            if (linkDescriptor != null)
                            {
                                bulkUpdateGraph.AddRelatedDescriptor(topLevelDescriptor, entityDescriptor);
                                this.GetLinkDescriptors(entityDescriptor).Add(linkDescriptor);
                                this.BuildDescriptorGraph(descriptors, false, entityDescriptor.Entity);
                            }         
                        }
                    }
                    else if (descriptor is LinkDescriptor link)
                    {
                        if (link.Source.Equals(topLevelDescriptor.Entity) && !bulkUpdateGraph.Contains(link))
                        {
                            bulkUpdateGraph.AddRelatedDescriptor(topLevelDescriptor, link);
                            this.BuildDescriptorGraph(descriptors, false, link.Target);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the link descriptor representations for the provided descriptor.
        /// </summary>
        /// <param name="descriptor">The descriptor.</param>
        /// <returns>All the link descriptors to a given key descriptor.</returns>
        public List<LinkDescriptor> GetLinkDescriptors(Descriptor descriptor)
        {
            if (this.linkDescriptors.TryGetValue(descriptor, out List<LinkDescriptor> relatedLinkDescriptors))
            {
                return relatedLinkDescriptors;
            }

            relatedLinkDescriptors = new List<LinkDescriptor>();
            this.linkDescriptors[descriptor] = relatedLinkDescriptors;

            return relatedLinkDescriptors;
        }

        /// <summary>
        /// Creates an ODataRequestMessage for a bulk update request.
        /// </summary>
        /// <returns>Returns an instance of the ODataRequestMessage for the bulk update request.</returns>
        private ODataRequestMessageWrapper CreateBulkUpdateRequest(BulkUpdateGraph bulkUpdateGraph)
        {
            EntityDescriptor baseDescriptor = bulkUpdateGraph.TopLevelDescriptors[0];
            Uri requestUri = UriUtil.CreateUri(this.RequestInfo.BaseUriResolver.GetBaseUriWithSlash(), UriUtil.CreateUri(bulkUpdateGraph.EntitySetName, UriKind.Relative));
            Version requestVersion = Util.ODataVersion401;
            string httpMethod = this.GetHttpMethod(EntityStates.Modified, ref requestVersion);

            HeaderCollection headers = new HeaderCollection();

            this.RequestInfo.Context.Format.SetRequestContentTypeForEntry(headers);

            ApplyPreferences(headers, httpMethod, this.RequestInfo.AddAndUpdateResponsePreference, ref requestVersion);

            headers.SetHeader(XmlConstants.HttpODataVersion, XmlConstants.ODataVersion401);
            headers.SetHeader(XmlConstants.HttpODataMaxVersion, XmlConstants.ODataVersion401);

            this.RequestInfo.Format.SetRequestAcceptHeader(headers);

            return this.CreateRequestMessage(httpMethod, requestUri, headers, this.RequestInfo.HttpStack, baseDescriptor, this.IsBatchRequest ? baseDescriptor.ChangeOrder.ToString(CultureInfo.InvariantCulture) : null);
        }

        /// <summary>
        /// Generate the bulk update request for all changes to save.
        /// </summary>
        /// <returns>Returns the instance of ODataRequestMessage containing all the headers and payload for the delta request.</returns>
        private ODataRequestMessageWrapper GenerateBulkUpdateRequest()
        {
            if (this.bulkUpdateGraph.TopLevelDescriptors.Count == 0)
            {
                this.SetCompleted();
                return null;
            }
 
            ODataRequestMessageWrapper bulkUpdateRequestMessage = this.CreateBulkUpdateRequest(this.bulkUpdateGraph);

            // we need to fire request after the headers have been written, but before we write the payload
            bulkUpdateRequestMessage.FireSendingRequest2(descriptor: null, true);

            using (ODataMessageWriter messageWriter = Serializer.CreateDeltaMessageWriter(bulkUpdateRequestMessage, this.RequestInfo, isParameterPayload: false))
            {
                this.writer = ODataWriterWrapper.CreateForDeltaFeed(messageWriter, this.bulkUpdateGraph.EntitySetName, this.RequestInfo.Configurations.RequestPipeline, bulkUpdateRequestMessage, this.RequestInfo);
                this.SerializerInstance.WriteDeltaResourceSet(this.bulkUpdateGraph.TopLevelDescriptors, this.linkDescriptors, this.bulkUpdateGraph, writer);
            }

            return bulkUpdateRequestMessage;
        }

        /// <summary>
        /// Returns true if the response payload needs to be processed.
        /// </summary>
        protected override bool ProcessResponsePayload
        {
            get
            {
                Debug.Assert(this.cachedResponse.Exception == null, "no exception should have been encountered");
                return this.cachedResponse.DeltaFeed != null;
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

        /// <summary>
        /// Gets the materializer state to process the response.
        /// </summary>
        /// <param name="entityDescriptor">The entity descriptor whose response is getting materialized.</param>
        /// <param name="responseInfo">Information about the response to be materialized.</param>
        /// <returns>An instance of <see cref="ObjectMaterializer"/> that can be used to materialize the response.</returns>
        protected override ObjectMaterializer GetMaterializer(EntityDescriptor entityDescriptor, ResponseInfo responseInfo)
        {
            Debug.Assert(this.cachedResponse.Exception == null && this.materializerStateForDescriptor != null, "this.cachedResponse.Exception == null && this.materializerStateForDescriptor != null");

            if (this.materializerStateForDescriptor.TryGetValue(entityDescriptor, out IMaterializerState materializerState) && materializerState is MaterializerEntry materializerEntry)
            {
                return new ObjectMaterializer(responseInfo, new[] { materializerEntry.Entry }, entityDescriptor.Entity.GetType(), materializerEntry.Format, base.MaterializerCache);
            }

            return null;
        }
        
        /// <summary>
        /// Handles the <see cref="IODataResponseMessage"/> for the bulk update operation.
        /// </summary>
        /// <param name="responseMessage">An instance of <see cref="IODataResponseMessage"/>.</param>
        protected override void HandleOperationResponse(IODataResponseMessage responseMessage)
        {
            this.batchResponseMessage = responseMessage;
        }

        /// <summary>
        /// Handles the bulk update response.
        /// </summary>
        /// <returns>An instance of the DataServiceResponse, containing responses for all the operations in a bulk update.</returns>
        protected override DataServiceResponse HandleResponse()
        {
            DataServiceResponse dataServiceResponse = new DataServiceResponse(headers: null, statusCode: -1, this.operationResponses, batchResponse: false);

            if (this.cachedResponse.Exception != null)
            {
                throw new DataServiceRequestException(Strings.DataServiceException_GeneralError, this.cachedResponse.Exception, dataServiceResponse);
            }
                
            Stack<(int LastVisited, IReadOnlyList<Descriptor> Descriptors)> adjacentDescriptors = new Stack<(int LastVisited, IReadOnlyList<Descriptor> Descriptors)>();
            Stack<List<OperationResponse>> operationResponseTracker = new Stack<List<OperationResponse>>();
            List<IMaterializerState> entries = this.cachedResponse.DeltaFeed.Entries;

            HandleInnerBulkUpdateResponse(entries, true, adjacentDescriptors, operationResponseTracker);

            return dataServiceResponse;
        }

        /// <summary>
        /// Loops through the materializer list entries and creates an operation response for each entry.
        /// </summary>
        /// <param name="entries">The materializer entries for the bulk update response.</param>
        /// <param name="isTopLevelResponse">true if the materializer entry is for a top-level response, otherwise false.</param>
        /// <param name="adjacentDescriptors">Tracks the descriptors adjacent to a descriptor that has been visited.</param>
        /// <param name="operationResponseTracker">Tracks the operation responses for the operations that have been performed.</param>
        private void HandleInnerBulkUpdateResponse(
            List<IMaterializerState> entries, 
            bool isTopLevelResponse, 
            Stack<(int LastVisited, IReadOnlyList<Descriptor> Descriptors)> adjacentDescriptors,
            Stack<List<OperationResponse>> operationResponseTracker) 
        {  
            foreach (IMaterializerState entry in entries)
            {
                if (entry is MaterializerEntry materializerEntry)
                {
                    CreateOperationResponse(materializerEntry, isTopLevelResponse, adjacentDescriptors, operationResponseTracker);
                   
                    foreach (IMaterializerState nestedItem in materializerEntry?.NestedItems)
                    {
                        if (nestedItem is MaterializerNestedEntry materializerNestedEntry)
                        {
                            foreach (IMaterializerState nestedEntries in materializerNestedEntry?.NestedItems)
                            {
                                if (nestedEntries is MaterializerDeltaFeed feed)
                                {
                                    this.HandleInnerBulkUpdateResponse(feed.Entries, false, adjacentDescriptors, operationResponseTracker);
                                }
                            }
                        }
                    }
                }
                else if (entry is MaterializerDeletedEntry deletedEntry)
                {
                    CreateOperationResponse(deletedEntry, isTopLevelResponse, adjacentDescriptors, operationResponseTracker);
                }
            }
        }

        /// <summary>
        /// Processes the bulk update <see cref="IODataResponseMessage"/> response and caches the result.
        /// </summary>
        /// <param name="response">An instance of the <see cref="IODataResponseMessage"/> for the bulk update response.</param>
        /// <param name="responseStream">The response stream.</param>
        private void HandleBulkUpdateResponse(IODataResponseMessage response, Stream responseStream)
        {
            EntityDescriptor entityDescriptor = this.bulkUpdateGraph.TopLevelDescriptors[0];
            MaterializerDeltaFeed deltaFeed = default;

            // We are setting the MaxProtocolVersion to v4.01 when reading bulk update responses.
            this.RequestInfo.Context.MaxProtocolVersion = ODataProtocolVersion.V401;
            Exception exception = HandleResponse(
                this.RequestInfo, 
                (HttpStatusCode)response.StatusCode, 
                response.GetHeader(XmlConstants.HttpODataVersion), 
                () => this.ResponseStream,
                throwOnFailure: false, 
                out Version responseVersion);

            HeaderCollection headers = new HeaderCollection(response);
           
            if (responseStream != null && entityDescriptor.DescriptorKind == DescriptorKind.Entity && exception == null)
            {
                try
                {
                    ResponseInfo responseInfo = this.CreateResponseInfo(entityDescriptor);
                    HttpWebResponseMessage responseMessageWrapper = new HttpWebResponseMessage(
                        headers,
                        response.StatusCode,
                        () => this.ResponseStream);

                    ODataMaterializerContext materializerContext = new ODataMaterializerContext(responseInfo, base.MaterializerCache);
                    deltaFeed = ODataReaderEntityMaterializer.ParseDeltaResourceSetPayload(responseMessageWrapper, responseInfo, entityDescriptor.Entity.GetType(), materializerContext);
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

            // Cache the response.
            this.cachedResponse = new CachedResponse(
                this.bulkUpdateGraph,
                headers,
                (HttpStatusCode)response.StatusCode,
                responseVersion,
                deltaFeed,
                exception);
        }

        /// <summary>
        /// Gets the last descriptor that was processed or visited. 
        /// </summary>
        /// <param name="adjacentDescriptors">Tracks the descriptors adjacent to a descriptor that has been visited.</param>
        /// <returns>The descriptor for the particular operation being processed.</returns>
        private static Descriptor GetLastVisitedDescriptor(Stack<(int LastVisited, IReadOnlyList<Descriptor> Descriptors)> adjacentDescriptors)
        {
            Descriptor lastVisitedDescriptor = null;

            if (adjacentDescriptors.Count > 0)
            {
                int index = adjacentDescriptors.Peek().LastVisited;

                IReadOnlyList<Descriptor> descriptors = adjacentDescriptors.Peek().Descriptors;

                if (index < descriptors.Count)
                {
                    lastVisitedDescriptor = descriptors[index];
                }
            }
           
            return lastVisitedDescriptor;
        }

        /// <summary>
        /// Checks whether an entry has a data modification exception instance annotation.
        /// </summary>
        /// <param name="materializerState">The materializer state entry to check for the data modification exception instance annotation.</param>
        /// <returns>true if the entry has the data modification exception, otherwise false.</returns>
        private static bool HasFailedOperation(IMaterializerState materializerState)
        {
            if (materializerState is MaterializerDeletedEntry deltaEntry)
            {
                return deltaEntry.DeletedResource.InstanceAnnotations.Any(a => a.Name == XmlConstants.DataModificationException);
            }
            else if (materializerState is MaterializerEntry entry)
            {
                return entry.Entry.InstanceAnnotations.Any(a => a.Name == XmlConstants.DataModificationException);
            }

            return false;
        }

        /// <summary>
        /// Creates an operation response for each operation.
        /// </summary>
        /// <param name="materializerState">An instance of the <see cref="IMaterializerState"/> entry associated with an operation response.</param>
        /// <param name="isTopLevelResponse">true for a top-level response otherwise false.</param>
        /// <param name="adjacentDescriptors">Tracks the descriptors adjacent to a descriptor that has been visited.</param>
        /// <param name="operationResponseTracker">Tracks the operation responses for the operations that have been performed.</param>
        /// <exception cref="DataServiceRequestException"></exception>
        private void CreateOperationResponse(
            IMaterializerState materializerState, 
            bool isTopLevelResponse, 
            Stack<(int LastVisited, IReadOnlyList<Descriptor> Descriptors)> adjacentDescriptors,
            Stack<List<OperationResponse>> operationResponseTracker)
        {
            Exception ex = null;
            OperationResponse operationResponse = null;
            DataServiceResponse dataServiceResponse = new DataServiceResponse(
                headers: null, 
                statusCode: -1, 
                this.operationResponses, 
                batchResponse: false);
            Descriptor descriptor = null;

            try
            {
                if (adjacentDescriptors.Count == 0)
                {
                    descriptor = this.bulkUpdateGraph.TopLevelDescriptors[0];
                    // If the adjacentDescriptors is empty then we push a tuple with index 0 and 
                    // the list of top-level descriptors to the stack.  
                    adjacentDescriptors.Push((0, this.bulkUpdateGraph.TopLevelDescriptors));
                }
                else
                {
                    Descriptor lastVisitedDescriptor = GetLastVisitedDescriptor(adjacentDescriptors);

                    if (lastVisitedDescriptor != null)
                    {
                        IReadOnlyList<Descriptor> relatedDescriptors = this.bulkUpdateGraph.GetRelatedDescriptors(lastVisitedDescriptor);

                        if (relatedDescriptors.Count == 0)
                        {
                            // Check the index of the last visited descriptor in the descriptors list.
                            // If the index is the same value as the length of the descriptors list then
                            // all the descriptors in the descriptors list have been traversed and we pop the items from the stack. 
                            if (adjacentDescriptors.Peek().LastVisited == adjacentDescriptors.Peek().Descriptors.Count - 1)
                            {
                                adjacentDescriptors.Pop();
                            }

                            int index = adjacentDescriptors.Peek().LastVisited;
                            IReadOnlyList<Descriptor> descriptors = adjacentDescriptors.Peek().Descriptors;

                            if (index == descriptors.Count - 1)
                            {
                                adjacentDescriptors.Pop();
                            }
                            else
                            {
                                // To get the next descriptor to be visited, we take the last visited index and increment it by one.
                                // Then we get the descriptor at the new index from the descriptors list. 
                                // We then update the index to the new index value. 
                                descriptor = descriptors[index + 1];
                                adjacentDescriptors.Pop();
                                adjacentDescriptors.Push((index + 1, descriptors));
                            }
                        }
                        else 
                        {
                            // If we find the last visited descriptors had related descriptors then we start traversing the related descriptors list and
                            // we start with the descriptor at index 0 and keep updating the index as we traverse the list. 
                            descriptor = relatedDescriptors[0];
                            adjacentDescriptors.Push((0, relatedDescriptors));
                        }
                    }          
                }
                 
                if (descriptor != null) 
                {
                    this.SaveResultProcessed(descriptor, HasFailedOperation(materializerState));

                    operationResponse = new ChangeOperationResponse(this.cachedResponse.Headers, descriptor)
                    {
                        StatusCode = (int)this.cachedResponse.StatusCode
                    };

                    this.materializerStateForDescriptor.Add(descriptor, materializerState);

#if DEBUG
                    this.HandleOperationResponse(descriptor, this.cachedResponse.Headers, this.cachedResponse.StatusCode);
#else
                    this.HandleOperationResponse(descriptor, this.cachedResponse.Headers);
#endif

                    if (isTopLevelResponse)
                    {
                        this.operationResponses.Add(operationResponse);
                        operationResponseTracker.Push(operationResponses);
                    }
                    else
                    {
                        List<OperationResponse> lastOperationResponses = operationResponseTracker.Peek();
                        lastOperationResponses[lastOperationResponses.Count-1].NestedResponses.Add(operationResponse);

                        if (this.bulkUpdateGraph.GetRelatedDescriptors(descriptor).Count == 0)
                        {
                            operationResponseTracker.Pop();
                            operationResponseTracker.Push(lastOperationResponses);
                        }
                        else
                        {
                            operationResponseTracker.Pop();
                            operationResponseTracker.Push(lastOperationResponses[lastOperationResponses.Count-1].NestedResponses);
                        }
                    }
                }  
            }
            catch (InvalidOperationException e)
            {
                ex = e;
            }

            if (ex != null)
            {
                throw new DataServiceRequestException(Strings.DataServiceException_GeneralError, ex, dataServiceResponse);
            }
        }
        
        /// <summary>
        /// The cached response of a bulk update response.
        /// </summary>
        private readonly struct CachedResponse
        {
            /// <summary>The response headers</summary>
            public readonly HeaderCollection Headers;

            /// <summary>The response status code</summary>
            public readonly HttpStatusCode StatusCode;

            /// <summary>The parsed response OData-Version header.</summary>
            public readonly Version Version;

            /// <summary>The <see cref="MaterializerDeltaFeed"/> of the <see cref="ODataDeltaResourceSet"/> entry being parsed.</summary>
            public readonly MaterializerDeltaFeed DeltaFeed;

            /// <summary>Exception if encountered.</summary>
            public readonly Exception Exception;

            /// <summary>The descriptor graph that was used when writing the request and whose response is being parsed.</summary>
            public readonly BulkUpdateGraph DescriptorGraph;

            /// <summary>
            /// Creates an instance of <see cref="CachedResponse"/>.
            /// </summary>
            /// <param name="descriptorGraph">The descriptor graph used in writing the request whose response is getting processed.</param>
            /// <param name="headers">The headers.</param>
            /// <param name="statusCode">The status code.</param>
            /// <param name="responseVersion">The parsed response OData-Version header.</param>
            /// <param name="deltaFeed">The <see cref="MaterializerDeltaFeed"/> of the <see cref="ODataDeltaResourceSet"/> of the response payload.</param>
            /// <param name="exception">The exception, if the request threw an exception.</param>
            internal CachedResponse(BulkUpdateGraph descriptorGraph, HeaderCollection headers, HttpStatusCode statusCode, Version responseVersion, MaterializerDeltaFeed deltaFeed, Exception exception)
            {
                Debug.Assert(descriptorGraph != null, "descriptorGraph != null");
                Debug.Assert(headers != null, "headers != null");
                Debug.Assert(deltaFeed == null || (exception == null), "if deltaFeed is specified, exception cannot be specified");

                this.DescriptorGraph = descriptorGraph;
                this.DeltaFeed = deltaFeed;
                this.Exception = exception;
                this.Headers = headers;
                this.StatusCode = statusCode;
                this.Version = responseVersion;
            }
        }
    }
}

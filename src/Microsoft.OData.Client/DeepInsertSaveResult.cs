//---------------------------------------------------------------------
// <copyright file="DeepInsertSaveResult.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.OData.Client
{
    /// <summary>
    /// Handles the deep insert requests and responses (both sync and async).
    /// </summary>
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
            this.linkDescriptors = new Dictionary<Descriptor, List<LinkDescriptor>>();
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

        protected override Stream ResponseStream => throw new NotImplementedException();

        /// <summary>
        /// Synchronous deep insert request.
        /// </summary>
        /// <typeparam name="T"> The type of the top-level object to be deep inserted.</typeparam>
        /// <param name="resource"> The top-level object to be deep inserted.</param>
        internal void DeepInsertRequest<T>(T resource)
        {
            BuildDescriptorGraph(this.ChangedEntries, true, resource);

            ODataRequestMessageWrapper deeepInsertRequestMessage = this.GenerateDeepInsertRequest();

            if (deeepInsertRequestMessage == null)
            {
                return;
            }

            deeepInsertRequestMessage.SetRequestStream(deeepInsertRequestMessage.CachedRequestStream);

            try
            {
                this.batchResponseMessage = this.RequestInfo.GetSynchronousResponse(deeepInsertRequestMessage, false);
            }
            catch (DataServiceTransportException ex)
            {
                InvalidOperationException exception = WebUtil.GetHttpWebResponse(ex, ref this.batchResponseMessage);

                throw exception;
            }
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

                // If we do not find an EntityDescriptor in the entitytracker
                // for any of the provided objects then we throw an exception. 
                if (topLevelDescriptor == null)
                {
                    throw Error.InvalidOperation(Strings.Context_EntityNotContained);
                }

                // If it's a top-level object sent by the user.
                if (isRootObject)
                {
                    // Validate that we can only have one top level entity in Deep Insert.
                    if (bulkUpdateGraph.TopLevelDescriptors.Count > 0)
                    {
                        throw Error.InvalidOperation(Strings.Context_DeepInsertOneTopLevelEntity);
                    }

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
                                throw Error.InvalidOperation(Strings.Context_DeepInsertDeletedOrModified);
                            }

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
                                // We don't support delete and update in deep insert.
                                // We check if the linkDescriptor or the descriptor of the target entity is Deleted or Modified
                                if (entityDescriptor.State == EntityStates.Deleted || entityDescriptor.State == EntityStates.Modified || linkDescriptor.State == EntityStates.Deleted || linkDescriptor.State == EntityStates.Modified)
                                {
                                    throw Error.InvalidOperation(Strings.Context_DeepInsertDeletedOrModified);
                                }

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
                            // We don't support delete and update in deep insert.
                            if (link.State == EntityStates.Deleted || link.State == EntityStates.Modified)
                            {
                                throw Error.InvalidOperation(Strings.Context_DeepInsertDeletedOrModified);
                            }

                            EntityDescriptor targetDescriptor = this.RequestInfo.Context.GetEntityDescriptor(link.Target);

                            if (targetDescriptor != null && (targetDescriptor.State == EntityStates.Deleted || targetDescriptor.State == EntityStates.Modified))
                            {
                                throw Error.InvalidOperation(Strings.Context_DeepInsertDeletedOrModified);
                            }

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

            headers.SetHeader("OData-Version", "4.01");
            headers.SetHeader("OData-MaxVersion", "4.01");

            this.RequestInfo.Format.SetRequestAcceptHeader(headers);

            return this.CreateRequestMessage(httpMethod, requestUri, headers, this.RequestInfo.HttpStack, baseDescriptor, this.IsBatchRequest ? baseDescriptor.ChangeOrder.ToString(CultureInfo.InvariantCulture) : null);
        }

        /// <summary>
        /// Generate the deep insert request for all changes to save.
        /// </summary>
        /// <returns>Returns the instance of ODataRequestMessage containing all the headers and payload for the delta request.</returns>
        private ODataRequestMessageWrapper GenerateDeepInsertRequest()
        {
            if (this.bulkUpdateGraph.TopLevelDescriptors.Count == 0)
            {
                this.SetCompleted();
                return null;
            }
 
            ODataRequestMessageWrapper deepInsertRequestMessage = this.CreateDeepInsertRequest(this.bulkUpdateGraph);

            // we need to fire request after the headers have been written, but before we write the payload
            deepInsertRequestMessage.FireSendingRequest2(descriptor: null, true);

            using (ODataMessageWriter messageWriter = Serializer.CreateMessageWriter(deepInsertRequestMessage, this.RequestInfo, isParameterPayload: false))
            {
                this.writer = ODataWriterWrapper.CreateForEntry(messageWriter, this.RequestInfo.Configurations.RequestPipeline);
                EntityDescriptor topLevelDescriptor = this.bulkUpdateGraph.TopLevelDescriptors[0];
                this.SerializerInstance.WriteDeepInsertEntry(topLevelDescriptor, this.linkDescriptors, this.bulkUpdateGraph, this.writer);
            }

            return deepInsertRequestMessage;
        }

        protected override bool ProcessResponsePayload => throw new NotImplementedException();

        internal override bool IsBatchRequest
        {
            get { return false; }
        }

        protected override ODataRequestMessageWrapper CreateRequestMessage(string method, Uri requestUri, HeaderCollection headers, HttpStack httpStack, Descriptor descriptor, string contentId)
        {
            return this.CreateTopLevelRequest(method, requestUri, headers, httpStack, descriptor);
        }

        protected override MaterializeAtom GetMaterializer(EntityDescriptor entityDescriptor, ResponseInfo responseInfo)
        {
            throw new NotImplementedException();
        }

        protected override void HandleOperationResponse(IODataResponseMessage responseMessage)
        {
            throw new NotImplementedException();
        }

        protected override DataServiceResponse HandleResponse()
        {
            throw new NotImplementedException();
        }
    }
}

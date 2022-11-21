//---------------------------------------------------------------------
// <copyright file="BulkUpdateSaveResult.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Microsoft.OData.Client
{
    /// <summary>
    /// Handles the bulk update requests and responses (both sync and async)
    /// </summary>
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
        }

        /// <summary>
        /// Returns an instance of the <see cref="BulkUpdateGraph"/>
        /// </summary>
        internal BulkUpdateGraph BulkUpdateGraph
        {
            get { return this.bulkUpdateGraph; }
        }

        protected override Stream ResponseStream => throw new NotImplementedException();

        /// <summary>
        /// Syncronous bulk update request.
        /// </summary>
        /// <typeparam name="T"> The type of the top-level objects to be deep-updated.</typeparam>
        /// <param name="objects"> The top-level objects of the type to be deep updated.</param>
        internal void BulkUpdateRequest<T>(params T[] objects)
        {
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

            headers.SetHeader("OData-Version", ODataVersion.V401.ToString());
            headers.SetHeader("OData-MaxVersion", "4.01");

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
                this.SerializerInstance.WriteDeltaResourceSet(this.bulkUpdateGraph.TopLevelDescriptors, this.bulkUpdateGraph, writer);
            }

            return bulkUpdateRequestMessage;
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

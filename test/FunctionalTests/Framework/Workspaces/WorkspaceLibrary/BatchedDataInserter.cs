//---------------------------------------------------------------------
// <copyright file="BatchedDataInserter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Test.Astoria
{
    /// <summary>
    /// An implementation of IDataInserter using batched Astoria requests
    /// </summary>
    public class BatchedDataInserter : IDataInserter
    {
        #region Constructor
        /// <summary>
        /// Constructs a BatchedDataInserter for the given workspace, with the given settings
        /// </summary>
        /// <param name="w">Workspace to use when creating requests</param>
        /// <param name="batchSize">maximum batch size</param>
        /// <param name="autoSubmit">whether or not to auto-submit the batch when the max size is reached</param>
        public BatchedDataInserter(Workspace w, uint batchSize, bool autoSubmit)
        {
            queue = new BatchRequestQueue(batchSize, autoSubmit);
            workspace = w;
        }
        #endregion

        #region private fields
        /// <summary>
        /// Used to track content-id's so that associations can be added using cross-referencing
        /// </summary>
        private Dictionary<KeyedResourceInstance, string> contentIDMap = new Dictionary<KeyedResourceInstance, string>();

        /// <summary>
        /// Used to queue up the requests and send them as a batch
        /// </summary>
        private BatchRequestQueue queue;

        /// <summary>
        /// Workspace to use when creating requests
        /// </summary>
        private Workspace workspace;
        #endregion

        #region IDataInserter Members

        /// <summary>
        /// Event fired when AddAssociation is called
        /// </summary>
        public event Action<KeyedResourceInstance, ResourceProperty, KeyedResourceInstance> OnAddingAssociation;

        /// <summary>
        /// Event fired when AddEntity is called
        /// </summary>
        public event Action<KeyExpression, KeyedResourceInstance> OnAddingEntity;

        /// <summary>
        /// Because the service must be set up before requests can be sent, this always returns false
        /// </summary>
        public bool BeforeServiceCreation
        {
            get { return false; }
        }

        /// <summary>
        /// Implements IDataInserter.AddAssociation by creating a PUT/POST $ref request using cross-referencing, and adds it to the batch
        /// </summary>
        /// <param name="parent">Update tree of parent, must have already been added using AddEntity</param>
        /// <param name="property">Navigation property for the association</param>
        /// <param name="child">Update tree of child, must have already been added using AddEntity</param>
        public void AddAssociation(KeyedResourceInstance parent, ResourceProperty property, KeyedResourceInstance child)
        {
            // determine which verb to use
            //
            RequestVerb verb;
            if (property.OtherAssociationEnd.Multiplicity == Multiplicity.Many)
                verb = RequestVerb.Post;
            else
                verb = RequestVerb.Put;

            // create the request
            //
            AstoriaRequest request = workspace.CreateRequest();
            request.Verb = verb;

            // use content-id cross referencing to link them
            //
            string parentContentId = contentIDMap[parent];
            string childContentId = contentIDMap[child];

            request.URI = "$" + parentContentId + "/" + property.Name + "/$ref";
            request.Payload = "<adsm:uri xmlns:adsm=\"http://docs.oasis-open.org/odata/ns/metadata\">$" + childContentId + "</adsm:uri>";

            // add it to the queue
            //
            queue.Add(request);

            // fire the event
            //
            if (this.OnAddingAssociation != null)
                OnAddingAssociation(parent, property, child);
        }

        /// <summary>
        /// Implements IDataInserter.AddEntity by creating a POST-based insert request and adding it to the batch queue
        /// </summary>
        /// <param name="key">Key expression for new entity</param>
        /// <param name="entity">Update tree for new entity</param>
        public void AddEntity(KeyExpression key, KeyedResourceInstance entity)
        {
            // build the request
            //
            ExpNode containerQuery = ContainmentUtil.BuildCanonicalQuery(key, true);
            AstoriaRequest request = workspace.CreateRequest(containerQuery, entity, RequestVerb.Post);

            // set ETagHeaderExpected appropriately
            if (key.ResourceType.Properties.Any(p => p.Facets.ConcurrencyModeFixed))
                request.ETagHeaderExpected = true;

            // add it to the queue
            //
            queue.Add(request);

            // store the content-id
            //
            contentIDMap[entity] = request.Headers["Content-ID"];

            // fire the event
            //
            if (this.OnAddingEntity != null)
                OnAddingEntity(key, entity);
        }

        /// <summary>
        /// Implements IDataInserter.Close by calling flush. No other operations are performed
        /// </summary>
        public void Close()
        {
            Flush();
        }

        /// <summary>
        /// Implements IDataInserter.Flush by sending the queued batch request and clearing the contentID list
        /// </summary>
        public void Flush()
        {
            // send the batch
            //
            queue.Finish();

            // clear the contentIDs
            //
            contentIDMap.Clear();
        }

        #endregion
    }
}

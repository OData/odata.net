//---------------------------------------------------------------------
// <copyright file="BulkUpdateGraph.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.UriParser;

namespace Microsoft.OData.Client
{
    /// <summary>
    /// Contains information about descriptors and their related descriptors. 
    /// </summary>
    internal sealed class BulkUpdateGraph
    {
        /// <summary>Set of related descriptors.</summary>
        private readonly Dictionary<Descriptor, List<Descriptor>> descriptorGraph;

        /// <summary>Set of top level descriptors.</summary>
        private readonly List<EntityDescriptor> topLevelDescriptors;

        /// <summary>The entity set name of the top-level descriptor types.</summary>
        private string entitySetName;

        /// <summary>where to pull the changes from</summary>
        private readonly RequestInfo requestInfo;

        public BulkUpdateGraph(RequestInfo requestInfo)
        { 
            this.descriptorGraph = new Dictionary<Descriptor, List<Descriptor>>(); 
            this.topLevelDescriptors = new List<EntityDescriptor>();
            this.requestInfo = requestInfo;
        }

        /// <summary>
        /// Returns a collection of all the root descriptors used in a deep update.
        /// </summary>
        public IReadOnlyList<EntityDescriptor> TopLevelDescriptors
        {
            get
            {
                return this.topLevelDescriptors;
            }
        }

        /// <summary>
        /// Gets the entity set name.
        /// </summary>
        public string EntitySetName
        {
            get
            {
                if (string.IsNullOrEmpty(this.entitySetName))
                {
                    ComputeEntitySetName();
                }

                return this.entitySetName;
            }
        }

        /// <summary>
        /// Computes the entity set name in cases where the top-level entity descriptors
        /// do not have the entity set name set. 
        /// </summary>
        private void ComputeEntitySetName()
        {
            if (this.TopLevelDescriptors.Count == 0)
            {
                this.entitySetName = string.Empty;
            }

            EntityDescriptor parentDescriptor = this.TopLevelDescriptors[0];

            if (!string.IsNullOrEmpty(parentDescriptor.EntitySetName))
            {
                this.entitySetName = parentDescriptor.EntitySetName;
            }
            else
            {
                Uri baseUri = this.requestInfo.BaseUriResolver.BaseUriOrNull == null ? null : this.requestInfo.BaseUriResolver.GetBaseUriWithSlash();
                Uri resourceUri = parentDescriptor.EditLink;
                ODataUriParser parser = new ODataUriParser(this.requestInfo.Format.ServiceModel, baseUri, resourceUri);
                ODataPath path = parser.ParsePath();

                if (path != null)
                {
                    EntitySetSegment entitySetSegment = path.OfType<EntitySetSegment>().LastOrDefault();
                    this.entitySetName = entitySetSegment.Identifier;
                }
                else
                {
                    throw Error.InvalidOperation(Strings.DataBinding_Util_UnknownEntitySetName(parentDescriptor.Entity.GetType().FullName));
                }
            }
        }

        /// <summary>
        /// Links a descriptor with its related descriptors.
        /// </summary>
        /// <param name="parent">The parent descriptor.</param>
        /// <param name="relatedDescriptor">The related descriptor to the parent descriptor.</param>
        public void AddRelatedDescriptor(Descriptor parent, Descriptor relatedDescriptor)
        {
            List<Descriptor> childrenDescriptors = this.GetRelatedDescriptors(parent);
            childrenDescriptors.Add(relatedDescriptor);
        }

        /// <summary>
        /// Adds to the top-level descriptors' list.
        /// </summary>
        /// <param name="descriptor">The descriptor to be added to the top-level descriptors' list.</param>
        public void AddTopLevelDescriptor(EntityDescriptor descriptor)
        {
            this.topLevelDescriptors.Add(descriptor);
        }

        /// <summary>
        /// Gets the descriptors of the entities related to the entity associated with the provided descriptor
        /// </summary>
        /// <param name="descriptor">The descriptor.</param>
        /// <returns>All the related descriptors to a given key descriptor.</returns>
        public List<Descriptor> GetRelatedDescriptors(Descriptor descriptor)
        {
            if (this.descriptorGraph.TryGetValue(descriptor, out List<Descriptor> relatedDescriptors))
            {
                return relatedDescriptors;
            }

            relatedDescriptors = new List<Descriptor>();
            this.descriptorGraph[descriptor] = relatedDescriptors;

            return relatedDescriptors;
        }

        /// <summary>
        /// This method checks whether the descriptor has already been added to the 
        /// graph to avoid cyclic operations. If the descriptor has already been added, we don't do the recursive call. 
        /// </summary>
        /// <param name="descriptor">The descriptor to check if it already exists in the graph.</param>
        /// <returns> True if the link already exists, otherwise returns false.</returns>
        public bool Contains(Descriptor descriptor)
        {
            return this.descriptorGraph.ContainsKey(descriptor);
        }
    }
}

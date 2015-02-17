//---------------------------------------------------------------------
// <copyright file="DictionaryMetadataHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.TestProviders.Dictionary
{
    using System.Collections.Generic;
    using Microsoft.OData.Service.Providers;

    /// <summary>
    /// Helper class for managing the metadata instances for the dictionary metadata provider implementation
    /// </summary>
    public class DictionaryMetadataHelper
    {      
        /// <summary>
        /// Initializes a new instance of the DictionaryMetadataHelper class
        /// </summary>
        public DictionaryMetadataHelper()
        {
            this.ResourceTypes = new List<ResourceType>();
            this.ResourceSets = new List<ResourceSet>();
            this.ResourceAssociationSets = new List<ResourceAssociationSet>();
            this.ServiceOperations = new List<ServiceOperation>();
        }

        /// <summary>
        /// Gets the list of resource types in the metadata
        /// </summary>
        public IList<ResourceType> ResourceTypes { get; private set; }

        /// <summary>
        /// Gets the list of resource sets in the metadata
        /// </summary>
        public IList<ResourceSet> ResourceSets { get; private set; }

        /// <summary>
        /// Gets the list of resource association sets in the metadata
        /// </summary>
        public IList<ResourceAssociationSet> ResourceAssociationSets { get; private set; }

        /// <summary>
        /// Gets the list of service operations in the metadata
        /// </summary>
        public IList<ServiceOperation> ServiceOperations { get; private set; }

        /// <summary>
        /// Gets the ResourceTypes associated with the particular ResourceSet
        /// </summary>
        /// <param name="set">Set to find the types</param>
        /// <returns>A List of ResourceTypes associated with the set</returns>
        public IList<ResourceType> GetResourceTypesOfResourceSet(ResourceSet set)
        {
            List<ResourceType> resourceTypes = new List<ResourceType>();
            foreach (ResourceType rt in this.ResourceTypes)
            {
                if (this.IsKindOf(set.ResourceType, rt))
                {
                    resourceTypes.Add(rt);
                }
            }

            return resourceTypes;
        }

        /// <summary>
        /// Determines if a ResourceType is derived from or equal to another ResourceType
        /// </summary>
        /// <param name="possibleType">Possible parent type of the childType</param>
        /// <param name="childType">Child Type to compare with</param>
        /// <returns>true if the possible Type is a Kind of the child type</returns>
        internal bool IsKindOf(ResourceType possibleType, ResourceType childType)
        {
            ResourceType currentType = childType;
            while (currentType != null)
            {
                if (currentType == possibleType)
                {
                    return true;
                }

                currentType = currentType.BaseType;
            }

            return false;
        }
    }
}
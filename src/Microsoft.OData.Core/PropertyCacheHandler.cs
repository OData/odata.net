//---------------------------------------------------------------------
// <copyright file="PropertyCacheHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.OData.Edm;

namespace Microsoft.OData
{
    /// <summary>
    /// Manage PropertyCache for ODataResourceSet in serialization.
    /// One ODataResourceSet has one PropertyCache.
    /// ODataResourceSets with same resource type share the same PropertyCache.
    /// </summary>
    internal class PropertyCacheHandler
    {
        private readonly Stack<PropertyCache> cacheStack = new Stack<PropertyCache>();

        private readonly Stack<int> scopeLevelStack = new Stack<int>();

        private readonly Dictionary<IEdmStructuredType, PropertyCache> cacheDictionary = new Dictionary<IEdmStructuredType, PropertyCache>();

        private PropertyCache currentPropertyCache;

        private int resourceSetScopeLevel;

        private int currentResourceScopeLevel;

        public PropertySerializationInfo GetProperty(IEdmModel model, string name, IEdmStructuredType owningType)
        {
            int depth = this.currentResourceScopeLevel - this.resourceSetScopeLevel;

            Debug.Assert(depth >= 1, "'depth' should always be greater than or equal to 1");

            return this.currentPropertyCache.GetProperty(model, depth, name, owningType);
        }

        public void SetCurrentResourceScopeLevel(int level)
        {
            this.currentResourceScopeLevel = level;
        }

        public void EnterResourceSetScope(IEdmStructuredType resourceType, int scopeLevel)
        {
            // Set cache for current resource set
            PropertyCache propertyCache;
            if (resourceType != null)
            {
                if (!cacheDictionary.TryGetValue(resourceType, out propertyCache))
                {
                    propertyCache = new PropertyCache();
                    cacheDictionary[resourceType] = propertyCache;
                }
            }
            else
            {
                propertyCache = new PropertyCache();
            }

            this.cacheStack.Push(this.currentPropertyCache);
            this.currentPropertyCache = propertyCache;

            // Set scope level for current resource set
            this.scopeLevelStack.Push(this.resourceSetScopeLevel);
            this.resourceSetScopeLevel = scopeLevel;
        }

        public void LeaveResourceSetScope()
        {
            Debug.Assert(this.cacheStack.Count != 0, "this.cacheStack.Count != 0");
            Debug.Assert(this.scopeLevelStack.Count != 0, "this.scopeLevelStack.Count != 0");

            this.resourceSetScopeLevel = this.scopeLevelStack.Pop();
            this.currentPropertyCache = this.cacheStack.Pop();
        }

        public bool InResourceSetScope()
        {
            return this.resourceSetScopeLevel > 0;
        }
    }
}

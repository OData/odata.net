//---------------------------------------------------------------------
// <copyright file="PropertyCacheHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
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

        public PropertySerializationInfo GetProperty(string name, IEdmStructuredType owningType)
        {
            StringBuilder uniqueName = new StringBuilder();
            if (owningType != null)
            {
                uniqueName.Append(owningType.FullTypeName());
                uniqueName.Append("-");
            }

            uniqueName.Append(name);
            if (this.currentResourceScopeLevel != this.resourceSetScopeLevel + 1)
            {
                // To avoid the property name conflicts of single navigation property and navigation source
                uniqueName.Append(this.currentResourceScopeLevel - this.resourceSetScopeLevel);
            }

            return this.currentPropertyCache.GetProperty(name, uniqueName.ToString(), owningType);
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

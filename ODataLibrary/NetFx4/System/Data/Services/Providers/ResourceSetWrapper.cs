//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Data.Services.Providers
{
    #region Namespaces.
    using System;
    using System.Collections.Generic;
    using System.Data.OData;
    using System.Diagnostics;
    using System.Linq;
    #endregion Namespaces.

    /// <summary>
    /// Wrapper class for a resource set.
    /// </summary>
    [DebuggerDisplay("{Name}: {ResourceType}")]
    internal class ResourceSetWrapper
    {
        #region Fields
        /// <summary>
        /// The string used as a key to a dictionary in CustomState of the resource set. If this key exists and its
        /// value is a boolean true value, then the QFE way of enabling the metadata key order was used and it overrides
        /// any setting of the public property on the resource set.
        /// </summary>
        private const string UseMetadataKeyOrderDictionaryKey = "UseMetadataKeyOrder";

        /// <summary>
        /// Reference to the wrapped resource set.
        /// </summary>
        private readonly ResourceSet resourceSet;

        /// <summary>
        /// Reference to the resource type this resource set is a collection of.
        /// </summary>
        private ResourceType resourceType;

#if DEBUG
        /// <summary>
        /// Is true, if the resource set is fully initialized and validated. No more changes can be made once its set to readonly.
        /// </summary>
        private bool isReadOnly;
#endif
        #endregion Fields

        #region Constructors
        /// <summary>
        /// Constructs a new <see cref="ResourceSetWrapper"/> instance using the <see cref="ResourceSet"/> instance to be enclosed.
        /// </summary>
        /// <param name="resourceSet"><see cref="ResourceSet"/> instance to be wrapped by the current instance.</param>
        private ResourceSetWrapper(ResourceSet resourceSet)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");
            this.resourceSet = resourceSet;
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Name of the resource set.
        /// </summary>
        internal string Name
        {
            get 
            {
                DebugUtils.CheckNoExternalCallers();
                return this.resourceSet.Name; 
            }
        }

        /// <summary>
        /// Reference to resource type that this resource set is a collection of.
        /// </summary>
        internal ResourceType ResourceType
        {
            get 
            {
                DebugUtils.CheckNoExternalCallers();
                return this.resourceType; 
            }
        }

        /// <summary>
        /// Returns the wrapped resource set instance.
        /// </summary>
        internal ResourceSet ResourceSet
        {
            [DebuggerStepThrough]
            get
            {
                DebugUtils.CheckNoExternalCallers();
#if DEBUG
                Debug.Assert(this.resourceSet != null, "this.resourceSet != null");
#endif
                return this.resourceSet;
            }
        }

        /// <summary>
        /// Is true, if key properties should be ordered as per declared order when used for constructing OrderBy queries.
        /// Otherwise the default alphabetical order is used.
        /// </summary>
        internal bool UseMetadataKeyOrder
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
#if DEBUG
                Debug.Assert(this.isReadOnly, "UseMetadataKeyOrder - entity set settings not initialized.");
#endif

                // First check if the QFE way of enabling this is used
                Dictionary<string, object> dictionary = this.resourceSet.CustomState as Dictionary<string, object>;
                object useMetadataKeyPropertyValue;
                if (dictionary != null && dictionary.TryGetValue(UseMetadataKeyOrderDictionaryKey, out useMetadataKeyPropertyValue))
                {
                    if ((useMetadataKeyPropertyValue is bool) && ((bool)useMetadataKeyPropertyValue))
                    {
                        return true;
                    }
                }

                // Otherwise use the public property on the resource set
                return this.resourceSet.UseMetadataKeyOrder;
            }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Creates the wrapper from the given resource set.
        /// </summary>
        /// <param name="resourceSet">Resource set instance whose wrapper needs to get created.</param>
        /// <param name="resourceTypeValidator">Resource type validator.</param>
        /// <returns>Wrapper for the given resource set.</returns>
        internal static ResourceSetWrapper CreateResourceSetWrapper(
            ResourceSet resourceSet, 
            Func<ResourceType, ResourceType> resourceTypeValidator)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(resourceSet != null, "resourceSet != null");
            Debug.Assert(resourceSet.IsReadOnly, "The resourceSet must be read-only by now.");
            Debug.Assert(resourceTypeValidator != null, "resourceTypeValidator != null");

            ResourceSetWrapper resourceSetWrapper = new ResourceSetWrapper(resourceSet);
#if DEBUG
            resourceSetWrapper.isReadOnly = true;
#endif

            resourceSetWrapper.resourceType = resourceTypeValidator(resourceSet.ResourceType);
            return resourceSetWrapper;
        }

        /// <summary>Returns list of key properties ordered as appropriate for construction of OrderBy queries
        /// (for implicit sorting of results).</summary>
        /// <returns>List of key properties ordered either alphabetically or in the declared order depending on the UseMetadataKeyOrder.</returns>
        internal IEnumerable<ResourceProperty> GetKeyPropertiesForOrderBy()
        {
            DebugUtils.CheckNoExternalCallers();
#if DEBUG
            Debug.Assert(this.isReadOnly, "GetKeyPropertiesForOrderBy - entity set settings not initialized.");
#endif

            if (this.UseMetadataKeyOrder)
            {
                // This is not too slow, since Properties returnes a cached list, so we're just filtering it every time
                return this.ResourceType.Properties.Where(resourceProperty => resourceProperty.IsOfKind(ResourcePropertyKind.Key));
            }
            else
            {
                // The KeyProperties property returns the list of key properties sorted by their name alphabetically
                return this.ResourceType.KeyProperties;
            }
        }
        #endregion Methods
    }
}

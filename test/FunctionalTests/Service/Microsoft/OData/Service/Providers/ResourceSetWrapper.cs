//---------------------------------------------------------------------
// <copyright file="ResourceSetWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    #endregion Namespaces

    /// <summary>
    /// Wrapper class for a resource set.  A resource set object can be shared across services,
    /// this wrapper class contains the resouce set information and also service specific
    /// information about that resource set.
    /// </summary>
    [DebuggerDisplay("{Name}: {ResourceType}")]
    internal class ResourceSetWrapper
    {
        #region Fields

        /// <summary>The string used as a key to a dictionary in CustomState of the resource set. If this key exists and its
        /// value is a boolean true value, then the QFE way of enabling the metadata key order was used and it overrides
        /// any setting of the public property on the resource set.</summary>
        private const string UseMetadataKeyOrderDictionaryKey = "UseMetadataKeyOrder";

        /// <summary>Reference to the wrapped resource set</summary>
        private readonly ResourceSet resourceSet;

        /// <summary>
        /// Caches for all the visible properties for the given resource type.
        /// </summary>
        private readonly Dictionary<ResourceType, ResourcePropertyCache> resourcePropertyCache;

        /// <summary>Reference to the wrapped resource type.</summary>
        private ResourceType resourceType;

        /// <summary>Access rights to this resource set.</summary>
        private EntitySetRights rights;

        /// <summary>Page Size for this resource set.</summary>
        private int pageSize;

        /// <summary>Methods to be called when composing read queries to allow authorization.</summary>
        private MethodInfo[] readAuthorizationMethods;

        /// <summary>Methods to be called when validating write methods to allow authorization.</summary>
        private MethodInfo[] writeAuthorizationMethods;

        /// <summary>Whether the types contained in the set have any navigation property.</summary>
        private bool hasAccessibleNavigationProperty;

        /// <summary>Whether the set has open types.</summary>
        private bool hasOpenTypes;

        /// <summary>Whether the set type has derived types.</summary>
        private bool hasDerivedTypes;

#if DEBUG
        /// <summary>Is true, if the resource set is fully initialized and validated. No more changes can be made once its set to readonly.</summary>
        private bool isReadOnly;
#endif

        /// <summary>Metadata version of the resourceSet.</summary>
        private Version metadataVersion;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Constructs a new ResourceSetWrapper instance using the ResourceSet instance to be enclosed.
        /// </summary>
        /// <param name="resourceSet">ResourceSet instance to be wrapped by the current instance</param>
        private ResourceSetWrapper(ResourceSet resourceSet)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");
            
            if (!resourceSet.IsReadOnly)
            {
                throw new DataServiceException(500, Strings.DataServiceProviderWrapper_ResourceContainerNotReadonly(resourceSet.Name));
            }

            this.resourceSet = resourceSet;
            this.resourcePropertyCache = new Dictionary<ResourceType, ResourcePropertyCache>(ReferenceEqualityComparer<ResourceType>.Instance);
        }

        #endregion Constructors

        #region Properties

        /// <summary>Name of the resource set.</summary>
        public string Name
        {
            get { return this.resourceSet.Name; }
        }

        /// <summary> Reference to resource type that this resource set is a collection of </summary>
        public ResourceType ResourceType
        {
            get { return this.resourceType; }
        }

        /// <summary>Whether the resource set is visible to service consumers.</summary>
        public bool IsVisible
        {
            get
            {
#if DEBUG
                Debug.Assert(this.isReadOnly, "IsVisible - entity set settings not initialized.");
#endif
                return this.rights != EntitySetRights.None;
            }
        }

        /// <summary>Access rights to this resource set.</summary>
        public EntitySetRights Rights
        {
            get
            {
#if DEBUG
                Debug.Assert(this.isReadOnly, "Rights - entity set settings not initialized.");
#endif
                return this.rights;
            }
        }

        /// <summary>Page Size for this resource set.</summary>
        public int PageSize
        {
            get
            {
#if DEBUG
                Debug.Assert(this.isReadOnly, "Rights - entity set settings not initialized.");
#endif
                return this.pageSize;
            }
        }

        /// <summary>Retursn the list of query interceptors for this set (possibly null).</summary>
        public MethodInfo[] QueryInterceptors
        {
            [DebuggerStepThrough]
            get
            {
#if DEBUG
                Debug.Assert(this.isReadOnly, "QueryInterceptors - entity set settings not initialized.");
#endif
                return this.readAuthorizationMethods;
            }
        }

        /// <summary>Returns the list of change interceptors for this set (possible null).</summary>
        public MethodInfo[] ChangeInterceptors
        {
            [DebuggerStepThrough]
            get
            {
#if DEBUG
                Debug.Assert(this.isReadOnly, "ChangeInterceptors - entity set settings not initialized.");
#endif
                return this.writeAuthorizationMethods;
            }
        }

        /// <summary>Returns the wrapped resource set instance.</summary>
        internal ResourceSet ResourceSet
        {
            [DebuggerStepThrough]
            get
            {
#if DEBUG
                Debug.Assert(this.resourceSet != null, "this.resourceSet != null");
#endif
                return this.resourceSet;
            }
        }

        /// <summary>Type of the query root for the set.</summary>
        internal Type QueryRootType
        {
            get
            {
                return this.resourceSet.QueryRootType;
            }
        }

        /// <summary>Is true, if key properties should be ordered as per declared order when used for constructing OrderBy queries.
        /// Otherwise the default alphabetical order is used.</summary>
        internal bool UseMetadataKeyOrder
        {
            get
            {
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

         /// <summary>
        /// Name of the entity container to which the set belongs to.
        /// </summary>
        internal string EntityContainerName
        {
            get { return this.resourceSet.EntityContainerName; }
        }

        /// <summary>
        /// Returns the list of custom annotations defined on this set.
        /// </summary>
        internal IEnumerable<KeyValuePair<string, object>> CustomAnnotations
        {
            get { return this.resourceSet.CustomAnnotations; }
        }

        #endregion Properties

        #region Methods

         /// <summary>
        /// Creates the wrapper from the given resource set for use in unit tests.
        /// </summary>
        /// <param name="resourceSet">resource set instance whose wrapper needs to get created.</param>
        /// <param name="rights">Optional rights for the set. Defaults to None.</param>
        /// <returns>Wrapper for the given resource set.</returns>
        internal static ResourceSetWrapper CreateForTests(ResourceSet resourceSet, EntitySetRights rights = EntitySetRights.None)
        {
             var resourceSetWrapper = new ResourceSetWrapper(resourceSet) 
             { 
                 rights = rights,
#if DEBUG
                 isReadOnly = true
#endif
             };

             return resourceSetWrapper;
        }

        /// <summary>
        /// Creates the wrapper from the given resource set. This method returns null, if the given resource set is not visible.
        /// It also checks for the resource set metadata to make sure that the MPV in the configuration is set correctly
        /// </summary>
        /// <param name="resourceSet">resource set instance whose wrapper needs to get created.</param>
        /// <param name="provider">DataServiceProviderWrapper instance.</param>
        /// <param name="resourceTypeValidator">resource type validator.</param>
        /// <returns>Wrapper for the given resource set, if the resource set/resource type metadata is valid and adheres to the protocol version in the server.</returns>
        internal static ResourceSetWrapper CreateResourceSetWrapper(ResourceSet resourceSet, DataServiceProviderWrapper provider, Func<ResourceType, ResourceType> resourceTypeValidator)
        {
            ResourceSetWrapper resourceSetWrapper = new ResourceSetWrapper(resourceSet);
            resourceSetWrapper.ApplyConfiguration(provider.Configuration, provider.StaticConfiguration);
            if (!resourceSetWrapper.IsVisible)
            {
                return null;
            }

            // Only validate the resource type if the set is visible.
            resourceSetWrapper.resourceType = resourceTypeValidator(resourceSet.ResourceType);
            return resourceSetWrapper;
        }

        /// <summary>
        /// Determines whether the element type of the resource set has any derived types.
        /// </summary>
        /// <param name="provider">Data service provider instance.</param>
        /// <returns>true if the resource set type has any derived types.</returns>
        internal bool HasDerivedTypes(DataServiceProviderWrapper provider)
        {
            this.CheckHierarchy(provider);
            return this.hasDerivedTypes;
        }

        /// <summary>Whether the types contained in the set have any navigation property or not</summary>
        /// <param name="provider">Data service provider instance.</param>
        /// <returns>True if there's any type in this set which has named streams. False otherwise.</returns>
        internal bool HasAccessibleNavigationProperty(DataServiceProviderWrapper provider)
        {
            Debug.Assert(this.resourceSet != null, "this.resourceSet != null");
            this.CheckHierarchy(provider);
            return this.hasAccessibleNavigationProperty;
        }

        /// <summary>
        /// Determines the minimum payload version that can be used for the set.
        /// </summary>
        /// <param name="service">The data service instance</param>
        /// <returns>The minimum version that can be used for a payload for or from this set.</returns>
        internal Version MinimumResponsePayloadVersion(IDataService service)
        {
            Version minimumVersion = VersionUtil.Version4Dot0;

            // Call this.CheckHierarchy(provider) method, in case its already not called
            // (when ShouldIncludeAssociationLinksInResponse is false)
            this.CheckHierarchy(service.Provider);
            Debug.Assert(this.metadataVersion != null, "this.version != null");

            // If target set contains collection properties we need v4.0
            // If target type contains named streams, we need v4.0
            minimumVersion = VersionUtil.RaiseVersion(minimumVersion, this.metadataVersion);

            // If the resource type is an open type, then we do not know the metadata of the open property and hence cannot
            // predict the response version. Hence we need to bump the version to the maximum possible version that the server
            // can write and client can understand (i.e. min of MPV in the server and request MaxDSV).
            // If we encounter during serialization, anything greater than the computed response version, we will throw instream error.
            if (this.hasOpenTypes)
            {
                Version maxProtocolVersion = service.Configuration.DataServiceBehavior.MaxProtocolVersion.ToVersion();
                Version requestMaxVersion = service.OperationContext.RequestMessage.RequestMaxVersion;
                Version responseVersion = (requestMaxVersion < maxProtocolVersion) ? requestMaxVersion : maxProtocolVersion;
                minimumVersion = VersionUtil.RaiseVersion(minimumVersion, responseVersion);
            }

            return minimumVersion;
        }

        /// <summary>Returns list of key properties ordered as appropriate for construction of OrderBy queries
        /// (for implicit sorting of results).</summary>
        /// <returns>List of key properties ordered either alphabetically or in the declared order depending on the UseMetadataKeyOrder.</returns>
        internal IEnumerable<ResourceProperty> GetKeyPropertiesForOrderBy()
        {
            // This is not too slow, since Properties returnes a cached list, so we're just filtering it every time
            // The KeyProperties property returns the list of key properties sorted by their name alphabetically
            return this.UseMetadataKeyOrder 
                ? this.ResourceType.Properties.Where(resourceProperty => resourceProperty.IsOfKind(ResourcePropertyKind.Key)) 
                : this.ResourceType.KeyProperties;
        }

        /// <summary>
        /// Gets the visible resource properties for <paramref name="entityType"/> for this set.
        /// We cache the list of visible resource properties so we don't have to calculate it repeatedly when serializing feeds.
        /// </summary>
        /// <param name="provider">Data service provider instance.</param>
        /// <param name="entityType">Resource type in question.</param>
        /// <returns>List of visible resource properties for the resource type.</returns>
        internal IEnumerable<ResourceProperty> GetEntitySerializableProperties(DataServiceProviderWrapper provider, ResourceType entityType)
        {
            Debug.Assert(entityType != null && entityType.ResourceTypeKind == ResourceTypeKind.EntityType, "entityType != null && entityType.ResourceTypeKind == ResourceTypeKind.EntityType");

            ResourcePropertyCache cachedResourceProperties = this.InitializeResourcePropertyCache(provider, entityType);
            return cachedResourceProperties.Properties;
        }

        /// <summary>
        /// Gets the visible resource properties declared on the <paramref name="entityType"/> for this set.
        /// We cache the list of visible resource properties so we don't have to calculate it repeatedly when serializing feeds.
        /// </summary>
        /// <param name="provider">Data service provider instance.</param>
        /// <param name="entityType">Resource type in question.</param>
        /// <returns>List of visible resource properties declared on the resource type.</returns>
        private IEnumerable<ResourceProperty> GetEntitySerializablePropertiesDeclaredOnTheResourceType(DataServiceProviderWrapper provider, ResourceType entityType)
        {
            Debug.Assert(entityType != null && entityType.ResourceTypeKind == ResourceTypeKind.EntityType, "entityType != null && entityType.ResourceTypeKind == ResourceTypeKind.EntityType");

            ResourcePropertyCache cachedResourceProperties = this.InitializeResourcePropertyCache(provider, entityType);
            return cachedResourceProperties.PropertiesDeclaredOnTheType;
        }

        /// <summary>Whether the types contained in the set have named streams or not</summary>
        /// <param name="provider">Data service provider instance.</param>
        private void CheckHierarchy(DataServiceProviderWrapper provider)
        {
#if DEBUG
            Debug.Assert(provider != null, "provider != null");
            Debug.Assert(this.resourceSet != null, "this.resourceSet != null");
            Debug.Assert(this.isReadOnly, "this.isReadOnly - entity set settings not initialized.");
#endif

            // Go through all types contained in the set.
            //  Find the maximum of minimum DSPVs required for resource types.
            //  If any one type has collection properties, the whole set has them.
            //  If any one type has named streams, the whole set has them.
            ResourceType baseType = this.resourceSet.ResourceType;
            bool hasNavigationPropertyOrNamedStreamOnDerivedType = false;
            bool hasNavigationProperty = this.GetEntitySerializableProperties(provider, baseType).Any(p => p.TypeKind == ResourceTypeKind.EntityType);
            bool hasOpenType = baseType.IsOpenType;
            this.hasDerivedTypes = provider.HasDerivedTypes(baseType);
            Version resourceSetVersion = baseType.MetadataVersion;

            // If we have all the V3 features or no derived types then we do not need to look any further
            if (this.hasDerivedTypes)
            {
                foreach (ResourceType derivedType in provider.GetDerivedTypes(baseType))
                {
                    bool hasNavigationPropertyOnDerivedType = this.GetEntitySerializablePropertiesDeclaredOnTheResourceType(provider, derivedType).Any(p => p.TypeKind == ResourceTypeKind.EntityType);
                    hasNavigationProperty |= hasNavigationPropertyOnDerivedType;
                    hasNavigationPropertyOrNamedStreamOnDerivedType |= derivedType.HasNamedStreamsDeclaredOnThisType | hasNavigationPropertyOnDerivedType;

                    resourceSetVersion = VersionUtil.RaiseVersion(resourceSetVersion, derivedType.MetadataVersion);

                    hasOpenType |= derivedType.IsOpenType;

                    // If we have all the V3(latest) features we do not need to look any further
                    if (resourceSetVersion == VersionUtil.Version4Dot0 &&
                        hasNavigationPropertyOrNamedStreamOnDerivedType &&
                        hasOpenType)
                    {
                        break;
                    }
                }
            }

            this.hasAccessibleNavigationProperty = hasNavigationProperty;
            this.hasOpenTypes = hasOpenType;
            this.metadataVersion = resourceSetVersion;
        }

        /// <summary>
        /// Apply the given configuration to the resource set.
        /// </summary>
        /// <param name="configuration">data service configuration instance.</param>
        /// <param name="staticConfiguration">Data service static configuration.</param>
        private void ApplyConfiguration(DataServiceConfiguration configuration, DataServiceStaticConfiguration staticConfiguration)
        {
#if DEBUG
            Debug.Assert(!this.isReadOnly, "Can only apply the configuration once.");
#endif

            // Set entity set rights
            this.rights = configuration.GetResourceSetRights(this.resourceSet);

            // Set page size
            this.pageSize = configuration.GetResourceSetPageSize(this.resourceSet);
            if (this.pageSize < 0)
            {
                throw new DataServiceException(500, Strings.DataService_SDP_PageSizeMustbeNonNegative(this.pageSize, this.Name));
            }

            // Add QueryInterceptors
            this.readAuthorizationMethods = staticConfiguration.GetReadAuthorizationMethods(this.resourceSet);

            // Add ChangeInterceptors
            this.writeAuthorizationMethods = staticConfiguration.GetWriteAuthorizationMethods(this.resourceSet);

#if DEBUG
            this.isReadOnly = true;
#endif
        }

        /// <summary>
        /// Checks if the cache is populated, otherwise populates it.
        /// </summary>
        /// <param name="provider">Data service provider instance.</param>
        /// <param name="type">Resource type in question.</param>
        /// <returns>An instance of ResourcePropertyCache, with all information about the properties cached.</returns>
        private ResourcePropertyCache InitializeResourcePropertyCache(DataServiceProviderWrapper provider, ResourceType type)
        {
            Debug.Assert(provider != null, "provider != null");
            Debug.Assert(type != null, "resourceType != null");
            Debug.Assert(this.ResourceType.IsAssignableFrom(type), "this.ResourceType.IsAssignableFrom(resourceType)");

            ResourcePropertyCache propertyCache;
            if (!this.resourcePropertyCache.TryGetValue(type, out propertyCache))
            {
                propertyCache = new ResourcePropertyCache();
                propertyCache.Properties = new List<ResourceProperty>();
                foreach (ResourceProperty property in type.Properties)
                {
                    if (property.TypeKind == ResourceTypeKind.EntityType && provider.GetResourceSet(this, type, property) == null)
                    {
                        // non-visible nav properties
                        continue;
                    }

                    propertyCache.Properties.Add(property);
                }

                propertyCache.PropertiesDeclaredOnTheType = new List<ResourceProperty>();
                foreach (ResourceProperty property in type.PropertiesDeclaredOnThisType)
                {
                    if (property.TypeKind == ResourceTypeKind.EntityType && provider.GetResourceSet(this, type, property) == null)
                    {
                        // non-visible nav properties
                        continue;
                    }

                    propertyCache.PropertiesDeclaredOnTheType.Add(property);
                }

                this.resourcePropertyCache.Add(type, propertyCache);
            }

            return propertyCache;
        }
        #endregion Methods

        /// <summary>
        /// Class to cache all the visible properties of a resource type.
        /// </summary>
        private class ResourcePropertyCache
        {
            /// <summary>List of all visible properties for a resource type.</summary>
            public List<ResourceProperty> Properties { get; set; }

            /// <summary>List of all visible properties declared on a resource type.</summary>
            public List<ResourceProperty> PropertiesDeclaredOnTheType { get; set; }
        }
    }
}

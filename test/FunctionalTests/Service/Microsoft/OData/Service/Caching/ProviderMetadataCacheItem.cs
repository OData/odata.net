//---------------------------------------------------------------------
// <copyright file="ProviderMetadataCacheItem.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using Microsoft.OData.Service.Providers;

    /// <summary>Use this class to cache metadata for providers.</summary>
    internal class ProviderMetadataCacheItem
    {
        #region Private fields

        /// <summary> list of top level entity sets</summary>
        private readonly Dictionary<string, ResourceSet> entitySets;

        /// <summary>Collection of service operations, keyed by name.</summary>
        private readonly Dictionary<string, ServiceOperation> serviceOperations;

        /// <summary>Target type for the data provider.</summary>
        private readonly Type type;

        /// <summary>Cache of resource properties per type.</summary>
        private readonly Dictionary<Type, ResourceTypeCacheItem> typeCache;

        /// <summary>Cache of immediate derived types per type.</summary>
        private readonly Dictionary<ResourceType, List<ResourceType>> childTypesCache;

        /// <summary>Cache of IL's instructions for getting the query root for sets.</summary>
        private readonly Dictionary<ResourceSet, Func<object, IQueryable>> queryRootCache;

        /// <summary>
        /// Record the mapping betoween the source rouceproperty and selected target resourceproperty 
        /// </summary>
        private readonly Dictionary<string, ResourceProperty> targetResourcePropertiesCacheItems;

        #endregion Private fields

        /// <summary>Initializes a new <see cref="ProviderMetadataCacheItem"/> instance.</summary>
        /// <param name='type'>Type of data context for which metadata will be generated.</param>
        internal ProviderMetadataCacheItem(Type type)
        {
            Debug.Assert(type != null, "type != null");

            this.serviceOperations = new Dictionary<string, ServiceOperation>(EqualityComparer<string>.Default);
            this.typeCache = new Dictionary<Type, ResourceTypeCacheItem>(EqualityComparer<Type>.Default);
            this.entitySets = new Dictionary<string, ResourceSet>(EqualityComparer<string>.Default);
            this.childTypesCache = new Dictionary<ResourceType, List<ResourceType>>(ReferenceEqualityComparer<ResourceType>.Instance);
            this.queryRootCache = new Dictionary<ResourceSet, Func<object, IQueryable>>(ReferenceEqualityComparer<ResourceSet>.Instance);
            this.targetResourcePropertiesCacheItems = new Dictionary<string, ResourceProperty>(EqualityComparer<string>.Default);
            this.type = type;
        }

        #region Properties

        /// <summary>Collection of service operations, keyed by name.</summary>
        internal Dictionary<string, ServiceOperation> ServiceOperations
        {
            [DebuggerStepThrough]
            get { return this.serviceOperations; }
        }

        /// <summary>Cache of ResourceTypeCacheItems which contains the ResourceType and its metadata.</summary>
        internal IEnumerable<ResourceTypeCacheItem> ResourceTypeCacheItems
        {
            [DebuggerStepThrough]
            get { return this.typeCache.Values; }
        }

        /// <summary>Cache of ResourceTypeCacheItems which contains the ResourceType and its metadata.</summary>
        internal Dictionary<string, ResourceProperty> TargetResourcePropertiesCacheItems
        {
            [DebuggerStepThrough]
            get { return this.targetResourcePropertiesCacheItems; }
        }

        /// <summary>Cache of immediate derived types per type.</summary>
        internal Dictionary<ResourceType, List<ResourceType>> ChildTypesCache
        {
            [DebuggerStepThrough]
            get { return this.childTypesCache; }
        }

        /// <summary> list of top level entity sets</summary>
        internal Dictionary<string, ResourceSet> EntitySets
        {
            [DebuggerStepThrough]
            get { return this.entitySets; }
        }

        /// <summary>Target type for the data provider.</summary>
        internal Type Type
        {
            [DebuggerStepThrough]
            get { return this.type; }
        }

        /// <summary>Returns the cache of IL's instructions for getting the query root for sets.</summary>
        internal Dictionary<ResourceSet, Func<object, IQueryable>> QueryRootCache
        {
            [DebuggerStepThrough]
            get { return this.queryRootCache; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the ResourceType for the given CLR type.
        /// </summary>
        /// <param name="type">CLR type.</param>
        /// <returns>ResourceType instance for the given CLR type.</returns>
        internal ResourceType TryGetResourceType(Type type)
        {
            var resourceTypeCacheItem = this.TryGetResourceTypeCacheItem(type);
            return resourceTypeCacheItem != null ? resourceTypeCacheItem.ResourceType : null;
        }

        /// <summary>
        /// Gets the ResourceType for the given CLR type.
        /// </summary>
        /// <param name="type">CLR type.</param>
        /// <returns>ResourceType instance for the given CLR type.</returns>
        internal ResourceTypeCacheItem TryGetResourceTypeCacheItem(Type type)
        {
            ResourceTypeCacheItem resourceTypeCacheItem;
            this.typeCache.TryGetValue(type, out resourceTypeCacheItem);
            return resourceTypeCacheItem;
        }

        /// <summary>
        /// Adds the given ResourceType to the cache.
        /// </summary>
        /// <param name="type">CLR type.</param>
        /// <param name="resourceType">ResourceType instance.</param>
        internal void AddResourceType(Type type, ResourceType resourceType)
        {
            var cacheItem = new ResourceTypeCacheItem(resourceType);
            this.typeCache.Add(type, cacheItem);
        }

        #endregion Methods
    }

    /// <summary>
    /// Class to cache information for the given resource type.
    /// </summary>
    internal class ResourceTypeCacheItem
    {
        /// <summary>ResourceType instance for which the metadata needs to be cached.</summary>
        private readonly ResourceType resourceType;

        /// <summary>Cache for storing the metadata about the property.</summary>
        private readonly Dictionary<ResourceProperty, ResourcePropertyCacheItem> resourcePropertyMetadataCache = new Dictionary<ResourceProperty, ResourcePropertyCacheItem>(ReferenceEqualityComparer<ResourceProperty>.Instance);

        /// <summary>Constructor Delegate for the resource type.</summary>
        private Func<object> constructorDelegate;

        /// <summary>
        /// Creates a new instance of ResourceTypeCacheItem.
        /// </summary>
        /// <param name="resourceType">ResourceType instance.</param>
        public ResourceTypeCacheItem(ResourceType resourceType)
        {
            Debug.Assert(resourceType != null, "resourceType != null");
            this.resourceType = resourceType;
        }

        /// <summary>Cached delegate to create a new instance of this type.</summary>
        internal Func<object> ConstructorDelegate
        {
            get
            {
                Debug.Assert(
                    this.resourceType.ResourceTypeKind != ResourceTypeKind.Primitive && this.resourceType.ResourceTypeKind != ResourceTypeKind.Collection && this.resourceType.ResourceTypeKind != ResourceTypeKind.EntityCollection,
                    "Constructor delegate should only be accessed on types which are not primitives or collections.");

                if (this.constructorDelegate == null)
                {
                    this.constructorDelegate = (Func<object>)
                        WebUtil.CreateNewInstanceConstructor(this.resourceType.InstanceType, this.resourceType.FullName, typeof(object));
                }

                return this.constructorDelegate;
            }
        }

        /// <summary>Gets the instance of ResourceType whose metadata is getting cached in this cache item.</summary>
        internal ResourceType ResourceType
        {
            get { return this.resourceType; }
        }

        /// <summary>
        /// Gets the cache item for the given property.
        /// </summary>
        /// <param name="property">ResourceProperty instance.</param>
        /// <returns>the cache item for the given property.</returns>
        internal ResourcePropertyCacheItem GetResourcePropertyCacheItem(ResourceProperty property)
        {
            ResourcePropertyCacheItem resourcePropertyCacheItem;
            this.resourcePropertyMetadataCache.TryGetValue(property, out resourcePropertyCacheItem);
            if (resourcePropertyCacheItem == null)
            {
                throw new DataServiceException((int)HttpStatusCode.InternalServerError, Strings.ObjectContext_PublicPropertyNotDefinedOnType(this.resourceType.FullName, property.Name));
            }

            return resourcePropertyCacheItem;
        }

        /// <summary>
        /// Add the given property metadata to the cache.
        /// </summary>
        /// <param name="property">ResourceProperty instance.</param>
        /// <param name="propertyCacheItem">Cache item containing the metadata about the property.</param>
        internal void AddResourcePropertyCacheItem(ResourceProperty property, ResourcePropertyCacheItem propertyCacheItem)
        {
            this.resourcePropertyMetadataCache.Add(property, propertyCacheItem);
        }
    }

    /// <summary>
    /// Class for storing metadata for a given ResourceProperty.
    /// </summary>
    internal class ResourcePropertyCacheItem
    {
        /// <summary>PropertyInfo instance for the given ResourceProperty.</summary>
        private readonly PropertyInfo propertyInfo;

        /// <summary>
        /// Creates a new instance of ResourcePropertyCacheItem.
        /// </summary>
        /// <param name="propertyInfo">PropertyInfo instance for the given ResourceProperty.</param>
        internal ResourcePropertyCacheItem(PropertyInfo propertyInfo)
        {
            Debug.Assert(propertyInfo != null, "propertyInfo != null");
            this.propertyInfo = propertyInfo;
        }

        /// <summary>
        /// Returns PropertyInfo instance for the given ResourceProperty.
        /// </summary>
        public PropertyInfo PropertyInfo
        {
            get { return this.propertyInfo; }
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="BaseServiceProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Microsoft.OData.Service.Caching;
    using Microsoft.OData.Client;


    #endregion Namespaces

    /// <summary>Provides a reflection-based provider implementation.</summary>
    internal abstract class BaseServiceProvider : 
        IDataServiceMetadataProvider, 
        IDataServiceQueryProvider, 
#if !EF6Provider
        IDataServiceInternalProvider,
#endif
        IDataServiceProviderBehavior, 
        IDisposable
    {
        /// <summary>Bindings Flags to be used for reflection.</summary>
        protected const BindingFlags ResourceContainerBindingFlags = WebUtil.PublicInstanceBindingFlags;

        /// <summary>instance of the service to invoke service operations.</summary>
        private readonly object dataServiceInstance;

        /// <summary>Instance from which data is provided.</summary>
        private object dataSourceInstance;

        /// <summary>Metadata to be used by the service provider.</summary>
        private ProviderMetadataCacheItem metadata;

        /// <summary>Internal providers need to know if the metadata was loaded from cache or initialized.
        /// Once we make the providers completely public, we can get rid of this.</summary>
        private bool metadataRequiresInitialization;

        /// <summary>
        /// Initializes a new Microsoft.OData.Service.BaseServiceProvider instance.
        /// </summary>
        /// <param name="dataServiceInstance">data service instance.</param>
        /// <param name="dataSourceInstance">data source instance.</param>
        protected BaseServiceProvider(object dataServiceInstance, object dataSourceInstance)
        {
            WebUtil.CheckArgumentNull(dataServiceInstance, "dataServiceInstance");
            WebUtil.CheckArgumentNull(dataSourceInstance, "dataSourceInstance");

            this.dataServiceInstance = dataServiceInstance;
            this.dataSourceInstance = dataSourceInstance;
        }

        #region IDataServiceQueryProvider Properties

        /// <summary>Returns the instance from which data is provided.</summary>
        public virtual object CurrentDataSource
        {
            [DebuggerStepThrough]
            get
            {
                // Many debuggers will try to display this property, and we don't want to trigger an assertion.
                Debug.Assert(
                    System.Diagnostics.Debugger.IsAttached || this.dataSourceInstance != null,
                    "this.instance != null -- otherwise CurrentDataSource is accessed before initialization or after disposal.");
                return this.dataSourceInstance;
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>Gets a value indicating whether null propagation is required in expression trees.</summary>
        public abstract bool IsNullPropagationRequired
        {
            get;
        }

        #endregion IDataServiceQueryProvider Properties

        #region IDataServiceMetadataProvider Properties

        /// <summary>Namespace name for the EDM container.</summary>
        public abstract string ContainerNamespace
        {
            get;
        }

        /// <summary>Name of the EDM container</summary>
        public abstract string ContainerName
        {
            get;
        }

        /// <summary>Gets all available containers.</summary>
        /// <returns>An enumerable object with all available containers.</returns>
        public virtual IEnumerable<ResourceSet> ResourceSets
        {
            get { return this.metadata.EntitySets.Values; }
        }

        /// <summary>Returns all the types in this data source</summary>
        public virtual IEnumerable<ResourceType> Types
        {
            get { return this.metadata.ResourceTypeCacheItems.Select(c => c.ResourceType); }
        }

        /// <summary>Returns all known service operations.</summary>
        public virtual IEnumerable<ServiceOperation> ServiceOperations
        {
            get
            {
                foreach (ServiceOperation serviceOperation in this.metadata.ServiceOperations.Values)
                {
                    yield return serviceOperation;
                }
            }
        }

        #endregion IDataServiceMetadataProvider Properties

        #region IDataServiceProviderBehavior

        /// <summary>
        /// Instance of provider behavior that defines the assumptions service should make
        /// about the provider.
        /// </summary>
        public abstract ProviderBehavior ProviderBehavior { get; }

        #endregion IDataServiceProviderBehavior

        #region Protected Properties

        /// <summary>
        /// Gets the MetadataCacheItem containing all the cached metadata.
        /// </summary>
        protected ProviderMetadataCacheItem MetadataCacheItem
        {
            get { return this.metadata; }
        }

        #endregion Protected Properties

        #region Private Members

        /// <summary>Cache of immediate derived types per type.</summary>
        private Dictionary<ResourceType, List<ResourceType>> ChildTypesCache
        {
            [DebuggerStepThrough]
            get { return this.metadata.ChildTypesCache; }
        }

        /// <summary>Target type for the data provider </summary>
        private Type DataSourceType
        {
            [DebuggerStepThrough]
            get { return this.dataSourceInstance.GetType(); }
        }
        #endregion

        #region Public Methods

        /// <summary>Releases the current data source object as necessary.</summary>
        void IDisposable.Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion Public Methods

        #region IDataServiceQueryProvider Methods

        /// <summary>
        /// Returns the IQueryable that represents the container.
        /// </summary>
        /// <param name="container">resource set representing the entity set.</param>
        /// <returns>
        /// An IQueryable that represents the container; null if there is 
        /// no container for the specified name.
        /// </returns>
        public abstract IQueryable GetQueryRootForResourceSet(ResourceSet container);

        /// <summary>Gets the <see cref="ResourceType"/> for the specified <paramref name="resource"/>.</summary>
        /// <param name="resource">Instance to extract a <see cref="ResourceType"/> from.</param>
        /// <returns>The <see cref="ResourceType"/> that describes this <paramref name="resource"/> in this provider.</returns>
        public virtual ResourceType GetResourceType(object resource)
        {
            WebUtil.CheckArgumentNull(resource, "resource");
            return this.GetNonPrimitiveType(resource.GetType());
        }

        /// <summary>
        /// Get the value of the strongly typed property.
        /// </summary>
        /// <param name="target">instance of the type declaring the property.</param>
        /// <param name="resourceProperty">resource property describing the property.</param>
        /// <returns>value for the property.</returns>
        public virtual object GetPropertyValue(object target, ResourceProperty resourceProperty)
        {
            WebUtil.CheckArgumentNull(target, "target");
            WebUtil.CheckArgumentNull(resourceProperty, "resourceProperty");
            try
            {
                var resourceTypeCacheItem = this.ResolveNonPrimitiveTypeCacheItem(target.GetType());
                PropertyInfo propertyInfo = this.GetResourcePropertyCacheItem(resourceTypeCacheItem, resourceProperty).PropertyInfo;
                Debug.Assert(propertyInfo != null, "propertyInfo != null");
                return propertyInfo.GetGetMethod().Invoke(target, null);
            }
            catch (TargetInvocationException exception)
            {
                ErrorHandler.HandleTargetInvocationException(exception);
                throw;
            }
        }

        /// <summary>
        /// Gets the value of the open property.
        /// </summary>
        /// <param name="target">instance of the resource type.</param>
        /// <param name="propertyName">name of the property.</param>
        /// <returns>the value of the open property. If the property is not present, return null.</returns>
        public abstract object GetOpenPropertyValue(object target, string propertyName);

        /// <summary>
        /// Get the name and values of all the properties defined in the given instance of an open type.
        /// </summary>
        /// <param name="target">instance of a open type.</param>
        /// <returns>collection of name and values of all the open properties.</returns>
        public abstract IEnumerable<KeyValuePair<string, object>> GetOpenPropertyValues(object target);

        /// <summary>
        /// Invoke the given service operation instance.
        /// </summary>
        /// <param name="serviceOperation">metadata for the service operation to invoke.</param>
        /// <param name="parameters">list of parameters to pass to the service operation.</param>
        /// <returns>returns the result by the service operation instance.</returns>
        public virtual object InvokeServiceOperation(ServiceOperation serviceOperation, object[] parameters)
        {
            WebUtil.CheckArgumentNull(serviceOperation, "serviceOperation");

            try
            {
                return ((MethodInfo)serviceOperation.CustomState).Invoke(
                    this.dataServiceInstance,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy,
                    null,
                    parameters,
                    CultureInfo.InvariantCulture);
            }
            catch (TargetInvocationException exception)
            {
                ErrorHandler.HandleTargetInvocationException(exception);
                throw;
            }
        }

        #endregion IDataServiceQueryProvider Methods

        #region IDataServiceMetadataProvider Methods

        /// <summary>
        /// The method must return a collection of all the types derived from <paramref name="resourceType"/>.
        /// The collection returned should NOT include the type passed in as a parameter.
        /// An implementer of the interface should return null if the type does not have any derived types (ie. null == no derived types).
        /// </summary>
        /// <param name="resourceType">Resource to get derived resource types from.</param>
        /// <returns>
        /// A collection of resource types (<see cref="ResourceType"/>) derived from the specified <paramref name="resourceType"/> 
        /// or null if there no types derived from the specified <paramref name="resourceType"/> exist.
        /// </returns>
        public virtual IEnumerable<ResourceType> GetDerivedTypes(ResourceType resourceType)
        {
            WebUtil.CheckArgumentNull(resourceType, "resourceType");
            if (!this.ChildTypesCache.ContainsKey(resourceType))
            {
                throw new InvalidOperationException(Strings.BaseServiceProvider_UnknownResourceTypeInstance(resourceType.FullName));
            }

            List<ResourceType> childTypes = this.ChildTypesCache[resourceType];
            if (childTypes != null)
            {
                foreach (ResourceType childType in childTypes)
                {
                    yield return childType;

                    foreach (ResourceType descendantType in this.GetDerivedTypes(childType))
                    {
                        yield return descendantType;
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if <paramref name="resourceType"/> represents an Entity Type which has derived Entity Types, else false.
        /// </summary>
        /// <param name="resourceType">instance of the resource type in question.</param>
        /// <returns>True if <paramref name="resourceType"/> represents an Entity Type which has derived Entity Types, else false.</returns>
        public virtual bool HasDerivedTypes(ResourceType resourceType)
        {
            WebUtil.CheckArgumentNull(resourceType, "resourceType");
            if (!this.ChildTypesCache.ContainsKey(resourceType))
            {
                throw new InvalidOperationException(Strings.BaseServiceProvider_UnknownResourceTypeInstance(resourceType.FullName));
            }

            Debug.Assert(this.ChildTypesCache[resourceType] == null || this.ChildTypesCache[resourceType].Count > 0, "this.ChildTypesCache[resourceType] == null || this.ChildTypesCache[resourceType].Count > 0");
            return this.ChildTypesCache[resourceType] != null;
        }

        /// <summary>Given the specified name, tries to find a resource set.</summary>
        /// <param name="name">Name of the resource set to resolve.</param>
        /// <param name="resourceSet">Returns the resolved resource set, null if no resource set for the given name was found.</param>
        /// <returns>True if resource set with the given name was found, false otherwise.</returns>
        public virtual bool TryResolveResourceSet(string name, out ResourceSet resourceSet)
        {
            WebUtil.CheckStringArgumentNullOrEmpty(name, "name");
            return this.metadata.EntitySets.TryGetValue(name, out resourceSet);
        }

        /// <summary>Given the specified name, tries to find a service operation.</summary>
        /// <param name="name">Name of the service operation to resolve.</param>
        /// <param name="serviceOperation">Returns the resolved service operation, null if no service operation was found for the given name.</param>
        /// <returns>True if we found the service operation for the given name, false otherwise.</returns>
        public virtual bool TryResolveServiceOperation(string name, out ServiceOperation serviceOperation)
        {
            WebUtil.CheckStringArgumentNullOrEmpty(name, "name");
            return this.metadata.ServiceOperations.TryGetValue(name, out serviceOperation);
        }

        /// <summary>Given the specified name, tries to find a type.</summary>
        /// <param name="name">Name of the type to resolve.</param>
        /// <param name="resourceType">Returns the resolved resource type, null if no resource type for the given name was found.</param>
        /// <returns>True if we found the resource type for the given name, false otherwise.</returns>
        public virtual bool TryResolveResourceType(string name, out ResourceType resourceType)
        {
            WebUtil.CheckStringArgumentNullOrEmpty(name, "name");
            Debug.Assert(this.metadata != null, "this.metadata != null");
            foreach (var cacheItem in this.metadata.ResourceTypeCacheItems)
            {
                if (cacheItem.ResourceType.FullName == name)
                {
                    resourceType = cacheItem.ResourceType;
                    return true;
                }
            }

            resourceType = null;
            return false;
        }

        /// <summary>
        /// Gets the ResourceAssociationSet instance when given the source association end.
        /// </summary>
        /// <param name="resourceSet">Resource set of the source association end.</param>
        /// <param name="resourceType">Resource type of the source association end.</param>
        /// <param name="resourceProperty">Resource property of the source association end.</param>
        /// <returns>ResourceAssociationSet instance.</returns>
        public abstract ResourceAssociationSet GetResourceAssociationSet(ResourceSet resourceSet, ResourceType resourceType, ResourceProperty resourceProperty);

        #endregion IDataServiceMetadataProvider Methods

        #region IDataServiceInternalProvider

        /// <summary>
        /// Called by the service to let the provider perform data model validation.
        /// </summary>
        /// <param name="knownTypes">Collection of known types.</param>
        /// <param name="useMetadataCacheOrder">Whether to use metadata cache ordering instead of default ordering.</param>
        public virtual void FinalizeMetadataModel(IEnumerable<Type> knownTypes, bool useMetadataCacheOrder)
        {
            Debug.Assert(knownTypes != null, "knownTypes != null");

            if (this.metadataRequiresInitialization)
            {
                this.PopulateMetadataForUserSpecifiedTypes(knownTypes, this.metadata);

                if (useMetadataCacheOrder)
                {
                    foreach (ResourceSet resourceSet in this.metadata.EntitySets.Values)
                    {
                        resourceSet.UseMetadataKeyOrder = true;
                    }
                }

                this.CheckModelConsistency();

                this.MakeMetadataReadonly();
            }
        }

        /// <summary>
        /// Return the list of custom annotation for the entity container with the given name.
        /// </summary>
        /// <param name="entityContainerName">Name of the EntityContainer.</param>
        /// <returns>Return the list of custom annotation for the entity container with the given name.</returns>
        public virtual IEnumerable<KeyValuePair<string, object>> GetEntityContainerAnnotations(string entityContainerName)
        {
            return WebUtil.EmptyKeyValuePairStringObject;
        }

        #endregion IDataServiceInternalProvider

        #region Internal Methods

        /// <summary>
        /// Returns the "T" in the IQueryable of T implementation of type.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <param name="typeFilter">filter against which the type is checked</param>
        /// <returns>
        /// The element type for the generic IQueryable interface of the type,
        /// or null if it has none or if it's ambiguous.
        /// </returns>
        internal static Type GetGenericInterfaceElementType(Type type, TypeFilter typeFilter)
        {
            Debug.Assert(type != null, "type != null");
            Debug.Assert(!type.IsGenericTypeDefinition, "!type.IsGenericTypeDefinition");

            if (typeFilter(type, null))
            {
                return type.GetGenericArguments()[0];
            }

            Type[] queriables = type.FindInterfaces(typeFilter, null);
            return queriables != null && queriables.Length == 1 ? queriables[0].GetGenericArguments()[0] : null;
        }

        /// <summary>
        /// Returns the type of the IQueryable if the type implements IQueryable interface
        /// </summary>
        /// <param name="type">clr type on which IQueryable check needs to be performed.</param>
        /// <returns>Element type if the property type implements IQueryable, else returns null</returns>
        internal static Type GetIQueryableElement(Type type)
        {
            return GetGenericInterfaceElementType(type, IQueryableTypeFilter);
        }

        /// <summary>
        /// Returns the type of the IEnumerable if the type implements IEnumerable interface; null otherwise.
        /// </summary>
        /// <param name="type">type that needs to be checked</param>
        /// <returns>Element type if the type implements IEnumerable, else returns null</returns>
        internal static Type GetIEnumerableElement(Type type)
        {
            return GetGenericInterfaceElementType(type, IEnumerableTypeFilter);
        }

        /// <summary>Checks whether the specified method has a SingleResultAttribute declared on it.</summary>
        /// <param name="method">Method to check.</param>
        /// <returns>
        /// true if the specified method (in its declared type or in an 
        /// ancestor declaring the type) has the SingleResultAttribute set.
        /// </returns>
        internal static bool MethodHasSingleResult(MethodInfo method)
        {
            Debug.Assert(method != null, "method != null");
            return method.GetCustomAttributes(typeof(SingleResultAttribute), true).Length > 0;
        }

        /// <summary>
        /// Gets the MIME type declared on the specified <paramref name="member"/>.
        /// </summary>
        /// <param name="member">Member to check.</param>
        /// <returns>
        /// The MIME type declared on the specified <paramref name="member"/>; null
        /// if no attribute is declared.
        /// </returns>
        internal static MimeTypeAttribute GetMimeTypeAttribute(MemberInfo member)
        {
            Debug.Assert(member != null, "member != null");

            return member.ReflectedType.GetCustomAttributes(typeof(MimeTypeAttribute), true)
                .Cast<MimeTypeAttribute>()
                .FirstOrDefault(o => o.MemberName == member.Name);
        }

#if DEBUG
        /// <summary>Assert that we are using the cached version of the provider metadata.</summary>
        internal void AssertUsingCachedProviderMetadata()
        {
            ProviderMetadataCacheItem providerMetadataCacheItem = MetadataCache<ProviderMetadataCacheItem>.TryLookup(this.dataServiceInstance.GetType(), this.dataSourceInstance);
            Debug.Assert(Object.ReferenceEquals(providerMetadataCacheItem, this.metadata), "we are not using the cached provider metadata");
        }
#endif

        /// <summary>
        /// Looks up the metadata in the cache. If not present in the cache, then loads metadata from the provider.
        /// </summary>
        /// <param name="skipServiceOperations">Should service operations be loaded.</param>
        internal void LoadMetadata(bool skipServiceOperations)
        {
            Type dataServiceType = this.dataServiceInstance.GetType();
            Type dataSourceType = this.dataSourceInstance.GetType();

            // If 2 threads enter at the same time, and none of them find the metadata (since its the first time),
            // both of them will load new metadata and try to add them to the cache. But the cache is thread safe, hence
            // before adding, it will check again and return if there is an existing metadata. If it is, we should discard the
            // metadata that just got initialized and use one from the cache.
            this.metadata = MetadataCache<ProviderMetadataCacheItem>.TryLookup(dataServiceType, this.dataSourceInstance);
            if (this.metadata == null)
            {
                this.metadata = new ProviderMetadataCacheItem(dataSourceType);

                // Populate metadata in provider.
                this.PopulateMetadata(this.metadata);

                // Populate service operations only on-demand.
                if (!skipServiceOperations)
                {
                    this.LoadServiceOperations();
                }

                this.metadataRequiresInitialization = true;

                // no need to add metadata yet in the cache, since there might be some encountered while applying
                // configuration or while making it read-only.
            }
        }

        #endregion Internal Methods

        #region Protected methods.

        /// <summary>
        /// Find the corresponding ResourceType for a given Type, primitive or not
        /// </summary>
        /// <param name="metadataCacheItem">Instance of ProviderMetadataCacheItem.</param>
        /// <param name="type">Type to look for</param>
        /// <param name="resourceType">Corresponding ResourceType, if found</param>
        /// <returns>True if type found, false otherwise</returns>
        protected static bool TryGetType(ProviderMetadataCacheItem metadataCacheItem, Type type, out ResourceType resourceType)
        {
            Debug.Assert(metadataCacheItem != null, "metadataCacheItem != null");
            Debug.Assert(type != null, "type != null");

            resourceType = PrimitiveResourceTypeMap.TypeMap.GetPrimitive(type);

            if (resourceType == null)
            {
                resourceType = metadataCacheItem.TryGetResourceType(type);
            }

            return resourceType != null;
        }

        /// <summary>
        /// Add stream properties that are marked NamedStream in clrType to resourceType.
        /// </summary>
        /// <param name="resourceType">the given resource type.</param>
        /// <param name="clrType">backing clr type for the resource.</param>
        /// <param name="inherit">indicates if the resource type has a base type.</param>
        protected static void AddStreamProperties(ResourceType resourceType, Type clrType, bool inherit)
        {
            Debug.Assert(resourceType != null, "resourceType != null");
            Debug.Assert(clrType != null, "clrType != null");

            // Add named streams if there is any.
            // Note Named streams are like virtual properties and each ResourceType will inherit named streams from its parent type.
            // That's why we set inherit to 'false' when the base entity type is not null or else we get name collisions.
            // However if the NamedStreamAttribute is on a base type that is not an entity type, we want to inherit it by setting
            // inherit to 'true'.
            var namedStreamAttributes = clrType.GetCustomAttributes(typeof(NamedStreamAttribute), inherit).Cast<NamedStreamAttribute>().OrderBy(a => a.Name);
            foreach (var namedStream in namedStreamAttributes)
            {
                resourceType.AddProperty(new ResourceProperty((namedStream).Name, ResourcePropertyKind.Stream, PrimitiveResourceTypeMap.TypeMap.GetPrimitive(typeof(Stream))));
            }
        }

        /// <summary>
        /// Get the PropertyInfo for the given resource property
        /// </summary>
        /// <param name="resourceTypeCacheItem">Instance of ResourceTypeCacheItem containing the ResourceType instance.</param>
        /// <param name="resourceProperty">ResourceProperty instance.</param>
        /// <returns>PropertyInfo instance for the given ResourceProperty.</returns>
        protected ResourcePropertyCacheItem GetResourcePropertyCacheItem(ResourceTypeCacheItem resourceTypeCacheItem, ResourceProperty resourceProperty)
        {
            Debug.Assert(resourceTypeCacheItem.ResourceType.Properties.Contains(resourceProperty), "resourceTypeCacheItem.ResourceType.Properties.Contains(resourceProperty)");
            var declaringResourceType = resourceTypeCacheItem.ResourceType.GetDeclaringTypeForProperty(resourceProperty);
            if (declaringResourceType != resourceTypeCacheItem.ResourceType)
            {
                resourceTypeCacheItem = this.ResolveNonPrimitiveTypeCacheItem(declaringResourceType.InstanceType);
            }

            return resourceTypeCacheItem.GetResourcePropertyCacheItem(resourceProperty);    
        }

        /// <summary>Checks that the metadata model is consistent.</summary>
        protected virtual void CheckModelConsistency()
        {
        }

        /// <summary>Releases the current data source object as necessary.</summary>
        /// <param name="disposing">
        /// Whether this method is called from an explicit call to Dispose by 
        /// the consumer, rather than during finalization.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            WebUtil.Dispose(this.dataSourceInstance);
            this.dataSourceInstance = null;
        }

        /// <summary>
        /// Populates the metadata for the given provider
        /// </summary>
        /// <param name="metadataCacheItem">Instance of ProviderMetadataCacheItem in which metadata needs to be populated.</param>
        protected abstract void PopulateMetadata(ProviderMetadataCacheItem metadataCacheItem);

        /// <summary>
        /// Populate types for metadata specified by the provider
        /// </summary>
        /// <param name="userSpecifiedTypes">list of types specified by the provider</param>
        /// <param name="metadataCacheItem">Instance of ProviderMetadataCacheItem.</param>
        protected abstract void PopulateMetadataForUserSpecifiedTypes(IEnumerable<Type> userSpecifiedTypes, ProviderMetadataCacheItem metadataCacheItem);

        /// <summary>
        /// Populate metadata for the given clr type.
        /// </summary>
        /// <param name="type">type whose metadata needs to be loaded.</param>
        /// <param name="metadataCacheItem">Instance of ProviderMetadataCacheItem.</param>
        /// <returns>resource type containing metadata for the given clr type.</returns>
        protected abstract ResourceType PopulateMetadataForType(Type type, ProviderMetadataCacheItem metadataCacheItem);

        /// <summary>
        /// Returns the resource type for the corresponding clr type.
        /// </summary>
        /// <param name="type">clrType whose corresponding resource type needs to be returned</param>
        /// <returns>Returns the resource type</returns>
        protected virtual ResourceTypeCacheItem ResolveNonPrimitiveTypeCacheItem(Type type)
        {
            return this.metadata.TryGetResourceTypeCacheItem(type);
        }

        /// <summary>
        /// Returns the resource type for the corresponding clr type.
        /// </summary>
        /// <param name="type">clrType whose corresponding resource type needs to be returned</param>
        /// <returns>Returns the resource type</returns>
        protected ResourceType ResolveNonPrimitiveType(Type type)
        {
            var metadataCacheItem = this.ResolveNonPrimitiveTypeCacheItem(type);
            return metadataCacheItem != null ? metadataCacheItem.ResourceType : null;
        }

        /// <summary>
        /// Get the QueryRoot delegate for the given ResourceSet.
        /// </summary>
        /// <param name="resourceSet">ResourceSet instance.</param>
        /// <returns>the delegate for the given ResourceSet.</returns>
        protected Func<object, IQueryable> GetQueryRootDelegate(ResourceSet resourceSet)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");
            Func<object, IQueryable> queryRootDelegate;
            this.metadata.QueryRootCache.TryGetValue(resourceSet, out queryRootDelegate);
            Debug.Assert(queryRootDelegate != null, "queryRootDelegate != null");
            return queryRootDelegate;
        }

        #endregion Protected methods.

        #region Private Methods

        /// <summary>Filter callback for finding IQueryable implementations.</summary>
        /// <param name="m">Type to inspect.</param>
        /// <param name="filterCriteria">Filter criteria.</param>
        /// <returns>true if the specified type is an IQueryable of T; false otherwise.</returns>
        private static bool IQueryableTypeFilter(Type m, object filterCriteria)
        {
            Debug.Assert(m != null, "m != null");
            return m.IsGenericType && m.GetGenericTypeDefinition() == typeof(IQueryable<>);
        }

        /// <summary>Filter callback for finding IEnumerable implementations.</summary>
        /// <param name="m">Type to inspect.</param>
        /// <param name="filterCriteria">Filter criteria.</param>
        /// <returns>true if the specified type is an IEnumerable of T; false otherwise.</returns>
        private static bool IEnumerableTypeFilter(Type m, object filterCriteria)
        {
            Debug.Assert(m != null, "m != null");
            return m.IsGenericType && m.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        /// <summary>Adds service operations based on methods on the data service type.</summary>
        private void LoadServiceOperations()
        {
            // This method is only called for V1 providers, since in case of custom providers,
            // they are suppose to load the metadata themselves.
            Type dataServiceType = this.dataServiceInstance.GetType();

            ServiceOperationProvider sop = new ServiceOperationProvider(dataServiceType, this.ResolveResourceType, this.ResolveResourceSet);

            foreach (ServiceOperation so in sop.ServiceOperations)
            {
                if (this.metadata.ServiceOperations.ContainsKey(so.Name))
                {
                    throw new InvalidOperationException(Strings.BaseServiceProvider_OverloadingNotSupported(dataServiceType, (MethodInfo)so.CustomState));
                }

                this.metadata.ServiceOperations.Add(so.Name, so);
            }
        }

        /// <summary>
        /// Given a CLR type, provides the corresponding <see cref="ResourceType"/> by either looking it up, or loading it's metadata.
        /// </summary>
        /// <param name="type">CLR type for which resource type is being looked up.</param>
        /// <returns><see cref="ResourceType"/> corresponding to <paramref name="type"/>.</returns>
        private ResourceType ResolveResourceType(Type type)
        {
            return this.PopulateMetadataForType(type, this.metadata);
        }

        /// <summary>
        /// Given a <see cref="ResourceType"/>, finds the corresponding <see cref="ResourceSet"/>.
        /// </summary>
        /// <param name="resourceType">Given resource type.</param>
        /// <param name="method">Method implementing service operation.</param>
        /// <returns><see cref="ResourceSet"/> corresponding to <paramref name="resourceType"/>.</returns>
        private ResourceSet ResolveResourceSet(ResourceType resourceType, MethodInfo method)
        {
            ResourceSet container;
            this.TryFindAnyContainerForType(resourceType, out container);
            return container;
        }

        /// <summary>Make all the metadata readonly</summary>
        private void MakeMetadataReadonly()
        {
            Debug.Assert(this.metadataRequiresInitialization, "Should only call when initializing metadata.");

            foreach (ResourceSet container in this.ResourceSets)
            {
                container.SetReadOnly();
            }

            foreach (ResourceType resourceType in this.Types)
            {
                resourceType.SetReadOnly();

                // This will cause Properties collection to be initialized and validated.
                resourceType.PropertiesDeclaredOnThisType.Count();
            }

            foreach (ServiceOperation operation in this.ServiceOperations)
            {
                operation.SetReadOnly();
            }

            // After metadata has been completely loaded, add it to the cache.
            this.metadata = MetadataCache<ProviderMetadataCacheItem>.AddCacheItem(this.dataServiceInstance.GetType(), this.dataSourceInstance, this.metadata);
        }

        /// <summary>
        /// Returns the resource type for the corresponding clr type.
        /// If the given clr type is a collection, then resource type describes the element type of the collection.
        /// </summary>
        /// <param name="type">clrType whose corresponding resource type needs to be returned</param>
        /// <returns>Returns the resource type</returns>
        private ResourceType GetNonPrimitiveType(Type type)
        {
            Debug.Assert(type != null, "type != null");

            // Check for the type directly first
            ResourceType resourceType = this.ResolveNonPrimitiveType(type);
            if (resourceType == null)
            {
                // check for ienumerable types
                Type elementType = BaseServiceProvider.GetIEnumerableElement(type);
                if (elementType != null)
                {
                    resourceType = PrimitiveResourceTypeMap.TypeMap.GetPrimitive(elementType);
                    if (resourceType == null)
                    {
                        resourceType = this.ResolveNonPrimitiveType(elementType);
                    }
                }
            }

            return resourceType;
        }

        /// <summary>
        /// Looks for the first resource set that the specified <paramref name="type"/>
        /// could belong to.
        /// </summary>
        /// <param name="type">Type to look for.</param>
        /// <param name="container">After the method returns, the container to which the type could belong.</param>
        /// <returns>true if a container was found; false otherwise.</returns>
        private bool TryFindAnyContainerForType(ResourceType type, out ResourceSet container)
        {
            Debug.Assert(type != null, "type != null");

            foreach (ResourceSet c in this.metadata.EntitySets.Values)
            {
                if (c.ResourceType.IsAssignableFrom(type))
                {
                    container = c;
                    return true;
                }
            }

            container = default(ResourceSet);
            return false;
        }
        #endregion Private Methods
    }
}

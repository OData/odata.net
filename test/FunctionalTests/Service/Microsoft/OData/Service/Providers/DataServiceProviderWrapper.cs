//---------------------------------------------------------------------
// <copyright file="DataServiceProviderWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.OData.Service.Caching;
    using s = Microsoft.OData.Service;

    #endregion Namespaces

    /// <summary>
    /// Class to abstract IDataServiceMetadataProvider and IDataServiceQueryProvider, 
    /// hence making sure all the metadata and query provider calls are made via this class.
    /// 
    /// Each request must create a new instance of this class because a 
    /// request is the defined scope of metadata consistency.
    /// </summary>
    internal class DataServiceProviderWrapper
    {
        #region Private Fields

        /// <summary>
        /// Maps operations to OperationWrappers.
        /// </summary>
        private readonly OperationCache operationWrapperCache;

        /// <summary>
        /// Maps the operation context to the corresponding MetadataProviderEdmModel which wraps the IDataServiceMetadataProvider
        /// no annotations are included.
        /// </summary>
        private readonly Dictionary<DataServiceOperationContext, MetadataProviderEdmModel> metadataProviderEdmModels;

        /// <summary>
        /// Metadata to be used by the service provider wrapper.
        /// </summary>
        private readonly DataServiceCacheItem metadata;

        /// <summary>
        /// The metadata provider instance.
        /// </summary>
        private IDataServiceMetadataProvider metadataProvider;

        /// <summary>
        /// The query provider instance.
        /// </summary>
        private IDataServiceQueryProvider queryProvider;

        /// <summary>
        /// The data service instance.
        /// </summary>
        private IDataService dataService;

        /// <summary>
        /// boolean flag indicating whether we've enumerated all the resources sets exposed
        /// by the metadata provider and the cache is fully initialized.
        /// </summary>
        private bool IsResourceSetsCacheInitialized = false;

#if DEBUG
        /// <summary>
        /// Set to true after preloading of the cache items, so we can detect lazy loading
        /// in scenarios that we expect to be fully preloaded.
        /// </summary>
        private bool cachePreloaded = false;
#endif

        /// <summary>
        /// Stores the string value "ContainerName."
        /// </summary>
        private string containerNamePrefix;

        /// <summary>
        /// Stores the string value "Namespace.ContainerName."
        /// </summary>
        private string fullyQualifiedContainerNamePrefix;

        /// <summary>
        /// Stores the value of the IDataServiceMetadataProvider.ContainerName value
        /// </summary>
        private string containerNameCache;

        /// <summary>
        /// Stores the value of the IDataServiceMetadataProvider.ContainerNamespace value
        /// </summary>
        private string containerNamespaceCache;

        #region Dynamic Properties

        /// <summary>
        /// The provider behavior. Updated per operation.
        /// </summary>
        private IDataServiceProviderBehavior providerBehavior;

        /// <summary>
        /// The ETag provider. Updated per operation.
        /// </summary>
        private IDataServiceEntityFrameworkProvider entityFrameworkProvider;

        #endregion Dynamic Properties

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Creates a new instance of DataServiceProviderWrapper instance.
        /// </summary>
        /// <param name="cacheItem">Instance of DataServiceCacheItem containing cached configuration and metadata.</param>
        /// <param name="metadataProvider">Instance of the metadata provider.</param>
        /// <param name="queryProvider">Instance of the query provider.</param>
        /// <param name="dataService">The data service instance.</param>
        /// <param name="isInternallyCreatedProvider">Whether the provider was created internally.</param>
        internal DataServiceProviderWrapper(
            DataServiceCacheItem cacheItem,
            IDataServiceMetadataProvider metadataProvider,
            IDataServiceQueryProvider queryProvider,
            IDataService dataService,
            bool isInternallyCreatedProvider)
        {
            Debug.Assert(cacheItem != null, "cacheItem != null");
            Debug.Assert(metadataProvider != null, "metadataProvider != null");
            Debug.Assert(queryProvider != null, "queryProvider != null");

            this.metadata = cacheItem;
            this.metadataProvider = metadataProvider;
            this.queryProvider = queryProvider;
            this.dataService = dataService;
            this.operationWrapperCache = new OperationCache();
            this.metadataProviderEdmModels = new Dictionary<DataServiceOperationContext, MetadataProviderEdmModel>(EqualityComparer<DataServiceOperationContext>.Default);
            this.containerNameCache = null;
            this.containerNamespaceCache = null;

            if (isInternallyCreatedProvider)
            {
                // Obtain the provider behavior and ETag provider just once.
                this.providerBehavior = metadataProvider as IDataServiceProviderBehavior;
                Debug.Assert(this.providerBehavior != null, "Internal providers must implement IDataServiceProviderBehavior interface.");
                
                this.entityFrameworkProvider = metadataProvider as IDataServiceEntityFrameworkProvider;
                Debug.Assert(
                    this.entityFrameworkProvider != null || !DataServiceProviderBehavior.HasEntityFrameworkProviderQueryBehavior(this.providerBehavior),
                    "EntityFrameworkDataServiceProvider must implement IDataServiceEntityFrameworkProvider interface.");
            }
            else
            {
                this.providerBehavior = DataServiceProviderBehavior.CustomDataServiceProviderBehavior;
            }

            this.IsInternallyCreatedProvider = isInternallyCreatedProvider;
        }

        #endregion Constructors

        #region ProviderBehavior

        /// <summary>
        /// Provider behavior, this changes dynamically for each operation for non-internally created providers.
        /// </summary>
        public IDataServiceProviderBehavior ProviderBehavior
        {
            get
            {
                Debug.Assert(Debugger.IsAttached || this.providerBehavior != null, "Must have set the provider behavior before asking for it.");
                return this.providerBehavior;
            }

            set
            {
                Debug.Assert(!this.IsInternallyCreatedProvider, "ProviderBehavior must be set only for non-internally created providers.");

                // Update both the provider behavior as well as the EntityFramework provider.
                this.providerBehavior = value;

                // Ask for the EntityFramework provider for entity framework provider behavior.
                if (DataServiceProviderBehavior.HasEntityFrameworkProviderQueryBehavior(this.providerBehavior))
                {
                    this.entityFrameworkProvider = this.GetService<IDataServiceEntityFrameworkProvider>();
                }
            }
        }

        #endregion

        #region IDataServiceQueryProvider Properties

        /// <summary>The data source from which data is provided.</summary>
        public object CurrentDataSource
        {
            get { return this.queryProvider.CurrentDataSource; }
        }

        /// <summary>Gets a value indicating whether null propagation is required in expression trees.</summary>
        public bool NullPropagationRequired
        {
            get { return this.queryProvider.IsNullPropagationRequired; }
        }

        #endregion IDataServiceQueryProvider Properties

        #region IDataServiceMetadataProvider Properties

        /// <summary>Namespace name for the container.</summary>
        public string ContainerNamespace
        {
            get
            {
                if (this.containerNamespaceCache == null)
                {
                    this.containerNamespaceCache = this.metadataProvider.ContainerNamespace;

                    // [Breaking Change]: Reflection Provider should not allow null ContainerNamespace
                    // In V1 the reflection provider allows the namespace to be null. Fixing this would be a breaking change.
                    // We will skip this check for V1 providers for now.
                    if (string.IsNullOrEmpty(this.containerNamespaceCache) && !DataServiceProviderBehavior.HasReflectionProviderQueryBehavior(this.ProviderBehavior))
                    {
                        throw new InvalidOperationException(s.Strings.DataServiceProviderWrapper_ContainerNamespaceMustNotBeNullOrEmpty);
                    }
                }

                return this.containerNamespaceCache;
            }
        }

        /// <summary>Name of the container</summary>
        public string ContainerName
        {
            get
            {
                if (this.containerNameCache == null)
                {
                    this.containerNameCache = this.metadataProvider.ContainerName;
                    if (string.IsNullOrEmpty(this.containerNameCache))
                    {
                        throw new InvalidOperationException(s.Strings.DataServiceProviderWrapper_ContainerNameMustNotBeNullOrEmpty);
                    }
                }

                return this.containerNameCache;
            }
        }

        #endregion IDataServiceMetadataProvider Properties

        #region Properties

        /// <summary>EDM version to which metadata is compatible.</summary>
        /// <remarks>
        /// For example, a service operation of type Void is not acceptable 1.0 CSDL,
        /// so it should use 1.1 CSDL instead. Similarly, OpenTypes are supported
        /// in 1.2 and not before.
        /// </remarks>
        public MetadataEdmSchemaVersion SchemaVersion
        {
            get
            {
                return MetadataEdmSchemaVersion.Version4Dot0;
            }
        }

        /// <summary>Data Service Response Version for the $metadata.</summary>
        public Version ResponseMetadataVersion
        {
            get { return VersionUtil.Version4Dot0; }
        }

        /// <summary>
        /// Cached configuration with access rights info.
        /// </summary>
        internal DataServiceConfiguration Configuration
        {
            [DebuggerStepThrough]
            get { return this.metadata.Configuration; }
        }

        /// <summary>
        /// Cached static configuration with intercepter information.
        /// </summary>
        internal DataServiceStaticConfiguration StaticConfiguration
        {
            [DebuggerStepThrough]
            get
            {
                return this.metadata.StaticConfiguration;
            }
        }

#if DEBUG
        /// <summary>
        /// Used for verifying expression generated for queries, checks if all the 
        /// </summary>
        internal bool AreAllResourceTypesNonOpen
        {
            get
            {
                return !this.VisibleTypeCache.Values.Any(rt => rt.IsOpenType);
            }
        }
#endif

        /// <summary>
        /// The metadata provider instance.
        /// </summary>
        internal IDataServiceMetadataProvider MetadataProvider
        {
            [DebuggerStepThrough]
            get
            {
                Debug.Assert(this.metadataProvider != null, "this.metadataProvider != null");
                return this.metadataProvider;
            }
        }

        /// <summary>
        /// The query provider instance.
        /// </summary>
        internal IDataServiceQueryProvider QueryProvider
        {
            [DebuggerStepThrough]
            get
            {
                Debug.Assert(this.queryProvider != null, "this.queryProvider != null");
                return this.queryProvider;
            }
        }

        /// <summary>
        /// Returns true if the data provider has ReflectionProviderQueryBehavior or EntityFrameworkProviderQueryBehavior.
        /// Otherwise returns false.
        /// </summary>
        internal bool HasReflectionOrEFProviderQueryBehavior
        {
            get
            {
                return DataServiceProviderBehavior.HasReflectionOrEntityFrameworkProviderQueryBehavior(this.ProviderBehavior);
            }
        }

        /// <summary>
        /// Returns true if the provider was internally created, false otherwise.
        /// </summary>
        internal bool IsInternallyCreatedProvider
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns the <see cref="IProjectionProvider"/> for this provider
        /// </summary>
        /// <returns>The <see cref="IProjectionProvider"/> for this provider</returns>
        /// <remarks>Note that this will only return non-null on V1 providers
        /// in which case it returns our V1 provider's implementation of this interface.
        /// In all other cases this returns null as we don't allow custom implementation of this interface yet.</remarks>
        internal IProjectionProvider ProjectionProvider
        {
            get
            {
                if (this.HasReflectionOrEFProviderQueryBehavior)
                {
                    return DataServiceProviderBehavior.HasEntityFrameworkProviderQueryBehavior(this.ProviderBehavior)
                        ? new BasicExpandProvider(this, false /*expanded*/, false /*castToObject*/)
                        : new BasicExpandProvider(this, true/*expanded*/, true/*castToObject*/);
                }

                return null;
            }
        }

        /// <summary>
        /// Maps operations to OperationWrappers.
        /// </summary>
        internal OperationCache OperationWrapperCache
        {
            [DebuggerStepThrough]
            get { return this.operationWrapperCache; }
        }

        /// <summary>
        /// The data service instance. For batch operations, this is the internal
        /// data service instance and not the top level batch one.
        /// </summary>
        internal IDataService DataService
        {
            get
            {
                return this.dataService;
            }

            set
            {
                Debug.Assert(value != null, "value != null");
                this.dataService = value;
            }
        }

        /// <summary>
        /// Returns the operation context for the current request. For operation within batch,
        /// this returns the context of the current operation, and not the one for the top level
        /// batch request.
        /// </summary>
        internal DataServiceOperationContext OperationContext
        {
            get
            {
                Debug.Assert(this.DataService.OperationContext != null, "this.DataService.OperationContext != null");
                return this.DataService.OperationContext;
            }
        }

        /// <summary>
        /// Returns all types in this data source
        /// WARNING!!! This property can only be called for the $metadata path because it enumerates through all resource types.
        /// Calling it from outside of the $metadata path would break our IDSP contract.
        /// </summary>
        private IEnumerable<ResourceType> Types
        {
            get
            {
                VerifyMetadataRequestUri(this.dataService, false /*canBeServiceDocumentUri*/);
                var types = this.metadataProvider.Types;
                if (types != null)
                {
                    HashSet<string> resourceTypeNames = new HashSet<string>(EqualityComparer<string>.Default);

                    foreach (ResourceType resourceType in types)
                    {
                        // Skip types which should never be defined directly by the metadata provider
                        // This means primitive types which are predefined by the runtime
                        //   as well as collection types which are defined by the runtime on request from the metadata provider 
                        //   and are never used as a standalone types
                        // We will not fail if such types are reported by the metadata provider though
                        if (resourceType.ResourceTypeKind != ResourceTypeKind.EntityType && resourceType.ResourceTypeKind != ResourceTypeKind.ComplexType)
                        {
                            continue;
                        }

                        // verify that the name of the resource type is unique
                        AddUniqueNameToSet(
                            resourceType != null ? resourceType.FullName : null,
                            resourceTypeNames,
                            s.Strings.DataServiceProviderWrapper_MultipleResourceTypesWithSameName(resourceType.FullName));

                        // For IDSP, we want to make sure the metadata object instance stay the same within
                        // a request because we do reference comparisons.  Note the provider can return 
                        // different metadata instances within the same request.  The the Validate*() methods
                        // will make sure to return the first cached instance.
                        ResourceType type = this.ValidateResourceType(resourceType);
                        if (type != null)
                        {
                            yield return type;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Keep track of the calculated visibility of resource types.
        /// </summary>
        private Dictionary<string, ResourceType> VisibleTypeCache
        {
            [DebuggerStepThrough]
            get { return this.metadata.VisibleTypeCache; }
        }

        /// <summary>
        /// Maps resource set names to ResourceSetWrappers.
        /// </summary>
        private Dictionary<string, ResourceSetWrapper> ResourceSetWrapperCache
        {
            [DebuggerStepThrough]
            get { return this.metadata.ResourceSetWrapperCache; }
        }

        /// <summary>
        /// Maps names to ResourceAssociationSets.
        /// </summary>
        private Dictionary<string, ResourceAssociationSet> ResourceAssociationSetCache
        {
            [DebuggerStepThrough]
            get { return this.metadata.ResourceAssociationSetCache; }
        }

        #endregion Properties

        #region IDataServiceQueryProvider Methods

        /// <summary>
        /// Gets all visible containers.
        /// WARNING!!! This property can only be called for the $metadata path because it enumerates through all resource sets.
        /// Calling it from outside of the $metadata path would break our IDSP contract.
        /// </summary>
        /// <returns>All visible containers.</returns>
        public IEnumerable<ResourceSetWrapper> GetResourceSets()
        {
            VerifyMetadataRequestUri(this.dataService, true /*canBeServiceDocumentUri*/);

            // If the cache is already initialized, use the resource set instances from the cache.
            if (!this.IsResourceSetsCacheInitialized)
            {
                var resourceSets = this.metadataProvider.ResourceSets;
                if (resourceSets != null)
                {
                    HashSet<string> resourceSetNames = new HashSet<string>(EqualityComparer<string>.Default);

                    foreach (ResourceSet resourceSet in resourceSets)
                    {
                        // verify that the name of the resource set is unique
                        AddUniqueNameToSet(
                            resourceSet != null ? resourceSet.Name : null,
                            resourceSetNames,
                            s.Strings.DataServiceProviderWrapper_MultipleEntitySetsWithSameName(resourceSet.Name));

                        // For IDSP, we want to make sure the metadata object instance stay the same within
                        // a request because we do reference comparisons.  Note the provider can return 
                        // different metadata instances within the same request.  The the Validate*() methods
                        // will make sure to return the first cached instance.
                        this.ValidateResourceSet(resourceSet);
                    }

                    this.IsResourceSetsCacheInitialized = true;
                }
            }

            return this.ResourceSetWrapperCache.Values.Where(resourceSetWrapper => resourceSetWrapper != null);
        }

        /// <summary>
        /// Returns the Expression that represents the container.
        /// </summary>
        /// <param name="resourceSet">resource set representing the entity set.</param>
        /// <returns>
        /// An Expression that represents the container; null if there is 
        /// no container for the specified name.
        /// </returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "operationContext", Justification = "Intended to only be used in debug builds.")]
        public ConstantExpression GetQueryRootForResourceSet(ResourceSetWrapper resourceSet)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");
            Debug.Assert(this.dataService != null, "this.dataService != null");
            this.dataService.ProcessingPipeline.AssertDebugStateDuringRequestProcessing(this.OperationContext);

            IQueryable queryRoot = this.queryProvider.GetQueryRootForResourceSet(resourceSet.ResourceSet);
            WebUtil.CheckResourceExists(queryRoot != null, resourceSet.Name);
            if (!resourceSet.QueryRootType.IsAssignableFrom(queryRoot.GetType()))
            {
                throw new InvalidOperationException(s.Strings.DataServiceProviderWrapper_InvalidQueryRootType(resourceSet.Name, resourceSet.QueryRootType.FullName));
            }

            return Expression.Constant(queryRoot);
        }

        /// <summary>Gets the <see cref="ResourceType"/> for the specified <paramref name="instance"/>.</summary>
        /// <param name="instance">Instance to extract a <see cref="ResourceType"/> from.</param>
        /// <returns>The <see cref="ResourceType"/> that describes this <paramref name="instance"/> in this provider.</returns>
        public ResourceType GetResourceType(object instance)
        {
            Debug.Assert(instance != null, "instance != null");
            return this.ValidateResourceType(this.queryProvider.GetResourceType(instance));
        }

        /// <summary>
        /// Get the value of the strongly typed property.
        /// </summary>
        /// <param name="target">instance of the type declaring the property.</param>
        /// <param name="resourceProperty">resource property describing the property.</param>
        /// <param name="resourceType">Resource type to which the property belongs.</param>
        /// <returns>value for the property.</returns>
        public object GetPropertyValue(object target, ResourceProperty resourceProperty, ResourceType resourceType)
        {
            Debug.Assert(target != null, "target != null");
            Debug.Assert(resourceProperty != null, "resourceProperty != null");
            Debug.Assert(resourceProperty.IsReadOnly, "resourceProperty.IsReadOnly");
            if (resourceProperty.CanReflectOnInstanceTypeProperty)
            {
                if (resourceType == null)
                {
                    resourceType = this.GetResourceType(target);
                }

                Debug.Assert(resourceType != null, "resourceType != null");
                return resourceType.GetPropertyValue(resourceProperty, target);
            }

            return this.queryProvider.GetPropertyValue(target, resourceProperty);
        }

        /// <summary>
        /// Get the value of the open property.
        /// </summary>
        /// <param name="target">instance of the type declaring the open property.</param>
        /// <param name="propertyName">name of the open property.</param>
        /// <returns>value for the open property.</returns>
        public object GetOpenPropertyValue(object target, string propertyName)
        {
            Debug.Assert(target != null, "target != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            return this.queryProvider.GetOpenPropertyValue(target, propertyName);
        }

        /// <summary>
        /// Get the name and values of all the properties defined in the given instance of an open type.
        /// </summary>
        /// <param name="target">instance of a open type.</param>
        /// <returns>collection of name and values of all the open properties.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "need to return a collection of key value pair")]
        public IEnumerable<KeyValuePair<string, object>> GetOpenPropertyValues(object target)
        {
            Debug.Assert(target != null, "target != null");
            IEnumerable<KeyValuePair<string, object>> result = this.queryProvider.GetOpenPropertyValues(target);
            if (result == null)
            {
                return WebUtil.EmptyKeyValuePairStringObject;
            }

            return result;
        }

        /// <summary>
        /// Invoke the given service operation and returns the results.
        /// </summary>
        /// <param name="serviceOperation">service operation to invoke.</param>
        /// <param name="parameters">value of parameters to pass to the service operation.</param>
        /// <returns>returns the result of the service operation. If the service operation returns void, then this should return null.</returns>
        public ConstantExpression InvokeServiceOperation(OperationWrapper serviceOperation, object[] parameters)
        {
            Debug.Assert(serviceOperation != null, "serviceOperation != null");
            Debug.Assert(serviceOperation.Kind == OperationKind.ServiceOperation, "serviceOperation.Kind == OperationKind.ServiceOperation");
            try
            {
                return Expression.Constant(this.queryProvider.InvokeServiceOperation(serviceOperation.ServiceOperation, parameters));
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
        /// Returns all visible types in this data source
        /// WARNING!!! This property can only be called for the $metadata path because it enumerates through all resource types.
        /// Calling it from outside of the $metadata path would break our IDSP contract.
        /// </summary>
        /// <returns>All visible types in this data source.</returns>
        public IEnumerable<ResourceType> GetVisibleTypes()
        {
            if (this.Configuration.AccessEnabledForAllResourceTypes)
            {
                // If all types is marked visible, we can simply return the type collection from the wrapped provider
                foreach (ResourceType type in this.Types)
                {
                    Debug.Assert(this.VisibleTypeCache.ContainsKey(type.FullName), "this.VisibleTypeCache.ContainsKey(type.FullName) or else the type has not been validated");
                    yield return type;
                }
            }
            else
            {
                // Hash set to make sure we only return each type once.  Note the hash set will use reference comparison for ResourceType.
                // The EqualityComparer<T>.Default property checks whether type T implements the System::IEquatable<T> interface and, if so, returns an EqualityComparer<T>
                // that uses that implementation. Otherwise, it returns an EqualityComparer<T> that uses the overrides of Object::Equals and Object::GetHashCode provided by T.
                HashSet<ResourceType> visitedTypes = new HashSet<ResourceType>(EqualityComparer<ResourceType>.Default);
                IEnumerable<ResourceType> visibleTypes = new ResourceType[0];

                // Add entity types reachable from visible sets
                foreach (ResourceSetWrapper resourceSet in this.GetResourceSets())
                {
                    visibleTypes = visibleTypes.Concat(this.GetReachableTypesFromSet(resourceSet, visitedTypes));
                }

                // Add resource types reachable from operations
                foreach (OperationWrapper serviceOperation in this.GetVisibleOperations())
                {
                    visibleTypes = visibleTypes.Concat(this.GetReachableComplexTypesFromOperation(serviceOperation, visitedTypes));
                }

                // Add resource types marked visible by DataServiceConfiguration.EnableAccess().
                foreach (string resourceTypeName in this.Configuration.GetAccessEnabledResourceTypes())
                {
                    ResourceType complexType = this.TryResolveResourceType(resourceTypeName);
                    if (complexType == null)
                    {
                        throw new InvalidOperationException(s.Strings.MetadataSerializer_AccessEnabledTypeNoLongerExists(resourceTypeName));
                    }

                    Debug.Assert(complexType.ResourceTypeKind == ResourceTypeKind.ComplexType, "complexType.ResourceTypeKind == ResourceTypeKind.ComplexType");
                    visibleTypes = visibleTypes.Concat(this.GetResourceTypeAndReachableComplexTypes(complexType, visitedTypes));
                }

                foreach (ResourceType type in visibleTypes)
                {
                    Debug.Assert(visitedTypes.Contains(type), "visitedTypes.Contains(type)");
                    Debug.Assert(this.VisibleTypeCache.ContainsKey(type.FullName), "this.VisibleTypeCache.ContainsKey(type.FullName) or else the type has not been validated");
                    yield return type;
                }
            }
        }

        /// <summary>
        /// Returns all the visible operations in this data service.
        /// WARNING!!! This property can only be called for the $metadata path because it enumerates through all service operations.
        /// Calling it from outside of the $metadata path would break our IDSP contract.
        /// </summary>
        /// <returns>All visible operations in this data service.</returns>
        public IEnumerable<OperationWrapper> GetVisibleOperations()
        {
            Debug.Assert(this.dataService != null, "this.dataService != null");
            VerifyMetadataRequestUri(this.dataService, false /*canBeServiceDocumentUri*/);

            OperationCache operations = new OperationCache();

            var serviceOperations = this.metadataProvider.ServiceOperations;
            if (serviceOperations != null)
            {
                foreach (ServiceOperation serviceOperation in serviceOperations)
                {
                    if (serviceOperation != null && operations.Contains(serviceOperation))
                    {
                        throw new DataServiceException(500, s.Strings.DataServiceProviderWrapper_MultipleServiceOperationsWithSameName(serviceOperation.Name));
                    }

                    // For IDSP, we want to make sure the metadata object instance stay the same within
                    // a request because we do reference comparisons.  Note the provider can return 
                    // different metadata instances within the same request.  The the Validate*() methods
                    // will make sure to return the first cached instance.
                    OperationWrapper serviceOperationWrapper = this.ValidateOperation(serviceOperation);
                    if (serviceOperationWrapper != null)
                    {
                        operations.Add(serviceOperationWrapper);
                        yield return serviceOperationWrapper;
                    }
                }
            }

            foreach (OperationWrapper serviceAction in this.dataService.ActionProvider.GetServiceActions())
            {
                Debug.Assert(serviceAction != null, "serviceAction != null");

                if (operations.Contains(serviceAction.ServiceAction))
                {
                    throw new DataServiceException(500, s.Strings.DataServiceActionProviderWrapper_DuplicateAction(serviceAction.Name));
                }

                operations.Add(serviceAction);

                List<ResourceSetWrapper> bindableSets = new List<ResourceSetWrapper>();

                // If the binding entity type is not reachable from any visible set, the service action is not visible.
                if (serviceAction.BindingParameter != null)
                {
                    ResourceType bindingElementType = serviceAction.BindingParameter.ParameterType;
                    if (bindingElementType.ResourceTypeKind == ResourceTypeKind.EntityCollection)
                    {
                        bindingElementType = ((EntityCollectionResourceType)bindingElementType).ItemType;
                    }

                    foreach (ResourceSetWrapper set in this.GetResourceSets())
                    {
                        if (set.ResourceType.IsAssignableFrom(bindingElementType))
                        {
                            bindableSets.Add(set);
                        }
                    }

                    if (!bindableSets.Any())
                    {
                        throw new InvalidOperationException(s.Strings.DataServiceProviderWrapper_ActionHasNoBindableSet(serviceAction.Name, serviceAction.BindingParameter.ParameterType.FullName));
                    }
                }

                Debug.Assert(
                    serviceAction.ResultType == null || serviceAction.ResultType.ResourceTypeKind != ResourceTypeKind.EntityType || serviceAction.ResultSetPathExpression != null || serviceAction.ResourceSet.IsVisible,
                    "When the action returns an entity or entity collection type, the action must have a path expression or its result set is visible.");

                // If the ResultSetPathExpression is not null, the service action is visible only if the path can yield
                // a visible result set.
                if (serviceAction.ResultSetPathExpression != null)
                {
                    Debug.Assert(serviceAction.ResourceSet == null, "ResourceSet and ResultSetPathExpression are mutually exclusive.");
                    if (bindableSets.All(set => serviceAction.ResultSetPathExpression.GetTargetSet(this, set) == null))
                    {
                        throw new InvalidOperationException(s.Strings.DataServiceProviderWrapper_ActionHasNoVisibleSetReachableFromPathExpression(serviceAction.Name, serviceAction.ResultSetPathExpression.PathExpression));
                    }
                }

                yield return serviceAction;
            }
        }

        /// <summary>Given the specified name, tries to find a resource set.</summary>
        /// <param name="name">Name of the resource set to resolve.</param>
        /// <returns>Resolved resource set, possibly null.</returns>
        public ResourceSetWrapper TryResolveResourceSet(string name)
        {
            Debug.Assert(!string.IsNullOrEmpty(name), "!string.IsNullOrEmpty(name)");

            // For IDSP, we want to make sure the metadata object instance stay the same within
            // a request because we do reference comparisons.
            ResourceSetWrapper resourceSetWrapper;
            if (this.ResourceSetWrapperCache.TryGetValue(name, out resourceSetWrapper))
            {
                return resourceSetWrapper;
            }

            ResourceSet resourceSet;
            if (this.metadataProvider.TryResolveResourceSet(name, out resourceSet))
            {
                return this.ValidateResourceSet(resourceSet);
            }

            return null;
        }

        /// <summary>
        /// Gets the ResourceAssociationSet instance when given the source association end.
        /// </summary>
        /// <param name="resourceSet">Resource set of the source association end.</param>
        /// <param name="resourceType">Resource type of the source association end.</param>
        /// <param name="resourceProperty">Resource property of the source association end.</param>
        /// <returns>ResourceAssociationSet instance.</returns>
        public ResourceAssociationSet GetResourceAssociationSet(ResourceSetWrapper resourceSet, ResourceType resourceType, ResourceProperty resourceProperty)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");
            Debug.Assert(resourceType != null, "resourceType != null");
            Debug.Assert(resourceProperty != null && !resourceProperty.IsOfKind(ResourcePropertyKind.Stream), "resourceProperty != null && !resourceProperty.IsOfKind(ResourcePropertyKind.Stream)");

            // If the association set has already been cached, use the cached copy
            resourceType = resourceType.GetDeclaringTypeForProperty(resourceProperty);
            string associationSetKey = resourceSet.Name + '_' + resourceType.FullName + '_' + resourceProperty.Name;
            ResourceAssociationSet associationSet;
            if (this.ResourceAssociationSetCache.TryGetValue(associationSetKey, out associationSet))
            {
                if (associationSet != null)
                {
                    // If the resource association set is already in the cache, we need to make sure the entity set and type of the related
                    // end is already present in the model
                    ResourceAssociationSetEnd relatedEnd = associationSet.GetRelatedResourceAssociationSetEnd(resourceSet, resourceType, resourceProperty);
                    this.ValidateResourceSet(relatedEnd.ResourceSet);
                }

                return associationSet;
            }

            // Get the association set from the underlying provider.
            associationSet = this.metadataProvider.GetResourceAssociationSet(resourceSet.ResourceSet, resourceType, resourceProperty);
            if (associationSet != null)
            {
                ResourceAssociationSetEnd thisEnd = associationSet.GetResourceAssociationSetEnd(resourceSet, resourceType, resourceProperty);
                ResourceAssociationSetEnd relatedEnd = associationSet.GetRelatedResourceAssociationSetEnd(resourceSet, resourceType, resourceProperty);
                ResourceSetWrapper relatedSet = this.ValidateResourceSet(relatedEnd.ResourceSet);
                if (relatedSet == null)
                {
                    // If the related set is not visible, the association set is also not visible.
                    associationSet = null;
                }
                else
                {
                    ResourceType relatedType = this.ValidateResourceType(relatedEnd.ResourceType);
                    ResourceProperty relatedProperty = null;
                    if (relatedEnd.ResourceProperty != null)
                    {
                        relatedProperty = relatedType.TryResolvePropertyName(relatedEnd.ResourceProperty.Name, exceptKind: ResourcePropertyKind.Stream);
                    }

                    // For IDSP, we want to make sure the metadata object instance stay the same within a request because we 
                    // do reference comparisons.  Note if the provider returns a ResourceAssociationSet with different instances
                    // of ResourceSet, ResourceType and ResourceProperty than what we've seen earlier in the same request, we
                    // create a new instance of the association set using the metadata objects we've already cached.
                    // If the metadata change should cause a failure, we want the IDSP to detect it and fail. At the Astoria runtime
                    // layer we assume the metadata objects to remain constant throughout the request.
                    resourceType = this.ValidateResourceType(thisEnd.ResourceType);
                    if (thisEnd.ResourceSet != resourceSet.ResourceSet ||
                        thisEnd.ResourceType != resourceType ||
                        thisEnd.ResourceProperty != resourceProperty ||
                        relatedEnd.ResourceSet != relatedSet.ResourceSet ||
                        relatedEnd.ResourceType != relatedType ||
                        relatedEnd.ResourceProperty != relatedProperty)
                    {
                        associationSet = new ResourceAssociationSet(
                            associationSet.Name,
                            new ResourceAssociationSetEnd(resourceSet.ResourceSet, resourceType, resourceProperty),
                            new ResourceAssociationSetEnd(relatedSet.ResourceSet, relatedType, relatedProperty));
                    }
                }
            }

            AssertCacheNotPreloaded(this);
            this.ResourceAssociationSetCache.Add(associationSetKey, associationSet);
            return associationSet;
        }

        /// <summary>Given the specified name, tries to find a type.</summary>
        /// <param name="name">Name of the type to resolve.</param>
        /// <returns>Resolved resource type, possibly null.</returns>
        public ResourceType TryResolveResourceType(string name)
        {
            Debug.Assert(!string.IsNullOrEmpty(name), "!string.IsNullOrEmpty(name)");

            // For IDSP, we want to make sure the metadata object instance stay the same within
            // a request because we do reference comparisons.
            ResourceType resourceType;
            if (this.VisibleTypeCache.TryGetValue(name, out resourceType))
            {
                return resourceType;
            }

            if (this.metadataProvider.TryResolveResourceType(name, out resourceType))
            {
                return this.ValidateResourceType(resourceType);
            }

            return null;
        }

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
        public IEnumerable<ResourceType> GetDerivedTypes(ResourceType resourceType)
        {
            Debug.Assert(resourceType != null, "resourceType != null");

            var derivedTypes = this.metadataProvider.GetDerivedTypes(resourceType);
            if (derivedTypes != null)
            {
                foreach (ResourceType derivedType in derivedTypes)
                {
                    ResourceType type = this.ValidateResourceType(derivedType);
                    if (type != null)
                    {
                        yield return type;
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if <paramref name="resourceType"/> represents an Entity Type which has derived Entity Types, else false.
        /// </summary>
        /// <param name="resourceType">instance of the resource type in question.</param>
        /// <returns>True if <paramref name="resourceType"/> represents an Entity Type which has derived Entity Types, else false.</returns>
        public bool HasDerivedTypes(ResourceType resourceType)
        {
            Debug.Assert(this.ValidateResourceType(resourceType) != null, "resourceType must be read-only and visible.");
            return this.metadataProvider.HasDerivedTypes(resourceType);
        }

        /// <summary>Given the specified name, tries to find a service operation.</summary>
        /// <param name="name">Name of the service operation to resolve.</param>
        /// <returns>Resolved service operation, possibly null.</returns>
        public OperationWrapper TryResolveServiceOperation(string name)
        {
            Debug.Assert(!string.IsNullOrEmpty(name), "!string.IsNullOrEmpty(name)");

            // For IDSP, we want to make sure the metadata object instance stay the same within
            // a request because we do reference comparisons.
            OperationWrapper serviceOperationWrapper;
            if (this.operationWrapperCache.TryGetWrapper(name, /*bindingType*/ null, out serviceOperationWrapper))
            {
                if (serviceOperationWrapper != null && serviceOperationWrapper.Kind == OperationKind.ServiceOperation)
                {
                    return serviceOperationWrapper;
                }

                return null;
            }

            ServiceOperation serviceOperation;
            if (this.metadataProvider.TryResolveServiceOperation(name, out serviceOperation))
            {
                return this.ValidateOperation(serviceOperation);
            }

            return null;
        }

        #endregion IDataServiceMetadataProvider Methods

        #region Internal Methods

        /// <summary>Disposes of the metadata and query providers.</summary>
        internal void DisposeDataSource()
        {
            Debug.Assert(this.queryProvider != null, "this.queryProvider != null");
            Debug.Assert(this.metadataProvider != null, "this.metadataProvider != null");

            WebUtil.Dispose(this.metadataProvider);

            // If the same instance implements IDataServiceMetadataProvider and IDataServiceQueryProvider interface,
            // we call dispose only once.
            if (this.metadataProvider != this.queryProvider)
            {
                WebUtil.Dispose(this.queryProvider);
            }

            this.metadataProvider = null;
            this.queryProvider = null;
            this.dataService = null;
        }

        /// <summary>
        /// Gets the MetadataProviderEdmModel over this provider's metadata.
        /// </summary>
        /// <returns>The MetadataProviderEdmModel over this provider's metadata.</returns>
        internal MetadataProviderEdmModel GetMetadataProviderEdmModel()
        {
            Debug.Assert(this.metadataProviderEdmModels != null, "this.metadataProviderEdmModels != null");

            MetadataProviderEdmModel model;
            if (!this.metadataProviderEdmModels.TryGetValue(this.OperationContext, out model))
            {
                Debug.Assert(this.dataService != null, "this.dataService != null");
                model = new MetadataProviderEdmModel(this, this.dataService.StreamProvider, this.dataService.ActionProvider);

                this.metadataProviderEdmModels[this.OperationContext] = model;
            }

            return model;
        }

        /// <summary>
        /// Iterates through the resource sets, service operations and resource types to pre-populate the metadata cache item.
        /// </summary>
        internal void PopulateMetadataCacheItemForBuiltInProvider()
        {
            Debug.Assert(this.IsInternallyCreatedProvider, "this.IsInternallyCreatedProvider");

            // This is only called when we initialize the service for the first time.
            // The Count extention method will cause the iterator to instantiate the wrapper classes.
            //
            // The OperationWrapperCache contains both service operations and service actions. Service action metadata is dynamic
            // and can change between requests, therefore the OperationWrapperCache will not persist across requests.  We only
            // need to instantiate the ServiceOperations in the underlying provider and not the OperationWrappers here.
            var serviceOperations = this.metadataProvider.ServiceOperations;
            if (serviceOperations != null)
            {
                serviceOperations.Count();
            }

            // Enumerate all types to populate the type cache.
            this.GetVisibleTypes().Count();

            // load the cache with
            //   ResourceSets
            //   ResourceAssociationSets
            foreach (ResourceSetWrapper resourceSet in this.GetResourceSets())
            {
                // do it for all the derived types
                foreach (ResourceType derivedType in this.GetDerivedTypes(resourceSet.ResourceType))
                {
                    resourceSet.GetEntitySerializableProperties(this, derivedType);
                    foreach (var resourceProperty in derivedType.PropertiesDeclaredOnThisType.Where(p => p.Kind == ResourcePropertyKind.ResourceReference || p.Kind == ResourcePropertyKind.ResourceSetReference))
                    {
                        this.GetResourceAssociationSet(resourceSet, derivedType, resourceProperty);
                    }
                }

                // do it for the base resourceSet type
                ResourceType resourceType = resourceSet.ResourceType;
                resourceSet.GetEntitySerializableProperties(this, resourceType);
                foreach (var resourceProperty in resourceType.Properties.Where(p => p.Kind == ResourcePropertyKind.ResourceReference || p.Kind == ResourcePropertyKind.ResourceSetReference))
                {
                    this.GetResourceAssociationSet(resourceSet, resourceType, resourceProperty);
                }
            }

#if DEBUG
            this.cachePreloaded = true;
#endif
        }

        /// <summary>
        /// Gets the target container for the given navigation property, source container and the source resource type
        /// </summary>
        /// <param name="sourceResourceSet">source entity set.</param>
        /// <param name="sourceResourceType">source resource type.</param>
        /// <param name="navigationProperty">navigation property.</param>
        /// <returns>target container that the navigation property refers to.</returns>
        internal ResourceSetWrapper GetResourceSet(ResourceSetWrapper sourceResourceSet, ResourceType sourceResourceType, ResourceProperty navigationProperty)
        {
            ResourceAssociationSet associationSet = this.GetResourceAssociationSet(sourceResourceSet, sourceResourceType, navigationProperty);
            if (associationSet != null)
            {
                ResourceAssociationSetEnd relatedEnd = associationSet.GetRelatedResourceAssociationSetEnd(sourceResourceSet, sourceResourceType, navigationProperty);
                return this.ValidateResourceSet(relatedEnd.ResourceSet);
            }

            return null;
        }

        /// <summary>
        /// Return the list of ETag properties for a given type in the context of a given container
        /// </summary>
        /// <param name="containerName">Name of the container to use for context (for MEST-enabled providers)</param>
        /// <param name="resourceType">Type to get the ETag properties for</param>
        /// <returns>A collection of the properties that form the ETag for the given type in the given container</returns>
        internal IList<ResourceProperty> GetETagProperties(string containerName, ResourceType resourceType)
        {
            Debug.Assert(resourceType != null && resourceType.ResourceTypeKind == ResourceTypeKind.EntityType, "Resource should be non-null and of an entity type");

            if (this.entityFrameworkProvider != null)
            {
                return this.entityFrameworkProvider.GetETagProperties(containerName, resourceType);
            }
            else
            {
                return resourceType.ETagProperties;
            }
        }

        /// <summary>
        /// Gets the visible resource properties for <paramref name="resourceType"/> from <paramref name="resourceSet"/>.
        /// We cache the list of visible resource properties so we don't have to calculate it repeatedly when serializing feeds.
        /// </summary>
        /// <param name="resourceSet">Resource set in question.</param>
        /// <param name="resourceType">Resource type in question.</param>
        /// <returns>List of visible resource properties from the given resource set and resource type.</returns>
        internal IEnumerable<ResourceProperty> GetResourceSerializableProperties(ResourceSetWrapper resourceSet, ResourceType resourceType)
        {
            Debug.Assert(resourceType != null, "resourceType != null");
            if (resourceType.ResourceTypeKind == ResourceTypeKind.EntityType)
            {
                Debug.Assert(resourceSet != null, "resourceSet != null");
                return resourceSet.GetEntitySerializableProperties(this, resourceType);
            }
            else
            {
                return resourceType.Properties;
            }
        }

        /// <summary>
        /// Checks whether the current data provider behaves like a reflection or EF provider and whether it implements IUpdatable.
        /// </summary>
        /// <returns>Returns true if the current data source behaves like a reflection or EF provider and implements IUpdatable. Otherwise, returns false.</returns>
        internal bool IsReflectionOrEFProviderAndImplementsUpdatable()
        {
            return this.HasReflectionOrEFProviderQueryBehavior && this.GetService<IUpdatable>() != null;
        }

        /// <summary>
        /// Retrieve an implementation of a data service interface (ie. IUpdatable, IExpandProvider,etc)
        /// </summary>
        /// <typeparam name="T">The type representing the requested interface</typeparam>
        /// <returns>An object implementing the requested interface, or null if not available</returns>
        internal T GetService<T>() where T : class
        {
            Debug.Assert(this.dataService != null, "this.dataService != null");
            Debug.Assert(this.dataService.Provider == this, "this.dataService.Provider == this");
            Debug.Assert(typeof(T) != typeof(IDataServiceMetadataProvider), "typeof(T) != typeof(IDataServiceMetadataProvider)");
            Debug.Assert(typeof(T) != typeof(IDataServiceQueryProvider), "typeof(T) != typeof(IDataServiceQueryProvider)");
            Debug.Assert(typeof(T).IsVisible, "Trying to ask the service for non-public interface.");

            this.dataService.ProcessingPipeline.AssertAndUpdateDebugStateAtGetService();

            // 1. Check if subclass of DataService<T> implements IServiceProvider. If it does, then call IServiceProvider.GetService()
            // with the appropriate type. If it doesn�t proceed to Step 2
            //    a. If IServiceProvider.GetService() returns something, then go ahead and use that instance
            //    b. If IServiceProvider.GetService() doesn't return anything, proceed to Step 2.
            T result = WebUtil.GetService<T>(this.dataService.Instance);
            if (result != null)
            {
                return result;
            }

            // 2. Check if the T (where T is the type from DataService<T>) implements the interface.
            //    a. If yes, use that.
            //    b. If no, proceed to Step 3
            result = this.CurrentDataSource as T;
            if (result != null)
            {
                return result;
            }

            // 3. Check if the data service provider is a V1 provider
            //    a. If yes, return an internal implementation if we can find one.
            //    b. If no, then the provider doesn�t support the current interface functionality.
            if (this.HasReflectionOrEFProviderQueryBehavior)
            {
                // Look for internal implementation for the interface
                return WebUtil.GetService<T>(this.metadataProvider);
            }

            return null;
        }

        /// <summary>
        /// Validates if the container should be visible and is not read only. If the container rights
        /// are set to None the container should not be visible.
        /// </summary>
        /// <param name="resourceSet">Resource set to be validated.</param>
        /// <returns>Validated container, null if the container is not supposed to be visible.</returns>
        internal ResourceSetWrapper ValidateResourceSet(ResourceSet resourceSet)
        {
            ResourceSetWrapper resourceSetWrapper = null;
            if (resourceSet != null)
            {
                // For IDSP, we want to make sure the metadata object instance stay the same within
                // a request because we do reference comparisons.  Note the provider can return 
                // different metadata instances within the same request.  The the Validate*() methods
                // will make sure to return the first cached instance.
                if (this.ResourceSetWrapperCache.TryGetValue(resourceSet.Name, out resourceSetWrapper))
                {
                    return resourceSetWrapper;
                }

                resourceSetWrapper = ResourceSetWrapper.CreateResourceSetWrapper(resourceSet, this, this.ValidateResourceType);
                AssertCacheNotPreloaded(this);
                this.ResourceSetWrapperCache[resourceSet.Name] = resourceSetWrapper;

                // Today we have nothing to verify on the resourceset. But if we had a feature in the future
                // that requires the protocol version to be bumped based on the set, this is the place to add the
                // verification
            }

            return resourceSetWrapper;
        }

        /// <summary>
        /// Validates if the service operation should be visible and is read only. If the service operation
        /// rights are set to None the service operation should not be visible.
        /// </summary>
        /// <param name="operation">Operation to be validated.</param>
        /// <returns>Validated service operation, null if the service operation is not supposed to be visible.</returns>
        internal OperationWrapper ValidateOperation(Operation operation)
        {
            OperationWrapper serviceOperationWrapper = null;
            if (operation != null)
            {
                // For IDSP, we want to make sure the metadata object instance stay the same within
                // a request because we do reference comparisons.  Note the provider can return 
                // different metadata instances within the same request.  The the Validate*() methods
                // will make sure to return the first cached instance.
                if (this.operationWrapperCache.TryGetWrapper(operation, out serviceOperationWrapper))
                {
                    return serviceOperationWrapper;
                }

                serviceOperationWrapper = new OperationWrapper(operation);
                serviceOperationWrapper.ApplyConfiguration(this.Configuration, this);
                if (!serviceOperationWrapper.IsVisible)
                {
                    serviceOperationWrapper = null;
                }
                else
                {
                    this.operationWrapperCache.Add(serviceOperationWrapper);
                }

                // Today we have nothing to verify on the serviceoperation. But if we had a feature in the future
                // that requires the protocol version to be bumped based on the sop, this is the place to add the
                // verification
            }

            return serviceOperationWrapper;
        }

        /// <summary>
        /// Return the list of custom annotation for the entity container with the given name.
        /// </summary>
        /// <param name="entityContainerName">Name of the EntityContainer.</param>
        /// <returns>Return the list of custom annotation for the entity container with the given name.</returns>
        internal IEnumerable<KeyValuePair<string, object>> GetEntityContainerAnnotations(string entityContainerName)
        {
            // Only return annotations for internally created entity framework provider.
            if (this.IsInternallyCreatedProvider)
            {
                IDataServiceInternalProvider internalProvider = this.MetadataProvider as IDataServiceInternalProvider;
                Debug.Assert(internalProvider != null, "Internally created providers must implement IDataServiceInternalProvider interface.");
                if (internalProvider != null)
                {
                    return internalProvider.GetEntityContainerAnnotations(entityContainerName);
                }
            }

            return WebUtil.EmptyKeyValuePairStringObject;
        }

        /// <summary>
        /// Get the name portion of a container qualified name, i.e. returns Name from Namespace.ContainerName.Name or ContainerName.Name
        /// </summary>
        /// <param name="containerQualifiedName">A name qualified by the container name.</param>
        /// <param name="nameIsContainerQualified">Returns true if <paramref name="containerQualifiedName"/> is prefixed with "ContainerName." or "Namespace.ContainerName."; otherwise return false.</param>
        /// <returns>Returns the name portion of <paramref name="containerQualifiedName"/> if it is prefixed with "ContainerName." or "Namespace.ContainerName.";
        /// otherwise returns the given <paramref name="containerQualifiedName"/>.</returns>
        internal string GetNameFromContainerQualifiedName(string containerQualifiedName, out bool nameIsContainerQualified)
        {
            Debug.Assert(containerQualifiedName != null, "fullyQualifiedServiceActionName != null");

            nameIsContainerQualified = false;
            string name = containerQualifiedName;
            this.containerNamePrefix = this.containerNamePrefix ?? this.ContainerName + ".";
            this.fullyQualifiedContainerNamePrefix = this.fullyQualifiedContainerNamePrefix ?? (string.IsNullOrEmpty(this.ContainerNamespace) ? this.containerNamePrefix : this.ContainerNamespace + "." + this.containerNamePrefix);

            if (name.StartsWith(this.fullyQualifiedContainerNamePrefix, StringComparison.Ordinal))
            {
                name = name.Substring(this.fullyQualifiedContainerNamePrefix.Length);
                nameIsContainerQualified = true;
            }
            else if (name.StartsWith(this.containerNamePrefix, StringComparison.Ordinal))
            {
                name = name.Substring(this.containerNamePrefix.Length);
                nameIsContainerQualified = true;
            }

            // If name is empty (name can never be null), it means the containerQualifiedName is "Namespace.ContainerName." or "ContainerName.".
            // We will return the original containerQualifiedName as it is and throw at a later point if the providers can't resolve the name.
            if (string.IsNullOrEmpty(name))
            {
                nameIsContainerQualified = false;
                return containerQualifiedName;
            }

            return name;
        }

        /// <summary>
        /// Get the name portion of a namespace qualified name, i.e. returns Name from Namespace.Name
        /// </summary>
        /// <param name="namespaceQualifiedName">A name qualified by the namespace name.</param>
        /// <param name="nameIsNamespaceQualified">Returns true if <paramref name="nameIsNamespaceQualified"/> is prefixed with "Namespace."; otherwise return false.</param>
        /// <returns>Returns the name portion of <paramref name="namespaceQualifiedName"/> if it is prefixed with "Namespace.";
        /// otherwise returns the given <paramref name="namespaceQualifiedName"/>.</returns>
        internal string GetNameFromNamespaceQualifiedName(string namespaceQualifiedName, out bool nameIsNamespaceQualified)
        {
            Debug.Assert(namespaceQualifiedName != null, "namespaceQualifiedName != null");

            nameIsNamespaceQualified = false;
            
            string name = namespaceQualifiedName;
            if (name.StartsWith(this.ContainerNamespace + ".", StringComparison.Ordinal))
            {
                name = name.Substring(this.ContainerNamespace.Length + 1);
                nameIsNamespaceQualified = true;
            }

            return name;
        }

        #endregion Internal Methods

        #region Private Methods

        /// <summary>
        /// Throws if resource type is not sealed.
        /// </summary>
        /// <param name="resourceType">resource type to inspect.</param>
        private static void ValidateResourceTypeReadOnly(ResourceType resourceType)
        {
            Debug.Assert(resourceType != null, "resourceType != null");
            if (!resourceType.IsReadOnly)
            {
                throw new DataServiceException(500, s.Strings.DataServiceProviderWrapper_ResourceTypeNotReadonly(resourceType.FullName));
            }
        }

        /// <summary>
        /// This is a common method for checking uniqe names across entity sets, resource types and service operations.
        /// </summary>
        /// <param name="name">Name to be added to set.</param>
        /// <param name="names">Set containing already verified names.</param>
        /// <param name="exceptionString">String for exception to be thrown if the name is not unique.</param>
        private static void AddUniqueNameToSet(string name, HashSet<string> names, string exceptionString)
        {
            if (name != null)
            {
                if (names.Contains(name))
                {
                    throw new DataServiceException(500, exceptionString);
                }

                names.Add(name);
            }
        }

        /// <summary>
        /// Assert that we didn't expect the cache to already be preloaded.
        /// </summary>
        /// <param name="wrapper">The data service provider wrapper whose value of cache preloaded to check.</param>
        [Conditional("DEBUG")]
        private static void AssertCacheNotPreloaded(DataServiceProviderWrapper wrapper)
        {
#if DEBUG
            Debug.Assert(!wrapper.cachePreloaded, "the cache was preloaded, why did we get a cache miss?");
#endif
        }

        /// <summary>
        /// Verify that the current request uri is metadata or service document uri.
        /// </summary>
        /// <param name="dataService">The data service instance.</param>
        /// <param name="canBeServiceDocumentUri">true if the request uri can be service uri also.</param>
        [Conditional("DEBUG")]
        private static void VerifyMetadataRequestUri(IDataService dataService, bool canBeServiceDocumentUri)
        {
            // For testing purposes, we need to check if the service and operationContext is null or not.
            // In actual service scenarios, the service and the context will never be null.
            if (dataService != null && dataService.OperationContext != null)
            {
                DataServiceOperationContext operationContext = dataService.OperationContext;

                // For internal providers, we call the Types/Sets method for the first request to load all the metadata.
                // So this check must be done only for custom providers.
                if (!dataService.Provider.IsInternallyCreatedProvider)
                {
                    if (!canBeServiceDocumentUri)
                    {
                        // Uri.AbsoluteUri escapes the URI in 4.5
                        Debug.Assert(
                            operationContext.AbsoluteRequestUri.AbsoluteUri.Contains("$metadata") || 
                            operationContext.AbsoluteRequestUri.AbsoluteUri.Contains("%24metadata"),
                            "This method must be called only for $metadata");
                    }
                    else
                    {
                        Debug.Assert(
                            operationContext.AbsoluteRequestUri.AbsoluteUri.Contains("$metadata") ||
                            operationContext.AbsoluteRequestUri.AbsoluteUri.Contains("%24metadata") ||
                            operationContext.AbsoluteRequestUri.AbsoluteUri == operationContext.AbsoluteServiceUri.AbsoluteUri,
                            "This method must be called only for $metadata or for service documents");
                    }
                }
            }
        }

        /// <summary>Returns the given resource type and all reachable complex types from the resource type</summary>
        /// <param name="resourceType">resource type to inspect</param>
        /// <param name="visitedTypes">Hash set to make sure we only return unvisited types.</param>
        /// <returns>Returns the given resource type and all reachable complex types from the resource type</returns>
        private IEnumerable<ResourceType> GetResourceTypeAndReachableComplexTypes(ResourceType resourceType, HashSet<ResourceType> visitedTypes)
        {
            Debug.Assert(resourceType != null, "resourceType != null");
            Debug.Assert(visitedTypes != null, "visitedTypes != null");

            // ValidateResourceType must be called before we can safely use reference comparison against the type
            resourceType = this.ValidateResourceType(resourceType);
            if (!visitedTypes.Contains(resourceType))
            {
                visitedTypes.Add(resourceType);
                yield return resourceType;

                foreach (ResourceProperty property in resourceType.PropertiesDeclaredOnThisType)
                {
                    // Complex types can be refered to by either complex properties or by collection properties.
                    ResourceType candidateResourceType = property.ResourceType;
                    if (candidateResourceType.ResourceTypeKind == ResourceTypeKind.Collection)
                    {
                        Debug.Assert(candidateResourceType is CollectionResourceType, "Collection resource type must be an instance of CollectionResourceType.");
                        CollectionResourceType collectionResourceType = (CollectionResourceType)candidateResourceType;

                        // We have to check for derived complex types in collection property here since we can't do that during metadata creation
                        // The ability to determine if a given type has derived types requires the IDSMP in hand (and thus all ResourceType methods are out of question)
                        // We could verify this in the provider wrapper before we return a ResourceType, but that would mean walking all its properties
                        //   because collection types are not returned as part of the Types collection.
                        // Since we're going ot fail in serializer/deserializer if we hit this anyway, and we kind of require services to test their $metadata
                        //   it's OK to perform this check here.
                        this.ValidateCollectionResourceType(collectionResourceType);

                        candidateResourceType = collectionResourceType.ItemType;
                        Debug.Assert(
                            candidateResourceType.ResourceTypeKind == ResourceTypeKind.Primitive || candidateResourceType.ResourceTypeKind == ResourceTypeKind.ComplexType,
                            "Only collections of primitive or resource types are allowed.");
                    }

                    if (candidateResourceType.ResourceTypeKind == ResourceTypeKind.ComplexType)
                    {
                        foreach (ResourceType complexType in this.GetResourceTypeAndReachableComplexTypes(candidateResourceType, visitedTypes))
                        {
                            yield return complexType;
                        }
                    }
                }
            }
        }

        /// <summary>Get all reachable resource types from a resource set</summary>
        /// <param name="resourceSet">resource set to inspect</param>
        /// <param name="visitedTypes">Hash set to make sure we only return unvisited types.</param>
        /// <returns>List of reachable resource types from the given resource set</returns>
        private IEnumerable<ResourceType> GetReachableTypesFromSet(ResourceSetWrapper resourceSet, HashSet<ResourceType> visitedTypes)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");
            Debug.Assert(resourceSet.ResourceType != null, "resourceSet.ResourceType != null");
            Debug.Assert(visitedTypes != null, "visitedTypes != null");

            // Derived types of a visible type are visible
            if (this.HasDerivedTypes(resourceSet.ResourceType))
            {
                foreach (ResourceType derivedType in this.GetDerivedTypes(resourceSet.ResourceType))
                {
                    foreach (ResourceType type in this.GetResourceTypeAndReachableComplexTypes(derivedType, visitedTypes))
                    {
                        yield return type;
                    }
                }
            }

            // Base types of a visible type are visible
            ResourceType resourceType = resourceSet.ResourceType;
            while (resourceType != null)
            {
                foreach (ResourceType type in this.GetResourceTypeAndReachableComplexTypes(resourceType, visitedTypes))
                {
                    yield return type;
                }

                resourceType = resourceType.BaseType;
            }
        }

        /// <summary>Get all complex types reachable by the given service operation.</summary>
        /// <param name="operation">Operation to inspect</param>
        /// <param name="visitedTypes">Hash set to make sure we only return unvisited types.</param>
        /// <returns>List of reachable complex types from the given service operation.</returns>
        private IEnumerable<ResourceType> GetReachableComplexTypesFromOperation(OperationWrapper operation, HashSet<ResourceType> visitedTypes)
        {
            Debug.Assert(operation != null, "operation != null");
            Debug.Assert(visitedTypes != null, "visitedTypes != null");

            List<ResourceType> complexTypes = new List<ResourceType>();
            if (operation.ResultType != null && operation.ResultType.ResourceTypeKind == ResourceTypeKind.ComplexType)
            {
                complexTypes.Add(operation.ResultType);
            }

            complexTypes.AddRange(operation.Parameters.Where(p => p.ParameterType.ResourceTypeKind == ResourceTypeKind.ComplexType).Select(p => p.ParameterType));
            foreach (ResourceType complexType in complexTypes)
            {
                foreach (ResourceType reachableComplexType in this.GetResourceTypeAndReachableComplexTypes(complexType, visitedTypes))
                {
                    yield return reachableComplexType;
                }
            }
        }

        /// <summary>
        /// Validates a collection resource type. Checks that it doesn't use derived complex types as its items.
        /// </summary>
        /// <param name="collectionResourceType">The <see cref="CollectionResourceType"/> to check.</param>
        private void ValidateCollectionResourceType(CollectionResourceType collectionResourceType)
        {
            Debug.Assert(collectionResourceType != null, "collectionResourceType != null");
            Debug.Assert(collectionResourceType.ItemType != ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream)), "collectionResourceType.ItemType != ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream))");

            if (collectionResourceType.ItemType.ResourceTypeKind == ResourceTypeKind.ComplexType)
            {
                if (this.HasDerivedTypes(collectionResourceType.ItemType))
                {
                    throw new DataServiceException(500, s.Strings.DataServiceProviderWrapper_CollectionOfComplexTypeWithDerivedTypes(collectionResourceType.ItemType.FullName));
                }
            }
        }

        /// <summary>Validates that <paramref name="resourceType"/> is cached and read only.</summary>
        /// <param name="resourceType">Resource type to be validated.</param>
        /// <returns>Validated resource type, null if the resource type is not supposed to be visible.</returns>
        private ResourceType ValidateResourceType(ResourceType resourceType)
        {
            if (resourceType != null)
            {
                // For IDSP, we want to make sure the metadata object instance stay the same within
                // a request because we do reference comparisons.  Note the provider can return 
                // different metadata instances within the same request.  The the Validate*() methods
                // will make sure to return the first cached instance.
                ResourceType cachedType;
                if (this.VisibleTypeCache.TryGetValue(resourceType.FullName, out cachedType))
                {
                    return cachedType;
                }

                ValidateResourceTypeReadOnly(resourceType);
                AssertCacheNotPreloaded(this);
                this.VisibleTypeCache[resourceType.FullName] = resourceType;

                // Whenever we encounter a new resource type, we need to ensure that the metadata is 
                // complaint with the MPV set in the configuration.
                Version requiredFeatureVersion = resourceType.GetMinimumProtocolVersion();
                Version maxProtocolVersion = this.Configuration.DataServiceBehavior.MaxProtocolVersion.ToVersion();
                VersionUtil.CheckMaxProtocolVersion(requiredFeatureVersion, maxProtocolVersion);

                return resourceType;
            }

            return null;
        }
        #endregion Private Methods
    }
}

//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Annotations;
    using Microsoft.Data.Edm.Csdl;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.Edm.Library.Annotations;
    using Microsoft.Data.OData;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// An <see cref="IEdmModel"/> implementation backed by an IDSMP metadata provider.
    /// </summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Type coupling because of mapping from all the IDSMP types to the EdmLib types.")]
    internal class MetadataProviderEdmModel : EdmElement, IEdmModel, IODataUriParserModelExtensions
    {
        /// <summary>Default nullability for entity types.</summary>
        private const bool EntityTypeDefaultNullability = true;

        /// <summary>Default nullability for V3 primitive or complex collection property types.</summary>
        private const bool PrimitiveOrComplexCollectionTypeDefaultNullability = true;

        /// <summary>Default nullability for V3 primitive or complex collection property item types.</summary>
        private const bool PrimitiveOrComplexCollectionItemTypeDefaultNullability = false;

        /// <summary>Default nullability for collection types used for collection navigation properties, and other collections returned from service operations.</summary>
        private const bool EntityPrimitiveOrComplexCollectionTypeDefaultNullability = true;

        /// <summary>Data service metadata provider instance.</summary>
        private readonly DataServiceProviderWrapper metadataProvider;

        /// <summary>The stream provider wrapper instance.</summary>
        private readonly DataServiceStreamProviderWrapper streamProviderWrapper;

        /// <summary>The action provider wrapper instance.</summary>
        private readonly DataServiceActionProviderWrapper actionProviderWrapper;

        /// <summary>The annotations manager.</summary>
        private readonly IEdmDirectValueAnnotationsManager directValueAnnotationsManager = new EdmDirectValueAnnotationsManager();

        /// <summary>Referenced core model.</summary>
        private readonly IEdmModel coreModel = EdmCoreModel.Instance;

        /// <summary>Referenced models.</summary>
        private readonly IEnumerable<IEdmModel> referencedModels = new IEdmModel[] { EdmCoreModel.Instance };

        /// <summary>
        /// The cache of all the schema types.
        /// </summary>
        /// <remarks>
        /// The schema type cache contains the entity types and complex types of the schema.
        /// Functions are part of the schema but currently not supported (outside the built-in EDM library functions).
        /// </remarks>
        private readonly Dictionary<string, IEdmSchemaType> schemaTypeCache;

        /// <summary>
        /// The cache of types that derive directly from a given base type.
        /// </summary>
        private readonly Dictionary<IEdmStructuredType, List<IEdmStructuredType>> derivedTypeMappings;

        /// <summary>
        /// The cache of all the collection types.
        /// </summary>
        private readonly Dictionary<string, HashSet<ResourceType>> resourceTypesPerNamespaceCache;

        /// <summary>
        /// The cache of all the entity containers.
        /// </summary>
        /// <remarks>
        /// The entity container cache contains the entity containers with the entity sets, association sets and function imports of the model.
        /// </remarks>
        private readonly Dictionary<string, MetadataProviderEdmEntityContainer> entityContainerCache;

        /// <summary>
        /// The cache of all the primitive or complex collection types.
        /// </summary>
        /// <remarks>The resource types in the cache are retrieved from the data service provider wrapper which ensures that they are atomized.</remarks>
        private readonly Dictionary<ResourceType, IEdmCollectionType> primitiveOrComplexCollectionTypeCache;

        /// <summary>
        /// The cache of all the collection types used for collection navigation properties, and other collections returned from service operations.
        /// </summary>
        /// <remarks>The resource types in the cache are retrieved from the data service provider wrapper which ensures that they are atomized.</remarks>
        private readonly Dictionary<ResourceType, IEdmCollectionType> entityPrimitiveOrComplexCollectionTypeCache;

        /// <summary>
        /// Cache of association set names by key
        /// </summary>
        /// <remarks>This allows detection of some error conditions, as well as allows us to skip fixing up a navigation property if its partner has been fixed up already.</remarks>
        private readonly Dictionary<string, string> associationSetByKeyCache;

        /// <summary>The state of the metadata cache.</summary>
        /// <remarks>This field indicates what parts of the metadata have been fully cached.</remarks>
        private MetadataProviderState cacheState;

        /// <summary>
        /// The state of the current metadata materialization (what metadata is currently being materialized).
        /// This is used to track that only methods are called that are expected in a given 
        /// materialization state.
        /// </summary>
        private MetadataProviderState materializationState;

        /// <summary>
        /// The current mode in which the model is being used.
        /// </summary>
        private MetadataProviderEdmModelMode currentMode = MetadataProviderEdmModelMode.Serialization;

        /// <summary>Constructs a <see cref="MetadataProviderEdmModel"/> which wraps the given <see cref="DataServiceProviderWrapper"/> instance.</summary>
        /// <param name="provider"><see cref="DataServiceProviderWrapper"/> instance.</param>
        /// <param name="streamProviderWrapper">The stream provider wrapper instance.</param>
        /// <param name="actionProviderWrapper">The action provider wrapper instance.</param>
        internal MetadataProviderEdmModel(DataServiceProviderWrapper provider, DataServiceStreamProviderWrapper streamProviderWrapper, DataServiceActionProviderWrapper actionProviderWrapper)
        {
            Debug.Assert(provider != null, "provider != null");

            this.metadataProvider = provider;
            this.streamProviderWrapper = streamProviderWrapper;
            this.actionProviderWrapper = actionProviderWrapper;
            this.schemaTypeCache = new Dictionary<string, IEdmSchemaType>(StringComparer.Ordinal);
            this.resourceTypesPerNamespaceCache = new Dictionary<string, HashSet<ResourceType>>(StringComparer.Ordinal);
            this.entityContainerCache = new Dictionary<string, MetadataProviderEdmEntityContainer>(StringComparer.Ordinal);
            this.primitiveOrComplexCollectionTypeCache = new Dictionary<ResourceType, IEdmCollectionType>(EqualityComparer<ResourceType>.Default);
            this.entityPrimitiveOrComplexCollectionTypeCache = new Dictionary<ResourceType, IEdmCollectionType>(EqualityComparer<ResourceType>.Default);
            this.derivedTypeMappings = new Dictionary<IEdmStructuredType, List<IEdmStructuredType>>(EqualityComparer<IEdmStructuredType>.Default);
            this.associationSetByKeyCache = new Dictionary<string, string>(StringComparer.Ordinal);

            Version dataServiceVersion = this.metadataProvider.Configuration.DataServiceBehavior.MaxProtocolVersion.ToVersion();
            this.SetDataServiceVersion(dataServiceVersion);
            Version edmVersion = null;
            if (!MetadataProviderUtils.DataServiceEdmVersionMap.TryGetValue(dataServiceVersion, out edmVersion))
            {
                this.SetEdmVersion(Microsoft.Data.Edm.Library.EdmConstants.EdmVersionLatest);
            }
            else
            {
                this.SetEdmVersion(edmVersion);
            }

            // Initialize the minimum Edm Metadata Version to 1.0.
            this.MinMetadataEdmSchemaVersion = Microsoft.Data.Edm.Library.EdmConstants.EdmVersion1;

            this.AnnotationsCache = new VocabularyAnnotationCache(this);
        }

        /// <summary>
        /// Gets the vocabulary annoations defined in the model.
        /// </summary>
        public IEnumerable<IEdmVocabularyAnnotation> VocabularyAnnotations
        {
            get
            {
                this.AssertCacheState(MetadataProviderState.Full);
                return this.AnnotationsCache.VocabularyAnnotations;
            }
        }

        /// <summary>
        /// Gets the referenced models.
        /// </summary>
        public IEnumerable<IEdmModel> ReferencedModels
        {
            get { return this.referencedModels; }
        }

        /// <summary>
        /// Gets the collection of schema elements that are contained in this model.
        /// </summary>
        /// <remarks>
        /// Materialization state: none required. This property should only be called when 
        ///     it is ok to materialize the full metadata.
        /// Cache state: 'full' after the property returns.
        /// </remarks>
        public IEnumerable<IEdmSchemaElement> SchemaElements
        {
            get
            {
                // This should be called only in $metadata scenarios
                this.AssertCacheState(MetadataProviderState.Full);

                // returns the entity types and complex types
                foreach (IEdmSchemaType schemaType in this.schemaTypeCache.Values)
                {
                    yield return schemaType;
                }

                foreach (IEdmEntityContainer container in this.entityContainerCache.Values.Distinct())
                {
                    yield return container;
                }
            }
        }

        /// <summary>
        /// Gets the model's annotations manager.
        /// </summary>
        public IEdmDirectValueAnnotationsManager DirectValueAnnotationsManager
        {
            get { return this.directValueAnnotationsManager; }
        }

        /// <summary>
        /// The IDSMP metadata provider backing this model.
        /// </summary>
        internal DataServiceProviderWrapper MetadataProvider
        {
            get { return this.metadataProvider; }
        }

        /// <summary>
        /// Gets the vocabulary annotations cache for this model.
        /// </summary>
        internal VocabularyAnnotationCache AnnotationsCache { get; private set; }

        /// <summary>
        /// The current mode in which the model is being used.
        /// </summary>
        internal MetadataProviderEdmModelMode Mode
        {
            get { return this.currentMode; }
            set { this.currentMode = value; }
        }

        /// <summary>
        /// Minimum Edm Metadata Version. This is calculated during full metadata loading.
        /// </summary>
        internal Version MinMetadataEdmSchemaVersion
        {
            get;
            private set;
        }

        /// <summary>
        /// Whether we are serializing $metadata document, and therefor may need to hide certain properties.
        /// </summary>
        private bool ShouldCheckForPropertyVisibility
        {
            get { return this.HasCacheState(MetadataProviderState.Full) || this.HasMaterializationState(MetadataProviderState.Full); }
        }

        /// <summary>
        /// Searches for an entity container with the given name in this model and returns null if no such entity container exists.
        /// </summary>
        /// <param name="name">The name of the entity container being found.</param>
        /// <returns>The requested entity container, or null if no such entity container exists.</returns>
        /// <remarks>
        /// Materialization state: there is currently no scenario that would call this method 
        ///     (it is needed as part of the interface implementation). We don't require
        ///     a materialization state.
        /// Cache state: 'EntityContainers' after the method returned.
        /// </remarks>
        public IEdmEntityContainer FindDeclaredEntityContainer(string name)
        {
            WebUtil.CheckStringArgumentNullOrEmpty(name, "name");
            return this.FindExistingEntityContainer(name);
        }

        /// <summary>
        /// Searches for a schema type with the given name in this model and returns null if no such schema element exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the schema element being found.</param>
        /// <returns>The requested schema element, or null if no such schema element exists.</returns>
        /// <remarks>
        /// This method only supports by-name lookup for entity types, complex types and primitive types. 
        /// By-name lookup for association types is not supported.
        /// 
        /// Materialization state: none required. No change in materialization state.
        /// Cache state: none required. No change in cache state.
        /// </remarks>
        public IEdmSchemaType FindDeclaredType(string qualifiedName)
        {
            WebUtil.CheckStringArgumentNullOrEmpty(qualifiedName, "qualifiedName");

            // NOTE: Schema types are entity types, complex types, primitive types (!)
            IEdmSchemaType schemaType;
            if (this.schemaTypeCache.TryGetValue(qualifiedName, out schemaType))
            {
                return schemaType;
            }

            // If the type is primitive or otherwise exists in the core model, do not attempt to resolve it.
            if (this.coreModel.FindDeclaredType(qualifiedName) != null)
            {
                return null;
            }

            if (this.cacheState == MetadataProviderState.Full)
            {
                // The cache is completely filled but we did not find a type with the specified name.
                return null;
            }

            // For open properties, we might need to go to the metadata provider to find the type
            // since it will not have got lazy loaded.
            ResourceType resourceType = this.metadataProvider.TryResolveResourceType(qualifiedName);
            if (resourceType != null)
            {
                return this.EnsureSchemaType(resourceType);
            }

            return null;
        }

        /// <summary>
        /// Searches for functions with the given name in this model and returns an empty enumerable if no such function exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the function being found.</param>
        /// <returns>A set functions sharing the specified qualified name, or an empty enumerable if no such function exists.</returns>
        /// <remarks>
        /// Materialization state: none required. No change in materialization state.
        /// Cache state: none required. No change in cache state.
        /// </remarks>
        public IEnumerable<IEdmFunction> FindDeclaredFunctions(string qualifiedName)
        {
            // Model functions are not supported.
            return Enumerable.Empty<IEdmFunction>();
        }

        /// <summary>
        /// Searches for a value term with the given name in this model and returns null if no such value term exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the value term being found.</param>
        /// <returns>The requested value term, or null if no such value term exists.</returns>
        /// <remarks>
        /// Materialization state: none required. No change in materialization state.
        /// Cache state: none required. No change in cache state.
        /// </remarks>
        public IEdmValueTerm FindDeclaredValueTerm(string qualifiedName)
        {
            return null;
        }

        /// <summary>
        /// Searches for vocabulary annotations specified by this model or a referenced model for a given element.
        /// </summary>
        /// <param name="element">The annotated element.</param>
        /// <returns>The vocabulary annotations for the element.</returns>
        /// <remarks>
        /// Materialization state: none required. No change in materialization state.
        /// Cache state: none required. No change in cache state.
        /// </remarks>
        public IEnumerable<IEdmVocabularyAnnotation> FindDeclaredVocabularyAnnotations(IEdmVocabularyAnnotatable element)
        {
            this.AssertCacheState(MetadataProviderState.Full);
            return this.AnnotationsCache.FindDeclaredVocabularyAnnotations(element);
        }

        /// <summary>
        /// Finds a list of types that derive directly from the supplied type.
        /// </summary>
        /// <param name="baseType">The base type that derived types are being searched for.</param>
        /// <returns>A list of types from this model that derive directly from the given type.</returns>
        /// <remarks>
        /// Materialization state: none required. This property should only be called when 
        ///     it is ok to materialize the full metadata.
        /// Cache state: 'full' after the property returns.
        /// </remarks>
        public IEnumerable<IEdmStructuredType> FindDirectlyDerivedTypes(IEdmStructuredType baseType)
        {
            // This should be called only in $metadata scenarios
            this.AssertCacheState(MetadataProviderState.Full);

            List<IEdmStructuredType> types;
            if (this.derivedTypeMappings.TryGetValue(baseType, out types))
            {
                return types;
            }

            return Enumerable.Empty<IEdmStructuredType>();
        }

        /// <summary>
        /// Finds all function imports with the given name which are bindable to an instance of the giving binding type or a more derived type.
        /// </summary>
        /// <param name="bindingType">The binding entity type.</param>
        /// <param name="functionImportName">The name of the function imports to find. May be qualified with an entity container name.</param>
        /// <returns>The function imports that match the search criteria.</returns>
        IEnumerable<IEdmFunctionImport> IODataUriParserModelExtensions.FindFunctionImportsByBindingParameterTypeHierarchy(IEdmType bindingType, string functionImportName)
        {
            DebugUtils.CheckNoExternalCallers();
            if (this.actionProviderWrapper != null)
            {
                bool nameIsContainerQualified;
                var operationName = this.metadataProvider.GetNameFromContainerQualifiedName(functionImportName, out nameIsContainerQualified);

                ResourceType bindingResourceType = MetadataProviderUtils.GetResourceType(bindingType);
                MetadataProviderEdmEntityContainer defaultEntityContainer = this.EnsureDefaultEntityContainer();
                return this.actionProviderWrapper.GetActionsBoundToAnyTypeInHierarchy(bindingResourceType)
                    .Where(operation => operation.Name == operationName)
                    .Select(defaultEntityContainer.EnsureFunctionImport);
            }

            return null;
        }

        /// <summary>
        /// Finds an entity set given a name that may be container qualified. If no container name is provided, the default container should be used.
        /// </summary>
        /// <param name="containerQualifiedEntitySetName">The name which might be container qualified. If no container name is provided, the default container should be used.</param>
        /// <returns>The entity set if one was found or null.</returns>
        IEdmEntitySet IODataUriParserModelExtensions.FindEntitySetFromContainerQualifiedName(string containerQualifiedEntitySetName)
        {
            var resourceSet = this.metadataProvider.TryResolveResourceSet(containerQualifiedEntitySetName);
            if (resourceSet == null)
            {
                return null;
            }

            return this.EnsureEntitySet(resourceSet);
        }

        /// <summary>
        /// Finds a service operation for the given name.
        /// </summary>
        /// <param name="serviceOperationName">The name of the service operation to find. May be qualified with an entity container name.</param>
        /// <returns>The function import representing a service operation or null if one could not be found with the given name.</returns>
        public IEdmFunctionImport FindServiceOperation(string serviceOperationName)
        {
            var serviceOperation = this.metadataProvider.TryResolveServiceOperation(serviceOperationName);
            if (serviceOperation == null)
            {
                return null;
            }

            return this.EnsureDefaultEntityContainer().EnsureFunctionImport(serviceOperation);
        }

        /// <summary>
        /// Finds a function or action bound to the specific type with the given name.
        /// </summary>
        /// <param name="bindingType">The binding type.</param>
        /// <param name="functionImportName">The name of the function imports to find. May be qualified with an entity container name.</param>
        /// <param name="nonBindingParameterNamesFromUri">The parameter names of the non-binding parameters, if provided in the request URI.</param>
        /// <returns>The function import that matches the search criteria or null if there was no match.</returns>
        public IEdmFunctionImport FindFunctionImportByBindingParameterType(IEdmType bindingType, string functionImportName, IEnumerable<string> nonBindingParameterNamesFromUri)
        {
            if (nonBindingParameterNamesFromUri.Any())
            {
                // parameter names in the URL indicate that a function is being invoked, not an action.
                return null;
            }

            if (this.actionProviderWrapper != null)
            {
                bool nameIsContainerQualified;
                var operationName = this.metadataProvider.GetNameFromContainerQualifiedName(functionImportName, out nameIsContainerQualified);
                var operation = this.actionProviderWrapper.TryResolveServiceAction(operationName, MetadataProviderUtils.GetResourceType(bindingType));
                if (operation != null)
                {
                    return this.EnsureDefaultEntityContainer().EnsureFunctionImport(operation);
                }
            }

            return null;
        }

        /// <summary>
        /// Ensure the EDM schema type for the specified <paramref name="resourceType"/>.
        /// </summary>
        /// <param name="resourceType">The resource type for which to create an EDM schema type.</param>
        /// <returns>The EDM schema type found or created for the <paramref name="resourceType"/>.</returns>
        /// <remarks>
        /// Materialization state: none required. No change in materialization state.
        /// Cache state: none required. No change in cache state.
        /// </remarks>
        internal IEdmSchemaType EnsureSchemaType(ResourceType resourceType)
        {
            Debug.Assert(resourceType != null, "resourceType != null");
            Debug.Assert(resourceType.IsReadOnly, "resourceType.IsReadOnly");

            ResourceTypeKind resourceTypeKind = resourceType.ResourceTypeKind;
            Debug.Assert(resourceTypeKind != ResourceTypeKind.EntityCollection, "Entity collection types are not valid schema types.");
            Debug.Assert(resourceTypeKind != ResourceTypeKind.Collection, "Collections are not valid schema types.");

            if (resourceTypeKind == ResourceTypeKind.Primitive)
            {
                return MetadataProviderUtils.CreatePrimitiveTypeReference(resourceType, null).PrimitiveDefinition();
            }

            string namespaceName = this.GetTypeNamespace(resourceType);
            string schemaTypeKey = ComputeSchemaTypeCacheKey(namespaceName, resourceType);

            IEdmSchemaType schemaType;
            if (this.schemaTypeCache.TryGetValue(schemaTypeKey, out schemaType))
            {
                return schemaType;
            }

            // We did not find the schema type in the cache - create and cache it.
            IEdmSchemaType edmType;
            switch (resourceTypeKind)
            {
                case ResourceTypeKind.EntityType:
                    edmType = this.AddEntityType(resourceType, namespaceName);
                    break;

                case ResourceTypeKind.ComplexType:
                    edmType = this.AddComplexType(resourceType, namespaceName);
                    break;

                default:
                    throw new InvalidOperationException(System.Data.Services.Strings.MetadataProviderEdmModel_UnsupportedSchemaTypeKind(resourceTypeKind.ToString()));
            }

            return edmType;
        }

        /// <summary>
        /// Creates an <see cref="IEdmTypeReference"/> for the specified <paramref name="resourceType"/>.
        /// </summary>
        /// <param name="resourceType">The resource type to create an <see cref="IEdmTypeReference"/> for.</param>
        /// <param name="customAnnotations">The optional annotations for the resource type; 
        /// the annotations can contain facets that need to be applied to the type reference.</param>
        /// <returns>An <see cref="IEdmTypeReference"/> instance for the <paramref name="resourceType"/>.</returns>
        /// <remarks>
        /// Materialization state: none required. No change in materialization state.
        /// Cache state: none required. No change in cache state.
        /// </remarks>
        internal IEdmTypeReference EnsureTypeReference(ResourceType resourceType, List<KeyValuePair<string, object>> customAnnotations)
        {
            Debug.Assert(resourceType != null, "resourceType != null");

            switch (resourceType.ResourceTypeKind)
            {
                case ResourceTypeKind.EntityType:
                    {
                        bool? nullableFacet = MetadataProviderUtils.GetAndRemoveNullableFacet(customAnnotations);
                        bool isNullable = nullableFacet.HasValue ? nullableFacet.Value : EntityTypeDefaultNullability;
                        return this.EnsureSchemaType(resourceType).ToTypeReference(isNullable);
                    }

                case ResourceTypeKind.ComplexType:
                    {
                        bool? nullableFacet = MetadataProviderUtils.GetAndRemoveNullableFacet(customAnnotations);

                        // TODO: Astoria treats all properties with complex type as nullable, CSDL spec says in Section 2.1.3 that complex properties must not be nullable
                        Version dataServiceVersion = this.GetDataServiceVersion();
                        Debug.Assert(dataServiceVersion != null, "Data service version on the Edm model should not be null.");

                        bool complexTypeDefaultNullability = dataServiceVersion >= VersionUtil.Version3Dot0;
                        Version edmVersion = this.GetEdmVersion();

                        // If the edm version is < 3.0, then override what MPV says and treat all complex properties as non-nullable.
                        if (edmVersion != null && edmVersion < MetadataEdmSchemaVersion.Version3Dot0.ToVersion())
                        {
                            complexTypeDefaultNullability = false;
                        }

                        bool isNullable = nullableFacet.HasValue ? nullableFacet.Value : complexTypeDefaultNullability;
                        return this.EnsureSchemaType(resourceType).ToTypeReference(isNullable);
                    }

                case ResourceTypeKind.Primitive:
                    return MetadataProviderUtils.CreatePrimitiveTypeReference(resourceType, customAnnotations);

                case ResourceTypeKind.Collection:
                    return this.EnsurePrimitiveOrComplexCollectionType(resourceType, customAnnotations).ToTypeReference(PrimitiveOrComplexCollectionTypeDefaultNullability);

                case ResourceTypeKind.EntityCollection:
                    return this.EnsureEntityCollectionType(resourceType, customAnnotations).ToTypeReference(EntityPrimitiveOrComplexCollectionTypeDefaultNullability);

                default:
                    throw new InvalidOperationException(System.Data.Services.Strings.MetadataProviderEdmModel_UnsupportedResourceTypeKind(resourceType.ResourceTypeKind.ToString()));
            }
        }

        /// <summary>
        /// Gets or creates a collection type reference for the <paramref name="itemResourceType"/>.
        /// </summary>
        /// <param name="itemResourceType">The item resource type to create an EDM collection type reference for.</param>
        /// <param name="collectionResourceType">The collection resource type to create an EDM collection type for.</param>
        /// <param name="customAnnotations">The optional annotations for the resource type; the annotations can contain facets that need to be applied to the type reference.</param>
        /// <returns>A collection type reference for the <paramref name="itemResourceType"/> item type.</returns>
        /// <remarks>
        /// Materialization state: none required. No change in materialization state.
        /// Cache state: none required. No change in cache state.
        /// </remarks>
        internal IEdmTypeReference EnsureEntityPrimitiveOrComplexCollectionTypeReference(ResourceType itemResourceType, ResourceType collectionResourceType, List<KeyValuePair<string, object>> customAnnotations)
        {
            Debug.Assert(itemResourceType != null, "itemResourceType != null");

            IEdmCollectionType collectionType = this.EnsureCollectionItemTypeIsEntityPrimitiveOrComplex(itemResourceType, collectionResourceType, customAnnotations);
            return collectionType.ToTypeReference(EntityPrimitiveOrComplexCollectionTypeDefaultNullability);
        }

        /// <summary>
        /// Gets or creates the default entity container.
        /// </summary>
        /// <returns>The default entity container.</returns>
        internal MetadataProviderEdmEntityContainer EnsureDefaultEntityContainer()
        {
            // Get the default container name.
            string defaultContainerName = this.metadataProvider.ContainerName;
            Debug.Assert(!string.IsNullOrEmpty(defaultContainerName), "!string.IsNullOrEmpty(defaultContainerName)");

            MetadataProviderEdmEntityContainer defaultContainer;
            if (!this.entityContainerCache.TryGetValue(defaultContainerName, out defaultContainer))
            {
                defaultContainer = new MetadataProviderEdmEntityContainer(this, defaultContainerName, this.GetContainerNamespace());
                this.SetIsDefaultEntityContainer(defaultContainer, true);
                MetadataProviderUtils.ConvertCustomAnnotations(this, this.metadataProvider.GetEntityContainerAnnotations(defaultContainerName), defaultContainer);
                this.entityContainerCache.Add(defaultContainerName, defaultContainer);
                this.entityContainerCache.Add(defaultContainer.FullName(), defaultContainer);
            }

            Debug.Assert(defaultContainer != null, "defaultContainer != null");
            return defaultContainer;
        }

        /// <summary>
        /// Add the given entity set to the model.
        /// </summary>
        /// <param name="resourceSet">ResourceSetWrapper instance to add.</param>
        /// <returns>an instance of IEdmEntitySet for the given <paramref name="resourceSet"/>.</returns>
        internal IEdmEntitySet EnsureEntitySet(ResourceSetWrapper resourceSet)
        {
            this.EnsureDefaultEntityContainer();

            // Make sure if the entity container exists
            string entityContainerName = resourceSet.EntityContainerName ?? this.metadataProvider.ContainerName;
            MetadataProviderEdmEntityContainer entityContainer = this.FindExistingEntityContainer(entityContainerName);
            if (entityContainer == null)
            {
                entityContainer = new MetadataProviderEdmEntityContainer(this, entityContainerName, this.GetContainerNamespace());
                MetadataProviderUtils.ConvertCustomAnnotations(this, this.metadataProvider.GetEntityContainerAnnotations(entityContainerName), entityContainer);
                this.entityContainerCache.Add(entityContainerName, entityContainer);
                this.entityContainerCache.Add(entityContainer.FullName(), entityContainer);
            }

            string entitySetName = MetadataProviderUtils.GetEntitySetName(resourceSet.ResourceSet);
            IEdmEntitySet entitySet = entityContainer.FindEntitySet(entitySetName);
            if (entitySet == null)
            {
                entitySet = entityContainer.AddEntitySet(entitySetName, resourceSet);
            }

            return entitySet;
        }

        /// <summary>
        /// Add the given service operation to the model.
        /// </summary>
        /// <param name="operationWrapper">ServiceOperationWrapper instance to add.</param>
        internal void AddServiceOperation(OperationWrapper operationWrapper)
        {
            Debug.Assert(operationWrapper != null, "operationWrapper != null");

            // All the service operations live in the default entity container
            MetadataProviderEdmEntityContainer defaultEntityContainer = this.EnsureDefaultEntityContainer();
            defaultEntityContainer.EnsureFunctionImport(operationWrapper);
        }

        /// <summary>
        /// Ensures all of the metadata is loaded. Should only be used for $metadata
        /// </summary>
        internal void EnsureFullMetadataLoaded()
        {
            this.RunInState(this.EnsureFullMetadata, MetadataProviderState.Full);
        }

        /// <summary>
        /// Assert that the specified cache state has been reached (or exceeded).
        /// </summary>
        /// <param name="state">The <see cref="MetadataProviderState"/> that has to be reached.</param>
        [Conditional("DEBUG")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Instance fields are used in debug; this method will not be called in retail.")]
        internal void AssertCacheState(MetadataProviderState state)
        {
#if DEBUG
            Debug.Assert(state != MetadataProviderState.Incremental, "Should never attempt to assert the 'Incremental' state.");

            if (this.cacheState < state)
            {
                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "The current cache state is '{0}' but expected at least '{1}'.",
                    this.materializationState.ToString(),
                    state.ToString());
                Debug.Assert(false, message);
            }
#endif
        }

        /// <summary>
        /// Asserts that the models caches are empty and that no types, containers, sets, etc have been built yet.
        /// </summary>
        [Conditional("DEBUG")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Instance fields are used in debug; this method will not be called in retail.")]
        internal void AssertCacheEmpty()
        {
#if DEBUG
            Debug.Assert(this.schemaTypeCache.Count == 0, "Cache should be empty.");
            Debug.Assert(this.resourceTypesPerNamespaceCache.Count == 0, "Cache should be empty.");
            Debug.Assert(this.entityContainerCache.Count == 0, "Cache should be empty.");
            Debug.Assert(this.primitiveOrComplexCollectionTypeCache.Count == 0, "Cache should be empty.");
            Debug.Assert(this.entityPrimitiveOrComplexCollectionTypeCache.Count == 0, "Cache should be empty.");
            Debug.Assert(this.derivedTypeMappings.Count == 0, "Cache should be empty.");
            Debug.Assert(this.associationSetByKeyCache.Count == 0, "Cache should be empty.");
#endif
        }

        /// <summary>
        /// Assert that a specified materialization state has been reached (or exceeded).
        /// </summary>
        /// <param name="state">The <see cref="MetadataProviderState"/> that has to be reached.</param>
        [Conditional("DEBUG")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Instance fields are used in debug; this method will not be called in retail.")]
        internal void AssertMaterializationState(MetadataProviderState state)
        {
#if DEBUG
            Debug.Assert(state != MetadataProviderState.Incremental, "Should never attempt to assert the 'Incremental' state.");

            if (this.materializationState < state)
            {
                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "The current materialization state is '{0}' but expected at least '{1}'.",
                    this.materializationState.ToString(),
                    state.ToString());
                Debug.Assert(false, message);
            }
#endif
        }

        /// <summary>
        /// Fills in the rest of required information for navigation properties.
        /// </summary>
        /// <param name="resourceSet">Resource set to inspect.</param>
        /// <param name="resourceType">Resource type to inspect.</param>
        /// <param name="navigationProperty">Navigation property to inspect.</param>
        internal void PairUpNavigationProperty(ResourceSetWrapper resourceSet, ResourceType resourceType, ResourceProperty navigationProperty)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");
            Debug.Assert(resourceType != null, "resourceType != null");
            Debug.Assert(navigationProperty != null && navigationProperty.ResourceType.ResourceTypeKind == ResourceTypeKind.EntityType, "navigationProperty != null && navigationProperty.ResourceType.ResourceTypeKind == ResourceTypeKind.EntityType");
            Debug.Assert(resourceType.TryResolvePropertiesDeclaredOnThisTypeByName(navigationProperty.Name) != null, "navigationProperty must be declared on resourceType.");

            string associationSetKey = resourceSet.Name + '_' + resourceType.FullName + '_' + navigationProperty.Name;

            // Check the cache first; we might already have visited the partiner of this navigation property.
            if (this.associationSetByKeyCache.ContainsKey(associationSetKey))
            {
                return;
            }

            ResourceAssociationSet resourceAssociationSet = this.MetadataProvider.GetResourceAssociationSet(resourceSet, resourceType, navigationProperty);
            if (resourceAssociationSet != null)
            {
                ResourceAssociationSetEnd relatedEnd = resourceAssociationSet.GetRelatedResourceAssociationSetEnd(resourceSet, resourceType, navigationProperty);

                // If this is a two way relationship, GetResourceAssociationSet should return the same association set when called from either end.
                // For example, it's not valid to have the association sets {GoodCustomerSet} <-> {OrdersSet} and {BadCustomerSet} <-> {OrdersSet}
                // because starting from {OrderSet} we won't know which customers set to resolve to.
                if (relatedEnd.ResourceProperty != null)
                {
                    ResourceAssociationSet reverseAssociationSet = this.MetadataProvider.GetResourceAssociationSet(
                        this.MetadataProvider.ValidateResourceSet(relatedEnd.ResourceSet),
                        relatedEnd.ResourceType,
                        relatedEnd.ResourceProperty);

                    if (reverseAssociationSet == null || resourceAssociationSet.Name != reverseAssociationSet.Name)
                    {
                        throw new InvalidOperationException(System.Data.Services.Strings.ResourceAssociationSet_BidirectionalAssociationMustReturnSameResourceAssociationSetFromBothEnd);
                    }
                }

                // Cache the association set for the reverse direction.
                string reverseAssociationSetKey;
                if (relatedEnd.ResourceProperty != null)
                {
                    reverseAssociationSetKey = relatedEnd.ResourceSet.Name + '_' + relatedEnd.ResourceType.FullName + '_' + relatedEnd.ResourceProperty.Name;
                }
                else
                {
                    reverseAssociationSetKey = relatedEnd.ResourceSet.Name + "_Null_" + resourceType.FullName + '_' + navigationProperty.Name;
                }

                string conflictingAssociationSetName;
                if (this.associationSetByKeyCache.TryGetValue(reverseAssociationSetKey, out conflictingAssociationSetName))
                {
                    // If only one of the ends is already in the cache, we know that the provider is giving us inconsistant metadata.
                    // Make sure that if two or more AssociationSets refer to the same AssociationType, the ends must not refer to the same EntitySet.
                    // For CLR context, this could happen if multiple entity sets have entity types that have a common ancestor and the ancestor has a property of derived entity types.
                    throw new InvalidOperationException(System.Data.Services.Strings.ResourceAssociationSet_MultipleAssociationSetsForTheSameAssociationTypeMustNotReferToSameEndSets(conflictingAssociationSetName, resourceAssociationSet.Name, relatedEnd.ResourceSet.Name));
                }

                this.PairUpNavigationPropertyWithResourceAssociationSet(resourceAssociationSet);
                this.associationSetByKeyCache.Add(reverseAssociationSetKey, resourceAssociationSet.Name);
                this.associationSetByKeyCache.Add(associationSetKey, resourceAssociationSet.Name);
            }
        }

        /// <summary>
        /// Compute the cache key used to cache schema types (based on resource type names).
        /// </summary>
        /// <param name="namespaceName">The namespace name of the type.</param>
        /// <param name="resourceType">The resource type to use in the computation.</param>
        /// <returns>The cache key to be used for looking up and storing schema types.</returns>
        /// <remarks>
        /// This method is necessary because resource types don't always have a namespace. In such cases, the full name of the resource type is just its name.
        /// In EDM, however, the respective schema type will be created in the container namespace. As a result, we have to always include the namespace 
        /// in the key computation in order to find cached schema types for resource types.
        /// 
        /// Materialization state: none required. No change in materialization state.
        /// Cache state: none required. No change in cache state.
        /// </remarks>
        private static string ComputeSchemaTypeCacheKey(string namespaceName, ResourceType resourceType)
        {
            Debug.Assert(!string.IsNullOrEmpty(namespaceName), "!string.IsNullOrEmpty(namespaceName)");
            Debug.Assert(resourceType != null, "resourceType != null");

            return namespaceName + "." + resourceType.Name;
        }

        /// <summary>
        /// Gets or creates an <see cref="IEdmCollectionType"/> for the <paramref name="itemResourceType"/>.
        /// </summary>
        /// <param name="itemResourceType">The item resource type to create an EDM collection type for.</param>
        /// <param name="collectionResourceType">The collection resource type to create an EDM collection type for.</param>
        /// <param name="customAnnotations">The optional annotations for the resource type; the annotations can contain facets that need to be applied to the type reference.</param>
        /// <returns>An <see cref="IEdmCollectionType"/> instance for the <paramref name="itemResourceType"/> item type.</returns>
        /// <remarks>
        /// Materialization state: none required. No change in materialization state.
        /// Cache state: none required. No change in cache state.
        /// </remarks>
        private IEdmCollectionType EnsureCollectionItemTypeIsEntityPrimitiveOrComplex(ResourceType itemResourceType, ResourceType collectionResourceType, List<KeyValuePair<string, object>> customAnnotations)
        {
            Debug.Assert(itemResourceType != null, "itemResourceType != null");

            IEdmCollectionType collectionType;
            if (this.entityPrimitiveOrComplexCollectionTypeCache.TryGetValue(itemResourceType, out collectionType))
            {
                return collectionType;
            }

            IEdmTypeReference elementTypeReference;
            switch (itemResourceType.ResourceTypeKind)
            {
                case ResourceTypeKind.ComplexType:
                    elementTypeReference = this.EnsureTypeReference(itemResourceType, customAnnotations);
                    break;
                case ResourceTypeKind.Primitive:
                    elementTypeReference = MetadataProviderUtils.CreatePrimitiveTypeReference(itemResourceType, customAnnotations);
                    break;
                case ResourceTypeKind.EntityType:
                    elementTypeReference = this.EnsureTypeReference(itemResourceType, customAnnotations);
                    break;
                case ResourceTypeKind.Collection:       // fall through
                case ResourceTypeKind.EntityCollection: // fall through
                default:
                    throw new InvalidOperationException(System.Data.Services.Strings.MetadataProviderEdmModel_UnsupportedCollectionItemType_EntityPrimitiveOrComplex(itemResourceType.ResourceTypeKind.ToString()));
            }

            collectionType = new MetadataProviderEdmCollectionType(collectionResourceType, elementTypeReference);
            this.entityPrimitiveOrComplexCollectionTypeCache.Add(itemResourceType, collectionType);
            return collectionType;
        }

        /// <summary>
        /// Gets or creates an <see cref="IEdmCollectionType"/> for the <paramref name="itemResourceType"/>.
        /// </summary>
        /// <param name="collectionResourceType">The collection resource type that the edm type is being created from.</param>
        /// <param name="itemResourceType">The item resource type to create an EDM collection type for.</param>
        /// <param name="customAnnotations">The optional annotations for the resource type; the annotations can contain facets that need to be applied to the type reference.</param>
        /// <returns>An <see cref="IEdmCollectionType"/> instance for the <paramref name="itemResourceType"/> item type.</returns>
        /// <remarks>
        /// Materialization state: none required. No change in materialization state.
        /// Cache state: none required. No change in cache state.
        /// </remarks>
        private IEdmCollectionType EnsureCollectionItemTypeIsPrimitiveOrComplex(CollectionResourceType collectionResourceType, ResourceType itemResourceType, List<KeyValuePair<string, object>> customAnnotations)
        {
            Debug.Assert(itemResourceType != null, "itemResourceType != null");

            IEdmCollectionType collectionType;
            if (this.primitiveOrComplexCollectionTypeCache.TryGetValue(itemResourceType, out collectionType))
            {
                return collectionType;
            }

            IEdmTypeReference elementTypeReference;
            switch (itemResourceType.ResourceTypeKind)
            {
                case ResourceTypeKind.ComplexType:
                    // WCF DS forces the item types of collection types to not be nullable. Only starting v3, can complex properties be nullable. 
                    // For providers, like reflection providers, we need to ensure complex items continue to be non-nullable.
                    elementTypeReference = this.EnsureTypeReference(itemResourceType, customAnnotations);
                    elementTypeReference = elementTypeReference.IsNullable != PrimitiveOrComplexCollectionItemTypeDefaultNullability
                        ? elementTypeReference.Clone(PrimitiveOrComplexCollectionItemTypeDefaultNullability)
                        : elementTypeReference;
                    break;
                case ResourceTypeKind.Primitive:
                    // WCF DS forces the item types of collection types to not be nullable
                    MetadataProviderUtils.GetAndRemoveNullableFacet(customAnnotations);
                    IEdmPrimitiveTypeReference primitiveTypeReference = MetadataProviderUtils.CreatePrimitiveTypeReference(itemResourceType, customAnnotations);
                    elementTypeReference = primitiveTypeReference.IsNullable != PrimitiveOrComplexCollectionItemTypeDefaultNullability
                        ? primitiveTypeReference.Clone(PrimitiveOrComplexCollectionItemTypeDefaultNullability)
                        : primitiveTypeReference;

                    break;
                case ResourceTypeKind.EntityType:       // fall through
                case ResourceTypeKind.Collection:       // fall through
                case ResourceTypeKind.EntityCollection: // fall through
                default:
                    throw new InvalidOperationException(System.Data.Services.Strings.MetadataProviderEdmModel_UnsupportedCollectionItemType_PrimitiveOrComplex(itemResourceType.ResourceTypeKind.ToString()));
            }

            collectionType = new MetadataProviderEdmCollectionType(collectionResourceType, elementTypeReference);
            this.primitiveOrComplexCollectionTypeCache.Add(itemResourceType, collectionType);
            return collectionType;
        }

        /// <summary>
        /// Ensure that all the metadata elements have been created or create them.
        /// </summary>
        /// <remarks>
        /// Materialization state: full required. No change in materialization state.
        /// Cache state: none required. Cache state will be 'Full' after the method returned.
        /// </remarks>
        private void EnsureFullMetadata()
        {
            this.AssertMaterializationState(MetadataProviderState.Full);

            if (this.HasCacheState(MetadataProviderState.Full))
            {
                // metadata is already fully cached
                return;
            }

            // Group the resource types by namespace
            bool hasVisibleMediaLinkEntry = false;
            bool hasVisibleNamedStreams = false;

            IEnumerable<ResourceType> visibleTypes = this.metadataProvider.GetVisibleTypes().ToList();
            this.GroupResourceTypesByNamespace(visibleTypes, ref hasVisibleMediaLinkEntry, ref hasVisibleNamedStreams);

            // If we have encountered a visible named stream, we need to make sure there is an implementation for IDataServiceStreamProvider2
            // from this service or else we can end up with in-stream errors at runtime.
            if (this.streamProviderWrapper != null)
            {
                if (hasVisibleNamedStreams)
                {
                    this.streamProviderWrapper.LoadAndValidateStreamProvider2();
                }
                else if (hasVisibleMediaLinkEntry)
                {
                    // If we have encountered a visible MLE type, we need to make sure there is an implementation for IDataServiceStreamProvider
                    // from this service or else we can end up with in-stream errors at runtime.
                    this.streamProviderWrapper.LoadAndValidateStreamProvider();
                }
            }

            // Add all the schema elements first
            this.EnsureStructuredTypes(visibleTypes);

            // Ensure the entity containers with the entity sets and association sets
            this.EnsureEntityContainers();

            // NOTE: This will create the association types and fix up the navigation property with the 
            //       association ends as needed.
            // NOTE: it is important to not do this as part of EnsureEntityContainers since we need the
            //       full cache of types for the navigation property fixup which we don't have in all
            //       cases when EnsureEntityContainers is called.
            this.PairUpNavigationProperties();

            this.SetCacheState(MetadataProviderState.Full);
        }

        /// <summary>
        /// Groups all visible resource types by there namespace.
        /// </summary>
        /// <param name="visibleTypes">The visible types in provider metadata.</param>
        /// <param name="hasVisibleMediaLinkEntry">Set to true if we see any visible MLE.</param>
        /// <param name="hasVisibleNamedStreams">Set to true if we see any visible NamedStream.</param>
        /// <remarks>
        /// Materialization state: full required. No change in materialization state.
        /// Cache state: none required. No change in cache state.
        /// </remarks>
        private void GroupResourceTypesByNamespace(IEnumerable<ResourceType> visibleTypes, ref bool hasVisibleMediaLinkEntry, ref bool hasVisibleNamedStreams)
        {
            this.AssertMaterializationState(MetadataProviderState.Full);

            // Add all visible types
            foreach (ResourceType resourceType in visibleTypes)
            {
                this.AddVisibleResourceTypeToTypesInNamespaceCache(resourceType, ref hasVisibleMediaLinkEntry, ref hasVisibleNamedStreams);
            }
        }

        /// <summary>Add a resource type to the list of visible types for the type's namespace.</summary>
        /// <param name="resourceType">The resource type to add.</param>
        /// <param name="hasVisibleMediaLinkEntry">Set to true if we see any visible MLE.</param>
        /// <param name="hasVisibleNamedStreams">Set to true if we see any visible NamedStream.</param>
        /// <returns>True if we successfully added the type, false if the type is already in the hashset.</returns>
        /// <remarks>
        /// Materialization state: full required. No change in materialization state.
        /// Cache state: none required. No change in cache state.
        /// </remarks>
        private bool AddVisibleResourceTypeToTypesInNamespaceCache(ResourceType resourceType, ref bool hasVisibleMediaLinkEntry, ref bool hasVisibleNamedStreams)
        {
            this.AssertMaterializationState(MetadataProviderState.Full);

            string typeNamespace = this.GetTypeNamespace(resourceType);
            HashSet<ResourceType> typesInSameNamespace = this.GetResourceTypesForNamespace(typeNamespace);

            if (resourceType.IsMediaLinkEntry)
            {
                hasVisibleMediaLinkEntry = true;
            }

            if (resourceType.HasNamedStreams)
            {
                hasVisibleNamedStreams = true;
            }

            return typesInSameNamespace.Add(resourceType);
        }

        /// <summary>
        /// Ensure that all the schema types have been created or create them.
        /// </summary>
        /// <param name="visibleTypes">Visible types in provider metadata</param>
        /// <remarks>
        /// Materialization state: full required. No change in materialization state.
        /// Cache state: none required. No change in cache state.
        /// </remarks>
        private void EnsureStructuredTypes(IEnumerable<ResourceType> visibleTypes)
        {
            this.AssertMaterializationState(MetadataProviderState.Full);

            if (visibleTypes != null)
            {
                foreach (ResourceType resourceType in visibleTypes)
                {
                    this.EnsureSchemaType(resourceType);
                }
            }
        }

        /// <summary>
        /// Ensure that all entity containers have been created or create them.
        /// </summary>
        /// <remarks>
        /// Materialization state: EntityContainers required. No change in materialization state.
        /// Cache state: none required. 'EntityContainers' when the method returns.
        /// </remarks>
        private void EnsureEntityContainers()
        {
            this.AssertMaterializationState(MetadataProviderState.Full);

            if (this.HasCacheState(MetadataProviderState.Full))
            {
                // Entity containers have already been materialized.
                return;
            }

            // Service operations will go into the default entity container. Note that there should always be a default container.
            MetadataProviderEdmEntityContainer serviceOperationContainer = this.EnsureDefaultEntityContainer();
            Debug.Assert(serviceOperationContainer != null, "serviceOperationContainer != null");

            IEnumerable<ResourceSetWrapper> resourceSets = this.metadataProvider.GetResourceSets();
            if (resourceSets != null)
            {
                foreach (ResourceSetWrapper resourceSet in resourceSets)
                {
                    this.EnsureEntitySet(resourceSet);
                }
            }

            IEnumerable<OperationWrapper> serviceOperations = this.metadataProvider.GetVisibleOperations();
            if (serviceOperations != null)
            {
                foreach (OperationWrapper serviceOperation in serviceOperations)
                {
                    serviceOperationContainer.EnsureFunctionImport(serviceOperation);
                }
            }
        }

        /// <summary>
        /// Match navigation properties with their partners for all entity sets.
        /// </summary>
        /// <remarks>
        /// Materialization state: Full required. No change in materialization state.
        /// Cache state: none required. No change in cache state.
        /// </remarks>
        private void PairUpNavigationProperties()
        {
            this.AssertMaterializationState(MetadataProviderState.Full);
            Debug.Assert(!this.HasCacheState(MetadataProviderState.Full), "Should not have fully cached the metadata.");

            IEnumerable<ResourceSetWrapper> resourceSets = this.metadataProvider.GetResourceSets();
            if (resourceSets != null)
            {
                foreach (ResourceSetWrapper resourceSet in resourceSets)
                {
                    // NOTE: reachable entity types need to be populated before we can populate the associations.
                    this.PairUpNavigationPropertiesForEntitySet(resourceSet);
                }
            }
        }

        /// <summary>
        /// Match navigation properties with their partners for the given resource set.
        /// </summary>
        /// <param name="resourceSet">The resource set to supply the necessary data for matchign up the navigation properties.</param>
        /// <remarks>
        /// Materialization state: Full required. No change in materialization state.
        /// Cache state: none required. No change in cache state.
        /// </remarks>
        private void PairUpNavigationPropertiesForEntitySet(ResourceSetWrapper resourceSet)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");
            this.AssertMaterializationState(MetadataProviderState.Full);

            // Populate for derived types
            foreach (ResourceType derivedType in this.MetadataProvider.GetDerivedTypes(resourceSet.ResourceType))
            {
                this.PairUpNavigationPropertiesForEntitySetAndType(resourceSet, derivedType);
            }

            // Populate for this type and its base types
            ResourceType resourceType = resourceSet.ResourceType;
            while (resourceType != null)
            {
                this.PairUpNavigationPropertiesForEntitySetAndType(resourceSet, resourceType);
                resourceType = resourceType.BaseType;
            }
        }

        /// <summary>Match navigation properties with their partners for the given set and type</summary>
        /// <param name="resourceSet">Resource type to inspect.</param>
        /// <param name="resourceType">Resource set to inspect.</param>
        /// <remarks>
        /// Materialization state: Full required. No change in materialization state.
        /// Cache state: none required. No change in cache state.
        /// </remarks>
        private void PairUpNavigationPropertiesForEntitySetAndType(ResourceSetWrapper resourceSet, ResourceType resourceType)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");
            Debug.Assert(resourceType != null, "resourceType != null");
            this.AssertMaterializationState(MetadataProviderState.Full);

            IEnumerable<ResourceProperty> resourceProperties = resourceType.PropertiesDeclaredOnThisType;
            if (resourceProperties != null)
            {
                foreach (ResourceProperty navigationProperty in resourceProperties.Where(p => p.TypeKind == ResourceTypeKind.EntityType))
                {
                    // For every nav property, hook it up to its partner.
                    this.PairUpNavigationProperty(resourceSet, resourceType, navigationProperty);
                }
            }
        }

        /// <summary>
        /// Fixes up the two navigation properties refered to by a resource association set
        /// </summary>
        /// <param name="resourceAssociationSet">Resource association set to inspect.</param>
        private void PairUpNavigationPropertyWithResourceAssociationSet(ResourceAssociationSet resourceAssociationSet)
        {
            bool isBiDirectional = resourceAssociationSet.End1.ResourceProperty != null && resourceAssociationSet.End2.ResourceProperty != null;

            string end1Name;
            EdmMultiplicity navigationProperty1Multiplicity;
            EdmOnDeleteAction end1DeleteAction;

            string end2Name;
            EdmMultiplicity navigationProperty2Multiplicity;
            EdmOnDeleteAction end2DeleteAction;

            bool end1IsPrinciple = false;
            List<IEdmStructuralProperty> dependentProperties = null;
            ResourceAssociationType resourceAssociationType = resourceAssociationSet.ResourceAssociationType;

            if (resourceAssociationType != null)
            {
                end1Name = resourceAssociationType.End1.Name;
                end1DeleteAction = resourceAssociationType.End1.DeleteBehavior;
                navigationProperty2Multiplicity = MetadataProviderUtils.ConvertMultiplicity(resourceAssociationType.End1.Multiplicity);

                end2Name = resourceAssociationType.End2.Name;
                end2DeleteAction = resourceAssociationType.End2.DeleteBehavior;
                navigationProperty1Multiplicity = MetadataProviderUtils.ConvertMultiplicity(resourceAssociationType.End2.Multiplicity);

                ResourceReferentialConstraint referentialConstraint = resourceAssociationType.ReferentialConstraint;
                if (referentialConstraint != null)
                {
                    end1IsPrinciple = object.ReferenceEquals(resourceAssociationType.End1, referentialConstraint.PrincipalEnd);
                    IEdmEntityType dependentEntityType = end1IsPrinciple
                        ? (IEdmEntityType)this.EnsureSchemaType(resourceAssociationSet.End2.ResourceType)
                        : (IEdmEntityType)this.EnsureSchemaType(resourceAssociationSet.End1.ResourceType);

                    // now resolve all the dependent properties against the dependent entity type
                    dependentProperties = new List<IEdmStructuralProperty>();
                    foreach (ResourceProperty dependentResourceProperty in referentialConstraint.DependentProperties)
                    {
                        IEdmProperty property = dependentEntityType.FindProperty(dependentResourceProperty.Name);

                        if (!dependentEntityType.HasDeclaredKeyProperty(property))
                        {
                            // When there is a referential constraint referring to a non-key property as dependent,
                            // then consider it as at least a v2 model.
                            this.MinMetadataEdmSchemaVersion = Microsoft.Data.Edm.Library.EdmConstants.EdmVersion2;
                        }
                        
                        Debug.Assert(property.PropertyKind == EdmPropertyKind.Structural, "Dependent properties must be structural.");
                        dependentProperties.Add((IEdmStructuralProperty)property);
                    }
                }
            }
            else
            {
                if (!isBiDirectional)
                {
                    // If this association is not bi-directional, we use the type name as the from role name and the property name as the to role name
                    // This is the behavior for V1.
                    if (resourceAssociationSet.End1.ResourceProperty != null)
                    {
                        end1Name = resourceAssociationSet.End1.ResourceType.Name;
                        end2Name = resourceAssociationSet.End1.ResourceProperty.Name;
                    }
                    else
                    {
                        end1Name = resourceAssociationSet.End2.ResourceProperty.Name;
                        end2Name = resourceAssociationSet.End2.ResourceType.Name;
                    }
                }
                else
                {
                    // If the association is bi-directional, we use typeName_propertyName from each end as the name for that role
                    end1Name = MetadataProviderUtils.GetAssociationEndName(resourceAssociationSet.End1.ResourceType, resourceAssociationSet.End1.ResourceProperty);
                    end2Name = MetadataProviderUtils.GetAssociationEndName(resourceAssociationSet.End2.ResourceType, resourceAssociationSet.End2.ResourceProperty);
                    Debug.Assert(end1Name != end2Name, "end1Name != end2Name");
                }

                navigationProperty1Multiplicity = MetadataProviderUtils.GetMultiplicity(resourceAssociationSet.End1.ResourceProperty);
                end1DeleteAction = EdmOnDeleteAction.None;

                navigationProperty2Multiplicity = MetadataProviderUtils.GetMultiplicity(resourceAssociationSet.End2.ResourceProperty);
                end2DeleteAction = EdmOnDeleteAction.None;
            }

            string associationName = resourceAssociationType == null ? MetadataProviderUtils.GetAssociationName(resourceAssociationSet) : resourceAssociationType.Name;

            string associationNamespace;
            if (resourceAssociationType == null || resourceAssociationType.NamespaceName == null)
            {
                ResourceAssociationSetEnd end1 = resourceAssociationSet.End1.ResourceProperty != null ? resourceAssociationSet.End1 : resourceAssociationSet.End2;
                associationNamespace = this.GetTypeNamespace(end1.ResourceType);
            }
            else
            {
                associationNamespace = resourceAssociationType.NamespaceName;
            }

            ResourceProperty resourceProperty1 = resourceAssociationSet.End1.ResourceProperty;
            ResourceProperty resourceProperty2 = resourceAssociationSet.End2.ResourceProperty;
            MetadataProviderEdmNavigationProperty metadataNavigationProperty1 = null;
            MetadataProviderEdmNavigationProperty metadataNavigationProperty2 = null;
            if (resourceProperty1 != null)
            {
                IEdmEntityType entityType = (IEdmEntityType)this.EnsureSchemaType(resourceAssociationSet.End1.ResourceType);
                Debug.Assert(entityType != null, "entityType != null");
                metadataNavigationProperty1 = (MetadataProviderEdmNavigationProperty)entityType.FindProperty(resourceProperty1.Name);
                Debug.Assert(metadataNavigationProperty1 != null, "metadataNavigationProperty1 != null");
            }

            if (resourceProperty2 != null)
            {
                IEdmEntityType entityType = (IEdmEntityType)this.EnsureSchemaType(resourceAssociationSet.End2.ResourceType);
                Debug.Assert(entityType != null, "entityType != null");
                metadataNavigationProperty2 = (MetadataProviderEdmNavigationProperty)entityType.FindProperty(resourceProperty2.Name);
                Debug.Assert(metadataNavigationProperty2 != null, "metadataNavigationProperty2 != null");
            }

            IEdmNavigationProperty navigationProperty1 = metadataNavigationProperty1 != null
                ? (IEdmNavigationProperty)metadataNavigationProperty1
                : new MetadataProviderEdmSilentNavigationProperty(metadataNavigationProperty2, end1DeleteAction, navigationProperty1Multiplicity, end1Name);
            IEdmNavigationProperty navigationProperty2 = metadataNavigationProperty2 != null
                ? (IEdmNavigationProperty)metadataNavigationProperty2
                : new MetadataProviderEdmSilentNavigationProperty(metadataNavigationProperty1, end2DeleteAction, navigationProperty2Multiplicity, end2Name);

            MetadataProviderUtils.FixUpNavigationPropertyWithAssociationSetData(navigationProperty1, navigationProperty2, end1IsPrinciple, dependentProperties, end1DeleteAction, navigationProperty1Multiplicity);
            MetadataProviderUtils.FixUpNavigationPropertyWithAssociationSetData(navigationProperty2, navigationProperty1, !end1IsPrinciple, dependentProperties, end2DeleteAction, navigationProperty2Multiplicity);

            MetadataProviderEdmEntitySet end1EntitySet = (MetadataProviderEdmEntitySet)this.EnsureEntitySet(this.metadataProvider.ValidateResourceSet(resourceAssociationSet.End1.ResourceSet));
            MetadataProviderEdmEntitySet end2EntitySet = (MetadataProviderEdmEntitySet)this.EnsureEntitySet(this.metadataProvider.ValidateResourceSet(resourceAssociationSet.End2.ResourceSet));

            if (metadataNavigationProperty1 != null)
            {
                end1EntitySet.EnsureNavigationTarget(metadataNavigationProperty1, end2EntitySet);
                this.SetAssociationSetName(end1EntitySet, metadataNavigationProperty1, resourceAssociationSet.Name);
                this.SetAssociationSetAnnotations(
                    end1EntitySet,
                    metadataNavigationProperty1,
                    MetadataProviderUtils.ConvertCustomAnnotations(this, resourceAssociationSet.CustomAnnotations),
                    MetadataProviderUtils.ConvertCustomAnnotations(this, resourceAssociationSet.End1.CustomAnnotations),
                    MetadataProviderUtils.ConvertCustomAnnotations(this, resourceAssociationSet.End2.CustomAnnotations));
            }

            if (metadataNavigationProperty2 != null)
            {
                end2EntitySet.EnsureNavigationTarget(metadataNavigationProperty2, end1EntitySet);
                this.SetAssociationSetName(end2EntitySet, metadataNavigationProperty2, resourceAssociationSet.Name);

                // Make sure to only set the annotations once.
                if (metadataNavigationProperty1 == null)
                {
                    this.SetAssociationSetAnnotations(
                        end1EntitySet,
                        metadataNavigationProperty2,
                        MetadataProviderUtils.ConvertCustomAnnotations(this, resourceAssociationSet.CustomAnnotations),
                        MetadataProviderUtils.ConvertCustomAnnotations(this, resourceAssociationSet.End1.CustomAnnotations),
                        MetadataProviderUtils.ConvertCustomAnnotations(this, resourceAssociationSet.End2.CustomAnnotations));
                }
            }

            this.SetAssociationNamespace(navigationProperty1, associationNamespace);
            this.SetAssociationName(navigationProperty1, associationName);
            this.SetAssociationEndName(navigationProperty1, end1Name);

            this.SetAssociationNamespace(navigationProperty2, associationNamespace);
            this.SetAssociationName(navigationProperty2, associationName);
            this.SetAssociationEndName(navigationProperty2, end2Name);

            if (resourceAssociationType != null)
            {
                this.SetAssociationAnnotations(
                    navigationProperty1,
                    MetadataProviderUtils.ConvertCustomAnnotations(this, resourceAssociationType.CustomAnnotations),
                    MetadataProviderUtils.ConvertCustomAnnotations(this, navigationProperty1.GetPrimary() == navigationProperty1 ? resourceAssociationType.End1.CustomAnnotations : resourceAssociationType.End2.CustomAnnotations),
                    MetadataProviderUtils.ConvertCustomAnnotations(this, navigationProperty1.GetPrimary() == navigationProperty1 ? resourceAssociationType.End2.CustomAnnotations : resourceAssociationType.End1.CustomAnnotations),
                    MetadataProviderUtils.ConvertCustomAnnotations(this, resourceAssociationType.ReferentialConstraint != null ? resourceAssociationType.ReferentialConstraint.CustomAnnotations : null));
            }
        }

        /// <summary>
        /// Creates an <see cref="IEdmEntityType"/> for the <paramref name="resourceType"/>
        /// and adds it to the schema type cache.
        /// </summary>
        /// <param name="resourceType">The resource type to create an EDM schema type for.</param>
        /// <param name="resourceTypeNamespace">
        /// The namespace name for the entity type to create. Note that the namespace on the <paramref name="resourceType"/>
        /// can be null in which case this will be the default (i.e., container) namespace for the type.
        /// </param>
        /// <returns>An <see cref="IEdmEntityType"/> instance for the <paramref name="resourceType"/>.</returns>
        /// <remarks>
        /// This method will transitively ensure that all the base types of this type have been created.
        /// 
        /// Materialization state: none required. No change in materialization state.
        /// Cache state: none required. No change in cache state.
        /// </remarks>
        private IEdmEntityType AddEntityType(ResourceType resourceType, string resourceTypeNamespace)
        {
            Debug.Assert(resourceType != null, "resourceType != null");
            Debug.Assert(resourceType.ResourceTypeKind == ResourceTypeKind.EntityType, "resourceType.ResourceTypeKind == ResourceTypeKind.EntityType");
            
            // action to create properties
            Action<EdmEntityTypeWithDelayLoadedProperties> createPropertiesAction = type =>
                {
                    IEnumerable<ResourceProperty> resourceProperties = resourceType.PropertiesDeclaredOnThisType;
                    if (resourceProperties != null)
                    {
                        if (this.ShouldCheckForPropertyVisibility)
                        {
                            resourceProperties = resourceProperties.Where(this.ShouldPropertyBeIncludedInMetadata);
                        }

                        IDictionary<ResourceProperty, IEdmStructuralProperty> keyProperties = null;
                        foreach (ResourceProperty resourceProperty in resourceProperties)
                        {
                            IEdmProperty property = this.CreateProperty(type, resourceProperty);
                            if (resourceProperty.IsOfKind(ResourcePropertyKind.Key))
                            {
                                if (keyProperties == null)
                                {
                                    keyProperties = new Dictionary<ResourceProperty, IEdmStructuralProperty>(ReferenceEqualityComparer<ResourceProperty>.Instance);
                                }

                                Debug.Assert(property.PropertyKind == EdmPropertyKind.Structural, "property.PropertyKind == EdmPropertyKind.Structural");    
                                keyProperties.Add(resourceProperty, (IEdmStructuralProperty)property);
                            }
                        }

                        if (keyProperties != null)
                        {
                            Debug.Assert(resourceType.KeyProperties != null, "resourceType.KeyProperties != null");
                            type.AddKeys(resourceType.KeyProperties.Select(p => keyProperties[p]));
                        }
                    }
                };

            // First add the entity type to the cache so that if we encounter it again during processing 
            // of the base type's properties or this type's properties we won't re-create it.
            MetadataProviderEdmEntityType entityType = new MetadataProviderEdmEntityType(
                resourceTypeNamespace,
                resourceType,
                resourceType.BaseType != null ? (IEdmEntityType)this.EnsureSchemaType(resourceType.BaseType) : null,
                resourceType.IsAbstract,
                resourceType.IsOpenType,
                createPropertiesAction);

            // TODO: Once we want to support OData.Query we need to add an EdmType annotation here to specify the CanReflectOnInstanceType and InstanceType values.
            // add the type annotation to the entity type
            // entityType.SetAnnotation(MetadataProviderUtils.CreateODataEdmTypeAnnotation(resourceType));

            // add the entity type to the cache here to prevent endless loops
            // if some of the properties reference the type itself
            this.CacheSchemaType(entityType);

            // Set the HasStream attribute only if this is an MLE and the base type isn't also an MLE (if it is,
            // it's sufficient that HasStream is written on the base type). 
            if (resourceType.IsMediaLinkEntry && (resourceType.BaseType == null || resourceType.BaseType.IsMediaLinkEntry == false))
            {
                this.SetHasDefaultStream(entityType, true);
            }

            // convert EPM information
            if (resourceType.HasEntityPropertyMappings)
            {
                MetadataProviderUtils.ConvertEntityPropertyMappings(this, resourceType, entityType);
            }

            MetadataProviderUtils.ConvertCustomAnnotations(this, resourceType.CustomAnnotations, entityType);

            return entityType;
        }
        
        /// <summary>
        /// Returns whether or not the property should be included in the $metadata output. 
        /// Navigation properties are only included when their target type is reachable from a visible set.
        /// Structural properties are always included.
        /// </summary>
        /// <param name="property">The property to check.</param>
        /// <returns>Whether the property should be included in $metadata.</returns>
        private bool ShouldPropertyBeIncludedInMetadata(ResourceProperty property)
        {
            Debug.Assert(property != null, "property != null");
            Debug.Assert(this.ShouldCheckForPropertyVisibility, "Should only be called during metadata serialization.");

            if (property.TypeKind != ResourceTypeKind.EntityType)
            {
                // non-entity properties are always visible.
                return true;
            }

            // NOTE: For $metadata serialization we check the set of visible types to determine whether
            //       a navigation property should be visible or not.
            //       When ODataLib serializes a payload and looks up properties, 
            //       we don't consider visibility at all since it depends
            //       not only on the type but also the entity set the type came from (MEST scenarios!).
            //       ODataLib has no knowledge of entity sets so the design is for Astoria to not include
            //       hidden navigation properties in the ODataEntry to begin with and not bother with
            //       visibility here.
            // NOTE: We get here in materialization state 'Full' when we create the association types.
            //       We get here in cache state 'Full' when delay-loading the properties.
            ResourceType propertyType = property.ResourceType;
            string typeNamespace = this.GetTypeNamespace(propertyType);
            HashSet<ResourceType> typesInNamespace = this.GetResourceTypesForNamespace(typeNamespace);
            return typesInNamespace.Contains(propertyType);
        }

        /// <summary>
        /// Creates an <see cref="IEdmComplexType"/> for the <paramref name="resourceType"/>
        /// and adds it to the schema type cache.
        /// </summary>
        /// <param name="resourceType">The resource type to create an EDM schema type for.</param>
        /// <param name="resourceTypeNamespace">The namespace name for the entity type to create.</param>
        /// <returns>An <see cref="IEdmComplexType"/> instance for the <paramref name="resourceType"/>.</returns>
        /// <remarks>
        /// This method will transitively ensure that all the base types of this type have been created.
        /// 
        /// Materialization state: none required. No change in materialization state.
        /// Cache state: none required. No change in cache state.
        /// </remarks>
        private IEdmComplexType AddComplexType(ResourceType resourceType, string resourceTypeNamespace)
        {
            Debug.Assert(resourceType != null, "resourceType != null");
            Debug.Assert(resourceType.ResourceTypeKind == ResourceTypeKind.ComplexType, "resourceType.ResourceTypeKind == ResourceTypeKind.ComplexType");

            // action to create the properties of the complex type
            Action<EdmComplexTypeWithDelayLoadedProperties> createPropertiesAction = type =>
                {
                    IEnumerable<ResourceProperty> resourceProperties = resourceType.PropertiesDeclaredOnThisType;
                    if (resourceProperties != null)
                    {
                        foreach (ResourceProperty resourceProperty in resourceProperties)
                        {
                            this.CreateProperty(type, resourceProperty);
                        }
                    }
                };

            // EDM does not support open complex types. If this is no longer true, pass resourceType.IsOpenType as a parameter into the constructor below.
            Debug.Assert(!resourceType.IsOpenType, "EDM does not support open complex types.");

            MetadataProviderEdmComplexType complexType = new MetadataProviderEdmComplexType(
                resourceTypeNamespace,
                resourceType,
                resourceType.BaseType != null ? (IEdmComplexType)this.EnsureSchemaType(resourceType.BaseType) : null,
                false,
                createPropertiesAction);

            // TODO: Once we want to support OData.Query we need to add an EdmType annotation here to specify the CanReflectOnInstanceType and InstanceType values.
            // add the type annotation to the complex type
            // complexType.SetAnnotation(MetadataProviderUtils.CreateODataEdmTypeAnnotation(resourceType));
           
            // add the complex type to the cache here to prevent endless loops
            // if some of the properties reference the type itself
            this.CacheSchemaType(complexType);

            MetadataProviderUtils.ConvertCustomAnnotations(this, resourceType.CustomAnnotations, complexType);

            return complexType;
        }

        /// <summary>
        /// Gets or creates an <see cref="IEdmCollectionType"/> for the <paramref name="resourceType"/>.
        /// </summary>
        /// <param name="resourceType">The resource type to create an EDM collection type for.</param>
        /// <param name="customAnnotations">The optional annotations for the resource type; the annotations can contain facets that need to be applied to the type reference.</param>
        /// <returns>An <see cref="IEdmCollectionType"/> instance for the <paramref name="resourceType"/>.</returns>
        /// <remarks>
        /// Materialization state: none required. No change in materialization state.
        /// Cache state: none required. No change in cache state.
        /// </remarks>
        private IEdmCollectionType EnsurePrimitiveOrComplexCollectionType(ResourceType resourceType, List<KeyValuePair<string, object>> customAnnotations)
        {
            Debug.Assert(resourceType != null, "resourceType != null");
            Debug.Assert(resourceType.ResourceTypeKind == ResourceTypeKind.Collection, "resourceType.ResourceTypeKind == ResourceTypeKind.Collection");

            CollectionResourceType collectionResourceType = (CollectionResourceType)resourceType;
            ResourceType itemType = collectionResourceType.ItemType;

            return this.EnsureCollectionItemTypeIsPrimitiveOrComplex(collectionResourceType, itemType, customAnnotations);
        }

        /// <summary>
        /// Gets or creates an <see cref="IEdmCollectionType"/> for the <paramref name="resourceType"/>.
        /// </summary>
        /// <param name="resourceType">The resource type to create an EDM collection type for.</param>
        /// <param name="customAnnotations">The optional annotations for the resource type; the annotations can contain facets that need to be applied to the type reference.</param>
        /// <returns>An <see cref="IEdmCollectionType"/> instance for the <paramref name="resourceType"/>.</returns>
        /// <remarks>
        /// This method is called with entity types (navigation properties) and entity collection types (collection types).
        /// 
        /// Materialization state: none required. No change in materialization state.
        /// Cache state: none required. No change in cache state.
        /// </remarks>
        private IEdmCollectionType EnsureEntityCollectionType(ResourceType resourceType, List<KeyValuePair<string, object>> customAnnotations)
        {
            Debug.Assert(resourceType != null, "resourceType != null");
            Debug.Assert(
              resourceType.ResourceTypeKind == ResourceTypeKind.EntityCollection || resourceType.ResourceTypeKind == ResourceTypeKind.EntityType,
              "resourceType.ResourceTypeKind == ResourceTypeKind.EntityCollection || resourceType.ResourceTypeKind == ResourceTypeKind.EntityType");
            ResourceType itemType = resourceType;
            EntityCollectionResourceType entityCollectionResourceType = resourceType as EntityCollectionResourceType;
            if (entityCollectionResourceType != null)
            {
                itemType = entityCollectionResourceType.ItemType;
            }

            Debug.Assert(itemType.ResourceTypeKind == ResourceTypeKind.EntityType, "itemType.ResourceTypeKind == ResourceTypeKind.EntityType");
            return this.EnsureCollectionItemTypeIsEntityPrimitiveOrComplex(itemType, resourceType, customAnnotations);
        }

        /// <summary>
        /// Creates an <see cref="IEdmProperty"/> for the <paramref name="resourceProperty"/>.
        /// </summary>
        /// <param name="declaringType">The declaring type of the <paramref name="resourceProperty"/>.</param>
        /// <param name="resourceProperty">The resource property to create an EDM property for.</param>
        /// <returns>An <see cref="IEdmProperty"/> instance for the <paramref name="resourceProperty"/>.</returns>
        /// <remarks>
        /// Materialization state: none required. No change in materialization state.
        /// Cache state: none required. No change in cache state.
        /// </remarks>
        private IEdmProperty CreateProperty(EdmStructuredType declaringType, ResourceProperty resourceProperty)
        {
            Debug.Assert(declaringType != null, "structuralType != null");
            Debug.Assert(resourceProperty != null, "resourceProperty != null");

            List<KeyValuePair<string, object>> customAnnotations = resourceProperty.CustomAnnotations == null ? null : resourceProperty.CustomAnnotations.ToList();
            IEdmProperty result;
            ODataNullValueBehaviorKind nullValueReadBehaviorKind = ODataNullValueBehaviorKind.Default;
            if (resourceProperty.IsOfKind(ResourcePropertyKind.Primitive) || resourceProperty.IsOfKind(ResourcePropertyKind.Stream))
            {
                ResourceType resourceType = resourceProperty.ResourceType;
                IEdmPrimitiveTypeReference primitiveTypeReference = MetadataProviderUtils.CreatePrimitiveTypeReference(resourceType, customAnnotations);

                if (resourceProperty.IsOfKind(ResourcePropertyKind.Key))
                {
                    // if the property is a key we force nullable to be 'false'
                    if (primitiveTypeReference.IsNullable)
                    {
                        Debug.Assert(resourceProperty.IsOfKind(ResourcePropertyKind.Primitive), "Streams are always non-nullable so this must be a primitive resource property");
                        primitiveTypeReference = (IEdmPrimitiveTypeReference)primitiveTypeReference.Clone(/*nullable*/false);
                    }

                    nullValueReadBehaviorKind = ODataNullValueBehaviorKind.IgnoreValue;
                }
                else if (MetadataProviderUtils.ShouldDisablePrimitivePropertyNullValidation(resourceProperty, primitiveTypeReference))
                {
                    nullValueReadBehaviorKind = ODataNullValueBehaviorKind.DisableValidation;
                }

                string defaultValue = MetadataProviderUtils.GetAndRemoveDefaultValue(customAnnotations);
                EdmConcurrencyMode concurrencyMode = resourceProperty.IsOfKind(ResourcePropertyKind.ETag)
                    ? EdmConcurrencyMode.Fixed
                    : EdmConcurrencyMode.None;
                result = new MetadataProviderEdmStructuralProperty(declaringType, resourceProperty, primitiveTypeReference, defaultValue, concurrencyMode);
                declaringType.AddProperty(result);

                string mimeType = resourceProperty.MimeType;
                if (!string.IsNullOrEmpty(mimeType))
                {
                    ODataUtils.SetMimeType(this, result, mimeType);
                }
            }
            else if (resourceProperty.IsOfKind(ResourcePropertyKind.ComplexType))
            {
                IEdmTypeReference typeReference = this.EnsureTypeReference(resourceProperty.ResourceType, customAnnotations);

                // NOTE: WCF DS does not support complex ETag properties
                string defaultValue = MetadataProviderUtils.GetAndRemoveDefaultValue(customAnnotations);

                result = new MetadataProviderEdmStructuralProperty(declaringType, resourceProperty, typeReference, defaultValue, EdmConcurrencyMode.None);
                declaringType.AddProperty(result);

                if (this.metadataProvider.HasReflectionOrEFProviderQueryBehavior && !typeReference.IsNullable)
                {
                    nullValueReadBehaviorKind = ODataNullValueBehaviorKind.DisableValidation;
                }
            }
            else if (resourceProperty.IsOfKind(ResourcePropertyKind.Collection))
            {
                string defaultValue = MetadataProviderUtils.GetAndRemoveDefaultValue(customAnnotations);
                IEdmTypeReference collectionTypeReference = this.EnsureTypeReference(resourceProperty.ResourceType, customAnnotations);
                result = new MetadataProviderEdmStructuralProperty(declaringType, resourceProperty, collectionTypeReference, defaultValue, EdmConcurrencyMode.None);
                declaringType.AddProperty(result);
            }
            else if (resourceProperty.IsOfKind(ResourcePropertyKind.ResourceSetReference) || resourceProperty.IsOfKind(ResourcePropertyKind.ResourceReference))
            {
                Debug.Assert(resourceProperty.TypeKind == ResourceTypeKind.EntityType, "Expected entity type kind for navigation properties");
                Debug.Assert(declaringType.TypeKind == EdmTypeKind.Entity, "declaringType.TypeKind == EdmTypeKind.Entity");
                EdmEntityType sourceEntityType = (EdmEntityType)declaringType;

                IEdmTypeReference targetTypeReference = resourceProperty.IsOfKind(ResourcePropertyKind.ResourceSetReference)
                    ? this.EnsureEntityPrimitiveOrComplexCollectionTypeReference(resourceProperty.ResourceType, new EntityCollectionResourceType(resourceProperty.ResourceType), customAnnotations)
                    : this.EnsureTypeReference(resourceProperty.ResourceType, customAnnotations);
                result = new MetadataProviderEdmNavigationProperty(sourceEntityType, resourceProperty, targetTypeReference);
                sourceEntityType.AddProperty(result);
            }
            else
            {
                throw new InvalidOperationException(System.Data.Services.Strings.MetadataProviderEdmModel_UnsupportedResourcePropertyKind(resourceProperty.Kind.ToString()));
            }

            // Add the OData property annotation to the property
            // TODO: Once we want to support OData.Query we need to add a property annotation here to specify the CanReflectOnInstanceProperty value.
            this.SetNullValueReaderBehavior(result, nullValueReadBehaviorKind);

            // NOTE: we removed any facets from the annotations above; the remaining annotations will be serialized.
            MetadataProviderUtils.ConvertCustomAnnotations(this, customAnnotations, result);

            return result;
        }

        /// <summary>
        /// Adds a schema type to the internal caches of the model.
        /// </summary>
        /// <param name="schemaType">The <see cref="IEdmSchemaType"/> to cache.</param>
        /// <remarks>
        /// Materialization state: none required. No change in materialization state.
        /// Cache state: none required. No change in cache state.
        /// </remarks>
        private void CacheSchemaType(IEdmSchemaType schemaType)
        {
            Debug.Assert(schemaType != null, "schemaType != null");

            // first add the schema element to the schema element cache
            string fullName = schemaType.FullName();
            Debug.Assert(!this.schemaTypeCache.ContainsKey(fullName), "Schema type cache already contains an element with name " + fullName + ".");
            this.schemaTypeCache.Add(fullName, schemaType);
            IEdmStructuredType structuredType = schemaType as IEdmStructuredType;
            if (structuredType != null && structuredType.BaseType != null)
            {
                List<IEdmStructuredType> derivedTypes;
                if (!this.derivedTypeMappings.TryGetValue(structuredType.BaseType, out derivedTypes))
                {
                    derivedTypes = new List<IEdmStructuredType>();
                    this.derivedTypeMappings[structuredType.BaseType] = derivedTypes;
                }

                derivedTypes.Add(structuredType);
            }
        }

        /// <summary>
        /// Searches for an entity container with the given name in this model and returns null if no such entity container exists.
        /// </summary>
        /// <param name="name">The name of the entity container being found.</param>
        /// <returns>The requested entity container, or null if no such entity container exists.</returns>
        /// <remarks>
        /// Materialization state: none required. No change in materialization state.
        /// Cache state: EntityContainers required. No change in cache state.
        /// </remarks>
        private MetadataProviderEdmEntityContainer FindExistingEntityContainer(string name)
        {
            Debug.Assert(!string.IsNullOrEmpty(name), "!string.IsNullOrEmpty(name)");

            MetadataProviderEdmEntityContainer entityContainer;
            if (this.entityContainerCache.TryGetValue(name, out entityContainer))
            {
                return entityContainer;
            }

            return null;
        }

        /// <summary>
        /// Get the schema element hash set for the given namespace
        /// </summary>
        /// <param name="schemaElementNamespace">The namespace of the schema element.</param>
        /// <returns>resource type hash</returns>
        /// <remarks>
        /// Materialization state 'Full' or cache state 'Full' required. No change to either state.
        /// </remarks>
        private HashSet<ResourceType> GetResourceTypesForNamespace(string schemaElementNamespace)
        {
            Debug.Assert(
                this.materializationState == MetadataProviderState.Full || this.cacheState == MetadataProviderState.Full,
                "Either the materialization state or the cache state must be 'Full' to look up the resource types for a namespace.");

            HashSet<ResourceType> resourceTypesInSameNamespace;
            if (!this.resourceTypesPerNamespaceCache.TryGetValue(schemaElementNamespace, out resourceTypesInSameNamespace))
            {
                resourceTypesInSameNamespace = new HashSet<ResourceType>(EqualityComparer<ResourceType>.Default);
                this.resourceTypesPerNamespaceCache.Add(schemaElementNamespace, resourceTypesInSameNamespace);
            }

            return resourceTypesInSameNamespace;
        }

        /// <summary>Gets the namespace of a resource type. If it's null, default to the container namespace.</summary>
        /// <param name="resourceType">The resource type to get the namespace name for.</param>
        /// <returns>The namespace of the <paramref name="resourceType"/>.</returns>
        /// <remarks>
        /// Materialization state: none required. No change in materialization state.
        /// Cache state: none required. No change in cache state.
        /// </remarks>
        private string GetTypeNamespace(ResourceType resourceType)
        {
            string typeNamespace = resourceType.Namespace;
            if (string.IsNullOrEmpty(typeNamespace))
            {
                typeNamespace = this.GetContainerNamespace();
            }

            return typeNamespace;
        }

        /// <summary>Gets the namespace of the container. If it's null, default to the container name.</summary>
        /// <returns>The namespace of the default container.</returns>
        /// <remarks>
        /// Materialization state: none required. No change in materialization state.
        /// Cache state: none required. No change in cache state.
        /// </remarks>
        private string GetContainerNamespace()
        {
            string typeNamespace = this.metadataProvider.ContainerNamespace;
            if (string.IsNullOrEmpty(typeNamespace))
            {
                typeNamespace = this.metadataProvider.ContainerName;
            }

            return typeNamespace;
        }

        /// <summary>
        /// First sets the materialization state, then runs the specified action and then
        /// resets the materialization state and checks the cache state.
        /// </summary>
        /// <param name="action">The action to run.</param>
        /// <param name="state">The <see cref="MetadataProviderState"/> to run the action in.</param>
        /// <remarks>
        /// The materialization is expected to be 'Incremental' when this method is called.
        /// The method should only be used in public API methods that are not re-entered by
        /// private/internal code since only public API code should set the materialization state.
        /// </remarks>
        private void RunInState(Action action, MetadataProviderState state)
        {
            Debug.Assert(action != null, "action != null");
            Debug.Assert(
                this.materializationState == MetadataProviderState.Incremental,
                "No ongoing materialization code should call back into this public API");

            this.SetMaterializationState(state);
            action();
            this.SetMaterializationState(MetadataProviderState.Incremental);
            this.AssertCacheState(state);
        }

        /// <summary>
        /// Sets the current materialization state of the metadata if the new state is 
        /// greater than the existing state.
        /// </summary>
        /// <param name="newState">The new state to set.</param>
        /// <remarks>
        /// This method assumes that it is never called with the <paramref name="newState"/> being the same as the current
        /// state. Nested calls in the same state are not allowed.
        /// </remarks>
        private void SetMaterializationState(MetadataProviderState newState)
        {
            Debug.Assert(this.materializationState != newState, "Nested calls of this method with the same state are not allowed.");

            if (newState == MetadataProviderState.Incremental)
            {
                // Reset the materialization state
                this.materializationState = MetadataProviderState.Incremental;
            }
            else if (this.materializationState < newState)
            {
                this.materializationState = newState;
            }
        }

        /// <summary>
        /// Checks whether a specified materialization state has been reached (or exceeded).
        /// </summary>
        /// <param name="state">The state that has to be reached.</param>
        /// <returns>true if the materialization state has reached the state specified in <paramref name="state"/>; otherwise false.</returns>
        private bool HasMaterializationState(MetadataProviderState state)
        {
            return this.materializationState >= state;
        }

        /// <summary>
        /// Sets the current cache state of the metadata if the new state is 
        /// greater than the existing state.
        /// </summary>
        /// <param name="newState">The new state to set.</param>
        /// <remarks>
        /// This method assumes that it is never called with the <paramref name="newState"/> 
        /// being set to MetadataProviderState.Incremental.
        /// </remarks>
        private void SetCacheState(MetadataProviderState newState)
        {
            Debug.Assert(newState != MetadataProviderState.Incremental, "Should never set the cache state to 'Incremental'.");

            if (this.cacheState < newState)
            {
                this.cacheState = newState;
            }
        }

        /// <summary>
        /// Checks whether a specified cache state has been reached (or exceeded).
        /// </summary>
        /// <param name="state">The state that has to be reached.</param>
        /// <returns>true if the cache state has reached the state specified in <paramref name="state"/>; otherwise false.</returns>
        private bool HasCacheState(MetadataProviderState state)
        {
            return this.cacheState >= state;
        }
    }
}

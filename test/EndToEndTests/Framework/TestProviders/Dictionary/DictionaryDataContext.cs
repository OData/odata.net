//---------------------------------------------------------------------
// <copyright file="DictionaryDataContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.TestProviders.Dictionary
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using Microsoft.OData.Service.Providers;
    using Microsoft.Test.OData.Framework.TestProviders.Common;
    using Microsoft.Test.OData.Framework.TestProviders.Contracts;

    /// <summary>
    /// An implementation of the query, metadata, and update providers that uses typed/untyped and open/closed resources based on dictionaries
    /// </summary>
    public abstract class DictionaryDataContext
    {
        private static readonly Dictionary<Type, Dictionary<string, IList<ResourceInstance>>> resourceSetsByContextTypeStorage = new Dictionary<Type, Dictionary<string, IList<ResourceInstance>>>();
        private readonly IList<object> deletedObjects = new List<object>();
        private readonly IList<Action> pendingChanges = new List<Action>();
        private readonly HashSet<string> resourceTypeCache = new HashSet<string>();
        private readonly HashSet<string> resourceSetCache = new HashSet<string>();
        private readonly HashSet<string> serviceOperationCache = new HashSet<string>();
        private readonly HashSet<string> resourceAssociationSetCache = new HashSet<string>();
        private readonly DictionaryMetadataHelper metadataHelper;
        private readonly object dataServiceInstance;

        private bool containerNameAlreadyAccessed = false;
        private bool containerNamespaceAlreadyAccessed = false;

        /// <summary>
        /// Initializes a new instance of the DictionaryDataContext class
        /// </summary>
        /// <remarks>
        /// Using this constructor is incompatible with service operations
        /// </remarks>
        protected DictionaryDataContext() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DictionaryDataContext class
        /// </summary>
        /// <param name="dataService">the Data Service</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Must be initialized at construction time")]
        protected DictionaryDataContext(object dataService)
        {
            this.metadataHelper = this.CreateMetadataHelper();
            this.dataServiceInstance = dataService;

            if (!resourceSetsByContextTypeStorage.ContainsKey(this.GetType()))
            {
                resourceSetsByContextTypeStorage.Add(this.GetType(), new Dictionary<string, IList<ResourceInstance>>());
                this.InitializeResourceSetsStorage();
            }

            this.EnsureDataIsInitialized();
        }

        /// <summary>
        /// Gets the container name
        /// </summary>
        public virtual string ContainerName 
        {
            get 
            {
                if (ProviderImplementationSettings.Current.EnforceMetadataCaching)
                {
                    ExceptionUtilities.ThrowDataServiceExceptionIfFalse(!this.containerNameAlreadyAccessed, 500, "IDSMP.ContainerName should be cached after the first access");
                    this.containerNameAlreadyAccessed = true;
                }

                return this.GetType().Name; 
            } 
        }

        /// <summary>
        /// Gets the container namespace
        /// </summary>
        public virtual string ContainerNamespace 
        {
            get
            {
                if (ProviderImplementationSettings.Current.EnforceMetadataCaching)
                {
                    ExceptionUtilities.ThrowDataServiceExceptionIfFalse(!this.containerNamespaceAlreadyAccessed, 500, "IDSMP.ContainerNamespace should be cached after the first access");
                    this.containerNamespaceAlreadyAccessed = true;
                }

                return this.GetType().Namespace;
            }
        }

        /// <summary>
        /// Gets the metadata providers's types
        /// </summary>
        public virtual IEnumerable<ResourceType> Types
        {
            get
            {
                // Because this is not a 'try-resolve' style API, do not throw on repeats
                return this.metadataHelper.ResourceTypes.Select(t => this.EnforceMetadataCache(t, false, true));
            }
        }

        /// <summary>
        /// Gets the metadata providers's sets
        /// </summary>
        public virtual IEnumerable<ResourceSet> ResourceSets
        {
            get
            {
                // Because this is not a 'try-resolve' style API, do not throw on repeats
                return this.metadataHelper.ResourceSets.Select(s => this.EnforceMetadataCache(s, false, true, true));
            }
        }

        /// <summary>
        /// Gets the metadata providers's service operations
        /// </summary>
        public virtual IEnumerable<ServiceOperation> ServiceOperations
        {
            get
            {
                // Because this is not a 'try-resolve' style API, do not throw on repeats
                return this.metadataHelper.ServiceOperations.Select(s => this.EnforceMetadataCache(s, false));
            }
        }

        /// <summary>
        /// Gets or sets the current data source
        /// </summary>
        public virtual object CurrentDataSource { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not null-propagation is required
        /// </summary>
        public virtual bool IsNullPropagationRequired 
        { 
            get { return true; } 
        }

        /// <summary>
        /// Gets the Dictionary that contains the resourceSets
        /// </summary>
        protected internal Dictionary<string, IList<ResourceInstance>> ResourceSetsStorage
        {
            get
            {
                Dictionary<string, IList<ResourceInstance>> resourceSetsLookup = null;

                Type currentContextType = this.GetType();
                bool found = resourceSetsByContextTypeStorage.TryGetValue(currentContextType, out resourceSetsLookup);

                ExceptionUtilities.Assert(found, "Cannot find resource sets by the context type '{0}'", currentContextType);

                return resourceSetsLookup;
            }
        }

        /// <summary>
        /// Gets the set of method replacement strategies to use when evaluating queries
        /// </summary>
        protected abstract IEnumerable<IMethodReplacementStrategy> MethodReplacementStrategies { get; }
       
        /// <summary>
        /// Gets the resource association set with the given values
        /// </summary>
        /// <param name="resourceSet">The resource set</param>
        /// <param name="resourceType">The resource type</param>
        /// <param name="resourceProperty">The resource property</param>
        /// <returns>The resource association set</returns>
        public virtual ResourceAssociationSet GetResourceAssociationSet(ResourceSet resourceSet, ResourceType resourceType, ResourceProperty resourceProperty)
        {
            ExceptionUtilities.CheckArgumentNotNull(resourceSet, "resourceSet");
            ExceptionUtilities.CheckArgumentNotNull(resourceType, "resourceType");
            ExceptionUtilities.CheckArgumentNotNull(resourceProperty, "resourceProperty");

            ExceptionUtilities.Assert(
                (resourceProperty.Kind & (ResourcePropertyKind.ResourceReference | ResourcePropertyKind.ResourceSetReference)) != 0,
                "GetResourceAssociationSet called with non-navigation property '{0}' of kind '{1}'.", 
                resourceProperty.Name,
                resourceProperty.Kind);

            ExceptionUtilities.Assert(resourceType.GetLocalPropertiesLazily().Contains(resourceProperty), "ResourceProperty '{0}' must be on the ResourceType '{1}' when finding the ResourceAssociationSet", resourceProperty.Name, resourceType.Name);
            ExceptionUtilities.Assert(this.metadataHelper.GetResourceTypesOfResourceSet(resourceSet).Contains(resourceType), "ResourceSet '{0}' does not contain ResourceType '{1}'", resourceSet.Name, resourceType.Name);

            foreach (var associationSet in this.metadataHelper.ResourceAssociationSets)
            {
                if (associationSet.End1.ResourceSet == resourceSet && associationSet.End1.ResourceType == resourceType && associationSet.End1.ResourceProperty == resourceProperty)
                {
                    return this.EnforceMetadataCache(associationSet, resourceSet, resourceType, resourceProperty, true);
                }

                if (associationSet.End2.ResourceSet == resourceSet && associationSet.End2.ResourceType == resourceType && associationSet.End2.ResourceProperty == resourceProperty)
                {
                    return this.EnforceMetadataCache(associationSet, resourceSet, resourceType, resourceProperty, true);
                }
            }

            return null;
        }

        /// <summary>
        /// Trys to find the resource set with the given name 
        /// </summary>
        /// <param name="name">The name of the set</param>
        /// <param name="resourceSet">The set, if found</param>
        /// <returns>Whether or not the set was found</returns>
        public virtual bool TryResolveResourceSet(string name, out ResourceSet resourceSet)
        {
            ExceptionUtilities.CheckArgumentNotNull(name, "name");
            resourceSet = this.GetResourceSet(name);
            if (resourceSet != null)
            {
                // ideally, we would throw on re-resolution, but for each query interceptor this API will
                // be invoked regardless of cache status and the actual resource type needs to match
                resourceSet = this.EnforceMetadataCache(resourceSet, false, false, true);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Trys to find the service operation with the given name 
        /// </summary>
        /// <param name="name">The name of the set</param>
        /// <param name="serviceOperation">The service operation, if found</param>
        /// <returns>Whether or not the service operation was found</returns>
        public virtual bool TryResolveServiceOperation(string name, out ServiceOperation serviceOperation)
        {
            ExceptionUtilities.CheckArgumentNotNull(name, "name");
            serviceOperation = this.metadataHelper.ServiceOperations.SingleOrDefault(o => o.Name == name);
            if (serviceOperation != null)
            {
                // Because this is a 'try-resolve' style API, throw on repeats
                serviceOperation = this.EnforceMetadataCache(serviceOperation, true);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Trys to find the resource type with the given name 
        /// </summary>
        /// <param name="name">The name of the type</param>
        /// <param name="resourceType">The type, if found</param>
        /// <returns>Whether or not the type was found</returns>
        public virtual bool TryResolveResourceType(string name, out ResourceType resourceType)
        {
            ExceptionUtilities.CheckArgumentNotNull(name, "name");
            resourceType = this.metadataHelper.ResourceTypes.SingleOrDefault(t => t.FullName == name);
            if (resourceType != null)
            {
                // Because this is a 'try-resolve' style API, throw on repeats
                resourceType = this.EnforceMetadataCache(resourceType, true, true);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns whether or not the type has derived types
        /// </summary>
        /// <param name="resourceType">The type to check for derived types</param>
        /// <returns>True if the type has derived types, false otherwise</returns>
        public virtual bool HasDerivedTypes(ResourceType resourceType)
        {
            ExceptionUtilities.CheckArgumentNotNull(resourceType, "resourceType");
            ExceptionUtilities.Assert(resourceType.ResourceTypeKind == ResourceTypeKind.EntityType || resourceType.ResourceTypeKind == ResourceTypeKind.ComplexType, "HasDerivedTypes called on non-entity, non-complex type '{0}'", resourceType.FullName);
            return this.metadataHelper.ResourceTypes.Any(t => t.BaseType == resourceType);
        }

        /// <summary>
        /// Returns the types derived from the given type
        /// </summary>
        /// <param name="resourceType">The given type</param>
        /// <returns>The type's derived types</returns>
        public virtual IEnumerable<ResourceType> GetDerivedTypes(ResourceType resourceType)
        {
            ExceptionUtilities.CheckArgumentNotNull(resourceType, "resourceType");
            ExceptionUtilities.Assert(resourceType.ResourceTypeKind == ResourceTypeKind.EntityType, "GetDerivedTypes called on non-entity type '{0}'", resourceType.FullName);

            // Because this is not a 'try-resolve' style API, do not throw on repeats
            return this.GetDerivedTypesInternal(resourceType).Select(t => this.EnforceMetadataCache(t, false, true));
        }

        /// <summary>
        /// Gets the value of the given property for the given resource
        /// </summary>
        /// <param name="targetResource">The given resource</param>
        /// <param name="resourceProperty">The property to get the value for</param>
        /// <returns>The property value</returns>
        public virtual object GetPropertyValue(object targetResource, ResourceProperty resourceProperty)
        {
            ExceptionUtilities.CheckArgumentNotNull(targetResource, "targetResource");
            ExceptionUtilities.CheckArgumentNotNull(resourceProperty, "resourceProperty");
            ExceptionUtilities.Assert(!resourceProperty.CanReflectOnInstanceTypeProperty, "Should not have been called for property '{0}' which can be reflected on", resourceProperty.Name);
            ExceptionUtilities.Assert(!resourceProperty.Kind.HasFlag(ResourcePropertyKind.Stream), "GetPropertyValue called on stream property '{0}'", resourceProperty.Name);

            var resourceInstance = targetResource as ResourceInstance;
            ExceptionUtilities.CheckObjectNotNull(resourceInstance, "Given target was not a resource instance. Type was '{0}'.", targetResource.GetType());

            object propertyValue;
            if (!resourceInstance.TryGetValue(resourceProperty.Name, out propertyValue))
            {
                propertyValue = null;
            }

            return propertyValue;
        }

        /// <summary>
        /// Gets the open property values for the given resource
        /// </summary>
        /// <param name="target">The instance to get values from</param>
        /// <returns>The open property values</returns>
        public virtual IEnumerable<KeyValuePair<string, object>> GetOpenPropertyValues(object target)
        {
            ExceptionUtilities.CheckArgumentNotNull(target, "target");

            var instance = target as ResourceInstance;
            ExceptionUtilities.CheckObjectNotNull(instance, "Given target was not a resource instance. Type was '{0}'.", target.GetType());
            ExceptionUtilities.Assert(instance.IsEntityType, "GetOpenPropertyValues called on instance of complex type '{0}'", instance.ResourceTypeName);

            var resourceType = this.metadataHelper.ResourceTypes.Single(t => t.FullName == instance.ResourceTypeName);
            ExceptionUtilities.Assert(resourceType.IsOpenType, "GetOpenPropertyValues called on non-open type '{0}'", resourceType.FullName);

            return instance.Where(pair => !resourceType.GetAllPropertiesLazily().Any(p => p.Name == pair.Key));
        }

        /// <summary>
        /// Gets an open property value with the given name
        /// </summary>
        /// <param name="target">The instance to get the value from</param>
        /// <param name="propertyName">The open property name</param>
        /// <returns>The value of the property</returns>
        public virtual object GetOpenPropertyValue(object target, string propertyName)
        {
            ExceptionUtilities.CheckArgumentNotNull(target, "target");
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");

            var instance = target as ResourceInstance;
            ExceptionUtilities.CheckObjectNotNull(target, "Given target was not a resource instance. Type was '{0}'.", target.GetType());
            ExceptionUtilities.Assert(instance.IsEntityType, "GetOpenPropertyValue called on instance of complex type '{0}'", instance.ResourceTypeName);

            var resourceType = this.metadataHelper.ResourceTypes.Single(t => t.FullName == instance.ResourceTypeName);
            ExceptionUtilities.Assert(resourceType.IsOpenType, "GetOpenPropertyValue called on non-open type '{0}'", resourceType.FullName);

            ExceptionUtilities.Assert(
                !resourceType.GetAllPropertiesLazily().Any(p => p.Name == propertyName),
                "GetOpenPropertyValue called on declared property '{0}' on type '{1}'",
                propertyName,
                resourceType.FullName);

            object value;
            if (instance.TryGetValue(propertyName, out value))
            {
                return value;
            }

            return null;
        }

        /// <summary>
        /// Gets the root queryable for the given set
        /// </summary>
        /// <param name="container">The resource set</param>
        /// <returns>The root queryable</returns>
        public virtual IQueryable GetQueryRootForResourceSet(ResourceSet container)
        {
            ExceptionUtilities.CheckArgumentNotNull(container, "container");

            var queryable = this.ResourceSetsStorage[container.Name].AsQueryable();

            if (this.MethodReplacementStrategies.Any())
            {
                var provider = new MethodReplacingQueryProvider(queryable.Provider, new MethodReplacingExpressionVisitor(this.MethodReplacementStrategies));
                queryable = provider.CreateQuery<ResourceInstance>(queryable.Expression);
            }

            if (container.ResourceType.InstanceType == typeof(ResourceInstance))
            {
                return queryable;
            }
            else
            {
                MethodInfo castMethod = typeof(Queryable).GetMethod("Cast", BindingFlags.Static | BindingFlags.Public);
                castMethod = castMethod.MakeGenericMethod(container.ResourceType.InstanceType);

                object newQueryable = castMethod.Invoke(null, new object[] { queryable });
                return newQueryable as IQueryable;
            }
        }

        /// <summary>
        /// Gets the resource type for the given instance
        /// </summary>
        /// <param name="resource">The instance to get the type for</param>
        /// <returns>The type of the resource</returns>
        public virtual ResourceType GetResourceType(object resource)
        {
            // Because this is not a 'try-resolve' style API, do not throw on repeats
            return this.EnforceMetadataCache(this.GetResourceTypeInternal(resource), false, true);
        }

        /// <summary>
        /// Invokes the service operation with the given parameters
        /// </summary>
        /// <param name="serviceOperation">The service operation to invoke</param>
        /// <param name="parameters">The parameters to the service operation</param>
        /// <returns>The result of invoking the service operation</returns>
        public virtual object InvokeServiceOperation(ServiceOperation serviceOperation, object[] parameters)
        {
            ExceptionUtilities.CheckArgumentNotNull(serviceOperation, "serviceOperation");
            ExceptionUtilities.CheckArgumentNotNull(parameters, "parameters");
            ExceptionUtilities.CheckObjectNotNull(this.dataServiceInstance, "The data service instance has not been specified");

            Type serviceType = this.dataServiceInstance.GetType();
            MethodInfo serviceOperationMethod = serviceType.GetMethod(serviceOperation.Name);

            ExceptionUtilities.CheckObjectNotNull(serviceOperationMethod, "Method for service operation {0} not found", serviceOperation.Name);
            return serviceOperationMethod.Invoke(this.dataServiceInstance, BindingFlags.FlattenHierarchy | BindingFlags.Instance, null, parameters, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Adds the given reference to the given collection property
        /// </summary>
        /// <param name="targetResource">The instance whose property to add to</param>
        /// <param name="propertyName">The property to add</param>
        /// <param name="resourceToBeAdded">The resource to add</param>
        public virtual void AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
        {
            ExceptionUtilities.CheckArgumentNotNull(targetResource, "targetResource");
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");
            ExceptionUtilities.CheckArgumentNotNull(resourceToBeAdded, "resourceToBeAdded");

            targetResource = UpdatableToken.AssertIsTokenAndResolve(targetResource, "targetResource");
            resourceToBeAdded = UpdatableToken.AssertIsTokenAndResolve(resourceToBeAdded, "resourceToBeAdded");

            var targetInstance = targetResource as ResourceInstance;
            ExceptionUtilities.CheckObjectNotNull(targetInstance, "Target resource was not a resource instance. Type was '{0}'", targetResource.GetType());

            var addInstance = resourceToBeAdded as ResourceInstance;
            ExceptionUtilities.CheckObjectNotNull(addInstance, "Resource to be added was not a resource instance. Type was '{0}'", targetResource.GetType());

            this.pendingChanges.Add(() => this.AddReferenceToCollection_Internal(targetInstance, propertyName, addInstance, true));
        }

        /// <summary>
        /// Creates a resource in the given set with the given type
        /// </summary>
        /// <param name="containerName">The given set name</param>
        /// <param name="fullTypeName">The given type name</param>
        /// <returns>The resource created</returns>
        public virtual object CreateResource(string containerName, string fullTypeName)
        {
            ExceptionUtilities.CheckArgumentNotNull(fullTypeName, "fullTypeName");

            ResourceType type = this.metadataHelper.ResourceTypes.SingleOrDefault(t => t.FullName == fullTypeName);
            ExceptionUtilities.CheckObjectNotNull(type, "Could not find type with name '{0}'", fullTypeName);
            ExceptionUtilities.ThrowDataServiceExceptionIfFalse(!type.IsAbstract, 400, "Cannot create resource because type '{0}' is abstract.", type.FullName);

            ResourceInstance instance;
            if (type.ResourceTypeKind == ResourceTypeKind.ComplexType)
            {
                ExceptionUtilities.Assert(containerName == null, "Container name must be null for complex types. Value was '{0}'", containerName);

                if (type.InstanceType == typeof(ResourceInstance))
                {
                    instance = new ResourceInstance(type);
                }
                else
                {
                    ConstructorInfo constructor = type.InstanceType.GetConstructor(new[] { typeof(ResourceType) });
                    ExceptionUtilities.CheckObjectNotNull(constructor, "Could not find constructor for complex type '{0}'", type.InstanceType);
                    instance = (ResourceInstance)constructor.Invoke(new object[] { type });
                }
            }
            else
            {
                ExceptionUtilities.CheckArgumentNotNull(containerName, "containerName");
                ResourceSet set = this.GetResourceSet(containerName);
                ExceptionUtilities.CheckObjectNotNull(set, "Could not find resource set with name '{0}'", containerName);

                ExceptionUtilities.Assert(set.ResourceType == type || this.GetDerivedTypesInternal(set.ResourceType).Contains(type), "An entity of type '{0}' cannot be added to set '{1}'", fullTypeName, containerName);

                if (type.InstanceType == typeof(ResourceInstance))
                {
                    instance = new ResourceInstance(type, set);
                }
                else
                {
                    ConstructorInfo constructor = type.InstanceType.GetConstructor(new[] { typeof(ResourceType), typeof(ResourceSet) });
                    ExceptionUtilities.CheckObjectNotNull(constructor, "Could not find constructor for entity type '{0}'", type.InstanceType);
                    instance = (ResourceInstance)constructor.Invoke(new object[] { type, set });
                }

                // TODO: better message
                ExceptionUtilities.ThrowDataServiceExceptionIfFalse(!this.ResourceSetsStorage[set.Name].Any(e => this.AreKeysEqual(e, instance)), 500, "Duplicate key");

                this.pendingChanges.Add(() => this.ResourceSetsStorage[set.Name].Add(instance));
            }

            var token = new UpdatableToken(instance);

            // foreach property marked as being server generated
            foreach (ResourceProperty property in type.GetAllPropertiesLazily())
            {
                string propertyName = property.Name;
                object generatedValue;
                if (this.TryGetStoreGeneratedValue(containerName, fullTypeName, propertyName, out generatedValue))
                {
                    token.PendingPropertyUpdates[propertyName] = generatedValue;
                    this.pendingChanges.Add(() => instance[propertyName] = generatedValue);
                }
            }

            this.InitializeCollectionProperties(type, instance, token, p => p.Kind == ResourcePropertyKind.ResourceSetReference || p.Kind == ResourcePropertyKind.Collection);

            return token;
        }
        
        /// <summary>
        /// Deletes the given resource
        /// </summary>
        /// <param name="targetResource">The resource to delete</param>
        public virtual void DeleteResource(object targetResource)
        {
            ExceptionUtilities.CheckArgumentNotNull(targetResource, "targetResource");

            targetResource = UpdatableToken.AssertIsTokenAndResolve(targetResource, "targetResource");

            var instance = targetResource as ResourceInstance;
            ExceptionUtilities.CheckObjectNotNull(instance, "Target resource was not a resource instance. Type was '{0}'", targetResource.GetType());
            ExceptionUtilities.Assert(instance.IsEntityType, "Target resource was not an entity instance. Resource type was '{0}'", instance.ResourceTypeName);

            this.deletedObjects.Add(targetResource);

            this.DeleteAllReferences(instance);

            this.pendingChanges.Add(() => this.ResourceSetsStorage[instance.ResourceSetName].Remove(instance));
        }

        /// <summary>
        /// Gets the singleton result of the given query
        /// </summary>
        /// <param name="query">The given query</param>
        /// <param name="fullTypeName">The full type name</param>
        /// <returns>The resource from the query</returns>
        public virtual object GetResource(IQueryable query, string fullTypeName)
        {
            ExceptionUtilities.CheckArgumentNotNull(query, "query");

            object resource = GetResourceInternal(query);

            if (resource != null)
            {
                return new UpdatableToken(resource);
            }

            return null;
        }

        /// <summary>
        /// Gets the value of the given property
        /// </summary>
        /// <param name="targetResource">The resource to get the property value from</param>
        /// <param name="propertyName">The property name</param>
        /// <returns>The value of the property</returns>
        public virtual object GetValue(object targetResource, string propertyName)
        {
            ExceptionUtilities.CheckArgumentNotNull(targetResource, "targetResource");
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");

            // resolve the token, and return a pending value if there is one
            // NOTE: this code is specifically to handle cases of mapped complex-type values, because the product does not cache the
            // value returned by CreateResource so we need to take into account any pending updates, or we risk returning stale data
            var token = UpdatableToken.AssertIsToken(targetResource, "targetResource");
            if (token.PendingPropertyUpdates.ContainsKey(propertyName))
            {
                return token.PendingPropertyUpdates[propertyName];
            }

            targetResource = token.Resource;
            var instance = targetResource as ResourceInstance;
            ExceptionUtilities.CheckObjectNotNull(instance, "Target resource was not a resource instance. Type was '{0}'", targetResource.GetType());

            var resourceType = this.metadataHelper.ResourceTypes.Single(t => t.FullName == instance.ResourceTypeName);
            var property = resourceType.GetAllPropertiesLazily().SingleOrDefault(p => p.Name == propertyName);
            if (property != null)
            {
                ExceptionUtilities.Assert(!property.Kind.HasFlag(ResourcePropertyKind.Stream), "GetValue called on stream property '{0}'", property.Name);
            }

            object value = null;
            
            // Check for strongly typed properties
            var type = targetResource.GetType();
            PropertyInfo propertyInfo = type.GetProperty(propertyName);
            if (propertyInfo != null)
            {
                value = propertyInfo.GetValue(targetResource, null);
            }
            else
            {
                object propertyValue;
                if (instance.TryGetValue(propertyName, out propertyValue))
                {
                    value = propertyValue;
                }
            }

            var valueInstance = value as ResourceInstance;
            if (valueInstance != null)
            {
                ExceptionUtilities.Assert(!valueInstance.IsEntityType, "GetValue should never be called for reference properties. Type was '{0}', property was '{1}'", resourceType.FullName, propertyName);
                value = new UpdatableToken(valueInstance);
            }

            return value;
        }

        /// <summary>
        /// Removes the given resource from the given collection property
        /// </summary>
        /// <param name="targetResource">The resource whose collection property to modify</param>
        /// <param name="propertyName">The name of the property</param>
        /// <param name="resourceToBeRemoved">The resource to remove</param>
        public virtual void RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
        {
            ExceptionUtilities.CheckArgumentNotNull(targetResource, "targetResource");
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");
            ExceptionUtilities.CheckArgumentNotNull(resourceToBeRemoved, "resourceToBeRemoved");

            targetResource = UpdatableToken.AssertIsTokenAndResolve(targetResource, "targetResource");
            resourceToBeRemoved = UpdatableToken.AssertIsTokenAndResolve(resourceToBeRemoved, "resourceToBeRemoved");

            var targetInstance = targetResource as ResourceInstance;
            ExceptionUtilities.CheckObjectNotNull(targetInstance, "Target resource was not a resource instance. Type was '{0}'", targetResource.GetType());

            var removeInstance = resourceToBeRemoved as ResourceInstance;
            ExceptionUtilities.CheckObjectNotNull(removeInstance, "Resource to be removed was not a resource instance. Type was '{0}'", targetResource.GetType());

            this.pendingChanges.Add(() => this.RemoveReferenceFromCollection_Internal(targetInstance, propertyName, removeInstance, true));
        }

        /// <summary>
        /// Resets the given resource
        /// </summary>
        /// <param name="resource">The resource to reset</param>
        /// <returns>The reset resource</returns>
        public virtual object ResetResource(object resource)
        {
            ExceptionUtilities.CheckArgumentNotNull(resource, "resource");

            var token = UpdatableToken.AssertIsToken(resource, "resource");
            resource = token.Resource;

            // create a new token
            token = new UpdatableToken(resource);

            var instance = resource as ResourceInstance;
            ExceptionUtilities.CheckObjectNotNull(instance, "Resource was not a resource instance. Type was '{0}'", resource.GetType());

            var resourceType = this.metadataHelper.ResourceTypes.Single(t => t.FullName == instance.ResourceTypeName);
            foreach (ResourceProperty p in resourceType.GetAllPropertiesLazily())
            {
                var property = p;
                if ((property.Kind & (ResourcePropertyKind.Key | ResourcePropertyKind.ResourceReference | ResourcePropertyKind.ResourceSetReference)) == 0)
                {
                    // TODO: initialize to defaults?
                    this.pendingChanges.Add(() => instance.Remove(property.Name));
                }
            }

            // remove all dynamic properties
            // the ToList is so we can modify the collection inside the loop
            foreach (string key in instance.Keys.Except(resourceType.GetAllPropertiesLazily().Select(p => p.Name)).ToList())
            {
                var propertyName = key;
                this.pendingChanges.Add(() => instance.Remove(propertyName));
            }

            this.InitializeCollectionProperties(resourceType, instance, token, p => p.Kind == ResourcePropertyKind.Collection);

            return token;
        }

        /// <summary>
        /// Resolves the given resource
        /// </summary>
        /// <param name="resource">The resource to resolve</param>
        /// <returns>The resolved resource</returns>
        public virtual object ResolveResource(object resource)
        {
            ExceptionUtilities.CheckArgumentNotNull(resource, "resource");
            return UpdatableToken.AssertIsTokenAndResolve(resource, "resource");
        }

        /// <summary>
        /// Saves all pending changes
        /// </summary>
        public virtual void SaveChanges()
        {
            foreach (var pendingChange in this.pendingChanges)
            {
                pendingChange();
            }

            this.pendingChanges.Clear();

            foreach (var deleted in this.deletedObjects)
            {
                foreach (var entity in this.ResourceSetsStorage.SelectMany(p => p.Value))
                {
                    ExceptionUtilities.Assert(!object.ReferenceEquals(deleted, entity), "Found deleted entity!");
                    var resourceType = this.metadataHelper.ResourceTypes.Single(t => t.FullName == entity.ResourceTypeName);

                    foreach (var property in resourceType.GetAllPropertiesLazily())
                    {
                        object value;
                        if (!entity.TryGetValue(property.Name, out value))
                        {
                            continue;
                        }

                        ExceptionUtilities.Assert(!object.ReferenceEquals(deleted, value), "Found deleted entity!");

                        var enumerable = value as IEnumerable;
                        if (enumerable != null)
                        {
                            foreach (var valueElement in enumerable.Cast<object>())
                            {
                                ExceptionUtilities.Assert(!object.ReferenceEquals(deleted, valueElement), "Found deleted entity!");
                            }
                        }
                    }
                }
            }

            this.deletedObjects.Clear();
        }

        /// <summary>
        /// Sets the given reference property
        /// </summary>
        /// <param name="targetResource">The resource whose property to set</param>
        /// <param name="propertyName">The name of the reference property</param>
        /// <param name="propertyValue">The reference to set the property to</param>
        public virtual void SetReference(object targetResource, string propertyName, object propertyValue)
        {
            ExceptionUtilities.CheckArgumentNotNull(targetResource, "targetResource");
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");

            targetResource = UpdatableToken.AssertIsTokenAndResolve(targetResource, "targetResource");
            
            if (propertyValue != null)
            {
                propertyValue = UpdatableToken.AssertIsTokenAndResolve(propertyValue, "propertyValue");
            }

            var targetInstance = targetResource as ResourceInstance;
            ExceptionUtilities.CheckObjectNotNull(targetInstance, "Target resource was not a resource instance. Type was '{0}'", targetResource.GetType());

            ResourceInstance valueInstance = null;
            if (propertyValue != null)
            {
                valueInstance = propertyValue as ResourceInstance;
                ExceptionUtilities.CheckObjectNotNull(valueInstance, "Value was not a resource instance. Type was '{0}'", propertyValue.GetType());
            }

            this.pendingChanges.Add(() => this.SetReference_Internal(targetInstance, propertyName, valueInstance, true));
        }

        /// <summary>
        /// Sets the value of the given property UpdatableToken
        /// </summary>
        /// <param name="targetResource">The resource whose property to set</param>
        /// <param name="propertyName">The name of the property</param>
        /// <param name="propertyValue">The value to set the property to</param>
        public virtual void SetValue(object targetResource, string propertyName, object propertyValue)
        {
            ExceptionUtilities.CheckArgumentNotNull(targetResource, "targetResource");
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");

            // Resolve targetResource
            var token = UpdatableToken.AssertIsToken(targetResource, "targetResource");
            targetResource = token.Resource;

            var instance = targetResource as ResourceInstance;            
            ExceptionUtilities.CheckObjectNotNull(instance, "Resource was not a resource instance. Type was '{0}'", targetResource.GetType());
                                                                                                                           
            if (propertyValue != null)
            {               
                // TODO: Remove if these bugs gets fixed because this is a side affect of those.
                // [Open Types] Server rejects updates to collection properties declared on complex types when accessed via an un-declared complex property 
                // [Open Types] Property and $value update codepath allows modifying undeclared properties on complex types to an arbitrary depth 
                propertyValue = this.ConvertPropertyValueType(propertyName, propertyValue, this.GetResourceTypeInternal(targetResource));
            }

            object generatedValue;
            if (this.TryGetStoreGeneratedValue(instance.ResourceSetName, instance.ResourceTypeName, propertyName, out generatedValue))
            {
                propertyValue = generatedValue;
            }

            token.PendingPropertyUpdates[propertyName] = propertyValue;
            this.SetValue(instance, propertyName, propertyValue);
        }
             
        /// <summary>
        /// Sets the value of the given property ResourceInstance
        /// </summary>
        /// <param name="targetResource">The resource whose property to set</param>
        /// <param name="propertyName">The name of the property</param>
        /// <param name="propertyValue">The value to set the property to</param>
        public virtual void SetValueResourceInstance(object targetResource, string propertyName, object propertyValue)
        {
            ExceptionUtilities.CheckArgumentNotNull(targetResource, "targetResource");
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");

            var instance = targetResource as ResourceInstance;
            ExceptionUtilities.CheckObjectNotNull(targetResource, "Resource was not a resource instance. Type was '{0}'", targetResource.GetType());

            object generatedValue;
            if (this.TryGetStoreGeneratedValue(instance.ResourceSetName, instance.ResourceTypeName, propertyName, out generatedValue))
            {
                propertyValue = generatedValue;
            }

            this.SetValue(instance, propertyName, propertyValue);
        }
        
        /// <summary>
        /// Clears all pending changes
        /// </summary>
        public virtual void ClearChanges()
        {
            this.pendingChanges.Clear();
        }

        /// <summary>
        /// Sets the original concurrency values of the given object so that optimistic concurrency can be enforced. Will throw immediately if they do not match.
        /// </summary>
        /// <param name="resourceCookie">The resource to set original values for</param>
        /// <param name="checkForEquality">Whether the check should be for equality</param>
        /// <param name="concurrencyValues">The concurrency values</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ETag", Justification = "Spelling is correct")]
        public virtual void SetConcurrencyValues(object resourceCookie, bool? checkForEquality, IEnumerable<KeyValuePair<string, object>> concurrencyValues)
        {
            ExceptionUtilities.CheckArgumentNotNull(resourceCookie, "resourceCookie");
            ExceptionUtilities.ThrowDataServiceExceptionIfFalse(checkForEquality.HasValue, 417, "Missing concurrency token for update operation");
            ExceptionUtilities.Assert(checkForEquality.Value, "Should not be called with check for equality parameter equal to false");
            ExceptionUtilities.CheckArgumentNotNull(concurrencyValues, "concurrencyValues");

            // If-Match: *
            if (!concurrencyValues.Any())
            {
                return;
            }

            var instance = UpdatableToken.AssertIsTokenAndResolve(resourceCookie, "resourceCookie") as ResourceInstance;
            ExceptionUtilities.CheckObjectNotNull(instance, "Resource was not a resource instance");
            
            var resourceType = this.metadataHelper.ResourceTypes.Single(t => t.FullName == instance.ResourceTypeName);
            var etagPropertyNames = resourceType.GetAllPropertiesLazily().Where(p => p.Kind.HasFlag(ResourcePropertyKind.ETag)).Select(p => p.Name);

            ExceptionUtilities.ThrowDataServiceExceptionIfFalse(
                etagPropertyNames.SequenceEqual(concurrencyValues.Select(p => p.Key)), 
                412,
                "Unexpected or missing ETag properties");

            foreach (var etagProperty in concurrencyValues)
            {
                object propertyValue = this.GetValue(resourceCookie, etagProperty.Key);

                ExceptionUtilities.ThrowDataServiceExceptionIfFalse(
                    ResourceInstance.ArePropertyValuesEqual(propertyValue, etagProperty.Value),
                    412,
                    "The etag value in the request header does not match with the current etag value of the object.");
            }
        }

        /// <summary>
        /// Get resources
        /// </summary>
        /// <param name="queryable">Queryable to get data from</param>
        /// <returns>Object to return</returns>
        public virtual object GetResources(IQueryable queryable)
        {
            if (DataServiceOverrides.UpdateProvider2.GetResourcesFunc != null)
            {
                return DataServiceOverrides.UpdateProvider2.GetResourcesFunc(queryable);
            }

            return queryable;
        }

        /// <summary>
        /// Adds an invokable to list of operations occuring in savechanges
        /// </summary>
        /// <param name="invokable">Action to invoke at save changes</param>
        public virtual void ScheduleInvokable(IDataServiceInvokable invokable)
        {
            if (DataServiceOverrides.UpdateProvider2.ImmediateCreateInvokableFunc != null)
            {
                DataServiceOverrides.UpdateProvider2.ImmediateCreateInvokableFunc(invokable);
            }
            else if (DataServiceOverrides.UpdateProvider2.AddPendingActionsCreateInvokableFunc != null)
            {
                this.pendingChanges.Add(
                    () =>
                    {
                        DataServiceOverrides.UpdateProvider2.AddPendingActionsCreateInvokableFunc(invokable);
                    });
            }
            else
            {
                this.pendingChanges.Add(
                    () =>
                    {
                        invokable.Invoke();
                    });
            }
        }

        /// <summary>
        /// Gets the instance type of a particular collection property
        /// </summary>
        /// <param name="fullTypeName">The full type name</param>
        /// <param name="propertyName">The property name</param>
        /// <returns>The instance type or null if the type is unknown</returns>
        protected virtual Type GetCollectionPropertyType(string fullTypeName, string propertyName)
        {
            return null;
        }

        /// <summary>
        /// Sets the value of a collection property
        /// </summary>
        /// <param name="targetResource">The resource to set the value on</param>
        /// <param name="propertyName">The property to set</param>
        /// <param name="propertyValue">The collection value</param>
        protected virtual void SetCollectionPropertyValue(ResourceInstance targetResource, string propertyName, IEnumerable propertyValue)
        {
            Type collectionType = this.GetCollectionPropertyType(targetResource.ResourceTypeName, propertyName);
            if (collectionType == null)
            {
                targetResource[propertyName] = propertyValue;
                return;
            }

            // need to go through the enumerable and resolve any tokens
            propertyValue = propertyValue.Cast<object>().Select(o => UpdatableToken.ResolveIfToken(o));

            object collection;
            var enumerableConstructor = collectionType.GetConstructor(new Type[] { typeof(IEnumerable) });
            if (enumerableConstructor != null)
            {
                collection = enumerableConstructor.Invoke(new object[] { propertyValue });
            }
            else if (collectionType.IsGenericType && collectionType.GetGenericArguments().Count() == 1)
            {
                var typeArgument = collectionType.GetGenericArguments().Single();
                var typedEnumerableConstructor = collectionType.GetConstructor(new Type[] { typeof(IEnumerable<>).MakeGenericType(typeArgument) });

                if (typedEnumerableConstructor != null)
                {
                    var typedEnumerable = typeof(Enumerable).GetMethod("Cast").MakeGenericMethod(typeArgument).Invoke(null, new object[] { propertyValue });
                    collection = typedEnumerableConstructor.Invoke(new object[] { typedEnumerable });
                }
                else
                {
                    var typedAddMethod = collectionType.GetMethod("Add", new Type[] { typeArgument });
                    ExceptionUtilities.CheckObjectNotNull(typedAddMethod, "Could not find constructor or add method for type: " + collectionType.FullName);
                    
                    collection = Activator.CreateInstance(collectionType);
                    foreach (var element in propertyValue)
                    {
                        typedAddMethod.Invoke(collection, new object[] { element });
                    }
                }
            }
            else
            {
                var addMethod = collectionType.GetMethod("Add");
                ExceptionUtilities.CheckObjectNotNull(addMethod, "Could not find constructor or add method for type: " + collectionType.FullName);
               
                collection = Activator.CreateInstance(collectionType);
                foreach (var element in propertyValue)
                {
                    addMethod.Invoke(collection, new object[] { element });
                }
            }

            targetResource[propertyName] = collection;
        }

        /// <summary>
        /// Gets the specified resource set.  This is for tests to use instead of the public API to avoid caching issues.
        /// </summary>
        /// <param name="name">The name of the resource set</param>
        /// <returns>The given resource set.</returns>
        protected ResourceSet GetResourceSet(string name)
        {
            ExceptionUtilities.CheckArgumentNotNull(name, "name");
            return this.metadataHelper.ResourceSets.SingleOrDefault(s => s.Name == name);
        }

        /// <summary>
        /// Creates a metadata helper for this context
        /// </summary>
        /// <returns>The metadata helper</returns>
        protected abstract DictionaryMetadataHelper CreateMetadataHelper();

        /// <summary>
        /// Initializes data the first time it is called, then does nothing on subsequent calls
        /// </summary>
        protected abstract void EnsureDataIsInitialized();

        /// <summary>
        /// Attempts to get a store-generated value for the given property of an entity
        /// </summary>
        /// <param name="entitySetName">The entity set name</param>
        /// <param name="fullTypeName">The full type name of the entity</param>
        /// <param name="propertyName">The property name</param>
        /// <param name="propertyValue">The generated property value</param>
        /// <returns>Whether or not a value could (or should) be generated for the given property</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", Justification = "Type cannot be inferred statically")]
        protected virtual bool TryGetStoreGeneratedValue(string entitySetName, string fullTypeName, string propertyName, out object propertyValue)
        {
            propertyValue = null;
            return false;
        }

        /// <summary>
        /// Converts the the property value to the expected property type if it is a declared primitive property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyValue">The property value.</param>
        /// <param name="targetResourceType">Type of the target resource/entity.</param>
        /// <returns>The converted property value</returns>
        protected virtual object ConvertPropertyValueType(string propertyName, object propertyValue, ResourceType targetResourceType)
        {
            var resourceProperty = targetResourceType.GetAllPropertiesLazily().SingleOrDefault(p => p.Name == propertyName);
            if (resourceProperty == null || !resourceProperty.Kind.HasFlag(ResourcePropertyKind.Primitive))
            {
                return propertyValue;
            }
            else
            {
                if (!resourceProperty.ResourceType.InstanceType.IsAssignableFrom(propertyValue.GetType()))
                {
                    var propertyValueString = propertyValue as string;
                    if (propertyValueString != null)
                    {
                        propertyValue = this.ConvertStringToType(propertyValueString, resourceProperty.ResourceType.InstanceType);
                    }
                    else
                    {
                        propertyValue = Convert.ChangeType(propertyValue, resourceProperty.ResourceType.InstanceType, CultureInfo.InvariantCulture);
                    }
                }
            }

            return propertyValue;
        }  

        /// <summary>
        /// Initializes the resource set storage with an empty container for each set.
        /// </summary>
        protected void InitializeResourceSetsStorage()
        {
            ExceptionUtilities.CheckObjectNotNull(this.ResourceSetsStorage, "Resource sets storage has not been created");
            ExceptionUtilities.CheckObjectNotNull(this.metadataHelper, "Metadata helper has not been initialised");

            foreach (var set in this.metadataHelper.ResourceSets)
            {
                if (!this.ResourceSetsStorage.ContainsKey(set.Name))
                {
                    this.ResourceSetsStorage[set.Name] = new List<ResourceInstance>();
                }
            }
        }

        private static object GetResourceInternal(IQueryable query)
        {
            object resource = null;
            foreach (object r in query)
            {
                ExceptionUtilities.Assert(resource == null, "Query returned more than one element. Query was '{0}'", query.Expression);
                resource = r;
            }

            return resource;
        }

        /// <summary>
        /// Returns whether two instances have equivalent keys
        /// </summary>
        /// <param name="instance1">The first instance to compare</param>
        /// <param name="instance2">The second instance to compare</param>
        /// <returns>True if the keys are equal, otherwise false</returns>
        private bool AreKeysEqual(ResourceInstance instance1, ResourceInstance instance2)
        {
            ExceptionUtilities.CheckArgumentNotNull(instance1, "instance1");
            ExceptionUtilities.CheckArgumentNotNull(instance2, "instance2");
            ExceptionUtilities.Assert(instance1.IsEntityType, "Key equality check only supported on entities");
            ExceptionUtilities.Assert(instance2.IsEntityType, "Key equality check only supported on entities");

            if (instance1.ResourceSetName != instance2.ResourceSetName)
            {
                return false;
            }

            var type1 = this.metadataHelper.ResourceTypes.Single(t => t.FullName == instance1.ResourceTypeName);
            var type2 = this.metadataHelper.ResourceTypes.Single(t => t.FullName == instance2.ResourceTypeName);

            if (!type1.KeyProperties.SequenceEqual(type2.KeyProperties))
            {
                return false;
            }

            foreach (var keyProperty in type1.KeyProperties)
            {
                var value1 = instance1.Where(pair => pair.Key == keyProperty.Name).Select(pair => pair.Value).SingleOrDefault();
                var value2 = instance2.Where(pair => pair.Key == keyProperty.Name).Select(pair => pair.Value).SingleOrDefault();
                if (!ResourceInstance.ArePropertyValuesEqual(value1, value2))
                {
                    return false;
                }
            }

            return true;
        }

        private void SetReference_Internal(ResourceInstance targetResource, string propertyName, ResourceInstance propertyValue, bool setBackReference)
        {
            ExceptionUtilities.CheckArgumentNotNull(targetResource, "targetResource");
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");

            var resourceType = this.metadataHelper.ResourceTypes.Single(t => t.FullName == targetResource.ResourceTypeName);
            var property = resourceType.GetAllPropertiesLazily().SingleOrDefault(p => p.Name == propertyName);
            ExceptionUtilities.CheckObjectNotNull(property, "Could not find resource property '{0}' on resource type '{1}'", propertyName, resourceType.FullName);
            ExceptionUtilities.Assert(property.Kind == ResourcePropertyKind.ResourceReference, "SetReference called on property '{0}' with kind '{1}'", propertyName, property.Kind);

            object oldValue;
            if (!targetResource.TryGetValue(propertyName, out oldValue))
            {
                oldValue = null;
            }

            targetResource[propertyName] = propertyValue;
            
            if (setBackReference)
            {
                if (propertyValue == null)
                {
                    propertyValue = (ResourceInstance)oldValue;
                }

                if (propertyValue != null)
                {
                    this.SetReverseNavigation(targetResource, propertyValue, propertyName, (propertyValue == null));
                }
            }
        }

        private void AddReferenceToCollection_Internal(ResourceInstance targetResource, string propertyName, ResourceInstance resourceToBeAdded, bool addBackReference)
        {
            ExceptionUtilities.CheckArgumentNotNull(targetResource, "targetResource");
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");
            ExceptionUtilities.CheckArgumentNotNull(resourceToBeAdded, "resourceToBeAdded");

            var resourceType = this.metadataHelper.ResourceTypes.Single(t => t.FullName == targetResource.ResourceTypeName);
            var property = resourceType.GetAllPropertiesLazily().SingleOrDefault(p => p.Name == propertyName);
            ExceptionUtilities.CheckObjectNotNull(property, "Could not find resource property '{0}' on resource type '{1}'", propertyName, resourceType.FullName);
            ExceptionUtilities.Assert(property.Kind == ResourcePropertyKind.ResourceSetReference, "AddReferenceToCollection called on property '{0}' with kind '{1}'", propertyName, property.Kind);

            object propertyValue;
            if (!targetResource.TryGetValue(propertyName, out propertyValue))
            {
                Type collectionType = this.GetCollectionPropertyType(targetResource.ResourceTypeName, propertyName);
                ExceptionUtilities.CheckObjectNotNull(collectionType, "Could not get collection type for '{0}.{1}'", targetResource.ResourceTypeName, propertyName);
                targetResource[propertyName] = propertyValue = Activator.CreateInstance(collectionType);
            }

            // Check to see if item already exists
            var collection = (IEnumerable)propertyValue;
            foreach (var existingEntity in collection)
            {
                if (this.AreKeysEqual(resourceToBeAdded, (ResourceInstance)existingEntity))
                {
                    // TODO: throw?
                    return;
                }
            }

            var addMethod = collection.GetType().GetMethod("Add");
            ExceptionUtilities.CheckObjectNotNull(addMethod, "Could not find 'Add' method on collection of type: " + collection.GetType());

            addMethod.Invoke(collection, new object[] { resourceToBeAdded });

            if (addBackReference)
            {
                this.SetReverseNavigation(targetResource, resourceToBeAdded, propertyName, false);
            }
        }

        private void RemoveReferenceFromCollection_Internal(ResourceInstance targetResource, string propertyName, ResourceInstance resourceToBeRemoved, bool removeBackReference)
        {
            ExceptionUtilities.CheckArgumentNotNull(targetResource, "targetResource");
            ExceptionUtilities.CheckArgumentNotNull(propertyName, "propertyName");
            ExceptionUtilities.CheckArgumentNotNull(resourceToBeRemoved, "resourceToBeRemoved");

            var resourceType = this.metadataHelper.ResourceTypes.Single(t => t.FullName == targetResource.ResourceTypeName);
            var property = resourceType.GetAllPropertiesLazily().SingleOrDefault(p => p.Name == propertyName);
            ExceptionUtilities.CheckObjectNotNull(property, "Could not find resource property '{0}' on resource type '{1}'", propertyName, resourceType.FullName);
            ExceptionUtilities.Assert(property.Kind == ResourcePropertyKind.ResourceSetReference, "RemoveReferenceFromCollection called on property '{0}' with kind '{1}'", propertyName, property.Kind);

            var collection = (IEnumerable)targetResource[propertyName];
            var removeMethod = collection.GetType().GetMethod("Remove");
            ExceptionUtilities.CheckObjectNotNull(removeMethod, "Could not find 'Remove' method on collection of type: " + collection.GetType());

            removeMethod.Invoke(collection, new object[] { resourceToBeRemoved });

            if (removeBackReference)
            {
                this.SetReverseNavigation(targetResource, resourceToBeRemoved, propertyName, true);
            }
        }

        /// <summary>
        /// For the given association from source to target, perform the appropriate reversed action
        /// </summary>
        /// <param name="source">the source of the association to reverse</param>
        /// <param name="target">the target of the association to reverse</param>
        /// <param name="forwardPropertyName">the name of the property from source to target</param>
        /// <param name="remove">whether or not to remove the reversed association</param>
        private void SetReverseNavigation(ResourceInstance source, ResourceInstance target, string forwardPropertyName, bool remove)
        {
            ResourceType targetType;
            ResourceType sourceType = this.GetResourceTypeInternal(source);

            if (target == null)
            {
                targetType = sourceType.GetAllPropertiesLazily().Single(p => p.Name == forwardPropertyName).ResourceType;
            }
            else
            {
                targetType = this.GetResourceTypeInternal(target);
            }

            ResourceProperty forwardProperty = sourceType.GetAllPropertiesLazily().SingleOrDefault(p => p.Name == forwardPropertyName);

            if (forwardProperty != null && forwardProperty.CustomState != null)
            {
                // because sourceType could match targetType, we need to make sure we filter out the target property
                ResourceProperty reverseProperty = targetType.GetAllPropertiesLazily()
                    .SingleOrDefault(
                    p =>
                    p != forwardProperty && p.CustomState != null &&
                    (string)p.CustomState == (string)forwardProperty.CustomState);

                if (reverseProperty != null)
                {
                    bool reference = (reverseProperty.Kind & ResourcePropertyKind.ResourceReference) != 0;
                    if (remove)
                    {
                        if (reference)
                        {
                            this.SetReference_Internal(target, reverseProperty.Name, null, false);
                        }
                        else
                        {
                            this.RemoveReferenceFromCollection_Internal(target, reverseProperty.Name, source, false);
                        }
                    }
                    else
                    {
                        if (reference)
                        {
                            this.SetReference_Internal(target, reverseProperty.Name, source, false);
                        }
                        else
                        {
                            this.AddReferenceToCollection_Internal(target, reverseProperty.Name, source, false);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The IUpdatable was leaving around left over references after it a resource was being deleted. This will ensure the reference is removed from collections
        /// and reference properties
        /// </summary>
        /// <param name="targetResource">TargetResource To remove</param>
        private void DeleteAllReferences(ResourceInstance targetResource)
        {
            ExceptionUtilities.CheckArgumentNotNull(targetResource, "targetResource");

            ResourceSet targetResourceSet = this.GetResourceSet(targetResource.ResourceSetName);
            ExceptionUtilities.CheckObjectNotNull(targetResourceSet, "Resource set could not be found");
            List<ResourceSet> resourceSetsToCheck = this.metadataHelper.ResourceAssociationSets.Where(ras => ras.End1.ResourceSet == targetResourceSet).Select(ras2 => ras2.End2.ResourceSet).ToList();
            resourceSetsToCheck.AddRange(this.metadataHelper.ResourceAssociationSets.Where(ras => ras.End2.ResourceSet == targetResourceSet).Select(ras2 => ras2.End1.ResourceSet).ToList());
            resourceSetsToCheck = resourceSetsToCheck.Distinct().ToList();

            foreach (var resourceSet in resourceSetsToCheck)
            {
                IList<ResourceInstance> entitySetList = this.ResourceSetsStorage[resourceSet.Name];

                foreach (var currentEntityInstance in entitySetList)
                {
                    ResourceType currentEntityInstanceType = this.GetResourceTypeInternal(currentEntityInstance);

                    foreach (var referenceProperty in currentEntityInstanceType.GetAllPropertiesLazily().Where(p => p.Kind == ResourcePropertyKind.ResourceReference))
                    {
                        if (this.metadataHelper.IsKindOf(currentEntityInstanceType, currentEntityInstanceType))
                        {
                            this.SetEntityReferenceToNullOnTargetResourceMatch(targetResource, referenceProperty, currentEntityInstance);
                        }
                    }

                    foreach (var collectionProperty in currentEntityInstanceType.GetAllPropertiesLazily().Where(p => p.Kind == ResourcePropertyKind.ResourceSetReference))
                    {
                        this.RemoveResourceFromCollectionOnTargetResourceMatch(targetResource, collectionProperty, currentEntityInstance);
                    }
                }
            }
        }

        /// <summary>
        /// Method is done as a cleanup, when a resource is deleted, this method will remove the deleted object from an entityInstance collection property
        /// </summary>
        /// <param name="targetResource">Resource to look for in the currentEntityInstance</param>
        /// <param name="property">The collection property to remove from</param>
        /// <param name="currentEntityInstance">currentEntityInstance that may contain the targetResource</param>
        private void RemoveResourceFromCollectionOnTargetResourceMatch(ResourceInstance targetResource, ResourceProperty property, ResourceInstance currentEntityInstance)
        {
            ExceptionUtilities.CheckArgumentNotNull(targetResource, "targetResource");
            ExceptionUtilities.CheckArgumentNotNull(property, "property");
            ExceptionUtilities.CheckArgumentNotNull(currentEntityInstance, "currentEntityInstance");
            ExceptionUtilities.Assert(property.Kind == ResourcePropertyKind.ResourceSetReference, "Should only be called with reference properties");

            string propertyName = property.Name;
            object propertyValue;
            if (!currentEntityInstance.TryGetValue(propertyName, out propertyValue))
            {
                return;
            }

            IEnumerable childCollectionObject = propertyValue as IEnumerable;
            ExceptionUtilities.CheckObjectNotNull(childCollectionObject, "Value of '{0}.{1}' was null or not a collection", currentEntityInstance.ResourceTypeName, propertyName);

            var removeMethod = childCollectionObject.GetType().GetMethod("Remove");
            ExceptionUtilities.CheckObjectNotNull(removeMethod, "Could not find remove method for collection property '{0}.{1}'", currentEntityInstance.ResourceTypeName, propertyName);

            if (childCollectionObject.Cast<object>().Any(o => o == targetResource))
            {
                this.pendingChanges.Add(() => removeMethod.Invoke(childCollectionObject, new object[] { targetResource }));
            }
        }

        /// <summary>
        /// Method is done as a cleanup, when a resource is deleted, this method will remove the deleted object from an entityInstance reference property
        /// </summary>
        /// <param name="targetResource">Resource to look for in the currentEntityInstance</param>
        /// <param name="property">Reference property to look in on the currentEntityInstance</param>
        /// <param name="currentEntityInstance">currentEntityInstance that may contain the targetResource</param>
        private void SetEntityReferenceToNullOnTargetResourceMatch(ResourceInstance targetResource, ResourceProperty property, ResourceInstance currentEntityInstance)
        {
            ExceptionUtilities.CheckArgumentNotNull(targetResource, "targetResource");
            ExceptionUtilities.CheckArgumentNotNull(property, "property");
            ExceptionUtilities.CheckArgumentNotNull(currentEntityInstance, "currentEntityInstance");
            ExceptionUtilities.Assert(property.Kind == ResourcePropertyKind.ResourceReference, "Should only be called with reference properties");

            string propertyName = property.Name;
            object propertyValue;
            if (currentEntityInstance.TryGetValue(propertyName, out propertyValue) && propertyValue == targetResource)
            {
                this.pendingChanges.Add(() => currentEntityInstance[propertyName] = null);
            }
        }

        private void SetValue(ResourceInstance instance, string propertyName, object propertyValue)
        {
            var resourceType = this.metadataHelper.ResourceTypes.Single(t => t.FullName == instance.ResourceTypeName);

            ResourceProperty property = resourceType.GetAllPropertiesLazily().SingleOrDefault(p => p.Name == propertyName);

            bool isMultiValue = false;
            if (property != null)
            {
                ExceptionUtilities.Assert(!property.Kind.HasFlag(ResourcePropertyKind.Stream), "SetValue called on stream property '{0}'", propertyName);
                isMultiValue = property.Kind.HasFlag(ResourcePropertyKind.Collection);
            }

            if (isMultiValue)
            {
                var enumerable = propertyValue as IEnumerable;
                ExceptionUtilities.CheckObjectNotNull(enumerable, "Collection property value was not an enumerable");

                this.pendingChanges.Add(() => this.SetCollectionPropertyValue(instance, propertyName, enumerable));
            }
            else
            {
                this.pendingChanges.Add(() => instance[propertyName] = UpdatableToken.ResolveIfToken(propertyValue));
            }
        }

        private void InitializeCollectionProperties(ResourceType type, ResourceInstance instance, UpdatableToken token, Func<ResourceProperty, bool> filter)
        {
            foreach (ResourceProperty property in type.GetAllPropertiesLazily().Where(filter))
            {
                string propertyName = property.Name;
                Type collectionType = this.GetCollectionPropertyType(type.FullName, propertyName);
                if (collectionType != null)
                {
                    var newCollection = Activator.CreateInstance(collectionType);
                    token.PendingPropertyUpdates[propertyName] = newCollection;
                    this.pendingChanges.Add(() => instance[propertyName] = newCollection);
                }
            }
        }

        /// <summary>
        /// Gets the resource type for the given instance
        /// </summary>
        /// <param name="resource">The instance to get the type for</param>
        /// <returns>The type of the resource</returns>
        private ResourceType GetResourceTypeInternal(object resource)
        {
            ExceptionUtilities.Assert(!(resource is UpdatableToken), "Should not be called with a token");

            var instance = resource as ResourceInstance;
            if (instance == null)
            {
                return null;
            }

            return this.metadataHelper.ResourceTypes.Single(t => t.FullName == instance.ResourceTypeName);
        }

        /// <summary>
        /// Returns the types derived from the given type
        /// </summary>
        /// <param name="resourceType">The given type</param>
        /// <returns>The type's derived types</returns>
        private IEnumerable<ResourceType> GetDerivedTypesInternal(ResourceType resourceType)
        {
            var children = this.metadataHelper.ResourceTypes.Where(t => t.BaseType == resourceType);
            return children.Union(children.SelectMany(t => this.GetDerivedTypesInternal(t)));
        }

        /// <summary>
        /// Ensures that the product caches resource types based on their names.
        /// </summary>
        /// <param name="type">The resource type which may or may not be cached.</param>
        /// <param name="throwIfAlreadyCached">Whether or not to throw if the resource type is already cached.</param>
        /// <param name="addToCache">Whether or not to add the type to the cache. Certain code paths do not cache all the resource type they could.</param>
        /// <returns>The same resource type if not cached, or a copy if it was.</returns>
        private ResourceType EnforceMetadataCache(ResourceType type, bool throwIfAlreadyCached, bool addToCache)
        {
            if (type == null)
            {
                return null;
            }

            if (!ProviderImplementationSettings.Current.EnforceMetadataCaching)
            {
                return type;
            }

            if (type.ResourceTypeKind != ResourceTypeKind.ComplexType && type.ResourceTypeKind != ResourceTypeKind.EntityType)
            {
                return type;
            }

            bool cached;
            if (addToCache)
            {
                cached = !this.resourceTypeCache.Add(type.FullName);
            }
            else
            {
                cached = this.resourceTypeCache.Contains(type.FullName);
            }

            if (!cached)
            {
                return type;
            }

            ExceptionUtilities.Assert(!throwIfAlreadyCached, "Resource type '{0}' was resolved more than once", type.Name);

            var copy = new ResourceType(typeof(object), type.ResourceTypeKind, null, type.Namespace, type.Name, !type.IsAbstract);
            copy.SetReadOnly();
            return copy;
        }

        /// <summary>
        /// Ensures that the product caches resource sets based on their names.
        /// </summary>
        /// <param name="resourceSet">The resource set which may or may not be cached.</param>
        /// <param name="throwIfAlreadyCached">Whether or not to throw if the resource set is already cached.</param>
        /// <param name="createDummyTypeIfCached">Whether or not to return a dummy type when copying the set for already-cached sets. In some places the product does not correctly resolve the copy to the original.</param>
        /// <param name="addToCache">Whether or not to add the set to the cache. Certain code paths do not cache all the resource sets they could.</param>
        /// <returns>The same resource set if not cached, or a copy if it was.</returns>
        private ResourceSet EnforceMetadataCache(ResourceSet resourceSet, bool throwIfAlreadyCached, bool createDummyTypeIfCached, bool addToCache)
        {
            if (!ProviderImplementationSettings.Current.EnforceMetadataCaching)
            {
                return resourceSet;
            }

            // the ResourceType of a ResourceSet should be cached
            ResourceType type;
            if (createDummyTypeIfCached)
            {
                type = this.EnforceMetadataCache(resourceSet.ResourceType, false, true);
            }
            else
            {
                type = resourceSet.ResourceType;
            }

            bool cached;
            if (addToCache)
            {
                cached = !this.resourceSetCache.Add(resourceSet.Name);
            }
            else
            {
                cached = this.resourceSetCache.Contains(resourceSet.Name);
            }

            if (!cached)
            {
                return resourceSet;
            }

            ExceptionUtilities.Assert(!throwIfAlreadyCached, "Resource set '{0}' was resolved more than once", resourceSet.Name);

            var copy = new ResourceSet(resourceSet.Name, type);
            copy.SetReadOnly();
            return copy;
        }

        /// <summary>
        /// Ensures that the product caches service operations based on their names.
        /// </summary>
        /// <param name="operation">The operation which may or may-not be cached.</param>
        /// <param name="throwIfAlreadyCached">Whether or not to throw if the operation is already cached.</param>
        /// <returns>The same operation if not cached, or a copy if it was.</returns>
        private ServiceOperation EnforceMetadataCache(ServiceOperation operation, bool throwIfAlreadyCached)
        {
            if (!ProviderImplementationSettings.Current.EnforceMetadataCaching)
            {
                return operation;
            }

            ResourceType resultType = null;
            ResourceSet resultSet = null;
            if (operation.ResourceSet != null)
            {
                // The ResourceSet of a service operation should be cached, but it isnt
                resultSet = this.EnforceMetadataCache(operation.ResourceSet, false, true, false);
                resultType = resultSet.ResourceType;
            }
            else
            {
                // The ResultType of a service operation should be cached, but it isn't
                resultType = this.EnforceMetadataCache(operation.ResultType, false, false);
            }

            if (this.serviceOperationCache.Add(operation.Name))
            {
                return operation;
            }

            ExceptionUtilities.Assert(!throwIfAlreadyCached, "Service operation '{0}' was resolved more than once", operation.Name);

            var copy = new ServiceOperation(operation.Name, operation.ResultKind, resultType, resultSet, operation.Method, Enumerable.Empty<ServiceOperationParameter>());
            copy.SetReadOnly();
            return copy;
        }

        /// <summary>
        /// Ensures that the product caches association sets based on the set/type/property name.
        /// </summary>
        /// <param name="associationSet">The association set which may or may-not be cached.</param>
        /// <param name="resourceSet">The resource set used to find the association set.</param>
        /// <param name="resourceType">Type of the resource used to find the association set.</param>
        /// <param name="resourceProperty">The resource property used to find the association set.</param>
        /// <param name="throwIfAlreadyCached">Whether or not to throw if the association set is already cached.</param>
        /// <returns>The same association set if not cached, or a copy if it was.</returns>
        private ResourceAssociationSet EnforceMetadataCache(ResourceAssociationSet associationSet, ResourceSet resourceSet, ResourceType resourceType, ResourceProperty resourceProperty, bool throwIfAlreadyCached)
        {
            if (!ProviderImplementationSettings.Current.EnforceMetadataCaching)
            {
                return associationSet;
            }

            var cacheKey = resourceSet.Name + "_" + resourceType.Name + "_" + (resourceProperty == null ? null : resourceProperty.Name);

            var end1 = associationSet.End1;
            var resourceSet1 = this.EnforceMetadataCache(end1.ResourceSet, false, true, true);
            var resourceType1 = this.EnforceMetadataCache(end1.ResourceType, false, true);
            
            var end2 = associationSet.End2;
            var resourceSet2 = this.EnforceMetadataCache(end2.ResourceSet, false, true, true);
            var resourceType2 = this.EnforceMetadataCache(end2.ResourceType, false, true);
            
            if (this.resourceAssociationSetCache.Add(cacheKey))
            {
                return associationSet;
            }

            ExceptionUtilities.Assert(!throwIfAlreadyCached, "Association set with cache key '{0}' was resolved more than once", cacheKey);

            return new ResourceAssociationSet(
                associationSet.Name, 
                new ResourceAssociationSetEnd(resourceSet1, resourceType1, null), 
                new ResourceAssociationSetEnd(resourceSet2, resourceType2, null));
        }

        private object ConvertStringToType(string propertyValue, Type type)
        {                                   
            // Unwrap if Nullable<T>
            var innertype = Nullable.GetUnderlyingType(type);
            if (innertype != null)
            {                
                type = innertype;
            }
            
            if (type == typeof(DateTimeOffset))
            {
                return XmlConvert.ToDateTimeOffset(propertyValue);
            }
            else if (type == typeof(TimeSpan))
            {
                return XmlConvert.ToTimeSpan(propertyValue);
            }
            else if (type == typeof(Guid))
            {
                return Guid.Parse(propertyValue);
            }
            else if (type == typeof(byte[]))
            {
                return Convert.FromBase64String(propertyValue);
            }
            else if (type == typeof(long))
            {
                return long.Parse(propertyValue, CultureInfo.InvariantCulture);
            }
            else if (type == typeof(decimal))
            {
                return decimal.Parse(propertyValue, CultureInfo.InvariantCulture);
            }            

            throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Property value '{0}' could not be converted to type '{1}'", propertyValue, type.FullName));
        }
    }
}
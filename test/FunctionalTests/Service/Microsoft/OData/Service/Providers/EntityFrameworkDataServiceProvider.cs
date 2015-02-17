//---------------------------------------------------------------------
// <copyright file="EntityFrameworkDataServiceProvider.cs" company="Microsoft">
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
    #endregion

#if !EF6Provider
    /// <summary>
    /// Entity Framework based provider.
    /// </summary>
    public class EntityFrameworkDataServiceProvider : 
#else
    /// <summary>
    /// Entity Framework based provider.
    /// </summary>
    /// <typeparam name="T">Type of the dbcontext datasource</typeparam>
    public class EntityFrameworkDataServiceProvider2<T> :     
#endif
 IDataServiceMetadataProvider, 
        IDataServiceQueryProvider, 
        IDataServiceUpdateProvider2,
        IDataServiceInternalProvider,
        IDataServiceProviderBehavior,
        IDataServiceEntityFrameworkProvider,
        IServiceProvider, 
        IDisposable
    {
        /// <summary>
        /// ObjectContextServiceProvider which provides implementation of metadata, query and update interfaces.
        /// </summary>
        private ObjectContextServiceProvider innerProvider;

        /// <summary>
        /// Initializes a new Microsoft.OData.Service.Providers.EntityFrameworkDataServiceProvider instance.
        /// </summary>
        /// <param name="args"><see cref="DataServiceProviderArgs"/> needed during provider construction.</param>
#if !EF6Provider
        public EntityFrameworkDataServiceProvider(DataServiceProviderArgs args)
#else
        public EntityFrameworkDataServiceProvider2(DataServiceProviderArgs args)
#endif
        {
            WebUtil.CheckArgumentNull(args, "args");

            this.CreateInnerProvider(args.DataServiceInstance, args.DataSourceInstance);

            this.LoadMetadata(args.SkipServiceOperationMetadata);

            // Load known types and set ordering property and make readonly
            this.innerProvider.FinalizeMetadataModel(args.KnownTypes, args.UseMetadataKeyOrder);
        }

        /// <summary>
        /// Initializes a new Microsoft.OData.Service.Providers.EntityFrameworkDataServiceProvider instance.
        /// </summary>
        /// <param name="dataServiceInstance">Required data service instance.</param>
        /// <param name="dataSourceInstance">Required data source instance.</param>
#if !EF6Provider
        internal EntityFrameworkDataServiceProvider(object dataServiceInstance, object dataSourceInstance)
#else
        internal EntityFrameworkDataServiceProvider2(object dataServiceInstance, object dataSourceInstance)
#endif
        {
            this.CreateInnerProvider(dataServiceInstance, dataSourceInstance);

            this.LoadMetadata(false /*skipServiceOperations*/);
        }

        #region IDataServiceMetadataProvider

        /// <summary>Namespace name for the EDM container.</summary>
        public virtual string ContainerNamespace
        {
            get
            {
                return this.innerProvider.ContainerNamespace;
            }
        }

        /// <summary>Name of the EDM container</summary>
        public virtual string ContainerName
        {
            get
            {
                return this.innerProvider.ContainerName;
            }
        }

        /// <summary>Gets all available containers.</summary>
        public virtual IEnumerable<ResourceSet> ResourceSets
        {
            get
            {
                return this.innerProvider.ResourceSets;
            }
        }

        /// <summary>Returns all the types in this data source</summary>
        public virtual IEnumerable<ResourceType> Types
        {
            get
            {
                return this.innerProvider.Types;
            }
        }

        /// <summary>Returns all the service operations in this data source</summary>
        public virtual IEnumerable<ServiceOperation> ServiceOperations
        {
            get
            {
                return this.innerProvider.ServiceOperations;
            }
        }

        #endregion IDataServiceMetadataProvider

        #region IDataServiceQueryProvider

        /// <summary>The data source from which data is provided.</summary>
        public virtual object CurrentDataSource
        {
            get
            {
                return this.innerProvider.CurrentDataSource;
            }

            set
            {
                this.innerProvider.CurrentDataSource = value;
            }
        }

        /// <summary>Gets a value indicating whether null propagation is required in expression trees.</summary>
        public virtual bool IsNullPropagationRequired
        {
            get
            {
                return this.innerProvider.IsNullPropagationRequired;
            }
        }

        #endregion IDataServiceQueryProvider

        #region IDataServiceProviderBehavior

        /// <summary>
        /// Instance of provider behavior that defines the assumptions service should make
        /// about the provider.
        /// </summary>
        public ProviderBehavior ProviderBehavior
        {
            get
            {
                return this.innerProvider.ProviderBehavior;
            }
        }

        #endregion IDataServiceProviderBehavior

        #region IDataServiceEntityFrameworkProvider

        /// <summary>
        /// Gets the metadata schema version.
        /// </summary>
        public virtual MetadataEdmSchemaVersion EdmSchemaVersion
        {
            get
            {
                return this.innerProvider.EdmSchemaVersion;
            }
        }

        #endregion IDataServiceEntityFrameworkProvider

        #region IDataServiceMetadataProvider

        /// <summary>Given the specified name, tries to find a resource set.</summary>
        /// <param name="name">Name of the resource set to resolve.</param>
        /// <param name="resourceSet">Returns the resolved resource set, null if no resource set for the given name was found.</param>
        /// <returns>True if resource set with the given name was found, false otherwise.</returns>
        public virtual bool TryResolveResourceSet(string name, out ResourceSet resourceSet)
        {
            return this.innerProvider.TryResolveResourceSet(name, out resourceSet);
        }

        /// <summary>
        /// Gets the ResourceAssociationSet instance when given the source association end.
        /// </summary>
        /// <param name="resourceSet">Resource set of the source association end.</param>
        /// <param name="resourceType">Resource type of the source association end.</param>
        /// <param name="resourceProperty">Resource property of the source association end.</param>
        /// <returns>ResourceAssociationSet instance.</returns>
        public virtual ResourceAssociationSet GetResourceAssociationSet(ResourceSet resourceSet, ResourceType resourceType, ResourceProperty resourceProperty)
        {
            return this.innerProvider.GetResourceAssociationSet(resourceSet, resourceType, resourceProperty);
        }

        /// <summary>Given the specified name, tries to find a type.</summary>
        /// <param name="name">Name of the type to resolve.</param>
        /// <param name="resourceType">Returns the resolved resource type, null if no resource type for the given name was found.</param>
        /// <returns>True if we found the resource type for the given name, false otherwise.</returns>
        public virtual bool TryResolveResourceType(string name, out ResourceType resourceType)
        {
            return this.innerProvider.TryResolveResourceType(name, out resourceType);
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
        public virtual IEnumerable<ResourceType> GetDerivedTypes(ResourceType resourceType)
        {
            return this.innerProvider.GetDerivedTypes(resourceType);
        }

        /// <summary>
        /// Returns true if <paramref name="resourceType"/> represents an Entity Type which has derived Entity Types, else false.
        /// </summary>
        /// <param name="resourceType">instance of the resource type in question.</param>
        /// <returns>True if <paramref name="resourceType"/> represents an Entity Type which has derived Entity Types, else false.</returns>
        public virtual bool HasDerivedTypes(ResourceType resourceType)
        {
            return this.innerProvider.HasDerivedTypes(resourceType);
        }

        /// <summary>Given the specified name, tries to find a service operation.</summary>
        /// <param name="name">Name of the service operation to resolve.</param>
        /// <param name="serviceOperation">Returns the resolved service operation, null if no service operation was found for the given name.</param>
        /// <returns>True if we found the service operation for the given name, false otherwise.</returns>
        public virtual bool TryResolveServiceOperation(string name, out ServiceOperation serviceOperation)
        {
            return this.innerProvider.TryResolveServiceOperation(name, out serviceOperation);
        }

        #endregion IDataServiceMetadataProvider

        #region IDataServiceQueryProvider

        /// <summary>
        /// Returns the IQueryable that represents the resource set.
        /// </summary>
        /// <param name="resourceSet">resource set representing the entity set.</param>
        /// <returns>
        /// An IQueryable that represents the set; null if there is 
        /// no set for the specified name.
        /// </returns>
        public virtual IQueryable GetQueryRootForResourceSet(ResourceSet resourceSet)
        {
            return this.innerProvider.GetQueryRootForResourceSet(resourceSet);
        }

        /// <summary>Gets the <see cref="ResourceType"/> for the specified <paramref name="target"/>.</summary>
        /// <param name="target">Target instance to extract a <see cref="ResourceType"/> from.</param>
        /// <returns>The <see cref="ResourceType"/> that describes this <paramref name="target"/> in this provider.</returns>
        public virtual ResourceType GetResourceType(object target)
        {
            return this.innerProvider.GetResourceType(target);
        }

        /// <summary>
        /// Get the value of the strongly typed property.
        /// </summary>
        /// <param name="target">instance of the type declaring the property.</param>
        /// <param name="resourceProperty">resource property describing the property.</param>
        /// <returns>value for the property.</returns>
        public virtual object GetPropertyValue(object target, ResourceProperty resourceProperty)
        {
            return this.innerProvider.GetPropertyValue(target, resourceProperty);
        }

        /// <summary>
        /// Get the value of the open property.
        /// </summary>
        /// <param name="target">instance of the type declaring the open property.</param>
        /// <param name="propertyName">name of the open property.</param>
        /// <returns>value for the open property.</returns>
        public virtual object GetOpenPropertyValue(object target, string propertyName)
        {
            return this.innerProvider.GetOpenPropertyValue(target, propertyName);
        }

        /// <summary>
        /// Get the name and values of all the properties defined in the given instance of an open type.
        /// </summary>
        /// <param name="target">instance of a open type.</param>
        /// <returns>collection of name and values of all the open properties.</returns>
        public virtual IEnumerable<KeyValuePair<string, object>> GetOpenPropertyValues(object target)
        {
            return this.innerProvider.GetOpenPropertyValues(target);
        }

        /// <summary>
        /// Invoke the given service operation and returns the results.
        /// </summary>
        /// <param name="serviceOperation">service operation to invoke.</param>
        /// <param name="parameters">value of parameters to pass to the service operation.</param>
        /// <returns>returns the result of the service operation. If the service operation returns void, then this should return null.</returns>
        public virtual object InvokeServiceOperation(ServiceOperation serviceOperation, object[] parameters)
        {
            return this.innerProvider.InvokeServiceOperation(serviceOperation, parameters);
        }

        #endregion IDataServiceQueryProvider

        #region IDataServiceUpdateProvider2

        /// <summary>
        /// Queues up the <paramref name="invokable"/> to be invoked during IUpdatable.SaveChanges().
        /// </summary>
        /// <param name="invokable">The invokable instance whose Invoke() method will be called during IUpdatable.SaveChanges().</param>
        public virtual void ScheduleInvokable(IDataServiceInvokable invokable)
        {
            this.innerProvider.ScheduleInvokable(invokable);
        }

        /// <summary>
        /// Passes the etag value for the given resource.
        /// </summary>
        /// <param name="resourceCookie">cookie representing the resource.</param>
        /// <param name="checkForEquality">true if we need to compare the property values for equality. If false, then we need to compare values for non-equality.</param>
        /// <param name="concurrencyValues">list of the etag property names and its corresponding values.</param>
        public virtual void SetConcurrencyValues(object resourceCookie, bool? checkForEquality, IEnumerable<KeyValuePair<string, object>> concurrencyValues)
        {
            this.innerProvider.SetConcurrencyValues(resourceCookie, checkForEquality, concurrencyValues);
        }

        /// <summary>
        /// Creates the resource of the given type and belonging to the given container
        /// </summary>
        /// <param name="containerName">container name to which the resource needs to be added</param>
        /// <param name="fullTypeName">full type name i.e. Namespace qualified type name of the resource</param>
        /// <returns>object representing a resource of given type and belonging to the given container</returns>
        public virtual object CreateResource(string containerName, string fullTypeName)
        {
            return this.innerProvider.CreateResource(containerName, fullTypeName);
        }

        /// <summary>
        /// Gets the resource of the given type that the query points to
        /// </summary>
        /// <param name="query">query pointing to a particular resource</param>
        /// <param name="fullTypeName">full type name i.e. Namespace qualified type name of the resource</param>
        /// <returns>object representing a resource of given type and as referenced by the query</returns>
        public virtual object GetResource(IQueryable query, string fullTypeName)
        {
            return this.innerProvider.GetResource(query, fullTypeName);
        }

        /// <summary>
        /// Resets the value of the given resource to its default value
        /// </summary>
        /// <param name="resource">resource whose value needs to be reset</param>
        /// <returns>same resource with its value reset</returns>
        public virtual object ResetResource(object resource)
        {
            return this.innerProvider.ResetResource(resource);
        }

        /// <summary>
        /// Sets the value of the given property on the target object
        /// </summary>
        /// <param name="targetResource">target object which defines the property</param>
        /// <param name="propertyName">name of the property whose value needs to be updated</param>
        /// <param name="propertyValue">value of the property</param>
        public virtual void SetValue(object targetResource, string propertyName, object propertyValue)
        {
            this.innerProvider.SetValue(targetResource, propertyName, propertyValue);
        }

        /// <summary>
        /// Gets the value of the given property on the target object
        /// </summary>
        /// <param name="targetResource">target object which defines the property</param>
        /// <param name="propertyName">name of the property whose value needs to be updated</param>
        /// <returns>the value of the property for the given target resource</returns>
        public virtual object GetValue(object targetResource, string propertyName)
        {
            return this.innerProvider.GetValue(targetResource, propertyName);
        }

        /// <summary>
        /// Sets the value of the given reference property on the target object
        /// </summary>
        /// <param name="targetResource">target object which defines the property</param>
        /// <param name="propertyName">name of the property whose value needs to be updated</param>
        /// <param name="propertyValue">value of the property</param>
        public virtual void SetReference(object targetResource, string propertyName, object propertyValue)
        {
            this.innerProvider.SetReference(targetResource, propertyName, propertyValue);
        }

        /// <summary>
        /// Adds the given value to the collection
        /// </summary>
        /// <param name="targetResource">target object which defines the property</param>
        /// <param name="propertyName">name of the property whose value needs to be updated</param>
        /// <param name="resourceToBeAdded">value of the property which needs to be added</param>
        public virtual void AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
        {
            this.innerProvider.AddReferenceToCollection(targetResource, propertyName, resourceToBeAdded);
        }

        /// <summary>
        /// Removes the given value from the collection
        /// </summary>
        /// <param name="targetResource">target object which defines the property</param>
        /// <param name="propertyName">name of the property whose value needs to be updated</param>
        /// <param name="resourceToBeRemoved">value of the property which needs to be removed</param>
        public virtual void RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
        {
            this.innerProvider.RemoveReferenceFromCollection(targetResource, propertyName, resourceToBeRemoved);
        }

        /// <summary>
        /// Delete the given resource
        /// </summary>
        /// <param name="targetResource">resource that needs to be deleted</param>
        public virtual void DeleteResource(object targetResource)
        {
            this.innerProvider.DeleteResource(targetResource);
        }

        /// <summary>
        /// Saves all the pending changes made till now
        /// </summary>
        public virtual void SaveChanges()
        {
            this.innerProvider.SaveChanges();
        }

        /// <summary>
        /// Returns the actual instance of the resource represented by the given resource object
        /// </summary>
        /// <param name="resource">object representing the resource whose instance needs to be fetched</param>
        /// <returns>The actual instance of the resource represented by the given resource object</returns>
        public virtual object ResolveResource(object resource)
        {
            return this.innerProvider.ResolveResource(resource);
        }

        /// <summary>
        /// Revert all the pending changes.
        /// </summary>
        public virtual void ClearChanges()
        {
            this.innerProvider.ClearChanges();
        }

        #endregion IDataServiceUpdateProvider2

        #region IDataServiceInternalProvider

        /// <summary>
        /// Called by the service to let the provider perform data model validation.
        /// </summary>
        /// <param name="knownTypes">Collection of known types.</param>
        /// <param name="useMetadataCacheOrder">Whether to use metadata cache ordering instead of default ordering.</param>
        public void FinalizeMetadataModel(IEnumerable<Type> knownTypes, bool useMetadataCacheOrder)
        {
            this.innerProvider.FinalizeMetadataModel(knownTypes, useMetadataCacheOrder);
        }

        /// <summary>
        /// Return the list of custom annotation for the entity container with the given name.
        /// </summary>
        /// <param name="entityContainerName">Name of the EntityContainer.</param>
        /// <returns>Return the list of custom annotation for the entity container with the given name.</returns>
        public IEnumerable<KeyValuePair<string, object>> GetEntityContainerAnnotations(string entityContainerName)
        {
            return this.innerProvider.GetEntityContainerAnnotations(entityContainerName);
        }

        #endregion IDataServiceInternalProvider

        #region IDataServiceEntityFrameworkProvider

        /// <summary>
        /// Given a resource container and resource type, gets the list of ResourceProperties that
        /// are part of the ETag.
        /// </summary>
        /// <param name="containerName">Resource set name.</param>
        /// <param name="resourceType">Resource type of entities in the resource container.</param>
        /// <returns>Collection of properties that are part of the ETag.</returns>
        public virtual IList<ResourceProperty> GetETagProperties(string containerName, ResourceType resourceType)
        {
            return this.innerProvider.GetETagProperties(containerName, resourceType);
        }

        #endregion IDataServiceEntityFrameworkProvider

        #region IServiceProvider

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>A service object of type serviceType.-or- null if there is no service object of type serviceType.</returns>
        public virtual object GetService(Type serviceType)
        {
            if (typeof(IDataServiceMetadataProvider) == serviceType ||
                typeof(IDataServiceQueryProvider) == serviceType ||
                typeof(IDataServiceUpdateProvider2) == serviceType ||
                typeof(IDataServiceUpdateProvider) == serviceType ||
                typeof(IUpdatable) == serviceType ||
                typeof(IDataServiceProviderBehavior) == serviceType ||
                typeof(IDataServiceEntityFrameworkProvider) == serviceType)
            {
                return this;
            }

            return null;
        }

        #endregion IServiceProvider

        #region IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// Disposes the current object.
        /// </summary>
        /// <param name="disposing">Whethere the call is coming from IDisposable interface.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    if (this.innerProvider != null)
                    {
                        (this.innerProvider as IDisposable).Dispose();
                    }
                }
                finally
                {
                    this.innerProvider = null;
                }
            }
        }

        /// <summary>
        /// Creates the inner provider object.
        /// </summary>
        /// <param name="dataServiceInstance">Data service instance.</param>
        /// <param name="dataSourceInstance">Data source instance.</param>
        private void CreateInnerProvider(object dataServiceInstance, object dataSourceInstance)
        {
            Debug.Assert(dataServiceInstance != null, "dataServiceInstance != null");
            Debug.Assert(dataSourceInstance != null, "dataSourceInstance != null");
            this.innerProvider = new ObjectContextServiceProvider(dataServiceInstance, dataSourceInstance);
        }

        /// <summary>
        /// Initializes the provider by loading metdata information.
        /// </summary>
        /// <param name="skipServiceOperations">Should service operations be loaded.</param>
        private void LoadMetadata(bool skipServiceOperations)
        {
            Debug.Assert(this.innerProvider != null, "Inner Provider must have been initialized.");
            this.innerProvider.LoadMetadata(skipServiceOperations);
        }
    }
}

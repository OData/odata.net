//---------------------------------------------------------------------
// <copyright file="HybridProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.PublicProvider
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.OData.Services.Astoria;
    using Microsoft.Test.OData.Services.AstoriaDefaultService;

    /// <summary>
    /// The hybrid provider
    /// </summary>
    public class HybridProvider :
        IDataServiceMetadataProvider,
        IDataServiceQueryProvider,
        IDataServiceUpdateProvider2,
        IDataServiceProviderBehavior,
        IDataServiceEntityFrameworkProvider,
        IDisposable
    {
        private ReflectionDataServiceProvider reflectionProvider;
        private EntityFrameworkDataServiceProvider efProvider;

        private readonly ServiceOperationProvider serviceOperationProvider;

        private readonly HybridService hybridService;

        /// <summary>
        /// create an instance of class HybridProvider
        /// </summary>
        /// <param name="service">The hybrid service</param>
        /// <param name="dataSource">The hybrid datasource</param>
        public HybridProvider(HybridService service, HybridDataSource dataSource)
        {
            reflectionProvider =
                new ReflectionProvider(
                    new DataServiceProviderArgs(service, dataSource.ReflectionDataSource, null, false) { SkipServiceOperationMetadata = true });

            efProvider =
                new EFProvider(
                    new DataServiceProviderArgs(service, dataSource.DatabaseSource, null, false) { SkipServiceOperationMetadata = true });

            serviceOperationProvider = new ServiceOperationProvider(service.GetType(), ResolveResourceType, ResolveResourceSet);

            hybridService = service;
        }

        /// <summary>
        /// Resolve resource type by clr type
        /// </summary>
        /// <param name="type">The clr type</param>
        /// <returns>The resource type</returns>
        private ResourceType ResolveResourceType(Type type)
        {
            return Types.SingleOrDefault(t => t.InstanceType == type);
        }

        /// <summary>
        /// Resolve resourceset by resource type
        /// </summary>
        /// <param name="resourceType">The resource type</param>
        /// <param name="method">The method</param>
        /// <returns>The resource type</returns>
        private ResourceSet ResolveResourceSet(ResourceType resourceType, MethodInfo method)
        {
            return ResourceSets.SingleOrDefault(rs => rs.ResourceType == resourceType);
        }

        #region IDataServiceMetadataProvider

        /// <summary>Namespace name for the EDM container.</summary>
        public string ContainerNamespace
        {
            get
            {
                Log.Trace();
                return "HybridService";
            }
        }

        /// <summary>Name of the EDM container</summary>
        public string ContainerName
        {
            get
            {
                Log.Trace();
                // The container name has to be EF provider, so ReflectionProvider will pick up this value and end up merge two provider into one EntityContainer
                return efProvider.ContainerName;
            }
        }

        /// <summary>Gets all available containers.</summary>
        public IEnumerable<ResourceSet> ResourceSets
        {
            get
            {
                Log.Trace();
                return efProvider.ResourceSets.Union(reflectionProvider.ResourceSets);
            }
        }

        /// <summary>Returns all the types in this data source</summary>
        public IEnumerable<ResourceType> Types
        {
            get
            {
                Log.Trace();
                return efProvider.Types.Union(reflectionProvider.Types);
            }
        }

        /// <summary>Returns all the service operations in this data source</summary>
        public IEnumerable<ServiceOperation> ServiceOperations
        {
            get
            {
                Log.Trace();
                var sos = serviceOperationProvider.ServiceOperations.ToArray();
                foreach (var so in sos)
                {
                    so.SetReadOnly();
                }
                return sos;
            }
        }

        /// <summary>Given the specified name, tries to find a resource set.</summary>
        /// <param name="name">Name of the resource set to resolve.</param>
        /// <param name="resourceSet">Returns the resolved resource set, null if no resource set for the given name was found.</param>
        /// <returns>True if resource set with the given name was found, false otherwise.</returns>
        public bool TryResolveResourceSet(string name, out ResourceSet resourceSet)
        {
            Log.Trace(name);
            var ret = efProvider.TryResolveResourceSet(name, out resourceSet);
            if (!ret)
            {
                ret = reflectionProvider.TryResolveResourceSet(name, out resourceSet);
            }
            return ret;
        }

        /// <summary>
        /// Gets the ResourceAssociationSet instance when given the source association end.
        /// </summary>
        /// <param name="resourceSet">Resource set of the source association end.</param>
        /// <param name="resourceType">Resource type of the source association end.</param>
        /// <param name="resourceProperty">Resource property of the source association end.</param>
        /// <returns>ResourceAssociationSet instance.</returns>
        public ResourceAssociationSet GetResourceAssociationSet(ResourceSet resourceSet, ResourceType resourceType,
                                                                ResourceProperty resourceProperty)
        {
            try
            {
                Log.Trace();
                return efProvider.GetResourceAssociationSet(resourceSet, resourceType, resourceProperty);
            }
            catch (ArgumentException)
            {
                return reflectionProvider.GetResourceAssociationSet(resourceSet, resourceType, resourceProperty);
            }
        }

        /// <summary>Given the specified name, tries to find a type.</summary>
        /// <param name="name">Name of the type to resolve.</param>
        /// <param name="resourceType">Returns the resolved resource type, null if no resource type for the given name was found.</param>
        /// <returns>True if we found the resource type for the given name, false otherwise.</returns>
        public bool TryResolveResourceType(string name, out ResourceType resourceType)
        {
            Log.Trace(name);
            var ret = efProvider.TryResolveResourceType(name, out resourceType);
            if (!ret)
            {
                ret = reflectionProvider.TryResolveResourceType(name, out resourceType);
            }
            return ret;
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
            Log.Trace(resourceType.Namespace, resourceType.Name);
            try
            {
                // Use ToArray to invoke the enumerator to ensure EF provider can handle this resource type.
                return efProvider.GetDerivedTypes(resourceType).ToArray();
            }
            catch (InvalidOperationException)
            {
                // Use ToArray to invoke the enumerator to ensure Reflection provider can handle this resource type.
                return reflectionProvider.GetDerivedTypes(resourceType).ToArray();
            }
        }

        /// <summary>
        /// Returns true if <paramref name="resourceType"/> represents an Entity Type which has derived Entity Types, else false.
        /// </summary>
        /// <param name="resourceType">instance of the resource type in question.</param>
        /// <returns>True if <paramref name="resourceType"/> represents an Entity Type which has derived Entity Types, else false.</returns>
        public bool HasDerivedTypes(ResourceType resourceType)
        {
            Log.Trace(resourceType.Namespace, resourceType.Name);
            try
            {
                return efProvider.HasDerivedTypes(resourceType);
            }
            catch (InvalidOperationException)
            {
                return reflectionProvider.HasDerivedTypes(resourceType);
            }
        }

        /// <summary>Given the specified name, tries to find a service operation.</summary>
        /// <param name="name">Name of the service operation to resolve.</param>
        /// <param name="serviceOperation">Returns the resolved service operation, null if no service operation was found for the given name.</param>
        /// <returns>True if we found the service operation for the given name, false otherwise.</returns>
        public bool TryResolveServiceOperation(string name, out ServiceOperation serviceOperation)
        {
            Log.Trace(name);
            serviceOperation = serviceOperationProvider.ServiceOperations.SingleOrDefault(so => so.Name == name);
            if (serviceOperation != null)
            {
                serviceOperation.SetReadOnly();
            }
            return serviceOperation != null;
        }

        #endregion IDataServiceMetadataProvider

        #region IDataServiceQueryProvider

        /// <summary>The data source from which data is provided.</summary>
        public object CurrentDataSource
        {
            get
            {
                Log.Trace();
                return new HybridDataSource
                    {
                        DatabaseSource = (AstoriaDefaultServiceDBEntities)efProvider.CurrentDataSource,
                        ReflectionDataSource = (DefaultContainer)reflectionProvider.CurrentDataSource
                    };
            }
            set
            {
                Log.Trace();
                var source = (HybridDataSource)value;
                efProvider.CurrentDataSource = source.DatabaseSource;
                reflectionProvider.CurrentDataSource = source.ReflectionDataSource;
            }
        }

        /// <summary>Gets a value indicating whether null propagation is required in expression trees.</summary>
        public bool IsNullPropagationRequired
        {
            get
            {
                Log.Trace();
                return GetCurrentProvider<IDataServiceQueryProvider>().IsNullPropagationRequired;
            }
        }

        /// <summary>
        /// Returns the IQueryable that represents the resource set.
        /// </summary>
        /// <param name="resourceSet">resource set representing the entity set.</param>
        /// <returns>
        /// An IQueryable that represents the set; null if there is 
        /// no set for the specified name.
        /// </returns>
        public IQueryable GetQueryRootForResourceSet(ResourceSet resourceSet)
        {
            Log.Trace(resourceSet.Name);
            return GetCurrentProvider<IDataServiceQueryProvider>().GetQueryRootForResourceSet(resourceSet);
        }

        /// <summary>Gets the <see cref="ResourceType"/> for the specified <paramref name="target"/>.</summary>
        /// <param name="target">Target instance to extract a <see cref="ResourceType"/> from.</param>
        /// <returns>The <see cref="ResourceType"/> that describes this <paramref name="target"/> in this provider.</returns>
        public ResourceType GetResourceType(object target)
        {
            Log.Trace(target);
            return GetCurrentProvider<IDataServiceQueryProvider>().GetResourceType(target);
        }

        /// <summary>
        /// Get the value of the strongly typed property.
        /// </summary>
        /// <param name="target">instance of the type declaring the property.</param>
        /// <param name="resourceProperty">resource property describing the property.</param>
        /// <returns>value for the property.</returns>
        public object GetPropertyValue(object target, ResourceProperty resourceProperty)
        {
            Log.Trace(target, resourceProperty.Name);
            return GetCurrentProvider<IDataServiceQueryProvider>().GetPropertyValue(target, resourceProperty);
        }

        /// <summary>
        /// Get the value of the open property.
        /// </summary>
        /// <param name="target">instance of the type declaring the open property.</param>
        /// <param name="propertyName">name of the open property.</param>
        /// <returns>value for the open property.</returns>
        public object GetOpenPropertyValue(object target, string propertyName)
        {
            Log.Trace(target, propertyName);
            return GetCurrentProvider<IDataServiceQueryProvider>().GetOpenPropertyValue(target, propertyName);
        }

        /// <summary>
        /// Get the name and values of all the properties defined in the given instance of an open type.
        /// </summary>
        /// <param name="target">instance of a open type.</param>
        /// <returns>collection of name and values of all the open properties.</returns>
        public IEnumerable<KeyValuePair<string, object>> GetOpenPropertyValues(object target)
        {
            Log.Trace(target);
            return GetCurrentProvider<IDataServiceQueryProvider>().GetOpenPropertyValues(target);
        }

        /// <summary>
        /// Invoke the given service operation and returns the results.
        /// </summary>
        /// <param name="serviceOperation">service operation to invoke.</param>
        /// <param name="parameters">value of parameters to pass to the service operation.</param>
        /// <returns>returns the result of the service operation. If the service operation returns void, then this should return null.</returns>
        public object InvokeServiceOperation(ServiceOperation serviceOperation, object[] parameters)
        {
            Log.Trace(serviceOperation);
            try
            {
                return ((MethodInfo)serviceOperation.CustomState).Invoke(
                    hybridService,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy,
                    null,
                    parameters,
                    CultureInfo.InvariantCulture);
            }
            catch (TargetInvocationException exception)
            {
                Log.Trace(exception);
                throw;
            }
        }

        #endregion IDataServiceQueryProvider

        #region IUpdatable

        /// <summary>
        /// Creates the resource of the given type and belonging to the given container
        /// </summary>
        /// <param name="containerName">container name to which the resource needs to be added</param>
        /// <param name="fullTypeName">full type name i.e. Namespace qualified type name of the resource</param>
        /// <returns>object representing a resource of given type and belonging to the given container</returns>
        public object CreateResource(string containerName, string fullTypeName)
        {
            return GetCurrentProvider<IUpdatable>().CreateResource(containerName, fullTypeName);
        }

        /// <summary>
        /// Gets the resource of the given type that the query points to
        /// </summary>
        /// <param name="query">query pointing to a particular resource</param>
        /// <param name="fullTypeName">full type name i.e. Namespace qualified type name of the resource</param>
        /// <returns>object representing a resource of given type and as referenced by the query</returns>
        public object GetResource(IQueryable query, string fullTypeName)
        {
            return GetCurrentProvider<IUpdatable>().GetResource(query, fullTypeName);
        }

        /// <summary>
        /// Resets the value of the given resource to its default value
        /// </summary>
        /// <param name="resource">resource whose value needs to be reset</param>
        /// <returns>same resource with its value reset</returns>
        public object ResetResource(object resource)
        {
            return GetCurrentProvider<IUpdatable>().ResetResource(resource);
        }

        /// <summary>
        /// Sets the value of the given property on the target object
        /// </summary>
        /// <param name="targetResource">target object which defines the property</param>
        /// <param name="propertyName">name of the property whose value needs to be updated</param>
        /// <param name="propertyValue">value of the property</param>
        public void SetValue(object targetResource, string propertyName, object propertyValue)
        {
            GetCurrentProvider<IUpdatable>().SetValue(targetResource, propertyName, propertyValue);
        }

        /// <summary>
        /// Gets the value of the given property on the target object
        /// </summary>
        /// <param name="targetResource">target object which defines the property</param>
        /// <param name="propertyName">name of the property whose value needs to be updated</param>
        /// <returns>the value of the property for the given target resource</returns>
        public object GetValue(object targetResource, string propertyName)
        {
            return GetCurrentProvider<IUpdatable>().GetValue(targetResource, propertyName);
        }

        /// <summary>
        /// Sets the value of the given reference property on the target object
        /// </summary>
        /// <param name="targetResource">target object which defines the property</param>
        /// <param name="propertyName">name of the property whose value needs to be updated</param>
        /// <param name="propertyValue">value of the property</param>
        public void SetReference(object targetResource, string propertyName, object propertyValue)
        {
            GetCurrentProvider<IUpdatable>().SetReference(targetResource, propertyName, propertyValue);
        }

        /// <summary>
        /// Adds the given value to the collection
        /// </summary>
        /// <param name="targetResource">target object which defines the property</param>
        /// <param name="propertyName">name of the property whose value needs to be updated</param>
        /// <param name="resourceToBeAdded">value of the property which needs to be added</param>
        public void AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
        {
            GetCurrentProvider<IUpdatable>().AddReferenceToCollection(targetResource, propertyName, resourceToBeAdded);
        }

        /// <summary>
        /// Removes the given value from the collection
        /// </summary>
        /// <param name="targetResource">target object which defines the property</param>
        /// <param name="propertyName">name of the property whose value needs to be updated</param>
        /// <param name="resourceToBeRemoved">value of the property which needs to be removed</param>
        public void RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
        {
            GetCurrentProvider<IUpdatable>().RemoveReferenceFromCollection(targetResource, propertyName, resourceToBeRemoved);
        }

        /// <summary>
        /// Delete the given resource
        /// </summary>
        /// <param name="targetResource">resource that needs to be deleted</param>
        public void DeleteResource(object targetResource)
        {
            GetCurrentProvider<IUpdatable>().DeleteResource(targetResource);
        }

        /// <summary>
        /// Saves all the pending changes made till now
        /// </summary>
        public void SaveChanges()
        {
            GetCurrentProvider<IUpdatable>().SaveChanges();
        }

        /// <summary>
        /// Returns the actual instance of the resource represented by the given resource object
        /// </summary>
        /// <param name="resource">object representing the resource whose instance needs to be fetched</param>
        /// <returns>The actual instance of the resource represented by the given resource object</returns>
        public object ResolveResource(object resource)
        {
            return GetCurrentProvider<IUpdatable>().ResolveResource(resource);
        }

        /// <summary>
        /// Revert all the pending changes.
        /// </summary>
        public void ClearChanges()
        {
            GetCurrentProvider<IUpdatable>().ClearChanges();
        }
        #endregion

        #region IDataServiceUpdateProvider

        /// <summary>
        /// Passes the etag value for the given resource.
        /// </summary>
        /// <param name="resourceCookie">cookie representing the resource.</param>
        /// <param name="checkForEquality">true if we need to compare the property values for equality. If false, then we need to compare values for non-equality.</param>
        /// <param name="concurrencyValues">list of the etag property names and its corresponding values.</param>
        public void SetConcurrencyValues(object resourceCookie, bool? checkForEquality,
                                         IEnumerable<KeyValuePair<string, object>> concurrencyValues)
        {
            GetCurrentProvider<IDataServiceUpdateProvider>().SetConcurrencyValues(resourceCookie, checkForEquality, concurrencyValues);
        }

        #endregion IDataServiceUpdateProvider

        #region IDataServiceUpdateProvider2

        /// <summary>
        /// Queues up the <paramref name="invokable"/> to be invoked during IUpdatable.SaveChanges().
        /// </summary>
        /// <param name="invokable">The invokable instance whose Invoke() method will be called during IUpdatable.SaveChanges().</param>
        public void ScheduleInvokable(IDataServiceInvokable invokable)
        {
            GetCurrentProvider<IDataServiceUpdateProvider2>().ScheduleInvokable(invokable);
        }

        #endregion IDataServiceUpdateProvider2

        #region IDataServiceProviderBehavior

        /// <summary>
        /// The kind of behavior service should assume from the provider.
        /// </summary>
        public ProviderBehavior ProviderBehavior
        {
            get
            {
                Log.Trace();
                return GetCurrentProvider<IDataServiceProviderBehavior>().ProviderBehavior;
            }
        }

        #endregion IDataServiceProviderBehavior

        #region IDataServiceEntityFrameworkProvider

        public MetadataEdmSchemaVersion EdmSchemaVersion {
            get
            {
                Log.Trace();
                var provider = GetCurrentProvider<IDataServiceEntityFrameworkProvider>();

                // if provider is not implement the IDSEtagProvider, return the default properties
                if (provider == null)
                {
                    return efProvider.EdmSchemaVersion;
                }
                return provider.EdmSchemaVersion;
            }
        }

        /// <summary>
        /// Given a resource container and resource type, gets the list of ResourceProperties that
        /// are part of the ETag.
        /// </summary>
        /// <param name="containerName">Resource set name.</param>
        /// <param name="resourceType">Resource type of entities in the resource container.</param>
        /// <returns>Collection of properties that are part of the ETag.</returns>
        public IList<ResourceProperty> GetETagProperties(string containerName, ResourceType resourceType)
        {
            Log.Trace(containerName, resourceType.Name);
            var provider = GetCurrentProvider<IDataServiceEntityFrameworkProvider>();

            // if provider is not implement the IDSEtagProvider, return the default properties
            if (provider == null)
            {
                return resourceType.ETagProperties;
            }
            return provider.GetETagProperties(containerName, resourceType);
        }

        #endregion IDataServiceEntityFrameworkProvider

        #region IDisposable

        /// <summary>
        /// Dispose the object
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the object
        /// </summary>
        /// <param name="disposing">If it is already under disposing</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    if (reflectionProvider != null)
                    {
                        reflectionProvider.Dispose();
                    }
                }
                finally
                {
                    reflectionProvider = null;
                }
                try
                {
                    if (efProvider != null)
                    {
                        efProvider.Dispose();
                    }
                }
                finally
                {
                    efProvider = null;
                }
            }
        }

        #endregion

        /// <summary>
        /// Get the current provider based on the entity set
        /// </summary>
        /// <typeparam name="T">The type of the provider</typeparam>
        /// <returns>The provider</returns>
        private T GetCurrentProvider<T>()
            where T : class
        {
            string entitySet = hybridService.EntitySet;
            if (string.IsNullOrEmpty(entitySet))
            {
                //if entitySet is unknown yet, return efProvider as the default provider.
                return efProvider as T;
            }
            ResourceSet resource;
            if (efProvider.TryResolveResourceSet(entitySet, out resource))
            {
                return efProvider as T;
            }
            if (reflectionProvider.TryResolveResourceSet(entitySet, out resource))
            {
                // If it is reflection provider, the IDSUpdateProvider2 is implemented on the data source
                return reflectionProvider as T ?? reflectionProvider.CurrentDataSource as T;
            }
            // Check if it is service operation
            ServiceOperation svcop;
            if (TryResolveServiceOperation(entitySet, out svcop))
            {
                var  method = hybridService.GetType().GetMethod(entitySet);
                if (method != null)
                {
                    var providerType = Attribute.GetCustomAttribute(method, typeof (ProviderTypeAttribute), true) as ProviderTypeAttribute;
                    if (providerType != null)
                    {
                        if (providerType.Type == "EF")
                        {
                            return efProvider as T; 
                        }
                        if(providerType.Type == "Reflection")
                        {
                            return reflectionProvider as T; 
                        }
                    }
                }
            }

            //it is not an entity, return efProvider as the default provider.
            return efProvider as T;
        }
    }

    public class Globalization {}
}

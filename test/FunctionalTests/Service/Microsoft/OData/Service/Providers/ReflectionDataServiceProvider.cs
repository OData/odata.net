//---------------------------------------------------------------------
// <copyright file="ReflectionDataServiceProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Diagnostics;
    #endregion

    /// <summary>
    /// Reflection based provider.
    /// </summary>
    public class ReflectionDataServiceProvider : 
        IDataServiceMetadataProvider, 
        IDataServiceQueryProvider,
        IDataServiceProviderBehavior,
        IDataServiceInternalProvider,
        IServiceProvider, 
        IDisposable
    {
        /// <summary>
        /// ReflectionServiceProvider which provides implementation of metadata and query interfaces.
        /// </summary>
        private ReflectionServiceProvider innerProvider;

        /// <summary>
        /// Initializes a new Microsoft.OData.Service.Providers.ReflectionDataServiceProvider instance.
        /// </summary>
        /// <param name="args"><see cref="DataServiceProviderArgs"/> needed during provider construction.</param>
        public ReflectionDataServiceProvider(DataServiceProviderArgs args)
        {
            WebUtil.CheckArgumentNull(args, "args");

            this.CreateInnerProvider(args.DataServiceInstance, args.DataSourceInstance);

            this.LoadMetadata(args.SkipServiceOperationMetadata);

            // Load known types and set ordering property and make readonly
            this.innerProvider.FinalizeMetadataModel(args.KnownTypes, args.UseMetadataKeyOrder);
        }

        /// <summary>
        /// Initializes a new Microsoft.OData.Service.Providers.ReflectionDataServiceProvider instance.
        /// </summary>
        /// <param name="dataServiceInstance">Required data service instance.</param>
        /// <param name="dataSourceInstance">Required data source instance.</param>
        internal ReflectionDataServiceProvider(object dataServiceInstance, object dataSourceInstance)
        {
            this.CreateInnerProvider(dataServiceInstance, dataSourceInstance);

            this.LoadMetadata(false/*skipServiceOperations*/);
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
                typeof(IDataServiceProviderBehavior) == serviceType)
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
            this.innerProvider = new ReflectionServiceProvider(dataServiceInstance, dataSourceInstance);
        }

        /// <summary>
        /// Initializes the provider by loading metdata information.
        /// </summary>
        /// <param name="skipServiceOperations">Sould service operations be loaded.</param>
        private void LoadMetadata(bool skipServiceOperations)
        {
            Debug.Assert(this.innerProvider != null, "Inner Provider must have been initialized.");
            this.innerProvider.LoadMetadata(skipServiceOperations);
        }
    }
}

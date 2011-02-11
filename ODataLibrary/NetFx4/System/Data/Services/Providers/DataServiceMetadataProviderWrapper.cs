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
    using System.Collections.Generic;
    using System.Data.OData;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    #endregion Namespaces.

    /// <summary>
    /// Wrapper around the IDataServiceMetadataProvider interface which validates the interface implementation
    /// and provides some additional functionality on top of it.
    /// </summary>
    internal sealed class DataServiceMetadataProviderWrapper
    {
        /// <summary>
        /// metadata provider instance.
        /// </summary>
        private IDataServiceMetadataProvider metadataProvider;

        /// <summary>
        /// Cache of resource types - a map from the full name of the resource type to the instance.
        /// </summary>
        private Dictionary<string, ResourceType> resourceTypeCache;

        /// <summary>
        /// Cache of resource sets - a map from the full name of the resource set to the instance.
        /// </summary>
        private Dictionary<string, ResourceSetWrapper> resourceSetCache;

        /// <summary>
        /// Cache of service operations - a map from the name of the service operation to the instance.
        /// </summary>
        private Dictionary<string, ServiceOperationWrapper> serviceOperationCache;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="metadataProvider">The IDataServiceMetadataProvider instance to wrap.</param>
        internal DataServiceMetadataProviderWrapper(IDataServiceMetadataProvider metadataProvider)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(metadataProvider != null, "metadataProvider != null");

            this.metadataProvider = metadataProvider;
            this.resourceTypeCache = new Dictionary<string, ResourceType>(EqualityComparer<string>.Default);
            this.resourceSetCache = new Dictionary<string, ResourceSetWrapper>(EqualityComparer<string>.Default);
            this.serviceOperationCache = new Dictionary<string, ServiceOperationWrapper>(EqualityComparer<string>.Default);
        }

        /// <summary>
        /// The enumerable of all resource sets in the metadata.
        /// </summary>
        [SuppressMessage("DataWeb.Usage", "AC0015:CheckNoExternalCallersRule", Justification = "Calling DebugUtils.CheckNoExternalCallers; false positive due to 'yield return'.")]
        internal IEnumerable<ResourceSetWrapper> ResourceSets
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();

                IEnumerable<ResourceSet> resourceSets = this.metadataProvider.ResourceSets;
                if (resourceSets != null)
                {
                    HashSet<string> resourceSetNames = new HashSet<string>(EqualityComparer<string>.Default);

                    foreach (ResourceSet resourceSet in resourceSets)
                    {
                        // verify that the name of the resource set is unique
                        AddUniqueNameToSet(
                            resourceSet != null ? resourceSet.Name : null,
                            resourceSetNames,
                            Strings.DataServiceMetadataProviderWrapper_MultipleEntitySetsWithSameName(resourceSet.Name));

                        // For IDSP, we want to make sure the metadata object instance stay the same within
                        // a request because we do reference comparisons.  Note the provider can return 
                        // different metadata instances within the same request.  The the Validate*() methods
                        // will make sure to return the first cached instance.
                        ResourceSetWrapper resourceSetWrapper = this.ValidateResourceSet(resourceSet);
                        if (resourceSetWrapper != null)
                        {
                            yield return resourceSetWrapper;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The enumerable of all resource sets in the metadata.
        /// </summary>
        [SuppressMessage("DataWeb.Usage", "AC0015:CheckNoExternalCallersRule", Justification = "Calling DebugUtils.CheckNoExternalCallers; false positive due to 'yield return'.")]
        internal IEnumerable<ServiceOperationWrapper> ServiceOperations
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();

                IEnumerable<ServiceOperation> serviceOperations = this.metadataProvider.ServiceOperations;
                if (serviceOperations != null)
                {
                    HashSet<string> serviceOperationNames = new HashSet<string>(EqualityComparer<string>.Default);

                    foreach (ServiceOperation serviceOperation in serviceOperations)
                    {
                        // verify that the name of the service operation is unique
                        AddUniqueNameToSet(
                            serviceOperation != null ? serviceOperation.Name : null,
                            serviceOperationNames,
                            Strings.DataServiceMetadataProviderWrapper_MultipleServiceOperationsWithSameName(serviceOperation.Name));

                        // For IDSP, we want to make sure the metadata object instance stay the same within
                        // a request because we do reference comparisons.  Note the provider can return 
                        // different metadata instances within the same request.  The the Validate*() methods
                        // will make sure to return the first cached instance.
                        ServiceOperationWrapper serviceOperationWrapper = this.ValidateServiceOperation(serviceOperation);
                        if (serviceOperationWrapper != null)
                        {
                            yield return serviceOperationWrapper;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The enumerable of all resource sets in the metadata.
        /// </summary>
        [SuppressMessage("DataWeb.Usage", "AC0015:CheckNoExternalCallersRule", Justification = "Calling DebugUtils.CheckNoExternalCallers; false positive due to 'yield return'.")]
        internal IEnumerable<ResourceType> ResourceTypes
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();

                IEnumerable<ResourceType> resourceTypes = this.metadataProvider.Types;
                if (resourceTypes != null)
                {
                    HashSet<string> resourceTypeNames = new HashSet<string>(EqualityComparer<string>.Default);

                    foreach (ResourceType resourceType in resourceTypes)
                    {
                        // verify that the name of the resource type is unique
                        AddUniqueNameToSet(
                            resourceType != null ? resourceType.Name : null,
                            resourceTypeNames,
                            Strings.DataServiceMetadataProviderWrapper_MultipleResourceTypesWithSameName(resourceType.Name));

                        // For IDSP, we want to make sure the metadata object instance stay the same within
                        // a request because we do reference comparisons.  Note the provider can return 
                        // different metadata instances within the same request.  The the Validate*() methods
                        // will make sure to return the first cached instance.
                        ResourceType validatedResourceType = this.ValidateResourceType(resourceType);
                        if (validatedResourceType != null)
                        {
                            yield return validatedResourceType;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Namespace name for the EDM container.
        /// </summary>
        internal string ContainerNamespace
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.metadataProvider.ContainerNamespace;
            }
        }

        /// <summary>
        /// Name of the EDM container.
        /// </summary>
        internal string ContainerName
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.metadataProvider.ContainerName;
            }
        }

        /// <summary>
        /// Get all derived types of the specified <paramref name="resourceType"/>.
        /// </summary>
        /// <param name="resourceType">The resource type to get the derived types for.</param>
        /// <returns>The derived types of the <paramref name="resourceType"/>.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0015:CheckNoExternalCallersRule", Justification = "Calling DebugUtils.CheckNoExternalCallers; false positive due to 'yield return'.")]
        internal IEnumerable<ResourceType> GetDerivedTypes(ResourceType resourceType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(resourceType != null, "resourceType != null");

            foreach (ResourceType derivedType in this.metadataProvider.GetDerivedTypes(resourceType))
            {
                yield return this.ValidateResourceType(derivedType);
            }
        }

        /// <summary>
        /// Given the specified name, tries to find a type.
        /// </summary>
        /// <param name="name">Name of the type to resolve.</param>
        /// <returns>Resolved resource type, possibly null.</returns>
        /// <remarks>The implementation caches the reply from the underlying IDSMP and thus always returns the same instance for the same input
        /// so that caller can rely on reference comparison if necessary.</remarks>
        internal ResourceType TryResolveResourceType(string name)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(name != null, "name != null");

            ResourceType resourceType;
            if (this.resourceTypeCache.TryGetValue(name, out resourceType))
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
        /// Given the specified name, tries to find a resource set.
        /// </summary>
        /// <param name="name">Name of the resource set to resolve.</param>
        /// <returns>Resolved resource set, possibly null.</returns>
        internal ResourceSetWrapper TryResolveResourceSet(string name)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(name != null, "name != null");

            // For IDSP, we want to make sure the metadata object instance stay the same within
            // a request because we do reference comparisons.
            ResourceSetWrapper resourceSetWrapper;
            if (this.resourceSetCache.TryGetValue(name, out resourceSetWrapper))
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
        /// Given the specified name, tries to find a service operation.
        /// </summary>
        /// <param name="name">Name of the service operation to resolve.</param>
        /// <returns>Resolved service operation, possibly null.</returns>
        internal ServiceOperationWrapper TryResolveServiceOperation(string name)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(name != null, "name != null");

            // For IDSP, we want to make sure the metadata object instance stay the same within
            // a request because we do reference comparisons.
            ServiceOperationWrapper serviceOperationWrapper;
            if (this.serviceOperationCache.TryGetValue(name, out serviceOperationWrapper))
            {
                return serviceOperationWrapper;
            }

            ServiceOperation serviceOperation;
            if (this.metadataProvider.TryResolveServiceOperation(name, out serviceOperation))
            {
                return this.ValidateServiceOperation(serviceOperation);
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
        internal ResourceAssociationSet GetResourceAssociationSet(ResourceSetWrapper resourceSet, ResourceType resourceType, ResourceProperty resourceProperty)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(resourceSet != null, "resourceSet != null");
            Debug.Assert(resourceType != null, "resourceType != null");
            Debug.Assert(resourceProperty != null, "resourceProperty != null");

            return this.metadataProvider.GetResourceAssociationSet(resourceSet.ResourceSet, resourceType, resourceProperty);
        }

        /// <summary>
        /// Validates that <paramref name="resourceType"/> is cached and read only.
        /// </summary>
        /// <param name="resourceType">Resource type to be validated.</param>
        /// <returns>Validated resource type, null if the resource type is not supposed to be visible.</returns>
        internal ResourceType ValidateResourceType(ResourceType resourceType)
        {
            DebugUtils.CheckNoExternalCallers();

            if (resourceType != null && resourceType.ResourceTypeKind != ResourceTypeKind.Primitive)
            {
                // For IDSP, we want to make sure the metadata object instance stays the same within
                // a request because we do reference comparisons.  Note the provider can return 
                // different metadata instances within the same request. The Validate*() methods
                // will make sure to return the first cached instance.
                ResourceType cachedResourceType;
                if (this.resourceTypeCache.TryGetValue(resourceType.FullName, out cachedResourceType))
                {
                    return cachedResourceType;
                }

                ValidateResourceTypeReadOnly(resourceType);
                this.resourceTypeCache[resourceType.FullName] = resourceType;
            }

            return resourceType;
        }

        /// <summary>
        /// Validates that <paramref name="resourceSet"/> is cached and read only.
        /// </summary>
        /// <param name="resourceSet">Resource set to be validated.</param>
        /// <returns>Validated resource set, null if the resource set is not supposed to be visible.</returns>
        internal ResourceSetWrapper ValidateResourceSet(ResourceSet resourceSet)
        {
            DebugUtils.CheckNoExternalCallers();

            ResourceSetWrapper resourceSetWrapper = null;
            if (resourceSet != null)
            {
                // For IDSP, we want to make sure the metadata object instance stays the same within
                // a request because we do reference comparisons.  Note the provider can return 
                // different metadata instances within the same request. The Validate*() methods
                // will make sure to return the first cached instance.
                if (!this.resourceSetCache.TryGetValue(resourceSet.Name, out resourceSetWrapper))
                {
                    ValidateResourceSetReadOnly(resourceSet);
                    resourceSetWrapper = ResourceSetWrapper.CreateResourceSetWrapper(resourceSet, this.ValidateResourceType);
                    this.resourceSetCache[resourceSet.Name] = resourceSetWrapper;
                }
            }

            return resourceSetWrapper;
        }

        /// <summary>
        /// Validates if the service operation should be visible and is read only. If the service operation
        /// rights are set to None the service operation should not be visible.
        /// </summary>
        /// <param name="serviceOperation">Service operation to be validated.</param>
        /// <returns>Validated service operation, null if the service operation is not supposed to be visible.</returns>
        internal ServiceOperationWrapper ValidateServiceOperation(ServiceOperation serviceOperation)
        {
            DebugUtils.CheckNoExternalCallers();

            ServiceOperationWrapper serviceOperationWrapper = null;
            if (serviceOperation != null)
            {
                // For IDSP, we want to make sure the metadata object instance stay the same within
                // a request because we do reference comparisons.  Note the provider can return 
                // different metadata instances within the same request.  The the Validate*() methods
                // will make sure to return the first cached instance.
                if (!this.serviceOperationCache.TryGetValue(serviceOperation.Name, out serviceOperationWrapper))
                {
                    ValidateServiceOperationReadOnly(serviceOperation);
                    serviceOperationWrapper = ServiceOperationWrapper.CreateServiceOperationWrapper(serviceOperation, this.ValidateResourceSet, this.ValidateResourceType);
                    this.serviceOperationCache[serviceOperation.Name] = serviceOperationWrapper;
                }
            }

            return serviceOperationWrapper;
        }

        /// <summary>
        /// Throws if <paramref name="resourceType"/> is not sealed.
        /// </summary>
        /// <param name="resourceType">Resource type to inspect.</param>
        private static void ValidateResourceTypeReadOnly(ResourceType resourceType)
        {
            Debug.Assert(resourceType != null, "resourceType != null");
            if (!resourceType.IsReadOnly)
            {
                throw new ODataException(Strings.DataServiceMetadataProviderWrapper_ResourceTypeNotReadonly(resourceType.FullName));
            }
        }

        /// <summary>
        /// Throws if <paramref name="resourceSet"/> is not sealed.
        /// </summary>
        /// <param name="resourceSet">Resource set to inspect.</param>
        private static void ValidateResourceSetReadOnly(ResourceSet resourceSet)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");
            if (!resourceSet.IsReadOnly)
            {
                throw new ODataException(Strings.DataServiceMetadataProviderWrapper_ResourceSetNotReadonly(resourceSet.Name));
            }
        }

        /// <summary>
        /// Throws if <paramref name="serviceOperation"/> is not sealed.
        /// </summary>
        /// <param name="serviceOperation">Service operation to inspect.</param>
        private static void ValidateServiceOperationReadOnly(ServiceOperation serviceOperation)
        {
            Debug.Assert(serviceOperation != null, "serviceOperation != null");
            if (!serviceOperation.IsReadOnly)
            {
                throw new ODataException(Strings.DataServiceMetadataProviderWrapper_ServiceOperationNotReadonly(serviceOperation.Name));
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
                    throw new ODataException(exceptionString);
                }

                names.Add(name);
            }
        }
    }
}

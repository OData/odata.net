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
    #endregion Namespaces.

    /// <summary>
    /// Provides a metadata and query source abstraction for a 
    /// web data service's store.
    /// </summary>
#if INTERNAL_DROP
    internal interface IDataServiceMetadataProvider
#else
    public interface IDataServiceMetadataProvider
#endif
    {
        /// <summary>
        /// Namespace name for the EDM container.
        /// </summary>
        string ContainerNamespace
        {
            get;
        }

        /// <summary>
        /// Name of the EDM container.
        /// </summary>
        string ContainerName
        {
            get;
        }

        /// <summary>
        /// Gets all available containers.
        /// </summary>
        IEnumerable<ResourceSet> ResourceSets
        {
            get;
        }

        /// <summary>
        /// Returns all the types in this data source.
        /// </summary>
        IEnumerable<ResourceType> Types
        {
            get;
        }

        /// <summary>
        /// Returns all the service operations in this data source.
        /// </summary>
        IEnumerable<ServiceOperation> ServiceOperations
        {
            get;
        }

        /// <summary>
        /// Given the specified name, tries to find a resource set.
        /// </summary>
        /// <param name="name">Name of the resource set to resolve.</param>
        /// <param name="resourceSet">Returns the resolved resource set, null if no resource set for the given name was found.</param>
        /// <returns>True if resource set with the given name was found, false otherwise.</returns>
        bool TryResolveResourceSet(string name, out ResourceSet resourceSet);

        /// <summary>
        /// Gets the ResourceAssociationSet instance when given the source association end.
        /// </summary>
        /// <param name="resourceSet">Resource set of the source association end.</param>
        /// <param name="resourceType">Resource type of the source association end.</param>
        /// <param name="resourceProperty">Resource property of the source association end.</param>
        /// <returns>ResourceAssociationSet instance.</returns>
        ResourceAssociationSet GetResourceAssociationSet(ResourceSet resourceSet, ResourceType resourceType, ResourceProperty resourceProperty);

        /// <summary>
        /// Given the specified name, tries to find a type.
        /// </summary>
        /// <param name="name">Name of the type to resolve.</param>
        /// <param name="resourceType">Returns the resolved resource type, null if no resource type for the given name was found.</param>
        /// <returns>True if we found the resource type for the given name, false otherwise.</returns>
        bool TryResolveResourceType(string name, out ResourceType resourceType);

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
        IEnumerable<ResourceType> GetDerivedTypes(ResourceType resourceType);

        /// <summary>
        /// Returns true if <paramref name="resourceType"/> represents an Entity Type which has derived Entity Types, else false.
        /// </summary>
        /// <param name="resourceType">instance of the resource type in question.</param>
        /// <returns>True if <paramref name="resourceType"/> represents an Entity Type which has derived Entity Types, else false.</returns>
        bool HasDerivedTypes(ResourceType resourceType);

        /// <summary>
        /// Given the specified name, tries to find a service operation.
        /// </summary>
        /// <param name="name">Name of the service operation to resolve.</param>
        /// <param name="serviceOperation">Returns the resolved service operation, null if no service operation was found for the given name.</param>
        /// <returns>True if we found the service operation for the given name, false otherwise.</returns>
        bool TryResolveServiceOperation(string name, out ServiceOperation serviceOperation);
    }
}

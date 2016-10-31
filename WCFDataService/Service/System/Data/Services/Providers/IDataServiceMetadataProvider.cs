//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace System.Data.Services.Providers
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides a metadata and query source abstraction for a 
    /// web data service's store.
    /// </summary>
    public interface IDataServiceMetadataProvider
    {
        /// <summary>Namespace name for the data source.</summary>
        /// <returns>String that contains the namespace name.</returns>
        string ContainerNamespace
        {
            get;
        }

        /// <summary>Container name for the data source.</summary>
        /// <returns>String that contains the name of the container.</returns>
        string ContainerName
        {
            get;
        }

        /// <summary>Gets all available containers.</summary>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> collection of <see cref="T:System.Data.Services.Providers.ResourceSet" /> objects.</returns>
        IEnumerable<ResourceSet> ResourceSets
        {
            get;
        }

        /// <summary>Returns all the types in this data source.</summary>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> collection of <see cref="T:System.Data.Services.Providers.ResourceType" /> objects.</returns>
        IEnumerable<ResourceType> Types
        {
            get;
        }

        /// <summary>Returns all the service operations in this data source.</summary>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> collection of <see cref="T:System.Data.Services.Providers.ServiceOperation" /> objects.</returns>
        IEnumerable<ServiceOperation> ServiceOperations
        {
            get;
        }

        /// <summary>Tries to get a resource set based on the specified name.</summary>
        /// <returns>true when resource set with the given <paramref name="name" /> is found; otherwise false.</returns>
        /// <param name="name">Name of the <see cref="T:System.Data.Services.Providers.ResourceSet" /> to resolve.</param>
        /// <param name="resourceSet">Returns the resource set or a null value if a resource set with the given <paramref name="name" /> is not found.</param>
        bool TryResolveResourceSet(string name, out ResourceSet resourceSet);

        /// <summary>Gets the <see cref="T:System.Data.Services.Providers.ResourceAssociationSet" /> instance when given the source association end.</summary>
        /// <returns>A <see cref="T:System.Data.Services.Providers.ResourceAssociationSet" /> instance.</returns>
        /// <param name="resourceSet">Resource set of the source association end.</param>
        /// <param name="resourceType">Resource type of the source association end.</param>
        /// <param name="resourceProperty">Resource property of the source association end.</param>
        ResourceAssociationSet GetResourceAssociationSet(ResourceSet resourceSet, ResourceType resourceType, ResourceProperty resourceProperty);

        /// <summary>Tries to get a resource type based on the specified name.</summary>
        /// <returns>true when resource type with the given <paramref name="name" /> is found; otherwise false.</returns>
        /// <param name="name">Name of the type to resolve.</param>
        /// <param name="resourceType">Returns the resource type or a null value if a resource type with the given <paramref name="name" /> is not found.</param>
        bool TryResolveResourceType(string name, out ResourceType resourceType);

        /// <summary>Attempts to return all types that derive from the specified resource type.</summary>
        /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> collection of derived <see cref="T:System.Data.Services.Providers.ResourceType" /> objects.</returns>
        /// <param name="resourceType">The base <see cref="T:System.Data.Services.Providers.ResourceType" />.</param>
        /// <remarks>The method must return a collection of all the types derived from <paramref name="resourceType"/>.
        /// The collection returned should NOT include the type passed in as a parameter.
        /// An implementer of the interface should return null if the type does not have any derived types (ie. null == no derived types).
        /// </remarks>
        IEnumerable<ResourceType> GetDerivedTypes(ResourceType resourceType);

        /// <summary>Determines whether a resource type has derived types.</summary>
        /// <returns>true when <paramref name="resourceType" /> represents an entity that has derived types; otherwise false.</returns>
        /// <param name="resourceType">A <see cref="T:System.Data.Services.Providers.ResourceType" /> object to evaluate.</param>
        bool HasDerivedTypes(ResourceType resourceType);

        /// <summary>Tries to get a service operation based on the specified name.</summary>
        /// <returns>true when service operation with the given <paramref name="name" /> is found; otherwise false.</returns>
        /// <param name="name">Name of the service operation to resolve.</param>
        /// <param name="serviceOperation">Returns the service operation or a null value if a service operation with the given <paramref name="name" /> is not found.</param>
        bool TryResolveServiceOperation(string name, out ServiceOperation serviceOperation);
    }
}

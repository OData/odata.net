//---------------------------------------------------------------------
// <copyright file="MetadataProviderEdmCollectionType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    using System.Diagnostics;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Implementation of <see cref="IEdmCollectionType"/> based on a <see cref="ResourceType"/>.
    /// </summary>
    internal class MetadataProviderEdmCollectionType : EdmCollectionType, IResourceTypeBasedEdmType
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MetadataProviderEdmCollectionType"/>.
        /// </summary>
        /// <param name="collectionResourceType">The collection resource type this edm collection type is being created for.</param>
        /// <param name="elementType">The element type of the collection.</param>
        public MetadataProviderEdmCollectionType(ResourceType collectionResourceType, IEdmTypeReference elementType)
            : base(elementType)
        {
            Debug.Assert(collectionResourceType != null, "collectionResourceType != null");
            Debug.Assert(collectionResourceType is CollectionResourceType || collectionResourceType is EntityCollectionResourceType, "resource type must represent a collection.");

            this.ResourceType = collectionResourceType;
        }

        /// <summary>
        /// The resource-type that this type was created from.
        /// </summary>
        public ResourceType ResourceType { get; private set; }
    }
}
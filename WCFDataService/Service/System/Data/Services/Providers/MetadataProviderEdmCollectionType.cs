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
    using System.Diagnostics;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;

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

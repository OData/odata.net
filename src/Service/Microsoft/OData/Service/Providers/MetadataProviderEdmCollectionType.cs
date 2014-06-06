//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service.Providers
{
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;

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

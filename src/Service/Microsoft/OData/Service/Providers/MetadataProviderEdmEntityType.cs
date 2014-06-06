//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Annotations;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Edm.Library.Annotations;

    #endregion Namespaces

    /// <summary>
    /// An <see cref="IEdmEntityType"/> implementation that supports delay-loading of properties and remembers the <see cref="ResourceType"/> it was based on.
    /// </summary>
    internal sealed class MetadataProviderEdmEntityType : EdmEntityTypeWithDelayLoadedProperties, IEdmEntityType, IResourceTypeBasedEdmType
    {
        /// <summary>
        /// Initializes a new instance of the MetadataProviderEdmEntityType class.
        /// </summary>
        /// <param name="namespaceName">Namespace the entity belongs to.</param>
        /// <param name="resourceType">The resource type this edm type is being created from.</param>
        /// <param name="baseType">The base type of this entity type.</param>
        /// <param name="isAbstract">Denotes an entity that cannot be instantiated.</param>
        /// <param name="isOpen">Denotes if the type is open.</param>
        /// <param name="hasStream">Denotes if the type is a media entity.</param>
        /// <param name="propertyLoadAction">An action that is used to create the properties for this type.</param>
        internal MetadataProviderEdmEntityType(
            string namespaceName, 
            ResourceType resourceType,
            IEdmEntityType baseType, 
            bool isAbstract, 
            bool isOpen, 
            bool hasStream,
            Action<EdmEntityTypeWithDelayLoadedProperties> propertyLoadAction)
            : base(namespaceName, resourceType.Name, baseType, isAbstract, isOpen, hasStream, propertyLoadAction)
        {
            Debug.Assert(resourceType != null, "resourceType != null");
            Debug.Assert(resourceType.ResourceTypeKind == ResourceTypeKind.EntityType, "Must be an entity type.");
            this.ResourceType = resourceType;
        }

        /// <summary>
        /// The resource-type that this type was created from.
        /// </summary>
        public ResourceType ResourceType { get; private set; }
    }
}

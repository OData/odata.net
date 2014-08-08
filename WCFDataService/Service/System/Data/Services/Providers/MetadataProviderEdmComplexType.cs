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
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Annotations;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.Edm.Library.Annotations;

    #endregion Namespaces

    /// <summary>
    /// An <see cref="IEdmComplexType"/> implementation that supports delay-loading of properties and remembers the <see cref="ResourceType"/> it was based on.
    /// </summary>
    internal sealed class MetadataProviderEdmComplexType : EdmComplexTypeWithDelayLoadedProperties, IEdmComplexType, IResourceTypeBasedEdmType
    {
        /// <summary>
        /// Initializes a new instance of the MetadataProviderEdmEntityType class.
        /// </summary>
        /// <param name="namespaceName">Namespace the entity belongs to.</param>
        /// <param name="resourceType">The resource type this edm type is based on.</param>
        /// <param name="baseType">The base type of this entity type.</param>
        /// <param name="isAbstract">Denotes an entity that cannot be instantiated.</param>
        /// <param name="propertyLoadAction">An action that is used to create the properties for this type.</param>
        internal MetadataProviderEdmComplexType(
            string namespaceName,
            ResourceType resourceType,
            IEdmComplexType baseType,
            bool isAbstract, 
            Action<EdmComplexTypeWithDelayLoadedProperties> propertyLoadAction)
            : base(namespaceName, resourceType.Name, baseType, isAbstract, propertyLoadAction)
        {
            Debug.Assert(resourceType != null, "resourceType != null");
            Debug.Assert(resourceType.ResourceTypeKind == ResourceTypeKind.ComplexType, "Must be a complex type.");
            this.ResourceType = resourceType;
        }

        /// <summary>
        /// The resource-type that this type was created from.
        /// </summary>
        public ResourceType ResourceType { get; private set; }
    }
}

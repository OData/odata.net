//---------------------------------------------------------------------
// <copyright file="MetadataProviderEdmComplexType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;

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
        /// /// <param name="isOpen">Denotes if the type is open.</param>
        /// <param name="propertyLoadAction">An action that is used to create the properties for this type.</param>
        internal MetadataProviderEdmComplexType(
            string namespaceName,
            ResourceType resourceType,
            IEdmComplexType baseType,
            bool isAbstract, 
            bool isOpen,
            Action<EdmComplexTypeWithDelayLoadedProperties> propertyLoadAction)
            : base(namespaceName, resourceType.Name, baseType, isAbstract, isOpen, propertyLoadAction)
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

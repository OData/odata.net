//---------------------------------------------------------------------
// <copyright file="EntityCollectionResourceType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces.

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.OData.Metadata;

    #endregion Namespaces.

    /// <summary>Use this class to represent a collection of entities.</summary>
    [DebuggerDisplay("{Name}: {InstanceType}, {ResourceTypeKind}")]
    public class EntityCollectionResourceType : ResourceType
    {
        /// <summary>Resource type of a single item in the collection.</summary>
        private readonly ResourceType itemType;

        /// <summary>Constructor.</summary>
        /// <param name="itemType">Resource type of a single item in the collection.</param>
        internal EntityCollectionResourceType(ResourceType itemType)
            : base(GetInstanceType(itemType), ResourceTypeKind.EntityCollection, string.Empty, GetName(itemType))
        {
            Debug.Assert(itemType != null, "itemType != null");

            if (itemType.ResourceTypeKind != ResourceTypeKind.EntityType)
            {
                throw new ArgumentException(Strings.ResourceType_CollectionItemCanBeOnlyEntity);
            }

            this.itemType = itemType;
        }

        /// <summary>Resource type of a single item in the collection.</summary>
        /// <returns>The resource type.</returns>
        public ResourceType ItemType
        {
            get
            {
                return this.itemType;
            }
        }

        /// <summary>Returns the instance type for a collection of specified <paramref name="itemType"/>.</summary>
        /// <param name="itemType">Resource type of a single item in the collection.</param>
        /// <returns>Instance type of the collection of <paramref name="itemType"/>.</returns>
        private static Type GetInstanceType(ResourceType itemType)
        {
            Debug.Assert(itemType != null, "itemType != null");
            return typeof(IEnumerable<>).MakeGenericType(itemType.InstanceType);
        }

        /// <summary>Returns EDM name of the type for a collection of specified <paramref name="itemType"/>.</summary>
        /// <param name="itemType">Resource type of a single item in the collection.</param>
        /// <returns>EDM name of the type of a collection of <paramref name="itemType"/>.</returns>
        private static string GetName(ResourceType itemType)
        {
            Debug.Assert(itemType != null, "itemType != null");
            return EdmLibraryExtensions.GetCollectionTypeName(itemType.FullName);
        }
    }
}

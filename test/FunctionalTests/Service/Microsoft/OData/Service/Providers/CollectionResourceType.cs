//---------------------------------------------------------------------
// <copyright file="CollectionResourceType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.OData.Metadata;

    #endregion Namespaces

    /// <summary>Use this class to represent a DataService type representing a collection property of primitive or complex types.</summary>
    [DebuggerDisplay("{Name}: {InstanceType}, {ResourceTypeKind}")]
    public class CollectionResourceType : ResourceType
    {
        /// <summary>Resource type of a single item in the collection property.</summary>
        private readonly ResourceType itemType;

        /// <summary>Constructor.</summary>
        /// <param name="itemType">Resource type of a single item in the collection property.</param>
        internal CollectionResourceType(ResourceType itemType)
            : base(GetInstanceType(itemType), ResourceTypeKind.Collection, string.Empty, GetName(itemType))
        {
            Debug.Assert(itemType != null, "itemType != null");

            if (itemType.ResourceTypeKind != ResourceTypeKind.Primitive && itemType.ResourceTypeKind != ResourceTypeKind.ComplexType)
            {
                throw new ArgumentException(Strings.ResourceType_CollectionItemCanBeOnlyPrimitiveOrComplex);
            }

            if (itemType == ResourceType.GetPrimitiveResourceType(typeof(System.IO.Stream)))
            {
                throw new ArgumentException(Strings.ResourceType_CollectionItemCannotBeStream(itemType.FullName), "itemType");
            }

            this.itemType = itemType;
        }

        /// <summary>Resource type of a single item in the collection.</summary>
        public ResourceType ItemType
        {
            get
            {
                return this.itemType;
            }
        }

        /// <summary>Returns instance type for a collection property of specified <paramref name="itemType"/>.</summary>
        /// <param name="itemType">Resource type of a single item in the collection property.</param>
        /// <returns>Instance type of the collection property of <paramref name="itemType"/>.</returns>
        private static Type GetInstanceType(ResourceType itemType)
        {
            Debug.Assert(itemType != null, "itemType != null");
            return typeof(IEnumerable<>).MakeGenericType(itemType.InstanceType);
        }

        /// <summary>Returns EDM name of the type for a collection property of specified <paramref name="itemType"/>.</summary>
        /// <param name="itemType">Resource type of a single item in the collection property.</param>
        /// <returns>EDM name of the type of the collection property of <paramref name="itemType"/>.</returns>
        private static string GetName(ResourceType itemType)
        {
            Debug.Assert(itemType != null, "itemType != null");
            return EdmLibraryExtensions.GetCollectionTypeName(itemType.FullName);
        }
    }
}

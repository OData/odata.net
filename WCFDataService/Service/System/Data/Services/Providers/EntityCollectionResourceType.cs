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
    #region Namespaces.

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.Data.OData.Metadata;

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

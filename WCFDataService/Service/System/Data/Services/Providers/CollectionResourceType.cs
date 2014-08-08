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

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.Data.OData.Metadata;

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

//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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

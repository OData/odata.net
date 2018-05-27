//---------------------------------------------------------------------
// <copyright file="EdmModelUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Utils.Metadata
{
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.Common;

    /// <summary>
    /// Helper methods for working with EDM types.
    /// </summary>
    public static class EdmModelUtils
    {
        /// <summary>
        /// Prefix of collection type names.
        /// </summary>
        public const string CollectionTypeNamePrefix = "Collection(";

        /// <summary>
        /// Suffix of collection type names.
        /// </summary>
        private const string CollectionTypeNameSuffix = ")";

        /// <summary>
        /// Returns the item type name of a collection item type name.
        /// </summary>
        /// <param name="collectionTypeName">The collection item type name to parse.</param>
        /// <returns>The item type name or null if the <paramref name="collectionTypeName"/> is not a collection type name.</returns>
        public static string GetCollectionItemTypeName(string collectionTypeName)
        {
            ExceptionUtilities.CheckArgumentNotNull(collectionTypeName, "collectionTypeName");

            if (collectionTypeName.StartsWith(CollectionTypeNamePrefix) && collectionTypeName.EndsWith(CollectionTypeNameSuffix))
            {
                return collectionTypeName.Substring(
                    CollectionTypeNamePrefix.Length, 
                    collectionTypeName.Length - CollectionTypeNamePrefix.Length - CollectionTypeNameSuffix.Length);
            }

            return null;
        }

        /// <summary>
        /// Constructs a collection type name from the type name of the item.
        /// </summary>
        /// <param name="itemTypeName">The type name of the item in collection.</param>
        /// <returns>The collection type name.</returns>
        public static string GetCollectionTypeName(string itemTypeName)
        {
            return CollectionTypeNamePrefix + itemTypeName + CollectionTypeNameSuffix;
        }

        /// <summary>
        /// Returns a full name of entity set.
        /// </summary>
        /// <param name="entitySet">The entity set to get the name for.</param>
        /// <returns>The container qualified full name of the entity set.</returns>
        public static string FullEdmName(this IEdmEntitySet entitySet)
        {
            IEdmEntityContainer container = entitySet.Container;
            if (container == null)
            {
                return entitySet.Name;
            }
            
            return container.Namespace + "." + container.Name + "." + entitySet.Name;
        }

        /// <summary>
        /// Asserts that a given entity type is derived from the specified base entity type.
        /// </summary>
        /// <param name="derivedType">The derived entity type.</param>
        /// <param name="baseType">The base entity type.</param>
        public static void AssertEntityTypeIsDerivedFrom(IEdmEntityType derivedType, IEdmEntityType baseType)
        {
            ExceptionUtilities.CheckArgumentNotNull(derivedType, "derivedType");
            ExceptionUtilities.CheckArgumentNotNull(baseType, "baseType");

            if (derivedType == baseType)
            {
                return;
            }

            var entityType = derivedType.BaseEntityType();
            while (entityType != null)
            {
                if (entityType == baseType)
                {
                    return;
                }

                entityType = entityType.BaseEntityType();
            }

            ExceptionUtilities.Assert(false, "Expected entity type " + derivedType.FullName() + " to be derived from " + baseType.FullName());
        }
    }
}

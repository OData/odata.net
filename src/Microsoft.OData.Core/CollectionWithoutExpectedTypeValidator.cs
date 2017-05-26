//---------------------------------------------------------------------
// <copyright file="CollectionWithoutExpectedTypeValidator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Helper class to verify that all items of a collection are of the same kind and type.
    /// </summary>
    /// <remarks>This class is only used if no expected item type is specified for the collection;
    /// otherwise all items are already validated against the expected item type.</remarks>
    internal sealed class CollectionWithoutExpectedTypeValidator
    {
        /// <summary>true if the item type was derived from the collection value; otherwise false.</summary>
        private readonly bool itemTypeDerivedFromCollectionValue;

        /// <summary>The item type name extracted from the first non-null item.</summary>
        private string itemTypeName;

        /// <summary>
        /// The primitive type denoted by the item type name or null if the type name is not a valid primitive type name.
        /// </summary>
        private IEdmPrimitiveType primitiveItemType;

        /// <summary>The item type kind from the first non-null item.</summary>
        private EdmTypeKind itemTypeKind;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="itemTypeNameFromCollection">The item type name extracted from the collection type name.</param>
        internal CollectionWithoutExpectedTypeValidator(string itemTypeNameFromCollection)
        {
            if (itemTypeNameFromCollection != null)
            {
                this.itemTypeName = GetItemTypeFullName(itemTypeNameFromCollection);
                this.itemTypeKind = ComputeExpectedTypeKind(this.itemTypeName, out this.primitiveItemType);
                this.itemTypeDerivedFromCollectionValue = true;
            }
        }

        /// <summary>
        /// If specified on a collection, returns the item type name that all items are expected to be compatible with; otherwise null.
        /// </summary>
        internal string ItemTypeNameFromCollection
        {
            get
            {
                return this.itemTypeDerivedFromCollectionValue ? this.itemTypeName : null;
            }
        }

        /// <summary>
        /// If specified on a collection, returns the item type kind that all items are expected to be compatible with; otherwise EdmTypeKind.None.
        /// </summary>
        internal EdmTypeKind ItemTypeKindFromCollection
        {
            get
            {
                return this.itemTypeDerivedFromCollectionValue ? this.itemTypeKind : EdmTypeKind.None;
            }
        }

        /// <summary>
        /// Validates a collection item that was read to make sure it is valid (i.e., has the correct
        /// type name and type kind) with respect to the other items in the collection.
        /// </summary>
        /// <param name="collectionItemTypeName">The type name of the item from the payload.</param>
        /// <param name="collectionItemTypeKind">The type kind of the item from the payload.</param>
        internal void ValidateCollectionItem(string collectionItemTypeName, EdmTypeKind collectionItemTypeKind)
        {
            // Only primitive and complex values are allowed in collections
            if (collectionItemTypeKind != EdmTypeKind.Primitive && collectionItemTypeKind != EdmTypeKind.Enum && collectionItemTypeKind != EdmTypeKind.Complex)
            {
                throw new ODataException(Strings.CollectionWithoutExpectedTypeValidator_InvalidItemTypeKind(collectionItemTypeKind));
            }

            if (this.itemTypeDerivedFromCollectionValue)
            {
                Debug.Assert(this.itemTypeName != null, "this.itemType != null");

                // If the collection has a type name assign missing item type names from it.
                collectionItemTypeName = collectionItemTypeName ?? this.itemTypeName;

                // If we have a type name from the collection, make sure the type names of all items match
                this.ValidateCollectionItemTypeNameAndKind(collectionItemTypeName, collectionItemTypeKind);
            }
            else
            {
                // If we don't have a type name from the collection, store the type name and type kind of the first non-null item.
                if (this.itemTypeKind == EdmTypeKind.None)
                {
                    // Compute the kind from the specified type name if available.
                    this.itemTypeKind = collectionItemTypeName == null
                        ? collectionItemTypeKind
                        : ComputeExpectedTypeKind(collectionItemTypeName, out this.primitiveItemType);

                    // If no payload type name is specified either default to Edm.String (for primitive type kinds) or leave the type name
                    // null (for complex items without type name)
                    if (collectionItemTypeName == null)
                    {
                        this.itemTypeKind = collectionItemTypeKind;
                        if (this.itemTypeKind == EdmTypeKind.Primitive)
                        {
                            this.itemTypeName = Metadata.EdmConstants.EdmStringTypeName;
                            this.primitiveItemType = EdmCoreModel.Instance.GetString(/*isNullable*/ false).PrimitiveDefinition();
                        }
                        else
                        {
                            this.itemTypeName = null;
                            this.primitiveItemType = null;
                        }
                    }
                    else
                    {
                        this.itemTypeKind = ComputeExpectedTypeKind(collectionItemTypeName, out this.primitiveItemType);
                        this.itemTypeName = collectionItemTypeName;
                    }
                }

                if (collectionItemTypeName == null && collectionItemTypeKind == EdmTypeKind.Primitive)
                {
                    // Default to Edm.String if no payload type is specified and the type kind is 'Primitive'
                    collectionItemTypeName = Metadata.EdmConstants.EdmStringTypeName;
                }

                // Validate the expected and actual type names and type kinds.
                // Note that we compute the expected type kind from the expected type name and thus the payload
                // type kind (passed to this method) might be different from the computed expected type kind.
                this.ValidateCollectionItemTypeNameAndKind(collectionItemTypeName, collectionItemTypeKind);
            }
        }

        /// <summary>
        /// Computes the expected type kind of an item from the type name read from the payload.
        /// </summary>
        /// <param name="typeName">The type name to compute the type kind from.</param>
        /// <param name="primitiveItemType">The primitive type for the specified type name or null if the type name is not a valid primitve type.</param>
        /// <returns>The <see cref="EdmTypeKind"/> of the type with the specified <paramref name="typeName"/>.</returns>
        private static EdmTypeKind ComputeExpectedTypeKind(string typeName, out IEdmPrimitiveType primitiveItemType)
        {
            IEdmSchemaType knownType = EdmCoreModel.Instance.FindDeclaredType(typeName);
            if (knownType != null)
            {
                Debug.Assert(knownType.TypeKind == EdmTypeKind.Primitive, "Only primitive types should be resolved by the core model.");
                primitiveItemType = (IEdmPrimitiveType)knownType;
                return EdmTypeKind.Primitive;
            }

            primitiveItemType = null;
            return EdmTypeKind.Complex;
        }

        /// <summary>
        /// Get the item type full name if it's the primitive type
        /// </summary>
        /// <param name="typeName">the original type name</param>
        /// <returns>the item type full name if it's the primitive type</returns>
        private static string GetItemTypeFullName(string typeName)
        {
            IEdmSchemaType knownType = EdmCoreModel.Instance.FindDeclaredType(typeName);
            if (knownType != null)
            {
                return knownType.FullName();
            }

            return typeName;
        }

        /// <summary>
        /// Validate that the expected and actual type names and type kinds are compatible.
        /// </summary>
        /// <param name="collectionItemTypeName">The actual type name.</param>
        /// <param name="collectionItemTypeKind">The actual type kind.</param>
        private void ValidateCollectionItemTypeNameAndKind(string collectionItemTypeName, EdmTypeKind collectionItemTypeKind)
        {
            // Compare the item type kinds.
            if (this.itemTypeKind != collectionItemTypeKind)
            {
                throw new ODataException(Strings.CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeKind(collectionItemTypeKind, this.itemTypeKind));
            }

            if (this.itemTypeKind == EdmTypeKind.Primitive)
            {
                Debug.Assert(this.primitiveItemType != null, "this.primitiveItemType != null");
                Debug.Assert(collectionItemTypeName != null, "collectionItemTypeName != null");

                // NOTE: we do support type inheritance for spatial primitive types; otherwise the type names have to match.
                if (!string.IsNullOrEmpty(this.itemTypeName) && this.itemTypeName.Equals(collectionItemTypeName, System.StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                if (this.primitiveItemType.IsSpatial())
                {
                    EdmPrimitiveTypeKind collectionItemPrimitiveKind = EdmCoreModel.Instance.GetPrimitiveTypeKind(collectionItemTypeName);
                    IEdmPrimitiveType collectionItemPrimitiveType = EdmCoreModel.Instance.GetPrimitiveType(collectionItemPrimitiveKind);

                    if (this.itemTypeDerivedFromCollectionValue)
                    {
                        // If the collection defines an item type, the collection item type has to be assignable to it.
                        if (this.primitiveItemType.IsAssignableFrom(collectionItemPrimitiveType))
                        {
                            return;
                        }
                    }
                    else
                    {
                        // If the collection does not define an item type, the collection items must have a common base type.
                        IEdmPrimitiveType commonBaseType = EdmLibraryExtensions.GetCommonBaseType(this.primitiveItemType, collectionItemPrimitiveType);
                        if (commonBaseType != null)
                        {
                            this.primitiveItemType = commonBaseType;
                            this.itemTypeName = commonBaseType.FullTypeName();
                            return;
                        }
                    }
                }

                throw new ODataException(Strings.CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName(collectionItemTypeName, this.itemTypeName));
            }
            else
            {
                // Since we do not support type inheritance for complex types, comparison of the type names is sufficient
                if (string.CompareOrdinal(this.itemTypeName, collectionItemTypeName) != 0)
                {
                    throw new ODataException(Strings.CollectionWithoutExpectedTypeValidator_IncompatibleItemTypeName(collectionItemTypeName, this.itemTypeName));
                }
            }
        }
    }
}

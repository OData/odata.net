//---------------------------------------------------------------------
// <copyright file="EdmCollectionTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a reference to an EDM collection type.
    /// </summary>
    public class EdmCollectionTypeReference : EdmTypeReference, IEdmCollectionTypeReference
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmCollectionTypeReference"/> class.
        /// </summary>
        /// <param name="collectionType">The type definition this reference refers to.</param>
        public EdmCollectionTypeReference(IEdmCollectionType collectionType)
            : base(collectionType, GetIsNullable(collectionType))
        {
        }

        private static bool GetIsNullable(IEdmCollectionType collectionType)
        {
            // check if the member type is entity, if yes, pass in default value (true),
            //   as per spec: A navigation property whose Type attribute specifies a collection MUST NOT
            //   specify a value for the Nullable attribute as the collection always exists, it may just be empty.
            // else replace with the nullable in member type reference
            IEdmTypeReference elementType = collectionType.ElementType;
            if (elementType == null)
            {
                return true;
            }

            IEdmEntityTypeReference entityReference = elementType as IEdmEntityTypeReference;
            return entityReference != null || collectionType.ElementType.IsNullable;
        }
    }
}
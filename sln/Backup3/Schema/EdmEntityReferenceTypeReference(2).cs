//---------------------------------------------------------------------
// <copyright file="EdmEntityReferenceTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a reference to an EDM entity reference type.
    /// </summary>
    public class EdmEntityReferenceTypeReference : EdmTypeReference, IEdmEntityReferenceTypeReference
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmEntityReferenceTypeReference"/> class.
        /// </summary>
        /// <param name="entityReferenceType">The definition referred to by this reference.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        public EdmEntityReferenceTypeReference(IEdmEntityReferenceType entityReferenceType, bool isNullable)
            : base(entityReferenceType, isNullable)
        {
        }

        /// <summary>
        /// Gets the entity reference definition to which this type refers.
        /// </summary>
        public IEdmEntityReferenceType EntityReferenceDefinition
        {
            get { return (IEdmEntityReferenceType)Definition; }
        }
    }
}

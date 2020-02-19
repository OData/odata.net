//---------------------------------------------------------------------
// <copyright file="EdmCollectionType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a definition of an EDM collection type.
    /// </summary>
    public class EdmCollectionType : EdmType, IEdmCollectionType
    {
        private readonly IEdmTypeReference elementType;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmCollectionType"/> class.
        /// </summary>
        /// <param name="elementType">The type of the elements in this collection.</param>
        public EdmCollectionType(IEdmTypeReference elementType)
        {
            EdmUtil.CheckArgumentNull(elementType, "elementType");
            this.elementType = elementType;
        }

        /// <summary>
        /// Gets the kind of this type.
        /// </summary>
        public override EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.Collection; }
        }

        /// <summary>
        /// Gets the element type of this collection type.
        /// </summary>
        public IEdmTypeReference ElementType
        {
            get { return this.elementType; }
        }
    }
}

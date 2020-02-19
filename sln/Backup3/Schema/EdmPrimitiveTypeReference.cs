//---------------------------------------------------------------------
// <copyright file="EdmPrimitiveTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a reference to an EDM primitive type.
    /// </summary>
    public class EdmPrimitiveTypeReference : EdmTypeReference, IEdmPrimitiveTypeReference
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmPrimitiveTypeReference"/> class.
        /// </summary>
        /// <param name="definition">The type this reference refers to.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        public EdmPrimitiveTypeReference(IEdmPrimitiveType definition, bool isNullable)
            : base(definition, isNullable)
        {
        }
    }
}

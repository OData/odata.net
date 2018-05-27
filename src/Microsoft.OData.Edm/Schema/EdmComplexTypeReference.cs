//---------------------------------------------------------------------
// <copyright file="EdmComplexTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a reference to an EDM complex type.
    /// </summary>
    public class EdmComplexTypeReference : EdmTypeReference, IEdmComplexTypeReference
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmComplexTypeReference"/> class.
        /// </summary>
        /// <param name="complexType">The type definition this reference refers to.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        public EdmComplexTypeReference(IEdmComplexType complexType, bool isNullable)
            : base(complexType, isNullable)
        {
        }
    }
}

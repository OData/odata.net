//---------------------------------------------------------------------
// <copyright file="EdmEnumTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a reference to an EDM enumeration type.
    /// </summary>
    public class EdmEnumTypeReference : EdmTypeReference, IEdmEnumTypeReference
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmEnumTypeReference"/> class.
        /// </summary>
        /// <param name="enumType">The definition refered to by this reference.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        public EdmEnumTypeReference(IEdmEnumType enumType, bool isNullable)
            : base(enumType, isNullable)
        {
        }
    }
}

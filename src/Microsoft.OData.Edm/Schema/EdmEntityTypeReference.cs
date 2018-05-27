//---------------------------------------------------------------------
// <copyright file="EdmEntityTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a reference to an EDM entity type.
    /// </summary>
    public class EdmEntityTypeReference : EdmTypeReference, IEdmEntityTypeReference
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmEntityTypeReference"/> class.
        /// </summary>
        /// <param name="entityType">The definition refered to by this reference.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        public EdmEntityTypeReference(IEdmEntityType entityType, bool isNullable)
            : base(entityType, isNullable)
        {
        }
    }
}

//---------------------------------------------------------------------
// <copyright file="EdmUntypedTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a reference to an EDM Untyped type.
    /// </summary>
    public class EdmUntypedTypeReference : EdmTypeReference, IEdmUntypedTypeReference
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="definition">IEdmUntypedType definition.</param>
        public EdmUntypedTypeReference(IEdmUntypedType definition)
            : this(definition, true)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="definition">IEdmUntypedType definition.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        public EdmUntypedTypeReference(IEdmUntypedType definition, bool isNullable)
            : base(definition, isNullable)
        {
        }
    }
}

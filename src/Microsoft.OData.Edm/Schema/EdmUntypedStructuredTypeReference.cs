//---------------------------------------------------------------------
// <copyright file="EdmUntypedStructuredTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a reference to an EDM Untyped type.
    /// </summary>
    public class EdmUntypedStructuredTypeReference : EdmTypeReference, IEdmUntypedTypeReference, IEdmStructuredTypeReference
    {
        /// <summary>
        /// Returns a static instance of a nullable untyped structured type reference.
        /// </summary>
        public static readonly IEdmStructuredTypeReference NullableTypeReference = new EdmUntypedStructuredTypeReference(EdmUntypedStructuredType.Instance, true);

        /// <summary>
        /// Returns a static instance of a non-nullable untyped structured type reference.
        /// </summary>
        public static readonly IEdmStructuredTypeReference NonNullableTypeReference = new EdmUntypedStructuredTypeReference(EdmUntypedStructuredType.Instance, false);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="definition">IEdmStructuredType definition.</param>
        public EdmUntypedStructuredTypeReference(IEdmStructuredType definition)
            : this(definition, true)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="definition">IEdmStructuredType definition.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        public EdmUntypedStructuredTypeReference(IEdmStructuredType definition, bool isNullable)
            : base(definition, isNullable)
        {
        }
    }
}
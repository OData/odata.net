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
        /// Constructor
        /// </summary>
        /// <param name="definition">IEdmStructuredType definition.</param>
        public EdmUntypedStructuredTypeReference(IEdmStructuredType definition)
            : base(definition, true)
        {
        }
    }
}
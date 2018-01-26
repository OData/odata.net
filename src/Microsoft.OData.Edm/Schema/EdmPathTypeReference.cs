//---------------------------------------------------------------------
// <copyright file="EdmPathTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a reference to an EDM Path type.
    /// </summary>
    public class EdmPathTypeReference : EdmTypeReference, IEdmPathTypeReference
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="definition">IEdmPathType definition.</param>
        /// <param name="isNullable">nullable or not.</param>
        public EdmPathTypeReference(IEdmPathType definition, bool isNullable)
            : base(definition, isNullable)
        {
        }
    }
}

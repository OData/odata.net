//---------------------------------------------------------------------
// <copyright file="EdmTypeDefinitionReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents a reference to an EDM type definition.
    /// </summary>
    public class EdmTypeDefinitionReference : EdmTypeReference, IEdmTypeDefinitionReference
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmTypeDefinitionReference"/> class.
        /// </summary>
        /// <param name="typeDefinition">The definition refered to by this reference.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        public EdmTypeDefinitionReference(IEdmTypeDefinition typeDefinition, bool isNullable)
            : base(typeDefinition, isNullable)
        {
        }
    }
}

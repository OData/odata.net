//---------------------------------------------------------------------
// <copyright file="EdmDictionaryOfStringStringTypeReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a reference to an EDM dictionary of string string type.
    /// </summary>
    public class EdmDictionaryOfStringStringTypeReference : EdmPrimitiveTypeReference
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmDictionaryOfStringStringTypeReference"/> class.
        /// </summary>
        /// <param name="definition">The type this reference refers to.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        public EdmDictionaryOfStringStringTypeReference(IEdmPrimitiveType definition, bool isNullable)
            : base(definition, isNullable)
        {
            EdmUtil.CheckArgumentNull(definition, "definition");
        }
    }
}

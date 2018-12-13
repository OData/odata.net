//---------------------------------------------------------------------
// <copyright file="EdmOperationTypeDefinitionReturnType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents operation return type as a reference to an EDM complex type.
    /// </summary>
    internal class EdmOperationTypeDefinitionReturnType : EdmTypeDefinitionReference, IEdmOperationReturnType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmOperationTypeDefinitionReturnType"/> class.
        /// </summary>
        /// <param name="declaringOperation">Declaring operation of the return type.</param>
        /// <param name="type">The return type of the operation.</param>
        public EdmOperationTypeDefinitionReturnType(IEdmOperation declaringOperation, IEdmTypeDefinitionReference type)
            : base(type.TypeDefinition(), type.IsNullable)
        {
            EdmUtil.CheckArgumentNull(declaringOperation, "declaringOperation");

            DeclaringOperation = declaringOperation;
            Type = this;
        }

        public IEdmOperation DeclaringOperation { get; set; }

        public IEdmTypeReference Type { get; set; }
    }
}

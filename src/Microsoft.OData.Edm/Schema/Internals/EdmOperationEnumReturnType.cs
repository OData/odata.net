//---------------------------------------------------------------------
// <copyright file="EdmOperationEnumReturnType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents operation return type as a reference to an EDM enum type.
    /// </summary>
    internal class EdmOperationEnumReturnType : EdmEnumTypeReference, IEdmOperationReturnType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmOperationEnumReturnType"/> class.
        /// </summary>
        /// <param name="declaringOperation">Declaring operation of the return type.</param>
        /// <param name="type">The return type of the operation.</param>
        public EdmOperationEnumReturnType(IEdmOperation declaringOperation, IEdmEnumTypeReference type)
            : base(type.EnumDefinition(), type.IsNullable)
        {
            EdmUtil.CheckArgumentNull(declaringOperation, "declaringOperation");

            DeclaringOperation = declaringOperation;
            Type = this;
        }

        public IEdmOperation DeclaringOperation { get; set; }

        public IEdmTypeReference Type { get; set; }
    }
}

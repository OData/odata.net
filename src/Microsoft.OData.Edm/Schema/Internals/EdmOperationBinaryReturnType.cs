//---------------------------------------------------------------------
// <copyright file="EdmOperationBinaryReturnType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents operation return type as a reference to an EDM binary type.
    /// </summary>
    internal class EdmOperationBinaryReturnType : EdmBinaryTypeReference, IEdmOperationReturnType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmOperationBinaryReturnType"/> class.
        /// </summary>
        /// <param name="declaringOperation">Declaring operation of the return type.</param>
        /// <param name="type">The return type of the operation.</param>
        public EdmOperationBinaryReturnType(IEdmOperation declaringOperation, IEdmBinaryTypeReference type)
            : base(type.PrimitiveDefinition(), type.IsNullable)
        {
            EdmUtil.CheckArgumentNull(declaringOperation, "declaringOperation");

            DeclaringOperation = declaringOperation;
            Type = this;
        }

        public IEdmOperation DeclaringOperation { get; set; }

        public IEdmTypeReference Type { get; set; }
    }
}

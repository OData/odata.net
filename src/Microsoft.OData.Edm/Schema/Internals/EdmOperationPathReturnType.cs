//---------------------------------------------------------------------
// <copyright file="EdmOperationPathReturnType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents operation return type as a reference to an EDM path type.
    /// </summary>
    internal class EdmOperationPathReturnType : EdmPathTypeReference, IEdmOperationReturnType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmOperationPathReturnType"/> class.
        /// </summary>
        /// <param name="declaringOperation">Declaring operation of the return type.</param>
        /// <param name="type">The return type of the operation.</param>
        public EdmOperationPathReturnType(IEdmOperation declaringOperation, IEdmPathTypeReference type)
            : base(type.PathDefinition(), type.IsNullable)
        {
            EdmUtil.CheckArgumentNull(declaringOperation, "declaringOperation");

            DeclaringOperation = declaringOperation;
            Type = this;
        }

        public IEdmOperation DeclaringOperation { get; set; }

        public IEdmTypeReference Type { get; set; }
    }
}

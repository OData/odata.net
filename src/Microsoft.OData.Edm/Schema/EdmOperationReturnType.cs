//---------------------------------------------------------------------
// <copyright file="EdmOperationReturnType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents an EDM operation return type.
    /// </summary>
    public class EdmOperationReturnType : EdmTypeReference, IEdmOperationReturnType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmOperationReturnType"/> class.
        /// </summary>
        /// <param name="declaringOperation">Declaring operation of the return type.</param>
        /// <param name="type">The return type of the operation.</param>
        public EdmOperationReturnType(IEdmOperation declaringOperation, IEdmTypeReference type)
            : base(type.Definition, type.IsNullable)
        {
            EdmUtil.CheckArgumentNull(declaringOperation, "declaringOperation");
            EdmUtil.CheckArgumentNull(type, "type");

            this.DeclaringOperation = declaringOperation;
            this.Type = type;
        }

        /// <summary>
        /// Gets the operation that declared this return type.
        /// </summary>
        public IEdmOperation DeclaringOperation { get; }

        /// <summary>
        /// Gets the type reference of this return type.
        /// </summary>
        public IEdmTypeReference Type { get; }
    }
}

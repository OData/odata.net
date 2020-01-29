//---------------------------------------------------------------------
// <copyright file="EdmOperationReturn.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents an EDM operation return.
    /// Be noted: it is marked as internal class because it's not allowed to create an instance from end user.
    /// </summary>
    internal class EdmOperationReturn : EdmElement, IEdmOperationReturn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmOperationReturn"/> class.
        /// </summary>
        /// <param name="declaringOperation">Declaring operation of the return.</param>
        /// <param name="type">The return type of the return.</param>
        public EdmOperationReturn(IEdmOperation declaringOperation, IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(declaringOperation, "declaringOperation");
            EdmUtil.CheckArgumentNull(type, "type");

            this.Type = type;
            this.DeclaringOperation = declaringOperation;
        }

        /// <summary>
        /// Gets the return type of this return.
        /// </summary>
        public IEdmTypeReference Type { get; private set; }

        /// <summary>
        /// Gets the operation that declared this return.
        /// </summary>
        public IEdmOperation DeclaringOperation { get; private set; }
    }
}

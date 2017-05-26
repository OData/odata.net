//---------------------------------------------------------------------
// <copyright file="EdmOperationParameter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents an EDM operation parameter.
    /// </summary>
    public class EdmOperationParameter : EdmNamedElement, IEdmOperationParameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmOperationParameter"/> class.
        /// </summary>
        /// <param name="declaringOperation">Declaring function of the parameter.</param>
        /// <param name="name">Name of the parameter.</param>
        /// <param name="type">Type of the parameter.</param>
        public EdmOperationParameter(IEdmOperation declaringOperation, string name, IEdmTypeReference type)
            : base(name)
        {
            EdmUtil.CheckArgumentNull(declaringOperation, "declaringFunction");
            EdmUtil.CheckArgumentNull(name, "name");
            EdmUtil.CheckArgumentNull(type, "type");

            this.Type = type;
            this.DeclaringOperation = declaringOperation;
        }

        /// <summary>
        /// Gets the type of this parameter.
        /// </summary>
        public IEdmTypeReference Type { get; private set; }

        /// <summary>
        /// Gets the operation that declared this parameter.
        /// </summary>
        public IEdmOperation DeclaringOperation { get; private set; }
    }
}
